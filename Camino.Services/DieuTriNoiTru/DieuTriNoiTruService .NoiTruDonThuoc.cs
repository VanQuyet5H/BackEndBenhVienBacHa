using Camino.Core.Domain.ValueObject;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Internal;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using System;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public GridDataSource GetDataForGridDanhSachNoiTruDonThuoc(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var lstDuocPham = _noiTruDonThuocChiTietRepository.TableNoTracking
                             .Where(o => o.NoiTruDonThuoc.YeuCauTiepNhanId == yeuCauTiepNhanId)
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

            var query = _noiTruDonThuocChiTietRepository.TableNoTracking
                .Where(o => o.NoiTruDonThuoc.YeuCauTiepNhanId == yeuCauTiepNhanId && o.SoLuong > 0)
                .OrderBy(o => o.NoiTruDonThuoc.LoaiDonThuoc)
                .Select(s => new NoiTruDonThuocChiTietGridVoItem
                {
                    DonThuocThanhToanChiTietIds = s.DonThuocThanhToanChiTiets.Select(o => o.Id).ToList(),
                    STT = s.SoThuTu,
                    Id = s.Id,
                    DuocPhamId = s.DuocPhamId,
                    YeuCauTiepNhanId = s.NoiTruDonThuoc.YeuCauTiepNhanId,
                    NoiTruDonThuocId = s.NoiTruDonThuocId,
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
                    //DonGiaNhap = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().DonGiaNhap : 0,
                    //DonGia = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().DonGiaBan : 0,
                    //TiLeTheoThapGia = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().TiLeTheoThapGia : 0,
                    //VAT = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().VAT : 0,
                    LoaiDonThuoc = s.NoiTruDonThuoc.LoaiDonThuoc,
                    ThuocBHYT = s.NoiTruDonThuoc.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT ? "Có" : "Không",
                    //DiUngThuocDisplay = s.NoiTruDonThuoc.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng == s.DuocPham.MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không",
                    //TuongTacThuoc = GetTuongTac(s.DuocPham.MaHoatChat, lstDuocPham, lstADR),
                    GhiChu = s.GhiChu,
                    GhiChuDonThuoc = s.NoiTruDonThuoc.GhiChu,
                    NhomId = s.NoiTruDonThuoc.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT ? 1 : 0,  //  1 thuoc BHYT , 0 : Khong BHYT,
                    Nhom = s.NoiTruDonThuoc.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                    //ChuaThanhToan = s.LaDuocPhamBenhVien && s.DonThuocThanhToanChiTiets.FirstOrDefault() != null ? s.DonThuocThanhToanChiTiets.FirstOrDefault().DonThuocThanhToan.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan : true,
                    CheckBox = true,

                    //BVHD-3905
                    TiLeThanhToanBHYT = s.DuocPham.DuocPhamBenhVien.TiLeThanhToanBHYT
                });

            var lstQuery = query.ToList();

            var benhNhanDiUngThuocs = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId).Select(o => o.BenhNhan.BenhNhanDiUngThuocs).FirstOrDefault();

            var donThuocThanhToanChiTietIds = lstQuery.SelectMany(o => o.DonThuocThanhToanChiTietIds).ToList();
            var donThuocThanhToanChiTietDatas = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => donThuocThanhToanChiTietIds.Contains(o.Id))
                .Select(o => new DonThuocThanhToanChiTietData
                {
                    Id = o.Id,
                    DonGiaNhap = o.DonGiaNhap,
                    DonGiaBan = o.DonGiaBan,
                    TiLeTheoThapGia = o.TiLeTheoThapGia,
                    VAT = o.VAT,
                    TrangThaiThanhToan = o.DonThuocThanhToan.TrangThaiThanhToan
                }).ToList();

            for (int i = 0; i < lstQuery.Count(); i++)
            {
                var donThuocThanhToanChiTiet = donThuocThanhToanChiTietDatas.FirstOrDefault(o => lstQuery[i].DonThuocThanhToanChiTietIds.Contains(o.Id));
                if (donThuocThanhToanChiTiet != null && lstQuery[i].LaDuocPhamBenhVien == true)
                {
                    lstQuery[i].DonGiaNhap = donThuocThanhToanChiTiet.DonGiaNhap;
                    lstQuery[i].DonGia = donThuocThanhToanChiTiet.DonGiaBan;
                    lstQuery[i].TiLeTheoThapGia = donThuocThanhToanChiTiet.TiLeTheoThapGia;
                    lstQuery[i].VAT = donThuocThanhToanChiTiet.VAT;
                    lstQuery[i].ChuaThanhToan = donThuocThanhToanChiTiet.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan;
                }
                else
                {
                    lstQuery[i].ChuaThanhToan = true;
                }
                lstQuery[i].DiUngThuocDisplay = benhNhanDiUngThuocs.Any(diung => diung.TenDiUng == lstQuery[i].MaHoatChat && diung.LoaiDiUng == LoaiDiUng.Thuoc) ? "Có" : "Không";
                lstQuery[i].TuongTacThuoc = GetTuongTac(lstQuery[i].MaHoatChat, lstDuocPham, lstADR);
                
            }

            //var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = lstQuery.OrderBy(o => o.STT).ToArray(), TotalRowCount = lstQuery.Count };
        }
        public GridDataSource GetTotalPageForGridDanhSachNoiTruDonThuoc(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<string> ThemNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietVo donThuocChiTiet)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            YeuCauTiepNhan ycTiepNhan;
            if (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT)
            {
                ycTiepNhan = BaseRepository.GetById(donThuocChiTiet.YeuCauTiepNhanId, x => x.Include(o => o.NoiTruDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans)
                    //.Include(dt => dt.YeuCauKhamBenhs)
                    //.Include(dt => dt.YeuCauDichVuKyThuats)
                    //.Include(dt => dt.YeuCauDichVuGiuongBenhViens)
                    //.Include(dt => dt.YeuCauDuocPhamBenhViens)
                    //.Include(dt => dt.YeuCauVatTuBenhViens)
                    .Include(dt => dt.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets)
                    //.Include(o => o.YeuCauKhamBenhLichSuTrangThais)
                    );
            }
            else
            {
                ycTiepNhan = BaseRepository.GetById(donThuocChiTiet.YeuCauTiepNhanId, x =>
                    x.Include(o => o.NoiTruDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans));
            }


            var donThuoc = ycTiepNhan.NoiTruDonThuocs.FirstOrDefault(o => o.LoaiDonThuoc == donThuocChiTiet.LoaiDonThuoc);
            if (donThuoc == null)
            {
                donThuoc = new NoiTruDonThuoc
                {
                    TrangThai = EnumTrangThaiDonThuoc.ChuaCapThuoc,
                    LoaiDonThuoc = donThuocChiTiet.LoaiDonThuoc,
                    ThoiDiemKeDon = DateTime.Now,
                    BacSiKeDonId = _userAgentHelper.GetCurrentUserId(),
                    NoiKeDonId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                ycTiepNhan.NoiTruDonThuocs.Add(donThuoc);
            }

            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamId,
                x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));
            var ntDonThuocChiTiet =
                new NoiTruDonThuocChiTiet
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
                    ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang,
                    ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua,
                    ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu,
                    ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi,
                    DuocHuongBaoHiem = donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT,
                    BenhNhanMuaNgoai = donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocNgoaiBenhVien,
                    GhiChu = donThuocChiTiet.GhiChu
                };
            donThuoc.NoiTruDonThuocChiTiets.Add(ntDonThuocChiTiet);

            if (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT || donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien)
            {
                if (duocPham.DuocPhamBenhVien == null ||
                    (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT && duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => (o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoThuocBHYT) && o.LaDuocPhamBHYT && o.HanSuDung >= DateTime.Now && o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < donThuocChiTiet.SoLuong) ||
                    (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien && duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc && !o.LaDuocPhamBHYT && o.HanSuDung >= DateTime.Now && o.SoLuongNhap > o.SoLuongDaXuat).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < donThuocChiTiet.SoLuong))
                {
                    //return "Dược phẩm không có trong kho hoặc số lượng tồn không đủ";
                    return GetResourceValueByResourceName("DonThuoc.ThuocKhongCoTrongKhoHoacSoLuongKhongDu");
                }
                //ktra don thuoc thanh toan
                var donThuocThanhToan = donThuoc.DonThuocThanhToans.FirstOrDefault(o => o.LoaiDonThuoc == donThuocChiTiet.LoaiDonThuoc && o.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan);
                if (donThuocThanhToan == null)
                {
                    donThuocThanhToan = new DonThuocThanhToan
                    {
                        LoaiDonThuoc = donThuocChiTiet.LoaiDonThuoc,
                        YeuCauTiepNhanId = ycTiepNhan.Id,
                        BenhNhanId = ycTiepNhan.BenhNhanId,
                        TrangThai = TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan
                    };
                    donThuoc.DonThuocThanhToans.Add(donThuocThanhToan);
                    ycTiepNhan.DonThuocThanhToans.Add(donThuocThanhToan);
                }
                //them don thuoc thanh toan chi tiet
                double soLuongCanXuat = donThuocChiTiet.SoLuong;
                while (!soLuongCanXuat.Equals(0))
                {
                    // tinh so luong xuat
                    var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                        .Where(o => ((donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT && (o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoThuocBHYT) && o.LaDuocPhamBHYT) ||
                                    (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBenhVien && o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc && !o.LaDuocPhamBHYT)) && o.HanSuDung >= DateTime.Now
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
                        NoiTruDonThuocChiTiet = ntDonThuocChiTiet,
                        XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
                        Ten = duocPham.Ten,
                        TenTiengAnh = duocPham.TenTiengAnh,
                        SoDangKy = duocPham.SoDangKy,
                        STTHoatChat = duocPham.STTHoatChat,
                        NhomChiPhi = donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT ? EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT : EnumDanhMucNhomTheoChiPhi.ThuocThanhToanTheoTyLe,
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
            //bo duyet tu dong
            //if (donThuocChiTiet.LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT)
            //{
            //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
            //    if (cauHinh.DuyetBHYTTuDong)
            //    {
            //        DuyetBHYTChoDonThuoc(ycTiepNhan, (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)PhongHeThong.PhongDuyetBHYTToanTuDong);
            //    }
            //}

            //await BaseRepository.UpdateAsync(ycTiepNhan);
            BaseRepository.Context.SaveChanges();
            await XuLySoThuTuNoiTruDonThuocRaVien(ycTiepNhan.Id);
            return string.Empty;
        }
        public async Task<string> CapNhatNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietVo donThuocChiTiet)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var ntDonThuocChiTiet = _noiTruDonThuocChiTietRepository.GetById(donThuocChiTiet.NoiTruDonThuocChiTietId,
                x => x.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan)
                    .Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(xk => xk.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(xk => xk.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.DonThuocThanhToan)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.DuyetBaoHiemChiTiets)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(tt => tt.TaiKhoanBenhNhanChis)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauKhamBenhs)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuKyThuats)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDichVuGiuongBenhViens)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauDuocPhamBenhViens)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.YeuCauVatTuBenhViens)
                    );

            if (ntDonThuocChiTiet == null)
            {
                return GetResourceValueByResourceName("DonThuoc.NotExists");
            }
            //kiem tra truoc khi cap nhat
            if (ntDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan))
            {
                //return "Dược phẩm đã được thanh toán";
                return GetResourceValueByResourceName("DonThuoc.ThuocDaThanhToan");
            }
            if (ntDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc))
            {
                return GetResourceValueByResourceName("DonThuoc.ThuocDaXuat");
            }
            var soLuongTruoc = ntDonThuocChiTiet.SoLuong;
            //var duocHuongBaoHiem = ntDonThuocChiTiet.DuocHuongBaoHiem;

            var duocPham = _duocPhamRepository.GetById(donThuocChiTiet.DuocPhamId,
                x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams).Include(o => o.HopDongThauDuocPhamChiTiets)
                    .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

            ntDonThuocChiTiet.DuocPhamId = duocPham.Id;
            ntDonThuocChiTiet.Ten = duocPham.Ten;
            ntDonThuocChiTiet.TenTiengAnh = duocPham.TenTiengAnh;
            ntDonThuocChiTiet.SoDangKy = duocPham.SoDangKy;
            ntDonThuocChiTiet.StthoatChat = duocPham.STTHoatChat;
            ntDonThuocChiTiet.MaHoatChat = duocPham.MaHoatChat;
            ntDonThuocChiTiet.HoatChat = duocPham.HoatChat;
            ntDonThuocChiTiet.LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat;
            ntDonThuocChiTiet.NhaSanXuat = duocPham.NhaSanXuat;
            ntDonThuocChiTiet.NuocSanXuat = duocPham.NuocSanXuat;
            ntDonThuocChiTiet.DuongDungId = duocPham.DuongDungId;
            ntDonThuocChiTiet.HamLuong = duocPham.HamLuong;
            ntDonThuocChiTiet.QuyCach = duocPham.QuyCach;
            ntDonThuocChiTiet.TieuChuan = duocPham.TieuChuan;
            ntDonThuocChiTiet.DangBaoChe = duocPham.DangBaoChe;
            ntDonThuocChiTiet.DonViTinhId = duocPham.DonViTinhId;
            ntDonThuocChiTiet.HuongDan = duocPham.HuongDan;
            ntDonThuocChiTiet.MoTa = duocPham.MoTa;
            ntDonThuocChiTiet.ChiDinh = duocPham.ChiDinh;
            ntDonThuocChiTiet.ChongChiDinh = duocPham.ChongChiDinh;
            ntDonThuocChiTiet.LieuLuongCachDung = duocPham.LieuLuongCachDung;
            ntDonThuocChiTiet.TacDungPhu = duocPham.TacDungPhu;
            ntDonThuocChiTiet.ChuYdePhong = duocPham.ChuYDePhong;
            ntDonThuocChiTiet.SoLuong = donThuocChiTiet.SoLuong;
            ntDonThuocChiTiet.SoNgayDung = donThuocChiTiet.SoNgayDung;
            ntDonThuocChiTiet.DungSang = donThuocChiTiet.DungSang;
            ntDonThuocChiTiet.DungTrua = donThuocChiTiet.DungTrua;
            ntDonThuocChiTiet.DungChieu = donThuocChiTiet.DungChieu;
            ntDonThuocChiTiet.DungToi = donThuocChiTiet.DungToi;
            ntDonThuocChiTiet.ThoiGianDungSang = donThuocChiTiet.ThoiGianDungSang;
            ntDonThuocChiTiet.ThoiGianDungTrua = donThuocChiTiet.ThoiGianDungTrua;
            ntDonThuocChiTiet.ThoiGianDungChieu = donThuocChiTiet.ThoiGianDungChieu;
            ntDonThuocChiTiet.ThoiGianDungToi = donThuocChiTiet.ThoiGianDungToi;
            ntDonThuocChiTiet.GhiChu = donThuocChiTiet.GhiChu;


            if (ntDonThuocChiTiet.LaDuocPhamBenhVien)
            {
                if (!soLuongTruoc.AlmostEqual(donThuocChiTiet.SoLuong))
                {
                    var donThuocThanhToanChiTietLast = ntDonThuocChiTiet.DonThuocThanhToanChiTiets.Last();
                    if (soLuongTruoc < donThuocChiTiet.SoLuong)
                    {
                        var soLuongTang = donThuocChiTiet.SoLuong - soLuongTruoc;
                        if (duocPham.DuocPhamBenhVien == null ||
                            duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                .Where(o => (ntDonThuocChiTiet.DuocHuongBaoHiem == o.LaDuocPhamBHYT && o.HanSuDung >= DateTime.Now &&
                                             (o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoThuocBHYT)))
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
                                        (ntDonThuocChiTiet.DuocHuongBaoHiem == o.LaDuocPhamBHYT &&
                                         (o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc || o.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoThuocBHYT)) && o.HanSuDung >= DateTime.Now &&
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
                                    NoiTruDonThuocChiTiet = ntDonThuocChiTiet,
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
                                ntDonThuocChiTiet.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                                soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                            }
                        }
                    }
                    else//Giam so luong thuoc
                    {
                        var soLuongGiam = soLuongTruoc - donThuocChiTiet.SoLuong;
                        var donThuocThanhToanChiTiets = ntDonThuocChiTiet.DonThuocThanhToanChiTiets.OrderByDescending(o => o.Id).ToList();
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
                    //        DuyetBHYTChoDonThuoc(ntDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhan, (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)PhongHeThong.PhongDuyetBHYTToanTuDong);
                    //    }
                    //}
                }
            }
            await _noiTruDonThuocChiTietRepository.UpdateAsync(ntDonThuocChiTiet);
            return string.Empty;
        }
        public async Task<string> XoaNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietVo donThuocChiTiet)
        {
            var ntDonThuocChiTiet = _noiTruDonThuocChiTietRepository.GetById(donThuocChiTiet.NoiTruDonThuocChiTietId,
                x => x
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauKhamBenhs)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDichVuKyThuats)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDichVuGiuongBenhViens)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDuocPhamBenhViens)
                    //.Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauVatTuBenhViens)
                    .Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets)
                    .Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.YeuCauTiepNhan).ThenInclude(dt => dt.NoiTruDonThuocs)
                    .Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.DonThuocThanhToans)
                    .Include(o => o.NoiTruDonThuoc).ThenInclude(dt => dt.NoiTruDonThuocChiTiets)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuyetBaoHiemChiTiets)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                    //.Include(o => o.YeuCauKhamBenhDonThuoc).ThenInclude(dt => dt.YeuCauKhamBenh).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTs)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonThuocThanhToan).ThenInclude(o => o.DonThuocThanhToanChiTiets)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                    .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet));
            //ntDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.LastTime = DateTime.Now;
            if (ntDonThuocChiTiet == null)
            {
                return GetResourceValueByResourceName("DonThuoc.NotExists");
            }
            //kiem tra truoc khi cap nhat
            if (ntDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan))
            {
                //return "Dược phẩm đã được thanh toán";
                return GetResourceValueByResourceName("DonThuoc.ThuocDaThanhToan");
            }
            if (ntDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc))
            {
                return GetResourceValueByResourceName("DonThuoc.ThuocDaXuat");
            }
            var duocHuongBaoHiem = false;
            if (ntDonThuocChiTiet.DonThuocThanhToanChiTiets.Any(o => o.BaoHiemChiTra != false))
            {
                duocHuongBaoHiem = true;

            }
            //hoan lai duoc pham da book trong kho
            foreach (var donThuocThanhToanChiTiet in ntDonThuocChiTiet.DonThuocThanhToanChiTiets)
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
            ntDonThuocChiTiet.WillDelete = true;
            if (ntDonThuocChiTiet.NoiTruDonThuoc.NoiTruDonThuocChiTiets.All(o => o.WillDelete))
            {
                ntDonThuocChiTiet.NoiTruDonThuoc.WillDelete = true;
            }
            //if (duocHuongBaoHiem)
            //{
            //    DuyetBHYTChoDonThuoc(ntDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhan, (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)PhongHeThong.PhongDuyetBHYTToanTuDong);
            //}
            //if (ntDonThuocChiTiet.NoiTruDonThuoc.NoiTruDonThuocChiTiets.All(o => o.WillDelete) 
            //    //&& !ntDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any()
            //    )
            //{
            //    ntDonThuocChiTiet.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.CoKeToa = null;
            //}

            await _noiTruDonThuocChiTietRepository.UpdateAsync(ntDonThuocChiTiet);
            await XuLySoThuTuNoiTruDonThuocRaVien(ntDonThuocChiTiet.NoiTruDonThuoc.YeuCauTiepNhanId);
            return string.Empty;
        }
        //bo duyet tu dong
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
        //                    p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList();
        //    foreach (var yeuCauKhamBenh in dsDichVuKhamBenh)
        //    {
        //        if (yeuCauKhamBenh.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
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
        //            if (yeuCauKhamBenh.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
        //            {
        //                yeuCauKhamBenh.TrangThaiThanhToan = TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }
        //    foreach (var yckt in ycTiepNhan.YeuCauDichVuKyThuats
        //        .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
        //    {
        //        if (yckt.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
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
        //            if (yckt.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
        //            {
        //                yckt.TrangThaiThanhToan = TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycdp in ycTiepNhan.YeuCauDuocPhamBenhViens
        //                .Where(p => p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy))
        //    {
        //        if (ycdp.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
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
        //            if (ycdp.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycdp.TrangThaiThanhToan = TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycvt in ycTiepNhan.YeuCauVatTuBenhViens
        //        .Where(p => p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy))
        //    {
        //        if (ycvt.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
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
        //            if (ycvt.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycvt.TrangThaiThanhToan = TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycdt in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != TrangThaiDonThuocThanhToan.DaHuy)
        //                .SelectMany(w => w.DonThuocThanhToanChiTiets)
        //                .Where(p => !p.WillDelete && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false))
        //    {
        //        if (ycdt.DonThuocThanhToan?.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
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
        //            if (ycdt.DonThuocThanhToan?.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycdt.DonThuocThanhToan.TrangThaiThanhToan = TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    if (duyetBaoHiem.DuyetBaoHiemChiTiets.Any())
        //    {
        //        ycTiepNhan.DuyetBaoHiems.Add(duyetBaoHiem);
        //    }
        //}

        public async Task<string> XuLySoThuTuNoiTruDonThuocRaVien(long yeuCauTiepNhanId)
        {
            var STT = 1;
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId, s => s.Include(z => z.NoiTruDonThuocs).ThenInclude(z => z.NoiTruDonThuocChiTiets).ThenInclude(z => z.DuocPham).ThenInclude(z => z.DuocPhamBenhVien));
            var listThuocChiTiet = yeuCauTiepNhan.NoiTruDonThuocs.Where(z => z.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT).SelectMany(z => z.NoiTruDonThuocChiTiets)
                                   .Union(yeuCauTiepNhan.NoiTruDonThuocs.Where(z => z.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).SelectMany(z => z.NoiTruDonThuocChiTiets));

            //BVHD-3959
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
            BaseRepository.Context.SaveChanges();
            return string.Empty;

            //if (listThuocChiTiet.Any(z => z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc))
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}
            //if (listThuocChiTiet.Any(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)))//Tiêm 
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}

            //if (listThuocChiTiet.Any(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)))//Uống
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}

            //if (listThuocChiTiet.Any(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)))//Đặt z.DuocPham.DuongDung.Ma.Trim() == "4.04".Trim()
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}

            //if (listThuocChiTiet.Any(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)))//Dùng ngoài  z.DuocPham.DuongDung.Ma.Trim() == "3.05".Trim()
            //{
            //    foreach (var item in listThuocChiTiet.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai && !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)).OrderBy(z => z.CreatedOn))
            //    {
            //        item.SoThuTu = STT;
            //        STT++;
            //    }
            //}
            //foreach (var item in listThuocChiTiet.Where(z => !(z.DuocPham.DuocPhamBenhVien != null && z.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)
            //                                                                     && z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                     && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                     && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                     && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai).OrderBy(z => z.CreatedOn))
            //{
            //    item.SoThuTu = STT;
            //    STT++;
            //}

            //BaseRepository.Context.SaveChanges();
            //return string.Empty;
        }

        public GetDuocPhamTonKhoGridVoItem GetNoiTruDuocPhamInfoById(ThongTinThuocNoiTruVo thongTinThuocVo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var yeuCauTiepNhan = BaseRepository.GetById(thongTinThuocVo.YeuCauTiepNhanId, x => x.Include(o => o.NoiTruDonThuocs).ThenInclude(dt => dt.NoiTruDonThuocChiTiets)
                                                                                .Include(yc => yc.BenhNhan).ThenInclude(bn => bn.BenhNhanDiUngThuocs));

            var lstDuocPham = yeuCauTiepNhan.NoiTruDonThuocs.SelectMany(o => o.NoiTruDonThuocChiTiets)
                .Select(o => o.MaHoatChat).ToList();
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
                .Where(o => o.Id == thongTinThuocVo.DuocPhamId)
                .Select(d => new GetDuocPhamTonKhoGridVoItem
                {
                    Id = d.Id,
                    TuongTacThuoc = GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR) == string.Empty ? "Không" : GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR),
                    FlagTuongTac = !GetTuongTac(d.MaHoatChat, lstDuocPham, lstADR).Contains("Không") ? true : false,
                    FlagDiUng = d.HoatChat != null && yeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Any(diung => diung.TenDiUng.Contains(d.HoatChat) && diung.LoaiDiUng == LoaiDiUng.Thuoc),
                    FlagThuocDaKe = yeuCauTiepNhan.NoiTruDonThuocs.SelectMany(dt => dt.NoiTruDonThuocChiTiets).Any(dtct => dtct.MaHoatChat == d.MaHoatChat),
                    DVTId = d.DonViTinh.Id,
                    TenDonViTinh = d.DonViTinh.Ten,
                    DuongDungId = d.DuongDung.Id,
                    TenDuongDung = d.DuongDung.Ten,
                    TonKho = d.DuocPhamBenhVien == null || !(thongTinThuocVo.LoaiDuocPham == 1 || thongTinThuocVo.LoaiDuocPham == 2) ? 0 :
                                (d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => (nkct.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc || nkct.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoThuocBHYT) && nkct.LaDuocPhamBHYT == (thongTinThuocVo.LoaiDuocPham == 1) &&
                                                                                          nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat)).MathRoundNumber(1),
                    HanSuDung = d.DuocPhamBenhVien == null || !(thongTinThuocVo.LoaiDuocPham == 1 || thongTinThuocVo.LoaiDuocPham == 2) ? (DateTime?)null :
                                (d.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
                                        .Where(nkct => (nkct.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc || nkct.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoThuocBHYT) && nkct.LaDuocPhamBHYT == (thongTinThuocVo.LoaiDuocPham == 1) && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                        .Select(o => o.HanSuDung).FirstOrDefault()),
                    MucDo = yeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs.Where(p => lstThuocHoacHoatChat.Contains(p.TenDiUng)).Select(p => p.MucDo).FirstOrDefault(),
                    HamLuong = d.HamLuong,
                    NhaSanXuat = d.NhaSanXuat,
                    DuongDung = d.DuongDung.Ten
                }).FirstOrDefault();
            return duocPhamInfo;
        }
        public string InDonThuocRaVien(InToaThuocRaVien inToaThuoc)
        {
            var noiTruDonThuocs = _noiTruDonThuocRepository.TableNoTracking.Where(z => z.YeuCauTiepNhanId == inToaThuoc.YeuCauTiepNhanId).ToList();
            foreach (var item in noiTruDonThuocs)
            {
                item.GhiChu = inToaThuoc.GhiChu;
            }
            _noiTruDonThuocRepository.Update(noiTruDonThuocs);

            //Cập nhật 23/06/2022: lời dặn cho phép xuống dòng khi in
            if (!string.IsNullOrEmpty(inToaThuoc.GhiChu))
            {
                inToaThuoc.GhiChu = inToaThuoc.GhiChu.Replace("\n", "<br>");
            }
            //=========================

            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();
            var infoBN = ThongTinBenhNhanPhieuThuoc(inToaThuoc.YeuCauTiepNhanId);
            infoBN.LoiDan= inToaThuoc.GhiChu;
            var thongRaVien = new InNoiTruPhieuDieuTriThuocRaVienICDKemTheo();
            var noiTruBenhAn = _noiTruBenhAnRepository.GetById(inToaThuoc.YeuCauTiepNhanId, s => s.Include(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruThamKhamChanDoanKemTheos).ThenInclude(z => z.ICD)
                                                                                                  .Include(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.ChanDoanChinhICD));
            var icdKemTheos = new List<string>();
            if (noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong)
            {
                var noiTruPhieuDieuTriLast = noiTruBenhAn.NoiTruPhieuDieuTris.OrderByDescending(z => z.NgayDieuTri).FirstOrDefault();
                if (noiTruPhieuDieuTriLast != null)
                {
                    infoBN.ChuanDoan = noiTruPhieuDieuTriLast.ChanDoanChinhICD != null ?
                                               (noiTruPhieuDieuTriLast.ChanDoanChinhICD.Ma + " - " + noiTruPhieuDieuTriLast.ChanDoanChinhICD.TenTiengViet
                                                   + (!string.IsNullOrEmpty(noiTruPhieuDieuTriLast.ChanDoanChinhGhiChu) ? " (" + noiTruPhieuDieuTriLast.ChanDoanChinhGhiChu + ")" : "")) : "";
                    if (noiTruPhieuDieuTriLast.NoiTruThamKhamChanDoanKemTheos.Any())
                    {
                        foreach (var chanDoanKemTheo in noiTruPhieuDieuTriLast.NoiTruThamKhamChanDoanKemTheos)
                        {
                            var icdKemTheo = chanDoanKemTheo.ICD.Ma + " - " + chanDoanKemTheo.ICD.TenTiengViet + (!string.IsNullOrEmpty(chanDoanKemTheo.GhiChu) ? " (" + chanDoanKemTheo.GhiChu + ")" : "");

                            icdKemTheos.Add(icdKemTheo);
                        }
                        if (icdKemTheos.Any())
                        {

                            infoBN.ChuanDoan += "; " + string.Join("; ", icdKemTheos);
                        }
                    }
                }
                else
                {
                    infoBN.ChuanDoan = "";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinRaVien))
                {
                    thongRaVien = JsonConvert.DeserializeObject<InNoiTruPhieuDieuTriThuocRaVienICDKemTheo>(noiTruBenhAn.ThongTinRaVien);
                    if (thongRaVien != null && thongRaVien.ChuanDoanKemTheos != null && thongRaVien.ChuanDoanKemTheos.Any())
                    {
                        foreach (var chanDoanKemTheo in thongRaVien.ChuanDoanKemTheos)
                        {
                            var icdKemTheo = chanDoanKemTheo.TenICD + (!string.IsNullOrEmpty(chanDoanKemTheo.ChuanDoan) ? " (" + chanDoanKemTheo.ChuanDoan + ")" : "");
                            icdKemTheos.Add(icdKemTheo);
                        }
                    }
                }
                if (icdKemTheos.Any())
                {
                    infoBN.ChuanDoan += "; " + string.Join("; ", icdKemTheos);
                }
            }

            var templateDonThuocBHYT = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYTTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYT")).First();
            var templateDonThuocTrongBenhVien = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYTTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYT")).First();
            var templateDonThuocThucPhamChucNang = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocThucPhamChucNang")).FirstOrDefault();
            var templateDonThuocNgoaiBenhVien = infoBN.LaTreEm == true ? _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBVTreEm")).First() : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBV")).First();

            //var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            //var khoaPhong = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == noiLamViecCurrentId).Select(p => p.KhoaPhong.Ten).FirstOrDefault();

            #region GhiChu
            //DP thuộc nhóm Tân dược, Sinh phẩm, Sinh phẩm chẩn đoán->ĐƠN THUỐC
            //DP thuộc nhóm thuốc BHYT -> ĐƠN THUỐC(BHYT)
            //DP thuộc nhóm Thực phẩm chức năng, Mỹ phẩm, Vật tư y tế, Thiết bị y tế, Thuốc từ dược liệu->ĐƠN TƯ VẤN
            //DP thuộc thuốc ngoài bệnh viện -> ĐƠN THUỐC(NGOÀI BỆNH VIỆN)
            #endregion


            var donThuocBHYTChiTiets = _noiTruDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.NoiTruDonThuoc.YeuCauTiepNhanId == inToaThuoc.YeuCauTiepNhanId && z.NoiTruDonThuoc.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                                && inToaThuoc.Ids.Contains(z.Id))
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
                                    ThoiDiemKeDon = s.NoiTruDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.NoiTruDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.NoiTruDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.NoiTruDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).OrderBy(p => p.STT ?? 0).ToList();
            var donThuocBHYTs = donThuocBHYTChiTiets;

            var donThuocKhongBHYTChiTiets = _noiTruDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.NoiTruDonThuoc.YeuCauTiepNhanId == inToaThuoc.YeuCauTiepNhanId && z.NoiTruDonThuoc.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT
                                && inToaThuoc.Ids.Contains(z.Id))

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
                                    ThoiDiemKeDon = s.NoiTruDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.NoiTruDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.NoiTruDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.NoiTruDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).ToList();
            foreach (var thuoc in donThuocKhongBHYTChiTiets)
            {
                thuoc.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(thuoc.DuocPhamBenhVienPhanNhomId.GetValueOrDefault(), duocPhamBenhVienPhanNhoms);
            }

            var userCurrentId = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.First().BacSiKeDonId : (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.First().BacSiKeDonId : 0);

            var tenBacSiKeDon = _useRepository.TableNoTracking
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
                                 "<th>THỰC PHẨM CHỨC NĂNG</ th>" +
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
                    resultThuocBHYT += "<td >" + _yeuCauKhamBenhService.FormatTenDuocPham(donThuocBHYTChiTiet.Ten, donThuocBHYTChiTiet.HoatChat, donThuocBHYTChiTiet.HamLuong, donThuocBHYTChiTiet.DuocPhamBenhVienPhanNhomId)
                        + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                        + "</td>";

                    resultThuocBHYT += "<td  style='vertical-align: top;text-align: center' >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocBHYTChiTiet.SoLuong, donThuocBHYTChiTiet.LoaiThuocTheoQuanLy) + " " + donThuocBHYTChiTiet.DVT + "</td>";
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
                        ChuanDoan = infoBN?.ChuanDoan,
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
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";


                        }
                        else
                        {
                            sttKhongBHYTTrongBV++;
                            resultThuocTrongBenhVien += "<tr>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTTrongBV + "</td>";
                            resultThuocTrongBenhVien += "<td >" + _yeuCauKhamBenhService.FormatTenDuocPham(donThuocTrongBV.Ten, donThuocTrongBV.HoatChat, donThuocTrongBV.HamLuong, donThuocTrongBV.DuocPhamBenhVienPhanNhomId)
                                 + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center'  >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
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
                            ChuanDoan = infoBN?.ChuanDoan,
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
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";
                        }
                        else
                        {
                            sttKhongBHYTNgoaiBV++;
                            resultThuocNgoaiBenhVien += "<tr>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTNgoaiBV + "</td>";
                            resultThuocNgoaiBenhVien += "<td >" + _yeuCauKhamBenhService.FormatTenDuocPham(donThuocNgoaiBV.Ten, donThuocNgoaiBV.HoatChat, donThuocNgoaiBV.HamLuong, donThuocNgoaiBV.DuocPhamBenhVienPhanNhomId)
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
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
                            ChuanDoan = infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = inToaThuoc.GhiChu,
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
                    ChuanDoan = infoBN?.ChuanDoan,
                    BacSiKham = tenBacSiKeDon,
                    LoiDan = inToaThuoc.GhiChu,
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

            if(content.Length > 30) 
                content = content.Remove(content.Length - 30);
            return content;
        }
    }
}
