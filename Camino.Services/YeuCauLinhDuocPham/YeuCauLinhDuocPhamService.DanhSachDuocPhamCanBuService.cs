using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Newtonsoft.Json;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.Grid;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanBu;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial class YeuCauLinhDuocPhamService
    {

        #region Duoc pham
        public GridDataSource GetDanhSachDuocPhamCanBuForGrid(QueryInfo queryInfo)
        {
            var list = GetDanhSachDuocPhamCanBu(queryInfo);
            return new GridDataSource { Data = list.ToArray(), TotalRowCount = list.Count() };

        }
        public List<DanhSachDuocPhamCanBuGridVo>  GetDanhSachDuocPhamCanBu(QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanBuQueryInfo = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ?
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanBuQueryInfo>(queryInfo.AdditionalSearchString) : new DanhSachDuocPhamCanBuQueryInfo();
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            //Lấy ds các kho dược phẩm mà nhân viên đang login có thể truy cập

            var khoLinhVeIds = _khoNhanVienQuanLyRepository.TableNoTracking
                .Where(p => p.NhanVienId == userCurrentId && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin)
                            && p.Kho.LoaiDuocPham == true
                            && (danhSachDuocPhamCanBuQueryInfo.KhoBuId == null || p.KhoId == danhSachDuocPhamCanBuQueryInfo.KhoBuId)
                            && p.Kho.YeuCauDuocPhamBenhViens.Any(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                                                      o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                                                      && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(t => t.YeuCauLinhDuocPham.DuocDuyet != null))
                                                                      && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                                                      && o.KhongLinhBu != true
                                                                      && o.YeuCauLinhDuocPhamId == null && o.KhoLinhId != null &&
                                                                      (long)o.KhoLinhId == p.KhoId))
                .Select(s => s.KhoId).ToList();
            var khoCap2S =
                _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && p.LoaiDuocPham == true
                && (danhSachDuocPhamCanBuQueryInfo.KhoLinhId == null || p.Id == danhSachDuocPhamCanBuQueryInfo.KhoLinhId)).ToList();

            var dsKhos = _khoRepository.TableNoTracking
                .Select(d => new
                {
                    Id =d.Id,
                    Ten = d.Ten
                }).ToList();

            var list = new List<DanhSachDuocPhamCanBuGridVo>();

            var yeuCauDuocPhamBenhVienDatas = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                            o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                            && o.SoLuong > 0
                            && o.KhoLinh.LoaiDuocPham == true
                            && o.KhongLinhBu != true
                            && o.YeuCauLinhDuocPhamId == null && o.KhoLinhId != null &&
                            khoLinhVeIds.Contains(o.KhoLinhId.GetValueOrDefault()))
                .Select(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.LaDuocPhamBHYT,
                    o.KhoLinhId,
                    YeuCauLinhDuocPhamChiTiets = o.YeuCauLinhDuocPhamChiTiets.Select(ct => new { ct.Id, ct.YeuCauLinhDuocPham.DuocDuyet }).ToList()
                }).ToList();

            var yeuCauDuocPhamBenhVienAlls = yeuCauDuocPhamBenhVienDatas.Where(o=>!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(p => p.DuocDuyet != null))
                .Select(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.LaDuocPhamBHYT,
                    o.KhoLinhId
                }).Distinct().ToList();

            var yeuCauDPBenhVienIds = yeuCauDuocPhamBenhVienAlls.Select(d => d.DuocPhamBenhVienId).ToList();

            var khoCap2Ids = khoCap2S.Select(d => d.Id).ToList();
            // Dp
            var dpsoLuongTonTheoDuocPhams = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => yeuCauDPBenhVienIds.Contains(x.DuocPhamBenhVienId)
                                            && khoCap2Ids.Contains(x.NhapKhoDuocPhams.KhoId)
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            //&& x.LaDuocPhamBHYT == item.LaDuocPhamVatTuBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                .Select(d => new {
                                    DuocPhamBenhVienId = d.DuocPhamBenhVienId,
                                    KhoId = d.NhapKhoDuocPhams.KhoId,
                                    DaHet = d.NhapKhoDuocPhams.DaHet,
                                    LaDuocPhamBHYT = d.LaDuocPhamBHYT,
                                    SoLuongNhap = d.SoLuongNhap,
                                    SoLuongDaXuat = d.SoLuongDaXuat
                                }).ToList();




            foreach (var khoLinhVeId in khoLinhVeIds)
            {

                var yeuCauDuocPhamBenhViens = yeuCauDuocPhamBenhVienAlls.Where(o => o.KhoLinhId == khoLinhVeId);
                if (yeuCauDuocPhamBenhViens.Any())
                {
                    foreach (var item in yeuCauDuocPhamBenhViens)
                    {
                        var coTon = false;
                        foreach (var khoCap2 in khoCap2S)
                        {

                            var soLuongTon = dpsoLuongTonTheoDuocPhams
                           .Where(x => x.DuocPhamBenhVienId == item.DuocPhamBenhVienId
                                       && x.KhoId == khoCap2.Id
                                       && x.DaHet != true
                                       && x.LaDuocPhamBHYT == item.LaDuocPhamBHYT
                                       && x.SoLuongDaXuat < x.SoLuongNhap)
                           .Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);

                            if (soLuongTon > 0)
                            {
                                coTon = true;
                                if (!list.Any(o => o.KhoLinhId == khoCap2.Id && o.KhoBuId == khoLinhVeId))
                                {
                                    list.Add(new DanhSachDuocPhamCanBuGridVo
                                    {
                                        KhoLinhId = khoCap2.Id,
                                        KhoLinh = khoCap2.Ten,
                                        KhoBuId = khoLinhVeId,
                                        KhoBu = dsKhos.Where(d=>d.Id == khoLinhVeId).Select(d=>d.Ten).FirstOrDefault()
                                    });
                                }
                                break;
                            }
                        }
                        if (!coTon && danhSachDuocPhamCanBuQueryInfo.KhoLinhId == null && !list.Any(o => o.KhoLinhId == 0 && o.KhoBuId == khoLinhVeId))
                        {
                            list.Add(new DanhSachDuocPhamCanBuGridVo
                            {
                                KhoLinhId = 0,
                                KhoLinh = "---Không có kho tồn---",
                                KhoBuId = khoLinhVeId,
                                KhoBu = dsKhos.Where(d => d.Id == khoLinhVeId).Select(d => d.Ten).FirstOrDefault()
                            });
                        }
                    }
                }
            }
            return list;


        }
        public async Task<GridDataSource> GetDanhSachChiTietDuocPhamCanBuForGrid(QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanBuQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.KhoLinhId == danhSachDuocPhamCanBuQueryInfo.KhoBuId &&
                            x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                            && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                            && (x.KhongLinhBu != true)
                            && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                            && x.KhoLinh.LoaiDuocPham == true
                            && x.SoLuong > 0
                            && x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                {
                    Id = item.Id,
                    DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                    LaBHYT = item.LaDuocPhamBHYT,
                    TenDuocPham = item.Ten,
                    //Nhom = item.LaDuocPhamBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                    NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                    HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                    DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                    DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                    NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                    SoLuongCanBu = item.SoLuong,
                    KhongLinhBu = item.KhongLinhBu,
                    SoLuongDaBu = item.SoLuongDaLinhBu
                })
                .GroupBy(x => new
                {
                    x.YeuCauLinhDuocPhamId,
                    x.DuocPhamBenhVienId,
                    x.LaBHYT,
                    x.Nhom,
                    x.NongDoHamLuong,
                    x.HoatChat,
                    x.DuongDung,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat,
                    x.SoLuongTon
                })
                .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                {
                    YeuCauLinhDuocPhamIdstring = string.Join(",", item.Select(x => x.Id)),
                    DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                    LaBHYT = item.First().LaBHYT,
                    TenDuocPham = item.First().TenDuocPham,
                    //Nhom = item.First().Nhom,
                    NongDoHamLuong = item.First().NongDoHamLuong,
                    HoatChat = item.First().HoatChat,
                    DuongDung = item.First().DuongDung,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    KhongLinhBu = item.First().KhongLinhBu,
                    SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                    SoLuongDaBu = item.Sum(x => x.SoLuongDaBu).MathRoundNumber(2),
                    SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                        x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                    && (danhSachDuocPhamCanBuQueryInfo.KhoLinhId == 0 || x.NhapKhoDuocPhams.KhoId == danhSachDuocPhamCanBuQueryInfo.KhoLinhId)
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.LaDuocPhamBHYT == item.First().LaBHYT
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat).MathRoundNumber(2),
                    KhoLinhId = danhSachDuocPhamCanBuQueryInfo.KhoLinhId,
                    KhoBuId = danhSachDuocPhamCanBuQueryInfo.KhoBuId


                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham)
                .Where(o => (danhSachDuocPhamCanBuQueryInfo.KhoLinhId == 0 && o.SoLuongTon < 0.001) || (danhSachDuocPhamCanBuQueryInfo.KhoLinhId > 0 && o.SoLuongTon > 0)).Distinct();
            //.OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageDanhSachChiTietDuocPhamCanBuForGrid(QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanBuQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.KhoLinhId == danhSachDuocPhamCanBuQueryInfo.KhoBuId &&
                            x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                            && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                            && (x.KhongLinhBu != true)
                            && x.KhoLinh.LoaiDuocPham == true
                            && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                            && x.SoLuong > 0
                            && x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                {
                    Id = item.Id,
                    DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                    LaBHYT = item.LaDuocPhamBHYT,
                    TenDuocPham = item.Ten,
                    //Nhom = item.LaDuocPhamBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                    NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                    HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                    DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                    DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                    NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                    KhongLinhBu = item.KhongLinhBu,
                    SoLuongCanBu = item.SoLuong,
                    SoLuongDaBu = item.SoLuongDaLinhBu

                })
                .GroupBy(x => new
                {
                    x.YeuCauLinhDuocPhamId,
                    x.DuocPhamBenhVienId,
                    x.LaBHYT,
                    x.Nhom,
                    x.NongDoHamLuong,
                    x.HoatChat,
                    x.DuongDung,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat,
                    x.SoLuongTon
                })
                .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                {
                    YeuCauLinhDuocPhamIdstring = string.Join(",", item.Select(x => x.Id)),
                    DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                    LaBHYT = item.First().LaBHYT,
                    TenDuocPham = item.First().TenDuocPham,
                    //Nhom = item.First().Nhom,
                    NongDoHamLuong = item.First().NongDoHamLuong,
                    HoatChat = item.First().HoatChat,
                    DuongDung = item.First().DuongDung,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    KhongLinhBu = item.First().KhongLinhBu,
                    NuocSanXuat = item.First().NuocSanXuat,
                    SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                    SoLuongDaBu = item.Sum(x => x.SoLuongDaBu),
                    SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 &&
                        x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                    && (danhSachDuocPhamCanBuQueryInfo.KhoLinhId == 0 || x.NhapKhoDuocPhams.KhoId == danhSachDuocPhamCanBuQueryInfo.KhoLinhId)
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.LaDuocPhamBHYT == item.First().LaBHYT
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    KhoLinhId = danhSachDuocPhamCanBuQueryInfo.KhoLinhId,
                    KhoBuId = danhSachDuocPhamCanBuQueryInfo.KhoBuId


                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham)
                .Where(o => (danhSachDuocPhamCanBuQueryInfo.KhoLinhId == 0 && o.SoLuongTon < 0.001) || (danhSachDuocPhamCanBuQueryInfo.KhoLinhId > 0 && o.SoLuongTon > 0)).Distinct();

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }
        public async Task<GridDataSource> GetDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid(
            QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanBuChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => o.DuocPhamBenhVienId == danhSachDuocPhamCanBuQueryInfo.DuocPhamBenhVienId
                                && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && o.LaDuocPhamBHYT == danhSachDuocPhamCanBuQueryInfo.LaBHYT
                                && o.YeuCauLinhDuocPhamId == null
                                && o.KhongLinhBu != true
                                && o.KhoLinh.LoaiDuocPham == true
                                && o.SoLuong > 0
                                && o.KhoLinhId == danhSachDuocPhamCanBuQueryInfo.KhoBuId
                                && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(a => a.YeuCauLinhDuocPham.DuocDuyet != null)
                                && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                )
                            )
                            .OrderBy(x => x.ThoiDiemChiDinh)
                            .Select(s => new DuocPhamLinhBuCuaBNGridVo
                            {
                                Id = s.Id,
                                MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                                HoTen = s.YeuCauTiepNhan.HoTen,
                                SL = s.SoLuong.MathRoundNumber(2),
                                SLDaBu = s.SoLuongDaLinhBu.MathRoundNumber(2),
                                DVKham = s.YeuCauKhamBenh.TenDichVu,
                                BSKeToa = s.NhanVienChiDinh.User.HoTen,
                                NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                                NgayDieuTri = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh
                            });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanBuForGrid(
            QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanBuChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => o.DuocPhamBenhVienId == danhSachDuocPhamCanBuQueryInfo.DuocPhamBenhVienId
                            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                            && o.LaDuocPhamBHYT == danhSachDuocPhamCanBuQueryInfo.LaBHYT
                            && o.YeuCauLinhDuocPhamId == null
                            && o.KhongLinhBu != true
                            && o.KhoLinh.LoaiDuocPham == true
                            && o.SoLuong > 0
                            && o.KhoLinhId == danhSachDuocPhamCanBuQueryInfo.KhoBuId
                            && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(a => a.YeuCauLinhDuocPham.DuocDuyet != null)
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                            )
                )
                .OrderBy(x => x.ThoiDiemChiDinh)
                .Select(s => new DuocPhamLinhBuCuaBNGridVo
                {
                    Id = s.Id,
                    MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SL = s.SoLuong,
                    SLDaBu = s.SoLuongDaLinhBu,
                    DVKham = s.YeuCauKhamBenh.TenDichVu,
                    BSKeToa = s.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    NgayDieuTri = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh
                });
            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }

        public List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var danhSachDuocPhamCanBu = GetDanhSachDuocPhamCanBu(new QueryInfo());
            return danhSachDuocPhamCanBu.Any() ? danhSachDuocPhamCanBu.GroupBy(o => new { o.KhoLinhId, o.KhoLinh }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.KhoLinhId,
                DisplayName = s.Key.KhoLinh
            }).ToList() : new List<LookupItemVo>();
        }
        public List<LookupItemVo> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var danhSachDuocPhamCanBu = GetDanhSachDuocPhamCanBu(new QueryInfo());
            return danhSachDuocPhamCanBu.Any() ? danhSachDuocPhamCanBu.GroupBy(o => new { o.KhoBuId, o.KhoBu }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.KhoBuId,
                DisplayName = s.Key.KhoBu
            }).ToList() : new List<LookupItemVo>();
        }
#endregion

        public async Task UpdateYeuCauDuocPhamBenhVien(string yeuCauLinhDuocPhamIdstring)
        {
            var yeuCauDuocPhamBenhVienIds = yeuCauLinhDuocPhamIdstring.Split(",").Select(p => long.Parse(p)).ToList();
            var chiTiets = _yeuCauDuocPhamBenhVienRepository.Table.Where(p => yeuCauDuocPhamBenhVienIds.Any(x => x == p.Id)).ToList();
            long userLogin = _userAgentHelper.GetCurrentUserId();
          
            foreach (var item in chiTiets)
            {
                item.KhongLinhBu = true;
                item.NgayDanhDauKhongBu = DateTime.Now;
                item.NguoiDanhDauKhongBuId = userLogin;
            }
            await _yeuCauDuocPhamBenhVienRepository.UpdateAsync(chiTiets);
        }
    }
}
