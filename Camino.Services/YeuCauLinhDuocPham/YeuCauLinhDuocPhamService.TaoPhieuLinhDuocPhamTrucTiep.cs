using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System.Globalization;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial class YeuCauLinhDuocPhamService
    {

        public async Task<List<DanhSachLinhVeKhoGridVo>> GetListKhoLinhVe(DropDownListRequestModel model)
        {
            var lstkho = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Include(p => p.KhoLinh).Where(x => x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan).Distinct()
                .ApplyLike(model.Query, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = lstkho.Select(item => new
            {
                KeyId = item.KhoLinh.Id,
                DisplayName = item.KhoLinh.Ten,
                TenKhoLinh = item.KhoLinh.Ten,
                LoaiLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan.GetDescription()
            }).Distinct().ToList();
            var querys = query.Select(item => new DanhSachLinhVeKhoGridVo()
            {
                KeyId = item.KeyId,
                DisplayName = item.DisplayName,
                TenKhoLinh = item.TenKhoLinh,
                LoaiLinh = item.TenKhoLinh
            }).ToList();
            return querys;
        }
        public string TenKhoCho(long IdDuocPham)
        {
            var TenKho = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Include(p => p.KhoLinh).Where(x => x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan && x.DuocPhamBenhVienId == IdDuocPham).Select(x => x.KhoLinh.Ten);
            return EnumLoaiPhieuLinh.LinhChoBenhNhan.GetDescription() + " " + '-' + " " + TenKho;
        }
        public List<ThongTinLinhTuKhoGridVo> GetData(long idKhoLinh, long phongDangNhapId ,string dateSearchStart, string dateSearchEnd)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }


            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
         
            DateTime? tuNgay = null;
            DateTime? denNgay = null;

            if (!string.IsNullOrEmpty(dateSearchStart) && dateSearchStart != "null")
            {
                DateTime.TryParseExact(dateSearchStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgayTemp);
                //tuNgay = new DateTime(tuNgayTemp.Year, tuNgayTemp.Month, tuNgayTemp.Day, 0, 0, 0);
                tuNgay = tuNgayTemp;
            }
            if (!string.IsNullOrEmpty(dateSearchEnd) && dateSearchEnd != "null")
            {
                DateTime.TryParseExact(dateSearchEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgayTemp);
                //denNgay = new DateTime(denNgayTemp.Year, denNgayTemp.Month, denNgayTemp.Day, 23, 59, 59);
                denNgay = denNgayTemp;
            }
            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();

            var queryYeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Include(o => o.DuocPhamBenhVien)
                .ThenInclude(o => o.NhapKhoDuocPhamChiTiets).ThenInclude(p => p.NhapKhoDuocPhams).Include(o => o.KhoLinh)
                .Include(o => o.KhoLinh)
                .Where(x => x.KhoLinhId == idKhoLinh &&
                                                                                        x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                                                                        x.YeuCauLinhDuocPhamId == null &&
                                                                                        phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                                                        x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                        ).Select(d => d);


            var yeuCauDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.KhoLinhId == idKhoLinh &&
                                                                                        x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                                                                        x.YeuCauLinhDuocPhamId == null &&
                                                                                        phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                                                        x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                        )
                                     .Select(s => new ThongTinLinhTuKhoGridVo()
                                     {
                                         Id = s.YeuCauTiepNhanId,
                                         TenDuocPham = s.Ten,
                                         NongDoVaHamLuong = s.HamLuong,
                                         HoatChat = s.HoatChat,
                                         DuongDung = s.DuongDung.Ten,
                                         DonViTinh = s.DonViTinh.Ten,
                                         HangSX = s.NhaSanXuat,
                                         NuocSanXuat = s.NuocSanXuat,
                                         SLYeuCau = s.SoLuong,
                                         LoaiThuoc = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                         LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                                         DuocPhamId = s.DuocPhamBenhVienId,
                                         KhoLinhId = idKhoLinh,
                                         MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                         MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                                         HoTen = s.YeuCauTiepNhan.HoTen,
                                         YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                         LoaiDuocPham = s.LaDuocPhamBHYT,
                                         //SoLuongTon = s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoLinhId && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                         NgayDieuTri = s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh, // ngày đăng ký
                                         NgayYeuCau = s.ThoiDiemChiDinh,// ngày chỉ định
                                         IsCheckRowItem = false
                                     })
                                    .GroupBy(x => new
                                    {
                                        x.MaBN,
                                        x.HoTen,
                                        x.MaTN,
                                        x.YeuCauTiepNhanId,
                                        
                                    }).Select(s => new ThongTinLinhTuKhoGridVo()
                                    {
                                        Id = s.First().Id,
                                        TenDuocPham = s.First().TenDuocPham,
                                        NongDoVaHamLuong = s.First().NongDoVaHamLuong,
                                        HoatChat = s.First().HoatChat,
                                        DuongDung = s.First().DuongDung,
                                        DonViTinh = s.First().DonViTinh,
                                        HangSX = s.First().HangSX,
                                        NuocSanXuat = s.First().NuocSanXuat,
                                        SLYeuCau = s.Sum(x => x.SLYeuCau),
                                        LoaiThuoc = s.First().LoaiThuoc,
                                        LaDuocPhamBHYT = s.First().LaDuocPhamBHYT,
                                        DuocPhamId = s.First().DuocPhamId,
                                        KhoLinhId = idKhoLinh,
                                        MaTN = s.First().MaTN,
                                        MaBN = s.First().MaBN,
                                        HoTen = s.First().HoTen,
                                        YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                                        LoaiDuocPham = s.First().LoaiDuocPham,
                                        SoLuongTon = s.First().SoLuongTon,
                                        NgayYeuCau = s.First().NgayYeuCau,
                                        NgayDieuTris = s.Select(d=>d.NgayDieuTri).ToList(),
                                        IsCheckRowItem = s.First().IsCheckRowItem
                                    }).OrderBy(d => d.MaBN).ToList();
            var yeuCauDuocPhams = yeuCauDuocPham.Where(p => p.NgayDieuTris.Where(f => (tuNgay == null || f >= tuNgay) && (denNgay == null || f <= denNgay)).Any()).ToList();






            var dsMaYeuCauTiepNhan = yeuCauDuocPhams.Select(o => o.MaTN).ToList();
            if (dsMaYeuCauTiepNhan.Any())
            {
                var listAllYCDPBV = queryYeuCauDuocPhamBenhViens.Where(o => dsMaYeuCauTiepNhan.Contains(o.YeuCauTiepNhan.MaYeuCauTiepNhan)).Select(p => new 
                {
                    Id = p.Id,
                    MaTN = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    DVKham = p.YeuCauKhamBenhId != null ? p.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : p.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? p.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "Ghi nhận trong PTTT",
                    BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                    SLKe = (int)p.SoLuong,
                    NgayYeuCau = p.ThoiDiemChiDinh,
                    NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    NgayDieuTri = p.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && p.NoiTruPhieuDieuTri != null ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                    DuocDuyet = p.YeuCauLinhDuocPhamId == null ? false : p.YeuCauLinhDuocPham.DuocDuyet == true ? true : false,
                    DuocPhamId = p.DuocPhamBenhVienId,
                    TenDuocPham = p.Ten,
                    NongDoVaHamLuong = p.HamLuong,
                    HoatChat = p.HoatChat,
                    DuongDung = p.DuongDung.Ten,
                    DonViTinh = p.DonViTinh.Ten,
                    HangSX = p.NhaSanXuat,
                    NuocSanXuat = p.NuocSanXuat,
                    SLYeuCau = p.SoLuong,
                    LoaiThuoc = p.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                    SoLuongTon = p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == p.KhoLinhId && nkct.LaDuocPhamBHYT == p.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                    IsCheckRowItem = false,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    LaDuocPhamBHYT = p.LaDuocPhamBHYT,
                    KhoLinhId = idKhoLinh,
                }).Where(p => (tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay)).ToList();

                //var test = queryYeuCauDuocPhamBenhViens.Where(o => dsMaYeuCauTiepNhan.Contains(o.YeuCauTiepNhan.MaYeuCauTiepNhan))
                //          .Select(d => d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == d.KhoLinhId && nkct.LaDuocPhamBHYT == d.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now)
                //                     .Select(nkct => new
                //                     {
                //                         DuocPhamBenhVienId = nkct.DuocPhamBenhVienId,
                //                         KhoId = nkct.NhapKhoDuocPhams.KhoId,
                //                         LaDuocPhamBHYT = nkct.LaDuocPhamBHYT,
                //                         SoLuongNhap = nkct.SoLuongNhap,
                //                         SoLuongDaXuat =nkct.SoLuongDaXuat,
                //                         HanSuDung =nkct.HanSuDung
                //                     }).ToList());
                if (listAllYCDPBV != null)
                {
                    var dsDuocPhamBenhVienId = listAllYCDPBV.Select(o => o.DuocPhamId).Distinct().ToList();
                    var dsKhoId = listAllYCDPBV.Select(o => o.KhoLinhId).Distinct().ToList();

                    
                    var dsDuocPhamTrongKhoCanKiemTra = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(nkct => dsDuocPhamBenhVienId.Contains(nkct.DuocPhamBenhVienId) &&
                                 dsKhoId.Contains(nkct.NhapKhoDuocPhams.KhoId)  && nkct.HanSuDung >= DateTime.Now)
                        .Select(nkct => new { nkct.DuocPhamBenhVienId, nkct.NhapKhoDuocPhams.KhoId, nkct.LaDuocPhamBHYT, nkct.SoLuongNhap, nkct.SoLuongDaXuat,nkct.HanSuDung }).ToList();

                    foreach (var yeuCauTiepNhanCoLinhTrucTiep in yeuCauDuocPhams)
                    {
                        var listYCDPBV = listAllYCDPBV.Where(o => o.MaTN == yeuCauTiepNhanCoLinhTrucTiep.MaTN && o.KhoLinhId == yeuCauTiepNhanCoLinhTrucTiep.KhoLinhId).OrderByDescending(d=> d.SoLuongTon < d.SLKe ? 1 : 0).ThenBy(d=>d.TenDuocPham);

                        foreach (var yCDPBV in listYCDPBV)
                        {

                            var tonkho = dsDuocPhamTrongKhoCanKiemTra
                                .Where(nkct => nkct.DuocPhamBenhVienId == yCDPBV.DuocPhamId &&
                                     nkct.KhoId == yCDPBV.KhoLinhId && nkct.LaDuocPhamBHYT == yCDPBV.LaDuocPhamBHYT
                                     && nkct.HanSuDung >= DateTime.Now)
                                .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat);

                            yeuCauTiepNhanCoLinhTrucTiep.ListYeuCauDuocPhamBenhViens.Add(new ThongTinLanKhamKho
                            {
                                Id = yCDPBV.Id,
                                MaTN = yCDPBV.MaTN,
                                MaBN = yCDPBV.MaBN,
                                HoTen = yCDPBV.HoTen,
                                DVKham = yCDPBV.DVKham,
                                BacSyKeToa = yCDPBV.BacSyKeToa,
                                SLKe = yCDPBV.SLKe,
                                NgayYeuCau = yCDPBV.NgayYeuCau,
                                NgayKe = yCDPBV.NgayKe,
                                NgayDieuTri = yCDPBV.NgayDieuTri,
                                DuocDuyet = yCDPBV.DuocDuyet,
                                DuocPhamId = yCDPBV.DuocPhamId,
                                TenDuocPham = yCDPBV.TenDuocPham,
                                NongDoVaHamLuong = yCDPBV.NongDoVaHamLuong,
                                HoatChat = yCDPBV.HoatChat,
                                DuongDung = yCDPBV.DuongDung,
                                DonViTinh = yCDPBV.DonViTinh,
                                HangSX = yCDPBV.HangSX,
                                NuocSanXuat = yCDPBV.NuocSanXuat,
                                SLYeuCau = yCDPBV.SLYeuCau,
                                LoaiThuoc = yCDPBV.LoaiThuoc,
                                SoLuongTon = tonkho,
                                IsCheckRowItem = false,
                                YeuCauTiepNhanId = yCDPBV.YeuCauTiepNhanId,
                                LaDuocPhamBHYT = yCDPBV.LaDuocPhamBHYT,
                            });

                        }

                    }
                }
            }

            return yeuCauDuocPhams;
        }
        public  List<ThongTinLinhTuKhoGridVo> GetDataDaTao(long idKhoLinh, long idYeuCauLinhDuocPham, long phongDangNhapId, long trangThai)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();

            // chắc chắn có yêu cầu lĩnh vì view  chi tiết những yêu cầu đã tạo
            var kiemTraTrangThaiDuyetCuaPhieuLinh = _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(d => d.Id == idYeuCauLinhDuocPham).Select(d => d.DuocDuyet).First();





            if(kiemTraTrangThaiDuyetCuaPhieuLinh == true) // trạng thái đã duyệt
            {
                var yeuCauLinhDuocPhamKhoXuatId =
                _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(x => x.Id == idYeuCauLinhDuocPham).Select(s => s.KhoXuatId).First();

                var yeuCauDuocPham = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(o=>o.YeuCauLinhDuocPhamId == idYeuCauLinhDuocPham
                                        && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                        && o.YeuCauDuocPhamBenhVien.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                        && o.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhanId != null)
                    .Select(s => new ThongTinLinhTuKhoGridVo()
                 {
                     Id = s.Id,
                     TenDuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                     NongDoVaHamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                     HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                     DuongDung = s.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                     DonViTinh = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                     HangSX = s.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                     NuocSanXuat = s.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                     SLYeuCau = s.SoLuong,
                     LoaiThuoc = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                     LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                     DuocPhamId = s.DuocPhamBenhVienId,
                     KhoLinhId = idKhoLinh,
                     MaTN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                     MaBN = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN ,
                     HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen ,
                     YeuCauTiepNhanId = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                 }).GroupBy(x => new
                 {
                     x.MaBN,
                     x.HoTen,
                     x.MaTN,
                     x.YeuCauTiepNhanId
                 }).Select(s => new ThongTinLinhTuKhoGridVo()
                 {
                     Id = s.First().Id,
                     TenDuocPham = s.First().TenDuocPham,
                     NongDoVaHamLuong = s.First().NongDoVaHamLuong,
                     HoatChat = s.First().HoatChat,
                     DuongDung = s.First().DuongDung,
                     DonViTinh = s.First().DonViTinh,
                     HangSX = s.First().HangSX,
                     NuocSanXuat = s.First().NuocSanXuat,
                     SLYeuCau = s.Sum(x => x.SLYeuCau),
                     LoaiThuoc = s.First().LoaiThuoc,
                     DuocPhamId = s.First().DuocPhamId,
                     KhoLinhId = idKhoLinh,
                     MaTN = s.First().MaTN,
                     MaBN = s.First().MaBN,
                     HoTen = s.First().HoTen,
                     YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                     SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                 .Where(x => x.DuocPhamBenhVienId == s.First().DuocPhamId
                                                             && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPhamKhoXuatId // to do
                                                             && x.LaDuocPhamBHYT == s.First().LaDuocPhamBHYT
                                                             && x.NhapKhoDuocPhams.DaHet != true
                                                             && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                 });
                return yeuCauDuocPham.OrderBy(d=>d.LaDuocPhamBHYT).ThenBy(d=>d.MaTN).ThenBy(d=>d.TenDuocPham).ToList();
            }
            else if(kiemTraTrangThaiDuyetCuaPhieuLinh == null) // đang chờ  duyệt
            {
                var yeuCauDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.KhoLinhId == idKhoLinh &&
                                                                                           x.YeuCauLinhDuocPhamId == idYeuCauLinhDuocPham &&
                                                                                           //phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                                                           x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                                                                                           x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                                    .Select(s => new ThongTinLinhTuKhoGridVo()
                                    {
                                        Id = s.Id,
                                        TenDuocPham = s.Ten,
                                        NongDoVaHamLuong = s.HamLuong,
                                        HoatChat = s.HoatChat,
                                        DuongDung = s.DuongDung.Ten,
                                        DonViTinh = s.DonViTinh.Ten,
                                        HangSX = s.NhaSanXuat,
                                        NuocSanXuat = s.NuocSanXuat,
                                        SLYeuCau = s.SoLuong,
                                        LoaiThuoc = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
                                        LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                                        DuocPhamId = s.DuocPhamBenhVienId,
                                        KhoLinhId = idKhoLinh,
                                        MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                        MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                                        HoTen = s.YeuCauTiepNhan.HoTen,
                                        YeuCauTiepNhanId = s.YeuCauTiepNhanId
                                    }).GroupBy(x => new
                                    {
                                        x.MaBN,
                                        x.HoTen,
                                        x.MaTN,
                                        x.YeuCauTiepNhanId
                                    }).Select(s => new ThongTinLinhTuKhoGridVo()
                                    {
                                        Id = s.First().Id,
                                        TenDuocPham = s.First().TenDuocPham,
                                        NongDoVaHamLuong = s.First().NongDoVaHamLuong,
                                        HoatChat = s.First().HoatChat,
                                        DuongDung = s.First().DuongDung,
                                        DonViTinh = s.First().DonViTinh,
                                        HangSX = s.First().HangSX,
                                        NuocSanXuat = s.First().NuocSanXuat,
                                        SLYeuCau = s.Sum(x => x.SLYeuCau),
                                        LoaiThuoc = s.First().LoaiThuoc,
                                        DuocPhamId = s.First().DuocPhamId,
                                        KhoLinhId = idKhoLinh,
                                        MaTN = s.First().MaTN,
                                        MaBN = s.First().MaBN,
                                        HoTen = s.First().HoTen,
                                        YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                                        SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                                                    .Where(x => x.DuocPhamBenhVienId == s.First().DuocPhamId
                                                                                && x.NhapKhoDuocPhams.KhoId == idKhoLinh
                                                                                && x.LaDuocPhamBHYT == s.First().LaDuocPhamBHYT
                                                                                && x.NhapKhoDuocPhams.DaHet != true
                                                                                && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                    });
                return yeuCauDuocPham.OrderBy(d => d.LaDuocPhamBHYT).ThenBy(d => d.MaTN).ThenBy(d => d.TenDuocPham).ToList();
            }
            else if(kiemTraTrangThaiDuyetCuaPhieuLinh == false) // từ chối
            {
                var yeuCauLinhDuocPhamKhoXuatId =
                _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(x => x.Id == idYeuCauLinhDuocPham).Select(s => s.KhoXuatId).First();

                var queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhDuocPhamId == idYeuCauLinhDuocPham)
                    .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = idYeuCauLinhDuocPham,
                        DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                        LaBHYT = item.LaDuocPhamBHYT,
                        TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                        NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                        HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                        DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        SoLuongYeuCau = item.SoLuong
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhDuocPhamId,
                        x.DuocPhamBenhVienId,
                        x.LaBHYT,
                        x.Nhom,
                        x.NongDoHamLuong,
                        x.HoatChat,
                        x.DuongDung,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.SoLuongYeuCau
                    })
                    .Select(item => new ThongTinLinhTuKhoGridVo()
                    {
                        Id = idYeuCauLinhDuocPham,
                        KhoLinhId = idKhoLinh,
                        DuocPhamId = item.First().DuocPhamBenhVienId,
                        LoaiThuoc = item.First().LaBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                        TenDuocPham = item.First().TenDuocPham,
                        NongDoVaHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSX = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SLYeuCau = item.Sum(x => x.SoLuongYeuCau),
                        SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                        && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPhamKhoXuatId
                                        && x.NhapKhoDuocPhams.DaHet != true
                                        && x.LaDuocPhamBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                    });

                return queryable.OrderBy(d => d.LaDuocPhamBHYT).ThenBy(d => d.TenDuocPham).ToList();
            }

            return null;





            //if (trangThai == 1)
            //{
            //    var yeuCauDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.KhoLinhId == idKhoLinh &&
            //                                                                               x.YeuCauLinhDuocPhamId == idYeuCauLinhDuocPham  &&
            //                                                                               //phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
            //                                                                               x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
            //                                                                               x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
            //                        .Select(s => new ThongTinLinhTuKhoGridVo()
            //                        {
            //                            Id = s.Id,
            //                            TenDuocPham = s.Ten,
            //                            NongDoVaHamLuong = s.HamLuong,
            //                            HoatChat = s.HoatChat,
            //                            DuongDung = s.DuongDung.Ten,
            //                            DonViTinh = s.DonViTinh.Ten,
            //                            HangSX = s.NhaSanXuat,
            //                            NuocSanXuat = s.NuocSanXuat,
            //                            SLYeuCau = s.SoLuong,
            //                            LoaiThuoc = s.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
            //                            LaDuocPhamBHYT = s.LaDuocPhamBHYT,
            //                            DuocPhamId = s.DuocPhamBenhVienId,
            //                            KhoLinhId = idKhoLinh,
            //                            MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //                            MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
            //                            HoTen = s.YeuCauTiepNhan.HoTen,
            //                            YeuCauTiepNhanId = s.YeuCauTiepNhanId
            //                        }).GroupBy(x => new
            //                        {
            //                            x.MaBN,
            //                            x.HoTen,
            //                            x.MaTN,
            //                            x.YeuCauTiepNhanId
            //                        }).Select(s => new ThongTinLinhTuKhoGridVo()
            //                        {
            //                            Id = s.First().Id,
            //                            TenDuocPham = s.First().TenDuocPham,
            //                            NongDoVaHamLuong = s.First().NongDoVaHamLuong,
            //                            HoatChat = s.First().HoatChat,
            //                            DuongDung = s.First().DuongDung,
            //                            DonViTinh = s.First().DonViTinh,
            //                            HangSX = s.First().HangSX,
            //                            NuocSanXuat = s.First().NuocSanXuat,
            //                            SLYeuCau = s.Sum(x => x.SLYeuCau),
            //                            LoaiThuoc = s.First().LoaiThuoc,
            //                            DuocPhamId = s.First().DuocPhamId,
            //                            KhoLinhId = idKhoLinh,
            //                            MaTN = s.First().MaTN,
            //                            MaBN = s.First().MaBN,
            //                            HoTen = s.First().HoTen,
            //                            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
            //                            SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
            //                                                        .Where(x => x.DuocPhamBenhVienId == s.First().DuocPhamId
            //                                                                    && x.NhapKhoDuocPhams.KhoId == idKhoLinh
            //                                                                    && x.LaDuocPhamBHYT == s.First().LaDuocPhamBHYT
            //                                                                    && x.NhapKhoDuocPhams.DaHet != true
            //                                                                    && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
            //                        });
            //    return yeuCauDuocPham.ToList();
            //}
            //else
            //{
            //    var yeuCauLinhDuocPhamKhoXuatId =
            //    _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(x => x.Id == idYeuCauLinhDuocPham).Select(s => s.KhoXuatId).First();

            //    var queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
            //        .Where(x => x.YeuCauLinhDuocPhamId == idYeuCauLinhDuocPham)
            //        .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
            //        {
            //            YeuCauLinhDuocPhamId = idYeuCauLinhDuocPham,
            //            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
            //            LaBHYT = item.LaDuocPhamBHYT,
            //            TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
            //            NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
            //            HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
            //            DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
            //            DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
            //            HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
            //            NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
            //            SoLuongYeuCau = item.SoLuong
            //        })
            //        .GroupBy(x => new
            //        {
            //            x.YeuCauLinhDuocPhamId,
            //            x.DuocPhamBenhVienId,
            //            x.LaBHYT,
            //            x.Nhom,
            //            x.NongDoHamLuong,
            //            x.HoatChat,
            //            x.DuongDung,
            //            x.DonViTinh,
            //            x.HangSanXuat,
            //            x.NuocSanXuat,
            //            x.SoLuongYeuCau
            //        })
            //        .Select(item => new ThongTinLinhTuKhoGridVo()
            //        {
            //            Id = idYeuCauLinhDuocPham,
            //            KhoLinhId = idKhoLinh,
            //            DuocPhamId = item.First().DuocPhamBenhVienId,
            //            LoaiThuoc = item.First().LaBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
            //            TenDuocPham = item.First().TenDuocPham,
            //            NongDoVaHamLuong = item.First().NongDoHamLuong,
            //            HoatChat = item.First().HoatChat,
            //            DuongDung = item.First().DuongDung,
            //            DonViTinh = item.First().DonViTinh,
            //            HangSX = item.First().HangSanXuat,
            //            NuocSanXuat = item.First().NuocSanXuat,
            //            SLYeuCau = item.Sum(x => x.SoLuongYeuCau),
            //            SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
            //                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
            //                            && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPhamKhoXuatId
            //                            && x.NhapKhoDuocPhams.DaHet != true
            //                            && x.LaDuocPhamBHYT == item.First().LaBHYT
            //                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
            //        });

            //    return queryable.ToList();
            //}
        }
        public List<ThongTinLinhTuKho> GetDataThongTin(long idKhoLinh)
        {
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            long phongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();


            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Select(s => new ThongTinLinhTuKho()
                {
                    LinhVePhongId = phongLamViecId,
                    //LinhVePhong = phongBenhVien,
                    LinhVeKhoa = tenKhoa,
                    NguoiYeuCau = nguoiLogin,
                    NhanVienYeuCauId = userId,
                    NgayYeuCau = s.ThoiDiemChiDinh,
                    GhiChu = s.GhiChu,
                    TenKho = s.KhoLinh.Ten
                });
            return query.ToList();
        }
        public ThongTinLinhTuKho ThongTinDanhSachCanLinh(long idKhoLinh, long phongBenhVienId)
        {
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            // lấy 1 phòng bệnh viện bất kỳ thuộc khoa
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Select(s => new ThongTinLinhTuKho()
                {
                    LinhVePhongId = phongBenhVien.Id,
                    LinhVeKhoa = tenKhoa,
                    NguoiYeuCau = nguoiLogin,
                    NhanVienYeuCauId = userId,
                    NgayYeuCau = s.ThoiDiemChiDinh,
                    GhiChu = s.GhiChu,
                    TenKho = s.KhoLinh.Ten
                });
            return query.FirstOrDefault();
        }
        public List<ThongTinLinhTuKho> GetDataThongTinDaTao(long idYeuCauLinh)
        {
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(s => s.YeuCauLinhDuocPhamId == idYeuCauLinh);
            if (query.Any())
            {
                var queryChuaDuyetVaDaDuyet = BaseRepository.TableNoTracking.Where(s => s.Id == idYeuCauLinh).Select(s => new ThongTinLinhTuKho()
                {
                    Id = s.Id,
                    LinhVePhongId = s.KhoNhapId,
                    LinhVeKhoa = s.NoiYeuCauId != null ? s.NoiYeuCau.KhoaPhong.Ten :"",
                    NoiChiDinhId = s.YeuCauDuocPhamBenhViens.First().NoiChiDinh.Id,
                    LinhVePhong = s.YeuCauDuocPhamBenhViens.First().NoiChiDinh.Ten,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    NhanVienYeuCauId = (long)s.NhanVienYeuCauId,
                    NgayYeuCau = s.NgayYeuCau.Date,
                    GhiChu = s.GhiChu,
                    TenKho = s.KhoXuat.Ten
                }).ToList();
                return queryChuaDuyetVaDaDuyet.ToList();
            }
            else
            {
                var queryTuChoiDuyet = BaseRepository.TableNoTracking.Where(s => s.Id == idYeuCauLinh).Select(s => new ThongTinLinhTuKho()
                {
                    Id = s.Id,
                    LinhVePhongId = s.KhoNhapId,
                    LinhVeKhoa = s.NoiYeuCauId != null ? s.NoiYeuCau.KhoaPhong.Ten : "",
                    LinhVePhong = s.NoiYeuCau.Ten,
                    NguoiYeuCau = s.NhanVienYeuCau.User.HoTen,
                    NhanVienYeuCauId = (long)s.NhanVienYeuCauId,
                    NgayYeuCau = s.NgayYeuCau.Date,
                    GhiChu = s.GhiChu,
                    TenKho = s.KhoXuat.Ten
                }).ToList();
                return queryTuChoiDuyet.ToList();
            }

            return null;
        }
        // create
       
        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<LinhTrucTiepVo>(queryInfo.AdditionalSearchString);
            long phongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();

            var querystring = queryInfo.AdditionalSearchString.Split('-');

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            
            if (!string.IsNullOrEmpty(queryString.DateBatDau) && queryString.DateBatDau != "null")
            {
                DateTime.TryParseExact(queryString.DateBatDau, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgayTemp);
                tuNgay = new DateTime(tuNgayTemp.Year, tuNgayTemp.Month, tuNgayTemp.Day, 0, 0, 0);
            }
            if (!string.IsNullOrEmpty(queryString.DateKetThuc) && queryString.DateKetThuc != "null")
                {
                DateTime.TryParseExact(queryString.DateKetThuc, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgayTemp);
                denNgay = new DateTime(denNgayTemp.Year, denNgayTemp.Month, denNgayTemp.Day, 23, 59, 59);
            }
            var i = 1;
            var queryYeuCauKhamBenh = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId &&
                            x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                            x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                             phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                            x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                            x.KhoLinhId == queryString.KhoLinhId
                            )
                .Select(p => new ThongTinLanKhamKho()
                {
                    STT = i + 1,
                    Id = p.Id,
                    MaTN = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    DVKham = p.YeuCauKhamBenhId != null ? p.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : p.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? p.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                    BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                    SLKe = (int)p.SoLuong,
                    NgayYeuCau = p.ThoiDiemChiDinh,
                    NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    NgayDieuTri = (p.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && p.NoiTruPhieuDieuTri != null) ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                    DuocDuyet = p.YeuCauLinhDuocPhamId == null ? false : p.YeuCauLinhDuocPham.DuocDuyet == true ? true : false,
                    TenDuocPham = p.Ten,
                    NongDoVaHamLuong = p.HamLuong,
                    HoatChat = p.HoatChat,
                    DuongDung = p.DuongDung.Ten,
                    DonViTinh = p.DonViTinh.Ten,
                    HangSX = p.NhaSanXuat,
                    NuocSanXuat = p.NuocSanXuat,
                    SLYeuCau = p.SoLuong,
                    LoaiThuoc = p.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                    SoLuongTon = p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == p.KhoLinhId && nkct.LaDuocPhamBHYT == p.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                    IsCheckRowItem = queryString.CheckItem,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId
                }).Where(p => (tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay));

            // search ngày đăng ký
            var quaythuoc = queryYeuCauKhamBenh.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = queryYeuCauKhamBenh.Count();

            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
        }
        public List<LinhTrucTiepDuocPhamChiTietGridVo> GetDataForGridChiTietChildCreateAsync(long yeuCauDuocPhamBenhVienId)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();
            var queryYeuCauKhamBenh = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
            .Where(x => x.Id == yeuCauDuocPhamBenhVienId && x.TrangThai == EnumYeuCauDuocPhamBenhVien.ChuaThucHien && x.YeuCauLinhDuocPhamId == null
                        )
              .Select(s => new LinhTrucTiepDuocPhamChiTietGridVo()
              {
                  Id = yeuCauDuocPhamBenhVienId,
                  LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                  DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                  SoLuong = s.SoLuong,
                  SLTon = s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoLinhId && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
              });
            return queryYeuCauKhamBenh.Where(s=>s.SLTon >= s.SoLuong).ToList();
        }
        public List<YeuCauDuocPhamBenhVienTT> GetPhieuLinhTrucTiepTT(long yeuCauTiepNhanId, long? khLinhId)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();
            var queryYeuCauKhamBenh = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
            .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                        x.KhoLinhId == khLinhId &&
                        phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                        x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                        x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                        x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                        )
              .Select(s => new YeuCauDuocPhamBenhVienTT()
              {
                  Id = s.Id,
                  DuocPhamId = s.DuocPhamBenhVienId,
                  YeuCauTiepNhanId = yeuCauTiepNhanId,
                  LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                  TenThuoc = s.DuocPhamBenhVien.DuocPham.Ten,
                  SoLuong = s.SoLuong,
                  SLTon = s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoLinhId && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
              });
            return queryYeuCauKhamBenh.Where(s => s.SLTon >= s.SoLuong).ToList();
        }
        // đã tạo
        public async Task<GridDataSource> GetAllYeuCauLinhThuocTuKhoDaTao(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<LinhTrucTiepVo>(queryInfo.AdditionalSearchString);
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();

            var i = 1;
            var queryYeuCauKhamBenh = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                                              .Where(x=> x.YeuCauLinhDuocPhamId == queryString.YeuCauLinhDuocPhamId
                                                                         //&& phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId)
                                                                         && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                         && x.KhoLinhId == queryString.KhoLinhId 
                                                                         && x.YeuCauTiepNhanId == queryString.YeuCauTiepNhanId )
              .Select(p => new ThongTinLanKhamKho()
              {
                  MaTN = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                  MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                  HoTen = p.YeuCauTiepNhan.HoTen,
                  DVKham = p.YeuCauKhamBenhId != null ? p.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : p.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? p.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "Ghi nhận trong PTTT",
                  BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                  SLKe = (int)p.SoLuong,
                  NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                  DuocDuyet = p.YeuCauLinhDuocPhamId == null ? false : p.YeuCauLinhDuocPham.DuocDuyet == true ? true : false,
                  TenDuocPham = p.Ten,
                  NongDoVaHamLuong = p.HamLuong,
                  HoatChat = p.HoatChat,
                  DuongDung = p.DuongDung.Ten,
                  DonViTinh = p.DonViTinh.Ten,
                  HangSX = p.NhaSanXuat,
                  NuocSanXuat = p.NuocSanXuat,
                  SLYeuCau = p.SoLuong,
                  LoaiThuoc = p.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                  SoLuongTon = p.YeuCauLinhDuocPham.DuocDuyet == true ? _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId
                                           && x.NhapKhoDuocPhams.KhoId == queryString.KhoLinhId
                                           && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                           && x.NhapKhoDuocPhams.DaHet != true
                                           && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat):
                                           _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                               .Where(x => x.DuocPhamBenhVienId == p.DuocPhamBenhVienId
                                           && x.NhapKhoDuocPhams.KhoId == queryString.KhoLinhId
                                           && x.LaDuocPhamBHYT == p.LaDuocPhamBHYT
                                           && x.NhapKhoDuocPhams.DaHet != true
                                           && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                  NgayDieuTri = (p.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && p.NoiTruPhieuDieuTri != null) ? p.NoiTruPhieuDieuTri.NgayDieuTri  : p.ThoiDiemChiDinh,

              });
            var item = 1;
            var thongTinLanKhamKho = new List<ThongTinLanKhamKho>();
            foreach (var itemx in queryYeuCauKhamBenh.ToList())
            {
                itemx.STT = item++;
                thongTinLanKhamKho.Add(itemx);
            }
            var dataOrderByss = thongTinLanKhamKho.OrderByDescending(dd => dd.SoLuongTon < dd.SLKe ? 1 : 0).ThenBy(d => d.TenDuocPham).ToList();
            var dataOrderBy = dataOrderByss.AsQueryable();
            var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();

            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
        }
        public long GetIdKhoNhap(long? idPhongBenhVien)
        {
            var idKhoNhap = _khoRepository.TableNoTracking.Where(x => x.PhongBenhVienId == idPhongBenhVien && x.LaKhoKSNK != true).Select(s => s.Id);
            return (long)idKhoNhap.FirstOrDefault();
        }
        public long GetAllIdKhoNhapNhanVien(long? idNhanVien)
        {
            var khoNhanVien = _khoNhanVienQuanLyRepository.TableNoTracking.Where(s => s.NhanVienId == idNhanVien && ( s.Kho != null && s.Kho.LaKhoKSNK != true)).Select(x => x.KhoId);

            return khoNhanVien.FirstOrDefault();

        }
        public bool? GetTrangThaiDuyet(long id)
        {
            var duocDuyet = BaseRepository.TableNoTracking
               .Where(x => x.Id == id).Select(p => p.DuocDuyet).FirstOrDefault();
            return duocDuyet;
        }
        public DaDuyet GetDaDuyet(long IdYeuCauLinh)
        {
            var queryYCLDP = BaseRepository.TableNoTracking
                .Where(x => x.Id == IdYeuCauLinh).Select(p => new DaDuyet()
                {
                    NgayDuyet = p.NgayDuyet,
                    NguoiDuyet = p.NhanVienDuyetId != null ? p.NhanVienDuyet.User.HoTen : ""
                });

            return queryYCLDP.FirstOrDefault();
        }
        public string InPhieuLinhTrucTiepDuocPham(XacNhanInLinhDuocPham xacNhanInLinhDuocPham)
        {
            var content = "";
            var content1 = "";
            var ThuocHoacVatTu = " ";

            var result = _templateRepo.TableNoTracking
              .FirstOrDefault(x => x.Name.Equals("PhieuLinhThuocDuocPham"));
            var resultGayNghien = _templateRepo.TableNoTracking
             .FirstOrDefault(x => x.Name.Equals("PhieuLinhThuocTrucTiepGayNghien"));

            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU LĨNH DƯỢC PHẨM</div></div>";


            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            int? nhomVatTu = 0;
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }
            if (xacNhanInLinhDuocPham.TrangThaiIn == false)
            {
                var yeuCauLinhDuocPhamTuChoi = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Include(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham).ThenInclude(p => p.DonViTinh)
                                                                                                   .Include(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham).ThenInclude(p => p.DuongDung)
                                                                                                   .Include(p => p.YeuCauLinhDuocPham).ThenInclude(x => x.KhoXuat)
                                                                                                   .Include(p => p.YeuCauLinhDuocPham).ThenInclude(x => x.KhoNhap)
                                                                                                   .Include(p => p.YeuCauLinhDuocPham).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                                                   .Include(p => p.YeuCauDuocPhamBenhVien).ThenInclude(p=>p.YeuCauTiepNhan)
                                                                                                   .Where(s => s.YeuCauLinhDuocPhamId == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId);

                var listDuocPham = yeuCauLinhDuocPhamTuChoi.Where(d => d.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && d.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan)
                                                               .Select(o => new DuocPhamLoaiQuanLyLinhTTGridVo
                                                               {
                                                                   LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                                                                   MaDuocPham = o.DuocPhamBenhVien.Ma,
                                                                   TenDuocPham = o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId != null ? Convert.ToInt32(o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == Convert.ToInt32(nhomVatTu) ?
                                                                                                                                  o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && o.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                                                                    (o.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && o.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                                                                                  o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.HamLuong != null && o.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + o.DuocPhamBenhVien.DuocPham.HamLuong : "") : "",//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                                                   DuocDuyet = o.YeuCauLinhDuocPham.DuocDuyet,
                                                                   SoLuong = o.SoLuong,
                                                                   DonViTinh = o.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                   SoLuongCoTheXuat = o.SoLuongCoTheXuat,
                                                               }).GroupBy(xy => new { xy.TenDuocPham, xy.MaDuocPham, xy.DonViTinh })
                                                             .Select(o => new DuocPhamLoaiQuanLyLinhTTGridVo
                                                             {
                                                                 LaDuocPhamBHYT = o.First().LaDuocPhamBHYT,
                                                                 MaDuocPham = o.First().MaDuocPham,
                                                                 TenDuocPham = o.First().TenDuocPham,
                                                                 SoLuong = o.Sum(s => s.SoLuong),
                                                                 DonViTinh = o.First().DonViTinh,
                                                             }).OrderBy(d=>d.TenDuocPham).ToList();

                var listGayNghienHuongThan = yeuCauLinhDuocPhamTuChoi.Where(d => (d.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || d.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                                                              .Select(o => new DuocPhamLoaiQuanLyLinhTTGridVo
                                                              {
                                                                  LaDuocPhamBHYT = o.LaDuocPhamBHYT,
                                                                  MaDuocPham = o.DuocPhamBenhVien.Ma,
                                                                  TenDuocPham = o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId != null ? Convert.ToInt32(o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == Convert.ToInt32(nhomVatTu) ?
                                                                                                                                 o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && o.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                                                                   (o.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && o.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + o.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                                                                                 o.DuocPhamBenhVien.DuocPham.Ten + (o.DuocPhamBenhVien.DuocPham.HamLuong != null && o.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + o.DuocPhamBenhVien.DuocPham.HamLuong : "") : "",//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                                                  DuocDuyet = o.YeuCauLinhDuocPham.DuocDuyet,
                                                                  SoLuong = o.SoLuong,
                                                                  DonViTinh = o.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                  SoLuongCoTheXuat = o.SoLuongCoTheXuat,
                                                              }).GroupBy(xy => new { xy.TenDuocPham, xy.MaDuocPham, xy.DonViTinh })
                                                            .Select(o => new DuocPhamLoaiQuanLyLinhTTGridVo
                                                            {
                                                                LaDuocPhamBHYT = o.First().LaDuocPhamBHYT,
                                                                MaDuocPham = o.First().MaDuocPham,
                                                                TenDuocPham = o.First().TenDuocPham,
                                                                SoLuong = o.Sum(s => s.SoLuong),
                                                                DonViTinh = o.First().DonViTinh,
                                                            }).OrderBy(d => d.TenDuocPham).ToList();
                if (listDuocPham.Any())
                {
                    var objData = GetHTMLPhieuLinhBenhNhanTuChoi(listDuocPham.ToList(),false);
                    ThuocHoacVatTu = objData.html;
                    var maVachPhieuLinh = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.SoPhieu).FirstOrDefault();

                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.SoPhieu).FirstOrDefault()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.KhoXuat.Ten).FirstOrDefault(),
                        DienGiai = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.GhiChu).FirstOrDefault(),
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.GhiChu).FirstOrDefault(),
                        NguoiNhan = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.NhanVienDuyet.User.HoTen).FirstOrDefault(),
                        TuNgay = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.ThoiDiemLinhTongHopTuNgay).Any() ? yeuCauLinhDuocPhamTuChoi.Select(x => (DateTime)x.YeuCauLinhDuocPham.ThoiDiemLinhTongHopTuNgay).FirstOrDefault().ApplyFormatDateTimeSACH() : "",
                        DenNgay = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.ThoiDiemLinhTongHopDenNgay).Any() ? yeuCauLinhDuocPhamTuChoi.Select(x => (DateTime)x.YeuCauLinhDuocPham.ThoiDiemLinhTongHopDenNgay).FirstOrDefault().ApplyFormatDateTimeSACH() : "",
                        TruongPhongKhoaPhong = "",
                        CongKhoan = objData.Index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        ThuocHoacVatTu = ThuocHoacVatTu,
                        KhoaPhong = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.KhoNhap.Ten).FirstOrDefault(),
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        //Gio = DateTime.Now.ApplyFormatTime()
                        NoiNhan = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.NoiYeuCauId).Any()?
                                  TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPhamTuChoi.Select(x=>x.YeuCauLinhDuocPham.NoiYeuCauId).FirstOrDefault())
                                  :""
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                }
                if (listGayNghienHuongThan.Any())
                {
                    var objData = GetHTMLPhieuLinhBenhNhanTuChoi(listGayNghienHuongThan.ToList(),true);
                    ThuocHoacVatTu = objData.html;
                    var maVachPhieuLinh = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.SoPhieu).FirstOrDefault();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.SoPhieu).FirstOrDefault()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.KhoXuat.Ten).FirstOrDefault(),
                        DienGiai = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.GhiChu).FirstOrDefault(),
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.GhiChu).FirstOrDefault(),
                        NguoiNhan = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.NhanVienDuyet.User.HoTen).FirstOrDefault(),
                        TuNgay = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.ThoiDiemLinhTongHopTuNgay).Any() ? yeuCauLinhDuocPhamTuChoi.Select(x => (DateTime)x.YeuCauLinhDuocPham.ThoiDiemLinhTongHopTuNgay).FirstOrDefault().ApplyFormatDateTimeSACH() : "",
                        DenNgay = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.ThoiDiemLinhTongHopDenNgay).Any() ? yeuCauLinhDuocPhamTuChoi.Select(x => (DateTime)x.YeuCauLinhDuocPham.ThoiDiemLinhTongHopDenNgay).FirstOrDefault().ApplyFormatDateTimeSACH() : "",
                        TruongPhongKhoaPhong = "",
                        CongKhoan = objData.Index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        ThuocHoacVatTu = ThuocHoacVatTu,
                        KhoaPhong = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.KhoNhap.Ten).FirstOrDefault(),
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        //Gio = DateTime.Now.ApplyFormatTime()
                        NoiNhan = yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.NoiYeuCauId).Any() ?
                                  TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPhamTuChoi.Select(x => x.YeuCauLinhDuocPham.NoiYeuCauId).FirstOrDefault())
                                  : ""
                    };
                    content1 = TemplateHelpper.FormatTemplateWithContentTemplate(resultGayNghien.Body, data);
                }

            }
            else
            {
                content1 = "";
                var listDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(s => s.YeuCauLinhDuocPhamId == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId &&
                    s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                    s.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                    && s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien &&
                    s.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan &&
                    s.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                              .Select(p => new ThongTinLanKhamKho()
                              {
                                  NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                  DuocDuyet = p.YeuCauLinhDuocPhamId == null ? false : p.YeuCauLinhDuocPham.DuocDuyet == true ? true : false,
                                  BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                                  NongDoVaHamLuong = p.HamLuong,
                                  HoatChat = p.DuocPhamBenhVien.Ma, // MÃ DƯỢC PHẨM K PHẢI MÃ HOẠT CHẤT
                                  DuongDung = p.DuongDung.Ten,
                                  DonViTinh = p.DonViTinh.Ten,
                                  HangSX = p.NhaSanXuat,
                                  NuocSanXuat = p.NuocSanXuat,
                                  SLYeuCau = p.SoLuong,
                                  LoaiThuoc = p.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                  DuocPhamId = p.DuocPhamBenhVienId,
                                  LaDuocPhamBHYT = p.LaDuocPhamBHYT,
                                  GhiChu = p.GhiChu,
                                  LoaiPhieuLinh = p.LoaiPhieuLinh.GetDescription(),
                                  TenDuocPham = (int?)(p.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)(nhomVatTu) ?
                                                                 (p.DuocPhamBenhVien.DuocPham.Ten + (p.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && p.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + p.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                 (p.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && p.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + p.DuocPhamBenhVien.DuocPham.NuocSanXuat : "")) :
                                                                 p.DuocPhamBenhVien.DuocPham.Ten + (p.DuocPhamBenhVien.DuocPham.HamLuong != null && p.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + p.DuocPhamBenhVien.DuocPham.HamLuong : ""),//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                              })
                            .GroupBy(x => new
                            {
                                x.DuocPhamId,
                                x.LaDuocPhamBHYT,
                                x.NongDoVaHamLuong,
                                x.HoatChat,
                                x.DuongDung,
                                x.DonViTinh,
                                x.NuocSanXuat,
                                x.NgayKe
                            })
                            .Select(p => new ThongTinLanKhamKho()
                            {
                                BacSyKeToa = p.First().BacSyKeToa,
                                DuocDuyet = p.First().DuocDuyet,
                                LoaiThuoc = p.First().LoaiThuoc,
                                LaDuocPhamBHYT = p.First().LaDuocPhamBHYT,
                                TenDuocPham = p.First().TenDuocPham,
                                HoatChat = p.First().HoatChat,
                                NongDoVaHamLuong = p.First().NongDoVaHamLuong,
                                DuongDung = p.First().DuongDung,
                                HangSX = p.First().HangSX,
                                NuocSanXuat = p.First().NuocSanXuat,
                                SLYeuCau = p.Sum(s => s.SLYeuCau),
                                NgayKe = p.First().NgayKe,
                                GhiChu = p.First().GhiChu,
                                LoaiPhieuLinh = p.First().LoaiPhieuLinh,
                                DonViTinh = p.First().DonViTinh,
                            }).OrderBy(p => p.TenDuocPham).ToList();

                var listGayNghienHuongThan = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(s => s.YeuCauLinhDuocPhamId == xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId &&
                    s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                    s.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                    s.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                    && (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                              .Select(p => new ThongTinLanKhamKho()
                              {
                                  NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                  DuocDuyet = p.YeuCauLinhDuocPhamId == null ? false : p.YeuCauLinhDuocPham.DuocDuyet == true ? true : false,
                                  BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                                  NongDoVaHamLuong = p.HamLuong,
                                  HoatChat = p.DuocPhamBenhVien.Ma, // MÃ DƯỢC PHẨM K PHẢI MÃ HOẠT CHẤT
                                  DuongDung = p.DuongDung.Ten,
                                  DonViTinh = p.DonViTinh.Ten,
                                  HangSX = p.NhaSanXuat,
                                  NuocSanXuat = p.NuocSanXuat,
                                  SLYeuCau = p.SoLuong,
                                  LoaiThuoc = p.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                  DuocPhamId = p.DuocPhamBenhVienId,
                                  LaDuocPhamBHYT = p.LaDuocPhamBHYT,
                                  GhiChu = p.GhiChu,
                                  LoaiPhieuLinh = p.LoaiPhieuLinh.GetDescription(),
                                  TenDuocPham = (int?)(p.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)(nhomVatTu) ?
                                                                 (p.DuocPhamBenhVien.DuocPham.Ten + (p.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && p.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + p.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                 (p.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && p.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + p.DuocPhamBenhVien.DuocPham.NuocSanXuat : "")) :
                                                                 p.DuocPhamBenhVien.DuocPham.Ten + (p.DuocPhamBenhVien.DuocPham.HamLuong != null && p.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + p.DuocPhamBenhVien.DuocPham.HamLuong : ""),//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,

                              })
                            .GroupBy(x => new
                            {
                                x.DuocPhamId,
                                x.LaDuocPhamBHYT,
                                x.NongDoVaHamLuong,
                                x.HoatChat,
                                x.DuongDung,
                                x.DonViTinh,
                                x.NuocSanXuat,
                                x.NgayKe
                            })
                            .Select(p => new ThongTinLanKhamKho()
                            {
                                BacSyKeToa = p.First().BacSyKeToa,
                                DuocDuyet = p.First().DuocDuyet,
                                LoaiThuoc = p.First().LoaiThuoc,
                                LaDuocPhamBHYT = p.First().LaDuocPhamBHYT,
                                TenDuocPham = p.First().TenDuocPham,
                                HoatChat = p.First().HoatChat,
                                NongDoVaHamLuong = p.First().NongDoVaHamLuong,
                                DuongDung = p.First().DuongDung,
                                HangSX = p.First().HangSX,
                                NuocSanXuat = p.First().NuocSanXuat,
                                SLYeuCau = p.Sum(s => s.SLYeuCau),
                                NgayKe = p.First().NgayKe,
                                GhiChu = p.First().GhiChu,
                                LoaiPhieuLinh = p.First().LoaiPhieuLinh,
                                DonViTinh = p.First().DonViTinh,
                            }).OrderBy(p => p.TenDuocPham).ToList();

                if (listDuocPham.Any())
                {
                    var objData = GetHTMLPhieuLinhBenhNhan(listDuocPham,false);
                    ThuocHoacVatTu = objData.html;
                    var yeuCauLinhDuocPhams = BaseRepository.GetByIdAsync(xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId,
                                                                   s => s.Include(z => z.KhoNhap)
                                                                        .Include(z => z.KhoXuat)
                                                                        .Include(z => z.NhanVienYeuCau)
                                                                        .Include(z => z.NhanVienDuyet)
                                                                        .Include(z => z.YeuCauDuocPhamBenhViens)
                                                                        .Include(z => z.YeuCauLinhDuocPhamChiTiets));
                    var maVachPhieuLinh = yeuCauLinhDuocPhams?.Result?.SoPhieu.ToString();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham?.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPhams?.Result?.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhDuocPhams?.Result?.KhoXuat?.Ten,
                        DienGiai = yeuCauLinhDuocPhams?.Result?.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhDuocPhams?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhDuocPhams?.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhDuocPhams?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhDuocPhams?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = objData.Index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        //HeaderPhieuLinhThuoc = tenLoaiLinh,
                        ThuocHoacVatTu = ThuocHoacVatTu,
                        KhoaPhong = yeuCauLinhDuocPhams?.Result?.KhoNhap?.Ten,
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        //Gio = DateTime.Now.ApplyFormatTime()
                        NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPhams?.Result?.NoiYeuCauId)


                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                    //return content;

                }
                if (listGayNghienHuongThan.Any())
                {
                    var objData = GetHTMLPhieuLinhBenhNhan(listGayNghienHuongThan,true);
                    ThuocHoacVatTu = objData.html;

                    var yeuCauLinhDuocPhams = BaseRepository.GetByIdAsync(xacNhanInLinhDuocPham.YeuCauLinhDuocPhamId,
                                                                   s => s.Include(z => z.KhoNhap)
                                                                        .Include(z => z.KhoXuat)
                                                                        .Include(z => z.NhanVienYeuCau)
                                                                        .Include(z => z.NhanVienDuyet)
                                                                        .Include(z => z.YeuCauDuocPhamBenhViens)
                                                                        .Include(z => z.YeuCauLinhDuocPhamChiTiets));
                    var maVachPhieuLinh = yeuCauLinhDuocPhams?.Result?.SoPhieu.ToString();
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham?.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPhams?.Result?.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhDuocPhams?.Result?.KhoXuat?.Ten,
                        DienGiai = yeuCauLinhDuocPhams?.Result?.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhDuocPhams?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhDuocPhams?.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhDuocPhams?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhDuocPhams?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = objData.Index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        //HeaderPhieuLinhThuoc = tenLoaiLinh,
                        ThuocHoacVatTu = ThuocHoacVatTu,
                        KhoaPhong = yeuCauLinhDuocPhams?.Result?.KhoNhap?.Ten,
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        //Gio = DateTime.Now.ApplyFormatTime()
                        NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPhams?.Result?.NoiYeuCauId)
                    };
                    content1 = TemplateHelpper.FormatTemplateWithContentTemplate(resultGayNghien.Body, data);
                    //return content;

                }
            }
            if (!string.IsNullOrEmpty(content))
            {
                content = headerTitile + content;
            }
            var congPage = string.Empty;
            congPage = !string.IsNullOrEmpty(content) ? "<div style='break-after:page'></div>" : "";
            if (content1 != "")
            {
                content = content + headerTitile + congPage + content1;
            }

            return content;
        }
        public string InPhieuLinhTrucTiepDuocPhamXemTruoc(XacNhanInLinhDuocPhamXemTruoc xacNhanInLinhDuocPhamXemTruoc)
        {
            var content = "";
            var contentGNHT = "";
            var ThuocHoacVatTu = " ";

            var result = _templateRepo.TableNoTracking
              .FirstOrDefault(x => x.Name.Equals("PhieuLinhThuocDuocPham"));
            var resultGNHT = _templateRepo.TableNoTracking
             .FirstOrDefault(x => x.Name.Equals("PhieuLinhThuocTrucTiepGayNghien"));

            var thuocBHYT = "";
            var thuocKBHYT = "";
            var groupThuocBHYT = "Dược Phẩm BHYT";
            var groupThuocKhongBHYT = "Dược Phẩm Không BHYT";

            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                        + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";
            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU LĨNH DƯỢC PHẨM</div></div>";


            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            int? nhomVatTu = 0;
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }
            List<ThongTinLanKhamKho> listAllDuocPhamTheoListYeuCauDuocPhamId = new List<ThongTinLanKhamKho>();
            List<ThongTinLanKhamKho> listAllDuocPhamTheoListYeuCauDuocPhamIdHuongThanGayNghien = new List<ThongTinLanKhamKho>();

            if (xacNhanInLinhDuocPhamXemTruoc.YeuCauDuocPhamBenhVienIds.Any())
            {
                var yeuCauLinhDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(s => xacNhanInLinhDuocPhamXemTruoc.YeuCauDuocPhamBenhVienIds.Contains(s.Id) && s.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                              .Select(p => new ThongTinLanKhamKho()
                              {
                                  NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                  DuocDuyet = p.YeuCauLinhDuocPhamId == null ? false : p.YeuCauLinhDuocPham.DuocDuyet == true ? true : false,
                                  BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                                  NongDoVaHamLuong = p.HamLuong,
                                  HoatChat = p.DuocPhamBenhVien.Ma, // MÃ DƯỢC PHẨM K PHẢI MÃ HOẠT CHẤT
                                  DuongDung = p.DuongDung.Ten,
                                  DonViTinh = p.DonViTinh.Ten,
                                  HangSX = p.NhaSanXuat,
                                  NuocSanXuat = p.NuocSanXuat,
                                  SLYeuCau = p.SoLuong,
                                  LoaiThuoc = p.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                  DuocPhamId = p.DuocPhamBenhVienId,
                                  LaDuocPhamBHYT = p.LaDuocPhamBHYT,
                                  GhiChu = p.GhiChu,
                                  LoaiPhieuLinh = p.LoaiPhieuLinh.GetDescription(),
                                  TenDuocPham = (int?)(p.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)(nhomVatTu) ?
                                                                 (p.DuocPhamBenhVien.DuocPham.Ten + (p.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && p.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + p.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                 (p.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && p.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + p.DuocPhamBenhVien.DuocPham.NuocSanXuat : "")) :
                                                                 p.DuocPhamBenhVien.DuocPham.Ten + (p.DuocPhamBenhVien.DuocPham.HamLuong != null && p.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + p.DuocPhamBenhVien.DuocPham.HamLuong : ""),//o.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId o.DuocPhamBenhVien.DuocPham.Ten + "; " +o.DuocPhamBenhVien.DuocPham.HamLuong ,
                                  LoaiThuocTheoQuanLy = p.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                              })
                            .GroupBy(x => new
                            {
                                x.DuocPhamId,
                                x.LaDuocPhamBHYT,
                                x.NongDoVaHamLuong,
                                x.HoatChat,
                                x.DuongDung,
                                x.DonViTinh,
                                x.NuocSanXuat,
                                x.NgayKe
                            })
                            .Select(p => new ThongTinLanKhamKho()
                            {
                                BacSyKeToa = p.First().BacSyKeToa,
                                DuocDuyet = p.First().DuocDuyet,
                                LoaiThuoc = p.First().LoaiThuoc,
                                LaDuocPhamBHYT = p.First().LaDuocPhamBHYT,
                                TenDuocPham = p.First().TenDuocPham,
                                HoatChat = p.First().HoatChat,
                                NongDoVaHamLuong = p.First().NongDoVaHamLuong,
                                DuongDung = p.First().DuongDung,
                                HangSX = p.First().HangSX,
                                NuocSanXuat = p.First().NuocSanXuat,
                                SLYeuCau = p.Sum(s => s.SLYeuCau),
                                NgayKe = p.First().NgayKe,
                                GhiChu = p.First().GhiChu,
                                LoaiPhieuLinh = p.First().LoaiPhieuLinh,
                                DonViTinh = p.First().DonViTinh,
                                DuocPhamId = p.First().DuocPhamId,
                                LoaiThuocTheoQuanLy = p.First().LoaiThuocTheoQuanLy,
                           }).OrderBy(d => d.TenDuocPham).ToList();

                var listThuocBinhThuongs = yeuCauLinhDuocPham.Where(s => (s.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien &&
                                                                         s.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan)).ToList();

                var listThuocGayNghienHuongThans = yeuCauLinhDuocPham.Where(s => (s.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien ||
                                                                         s.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)).ToList();

                listAllDuocPhamTheoListYeuCauDuocPhamId.AddRange(listThuocBinhThuongs);
                listAllDuocPhamTheoListYeuCauDuocPhamIdHuongThanGayNghien.AddRange(listThuocGayNghienHuongThans);
            }


            if (listAllDuocPhamTheoListYeuCauDuocPhamId.Any())
            {

                var objData = GetHTMLPhieuLinhBenhNhan(listAllDuocPhamTheoListYeuCauDuocPhamId.ToList(),false);
                string tenPhong = "";
                var khoLinh = _khoRepository.Table.FirstOrDefault(o => o.Id == xacNhanInLinhDuocPhamXemTruoc.KhoLinhId);
                if (khoLinh != null)
                {
                    tenPhong = khoLinh.Ten;
                }
                #region gét nơi nhận của phiếu chưa tạo theo người đăng nhập
                long userId = _userAgentHelper.GetCurrentUserId();
                string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
                long phongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();


                long khoaId = 0;
                var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
                if (phongBenhVien != null)
                {
                    khoaId = phongBenhVien.KhoaPhongId;
                }
                var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
                #endregion
                var data = new
                {
                    LogoUrl = xacNhanInLinhDuocPhamXemTruoc?.Hosting + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = "",
                    MaVachPhieuLinh = "",
                    NoiGiao = tenPhong,
                    DienGiai = "",
                    TruongKhoaDuocVTYT = "",
                    NguoiGiao = "",
                    NguoiNhan = "",
                    TuNgay = xacNhanInLinhDuocPhamXemTruoc.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                    DenNgay = xacNhanInLinhDuocPhamXemTruoc.ThoiDiemLinhTongHopDenNgay == null ? DateTime.Now.ApplyFormatDateTimeSACH() : xacNhanInLinhDuocPhamXemTruoc.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                    TruongPhongKhoaPhong = "",
                    CongKhoan = objData.Index - 1,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //HeaderPhieuLinhThuoc = tenLoaiLinh,
                    ThuocHoacVatTu = objData.html,
                    KhoaPhong = "",
                    Ngay = DateTime.Now.Day,
                    Thang = DateTime.Now.Month,
                    Nam = DateTime.Now.Year,
                    //Gio = DateTime.Now.ApplyFormatTime()
                    Noinhan= tenKhoa
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            }
            if (listAllDuocPhamTheoListYeuCauDuocPhamIdHuongThanGayNghien.Any())
            {

                var objData = GetHTMLPhieuLinhBenhNhan(listAllDuocPhamTheoListYeuCauDuocPhamIdHuongThanGayNghien.ToList(),true);
                ThuocHoacVatTu = objData.html;
                string tenPhong = "";
                var khoLinh = _khoRepository.Table.FirstOrDefault(o => o.Id == xacNhanInLinhDuocPhamXemTruoc.KhoLinhId);
                if (khoLinh != null)
                {
                    tenPhong = khoLinh.Ten;
                }

                #region gét nơi nhận của phiếu chưa tạo theo người đăng nhập
                long userId = _userAgentHelper.GetCurrentUserId();
                string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
                long phongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();


                long khoaId = 0;
                var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
                if (phongBenhVien != null)
                {
                    khoaId = phongBenhVien.KhoaPhongId;
                }
                var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
                #endregion

                var data = new
                {
                    LogoUrl = xacNhanInLinhDuocPhamXemTruoc?.Hosting + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = "",
                    MaVachPhieuLinh = "",
                    NoiGiao = tenPhong,
                    DienGiai = "",
                    TruongKhoaDuocVTYT = "",
                    NguoiGiao = "",
                    NguoiNhan = "",
                    TuNgay = xacNhanInLinhDuocPhamXemTruoc.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                    DenNgay = xacNhanInLinhDuocPhamXemTruoc.ThoiDiemLinhTongHopDenNgay == null ? DateTime.Now.ApplyFormatDateTimeSACH() : xacNhanInLinhDuocPhamXemTruoc.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                    TruongPhongKhoaPhong = "",
                    CongKhoan = objData.Index - 1,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //HeaderPhieuLinhThuoc = tenLoaiLinh,
                    ThuocHoacVatTu = ThuocHoacVatTu,
                    KhoaPhong = "",
                    Ngay = DateTime.Now.Day,
                    Thang = DateTime.Now.Month,
                    Nam = DateTime.Now.Year,
                    //Gio = DateTime.Now.ApplyFormatTime()
                    NoiNhan = tenKhoa
                };
                contentGNHT = TemplateHelpper.FormatTemplateWithContentTemplate(resultGNHT.Body, data);
            }


            if (!string.IsNullOrEmpty(content))
            {
                content = headerTitile + content;
            }
            if (contentGNHT != "")
            {

                content = content + headerTitile + "<div style='break-after:page'></div>" + contentGNHT;
            }
            return content;
        }
        public async Task XuLyThemYeuCauLinhDuocPhamTTAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhTtDuocPham, List<long> yeuCauDuocPhamIds)
        {
            var yeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.Table.Where(o => yeuCauDuocPhamIds.Contains(o.Id)).ToList();
            if (yeuCauDuocPhamBenhViens.Any(o => o.YeuCauLinhDuocPhamId != null))
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }
            foreach (var yc in yeuCauDuocPhamBenhViens)
            {
                yeuCauLinhTtDuocPham.YeuCauDuocPhamBenhViens.Add(yc);
            }
        }

        // update 
        public async Task<List<ThongTinLanKhamKho>> GetDataThuocAsync(long yeuCauTiepNhanId, long phongBenhVienId, long nhanVienLogin, long khoLinhId)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();

            var queryYeuCauKhamBenh = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
            .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                        //x.LaDuocPhamBHYT == true &&
                        x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                        x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                         phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                        x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                        x.KhoLinhId == khoLinhId
                        )
            .Select(p => new ThongTinLanKhamKho()
            {
                MaTN = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                HoTen = p.YeuCauTiepNhan.HoTen,
                DVKham = p.YeuCauKhamBenhId != null ? p.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : p.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? p.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                SLKe = (int)p.SoLuong,
                NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                DuocDuyet = p.YeuCauLinhDuocPhamId == null ? false : p.YeuCauLinhDuocPham.DuocDuyet == true ? true : false,
                TenDuocPham = p.Ten,
                NongDoVaHamLuong = p.HamLuong,
                HoatChat = p.HoatChat,
                DuongDung = p.DuongDung.Ten,
                DonViTinh = p.DonViTinh.Ten,
                HangSX = p.NhaSanXuat,
                NuocSanXuat = p.NuocSanXuat,
                SLYeuCau = p.SoLuong,
                LoaiThuoc = p.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT",
            });
            var item = 1;
            var thongTinLanKhamKho = new List<ThongTinLanKhamKho>();
            foreach (var itemx in queryYeuCauKhamBenh)
            {
                itemx.STT = item++;
                thongTinLanKhamKho.Add(itemx);
            }
            return thongTinLanKhamKho;
        }
        #region update ds cho goi 30072021
        public List<ThongTinLinhTuKhoGridVo> GetGridChoGoi(long yeuCauLinhDuocPhamId, string dateSearchStart, string dateSearchEnd)
        {
          
            var yeuCauDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId && 
                                                                                        x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                        )
                                     .Select(s => new ThongTinLinhTuKhoGridVo()
                                     {
                                         Id = s.YeuCauTiepNhanId,
                                         TenDuocPham = s.Ten,
                                         NongDoVaHamLuong = s.HamLuong,
                                         HoatChat = s.HoatChat,
                                         DuongDung = s.DuongDung.Ten,
                                         DonViTinh = s.DonViTinh.Ten,
                                         HangSX = s.NhaSanXuat,
                                         NuocSanXuat = s.NuocSanXuat,
                                         SLYeuCau = s.SoLuong,
                                         LoaiThuoc = s.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                                         DuocPhamId = s.DuocPhamBenhVienId,
                                         KhoLinhId = (long)s.KhoLinhId,
                                         MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                         MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                                         HoTen = s.YeuCauTiepNhan.HoTen,
                                         YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                         LoaiDuocPham = s.LaDuocPhamBHYT,
                                         SoLuongTon = s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == s.KhoLinhId && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                         NgayDieuTri = (s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTri != null) ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh, // ngày đăng ký
                                         NgayYeuCau = s.ThoiDiemChiDinh,// ngày chỉ định
                                         IsCheckRowItem = true,
                                         YeuCauLinhDuocPhamId =(long) s.YeuCauLinhDuocPhamId
                                     });
            var item = 1;
            var thongTinLinhTuKhoGridVo = new List<ThongTinLinhTuKhoGridVo>();
            var queryYeuCauDuocPhamBenhVien = yeuCauDuocPham.ToList();

            DateTime? tuNgay = null;
            DateTime? denNgay = null;

            if (!string.IsNullOrEmpty(dateSearchStart) && dateSearchStart != "null")
            {
                DateTime.TryParseExact(dateSearchStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgayTemp);
                //tuNgay = new DateTime(tuNgayTemp.Year, tuNgayTemp.Month, tuNgayTemp.Day, 0, 0, 0);
                tuNgay = tuNgayTemp;
            }
            if (!string.IsNullOrEmpty(dateSearchEnd) && dateSearchEnd != "null")
            {
                DateTime.TryParseExact(dateSearchEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgayTemp);
                //denNgay = new DateTime(denNgayTemp.Year, denNgayTemp.Month, denNgayTemp.Day, 23, 59, 59);
                denNgay = denNgayTemp;
            }

            var yeuCauDuocPhamGroup = yeuCauDuocPham.Where(p => (tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay))
                .GroupBy(x => new
                {
                    x.MaBN,
                    x.HoTen,
                    x.MaTN,
                    x.YeuCauTiepNhanId,
                    //x.LoaiDuocPham,
                    //x.TenDuocPham
                }).Select(s => new ThongTinLinhTuKhoGridVo()
                {
                    Id = s.First().Id,
                    TenDuocPham = s.First().TenDuocPham,
                    NongDoVaHamLuong = s.First().NongDoVaHamLuong,
                    HoatChat = s.First().HoatChat,
                    DuongDung = s.First().DuongDung,
                    DonViTinh = s.First().DonViTinh,
                    HangSX = s.First().HangSX,
                    NuocSanXuat = s.First().NuocSanXuat,
                    SLYeuCau = s.Sum(x => x.SLYeuCau),
                    LoaiThuoc = s.First().LoaiThuoc,
                    DuocPhamId = s.First().DuocPhamId,
                    KhoLinhId = s.First().KhoLinhId,
                    MaTN = s.First().MaTN,
                    MaBN = s.First().MaBN,
                    HoTen = s.First().HoTen,
                    YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                    LoaiDuocPham = s.First().LoaiDuocPham,
                    SoLuongTon = s.First().SoLuongTon,
                    NgayYeuCau = s.First().NgayYeuCau,
                    NgayDieuTri = s.First().NgayDieuTri,
                    IsCheckRowItem = s.First().IsCheckRowItem,
                    YeuCauLinhDuocPhamId = s.First().YeuCauLinhDuocPhamId
                }).ToList();
            List<ThongTinLinhTuKhoGridVo> list = new List<ThongTinLinhTuKhoGridVo>();
            foreach (var itemCha in yeuCauDuocPhamGroup)
            {
                itemCha.ListYeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
               .Where(x => x.YeuCauLinhDuocPhamId == itemCha.YeuCauLinhDuocPhamId && x.YeuCauTiepNhanId == itemCha.YeuCauTiepNhanId
                           )
               .Select(p => new ThongTinLanKhamKho()
               {
                   Id = p.Id,
                   MaTN = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                   MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                   HoTen = p.YeuCauTiepNhan.HoTen,
                   DVKham = p.YeuCauKhamBenhId != null ? p.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : p.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? p.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                   BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                   SLKe = (int)p.SoLuong,
                   NgayYeuCau = p.ThoiDiemChiDinh,
                   NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                   NgayDieuTri = (p.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && p.NoiTruPhieuDieuTri != null )? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                   DuocDuyet = p.YeuCauLinhDuocPhamId == null ? false : p.YeuCauLinhDuocPham.DuocDuyet == true ? true : false,
                   DuocPhamId = p.DuocPhamBenhVienId,
                   TenDuocPham = p.Ten,
                   NongDoVaHamLuong = p.HamLuong,
                   HoatChat = p.HoatChat,
                   DuongDung = p.DuongDung.Ten,
                   DonViTinh = p.DonViTinh.Ten,
                   HangSX = p.NhaSanXuat,
                   NuocSanXuat = p.NuocSanXuat,
                   SLYeuCau = p.SoLuong,
                   LoaiThuoc = p.LaDuocPhamBHYT == true ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT",
                   SoLuongTon = p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == p.KhoLinhId && nkct.LaDuocPhamBHYT == p.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                   IsCheckRowItem = true,
                   YeuCauTiepNhanId = p.YeuCauTiepNhanId
               }).Where(p => (tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay)).ToList();
                list.Add(itemCha);
            }
            return list;
        }
        public List<long> GetYeuCauDuocPhamIdDaTao(long yeuCauLinhDuocPhamId)
        {
            var queryYeuCauKhamBenhId= _yeuCauDuocPhamBenhVienRepository.TableNoTracking
            .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId && x.TrangThai == EnumYeuCauDuocPhamBenhVien.ChuaThucHien
                        )
              .Select(s => s.Id);
            return queryYeuCauKhamBenhId.ToList();
        }
        public async Task XuLyHuyYeuCauDuocPhamTTAsync(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDuocPhamBenhVien ycdpbv, List<long> ycdpbvs)
        {


        }
        public void XuLyHuyYeuCauDuocPhamTTAsync(List<long> ycdpbvs)
        {
            foreach(var item in ycdpbvs)
            {
                var ycdp = _yeuCauDuocPhamBenhVienRepository.GetById(item,s=>s.Include(d=>d.YeuCauLinhDuocPhamChiTiets));
                ycdp.YeuCauLinhDuocPhamId = null;
                if(ycdp.YeuCauLinhDuocPhamChiTiets.Select(d => d.Id).Any())
                {
                   _yeuCauLinhDuocPhamChiTietRepository.DeleteAsync(ycdp.YeuCauLinhDuocPhamChiTiets);
                }
                _yeuCauDuocPhamBenhVienRepository.Context.SaveChanges();
            }
        }
        #endregion
        private OBJList GetHTMLPhieuLinhBenhNhanTuChoi (List<DuocPhamLoaiQuanLyLinhTTGridVo> gridVos,bool loaiThuoc)
        {
            var index = 1;
            string sluongDaXuat = "";
            string ghiChu = "";
            string thuoc = "";
   
                foreach (var itemx in gridVos)
                {
                    if (itemx.SoLuongCoTheXuat == null)
                    {
                        sluongDaXuat = "";
                    }
                    else
                    {
                        sluongDaXuat = Convert.ToString(itemx.SoLuongCoTheXuat);
                    }
                thuoc = thuoc + "<tr style='border: 1px solid #020000;'>"
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            index++
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                           itemx.MaDuocPham
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                           itemx.TenDuocPham
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                           itemx.DonViTinh
                                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                           (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemx.SoLuong), false) : itemx.SoLuong.MathRoundNumber(2) + "")
                                             + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                            (loaiThuoc == true && itemx.SoLuongCoTheXuat != null ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(sluongDaXuat), false) : sluongDaXuat)
                                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                            "&nbsp;";
                                            //ghiChu; => để trống 14/04/2021
                    //itemx?.YeuCauLinhDuocPham.GhiChu;
                }
            var data = new OBJList
            {
                Index = index,
                html = thuoc
            };
            return data;
        }
        private OBJList GetHTMLPhieuLinhBenhNhan(List<ThongTinLanKhamKho> gridVos,bool loaiThuoc)
        {
            var index = 1;

            string yeuCau = ""; // to do
            var tenLoaiLinh = "";
            var donViTinh = "";
            string thuoc = "";

            var groupDuocPhamTrungNhau = gridVos.GroupBy(x => new
            {
                x.TenDuocPham,
                x.HoatChat,
                x.DonViTinh,
            })
            .Select(p => new ThongTinLanKhamKho()
            {
                BacSyKeToa = p.First().BacSyKeToa,
                DuocDuyet = p.First().DuocDuyet,
                LoaiThuoc = p.First().LoaiThuoc,
                TenDuocPham = p.First().TenDuocPham,
                HoatChat = p.First().HoatChat,
                NongDoVaHamLuong = p.First().NongDoVaHamLuong,
                DuongDung = p.First().DuongDung,
                HangSX = p.First().HangSX,
                NuocSanXuat = p.First().NuocSanXuat,
                SLYeuCau = p.Sum(s => s.SLYeuCau),
                NgayKe = p.First().NgayKe,
                GhiChu = p.Select(s => s.GhiChu).ToList().Distinct().Join(" ;"),
                LoaiPhieuLinh = p.First().LoaiPhieuLinh,
                DonViTinh = p.First().DonViTinh,
            }).OrderBy(d=>d.TenDuocPham).ToList();


            foreach (var itemx in groupDuocPhamTrungNhau.ToList())
            {
                if (itemx.DuocDuyet == true)
                {
                    yeuCau = Convert.ToString(itemx.SLYeuCau);
                }
                else if (itemx.DuocDuyet == null)
                {
                    yeuCau = "";
                }
                thuoc = thuoc + "<tr style='border: 1px solid #020000;'>"
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.HoatChat
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.TenDuocPham
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                       (loaiThuoc == true  ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemx.SLYeuCau), false) : itemx.SLYeuCau.MathRoundNumber(2) + "")
                                         + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                       (loaiThuoc == true &&  itemx.DuocDuyet == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemx.SLYeuCau), false) : yeuCau)
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        "&nbsp;";
                tenLoaiLinh = itemx.LoaiPhieuLinh;
                donViTinh = "";
            }
            var data = new OBJList
            {
                Index = index,
                html = thuoc
            };
            return data;
        }
        public string TenNoiNhanPhieuLinhTrucTiep(long noiYeuCauId)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == noiYeuCauId);
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            return tenKhoa;
        }
    }
}

