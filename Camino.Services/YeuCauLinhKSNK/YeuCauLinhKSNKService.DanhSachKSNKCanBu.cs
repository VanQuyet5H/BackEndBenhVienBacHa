using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhBuKSNK;
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
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial class YeuCauLinhKSNKService
    {
        #region  Ds vat tư cần bù

        public GridDataSource GetDanhSachKSNKCanBuForGrid(QueryInfo queryInfo)
        {
            var list = GetDanhSachKSNKCanBu(queryInfo);
            return new GridDataSource { Data = list.ToArray(), TotalRowCount = list.Count() };
        }

        public List<DanhSachKSNKCanBuGridVo> GetDanhSachKSNKCanBu(QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ?
                JsonConvert.DeserializeObject<DanhSachKSNKCanBuQueryInfo>(queryInfo.AdditionalSearchString) : new DanhSachKSNKCanBuQueryInfo();
            var userCurrentId = _userAgentHelper.GetCurrentUserId();




            //Lấy ds các kho vật tư mà nhân viên đang login có thể truy cập
            var khoLinhVeIds = _khoNhanVienQuanLyRepository.TableNoTracking
                .Where(p => p.NhanVienId == userCurrentId && p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                            (danhSachVatTuCanBuQueryInfo.KhoBuId == null || p.KhoId == danhSachVatTuCanBuQueryInfo.KhoBuId) &&
                            ( p.Kho.YeuCauVatTuBenhViens.Any(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                                                      o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                                                      && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(t => t.YeuCauLinhVatTu.DuocDuyet != null))
                                                                      && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                                                      && o.KhongLinhBu != true
                                                                      && o.YeuCauLinhVatTuId == null && o.KhoLinhId != null &&
                                                                      (long)o.KhoLinhId == p.KhoId)
                            ||
                             p.Kho.YeuCauDuocPhamBenhViens.Any(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                                                      o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                                                      && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(t => t.YeuCauLinhDuocPham.DuocDuyet != null))
                                                                      && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                                                      && o.KhongLinhBu != true
                                                                      && o.YeuCauLinhDuocPhamId == null && o.KhoLinhId != null &&
                                                                      (long)o.KhoLinhId == p.KhoId))
                            && p.Kho.LaKhoKSNK == true) // to do
                .Select(s => s.KhoId).ToList(); // lấy tất cả kho lĩnh về là LaKhoKSNK (YC DP/ VT)

            var khoCap2S =
                _khoRepository.TableNoTracking.Where(p => (p.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK || p.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh) &&
                                                          (danhSachVatTuCanBuQueryInfo.KhoLinhId == null || p.Id == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                                          && p.LaKhoKSNK == true).ToList();


            var dsKhos = _khoRepository.TableNoTracking
             .Select(d => new
             {
                 Id = d.Id,
                 Ten = d.Ten
             }).ToList();

            var list = new List<DanhSachKSNKCanBuGridVo>();


            //Kiểm tra xem có dược phẩm, vật tư nào cần bù trong kho lĩnh về này hay ko?
            var yeuCauVatTuBenhVienDatas = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                            o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                            && o.KhongLinhBu != true
                            && o.SoLuong > 0
                            && o.YeuCauLinhVatTuId == null && o.KhoLinhId != null
                            && o.KhoLinh.LaKhoKSNK == true 
                            && khoLinhVeIds.Contains(o.KhoLinhId.GetValueOrDefault()))
               .Select(o => new 
               {
                   DuocPhamVatTuBenhVienId = o.VatTuBenhVienId,
                   LaDuocPhamVatTuBHYT= o.LaVatTuBHYT,
                   KhoLinhId= o.KhoLinhId,
                   LoaiDuocPhamHayVatTu = false,
                   YeuCauLinhVatTuChiTiets = o.YeuCauLinhVatTuChiTiets.Select(ct => new { ct.Id, ct.YeuCauLinhVatTu.DuocDuyet }).ToList()
                 }).Distinct().ToList();

            var yeuCauVatTuBenhVienAlls = yeuCauVatTuBenhVienDatas.Where(o => !o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(p => p.DuocDuyet != null))
                  .Select(o => new YeuCauDuocPhamVatTuBenhVienGridVo
                  {
                      DuocPhamVatTuBenhVienId = o.DuocPhamVatTuBenhVienId,
                      LaDuocPhamVatTuBHYT = o.LaDuocPhamVatTuBHYT,
                      KhoLinhId = o.KhoLinhId,
                      LoaiDuocPhamHayVatTu = false,
                  }).Distinct().ToList();


            //YeuCauDuocPhamVatTuBenhVienGridVo


            var yeuCauDuocPhamBenhVienDatas = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
              .Where(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                          o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                          && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                          && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                          && o.SoLuong > 0
                          && o.KhoLinh.LoaiDuocPham == true
                          && o.KhongLinhBu != true
                          && o.YeuCauLinhDuocPhamId == null && o.KhoLinhId != null
                          && o.KhoLinh.LaKhoKSNK == true
                          && khoLinhVeIds.Contains(o.KhoLinhId.GetValueOrDefault()))
              .Select(o => new 
              {
                  DuocPhamVatTuBenhVienId =o.DuocPhamBenhVienId,
                  LaDuocPhamVatTuBHYT =o.LaDuocPhamBHYT,
                  KhoLinhId =o.KhoLinhId,
                  LoaiDuocPhamHayVatTu =true,
                  YeuCauLinhDuocPhamChiTiets = o.YeuCauLinhDuocPhamChiTiets.Select(ct => new { ct.Id, ct.YeuCauLinhDuocPham.DuocDuyet }).ToList()
              }).Distinct().ToList();

            var yeuCauDuocPhamBenhVienAlls = yeuCauDuocPhamBenhVienDatas.Where(o => !o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(p => p.DuocDuyet != null))
                 .Select(o => new YeuCauDuocPhamVatTuBenhVienGridVo
                 {
                     DuocPhamVatTuBenhVienId = o.DuocPhamVatTuBenhVienId,
                     LaDuocPhamVatTuBHYT = o.LaDuocPhamVatTuBHYT,
                     KhoLinhId = o.KhoLinhId,
                     LoaiDuocPhamHayVatTu = true,
                 }).Distinct().ToList();



            var yeuCauDPVTBenhVienInfos = yeuCauVatTuBenhVienAlls.Concat(yeuCauDuocPhamBenhVienAlls).ToList();

            var yeuCauDPBenhVienIds = yeuCauDPVTBenhVienInfos.Where(d=>d.LoaiDuocPhamHayVatTu == true).Select(d => d.DuocPhamVatTuBenhVienId).ToList();
            
            var yeuCauVTBenhVienIds = yeuCauDPVTBenhVienInfos.Where(d => d.LoaiDuocPhamHayVatTu == false).Select(d => d.DuocPhamVatTuBenhVienId).ToList();

            
            var khoCap2Ids = khoCap2S.Select(d => d.Id).ToList();
            // Dp
            var dpsoLuongTonTheoDuocPhams = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => yeuCauDPBenhVienIds.Contains(x.DuocPhamBenhVienId)
                                            && khoCap2Ids.Contains(x.NhapKhoDuocPhams.KhoId)
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            //&& x.LaDuocPhamBHYT == item.LaDuocPhamVatTuBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                .Select(d => new {
                                    DuocPhamVatTuBenhVienId = d.DuocPhamBenhVienId,
                                    KhoId = d.NhapKhoDuocPhams.KhoId,
                                    DaHet = d.NhapKhoDuocPhams.DaHet,
                                    LaDuocPhamVatTuBHYT = d.LaDuocPhamBHYT,
                                    SoLuongNhap = d.SoLuongNhap ,
                                    SoLuongDaXuat = d.SoLuongDaXuat
                                }).ToList();

            // VT
            // đối với vật tư kho hành chính kiểm tra nhóm vật tư có bằng với enums  nhóm hành chính

            var dpSLTonTheoVatTus = _nhapKhoVatTuChiTietRepository.TableNoTracking
                              .Where(x => yeuCauVTBenhVienIds.Contains(x.VatTuBenhVienId) 
                                          && khoCap2Ids.Contains(x.NhapKhoVatTu.KhoId) 
                                          && x.NhapKhoVatTu.DaHet != true
                                          //&& x.LaVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                          && x.SoLuongDaXuat < x.SoLuongNhap)
                               .Select(d => new VatTuThuocNhomHCGridVo
                               {
                                    DuocPhamVatTuBenhVienId = d.VatTuBenhVienId,
                                    KhoId = d.NhapKhoVatTu.KhoId,
                                    DaHet = d.NhapKhoVatTu.DaHet,
                                    LaDuocPhamVatTuBHYT = d.LaVatTuBHYT,
                                    SoLuongNhap = d.SoLuongNhap,
                                    SoLuongDaXuat = d.SoLuongDaXuat,
                                    VatTuThuocLoaiKhoHC = d.NhapKhoVatTu.Kho.LoaiKho,
                                    VatTuThuocNhomHC = d.VatTuBenhVien.VatTus.NhomVatTuId
                               }).ToList();

            var inFoVatTus = new List<VatTuThuocNhomHCGridVo>();
            foreach (var item in dpSLTonTheoVatTus)
            {
                if(item.VatTuThuocLoaiKhoHC ==  Enums.EnumLoaiKhoDuocPham.KhoHanhChinh && item.VatTuThuocNhomHC == (long)Enums.EnumNhomVatTu.NhomHanhChinh)
                {
                    inFoVatTus.Add(item);
                }
                if (item.VatTuThuocLoaiKhoHC != Enums.EnumLoaiKhoDuocPham.KhoHanhChinh)
                {
                    inFoVatTus.Add(item);
                }
            }

            var inFoVatTuIds = inFoVatTus.Select(d => d.DuocPhamVatTuBenhVienId).ToList();

            // hiện tại đang show tất cả vật tư và dược phẩm theo loaiKho KSNK = true và Kho hành chính
            // loại bỏ những vật tư thuộc loại kho hành chính nhưng nhóm vật tư Id ! = Enums.EnumNhomVatTu.NhomHanhChinh
            var dpvt = yeuCauVatTuBenhVienAlls.Where(d=> inFoVatTuIds.Contains(d.DuocPhamVatTuBenhVienId)).Concat(yeuCauDuocPhamBenhVienAlls).ToList();
             

            foreach (var khoLinhVeId in khoLinhVeIds) //
            {
                var yeuCauDuocPhamVatTuBenhViens = dpvt.Where(o => o.KhoLinhId == khoLinhVeId);
                if (yeuCauDuocPhamVatTuBenhViens.Any())
                {
                    foreach (var item in yeuCauDuocPhamVatTuBenhViens)
                    {
                        var coTon = false;
                        foreach (var khoCap2 in khoCap2S)
                        {
                            if (item.LoaiDuocPhamHayVatTu == true)
                            {
                                var soLuongTonDP = dpsoLuongTonTheoDuocPhams
                                .Where(x => x.DuocPhamVatTuBenhVienId == item.DuocPhamVatTuBenhVienId
                                            && x.KhoId == khoCap2.Id
                                            && x.DaHet != true
                                            && x.LaDuocPhamVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                .Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                                if (soLuongTonDP > 0)
                                {
                                    coTon = true;
                                    if (!list.Any(o => o.KhoLinhId == khoCap2.Id && o.KhoBuId == khoLinhVeId))
                                    {
                                        list.Add(new DanhSachKSNKCanBuGridVo
                                        {
                                            KhoLinhId = khoCap2.Id,
                                            KhoLinh = khoCap2.Ten,
                                            KhoBuId = khoLinhVeId,
                                            KhoBu = dsKhos.Where(d => d.Id == khoLinhVeId).Select(d => d.Ten).FirstOrDefault(),
                                            LoaiDuocPhamHayVatTu = item.LoaiDuocPhamHayVatTu
                                        });
                                    }
                                    break;
                                }


                            }
                            else
                            {
                                var soLuongTonVT = inFoVatTus
                                   .Where(x => x.DuocPhamVatTuBenhVienId == item.DuocPhamVatTuBenhVienId
                                               && x.KhoId == khoCap2.Id
                                               && x.DaHet != true
                                               && x.LaDuocPhamVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                               && x.SoLuongDaXuat < x.SoLuongNhap)
                                   .Sum(x => x.SoLuongNhap - x.SoLuongDaXuat);
                                if (soLuongTonVT > 0)
                                {
                                    coTon = true;
                                    if (!list.Any(o => o.KhoLinhId == khoCap2.Id && o.KhoBuId == khoLinhVeId))
                                    {
                                        list.Add(new DanhSachKSNKCanBuGridVo
                                        {
                                            KhoLinhId = khoCap2.Id,
                                            KhoLinh = khoCap2.Ten,
                                            KhoBuId = khoLinhVeId,
                                            KhoBu = dsKhos.Where(d => d.Id == khoLinhVeId).Select(d => d.Ten).FirstOrDefault(),
                                            LoaiDuocPhamHayVatTu = item.LoaiDuocPhamHayVatTu
                                        });
                                    }
                                    break;
                                }
                            }
                        }
                        if (!coTon && !list.Any(o => o.KhoLinhId == 0 && o.KhoBuId == khoLinhVeId))
                        {
                            list.Add(new DanhSachKSNKCanBuGridVo
                            {
                                KhoLinhId = 0,
                                KhoLinh = "---Không có kho tồn---",
                                KhoBuId = khoLinhVeId,
                                KhoBu = dsKhos.Where(d => d.Id == khoLinhVeId).Select(d => d.Ten).FirstOrDefault(),
                                LoaiDuocPhamHayVatTu = item.LoaiDuocPhamHayVatTu
                            });
                        }
                    }
                }
            }
            return list;
        }
        public async Task<GridDataSource> GetDanhSachChiTietKSNKCanBuForGrid(QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachKSNKCanBuQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            // VT
            var queryVT = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                            && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.All(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                            && x.KhongLinhBu != true
                            && x.SoLuong > 0
                            && x.YeuCauLinhVatTuId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && x.KhoLinh.LaKhoKSNK == true)
                .Select(item => new YeuCauLinhBuKSNKVo()
                {
                    Id = item.Id,
                    VatTuBenhVienId = item.VatTuBenhVienId,
                    LaBHYT = item.LaVatTuBHYT,
                    TenVatTu = item.Ten,
                    Nhom = item.LaVatTuBHYT ? "BHYT" : "Không BHYT",
                    DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                    HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                    NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                    SoLuongCanBu = item.SoLuong,
                    KhongLinhBu = item.KhongLinhBu,
                    SoLuongDaLinhBu = item.SoLuongDaLinhBu,
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
                .Select(item => new YeuCauLinhBuKSNKVo()
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
                        .Where(x => (x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK || x.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh) &&
                        x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                    && (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 || x.NhapKhoVatTu.KhoId == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.LaVatTuBHYT == item.First().LaBHYT
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat).MathRoundNumber(2),
                    KhoLinhId = danhSachVatTuCanBuQueryInfo.KhoLinhId,
                    KhoBuId = danhSachVatTuCanBuQueryInfo.KhoBuId,
                    LoaiDuocPhamHayVatTu = false,
                    YeuCauLinhVatTuInFos = item.Select(f => new KhongYeuCauLinhBuKSNKVo()
                    {
                        YeuCauLinhId = f.Id,
                        LoaiDuocPhamHayVatTu = false
                    }).ToList(),
                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu)
                .Where(o => (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 && o.SoLuongTon < 0.001) || (danhSachVatTuCanBuQueryInfo.KhoLinhId > 0 && o.SoLuongTon > 0)).Distinct();

            var yeuCauVTBenhVienIds = queryVT.Select(d => d.VatTuBenhVienId).ToList();
            var khoCap2Ids =
                _khoRepository.TableNoTracking.Where(p => (p.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK || p.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh) &&
                                                          (danhSachVatTuCanBuQueryInfo.KhoLinhId == null || p.Id == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                                          && p.LaKhoKSNK == true).Select(d=>d.Id).ToList();

            var dpSLTonTheoVatTus = _nhapKhoVatTuChiTietRepository.TableNoTracking
                             .Where(x => yeuCauVTBenhVienIds.Contains(x.VatTuBenhVienId)
                                         && khoCap2Ids.Contains(x.NhapKhoVatTu.KhoId)
                                         && x.NhapKhoVatTu.DaHet != true
                                         //&& x.LaVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                         && x.SoLuongDaXuat < x.SoLuongNhap)
                              .Select(d => new
                              {
                                  VatTuBenhVienId = d.VatTuBenhVienId,
                       
                                  VatTuLoaiKhoHC = d.NhapKhoVatTu.Kho.LoaiKho,
                                  VatTuNhomHC = d.VatTuBenhVien.VatTus.NhomVatTuId
                              }).ToList();

            var inFoVatTus = new List<long>();
            foreach (var item in dpSLTonTheoVatTus)
            {
                if (item.VatTuLoaiKhoHC == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh && item.VatTuNhomHC == (long)Enums.EnumNhomVatTu.NhomHanhChinh)
                {
                    inFoVatTus.Add(item.VatTuBenhVienId);
                }
                if (item.VatTuLoaiKhoHC != Enums.EnumLoaiKhoDuocPham.KhoHanhChinh)
                {
                    inFoVatTus.Add(item.VatTuBenhVienId);
                }
            }

             queryVT = queryVT.Where(d=> inFoVatTus.Contains(d.VatTuBenhVienId));


            // DP
            var queryDP = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId &&
                            x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                            && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                            && (x.KhongLinhBu != true)
                            && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                            && x.KhoLinh.LoaiDuocPham == true
                            && x.SoLuong > 0
                            && x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && x.KhoLinh.LaKhoKSNK == true)
                .Select(item => new YeuCauLinhBuKSNKVo()
                {
                    Id = item.Id,
                    VatTuBenhVienId = item.DuocPhamBenhVienId,
                    LaBHYT = item.LaDuocPhamBHYT,
                    TenVatTu = item.Ten,
                    Nhom = item.LaDuocPhamBHYT ? "BHYT" : "Không BHYT",
                    //NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                    //HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                    //DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                    DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                    NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
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
                    //x.NongDoHamLuong,
                    //x.HoatChat,
                    //x.DuongDung,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat,
                    x.SoLuongTon
                })
                .Select(item => new YeuCauLinhBuKSNKVo()
                {
                    YeuCauLinhVatTuIdstring = string.Join(",", item.Select(x => x.Id)),
                    VatTuBenhVienId = item.First().VatTuBenhVienId,
                    LaBHYT = item.First().LaBHYT,
                    TenVatTu = item.First().TenVatTu,
                    Nhom = item.First().Nhom,
                    //NongDoHamLuong = item.First().NongDoHamLuong,
                    //HoatChat = item.First().HoatChat,
                    //DuongDung = item.First().DuongDung,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    KhongLinhBu = item.First().KhongLinhBu,
                    SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                    SoLuongDaLinhBu = item.Sum(x => x.SoLuongDaLinhBu).MathRoundNumber(2),
                    SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => (x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK || x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh) && 
                        x.DuocPhamBenhVienId == item.First().VatTuBenhVienId
                                    && (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 || x.NhapKhoDuocPhams.KhoId == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.LaDuocPhamBHYT == item.First().LaBHYT
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat).MathRoundNumber(2),
                    KhoLinhId = danhSachVatTuCanBuQueryInfo.KhoLinhId,
                    KhoBuId = danhSachVatTuCanBuQueryInfo.KhoBuId,
                    LoaiDuocPhamHayVatTu = true,
                    YeuCauLinhVatTuInFos = item.Select(f => new KhongYeuCauLinhBuKSNKVo()
                    {
                        YeuCauLinhId = f.Id,
                        LoaiDuocPhamHayVatTu = true
                    }).ToList()

                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu)
                .Where(o => (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 && o.SoLuongTon < 0.001) || (danhSachVatTuCanBuQueryInfo.KhoLinhId > 0 && o.SoLuongTon > 0)).Distinct();



            var query = queryVT.Union(queryDP);


            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
           
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageDanhSachChiTietKSNKCanBuForGrid(QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachKSNKCanBuQueryInfo>(queryInfo.AdditionalSearchString);

            var queryVT = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                            && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                            && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.All(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                            && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                            && x.KhongLinhBu != true
                            && x.SoLuong > 0
                            && x.YeuCauLinhVatTuId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && x.KhoLinh.LaKhoKSNK == true)
                .Select(item => new YeuCauLinhBuKSNKVo()
                {
                    Id = item.Id,
                    VatTuBenhVienId = item.VatTuBenhVienId,
                    LaBHYT = item.LaVatTuBHYT,
                    TenVatTu = item.Ten,
                    Nhom = item.LaVatTuBHYT ? "BHYT" : "Không BHYT",
                    DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                    HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                    NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                    SoLuongCanBu = item.SoLuong,
                    KhongLinhBu = item.KhongLinhBu,
                    SoLuongDaLinhBu = item.SoLuongDaLinhBu,
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
                .Select(item => new YeuCauLinhBuKSNKVo()
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
                        .Where(x => (x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK || x.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh) &&
                        x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                    && (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 || x.NhapKhoVatTu.KhoId == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.LaVatTuBHYT == item.First().LaBHYT
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat).MathRoundNumber(2),
                    KhoLinhId = danhSachVatTuCanBuQueryInfo.KhoLinhId,
                    KhoBuId = danhSachVatTuCanBuQueryInfo.KhoBuId,
                    LoaiDuocPhamHayVatTu = false,
                    YeuCauLinhVatTuInFos = item.Select(f => new KhongYeuCauLinhBuKSNKVo()
                    {
                        YeuCauLinhId = f.Id,
                        LoaiDuocPhamHayVatTu = false
                    }).ToList(),
                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu)
                .Where(o => (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 && o.SoLuongTon < 0.001) || (danhSachVatTuCanBuQueryInfo.KhoLinhId > 0 && o.SoLuongTon > 0)).Distinct();


            var yeuCauVTBenhVienIds = queryVT.Select(d => d.VatTuBenhVienId).ToList();
            var khoCap2Ids =
                _khoRepository.TableNoTracking.Where(p => (p.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK || p.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh) &&
                                                          (danhSachVatTuCanBuQueryInfo.KhoLinhId == null || p.Id == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                                          && p.LaKhoKSNK == true).Select(d => d.Id).ToList();

            var dpSLTonTheoVatTus = _nhapKhoVatTuChiTietRepository.TableNoTracking
                             .Where(x => yeuCauVTBenhVienIds.Contains(x.VatTuBenhVienId)
                                         && khoCap2Ids.Contains(x.NhapKhoVatTu.KhoId)
                                         && x.NhapKhoVatTu.DaHet != true
                                         //&& x.LaVatTuBHYT == item.LaDuocPhamVatTuBHYT
                                         && x.SoLuongDaXuat < x.SoLuongNhap)
                              .Select(d => new
                              {
                                  VatTuBenhVienId = d.VatTuBenhVienId,
                                  VatTuLoaiKhoHC = d.NhapKhoVatTu.Kho.LoaiKho,
                                  VatTuNhomHC = d.VatTuBenhVien.VatTus.NhomVatTuId
                              }).ToList();

            var inFoVatTus = new List<long>();
            foreach (var item in dpSLTonTheoVatTus)
            {
                if (item.VatTuLoaiKhoHC == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh && item.VatTuNhomHC == (long)Enums.EnumNhomVatTu.NhomHanhChinh)
                {
                    inFoVatTus.Add(item.VatTuBenhVienId);
                }
                if (item.VatTuLoaiKhoHC != Enums.EnumLoaiKhoDuocPham.KhoHanhChinh)
                {
                    inFoVatTus.Add(item.VatTuBenhVienId);
                }
            }

            queryVT = queryVT.Where(d => inFoVatTus.Contains(d.VatTuBenhVienId));

            // DP
            var queryDP = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId &&
                            x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                            && (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                            && (x.KhongLinhBu != true)
                            && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                            && x.KhoLinh.LoaiDuocPham == true
                            && x.SoLuong > 0
                            && x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            && x.KhoLinh.LaKhoKSNK == true)
                .Select(item => new YeuCauLinhBuKSNKVo()
                {
                    Id = item.Id,
                    VatTuBenhVienId = item.DuocPhamBenhVienId,
                    LaBHYT = item.LaDuocPhamBHYT,
                    TenVatTu = item.Ten,
                    Nhom = item.LaDuocPhamBHYT ? "BHYT" : "Không BHYT",
                    //NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                    //HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                    //DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                    DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                    NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
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
                    //x.NongDoHamLuong,
                    //x.HoatChat,
                    //x.DuongDung,
                    x.DonViTinh,
                    x.HangSanXuat,
                    x.NuocSanXuat,
                    x.SoLuongTon
                })
                .Select(item => new YeuCauLinhBuKSNKVo()
                {
                    YeuCauLinhVatTuIdstring = string.Join(",", item.Select(x => x.Id)),
                    VatTuBenhVienId = item.First().VatTuBenhVienId,
                    LaBHYT = item.First().LaBHYT,
                    TenVatTu = item.First().TenVatTu,
                    //Nhom = item.First().Nhom,
                    //NongDoHamLuong = item.First().NongDoHamLuong,
                    //HoatChat = item.First().HoatChat,
                    //DuongDung = item.First().DuongDung,
                    DonViTinh = item.First().DonViTinh,
                    HangSanXuat = item.First().HangSanXuat,
                    NuocSanXuat = item.First().NuocSanXuat,
                    KhongLinhBu = item.First().KhongLinhBu,
                    SoLuongCanBu = item.Sum(x => x.SoLuongCanBu).MathRoundNumber(2),
                    SoLuongDaLinhBu = item.Sum(x => x.SoLuongDaLinhBu).MathRoundNumber(2),
                    SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK &&
                        x.DuocPhamBenhVienId == item.First().VatTuBenhVienId
                                    && (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 || x.NhapKhoDuocPhams.KhoId == danhSachVatTuCanBuQueryInfo.KhoLinhId)
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.LaDuocPhamBHYT == item.First().LaBHYT
                                    && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat).MathRoundNumber(2),
                    KhoLinhId = danhSachVatTuCanBuQueryInfo.KhoLinhId,
                    KhoBuId = danhSachVatTuCanBuQueryInfo.KhoBuId,
                    LoaiDuocPhamHayVatTu = true

                })
                .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu)
                .Where(o => (danhSachVatTuCanBuQueryInfo.KhoLinhId == 0 && o.SoLuongTon < 0.001) || (danhSachVatTuCanBuQueryInfo.KhoLinhId > 0 && o.SoLuongTon > 0)).Distinct();

            var query = queryVT.Union(queryDP);

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }
        public async Task<GridDataSource> GetDanhSachChiTietYeuCauTheoKSNKCanBuForGrid(
            QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachKSNKCanBuChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            if(danhSachVatTuCanBuQueryInfo.LoaiDuocPhamHayVatTu == true)
            {
                var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => o.DuocPhamBenhVienId == danhSachVatTuCanBuQueryInfo.VatTuBenhVienId
                                && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && o.LaDuocPhamBHYT == danhSachVatTuCanBuQueryInfo.LaBHYT
                                && o.YeuCauLinhDuocPhamId == null
                                && o.KhongLinhBu != true
                                && o.KhoLinh.LoaiDuocPham == true
                                && o.SoLuong > 0
                                && o.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                                && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(a => a.YeuCauLinhDuocPham.DuocDuyet != null)
                                && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                && o.KhoLinh.LaKhoKSNK == true)
                            )
                            .OrderBy(x => x.ThoiDiemChiDinh)
                            .Select(s => new KSNKLinhBuCuaBNGridVo
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
            else
            {
                var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.VatTuBenhVienId == danhSachVatTuCanBuQueryInfo.VatTuBenhVienId
                                && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                && o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && o.LaVatTuBHYT == danhSachVatTuCanBuQueryInfo.LaBHYT
                                && o.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                                && o.KhongLinhBu != true
                                && o.SoLuong > 0
                                && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(a => a.YeuCauLinhVatTu.DuocDuyet != null)
                                && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                && o.KhoLinh.LaKhoKSNK == true)
                            )
                            .OrderBy(x => x.ThoiDiemChiDinh)
                            .Select(s => new KSNKLinhBuCuaBNGridVo
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
        }

        public async Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoKSNKCanBuForGrid(
            QueryInfo queryInfo)
        {
            var danhSachVatTuCanBuQueryInfo =
                JsonConvert.DeserializeObject<DanhSachKSNKCanBuChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            if (danhSachVatTuCanBuQueryInfo.LoaiDuocPhamHayVatTu == true)
            {
                var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => o.DuocPhamBenhVienId == danhSachVatTuCanBuQueryInfo.VatTuBenhVienId
                                && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu &&
                                o.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                                && o.LaDuocPhamBHYT == danhSachVatTuCanBuQueryInfo.LaBHYT
                                && o.YeuCauLinhDuocPhamId == null
                                && o.KhongLinhBu != true
                                && o.KhoLinh.LoaiDuocPham == true
                                && o.SoLuong > 0
                                && o.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                                && (!o.YeuCauLinhDuocPhamChiTiets.Any() || o.YeuCauLinhDuocPhamChiTiets.All(a => a.YeuCauLinhDuocPham.DuocDuyet != null)
                                && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                && o.KhoLinh.LaKhoKSNK == true)
                            )
                            .OrderBy(x => x.ThoiDiemChiDinh)
                            .Select(s => new KSNKLinhBuCuaBNGridVo
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
               return new GridDataSource { TotalRowCount = await query.CountAsync() };
            }
            else
            {
                var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.VatTuBenhVienId == danhSachVatTuCanBuQueryInfo.VatTuBenhVienId
                                && o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                                && o.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && o.LaVatTuBHYT == danhSachVatTuCanBuQueryInfo.LaBHYT
                                && o.KhoLinhId == danhSachVatTuCanBuQueryInfo.KhoBuId
                                && o.KhongLinhBu != true
                                && o.SoLuong > 0
                                && (!o.YeuCauLinhVatTuChiTiets.Any() || o.YeuCauLinhVatTuChiTiets.All(a => a.YeuCauLinhVatTu.DuocDuyet != null)
                                && (o.SoLuongDaLinhBu == null || o.SoLuongDaLinhBu < o.SoLuong)
                                && o.KhoLinh.LaKhoKSNK == true)
                            )
                            .OrderBy(x => x.ThoiDiemChiDinh)
                            .Select(s => new KSNKLinhBuCuaBNGridVo
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
                return new GridDataSource { TotalRowCount = await query.CountAsync() };
            }
            
        }
        public List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var danhSachVatTuCanBu = GetDanhSachKSNKCanBu(new QueryInfo());
            return danhSachVatTuCanBu.Any() ? danhSachVatTuCanBu.GroupBy(o => new { o.KhoLinhId, o.KhoLinh }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.KhoLinhId,
                DisplayName = s.Key.KhoLinh
            }).ToList() : new List<LookupItemVo>();
        }
        public List<LookupItemVo> GetTatCaKhoLinhVeCuaNhanVienLoginLinhBu(LookupQueryInfo model)
        {
            var danhSachVatTuCanBu = GetDanhSachKSNKCanBu(new QueryInfo());
            return danhSachVatTuCanBu.Any() ? danhSachVatTuCanBu.GroupBy(o => new { o.KhoBuId, o.KhoBu }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.KhoBuId,
                DisplayName = s.Key.KhoBu
            }).ToList() : new List<LookupItemVo>();
        }
        #endregion

        public async Task UpdateYeuCauKSNKBenhVien(List<KhongYeuCauLinhBuKSNKVo> yeuCauLinhVatTuIdstring)
        {
            if(yeuCauLinhVatTuIdstring.Where(d=>d.LoaiDuocPhamHayVatTu == true).Count() != 0)
            {
                var yeuCauBenhVienIds = yeuCauLinhVatTuIdstring.Where(d => d.LoaiDuocPhamHayVatTu == true).Select(d => d.YeuCauLinhId).ToList();
                var chiTiets = _yeuCauDuocPhamBenhVienRepository.Table.Where(p => yeuCauBenhVienIds.Any(x => x == p.Id)).ToList();
                long userLogin = _userAgentHelper.GetCurrentUserId();

                foreach (var item in chiTiets)
                {
                    item.KhongLinhBu = true;
                    item.NgayDanhDauKhongBu = DateTime.Now;
                    item.NguoiDanhDauKhongBuId = userLogin;
                }
                await _yeuCauDuocPhamBenhVienRepository.UpdateAsync(chiTiets);
            }
            if (yeuCauLinhVatTuIdstring.Where(d => d.LoaiDuocPhamHayVatTu == false).Count() != 0)
            {
                var yeuCauVatTuIds = yeuCauLinhVatTuIdstring.Where(d=>d.LoaiDuocPhamHayVatTu == false).Select(d => d.YeuCauLinhId).ToList();
                var chiTiets = _yeuCauVatTuBenhVienRepository.Table.Where(p => yeuCauVatTuIds.Any(x => x == p.Id)).ToList();
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
}
