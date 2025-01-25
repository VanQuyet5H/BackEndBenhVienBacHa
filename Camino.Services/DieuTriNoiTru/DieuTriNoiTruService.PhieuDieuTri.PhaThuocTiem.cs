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
        public GridDataSource GetDataForGridDanhSachPhaThuocTiem(QueryInfo queryInfo)
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
                          && o.NoiTruChiDinhPhaThuocTiemId != null
                          )
                .Select(s => new PhieuDieuTriPhaThuocTiemGridVo
                {
                    Id = s.Id,
                    NoiTruChiDinhPhaThuocTiemId = s.NoiTruChiDinhPhaThuocTiemId,
                    SoThuTu = s.SoThuTu,
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
                    PhieuLinh = s.YeuCauDuocPhamBenhViens.Any() ? s.YeuCauDuocPhamBenhViens.First().YeuCauLinhDuocPham.SoPhieu : null,
                    PhieuXuat = s.YeuCauDuocPhamBenhViens.Any(a => a.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null) ?
                        s.YeuCauDuocPhamBenhViens.Select(a => a.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu).FirstOrDefault() : "",
                    KhongTinhPhi = s.YeuCauDuocPhamBenhViens.Select(p => !p.KhongTinhPhi).FirstOrDefault(),
                    LaThuocHuongThanGayNghien = s.DuocPhamBenhVien.DuocPham.LaThuocHuongThanGayNghien,
                    ThoiGianBatDauTiem = s.NoiTruChiDinhPhaThuocTiem != null ? s.NoiTruChiDinhPhaThuocTiem.ThoiGianBatDauTiem : null,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    KhuVuc = 1,
                    DuongDungId = s.DuongDungId,
                    SoLanTrenMui = s.NoiTruChiDinhPhaThuocTiem != null ? s.NoiTruChiDinhPhaThuocTiem.SoLanTrenMui : null,
                    SoLanTrenNgay = s.NoiTruChiDinhPhaThuocTiem != null ? s.NoiTruChiDinhPhaThuocTiem.SoLanTrenNgay : null,
                    CachGioTiem = s.NoiTruChiDinhPhaThuocTiem != null ? s.NoiTruChiDinhPhaThuocTiem.CachGioTiem : null,

                });
            double seconds = 3600;
            var lstQuery = query.ToList();
            lstQuery = lstQuery.AsQueryable().OrderBy(queryInfo.SortString).ThenBy(z => z.DuongDungNumber).ToList();

            for (int i = 0; i < lstQuery.Count(); i++)
            {
                var thoiGianBatDauTiem = lstQuery[i].ThoiGianBatDauTiem;
                if (thoiGianBatDauTiem != null)
                {
                    if (lstQuery[i].SoLanTrenNgay != null && lstQuery[i].CachGioTiem != null)
                    {
                        for (int j = 0; j < lstQuery[i].SoLanTrenNgay; j++)
                        {
                            var time = thoiGianBatDauTiem.Value.ConvertIntSecondsToTime12h();
                            thoiGianBatDauTiem += (int?)(lstQuery[i].CachGioTiem * seconds);
                            lstQuery[i].GioSuDung += time + "; ";
                        }
                    }
                    else
                    {
                        lstQuery[i].GioSuDung = thoiGianBatDauTiem.Value.ConvertIntSecondsToTime12h();
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
        public GridDataSource GetTotalPageForGridDanhSachPhaThuocTiem(QueryInfo queryInfo)
        {
            return null;
        }

        public GridDataSource GetDataForGridDanhSachPhaThuocTiemNgoai(QueryInfo queryInfo)
        {
            return null;
        }
        public GridDataSource GetTotalPageForGridDanhSachPhaThuocTiemNgoai(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task ThemPhaThuocTiem(PhaThuocTiemBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId).First();

            var noiTruChiDinhPhaThuocTiem = new NoiTruChiDinhPhaThuocTiem
            {
                YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                NoiTruBenhAnId = donThuocChiTiet.YeuCauTiepNhanId,
                NoiTruPhieuDieuTriId = donThuocChiTiet.PhieuDieuTriHienTaiId.Value,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                ThoiGianBatDauTiem = donThuocChiTiet.ThoiGianBatDauTiem,
                SoLanTrenMui = donThuocChiTiet.SoLanTrenMui,
                SoLanTrenNgay = donThuocChiTiet.SoLanTrenNgay,
                CachGioTiem = donThuocChiTiet.CachGioTiem
            };
            var soThuTu = donThuocChiTiet.SoThuTu;
            foreach (var phaThuocTiem in donThuocChiTiet.NoiTruChiDinhDuocPhams)
            {
                var duocPham = _duocPhamRepository.GetById(phaThuocTiem.DuocPhamBenhVienId.Value,
                    x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

                var loaiKho = await _khoRepository.TableNoTracking.Where(p => p.Id == phaThuocTiem.KhoId).Select(p => p.LoaiKho).FirstAsync();
                var bacSiChiDinhId = _userAgentHelper.GetCurrentUserId();
                var noiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (phaThuocTiem.LaDuocPhamBHYTNumber != null)
                {
                    phaThuocTiem.LaDuocPhamBHYT = phaThuocTiem.LaDuocPhamBHYTNumber;
                }
                var laDuocPhamBHYT = false;
                if (phaThuocTiem.LaDuocPhamBHYT == 2)
                {
                    laDuocPhamBHYT = true;
                }
                var SLTon = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                            .Where(p => p.NhapKhoDuocPhams.KhoId == phaThuocTiem.KhoId && (p.LaDuocPhamBHYT == laDuocPhamBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
                            .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                if (SLTon < phaThuocTiem.SoLuong)
                {
                    throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                }
                double soLuongCanXuat = phaThuocTiem.SoLuong.Value;

                var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                 .Where(o => o.NhapKhoDuocPhams.KhoId == phaThuocTiem.KhoId
                          && o.LaDuocPhamBHYT == laDuocPhamBHYT
                          && o.DuocPhamBenhVienId == phaThuocTiem.DuocPhamBenhVienId
                          && o.HanSuDung >= DateTime.Now
                          && o.SoLuongNhap > o.SoLuongDaXuat)
                          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

                var noiTruChiDinhDuocPham = new NoiTruChiDinhDuocPham
                {
                    YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                    DuocPhamBenhVienId = phaThuocTiem.DuocPhamBenhVienId.Value,
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
                    TrangThai = phaThuocTiem.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                    DuocHuongBaoHiem = laDuocPhamBHYT,
                    LaDuocPhamBHYT = laDuocPhamBHYT,
                    SoLanDungTrongNgay = donThuocChiTiet.SoLanTrenNgay,
                    TheTich = duocPham.TheTich,
                    GhiChu = donThuocChiTiet.GhiChu,
                    LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri,
                    SoLanTrenVien = donThuocChiTiet.SoLanTrenVien,
                    CachGioDungThuoc = donThuocChiTiet.CachGioDungThuoc,
                    LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay,
                    NoiTruChiDinhPhaThuocTiem = noiTruChiDinhPhaThuocTiem,
                    LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                    SoThuTu = soThuTu
                };
                soThuTu++;
                var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
                {
                    YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                    DuocPhamBenhVienId = phaThuocTiem.DuocPhamBenhVienId.Value,
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
                    TrangThai = phaThuocTiem.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                    DuocHuongBaoHiem = laDuocPhamBHYT,
                    LaDuocPhamBHYT = laDuocPhamBHYT,
                    SoTienBenhNhanDaChi = 0,
                    KhoLinhId = phaThuocTiem.KhoId,
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
                        KhoXuatId = phaThuocTiem.KhoId.Value
                    };
                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                    {
                        DuocPhamBenhVienId = phaThuocTiem.DuocPhamBenhVienId.Value,
                        XuatKhoDuocPham = xuatKhoDuocPham,
                        NgayXuat = DateTime.Now
                    };

                    var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();

                    var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(o => (o.NhapKhoDuocPhams.KhoId == phaThuocTiem.KhoId)
                                     && o.LaDuocPhamBHYT == laDuocPhamBHYT
                                     && o.DuocPhamBenhVienId == phaThuocTiem.DuocPhamBenhVienId
                                     && o.HanSuDung >= DateTime.Now
                                     && o.SoLuongNhap > o.SoLuongDaXuat)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                        .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToList();
                    //.Where(p => p.LaDuocPhamBHYT == laDuocPhamBHYT && p.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId && p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId).ToList();
                    foreach (var item in nhapKhoDuocPhamChiTiets)
                    {
                        if (phaThuocTiem.SoLuong > 0)
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
                                    DuocPhamBenhVienId = phaThuocTiem.DuocPhamBenhVienId.Value,
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
                                if (phaThuocTiem.SoLuong <= tonTheoItem)
                                {
                                    xuatViTri.SoLuongXuat = phaThuocTiem.SoLuong.Value;
                                    item.SoLuongDaXuat = (item.SoLuongDaXuat + phaThuocTiem.SoLuong.Value).MathRoundNumber(2);
                                    phaThuocTiem.SoLuong = 0;
                                }
                                else
                                {
                                    xuatViTri.SoLuongXuat = tonTheoItem;
                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                    phaThuocTiem.SoLuong = (phaThuocTiem.SoLuong - tonTheoItem).MathRoundNumber(2);
                                }

                                xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                            }

                            if (phaThuocTiem.SoLuong == 0)
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
                    if (phaThuocTiem.SoLuong > 0)
                    {
                        var yeuCauNew = ycDuocPhamBenhVien.Clone();

                        var thongTinNhapDuocPham = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoId == phaThuocTiem.KhoId
                                                                                                              && o.LaDuocPhamBHYT == laDuocPhamBHYT
                                                                                                              && o.DuocPhamBenhVienId == phaThuocTiem.DuocPhamBenhVienId
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
                        yeuCauNew.SoLuong = phaThuocTiem.SoLuong.Value;
                        phaThuocTiem.SoLuong = 0;
                        noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                        noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                    }
                }
            }





        }

        public async Task CapNhatPhaThuocTiem(PhaThuocTiemBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId).First();
            foreach (var noiTruDuocPhamUpdate in donThuocChiTiet.NoiTruChiDinhDuocPhams)
            {
                var noiTruChiDinhDuocPham = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.FirstOrDefault(z => z.Id == noiTruDuocPhamUpdate.Id);
                if (noiTruChiDinhDuocPham == null || (noiTruChiDinhDuocPham != null
                                                && noiTruChiDinhDuocPham.SoLuong.AlmostEqual(noiTruDuocPhamUpdate.SoLuong.GetValueOrDefault())
                                                && noiTruChiDinhDuocPham.GhiChu == donThuocChiTiet.GhiChu
                                                ))
                {
                    if (noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem != null
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.ThoiGianBatDauTiem == donThuocChiTiet.ThoiGianBatDauTiem
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.SoLanTrenMui == donThuocChiTiet.SoLanTrenMui
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.SoLanTrenNgay == donThuocChiTiet.SoLanTrenNgay
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.CachGioTiem == donThuocChiTiet.CachGioTiem)
                    {
                        continue;

                    }
                    else if (noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen != null
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.TocDoTruyen == donThuocChiTiet.TocDoTruyen
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.DonViTocDoTruyen == donThuocChiTiet.DonViTocDoTruyen
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.ThoiGianBatDauTruyen == donThuocChiTiet.ThoiGianBatDauTruyen
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.SoLanTrenNgay == donThuocChiTiet.SoLanTrenNgay
                        && noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.CachGioTruyen == donThuocChiTiet.CachGioTruyen
                        )
                    {
                        continue;

                    }
                    else
                    {
                        continue;
                    }
                }
                var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();
                foreach (var yeuCauDuocPhamBenhVien in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Where(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy))
                {
                    var nhapKhoDuocPhamChiTiets = yeuCauDuocPhamBenhVien.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(o => (o.NhapKhoDuocPhams.KhoId == yeuCauDuocPhamBenhVien.KhoLinhId)
                                     && o.LaDuocPhamBHYT == yeuCauDuocPhamBenhVien.LaDuocPhamBHYT
                                     && o.DuocPhamBenhVienId == yeuCauDuocPhamBenhVien.DuocPhamBenhVienId
                                     && o.HanSuDung >= DateTime.Now
                                     && o.SoLuongNhap > o.SoLuongDaXuat)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                        .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToList();
                    var SLTon = nhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    if (SLTon + noiTruChiDinhDuocPham.SoLuong < noiTruDuocPhamUpdate.SoLuong)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }
                    if (yeuCauDuocPhamBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                    {
                        //trả lại số lượng
                        var xuatKhoDpViTris = yeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.ToList();
                        if (yeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet != null)
                        {
                            foreach (var xuatKhoDuocPhamChiTietViTri in xuatKhoDpViTris)
                            {
                                xuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= xuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                            }
                        }
                        foreach (var item in xuatKhoDpViTris)
                        {
                            var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                            {
                                XuatKhoDuocPhamChiTiet = yeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet,
                                NhapKhoDuocPhamChiTietId = item.NhapKhoDuocPhamChiTietId,
                                SoLuongXuat = -item.SoLuongXuat,
                                NgayXuat = DateTime.Now,
                                GhiChu = noiTruChiDinhDuocPham.TrangThai.GetDescription()
                            };
                            yeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                        }

                        //cập nhật số lượng mới
                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.ThoiGianBatDauTiem = donThuocChiTiet.ThoiGianBatDauTiem;
                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.SoLanTrenMui = donThuocChiTiet.SoLanTrenMui;
                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.SoLanTrenNgay = donThuocChiTiet.SoLanTrenNgay;
                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.CachGioTiem = donThuocChiTiet.CachGioTiem;

                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.TocDoTruyen = donThuocChiTiet.TocDoTruyen;
                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen;
                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.SoLanTrenNgay = donThuocChiTiet.SoLanTrenNgay;
                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen;
                        noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.CachGioTruyen = donThuocChiTiet.CachGioTruyen;

                        yeuCauDuocPhamBenhVien.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
                        var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
                        {
                            YeuCauTiepNhanId = yeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                            DuocPhamBenhVienId = yeuCauDuocPhamBenhVien.DuocPhamBenhVienId,
                            Ten = yeuCauDuocPhamBenhVien.Ten,
                            TenTiengAnh = yeuCauDuocPhamBenhVien.TenTiengAnh,
                            SoDangKy = yeuCauDuocPhamBenhVien.SoDangKy,
                            STTHoatChat = yeuCauDuocPhamBenhVien.STTHoatChat,
                            MaHoatChat = yeuCauDuocPhamBenhVien.MaHoatChat,
                            HoatChat = yeuCauDuocPhamBenhVien.HoatChat,
                            LoaiThuocHoacHoatChat = yeuCauDuocPhamBenhVien.LoaiThuocHoacHoatChat,
                            NhaSanXuat = yeuCauDuocPhamBenhVien.NhaSanXuat,
                            NuocSanXuat = yeuCauDuocPhamBenhVien.NuocSanXuat,
                            DuongDungId = yeuCauDuocPhamBenhVien.DuongDungId,
                            HamLuong = yeuCauDuocPhamBenhVien.HamLuong,
                            QuyCach = yeuCauDuocPhamBenhVien.QuyCach,
                            TieuChuan = yeuCauDuocPhamBenhVien.TieuChuan,
                            DangBaoChe = yeuCauDuocPhamBenhVien.DangBaoChe,
                            DonViTinhId = yeuCauDuocPhamBenhVien.DonViTinhId,
                            HuongDan = yeuCauDuocPhamBenhVien.HuongDan,
                            MoTa = yeuCauDuocPhamBenhVien.MoTa,
                            ChiDinh = yeuCauDuocPhamBenhVien.ChiDinh,
                            ChongChiDinh = yeuCauDuocPhamBenhVien.ChongChiDinh,
                            LieuLuongCachDung = yeuCauDuocPhamBenhVien.LieuLuongCachDung,
                            TacDungPhu = yeuCauDuocPhamBenhVien.TacDungPhu,
                            ChuYdePhong = yeuCauDuocPhamBenhVien.ChuYdePhong,
                            NhanVienChiDinhId = yeuCauDuocPhamBenhVien.NhanVienChiDinhId,
                            NoiChiDinhId = yeuCauDuocPhamBenhVien.NoiChiDinhId,
                            ThoiDiemChiDinh = yeuCauDuocPhamBenhVien.ThoiDiemChiDinh,
                            TrangThai = EnumYeuCauDuocPhamBenhVien.DaThucHien,
                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                            DuocHuongBaoHiem = yeuCauDuocPhamBenhVien.DuocHuongBaoHiem,
                            LaDuocPhamBHYT = yeuCauDuocPhamBenhVien.LaDuocPhamBHYT,
                            SoTienBenhNhanDaChi = 0,
                            KhoLinhId = yeuCauDuocPhamBenhVien.KhoLinhId,
                            TheTich = yeuCauDuocPhamBenhVien.TheTich,
                            LoaiPhieuLinh = yeuCauDuocPhamBenhVien.LoaiPhieuLinh,
                            GhiChu = donThuocChiTiet.GhiChu,
                            NoiTruPhieuDieuTriId = yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTriId,
                            LaDichTruyen = yeuCauDuocPhamBenhVien.LaDichTruyen,
                            KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi
                        };


                        var sLUpdate = noiTruDuocPhamUpdate.SoLuong.MathRoundNumber(2);
                        var yeuCauDuocPhamBenhVienNew = ycDuocPhamBenhVien.Clone();
                        var xuatKhoDuocPham = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham
                        {
                            LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                            LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                            TenNguoiNhan = yeuCauTiepNhan.HoTen,
                            NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                            LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                            NgayXuat = DateTime.Now,
                            KhoXuatId = yeuCauDuocPhamBenhVien.KhoLinhId.GetValueOrDefault()
                        };
                        var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet()
                        {
                            DuocPhamBenhVienId = yeuCauDuocPhamBenhVien.DuocPhamBenhVienId,
                            XuatKhoDuocPham = xuatKhoDuocPham,
                            NgayXuat = DateTime.Now
                        };
                        var duocPham = _duocPhamRepository.GetById(yeuCauDuocPhamBenhVien.DuocPhamBenhVienId,
                            x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                            .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
                        foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                        {
                            if (sLUpdate > 0)
                            {
                                var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.Where(o => o.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId).Select(p => p.Gia).FirstOrDefault();

                                var donGiaBaoHiem = nhapKhoDuocPhamChiTiet.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : nhapKhoDuocPhamChiTiet.DonGiaNhap;

                                var tileBHYTThanhToanTheoNhap = nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT ? nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan ?? 100 : 0;

                                if (yeuCauDuocPhamBenhVienNew.DonGiaNhap != 0 && (yeuCauDuocPhamBenhVienNew.DonGiaNhap != nhapKhoDuocPhamChiTiet.DonGiaNhap || yeuCauDuocPhamBenhVienNew.VAT != nhapKhoDuocPhamChiTiet.VAT || yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia != nhapKhoDuocPhamChiTiet.TiLeTheoThapGia || yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap)) // nếu khác đơn giá(lô) thì tạo ra 1 th dp mới
                                {
                                    yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet = yeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet;
                                    yeuCauDuocPhamBenhVienNew.SoLuong = yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat);
                                    lstYeuCau.Add(yeuCauDuocPhamBenhVienNew);

                                    yeuCauDuocPhamBenhVienNew = ycDuocPhamBenhVien.Clone();
                                    yeuCauDuocPhamBenhVienNew.DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap;
                                    yeuCauDuocPhamBenhVienNew.DaCapThuoc = true;
                                    yeuCauDuocPhamBenhVienNew.VAT = nhapKhoDuocPhamChiTiet.VAT;
                                    yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia;
                                    yeuCauDuocPhamBenhVienNew.PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho;
                                    yeuCauDuocPhamBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                                    yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;

                                    xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet()
                                    {
                                        DuocPhamBenhVienId = yeuCauDuocPhamBenhVien.DuocPhamBenhVienId,
                                        XuatKhoDuocPham = xuatKhoDuocPham,
                                        NgayXuat = DateTime.Now
                                    };
                                }
                                else // tạo bình thường
                                {
                                    yeuCauDuocPhamBenhVienNew.DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap;
                                    yeuCauDuocPhamBenhVienNew.VAT = nhapKhoDuocPhamChiTiet.VAT;
                                    yeuCauDuocPhamBenhVienNew.DaCapThuoc = true;
                                    yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia;
                                    yeuCauDuocPhamBenhVienNew.PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho;
                                    yeuCauDuocPhamBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                                    yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;
                                }
                                if (nhapKhoDuocPhamChiTiet.SoLuongNhap > nhapKhoDuocPhamChiTiet.SoLuongDaXuat)
                                {
                                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri()
                                    {
                                        NhapKhoDuocPhamChiTietId = nhapKhoDuocPhamChiTiet.Id,
                                        NgayXuat = DateTime.Now,
                                        GhiChu = xuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LyDoXuatKho
                                    };

                                    var sLTonConLai = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                                    if (sLUpdate > sLTonConLai)
                                    {
                                        yeuCauDuocPhamBenhVienNew.SoLuong = (yeuCauDuocPhamBenhVienNew.SoLuong + sLTonConLai).MathRoundNumber(2);
                                        sLUpdate = (sLUpdate - sLTonConLai).MathRoundNumber(2);
                                        xuatKhoDuocPhamChiTietViTri.SoLuongXuat = sLTonConLai;
                                    }
                                    else
                                    {
                                        yeuCauDuocPhamBenhVienNew.SoLuong = (yeuCauDuocPhamBenhVienNew.SoLuong + sLUpdate.GetValueOrDefault()).MathRoundNumber(2);
                                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + sLUpdate.GetValueOrDefault()).MathRoundNumber(2);
                                        xuatKhoDuocPhamChiTietViTri.SoLuongXuat = sLUpdate.GetValueOrDefault();
                                        sLUpdate = 0;
                                    }

                                    xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                                }

                                if (sLUpdate == 0)
                                {
                                    yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet = xuatKhoDuocPhamChiTiet;
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

                    }
                    else
                    {
                        if (noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem != null)
                        {
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.ThoiGianBatDauTiem = donThuocChiTiet.ThoiGianBatDauTiem;
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.SoLanTrenMui = donThuocChiTiet.SoLanTrenMui;
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.SoLanTrenNgay = donThuocChiTiet.SoLanTrenNgay;
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTiem.CachGioTiem = donThuocChiTiet.CachGioTiem;
                        }
                        if (noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen != null)
                        {
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.TocDoTruyen = donThuocChiTiet.TocDoTruyen;
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen;
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.SoLanTrenNgay = donThuocChiTiet.SoLanTrenNgay;
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen;
                            noiTruChiDinhDuocPham.NoiTruChiDinhPhaThuocTruyen.CachGioTruyen = donThuocChiTiet.CachGioTruyen;
                        }
                        noiTruChiDinhDuocPham.GhiChu = donThuocChiTiet.GhiChu;
                        yeuCauDuocPhamBenhVien.SoLuong = noiTruDuocPhamUpdate.SoLuong.MathRoundNumber(2) ?? 0;
                    }
                }
                if (lstYeuCau.Any() && noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Any(x => x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu))
                {
                    foreach (var item in lstYeuCau)
                    {
                        noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(item);
                    }
                    noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                }
                noiTruChiDinhDuocPham.SoLuong = noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Where(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Sum(z => z.SoLuong).MathRoundNumber(2);
            }
        }

        //public async Task XoaPhaThuocTiems(PhaThuocTiemBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        //{
        //    foreach (var phaThuocTiem in donThuocChiTiet.NoiTruChiDinhDuocPhams)
        //    {
        //        var noiTruChiDinhDuocPham = yeuCauTiepNhan.NoiTruChiDinhDuocPhams.FirstOrDefault(x => x.Id == phaThuocTiem.Id);
        //        if (noiTruChiDinhDuocPham == null)
        //        {
        //            throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.NotExists"));
        //        }
        //        if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().YeuCauLinhDuocPhamId != null)
        //        {
        //            throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.DaLinh"));
        //        }

        //        if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
        //        {
        //            noiTruChiDinhDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
        //            foreach (var dpbv in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
        //            {
        //                dpbv.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;


        //            }
        //        }
        //        else
        //        {
        //            foreach (var dpbv in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
        //            {
        //                if (dpbv.XuatKhoDuocPhamChiTiet != null)
        //                {
        //                    foreach (var thongTinXuat in dpbv.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
        //                    {
        //                        thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= thongTinXuat.SoLuongXuat;
        //                    }
        //                }
        //                dpbv.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
        //            }
        //        }
        //        noiTruChiDinhDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
        //        await XuLyXoaYLenhKhiXoaDichVuNoiTruAsync(EnumNhomGoiDichVu.DuocPham, phaThuocTiem.Id);
        //    }
        //}

        //public async Task<string> XuLySoThuTuPhaThuocTiemHoacTruyen(YeuCauTiepNhan yeuCauTiepNhan, bool LaPhaThuocTiem, long phieuDieuTriHienTaiId)
        //{
        //    var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == phieuDieuTriHienTaiId).First();
        //    var STTThuocTiem = 1;
        //    var STTThuocTruyen = 1;

        //    if (noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Any(z => z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)))
        //    {
        //        foreach (var item in noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)).OrderBy(z => z.CreatedOn))
        //        {
        //            item.SoThuTu = LaPhaThuocTiem == true ? STTThuocTiem : STTThuocTruyen;
        //            STTThuocTiem++;
        //            STTThuocTruyen++;
        //        }
        //    }

        //    if (noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Any(z => z.DuongDungId == 12 && !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc) && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)))//Tiêm z.DuocPham.DuongDung.Ma.Trim() == "2.10".Trim()) 12
        //    {
        //        foreach (var item in noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.DuongDungId == 12 && !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc) && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)).OrderBy(z => z.CreatedOn))
        //        {
        //            item.SoThuTu = LaPhaThuocTiem == true ? STTThuocTiem : STTThuocTruyen;
        //            STTThuocTiem++;
        //            STTThuocTruyen++;
        //        }
        //    }

        //    if (noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Any(z => z.DuongDungId == 1 && !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc) && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)))//Uống z.DuocPham.DuongDung.Ma.Trim() == "1.01".Trim() 1
        //    {
        //        foreach (var item in noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.DuongDungId == 1 && !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc) && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)).OrderBy(z => z.CreatedOn))
        //        {
        //            item.SoThuTu = LaPhaThuocTiem == true ? STTThuocTiem : STTThuocTruyen;
        //            STTThuocTiem++;
        //            STTThuocTruyen++;
        //        }
        //    }

        //    if (noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Any(z => z.DuongDungId == 26 && !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc) && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)))//Đặt 26 z.DuocPham.DuongDung.Ma.Trim() == "4.04".Trim()
        //    {
        //        foreach (var item in noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.DuongDungId == 26 && !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc) && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)).OrderBy(z => z.CreatedOn))
        //        {
        //            item.SoThuTu = LaPhaThuocTiem == true ? STTThuocTiem : STTThuocTruyen;
        //            STTThuocTiem++;
        //            STTThuocTruyen++;
        //        }
        //    }

        //    if (noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Any(z => z.DuongDungId == 22 && !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc) && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)))//Dùng ngoài 22 z.DuocPham.DuongDung.Ma.Trim() == "3.05".Trim()
        //    {
        //        foreach (var item in noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.DuongDungId == 22 && !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc) && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)).OrderBy(z => z.CreatedOn))
        //        {
        //            item.SoThuTu = LaPhaThuocTiem == true ? STTThuocTiem : STTThuocTruyen;
        //            STTThuocTiem++;
        //            STTThuocTruyen++;
        //        }
        //    }
        //    foreach (var item in noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => !(z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc)
        //                                                                         && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
        //                                                                         && (LaPhaThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId != null : z.NoiTruChiDinhPhaThuocTruyenId != null)
        //                                                                         && z.DuongDungId != 12
        //                                                                         && z.DuongDungId != 1
        //                                                                         && z.DuongDungId != 26
        //                                                                         && z.DuongDungId != 22).OrderBy(z => z.CreatedOn))
        //    {
        //        item.SoThuTu = LaPhaThuocTiem == true ? STTThuocTiem : STTThuocTruyen;
        //        STTThuocTiem++;
        //        STTThuocTruyen++;
        //    }

        //    await _yeuCauTiepNhanRepository.UpdateAsync(yeuCauTiepNhan);


        //    return string.Empty;
        //}

        //public async Task<string> TangHoacGiamSTTDonThuocTiemChiTiet(ThuocBenhVienTangGiamSTTTiemHoacTruyenVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        //{
        //    var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId).First();
        //    var dtChiTiet = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.Id == donThuocChiTiet.Id).FirstOrDefault();
        //    if (donThuocChiTiet.LaTangSTT == true)
        //    {
        //        if (donThuocChiTiet.LaThuocTiem)
        //        {
        //            var dtChiTietKeTiep = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.SoThuTu == (dtChiTiet.SoThuTu + 1) && z.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && z.NoiTruChiDinhPhaThuocTiemId != null).FirstOrDefault();
        //            if (dtChiTietKeTiep != null)
        //            {
        //                dtChiTiet.SoThuTu++;
        //                dtChiTietKeTiep.SoThuTu--;
        //            }
        //            else
        //            {
        //                throw new Exception(_localizationService.GetResource("DonThuoc.STT.KhongTheTang"));
        //            }
        //        }
        //        else
        //        {
        //            var dtChiTietKeTiep = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.SoThuTu == (dtChiTiet.SoThuTu + 1) && z.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && z.NoiTruChiDinhPhaThuocTruyenId != null).FirstOrDefault();
        //            if (dtChiTietKeTiep != null)
        //            {
        //                dtChiTiet.SoThuTu++;
        //                dtChiTietKeTiep.SoThuTu--;
        //            }
        //            else
        //            {
        //                throw new Exception(_localizationService.GetResource("DonThuoc.STT.KhongTheTang"));
        //            }

        //        }

        //    }
        //    else
        //    {
        //        if (donThuocChiTiet.LaThuocTiem)
        //        {
        //            var dtChiTietDangTruoc = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.SoThuTu == (dtChiTiet.SoThuTu - 1) && z.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && z.NoiTruChiDinhPhaThuocTiemId != null).FirstOrDefault();
        //            if (dtChiTietDangTruoc != null)
        //            {
        //                dtChiTiet.SoThuTu--;
        //                dtChiTietDangTruoc.SoThuTu++;
        //            }
        //            else
        //            {
        //                throw new Exception(_localizationService.GetResource("DonThuoc.STT.KhongTheGiam"));
        //            }
        //        }
        //        else
        //        {
        //            var dtChiTietDangTruoc = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.SoThuTu == (dtChiTiet.SoThuTu - 1) && z.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT && z.LaDichTruyen != true && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && z.NoiTruChiDinhPhaThuocTruyenId != null).FirstOrDefault();
        //            if (dtChiTietDangTruoc != null)
        //            {
        //                dtChiTiet.SoThuTu--;
        //                dtChiTietDangTruoc.SoThuTu++;
        //            }
        //            else
        //            {
        //                throw new Exception(_localizationService.GetResource("DonThuoc.STT.KhongTheGiam"));
        //            }
        //        }

        //    }
        //    await _noiTruPhieuDieuTriRepository.UpdateAsync(noiTruPhieuDieuTri);
        //    return string.Empty;
        //}

        public async Task CapNhatKhongTinhPhiTiem(CapNhatKhongTinhPhiTiem capNhatKhongTinhPhi, YeuCauTiepNhan yeuCauTiepNhan)
        {
            foreach (var id in capNhatKhongTinhPhi.Ids)
            {
                var noiTruChiDinhDuocPham = yeuCauTiepNhan.NoiTruChiDinhDuocPhams.First(p => p.Id == id);
                foreach (var yeuCauDuocPhamBenhVien in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
                {
                    yeuCauDuocPhamBenhVien.KhongTinhPhi = !capNhatKhongTinhPhi.KhongTinhPhi;
                }
            }
        }
    }
}
