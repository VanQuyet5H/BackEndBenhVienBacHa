using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial class YeuCauLinhVatTuService
    {
        public string TenKhoCho(long IdDuocPham)
        {
            //var TenKho = _yeu.TableNoTracking.Include(p => p.KhoLinh).Where(x => x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan && x.DuocPhamBenhVienId == IdDuocPham).Select(x => x.KhoLinh.Ten);
            //return EnumLoaiPhieuLinh.LinhChoBenhNhan.GetDescription() + " " + '-' + " " + TenKho;
            return null;
        }
        public List<ThongTinLinhVatTuTuKhoGridVo> GetData(long idKhoLinh, long phongDangNhapId , string dateSearchStart, string dateSearchEnd)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;

            if (!string.IsNullOrEmpty(dateSearchStart) && dateSearchStart != "null")
            {
                DateTime.TryParseExact(dateSearchStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgayTemp);
                tuNgay = tuNgayTemp;
            }
            if (!string.IsNullOrEmpty(dateSearchEnd) && dateSearchEnd != "null")
            {
                DateTime.TryParseExact(dateSearchEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgayTemp);
                denNgay = denNgayTemp;

            }
            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();

            var queryYeuCauVatTuBenhViens = _yeuCauVatTuBenhVienRepository.TableNoTracking.Include(o => o.VatTuBenhVien)
               .ThenInclude(o => o.NhapKhoVatTuChiTiets).ThenInclude(p => p.NhapKhoVatTu)
               .Include(o => o.KhoLinh)
               .Where(x => x.KhoLinhId == idKhoLinh &&
                                      x.YeuCauLinhVatTuId == null &&
                                      x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                      phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                      x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Select(o => o);

            var yeuCauVT = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                     .Where(x => x.KhoLinhId == idKhoLinh &&
                                      x.YeuCauLinhVatTuId == null &&
                                      x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                      phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                      x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                                     .Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                                     {
                                         Id = s.YeuCauTiepNhanId,
                                         TenVatTu = s.Ten,
                                         HoatChat = s.Ma,
                                         DonViTinh = s.DonViTinh,
                                         HangSX = s.NhaSanXuat,
                                         NuocSanXuat = s.NuocSanXuat,
                                         SLYeuCau = s.SoLuong,
                                         LoaiThuoc = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                         VatTuId = s.VatTuBenhVienId,
                                         KhoLinhId = idKhoLinh,
                                         MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                         MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                                         HoTen = s.YeuCauTiepNhan.HoTen,
                                         YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                         NgayYeuCau = s.ThoiDiemChiDinh,
                                         NgayDieuTri = s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTri!=null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                                         IsCheckRowItem = false
                                     })
                                     .GroupBy(x => new
                                     {
                                         x.MaBN,
                                         x.HoTen,
                                         x.YeuCauTiepNhanId,
                                         x.MaTN
                                     }).Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                                     {
                                         Id = s.First().Id,
                                         TenVatTu = s.First().TenVatTu,
                                         HoatChat = s.First().HoatChat,
                                         DonViTinh = s.First().DonViTinh,
                                         HangSX = s.First().HangSX,
                                         NuocSanXuat = s.First().NuocSanXuat,
                                         SLYeuCau = s.Sum(x => x.SLYeuCau),
                                         LoaiThuoc = s.First().LoaiThuoc,
                                         VatTuId = s.First().VatTuId,
                                         KhoLinhId = idKhoLinh,
                                         MaTN = s.First().MaTN,
                                         MaBN = s.First().MaBN,
                                         HoTen = s.First().HoTen,
                                         YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                                         NgayYeuCau = s.First().NgayYeuCau,
                                         NgayDieuTris = s.Select(f=>f.NgayDieuTri).ToList(),
                                         IsCheckRowItem = s.First().IsCheckRowItem
                                     }).OrderBy(d => d.MaBN).ToList();

            var yeuCauVTs = yeuCauVT.Where(p => p.NgayDieuTris.Where(f => (tuNgay == null || f >= tuNgay) && (denNgay == null || f <= denNgay)).Any()).ToList();

            var dsMaYeuCauTiepNhan = yeuCauVTs.Select(o => o.MaTN).ToList();
            if (dsMaYeuCauTiepNhan.Any())
            {
                var listAllYCDPBV = queryYeuCauVatTuBenhViens.Where(o => dsMaYeuCauTiepNhan.Contains(o.YeuCauTiepNhan.MaYeuCauTiepNhan))
                     .Select(p => new
                     {
                         Id = p.Id,
                         MaTN = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                         MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                         HoTen = p.YeuCauTiepNhan.HoTen,
                         DVKham = p.YeuCauKhamBenhId != null ? p.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : p.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? p.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "Ghi nhận trong PTTT",
                         BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                         SLKe = (int)p.SoLuong,
                         NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                         DuocDuyet = p.YeuCauLinhVatTuId == null ? false : p.YeuCauLinhVatTu.DuocDuyet == true ? true : false,
                         TenVatTu = p.Ten,
                         HoatChat = p.Ma,
                         DonViTinh = p.DonViTinh,
                         HangSX = p.NhaSanXuat,
                         NuocSanXuat = p.NuocSanXuat,
                         SLYeuCau = p.SoLuong,
                         LoaiThuoc = p.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                         NgayYeuCau = p.ThoiDiemChiDinh,
                         //IsCheckRowItem = false,
                         VatTuId = p.VatTuBenhVienId,
                         YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                         SoLuongTon = p.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == p.KhoLinhId && nkct.LaVatTuBHYT == p.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                         NgayDieuTri = p.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && p.NoiTruPhieuDieuTri != null ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                         LaVatTuBHYT = p.LaVatTuBHYT,
                         KhoLinhId = idKhoLinh
                     }).Where(p => (tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay)).ToList();
                           
                if (listAllYCDPBV != null)
                {

                    var dsDuocPhamBenhVienId = listAllYCDPBV.Select(o => o.VatTuId).Distinct().ToList();
                    var dsKhoId = listAllYCDPBV.Select(o => o.KhoLinhId).Distinct().ToList();

                    var dsDuocPhamTrongKhoCanKiemTra = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(nkct => dsDuocPhamBenhVienId.Contains(nkct.VatTuBenhVienId) &&
                                 dsKhoId.Contains(nkct.NhapKhoVatTu.KhoId) && nkct.HanSuDung >= DateTime.Now)
                        .Select(nkct => new { nkct.VatTuBenhVienId, nkct.NhapKhoVatTu.KhoId, nkct.LaVatTuBHYT, nkct.SoLuongNhap, nkct.SoLuongDaXuat }).ToList();

                    foreach (var yeuCauTiepNhanCoLinhTrucTiep in yeuCauVTs)
                    {
                        // phải check thuốc của bệnh nhân theo kho . vì 1 bệnh nhân có thể kê 1 thuốc ở 2 kho khác nhau
                        var listYCDPBV = listAllYCDPBV.Where(o => o.MaTN == yeuCauTiepNhanCoLinhTrucTiep.MaTN && o.KhoLinhId == yeuCauTiepNhanCoLinhTrucTiep.KhoLinhId).OrderByDescending(d => d.SoLuongTon < d.SLKe ? 1 : 0).ThenBy(d => d.TenVatTu);

                        foreach (var yCDPBV in listYCDPBV)
                        {

                            var tonkho = dsDuocPhamTrongKhoCanKiemTra
                                .Where(nkct => nkct.VatTuBenhVienId == yCDPBV.VatTuId &&
                                     nkct.KhoId == yCDPBV.KhoLinhId && nkct.LaVatTuBHYT == yCDPBV.LaVatTuBHYT)
                                .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat);

                            yeuCauTiepNhanCoLinhTrucTiep.ListYeuCauVatTuBenhViens.Add(new ThongTinLanKhamKho
                            {
                                Id = yCDPBV.Id,
                                MaTN = yCDPBV.MaTN,
                                MaBN = yCDPBV.MaBN,
                                HoTen = yCDPBV.HoTen,
                                DVKham = yCDPBV.DVKham,
                                BacSyKeToa = yCDPBV.BacSyKeToa,
                                SLKe = yCDPBV.SLKe,
                                NgayKe = yCDPBV.NgayKe,
                                DuocDuyet = yCDPBV.DuocDuyet,
                                TenVatTu = yCDPBV.TenVatTu,
                                HoatChat = yCDPBV.HoatChat,
                                DonViTinh = yCDPBV.DonViTinh,
                                HangSX = yCDPBV.HangSX,
                                NuocSanXuat = yCDPBV.NuocSanXuat,
                                SLYeuCau = yCDPBV.SLYeuCau,
                                LoaiThuoc = yCDPBV.LoaiThuoc,
                                NgayYeuCau = yCDPBV.NgayYeuCau,
                                //IsCheckRowItem = false,
                                VatTuId = yCDPBV.VatTuId,
                                YeuCauTiepNhanId = yCDPBV.YeuCauTiepNhanId,
                                SoLuongTon = tonkho,
                                NgayDieuTri = yCDPBV.NgayDieuTri,
                                LaVatTuBHYT = yCDPBV.LaVatTuBHYT
                            });
                        }

                    }
                }
            }
            return yeuCauVTs;
        }
        public List<ThongTinLinhVatTuTuKhoGridVo> GetDataDaTao(long idKhoLinh, long idYeuCauLinhVT, long phongDangNhapId, long trangThai)
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


            // 10/11/2021 cập nhật lại xem chi tiết lĩnh trực tiếp 
            // chắc chắn có yêu cầu lĩnh vì view  chi tiết những yêu cầu đã tạo
            var kiemTraTrangThaiDuyetCuaPhieuLinh = _yeuCauLinhVatTuRepository.TableNoTracking.Where(d => d.Id == idYeuCauLinhVT).Select(d => d.DuocDuyet).First();




            var yeuCauLinhVTKhoXuatId =_yeuCauLinhVatTuRepository.TableNoTracking.Where(x => x.Id == idYeuCauLinhVT).Select(s => s.KhoXuatId).First();

            if (kiemTraTrangThaiDuyetCuaPhieuLinh == true) // trạng thái đã duyệt
            {
                var yeuCauVT = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(d=>d.YeuCauLinhVatTuId == idYeuCauLinhVT)
                      .Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                      {
                          Id = s.Id,
                          TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                          HoatChat = s.VatTuBenhVien.VatTus.Ma,
                          DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                          HangSX = s.VatTuBenhVien.VatTus.NhaSanXuat,
                          NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                          SLYeuCau = s.SoLuong,
                          LoaiThuoc = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",

                          VatTuId = s.VatTuBenhVienId,
                          KhoLinhId = idKhoLinh,
                          MaTN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                          MaBN = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                          HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                          YeuCauTiepNhanId = s.YeuCauVatTuBenhVien.YeuCauTiepNhanId,
                          DVKham = s.YeuCauVatTuBenhVien != null ? s.YeuCauVatTuBenhVien.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "Ghi nhận trong PTTT",
                          NgayDieuTri = (s.YeuCauVatTuBenhVien.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null) ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh,
                          LaVatTuBHYT = s.LaVatTuBHYT
                      }).GroupBy(x => new
                      {
                          x.MaBN,
                          x.HoTen
                      }).Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                      {
                          Id = s.First().Id,
                          TenVatTu = s.First().TenVatTu,
                          HoatChat = s.First().HoatChat,
                          DonViTinh = s.First().DonViTinh,
                          HangSX = s.First().HangSX,
                          NuocSanXuat = s.First().NuocSanXuat,
                          SLYeuCau = s.Sum(x => x.SLYeuCau),
                          LoaiThuoc = s.First().LoaiThuoc,
                          VatTuId = s.First().VatTuId,
                          KhoLinhId = idKhoLinh,
                          MaTN = s.First().MaTN,
                          MaBN = s.First().MaBN,
                          HoTen = s.First().HoTen,
                          YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                          DVKham = s.First().DVKham,
                          NgayDieuTri = s.First().NgayDieuTri,
                          SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                             .Where(x => x.VatTuBenhVienId == s.First().VatTuId
                              && x.NhapKhoVatTu.KhoId == yeuCauLinhVTKhoXuatId
                              && x.NhapKhoVatTu.DaHet != true
                              && x.LaVatTuBHYT == s.First().LaVatTuBHYT
                              && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                      });
                return yeuCauVT.OrderBy(d=>d.LaVatTuBHYT).ThenBy(d=>d.MaTN).ThenBy(d=>d.TenVatTu).ToList();
            }
            else if (kiemTraTrangThaiDuyetCuaPhieuLinh == null) // đang chờ  duyệt
            {

                var yeuCauVT = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(x => x.KhoLinhId == idKhoLinh &&
                                                                                     x.YeuCauLinhVatTuId == idYeuCauLinhVT &&
                                                                                     x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                                                                                     //phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                                                     x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                                    //phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                    .Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                                    {
                                        Id = s.Id,
                                        TenVatTu = s.Ten,
                                        HoatChat = s.Ma,
                                        DonViTinh = s.DonViTinh,
                                        HangSX = s.NhaSanXuat,
                                        NuocSanXuat = s.NuocSanXuat,
                                        SLYeuCau = s.SoLuong,
                                        LoaiThuoc = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                                        
                                        VatTuId = s.VatTuBenhVienId,
                                        KhoLinhId = idKhoLinh,
                                        MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                        MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                                        HoTen = s.YeuCauTiepNhan.HoTen,
                                        YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                        DVKham = s.YeuCauKhamBenhId != null ? s.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : s.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "Ghi nhận trong PTTT",
                                        NgayDieuTri = (s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTri !=null)? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                                        LaVatTuBHYT = s.LaVatTuBHYT
                                    }).GroupBy(x => new
                                    {
                                        x.MaBN,
                                        x.HoTen
                                    }).Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                                    {
                                        Id = s.First().Id,
                                        TenVatTu = s.First().TenVatTu,
                                        HoatChat = s.First().HoatChat,
                                        DonViTinh = s.First().DonViTinh,
                                        HangSX = s.First().HangSX,
                                        NuocSanXuat = s.First().NuocSanXuat,
                                        SLYeuCau = s.Sum(x => x.SLYeuCau),
                                        LoaiThuoc = s.First().LoaiThuoc,
                                        VatTuId = s.First().VatTuId,
                                        KhoLinhId = idKhoLinh,
                                        MaTN = s.First().MaTN,
                                        MaBN = s.First().MaBN,
                                        HoTen = s.First().HoTen,
                                        YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                                        DVKham = s.First().DVKham,
                                        NgayDieuTri = s.First().NgayDieuTri,
                                        SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                           .Where(x => x.VatTuBenhVienId == s.First().VatTuId
                                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVTKhoXuatId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == s.First().LaVatTuBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                                    });

                return yeuCauVT.OrderBy(d => d.LaVatTuBHYT).ThenBy(d => d.MaTN).ThenBy(d => d.TenVatTu).ToList();
            }
            else if (kiemTraTrangThaiDuyetCuaPhieuLinh == false) // từ chối
            {
                var yeuCauVatTu = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.YeuCauLinhVatTuId == idYeuCauLinhVT)
                                    .Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                                    {
                                        Id = s.Id,
                                        TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                                        HoatChat = s.VatTuBenhVien.VatTus.Ma,
                                        DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                                        HangSX = s.VatTuBenhVien.VatTus.NhaSanXuat,
                                        NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
                                        SLYeuCau = s.SoLuong,
                                        LoaiThuoc = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                                        VatTuId = s.VatTuBenhVienId,
                                        KhoLinhId = idKhoLinh,
                                        NgayDieuTriTuChoi = s.YeuCauVatTuBenhVien != null ? (s.YeuCauVatTuBenhVien.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null) ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri.ApplyFormatDateTimeSACH() : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH():"",
                                        DVKham = s.YeuCauVatTuBenhVien != null ? s.YeuCauVatTuBenhVien.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "Ghi nhận trong PTTT",
                                        NgayKe = s.YeuCauVatTuBenhVien != null ? s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH():""
                                    });

                return yeuCauVatTu.OrderBy(d => d.LaVatTuBHYT).ThenBy(d => d.TenVatTu).ToList();
            }

            return null;







            //if (trangThai == 1)
            //{
            //    var yeuCauVT = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(x => x.KhoLinhId == idKhoLinh &&
            //                                                                         x.YeuCauLinhVatTuId == idYeuCauLinhVT &&
            //                                                                         x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
            //                                                                         //phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
            //                                                                         x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
            //                                   //phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
            //                        .Select(s => new ThongTinLinhVatTuTuKhoGridVo()
            //                        {
            //                            Id = s.Id,
            //                            TenVatTu = s.Ten,
            //                            HoatChat = s.Ma,
            //                            DonViTinh = s.DonViTinh,
            //                            HangSX = s.NhaSanXuat,
            //                            NuocSanXuat = s.NuocSanXuat,
            //                            SLYeuCau = s.SoLuong,
            //                            LoaiThuoc = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
            //                            VatTuId = s.VatTuBenhVienId,
            //                            KhoLinhId = idKhoLinh,
            //                            MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //                            MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
            //                            HoTen = s.YeuCauTiepNhan.HoTen,
            //                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
            //                            DVKham = s.YeuCauKhamBenhId != null ? s.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : s.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "Ghi nhận trong PTTT",
            //                        }).GroupBy(x => new
            //                        {
            //                            x.MaBN,
            //                            x.HoTen
            //                        }).Select(s => new ThongTinLinhVatTuTuKhoGridVo()
            //                        {
            //                            Id = s.First().Id,
            //                            TenVatTu = s.First().TenVatTu,
            //                            HoatChat = s.First().HoatChat,
            //                            DonViTinh = s.First().DonViTinh,
            //                            HangSX = s.First().HangSX,
            //                            NuocSanXuat = s.First().NuocSanXuat,
            //                            SLYeuCau = s.Sum(x => x.SLYeuCau),
            //                            LoaiThuoc = s.First().LoaiThuoc,
            //                            VatTuId = s.First().VatTuId,
            //                            KhoLinhId = idKhoLinh,
            //                            MaTN = s.First().MaTN,
            //                            MaBN = s.First().MaBN,
            //                            HoTen = s.First().HoTen,
            //                            YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
            //                            DVKham = s.First().DVKham
            //                        });

            //    return yeuCauVT.ToList();
            //}
            //else
            //{
            //    var yeuCauVatTu = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(x => x.YeuCauLinhVatTuId == idYeuCauLinhVT)
            //                        .Select(s => new ThongTinLinhVatTuTuKhoGridVo()
            //                        {
            //                            Id = s.Id,
            //                            TenVatTu = s.VatTuBenhVien.VatTus.Ten,
            //                            HoatChat = s.VatTuBenhVien.VatTus.Ma,
            //                            DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
            //                            HangSX = s.VatTuBenhVien.VatTus.NhaSanXuat,
            //                            NuocSanXuat = s.VatTuBenhVien.VatTus.NuocSanXuat,
            //                            SLYeuCau = s.SoLuong,
            //                            LoaiThuoc = s.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
            //                            VatTuId = s.VatTuBenhVienId,
            //                            KhoLinhId = idKhoLinh,
            //                        });

            //    return yeuCauVatTu.ToList();
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

            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Select(s => new ThongTinLinhTuKho()
                {
                    LinhVePhongId = phongLamViecId,
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
            long phongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
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
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(s => s.YeuCauLinhVatTuId == idYeuCauLinh);
            if (query.Any())
            {
                var queryChuaDuyetVaDaDuyet = BaseRepository.TableNoTracking.Where(s => s.Id == idYeuCauLinh).Select(s => new ThongTinLinhTuKho()
                {
                    Id = s.Id,
                    LinhVePhongId = s.KhoNhapId,
                    LinhVeKhoa = s.NoiYeuCauId != null ? s.NoiYeuCau.KhoaPhong.Ten : "",
                    NoiChiDinhId = s.YeuCauVatTuBenhViens.First().NoiChiDinh.Id,
                    LinhVePhong = s.YeuCauVatTuBenhViens.First().NoiChiDinh.Ten,
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
                    LinhVePhong = s.NoiYeuCau.Ten,
                    LinhVeKhoa = s.NoiYeuCauId != null ? s.NoiYeuCau.KhoaPhong.Ten : "",
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

            if (!string.IsNullOrEmpty(querystring[4]) && querystring[4] != "null")
            {
                DateTime.TryParseExact(querystring[4], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgayTemp);
                tuNgay = new DateTime(tuNgayTemp.Year, tuNgayTemp.Month, tuNgayTemp.Day, 0, 0, 0);
            }
            if (!string.IsNullOrEmpty(querystring[5]) && querystring[5] != "null")
            {
                DateTime.TryParseExact(querystring[5], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgayTemp);
                denNgay = new DateTime(denNgayTemp.Year, denNgayTemp.Month, denNgayTemp.Day, 23, 59, 59);
            }
            var i = 1;
            var queryYeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking.Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan).Include(o => o.YeuCauKhamBenh).ThenInclude(o => o.DichVuKhamBenhBenhVien)
               .Where(x => x.YeuCauTiepNhanId == long.Parse(querystring[0]) &&
                           //x.LaVatTuBHYT == true &&
                           x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                           x.YeuCauLinhVatTuId == null &&
                           x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                           phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                           x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 &&
                           x.KhoLinhId == long.Parse(querystring[3])
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
                   DuocDuyet = p.YeuCauLinhVatTuId == null ? false : p.YeuCauLinhVatTu.DuocDuyet == true ? true : false,
                   TenVatTu = p.Ten,
                   HoatChat = p.Ma,
                   DonViTinh = p.DonViTinh,
                   HangSX = p.NhaSanXuat,
                   NuocSanXuat = p.NuocSanXuat,
                   SLYeuCau = p.SoLuong,
                   LoaiThuoc = p.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                   NgayYeuCau = p.ThoiDiemChiDinh,
                   NgayDieuTri = (p.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && p.NoiTruPhieuDieuTri != null) ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh
               }).Where(p => (tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay));
            // search ngày đăng ký
            var quaythuoc = queryYeuCauVatTu.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = queryYeuCauVatTu.Count();

            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
        }

        public List<LinhTrucTiepVatTuChiTietGridVo> GetDataForGridChiTietChildCreateAsync(long yeuCauVatTuBenhVienId)
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
            var queryYeuCauKhamBenh = _yeuCauVatTuBenhVienRepository.TableNoTracking
            .Where(x => x.Id == yeuCauVatTuBenhVienId && x.TrangThai ==  Enums.EnumYeuCauVatTuBenhVien.ChuaThucHien && x.YeuCauLinhVatTuId == null)
              .Select(s => new LinhTrucTiepVatTuChiTietGridVo()
              {
                  Id = yeuCauVatTuBenhVienId,
                  LaVatTuBHYT = s.LaVatTuBHYT,
                  VatTuBenhVienId = s.VatTuBenhVienId,
                  SoLuong = s.SoLuong,
                  //SLTon =
                  //              s.VatTuBenhVien.NhapKhoVatTuChiTiets
                  //                  .Where(nkct =>
                  //                      nkct.NhapKhoVatTu.KhoId == s.KhoLinhId &&
                  //                      nkct.LaVatTuBHYT == s.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now && s.VatTuBenhVien.VatTus.IsDisabled != true && s.VatTuBenhVien.HieuLuc == true)
                  //                  .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                  SLTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.VatTuBenhVienId == s.VatTuBenhVienId
                                        && x.NhapKhoVatTu.KhoId == s.KhoLinhId
                                        && x.LaVatTuBHYT == s.LaVatTuBHYT
                                        && x.NhapKhoVatTu.DaHet != true
                                        && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
              });
            return queryYeuCauKhamBenh.Where(s=>s.SLTon >= s.SoLuong).ToList();
        }

        public List<YeuCauVatTuBenhVienTT> GetPhieuLinhTrucTiepTT(long yeuCauTiepNhanId)
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
            var queryYeuCauKhamBenh = _yeuCauVatTuBenhVienRepository.TableNoTracking
           .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                             x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                             x.YeuCauLinhVatTuId == null &&
                             x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                             x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
              .Select(s => new YeuCauVatTuBenhVienTT()
              {
                  Id = s.Id,
                  YeuCauTiepNhanId = yeuCauTiepNhanId,
                  LaVatTuBHYT = s.LaVatTuBHYT,
                  TenThuoc = s.VatTuBenhVien.VatTus.Ten,
                  VatTuId = s.VatTuBenhVienId,
                  SoLuong = s.SoLuong,
                  SLTon =
                                s.VatTuBenhVien.NhapKhoVatTuChiTiets
                                    .Where(nkct =>
                                        nkct.NhapKhoVatTu.KhoId == s.KhoLinhId &&
                                        nkct.LaVatTuBHYT == s.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now && s.VatTuBenhVien.VatTus.IsDisabled != true && s.VatTuBenhVien.HieuLuc == true)
                                    .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
              });
            return queryYeuCauKhamBenh.Where(s => s.SLTon >= s.SoLuong).ToList();
        }

        // đã tạo
        public async Task<GridDataSource> GetAllYeuCauLinhVatTuTuKhoDaTao(QueryInfo queryInfo)
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
            var querystring = queryInfo.AdditionalSearchString.Split('-');
            var queryYeuCauVatTuBenhVien = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                                                 .Where(x => x.YeuCauTiepNhanId == long.Parse(querystring[0]) &&
                                                                             //phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                                                                             x.YeuCauLinhVatTuId == long.Parse(querystring[3]) &&
                                                                             x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                                                                             x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                 .Select(p => new ThongTinLanKhamKho()
                 {
                     MaTN = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                     MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                     HoTen = p.YeuCauTiepNhan.HoTen,
                     DVKham = p.YeuCauKhamBenhId != null ? p.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : p.YeuCauDichVuKyThuat.YeuCauKhamBenhId != null ? p.YeuCauDichVuKyThuat.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "Ghi nhận trong PTTT",
                     BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                     SLKe = (int)p.SoLuong,
                     NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                     DuocDuyet = p.YeuCauLinhVatTuId == null ? false : p.YeuCauLinhVatTu.DuocDuyet == true ? true : false,
                     TenVatTu = p.Ten,
                     HoatChat = p.Ma,
                     DonViTinh = p.DonViTinh,
                     HangSX = p.NhaSanXuat,
                     NuocSanXuat = p.NuocSanXuat,
                     SLYeuCau = p.SoLuong,
                     LoaiThuoc = p.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                     NgayDieuTri = (p.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && p.NoiTruPhieuDieuTri != null) ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh,
                     SoLuongTon = p.YeuCauLinhVatTu.DuocDuyet == true ? _nhapKhoVatTuChiTietRepository.TableNoTracking
                               .Where(x => x.VatTuBenhVienId == p.VatTuBenhVienId
                                           && x.NhapKhoVatTu.KhoId == long.Parse(querystring[4])
                                           && x.LaVatTuBHYT == p.LaVatTuBHYT
                                           && x.NhapKhoVatTu.DaHet != true
                                           && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat):
                                           _nhapKhoVatTuChiTietRepository.TableNoTracking
                               .Where(x => x.VatTuBenhVienId == p.VatTuBenhVienId
                                           && x.NhapKhoVatTu.KhoId == long.Parse(querystring[4])
                                           && x.LaVatTuBHYT == p.LaVatTuBHYT
                                           && x.NhapKhoVatTu.DaHet != true
                                           && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                 });
            var item = 1;
            var thongTinLanKhamKho = new List<ThongTinLanKhamKho>();
            foreach (var itemx in queryYeuCauVatTuBenhVien.ToList())
            {
                itemx.STT = item++;
                thongTinLanKhamKho.Add(itemx);
            }

            var data = thongTinLanKhamKho.OrderByDescending(d=>d.SoLuongTon < d.SLKe ? 1 : 0).ThenBy(d=>d.TenVatTu).ToList();
            var dataOrderBy = data.AsQueryable();
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
            var khoNhanVien = _khoNhanVienQuanLyRepository.TableNoTracking.Where(s => s.NhanVienId == idNhanVien  && ( s.Kho != null && s.Kho.LaKhoKSNK != true)).Select(x => x.KhoId);
            return khoNhanVien.FirstOrDefault();
        }
        public bool? GetTrangThaiDuyet(long id)
        {
            var duocDuyet = BaseRepository.TableNoTracking
               .Where(x => x.Id == id).Select(p => p.DuocDuyet).FirstOrDefault();
            return duocDuyet;
        }
        public DaDuyetVatTu GetDaDuyet(long IdYeuCauLinh)
        {
            var queryYCLDP = BaseRepository.TableNoTracking
                .Where(x => x.Id == IdYeuCauLinh).Select(p => new DaDuyetVatTu()
                {
                    NgayDuyet = p.NgayDuyet,
                    NguoiDuyet = p.NhanVienDuyetId != null ? p.NhanVienDuyet.User.HoTen : ""
                });

            return queryYCLDP.FirstOrDefault();
        }
        public async Task<string> InPhieuLinhTrucTiepVatTu(XacNhanInLinhVatTu xacNhanInLinhDuocPham)
        {
            var content = "";
            var ThuocHoacVatTu = " ";
            var index = 1;
            var result = _templateRepo.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuLinhVatTuTrucTiep"));
            var vatTu = "";

            if (xacNhanInLinhDuocPham.TrangThaiIn == false)
            {
                //var yeuCauLinhVatTuTuChoi = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Include(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)
                //                                                                                   .Include(p => p.YeuCauLinhVatTu).ThenInclude(x => x.KhoXuat)
                //                                                                                   .Include(p => p.YeuCauLinhVatTu).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                //                                                                                   .Where(s => s.YeuCauLinhVatTuId == xacNhanInLinhDuocPham.YeuCauLinhVatTuId);
                var yeuCauLinhVatTuTuChoi = BaseRepository.GetByIdAsync(xacNhanInLinhDuocPham.YeuCauLinhVatTuId,
                                                                s => s.Include(z => z.KhoNhap)
                                                                     .Include(z => z.KhoXuat)
                                                                     .Include(z => z.NhanVienYeuCau).ThenInclude(d => d.User)
                                                                     .Include(z => z.NhanVienDuyet).ThenInclude(d=>d.User)
                                                                     .Include(z => z.YeuCauLinhVatTuChiTiets).ThenInclude(k => k.VatTuBenhVien).ThenInclude(w => w.VatTus)
                                                                     .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(k => k.VatTuBenhVien).ThenInclude(w => w.VatTus)
                                                                     .Include(z => z.YeuCauLinhVatTuChiTiets));


                if (yeuCauLinhVatTuTuChoi.Result != null)
                {
                    var vt = yeuCauLinhVatTuTuChoi.Result.YeuCauLinhVatTuChiTiets
                                                              .Select(o => new VatTuGridVo
                                                              {
                                                                  MaVatTu = o.VatTuBenhVien.Ma,
                                                                  TenVatTu= o.VatTuBenhVien.VatTus.Ten + (o.VatTuBenhVien.VatTus.NhaSanXuat != null && o.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                                                                                               (o.VatTuBenhVien.VatTus.NuocSanXuat != null && o.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + o.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                                                  SoLuong = o.SoLuong,
                                                                  DonViTinh = o.VatTuBenhVien.VatTus.DonViTinh,
                                                                  SoLuongCoTheXuat = o.SoLuongCoTheXuat,
                                                                  LaVatTuBHYT = o.LaVatTuBHYT
                                                              }).GroupBy(xy => new { xy.MaVatTu, xy.TenVatTu, xy.DonViTinh })
                                                            .Select(o => new VatTuGridVo
                                                            {
                                                                MaVatTu = o.First().MaVatTu,
                                                                TenVatTu = o.First().TenVatTu,
                                                                SoLuong = o.Sum(s => s.SoLuong),
                                                                DonViTinh = o.First().DonViTinh,
                                                                SoLuongCoTheXuat = o.Sum(s => s.SoLuongCoTheXuat),
                                                                LaVatTuBHYT = o.First().LaVatTuBHYT
                                                            }).OrderBy(d => d.TenVatTu);
                    
                    var objData = GetHTMLLinhTaoBenhNhanTuChoi(vt.ToList());
                  
                    index = objData.Index;
                    ThuocHoacVatTu = objData.html;

                    var maVachPhieuLinh = yeuCauLinhVatTuTuChoi.Result.SoPhieu;
                    var data = new
                    {
                        LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhVatTuTuChoi.Result.SoPhieu) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                        MaVachPhieuLinh = maVachPhieuLinh,
                        NoiGiao = yeuCauLinhVatTuTuChoi.Result.KhoXuat.Ten,
                        DienGiai = yeuCauLinhVatTuTuChoi.Result.GhiChu,
                        TruongKhoaDuocVTYT = "",
                        NguoiGiao = yeuCauLinhVatTuTuChoi?.Result?.NhanVienDuyet?.User?.HoTen,
                        NguoiNhan = yeuCauLinhVatTuTuChoi.Result?.NhanVienYeuCau?.User?.HoTen,
                        TuNgay = yeuCauLinhVatTuTuChoi?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                        DenNgay = yeuCauLinhVatTuTuChoi?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(),
                        TruongPhongKhoaPhong = "",
                        CongKhoan = index - 1,
                        NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                        ThuocHoacVatTu = ThuocHoacVatTu,
                        KhoaPhong = yeuCauLinhVatTuTuChoi?.Result?.KhoNhap?.Ten,
                        Ngay = DateTime.Now.Day,
                        Thang = DateTime.Now.Month,
                        Nam = DateTime.Now.Year,
                        //Gio = DateTime.Now.ApplyFormatTime()
                        NoiNhan=TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhVatTuTuChoi?.Result?.NoiYeuCauId)
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                }
              
                return content;
            }
            else
            {
                var yeuCauLinhDuocPham = BaseRepository.GetByIdAsync(xacNhanInLinhDuocPham.YeuCauLinhVatTuId,
                                                                s => s.Include(z => z.KhoNhap)
                                                                     .Include(z => z.KhoXuat)
                                                                     .Include(z => z.NhanVienYeuCau)
                                                                     .Include(z => z.NhanVienDuyet)
                                                                     .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(k=>k.VatTuBenhVien).ThenInclude(z=>z.VatTus)
                                                                     .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(k => k.NhanVienChiDinh).ThenInclude(z => z.User)
                                                                     .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(k=>k.YeuCauTiepNhan)
                                                                     .Include(z => z.YeuCauLinhVatTuChiTiets));
                if (yeuCauLinhDuocPham.Result != null)
                {
                    var yeucau = ""; // to do
                    var thucChat = 0; // to do
                    var tenLoaiLinh = "";
                    var donViTinh = "";
                    if (yeuCauLinhDuocPham.Result.YeuCauVatTuBenhViens.Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan && s.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && s.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Any())
                    {
                        var vt = yeuCauLinhDuocPham.Result.YeuCauVatTuBenhViens
                                                       .Where(s => s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan && 
                                                               s.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2
                                                               && s.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                                               )
                                                        .Select(p => new 
                                                        {
                                                            NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                                            DuocDuyet = p.YeuCauLinhVatTuId == null ? false : p.YeuCauLinhVatTu.DuocDuyet == true ? true : false,
                                                            BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                                                            HoatChat = p.VatTuBenhVien.Ma, // MÃ DƯỢC PHẨM K PHẢI MÃ HOẠT CHẤT
                                                            DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                            HangSX = p.NhaSanXuat,
                                                            NuocSanXuat = p.NuocSanXuat,
                                                            SLYeuCau = p.SoLuong,
                                                            LoaiVatTu = p.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                                                            VatTuId = p.VatTuBenhVienId,
                                                            LaVatTuBHYT = p.LaVatTuBHYT,
                                                            GhiChu = p.GhiChu,
                                                            LoaiPhieuLinh = p.LoaiPhieuLinh.GetDescription(),
                                                            TenVatTu = p.VatTuBenhVien.VatTus.Ten + (p.VatTuBenhVien.VatTus.NhaSanXuat != null && p.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + p.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                                                                                         (p.VatTuBenhVien.VatTus.NuocSanXuat != null && p.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + p.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                                        })
                           .GroupBy(x => new
                           {
                               x.VatTuId,
                               x.LaVatTuBHYT,
                               x.HoatChat,
                               x.DonViTinh,
                               x.NuocSanXuat,
                               x.NgayKe
                           })
                           .Select(p => new 
                           {
                               BacSyKeToa = p.First().BacSyKeToa,
                               DuocDuyet = p.First().DuocDuyet,
                               LoaiVatTu = p.First().LoaiVatTu,
                               TenVatTu = p.First().TenVatTu,
                               HoatChat = p.First().HoatChat,
                               HangSX = p.First().HangSX,
                               NuocSanXuat = p.First().NuocSanXuat,
                               SLYeuCau = p.Sum(s => s.SLYeuCau),
                               NgayKe = p.First().NgayKe,
                               GhiChu = p.First().GhiChu,
                               LoaiPhieuLinh = p.First().LoaiPhieuLinh,
                               DonViTinh = p.First().DonViTinh,
                               LaVatTuBHYT = p.First().LaVatTuBHYT
                           }).OrderBy(d => d.TenVatTu).ToList();

                       
                        var groupTrungBHYT = vt.GroupBy(x => new
                        {
                            x.TenVatTu,
                            x.HoatChat,
                            x.DonViTinh,
                        })
                           .Select(p => new
                           {
                               BacSyKeToa = p.First().BacSyKeToa,
                               DuocDuyet = p.First().DuocDuyet,
                               LoaiVatTu = p.First().LoaiVatTu,
                               TenVatTu = p.First().TenVatTu,
                               HoatChat = p.First().HoatChat,
                               HangSX = p.First().HangSX,
                               NuocSanXuat = p.First().NuocSanXuat,
                               SLYeuCau = p.Sum(s => s.SLYeuCau),
                               NgayKe = p.First().NgayKe,
                               GhiChu = p.First().GhiChu,
                               LoaiPhieuLinh = p.First().LoaiPhieuLinh,
                               DonViTinh = p.First().DonViTinh,
                               LaVatTuBHYT = p.First().LaVatTuBHYT
                           }).OrderBy(d => d.TenVatTu).ToList();
                        foreach (var itemx in groupTrungBHYT.ToList())
                        {
                            vatTu = vatTu + "<tr style='border: 1px solid #020000;'>"
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                    index++
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                   itemx.HoatChat
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                    itemx.TenVatTu
                                                    + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                    itemx.DonViTinh
                                                    + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                    itemx.SLYeuCau
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                    thucChat
                                                    + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" + "&nbsp;";//itemx?.GhiChu; => để trống 14/04/2021
                            tenLoaiLinh = itemx.LoaiPhieuLinh;
                            donViTinh = "";
                        }
                        ThuocHoacVatTu = vatTu;
                    }
                   
                }
                var maVachPhieuLinh = yeuCauLinhDuocPham.Result.SoPhieu.ToString();
                var data = new
                {
                    LogoUrl = xacNhanInLinhDuocPham.Hosting + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauLinhDuocPham.Result.SoPhieu.ToString()) ? BarcodeHelper.GenerateBarCode(maVachPhieuLinh) : "",
                    MaVachPhieuLinh = maVachPhieuLinh,
                    NoiGiao = yeuCauLinhDuocPham?.Result?.KhoXuat?.Ten,
                    DienGiai = yeuCauLinhDuocPham?.Result?.GhiChu,
                    TruongKhoaDuocVTYT = "",
                    NguoiGiao = yeuCauLinhDuocPham?.Result?.NhanVienDuyet?.User?.HoTen,
                    NguoiNhan = yeuCauLinhDuocPham.Result?.NhanVienYeuCau?.User?.HoTen,
                    TuNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                    DenNgay = yeuCauLinhDuocPham?.Result?.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH(), 
                    TruongPhongKhoaPhong = "",
                    CongKhoan = index - 1,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                    ThuocHoacVatTu = ThuocHoacVatTu,
                    KhoaPhong = yeuCauLinhDuocPham?.Result?.KhoNhap?.Ten,
                    Ngay = DateTime.Now.Day,
                    Thang = DateTime.Now.Month,
                    Nam = DateTime.Now.Year,
                    Gio = DateTime.Now.ApplyFormatTime(),
                    NoiNhan = TenNoiNhanPhieuLinhTrucTiep((long)yeuCauLinhDuocPham?.Result?.NoiYeuCauId)
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                return content;
            }
            return null;


        }
        public async Task<string> InXemtruocPhieuLinhTrucTiepVatTu(XacNhanInLinhVatTuXemTruoc xacNhanInLinhVatTuXemTruoc)
        {
            var content = "";
            var ThuocHoacVatTu = " ";
            var index = 1;
            var result = _templateRepo.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuLinhVatTuTrucTiep"));
            var vatTu = "";
            var queryList = new List<InXemTruocLinhTTGridVo>();
            if(xacNhanInLinhVatTuXemTruoc.YeuCauVatTuBenhVienIds.Any())
            {
                var yeuCauVTBV = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                     .Where(s => xacNhanInLinhVatTuXemTruoc.YeuCauVatTuBenhVienIds.Contains(s.Id) && s.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                                                  .Select(p => new InXemTruocLinhTTGridVo
                                                  {
                                                      NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                                      DuocDuyet = p.YeuCauLinhVatTuId == null ? false : p.YeuCauLinhVatTu.DuocDuyet == true ? true : false,
                                                      BacSyKeToa = p.NhanVienChiDinh.User.HoTen,
                                                      HoatChat = p.VatTuBenhVien.Ma, // MÃ DƯỢC PHẨM K PHẢI MÃ HOẠT CHẤT
                                                      DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                      HangSX = p.NhaSanXuat,
                                                      NuocSanXuat = p.NuocSanXuat,
                                                      SLYeuCau = p.SoLuong,
                                                      LoaiVatTu = p.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                                                      VatTuId = p.VatTuBenhVienId,
                                                      LaVatTuBHYT = p.LaVatTuBHYT,
                                                      GhiChu = p.GhiChu,
                                                      LoaiPhieuLinh = p.LoaiPhieuLinh.GetDescription(),
                                                      TenVatTu = p.VatTuBenhVien.VatTus.Ten + (p.VatTuBenhVien.VatTus.NhaSanXuat != null && p.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + p.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                                                                                   (p.VatTuBenhVien.VatTus.NuocSanXuat != null && p.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + p.VatTuBenhVien.VatTus.NuocSanXuat : "")
                                                  })
                     .GroupBy(x => new
                     {
                         x.VatTuId,
                         x.LaVatTuBHYT,
                         x.HoatChat,
                         x.DonViTinh,
                         x.NuocSanXuat,
                         x.NgayKe
                     })
                     .Select(p => new InXemTruocLinhTTGridVo
                     {
                         BacSyKeToa = p.First().BacSyKeToa,
                         DuocDuyet = p.First().DuocDuyet,
                         LoaiVatTu = p.First().LoaiVatTu,
                         TenVatTu = p.First().TenVatTu,
                         HoatChat = p.First().HoatChat,
                         HangSX = p.First().HangSX,
                         NuocSanXuat = p.First().NuocSanXuat,
                         SLYeuCau = p.Sum(s => s.SLYeuCau),
                         NgayKe = p.First().NgayKe,
                         GhiChu = p.First().GhiChu,
                         LoaiPhieuLinh = p.First().LoaiPhieuLinh,
                         DonViTinh = p.First().DonViTinh,
                         LaVatTuBHYT = p.First().LaVatTuBHYT
                       }).OrderBy(d => d.TenVatTu).ToList();
                queryList.AddRange(yeuCauVTBV);
            }
           
          
            if (queryList.Any())
            {
                var yeucau = ""; // to do
                var thucChat = 0; // to do
                var tenLoaiLinh = "";
                var donViTinh = "";
                var groupTrungBHYT = queryList.GroupBy(x => new
                {
                    x.TenVatTu,
                    x.HoatChat,
                    x.DonViTinh,
                })
                 .Select(p => new
                 {
                     BacSyKeToa = p.First().BacSyKeToa,
                     DuocDuyet = p.First().DuocDuyet,
                     LoaiVatTu = p.First().LoaiVatTu,
                     TenVatTu = p.First().TenVatTu,
                     HoatChat = p.First().HoatChat,
                     HangSX = p.First().HangSX,
                     NuocSanXuat = p.First().NuocSanXuat,
                     SLYeuCau = p.Sum(s => s.SLYeuCau),
                     NgayKe = p.First().NgayKe,
                     GhiChu = p.First().GhiChu,
                     LoaiPhieuLinh = p.First().LoaiPhieuLinh,
                     DonViTinh = p.First().DonViTinh,
                     LaVatTuBHYT = p.First().LaVatTuBHYT
                 }).OrderBy(p => p.TenVatTu).ToList();
                if(groupTrungBHYT.Any())
                {
                    foreach (var itemx in groupTrungBHYT)
                    {
                        vatTu = vatTu + "<tr style='border: 1px solid #020000;'>"
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                index++
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                               itemx.HoatChat
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                itemx.TenVatTu
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                itemx.DonViTinh
                                                + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                itemx.SLYeuCau
                                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                ""
                                                + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                               "&nbsp;";
                        tenLoaiLinh = itemx.LoaiPhieuLinh;
                        donViTinh = "";
                    }
                    ThuocHoacVatTu = vatTu;
                }
            }
            ThuocHoacVatTu = vatTu;
            string tenPhong = "";
            var khoLinh = _khoRepository.Table.FirstOrDefault(o => o.Id == xacNhanInLinhVatTuXemTruoc.KhoLinhId);
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
                LogoUrl = xacNhanInLinhVatTuXemTruoc.Hosting + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 =  "",
                MaVachPhieuLinh = "",
                NoiGiao = tenPhong,
                DienGiai = "",
                TruongKhoaDuocVTYT = "",
                NguoiGiao = "",
                NguoiNhan = "",
                TuNgay = xacNhanInLinhVatTuXemTruoc.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDateTimeSACH(),
                DenNgay = xacNhanInLinhVatTuXemTruoc.ThoiDiemLinhTongHopDenNgay != null ? xacNhanInLinhVatTuXemTruoc.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDateTimeSACH() :DateTime.Now.ApplyFormatDateTimeSACH(),
                TruongPhongKhoaPhong = "",
                CongKhoan = index - 1,
                NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                ThuocHoacVatTu = ThuocHoacVatTu,
                KhoaPhong = "",
                Ngay = DateTime.Now.Day,
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                Gio = DateTime.Now.ApplyFormatTime(),
                NoiNhan = tenKhoa
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        public async Task XuLyThemYeuCauLinhVatTuTTAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhVT, List<long> yeuCauVatTuIds)
        {
            var yeuCauVatTuBenhViens = _yeuCauVatTuBenhVienRepository.Table.Where(o => yeuCauVatTuIds.Contains(o.Id)).ToList();
            if (yeuCauVatTuBenhViens.Any(o => o.YeuCauLinhVatTuId != null))
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }
            foreach (var yc in yeuCauVatTuBenhViens)
            {
                yeuCauLinhVT.YeuCauVatTuBenhViens.Add(yc);
            }
        }
        private OBJListVatTu GetHTMLLinhTaoBenhNhanTuChoi(List<VatTuGridVo> gridVos)
        {
            var sluongDaXuat = string.Empty;
            var vt = string.Empty;
            var index = 1;
            var ghiChu = string.Empty;
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
                vt = vt + "<tr style='border: 1px solid #020000;'>"
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        index++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.MaVatTu
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        itemx.TenVatTu
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                       itemx.DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        itemx.SoLuong
                                         + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        sluongDaXuat
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" + "&nbsp;";//ghiChu; => để trống 14/04/2021
            }
            var data = new OBJListVatTu { 
                html = vt,
                Index = index
            };
            return data;
        }
        #region update ds cho goi 30072021
        public List<ThongTinLinhVatTuTuKhoGridVo> GetGridChoGoi(long yeuCauLinhVatTuId, string dateSearchStart, string dateSearchEnd)
        {

            var yeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId &&
                                                                                        x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                                                                        )
                                     .Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                                     {
                                         Id = s.YeuCauTiepNhanId,
                                         TenVatTu = s.Ten,
                                         HoatChat = s.Ma,
                                         DonViTinh = s.DonViTinh,
                                         HangSX = s.NhaSanXuat,
                                         NuocSanXuat = s.NuocSanXuat,
                                         SLYeuCau = s.SoLuong,
                                         LoaiThuoc = s.LaVatTuBHYT == true ? "BHYT" : "Không BHYT",
                                         VatTuId = s.VatTuBenhVienId,
                                         KhoLinhId = (long)s.KhoLinhId,
                                         MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                         MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                                         HoTen = s.YeuCauTiepNhan.HoTen,
                                         YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                         NgayYeuCau = s.ThoiDiemChiDinh,
                                         SoLuongTon = s.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == s.KhoLinhId && nkct.LaVatTuBHYT == s.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                         NgayDieuTri = (s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && s.NoiTruPhieuDieuTri != null)? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                                         IsCheckRowItem = true,
                                         YeuCauLinhVatTuId = (long)s.YeuCauLinhVatTuId
                                     });
            var item = 1;
            var thongTinLinhTuKhoGridVo = new List<ThongTinLinhVatTuTuKhoGridVo>();
            var queryYeuCauVatTuBenhVien = yeuCauVatTu.ToList();

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

            var yeuCauDuocPhamGroup = yeuCauVatTu.Where(p => (tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay))
                .GroupBy(x => new
                {
                    x.MaBN,
                    x.HoTen,
                    x.MaTN,
                    x.YeuCauTiepNhanId,
                    //x.LoaiDuocPham,
                    //x.TenDuocPham
                }).Select(s => new ThongTinLinhVatTuTuKhoGridVo()
                {
                    Id = s.First().Id,
                    TenVatTu = s.First().TenVatTu,
                    NongDoVaHamLuong = s.First().NongDoVaHamLuong,
                    HoatChat = s.First().HoatChat,
                    DuongDung = s.First().DuongDung,
                    DonViTinh = s.First().DonViTinh,
                    HangSX = s.First().HangSX,
                    NuocSanXuat = s.First().NuocSanXuat,
                    SLYeuCau = s.Sum(x => x.SLYeuCau),
                    LoaiThuoc = s.First().LoaiThuoc,
                    VatTuId = s.First().VatTuId,
                    KhoLinhId = s.First().KhoLinhId,
                    MaTN = s.First().MaTN,
                    MaBN = s.First().MaBN,
                    HoTen = s.First().HoTen,
                    YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                    LoaiVatTu = s.First().LoaiVatTu,
                    SoLuongTon = s.First().SoLuongTon,
                    NgayYeuCau = s.First().NgayYeuCau,
                    NgayDieuTri = s.First().NgayDieuTri,
                    IsCheckRowItem = s.First().IsCheckRowItem,
                    YeuCauLinhVatTuId = s.First().YeuCauLinhVatTuId
                }).ToList();
            List<ThongTinLinhVatTuTuKhoGridVo> list = new List<ThongTinLinhVatTuTuKhoGridVo>();
            foreach (var itemCha in yeuCauDuocPhamGroup)
            {
                itemCha.ListYeuCauVatTuBenhViens = _yeuCauVatTuBenhVienRepository.TableNoTracking
               .Where(x => x.YeuCauLinhVatTuId == itemCha.YeuCauLinhVatTuId && x.YeuCauTiepNhanId == itemCha.YeuCauTiepNhanId
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
                   NgayKe = p.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                   DuocDuyet = p.YeuCauLinhVatTuId == null ? false : p.YeuCauLinhVatTu.DuocDuyet == true ? true : false,
                   TenVatTu = p.Ten,
                   HoatChat = p.Ma,
                   DonViTinh = p.DonViTinh,
                   HangSX = p.NhaSanXuat,
                   NuocSanXuat = p.NuocSanXuat,
                   SLYeuCau = p.SoLuong,
                   LoaiThuoc = p.LaVatTuBHYT == true ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                   NgayYeuCau = p.ThoiDiemChiDinh,
                   IsCheckRowItem = true,
                   VatTuId = p.VatTuBenhVienId,
                   YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                   SoLuongTon = p.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == p.KhoLinhId && nkct.LaVatTuBHYT == p.LaVatTuBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                   NgayDieuTri = (p.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && p.NoiTruPhieuDieuTri != null) ? p.NoiTruPhieuDieuTri.NgayDieuTri : p.ThoiDiemChiDinh
               }).Where(p => (tuNgay == null || p.NgayDieuTri >= tuNgay) && (denNgay == null || p.NgayDieuTri <= denNgay)).ToList();
                list.Add(itemCha);
            }
            return list;
        }
        public List<long> GetYeuCauVatTuIdDaTao(long yeuCauLinhVatTuId)
        {
            var queryYeuCauKhamBenhId = _yeuCauVatTuBenhVienRepository.TableNoTracking
            .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId && x.TrangThai == EnumYeuCauVatTuBenhVien.ChuaThucHien
                        )
              .Select(s => s.Id);
            return queryYeuCauKhamBenhId.ToList();
        }
        public async Task XuLyHuyYeuCauVatTuTTAsync(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDuocPhamBenhVien ycdpbv, List<long> ycdpbvs)
        {


        }
        public void XuLyHuyYeuCauVatTuTTAsync(List<long> ycdpbvs)
        {
            foreach (var item in ycdpbvs)
            {
                var ycdp = _yeuCauVatTuBenhVienRepository.GetById(item, s => s.Include(d => d.YeuCauLinhVatTuChiTiets));
                ycdp.YeuCauLinhVatTuId = null;
                if (ycdp.YeuCauLinhVatTuChiTiets.Select(d => d.Id).Any())
                {
                    _yeuCauLinhVatTuChiTietRepository.DeleteAsync(ycdp.YeuCauLinhVatTuChiTiets);
                }
                _yeuCauVatTuBenhVienRepository.Context.SaveChanges();
            }
        }
        #endregion
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
