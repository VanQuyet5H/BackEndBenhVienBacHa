using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.CauHinh;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.KhamDoans;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {

        public GridDataSource GetDataForGridTuVanThuoc(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var lstDuocPham = _tuVanThuocKhamSucKhoeRepository.TableNoTracking
                             .Include(o => o.DuocPham)
                             .Select(p => p.DuocPham.MaHoatChat).ToList();

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

            var query = _tuVanThuocKhamSucKhoeRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.SoLuong > 0)
                .Select(s => new TuVanThuocGridVoItem
                {
                    Id = s.Id,
                    DuocPhamId = s.DuocPhamId,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaHoatChat = s.DuocPham.MaHoatChat,
                    Ma = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.Ma : s.DuocPham.MaHoatChat,
                    Ten = s.DuocPham.Ten,
                    HoatChat = s.DuocPham.HoatChat,
                    DVT = s.DuocPham.DonViTinh.Ten,
                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                    ThoiGianDungSang = s.ThoiGianDungSang,
                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                    ThoiGianDungToi = s.ThoiGianDungToi,
                    SangDisplay = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                    TruaDisplay = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                    ChieuDisplay = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                    ToiDisplay = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                    ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                    SoNgayDung = s.SoNgayDung,
                    SoLuong = s.SoLuong,
                    SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                    TenDuongDung = s.DuongDung.Ten,
                    DiUngThuocDisplay = s.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.DuocPham.MaHoatChat && diung.LoaiDiUng == Enums.LoaiDiUng.Thuoc) ? "Có" : "Không",
                    TuongTacThuoc = GetTuongTac(s.DuocPham.MaHoatChat, lstDuocPham, lstADR),
                    GhiChu = s.GhiChu,
                });
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };

        }

        public GridDataSource GetTotalPageForGridTuVanThuoc(QueryInfo queryInfo)
        {
            return null;
        }
        public async Task<List<DuocPhamTuVanTemplate>> GetDuocPhamTuVans(DropDownListRequestModel queryInfo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var lstColumnNameSearch = new List<string>
                {
                    nameof(DuocPham.Ten),
                    nameof(DuocPham.HoatChat)
                };

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var duocPhams = _duocPhamRepository.TableNoTracking
                .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                .Select(s => new DuocPhamTuVanTemplate
                {
                    DisplayName = s.Ten,
                    KeyId = s.Id,
                    Ten = s.Ten,
                    LaDuocPhamBenhVien = s.DuocPhamBenhVien != null,
                    CoNhapKhoDuocPhamChiTiet = s.DuocPhamBenhVien != null && s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(),
                    SLTon = s.DuocPhamBenhVien != null && s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat) : 0,
                })
                .ApplyLike(queryInfo.Query, o => o.Ten, o => o.DisplayName)
                .Take(queryInfo.Take);
                return await duocPhams.ToListAsync();
            }
            else
            {
                var duocPhamIds = _duocPhamRepository
                           .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                           .Select(p => p.Id).ToList();

                var dictionary = duocPhamIds.Select((id, index) => new
                {
                    key = id,
                    rank = index,
                }).ToDictionary(o => o.key, o => o.rank);

                var duocPhams = _duocPhamRepository
                                    .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                                    .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                                    .Take(queryInfo.Take)
                                    .Select(s => new DuocPhamTuVanTemplate
                                    {
                                        Rank = dictionary.Any(a => a.Key == s.Id) ? dictionary[s.Id] : dictionary.Count,
                                        DisplayName = s.Ten,
                                        KeyId = s.Id,
                                        Ten = s.Ten,
                                        LaDuocPhamBenhVien = s.DuocPhamBenhVien != null,
                                        CoNhapKhoDuocPhamChiTiet = s.DuocPhamBenhVien != null && s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(),
                                        SLTon = s.DuocPhamBenhVien != null && s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any() ? s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat) : 0,
                                    });
                return await duocPhams.ToListAsync();
            }
        }

        public GetDuocPhamTonKhoGridVoItem GetDuocPhamTuVanInfoById(ThongTinThuocTuVanVo thongTinThuocVo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(thongTinThuocVo.YeuCauTiepNhanId, x => x.Include(o => o.TuVanThuocKhamSucKhoes)
                                                                                                        .Include(yc => yc.BenhNhan).ThenInclude(bn => bn.BenhNhanDiUngThuocs));
            var lstThuocHoacHoatChat = _thuocHoacHoatChatRepository.TableNoTracking.Select(p => p.Ten).ToList();

            var lstDuocPham = yeuCauTiepNhan.TuVanThuocKhamSucKhoes.Select(o => o.MaHoatChat).ToList();
            var lstADR = _aDRRepository.TableNoTracking
                           .Select(s => new MaHoatChatGridVo
                           {
                               Ten1 = s.ThuocHoacHoatChat1.Ten,
                               Ten2 = s.ThuocHoacHoatChat2.Ten,
                               MaHoatChat1 = s.ThuocHoacHoatChat1.Ma,
                               MaHoatChat2 = s.ThuocHoacHoatChat2.Ma
                           }).ToList();
            var duocPhamInfo = _duocPhamRepository.TableNoTracking
                .Where(o => o.Id == thongTinThuocVo.DuocPhamId)
                .Select(d => new GetDuocPhamTonKhoGridVoItem
                {
                    Id = d.Id,
                    TuongTacThuoc = GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR) == string.Empty ? "Không" : GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR),
                    FlagTuongTac = !GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR).Contains("Không") ? true : false,
                    FlagDiUng = d.HoatChat != null && yeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng.Contains(d.HoatChat) && diung.LoaiDiUng == Enums.LoaiDiUng.Thuoc),
                    FlagThuocDaKe = yeuCauTiepNhan.TuVanThuocKhamSucKhoes.Any(dtct => dtct.MaHoatChat == d.MaHoatChat),
                    DVTId = d.DonViTinh.Id,
                    TenDonViTinh = d.DonViTinh.Ten,
                    DuongDungId = d.DuongDung.Id,
                    TenDuongDung = d.DuongDung.Ten,
                    CoNhapKhoDuocPhamChiTiet = d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(),
                    HanSuDung = d.DuocPhamBenhVien != null && d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any() ? d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                             nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc &&
                                             nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).Select(p => p.HanSuDung).FirstOrDefault() : (DateTime?)null,
                    TonKho = d.DuocPhamBenhVien != null && d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any() ? d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat) : 0,
                    MucDo = yeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Where(p => lstThuocHoacHoatChat.Contains(p.TenDiUng)).Select(p => p.MucDo).FirstOrDefault(),
                }).FirstOrDefault();
            return duocPhamInfo;
        }

        public async Task<string> ThemDonThuocTuVanSucKhoe(DonThuocChiTietVo donThuocChiTiet)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(donThuocChiTiet.YeuCauTiepNhanId.Value);
            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamId, s => s.Include(p => p.DuocPhamBenhVien));
            var tuVanThuocKhamSucKhoe = new TuVanThuocKhamSucKhoe
            {
                DuocPhamId = donThuocChiTiet.DuocPhamId,
                LaDuocPhamBenhVien = duocPham.DuocPhamBenhVien != null,
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
                ChuYDePhong = duocPham.ChuYDePhong,
                SoLuong = donThuocChiTiet.SoLuong,
                ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                DungSang = donThuocChiTiet.DungSang,
                DungTrua = donThuocChiTiet.DungTrua,
                DungChieu = donThuocChiTiet.DungChieu,
                DungToi = donThuocChiTiet.DungToi,
                GhiChu = donThuocChiTiet.GhiChu
            };
            yeuCauTiepNhan.TuVanThuocKhamSucKhoes.Add(tuVanThuocKhamSucKhoe);
            await _yeuCauTiepNhanRepository.UpdateAsync(yeuCauTiepNhan);
            return string.Empty;
        }

        public async Task<string> CapNhatDonThuocTuVanSucKhoe(DonThuocChiTietVo donThuocChiTiet)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(donThuocChiTiet.YeuCauTiepNhanId.Value, s => s.Include(p => p.TuVanThuocKhamSucKhoes));
            var tuVanSucKhoe = yeuCauTiepNhan.TuVanThuocKhamSucKhoes.Where(p => p.Id == donThuocChiTiet.DonThuocChiTietId).FirstOrDefault();
            if (tuVanSucKhoe == null)
            {
                return GetResourceValueByResourceName("PhieuDieuTri.DonThuoc.NotExists");
            }
            tuVanSucKhoe.SoLuong = donThuocChiTiet.SoLuong;
            tuVanSucKhoe.DungSang = donThuocChiTiet.DungSang;
            tuVanSucKhoe.DungTrua = donThuocChiTiet.DungTrua;
            tuVanSucKhoe.DungChieu = donThuocChiTiet.DungChieu;
            tuVanSucKhoe.DungToi = donThuocChiTiet.DungToi;
            tuVanSucKhoe.ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang;
            tuVanSucKhoe.ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua;
            tuVanSucKhoe.ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu;
            tuVanSucKhoe.ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi;
            tuVanSucKhoe.GhiChu = donThuocChiTiet.GhiChu;
            await _yeuCauTiepNhanRepository.UpdateAsync(yeuCauTiepNhan);
            return string.Empty;
        }

        public async Task<bool> KiemTraThuocTuVanSucKhoe(long donThuocChiTietId)
        {
            var checkExist = await _tuVanThuocKhamSucKhoeRepository.TableNoTracking
              .Where(x => x.Id == donThuocChiTietId).ToListAsync();
            if (checkExist.Any())
            {
                return true;
            }
            return false;
        }

        public string InTuVanThuoc(InTuVanThuoc inTuVanThuoc)
        {
            var content = string.Empty;
            var infoBN = ThongTinBenhNhanPhieuThuoc(inTuVanThuoc.YeuCauTiepNhanId, inTuVanThuoc.YeuCauKhamBenhId);
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("TuVanThuoc")).First();
            var getTenBSKham = _bacSiChiDinhRepository.TableNoTracking
                               .Where(bs => bs.Id == _userAgentHelper.GetCurrentUserId())
                               .Select(bs => bs.HoTen).FirstOrDefault();
            var tuVanThuocs = _tuVanThuocKhamSucKhoeRepository.TableNoTracking
                              .Include(p => p.DuocPham).ThenInclude(p => p.DonViTinh)
                              .Include(p => p.DuocPham).ThenInclude(p => p.DuongDung)
                              .Include(dtct => dtct.DuocPham).ThenInclude(dtct => dtct.DuocPhamBenhVien)
                              .Where(p => p.YeuCauTiepNhanId == inTuVanThuoc.YeuCauTiepNhanId).ToList();

            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaPhong = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == noiLamViecCurrentId).Select(p => p.KhoaPhong.Ten).FirstOrDefault();
            var resultThuoc = string.Empty;
            var sttBHYT = 0;

            if (tuVanThuocs.Any())
            {
                var lstThoiGian = tuVanThuocs.Select(s => new ThoiGianDungThuoc
                {
                    ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                }).ToList();
                for (int i = 0; i < tuVanThuocs.Count; i++)
                {
                    var cd =
                             (tuVanThuocs[i].DungSang != null
                                 //? "Sáng " + tuVanThuocs[i].DungSang.FloatToStringFraction() +
                                 ? "Sáng " + ((tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                                        NumberHelper.ChuyenSoRaText(Convert.ToDouble(tuVanThuocs[i].DungSang), false) :
                                                       (Convert.ToDouble(tuVanThuocs[i].DungSang) < 10 && Convert.ToInt32(tuVanThuocs[i].DungSang) == tuVanThuocs[i].DungSang) ? "0" + tuVanThuocs[i].DungSang.FloatToStringFraction() + " " : tuVanThuocs[i].DungSang.FloatToStringFraction() + " ") +
                                   (!string.IsNullOrEmpty(lstThoiGian[i].ThoiGianDungSangDisplay)
                                       ? " " + lstThoiGian[i].ThoiGianDungSangDisplay
                                       : "") + " " + tuVanThuocs[i].DonViTinh?.Ten + ","
                                 : "") +
                             (tuVanThuocs[i].DungTrua != null
                                 ? "Trưa " + ((tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                                        NumberHelper.ChuyenSoRaText(Convert.ToDouble(tuVanThuocs[i].DungTrua), false) :
                                                       (Convert.ToDouble(tuVanThuocs[i].DungTrua) < 10 && Convert.ToInt32(tuVanThuocs[i].DungTrua) == tuVanThuocs[i].DungTrua) ? "0" + tuVanThuocs[i].DungTrua.FloatToStringFraction() + " " : tuVanThuocs[i].DungTrua.FloatToStringFraction() + " ") +
                                   (!string.IsNullOrEmpty(lstThoiGian[i].ThoiGianDungTruaDisplay)
                                       ? " " + lstThoiGian[i].ThoiGianDungTruaDisplay
                                       : "") + " " + tuVanThuocs[i].DonViTinh?.Ten + ","
                                 : "") +
                             (tuVanThuocs[i].DungChieu != null
                                 ? "Chiều " + ((tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                                        NumberHelper.ChuyenSoRaText(Convert.ToDouble(tuVanThuocs[i].DungChieu), false) :
                                                       (Convert.ToDouble(tuVanThuocs[i].DungChieu) < 10 && Convert.ToInt32(tuVanThuocs[i].DungChieu) == tuVanThuocs[i].DungChieu) ? "0" + tuVanThuocs[i].DungChieu.FloatToStringFraction() + " " : tuVanThuocs[i].DungChieu.FloatToStringFraction() + " ") +
                                   (!string.IsNullOrEmpty(lstThoiGian[i].ThoiGianDungChieuDisplay)
                                       ? " " + lstThoiGian[i].ThoiGianDungChieuDisplay
                                       : "") + " " + tuVanThuocs[i].DonViTinh?.Ten + ","
                                 : "") +
                             (tuVanThuocs[i].DungToi != null
                                 ? "Tối " + ((tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ?
                                                        NumberHelper.ChuyenSoRaText(Convert.ToDouble(tuVanThuocs[i].DungToi), false) :
                                                       (Convert.ToDouble(tuVanThuocs[i].DungToi) < 10 && Convert.ToInt32(tuVanThuocs[i].DungToi) == tuVanThuocs[i].DungToi) ? "0" + tuVanThuocs[i].DungToi.FloatToStringFraction() + " " : tuVanThuocs[i].DungToi.FloatToStringFraction() + " ") +
                                   (!string.IsNullOrEmpty(lstThoiGian[i].ThoiGianDungToiDisplay)
                                       ? " " + lstThoiGian[i].ThoiGianDungToiDisplay
                                       : "") + " " + tuVanThuocs[i].DonViTinh?.Ten + ","
                                 : "");

                    var cachDung = tuVanThuocs[i].DuongDung?.Ten + " " + (!string.IsNullOrEmpty(cd) ? cd.Trim().Remove(cd.Trim().Length - 1) : "") + " " + tuVanThuocs[i].GhiChu;

                    sttBHYT++;
                    resultThuoc += "<tr>";
                    resultThuoc += "<td rowspan='2' align='center'>" + sttBHYT + "</td>";
                    resultThuoc += "<td>" + FormatTenDuocPham(tuVanThuocs[i].Ten, tuVanThuocs[i].HoatChat, tuVanThuocs[i].HamLuong, tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhomId) + "</td>";
                    resultThuoc += "<td rowspan='2' align='center'>" + FormatSoLuong(tuVanThuocs[i].SoLuong, tuVanThuocs[i].DuocPham?.DuocPhamBenhVien?.LoaiThuocTheoQuanLy) + " " +
                                       tuVanThuocs[i].DonViTinh?.Ten + "</td>";
                    resultThuoc += "</tr>";
                    resultThuoc += "<tr>";
                    resultThuoc += "<td><i>" + cachDung + "&nbsp;</i></td>";
                    resultThuoc += "</tr>";
                }
                if (!string.IsNullOrEmpty(resultThuoc))
                {
                    resultThuoc = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>TÊN THUỐC – HÀM LƯỢNG</th><th>SỐ LƯỢNG</th></tr></thead><tbody>" + resultThuoc + "</tbody></table>";
                    var data = new DataYCKBDonThuoc
                    {
                        TemplateDonThuoc = resultThuoc,
                        LogoUrl = inTuVanThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                        MaTN = infoBN.MaTN,
                        HoTen = infoBN.HoTen,
                        Tuoi = infoBN.Tuoi,
                        DiaChi = infoBN.DiaChi,
                        NamSinh = infoBN.NamSinh,
                        GioiTinh = infoBN.GioiTinh,
                        BacSiKham = getTenBSKham,
                        LoiDan = infoBN.LoiDan,
                        MaBN = infoBN.MaBN,
                        SoDienThoai = infoBN.SoDienThoai,
                        CongKhoan = sttBHYT,
                        KhoaPhong = khoaPhong
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                }
            }
            return content;
        }
    }
}
