using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.YeuCauHoanTra.VatTu
{
    public partial class YeuCauHoanTraVatTuService
    {
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
                           .Select(s => new VatTuHoanTraGridVo
                           {
                               Id = s.Id + "," + s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription() + "," + (s.LaVatTuBHYT ? "true" : "false"),
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
                          .Select(s => new VatTuHoanTraGridVo
                          {
                              Id = s.Id + "," + s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription() + "," + (s.LaVatTuBHYT ? "true" : "false"),
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



        public async Task<List<LookupItemVo>> GetKhoVatTu(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var khoVatTus = _khoRepository.TableNoTracking.Where(p =>
                           (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe
                         || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin
                           || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc)
                           && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
                           && p.LoaiVatTu == true)
                            .ApplyLike(model.Query, p => p.Ten)
                            .Select(item => new LookupItemVo
                            {
                                DisplayName = item.Ten,
                                KeyId = item.Id,
                            }).Take(model.Take);
            return await khoVatTus.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetKhoVatTuHoanTra(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var khoVatTus =  _khoRepository.TableNoTracking.Where(p =>
                            (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                            // && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
                            //&& p.LoaiVatTu == true
                            )
                            .ApplyLike(model.Query, p => p.Ten)
                            .Select(item => new LookupItemVo
                            {
                                DisplayName = item.Ten,
                                KeyId = item.Id,
                            }).Take(model.Take);
            return await khoVatTus.ToListAsync();
        }

        public async Task<List<VatTuHoanTraGridVo>> GetVatTuOnGroup(Enums.LoaiSuDung groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon)
        {
            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == khoXuatId
            && p.VatTuBenhVien.LoaiSuDung == groupId
             && p.SoLuongNhap - p.SoLuongDaXuat > 0)
                           .Select(s => new VatTuHoanTraGridVo
                           {
                               Id = s.Id + "," + s.VatTuBenhVien.LoaiSuDung.GetDescription() + "," + (s.LaVatTuBHYT ? "true" : "false"),
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


        public async Task<YeuCauTraVatTuChiTiet> GetVatTu(ThemVatTuHoanTra model)
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

        public async Task<bool> CheckValidSlTon(List<ThemVatTuHoanTra> lstModelThemVatTuHoanTra, long id)
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
