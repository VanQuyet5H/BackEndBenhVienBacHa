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
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Domain.Entities.Thuocs;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<List<KhoLookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaPhongId = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == noiLamViecCurrentId).Select(p => p.KhoaPhongId).First();
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                          //.Where(p => p.NhanVienId == userCurrentId && p.Kho.KhoaPhongId == khoaPhongId
                          //    && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2))
                          //Lấy tất cả kho lẻ và kho vacxin của nhân viên dc quyền vào cộng với tất cả kho tổng cấp 2
                          .Where(p => (p.NhanVienId == userCurrentId && (p.Kho.PhongBenhVienId == noiLamViecCurrentId || p.Kho.KhoaPhongId == khoaPhongId) && p.Kho.LoaiDuocPham == true && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin)) || (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && p.Kho.LoaiDuocPham == true))
                        .Select(s => new KhoLookupItemVo
                        {
                            KeyId = s.KhoId,
                            DisplayName = s.Kho.Ten,
                            LoaiKho = s.Kho.LoaiKho
                        })
                        .OrderByDescending(x => x.KeyId == noiLamViecCurrentId).ThenBy(x => x.DisplayName)
                        .ApplyLike(queryInfo.Query, o => o.DisplayName).Distinct()
                        .Take(queryInfo.Take);

            return await result.ToListAsync();
        }

        public GetDuocPhamTonKhoGridVoItem GetDuocPhamInfoById(ThongTinThuocDieuTriVo thongTinThuocVo)
        {

            if (thongTinThuocVo.KhuVuc == 1)
            {
                var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
                var yeuCauTiepNhan = BaseRepository.GetById(thongTinThuocVo.YeuCauTiepNhanId, x => x.Include(yc => yc.BenhNhan).ThenInclude(bn => bn.BenhNhanDiUngThuocs));
                var lstMaHoatChat = _noiTruChiDinhDuocPhamRepository.TableNoTracking.Where(p => p.NoiTruPhieuDieuTriId == thongTinThuocVo.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Select(p => p.MaHoatChat).ToList();
                var lstThuocHoacHoatChat = _thuocHoacHoatChatRepository.TableNoTracking.Select(p => p.Ten).ToList();
                var lstADR = _aDRRepository.TableNoTracking
                               .Select(s => new MaHoatChatGridVo
                               {
                                   Ten1 = s.ThuocHoacHoatChat1.Ten,
                                   Ten2 = s.ThuocHoacHoatChat2.Ten,
                                   MaHoatChat1 = s.ThuocHoacHoatChat1.Ma,
                                   MaHoatChat2 = s.ThuocHoacHoatChat2.Ma
                               }).ToList();
                var laDuocPhamBHYT = false;
                if (thongTinThuocVo.LoaiDuocPham == 2) // Thuốc BHYT
                {
                    laDuocPhamBHYT = true;
                }
                var duocPhamInfo = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.Id == thongTinThuocVo.DuocPhamBenhVienId)
                    .Select(d => new GetDuocPhamTonKhoGridVoItem
                    {
                        Id = d.Id,
                        TuongTacThuoc = GetTuongTac(d.DuocPham.MaHoatChat, lstMaHoatChat, lstADR) == string.Empty ? "Không" : GetTuongTac(d.DuocPham.MaHoatChat, lstMaHoatChat, lstADR),
                        FlagTuongTac = !GetTuongTac(d.DuocPham.MaHoatChat, lstMaHoatChat, lstADR).Contains("Không") ? true : false,
                        FlagDiUng = d.DuocPham.HoatChat != null && yeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng.Contains(d.DuocPham.HoatChat) && diung.LoaiDiUng == LoaiDiUng.Thuoc),
                        FlagThuocDaKe = _noiTruChiDinhDuocPhamRepository.TableNoTracking.Any(p => p.NoiTruPhieuDieuTriId == thongTinThuocVo.NoiTruPhieuDieuTriId
                                                                                               && p.DuocPhamBenhVienId == thongTinThuocVo.DuocPhamBenhVienId
                                                                                               && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                               && p.NoiTruChiDinhPhaThuocTiemId == null
                                                                                               && p.NoiTruChiDinhPhaThuocTruyenId == null),
                        FlagThuocDaKeTrungKho = _noiTruChiDinhDuocPhamRepository.TableNoTracking.Any(p => p.NoiTruPhieuDieuTriId == thongTinThuocVo.NoiTruPhieuDieuTriId
                                                                                                       && p.DuocPhamBenhVienId == thongTinThuocVo.DuocPhamBenhVienId
                                                                                                       && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                       && p.LaDuocPhamBHYT == laDuocPhamBHYT
                                                                                                       && p.NoiTruChiDinhPhaThuocTiemId == null
                                                                                                       && p.NoiTruChiDinhPhaThuocTruyenId == null
                                                                                                       && p.YeuCauDuocPhamBenhViens.Any(z => z.YeuCauLinhDuocPhamId == null
                                                                                                                                          && z.DuocPhamBenhVienId == thongTinThuocVo.DuocPhamBenhVienId
                                                                                                                                          && z.KhoLinhId == thongTinThuocVo.KhoId)),
                        FlagDichDaKe = _noiTruChiDinhDuocPhamRepository.TableNoTracking.Any(p => p.NoiTruPhieuDieuTriId == thongTinThuocVo.NoiTruPhieuDieuTriId
                                                                                              && p.DuocPhamBenhVienId == thongTinThuocVo.DuocPhamBenhVienId
                                                                                              && p.LaDichTruyen == true && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                              && p.NoiTruChiDinhPhaThuocTiemId == null
                                                                                              && p.NoiTruChiDinhPhaThuocTruyenId == null
                                                                                              ),
                        FlagDichDaKeTrungKho = _noiTruChiDinhDuocPhamRepository.TableNoTracking.Any(p => p.NoiTruPhieuDieuTriId == thongTinThuocVo.NoiTruPhieuDieuTriId
                                                                                                 && p.DuocPhamBenhVienId == thongTinThuocVo.DuocPhamBenhVienId
                                                                                                 && p.LaDichTruyen == true && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                 && p.NoiTruChiDinhPhaThuocTiemId == null
                                                                                                 && p.NoiTruChiDinhPhaThuocTruyenId == null
                                                                                                 && p.YeuCauDuocPhamBenhViens.Any(x => x.KhoLinhId == thongTinThuocVo.KhoId)),
                        TenDonViTinh = d.DuocPham.DonViTinh.Ten,
                        TenDuongDung = d.DuocPham.DuongDung.Ten,
                        TonKho = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == thongTinThuocVo.KhoId && nkct.DuocPhamBenhVienId == thongTinThuocVo.DuocPhamBenhVienId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2),
                        HanSuDung = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                            .Where(nkct => nkct.NhapKhoDuocPhams.KhoId == thongTinThuocVo.KhoId && nkct.DuocPhamBenhVienId == thongTinThuocVo.DuocPhamBenhVienId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now)
                                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                            .Select(o => o.HanSuDung).FirstOrDefault(),
                        MucDo = yeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Where(p => lstThuocHoacHoatChat.Contains(p.TenDiUng)).Select(p => p.MucDo).FirstOrDefault(),
                        TheTich = d.DuocPham.TheTich,
                    }).FirstOrDefault();
                return duocPhamInfo;
            }
            else
            {
                var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
                var yeuCauTiepNhan = BaseRepository.GetById(thongTinThuocVo.YeuCauTiepNhanId, x => x.Include(yc => yc.BenhNhan).ThenInclude(bn => bn.BenhNhanDiUngThuocs));
                var lstMaHoatChat = _noiTruPhieuDieuTriTuVanThuocRepository.TableNoTracking.Where(p => p.NoiTruPhieuDieuTriId == thongTinThuocVo.NoiTruPhieuDieuTriId).Select(p => p.MaHoatChat).ToList();
                var lstThuocHoacHoatChat = _thuocHoacHoatChatRepository.TableNoTracking.Select(p => p.Ten).ToList();
                var lstADR = _aDRRepository.TableNoTracking
                               .Select(s => new MaHoatChatGridVo
                               {
                                   Ten1 = s.ThuocHoacHoatChat1.Ten,
                                   Ten2 = s.ThuocHoacHoatChat2.Ten,
                                   MaHoatChat1 = s.ThuocHoacHoatChat1.Ma,
                                   MaHoatChat2 = s.ThuocHoacHoatChat2.Ma
                               }).ToList();
                var duocPhamInfo = _duocPhamRepository.TableNoTracking
                    .Where(o => o.Id == thongTinThuocVo.DuocPhamBenhVienId)
                    .Select(d => new GetDuocPhamTonKhoGridVoItem
                    {
                        Id = d.Id,
                        TuongTacThuoc = GetTuongTac(d.MaHoatChat, lstMaHoatChat, lstADR) == string.Empty ? "Không" : GetTuongTac(d.MaHoatChat, lstMaHoatChat, lstADR),
                        FlagTuongTac = !GetTuongTac(d.MaHoatChat, lstMaHoatChat, lstADR).Contains("Không") ? true : false,
                        FlagDiUng = d.HoatChat != null && yeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng.Contains(d.HoatChat) && diung.LoaiDiUng == LoaiDiUng.Thuoc),
                        FlagThuocDaKe = _noiTruPhieuDieuTriTuVanThuocRepository.TableNoTracking.Any(p => p.NoiTruPhieuDieuTriId == thongTinThuocVo.NoiTruPhieuDieuTriId && p.DuocPhamId == thongTinThuocVo.DuocPhamBenhVienId),
                        FlagDichDaKe = _noiTruPhieuDieuTriTuVanThuocRepository.TableNoTracking.Any(p => p.NoiTruPhieuDieuTriId == thongTinThuocVo.NoiTruPhieuDieuTriId && p.DuocPhamId == thongTinThuocVo.DuocPhamBenhVienId && p.LaDichTruyen == true),
                        TenDonViTinh = d.DonViTinh.Ten,
                        TenDuongDung = d.DuongDung.Ten,
                        MucDo = yeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Where(p => lstThuocHoacHoatChat.Contains(p.TenDiUng)).Select(p => p.MucDo).FirstOrDefault(),
                        TheTich = d.TheTich,
                    }).FirstOrDefault();
                return duocPhamInfo;
            }

        }

        private string GetTuongTac(string MaHoatChat, List<string> lstMaHoatChat, List<MaHoatChatGridVo> lstADR)
        {
            var TuongTac = string.Empty;
            if (lstADR.Count > 0)
            {
                foreach (var item in lstADR)
                {
                    if (item.MaHoatChat1 == MaHoatChat && lstMaHoatChat.Where(p => p != MaHoatChat).Contains(item.MaHoatChat2))
                    {
                        TuongTac += item.Ten2 + "; ";
                    }
                    if ((item.MaHoatChat2 == MaHoatChat && lstMaHoatChat.Where(p => p != MaHoatChat).Contains(item.MaHoatChat1)))
                    {
                        TuongTac += item.Ten1 + "; ";
                    }
                    if (TuongTac == string.Empty)
                    {
                        TuongTac = "Không";
                    }
                }
            }
            else
            {
                if (TuongTac == string.Empty)
                {
                    TuongTac = "Không";
                }
            }

            return TuongTac;
        }

        public GridDataSource GetDataForGridDanhSachThuocKhoLe(QueryInfo queryInfo)
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
                          && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen != true
                          && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                          && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                          && o.YeuCauDuocPhamBenhViens.All(x => x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                          && o.NoiTruChiDinhPhaThuocTiemId == null
                          && o.NoiTruChiDinhPhaThuocTruyenId == null
                          )
                .Select(s => new PhieuDieuTriThuocGridVo
                {
                    Id = s.Id,
                    SoThuTu = s.SoThuTu,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    MaHoatChat = s.MaHoatChat,
                    Ma = s.DuocPhamBenhVien.Ma,
                    Ten = s.Ten,
                    KhoId = s.YeuCauDuocPhamBenhViens.First().KhoLinhId,
                    TenKho = s.YeuCauDuocPhamBenhViens.First().KhoLinh.Ten,
                    HoatChat = s.HoatChat,
                    HamLuong = s.HamLuong,
                    DVT = s.DonViTinh.Ten,
                    ThoiGianDungSang = s.ThoiGianDungSang,
                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                    ThoiGianDungToi = s.ThoiGianDungToi,
                    DungSang = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                    DungTrua = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                    DungChieu = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                    DungToi = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                    SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                    TenDuongDung = s.DuongDung.Ten,
                    SoLuongDisplay = s.YeuCauDuocPhamBenhViens.Select(p => p.SoLuong).ToList(),
                    SoLuong = s.SoLuong,
                    DonGias = s.YeuCauDuocPhamBenhViens.Select(p => p.DonGiaBan).ToList(),
                    ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true && p.NoiTruChiDinhDuocPhamId == s.Id) ? s.YeuCauDuocPhamBenhViens.Where(x => x.NoiTruChiDinhDuocPhamId == s.Id).Select(p => p.GiaBan).ToList() : new List<decimal> { 0 },
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    DiUngThuoc = s.NoiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                    TuongTacThuoc = GetTuongTac(s.MaHoatChat, lstMaHoatChat, lstADR),
                    GhiChu = s.GhiChu,
                    TinhTrang = s.YeuCauDuocPhamBenhViens.First().XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    LaDichTruyen = s.LaDichTruyen,
                    CoYeuCauTraDuocPhamTuBenhNhanChiTiet = s.YeuCauDuocPhamBenhViens.Any(yc => yc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any()),
                    LaTuTruc = s.YeuCauDuocPhamBenhViens.Any(yc => yc.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu),
                    KhongTinhPhi = s.YeuCauDuocPhamBenhViens.Select(p => !p.KhongTinhPhi).FirstOrDefault(),
                    LaThuocHuongThanGayNghien = s.DuocPhamBenhVien.DuocPham.LaThuocHuongThanGayNghien,
                    TocDoTruyen = s.TocDoTruyen,
                    DonViTocDoTruyen = s.DonViTocDoTruyen,
                    CachGioTruyenDich = s.CachGioTruyenDich,
                    ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    KhuVuc = 1,
                    DuongDungId = s.DuongDungId,
                    SoLanTrenVien = s.SoLanTrenVien,
                    LieuDungTrenNgay = s.LieuDungTrenNgay,
                    CachGioDungThuoc = s.CachGioDungThuoc,
                    CoThucHienYLenh = s.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null),
                    LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                });
            var lstQuery = query.ToList();
            //.OrderBy(queryInfo.SortString).ThenBy(z => z.DuongDungNumber)
            var lstBHYTSTT = lstQuery.Where((p => p.LaDuocPhamBHYT)).Select(p => p).ToList();
            var lstKhongBHYTSTT = lstQuery.Where((p => !p.LaDuocPhamBHYT)).Select(p => p).ToList();
            for (int i = 0; i < lstBHYTSTT.Count(); i++)
            {
                if (!lstBHYTSTT[i].ThanhTiens.Any())
                {
                    lstBHYTSTT[i].ThanhTiens = new List<decimal> { 0 };
                }
            }
            for (int i = 0; i < lstKhongBHYTSTT.Count(); i++)
            {
                if (!lstKhongBHYTSTT[i].ThanhTiens.Any())
                {
                    lstKhongBHYTSTT[i].ThanhTiens = new List<decimal> { 0 };
                }
            }
            var countTask = queryInfo.LazyLoadPage == true ? 0 : lstQuery.Count();
            var queryTask = lstQuery.AsQueryable().OrderBy(queryInfo.SortString).ThenBy(z => z.DuongDungNumber).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public GridDataSource GetTotalPageForGridDanhSachThuocKhoLe(QueryInfo queryInfo)
        {
            return null;
        }

        public GridDataSource GetDataForGridDanhSachThuocKhoTong(QueryInfo queryInfo)
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
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId
                          && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen != true
                          && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                          && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                          //&& o.YeuCauDuocPhamBenhViens.All(x => x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
                          && o.NoiTruChiDinhPhaThuocTiemId == null
                          && o.NoiTruChiDinhPhaThuocTruyenId == null
                          )
                .Select(s => new PhieuDieuTriThuocGridVo
                {
                    YeuCauDuocPhamBenhIds = s.YeuCauDuocPhamBenhViens.Select(o=>o.Id).ToList(),
                    Id = s.Id,
                    SoThuTu = s.SoThuTu,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    MaHoatChat = s.MaHoatChat,
                    Ma = s.DuocPhamBenhVien.Ma,
                    Ten = s.Ten,
                    //KhoId = s.YeuCauDuocPhamBenhViens.First().KhoLinhId,
                    //TenKho = s.YeuCauDuocPhamBenhViens.First().KhoLinh.Ten,
                    HoatChat = s.HoatChat,
                    HamLuong = s.HamLuong,
                    DVT = s.DonViTinh.Ten,
                    ThoiGianDungSang = s.ThoiGianDungSang,
                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                    ThoiGianDungToi = s.ThoiGianDungToi,
                    DungSang = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                    DungTrua = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                    DungChieu = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                    DungToi = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                    TenDuongDung = s.DuongDung.Ten,
                    //SoLuongDisplay = s.YeuCauDuocPhamBenhViens.Select(p => p.SoLuong).ToList(),
                    SoLuong = s.SoLuong,
                    //DonGias = s.YeuCauDuocPhamBenhViens.Select(p => p.DonGiaBan).ToList(),
                    //ThanhTiens = s.YeuCauDuocPhamBenhViens.Any(p => p.KhongTinhPhi != true && p.NoiTruChiDinhDuocPhamId == s.Id) ? s.YeuCauDuocPhamBenhViens.Where(x => x.NoiTruChiDinhDuocPhamId == s.Id).Select(p => p.GiaBan).ToList() : new List<decimal> { 0 },
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    //DiUngThuoc = s.NoiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                    //TuongTacThuoc = GetTuongTac(s.MaHoatChat, lstMaHoatChat, lstADR),
                    GhiChu = s.GhiChu,
                    //TinhTrang = s.YeuCauDuocPhamBenhViens.First().XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    //PhieuLinh = s.YeuCauDuocPhamBenhViens.First().YeuCauLinhDuocPham.SoPhieu,
                    //PhieuXuat = s.YeuCauDuocPhamBenhViens.Any(a => a.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null) ?
                    //    s.YeuCauDuocPhamBenhViens.Select(a => a.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu).FirstOrDefault() : "",
                    LaDichTruyen = s.LaDichTruyen,
                    //CoYeuCauTraDuocPhamTuBenhNhanChiTiet = s.YeuCauDuocPhamBenhViens.Any(yc => yc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any()),
                    //KhongTinhPhi = s.YeuCauDuocPhamBenhViens.Select(p => !p.KhongTinhPhi).FirstOrDefault(),
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    KhuVuc = 1,
                    DuongDungId = s.DuongDungId,
                    SoLanTrenVien = s.SoLanTrenVien,
                    LieuDungTrenNgay = s.LieuDungTrenNgay,
                    CachGioDungThuoc = s.CachGioDungThuoc,
                    #region Cập nhật 26/12/2022
                    //CoThucHienYLenh = s.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null),
                    #endregion
                    TocDoTruyen = s.TocDoTruyen,
                    DonViTocDoTruyen = s.DonViTocDoTruyen,
                    SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                    ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                    TrangThai = s.TrangThai,
                    //LaTuTruc = s.YeuCauDuocPhamBenhViens.Any(yc => yc.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu),
                    SoLanTrenNgay = s.LieuDungTrenNgay,
                    LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,

                    //BVHD-3905
                    TiLeThanhToanBHYT = s.DuocPhamBenhVien.TiLeThanhToanBHYT,
                    ThoiGianDienBien = s.ThoiGianDienBien
                });
            var lstQuery = query.ToList();

            #region Cập nhật 26/12/2022
            if(lstQuery.Any())
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

            var benhNhanDiUngThuocs = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId).Select(o=>o.BenhNhan.BenhNhanDiUngThuocs).FirstOrDefault();

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
                }).ToList();

            #region Cập nhật 26/12/2022
            if(allYeuCauDuocPhamBenhVienDatas.Any())
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
                var yeuCauDuocPhamBenhVienDatas = allYeuCauDuocPhamBenhVienDatas.Where(o => o.NoiTruChiDinhDuocPhamId == lstQuery[i].Id).ToList();
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
                if (!lstQuery[i].ThanhTiens.Any())
                {
                    lstQuery[i].ThanhTiens = new List<decimal> { 0 };
                }
            }
            //var countTask = queryInfo.LazyLoadPage == true ? 0 : lstQuery.Count();
            //var queryTask = lstQuery.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = lstQuery.OrderBy(o=>o.SoThuTu).ThenBy(z => z.DuongDungNumber).ToArray(), TotalRowCount = lstQuery.Count };
        }

        public GridDataSource GetTotalPageForGridDanhSachThuocKhoTong(QueryInfo queryInfo)
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
                          && o.NoiTruChiDinhPhaThuocTiemId == null
                          && o.NoiTruChiDinhPhaThuocTruyenId == null
                          )
                .Select(s => new PhieuDieuTriThuocGridVo
                { Id = s.Id });
            return new GridDataSource { TotalRowCount = query.Count() };

        }

        public void ApDungThoiGianDienBienChiDinhDuocPham(List<long> chiDinhDuocPhamIds, DateTime? thoiGianDienBien)
        {
            var chiDinhDuocPhams = _noiTruChiDinhDuocPhamRepository.Table.Where(o => chiDinhDuocPhamIds.Contains(o.Id)).ToList();
            if (chiDinhDuocPhams.Any())
            {
                foreach(var chiDinhDuocPham in chiDinhDuocPhams)
                {
                    chiDinhDuocPham.ThoiGianDienBien = thoiGianDienBien;
                }
                _noiTruChiDinhDuocPhamRepository.Context.SaveChanges();
            }
        }

        public async Task ThemThuoc(ThuocBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId).First();
            var laDuocPhamBHYT = false;
            if (donThuocChiTiet.LaDuocPhamBHYT == 2)
            {
                laDuocPhamBHYT = true;
            }

            //var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamBenhVienId,
            //    x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
            //    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
            var thongTinDuocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamBenhVienId,
                x => x.Include(o => o.DuocPhamBenhVien).Include(o => o.HopDongThauDuocPhamChiTiets));

            var dataNhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                .Where(p => p.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId && p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId && (p.LaDuocPhamBHYT == laDuocPhamBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now);

            var loaiKho = await _khoRepository.TableNoTracking.Where(p => p.Id == donThuocChiTiet.KhoId).Select(p => p.LoaiKho).FirstAsync();
            var bacSiChiDinhId = _userAgentHelper.GetCurrentUserId();
            var noiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
            
            //var SLTon = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
            //            .Where(p => p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId && (p.LaDuocPhamBHYT == laDuocPhamBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
            //            .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            var SLTon = dataNhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber(2);
            if (SLTon < donThuocChiTiet.SoLuong)
            {
                throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
            }

            double soLuongCanXuat = donThuocChiTiet.SoLuong;

            //var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
            // .Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
            //          && o.LaDuocPhamBHYT == laDuocPhamBHYT
            //          && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
            //          && o.HanSuDung >= DateTime.Now
            //          && o.SoLuongNhap > o.SoLuongDaXuat)
            //          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();

            var nhapKhoDuocPhamChiTiet = dataNhapKhoDuocPhamChiTiets.OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();

            var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

            var noiTruChiDinhDuocPham = new NoiTruChiDinhDuocPham
            {
                YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                Ten = thongTinDuocPham.Ten,
                TenTiengAnh = thongTinDuocPham.TenTiengAnh,
                SoDangKy = thongTinDuocPham.SoDangKy,
                STTHoatChat = thongTinDuocPham.STTHoatChat,
                MaHoatChat = thongTinDuocPham.MaHoatChat,
                HoatChat = thongTinDuocPham.HoatChat,
                LoaiThuocHoacHoatChat = thongTinDuocPham.LoaiThuocHoacHoatChat,
                NhaSanXuat = thongTinDuocPham.NhaSanXuat,
                NuocSanXuat = thongTinDuocPham.NuocSanXuat,
                DuongDungId = thongTinDuocPham.DuongDungId,
                HamLuong = thongTinDuocPham.HamLuong,
                QuyCach = thongTinDuocPham.QuyCach,
                TieuChuan = thongTinDuocPham.TieuChuan,
                DangBaoChe = thongTinDuocPham.DangBaoChe,
                DonViTinhId = thongTinDuocPham.DonViTinhId,
                HuongDan = thongTinDuocPham.HuongDan,
                MoTa = thongTinDuocPham.MoTa,
                ChiDinh = thongTinDuocPham.ChiDinh,
                ChongChiDinh = thongTinDuocPham.ChongChiDinh,
                LieuLuongCachDung = thongTinDuocPham.LieuLuongCachDung,
                TacDungPhu = thongTinDuocPham.TacDungPhu,
                ChuYdePhong = thongTinDuocPham.ChuYDePhong,
                SoLuong = soLuongXuat,
                NhanVienChiDinhId = bacSiChiDinhId,
                NoiChiDinhId = noiChiDinhId,
                ThoiDiemChiDinh = DateTime.Now,
                TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                DuocHuongBaoHiem = laDuocPhamBHYT,
                LaDuocPhamBHYT = laDuocPhamBHYT,
                SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay,
                DungSang = donThuocChiTiet.DungSang,
                DungTrua = donThuocChiTiet.DungTrua,
                DungChieu = donThuocChiTiet.DungChieu,
                DungToi = donThuocChiTiet.DungToi,
                ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich,
                TheTich = thongTinDuocPham.TheTich,
                GhiChu = donThuocChiTiet.GhiChu,
                LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri,
                SoLanTrenVien = donThuocChiTiet.SoLanTrenVien,
                CachGioDungThuoc = donThuocChiTiet.CachGioDungThuoc,
                LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay,
                SoThuTu = donThuocChiTiet.SoThuTu
            };
            var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
            {
                YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                Ten = thongTinDuocPham.Ten,
                TenTiengAnh = thongTinDuocPham.TenTiengAnh,
                SoDangKy = thongTinDuocPham.SoDangKy,
                STTHoatChat = thongTinDuocPham.STTHoatChat,
                MaHoatChat = thongTinDuocPham.MaHoatChat,
                HoatChat = thongTinDuocPham.HoatChat,
                LoaiThuocHoacHoatChat = thongTinDuocPham.LoaiThuocHoacHoatChat,
                //TiLeBaoHiemThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan ?? 100,
                NhaSanXuat = thongTinDuocPham.NhaSanXuat,
                NuocSanXuat = thongTinDuocPham.NuocSanXuat,
                DuongDungId = thongTinDuocPham.DuongDungId,
                HamLuong = thongTinDuocPham.HamLuong,
                QuyCach = thongTinDuocPham.QuyCach,
                TieuChuan = thongTinDuocPham.TieuChuan,
                DangBaoChe = thongTinDuocPham.DangBaoChe,
                DonViTinhId = thongTinDuocPham.DonViTinhId,
                HuongDan = thongTinDuocPham.HuongDan,
                MoTa = thongTinDuocPham.MoTa,
                ChiDinh = thongTinDuocPham.ChiDinh,
                ChongChiDinh = thongTinDuocPham.ChongChiDinh,
                LieuLuongCachDung = thongTinDuocPham.LieuLuongCachDung,
                TacDungPhu = thongTinDuocPham.TacDungPhu,
                ChuYdePhong = thongTinDuocPham.ChuYDePhong,
                //HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                //NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                //SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                //SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                //LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                //LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                //NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                //GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                //NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                SoLuong = soLuongXuat,
                NhanVienChiDinhId = bacSiChiDinhId,
                NoiChiDinhId = noiChiDinhId,
                ThoiDiemChiDinh = DateTime.Now,
                //TrangThai = EnumYeuCauDuocPhamBenhVien.ChuaThucHien,
                TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                DuocHuongBaoHiem = laDuocPhamBHYT,
                LaDuocPhamBHYT = laDuocPhamBHYT,
                //DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                //TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                //VAT = nhapKhoDuocPhamChiTiet.VAT,
                SoTienBenhNhanDaChi = 0,
                KhoLinhId = donThuocChiTiet.KhoId,
                TheTich = thongTinDuocPham.TheTich,
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
                    KhoXuatId = donThuocChiTiet.KhoId
                };
                var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                {
                    DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                    XuatKhoDuocPham = xuatKhoDuocPham,
                    NgayXuat = DateTime.Now
                };

                var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();

                var nhapKhoDuocPhamChiTiets = dataNhapKhoDuocPhamChiTiets                    
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                    .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .ToList();
                //.Where(p => p.LaDuocPhamBHYT == laDuocPhamBHYT && p.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId && p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId).ToList();
                foreach (var item in nhapKhoDuocPhamChiTiets)
                {
                    if (donThuocChiTiet.SoLuong > 0)
                    {
                        var giaTheoHopDong = thongTinDuocPham.HopDongThauDuocPhamChiTiets.Where(o => o.HopDongThauDuocPhamId == item.HopDongThauDuocPhamId).Select(p => p.Gia).FirstOrDefault();

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
                                DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                            if (donThuocChiTiet.SoLuong <= tonTheoItem)
                            {
                                xuatViTri.SoLuongXuat = donThuocChiTiet.SoLuong;
                                item.SoLuongDaXuat = (item.SoLuongDaXuat + donThuocChiTiet.SoLuong).MathRoundNumber(2);
                                donThuocChiTiet.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                donThuocChiTiet.SoLuong = (donThuocChiTiet.SoLuong - tonTheoItem).MathRoundNumber(2);
                            }

                            xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                        }

                        if (donThuocChiTiet.SoLuong == 0)
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
                if (donThuocChiTiet.SoLuong > 0)
                {
                    var yeuCauNew = ycDuocPhamBenhVien.Clone();

                    var thongTinNhapDuocPham = dataNhapKhoDuocPhamChiTiets
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                    var giaTheoHopDong = thongTinDuocPham.HopDongThauDuocPhamChiTiets.Where(o => o.HopDongThauDuocPhamId == thongTinNhapDuocPham.HopDongThauDuocPhamId).Select(p => p.Gia).FirstOrDefault();
                    var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                    yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                    yeuCauNew.DaCapThuoc = false;
                    yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                    yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                    yeuCauNew.PhuongPhapTinhGiaTriTonKho = thongTinNhapDuocPham.PhuongPhapTinhGiaTriTonKho;
                    yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                    yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                    yeuCauNew.SoLuong = donThuocChiTiet.SoLuong;
                    donThuocChiTiet.SoLuong = 0;
                    noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                    noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                }
            }
        }

        public async Task CapNhatThuoc(ThuocBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruChiDinhDuocPham = yeuCauTiepNhan.NoiTruChiDinhDuocPhams.FirstOrDefault(p => p.Id == donThuocChiTiet.Id);

            if (noiTruChiDinhDuocPham == null)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.NotExists"));
            }
            //if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
            //{
            //    throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.DaThanhToan"));
            //}
            if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().YeuCauLinhDuocPhamId != null)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.DaLinh"));
            }
            noiTruChiDinhDuocPham.SoLuong = donThuocChiTiet.SoLuong;
            //if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.All(p => p.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan && p.LaDichTruyen != true))
            //{
            //    foreach (var item in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
            //    {
            //        item.SoLuong = donThuocChiTiet.SoLuong;
            //        //item.KhongTinhPhi = donThuocChiTiet.KhongTinhPhi;
            //    }
            //}
            noiTruChiDinhDuocPham.LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay;
            noiTruChiDinhDuocPham.SoLanTrenVien = donThuocChiTiet.SoLanTrenVien;
            noiTruChiDinhDuocPham.CachGioDungThuoc = donThuocChiTiet.CachGioDungThuoc;
            noiTruChiDinhDuocPham.SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay;
            noiTruChiDinhDuocPham.DungSang = donThuocChiTiet.DungSang;
            noiTruChiDinhDuocPham.DungTrua = donThuocChiTiet.DungTrua;
            noiTruChiDinhDuocPham.DungChieu = donThuocChiTiet.DungChieu;
            noiTruChiDinhDuocPham.DungToi = donThuocChiTiet.DungToi;
            noiTruChiDinhDuocPham.ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang;
            noiTruChiDinhDuocPham.ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua;
            noiTruChiDinhDuocPham.ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu;
            noiTruChiDinhDuocPham.ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi;
            noiTruChiDinhDuocPham.GhiChu = donThuocChiTiet.GhiChu;
            foreach (var item in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
            {
                item.SoLuong = donThuocChiTiet.SoLuong;
                var lstNhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoDuocPhams.KhoId == item.KhoLinhId
                                    && x.DuocPhamBenhVienId == item.DuocPhamBenhVienId
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.LaDuocPhamBHYT == item.LaDuocPhamBHYT
                                    && x.HanSuDung >= DateTime.Now
                                    && x.SoLuongDaXuat < x.SoLuongNhap)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                        .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToListAsync();
                item.KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi;
                if (lstNhapKhoDuocPhamChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < donThuocChiTiet.SoLuong)
                {
                    throw new Exception(
                        _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                }
            }
        }

        public async Task<string> CapNhatThuocChoTuTruc(ThuocBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruChiDinhDuocPham = yeuCauTiepNhan.NoiTruChiDinhDuocPhams.FirstOrDefault(p => p.Id == donThuocChiTiet.Id);

            if (noiTruChiDinhDuocPham == null)
            {
                return GetResourceValueByResourceName("PhieuDieuTri.DonThuoc.NotExists");
            }
            if (donThuocChiTiet.SoLuong > noiTruChiDinhDuocPham.SoLuong || donThuocChiTiet.SoLuong.AlmostEqual(noiTruChiDinhDuocPham.SoLuong))
            {
                var laDuocPhamBHYT = false;
                if (donThuocChiTiet.LaDuocPhamBHYT == 2)
                {
                    laDuocPhamBHYT = true;
                }
                foreach (var ycDuocPhamBenhVien in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
                {
                    var SLTon = ycDuocPhamBenhVien.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                           .Where(p => p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId && p.LaDuocPhamBHYT == laDuocPhamBHYT && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now && p.DuocPhamBenhVienId == ycDuocPhamBenhVien.DuocPhamBenhVienId)
                           .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) + noiTruChiDinhDuocPham.SoLuong;
                    ycDuocPhamBenhVien.KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi;
                    if (SLTon < donThuocChiTiet.SoLuong)
                    {
                        return GetResourceValueByResourceName("DonVTYT.VTYTSoLuongTon");
                    }
                }
            }
            return string.Empty;
        }

        public async Task XoaThuoc(long noiTruChiDinhDuocPhamId, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var noiTruChiDinhDuocPham = yeuCauTiepNhan.NoiTruChiDinhDuocPhams.FirstOrDefault(x => x.Id == noiTruChiDinhDuocPhamId);
            if (noiTruChiDinhDuocPham == null)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.NotExists"));
            }
            if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauLinhDuocPhamChiTiets.Any()))
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.DaTaoPhieuLinh"));
            }
            if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().YeuCauLinhDuocPhamId != null)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.DaLinh"));
            }

            if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
            {
                noiTruChiDinhDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
                foreach (var dpbv in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
                {
                    dpbv.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
                    var xuatKhoDpViTris = dpbv.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.ToList();
                    var xuatKhoDuocPhamViTriHoanTras = new List<XuatKhoDuocPhamChiTietViTri>();
                    if (noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.First().XuatKhoDuocPhamChiTiet != null)
                    {
                        foreach (var thongTinXuat in xuatKhoDpViTris)
                        {
                            thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= thongTinXuat.SoLuongXuat;
                        }
                    }
                    foreach (var item in xuatKhoDpViTris)
                    {
                        var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                        {
                            XuatKhoDuocPhamChiTietId = item.XuatKhoDuocPhamChiTietId,
                            NhapKhoDuocPhamChiTietId = item.NhapKhoDuocPhamChiTietId,
                            SoLuongXuat = -item.SoLuongXuat,
                            NgayXuat = DateTime.Now,
                            GhiChu = noiTruChiDinhDuocPham.TrangThai.GetDescription()
                        };
                        dpbv.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                    }
                    //foreach (var item in dpbv.YeuCauTraDuocPhamTuBenhNhanChiTiets)EnumYeuCauDuocPhamBenhVien.DaHuy 
                    //{
                    //    item.WillDelete = true;
                    //}
                }
            }
            else
            {
                foreach (var dpbv in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
                {
                    if (dpbv.XuatKhoDuocPhamChiTiet != null)
                    {
                        foreach (var thongTinXuat in dpbv.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
                        {
                            thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= thongTinXuat.SoLuongXuat;
                        }
                    }
                    dpbv.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
                }
            }
            noiTruChiDinhDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
            await XuLyXoaYLenhKhiXoaDichVuNoiTruAsync(EnumNhomGoiDichVu.DuocPham, noiTruChiDinhDuocPhamId);
        }

        public async Task SapXepThuoc(SapXepThuoc donThuocChiTiet)
        {
            var noiTruPhieuDieuTri = _noiTruPhieuDieuTriRepository.Table.Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien).Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId).First();
            var STTThuoc = 1;
            var STTDichTruyen = 1;
            var thuocs = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.LaDichTruyen != true);
            //BVHD-3959
            var thuocsSapXep = thuocs
                .Where(o => o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                .OrderBy(o => o.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc ? 1 : 2)
                .ThenBy(o => BenhVienHelper.GetSoThuThuocTheoDuongDung(o.DuongDungId))
                .ThenBy(o => o.CreatedOn)
                .ToList();
            foreach (var item in thuocsSapXep)
            {
                item.SoThuTu = STTThuoc;
                STTThuoc++;
            }

            //if (thuocs.Any())
            //{
            //    var thuocDocs = thuocs.Where(z => z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (thuocDocs.Any())
            //    {
            //        foreach (var item in thuocDocs)
            //        {
            //            item.SoThuTu = STTThuoc;
            //            STTThuoc++;
            //        }
            //    }
            //    var thuocTiems = thuocs.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (thuocTiems.Any())
            //    {
            //        foreach (var item in thuocTiems)
            //        {
            //            item.SoThuTu = STTThuoc;
            //            STTThuoc++;
            //        }
            //    }
            //    var thuocUongs = thuocs.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (thuocUongs.Any())
            //    {
            //        foreach (var item in thuocUongs)
            //        {
            //            item.SoThuTu = STTThuoc;
            //            STTThuoc++;
            //        }
            //    }
            //    var thuocDats = thuocs.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (thuocDats.Any())
            //    {
            //        foreach (var item in thuocDats)
            //        {
            //            item.SoThuTu = STTThuoc;
            //            STTThuoc++;
            //        }
            //    }
            //    var thuocDungNgoais = thuocs.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (thuocDungNgoais.Any())
            //    {
            //        foreach (var item in thuocDungNgoais)
            //        {
            //            item.SoThuTu = STTThuoc;
            //            STTThuoc++;
            //        }
            //    }
            //    var thuocKhacs = thuocs.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                      && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                      && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                      && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                      && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc
            //                                      && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (thuocKhacs.Any())
            //    {
            //        foreach (var item in thuocKhacs)
            //        {
            //            item.SoThuTu = STTThuoc;
            //            STTThuoc++;
            //        }
            //    }
            //}


            var dichTruyens = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.LaDichTruyen == true);
            //BVHD-3959
            var dichTruyensSapXep = dichTruyens
                .Where(o => o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                .OrderBy(o => o.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc ? 1 : 2)
                .ThenBy(o => BenhVienHelper.GetSoThuThuocTheoDuongDung(o.DuongDungId))
                .ThenBy(o => o.CreatedOn)
                .ToList();
            foreach (var item in thuocsSapXep)
            {
                item.SoThuTu = STTDichTruyen;
                STTDichTruyen++;
            }

            //if (dichTruyens.Any())
            //{
            //    var dichTruyenDocs = dichTruyens.Where(z => z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (dichTruyenDocs.Any())
            //    {
            //        foreach (var item in dichTruyenDocs)
            //        {
            //            item.SoThuTu = STTDichTruyen;
            //            STTDichTruyen++;
            //        }
            //    }
            //    var dichTruyenTiems = dichTruyens.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (dichTruyenTiems.Any())
            //    {
            //        foreach (var item in dichTruyenTiems)
            //        {
            //            item.SoThuTu = STTDichTruyen;
            //            STTDichTruyen++;
            //        }
            //    }
            //    var dichTruyenUongs = dichTruyens.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (dichTruyenUongs.Any())
            //    {
            //        foreach (var item in dichTruyenUongs)
            //        {
            //            item.SoThuTu = STTDichTruyen;
            //            STTDichTruyen++;
            //        }
            //    }
            //    var dichTruyenDats = dichTruyens.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (dichTruyenDats.Any())
            //    {
            //        foreach (var item in dichTruyenDats)
            //        {
            //            item.SoThuTu = STTDichTruyen;
            //            STTDichTruyen++;
            //        }
            //    }
            //    var dichTruyenDungNgoais = dichTruyens.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (dichTruyenDungNgoais.Any())
            //    {
            //        foreach (var item in dichTruyenDungNgoais)
            //        {
            //            item.SoThuTu = STTDichTruyen;
            //            STTDichTruyen++;
            //        }
            //    }
            //    var dichTruyenKhacs = dichTruyens.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                              && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                              && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                              && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                              && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc
            //                                              && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).OrderBy(z => z.CreatedOn);
            //    if (dichTruyenKhacs.Any())
            //    {
            //        foreach (var item in dichTruyenKhacs)
            //        {
            //            item.SoThuTu = STTDichTruyen;
            //            STTDichTruyen++;
            //        }
            //    }
            //}

            await _noiTruPhieuDieuTriRepository.UpdateAsync(noiTruPhieuDieuTri);
        }


        public async Task<string> TangHoacGiamSTTDonThuocChiTiet(ThuocBenhVienTangGiamSTTVo donThuocChiTiet)
        {
            var noiTruPhieuDieuTri = _noiTruPhieuDieuTriRepository.Table.Include(z => z.NoiTruChiDinhDuocPhams).Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId).First();
            var dtChiTiet = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.Id == donThuocChiTiet.Id).FirstOrDefault();
            if (donThuocChiTiet.LaTangSTT == true)
            {
                var dtChiTietDangTruoc = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams
                    .Where(z => z.SoThuTu == (dtChiTiet.SoThuTu - 1) && z.LaDichTruyen.GetValueOrDefault() == donThuocChiTiet.LaDichTruyen.GetValueOrDefault() && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .FirstOrDefault();
                if (dtChiTietDangTruoc != null)
                {
                    dtChiTiet.SoThuTu--;
                    dtChiTietDangTruoc.SoThuTu++;
                }
                else
                {
                    //return GetResourceValueByResourceName("DonThuoc.STT.KhongTheGiam");
                    throw new Exception(_localizationService.GetResource("DonThuoc.STT.KhongTheTang"));

                }
            }
            else
            {
                var dtChiTietKeTiep = noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Where(z => z.SoThuTu == (dtChiTiet.SoThuTu + 1) && z.LaDichTruyen.GetValueOrDefault() == donThuocChiTiet.LaDichTruyen.GetValueOrDefault() && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).FirstOrDefault();
                if (dtChiTietKeTiep != null)
                {
                    dtChiTiet.SoThuTu++;
                    dtChiTietKeTiep.SoThuTu--;
                }
                else
                {
                    //return GetResourceValueByResourceName("DonThuoc.STT.KhongTheTang");
                    throw new Exception(_localizationService.GetResource("DonThuoc.STT.KhongTheGiam"));

                }
            }
            await _noiTruPhieuDieuTriRepository.UpdateAsync(noiTruPhieuDieuTri);
            return string.Empty;
        }

        public string GetResourceValueByResourceName(string ten)
        {
            var result = _localeStringResourceRepository.Table.AsNoTracking()
                .Where(o => o.ResourceName.Contains(ten))
                .Select(o => o.ResourceValue)
                .FirstOrDefault();
            return result;
        }

        private ThongTinChungBenhNhanPhieuDieuTri ThongTinChungBenhNhanPhieuDieuTri(long yeuCauTiepNhanId)
        {
            var today = DateTime.Now;
            var thongTin = BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                .Select(s => new ThongTinChungBenhNhanPhieuDieuTri
                {
                    MaTN = s.MaYeuCauTiepNhan,
                    MaBN = s.BenhNhan.MaBN,
                    HoTenNguoiBenh = s.HoTen,
                    Tuoi = s.NamSinh != null && s.NamSinh != 0 ? DateTime.Now.Year - s.NamSinh : null,
                    TuoiDisplay = DateHelper.DOBFormat(s.NgaySinh, s.ThangSinh, s.NamSinh),
                    Gioi = s.GioiTinh.GetValueOrDefault().GetDescription(),
                    NgayVaoVien = s.ThoiDiemTiepNhan.ApplyFormatDate(),
                    CoBHYT = s.CoBHYT,
                    SoGiuong = s.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan) ? s.YeuCauDichVuGiuongBenhViens.OrderByDescending(p => p.Id).FirstOrDefault(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan).GiuongBenh.Ten : "",
                    SoBA = s.NoiTruBenhAn.SoBenhAn,
                    KhoaPhong = s.NoiTruBenhAn.KhoaPhongNhapVien.Ten,
                    MaTheBHYT = s.BHYTMaSoThe,
                    Buong = s.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan && p.ThoiDiemBatDauSuDung <= today && (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today))
                          && s.YeuCauDichVuGiuongBenhViens.Any(p => p.GiuongBenhId != null) ? s.YeuCauDichVuGiuongBenhViens.Where(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan && p.ThoiDiemBatDauSuDung <= today && (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today)).Select(x => x.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault() : ""
                }).FirstOrDefault();
            return thongTin;
        }

        public string InPhieuCongKhaiThuocVatTu(InPhieuCongKhaiThuocVatTuReOrder inToaThuoc)
        {
            var content = string.Empty;
            var thongTin = ThongTinChungBenhNhanPhieuDieuTri(inToaThuoc.YeuCauTiepNhanId);
            if (inToaThuoc.LoaiThuocVatTu == 1) // 1: Thuốc, 2: Vật Tư
            {
                var phieuThuoc = thongTin.CoBHYT == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuCongKhaiThuocBHYT")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuCongKhaiThuocKhongBHYT")).First();

                var thuocs = new List<ThongTinThuocVatTuPhieuDieuTri>();
                var thuocDocs = new List<ThongTinThuocVatTuPhieuDieuTri>();
                var dichTruyens = new List<ThongTinThuocVatTuPhieuDieuTri>();

                var thuoc = string.Empty;
                var STT = 1;
                var noiTru = _noiTruPhieuDieuTriRepository.TableNoTracking.FirstOrDefault(p => p.Id == inToaThuoc.NoiTruPhieuDieuTriId);
                var tongSLThuoc = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && p.NoiTruChiDinhDuocPhamId != null
                //&& inToaThuoc.Ids.Contains(p.NoiTruChiDinhDuocPhamId.GetValueOrDefault())
                ).Sum(p => p.SoLuong);
                decimal tongThanhTien = 0;

                thuocDocs = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId
                       //&& inToaThuoc.Ids.Contains(p.NoiTruChiDinhDuocPhamId.GetValueOrDefault())
                       && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                       && p.NoiTruChiDinhDuocPhamId != null
                       && p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.ThuocDoc
                     )
                     .Select(s => new ThongTinThuocVatTuPhieuDieuTri
                     {
                         STT = s.NoiTruChiDinhDuocPham.SoThuTu,
                         //  TenThuoc = s.Ten + (!string.IsNullOrEmpty(s.HamLuong) ? " (" + s.HamLuong + ")" : ""),
                         TenThuoc = s.Ten,
                         DonVi = s.DonViTinh.Ten,
                         SoLuong = s.SoLuong,
                         DonGia = s.DonGiaBan,
                         ThanhTien = (decimal)(s.SoLuong * (double)s.DonGiaBan),
                         GhiChu = s.GhiChu,
                         KhongTinhPhi = s.KhongTinhPhi,
                         LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                         DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                         DuongDungId = s.DuongDungId,
                         HamLuong = s.HamLuong,
                         HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                         LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                     })
                  .GroupBy(x => new { x.TenThuoc, x.DonVi, x.DonGia, x.STT })
                     .Select(item => new ThongTinThuocVatTuPhieuDieuTri
                     {
                         STT = item.First().STT,
                         TenThuoc = item.First().TenThuoc,
                         DonVi = item.First().DonVi,
                         //SoLuong = item.First().SoLuong,
                         DuongDungId = item.First().DuongDungId,
                         SoLuong = item.Sum(x => x.SoLuong),
                         TongSo = item.Sum(x => x.SoLuong),
                         DonGia = item.First().DonGia,
                         ThanhTien = item.Sum(x => x.ThanhTien),
                         GhiChu = item.First().GhiChu,
                         KhongTinhPhi = item.First().KhongTinhPhi,
                         LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                         DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVienPhanNhomId,
                         HamLuong = item.First().HamLuong,
                         HoatChat = item.First().HoatChat,
                         LoaiThuocTheoQuanLy = item.First().LoaiThuocTheoQuanLy
                     }).ToList();

                dichTruyens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId
                       //&& inToaThuoc.Ids.Contains(p.NoiTruChiDinhDuocPhamId.GetValueOrDefault())
                       && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                       && p.NoiTruChiDinhDuocPhamId != null
                       && p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc
                       && p.NoiTruChiDinhDuocPham.LaDichTruyen == true
                       //&& p.DuongDung.Ma.Trim() != "2.10".Trim()
                       //&& p.DuongDung.Ma.Trim() != "1.01".Trim()
                       //&& p.DuongDung.Ma.Trim() != "4.04".Trim()
                       //&& p.DuongDung.Ma.Trim() != "3.05".Trim()
                     )
                     .Select(s => new ThongTinThuocVatTuPhieuDieuTri
                     {
                         STT = s.NoiTruChiDinhDuocPham.SoThuTu,
                         //  TenThuoc = s.Ten + (!string.IsNullOrEmpty(s.HamLuong) ? " (" + s.HamLuong + ")" : ""),
                         TenThuoc = s.Ten,
                         DonVi = s.DonViTinh.Ten,
                         SoLuong = s.SoLuong,
                         DonGia = s.DonGiaBan,
                         ThanhTien = (decimal)(s.SoLuong * (double)s.DonGiaBan),
                         GhiChu = s.GhiChu,
                         KhongTinhPhi = s.KhongTinhPhi,
                         LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                         DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                         DuongDungId = s.DuongDungId,
                         HamLuong = s.HamLuong,
                         HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                         LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                     })
                  .GroupBy(x => new { x.TenThuoc, x.DonVi, x.DonGia, x.STT })
                     .Select(item => new ThongTinThuocVatTuPhieuDieuTri
                     {
                         STT = item.First().STT,
                         TenThuoc = item.First().TenThuoc,
                         DonVi = item.First().DonVi,
                         //SoLuong = item.First().SoLuong,
                         DuongDungId = item.First().DuongDungId,
                         SoLuong = item.Sum(x => x.SoLuong),
                         TongSo = item.Sum(x => x.SoLuong),
                         DonGia = item.First().DonGia,
                         ThanhTien = item.Sum(x => x.ThanhTien),
                         GhiChu = item.First().GhiChu,
                         KhongTinhPhi = item.First().KhongTinhPhi,
                         LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                         DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVienPhanNhomId,
                         HamLuong = item.First().HamLuong,
                         HoatChat = item.First().HoatChat,
                         LoaiThuocTheoQuanLy = item.First().LoaiThuocTheoQuanLy
                     }).ToList();

                thuocs = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                       .Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId
                       //&& inToaThuoc.Ids.Contains(p.NoiTruChiDinhDuocPhamId.GetValueOrDefault())
                       && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                       && p.NoiTruChiDinhDuocPhamId != null
                       && p.LaDichTruyen != true
                       && p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.ThuocDoc
                       )
                     .Select(s => new ThongTinThuocVatTuPhieuDieuTri
                     {
                         STT = s.NoiTruChiDinhDuocPham.SoThuTu,
                         TenThuoc = s.Ten,
                         DonVi = s.DonViTinh.Ten,
                         SoLuong = s.SoLuong,
                         DonGia = s.DonGiaBan,
                         ThanhTien = (decimal)(s.SoLuong * (double)s.DonGiaBan),
                         GhiChu = s.GhiChu,
                         KhongTinhPhi = s.KhongTinhPhi,
                         DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                         DuongDungId = s.DuongDungId,
                         HamLuong = s.HamLuong,
                         HoatChat = s.DuocPhamBenhVien.DuocPham.HoatChat,
                         LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                         LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                     })
                  .GroupBy(x => new { x.TenThuoc, x.DonVi, x.DonGia, x.STT })
                     .Select(item => new ThongTinThuocVatTuPhieuDieuTri
                     {
                         STT = item.First().STT,
                         TenThuoc = item.First().TenThuoc,
                         DonVi = item.First().DonVi,
                         //SoLuong = item.First().SoLuong,
                         DuongDungId = item.First().DuongDungId,
                         SoLuong = item.Sum(x => x.SoLuong),
                         TongSo = item.Sum(x => x.SoLuong),
                         DonGia = item.First().DonGia,
                         ThanhTien = item.Sum(x => x.ThanhTien),
                         GhiChu = item.First().GhiChu,
                         KhongTinhPhi = item.First().KhongTinhPhi,
                         DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVienPhanNhomId,
                         HamLuong = item.First().HamLuong,
                         HoatChat = item.First().HoatChat,
                         LoaiThuocTheoQuanLy = item.First().LoaiThuocTheoQuanLy,
                         LaDuocPhamBHYT = item.First().LaDuocPhamBHYT
                     }).ToList();

                var dataAll = (thuocDocs.OrderBy(z => z.DuongDungNumber).ThenBy(z => z.STT))
                    .Concat(dichTruyens.OrderBy(z => z.DuongDungNumber).ThenBy(z => z.STT))
                    .Concat(thuocs.OrderBy(z => z.DuongDungNumber).ThenBy(z => z.STT));
                decimal thanhTienKhong = 0;
                if (dataAll.Any())
                {
                    foreach (var item in dataAll)
                    {
                        tongThanhTien += item.KhongTinhPhi != true ? item.ThanhTien : 0;
                        thuoc += "<tr style = 'border: 1px solid #020000;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                       + "<td style = 'border: 1px solid #020000;padding-left:3px;'>" + _yeuCauKhamBenhService.FormatTenDuocPham(item.TenThuoc, item.HoatChat, item.HamLuong, item.DuocPhamBenhVienPhanNhomId)
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonVi
                                       + "<td style = 'border: 1px solid #020000;text-align:center;'>" + _yeuCauKhamBenhService.FormatSoLuong(item.SoLuong, item.LoaiThuocTheoQuanLy)
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.TongSo
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + item.DonGia.ApplyFormatMoneyVND()
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + (item.KhongTinhPhi != true ? item.ThanhTien.ApplyFormatMoneyVND() : thanhTienKhong.ApplyFormatMoneyVND())
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px; word-break: break-word;'>" + item.GhiChu
                                       + "</tr>";
                        STT++;
                    }
                }
                thuoc += "<tr style = 'border: 1px solid #020000;'>"
                                       + "<td colspan='2' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Tổng số khoản thuốc dùng</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "<b>" + tongSLThuoc + "</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + "<b>" + tongThanhTien.ApplyFormatMoneyVND() + "</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                                       + "</tr>"
                                       + "<tr>"
                                       + "<td colspan='2' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Chữ ký của người bệnh/ người nhà người bệnh</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                                       + "</tr>"
                                       ;
                var data = new DataThuocVatTuPhieuDieuTri
                {
                    LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    MaTN = thongTin.MaTN,
                    SoBA = thongTin.SoBA,
                    KhoaPhong = thongTin.KhoaPhong,
                    SoVaoVien = thongTin.MaTN,
                    HoTenNguoiBenh = thongTin.HoTenNguoiBenh,
                    Buong = thongTin.Buong,
                    Thuoc = thuoc,
                    Tuoi = thongTin.Tuoi,
                    Gioi = thongTin.Gioi,
                    SoGiuong = thongTin.SoGiuong,
                    MaTheBHYT = thongTin.MaTheBHYT,
                    NgayVaoVien = thongTin.NgayVaoVien,
                    NgayRaVien = "&nbsp;",
                    ChanDoan = noiTru.ChanDoanChinhGhiChu,
                    NgayThangNam = noiTru.NgayDieuTri.ApplyFormatDate(),
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(phieuThuoc.Body, data);
            }
            else //Vật tư
            {
                var phieuVatTu = thongTin.CoBHYT == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuCongKhaiVatTuBHYT")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuCongKhaiVatTuKhongBHYT")).First();

                var vatTus = new List<ThongTinThuocVatTuPhieuDieuTri>();
                var vatTu = string.Empty;
                var STT = 1;
                var noiTru = _noiTruPhieuDieuTriRepository.TableNoTracking.FirstOrDefault(p => p.Id == inToaThuoc.NoiTruPhieuDieuTriId);
                var tongSLVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Sum(p => p.SoLuong);
                decimal tongThanhTien = 0;
                vatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                      .Select(s => new ThongTinThuocVatTuPhieuDieuTri
                      {
                          TenVatTu = s.Ten,
                          DonVi = s.DonViTinh,
                          SoLuong = s.SoLuong,
                          DonGia = s.DonGiaBan,
                          ThanhTien = (decimal)(s.SoLuong * (double)s.DonGiaBan),
                          GhiChu = s.GhiChu,
                          KhongTinhPhi = s.KhongTinhPhi
                      })
                   .GroupBy(x => new { x.TenVatTu, x.DonVi, x.DonGia })
                      .Select(item => new ThongTinThuocVatTuPhieuDieuTri
                      {
                          TenVatTu = item.First().TenVatTu,
                          DonVi = item.First().DonVi,
                          //SoLuong = item.First().SoLuong,
                          SoLuong = item.Sum(x => x.SoLuong),
                          TongSo = item.Sum(x => x.SoLuong),
                          DonGia = item.First().DonGia,
                          ThanhTien = item.Sum(x => x.ThanhTien),
                          GhiChu = item.First().GhiChu,
                          KhongTinhPhi = item.First().KhongTinhPhi,

                      }).Distinct().ToList();
                decimal thanhTienKhong = 0;
                if (vatTus.Any())
                {
                    foreach (var item in vatTus)
                    {
                        tongThanhTien += item.KhongTinhPhi != true ? item.ThanhTien : 0;
                        vatTu += "<tr style = 'border: 1px solid #020000;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                       + "<td style = 'border: 1px solid #020000;padding-left:3px;'>" + item.TenVatTu
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonVi
                                       + "<td style = 'border: 1px solid #020000;text-align:center;'>" + item.SoLuong
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.TongSo
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + item.DonGia.ApplyFormatMoneyVND()
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + (item.KhongTinhPhi != true ? item.ThanhTien.ApplyFormatMoneyVND() : thanhTienKhong.ApplyFormatMoneyVND())
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px; word-break: break-word;'>" + item.GhiChu
                                       + "</tr>";
                        STT++;
                    }
                }
                vatTu += "<tr style = 'border: 1px solid #020000;'>"
                                       + "<td colspan='2' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Tổng số khoản VTTH dùng</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "<b>" + tongSLVatTu + "</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + "<b>" + tongThanhTien.ApplyFormatMoneyVND() + "</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                                       + "</tr>"
                                       + "<tr>"
                                       + "<td colspan='2' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Chữ ký của người bệnh/ người nhà người bệnh</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                                       + "</tr>"
                                       ;
                var data = new DataThuocVatTuPhieuDieuTri
                {
                    LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    MaTN = thongTin.MaTN,
                    SoBA = thongTin.SoBA,
                    KhoaPhong = thongTin.KhoaPhong,
                    SoVaoVien = thongTin.MaTN,
                    HoTenNguoiBenh = thongTin.HoTenNguoiBenh,
                    Buong = thongTin.Buong,
                    VatTu = vatTu,
                    Tuoi = thongTin.Tuoi,
                    Gioi = thongTin.Gioi,
                    SoGiuong = thongTin.SoGiuong,
                    MaTheBHYT = thongTin.MaTheBHYT,
                    NgayVaoVien = thongTin.NgayVaoVien,
                    NgayRaVien = "&nbsp;",
                    ChanDoan = noiTru?.ChanDoanChinhGhiChu,
                    NgayThangNam = noiTru?.NgayDieuTri.ApplyFormatDate(),
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(phieuVatTu.Body, data);
            }
            return content;
        }

        public string InPhieuThucHienThuocVatTu(InPhieuThucHienThuocVatTu inToaThuoc)
        {
            var content = string.Empty;
            var thongTin = ThongTinChungBenhNhanPhieuDieuTri(inToaThuoc.YeuCauTiepNhanId);
            var ngayDieuTri = _noiTruPhieuDieuTriRepository.GetById(inToaThuoc.NoiTruPhieuDieuTriId);
            if (inToaThuoc.LoaiThuocVatTu == 1) // 1: Thuốc, 2: Vật Tư
            {
                var phieuThuoc = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuThucHienThuocVer2")).First();

                var thuocs = new List<ThongTinThuocVatTuPhieuDieuTriThucHien>();
                var thuoc = string.Empty;
                var STT = 1;
                var noiTru = _noiTruPhieuDieuTriRepository.TableNoTracking.Include(z => z.ChanDoanChinhICD).FirstOrDefault(p => p.Id == inToaThuoc.NoiTruPhieuDieuTriId);
                //&& p.NoiTruChiDinhDuocPham.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null)

                //&& p.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null)
                thuocs = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                       .Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId 
                                   && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                   
                                   //BVHD-3859
                                   //Cập nhật 16/03/2022: yêu cầu hiện luôn cả dược phẩm đã hoàn trả
                                   //&& p.SoLuong > 0
                                   )
                     .Select(s => new ThongTinThuocVatTuPhieuDieuTriThucHien
                     {
                         TenThuoc = s.Ten,
                         DonVi = s.DonViTinh.Ten,
                         SoLuong = s.SoLuong,
                         DuongDung = s.DuongDung.Ten,
                         // thuốc gây nghiện,hướng thần thì cách dùng con số chuyển thành text , ngược lại nếu thuống thường kiểm tra sl kê nhỏ hơn 10 thì thêm 0 phía trước , còn lại bình thường
                         DungSang = (s.DungSang != null ? ((s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungSang), false) : (s.DungSang < 10 && Convert.ToInt32(s.DungSang) == s.DungSang ? "0" + s.DungSang.FloatToStringFraction() + " " : s.DungSang.FloatToStringFraction() + " ")) : "")
                                  + (s.ThoiGianDungSang != null ? "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")" : ""),

                         DungTrua = (s.DungTrua != null ? ((s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungTrua), false) : (s.DungTrua < 10 && Convert.ToInt32(s.DungTrua) == s.DungTrua ? "0" + s.DungTrua.FloatToStringFraction() + " " : s.DungTrua.FloatToStringFraction() + " ")) : "")
                                  + (s.ThoiGianDungTrua != null ? "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")" : ""),

                         DungChieu = (s.DungChieu != null ? ((s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungChieu), false) : (s.DungChieu < 10 && Convert.ToInt32(s.DungChieu) == s.DungChieu ? "0" + s.DungChieu.FloatToStringFraction() + " " : s.DungChieu.FloatToStringFraction() + " ")) : "")
                                  + (s.ThoiGianDungChieu != null ? "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")" : ""),

                         DungToi = (s.DungToi != null ? ((s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || s.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(s.DungToi), false) : (s.DungToi < 10 && Convert.ToInt32(s.DungToi) == s.DungToi ? "0" + s.DungToi.FloatToStringFraction() + " " : s.DungToi.FloatToStringFraction() + " ")) : "")
                                  + (s.ThoiGianDungToi != null ? "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")" : ""),

                         GhiChu = s.GhiChu,
                         BSChiDinh = s.NhanVienChiDinh.User.HoTen,

                         ThoiDiemXacNhanThucHien = s.NoiTruPhieuDieuTriChiTietYLenhs.Any() ? s.NoiTruPhieuDieuTriChiTietYLenhs.Select(z => z.ThoiDiemXacNhanThucHien).FirstOrDefault() : null,

                         STT = s.SoThuTu,
                         DuongDungId = s.DuongDungId,
                         // update format DuocPham
                         LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                         DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                         HamLuong = s.HamLuong,
                         HoatChat = s.HoatChat,
                         LaDichTruyen = s.LaDichTruyen != null && s.LaDichTruyen == true,

                         //BVHD-3859
                         NoiTruChiDinhDuocPhamId = s.Id,
                         ChiTietXuats = s.YeuCauDuocPhamBenhViens.Where(a => a.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                                                                             //Cập nhật 16/03/2022: yêu cầu hiện luôn cả dược phẩm đã hoàn trả
                                                                             //&& a.SoLuong > 0)
                                 .Select(a => new ChiTietDuocPhamTheoXuatVo()
                                 {
                                     XuatKhoDuocPhamChiTietId = a.XuatKhoDuocPhamChiTietId
                                 }).ToList(),
                         CachDungDisplay = s.GhiChu
                     }).ToList();

                #region BVHD-3859
                var lstXuatKhoDuocPhamChiTietId = thuocs.SelectMany(x => x.ChiTietXuats)
                    .Select(x => x.XuatKhoDuocPhamChiTietId)
                    .Where(x => x != null).Distinct().ToList();

                var lstXuatViTri = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                    .Where(x => lstXuatKhoDuocPhamChiTietId.Contains(x.XuatKhoDuocPhamChiTietId))
                    .Select(x => new ChiTietNhapKhoDuocPhamVo()
                    {
                        XuatKhoDuocPhamChiTietId = x.XuatKhoDuocPhamChiTietId,
                        NhapKhoDuocPhamChiTietId = x.NhapKhoDuocPhamChiTietId,
                        HanSuDung = x.NhapKhoDuocPhamChiTiet.HanSuDung,
                        SoLo = x.NhapKhoDuocPhamChiTiet.Solo,
                        SoLuong = x.SoLuongXuat
                    }).ToList();

                //BVHD-3859: cập nhật ngày 18/03/2022: gập dược phẩm lại 1 dòng theo tên
                //var lstNoiTruChiDinhBoSung = new List<ThongTinThuocVatTuPhieuDieuTriThucHien>();
                var listThuocDaXuat = thuocs.Where(a => a.ChiTietXuats.Any(b => b.XuatKhoDuocPhamChiTietId != null)).ToList();
                foreach (var duocPham in listThuocDaXuat)
                {
                    var lstXuatViTriTheoNoiTruChiDinh = lstXuatViTri
                        .Where(x => duocPham.ChiTietXuats.Select(a => a.XuatKhoDuocPhamChiTietId.Value)
                                                        .Contains(x.XuatKhoDuocPhamChiTietId)).ToList();
                    // xử lý loại bỏ các xuất chi tiết vị trí đã hoàn trả, data hoàn trả thì số lượng âm theo nhập kho chi tiết
                    lstXuatViTriTheoNoiTruChiDinh = lstXuatViTriTheoNoiTruChiDinh
                        .GroupBy(x => new {x.XuatKhoDuocPhamChiTietId, x.NhapKhoDuocPhamChiTietId})
                        .Select(x => new ChiTietNhapKhoDuocPhamVo()
                        {
                            XuatKhoDuocPhamChiTietId = x.Key.XuatKhoDuocPhamChiTietId,
                            NhapKhoDuocPhamChiTietId = x.Key.NhapKhoDuocPhamChiTietId,
                            HanSuDung = x.First().HanSuDung,
                            SoLo = x.First().SoLo,
                            SoLuong = x.Sum(a => a.SoLuong).MathRoundNumber(2)
                        })
                        //Cập nhật 16/03/2022: yêu cầu hiện luôn cả dược phẩm đã hoàn trả
                        //.Where(x => x.SoLuong > 0)
                        .ToList();


                    //BVHD-3859: cập nhật ngày 18/03/2022: gập dược phẩm lại 1 dòng theo tên
                    //if (lstXuatViTriTheoNoiTruChiDinh.Any())
                    //{
                    //    foreach (var xuatViTri in lstXuatViTriTheoNoiTruChiDinh)
                    //    {
                    //        var newNoiTruChiDinh = duocPham.Clone();
                    //        newNoiTruChiDinh.HanSuDung = xuatViTri.HanSuDung;
                    //        newNoiTruChiDinh.SoLo = xuatViTri.SoLo;
                    //        newNoiTruChiDinh.SoLuong = xuatViTri.SoLuong;
                    //        lstNoiTruChiDinhBoSung.Add(newNoiTruChiDinh);
                    //    }
                    //}

                    duocPham.ChiTietXuats = duocPham.ChiTietXuats
                        .Where(x => x.XuatKhoDuocPhamChiTietId != null
                                    && lstXuatViTriTheoNoiTruChiDinh
                                        .Select(a => a.XuatKhoDuocPhamChiTietId)
                                        .Contains(x.XuatKhoDuocPhamChiTietId.Value))
                        .Distinct().ToList();
                    foreach (var chiTietXuat in duocPham.ChiTietXuats)
                    {
                        chiTietXuat.ChiTietNhaps = lstXuatViTriTheoNoiTruChiDinh
                            .Where(x => x.XuatKhoDuocPhamChiTietId == chiTietXuat.XuatKhoDuocPhamChiTietId).ToList();
                    }
                }

                //BVHD-3859: cập nhật ngày 18/03/2022: gập dược phẩm lại 1 dòng theo tên
                //if (lstNoiTruChiDinhBoSung.Any())
                //{
                //    var lstNoiTruChiDinhBoSungId = lstNoiTruChiDinhBoSung.Select(x => x.NoiTruChiDinhDuocPhamId).Distinct().ToList();
                //    thuocs = thuocs.Where(x => !lstNoiTruChiDinhBoSungId.Contains(x.NoiTruChiDinhDuocPhamId)).ToList();
                //    thuocs.AddRange(lstNoiTruChiDinhBoSung);
                //}
                #endregion

                var dataGroup = thuocs
                    //BVHD-3859
                    //.GroupBy(x => new { x.TenThuoc, x.DonVi, x.STT })

                    //BVHD-3859: cập nhật ngày 18/03/2022: gập dược phẩm lại 1 dòng theo tên
                    //.GroupBy(x => new { x.TenThuoc, x.HanSuDung, x.SoLo, x.CachDungDisplay })
                    .GroupBy(x => new { x.TenThuoc, x.HoatChat, x.HamLuong, x.DuocPhamBenhVienPhanNhomId })
                   .Select(item => new ThongTinThuocVatTuPhieuDieuTriThucHien
                   {
                       STT = item.First().STT,
                       TenThuoc = item.First().TenThuoc,
                       DonVi = item.First().DonVi,
                       DuongDung = item.First().DuongDung,
                       DuongDungId = item.First().DuongDungId,
                       SoLuong = item.Sum(x => x.SoLuong).MathRoundNumber(2),
                       TongSo = item.Sum(x => x.SoLuong).MathRoundNumber(2),
                       SLDungSang = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungSang),
                       SLDungTrua = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungTrua),
                       SLDungChieu = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungChieu),
                       SLDungToi = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungToi),
                       DungSangs = string.Join(",", item.Where(z => !string.IsNullOrEmpty(z.DungSang)).Select(x => x.DungSang)),
                       DungTruas = string.Join(",", item.Where(z => !string.IsNullOrEmpty(z.DungTrua)).Select(x => x.DungTrua)),
                       DungChieus = string.Join(",", item.Where(z => !string.IsNullOrEmpty(z.DungChieu)).Select(x => x.DungChieu)),
                       DungTois = string.Join(",", item.Where(z => !string.IsNullOrEmpty(z.DungToi)).Select(x => x.DungToi)),
                       GhiChus = string.Join(",", item.Select(x => x.GhiChu)),
                       //BSChiDinhs = string.Join(",", item.Select(x => x.BSChiDinh).Distinct()),
                       BSChiDinhs = item.First().BSChiDinh,
                       LoaiThuocTheoQuanLy = item.First().LoaiThuocTheoQuanLy,
                       DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVienPhanNhomId,
                       HamLuong = item.First().HamLuong,
                       HoatChat = item.First().HoatChat,
                       LaDichTruyen = item.First().LaDichTruyen,

                       //BVHD-3859
                       //HanSuDung = item.Key.HanSuDung, //  string.Join("<br>", item.Where(a => a.HanSuDungDisplay != null).Select(a => a.HanSuDungDisplay)),
                       //SoLo = item.Key.SoLo, // string.Join("<br>", item.Where(a => a.SoLo != null).Select(a => a.SoLo)),
                       //CachDungDisplay = item.Key.CachDungDisplay // string.Join("<br>", item.Where(a => !string.IsNullOrEmpty(a.CachDungDisplay)).Select(a => a.CachDungDisplay)),

                       ChiTietXuats = item.SelectMany(a => a.ChiTietXuats).ToList(),
                       CachDungDisplay = item.Any(a => !string.IsNullOrEmpty(a.CachDungDisplay))
                           ? string.Join("; ", item.Where(a => !string.IsNullOrEmpty(a.CachDungDisplay)).Select(a => a.CachDungDisplay).Distinct().ToList())
                           : string.Empty

                   }).OrderBy(z => !z.LaDichTruyen).ThenBy(z => z.STT).ToList();
                if (dataGroup.Any())
                {
                    //var rowConstant = 10; // số dòng mặc định theo yc
                    foreach (var item in dataGroup)
                    {
                        thuoc += "<tr style = 'border: 1px solid #020000;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT + "</td>"
                                       + "<td style = 'border: 1px solid #020000;padding-left:3px;'>" + _yeuCauKhamBenhService.FormatTenDuocPham(item.TenThuoc, item.HoatChat, item.HamLuong, item.DuocPhamBenhVienPhanNhomId, true) + "</td>"

                                       //BVHD-3859
                                       //+ "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center; font-size: 10px; padding: 1px !important;'>" + item.HanSuDungDisplay + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center; font-size: 10px; padding: 1px !important;'>" + item.SoLo + "</td>"

                                       + "<td style = 'border: 1px solid #020000;text-align:center; font-size: 10px; padding: 1px !important;'>" + (item.SoLuong != null ? _yeuCauKhamBenhService.FormatSoLuong(Convert.ToDouble(item.SoLuong), item.LoaiThuocTheoQuanLy) : "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.CachDungDisplay + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px; word-break: break-word;'>" + item.BSChiDinhs + "</td>"
                                       + "</tr>";
                        STT++;
                    }
                    //if (dataGroup.Count < rowConstant)
                    //{
                    for (int i = 0; i < 5; i++)
                    {
                        thuoc += "<tr style = 'border: 1px solid #020000;'>"
                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                   + "<td style = 'border: 1px solid #020000;padding-left:3px;'>" + "&nbsp;"
                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"

                                   //BVHD-3859
                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"

                                   + "<td style = 'border: 1px solid #020000;text-align:center;'>" + "&nbsp;"
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                   + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px; word-break: break-word;'>" + "&nbsp;"
                                   + "</tr>";
                        STT++;
                    }
                    //}
                }
                //var tongSLThuoc = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && p.NoiTruChiDinhDuocPhamId != null).Sum(p => p.SoLuong);
                var tongSLThuoc = dataGroup.Count();
                thuoc += "<tr style = 'border: 1px solid #020000;'>"
                                        //BVHD-3859: colspan 3 -> 4
                                       + "<td colspan='4' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Tổng số khoản thuốc dùng</b>"
                                       + "<td colspan='27' style = 'border: 1px solid #020000;text-align: left;'>" + "<b>" + tongSLThuoc + "</b>"
                                       + "</tr>"
                                       + "<tr>"
                                        //BVHD-3859: colspan 5 -> 6
                                       + "<td colspan='6' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Người thực hiện (Điều dưỡng)</b>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"//BSTrua
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"//BSChieu
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"//BSToi
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"//BSSang
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                                       + "</tr>"
                                       ;
                var data = new DataThuocVatTuPhieuDieuTri
                {
                    LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    MaTN = thongTin.MaTN,
                    MaBN = thongTin.MaBN,
                    SoBA = thongTin.SoBA,
                    KhoaPhong = thongTin.KhoaPhong,
                    HoTenNguoiBenh = thongTin.HoTenNguoiBenh,
                    Buong = thongTin.Buong,
                    Thuoc = thuoc,
                    TuoiStr = thongTin.TuoiDisplay,
                    Gioi = thongTin.Gioi,
                    SoGiuong = thongTin.SoGiuong,
                    NgayVaoVien = thongTin.NgayVaoVien,
                    ChanDoan = noiTru.ChanDoanChinhICD != null ? noiTru.ChanDoanChinhICD.Ma + " - " + noiTru.ChanDoanChinhICD.TenTiengViet : "",
                    NgayDieuTri = ngayDieuTri.NgayDieuTri.ApplyFormatNgayThangNam()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(phieuThuoc.Body, data);
            }
            else //Vật tư
            {
                //var phieuVatTu = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuThucHienVTYT")).First();

                //var vatTus = new List<ThongTinThuocVatTuPhieuDieuTriThucHien>();
                //var vatTu = string.Empty;
                //var STT = 1;
                //var noiTru = _noiTruPhieuDieuTriRepository.TableNoTracking.Include(z => z.ChanDoanChinhICD).FirstOrDefault(p => p.Id == inToaThuoc.NoiTruPhieuDieuTriId);
                ////&& p.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null)
                //var tongSLVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Sum(p => p.SoLuong);
                ////&& p.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null)
                //vatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                //         .Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                //       .Select(s => new ThongTinThuocVatTuPhieuDieuTriThucHien
                //       {
                //           TenVatTu = s.Ten + (!string.IsNullOrEmpty(s.NhaSanXuat) ? " - " + s.NhaSanXuat : "") + (!string.IsNullOrEmpty(s.NuocSanXuat) ? " - " + s.NuocSanXuat : ""),
                //           DonVi = s.DonViTinh,
                //           SoLuong = s.SoLuong,
                //           GhiChu = s.GhiChu,
                //           BSChiDinh = s.NhanVienChiDinh.User.HoTen,

                //           ThoiDiemXacNhanThucHien = s.NoiTruPhieuDieuTriChiTietYLenhs.Any() ? s.NoiTruPhieuDieuTriChiTietYLenhs.Select(z => z.ThoiDiemXacNhanThucHien).FirstOrDefault() : null,

                //       }).ToList();

                //var dataGroup = vatTus.GroupBy(x => new { x.TenVatTu, x.DonVi })
                //   .Select(item => new ThongTinThuocVatTuPhieuDieuTriThucHien
                //   {
                //       TenVatTu = item.First().TenVatTu,
                //       DonVi = item.First().DonVi,
                //       SoLuong = item.Sum(x => x.SoLuong),
                //       TongSo = item.Sum(x => x.SoLuong),
                //       SLDungSang = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungSang),
                //       SLDungTrua = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungTrua),
                //       SLDungChieu = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungChieu),
                //       SLDungToi = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungToi),
                //       GhiChus = string.Join(",", item.Select(x => x.GhiChu)),
                //       //BSChiDinhs = string.Join(",", item.Select(x => x.BSChiDinh).Distinct()),
                //       BSChiDinhs = item.First().BSChiDinh,
                //   }).ToList();
                //if (dataGroup.Any())
                //{
                //    //var rowConstant = 10; // số dòng mặc định theo yc
                //    foreach (var item in dataGroup)
                //    {
                //        vatTu += "<tr style = 'border: 1px solid #020000;'>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                //                       + "<td style = 'border: 1px solid #020000;padding-left:3px;'>" + item.TenVatTu
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonVi
                //                       + "<td style = 'border: 1px solid #020000;text-align:center;'>" + item.SoLuong
                //                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.CachDung
                //                         + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px; word-break: break-word;'>" + item.BSChiDinhs
                //                       + "</tr>";
                //        STT++;
                //    }

                //    for (int i = 0; i < 5; i++)
                //    {
                //        vatTu += "<tr style = 'border: 1px solid #020000;'>"
                //                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                //                   + "<td style = 'border: 1px solid #020000;padding-left:3px;'>" + "&nbsp;"
                //                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                   + "<td style = 'border: 1px solid #020000;text-align:center;'>" + "&nbsp;"
                //                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                   + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px; word-break: break-word;'>" + "&nbsp;"
                //                   + "</tr>";
                //        STT++;
                //    }
                //}
                //vatTu += "<tr style = 'border: 1px solid #020000;'>"
                //                      + "<td colspan='3' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Tổng số VTYT dùng</b>"
                //                      + "<td colspan='7' style = 'border: 1px solid #020000;text-align: left;'>" + "<b>" + tongSLVatTu + "</b>"
                //                      + "</tr>"
                //                      + "<tr>"
                //                      + "<td colspan='5' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Người thực hiện (Điều dưỡng)</b>"
                //                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"//BSTrua
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"//BSChieu
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"//BSToi
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"//BSSang
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                //                      + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                //                      + "</tr>"
                //                      ;
                //var data = new DataThuocVatTuPhieuDieuTri
                //{
                //    LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                //    MaTN = thongTin.MaTN,
                //    SoBA = thongTin.SoBA,
                //    MaBN = thongTin.MaBN,
                //    KhoaPhong = thongTin.KhoaPhong,
                //    //HoTenNguoiBenh = thongTin.HoTenNguoiBenh,
                //    Buong = thongTin.Buong,
                //    VatTu = vatTu,
                //    TuoiStr = thongTin.TuoiDisplay,
                //    Gioi = thongTin.Gioi,
                //    SoGiuong = thongTin.SoGiuong,
                //    NgayVaoVien = thongTin.NgayVaoVien,
                //    ChanDoan = noiTru.ChanDoanChinhICD != null ? noiTru.ChanDoanChinhICD.Ma + " - " + noiTru.ChanDoanChinhICD.TenTiengViet : "",
                //    NgayThangNam = noiTru?.NgayDieuTri.ApplyFormatDate(),
                //    NgayDieuTri = ngayDieuTri.NgayDieuTri.ApplyFormatNgayThangNam()
                //};
                //content = TemplateHelpper.FormatTemplateWithContentTemplate(phieuVatTu.Body, data);

                //version 1
                var phieuVatTu = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuThucHienVTYT")).First();

                var vatTus = new List<ThongTinThuocVatTuPhieuDieuTriThucHien>();
                var vatTu = string.Empty;
                var STT = 1;
                var noiTru = _noiTruPhieuDieuTriRepository.TableNoTracking.Include(z => z.ChanDoanChinhICD).FirstOrDefault(p => p.Id == inToaThuoc.NoiTruPhieuDieuTriId);
                //&& p.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null)
                //var tongSLVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Sum(p => p.SoLuong);
                //&& p.NoiTruPhieuDieuTriChiTietYLenhs.Any(z => z.ThoiDiemXacNhanThucHien != null)
                vatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                         .Where(p => p.NoiTruPhieuDieuTriId == inToaThuoc.NoiTruPhieuDieuTriId && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                       .Select(s => new ThongTinThuocVatTuPhieuDieuTriThucHien
                       {
                           TenVatTu = s.Ten + (!string.IsNullOrEmpty(s.NhaSanXuat) ? " - " + s.NhaSanXuat : "") + (!string.IsNullOrEmpty(s.NuocSanXuat) ? " - " + s.NuocSanXuat : ""),
                           DonVi = s.DonViTinh,
                           SoLuong = s.SoLuong,
                           GhiChu = s.GhiChu,
                           BSChiDinh = s.NhanVienChiDinh.User.HoTen,

                           ThoiDiemXacNhanThucHien = s.NoiTruPhieuDieuTriChiTietYLenhs.Any() ? s.NoiTruPhieuDieuTriChiTietYLenhs.Select(z => z.ThoiDiemXacNhanThucHien).FirstOrDefault() : null,
                           SLDungSang = s.NoiTruPhieuDieuTriChiTietYLenhs.Where(z => z.ThoiDiemXacNhanThucHien != null
                                                                                && z.XacNhanThucHien == true
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour >= 3 && z.ThoiDiemXacNhanThucHien.Value.Minute >= 0
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour <= 7 && z.ThoiDiemXacNhanThucHien.Value.Minute <= 59
                                                                                ).Sum(z => z.YeuCauVatTuBenhVien.SoLuong),
                           SLDungTrua = s.NoiTruPhieuDieuTriChiTietYLenhs.Where(z => z.ThoiDiemXacNhanThucHien != null
                                                                                && z.XacNhanThucHien == true
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour >= 8 && z.ThoiDiemXacNhanThucHien.Value.Minute >= 0
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour <= 14 && z.ThoiDiemXacNhanThucHien.Value.Minute <= 59
                                                                                ).Sum(z => z.YeuCauVatTuBenhVien.SoLuong),

                           SLDungChieu = s.NoiTruPhieuDieuTriChiTietYLenhs.Where(z => z.ThoiDiemXacNhanThucHien != null
                                                                                && z.XacNhanThucHien == true
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour >= 15 && z.ThoiDiemXacNhanThucHien.Value.Minute >= 0
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour <= 20 && z.ThoiDiemXacNhanThucHien.Value.Minute <= 59
                                                                                ).Sum(z => z.YeuCauVatTuBenhVien.SoLuong),

                           SLDungToi = s.NoiTruPhieuDieuTriChiTietYLenhs.Where(z => z.ThoiDiemXacNhanThucHien != null
                                                                                && z.XacNhanThucHien == true
                                                                                && (z.ThoiDiemXacNhanThucHien.Value.Hour >= 21 && z.ThoiDiemXacNhanThucHien.Value.Minute >= 0
                                                                                  || z.ThoiDiemXacNhanThucHien.Value.Hour <= 2 && z.ThoiDiemXacNhanThucHien.Value.Minute <= 59)
                                                                                  ).Sum(z => z.YeuCauVatTuBenhVien.SoLuong),

                           BSThucHienChiTietSangs = s.NoiTruPhieuDieuTriChiTietYLenhs.Where(z => z.ThoiDiemXacNhanThucHien != null
                                                                                && z.XacNhanThucHien == true
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour >= 3 && z.ThoiDiemXacNhanThucHien.Value.Minute >= 0
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour <= 7 && z.ThoiDiemXacNhanThucHien.Value.Minute <= 59
                                                                                ).Select(c => c.NhanVienXacNhanThucHien.User.HoTen).ToList(),
                           BSThucHienChiTietTruas = s.NoiTruPhieuDieuTriChiTietYLenhs.Where(z => z.ThoiDiemXacNhanThucHien != null
                                                                                && z.XacNhanThucHien == true
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour >= 8 && z.ThoiDiemXacNhanThucHien.Value.Minute >= 0
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour <= 14 && z.ThoiDiemXacNhanThucHien.Value.Minute <= 59
                                                                                ).Select(c => c.NhanVienXacNhanThucHien.User.HoTen).ToList(),
                           BSThucHienChiTietChieus = s.NoiTruPhieuDieuTriChiTietYLenhs.Where(z => z.ThoiDiemXacNhanThucHien != null
                                                                                && z.XacNhanThucHien == true
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour >= 15 && z.ThoiDiemXacNhanThucHien.Value.Minute >= 0
                                                                                && z.ThoiDiemXacNhanThucHien.Value.Hour <= 20 && z.ThoiDiemXacNhanThucHien.Value.Minute <= 59
                                                                                ).Select(c => c.NhanVienXacNhanThucHien.User.HoTen).ToList(),
                           BSThucHienChiTietTois = s.NoiTruPhieuDieuTriChiTietYLenhs.Where(z => z.ThoiDiemXacNhanThucHien != null
                                                                                && z.XacNhanThucHien == true
                                                                                && (z.ThoiDiemXacNhanThucHien.Value.Hour >= 21 && z.ThoiDiemXacNhanThucHien.Value.Minute >= 0
                                                                                  || z.ThoiDiemXacNhanThucHien.Value.Hour <= 2 && z.ThoiDiemXacNhanThucHien.Value.Minute <= 59)
                                                                                  ).Select(c => c.NhanVienXacNhanThucHien.User.HoTen).ToList()
                       }).ToList();

                var dataGroup = vatTus.GroupBy(x => new { x.TenVatTu, x.DonVi })
                   .Select(item => new ThongTinThuocVatTuPhieuDieuTriThucHien
                   {
                       TenVatTu = item.First().TenVatTu,
                       DonVi = item.First().DonVi,
                       SoLuong = item.Sum(x => x.SoLuong),
                       TongSo = item.Sum(x => x.SoLuong),
                       SLDungSang = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungSang),
                       SLDungTrua = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungTrua),
                       SLDungChieu = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungChieu),
                       SLDungToi = item.Where(z => z.ThoiDiemXacNhanThucHien != null).Sum(x => x.SLDungToi),
                       GhiChus = string.Join(",", item.Select(x => x.GhiChu)),
                       //BSChiDinhs = string.Join(",", item.Select(x => x.BSChiDinh).Distinct()),
                       BSChiDinhs = item.First().BSChiDinh,
                   }).ToList();
                if (dataGroup.Any())
                {
                    var rowConstant = 10; // số dòng mặc định theo yc
                    foreach (var item in dataGroup)
                    {
                        vatTu += "<tr style = 'border: 1px solid #020000;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                       + "<td style = 'border: 1px solid #020000;padding-left:3px;'>" + item.TenVatTu
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonVi
                                       + "<td style = 'border: 1px solid #020000;text-align:center;'>" + item.SoLuong
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.CachDung
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SLDungTrua.FloatToStringFraction()
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SLDungChieu.FloatToStringFraction()
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SLDungToi.FloatToStringFraction()
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SLDungSang.FloatToStringFraction()
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px; word-break: break-word;'>" + item.BSChiDinhs
                                       + "</tr>";
                        STT++;
                    }
                    if (dataGroup.Count < rowConstant)
                    {
                        for (int i = 0; i < rowConstant - dataGroup.Count; i++)
                        {
                            vatTu += "<tr style = 'border: 1px solid #020000;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                       + "<td style = 'border: 1px solid #020000;padding-left:3px;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align:center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px; word-break: break-word;'>" + "&nbsp;"
                                       + "</tr>";
                            STT++;
                        }
                    }
                }
                var BSSang = string.Join(",", vatTus.SelectMany(z => z.BSThucHienChiTietSangs.Select(c => c)).Distinct());
                if (!string.IsNullOrEmpty(BSSang))
                {
                    BSSang = BSSang.Split(',').First();
                }
                var BSTrua = string.Join(",", vatTus.SelectMany(z => z.BSThucHienChiTietTruas.Select(c => c)).Distinct());
                if (!string.IsNullOrEmpty(BSTrua))
                {
                    BSTrua = BSTrua.Split(',').First();
                }
                var BSChieu = string.Join(",", vatTus.SelectMany(z => z.BSThucHienChiTietChieus.Select(c => c)).Distinct());
                if (!string.IsNullOrEmpty(BSChieu))
                {
                    BSChieu = BSChieu.Split(',').First();
                }
                var BSToi = string.Join(",", vatTus.SelectMany(z => z.BSThucHienChiTietTois.Select(c => c)).Distinct());
                if (!string.IsNullOrEmpty(BSToi))
                {
                    BSToi = BSToi.Split(',').First();
                }

                var tongSLVatTu = dataGroup.Count();
                vatTu += "<tr style = 'border: 1px solid #020000;'>"
                                      + "<td colspan='3' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Tổng số VTYT dùng</b>"
                                      + "<td colspan='7' style = 'border: 1px solid #020000;text-align: left;'>" + "<b>" + tongSLVatTu + "</b>"
                                      + "</tr>"
                                      + "<tr>"
                                      + "<td colspan='5' style = 'border: 1px solid #020000;padding-left:3px;'>" + "<b>Người thực hiện (Điều dưỡng)</b>"
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + BSTrua
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + BSChieu
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + BSToi
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + BSSang
                                      + "<td style = 'border: 1px solid #020000;text-align: left;padding-left:3px;'>" + "&nbsp;"
                                      + "</tr>"
                                      ;
                var data = new DataThuocVatTuPhieuDieuTri
                {
                    LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    MaTN = thongTin.MaTN,
                    SoBA = thongTin.SoBA,
                    MaBN = thongTin.MaBN,
                    KhoaPhong = thongTin.KhoaPhong,
                    HoTenNguoiBenh = thongTin.HoTenNguoiBenh,
                    Buong = thongTin.Buong,
                    VatTu = vatTu,
                    Tuoi = thongTin.Tuoi,
                    Gioi = thongTin.Gioi,
                    SoGiuong = thongTin.SoGiuong,
                    NgayVaoVien = thongTin.NgayVaoVien,
                    ChanDoan = noiTru.ChanDoanChinhICD != null ? noiTru.ChanDoanChinhICD.Ma + " - " + noiTru.ChanDoanChinhICD.TenTiengViet : "",
                    NgayThangNam = noiTru?.NgayDieuTri.ApplyFormatDate(),
                    NgayDieuTri = ngayDieuTri.NgayDieuTri.ApplyFormatNgayThangNam()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(phieuVatTu.Body, data);
            }
            return content;

        }

        public async Task<string> HoanTraDuocPhamTuBenhNhan(YeuCauTraDuocPhamTuBenhNhanChiTietVo yeuCauTraDuocPham)
        {
            var noiTruChiDinh = _noiTruChiDinhDuocPhamRepository.GetById(yeuCauTraDuocPham.Id, s => s
                                                .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                                                .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.NoiTruChiDinhDuocPham)
                                                .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.XuatKhoDuocPhamChiTiet).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTris).ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
                                                .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.XuatKhoDuocPhamChiTiet).ThenInclude(p => p.XuatKhoDuocPham).ThenInclude(p => p.KhoDuocPhamXuat))
                                                ;

            if (yeuCauTraDuocPham.LaDichTruyen)
            {
                foreach (var item in noiTruChiDinh.YeuCauDuocPhamBenhViens.Where(p => p.LaDichTruyen == true))
                {
                    var khoXuatId = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId;
                    //var loaiKho = item.KhoLinh.Where(p => p.Id == khoXuatId).First().LoaiKho;
                    var loaiKho = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.LoaiKho;
                    var traVeTuTruc = false;
                    var yc = item.YeuCauTraDuocPhamTuBenhNhanChiTiets.FirstOrDefault(p => p.YeuCauTraDuocPhamTuBenhNhanId == null);
                    if (loaiKho == EnumLoaiKhoDuocPham.KhoLe)
                    {
                        traVeTuTruc = true;
                        yc = null;
                    }
                    var soLuongTra = yeuCauTraDuocPham.YeuCauDuocPhamBenhViens.First(p => p.YeuCauDuocPhamBenhVienId == item.Id).SoLuongTra;
                    if (yc == null)
                    {
                        if (soLuongTra > 0)
                        {
                            if (!traVeTuTruc)
                            {
                                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả                                
                                if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && item.KhongTinhPhi != true)
                                {
                                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                                }
                                if (item.SoLuongDaTra == null)
                                {
                                    item.SoLuongDaTra = 0;
                                }
                                item.SoLuongDaTra += soLuongTra;
                                item.SoLuong -= soLuongTra.Value;
                                item.NoiTruChiDinhDuocPham.SoLuong -= soLuongTra.Value;

                                if (item.SoLuong < 0)
                                {
                                    throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                                }

                                //end update BVHD-3411
                                var ycHoanTraDuocPhamNew = new YeuCauTraDuocPhamTuBenhNhanChiTiet
                                {
                                    DuocPhamBenhVienId = yeuCauTraDuocPham.DuocPhamBenhVienId,
                                    LaDuocPhamBHYT = yeuCauTraDuocPham.LaDuocPhamBHYT,
                                    SoLuongTra = soLuongTra ?? 0,
                                    KhoTraId = khoXuatId,
                                    TraVeTuTruc = traVeTuTruc,
                                    NgayYeuCau = yeuCauTraDuocPham.NgayYeuCau,
                                    NhanVienYeuCauId = yeuCauTraDuocPham.NhanVienYeuCauId
                                };
                                item.YeuCauTraDuocPhamTuBenhNhanChiTiets.Add(ycHoanTraDuocPhamNew);
                            }
                            else
                            {
                                var xuatKhoDpViTris = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.ToList();
                                var xuatKhoDuocPhamViTriHoanTras = new List<XuatKhoDuocPhamChiTietViTri>();
                                if (item.SoLuongDaTra == null)
                                {
                                    item.SoLuongDaTra = 0;
                                }
                                item.SoLuongDaTra += soLuongTra;
                                item.SoLuong -= soLuongTra.Value;
                                item.NoiTruChiDinhDuocPham.SoLuong -= soLuongTra.Value;
                                foreach (var thongTinXuat in xuatKhoDpViTris)
                                {
                                    thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= soLuongTra.Value;
                                }
                                foreach (var vitri in xuatKhoDpViTris)
                                {
                                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                                    {
                                        XuatKhoDuocPhamChiTietId = vitri.XuatKhoDuocPhamChiTietId,
                                        NhapKhoDuocPhamChiTietId = vitri.NhapKhoDuocPhamChiTietId,
                                        SoLuongXuat = -soLuongTra.Value,
                                        NgayXuat = DateTime.Now,
                                        GhiChu = "Hoàn trả dịch truyền"
                                    };
                                    vitri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (traVeTuTruc)
                        {
                            break;
                        }
                        //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                        if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && item.KhongTinhPhi != true)
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                        }
                        var soLuongThayDoi = soLuongTra.GetValueOrDefault() - yc.SoLuongTra;
                        if (item.SoLuongDaTra == null)
                        {
                            item.SoLuongDaTra = 0;
                        }
                        item.SoLuongDaTra += soLuongThayDoi;
                        item.SoLuong -= soLuongThayDoi;
                        item.NoiTruChiDinhDuocPham.SoLuong -= soLuongThayDoi;
                        if (item.SoLuong < 0)
                        {
                            throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                        }
                        //end update BVHD-3411
                        if (soLuongTra > 0) // Cập nhật
                        {
                            yc.NhanVienYeuCauId = yeuCauTraDuocPham.NhanVienYeuCauId;
                            yc.NgayYeuCau = yeuCauTraDuocPham.NgayYeuCau;
                            yc.SoLuongTra = soLuongTra.GetValueOrDefault();
                        }
                        else // Xóa
                        {
                            yc.WillDelete = true;
                        }
                    }
                }
            }
            else
            {
                var lstThuoc = noiTruChiDinh.YeuCauDuocPhamBenhViens.Where(p => p.LaDichTruyen != true).ToList();
                foreach (var item in lstThuoc)
                {
                    var khoXuatId = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId;
                    var loaiKho = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.LoaiKho;
                    var traVeTuTruc = false;

                    var yc = item.YeuCauTraDuocPhamTuBenhNhanChiTiets.FirstOrDefault(p => p.YeuCauTraDuocPhamTuBenhNhanId == null);
                    if (loaiKho == EnumLoaiKhoDuocPham.KhoLe)
                    {
                        traVeTuTruc = true;
                        yc = null;
                    }
                    var soLuongTra = yeuCauTraDuocPham.YeuCauDuocPhamBenhViens.First(p => p.YeuCauDuocPhamBenhVienId == item.Id).SoLuongTra;
                    if (yc == null)
                    {
                        if (soLuongTra > 0)
                        {
                            if (!traVeTuTruc)
                            {
                                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                                if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && item.KhongTinhPhi != true)
                                {
                                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                                }
                                if (item.SoLuongDaTra == null)
                                {
                                    item.SoLuongDaTra = 0;
                                }
                                item.SoLuongDaTra += soLuongTra;
                                item.SoLuong -= soLuongTra.Value;
                                item.NoiTruChiDinhDuocPham.SoLuong -= soLuongTra.Value;
                                if (item.SoLuong < 0)
                                {
                                    throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                                }
                                //end update BVHD-3411
                                var ycHoanTraDuocPhamNew = new YeuCauTraDuocPhamTuBenhNhanChiTiet
                                {
                                    DuocPhamBenhVienId = yeuCauTraDuocPham.DuocPhamBenhVienId,
                                    LaDuocPhamBHYT = yeuCauTraDuocPham.LaDuocPhamBHYT,
                                    SoLuongTra = soLuongTra ?? 0,
                                    KhoTraId = khoXuatId,
                                    TraVeTuTruc = traVeTuTruc,
                                    NgayYeuCau = yeuCauTraDuocPham.NgayYeuCau,
                                    NhanVienYeuCauId = yeuCauTraDuocPham.NhanVienYeuCauId
                                };
                                item.YeuCauTraDuocPhamTuBenhNhanChiTiets.Add(ycHoanTraDuocPhamNew);
                            }
                            else
                            {
                                var xuatKhoDpViTris = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.ToList();
                                var xuatKhoDuocPhamViTriHoanTras = new List<XuatKhoDuocPhamChiTietViTri>();
                                if (item.SoLuongDaTra == null)
                                {
                                    item.SoLuongDaTra = 0;
                                }
                                item.SoLuongDaTra += soLuongTra;
                                item.SoLuong -= soLuongTra.Value;
                                item.NoiTruChiDinhDuocPham.SoLuong -= soLuongTra.Value;
                                foreach (var thongTinXuat in xuatKhoDpViTris)
                                {
                                    thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= soLuongTra.Value;
                                }
                                foreach (var vitri in xuatKhoDpViTris)
                                {
                                    var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                                    {
                                        XuatKhoDuocPhamChiTietId = vitri.XuatKhoDuocPhamChiTietId,
                                        NhapKhoDuocPhamChiTietId = vitri.NhapKhoDuocPhamChiTietId,
                                        SoLuongXuat = -soLuongTra.Value,
                                        NgayXuat = DateTime.Now,
                                        //GhiChu = item.NoiTruChiDinhDuocPham.TrangThai.GetDescription()
                                        GhiChu = "Hoàn trả thuốc"
                                    };
                                    vitri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (traVeTuTruc)
                        {
                            break;
                        }
                        //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                        if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && item.KhongTinhPhi != true)
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                        }
                        var soLuongThayDoi = soLuongTra.GetValueOrDefault() - yc.SoLuongTra;
                        if (item.SoLuongDaTra == null)
                        {
                            item.SoLuongDaTra = 0;
                        }
                        item.SoLuongDaTra += soLuongThayDoi;
                        item.SoLuong -= soLuongThayDoi;
                        item.NoiTruChiDinhDuocPham.SoLuong -= soLuongThayDoi;
                        if (item.SoLuong < 0)
                        {
                            throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                        }
                        //end update BVHD-3411
                        if (soLuongTra > 0) // Cập nhật
                        {
                            yc.NhanVienYeuCauId = yeuCauTraDuocPham.NhanVienYeuCauId;
                            yc.NgayYeuCau = yeuCauTraDuocPham.NgayYeuCau;
                            yc.SoLuongTra = soLuongTra.GetValueOrDefault();
                        }
                        else // Xóa
                        {
                            yc.WillDelete = true;
                        }
                    }

                }
            }
            _noiTruChiDinhDuocPhamRepository.Update(noiTruChiDinh);
            return string.Empty;
        }

        //public async Task<ThongTinHoanTraThuocVo> GetThongTinHoanTraThuocVo(HoanTraThuocVo hoanTraThuocVo)
        public ThongTinHoanTraThuocVo GetThongTinHoanTraThuocVo(HoanTraThuocVo hoanTraThuocVo)

        {
            var nhanVienDangNhap = _nhanVienRepository.TableNoTracking.Where(p => p.Id == _userAgentHelper.GetCurrentUserId()).Select(p => p.User.HoTen).FirstOrDefault();

            var noitruChiDinhDuocPham = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Include(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.YeuCauTraDuocPhamTuBenhNhanChiTiets).ThenInclude(z => z.NhanVienYeuCau).ThenInclude(z => z.User)
                .Include(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.KhoLinh)
                .Where(p => p.Id == hoanTraThuocVo.NoiTruChiDinhDuocPhamId && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && p.YeuCauDuocPhamBenhViens.All(z => z.KhoLinh.LoaiKho == hoanTraThuocVo.LoaiKho) && p.LaDichTruyen == hoanTraThuocVo.LaDichTruyen);
            var yeuCauDuocPhamBenhVien = noitruChiDinhDuocPham.FirstOrDefault()?.YeuCauDuocPhamBenhViens.FirstOrDefault();
            var yeuCauTraDuocPhamTuBenhNhanChiTiet = yeuCauDuocPhamBenhVien != null && yeuCauDuocPhamBenhVien.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any() ? yeuCauDuocPhamBenhVien.YeuCauTraDuocPhamTuBenhNhanChiTiets.FirstOrDefault() : null;
            var thongTinHoanTraThuocVo = noitruChiDinhDuocPham
                .Select(s => new ThongTinHoanTraThuocVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    Ten = s.Ten,
                    TenKho = yeuCauDuocPhamBenhVien != null ? yeuCauDuocPhamBenhVien.KhoLinh.Ten : "",
                    NhanVienYeuCauId = yeuCauTraDuocPhamTuBenhNhanChiTiet != null && yeuCauTraDuocPhamTuBenhNhanChiTiet.NhanVienYeuCauId != 0 ? yeuCauTraDuocPhamTuBenhNhanChiTiet.NhanVienYeuCauId : _userAgentHelper.GetCurrentUserId(),
                    TenNhanVienYeuCau = yeuCauTraDuocPhamTuBenhNhanChiTiet != null && yeuCauTraDuocPhamTuBenhNhanChiTiet.NhanVienYeuCau != null ? yeuCauTraDuocPhamTuBenhNhanChiTiet.NhanVienYeuCau.User.HoTen : nhanVienDangNhap,
                    LaDichTruyen = s.LaDichTruyen,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    NgayYeuCau = s.YeuCauDuocPhamBenhViens.First().LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu ? (DateTime?)null : s.YeuCauDuocPhamBenhViens.First().YeuCauTraDuocPhamTuBenhNhanChiTiets.FirstOrDefault(c => c.YeuCauTraDuocPhamTuBenhNhanId == null).NgayYeuCau,
                    YeuCauDuocPhamBenhViens = s.YeuCauDuocPhamBenhViens.Select(o => new ThongTinHoanTraThuocChiTietVo
                    {
                        YeuCauDuocPhamBenhVienId = o.Id,
                        SoLuong = o.SoLuong,
                        DonGiaNhap = o.DonGiaNhap,
                        DonGia = o.DonGiaBan,
                        TiLeTheoThapGia = o.TiLeTheoThapGia,
                        VAT = o.VAT,
                        KhongTinhPhi = o.KhongTinhPhi,
                        //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                        SoLuongDaTra = o.SoLuongDaTra.GetValueOrDefault() - (o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? o.YeuCauTraDuocPhamTuBenhNhanChiTiets.Where(x => x.LaDuocPhamBHYT == s.LaDuocPhamBHYT && x.YeuCauTraDuocPhamTuBenhNhanId == null).Select(z => z.SoLuongTra).FirstOrDefault() : 0),
                        SoLuongTra = o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? o.YeuCauTraDuocPhamTuBenhNhanChiTiets.Where(x => x.LaDuocPhamBHYT == s.LaDuocPhamBHYT && x.YeuCauTraDuocPhamTuBenhNhanId == null).Select(z => z.SoLuongTra).FirstOrDefault() : 0,
                    }).ToList(),
                })
                .FirstOrDefault();
            return thongTinHoanTraThuocVo;
            //var loaiKho = _khoRepository.TableNoTracking.Where(p => p.Id == hoanTraThuocVo.KhoId).Select(p => p.LoaiKho).First();
            //var tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == hoanTraThuocVo.KhoId).Select(p => p.Ten).First();
            //var nhanVienDangNhap = _nhanVienRepository.TableNoTracking.Where(p => p.Id == _userAgentHelper.GetCurrentUserId()).Select(p => p.User.HoTen).FirstOrDefault();
            //var yeuCauDuocPhamBenhVienId = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(p => p.NoiTruChiDinhDuocPhamId == hoanTraThuocVo.NoiTruChiDinhDuocPhamId).Select(p => p.Id).FirstOrDefault();

            //var nhanVienYeuCauId = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking.Where(p => p.YeuCauDuocPhamBenhVienId == yeuCauDuocPhamBenhVienId).Select(p => p.NhanVienYeuCauId).FirstOrDefault();

            //var tenNhanVienYeuCau = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking.Where(p => p.YeuCauDuocPhamBenhVienId == yeuCauDuocPhamBenhVienId).Select(p => p.NhanVienYeuCau.User.HoTen).FirstOrDefault();

            //var thongTinHoanTraThuocVo = await _noiTruChiDinhDuocPhamRepository.TableNoTracking.Where(p => p.Id == hoanTraThuocVo.NoiTruChiDinhDuocPhamId && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && p.YeuCauDuocPhamBenhViens.All(z => z.KhoLinh.LoaiKho == loaiKho) && p.LaDichTruyen == hoanTraThuocVo.LaDichTruyen)
            //    .Select(s => new ThongTinHoanTraThuocVo
            //    {
            //        Id = s.Id,
            //        DuocPhamBenhVienId = s.DuocPhamBenhVienId,
            //        Ten = s.Ten,
            //        TenKho = tenKho,
            //        NhanVienYeuCauId = nhanVienYeuCauId != 0 ? nhanVienYeuCauId : _userAgentHelper.GetCurrentUserId(),
            //        TenNhanVienYeuCau = !string.IsNullOrEmpty(tenNhanVienYeuCau) ? tenNhanVienYeuCau : nhanVienDangNhap,
            //        LaDichTruyen = s.LaDichTruyen,
            //        YeuCauTiepNhanId = s.YeuCauTiepNhanId,
            //        NgayYeuCau = s.YeuCauDuocPhamBenhViens.First().LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu ? (DateTime?)null : s.YeuCauDuocPhamBenhViens.First().YeuCauTraDuocPhamTuBenhNhanChiTiets.FirstOrDefault(c => c.YeuCauTraDuocPhamTuBenhNhanId == null).NgayYeuCau,
            //        YeuCauDuocPhamBenhViens = s.YeuCauDuocPhamBenhViens.Select(o => new ThongTinHoanTraThuocChiTietVo
            //        {
            //            YeuCauDuocPhamBenhVienId = o.Id,
            //            SoLuong = o.SoLuong,
            //            DonGiaNhap = o.DonGiaNhap,
            //            DonGia = o.DonGiaBan,
            //            TiLeTheoThapGia = o.TiLeTheoThapGia,
            //            VAT = o.VAT,
            //            KhongTinhPhi = o.KhongTinhPhi,
            //            SoLuongDaTra = o.SoLuongDaTra.GetValueOrDefault(),
            //            SoLuongTra = o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? o.YeuCauTraDuocPhamTuBenhNhanChiTiets.Where(x => x.LaDuocPhamBHYT == s.LaDuocPhamBHYT && x.YeuCauTraDuocPhamTuBenhNhanId == null).Select(z => z.SoLuongTra).FirstOrDefault() : 0,
            //        }).ToList(),
            //    })
            //    .FirstOrDefaultAsync();
            //return thongTinHoanTraThuocVo;

        }

        public async Task<GridDataSource> GetDataForGridDanhSachThuocHoanTra(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var noiTruChiDinhDuocPhamId = long.Parse(queryObj[0]);
            var laDuocPhamBHYT = bool.Parse(queryObj[1]);
            var laTuTruc = bool.Parse(queryObj[2]);
            if (laTuTruc)
            {
                var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPhamId == noiTruChiDinhDuocPhamId
                && p.LaDuocPhamBHYT == laDuocPhamBHYT)
                .Select(s => new YeuCauTraDuocPhamTuBenhNhanChiTietGridVo
                {
                    Id = s.Id,
                    NgayTra = s.NgayYeuCau,
                    SoLuongTra = ((double?)s.SoLuongTra).FloatToStringFraction(),
                    NhanVienTra = s.NhanVienYeuCau.User.HoTen,
                    SoPhieu = s.YeuCauTraDuocPhamTuBenhNhan.SoPhieu,
                    DuocDuyet = true,
                    NgayTao = s.CreatedOn,
                });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            else
            {
                var query = _yeuCauTraDuocPhamTuBenhNhanChiTietRepository.TableNoTracking
                                .Where(p => p.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPhamId == noiTruChiDinhDuocPhamId
                                && p.LaDuocPhamBHYT == laDuocPhamBHYT && p.YeuCauTraDuocPhamTuBenhNhanId != null)
                                .Select(s => new YeuCauTraDuocPhamTuBenhNhanChiTietGridVo
                                {
                                    Id = s.Id,
                                    NgayTra = s.NgayYeuCau,
                                    SoLuongTra = ((double?)s.SoLuongTra).FloatToStringFraction(),
                                    NhanVienTra = s.NhanVienYeuCau.User.HoTen,
                                    SoPhieu = s.YeuCauTraDuocPhamTuBenhNhan.SoPhieu,
                                    DuocDuyet = s.YeuCauTraDuocPhamTuBenhNhan.DuocDuyet,
                                    NgayTao = s.CreatedOn,
                                });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
        }

        public async Task<GridDataSource> GetTotalPageForGridDanhSachThuocHoanTra(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<bool> KiemTraNgayTra(DateTime? ngayTra)
        {
            if (ngayTra == null)
            {
                return true;
            }
            var ngayHienTai = DateTime.Now.Date;
            if (ngayTra.Value.Date > ngayHienTai)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CheckSoLuongTra(double soLuong, double soLuongDaTra, double? soLuongTra)
        {
            if (soLuongTra == null)
            {
                return true;
            }
            //var soLuongCoTheTra = soLuong - soLuongDaTra;
            if (soLuong < soLuongTra.Value)
            {
                return false;
            }
            return true;
        }
        public GridDataSource GetDataForGridDanhSachThuocNgoai(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var lstMaHoatChat = _noiTruPhieuDieuTriTuVanThuocRepository.TableNoTracking
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

            var query = _noiTruPhieuDieuTriTuVanThuocRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId
                          && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen != true)
                .Select(s => new PhieuDieuTriThuocNgoaiBenhVienGridVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamId,
                    Ma = s.MaHoatChat,
                    Ten = s.Ten,
                    HoatChat = s.HoatChat,
                    HamLuong = s.HamLuong,
                    DVT = s.DonViTinh.Ten,
                    ThoiGianDungSang = s.ThoiGianDungSang,
                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                    ThoiGianDungToi = s.ThoiGianDungToi,
                    DungSang = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                    DungTrua = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                    DungChieu = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                    DungToi = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                    ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                    SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                    TenDuongDung = s.DuongDung.Ten,
                    SoLuong = s.SoLuong,
                    DiUngThuoc = s.NoiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                    TuongTacThuoc = GetTuongTac(s.MaHoatChat, lstMaHoatChat, lstADR),
                    GhiChu = s.GhiChu,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    KhuVuc = 2,

                    //BVHD-3905
                    TiLeThanhToanBHYT = s.DuocPham.DuocPhamBenhVien.TiLeThanhToanBHYT
                });

            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public GridDataSource GetDataForGridDanhSachDichTruyenNgoai(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var lstMaHoatChat = _noiTruPhieuDieuTriTuVanThuocRepository.TableNoTracking
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

            var query = _noiTruPhieuDieuTriTuVanThuocRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId
                          && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LaDichTruyen == true)
                .Select(s => new PhieuDieuTriThuocNgoaiBenhVienGridVo
                {
                    Id = s.Id,
                    DuocPhamBenhVienId = s.DuocPhamId,
                    Ma = s.MaHoatChat,
                    Ten = s.Ten,
                    HoatChat = s.HoatChat,
                    HamLuong = s.HamLuong,
                    DVT = s.DonViTinh.Ten,
                    SoLanDungTrongNgay = s.SoLanDungTrongNgay,
                    TenDuongDung = s.DuongDung.Ten,
                    SoLuong = s.SoLuong,
                    DiUngThuoc = s.NoiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                    TuongTacThuoc = GetTuongTac(s.MaHoatChat, lstMaHoatChat, lstADR),
                    GhiChu = s.GhiChu,
                    LaDichTruyen = s.LaDichTruyen,
                    TocDoTruyen = s.TocDoTruyen,
                    DonViTocDoTruyen = s.DonViTocDoTruyen,
                    CachGioTruyenDich = s.CachGioTruyenDich,
                    ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    KhuVuc = 2,
                    TheTich = s.TheTich,

                    //BVHD-3905
                    TiLeThanhToanBHYT = s.DuocPham.DuocPhamBenhVien.TiLeThanhToanBHYT
                });

            var lstQuery = query.ToList();
            double seconds = 3600;
            for (int i = 0; i < lstQuery.Count(); i++)
            {
                lstQuery[i].STT = i + 1;
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
            }
            var countTask = queryInfo.LazyLoadPage == true ? 0 : lstQuery.Count();
            var queryTask = lstQuery.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task ThemThuocNgoaiBenhVien(ThuocBenhVienVo donThuocChiTiet)
        {
            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamBenhVienId);
            var noiTruPhieuDieuTriTuVanThuoc = new NoiTruPhieuDieuTriTuVanThuoc
            {
                YeuCauTiepNhanId = donThuocChiTiet.YeuCauTiepNhanId,
                NoiTruPhieuDieuTriId = donThuocChiTiet.PhieuDieuTriHienTaiId.Value,
                DuocPhamId = donThuocChiTiet.DuocPhamBenhVienId,
                Ten = duocPham.Ten,
                LaDuocPhamBenhVien = false,
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
                ChuYDePhong = duocPham.ChuYDePhong,
                SoLuong = donThuocChiTiet.SoLuong,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay,
                DungSang = donThuocChiTiet.DungSang,
                DungTrua = donThuocChiTiet.DungTrua,
                DungChieu = donThuocChiTiet.DungChieu,
                DungToi = donThuocChiTiet.DungToi,
                ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich,
                TheTich = duocPham.TheTich,
                GhiChu = donThuocChiTiet.GhiChu,
            };
            await _noiTruPhieuDieuTriTuVanThuocRepository.AddAsync(noiTruPhieuDieuTriTuVanThuoc);
        }
        public async Task CapNhatThuocNgoaiBenhVien(ThuocBenhVienVo donThuocChiTiet)
        {
            var noiTruPhieuDieuTriTuVanThuoc = _noiTruPhieuDieuTriTuVanThuocRepository.GetById(donThuocChiTiet.Id);
            noiTruPhieuDieuTriTuVanThuoc.SoLuong = donThuocChiTiet.SoLuong;
            noiTruPhieuDieuTriTuVanThuoc.DungSang = donThuocChiTiet.DungSang;
            noiTruPhieuDieuTriTuVanThuoc.DungTrua = donThuocChiTiet.DungTrua;
            noiTruPhieuDieuTriTuVanThuoc.DungChieu = donThuocChiTiet.DungChieu;
            noiTruPhieuDieuTriTuVanThuoc.DungToi = donThuocChiTiet.DungToi;
            noiTruPhieuDieuTriTuVanThuoc.ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang;
            noiTruPhieuDieuTriTuVanThuoc.ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua;
            noiTruPhieuDieuTriTuVanThuoc.ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu;
            noiTruPhieuDieuTriTuVanThuoc.ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi;
            noiTruPhieuDieuTriTuVanThuoc.GhiChu = donThuocChiTiet.GhiChu;
            noiTruPhieuDieuTriTuVanThuoc.LaDichTruyen = donThuocChiTiet.LaDichTruyen;
            noiTruPhieuDieuTriTuVanThuoc.TocDoTruyen = donThuocChiTiet.TocDoTruyen;
            noiTruPhieuDieuTriTuVanThuoc.ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen;
            noiTruPhieuDieuTriTuVanThuoc.TheTich = donThuocChiTiet.TheTich;
            noiTruPhieuDieuTriTuVanThuoc.CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich;
            noiTruPhieuDieuTriTuVanThuoc.SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay;
            noiTruPhieuDieuTriTuVanThuoc.DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen;
            await _noiTruPhieuDieuTriTuVanThuocRepository.UpdateAsync(noiTruPhieuDieuTriTuVanThuoc);
        }
        public async Task XoaThuocNgoaiBenhVien(long noiTruPhieuDieuTriTuVanThuocId)
        {
            var noiTruPhieuDieuTriTuVanThuoc = _noiTruPhieuDieuTriTuVanThuocRepository.GetById(noiTruPhieuDieuTriTuVanThuocId);
            await _noiTruPhieuDieuTriTuVanThuocRepository.DeleteAsync(noiTruPhieuDieuTriTuVanThuoc);
        }


        private DataBenhNhan ThongTinBenhNhanPhieuThuoc(long yeuCauTiepNhanId)
        {
            var thongTinBenhNhanPhieuThuoc = _yeuCauTiepNhanRepository.TableNoTracking
                           .Where(s => s.Id == yeuCauTiepNhanId)
                           .Select(s => new DataBenhNhan
                           {
                               MaTN = s.MaYeuCauTiepNhan,
                               Id = s.BenhNhan.Id,
                               HoTen = s.HoTen,
                               GioiTinh = s.GioiTinh.GetDescription(),
                               NamSinh = s.NamSinh,
                               NamSinhDayDu = DateHelper.DOBFormat(s.NgaySinh, s.ThangSinh, s.NamSinh),
                               Tuoi = s.NamSinh != null ? (DateTime.Now.Year - s.NamSinh) : null,
                               DiaChi = s.DiaChiDayDu,
                               SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                               MaBN = s.BenhNhan.MaBN,
                               SoThang = CalculateHelper.TinhTongSoThangCuaTuoi(s.NgaySinh, s.ThangSinh, s.NamSinh),
                               BHYTMaSoThe = s.BHYTMaSoThe,
                               ChuanDoan = s.NoiTruBenhAn != null && s.NoiTruBenhAn.ChanDoanChinhRaVienICD != null ? (s.NoiTruBenhAn.ChanDoanChinhRaVienICD.Ma + " - " + s.NoiTruBenhAn.ChanDoanChinhRaVienICD.TenTiengViet + (!string.IsNullOrEmpty(s.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu) ? " (" + s.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu + ")" : "")) : "",
                           }).FirstOrDefault();
            return thongTinBenhNhanPhieuThuoc;
        }

        public string InNoiTruPhieuDieuTriTuVanThuoc(InNoiTruPhieuDieuTriTuVanThuoc inNoiTruPhieuDieuTriTuVanThuoc)
        {
            var content = string.Empty;
            var infoBN = ThongTinBenhNhanPhieuThuoc(inNoiTruPhieuDieuTriTuVanThuoc.YeuCauTiepNhanId);
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("NoiTruPhieuDieuTriTuVanThuoc")).First();

            var noiTruPhieuDieuTriTuVanThuocs = _noiTruPhieuDieuTriTuVanThuocRepository.TableNoTracking
                               .Include(p => p.DonViTinh)
                               .Include(p => p.DuongDung)
                              .Include(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                              .Include(dtct => dtct.DuocPham).ThenInclude(dtct => dtct.DuocPhamBenhVien)
                              .Where(p => p.YeuCauTiepNhanId == inNoiTruPhieuDieuTriTuVanThuoc.YeuCauTiepNhanId && p.NoiTruPhieuDieuTriId == inNoiTruPhieuDieuTriTuVanThuoc.NoiTruPhieuDieuTriId).OrderBy(z => z.LaDichTruyen).ThenBy(z => z.Id).ToList();


            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaPhong = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == noiLamViecCurrentId).Select(p => p.KhoaPhong.Ten).FirstOrDefault();
            var resultThuoc = string.Empty;
            var sttThuoc = 0;

            var headerThuoc = string.Empty;
            var headerDichTruyen = string.Empty;

            if (noiTruPhieuDieuTriTuVanThuocs.Any())
            {
                var lstThoiGian = noiTruPhieuDieuTriTuVanThuocs.Select(s => new ThoiGianDungThuoc
                {
                    ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                }).ToList();
                for (int i = 0; i < noiTruPhieuDieuTriTuVanThuocs.Count; i++)
                {
                    if (noiTruPhieuDieuTriTuVanThuocs[i].LaDichTruyen != true)
                    {
                        var cd =
                             (noiTruPhieuDieuTriTuVanThuocs[i].DungSang != null
                                 ? "Sáng " + ((noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                                        NumberHelper.ChuyenSoRaText(Convert.ToDouble(noiTruPhieuDieuTriTuVanThuocs[i].DungSang), false) :
                                                       (Convert.ToDouble(noiTruPhieuDieuTriTuVanThuocs[i].DungSang) < 10 && Convert.ToInt32(noiTruPhieuDieuTriTuVanThuocs[i].DungSang) == noiTruPhieuDieuTriTuVanThuocs[i].DungSang) ? "0" + noiTruPhieuDieuTriTuVanThuocs[i].DungSang.FloatToStringFraction() + " " : noiTruPhieuDieuTriTuVanThuocs[i].DungSang.FloatToStringFraction() + " ") +
                                   (!string.IsNullOrEmpty(lstThoiGian[i].ThoiGianDungSangDisplay)
                                       ? " " + lstThoiGian[i].ThoiGianDungSangDisplay
                                       : "") + " " + noiTruPhieuDieuTriTuVanThuocs[i].DonViTinh?.Ten + ","
                                 : "") +
                             (noiTruPhieuDieuTriTuVanThuocs[i].DungTrua != null
                                 ? "Trưa " + ((noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                                        NumberHelper.ChuyenSoRaText(Convert.ToDouble(noiTruPhieuDieuTriTuVanThuocs[i].DungTrua), false) :
                                                       (Convert.ToDouble(noiTruPhieuDieuTriTuVanThuocs[i].DungTrua) < 10 && Convert.ToInt32(noiTruPhieuDieuTriTuVanThuocs[i].DungTrua) == noiTruPhieuDieuTriTuVanThuocs[i].DungTrua) ? "0" + noiTruPhieuDieuTriTuVanThuocs[i].DungTrua.FloatToStringFraction() + " " : noiTruPhieuDieuTriTuVanThuocs[i].DungTrua.FloatToStringFraction() + " ") +
                                   (!string.IsNullOrEmpty(lstThoiGian[i].ThoiGianDungTruaDisplay)
                                       ? " " + lstThoiGian[i].ThoiGianDungTruaDisplay
                                       : "") + " " + noiTruPhieuDieuTriTuVanThuocs[i].DonViTinh?.Ten + ","
                                 : "") +
                             (noiTruPhieuDieuTriTuVanThuocs[i].DungChieu != null
                                 ? "Chiều " + ((noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                                        NumberHelper.ChuyenSoRaText(Convert.ToDouble(noiTruPhieuDieuTriTuVanThuocs[i].DungChieu), false) :
                                                       (Convert.ToDouble(noiTruPhieuDieuTriTuVanThuocs[i].DungChieu) < 10 && Convert.ToInt32(noiTruPhieuDieuTriTuVanThuocs[i].DungChieu) == noiTruPhieuDieuTriTuVanThuocs[i].DungChieu) ? "0" + noiTruPhieuDieuTriTuVanThuocs[i].DungChieu.FloatToStringFraction() + " " : noiTruPhieuDieuTriTuVanThuocs[i].DungChieu.FloatToStringFraction() + " ") +
                                   (!string.IsNullOrEmpty(lstThoiGian[i].ThoiGianDungChieuDisplay)
                                       ? " " + lstThoiGian[i].ThoiGianDungChieuDisplay
                                       : "") + " " + noiTruPhieuDieuTriTuVanThuocs[i].DonViTinh?.Ten + ","
                                 : "") +
                             (noiTruPhieuDieuTriTuVanThuocs[i].DungToi != null
                                 ? "Tối " + ((noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                                        NumberHelper.ChuyenSoRaText(Convert.ToDouble(noiTruPhieuDieuTriTuVanThuocs[i].DungToi), false) :
                                                       (Convert.ToDouble(noiTruPhieuDieuTriTuVanThuocs[i].DungToi) < 10 && Convert.ToInt32(noiTruPhieuDieuTriTuVanThuocs[i].DungToi) == noiTruPhieuDieuTriTuVanThuocs[i].DungToi) ? "0" + noiTruPhieuDieuTriTuVanThuocs[i].DungToi.FloatToStringFraction() + " " : noiTruPhieuDieuTriTuVanThuocs[i].DungToi.FloatToStringFraction() + " ") +
                                   (!string.IsNullOrEmpty(lstThoiGian[i].ThoiGianDungToiDisplay)
                                       ? " " + lstThoiGian[i].ThoiGianDungToiDisplay
                                       : "") + " " + noiTruPhieuDieuTriTuVanThuocs[i].DonViTinh?.Ten + ","
                                 : "");

                        var cachDung = noiTruPhieuDieuTriTuVanThuocs[i].DuongDung?.Ten + " " + (!string.IsNullOrEmpty(cd) ? cd.Trim().Remove(cd.Trim().Length - 1) : "") + " " + noiTruPhieuDieuTriTuVanThuocs[i].GhiChu;

                        sttThuoc++;
                        resultThuoc += "<tr>";
                        resultThuoc += "<td rowspan='2' align='center'>" + sttThuoc + "</td>";
                        resultThuoc += "<td>" + _yeuCauKhamBenhService.FormatTenDuocPham(noiTruPhieuDieuTriTuVanThuocs[i].Ten, noiTruPhieuDieuTriTuVanThuocs[i].HoatChat, noiTruPhieuDieuTriTuVanThuocs[i].HamLuong, noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhomId) + "</td>";
                        resultThuoc += "<td rowspan='2' align='center'>" + _yeuCauKhamBenhService.FormatSoLuong(noiTruPhieuDieuTriTuVanThuocs[i].SoLuong, noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy) +
                                           noiTruPhieuDieuTriTuVanThuocs[i].DonViTinh?.Ten + "</td>";

                        resultThuoc += "</tr>";
                        resultThuoc += "<tr>";
                        resultThuoc += "<td><i>" + cachDung + "&nbsp;</i></td>";
                        resultThuoc += "</tr>";
                    }
                    else
                    {
                        double seconds = 3600;
                        var cd = string.Empty;
                        var thoiGianBatDauTruyen = noiTruPhieuDieuTriTuVanThuocs[i].ThoiGianBatDauTruyen;
                        if (thoiGianBatDauTruyen != null)
                        {
                            if (noiTruPhieuDieuTriTuVanThuocs[i].SoLanDungTrongNgay != null && noiTruPhieuDieuTriTuVanThuocs[i].CachGioTruyenDich != null)
                            {
                                for (int j = 0; j < noiTruPhieuDieuTriTuVanThuocs[i].SoLanDungTrongNgay; j++)
                                {
                                    var time = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                                    thoiGianBatDauTruyen += (int?)(noiTruPhieuDieuTriTuVanThuocs[i].CachGioTruyenDich * seconds);
                                    cd += time + "; ";
                                }
                            }
                            else
                            {
                                cd = thoiGianBatDauTruyen.Value.ConvertIntSecondsToTime12h();
                            }
                        }

                        var cachDung = noiTruPhieuDieuTriTuVanThuocs[i].DuongDung?.Ten + " " + (!string.IsNullOrEmpty(cd) ? "(" + cd.Trim().Remove(cd.Trim().Length - 1) + ")" : "") + " " + noiTruPhieuDieuTriTuVanThuocs[i].GhiChu;

                        sttThuoc++;
                        resultThuoc += "<tr>";
                        resultThuoc += "<td rowspan='2' align='center'>" + sttThuoc + "</td>";
                        //resultThuoc += "<td>" + noiTruPhieuDieuTriTuVanThuocs[i].HoatChat + "</span> " +
                        //                   "(<span style='text-transform: uppercase;'>" + noiTruPhieuDieuTriTuVanThuocs[i].Ten +
                        //                   "</span>) " + noiTruPhieuDieuTriTuVanThuocs[i].HamLuong + "</td>";
                        //resultThuoc += "<td rowspan='2' align='center'>" + noiTruPhieuDieuTriTuVanThuocs[i].SoLuong + " " +
                        //                   noiTruPhieuDieuTriTuVanThuocs[i].DonViTinh?.Ten + "</td>";
                        resultThuoc += "<td>" + _yeuCauKhamBenhService.FormatTenDuocPham(noiTruPhieuDieuTriTuVanThuocs[i].Ten, noiTruPhieuDieuTriTuVanThuocs[i].HoatChat, noiTruPhieuDieuTriTuVanThuocs[i].HamLuong, noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhomId) + "</td>";
                        resultThuoc += "<td rowspan='2' align='center'>" + _yeuCauKhamBenhService.FormatSoLuong(noiTruPhieuDieuTriTuVanThuocs[i].SoLuong, noiTruPhieuDieuTriTuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy) +
                                           noiTruPhieuDieuTriTuVanThuocs[i].DonViTinh?.Ten + "</td>";
                        resultThuoc += "</tr>";
                        resultThuoc += "<tr>";
                        resultThuoc += "<td><i>" + cachDung + "&nbsp;</i></td>";
                        resultThuoc += "</tr>";
                    }

                }
                if (!string.IsNullOrEmpty(resultThuoc))
                {
                    resultThuoc = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>TÊN THUỐC – HÀM LƯỢNG</th><th>SỐ LƯỢNG</th></tr></thead><tbody>" + resultThuoc + "</tbody></table>";
                    var data = new NoiTruPhieuDieuTriTuVanThuocData
                    {
                        TemplateDonThuoc = resultThuoc,
                        LogoUrl = inNoiTruPhieuDieuTriTuVanThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                        MaTN = infoBN.MaTN,
                        HoTen = infoBN.HoTen,
                        Tuoi = infoBN.Tuoi,
                        DiaChi = infoBN.DiaChi,
                        NamSinh = infoBN.NamSinh,
                        GioiTinh = infoBN.GioiTinh,
                        BacSiChiDinh = noiTruPhieuDieuTriTuVanThuocs?.Select(p => p.NhanVienChiDinh.User.HoTen).FirstOrDefault(),
                        LoiDan = infoBN.LoiDan,
                        MaBN = infoBN.MaBN,
                        SoDienThoai = infoBN.SoDienThoai,
                        CongKhoan = sttThuoc,
                        KhoaPhong = khoaPhong
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                }
            }
            return content;
        }

        public async Task CapNhatKhongTinhPhi(CapNhatKhongTinhPhi capNhatKhongTinhPhi, YeuCauTiepNhan yeuCauTiepNhan)
        {
            if (capNhatKhongTinhPhi.LaThuoc)
            {
                var noiTruChiDinhDuocPham = yeuCauTiepNhan.NoiTruChiDinhDuocPhams.First(p => p.Id == capNhatKhongTinhPhi.Id);
                foreach (var yeuCauDuocPhamBenhVien in noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens)
                {
                    yeuCauDuocPhamBenhVien.KhongTinhPhi = !capNhatKhongTinhPhi.KhongTinhPhi;
                }
            }
            else
            {
                var yeuCauVatTuBenhVien = yeuCauTiepNhan.YeuCauVatTuBenhViens.FirstOrDefault(p => p.Id == capNhatKhongTinhPhi.Id);
                yeuCauVatTuBenhVien.KhongTinhPhi = !capNhatKhongTinhPhi.KhongTinhPhi;
            }
        }

        public async Task XuLyXoaYLenhKhiXoaDichVuNoiTruAsync(EnumNhomGoiDichVu loaiDichVu, long yeuCauDichVuId)
        {

            var phieuDieuTri = await _noiTruBenhAnRepository.Table
                .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs)
                .Include(x => x.NoiTruPhieuDieuTriChiTietDienBiens)
                .Where(x => x.NoiTruPhieuDieuTriChiTietYLenhs.Any(a => (loaiDichVu == EnumNhomGoiDichVu.DichVuKyThuat && a.YeuCauDichVuKyThuatId == yeuCauDichVuId)
                                                                       || (loaiDichVu == EnumNhomGoiDichVu.DuocPham && a.NoiTruChiDinhDuocPhamId == yeuCauDichVuId)
                                                                       || (loaiDichVu == EnumNhomGoiDichVu.VatTuTieuHao && a.YeuCauVatTuBenhVienId == yeuCauDichVuId)
                                                                       || (loaiDichVu == EnumNhomGoiDichVu.TruyenMau && a.YeuCauTruyenMauId == yeuCauDichVuId)
                                                                       ))
                .FirstOrDefaultAsync();

            if (phieuDieuTri != null)
            {
                var chiTietYLenhTheoDichVus = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs
                    .Where(x => x.YeuCauDichVuKyThuatId == yeuCauDichVuId).ToList();
                foreach (var yLenh in chiTietYLenhTheoDichVus)
                {
                    yLenh.WillDelete = true;
                }

                var dienBienKhongCoYLenhs = phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.Where(x =>
                    phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Where(y => !y.WillDelete).All(a => a.GioYLenh != x.GioDienBien)).ToList();
                foreach (var dienBien in dienBienKhongCoYLenhs)
                {
                    dienBien.WillDelete = true;
                }
            }
        }

        public async Task CreateNewDateDieuTri(YeuCauTiepNhan yeuCauTiepNhan, long? khoaId, List<DateTime> dates)
        {
            var userLogin = _userAgentHelper.GetCurrentUserId();

            #region Cập nhật 26/12/2022 - đóng code check thấy ko sử dụng
            //var yctnNhapVien = BaseRepository.TableNoTracking
            //                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens)
            //                .Where(z => z.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId).FirstOrDefault();
            #endregion

            var noitruPhieuDieuTri = new NoiTruPhieuDieuTri();
            if (yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Any())
            {
                noitruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderByDescending(p => p.Id).FirstOrDefault();
            }
            else
            {
                noitruPhieuDieuTri = null;
            }
            var datesTemp = new List<DateTime>();
            foreach (var date in dates)
            {
                if (!yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Any(x => date == x.NgayDieuTri && (khoaId == null || khoaId == x.KhoaPhongDieuTriId)))
                {
                    datesTemp.Add(date.Date);
                }
            }
            dates = datesTemp;
            foreach (var date in dates)
            {
                if (yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien != null && date.Date < yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.Date)
                {
                    var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                    throw new Exception(string.Format(errMess, yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.ApplyFormatDate()));
                }
                //if (yctnNhapVien != null)
                //{
                //    if (date.Date < yctnNhapVien.YeuCauKhamBenhs.FirstOrDefault()?.YeuCauNhapViens.FirstOrDefault()?.ThoiDiemChiDinh.Date)
                //    {
                //        var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                //        throw new Exception(string.Format(errMess, yctnNhapVien.YeuCauKhamBenhs.FirstOrDefault()?.YeuCauNhapViens.FirstOrDefault()?.ThoiDiemChiDinh.ApplyFormatDate()));
                //    }
                //}
                //else
                //{
                //    if (yeuCauTiepNhan.YeuCauNhapVienId != null && date.Date < yeuCauTiepNhan.ThoiDiemTiepNhan.Date)
                //    {
                //        var errMess = _localizationService.GetResource("PhieuDieuTri.NgayDieuTri.NoValid");//không cho tạo các ngày điều trị trc khi nhập viện
                //        throw new Exception(string.Format(errMess, yeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate()));
                //    }
                //}
                long phongDieuTriId = 0;
                if (khoaId != null)
                {
                    phongDieuTriId = (long)khoaId;
                }
                else
                {
                    var lastNoiDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(o => (o.ThoiDiemVaoKhoa <= date || o.ThoiDiemVaoKhoa.Date == date.Date) && (o.ThoiDiemRaKhoa == null || o.ThoiDiemRaKhoa >= date || ((DateTime)o.ThoiDiemRaKhoa).Date == date.Date)).LastOrDefault();
                    phongDieuTriId = lastNoiDieuTri != null ? lastNoiDieuTri.KhoaPhongChuyenDenId : 0;
                }
                var noiTru = new NoiTruPhieuDieuTri
                {
                    NhanVienLapId = userLogin,
                    KhoaPhongDieuTriId = phongDieuTriId,
                    NgayDieuTri = date.Date,
                    ChanDoanChinhICDId = noitruPhieuDieuTri != null ? noitruPhieuDieuTri.ChanDoanChinhICDId : yeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICDId,
                    ChanDoanChinhGhiChu = noitruPhieuDieuTri != null ? noitruPhieuDieuTri.ChanDoanChinhGhiChu : yeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    CheDoAnId = noitruPhieuDieuTri?.CheDoAnId,
                    CheDoChamSoc = noitruPhieuDieuTri?.CheDoChamSoc
                };
                if (yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Any() && noitruPhieuDieuTri != null && noitruPhieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Any())
                {
                    foreach (var item in noitruPhieuDieuTri.NoiTruThamKhamChanDoanKemTheos)
                    {
                        var noiTruThamKhamChanDoanKemTheo = new NoiTruThamKhamChanDoanKemTheo
                        {
                            ICDId = item.ICDId,
                            GhiChu = item.GhiChu
                        };
                        noiTru.NoiTruThamKhamChanDoanKemTheos.Add(noiTruThamKhamChanDoanKemTheo);
                    }
                }
                yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTru);
            }
        }

        public async Task<DateTime?> GetNgayNhapVien(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await BaseRepository.GetByIdAsync(yeuCauTiepNhanId);
            var yctnNhapVien = BaseRepository.TableNoTracking
                            .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens)
                            .Where(z => z.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId).FirstOrDefault();
            if (yctnNhapVien != null)
            {
                return yctnNhapVien.YeuCauKhamBenhs.FirstOrDefault()?.YeuCauNhapViens.FirstOrDefault()?.ThoiDiemChiDinh.Date;
            }
            else
            {
                return yeuCauTiepNhan.ThoiDiemTiepNhan.Date;
            }
        }

        public async Task<long> GetNoiTruKhoaChuyenDen(long yeuCauTiepNhanId)
        {
            return await _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(z => z.NoiTruBenhAnId == yeuCauTiepNhanId && z.ThoiDiemRaKhoa == null).Select(z => z.KhoaPhongChuyenDenId).FirstAsync();
        }

        /// Áp dụng thuốc và tạo ngày
        public async Task<KetQuaApDungNoiTruDonThuocTongHopVo> ApDungDonThuocChoCacNgayDieuTriAsync(NoiTruDonThuocTongHopVo model, YeuCauTiepNhan yeuCauTiepNhan)
        {
            List<NoiTruDonThuocTongHopChiTietVo> noiTruDonThuocTongHopChiTietVos = new List<NoiTruDonThuocTongHopChiTietVo>();
            List<NoiTruDonThuocTongHopChiTietVo> noiTruDonThuocTuVanChiTietVos = new List<NoiTruDonThuocTongHopChiTietVo>();
            foreach (var ngayDieuTri in model.Dates)
            {
                var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.NgayDieuTri == ngayDieuTri && p.KhoaPhongDieuTriId == model.KhoaId).FirstOrDefault();
                if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id == 0)
                {
                    noiTruPhieuDieuTri.NhanVienLapId = _userAgentHelper.GetCurrentUserId();
                    noiTruPhieuDieuTri.KhoaPhongDieuTriId = model.KhoaId.GetValueOrDefault();
                }
                //thuốc thường (dịch truyền)
                foreach (var donThuocChiTiet in model.NoiTruDonThuocTongHopChiTietVos.Where(z => z.NoiTruChiDinhPhaThuocTiemId == null && z.NoiTruChiDinhPhaThuocTruyenId == null))
                {
                    var donThuocChiTietNew = donThuocChiTiet.Clone();
                    noiTruDonThuocTongHopChiTietVos.Add(await ApDungDonThuocNoiTruChiTiet(donThuocChiTietNew, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, false));
                }

                // thuốc tiêm
                if (model.NoiTruDonThuocTongHopChiTietVos.Any(z => z.NoiTruChiDinhPhaThuocTiemId != null))
                {
                    var thuocTiems = model.NoiTruDonThuocTongHopChiTietVos.Where(z => z.NoiTruChiDinhPhaThuocTiemId != null).ToList();
                    noiTruDonThuocTongHopChiTietVos.AddRange(await ApDungDonThuocNoiTruChiTiets(thuocTiems, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, true));
                }

                // thuốc truyền
                if (model.NoiTruDonThuocTongHopChiTietVos.Any(z => z.NoiTruChiDinhPhaThuocTruyenId != null))
                {
                    var thuocTruyens = model.NoiTruDonThuocTongHopChiTietVos.Where(z => z.NoiTruChiDinhPhaThuocTruyenId != null).ToList();
                    noiTruDonThuocTongHopChiTietVos.AddRange(await ApDungDonThuocNoiTruChiTiets(thuocTruyens, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, false));
                }

                // thuốc thường tư vấn
                foreach (var donThuocChiTiet in model.NoiTruDonThuocTuVanChiTietVos)
                {
                    noiTruDonThuocTuVanChiTietVos.Add(await ApDungDonThuocNoiTruChiTiet(donThuocChiTiet, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, true));
                }
            }
            if (noiTruDonThuocTongHopChiTietVos.Any(o => o.SoLuongTonKho < o.SoLuong))
            {
                return new KetQuaApDungNoiTruDonThuocTongHopVo
                {
                    ThanhCong = false,
                    Error = "Số lượng thuốc trong kho không đủ",
                    NoiTruDonThuocTongHopChiTietVos = noiTruDonThuocTongHopChiTietVos,
                    NoiTruDonThuocTuVanChiTietVos = noiTruDonThuocTuVanChiTietVos,
                };
            }
            return new KetQuaApDungNoiTruDonThuocTongHopVo
            {
                ThanhCong = true,
            };
        }

        //Thêm thuốc thường
        private async Task<NoiTruDonThuocTongHopChiTietVo> ApDungDonThuocNoiTruChiTiet(NoiTruDonThuocTongHopChiTietVo donThuocChiTiet, NoiTruPhieuDieuTri noiTruPhieuDieuTri, YeuCauTiepNhan yeuCauTiepNhan, long khoaPhongDieuTriId, DateTime ngayDieuTri, bool laNoiTruDonThuocTuVan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamBenhVienId,
                   x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                   .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
            if (laNoiTruDonThuocTuVan)
            {
                //var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId).FirstOrDefault();
                return await ThemDonThuocTuVan(donThuocChiTiet, yeuCauTiepNhan, noiTruPhieuDieuTri, duocPham, khoaPhongDieuTriId, ngayDieuTri);
            }
            else
            {
                //var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donThuocChiTiet.PhieuDieuTriHienTaiId || (p.NgayDieuTri == ngayDieuTri && p.Id != 0)).FirstOrDefault();
                var SLTon = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                            .Where(p => p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId && (p.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
                            .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);

                var noiTruDonThuocTongHopChiTietVo = donThuocChiTiet;
                var soLuong = donThuocChiTiet.SoLuong;
                noiTruDonThuocTongHopChiTietVo.SoLuongTonKho = laNoiTruDonThuocTuVan ? Int16.MaxValue : 0;
                noiTruDonThuocTongHopChiTietVo.LaNoiTruDonThuocTuVan = laNoiTruDonThuocTuVan;
                noiTruDonThuocTongHopChiTietVo.NgayDieuTri = ngayDieuTri;
                double soLuongCanXuat = donThuocChiTiet.SoLuong;

                var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                 .Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
                          && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                          && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
                          && o.HanSuDung >= DateTime.Now
                          && o.SoLuongNhap > o.SoLuongDaXuat)
                          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault();
                if (nhapKhoDuocPhamChiTiet == null)
                {
                    noiTruDonThuocTongHopChiTietVo.SoLuongTonKho = 0;
                    return noiTruDonThuocTongHopChiTietVo;
                }
                var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

                var noiTruChiDinhDuocPham = new NoiTruChiDinhDuocPham
                {
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                    NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                    NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                    ThoiDiemChiDinh = DateTime.Now,
                    TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                    DuocHuongBaoHiem = donThuocChiTiet.LaDuocPhamBHYT,
                    LaDuocPhamBHYT = donThuocChiTiet.LaDuocPhamBHYT,
                    SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay,
                    DungSang = donThuocChiTiet.DungSang.ToFloatFromFraction(),
                    DungTrua = donThuocChiTiet.DungTrua.ToFloatFromFraction(),
                    DungChieu = donThuocChiTiet.DungChieu.ToFloatFromFraction(),
                    DungToi = donThuocChiTiet.DungToi.ToFloatFromFraction(),
                    ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                    ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                    ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                    ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                    LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                    TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                    DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                    ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                    CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich,
                    TheTich = duocPham.TheTich,
                    GhiChu = donThuocChiTiet.GhiChu,
                    LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri,
                    SoLanTrenVien = donThuocChiTiet.SoLanTrenVien,
                    CachGioDungThuoc = donThuocChiTiet.CachGioDungThuoc,
                    LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay,
                    SoThuTu = donThuocChiTiet.SoThuTu,
                };
                var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
                {
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                    NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                    NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                    ThoiDiemChiDinh = DateTime.Now,
                    TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                    DuocHuongBaoHiem = donThuocChiTiet.LaDuocPhamBHYT,
                    LaDuocPhamBHYT = donThuocChiTiet.LaDuocPhamBHYT,
                    SoTienBenhNhanDaChi = 0,
                    KhoLinhId = donThuocChiTiet.KhoId,
                    TheTich = duocPham.TheTich,
                    LoaiPhieuLinh = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                    GhiChu = donThuocChiTiet.GhiChu,
                    LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                    KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi,
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
                        KhoXuatId = donThuocChiTiet.KhoId.GetValueOrDefault()
                    };
                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                    {
                        DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                        XuatKhoDuocPham = xuatKhoDuocPham,
                        NgayXuat = DateTime.Now
                    };

                    var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();

                    var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(o => (o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId)
                                     && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                                     && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
                                     && o.HanSuDung >= DateTime.Now
                                     && o.SoLuongNhap > o.SoLuongDaXuat)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                        .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToList();
                    //.Where(p => p.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT && p.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId && p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId).ToList();
                    foreach (var item in nhapKhoDuocPhamChiTiets)
                    {
                        if (donThuocChiTiet.SoLuong > 0)
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
                                    DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                                var slTon = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                                noiTruDonThuocTongHopChiTietVo.SoLuongTonKho = slTon;
                                var tonTheoItem = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                                if (donThuocChiTiet.SoLuong <= tonTheoItem)
                                {
                                    xuatViTri.SoLuongXuat = donThuocChiTiet.SoLuong;
                                    item.SoLuongDaXuat = (item.SoLuongDaXuat + donThuocChiTiet.SoLuong).MathRoundNumber(2);
                                    donThuocChiTiet.SoLuong = 0;
                                }
                                else
                                {
                                    xuatViTri.SoLuongXuat = tonTheoItem;
                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                    donThuocChiTiet.SoLuong = (donThuocChiTiet.SoLuong - tonTheoItem).MathRoundNumber(2);
                                }

                                xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                            }

                            if (donThuocChiTiet.SoLuong == 0)
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
                    if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                    {
                        foreach (var yeuCauDuocPhamBenhVien in lstYeuCau)
                        {
                            yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                            noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                        }
                        noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);

                    }
                    else
                    {
                        foreach (var yeuCauDuocPhamBenhVien in lstYeuCau)
                        {
                            yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                            noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                        }
                        noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                        yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                    }
                }
                else
                {
                    if (donThuocChiTiet.SoLuong > 0)
                    {
                        var yeuCauNew = ycDuocPhamBenhVien.Clone();

                        var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
                                                                                                              && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                                                                                                              && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
                                                                                                              && o.HanSuDung >= DateTime.Now
                                                                                                              && o.SoLuongNhap > o.SoLuongDaXuat)
                          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung);

                        var thongTinNhapDuocPham = nhapKhoDuocPhamChiTiets.First();
                        var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.Where(o => o.HopDongThauDuocPhamId == thongTinNhapDuocPham.HopDongThauDuocPhamId).Select(p => p.Gia).FirstOrDefault();
                        var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                        yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                        yeuCauNew.DaCapThuoc = false;
                        yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                        yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                        yeuCauNew.PhuongPhapTinhGiaTriTonKho = thongTinNhapDuocPham.PhuongPhapTinhGiaTriTonKho;
                        yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                        yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                        yeuCauNew.SoLuong = donThuocChiTiet.SoLuong;
                        donThuocChiTiet.SoLuong = 0;
                        noiTruDonThuocTongHopChiTietVo.SoLuong = donThuocChiTiet.SoLuong;
                        noiTruDonThuocTongHopChiTietVo.SoLuongTonKho = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                        if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                        {
                            yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                            noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                            noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                        }
                        else
                        {
                            yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                            noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                            noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                            yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                        }
                    }
                }
                noiTruDonThuocTongHopChiTietVo.SoLuong = soLuong;
                return noiTruDonThuocTongHopChiTietVo;
            }

        }

        // Thêm thuốc tiêm/ truyền
        private async Task<List<NoiTruDonThuocTongHopChiTietVo>> ApDungDonThuocNoiTruChiTiets(List<NoiTruDonThuocTongHopChiTietVo> donThuocChiTiets, NoiTruPhieuDieuTri noiTruPhieuDieuTri, YeuCauTiepNhan yeuCauTiepNhan, long khoaPhongDieuTriId, DateTime ngayDieuTri, bool laThuocTiem)
        {
            var noiTruDonThuocTongHopChiTietVos = new List<NoiTruDonThuocTongHopChiTietVo>();
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var Ids = laThuocTiem ? donThuocChiTiets.Select(z => z.NoiTruChiDinhPhaThuocTiemId).Distinct().ToList() : donThuocChiTiets.Select(z => z.NoiTruChiDinhPhaThuocTruyenId).Distinct().ToList();
            foreach (var id in Ids)
            {
                NoiTruChiDinhPhaThuocTiem noiTruChiDinhPhaThuocTiem = null;
                NoiTruChiDinhPhaThuocTruyen noiTruChiDinhPhaThuocTruyen = null;
                if (laThuocTiem)
                {
                    noiTruChiDinhPhaThuocTiem = new NoiTruChiDinhPhaThuocTiem
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        NoiTruBenhAnId = yeuCauTiepNhan.Id,
                        NoiTruPhieuDieuTri = noiTruPhieuDieuTri,
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        ThoiGianBatDauTiem = donThuocChiTiets.First().ThoiGianBatDauTiem,
                        SoLanTrenMui = donThuocChiTiets.First().SoLanTrenMui,
                        SoLanTrenNgay = donThuocChiTiets.First().SoLanTrenNgay,
                        CachGioTiem = donThuocChiTiets.First().CachGioTiem
                    };
                }
                else
                {
                    noiTruChiDinhPhaThuocTruyen = new NoiTruChiDinhPhaThuocTruyen
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        NoiTruBenhAnId = yeuCauTiepNhan.Id,
                        NoiTruPhieuDieuTri = noiTruPhieuDieuTri,
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        ThoiGianBatDauTruyen = donThuocChiTiets.First().ThoiGianBatDauTruyen,
                        SoLanTrenNgay = donThuocChiTiets.First().SoLanTrenNgay,
                        CachGioTruyen = donThuocChiTiets.First().CachGioTruyen,
                        DonViTocDoTruyen = donThuocChiTiets.First().DonViTocDoTruyen,
                        TocDoTruyen = donThuocChiTiets.First().TocDoTruyen
                    };
                }
                foreach (var donThuocChiTiet in donThuocChiTiets.Where(z => laThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId == id : z.NoiTruChiDinhPhaThuocTruyenId == id))
                {
                    var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamBenhVienId,
                    x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

                    var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                      .Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
                               && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                               && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
                               && o.HanSuDung >= DateTime.Now
                               && o.SoLuongNhap > o.SoLuongDaXuat)
                               .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                    var SLTon = nhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    var noiTruDonThuocTongHopChiTietVo = donThuocChiTiet.Clone();
                    noiTruDonThuocTongHopChiTietVo.SoLuong = donThuocChiTiet.SoLuong;
                    noiTruDonThuocTongHopChiTietVo.SoLuongTonKho = SLTon;
                    noiTruDonThuocTongHopChiTietVo.NgayDieuTri = ngayDieuTri;

                    double soLuongCanXuat = donThuocChiTiet.SoLuong;
                    var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

                    var noiTruChiDinhDuocPham = new NoiTruChiDinhDuocPham
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                        DuocHuongBaoHiem = donThuocChiTiet.LaDuocPhamBHYT,
                        LaDuocPhamBHYT = donThuocChiTiet.LaDuocPhamBHYT,
                        SoLanDungTrongNgay = donThuocChiTiet.SoLanTrenNgay,
                        TheTich = duocPham.TheTich,
                        GhiChu = donThuocChiTiet.GhiChu,
                        LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri,
                        SoLanTrenVien = donThuocChiTiet.SoLanTrenVien,
                        CachGioDungThuoc = donThuocChiTiet.CachGioDungThuoc,
                        LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay,
                        NoiTruChiDinhPhaThuocTiem = noiTruChiDinhPhaThuocTiem,
                        NoiTruChiDinhPhaThuocTruyen = noiTruChiDinhPhaThuocTruyen,
                        LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                        ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                        CachGioTruyenDich = donThuocChiTiet.CachGioTruyen,
                        DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                        TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                        SoThuTu = donThuocChiTiet.SoThuTu
                    };
                    var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                        DuocHuongBaoHiem = donThuocChiTiet.LaDuocPhamBHYT,
                        LaDuocPhamBHYT = donThuocChiTiet.LaDuocPhamBHYT,
                        SoTienBenhNhanDaChi = 0,
                        KhoLinhId = donThuocChiTiet.KhoId,
                        TheTich = duocPham.TheTich,
                        LoaiPhieuLinh = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                        GhiChu = donThuocChiTiet.GhiChu,
                        NoiTruPhieuDieuTriId = donThuocChiTiet.PhieuDieuTriHienTaiId,
                        LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                        KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi
                    };

                    soLuongCanXuat = soLuongCanXuat - soLuongXuat;

                    ycDuocPhamBenhVien.HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiets.First().HopDongThauDuocPhamId;

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
                            KhoXuatId = donThuocChiTiet.KhoId.GetValueOrDefault()
                        };
                        var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                        {
                            DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                            XuatKhoDuocPham = xuatKhoDuocPham,
                            NgayXuat = DateTime.Now
                        };

                        var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();
                        foreach (var item in nhapKhoDuocPhamChiTiets)
                        {
                            if (donThuocChiTiet.SoLuong > 0)
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
                                        DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                                    noiTruDonThuocTongHopChiTietVo.SoLuongTonKho = tonTheoItem;
                                    if (donThuocChiTiet.SoLuong <= tonTheoItem)
                                    {
                                        xuatViTri.SoLuongXuat = donThuocChiTiet.SoLuong;
                                        item.SoLuongDaXuat = (item.SoLuongDaXuat + donThuocChiTiet.SoLuong).MathRoundNumber(2);
                                        donThuocChiTiet.SoLuong = 0;
                                    }
                                    else
                                    {
                                        xuatViTri.SoLuongXuat = tonTheoItem;
                                        item.SoLuongDaXuat = item.SoLuongNhap;
                                        donThuocChiTiet.SoLuong = (donThuocChiTiet.SoLuong - tonTheoItem).MathRoundNumber(2);
                                    }

                                    xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                                }

                                if (donThuocChiTiet.SoLuong == 0)
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
                        //foreach (var item in lstYeuCau)
                        //{
                        //    noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(item);
                        //}
                        //noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                        noiTruDonThuocTongHopChiTietVos.Add(noiTruDonThuocTongHopChiTietVo);
                        if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                        {
                            foreach (var yeuCauDuocPhamBenhVien in lstYeuCau)
                            {

                                yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                                noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                            }
                            noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);

                        }
                        else
                        {
                            foreach (var yeuCauDuocPhamBenhVien in lstYeuCau)
                            {
                                yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                                noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                            }
                            noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                            yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                        }

                    }
                    else
                    {
                        if (donThuocChiTiet.SoLuong > 0)
                        {
                            var yeuCauNew = ycDuocPhamBenhVien.Clone();

                            var thongTinNhapDuocPham = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
                                                                                                                  && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                                                                                                                  && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
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
                            yeuCauNew.SoLuong = donThuocChiTiet.SoLuong;
                            donThuocChiTiet.SoLuong = 0;

                            noiTruDonThuocTongHopChiTietVo.SoLuongTonKho = nhapKhoDuocPhamChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                            noiTruDonThuocTongHopChiTietVos.Add(noiTruDonThuocTongHopChiTietVo);
                            if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                            {
                                yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                                noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                                noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                            }
                            else
                            {
                                yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                                noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                                noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                                yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                            }
                        }
                    }
                }
            }
            return noiTruDonThuocTongHopChiTietVos;
        }
        private async Task<NoiTruDonThuocTongHopChiTietVo> ThemDonThuocTuVan(NoiTruDonThuocTongHopChiTietVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan, NoiTruPhieuDieuTri noiTruPhieuDieuTri, DuocPham duocPham, long khoaPhongDieuTriId, DateTime ngayDieuTri)
        {
            NoiTruDonThuocTongHopChiTietVo noiTruDonThuocTongHopChiTietVo = donThuocChiTiet;
            noiTruDonThuocTongHopChiTietVo.LaNoiTruDonThuocTuVan = true;
            var noiTruPhieuDieuTriTuVanThuoc = new NoiTruPhieuDieuTriTuVanThuoc
            {
                YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                NoiTruPhieuDieuTriId = donThuocChiTiet.PhieuDieuTriHienTaiId.Value,
                DuocPhamId = donThuocChiTiet.DuocPhamBenhVienId,
                Ten = duocPham.Ten,
                LaDuocPhamBenhVien = false,
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
                ChuYDePhong = duocPham.ChuYDePhong,
                SoLuong = donThuocChiTiet.SoLuong,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay,
                DungSang = donThuocChiTiet.DungSang.ToFloatFromFraction(),
                DungTrua = donThuocChiTiet.DungTrua.ToFloatFromFraction(),
                DungChieu = donThuocChiTiet.DungChieu.ToFloatFromFraction(),
                DungToi = donThuocChiTiet.DungToi.ToFloatFromFraction(),
                ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich,
                TheTich = duocPham.TheTich,
                GhiChu = donThuocChiTiet.GhiChu,
            };
            if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
            {
                noiTruPhieuDieuTri.NoiTruPhieuDieuTriTuVanThuocs.Add(noiTruPhieuDieuTriTuVanThuoc);
            }
            else
            {
                noiTruPhieuDieuTri.NoiTruPhieuDieuTriTuVanThuocs.Add(noiTruPhieuDieuTriTuVanThuoc);
                yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
            }
            return noiTruDonThuocTongHopChiTietVo;
        }

        /// Cofirm  Áp dụng thuốc và tạo ngày
        public async Task<string> ApDungDonThuocChoCacNgayDieuTriConfirmAsync(NoiTruDonThuocTongHopVo model, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var error = string.Empty;
            foreach (var ngayDieuTri in model.Dates)
            {
                var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.NgayDieuTri == ngayDieuTri && p.KhoaPhongDieuTriId == model.KhoaId).FirstOrDefault();
                if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id == 0)
                {
                    noiTruPhieuDieuTri.NhanVienLapId = _userAgentHelper.GetCurrentUserId();
                    noiTruPhieuDieuTri.KhoaPhongDieuTriId = model.KhoaId.GetValueOrDefault();
                }
                //thuốc thường (dịch truyền)
                foreach (var donThuocChiTiet in model.NoiTruDonThuocTongHopChiTietVos.Where(z => z.NoiTruChiDinhPhaThuocTiemId == null && z.NoiTruChiDinhPhaThuocTruyenId == null))
                {
                    var donThuocChiTietNew = donThuocChiTiet.Clone();
                    error = await ApDungDonThuocNoiTruChiTietConfirm(donThuocChiTietNew, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, false);
                }

                // thuốc tiêm
                if (model.NoiTruDonThuocTongHopChiTietVos.Any(z => z.NoiTruChiDinhPhaThuocTiemId != null))
                {
                    var thuocTiems = model.NoiTruDonThuocTongHopChiTietVos.Where(z => z.NoiTruChiDinhPhaThuocTiemId != null).ToList();
                    error = await ApDungDonThuocNoiTruChiTietsConfirm(thuocTiems, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, true);
                }

                // thuốc truyền
                if (model.NoiTruDonThuocTongHopChiTietVos.Any(z => z.NoiTruChiDinhPhaThuocTruyenId != null))
                {
                    var thuocTruyens = model.NoiTruDonThuocTongHopChiTietVos.Where(z => z.NoiTruChiDinhPhaThuocTruyenId != null).ToList();
                    error = await ApDungDonThuocNoiTruChiTietsConfirm(thuocTruyens, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, true);
                }

                // thuốc thường tư vấn
                foreach (var donThuocChiTiet in model.NoiTruDonThuocTuVanChiTietVos)
                {
                    error = await ApDungDonThuocNoiTruChiTietConfirm(donThuocChiTiet, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, true);
                }
            }
            return error;
        }

        private async Task<string> ApDungDonThuocNoiTruChiTietConfirm(NoiTruDonThuocTongHopChiTietVo donThuocChiTiet, NoiTruPhieuDieuTri noiTruPhieuDieuTri, YeuCauTiepNhan yeuCauTiepNhan, long khoaPhongDieuTriId, DateTime ngayDieuTri, bool laNoiTruDonThuocTuVan)
        {
            var error = string.Empty;
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamBenhVienId,
                   x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                   .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
            if (laNoiTruDonThuocTuVan)
            {
                return await ThemDonThuocTuVanConfirm(donThuocChiTiet, yeuCauTiepNhan, noiTruPhieuDieuTri, duocPham, khoaPhongDieuTriId, ngayDieuTri);
            }
            else
            {
                var SLTon = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                            .Where(p => p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId && (p.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
                            .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);

                if (SLTon < donThuocChiTiet.SoLuong)
                {
                    return error;
                }

                double soLuongCanXuat = donThuocChiTiet.SoLuong;

                var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                 .Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
                          && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                          && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
                          && o.HanSuDung >= DateTime.Now
                          && o.SoLuongNhap > o.SoLuongDaXuat)
                          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

                var noiTruChiDinhDuocPham = new NoiTruChiDinhDuocPham
                {
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                    NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                    NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                    ThoiDiemChiDinh = DateTime.Now,
                    TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                    DuocHuongBaoHiem = donThuocChiTiet.LaDuocPhamBHYT,
                    LaDuocPhamBHYT = donThuocChiTiet.LaDuocPhamBHYT,
                    SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay,
                    DungSang = donThuocChiTiet.DungSang.ToFloatFromFraction(),
                    DungTrua = donThuocChiTiet.DungTrua.ToFloatFromFraction(),
                    DungChieu = donThuocChiTiet.DungChieu.ToFloatFromFraction(),
                    DungToi = donThuocChiTiet.DungToi.ToFloatFromFraction(),
                    ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                    ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                    ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                    ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                    LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                    TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                    DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                    ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                    CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich,
                    TheTich = duocPham.TheTich,
                    GhiChu = donThuocChiTiet.GhiChu,
                    LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri,
                    SoLanTrenVien = donThuocChiTiet.SoLanTrenVien,
                    CachGioDungThuoc = donThuocChiTiet.CachGioDungThuoc,
                    LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay,
                    SoThuTu = donThuocChiTiet.SoThuTu,
                };
                var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
                {
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                    NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                    NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                    ThoiDiemChiDinh = DateTime.Now,
                    TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                    DuocHuongBaoHiem = donThuocChiTiet.LaDuocPhamBHYT,
                    LaDuocPhamBHYT = donThuocChiTiet.LaDuocPhamBHYT,
                    SoTienBenhNhanDaChi = 0,
                    KhoLinhId = donThuocChiTiet.KhoId,
                    TheTich = duocPham.TheTich,
                    LoaiPhieuLinh = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                    GhiChu = donThuocChiTiet.GhiChu,
                    LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                    KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi,
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
                        KhoXuatId = donThuocChiTiet.KhoId.GetValueOrDefault()
                    };
                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                    {
                        DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                        XuatKhoDuocPham = xuatKhoDuocPham,
                        NgayXuat = DateTime.Now
                    };

                    var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();

                    var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(o => (o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId)
                                     && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                                     && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
                                     && o.HanSuDung >= DateTime.Now
                                     && o.SoLuongNhap > o.SoLuongDaXuat)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                        .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToList();
                    //.Where(p => p.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT && p.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId && p.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId).ToList();
                    foreach (var item in nhapKhoDuocPhamChiTiets)
                    {
                        if (donThuocChiTiet.SoLuong > 0)
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
                                    DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                                if (donThuocChiTiet.SoLuong <= tonTheoItem)
                                {
                                    xuatViTri.SoLuongXuat = donThuocChiTiet.SoLuong;
                                    item.SoLuongDaXuat = (item.SoLuongDaXuat + donThuocChiTiet.SoLuong).MathRoundNumber(2);
                                    donThuocChiTiet.SoLuong = 0;
                                }
                                else
                                {
                                    xuatViTri.SoLuongXuat = tonTheoItem;
                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                    donThuocChiTiet.SoLuong = (donThuocChiTiet.SoLuong - tonTheoItem).MathRoundNumber(2);
                                }

                                xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                            }

                            if (donThuocChiTiet.SoLuong == 0)
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
                    if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                    {
                        foreach (var yeuCauDuocPhamBenhVien in lstYeuCau)
                        {
                            yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                            noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                        }
                        noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                    }
                    else
                    {
                        foreach (var yeuCauDuocPhamBenhVien in lstYeuCau)
                        {
                            yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                            noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                        }
                        noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                        yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                    }
                }
                else
                {
                    if (donThuocChiTiet.SoLuong > 0)
                    {
                        var yeuCauNew = ycDuocPhamBenhVien.Clone();

                        var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
                                                                                                              && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                                                                                                              && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
                                                                                                              && o.HanSuDung >= DateTime.Now
                                                                                                              && o.SoLuongNhap > o.SoLuongDaXuat)
                          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung);

                        var thongTinNhapDuocPham = nhapKhoDuocPhamChiTiets.First();
                        var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.Where(o => o.HopDongThauDuocPhamId == thongTinNhapDuocPham.HopDongThauDuocPhamId).Select(p => p.Gia).FirstOrDefault();
                        var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                        yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                        yeuCauNew.DaCapThuoc = false;
                        yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                        yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                        yeuCauNew.PhuongPhapTinhGiaTriTonKho = thongTinNhapDuocPham.PhuongPhapTinhGiaTriTonKho;
                        yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                        yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                        yeuCauNew.SoLuong = donThuocChiTiet.SoLuong;
                        donThuocChiTiet.SoLuong = 0;

                        if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                        {
                            yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                            noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                            noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                        }
                        else
                        {
                            yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                            noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                            noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                            yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                        }
                    }
                }
                return error;
            }

        }

        private async Task<string> ApDungDonThuocNoiTruChiTietsConfirm(List<NoiTruDonThuocTongHopChiTietVo> donThuocChiTiets, NoiTruPhieuDieuTri noiTruPhieuDieuTri, YeuCauTiepNhan yeuCauTiepNhan, long khoaPhongDieuTriId, DateTime ngayDieuTri, bool laThuocTiem)
        {
            var noiTruDonThuocTongHopChiTietVos = new List<NoiTruDonThuocTongHopChiTietVo>();
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var Ids = laThuocTiem ? donThuocChiTiets.Select(z => z.NoiTruChiDinhPhaThuocTiemId).Distinct().ToList() : donThuocChiTiets.Select(z => z.NoiTruChiDinhPhaThuocTruyenId).Distinct().ToList();
            foreach (var id in Ids)
            {
                NoiTruChiDinhPhaThuocTiem noiTruChiDinhPhaThuocTiem = null;
                NoiTruChiDinhPhaThuocTruyen noiTruChiDinhPhaThuocTruyen = null;
                if (laThuocTiem)
                {
                    noiTruChiDinhPhaThuocTiem = new NoiTruChiDinhPhaThuocTiem
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        NoiTruBenhAnId = yeuCauTiepNhan.Id,
                        NoiTruPhieuDieuTri = noiTruPhieuDieuTri,
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        ThoiGianBatDauTiem = donThuocChiTiets.First().ThoiGianBatDauTiem,
                        SoLanTrenMui = donThuocChiTiets.First().SoLanTrenMui,
                        SoLanTrenNgay = donThuocChiTiets.First().SoLanTrenNgay,
                        CachGioTiem = donThuocChiTiets.First().CachGioTiem
                    };
                }
                else
                {
                    noiTruChiDinhPhaThuocTruyen = new NoiTruChiDinhPhaThuocTruyen
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        NoiTruBenhAnId = yeuCauTiepNhan.Id,
                        NoiTruPhieuDieuTri = noiTruPhieuDieuTri,
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        ThoiGianBatDauTruyen = donThuocChiTiets.First().ThoiGianBatDauTruyen,
                        SoLanTrenNgay = donThuocChiTiets.First().SoLanTrenNgay,
                        CachGioTruyen = donThuocChiTiets.First().CachGioTruyen,
                        DonViTocDoTruyen = donThuocChiTiets.First().DonViTocDoTruyen,
                        TocDoTruyen = donThuocChiTiets.First().TocDoTruyen
                    };
                }
                foreach (var donThuocChiTiet in donThuocChiTiets.Where(z => laThuocTiem ? z.NoiTruChiDinhPhaThuocTiemId == id : z.NoiTruChiDinhPhaThuocTruyenId == id))
                {
                    var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamBenhVienId,
                    x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

                    var nhapKhoDuocPhamChiTiets = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                      .Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
                               && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                               && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
                               && o.HanSuDung >= DateTime.Now
                               && o.SoLuongNhap > o.SoLuongDaXuat)
                               .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                    var SLTon = nhapKhoDuocPhamChiTiets.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    if (SLTon < donThuocChiTiet.SoLuong)
                    {
                        continue;
                    }
                    double soLuongCanXuat = donThuocChiTiet.SoLuong;
                    var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

                    var noiTruChiDinhDuocPham = new NoiTruChiDinhDuocPham
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                        DuocHuongBaoHiem = donThuocChiTiet.LaDuocPhamBHYT,
                        LaDuocPhamBHYT = donThuocChiTiet.LaDuocPhamBHYT,
                        SoLanDungTrongNgay = donThuocChiTiet.SoLanTrenNgay,
                        TheTich = duocPham.TheTich,
                        GhiChu = donThuocChiTiet.GhiChu,
                        LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri,
                        SoLanTrenVien = donThuocChiTiet.SoLanTrenVien,
                        CachGioDungThuoc = donThuocChiTiet.CachGioDungThuoc,
                        LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay,
                        NoiTruChiDinhPhaThuocTiem = noiTruChiDinhPhaThuocTiem,
                        NoiTruChiDinhPhaThuocTruyen = noiTruChiDinhPhaThuocTruyen,
                        LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                        ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                        CachGioTruyenDich = donThuocChiTiet.CachGioTruyen,
                        DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                        TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                        SoThuTu = donThuocChiTiet.SoThuTu
                    };
                    var ycDuocPhamBenhVien = new YeuCauDuocPhamBenhVien
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                        NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        TrangThai = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumYeuCauDuocPhamBenhVien.ChuaThucHien : EnumYeuCauDuocPhamBenhVien.DaThucHien,
                        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                        DuocHuongBaoHiem = donThuocChiTiet.LaDuocPhamBHYT,
                        LaDuocPhamBHYT = donThuocChiTiet.LaDuocPhamBHYT,
                        SoTienBenhNhanDaChi = 0,
                        KhoLinhId = donThuocChiTiet.KhoId,
                        TheTich = duocPham.TheTich,
                        LoaiPhieuLinh = donThuocChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                        GhiChu = donThuocChiTiet.GhiChu,
                        NoiTruPhieuDieuTriId = donThuocChiTiet.PhieuDieuTriHienTaiId,
                        LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                        KhongTinhPhi = !donThuocChiTiet.KhongTinhPhi
                    };

                    soLuongCanXuat = soLuongCanXuat - soLuongXuat;

                    ycDuocPhamBenhVien.HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiets.First().HopDongThauDuocPhamId;

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
                            KhoXuatId = donThuocChiTiet.KhoId.GetValueOrDefault()
                        };
                        var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                        {
                            DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
                            XuatKhoDuocPham = xuatKhoDuocPham,
                            NgayXuat = DateTime.Now
                        };

                        var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();
                        foreach (var item in nhapKhoDuocPhamChiTiets)
                        {
                            if (donThuocChiTiet.SoLuong > 0)
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
                                        DuocPhamBenhVienId = donThuocChiTiet.DuocPhamBenhVienId,
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
                                    if (donThuocChiTiet.SoLuong <= tonTheoItem)
                                    {
                                        xuatViTri.SoLuongXuat = donThuocChiTiet.SoLuong;
                                        item.SoLuongDaXuat = (item.SoLuongDaXuat + donThuocChiTiet.SoLuong).MathRoundNumber(2);
                                        donThuocChiTiet.SoLuong = 0;
                                    }
                                    else
                                    {
                                        xuatViTri.SoLuongXuat = tonTheoItem;
                                        item.SoLuongDaXuat = item.SoLuongNhap;
                                        donThuocChiTiet.SoLuong = (donThuocChiTiet.SoLuong - tonTheoItem).MathRoundNumber(2);
                                    }

                                    xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);
                                }

                                if (donThuocChiTiet.SoLuong == 0)
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
                        if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                        {
                            foreach (var yeuCauDuocPhamBenhVien in lstYeuCau)
                            {

                                yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                                noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                            }
                            noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);

                        }
                        else
                        {
                            foreach (var yeuCauDuocPhamBenhVien in lstYeuCau)
                            {
                                yeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                                noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                            }
                            noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                            yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                        }

                    }
                    else
                    {
                        if (donThuocChiTiet.SoLuong > 0)
                        {
                            var yeuCauNew = ycDuocPhamBenhVien.Clone();

                            var thongTinNhapDuocPham = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoId == donThuocChiTiet.KhoId
                                                                                                                  && o.LaDuocPhamBHYT == donThuocChiTiet.LaDuocPhamBHYT
                                                                                                                  && o.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamBenhVienId
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
                            yeuCauNew.SoLuong = donThuocChiTiet.SoLuong;
                            donThuocChiTiet.SoLuong = 0;
                            if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                            {
                                yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                                noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                                noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                            }
                            else
                            {
                                yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                                noiTruChiDinhDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                                noiTruPhieuDieuTri.NoiTruChiDinhDuocPhams.Add(noiTruChiDinhDuocPham);
                                yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        private async Task<string> ThemDonThuocTuVanConfirm(NoiTruDonThuocTongHopChiTietVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan, NoiTruPhieuDieuTri noiTruPhieuDieuTri, DuocPham duocPham, long khoaPhongDieuTriId, DateTime ngayDieuTri)
        {
            NoiTruDonThuocTongHopChiTietVo noiTruDonThuocTongHopChiTietVo = donThuocChiTiet;
            noiTruDonThuocTongHopChiTietVo.LaNoiTruDonThuocTuVan = true;
            var noiTruPhieuDieuTriTuVanThuoc = new NoiTruPhieuDieuTriTuVanThuoc
            {
                YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                NoiTruPhieuDieuTriId = donThuocChiTiet.PhieuDieuTriHienTaiId.Value,
                DuocPhamId = donThuocChiTiet.DuocPhamBenhVienId,
                Ten = duocPham.Ten,
                LaDuocPhamBenhVien = false,
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
                ChuYDePhong = duocPham.ChuYDePhong,
                SoLuong = donThuocChiTiet.SoLuong,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                SoLanDungTrongNgay = donThuocChiTiet.SoLanDungTrongNgay,
                DungSang = donThuocChiTiet.DungSang.ToFloatFromFraction(),
                DungTrua = donThuocChiTiet.DungTrua.ToFloatFromFraction(),
                DungChieu = donThuocChiTiet.DungChieu.ToFloatFromFraction(),
                DungToi = donThuocChiTiet.DungToi.ToFloatFromFraction(),
                ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                LaDichTruyen = donThuocChiTiet.LaDichTruyen,
                TocDoTruyen = donThuocChiTiet.TocDoTruyen,
                DonViTocDoTruyen = donThuocChiTiet.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = donThuocChiTiet.ThoiGianBatDauTruyen,
                CachGioTruyenDich = donThuocChiTiet.CachGioTruyenDich,
                TheTich = duocPham.TheTich,
                GhiChu = donThuocChiTiet.GhiChu,
            };
            if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
            {
                noiTruPhieuDieuTri.NoiTruPhieuDieuTriTuVanThuocs.Add(noiTruPhieuDieuTriTuVanThuoc);
            }
            else
            {
                noiTruPhieuDieuTri.NoiTruPhieuDieuTriTuVanThuocs.Add(noiTruPhieuDieuTriTuVanThuoc);
                yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
            }
            return string.Empty;
        }
    }
}
