using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauHoanTra.DuocPham
{
    public partial class YeuCauHoanTraDuocPhamService
    {
        public async Task<GridDataSource> GetAllDuocPhamData(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;
            var lstDaChon = new List<DaSuaSoLuongXuat>();
            var lstDaChon2 = new List<DuocPhamDaChonVo>();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
                lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|")[2]);
                //lstDaChon2 = JsonConvert.DeserializeObject<List<DuocPhamDaChonVo>>(queryInfo.AdditionalSearchString.Split("|")[3]);
            }


            var query = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(p => p.NhapKhoDuocPhams.KhoId == khoXuatId
             && p.SoLuongNhap - p.SoLuongDaXuat > 0)
                           .Select(s => new DuocPhamHoanTraGridVo
                           {
                               NhaKhoDuocPhamChiTietId = s.Id,
                               DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                               Id = s.Id + "," + (s.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId ?? 0) + "," + (s.LaDuocPhamBHYT ? "true" : "false"),
                               TenDuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                               DVT = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                               LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                               HanSuDung = s.HanSuDung,
                               DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                               TenNhom = s.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                               SoLo = s.Solo,
                               MaDuocPham = s.DuocPhamBenhViens.Ma,
                               SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
                               NgayNhap = s.NgayNhap,
                           });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenDuocPham,
                g => g.DVT,
                g => g.SoDangKy,
                g => g.MaDuocPham,
                g => g.SoLo);

            if (!string.IsNullOrEmpty(lstIdString))
            {
                var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
                query = query.Where(p => !lstId.Contains(p.Id));
            }

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();
            //await Task.WhenAll(countTask, queryTask);
            var queryResult = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();


            var stt = 1;
            var nhapKhoDucPhamChiTietIds = queryResult.Select(c => c.NhaKhoDuocPhamChiTietId).ToList();
            var nhapKhoDucPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => nhapKhoDucPhamChiTietIds.Contains(x.Id)).ToList();
            foreach (var item in queryResult)
            {
                //
                var id = long.Parse(item.Id.Split(",")[0]);
                var nhapKho = nhapKhoDucPhamChiTiets.FirstOrDefault(p => p.Id == id);
                if (nhapKho != null)
                {
                    item.SoLuongTon = nhapKho.SoLuongNhap - nhapKho.SoLuongDaXuat;
                    if (lstDaChon.Any(p => p.Id == item.Id))
                    {
                        item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                    }
                    else
                    {
                        item.SoLuongXuat = item.SoLuongTon;
                    }
                    //
                    //item.Id = item.Id.Split(",")[0] + "," + nhapKho. + "," item.Id.Split(",")[2];
                    //
                    item.STT = stt;
                    stt++;
                }
            }
            //, x.NgayNhap
            //var dataGroup = queryResult.GroupBy(x => new { x.MaDuocPham, x.TenDuocPham, x.LaDuocPhamBHYT, x.SoLo, x.HanSuDung, x.SoDangKy })
            //        .Select(item => new DuocPhamHoanTraGridVo()
            //        {
            //            Id = item.First().Id,
            //            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
            //            TenDuocPham = item.First().TenDuocPham,
            //            DVT = item.First().DVT,
            //            LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
            //            HanSuDung = item.First().HanSuDung,
            //            DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVienPhanNhomId,
            //            TenNhom = item.First().TenNhom,
            //            SoLo = item.First().SoLo,
            //            MaDuocPham = item.First().MaDuocPham,
            //            SoDangKy = item.First().SoDangKy,
            //            NgayNhap = item.First().NgayNhap,
            //            SoLuongTon = item.Sum(x => x.SoLuongTon),
            //            SoLuongXuat = item.Sum(x => x.SoLuongXuat),
            //        }).ToArray();
            //if (lstDaChon2.Any())
            //{
            //    dataGroup = dataGroup.Where(g => !lstDaChon2.Any(x => x.DuocPhamBenhVienId == g.DuocPhamBenhVienId 
            //                                                       && x.MaDuocPham == g.MaDuocPham 
            //                                                       && x.TenDuocPham == g.TenDuocPham 
            //                                                       && x.HanSuDung == g.HanSuDung 
            //                                                       && x.SoDangKy == g.SoDangKy
            //                                                       && x.SoLo == g.SoLo
            //                                                       && x.LaDuocPhamBHYT == g.LaDuocPhamBHYT)).ToArray();
            //}
            return new GridDataSource { Data = queryResult, TotalRowCount = queryResult.Count() };

            //return new GridDataSource { Data = dataGroup, TotalRowCount = dataGroup.Count() };

        }

        public async Task<GridDataSource> GetAllDuocPhamTotal(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;
            var lstDaChon = new List<DaSuaSoLuongXuat>();
            var lstDaChon2 = new List<DuocPhamDaChonVo>();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
                lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|")[2]);
                //lstDaChon2 = JsonConvert.DeserializeObject<List<DuocPhamDaChonVo>>(queryInfo.AdditionalSearchString.Split("|")[3]);
            }

            //if (khoXuatId == 0)
            //{
            //    return new GridDataSource { Data = null, TotalRowCount = 0 };
            //}

            var query = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(p => p.NhapKhoDuocPhams.KhoId == khoXuatId
            && p.SoLuongNhap - p.SoLuongDaXuat > 0)
                          .Select(s => new DuocPhamHoanTraGridVo
                          {
                              DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                              Id = s.Id + "," + (s.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId ?? 0) + "," + (s.LaDuocPhamBHYT ? "true" : "false"),
                              TenDuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                              DVT = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                              LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                              HanSuDung = s.HanSuDung,
                              DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                              TenNhom = s.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                              SoLo = s.Solo,
                              MaDuocPham = s.DuocPhamBenhViens.Ma,
                              SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
                              NgayNhap = s.NgayNhap,
                          });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenDuocPham,
                g => g.DVT,
                g => g.SoDangKy,
                g => g.MaDuocPham,
                g => g.SoLo);
            if (!string.IsNullOrEmpty(lstIdString))
            {
                var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
                query = query.Where(p => !lstId.Contains(p.Id));
            }
            //var dataGroup = query.GroupBy(x => new { x.MaDuocPham, x.TenDuocPham, x.LaDuocPhamBHYT, x.SoLo, x.HanSuDung, x.SoDangKy })
            //       .Select(item => new DuocPhamHoanTraGridVo()
            //       {
            //           Id = item.First().Id,
            //           DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
            //           TenDuocPham = item.First().TenDuocPham,
            //           DVT = item.First().DVT,
            //           LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
            //           HanSuDung = item.First().HanSuDung,
            //           DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVienPhanNhomId,
            //           TenNhom = item.First().TenNhom,
            //           SoLo = item.First().SoLo,
            //           MaDuocPham = item.First().MaDuocPham,
            //           SoDangKy = item.First().SoDangKy,
            //           NgayNhap = item.First().NgayNhap,
            //           SoLuongTon = item.Sum(x => x.SoLuongTon),
            //           SoLuongXuat = item.Sum(x => x.SoLuongXuat),
            //       }).ToArray();
            //if (lstDaChon2.Any())
            //{
            //    dataGroup = dataGroup.Where(g => !lstDaChon2.Any(x => x.DuocPhamBenhVienId == g.DuocPhamBenhVienId
            //                                                       && x.MaDuocPham == g.MaDuocPham
            //                                                       && x.TenDuocPham == g.TenDuocPham
            //                                                       && x.HanSuDung == g.HanSuDung
            //                                                       && x.SoLo == g.SoLo
            //                                                       && x.SoDangKy == g.SoDangKy
            //                                                       && x.LaDuocPhamBHYT == g.LaDuocPhamBHYT)).ToArray();
            //}

            //var countTask = dataGroup.AsQueryable().CountAsync();
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }



        public async Task<List<LookupItemVo>> GetKhoDuocPham(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var lstEntity = await _khoRepository.TableNoTracking.Where(p =>
            (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe
                         || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin
            || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc)
            && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
            && EF.Functions.Like(p.Ten, $"%{model.Query}%"))
                .Take(1000)
                .ToListAsync();
            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetKhoDuocHoanTra(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var lstEntity = await _khoRepository.TableNoTracking.Where(p =>
            (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
            && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
            && EF.Functions.Like(p.Ten, $"%{model.Query}%"))
                .Take(1000)
                .ToListAsync();
            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetKhoLoaiDuocPham(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var khos = _khoRepository.TableNoTracking.Where(p =>
                                   (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe
                                   || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc
                                   || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin)
                                   && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId) && p.LoaiDuocPham == true)
                                    .ApplyLike(model.Query, p => p.Ten)
                                    .Select(item => new LookupItemVo
                                    {
                                        DisplayName = item.Ten,
                                        KeyId = item.Id,
                                    }).Take(model.Take);
            return await khos.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetKhoLoaiDuocHoanTra(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var khos = _khoRepository.TableNoTracking.Where(p =>
                                   (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
                                     //&& p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
                                     //&& p.LoaiDuocPham == true
                                     )
                                      .ApplyLike(model.Query, p => p.Ten)
                                      .Select(item => new LookupItemVo
                                      {
                                          DisplayName = item.Ten,
                                          KeyId = item.Id,
                                      }).Take(model.Take);
            return await khos.ToListAsync();
        }

        public async Task<List<DuocPhamHoanTraGridVo>> GetDuocPhamOnGroup(long groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon)
        {
            var query = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(p => p.NhapKhoDuocPhams.KhoId == khoXuatId
            && p.DuocPhamBenhVienPhanNhomId == groupId
             && p.SoLuongNhap - p.SoLuongDaXuat > 0)
                           .Select(s => new DuocPhamHoanTraGridVo
                           {
                               Id = s.Id + "," + groupId + "," + (s.LaDuocPhamBHYT ? "true" : "false"),
                               TenDuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                               DVT = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                               LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                               HanSuDung = s.HanSuDung,
                               DuocPhamBenhVienPhanNhomId = groupId,
                               TenNhom = s.DuocPhamBenhVienPhanNhom != null ? s.DuocPhamBenhVienPhanNhom.Ten : "",
                               SoLo = s.Solo,
                               MaDuocPham = s.DuocPhamBenhViens.Ma,
                               SoDangKy = s.DuocPhamBenhViens.DuocPham.SoDangKy,
                               NgayNhap = s.NgayNhap,
                           });

            if (searchString != "undefined")
            {
                query = query.ApplyLike(searchString,
                    g => g.TenDuocPham,
                g => g.DVT,
                g => g.SoDangKy,
                g => g.MaDuocPham,
                g => g.SoLo);
            }
            var result = query.ToList();

            foreach (var item in result)
            {
                //
                var id = long.Parse(item.Id.Split(",")[0]);
                //
                var nhapKho = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                   .Include(p => p.DuocPhamBenhVienPhanNhom)
                   .First(p => p.Id == id)
                   ;

                item.SoLuongTon = nhapKho.SoLuongNhap - nhapKho.SoLuongDaXuat;
                //item.SoLuongXuat = item.SoLuongTon;
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


        public async Task<YeuCauTraDuocPhamChiTiet> GetDuocPham(ThemDuocPhamHoanTra model)
        {
            var result = new YeuCauTraDuocPhamChiTiet();
            var nhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.FirstAsync(p => p.Id == model.NhapKhoDuocPhamChiTietId);

            result.SoLuongTra = model.SoLuongXuat ?? 0;
            result.HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId;
            result.LaDuocPhamBHYT = nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT;
            result.DuocPhamBenhVienPhanNhomId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId;
            result.NgayNhapVaoBenhVien = nhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien;
            result.Solo = nhapKhoDuocPhamChiTiet.Solo;
            result.HanSuDung = nhapKhoDuocPhamChiTiet.HanSuDung;
            result.DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap;
            result.TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia;
            result.VAT = nhapKhoDuocPhamChiTiet.VAT;
            result.MaVach = nhapKhoDuocPhamChiTiet.MaVach;
            result.MaRef = nhapKhoDuocPhamChiTiet.MaRef;
            result.KhoViTriId = nhapKhoDuocPhamChiTiet.KhoViTriId;
            result.DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId;

            var xuatKhoChiTiet = new XuatKhoDuocPhamChiTiet();
            xuatKhoChiTiet.DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId;
            xuatKhoChiTiet.NgayXuat = DateTime.Now;

            var xuatKhoViTri = new XuatKhoDuocPhamChiTietViTri();
            xuatKhoViTri.XuatKhoDuocPhamChiTiet = xuatKhoChiTiet;
            xuatKhoViTri.SoLuongXuat = model.SoLuongXuat ?? 0;
            xuatKhoViTri.NgayXuat = DateTime.Now;
            xuatKhoViTri.NhapKhoDuocPhamChiTietId = model.NhapKhoDuocPhamChiTietId ?? 0;

            result.XuatKhoDuocPhamChiTietViTri = xuatKhoViTri;

            return result;
        }


        public async Task UpdateGiaChoNhapKhoChiTiet(double soLuongXuat, long id)
        {
            var entity = await _nhapKhoDuocPhamChiTietRepository.Table.FirstAsync(p => p.Id == id);
            entity.SoLuongDaXuat = entity.SoLuongDaXuat + soLuongXuat;
            await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(entity);
        }

        public async Task UpdateSlXuatNhapKhoChiTiet(double soLuongXuat, long id)
        {
            BaseRepository.AutoCommitEnabled = false;
            var entity = await _nhapKhoDuocPhamChiTietRepository.Table.FirstAsync(p => p.Id == id);
            entity.SoLuongDaXuat = entity.SoLuongDaXuat + soLuongXuat;
            await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(entity);
        }

        public async Task<bool> CheckValidSlTon(List<ThemDuocPhamHoanTra> lstModelThemDuocPhamHoanTra, long id)
        {
            foreach (var duocPhamHoanTra in lstModelThemDuocPhamHoanTra)
            {
                var ycTraDp =
                    await BaseRepository.GetByIdAsync(id,
                        s => s.Include(w => w.YeuCauTraDuocPhamChiTiets)
                            .ThenInclude(w => w.XuatKhoDuocPhamChiTietViTri).ThenInclude(w => w.NhapKhoDuocPhamChiTiet));
                var slXuatCurrent = ycTraDp.YeuCauTraDuocPhamChiTiets.FirstOrDefault(e =>
                    e.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId
                    == duocPhamHoanTra.NhapKhoDuocPhamChiTietId)?.XuatKhoDuocPhamChiTietViTri?.SoLuongXuat;
                var nhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.GetByIdAsync(duocPhamHoanTra
                    .NhapKhoDuocPhamChiTietId.GetValueOrDefault());
                var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;

                if (slXuatCurrent == null)
                {
                    if (soLuongTon < duocPhamHoanTra.SoLuongXuat.GetValueOrDefault())
                    {
                        return false;
                    }
                }
                else
                {
                    if (duocPhamHoanTra.SoLuongXuat.GetValueOrDefault() - slXuatCurrent > soLuongTon)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
