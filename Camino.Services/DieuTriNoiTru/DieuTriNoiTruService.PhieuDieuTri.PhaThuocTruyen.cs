using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public GridDataSource GetDataForGridDanhSachPhaThuocTruyen(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var query = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId
                          && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen != true
                          && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                          && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                          //&& o.YeuCauDuocPhamBenhViens.All(x => x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                          && o.NoiTruChiDinhPhaThuocTruyenId != null
                          )
                .Select(s => new PhieuDieuTriPhaThuocTruyenGridVo
                {
                    Id = s.Id,
                    STT = s.SoThuTu,
                    NoiTruChiDinhPhaThuocTruyenId = s.NoiTruChiDinhPhaThuocTruyenId,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    Ma = s.DuocPhamBenhVien.Ma,
                    Ten = s.Ten,
                    KhoId = s.YeuCauDuocPhamBenhViens.First().KhoLinhId,
                    TenKho = s.YeuCauDuocPhamBenhViens.First().KhoLinh.Ten,
                    HoatChat = s.HoatChat,
                    DVT = s.DonViTinh.Ten,
                    TenDuongDung = s.DuongDung.Ten,
                    SoLuongDisplay = s.YeuCauDuocPhamBenhViens.Select(p => p.SoLuong).ToList(),
                    SoLuong = s.SoLuong,
                    DonGias = s.YeuCauDuocPhamBenhViens.Select(p => p.DonGiaBan).ToList(),
                    ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true && p.NoiTruChiDinhDuocPhamId == s.Id) ? s.YeuCauDuocPhamBenhViens.Where(x => x.NoiTruChiDinhDuocPhamId == s.Id).Select(p => p.GiaBan).ToList() : new List<decimal> { 0 },
                    //DonGias = s.YeuCauDuocPhamBenhViens.Select(p => CalculateHelper.TinhDonGiaBan(p.DonGiaNhap, p.TiLeTheoThapGia, p.VAT)).ToList(),
                    //ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true && p.NoiTruChiDinhDuocPhamId == s.Id) ? s.YeuCauDuocPhamBenhViens.Where(x => x.NoiTruChiDinhDuocPhamId == s.Id).Select(p => (CalculateHelper.TinhDonGiaBan(p.DonGiaNhap, p.TiLeTheoThapGia, p.VAT)) * (decimal)p.SoLuong).ToList() : new List<decimal> { 0 },
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    GhiChu = s.GhiChu,
                    TinhTrang = s.YeuCauDuocPhamBenhViens.First().XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    LaDichTruyen = s.LaDichTruyen,
                    CoYeuCauTraDuocPhamTuBenhNhanChiTiet = s.YeuCauDuocPhamBenhViens.Any(yc => yc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any()),
                    LaTuTruc = s.YeuCauDuocPhamBenhViens.Any(yc => yc.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu),
                    KhongTinhPhi = s.YeuCauDuocPhamBenhViens.Select(p => !p.KhongTinhPhi).FirstOrDefault(),
                    LaThuocHuongThanGayNghien = s.DuocPhamBenhVien.DuocPham.LaThuocHuongThanGayNghien,
                    ThoiGianBatDauTruyen = s.NoiTruChiDinhPhaThuocTruyen != null ? s.NoiTruChiDinhPhaThuocTruyen.ThoiGianBatDauTruyen : null,
                    PhieuLinh = s.YeuCauDuocPhamBenhViens.Any() ? s.YeuCauDuocPhamBenhViens.First().YeuCauLinhDuocPham.SoPhieu : null,
                    PhieuXuat = s.YeuCauDuocPhamBenhViens.Any(a => a.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null) ?
                        s.YeuCauDuocPhamBenhViens.Select(a => a.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu).FirstOrDefault() : "",
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    KhuVuc = 1,
                    DuongDungId = s.DuongDungId,
                    SoLanTrenNgay = s.NoiTruChiDinhPhaThuocTruyen != null ? s.NoiTruChiDinhPhaThuocTruyen.SoLanTrenNgay : null,
                    CachGioTruyen = s.NoiTruChiDinhPhaThuocTruyen != null ? s.NoiTruChiDinhPhaThuocTruyen.CachGioTruyen : null,
                    TocDoTruyen = s.NoiTruChiDinhPhaThuocTruyen != null ? s.NoiTruChiDinhPhaThuocTruyen.TocDoTruyen : null,
                    DonViTocDoTruyen = s.NoiTruChiDinhPhaThuocTruyen != null ? s.NoiTruChiDinhPhaThuocTruyen.DonViTocDoTruyen : null
                });
            double seconds = 3600;
            var lstQuery = query.ToList();
            lstQuery = lstQuery.AsQueryable().OrderBy(queryInfo.SortString).ThenBy(z => z.DuongDungNumber).ToList();
            for (int i = 0; i < lstQuery.Count(); i++)
            {
                var thoiGianBatDauTruyen = lstQuery[i].ThoiGianBatDauTruyen;
                if (thoiGianBatDauTruyen != null)
                {
                    if (lstQuery[i].SoLanTrenNgay != null && lstQuery[i].CachGioTruyen != null)
                    {
                        for (int j = 0; j < lstQuery[i].SoLanTrenNgay; j++)
                        {
                            var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                            thoiGianBatDauTruyen += (int?)(lstQuery[i].CachGioTruyen * seconds);
                            lstQuery[i].GioSuDung += time + "; ";
                        }
                    }
                    else
                    {
                        lstQuery[i].GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                    }
                }
                if (!lstQuery[i].ThanhTiens.Any())
                {
                    lstQuery[i].ThanhTiens = new List<decimal> { 0 };
                }
            }
            var countTask = queryInfo.LazyLoadPage == true ? 0 : lstQuery.Count();
            var queryTask = lstQuery.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public GridDataSource GetTotalPageForGridDanhSachPhaThuocTruyen(QueryInfo queryInfo)
        {
            return null;
        }
        public async Task ThemPhaThuocTruyen(PhaThuocTruyenBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId).First();

            var noiTruChiDinhPhaThuocTruyen = new NoiTruChiDinhPhaThuocTruyen
            {
                YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                NoiTruBenhAnId = donThuocChiTiet.YeuCauTiepNhanId,
                NoiTruPhieuDieuTriId = donThuocChiTiet.PhieuDieuTriHienTaiId.Value,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                SoLanTrenNgay = donThuocChiTiet.SoLanTrenNgay,
                CachGioTruyen = donThuocChiTiet.CachGioTruyen,
                DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                TocDoTruyen = donThuocChiTiet.TocDoTruyen
            };
            var soThuTu = donThuocChiTiet.SoThuTu;
            foreach (var phaThuocTruyen in donThuocChiTiet.NoiTruChiDinhDuocPhams)
            {
                var duocPham = _duocPhamRepository.GetById(phaThuocTruyen.DuocPhamBenhVienId.Value,
                    x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

                var loaiKho = await _khoRepository.TableNoTracking.Where(p => p.Id == phaThuocTruyen.KhoId).Select(p => p.LoaiKho).FirstAsync();
                var bacSiChiDinhId = _userAgentHelper.GetCurrentUserId();
                var noiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (phaThuocTruyen.LaDuocPhamBHYTNumber != null)
                {
                    phaThuocTruyen.LaDuocPhamBHYT = phaThuocTruyen.LaDuocPhamBHYTNumber;
                }
                var laDuocPhamBHYT = false;
                if (phaThuocTruyen.LaDuocPhamBHYT == 2)
                {
                    laDuocPhamBHYT = true;
                }
                var SLTon = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                            .Where(p => p.NhapKhoDuocPhams.KhoId == phaThuocTruyen.KhoId && (p.LaDuocPhamBHYT == laDuocPhamBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
                            .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                if (SLTon < phaThuocTruyen.SoLuong)
                {
                    throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                }
                double soLuongCanXuat = phaThuocTruyen.SoLuong.Value;

                var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                 .Where(o => o.NhapKhoDuocPhams.KhoId == phaThuocTruyen.KhoId
                          && o.LaDuocPhamBHYT == laDuocPhamBHYT
                          && o.DuocPhamBenhVienId == phaThuocTruyen.DuocPhamBenhVienId
                          && o.HanSuDung >= DateTime.Now
                          && o.SoLuongNhap > o.SoLuongDaXuat)
                          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

                var noiTruChiDinhDuocPham = new NoiTruChiDinhDuocPham
                {
                    YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                    DuocPhamBenhVienId = phaThuocTruyen.DuocPhamBenhVienId.Value,
                    Ten = duocPham.Ten,
                    TenTiengAnh = duocPham.TenTiengAnh,
                    SoDangKy = duocPham.SoDangKy,
                    STTHoatChat = duocPham.STTHoatChat,
                    MaHoatChat = duocPham.MaHoatChat,
                    HoatChat = duocPham.HoatChat,
                    LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat,
                    NhaSanXuat = duocPham.NhaSanXuat,
                    NuocSanXuat = duocPham.NuocSanXuat,
                    DuongDungId = duocPham.DuongDungId,
                    HamLuong = duocPham.HamLuong,
                    QuyCach = duocPham.QuyCach,
                    TieuChuan = duocPham.TieuChuan,
                    DangBaoChe = duocPham.DangBaoChe,
                    DonViTinhId = duocPham.DonViTinhId,
                    HuongDan = duocPham.HuongDan,
                    MoTa = duocPham.MoTa,
                    ChiDinh = duocPham.ChiDinh,
                    ChongChiDinh = duocPham.ChongChiDinh,
                    LieuLuongCachDung = duocPham.LieuLuongCachDung,
                    TacDungPhu = duocPham.TacDungPhu,
                    ChuYdePhong = duocPham.ChuYDePhong,
                    SoLuong = soLuongXuat,
                    NhanVienChiDinhId = bacSiChiDinhId,
                    NoiChiDinhId = noiChiDinhId,
                    ThoiDiemChiDinh = DateTime.Now,
                    TrangThai = phaThuocTruyen.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                    DuocHuongBaoHiem = laDuocPhamBHYT,
                    LaDuocPhamBHYT = laDuocPhamBHYT,
                    TheTich = duocPham.TheTich,
                    GhiChu = donThuocChiTiet.GhiChu,
                    LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri,
                    CachGioDungThuoc = donThuocChiTiet.CachGioDungThuoc,
                    LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay,
                    NoiTruChiDinhPhaThuocTruyen = noiTruChiDinhPhaThuocTruyen,
                    LaDichTruyen = donThuocChiTiet.LaDichTruyen,

                    SoLanDungTrongNgay = donThuocChiTiet.SoLanTrenNgay,
                    ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                    CachGioTruyenDich = donThuocChiTiet.CachGioTruyen,
                    DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                    TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                    SoThuTu = soThuTu
                };
                soThuTu++;
                var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
                {
                    YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                    DuocPhamBenhVienId = phaThuocTruyen.DuocPhamBenhVienId.Value,
                    Ten = duocPham.Ten,
                    TenTiengAnh = duocPham.TenTiengAnh,
                    SoDangKy = duocPham.SoDangKy,
                    STTHoatChat = duocPham.STTHoatChat,
                    MaHoatChat = duocPham.MaHoatChat,
                    HoatChat = duocPham.HoatChat,
                    LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat,
                    NhaSanXuat = duocPham.NhaSanXuat,
                    NuocSanXuat = duocPham.NuocSanXuat,
                    DuongDungId = duocPham.DuongDungId,
                    HamLuong = duocPham.HamLuong,
                    QuyCach = duocPham.QuyCach,
                    TieuChuan = duocPham.TieuChuan,
                    DangBaoChe = duocPham.DangBaoChe,
                    DonViTinhId = duocPham.DonViTinhId,
                    HuongDan = duocPham.HuongDan,
                    MoTa = duocPham.MoTa,
                    ChiDinh = duocPham.ChiDinh,
                    ChongChiDinh = duocPham.ChongChiDinh,
                    LieuLuongCachDung = duocPham.LieuLuongCachDung,
                    TacDungPhu = duocPham.TacDungPhu,
                    ChuYdePhong = duocPham.ChuYDePhong,
                    SoLuong = soLuongXuat,
                    NhanVienChiDinhId = bacSiChiDinhId,
                    NoiChiDinhId = noiChiDinhId,
                    ThoiDiemChiDinh = DateTime.Now,
                    TrangThai = phaThuocTruyen.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                    DuocHuongBaoHiem = laDuocPhamBHYT,
                    LaDuocPhamBHYT = laDuocPhamBHYT,
                    SoTienBenhNhanDaChi = 0,
                    KhoLinhId = phaThuocTruyen.KhoId,
                    TheTich = duocPham.TheTich,
                    LoaiPhieuLinh = loaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                    GhiChu = donThuocChiTiet.GhiChu,
                    NoiTruPhieuDieuTriId = donThuocChiTiet.PhieuDieuTriHienTaiId,
                    LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                    KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi
                };

                soLuongCanXuat = soLuongCanXuat - soLuongXuat;

                ycDuocPhamBenhVien.HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId;

                //Lĩnh bù hoặc lĩnh cho người bệnh
                if (ycDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                {
                    var yeuCauDuocPhamBenhVienNew = ycDuocPhamBenhVien.Clone();
                    var xuatKhoDuocPham = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham
                    {
                        LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                        LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                        TenNguoiNhan = yeuCauTiepNhan.HoTen,
                        NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                        LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                        NgayXuat = DateTime.Now,
                        KhoXuatId = phaThuocTruyen.KhoId.Value
                    };
                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                    {
                        DuocPhamBenhVienId = phaThuocTruyen.DuocPhamBenhVienId.Value,
                        XuatKhoDuocPham = xuatKhoDuocPham,
                        NgayXuat = DateTime.Now
                    };

                    var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();

                    var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(o => (o.NhapKhoDuocPhams.KhoId == phaThuocTruyen.KhoId)
                                     && o.LaDuocPhamBHYT == laDuocPhamBHYT
                                     && o.DuocPhamBenhVienId == phaThuocTruyen.DuocPhamBenhVienId
                                     && o.HanSuDung >= DateTime.Now
                                     && o.SoLuongNhap > o.SoLuongDaXuat)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                        .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToList();
                    //.Where(p => p.LaDuocPhamBHYT == laDuocPhamBHYT && p.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId && p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId).ToList();
                    foreach (var item in nhapKhoDuocPhamChiTiets)
                    {
                        if (phaThuocTruyen.SoLuong > 0)
                        {
                            var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.Where(o => o.HopDongThauDuocPhamId == item.HopDongThauDuocPhamId).Select(p => p.Gia).FirstOrDefault();

                            var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;
                            var tileBHYTThanhToanTheoNhap = item.LaDuocPhamBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;
                            if (yeuCauDuocPhamBenhVienNew.DonGiaNhap != 0 && (yeuCauDuocPhamBenhVienNew.DonGiaNhap != item.DonGiaNhap || yeuCauDuocPhamBenhVienNew.VAT != item.VAT || yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap)) // nếu khác đơn giá(lô) thì tạo ra 1 th dp mới
                            {
                                yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                                yeuCauDuocPhamBenhVienNew.SoLuong = yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat);
                                lstYeuCau.Add(yeuCauDuocPhamBenhVienNew);

                                yeuCauDuocPhamBenhVienNew = ycDuocPhamBenhVien.Clone();
                                yeuCauDuocPhamBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauDuocPhamBenhVienNew.DaCapThuoc = true;
                                yeuCauDuocPhamBenhVienNew.VAT = item.VAT;
                                yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauDuocPhamBenhVienNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                                yeuCauDuocPhamBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;

                                xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                                {
                                    DuocPhamBenhVienId = phaThuocTruyen.DuocPhamBenhVienId.Value,
                                    XuatKhoDuocPham = xuatKhoDuocPham,
                                    NgayXuat = DateTime.Now
                                };
                            }
                            else // tạo bình thường
                            {
                                yeuCauDuocPhamBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauDuocPhamBenhVienNew.VAT = item.VAT;
                                yeuCauDuocPhamBenhVienNew.DaCapThuoc = true;
                                yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauDuocPhamBenhVienNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                                yeuCauDuocPhamBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;
                            }
                            if (item.SoLuongNhap > item.SoLuongDaXuat)
                            {
                                var xuatViTri = new XuatKhoDuocPhamChiTietViTri()
                                {
                                    NhapKhoDuocPhamChiTietId = item.Id,
                                    NgayXuat = DateTime.Now,
                                    GhiChu = xuatChiTiet.XuatKhoDuocPham.LyDoXuatKho
                                };

                                var tonTheoItem = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                                if (phaThuocTruyen.SoLuong <= tonTheoItem)
                                {
                                    xuatViTri.SoLuongXuat = phaThuocTruyen.SoLuong.Value;
                                    item.SoLuongDaXuat = (item.SoLuongDaXuat + phaThuocTruyen.SoLuong.Value).MathRoundNumber(2);
                                    phaThuocTruyen.SoLuong = 0;
                                }
                                else
                                {
                                    xuatViTri.SoLuongXuat = tonTheoItem;
                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                    phaThuocTruyen.SoLuong = (phaThuocTruyen.SoLuong - tonTheoItem).MathRoundNumber(2);
                                }

                                xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                            }

                            if (phaThuocTruyen.SoLuong == 0)
                            {
                                yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                                yeuCauDuocPhamBenhVienNew.SoLuong = yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat).MathRoundNumber(2);
                                lstYeuCau.Add(yeuCauDuocPhamBenhVienNew);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    foreach (var item in lstYeuCau)
                    {
                        noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(item);
                    }
                    noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);

                }
                else
                {
                    if (phaThuocTruyen.SoLuong > 0)
                    {
                        var yeuCauNew = ycDuocPhamBenhVien.Clone();

                        var thongTinNhapDuocPham = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoId == phaThuocTruyen.KhoId
                                                                                                              && o.LaDuocPhamBHYT == laDuocPhamBHYT
                                                                                                              && o.DuocPhamBenhVienId == phaThuocTruyen.DuocPhamBenhVienId
                                                                                                              && o.HanSuDung >= DateTime.Now
                                                                                                              && o.SoLuongNhap > o.SoLuongDaXuat)
                          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                        var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.Where(o => o.HopDongThauDuocPhamId == thongTinNhapDuocPham.HopDongThauDuocPhamId).Select(p => p.Gia).FirstOrDefault();
                        var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                        yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                        yeuCauNew.DaCapThuoc = false;
                        yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                        yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                        yeuCauNew.PhuongPhapTinhGiaTriTonKho = thongTinNhapDuocPham.PhuongPhapTinhGiaTriTonKho;
                        yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                        yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                        yeuCauNew.SoLuong = phaThuocTruyen.SoLuong.Value;
                        phaThuocTruyen.SoLuong = 0;
                        noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                        noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                    }
                }
            }





        }
    }
}
