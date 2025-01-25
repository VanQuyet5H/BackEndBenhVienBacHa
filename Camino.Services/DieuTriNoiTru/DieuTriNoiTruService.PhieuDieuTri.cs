using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;
using Newtonsoft.Json;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task KiemTraThoiDiemXuatVienBenhAn(long yeuCauTiepNhanId)
        {
            #region Cập nhật 23/12/2022
            //var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId).First();
            //if (noiTruBenhAn.ThoiDiemRaVien != null)
            //{
            //    var currentUserLanguge = _userAgentHelper.GetUserLanguage();// ?? Enums.LanguageType.VietNam;
            //    var mess = await _localeStringResourceRepository.TableNoTracking
            //       .Where(x => x.ResourceName == "ApiError.ConcurrencyError" && x.Language == (int)currentUserLanguge)
            //       .Select(x => x.ResourceValue).FirstOrDefaultAsync();
            //    throw new Exception(mess ?? "ApiError.ConcurrencyError");
            //}

            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId).Select(x => new { x.ThoiDiemRaVien }).First();
            if (noiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }
            #endregion
        }
        public bool IsCheckThongTinBenhAnDaKetThuc(long yeuCauTiepNhanId)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId).First();
            if (noiTruBenhAn.ThoiDiemRaVien != null)
            {
                return true;
            }
            return false;
        }
        public async Task<string> GetContentInPhieuThamKhamOld(long yeuCauTiepNhanId, long? phieuDieuTriId = null, List<long> dienBienIds = null)
        {
            var content = "";
            var html = _templateRepository.TableNoTracking.OrderByDescending(k => k.Version)
                .Where(o => o.Name == "ToDieuTri").Select(o => o.Body).FirstOrDefault();

            var thongTinBenhAn = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.SoBenhAn,
                    NoiTruPhieuDieuTris = o.NoiTruPhieuDieuTris.Where(p => phieuDieuTriId == null || p.Id == phieuDieuTriId).Select(p => new
                    {
                        p.Id,
                        TenKhoaPhongDieuTri = p.KhoaPhongDieuTri.Ten,
                        p.ChanDoanChinhGhiChu,
                        GhiChuThamKhamChanDoanKemTheos = p.NoiTruThamKhamChanDoanKemTheos.Select(cdkt => cdkt.GhiChu).ToList(),
                        p.DienBien,
                        p.NgayDieuTri,
                        p.ThoiDiemThamKham,
                        p.CheDoAn,
                        p.CheDoChamSoc,
                        p.LastUserId
                    }),
                    KhoaPhongDieuTris = o.NoiTruKhoaPhongDieuTris.Select(k => new
                    {
                        k.Id,
                        k.ThoiDiemVaoKhoa,
                        TenKhoaPhongChuyenDen =
                        k.KhoaPhongChuyenDen.Ten
                    }).ToList(),
                    NoiTruEkipDieuTris = o.NoiTruEkipDieuTris
                })
                .FirstOrDefault();

            var thongTinYeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    o.HoTen,
                    o.NgaySinh,
                    o.ThangSinh,
                    o.NamSinh,
                    o.GioiTinh,
                    YeuCauDichVuGiuongBenhViens = o.YeuCauDichVuGiuongBenhViens.Select(g => new
                    {
                        g.Id,
                        g.TrangThai,
                        g.DoiTuongSuDung,
                        g.ThoiDiemBatDauSuDung,
                        g.GiuongBenhId
                    }).ToList(),
                    YeuCauDichVuKyThuats = o.YeuCauDichVuKyThuats.Select(kt => new
                    {
                        kt.Id,
                        kt.TrangThai,
                        kt.LoaiDichVuKyThuat,
                        kt.NoiTruPhieuDieuTriId,
                        kt.ThoiDiemChiDinh,
                        kt.ThoiGianDienBien,
                        kt.TenDichVu,
                        kt.DichVuKyThuatBenhVienId,
                        kt.SoLan
                    }).ToList(),
                    NgayDungDuocPhamBenhVienVos = o.YeuCauDuocPhamBenhViens.Where(dp => dp.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && dp.SoLuong > 0).Select(dp => new NgayDungDuocPhamBenhVienVo
                    {
                        DuocPhamBenhVienId = dp.DuocPhamBenhVienId,
                        TenDuocPham = dp.Ten,
                        TenHoatChat = dp.HoatChat,
                        HamLuong = dp.HamLuong,
                        NgayDung = dp.NoiTruPhieuDieuTriId != null ? dp.NoiTruPhieuDieuTri.NgayDieuTri : dp.ThoiDiemChiDinh
                    }).ToList(),
                    YeuCauTruyenMaus = o.YeuCauTruyenMaus.Select(m => new
                    {
                        m.Id,
                        m.TrangThai,
                        m.NoiTruPhieuDieuTriId,
                        m.TenDichVu,
                        m.ThoiDiemChiDinh,
                        m.ThoiGianDienBien
                    }).ToList(),
                })
                .FirstOrDefault();

            var thongTinBacSis = _nhanVienRepository.TableNoTracking.Select(o => new 
            {
                o.Id, 
                o.User.HoTen, 
                MaHocHamHocVi = o.HocHamHocViId != null ? o.HocHamHocVi.Ma : ""
            }).ToList();

            var thongTinGiuongs = _giuongBenhRepository.TableNoTracking.Select(o => new
            {
                o.Id,
                TenPhong = o.PhongBenhVien.Ten,
                TenGiuong = o.Ten
            }).ToList();

            var dichVuKyThuatBenhVienIds = thongTinYeuCauTiepNhan.YeuCauDichVuKyThuats.Select(o => o.DichVuKyThuatBenhVienId).ToList();

            var dichVuKyThuatBenhViens = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(o => dichVuKyThuatBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    TenNhomDichVuBenhVien = o.NhomDichVuBenhVien.Ten,
                    SoThuTuXetNghiem = o.DichVuXetNghiemId == null ? 0 : (o.DichVuXetNghiem.SoThuTu ?? o.DichVuXetNghiemId)
                });

            var yeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking                        
                        .Where(x => x.YeuCauTiepNhanId == thongTinYeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId
                                    && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && x.LaChiDinhTuNoiTru != null && x.LaChiDinhTuNoiTru == true)
                        .Select(k => new
                        {
                            k.Id,
                            k.TenDichVu,
                            k.ThoiDiemChiDinh,
                            k.ThoiGianDienBien,
                            k.ThoiDiemDangKy
                        }).ToList();

            var chiDinhDuocPhams = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Where(o => o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && 
                            ((phieuDieuTriId != null && o.NoiTruPhieuDieuTriId == phieuDieuTriId) || (phieuDieuTriId == null && o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId)))
                .Select(s => new PhieuDieuTriChiDinhThuocVo
                {
                    Id = s.Id,
                    NoiTruPhieuDieuTriId = s.NoiTruPhieuDieuTriId,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    MaHoatChat = s.MaHoatChat,
                    Ma = s.DuocPhamBenhVien.Ma,
                    Ten = s.Ten,
                    HoatChat = s.HoatChat,
                    HamLuong = s.HamLuong,
                    DVT = s.DonViTinh.Ten,
                    SoThuTu = s.SoThuTu,
                    DuongDungId = s.DuongDungId,
                    ThoiGianDungSang = s.ThoiGianDungSang,
                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                    ThoiGianDungToi = s.ThoiGianDungToi,
                    DungSang = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungSang == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungSang.Value), false) + ")"
                                              : s.DungSang == null ? null : "(" + s.DungSang.FloatToStringFraction() + ")",
                    DungTrua = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungTrua == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungTrua.Value), false) + ")"
                                              : s.DungTrua == null ? null : "(" + s.DungTrua.FloatToStringFraction() + ")",
                    DungChieu = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungChieu == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungChieu.Value), false) + ")"
                                              : s.DungChieu == null ? null : "(" + s.DungChieu.FloatToStringFraction() + ")",
                    DungToi = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungToi == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungToi.Value), false) + ")"
                                              : s.DungToi == null ? null : "(" + s.DungToi.FloatToStringFraction() + ")",                    
                    SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                    TenDuongDung = s.DuongDung.Ten,
                    SoLuong = s.SoLuong,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    GhiChu = s.GhiChu,
                    LaDichTruyen = s.LaDichTruyen,
                    LaThuocHuongThanGayNghien = s.DuocPhamBenhVien.DuocPham.LaThuocHuongThanGayNghien,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    ThoiGianDienBien = s.ThoiGianDienBien,
                    TocDoTruyen = s.TocDoTruyen,
                    DonViTocDoTruyen = s.DonViTocDoTruyen,
                    ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                    CachGioTruyenDich = s.CachGioTruyenDich,
                    DuoCPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                    LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                }).ToList();
            var countPage = 1;
            if (thongTinBenhAn != null)
            {
                foreach (var phieuDieuTri in thongTinBenhAn.NoiTruPhieuDieuTris.OrderBy(o => o.NgayDieuTri))
                {
                    if (countPage != 1)
                    {
                        content += "<div class=\"pagebreak\"> </div>";
                    }                    

                    var noiDungToDieuTri = string.Empty;
                    //lstDVKT chua tinh dv kham
                    var phieuDieuTriDichVuKyThuats = thongTinYeuCauTiepNhan.YeuCauDichVuKyThuats
                        .Where(o => o.NoiTruPhieuDieuTriId == phieuDieuTri.Id && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.SuatAn)
                        .Select(o => new
                        {
                            o.Id,
                            SoLuong = o.SoLan,
                            o.TenDichVu,
                            ThoiGianTheoDienBien = o.ThoiGianDienBien ?? o.ThoiDiemChiDinh,
                            o.NoiTruPhieuDieuTriId,
                            o.LoaiDichVuKyThuat,
                            SoThuTuXetNghiem = dichVuKyThuatBenhViens.FirstOrDefault(dv=>dv.Id == o.DichVuKyThuatBenhVienId)?.SoThuTuXetNghiem,
                            TenNhomDichVuBenhVien = dichVuKyThuatBenhViens.FirstOrDefault(dv => dv.Id == o.DichVuKyThuatBenhVienId)?.TenNhomDichVuBenhVien
                        })
                        .OrderBy(x => x.TenNhomDichVuBenhVien)
                        .ThenBy(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ? (x.SoThuTuXetNghiem) : 0)
                        .ThenBy(x => x.ThoiGianTheoDienBien)
                        .ToList();

                    var phieuDieuTriTruyenMaus = thongTinYeuCauTiepNhan.YeuCauTruyenMaus
                        .Where(o => o.NoiTruPhieuDieuTriId == phieuDieuTri.Id && o.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy)
                        .Select(o => new
                        {
                            o.Id,
                            o.TenDichVu,
                            ThoiGianTheoDienBien = o.ThoiGianDienBien ?? o.ThoiDiemChiDinh
                        })
                        .OrderBy(x => x.ThoiGianTheoDienBien)
                        .ToList();

                    var phieuDieuTriDichVuKhams = yeuCauKhams
                        .Where(o => o.ThoiDiemDangKy.Date == phieuDieuTri.NgayDieuTri.Date)
                        .Select(o => new
                        {
                            o.Id,
                            o.TenDichVu,
                            ThoiGianTheoDienBien = o.ThoiGianDienBien ?? o.ThoiDiemChiDinh
                        })
                        .OrderBy(x => x.ThoiGianTheoDienBien)
                        .ToList();

                    var phieuDieuTriDuocPhams = chiDinhDuocPhams
                        .Where(o=>o.NoiTruPhieuDieuTriId == phieuDieuTri.Id && !o.SoLuong.AlmostEqual(0))
                        .OrderBy(o => o.SoThuTu).ThenBy(z => z.DuongDungNumber).OrderBy(o => o.ThoiGianTheoDienBien)
                        .ToList();

                    var dienBiens = new List<NoiTruPhieuDieuTriDienBien>();
                    if (!string.IsNullOrEmpty(phieuDieuTri.DienBien))
                    {
                        dienBiens = (JsonConvert.DeserializeObject<List<NoiTruPhieuDieuTriDienBien>>(phieuDieuTri.DienBien)).OrderBy(o => o.ThoiGian).ToList();
                    }
                    var inTatCaDienBien = dienBienIds == null || dienBienIds.Count() == 0 || dienBiens.Count() == dienBienIds.Count;
                    for (int i = -1; i < dienBiens.Count; i++)
                    {

                        NoiTruPhieuDieuTriDienBien dienBien = i < 0 ? null : dienBiens[i];
                        NoiTruPhieuDieuTriDienBien dienBienSau = (i + 1) < dienBiens.Count ? dienBiens[i + 1] : null;
                        if (!inTatCaDienBien && (dienBien == null || !dienBienIds.Contains(dienBien.IdView))) continue;

                        //nội dung diễn biến: diễn biến + DichVuKyThuat + TruyenMau + DichVuKham + BS
                        var noiDungDienBien = string.Empty;
                        if(dienBien != null && !string.IsNullOrEmpty(dienBien.DienBien))
                        {
                            noiDungDienBien += "<div> " + (dienBien.DienBien.Replace("\n", "<br>")) + " </div>";
                        }
                        //noiDungDienBien += "<div> " + (!string.IsNullOrEmpty(dienBien?.DienBien) ? dienBien?.DienBien.Replace("\n", "<br>") : "") + " </div>";
                        //noiDungDienBien += !string.IsNullOrEmpty(dienBien?.DienBien) ? "<br>" : "";
                        //DichVuKyThuat
                        foreach (var item in phieuDieuTriDichVuKyThuats
                            .Where(o => (dienBien == null || o.ThoiGianTheoDienBien >= dienBien.ThoiGian) && (dienBienSau == null || o.ThoiGianTheoDienBien < dienBienSau.ThoiGian)))
                        {
                            noiDungDienBien += "<div> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
                        }
                        //TruyenMau
                        foreach (var item in phieuDieuTriTruyenMaus
                            .Where(o => (dienBien == null || o.ThoiGianTheoDienBien >= dienBien.ThoiGian) && (dienBienSau == null || o.ThoiGianTheoDienBien < dienBienSau.ThoiGian)))
                        {
                            noiDungDienBien += "<div> - " + item.TenDichVu + " </div>";
                        }
                        //DichVuKham
                        foreach (var item in phieuDieuTriDichVuKhams
                            .Where(o => (dienBien == null || o.ThoiGianTheoDienBien >= dienBien.ThoiGian) && (dienBienSau == null || o.ThoiGianTheoDienBien < dienBienSau.ThoiGian)))
                        {
                            noiDungDienBien += "<div> - " + item.TenDichVu + " </div>";
                        }

                        //BÁC SĨ diễn biến
                        var bacSiDienBien = string.Empty;
                        if (noiDungDienBien != string.Empty)
                        {                            
                            if (dienBien != null)
                            {
                                var bacSiId = dienBien.DienBienLastUserId ?? phieuDieuTri.LastUserId;
                                var bacSi = thongTinBacSis.FirstOrDefault(s => s.Id == bacSiId);
                                if (bacSi != null)
                                {
                                    bacSiDienBien = (!string.IsNullOrEmpty(bacSi.MaHocHamHocVi) ? (bacSi.MaHocHamHocVi + ". ") : "") + bacSi.HoTen;
                                }
                            }
                            //noiDungDienBien += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                            //noiDungDienBien += "<p style='text-align: center;height:30px'></p>";
                            //noiDungDienBien += "<p style='text-align: center;'><b> " + bacSiDienBien + "</b></p>";
                        }                        

                        //nội dung y lệnh: DuocPhams + y lệnh + Chế độ ăn + Chế độ chăm sóc + BS
                        var noiDungYLenh = string.Empty;
                        //DuocPhams
                        foreach (var item in phieuDieuTriDuocPhams
                            .Where(o => (dienBien == null || o.ThoiGianTheoDienBien >= dienBien.ThoiGian) && (dienBienSau == null || o.ThoiGianTheoDienBien < dienBienSau.ThoiGian)))
                        {
                            //DỊCH TRUYỀN
                            if (item.LaDichTruyen == true)
                            {
                                noiDungYLenh += "<div><b>" +
                                         ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid)
                                             ? "(" + GetSoNgayDungThuocGayNghien(thongTinYeuCauTiepNhan.NgayDungDuocPhamBenhVienVos, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId, item.Ten, item.HoatChat, item.HamLuong) + ") "
                                             : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                        (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>" + " " + item.DVT + "</b></div>";
                                noiDungYLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
                                             !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
                                             !string.IsNullOrEmpty(item.GhiChu) ?
                                             "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
                                             item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

                                var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
                                if (thoiGianBatDauTruyen != null)
                                {
                                    if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
                                    {
                                        for (int j = 0; j < item.SoLanDungTrongNgay; j++)
                                        {
                                            var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                            thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
                                            item.GioSuDung += time + "; ";
                                        }
                                    }
                                    else
                                    {
                                        item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                    }
                                }
                                noiDungYLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
                                     !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
                                     !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

                                     "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
                                     (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
                                     " " + (item.CachGioTruyenDich != null
                                         ? "cách " + item.CachGioTruyenDich + " giờ,"
                                         : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
                                         ? "giờ sử dụng: " + item.GioSuDung
                                         : "") + "</div>"

                                     : "";
                            }
                            else
                            {
                                noiDungYLenh += "<div><b>" + ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid) ? "(" + GetSoNgayDungThuocGayNghien(thongTinYeuCauTiepNhan.NgayDungDuocPhamBenhVienVos, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId, item.Ten, item.HoatChat, item.HamLuong) + ") " : "") +
                                          (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                         (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                                noiDungYLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : ""; ;
                                noiDungYLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
                                             ? (" " + item.ThoiGianDungSangDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
                                                                                              !string.IsNullOrEmpty(item.DungChieu) ||
                                                                                              !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
                                             ? (" " + item.ThoiGianDungTruaDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
                                                                                              !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
                                             ? (" " + item.ThoiGianDungChieuDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
                                             ? (" " + item.ThoiGianDungToiDisplay)
                                             : "") + "</div>";
                            }
                        }
                        //y lệnh
                        if (dienBien != null && !string.IsNullOrEmpty(dienBien.YLenh))
                        {
                            noiDungYLenh += "<div>" + (dienBien.YLenh.Replace("\n", "<br>")) + " </div>";
                        }

                        //CHẾ ĐỘ ĂN
                        if (dienBien != null && !string.IsNullOrEmpty(dienBien.CheDoAn))
                        {
                            noiDungYLenh += "<div><b>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div>";
                        }

                        //CHẾ ĐỘ CHĂM SÓC
                        if (dienBien != null && !string.IsNullOrEmpty(dienBien.CheDoChamSoc))
                        {
                            noiDungYLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
                        }
                        //BÁC SĨ y lệnh
                        var bacSiYLenh = string.Empty;
                        if (noiDungYLenh != string.Empty)
                        {
                            
                            if (dienBien != null)
                            {
                                var bacSiId = dienBien.YLenhLastUserId ?? phieuDieuTri.LastUserId;
                                var bacSi = thongTinBacSis.FirstOrDefault(s => s.Id == bacSiId);
                                if (bacSi != null)
                                {
                                    bacSiYLenh = (!string.IsNullOrEmpty(bacSi.MaHocHamHocVi) ? (bacSi.MaHocHamHocVi + ". ") : "") + bacSi.HoTen;
                                }
                            }
                            //noiDungYLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                            //noiDungYLenh += "<p style='text-align: center;height:30px'></p>";
                            //noiDungYLenh += "<p style='text-align: center;'><b> " + bacSiYLenh + "</b></p>";
                        }

                        var ngayGio = string.Empty;
                        if(dienBien != null)
                        {
                            ngayGio = $"{dienBien.ThoiGian.Hour.ToString("00")} giờ {dienBien.ThoiGian.Minute.ToString("00")} phút, {dienBien.ThoiGian.ApplyFormatDate()}";
                        }

                        if(noiDungDienBien != string.Empty || noiDungYLenh != string.Empty)
                        {                            
                            noiDungToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\"><div>" + ngayGio + "</div> </td>";
                            noiDungToDieuTri += "<td class=\"contain-grid\" style=\"vertical-align: top;\"><div>" + noiDungDienBien + "</div> </td>";
                            noiDungToDieuTri += "<td class=\"contain-grid\"  style=\"vertical-align: top;\"><div>" + noiDungYLenh + "</div> </td> </tr>";
                            //BÁC SĨ
                            var htmlBacSiDienBien = string.Empty;
                            var htmlBacSiYLenh = string.Empty;
                            if (noiDungDienBien != string.Empty)
                            {
                                htmlBacSiDienBien += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                                htmlBacSiDienBien += "<p style='text-align: center;height:30px'></p>";
                                htmlBacSiDienBien += "<p style='text-align: center;'><b> " + (string.IsNullOrEmpty(bacSiDienBien) ? "&nbsp;" : bacSiDienBien) + "</b></p>";
                            }
                            if (noiDungYLenh != string.Empty)
                            {
                                htmlBacSiYLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                                htmlBacSiYLenh += "<p style='text-align: center;height:30px'></p>";
                                htmlBacSiYLenh += "<p style='text-align: center;'><b> " + (string.IsNullOrEmpty(bacSiYLenh) ? "&nbsp;" : bacSiYLenh) + "</b></p>";
                            }
                            noiDungToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\"></td>";
                            noiDungToDieuTri += "<td class=\"contain-grid\" style=\"vertical-align: top;\"><div>" + htmlBacSiDienBien + "</div> </td>";
                            noiDungToDieuTri += "<td class=\"contain-grid\"  style=\"vertical-align: top;\"><div>" + htmlBacSiYLenh + "</div> </td> </tr>";
                        }
                    }
                    
                    var buong = string.Empty;
                    var giuong = string.Empty;
                    var yeuCauDichVuGiuong = thongTinYeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
                        .Where(o => o.TrangThai != EnumTrangThaiGiuongBenh.DaHuy && o.DoiTuongSuDung == DoiTuongSuDung.BenhNhan && o.GiuongBenhId != null)
                        .OrderBy(o=>o.ThoiDiemBatDauSuDung).LastOrDefault();
                    if(yeuCauDichVuGiuong != null)
                    {
                        var thongTinGiuong = thongTinGiuongs.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.GiuongBenhId);
                        if(thongTinGiuong != null)
                        {
                            giuong = thongTinGiuong.TenGiuong;
                            buong = thongTinGiuong.TenPhong;
                        }
                    }
                    var data = new
                    {
                        So = "",
                        SoVaoVien = thongTinBenhAn.SoBenhAn ?? "",
                        HoTen = thongTinYeuCauTiepNhan.HoTen,
                        Tuoi = CalculateHelper.TinhTuoiHienThiTrenBieuMau(thongTinYeuCauTiepNhan.NgaySinh, thongTinYeuCauTiepNhan.ThangSinh, thongTinYeuCauTiepNhan.NamSinh, phieuDieuTri.NgayDieuTri),
                        GioiTinh = (thongTinYeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNam || thongTinYeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNu) ? thongTinYeuCauTiepNhan.GioiTinh.GetDescription() : "",
                        Khoa = phieuDieuTri.TenKhoaPhongDieuTri?.Replace("Khoa", string.Empty),
                        Buong = buong,
                        Giuong = giuong,
                        ChanDoan = "<b>" + phieuDieuTri.ChanDoanChinhGhiChu + (phieuDieuTri.GhiChuThamKhamChanDoanKemTheos.Count > 0 ? "; " + string.Join("; ", phieuDieuTri.GhiChuThamKhamChanDoanKemTheos) : "") + "</b>",
                        ToDieuTri = noiDungToDieuTri
                    };

                    content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                    countPage++;
                }
            }
            return content;
        }

        public async Task<List<string>> GetContentInPhieuThamKham(long yeuCauTiepNhanId, long? phieuDieuTriId = null, List<long> dienBienIds = null)
        {
            
            var contents = new List<string>();
            var html = _templateRepository.TableNoTracking.OrderByDescending(k => k.Version)
                .Where(o => o.Name == "ToDieuTri").Select(o => o.Body).FirstOrDefault();

            var thongTinBenhAn = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => o.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.SoBenhAn,
                    NoiTruPhieuDieuTris = o.NoiTruPhieuDieuTris.Where(p => phieuDieuTriId == null || p.Id == phieuDieuTriId).Select(p => new
                    {
                        p.Id,
                        TenKhoaPhongDieuTri = p.KhoaPhongDieuTri.Ten,
                        p.ChanDoanChinhGhiChu,
                        GhiChuThamKhamChanDoanKemTheos = p.NoiTruThamKhamChanDoanKemTheos.Select(cdkt => cdkt.GhiChu).ToList(),
                        p.DienBien,
                        p.NgayDieuTri,
                        p.ThoiDiemThamKham,
                        p.CheDoAn,
                        p.CheDoChamSoc,
                        p.LastUserId
                    }),
                    KhoaPhongDieuTris = o.NoiTruKhoaPhongDieuTris.Select(k => new
                    {
                        k.Id,
                        k.ThoiDiemVaoKhoa,
                        TenKhoaPhongChuyenDen =
                        k.KhoaPhongChuyenDen.Ten
                    }).ToList(),
                    NoiTruEkipDieuTris = o.NoiTruEkipDieuTris
                })
                .FirstOrDefault();

            var thongTinYeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    o.HoTen,
                    o.NgaySinh,
                    o.ThangSinh,
                    o.NamSinh,
                    o.GioiTinh,
                    YeuCauDichVuGiuongBenhViens = o.YeuCauDichVuGiuongBenhViens.Select(g => new
                    {
                        g.Id,
                        g.TrangThai,
                        g.DoiTuongSuDung,
                        g.ThoiDiemBatDauSuDung,
                        g.GiuongBenhId
                    }).ToList(),
                    YeuCauDichVuKyThuats = o.YeuCauDichVuKyThuats.Select(kt => new
                    {
                        kt.Id,
                        kt.TrangThai,
                        kt.LoaiDichVuKyThuat,
                        kt.NoiTruPhieuDieuTriId,
                        kt.ThoiDiemChiDinh,
                        kt.ThoiGianDienBien,
                        kt.TenDichVu,
                        kt.DichVuKyThuatBenhVienId,
                        kt.SoLan
                    }).ToList(),
                    NgayDungDuocPhamBenhVienVos = o.YeuCauDuocPhamBenhViens.Where(dp => dp.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && dp.SoLuong > 0).Select(dp => new NgayDungDuocPhamBenhVienVo
                    {
                        DuocPhamBenhVienId = dp.DuocPhamBenhVienId,
                        TenDuocPham = dp.Ten,
                        TenHoatChat = dp.HoatChat,
                        HamLuong = dp.HamLuong,
                        NgayDung = dp.NoiTruPhieuDieuTriId != null ? dp.NoiTruPhieuDieuTri.NgayDieuTri : dp.ThoiDiemChiDinh
                    }).ToList(),
                    YeuCauTruyenMaus = o.YeuCauTruyenMaus.Select(m => new
                    {
                        m.Id,
                        m.TrangThai,
                        m.NoiTruPhieuDieuTriId,
                        m.TenDichVu,
                        m.ThoiDiemChiDinh,
                        m.ThoiGianDienBien
                    }).ToList(),
                })
                .FirstOrDefault();

            var thongTinBacSis = _nhanVienRepository.TableNoTracking.Select(o => new
            {
                o.Id,
                o.User.HoTen,
                MaHocHamHocVi = o.HocHamHocViId != null ? o.HocHamHocVi.Ma : ""
            }).ToList();

            var thongTinGiuongs = _giuongBenhRepository.TableNoTracking.Select(o => new
            {
                o.Id,
                TenPhong = o.PhongBenhVien.Ten,
                TenGiuong = o.Ten
            }).ToList();

            var dichVuKyThuatBenhVienIds = thongTinYeuCauTiepNhan.YeuCauDichVuKyThuats.Select(o => o.DichVuKyThuatBenhVienId).ToList();

            var dichVuKyThuatBenhViens = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(o => dichVuKyThuatBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    TenNhomDichVuBenhVien = o.NhomDichVuBenhVien.Ten,
                    SoThuTuXetNghiem = o.DichVuXetNghiemId == null ? 0 : (o.DichVuXetNghiem.SoThuTu ?? o.DichVuXetNghiemId)
                });

            var yeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhanId == thongTinYeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId
                                    && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && x.LaChiDinhTuNoiTru != null && x.LaChiDinhTuNoiTru == true)
                        .Select(k => new
                        {
                            k.Id,
                            k.TenDichVu,
                            k.ThoiDiemChiDinh,
                            k.ThoiGianDienBien,
                            k.ThoiDiemDangKy
                        }).ToList();

            var chiDinhDuocPhams = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Where(o => o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                            ((phieuDieuTriId != null && o.NoiTruPhieuDieuTriId == phieuDieuTriId) || (phieuDieuTriId == null && o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId)))
                .Select(s => new PhieuDieuTriChiDinhThuocVo
                {
                    Id = s.Id,
                    NoiTruPhieuDieuTriId = s.NoiTruPhieuDieuTriId,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    MaHoatChat = s.MaHoatChat,
                    Ma = s.DuocPhamBenhVien.Ma,
                    Ten = s.Ten,
                    HoatChat = s.HoatChat,
                    HamLuong = s.HamLuong,
                    DVT = s.DonViTinh.Ten,
                    SoThuTu = s.SoThuTu,
                    DuongDungId = s.DuongDungId,
                    ThoiGianDungSang = s.ThoiGianDungSang,
                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                    ThoiGianDungToi = s.ThoiGianDungToi,
                    DungSang = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungSang == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungSang.Value), false) + ")"
                                              : s.DungSang == null ? null : "(" + s.DungSang.FloatToStringFraction() + ")",
                    DungTrua = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungTrua == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungTrua.Value), false) + ")"
                                              : s.DungTrua == null ? null : "(" + s.DungTrua.FloatToStringFraction() + ")",
                    DungChieu = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungChieu == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungChieu.Value), false) + ")"
                                              : s.DungChieu == null ? null : "(" + s.DungChieu.FloatToStringFraction() + ")",
                    DungToi = (s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan) ?
                                               s.DungToi == null ? null : "(" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungToi.Value), false) + ")"
                                              : s.DungToi == null ? null : "(" + s.DungToi.FloatToStringFraction() + ")",
                    SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                    TenDuongDung = s.DuongDung.Ten,
                    SoLuong = s.SoLuong,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    GhiChu = s.GhiChu,
                    LaDichTruyen = s.LaDichTruyen,
                    LaThuocHuongThanGayNghien = s.DuocPhamBenhVien.DuocPham.LaThuocHuongThanGayNghien,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    ThoiGianDienBien = s.ThoiGianDienBien,
                    TocDoTruyen = s.TocDoTruyen,
                    DonViTocDoTruyen = s.DonViTocDoTruyen,
                    ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                    CachGioTruyenDich = s.CachGioTruyenDich,
                    DuoCPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                    LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                }).ToList();
            if (thongTinBenhAn != null)
            {
                foreach (var phieuDieuTri in thongTinBenhAn.NoiTruPhieuDieuTris.OrderBy(o => o.NgayDieuTri))
                {
                    //if (countPage != 1)
                    //{
                    //    content += "<div class=\"pagebreak\"> </div>";
                    //}

                    var noiDungToDieuTri = string.Empty;
                    //lstDVKT chua tinh dv kham
                    var phieuDieuTriDichVuKyThuats = thongTinYeuCauTiepNhan.YeuCauDichVuKyThuats
                        .Where(o => o.NoiTruPhieuDieuTriId == phieuDieuTri.Id && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.SuatAn)
                        .Select(o => new
                        {
                            o.Id,
                            SoLuong = o.SoLan,
                            o.TenDichVu,
                            ThoiGianTheoDienBien = o.ThoiGianDienBien ?? o.ThoiDiemChiDinh,
                            o.NoiTruPhieuDieuTriId,
                            o.LoaiDichVuKyThuat,
                            SoThuTuXetNghiem = dichVuKyThuatBenhViens.FirstOrDefault(dv => dv.Id == o.DichVuKyThuatBenhVienId)?.SoThuTuXetNghiem,
                            TenNhomDichVuBenhVien = dichVuKyThuatBenhViens.FirstOrDefault(dv => dv.Id == o.DichVuKyThuatBenhVienId)?.TenNhomDichVuBenhVien
                        })
                        .OrderBy(x => x.TenNhomDichVuBenhVien)
                        .ThenBy(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ? (x.SoThuTuXetNghiem) : 0)
                        .ThenBy(x => x.ThoiGianTheoDienBien)
                        .ToList();

                    var phieuDieuTriTruyenMaus = thongTinYeuCauTiepNhan.YeuCauTruyenMaus
                        .Where(o => o.NoiTruPhieuDieuTriId == phieuDieuTri.Id && o.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy)
                        .Select(o => new
                        {
                            o.Id,
                            o.TenDichVu,
                            ThoiGianTheoDienBien = o.ThoiGianDienBien ?? o.ThoiDiemChiDinh
                        })
                        .OrderBy(x => x.ThoiGianTheoDienBien)
                        .ToList();

                    var phieuDieuTriDichVuKhams = yeuCauKhams
                        .Where(o => o.ThoiDiemDangKy.Date == phieuDieuTri.NgayDieuTri.Date)
                        .Select(o => new
                        {
                            o.Id,
                            o.TenDichVu,
                            ThoiGianTheoDienBien = o.ThoiGianDienBien ?? o.ThoiDiemChiDinh
                        })
                        .OrderBy(x => x.ThoiGianTheoDienBien)
                        .ToList();

                    var phieuDieuTriDuocPhams = chiDinhDuocPhams
                        .Where(o => o.NoiTruPhieuDieuTriId == phieuDieuTri.Id && !o.SoLuong.AlmostEqual(0))
                        .OrderBy(o => o.SoThuTu).ThenBy(z => z.DuongDungNumber).OrderBy(o => o.ThoiGianTheoDienBien)
                        .ToList();

                    var dienBiens = new List<NoiTruPhieuDieuTriDienBien>();
                    if (!string.IsNullOrEmpty(phieuDieuTri.DienBien))
                    {
                        dienBiens = (JsonConvert.DeserializeObject<List<NoiTruPhieuDieuTriDienBien>>(phieuDieuTri.DienBien)).OrderBy(o => o.ThoiGian).ToList();
                    }
                    var inTatCaDienBien = dienBienIds == null || dienBienIds.Count() == 0 || dienBiens.Count() == dienBienIds.Count;
                    for (int i = -1; i < dienBiens.Count; i++)
                    {

                        NoiTruPhieuDieuTriDienBien dienBien = i < 0 ? null : dienBiens[i];
                        NoiTruPhieuDieuTriDienBien dienBienSau = (i + 1) < dienBiens.Count ? dienBiens[i + 1] : null;
                        if (!inTatCaDienBien && (dienBien == null || !dienBienIds.Contains(dienBien.IdView))) continue;

                        //nội dung diễn biến: diễn biến + DichVuKyThuat + TruyenMau + DichVuKham + BS
                        var noiDungDienBien = string.Empty;
                        if (dienBien != null && !string.IsNullOrEmpty(dienBien.DienBien))
                        {
                            noiDungDienBien += "<div> " + (dienBien.DienBien.Replace("\n", "<br>")) + " </div>";
                        }
                        //noiDungDienBien += "<div> " + (!string.IsNullOrEmpty(dienBien?.DienBien) ? dienBien?.DienBien.Replace("\n", "<br>") : "") + " </div>";
                        //noiDungDienBien += !string.IsNullOrEmpty(dienBien?.DienBien) ? "<br>" : "";
                        //DichVuKyThuat
                        foreach (var item in phieuDieuTriDichVuKyThuats
                            .Where(o => (dienBien == null || o.ThoiGianTheoDienBien >= dienBien.ThoiGian) && (dienBienSau == null || o.ThoiGianTheoDienBien < dienBienSau.ThoiGian)))
                        {
                            noiDungDienBien += "<div> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
                        }
                        //TruyenMau
                        foreach (var item in phieuDieuTriTruyenMaus
                            .Where(o => (dienBien == null || o.ThoiGianTheoDienBien >= dienBien.ThoiGian) && (dienBienSau == null || o.ThoiGianTheoDienBien < dienBienSau.ThoiGian)))
                        {
                            noiDungDienBien += "<div> - " + item.TenDichVu + " </div>";
                        }
                        //DichVuKham
                        foreach (var item in phieuDieuTriDichVuKhams
                            .Where(o => (dienBien == null || o.ThoiGianTheoDienBien >= dienBien.ThoiGian) && (dienBienSau == null || o.ThoiGianTheoDienBien < dienBienSau.ThoiGian)))
                        {
                            noiDungDienBien += "<div> - " + item.TenDichVu + " </div>";
                        }

                        //BÁC SĨ diễn biến
                        var bacSiDienBien = string.Empty;
                        if (noiDungDienBien != string.Empty)
                        {
                            if (dienBien != null)
                            {
                                var bacSiId = dienBien.DienBienLastUserId ?? phieuDieuTri.LastUserId;
                                var bacSi = thongTinBacSis.FirstOrDefault(s => s.Id == bacSiId);
                                if (bacSi != null)
                                {
                                    bacSiDienBien = (!string.IsNullOrEmpty(bacSi.MaHocHamHocVi) ? (bacSi.MaHocHamHocVi + ". ") : "") + bacSi.HoTen;
                                }
                            }
                            noiDungDienBien += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                            noiDungDienBien += "<p style='text-align: center;height:30px'></p>";
                            noiDungDienBien += "<p style='text-align: center;'><b> " + (string.IsNullOrEmpty(bacSiDienBien) ? "&nbsp;" : bacSiDienBien) + "</b></p>";
                        }

                        //nội dung y lệnh: DuocPhams + y lệnh + Chế độ ăn + Chế độ chăm sóc + BS
                        var noiDungYLenh = string.Empty;
                        //DuocPhams
                        foreach (var item in phieuDieuTriDuocPhams
                            .Where(o => (dienBien == null || o.ThoiGianTheoDienBien >= dienBien.ThoiGian) && (dienBienSau == null || o.ThoiGianTheoDienBien < dienBienSau.ThoiGian)))
                        {
                            //DỊCH TRUYỀN
                            if (item.LaDichTruyen == true)
                            {
                                noiDungYLenh += "<div><b>" +
                                         ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid)
                                             ? "(" + GetSoNgayDungThuocGayNghien(thongTinYeuCauTiepNhan.NgayDungDuocPhamBenhVienVos, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId, item.Ten, item.HoatChat, item.HamLuong) + ") "
                                             : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                        (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>" + " " + item.DVT + "</b></div>";
                                noiDungYLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
                                             !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
                                             !string.IsNullOrEmpty(item.GhiChu) ?
                                             "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
                                             item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

                                var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
                                if (thoiGianBatDauTruyen != null)
                                {
                                    if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
                                    {
                                        for (int j = 0; j < item.SoLanDungTrongNgay; j++)
                                        {
                                            var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                            thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
                                            item.GioSuDung += time + "; ";
                                        }
                                    }
                                    else
                                    {
                                        item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                    }
                                }
                                noiDungYLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
                                     !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
                                     !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

                                     "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
                                     (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
                                     " " + (item.CachGioTruyenDich != null
                                         ? "cách " + item.CachGioTruyenDich + " giờ,"
                                         : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
                                         ? "giờ sử dụng: " + item.GioSuDung
                                         : "") + "</div>"

                                     : "";
                            }
                            else
                            {
                                noiDungYLenh += "<div><b>" + ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid) ? "(" + GetSoNgayDungThuocGayNghien(thongTinYeuCauTiepNhan.NgayDungDuocPhamBenhVienVos, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId, item.Ten, item.HoatChat, item.HamLuong) + ") " : "") +
                                          (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                         (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                                noiDungYLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : ""; ;
                                noiDungYLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
                                             ? (" " + item.ThoiGianDungSangDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
                                                                                              !string.IsNullOrEmpty(item.DungChieu) ||
                                                                                              !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
                                             ? (" " + item.ThoiGianDungTruaDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
                                                                                              !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
                                             ? (" " + item.ThoiGianDungChieuDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
                                             ? (" " + item.ThoiGianDungToiDisplay)
                                             : "") + "</div>";
                            }
                        }
                        //y lệnh
                        if (dienBien != null && !string.IsNullOrEmpty(dienBien.YLenh))
                        {
                            noiDungYLenh += "<div>" + (dienBien.YLenh.Replace("\n", "<br>")) + " </div>";
                        }

                        //CHẾ ĐỘ ĂN
                        if (dienBien != null && !string.IsNullOrEmpty(dienBien.CheDoAn))
                        {
                            noiDungYLenh += "<div><b>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div>";
                        }

                        //CHẾ ĐỘ CHĂM SÓC
                        if (dienBien != null && !string.IsNullOrEmpty(dienBien.CheDoChamSoc))
                        {
                            noiDungYLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
                        }
                        //BÁC SĨ y lệnh
                        var bacSiYLenh = string.Empty;
                        if (noiDungYLenh != string.Empty)
                        {

                            if (dienBien != null)
                            {
                                var bacSiId = dienBien.YLenhLastUserId ?? phieuDieuTri.LastUserId;
                                var bacSi = thongTinBacSis.FirstOrDefault(s => s.Id == bacSiId);
                                if (bacSi != null)
                                {
                                    bacSiYLenh = (!string.IsNullOrEmpty(bacSi.MaHocHamHocVi) ? (bacSi.MaHocHamHocVi + ". ") : "") + bacSi.HoTen;
                                }
                            }
                            noiDungYLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                            noiDungYLenh += "<p style='text-align: center;height:30px'></p>";
                            noiDungYLenh += "<p style='text-align: center;'><b> " + (string.IsNullOrEmpty(bacSiYLenh) ? "&nbsp;" : bacSiYLenh) + "</b></p>";
                        }

                        var ngayGio = string.Empty;
                        if (dienBien != null)
                        {
                            ngayGio = $"{dienBien.ThoiGian.Hour.ToString("00")} giờ {dienBien.ThoiGian.Minute.ToString("00")} phút, {dienBien.ThoiGian.ApplyFormatDate()}";
                        }

                        if (noiDungDienBien != string.Empty || noiDungYLenh != string.Empty)
                        {
                            noiDungToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\"><div>" + ngayGio + "</div> </td>";
                            noiDungToDieuTri += "<td class=\"contain-grid\" style=\"vertical-align: top;\"><div>" + noiDungDienBien + "</div> </td>";
                            noiDungToDieuTri += "<td class=\"contain-grid\"  style=\"vertical-align: top;\"><div>" + noiDungYLenh + "</div> </td> </tr>";
                            ////BÁC SĨ
                            //var htmlBacSiDienBien = string.Empty;
                            //var htmlBacSiYLenh = string.Empty;
                            //if (noiDungDienBien != string.Empty)
                            //{
                            //    htmlBacSiDienBien += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                            //    htmlBacSiDienBien += "<p style='text-align: center;height:30px'></p>";
                            //    htmlBacSiDienBien += "<p style='text-align: center;'><b> " + (string.IsNullOrEmpty(bacSiDienBien) ? "&nbsp;" : bacSiDienBien) + "</b></p>";
                            //}
                            //if (noiDungYLenh != string.Empty)
                            //{
                            //    htmlBacSiYLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                            //    htmlBacSiYLenh += "<p style='text-align: center;height:30px'></p>";
                            //    htmlBacSiYLenh += "<p style='text-align: center;'><b> " + (string.IsNullOrEmpty(bacSiYLenh) ? "&nbsp;" : bacSiYLenh) + "</b></p>";
                            //}
                            //noiDungToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\"></td>";
                            //noiDungToDieuTri += "<td class=\"contain-grid\" style=\"vertical-align: top;\"><div>" + htmlBacSiDienBien + "</div> </td>";
                            //noiDungToDieuTri += "<td class=\"contain-grid\"  style=\"vertical-align: top;\"><div>" + htmlBacSiYLenh + "</div> </td> </tr>";
                        }
                    }

                    var buong = string.Empty;
                    var giuong = string.Empty;
                    var yeuCauDichVuGiuong = thongTinYeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
                        .Where(o => o.TrangThai != EnumTrangThaiGiuongBenh.DaHuy && o.DoiTuongSuDung == DoiTuongSuDung.BenhNhan && o.GiuongBenhId != null)
                        .OrderBy(o => o.ThoiDiemBatDauSuDung).LastOrDefault();
                    if (yeuCauDichVuGiuong != null)
                    {
                        var thongTinGiuong = thongTinGiuongs.FirstOrDefault(o => o.Id == yeuCauDichVuGiuong.GiuongBenhId);
                        if (thongTinGiuong != null)
                        {
                            giuong = thongTinGiuong.TenGiuong;
                            buong = thongTinGiuong.TenPhong;
                        }
                    }
                    var data = new
                    {
                        So = "",
                        SoVaoVien = thongTinBenhAn.SoBenhAn ?? "",
                        HoTen = thongTinYeuCauTiepNhan.HoTen,
                        Tuoi = CalculateHelper.TinhTuoiHienThiTrenBieuMau(thongTinYeuCauTiepNhan.NgaySinh, thongTinYeuCauTiepNhan.ThangSinh, thongTinYeuCauTiepNhan.NamSinh, phieuDieuTri.NgayDieuTri),
                        GioiTinh = (thongTinYeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNam || thongTinYeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNu) ? thongTinYeuCauTiepNhan.GioiTinh.GetDescription() : "",
                        Khoa = phieuDieuTri.TenKhoaPhongDieuTri?.Replace("Khoa", string.Empty),
                        Buong = buong,
                        Giuong = giuong,
                        ChanDoan = "<b>" + phieuDieuTri.ChanDoanChinhGhiChu + (phieuDieuTri.GhiChuThamKhamChanDoanKemTheos.Count > 0 ? "; " + string.Join("; ", phieuDieuTri.GhiChuThamKhamChanDoanKemTheos) : "") + "</b>",
                        ToDieuTri = noiDungToDieuTri
                    };

                    var content = TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                    contents.Add(content);
                }
            }
            return contents;
        }

        /*
        private async Task<string> getContent(YeuCauTiepNhan entity, NoiTruPhieuDieuTri phieuDieuTri, long yeuCauTiepNhanId)
        {

            var content = "";
            var html = _danhSachChoKhamService.GetBodyByName("ToDieuTri");
            var soThuTu = entity.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderBy(o => o.NgayDieuTri).IndexOf(phieuDieuTri) + 1;
            var cdkt = phieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Select(o => o.GhiChu).ToArray();
            
            var noiDungToDieuTri = string.Empty;

            var lstDVKT = await _phauThuatThuThuatService.GetDichVuKyThuatsByYeuCauTiepNhan(yeuCauTiepNhanId, phieuDieuTri.Id);
            var lstThuoc = await _dieuTriNoiTruService.GetDanhSachThuoc(yeuCauTiepNhanId, phieuDieuTri.Id);

            if (!string.IsNullOrEmpty(phieuDieuTri.DienBien))
            {
                var dienBiens = JsonConvert.DeserializeObject<List<NoiTruPhieuDieuTriDienBien>>(phieuDieuTri.DienBien);
                var i = 0;
                dienBiens.OrderBy(o => o.ThoiGian).ToList().ForEach(dienBien =>
                {

                    var bacSiThoiGianDienBienNull = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
                        ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                            .Where(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
                            .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";


                    var bacSiThoiGianDienBienKhacNull = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s =>
                                                                              (s.TuNgay <= dienBien.ThoiGian &&
                                                                              (s.DenNgay == null || dienBien.ThoiGian <= s.DenNgay.Value)))
                       ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                           .Where(s =>
                                       (s.TuNgay <= dienBien.ThoiGian && (s.DenNgay == null || dienBien.ThoiGian <= s.DenNgay.Value)))
                           .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";


                    var ngayGio = string.Empty;
                    var dienBienBenh = string.Empty;
                    var yLenh = string.Empty;

                    if (i == 0 && (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian) ||
                        lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian)))
                    {
                        ngayGio += "Trước " + dienBien.ThoiGian.Hour + " giờ " + dienBien.ThoiGian.Minute + " phút, " + dienBien.ThoiGian.ApplyFormatDate();
                        
                        if (lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
                        {
                            foreach (var item in lstDVKT.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
                            {
                                dienBienBenh += "<div style='padding:1px;'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
                            }
                        }
                        
                        if (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
                        {
                            foreach (var item in lstThuoc.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh < dienBien.ThoiGian))
                            {
                                //DỊCH TRUYỀN
                                if (item.LaDichTruyen == true)
                                {
                                    yLenh += "<div><b>" +
                                             ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid)
                                                 ? "(" + GetSoNgayDungThuocGayNghien(entity.YeuCauDuocPhamBenhViens, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId) +
                                                   ") "
                                                 : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                             (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                                    yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
                                             !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
                                             !string.IsNullOrEmpty(item.GhiChu) ?
                                             "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
                                             item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

                                    var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
                                    if (thoiGianBatDauTruyen != null)
                                    {
                                        if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
                                        {
                                            for (int j = 0; j < item.SoLanDungTrongNgay; j++)
                                            {
                                                var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                                thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
                                                item.GioSuDung += time + "; ";
                                            }
                                        }
                                        else
                                        {
                                            item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                        }
                                    }
                                    yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
                                    !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
                                    !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

                                    "<p style='margin-left:15px;margin-bottom:0.1cm'>" +
                                    (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
                                    " " + (item.CachGioTruyenDich != null
                                        ? "cách " + item.CachGioTruyenDich + " giờ,"
                                        : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
                                        ? "giờ sử dụng: " + item.GioSuDung
                                        : "") + "</p>"

                                    : "";
                                }
                                else
                                {
                                    yLenh += "<div><b>" + ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid) ? "(" + GetSoNgayDungThuocGayNghien(entity.YeuCauDuocPhamBenhViens, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId) + ") " : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId))
                                              + " x " +
                                             (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                                    yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<p style='margin-left:15px'>" + item.GhiChu + "</p>" : ""; ;
                                    yLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
                                             (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
                                                 ? (" " + item.ThoiGianDungSangDisplay)
                                                 : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
                                                                                                  !string.IsNullOrEmpty(item.DungChieu) ||
                                                                                                  !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                             (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
                                             (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
                                                 ? (" " + item.ThoiGianDungTruaDisplay)
                                                 : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
                                                                                                  !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                             (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
                                             (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
                                                 ? (" " + item.ThoiGianDungChieuDisplay)
                                                 : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                             (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
                                             (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
                                                 ? (" " + item.ThoiGianDungToiDisplay)
                                                 : "") + "</div>";
                                }
                            }
                        }

                        //CHẾ ĐỘ ĂN
                        yLenh += "<div><b>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div>";
                        yLenh += "<br>";
                        //CHẾ ĐỘ CHĂM SÓC
                        yLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
                        //BÁC SĨ
                        yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                        yLenh += "<p style='text-align: center;height:30px'></p>";

                        if (dienBien.ThoiGian != null)
                        {
                            yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienKhacNull + "</p>";
                        }
                        else
                        {
                            yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienNull + "</p>";
                        }

                        if (i == 0)
                        {
                            noiDungToDieuTri += "<tr style='border-bottom:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\"> ";
                        }

                        noiDungToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                        noiDungToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
                        noiDungToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
                    }
                    ngayGio = string.Empty;
                    dienBienBenh = string.Empty;
                    yLenh = string.Empty;
                    ngayGio += dienBien.ThoiGian.Hour + " giờ " + dienBien.ThoiGian.Minute + " phút, " + dienBien.ThoiGian.ApplyFormatDate();
                    //DIỄN BIẾN
                    dienBienBenh += "<div> " + (!string.IsNullOrEmpty(dienBien?.DienBien) ? dienBien?.DienBien.Replace("\n", "<br>") : "") + " </div>";
                    dienBienBenh += !string.IsNullOrEmpty(dienBien?.DienBien) ? "<br>" : "";
                    //DVKT
                    if (lstDVKT.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
                    {
                        foreach (var item in lstDVKT.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
                        {
                            dienBienBenh += "<div style='padding;1px'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
                        }
                    }
                    //Y lệnh
                    yLenh += "<div>" + (!string.IsNullOrEmpty(dienBien?.YLenh) ? dienBien?.YLenh.Replace("\n", "<br>") : "") + " </div>";
                    yLenh += !string.IsNullOrEmpty(dienBien?.YLenh) ? "<br>" : "";
                    ////THUỐC
                    if (lstThuoc.Any(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
                    {
                        foreach (var item in lstThuoc.Where(o => o.ThoiDiemChiDinh != null && o.ThoiDiemChiDinh >= dienBien.ThoiGian && (i == dienBiens.Count - 1 || o.ThoiDiemChiDinh < dienBiens[i + 1].ThoiGian)))
                        {
                            //DỊCH TRUYỀN
                            if (item.LaDichTruyen == true)
                            {
                                yLenh += "<div><b>" +
                                         ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid)
                                             ? "(" + GetSoNgayDungThuocGayNghien(entity.YeuCauDuocPhamBenhViens, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId) + ") "
                                             : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                        (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>" + " " + item.DVT + "</b></div>";
                                yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
                                             !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
                                             !string.IsNullOrEmpty(item.GhiChu) ?
                                             "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
                                             item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

                                var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
                                if (thoiGianBatDauTruyen != null)
                                {
                                    if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
                                    {
                                        for (int j = 0; j < item.SoLanDungTrongNgay; j++)
                                        {
                                            var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                            thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
                                            item.GioSuDung += time + "; ";
                                        }
                                    }
                                    else
                                    {
                                        item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                    }
                                }
                                yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
                                     !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
                                     !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

                                     "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
                                     (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
                                     " " + (item.CachGioTruyenDich != null
                                         ? "cách " + item.CachGioTruyenDich + " giờ,"
                                         : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
                                         ? "giờ sử dụng: " + item.GioSuDung
                                         : "") + "</div>"

                                     : "";
                            }
                            else
                            {
                                yLenh += "<div><b>" + ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid) ? "(" + GetSoNgayDungThuocGayNghien(entity.YeuCauDuocPhamBenhViens, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId) + ") " : "") +
                                          (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                         (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                                yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : ""; ;
                                yLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
                                             ? (" " + item.ThoiGianDungSangDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
                                                                                              !string.IsNullOrEmpty(item.DungChieu) ||
                                                                                              !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
                                             ? (" " + item.ThoiGianDungTruaDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
                                                                                              !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
                                             ? (" " + item.ThoiGianDungChieuDisplay)
                                             : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                         (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
                                         (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
                                             ? (" " + item.ThoiGianDungToiDisplay)
                                             : "") + "</div>";
                            }
                        }
                    }
                    //CHẾ ĐỘ ĂN
                    yLenh += "<div><b>Chế độ ăn: </b> " + dienBien.CheDoAn + "</div>";
                    yLenh += "<br>";
                    //CHẾ ĐỘ CHĂM SÓC
                    yLenh += "<div><b>Chế độ chăm sóc: </b>" + dienBien.CheDoChamSoc + " </div>";
                    //BÁC SĨ
                    yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                    yLenh += "<p style='text-align: center;height:30px'></p>";

                    if (dienBien.ThoiGian != null)
                    {
                        yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienKhacNull + "</p>";
                    }
                    else
                    {
                        yLenh += "<p style='text-align: center;'> " + bacSiThoiGianDienBienNull + "</p>";
                    }
                    if (i != 0 && i != dienBiens.OrderBy(o => o.ThoiGian).ToList().Count())
                    {
                        noiDungToDieuTri += "<tr style='border-top:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                    }
                    else if (i == (dienBiens.OrderBy(o => o.ThoiGian).ToList().Count() - 1))
                    {
                        noiDungToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                    }
                    else
                    {
                        noiDungToDieuTri += "<tr style='border-bottom:hidden'> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                    }

                    noiDungToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                    noiDungToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
                    noiDungToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
                    i++;
                });
            }
            else
            {

                var ngayGio = string.Empty;
                var dienBienBenh = string.Empty;
                var yLenh = string.Empty;

                if (phieuDieuTri.ThoiDiemThamKham != null)
                {
                    ngayGio += phieuDieuTri.ThoiDiemThamKham.Value.Hour + " giờ " + phieuDieuTri.ThoiDiemThamKham.Value.Minute + " phút, " + phieuDieuTri.ThoiDiemThamKham.Value.ApplyFormatDate();

                }
                foreach (var item in lstDVKT)
                {
                    dienBienBenh += "<div style='padding:1px;'> - " + item.TenDichVu + (item.SoLuong > 1 ? "<b> x " + item.SoLuong + " lần</b>" : "") + " </div>";
                }
                if (lstThuoc.Any())
                {
                    foreach (var item in lstThuoc)
                    {
                        //DỊCH TRUYỀN
                        if (item.LaDichTruyen == true)
                        {
                            yLenh += "<div><b>" +
                                     ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid)
                                         ? "(" + GetSoNgayDungThuocGayNghien(entity.YeuCauDuocPhamBenhViens, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId) + ") "
                                         : "") + (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                     (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";
                            yLenh += !string.IsNullOrEmpty(item.TocDoTruyen?.ToString()) ||
                                             !string.IsNullOrEmpty(item.DonViTocDoTruyenDisplay) ||
                                             !string.IsNullOrEmpty(item.GhiChu) ?
                                             "<div style='margin-left:15px'>" + item.TocDoTruyen + " " +
                                             item.DonViTocDoTruyenDisplay + " " + item.GhiChu + "</div>" : "";

                            var thoiGianBatDauTruyen = item.ThoiGianBatDauTruyen;
                            if (thoiGianBatDauTruyen != null)
                            {
                                if (item.SoLanDungTrongNgay != null && item.CachGioTruyenDich != null)
                                {
                                    for (int j = 0; j < item.SoLanDungTrongNgay; j++)
                                    {
                                        var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                        thoiGianBatDauTruyen += item.SoLanDungTrongNgay * 3600;
                                        item.GioSuDung += time + "; ";
                                    }
                                }
                                else
                                {
                                    item.GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                }
                            }
                            yLenh += !string.IsNullOrEmpty(item.SoLanDungTrongNgay?.ToString()) ||
                                     !string.IsNullOrEmpty(item.CachGioTruyenDich?.ToString()) ||
                                     !string.IsNullOrEmpty(item.GioSuDung?.ToString()) ?

                                     "<div style='margin-left:15px;margin-bottom:0.1cm'>" +
                                     (item.SoLanDungTrongNgay != null ? item.SoLanDungTrongNgay + " lần/ngày," : "") +
                                     " " + (item.CachGioTruyenDich != null
                                         ? "cách " + item.CachGioTruyenDich + " giờ,"
                                         : "") + " " + (!string.IsNullOrEmpty(item.GioSuDung)
                                         ? "giờ sử dụng: " + item.GioSuDung
                                         : "") + "</div>"

                                     : "";
                        }
                        else
                        {
                            
                            yLenh += "<div><b>" + ((item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocPhongxa || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.KhangSinh
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDieuTriLao
                                                                                || item.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocCorticoid) ? "(" + GetSoNgayDungThuocGayNghien(entity.YeuCauDuocPhamBenhViens, phieuDieuTri.NgayDieuTri, item.DuocPhamBenhVienId) + ") " : "") +
                                (_yeuCauKhamBenhService.FormatTenDuocPham(item.Ten, item.HoatChat, item.HamLuong, item.DuoCPhamBenhVienPhanNhomId)) + " x " +
                                     (_yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)) + " " + item.DVT + "</b></div>";

                            yLenh += !string.IsNullOrEmpty(item.GhiChu) ? "<div style='margin-left:15px'>" + item.GhiChu + "</div>" : ""; ;


                            yLenh += "<div style='margin-left:15px;margin-bottom:0.1cm'>" + (!string.IsNullOrEmpty(item.DungSang) ? ("Sáng " + item.DungSang + " " + item.DVT) : "") +
                                     (!string.IsNullOrEmpty(item.ThoiGianDungSangDisplay)
                                         ? (" " + item.ThoiGianDungSangDisplay)
                                         : "") + (!string.IsNullOrEmpty(item.DungSang) && (!string.IsNullOrEmpty(item.DungTrua) ||
                                                                                          !string.IsNullOrEmpty(item.DungChieu) ||
                                                                                          !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                     (!string.IsNullOrEmpty(item.DungTrua) ? ("Trưa " + item.DungTrua + " " + item.DVT) : "") +
                                     (!string.IsNullOrEmpty(item.ThoiGianDungTruaDisplay)
                                         ? (" " + item.ThoiGianDungTruaDisplay)
                                         : "") + (!string.IsNullOrEmpty(item.DungTrua) && (!string.IsNullOrEmpty(item.DungChieu) ||
                                                                                          !string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                     (!string.IsNullOrEmpty(item.DungChieu) ? ("Chiều " + item.DungChieu + " " + item.DVT) : "") +
                                     (!string.IsNullOrEmpty(item.ThoiGianDungChieuDisplay)
                                         ? (" " + item.ThoiGianDungChieuDisplay)
                                         : "") + (!string.IsNullOrEmpty(item.DungChieu) && (!string.IsNullOrEmpty(item.DungToi)) ? ", " : "") +
                                     (!string.IsNullOrEmpty(item.DungToi) ? ("Tối " + item.DungToi + " " + item.DVT) : "") +
                                     (!string.IsNullOrEmpty(item.ThoiGianDungToiDisplay)
                                         ? (" " + item.ThoiGianDungToiDisplay)
                                         : "") + "</div>";
                        }
                    }
                }
                //CHẾ ĐỘ ĂN
                yLenh += "<div><b>Chế độ ăn: </b> " + phieuDieuTri?.CheDoAn?.Ten + "</div>";
                yLenh += "<br>";
                //CHẾ ĐỘ CHĂM SÓC
                yLenh += "<div><b>Chế độ chăm sóc:</b> " + phieuDieuTri?.CheDoChamSoc?.GetDescription() + "</div>";
                //BÁC SĨ

                //BÁC SĨ
                yLenh += "<p style='text-align: center;'> <b>BÁC SĨ</b></p>";
                yLenh += "<p style='text-align: center;height:30px'></p>";

                // ngày thăm khám
                if (string.IsNullOrEmpty(ngayGio))
                {
                    var bacSi = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
                   ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                       .Where(s => s.TuNgay.Date <= phieuDieuTri.NgayDieuTri && (s.DenNgay == null || phieuDieuTri.NgayDieuTri <= s.DenNgay.Value.Date))
                       .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";

                    yLenh += "<p style='text-align: center;'> " + bacSi + "</p>";
                }
                else
                {

                    var bacSiTheoNgayGioThamKham = entity.NoiTruBenhAn.NoiTruEkipDieuTris.Any(s => s.TuNgay.Date <= phieuDieuTri.ThoiDiemThamKham && (s.DenNgay == null || phieuDieuTri.ThoiDiemThamKham <= s.DenNgay.Value.Date))
                   ? entity.NoiTruBenhAn.NoiTruEkipDieuTris
                       .Where(s => s.TuNgay.Date <= phieuDieuTri.ThoiDiemThamKham && (s.DenNgay == null || phieuDieuTri.ThoiDiemThamKham <= s.DenNgay.Value.Date))
                       .Select(s => (s.BacSi.HocHamHocViId != null ? s.BacSi.HocHamHocVi.Ma + ". " : "") + s.BacSi.User.HoTen).Distinct().Join(", ") : "";

                    yLenh += "<p style='text-align: center;'> " + bacSiTheoNgayGioThamKham + "</p>";
                }

                noiDungToDieuTri += "<tr> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                noiDungToDieuTri += "<div>" + ngayGio + "</div> </td> <td class=\"contain-grid\" style=\"vertical-align: top;\">";
                noiDungToDieuTri += "<div>" + dienBienBenh + "</div> </td> <td class=\"contain-grid\"  style=\"vertical-align: top;\">";
                noiDungToDieuTri += "<div>" + yLenh + "</div> </td> </tr>";
            }
            var data = new
            {
                So = soThuTu + "",
                SoVaoVien = entity.NoiTruBenhAn?.SoBenhAn ?? "",
                HoTen = entity.HoTen,
                Tuoi = DateTime.Now.Year - entity.NamSinh + "",
                GioiTinh = entity.GioiTinh == LoaiGioiTinh.GioiTinhNam ? "Nam" : "Nữ",

                Khoa = entity.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                    ? entity.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten.Replace("Khoa", string.Empty) : "",

                Buong = entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan)
                    ? entity.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan).GiuongBenh?.PhongBenhVien?.Ten
                        : "",
                Giuong = entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan)
                    ? entity.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan).GiuongBenh.Ten
                        : "",
                ChanDoan = "<b>" + phieuDieuTri.ChanDoanChinhGhiChu + (cdkt.Length > 0 ? "; " + string.Join("; ", cdkt) : "") + "</b>",
                ToDieuTri = noiDungToDieuTri
            };

            content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
            return content;
        }
        */

        public int GetSoNgayDungThuocGayNghien(List<NgayDungDuocPhamBenhVienVo> ngayDungDuocPhamBenhVienVos, DateTime ngayDieuTri, long duocPhamBenhVienId, string tenDuocPham, string tenHoatChat, string hamLuong)
        {
            var ngaySuDungs = ngayDungDuocPhamBenhVienVos
                .Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId
                            || ((o.TenDuocPham == tenDuocPham || (o.TenDuocPham != null && tenDuocPham != null && o.TenDuocPham.ToLower() == tenDuocPham.ToLower()))
                                && (o.TenHoatChat == tenHoatChat || (o.TenHoatChat != null && tenHoatChat != null && o.TenHoatChat.ToLower() == tenHoatChat.ToLower()))
                                && (o.HamLuong == hamLuong || (o.HamLuong != null && hamLuong != null && o.HamLuong.ToLower() == hamLuong.ToLower()))))
                .Select(o => o.NgayDung.Date)
                .OrderBy(o => o)
                .Distinct().ToList();

            return ngaySuDungs.IndexOf(ngayDieuTri.Date) + 1;
        }

    }
    public class NgayDungDuocPhamBenhVienVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public string TenDuocPham { get; set; }
        public string TenHoatChat { get; set; }
        public string HamLuong { get; set; }
        public DateTime NgayDung { get; set; }
    }

    public class PhieuDieuTriChiDinhThuocVo
    {
        public long Id { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public int? SoThuTu { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string MaHoatChat { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public string DungSang { get; set; }
        public string DungTrua { get; set; }
        public string DungChieu { get; set; }
        public string DungToi { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public string ThoiGianDungSangDisplay => ThoiGianDungSang != null ? "(" + ThoiGianDungSang?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungTruaDisplay => ThoiGianDungTrua != null ? "(" + ThoiGianDungTrua?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungChieuDisplay => ThoiGianDungChieu != null ? "(" + ThoiGianDungChieu?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungToiDisplay => ThoiGianDungToi != null ? "(" + ThoiGianDungToi?.ConvertIntSecondsToTime12h() + ")" : "";
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public string TenDuongDung { get; set; }
        public double SoLuong { get; set; }                
        public string GhiChu { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public bool? LaDichTruyen { get; set; }
        public bool? LaThuocHuongThanGayNghien { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public string DonViTocDoTruyenDisplay => DonViTocDoTruyen?.GetDescription();
        public double? CachGioTruyenDich { get; set; }
        public string GioSuDung { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public DateTime? ThoiGianDienBien { get; set; }
        public DateTime ThoiGianTheoDienBien => ThoiGianDienBien ?? ThoiDiemChiDinh;
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh.ApplyFormatDateTime();
        public long DuongDungId { get; set; }
        //BVHD-3959
        //public int DuongDungNumber => DuongDungId == 12 ? 1 : (DuongDungId == 1 ? 2 : (DuongDungId == 26 ? 3 : (DuongDungId == 22 ? 4 : 5)));
        public int DuongDungNumber => BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public long? DuoCPhamBenhVienPhanNhomId { get; set; }
        public EnumYeuCauDuocPhamBenhVien TrangThai { get; set; }
    }
}
