using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TonKhos;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Services.CauHinh;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject.ToaThuocMau;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.TonKhos
{
    public partial class TonKhoService
    {
        public List<DuocPhamSapHetHanGridVo> GetDuocPhamSapHetHan(string search)
        {
            var settings = _cauhinhService.LoadSetting<CauHinhBaoCao>();
            //DateTime dayHetHan = DateTime.Now.AddDays(settings.DuocPhamSapHetHanNgayHetHan);
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            DateTime dayHetHan = DateTime.Now;
            var queryString = JsonConvert.DeserializeObject<DuocPhamSapHetHanSearchGridVoItem>(search);
            var query = _nhapKhoDuocPhamRepository.TableNoTracking
               .Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DinhMucDuocPhamTonKhos).ThenInclude(x => x.KhoDuocPham)
               .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.KhoDuocPhamViTri).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
               .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
               .Select(s => new DuocPhamSapHetHanGridVo
               {

                   Id = s.Id,
                   TenDuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                   HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                   PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                   TenHoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                   DonViTinh = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                   TenKho = s.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                   NgayHetHanHienThi = s.HanSuDung.ApplyFormatDate(),
                   NgayHetHan = s.HanSuDung,
                   ViTri = s.KhoDuocPhamViTri.Ten,
                   VitriId = s.KhoDuocPhamViTri.Id,
                   KhoId = s.NhapKhoDuocPhams.KhoDuocPhams.Id,
                   SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                   SoNgayTruocKhiHetHan = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(p2 => p2.KhoId == s.NhapKhoDuocPhams.KhoId && p2.SoNgayTruocKhiHetHan != null) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.First(p2 => p2.KhoId == s.NhapKhoDuocPhams.KhoId).SoNgayTruocKhiHetHan.GetValueOrDefault() : settings.DuocPhamSapHetHanNgayHetHan,
                   SoLo = s.Solo,
                   NhapKhoDuocPhamId = s.NhapKhoDuocPhamId,
                   DuocPhamId = s.DuocPhamBenhVienId,
                   MaDuocPham = s.DuocPhamBenhViens.Ma,
                   DonGiaNhap = s.DonGiaNhap,

               })
               .Distinct()
               .Where(x => x.NgayHetHan >= DateTime.Now.Date && x.NgayHetHan <= dayHetHan.AddDays(x.SoNgayTruocKhiHetHan).Date && x.SoLuongTon > 0)
               .GroupBy(item => new
               {
                   item.DuocPhamId,
                   item.KhoId,
                   item.DonGiaNhap,
                   item.SoLo,
                   item.NgayHetHan,
                   item.SoNgayTruocKhiHetHan,
               })
              .Select(item => new DuocPhamSapHetHanGridVo
              {
                  Id = item.First().Id,
                  TenDuocPham = item.First().TenDuocPham,
                  HamLuong = item.First().HamLuong,
                  PhanLoai = item.First().PhanLoai,
                  TenHoatChat = item.First().TenHoatChat,
                  DonViTinh = item.First().DonViTinh,
                  TenKho = item.First().TenKho,
                  NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                  NgayHetHan = item.First().NgayHetHan,
                  ViTri = item.First().ViTri,
                  VitriId = item.First().VitriId,
                  KhoId = item.First().KhoId,
                  SoLuongTon = item.Sum(d => d.SoLuongTon),//s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongDaXuat) : 0,
                   SoNgayTruocKhiHetHan = item.First().SoNgayTruocKhiHetHan,
                  MaDuocPham = item.First().MaDuocPham,
                  SoLo = item.First().SoLo,
                  NhapKhoDuocPhamId = item.First().NhapKhoDuocPhamId,
                  DuocPhamId = item.First().DuocPhamId,
                  DonGiaNhap = item.First().DonGiaNhap,
              });


            if (queryString.KhoId != 0)
            {
                query = query.Where(x => x.KhoId.Equals(queryString.KhoId));
            }
            if (!string.IsNullOrEmpty(queryString.DuocPham))
            {
                query = query.Where(p => p.TenDuocPham.TrimEnd().TrimStart().ToLower().Contains(queryString.DuocPham.TrimEnd().TrimStart().ToLower()));
            }

            return query.ToList();
        }
        public string GeHTML(string search)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var result = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoDuocPhamSapHetHan")).FirstOrDefault();
            var settings = _cauhinhService.LoadSetting<CauHinhBaoCao>();
            var duocPham = new List<DuocPhamSapHetHanGridVo>();
            var duocPhamquery = new List<DuocPhamSapHetHanGridVo>();
            var duocPhamChi = new DuocPhamSapHetHanGridVo();
            //DateTime dayHetHan = DateTime.Now.AddDays(settings.DuocPhamSapHetHanNgayHetHan);
            DateTime dayHetHan = DateTime.Now;
            var queryString = JsonConvert.DeserializeObject<DuocPhamSapHetHanSearchGridVoItem>(search);
            var query = _nhapKhoDuocPhamRepository.TableNoTracking
                 .Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DinhMucDuocPhamTonKhos).ThenInclude(x => x.KhoDuocPham)
                 .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.KhoDuocPhamViTri).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                 .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                 .Select(s => new DuocPhamSapHetHanGridVo
                 {

                     Id = s.Id,
                     TenDuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                     HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                     PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                     TenHoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                     DonViTinh = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                     TenKho = s.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                     NgayHetHanHienThi = s.HanSuDung.ApplyFormatDate(),
                     NgayHetHan = s.HanSuDung,
                     ViTri = s.KhoDuocPhamViTri.Ten,
                     VitriId = s.KhoDuocPhamViTri.Id,
                     KhoId = s.NhapKhoDuocPhams.KhoDuocPhams.Id,
                     SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                     SoNgayTruocKhiHetHan = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(p2 => p2.KhoId == s.NhapKhoDuocPhams.KhoId && p2.SoNgayTruocKhiHetHan != null) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.First(p2 => p2.KhoId == s.NhapKhoDuocPhams.KhoId).SoNgayTruocKhiHetHan.GetValueOrDefault() : settings.DuocPhamSapHetHanNgayHetHan,
                     SoLo = s.Solo,
                     NhapKhoDuocPhamId = s.NhapKhoDuocPhamId,
                     DuocPhamId = s.DuocPhamBenhVienId,
                     MaDuocPham = s.DuocPhamBenhViens.Ma,
                     DonGiaNhap = s.DonGiaNhap,
                 })
                 .Distinct()
                 .Where(x => x.NgayHetHan >= DateTime.Now.Date && x.NgayHetHan <= dayHetHan.AddDays(x.SoNgayTruocKhiHetHan).Date && x.SoLuongTon > 0)
                 .GroupBy(item => new
                 {
                     item.DuocPhamId,
                     item.KhoId,
                     item.DonGiaNhap,
                     item.SoLo,
                     item.NgayHetHan,
                     item.SoNgayTruocKhiHetHan,
                 })
                .Select(item => new DuocPhamSapHetHanGridVo
                {
                    Id = item.First().Id,
                    TenDuocPham = item.First().TenDuocPham,
                    HamLuong = item.First().HamLuong,
                    PhanLoai = item.First().PhanLoai,
                    TenHoatChat = item.First().TenHoatChat,
                    DonViTinh = item.First().DonViTinh,
                    TenKho = item.First().TenKho,
                    NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                    NgayHetHan = item.First().NgayHetHan,
                    ViTri = item.First().ViTri,
                    VitriId = item.First().VitriId,
                    KhoId = item.First().KhoId,
                    SoLuongTon = item.Sum(d => d.SoLuongTon),//s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongDaXuat) : 0,
                    SoNgayTruocKhiHetHan = item.First().SoNgayTruocKhiHetHan,
                    MaDuocPham = item.First().MaDuocPham,
                    SoLo = item.First().SoLo,
                    NhapKhoDuocPhamId = item.First().NhapKhoDuocPhamId,
                    DuocPhamId = item.First().DuocPhamId,
                    DonGiaNhap = item.First().DonGiaNhap,
                });


            if (queryString.KhoId != 0)
            {
                query = query.Where(x => x.KhoId.Equals(queryString.KhoId));
            }
            if (!string.IsNullOrEmpty(queryString.DuocPham))
            {
                query = query.Where(p => p.TenDuocPham.TrimEnd().TrimStart().ToLower().Contains(queryString.DuocPham.TrimEnd().TrimStart().ToLower()));
            }
            duocPhamquery = query.ToList();
            for (int i = 0; i < duocPhamquery.Count(); i++)
            {
                if (i == 0)
                {
                    if (string.IsNullOrEmpty(duocPhamquery.ToList()[i].ViTri))
                    {
                        //soLuongTon = duocPhamquery.Where(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.ViTri == null
                        // && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan)).Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamquery.ToList()[i];
                        //duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        duocPhamquery.RemoveAll(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.ViTri == null
                                                  && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan));
                    }
                    else
                    {
                        var aa = duocPhamquery.Where(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.ViTri.Equals(duocPhamquery.ToList()[i].ViTri)
                                                  && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan)).ToList();
                        // soLuongTon = aa.Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamquery.ToList()[i];
                        // duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        foreach (var item in aa)
                        {
                            duocPhamquery.Remove(item);
                        }

                        //duocPhamquery.RemoveAll(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.VitriId.Equals(duocPhamquery.ToList()[i].VitriId)
                        //                          && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan));
                    }

                    i = -1;
                }


            }
            string finalresult = String.Empty;
            for (int i = 0; i < duocPham.ToList().Count(); i++)
            {
                finalresult = finalresult + "<tr style='border: 1px solid #020000;text-align: center; '><td style = 'padding:5px;border: 1px solid #020000;'>" + (i + 1) + "</td><td style=''border: 1px solid #020000;'>" + query.ToList()[i].TenKho
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].MaDuocPham + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].TenDuocPham + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].HamLuong + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].TenHoatChat + "</td>"

                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].DonViTinh + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].SoLo + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].ViTri + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].DonGiaNhap + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;text-align: right;'>" + duocPham.ToList()[i].SoLuongTon + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;text-align: right;'>" + duocPham.ToList()[i].ThanhTien + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + duocPham.ToList()[i].NgayHetHanHienThi + "</td>" + "</tr>";

            }
            string ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;

            var data = new DataVaLueHTML
            {
                TemplateDuocPham = finalresult,
                Ngay = ngayThangHientai
            };
            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }

        public async Task<GridDataSource> GetDataForGridNhapXuatTonAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;
            var searchTerm = string.Empty;
            var queryString = new NhapXuatTonKhoGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<NhapXuatTonKhoGridVoItem>(queryInfo.AdditionalSearchString);

                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
                searchTerm = queryString.Description != null ? queryString.Description.Trim() : searchTerm;
            }
            if (queryString.KhoId == 0)
            {
                return new GridDataSource { Data = new VatTuTonKhoNhapXuatGridVo[0], TotalRowCount = 0 };
            }

            //var query = _nhapKhoDuocPhamRepository.TableNoTracking
            //    .Where(x => x.KhoId == khoDuocPhamId && x.NgayNhap.Date <= endDate.Date)
            //    .SelectMany(x => x.NhapKhoDuocPhamChiTiets)
            //    .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
            //    .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
            //    .Include(x => x.XuatKhoDuocPhamChiTietViTris)
            //    .GroupBy(
            //        o => new
            //        {
            //            o.DuocPhamBenhViens.DuocPham.Id,
            //            DuocPham = o.DuocPhamBenhViens.DuocPham.Ten,
            //            HamLuong = o.DuocPhamBenhViens.DuocPham.HamLuong,
            //            PhanLoai = o.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && o.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, o.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
            //            o.DuocPhamBenhViens.DuocPham.HoatChat,
            //            DonViTinhDisplay = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten
            //        }, o => o,
            //        (k, v) => new NhapXuatTonKhoGridVo
            //        {
            //            Id = k.Id,
            //            DuocPham = k.DuocPham,
            //            HamLuong = k.HamLuong,
            //            PhanLoai = k.PhanLoai,
            //            HoatChat = k.HoatChat,
            //            DonViTinhDisplay = k.DonViTinhDisplay,
            //            TonDauKy = v.Where(o => o.NgayNhap.Date <= startDate.Date)
            //                        .Select(o => o.SoLuongNhap).DefaultIfEmpty(0).Sum() - v.SelectMany(o => o.XuatKhoDuocPhamChiTietViTris).Where(p => p.NgayXuat != null && p.NgayXuat.Value.Date <= startDate.Date).Select(p => p.SoLuongXuat).DefaultIfEmpty(0).Sum(),
            //            NhapTrongKy = v.Where(o => startDate.Date <= o.NgayNhap.Date && o.NgayNhap.Date <= endDate.Date).Select(o => o.SoLuongNhap).DefaultIfEmpty(0).Sum(),
            //            XuatTrongKy = v.SelectMany(o => o.XuatKhoDuocPhamChiTietViTris).Where(o => o.NgayXuat != null && startDate.Date <= o.NgayXuat.Value.Date && o.NgayXuat.Value.Date <= endDate.Date).Select(p => p.SoLuongXuat).DefaultIfEmpty(0).Sum(),
            //            TonCuoiKy = v.Where(o => o.NgayNhap.Date <= endDate.Date).Select(o => o.SoLuongNhap).DefaultIfEmpty(0).Sum() - v.SelectMany(o => o.XuatKhoDuocPhamChiTietViTris).Where(p => p.NgayXuat != null && p.NgayXuat.Value.Date <= endDate.Date).Select(p => p.SoLuongXuat).DefaultIfEmpty(0).Sum()
            //        });

            //query = query.ApplyLike(searchTerm, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhDisplay);

            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == queryString.KhoId && o.NgayNhap <= endDate)
                    .Select(o => new BaoCaoChiTietXuatNhapTonGridVo()
                    {
                        Id = o.Id,
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        Ma = o.DuocPhamBenhViens.Ma,
                        Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        HamLuong = o.DuocPhamBenhViens.DuocPham.HamLuong,
                        HoatChat = o.DuocPhamBenhViens.DuocPham.HoatChat,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLo = o.Solo,
                        DonGiaNhap = o.DonGiaNhap,
                        VAT = o.VAT,
                        NgayNhapXuat = o.NgayNhap,
                        LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                        //Nhom = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                        NhomId = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ApplyLike(searchTerm, g => g.Ten, g => g.HoatChat, g => g.HamLuong, g => g.Ma).ToList();

            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryString.KhoId
                            //&& ((o.NgayXuat != null && o.NgayXuat <= endDate) ||
                            //    (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= endDate))
                            )
                .Select(o => new BaoCaoChiTietXuatNhapTonGridVo
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                    HoatChat = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HoatChat,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    DonGiaNhap = o.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                    VAT = o.NhapKhoDuocPhamChiTiet.VAT,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    LaDuocPhamBHYT = o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                    //Nhom = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    NhomId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ApplyLike(searchTerm, g => g.Ten, g => g.HoatChat, g => g.HamLuong, g => g.Ma).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat.Where(o=>o.NgayNhapXuat <= endDate)).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => o.DuocPhamBenhVienId);
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var dataReturn = new List<NhapXuatTonKhoGridVo>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < startDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum();
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= startDate).ToList();
                var nhapTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var xuatTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var tonCuoi = tonDau + nhapTrongKy - xuatTrongKy;
                dataReturn.Add(new NhapXuatTonKhoGridVo
                {
                    Id = xuatNhapDuocPham.Key,
                    DuocPham = xuatNhapDuocPham.First().Ten,
                    Ma = xuatNhapDuocPham.First().Ma,
                    HamLuong = xuatNhapDuocPham.First().HamLuong,
                    PhanLoai = xuatNhapDuocPham.First().Nhom,
                    HoatChat = xuatNhapDuocPham.First().HoatChat,
                    DonViTinhDisplay = xuatNhapDuocPham.First().DVT,
                    TenDuocPhamBenhVienPhanNhom = xuatNhapDuocPham.First().NhomId != null ? duocPhamBenhVienPhanNhoms.FirstOrDefault(o=>o.Id == xuatNhapDuocPham.First().NhomId)?.Ten : "Các thuốc khác",
                    DuocPhamBenhVienPhanNhomId = xuatNhapDuocPham.First().NhomId,
                    TonDauKy = tonDau,
                    NhapTrongKy = nhapTrongKy,
                    XuatTrongKy = xuatTrongKy,
                    TonCuoiKy = tonCuoi
                });
            }

            var countTask = queryInfo.LazyLoadPage == true ? 0 : dataReturn.Count();
            var queryTask = dataReturn.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };

        }

        public async Task<GridDataSource> GetTotalPageForGridNhapXuatTonAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;
            var searchTerm = string.Empty;
            var queryString = new NhapXuatTonKhoGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<NhapXuatTonKhoGridVoItem>(queryInfo.AdditionalSearchString);

                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
                searchTerm = queryString.Description != null ? queryString.Description.Trim() : searchTerm;
            }
            if (queryString.KhoId == 0)
            {
                return new GridDataSource { TotalRowCount = 0 };
            }

            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == queryString.KhoId && o.NgayNhap <= endDate)
                .Select(o => new BaoCaoChiTietXuatNhapTonGridVo()
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                    Ma = o.DuocPhamBenhViens.Ma,
                    Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = o.DuocPhamBenhViens.DuocPham.HamLuong,
                    HoatChat = o.DuocPhamBenhViens.DuocPham.HoatChat,
                    DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.Solo,
                    DonGiaNhap = o.DonGiaNhap,
                    VAT = o.VAT,
                    NgayNhapXuat = o.NgayNhap,
                    LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                    //Nhom = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    SLNhap = o.SoLuongNhap,
                    SLXuat = 0
                }).ApplyLike(searchTerm, g => g.Ten, g => g.HoatChat, g => g.HamLuong).ToList();

            var countTask = allDataNhap.GroupBy(o => o.DuocPhamBenhVienId).Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        public string GetXuatNhapTonKhoHTML(string search)
        {
            var result = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoDuocPhamXuatNhapTon")).FirstOrDefault();
            var khoDuocPhamId = BaseRepository.TableNoTracking.FirstOrDefault().Id;
            var startDate = new DateTime();
            startDate = DateTime.MinValue;
            var endDate = new DateTime();
            endDate = DateTime.Now;
            var searchString = string.Empty;
            var sortString = string.Empty;
            if (!string.IsNullOrEmpty(search))
            {
                var queryString = JsonConvert.DeserializeObject<NhapXuatTonKhoGridVoItem>(search);
                if (queryString.KhoId != 0)
                {
                    khoDuocPhamId = queryString.KhoId;
                }
                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
                searchString = queryString.Description;
                sortString = queryString.SortString;
            }
            var getTenKho = _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == khoDuocPhamId).Select(p => p.Ten).FirstOrDefault();

            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == khoDuocPhamId && o.NgayNhap <= endDate)
                    .Select(o => new BaoCaoChiTietXuatNhapTonGridVo()
                    {
                        Id = o.Id,
                        DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                        Ma = o.DuocPhamBenhViens.Ma,
                        Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        HamLuong = o.DuocPhamBenhViens.DuocPham.HamLuong,
                        HoatChat = o.DuocPhamBenhViens.DuocPham.HoatChat,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLo = o.Solo,
                        DonGiaNhap = o.DonGiaNhap,
                        VAT = o.VAT,
                        NgayNhapXuat = o.NgayNhap,
                        LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                        Nhom = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                        NhomId = o.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ApplyLike(searchString, g => g.Ten, g => g.HoatChat, g => g.HamLuong, g => g.Ma).ToList();

            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == khoDuocPhamId
                            && ((o.NgayXuat != null && o.NgayXuat <= endDate) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= endDate)))
                .Select(o => new BaoCaoChiTietXuatNhapTonGridVo
                {
                    Id = o.Id,
                    DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                    Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                    HoatChat = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HoatChat,
                    DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLo = o.NhapKhoDuocPhamChiTiet.Solo,
                    DonGiaNhap = o.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                    VAT = o.NhapKhoDuocPhamChiTiet.VAT,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    LaDuocPhamBHYT = o.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT,
                    Nhom = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId != null ? o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten : "Các thuốc khác",
                    NhomId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ApplyLike(searchString, g => g.Ten, g => g.HoatChat, g => g.HamLuong, g => g.Ma).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).ToList();

            var allDataGroup = allDataNhapXuat.GroupBy(o => o.DuocPhamBenhVienId);

            var dataReturn = new List<NhapXuatTonKhoGridVo>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < startDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum();
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= startDate).ToList();
                var nhapTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var xuatTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var tonCuoi = tonDau + nhapTrongKy - xuatTrongKy;
                dataReturn.Add(new NhapXuatTonKhoGridVo
                {
                    Id = xuatNhapDuocPham.Key,
                    DuocPham = xuatNhapDuocPham.First().Ten,
                    Ma = xuatNhapDuocPham.First().Ma,
                    HamLuong = xuatNhapDuocPham.First().HamLuong,
                    HoatChat = xuatNhapDuocPham.First().HoatChat,
                    DonViTinhDisplay = xuatNhapDuocPham.First().DVT,
                    TenDuocPhamBenhVienPhanNhom = xuatNhapDuocPham.First().Nhom,
                    DuocPhamBenhVienPhanNhomId = xuatNhapDuocPham.First().NhomId,
                    TonDauKy = tonDau,
                    NhapTrongKy = nhapTrongKy,
                    XuatTrongKy = xuatTrongKy,
                    TonCuoiKy = tonCuoi
                });
            }

            var duocPhamBenhVienPhanNhoms = dataReturn.Select(z => new { z.DuocPhamBenhVienPhanNhomId, z.TenDuocPhamBenhVienPhanNhom }).Distinct().OrderBy(x => x.TenDuocPhamBenhVienPhanNhom).ToList();
            string finalresult = String.Empty;
            var STT = 1;
            foreach (var nhom in duocPhamBenhVienPhanNhoms)
            {
                var coHeaderGroup = true;
                foreach (var item in dataReturn)
                {
                    if (nhom.DuocPhamBenhVienPhanNhomId == item.DuocPhamBenhVienPhanNhomId)
                    {
                        if (coHeaderGroup)
                        {
                            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                          + "<td style='border: 1px solid #020000;text-align: left;' colspan='9'><b>" + nhom.TenDuocPhamBenhVienPhanNhom
                                          + "</b></tr>";
                            finalresult += headerBHYT;
                            coHeaderGroup = false;
                        }
                        finalresult = finalresult + "<tr style='border: 1px solid #020000;text-align: center;'>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DuocPham + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HoatChat + "</td>"
                                            + "<td style = 'border: 1px solid #020000;'>" + item.HamLuong + "</td>"
                                            + "<td style = 'border: 1px solid #020000;'>" + item.DonViTinhDisplay + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (item.TonDauKy == 0 ? "0" : item.TonDauKy.ApplyNumber()) + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (item.NhapTrongKy == 0 ? "0" : item.NhapTrongKy.ApplyNumber()) + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (item.XuatTrongKy == 0 ? "0" : item.XuatTrongKy.ApplyNumber()) + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (item.TonCuoiKy == 0 ? "0" : item.TonCuoiKy.ApplyNumber()) + "</td>"
                                            + "</tr>";
                        STT++;
                    }
                }
            }

            var data = new DataTitleXuatNhapTonKhoHTML
            {
                TemplateDuocPhamTonKho = finalresult,
                TenKho = getTenKho,
                StartDate = startDate.ApplyFormatDate(),
                EndDate = endDate.ApplyFormatDate(),
                NgayNow = DateTime.Now.Day.ConvertDateToString(),
                ThangNow = DateTime.Now.Month.ConvertMonthToString(),
                NamNow = DateTime.Now.Year.ConvertYearToString(),
            };
            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }

        public async Task<GridDataSource> GetDataForGridNhapXuatTonChiTietAsync(QueryInfo queryInfo)
        {
            var duocPhamId = _duocPhamRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            var khoId = BaseRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<NhatXuatTonKhoChiTietGridVoItem>(queryInfo.AdditionalSearchString);
                khoId = queryString.KhoId == 0 ? khoId : queryString.KhoId;
                duocPhamId = queryString.DuocPhamId;
                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
            }

            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == khoId && o.NgayNhap <= endDate && o.DuocPhamBenhVienId == duocPhamId)
                    .Select(o => new NhapXuatTonKhoDetailGridVo()
                    {
                        Id = o.Id,
                        NgayNhapXuat = o.NgayNhap,
                        NgayDisplay = o.NgayNhap.ApplyFormatDateTime(),
                        MaChungTu = o.NhapKhoDuocPhams.SoPhieu,
                        Nhap = o.SoLuongNhap,
                        Xuat = 0
                    }).ToList();

            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == khoId && o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == duocPhamId
                            && ((o.NgayXuat != null && o.NgayXuat <= endDate) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= endDate)))
                .Select(o => new NhapXuatTonKhoDetailGridVo
                {
                    Id = o.Id,
                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                    NgayDisplay = (o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat).ApplyFormatDateTime(),
                    MaChungTu = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
                    Nhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    Xuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.NgayNhapXuat).ToList();

            var tonDauKy = allDataNhapXuat.Where(o => o.NgayNhapXuat < startDate)
                .Select(o => o.Nhap - o.Xuat).DefaultIfEmpty(0).Sum();
            var allDataNhapXuatTuNgay = allDataNhapXuat.Where(o => o.NgayNhapXuat >= startDate).ToList();
            for (int i = 0; i < allDataNhapXuatTuNgay.Count; i++)
            {
                allDataNhapXuatTuNgay[i].STT = i + 1;
                if (i == 0)
                {
                    allDataNhapXuatTuNgay[i].Ton = tonDauKy + allDataNhapXuatTuNgay[i].Nhap - allDataNhapXuatTuNgay[i].Xuat;
                }
                else
                {
                    allDataNhapXuatTuNgay[i].Ton = allDataNhapXuatTuNgay[i - 1].Ton + allDataNhapXuatTuNgay[i].Nhap - allDataNhapXuatTuNgay[i].Xuat;
                }
            }

            return new GridDataSource { Data = allDataNhapXuatTuNgay.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = allDataNhapXuatTuNgay.Count };
        }

        public async Task<GridDataSource> GetTotalPageForGridNhapXuatTonChiTietAsync(QueryInfo queryInfo)
        {
            var duocPhamId = _duocPhamRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            var khoId = BaseRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<NhatXuatTonKhoChiTietGridVoItem>(queryInfo.AdditionalSearchString);
                khoId = queryString.KhoId == 0 ? khoId : queryString.KhoId;
                duocPhamId = queryString.DuocPhamId;
                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
            }
            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == khoId && o.NgayNhap >= startDate && o.NgayNhap <= endDate && o.DuocPhamBenhVienId == duocPhamId)
                    .Select(o => new NhapXuatTonKhoDetailGridVo()
                    {
                        Id = o.Id
                    }).ToList();

            var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                            o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == khoId && o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == duocPhamId
                            && ((o.NgayXuat != null && o.NgayXuat >= startDate && o.NgayXuat <= endDate) ||
                                (o.NgayXuat == null && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat >= startDate && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= endDate)))
                .Select(o => new NhapXuatTonKhoDetailGridVo
                {
                    Id = o.Id
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).ToList();

            return new GridDataSource { TotalRowCount = allDataNhapXuat.Count };
        }

        public async Task<ChiTietItem> GetChiTiet(ChiTietItem model)
        {
            var duocPhamName =
                (await _duocPhamRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.DuocPhamId))?.Ten;
            var khoName = (await _khoDuocPhamRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.KhoId))
                ?.Ten;
            model.KhoDisplay = khoName;
            model.DuocPhamDisplay = duocPhamName;
            return model;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {

            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var khoDuocPhamId = (long)0;
            var queryString = new TonKhoGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(queryInfo.AdditionalSearchString);
                if (queryString.KhoId != 0)
                {
                    khoDuocPhamId = queryString.KhoId;
                }
            }


            var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == queryString.KhoId)
                .Select(p => new
                {
                    Id = p.DuocPhamBenhVienId,
                    SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat
                }).ToList();

            var allDataGroup = allDataNhap.GroupBy(o => o.Id);
            var duocPhamBenhVienIds = allDataGroup.Select(o => o.Key).ToList();
            var dinhMucDuocPhamTonKhos = _dinhMucDuocPhamTonKhoRepository.TableNoTracking
                .Where(o => o.KhoId == queryString.KhoId).ToList();

            var duocPhamBenhViens = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => duocPhamBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.DuocPhamBenhVienPhanNhomId,
                    o.DuocPham.Ten,
                    o.DuocPham.HoatChat,
                    o.DuocPham.HamLuong,
                    o.Ma,
                    DonViTinh = o.DuocPham.DonViTinh.Ten
                })
                .ToList();

            var dataReturn = new List<TonKhoGridVo>();
            foreach (var group in allDataGroup)
            {
                var tonKhoGridVo = new TonKhoGridVo
                {
                    Id = group.Key,
                    SoLuongTon = group.Sum(o => o.SoLuongTon).MathRoundNumber(2),                    
                };
                var duocPhamBenhVien = duocPhamBenhViens.First(o => o.Id == group.Key);
                tonKhoGridVo.DuocPham = duocPhamBenhVien.Ten;
                tonKhoGridVo.HoatChat = duocPhamBenhVien.HoatChat;
                tonKhoGridVo.HamLuong = duocPhamBenhVien.HamLuong;
                tonKhoGridVo.DonViTinhName = duocPhamBenhVien.DonViTinh;
                tonKhoGridVo.MaDuocPham = duocPhamBenhVien.Ma;
                tonKhoGridVo.PhanLoai = phanNhoms.FirstOrDefault(o => o.Id == duocPhamBenhVien.DuocPhamBenhVienPhanNhomId)?.Ten ?? "";

                var dinhMucTonKho = dinhMucDuocPhamTonKhos.Where(o => o.DuocPhamBenhVienId == group.Key).FirstOrDefault();
                if (dinhMucTonKho != null)
                {
                    tonKhoGridVo.TonToiThieu = dinhMucTonKho.TonToiThieu;
                    tonKhoGridVo.TonToiDa = dinhMucTonKho.TonToiDa;
                }
                dataReturn.Add(tonKhoGridVo);
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                dataReturn = dataReturn.Where(o => o.DuocPham.ToLower().RemoveVietnameseDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveVietnameseDiacritics())
                                                   || (o.MaDuocPham != null && o.MaDuocPham.ToLower().RemoveVietnameseDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveVietnameseDiacritics()))
                                                   || (o.HoatChat != null && o.HoatChat.ToLower().RemoveVietnameseDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveVietnameseDiacritics()))).ToList();
            }
            if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
            {
                dataReturn = dataReturn.Where(p => p.CanhBao.Contains(queryString.CanhBao)).ToList();
            }

            return new GridDataSource { Data = dataReturn.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dataReturn.Count() };




            //var query = _nhapKhoDuocPhamRepository.TableNoTracking
            //    .Where(x => x.KhoId == queryString.KhoId).Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DinhMucDuocPhamTonKhos)
            //    .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
            //    .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
            //    .Select(s => new TonKhoGridVo
            //    {
            //        Id = s.DuocPhamBenhViens.DuocPham.Id,
            //        DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
            //        HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
            //        HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
            //        PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
            //        TonToiThieu = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(x => x.KhoId == queryString.KhoId) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.FirstOrDefault(x => x.KhoId == queryString.KhoId).TonToiThieu : null,
            //        TonToiDa = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(x => x.KhoId == queryString.KhoId) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.FirstOrDefault(x => x.KhoId == queryString.KhoId).TonToiDa : null,
            //        DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
            //        SoLuongTon = s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == queryString.KhoId).Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == queryString.KhoId).Sum(o => o.SoLuongDaXuat) : 0,

            //        //BVHD-3912
            //        MaDuocPham = s.DuocPhamBenhViens.Ma
            //    }).Distinct().Where(o => o.CanhBao != string.Empty);
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhName);

            //if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
            //{
            //    query = query.Where(p => p.CanhBao.Contains(queryString.CanhBao));
            //}
            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArray();
            //return new GridDataSource { Data = queryTask, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var khoDuocPhamId = (long)0;
            var queryString = new TonKhoGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(queryInfo.AdditionalSearchString);
                if (queryString.KhoId != 0)
                {
                    khoDuocPhamId = queryString.KhoId;
                }
            }
            var query = _nhapKhoDuocPhamRepository.TableNoTracking
                .Where(x => x.KhoId == queryString.KhoId).Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DinhMucDuocPhamTonKhos)
                .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                .Select(s => new TonKhoGridVo
                {
                    Id = s.DuocPhamBenhViens.DuocPham.Id,
                    DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                    PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                    HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                    TonToiThieu = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(x => x.KhoId == queryString.KhoId) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.FirstOrDefault(x => x.KhoId == queryString.KhoId).TonToiThieu : null,
                    TonToiDa = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(x => x.KhoId == queryString.KhoId) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.FirstOrDefault(x => x.KhoId == queryString.KhoId).TonToiDa : null,
                    DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLuongTon = s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == queryString.KhoId).Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == queryString.KhoId).Sum(o => o.SoLuongDaXuat) : 0
                }).Distinct().Where(o => o.CanhBao != string.Empty);

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhName);

            if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
            {
                query = query.Where(p => p.CanhBao.Contains(queryString.CanhBao));
            }

            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }


        public List<TonKhoGridVo> GetTonKhoCanhBao(string search)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var khoDuocPhamId = BaseRepository.TableNoTracking.FirstOrDefault().Id;
            if (!string.IsNullOrEmpty(search))
            {
                var queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(search);
                if (queryString.KhoId != 0)
                {
                    khoDuocPhamId = queryString.KhoId;
                }
            }
            var qString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(search);
            var query = _nhapKhoDuocPhamRepository.TableNoTracking
                 .Where(x => x.KhoId == khoDuocPhamId).Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DinhMucDuocPhamTonKhos)
                 .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                 .Select(s => new TonKhoGridVo
                 {
                     Id = s.DuocPhamBenhViens.DuocPham.Id,
                     DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                     HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                     PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                     HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                     TonToiThieu = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(x => x.KhoId == khoDuocPhamId) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.FirstOrDefault(x => x.KhoId == khoDuocPhamId).TonToiThieu : null,
                     TonToiDa = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(x => x.KhoId == khoDuocPhamId) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.FirstOrDefault(x => x.KhoId == khoDuocPhamId).TonToiDa : null,
                     DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                     SoLuongTon = s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == khoDuocPhamId).Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == khoDuocPhamId).Sum(o => o.SoLuongDaXuat) : 0
                 }).Distinct().Where(o => o.CanhBao != string.Empty);
            if (!string.IsNullOrEmpty(search))
            {
                var queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(search);
                if (!string.IsNullOrEmpty(queryString.searchString))
                {
                    query = query.Where(p => p.HoatChat.ToLower().TrimEnd().TrimStart().Contains(queryString.searchString.ToLower().TrimEnd().TrimStart())
                     || p.DuocPham.ToLower().TrimEnd().TrimStart().Contains(queryString.searchString.ToLower().TrimEnd().TrimStart()));
                }
                if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
                {
                    query = query.Where(p => p.CanhBao.Contains(queryString.CanhBao));
                }
            }
            return query.ToList();
        }

        public string GetCanhBaoDuocPhamHTML(string search)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var result = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoDuocPhamDangCanhBao")).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(search);
            var getTenKho = _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == queryString.KhoId).Select(p => p.Ten).FirstOrDefault();

            var model = new List<LookupItemVo>
            {
                new LookupItemVo { DisplayName = "Tất cả", KeyId = 0 },
                new LookupItemVo { DisplayName = "Tồn kho quá nhiều", KeyId = 1 },
                new LookupItemVo { DisplayName = "Hết tồn kho", KeyId = 2 },
                new LookupItemVo { DisplayName = "Sắp hết tồn kho", KeyId = 3 }
            };
            var getKeyId = model.Where(p => p.DisplayName.Contains(queryString.CanhBao)).Select(p => p.KeyId).FirstOrDefault();
            var query = _nhapKhoDuocPhamRepository.TableNoTracking
                .Where(x => x.KhoId == queryString.KhoId).Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DinhMucDuocPhamTonKhos)
                .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                .Select(s => new TonKhoGridVo
                {
                    Id = s.DuocPhamBenhViens.DuocPham.Id,
                    DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                    PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                    HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                    TonToiThieu = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(x => x.KhoId == queryString.KhoId) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.FirstOrDefault(x => x.KhoId == queryString.KhoId).TonToiThieu : null,
                    TonToiDa = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(x => x.KhoId == queryString.KhoId) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.FirstOrDefault(x => x.KhoId == queryString.KhoId).TonToiDa : null,
                    DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLuongTon = s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == queryString.KhoId).Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == queryString.KhoId).Sum(o => o.SoLuongDaXuat) : 0,

                    //BVHD-3912
                    MaDuocPham = s.DuocPhamBenhViens.Ma
                }).Distinct();
            if (getKeyId == 0)
            {
                query = query.Where(o => o.CanhBao != string.Empty);
            }
            else
            {
                query = query.Where(o => o.CanhBao != string.Empty && o.CanhBao.Contains(queryString.CanhBao));
            }
            var DateNow = DateTime.Today.Day > 9 ? DateTime.Today.Day.ToString() : "0" + DateTime.Today.Day.ToString();
            var MonthNow = DateTime.Today.Month > 9 ? DateTime.Today.Month.ToString() : "0" + DateTime.Today.Month.ToString();
            var YearNow = DateTime.Today.Year.ToString();

            if (!string.IsNullOrEmpty(search))
            {
                if (!string.IsNullOrEmpty(queryString.searchString))
                {
                    query = query.Where(p => p.HoatChat.ToLower().TrimEnd().TrimStart().Contains(queryString.searchString.ToLower().TrimEnd().TrimStart())
                     || p.DuocPham.ToLower().TrimEnd().TrimStart().Contains(queryString.searchString.ToLower().TrimEnd().TrimStart()));
                }
                if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
                {
                    query = query.Where(p => p.CanhBao.Contains(queryString.CanhBao));
                }
            }
            var lstDuocPham = query.ToList();
            string finalresult = string.Empty;
            var i = 0;
            foreach (var item in lstDuocPham)
            {
                finalresult = finalresult + "<tr style='border: 1px solid #020000;text-align: center; '><td style = 'padding:5px;border: 1px solid #020000;'>" + (i + 1) + "</td>"
                              + "<td style='border: 1px solid #020000;'>" + item.MaDuocPham + "</td>"
                              + "<td style='border: 1px solid #020000;'>" + item.DuocPham + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.HoatChat + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.HamLuong + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.PhanLoai + "</td>" 
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.DonViTinhName + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;text-align: right;'>" + item.SoLuongTon + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.CanhBao + "</tr>";
                i++;
            }

            var data = new DataTitleCanhBaoHTML
            {
                TemplateDuocPhamCanhBao = finalresult,
                TenKho = getTenKho,
                LoaiCanhBao = queryString.CanhBao,
                NgayNow = DateNow,
                ThangNow = MonthNow,
                NamNow = YearNow,
            };
            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        public List<TonKhoTatCaGridVo> GetTongHopTonKho(string search)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var query = _nhapKhoDuocPhamRepository.TableNoTracking
                .Include(x => x.NhapKhoDuocPhamChiTiets)
                .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                .Select(s => new TonKhoTatCaGridVo
                {

                    Id = s.DuocPhamBenhViens.DuocPham.Id,
                    DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                    PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                    HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                    DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    SoLuongTon = s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongDaXuat) : 0
                }).Distinct();
            if (!string.IsNullOrEmpty(search))
            {
                var queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(search);
                if (queryString.KhoId != 0)
                {
                    query = _nhapKhoDuocPhamRepository.TableNoTracking
                    .Where(x => x.KhoId == queryString.KhoId).Include(x => x.NhapKhoDuocPhamChiTiets)
                    .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                        .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                    .Select(s => new TonKhoTatCaGridVo
                    {

                        Id = s.DuocPhamBenhViens.DuocPham.Id,
                        DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                        HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                        HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                        PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                        DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLuongTon = s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == queryString.KhoId).Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == queryString.KhoId).Sum(o => o.SoLuongDaXuat) : 0
                    }).Distinct();

                }
                if (!string.IsNullOrEmpty(queryString.searchString))
                {
                    query = query.Where(p => p.HoatChat.ToLower().TrimEnd().TrimStart().Contains(queryString.searchString.ToLower().TrimEnd().TrimStart())
                   || p.DuocPham.ToLower().TrimEnd().TrimStart().Contains(queryString.searchString.ToLower().TrimEnd().TrimStart()));
                }
            }
            return query.ToList();
        }
        public string GetTonKhoDuocPhamHTML(string search)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var result = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoDuocPhamTonKho")).FirstOrDefault();
            //var khoDuocPhamId = BaseRepository.TableNoTracking.FirstOrDefault().Id;
            var queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(search);

            //if (!string.IsNullOrEmpty(search))
            //{
            //    if (queryString.KhoId == 0)
            //    {
            //        khoDuocPhamId = 0;
            //    }
            //    else
            //    {
            //        khoDuocPhamId = queryString.KhoId;
            //    }
            //}

            //var qString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(search);
            var getTenKho = "";
            if (queryString.KhoId == 0)
            {
                getTenKho = "Kho Tổng";
            }
            else
            {
                getTenKho = _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == queryString.KhoId).Select(p => p.Ten).FirstOrDefault();
            }
            IQueryable<TonKhoTatCaGridVo> query = _nhapKhoDuocPhamRepository.TableNoTracking
                 .Include(x => x.NhapKhoDuocPhamChiTiets)
                 .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                .GroupBy(s => new
                {
                    Id = s.DuocPhamBenhViens.DuocPham.Id,
                    DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                    PhanLoai =
                        s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null &&
                        s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
                            .DuocPhamBenhVienPhanNhom != null
                            ? PhanNhomChaCuaDuocPham(phanNhoms,
                                s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
                                    .DuocPhamBenhVienPhanNhom)
                            : "",
                    SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
                    HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                    DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,

                    //BVHD-3912
                    MaDuocPham = s.DuocPhamBenhViens.Ma
                }, p => p,
                    (s, v) => new TonKhoTatCaGridVo
                    {
                        Id = s.Id,
                        DuocPham = s.DuocPham,
                        HamLuong = s.HamLuong,
                        PhanLoai = s.PhanLoai,
                        SoDangKy = s.SoDangKy,
                        HoatChat = s.HoatChat,
                        DonViTinhName = s.DonViTinhName,
                        SoLuongTon = v.Any() ? v.Sum(o => o.SoLuongNhap) - v.Sum(o => o.SoLuongDaXuat) : 0,
                        GiaTriSoLuongTon = v.Any() ? v.Sum(o => (decimal)o.SoLuongNhap * o.DonGiaBan) - v.Sum(o => (decimal)o.SoLuongDaXuat * o.DonGiaBan) : 0,

                        //BVHD-3912
                        MaDuocPham = s.MaDuocPham
                    }).OrderBy(o => o.DuocPham);

            if (queryString.KhoId != 0)
            {
                query = _nhapKhoDuocPhamRepository.TableNoTracking
                    .Where(x => x.KhoId == queryString.KhoId).Include(x => x.NhapKhoDuocPhamChiTiets)
                    .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens)
                    .ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                    .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                    .GroupBy(s => new
                    {
                        Id = s.DuocPhamBenhViens.DuocPham.Id,
                        DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                        HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                        PhanLoai =
                            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null &&
                            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
                                .DuocPhamBenhVienPhanNhom != null
                                ? PhanNhomChaCuaDuocPham(phanNhoms,
                                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
                                        .DuocPhamBenhVienPhanNhom)
                                : "",
                        SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
                        HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                        DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,

                        //BVHD-3912
                        MaDuocPham = s.DuocPhamBenhViens.Ma
                    }, p => p,
                        (s, v) => new TonKhoTatCaGridVo
                        {
                            Id = s.Id,
                            DuocPham = s.DuocPham,
                            HamLuong = s.HamLuong,
                            PhanLoai = s.PhanLoai,
                            SoDangKy = s.SoDangKy,
                            HoatChat = s.HoatChat,
                            DonViTinhName = s.DonViTinhName,
                            SoLuongTon = v.Any() ? v.Sum(o => o.SoLuongNhap) - v.Sum(o => o.SoLuongDaXuat) : 0,
                            GiaTriSoLuongTon = v.Any() ? v.Sum(o => (decimal)o.SoLuongNhap * o.DonGiaNhap) - v.Sum(o => (decimal)o.SoLuongDaXuat * o.DonGiaNhap) : 0,

                            //BVHD-3912
                            MaDuocPham = s.MaDuocPham
                        }).OrderBy(o => o.DuocPham);
            }
            var DateNow = DateTime.Today.Day > 9 ? DateTime.Today.Day.ToString() : "0" + DateTime.Today.Day.ToString();
            var MonthNow = DateTime.Today.Month > 9 ? DateTime.Today.Month.ToString() : "0" + DateTime.Today.Month.ToString();
            var YearNow = DateTime.Today.Year.ToString();

            if (!string.IsNullOrEmpty(search))
            {
                if (!string.IsNullOrEmpty(queryString.searchString))
                {
                    query = query.Where(p => p.HoatChat.ToLower().TrimEnd().TrimStart().Contains(queryString.searchString.ToLower().TrimEnd().TrimStart())
                     || p.DuocPham.ToLower().TrimEnd().TrimStart().Contains(queryString.searchString.ToLower().TrimEnd().TrimStart()));
                }
            }
            var lstDuocPham = query.ToList();
            string finalresult = string.Empty;
            var i = 1;
            decimal totalGiaTriSoLuongTon = 0;
            foreach (var item in lstDuocPham)
            {
                finalresult = finalresult + "<tr style='border: 1px solid #020000;'><td style='padding:5px;border: 1px solid #020000;'>" + i + "</td>"
                              + "<td style='padding:5px;border: 1px solid #020000;'>" + item.MaDuocPham + "</td>"
                              + "<td style='padding:5px;border: 1px solid #020000;'>" + item.DuocPham + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.HoatChat + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.HamLuong + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.PhanLoai + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.DonViTinhName + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;text-align: right;'>" + item.SoLuongTon.MathRoundNumber(2) + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;text-align: right;'>" + item.GiaTriSoLuongTonFormat + "</td>"
                              + "</tr>";
                totalGiaTriSoLuongTon += item.GiaTriSoLuongTon;
                i++;
            }
            var data = new DataTitleTonKhoHTML
            {
                TemplateDuocPhamTonKho = finalresult,
                TenKho = getTenKho,
                NgayNow = DateNow,
                ThangNow = MonthNow,
                NamNow = YearNow,
                TotalGiaTriSoLuongTon = totalGiaTriSoLuongTon.ApplyFormatMoneyVND()
            };
            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }

        private string CanhBao(List<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> dmdv, List<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho> dv)
        {

            string result = null;
            double soLuongTon = TinhSoLuongTon(dmdv);
            if (dv.Count > 0)
            {

                int? tonToiDa = 0;
                int? tonToiThieu = 0;
                for (int i = 0; i < dv.Count; i++)
                {
                    tonToiDa = tonToiDa + dv.ToList()[i].TonToiDa;
                    tonToiThieu = tonToiThieu + dv.ToList()[i].TonToiThieu;
                }
                if (soLuongTon > tonToiDa)
                {
                    result = "Tồn kho quá nhiều";
                }
                else if (soLuongTon == 0)
                {
                    result = "Hết tồn kho";
                }
                else if (soLuongTon < tonToiThieu)
                {
                    result = "Sắp hết tồn kho";
                }
            }
            if (soLuongTon == 0)
            {
                result = "Hết tồn kho";
            }
            return result;
        }
        private double TinhSoLuongTon(List<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> dv)
        {
            double soLuongNhap = 0;
            double soLuongXuat = 0;
            for (int i = 0; i < dv.Count; i++)
            {
                soLuongNhap = soLuongNhap + dv.ToList()[i].SoLuongNhap;
                soLuongXuat = soLuongXuat + dv.ToList()[i].SoLuongDaXuat;
            }

            return soLuongNhap - soLuongXuat;
        }
        public async Task<List<Kho>> GetDataTonKho()
        {
            var khoDuocPhamId = BaseRepository.TableNoTracking.FirstOrDefault().Id;
            var lstEntity = await BaseRepository.Table.Include(x => x.NhapKhoDuocPhams).ThenInclude(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ToListAsync();
            var test = await _duocPhamRepository.Table.Include(x => x.DuocPhamBenhVien).ThenInclude(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.NhapKhoDuocPhams).ThenInclude(x => x.KhoDuocPhams).Where(x => x.Ten.Contains("Panadol")).SelectMany(x => x.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets).Where(x => x.NhapKhoDuocPhams.KhoId == khoDuocPhamId).ToListAsync();
            //for (int i = 0; i < test.Count; i++)
            //{
            //    foreach(var item in test[i].DuocPhamBenhVien.NhapKhoDuocPhamChiTiets) {
            //        if(item.Kho)
            //    }
            //    for (int j = 0; j < test[i].DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Count; j++)
            //    {

            //    }
            //}
            // var thuoc = await BaseRepository.Table.Include(x => x.DinhMucDuocPhamTonKhos).ThenInclude(a => a.DuocPhamBenhVien).ThenInclude(x => x.DuocPham).ToListAsync();
            var test1 = _duocPhamRepository.Table.Include(x => x.DuocPhamBenhVien).ThenInclude(x => x.DinhMucDuocPhamTonKhos)
                                                 .Include(x => x.DuocPhamBenhVien).ThenInclude(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.NhapKhoDuocPhams).ThenInclude(x => x.KhoDuocPhams)
                                                 .Where(x => x.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(y => y.NhapKhoDuocPhams.KhoId == khoDuocPhamId)).ToList();
            var test2 = _duocPhamRepository.Table.Include(x => x.DuocPhamBenhVien).ThenInclude(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.NhapKhoDuocPhams).ThenInclude(x => x.KhoDuocPhams).ToList();
            return lstEntity;
        }
        public async Task<List<LookupItemVo>> GetKho(LookupQueryInfo queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                         .Where(p => p.NhanVienId == userCurrentId && p.Kho.LoaiDuocPham == true)
                         .Select(s => new LookupItemVo
                         {
                             KeyId = s.KhoId,
                             DisplayName = s.Kho.Ten
                         })
                         .ApplyLike(queryInfo.Query, o => o.DisplayName)
                         .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetKhoVatTuChoKT(LookupQueryInfo queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var result = _khoDuocPhamRepository.TableNoTracking
                         //.Where(p => p.NhanVienId == userCurrentId &&  p.Kho.LoaiVatTu == true)//update 01/10/21: KT dc xem tat ca cac kho
                         .Select(s => new LookupItemVo
                         {
                             KeyId = s.Id,
                             DisplayName = s.Ten
                         })
                         .ApplyLike(queryInfo.Query, o => o.DisplayName)
                         .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetKhoDuocPhamAsync(LookupQueryInfo queryInfo)
        {
            var lstKhoDuocPham = await BaseRepository.TableNoTracking
                .Where(p => p.LoaiDuocPham == true)
                .ApplyLike(queryInfo.Query, g => g.Ten)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = Convert.ToInt32(item.Id),
                })
                .ToListAsync();

            return lstKhoDuocPham;
        }
        //////////////////////////////////
        public async Task<GridDataSource> GetDataForGridTatCaAsync(QueryInfo queryInfo, bool exportExcel)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var queryString = new TonKhoGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(queryInfo.AdditionalSearchString);
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { Data = new TonKhoTatCaGridVo[0], TotalRowCount = 0 };
            }
            //var query = _nhapKhoDuocPhamRepository.TableNoTracking
            //    .Include(x => x.NhapKhoDuocPhamChiTiets)
            //    .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
            //    .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
            //    .GroupBy(s => new
            //    {
            //        Id = s.DuocPhamBenhViens.DuocPham.Id,
            //        DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
            //        HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
            //        PhanLoai =
            //            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null &&
            //            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
            //                .DuocPhamBenhVienPhanNhom != null
            //                ? PhanNhomChaCuaDuocPham(phanNhoms,
            //                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
            //                        .DuocPhamBenhVienPhanNhom)
            //                : "",
            //        SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
            //        HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
            //        DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
            //    }, p => p,
            //        (s, v) => new TonKhoTatCaGridVo
            //        {
            //            Id = s.Id,
            //            DuocPham = s.DuocPham,
            //            HamLuong = s.HamLuong,
            //            PhanLoai = s.PhanLoai,
            //            SoDangKy = s.SoDangKy,
            //            HoatChat = s.HoatChat,
            //            DonViTinhName = s.DonViTinhName,
            //            SoLuongTon = v.Any() ? v.Sum(o => o.SoLuongNhap) - v.Sum(o => o.SoLuongDaXuat) : 0,
            //            GiaTriSoLuongTon = v.Any() ? v.Sum(o => (decimal)o.SoLuongNhap * o.DonGiaNhap) - v.Sum(o => (decimal)o.SoLuongDaXuat * o.DonGiaNhap) : 0,
            //        });


            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhName);
            var query = _nhapKhoDuocPhamRepository.TableNoTracking
                    .Where(x => x.KhoId == queryString.KhoId).Include(x => x.NhapKhoDuocPhamChiTiets)
                    .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                        .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                            .GroupBy(s => new
                            {
                                Id = s.DuocPhamBenhViens.DuocPham.Id,
                                DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                                HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                                PhanLoai =
                                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null &&
                                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
                                        .DuocPhamBenhVienPhanNhom != null
                                        ? PhanNhomChaCuaDuocPham(phanNhoms,
                                            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
                                                .DuocPhamBenhVienPhanNhom)
                                        : "",
                                SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
                                HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                                DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,

                                //BVHD-3912
                                MaDuocPham = s.DuocPhamBenhViens.Ma
                            }, p => p,
                                (s, v) => new TonKhoTatCaGridVo
                                {
                                    Id = s.Id,
                                    DuocPham = s.DuocPham,
                                    HamLuong = s.HamLuong,
                                    PhanLoai = s.PhanLoai,
                                    SoDangKy = s.SoDangKy,
                                    HoatChat = s.HoatChat,
                                    DonViTinhName = s.DonViTinhName,
                                    SoLuongTon = v.Any() ? v.Sum(o => o.SoLuongNhap) - v.Sum(o => o.SoLuongDaXuat) : 0,
                                    GiaTriSoLuongTon = v.Any() ? v.Sum(o => (decimal)o.SoLuongNhap * o.DonGiaNhap) - v.Sum(o => (decimal)o.SoLuongDaXuat * o.DonGiaNhap) : 0,

                                    //BVHD-3912
                                    MaDuocPham = s.MaDuocPham
                                })
                    .ApplyLike(queryString.Description, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhName);
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();

            var returnData = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToList();
            returnData.ForEach(o => o.SoLuongTon = o.SoLuongTon.MathRoundNumber(2));
            // await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = returnData.ToArray(), TotalRowCount = countTask };

        }

        public GridDataSource GetTotalPageForGridTatCaAsync(QueryInfo queryInfo)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            BuildDefaultSortExpression(queryInfo);
            var queryString = new TonKhoGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(queryInfo.AdditionalSearchString);
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { Data = new TonKhoTatCaGridVo[0], TotalRowCount = 0 };
            }
            var query = _nhapKhoDuocPhamRepository.TableNoTracking
                    .Where(x => x.KhoId == queryString.KhoId).Include(x => x.NhapKhoDuocPhamChiTiets)
                    .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                        .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                            .GroupBy(s => new
                            {
                                Id = s.DuocPhamBenhViens.DuocPham.Id,
                                DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                                HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                                PhanLoai =
                                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null &&
                                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
                                        .DuocPhamBenhVienPhanNhom != null
                                        ? PhanNhomChaCuaDuocPham(phanNhoms,
                                            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
                                                .DuocPhamBenhVienPhanNhom)
                                        : "",
                                SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
                                HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                                DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                            }, p => p,
                                (s, v) => new TonKhoTatCaGridVo
                                {
                                    Id = s.Id,
                                    DuocPham = s.DuocPham,
                                    HamLuong = s.HamLuong,
                                    PhanLoai = s.PhanLoai,
                                    SoDangKy = s.SoDangKy,
                                    HoatChat = s.HoatChat,
                                    DonViTinhName = s.DonViTinhName,
                                    SoLuongTon = v.Any() ? v.Sum(o => o.SoLuongNhap) - v.Sum(o => o.SoLuongDaXuat) : 0,
                                    GiaTriSoLuongTon = v.Any() ? v.Sum(o => (decimal)o.SoLuongNhap * o.DonGiaNhap) - v.Sum(o => (decimal)o.SoLuongDaXuat * o.DonGiaNhap) : 0
                                })
                    .ApplyLike(queryString.Description, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhName);

            //var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            //var query = _nhapKhoDuocPhamRepository.TableNoTracking
            //    .Include(x => x.NhapKhoDuocPhamChiTiets)
            //    .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens)
            //    .ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
            //    .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom)
            //    .ThenInclude(x => x.NhomCha)
            //    .GroupBy(s => new
            //    {
            //        Id = s.DuocPhamBenhViens.DuocPham.Id,
            //        DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
            //        HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
            //        PhanLoai =
            //            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null &&
            //            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
            //                .DuocPhamBenhVienPhanNhom != null
            //                ? PhanNhomChaCuaDuocPham(phanNhoms,
            //                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
            //                        .DuocPhamBenhVienPhanNhom)
            //                : "",
            //        SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
            //        HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
            //        DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
            //    }, p => p,
            //        (s, v) => new TonKhoTatCaGridVo
            //        {
            //            Id = s.Id,
            //            DuocPham = s.DuocPham,
            //            HamLuong = s.HamLuong,
            //            PhanLoai = s.PhanLoai,
            //            SoDangKy = s.SoDangKy,
            //            HoatChat = s.HoatChat,
            //            DonViTinhName = s.DonViTinhName,
            //            SoLuongTon = v.Any() ? v.Sum(o => o.SoLuongNhap) - v.Sum(o => o.SoLuongDaXuat) : 0
            //        });


            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhName);

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{
            //    var queryString = JsonConvert.DeserializeObject<TonKhoGridVoItem>(queryInfo.AdditionalSearchString);


            //    if (queryString.KhoId != 0)
            //    {
            //        query = _nhapKhoDuocPhamRepository.TableNoTracking
            //        .Where(x => x.KhoId == queryString.KhoId).Include(x => x.NhapKhoDuocPhamChiTiets)
            //        .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
            //            .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
            //            .GroupBy(s => new
            //            {
            //                Id = s.DuocPhamBenhViens.DuocPham.Id,
            //                DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
            //                HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
            //                PhanLoai =
            //                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null &&
            //                    s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
            //                        .DuocPhamBenhVienPhanNhom != null
            //                        ? PhanNhomChaCuaDuocPham(phanNhoms,
            //                            s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien
            //                                .DuocPhamBenhVienPhanNhom)
            //                        : "",
            //                SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
            //                HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
            //                DonViTinhName = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
            //            }, p => p,
            //                (s, v) => new TonKhoTatCaGridVo
            //                {
            //                    Id = s.Id,
            //                    DuocPham = s.DuocPham,
            //                    HamLuong = s.HamLuong,
            //                    PhanLoai = s.PhanLoai,
            //                    SoDangKy = s.SoDangKy,
            //                    HoatChat = s.HoatChat,
            //                    DonViTinhName = s.DonViTinhName,
            //                    SoLuongTon = v.Any() ? v.Sum(o => o.SoLuongNhap) - v.Sum(o => o.SoLuongDaXuat) : 0
            //                });
            //        query = query.ApplyLike(queryString.Description, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhName);
            //    }
            //    //                if (!string.IsNullOrEmpty(queryString.Description))
            //    //                {
            //    //                    query = query.ApplyLike(queryString.Description, g => g.DuocPham, g => g.HoatChat, g => g.DonViTinhName);
            //    //                }
            //}
            //var countTask = query.ToArray();
            //await Task.WhenAll(countTask);
            //return new GridDataSource { TotalRowCount = countTask.Result };
            return new GridDataSource { TotalRowCount = query.Count() };
        }
        /////////////////////////////////Duoc Pham Sap het han
        ///

        public async Task<GridDataSource> GetDataForDuocPhamSapHetHanGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var settings = _cauhinhService.LoadSetting<CauHinhBaoCao>();
            var duocPham = new List<DuocPhamSapHetHanGridVo>();
            var duocPhamquery = new List<DuocPhamSapHetHanGridVo>();
            var duocPhamChi = new DuocPhamSapHetHanGridVo();
            //DateTime dayHetHan = DateTime.Now.AddDays(settings.DuocPhamSapHetHanNgayHetHan);
            DateTime dayHetHan = DateTime.Now;
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = _nhapKhoDuocPhamRepository.TableNoTracking
                .Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DinhMucDuocPhamTonKhos).ThenInclude(x => x.KhoDuocPham)
                .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.KhoDuocPhamViTri).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
                .Select(s => new DuocPhamSapHetHanGridVo
                {

                    Id = s.Id,
                    TenDuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                    HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                    PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                    TenHoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                    DonViTinh = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    TenKho = s.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                    NgayHetHanHienThi = s.HanSuDung.ApplyFormatDate(),
                    NgayHetHan = s.HanSuDung,
                    ViTri = s.KhoDuocPhamViTri.Ten,
                    VitriId = s.KhoDuocPhamViTri.Id,
                    KhoId = s.NhapKhoDuocPhams.KhoDuocPhams.Id,
                    SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                    SoNgayTruocKhiHetHan = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(p2 => p2.KhoId == s.NhapKhoDuocPhams.KhoId && p2.SoNgayTruocKhiHetHan != null) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.First(p2 => p2.KhoId == s.NhapKhoDuocPhams.KhoId).SoNgayTruocKhiHetHan.GetValueOrDefault() : settings.DuocPhamSapHetHanNgayHetHan,
                    SoLo = s.Solo,
                    NhapKhoDuocPhamId = s.NhapKhoDuocPhamId,
                    DuocPhamId = s.DuocPhamBenhVienId,
                    MaDuocPham = s.DuocPhamBenhViens.Ma,
                    DonGiaNhap = s.DonGiaNhap,

                })
                .Distinct()
                .Where(x => x.NgayHetHan >= DateTime.Now.Date && x.NgayHetHan <= dayHetHan.AddDays(x.SoNgayTruocKhiHetHan).Date && x.SoLuongTon > 0) 
                .GroupBy(item => new
                {
                    item.DuocPhamId,
                    item.KhoId,
                    item.DonGiaNhap,
                    item.SoLo,
                    item.NgayHetHan,
                    item.SoNgayTruocKhiHetHan,
                })
               .Select(item => new DuocPhamSapHetHanGridVo
               {
                   Id = item.First().Id,
                   TenDuocPham = item.First().TenDuocPham,
                   HamLuong = item.First().HamLuong,
                   PhanLoai = item.First().PhanLoai,
                   TenHoatChat = item.First().TenHoatChat,
                   DonViTinh = item.First().DonViTinh,
                   TenKho = item.First().TenKho,
                   NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                   NgayHetHan = item.First().NgayHetHan,
                   ViTri = item.First().ViTri,
                   VitriId = item.First().VitriId,
                   KhoId = item.First().KhoId,
                   SoLuongTon = item.Sum(d=>d.SoLuongTon),//s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongDaXuat) : 0,
                   SoNgayTruocKhiHetHan = item.First().SoNgayTruocKhiHetHan,
                   MaDuocPham = item.First().MaDuocPham,
                   SoLo = item.First().SoLo,
                   NhapKhoDuocPhamId = item.First().NhapKhoDuocPhamId,
                   DuocPhamId = item.First().DuocPhamId,
                   DonGiaNhap = item.First().DonGiaNhap,
               });




            //.Where(x => x.NgayHetHan >= DateTime.Now.Date && x.NgayHetHan < dayHetHan);

             query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenDuocPham, g => g.TenHoatChat, g => g.DonViTinh, g => g.TenKho, g => g.ViTri);
            //var queryDistinct = query.GroupBy(g => new
            //{
            //    g.VitriId
            //})
            //    .Select(g => g.First());
            //query = queryDistinct;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuocPhamSapHetHanSearchGridVoItem>(queryInfo.AdditionalSearchString);


                if (queryString.KhoId != 0 && queryString.KhoId != null)
                {
                    query = query.Where(x => x.KhoId.Equals(queryString.KhoId));
                }
                if (!string.IsNullOrEmpty(queryString.DuocPham))
                {
                    query = query.Where(p => p.TenDuocPham.TrimEnd().TrimStart().ToLower().Contains(queryString.DuocPham.TrimEnd().TrimStart().ToLower()));
                }
            }
            duocPhamquery = query.ToList();
            for (int i = 0; i < duocPhamquery.Count(); i++)
            {
                int count = 0;
                double soLuongTon = 0;
                if (i == 0)
                {
                    if (string.IsNullOrEmpty(duocPhamquery.ToList()[i].ViTri))
                    {
                        //soLuongTon = duocPhamquery.Where(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.ViTri == null
                        //                     && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan)).Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamquery.ToList()[i];
                        //duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        duocPhamquery.RemoveAll(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.ViTri == null
                                                  && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan));
                    }
                    else
                    {
                        var aa = duocPhamquery.Where(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.VitriId.Equals(duocPhamquery.ToList()[i].VitriId)
                                                  && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan)).ToList();
                        // soLuongTon = aa.Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamquery.ToList()[i];
                        // duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        foreach (var item in aa)
                        {
                            duocPhamquery.Remove(item);
                        }

                        //duocPhamquery.RemoveAll(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.VitriId.Equals(duocPhamquery.ToList()[i].VitriId)
                        //                          && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan));
                    }

                    i = -1;
                }


            }

            var queryable = duocPham.AsQueryable();
           
            var countTask = queryInfo.LazyLoadPage == true ? 0 : queryable.Count();

            var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };

        }

        public GridDataSource GetTotalPageDuocPhamSapHetHanForGridAsync(QueryInfo queryInfo)
        {
            var phanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var settings = _cauhinhService.LoadSetting<CauHinhBaoCao>();
            var duocPham = new List<DuocPhamSapHetHanGridVo>();
            var duocPhamquery = new List<DuocPhamSapHetHanGridVo>();
            var duocPhamChi = new DuocPhamSapHetHanGridVo();
            //DateTime dayHetHan = DateTime.Now.AddDays(settings.DuocPhamSapHetHanNgayHetHan);
            DateTime dayHetHan = DateTime.Now;
            BuildDefaultSortExpression(queryInfo);

            var query = _nhapKhoDuocPhamRepository.TableNoTracking
               .Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens).ThenInclude(x => x.DinhMucDuocPhamTonKhos).ThenInclude(x => x.KhoDuocPham)
               .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.KhoDuocPhamViTri).Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
               .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPhamBenhVienPhanNhom).ThenInclude(x => x.NhomCha)
               .Select(s => new DuocPhamSapHetHanGridVo
               {

                   Id = s.Id,
                   TenDuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                   HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong,
                   PhanLoai = s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien != null && s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom != null ? PhanNhomChaCuaDuocPham(phanNhoms, s.DuocPhamBenhViens.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom) : "",
                   TenHoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                   DonViTinh = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                   TenKho = s.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                   NgayHetHanHienThi = s.HanSuDung.ApplyFormatDate(),
                   NgayHetHan = s.HanSuDung,
                   ViTri = s.KhoDuocPhamViTri.Ten,
                   VitriId = s.KhoDuocPhamViTri.Id,
                   KhoId = s.NhapKhoDuocPhams.KhoDuocPhams.Id,
                   SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                   SoNgayTruocKhiHetHan = s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.Any(p2 => p2.KhoId == s.NhapKhoDuocPhams.KhoId && p2.SoNgayTruocKhiHetHan != null) ? s.DuocPhamBenhViens.DinhMucDuocPhamTonKhos.First(p2 => p2.KhoId == s.NhapKhoDuocPhams.KhoId).SoNgayTruocKhiHetHan.GetValueOrDefault() : settings.DuocPhamSapHetHanNgayHetHan,
                   SoLo = s.Solo,
                   NhapKhoDuocPhamId = s.NhapKhoDuocPhamId,
                   DuocPhamId = s.DuocPhamBenhVienId,
                   MaDuocPham = s.DuocPhamBenhViens.Ma,
                   DonGiaNhap = s.DonGiaNhap
               })
               .Distinct()
               .Where(x => x.NgayHetHan >= DateTime.Now.Date && x.NgayHetHan <= dayHetHan.AddDays(x.SoNgayTruocKhiHetHan).Date && x.SoLuongTon > 0)
               .GroupBy(item => new
               {
                   item.DuocPhamId,
                   item.KhoId,
                   item.DonGiaNhap,
                   item.SoLo,
                   item.NgayHetHan,
                   item.SoNgayTruocKhiHetHan,
               })
              .Select(item => new DuocPhamSapHetHanGridVo
              {
                  Id = item.First().Id,
                  TenDuocPham = item.First().TenDuocPham,
                  HamLuong = item.First().HamLuong,
                  PhanLoai = item.First().PhanLoai,
                  TenHoatChat = item.First().TenHoatChat,
                  DonViTinh = item.First().DonViTinh,
                  TenKho = item.First().TenKho,
                  NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                  NgayHetHan = item.First().NgayHetHan,
                  ViTri = item.First().ViTri,
                  VitriId = item.First().VitriId,
                  KhoId = item.First().KhoId,
                  SoLuongTon = item.First().SoLuongTon,//s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap) - s.DuocPhamBenhViens.NhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongDaXuat) : 0,
                   SoNgayTruocKhiHetHan = item.First().SoNgayTruocKhiHetHan,
                  MaDuocPham = item.First().MaDuocPham,
                  SoLo = item.First().SoLo,
                  NhapKhoDuocPhamId = item.First().NhapKhoDuocPhamId,
                  DuocPhamId = item.First().DuocPhamId,
              });

            //.Where(x => x.NgayHetHan >= DateTime.Now.Date && x.NgayHetHan < dayHetHan);


            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenDuocPham, g => g.TenHoatChat, g => g.DonViTinh, g => g.TenKho, g => g.ViTri);
            //var queryDistinct = query.GroupBy(g => new
            //{
            //    g.VitriId
            //})
            //    .Select(g => g.First());
            //query = queryDistinct;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuocPhamSapHetHanSearchGridVoItem>(queryInfo.AdditionalSearchString);


                if (queryString.KhoId != 0)
                {
                    query = query.Where(x => x.KhoId.Equals(queryString.KhoId));
                }
                if (!string.IsNullOrEmpty(queryString.DuocPham))
                {
                    query = query.Where(p => p.TenDuocPham.TrimEnd().TrimStart().ToLower().Contains(queryString.DuocPham.TrimEnd().TrimStart().ToLower()));
                }
            }
            duocPhamquery = query.ToList();
            for (int i = 0; i < duocPhamquery.Count(); i++)
            {
                int count = 0;
                double soLuongTon = 0;
                if (i == 0)
                {
                    if (string.IsNullOrEmpty(duocPhamquery.ToList()[i].ViTri))
                    {
                        //soLuongTon = duocPhamquery.Where(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.ViTri == null
                        //                     && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan)).Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamquery.ToList()[i];
                        //duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        duocPhamquery.RemoveAll(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.ViTri == null
                                                  && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan));
                    }
                    else
                    {
                        var aa = duocPhamquery.Where(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.VitriId.Equals(duocPhamquery.ToList()[i].VitriId)
                                                  && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan)).ToList();
                        // soLuongTon = aa.Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamquery.ToList()[i];
                        // duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        foreach (var item in aa)
                        {
                            duocPhamquery.Remove(item);
                        }

                        //duocPhamquery.RemoveAll(x => x.TenKho.Contains(duocPhamquery.ToList()[i].TenKho) && x.TenHoatChat.Contains(duocPhamquery.ToList()[i].TenHoatChat) && x.VitriId.Equals(duocPhamquery.ToList()[i].VitriId)
                        //                          && x.TenDuocPham.Contains(duocPhamquery.ToList()[i].TenDuocPham) && x.SoLuongTon.Equals(duocPhamquery.ToList()[i].SoLuongTon) && x.NgayHetHan.Equals(duocPhamquery.ToList()[i].NgayHetHan));
                    }

                    i = -1;
                }


            }

            var queryable = duocPham.AsQueryable();
            //var countTask = queryInfo.LazyLoadPage == true ?
            //       Task.FromResult(0) :
            //       query.CountAsync();
            //var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);

            //return new GridDataSource
            //{
            //    Data = queryTask.Result,
            //    TotalRowCount = countTask.Result
            //};
            var countTask = queryable.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetChiTietTonKhoCuaDuocPham(QueryInfo queryInfo)
        {
            var duocPhamId = _duocPhamRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            var khoId = BaseRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<NhatXuatTonKhoChiTietGridVoItem>(queryInfo.AdditionalSearchString);
                khoId = queryString.KhoId == 0 ? khoId : queryString.KhoId;
                duocPhamId = queryString.DuocPhamId;
            }
            //var tonDauKy = _nhapKhoDuocPhamRepository.TableNoTracking
            //    .Where(x => x.KhoId == khoId)
            //    .Sum(o => o.NhapKhoDuocPhamChiTiets.Where(c => c.DuocPhamBenhVienId == duocPhamId)
            //                                        .Select(c => c.SoLuongNhap - c.XuatKhoDuocPhamChiTietViTris.Where(p => p.NgayXuat != null).Select(p => p.SoLuongXuat).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum());

            var queryNhapKho = _nhapKhoDuocPhamRepository.TableNoTracking
                .Where(x => x.KhoId == khoId)
                .SelectMany(p => p.NhapKhoDuocPhamChiTiets)
                .Where(x => x.DuocPhamBenhViens.DuocPham.Id == duocPhamId)
                .Select(s => new NhapXuatTonKhoCapNhatDetailGridVo
                {
                    Id = s.Id,
                    NgayNhapXuat = s.NgayNhap,
                    NgayDisplay = s.NgayNhap.ApplyFormatDateTime(),
                    MaChungTu = s.NhapKhoDuocPhams.SoPhieu,
                    Nhap = s.SoLuongNhap,
                    Xuat = 0,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    Loai = 1,
                    SoLuong = s.SoLuongNhap,
                    SoLuongDaXuat = s.SoLuongDaXuat,
                    MaVach = s.MaVach,
                    MaRef = s.MaRef,
                    VAT = s.VAT,
                    TiLeBHYTThanhToan = s.TiLeBHYTThanhToan,
                    LaVatTuBHYT = s.LaDuocPhamBHYT,
                    DonGiaNhap = s.DonGiaNhap,
                    DuocPhamBenhVienPhanNhomTen = s.DuocPhamBenhVienPhanNhom != null ? s.DuocPhamBenhVienPhanNhom.Ten : "",
                    DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVienPhanNhomId
                });

            var queryXuatKho = _xuatKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(x => x.XuatKhoDuocPham.KhoXuatId == khoId
                        && x.NgayXuat != null && x.DuocPhamBenhVien.DuocPham.Id == duocPhamId)
                .Select(s => new NhapXuatTonKhoCapNhatDetailGridVo
                {
                    Id = s.Id,
                    NgayNhapXuat = s.NgayXuat.Value,
                    NgayDisplay = s.NgayXuat != null ? s.NgayXuat.Value.ApplyFormatDateTime() : string.Empty,
                    MaChungTu = s.XuatKhoDuocPham.SoPhieu,
                    Nhap = 0,
                    Xuat = s.XuatKhoDuocPhamChiTietViTris.Sum(p => p.SoLuongXuat),
                    Loai = 2,
                    SoLuong = s.XuatKhoDuocPhamChiTietViTris.Sum(p => p.SoLuongXuat)
                });

            var queryDangBook = _xuatKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(x => x.XuatKhoDuocPhamId == null && x.XuatKhoDuocPhamChiTietViTris.Any(o=> o.SoLuongXuat > 0 && o.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.KhoId == khoId)
                        && x.DuocPhamBenhVien.DuocPham.Id == duocPhamId)
                .Select(s => new NhapXuatTonKhoCapNhatDetailGridVo
                {
                    Id = s.Id,
                    NgayNhapXuat = s.CreatedOn.Value,
                    NgayDisplay = s.CreatedOn != null ? s.CreatedOn.Value.ApplyFormatDateTime() : string.Empty,
                    MaChungTu = "Đang book",
                    Nhap = 0,
                    Xuat = s.XuatKhoDuocPhamChiTietViTris.Sum(p => p.SoLuongXuat),
                    Loai = 2,
                    SoLuong = s.XuatKhoDuocPhamChiTietViTris.Sum(p => p.SoLuongXuat),
                    HighLightClass= "bg-row-lightpink",
                    ThongTinBooking=s.XuatKhoDuocPhamChiTietViTris.Any(o => o.DonThuocThanhToanChiTiets.Any())?"Mã YCTN có đơn thuốc: "+ string.Join(", ", s.XuatKhoDuocPhamChiTietViTris.SelectMany(o => o.DonThuocThanhToanChiTiets.Select(i=>i.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan)).Distinct()):
                    (s.XuatKhoDuocPhamChiTietViTris.Any(o => o.YeuCauXuatKhoDuocPhamChiTiets.Any()) ? "Yêu cầu xuất dược phẩm: " + string.Join(", ", s.XuatKhoDuocPhamChiTietViTris.SelectMany(o => o.YeuCauXuatKhoDuocPhamChiTiets.Select(i => i.YeuCauXuatKhoDuocPham.SoChungTu)).Distinct()) :
                    (s.XuatKhoDuocPhamChiTietViTris.Any(o => o.YeuCauDieuChuyenDuocPhamChiTiets.Any()) ? "Yều cầu điều chuyển về kho: " + string.Join(", ", s.XuatKhoDuocPhamChiTietViTris.SelectMany(o => o.YeuCauDieuChuyenDuocPhamChiTiets.Select(i => i.YeuCauDieuChuyenDuocPham.KhoNhap.Ten)).Distinct()) :
                    (s.XuatKhoDuocPhamChiTietViTris.Any(o => o.YeuCauTraDuocPhamChiTiets.Any()) ? "Yều cầu trả dược phẩm: " + string.Join(", ", s.XuatKhoDuocPhamChiTietViTris.SelectMany(o => o.YeuCauTraDuocPhamChiTiets.Select(i => i.YeuCauTraDuocPham.SoPhieu)).Distinct()) :
                    (s.XuatKhoDuocPhamChiTietViTris.Any(o => o.XuatKhoDuocPhamChiTiet.YeuCauDuocPhamBenhViens.Any()) ? "Mã YCTN có dược phẩm: " + string.Join(", ", s.XuatKhoDuocPhamChiTietViTris.SelectMany(o => o.XuatKhoDuocPhamChiTiet.YeuCauDuocPhamBenhViens.Select(i => i.YeuCauTiepNhan.MaYeuCauTiepNhan)).Distinct()) :
                    (s.XuatKhoDuocPhamChiTietViTris.Any(o => o.XuatKhoDuocPhamChiTiet.YeuCauDichVuKyThuatTiemChungs.Any()) ? "Mã YCTN có vắcxin: " + string.Join(", ", s.XuatKhoDuocPhamChiTietViTris.SelectMany(o => o.XuatKhoDuocPhamChiTiet.YeuCauDichVuKyThuatTiemChungs.Select(i => i.YeuCauDichVuKyThuat.YeuCauTiepNhan.MaYeuCauTiepNhan)).Distinct())  : "")))))
                });
            var queryFormat = queryNhapKho.Concat(queryXuatKho).Concat(queryDangBook).OrderByDescending(o => o.NgayNhapXuat);

            //var result = await queryFormat.ToListAsync();
            //var index = 0;
            //var slTon = tonDauKy;
            //foreach (var item in result)
            //{
            //    item.STT = index;
            //    item.Ton = slTon + item.Nhap - item.Xuat;
            //    slTon = item.Ton;
            //    index++;
            //}
            var query = queryFormat.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();


            var queryTask = query.Skip(queryInfo.Skip).Take(queryInfo.Take);

            //if (!string.IsNullOrEmpty(queryInfo.SortString))
            //{
            //    queryTask = queryTask.OrderBy(queryInfo.SortString);
            //}
            return new GridDataSource { Data = queryTask.ToArray(), TotalRowCount = countTask };
        }

        public double GetTongTonKhoCuaDuocPham(QueryInfo queryInfo)
        {
            var duocPhamId = _duocPhamRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            var khoId = BaseRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<NhatXuatTonKhoChiTietGridVoItem>(queryInfo.AdditionalSearchString);
                khoId = queryString.KhoId == 0 ? khoId : queryString.KhoId;
                duocPhamId = queryString.DuocPhamId;
            }
         
            var tongTon = _nhapKhoDuocPhamRepository.TableNoTracking
                .Where(x => x.KhoId == khoId)
                .SelectMany(p => p.NhapKhoDuocPhamChiTiets)
                .Where(x => x.DuocPhamBenhViens.DuocPham.Id == duocPhamId)
                .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);

            return tongTon;
        }
        public void UpdateChiTietTonKhoCuaDuocPham(CapNhatTonKhoItem capNhatTonKhoItem)
        {
            var duocPham = _duocPhamRepository.GetById(capNhatTonKhoItem.DuocPhamId);
            if (duocPham != null)
            {
                duocPham.SoDangKy = capNhatTonKhoItem.SoDangKy;
                _duocPhamRepository.Update(duocPham);
            }
            if (capNhatTonKhoItem.CapNhatTonKhoDuocPhamChiTiets.Any())
            {
                foreach (var item in capNhatTonKhoItem.CapNhatTonKhoDuocPhamChiTiets)
                {
                    if (item.Loai == 1)
                    {
                        var nhapKhoDuocPhamChiTiet = _nhapKhoDuocPhamChiTietRepository.GetById(item.Id);
                        if (nhapKhoDuocPhamChiTiet != null)
                        {
                            if (item.SoLuong >= nhapKhoDuocPhamChiTiet.SoLuongDaXuat)
                            {
                                nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId = capNhatTonKhoItem.DuocPhamId;
                                nhapKhoDuocPhamChiTiet.SoLuongNhap = item.SoLuong;
                                nhapKhoDuocPhamChiTiet.HanSuDung = item.HanSuDung.GetValueOrDefault();
                                nhapKhoDuocPhamChiTiet.Solo = item.SoLo;
                                nhapKhoDuocPhamChiTiet.MaVach = item.MaVach;
                                nhapKhoDuocPhamChiTiet.MaRef = item.MaRef;
                                nhapKhoDuocPhamChiTiet.DonGiaNhap = item.DonGiaNhap.GetValueOrDefault();
                                nhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId = (long)item.DuocPhamBenhVienPhanNhomId;
                                if (nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT)
                                {
                                    nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan = item.TiLeBHYTThanhToan.GetValueOrDefault();
                                    nhapKhoDuocPhamChiTiet.VAT = 0;
                                }
                                else
                                {
                                    nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan = null;
                                    nhapKhoDuocPhamChiTiet.VAT = item.VAT.GetValueOrDefault();
                                }
                                _nhapKhoDuocPhamChiTietRepository.Update(nhapKhoDuocPhamChiTiet);
                            }
                        }
                    }
                    //                    else
                    //                    {
                    //                        var xuatKhoDuocPhamChiTiet = _xuatKhoDuocPhamChiTietRepository.GetById(item.Id);
                    //                        if (xuatKhoDuocPhamChiTiet != null)
                    //                        {
                    //                            var soLuongXuat =
                    //                                xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(p => p.SoLuongXuat);
                    //                            if (item.SoLuong > soLuongXuat)
                    //                            {
                    //                                
                    //                            }
                    //                            else
                    //                            {
                    //                                if (item.SoLuong < soLuongXuat)
                    //                                {
                    //
                    //                                }
                    //                            }
                    //                            xuatKhoDuocPhamChiTiet. = item.SoLuong;
                    //                        }
                    //                    }
                }
            }
            _nhapKhoDuocPhamChiTietRepository.Context.SaveChanges();
        }

        public async Task<List<DuocPhamTemplateGridVo>> GetDuocPhamBenhVien(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
               nameof(DuocPham.Ten),
               nameof(DuocPham.HoatChat)
            };

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var result = _duocPhamBenhVienRepository.TableNoTracking
                   .Where(p => p.HieuLuc)
                   .Select(s => new DuocPhamTemplateGridVo
                   {
                       KeyId = s.Id,
                       DisplayName = s.DuocPham.Ten,
                       Ten = s.DuocPham.Ten,
                       HoatChat = s.DuocPham.HoatChat,
                   })
                   .ApplyLike(queryInfo.Query, o => o.DisplayName)
                   .Take(queryInfo.Take)
                   ;
                return await result.ToListAsync();
            }
            else
            {
                var lstDuocPhamId = await _duocPhamRepository
                  .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                  //.Where(dv => dv.DuocPhamBenhVien.HieuLuc == true)
                  .Select(s => s.Id).ToListAsync();

                var dct = lstDuocPhamId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                var lstDichVuKyThuatBenhVien = _duocPhamBenhVienRepository
                                    .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                 //.Where(dv => dv.HieuLuc == true)
                 .Select(s => new DuocPhamTemplateGridVo
                 {
                     KeyId = s.Id,
                     DisplayName = s.DuocPham.Ten,
                     Ten = s.DuocPham.Ten,
                     HoatChat = s.DuocPham.HoatChat,
                 })
                  .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                  .Take(queryInfo.Take);
                return await lstDichVuKyThuatBenhVien.ToListAsync();
            }
        }

        private string PhanNhomChaCuaDuocPham(List<DuocPhamBenhVienPhanNhom> phanNhoms, DuocPhamBenhVienPhanNhom duocPhamBenhVienPhanNhom)
        {
            if (duocPhamBenhVienPhanNhom.NhomCha != null)
                return PhanNhomChaCuaDuocPham(phanNhoms, duocPhamBenhVienPhanNhom.NhomCha);
            else
            {
                if (duocPhamBenhVienPhanNhom.NhomChaId != null)
                {
                    var item = phanNhoms.FirstOrDefault(o => o.Id == (long)duocPhamBenhVienPhanNhom.NhomChaId);
                    if (item != null)
                    {
                        if (item.NhomCha != null)
                        {
                            return PhanNhomChaCuaDuocPham(phanNhoms, item.NhomCha);
                        }
                        else
                        {
                            return item.Ten;
                        }
                    }
                    else
                    {
                        return duocPhamBenhVienPhanNhom.Ten;
                    }
                }
                else
                {
                    return duocPhamBenhVienPhanNhom.Ten;
                }
            }
        }
    }
}
