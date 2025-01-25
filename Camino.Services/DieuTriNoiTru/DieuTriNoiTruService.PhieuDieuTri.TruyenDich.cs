using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Internal;
namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public GridDataSource GetDataForGridDanhSachTruyenDich(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var lstMaHoatChat = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                             .Select(p => p.MaHoatChat).ToList();

            var lstADR = _aDRRepository.TableNoTracking
                           .Where(o => o.ThuocHoacHoatChat1Id == o.ThuocHoacHoatChat1.Id
                                        && o.ThuocHoacHoatChat2Id == o.ThuocHoacHoatChat2.Id)
                           .Select(s => new MaHoatChatGridVo
                           {
                               Ten1 = s.ThuocHoacHoatChat1.Ten,
                               Ten2 = s.ThuocHoacHoatChat2.Ten,
                               MaHoatChat1 = s.ThuocHoacHoatChat1.Ma,
                               MaHoatChat2 = s.ThuocHoacHoatChat2.Ma
                           }).ToList();

            var query = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId
                          && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen == true
                          && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                          && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                          && o.YeuCauDuocPhamBenhViens.All(x => x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                          && o.NoiTruChiDinhPhaThuocTiemId == null
                          && o.NoiTruChiDinhPhaThuocTruyenId == null
                          )
                        .Select(s => new PhieuDieuTriTruyenDichGridVo
                        {
                            Id = s.Id,
                            DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                            MaHoatChat = s.MaHoatChat,
                            Ma = s.DuocPhamBenhVien.Ma,
                            Ten = s.Ten,
                            KhoId = s.YeuCauDuocPhamBenhViens.First().KhoLinhId,
                            TenKho = s.YeuCauDuocPhamBenhViens.First().KhoLinh.Ten,
                            HoatChat = s.HoatChat,
                            HamLuong = s.HamLuong,
                            DVT = s.DonViTinh.Ten,
                            TocDoTruyen = s.TocDoTruyen,
                            DonViTocDoTruyen = s.DonViTocDoTruyen,
                            SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                            ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                            CachGioTruyenDich = s.CachGioTruyenDich,
                            SoLuongDisplay = s.YeuCauDuocPhamBenhViens.Select(p => p.SoLuong).ToList(),
                            SoLuong = s.SoLuong,
                            DonGias = s.YeuCauDuocPhamBenhViens.Select(p => p.DonGiaBan).ToList(),
                            ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true && p.NoiTruChiDinhDuocPhamId == s.Id) ? s.YeuCauDuocPhamBenhViens.Where(x => x.NoiTruChiDinhDuocPhamId == s.Id).Select(p => p.GiaBan).ToList() : new List<decimal> { 0 },
                            LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                            DiUngThuoc = s.NoiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                            TuongTacThuoc = GetTuongTac(s.MaHoatChat, lstMaHoatChat, lstADR),
                            GhiChu = s.GhiChu,
                            TrangThai = s.TrangThai,
                            TinhTrang = s.YeuCauDuocPhamBenhViens.First().XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                            LaDichTruyen = s.LaDichTruyen,
                            TheTich = s.TheTich,
                            CoYeuCauTraDuocPhamTuBenhNhanChiTiet = s.YeuCauDuocPhamBenhViens.Any(yc => yc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any()),
                            LaTuTruc = s.YeuCauDuocPhamBenhViens.Any(yc => yc.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu),
                            KhongTinhPhi = s.YeuCauDuocPhamBenhViens.Select(p => !p.KhongTinhPhi).FirstOrDefault(),
                            LaThuocHuongThanGayNghien = s.DuocPhamBenhVien.DuocPham.LaThuocHuongThanGayNghien,
                            ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                            TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                            KhuVuc = 1,
                            CoThucHienYLenh = s.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null),
                            DuongDungId = s.DuongDungId
                        });
            double seconds = 3600;
            var lstQuery = query.ToList();
            lstQuery = lstQuery.AsQueryable().OrderBy(queryInfo.SortString).ThenBy(z => z.DuongDungNumber).ToList();
            for (int i = 0; i < lstQuery.Count(); i++)
            {
                var thoiGianBatDauTruyen = lstQuery[i].ThoiGianBatDauTruyen;
                if (thoiGianBatDauTruyen != null)
                {
                    if (lstQuery[i].SoLanDungTrongNgay != null && lstQuery[i].CachGioTruyenDich != null)
                    {
                        for (int j = 0; j < lstQuery[i].SoLanDungTrongNgay; j++)
                        {
                            var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                            thoiGianBatDauTruyen += (int?)(lstQuery[i].CachGioTruyenDich * seconds);
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
        public GridDataSource GetTotalPageForGridDanhSachTruyenDich(QueryInfo queryInfo)
        {
            return null;
        }
        public GridDataSource GetDataForGridDanhSachTruyenDichKhoTong(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            //get theo yctn
            var lstMaHoatChat = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                                .Select(p => p.MaHoatChat).ToList();

            var lstADR = _aDRRepository.TableNoTracking
                           .Where(o => o.ThuocHoacHoatChat1Id == o.ThuocHoacHoatChat1.Id
                                        && o.ThuocHoacHoatChat2Id == o.ThuocHoacHoatChat2.Id)
                           .Select(s => new MaHoatChatGridVo
                           {
                               Ten1 = s.ThuocHoacHoatChat1.Ten,
                               Ten2 = s.ThuocHoacHoatChat2.Ten,
                               MaHoatChat1 = s.ThuocHoacHoatChat1.Ma,
                               MaHoatChat2 = s.ThuocHoacHoatChat2.Ma
                           }).ToList();

            var query = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen == true
                          && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                          && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                          //&& o.YeuCauDuocPhamBenhViens.All(x => x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
                          //&& o.NoiTruChiDinhPhaThuocTiemId == null
                          //&& o.NoiTruChiDinhPhaThuocTruyenId == null
                          )
                .Select(s => new PhieuDieuTriTruyenDichGridVo
                {
                    YeuCauDuocPhamBenhIds = s.YeuCauDuocPhamBenhViens.Select(o => o.Id).ToList(),
                    SoThuTu = s.SoThuTu,
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    NoiTruChiDinhPhaThuocTiemId = s.NoiTruChiDinhPhaThuocTiemId,
                    NoiTruChiDinhPhaThuocTruyenId = s.NoiTruChiDinhPhaThuocTruyenId,
                    MaHoatChat = s.MaHoatChat,
                    Ma = s.DuocPhamBenhVien.Ma,
                    Ten = s.Ten,
                    //KhoId = s.YeuCauDuocPhamBenhViens.First(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).KhoLinhId,
                    //TenKho = s.YeuCauDuocPhamBenhViens.First(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).KhoLinh.Ten,
                    TenDuongDung = s.DuongDung.Ten,
                    HoatChat = s.HoatChat,
                    HamLuong = s.HamLuong,
                    DVT = s.DonViTinh.Ten,
                    TocDoTruyen = s.TocDoTruyen,
                    DonViTocDoTruyen = s.DonViTocDoTruyen,
                    ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                    CachGioTruyenDich = s.CachGioTruyenDich,
                    SoLuong = s.SoLuong,
                    //SoLuongDisplay = s.YeuCauDuocPhamBenhViens.Where(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Select(p => p.SoLuong).ToList(),
                    //DonGias = s.YeuCauDuocPhamBenhViens.Where(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Select(p => p.DonGiaBan).ToList(),
                    //ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true && p.NoiTruChiDinhDuocPhamId == s.Id && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy) ? s.YeuCauDuocPhamBenhViens.Where(x => x.NoiTruChiDinhDuocPhamId == s.Id && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Select(p => p.GiaBan).ToList() : new List<decimal> { 0 },
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    //DiUngThuoc = s.NoiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                    //TuongTacThuoc = GetTuongTac(s.MaHoatChat, lstMaHoatChat, lstADR),
                    GhiChu = s.GhiChu,
                    TrangThai = s.TrangThai,
                    //TinhTrang = s.YeuCauDuocPhamBenhViens.First(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    //PhieuLinh = s.YeuCauDuocPhamBenhViens.First(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).YeuCauLinhDuocPham.SoPhieu,
                    //PhieuXuat = s.YeuCauDuocPhamBenhViens.Any(a => a.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null && a.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy) ?
                    //    s.YeuCauDuocPhamBenhViens.Where(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Select(a => a.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu).FirstOrDefault() : "",
                    LaDichTruyen = s.LaDichTruyen,
                    TheTich = s.TheTich,
                    //CoYeuCauTraDuocPhamTuBenhNhanChiTiet = s.YeuCauDuocPhamBenhViens.Any(yc => yc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any() && yc.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy),
                    //KhongTinhPhi = s.YeuCauDuocPhamBenhViens.Where(z => z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Select(p => !p.KhongTinhPhi).FirstOrDefault(),
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    KhuVuc = 1,
                    DuongDungId = s.DuongDungId,
                    #region Cập nhật 26/12/2022
                    //CoThucHienYLenh = s.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null),
                    #endregion
                    //LaTuTruc = s.YeuCauDuocPhamBenhViens.Any(yc => yc.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu && yc.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy),
                    SoLanTrenNgay = (s.NoiTruChiDinhPhaThuocTiem == null && s.NoiTruChiDinhPhaThuocTruyen == null) ? s.SoLanDungTrongNgay : (s.NoiTruChiDinhPhaThuocTiem != null ? s.NoiTruChiDinhPhaThuocTiem.SoLanTrenNgay : (s.NoiTruChiDinhPhaThuocTruyen != null ? s.NoiTruChiDinhPhaThuocTruyen.SoLanTrenNgay : null)),
                    CachGioTiem = s.NoiTruChiDinhPhaThuocTiem != null ? s.NoiTruChiDinhPhaThuocTiem.CachGioTiem : null,
                    ThoiGianBatDauTiem = s.NoiTruChiDinhPhaThuocTiem != null ? s.NoiTruChiDinhPhaThuocTiem.ThoiGianBatDauTiem : null,
                    LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                    SoLanTrenMui = s.NoiTruChiDinhPhaThuocTiem != null ? s.NoiTruChiDinhPhaThuocTiem.SoLanTrenMui : null,

                    //BVHD-3905
                    TiLeThanhToanBHYT = s.DuocPhamBenhVien.TiLeThanhToanBHYT,
                    ThoiGianDienBien = s.ThoiGianDienBien
                });
            double seconds = 3600;
            var lstQuery = query.ToList();

            #region Cập nhật 26/12/2022
            if (lstQuery.Any())
            {
                var lstNoiTruChiDinhDuocPhamId = lstQuery.Select(x => x.Id).ToList();
                var lstChiDinhDuocPhamIdCoThucHienYLenh = _noiTruPhieuDieuTriChiTietYLenhRepository.TableNoTracking
                    .Where(x => x.ThoiDiemXacNhanThucHien != null
                                && x.NoiTruChiDinhDuocPhamId != null
                                && lstNoiTruChiDinhDuocPhamId.Contains(x.NoiTruChiDinhDuocPhamId.Value))
                    .Select(x => x.NoiTruChiDinhDuocPhamId.Value)
                    .ToList();
                lstQuery.ForEach(x => x.CoThucHienYLenh = lstChiDinhDuocPhamIdCoThucHienYLenh.Contains(x.Id));
            }
            #endregion

            var benhNhanDiUngThuocs = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId).Select(o => o.BenhNhan.BenhNhanDiUngThuocs).FirstOrDefault();

            var yeuCauDuocPhamBenhIds = lstQuery.SelectMany(o => o.YeuCauDuocPhamBenhIds).ToList();
            var allYeuCauDuocPhamBenhVienDatas = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => yeuCauDuocPhamBenhIds.Contains(o.Id))
                .Select(o => new YeuCauDuocPhamBenhVienData
                {
                    NoiTruChiDinhDuocPhamId = o.NoiTruChiDinhDuocPhamId,
                    KhoId = o.KhoLinhId,
                    TenKho = o.KhoLinhId != null ? o.KhoLinh.Ten : "",
                    SoLuong = o.SoLuong,
                    DonGiaBan = o.DonGiaBan,
                    KhongTinhPhi = o.KhongTinhPhi,
                    GiaBan = o.GiaBan,
                    XuatKhoDuocPhamId = o.XuatKhoDuocPhamChiTietId != null ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId : null,
                    PhieuLinh = o.YeuCauLinhDuocPhamId != null ? o.YeuCauLinhDuocPham.SoPhieu : "",
                    PhieuXuat = o.XuatKhoDuocPhamChiTietId != null ? (o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "") : "",
                    #region Cập nhật 26/12/2022
                    //CoYeuCauTraDuocPhamTuBenhNhanChiTiet = o.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any(),
                    YeuCauDuocPhamBenhVienId = o.Id,
                    #endregion
                    LoaiPhieuLinh = o.LoaiPhieuLinh,
                    TrangThai = o.TrangThai
                }).ToList();

            #region Cập nhật 26/12/2022
            if (allYeuCauDuocPhamBenhVienDatas.Any())
            {
                var lstYeuCauDuocPhamBenhVienIdCoYeuCauTra = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                    .Where(x => yeuCauDuocPhamBenhIds.Contains(x.YeuCauDuocPhamBenhVienId))
                    .Select(x => x.YeuCauDuocPhamBenhVienId)
                    .Distinct()
                    .ToList();
                allYeuCauDuocPhamBenhVienDatas.ForEach(x => x.CoYeuCauTraDuocPhamTuBenhNhanChiTiet = lstYeuCauDuocPhamBenhVienIdCoYeuCauTra.Contains(x.YeuCauDuocPhamBenhVienId ?? 0));
            }
            #endregion

            for (int i = 0; i < lstQuery.Count(); i++)
            {
                var yeuCauDuocPhamBenhVienDatas = allYeuCauDuocPhamBenhVienDatas.Where(o => o.NoiTruChiDinhDuocPhamId == lstQuery[i].Id && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).ToList();
                if (yeuCauDuocPhamBenhVienDatas.Any())
                {
                    lstQuery[i].KhoId = yeuCauDuocPhamBenhVienDatas.First().KhoId;
                    lstQuery[i].TenKho = yeuCauDuocPhamBenhVienDatas.First().TenKho;
                    lstQuery[i].SoLuongDisplay = yeuCauDuocPhamBenhVienDatas.Select(p => p.SoLuong).ToList();
                    lstQuery[i].DonGias = yeuCauDuocPhamBenhVienDatas.Select(p => p.DonGiaBan).ToList();
                    lstQuery[i].ThanhTiens = yeuCauDuocPhamBenhVienDatas.Any(p => p.KhongTinhPhi != true) ? yeuCauDuocPhamBenhVienDatas.Select(p => p.GiaBan).ToList() : new List<decimal> { 0 };
                    lstQuery[i].TinhTrang = yeuCauDuocPhamBenhVienDatas.First().XuatKhoDuocPhamId != null;
                    lstQuery[i].PhieuLinh = yeuCauDuocPhamBenhVienDatas.First().PhieuLinh;
                    lstQuery[i].PhieuXuat = yeuCauDuocPhamBenhVienDatas.First().PhieuXuat;
                    lstQuery[i].CoYeuCauTraDuocPhamTuBenhNhanChiTiet = yeuCauDuocPhamBenhVienDatas.Any(yc => yc.CoYeuCauTraDuocPhamTuBenhNhanChiTiet);
                    lstQuery[i].KhongTinhPhi = yeuCauDuocPhamBenhVienDatas.Select(p => !p.KhongTinhPhi).FirstOrDefault();
                    lstQuery[i].LaTuTruc = yeuCauDuocPhamBenhVienDatas.Any(yc => yc.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu);
                }
                lstQuery[i].DiUngThuoc = benhNhanDiUngThuocs.Any(diung => diung.TenDiUng == lstQuery[i].MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không";
                lstQuery[i].TuongTacThuoc = GetTuongTac(lstQuery[i].MaHoatChat, lstMaHoatChat, lstADR);

                var thoiGianBatDauTruyen = lstQuery[i].ThoiGianBatDauTruyen;
                if (thoiGianBatDauTruyen != null)
                {
                    if (lstQuery[i].SoLanTrenNgay != null && lstQuery[i].CachGioTruyenDich != null)
                    {
                        for (int j = 0; j < lstQuery[i].SoLanTrenNgay; j++)
                        {
                            var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                            thoiGianBatDauTruyen += (int?)(lstQuery[i].CachGioTruyenDich * seconds);
                            lstQuery[i].GioSuDung += time + "; ";
                        }
                    }
                    else
                    {
                        lstQuery[i].GioSuDung = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                    }
                }
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
            //var countTask = queryInfo.LazyLoadPage == true ? 0 : lstQuery.Count();
            //var queryTask = lstQuery.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = lstQuery.OrderBy(o => o.SoThuTu).ThenBy(z => z.DuongDungNumber).ToArray(), TotalRowCount = lstQuery.Count };
        }
        public GridDataSource GetTotalPageForGridDanhSachTruyenDichKhoTong(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);
            var query = _noiTruChiDinhDuocPhamRepository.TableNoTracking
               .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen == true
                         && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                         && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                         )
               .Select(s => new PhieuDieuTriTruyenDichGridVo
               { Id = s.Id });
            return new GridDataSource { TotalRowCount = query.Count() };
        }
        public List<LookupItemVo> GetDonViTocDoTruyen(DropDownListRequestModel queryInfo)
        {
            var enums = Enum.GetValues(typeof(DonViTocDoTruyen)).Cast<Enum>();
            var result = enums.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower()
                    .Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower()));
            return result.ToList();
        }
        public async Task ThemThuocTruyenDich(ThuocTruyenDichBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donThuocChiTiet.Id).First();

            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamBenhVienId,
                x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

            var loaiKho = _khoRepository.TableNoTracking.Where(p => p.Id == donThuocChiTiet.KhoId).Select(p => p.LoaiKho).First();
            var bacSiChiDinhId = _userAgentHelper.GetCurrentUserId();
            var noiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var laDuocPhamBHYT = false;
            if (donThuocChiTiet.LaDuocPhamBHYT == 2)
            {
                laDuocPhamBHYT = true;
            }
            var SLTon = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(p => p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId && (p.LaDuocPhamBHYT == laDuocPhamBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
                        .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            if (SLTon < donThuocChiTiet.SoLuong)
            {
                throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
            }

            double soLuongCanXuat = donThuocChiTiet.SoLuong;

            var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
             .Where(o => ((o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId) && (o.LaDuocPhamBHYT == laDuocPhamBHYT)
                         && o.SoLuongNhap > o.SoLuongDaXuat)).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
            var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

            var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
            {
                YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                Ten = duocPham.Ten,
                TenTiengAnh = duocPham.TenTiengAnh,
                SoDangKy = duocPham.SoDangKy,
                STTHoatChat = duocPham.STTHoatChat,
                MaHoatChat = duocPham.MaHoatChat,
                HoatChat = duocPham.HoatChat,
                LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat,
                TiLeBaoHiemThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan ?? 100,
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
                HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                SoLuong = soLuongXuat,
                NhanVienChiDinhId = bacSiChiDinhId,
                NoiChiDinhId = noiChiDinhId,
                ThoiDiemChiDinh = DateTime.Now,
                //DaCapThuoc = false,
                TrangThai = EnumYeuCauDuocPhamBenhVien.ChuaThucHien,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                LaDuocPhamBHYT = laDuocPhamBHYT,
                DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                VAT = nhapKhoDuocPhamChiTiet.VAT,
                SoTienBenhNhanDaChi = 0,
                KhoLinhId = donThuocChiTiet.KhoId,
                LaDichTruyen = true,
                //TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                //DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                //ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                //CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich,
                //SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay,
                DuocHuongBaoHiem = laDuocPhamBHYT,
                LoaiPhieuLinh = loaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                GhiChu = donThuocChiTiet.GhiChu,
                TheTich = duocPham.TheTich,
                KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi
            };
            soLuongCanXuat = soLuongCanXuat - soLuongXuat;
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
                    KhoXuatId = donThuocChiTiet.KhoId
                };
                var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                {
                    DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                    XuatKhoDuocPham = xuatKhoDuocPham,
                    NgayXuat = DateTime.Now
                };
                var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();

                var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                    .Where(o => ((o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId) && (o.LaDuocPhamBHYT == laDuocPhamBHYT)
                                 && o.SoLuongNhap > o.SoLuongDaXuat)).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var item in nhapKhoDuocPhamChiTiets)
                {
                    if (donThuocChiTiet.SoLuong > 0)
                    {
                        var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == item.HopDongThauDuocPhamId).Gia;
                        var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;

                        if (yeuCauDuocPhamBenhVienNew.DonGiaNhap != 0 && (yeuCauDuocPhamBenhVienNew.DonGiaNhap != item.DonGiaNhap || yeuCauDuocPhamBenhVienNew.VAT != item.VAT || yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan != item.TiLeBHYTThanhToan))
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
                            yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;

                            xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                            {
                                DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                                XuatKhoDuocPham = xuatKhoDuocPham,
                                NgayXuat = DateTime.Now
                            };
                        }
                        else
                        {
                            yeuCauDuocPhamBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                            yeuCauDuocPhamBenhVienNew.VAT = item.VAT;
                            yeuCauDuocPhamBenhVienNew.DaCapThuoc = true;
                            yeuCauDuocPhamBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                            yeuCauDuocPhamBenhVienNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                            yeuCauDuocPhamBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauDuocPhamBenhVienNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                        }
                        if (item.SoLuongNhap > item.SoLuongDaXuat)
                        {
                            var xuatViTri = new XuatKhoDuocPhamChiTietViTri()
                            {
                                NhapKhoDuocPhamChiTietId = item.Id,
                                NgayXuat = DateTime.Now,
                                GhiChu = xuatChiTiet.XuatKhoDuocPham.LyDoXuatKho
                            };

                            var tonTheoItem = item.SoLuongNhap - item.SoLuongDaXuat;
                            if (donThuocChiTiet.SoLuong <= tonTheoItem)
                            {
                                xuatViTri.SoLuongXuat = donThuocChiTiet.SoLuong;
                                item.SoLuongDaXuat += donThuocChiTiet.SoLuong;
                                donThuocChiTiet.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                donThuocChiTiet.SoLuong -= tonTheoItem;
                            }

                            xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                        }

                        if (donThuocChiTiet.SoLuong == 0)
                        {
                            yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                            yeuCauDuocPhamBenhVienNew.SoLuong = yeuCauDuocPhamBenhVienNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat);
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
                    yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Add(item);
                }
                noiTruPhieuDieuTri.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVienNew);
            }
            else
            {
                if (donThuocChiTiet.SoLuong > 0)
                {
                    var yeuCauNew = ycDuocPhamBenhVien.Clone();

                    var thongTinNhapDuocPham = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => ((o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId) && (o.LaDuocPhamBHYT == laDuocPhamBHYT)
                                                                                                              && o.SoLuongNhap > o.SoLuongDaXuat)).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                    var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == thongTinNhapDuocPham.HopDongThauDuocPhamId).Gia;
                    var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                    yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                    yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                    yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                    yeuCauNew.PhuongPhapTinhGiaTriTonKho = thongTinNhapDuocPham.PhuongPhapTinhGiaTriTonKho;
                    yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                    yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                    yeuCauNew.SoLuong = donThuocChiTiet.SoLuong;
                    donThuocChiTiet.SoLuong = 0;
                    noiTruPhieuDieuTri.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                }
            }
        }
        public async Task CapNhatThuocTruyenDich(ThuocTruyenDichBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruChiDinhDuocPham = yeuCauTiepNhan.NoiTruChiDinhDuocPhams.FirstOrDefault(p => p.Id == donThuocChiTiet.Id);

            if (noiTruChiDinhDuocPham == null)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.NotExists"));
            }
            if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.DaThanhToan"));
            }
            if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().YeuCauLinhDuocPhamId != null)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.DaLinh"));
            }

            noiTruChiDinhDuocPham.SoLuong = donThuocChiTiet.SoLuong;
            if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.All(p => p.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan && p.LaDichTruyen == true))
            {
                foreach (var item in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
                {
                    item.SoLuong = donThuocChiTiet.SoLuong;
                    //item.KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi;
                }
            }
            noiTruChiDinhDuocPham.TocDoTruyen = donThuocChiTiet.TocDoTruyen;
            noiTruChiDinhDuocPham.DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen;
            noiTruChiDinhDuocPham.ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen;
            noiTruChiDinhDuocPham.CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich;
            noiTruChiDinhDuocPham.SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay;
            noiTruChiDinhDuocPham.GhiChu = donThuocChiTiet.GhiChu;

            foreach (var item in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
            {
                var lstNhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                      .Where(x => x.NhapKhoDuocPhams.KhoId == item.KhoLinhId
                                  && x.DuocPhamBenhVienId == item.DuocPhamBenhVienId
                                  && x.HanSuDung >= DateTime.Now
                                  && x.NhapKhoDuocPhams.DaHet != true
                                  && x.LaDuocPhamBHYT == item.LaDuocPhamBHYT
                                  && x.SoLuongDaXuat < x.SoLuongNhap)
                      .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                      .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                      .ToListAsync();

                if (lstNhapKhoDuocPhamChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < donThuocChiTiet.SoLuong)
                {
                    throw new Exception(
                        _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                }
            }

        }

        public async Task<CoDonThuocKhoLeKhoTong> KiemTraCoDonTruyenDich(long noiTruPhieuDieuTriId)
        {
            var yeuDuocPhamBVKhoLe = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(p => p.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId
                && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                && p.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && p.LaDichTruyen == true).ToListAsync();

            var yeuDuocPhamBVKhoTong = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(p => p.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId
                && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                && p.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && p.LaDichTruyen == true).ToListAsync();

            var coDonThuocKhoLeKhoTong = new CoDonThuocKhoLeKhoTong();
            if (yeuDuocPhamBVKhoLe.Any())
            {
                coDonThuocKhoLeKhoTong.CoDonThuocKhoLe = true;
            }
            if (yeuDuocPhamBVKhoTong.Any())
            {
                coDonThuocKhoLeKhoTong.CoDonThuocKhoTong = true;
            }
            return coDonThuocKhoLeKhoTong;
        }
    }
}
