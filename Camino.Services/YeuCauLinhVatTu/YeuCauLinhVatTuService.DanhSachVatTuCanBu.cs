using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System.Globalization;
using Camino.Core.Domain.ValueObject.DanhSachVatTuCanBu;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial class YeuCauLinhVatTuService
    {
        #region  Ds vat tư cần bù

        public GridDataSource GetDanhSachVatTuCanBuForGrid(QueryInfo queryInfo)
        {
            var list = GetDanhSachVatTuCanBu(queryInfo);
            return new GridDataSource { Data = list.ToArray(), TotalRowCount = list.Count() };


        }
        public List<DanhSachVatTuCanBuGridVo> GetDanhSachVatTuCanBuOld(QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ?
                JsonConvert.DeserializeObject<DanhSachVatTuCanBuQueryInfo>(queryInfo.AdditionalSearchString) : new DanhSachVatTuCanBuQueryInfo();
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            //Lấy ds các kho vật tư mà nhân viên đang login có thể truy cập
            var khoLinhVes = _khoNhanVienQuanLyRepository.TableNoTracking
                .Where(p => p.NhanVienId == userCurrentId && p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                            (danhSachVatTuCanBuQueryInfo.KhoBuId == null || p.KhoId == danhSachVatTuCanBuQueryInfo.KhoBuId)
                            && p.Kho.YeuCauVatTuBenhViens.Any(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                                                      o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                                                      && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(t => t.YeuCauLinhVatTu.DuocDuyet != null))
                                                                      && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                                                      && o.KhongLinhBu != true
                                                                      && o.YeuCauLinhVatTuId == null && o.KhoLinhId != null &&
                                                                      (long)o.KhoLinhId == p.KhoId))
                .Select(s => new LookupItemVo
                {
                    KeyId = s.KhoId,
                    DisplayName = s.Kho.Ten
                }).OrderBy(o => o.DisplayName);

            var khoCap2S =
                _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2).ToList();
            var list = new List<DanhSachVatTuCanBuGridVo>();
            //Kiểm tra xem có dược phẩm nào cần bù trong kho lĩnh về này hay ko?
            var yeuCauVatTuBenhViens = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                            o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                            && o.KhongLinhBu != true
                            && o.SoLuong > 0
                            && o.YeuCauLinhVatTuId == null && o.KhoLinhId != null
                             && khoLinhVes.Any(i => i.KeyId == (long)o.KhoLinhId))
                .Select(o => o)
                .GroupBy(x => new
                {
                    x.VatTuBenhVienId,
                    x.LaVatTuBHYT
                }).Select(o => new
                {
                    o.First().VatTuBenhVienId,
                    o.First().LaVatTuBHYT
                }).Distinct().ToList();

            var khoCap2Ids = khoCap2S.Select(d => d.Id).ToList();
            var yeuCauVTBenhVienIds = yeuCauVatTuBenhViens.Select(d => d.VatTuBenhVienId).ToList();

            var dpsoLuongTonTheoVatTus = _nhapKhoVatTuChiTietRepository.TableNoTracking
                          .Where(x => yeuCauVTBenhVienIds.Contains(x.VatTuBenhVienId)
                                      && khoCap2Ids.Contains(x.NhapKhoVatTu.KhoId)
                                      && x.NhapKhoVatTu.DaHet != true
                                      //&& x.LaVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                      && x.SoLuongDaXuat < x.SoLuongNhap)
                           .Select(d => new {
                               VatTuBenhVienId = d.VatTuBenhVienId,
                               KhoId = d.NhapKhoVatTu.KhoId,
                               DaHet = d.NhapKhoVatTu.DaHet,
                               LaVatTuBHYT = d.LaVatTuBHYT,
                               SoLuongNhap = d.SoLuongNhap,
                               SoLuongDaXuat = d.SoLuongDaXuat
                           }).ToList();

            foreach (var khoLinhVe in khoLinhVes) //
            {
                if (yeuCauVatTuBenhViens.Any())
                {
                    foreach (var item in yeuCauVatTuBenhViens)
                    {
                        var coTon = false;
                        foreach (var khoCap2 in khoCap2S)
                        {
                            var soLuongTon = dpsoLuongTonTheoVatTus
                                   .Where(x => x.VatTuBenhVienId == item.VatTuBenhVienId
                                               && x.KhoId == khoCap2.Id
                                               && x.DaHet != true
                                               && x.LaVatTuBHYT == item.LaVatTuBHYT
                                               && x.SoLuongDaXuat < x.SoLuongNhap)
                                   .Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);

                            if (soLuongTon > 0)
                            {
                                coTon = true;
                                if (!list.Any(o => o.KhoLinhId == khoCap2.Id && o.KhoBuId == khoLinhVe.KeyId))
                                {
                                    list.Add(new DanhSachVatTuCanBuGridVo
                                    {
                                        KhoLinhId = khoCap2.Id,
                                        KhoLinh = khoCap2.Ten,
                                        KhoBuId = khoLinhVe.KeyId,
                                        KhoBu = khoLinhVe.DisplayName
                                    });
                                }
                                break;
                            }
                        }
                        if (!coTon && !list.Any(o => o.KhoLinhId == 0 && o.KhoBuId == khoLinhVe.KeyId))
                        {
                            list.Add(new DanhSachVatTuCanBuGridVo
                            {
                                KhoLinhId = 0,
                                KhoLinh = "---Không có kho tồn---",
                                KhoBuId = khoLinhVe.KeyId,
                                KhoBu = khoLinhVe.DisplayName
                            });
                        }
                    }
                }
            }
            return list;
        }
        public List<DanhSachVatTuCanBuGridVo> GetDanhSachVatTuCanBu(QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ?
                JsonConvert.DeserializeObject<DanhSachVatTuCanBuQueryInfo>(queryInfo.AdditionalSearchString) : new DanhSachVatTuCanBuQueryInfo();
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            //Lấy ds các kho vật tư mà nhân viên đang login có thể truy cập
            var khoLinhVeIds = _khoNhanVienQuanLyRepository.TableNoTracking
                .Where(p => p.NhanVienId == userCurrentId && p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                            (danhSachVatTuCanBuQueryInfo.KhoBuId == null || p.KhoId == danhSachVatTuCanBuQueryInfo.KhoBuId)
                            && p.Kho.YeuCauVatTuBenhViens.Any(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                                                      o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                                                      && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(t => t.YeuCauLinhVatTu.DuocDuyet != null))
                                                                      && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                                                      && o.KhongLinhBu != true
                                                                      && o.YeuCauLinhVatTuId == null && o.KhoLinhId != null &&
                                                                      (long)o.KhoLinhId == p.KhoId))
                .Select(s => s.KhoId).ToList();
             

            var khoCap2S =
                _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2).ToList();
            var list = new List<DanhSachVatTuCanBuGridVo>();

            var dsKhos = _khoRepository.TableNoTracking
               .Select(d => new
               {
                   Id = d.Id,
                   Ten = d.Ten
               }).ToList();


            //Kiểm tra xem có dược phẩm nào cần bù trong kho lĩnh về này hay ko?
            var yeuCauVatTuBenhVienDatas = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                            o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            //&& (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                            && o.KhongLinhBu != true
                            && o.SoLuong > 0
                            && o.YeuCauLinhVatTuId == null && o.KhoLinhId != null
                             && khoLinhVeIds.Contains(o.KhoLinhId.GetValueOrDefault()))
                .Select(o => o)
              .Select(o => new
              {
                  o.VatTuBenhVienId,
                  o.LaVatTuBHYT,
                  YeuCauLinhVatTuChiTiets = o.YeuCauLinhVatTuChiTiets.Select(ct => new { ct.Id, ct.YeuCauLinhVatTu.DuocDuyet }).ToList()
              }).ToList();

            var yeuCauVatTuBenhVienAlls = yeuCauVatTuBenhVienDatas.Where(o => !o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(p => p.DuocDuyet != null))
                  .Select(o => new
                  {
                      o.VatTuBenhVienId,
                      o.LaVatTuBHYT,
                  }).Distinct().ToList();

            var khoCap2Ids = khoCap2S.Select(d => d.Id).ToList();
            var yeuCauVTBenhVienIds = yeuCauVatTuBenhVienAlls.Select(d => d.VatTuBenhVienId).ToList();

            var dpsoLuongTonTheoVatTus = _nhapKhoVatTuChiTietRepository.TableNoTracking
                          .Where(x => yeuCauVTBenhVienIds.Contains(x.VatTuBenhVienId)
                                      && khoCap2Ids.Contains(x.NhapKhoVatTu.KhoId)
                                      && x.NhapKhoVatTu.DaHet != true
                                      //&& x.LaVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                      && x.SoLuongDaXuat < x.SoLuongNhap)
                           .Select(d => new {
                               VatTuBenhVienId = d.VatTuBenhVienId,
                               KhoId = d.NhapKhoVatTu.KhoId,
                               DaHet = d.NhapKhoVatTu.DaHet,
                               LaVatTuBHYT = d.LaVatTuBHYT,
                               SoLuongNhap = d.SoLuongNhap,
                               SoLuongDaXuat = d.SoLuongDaXuat
                           }).ToList();



            foreach (var khoLinhVeId in khoLinhVeIds) //
            {
                if (yeuCauVatTuBenhVienAlls.Any())
                {
                    foreach (var item in yeuCauVatTuBenhVienAlls)
                    {
                        var coTon = false;
                        foreach (var khoCap2 in khoCap2S)
                        {
                            var soLuongTon = dpsoLuongTonTheoVatTus
                                   .Where(x => x.VatTuBenhVienId == item.VatTuBenhVienId
                                               && x.KhoId == khoCap2.Id
                                               && x.DaHet != true
                                               && x.LaVatTuBHYT == item.LaVatTuBHYT
                                               && x.SoLuongDaXuat < x.SoLuongNhap)
                                   .Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);

                            if (soLuongTon > 0)
                            {
                                coTon = true;
                                if (!list.Any(o => o.KhoLinhId == khoCap2.Id && o.KhoBuId == khoLinhVeId))
                                {
                                    list.Add(new DanhSachVatTuCanBuGridVo
                                    {
                                        KhoLinhId = khoCap2.Id,
                                        KhoLinh = khoCap2.Ten,
                                        KhoBuId = khoLinhVeId,
                                        KhoBu = dsKhos.Where(d => d.Id == khoLinhVeId).Select(d => d.Ten).FirstOrDefault()
                                    });
                                }
                                break;
                            }
                        }
                        if (!coTon && !list.Any(o => o.KhoLinhId == 0 && o.KhoBuId == khoLinhVeId))
                        {
                            list.Add(new DanhSachVatTuCanBuGridVo
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
        public async Task<GridDataSource> GetDanhSachChiTietVatTuCanBuForGrid(QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachVatTuCanBuQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                            && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.All(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                            && x.KhongLinhBu != true
                            && x.SoLuong > 0
                            && x.YeuCauLinhVatTuId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                .Select(item => new YeuCauLinhBuVatTuVo()
                {
                    Id = item.Id,
                    VatTuBenhVienId = item.VatTuBenhVienId,
                    LaBHYT = item.LaVatTuBHYT,
                    TenVatTu = item.Ten,
                    Nhom = item.LaVatTuBHYT ? "VT BHYT" : "VT Không BHYT",
                    DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                    HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                    NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                    SoLuongCanBu = item.SoLuong,
                    KhongLinhBu = item.KhongLinhBu,
                    SoLuongDaLinhBu = item.SoLuongDaLinhBu
                })
                .GroupBy(x => new
                {
                    x.YeuCauLinhVatTuId,
                    x.VatTuBenhVienId,
                    x.LaBHYT,
                    x.Nhom,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat,
                    x.SoLuongTon
                })
                .Select(item => new YeuCauLinhBuVatTuVo()
                {
                    YeuCauLinhVatTuIdstring = string.Join(",", item.Select(x => x.Id)),
                    VatTuBenhVienId = item.First().VatTuBenhVienId,
                    LaBHYT = item.First().LaBHYT,
                    TenVatTu = item.First().TenVatTu,
                    Nhom = item.First().Nhom,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                    KhongLinhBu = item.First().KhongLinhBu,
                    SoLuongDaLinhBu = item.Sum(x => x.SoLuongDaLinhBu).MathRoundNumber(2),
                    SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && 
                        x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                    && (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 || x.NhapKhoVatTu.KhoId == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.LaVatTuBHYT == item.First().LaBHYT
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat).MathRoundNumber(2),
                    KhoLinhId = danhSachVatTuCanBuQueryInfo.KhoLinhId,
                    KhoBuId = danhSachVatTuCanBuQueryInfo.KhoBuId


                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu)
                .Where(o => (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 && o.SoLuongTon < 0.001) || (danhSachVatTuCanBuQueryInfo.KhoLinhId > 0 && o.SoLuongTon > 0)).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageDanhSachChiTietVatTuCanBuForGrid(QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachVatTuCanBuQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                            && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.All(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                            && x.KhongLinhBu != true
                            && x.SoLuong > 0
                            && x.YeuCauLinhVatTuId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                .Select(item => new YeuCauLinhBuVatTuVo()
                {
                    Id = item.Id,
                    VatTuBenhVienId = item.VatTuBenhVienId,
                    LaBHYT = item.LaVatTuBHYT,
                    TenVatTu = item.Ten,
                    Nhom = item.LaVatTuBHYT ? "VT BHYT" : "VT Không BHYT",
                    DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                    HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                    NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                    SoLuongCanBu = item.SoLuong,
                    KhongLinhBu = item.KhongLinhBu,
                    SoLuongDaLinhBu = item.SoLuongDaLinhBu
                })
                .GroupBy(x => new
                {
                    x.YeuCauLinhVatTuId,
                    x.VatTuBenhVienId,
                    x.LaBHYT,
                    x.Nhom,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat,
                    x.SoLuongTon
                })
                .Select(item => new YeuCauLinhBuVatTuVo()
                {
                    YeuCauLinhVatTuIdstring = string.Join(",", item.Select(x => x.Id)),
                    VatTuBenhVienId = item.First().VatTuBenhVienId,
                    LaBHYT = item.First().LaBHYT,
                    TenVatTu = item.First().TenVatTu,
                    Nhom = item.First().Nhom,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    KhongLinhBu = item.First().KhongLinhBu,
                    SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                    SoLuongDaLinhBu = item.Sum(x => x.SoLuongDaLinhBu),
                    SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && 
                        x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                    && (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 || x.NhapKhoVatTu.KhoId == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.LaVatTuBHYT == item.First().LaBHYT
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    KhoLinhId = danhSachVatTuCanBuQueryInfo.KhoLinhId,
                    KhoBuId = danhSachVatTuCanBuQueryInfo.KhoBuId


                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu)
                .Where(o => (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 && o.SoLuongTon < 0.001) || (danhSachVatTuCanBuQueryInfo.KhoLinhId > 0 && o.SoLuongTon > 0)).Distinct();

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }
        public async Task<GridDataSource> GetDanhSachChiTietYeuCauTheoVatTuCanBuForGrid(
            QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachVatTuCanBuChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.VatTuBenhVienId == danhSachVatTuCanBuQueryInfo.VatTuBenhVienId
                                && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                && o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && o.LaVatTuBHYT == danhSachVatTuCanBuQueryInfo.LaBHYT
                                && o.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                                && o.KhongLinhBu != true
                                && o.SoLuong > 0
                                && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(a => a.YeuCauLinhVatTu.DuocDuyet != null)
                                && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong))
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
                                NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH()
                            });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanBuForGrid(
            QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachVatTuCanBuChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.VatTuBenhVienId == danhSachVatTuCanBuQueryInfo.VatTuBenhVienId
                            && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && o.LaVatTuBHYT == danhSachVatTuCanBuQueryInfo.LaBHYT
                            && o.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId 
                            && o.KhongLinhBu != true
                            && o.SoLuong > 0
                            && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(a => a.YeuCauLinhVatTu.DuocDuyet != null)
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong))
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
                    NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH()
                });
            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }
        public List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var danhSachVatTuCanBu = GetDanhSachVatTuCanBu(new QueryInfo());
            return danhSachVatTuCanBu.Any() ? danhSachVatTuCanBu.GroupBy(o => new { o.KhoLinhId, o.KhoLinh }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.KhoLinhId,
                DisplayName = s.Key.KhoLinh
            }).ToList() : new List<LookupItemVo>();
        }
        public List<LookupItemVo> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var danhSachVatTuCanBu = GetDanhSachVatTuCanBu(new QueryInfo());
            return danhSachVatTuCanBu.Any() ? danhSachVatTuCanBu.GroupBy(o => new { o.KhoBuId, o.KhoBu }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.KhoBuId,
                DisplayName = s.Key.KhoBu
            }).ToList() : new List<LookupItemVo>();
        }
        #endregion

        public async Task UpdateYeuCauVatTuBenhVien(string yeuCauLinhVatTuIdstring)
        {
            var yeuCauDuocPhamVatTuIds = yeuCauLinhVatTuIdstring.Split(",").Select(p => long.Parse(p)).ToList();
            var chiTiets = _yeuCauVatTuBenhVienRepository.Table.Where(p => yeuCauDuocPhamVatTuIds.Any(x => x == p.Id)).ToList();
            long userLogin = _userAgentHelper.GetCurrentUserId();

            foreach (var item in chiTiets)
            {
                item.KhongLinhBu = true;
                item.NgayDanhDauKhongBu = DateTime.Now;
                item.NguoiDanhDauKhongBuId = userLogin;
            }
            await _yeuCauVatTuBenhVienRepository.UpdateAsync(chiTiets);
        }
    }
}
