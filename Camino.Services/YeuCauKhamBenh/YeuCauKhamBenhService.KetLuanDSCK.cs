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
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.CauHinh;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {
        public GridDataSource GetDataForGridKeToa(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKhamBenhId = long.Parse(queryInfo.AdditionalSearchString);
            var lstDuocPham = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
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

            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                .Where(o => o.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == yeuCauKhamBenhId && o.SoLuong > 0)
                .OrderBy(o => o.YeuCauKhamBenhDonThuoc.LoaiDonThuoc)
                .Select(s => new DonThuocChiTietGridVoItem
                {
                    STT = s.SoThuTu,
                    Id = s.Id,
                    DuocPhamId = s.DuocPhamId,
                    YeuCauKhamBenhId = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId,
                    YeuCauKhamBenhDonThuocId = s.YeuCauKhamBenhDonThuocId,
                    MaHoatChat = s.DuocPham.MaHoatChat,
                    Ma = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.Ma : s.DuocPham.MaHoatChat,
                    Ten = s.DuocPham.Ten,
                    HoatChat = s.DuocPham.HoatChat,
                    HamLuong = s.DuocPham.HamLuong,
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
                    LieuDungTrenNgay = s.LieuDungTrenNgay,
                    SoLanTrenVien = s.SoLanTrenVien,
                    ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                    SoNgayDung = s.SoNgayDung,
                    SoLuong = s.SoLuong,
                    SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                    TenDuongDung = s.DuongDung.Ten,
                    DuongDungId = s.DuongDungId,

                    #region cập nhật 08/12/2022 fix select chậm
                    //DonGiaNhap = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().DonGiaNhap : 0,
                    //DonGia = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().DonGiaBan : 0,
                    //TiLeTheoThapGia = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().TiLeTheoThapGia : 0,
                    //VAT = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().VAT : 0,

                    DonGiaNhap = 0,
                    DonGia = 0,
                    TiLeTheoThapGia = 0,
                    VAT = 0,
                    #endregion

                    LoaiDonThuoc = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc,
                    ThuocBHYT = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? "Có" : "Không",

                    #region cập nhật 08/12/2022 fix select chậm
                    //DiUngThuocDisplay = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.DuocPham.MaHoatChat && diung.LoaiDiUng == Enums.LoaiDiUng.Thuoc) ? "Có" : "Không",
                    //TuongTacThuoc = GetTuongTac(s.DuocPham.MaHoatChat, lstDuocPham, lstADR),

                    BenhNhanId = s.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId,
                    #endregion
                    GhiChu = s.GhiChu,
                    GhiChuDonThuoc = s.YeuCauKhamBenhDonThuoc.GhiChu,
                    NhomId = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? 1 : 0,  //  1 thuoc BHYT , 0 : Khong BHYT,
                    Nhom = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? "BHYT" : "Không BHYT",
                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy : null,

                    //BVHD-3905
                    TiLeThanhToanBHYT = s.DuocPham.DuocPhamBenhVien.TiLeThanhToanBHYT
                });


            #region Cập nhật 08/12/2022 fix bug phân trang
            //var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            #endregion

            //BVHD-3959            
            //var queryTask = query.OrderBy(queryInfo.SortString).ThenBy(z => z.DuongDungNumber).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArray();

            #region Cập nhật 08/12/2022 fix bug phân trang
            //var allData = query.ToList();
            var allData = query.OrderBy(queryInfo.SortString).ThenBy(z => z.DuongDungNumber).ToList();
            var countTask = query.Count();
            #endregion

            #region Cập nhật 08/12/2022 fix select chậm
            if (allData.Any())
            {
                #region đơn thuốc thanh toán
                var lstYCKhamDonThuocChiTietId = allData.Select(x => x.Id).Distinct().ToList();
                var lstDonThuocThanhToanChiTiet = _donThuocThanhToanChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauKhamBenhDonThuocChiTietId != null
                                && lstYCKhamDonThuocChiTietId.Contains(x.YeuCauKhamBenhDonThuocChiTietId.Value))
                    .Select(x => new
                    {
                        YeuCauKhamBenhDonThuocChiTietId = x.YeuCauKhamBenhDonThuocChiTietId,
                        DonGiaNhap = x.DonGiaNhap,
                        DonGiaBan = x.DonGiaBan,
                        TiLeTheoThapGia = x.TiLeTheoThapGia,
                        VAT = x.VAT
                    }).ToList();
                #endregion

                #region dị ứng thuốc
                var lstBenhNhanId = allData.Where(x => x.BenhNhanId.GetValueOrDefault() != 0).Select(x => x.BenhNhanId.Value).Distinct().ToList();
                var lstBenhNhanDiUngThuoc = _benhNhanDiUngThuocRepository.TableNoTracking
                        .Where(x => x.LoaiDiUng == Enums.LoaiDiUng.Thuoc
                                    && lstBenhNhanId.Contains(x.BenhNhanId))
                        .Select(x => new
                        {
                            BenhNhanId = x.BenhNhanId,
                            TenDiUng = x.TenDiUng
                        }).ToList();
                #endregion

                allData.ForEach(x =>
                {
                    var donThuocThanhToanChiTiet = lstDonThuocThanhToanChiTiet.FirstOrDefault(a => a.YeuCauKhamBenhDonThuocChiTietId == x.Id);
                    if(donThuocThanhToanChiTiet != null)
                    {
                        x.DonGiaNhap = donThuocThanhToanChiTiet.DonGiaNhap;
                        x.DonGia = donThuocThanhToanChiTiet.DonGiaBan;
                        x.TiLeTheoThapGia = donThuocThanhToanChiTiet.TiLeTheoThapGia;
                        x.VAT = donThuocThanhToanChiTiet.VAT;
                    }

                    var diUngThuoc = lstBenhNhanDiUngThuoc.Where(a => x.BenhNhanId == a.BenhNhanId).Any(a => a.TenDiUng.Trim().ToLower() == x.MaHoatChat.Trim().ToLower());
                    x.DiUngThuocDisplay = diUngThuoc ? "Có" : "Không";

                    x.TuongTacThuoc = GetTuongTac(x.MaHoatChat, lstDuocPham, lstADR);
                });
            }
            #endregion

            //return new GridDataSource { Data = allData.AsQueryable().OrderBy(queryInfo.SortString).ThenBy(z => z.DuongDungNumber).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = countTask };
            return new GridDataSource { Data = allData.ToArray(), TotalRowCount = countTask };

        }

        public GridDataSource GetTotalPageForGridKeToa(QueryInfo queryInfo)
        {
            return null;
        }

        private string GetTuongTac(string MaHoatChat, List<string> lstDuocPham, List<MaHoatChatGridVo> lstADR)
        {
            var TuongTac = string.Empty;
            if (lstADR.Count > 0)
            {
                foreach (var item in lstADR)
                {
                    if (item.MaHoatChat1 == MaHoatChat && lstDuocPham.Where(p => p != MaHoatChat).Contains(item.MaHoatChat2))
                    {
                        TuongTac += item.Ten2 + "; ";
                    }
                    if ((item.MaHoatChat2 == MaHoatChat && lstDuocPham.Where(p => p != MaHoatChat).Contains(item.MaHoatChat1)))
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
        public GetDuocPhamTonKhoGridVoItem GetDuocPhamInfoById(ThongTinThuocVo thongTinThuocVo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var yeuCauKhamBenh = BaseRepository.GetById(thongTinThuocVo.YeuCauKhamBenhId, x => x.Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets)
                                                                                .Include(o => o.YeuCauTiepNhan).ThenInclude(yc => yc.BenhNhan).ThenInclude(bn => bn.BenhNhanDiUngThuocs));

            var lstDuocPham = yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.SelectMany(o => o.YeuCauKhamBenhDonThuocChiTiets)
                .Select(o => o.MaHoatChat).ToList();

            var lstADR = _aDRRepository.TableNoTracking
                           .Select(s => new MaHoatChatGridVo
                           {
                               Ten1 = s.ThuocHoacHoatChat1.Ten,
                               Ten2 = s.ThuocHoacHoatChat2.Ten,
                               MaHoatChat1 = s.ThuocHoacHoatChat1.Ma,
                               MaHoatChat2 = s.ThuocHoacHoatChat2.Ma
                           }).ToList();
            var duocPhamInfo = _duocPhamRepository.TableNoTracking.Include(o => o.DuongDung)
                .Where(o => o.Id == thongTinThuocVo.DuocPhamId)
                .Select(d => new GetDuocPhamTonKhoGridVoItem
                {
                    Id = d.Id,
                    TuongTacThuoc = GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR) == string.Empty ? "Không" : GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR),
                    FlagTuongTac = !GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR).Contains("Không") ? true : false,
                    FlagDiUng = d.HoatChat != null && yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng.Contains(d.HoatChat) && diung.LoaiDiUng == Enums.LoaiDiUng.Thuoc),
                    FlagThuocDaKe = yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.SelectMany(dt => dt.YeuCauKhamBenhDonThuocChiTiets).Any(dtct => dtct.MaHoatChat == d.MaHoatChat),
                    DVTId = d.DonViTinh.Id,
                    TenDonViTinh = d.DonViTinh.Ten,
                    DuongDungId = d.DuongDung.Id,
                    TenDuongDung = d.DuongDung.Ten,
                    TonKho = Math.Round(d.DuocPhamBenhVien == null || !(thongTinThuocVo.LoaiDuocPham == 1 || thongTinThuocVo.LoaiDuocPham == 2) ? 0 :
                                (d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => (nkct.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || nkct.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT) && nkct.LaDuocPhamBHYT == (thongTinThuocVo.LoaiDuocPham == 1) &&
                                                                                          nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat)), 1),
                    HanSuDung = d.DuocPhamBenhVien == null || !(thongTinThuocVo.LoaiDuocPham == 1 || thongTinThuocVo.LoaiDuocPham == 2) ? (DateTime?)null :
                                (d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                        .Where(nkct => (nkct.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || nkct.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT) && nkct.LaDuocPhamBHYT == (thongTinThuocVo.LoaiDuocPham == 1) && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                        .Select(o => o.HanSuDung).FirstOrDefault()),
                    HamLuong = d.HamLuong,
                    NhaSanXuat = d.NhaSanXuat,
                    DuongDung = d.DuongDung != null ? d.DuongDung.Ten : ""
                }).FirstOrDefault();
            return duocPhamInfo;
        }

        public VatTuTrongKhoVo GetVatTuInfoById(ThongTinVatTuVo thongTinThuocVo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var yeuCauKhamBenh = BaseRepository.GetById(thongTinThuocVo.YeuCauKhamBenhId, x => x.Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets));

            var vatTuInfo = _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => o.Id == thongTinThuocVo.VatTuId && o.NhapKhoVatTuChiTiets.Any(kho =>
                                kho.NhapKhoVatTu.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc && kho.LaVatTuBHYT == false &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .Select(s => new VatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                DonViTinh = s.VatTus.DonViTinh,
                                FlagVatTuDaKe = yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.SelectMany(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).Any(dvtct => dvtct.VatTuBenhVienId == thongTinThuocVo.VatTuId),
                                TonKho =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            nkct.NhapKhoVatTu.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc &&
                                            nkct.LaVatTuBHYT == false && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        nkct.NhapKhoVatTu.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc &&
                                        nkct.LaVatTuBHYT == false && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First()
                            }).FirstOrDefault();
            return vatTuInfo;
        }

        public async Task<List<DuocPhamVaVatTuTemplate>> GetDuocPhamVaVatTuKeToaAsync(DropDownListRequestModel queryInfo, bool laNoiTruDuocPham = false)
        {
            long loaiDuocPham = int.Parse(queryInfo.ParameterDependencies);// 1:thuoc BHYT, 2:Thuoc khong BHYT->thuoc trong BV, 3:Thuoc khong BHYT->thuoc ngoai BV
            _logger.LogInfo($"GetDuocPhamVaVatTuKeToaAsync loaiDuocPham:{loaiDuocPham}, Query: {queryInfo.Query}");
            if (loaiDuocPham == 1)
            {
                var duocPhamVaVatTus = await _duocPhamVaVatTuBenhVienService.GetDuocPhamVaVatTuTrongNhieuKho(true, queryInfo.Query, queryInfo.Take, (long)Enums.EnumKhoDuocPham.KhoThuocBHYT);
                if (laNoiTruDuocPham)
                {
                    duocPhamVaVatTus = duocPhamVaVatTus.Where(z => z.LoaiDuocPhamHoacVatTu != Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).ToList();
                }
                return duocPhamVaVatTus.Select(s => new DuocPhamVaVatTuTemplate
                {
                    //DisplayName = s.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                    //    ? s.Ten + " - " + s.HoatChat
                    //    : s.Ten,
                    DisplayName = s.Ten,
                    KeyId = s.Id,
                    Ten = s.Ten,
                    HoatChat = s.HoatChat,
                    DVT = s.DonViTinh,
                    SLTon = Math.Round(s.SoLuongTon, 1),
                    HanSuDung = s.HanSuDung?.ApplyFormatDate(),
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu,
                    TenLoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? "Dược phẩm" : "Vật tư",
                    HamLuong = s.HamLuong,
                    NhaSanXuat = s.NhaSanXuat,
                    DuongDung = s.DuongDung,
                    LoaiThuocTheoQuanLy = s.LoaiThuocTheoQuanLy
                }).ToList();
            }
            if (loaiDuocPham == 2)
            {
                var duocPhamVaVatTus = await _duocPhamVaVatTuBenhVienService.GetDuocPhamVaVatTuTrongNhieuKho(false, queryInfo.Query, queryInfo.Take, (long)Enums.EnumKhoDuocPham.KhoNhaThuoc);
                if (laNoiTruDuocPham)
                {
                    duocPhamVaVatTus = duocPhamVaVatTus.Where(z => z.LoaiDuocPhamHoacVatTu != Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).ToList();
                }
                return duocPhamVaVatTus.Select(s => new DuocPhamVaVatTuTemplate
                {
                    //DisplayName = s.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                    //    ? s.Ten + " - " + s.HoatChat
                    //    : s.Ten,
                    DisplayName = s.Ten,
                    KeyId = s.Id,
                    Ten = s.Ten,
                    HoatChat = s.HoatChat,
                    DVT = s.DonViTinh,
                    SLTon = Math.Round(s.SoLuongTon, 1),
                    HanSuDung = s.HanSuDung?.ApplyFormatDate(),
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu,
                    TenLoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? "Dược phẩm" : "Vật tư",
                    HamLuong = s.HamLuong,
                    NhaSanXuat = s.NhaSanXuat,
                    DuongDung = s.DuongDung,
                    LoaiThuocTheoQuanLy = s.LoaiThuocTheoQuanLy
                }).ToList();
            }
            if (loaiDuocPham == 3)
            {
                var lstColumnNameSearch = new List<string>
                {
                    nameof(DuocPham.Ten),
                    nameof(DuocPham.HoatChat)
                };
                if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
                {
                    var query = _duocPhamRepository.TableNoTracking.Include(o => o.DuongDung)
                        .Select(s => new DuocPhamVaVatTuTemplate
                        {
                            LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                            //DisplayName = s.Ten + " - " + s.HoatChat,
                            DisplayName = s.Ten,
                            KeyId = s.Id,
                            Ten = s.Ten,
                            HoatChat = s.HoatChat,
                            DVT = s.DonViTinh != null ? s.DonViTinh.Ten : null,
                            HamLuong = s.HamLuong,
                            NhaSanXuat = s.NhaSanXuat,
                            DuongDung = s.DuongDung != null ? s.DuongDung.Ten : "",
                            LoaiThuocTheoQuanLy = s.DuocPhamBenhVien != null ? s.DuocPhamBenhVien.LoaiThuocTheoQuanLy : null
                        })
                        .ApplyLike(queryInfo.Query, p => p.HoatChat, p => p.Ten, p => p.DisplayName)
                        .Take(queryInfo.Take);
                    return await query.ToListAsync();
                }
                else
                {
                    var query = _duocPhamRepository.ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch).Include(o => o.DuongDung)
                          .Select(s => new DuocPhamVaVatTuTemplate
                          {
                              LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                              //DisplayName = s.Ten + " - " + s.HoatChat,
                              DisplayName = s.Ten,
                              KeyId = s.Id,
                              Ten = s.Ten,
                              HoatChat = s.HoatChat,
                              DVT = s.DonViTinh != null ? s.DonViTinh.Ten : null,
                              HamLuong = s.HamLuong,
                              NhaSanXuat = s.NhaSanXuat,
                              DuongDung = s.DuongDung != null ? s.DuongDung.Ten : "",
                              LoaiThuocTheoQuanLy = s.DuocPhamBenhVien != null ? s.DuocPhamBenhVien.LoaiThuocTheoQuanLy : null

                          })
                          .Take(queryInfo.Take);
                    return await query.ToListAsync();
                }

            }
            return null;
        }

        public async Task<ThongTinDichVuKhamTiepTheo> GiaBenhVienAsync(ThongTinDichVuKhamTiepTheo thongTinDichVuKhamTiepTheo)
        {
            // BVHD-3575: cập nhật dùng chung cho chỉ định dv khám từ nội trú
            // chỉ định từ nội trú sẽ gán mặc định YeuCauKhamBenhId = 0
            var thongTinKham = await BaseRepository.TableNoTracking
                .Include(x => x.YeuCauTiepNhan)
                .Where(x => x.Id == thongTinDichVuKhamTiepTheo.YeuCauKhamBenhId && x.YeuCauTiepNhanId == thongTinDichVuKhamTiepTheo.YeuCauTiepNhanId).FirstOrDefaultAsync();
            var giaBH = await _dichVuKhamBenhBenhVienGiaBaoHiemGiaRepository.TableNoTracking
                .Where(x => x.DichVuKhamBenhBenhVienId == thongTinDichVuKhamTiepTheo.DichVuKhamBenhId &&
                            x.TuNgay <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay >= DateTime.Now.Date))
                .FirstOrDefaultAsync();

            //BVHD-3575
            if (thongTinKham == null)
            {
                var yeuCauTiepNhan = await _yeuCauTiepNhanRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == thongTinDichVuKhamTiepTheo.YeuCauTiepNhanId);
                thongTinDichVuKhamTiepTheo.FlagDuocHuongBaoHiem = yeuCauTiepNhan.CoBHYT == true && giaBH != null;
            }
            else
            {
                thongTinDichVuKhamTiepTheo.FlagDuocHuongBaoHiem = thongTinKham.YeuCauTiepNhan.CoBHYT == true && giaBH != null && thongTinKham.DuocHuongBaoHiem;
            }

            //thongTinDichVuKhamTiepTheo.FlagDuocHuongBaoHiem = thongTinKham.YeuCauTiepNhan.CoBHYT == true && giaBH != null && thongTinKham.DuocHuongBaoHiem;
            if (thongTinDichVuKhamTiepTheo.FlagDuocHuongBaoHiem)
            {
                thongTinDichVuKhamTiepTheo.DuocHuongBaoHiem = true;
            }

            DichVuKhamBenhBenhVienGiaBenhVien giaBenhVien = null;
            if (thongTinDichVuKhamTiepTheo.LoaiGiaId != 0)
            {
                giaBenhVien = await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                    .Where(o => o.NhomGiaDichVuKhamBenhBenhVienId == thongTinDichVuKhamTiepTheo.LoaiGiaId
                                && o.DichVuKhamBenhBenhVienId == thongTinDichVuKhamTiepTheo.DichVuKhamBenhId
                                && o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay))
                    .FirstOrDefaultAsync();
            }

            thongTinDichVuKhamTiepTheo.GiaDichVuKham = giaBenhVien == null ? 0 : giaBenhVien.Gia;
            //thongTinDichVuKhamTiepTheo.MucHuong = 100;
            //if (thongTinDichVuKhamTiepTheo.DuocHuongBaoHiem && thongTinTiepNhan != null && thongTinTiepNhan.CoBHYT == true && giaBH != null)
            //{
            //    var lstLanKhamHuongBHTrongNgay = await _yeuCauTiepNhanRepository.TableNoTracking.Where(x => x.Id == thongTinDichVuKhamTiepTheo.YeuCauTiepNhanId)
            //                                                    .SelectMany(x => x.YeuCauKhamBenhs.Where(y => y.DuocHuongBaoHiem)).ToListAsync();
            //    if (lstLanKhamHuongBHTrongNgay.Any())
            //    {
            //        thongTinDichVuKhamTiepTheo.LanKhamCoBHTrongNgay = lstLanKhamHuongBHTrongNgay.FindIndex(x => x.Id == thongTinDichVuKhamTiepTheo.YeuCauKhamBenhId) + 2; // + 1 index start from 0; +1 yeucaukhamtieptheo

            //        if (thongTinDichVuKhamTiepTheo.LanKhamCoBHTrongNgay > 0 && thongTinDichVuKhamTiepTheo.LanKhamCoBHTrongNgay < 6)
            //        {
            //            switch (thongTinDichVuKhamTiepTheo.LanKhamCoBHTrongNgay)
            //            {
            //                case 1:
            //                    thongTinDichVuKhamTiepTheo.TiLeBaoHiemThanhToan = 1;
            //                    break;
            //                case 2:
            //                case 3:
            //                case 4:
            //                    thongTinDichVuKhamTiepTheo.TiLeBaoHiemThanhToan = decimal.Parse("0.3");
            //                    break;
            //                case 5:
            //                    thongTinDichVuKhamTiepTheo.TiLeBaoHiemThanhToan = decimal.Parse("0.1");
            //                    break;
            //                default:
            //                    thongTinDichVuKhamTiepTheo.TiLeBaoHiemThanhToan = 0;
            //                    break;
            //            }

            //            thongTinDichVuKhamTiepTheo.GiaBHTT = (giaBH.Gia * (decimal)giaBH.TiLeBaoHiemThanhToan / 100) * thongTinDichVuKhamTiepTheo.TiLeBaoHiemThanhToan * (decimal)thongTinDichVuKhamTiepTheo.MucHuong / 100;
            //            if (thongTinDichVuKhamTiepTheo.GiaBHTT > thongTinDichVuKhamTiepTheo.SoTienBHTTToanBo)
            //            {
            //                thongTinDichVuKhamTiepTheo.MucHuong = thongTinTiepNhan.BHYTMucHuong ?? 0;
            //                thongTinDichVuKhamTiepTheo.GiaBHTT = (giaBH.Gia * (decimal)giaBH.TiLeBaoHiemThanhToan / 100) * thongTinDichVuKhamTiepTheo.TiLeBaoHiemThanhToan * (decimal)thongTinDichVuKhamTiepTheo.MucHuong / 100;
            //            }
            //        }
            //    }
            //}
            return thongTinDichVuKhamTiepTheo;
        }

        public async Task<List<ICDTemplateVo>> GetICDs(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(ICD.Ma),
                nameof(ICD.TenTiengViet),
            };
            var lstICDs = new List<ICDTemplateVo>();
            var chanDoanICDId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstICDs = await _iCDRepository.TableNoTracking
                    .Where(x => x.Id == chanDoanICDId || x.HieuLuc == true)
                    .Select(item => new ICDTemplateVo
                    {
                        DisplayName = item.Ma + " - " + item.TenTiengViet, //item.TenTiengViet,
                        KeyId = item.Id,
                        Ten = item.TenTiengViet,
                        Ma = item.Ma,
                    })
                    .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                    .OrderByDescending(x => x.KeyId == chanDoanICDId).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstICDId = await _iCDRepository
                    .ApplyFulltext(queryInfo.Query, nameof(ICD), lstColumnNameSearch)
                    .Where(x => x.Id == chanDoanICDId || x.HieuLuc == true)
                    .Select(x => x.Id)
                    .ToListAsync();
                lstICDs = await _iCDRepository.TableNoTracking
                    .Where(x => lstICDId.Contains(x.Id))
                    .OrderByDescending(x => x.Id == chanDoanICDId)
                    .ThenBy(p => lstICDId.IndexOf(p.Id) != -1 ? lstICDId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new ICDTemplateVo
                    {
                        DisplayName = item.Ma + " - " + item.TenTiengViet, //item.TenTiengViet,
                        KeyId = item.Id,
                        Ten = item.TenTiengViet,
                        Ma = item.Ma,
                    }).ToListAsync();
            }
            return lstICDs;

        }

        public async Task<string> GetLoiDanTheoICD(long iCDId)
        {
            return await _iCDRepository.TableNoTracking.Where(p => p.Id == iCDId).Select(p => p.LoiDanCuaBacSi).FirstOrDefaultAsync();
        }
        public async Task<List<MaDichVuTemplateVo>> GetDichVuKhamBenh(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstEntity = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh)
                    .Where(p => p.HieuLuc)
                    .Select(p => new MaDichVuTemplateVo
                    {
                        DisplayName = p.Ten,//p.Ma + " - " + p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        KeyId = p.Id,
                    })
                    .Where(s => s.Ten.Contains(model.Query ?? "") || s.Ma.Contains(model.Query ?? ""))
                    .Take(model.Take)
                    .ToListAsync();
                return lstEntity;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");

                var lstEntity = await _dichVuKhamBenhBenhVienRepository
                    .ApplyFulltext(model.Query, nameof(DichVuKhamBenhBenhVien), lstColumnNameSearch)
                    .Where(x => x.HieuLuc)
                    .Take(model.Take)
                    .Select(p => new MaDichVuTemplateVo
                    {
                        DisplayName = p.Ten,//p.Ma + " - " + p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        KeyId = p.Id,
                    })
                    .ToListAsync();
                return lstEntity;
            }
        }

        public async Task<List<NhanVienHoTongTemplateVo>> GetNhanVienHoTongs(DropDownListRequestModel queryInfo)
        {
            var nhanViens = _nhanVienRepository.TableNoTracking
                            .Where(p => p.ChucDanh.IsDisabled != true && p.VanBangChuyenMon.IsDisabled != true)
                            .Select(nv => new NhanVienHoTongTemplateVo
                            {
                                DisplayName = nv.User.HoTen + (nv.ChucDanh != null ? " - " + nv.ChucDanh.NhomChucDanh.Ten : "")
                                                    + (nv.VanBangChuyenMon != null ? " - " + nv.VanBangChuyenMon.Ten : ""),
                                TenNhanVien = nv.User.HoTen,
                                TenNhomChucDanh = nv.ChucDanh.NhomChucDanh.Ten,
                                TenVanBang = nv.VanBangChuyenMon.Ten,
                                KeyId = nv.Id,
                            })
                            .ApplyLike(queryInfo.Query, o => o.TenNhanVien, o => o.TenNhomChucDanh, o => o.TenVanBang, o => o.DisplayName)
                            .Take(queryInfo.Take);

            return await nhanViens.ToListAsync();
        }

        public async Task<List<LookupItemTemplateVo>> GetNoiThucHiens(DropDownListRequestModel queryInfo, string selectedItems = null)
        {
            var dichVuKhamBenhBenhVienId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var noiThucHiens = await _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking
                    .Where(p => p.DichVuKhamBenhBenhVienId == dichVuKhamBenhBenhVienId)
                    .ToListAsync();
            var lstKhoaPhongKhamId = new List<long>();
            var lstPhongBenhVienId = new List<long>();

            if (dichVuKhamBenhBenhVienId != 0 && noiThucHiens.Any())
            {
                lstKhoaPhongKhamId = noiThucHiens.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhongId.Value).ToList();
                lstPhongBenhVienId = noiThucHiens.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList();

                if (lstKhoaPhongKhamId.Any())
                {
                    var lstPhongIdTheoKhoa = await _phongBenhVienRepository.TableNoTracking
                        .Where(x => x.IsDisabled != true && lstKhoaPhongKhamId.Any(y => y == x.KhoaPhongId))
                        .Select(x => x.Id).ToListAsync();
                    lstPhongBenhVienId.AddRange(lstPhongIdTheoKhoa);
                }
            }
            else
            {
                lstPhongBenhVienId.AddRange(_phongBenhVienRepository.TableNoTracking.Where(p => p.IsDisabled != true)
                    .Take(queryInfo.Take).Select(p => p.Id));
            }

            if (queryInfo.Id != 0)
            {
                lstPhongBenhVienId.Add(queryInfo.Id);
            }

            //multiselect
            var selectedItemStrs = new List<long>();
            if (!string.IsNullOrEmpty(selectedItems))
            {
                selectedItemStrs = selectedItems.Split(",").Select(long.Parse).ToList();
            }

            var result = await _phongBenhVienRepository.TableNoTracking
                .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                .Where(x => x.IsDisabled != true && (lstPhongBenhVienId.Any(y => y == x.Id) || selectedItemStrs.Any(z => z == x.Id)))
                .Distinct()
                .OrderByDescending(x => x.Id == queryInfo.Id || selectedItemStrs.Any(z => z == x.Id)).ThenBy(x => x.Id)
                .Take(queryInfo.Take)
                .Select(p => new LookupItemTemplateVo
                {
                    DisplayName = p.Ten, //p.Ma + " - " + p.Ten
                    KeyId = p.Id,
                    Ten = p.Ten,
                    Ma = p.Ma
                })
                .ToListAsync();
            return result;
        }
        public async Task<List<LookupItemVo>> GetBacSiKhams(DropDownListRequestModel queryInfo)
        {
            //var phongBenhVienId = queryInfo.Id != 0 ? queryInfo.Id : CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var phongBenhVienId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var lstNhanVien = await _hoatDongNhanVienRepository.TableNoTracking
                        .Include(hd => hd.NhanVien).ThenInclude(nv => nv.User)
                        .Include(hd => hd.NhanVien).ThenInclude(nv => nv.ChucDanh)
                        .Where(hd => hd.PhongBenhVienId == phongBenhVienId && hd.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi)
                        .Include(hd => hd.PhongBenhVien)
                        .Select(hd => hd.NhanVien)
                        .ApplyLike(queryInfo.Query, g => g.User.HoTen)
                        .Distinct().Take(queryInfo.Take)
                        .Select(s => new LookupItemVo
                        {
                            DisplayName = s.User.HoTen,
                            KeyId = s.Id
                        })
                        .ToListAsync();
            return lstNhanVien;
        }

        public async Task<List<ICDKhacsTemplateVo>> GetICDKhacs(DropDownListRequestModel model)
        {
            var yeuCauKhamBenhId = int.Parse(model.ParameterDependencies);
            return await _yeuCauKhamBenhICDKhacRepository.TableNoTracking
                .Where(dv => dv.YeuCauKhamBenhId == yeuCauKhamBenhId && dv.ICD.HieuLuc == true)
                .Select(item => new ICDKhacsTemplateVo
                {
                    YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                    TenICD = item.ICD.Ma + " - " + item.ICD.TenTiengViet,
                    //TenICD = item.ICD.TenTiengViet,
                    ICDId = item.ICDId,
                    GhiChu = item.GhiChu,
                    Id = item.Id
                }).ApplyLike(model.Query, o => o.TenICD, o => o.GhiChu)
                .Take(model.Take).ToListAsync();
        }

        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuatBenhViens(DropDownListRequestModel model)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien.Ten),
                nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien.Ma)
            };
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                if (!string.IsNullOrEmpty(model.ParameterDependencies))
                {
                    var lstDichVuKyThuatBV = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .OrderByDescending(x => long.Parse(model.ParameterDependencies) == 0 || x.Id == long.Parse(model.ParameterDependencies)).ThenBy(x => x.Id)
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        //DisplayName = item.Ma + " - " + item.Ten,
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma,
                    })
                    .ApplyLike(model.Query, o => o.Ma, o => o.DichVu)
                    .Take(model.Take)
                    .ToListAsync();
                    return lstDichVuKyThuatBV;
                }
                else
                {
                    var lstDichVuKyThuatBV = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .OrderByDescending(x => model.Id == 0 || x.Id == model.Id).ThenBy(x => x.Id)
                    .Select(item => new DichVuKyThuatTemplateVo
                    {
                        //DisplayName = item.Ma + " - " + item.Ten,
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        DichVu = item.Ten,
                        Ma = item.Ma,
                    })
                    .ApplyLike(model.Query, o => o.Ma, o => o.DichVu)
                    .Take(model.Take)
                    .ToListAsync();
                    return lstDichVuKyThuatBV;
                }

            }
            else
            {
                var lstId = _dichVuKyThuatBenhVienRepository
               .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien), lstColumnNameSearch)
               .Select(p => p.Id).ToList();
                var dct = lstId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                var lst = _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(dv => dv.HieuLuc == true)
                    .Where(p => lstId.Any(x => x == p.Id));

                var lstDichVuKyThuatBenhVien = await lst.Select(item => new DichVuKyThuatTemplateVo
                {
                    //DisplayName = item.Ma + " - " + item.Ten,
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    DichVu = item.Ten,
                    Ma = item.Ma,
                })
                    .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                    .Take(model.Take)
                    .ToListAsync();
                return lstDichVuKyThuatBenhVien;
            }
        }
        public async Task<List<LookupItemVo>> GetLoaiGia()
        {
            var result = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).Distinct().ToListAsync();
            await Task.WhenAll(result);
            return result.Result;
        }

        public async Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuKham(DropDownListRequestModel model)
        {
            var dichVuKhamId = CommonHelper.GetIdFromRequestDropDownList(model);
            //var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKhamBenh.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var result = await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => o.NhomGiaDichVuKhamBenhBenhVienId == model.Id
                                || (o.DichVuKhamBenhBenhVienId == dichVuKhamId
                                    && o.TuNgay.Date <= DateTime.Now.Date
                                    && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                                )
                .OrderByDescending(x => x.NhomGiaDichVuKhamBenhBenhVienId == nhomGiaThuongId)
                .ThenBy(x => x.CreatedOn)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    KeyId = item.NhomGiaDichVuKhamBenhBenhVienId,
                }).Distinct().ToListAsync();
            //var result = _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .Select(item => new LookupItemVo
            //    {
            //        DisplayName = item.Ten,
            //        KeyId = item.Id,
            //    }).Distinct().ToListAsync();
            return result;
        }

        public async Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuKyThuat(DropDownListRequestModel model)
        {
            var dichKyThuatId = CommonHelper.GetIdFromRequestDropDownList(model);
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var result = await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => o.NhomGiaDichVuKyThuatBenhVienId == model.Id
                                || (o.DichVuKyThuatBenhVienId == dichKyThuatId
                                    && o.TuNgay.Date <= DateTime.Now.Date
                                    && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                                )
                .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                .ThenBy(x => x.CreatedOn)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                    KeyId = item.NhomGiaDichVuKyThuatBenhVienId,
                }).Distinct().ToListAsync();
            return result;
        }

        public async Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuGiuong(DropDownListRequestModel model)
        {
            var dichVuGiuongId = CommonHelper.GetIdFromRequestDropDownList(model);
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuGiuong.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var result = await _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => o.NhomGiaDichVuGiuongBenhVienId == model.Id
                            || (o.DichVuGiuongBenhVienId == dichVuGiuongId
                                && o.TuNgay.Date <= DateTime.Now.Date
                                && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                )
                .OrderByDescending(x => x.NhomGiaDichVuGiuongBenhVienId == nhomGiaThuongId)
                .ThenBy(x => x.CreatedOn)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.NhomGiaDichVuGiuongBenhVien.Ten,
                    KeyId = item.NhomGiaDichVuGiuongBenhVienId,
                }).Distinct().ToListAsync();
            return result;
        }

        public async Task<List<LookupItemVo>> GetKhoaPhongNhapViens(DropDownListRequestModel queryInfo)
        {
            var result = _khoaPhongRepository.TableNoTracking
                .Where(c => c.CoKhamNoiTru == true)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                })
                .ApplyLike(queryInfo.Query, g => g.DisplayName)
                .Take(queryInfo.Take).ToListAsync();
            await Task.WhenAll(result);
            return result.Result;
        }

        public async Task<List<LookupItemVo>> GetBenhVienChuyenViens(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.BenhVien.BenhVien.Ten)
            };
            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var benhVienChuyenVienId = long.Parse(queryInfo.ParameterDependencies);
                    var lstBenhVien = await _benhVienRepository.TableNoTracking
                   .OrderByDescending(x => benhVienChuyenVienId == 0 || x.Id == benhVienChuyenVienId).ThenBy(x => x.Ten)
                   .Select(item => new LookupItemVo
                   {
                       DisplayName = item.Ten,
                       KeyId = item.Id,
                   })
                  .ApplyLike(queryInfo.Query, o => o.DisplayName)
                  .Take(queryInfo.Take)
                  .ToListAsync();
                    return lstBenhVien;
                }
                else
                {
                    var lstBenhVien = await _benhVienRepository.TableNoTracking
                   .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Ten)
                   .Select(item => new LookupItemVo
                   {
                       DisplayName = item.Ten,
                       KeyId = item.Id,
                   })
                  .ApplyLike(queryInfo.Query, o => o.DisplayName)
                  .Take(queryInfo.Take)
                  .ToListAsync();
                    return lstBenhVien;
                }
            }
            else
            {
                var lstBenhVien = await
                  _benhVienRepository.ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.BenhVien.BenhVien), lstColumnNameSearch)
                 .Where(bv => bv.Ma != settings.BenhVienTiepNhan)
                 .Select(item => new LookupItemVo
                 {
                     DisplayName = item.Ten,
                     KeyId = item.Id,
                 })
                 .Take(queryInfo.Take)
                 .ToListAsync();
                return lstBenhVien;
            }
        }

        public async Task<string> GetMucDoDiUng(MucDoDiUngThuocVo mucDoVo)
        {
            var yeuCauKhamBenh = BaseRepository.GetById(mucDoVo.YeuCauKhamBenhId, x => x.Include(o => o.YeuCauTiepNhan).ThenInclude(yc => yc.BenhNhan).ThenInclude(bn => bn.BenhNhanDiUngThuocs));
            var duocPhamHoatChat = await _duocPhamRepository.TableNoTracking
               .Where(o => o.Id == mucDoVo.DuocPhamId).Select(o => o.HoatChat).FirstOrDefaultAsync();

            var mucDoDiUng = yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs
                                            .OrderByDescending(o => o.MucDo)
                                            .Where(diung => diung.TenDiUng.Contains(duocPhamHoatChat) && diung.LoaiDiUng == Enums.LoaiDiUng.Thuoc)
                                            .Select(diung => diung.MucDo.GetDescription()).FirstOrDefault();
            return mucDoDiUng;
        }

        public List<string> GetThoiGianDonThuoc()
        {
            var list = Enum.GetValues(typeof(Enums.EnumThoiGianDonThuoc)).Cast<Enum>();
            var result2 = list.Select(p => p.GetDescription()).ToList();
            return result2;
        }

        public List<string> GetGhiChuDonThuocChiTiet()
        {
            var list = _inputStringStoredRepository.TableNoTracking
                    .Where(p => p.Set == Enums.InputStringStoredKey.Thuoc).Select(p => p.Value).ToList();
            return list;
        }

        public async Task<List<string>> GetGhiChuDonThuocChiTietString(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
               nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored.Value)
            };
            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                var loaiDuocPhamHoacVatTu = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                if (loaiDuocPhamHoacVatTu == (long)Enums.InputStringStoredKey.Thuoc)
                {
                    if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
                    {
                        var lstValues = _inputStringStoredRepository.TableNoTracking
                            .Where(p => p.Set == Enums.InputStringStoredKey.Thuoc)
                            .Select(p => p.Value)
                            .ApplyLike(queryInfo.Query, o => o)
                            .Take(queryInfo.Take);

                        return await lstValues.ToListAsync();
                    }
                    else
                    {
                        var lstIds = _inputStringStoredRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored), lstColumnNameSearch)
                                        .Select(p => p.Id).ToList();

                        var dictionary = lstIds.Select((id, index) => new
                        {
                            keys = id,
                            rank = index,
                        }).ToDictionary(o => o.keys, o => o.rank);

                        var lstValues = await _inputStringStoredRepository
                                                .TableNoTracking
                                                .Where(p => p.Set == Enums.InputStringStoredKey.Thuoc)
                                                .ApplyLike(queryInfo.Query, o => o.Value)
                                                .Take(queryInfo.Take)
                                                .Select(item => new InputStringStoredTemplateVo
                                                {
                                                    Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                                    DisplayName = item.Value,
                                                    KeyId = item.Id,
                                                })
                                                .OrderBy(r => r.Rank)
                                                .ToListAsync();
                        var listValueStrings = lstValues.Select(p => p.DisplayName).ToList();
                        return listValueStrings;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
                    {
                        var lstValues = _inputStringStoredRepository.TableNoTracking
                            .Where(p => p.Set == Enums.InputStringStoredKey.VatTu)
                            .Select(p => p.Value)
                            .ApplyLike(queryInfo.Query, o => o)
                            .Take(queryInfo.Take);

                        return await lstValues.ToListAsync();
                    }
                    else
                    {
                        var lstIds = _inputStringStoredRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored), lstColumnNameSearch)
                                        .Select(p => p.Id).ToList();

                        var dictionary = lstIds.Select((id, index) => new
                        {
                            keys = id,
                            rank = index,
                        }).ToDictionary(o => o.keys, o => o.rank);

                        var lstValues = await _inputStringStoredRepository
                                                .TableNoTracking
                                                .Where(p => p.Set == Enums.InputStringStoredKey.VatTu)
                                                .ApplyLike(queryInfo.Query, o => o.Value)
                                                .Take(queryInfo.Take)
                                                .Select(item => new InputStringStoredTemplateVo
                                                {
                                                    Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                                    DisplayName = item.Value,
                                                    KeyId = item.Id,
                                                })
                                                .OrderBy(r => r.Rank)
                                                .ToListAsync();
                        var listValueStrings = lstValues.Select(p => p.DisplayName).ToList();
                        return listValueStrings;
                    }
                }
            }
            else
            {
                return new List<string>();
            }
        }

        public async Task<List<string>> GetLyDoNhapVienString(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
               nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored.Value)
            };
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                var lstValues = _inputStringStoredRepository.TableNoTracking
                    .Where(p => p.Set == Enums.InputStringStoredKey.LyDoNhapVien)
                    .Select(p => p.Value)
                    .ApplyLike(queryInfo.Query, o => o)
                    .Take(queryInfo.Take);

                return await lstValues.ToListAsync();
            }
            else
            {
                var lstIds = _inputStringStoredRepository
                                .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored), lstColumnNameSearch)
                                .Select(p => p.Id).ToList();

                var dictionary = lstIds.Select((id, index) => new
                {
                    keys = id,
                    rank = index,
                }).ToDictionary(o => o.keys, o => o.rank);

                var lstValues = await _inputStringStoredRepository
                                        .TableNoTracking
                                        .Where(p => p.Set == Enums.InputStringStoredKey.LyDoNhapVien)
                                        .ApplyLike(queryInfo.Query, o => o.Value)
                                        .Take(queryInfo.Take)
                                        .Select(item => new InputStringStoredTemplateVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Value,
                                            KeyId = item.Id,
                                        })
                                        .OrderBy(r => r.Rank)
                                        .ToListAsync();
                var listValueStrings = lstValues.Select(p => p.DisplayName).ToList();
                return listValueStrings;
            }

        }

        public bool KiemTraCoThoiDiemBatDauDieuTri(long? yeuCauDichVuKyThuatId)
        {
            var thoiDiemBatDauDieuTri = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(p => p.Id == yeuCauDichVuKyThuatId && p.DieuTriNgoaiTru == true && p.TrangThai
               != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).FirstOrDefault().ThoiDiemBatDauDieuTri;
            if (thoiDiemBatDauDieuTri != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> ThemDonThuocChiTiet(DonThuocChiTietVo donThuocChiTiet)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh ycKhamBenh;
            if (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT)
            {
                ycKhamBenh = BaseRepository.GetById(donThuocChiTiet.YeuCauKhamBenhId, x => x.Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DuocPham).ThenInclude(dt => dt.DuocPhamBenhVien)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuKyThuats)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuGiuongBenhViens)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDuocPhamBenhViens)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauVatTuBenhViens)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets)
                    .Include(o => o.YeuCauKhamBenhLichSuTrangThais)
                    );
            }
            else
            {
                ycKhamBenh = BaseRepository.GetById(donThuocChiTiet.YeuCauKhamBenhId, x =>
                    x.Include(o => o.YeuCauTiepNhan)
                    .Include(o => o.YeuCauKhamBenhLichSuTrangThais)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DuocPham).ThenInclude(dt => dt.DuocPhamBenhVien)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans));
            }

            if (ycKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                ycKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
                ycKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                ycKhamBenh.NoiThucHienId = ycKhamBenh.NoiDangKyId; // _userAgentHelper.GetCurrentNoiLLamViecId();
                ycKhamBenh.ThoiDiemThucHien = DateTime.Now;
                // lưu lịch sử
                var lichSuNew = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = ycKhamBenh.TrangThai,
                    MoTa = ycKhamBenh.TrangThai.GetDescription(),
                };
                ycKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
            }

            var donThuoc = ycKhamBenh.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == donThuocChiTiet.LoaiDonThuoc);
            if (donThuoc == null)
            {
                donThuoc = new YeuCauKhamBenhDonThuoc
                {
                    TrangThai = Enums.EnumTrangThaiDonThuoc.ChuaCapThuoc,
                    LoaiDonThuoc = donThuocChiTiet.LoaiDonThuoc,
                    ThoiDiemKeDon = DateTime.Now,
                    BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                    NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                ycKhamBenh.YeuCauKhamBenhDonThuocs.Add(donThuoc);
            }

            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamId,
                x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
            var ycDonThuocChiTiet =
                new YeuCauKhamBenhDonThuocChiTiet
                {
                    DuocPhamId = duocPham.Id,
                    LaDuocPhamBenhVien = donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT || donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien,
                    Ten = duocPham.Ten,
                    TenTiengAnh = duocPham.TenTiengAnh,
                    SoDangKy = duocPham.SoDangKy,
                    StthoatChat = duocPham.STTHoatChat,
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
                    SoLuong = donThuocChiTiet.SoLuong,
                    SoNgayDung = donThuocChiTiet.SoNgayDung,
                    DungSang = donThuocChiTiet.DungSang,
                    DungTrua = donThuocChiTiet.DungTrua,
                    DungChieu = donThuocChiTiet.DungChieu,
                    DungToi = donThuocChiTiet.DungToi,
                    LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay,
                    SoLanTrenVien = donThuocChiTiet.SoLanTrenVien,
                    ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                    ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                    ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                    ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                    DuocHuongBaoHiem = donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                    BenhNhanMuaNgoai = donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien,
                    GhiChu = donThuocChiTiet.GhiChu,
                    SoThuTu = donThuocChiTiet.SoThuTu
                };
            donThuoc.YeuCauKhamBenhDonThuocChiTiets.Add(ycDonThuocChiTiet);

            if (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT || donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien)
            {
                if (duocPham.DuocPhamBenhVien == null ||
                    (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT && duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT) && o.LaDuocPhamBHYT && o.HanSuDung >= DateTime.Now && o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < donThuocChiTiet.SoLuong) ||
                    (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien && duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc && !o.LaDuocPhamBHYT && o.HanSuDung >= DateTime.Now && o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < donThuocChiTiet.SoLuong))
                {
                    //return "Dược phẩm không có trong kho hoặc số lượng tồn không đủ";
                    return GetResourceValueByResourceName("DonThuoc.ThuocKhongCoTrongKhoHoacSoLuongKhongDu");
                }
                //ktra don thuoc thanh toan
                var donThuocThanhToan = donThuoc.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == donThuocChiTiet.LoaiDonThuoc && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan);
                if (donThuocThanhToan == null)
                {
                    donThuocThanhToan = new DonThuocThanhToan
                    {
                        LoaiDonThuoc = donThuocChiTiet.LoaiDonThuoc,
                        YeuCauKhamBenhId = ycKhamBenh.Id,
                        YeuCauTiepNhanId = ycKhamBenh.YeuCauTiepNhan.Id,
                        BenhNhanId = ycKhamBenh.YeuCauTiepNhan.BenhNhanId,
                        TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                    };
                    donThuoc.DonThuocThanhToans.Add(donThuocThanhToan);
                    ycKhamBenh.YeuCauTiepNhan.DonThuocThanhToans.Add(donThuocThanhToan);
                }
                //them don thuoc thanh toan chi tiet
                double soLuongCanXuat = donThuocChiTiet.SoLuong;
                while (!soLuongCanXuat.Equals(0))
                {
                    // tinh so luong xuat
                    var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(o => ((donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT && (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT) && o.LaDuocPhamBHYT) ||
                                    (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien && o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc && !o.LaDuocPhamBHYT)) && o.HanSuDung >= DateTime.Now
                                    && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                    var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                    var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;

                    nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
                    var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = soLuongXuat,
                        NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                        XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVien = duocPham.DuocPhamBenhVien
                        }
                    };
                    var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId).Gia;
                    var donGiaBaoHiem = nhapKhoDuocPhamChiTiet.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : nhapKhoDuocPhamChiTiet.DonGiaNhap;
                    var dtttChiTiet = new DonThuocThanhToanChiTiet
                    {
                        DuocPhamId = duocPham.Id,
                        YeuCauKhamBenhDonThuocChiTiet = ycDonThuocChiTiet,
                        XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
                        Ten = duocPham.Ten,
                        TenTiengAnh = duocPham.TenTiengAnh,
                        SoDangKy = duocPham.SoDangKy,
                        STTHoatChat = duocPham.STTHoatChat,
                        NhomChiPhi = donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT ? Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT : Enums.EnumDanhMucNhomTheoChiPhi.ThuocThanhToanTheoTyLe,
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
                        HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                        NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                        SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                        SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                        LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                        LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                        NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                        GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                        NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                        DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                        TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                        PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                        VAT = nhapKhoDuocPhamChiTiet.VAT,
                        SoLuong = soLuongXuat,
                        SoTienBenhNhanDaChi = 0,
                        DuocHuongBaoHiem = donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                        DonGiaBaoHiem = donGiaBaoHiem,
                        TiLeBaoHiemThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan ?? 100,
                    };

                    donThuocThanhToan.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                    soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                }

            }
            ycKhamBenh.CoKeToa = true;
            ycKhamBenh.KhongKeToa = null;
            //bo duyet tu dong
            //if (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT)
            //{
            //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
            //    if (cauHinh.DuyetBHYTTuDong)
            //    {
            //        DuyetBHYTChoDonThuoc(ycKhamBenh.YeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong);
            //    }
            //}
            if (ycKhamBenh.CoChuyenVien != null)
            {
                ycKhamBenh.CoChuyenVien = null;
                ycKhamBenh.BenhVienChuyenVienId = null;
                ycKhamBenh.LyDoChuyenVien = null;
                ycKhamBenh.TinhTrangBenhNhanChuyenVien = null;
                ycKhamBenh.ThoiDiemChuyenVien = null;
                ycKhamBenh.NhanVienHoTongChuyenVienId = null;
                ycKhamBenh.PhuongTienChuyenVien = null;
            }
            if (ycKhamBenh.CoNhapVien != null)
            {
                ycKhamBenh.CoNhapVien = null;
                ycKhamBenh.KhoaPhongNhapVienId = null;
                ycKhamBenh.LyDoNhapVien = null;
            }
            ycKhamBenh.CoTuVong = null;
            if (ycKhamBenh.BacSiKetLuanId == null)
            {
                ycKhamBenh.BacSiKetLuanId = _userAgentHelper.GetCurrentUserId();
            }
            await BaseRepository.UpdateAsync(ycKhamBenh);
            await XuLySoThuTu(ycKhamBenh);
            return string.Empty;
        }
        private async Task XuLySoThuTu(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh ycKhamBenh)
        {
            //var STT = 1;
            //var listThuocChiTiet = ycKhamBenh.YeuCauKhamBenhDonThuocs.Where(z => z.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT).SelectMany(z => z.YeuCauKhamBenhDonThuocChiTiets)
            //                        .Union(ycKhamBenh.YeuCauKhamBenhDonThuocs.Where(z => z.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).SelectMany(z => z.YeuCauKhamBenhDonThuocChiTiets));
            //if (listThuocChiTiet.Any(z => z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc))
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}
            //if (listThuocChiTiet.Any(z => z.DuongDungId == 12 && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)))//Tiêm z.DuocPham.DuongDung.Ma.Trim() == "2.10".Trim()) 12
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuongDungId == 12 && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}

            //if (listThuocChiTiet.Any(z => z.DuongDungId == 1 && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)))//Uống z.DuocPham.DuongDung.Ma.Trim() == "1.01".Trim() 1
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuongDungId == 1 && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}

            //if (listThuocChiTiet.Any(z => z.DuongDungId == 26 && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)))//Đặt 26 z.DuocPham.DuongDung.Ma.Trim() == "4.04".Trim()
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuongDungId == 26 && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}

            //if (listThuocChiTiet.Any(z => z.DuongDungId == 22 && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)))//Dùng ngoài 22 z.DuocPham.DuongDung.Ma.Trim() == "3.05".Trim()
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuongDungId == 22 && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}
            //foreach (var item in listThuocChiTiet.Where(z => !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)
            //                                                                     && z.DuongDungId != 12
            //                                                                     && z.DuongDungId != 1
            //                                                                     && z.DuongDungId != 26
            //                                                                     && z.DuongDungId != 22).OrderBy(z => z.CreatedOn))
            //{
            //    item.SoThuTu = STT;
            //    STT++;
            //}
            //BVHD-3959
            var STT = 1;
            var listThuocChiTiet = ycKhamBenh.YeuCauKhamBenhDonThuocs.Where(z => z.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT).SelectMany(z => z.YeuCauKhamBenhDonThuocChiTiets)
                                    .Union(ycKhamBenh.YeuCauKhamBenhDonThuocs.Where(z => z.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).SelectMany(z => z.YeuCauKhamBenhDonThuocChiTiets));
            var listThuocChiTietSapXep = listThuocChiTiet
                .Where(o => o.DuocPham.DuocPhamBenhVien != null)
                .OrderBy(o => o.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc ? 1 : 2)
                .ThenBy(o => BenhVienHelper.GetSoThuThuocTheoDuongDung(o.DuocPham.DuongDungId))
                .ThenBy(o => o.CreatedOn)
                .ToList();
            foreach (var item in listThuocChiTietSapXep)
            {
                item.SoThuTu = STT;
                STT++;
            }

            await BaseRepository.UpdateAsync(ycKhamBenh);
        }

        //public void DuyetBHYTChoDonThuoc(YeuCauTiepNhan ycTiepNhan, long nguoiDuyetId, long noiDuyetId)
        //{
        //    var soTienBHYTSeThanhToanToanBo = _cauHinhService.SoTienBHYTSeThanhToanToanBo().Result;
        //    var soTienTheoMucHuong100 = GetSoTienBhytSeDuyetTheoMucHuong(ycTiepNhan, 100);
        //    var mucHuongHienTai = soTienTheoMucHuong100 <= soTienBHYTSeThanhToanToanBo ? 100 : ycTiepNhan.BHYTMucHuong.GetValueOrDefault();

        //    //xac nhan
        //    var duyetBaoHiem = new DuyetBaoHiem
        //    {
        //        NhanVienDuyetBaoHiemId = nguoiDuyetId,
        //        ThoiDiemDuyetBaoHiem = DateTime.Now,
        //        NoiDuyetBaoHiemId = noiDuyetId
        //    };

        //    var dsDichVuKhamBenh = ycTiepNhan.YeuCauKhamBenhs
        //        .Where(p => p.CreatedOn != null && p.DuocHuongBaoHiem && p.BaoHiemChiTra == true &&
        //                    p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList();
        //    foreach (var yeuCauKhamBenh in dsDichVuKhamBenh)
        //    {
        //        if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if ((mucHuongHienTai == 100 && yeuCauKhamBenh.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && yeuCauKhamBenh.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            yeuCauKhamBenh.MucHuongBaoHiem = mucHuongHienTai;
        //            yeuCauKhamBenh.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            yeuCauKhamBenh.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                YeuCauKhamBenh = yeuCauKhamBenh,
        //                SoLuong = 1,
        //                TiLeBaoHiemThanhToan = yeuCauKhamBenh.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = yeuCauKhamBenh.MucHuongBaoHiem,
        //                DonGiaBaoHiem = yeuCauKhamBenh.DonGiaBaoHiem
        //            });
        //            if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }
        //    foreach (var yckt in ycTiepNhan.YeuCauDichVuKyThuats
        //        .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
        //    {
        //        if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if ((mucHuongHienTai == 100 && yckt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && yckt.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            yckt.MucHuongBaoHiem = mucHuongHienTai;
        //            yckt.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            yckt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                YeuCauDichVuKyThuat = yckt,
        //                SoLuong = yckt.SoLan,
        //                TiLeBaoHiemThanhToan = yckt.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = yckt.MucHuongBaoHiem,
        //                DonGiaBaoHiem = yckt.DonGiaBaoHiem
        //            });
        //            if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                yckt.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycdp in ycTiepNhan.YeuCauDuocPhamBenhViens
        //                .Where(p => p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
        //    {
        //        if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if ((mucHuongHienTai == 100 && ycdp.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycdp.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            ycdp.MucHuongBaoHiem = mucHuongHienTai;
        //            ycdp.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            ycdp.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                YeuCauDuocPhamBenhVien = ycdp,
        //                SoLuong = ycdp.SoLuong,
        //                TiLeBaoHiemThanhToan = ycdp.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = ycdp.MucHuongBaoHiem,
        //                DonGiaBaoHiem = ycdp.DonGiaBaoHiem
        //            });
        //            if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycdp.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycvt in ycTiepNhan.YeuCauVatTuBenhViens
        //        .Where(p => p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
        //    {
        //        if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if ((mucHuongHienTai == 100 && ycvt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycvt.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            ycvt.MucHuongBaoHiem = mucHuongHienTai;
        //            ycvt.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            ycvt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                YeuCauVatTuBenhVien = ycvt,
        //                SoLuong = ycvt.SoLuong,
        //                TiLeBaoHiemThanhToan = ycvt.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = ycvt.MucHuongBaoHiem,
        //                DonGiaBaoHiem = ycvt.DonGiaBaoHiem
        //            });
        //            if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycvt.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycdt in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
        //                .SelectMany(w => w.DonThuocThanhToanChiTiets)
        //                .Where(p => !p.WillDelete && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false))
        //    {
        //        if (ycdt.DonThuocThanhToan?.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if (ycdt.BaoHiemChiTra == null || (mucHuongHienTai == 100 && ycdt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycdt.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            ycdt.BaoHiemChiTra = true;
        //            ycdt.MucHuongBaoHiem = mucHuongHienTai;
        //            ycdt.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            ycdt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                DonThuocThanhToanChiTiet = ycdt,
        //                SoLuong = ycdt.SoLuong,
        //                TiLeBaoHiemThanhToan = ycdt.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = ycdt.MucHuongBaoHiem,
        //                DonGiaBaoHiem = ycdt.DonGiaBaoHiem
        //            });
        //            if (ycdt.DonThuocThanhToan?.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycdt.DonThuocThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    if (duyetBaoHiem.DuyetBaoHiemChiTiets.Any())
        //    {
        //        ycTiepNhan.DuyetBaoHiems.Add(duyetBaoHiem);
        //    }
        //}

        //private decimal GetSoTienBhytSeDuyetTheoMucHuong(YeuCauTiepNhan yeuCauTiepNhan, int mucHuong)
        //{
        //    decimal total = 0;

        //    var dsDichVuKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs
        //        .Where(p => p.CreatedOn != null && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
        //        .OrderBy(o => o.Id)
        //        .Concat(yeuCauTiepNhan.YeuCauKhamBenhs.Where(p =>
        //            p.CreatedOn == null && p.DuocHuongBaoHiem)).ToList();

        //    total += dsDichVuKhamBenh.Select((o, i) => o.DonGiaBaoHiem.GetValueOrDefault() * TiLeHuongBHYTTheoLanKham(i) / 100 * mucHuong / 100).Sum();

        //    var dsDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats
        //        .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);


        //    foreach (var yckt in dsDichVuKyThuat)
        //    {
        //        total += yckt.DonGiaBaoHiem.GetValueOrDefault() * (yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ? (yckt.TiLeBaoHiemThanhToan ?? 100) : 100) / 100 * mucHuong / 100 * yckt.SoLan;
        //    }

        //    total += yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
        //        .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy)
        //            .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * mucHuong / 100).Sum();

        //    total += yeuCauTiepNhan.YeuCauDuocPhamBenhViens
        //            .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
        //            .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();

        //    total += yeuCauTiepNhan.YeuCauVatTuBenhViens
        //            .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
        //            .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();

        //    total += yeuCauTiepNhan.DonThuocThanhToans
        //            .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
        //            .SelectMany(o => o.DonThuocThanhToanChiTiets)
        //            .Where(p => !p.WillDelete && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false)
        //            .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();
        //    return total;
        //}

        //private void LoadAllDichVuBHYT(YeuCauTiepNhan yeuCauTiepNhan)
        //{
        //    if (yeuCauTiepNhan.CoBHYT == true)
        //    {
        //        var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.YeuCauKhamBenhs);
        //        if (!yeuCauKhamBenhs.IsLoaded) yeuCauKhamBenhs.Load();
        //        var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.YeuCauDichVuKyThuats);
        //        if (!yeuCauDichVuKyThuats.IsLoaded) yeuCauDichVuKyThuats.Load();
        //        var yeuCauDichVuGiuongBenhViens = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.YeuCauDichVuGiuongBenhViens);
        //        if (!yeuCauDichVuGiuongBenhViens.IsLoaded) yeuCauDichVuGiuongBenhViens.Load();
        //        var donThuocThanhToans = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.DonThuocThanhToans);
        //        if (!donThuocThanhToans.IsLoaded) donThuocThanhToans.Load();
        //        foreach (var donThuocThanhToan in yeuCauTiepNhan.DonThuocThanhToans)
        //        {
        //            var donThuocThanhToanChiTiets = BaseRepository.Context.Entry(donThuocThanhToan).Collection(o => o.DonThuocThanhToanChiTiets);
        //            if (!donThuocThanhToanChiTiets.IsLoaded) donThuocThanhToanChiTiets.Load();
        //        }
        //    }
        //}
        //private List<double> _getTiLeHuongBaoHiem5LanKhamDichVuBHYT;
        //private List<double> GetTiLeHuongBaoHiem5LanKhamDichVuBHYT()
        //{
        //    if (_getTiLeHuongBaoHiem5LanKhamDichVuBHYT == null)
        //    {
        //        _getTiLeHuongBaoHiem5LanKhamDichVuBHYT = _cauHinhService.GetTyLeHuongBaoHiem5LanKhamDichVuBHYT().Result;
        //    }
        //    return _getTiLeHuongBaoHiem5LanKhamDichVuBHYT;
        //}

        //protected int TiLeHuongBHYTTheoLanKham(int lanKham)
        //{
        //    if (lanKham < GetTiLeHuongBaoHiem5LanKhamDichVuBHYT().Count)
        //        return (int)GetTiLeHuongBaoHiem5LanKhamDichVuBHYT()[lanKham];
        //    return 0;
        //}

        public async Task<string> CapNhatDonThuocChiTiet(DonThuocChiTietVo donThuocChiTiet)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var ycDonThuocChiTiet = _yeuCauKhamBenhDonThuocChiTietRepository.GetById(donThuocChiTiet.DonThuocChiTietId,
                x => x.Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan)
                    .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(xk => xk.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(xk => xk.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.DonThuocThanhToan)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.DuyetBaoHiemChiTiets)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.TaiKhoanBenhNhanChis)
                    .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuKyThuats)
                    .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuGiuongBenhViens)
                    .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDuocPhamBenhViens)
                    .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauVatTuBenhViens));
            //kiem tra truoc khi cap nhat
            if (ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
            {
                //return "Dược phẩm đã được thanh toán";
                return GetResourceValueByResourceName("DonThuoc.ThuocDaThanhToan");
            }
            if (ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc))
            {
                return GetResourceValueByResourceName("DonThuoc.ThuocDaXuat");
            }
            var soLuongTruoc = ycDonThuocChiTiet.SoLuong;
            //var duocHuongBaoHiem = ycDonThuocChiTiet.DuocHuongBaoHiem;

            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamId,
                x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

            ycDonThuocChiTiet.DuocPhamId = duocPham.Id;
            ycDonThuocChiTiet.Ten = duocPham.Ten;
            ycDonThuocChiTiet.TenTiengAnh = duocPham.TenTiengAnh;
            ycDonThuocChiTiet.SoDangKy = duocPham.SoDangKy;
            ycDonThuocChiTiet.StthoatChat = duocPham.STTHoatChat;
            ycDonThuocChiTiet.MaHoatChat = duocPham.MaHoatChat;
            ycDonThuocChiTiet.HoatChat = duocPham.HoatChat;
            ycDonThuocChiTiet.LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat;
            ycDonThuocChiTiet.NhaSanXuat = duocPham.NhaSanXuat;
            ycDonThuocChiTiet.NuocSanXuat = duocPham.NuocSanXuat;
            ycDonThuocChiTiet.DuongDungId = duocPham.DuongDungId;
            ycDonThuocChiTiet.HamLuong = duocPham.HamLuong;
            ycDonThuocChiTiet.QuyCach = duocPham.QuyCach;
            ycDonThuocChiTiet.TieuChuan = duocPham.TieuChuan;
            ycDonThuocChiTiet.DangBaoChe = duocPham.DangBaoChe;
            ycDonThuocChiTiet.DonViTinhId = duocPham.DonViTinhId;
            ycDonThuocChiTiet.HuongDan = duocPham.HuongDan;
            ycDonThuocChiTiet.MoTa = duocPham.MoTa;
            ycDonThuocChiTiet.ChiDinh = duocPham.ChiDinh;
            ycDonThuocChiTiet.ChongChiDinh = duocPham.ChongChiDinh;
            ycDonThuocChiTiet.LieuLuongCachDung = duocPham.LieuLuongCachDung;
            ycDonThuocChiTiet.TacDungPhu = duocPham.TacDungPhu;
            ycDonThuocChiTiet.ChuYdePhong = duocPham.ChuYDePhong;
            ycDonThuocChiTiet.SoLuong = donThuocChiTiet.SoLuong;
            ycDonThuocChiTiet.SoNgayDung = donThuocChiTiet.SoNgayDung;
            ycDonThuocChiTiet.DungSang = donThuocChiTiet.DungSang;
            ycDonThuocChiTiet.DungTrua = donThuocChiTiet.DungTrua;
            ycDonThuocChiTiet.DungChieu = donThuocChiTiet.DungChieu;
            ycDonThuocChiTiet.DungToi = donThuocChiTiet.DungToi;
            ycDonThuocChiTiet.LieuDungTrenNgay = donThuocChiTiet.LieuDungTrenNgay;
            ycDonThuocChiTiet.SoLanTrenVien = donThuocChiTiet.SoLanTrenVien;
            ycDonThuocChiTiet.ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang;
            ycDonThuocChiTiet.ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua;
            ycDonThuocChiTiet.ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu;
            ycDonThuocChiTiet.ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi;
            ycDonThuocChiTiet.GhiChu = donThuocChiTiet.GhiChu;


            if (ycDonThuocChiTiet.LaDuocPhamBenhVien)
            {
                if (!soLuongTruoc.AlmostEqual(donThuocChiTiet.SoLuong))
                {
                    var donThuocThanhToanChiTietLast = ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Last();
                    if (soLuongTruoc < donThuocChiTiet.SoLuong)
                    {
                        var soLuongTang = donThuocChiTiet.SoLuong - soLuongTruoc;
                        if (duocPham.DuocPhamBenhVien == null ||
                            duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                .Where(o => (ycDonThuocChiTiet.DuocHuongBaoHiem == o.LaDuocPhamBHYT && o.HanSuDung >= DateTime.Now &&
                                             (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT)))
                                .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < soLuongTang)
                        {
                            //return "Dược phẩm không có trong kho";
                            return GetResourceValueByResourceName("DonThuoc.ThuocKhongCoTrongKhoHoacSoLuongKhongDu");
                        }

                        //update don thuoc thanh toan chi tiet
                        var xuatKhoLast = donThuocThanhToanChiTietLast.XuatKhoDuocPhamChiTietViTri;

                        var xkChiTietViTri = BaseRepository.Context.Entry(donThuocThanhToanChiTietLast).Reference(o => o.XuatKhoDuocPhamChiTietViTri);
                        if (!xkChiTietViTri.IsLoaded) xkChiTietViTri.Load();

                        var soLuongTonHt = xuatKhoLast.NhapKhoDuocPhamChiTiet.SoLuongNhap -
                        xuatKhoLast.NhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                        if (soLuongTonHt >= soLuongTang)
                        {
                            donThuocThanhToanChiTietLast.SoLuong += soLuongTang;
                            xuatKhoLast.NhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongTang;
                            xuatKhoLast.SoLuongXuat += soLuongTang;
                        }
                        else
                        {
                            donThuocThanhToanChiTietLast.SoLuong += soLuongTonHt;
                            xuatKhoLast.NhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongTonHt;
                            xuatKhoLast.SoLuongXuat += soLuongTonHt;
                            var soLuongCanXuat = soLuongTang - soLuongTonHt;
                            while (!soLuongCanXuat.Equals(0))
                            {
                                // tinh so luong xuat
                                var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(
                                    o =>
                                        (ycDonThuocChiTiet.DuocHuongBaoHiem == o.LaDuocPhamBHYT && o.HanSuDung >= DateTime.Now &&
                                         (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT)) &&
                                        o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                                var soLuongTon =
                                    (float)(nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat);
                                var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;

                                nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
                                var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
                                {
                                    SoLuongXuat = soLuongXuat,
                                    NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                                    XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                                    {
                                        DuocPhamBenhVien = duocPham.DuocPhamBenhVien,
                                    }
                                };
                                var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId).Gia;
                                var donGiaBaoHiem = nhapKhoDuocPhamChiTiet.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : nhapKhoDuocPhamChiTiet.DonGiaNhap;
                                var dtttChiTiet = new DonThuocThanhToanChiTiet
                                {
                                    DuocPhamId = duocPham.Id,
                                    YeuCauKhamBenhDonThuocChiTiet = ycDonThuocChiTiet,
                                    XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
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
                                    HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                                    NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                                    SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                                    SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                                    LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                                    LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                                    NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                                    GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                                    NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                                    DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                                    TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                                    PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                                    VAT = nhapKhoDuocPhamChiTiet.VAT,
                                    SoLuong = soLuongXuat,
                                    SoTienBenhNhanDaChi = 0,
                                    NhomChiPhi = donThuocThanhToanChiTietLast.NhomChiPhi,
                                    DuocHuongBaoHiem = donThuocThanhToanChiTietLast.DuocHuongBaoHiem,
                                    BaoHiemChiTra = donThuocThanhToanChiTietLast.BaoHiemChiTra,
                                    MucHuongBaoHiem = donThuocThanhToanChiTietLast.MucHuongBaoHiem,
                                    DonGiaBaoHiem = donGiaBaoHiem,
                                    TiLeBaoHiemThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan ?? 100,
                                    DonThuocThanhToanId = donThuocThanhToanChiTietLast.DonThuocThanhToanId
                                };
                                ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                                soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                            }
                        }
                    }
                    else//Giam so luong thuoc
                    {
                        var soLuongGiam = soLuongTruoc - donThuocChiTiet.SoLuong;
                        var donThuocThanhToanChiTiets = ycDonThuocChiTiet.DonThuocThanhToanChiTiets.OrderByDescending(o => o.Id).ToList();
                        for (int i = 0; i < donThuocThanhToanChiTiets.Count; i++)
                        {
                            if (donThuocThanhToanChiTiets[i].SoLuong <= soLuongGiam)
                            {
                                foreach (var duyetBaoHiemChiTiet in donThuocThanhToanChiTiets[i].DuyetBaoHiemChiTiets)
                                {
                                    duyetBaoHiemChiTiet.WillDelete = true;
                                }
                                foreach (var congNo in donThuocThanhToanChiTiets[i].CongTyBaoHiemTuNhanCongNos)
                                {
                                    congNo.WillDelete = true;
                                }
                                foreach (var mienGiam in donThuocThanhToanChiTiets[i].MienGiamChiPhis)
                                {
                                    mienGiam.WillDelete = true;
                                }
                                foreach (var taiKhoanBenhNhanChi in donThuocThanhToanChiTiets[i].TaiKhoanBenhNhanChis)
                                {
                                    taiKhoanBenhNhanChi.DonThuocThanhToanChiTietId = null;
                                }

                                donThuocThanhToanChiTiets[i].XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= donThuocThanhToanChiTiets[i].SoLuong;
                                donThuocThanhToanChiTiets[i].WillDelete = true;
                                donThuocThanhToanChiTiets[i].XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                                donThuocThanhToanChiTiets[i].XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                                if (donThuocThanhToanChiTiets[i].XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
                                    donThuocThanhToanChiTiets[i].XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;

                                soLuongGiam -= donThuocThanhToanChiTiets[i].SoLuong;
                                if (soLuongGiam.AlmostEqual(0))
                                    break;
                            }
                            else
                            {
                                donThuocThanhToanChiTiets[i].XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= soLuongGiam;
                                donThuocThanhToanChiTiets[i].XuatKhoDuocPhamChiTietViTri.SoLuongXuat -= soLuongGiam;
                                donThuocThanhToanChiTiets[i].SoLuong -= soLuongGiam;
                                break;
                            }
                        }
                    }
                    //bo duyet tu dong
                    //if (donThuocThanhToanChiTietLast.BaoHiemChiTra != false)
                    //{
                    //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                    //    if (cauHinh.DuyetBHYTTuDong)
                    //    {
                    //        DuyetBHYTChoDonThuoc(ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong);
                    //    }
                    //}
                }
            }
            await _yeuCauKhamBenhDonThuocChiTietRepository.UpdateAsync(ycDonThuocChiTiet);
            //return (BitConverter.ToInt64(ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.LastModified), string.Empty);
            return string.Empty;
        }
        /*
        public async Task<string> CapNhatDonThuocChiTietOld(DonThuocChiTietVo donThuocChiTiet)
        {
            var ycDonThuocChiTiet = _yeuCauKhamBenhDonThuocChiTietRepository.GetById(donThuocChiTiet.DonThuocChiTietId,
                x => x.Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan)
                    .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.DonThuocThanhToans)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonThuocThanhToan)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet));
            ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.LastTime = DateTime.Now;
            //kiem tra truoc khi cap nhat
            if (ycDonThuocChiTiet == null || ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan))
            {
                //return "Dược phẩm đã được thanh toán";
                return GetResourceValueByResourceName("DonThuoc.ThuocDaThanhToan");
            }

            var soLuongTruoc = ycDonThuocChiTiet.SoLuong;
            var duocHuongBaoHiem = false;
            //hoan lai duoc pham da book trong kho
            foreach (var donThuocThanhToanChiTiet in ycDonThuocChiTiet.DonThuocThanhToanChiTiets)
            {
                donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                donThuocThanhToanChiTiet.WillDelete = true;
                donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                if(donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
                    donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;

            }
            //cap nhat CauKhamBenhDonThuocChiTiet
            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamId,
                x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.DuocPhamBenhVienGiaBaoHiems).Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams)
                    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

            ycDonThuocChiTiet.DuocPhamId = duocPham.Id;
            ycDonThuocChiTiet.Ten = duocPham.Ten;
            ycDonThuocChiTiet.TenTiengAnh = duocPham.TenTiengAnh;
            ycDonThuocChiTiet.SoDangKy = duocPham.SoDangKy;
            ycDonThuocChiTiet.StthoatChat = duocPham.STTHoatChat;
            ycDonThuocChiTiet.MaHoatChat = duocPham.MaHoatChat;
            ycDonThuocChiTiet.HoatChat = duocPham.HoatChat;
            ycDonThuocChiTiet.LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat;
            ycDonThuocChiTiet.NhaSanXuat = duocPham.NhaSanXuat;
            ycDonThuocChiTiet.NuocSanXuat = duocPham.NuocSanXuat;
            ycDonThuocChiTiet.DuongDungId = duocPham.DuongDungId;
            ycDonThuocChiTiet.HamLuong = duocPham.HamLuong;
            ycDonThuocChiTiet.QuyCach = duocPham.QuyCach;
            ycDonThuocChiTiet.TieuChuan = duocPham.TieuChuan;
            ycDonThuocChiTiet.DangBaoChe = duocPham.DangBaoChe;
            ycDonThuocChiTiet.DonViTinhId = duocPham.DonViTinhId;
            ycDonThuocChiTiet.HuongDan = duocPham.HuongDan;
            ycDonThuocChiTiet.MoTa = duocPham.MoTa;
            ycDonThuocChiTiet.ChiDinh = duocPham.ChiDinh;
            ycDonThuocChiTiet.ChongChiDinh = duocPham.ChongChiDinh;
            ycDonThuocChiTiet.LieuLuongCachDung = duocPham.LieuLuongCachDung;
            ycDonThuocChiTiet.TacDungPhu = duocPham.TacDungPhu;
            ycDonThuocChiTiet.ChuYdePhong = duocPham.ChuYDePhong;
            ycDonThuocChiTiet.SoLuong = donThuocChiTiet.SoLuong;
            ycDonThuocChiTiet.SoNgayDung = donThuocChiTiet.SoNgayDung;
            ycDonThuocChiTiet.DungSang = donThuocChiTiet.DungSang;
            ycDonThuocChiTiet.DungTrua = donThuocChiTiet.DungTrua;
            ycDonThuocChiTiet.DungChieu = donThuocChiTiet.DungChieu;
            ycDonThuocChiTiet.DungToi = donThuocChiTiet.DungToi;
            ycDonThuocChiTiet.ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang;
            ycDonThuocChiTiet.ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua;
            ycDonThuocChiTiet.ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu;
            ycDonThuocChiTiet.ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi;
            ycDonThuocChiTiet.GhiChu = donThuocChiTiet.GhiChu;


            if (ycDonThuocChiTiet.LaDuocPhamBenhVien)
            {
                if (duocPham.DuocPhamBenhVien == null ||
                    duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                    .Where(o => ((ycDonThuocChiTiet.DuocHuongBaoHiem && o.NhapKhoDuocPhams.KhoDuocPhams.Id == (int)Enums.EnumKhoDuocPham.KhoThuocBHYT) ||
                                 (!ycDonThuocChiTiet.DuocHuongBaoHiem && o.NhapKhoDuocPhams.KhoDuocPhams.Id == (int)Enums.EnumKhoDuocPham.KhoThuocVienPhi)) &&
                                 o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < donThuocChiTiet.SoLuong)
                {
                    //return "Dược phẩm không có trong kho";
                    return GetResourceValueByResourceName("DonThuoc.ThuocKhongCoTrongKhoHoacSoLuongKhongDu");
                }


                if (ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DuocHuongBaoHiem))
                {
                    duocHuongBaoHiem = true;
                    LoadAllDichVuBHYT(ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan);
                }

                //ktra don thuoc thanh toan
                var donThuocThanhToan = ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.DonThuocThanhToans
                    .First(o => o.LoaiDonThuoc == ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.LoaiDonThuoc
                                 && o.TrangThai == Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc
                                 && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan
                                 && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan);

                //them don thuoc thanh toan chi tiet
                float soLuongCanXuat = (float)donThuocChiTiet.SoLuong;
                while (!soLuongCanXuat.Equals(0))
                {
                    // tinh so luong xuat
                    var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o =>
                                                     ((ycDonThuocChiTiet.DuocHuongBaoHiem && o.NhapKhoDuocPhams.KhoDuocPhams.Id == (int)Enums.EnumKhoDuocPham.KhoThuocBHYT) ||
                                                     (!ycDonThuocChiTiet.DuocHuongBaoHiem && o.NhapKhoDuocPhams.KhoDuocPhams.Id == (int)Enums.EnumKhoDuocPham.KhoThuocVienPhi))
                                                     && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(o => o.HanSuDung).First();
                    var soLuongTon = (float)(nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat);
                    var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;

                    nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
                    var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = soLuongXuat,
                        NgayXuat = DateTime.Now,
                        NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                        XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVien = duocPham.DuocPhamBenhVien,
                            //TODO update entity kho on 9/9/2020
                            //DatChatLuong = true,
                            NgayXuat = DateTime.Now,
                            XuatKhoDuocPham = new XuatKhoDuocPham
                            {
                                KhoDuocPhamXuat = nhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.KhoDuocPhams,
                                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                                TenNguoiNhan = ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.HoTen,
                                NgayXuat = DateTime.Now,
                                NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                                SoPhieu = ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.MaYeuCauTiepNhan
                            }
                        }
                    };
                    var ttGiaBh = duocPham.DuocPhamBenhVien.DuocPhamBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay));

                    var dtttChiTiet = new DonThuocThanhToanChiTiet
                    {
                        DuocPhamId = duocPham.Id,
                        YeuCauKhamBenhDonThuocChiTiet = ycDonThuocChiTiet,
                        XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
                        Ten = duocPham.Ten,
                        TenTiengAnh = duocPham.TenTiengAnh,
                        SoDangKy = duocPham.SoDangKy,
                        STTHoatChat = duocPham.STTHoatChat,
                        NhomChiPhi = ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT : Enums.EnumDanhMucNhomTheoChiPhi.ThuocThanhToanTheoTyLe,
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
                        HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                        NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                        SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                        SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                        LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                        LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                        NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                        GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                        NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                        //TODO update entity kho on 9/9/2020
                        //Gia = nhapKhoDuocPhamChiTiet.DonGiaBan + (nhapKhoDuocPhamChiTiet.DonGiaBan * nhapKhoDuocPhamChiTiet.VAT / 100),
                        SoLuong = soLuongXuat,
                        SoTienBenhNhanDaChi = 0,
                        DuocHuongBaoHiem = ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT,
                        DonGiaBaoHiem = ttGiaBh?.Gia
                    };

                    donThuocThanhToan.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                    soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                }
                if (donThuocChiTiet.SoLuong > soLuongTruoc)
                {
                    var duocPhamBenhVienGiaBaoHiem = BaseRepository.Context.Set<DuocPhamBenhVienGiaBaoHiem>().AsNoTracking().FirstOrDefault(p =>
                        p.DuocPhamBenhVienId == donThuocChiTiet.DuocPhamId && p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                    if (duocPhamBenhVienGiaBaoHiem != null)
                    {
                        var soTienBHYTSeThanhToanToanBo = _cauHinhService.SoTienBHYTSeThanhToanToanBo().Result;
                        var bhytThanhToan = duocPhamBenhVienGiaBaoHiem.Gia * duocPhamBenhVienGiaBaoHiem.TiLeBaoHiemThanhToan / 100;
                        foreach (var donThuocThanhToanChiTiet in donThuocThanhToan.DonThuocThanhToanChiTiets.Where(o => o.Id == 0))
                        {
                            //donThuocThanhToanChiTiet.GiaBaoHiemThanhToan = bhytThanhToan;
                            donThuocThanhToanChiTiet.DonGiaBaoHiem = duocPhamBenhVienGiaBaoHiem.Gia;
                        }
                        //var tongTienBHYTThanhToan = TinhTongTienBHYTThanhToan(ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan);
                        //if (ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong != 100 && tongTienBHYTThanhToan > soTienBHYTSeThanhToanToanBo)
                        //{
                        //    if (tongTienBHYTThanhToan - bhytThanhToan * (decimal)(donThuocChiTiet.SoLuong - soLuongTruoc) > soTienBHYTSeThanhToanToanBo)
                        //    {
                        //        foreach (var donThuocThanhToanChiTiet in donThuocThanhToan.DonThuocThanhToanChiTiets.Where(o => o.Id == 0))
                        //        {
                        //            donThuocThanhToanChiTiet.GiaBaoHiemThanhToan = bhytThanhToan * ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong.GetValueOrDefault() / 100;
                        //            donThuocThanhToanChiTiet.DonGiaBaoHiem = bhytThanhToan;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        ApDungMucHuongMoiTatCaDichVu(ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan, ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong.GetValueOrDefault());
                        //    }
                        //}
                    }
                }
                else
                {
                    if (duocHuongBaoHiem)
                    {
                        //TinhGiaBHYTHuyDonThuoc(ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan);
                    }
                }
            }
            await _yeuCauKhamBenhDonThuocChiTietRepository.UpdateAsync(ycDonThuocChiTiet);
            //return (BitConverter.ToInt64(ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.LastModified), string.Empty);
            return string.Empty;
        }
        */
        public async Task<string> XoaDonThuocChiTiet(DonThuocChiTietVo donThuocChiTiet)
        {
            #region Cập nhật 09/12/2022: bỏ bớt include thừa
            //var ycDonThuocChiTiet = _yeuCauKhamBenhDonThuocChiTietRepository.GetById(donThuocChiTiet.DonThuocChiTietId,
            //    x => x.Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauKhamBenhs)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDichVuKyThuats)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDichVuGiuongBenhViens)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDuocPhamBenhViens)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauVatTuBenhViens)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocs)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.DonThuocThanhToans)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DuocPham).ThenInclude(dt => dt.DuocPhamBenhVien)
            //        .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuyetBaoHiemChiTiets)
            //        .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
            //        .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
            //        .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
            //        .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTs)
            //        .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonThuocThanhToan).ThenInclude(o => o.DonThuocThanhToanChiTiets)
            //        .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
            //        .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet));

            var ycDonThuocChiTiet = _yeuCauKhamBenhDonThuocChiTietRepository.GetById(donThuocChiTiet.DonThuocChiTietId,
                        x =>
                            x.Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocs)
                            //.Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTs)
                            .Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets)
                            .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuyetBaoHiemChiTiets)
                            .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                            .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                            .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                            .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonThuocThanhToan).ThenInclude(o => o.DonThuocThanhToanChiTiets)
                            .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                            .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet));
            #endregion

            ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.LastTime = DateTime.Now;
            //kiem tra truoc khi cap nhat
            if (ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
            {
                //return "Dược phẩm đã được thanh toán";
                //return GetResourceValueByResourceName("DonThuoc.ThuocDaThanhToan");
                return _localizationService.GetResource("DonThuoc.ThuocDaThanhToan");
            }
            if (ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc))
            {
                //return GetResourceValueByResourceName("DonThuoc.ThuocDaXuat");
                return _localizationService.GetResource("DonThuoc.ThuocDaXuat");
            }
            var duocHuongBaoHiem = false;
            if (ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.BaoHiemChiTra != false))
            {
                duocHuongBaoHiem = true;

            }
            //hoan lai duoc pham da book trong kho
            foreach (var donThuocThanhToanChiTiet in ycDonThuocChiTiet.DonThuocThanhToanChiTiets)
            {
                foreach (var duyetBaoHiemChiTiet in donThuocThanhToanChiTiet.DuyetBaoHiemChiTiets)
                {
                    duyetBaoHiemChiTiet.WillDelete = true;
                }
                foreach (var congNo in donThuocThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                {
                    congNo.WillDelete = true;
                }
                foreach (var mienGiam in donThuocThanhToanChiTiet.MienGiamChiPhis)
                {
                    mienGiam.WillDelete = true;
                }
                foreach (var taiKhoanBenhNhanChi in donThuocThanhToanChiTiet.TaiKhoanBenhNhanChis)
                {
                    taiKhoanBenhNhanChi.DonThuocThanhToanChiTietId = null;
                }
                donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                donThuocThanhToanChiTiet.WillDelete = true;
                donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                if (donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
                    donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;
                if (donThuocThanhToanChiTiet.DonThuocThanhToan.DonThuocThanhToanChiTiets.All(o => o.WillDelete))
                {
                    donThuocThanhToanChiTiet.DonThuocThanhToan.WillDelete = true;
                }
            }
            ycDonThuocChiTiet.WillDelete = true;
            if (ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhDonThuocChiTiets.All(o => o.WillDelete))
            {
                ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.WillDelete = true;
            }
            //bo duyet tu dong
            //if (duocHuongBaoHiem)
            //{
            //    DuyetBHYTChoDonThuoc(ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong);
            //}
            if (ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauKhamBenhDonThuocs.All(o => o.WillDelete)) // && !ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any())
            {
                //ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.CoKeToa = null;

                // chuyển sang select sau -> bỏ include lúc đầu
                var kiemTraCoDonVatTu = _yeuCauKhamBenhDonVTYTRepository.TableNoTracking.Any(x => x.YeuCauKhamBenhId == ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId);
                if(!kiemTraCoDonVatTu)
                {
                    ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.CoKeToa = null;
                }
            }

            await _yeuCauKhamBenhDonThuocChiTietRepository.UpdateAsync(ycDonThuocChiTiet);

            #region Cập nhật 09/12/2022: fix bug get thiếu data YeuCauKhamBenhDonThuocChiTiets trường hợp cập nhật lại stt
            //var yeuCauKhamBenh = ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh;
            var yeucauKhamBenh = BaseRepository.Table
                .Include(x => x.YeuCauKhamBenhDonThuocs).ThenInclude(x => x.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(X => X.DuocPham).ThenInclude(x => x.DuocPhamBenhVien)
                .Where(x => x.Id == ycDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId)
                .First();
            #endregion

            await XuLySoThuTu(yeucauKhamBenh);
            return string.Empty;
        }

        public async Task<string> XoaDonThuocTheoYeuCauKhamBenh(XoaDonThuocTheoYeuCauKhamBenhVo xoaDonThuocTheoYeuCauKhamBenh)
        {
            var yeuCauKhamBenh = BaseRepository.GetById(xoaDonThuocTheoYeuCauKhamBenh.YeuCauKhamBenhId,
                x => x.Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans)
                    .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets)
                    .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.DonVTYTThanhToans)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.YeuCauKhamBenhs)
                    .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDichVuKyThuats)
                    .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDichVuGiuongBenhViens)
                    .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDuocPhamBenhViens)
                    .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauVatTuBenhViens)
                    .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuyetBaoHiemChiTiets)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.MienGiamChiPhis)
                    .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.TaiKhoanBenhNhanChis)
                    .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
                    .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet)
                    .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.MienGiamChiPhis)
                    .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.TaiKhoanBenhNhanChis)
                    );

            if (yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any(o => o.DonVTYTThanhToans.Any(tt => tt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)))
            {
                //return "Có đơn vật tư đã được thanh toán";
                return GetResourceValueByResourceName("DonVTYT.VTYTDaThanhToan");
            }
            if (yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any(o => o.DonVTYTThanhToans.Any(tt => tt.TrangThai == Enums.TrangThaiDonVTYTThanhToan.DaXuatVTYT)))
            {
                return GetResourceValueByResourceName("DonVTYT.VatTuDaXuat");
            }
            foreach (var yeuCauKhamBenhDonVTYT in yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs)
            {
                foreach (var yeuCauKhamBenhDonVTYTChiTiet in yeuCauKhamBenhDonVTYT.YeuCauKhamBenhDonVTYTChiTiets)
                {
                    foreach (var donVTYTThanhToanChiTiet in yeuCauKhamBenhDonVTYTChiTiet.DonVTYTThanhToanChiTiets)
                    {
                        foreach (var congNo in donVTYTThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                        {
                            congNo.WillDelete = true;
                        }
                        foreach (var mienGiam in donVTYTThanhToanChiTiet.MienGiamChiPhis)
                        {
                            mienGiam.WillDelete = true;
                        }
                        foreach (var taiKhoanBenhNhanChi in donVTYTThanhToanChiTiet.TaiKhoanBenhNhanChis)
                        {
                            taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                        }
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                        donVTYTThanhToanChiTiet.WillDelete = true;
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                        if (donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null)
                            donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.WillDelete = true;
                    }
                    yeuCauKhamBenhDonVTYTChiTiet.WillDelete = true;
                }
                foreach (var donVTYTThanhToan in yeuCauKhamBenhDonVTYT.DonVTYTThanhToans)
                {
                    donVTYTThanhToan.WillDelete = true;
                }
                yeuCauKhamBenhDonVTYT.WillDelete = true;
            }


            if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any(o => o.DonThuocThanhToans.Any(tt => tt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)))
            {
                //return "Có đơn thuốc đã được thanh toán";
                return GetResourceValueByResourceName("DonThuoc.ThuocDaThanhToan");
            }
            if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any(o => o.DonThuocThanhToans.Any(tt => tt.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)))
            {
                return GetResourceValueByResourceName("DonThuoc.ThuocDaXuat");
            }

            var duocHuongBaoHiem = false;
            if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.SelectMany(o => o.YeuCauKhamBenhDonThuocChiTiets).SelectMany(o => o.DonThuocThanhToanChiTiets).Any(o => o.BaoHiemChiTra != false))
            {
                duocHuongBaoHiem = true;
            }

            foreach (var yeuCauKhamBenhDonThuoc in yeuCauKhamBenh.YeuCauKhamBenhDonThuocs)
            {
                foreach (var yeuCauKhamBenhDonThuocChiTiet in yeuCauKhamBenhDonThuoc.YeuCauKhamBenhDonThuocChiTiets)
                {
                    foreach (var donThuocThanhToanChiTiet in yeuCauKhamBenhDonThuocChiTiet.DonThuocThanhToanChiTiets)
                    {
                        foreach (var duyetBaoHiemChiTiet in donThuocThanhToanChiTiet.DuyetBaoHiemChiTiets)
                        {
                            duyetBaoHiemChiTiet.WillDelete = true;
                        }
                        foreach (var congNo in donThuocThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                        {
                            congNo.WillDelete = true;
                        }
                        foreach (var mienGiam in donThuocThanhToanChiTiet.MienGiamChiPhis)
                        {
                            mienGiam.WillDelete = true;
                        }
                        foreach (var taiKhoanBenhNhanChi in donThuocThanhToanChiTiet.TaiKhoanBenhNhanChis)
                        {
                            taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                        }
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                        donThuocThanhToanChiTiet.WillDelete = true;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                        if (donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
                            donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;
                    }
                    yeuCauKhamBenhDonThuocChiTiet.WillDelete = true;
                }
                foreach (var donThuocThanhToan in yeuCauKhamBenhDonThuoc.DonThuocThanhToans)
                {
                    donThuocThanhToan.WillDelete = true;
                }
                yeuCauKhamBenhDonThuoc.WillDelete = true;
            }
            yeuCauKhamBenh.CoKeToa = null;
            //bo duyet tu dong
            //if (duocHuongBaoHiem)
            //{
            //    DuyetBHYTChoDonThuoc(yeuCauKhamBenh.YeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong);
            //}
            await BaseRepository.UpdateAsync(yeuCauKhamBenh);
            return string.Empty;
        }

        public async Task<string> TangHoacGiamSTTDonThuocChiTiet(DonThuocChiTietTangGiamSTTVo donThuocChiTiet)
        {
            var ycKhamBenh = BaseRepository.GetById(donThuocChiTiet.YeuCauKhamBenhId, x =>
                    x.Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DuocPham).ThenInclude(dt => dt.DuocPhamBenhVien));
            var donThuoc = ycKhamBenh.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == donThuocChiTiet.LoaiDonThuoc);
            var listThuocChiTiet = ycKhamBenh.YeuCauKhamBenhDonThuocs.Where(z => z.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT).SelectMany(z => z.YeuCauKhamBenhDonThuocChiTiets)
                                    .Union(ycKhamBenh.YeuCauKhamBenhDonThuocs.Where(z => z.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).SelectMany(z => z.YeuCauKhamBenhDonThuocChiTiets));
            var dtChiTiet = listThuocChiTiet.Where(z => z.Id == donThuocChiTiet.DonThuocChiTietId).FirstOrDefault();
            if (dtChiTiet == null)
            {
                return GetResourceValueByResourceName("PhieuDieuTri.DonThuoc.NotExists");
            }
            else
            {
                if (donThuocChiTiet.LaTangSTT == true)
                {
                    var dtChiTietKeTiep = listThuocChiTiet.Where(z => z.SoThuTu == (dtChiTiet.SoThuTu + 1)).FirstOrDefault();
                    if (dtChiTietKeTiep != null)
                    {
                        dtChiTiet.SoThuTu++;
                        dtChiTietKeTiep.SoThuTu--;
                    }
                    else
                    {
                        return GetResourceValueByResourceName("DonThuoc.STT.KhongTheTang");
                    }
                }
                else
                {
                    var dtChiTietDangTruoc = listThuocChiTiet.Where(z => z.SoThuTu == (dtChiTiet.SoThuTu - 1)).FirstOrDefault();
                    if (dtChiTietDangTruoc != null)
                    {
                        dtChiTiet.SoThuTu--;
                        dtChiTietDangTruoc.SoThuTu++;
                    }
                    else
                    {
                        return GetResourceValueByResourceName("DonThuoc.STT.KhongTheGiam");
                    }
                }
                await BaseRepository.UpdateAsync(ycKhamBenh);
            }

            return string.Empty;
        }
        public async Task<bool> KiemTraDonThuocDaXuatHayDaThanhToan(XoaDonThuocTheoYeuCauKhamBenhVo xoaDonThuocTheoYeuCauKhamBenh)
        {
            var yeuCauKhamBenh = BaseRepository.GetById(xoaDonThuocTheoYeuCauKhamBenh.YeuCauKhamBenhId,
               x => x.Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets)
                   .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans)
                   .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets)
                   .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.DonVTYTThanhToans));

            if (yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any(o => o.DonVTYTThanhToans.Any(tt => tt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)))
            {
                //return "Có đơn vật tư đã được thanh toán";
                //return GetResourceValueByResourceName("DonVTYT.VTYTDaThanhToan");
                return false;
            }
            if (yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any(o => o.DonVTYTThanhToans.Any(tt => tt.TrangThai == Enums.TrangThaiDonVTYTThanhToan.DaXuatVTYT)))
            {
                //return GetResourceValueByResourceName("DonVTYT.VatTuDaXuat");
                return false;
            }

            if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any(o => o.DonThuocThanhToans.Any(tt => tt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)))
            {
                //return "Có đơn thuốc đã được thanh toán";
                //return GetResourceValueByResourceName("DonThuoc.ThuocDaThanhToan");
                return false;
            }
            if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any(o => o.DonThuocThanhToans.Any(tt => tt.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)))
            {
                //return GetResourceValueByResourceName("DonThuoc.ThuocDaXuat");
                return false;
            }
            return true;
        }
        private DataBenhNhan ThongTinBenhNhanPhieuThuoc(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            var thongTinBenhNhanPhieuThuoc = _yeuCauTiepNhanRepository.TableNoTracking
                           .Where(s => s.YeuCauKhamBenhs.Any(y => y.Id == yeuCauKhamBenhId))
                           .Select(s => new DataBenhNhan
                           {
                               MaTN = s.MaYeuCauTiepNhan,
                               Id = s.BenhNhan.Id,
                               HoTen = s.HoTen,
                               NamSinh = s.NamSinh,
                               Tuoi = s.NamSinh != null ? (DateTime.Now.Year - s.NamSinh) : null,
                               CanNang = s.KetQuaSinhHieus.Count != 0 && s.KetQuaSinhHieus.FirstOrDefault() != null
                                                  ? (s.KetQuaSinhHieus.OrderByDescending(p => p.Id).Where(p => p.CanNang != null).FirstOrDefault().CanNang.ToString() + " kg") : null,
                               GioiTinh = s.GioiTinh.GetDescription(),
                               NamSinhDayDu = DateHelper.DOBFormat(s.NgaySinh, s.ThangSinh, s.NamSinh),
                               DiaChi = s.DiaChiDayDu,
                               BHYTMaSoThe = s.BHYTMaSoThe,
                               BHYTMaDKBD = s.BHYTMaDKBD,
                               ChuanDoan = s.YeuCauKhamBenhs.Where(p => p.Id == yeuCauKhamBenhId).Select(p => p.Icdchinh.Ma + " - " + p.Icdchinh.TenTiengViet + (!string.IsNullOrEmpty(p.GhiChuICDChinh) ? " (" + p.GhiChuICDChinh + ")" : "")).FirstOrDefault()
                                + "; " + string.Join("; ", s.YeuCauKhamBenhs.Where(p => p.Id == yeuCauKhamBenhId).SelectMany(z => z.YeuCauKhamBenhICDKhacs).Select(x => x.ICD.Ma + " - " + x.ICD.TenTiengViet + (!string.IsNullOrEmpty(x.GhiChu) ? " (" + x.GhiChu + ")" : ""))),
                               LoiDan = s.YeuCauKhamBenhs.Where(p => p.Id == yeuCauKhamBenhId).SelectMany(p => p.YeuCauKhamBenhDonThuocs).Select(dt => dt.GhiChu).FirstOrDefault().Replace("\n", "<br>"),
                               BHYTNgayHieuLuc = s.BHYTNgayHieuLuc,
                               BHYTNgayHetHan = s.BHYTNgayHetHan,
                               CMND = s.BenhNhan.SoChungMinhThu,
                               NguoiGiamHo = s.NguoiLienHeHoTen,
                               SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                               MaBN = s.BenhNhan.MaBN,
                               SoThang = CalculateHelper.TinhTongSoThangCuaTuoi(s.NgaySinh, s.ThangSinh, s.NamSinh)
                           }).FirstOrDefault();
            return thongTinBenhNhanPhieuThuoc;
        }


        public string InDonThuocKhamBenh(InToaThuocReOrder inToaThuoc)
        {
            var infoBN = ThongTinBenhNhanPhieuThuoc(inToaThuoc.YeuCauTiepNhanId, inToaThuoc.YeuCauKhamBenhId);

            var templateDonThuocBHYT = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYTTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYT")).First();
            var templateDonThuocTrongBenhVien = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYTTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYT")).First();
            var templateDonThuocThucPhamChucNang = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocThucPhamChucNang")).FirstOrDefault();
            var templateDonThuocNgoaiBenhVien = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBVTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBV")).First();
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();

            //var listDonThuocBHYT = inToaThuoc.ListGridThuoc.Where(p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            //var listDonThuocKhongBHYT = inToaThuoc.ListGridThuoc.Where(p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).ToList();
            //Dictionary<long?, int> dictionaryBHYT = new Dictionary<long?, int>();
            //Dictionary<long?, int> dictionaryKhongBHYT = new Dictionary<long?, int>();
            ////sort Grid hiện tại theo Grid truyền vào
            //dictionaryBHYT = listDonThuocBHYT
            //    .Select((id, index) => new
            //    {
            //        key = (long?)id.Id,
            //        rank = index
            //    }).ToDictionary(o => o.key, o => o.rank);
            //dictionaryKhongBHYT = listDonThuocKhongBHYT
            //    .Select((id, index) => new
            //    {
            //        key = (long?)id.Id,
            //        rank = index
            //    }).ToDictionary(o => o.key, o => o.rank);
            //Thuốc trong BV
            var donThuocBHYTChiTiets = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == inToaThuoc.YeuCauKhamBenhId && z.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                .Select(s => new InNoiTruDonThuocChiTietVo
                                {
                                    STT = s.SoThuTu,
                                    Id = s.Id,
                                    Ten = s.Ten,
                                    MaHoatChat = s.MaHoatChat,
                                    HoatChat = s.HoatChat,
                                    HamLuong = s.HamLuong,
                                    TenDuongDung = s.DuongDung.Ten,
                                    DVT = s.DonViTinh.Ten,
                                    SoLuong = s.SoLuong,
                                    SoNgayDung = s.SoNgayDung,
                                    ThoiGianDungSang = s.ThoiGianDungSang,
                                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                                    ThoiGianDungToi = s.ThoiGianDungToi,
                                    // thuốc gây nghiện,hướng thần thì cách dùng con số chuyển thành text , ngược lại nếu thuống thường kiểm tra sl kê nhỏ hơn 10 thì thêm 0 phía trước , còn lại bình thường
                                    DungSang = s.DungSang,
                                    DungTrua = s.DungTrua,
                                    DungChieu = s.DungChieu,
                                    DungToi = s.DungToi,
                                    ThoiDiemKeDon = s.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.YeuCauKhamBenhDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).OrderBy(p => p.STT ?? 0).ToList();
            var donThuocBHYTs = donThuocBHYTChiTiets;
            var donThuocKhongBHYTChiTiets = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == inToaThuoc.YeuCauKhamBenhId && z.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT)
                                .Select(s => new InNoiTruDonThuocChiTietVo
                                {
                                    STT = s.SoThuTu,
                                    Id = s.Id,
                                    Ten = s.Ten,
                                    MaHoatChat = s.MaHoatChat,
                                    HoatChat = s.HoatChat,
                                    HamLuong = s.HamLuong,
                                    TenDuongDung = s.DuongDung.Ten,
                                    DVT = s.DonViTinh.Ten,
                                    SoLuong = s.SoLuong,
                                    SoNgayDung = s.SoNgayDung,
                                    ThoiGianDungSang = s.ThoiGianDungSang,
                                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                                    ThoiGianDungToi = s.ThoiGianDungToi,
                                    DungSang = s.DungSang,
                                    DungTrua = s.DungTrua,
                                    DungChieu = s.DungChieu,
                                    DungToi = s.DungToi,
                                    ThoiDiemKeDon = s.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.YeuCauKhamBenhDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).ToList();
            foreach (var thuoc in donThuocKhongBHYTChiTiets)
            {
                thuoc.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(thuoc.DuocPhamBenhVienPhanNhomId.GetValueOrDefault(), duocPhamBenhVienPhanNhoms);
            }

            var userCurrentId = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.First().BacSiKeDonId : (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.First().BacSiKeDonId : 0);

            var tenBacSiKeDon = _userRepository.TableNoTracking
                             .Where(u => u.Id == userCurrentId).Select(u =>
                             (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                           //+ (u.NhanVien.ChucDanh != null ? u.NhanVien.ChucDanh.NhomChucDanh.Ma + "." : "")
                           + u.HoTen).FirstOrDefault();
            var donThuocTrongBVs = donThuocKhongBHYTChiTiets.Where(z => z.LaDuocPhamBenhVien).OrderBy(z => z.STT).ToList();
            var donThuocNgoaiBVs = donThuocKhongBHYTChiTiets.Where(z => !z.LaDuocPhamBenhVien).OrderBy(z => z.STT).ToList();

            var headerBHYT = string.Empty;
            var headerKhongBHYT = string.Empty;
            var headerThuocNgoaiBV = string.Empty;
            var headerThucPhamChucNang = string.Empty;

            if (inToaThuoc.Header)
            {
                headerBHYT = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>TOA THUỐC BẢO HIỂM Y TẾ</th>" +
                         "</p>";
                headerKhongBHYT = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>TOA THUỐC KHÔNG BẢO HIỂM Y TẾ</th>" +
                         "</p>";

                headerThuocNgoaiBV = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                             "<th>TOA THUỐC NGOÀI BỆNH VIỆN</ th>" +
                             "</p>";

                headerThucPhamChucNang = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                                 "<th>ĐƠN TƯ VẤN</ th>" +
                                 "</p>";
            }

            var contentThuocTrongBenhVien = string.Empty;
            var contentThuocNgoaiBenhVien = string.Empty;
            var contentThuocBHYT = string.Empty;
            var contentThucPhamChucNang = string.Empty;

            var resultThuocTrongBenhVien = string.Empty;
            var resultThuocNgoaiBenhVien = string.Empty;
            var resultThuocBHYT = string.Empty;
            var resultThuocThucPhamChucNang = string.Empty;
            var content = string.Empty;
            var sttBHYT = 0;

            var sttKhongBHYTTrongBV = 0;
            var sttKhongBHYTNgoaiBV = 0;
            var sttTPCN = 0;

            if (donThuocBHYTs.Any())
            {
                foreach (var donThuocBHYTChiTiet in donThuocBHYTs)
                {
                    var cd =
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungSangDisplay)
                                 ? "Sáng " + donThuocBHYTChiTiet.DungSang
                                 +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungSangDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungSangDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungTruaDisplay)
                                 ? "Trưa " + donThuocBHYTChiTiet.DungTrua +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungTruaDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungTruaDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungChieuDisplay)
                                 ? "Chiều " + donThuocBHYTChiTiet.DungChieu +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungChieuDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungChieuDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                            (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungToiDisplay)
                                 ? "Tối " + donThuocBHYTChiTiet.DungToi +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungToiDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungToiDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "");

                    var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                                 + (!string.IsNullOrEmpty(donThuocBHYTChiTiet.CachDung) ? "<p style='margin:0'><i>" + donThuocBHYTChiTiet.CachDung + " </i></p>" : "");
                    sttBHYT++;
                    resultThuocBHYT += "<tr>";
                    resultThuocBHYT += "<td style='vertical-align: top; text-align: center' >" + sttBHYT + "</td>";
                    resultThuocBHYT += "<td >" + FormatTenDuocPham(donThuocBHYTChiTiet.Ten, donThuocBHYTChiTiet.HoatChat, donThuocBHYTChiTiet.HamLuong, donThuocBHYTChiTiet.DuocPhamBenhVienPhanNhomId)
                        + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                        + "</td>";

                    resultThuocBHYT += "<td  style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocBHYTChiTiet.SoLuong, donThuocBHYTChiTiet.LoaiThuocTheoQuanLy) + " " + donThuocBHYTChiTiet.DVT + "</td>";
                    resultThuocBHYT += "</tr>";
                }

                if (!string.IsNullOrEmpty(resultThuocBHYT))
                {
                    resultThuocBHYT = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocBHYT + "</tbody></table>";
                    var data = new DataYCKBDonThuoc
                    {
                        Header = headerBHYT,
                        TemplateDonThuoc = resultThuocBHYT,
                        LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                        MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                        HoTen = infoBN.HoTen,
                        Tuoi = infoBN.Tuoi,
                        NamSinhDayDu = infoBN.NamSinhDayDu,
                        CanNang = infoBN.CanNang,
                        GioiTinh = infoBN?.GioiTinh,
                        DiaChi = infoBN?.DiaChi,
                        CMND = infoBN?.CMND,
                        SoTheBHYT = infoBN.BHYTMaSoThe,
                        NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                        NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                        ChuanDoan = inToaThuoc.IsChangeChanDoan == true ? inToaThuoc.ChanDoan + "; " + (inToaThuoc.YeuCauKhamBenhICDKhacs.Any() ? string.Join("; ", inToaThuoc.YeuCauKhamBenhICDKhacs.Select(z => z.ChanDoanKemTheo).Distinct()) : "") : infoBN?.ChuanDoan,
                        ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),
                        BacSiKham = tenBacSiKeDon,
                        LoiDan = infoBN.LoiDan,
                        MaBN = infoBN.MaBN,
                        SoDienThoai = infoBN.SoDienThoai,
                        SoThang = infoBN.SoThang,
                        CongKhoan = sttBHYT,
                        //KhoaPhong = khoaPhong
                    };
                    contentThuocBHYT = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocBHYT.Body, data);
                }

            }

            if (donThuocKhongBHYTChiTiets.Any())
            {
                if (donThuocTrongBVs.Any())
                {
                    foreach (var donThuocTrongBV in donThuocTrongBVs)
                    {
                        var cd =
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungSangDisplay)

                                     ? "Sáng " + donThuocTrongBV.DungSang +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungSangDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungSangDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungTruaDisplay)

                                     ? "Trưa " + donThuocTrongBV.DungTrua +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungTruaDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungTruaDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungChieuDisplay)

                                     ? "Chiều " + donThuocTrongBV.DungChieu +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungChieuDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungChieuDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungToiDisplay)

                                     ? "Tối " + donThuocTrongBV.DungToi +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungToiDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungToiDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "");

                        var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                               + (!string.IsNullOrEmpty(donThuocTrongBV.CachDung) ? "<p style='margin:0'><i>" + donThuocTrongBV.CachDung + " </i></p>" : "");
                        if (donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.MyPham
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.VatTuYTe
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThuocTuDuocLieu)
                        {
                            sttTPCN++;
                            resultThuocThucPhamChucNang += "<tr>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + sttTPCN + "</td>";
                            resultThuocThucPhamChucNang += "<td >" + "<b>" + donThuocTrongBV.Ten + "</b>"
                             + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";


                        }
                        else
                        {
                            sttKhongBHYTTrongBV++;
                            resultThuocTrongBenhVien += "<tr>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTTrongBV + "</td>";
                            resultThuocTrongBenhVien += "<td >" + FormatTenDuocPham(donThuocTrongBV.Ten, donThuocTrongBV.HoatChat, donThuocTrongBV.HamLuong, donThuocTrongBV.DuocPhamBenhVienPhanNhomId)
                                 + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center'  >" + FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocTrongBenhVien += "</tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(resultThuocTrongBenhVien))
                    {
                        resultThuocTrongBenhVien = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocTrongBenhVien + "</tbody></table>";
                        var data = new DataYCKBDonThuoc
                        {
                            Header = headerKhongBHYT,
                            TemplateDonThuoc = resultThuocTrongBenhVien,
                            LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                            MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                            HoTen = infoBN.HoTen,
                            NamSinhDayDu = infoBN.NamSinhDayDu,

                            Tuoi = infoBN.Tuoi,
                            CMND = infoBN?.CMND,
                            CanNang = infoBN.CanNang,
                            GioiTinh = infoBN?.GioiTinh,
                            DiaChi = infoBN?.DiaChi,
                            SoTheBHYT = infoBN.BHYTMaSoThe,
                            NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                            NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                            //ChuanDoan = infoBN?.ChuanDoan,
                            ChuanDoan = inToaThuoc.IsChangeChanDoan == true ? inToaThuoc.ChanDoan + "; " + (inToaThuoc.YeuCauKhamBenhICDKhacs.Any() ? string.Join("; ", inToaThuoc.YeuCauKhamBenhICDKhacs.Select(z => z.ChanDoanKemTheo).Distinct()) : "") : infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = infoBN.LoiDan,
                            NguoiGiamHo = infoBN?.NguoiGiamHo,
                            MaBN = infoBN.MaBN,
                            SoDienThoai = infoBN.SoDienThoai,
                            SoThang = infoBN.SoThang,
                            CongKhoan = sttKhongBHYTTrongBV,
                            //KhoaPhong = khoaPhong,
                            ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                        };
                        contentThuocTrongBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocTrongBenhVien.Body, data);
                    }

                }
                if (donThuocNgoaiBVs.Any())
                {
                    foreach (var donThuocNgoaiBV in donThuocNgoaiBVs)
                    {
                        var cd =
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungSangDisplay)

                                     ? "Sáng " + donThuocNgoaiBV.DungSang +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungSangDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungSangDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungTruaDisplay)

                                     ? "Trưa " + donThuocNgoaiBV.DungTrua +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungTruaDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungTruaDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungChieuDisplay)

                                     ? "Chiều " + donThuocNgoaiBV.DungChieu +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungChieuDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungChieuDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungToiDisplay)
                                     ? "Tối " + donThuocNgoaiBV.DungToi +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungToiDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungToiDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "");

                        var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                              + (!string.IsNullOrEmpty(donThuocNgoaiBV.CachDung) ? "<p style='margin:0'><i>" + donThuocNgoaiBV.CachDung + " </i></p>" : "");
                        if (donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.MyPham
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.VatTuYTe
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThuocTuDuocLieu)
                        {
                            sttTPCN++;
                            resultThuocThucPhamChucNang += "<tr>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + sttTPCN + "</td>";
                            resultThuocThucPhamChucNang += "<td >" + "<b>" + donThuocNgoaiBV.Ten + "</b>"
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")

                                + "</td>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";
                        }
                        else
                        {
                            sttKhongBHYTNgoaiBV++;
                            resultThuocNgoaiBenhVien += "<tr>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTNgoaiBV + "</td>";
                            resultThuocNgoaiBenhVien += "<td >" + FormatTenDuocPham(donThuocNgoaiBV.Ten, donThuocNgoaiBV.HoatChat, donThuocNgoaiBV.HamLuong, donThuocNgoaiBV.DuocPhamBenhVienPhanNhomId)
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocNgoaiBenhVien += "</tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(resultThuocNgoaiBenhVien))
                    {
                        resultThuocNgoaiBenhVien = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocNgoaiBenhVien + "</tbody></table>";
                        var data = new DataYCKBDonThuoc
                        {
                            Header = headerKhongBHYT,
                            TemplateDonThuoc = resultThuocNgoaiBenhVien,
                            LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                            MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                            HoTen = infoBN.HoTen,
                            NamSinhDayDu = infoBN.NamSinhDayDu,
                            Tuoi = infoBN.Tuoi,
                            CMND = infoBN?.CMND,
                            CanNang = infoBN.CanNang,
                            GioiTinh = infoBN?.GioiTinh,
                            DiaChi = infoBN?.DiaChi,
                            SoTheBHYT = infoBN.BHYTMaSoThe,
                            NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                            NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                            //ChuanDoan = infoBN?.ChuanDoan,
                            ChuanDoan = inToaThuoc.IsChangeChanDoan == true ? inToaThuoc.ChanDoan + "; " + (inToaThuoc.YeuCauKhamBenhICDKhacs.Any() ? string.Join("; ", inToaThuoc.YeuCauKhamBenhICDKhacs.Select(z => z.ChanDoanKemTheo).Distinct()) : "") : infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = infoBN.LoiDan,
                            NguoiGiamHo = infoBN?.NguoiGiamHo,
                            MaBN = infoBN.MaBN,
                            SoDienThoai = infoBN.SoDienThoai,
                            SoThang = infoBN.SoThang,
                            CongKhoan = sttKhongBHYTNgoaiBV,
                            //KhoaPhong = khoaPhong,
                            ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                        };
                        contentThuocNgoaiBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocNgoaiBenhVien.Body, data);
                    }
                }
            }
            if (!string.IsNullOrEmpty(resultThuocThucPhamChucNang))
            {
                resultThuocThucPhamChucNang = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên sản phẩm – Cách dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocThucPhamChucNang + "</tbody></table>";
                var data = new DataYCKBDonThuoc
                {
                    Header = headerThucPhamChucNang,
                    TemplateDonThuoc = resultThuocThucPhamChucNang,
                    LogoUrl = inToaThuoc.HostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                    MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                    HoTen = infoBN.HoTen,
                    NamSinhDayDu = infoBN.NamSinhDayDu,
                    Tuoi = infoBN.Tuoi,
                    CanNang = infoBN.CanNang,
                    GioiTinh = infoBN?.GioiTinh,
                    DiaChi = infoBN?.DiaChi,
                    SoTheBHYT = infoBN.BHYTMaSoThe,
                    //ChuanDoan = infoBN?.ChuanDoan,
                    ChuanDoan = inToaThuoc.IsChangeChanDoan == true ? inToaThuoc.ChanDoan + "; " + (inToaThuoc.YeuCauKhamBenhICDKhacs.Any() ? string.Join("; ", inToaThuoc.YeuCauKhamBenhICDKhacs.Select(z => z.ChanDoanKemTheo).Distinct()) : "") : infoBN?.ChuanDoan,
                    BacSiKham = tenBacSiKeDon,
                    LoiDan = infoBN.LoiDan,
                    MaBN = infoBN.MaBN,
                    SoDienThoai = infoBN.SoDienThoai,
                    SoThang = infoBN.SoThang,
                    CongKhoan = sttTPCN,
                    //KhoaPhong = khoaPhong,
                    ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                };
                contentThucPhamChucNang = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocThucPhamChucNang.Body, data);
            }
            if (contentThuocBHYT != "")
            {
                contentThuocBHYT = contentThuocBHYT + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThuocTrongBenhVien != "")
            {
                contentThuocTrongBenhVien = contentThuocTrongBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThuocNgoaiBenhVien != "")
            {
                contentThuocNgoaiBenhVien = contentThuocNgoaiBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThucPhamChucNang != "")
            {
                contentThucPhamChucNang = contentThucPhamChucNang + "<div class=\"pagebreak\"> </div>";
            }
            content = contentThuocBHYT + contentThuocTrongBenhVien + contentThuocNgoaiBenhVien + contentThucPhamChucNang;
            content = content.Remove(content.Length - 30);
            return content;
        }
        public async Task<bool> KiemTraCoDonThuoc(long yeuCauKhamBenhId)
        {
            var kTraDonThuoc = await _yeuCauKhamBenhDonThuocRepository.TableNoTracking
                                .Where(dt => dt.YeuCauKhamBenhId == yeuCauKhamBenhId)
                                .Select(dt => dt).ToListAsync();
            var kTraVatTu = await _yeuCauKhamBenhDonVTYTRepository.TableNoTracking
                                .Where(dt => dt.YeuCauKhamBenhId == yeuCauKhamBenhId)
                                .Select(dt => dt).ToListAsync();
            if (kTraDonThuoc.Any() || kTraVatTu.Any())
            {
                return true;
            }
            return false;
        }

        public async Task<bool> KiemTraCoBHYT(long yeuCauTiepNhanId)

        {
            var kTraCoBHYT = await _yeuCauTiepNhanRepository.TableNoTracking
                                .Where(yctn => yctn.Id == yeuCauTiepNhanId)
                                .Select(dt => dt.CoBHYT).FirstOrDefaultAsync();
            if (kTraCoBHYT != null)
            {
                return true;
            }
            return false;
        }
        public async Task<GridDataSource> GetDataForGridAsyncToaThuocMau(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _toaThuocMauRepository.TableNoTracking
                       .Where(o => o.IsDisabled != true)
                       .Select(s => new ToaThuocMauGridVo
                       {
                           Id = s.Id,
                           ICDId = s.ICDId,
                           TrieuChungId = s.TrieuChungId,
                           ChuanDoanId = s.ChuanDoanId,
                           BacSiKeToaId = s.BacSiKeToaId,
                           BacSiKeToa = s.BacSiKeToa.User.HoTen,
                           ChuanDoanICD = s.ICD.Ma + " - " + s.ICD.TenTiengViet,
                           IsDisabled = s.IsDisabled,
                           SuDung = s.YeuCauKhamBenhDonThuocs.Select(p => p.ToaThuocMauId) == null ? 0 : s.YeuCauKhamBenhDonThuocs.Select(p => p.ToaThuocMauId).Count(),
                           TenToaMau = s.Ten,
                           GhiChu = s.GhiChu,
                       });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncToaThuocMau(QueryInfo queryInfo)
        {
            var query = _toaThuocMauRepository.TableNoTracking
                       .Where(o => o.IsDisabled != true)
                       .Select(s => new ToaThuocMauGridVo
                       {
                           Id = s.Id,
                           ICDId = s.ICDId,
                           TrieuChungId = s.TrieuChungId,
                           ChuanDoanId = s.ChuanDoanId,
                           BacSiKeToaId = s.BacSiKeToaId,
                           BacSiKeToa = s.BacSiKeToa.User.HoTen,
                           ChuanDoanICD = s.ICD.Ma + " - " + s.ICD.TenTiengViet,
                           IsDisabled = s.IsDisabled,
                           SuDung = s.YeuCauKhamBenhDonThuocs.Select(p => p.ToaThuocMauId) == null ? 0 : s.YeuCauKhamBenhDonThuocs.Select(p => p.ToaThuocMauId).Count(),
                           TenToaMau = s.Ten,
                           GhiChu = s.GhiChu,
                       });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public GridDataSource GetDataForGridToaThuocMauChiTietChild(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = _toaThuocMauChiTietRepository.TableNoTracking
                        .Where(o => o.ToaThuocMauId == long.Parse(queryInfo.SearchTerms))
                        .Select(s => new ToaThuocMauChiTietGridVo
                        {
                            Id = s.Id,
                            ToaThuocMauId = s.ToaThuocMauId,
                            DuocPhamId = s.DuocPhamId,
                            SoLuong = s.SoLuong,
                            SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                            SoNgayDung = s.SoNgayDung,
                            SangDisplay = s.DungSang.FloatToStringFraction() + "  " + (s.ThoiGianDungSang == null ? "" : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")"),
                            TruaDisplay = s.DungTrua.FloatToStringFraction() + "  " + (s.ThoiGianDungTrua == null ? "" : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")"),
                            ChieuDisplay = s.DungChieu.FloatToStringFraction() + "  " + (s.ThoiGianDungChieu == null ? "" : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")"),
                            ToiDisplay = s.DungToi.FloatToStringFraction() + "  " + (s.ThoiGianDungToi == null ? "" : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")"),
                            Ten = s.DuocPham.Ten,
                            Ma = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.Ma : s.DuocPham.MaHoatChat,
                            HoatChat = s.DuocPham.HoatChat,
                            DVT = s.DuocPham.DonViTinh.Ten,
                            TenDuongDung = s.DuocPham.DuongDung.Ten,
                            GhiChu = s.GhiChu
                        });
            var lstQuery = query.ToList();
            for (int i = 0; i < lstQuery.Count(); i++)
            {
                lstQuery[i].STT = i + 1;
                var duocPham = _duocPhamRepository.GetById(lstQuery[i].DuocPhamId.Value,
                    x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams)
                        .Include(o => o.DonViTinh)
                        .Include(o => o.DuongDung)
                        .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
                bool checkBHYT = false;
                var loaiKhoThuoc = LoaiKhoThuoc.ThuocNgoaiBenhVien;
                if (duocPham.DuocPhamBenhVien != null)
                {
                    checkBHYT = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(o => o.LaDuocPhamBHYT && (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT) && o.SoLuongDaXuat < o.SoLuongNhap);
                    loaiKhoThuoc = LoaiKhoThuoc.ThuocBenhVien;
                }
                if (bool.Parse(queryInfo.AdditionalSearchString) && checkBHYT)
                {
                    loaiKhoThuoc = LoaiKhoThuoc.ThuocBHYT;
                }
                lstQuery[i].NhomToaMau = (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT");
            }
            query = lstQuery.OrderBy(p => p.STT).AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public GridDataSource GetTotalPageForToaThuocMauChiTietChild(QueryInfo queryInfo)
        {
            //var query = _toaThuocMauChiTietRepository.TableNoTracking
            //            .Where(o => o.ToaThuocMauId == long.Parse(queryInfo.SearchTerms))
            //            .Select(s => new ToaThuocMauChiTietGridVo
            //            {
            //                Id = s.Id,
            //                ToaThuocMauId = s.ToaThuocMauId,
            //                DuocPhamId = s.DuocPhamId,
            //                SoLuong = s.SoLuong,
            //                SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
            //                SoNgayDung = s.SoNgayDung,
            //                SangDisplay = s.DungSang.FloatToStringFraction() + "  " + (s.ThoiGianDungSang == null ? "" : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")"),
            //                TruaDisplay = s.DungTrua.FloatToStringFraction() + "  " + (s.ThoiGianDungTrua == null ? "" : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")"),
            //                ChieuDisplay = s.DungChieu.FloatToStringFraction() + "  " + (s.ThoiGianDungChieu == null ? "" : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")"),
            //                ToiDisplay = s.DungToi.FloatToStringFraction() + "  " + (s.ThoiGianDungToi == null ? "" : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")"),
            //                Ten = s.DuocPham.Ten,
            //                Ma = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.Ma : s.DuocPham.MaHoatChat,
            //                HoatChat = s.DuocPham.HoatChat,
            //                DVT = s.DuocPham.DonViTinh.Ten,
            //                TenDuongDung = s.DuocPham.DuongDung.Ten,
            //            });
            //var lstQuery = query.ToList();
            //for (int i = 0; i < lstQuery.Count(); i++)
            //{
            //    lstQuery[i].STT = i + 1;
            //    var duocPham = _duocPhamRepository.GetById(lstQuery[i].DuocPhamId.Value,
            //        x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams)
            //            .Include(o => o.DonViTinh)
            //            .Include(o => o.DuongDung)
            //            .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
            //    bool checkBHYT = false;
            //    var loaiKhoThuoc = LoaiKhoThuoc.ThuocNgoaiBenhVien;
            //    if (duocPham.DuocPhamBenhVien != null)
            //    {
            //        checkBHYT = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(o => o.LaDuocPhamBHYT && (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT) && o.SoLuongDaXuat < o.SoLuongNhap);
            //        loaiKhoThuoc = LoaiKhoThuoc.ThuocBenhVien;
            //    }
            //    if (bool.Parse(queryInfo.AdditionalSearchString) && checkBHYT)
            //    {
            //        loaiKhoThuoc = LoaiKhoThuoc.ThuocBHYT;
            //    }
            //    lstQuery[i].NhomToaMau = (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT");
            //}
            //query = lstQuery.OrderBy(p => p.STT).AsQueryable();
            //var countTask = query.Count();
            //return new GridDataSource { TotalRowCount = countTask };
            return null;
        }

        public async Task<GridDataSource> GetDataForGridAsyncLichSuKeToa(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var benhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = BaseRepository.TableNoTracking
                        .Where(yckb => yckb.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && yckb.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && yckb.YeuCauKhamBenhDonThuocs.Count() > 0)
                        .Select(s => new LichSuKeToaGridVo
                        {
                            Id = s.Id,
                            ThoiDiemHoanThanh = s.ThoiDiemHoanThanh == null ? null : s.ThoiDiemHoanThanh.Value.ApplyFormatDate(),
                            MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ChuanDoanICD = s.Icdchinh.Ma + " - " + s.Icdchinh.TenTiengViet,
                            BacSiKham = s.BacSiThucHien.User.HoTen,
                        });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncLichSuKeToa(QueryInfo queryInfo)
        {
            var benhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var query = BaseRepository.TableNoTracking
                        .Where(yckb => yckb.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && yckb.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && yckb.YeuCauKhamBenhDonThuocs.Count() > 0)
                        .Select(s => new LichSuKeToaGridVo
                        {
                            Id = s.Id,
                            ThoiDiemHoanThanh = s.ThoiDiemHoanThanh == null ? null : s.ThoiDiemHoanThanh.Value.ApplyFormatDate(),
                            MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ChuanDoanICD = s.Icdchinh.Ma + " - " + s.Icdchinh.TenTiengViet,
                            BacSiKham = s.BacSiThucHien.User.HoTen,
                        });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public GridDataSource GetDataForGridLichSuKeToaChild(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var benhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var yeuCauKhamBenhId = long.Parse(queryInfo.SearchTerms);
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                        .Where(dtct => dtct.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham &&
                                    dtct.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId == benhNhanId
                                    && dtct.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == yeuCauKhamBenhId)
                        .Select(s => new LichSuKeToaChildGridVo
                        {
                            Id = s.Id,
                            DuocPhamId = s.DuocPhamId,
                            Ten = s.DuocPham.Ten,
                            Ma = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.Ma : s.DuocPham.MaHoatChat,
                            HoatChat = s.DuocPham.HoatChat,
                            DVT = s.DonViTinh.Ten,
                            LoaiDonThuoc = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc,
                            NhomLSKT = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                            SangDisplay = s.DungSang.FloatToStringFraction() + " " + (s.ThoiGianDungSang == null ? "" : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")"),
                            TruaDisplay = s.DungTrua.FloatToStringFraction() + " " + (s.ThoiGianDungTrua == null ? "" : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")"),
                            ChieuDisplay = s.DungChieu.FloatToStringFraction() + " " + (s.ThoiGianDungChieu == null ? "" : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")"),
                            ToiDisplay = s.DungToi.FloatToStringFraction() + " " + (s.ThoiGianDungToi == null ? "" : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")"),
                            SoLuong = s.SoLuong,
                            SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                            SoNgayDung = s.SoNgayDung,
                            TenDuongDung = s.DuongDung.Ten,
                            GhiChu = s.GhiChu
                        });
            var lstQuery = query.ToList();
            var lstBHYTSTT = lstQuery.Where((p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)).Select(p => p).ToList();
            var lstKhongBHYTSTT = lstQuery.Where((p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT)).Select(p => p).ToList();
            for (int i = 0; i < lstBHYTSTT.Count(); i++)
            {
                lstBHYTSTT[i].STT = i + 1;
            }
            for (int i = 0; i < lstKhongBHYTSTT.Count(); i++)
            {
                lstKhongBHYTSTT[i].STT = i + 1;
            }
            query = lstQuery.OrderBy(p => p.STT).AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public GridDataSource GetTotalPageForLichSuKeToaChild(QueryInfo queryInfo)
        {
            var benhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var yeuCauKhamBenhId = long.Parse(queryInfo.SearchTerms);
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                          .Where(dtct => dtct.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                    && dtct.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId == benhNhanId
                                    && dtct.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == yeuCauKhamBenhId)
                        .Select(s => new LichSuKeToaChildGridVo
                        {
                            Id = s.Id,
                            DuocPhamId = s.DuocPhamId,
                            Ten = s.DuocPham.Ten,
                            Ma = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.Ma : s.DuocPham.MaHoatChat,
                            HoatChat = s.DuocPham.HoatChat,
                            DVT = s.DonViTinh.Ten,
                            LoaiDonThuoc = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc,
                            NhomLSKT = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                            SangDisplay = s.DungSang.FloatToStringFraction() + " " + (s.ThoiGianDungSang == null ? "" : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")"),
                            TruaDisplay = s.DungTrua.FloatToStringFraction() + " " + (s.ThoiGianDungTrua == null ? "" : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")"),
                            ChieuDisplay = s.DungChieu.FloatToStringFraction() + " " + (s.ThoiGianDungChieu == null ? "" : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")"),
                            ToiDisplay = s.DungToi.FloatToStringFraction() + " " + (s.ThoiGianDungToi == null ? "" : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")"),
                            SoLuong = s.SoLuong,
                            SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                            SoNgayDung = s.SoNgayDung,
                            TenDuongDung = s.DuongDung.Ten,
                            GhiChu = s.GhiChu
                        });
            var lstQuery = query.ToList();
            var lstBHYTSTT = lstQuery.Where((p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)).Select(p => p).ToList();
            var lstKhongBHYTSTT = lstQuery.Where((p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT)).Select(p => p).ToList();
            for (int i = 0; i < lstBHYTSTT.Count(); i++)
            {
                lstBHYTSTT[i].STT = i + 1;
            }
            for (int i = 0; i < lstKhongBHYTSTT.Count(); i++)
            {
                lstKhongBHYTSTT[i].STT = i + 1;
            }
            query = lstQuery.OrderBy(p => p.STT).AsQueryable();
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        private async Task<ApDungToaThuocChiTietVo> ApDungDonThuocChiTiet(YeuCauKhamBenhDonThuocChiTiet donThuocChiTiet, LoaiKhoThuoc loaiKhoThuoc, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh ycKhamBenh, YeuCauKhamBenhDonThuoc donThuoc, DonThuocThanhToan donThuocThanhToan)
        {

            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var duocPham = await _duocPhamRepository.GetByIdAsync(donThuocChiTiet.DuocPhamId,
                        x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams)
                            .Include(o => o.HopDongThauDuocPhamChiTiets)
                            .Include(o => o.DonViTinh)
                            .Include(o => o.DuongDung)
                            .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

            ApDungToaThuocChiTietVo apDungToaThuocChiTietVo = new ApDungToaThuocChiTietVo
            {
                DuocPhamId = donThuocChiTiet.DuocPhamId,
                Ten = duocPham.Ten,
                HoatChat = duocPham.HoatChat,
                DVT = duocPham.DonViTinh.Ten,
                DuongDung = duocPham.DuongDung.Ten,
                SoLuong = donThuocChiTiet.SoLuong,
                SoLuongDisplay = ((double?)donThuocChiTiet.SoLuong).FloatToStringFraction(),
                SoLuongTonKho = 0,
                SoNgayDung = donThuocChiTiet.SoNgayDung,
                DungSang = donThuocChiTiet.DungSang,
                DungTrua = donThuocChiTiet.DungTrua,
                DungChieu = donThuocChiTiet.DungChieu,
                DungToi = donThuocChiTiet.DungToi,
                ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                GhiChu = donThuocChiTiet.GhiChu,
                LoaiKhoThuoc = loaiKhoThuoc,
                Nhom = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                SangDisplay = donThuocChiTiet.DungSang.FloatToStringFraction(),
                TruaDisplay = donThuocChiTiet.DungTrua.FloatToStringFraction(),
                ChieuDisplay = donThuocChiTiet.DungChieu.FloatToStringFraction(),
                ToiDisplay = donThuocChiTiet.DungToi.FloatToStringFraction(),
                ThoiGianDungSangDisplay = donThuocChiTiet.ThoiGianDungSang == null ? "" : "(" + donThuocChiTiet.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                ThoiGianDungTruaDisplay = donThuocChiTiet.ThoiGianDungTrua == null ? "" : "(" + donThuocChiTiet.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                ThoiGianDungChieuDisplay = donThuocChiTiet.ThoiGianDungChieu == null ? "" : "(" + donThuocChiTiet.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                ThoiGianDungToiDisplay = donThuocChiTiet.ThoiGianDungToi == null ? "" : "(" + donThuocChiTiet.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
            };

            var ycDonThuocChiTiet = new YeuCauKhamBenhDonThuocChiTiet
            {
                DuocPhamId = duocPham.Id,
                LaDuocPhamBenhVien = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT || loaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien,
                Ten = duocPham.Ten,
                TenTiengAnh = duocPham.TenTiengAnh,
                SoDangKy = duocPham.SoDangKy,
                StthoatChat = duocPham.STTHoatChat,
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
                SoLuong = donThuocChiTiet.SoLuong,
                SoNgayDung = donThuocChiTiet.SoNgayDung,
                DungSang = donThuocChiTiet.DungSang,
                DungTrua = donThuocChiTiet.DungTrua,
                DungChieu = donThuocChiTiet.DungChieu,
                DungToi = donThuocChiTiet.DungToi,
                ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                DuocHuongBaoHiem = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                BenhNhanMuaNgoai = loaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien,
                GhiChu = donThuocChiTiet.GhiChu
            };
            donThuoc.YeuCauKhamBenhDonThuocChiTiets.Add(ycDonThuocChiTiet);

            if (loaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien)
            {
                apDungToaThuocChiTietVo.SoLuongTonKho = Int32.MaxValue;
                return apDungToaThuocChiTietVo;
            }
            if (duocPham.DuocPhamBenhVien == null)
            {
                return apDungToaThuocChiTietVo;
            }
            double soLuongTonKho = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                .Where(o => (o.LaDuocPhamBHYT == (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT) && (o.NhapKhoDuocPhams.KhoDuocPhams.Id == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoDuocPhams.Id == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT)) && o.HanSuDung >= DateTime.Now && o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            apDungToaThuocChiTietVo.SoLuongTonKho = soLuongTonKho;
            if (soLuongTonKho < donThuocChiTiet.SoLuong)
            {
                return apDungToaThuocChiTietVo;
            }
            double soLuongCanXuat = donThuocChiTiet.SoLuong;
            while (!soLuongCanXuat.Equals(0))
            {
                // tinh so luong xuat
                var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o =>
                    (o.LaDuocPhamBHYT == (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT) && (o.NhapKhoDuocPhams.KhoDuocPhams.Id == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoDuocPhams.Id == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT)) && o.HanSuDung >= DateTime.Now &&
                    o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;

                nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
                var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
                {
                    SoLuongXuat = soLuongXuat,
                    NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                    XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                    {
                        DuocPhamBenhVien = duocPham.DuocPhamBenhVien,
                    }
                };
                var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId).Gia;
                var donGiaBaoHiem = nhapKhoDuocPhamChiTiet.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : nhapKhoDuocPhamChiTiet.DonGiaNhap;
                var dtttChiTiet = new DonThuocThanhToanChiTiet
                {
                    DuocPhamId = duocPham.Id,
                    YeuCauKhamBenhDonThuocChiTiet = ycDonThuocChiTiet,
                    XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
                    Ten = duocPham.Ten,
                    TenTiengAnh = duocPham.TenTiengAnh,
                    SoDangKy = duocPham.SoDangKy,
                    STTHoatChat = duocPham.STTHoatChat,
                    NhomChiPhi = Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT,
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
                    HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                    NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                    SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                    SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                    LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                    LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                    NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                    GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                    NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                    DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                    TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                    PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                    VAT = nhapKhoDuocPhamChiTiet.VAT,
                    SoLuong = soLuongXuat,
                    SoTienBenhNhanDaChi = 0,
                    DuocHuongBaoHiem = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                    DonGiaBaoHiem = donGiaBaoHiem,
                    TiLeBaoHiemThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan ?? 100
                };

                donThuocThanhToan.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                soLuongCanXuat = soLuongCanXuat - soLuongXuat;
            }
            return apDungToaThuocChiTietVo;
        }

        private async Task<bool> ApDungDonThuocChiTiet(ApDungToaThuocChiTietVo apDungToaThuocChiTietVo, LoaiKhoThuoc loaiKhoThuoc, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh ycKhamBenh, YeuCauKhamBenhDonThuoc donThuoc, DonThuocThanhToan donThuocThanhToan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            var duocPham = await _duocPhamRepository.GetByIdAsync(apDungToaThuocChiTietVo.DuocPhamId,
                        x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams)
                            .Include(o => o.HopDongThauDuocPhamChiTiets)
                            .Include(o => o.DonViTinh)
                            .Include(o => o.DuongDung)
                            .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

            var ycDonThuocChiTiet = new YeuCauKhamBenhDonThuocChiTiet
            {
                DuocPhamId = duocPham.Id,
                LaDuocPhamBenhVien = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT || loaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien,
                Ten = duocPham.Ten,
                TenTiengAnh = duocPham.TenTiengAnh,
                SoDangKy = duocPham.SoDangKy,
                StthoatChat = duocPham.STTHoatChat,
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
                SoLuong = apDungToaThuocChiTietVo.SoLuong,
                SoNgayDung = apDungToaThuocChiTietVo.SoNgayDung,
                DungSang = apDungToaThuocChiTietVo.DungSang,
                DungTrua = apDungToaThuocChiTietVo.DungTrua,
                DungChieu = apDungToaThuocChiTietVo.DungChieu,
                DungToi = apDungToaThuocChiTietVo.DungToi,
                ThoiGianDungSang = apDungToaThuocChiTietVo.ThoiGianDungSang,
                ThoiGianDungTrua = apDungToaThuocChiTietVo.ThoiGianDungTrua,
                ThoiGianDungChieu = apDungToaThuocChiTietVo.ThoiGianDungChieu,
                ThoiGianDungToi = apDungToaThuocChiTietVo.ThoiGianDungToi,
                DuocHuongBaoHiem = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                BenhNhanMuaNgoai = loaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien,
                GhiChu = apDungToaThuocChiTietVo.GhiChu
            };
            donThuoc.YeuCauKhamBenhDonThuocChiTiets.Add(ycDonThuocChiTiet);

            if (loaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien)
            {
                return true;
            }
            if (duocPham.DuocPhamBenhVien == null)
            {
                return false;
            }
            double soLuongTonKho = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                .Where(o => (o.LaDuocPhamBHYT == (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT) && (o.NhapKhoDuocPhams.KhoDuocPhams.Id == (int)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoDuocPhams.Id == (int)Enums.EnumKhoDuocPham.KhoThuocBHYT)) && o.HanSuDung >= DateTime.Now &&
                            o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            apDungToaThuocChiTietVo.SoLuongTonKho = soLuongTonKho;
            if (soLuongTonKho < apDungToaThuocChiTietVo.SoLuong)
            {
                return false;
            }
            double soLuongCanXuat = apDungToaThuocChiTietVo.SoLuong;
            while (!soLuongCanXuat.Equals(0))
            {
                // tinh so luong xuat
                var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o =>
                    (o.LaDuocPhamBHYT == (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT) && (o.NhapKhoDuocPhams.KhoDuocPhams.Id == (int)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoDuocPhams.Id == (int)Enums.EnumKhoDuocPham.KhoThuocBHYT)) && o.HanSuDung >= DateTime.Now &&
                    o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;

                nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
                var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
                {
                    SoLuongXuat = soLuongXuat,
                    NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                    XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                    {
                        DuocPhamBenhVien = duocPham.DuocPhamBenhVien
                    }
                };
                var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId).Gia;
                var donGiaBaoHiem = nhapKhoDuocPhamChiTiet.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : nhapKhoDuocPhamChiTiet.DonGiaNhap;
                var dtttChiTiet = new DonThuocThanhToanChiTiet
                {
                    DuocPhamId = duocPham.Id,
                    YeuCauKhamBenhDonThuocChiTiet = ycDonThuocChiTiet,
                    XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
                    Ten = duocPham.Ten,
                    TenTiengAnh = duocPham.TenTiengAnh,
                    SoDangKy = duocPham.SoDangKy,
                    STTHoatChat = duocPham.STTHoatChat,
                    NhomChiPhi = Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT,
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
                    HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                    NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                    SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                    SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                    LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                    LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                    NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                    GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                    NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                    DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                    TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                    PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                    VAT = nhapKhoDuocPhamChiTiet.VAT,
                    SoLuong = soLuongXuat,
                    SoTienBenhNhanDaChi = 0,
                    DuocHuongBaoHiem = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                    DonGiaBaoHiem = donGiaBaoHiem,
                    TiLeBaoHiemThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan
                };

                donThuocThanhToan.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                soLuongCanXuat = soLuongCanXuat - soLuongXuat;
            }
            return true;
        }
        private async Task<ApDungToaThuocChiTietVo> ApDungToaThuocMauChiTiet(ToaThuocMauChiTiet toaThuocMauChiTiet, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh ycKhamBenh, YeuCauKhamBenhDonThuoc donThuocBHYT, DonThuocThanhToan donThuocThanhToanBHYT, YeuCauKhamBenhDonThuoc donThuoc, DonThuocThanhToan donThuocThanhToan, bool coBHYT)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var duocPham = await _duocPhamRepository.GetByIdAsync(toaThuocMauChiTiet.DuocPhamId,
                        x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams)
                            .Include(o => o.HopDongThauDuocPhamChiTiets)
                            .Include(o => o.DonViTinh)
                            .Include(o => o.DuongDung)
                            .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
            bool checkBHYT = false;
            var loaiKhoThuoc = LoaiKhoThuoc.ThuocNgoaiBenhVien;
            if (duocPham.DuocPhamBenhVien != null)
            {
                checkBHYT = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(o => o.LaDuocPhamBHYT && (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT) && o.HanSuDung >= DateTime.Now && o.SoLuongDaXuat < o.SoLuongNhap);
                loaiKhoThuoc = LoaiKhoThuoc.ThuocBenhVien;
            }
            if (coBHYT && checkBHYT)
            {
                loaiKhoThuoc = LoaiKhoThuoc.ThuocBHYT;
            }

            ApDungToaThuocChiTietVo apDungToaThuocChiTietVo = new ApDungToaThuocChiTietVo
            {
                DuocPhamId = toaThuocMauChiTiet.DuocPhamId,
                Ten = duocPham.Ten,
                HoatChat = duocPham.HoatChat,
                DVT = duocPham.DonViTinh.Ten,
                DuongDung = duocPham.DuongDung.Ten,
                SoLuong = toaThuocMauChiTiet.SoLuong,
                SoLuongDisplay = ((double?)toaThuocMauChiTiet.SoLuong).FloatToStringFraction(),
                SoLuongTonKho = 0,
                SoNgayDung = toaThuocMauChiTiet.SoNgayDung,
                DungSang = toaThuocMauChiTiet.DungSang,
                DungTrua = toaThuocMauChiTiet.DungTrua,
                DungChieu = toaThuocMauChiTiet.DungChieu,
                DungToi = toaThuocMauChiTiet.DungToi,
                LoaiKhoThuoc = loaiKhoThuoc,
                Nhom = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                SangDisplay = toaThuocMauChiTiet.DungSang.FloatToStringFraction(),
                TruaDisplay = toaThuocMauChiTiet.DungTrua.FloatToStringFraction(),
                ChieuDisplay = toaThuocMauChiTiet.DungChieu.FloatToStringFraction(),
                ToiDisplay = toaThuocMauChiTiet.DungToi.FloatToStringFraction(),
                GhiChu = toaThuocMauChiTiet.GhiChu,
                ThoiGianDungSangDisplay = toaThuocMauChiTiet.ThoiGianDungSang == null ? "" : "(" + toaThuocMauChiTiet.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                ThoiGianDungTruaDisplay = toaThuocMauChiTiet.ThoiGianDungTrua == null ? "" : "(" + toaThuocMauChiTiet.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                ThoiGianDungChieuDisplay = toaThuocMauChiTiet.ThoiGianDungChieu == null ? "" : "(" + toaThuocMauChiTiet.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                ThoiGianDungToiDisplay = toaThuocMauChiTiet.ThoiGianDungToi == null ? "" : "(" + toaThuocMauChiTiet.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
            };

            var ycDonThuocChiTiet = new YeuCauKhamBenhDonThuocChiTiet
            {
                DuocPhamId = duocPham.Id,
                LaDuocPhamBenhVien = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT || loaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien,
                Ten = duocPham.Ten,
                TenTiengAnh = duocPham.TenTiengAnh,
                SoDangKy = duocPham.SoDangKy,
                StthoatChat = duocPham.STTHoatChat,
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
                SoLuong = toaThuocMauChiTiet.SoLuong,
                SoNgayDung = toaThuocMauChiTiet.SoNgayDung,
                DungSang = toaThuocMauChiTiet.DungSang,
                DungTrua = toaThuocMauChiTiet.DungTrua,
                DungChieu = toaThuocMauChiTiet.DungChieu,
                DungToi = toaThuocMauChiTiet.DungToi,
                GhiChu = toaThuocMauChiTiet.GhiChu,
                ThoiGianDungSang = toaThuocMauChiTiet.ThoiGianDungSang,
                ThoiGianDungTrua = toaThuocMauChiTiet.ThoiGianDungTrua,
                ThoiGianDungChieu = toaThuocMauChiTiet.ThoiGianDungChieu,
                ThoiGianDungToi = toaThuocMauChiTiet.ThoiGianDungToi,
                DuocHuongBaoHiem = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                BenhNhanMuaNgoai = loaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien
            };
            if (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT)
                donThuocBHYT.YeuCauKhamBenhDonThuocChiTiets.Add(ycDonThuocChiTiet);
            else
                donThuoc.YeuCauKhamBenhDonThuocChiTiets.Add(ycDonThuocChiTiet);

            if (loaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien)
            {
                apDungToaThuocChiTietVo.SoLuongTonKho = Int32.MaxValue;
                return apDungToaThuocChiTietVo;
            }
            if (duocPham.DuocPhamBenhVien == null)
            {
                return apDungToaThuocChiTietVo;
            }
            double soLuongTonKho = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                .Where(o => (o.LaDuocPhamBHYT == (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT) && (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT)) && o.HanSuDung >= DateTime.Now &&
                            o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            apDungToaThuocChiTietVo.SoLuongTonKho = soLuongTonKho;
            if (soLuongTonKho < toaThuocMauChiTiet.SoLuong)
            {
                return apDungToaThuocChiTietVo;
            }
            double soLuongCanXuat = toaThuocMauChiTiet.SoLuong;
            while (!soLuongCanXuat.Equals(0))
            {
                // tinh so luong xuat
                var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o =>
                    (o.LaDuocPhamBHYT == (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT) && (o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)Enums.EnumKhoDuocPham.KhoThuocBHYT)) && o.HanSuDung >= DateTime.Now &&
                    o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;

                nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
                var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
                {
                    SoLuongXuat = soLuongXuat,
                    NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                    XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                    {
                        DuocPhamBenhVien = duocPham.DuocPhamBenhVien
                    }
                };
                var giaTheoHopDong = duocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId).Gia;
                var donGiaBaoHiem = nhapKhoDuocPhamChiTiet.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : nhapKhoDuocPhamChiTiet.DonGiaNhap;
                var dtttChiTiet = new DonThuocThanhToanChiTiet
                {
                    DuocPhamId = duocPham.Id,
                    YeuCauKhamBenhDonThuocChiTiet = ycDonThuocChiTiet,
                    XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
                    Ten = duocPham.Ten,
                    TenTiengAnh = duocPham.TenTiengAnh,
                    SoDangKy = duocPham.SoDangKy,
                    STTHoatChat = duocPham.STTHoatChat,
                    NhomChiPhi = Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT,
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
                    HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                    NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                    SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                    SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                    LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                    LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                    NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                    GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                    NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                    DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                    TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                    PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                    VAT = nhapKhoDuocPhamChiTiet.VAT,
                    SoLuong = soLuongXuat,
                    SoTienBenhNhanDaChi = 0,
                    DuocHuongBaoHiem = loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                    DonGiaBaoHiem = donGiaBaoHiem,
                    TiLeBaoHiemThanhToan = nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan ?? 100,
                };
                if (loaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT)
                    donThuocThanhToanBHYT.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                else
                    donThuocThanhToan.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                soLuongCanXuat = soLuongCanXuat - soLuongXuat;
            }
            return apDungToaThuocChiTietVo;
        }

        public async Task<string> ApDungToaThuocLichSuKhamBenhConfirmAsync(ApDungToaThuocLichSuKhamBenhConfirmVo apDungToaThuocLichSuKhamBenhConfirmVo)
        {
            var ycKhamBenhHienTai = await BaseRepository.GetByIdAsync(
                apDungToaThuocLichSuKhamBenhConfirmVo.YeuCauKhamBenhHienTaiId,
                x => x.Include(o => o.YeuCauKhamBenhDonThuocs).Include(o => o.DonThuocThanhToans)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuKyThuats)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuGiuongBenhViens)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDuocPhamBenhViens)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauVatTuBenhViens)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets));

            var ycDonThuocBHYTHienTai = ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            var ycDonThuocKhongBHYTHienTai = ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT);

            var donThuocTTBHYTHienTai = ycKhamBenhHienTai.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            var donThuocTTKhongBHYTHienTai = ycKhamBenhHienTai.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT);
            var thuocApDungs = apDungToaThuocLichSuKhamBenhConfirmVo.ApDungToaThuocChiTietVos.Where(o => o.SoLuong <= o.SoLuongTonKho).ToList();

            if (thuocApDungs.Any(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT))
            {
                if (ycKhamBenhHienTai.YeuCauTiepNhan.CoBHYT == true)
                {
                    if (ycDonThuocBHYTHienTai == null)
                    {
                        ycDonThuocBHYTHienTai = new YeuCauKhamBenhDonThuoc
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocBHYT,
                            ThoiDiemKeDon = DateTime.Now,
                            BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                            NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                        }; ;

                    }
                    if (donThuocTTBHYTHienTai == null)
                    {
                        donThuocTTBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocBHYTHienTai.DonThuocThanhToans.Add(donThuocTTBHYTHienTai);

                    }
                    foreach (var thuocApDung in thuocApDungs.Where(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT))
                    {
                        if (!await ApDungDonThuocChiTiet(thuocApDung, LoaiKhoThuoc.ThuocBHYT, ycKhamBenhHienTai, ycDonThuocBHYTHienTai, donThuocTTBHYTHienTai))
                        {
                            return GetResourceValueByResourceName("DonThuoc.Amount");
                        }
                    }
                }
                else
                {
                    if (ycDonThuocKhongBHYTHienTai == null)
                    {
                        ycDonThuocKhongBHYTHienTai = new YeuCauKhamBenhDonThuoc
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            ThoiDiemKeDon = DateTime.Now,
                            BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                            NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                        };

                    }
                    if (donThuocTTKhongBHYTHienTai == null)
                    {
                        donThuocTTKhongBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocKhongBHYTHienTai.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);

                    }
                    foreach (var thuocApDung in thuocApDungs.Where(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT))
                    {
                        if (!await ApDungDonThuocChiTiet(thuocApDung, LoaiKhoThuoc.ThuocBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai))
                        {
                            return GetResourceValueByResourceName("DonThuoc.Amount");
                        }
                    }
                }
            }
            if (thuocApDungs.Any(o => o.LoaiKhoThuoc != LoaiKhoThuoc.ThuocBHYT))
            {
                if (ycDonThuocKhongBHYTHienTai == null)
                {
                    ycDonThuocKhongBHYTHienTai = new YeuCauKhamBenhDonThuoc
                    {
                        LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                        ThoiDiemKeDon = DateTime.Now,
                        BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                        NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                    };
                }
                if (thuocApDungs.Any(o => o.LoaiKhoThuoc != LoaiKhoThuoc.ThuocBenhVien))
                {
                    if (donThuocTTKhongBHYTHienTai == null)
                    {
                        donThuocTTKhongBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocKhongBHYTHienTai.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);

                    }
                }
                foreach (var thuocApDung in thuocApDungs.Where(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien))
                {
                    if (!await ApDungDonThuocChiTiet(thuocApDung, LoaiKhoThuoc.ThuocBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai))
                    {
                        return GetResourceValueByResourceName("DonThuoc.Amount");
                    }
                }
                foreach (var thuocApDung in thuocApDungs.Where(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien))
                {
                    if (!await ApDungDonThuocChiTiet(thuocApDung, LoaiKhoThuoc.ThuocNgoaiBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai))
                    {
                        return GetResourceValueByResourceName("DonThuoc.Amount");
                    }
                }
            }

            ycKhamBenhHienTai.CoKeToa = true;
            if (ycDonThuocBHYTHienTai.CreatedOn == null && ycDonThuocBHYTHienTai.YeuCauKhamBenhDonThuocChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocBHYTHienTai);
            }
            if (ycDonThuocKhongBHYTHienTai.CreatedOn == null && ycDonThuocKhongBHYTHienTai.YeuCauKhamBenhDonThuocChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocKhongBHYTHienTai);
            }
            if (donThuocTTBHYTHienTai.CreatedOn == null && donThuocTTBHYTHienTai.DonThuocThanhToanChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauTiepNhan.DonThuocThanhToans.Add(donThuocTTBHYTHienTai);
            }
            if (donThuocTTKhongBHYTHienTai.CreatedOn == null && donThuocTTKhongBHYTHienTai.DonThuocThanhToanChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauTiepNhan.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);
            }
            //bo duyet tu dong
            //if (ycKhamBenhHienTai.YeuCauTiepNhan.CoBHYT == true)
            //{
            //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
            //    if (cauHinh.DuyetBHYTTuDong)
            //    {
            //        DuyetBHYTChoDonThuoc(ycKhamBenhHienTai.YeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong);
            //    }
            //}

            await BaseRepository.UpdateAsync(ycKhamBenhHienTai);
            return null;
        }

        public async Task<KetQuaApDungToaThuocVo> ApDungToaThuocLichSuKhamBenhAsync(ApDungToaThuocLichSuKhamBenhVo apDungToaThuocLichSuKhamBenhVo)
        {
            var ycKhamBenhTruoc = await BaseRepository.GetByIdAsync(
                apDungToaThuocLichSuKhamBenhVo.YeuCauKhamBenhTruocId,
                x => x.Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).Include(o => o.YeuCauTiepNhan));

            var ycKhamBenhHienTai = await BaseRepository.GetByIdAsync(
                apDungToaThuocLichSuKhamBenhVo.YeuCauKhamBenhHienTaiId,
                x => x.Include(o => o.YeuCauKhamBenhDonThuocs)
                .Include(o => o.DonThuocThanhToans)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauKhamBenhs)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuKyThuats)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuGiuongBenhViens)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDuocPhamBenhViens)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauVatTuBenhViens)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets));

            List<ApDungToaThuocChiTietVo> apDungToaThuocChiTietVos = new List<ApDungToaThuocChiTietVo>();

            var ycDonThuocBHYTTruoc = ycKhamBenhTruoc.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            var ycDonThuocKhongBHYTTruoc = ycKhamBenhTruoc.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT);

            var ycDonThuocBHYTHienTai = ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            var ycDonThuocKhongBHYTHienTai = ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT);

            var donThuocTTBHYTHienTai = ycKhamBenhHienTai.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            var donThuocTTKhongBHYTHienTai = ycKhamBenhHienTai.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT);


            if (ycDonThuocBHYTTruoc != null && ycDonThuocBHYTTruoc.YeuCauKhamBenhDonThuocChiTiets.Any())
            {
                if (ycKhamBenhHienTai.YeuCauTiepNhan.CoBHYT == true)
                {
                    if (ycDonThuocBHYTHienTai == null)
                    {
                        ycDonThuocBHYTHienTai = new YeuCauKhamBenhDonThuoc
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocBHYT,
                            ThoiDiemKeDon = DateTime.Now,
                            BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                            NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                        };

                    }
                    if (donThuocTTBHYTHienTai == null)
                    {
                        donThuocTTBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocBHYTHienTai.DonThuocThanhToans.Add(donThuocTTBHYTHienTai);

                    }
                    foreach (var yeuCauKhamBenhDonThuocChiTiet in ycDonThuocBHYTTruoc.YeuCauKhamBenhDonThuocChiTiets)
                    {
                        apDungToaThuocChiTietVos.Add(await ApDungDonThuocChiTiet(yeuCauKhamBenhDonThuocChiTiet,
                            LoaiKhoThuoc.ThuocBHYT, ycKhamBenhHienTai, ycDonThuocBHYTHienTai, donThuocTTBHYTHienTai));
                    }
                }
                else
                {
                    if (ycDonThuocKhongBHYTHienTai == null)
                    {
                        ycDonThuocKhongBHYTHienTai = new YeuCauKhamBenhDonThuoc
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            ThoiDiemKeDon = DateTime.Now,
                            BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                            NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                        };
                    }
                    if (donThuocTTKhongBHYTHienTai == null)
                    {
                        donThuocTTKhongBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocKhongBHYTHienTai.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);

                    }
                    foreach (var yeuCauKhamBenhDonThuocChiTiet in ycDonThuocBHYTTruoc.YeuCauKhamBenhDonThuocChiTiets)
                    {
                        apDungToaThuocChiTietVos.Add(await ApDungDonThuocChiTiet(yeuCauKhamBenhDonThuocChiTiet,
                            LoaiKhoThuoc.ThuocBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai));
                    }
                }
            }
            if (ycDonThuocKhongBHYTTruoc != null && ycDonThuocKhongBHYTTruoc.YeuCauKhamBenhDonThuocChiTiets.Any())
            {
                if (ycDonThuocKhongBHYTHienTai == null)
                {
                    ycDonThuocKhongBHYTHienTai = new YeuCauKhamBenhDonThuoc
                    {
                        LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                        ThoiDiemKeDon = DateTime.Now,
                        BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                        NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                    };
                }
                if (ycDonThuocKhongBHYTTruoc.YeuCauKhamBenhDonThuocChiTiets.Any(o => o.LaDuocPhamBenhVien))
                {
                    if (donThuocTTKhongBHYTHienTai == null)
                    {
                        donThuocTTKhongBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocKhongBHYTHienTai.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);

                    }
                }
                foreach (var yeuCauKhamBenhDonThuocChiTiet in ycDonThuocKhongBHYTTruoc.YeuCauKhamBenhDonThuocChiTiets.Where(o => o.LaDuocPhamBenhVien))
                {
                    apDungToaThuocChiTietVos.Add(await ApDungDonThuocChiTiet(yeuCauKhamBenhDonThuocChiTiet,
                        LoaiKhoThuoc.ThuocBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai));
                }
                foreach (var yeuCauKhamBenhDonThuocChiTiet in ycDonThuocKhongBHYTTruoc.YeuCauKhamBenhDonThuocChiTiets.Where(o => !o.LaDuocPhamBenhVien))
                {
                    apDungToaThuocChiTietVos.Add(await ApDungDonThuocChiTiet(yeuCauKhamBenhDonThuocChiTiet,
                        LoaiKhoThuoc.ThuocNgoaiBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai));
                }
            }
            if (apDungToaThuocChiTietVos.Any(o => o.SoLuongTonKho < o.SoLuong))
            {
                return new KetQuaApDungToaThuocVo
                {
                    ThanhCong = false,
                    Error = "Số lượng thuốc trong kho không đủ",
                    ApDungToaThuocChiTietVos = apDungToaThuocChiTietVos
                };
            }
            ycKhamBenhHienTai.CoKeToa = true;

            if (ycDonThuocBHYTHienTai.CreatedOn == null && ycDonThuocBHYTHienTai.YeuCauKhamBenhDonThuocChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocBHYTHienTai);
            }
            if (ycDonThuocKhongBHYTHienTai.CreatedOn == null && ycDonThuocKhongBHYTHienTai.YeuCauKhamBenhDonThuocChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocKhongBHYTHienTai);
            }
            if (donThuocTTBHYTHienTai.CreatedOn == null && donThuocTTBHYTHienTai.DonThuocThanhToanChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauTiepNhan.DonThuocThanhToans.Add(donThuocTTBHYTHienTai);
            }
            if (donThuocTTKhongBHYTHienTai.CreatedOn == null && donThuocTTKhongBHYTHienTai.DonThuocThanhToanChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauTiepNhan.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);
            }
            //bo duyet tu dong
            //if (ycKhamBenhHienTai.YeuCauTiepNhan.CoBHYT == true)
            //{
            //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
            //    if (cauHinh.DuyetBHYTTuDong)
            //    {
            //        DuyetBHYTChoDonThuoc(ycKhamBenhHienTai.YeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong);
            //    }
            //}

            await BaseRepository.UpdateAsync(ycKhamBenhHienTai);
            return new KetQuaApDungToaThuocVo
            {
                ThanhCong = true,
            };
        }

        public async Task<KetQuaApDungToaThuocVo> ApDungToaMauAsync(ApDungToaThuocMauVo toaMauVo)
        {
            var toaMau = await _toaThuocMauRepository.GetByIdAsync(toaMauVo.ToaMauId, x => x.Include(o => o.ToaThuocMauChiTiets));
            var ycKhamBenhHienTai = await BaseRepository.GetByIdAsync(toaMauVo.YeuCauKhamBenhHienTaiId,
                x => x.Include(o => o.YeuCauKhamBenhDonThuocs)
                .Include(o => o.DonThuocThanhToans)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauKhamBenhs)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuKyThuats)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuGiuongBenhViens)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDuocPhamBenhViens)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauVatTuBenhViens)
                .Include(o => o.YeuCauTiepNhan).ThenInclude(dt => dt.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets));

            List<ApDungToaThuocChiTietVo> apDungToaThuocChiTietVos = new List<ApDungToaThuocChiTietVo>();

            var ycDonThuocBHYTHienTai = ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            var ycDonThuocKhongBHYTHienTai = ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT);

            var donThuocTTBHYTHienTai = ycKhamBenhHienTai.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan);
            var donThuocTTKhongBHYTHienTai = ycKhamBenhHienTai.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan);

            if (ycDonThuocBHYTHienTai == null)
            {
                ycDonThuocBHYTHienTai = new YeuCauKhamBenhDonThuoc
                {
                    LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocBHYT,
                    ThoiDiemKeDon = DateTime.Now,
                    BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                    NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
            }
            if (donThuocTTBHYTHienTai == null)
            {
                donThuocTTBHYTHienTai = new DonThuocThanhToan
                {
                    LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocBHYT,
                    YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                    YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                    BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                    TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                };
                ycDonThuocBHYTHienTai.DonThuocThanhToans.Add(donThuocTTBHYTHienTai);
            }

            if (ycDonThuocKhongBHYTHienTai == null)
            {
                ycDonThuocKhongBHYTHienTai = new YeuCauKhamBenhDonThuoc
                {
                    LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                    ThoiDiemKeDon = DateTime.Now,
                    BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                    NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
            }
            if (donThuocTTKhongBHYTHienTai == null)
            {
                donThuocTTKhongBHYTHienTai = new DonThuocThanhToan
                {
                    LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                    YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                    YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                    BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                    TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                };
                ycDonThuocKhongBHYTHienTai.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);

            }
            foreach (var toaThuocMauChiTiet in toaMau.ToaThuocMauChiTiets)
            {
                apDungToaThuocChiTietVos.Add(await ApDungToaThuocMauChiTiet(toaThuocMauChiTiet, ycKhamBenhHienTai,
                    ycDonThuocBHYTHienTai, donThuocTTBHYTHienTai, ycDonThuocKhongBHYTHienTai,
                    donThuocTTKhongBHYTHienTai, ycKhamBenhHienTai.YeuCauTiepNhan.CoBHYT == true));

                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.Thuoc,
                    GhiChu = toaThuocMauChiTiet.GhiChu
                };
                await ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }

            if (apDungToaThuocChiTietVos.Any(o => o.SoLuongTonKho < o.SoLuong))
            {
                return new KetQuaApDungToaThuocVo
                {
                    ThanhCong = false,
                    Error = "Số lượng thuốc trong kho không đủ",
                    ApDungToaThuocChiTietVos = apDungToaThuocChiTietVos
                };
            }
            ycKhamBenhHienTai.CoKeToa = true;
            if (ycDonThuocBHYTHienTai.CreatedOn == null && ycDonThuocBHYTHienTai.YeuCauKhamBenhDonThuocChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocBHYTHienTai);
            }
            if (ycDonThuocKhongBHYTHienTai.CreatedOn == null && ycDonThuocKhongBHYTHienTai.YeuCauKhamBenhDonThuocChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocKhongBHYTHienTai);
            }
            if (donThuocTTBHYTHienTai.CreatedOn == null && donThuocTTBHYTHienTai.DonThuocThanhToanChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauTiepNhan.DonThuocThanhToans.Add(donThuocTTBHYTHienTai);
            }
            if (donThuocTTKhongBHYTHienTai.CreatedOn == null && donThuocTTKhongBHYTHienTai.DonThuocThanhToanChiTiets.Any())
            {
                ycKhamBenhHienTai.YeuCauTiepNhan.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);
            }
            //bo duyet tu dong
            //if (ycKhamBenhHienTai.YeuCauTiepNhan.CoBHYT == true)
            //{
            //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
            //    if (cauHinh.DuyetBHYTTuDong)
            //    {
            //        DuyetBHYTChoDonThuoc(ycKhamBenhHienTai.YeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong);
            //    }
            //}
            await BaseRepository.UpdateAsync(ycKhamBenhHienTai);
            return new KetQuaApDungToaThuocVo
            {
                ThanhCong = true,
            };
        }

        public async Task<string> ApDungToaThuocConfirmAsync(ApDungToaThuocConfirmVo apDungToaThuocConfirmVo)
        {
            var ycKhamBenhHienTai = await BaseRepository.GetByIdAsync(
                apDungToaThuocConfirmVo.YeuCauKhamBenhHienTaiId,
                x => x.Include(o => o.YeuCauKhamBenhDonThuocs).Include(o => o.DonThuocThanhToans).Include(o => o.YeuCauTiepNhan));

            var ycDonThuocBHYTHienTai = ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            var ycDonThuocKhongBHYTHienTai = ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT);

            var donThuocTTBHYTHienTai = ycKhamBenhHienTai.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            var donThuocTTKhongBHYTHienTai = ycKhamBenhHienTai.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT);
            var thuocApDungs = apDungToaThuocConfirmVo.ApDungToaThuocChiTietVos.Where(o => o.SoLuong <= o.SoLuongTonKho).ToList();

            if (thuocApDungs.Any(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT))
            {
                if (ycKhamBenhHienTai.YeuCauTiepNhan.CoBHYT == true)
                {
                    if (ycDonThuocBHYTHienTai == null)
                    {
                        ycDonThuocBHYTHienTai = new YeuCauKhamBenhDonThuoc
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocBHYT,
                            ThoiDiemKeDon = DateTime.Now,
                            BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                            NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                        };
                        ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocBHYTHienTai);

                    }
                    if (donThuocTTBHYTHienTai == null)
                    {
                        donThuocTTBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocBHYTHienTai.DonThuocThanhToans.Add(donThuocTTBHYTHienTai);

                    }
                    foreach (var thuocApDung in thuocApDungs.Where(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT))
                    {
                        if (!await ApDungDonThuocChiTiet(thuocApDung, LoaiKhoThuoc.ThuocBHYT, ycKhamBenhHienTai, ycDonThuocBHYTHienTai, donThuocTTBHYTHienTai))
                        {
                            //return "Số lượng thuốc trong kho không đủ";
                            return GetResourceValueByResourceName("DonThuoc.Amount");
                        }
                    }
                }
                else
                {
                    if (ycDonThuocKhongBHYTHienTai == null)
                    {
                        ycDonThuocKhongBHYTHienTai = new YeuCauKhamBenhDonThuoc
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            ThoiDiemKeDon = DateTime.Now,
                            BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                            NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                        };
                        ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocKhongBHYTHienTai);

                    }
                    if (donThuocTTKhongBHYTHienTai == null)
                    {
                        donThuocTTKhongBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocKhongBHYTHienTai.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);

                    }
                    foreach (var thuocApDung in thuocApDungs.Where(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT))
                    {
                        if (!await ApDungDonThuocChiTiet(thuocApDung, LoaiKhoThuoc.ThuocBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai))
                        {
                            return GetResourceValueByResourceName("DonThuoc.Amount");
                        }
                    }
                }
            }
            if (thuocApDungs.Any(o => o.LoaiKhoThuoc != LoaiKhoThuoc.ThuocBHYT))
            {
                if (ycDonThuocKhongBHYTHienTai == null)
                {
                    ycDonThuocKhongBHYTHienTai = new YeuCauKhamBenhDonThuoc
                    {
                        LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                        ThoiDiemKeDon = DateTime.Now,
                        BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                        NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                    };
                    ycKhamBenhHienTai.YeuCauKhamBenhDonThuocs.Add(ycDonThuocKhongBHYTHienTai);

                }
                if (thuocApDungs.Any(o => o.LoaiKhoThuoc != LoaiKhoThuoc.ThuocBenhVien))
                {
                    if (donThuocTTKhongBHYTHienTai == null)
                    {
                        donThuocTTKhongBHYTHienTai = new DonThuocThanhToan
                        {
                            LoaiDonThuoc = Enums.EnumLoaiDonThuoc.ThuocKhongBHYT,
                            YeuCauKhamBenhId = ycKhamBenhHienTai.Id,
                            YeuCauTiepNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.Id,
                            BenhNhanId = ycKhamBenhHienTai.YeuCauTiepNhan.BenhNhanId,
                            TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                        };
                        ycDonThuocKhongBHYTHienTai.DonThuocThanhToans.Add(donThuocTTKhongBHYTHienTai);

                    }
                }
                foreach (var thuocApDung in thuocApDungs.Where(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien))
                {
                    if (!await ApDungDonThuocChiTiet(thuocApDung, LoaiKhoThuoc.ThuocBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai))
                    {
                        return GetResourceValueByResourceName("DonThuoc.Amount");
                    }
                }
                foreach (var thuocApDung in thuocApDungs.Where(o => o.LoaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien))
                {
                    if (!await ApDungDonThuocChiTiet(thuocApDung, LoaiKhoThuoc.ThuocNgoaiBenhVien, ycKhamBenhHienTai, ycDonThuocKhongBHYTHienTai, donThuocTTKhongBHYTHienTai))
                    {
                        return GetResourceValueByResourceName("DonThuoc.Amount");
                    }
                }
            }
            await BaseRepository.UpdateAsync(ycKhamBenhHienTai);
            return null;
        }

        public async Task<KiemTraCoBHYTDuocHuongBaoHiem> KiemTraDeChonLoaiThuoc(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            var yeuCauKhamBenh = await BaseRepository.TableNoTracking
                          .Include(yckb => yckb.YeuCauTiepNhan)
                          .Where(yckb => yckb.Id == yeuCauKhamBenhId && yckb.YeuCauTiepNhanId == yeuCauTiepNhanId)
                          .Select(yckb => yckb)
                          .FirstOrDefaultAsync();

            var yeuCauTiepNhan = await _yeuCauTiepNhanRepository.TableNoTracking
                          .Where(yctn => yctn.Id == yeuCauTiepNhanId)
                          .Select(yctn => yctn)
                          .FirstOrDefaultAsync();

            var result = new KiemTraCoBHYTDuocHuongBaoHiem
            {
                DuocHuongBaoHiem = yeuCauKhamBenh.DuocHuongBaoHiem,
                CoBHYT = yeuCauTiepNhan.CoBHYT
            };
            return result;
        }
        public async Task<GridDataSource> GetChanDoanBacSiKhacDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKhamBenhId = long.Parse(queryInfo.AdditionalSearchString);
            var yeuCauTiepNhanId = BaseRepository.TableNoTracking
                                 .Where(p => p.Id == yeuCauKhamBenhId).Select(p => p.YeuCauTiepNhanId).FirstOrDefault();
            var query = BaseRepository.TableNoTracking
                         .Where(yckb => yckb.YeuCauTiepNhanId == yeuCauTiepNhanId && yckb.Id != yeuCauKhamBenhId && yckb.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                         .Select(s => new ChanDoanBacSiKhacVo
                         {
                             Id = s.Id,
                             DichVuKhamBenhBenhVienId = s.DichVuKhamBenhBenhVienId,
                             TenDichVu = s.MaDichVu + " - " + s.TenDichVu,
                             BacSiKhamBenhId = s.BacSiThucHienId,
                             TenBacSiKham = s.BacSiThucHien.User.HoTen,
                             ICDChinhId = s.IcdchinhId,
                             TenICDChinh = s.Icdchinh.Ma + " - " + s.Icdchinh.TenTiengViet,
                             KetQuaCLS = s.TomTatKetQuaCLS,
                             GhiChuICDChinh = s.GhiChuICDChinh,
                             CachGiaiQuyet = s.CachGiaiQuyet,
                             YeuCauKhamBenhICDKhacs =
                                s.YeuCauKhamBenhICDKhacs.Where(x => x.YeuCauKhamBenhId == s.Id).Select(item => new ICDKhacDetail()
                                {
                                    TenICDKhac = item.ICD.Ma + " - " + item.ICD.TenTiengViet,
                                    Id = item.ICDId,
                                    GhiChu = item.GhiChu
                                }).ToList()
                         });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetChanDoanBacSiKhacTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauKhamBenhId = long.Parse(queryInfo.AdditionalSearchString);
            var yeuCauTiepNhanId = BaseRepository.TableNoTracking
                                  .Where(p => p.Id == yeuCauKhamBenhId).Select(p => p.YeuCauTiepNhanId).FirstOrDefault();
            var query = BaseRepository.TableNoTracking
                         .Where(yckb => yckb.YeuCauTiepNhanId == yeuCauTiepNhanId && yckb.Id != yeuCauKhamBenhId && yckb.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                         .Select(s => new ChanDoanBacSiKhacVo
                         {
                             Id = s.Id,
                             DichVuKhamBenhBenhVienId = s.DichVuKhamBenhBenhVienId,
                             TenDichVu = s.MaDichVu + " - " + s.TenDichVu,
                             BacSiKhamBenhId = s.BacSiThucHienId,
                             TenBacSiKham = s.BacSiThucHien.User.HoTen,
                             ICDChinhId = s.IcdchinhId,
                             TenICDChinh = s.Icdchinh.Ma + " - " + s.Icdchinh.TenTiengViet,
                             KetQuaCLS = s.TomTatKetQuaCLS,
                             GhiChuICDChinh = s.GhiChuICDChinh,
                             CachGiaiQuyet = s.CachGiaiQuyet,
                             YeuCauKhamBenhICDKhacs =
                                s.YeuCauKhamBenhICDKhacs.Where(x => x.YeuCauKhamBenhId == s.Id).Select(item => new ICDKhacDetail()
                                {
                                    TenICDKhac = item.ICD.Ma + " - " + item.ICD.TenTiengViet,
                                    Id = item.ICDId,
                                    GhiChu = item.GhiChu
                                }).ToList()
                         });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetICDKhacDataForGridAsyncDetail(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKhamBenhId = long.Parse(queryInfo.SearchTerms);
            var query = _yeuCauKhamBenhICDKhacRepository.TableNoTracking
                         .Where(icdkhac => icdkhac.YeuCauKhamBenhId == yeuCauKhamBenhId)
                         .Select(s => new ICDKhacDetail
                         {
                             Id = s.Id,
                             TenICDKhac = s.ICD.Ma + " - " + s.ICD.TenTiengViet,
                             GhiChu = s.GhiChu,
                         });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetICDKhacTotalPageForGridAsyncDetail(QueryInfo queryInfo)
        {
            var yeuCauKhamBenhId = long.Parse(queryInfo.SearchTerms);
            var query = _yeuCauKhamBenhICDKhacRepository.TableNoTracking
                         .Where(icdkhac => icdkhac.YeuCauKhamBenhId == yeuCauKhamBenhId)
                         .Select(s => new ICDKhacDetail
                         {
                             Id = s.Id,
                             TenICDKhac = s.ICD.Ma + " - " + s.ICD.TenTiengViet,
                             GhiChu = s.GhiChu,
                         });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDonThuocBacSiKhacDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKhamBenhId = long.Parse(queryInfo.AdditionalSearchString);

            var yeuCauTiepNhanId = BaseRepository.TableNoTracking
                                  .Where(p => p.Id == yeuCauKhamBenhId).Select(p => p.YeuCauTiepNhanId).FirstOrDefault();

            var query = BaseRepository.TableNoTracking
                        .Where(yckb => yckb.Id != yeuCauKhamBenhId && yckb.YeuCauTiepNhanId == yeuCauTiepNhanId && yckb.YeuCauKhamBenhDonThuocs.Count() > 0)
                        .Select(s => new DonThuocBacSiKhacGridVo
                        {
                            Id = s.Id,
                            ThoiDiemHoanThanhDisplay = s.ThoiDiemHoanThanh == null ? null : s.ThoiDiemHoanThanh.Value.ApplyFormatDate(),
                            ThoiDiemHoanThanh = s.ThoiDiemHoanThanh,
                            MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ChuanDoanICD = s.Icdchinh.Ma + " - " + s.Icdchinh.TenTiengViet,
                            BacSiKham = s.BacSiThucHien.User.HoTen,
                        });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDonThuocBacSiKhacTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauKhamBenhId = long.Parse(queryInfo.AdditionalSearchString);

            var yeuCauTiepNhanId = BaseRepository.TableNoTracking
                                  .Where(p => p.Id == yeuCauKhamBenhId).Select(p => p.YeuCauTiepNhanId).FirstOrDefault();
            var query = BaseRepository.TableNoTracking
                        .Where(yckb => yckb.Id != yeuCauKhamBenhId && yckb.YeuCauTiepNhanId == yeuCauTiepNhanId && yckb.YeuCauKhamBenhDonThuocs.Count() > 0)
                        .Select(s => new DonThuocBacSiKhacGridVo
                        {
                            Id = s.Id,
                            ThoiDiemHoanThanhDisplay = s.ThoiDiemHoanThanh == null ? null : s.ThoiDiemHoanThanh.Value.ApplyFormatDate(),
                            ThoiDiemHoanThanh = s.ThoiDiemHoanThanh,
                            MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ChuanDoanICD = s.Icdchinh.Ma + " - " + s.Icdchinh.TenTiengViet,
                            BacSiKham = s.BacSiThucHien.User.HoTen,
                        });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public GridDataSource GetDonThuocBacSiKhacDataForGridAsyncDetail(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKhamBenhId = long.Parse(queryInfo.SearchTerms);
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                        .Where(dtct => dtct.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == yeuCauKhamBenhId)
                        .Select(s => new LichSuKeToaChildGridVo
                        {
                            Id = s.Id,
                            DuocPhamId = s.DuocPhamId,
                            Ten = s.DuocPham.Ten,
                            HoatChat = s.DuocPham.HoatChat,
                            DVT = s.DonViTinh.Ten,
                            LoaiDonThuoc = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc,
                            NhomLSKT = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                            SangDisplay = s.DungSang.FloatToStringFraction() + " " + (s.ThoiGianDungSang == null ? "" : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")"),
                            TruaDisplay = s.DungTrua.FloatToStringFraction() + " " + (s.ThoiGianDungTrua == null ? "" : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")"),
                            ChieuDisplay = s.DungChieu.FloatToStringFraction() + " " + (s.ThoiGianDungChieu == null ? "" : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")"),
                            ToiDisplay = s.DungToi.FloatToStringFraction() + " " + (s.ThoiGianDungToi == null ? "" : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")"),
                            SoLuong = s.SoLuong,
                            SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                            SoNgayDung = s.SoNgayDung,
                            TenDuongDung = s.DuongDung.Ten,
                            GhiChu = s.GhiChu
                        });
            var lstQuery = query.ToList();
            var lstBHYTSTT = lstQuery.Where((p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)).Select(p => p).ToList();
            var lstKhongBHYTSTT = lstQuery.Where((p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT)).Select(p => p).ToList();
            for (int i = 0; i < lstBHYTSTT.Count(); i++)
            {
                lstBHYTSTT[i].STT = i + 1;
            }
            for (int i = 0; i < lstKhongBHYTSTT.Count(); i++)
            {
                lstKhongBHYTSTT[i].STT = i + 1;
            }
            query = lstQuery.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public GridDataSource GetDonThuocBacSiKhacTotalPageForGridAsyncDetail(QueryInfo queryInfo)
        {
            var yeuCauKhamBenhId = long.Parse(queryInfo.SearchTerms);
            var query = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                          .Where(dtct => dtct.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == yeuCauKhamBenhId)
                        .Select(s => new LichSuKeToaChildGridVo
                        {
                            Id = s.Id,
                            DuocPhamId = s.DuocPhamId,
                            Ten = s.DuocPham.Ten,
                            HoatChat = s.DuocPham.HoatChat,
                            DVT = s.DonViTinh.Ten,
                            LoaiDonThuoc = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc,
                            NhomLSKT = s.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                            SangDisplay = s.DungSang.FloatToStringFraction() + " " + (s.ThoiGianDungSang == null ? "" : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")"),
                            TruaDisplay = s.DungTrua.FloatToStringFraction() + " " + (s.ThoiGianDungTrua == null ? "" : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")"),
                            ChieuDisplay = s.DungChieu.FloatToStringFraction() + " " + (s.ThoiGianDungChieu == null ? "" : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")"),
                            ToiDisplay = s.DungToi.FloatToStringFraction() + " " + (s.ThoiGianDungToi == null ? "" : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")"),
                            SoLuong = s.SoLuong,
                            SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                            SoNgayDung = s.SoNgayDung,
                            TenDuongDung = s.DuongDung.Ten,
                            GhiChu = s.GhiChu
                        });
            var lstQuery = query.ToList();
            var lstBHYTSTT = lstQuery.Where((p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)).Select(p => p).ToList();
            var lstKhongBHYTSTT = lstQuery.Where((p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT)).Select(p => p).ToList();
            for (int i = 0; i < lstBHYTSTT.Count(); i++)
            {
                lstBHYTSTT[i].STT = i + 1;
            }
            for (int i = 0; i < lstKhongBHYTSTT.Count(); i++)
            {
                lstKhongBHYTSTT[i].STT = i + 1;
            }
            query = lstQuery.AsQueryable();
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public string FormatNumber(double? inputNumber)
        {
            var result = inputNumber.FloatToStringFraction();
            return result;
        }

        public async Task<bool> KiemTraNgayChuyenVien(DateTime? ngayChuyenVien, long yeuCauKhamBenhId)
        {
            var thoiDiemTiepNhan = await BaseRepository.TableNoTracking
                                    .Where(p => p.Id == yeuCauKhamBenhId)
                                    .Select(p => p.YeuCauTiepNhan.ThoiDiemTiepNhan)
                                    .FirstOrDefaultAsync();
            var myTime = DateTime.Today;

            if (ngayChuyenVien != null)
            {
                if (thoiDiemTiepNhan <= ngayChuyenVien.Value && ngayChuyenVien.Value.Date <= myTime)
                {
                    return false;
                }
            }
            else if (ngayChuyenVien == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> KiemTraChiDinhDichVuDaThemTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, long dichVuBenhVienId, Enums.EnumNhomGoiDichVu? nhomDichVu)
        {
            var kiemTra = false;
            if (nhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                kiemTra = await BaseRepository.TableNoTracking
                    .AnyAsync(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                   && x.DichVuKhamBenhBenhVienId == dichVuBenhVienId
                                   && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
            }
            else if (nhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
            {
                kiemTra = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .AnyAsync(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                    && x.DichVuKyThuatBenhVienId == dichVuBenhVienId
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
            }
            else if (nhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
            {
                kiemTra = await _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking
                    .AnyAsync(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                    && x.DichVuGiuongBenhVienId == dichVuBenhVienId
                                    && x.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy);
            }

            return kiemTra;
        }

        public async Task<bool> KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, List<string> lstDichVu, long? noiTruPhieuDieuTriId = null)
        {
            var lstDichVuId = new List<long>();
            foreach (var dichVu in lstDichVu)
            {
                var dichVuObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(dichVu);
                lstDichVuId.Add(dichVuObj.DichVuId);
            }

            //BVHD-3809
            // Áp dụng cho nội trú, kiểm tra theo ngày điều trị
            DateTime? ngayDieuTri = null;
            if (noiTruPhieuDieuTriId != null)
            {
                ngayDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking.FirstOrDefault(p => p.Id == noiTruPhieuDieuTriId.Value)?.NgayDieuTri.Date;
            }

            var kiemTra = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Any(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                               && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                               //&& lstDichVuId.Any(a => a == x.DichVuKyThuatBenhVienId)
                               && lstDichVuId.Contains(x.DichVuKyThuatBenhVienId)
                               //BVHD-3809
                               && (ngayDieuTri == null || (x.NoiTruPhieuDieuTriId != null && x.NoiTruPhieuDieuTri.NgayDieuTri.Date == ngayDieuTri.Value))
                               );

            return kiemTra;
        }

        public async Task<bool> KiemTraNhomDichVuChiDinhDichVuKyThuat(long? nhomDichVuId, List<string> lstDichVu)
        {
            if (!lstDichVu.Any())
            {
                return nhomDichVuId != null;
            }

            bool kiemTra = false;
            long nhomIdTemp = 0;
            foreach (var dichVu in lstDichVu)
            {
                var dichVuObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(dichVu);

                if (nhomIdTemp == 0 || nhomIdTemp == dichVuObj.NhomId)
                {
                    nhomIdTemp = dichVuObj.NhomId;
                }
                else
                {
                    kiemTra = true;
                    break;
                }
            }

            return (kiemTra && nhomDichVuId == null) || (!kiemTra && nhomDichVuId != null);
        }

        public async Task<bool> KiemTraNhomDichVuDungTheoDichVuChiDinhDichVuKyThuat(long? nhomDichVuId, List<string> lstDichVu)
        {
            bool kiemTra = true;
            if (nhomDichVuId == null || !lstDichVu.Any())
            {
                return kiemTra;
            }

            foreach (var dichVu in lstDichVu)
            {
                var dichVuObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(dichVu);

                if (dichVuObj.NhomId != nhomDichVuId)
                {
                    kiemTra = false;
                    break;
                }
            }

            return kiemTra;
        }

        public async Task<bool> KiemTraChiDinhGoiDichVuDaCoDichVuTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, long goiDichVuId, List<string> lstDichVu)
        {
            var kiemTra = false;

            if (!lstDichVu.Any())
            {
                return kiemTra;
            }

            var lstDichVuDuocChonTrongGoi = new List<DichVuChiDinhTrongGoiVo>();
            foreach (var item in lstDichVu)
            {
                lstDichVuDuocChonTrongGoi.Add(JsonConvert.DeserializeObject<DichVuChiDinhTrongGoiVo>(item));
            }

            var thongTinGoiDichVu = await _goiDichVuRepository.TableNoTracking
                .Include(x => x.GoiDichVuChiTietDichVuGiuongs)
                .Include(x => x.GoiDichVuChiTietDichVuKhamBenhs)
                .Include(x => x.GoiDichVuChiTietDichVuKyThuats)
                .Where(x => x.Id == goiDichVuId)
                .FirstAsync();

            var lstGoiChiTietDVKBId = lstDichVuDuocChonTrongGoi
                .Where(x => x.NhomDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.Id).ToList();
            var lstGoiChiTietDVKTId = lstDichVuDuocChonTrongGoi
                .Where(x => x.NhomDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.Id).ToList();
            var lstGoiChiTietDVGId = lstDichVuDuocChonTrongGoi
                .Where(x => x.NhomDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).Select(x => x.Id).ToList();

            var lstDVKBId = thongTinGoiDichVu.GoiDichVuChiTietDichVuKhamBenhs.Where(x => lstGoiChiTietDVKBId.Any(y => y == x.Id)).Select(x => x.DichVuKhamBenhBenhVienId).ToList();
            var lstDVKTId = thongTinGoiDichVu.GoiDichVuChiTietDichVuKyThuats.Where(x => lstGoiChiTietDVKTId.Any(y => y == x.Id)).Select(x => x.DichVuKyThuatBenhVienId).ToList();
            var lstDVGId = thongTinGoiDichVu.GoiDichVuChiTietDichVuGiuongs.Where(x => lstGoiChiTietDVGId.Any(y => y == x.Id)).Select(x => x.DichVuGiuongBenhVienId).ToList();

            kiemTra = await _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.YeuCauKhamBenhs)
                .Include(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauDichVuGiuongBenhViens)
                .AnyAsync(x => x.Id == yeuCauTiepNhanId
                            && (x.YeuCauKhamBenhs.Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && lstDVKBId.Any(z => z == y.DichVuKhamBenhBenhVienId))
                            || x.YeuCauDichVuKyThuats.Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && lstDVKTId.Any(z => z == y.DichVuKyThuatBenhVienId))
                            || x.YeuCauDichVuGiuongBenhViens.Any(y => y.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy && lstDVGId.Any(z => z == y.DichVuGiuongBenhVienId))));

            //kiemTra = yeuCauTiepNhan.YeuCauKhamBenhs.Any(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && lstDVKB.Any(y => y == x.DichVuKhamBenhBenhVienId))
            //          || yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && lstDVKT.Any(y => y == x.DichVuKyThuatBenhVienId))
            //          || yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(x => x.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy && lstDVG.Any(y => y == x.DichVuGiuongBenhVienId));

            return kiemTra;
        }

        public async Task ThemGhiChuDonThuocHoacVatTuChiTiet(InputStringStoredVo inputStringStoredVo)
        {
            if (!string.IsNullOrEmpty(inputStringStoredVo.GhiChu))
            {
                if (inputStringStoredVo.Loai == Enums.InputStringStoredKey.Thuoc)// Thuốc
                {

                    var isExists = await _inputStringStoredRepository
                                   .TableNoTracking
                                   .AnyAsync(p => p.Value.Trim().ToLower() == inputStringStoredVo.GhiChu.Trim().ToLower()
                                              && p.Set == Enums.InputStringStoredKey.Thuoc);
                    if (!isExists)
                    {
                        var inputStringStored = new Core.Domain.Entities.InputStringStoreds.InputStringStored
                        {
                            Set = inputStringStoredVo.Loai,
                            Value = inputStringStoredVo.GhiChu
                        };
                        await _inputStringStoredRepository.AddAsync(inputStringStored);
                    }
                }
                else if (inputStringStoredVo.Loai == Enums.InputStringStoredKey.VatTu)// Vật tư
                {
                    var isExists = await _inputStringStoredRepository
                                       .TableNoTracking
                                       .AnyAsync(p => p.Value.Trim().ToLower() == inputStringStoredVo.GhiChu.Trim().ToLower()
                                                  && p.Set == Enums.InputStringStoredKey.VatTu);
                    if (!isExists)
                    {
                        var inputStringStored = new Core.Domain.Entities.InputStringStoreds.InputStringStored
                        {
                            Set = inputStringStoredVo.Loai,
                            Value = inputStringStoredVo.GhiChu
                        };
                        await _inputStringStoredRepository.AddAsync(inputStringStored);
                    }
                }
                else
                {
                    var isExists = await _inputStringStoredRepository
                                       .TableNoTracking
                                       .AnyAsync(p => p.Value.Trim().ToLower() == inputStringStoredVo.GhiChu.Trim().ToLower()
                                                  && p.Set == Enums.InputStringStoredKey.LyDoNhapVien);
                    if (!isExists)
                    {
                        var inputStringStored = new Core.Domain.Entities.InputStringStoreds.InputStringStored
                        {
                            Set = inputStringStoredVo.Loai,
                            Value = inputStringStoredVo.GhiChu
                        };
                        await _inputStringStoredRepository.AddAsync(inputStringStored);
                    }
                }
            }
        }
        public async Task<bool> CheckDonThuocChiTietExist(long donThuocChiTietId)
        {
            //var checkExist = await _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
            //    .Where(x => x.Id == donThuocChiTietId).ToListAsync();
            //if (checkExist.Any())
            //{
            //    return true;
            //}
            //return false;

            //Cập nhật 08/12/2022
            var checkExist = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                .Any(x => x.Id == donThuocChiTietId);
            return checkExist;
        }

        public async Task<bool> CheckVatTuChiTietExist(long vatTuChiTietId)
        {
            var checkExist = await _yeuCauKhamBenhDonVTYTChiTietRepository.TableNoTracking
               .Where(x => x.Id == vatTuChiTietId).ToListAsync();
            if (checkExist.Any())
            {
                return true;
            }
            return false;
        }

        public async Task<string> KiemTraDonThuocChiTietThanhToan(long donThuocChiTietId)
        {
            var ycDonThuocChiTiet = await _yeuCauKhamBenhDonThuocChiTietRepository.GetByIdAsync(donThuocChiTietId,
              x => x.Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.DonThuocThanhToans)
                  .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonThuocThanhToan).ThenInclude(o => o.DonThuocThanhToanChiTiets));
            //kiem tra truoc khi cap nhat
            if (ycDonThuocChiTiet == null || ycDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan))
            {
                //return "Dược phẩm đã được thanh toán";
                return GetResourceValueByResourceName("DonThuoc.ThuocDaThanhToan");
            }
            return string.Empty;
        }

        public async Task<string> KiemTraVatTuChiTietThanhToan(long yeuCauKhamBenhDonVTYTId)
        {
            var ycVatTuChiTiet = await _yeuCauKhamBenhDonVTYTChiTietRepository.GetByIdAsync(yeuCauKhamBenhDonVTYTId,
             x => x.Include(o => o.YeuCauKhamBenhDonVTYT).ThenInclude(dt => dt.DonVTYTThanhToans)
                 .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.DonVTYTThanhToan).ThenInclude(o => o.DonVTYTThanhToanChiTiets));

            if (ycVatTuChiTiet == null)
            {
                return GetResourceValueByResourceName("VatTu.VTYTDeleted");
            }
            //kiem tra truoc khi cap nhat
            if (ycVatTuChiTiet == null || ycVatTuChiTiet.DonVTYTThanhToanChiTiets.Any(o => o.DonVTYTThanhToan.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan))
            {
                //return "Vật tư đã được thanh toán";
                return GetResourceValueByResourceName("VatTu.VatTuDaThanhToan");
            }
            return string.Empty;
        }


        public async Task<string> ThemVatTuChiTiet(VatTuChiTietVo vatTuChiTiet)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh ycKhamBenh = BaseRepository.GetById(vatTuChiTiet.YeuCauKhamBenhId, x =>
                x.Include(o => o.YeuCauTiepNhan)
                    .Include(o => o.YeuCauKhamBenhLichSuTrangThais)
                    .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.DonVTYTThanhToans));

            if (ycKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                ycKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
                ycKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                ycKhamBenh.NoiThucHienId = ycKhamBenh.NoiDangKyId; // _userAgentHelper.GetCurrentNoiLLamViecId();
                ycKhamBenh.ThoiDiemThucHien = DateTime.Now;
                // lưu lịch sử
                var lichSuNew = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = ycKhamBenh.TrangThai,
                    MoTa = ycKhamBenh.TrangThai.GetDescription(),
                };
                ycKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
            }

            var donVTYT = ycKhamBenh.YeuCauKhamBenhDonVTYTs.FirstOrDefault();
            if (donVTYT == null)
            {
                donVTYT = new YeuCauKhamBenhDonVTYT
                {
                    TrangThai = Enums.EnumTrangThaiDonVTYT.ChuaCapVTYT,
                    ThoiDiemKeDon = DateTime.Now,
                    BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                    NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                ycKhamBenh.YeuCauKhamBenhDonVTYTs.Add(donVTYT);
            }

            var vatTu = _vatTuBenhVienRepository.GetById(vatTuChiTiet.VatTuId,
                x => x.Include(o => o.VatTus)
                    .Include(dpbv => dpbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu)
                    .Include(dpbv => dpbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho));

            var ycDonVTYTChiTiet =
                new YeuCauKhamBenhDonVTYTChiTiet
                {
                    VatTuBenhVienId = vatTu.Id,
                    Ten = vatTu.VatTus.Ten,
                    Ma = vatTu.VatTus.Ma,
                    NhomVatTuId = vatTu.VatTus.NhomVatTuId,

                    DonViTinh = vatTu.VatTus.DonViTinh,
                    NhaSanXuat = vatTu.VatTus.NhaSanXuat,
                    NuocSanXuat = vatTu.VatTus.NuocSanXuat,
                    QuyCach = vatTu.VatTus.QuyCach,
                    MoTa = vatTu.VatTus.MoTa,
                    SoLuong = vatTuChiTiet.SoLuong,
                    GhiChu = vatTuChiTiet.GhiChu
                };
            donVTYT.YeuCauKhamBenhDonVTYTChiTiets.Add(ycDonVTYTChiTiet);

            if ((vatTu.NhapKhoVatTuChiTiets.Where(o => o.NhapKhoVatTu.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc && !o.LaVatTuBHYT && o.HanSuDung >= DateTime.Now && o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < vatTuChiTiet.SoLuong))
            {
                //return "Vật tư không có trong kho hoặc số lượng tồn không đủ";
                return GetResourceValueByResourceName("DonVTYT.DonVTYT.VTYTSoLuongTon");
            }
            //ktra don vtyt thanh toan
            var donVTYTThanhToan = donVTYT.DonVTYTThanhToans.FirstOrDefault(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan);
            if (donVTYTThanhToan == null)
            {
                donVTYTThanhToan = new DonVTYTThanhToan
                {
                    YeuCauKhamBenhId = ycKhamBenh.Id,
                    YeuCauTiepNhanId = ycKhamBenh.YeuCauTiepNhan.Id,
                    BenhNhanId = ycKhamBenh.YeuCauTiepNhan.BenhNhanId,
                    TrangThai = Enums.TrangThaiDonVTYTThanhToan.ChuaXuatVTYT,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                };
                donVTYT.DonVTYTThanhToans.Add(donVTYTThanhToan);
                ycKhamBenh.YeuCauTiepNhan.DonVTYTThanhToans.Add(donVTYTThanhToan);
            }
            //them don thuoc thanh toan chi tiet
            double soLuongCanXuat = vatTuChiTiet.SoLuong;
            while (!soLuongCanXuat.Equals(0))
            {
                // tinh so luong xuat
                var nhapKhoVatTuChiTiet = vatTu.NhapKhoVatTuChiTiets
                    .Where(o => (o.NhapKhoVatTu.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc && !o.LaVatTuBHYT) && o.HanSuDung >= DateTime.Now && o.SoLuongNhap > o.SoLuongDaXuat)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                var soLuongTon = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;

                nhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongXuat;
                var xuatKhoChiTiet = new XuatKhoVatTuChiTietViTri
                {
                    SoLuongXuat = soLuongXuat,
                    NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                    XuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                    {
                        VatTuBenhVien = vatTu
                    }
                };

                var donVTYTThanhToanChiTiet = new DonVTYTThanhToanChiTiet
                {
                    VatTuBenhVienId = vatTu.Id,
                    YeuCauKhamBenhDonVTYTChiTiet = ycDonVTYTChiTiet,
                    XuatKhoVatTuChiTietViTri = xuatKhoChiTiet,
                    Ten = vatTu.VatTus.Ten,
                    Ma = vatTu.VatTus.Ma,
                    NhomVatTuId = vatTu.VatTus.NhomVatTuId,
                    DonViTinh = vatTu.VatTus.DonViTinh,
                    NhaSanXuat = vatTu.VatTus.NhaSanXuat,
                    NuocSanXuat = vatTu.VatTus.NuocSanXuat,
                    QuyCach = vatTu.VatTus.QuyCach,
                    MoTa = vatTu.VatTus.MoTa,

                    HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                    NhaThauId = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhaThauId,
                    SoHopDongThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoHopDong,
                    SoQuyetDinhThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoQuyetDinh,
                    LoaiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.LoaiThau,
                    NhomThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhomThau,
                    GoiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.GoiThau,
                    NamThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.Nam,
                    DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                    TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                    VAT = nhapKhoVatTuChiTiet.VAT,
                    SoLuong = soLuongXuat,
                    SoTienBenhNhanDaChi = 0
                };

                donVTYTThanhToan.DonVTYTThanhToanChiTiets.Add(donVTYTThanhToanChiTiet);
                soLuongCanXuat = soLuongCanXuat - soLuongXuat;
            }

            ycKhamBenh.CoKeToa = true;
            ycKhamBenh.KhongKeToa = null;
            if (ycKhamBenh.CoChuyenVien != null)
            {
                ycKhamBenh.CoChuyenVien = null;
                ycKhamBenh.BenhVienChuyenVienId = null;
                ycKhamBenh.LyDoChuyenVien = null;
                ycKhamBenh.TinhTrangBenhNhanChuyenVien = null;
                ycKhamBenh.ThoiDiemChuyenVien = null;
                ycKhamBenh.NhanVienHoTongChuyenVienId = null;
                ycKhamBenh.PhuongTienChuyenVien = null;
            }
            if (ycKhamBenh.CoNhapVien != null)
            {
                ycKhamBenh.CoNhapVien = null;
                ycKhamBenh.KhoaPhongNhapVienId = null;
                ycKhamBenh.LyDoNhapVien = null;
            }
            ycKhamBenh.CoTuVong = null;
            if (ycKhamBenh.BacSiKetLuanId == null)
            {
                ycKhamBenh.BacSiKetLuanId = _userAgentHelper.GetCurrentUserId();
            }
            await BaseRepository.UpdateAsync(ycKhamBenh);
            return string.Empty;
        }

        public async Task<string> CapNhatVatTuChiTiet(VatTuChiTietVo vatTuChiTiet)
        {
            var ycDonVTYTChiTiet = _yeuCauKhamBenhDonVTYTChiTietRepository.GetById(vatTuChiTiet.DonVTYTChiTietId,
                x => x.Include(o => o.YeuCauKhamBenhDonVTYT).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan)
                    .Include(o => o.YeuCauKhamBenhDonVTYT).ThenInclude(dt => dt.DonVTYTThanhToans).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(xk => xk.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(xk => xk.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(tt => tt.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(tt => tt.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(tt => tt.TaiKhoanBenhNhanChis)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(tt => tt.DonVTYTThanhToan));
            //kiem tra truoc khi cap nhat
            if (ycDonVTYTChiTiet.DonVTYTThanhToanChiTiets.Any(o => o.DonVTYTThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
            {
                //return "Vật tư đã được thanh toán";
                return GetResourceValueByResourceName("DonVTYT.VTYTDaThanhToan");
            }
            if (ycDonVTYTChiTiet.DonVTYTThanhToanChiTiets.Any(o => o.DonVTYTThanhToan.TrangThai == Enums.TrangThaiDonVTYTThanhToan.DaXuatVTYT))
            {
                return GetResourceValueByResourceName("DonVTYT.VatTuDaXuat");
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var soLuongTruoc = ycDonVTYTChiTiet.SoLuong;

            var vatTu = _vatTuBenhVienRepository.GetById(vatTuChiTiet.VatTuId,
                x => x.Include(dpbv => dpbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu)
                    .ThenInclude(dpbv => dpbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho)
                    .Include(dpbv => dpbv.VatTus));

            ycDonVTYTChiTiet.VatTuBenhVienId = vatTu.Id;
            ycDonVTYTChiTiet.Ten = vatTu.VatTus.Ten;
            ycDonVTYTChiTiet.Ma = vatTu.VatTus.Ma;
            ycDonVTYTChiTiet.NhomVatTuId = vatTu.VatTus.NhomVatTuId;

            ycDonVTYTChiTiet.DonViTinh = vatTu.VatTus.DonViTinh;
            ycDonVTYTChiTiet.NhaSanXuat = vatTu.VatTus.NhaSanXuat;
            ycDonVTYTChiTiet.NuocSanXuat = vatTu.VatTus.NuocSanXuat;
            ycDonVTYTChiTiet.QuyCach = vatTu.VatTus.QuyCach;
            ycDonVTYTChiTiet.MoTa = vatTu.VatTus.MoTa;
            ycDonVTYTChiTiet.SoLuong = vatTuChiTiet.SoLuong;
            ycDonVTYTChiTiet.GhiChu = vatTuChiTiet.GhiChu;

            if (!soLuongTruoc.AlmostEqual(vatTuChiTiet.SoLuong))
            {
                var donVTYTThanhToanChiTietLast = ycDonVTYTChiTiet.DonVTYTThanhToanChiTiets.Last();
                if (soLuongTruoc < vatTuChiTiet.SoLuong)
                {
                    var soLuongTang = vatTuChiTiet.SoLuong - soLuongTruoc;
                    if (vatTu.NhapKhoVatTuChiTiets
                            .Where(o => (!o.LaVatTuBHYT && o.HanSuDung >= DateTime.Now && o.NhapKhoVatTu.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc))
                            .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < soLuongTang)
                    {
                        //return "Vật tư không có trong kho";
                        return GetResourceValueByResourceName("DonVTYT.VTYTSoLuongTon");
                    }

                    //update vat tu thanh toan chi tiet
                    var xuatKhoLast = donVTYTThanhToanChiTietLast.XuatKhoVatTuChiTietViTri;

                    var xkChiTietViTri = BaseRepository.Context.Entry(donVTYTThanhToanChiTietLast).Reference(o => o.XuatKhoVatTuChiTietViTri);
                    if (!xkChiTietViTri.IsLoaded) xkChiTietViTri.Load();

                    var soLuongTonHt = xuatKhoLast.NhapKhoVatTuChiTiet.SoLuongNhap - xuatKhoLast.NhapKhoVatTuChiTiet.SoLuongDaXuat;
                    if (soLuongTonHt >= soLuongTang)
                    {
                        donVTYTThanhToanChiTietLast.SoLuong += soLuongTang;
                        xuatKhoLast.NhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongTang;
                        xuatKhoLast.SoLuongXuat += soLuongTang;
                    }
                    else
                    {
                        donVTYTThanhToanChiTietLast.SoLuong += soLuongTonHt;
                        xuatKhoLast.NhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongTonHt;
                        xuatKhoLast.SoLuongXuat += soLuongTonHt;
                        var soLuongCanXuat = soLuongTang - soLuongTonHt;
                        while (!soLuongCanXuat.Equals(0))
                        {
                            // tinh so luong xuat
                            var nhapKhoVatTuChiTiet = vatTu.NhapKhoVatTuChiTiets
                                .Where(o => (!o.LaVatTuBHYT && o.NhapKhoVatTu.KhoId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc) && o.HanSuDung >= DateTime.Now && o.SoLuongNhap > o.SoLuongDaXuat)
                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                            var soLuongTon =
                                (float)(nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat);
                            var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;

                            nhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongXuat;
                            var xuatKhoChiTiet = new XuatKhoVatTuChiTietViTri
                            {
                                SoLuongXuat = soLuongXuat,
                                NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                                XuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                                {
                                    VatTuBenhVien = vatTu,
                                }
                            };

                            var donVTYTThanhToanChiTiet = new DonVTYTThanhToanChiTiet
                            {
                                VatTuBenhVienId = vatTu.Id,
                                YeuCauKhamBenhDonVTYTChiTiet = ycDonVTYTChiTiet,
                                XuatKhoVatTuChiTietViTri = xuatKhoChiTiet,
                                Ten = vatTu.VatTus.Ten,
                                Ma = vatTu.VatTus.Ma,
                                NhomVatTuId = vatTu.VatTus.NhomVatTuId,
                                DonViTinh = vatTu.VatTus.DonViTinh,
                                NhaSanXuat = vatTu.VatTus.NhaSanXuat,
                                NuocSanXuat = vatTu.VatTus.NuocSanXuat,
                                QuyCach = vatTu.VatTus.QuyCach,
                                MoTa = vatTu.VatTus.MoTa,

                                HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                                NhaThauId = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhaThauId,
                                SoHopDongThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoHopDong,
                                SoQuyetDinhThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoQuyetDinh,
                                LoaiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.LoaiThau,
                                NhomThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhomThau,
                                GoiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.GoiThau,
                                NamThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.Nam,
                                DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                                TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                                VAT = nhapKhoVatTuChiTiet.VAT,
                                SoLuong = soLuongXuat,
                                SoTienBenhNhanDaChi = 0
                            };
                            ycDonVTYTChiTiet.DonVTYTThanhToanChiTiets.Add(donVTYTThanhToanChiTiet);
                            soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                        }
                    }
                }
                else//Giam so luong thuoc
                {
                    var soLuongGiam = soLuongTruoc - vatTuChiTiet.SoLuong;
                    var donVTYTThanhToanChiTiets = ycDonVTYTChiTiet.DonVTYTThanhToanChiTiets.OrderByDescending(o => o.Id).ToList();
                    for (int i = 0; i < donVTYTThanhToanChiTiets.Count; i++)
                    {
                        if (donVTYTThanhToanChiTiets[i].SoLuong <= soLuongGiam)
                        {
                            foreach (var congNo in donVTYTThanhToanChiTiets[i].CongTyBaoHiemTuNhanCongNos)
                            {
                                congNo.WillDelete = true;
                            }
                            foreach (var mienGiam in donVTYTThanhToanChiTiets[i].MienGiamChiPhis)
                            {
                                mienGiam.WillDelete = true;
                            }
                            foreach (var taiKhoanBenhNhanChi in donVTYTThanhToanChiTiets[i].TaiKhoanBenhNhanChis)
                            {
                                taiKhoanBenhNhanChi.DonThuocThanhToanChiTietId = null;
                            }

                            donVTYTThanhToanChiTiets[i].XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= donVTYTThanhToanChiTiets[i].SoLuong;
                            donVTYTThanhToanChiTiets[i].WillDelete = true;
                            donVTYTThanhToanChiTiets[i].XuatKhoVatTuChiTietViTri.WillDelete = true;
                            donVTYTThanhToanChiTiets[i].XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                            if (donVTYTThanhToanChiTiets[i].XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null)
                                donVTYTThanhToanChiTiets[i].XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.WillDelete = true;

                            soLuongGiam -= donVTYTThanhToanChiTiets[i].SoLuong;
                            if (soLuongGiam.AlmostEqual(0))
                                break;
                        }
                        else
                        {
                            donVTYTThanhToanChiTiets[i].XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= soLuongGiam;
                            donVTYTThanhToanChiTiets[i].XuatKhoVatTuChiTietViTri.SoLuongXuat -= soLuongGiam;
                            donVTYTThanhToanChiTiets[i].SoLuong -= soLuongGiam;
                            break;
                        }
                    }
                }
            }

            await _yeuCauKhamBenhDonVTYTChiTietRepository.UpdateAsync(ycDonVTYTChiTiet);
            return string.Empty;
        }

        public async Task<string> XoaVatTuChiTiet(VatTuChiTietVo vatTuChiTiet)
        {
            var ycDonVTYTChiTiet = _yeuCauKhamBenhDonVTYTChiTietRepository.GetById(vatTuChiTiet.DonVTYTChiTietId,
                x => x.Include(o => o.YeuCauKhamBenhDonVTYT).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauTiepNhan)
                    .Include(o => o.YeuCauKhamBenhDonVTYT).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTs)
                       .Include(o => o.YeuCauKhamBenhDonVTYT).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocs)
                    .Include(o => o.YeuCauKhamBenhDonVTYT).ThenInclude(dt => dt.DonVTYTThanhToans)
                    .Include(o => o.YeuCauKhamBenhDonVTYT).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.DonVTYTThanhToan).ThenInclude(o => o.DonVTYTThanhToanChiTiets)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet));
            //kiem tra truoc khi cap nhat
            if (ycDonVTYTChiTiet.DonVTYTThanhToanChiTiets.Any(o => o.DonVTYTThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
            {
                //return "Vật tư đã được thanh toán";
                return GetResourceValueByResourceName("DonVTYT.VTYTDaThanhToan");
            }
            if (ycDonVTYTChiTiet.DonVTYTThanhToanChiTiets.Any(o => o.DonVTYTThanhToan.TrangThai == Enums.TrangThaiDonVTYTThanhToan.DaXuatVTYT))
            {
                return GetResourceValueByResourceName("DonVTYT.VatTuDaXuat");
            }
            //hoan lai duoc pham da book trong kho
            foreach (var donVTYTThanhToanChiTiet in ycDonVTYTChiTiet.DonVTYTThanhToanChiTiets)
            {
                foreach (var congNo in donVTYTThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                {
                    congNo.WillDelete = true;
                }
                foreach (var mienGiam in donVTYTThanhToanChiTiet.MienGiamChiPhis)
                {
                    mienGiam.WillDelete = true;
                }
                foreach (var taiKhoanBenhNhanChi in donVTYTThanhToanChiTiet.TaiKhoanBenhNhanChis)
                {
                    taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                }

                donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                donVTYTThanhToanChiTiet.WillDelete = true;
                donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                if (donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null)
                    donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.WillDelete = true;
                if (donVTYTThanhToanChiTiet.DonVTYTThanhToan.DonVTYTThanhToanChiTiets.All(o => o.WillDelete))
                {
                    donVTYTThanhToanChiTiet.DonVTYTThanhToan.WillDelete = true;
                }
            }
            ycDonVTYTChiTiet.WillDelete = true;
            if (ycDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.YeuCauKhamBenhDonVTYTChiTiets.All(o => o.WillDelete))
            {
                ycDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.WillDelete = true;
            }
            if (ycDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.YeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.All(o => o.WillDelete) && !ycDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.YeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any())
            {
                ycDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.YeuCauKhamBenh.CoKeToa = null;
            }

            await _yeuCauKhamBenhDonVTYTChiTietRepository.UpdateAsync(ycDonVTYTChiTiet);
            return string.Empty;
        }

        public async Task<GridDataSource> GetVatTuYTDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var yeuCauKhamBenhId = long.Parse(queryInfo.AdditionalSearchString);
                var query = _yeuCauKhamBenhDonVTYTChiTietRepository.TableNoTracking
                             .Where(ycvt => ycvt.YeuCauKhamBenhDonVTYT.YeuCauKhamBenhId == yeuCauKhamBenhId && ycvt.SoLuong > 0)
                             .Select(s => new VatTuYTGridVo
                             {
                                 Id = s.Id,
                                 VatTuBenhVienId = s.VatTuBenhVienId,
                                 YeuCauKhamBenhId = yeuCauKhamBenhId,
                                 Ten = s.Ten,
                                 Ma = s.Ma,
                                 SoLuong = s.SoLuong,
                                 DonViTinh = s.DonViTinh,
                                 GhiChu = s.GhiChu,
                                 DonGiaNhap = s.DonVTYTThanhToanChiTiets.FirstOrDefault() != null ? s.DonVTYTThanhToanChiTiets.FirstOrDefault().DonGiaNhap : 0,
                                 DonGia = s.DonVTYTThanhToanChiTiets.FirstOrDefault() != null ? s.DonVTYTThanhToanChiTiets.FirstOrDefault().DonGiaBan : 0,
                                 TiLeTheoThapGia = s.DonVTYTThanhToanChiTiets.FirstOrDefault() != null ? s.DonVTYTThanhToanChiTiets.FirstOrDefault().TiLeTheoThapGia : 0,
                                 VAT = s.DonVTYTThanhToanChiTiets.FirstOrDefault() != null ? s.DonVTYTThanhToanChiTiets.FirstOrDefault().VAT : 0,
                                 NhaSX = s.NhaSanXuat,
                                 NuocSX = s.NuocSanXuat,

                                 //BVHD-3905
                                 TiLeThanhToanBHYT = s.VatTuBenhVien.TiLeThanhToanBHYT
                             });
                //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                //    .Take(queryInfo.Take).ToArrayAsync();
                //await Task.WhenAll(countTask, queryTask);
                //return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
                var lstQuery = await query.ToListAsync();
                for (int i = 0; i < lstQuery.Count(); i++)
                {
                    lstQuery[i].STT = i + 1;
                }

                query = lstQuery.AsQueryable();
                var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
            else
            {
                return null;
            }
        }
        public async Task<GridDataSource> GetVatTuYTTotalPageForGridAsync(QueryInfo queryInfo)
        {
            return null;
        }


        public string InVatTuKhamBenh(InVatTuReOrder inVatTu)
        {
            var content = string.Empty;
            var contentVatTu = string.Empty;
            var resultVatTu = string.Empty;

            var header = string.Empty;
            if (inVatTu.Header == true)
            {
                header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>VẬT TƯ Y TẾ</th>" +
                         "</p>";
            }
            var listVatTuYT = inVatTu.ListGridVatTu;
            Dictionary<long?, int> dictionary = new Dictionary<long?, int>();
            //sort Grid hiện tại theo Grid truyền vào
            dictionary = listVatTuYT
                .Select((id, index) => new
                {
                    key = (long?)id.Id,
                    rank = index
                }).ToDictionary(o => o.key, o => o.rank);
            var templateVatTuYT = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("KhamBenhVatTuYTe")).First();
            var infoBN = ThongTinBenhNhanPhieuThuoc(inVatTu.YeuCauTiepNhanId, inVatTu.YeuCauKhamBenhId);
            //var getKCBBanDau = string.Empty;
            //if (infoBN != null && !string.IsNullOrEmpty(infoBN.BHYTMaDKBD))
            //{
            //    getKCBBanDau = _benhVienRepository.TableNoTracking
            //                .Where(bv => bv.Ma == infoBN.BHYTMaDKBD).Select(p => p.Ten).FirstOrDefault();
            //}

            //var getTenBSKham = _bacSiChiDinhRepository.TableNoTracking
            //                    .Where(bs => bs.Id == _userAgentHelper.GetCurrentUserId())
            //                    .Select(bs => bs.HoTen).FirstOrDefault();


            var vTYTChiTiets = BaseRepository.TableNoTracking
                         .Include(yckb => yckb.YeuCauTiepNhan)
                         .Include(yckb => yckb.YeuCauKhamBenhDonVTYTs).ThenInclude(ycdt => ycdt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dtct => dtct.NhomVatTu)
                         .Include(yckb => yckb.YeuCauKhamBenhDonVTYTs).ThenInclude(ycdt => ycdt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dtct => dtct.VatTuBenhVien)
                         .SelectMany(yckb => yckb.YeuCauKhamBenhDonVTYTs)
                                 .Select(vt => vt)
                                 .Where(vt => vt.YeuCauKhamBenhId == inVatTu.YeuCauKhamBenhId)
                         .SelectMany(ycvt => ycvt.YeuCauKhamBenhDonVTYTChiTiets).Include(s => s.YeuCauKhamBenhDonVTYT)
                         .Select(vtct => vtct).Include(dtct => dtct.NhomVatTu).Include(dtct => dtct.VatTuBenhVien)
                         .OrderBy(p => dictionary.Any(a => a.Key == p.Id) ? dictionary[p.Id] : dictionary.Count)
                         .ToList();
            var userCurrentId = vTYTChiTiets.Any() ? vTYTChiTiets.First().YeuCauKhamBenhDonVTYT?.BacSiKeDonId : 0;

            var tenBacSiKeDon = _userRepository.TableNoTracking
                             .Where(u => u.Id == userCurrentId).Select(u =>
                             (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                           //+ (u.NhanVien.ChucDanh != null ? u.NhanVien.ChucDanh.NhomChucDanh.Ma + "." : "")
                           + u.HoTen).FirstOrDefault();
            var STT = 0;
            foreach (var item in vTYTChiTiets)
            {
                STT++;
                resultVatTu += "<tr>";
                resultVatTu += "<td style='vertical-align: top;text-align: center'>" + STT + "</td>";
                resultVatTu += "<td>" + item.Ten
                            + (!string.IsNullOrEmpty(item.GhiChu) ? "</br> </i>" + item.GhiChu + "<i>" : "")
                    + "</td>";
                resultVatTu += "<td style='vertical-align: top;text-align: center'>" + item.SoLuong + " " + item.DonViTinh + "</td>";
                //resultVatTu += "<td><i>" + (!string.IsNullOrEmpty(item.GhiChu) ? item.GhiChu : "&nbsp;") + "</i></td>";
                resultVatTu += "</tr>";
            }
            resultVatTu = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>TÊN VTYT</th><th>SỐ LƯỢNG</th></tr></thead><tbody>" + resultVatTu + "</tbody></table>";
            if (vTYTChiTiets.Any())
            {
                var data = new DataYCKBVatTu
                {
                    MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                    HoTen = infoBN.HoTen,
                    NamSinhDayDu = infoBN.NamSinhDayDu,
                    Tuoi = infoBN.Tuoi,
                    CanNang = infoBN.CanNang,
                    GioiTinh = infoBN?.GioiTinh,
                    DiaChi = infoBN?.DiaChi,
                    SoTheBHYT = infoBN.BHYTMaSoThe,
                    ChuanDoan = infoBN?.ChuanDoan,
                    BacSiKham = tenBacSiKeDon,
                    MaBN = infoBN.MaBN,
                    SoDienThoai = infoBN.SoDienThoai,
                    CongKhoan = STT,
                    TemplateVatTu = resultVatTu,
                    LogoUrl = inVatTu.HostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                    CMND = infoBN?.CMND,
                    NguoiGiamHo = infoBN?.NguoiGiamHo,
                    ThoiDiemKeDon = vTYTChiTiets.Any() ? vTYTChiTiets.Select(z => z.YeuCauKhamBenhDonVTYT.ThoiDiemKeDon).First() : (DateTime?)null

                };
                contentVatTu = TemplateHelpper.FormatTemplateWithContentTemplate(templateVatTuYT.Body, data);
            }
            return contentVatTu;
        }

        public async Task<string> KiemTraMucTranChiPhi(long yeuCauKhamBenhId)
        {
            var thanhTienDuocPham = _donThuocThanhToanChiTietRepository
                            .TableNoTracking.Where(p => p.DonThuocThanhToan.YeuCauKhamBenhId == yeuCauKhamBenhId)
                            .Sum(p => p.GiaBan);
            var thanhTienVatTu = _donVTYTThanhToanChiTietRepository
                            .TableNoTracking.Where(p => p.DonVTYTThanhToan.YeuCauKhamBenhId == yeuCauKhamBenhId)
                            .Sum(p => p.GiaBan);
            var sumThanhTien = thanhTienDuocPham + thanhTienVatTu;
            var mucTranChiPhi = await _cauHinhRepository
                        .TableNoTracking.Where(p => p.Name == "CauHinhTiepNhan.MucTranChiPhiKeToa").Select(p => p.Value).FirstAsync();
            if (sumThanhTien >= decimal.Parse(mucTranChiPhi))
            {
                return "NotOK";
            }
            return string.Empty;
        }

        public async Task<string> GetSoDangKyDuocPhamNgoaiBv()
        {
            var lastId = await _duocPhamRepository.TableNoTracking.Select(p => p.Id).LastAsync();
            var soDangKyPre = "SDK-" + lastId;
            var soDangKyTemp = "SDK-" + lastId;
            var soDangKy = "SDK-" + lastId;

            var lstSoDangKy = await _duocPhamRepository.TableNoTracking
                                        .Where(p => p.SoDangKy.Contains(soDangKyTemp))
                                        .OrderBy(p => p.SoDangKy)
                                        .Select(p => p.SoDangKy).ToListAsync();
            var number = 0;
            foreach (var item in lstSoDangKy)
            {
                if (soDangKy == item)
                {
                    number++;
                    var strNumber = number < 10 ? "0" + number : number.ToString();
                    soDangKyTemp = soDangKyTemp + "-" + strNumber;
                    soDangKy = soDangKyTemp;
                    soDangKyTemp = soDangKyPre;
                }
            }
            return soDangKy;
        }

        public async Task<bool> IsTenICDKhacExists(long? idICD, long yeuCauKhamBenhChanDoanICDId, long yeuCauKhamBenhId)
        {
            if (idICD == null)
            {
                return true;
            }

            var result = false;
            if (yeuCauKhamBenhChanDoanICDId == 0)
            {
                result = await _yeuCauKhamBenhICDKhacRepository.TableNoTracking.AnyAsync(p => p.YeuCauKhamBenhId == yeuCauKhamBenhId && p.ICDId == idICD);
            }
            else
            {
                result = await _yeuCauKhamBenhICDKhacRepository.TableNoTracking.AnyAsync(p => p.YeuCauKhamBenhId == yeuCauKhamBenhId && p.ICDId == idICD && p.Id != yeuCauKhamBenhChanDoanICDId);
            }
            if (result)
                return false;
            return true;
        }
        public async Task<string> GetTenICD(long id)
        {
            return await _iCDRepository.TableNoTracking.Where(p => p.Id == id).Select(p => p.Ma + " - " + p.TenTiengViet).FirstOrDefaultAsync();
        }

        public async Task<bool> LaBacSiKeDon(KiemTraThuocTrungBSKe kiemTraThuocTrungBSKe)
        {
            var bacSiKeDonId = _userAgentHelper.GetCurrentUserId();
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(kiemTraThuocTrungBSKe.YeuCauTiepNhanId, s => s.Include(z => z.YeuCauKhamBenhs).ThenInclude(z => z.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets));

            foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs)
            {
                var donThuocChiTiets = yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.SelectMany(z => z.YeuCauKhamBenhDonThuocChiTiets).ToList();
                if (donThuocChiTiets.Any(c => c.DuocPhamId == kiemTraThuocTrungBSKe.DuocPhamId && c.YeuCauKhamBenhDonThuoc.BacSiKeDonId != bacSiKeDonId))
                {
                    return true;
                }
            }
            //foreach (var donThuoc in _yeuCauKhamBenhDonThuocRepository.TableNoTracking.Include(z => z.YeuCauKhamBenhDonThuocChiTiets).Where(z => z.YeuCauKhamBenhId == kiemTraThuocTrungBSKe.YeuCauKhamBenhId).ToList())
            //quay
            //    if (donThuoc.BacSiKeDonId == bacSiKeDonId && donThuoc.YeuCauKhamBenhDonThuocChiTiets.Any(c => c.DuocPhamId == kiemTraThuocTrungBSKe.DuocPhamId))
            //    {
            //        return true;
            //    }
            //}
            return false;
        }
        //BVHD-3959: Chỉnh lại logic hiển thị tên thuốc theo thông tư 52/2017: 1 hoạt chất và nhiều hoạt chất (từ 2 hoạt chất trở lên)
        public string FormatTenDuocPham(string tenThuongMai, string tenQuocTe, string hamLuong, long? duocPhamBenhVienPhanNhomId, bool? laPhieuThucHienThuoc = null) // =>  tenThuongMai => tên dược phẩm , tenQuocTe => hoạt chất
        {
            var dp = string.Empty;
            // kiểm tra dược phẩm có mấy hoạt chất
            var soLuongHoatChat = SoluongHoatChatCuaDuocPham(tenQuocTe);
            // dược phẩm Phan nhóm == Sinh phẩm hoặc sinh phẩm chẩn đoán
            var valueDuocPhamPhanNhom = ValueDuocPhamBenhVien(duocPhamBenhVienPhanNhomId);
            if (valueDuocPhamPhanNhom == true)
            {
                dp = tenThuongMai + " " + hamLuong;
            }
            else
            {
                if (soLuongHoatChat == 1)
                {
                    //1.1 Tên thương mại trùng với hoạt chất thì hiển thị là: Hoạt chất_Hàm lượng
                    //Ví dụ: Thuốc Paracetamol có hoạt chất Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol 500mg
                    if (tenThuongMai == tenQuocTe)
                    {
                        dp = tenThuongMai + " " + hamLuong;
                    }
                    //1.2 Tên thương mại không trùng với hoạt chất thì hiển thị là: Hoạt chất(Tên thương mại) Hàm lượng
                    //Ví dụ: Thuốc Paradol có hoạt chất là Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol (Paradol) 500mg
                    if (tenThuongMai != tenQuocTe)
                    {
                        dp = tenQuocTe + " " + "<span style='font-weight: bold;'>" + "(" + tenThuongMai + ")" + "</span>" + " " + hamLuong;
                    }
                }
                else
                {
                    dp = tenThuongMai + " " + hamLuong;
                }
            }
            return dp;
        }

        ////  BVHD-3560
        ////        Dược phẩm có 1 hoạt chất:

        ////1.1 Tên thương mại trùng với hoạt chất thì hiển thị là: Hoạt chất_Hàm lượng
        ////Ví dụ: Thuốc Paracetamol có hoạt chất Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol 500mg
        ////1.2 Tên thương mại không trùng với hoạt chất thì hiển thị là: Hoạt chất(Tên thương mại )_Hàm lượng
        ////Ví dụ: Thuốc Paradol có hoạt chất là Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol(Paradol ) 500mg

        ////Dược phẩm có nhiều hoạt chất:

        ////2.1 Thuốc có 2 hoạt chất: cách hiển thị giống như phần 1.2
        ////Ví dụ: Thuốc A có hoạt chất 1 và hoạt chất 2, hàm lượng 500mg thì hiển thị là: Hoạt chất 1 + Hoạt chất 2 (A ) 500mg
        ////2.2 Thuốc có từ 3 hoạt chất trở lên thì hiển thị là: Tên thương mại_Hàm lượng

        ////Dược phẩm được phân loại thuốc hoặc hoạt chất là

        ////Sinh phẩm hoặc sinh phẩm chẩn đoán trong Danh mục dược phẩm bệnh viện thì hiển thị là: Tên thương mại_Hàm lượng
        //public string FormatTenDuocPham(string tenThuongMai, string tenQuocTe, string hamLuong, long? duocPhamBenhVienPhanNhomId, bool? laPhieuThucHienThuoc = null) // =>  tenThuongMai => tên dược phẩm , tenQuocTe => hoạt chất
        //{
        //    var dp = string.Empty;
        //    // kiểm tra dược phẩm có mấy hoạt chất
        //    var soLuongHoatChat = SoluongHoatChatCuaDuocPham(tenQuocTe);
        //    var listHoatChat = GetListHoatChat(tenQuocTe);
        //    // dược phẩm Phan nhóm == Sinh phẩm hoặc sinh phẩm chẩn đoán
        //    var valueDuocPhamPhanNhom = ValueDuocPhamBenhVien(duocPhamBenhVienPhanNhomId);
        //    if (valueDuocPhamPhanNhom == true)
        //    {
        //        dp = tenThuongMai + " " + hamLuong;
        //    }
        //    else
        //    {
        //        if (soLuongHoatChat == 1)
        //        {
        //            //1.1 Tên thương mại trùng với hoạt chất thì hiển thị là: Hoạt chất_Hàm lượng
        //            //Ví dụ: Thuốc Paracetamol có hoạt chất Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol 500mg
        //            if (tenThuongMai == tenQuocTe)
        //            {
        //                dp = tenThuongMai + " " + hamLuong;
        //            }
        //            //1.2 Tên thương mại không trùng với hoạt chất thì hiển thị là: Hoạt chất(Tên thương mại )_Hàm lượng
        //            //Ví dụ: Thuốc Paradol có hoạt chất là Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol(Paradol ) 500mg
        //            if (tenThuongMai != tenQuocTe)
        //            {
        //                //BVHD-3859
        //                if (laPhieuThucHienThuoc != null && laPhieuThucHienThuoc == true)
        //                {
        //                    dp = tenQuocTe + " " + "<span style='font-weight: bold;'>" + "(" + tenThuongMai + ")" + "</span>" + " " + hamLuong;
        //                }
        //                else
        //                {
        //                    dp = tenQuocTe + " " + "<span style='font-weight: bold;'>" + "(" + tenThuongMai + ")" + "</span>" + " " + hamLuong;
        //                }
        //            }
        //        }
        //        else if (soLuongHoatChat == 2)
        //        {
        //            //2.1 Thuốc có 2 hoạt chất: cách hiển thị giống như phần 1.2
        //            //Ví dụ: Thuốc A có hoạt chất 1 và hoạt chất 2, hàm lượng 500mg thì hiển thị là: Hoạt chất 1 + Hoạt chất 2 (A ) 500mg
        //            if (listHoatChat.Any(d => d == tenThuongMai))
        //            {
        //                dp = tenThuongMai + " " + hamLuong;  // tên thương mại tồn tại trong list hoat chất => 1.1
        //            }
        //            else
        //            {
        //                //BVHD-3859
        //                if (laPhieuThucHienThuoc != null && laPhieuThucHienThuoc == true)
        //                {
        //                    dp = listHoatChat[0] + " + " + listHoatChat[1] + " " + "<span style='font-weight: bold;'>" + "(" + tenThuongMai + ")" + "</span>" + " " + hamLuong;
        //                }
        //                else
        //                {
        //                    dp = listHoatChat[0] + " + " + listHoatChat[1] + " " + "<span style='font-weight: bold;'>" + "(" + tenThuongMai + ")" + "</span>" + " " + hamLuong ;
        //                }
        //            } 
        //        }
        //        else if (soLuongHoatChat > 2)
        //        {
        //            //2.2 Thuốc có từ 3 hoạt chất trở lên thì hiển thị là: Tên thương mại_Hàm lượng
        //            dp = tenThuongMai + " " + hamLuong;
        //        }
        //        else
        //        {
        //            dp = tenThuongMai + " " + hamLuong;
        //        }
        //    }


        //    return dp;
        //}
        public int SoluongHoatChatCuaDuocPham(string hoatChat)
        {
            if (!string.IsNullOrEmpty(hoatChat))
            {
                var slHC = hoatChat.Split('+');
                return slHC.Length;
            }

            return 0;
        }
        public List<string> GetListHoatChat(string hoatChat)
        {
            var lst = new List<string>();
            if (!string.IsNullOrEmpty(hoatChat))
            {
                var slHC = hoatChat.Split('+');
                foreach (var item in slHC)
                {
                    lst.Add(item);
                }
            }
            return lst;
        }
        //BVHD-3959: tối ưu code
        public bool ValueDuocPhamBenhVien(long? duocphamBenhVienPhanNhomId)
        {            
            if (duocphamBenhVienPhanNhomId != null)
            {
                string duocPhamBenhVienPhanNhomSinhPhamIds = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhChung.DuocPhamBenhVienPhanNhomSinhPhamIds").Select(s => s.Value).FirstOrDefault();
                if(!string.IsNullOrEmpty(duocPhamBenhVienPhanNhomSinhPhamIds) && duocPhamBenhVienPhanNhomSinhPhamIds.Split(';').Contains(duocphamBenhVienPhanNhomId.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        //public bool ValueDuocPhamBenhVien(long? duocphamBenhVienPhanNhomId)
        //{
        //    bool value = false;
        //    string sinhPham = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.DuocPhamBenhVienSinhPham").Select(s => s.Value).FirstOrDefault();
        //    string sinhPhamChanDoan = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.DuocPhamBenhVienSinhPhamChanDoan").Select(s => s.Value).FirstOrDefault();
        //    if (duocphamBenhVienPhanNhomId != null)
        //    {
        //        var duocPhamPhanNhom = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.Where(d => d.Id == duocphamBenhVienPhanNhomId && (d.Ten == sinhPham || d.Ten == sinhPhamChanDoan));
        //        if (duocPhamPhanNhom.Any())
        //        {
        //            value = true;
        //        }
        //    }
        //    return value;
        //}
        //        Với số lượng thuốc được kê nhỏ hơn 10 (<10) , hệ thống hiển thị thêm số 0 đằng trước

        //VD: số lượng thuốc là 5 -> Hiển thị ttrong đơn là 05

        //Đối với thuốc gây nghiện: số lượng thuốc phải viết bằng chữ, chữ đầu viết hoa

        //VD: số lượng thuốc 10 ->Hiển thị: Mười
        // format số lượng 
        public string FormatSoLuong(double soLuong, Enums.LoaiThuocTheoQuanLy? loaiThuoc)
        {
            var slDP = string.Empty;

            if (soLuong == 0)
            {
                if (loaiThuoc != Enums.LoaiThuocTheoQuanLy.GayNghien && loaiThuoc != Enums.LoaiThuocTheoQuanLy.HuongThan)
                {
                    slDP = "0";
                }
                else
                {
                    slDP = NumberHelper.ChuyenSoRaText(soLuong, false);
                }
            }
            else if (soLuong < 10 && soLuong > 0)
            {
                if (loaiThuoc != Enums.LoaiThuocTheoQuanLy.GayNghien && loaiThuoc != Enums.LoaiThuocTheoQuanLy.HuongThan)
                {
                    slDP = "0" + soLuong;
                }
                else
                {
                    slDP = NumberHelper.ChuyenSoRaText(soLuong, false);
                }

            }
            else if (soLuong >= 10) //&& (loaiThuoc == null || (loaiThuoc != Enums.LoaiThuocTheoQuanLy.GayNghien && loaiThuoc != Enums.LoaiThuocTheoQuanLy.HuongThan)))
            {
                if (loaiThuoc == Enums.LoaiThuocTheoQuanLy.GayNghien || loaiThuoc == Enums.LoaiThuocTheoQuanLy.HuongThan)
                {
                    slDP = NumberHelper.ChuyenSoRaText(soLuong, false);
                }
                else
                {
                    slDP = "" + soLuong;
                }
            }
            return slDP;
        }

        public string GetPhuongPhapKyThuatDieuTri(long yeuCauKhamBenhId)
        {
            var yeuCauKhamBenh = BaseRepository.GetById(yeuCauKhamBenhId, s => s.Include(kb => kb.YeuCauDuocPhamBenhViens)
                                                                                .Include(kb => kb.YeuCauVatTuBenhViens)
                                                                                .Include(kb => kb.YeuCauDichVuKyThuats).ThenInclude(kt => kt.DichVuKyThuatBenhVien));
            var yeuCauDuocPhamBenhViens = yeuCauKhamBenh.YeuCauDuocPhamBenhViens
                                          .Where(dp => dp.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                                          .Select(dp => dp.Ten).Distinct().ToList();

            var yeuCauVatTuBenhViens = yeuCauKhamBenh.YeuCauVatTuBenhViens.Where(z => z.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).Select(z => z.Ten).Distinct().ToList();
            var yeuCauDichVuKyThuats = yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(z => z.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                            && !string.IsNullOrEmpty(z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat)
                                                                            && z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat.Substring(0, 1).ToLower() != "p").Select(z => z.TenDichVu).Distinct().ToList();
            return string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats);
        }

        #region BVHD-3575
        public async Task<bool> KiemTraChiDinhDichVuKhamBenhDaCoTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, long dichVuKhamBenhVienId, long noiTruPhieuDieuTriId)
        {
            var yeuCauTiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .First(x => x.Id == yeuCauTiepNhanId);

            var yeuCauTiepNhanNgoaiTruId = yeuCauTiepNhanNoiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId;

            // trường hợp là bệnh án con, ko có YCTN ngoại trú thì khi chỉ định dv khám sẽ kiểm tra và tạo mới YCTN ngoại trú tương ứng
            if (yeuCauTiepNhanNgoaiTruId == null)
            {
                return false;
            }

            DateTime? ngayDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking.FirstOrDefault(p => p.Id == noiTruPhieuDieuTriId)?.NgayDieuTri.Date;
            var kiemTra = await BaseRepository.TableNoTracking
                .AnyAsync(x => x.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruId
                               && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                               && x.DichVuKhamBenhBenhVienId == dichVuKhamBenhVienId
                               && x.ThoiDiemDangKy.Date == ngayDieuTri.Value
                               && x.LaChiDinhTuNoiTru != null
                               && x.LaChiDinhTuNoiTru == true
                );

            return kiemTra;
        }


        #endregion
    }
}
