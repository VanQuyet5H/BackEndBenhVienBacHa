using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using static Camino.Core.Domain.ValueObject.KhamDoan.KetLuanKhamSucKhoeDoanVo;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {

        public async Task<GridDataSource> GetDataForGridAsyncDanhSachKetLuanKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<KetLuanKhamSucKhoeDoanVo>().ToArray(), TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var queryString = JsonConvert.DeserializeObject<KetLuanKhamSucKhoeDoanVo>(queryInfo.AdditionalSearchString);

            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(p => p.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                         && p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId
                         && p.YeuCauTiepNhans.Any()
                         && p.BenhNhanId != null)
                .Select(s => new KetLuanKhamSucKhoeDoanVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
                    CongTyKhamSucKhoeId = s.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                    HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
                    MaTN = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.MaYeuCauTiepNhan).FirstOrDefault(),
                    MaBN = s.BenhNhan.MaBN,
                    MaNhanVien = s.MaNhanVien,
                    HoTen = s.HoTen,
                    TenNgheNghiep = s.NgheNghiep.Ten,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    SoDienThoai = s.SoDienThoai,
                    Email = s.Email,
                    SoChungMinhThu = s.SoChungMinhThu,
                    TenDanToc = s.DanToc.Ten,
                    TenTinhThanh = s.TinhThanh.Ten,
                    NhomDoiTuongKhamSucKhoe = s.NhomDoiTuongKhamSucKhoe,
                    KSKKetLuanPhanLoaiSucKhoe = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KSKKetLuanPhanLoaiSucKhoe).FirstOrDefault(),
                    GoiDichVuId = s.GoiKhamSucKhoeId,
                    NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
                    LaHopDongDaKetLuan = s.HopDongKhamSucKhoe.DaKetThuc,
                    TrangThaiYeuCauTiepNhan = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.TrangThaiYeuCauTiepNhan).FirstOrDefault(),
                    // BVHD-3722
                    KetQuaKhamSucKhoeData = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KetQuaKhamSucKhoeData).FirstOrDefault()
                });
            if (queryString.LaHopDongDaKetLuan == true)
            {
                query = query.Where(p => p.LaHopDongDaKetLuan == true);
            }
            else
            {
                query = query.Where(p => p.LaHopDongDaKetLuan != true);
            }
            // 0: ChuaKetLuan, 1 : DaKetLuan
            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                query = query.Where(p => p.TinhTrang == 1);
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                query = query.Where(p => p.TinhTrang == 0);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                   g => g.MaTN,
                   g => g.MaBN,
                   g => g.MaNhanVien,
                   g => g.HoTen,
                   g => g.TenNgheNghiep,
                   g => g.NamSinh.ToString(),
                   g => g.SoDienThoai,
                   g => g.Email,
                   g => g.SoChungMinhThu,
                   g => g.TenTinhThanh,
                   g => g.NhomDoiTuongKhamSucKhoe
               );
            }
            var queryResult = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            var result = new List<KetLuanKhamSucKhoeDoanVo>();
            if (queryResult.Any())
            {
                var yctnIds = queryResult.Select(o => o.YeuCauTiepNhanId).ToList();
                var yeuCauDichVus = BaseRepository.TableNoTracking.Where(tn => yctnIds.Contains(tn.Id))
                    .Select(s => new
                    {
                        s.Id,
                        DichVuDaThucHien = s.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId
                                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(c => c.Id).Count()
                                           + s.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId
                                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                                                                    && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(c => c.Id).Count(),
                        TongDichVu = s.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(c => c.Id).Count()
                               + s.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(c => c.Id).Count()

                    }).ToList();
                foreach (var item in queryResult)
                {
                    item.DichVuDaThucHien = yeuCauDichVus.Where(yc => yc.Id == item.YeuCauTiepNhanId).Select(yc => yc.DichVuDaThucHien).FirstOrDefault();
                    item.TongDichVu = yeuCauDichVus.Where(yc => yc.Id == item.YeuCauTiepNhanId).Select(yc => yc.TongDichVu).FirstOrDefault();
                    result.Add(item);
                }
            }
            return new GridDataSource { Data = queryResult, TotalRowCount = queryResult.Count() };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachKetLuanKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;

            }
            var queryString = JsonConvert.DeserializeObject<KetLuanKhamSucKhoeDoanVo>(queryInfo.AdditionalSearchString);

            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                   .Where(p => p.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                   && p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId
                   && p.YeuCauTiepNhans.Any()
                   && p.BenhNhanId != null)
                .Select(s => new KetLuanKhamSucKhoeDoanVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
                    CongTyKhamSucKhoeId = s.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                    HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
                    MaTN = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.MaYeuCauTiepNhan).FirstOrDefault(),
                    MaBN = s.BenhNhan.MaBN,
                    MaNhanVien = s.MaNhanVien,
                    HoTen = s.HoTen,
                    TenNgheNghiep = s.NgheNghiep.Ten,
                    GioiTinh = s.GioiTinh,
                    NamSinh = s.NamSinh,
                    SoDienThoai = s.SoDienThoai,
                    Email = s.Email,
                    SoChungMinhThu = s.SoChungMinhThu,
                    TenDanToc = s.DanToc.Ten,
                    TenTinhThanh = s.TinhThanh.Ten,
                    NhomDoiTuongKhamSucKhoe = s.NhomDoiTuongKhamSucKhoe,
                    KSKKetLuanPhanLoaiSucKhoe = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KSKKetLuanPhanLoaiSucKhoe).FirstOrDefault(),
                    NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
                    LaHopDongDaKetLuan = s.HopDongKhamSucKhoe.DaKetThuc,
                    TrangThaiYeuCauTiepNhan = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.TrangThaiYeuCauTiepNhan).FirstOrDefault()
                });
            if (queryString.LaHopDongDaKetLuan == true)
            {
                query = query.Where(p => p.LaHopDongDaKetLuan == true);
            }
            else
            {
                query = query.Where(p => p.LaHopDongDaKetLuan != true);
            }
            // 0: ChuaKetLuan, 1 : DaKetLuan
            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                query = query.Where(p => p.TinhTrang == 1);
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                query = query.Where(p => p.TinhTrang == 0);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                   g => g.MaTN,
                   g => g.MaBN,
                   g => g.MaNhanVien,
                   g => g.HoTen,
                   g => g.TenNgheNghiep,
                   g => g.NamSinh.ToString(),
                   g => g.SoDienThoai,
                   g => g.Email,
                   g => g.SoChungMinhThu,
                   g => g.TenTinhThanh,
                   g => g.NhomDoiTuongKhamSucKhoe
               );
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public YeuCauTiepNhan GetYeuCauTiepNhan(long yeuCauTiepNhanId, long hopDongKhamSucKhoeNhanVienId)
        {
            return BaseRepository.Table
                .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.GoiKhamSucKhoe)
                .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauKhamBenhLichSuTrangThais)
                .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.GoiKhamSucKhoe)
                .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.PhongBenhVienHangDois)
                .Include(p => p.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.HopDongKhamSucKhoe)
                .Where(p => p.Id == yeuCauTiepNhanId && p.HopDongKhamSucKhoeNhanVienId == hopDongKhamSucKhoeNhanVienId).FirstOrDefault();
        }

        public void SaveBanInKhamDoanTiepNhan(long yeuCauTiepNhanId, string ketQuaKhamSucKhoeData, string ketLuanData)
        {
            var yctn = BaseRepository.GetById(yeuCauTiepNhanId);
            yctn.KetQuaKhamSucKhoeData = ketQuaKhamSucKhoeData;
            yctn.KSKKetLuanData = ketLuanData ;
            yctn.LoaiLuuInKetQuaKSK = true;
            BaseRepository.Context.SaveChanges();
        }

        public List<KetLuanKhamSucKhoeDoanDichVuKhamTemplateGroupVo> KetLuanKhamSucKhoeDoanDichVuKhamVos(YeuCauTiepNhan yeuCauTiepNhan)
        {
            var ketLuanKhamSucKhoes = new List<KetLuanKhamSucKhoeDoanDichVuKhamTemplateGroupVo>();
            ketLuanKhamSucKhoes.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(p => p.GoiKhamSucKhoeId != null && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Select(item => new KetLuanKhamSucKhoeDoanDichVuKhamTemplateGroupVo
                {
                    Type = item.ChuyenKhoaKhamSucKhoe,
                    Title = item.ChuyenKhoaKhamSucKhoe.GetDescription(),
                    ThongTinKhamTheoDichVuTemplate = item.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = item.ThongTinKhamTheoDichVuData
                }));
            return ketLuanKhamSucKhoes;
        }

        public async Task<List<KetQuaMauVo>> GetKetQuaMau(YeuCauTiepNhan yeuCauTiepNhan)
        {
            var ketQuaMaus = new List<KetQuaMauVo>();
            var binhThuong = "binh thuong";
            var bth = "bth";
            var bt = "bt";
            var phanLoaiMau = "phanloai";
            var lstPhanLoaiSucKhoe = EnumHelper.GetListEnum<PhanLoaiSucKhoe>();
            foreach (var item in yeuCauTiepNhan.YeuCauKhamBenhs)
            {
                if (!string.IsNullOrEmpty(item.ThongTinKhamTheoDichVuData))
                {
                    var thongTinKhamTheoDichVuData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(item.ThongTinKhamTheoDichVuData);
                    if (item.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat || item.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat || item.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong)
                    {
                        //Cheat
                        foreach (var data in thongTinKhamTheoDichVuData.DataKhamTheoTemplate)
                        {
                            if (!string.IsNullOrEmpty(data.Value) && !data.Value.RemoveDiacritics().ToLower().Trim().Contains(binhThuong) && !data.Value.RemoveDiacritics().ToLower().Trim().Contains(bth) &&
                                !data.Value.RemoveDiacritics().ToLower().Trim().Contains(bt) && lstPhanLoaiSucKhoe.All(p => p.GetDescription() != data.Value) && !data.Id.ToLower().Contains(phanLoaiMau)
                                && (data.Id == "CacBenhVeMat" || data.Id == "CacBenhRangHamMat" || data.Id == "CacBenhTaiMuiHong")
                                )
                            {
                                ketQuaMaus.Add(new KetQuaMauVo { IsCheck = true, Value = data.Value });
                            }
                        }
                    }
                    else
                    {
                        foreach (var data in thongTinKhamTheoDichVuData.DataKhamTheoTemplate)
                        {
                            if (!string.IsNullOrEmpty(data.Value) && !data.Value.RemoveDiacritics().ToLower().Trim().Contains(binhThuong) && !data.Value.RemoveDiacritics().ToLower().Trim().Contains(bth) &&
                                !data.Value.RemoveDiacritics().ToLower().Trim().Contains(bt) && lstPhanLoaiSucKhoe.All(p => p.GetDescription() != data.Value) && !data.Id.ToLower().Contains(phanLoaiMau))
                            {
                                ketQuaMaus.Add(new KetQuaMauVo { IsCheck = true, Value = data.Value });
                            }
                        }
                    }

                }
            }
            if (!string.IsNullOrEmpty(yeuCauTiepNhan.KSKKetQuaCanLamSang) && !yeuCauTiepNhan.KSKKetQuaCanLamSang.RemoveDiacritics().ToLower().Trim().Contains(binhThuong)
                && !yeuCauTiepNhan.KSKKetQuaCanLamSang.RemoveDiacritics().ToLower().Trim().Contains(bt) && !yeuCauTiepNhan.KSKKetQuaCanLamSang.RemoveDiacritics().ToLower().Trim().Contains(bth))
            {
                ketQuaMaus.Add(new KetQuaMauVo { IsCheck = true, Value = yeuCauTiepNhan.KSKKetQuaCanLamSang });
            }
            var ketQuaDistinct = ketQuaMaus.GroupBy(g => new
            {
                g.IsCheck,
                g.Value
            })
                .Select(g => g.First());
            return ketQuaDistinct.ToList();
        }

        public string InSoKSKDinhKy(InSoKSKVaKetQua inSoKSKVaKetQua)
        {
            var content = string.Empty;
            if (inSoKSKVaKetQua.IsInSoKSKDinhKy)
            {

            }
            else
            {

            }
            return content;
        }
        #region 

        public async Task<List<DanhSachDichVuKhamGrid>> GetDataKetQuaKSKDoanEdit(long hopDongKhamSucKhoeNhanVienId)
        {
            //var thongTinNhanVienKham =
            //BaseRepository.TableNoTracking
            //    .Include(x => x.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
            //     .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.KetQuaXetNghiemChiTiets)
            //     .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //    .Include(x => x.KetQuaSinhHieus)
            //    .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
            //    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.BenhNhan)
            //    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HopDongKhamSucKhoe).ThenInclude(z => z.CongTyKhamSucKhoe)
            //    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
            //    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
            //    .OrderByDescending(x => x.Id)
            //    .FirstOrDefault(x => x.HopDongKhamSucKhoeNhanVienId == hopDongKhamSucKhoeNhanVienId && x.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy);
            var yctnId = BaseRepository.TableNoTracking.Where(x => x.HopDongKhamSucKhoeNhanVienId == hopDongKhamSucKhoeNhanVienId && x.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy).Select(d => d.Id).FirstOrDefault();

            var thongTinKham = BaseRepository.TableNoTracking.Where(x => x.Id == yctnId)
                              .Select(d => new
                              {
                                  YeuCauKhamBenhIds = d.YeuCauKhamBenhs.Select(g => g.Id).ToList(),
                                  YeuCauDichVuKyThuatIds = d.YeuCauDichVuKyThuats.Select(g => g.Id).ToList(),
                                  KetQuaKhamSucKhoeData = d.KetQuaKhamSucKhoeData,
                                  LoaiLuuInKetQuaKSK = d.LoaiLuuInKetQuaKSK
                              }).FirstOrDefault();


            #region // yeu cau kham
            var infoYeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(s => thongTinKham.YeuCauKhamBenhIds.Contains(s.Id) && s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Select(v => new DanhSachDichVuKhamGrid
                {
                    Id = v.Id,
                    //TenDichVu = v.DichVuKhamBenhBenhVien?.Ten,
                    ThongTinKhamTheoDichVuTemplate = v.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = v.ThongTinKhamTheoDichVuData,
                    TrangThaiDVKham = (int)v.TrangThai,
                    DichVuKhamBenhVienId = v.DichVuKhamBenhBenhVienId
                }).ToList();

            var dichVuKhamBenhVienIds = infoYeuCauKhams.Select(d => d.DichVuKhamBenhVienId).ToList();

            var tenDichVuKhams = _dichVuKhamBenhBenhVienRepository.TableNoTracking.Where(d => dichVuKhamBenhVienIds.Contains(d.Id)).Select(d => new { d.Id, d.Ten }).ToList();

            foreach (var item in infoYeuCauKhams)
            {
                if (tenDichVuKhams.Where(d => d.Id == item.DichVuKhamBenhVienId).Select(d => d.Ten).Count() > 0)
                {
                    item.TenDichVu = tenDichVuKhams.Where(d => d.Id == item.DichVuKhamBenhVienId).Select(d => d.Ten).First();
                }
            }
            #endregion

            #region // YeuCauDichVuKyThuat 
            // +CDHA TDCN
            var infoDichVuKyThuatCDHATDCNs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(s => thongTinKham.YeuCauDichVuKyThuatIds.Contains(s.Id) && s.GoiKhamSucKhoeId != null &&
                 s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                 (s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                 .Select(z => new
                 {
                     DataKetQuaCanLamSang = z.DataKetQuaCanLamSang,
                     TenDichVuKyThuat = z.TenDichVu,
                     Id = z.Id,
                     GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                     TrangThaiDVKham = (int)z.TrangThai
                 }).ToList();
            #endregion
            #region // + XetNghiem


            var infoDichVuKyThuatXNs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => thongTinKham.YeuCauDichVuKyThuatIds.Contains(s.Id) && s.GoiKhamSucKhoeId != null &&
                s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
               .Select(z => new DVKTXetNghiem
               {
                   TenDichVuKyThuat = z.TenDichVu,
                   Id = z.Id,
                   GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                   TrangThai = (int)z.TrangThai
               }).ToList();

            var ids = infoDichVuKyThuatXNs.Select(d => d.Id).ToList();

            var listPhienXetNghiemChiTietss = _phienXetNghiemChiTietRepository.TableNoTracking
                .Include(d => d.KetQuaXetNghiemChiTiets)
                .Where(s => ids.Contains(s.YeuCauDichVuKyThuatId)).ToList();


            // lấy info phiên kêt qua xet nghien
            if (infoDichVuKyThuatXNs != null)
            {
                foreach (var idDichVuKyThuat in infoDichVuKyThuatXNs)
                {

                    idDichVuKyThuat.DataKetQuaCanLamSang = listPhienXetNghiemChiTietss.Where(d => d.YeuCauDichVuKyThuatId == idDichVuKyThuat.Id)
                                                                                       .Select(v => new DataKetQuaCanLamSangVo
                                                                                       {
                                                                                           KetQuaXetNghiemChiTiets = v.KetQuaXetNghiemChiTiets.ToList(),
                                                                                           LanThucHien = v.LanThucHien,
                                                                                           KetLuan = v.KetLuan
                                                                                       }).OrderBy(s => s.LanThucHien)
                                                                                        .LastOrDefault();
                }
            }
            #endregion

            #region //// lấy những dịch vụ kỹ thuật khác cls -> kết quả để null
            var infoDichVuKyThuatKhacXNKhacCDHAKhacPTTTs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => thongTinKham.YeuCauDichVuKyThuatIds.Contains(s.Id) &&
                s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem &&
                s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh &&
                s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang)
               .Select(z => new
               {
                   TenDichVuKyThuat = z.TenDichVu,
                   Id = z.Id,
                   GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                   TrangThai = (int)z.TrangThai
               }).ToList();
            #endregion

            #region // PTTT
            var infoDichVuKyThuatPTTTs = _yeuCauDichVuKyThuatRepository.TableNoTracking
               .Where(s => thongTinKham.YeuCauDichVuKyThuatIds.Contains(s.Id) &&
               s.GoiKhamSucKhoeId != null &&
               s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
               && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
              .Select(z => new InfoPTTT
              {
                  TenDichVuKyThuat = z.TenDichVu,
                  Id = z.Id,
                  GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                  TrangThai = (int)z.TrangThai,
              }).ToList();

            var dichVuKyThuatPTTTIds = infoDichVuKyThuatPTTTs.Select(d => d.Id).ToList();

            var ketQuaPTTTs = _yeuCauDichVuKyThuatTuongTrinhPTTTRepository.TableNoTracking.Where(d => dichVuKyThuatPTTTIds.Contains(d.Id)).Select(d => new { d.Id, d.GhiChuCaPTTT }).ToList();

            foreach (var item in infoDichVuKyThuatPTTTs)
            {
                if(ketQuaPTTTs.Where(d=>d.Id== item.Id).Select(d=>d).Count() > 0)
                {
                    item.KetQuaDichVu = ketQuaPTTTs.Where(d => d.Id == item.Id).Select(d => d.GhiChuCaPTTT).FirstOrDefault();
                }
            }

            #endregion

            var dichVuXetNghiemKetNoiChiSos = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Select(s => new
            {
                Id = s.Id,
                DichVuXetNghiemId = s.DichVuXetNghiemId,
                MauMayXetNghiemId = s.MauMayXetNghiemId,
                TenKetNoi = s.TenKetNoi
            }).ToList();

            var data = new KetQuaKhamSucKhoeVo();
            var tableKham = "";
            var tableKyThuat = "";
            List<DanhSachDichVuKhamGrid> listDichVu = new List<DanhSachDichVuKhamGrid>();
            List<DanhSachDichVuKhamGrid> listDichVuCu = new List<DanhSachDichVuKhamGrid>();
            List<long> listDichVuKyThuatTheoGoi = new List<long>();
            if (thongTinKham != null)
            {

                // DV Kham
                if (infoYeuCauKhams.Any())
                {
                    foreach (var itemDv in infoYeuCauKhams)
                    {
                        DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                        dvObject.Id = itemDv.Id;
                        dvObject.NhomId = EnumNhomGoiDichVu.DichVuKhamBenh;
                        dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                        dvObject.TenDichVu = itemDv.TenDichVu;
                        dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                        dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;


                        // lấy tất cae dịch vụ khám bắt đầu khám và khác hủy (!=1 && !=6)
                        // nếu ThongTinKhamTheoDichVuData == null => set default model json theo từng dịch vụ 
                        // trạng thái dịch vụ == 5 thì KetQuaDichVu mới được gán 

                        if (itemDv.ThongTinKhamTheoDichVuTemplate != null && itemDv.ThongTinKhamTheoDichVuData != null &&  itemDv.TrangThaiDVKham != 6) 
                        {
                            var jsonOjbectTemplate = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(itemDv.ThongTinKhamTheoDichVuTemplate);
                            var jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);
                            if(jsonOjbectData.DataKhamTheoTemplate.Count() == 0)
                            {
                                if (dvObject.TenDichVu == "Khám Ngoại")
                                {
                                    itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa);
                                }
                                if (dvObject.TenDichVu == "Khám Mắt")
                                {
                                    itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.Mat);
                                }

                                if (dvObject.TenDichVu == "Khám Răng Hàm Mặt")
                                {
                                    itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.RangHamMat);
                                }

                                if (dvObject.TenDichVu == "Khám Tai Mũi Họng")
                                {
                                    itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                }
                                if (dvObject.TenDichVu == "Khám Da liễu")
                                {
                                    itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                }
                                if (dvObject.TenDichVu == "Nội khoa")
                                {
                                    itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa);
                                }
                                if (dvObject.TenDichVu == "Sản phụ khoa")
                                {
                                    itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa);
                                }

                                jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);
                            }
                            
                         
                            foreach (var itemx in jsonOjbectTemplate.ComponentDynamics)
                            {
                                var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemx.Id);
                                if (kiemTra.Any())
                                {
                                    switch (itemx.Id)
                                    {
                                        case "TuanHoan":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if(dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                            break;
                                        case "HoHap":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                            break;

                                        case "TieuHoa":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                            break;

                                        case "ThanTietLieu":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                            break;

                                        case "NoiTiet":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                            break;

                                        case "CoXuongKhop":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                            break;

                                        case "ThanKinh":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                            break;

                                        case "TamThan":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                            break;

                                        case "NgoaiKhoa":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenNgoaiKhoa;
                                            break;
                                        case "SanPhuKhoa":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenSanPhuKhoa;
                                            break;


                                        case "CacBenhVeMat":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaMat;
                                            break;
                                        case "CacBenhTaiMuiHong":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaTaiMuiHong;
                                            break;

                                        case "CacBenhRangHamMat":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                            break;
                                        case "HamTren":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                            break;
                                        case "HamDuoi":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                            break;

                                        case "DaLieu":
                                            if (kiemTra.FirstOrDefault().Value != null)
                                            {
                                                if (dvObject.TrangThaiDVKham == 5)
                                                {
                                                    dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                }
                                            }
                                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenDaLieu;
                                            break;
                                        default:
                                            //do a different thing
                                            break;
                                    }
                                }

                            }
                        }
                        dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVu;
                        listDichVu.Add(dvObject);
                    }

                }

                // list theo yêu cầu tiếp nhận
                // CDHA TDCN
                if (infoDichVuKyThuatCDHATDCNs.Any())
                {

                    foreach (var itemDv in infoDichVuKyThuatCDHATDCNs)
                    {
                        DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                        dvObject.Id = itemDv.Id;
                        dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                        dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                        dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                        dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                        dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                        dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                        dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;

                        if (itemDv.DataKetQuaCanLamSang != null && itemDv.TrangThaiDVKham == 3 ) // != 1 => dịch vụ chưa thực hiện
                        {
                            var jsonOjbect = JsonConvert.DeserializeObject<DataCLS>(itemDv.DataKetQuaCanLamSang);

                            //dvObject.KetQuaDichVuDefault = jsonOjbect.KetQua;
                            //dvObject.KetQuaDichVu = jsonOjbect.KetQua;
                            var ketLuan = jsonOjbect.KetLuan;
                            if (!string.IsNullOrEmpty(ketLuan))
                            {
                                ketLuan = CommonHelper.StripHTML(Regex.Replace(ketLuan, "</p>(?![\n\r]+)", Environment.NewLine));
                                if (ketLuan.Length > 2 && ketLuan.Substring(ketLuan.Length - 2) == "\r\n")
                                {
                                    ketLuan = ketLuan.Remove(ketLuan.Length - 2);
                                }
                            }
                            dvObject.KetQuaDichVuDefault = ketLuan;
                            dvObject.KetQuaDichVu = ketLuan;
                        }

                        listDichVu.Add(dvObject);
                    }
                }

                // xét nghiệm
                if (infoDichVuKyThuatXNs.Any())
                {

                    foreach (var itemDv in infoDichVuKyThuatXNs)
                    {
                        DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();


                        dvObject.Id = itemDv.Id;
                        dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                        dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                        dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                        dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                        dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                        dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                        if (itemDv.DataKetQuaCanLamSang != null && itemDv.TrangThai == 3) // !=1 => chưa thực hiện
                        {
                            // phiên xét nghiệm chi tiết orderby cuoi cung
                            if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets != null)
                            {
                                if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Any())
                                {
                                    if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Count == 1)
                                    {
                                        var itemGiaTriMin = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMin).First();
                                        var itemGiaTriMax = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMax).First();
                                        var itemGTDuyet = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriDuyet).First();
                                        var itemGiaTriNhapTay = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriNhapTay).First();
                                        var itemGiaTriTuMay = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriTuMay).First();
                                        var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                                                                                                                                                                                                                               //double ketQua = !string.IsNullOrEmpty(value)  ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;

                                        if (!string.IsNullOrEmpty(value) && IsInt(value))
                                        {
                                            double ketQua = !string.IsNullOrEmpty(value) ? Convert.ToDouble(value) : 0;
                                            double cSBTMin = 0;
                                            double cSBTMax = 0;
                                            if (itemGiaTriMin == null && itemGiaTriMax == null)
                                            {
                                                dvObject.KetQuaDichVu = ketQua.ToString() + "";
                                                dvObject.KetQuaDichVuDefault = ketQua.ToString() + "";
                                            }
                                            if (itemGiaTriMin != null && itemGiaTriMax != null)
                                            {
                                                var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                if (!string.IsNullOrEmpty(min))
                                                {
                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                }
                                                else
                                                {
                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                    if (!string.IsNullOrEmpty(max))
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                    }
                                                    else
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                    }
                                                }

                                                
                                            }

                                            if (itemGiaTriMin != null && itemGiaTriMax == null)
                                            {
                                                if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                {
                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                    if (!string.IsNullOrEmpty(min))
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                    }
                                                    else
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                    }
                                                }
                                                else
                                                {
                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                }
                                            }
                                            if (itemGiaTriMin == null && itemGiaTriMax != null)
                                            {
                                                if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                {
                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                    if (!string.IsNullOrEmpty(max))
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                    }
                                                    else
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                    }
                                                }
                                                else
                                                {
                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(value) && !IsInt(value))
                                        {
                                            string ketQua = !string.IsNullOrEmpty(value) ? value : " ";
                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                        }
                                    }
                                    else
                                    {
                                        int itemCongKyTu = 0;
                                        foreach (var itemKetQuaListCon in itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.OrderByDescending(d => d.CapDichVu == 1 ? 1 : 0).ThenBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList())
                                        {
                                            var dichVuXetNghiemId = itemKetQuaListCon.DichVuXetNghiemId;
                                            var mauMayXetNghiemId = itemKetQuaListCon.MauMayXetNghiemId;
                                            var tenketQua = "";
                                            // nếu  mẫu máy xét nghiệm khác null => lấy ten dich vụ xét nghiệm trong db.DichVuXetNghiemKetNoiChiSo => field : TenKetNoi
                                            // hoặc DichVuXetNghiemKetNoiChiSoId != null
                                            if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null)
                                            {
                                                tenketQua = dichVuXetNghiemKetNoiChiSos.Where(s => s.Id == itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId).Select(s => s.TenKetNoi).FirstOrDefault();
                                                if (tenketQua != null)
                                                {
                                                    dvObject.KetQuaDichVu += tenketQua + ": ";
                                                    dvObject.KetQuaDichVuDefault += tenketQua + ": ";
                                                }

                                                var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                double ketQua;
                                                bool KieuSo = false;
                                                if (value != null)
                                                {
                                                    KieuSo = IsInt(value) ? true : false;
                                                }
                                                else
                                                {
                                                    KieuSo = false;
                                                }
                                                if (KieuSo == true)
                                                {
                                                    double cSBTMin = 0;
                                                    double cSBTMax = 0;
                                                    ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                    if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                    {
                                                        var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                        if (!string.IsNullOrEmpty(min))
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                        }
                                                        else
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                            }
                                                        }

                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }

                                                    if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                    {

                                                        if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "(Tăng)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }

                                                }
                                                if (KieuSo == false)
                                                {
                                                    dvObject.KetQuaDichVu += value + "";
                                                    dvObject.KetQuaDichVuDefault += value + "";
                                                }

                                                if (itemCongKyTu < itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Count())
                                                {
                                                    dvObject.KetQuaDichVu += "; ";
                                                    dvObject.KetQuaDichVuDefault += "; ";
                                                }

                                            }
                                            // nếu mẫu máy xét nghiệm == null => tên dịch vụ xét nghiệm trong db.KetQuaXetNghiemChiTiet => field :DichVuXetNghiemTen 
                                            // hoặc DichVuXetNghiemKetNoiChiSoId == null -> dịch vụ cha
                                            if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId == null)
                                            {
                                                dvObject.KetQuaDichVu += itemKetQuaListCon.DichVuXetNghiemTen + ": ";
                                                dvObject.KetQuaDichVuDefault += itemKetQuaListCon.DichVuXetNghiemTen + ": ";

                                                var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                //var value = itemGTDuyet ?? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? null));
                                                var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                double ketQua;
                                                bool KieuSo = false;
                                                if (value != null)
                                                {
                                                    KieuSo = IsInt(value) ? true : false;
                                                }
                                                else
                                                {
                                                    KieuSo = false;
                                                }
                                                double cSBTMin = 0;
                                                double cSBTMax = 0;
                                                if (KieuSo == true)
                                                {
                                                    ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                    if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                    {
                                                        var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                        if (!string.IsNullOrEmpty(min))
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                        }
                                                        else
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                            }
                                                        }

                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }

                                                    if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                    {

                                                        if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "(Tăng)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                }
                                                if (KieuSo == false)
                                                {
                                                    dvObject.KetQuaDichVu += value + "";
                                                    dvObject.KetQuaDichVuDefault += value + "";
                                                }

                                                if (itemCongKyTu < itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Count())
                                                {
                                                    dvObject.KetQuaDichVu += "; ";
                                                    dvObject.KetQuaDichVuDefault += "; ";
                                                }
                                            }
                                            itemCongKyTu++;
                                        }
                                    }

                                    if (dvObject.KetQuaDichVu == "" || dvObject.KetQuaDichVu == "0")
                                    {
                                        dvObject.KetQuaDichVu = "";
                                        dvObject.KetQuaDichVuDefault = "";
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(dvObject.KetQuaDichVu))
                        {
                            dvObject.KetQuaDichVu = dvObject.KetQuaDichVu.Split(";").Where(d => d != "" && d != " ").ToList().Distinct().Join(";");
                        }
                        if (!string.IsNullOrEmpty(dvObject.KetQuaDichVuDefault))
                        {
                            dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVuDefault.Split(";").Where(d => d != "" && d != " ").ToList().Distinct().Join(";");
                        }
                        listDichVu.Add(dvObject);
                    }
                }

                // BVHD-3668 -> lấy những dịch vụ kỹ thuật khác cls -> kết quả để null
                if (infoDichVuKyThuatKhacXNKhacCDHAKhacPTTTs.Any())
                {
                    foreach (var itemDVKTKhacCLS in infoDichVuKyThuatKhacXNKhacCDHAKhacPTTTs)
                    {
                        DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                        dvObject.Id = itemDVKTKhacCLS.Id;
                        dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                        dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                        dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVuKyThuat;
                        dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                        dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                        dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                        dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThai;
                        dvObject.KetQuaDichVu = string.Empty; // để tự nhập
                        dvObject.KetQuaDichVuDefault = string.Empty; // để tự nhập
                        listDichVu.Add(dvObject);
                    }
                }

                // BVHD-3877 -- thủ thuật phẩu thuật
                if (infoDichVuKyThuatPTTTs.Any())
                {
                    foreach (var itemDVKTKhacCLS in infoDichVuKyThuatPTTTs)
                    {
                        DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                        dvObject.Id = itemDVKTKhacCLS.Id;
                        dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                        dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                        dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVuKyThuat;
                        dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                        dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                        dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                        dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThai;
                        dvObject.KetQuaDichVu = itemDVKTKhacCLS.KetQuaDichVu;
                        dvObject.KetQuaDichVuDefault = itemDVKTKhacCLS.KetQuaDichVu;
                        listDichVu.Add(dvObject);
                    }
                }


                // cập nhật BVHD-3880
                if (thongTinKham.KetQuaKhamSucKhoeData != null)
                {
                    // chạy những data cũ  chưa lưu người thực hiện , và thời điểm thực hiện trong josn
                    if (thongTinKham.LoaiLuuInKetQuaKSK == null)
                    {
                        listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(thongTinKham.KetQuaKhamSucKhoeData);
                        // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất

                        foreach (var itemxDvMoi in listDichVu)
                        {
                            foreach (var dvcu in listDichVuCu)
                            {
                                if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                {
                                    itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                }
                            }
                        }
                    }
                    else
                    {

                        var jonKetLuan = JsonConvert.DeserializeObject<KetQuaKhamSucKhoeDaTa>(thongTinKham.KetQuaKhamSucKhoeData);
                        if (!string.IsNullOrEmpty(jonKetLuan.KetQuaKhamSucKhoe))
                        {
                            listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(jonKetLuan.KetQuaKhamSucKhoe);
                            // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                            // data mới KetQuaDaDuocLuu = true, false
                            if (listDichVuCu.Where(d => d.KetQuaDaDuocLuu != null).ToList().Count() !=0 )
                            {
                                foreach (var itemxDvMoi in listDichVu)
                                {
                                    foreach (var dvcu in listDichVuCu)
                                    {
                                        if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                        {
                                            if (dvcu.KetQuaDaDuocLuu == true)
                                            {
                                                itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                            }
                                        }
                                    }
                                }
                            }
                            else // Trường hợp data cũ  KetQuaDaDuocLuu null
                            {
                                foreach (var itemxDvMoi in listDichVu)
                                {
                                    foreach (var dvcu in listDichVuCu)
                                    {
                                        if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                        {
                                            itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                        }
                                    }
                                }
                            }
                           
                        }
                    }
                }


            }
            // xử lý list string trùng nhau (dịch vụ khám)
            //List<DanhSachDichVuKhamGrid> listDichVuLoaiTrung = new List<DanhSachDichVuKhamGrid>();

            foreach (var itemTrung in listDichVu.Where(s => s.NhomId != EnumNhomGoiDichVu.DichVuKyThuat).ToList())
            {
                if (!string.IsNullOrEmpty(itemTrung.KetQuaDichVu) && !string.IsNullOrEmpty(itemTrung.KetQuaDichVuDefault))
                {
                    var catstring = itemTrung.KetQuaDichVu.Split('.');
                    var catstringdefault = itemTrung.KetQuaDichVuDefault.Split('.');
                    itemTrung.KetQuaDichVu = catstring.Where(d => d != null && d != "").Distinct().Join(".");
                    itemTrung.KetQuaDichVuDefault = catstringdefault.Where(d => d != null && d != "").Distinct().Join(".");

                }
                //listDichVuLoaiTrung.Add(itemTrung);
            }
            //// xử lý push những dịch vụ kỹ thuật 
            //foreach (var itemTrung in listDichVu.Where(s => s.NhomId == EnumNhomGoiDichVu.DichVuKyThuat).ToList())
            //{
            //    listDichVuLoaiTrung.Add(itemTrung);
            //}



            return listDichVu.OrderBy(o => o.TenDichVu).ToList(); // trả về 1 list dịch vụ (dịch vụ khám , cls (dịch vụ kỹ thuật))
        }
        private bool IsInt(string sVal)
        {
            double test;
            return double.TryParse(sVal, out test);
        }

        public List<DanhSachPhanLoaiCacBenhTatGrid> GetGridPhanLoaiVaCacBenhtatDenghiByHopDong(long hopDongKhamSucKhoeId)
        {
            var thongTinNhanVienKhams =
               BaseRepository.TableNoTracking
                   .Include(x => x.HopDongKhamSucKhoeNhanVien)
                   .OrderByDescending(x => x.Id)
                   .Where(x => x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId).Select(o => o).ToList();
            List<DanhSachPhanLoaiCacBenhTatGrid> resultobjs = new List<DanhSachPhanLoaiCacBenhTatGrid>();
            if (thongTinNhanVienKhams != null && thongTinNhanVienKhams.Any())
            {
                foreach (var thongTinNhanVienKham in thongTinNhanVienKhams)
                {

                    List<DanhSachPhanLoaiCacBenhTatGrid> objs = new List<DanhSachPhanLoaiCacBenhTatGrid>();
                    #region phân loại
                    DanhSachPhanLoaiCacBenhTatGrid objPL = new DanhSachPhanLoaiCacBenhTatGrid();
                    objPL.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                    objPL.Ten = EnumTypeLoaiKetLuan.PhanLoai.GetDescription();
                    objPL.LoaiKetLuan = EnumTypeLoaiKetLuan.PhanLoai;
                    var listPhanLoaiSucKhoe = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
                    var result = listPhanLoaiSucKhoe.Select(item => new LookupItemVo
                    {
                        DisplayName = item.GetDescription(),
                        KeyId = Convert.ToInt32(item),
                    });
                    if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe))
                    {
                        result = result.Where(o => o.DisplayName.ToLower().Trim() == thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe.ToLower().Trim());
                        if (result.Any())
                        {
                            objPL.PhanLoaiId = result.Select(d => d.KeyId).First();
                        }
                    }
                    objPL.ShowComBoBox = true;
                    objPL.KetQua = thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe;
                    objPL.KetQuaDefault = thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe;
                    if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanCacBenhTat))
                    {
                        objPL.DaCoketLuan = true;
                    }
                    objs.Add(objPL);
                    #endregion
                    #region các bệnh tật khác
                    DanhSachPhanLoaiCacBenhTatGrid objCacBenhTatKhac = new DanhSachPhanLoaiCacBenhTatGrid();
                    objCacBenhTatKhac.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                    objCacBenhTatKhac.Ten = EnumTypeLoaiKetLuan.CacBenhTatNeuCo.GetDescription();
                    objCacBenhTatKhac.LoaiKetLuan = EnumTypeLoaiKetLuan.CacBenhTatNeuCo;
                    objCacBenhTatKhac.PhanLoaiId = 0;
                    objCacBenhTatKhac.KetQua = thongTinNhanVienKham.KSKKetLuanCacBenhTat;
                    objCacBenhTatKhac.KetQuaDefault = thongTinNhanVienKham.KSKKetLuanCacBenhTat;
                    if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanCacBenhTat))
                    {
                        objCacBenhTatKhac.DaCoketLuan = true;
                    }
                    objs.Add(objCacBenhTatKhac);
                    #endregion
                    #region dê nghi
                    DanhSachPhanLoaiCacBenhTatGrid objDeNghi = new DanhSachPhanLoaiCacBenhTatGrid();
                    objDeNghi.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                    objDeNghi.Ten = EnumTypeLoaiKetLuan.DeNghi.GetDescription();
                    objDeNghi.LoaiKetLuan = EnumTypeLoaiKetLuan.DeNghi;
                    objDeNghi.PhanLoaiId = 0;
                    objDeNghi.KetQua = thongTinNhanVienKham.KSKKetLuanGhiChu;
                    objDeNghi.KetQuaDefault = thongTinNhanVienKham.KSKKetLuanGhiChu;
                    if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanGhiChu))
                    {
                        objDeNghi.DaCoketLuan = true;
                    }
                    objs.Add(objDeNghi);
                    #endregion
                    if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanData))
                    {
                        if (thongTinNhanVienKham.KSKKetLuanData != "[]")
                        {
                            var jsonParse = JsonConvert.DeserializeObject<List<DanhSachPhanLoaiCacBenhTatGrid>>(thongTinNhanVienKham.KSKKetLuanData);
                            if (jsonParse.Any())
                            {
                                if (objs.Any())
                                {
                                    foreach (var item in objs)
                                    {
                                        var itemSaveJson = jsonParse.FirstOrDefault(o => o.LoaiKetLuan == item.LoaiKetLuan);
                                        if (itemSaveJson != null)
                                        {
                                            if (item.LoaiKetLuan == EnumTypeLoaiKetLuan.PhanLoai)
                                            {
                                                if (itemSaveJson.PhanLoaiIdCapNhat != 0)
                                                {
                                                    var listPhanLoaiSucKhoejson = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
                                                    var resultjson = listPhanLoaiSucKhoe.Select(itemjson => new LookupItemVo
                                                    {
                                                        DisplayName = itemjson.GetDescription(),
                                                        KeyId = Convert.ToInt32(itemjson),
                                                    });
                                                    resultjson = resultjson.Where(o => o.KeyId == itemSaveJson.PhanLoaiIdCapNhat);
                                                    if (resultjson.Any())
                                                    {
                                                        item.KetQua = resultjson.Select(d => d.DisplayName).FirstOrDefault();
                                                        item.PhanLoaiIdCapNhat = itemSaveJson.PhanLoaiIdCapNhat;
                                                    }
                                                }
                                                else
                                                {
                                                    item.KetQua = itemSaveJson.KetQua;
                                                    item.PhanLoaiIdCapNhat = itemSaveJson.PhanLoaiIdCapNhat;
                                                }

                                            }
                                            if (item.LoaiKetLuan == EnumTypeLoaiKetLuan.DeNghi)
                                            {
                                                item.KetQua = itemSaveJson.KetQua;
                                            }
                                            if (item.LoaiKetLuan == EnumTypeLoaiKetLuan.CacBenhTatNeuCo)
                                            {
                                                item.KetQua = itemSaveJson.KetQua;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                    resultobjs.AddRange(objs);
                }
            }
            return resultobjs;
        }
        public async Task<List<DanhSachPhanLoaiCacBenhTatGrid>> GetGridPhanLoaiVaCacBenhtatDenghi(long hopDongKhamSucKhoeId)
        {
            var thongTinNhanVienKham =
               await BaseRepository.TableNoTracking
                   .Include(x => x.HopDongKhamSucKhoeNhanVien)
                   .OrderByDescending(x => x.Id)
                   .FirstOrDefaultAsync(x => x.HopDongKhamSucKhoeNhanVienId == hopDongKhamSucKhoeId);
            List<DanhSachPhanLoaiCacBenhTatGrid> objs = new List<DanhSachPhanLoaiCacBenhTatGrid>();

            #region phân loại
            DanhSachPhanLoaiCacBenhTatGrid objPL = new DanhSachPhanLoaiCacBenhTatGrid();

            objPL.Ten = EnumTypeLoaiKetLuan.PhanLoai.GetDescription();
            objPL.LoaiKetLuan = EnumTypeLoaiKetLuan.PhanLoai;
            var listPhanLoaiSucKhoe = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
            var result = listPhanLoaiSucKhoe.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            });
            if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe))
            {
                result = result.Where(o => o.DisplayName.ToLower().Trim() == thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe.ToLower().Trim());
                if (result.Any())
                {
                    objPL.PhanLoaiId = result.Select(d => d.KeyId).First();
                }
            }
            objPL.ShowComBoBox = true;
            objPL.KetQua = thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe;
            objPL.KetQuaDefault = thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe;
            if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanCacBenhTat))
            {
                objPL.DaCoketLuan = true;
            }
            objs.Add(objPL);
            #endregion
            #region các bệnh tật khác
            DanhSachPhanLoaiCacBenhTatGrid objCacBenhTatKhac = new DanhSachPhanLoaiCacBenhTatGrid();
            objCacBenhTatKhac.Ten = EnumTypeLoaiKetLuan.CacBenhTatNeuCo.GetDescription();
            objCacBenhTatKhac.LoaiKetLuan = EnumTypeLoaiKetLuan.CacBenhTatNeuCo;
            objCacBenhTatKhac.PhanLoaiId = 0;
            objCacBenhTatKhac.KetQua = thongTinNhanVienKham.KSKKetLuanCacBenhTat;
            objCacBenhTatKhac.KetQuaDefault = thongTinNhanVienKham.KSKKetLuanCacBenhTat;
            if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanCacBenhTat))
            {
                objCacBenhTatKhac.DaCoketLuan = true;
            }
            objs.Add(objCacBenhTatKhac);
            #endregion
            #region dê nghi
            DanhSachPhanLoaiCacBenhTatGrid objDeNghi = new DanhSachPhanLoaiCacBenhTatGrid();
            objDeNghi.Ten = EnumTypeLoaiKetLuan.DeNghi.GetDescription();
            objDeNghi.LoaiKetLuan = EnumTypeLoaiKetLuan.DeNghi;
            objDeNghi.PhanLoaiId = 0;
            objDeNghi.KetQua = thongTinNhanVienKham.KSKKetLuanGhiChu;
            objDeNghi.KetQuaDefault = thongTinNhanVienKham.KSKKetLuanGhiChu;
            if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanGhiChu))
            {
                objDeNghi.DaCoketLuan = true;
            }
            objs.Add(objDeNghi);
            #endregion
            if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanData))
            {
                if (thongTinNhanVienKham.KSKKetLuanData != "[]")
                {
                    var jsonParse = JsonConvert.DeserializeObject<List<DanhSachPhanLoaiCacBenhTatGrid>>(thongTinNhanVienKham.KSKKetLuanData);
                    if (jsonParse.Any())
                    {
                        if (objs.Any())
                        {
                            foreach (var item in objs)
                            {
                                var itemSaveJson = jsonParse.FirstOrDefault(o => o.LoaiKetLuan == item.LoaiKetLuan);
                                if (itemSaveJson != null)
                                {
                                    if (item.LoaiKetLuan == EnumTypeLoaiKetLuan.PhanLoai)
                                    {
                                        if (itemSaveJson.PhanLoaiIdCapNhat != 0)
                                        {
                                            var listPhanLoaiSucKhoejson = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
                                            var resultjson = listPhanLoaiSucKhoe.Select(itemjson => new LookupItemVo
                                            {
                                                DisplayName = itemjson.GetDescription(),
                                                KeyId = Convert.ToInt32(itemjson),
                                            });
                                            resultjson = resultjson.Where(o => o.KeyId == itemSaveJson.PhanLoaiIdCapNhat);
                                            if (resultjson.Any())
                                            {
                                                item.KetQua = resultjson.Select(d => d.DisplayName).FirstOrDefault();
                                                item.PhanLoaiIdCapNhat = itemSaveJson.PhanLoaiIdCapNhat;
                                            }
                                        }
                                        else
                                        {
                                            item.KetQua = itemSaveJson.KetQua;
                                            item.PhanLoaiIdCapNhat = itemSaveJson.PhanLoaiIdCapNhat;
                                        }

                                    }
                                    if (item.LoaiKetLuan == EnumTypeLoaiKetLuan.DeNghi)
                                    {
                                        item.KetQua = itemSaveJson.KetQua;
                                    }
                                    if (item.LoaiKetLuan == EnumTypeLoaiKetLuan.CacBenhTatNeuCo)
                                    {
                                        item.KetQua = itemSaveJson.KetQua;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return objs;
        }
        public List<LookupItemVo> GetPhanLoaiSucKhoeKetLuan(DropDownListRequestModel model, long? phanLoaiId)
        {
            if (phanLoaiId == null)
            {
                var listPhanLoaiSucKhoe = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
                var result = listPhanLoaiSucKhoe.Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item),
                });
                if (!string.IsNullOrEmpty(model.Query))
                {
                    result = result.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
                }
                return result.ToList();
            }
            else
            {
                var listPhanLoaiSucKhoe = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
                var result = listPhanLoaiSucKhoe.Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item),
                });
                result = result.Where(p => p.KeyId == phanLoaiId).ToList();
                return result.ToList();
            }
        }
        #endregion

        public async Task<string> GetTenCongTy(long congTyKhamSucKhoeId)
        {
            return await _congTyKhamSucKhoeRepository.TableNoTracking.Where(z => z.Id == congTyKhamSucKhoeId).Select(z => z.Ten).FirstOrDefaultAsync();
        }
        public async Task<string> GetTenHopDongKhamSucKhoe(long hopDongKhamSucKhoeId)
        {
            return await _hopDongKhamSucKhoeRepository.TableNoTracking.Where(z => z.Id == hopDongKhamSucKhoeId).Select(z => z.SoHopDong).FirstOrDefaultAsync();
        }

        #region chức năng copy dịch vụ xét nghiệm có data của tất cả yêu cầu tiếp nhận thuộc hợp đồng khám 
        public void UpdateAllKetQuaKSKDoanCuaHopDongNhanVienBatDauKham(long hopDongKhamSucKhoeId)
        {
            //// test
            //var hopDongId = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(d => d.Id == hopDongKhamSucKhoeId).Select(d => d.HopDongKhamSucKhoeId).First();
            ////test


            var getThongTinNhanVienDaBatDauKhamTheoHopDongs = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p =>p.KetQuaKhamSucKhoeData!=null && (p.HopDongKhamSucKhoeNhanVien != null && p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId
                                                                                          && p.BenhNhanId != null))
                .Select(d => d.Id).ToList();
            
            
            
            var thongTinNhanVienKham =
             BaseRepository.TableNoTracking
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.KetQuaXetNghiemChiTiets)
                 .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .Include(x => x.KetQuaSinhHieus)
                .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.BenhNhan)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HopDongKhamSucKhoe).ThenInclude(z => z.CongTyKhamSucKhoe)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                .OrderByDescending(x => x.Id)
                .Where(x => getThongTinNhanVienDaBatDauKhamTheoHopDongs.Contains(x.Id)).ToList();

            var dichVuXetNghiemKetNoiChiSos = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Select(s => new
            {
                Id = s.Id,
                DichVuXetNghiemId = s.DichVuXetNghiemId,
                MauMayXetNghiemId = s.MauMayXetNghiemId,
                TenKetNoi = s.TenKetNoi
            }).ToList();
            var kSKUpdateAllNhanVienTheoHopDongs = new List<KSKUpdateAllNhanVienTheoHopDong>();
            var ds = thongTinNhanVienKham.ToList();
            var i = 1;
            foreach (var itemNhanVien in ds)
            {
                var data = new KetQuaKhamSucKhoeVo();
                var tableKham = "";
                var tableKyThuat = "";

                var nhanVienKetLuanId = _userAgentHelper.GetCurrentUserId();
                var thoiDiemKetLuan = DateTime.Now;
                List<DanhSachDichVuKhamGrid> listDichVu = new List<DanhSachDichVuKhamGrid>();
                List<DanhSachDichVuKhamGrid> listDichVuCu = new List<DanhSachDichVuKhamGrid>();
                List<long> listDichVuKyThuatTheoGoi = new List<long>();
                if (itemNhanVien != null)
                {

                    // DV Kham
                    if (itemNhanVien.YeuCauKhamBenhs.Where(s => s.GoiKhamSucKhoeId != null && s.ThongTinKhamTheoDichVuData != null && s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Any())
                    {
                        foreach (var itemDv in itemNhanVien.YeuCauKhamBenhs.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                                                                   .Select(v => new DanhSachDichVuKhamGrid
                                                                                   {
                                                                                       Id = v.Id,
                                                                                       TenDichVu = v.DichVuKhamBenhBenhVien?.Ten,
                                                                                       ThongTinKhamTheoDichVuTemplate = v.ThongTinKhamTheoDichVuTemplate,
                                                                                       ThongTinKhamTheoDichVuData = v.ThongTinKhamTheoDichVuData,
                                                                                       TrangThaiDVKham = (int)v.TrangThai
                                                                                   }).ToList())
                        {
                            DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                            dvObject.Id = itemDv.Id;
                            dvObject.NhomId = EnumNhomGoiDichVu.DichVuKhamBenh;
                            dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                            dvObject.TenDichVu = itemDv.TenDichVu;
                            dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                            dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;
                            //dvObject.TrangThaiThucHienDichVu = itemDv.TrangThaiThucHienDichVu == 5 ? 3 :0;// 3 đã khám
                            if (itemDv.ThongTinKhamTheoDichVuTemplate != null && itemDv.ThongTinKhamTheoDichVuData != null  && itemDv.TrangThaiDVKham != 6) 
                            {
                                var jsonOjbectTemplate = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(itemDv.ThongTinKhamTheoDichVuTemplate);
                                var jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);

                                //cập nhật BVHD-3880 cập nhật trạng thái dịch vụ
                                // bệnh nhân chưa bắt đầu khám
                                if (jsonOjbectData.DataKhamTheoTemplate.Count() == 0)
                                {
                                    if (dvObject.TenDichVu == "Khám Ngoại")
                                    {
                                        itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa);
                                    }
                                    if (dvObject.TenDichVu == "Khám Mắt")
                                    {
                                        itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.Mat);
                                    }

                                    if (dvObject.TenDichVu == "Khám Răng Hàm Mặt")
                                    {
                                        itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.RangHamMat);
                                    }

                                    if (dvObject.TenDichVu == "Khám Tai Mũi Họng")
                                    {
                                        itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                    }
                                    if (dvObject.TenDichVu == "Khám Da liễu")
                                    {
                                        itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                    }
                                    if (dvObject.TenDichVu == "Nội khoa")
                                    {
                                        itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa);
                                    }
                                    if (dvObject.TenDichVu == "Sản phụ khoa")
                                    {
                                        itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa);
                                    }

                                    jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);
                                }



                                foreach (var itemx in jsonOjbectTemplate.ComponentDynamics)
                                {
                                    var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemx.Id);
                                    if (kiemTra.Any())
                                    {
                                        switch (itemx.Id)
                                        {
                                            case "TuanHoan":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                break;
                                            case "HoHap":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                break;

                                            case "TieuHoa":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                break;

                                            case "ThanTietLieu":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                break;

                                            case "NoiTiet":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                break;

                                            case "CoXuongKhop":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                break;

                                            case "ThanKinh":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                break;

                                            case "TamThan":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                break;

                                            case "NgoaiKhoa":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenNgoaiKhoa;
                                                break;
                                            case "SanPhuKhoa":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenSanPhuKhoa;
                                                break;


                                            case "CacBenhVeMat":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaMat;
                                                break;
                                            case "CacBenhTaiMuiHong":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaTaiMuiHong;
                                                break;

                                            case "CacBenhRangHamMat":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                break;
                                            case "HamTren":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                break;
                                            case "HamDuoi":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                break;

                                            case "DaLieu":
                                                if (kiemTra.FirstOrDefault().Value != null)
                                                {
                                                    if (dvObject.TrangThaiDVKham == 5)
                                                    {
                                                        dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                    }
                                                }
                                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenDaLieu;
                                                break;
                                            default:
                                                //do a different thing
                                                break;
                                        }
                                    }


                                }
                            }
                            dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVu;
                            listDichVu.Add(dvObject);
                        }

                    }

                    // list theo yêu cầu tiếp nhận
                    // CDHA TDCN
                    if (itemNhanVien.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)).Any())
                    {

                        foreach (var itemDv in itemNhanVien.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                            .Select(z => new
                            {
                                DataKetQuaCanLamSang = z.DataKetQuaCanLamSang,
                                TenDichVuKyThuat = z.TenDichVu,
                                Id = z.Id,
                                GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                                TrangThaiDVKham = (int)z.TrangThai
                            }).ToList())
                        {
                            DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                            dvObject.Id = itemDv.Id;
                            dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                            dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                            dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                            dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                            dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                            dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;

                            if (itemDv.DataKetQuaCanLamSang != null && itemDv.TrangThaiDVKham == 3) // != 1 => dịch vụ chưa thực hiện
                            {
                                var jsonOjbect = JsonConvert.DeserializeObject<DataCLS>(itemDv.DataKetQuaCanLamSang);

                                //dvObject.KetQuaDichVuDefault = jsonOjbect.KetQua;
                                //dvObject.KetQuaDichVu = jsonOjbect.KetQua;
                                var ketLuan = jsonOjbect.KetLuan;
                                if (!string.IsNullOrEmpty(ketLuan))
                                {
                                    ketLuan = CommonHelper.StripHTML(Regex.Replace(ketLuan, "</p>(?![\n\r]+)", Environment.NewLine));
                                    if (ketLuan.Length > 2 && ketLuan.Substring(ketLuan.Length - 2) == "\r\n")
                                    {
                                        ketLuan = ketLuan.Remove(ketLuan.Length - 2);
                                    }
                                }
                                dvObject.KetQuaDichVuDefault = ketLuan;
                                dvObject.KetQuaDichVu = ketLuan;
                            }

                            listDichVu.Add(dvObject);
                        }
                    }

                    // xét nghiệm
                    if (itemNhanVien.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).Any())
                    {

                        foreach (var itemDv in itemNhanVien.YeuCauDichVuKyThuats
                            .Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                            .Select(z => new
                            {
                                DataKetQuaCanLamSang = z.PhienXetNghiemChiTiets
                                                        .Select(v => new
                                                        {
                                                            KetQuaXetNghiemChiTiet = v.KetQuaXetNghiemChiTiets.ToList(),
                                                            LanThucHien = v.LanThucHien,
                                                            KetLuan = v.KetLuan
                                                        }).OrderBy(s => s.LanThucHien)
                                                        .LastOrDefault(),
                                TenDichVuKyThuat = z.TenDichVu,
                                Id = z.Id,
                                GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                                TrangThai = (int)z.TrangThai
                            }).ToList())
                        {
                            DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();


                            dvObject.Id = itemDv.Id;
                            dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                            dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                            dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                            dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                            dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;

                            if (itemDv.DataKetQuaCanLamSang != null && itemDv.TrangThai == 3) // !=1 => chưa thực hiện
                            {
                                // phiên xét nghiệm chi tiết orderby cuoi cung
                                if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet != null)
                                {
                                    if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Any())
                                    {
                                        if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Count == 1)
                                        {
                                            var itemGiaTriMin = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriMin).First();
                                            var itemGiaTriMax = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriMax).First();
                                            var itemGTDuyet = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriDuyet).First();
                                            var itemGiaTriNhapTay = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriNhapTay).First();
                                            var itemGiaTriTuMay = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriTuMay).First();
                                            var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                                                                                                                                                                                                                                   //double ketQua = !string.IsNullOrEmpty(value)  ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;

                                            if (!string.IsNullOrEmpty(value) && IsInt(value))
                                            {
                                                double ketQua = !string.IsNullOrEmpty(value) ? Convert.ToDouble(value) : 0;
                                                double cSBTMin = 0;
                                                double cSBTMax = 0;
                                                if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                {
                                                    dvObject.KetQuaDichVu = ketQua.ToString() + "";
                                                    dvObject.KetQuaDichVuDefault = ketQua.ToString() + "";
                                                }
                                                if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                {
                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                    if (!string.IsNullOrEmpty(min))
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                    }
                                                    else
                                                    {
                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                        if (!string.IsNullOrEmpty(max))
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                        }
                                                    }


                                                }

                                                if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                {
                                                    if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                    {
                                                        var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                        if (!string.IsNullOrEmpty(min))
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                    }
                                                }
                                                if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                {
                                                    if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                    {
                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                        if (!string.IsNullOrEmpty(max))
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(value) && !IsInt(value))
                                            {
                                                string ketQua = !string.IsNullOrEmpty(value) ? value : " ";
                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                            }
                                        }
                                        else
                                        {
                                            int itemCongKyTu = 0;
                                            foreach (var itemKetQuaListCon in itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.OrderByDescending(d => d.CapDichVu == 1 ? 1 : 0).ThenBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList())
                                            {
                                                var dichVuXetNghiemId = itemKetQuaListCon.DichVuXetNghiemId;
                                                var mauMayXetNghiemId = itemKetQuaListCon.MauMayXetNghiemId;
                                                var tenketQua = "";
                                                // nếu  mẫu máy xét nghiệm khác null => lấy ten dich vụ xét nghiệm trong db.DichVuXetNghiemKetNoiChiSo => field : TenKetNoi
                                                // hoặc DichVuXetNghiemKetNoiChiSoId != null
                                                if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null)
                                                {
                                                    tenketQua = dichVuXetNghiemKetNoiChiSos.Where(s => s.Id == itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId).Select(s => s.TenKetNoi).FirstOrDefault();
                                                    if (tenketQua != null)
                                                    {
                                                        dvObject.KetQuaDichVu += tenketQua + ": ";
                                                        dvObject.KetQuaDichVuDefault += tenketQua + ": ";
                                                    }

                                                    var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                    var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                    var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                    var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                    var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                    var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                    double ketQua;
                                                    bool KieuSo = false;
                                                    if (value != null)
                                                    {
                                                        KieuSo = IsInt(value) ? true : false;
                                                    }
                                                    else
                                                    {
                                                        KieuSo = false;
                                                    }
                                                    if (KieuSo == true)
                                                    {
                                                        double cSBTMin = 0;
                                                        double cSBTMax = 0;
                                                        ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                        if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                        if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                            }
                                                            else
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                }
                                                            }

                                                        }
                                                        if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                        {
                                                            if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                            {
                                                                var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                if (!string.IsNullOrEmpty(min))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }

                                                        if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                        {

                                                            if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "(Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }

                                                    }
                                                    if (KieuSo == false)
                                                    {
                                                        dvObject.KetQuaDichVu += value + "";
                                                        dvObject.KetQuaDichVuDefault += value + "";
                                                    }

                                                    if (itemCongKyTu < itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Count())
                                                    {
                                                        dvObject.KetQuaDichVu += "; ";
                                                        dvObject.KetQuaDichVuDefault += "; ";
                                                    }

                                                }
                                                // nếu mẫu máy xét nghiệm == null => tên dịch vụ xét nghiệm trong db.KetQuaXetNghiemChiTiet => field :DichVuXetNghiemTen 
                                                // hoặc DichVuXetNghiemKetNoiChiSoId == null -> dịch vụ cha
                                                if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId == null)
                                                {
                                                    dvObject.KetQuaDichVu += itemKetQuaListCon.DichVuXetNghiemTen + ": ";
                                                    dvObject.KetQuaDichVuDefault += itemKetQuaListCon.DichVuXetNghiemTen + ": ";

                                                    var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                    var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                    var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                    var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                    var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                    //var value = itemGTDuyet ?? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? null));
                                                    var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                    double ketQua;
                                                    bool KieuSo = false;
                                                    if (value != null)
                                                    {
                                                        KieuSo = IsInt(value) ? true : false;
                                                    }
                                                    else
                                                    {
                                                        KieuSo = false;
                                                    }
                                                    double cSBTMin = 0;
                                                    double cSBTMax = 0;
                                                    if (KieuSo == true)
                                                    {
                                                        ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                        if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                        if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                            }
                                                            else
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                }
                                                            }

                                                        }
                                                        if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                        {
                                                            if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                            {
                                                                var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                if (!string.IsNullOrEmpty(min))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }

                                                        if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                        {

                                                            if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "(Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                    }
                                                    if (KieuSo == false)
                                                    {
                                                        dvObject.KetQuaDichVu += value + "";
                                                        dvObject.KetQuaDichVuDefault += value + "";
                                                    }

                                                    if (itemCongKyTu < itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Count())
                                                    {
                                                        dvObject.KetQuaDichVu += "; ";
                                                        dvObject.KetQuaDichVuDefault += "; ";
                                                    }
                                                }
                                                itemCongKyTu++;
                                            }
                                        }

                                        if (dvObject.KetQuaDichVu == "" || dvObject.KetQuaDichVu == "0")
                                        {
                                            dvObject.KetQuaDichVu = "";
                                            dvObject.KetQuaDichVuDefault = "";
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(dvObject.KetQuaDichVu))
                            {
                                dvObject.KetQuaDichVu = dvObject.KetQuaDichVu.Split(";").Where(d => d != "" && d != " ").ToList().Distinct().Join(";");
                            }
                            if (!string.IsNullOrEmpty(dvObject.KetQuaDichVuDefault))
                            {
                                dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVuDefault.Split(";").Where(d => d != "" && d != " ").ToList().Distinct().Join(";");
                            }
                            if (!string.IsNullOrEmpty(dvObject.KetQuaDichVuDefault))
                            {
                                dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVuDefault.Trim();
                            }
                            listDichVu.Add(dvObject);
                        }
                    }

                    // BVHD-3668 -> lấy những dịch vụ kỹ thuật khác cls -> kết quả để null
                    if (itemNhanVien.YeuCauDichVuKyThuats.Any(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat))
                    {
                        foreach (var itemDVKTKhacCLS in itemNhanVien.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang).ToList())
                        {
                            DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                            dvObject.Id = itemDVKTKhacCLS.Id;
                            dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                            dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                            dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVu;
                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                            dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                            dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                            dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThai;
                            dvObject.KetQuaDichVu = string.Empty; // để tự nhập
                            dvObject.KetQuaDichVuDefault = string.Empty; // để tự nhập
                            listDichVu.Add(dvObject);
                        }
                    }

                    // BVHD-3877 -> THủ thuật phẩu thuật 
                    if (itemNhanVien.YeuCauDichVuKyThuats.Any(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ))
                    {
                        foreach (var itemDVKTKhacCLS in itemNhanVien.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat).ToList())
                        {
                            DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                            dvObject.Id = itemDVKTKhacCLS.Id;
                            dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                            dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                            dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVu;
                            dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                            dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                            dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                            dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThai;
                            dvObject.KetQuaDichVu = itemDVKTKhacCLS.YeuCauDichVuKyThuatTuongTrinhPTTT != null ? itemDVKTKhacCLS.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuCaPTTT :""; 
                            dvObject.KetQuaDichVuDefault = itemDVKTKhacCLS.YeuCauDichVuKyThuatTuongTrinhPTTT != null ? itemDVKTKhacCLS.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuCaPTTT : "";
                            listDichVu.Add(dvObject);
                        }
                    }


                    if (itemNhanVien.KetQuaKhamSucKhoeData != null)
                    {
                        // chạy những data cũ  chưa lưu người thực hiện , và thời điểm thực hiện trong josn
                        if (itemNhanVien.LoaiLuuInKetQuaKSK == null)
                        {
                            listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(itemNhanVien.KetQuaKhamSucKhoeData);
                            // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                            foreach (var itemxDvMoi in listDichVu)
                            {
                                foreach (var dvcu in listDichVuCu)
                                {
                                    if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                    {
                                        itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var jonKetLuan = JsonConvert.DeserializeObject<KetQuaKhamSucKhoeDaTa>(itemNhanVien.KetQuaKhamSucKhoeData);
                            if (!string.IsNullOrEmpty(jonKetLuan.KetQuaKhamSucKhoe))
                            {
                                nhanVienKetLuanId = jonKetLuan.NhanVienKetLuanId;
                                thoiDiemKetLuan = jonKetLuan.ThoiDiemKetLuan;
                                listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(jonKetLuan.KetQuaKhamSucKhoe);
                                // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                                // data mới KetQuaDaDuocLuu = true, false
                                
                                if (listDichVuCu.Where(d => d.KetQuaDaDuocLuu != null).ToList().Count() != 0)
                                {
                                    foreach (var itemxDvMoi in listDichVu)
                                    {
                                        foreach (var dvcu in listDichVuCu)
                                        {
                                            if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                            {
                                                if (dvcu.KetQuaDaDuocLuu == true)
                                                {
                                                    itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                                }
                                            }
                                        }
                                    }
                                }
                                else // Trường hợp data cũ  KetQuaDaDuocLuu null
                                {
                                    foreach (var itemxDvMoi in listDichVu)
                                    {
                                        foreach (var dvcu in listDichVuCu)
                                        {
                                            if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                            {
                                                itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }


                }

                foreach (var itemTrung in listDichVu.Where(s => s.NhomId != EnumNhomGoiDichVu.DichVuKyThuat).ToList())
                {
                    if (!string.IsNullOrEmpty(itemTrung.KetQuaDichVu) && !string.IsNullOrEmpty(itemTrung.KetQuaDichVuDefault))
                    {
                        var catstring = itemTrung.KetQuaDichVu.Split('.');
                        var catstringdefault = itemTrung.KetQuaDichVuDefault.Split('.');
                        itemTrung.KetQuaDichVu = catstring.Where(d => d != null && d != "").Distinct().Join(".");
                        itemTrung.KetQuaDichVuDefault = catstringdefault.Where(d => d != null && d != "").Distinct().Join(".");

                    }
                }
                var canXyLy = false;
                // xử lý kiểm tra kết quả dịch vụ null , kết quả dịch vụ xét nghiệm defaul có thì copy sang  (dùng cho cả 2 loại dịch vụ)
                foreach (var itemCopy in listDichVu.Where(o=> !string.IsNullOrEmpty(o.KetQuaDichVuDefault) && string.IsNullOrEmpty(o.KetQuaDichVu)).OrderBy(o => o.TenDichVu).ToList())
                {
                    itemCopy.KetQuaDichVu = itemCopy.KetQuaDichVuDefault;
                    canXyLy = true;
                }
                if (canXyLy)
                {
                    // cập nhật  json field KetQuaKhamSucKhoeData im yêu cầu tiếp nhận

                    var JsonKetQuaKSK = JsonConvert.SerializeObject(listDichVu.OrderBy(o => o.TenDichVu).ToList());
                    #region cập nhật xem người khám   kết luận, Thời điểm kết luận
                    var objJsonKetLuan = new KetQuaKhamSucKhoeDaTa
                    {
                        NhanVienKetLuanId = nhanVienKetLuanId,
                        ThoiDiemKetLuan = thoiDiemKetLuan,
                        KetQuaKhamSucKhoe = JsonKetQuaKSK
                    };

                    var json = JsonConvert.SerializeObject(objJsonKetLuan);

                    #region Phân loại
                    var ketLuanPhanLoai = GetGridPhanLoaiVaCacBenhtatDenghi((long)itemNhanVien.HopDongKhamSucKhoeNhanVienId); // hopDongKhamSucKhoeId hop đồn nhân viên id
                    var ketLuanData = JsonConvert.SerializeObject(ketLuanPhanLoai.Result);
                    #endregion Phân loại

                    var kSKUpdateAllNhanVienTheoHopDong = new KSKUpdateAllNhanVienTheoHopDong()
                    {
                        KetQuaKhamSucKhoeData = json,
                        ketLuanData = ketLuanData,
                        YeuCauTiepNhanId = itemNhanVien.Id
                    };
                    kSKUpdateAllNhanVienTheoHopDongs.Add(kSKUpdateAllNhanVienTheoHopDong);
                }
                #endregion
                i++;
            }
            if (kSKUpdateAllNhanVienTheoHopDongs.Any())
            {
                var yeuCauTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(p => getThongTinNhanVienDaBatDauKhamTheoHopDongs.Contains(p.Id))
                    .Select(d => d);
                foreach (var itemNhanVienCapNhat in kSKUpdateAllNhanVienTheoHopDongs)
                {
                    var yctn = yeuCauTiepNhans.FirstOrDefault(o=>o.Id==itemNhanVienCapNhat.YeuCauTiepNhanId);
                    if (yctn != null)
                    {
                        yctn.KetQuaKhamSucKhoeData = itemNhanVienCapNhat.KetQuaKhamSucKhoeData;
                        yctn.KSKKetLuanData = itemNhanVienCapNhat.ketLuanData;
                        yctn.LoaiLuuInKetQuaKSK = true;
                        BaseRepository.Update(yctn);
                    }
                   
                    //SaveBanInKhamDoanTiepNhan(itemNhanVienCapNhat.YeuCauTiepNhanId, itemNhanVienCapNhat.KetQuaKhamSucKhoeData, itemNhanVienCapNhat.ketLuanData);
                }
            }
        }
        #endregion

        #region viết lại api báo cáo tổng hợp kết quả KSK 22122021
        public List<DanhSachDichVuKhamGrid> GetDataKetQuaKSKDoanEditByHopDongNew(long hopDongKhamSucKhoeId, List<long> tiepNhanIds)
        {
            var thongTinInfos = new List<ThongTinInfo>();


            var entity = BaseRepository.TableNoTracking.Where(x => tiepNhanIds.Contains(x.Id))
                .Select(d => new InfoDichVu
                {
                    YeuCauKhamBenhIds = d.YeuCauKhamBenhs.Select(f=>f.Id).ToList(),
                    YeuCauDVKTIds = d.YeuCauDichVuKyThuats.Select(g => g.Id).ToList(),
                    ThongTinNhanVienKhamKetQuaKhamSucKhoeData = d.KetQuaKhamSucKhoeData,
                    ThongTinNhanVienKhamLoaiLuuInKetQuaKSK = d.LoaiLuuInKetQuaKSK,
                    HopDongKhamSucKhoeNhanVienId = d.HopDongKhamSucKhoeNhanVienId,
                     
                    YeuCauDichVuKyThuatIds = d.YeuCauDichVuKyThuats.Select(g => new InfoDVKT { Id = g.Id, LoaiDichVuKyThuat = g.LoaiDichVuKyThuat  }).ToList(),
                }).ToList();

            // get info YeuCauKham
            var yckhamIds = entity.SelectMany(d => d.YeuCauKhamBenhIds).Select(d => d).ToList();
            var infoYeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(d => yckhamIds.Contains(d.Id) && 
                            d.GoiKhamSucKhoeId != null &&
                            d.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Select(v => new ThongTinNhanVienKhamTheoYeuCauKhamBenh
                {
                    Id = v.Id,
                    TenDichVuId = v.DichVuKhamBenhBenhVienId,
                    ThongTinKhamTheoDichVuTemplate = v.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = v.ThongTinKhamTheoDichVuData,
                    TrangThaiDVKham = (int)v.TrangThai,
                    HopDongKhamSucKhoeNhanVienId = v.YeuCauTiepNhanId
                }).ToList();

            // get info Yeu cầu dịch vụ kỹ thuật TDCN/CDHA
            var ycDVKIds = entity.SelectMany(d => d.YeuCauDichVuKyThuatIds).Select(d => d.Id).ToList();
            var infoDVKTTDCNCDHAs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => ycDVKIds.Contains(s.Id) && 
                            s.GoiKhamSucKhoeId != null && 
                            s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && 
                            (s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
               .Select(z => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatTDCNCDHA
               {
                   DataKetQuaCanLamSang = z.DataKetQuaCanLamSang,
                   TenDichVuKyThuat = z.TenDichVu,
                   Id = z.Id,
                   GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                   TrangThaiDVKham = (int)z.TrangThai
               }).ToList();
            // get in fo yêu cầu dịch vụ kỹ thuật xét nghiệm

            var infoXetNghiems = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => ycDVKIds.Contains(s.Id) &&  
                            s.GoiKhamSucKhoeId != null &&
                            s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                            s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                .Select(z => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatXetNghiem
                {
                    TenDichVuKyThuat = z.TenDichVu,
                    Id = z.Id,
                    GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                    TrangThaiDVKham = (int)z.TrangThai
                }).ToList();

           
            // get dịch vụ kỹ thuật khác CLS , ! thủ thuật phẩu huật
            var infoDichVuKhacClSVaKhacThuThuatPTs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => ycDVKIds.Contains(s.Id) &&
                            s.GoiKhamSucKhoeId != null &&
                            s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                            s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem &&
                            s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh &&
                            s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang &&
                            s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                .Select(d => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuat
                {
                    Id = d.Id,
                    GoiKhamSucKhoeId = d.GoiKhamSucKhoeId,
                    TenDichVuKyThuat = d.TenDichVu,
                    TrangThaiDVKham = (int)d.TrangThai
                }).ToList();

            // get in fo yêu cầu dịch vụ kỹ thuật  ! xét nghiệm !TDCN/CDHA
            var infopttts = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => ycDVKIds.Contains(s.Id) &&
                            s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                            s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                .Select(d => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuat
                {
                    Id = d.Id,
                    GoiKhamSucKhoeId = d.GoiKhamSucKhoeId,
                    TenDichVuKyThuat = d.TenDichVu,
                    TrangThaiDVKham = (int)d.TrangThai,
                }).ToList();

            var dichVuKyThuatTuongTrinhPTTTIds = entity.SelectMany(d => d.YeuCauDichVuKyThuatIds)
                .Where(d => d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                .Select(d => d.Id).ToList();

            var ketQuaPTTTs = _yeuCauDichVuKyThuatTuongTrinhPTTTRepository.TableNoTracking.Where(d => dichVuKyThuatTuongTrinhPTTTIds.Contains(d.Id))
                      .Select(d => new KetQuaDichVuKyThuatPTTT {
                          Id = d.Id,
                          ketQua = d.GhiChuCaPTTT
                      }).ToList();
                         

            #region get list tên dịch vụ khám
            var selectTenDichVuKhamBenhViens = _dichVuKhamBenhBenhVienRepository.TableNoTracking.Select(d => new { Id = d.Id, TenDichVu = d.Ten }).ToList();
            #endregion get list tên dịch vụ khám
            #region get list dịch vụ kỹ thuật
            var ids = entity.SelectMany(d => d.YeuCauDichVuKyThuatIds).Where(d=>d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).Select(d=>d.Id).ToList();

            var listPhienXetNghiemChiTietss = _phienXetNghiemChiTietRepository.TableNoTracking
                .Include(d => d.KetQuaXetNghiemChiTiets)
                .Where(s => ids.Contains(s.YeuCauDichVuKyThuatId)).ToList();


            #region dịch vụ xét nghiệm kết nối chỉ số
            var dichVuXetNghiemKetNoiChiSos = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Where(d => d.DichVuXetNghiemId != null && d.MauMayXetNghiemId != null).Select(s => new
            {
                Id = s.Id,
                TenKetNoi = s.TenKetNoi
            }).ToList();
            #endregion dịch vụ xét nghiệm kết nối chỉ số

            #endregion get list tên dịch vụ khám

            foreach (var info in entity)
            {
                var thongTinInfo = new ThongTinInfo();

                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs = infoYeuCauKhams.Where(d=> info.YeuCauKhamBenhIds.Contains(d.Id)).ToList();
                // lấy tên dịch vụ dịch vụ khám
                if (thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.Any())
                {
                    foreach (var infoYeuCauKhamBenh in thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.ToList())
                    {
                        infoYeuCauKhamBenh.TenDichVu = selectTenDichVuKhamBenhViens.Where(d => d.Id == infoYeuCauKhamBenh.TenDichVuId).Select(d => d.TenDichVu).FirstOrDefault();
                    }
                }

                thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs = infoDVKTTDCNCDHAs.Where(d => info.YeuCauDVKTIds.Contains(d.Id)).ToList();

                thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs = infoXetNghiems.Where(d => info.YeuCauDVKTIds.Contains(d.Id)).ToList();

                // lấy info phiên kêt qua xet nghien
                if (thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.Any())
                {
                    foreach (var idDichVuKyThuat in thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.ToList())
                    {
                        idDichVuKyThuat.DataKetQuaCanLamSangVo = null;
                        idDichVuKyThuat.DataKetQuaCanLamSangVo = listPhienXetNghiemChiTietss.Where(d => d.YeuCauDichVuKyThuatId == idDichVuKyThuat.Id)
                                                                                                                .Select(v => new DataKetQuaCanLamSangVo
                                                                                                                {
                                                                                                                    KetQuaXetNghiemChiTiets = v.KetQuaXetNghiemChiTiets.ToList(),
                                                                                                                    LanThucHien = v.LanThucHien,
                                                                                                                    KetLuan = v.KetLuan
                                                                                                                }).OrderBy(s => s.LanThucHien)
                                                                                                                .LastOrDefault();

                    }
                }

                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats = infoDichVuKhacClSVaKhacThuThuatPTs.Where(d => info.YeuCauDVKTIds.Contains(d.Id)).ToList();



                // BVHD-3877 thủ thuật phẩu thuật
                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats = infopttts.Where(d => info.YeuCauDVKTIds.Contains(d.Id)).ToList();

                foreach (var item in thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats)
                {
                    if(ketQuaPTTTs.Where(d=>d.Id == item.Id).Count() != 0)
                    {
                        item.KetQua = ketQuaPTTTs.Where(d => d.Id == item.Id).Select(d=>d.ketQua).FirstOrDefault();
                    }
                }




                thongTinInfo.ThongTinNhanVienKhamKetQuaKhamSucKhoeData = info.ThongTinNhanVienKhamKetQuaKhamSucKhoeData;
                thongTinInfo.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK = info.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK;
                thongTinInfo.HopDongKhamSucKhoeNhanVienId = info.HopDongKhamSucKhoeNhanVienId;
                thongTinInfos.Add(thongTinInfo);
            }

            List<DanhSachDichVuKhamGrid> listDichVu = new List<DanhSachDichVuKhamGrid>();
           

            if (thongTinInfos != null)
            {
                foreach (var thongTinNhanVienKham in thongTinInfos)
                {
                    var data = new KetQuaKhamSucKhoeVo();
                    var tableKham = "";
                    var tableKyThuat = "";
                    List<DanhSachDichVuKhamGrid> listDichVuCu = new List<DanhSachDichVuKhamGrid>();
                    List<long> listDichVuKyThuatTheoGoi = new List<long>();
                    if (thongTinNhanVienKham != null)
                    {
                        // DV Kham
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.Any())
                        {
                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauKhamBenhs)
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDv.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKhamBenh;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVu;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                                dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;
                                //dvObject.TrangThaiThucHienDichVu = itemDv.TrangThaiThucHienDichVu == 5 ? 3 :0;// 3 đã khám
                                
                                if (itemDv.ThongTinKhamTheoDichVuTemplate != null && itemDv.ThongTinKhamTheoDichVuData != null && itemDv.TrangThaiDVKham != 6) 
                                {
                                    var jsonOjbectTemplate = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(itemDv.ThongTinKhamTheoDichVuTemplate);
                                    var jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);

                                    //cập nhật BVHD-3880 cập nhật trạng thái dịch vụ
                                    if (jsonOjbectData.DataKhamTheoTemplate.Count() == 0)
                                    {
                                        if (dvObject.TenDichVu == "Khám Ngoại")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa);
                                        }
                                        if (dvObject.TenDichVu == "Khám Mắt")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.Mat);
                                        }

                                        if (dvObject.TenDichVu == "Khám Răng Hàm Mặt")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.RangHamMat);
                                        }

                                        if (dvObject.TenDichVu == "Khám Tai Mũi Họng")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                        }
                                        if (dvObject.TenDichVu == "Khám Da liễu")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                        }
                                        if (dvObject.TenDichVu == "Nội khoa")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa);
                                        }
                                        if (dvObject.TenDichVu == "Sản phụ khoa")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa);
                                        }
                                        jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);
                                    }


                                    foreach (var itemx in jsonOjbectTemplate.ComponentDynamics)
                                    {
                                        var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemx.Id);
                                        if (kiemTra.Any())
                                        {
                                            switch (itemx.Id)
                                            {
                                                case "TuanHoan":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;
                                                case "HoHap":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "TieuHoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "ThanTietLieu":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "NoiTiet":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "CoXuongKhop":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "ThanKinh":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "TamThan":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "NgoaiKhoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenNgoaiKhoa;
                                                    break;
                                                case "SanPhuKhoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenSanPhuKhoa;
                                                    break;


                                                case "CacBenhVeMat":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaMat;
                                                    break;
                                                case "CacBenhTaiMuiHong":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaTaiMuiHong;
                                                    break;

                                                case "CacBenhRangHamMat":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;
                                                case "HamTren":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;
                                                case "HamDuoi":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;

                                                case "DaLieu":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenDaLieu;
                                                    break;
                                                default:
                                                    //do a different thing
                                                    break;
                                            }
                                        }


                                    }
                                }
                                dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVu;
                                listDichVu.Add(dvObject);
                            }

                        }

                        // list theo yêu cầu tiếp nhận
                        // CDHA TDCN
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs.Any())
                        {

                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDv.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;

                                if (itemDv.DataKetQuaCanLamSang != null && itemDv.TrangThaiDVKham == 3) // ==3 => dịch vụ đã thực hiện
                                {
                                    var jsonOjbect = JsonConvert.DeserializeObject<DataCLS>(itemDv.DataKetQuaCanLamSang);

                                    var ketLuan = jsonOjbect.KetLuan;
                                    if (!string.IsNullOrEmpty(ketLuan))
                                    {
                                        ketLuan = CommonHelper.StripHTML(Regex.Replace(ketLuan, "</p>(?![\n\r]+)", Environment.NewLine));
                                        if (ketLuan.Length > 2 && ketLuan.Substring(ketLuan.Length - 2) == "\r\n")
                                        {
                                            ketLuan = ketLuan.Remove(ketLuan.Length - 2);
                                        }
                                    }
                                    dvObject.KetQuaDichVuDefault = ketLuan;
                                    dvObject.KetQuaDichVu = ketLuan;
                                }

                                listDichVu.Add(dvObject);
                            }
                        }

                        // xét nghiệm
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.Any())
                        {

                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();


                                dvObject.Id = itemDv.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                                dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                                if (itemDv.DataKetQuaCanLamSangVo != null && itemDv.TrangThaiDVKham == 3) // ==3 => đã thực hiện
                                {
                                    // phiên xét nghiệm chi tiết orderby cuoi cung
                                    if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets != null)
                                    {
                                        if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Any())
                                        {
                                            if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count == 1)
                                            {
                                                var itemGiaTriMin = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMin).First();
                                                var itemGiaTriMax = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMax).First();
                                                var itemGTDuyet = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriDuyet).First();
                                                var itemGiaTriNhapTay = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriNhapTay).First();
                                                var itemGiaTriTuMay = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriTuMay).First();
                                                var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; 

                                                if (!string.IsNullOrEmpty(value) && IsInt(value))
                                                {
                                                    double ketQua = !string.IsNullOrEmpty(value) ? Convert.ToDouble(value) : 0;
                                                   
                                                    if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                    {
                                                        dvObject.KetQuaDichVu = ketQua.ToString() + "";
                                                        dvObject.KetQuaDichVuDefault = ketQua.ToString() + "";
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                    if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                    {
                                                        if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                            }
                                                            else
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                }
                                                            }

                                                        }

                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                }
                                                if (!string.IsNullOrEmpty(value) && !IsInt(value))
                                                {
                                                    string ketQua = !string.IsNullOrEmpty(value) ? value : " ";
                                                    dvObject.KetQuaDichVu = ketQua.ToString() + " ";
                                                    dvObject.KetQuaDichVuDefault = ketQua.ToString() + " ";
                                                }
                                            }
                                            else
                                            {
                                                int itemCongKyTu = 0;
                                                foreach (var itemKetQuaListCon in itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.OrderByDescending(d => d.CapDichVu == 1 ? 1 : 0).ThenBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList())
                                                {
                                                    var dichVuXetNghiemId = itemKetQuaListCon.DichVuXetNghiemId;
                                                    var mauMayXetNghiemId = itemKetQuaListCon.MauMayXetNghiemId;
                                                    var tenketQua = "";
                                                    // nếu  mẫu máy xét nghiệm khác null => lấy ten dich vụ xét nghiệm trong db.DichVuXetNghiemKetNoiChiSo => field : TenKetNoi
                                                    // DichVuXetNghiemKetNoiChiSoId != null 
                                                    if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null)
                                                    {
                                                        tenketQua = dichVuXetNghiemKetNoiChiSos.Where(s => s.Id == itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId.GetValueOrDefault()).Select(s => s.TenKetNoi).FirstOrDefault();
                                                        if (tenketQua != null)
                                                        {
                                                            dvObject.KetQuaDichVu += tenketQua + ": ";
                                                            dvObject.KetQuaDichVuDefault += tenketQua + ": ";
                                                        }

                                                        var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                        var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                        var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                        var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                        var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                        var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                        double ketQua;
                                                        bool KieuSo = false;
                                                        if (value != null)
                                                        {
                                                            KieuSo = IsInt(value) ? true : false;
                                                        }
                                                        else
                                                        {
                                                            KieuSo = false;
                                                        }
                                                        if (KieuSo == true)
                                                        {
                                                            double cSBTMin = 0;
                                                            double cSBTMax = 0;
                                                            ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                            if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                            if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                            {
                                                                if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                        if (!string.IsNullOrEmpty(max))
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                        }
                                                                        else
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                        }
                                                                    }

                                                                }

                                                            }
                                                            if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                                {
                                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                    if (!string.IsNullOrEmpty(max))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE

                                                        }
                                                        if (KieuSo == false)
                                                        {
                                                            dvObject.KetQuaDichVu += value + "";
                                                            dvObject.KetQuaDichVuDefault += value + "";
                                                        }

                                                        if (itemCongKyTu < itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count())
                                                        {
                                                            dvObject.KetQuaDichVu += "; ";
                                                            dvObject.KetQuaDichVuDefault += "; ";
                                                        }

                                                    }
                                                    // nếu mẫu máy xét nghiệm == null => tên dịch vụ xét nghiệm trong db.KetQuaXetNghiemChiTiet => field :DichVuXetNghiemTen 
                                                    // DichVuXetNghiemKetNoiChiSoId == null  
                                                    if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId == null)
                                                    {
                                                        dvObject.KetQuaDichVu += itemKetQuaListCon.DichVuXetNghiemTen + ": ";
                                                        dvObject.KetQuaDichVuDefault += itemKetQuaListCon.DichVuXetNghiemTen + ": ";

                                                        var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                        var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                        var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                        var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                        var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                        //var value = itemGTDuyet ?? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? null));
                                                        var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                        double ketQua;
                                                        bool KieuSo = false;
                                                        if (value != null)
                                                        {
                                                            KieuSo = IsInt(value) ? true : false;
                                                        }
                                                        else
                                                        {
                                                            KieuSo = false;
                                                        }
                                                        double cSBTMin = 0;
                                                        double cSBTMax = 0;
                                                        if (KieuSo == true)
                                                        {
                                                            ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                            if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                            if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                            {
                                                                if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                        if (!string.IsNullOrEmpty(max))
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                        }
                                                                        else
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                        }
                                                                    }

                                                                }

                                                            }
                                                            if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                                {
                                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                    if (!string.IsNullOrEmpty(max))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                        }
                                                        if (KieuSo == false)
                                                        {
                                                            dvObject.KetQuaDichVu += value + "";
                                                            dvObject.KetQuaDichVuDefault += value + "";
                                                        }

                                                        if (itemCongKyTu < itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count())
                                                        {
                                                            dvObject.KetQuaDichVu += "; ";
                                                            dvObject.KetQuaDichVuDefault += "; ";
                                                        }
                                                    }
                                                    itemCongKyTu++;
                                                }
                                            }

                                            if (dvObject.KetQuaDichVu == "" || dvObject.KetQuaDichVu == "0")
                                            {
                                                dvObject.KetQuaDichVu = "";
                                                dvObject.KetQuaDichVuDefault = "";
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(dvObject.KetQuaDichVu))
                                {
                                    dvObject.KetQuaDichVu = dvObject.KetQuaDichVu.Split(";").Where(d => d != "" && d != " ").ToList().Distinct().Join(";");
                                }
                                if (!string.IsNullOrEmpty(dvObject.KetQuaDichVuDefault))
                                {
                                    dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVuDefault.Split(";").Where(d => d != "" && d != " ").ToList().Distinct().Join(";");
                                }
                                listDichVu.Add(dvObject);
                            }
                        }

                        // BVHD-3668 -> lấy những dịch vụ kỹ thuật khác cls -> kết quả để null
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats.Any())
                        {
                            foreach (var itemDVKTKhacCLS in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDVKTKhacCLS.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                //dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThaiDVKham;
                                dvObject.KetQuaDichVu = string.Empty; // để tự nhập
                                dvObject.KetQuaDichVuDefault = string.Empty; // để tự nhập
                                listDichVu.Add(dvObject);
                            }
                        }
                        // BVHD-3877 ->thủ thuật phẩu thuật
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats.Any())
                        {
                            foreach (var itemDVKTKhacCLS in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDVKTKhacCLS.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                //dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThaiDVKham;
                                dvObject.KetQuaDichVu = itemDVKTKhacCLS.KetQua;
                                dvObject.KetQuaDichVuDefault = itemDVKTKhacCLS.KetQua;
                                listDichVu.Add(dvObject);
                            }
                        }

                        if (thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData != null)
                        {
                            // chạy những data cũ  chưa lưu người thực hiện , và thời điểm thực hiện trong josn
                            if (thongTinNhanVienKham.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK == null)
                            {
                                listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData);
                                // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                                foreach (var itemxDvMoi in listDichVu)
                                {
                                    foreach (var dvcu in listDichVuCu)
                                    {
                                        if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                        {
                                            if (dvcu.KetQuaDaDuocLuu == true)
                                            {
                                                itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var jonKetLuan = JsonConvert.DeserializeObject<KetQuaKhamSucKhoeDaTa>(thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData);
                                if (!string.IsNullOrEmpty(jonKetLuan.KetQuaKhamSucKhoe))
                                {
                                    listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(jonKetLuan.KetQuaKhamSucKhoe);
                                    // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                                    // data mới KetQuaDaDuocLuu = true, false
                                    if (listDichVuCu.Where(d => d.KetQuaDaDuocLuu != null).ToList().Count() != 0)
                                    {
                                        foreach (var itemxDvMoi in listDichVu)
                                        {
                                            foreach (var dvcu in listDichVuCu)
                                            {
                                                if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                                {
                                                    if (dvcu.KetQuaDaDuocLuu == true)
                                                    {
                                                        itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else // Trường hợp data cũ  KetQuaDaDuocLuu null
                                    {
                                        foreach (var itemxDvMoi in listDichVu)
                                        {
                                            foreach (var dvcu in listDichVuCu)
                                            {
                                                if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                                {
                                                    itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    // xử lý list string trùng nhau (dịch vụ khám)
                    //List<DanhSachDichVuKhamGrid> listDichVuLoaiTrung = new List<DanhSachDichVuKhamGrid>();

                    foreach (var itemTrung in listDichVu.Where(s => s.NhomId != EnumNhomGoiDichVu.DichVuKyThuat).ToList())
                    {
                        if (!string.IsNullOrEmpty(itemTrung.KetQuaDichVu) && !string.IsNullOrEmpty(itemTrung.KetQuaDichVuDefault))
                        {
                            var catstring = itemTrung.KetQuaDichVu.Split('.');
                            var catstringdefault = itemTrung.KetQuaDichVuDefault.Split('.');
                            itemTrung.KetQuaDichVu = catstring.Where(d => d != null && d != "").Distinct().Join(".");
                            itemTrung.KetQuaDichVuDefault = catstringdefault.Where(d => d != null && d != "").Distinct().Join(".");

                        }
                    }
                }


            }

            return listDichVu.OrderBy(o => o.TenDichVu).ToList(); // trả về 1 list dịch vụ (dịch vụ khám , cls (dịch vụ kỹ thuật))
        }
        #endregion

        // BVHD-3880 cập nhật 
        public string SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe? chuyenKhoaKhamSucKhoe)
        {
            var thongTinKhamData = string.Empty;
            switch (chuyenKhoaKhamSucKhoe)
            {
                case Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"TuanHoan\",\"Value\":\"\"},{\"Id\":\"TuanHoanPhanLoai\",\"Value\":\"\"},{\"Id\":\"HoHap\",\"Value\":\"\"},{\"Id\":\"HoHapPhanLoai\",\"Value\":\"\"},{\"Id\":\"TieuHoa\",\"Value\":\"\"},{\"Id\":\"TieuHoaPhanLoai\",\"Value\":\"\"},{\"Id\":\"ThanTietLieu\",\"Value\":\"\"},{\"Id\":\"ThanTietLieuPhanLoai\",\"Value\":\"\"},{\"Id\":\"NoiTiet\",\"Value\":\"\"},{\"Id\":\"NoiTietPhanLoai\",\"Value\":\"\"},{\"Id\":\"CoXuongKhop\",\"Value\":\"\"},{\"Id\":\"CoXuongKhopPhanLoai\",\"Value\":\"\"},{\"Id\":\"ThanKinh\",\"Value\":\"\"},{\"Id\":\"ThanKinhPhanLoai\",\"Value\":\"\"},{\"Id\":\"TamThan\",\"Value\":\"\"},{\"Id\":\"TamThanPhanLoai\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"NgoaiKhoa\",\"Value\":\"\"},{\"Id\":\"NgoaiKhoaPhanLoai\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"SanPhuKhoa\",\"Value\":\"\"},{\"Id\":\"\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.Mat:
                    //thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"KhongKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"KhongKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CacBenhVeMat\",\"Value\":\"Bình thường\"},{\"Id\":\"MatPhanLoai\",\"Value\":1}]}";
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"KhongKinhMatPhai\",\"Value\":\"\"},{\"Id\":\"KhongKinhMatTrai\",\"Value\":\"\"},{\"Id\":\"CacBenhVeMat\",\"Value\":\"\"},{\"Id\":\"MatPhanLoai\",\"Value\":\"\"}]}"; //{\"Id\":\"CoKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatTrai\",\"Value\":\"10/10\"},
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.RangHamMat:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"HamTren\",\"Value\":\"\"},{\"Id\":\"HamDuoi\",\"Value\":\"\"},{\"Id\":\"CacBenhRangHamMat\",\"Value\":\"\"},{\"Id\":\"RangHamMatPhanLoai\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"TaiPhaiNoiThuong\",\"Value\":\"\"},{\"Id\":\"TaiPhaiNoiTham\",\"Value\":\"\"},{\"Id\":\"TaiTraiNoiThuong\",\"Value\":\"\"},{\"Id\":\"TaiTraiNoiTham\",\"Value\":\"\"},{\"Id\":\"CacBenhTaiMuiHong\",\"Value\":\"\"},{\"Id\":\"TaiMuiHongPhanLoai\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.DaLieu:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"DaLieu\",\"Value\":\"\"},{\"Id\":\"DaLieuPhanLoai\",\"Value\":\"\"}]}";
                    break;
                default:
                    thongTinKhamData = null;
                    break;
            }

            return thongTinKhamData;
        }
        #region BVHD 3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE   
        public static string GetStatusForXetNghiemGiaTriMin(string strGiaTriMin,string strGiaTriSoSanh)
        {
            var result = string.Empty;
            if (double.TryParse(strGiaTriSoSanh, out var giaTriSoSanh))
            {
                if (double.TryParse(strGiaTriMin, out var giaTriMin))
                {
                    if (giaTriSoSanh < giaTriMin)
                        result = " (Giảm)";
                }
                else if (KiemTraKhacThuong(giaTriSoSanh, strGiaTriMin))
                {

                    strGiaTriMin = strGiaTriMin.Trim();
                    if (strGiaTriMin.StartsWith(">="))
                    {
                        if (double.TryParse(strGiaTriMin.Replace(">=", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith("<="))
                    {
                        if (double.TryParse(strGiaTriMin.Replace("<=", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith("≥"))
                    {
                        if (double.TryParse(strGiaTriMin.Replace("≥", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith("≤"))
                    {
                        if (double.TryParse(strGiaTriMin.Replace("≤", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith(">"))
                    {
                        if (double.TryParse(strGiaTriMin.Replace(">", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith("<"))
                    {
                        if (double.TryParse(strGiaTriMin.Replace("<", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                }
            }
            return result;
        }
        public static string GetStatusForXetNghiemGiaTriMax( string strGiaTriMax, string strGiaTriSoSanh)
        {
            var result = string.Empty;
            if (double.TryParse(strGiaTriSoSanh, out var giaTriSoSanh))
            {
                if (double.TryParse(strGiaTriMax, out var giaTriMax))
                {
                    if (giaTriSoSanh > giaTriMax)
                        result = " (Tăng)";
                }
                else if (KiemTraKhacThuong(giaTriSoSanh, strGiaTriMax))
                {
                    strGiaTriMax = strGiaTriMax.Trim();
                    if (strGiaTriMax.StartsWith(">="))
                    {
                        if (double.TryParse(strGiaTriMax.Replace(">=", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith("<="))
                    {
                        if (double.TryParse(strGiaTriMax.Replace("<=", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith("≥"))
                    {
                        if (double.TryParse(strGiaTriMax.Replace("≥", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith("≤"))
                    {
                        if (double.TryParse(strGiaTriMax.Replace("≤", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith(">"))
                    {
                        if (double.TryParse(strGiaTriMax.Replace(">", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith("<"))
                    {
                        if (double.TryParse(strGiaTriMax.Replace("<", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                }
              
            }
            return result;
        }

        private static bool KiemTraKhacThuong(double giaTriSoSanh, string str)
        {
            if (str == null)
            {
                return false;
            }
            str = str.Trim();
            if (str.StartsWith(">="))
            {
                return true;
            }
            if (str.StartsWith("<="))
            {
                return true;
            }
            if (str.StartsWith("≥"))
            {
                return true;
            }
            if (str.StartsWith("≤"))
            {
                return true;
            }
            if (str.StartsWith(">"))
            {
                return true;
            }
            if (str.StartsWith("<"))
            {
                return true;
            }
            return false;
        }
        #endregion
        #region kiểm tra đúng dịch vụ của bệnh nhân
        public bool CheckDungDichVuCuaBenhNhan(DichVuGridVos vo)
        {
            var result = true;
            if(vo != null)
            {
                if(vo.NhomId == (long)EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    var yctnId = _yeuCauKhamBenhRepository.TableNoTracking.Where(d => d.Id == vo.IdDichVu).Select(d => d.YeuCauTiepNhanId).FirstOrDefault();

                    if(vo.YeuCauTiepNhanId != yctnId)
                    {
                        result = false;
                    }
                }
                else
                {
                    var yctnId = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(d => d.Id == vo.IdDichVu).Select(d => d.YeuCauTiepNhanId).FirstOrDefault();

                    if (vo.YeuCauTiepNhanId != yctnId)
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
