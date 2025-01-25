using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject.XuatKhoKSNK;
using XuatKhoKhacLookupItem = Camino.Core.Domain.ValueObject.XuatKhos.XuatKhoKhacLookupItem;

namespace Camino.Services.YeuCauHoanTra.KSNK
{
    public partial class YeuCauHoanTraKSNKService
    {
        public async Task<GridDataSource> GetAllDpVtKsnkData(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;
            var lstDaChon = new List<DaSuaSoLuongXuat>();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
                lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|")[2]);
            }

            var allDataNhapDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT,
                    o.NgayNhap,
                    o.VAT,
                    o.TiLeTheoThapGia
                }).GroupBy(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT,
                }, o => o,
                (k, v) => new KSNKHoanTraGridVo
                {
                    DuocPhamVatTuId = k.DuocPhamBenhVienId,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    //DonGia = k.DonGiaTonKho,
                    SoLo = k.Solo,
                    HanSuDung = k.HanSuDung,
                    LaVatTuBHYT = k.LaDuocPhamBHYT,
                    NgayNhap = v.First().NgayNhap,
                    Vat = v.First().VAT,
                    TiLeThapGia = v.First().TiLeTheoThapGia
                }).Where(o => o.SoLuongTon >= 0.01).ToList();

            var allDataNhapVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.VatTuBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT,
                    o.NgayNhap,
                    o.VAT,
                    o.TiLeTheoThapGia
                }).GroupBy(o => new
                {
                    o.VatTuBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT,
                }, o => o,
                    (k, v) => new KSNKHoanTraGridVo
                    {
                        DuocPhamVatTuId = k.VatTuBenhVienId,
                        LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                        SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        //DonGia = k.DonGiaTonKho,
                        SoLo = k.Solo,
                        HanSuDung = k.HanSuDung,
                        LaVatTuBHYT = k.LaVatTuBHYT,
                        NgayNhap = v.First().NgayNhap,
                        Vat = v.First().VAT,
                        TiLeThapGia = v.First().TiLeTheoThapGia
                    }).Where(o => o.SoLuongTon >= 0.01).ToList();

            //var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == khoXuatId
            // && p.SoLuongNhap - p.SoLuongDaXuat > 0)
            //               .Select(s => new KSNKHoanTraGridVo
            //               {
            //                   //Id = s.Id + "," + s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription() + "," + (s.LaVatTuBHYT ? "true" : "false"),
            //                   TenVatTu = s.VatTuBenhVien.VatTus.Ten,
            //                   DVT = s.VatTuBenhVien.VatTus.DonViTinh,
            //                   LaVatTuBHYT = s.LaVatTuBHYT,
            //                   HanSuDung = s.HanSuDung,
            //                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
            //                   SoLo = s.Solo,
            //                   MaVatTu = s.VatTuBenhVien.Ma,
            //                   NgayNhap = s.NgayNhap,
            //                   Vat = s.VAT,
            //                   TiLeThapGia = s.TiLeTheoThapGia
            //               });
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DVT, g => g.MaVatTu, g => g.SoLo);

            //if (!string.IsNullOrEmpty(lstIdString))
            //{
            //    var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
            //    query = query.Where(p => !lstId.Contains(p.Id));
            //}

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();
            //await Task.WhenAll(countTask, queryTask);
            var duocPhamBenhVienIds = allDataNhapDuocPham.Select(o => o.DuocPhamVatTuId).Distinct().ToList();
            var vatTuBenhVienIds = allDataNhapVatTu.Select(o => o.DuocPhamVatTuId).Distinct().ToList();

            var thongTinDuocPhams = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => duocPhamBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.DuocPham.Ten,
                    Nhom = (o.DuocPhamBenhVienPhanNhom != null ? o.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM"),
                    DVT = o.DuocPham.DonViTinh.Ten
                }).ToList();
            var thongTinVatTus = _vatTuBenhVienRepository.TableNoTracking
                .Where(o => vatTuBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.VatTus.Ten,
                    Nhom = o.VatTus.NhomVatTu.Ten,
                    DVT = o.VatTus.DonViTinh
                }).ToList();

            List<string> lstId = new List<string>();
            if (!string.IsNullOrEmpty(lstIdString))
            {
                lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
            }
            var allData = new List<KSNKHoanTraGridVo>();
            foreach (var hoanTraGridVo in allDataNhapDuocPham)
            {
                var thongTinDuocPham = thongTinDuocPhams.First(o => o.Id == hoanTraGridVo.DuocPhamVatTuId);
                hoanTraGridVo.MaVatTu = thongTinDuocPham.Ma;
                hoanTraGridVo.TenVatTu = thongTinDuocPham.Ten;
                hoanTraGridVo.Nhom = thongTinDuocPham.Nhom;
                hoanTraGridVo.DVT = thongTinDuocPham.DVT;
                if (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                    (hoanTraGridVo.TenVatTu != null && hoanTraGridVo.TenVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (hoanTraGridVo.MaVatTu != null && hoanTraGridVo.MaVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (hoanTraGridVo.SoLo != null && hoanTraGridVo.SoLo.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())))
                {
                    if (!lstId.Contains(hoanTraGridVo.Id))
                        allData.Add(hoanTraGridVo);
                }
            }

            foreach (var dpVtKsnkXuatGridVo in allDataNhapVatTu)
            {
                var thongTinVatTu = thongTinVatTus.First(o => o.Id == dpVtKsnkXuatGridVo.DuocPhamVatTuId);
                dpVtKsnkXuatGridVo.MaVatTu = thongTinVatTu.Ma;
                dpVtKsnkXuatGridVo.TenVatTu = thongTinVatTu.Ten;
                dpVtKsnkXuatGridVo.Nhom = thongTinVatTu.Nhom;
                dpVtKsnkXuatGridVo.DVT = thongTinVatTu.DVT;
                if (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                    (dpVtKsnkXuatGridVo.TenVatTu != null && dpVtKsnkXuatGridVo.TenVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.MaVatTu != null && dpVtKsnkXuatGridVo.MaVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (dpVtKsnkXuatGridVo.SoLo != null && dpVtKsnkXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())))
                {
                    if (!lstId.Contains(dpVtKsnkXuatGridVo.Id))
                        allData.Add(dpVtKsnkXuatGridVo);
                }
            }

            var dataReturn = allData.OrderBy(o => o.MaVatTu).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            var stt = 1;
            foreach (var item in dataReturn)
            {
                item.SoLuongXuat = item.SoLuongTon;
                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = dataReturn, TotalRowCount = allData.Count };

            //var stt = 1;
            //foreach (var item in queryTask.Result)
            //{
            //    var id = long.Parse(item.Id.Split(",")[0]);
            //    var nhapKho = _nhapKhoVatTuChiTietRepository.TableNoTracking.First(p => p.Id == id);

            //    item.SoLuongTon = nhapKho.SoLuongNhap - nhapKho.SoLuongDaXuat;
            //    if (lstDaChon.Any(p => p.Id == item.Id))
            //    {
            //        item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
            //    }
            //    else
            //    {
            //        item.SoLuongXuat = item.SoLuongTon;
            //    }

            //    item.STT = stt;
            //    stt++;
            //}

            //return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetAllVatTuData(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;
            var lstDaChon = new List<DaSuaSoLuongXuat>();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
                lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|")[2]);
            }

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == khoXuatId
             && p.SoLuongNhap - p.SoLuongDaXuat > 0)
                           .Select(s => new KSNKHoanTraGridVo
                           {
                               //Id = s.Id + "," + s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription() + "," + (s.LaVatTuBHYT ? "true" : "false"),
                               TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                               DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                               LaVatTuBHYT = s.LaVatTuBHYT,
                               HanSuDung = s.HanSuDung,
                               LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                               SoLo = s.Solo,
                               MaVatTu = s.VatTuBenhVien.Ma,
                               NgayNhap = s.NgayNhap,
                               Vat = s.VAT,
                               TiLeThapGia = s.TiLeTheoThapGia
                           });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DVT, g => g.MaVatTu, g => g.SoLo);

            if (!string.IsNullOrEmpty(lstIdString))
            {
                var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
                query = query.Where(p => !lstId.Contains(p.Id));
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                var id = long.Parse(item.Id.Split(",")[0]);
                var nhapKho = _nhapKhoVatTuChiTietRepository.TableNoTracking.First(p => p.Id == id);

                item.SoLuongTon = nhapKho.SoLuongNhap - nhapKho.SoLuongDaXuat;
                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
                else
                {
                    item.SoLuongXuat = item.SoLuongTon;
                }

                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetAllVatTuTotal(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
            }

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == khoXuatId
            && p.SoLuongNhap - p.SoLuongDaXuat > 0)
                          .Select(s => new KSNKHoanTraGridVo
                          {
                              //Id = s.Id + "," + s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription() + "," + (s.LaVatTuBHYT ? "true" : "false"),
                              TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                              DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                              LaVatTuBHYT = s.LaVatTuBHYT,
                              HanSuDung = s.HanSuDung,
                              LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                              SoLo = s.Solo,
                              MaVatTu = s.VatTuBenhVien.Ma,
                              NgayNhap = s.NgayNhap,
                              Vat = s.VAT,
                              TiLeThapGia = s.TiLeTheoThapGia
                          });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DVT, g => g.MaVatTu, g => g.SoLo);

            if (!string.IsNullOrEmpty(lstIdString))
            {
                var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
                query = query.Where(p => !lstId.Contains(p.Id));
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }



        public async Task<List<XuatKhoKhacLookupItem>> GetKhoDPvaVTKSNK(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var khoId = CommonHelper.GetIdFromRequestDropDownList(model);
            var khos = await _khoRepository.TableNoTracking
                .Where(p => p.KhoaPhongId == phongBenhVien.KhoaPhongId
                        && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
                        && p.LaKhoKSNK == true && (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoKSNK))
                        .ApplyLike(model.Query, p => p.Ten)
                        .Select(item => new XuatKhoKhacLookupItem
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            LoaiKho = item.LoaiKho
                        })
                        .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                        .Take(model.Take).ToListAsync();

            return khos;
        }

        public async Task<List<LookupItemVo>> GetKhoVatTuHoanTra(DropDownListRequestModel model)
        {
            var khoXuatId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (khoXuatId == 0) return new List<LookupItemVo>();
            var khoXuat = _khoRepository.TableNoTracking.First(p => p.Id == khoXuatId);

            var khoVatTus =  _khoRepository.TableNoTracking.Where(p => (khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoKSNK))
                                                                       || (khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoKSNK && (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoHanhChinh || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.Id == (long)Enums.EnumKhoDuocPham.KhoHoaChat)))
                            .ApplyLike(model.Query, p => p.Ten)
                            .Select(item => new LookupItemVo
                            {
                                DisplayName = item.Ten,
                                KeyId = item.Id,
                            }).Take(model.Take);
            return await khoVatTus.ToListAsync();
        }

        public async Task<List<KSNKHoanTraGridVo>> GetVatTuOnGroup(Enums.LoaiSuDung groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon)
        {
            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == khoXuatId
            && p.VatTuBenhVien.LoaiSuDung == groupId
             && p.SoLuongNhap - p.SoLuongDaXuat > 0)
                           .Select(s => new KSNKHoanTraGridVo
                           {
                               //Id = s.Id + "," + s.VatTuBenhVien.LoaiSuDung.GetDescription() + "," + (s.LaVatTuBHYT ? "true" : "false"),
                               TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                               DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                               LaVatTuBHYT = s.LaVatTuBHYT,
                               HanSuDung = s.HanSuDung,
                               LoaiSuDung = s.VatTuBenhVien.LoaiSuDung,
                               SoLo = s.Solo,
                               MaVatTu = s.VatTuBenhVien.Ma,
                               NgayNhap = s.NgayNhap,
                               Vat = s.VAT,
                               TiLeThapGia = s.TiLeTheoThapGia
                           });

            if (searchString != "undefined")
            {
                query = query.ApplyLike(searchString, g => g.TenVatTu, g => g.DVT, g => g.MaVatTu, g => g.SoLo);
            }
            var result = query.ToList();

            foreach (var item in result)
            {
                var id = long.Parse(item.Id.Split(",")[0]);
                var nhapKho = await _nhapKhoVatTuChiTietRepository.TableNoTracking.FirstAsync(p => p.Id == id);

                item.SoLuongTon = nhapKho.SoLuongNhap - nhapKho.SoLuongDaXuat;

                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
                else
                {
                    item.SoLuongXuat = item.SoLuongTon;
                }
            }

            return result.ToList();
        }


        public async Task<YeuCauTraVatTuChiTiet> GetVatTu(ThemKSNKHoanTra model)
        {
            var result = new YeuCauTraVatTuChiTiet();
            var nhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking.FirstAsync(p => p.Id == model.NhapKhoVatTuChiTietId);

            result.SoLuongTra = model.SoLuongXuat ?? 0;
            result.HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId;
            result.LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT;
            result.NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien;
            result.Solo = nhapKhoVatTuChiTiet.Solo;
            result.HanSuDung = nhapKhoVatTuChiTiet.HanSuDung;
            result.DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap;
            result.TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia;
            result.VAT = nhapKhoVatTuChiTiet.VAT;
            result.MaVach = nhapKhoVatTuChiTiet.MaVach;
            result.MaRef = nhapKhoVatTuChiTiet.MaRef;
            result.KhoViTriId = nhapKhoVatTuChiTiet.KhoViTriId;
            result.VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId;

            var xuatKhoChiTiet = new XuatKhoVatTuChiTiet();
            xuatKhoChiTiet.VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId;
            xuatKhoChiTiet.NgayXuat = DateTime.Now;

            var xuatKhoViTri = new XuatKhoVatTuChiTietViTri();
            xuatKhoViTri.XuatKhoVatTuChiTiet = xuatKhoChiTiet;
            xuatKhoViTri.SoLuongXuat = model.SoLuongXuat ?? 0;
            xuatKhoViTri.NgayXuat = DateTime.Now;
            xuatKhoViTri.NhapKhoVatTuChiTietId = model.NhapKhoVatTuChiTietId ?? 0;

            result.XuatKhoVatTuChiTietViTri = xuatKhoViTri;

            return result;
        }


        public async Task UpdateGiaChoNhapKhoChiTiet(double soLuongXuat, long id)
        {
            var entity = await _nhapKhoVatTuChiTietRepository.Table.FirstAsync(p => p.Id == id);
            entity.SoLuongDaXuat = entity.SoLuongDaXuat + soLuongXuat;
            await _nhapKhoVatTuChiTietRepository.UpdateAsync(entity);
        }

        public async Task UpdateSlXuatNhapKhoChiTiet(double soLuongXuat, long id)
        {
            BaseRepository.AutoCommitEnabled = false;
            var entity = await _nhapKhoVatTuChiTietRepository.Table.FirstAsync(p => p.Id == id);
            entity.SoLuongDaXuat = entity.SoLuongDaXuat + soLuongXuat;
            await _nhapKhoVatTuChiTietRepository.UpdateAsync(entity);
        }

        public async Task<bool> CheckValidSlTon(List<ThemKSNKHoanTra> lstModelThemVatTuHoanTra, long id)
        {
            foreach (var vatTuHoanTra in lstModelThemVatTuHoanTra)
            {
                var ycTraDp =
                    await BaseRepository.GetByIdAsync(id,
                        s => s.Include(w => w.YeuCauTraVatTuChiTiets)
                            .ThenInclude(w => w.XuatKhoVatTuChiTietViTri).ThenInclude(w => w.NhapKhoVatTuChiTiet));
                var slXuatCurrent = ycTraDp.YeuCauTraVatTuChiTiets.FirstOrDefault(e =>
                    e.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId
                    == vatTuHoanTra.NhapKhoVatTuChiTietId)?.XuatKhoVatTuChiTietViTri?.SoLuongXuat;
                var nhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.GetByIdAsync(vatTuHoanTra
                    .NhapKhoVatTuChiTietId.GetValueOrDefault());
                var soLuongTon = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;

                if (slXuatCurrent == null)
                {
                    if (soLuongTon < vatTuHoanTra.SoLuongXuat.GetValueOrDefault())
                    {
                        return false;
                    }
                }
                else
                {
                    if (vatTuHoanTra.SoLuongXuat.GetValueOrDefault() - slXuatCurrent > soLuongTon)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
