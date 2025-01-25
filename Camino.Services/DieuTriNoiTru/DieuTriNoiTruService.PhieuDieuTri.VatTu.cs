using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public GridDataSource GetDataForGridDanhSachVatTu(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId && o.NoiTruPhieuDieuTriId == phieuDieuTriId
                  && o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                          && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                          && o.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                .Select(s => new PhieuDieuTriVatTuGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenDVKT = s.YeuCauDichVuKyThuat.TenDichVu,
                    KhoId = s.KhoLinhId,
                    TenKho = s.KhoLinh.Ten,
                    DVT = s.DonViTinh,
                    SoLuong = s.SoLuong,
                    DonGiaNhap = s.DonGiaNhap,
                    DonGiaBan = s.DonGiaBan,
                    DonGia = s.DonGiaBan,
                    TiLeTheoThapGia = s.TiLeTheoThapGia,
                    VAT = s.VAT,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    TinhTrang = s.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null,
                    CoYeuCauTraVTTuBenhNhanChiTiet = s.YeuCauTraVatTuTuBenhNhanChiTiets.Any(),
                    LaTuTruc = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                    KhongTinhPhi = !s.KhongTinhPhi,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    DonGiaBaoHiem = s.DonGiaBaoHiem,
                    CreatedOn = s.CreatedOn,

                    //BVHD-3905
                    TiLeThanhToanBHYT = s.VatTuBenhVien.TiLeThanhToanBHYT
                });
            var lstQuery = query.GroupBy(x => new { x.Ma, x.Ten, x.TenKho, x.KhongTinhPhi, x.CreatedOn })
                .Select(item
                => new PhieuDieuTriVatTuGridVo()
                {
                    Id = item.First().Id,
                    Ids = string.Join(",", item.Select(x => x.Id)),
                    VatTuBenhVienId = item.First().VatTuBenhVienId,
                    Ma = item.First().Ma,
                    Ten = item.First().Ten,
                    TenDVKT = item.First().TenDVKT,
                    KhoId = item.First().KhoId,
                    TenKho = item.First().TenKho,
                    DVT = item.First().DVT,
                    DonGiaNhap = item.First().DonGiaNhap,
                    DonGiaBan = item.First().DonGiaBan,
                    DonGia = item.First().DonGiaBan,
                    SoLuong = item.Sum(a => a.SoLuong),
                    TiLeTheoThapGia = item.First().TiLeTheoThapGia,
                    VAT = item.First().VAT,
                    LaVatTuBHYT = item.First().LaVatTuBHYT,
                    TinhTrang = item.First().TinhTrang,
                    CoYeuCauTraVTTuBenhNhanChiTiet = item.First().CoYeuCauTraVTTuBenhNhanChiTiet,
                    LaTuTruc = item.First().LaTuTruc,
                    KhongTinhPhi = item.First().KhongTinhPhi,
                    ThoiDiemChiDinh = item.First().ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.First().TenNhanVienChiDinh,
                    DonGiaBaoHiem = item.First().DonGiaBaoHiem,
                    PhieuDieuTriVatTuGiaGroupGridVos = item.GroupBy(a => new { a.DonGiaBan, a.DonGiaBaoHiem }).Select(a => new PhieuDieuTriVatTuGiaGroupGridVo
                    {
                        KhongTinhPhi = a.First().KhongTinhPhi,
                        DonGia = a.First().DonGiaBan,
                        SoLuong = a.Sum(b => b.SoLuong)
                    }).ToList(),

                    //BVHD-3905
                    TiLeThanhToanBHYT = item.First().TiLeThanhToanBHYT

                }).OrderBy(x => x.CreatedOn).ThenBy(x => x.Ten).ToList();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : lstQuery.Count();
            var queryTask = lstQuery.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public GridDataSource GetTotalPageForGridDanhSachVatTu(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId && o.NoiTruPhieuDieuTriId == phieuDieuTriId
                  && o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                          && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                          && o.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                .Select(s => new PhieuDieuTriVatTuGridVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenKho = s.KhoLinh.Ten,
                    KhongTinhPhi = !s.KhongTinhPhi,
                    CreatedOn = s.CreatedOn,
                });
            var lstQuery = query.GroupBy(x => new { x.Ma, x.Ten, x.TenKho, x.KhongTinhPhi, x.CreatedOn })
            .Select(item
            => new PhieuDieuTriVatTuGridVo()
            {
                Id = item.First().Id,
            });
            return new GridDataSource { TotalRowCount = lstQuery.Count() };
        }

        public GridDataSource GetDataForGridDanhSachVatTuKhoTong(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId && o.NoiTruPhieuDieuTriId == phieuDieuTriId
                 && o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                          && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                          && o.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                .Select(s => new PhieuDieuTriVatTuGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    KhoId = s.KhoLinhId,
                    TenKho = s.KhoLinh.Ten,
                    TenDVKT = s.YeuCauDichVuKyThuat.TenDichVu,
                    DVT = s.DonViTinh,
                    SoLuong = s.SoLuong,
                    DonGiaNhap = s.DonGiaNhap,
                    DonGiaBan = s.DonGiaBan,
                    DonGia = s.DonGiaBan,
                    TiLeTheoThapGia = s.TiLeTheoThapGia,
                    VAT = s.VAT,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    TinhTrang = s.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null,
                    KhongTinhPhi = !s.KhongTinhPhi,
                    PhieuLinh = s.YeuCauLinhVatTu.SoPhieu,
                    PhieuXuat = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
                    CoYeuCauTraVTTuBenhNhanChiTiet = s.YeuCauTraVatTuTuBenhNhanChiTiets.Any(),
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    DonGiaBaoHiem = s.DonGiaBaoHiem,
                    CreatedOn = s.CreatedOn,

                    //BVHD-3905
                    TiLeThanhToanBHYT = s.VatTuBenhVien.TiLeThanhToanBHYT
                });
            var lstQuery = query.GroupBy(x => new { x.Ma, x.Ten, x.TenKho, x.KhongTinhPhi, x.CreatedOn })
                .Select(item
                => new PhieuDieuTriVatTuGridVo()
                {
                    Id = item.First().Id,
                    Ids = string.Join(",", item.Select(x => x.Id)),
                    VatTuBenhVienId = item.First().VatTuBenhVienId,
                    Ma = item.First().Ma,
                    Ten = item.First().Ten,
                    TenDVKT = item.First().TenDVKT,
                    KhoId = item.First().KhoId,
                    TenKho = item.First().TenKho,
                    DVT = item.First().DVT,
                    DonGiaNhap = item.First().DonGiaNhap,
                    DonGiaBan = item.First().DonGiaBan,
                    DonGia = item.First().DonGiaBan,
                    SoLuong = item.Sum(a => a.SoLuong),
                    TiLeTheoThapGia = item.First().TiLeTheoThapGia,
                    VAT = item.First().VAT,
                    LaVatTuBHYT = item.First().LaVatTuBHYT,
                    TinhTrang = item.First().TinhTrang,
                    CoYeuCauTraVTTuBenhNhanChiTiet = item.First().CoYeuCauTraVTTuBenhNhanChiTiet,
                    LaTuTruc = item.First().LaTuTruc,
                    KhongTinhPhi = item.First().KhongTinhPhi,
                    ThoiDiemChiDinh = item.First().ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.First().TenNhanVienChiDinh,
                    PhieuLinh = item.First().PhieuLinh,
                    PhieuXuat = item.First().PhieuXuat,
                    DonGiaBaoHiem = item.First().DonGiaBaoHiem,
                    PhieuDieuTriVatTuGiaGroupGridVos = item.GroupBy(a => new { a.DonGiaBan, a.DonGiaBaoHiem }).Select(a => new PhieuDieuTriVatTuGiaGroupGridVo
                    {
                        KhongTinhPhi = a.First().KhongTinhPhi,
                        DonGia = a.First().DonGiaBan,
                        SoLuong = a.Sum(b => b.SoLuong)
                    }).ToList(),

                    //BVHD-3905
                    TiLeThanhToanBHYT = item.First().TiLeThanhToanBHYT
                }).OrderBy(x => x.CreatedOn).ThenBy(x => x.Ten).ToList();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : lstQuery.Count();
            var queryTask = lstQuery.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public GridDataSource GetTotalPageForGridDanhSachVatTuKhoTong(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
              .Where(o => o.NoiTruPhieuDieuTri.NoiTruBenhAnId == yeuCauTiepNhanId && o.NoiTruPhieuDieuTriId == phieuDieuTriId
               && o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                        && o.LoaiNoiChiDinh == LoaiNoiChiDinh.NoiTruPhieuDieuTri
                        && o.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
              .Select(s => new PhieuDieuTriVatTuGridVo
              {
                  Id = s.Id,
                  Ma = s.Ma,
                  Ten = s.Ten,
                  KhongTinhPhi = !s.KhongTinhPhi,
                  CreatedOn = s.CreatedOn,
              });
            var lstQuery = query.GroupBy(x => new { x.Ma, x.Ten, x.TenKho, x.KhongTinhPhi, x.CreatedOn })
             .Select(item
             => new PhieuDieuTriVatTuGridVo()
             {
                 Id = item.First().Id,
             });
            return new GridDataSource { TotalRowCount = lstQuery.Count() };
        }

        public async Task<List<KhoLookupItemVo>> GetKhoVatTuCurrentUser(DropDownListRequestModel queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaPhongId = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == noiLamViecCurrentId).Select(p => p.KhoaPhongId).First();
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                        .Where(p => p.NhanVienId == userCurrentId && (p.Kho.PhongBenhVienId == noiLamViecCurrentId || p.Kho.KhoaPhongId == khoaPhongId) && p.Kho.LoaiVatTu == true && p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && p.Kho.LoaiVatTu == true))
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

        public ThongTinPhieuDieuTriVatTu GetVatTuInfoById(ThongTinChiTietVatTuTonKhoPDT thongTinVatTuVo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var laVatTuBHYT = false;
            if (thongTinVatTuVo.LoaiVatTu == 2) // BHYT
            {
                laVatTuBHYT = true;
            }
            var vatTuInfo = _vatTuBenhVienRepository.TableNoTracking
               .Where(o => o.Id == thongTinVatTuVo.VatTuBenhVienId)
               .Select(d => new ThongTinPhieuDieuTriVatTu
               {
                   FlagVatTuDaKeTrungKho = _yeuCauVatTuBenhVienRepository.TableNoTracking.Any(p => p.NoiTruPhieuDieuTriId == thongTinVatTuVo.NoiTruPhieuDieuTriId
                                                                                                && p.Ten == d.VatTus.Ten
                                                                                                && p.KhoLinhId == thongTinVatTuVo.KhoId
                                                                                                && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                                                                                && p.YeuCauLinhVatTuId == null
                                                                                                && p.LaVatTuBHYT == laVatTuBHYT
                                                                                                ),
                   FlagVatTuDaKe = _yeuCauVatTuBenhVienRepository.TableNoTracking.Any(p => p.NoiTruPhieuDieuTriId == thongTinVatTuVo.NoiTruPhieuDieuTriId
                                                                                        && p.Ten == d.VatTus.Ten && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy),
                   DonViTinh = d.VatTus.DonViTinh,
                   TonKho = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(nkct => nkct.NhapKhoVatTu.KhoId == thongTinVatTuVo.KhoId
                                                                              && nkct.VatTuBenhVienId == thongTinVatTuVo.VatTuBenhVienId
                                                                              && nkct.LaVatTuBHYT == laVatTuBHYT
                                                                              && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                   HanSuDung = _nhapKhoVatTuChiTietRepository.TableNoTracking
                                       .Where(nkct => nkct.NhapKhoVatTu.KhoId == thongTinVatTuVo.KhoId
                                                   && nkct.VatTuBenhVienId == thongTinVatTuVo.VatTuBenhVienId
                                                   && nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                       .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                        .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                       .Select(o => o.HanSuDung).FirstOrDefault(),
               }).FirstOrDefault();
            return vatTuInfo;
        }
        public async Task ThemVatTu(VatTuBenhVienVo donVatTuChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == donVatTuChiTiet.PhieuDieuTriHienTaiId).First();
            var laVatTuBHYT = false;
            if (donVatTuChiTiet.LaVatTuBHYT == 2)
            {
                laVatTuBHYT = true;
            }

            //var vatTu = _vatTuRepository.GetById(donVatTuChiTiet.VatTuBenhVienId,
            //   x => x.Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu).Include(o => o.HopDongThauVatTuChiTiets)
            //         .Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho));

            var thongTinVatTu = _vatTuRepository.GetById(donVatTuChiTiet.VatTuBenhVienId,
               x => x.Include(o => o.VatTuBenhVien).Include(o => o.HopDongThauVatTuChiTiets));

            var dataNhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                .Where(p => p.VatTuBenhVienId == donVatTuChiTiet.VatTuBenhVienId && p.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId && (p.LaVatTuBHYT == laVatTuBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now);

            var loaiKho = _khoRepository.TableNoTracking.Where(p => p.Id == donVatTuChiTiet.KhoId).Select(p => p.LoaiKho).First();
            var bacSiChiDinhId = _userAgentHelper.GetCurrentUserId();
            var noiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
            
            //var SLTon = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
            //            .Where(p => p.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId && (p.LaVatTuBHYT == laVatTuBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
            //            .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            var SLTon = dataNhapKhoVatTuChiTiets.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber(2);
            if (SLTon < donVatTuChiTiet.SoLuong)
            {
                throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
            }

            double soLuongCanXuat = donVatTuChiTiet.SoLuong;

            //var nhapKhoVatTuChiTiet = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
            // .Where(o => ((o.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId) && (o.LaVatTuBHYT == laVatTuBHYT)
            //             && o.HanSuDung >= DateTime.Now
            //             && o.VatTuBenhVienId == donVatTuChiTiet.VatTuBenhVienId
            //             && o.SoLuongNhap > o.SoLuongDaXuat)).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
            
            var nhapKhoVatTuChiTiet = dataNhapKhoVatTuChiTiets.OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();

            var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

            var ycVatTuBenhVien = new YeuCauVatTuBenhVien
            {
                YeuCauTiepNhanId = donVatTuChiTiet.YeuCauTiepNhanId,
                VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                Ten = thongTinVatTu.Ten,
                Ma = thongTinVatTu.Ma,
                NhomVatTuId = thongTinVatTu.NhomVatTuId,
                DonViTinh = thongTinVatTu.DonViTinh,
                NhaSanXuat = thongTinVatTu.NhaSanXuat,
                NuocSanXuat = thongTinVatTu.NuocSanXuat,
                QuyCach = thongTinVatTu.QuyCach,
                MoTa = thongTinVatTu.MoTa,
                KhongTinhPhi = !donVatTuChiTiet.KhongTinhPhi,
                //TiLeBaoHiemThanhToan = nhapKhoVatTuChiTiet.TiLeBHYTThanhToan ?? 100,
                //HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                //NhaThauId = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhaThauId,
                //SoHopDongThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoHopDong,
                //SoQuyetDinhThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoQuyetDinh,
                //LoaiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.LoaiThau,
                //NhomThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhomThau,
                //GoiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.GoiThau,
                //NamThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.Nam,
                SoLuong = soLuongXuat,
                NhanVienChiDinhId = bacSiChiDinhId,
                NoiChiDinhId = noiChiDinhId,
                ThoiDiemChiDinh = DateTime.Now,
                //DaCapVatTu = false,
                YeuCauDichVuKyThuatId = donVatTuChiTiet.YeuCauDichVuKyThuatId,
                //TrangThai = EnumYeuCauVatTuBenhVien.ChuaThucHien,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                LaVatTuBHYT = laVatTuBHYT,
                //DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                //TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                //VAT = nhapKhoVatTuChiTiet.VAT,
                SoTienBenhNhanDaChi = 0,
                KhoLinhId = donVatTuChiTiet.KhoId,
                DuocHuongBaoHiem = laVatTuBHYT,
                LoaiPhieuLinh = loaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri
            };

            if (ycVatTuBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
            {
                var yeuCauVatTuBenhVienNew = ycVatTuBenhVien.Clone();
                var xuatKhoVatTu = new XuatKhoVatTu
                {
                    LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                    LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                    TenNguoiNhan = yeuCauTiepNhan.HoTen,
                    NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now,
                    KhoXuatId = donVatTuChiTiet.KhoId
                };

                var xuatChiTiet = new XuatKhoVatTuChiTiet()
                {
                    VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                    XuatKhoVatTu = xuatKhoVatTu,
                    NgayXuat = DateTime.Now
                };
                var lstYeuCau = new List<YeuCauVatTuBenhVien>();

                var nhapKhoVatTuChiTiets = dataNhapKhoVatTuChiTiets
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var item in nhapKhoVatTuChiTiets)
                {
                    if (donVatTuChiTiet.SoLuong > 0)
                    {
                        var giaTheoHopDong = thongTinVatTu.HopDongThauVatTuChiTiets.Where(o => o.HopDongThauVatTuId == item.HopDongThauVatTuId).Select(p => p.Gia).FirstOrDefault();
                        var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;
                        var tileBHYTThanhToanTheoNhap = item.LaVatTuBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;

                        if (yeuCauVatTuBenhVienNew.DonGiaNhap != 0 && (yeuCauVatTuBenhVienNew.DonGiaNhap != item.DonGiaNhap || yeuCauVatTuBenhVienNew.VAT != item.VAT || yeuCauVatTuBenhVienNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap))
                        {
                            yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                            yeuCauVatTuBenhVienNew.SoLuong = yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat);
                            lstYeuCau.Add(yeuCauVatTuBenhVienNew);

                            yeuCauVatTuBenhVienNew = ycVatTuBenhVien.Clone();
                            yeuCauVatTuBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                            yeuCauVatTuBenhVienNew.DaCapVatTu = true;
                            yeuCauVatTuBenhVienNew.VAT = item.VAT;
                            yeuCauVatTuBenhVienNew.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCauVatTuBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                            yeuCauVatTuBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;

                            xuatChiTiet = new XuatKhoVatTuChiTiet()
                            {
                                VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                                XuatKhoVatTu = xuatKhoVatTu,
                                NgayXuat = DateTime.Now
                            };
                        }
                        else
                        {
                            yeuCauVatTuBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                            yeuCauVatTuBenhVienNew.DaCapVatTu = true;
                            yeuCauVatTuBenhVienNew.VAT = item.VAT;
                            yeuCauVatTuBenhVienNew.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCauVatTuBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                            yeuCauVatTuBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;
                        }
                        if (item.SoLuongNhap > item.SoLuongDaXuat)
                        {
                            var xuatViTri = new XuatKhoVatTuChiTietViTri()
                            {
                                NhapKhoVatTuChiTietId = item.Id,
                                NgayXuat = DateTime.Now,
                                GhiChu = xuatChiTiet.XuatKhoVatTu.LyDoXuatKho
                            };

                            var tonTheoItem = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                            if (donVatTuChiTiet.SoLuong <= tonTheoItem)
                            {
                                xuatViTri.SoLuongXuat = donVatTuChiTiet.SoLuong;
                                item.SoLuongDaXuat = (item.SoLuongDaXuat + donVatTuChiTiet.SoLuong).MathRoundNumber(2);
                                donVatTuChiTiet.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                donVatTuChiTiet.SoLuong = (donVatTuChiTiet.SoLuong - tonTheoItem).MathRoundNumber(2);
                            }

                            xuatChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatViTri);
                        }

                        if (donVatTuChiTiet.SoLuong == 0)
                        {
                            yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                            yeuCauVatTuBenhVienNew.SoLuong = yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat).MathRoundNumber(2);
                            lstYeuCau.Add(yeuCauVatTuBenhVienNew);
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
                    noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(item);
                }
            }
            else
            {
                var yeuCauNew = ycVatTuBenhVien.Clone();
                var thongTinNhapVatTu = dataNhapKhoVatTuChiTiets
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
                
                var giaTheoHopDong = thongTinVatTu.HopDongThauVatTuChiTiets.Where(o => o.HopDongThauVatTuId == thongTinNhapVatTu.HopDongThauVatTuId).Select(p => p.Gia).FirstOrDefault();
                var donGiaBaoHiem = thongTinNhapVatTu.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapVatTu.DonGiaNhap;
                yeuCauNew.DonGiaNhap = thongTinNhapVatTu.DonGiaNhap;
                yeuCauNew.VAT = thongTinNhapVatTu.VAT;
                yeuCauNew.TrangThai = EnumYeuCauVatTuBenhVien.ChuaThucHien;
                yeuCauNew.DaCapVatTu = false;
                yeuCauNew.TiLeTheoThapGia = thongTinNhapVatTu.TiLeTheoThapGia;
                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapVatTu.TiLeBHYTThanhToan ?? 100;
                yeuCauNew.SoLuong = donVatTuChiTiet.SoLuong;
                donVatTuChiTiet.SoLuong = 0;
                noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauNew);
            }
        }

        public async Task CapNhatVatTu(VatTuBenhVienVo donVatTuChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var ids = new List<long>();
            ids.AddRange(donVatTuChiTiet.Ids.Split(",").Select(z => long.Parse(z)));
            foreach (var id in ids)
            {
                var ycVatTuBenhVien = yeuCauTiepNhan.YeuCauVatTuBenhViens.FirstOrDefault(p => p.Id == id);

                if (ycVatTuBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("PhieuDieuTri.VatTu.NotExists"));
                }
                if (ycVatTuBenhVien.YeuCauLinhVatTuId != null)
                {
                    throw new Exception(_localizationService.GetResource("PhieuDieuTri.VatTu.DaLinh"));
                }

                ycVatTuBenhVien.SoLuong = donVatTuChiTiet.SoLuong;

                var soLuongTruoc = ycVatTuBenhVien.SoLuong;

                if (ycVatTuBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                {
                    var lstNhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                          .Where(x => x.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId
                                      && x.NhapKhoVatTu.DaHet != true
                                      && x.HanSuDung >= DateTime.Now
                                      && x.SoLuongDaXuat < x.SoLuongNhap)
                          .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                          .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                          .ToListAsync();

                    var chiTietXuat =
                           ycVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                .OrderByDescending(x => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? x.NhapKhoVatTuChiTiet.HanSuDung : x.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien)
                               .ThenByDescending(x => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? x.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien : x.NhapKhoVatTuChiTiet.HanSuDung);

                    var soLuongTonKhoChiTietHienTai = chiTietXuat.Sum(x => x.SoLuongXuat);
                    if (soLuongTonKhoChiTietHienTai < donVatTuChiTiet.SoLuong)// Tăng số lượng vt 
                    {
                        var SLTang = donVatTuChiTiet.SoLuong - soLuongTonKhoChiTietHienTai;
                        foreach (var item in chiTietXuat)
                        {
                            var SLTon = item.NhapKhoVatTuChiTiet.SoLuongNhap - item.NhapKhoVatTuChiTiet.SoLuongDaXuat;
                            if (SLTon <= SLTang)
                            {
                                item.SoLuongXuat +=
                                    (item.NhapKhoVatTuChiTiet.SoLuongNhap -
                                     item.NhapKhoVatTuChiTiet.SoLuongDaXuat);
                                item.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                                    item.NhapKhoVatTuChiTiet.SoLuongNhap;
                                var nhapChiTiet = lstNhapKhoVatTuChiTiet.First(x =>
                                    x.Id == item.NhapKhoVatTuChiTietId);
                                nhapChiTiet.SoLuongDaXuat =
                                    item.NhapKhoVatTuChiTiet.SoLuongDaXuat;
                                SLTang -= SLTon;
                            }
                            else
                            {
                                item.NhapKhoVatTuChiTiet.SoLuongDaXuat += SLTang;
                                item.SoLuongXuat += SLTang;
                                var nhapChiTiet = lstNhapKhoVatTuChiTiet.First(x =>
                                    x.Id == item.NhapKhoVatTuChiTietId);
                                nhapChiTiet.SoLuongDaXuat += SLTang;
                                SLTang = 0;
                            }
                        }
                        ycVatTuBenhVien.SoLuong =
                            ycVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);

                        if (SLTang > 0)
                        {
                            if ((lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) ||
                                         (lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < SLTang))
                            {
                                throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                            }
                            var xuatChiTiet = new XuatKhoVatTuChiTiet()
                            {
                                VatTuBenhVienId = ycVatTuBenhVien.VatTuBenhVienId
                            };
                            foreach (var item in lstNhapKhoVatTuChiTiet)
                            {
                                if (ycVatTuBenhVien.DonGiaNhap == item.DonGiaNhap && ycVatTuBenhVien.VAT == item.VAT &&
                                ycVatTuBenhVien.TiLeTheoThapGia == item.TiLeTheoThapGia)
                                {
                                    var newXuatViTri = new XuatKhoVatTuChiTietViTri()
                                    {
                                        NhapKhoVatTuChiTietId = item.Id
                                    };

                                    var SLTonNhapKhoChiTiet = item.SoLuongNhap - item.SoLuongDaXuat;
                                    if (SLTonNhapKhoChiTiet >= SLTang)
                                    {
                                        newXuatViTri.SoLuongXuat = SLTang;
                                        item.SoLuongDaXuat += SLTang;
                                        SLTang = 0;
                                    }
                                    else
                                    {
                                        newXuatViTri.SoLuongXuat = item.SoLuongNhap - item.SoLuongDaXuat;
                                        SLTang -= SLTonNhapKhoChiTiet;
                                        item.SoLuongDaXuat = item.SoLuongNhap;
                                    }

                                    ycVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(newXuatViTri);

                                    ycVatTuBenhVien.SoLuong = ycVatTuBenhVien.XuatKhoVatTuChiTiet
                                        .XuatKhoVatTuChiTietViTris.Where(p => p.WillDelete != true)
                                        .Sum(x => x.SoLuongXuat);
                                }
                                else
                                {
                                    await ThemVatTu(donVatTuChiTiet, yeuCauTiepNhan);
                                }
                            }
                        }
                        else
                        {
                            ycVatTuBenhVien.SoLuong =
                                ycVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                    .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                        }


                    }
                    else // Giảm số lượng vt
                    {
                        var soLuongGiam = soLuongTonKhoChiTietHienTai - donVatTuChiTiet.SoLuong;

                        foreach (var item in chiTietXuat)
                        {
                            if (item.NhapKhoVatTuChiTiet.SoLuongDaXuat > soLuongGiam)
                            {
                                item.NhapKhoVatTuChiTiet.SoLuongDaXuat -= soLuongGiam;
                                item.SoLuongXuat -= soLuongGiam;
                                soLuongGiam = 0;
                            }
                            else
                            {
                                soLuongGiam -= item.NhapKhoVatTuChiTiet.SoLuongDaXuat;
                                item.NhapKhoVatTuChiTiet.SoLuongDaXuat = 0;
                                item.WillDelete = true;
                            }
                        }
                        ycVatTuBenhVien.SoLuong = ycVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                   .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                    }
                }
                else
                {
                    var lstNhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.NhapKhoVatTu.KhoId == ycVatTuBenhVien.KhoLinhId
                                        && x.VatTuBenhVienId == ycVatTuBenhVien.VatTuBenhVienId
                                        && x.NhapKhoVatTu.DaHet != true
                                        && x.HanSuDung >= DateTime.Now
                                        && x.LaVatTuBHYT == ycVatTuBenhVien.LaVatTuBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                            .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .ToListAsync();
                    ycVatTuBenhVien.KhongTinhPhi = !donVatTuChiTiet.KhongTinhPhi;
                    if (lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < donVatTuChiTiet.SoLuong)
                    {
                        throw new Exception(
                            _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }
                }

            }

        }

        public async Task<string> CapNhatVatTuChoTuTruc(VatTuBenhVienVo donVatTuChiTiet, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var ids = new List<long>();
            ids.AddRange(donVatTuChiTiet.Ids.Split(",").Select(z => long.Parse(z)));
            foreach (var id in ids)
            {
                var ycVatTuBenhVien = yeuCauTiepNhan.YeuCauVatTuBenhViens.FirstOrDefault(p => p.Id == id);
                if (ycVatTuBenhVien == null)
                {
                    //throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.NotExists"));
                    return GetResourceValueByResourceName("PhieuDieuTri.VatTu.NotExists");
                }
                ycVatTuBenhVien.KhongTinhPhi = !donVatTuChiTiet.KhongTinhPhi;
                if (donVatTuChiTiet.SoLuong > ycVatTuBenhVien.SoLuong)
                {
                    var laVatTuBHYT = false;
                    if (donVatTuChiTiet.LaVatTuBHYT == 2)
                    {
                        laVatTuBHYT = true;
                    }
                    var SLTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(p => p.VatTuBenhVienId == ycVatTuBenhVien.VatTuBenhVienId && p.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId && p.LaVatTuBHYT == laVatTuBHYT && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
                        .Select(o => o.SoLuongNhap - o.SoLuongDaXuat).DefaultIfEmpty().Sum() + ycVatTuBenhVien.SoLuong;

                    //var SLTon = ycVatTuBenhVien.VatTuBenhVien.NhapKhoVatTuChiTiets
                    //  .Where(p => p.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId && p.LaVatTuBHYT == laVatTuBHYT && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now)
                    //  .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) + ycVatTuBenhVien.SoLuong;
                    if (SLTon.MathRoundNumber(2) < donVatTuChiTiet.SoLuong)
                    {
                        return GetResourceValueByResourceName("DonVTYT.VTYTSoLuongTon");
                    }
                }
            }
            return string.Empty;
        }
        public async Task XoaVatTu(string ids, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var ycVatTuBenhVienIds = new List<long>();
            ycVatTuBenhVienIds.AddRange(ids.Split(",").Select(z => long.Parse(z)));
            foreach (var ycVatTuBenhVienId in ycVatTuBenhVienIds)
            {
                var ycVatTuBenhVien = yeuCauTiepNhan.YeuCauVatTuBenhViens.FirstOrDefault(x => x.Id == ycVatTuBenhVienId);
                if (ycVatTuBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("PhieuDieuTri.VatTu.NotExists"));
                }
                if (ycVatTuBenhVien.YeuCauLinhVatTuChiTiets.Any())
                {
                    throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonThuoc.DaTaoPhieuLinh"));
                }
                if (ycVatTuBenhVien.YeuCauLinhVatTuId != null)
                {
                    throw new Exception(_localizationService.GetResource("PhieuDieuTri.VatTu.DaLinh"));
                }

                if (ycVatTuBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                {
                    ycVatTuBenhVien.TrangThai = EnumYeuCauVatTuBenhVien.DaHuy;
                    var xuatKhoVtViTris = ycVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.ToList();
                    var xuatKhoVatTuViTriHoanTras = new List<XuatKhoVatTuChiTietViTri>();
                    if (ycVatTuBenhVien.XuatKhoVatTuChiTiet != null)
                    {
                        foreach (var thongTinXuat in xuatKhoVtViTris)
                        {
                            thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat -= thongTinXuat.SoLuongXuat;
                        }
                    }
                    foreach (var item in xuatKhoVtViTris)
                    {
                        var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                        {
                            XuatKhoVatTuChiTietId = item.XuatKhoVatTuChiTietId,
                            NhapKhoVatTuChiTietId = item.NhapKhoVatTuChiTietId,
                            SoLuongXuat = -item.SoLuongXuat,
                            NgayXuat = DateTime.Now,
                            GhiChu = ycVatTuBenhVien.TrangThai.GetDescription()
                        };
                        ycVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                    }
                }
                else
                {
                    if (ycVatTuBenhVien.XuatKhoVatTuChiTiet != null)
                    {
                        foreach (var thongTinXuat in ycVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris)
                        {
                            thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat -= thongTinXuat.SoLuongXuat;
                        }
                    }
                    ycVatTuBenhVien.TrangThai = EnumYeuCauVatTuBenhVien.DaHuy;
                }
                await XuLyXoaYLenhKhiXoaDichVuNoiTruAsync(EnumNhomGoiDichVu.VatTuTieuHao, ycVatTuBenhVienId);
            }

        }

        public async Task<CoDonThuocKhoLeKhoTong> KiemTraCoDonVT(long noiTruPhieuDieuTriId)
        {
            var ycVatTuBVKhoLe = await _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(p => p.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId
                         && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                         && p.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe).ToListAsync();

            var ycVatTuBVKhoTong = await _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(p => p.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId
                         && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                         && p.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2).ToListAsync();

            var coDonThuocKhoLeKhoTong = new CoDonThuocKhoLeKhoTong();
            if (ycVatTuBVKhoLe.Any())
            {
                coDonThuocKhoLeKhoTong.CoDonThuocKhoLe = true;
            }
            if (ycVatTuBVKhoTong.Any())
            {
                coDonThuocKhoLeKhoTong.CoDonThuocKhoTong = true;
            }
            return coDonThuocKhoLeKhoTong;
        }

        public async Task<ThongTinHoanTraVTVo> GetThongTinHoanTraVatTu(HoanTraVTVo hoanTraVTVo)
        {
            var loaiKho = _khoRepository.TableNoTracking.Where(p => p.Id == hoanTraVTVo.KhoId).Select(p => p.LoaiKho).First();
            var tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == hoanTraVTVo.KhoId).Select(p => p.Ten).First();
            var nhanVienDangNhap = _nhanVienRepository.TableNoTracking.Where(p => p.Id == _userAgentHelper.GetCurrentUserId()).Select(p => p.User.HoTen).FirstOrDefault();

            var nhanVienYeuCauId = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking.Where(p => p.YeuCauVatTuBenhVienId == hoanTraVTVo.YeuCauVatTuBenhVienId).Select(p => p.NhanVienYeuCauId).FirstOrDefault();

            var tenNhanVienYeuCau = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking.Where(p => p.YeuCauVatTuBenhVienId == hoanTraVTVo.YeuCauVatTuBenhVienId).Select(p => p.NhanVienYeuCau.User.HoTen).FirstOrDefault();
            var ids = hoanTraVTVo.Ids.Split(",").Select(z => long.Parse(z));
            var thongTinChiTiets = new List<ThongTinHoanTraVTVo>();
            foreach (var id in ids)
            {
                var thongTinChiTiet = _yeuCauVatTuBenhVienRepository
                              .TableNoTracking.Where(p => p.Id == id && p.LaVatTuBHYT == hoanTraVTVo.LaVatTuBHYT && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy && p.KhoLinh.LoaiKho == loaiKho)
                              .Select(s => new ThongTinHoanTraVTVo
                              {
                                  Id = s.Id,
                                  VatTuBenhVienId = s.VatTuBenhVienId,
                                  Ten = s.Ten,
                                  TenKho = tenKho,
                                  NhanVienYeuCauId = nhanVienYeuCauId != 0 ? nhanVienYeuCauId : _userAgentHelper.GetCurrentUserId(),
                                  TenNhanVienYeuCau = !string.IsNullOrEmpty(tenNhanVienYeuCau) ? tenNhanVienYeuCau : nhanVienDangNhap,
                                  NgayYeuCau = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu ? (DateTime?)null : s.YeuCauTraVatTuTuBenhNhanChiTiets.FirstOrDefault(c => c.YeuCauTraVatTuTuBenhNhanId == null).NgayYeuCau,
                                  DonGiaNhap = s.DonGiaNhap,
                                  DonGia = s.DonGiaBan,
                                  SoLuong = s.SoLuong,
                                  SoLuongDaTra = s.SoLuongDaTra.GetValueOrDefault(),
                                  TiLeTheoThapGia = s.TiLeTheoThapGia,
                                  VAT = s.VAT,
                                  KhongTinhPhi = s.KhongTinhPhi,
                                  SoLuongTra = s.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan ? s.YeuCauTraVatTuTuBenhNhanChiTiets.Where(x => x.LaVatTuBHYT == s.LaVatTuBHYT && x.YeuCauTraVatTuTuBenhNhanId == null).Select(z => z.SoLuongTra).FirstOrDefault() : 0,
                              }).FirstOrDefault();
                thongTinChiTiets.Add(thongTinChiTiet);
            }
            var thongTin = new ThongTinHoanTraVTVo
            {
                Ids = string.Join(",", thongTinChiTiets.Select(x => x.Id)),
                VatTuBenhVienId = thongTinChiTiets.First().VatTuBenhVienId,
                Ten = thongTinChiTiets.First().Ten,
                TenKho = thongTinChiTiets.First().TenKho,
                NhanVienYeuCauId = thongTinChiTiets.First().NhanVienYeuCauId,
                TenNhanVienYeuCau = thongTinChiTiets.First().TenNhanVienYeuCau,
                NgayYeuCau = thongTinChiTiets.First().NgayYeuCau,
                YeuCauVatTuBenhViens = thongTinChiTiets.Select(p => new ThongTinHoanTraVTChiTietVo
                {
                    Id = p.Id,
                    YeuCauVatTuBenhVienId = p.Id,
                    KhongTinhPhi = p.KhongTinhPhi,
                    SoLuong = p.SoLuong,
                    DonGiaNhap = p.DonGiaNhap,
                    DonGia = p.DonGia,
                    TiLeTheoThapGia = p.TiLeTheoThapGia,
                    VAT = p.VAT,
                    //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                    SoLuongDaTra = p.SoLuongDaTra - p.SoLuongTra.GetValueOrDefault(),
                    SoLuongTra = p.SoLuongTra,
                }).ToList()
            };
            return thongTin;
        }

        public async Task<GridDataSource> GetDataForGridDanhSachVatTuHoanTra(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<HoanTraVTGridVo>(queryInfo.AdditionalSearchString);
            var yeuCauVatTuBenhVienIds = info.Ids.Split(",").Select(z => long.Parse(z)).ToList();
            var laVatTuBHYT = info.LaVatTuBHYT;
            var laTuTruc = info.LaTuTruc;

            if (laTuTruc)
            {
                var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
               .Where(p => yeuCauVatTuBenhVienIds.Contains(p.YeuCauVatTuBenhVienId) && p.LaVatTuBHYT == laVatTuBHYT)
               .Select(s => new YeuCauTraDuocPhamTuBenhNhanChiTietGridVo
               {
                   Id = s.Id,
                   NgayTra = s.NgayYeuCau,
                   SoLuongTra = ((double?)s.SoLuongTra).FloatToStringFraction(),
                   NhanVienTra = s.NhanVienYeuCau.User.HoTen,
                   SoPhieu = s.YeuCauTraVatTuTuBenhNhan.SoPhieu,
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
                var query = _yeuCauTraVatTuTuBenhNhanChiTietRepository.TableNoTracking
               .Where(p => yeuCauVatTuBenhVienIds.Contains(p.YeuCauVatTuBenhVienId) && p.LaVatTuBHYT == laVatTuBHYT && p.YeuCauTraVatTuTuBenhNhanId != null)
               .Select(s => new YeuCauTraDuocPhamTuBenhNhanChiTietGridVo
               {
                   Id = s.Id,
                   NgayTra = s.NgayYeuCau,
                   SoLuongTra = ((double?)s.SoLuongTra).FloatToStringFraction(),
                   NhanVienTra = s.NhanVienYeuCau.User.HoTen,
                   SoPhieu = s.YeuCauTraVatTuTuBenhNhan.SoPhieu,
                   DuocDuyet = s.YeuCauTraVatTuTuBenhNhan.DuocDuyet,
                   NgayTao = s.CreatedOn,
               });
                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }

        }

        public async Task<GridDataSource> GetTotalPageForGridDanhSachVatTuHoanTra(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<string> HoanTraVatTuTuBenhNhan(YeuCauTraVatTuTuBenhNhanChiTietVo yeuCauTraVT)
        {
            foreach (var vtbv in yeuCauTraVT.YeuCauVatTuBenhViens)
            {
                var yeuCau = await _yeuCauVatTuBenhVienRepository.GetByIdAsync(vtbv.YeuCauVatTuBenhVienId, s => s
                                            .Include(p => p.YeuCauTraVatTuTuBenhNhanChiTiets)
                                            .Include(p => p.XuatKhoVatTuChiTiet).ThenInclude(p => p.XuatKhoVatTuChiTietViTris).ThenInclude(p => p.NhapKhoVatTuChiTiet)
                                            .Include(p => p.XuatKhoVatTuChiTiet).ThenInclude(p => p.XuatKhoVatTu));
                if (yeuCau == null)
                {
                    throw new Exception(_localizationService.GetResource("PhieuDieuTri.VatTu.NotExists"));
                }
                var ycChiTiet = yeuCau.YeuCauTraVatTuTuBenhNhanChiTiets.FirstOrDefault(p => p.YeuCauTraVatTuTuBenhNhanId == null);
                var khoXuatId = yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId;
                var loaiKho = _khoRepository.TableNoTracking.Where(p => p.Id == khoXuatId).First().LoaiKho;
                var traVeTuTruc = false;
                if (loaiKho == EnumLoaiKhoDuocPham.KhoLe)
                {
                    traVeTuTruc = true;
                    ycChiTiet = null;
                }
                if (ycChiTiet == null)
                {
                    if (vtbv.SoLuongTra > 0)
                    {
                        if (!traVeTuTruc)
                        {
                            //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                            if (yeuCau.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && yeuCau.KhongTinhPhi != true)
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                            if (yeuCau.SoLuongDaTra == null)
                            {
                                yeuCau.SoLuongDaTra = 0;
                            }
                            yeuCau.SoLuongDaTra += vtbv.SoLuongTra;
                            yeuCau.SoLuong -= vtbv.SoLuongTra.Value;
                            if (yeuCau.SoLuong < 0)
                            {
                                throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                            }
                            //end update BVHD-3411
                            var ycHoanTraVTNew = new YeuCauTraVatTuTuBenhNhanChiTiet
                            {
                                VatTuBenhVienId = yeuCau.VatTuBenhVienId,
                                LaVatTuBHYT = yeuCau.LaVatTuBHYT,
                                SoLuongTra = vtbv.SoLuongTra ?? 0,
                                KhoTraId = khoXuatId,
                                TraVeTuTruc = traVeTuTruc,
                                NgayYeuCau = yeuCauTraVT.NgayYeuCau,
                                NhanVienYeuCauId = yeuCauTraVT.NhanVienYeuCauId
                            };
                            yeuCau.YeuCauTraVatTuTuBenhNhanChiTiets.Add(ycHoanTraVTNew);
                        }
                        else
                        {
                            var xuatKhoVtViTris = yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.ToList();
                            var xuatKhoVatTuViTriHoanTras = new List<XuatKhoVatTuChiTietViTri>();
                            if (yeuCau.SoLuongDaTra == null)
                            {
                                yeuCau.SoLuongDaTra = 0;
                            }
                            yeuCau.SoLuong -= vtbv.SoLuongTra.Value;
                            yeuCau.SoLuongDaTra += vtbv.SoLuongTra;
                            if (yeuCau.XuatKhoVatTuChiTiet != null)
                            {
                                foreach (var thongTinXuat in xuatKhoVtViTris)
                                {
                                    thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat -= vtbv.SoLuongTra.Value;
                                }
                            }
                            foreach (var item in xuatKhoVtViTris)
                            {
                                var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                                {
                                    XuatKhoVatTuChiTietId = item.XuatKhoVatTuChiTietId,
                                    NhapKhoVatTuChiTietId = item.NhapKhoVatTuChiTietId,
                                    SoLuongXuat = -vtbv.SoLuongTra.Value,
                                    NgayXuat = DateTime.Now,
                                    GhiChu = "Hoàn trả vật tư"
                                };
                                yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
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
                    if (yeuCau.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && yeuCau.KhongTinhPhi != true)
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                    }
                    var soLuongThayDoi = vtbv.SoLuongTra.GetValueOrDefault() - ycChiTiet.SoLuongTra;
                    if (yeuCau.SoLuongDaTra == null)
                    {
                        yeuCau.SoLuongDaTra = 0;
                    }
                    yeuCau.SoLuongDaTra += soLuongThayDoi;
                    yeuCau.SoLuong -= soLuongThayDoi;
                    if (yeuCau.SoLuong < 0)
                    {
                        throw new Exception(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
                    }
                    //end update BVHD-3411
                    if (vtbv.SoLuongTra > 0) // Cập nhật
                    {
                        ycChiTiet.NhanVienYeuCauId = yeuCauTraVT.NhanVienYeuCauId;
                        ycChiTiet.NgayYeuCau = yeuCauTraVT.NgayYeuCau;
                        ycChiTiet.SoLuongTra = vtbv.SoLuongTra.GetValueOrDefault();
                    }
                    else // Xóa
                    {
                        ycChiTiet.WillDelete = true;
                    }
                }
                await _yeuCauVatTuBenhVienRepository.UpdateAsync(yeuCau);
            }
            return string.Empty;
        }

        public async Task<List<DichVuKyThuatDaThemLookupItem>> GetDichVuKyThuatDaThem(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(YeuCauDichVuKyThuat.MaDichVu),
                nameof(YeuCauDichVuKyThuat.TenDichVu),
            };
            var lstDichVuKyThuats = new List<DichVuKyThuatDaThemLookupItem>();
            var noiTruPhieuDieuTriId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstDichVuKyThuats = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(p => p.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem || p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang))
                    .Select(item => new DichVuKyThuatDaThemLookupItem
                    {
                        DisplayName = item.TenDichVu,
                        KeyId = item.Id,
                        Ten = item.TenDichVu,
                        Ma = item.MaDichVu,
                    })
                    .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                    .OrderByDescending(x => x.KeyId == noiTruPhieuDieuTriId).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstYeuCauDichVuId = await _yeuCauDichVuKyThuatRepository
                    .ApplyFulltext(queryInfo.Query, nameof(YeuCauDichVuKyThuat), lstColumnNameSearch)
                    .Where(p => p.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem || p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang))
                    .Select(x => x.Id)
                    .ToListAsync();

                lstDichVuKyThuats = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => lstYeuCauDichVuId.Contains(x.Id))
                    .OrderByDescending(x => x.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId)
                    .ThenBy(p => lstYeuCauDichVuId.IndexOf(p.Id) != -1 ? lstYeuCauDichVuId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new DichVuKyThuatDaThemLookupItem
                    {
                        DisplayName = item.TenDichVu,
                        KeyId = item.Id,
                        Ten = item.TenDichVu,
                        Ma = item.MaDichVu,
                    }).ToListAsync();
            }
            return lstDichVuKyThuats;
        }

        public async Task<KetQuaApDungNoiTruDonVTYTTongHopVo> ApDungDonVTYTChoCacNgayDieuTriAsync(NoiTruDonVTYTTongHopVo model, YeuCauTiepNhan yeuCauTiepNhan)
        {
            #region Cập nhật 26/12/2022
            var lstVatTuBenhVienId = model.NoiTruDonVTYTTongHopChiTietVos.Select(x => x.VatTuBenhVienId).Distinct().ToList();
            var lstVatTuChiTiet = new List<Core.Domain.Entities.VatTus.VatTu>();
            if (lstVatTuBenhVienId.Any())
            {
                lstVatTuChiTiet = _vatTuRepository.Table
                    .Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu).Include(o => o.HopDongThauVatTuChiTiets)
                    .Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho)
                    .Where(x => lstVatTuBenhVienId.Contains(x.Id))
                    .ToList();
            }

            #endregion

            List<NoiTruDonVTYTTongHopChiTietVo> noiTruDonVTYTTongHopChiTietVos = new List<NoiTruDonVTYTTongHopChiTietVo>();
            foreach (var ngayDieuTri in model.Dates)
            {
                var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.NgayDieuTri == ngayDieuTri).FirstOrDefault();
                if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id == 0)
                {
                    noiTruPhieuDieuTri.NhanVienLapId = _userAgentHelper.GetCurrentUserId();
                    noiTruPhieuDieuTri.KhoaPhongDieuTriId = model.KhoaId.GetValueOrDefault();
                }

                foreach (var donThuocChiTiet in model.NoiTruDonVTYTTongHopChiTietVos)
                {
                    var donThuocChiTietNew = donThuocChiTiet.Clone();
                    noiTruDonVTYTTongHopChiTietVos.Add(await ApDungDonVTYTNoiTruChiTiet(donThuocChiTietNew, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri, lstVatTuChiTiet));
                }
            }
            if (noiTruDonVTYTTongHopChiTietVos.Any(o => o.SoLuongTonKho < o.SoLuong))
            {
                return new KetQuaApDungNoiTruDonVTYTTongHopVo
                {
                    ThanhCong = false,
                    Error = "Số lượng VTYT trong kho không đủ",
                    NoiTruDonVTYTTongHopChiTietVos = noiTruDonVTYTTongHopChiTietVos,
                };
            }
            return new KetQuaApDungNoiTruDonVTYTTongHopVo
            {
                ThanhCong = true,
            };
        }
        private async Task<NoiTruDonVTYTTongHopChiTietVo> ApDungDonVTYTNoiTruChiTiet(NoiTruDonVTYTTongHopChiTietVo donVatTuChiTiet, NoiTruPhieuDieuTri noiTruPhieuDieuTri, YeuCauTiepNhan yeuCauTiepNhan, long khoaPhongDieuTriId, DateTime ngayDieuTri, List<Core.Domain.Entities.VatTus.VatTu> vatTus)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            #region Cập nhật 26/12/2022
            //var vatTu = await _vatTuRepository.GetByIdAsync(donVatTuChiTiet.VatTuBenhVienId,
            //   x => x.Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu).Include(o => o.HopDongThauVatTuChiTiets)
            //         .Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho));
            var vatTu = vatTus.FirstOrDefault(x => x.Id == donVatTuChiTiet.VatTuBenhVienId);
            if (vatTu == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }
            #endregion
            var SLTon = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
                       .Where(p => p.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId && (p.LaVatTuBHYT == donVatTuChiTiet.LaVatTuBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber(2);

            var noiTruDonVTYTTongHopVo = donVatTuChiTiet;
            var soLuong = donVatTuChiTiet.SoLuong;
            noiTruDonVTYTTongHopVo.SoLuongTonKho = 0;
            noiTruDonVTYTTongHopVo.NgayDieuTri = ngayDieuTri;
            double soLuongCanXuat = donVatTuChiTiet.SoLuong;

            var nhapKhoVatTuChiTiet = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
             .Where(o => o.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId
                         && o.LaVatTuBHYT == donVatTuChiTiet.LaVatTuBHYT
                         && o.HanSuDung >= DateTime.Now
                         && o.VatTuBenhVienId == donVatTuChiTiet.VatTuBenhVienId
                         && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault();
            if (nhapKhoVatTuChiTiet == null)
            {
                noiTruDonVTYTTongHopVo.SoLuongTonKho = 0;
                return noiTruDonVTYTTongHopVo;
            }

            var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

            var ycVatTuBenhVien = new YeuCauVatTuBenhVien
            {
                YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                Ten = vatTu.Ten,
                Ma = vatTu.Ma,
                NhomVatTuId = vatTu.NhomVatTuId,
                DonViTinh = vatTu.DonViTinh,
                NhaSanXuat = vatTu.NhaSanXuat,
                NuocSanXuat = vatTu.NuocSanXuat,
                QuyCach = vatTu.QuyCach,
                MoTa = vatTu.MoTa,
                KhongTinhPhi = !donVatTuChiTiet.KhongTinhPhi,
                SoLuong = soLuongXuat,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                YeuCauDichVuKyThuatId = donVatTuChiTiet.YeuCauDichVuKyThuatId,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                LaVatTuBHYT = donVatTuChiTiet.LaVatTuBHYT,
                SoTienBenhNhanDaChi = 0,
                KhoLinhId = donVatTuChiTiet.KhoId,
                DuocHuongBaoHiem = donVatTuChiTiet.LaVatTuBHYT,
                LoaiPhieuLinh = donVatTuChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri
            };

            if (ycVatTuBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
            {
                var yeuCauVatTuBenhVienNew = ycVatTuBenhVien.Clone();
                var xuatKhoVatTu = new XuatKhoVatTu
                {
                    LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                    LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                    TenNguoiNhan = yeuCauTiepNhan.HoTen,
                    NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now,
                    KhoXuatId = donVatTuChiTiet.KhoId
                };

                var xuatChiTiet = new XuatKhoVatTuChiTiet()
                {
                    VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                    XuatKhoVatTu = xuatKhoVatTu,
                    NgayXuat = DateTime.Now
                };
                var lstYeuCau = new List<YeuCauVatTuBenhVien>();

                var nhapKhoVatTuChiTiets = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
                    .Where(o => o.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId
                         && o.LaVatTuBHYT == donVatTuChiTiet.LaVatTuBHYT
                         && o.HanSuDung >= DateTime.Now
                         && o.VatTuBenhVienId == donVatTuChiTiet.VatTuBenhVienId
                         && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var item in nhapKhoVatTuChiTiets)
                {
                    if (donVatTuChiTiet.SoLuong > 0)
                    {
                        var giaTheoHopDong = vatTu.HopDongThauVatTuChiTiets.Where(o => o.HopDongThauVatTuId == item.HopDongThauVatTuId).Select(p => p.Gia).FirstOrDefault();
                        var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;
                        var tileBHYTThanhToanTheoNhap = item.LaVatTuBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;

                        if (yeuCauVatTuBenhVienNew.DonGiaNhap != 0 && (yeuCauVatTuBenhVienNew.DonGiaNhap != item.DonGiaNhap || yeuCauVatTuBenhVienNew.VAT != item.VAT || yeuCauVatTuBenhVienNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap))
                        {
                            yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                            yeuCauVatTuBenhVienNew.SoLuong = yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat);
                            lstYeuCau.Add(yeuCauVatTuBenhVienNew);

                            yeuCauVatTuBenhVienNew = ycVatTuBenhVien.Clone();
                            yeuCauVatTuBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                            yeuCauVatTuBenhVienNew.DaCapVatTu = true;
                            yeuCauVatTuBenhVienNew.VAT = item.VAT;
                            yeuCauVatTuBenhVienNew.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCauVatTuBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                            yeuCauVatTuBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;

                            xuatChiTiet = new XuatKhoVatTuChiTiet()
                            {
                                VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                                XuatKhoVatTu = xuatKhoVatTu,
                                NgayXuat = DateTime.Now
                            };
                        }
                        else
                        {
                            yeuCauVatTuBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                            yeuCauVatTuBenhVienNew.DaCapVatTu = true;
                            yeuCauVatTuBenhVienNew.VAT = item.VAT;
                            yeuCauVatTuBenhVienNew.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCauVatTuBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                            yeuCauVatTuBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;
                        }
                        if (item.SoLuongNhap > item.SoLuongDaXuat)
                        {
                            var xuatViTri = new XuatKhoVatTuChiTietViTri()
                            {
                                NhapKhoVatTuChiTietId = item.Id,
                                NgayXuat = DateTime.Now,
                                GhiChu = xuatChiTiet.XuatKhoVatTu.LyDoXuatKho
                            };
                            var slTon = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                            noiTruDonVTYTTongHopVo.SoLuongTonKho = slTon;
                            var tonTheoItem = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                            if (donVatTuChiTiet.SoLuong <= tonTheoItem)
                            {
                                xuatViTri.SoLuongXuat = donVatTuChiTiet.SoLuong;
                                item.SoLuongDaXuat = (item.SoLuongDaXuat + donVatTuChiTiet.SoLuong).MathRoundNumber(2);
                                donVatTuChiTiet.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                donVatTuChiTiet.SoLuong = (donVatTuChiTiet.SoLuong - tonTheoItem).MathRoundNumber(2);
                            }

                            xuatChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatViTri);
                        }

                        if (donVatTuChiTiet.SoLuong == 0)
                        {
                            yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                            yeuCauVatTuBenhVienNew.SoLuong = yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat).MathRoundNumber(2);
                            lstYeuCau.Add(yeuCauVatTuBenhVienNew);
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
                    noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(item);
                }

                if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                {
                    foreach (var yeuCauVatTuBenhVien in lstYeuCau)
                    {
                        yeuCauVatTuBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                        noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauVatTuBenhVien);
                    }
                }
                else
                {
                    foreach (var yeuCauVatTuBenhVien in lstYeuCau)
                    {
                        yeuCauVatTuBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                        noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauVatTuBenhVien);
                    }
                    yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                }

            }
            else
            {
                var yeuCauNew = ycVatTuBenhVien.Clone();
                var nhapKhoVatTuChiTiets = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
                    .Where(o => o.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId
                         && o.LaVatTuBHYT == donVatTuChiTiet.LaVatTuBHYT
                         && o.HanSuDung >= DateTime.Now
                         && o.VatTuBenhVienId == donVatTuChiTiet.VatTuBenhVienId
                         && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung);

                var thongTinNhapVatTu = nhapKhoVatTuChiTiets.First();

                var giaTheoHopDong = vatTu.HopDongThauVatTuChiTiets.Where(o => o.HopDongThauVatTuId == thongTinNhapVatTu.HopDongThauVatTuId).Select(p => p.Gia).FirstOrDefault();
                var donGiaBaoHiem = thongTinNhapVatTu.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapVatTu.DonGiaNhap;
                yeuCauNew.DonGiaNhap = thongTinNhapVatTu.DonGiaNhap;
                yeuCauNew.VAT = thongTinNhapVatTu.VAT;
                yeuCauNew.TrangThai = EnumYeuCauVatTuBenhVien.ChuaThucHien;
                yeuCauNew.DaCapVatTu = false;
                yeuCauNew.TiLeTheoThapGia = thongTinNhapVatTu.TiLeTheoThapGia;
                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapVatTu.TiLeBHYTThanhToan ?? 100;
                yeuCauNew.SoLuong = donVatTuChiTiet.SoLuong;
                donVatTuChiTiet.SoLuong = 0;
                //noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauNew);

                noiTruDonVTYTTongHopVo.SoLuong = donVatTuChiTiet.SoLuong;
                noiTruDonVTYTTongHopVo.SoLuongTonKho = nhapKhoVatTuChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                {
                    yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                    noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauNew);
                }
                else
                {
                    yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                    noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauNew);
                    yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                }
            }
            noiTruDonVTYTTongHopVo.SoLuong = soLuong;
            return noiTruDonVTYTTongHopVo;
        }


        public async Task<string> ApDungDonVTYTChoCacNgayDieuTriConfirmAsync(NoiTruDonVTYTTongHopVo model, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var error = string.Empty;
            List<NoiTruDonVTYTTongHopChiTietVo> noiTruDonVTYTTongHopChiTietVos = new List<NoiTruDonVTYTTongHopChiTietVo>();
            foreach (var ngayDieuTri in model.Dates)
            {
                var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.NgayDieuTri == ngayDieuTri).FirstOrDefault();
                if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id == 0)
                {
                    noiTruPhieuDieuTri.NhanVienLapId = _userAgentHelper.GetCurrentUserId();
                    noiTruPhieuDieuTri.KhoaPhongDieuTriId = model.KhoaId.GetValueOrDefault();
                }
                foreach (var donThuocChiTiet in model.NoiTruDonVTYTTongHopChiTietVos)
                {
                    var donThuocChiTietNew = donThuocChiTiet.Clone();
                    error = await ApDungDonVTYTNoiTruChiTietConfirm(donThuocChiTietNew, noiTruPhieuDieuTri, yeuCauTiepNhan, model.KhoaId.GetValueOrDefault(), ngayDieuTri);
                }
            }
            return error;
        }

        private async Task<string> ApDungDonVTYTNoiTruChiTietConfirm(NoiTruDonVTYTTongHopChiTietVo donVatTuChiTiet, NoiTruPhieuDieuTri noiTruPhieuDieuTri, YeuCauTiepNhan yeuCauTiepNhan, long khoaPhongDieuTriId, DateTime ngayDieuTri)
        {
            var error = string.Empty;
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var vatTu = await _vatTuRepository.GetByIdAsync(donVatTuChiTiet.VatTuBenhVienId,
              x => x.Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu).Include(o => o.HopDongThauVatTuChiTiets)
                    .Include(o => o.VatTuBenhVien).ThenInclude(vtbv => vtbv.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho));
            var SLTon = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
                       .Where(p => p.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId && (p.LaVatTuBHYT == donVatTuChiTiet.LaVatTuBHYT) && p.SoLuongNhap > p.SoLuongDaXuat && p.HanSuDung >= DateTime.Now).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber(2);
            if (SLTon < donVatTuChiTiet.SoLuong)
            {
                return error;
            }

            double soLuongCanXuat = donVatTuChiTiet.SoLuong;
            var nhapKhoVatTuChiTiet = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
                .Where(o => o.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId
                      && o.LaVatTuBHYT == donVatTuChiTiet.LaVatTuBHYT
                      && o.HanSuDung >= DateTime.Now
                      && o.VatTuBenhVienId == donVatTuChiTiet.VatTuBenhVienId
                      && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First();
            var soLuongXuat = SLTon > soLuongCanXuat ? soLuongCanXuat : SLTon;

            var ycVatTuBenhVien = new YeuCauVatTuBenhVien
            {
                YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                Ten = vatTu.Ten,
                Ma = vatTu.Ma,
                NhomVatTuId = vatTu.NhomVatTuId,
                DonViTinh = vatTu.DonViTinh,
                NhaSanXuat = vatTu.NhaSanXuat,
                NuocSanXuat = vatTu.NuocSanXuat,
                QuyCach = vatTu.QuyCach,
                MoTa = vatTu.MoTa,
                KhongTinhPhi = !donVatTuChiTiet.KhongTinhPhi,
                SoLuong = soLuongXuat,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                YeuCauDichVuKyThuatId = donVatTuChiTiet.YeuCauDichVuKyThuatId,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                LaVatTuBHYT = donVatTuChiTiet.LaVatTuBHYT,
                SoTienBenhNhanDaChi = 0,
                KhoLinhId = donVatTuChiTiet.KhoId,
                DuocHuongBaoHiem = donVatTuChiTiet.LaVatTuBHYT,
                LoaiPhieuLinh = donVatTuChiTiet.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 ? EnumLoaiPhieuLinh.LinhChoBenhNhan : EnumLoaiPhieuLinh.LinhBu,
                LoaiNoiChiDinh = LoaiNoiChiDinh.NoiTruPhieuDieuTri
            };

            if (ycVatTuBenhVien.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
            {
                var yeuCauVatTuBenhVienNew = ycVatTuBenhVien.Clone();
                var xuatKhoVatTu = new XuatKhoVatTu
                {
                    LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                    LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                    TenNguoiNhan = yeuCauTiepNhan.HoTen,
                    NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now,
                    KhoXuatId = donVatTuChiTiet.KhoId
                };

                var xuatChiTiet = new XuatKhoVatTuChiTiet()
                {
                    VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                    XuatKhoVatTu = xuatKhoVatTu,
                    NgayXuat = DateTime.Now
                };
                var lstYeuCau = new List<YeuCauVatTuBenhVien>();

                var nhapKhoVatTuChiTiets = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
                    .Where(o => o.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId
                         && o.LaVatTuBHYT == donVatTuChiTiet.LaVatTuBHYT
                         && o.HanSuDung >= DateTime.Now
                         && o.VatTuBenhVienId == donVatTuChiTiet.VatTuBenhVienId
                         && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var item in nhapKhoVatTuChiTiets)
                {
                    if (donVatTuChiTiet.SoLuong > 0)
                    {
                        var giaTheoHopDong = vatTu.HopDongThauVatTuChiTiets.Where(o => o.HopDongThauVatTuId == item.HopDongThauVatTuId).Select(p => p.Gia).FirstOrDefault();
                        var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;
                        var tileBHYTThanhToanTheoNhap = item.LaVatTuBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;

                        if (yeuCauVatTuBenhVienNew.DonGiaNhap != 0 && (yeuCauVatTuBenhVienNew.DonGiaNhap != item.DonGiaNhap || yeuCauVatTuBenhVienNew.VAT != item.VAT || yeuCauVatTuBenhVienNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap))
                        {
                            yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                            yeuCauVatTuBenhVienNew.SoLuong = yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat);
                            lstYeuCau.Add(yeuCauVatTuBenhVienNew);

                            yeuCauVatTuBenhVienNew = ycVatTuBenhVien.Clone();
                            yeuCauVatTuBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                            yeuCauVatTuBenhVienNew.DaCapVatTu = true;
                            yeuCauVatTuBenhVienNew.VAT = item.VAT;
                            yeuCauVatTuBenhVienNew.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCauVatTuBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                            yeuCauVatTuBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;

                            xuatChiTiet = new XuatKhoVatTuChiTiet()
                            {
                                VatTuBenhVienId = donVatTuChiTiet.VatTuBenhVienId,
                                XuatKhoVatTu = xuatKhoVatTu,
                                NgayXuat = DateTime.Now
                            };
                        }
                        else
                        {
                            yeuCauVatTuBenhVienNew.DonGiaNhap = item.DonGiaNhap;
                            yeuCauVatTuBenhVienNew.DaCapVatTu = true;
                            yeuCauVatTuBenhVienNew.VAT = item.VAT;
                            yeuCauVatTuBenhVienNew.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCauVatTuBenhVienNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                            yeuCauVatTuBenhVienNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauVatTuBenhVienNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;
                        }
                        if (item.SoLuongNhap > item.SoLuongDaXuat)
                        {
                            var xuatViTri = new XuatKhoVatTuChiTietViTri()
                            {
                                NhapKhoVatTuChiTietId = item.Id,
                                NgayXuat = DateTime.Now,
                                GhiChu = xuatChiTiet.XuatKhoVatTu.LyDoXuatKho
                            };
                            var tonTheoItem = (item.SoLuongNhap - item.SoLuongDaXuat).MathRoundNumber(2);
                            if (donVatTuChiTiet.SoLuong <= tonTheoItem)
                            {
                                xuatViTri.SoLuongXuat = donVatTuChiTiet.SoLuong;
                                item.SoLuongDaXuat = (item.SoLuongDaXuat + donVatTuChiTiet.SoLuong).MathRoundNumber(2);
                                donVatTuChiTiet.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                donVatTuChiTiet.SoLuong = (donVatTuChiTiet.SoLuong - tonTheoItem).MathRoundNumber(2);
                            }

                            xuatChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatViTri);
                        }

                        if (donVatTuChiTiet.SoLuong == 0)
                        {
                            yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                            yeuCauVatTuBenhVienNew.SoLuong = yeuCauVatTuBenhVienNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat).MathRoundNumber(2);
                            lstYeuCau.Add(yeuCauVatTuBenhVienNew);
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
                    noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(item);
                }

                if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                {
                    foreach (var yeuCauVatTuBenhVien in lstYeuCau)
                    {
                        yeuCauVatTuBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                        noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauVatTuBenhVien);
                    }
                }
                else
                {
                    foreach (var yeuCauVatTuBenhVien in lstYeuCau)
                    {
                        yeuCauVatTuBenhVien.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                        noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauVatTuBenhVien);
                    }
                    yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                }

            }
            else
            {
                var yeuCauNew = ycVatTuBenhVien.Clone();
                var nhapKhoVatTuChiTiets = vatTu.VatTuBenhVien.NhapKhoVatTuChiTiets
                    .Where(o => o.NhapKhoVatTu.KhoId == donVatTuChiTiet.KhoId
                         && o.LaVatTuBHYT == donVatTuChiTiet.LaVatTuBHYT
                         && o.HanSuDung >= DateTime.Now
                         && o.VatTuBenhVienId == donVatTuChiTiet.VatTuBenhVienId
                         && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung);

                var thongTinNhapVatTu = nhapKhoVatTuChiTiets.First();

                var giaTheoHopDong = vatTu.HopDongThauVatTuChiTiets.Where(o => o.HopDongThauVatTuId == thongTinNhapVatTu.HopDongThauVatTuId).Select(p => p.Gia).FirstOrDefault();
                var donGiaBaoHiem = thongTinNhapVatTu.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapVatTu.DonGiaNhap;
                yeuCauNew.DonGiaNhap = thongTinNhapVatTu.DonGiaNhap;
                yeuCauNew.VAT = thongTinNhapVatTu.VAT;
                yeuCauNew.TrangThai = EnumYeuCauVatTuBenhVien.ChuaThucHien;
                yeuCauNew.DaCapVatTu = false;
                yeuCauNew.TiLeTheoThapGia = thongTinNhapVatTu.TiLeTheoThapGia;
                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapVatTu.TiLeBHYTThanhToan ?? 100;
                yeuCauNew.SoLuong = donVatTuChiTiet.SoLuong;
                donVatTuChiTiet.SoLuong = 0;
                //noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauNew);

                if (noiTruPhieuDieuTri != null && noiTruPhieuDieuTri.Id != 0)
                {
                    yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                    noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauNew);
                }
                else
                {
                    yeuCauNew.NoiTruPhieuDieuTri = noiTruPhieuDieuTri;
                    noiTruPhieuDieuTri.YeuCauVatTuBenhViens.Add(yeuCauNew);
                    yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Add(noiTruPhieuDieuTri);
                }
            }
            return error;

        }
    }
}
