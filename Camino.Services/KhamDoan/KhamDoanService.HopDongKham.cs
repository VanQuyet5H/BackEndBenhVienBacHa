using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using Camino.Core.Domain;
using System;
using static Camino.Core.Domain.Enums;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public void MoLaiHopDongKhamSucKhoe(MoHopDongKhamViewModel moHopDongKhamViewModel)
        {
            var nhanVienId = _userAgentHelper.GetCurrentUserId();

            var hopDongKhamSucKhoe = _hopDongKhamSucKhoeRepository.Table.Where(o => o.Id == moHopDongKhamViewModel.HopDongKhamSucKhoeId).FirstOrDefault();

            hopDongKhamSucKhoe.DaKetThuc = false;

            hopDongKhamSucKhoe.LyDoMoLaiHopHopDong = moHopDongKhamViewModel.LyDoMoLaiHopDong;
            hopDongKhamSucKhoe.NhanVienMoLaiHopDongId = nhanVienId;
            hopDongKhamSucKhoe.NgayMoLaiHopDong = DateTime.Now;

            _hopDongKhamSucKhoeRepository.Update(hopDongKhamSucKhoe);

        }

        public async Task<GridDataSource> GetDSHopDongKhamForGrid(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (isAllData)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = _hopDongKhamSucKhoeRepository.TableNoTracking.Include(cc => cc.CongTyKhamSucKhoe)
                                                                     .Include(cc => cc.HopDongKhamSucKhoeDiaDiems)
                                       .Select(s => new KhamDoanHopDongKhamGridVo
                                       {
                                           Id = s.Id,
                                           SoHopDong = s.SoHopDong,
                                           TenCongTy = s.CongTyKhamSucKhoe.Ten,
                                           NgayHopDong = s.NgayHopDong,
                                           LoaiHopDong = (Camino.Core.Domain.Enums.LoaiHopDong)(s.LoaiHopDong),
                                           DiaChiKham = s.HopDongKhamSucKhoeDiaDiems.Any() ? string.Join(",", s.HopDongKhamSucKhoeDiaDiems.Where(cc => cc.DiaDiem != null).Select(cc => cc.DiaDiem).Distinct()) : string.Empty,
                                           NgayKham = s.HopDongKhamSucKhoeDiaDiems.Any() ? string.Join(",", s.HopDongKhamSucKhoeDiaDiems.Where(cc => cc.Ngay != null).Select(cc => cc.Ngay.Value.ApplyFormatDate()).Distinct()) : string.Empty,
                                           TrangThai = !s.DaKetThuc ? (int)Camino.Core.Domain.Enums.TrangThaiHopDongKham.DangThucHienHD : (int)Camino.Core.Domain.Enums.TrangThaiHopDongKham.DaKetThucHD,
                                           TenTrangThai = !s.DaKetThuc ? Camino.Core.Domain.Enums.TrangThaiHopDongKham.DangThucHienHD.GetDescription() : Camino.Core.Domain.Enums.TrangThaiHopDongKham.DaKetThucHD.GetDescription()
                                       });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SreachHopDongKhamVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    query = query.ApplyLike(queryInfo.SearchTerms.Replace("\t", "").Trim(),
                                              g => g.SoHopDong,
                                              g => g.TenCongTy,
                                              g => g.NgayHopDong.ToString(),
                                              g => g.TenTrangThai);
                }
            }


            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPagesDSHopDongKhamForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _hopDongKhamSucKhoeRepository.TableNoTracking.Include(cc => cc.CongTyKhamSucKhoe)
                                                                     .Include(cc => cc.HopDongKhamSucKhoeDiaDiems)
                                       .Select(s => new KhamDoanHopDongKhamGridVo
                                       {
                                           Id = s.Id,
                                           SoHopDong = s.SoHopDong,
                                           TenCongTy = s.CongTyKhamSucKhoe.Ten,
                                           NgayHopDong = s.NgayHopDong,
                                           LoaiHopDong = (Camino.Core.Domain.Enums.LoaiHopDong)(s.LoaiHopDong),
                                           DiaChiKham = string.Join(",", s.HopDongKhamSucKhoeDiaDiems.Where(cc => cc.DiaDiem != null).Select(cc => cc.DiaDiem).Distinct()),
                                           NgayKham = string.Join(",", s.HopDongKhamSucKhoeDiaDiems.Where(cc => cc.Ngay != null).Select(cc => cc.Ngay).Distinct()),
                                           TrangThai = !s.DaKetThuc ? (int)Camino.Core.Domain.Enums.TrangThaiHopDongKham.DangThucHienHD : (int)Camino.Core.Domain.Enums.TrangThaiHopDongKham.DaKetThucHD,
                                           TenTrangThai = !s.DaKetThuc ? Camino.Core.Domain.Enums.TrangThaiHopDongKham.DangThucHienHD.GetDescription() : Camino.Core.Domain.Enums.TrangThaiHopDongKham.DaKetThucHD.GetDescription()
                                       });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<SreachHopDongKhamVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    query = query.ApplyLike(queryInfo.SearchTerms.Replace("\t", "").Trim(),
                                              g => g.SoHopDong,
                                              g => g.TenCongTy,
                                              g => g.NgayHopDong.ToString(),
                                              g => g.TenTrangThai);
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<HopDongKhamSucKhoe> ThemHoacCapNhatHopDongKham(HopDongKhamSucKhoe hopDongKhamSucKhoe)
        {
            if (hopDongKhamSucKhoe != null && hopDongKhamSucKhoe.Id.Equals(0))
            {
                await _hopDongKhamSucKhoeRepository.AddAsync(hopDongKhamSucKhoe);
            }
            else
            {
                await _hopDongKhamSucKhoeRepository.UpdateAsync(hopDongKhamSucKhoe);
            }
            return hopDongKhamSucKhoe;
        }

        public Core.Domain.Entities.PhongBenhViens.PhongBenhVien GetPhongBenhVien(long phongBenhVienId)
        {
            var phongBenhVien = _phongBenhVienRepository.GetById(phongBenhVienId,
                                                        c => c.Include(cc => cc.KhoaPhong)
                                                              .Include(cc => cc.KhoaPhongNhanViens));
            return phongBenhVien;
        }

        public async Task<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> ThemHoacCapNhatDanhSachPhongKhamCongTy
                                                                        (Core.Domain.Entities.PhongBenhViens.PhongBenhVien phongBenhVien, List<long> DanhSachNhanSu)
        {
            if (phongBenhVien != null && phongBenhVien.Id.Equals(0))
            {
                //Tạo ra 1 khoa khám ngoại viện với mã : KKDNV
                var khoaNgoaiVien = _khoaPhongRepository.TableNoTracking.Where(c => c.Ma == "KKDNV").FirstOrDefault();
                phongBenhVien.KhoaPhongId = khoaNgoaiVien.Id;

                if (DanhSachNhanSu != null && DanhSachNhanSu.Any())
                {
                    foreach (var nhanVienId in DanhSachNhanSu)
                    {
                        phongBenhVien.KhoaPhongNhanViens.Add(new Camino.Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien
                        {
                            KhoaPhongId = phongBenhVien.KhoaPhongId,
                            NhanVienId = nhanVienId
                        });
                    }
                }
                await _phongBenhVienRepository.AddAsync(phongBenhVien);
            }
            else
            {
                //Xóa khoa phòng như viên củ
                var khoaPhongNhanViens = phongBenhVien.KhoaPhongNhanViens;
                _khoaPhongNhanVienRepository.Delete(khoaPhongNhanViens);
                //Tạo lại khoa phòng nhân viên  mới
                if (DanhSachNhanSu != null && DanhSachNhanSu.Any())
                {
                    foreach (var nhanVienId in DanhSachNhanSu)
                    {
                        phongBenhVien.KhoaPhongNhanViens.Add(new Camino.Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien
                        {
                            KhoaPhongId = phongBenhVien.KhoaPhongId,
                            NhanVienId = nhanVienId
                        });
                    }
                }
                await _phongBenhVienRepository.UpdateAsync(phongBenhVien);
            }
            return phongBenhVien;
        }

        public HopDongKhamSucKhoe GetThongTinHopDongKham(long hopDongKhamSucKhoeId)
        {
            //var hopDongKhamSucKhoe = _hopDongKhamSucKhoeRepository.GetById(hopDongKhamSucKhoeId,
            //                                            c => c.Include(cc => cc.HopDongKhamSucKhoeDiaDiems)
            //                                                  .Include(cc => cc.CongTyKhamSucKhoe)
            //                                                  .Include(cc => cc.HopDongKhamSucKhoeNhanViens).ThenInclude(cc => cc.YeuCauTiepNhans).ThenInclude(cc => cc.YeuCauKhamBenhs)
            //                                                  .Include(cc => cc.HopDongKhamSucKhoeNhanViens).ThenInclude(cc => cc.YeuCauTiepNhans).ThenInclude(cc => cc.YeuCauDichVuKyThuats));
            var hopDongKhamSucKhoe = _hopDongKhamSucKhoeRepository.GetById(hopDongKhamSucKhoeId,
                                                        c => c.Include(cc => cc.HopDongKhamSucKhoeDiaDiems)
                                                              .Include(cc => cc.CongTyKhamSucKhoe));
            return hopDongKhamSucKhoe;
        }

        public decimal GetGiaTriHopDong(long hopDongKhamSucKhoeId)
        {
            var yeuCauTiepNhanIds = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && o.HopDongKhamSucKhoeNhanVienId != null && o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                .Select(o => o.Id);

            var yeuCauKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.GoiKhamSucKhoeId != null && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Select(o=>new
                {
                    o.Id,
                    o.DonGiaUuDai
                })
                .ToList();
            var yeuCauKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.GoiKhamSucKhoeId != null && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                .Select(o => new
                {
                    o.Id,
                    o.SoLan,
                    o.DonGiaUuDai,
                })
                .ToList();
            return yeuCauKhamBenhs.Select(o => o.DonGiaUuDai.GetValueOrDefault()).DefaultIfEmpty().Sum() +
                yeuCauKyThuats.Select(o => o.DonGiaUuDai.GetValueOrDefault() * o.SoLan).DefaultIfEmpty().Sum();
        }

        public async Task<GridDataSource> GetDSHopDongKhamSucKhoeNhanVienForGrid(QueryInfo queryInfo, long hopDongKhamSucKhoeId, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var hopDongKhamSucKhoeNhanVien = hopDongKhamSucKhoeId != 0 ? _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(cc => cc.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                                                                  .Include(c => c.BenhNhan)
                                                                                  .Include(c => c.DanToc)
                                                                                  .Include(c => c.TinhThanh) :
                                                                        _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                                                                                  .Include(c => c.BenhNhan)
                                                                                  .Include(c => c.DanToc)
                                                                                  .Include(c => c.TinhThanh);
            var query = hopDongKhamSucKhoeNhanVien
                                                .Select(s => new HopDongKhamSucKhoeNhanVienGridVo
                                                {
                                                    Id = s.Id,
                                                    HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
                                                    STTNhanVien = s.STTNhanVien,
                                                    MaBN = s.BenhNhan.MaBN,
                                                    MaNhanVien = s.MaNhanVien,
                                                    HoTen = s.HoTen.ToUpper(),
                                                    TenDonVi = s.TenDonViHoacBoPhan,
                                                    ViTriCongTac = string.Empty,
                                                    GioiTinh = s.GioiTinh.GetDescription(),
                                                    CMTSHC = s.SoChungMinhThu,
                                                    NamSinh = s.NamSinh,
                                                    SoDienThoai = s.SoDienThoai,
                                                    HoTenKhac = s.HoTenKhac,
                                                    Email = s.Email,
                                                    DanToc = s.DanToc.Ten,
                                                    TinhThanhPho = s.TinhThanh.Ten,
                                                    NhomKham = s.NhomDoiTuongKhamSucKhoe,
                                                    GhiChu = s.GhiChuDiUngThuoc + s.GhiChuTienSuBenh,
                                                    DaLapGiaDinh = s.DaLapGiaDinh
                                                });


            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                                            g => g.CMTSHC,
                                            g => g.MaBN,
                                            g => g.MaNhanVien,
                                            g => g.HoTen).OrderBy(queryInfo.SortString);

            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPagesDSHopDongKhamSucKhoeNhanVienForGrid(QueryInfo queryInfo, long hopDongKhamSucKhoeId)
        {
            BuildDefaultSortExpression(queryInfo);
            var hopDongKhamSucKhoeNhanVien = hopDongKhamSucKhoeId != 0 ? _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(cc => cc.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                                                                  .Include(c => c.BenhNhan)
                                                                                  .Include(c => c.DanToc)
                                                                                  .Include(c => c.TinhThanh) :
                                                                        _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                                                                                  .Include(c => c.BenhNhan)
                                                                                  .Include(c => c.DanToc)
                                                                                  .Include(c => c.TinhThanh);
            var query = hopDongKhamSucKhoeNhanVien
                                                .Select(s => new HopDongKhamSucKhoeNhanVienGridVo
                                                {
                                                    Id = s.Id,
                                                    HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
                                                    MaBN = s.BenhNhan.MaBN,
                                                    MaNhanVien = s.MaNhanVien,
                                                    HoTen = s.HoTen,
                                                    TenDonVi = s.TenDonViHoacBoPhan,
                                                    ViTriCongTac = string.Empty,
                                                    GioiTinh = s.GioiTinh.GetDescription(),
                                                    CMTSHC = s.SoChungMinhThu,
                                                    NamSinh = s.NamSinh,
                                                    SoDienThoai = s.SoDienThoai,
                                                    Email = s.Email,
                                                    DanToc = s.DanToc.Ten,
                                                    TinhThanhPho = s.TinhThanh.Ten,
                                                    NhomKham = s.NhomDoiTuongKhamSucKhoe,
                                                    GhiChu = s.GhiChuDiUngThuoc + s.GhiChuTienSuBenh,
                                                    DaLapGiaDinh = s.DaLapGiaDinh
                                                });


            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                                          g => g.CMTSHC,
                                          g => g.MaBN,
                                          g => g.MaNhanVien,
                                          g => g.HoTen).OrderBy(queryInfo.SortString);

            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public int GetTotalDanhSachNhanVienCongTyTheoHopDongKham(long hopDongKhamSucKhoeId)
        {

            var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(cc => cc.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                                                                  .Include(c => c.BenhNhan)
                                                                                  .Include(c => c.DanToc)
                                                                                  .Include(c => c.TinhThanh);
            return hopDongKhamSucKhoeNhanVien.Count();
        }


        public async Task<HopDongKhamSucKhoeNhanVien> ThemHoacCapNhatHopDongKhamSucKhoeNhanVien(HopDongKhamSucKhoeNhanVien hopDongKhamSucKhoeNhanVien)
        {
            if (hopDongKhamSucKhoeNhanVien != null && hopDongKhamSucKhoeNhanVien.Id.Equals(0))
            {
                await _hopDongKhamSucKhoeNhanVienRepository.AddAsync(hopDongKhamSucKhoeNhanVien);
            }
            else
            {
                // Update BN 
                if (hopDongKhamSucKhoeNhanVien.BenhNhanId != null && hopDongKhamSucKhoeNhanVien.BenhNhanId != 0)
                {
                    var yeuCauTiepNhan = hopDongKhamSucKhoeNhanVien.YeuCauTiepNhans.Where(c => c.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe).FirstOrDefault();
                    if (yeuCauTiepNhan != null)
                    {
                        yeuCauTiepNhan.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                        yeuCauTiepNhan.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                        yeuCauTiepNhan.NgaySinh = hopDongKhamSucKhoeNhanVien.NgaySinh;
                        yeuCauTiepNhan.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                        yeuCauTiepNhan.PhuongXaId = hopDongKhamSucKhoeNhanVien.PhuongXaId;
                        yeuCauTiepNhan.TinhThanhId = hopDongKhamSucKhoeNhanVien.TinhThanhId;
                        yeuCauTiepNhan.QuanHuyenId = hopDongKhamSucKhoeNhanVien.QuanHuyenId;
                        yeuCauTiepNhan.QuocTichId = hopDongKhamSucKhoeNhanVien.QuocTichId;
                        yeuCauTiepNhan.DanTocId = hopDongKhamSucKhoeNhanVien.DanTocId;
                        yeuCauTiepNhan.DiaChi = hopDongKhamSucKhoeNhanVien.DiaChi;
                        yeuCauTiepNhan.SoDienThoai = hopDongKhamSucKhoeNhanVien.SoDienThoai;
                        yeuCauTiepNhan.SoChungMinhThu = hopDongKhamSucKhoeNhanVien.SoChungMinhThu;
                        yeuCauTiepNhan.Email = hopDongKhamSucKhoeNhanVien.Email;
                        yeuCauTiepNhan.NgheNghiepId = hopDongKhamSucKhoeNhanVien.NgheNghiepId;
                        yeuCauTiepNhan.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                        yeuCauTiepNhan.ThangSinh = hopDongKhamSucKhoeNhanVien.ThangSinh;

                        yeuCauTiepNhan.BenhNhan.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                        yeuCauTiepNhan.BenhNhan.NgaySinh = hopDongKhamSucKhoeNhanVien.NgaySinh;
                        yeuCauTiepNhan.BenhNhan.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                        yeuCauTiepNhan.BenhNhan.PhuongXaId = hopDongKhamSucKhoeNhanVien.PhuongXaId;
                        yeuCauTiepNhan.BenhNhan.TinhThanhId = hopDongKhamSucKhoeNhanVien.TinhThanhId;
                        yeuCauTiepNhan.BenhNhan.QuanHuyenId = hopDongKhamSucKhoeNhanVien.QuanHuyenId;
                        yeuCauTiepNhan.BenhNhan.QuocTichId = hopDongKhamSucKhoeNhanVien.QuocTichId;
                        yeuCauTiepNhan.BenhNhan.DanTocId = hopDongKhamSucKhoeNhanVien.DanTocId;
                        yeuCauTiepNhan.BenhNhan.DiaChi = hopDongKhamSucKhoeNhanVien.DiaChi;
                        yeuCauTiepNhan.BenhNhan.SoDienThoai = hopDongKhamSucKhoeNhanVien.SoDienThoai;
                        yeuCauTiepNhan.BenhNhan.SoChungMinhThu = hopDongKhamSucKhoeNhanVien.SoChungMinhThu;
                        yeuCauTiepNhan.BenhNhan.Email = hopDongKhamSucKhoeNhanVien.Email;
                        yeuCauTiepNhan.BenhNhan.NgheNghiepId = hopDongKhamSucKhoeNhanVien.NgheNghiepId;
                        yeuCauTiepNhan.BenhNhan.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                        yeuCauTiepNhan.BenhNhan.ThangSinh = hopDongKhamSucKhoeNhanVien.ThangSinh;
                        await _yeuCauTiepNhanRepository.UpdateAsync(yeuCauTiepNhan);

                    }
                }
                await _hopDongKhamSucKhoeNhanVienRepository.UpdateAsync(hopDongKhamSucKhoeNhanVien);
            }

            return hopDongKhamSucKhoeNhanVien;
        }

        public HopDongKhamSucKhoeNhanVien GetThongTinHopDongKhamSucKhoeNhanVien(long hopDongKhamSucKhoeNhanVienId)
        {
            var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.GetById(hopDongKhamSucKhoeNhanVienId,
                                                                                        c => c.Include(cc => cc.BenhNhan)
                                                                                              .Include(cc => cc.GoiKhamSucKhoe)
                                                                                              .Include(cc => cc.YeuCauTiepNhans));
            return hopDongKhamSucKhoeNhanVien;
        }

        public List<HopDongKhamSucKhoeNhanVien> NhanVienKhamSucKhoeTheoHDs(long hopDongKhamSucKhoeId)
        {
            var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId).ToList();
            return hopDongKhamSucKhoeNhanVien;
        }

        public List<GoiKhamSucKhoe> GetGoiKhamTheoMaHDKvaMa(long hopDongKhamSucKhoeId, string maGoiKham)
        {
            var goiKhamSucKhoes = _goiKhamSucKhoeRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && c.Ma.Trim().ToUpper() == maGoiKham.Trim().ToUpper()).ToList();
            return goiKhamSucKhoes;
        }

        public List<KiemTraHopDongNhanVienChuaKham> KiemTraHopDongNhanVienChuaKham(long hopDongKhamSucKhoeId)
        {
            var data = new List<KiemTraHopDongNhanVienChuaKham>();
            var hopDongKhamSucKhoeNhanVienIds = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                                                                      .Include(c => c.YeuCauTiepNhans).ToList();
            if (hopDongKhamSucKhoeNhanVienIds.Any())
            {

                foreach (var hopDongKhamNhanVien in hopDongKhamSucKhoeNhanVienIds)
                {
                    DateTime ngayThangNamSinh;

                    hopDongKhamNhanVien.ThangSinh = hopDongKhamNhanVien.ThangSinh != 0 ? hopDongKhamNhanVien.ThangSinh : null;
                    hopDongKhamNhanVien.NgaySinh = hopDongKhamNhanVien.NgaySinh != 0 ? hopDongKhamNhanVien.NgaySinh : null;

                    ngayThangNamSinh = new DateTime(hopDongKhamNhanVien.NamSinh ?? 1500, hopDongKhamNhanVien.ThangSinh ?? 1, hopDongKhamNhanVien.NgaySinh ?? 1);
                    var hdkNhanVien = hopDongKhamNhanVien.YeuCauTiepNhans.OrderByDescending(c => c.Id).FirstOrDefault();

                    if (hdkNhanVien != null)
                    {
                        var query = BaseRepository.TableNoTracking
                                                       .Where(x => x.Id == hdkNhanVien.Id
                                                                   && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                                                   && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat
                                                                   && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                                                       .Select(item => new KiemTraHopDongNhanVienChuaKham()
                                                       {
                                                           GioiTinh = hopDongKhamNhanVien.GioiTinh.GetDescription(),
                                                           MaNhanVien = hopDongKhamNhanVien.MaNhanVien,
                                                           NgaySinh = hopDongKhamNhanVien.ThangSinh != null && hopDongKhamNhanVien.NgaySinh != null ? ngayThangNamSinh.ApplyFormatDate() : ngayThangNamSinh.Year.ToString(),
                                                           TenNhanVien = hopDongKhamNhanVien.HoTen,
                                                           TenTrangThai = "Đang khám",
                                                           TrangThai = 1,
                                                           NhanVienKhamXong = false
                                                       });
                        data.AddRange(query);
                    }
                    else
                    {
                        var query = new KiemTraHopDongNhanVienChuaKham()
                        {
                            GioiTinh = hopDongKhamNhanVien.GioiTinh.GetDescription(),
                            MaNhanVien = hopDongKhamNhanVien.MaNhanVien,
                            NgaySinh = hopDongKhamNhanVien.ThangSinh != null && hopDongKhamNhanVien.NgaySinh != null ? ngayThangNamSinh.ApplyFormatDate() : ngayThangNamSinh.Year.ToString(),
                            TenNhanVien = hopDongKhamNhanVien.HoTen,
                            TenTrangThai = "Chưa khám",
                            TrangThai = 2,
                            NhanVienKhamXong = false
                        };
                        data.Add(query);
                    }
                }
            }

            if (!data.Any())
            {
                var query = new KiemTraHopDongNhanVienChuaKham()
                {
                    NhanVienKhamXong = true
                };
                data.Add(query);
            }

            return data;
        }

        public async void KetThucHopDongKham(long hopDongKhamSucKhoeId)
        {
            var hopDongKham = _hopDongKhamSucKhoeRepository.TableNoTracking.FirstOrDefault(c => c.Id == hopDongKhamSucKhoeId);
            var hopDongKhamSucKhoeNhanVienIds = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                                                                      .Include(c => c.YeuCauTiepNhans).ToList();

            var khamTatCaNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                                                        .Where(c => c.YeuCauTiepNhans.Any(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                                                                   && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat
                                                                                   && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy));
            if (khamTatCaNhanVien.Any())
            {
                if (hopDongKhamSucKhoeNhanVienIds.Any())
                {

                    foreach (var hopDongKhamNhanVien in hopDongKhamSucKhoeNhanVienIds)
                    {
                        var yeuCauTiepNhan = hopDongKhamNhanVien.YeuCauTiepNhans.OrderByDescending(c => c.Id).FirstOrDefault();

                        if (yeuCauTiepNhan != null)
                        {
                            var yeuCauKhamBenhs = yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                                                                              && yc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
                            if (yeuCauKhamBenhs == null)
                            {
                                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                            }

                            foreach (var yeuCauKhamBenh in yeuCauKhamBenhs)
                            {
                                yeuCauKhamBenh.WillDelete = true;
                            }

                            var yeuCauDichVuKyThuats = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                                                                   && yc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
                            if (yeuCauDichVuKyThuats == null)
                            {
                                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                            }

                            foreach (var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
                            {
                                yeuCauDichVuKyThuat.WillDelete = true;
                            }
                            yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HuyKham = true;
                            await PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
                        }
                        else
                        {
                            hopDongKhamNhanVien.HuyKham = true;
                            _hopDongKhamSucKhoeNhanVienRepository.Update(hopDongKhamNhanVien);
                        }
                    }
                }
            }

            hopDongKham.DaKetThuc = true;
            _hopDongKhamSucKhoeRepository.Update(hopDongKham);
        }

        public async Task<GridDataSource> GetDanhSachPhongBenhVienGrid(QueryInfo queryInfo, long hopDongKhamSucKhoeId, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var hopDongKhamSucKhoes = hopDongKhamSucKhoeId != 0 ? _hopDongKhamSucKhoeRepository.TableNoTracking.Where(cc => cc.Id == hopDongKhamSucKhoeId)
                                                                                  .Include(c => c.PhongBenhViens) :
                                                                  _hopDongKhamSucKhoeRepository.TableNoTracking.Where(cc => cc.Id == hopDongKhamSucKhoeId)
                                                                                  .Include(c => c.PhongBenhViens);
            var query = hopDongKhamSucKhoes.SelectMany(c => c.PhongBenhViens)
                                           .Select(s => new DanhSachPhongKhamTaiCongTyGridVo
                                           {
                                               Id = s.Id,
                                               MaPhong = s.Ma,
                                               TenPhong = s.Ten,
                                               GhiChu = s.Tang,
                                               DanhSachNhanSu = s.KhoaPhongNhanViens.Select(c => c.NhanVienId).ToList()
                                           });


            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                                            g => g.MaPhong,
                                            g => g.TenPhong,
                                            g => g.GhiChu).OrderBy(queryInfo.SortString);

            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDanhSachPhongBenhVienGrid(QueryInfo queryInfo, long hopDongKhamSucKhoeId)
        {
            BuildDefaultSortExpression(queryInfo);
            var hopDongKhamSucKhoes = hopDongKhamSucKhoeId != 0 ? _hopDongKhamSucKhoeRepository.TableNoTracking.Where(cc => cc.Id == hopDongKhamSucKhoeId)
                                                                                  .Include(c => c.PhongBenhViens) :
                                                                  _hopDongKhamSucKhoeRepository.TableNoTracking.Where(cc => cc.Id == hopDongKhamSucKhoeId)
                                                                                  .Include(c => c.PhongBenhViens);
            var query = hopDongKhamSucKhoes.SelectMany(c => c.PhongBenhViens)
                                          .Select(s => new DanhSachPhongKhamTaiCongTyGridVo
                                          {
                                              Id = s.Id,
                                              MaPhong = s.Ma,
                                              TenPhong = s.Ten,
                                              GhiChu = s.Tang
                                          });


            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                                            g => g.MaPhong,
                                            g => g.TenPhong,
                                            g => g.GhiChu).OrderBy(queryInfo.SortString);

            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public (bool, string) XoaHopDongKham(long hopDongKhamSucKhoeId)
        {

            var hopDongSucKhoe = _hopDongKhamSucKhoeRepository.Table.Where(cc => cc.Id == hopDongKhamSucKhoeId)
                                                              .Include(a => a.HopDongKhamSucKhoeNhanViens)
                                                              .Include(a => a.HopDongKhamSucKhoeDiaDiems)
                                                              .Include(a => a.GoiKhamSucKhoes).ThenInclude(a => a.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(p => p.GoiKhamSucKhoeNoiThucHiens)
                                                              .Include(a => a.GoiKhamSucKhoes).ThenInclude(a => a.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(p => p.GoiKhamSucKhoeNoiThucHiens)
                                                              .Include(cc => cc.YeuCauNhanSuKhamSucKhoes)
                                                              .FirstOrDefault();

            if (hopDongSucKhoe.YeuCauNhanSuKhamSucKhoes.Any())
            {
                return (true, _localizationService.GetResource("HopDongKhamSucKhoe.YeuCauNhanSuKhamSucKhoe.Required"));
            }
            else
            {
                _hopDongKhamSucKhoeRepository.Delete(hopDongSucKhoe);
                return (false, "");
            }
        }

        public bool CapNhatSoLuongNhanVienKhamTrongHopDong(long hopDongKhamSucKhoeId)
        {
            var hopDongKham = _hopDongKhamSucKhoeRepository.TableNoTracking.FirstOrDefault(c => c.Id == hopDongKhamSucKhoeId);
            var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                                                                                  .Where(cc => cc.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId).Count();
            hopDongKham.SoNguoiKham = hopDongKhamSucKhoeNhanVien;
            _hopDongKhamSucKhoeRepository.Update(hopDongKham);

            return true;
        }

        public bool XoaHopDongKhamSucKhoeNhanVien(long hopDongKhamSucKhoeId)
        {
            var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                                                                                  .Where(cc => cc.Id == hopDongKhamSucKhoeId)
                                                                                  .FirstOrDefault();
            if (hopDongKhamSucKhoeNhanVien != null)
            {
                _hopDongKhamSucKhoeNhanVienRepository.Delete(hopDongKhamSucKhoeNhanVien);
                return true;
            }
            return false;
        }

        public async Task<bool> XoaTatCaNhanVienChuaKham(long hopDongKhamSucKhoeId)
        {
            var nhanVienChuaTaoYCTNTuKhamDoans = _hopDongKhamSucKhoeNhanVienRepository.Table
                .Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && !c.YeuCauTiepNhans.Any())
                .ToList();
            var isSuccessNhanVien = nhanVienChuaTaoYCTNTuKhamDoans.Any();
            if(nhanVienChuaTaoYCTNTuKhamDoans.Any())
            {
                foreach (var hopDongKhamNhanVien in nhanVienChuaTaoYCTNTuKhamDoans)
                {
                    hopDongKhamNhanVien.WillDelete = true;
                }
                _hopDongKhamSucKhoeNhanVienRepository.Context.SaveChanges();
            }
            return isSuccessNhanVien;

            //var khamTatCaNhanViens = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
            //                                                            .Where(c => c.YeuCauTiepNhans.Any(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
            //                                                                       && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat
            //                                                                       && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy));

            //var nhanVienChuaTaoYCTNTuKhamDoans = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId);

            //bool isSuccessNhanVien = false;
            //if (khamTatCaNhanViens.Any())
            //{
            //    foreach (var hopDongKhamNhanVien in khamTatCaNhanViens)
            //    {
            //        var yeuCauTiepNhan = hopDongKhamNhanVien.YeuCauTiepNhans.OrderByDescending(c => c.Id).FirstOrDefault();

            //        if (yeuCauTiepNhan != null)
            //        {
            //            var yeuCauKhamBenhs = yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham
            //                                                                              && yc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
            //            if (yeuCauKhamBenhs == null)
            //            {
            //                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            //            }

            //            foreach (var yeuCauKhamBenh in yeuCauKhamBenhs)
            //            {
            //                yeuCauKhamBenh.WillDelete = true;
            //            }

            //            var yeuCauDichVuKyThuats = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
            //                                                                                   && yc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
            //            if (yeuCauDichVuKyThuats == null)
            //            {
            //                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            //            }

            //            foreach (var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
            //            {
            //                yeuCauDichVuKyThuat.WillDelete = true;
            //            }
            //            yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.WillDelete = true;
            //            await PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
            //            isSuccessNhanVien = true;
            //        }
            //        else
            //        {
            //            _hopDongKhamSucKhoeNhanVienRepository.Delete(hopDongKhamNhanVien);
            //            isSuccessNhanVien = true;
            //        }
            //    }

            //}

            //if (nhanVienChuaTaoYCTNTuKhamDoans.Any())
            //{
            //    foreach (var hopDongKhamNhanVien in nhanVienChuaTaoYCTNTuKhamDoans)
            //    {
            //        hopDongKhamNhanVien.WillDelete = true;
            //    }

            //    await _hopDongKhamSucKhoeNhanVienRepository.DeleteAsync(nhanVienChuaTaoYCTNTuKhamDoans);
            //    isSuccessNhanVien = true;
            //}
            //return isSuccessNhanVien;
        }

        public bool XoaPhongKhamTaiCongTy(long id)
        {
            var phongKhamTaiCongTy = _phongBenhVienRepository.TableNoTracking
                                                             .Where(cc => cc.Id == id)
                                                             .Include(a => a.KhoaPhongNhanViens)
                                                             .FirstOrDefault();
            if (phongKhamTaiCongTy != null)
            {
                _phongBenhVienRepository.Delete(phongKhamTaiCongTy);
                return true;
            }
            return false;
        }

        public async Task<GridDataSource> GetDataForGridAsyncBangKeDichVuKhamDoan(QueryInfo queryInfo, bool exportExcel = false)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var hopDongKhamSucKhoeId = long.Parse(queryInfo.AdditionalSearchString);
            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                          .Where(p => p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                         .Select(s => new BaoCaoBangKeDichVuHopDongKSKVo
                         {
                             Id = s.Id,
                             YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
                             GoiKhamSucKhoeId = s.GoiKhamSucKhoeId,
                             BenhNhanId = s.BenhNhanId ?? 0,
                             MaBN = s.BenhNhan.MaBN,
                             HoTen = s.HoTen,
                             GioiTinh = s.GioiTinh.GetDescription(),
                             NamSinh = s.NamSinh,
                             CMND = s.SoChungMinhThu,
                             DonGiaUuDaiKhamBenhTrongGoi = s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.Sum(p => p.DonGiaUuDai),
                             TongChiPhiTrongGoiKhamBenh = 0,
                             TongChiPhiTrongGoiDichVuKyThuat = 0,
                             TongChiPhiNgoaiGoiGoiKhamBenh = 0,
                             TongChiPhiNgoaiGoiDichVuKyThuat = 0,
                             YeuCauTiepNhanVos = s.YeuCauTiepNhans.Select(tn => new BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanVo
                             {
                                 YeuCauTiepNhanId = tn.Id,
                                 YeuCauKhamBenhVos = tn.YeuCauKhamBenhs.Select(k => new BaoCaoBangKeYeuCauKhamBenhKSKVo
                                 {
                                     YeuCauKhamBenhId = k.Id,
                                     GoiKhamSucKhoeId = k.GoiKhamSucKhoeId,
                                     TrangThai = k.TrangThai,
                                     DonGiaUuDai = k.DonGiaUuDai,
                                     Gia = k.Gia,
                                     SoLuong = 1,
                                 }).ToList(),
                                 YeuCauDichVuKyThuatVos = tn.YeuCauDichVuKyThuats.Select(k => new BaoCaoBangKeYeuCauDichVuKyThuatKSKVo
                                 {
                                     YeuCauDichVuKyThuatId = k.Id,
                                     GoiKhamSucKhoeId = k.GoiKhamSucKhoeId,
                                     TrangThai = k.TrangThai,
                                     DonGiaUuDai = k.DonGiaUuDai,
                                     LoaiDichVuKyThuat = k.LoaiDichVuKyThuat,
                                     Gia = k.Gia,
                                     SoLuong = k.SoLan,
                                     CoTiepNhanMau = k.PhienXetNghiemChiTiets.Any(c => c.ThoiDiemNhanMau != null)
                                 }).ToList(),
                             }).OrderBy(tn => tn.YeuCauTiepNhanId).ToList(),

                             //GoiKhamSucKhoeYeuCauDichVuKyThuats = s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.Select(k => new BaoCaoBangKeDichVuKyThuatBenhVienKSKVo
                             //{
                             //    GoiKhamSucKhoeId = k.GoiKhamSucKhoeId,
                             //    DichVuKyThuatBenhVienId = k.DichVuKyThuatBenhVienId,
                             //    NhomGiaDichVuKyThuatBenhVienId = k.NhomGiaDichVuKyThuatBenhVienId,
                             //    NhomDichVuBenhVienId = k.DichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                             //    LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(k.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                             //    CoTiepNhanMau = CalculateHelper.GetLoaiDichVuKyThuat(k.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.XetNghiem && k.DichVuKyThuatBenhVien.PhienXetNghiemChiTiets.Any(c => c.ThoiDiemNhanMau != null && c.PhienXetNghiem.BenhNhanId == s.BenhNhanId),
                             //    DonGiaUuDai = k.DonGiaUuDai,
                             //    SoLuong = k.SoLan
                             //}).ToList(),
                             GoiKhamSucKhoeYeuCauKhams = s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.Select(k => new BaoCaoBangKeDichVuKhamBenhKSKVo
                             {
                                 GoiKhamSucKhoeId = k.GoiKhamSucKhoeId
                             }).ToList()
                         });

            var queryResult = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            var result = new List<BaoCaoBangKeDichVuHopDongKSKVo>();
            if (queryResult.Any())
            {
                foreach (var item in queryResult)
                {
                    if (item.YeuCauTiepNhanVos.Any())
                    {
                        var yeuCauTiepNhan = item.YeuCauTiepNhanVos.Last();
                        item.GoiKhamSucKhoeIds = yeuCauTiepNhan.YeuCauKhamBenhVos.Select(c => c.GoiKhamSucKhoeId)
                                         .Concat(yeuCauTiepNhan.YeuCauDichVuKyThuatVos.Select(c => c.GoiKhamSucKhoeId)).Distinct().ToList();
                        item.TongChiPhiTrongGoiKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhVos
                                               .Where(z => z.GoiKhamSucKhoeId != null
                                                           //&& z.GoiKhamSucKhoeId == item.GoiKhamSucKhoeId
                                                           && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                                           && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                                           .Sum(p => p.DonGiaUuDai.GetValueOrDefault());

                        item.TongChiPhiNgoaiGoiGoiKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhVos
                                                .Where(z => z.GoiKhamSucKhoeId == null
                                                            && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                                            && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                                            .Sum(p => p.Gia);


                        item.TongChiPhiTrongGoiDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuatVos
                                                            .Where(z => z.GoiKhamSucKhoeId != null
                                                            //&& z.GoiKhamSucKhoeId == item.GoiKhamSucKhoeId
                                                            && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                            && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && z.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem)
                                                            .Sum(p => p.DonGiaUuDai.GetValueOrDefault() * p.SoLuong)
                                               + yeuCauTiepNhan.YeuCauDichVuKyThuatVos
                                                            .Where(z => z.GoiKhamSucKhoeId != null
                                                            && z.CoTiepNhanMau
                                                            //&& z.GoiKhamSucKhoeId == item.GoiKhamSucKhoeId
                                                            && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
                                                            .Sum(p => p.DonGiaUuDai.GetValueOrDefault() * p.SoLuong)
                                                            ;
                        item.TongChiPhiNgoaiGoiDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuatVos
                                                            .Where(z => z.GoiKhamSucKhoeId == null
                                                            && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                            && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && z.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem)
                                                            .Sum(p => (p.DonGiaUuDai != null ? p.DonGiaUuDai.GetValueOrDefault() : p.Gia) * p.SoLuong)
                                               + yeuCauTiepNhan.YeuCauDichVuKyThuatVos
                                                            .Where(z => z.GoiKhamSucKhoeId == null
                                                            && z.CoTiepNhanMau
                                                            && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
                                                            //.Sum(p => p.DonGiaUuDai.GetValueOrDefault() * p.SoLuong)
                                                            .Sum(p => (p.DonGiaUuDai != null ? p.DonGiaUuDai.GetValueOrDefault() : p.Gia) * p.SoLuong)
                                                            ;
                    }
                    else
                    {
                        //item.GoiKhamSucKhoeIds = item.GoiKhamSucKhoeYeuCauKhams.Select(c => c.GoiKhamSucKhoeId).Concat(item.GoiKhamSucKhoeYeuCauDichVuKyThuats.Select(c => c.GoiKhamSucKhoeId)).Distinct().ToList();
                        item.TongChiPhiTrongGoiKhamBenh = item.DonGiaUuDaiKhamBenhTrongGoi;
                        item.TongChiPhiNgoaiGoiGoiKhamBenh = 0;
                        //item.TongChiPhiTrongGoiDichVuKyThuat = item.GoiKhamSucKhoeYeuCauDichVuKyThuats
                        //                                           .Where(c => c.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem || c.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && c.CoTiepNhanMau)
                        //                                           .Sum(c => c.DonGiaUuDai.GetValueOrDefault() * c.SoLuong);
                        item.TongChiPhiTrongGoiDichVuKyThuat = 0;
                        item.TongChiPhiNgoaiGoiDichVuKyThuat = 0;
                    }
                    result.Add(item);
                }

            }
            return new GridDataSource { Data = queryResult, TotalRowCount = queryResult.Count() };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncBangKeDichVuKhamDoan(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var hopDongKhamSucKhoeId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                        .Where(p => p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                           .Select(s => new BangKeDichVuGridVo
                           {
                               Id = s.Id,
                               //YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
                               //GoiKhamSucKhoeId = s.GoiKhamSucKhoeId,
                               //MaBN = s.BenhNhan.MaBN,
                               //HoTen = s.HoTen,
                               //GioiTinh = s.GioiTinh.GetDescription(),
                               //NamSinh = s.NamSinh,
                               //CMND = s.SoChungMinhThu,
                           });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridAsyncBangKeDichVuKhamDoanChiTiet(QueryInfo queryInfo, bool exportExcel = false)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var queryString = JsonConvert.DeserializeObject<DichVuKhamDoanChiTiet>(queryInfo.AdditionalSearchString);
            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
            if (queryString.LaDichVuTrongGoi)
            {
                if (queryString.YeuCauTiepNhanId > 0)
                {
                    var query = _yeuCauKhamBenhRepository.TableNoTracking
                                .Where(p => p.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId && p.GoiKhamSucKhoeId != null
                                && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)

                            .Select(s => new TongChiPhiTrongGridVo
                            {
                                Id = s.Id,
                                MaDichVu = s.DichVuKhamBenhBenhVien.Ma,
                                TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                                LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                                SoLuong = 1,
                                LaDichVuKham = true,
                                LoaiDichVuKyThuat = LoaiDichVuKyThuat.Khac,
                                DonGiaUuDai = s.DonGiaUuDai.GetValueOrDefault(),
                                NoiThucHien = s.NoiThucHien.Ten,
                                TrangThaiYeuCauKhamBenh = s.TrangThai
                            })
                            .Union(
                           _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(p => p.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId && p.GoiKhamSucKhoeId != null
                                //&& p.GoiKhamSucKhoeId == goiKhamSucKhoeId
                                && ((p.PhienXetNghiemChiTiets.Any(c => c.ThoiDiemNhanMau != null && c.PhienXetNghiem.BenhNhanId == queryString.BenhNhanId) && p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                || p.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                .Select(s => new TongChiPhiTrongGridVo
                                {
                                    Id = s.Id,
                                    MaDichVu = s.DichVuKyThuatBenhVien.Ma,
                                    TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                                    LoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                                    SoLuong = s.SoLan,
                                    LaDichVuKham = false,
                                    LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(s.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                                    DonGiaUuDai = s.DonGiaUuDai.GetValueOrDefault(),
                                    NoiThucHien = s.NoiThucHien.Ten,
                                    TrangThaiYeuCauDichVuKyThuat = s.TrangThai
                                }));

                    var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                    var queryTask = query.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
                else
                {
                    //var phienChitiet = _phienXetNghiemChiTietRepository.TableNoTracking.Where(xn => xn.ThoiDiemNhanMau != null && xn.YeuCauDichVuKyThuat.GoiKhamSucKhoeId == goiKhamSucKhoeId && xn.PhienXetNghiem.BenhNhanId == benhNhanId);
                    var query = _goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                             .Where(p => queryString.GoiKhamSucKhoeIds.Contains(p.GoiKhamSucKhoeId))
                             .Select(s => new TongChiPhiTrongGridVo
                             {
                                 Id = s.Id,
                                 MaDichVu = s.DichVuKhamBenhBenhVien.Ma,
                                 TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                                 LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                                 SoLuong = 1,
                                 LaDichVuKham = true,
                                 //NhomDichVuChiDinhKhamSucKhoe = NhomDichVuChiDinhKhamSucKhoe.KhamBenh,
                                 LoaiDichVuKyThuat = LoaiDichVuKyThuat.Khac,
                                 DonGiaUuDai = s.DonGiaUuDai,
                                 NoiThucHien = string.Join("; ", s.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten)),
                                 TrangThaiYeuCauKhamBenh = queryString.YeuCauTiepNhanId != 0 ? s.GoiKhamSucKhoe.YeuCauKhamBenhs.Where(p => queryString.GoiKhamSucKhoeIds.Contains(p.GoiKhamSucKhoeId) && p.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId && p.DichVuKhamBenhBenhVienId == s.DichVuKhamBenhBenhVienId).Select(z => z.TrangThai).FirstOrDefault() : EnumTrangThaiYeuCauKhamBenh.ChuaKham
                             })
                                 .Union(


                             _goiKhamSucKhoeDichVuDichVuKyThuatRepository.TableNoTracking
                                .Where(p => queryString.GoiKhamSucKhoeIds.Contains(p.GoiKhamSucKhoeId)
                                && CalculateHelper.GetLoaiDichVuKyThuat(p.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien) != LoaiDichVuKyThuat.XetNghiem
                                     )
                                     .Select(s => new TongChiPhiTrongGridVo
                                     {
                                         Id = s.Id,
                                         MaDichVu = s.DichVuKyThuatBenhVien.Ma,
                                         TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                                         LoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                                         SoLuong = s.SoLan,
                                         LaDichVuKham = false,
                                         LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(s.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                                         DonGiaUuDai = s.DonGiaUuDai,
                                         NoiThucHien = string.Join("; ", s.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten)),
                                         TrangThaiYeuCauDichVuKyThuat = queryString.YeuCauTiepNhanId != 0 ? s.GoiKhamSucKhoe.YeuCauDichVuKyThuats.Where(p => queryString.GoiKhamSucKhoeIds.Contains(p.GoiKhamSucKhoeId) && p.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId && p.DichVuKyThuatBenhVienId == s.DichVuKyThuatBenhVienId).Select(z => z.TrangThai).FirstOrDefault() : EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                     }
                                ).Union(
                                    _phienXetNghiemChiTietRepository.TableNoTracking
                                    .Where(xn => xn.ThoiDiemNhanMau != null && queryString.GoiKhamSucKhoeIds.Contains(xn.YeuCauDichVuKyThuat.GoiKhamSucKhoeId) && xn.PhienXetNghiem.BenhNhanId == queryString.BenhNhanId && xn.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                    .Select(s => new TongChiPhiTrongGridVo
                                    {
                                        Id = s.Id,
                                        MaDichVu = s.DichVuKyThuatBenhVien.Ma,
                                        TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                                        LoaiGia = s.YeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVien.Ten,
                                        SoLuong = s.YeuCauDichVuKyThuat.SoLan,
                                        LaDichVuKham = false,
                                        LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(s.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                                        DonGiaUuDai = s.YeuCauDichVuKyThuat.DonGiaUuDai.GetValueOrDefault(),
                                        NoiThucHien = string.Join("; ", s.DichVuKyThuatBenhVien.GoiKhamSucKhoeDichVuDichVuKyThuats.SelectMany(c => c.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten))),
                                        TrangThaiYeuCauDichVuKyThuat = queryString.YeuCauTiepNhanId != 0 ? s.YeuCauDichVuKyThuat.GoiKhamSucKhoe.YeuCauDichVuKyThuats.Where(p => queryString.GoiKhamSucKhoeIds.Contains(p.GoiKhamSucKhoeId) && p.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId && p.DichVuKyThuatBenhVienId == s.DichVuKyThuatBenhVienId).Select(z => z.TrangThai).FirstOrDefault() : EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                    })
                                 )
                             )
                             ;
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                    var queryTask = query.ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
            }
            else
            {
                var query = _yeuCauKhamBenhRepository.TableNoTracking
                             .Where(p => p.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId && p.GoiKhamSucKhoeId == null
                             && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)

                         .Select(s => new TongChiPhiTrongGridVo
                         {
                             Id = s.Id,
                             MaDichVu = s.DichVuKhamBenhBenhVien.Ma,
                             TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                             LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                             SoLuong = 1,
                             LaDichVuKham = true,
                             LoaiDichVuKyThuat = LoaiDichVuKyThuat.Khac,
                             DonGiaUuDai = s.DonGiaUuDai == null ? s.Gia : s.DonGiaUuDai.GetValueOrDefault(),
                             NoiThucHien = s.NoiThucHien.Ten,
                             TrangThaiYeuCauKhamBenh = s.TrangThai
                         })
                         .Union(
                        _yeuCauDichVuKyThuatRepository.TableNoTracking
                             .Where(p => p.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId && p.GoiKhamSucKhoeId == null
                             //&& p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                && ((p.PhienXetNghiemChiTiets.Any(c => c.ThoiDiemNhanMau != null && c.PhienXetNghiem.BenhNhanId == queryString.BenhNhanId) && p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                || p.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                             .Select(s => new TongChiPhiTrongGridVo
                             {
                                 Id = s.Id,
                                 MaDichVu = s.DichVuKyThuatBenhVien.Ma,
                                 TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                                 LoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                                 SoLuong = s.SoLan,
                                 LaDichVuKham = false,
                                 LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(s.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                                 //DonGiaUuDai = s.DonGiaUuDai ?? 0,
                                 DonGiaUuDai = s.DonGiaUuDai == null ? s.Gia : s.DonGiaUuDai.GetValueOrDefault(),
                                 NoiThucHien = s.NoiThucHien.Ten,
                                 TrangThaiYeuCauDichVuKyThuat = s.TrangThai
                             }));

                var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                var queryTask = query.ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncBangKeDichVuKhamDoanChiTiet(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<string> GetTenCongTyTheoHopDong(long hopDongKhamSucKhoeId)
        {
            return await _hopDongKhamSucKhoeRepository.TableNoTracking.Where(p => p.Id == hopDongKhamSucKhoeId).Select(p => p.CongTyKhamSucKhoe.Ten).FirstAsync();
        }

        public List<ExportExcelNhanVienDichVuTrongGoi> ExportExcelNhanVienDichVuTrongGois(long hopDongKhamSucKhoeId)
        {
            var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
            var nhanVienDichVuTrongGois = _hopDongKhamSucKhoeNhanVienRepository
                                            .TableNoTracking.Where(p => p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                            .Select(s => new ExportExcelNhanVienDichVuTrongGoi
                                            {
                                                Id = s.Id,
                                                GoiKhamSucKhoeId = s.GoiKhamSucKhoeId,
                                                MaBN = s.BenhNhan.MaBN,
                                                HoTen = s.HoTen,
                                                NamSinh = s.NamSinh,
                                                GioiTinh = s.GioiTinh.GetDescription(),
                                                TenDonViHoacBoPhan = s.TenDonViHoacBoPhan,
                                                YeuCauTiepNhanVos = s.YeuCauTiepNhans.Select(tn => new BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanExcelVo
                                                {
                                                    YeuCauKhamBenhVos = tn.YeuCauKhamBenhs.Where(z => z.GoiKhamSucKhoeId != null && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(z => new DichVuTrongGoiCuaNhanVien
                                                    {
                                                        Ma = z.MaDichVu,
                                                        Ten = z.TenDichVu,
                                                        SoLuong = 1,
                                                        DonGiaUuDai = z.DonGiaUuDai ?? 0,
                                                        GoiKhamSucKhoeId = z.GoiKhamSucKhoeId
                                                    }).ToList(),
                                                    YeuCauDichVuKyThuatVos = tn.YeuCauDichVuKyThuats.Where(z => z.GoiKhamSucKhoeId != null && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                   .Select(x => new DichVuTrongGoiCuaNhanVien
                                                   {
                                                       Id = x.Id,
                                                       Ma = x.MaDichVu,
                                                       Ten = x.TenDichVu,
                                                       SoLuong = x.SoLan,
                                                       DonGiaUuDai = x.DonGiaUuDai ?? 0,
                                                       GoiKhamSucKhoeId = x.GoiKhamSucKhoeId,
                                                       LoaiDichVuKyThuat = x.LoaiDichVuKyThuat
                                                   }).ToList()
                                                }).ToList(),
                                                //DichVuTrongGoiCuaNhanViens = s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.Select(z => new DichVuTrongGoiCuaNhanVien
                                                //{
                                                //    Ma = z.DichVuKhamBenhBenhVien.Ma,
                                                //    Ten = z.DichVuKhamBenhBenhVien.Ten,
                                                //    SoLuong = 1,
                                                //    DonGiaUuDai = z.DonGiaUuDai,
                                                //}).Union(s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.Select(z => new DichVuTrongGoiCuaNhanVien
                                                //{
                                                //    Ma = z.DichVuKyThuatBenhVien.Ma,
                                                //    Ten = z.DichVuKyThuatBenhVien.Ten,
                                                //    SoLuong = z.SoLan,
                                                //    DonGiaUuDai = z.DonGiaUuDai,
                                                //    LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(z.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                                                //})).OrderBy(z => z.Ten).ToList(),
                                            }).ToList();
            var dataReturn = new List<ExportExcelNhanVienDichVuTrongGoi>();
            var yeuCauDichVuKyThuatIds = nhanVienDichVuTrongGois
                             .SelectMany(nv => nv.YeuCauTiepNhanVos)
                             .SelectMany(tn => tn.YeuCauDichVuKyThuatVos
                             .Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
                             .Select(kt => kt.Id)).ToList();
            var yeuCauDichVuKyThuatTheoPhienXNChiTiets = new List<long>();
            var maxValue = 20000;
            var yeuCauDichVuKyThuatCount = yeuCauDichVuKyThuatIds.Count();
            //double maxIndex = Math.Ceiling(yeuCauDichVuKyThuatCount * 1.0 / maxValue);
            for (int i = 0; i < Math.Ceiling(yeuCauDichVuKyThuatCount * 1.0 / maxValue); i++)
            {
                var yeuCauDichVuKyThuatIdTakeValues = yeuCauDichVuKyThuatIds.Skip(i * maxValue).Take(maxValue).ToList();
                var yeuCauDichVuKyThuatTheoPhienXNChiTietTakeValues = _phienXetNghiemChiTietRepository.TableNoTracking.Where(xn => yeuCauDichVuKyThuatIdTakeValues.Contains(xn.YeuCauDichVuKyThuatId) && xn.ThoiDiemNhanMau != null).Select(c => c.YeuCauDichVuKyThuatId).Distinct().ToList();
                yeuCauDichVuKyThuatTheoPhienXNChiTiets.AddRange(yeuCauDichVuKyThuatTheoPhienXNChiTietTakeValues);
            }

            foreach (var item in nhanVienDichVuTrongGois)
            {
                if (item.YeuCauTiepNhanVos.Any())
                {
                    var yeuCauTiepNhan = item.YeuCauTiepNhanVos.Last();
                    var yeuCauDichVuKyThuatVoXns = yeuCauTiepNhan.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem).ToList();
                    var yeuCauDichVuKyThuatVoXnResults = new List<DichVuTrongGoiCuaNhanVien>();
                    foreach (var yeuCauDichVuKyThuatVo in yeuCauDichVuKyThuatVoXns)
                    {
                        yeuCauDichVuKyThuatVo.CoTiepNhanMau = yeuCauDichVuKyThuatTheoPhienXNChiTiets.Distinct().Contains(yeuCauDichVuKyThuatVo.Id);
                        yeuCauDichVuKyThuatVo.DonGiaUuDai = yeuCauDichVuKyThuatVo.CoTiepNhanMau ? yeuCauDichVuKyThuatVo.DonGiaUuDai : 0;
                        yeuCauDichVuKyThuatVoXnResults.Add(yeuCauDichVuKyThuatVo);
                    }
                    //x.GoiKhamSucKhoeId == item.GoiKhamSucKhoeId &&
                    item.DichVuTrongGoiCuaNhanViens = yeuCauTiepNhan.YeuCauKhamBenhVos
                                          .Concat(yeuCauTiepNhan.YeuCauDichVuKyThuatVos.Where(x => x.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem)
                                          .Concat(yeuCauDichVuKyThuatVoXnResults)).ToList();
                    item.MaVaTenDichVuTrongGoiCuaNhanViens = yeuCauTiepNhan.YeuCauKhamBenhVos
                                                        .Select(s => new MaVaTenDichVuTrongGoiCuaNhanVien { Ma = s.Ma, Ten = s.Ten, CoTiepNhanMau = s.CoTiepNhanMau, LoaiDichVuKyThuat = s.LoaiDichVuKyThuat })
                                                .Concat(yeuCauTiepNhan.YeuCauDichVuKyThuatVos
                                                        .Select(s => new MaVaTenDichVuTrongGoiCuaNhanVien { Ma = s.Ma, Ten = s.Ten, CoTiepNhanMau = s.CoTiepNhanMau, LoaiDichVuKyThuat = s.LoaiDichVuKyThuat })).ToList();
                }
                else
                {
                    item.MaVaTenDichVuTrongGoiCuaNhanViens = new List<MaVaTenDichVuTrongGoiCuaNhanVien>();
                }
                dataReturn.Add(item);
            }
            return dataReturn.OrderBy(c => c.Id).ToList();
        }

        public List<ExportExcelNhanVienDichVuTrongGoi> ExportExcelNhanVienDichVuNgoaiGois(long hopDongKhamSucKhoeId)
        {
            var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
            var nhanVienDichVuNgoaiGois = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(p => p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                            .Select(s => new ExportExcelNhanVienDichVuTrongGoi
                                            {
                                                Id = s.Id,
                                                MaBN = s.BenhNhan.MaBN,
                                                HoTen = s.HoTen,
                                                NamSinh = s.NamSinh,
                                                GioiTinh = s.GioiTinh.GetDescription(),
                                                TenDonViHoacBoPhan = s.TenDonViHoacBoPhan,
                                                YeuCauTiepNhanVos = s.YeuCauTiepNhans.Select(tn => new BaoCaoBangKeDichVuHopDongKSKYeuCauTiepNhanExcelVo
                                                {
                                                    YeuCauKhamBenhVos = tn.YeuCauKhamBenhs.Where(z => z.GoiKhamSucKhoeId == null && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(z => new DichVuTrongGoiCuaNhanVien
                                                    {
                                                        Ma = z.MaDichVu,
                                                        Ten = z.TenDichVu,
                                                        SoLuong = 1,
                                                        DonGiaUuDai = z.Gia,
                                                    }).ToList(),
                                                    YeuCauDichVuKyThuatVos = tn.YeuCauDichVuKyThuats.Where(z => z.GoiKhamSucKhoeId == null && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                    .Select(x => new DichVuTrongGoiCuaNhanVien
                                                    {
                                                        Id = x.Id,
                                                        Ma = x.MaDichVu,
                                                        Ten = x.TenDichVu,
                                                        SoLuong = x.SoLan,
                                                        DonGiaUuDai = x.Gia,
                                                        //CoTiepNhanMau = x.PhienXetNghiemChiTiets.Any(c => c.ThoiDiemNhanMau != null),
                                                        LoaiDichVuKyThuat = x.LoaiDichVuKyThuat
                                                    }).ToList()
                                                }).ToList()
                                            }).ToList();
            var dataReturn = new List<ExportExcelNhanVienDichVuTrongGoi>();
            var yeuCauDichVuKyThuatIds = nhanVienDichVuNgoaiGois.SelectMany(nv => nv.YeuCauTiepNhanVos).SelectMany(tn => tn.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem).Select(kt => kt.Id)).ToList();
            var yeuCauDichVuKyThuatTheoPhienXNChiTiets = new List<long>();
            var maxValue = 20000;
            var yeuCauDichVuKyThuatCount = yeuCauDichVuKyThuatIds.Count();
            for (int i = 0; i < Math.Ceiling(yeuCauDichVuKyThuatCount * 1.0 / maxValue); i++)
            {
                var yeuCauDichVuKyThuatIdTakeValues = yeuCauDichVuKyThuatIds.Skip(i * maxValue).Take(maxValue).ToList();
                var yeuCauDichVuKyThuatTheoPhienXNChiTietTakeValues = _phienXetNghiemChiTietRepository.TableNoTracking.Where(xn => yeuCauDichVuKyThuatIdTakeValues.Contains(xn.YeuCauDichVuKyThuatId) && xn.ThoiDiemNhanMau != null).Select(c => c.YeuCauDichVuKyThuatId).Distinct().ToList();
                yeuCauDichVuKyThuatTheoPhienXNChiTiets.AddRange(yeuCauDichVuKyThuatTheoPhienXNChiTietTakeValues);
            }
            foreach (var item in nhanVienDichVuNgoaiGois)
            {
                if (item.YeuCauTiepNhanVos.Any())
                {
                    var yeuCauTiepNhan = item.YeuCauTiepNhanVos.Last();
                    var yeuCauDichVuKyThuatVoXns = yeuCauTiepNhan.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem).ToList();
                    var yeuCauDichVuKyThuatVoXnResults = new List<DichVuTrongGoiCuaNhanVien>();
                    foreach (var yeuCauDichVuKyThuatVo in yeuCauDichVuKyThuatVoXns)
                    {
                        yeuCauDichVuKyThuatVo.CoTiepNhanMau = yeuCauDichVuKyThuatTheoPhienXNChiTiets.Distinct().Contains(yeuCauDichVuKyThuatVo.Id);
                        yeuCauDichVuKyThuatVo.DonGiaUuDai = yeuCauDichVuKyThuatVo.CoTiepNhanMau ? yeuCauDichVuKyThuatVo.DonGiaUuDai : 0;
                        yeuCauDichVuKyThuatVoXnResults.Add(yeuCauDichVuKyThuatVo);
                    }
                    item.DichVuTrongGoiCuaNhanViens = yeuCauTiepNhan.YeuCauKhamBenhVos.Concat(yeuCauTiepNhan.YeuCauDichVuKyThuatVos.Where(x => x.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem).Concat(yeuCauDichVuKyThuatVoXnResults)).ToList();
                    item.MaVaTenDichVuTrongGoiCuaNhanViens = yeuCauTiepNhan.YeuCauKhamBenhVos.Select(s => new MaVaTenDichVuTrongGoiCuaNhanVien { Ma = s.Ma, Ten = s.Ten })
                                                .Concat(yeuCauTiepNhan.YeuCauDichVuKyThuatVos.Select(s => new MaVaTenDichVuTrongGoiCuaNhanVien { Ma = s.Ma, Ten = s.Ten, CoTiepNhanMau = s.CoTiepNhanMau, LoaiDichVuKyThuat = s.LoaiDichVuKyThuat })).ToList();
                }
                else
                {
                    item.DichVuTrongGoiCuaNhanViens = new List<DichVuTrongGoiCuaNhanVien>();
                    item.MaVaTenDichVuTrongGoiCuaNhanViens = new List<MaVaTenDichVuTrongGoiCuaNhanVien>();
                }
                dataReturn.Add(item);
            }
            return dataReturn.OrderBy(c => c.Id).ToList();
        }

        public async Task<bool> KiemTraTrungMaNhanVien(long id, long hopDongKhamSucKhoeId, string maNhanVien)
        {
            bool result;
            if (id == 0)
            {
                result = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && c.MaNhanVien.ToUpper() == maNhanVien.ToUpper()).AnyAsync();
            }
            else
            {
                result = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && c.MaNhanVien.ToUpper() == maNhanVien.ToUpper() && c.Id != id).AnyAsync();
            }
            return !result;
        }

        public async Task<bool> KiemTraTrungSTTTrongHopDongKham(long id, long hopDongKhamSucKhoeId, int? sttNhanVien)
        {
            bool result;
            if (id == 0)
            {
                result = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && c.STTNhanVien == sttNhanVien).AnyAsync();
            }
            else
            {
                result = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && c.STTNhanVien == sttNhanVien && c.Id != id).AnyAsync();
            }
            return !result;
        }

        public async Task<bool> KiemTraTrungSoChungMinhThu(long id, long hopDongKhamSucKhoeId, string soChungMinhThu)
        {
            bool result;
            if (id == 0)
            {
                result = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && c.SoChungMinhThu.ToUpper() == soChungMinhThu.ToUpper()).AnyAsync();
            }
            else
            {
                result = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId && c.SoChungMinhThu.ToUpper() == soChungMinhThu.ToUpper() && c.Id != id).AnyAsync();
            }
            return !result;
        }

        public virtual byte[] ExportDichVuKhamDoanChiTiets(ICollection<ExportExcelNhanVienDichVuTrongGoi> dichVuKhamDoanChiTiets, string tenCongTy)
        {
            int index = 4;
            var requestProperties = new[]
            {
                new PropertyByName<ExportExcelNhanVienDichVuTrongGoi>("STT", p => index++)
            };
            //Tổng hợp dịch vụ của nhân viên theo gói
            var maVaTenDichVuTrongGoiCuaNhanViens = new List<MaVaTenDichVuTrongGoiCuaNhanVien>();
            foreach (var dichVuKhamDoanChiTiet in dichVuKhamDoanChiTiets)
            {
                maVaTenDichVuTrongGoiCuaNhanViens.AddRange(dichVuKhamDoanChiTiet.MaVaTenDichVuTrongGoiCuaNhanViens);
            }
            var tatCaDichVuNews = new List<MaVaTenDichVuTrongGoiCuaNhanVien>();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("TongHopChiPhiKetQuaKSKTheoHopDong");
                    var tongSoDichVu = dichVuKhamDoanChiTiets.Count;
                    worksheet.DefaultRowHeight = 16;
                    // SET  dịch vụ động
                    var tatCaDichVus = maVaTenDichVuTrongGoiCuaNhanViens
                        .GroupBy(g => new
                        {
                            g.Ma,
                            g.Ten
                        }).Select(g => g.First()).OrderBy(z => z.Ma).ThenBy(z => z.Ten).ToList();
                    var tongDichVus = tatCaDichVus.Count();// + thêm 6  columns đầu
                    var danhSachKyTuXuLy = new List<string>();
                    var kyTuCuoiCung = "G";

                    if (tongDichVus + 7 > 26)
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
                        worksheet.Column(i).Width = 15;
                        worksheet.Column(i + 5).Width = 15;
                        worksheet.Column(i + 6).Width = 15;
                    }



                    // Hiển thị các dịch vụ theo column
                    var indexDichVu = 3; // bắt đầu từ F3(dòng 3)

                    if (tongDichVus + 7 > 26)
                    {
                        danhSachKyTuXuLy = XuLyDanhSachKyTu(tongDichVus, 26);
                    }
                    else
                    {
                        danhSachKyTuXuLy = KyTus();
                    }
                    var indexdvKt = 6;
                    using (var range = worksheet.Cells["A3"])
                    {
                        range.Worksheet.Cells["A3"].Value = "STT";
                        range.Worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A3"].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells["A3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["B3"])
                    {
                        range.Worksheet.Cells["B3"].Value = "Mã NB";
                        range.Worksheet.Cells["B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["B3"].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells["B3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B3"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["C3"])
                    {
                        range.Worksheet.Cells["C3"].Value = "Họ Tên";
                        range.Worksheet.Cells["C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["C3"].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells["C3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C3"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["D3"])
                    {
                        range.Worksheet.Cells["D3"].Value = "NS";
                        range.Worksheet.Cells["D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D3"].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells["D3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D3"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["E3"])
                    {
                        range.Worksheet.Cells["E3"].Value = "GT";
                        range.Worksheet.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["E3"].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells["E3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E3"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["F3"])
                    {
                        range.Worksheet.Cells["F3"].Value = "Đơn vị";
                        range.Worksheet.Cells["F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["F3"].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells["F3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F3"].Style.Font.Bold = true;
                    }
                    foreach (var dv in tatCaDichVus)
                    {
                        for (int i = indexdvKt; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Value = dv.Ten;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + indexDichVu + ":" + danhSachKyTuXuLy[i] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Bold = true;
                            }
                            kyTuCuoiCung = danhSachKyTuXuLy[i + 1];
                            break;
                        }
                        indexdvKt++;
                    }
                    // Hiển thị chữ 'tổng cộng' ở cột sau dịch vụ cuối cùng
                    using (var range = worksheet.Cells[kyTuCuoiCung + indexDichVu])
                    {
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Value = "Tổng cộng";
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Font.Bold = true;
                    }

                    // SET data
                    indexDichVu++; // Tăng số dòng đổ data
                    var manager = new PropertyManager<ExportExcelNhanVienDichVuTrongGoi>(requestProperties);
                    var STT = 0;
                    foreach (var data in dichVuKhamDoanChiTiets)
                    {
                        STT++;
                        manager.CurrentObject = data;
                        var indexKyTu = 0;
                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Value = STT;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexKyTu++;
                            break;
                        }

                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Value = data.MaBN;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexKyTu++;
                            break;
                        }

                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Value = data.HoTen;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexKyTu++;
                            break;
                        }

                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Value = data.NamSinh;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexKyTu++;
                            break;
                        }

                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Value = data.GioiTinh;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexKyTu++;
                            break;
                        }

                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Value = data.TenDonViHoacBoPhan;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexKyTu++;
                            break;
                        }

                        //SET DATA NGANG
                        decimal sumThanhTien = 0;
                        tatCaDichVuNews = new List<MaVaTenDichVuTrongGoiCuaNhanVien>();
                        foreach (var dv in tatCaDichVus)
                        {
                            var dvTrongGoi = data.DichVuTrongGoiCuaNhanViens.FirstOrDefault(x => x.Ma == dv.Ma && x.Ten == dv.Ten);
                            if (dvTrongGoi != null)
                            {
                                var maVaTenDichVuTrongGoiCuaNhanVien = new MaVaTenDichVuTrongGoiCuaNhanVien()
                                {
                                    Ma = dv.Ma,
                                    Ten = dv.Ten,
                                    ThanhTien = dvTrongGoi.ThanhTien,
                                };
                                tatCaDichVuNews.Add(maVaTenDichVuTrongGoiCuaNhanVien);
                            }
                            else
                            {
                                var maVaTenDichVuTrongGoiCuaNhanVien = new MaVaTenDichVuTrongGoiCuaNhanVien()
                                {
                                    Ma = dv.Ma,
                                    Ten = dv.Ten,
                                    ThanhTien = 0,
                                };
                                tatCaDichVuNews.Add(maVaTenDichVuTrongGoiCuaNhanVien);
                            }
                        }

                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            foreach (var dv in tatCaDichVuNews.OrderBy(z => z.Ma).ThenBy(z => z.Ten).ToList())
                            {
                                using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + indexDichVu])
                                {
                                    range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + indexDichVu].Value = dv.ThanhTien;
                                    range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + indexDichVu].Style.Numberformat.Format = "#,##0.00";
                                    range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                    range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                }
                                sumThanhTien += dv.ThanhTien;
                                indexKyTu++;
                            }
                            kyTuCuoiCung = danhSachKyTuXuLy[indexKyTu];
                            break;
                        }

                        using (var range = worksheet.Cells[kyTuCuoiCung + indexDichVu])
                        {
                            range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Value = sumThanhTien;
                            range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Numberformat.Format = "#,##0.00";
                            range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                            range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexDichVu++;
                    }
                    //Tổng cộng
                    using (var range = worksheet.Cells["A" + indexDichVu + ":F" + indexDichVu])
                    {
                        range.Worksheet.Cells["A" + indexDichVu + ":F" + indexDichVu].Merge = true;
                        range.Worksheet.Cells["A" + indexDichVu + ":F" + indexDichVu].Value = "Tổng  cộng: ";
                        range.Worksheet.Cells["A" + indexDichVu + ":F" + indexDichVu].Style.Font.Bold = true;
                        range.Worksheet.Cells["A" + indexDichVu + ":F" + indexDichVu].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + indexDichVu + ":F" + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells["A" + indexDichVu + ":F" + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + indexDichVu + ":F" + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    var tongTienDichVuDocs = new List<MaVaThanhTienDichVuTrongGoiCuaNhanVien>();
                    tatCaDichVuNews = new List<MaVaTenDichVuTrongGoiCuaNhanVien>();

                    //SET DATA DỌC

                    foreach (var data in dichVuKhamDoanChiTiets)
                    {
                        foreach (var dv in tatCaDichVus)
                        {

                            var dvTrongGoi = data.DichVuTrongGoiCuaNhanViens.FirstOrDefault(x => x.Ma == dv.Ma && x.Ten == dv.Ten);
                            if (dvTrongGoi != null)
                            {
                                var maVaTenDichVuTrongGoiCuaNhanVien = new MaVaTenDichVuTrongGoiCuaNhanVien()
                                {
                                    Ma = dv.Ma,
                                    Ten = dv.Ten,
                                    ThanhTien = dvTrongGoi.ThanhTien,
                                };
                                tatCaDichVuNews.Add(maVaTenDichVuTrongGoiCuaNhanVien);
                            }
                            else
                            {
                                var maVaTenDichVuTrongGoiCuaNhanVien = new MaVaTenDichVuTrongGoiCuaNhanVien()
                                {
                                    Ma = dv.Ma,
                                    Ten = dv.Ten,
                                    ThanhTien = 0,
                                };
                                tatCaDichVuNews.Add(maVaTenDichVuTrongGoiCuaNhanVien);
                            }
                        }
                    }
                    decimal sumThanhTienDoc = 0;
                    var indexKyTuDoc = 6;
                    var tatCaDichVuNewSum = tatCaDichVuNews.GroupBy(x => new { x.Ma, x.Ten })
                        .Select(z => new MaVaTenDichVuTrongGoiCuaNhanVien
                        {
                            Ma = z.First().Ma,
                            Ten = z.First().Ten,
                            ThanhTien = z.Sum(v => v.ThanhTien)
                        }).OrderBy(z => z.Ma).ThenBy(z => z.Ten).ToList();
                    foreach (var dvTitle in tatCaDichVus)
                    {
                        for (int i = indexKyTuDoc; i < danhSachKyTuXuLy.Count();)
                        {
                            foreach (var dvSum in tatCaDichVuNewSum)
                            {
                                if (dvTitle.Ma == dvSum.Ma && dvTitle.Ten == dvSum.Ten)
                                {
                                    using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu])
                                    {
                                        range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Value = dvSum.ThanhTien;
                                        range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Numberformat.Format = "#,##0.00";
                                        range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                                        range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                                        range.Worksheet.Cells[danhSachKyTuXuLy[i] + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    }
                                    indexKyTuDoc++;
                                    sumThanhTienDoc += dvSum.ThanhTien;
                                    break;
                                }
                            }
                            kyTuCuoiCung = danhSachKyTuXuLy[i + 1];
                            break;
                        }
                    }

                    using (var range = worksheet.Cells[kyTuCuoiCung + indexDichVu])
                    {
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Value = sumThanhTienDoc;
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Numberformat.Format = "#,##0.00";
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[kyTuCuoiCung + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    // SET title head cho bảng excel

                    using (var range = worksheet.Cells["A1:" + kyTuCuoiCung + "1"])
                    {
                        range.Worksheet.Cells["A1:" + kyTuCuoiCung + "1"].Merge = true;
                        range.Worksheet.Cells["A1:" + kyTuCuoiCung + "1"].Value = "TỔNG HỢP CHI PHÍ KẾT QUẢ KHÁM SỨC KHOẺ THEO HỢP ĐỒNG";
                        range.Worksheet.Cells["A1:" + kyTuCuoiCung + "1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:" + kyTuCuoiCung + "1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:" + kyTuCuoiCung + "1"].Style.Font.SetFromFont(new Font("Arial", 18));
                        range.Worksheet.Cells["A1:" + kyTuCuoiCung + "1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:" + kyTuCuoiCung + "1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:" + kyTuCuoiCung + "2"])
                    {
                        range.Worksheet.Cells["A2:" + kyTuCuoiCung + "2"].Merge = true;
                        range.Worksheet.Cells["A2:" + kyTuCuoiCung + "2"].Value = tenCongTy.ToUpper();
                        range.Worksheet.Cells["A2:" + kyTuCuoiCung + "2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:" + kyTuCuoiCung + "2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:" + kyTuCuoiCung + "2"].Style.Font.SetFromFont(new Font("Arial", 14));
                        range.Worksheet.Cells["A2:" + kyTuCuoiCung + "2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:" + kyTuCuoiCung + "2"].Style.Font.Bold = true;
                    }

                    // END title head cho bảng excel

                    indexDichVu = indexDichVu + 2;
                    using (var range = worksheet.Cells["A" + indexDichVu])
                    {
                        range.Worksheet.Cells["A" + indexDichVu].Value = "Người lập(Ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["A" + indexDichVu].Style.WrapText = true;
                        range.Worksheet.Cells["A" + indexDichVu].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + indexDichVu].Style.Font.SetFromFont(new Font("Arial", 10));
                        range.Worksheet.Cells["A" + indexDichVu].Style.Font.Color.SetColor(Color.Black);
                        //range.Worksheet.Cells["A" + indexDichVu].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
        private List<string> XuLyDanhSachKyTu(int soDichVu, int tongSoKyTu)
        {
            var danhSachKyTuSauKhiXuKy = new List<string>();
            var danhSachKyTuPre = KyTus();
            var danhSachKyTuResult = KyTus();
            var soLanLap = (soDichVu / tongSoKyTu) + 1;
            for (int i = 0; i < soLanLap; i++)
            {
                foreach (var result in danhSachKyTuPre.Take(soLanLap))
                {
                    foreach (var item in danhSachKyTuResult)
                    {
                        danhSachKyTuSauKhiXuKy.Add(result + item);
                    }
                }
            }
            danhSachKyTuResult.AddRange(danhSachKyTuSauKhiXuKy);
            return danhSachKyTuResult;
        }
        private List<string> KyTus()
        {
            var kyTus = new List<string>()
                    { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };// 26 ký tự
            return kyTus;
        }
    }
}