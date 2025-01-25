using Camino.Core.Domain.ValueObject;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using System.Globalization;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<GridDataSource> GetDataForGridAsyncLuuTruHoSo(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LuuTruHoSoGridVo>(queryInfo.AdditionalSearchString);
                
                if (!string.IsNullOrEmpty(queryString.TuNgayText))
                {
                    DateTime TuNgayPart = DateTime.Now;
                    if(DateTime.TryParseExact(queryString.TuNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart))
                    {
                        tuNgay = TuNgayPart;
                    }
                }
                if (!string.IsNullOrEmpty(queryString.DenNgayText))
                {
                    DateTime DenNgaysPart = DateTime.Now;
                    if(DateTime.TryParseExact(queryString.DenNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart))
                    {
                        denNgay = DenNgaysPart;
                    }
                }
            }

            //lấy tất cả các YeuCauNhapVien có bệnh án con
            var noiTruBenhAnConIds = _yeuCauNhapVienRepository.TableNoTracking
                .Where(d => d.YeuCauTiepNhanMeId != null)
                .SelectMany(d => d.YeuCauTiepNhans)
                .Where(o=>o.NoiTruBenhAn != null && o.NoiTruBenhAn.ThoiDiemRaVien != null && (tuNgay == null || tuNgay <=o.NoiTruBenhAn.ThoiDiemRaVien) && (denNgay == null || denNgay >= o.NoiTruBenhAn.ThoiDiemRaVien))                
                .Select(d=>d.NoiTruBenhAn.Id).ToList();

            var cds = _icdRepository.TableNoTracking.Select(d => new {
                Ten = d.Ma + "-" + d.TenTiengViet,
                Id = d.Id
            }).ToList();

           

            //var test = listChanDoanTheoNoiTruBenhAn.Where(d => d.ICDChanDoanDieuTri != null && d.ICDChanDoanDieuTri != "").ToList();
            var query = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.ThoiDiemRaVien != null && !noiTruBenhAnConIds.Contains(o.Id))
                .Select(s => new LuuTruHoSoGridVo
                {
                    Id = s.Id,
                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    SoBA = s.SoBenhAn,
                    MaBN = s.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    //DoiTuong = (s.YeuCauTiepNhan.CoBHYT != true) ? "Viện phí" : "BHYT (" + s.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    CoBHYT = s.YeuCauTiepNhan.CoBHYT,
                    MucHuong = s.YeuCauTiepNhan.BHYTMucHuong,
                    //MucHuong = s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                    //    ? s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                    //        .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                    //        .Select(a => a.MucHuong).FirstOrDefault() : (int?)null,
                    KhoaNhapVien = s.KhoaPhongNhapVien.Ten,
                    GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                    ThuTuSapXepLuuTru = s.ThuTuSapXepLuuTru,
                    // cập nhật BVHD-3648
                    NamSinh = DateHelper.DOBFormat(s.YeuCauTiepNhan.NgaySinh, s.YeuCauTiepNhan.ThangSinh, s.YeuCauTiepNhan.NamSinh),
                    NgayVaoVien = s.ThoiDiemNhapVien,
                    NgayRaVien = s.ThoiDiemRaVien,

                    TinhTrangRaVien = s.TinhTrangRaVien != null ? s.TinhTrangRaVien.Value.GetDescription() : "", // chuyen vien
                    SoLuuTru = s.SoLuuTru,
                    EnumKetQuaDieuTri = s.KetQuaDieuTri,
                    ChuyenVien = s.ChuyenDenBenhVienId != null ? s.ChuyenDenBenhVien.Ten :"",
                    ThongTinRaVien = s.ThongTinRaVien,
                    // cập nhật BVHD-3648 22/11/2021
                    KhoaPhongId = s.KhoaPhongNhapVienId,
                    CheckBHYT = s.YeuCauTiepNhan.CoBHYT != null && s.YeuCauTiepNhan.CoBHYT  != false ? true: false,
                });



            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LuuTruHoSoGridVo>(queryInfo.AdditionalSearchString);
                // 0: Chưa sắp xếp, 1: Đã sắp xếp, 2
                if (queryString.ChuaSapXep == false && queryString.DaSapXep == true)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.ChuaSapXep == true && queryString.DaSapXep == false)
                {
                    query = query.Where(p => p.TinhTrang == 0);
                }
                // BHYT,Viện phí
                if (queryString.CheckBHYT == false && queryString.CheckVienPhi == true)
                {
                    query = query.Where(p => p.CheckBHYT == false);
                }
                else if (queryString.CheckBHYT == true && queryString.CheckVienPhi == false)
                {
                    query = query.Where(p => p.CheckBHYT == true);
                }

                if (!string.IsNullOrEmpty(queryString.TuNgayText))
                {
                    DateTime TuNgayPart = DateTime.Now;
                    DateTime.TryParseExact(queryString.TuNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                    query = query.Where(d => d.NgayRaVien.Value >= TuNgayPart);
                }
                if (!string.IsNullOrEmpty(queryString.DenNgayText))
                {
                    DateTime DenNgaysPart = DateTime.Now;
                    DateTime.TryParseExact(queryString.DenNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                    
                    if (!string.IsNullOrEmpty(queryString.TuNgayText))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.TuNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        if(DenNgaysPart > TuNgayPart)
                        {
                            query = query.Where(d => d.NgayRaVien.Value <= DenNgaysPart);
                        }
                    }
                    else
                    {
                        query = query.Where(d => d.NgayRaVien.Value <= DenNgaysPart);
                    }

                   
                }
                //: chọn từng khoa có NB nằm nội trú
                if (queryString.KhoaPhongId != null)
                {
                    if(queryString.KhoaPhongId != 0)
                    {
                        query = query.Where(d => d.KhoaPhongId == queryString.KhoaPhongId);
                    }
                }



                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.HoTen,
                        g => g.SoBA,
                        g => g.MaTN,
                        g => g.MaBN,
                        g => g.KhoaNhapVien,
                        g => g.SoLuuTru
                   );

                }
               

            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.HoTen,
                        g => g.SoBA,
                        g => g.MaTN,
                        g => g.MaBN,
                        g => g.KhoaNhapVien,
                        g => g.SoLuuTru
                    );

            }


         

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            var listQuerySkipTake = queryTask.Result.Select(s => s.Id).ToList();

            // lấy thông tin info theo bệnh nhân list skip take 
            var listChanDoanTheoNoiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking
               .Where(o => o.ThoiDiemRaVien != null && listQuerySkipTake.Contains(o.Id))
               .Select(s => new LuuTruHoSoGridVo
               {
                   Id = s.Id,
                   NoiTruPhieuDieuTriIdOrChanDoanKemTheoIds = (s.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo || s.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong || s.LoaiBenhAn == Enums.LoaiBenhAn.TreSoSinh) ?
                                         s.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).Select(d => d.Id.ToString()).FirstOrDefault() :
                                         s.DanhSachChanDoanKemTheoRaVienICDId,
                   ChanDoanICDChinhGhiChuOrDanhSachChanDoanKemTheoRaVienGhiChu = (s.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo || s.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong || s.LoaiBenhAn == Enums.LoaiBenhAn.TreSoSinh) ?
                                         s.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).Select(d => d.ChanDoanChinhGhiChu).FirstOrDefault() :
                                         s.ChanDoanChinhRaVienGhiChu,


                   LoaiBenhAn = s.LoaiBenhAn,
                   NoiTruPhieuDieuTriInfos = s.NoiTruPhieuDieuTris.Select(d => new NoiTruPhieuDieuTriInfoGridVo
                   {
                       NoiTruThamKhamChanDoanKemTheoICDIds = d.NoiTruThamKhamChanDoanKemTheos.Select(f => f.ICDId).ToList(),
                       NoiTruBenhAnId = d.NoiTruBenhAnId,
                       ChanDoanChinhICDId = d.ChanDoanChinhICDId,
                       ChanDoanChinhGhiChu = d.ChanDoanChinhGhiChu,
                       NoiTruPhieuDieuTriId = d.Id,
                       NgayDieuTri = d.NgayDieuTri,
                   }).ToList(),
                   //NoiTruPhieuDieuTriInfo = s.NoiTruPhieuDieuTris != null ? s.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri)
                   //                        .Select(d => new NoiTruPhieuDieuTriInfoGridVo
                   //                        {
                   //                            //NoiTruThamKhamChanDoanKemTheos = d.NoiTruThamKhamChanDoanKemTheos.Select(f => f.ICDId.ToString()).ToList().Join("|"),
                   //                            NoiTruThamKhamChanDoanKemTheoICDIds = d.NoiTruThamKhamChanDoanKemTheos.Select(f => f.ICDId).ToList(),
                   //                            NoiTruBenhAnId = d.NoiTruBenhAnId,
                   //                            ChanDoanChinhICDId = d.ChanDoanChinhICDId,
                   //                            ChanDoanChinhGhiChu = d.ChanDoanChinhGhiChu,
                   //                            NoiTruPhieuDieuTriId = d.Id
                   //                        }).FirstOrDefault() : new NoiTruPhieuDieuTriInfoGridVo(),
                   ChanDoanChinhRaVienICDId = s.ChanDoanChinhRaVienICDId,
                   ChanDoanChinhRaVienGhiChu = s.ChanDoanChinhRaVienGhiChu
               }).ToList();

            if (listChanDoanTheoNoiTruBenhAn.Any())
            {
                foreach (var item in listChanDoanTheoNoiTruBenhAn.ToList())
                {
                    if (item.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo ||
                        item.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong ||
                        item.LoaiBenhAn == Enums.LoaiBenhAn.TreSoSinh)
                    {

                        if (item.NoiTruPhieuDieuTriInfo != null)
                        {
                            var chanDoanKemTheo = string.Empty;
                            // lấy thông tin bệnh an phiếu điều trị ngày cuối cùng
                            //var chanDoanKemTheos = item.NoiTruPhieuDieuTriInfo.NoiTruThamKhamChanDoanKemTheos;
                            //List<string> icdKemTheos = new List<string>();
                            //if (!string.IsNullOrEmpty(chanDoanKemTheos))
                            //{
                            //    var listKemTheos = chanDoanKemTheos.Split("|");
                            //    if (listKemTheos.Length != 0)
                            //    {

                            //        foreach (var icd in listKemTheos)
                            //        {
                            //            icdKemTheos.Add(cds.Where(d => d.Id == long.Parse(icd)).Select(d => d.Ten).FirstOrDefault());
                            //        }
                            //    }
                            //    chanDoanKemTheo = "Chẩn đoán kèm theo: " + string.Join("; ", icdKemTheos);
                            //}

                            var listKemTheos = item.NoiTruPhieuDieuTriInfo.NoiTruThamKhamChanDoanKemTheoICDIds;
                            if (listKemTheos.Any())
                            {                                
                                List<string> icdKemTheos = new List<string>();
                                foreach (var icd in listKemTheos)
                                {
                                    icdKemTheos.Add(cds.Where(d => d.Id == icd).Select(d => d.Ten).FirstOrDefault());
                                }
                                chanDoanKemTheo = "Chẩn đoán kèm theo: " + string.Join("; ", icdKemTheos);
                            }
                            
                            

                            var chanDoanChinhICDId = item.NoiTruPhieuDieuTriInfo.ChanDoanChinhICDId;
                            var chanDoanChinhICDGhiChu = item.NoiTruPhieuDieuTriInfo.ChanDoanChinhGhiChu;

                            if (chanDoanChinhICDId != null)
                            {
                                var chanDoan = chanDoanChinhICDId != null ?
                                              cds.Where(d => d.Id == (long)chanDoanChinhICDId).Select(d => d.Ten).FirstOrDefault() : "";

                                item.ICDChanDoanDieuTri += (chanDoan != null ? chanDoan + "(" + chanDoanChinhICDGhiChu + ")" : chanDoanChinhICDGhiChu ) + (!string.IsNullOrEmpty(chanDoanKemTheo) ? "; " + chanDoanKemTheo : "");
                            }
                            else
                            {
                                item.ICDChanDoanDieuTri = chanDoanKemTheo;
                            }
                        }
                    }
                    else
                    {
                        if(item.ChanDoanChinhRaVienICDId != null )
                        {
                            var chanDoan = cds.Where(d => d.Id == item.ChanDoanChinhRaVienICDId).Select(d => d.Ten).First();
                            if (!string.IsNullOrEmpty(item.ChanDoanChinhRaVienGhiChu))
                            {
                                item.ICDChanDoanDieuTri = chanDoan + "(" + item.ChanDoanChinhRaVienGhiChu + ")";
                            }
                            else
                            {
                                item.ICDChanDoanDieuTri = chanDoan;
                            }
                        }
                        else
                        {
                            item.ICDChanDoanDieuTri = item.ChanDoanChinhRaVienGhiChu;
                        }
                    }
                }
            }
            foreach (var noiTruPhieuDieuTriInfoGridVo in queryTask.Result)
            {
                if(listChanDoanTheoNoiTruBenhAn.Any(d=>d.Id == noiTruPhieuDieuTriInfoGridVo.Id))
                {
                    noiTruPhieuDieuTriInfoGridVo.ICDChanDoanDieuTri = listChanDoanTheoNoiTruBenhAn.First(d => d.Id == noiTruPhieuDieuTriInfoGridVo.Id).ICDChanDoanDieuTri;
                    if(string.IsNullOrEmpty(noiTruPhieuDieuTriInfoGridVo.ICDChanDoanDieuTri))
                    {
                        if(!string.IsNullOrEmpty(noiTruPhieuDieuTriInfoGridVo.ThongTinRaVien))
                        {
                            var thongTinRaVien = JsonConvert.DeserializeObject<RaVien>(noiTruPhieuDieuTriInfoGridVo.ThongTinRaVien);
                            noiTruPhieuDieuTriInfoGridVo.ICDChanDoanDieuTri = thongTinRaVien.TenBenhVien;
                        }
                    }
                }
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncLuuTruHoSo(QueryInfo queryInfo)
        {
            //var noiTruBenhAnConIds = _yeuCauNhapVienRepository.TableNoTracking.Where(d => d.YeuCauTiepNhanMeId != null).SelectMany(d => d.YeuCauTiepNhans).Select(d => d.Id).ToList();

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LuuTruHoSoGridVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.TuNgayText))
                {
                    DateTime TuNgayPart = DateTime.Now;
                    if (DateTime.TryParseExact(queryString.TuNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart))
                    {
                        tuNgay = TuNgayPart;
                    }
                }
                if (!string.IsNullOrEmpty(queryString.DenNgayText))
                {
                    DateTime DenNgaysPart = DateTime.Now;
                    if (DateTime.TryParseExact(queryString.DenNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart))
                    {
                        denNgay = DenNgaysPart;
                    }
                }
            }

            var noiTruBenhAnConIds = _yeuCauNhapVienRepository.TableNoTracking
                .Where(d => d.YeuCauTiepNhanMeId != null)
                .SelectMany(d => d.YeuCauTiepNhans)
                .Where(o => o.NoiTruBenhAn != null && o.NoiTruBenhAn.ThoiDiemRaVien != null && (tuNgay == null || tuNgay <= o.NoiTruBenhAn.ThoiDiemRaVien) && (denNgay == null || denNgay >= o.NoiTruBenhAn.ThoiDiemRaVien))
                .Select(d => d.NoiTruBenhAn.Id).ToList();

            var query = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.ThoiDiemRaVien != null && !noiTruBenhAnConIds.Contains(o.Id))
                .Select(s => new LuuTruHoSoGridVo
                {
                    Id = s.Id,
                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    SoBA = s.SoBenhAn,
                    MaBN = s.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    //DoiTuong = (s.YeuCauTiepNhan.CoBHYT != true) ? "Viện phí" : "BHYT (" + s.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    CoBHYT = s.YeuCauTiepNhan.CoBHYT,
                    //MucHuong = s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                    //    ? s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                    //        .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                    //        .Select(a => a.MucHuong).FirstOrDefault() : (int?)null,
                    KhoaNhapVien = s.KhoaPhongNhapVien.Ten,
                    GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                    ThuTuSapXepLuuTru = s.ThuTuSapXepLuuTru,
                    // cập nhật BVHD-3648
                    NamSinh = DateHelper.DOBFormat(s.YeuCauTiepNhan.NgaySinh, s.YeuCauTiepNhan.ThangSinh, s.YeuCauTiepNhan.NamSinh),
                    NgayVaoVien = s.ThoiDiemNhapVien,
                    NgayRaVien = s.ThoiDiemRaVien,

                    TinhTrangRaVien = s.TinhTrangRaVien != null ? s.TinhTrangRaVien.Value.GetDescription() : "", // chuyen vien
                    SoLuuTru = s.SoLuuTru,
                    EnumKetQuaDieuTri = s.KetQuaDieuTri,
                    ChuyenVien = s.ChuyenDenBenhVienId != null ? s.ChuyenDenBenhVien.Ten : "",
                    ThongTinRaVien = s.ThongTinRaVien,
                    // cập nhật BVHD-3648 22/11/2021
                    KhoaPhongId = s.KhoaPhongNhapVienId,
                    CheckBHYT = s.YeuCauTiepNhan.CoBHYT != null && s.YeuCauTiepNhan.CoBHYT != false ? true : false,
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LuuTruHoSoGridVo>(queryInfo.AdditionalSearchString);
                // 0: Chưa sắp xếp, 1: Đã sắp xếp
                if (queryString.ChuaSapXep == false && queryString.DaSapXep == true)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.ChuaSapXep == true && queryString.DaSapXep == false)
                {
                    query = query.Where(p => p.TinhTrang == 0);
                }

                // BHYT,Viện phí
                if (queryString.CheckBHYT == false && queryString.CheckVienPhi == true)
                {
                    query = query.Where(p => p.CheckBHYT == false);
                }
                else if (queryString.CheckBHYT == true && queryString.CheckVienPhi == false)
                {
                    query = query.Where(p => p.CheckBHYT == true);
                }

                if (!string.IsNullOrEmpty(queryString.TuNgayText))
                {
                    DateTime TuNgayPart = DateTime.Now;
                    DateTime.TryParseExact(queryString.TuNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                    query = query.Where(d => d.NgayRaVien.Value >= TuNgayPart);
                }
                if (!string.IsNullOrEmpty(queryString.DenNgayText))
                {
                    DateTime DenNgaysPart = DateTime.Now;
                    DateTime.TryParseExact(queryString.DenNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);

                    if (!string.IsNullOrEmpty(queryString.TuNgayText))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.TuNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        if (DenNgaysPart > TuNgayPart)
                        {
                            query = query.Where(d => d.NgayRaVien.Value <= DenNgaysPart);
                        }
                    }
                    else
                    {
                        query = query.Where(d => d.NgayRaVien.Value <= DenNgaysPart);
                    }


                }
                //: chọn từng khoa có NB nằm nội trú
                if (queryString.KhoaPhongId != null)
                {
                    if (queryString.KhoaPhongId != 0)
                    {
                        query = query.Where(d => d.KhoaPhongId == queryString.KhoaPhongId);
                    }
                }


                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.HoTen,
                        g => g.SoBA,
                        g => g.MaTN,
                        g => g.MaBN,
                        g => g.KhoaNhapVien,
                        g => g.SoLuuTru
                   );

                }

            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                        g => g.HoTen,
                        g => g.SoBA,
                        g => g.MaTN,
                        g => g.MaBN,
                        g => g.KhoaNhapVien,
                        g=>g.SoLuuTru
                    );

            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };

        }

        public ThongTiLuuTruBenhAnNoiTru ThongTiLuuTruBenhAnNoiTru(long noiTruBenhAnId)
        {
            var thongTin = _noiTruBenhAnRepository.TableNoTracking
                                .Where(p => p.Id == noiTruBenhAnId)
                                .Select(s => new ThongTiLuuTruBenhAnNoiTru
                                {
                                    Id = s.Id,
                                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                                    HoTen = s.YeuCauTiepNhan.HoTen,
                                    Tuoi = s.YeuCauTiepNhan.BenhNhan.NamSinh != null ? (DateTime.Now.Year - s.YeuCauTiepNhan.BenhNhan.NamSinh).ToString() : "",
                                    GioiTinh = s.YeuCauTiepNhan.BenhNhan.GioiTinh,
                                    DiaChi = s.YeuCauTiepNhan.BenhNhan.DiaChiDayDu,
                                    NgheNghiep = s.YeuCauTiepNhan.BenhNhan.NgheNghiep != null ? s.YeuCauTiepNhan.BenhNhan.NgheNghiep.Ten : "",
                                    SoBenhAn = s.SoBenhAn,
                                    LoaiBenhAn = s.LoaiBenhAn.GetDescription(),
                                    Khoa = s.KhoaPhongNhapVien.Ten,
                                    CoBHYT = s.YeuCauTiepNhan.CoBHYT,
                                    MucHuong = s.YeuCauTiepNhan.BHYTMucHuong,
                                    //MucHuong = s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                    //? s.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                    //    .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                    //    .Select(a => a.MucHuong).FirstOrDefault() : (int?)null,
                                    SoLuuTru = s.SoLuuTru,
                                    ThuTuSapXepLuuTru = s.ThuTuSapXepLuuTru,
                                    NhanVienThucHien = s.NhanVienLuuTru.User.HoTen,
                                    NgayThucHien = s.NgayLuuTru != null ? s.NgayLuuTru.Value.ApplyFormatDateTimeSACH() : "",

                                    //BVHD-3800
                                    LaCapCuu = s.YeuCauTiepNhan.LaCapCuu ?? s.YeuCauTiepNhan.YeuCauNhapVien.YeuCauKhamBenh.YeuCauTiepNhan.LaCapCuu,

                                    //BVHD-3941
                                    YeuCauTiepNhanId = s.YeuCauTiepNhan.Id,
                                    CoBaoHiemTuNhan = s.YeuCauTiepNhan.CoBHTN
                                });
            return thongTin.First();
        }

        //Kiểm tra bệnh án nhâp trùng nếu nó BHYT ko dc trùng 30 ngày , còn viện phí ko dc trung 1 năm.
        public Task<bool> KiemTraThuTuSapXepLuuTruBATrung(long noiTruBenhAnId, string thuTuSapXepLuuTruBA)
        {
            var currentDate = DateTime.Now;
            var noiTruBenh = _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == noiTruBenhAnId)
                                               .Include(c => c.YeuCauTiepNhan).Include(c => c.KhoaPhongNhapVien)
                                               .FirstOrDefault();

            var khoaPhongNhapVienId = noiTruBenh.KhoaPhongNhapVienId;
            if (noiTruBenh.YeuCauTiepNhan.CoBHYT == true)
            {
                var thuTuSapXepLuuTrus = _noiTruBenhAnRepository.TableNoTracking.Where(p => p.KhoaPhongNhapVienId == khoaPhongNhapVienId && p.YeuCauTiepNhan.CoBHYT == true
                                                    && p.NgayLuuTru != null && (currentDate - p.NgayLuuTru).Value.TotalDays <= 30
                                                    && p.Id != noiTruBenhAnId).Select(c => c.ThuTuSapXepLuuTru);

                return thuTuSapXepLuuTrus.AnyAsync(c => c == thuTuSapXepLuuTruBA);
            }
            else
            {

                var thuTuSapXepLuuTrus = _noiTruBenhAnRepository.TableNoTracking.Where(p => p.KhoaPhongNhapVienId == khoaPhongNhapVienId && p.YeuCauTiepNhan.CoBHYT != true
                                                 && p.NgayLuuTru != null && (currentDate - p.NgayLuuTru).Value.TotalDays <= 365
                                                  && p.Id != noiTruBenhAnId).Select(c => c.ThuTuSapXepLuuTru);

                return thuTuSapXepLuuTrus.AnyAsync(c => c == thuTuSapXepLuuTruBA);
            }
        }

        #region cập nhật 3684
        public virtual byte[] ExportBaoCaoSoLuuTruBenhAn(IList<LuuTruHoSoGridVo> luuTruHoSoGridVos, QueryInfo queryInfo)
        {

            var dataBaoCaos = luuTruHoSoGridVos.ToList();

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<LuuTruHoSoGridVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BC05");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // set column
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 40;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 18;
                    worksheet.Column(8).Width = 18;
                    worksheet.Column(9).Width = 50;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N","O" };
                    var worksheetTitleBacHa = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleTuNgayDenNgay = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;

                    var tuNgay = string.Empty;
                    var denNgay = string.Empty;
                    var khoa = string.Empty;
                    int checkTenBaoCao = 1; // loại 1(true,true), 2(true,false),3(false,true)
                   
                    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
                    {
                        var queryString = JsonConvert.DeserializeObject<LuuTruHoSoGridVo>(queryInfo.AdditionalSearchString);

                        if (!string.IsNullOrEmpty(queryString.TuNgayText))
                        {
                            DateTime TuNgayPart = DateTime.Now;
                            DateTime.TryParseExact(queryString.TuNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                            tuNgay = TuNgayPart.ApplyFormatDateTime();
                        }
                        if (!string.IsNullOrEmpty(queryString.DenNgayText))
                        {
                            DateTime denNgayPart = DateTime.Now;
                            DateTime.TryParseExact(queryString.TuNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgayPart);
                            denNgay = denNgayPart.ApplyFormatDateTime();
                        }
                        if (queryString.KhoaPhongId == 0)
                        {
                            khoa = " Toàn viện";
                        }
                        if (queryString.KhoaPhongId != 0 && queryString.KhoaPhongId != null)
                        {
                            khoa = _khoaPhongRepository.TableNoTracking.Where(d => d.Id == queryString.KhoaPhongId).Select(d => d.Ten).FirstOrDefault();
                        }
                        if(queryString.CheckBHYT == true && queryString.CheckVienPhi == true)
                        {
                            checkTenBaoCao = 1;
                        }
                        if (queryString.CheckBHYT == true && queryString.CheckVienPhi == false)
                        {
                            checkTenBaoCao = 2;
                        }
                        if (queryString.CheckBHYT == false && queryString.CheckVienPhi == true)
                        {
                            checkTenBaoCao = 3;
                        }
                    }


                    using (var range = worksheet.Cells[worksheetTitleBacHa])
                    {
                        range.Worksheet.Cells[worksheetTitleBacHa].Merge = true;
                        range.Worksheet.Cells[worksheetTitleBacHa].Value = TenBaoCao(checkTenBaoCao) + " " + (checkTenBaoCao == 1 ? khoa.ToUpper() : "");
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.SetFromFont(new Font("Times New Roman", 15));
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.Bold = true;
                    }

                    

                    using (var range = worksheet.Cells[worksheetTitleTuNgayDenNgay])
                    {

                        range.Worksheet.Cells["A2:N2"].Merge = true;
                        range.Worksheet.Cells["A2:N2"].Value = "Từ: " + (!string.IsNullOrEmpty(tuNgay) ? tuNgay :"         " ) + " đến: " + (!string.IsNullOrEmpty(denNgay) ? denNgay : "         ");
                        range.Worksheet.Cells["A2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:N2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:N2"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A2:N2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:N2"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["A3:N3"])
                    {

                        range.Worksheet.Cells["A3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:N3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A3:N3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:N3"].Style.Font.Bold = true;

                        range.Worksheet.Cells["G3:G3"].Merge = true;
                        range.Worksheet.Cells["G3:G3"].Value = "Khoa";

                        range.Worksheet.Cells["H3:H3"].Merge = true;
                        range.Worksheet.Cells["H3:H3"].Value = khoa;
                    }

                    using (var range = worksheet.Cells["A4:O5"])
                    {
                        range.Worksheet.Cells["A4:O5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:O5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:O5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:O5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:O5"].Style.Font.Bold = true;

                        ////Set column A to F
                        //string[,] SetColumns = { { "A" , "STT" },{ "B", "Thứ tự sắp xếp" }, { "C", "Số lưu trữ" } , { "D", "Họ tên bệnh nhân" },
                        //   { "E", "Tuổi" },{ "F", "ICD - Chẩn đoán điều trị" } };
                        ////{ "E", "Ngày giờ vào/ra viện" },  { "H", "ICD - Chẩn đoán điều trị" }

                        //for (int i = 0; i < SetColumns.Length / 2; i++)
                        //{
                        //    var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 5).ToString();
                        //    range.Worksheet.Cells[setColumn].Merge = true;
                        //    range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        //}

                        // STT
                        range.Worksheet.Cells["A4:A4"].Merge = true;
                        range.Worksheet.Cells["A4:A4"].Value = "STT";

                        // Thứ tự sắp xếp
                        range.Worksheet.Cells["B4:B4"].Merge = true;
                        range.Worksheet.Cells["B4:B4"].Value = "Thứ tự sắp xếp";

                        // Thứ tự sắp xếp
                        range.Worksheet.Cells["C4:C4"].Merge = true;
                        range.Worksheet.Cells["C4:C4"].Value = "Số lưu trữ";

                        // Thứ tự sắp xếp
                        range.Worksheet.Cells["D4:D4"].Merge = true;
                        range.Worksheet.Cells["D4:D4"].Value = "Họ tên bệnh nhân";

                        //Set column D to E
                        // tuổi
                        range.Worksheet.Cells["E4:F4"].Merge = true;
                        range.Worksheet.Cells["E4:F4"].Value = "Tuổi";

                        
                        range.Worksheet.Cells["E5:E5"].Value = "Nam";
                        range.Worksheet.Cells["F5:F5"].Value = "Nữ";


                        //Set column F to G
                        // tuổi
                        range.Worksheet.Cells["G4:H4"].Merge = true;
                        range.Worksheet.Cells["G4:H4"].Value = "Ngày giờ vào/ra viện";

                        range.Worksheet.Cells["G5:G5"].Value = "Vào viện";
                        range.Worksheet.Cells["H5:H5"].Value = "Ra viện";


                        //Set column F to G
                        // tuổi
                        range.Worksheet.Cells["I4:I4"].Merge = true;
                        range.Worksheet.Cells["I4:I4"].Value = "ICD-Chẩn đoán điều trị";


                        //Set column I to I
                        // Khỏi
                        range.Worksheet.Cells["J4:J5"].Merge = true;
                        range.Worksheet.Cells["J4:J5"].Value = "Khỏi".ToUpper();


                        //Set column J to J
                        // Đỡ/ giảm
                        range.Worksheet.Cells["K4:K5"].Merge = true;
                        range.Worksheet.Cells["K4:K5"].Value = "Đỡ/ giảm".ToUpper();


                        //Set column K to K
                        // KHÔNG THAY ĐỔI
                        range.Worksheet.Cells["L4:L5"].Merge = true;
                        range.Worksheet.Cells["L4:L5"].Value = "KHÔNG THAY ĐỔI".ToUpper();


                        //Set column L to L
                        // NẶNG HƠN
                        range.Worksheet.Cells["M4:M5"].Merge = true;
                        range.Worksheet.Cells["M4:M5"].Value = "NẶNG HƠN".ToUpper();

                        //Set column M to M
                        // CHUYỂN VIỆN
                        range.Worksheet.Cells["N4:N5"].Merge = true;
                        range.Worksheet.Cells["N4:N5"].Value = "CHUYỂN VIỆN".ToUpper();

                        //Set column N to N
                        // TỬ VONG
                        range.Worksheet.Cells["O4:O5"].Merge = true;
                        range.Worksheet.Cells["O4:O5"].Value = "TỬ VONG".ToUpper();


                       


                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                   
                    var datas = dataBaoCaos.ToArray();
                    var index = 6;
                    var manager = new PropertyManager<LuuTruHoSoGridVo>(requestProperties);
                    int stt = 1;
                    foreach (var item  in datas)
                    {
                        manager.CurrentObject = item;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = stt;
                        worksheet.Cells["B" + index].Value = item.ThuTuSapXepLuuTru;
                        worksheet.Cells["C" + index].Value = item.SoLuuTru;
                        worksheet.Cells["D" + index].Value = item.HoTen;

                        var tuoiNam = string.Empty;
                        var tuoiNu = string.Empty;
                        if (item.GioiTinh != null)
                        {
                            if(item.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam)
                            {
                                tuoiNam = item.NamSinh;
                            }
                            else if (item.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu)
                            {
                                tuoiNu = item.NamSinh;
                            }
                        }

                        worksheet.Cells["E" + index].Value = tuoiNam;
                        worksheet.Cells["F" + index].Value = tuoiNu;
                        worksheet.Cells["G" + index].Value = item.NgayVaoVienDisplay;
                        worksheet.Cells["H" + index].Value = item.NgayRaVienDisplay;
                        worksheet.Cells["I" + index].Value = item.ICDChanDoanDieuTri;

                        using (var range = worksheet.Cells["I" + index + ":O" + index])
                        {
                            range.Worksheet.Cells["I" + index + ":O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["I" + index + ":O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["I" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["I" + index + ":O" + index].Style.Font.Color.SetColor(Color.Black);
                        
                            range.Worksheet.Cells["I" + index + ":O" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        }
                        worksheet.Cells["J" + index].Value = (item.EnumKetQuaDieuTri != null && item.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.Khoi) ? "X":"";
                        
                        worksheet.Cells["K" + index].Value = (item.EnumKetQuaDieuTri != null && item.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.Do) ? "X" : "";
                        worksheet.Cells["L" + index].Value = (item.EnumKetQuaDieuTri != null && item.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.KhongThayDoi) ? "X" : "";
                        worksheet.Cells["M" + index].Value = (item.EnumKetQuaDieuTri != null && item.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.NangHon) ? "X" : "";

                        worksheet.Cells["N" + index].Value = item.ChuyenVien;
                        worksheet.Cells["O" + index].Value = (item.EnumKetQuaDieuTri != null && item.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.TuVong) ? "X" : "";



                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.White);
                        }

                        worksheet.Row(index).Height = 20.5;
                        index++;
                        stt++;
                    }
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    ///

                    worksheet.Cells[worksheetFirstLast].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells[worksheetFirstLast].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[worksheetFirstLast].Style.Font.Bold = true;
                    worksheet.Cells[worksheetFirstLast].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells[worksheetFirstLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                   


                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                    {
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                    }


                    // TÍNH TỔNG 
                    var toatalKhoi = luuTruHoSoGridVos.Where(d => d.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.Khoi).ToList();

                    var toatalDo = luuTruHoSoGridVos.Where(d => d.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.Do).ToList();

                    var toatalKhongThayDoi = luuTruHoSoGridVos.Where(d => d.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.KhongThayDoi).ToList();

                    var toatalNangHon = luuTruHoSoGridVos.Where(d => d.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.NangHon).ToList();

                    //var toatalTuVong = luuTruHoSoGridVos.Where(d => d.EnumKetQuaDieuTri == Enums.EnumKetQuaDieuTri.TuVong).ToList();

                    var toatalChuyenVien = luuTruHoSoGridVos.Where(d => d.ChuyenVien != null && d.ChuyenVien !="").ToList();

                    // total grid
                    using (var range = worksheet.Cells["A" + index + ":I" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":I" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":I" + index].Value = "TỔNG SỐ HSBA: ".ToUpper() + datas.Count()+" HSBA".ToUpper();
                    }

                    worksheet.Cells["J" + index].Value = toatalKhoi.Count;
                    worksheet.Cells["K" + index].Value = toatalDo.Count;
                    worksheet.Cells["L" + index].Value = toatalKhongThayDoi.Count;
                    worksheet.Cells["M" + index].Value = toatalNangHon.Count;
                    worksheet.Cells["N" + index].Value = toatalChuyenVien.Count;
                    worksheet.Cells["O" + index].Value = "";


                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        private string TenBaoCao(int check)
        {
            var returnKq = string.Empty;
            if(check == 1)
            {
                returnKq = "SỔ LƯU TRỮ HỒ SƠ BỆNH ÁN";
            }
            else if (check == 2)
            {
                returnKq = "SỔ LƯU TRỮ HỒ SƠ BỆNH ÁN BHYT";
            }
            else if (check == 3)
            {
                returnKq = "SỔ LƯU TRỮ HỒ SƠ BỆNH ÁN";
            }
            return returnKq;
        }
        //private string NgayThangRaVien(string tuNgay , string denNgay)
        //{
        //    return (!string.IsNullOrEmpty(tuNgay)? "Từ:" + tuNgay + "đến:" :"") + (!string.IsNullOrEmpty(denNgay) ?"đến:" denNgay + : "");
        //}
        #endregion
    }
}
