using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial class YeuCauLinhKSNKService
    {
        public async Task<List<LookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaPhongId = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == noiLamViecCurrentId).Select(z => z.KhoaPhongId).FirstOrDefault();
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                        .Where(p => p.NhanVienId == userCurrentId &&
                               p.Kho.KhoaPhongId == khoaPhongId &&
                               p.Kho.LaKhoKSNK == true) // Kho KSNK 
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.KhoId,
                            DisplayName = s.Kho.Ten
                        })
                        .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);

            return await result.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetKhoLinh(DropDownListRequestModel queryInfo)
        {
            var khoLinhVeId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            if (khoLinhVeId == 0) return new List<LookupItemVo>();
            var khoLinhVe = _khoRepository.TableNoTracking.First(p => p.Id == khoLinhVeId);

            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var result = _khoRepository.TableNoTracking
                        .Where(p => (khoLinhVe.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && (p.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK || p.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh)) 
                                    || (khoLinhVe.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK && (p.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh || p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.Id == (long)EnumKhoDuocPham.KhoHoaChat)))
                        .Select(s => new LookupItemVo
                        {
                            KeyId = s.Id,
                            DisplayName = s.Ten
                        })
                        .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                        .ApplyLike(queryInfo.Query, o => o.DisplayName)
                        .Take(queryInfo.Take);

            return await result.ToListAsync();
        }

        public async Task<Core.Domain.ValueObject.YeuCauKSNK.NhanVienYeuCauVo> GetCurrentUser()
        {
            var getHoTen = _userRepository.TableNoTracking.Where(p => p.Id == _userAgentHelper.GetCurrentUserId()).Select(p => new Core.Domain.ValueObject.YeuCauKSNK.NhanVienYeuCauVo
            {
                NhanVienYeuCauId = _userAgentHelper.GetCurrentUserId(),
                HoTen = p.HoTen
            });

            return await getHoTen.FirstOrDefaultAsync();
        }

        public async Task<List<KSNKLookupVo>> GetKSNK(DropDownListRequestModel queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                return null;
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.VatTus.VatTu.Ten),
                nameof(Core.Domain.Entities.VatTus.VatTu.Ma)
            };
            var lstColumnNameSearchDuocPham = new List<string>
            {
               nameof(DuocPham.Ten),
               nameof(DuocPham.MaHoatChat),
               nameof(DuocPham.HoatChat),

            };

            var info = JsonConvert.DeserializeObject<KSNKJsonVo>(queryInfo.ParameterDependencies);

            

            var laDpVatTuKSNKBHYT = false; // LaKSKNKBHYT
            if (info.LaKSNKBHYT == 2) // LaKSKNKBHYT
            {
                laDpVatTuKSNKBHYT = true;
            }

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {

                var vatTuLookupVos = _vatTuBenhVienRepository.TableNoTracking
                   .Where(p => p.NhapKhoVatTuChiTiets.Any(ct => ct.NhapKhoVatTu.KhoId == info.KhoId && ct.SoLuongDaXuat < ct.SoLuongNhap && ct.LaVatTuBHYT == laDpVatTuKSNKBHYT
                                && ct.HanSuDung >= DateTime.Now && ct.NhapKhoVatTu.DaHet != true) && p.VatTus.IsDisabled != true && p.HieuLuc == true)
                   .Select(s => new KSNKLookupVo
                   {
                       KeyId = s.Id,
                       //DisplayName = s.VatTus.Ma + " - " + s.VatTus.Ten,
                       DisplayName = s.VatTus.Ten,
                       Ten = s.VatTus.Ten,
                       Ma = s.VatTus.Ma,
                       DVT = s.VatTus.DonViTinh,
                       SLTon =
                                s.NhapKhoVatTuChiTiets
                                    .Where(nkct =>
                                        nkct.NhapKhoVatTu.KhoId == info.KhoId &&
                                        nkct.LaVatTuBHYT == laDpVatTuKSNKBHYT && nkct.HanSuDung >= DateTime.Now && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                                    .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                       HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                        nkct.NhapKhoVatTu.KhoId == info.KhoId &&
                        nkct.LaVatTuBHYT == laDpVatTuKSNKBHYT && nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault() != null ? s.NhapKhoVatTuChiTiets.Where(nkct =>
                          nkct.NhapKhoVatTu.KhoId == info.KhoId &&
                          nkct.LaVatTuBHYT == laDpVatTuKSNKBHYT && nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault().HanSuDung.ApplyFormatDate() : null,

                       NhaSanXuat = s.VatTus.NhaSanXuat,
                       NuocSanXuat = s.VatTus.NuocSanXuat,
                       LaKSNKBHYT = laDpVatTuKSNKBHYT,
                       LoaiDuocPhamHayVatTu = false
                   })
                   .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.Ma)
                   .Take(queryInfo.Take)
                   .Union(_duocPhamBenhVienRepository.TableNoTracking
                      .Where(p => p.NhapKhoDuocPhamChiTiets.Any(ct => ct.NhapKhoDuocPhams.KhoId == info.KhoId && ct.SoLuongDaXuat < ct.SoLuongNhap && ct.LaDuocPhamBHYT == laDpVatTuKSNKBHYT
                                   && ct.HanSuDung >= DateTime.Now && ct.NhapKhoDuocPhams.DaHet != true))
                      .Select(s => new KSNKLookupVo
                      {
                          KeyId = s.Id,
                          //DisplayName = s.DuocPham.Ten + " - " + s.DuocPham.HoatChat,
                          DisplayName = s.DuocPham.Ten,
                          Ma = s.MaDuocPhamBenhVien,
                          Ten = s.DuocPham.Ten,
                          HoatChat = s.DuocPham.HoatChat,
                          //DVTId = s.DuocPham.DonViTinhId,
                          DVT = s.DuocPham.DonViTinh.Ten,
                          //DuongDungId = s.DuocPham.DuongDungId,
                          DuongDung = s.DuocPham.DuongDung.Ten,
                          HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                           nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDpVatTuKSNKBHYT &&
                           nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                           .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First().HanSuDung.ApplyFormatDate(),
                          SLTon = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDpVatTuKSNKBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2),
                          NhaSanXuat = s.DuocPham.NhaSanXuat,
                          NuocSanXuat = s.DuocPham.NuocSanXuat,
                          HamLuong = s.DuocPham.HamLuong,
                          LaKSNKBHYT = laDpVatTuKSNKBHYT,
                          LoaiDuocPhamHayVatTu = true
                      }).ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.Ma)
                   .Take(queryInfo.Take));
                   

                if(info.LoaiDuocPhamHayVatTu != null)
                {
                    return await vatTuLookupVos.Where(d=>d.LoaiDuocPhamHayVatTu == info.LoaiDuocPhamHayVatTu).ToListAsync();
                }
                else
                {
                    return await vatTuLookupVos.ToListAsync();
                }
                
            }
            else
            {

                var lstVatTuId = await _vatTuRepository
                      .ApplyFulltext(queryInfo.Query, nameof(VatTu), lstColumnNameSearch)
                      .Select(s => s.Id).ToListAsync();

                var dct = lstVatTuId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                var lstVDuocPhamId = await _duocPhamBenhVienRepository
                      .ApplyFulltext(queryInfo.Query, nameof(VatTu), lstColumnNameSearch)
                      .Select(s => s.Id).ToListAsync();

                var dctDP = lstVDuocPhamId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                var vatTuLookupVos = _vatTuBenhVienRepository.TableNoTracking
                 .Where(dv => dv.HieuLuc == true)
                 .Where(o => o.NhapKhoVatTuChiTiets.Any(kho => kho.NhapKhoVatTu.KhoId == info.KhoId
                                                                       && kho.SoLuongDaXuat < kho.SoLuongNhap
                                                                       && kho.LaVatTuBHYT == laDpVatTuKSNKBHYT
                                                                       && kho.HanSuDung >= DateTime.Now))
                 .Where(p => lstVatTuId.Any(x => x == p.Id)).Select(s => new KSNKLookupVo
                 {
                     KeyId = s.Id,
                     DisplayName = s.VatTus.Ten,
                     Ten = s.VatTus.Ten,
                     Ma = s.VatTus.Ma,
                     DVT = s.VatTus.DonViTinh,
                     HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                      nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laDpVatTuKSNKBHYT &&
                      nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault() != null ?
                            s.NhapKhoVatTuChiTiets.Where(nkct =>
                     nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laDpVatTuKSNKBHYT &&
                     nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault().HanSuDung.ApplyFormatDate() : null,
                     SLTon = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laDpVatTuKSNKBHYT).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                     NhaSanXuat = s.VatTus.NhaSanXuat,
                     NuocSanXuat = s.VatTus.NuocSanXuat,
                     LaKSNKBHYT = laDpVatTuKSNKBHYT,
                     LoaiDuocPhamHayVatTu = false
                 })
                  .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                  .Take(queryInfo.Take)
                  .Union( _duocPhamBenhVienRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearchDuocPham)
                     .Where(o => o.DuocPham.DuocPhamBenhVien.HieuLuc == true && lstVDuocPhamId.Any(x => x == o.Id) && o.NhapKhoDuocPhamChiTiets.Any(kho => kho.NhapKhoDuocPhams.KhoId == info.KhoId
                                                                           && kho.SoLuongDaXuat < kho.SoLuongNhap
                                                                           && kho.LaDuocPhamBHYT == laDpVatTuKSNKBHYT
                                                                           && kho.HanSuDung >= DateTime.Now))
                     .Select(s => new KSNKLookupVo
                     {
                         KeyId = s.Id,
                         DisplayName = s.DuocPham.Ten,
                         Ma = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.MaDuocPhamBenhVien : "",// đi đường vòng, fix bug fulltext search
                         Ten = s.DuocPham.Ten,
                         HoatChat = s.DuocPham.HoatChat,
                         //DVTId = s.DuocPham.DonViTinhId,
                         DVT = s.DuocPham.DonViTinh.Ten,
                         //DuongDungId = s.DuocPham.DuongDungId,
                         DuongDung = s.DuocPham.DuongDung.Ten,
                         HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                          nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDpVatTuKSNKBHYT &&
                          nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap).
                            OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First().HanSuDung.ApplyFormatDate(),
                         SLTon = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDpVatTuKSNKBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                         NhaSanXuat  = s.DuocPham.NhaSanXuat,
                         NuocSanXuat = s.DuocPham.NuocSanXuat,
                         HamLuong = s.DuocPham.HamLuong,
                         LaKSNKBHYT = laDpVatTuKSNKBHYT,
                         LoaiDuocPhamHayVatTu = true
                     })
                      .OrderBy(p => dctDP.Any(a => a.Key == p.KeyId) ? dctDP[p.KeyId] : dctDP.Count)
                      .Take(queryInfo.Take));

                if (info.LoaiDuocPhamHayVatTu != null)
                {
                    return await vatTuLookupVos.Where(d => d.LoaiDuocPhamHayVatTu == info.LoaiDuocPhamHayVatTu).ToListAsync();
                }
                else
                {
                    return await vatTuLookupVos.ToListAsync();
                }
            }

        }

        public async Task<Core.Domain.ValueObject.YeuCauKSNK.TrangThaiDuyetVos> GetTrangThaiPhieuLinh(long phieuLinhId,bool loaiDuocPhamHayVatTu)
        {
            if(loaiDuocPhamHayVatTu == true)
            {
                var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).FirstAsync();
                var trangThaiVo = new TrangThaiDuyetVos();
                if (yeuCauLinhDuocPham.DaGui != true)
                {
                    trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
                    trangThaiVo.Ten = EnumTrangThaiPhieuLinh.DangChoGui.GetDescription();
                    trangThaiVo.TrangThai = null;
                    return trangThaiVo;
                }
                else
                {
                    if (yeuCauLinhDuocPham.DuocDuyet == true)
                    {
                        trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                        trangThaiVo.TrangThai = true;
                        trangThaiVo.Ten = "Đã duyệt";
                        return trangThaiVo;
                    }
                    else if (yeuCauLinhDuocPham.DuocDuyet == false)
                    {
                        trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                        trangThaiVo.TrangThai = false;
                        trangThaiVo.Ten = "Từ chối duyệt";
                        return trangThaiVo;
                    }
                    else
                    {
                        trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                        trangThaiVo.TrangThai = null;
                        trangThaiVo.Ten = "Đang chờ duyệt";
                        return trangThaiVo;
                    }
                }
            }
            else
            {
                var yeuCauLinhVatTu = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).FirstAsync();
                var trangThaiVo = new Core.Domain.ValueObject.YeuCauKSNK.TrangThaiDuyetVos();
                if (yeuCauLinhVatTu.DaGui != true)
                {
                    trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoGui;
                    trangThaiVo.Ten = EnumTrangThaiPhieuLinh.DangChoGui.GetDescription();
                    trangThaiVo.TrangThai = null;
                    return trangThaiVo;
                }
                else
                {
                    if (yeuCauLinhVatTu.DuocDuyet == true)
                    {
                        trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DaDuyet;
                        trangThaiVo.TrangThai = true;
                        trangThaiVo.Ten = "Đã duyệt";
                        return trangThaiVo;
                    }
                    else if (yeuCauLinhVatTu.DuocDuyet == false)
                    {
                        trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.TuChoiDuyet;
                        trangThaiVo.TrangThai = false;
                        trangThaiVo.Ten = "Từ chối duyệt";
                        return trangThaiVo;
                    }
                    else
                    {
                        trangThaiVo.EnumTrangThaiPhieuLinh = EnumTrangThaiPhieuLinh.DangChoDuyet;
                        trangThaiVo.TrangThai = null;
                        trangThaiVo.Ten = "Đang chờ duyệt";
                        return trangThaiVo;
                    }
                }
            }
            
        }

        #region CheckValidator

        public async Task<bool> CheckKSNKExists(long? vatTuBenhVienId, bool? laVatTuBHYT, List<KSNKGridViewModelValidators> vatTuBenhVienIds, bool laDpHayVt)
        {
            if (vatTuBenhVienId == null)
            {
                return true;
            }
            if (vatTuBenhVienIds != null)
            {
                foreach (var item in vatTuBenhVienIds)
                {
                    if (vatTuBenhVienIds.Any(p => p.KSNKBenhVienId == vatTuBenhVienId && p.LaKSNKBHYT == laVatTuBHYT))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        public async Task<bool> CheckSoLuongTonKSNKGridVo(long? vatTuBenhVienId, double? soLuongYeuCau, long khoXuatId, bool laVatTuBHYT, bool laDpHayVt)
        {
            
            if (soLuongYeuCau == null)
            {
                return true;
            }
            if (laDpHayVt == true)
            {
                var soLuongTonDP = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == vatTuBenhVienId && o.LaDuocPhamBHYT == laVatTuBHYT && o.NhapKhoDuocPhams.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);

                if (soLuongYeuCau != null && vatTuBenhVienId != null && soLuongTonDP >= soLuongYeuCau)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var soLuongTonVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);

                if (soLuongYeuCau != null && vatTuBenhVienId != null && soLuongTonVatTu >= soLuongYeuCau)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
           
        }

        public async Task<bool> CheckSoLuongTonKSNK(long? vatTuBenhVienId, double? soLuongYeuCau, bool? duocDuyet, double? soLuongCoTheXuat, long? khoXuatId, bool? laVatTuBHYT, bool? isValidator, bool loaiDuocPhamHayVatTu)
        {
            if(loaiDuocPhamHayVatTu == true)
            {
                var soLuongTonDP = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == vatTuBenhVienId && o.LaDuocPhamBHYT == laVatTuBHYT && o.NhapKhoDuocPhams.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                if (isValidator == false && soLuongTonDP >= soLuongYeuCau)
                {
                    return true;
                }
                if (isValidator == false && soLuongTonDP < soLuongYeuCau)
                {
                    return false;
                }
                if (duocDuyet == null) // đang chờ duyệt
                {
                    if (soLuongYeuCau == null)
                    {
                        return true;
                    }
                    if (soLuongYeuCau != null && vatTuBenhVienId != null && soLuongTonDP >= soLuongYeuCau)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (duocDuyet == true) // được duyệt
                {
                    return true;
                }
                else // từ chối duyệt
                {
                    if (soLuongYeuCau == null)
                    {
                        return true;
                    }
                    if (soLuongCoTheXuat == null || soLuongCoTheXuat < soLuongYeuCau)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                var soLuongTonVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                if (isValidator == false && soLuongTonVatTu >= soLuongYeuCau)
                {
                    return true;
                }
                if (isValidator == false && soLuongTonVatTu < soLuongYeuCau)
                {
                    return false;
                }
                if (duocDuyet == null) // đang chờ duyệt
                {
                    if (soLuongYeuCau == null)
                    {
                        return true;
                    }
                    if (soLuongYeuCau != null && vatTuBenhVienId != null && soLuongTonVatTu >= soLuongYeuCau)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (duocDuyet == true) // được duyệt
                {
                    return true;
                }
                else // từ chối duyệt
                {
                    if (soLuongYeuCau == null)
                    {
                        return true;
                    }
                    if (soLuongCoTheXuat == null || soLuongCoTheXuat < soLuongYeuCau)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            
        }

        public async Task CheckPhieuLinhDaDuyetHoacDaHuy(long phieuLinhId)
        {
            var result = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).Select(p => p).FirstOrDefaultAsync();
            var resourceName = string.Empty;
            if (result == null)
            {
                resourceName = "YeuCauLinhVatTu.LinhThuong.NotExists";
            }
            else
            {
                if (result.DuocDuyet == true)
                {
                    resourceName = "YeuCauLinhVatTu.LinhThuong.DaDuyet";
                }
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                var currentUserLanguge = _userAgentHelper.GetUserLanguage();
                var mess = await _localeStringResourceRepository.TableNoTracking
                    .Where(x => x.ResourceName == resourceName && x.Language == (int)currentUserLanguge)
                    .Select(x => x.ResourceValue).FirstOrDefaultAsync();
                throw new Exception(mess ?? resourceName);
            }
        }

        #endregion

        public string InPhieuLinhThuongKSNK(PhieuLinhThuongDPVTModel phieuLinhThuongKSNK)
        {
            var listHTML = new List<string>();
            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU LĨNH </div></div>";
            if (phieuLinhThuongKSNK.YeuCauLinhVatTuIds.Where(d=> d.LoaiDuocPhamHayVatTu == true).Count() != 0 )
            {
                
                var hearder = string.Empty;
                
                var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongThuocDuocPham")).First();
                var templateLinhThuongGayNghien = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuocGayNghien")).First();

                int? nhomVatTu = 0;
                string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
                if (string.IsNullOrEmpty(nhomVatTuString))
                {
                    nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
                }

                var yeuCauLinhDuocPhamIds = phieuLinhThuongKSNK.YeuCauLinhVatTuIds
                                        .Where(d => d.LoaiDuocPhamHayVatTu == true)
                                        .Select(d => d.YeuCauLinhVatTuId).Distinct().ToList();


                var infoYeuCauLinhDuocPhams = _yeuCauLinhDuocPhamRepository.TableNoTracking
                                                         .Where(d => yeuCauLinhDuocPhamIds.Contains(d.Id))
                                                                          .Select(d => new {
                                                                              MaVachPhieuLinh = d.SoPhieu,
                                                                              BarCodeImgBase64 = d.SoPhieu,
                                                                              TuNgay = d.ThoiDiemLinhTongHopTuNgay != null ?d.ThoiDiemLinhTongHopTuNgay.GetValueOrDefault().ApplyFormatDate() : "&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                              DenNgay = d.ThoiDiemLinhTongHopDenNgay != null ? d.ThoiDiemLinhTongHopTuNgay.GetValueOrDefault().ApplyFormatDate() : "&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                              DienGiai = d.GhiChu,
                                                                              NoiGiao = d.KhoXuat.Ten,
                                                                              NhapVeKho = d.KhoNhap.Ten,
                                                                              YeuCauLinhChiTietIds = d.YeuCauLinhDuocPhamChiTiets.Select(g => g.Id).ToList(),
                                                                              YeuCauLinhId = d.Id
                                                                          }).ToList();

                foreach (var item in infoYeuCauLinhDuocPhams.ToList())
                {
                    var content = string.Empty;
                    var contentGayNghien = string.Empty;

                    var groupThuocBHYT = "Thuốc BHYT";
                    var groupThuocKhongBHYT = "Không BHYT";

                    if (phieuLinhThuongKSNK.Header == true)
                    {
                        hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                                      "<th>PHIẾU LĨNH KNSK</th>" +
                                 "</p>";
                    }

                    var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                                + "</b></tr>";
                    var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                                + "</b></tr>";
                    var query = new List<ThongTinLinhDuocPhamChiTiet>();
                    var queryGayNghien = new List<ThongTinLinhDuocPhamChiTiet>();

                    var queryThuoc = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                           .Where(p => p.YeuCauLinhDuocPhamId == item.YeuCauLinhId
                               && p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien
                               && p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan
                            )
                           .Select(s => new ThongTinLinhDuocPhamChiTiet
                           {
                               Ma = s.DuocPhamBenhVien.Ma,
                               TenThuocHoacVatTu = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == nhomVatTu ?
                                                                        s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NhaSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                          (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NuocSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                        s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.HamLuong) ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                               DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null,
                               SLYeuCau = s.SoLuong,
                               LaDuocPhamBHYT = s.LaDuocPhamBHYT
                           }).OrderByDescending(z => z.LaDuocPhamBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();
                    var thuocBHYT = queryThuoc.Where(z => z.LaDuocPhamBHYT).ToList();
                    var thuocKhongBHYT = queryThuoc.Where(z => !z.LaDuocPhamBHYT).ToList();
                    query = thuocBHYT.Concat(thuocKhongBHYT).ToList();

                    var queryThuocGayNghien = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                               .Where(p => p.YeuCauLinhDuocPhamId == item.YeuCauLinhId
                                        && (p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                               .Select(s => new ThongTinLinhDuocPhamChiTiet
                               {
                                   Ma = s.DuocPhamBenhVien.Ma,
                                   TenThuocHoacVatTu = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == nhomVatTu ?
                                                                            s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NhaSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                              (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NuocSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                            s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.HamLuong) ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                                   DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null,
                                   SLYeuCau = s.SoLuong,
                                   LaDuocPhamBHYT = s.LaDuocPhamBHYT
                               }).OrderByDescending(z => z.LaDuocPhamBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();

                    var thuocGayNghienBHYT = queryThuocGayNghien.Where(z => z.LaDuocPhamBHYT).ToList();
                    var thuocGayNghienKhongBHYT = queryThuocGayNghien.Where(z => !z.LaDuocPhamBHYT).ToList();

                    queryGayNghien = thuocGayNghienBHYT.Concat(thuocGayNghienKhongBHYT).ToList();



                    if (query.Any())
                    {
                        var STT = 1;
                        var infoLinhDuocChiTiet = string.Empty;
                        if (query.Any(p => p.LaDuocPhamBHYT == true))
                        {
                            infoLinhDuocChiTiet += headerBHYT;
                            foreach (var itemChiTiet in query)
                            {
                                if (itemChiTiet.LaDuocPhamBHYT == true)
                                {
                                    infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                        + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.Ma
                                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.TenThuocHoacVatTu
                                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.DVT
                                                        + "<td style = 'border: 1px solid #020000;text-align: right;'>" + itemChiTiet.SLYeuCau.MathRoundNumber(2)
                                                        + "<td style = 'border: 1px solid #020000;text-align: right;'>" + itemChiTiet.SLThucPhat
                                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.GhiChu
                                                        + "</tr>";
                                    STT++;
                                    groupThuocBHYT = string.Empty;
                                }

                            }
                        }
                        if (query.Any(p => p.LaDuocPhamBHYT == false))
                        {
                            infoLinhDuocChiTiet += headerKhongBHYT;
                            foreach (var itemChiTiet in query)
                            {
                                if (itemChiTiet.LaDuocPhamBHYT == false)
                                {
                                    infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                       + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.Ma
                                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.TenThuocHoacVatTu
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.DVT
                                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + itemChiTiet.SLYeuCau.MathRoundNumber(2)
                                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + itemChiTiet.SLThucPhat
                                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.GhiChu
                                                       + "</tr>";
                                    STT++;
                                    groupThuocKhongBHYT = string.Empty;
                                }
                            }
                        }

                        var data = new PhieuLinhThuongDuocPhamData
                        {
                            HeaderPhieuLinhThuoc = hearder,
                            MaVachPhieuLinh = "Số: " + item.MaVachPhieuLinh,
                            ThuocHoacVatTu = infoLinhDuocChiTiet,
                            LogoUrl = phieuLinhThuongKSNK.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(item.BarCodeImgBase64),
                            TuNgay = item.TuNgay,
                            DenNgay = item.DenNgay,
                            DienGiai = item.DienGiai,
                            NoiGiao = item.NoiGiao,
                            NhapVeKho = item.NhapVeKho,
                            CongKhoan = "Cộng khoản: " + (STT - 1).ToString() + " khoản",
                            Ngay = DateTime.Now.Day.ConvertDateToString(),
                            Thang = DateTime.Now.Month.ConvertMonthToString(),
                            Nam = DateTime.Now.Year.ConvertYearToString()
                        };
                        content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
                        listHTML.Add(content);
                    }
                    if (queryGayNghien.Any())
                    {
                        var STT = 1;
                        var infoLinhDuocChiTiet = string.Empty;
                        if (queryGayNghien.Any(p => p.LaDuocPhamBHYT == true))
                        {
                            infoLinhDuocChiTiet += headerBHYT;
                            foreach (var itemChiTiet in queryGayNghien)
                            {
                                if (itemChiTiet.LaDuocPhamBHYT == true)
                                {
                                    infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                        + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.Ma
                                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.TenThuocHoacVatTu
                                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.DVT
                                                        + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemChiTiet.SLYeuCau.MathRoundNumber(2)), false)
                                                        + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemChiTiet.SLThucPhat.MathRoundNumber(2)), false)
                                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.GhiChu
                                                        + "</tr>";
                                    STT++;
                                    groupThuocBHYT = string.Empty;
                                }

                            }
                        }
                        if (queryGayNghien.Any(p => p.LaDuocPhamBHYT == false))
                        {
                            infoLinhDuocChiTiet += headerKhongBHYT;
                            foreach (var itemChiTiet in queryGayNghien)
                            {
                                if (itemChiTiet.LaDuocPhamBHYT == false)
                                {
                                    infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                       + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.Ma
                                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.TenThuocHoacVatTu
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.DVT
                                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(itemChiTiet.SLYeuCau.MathRoundNumber(2)), false)
                                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"/*NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucPhat.MathRoundNumber(2)), false)*/
                                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.GhiChu
                                                       + "</tr>";
                                    STT++;
                                    groupThuocKhongBHYT = string.Empty;
                                }
                            }
                        }

                        var data = new PhieuLinhThuongDuocPhamData
                        {
                            HeaderPhieuLinhThuoc = hearder,
                            MaVachPhieuLinh = "Số: " + item.MaVachPhieuLinh,
                            ThuocHoacVatTu = infoLinhDuocChiTiet,
                            LogoUrl = phieuLinhThuongKSNK.HostingName + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(item.BarCodeImgBase64),
                            TuNgay = item.TuNgay,
                            DenNgay = item.DenNgay,
                            DienGiai = item.DienGiai,
                            NoiGiao = item.NoiGiao,
                            NhapVeKho = item.NhapVeKho,
                            CongKhoan = "Cộng khoản: " + (STT - 1).ToString() + " khoản",
                            Ngay = DateTime.Now.Day.ConvertDateToString(),
                            Thang = DateTime.Now.Month.ConvertMonthToString(),
                            Nam = DateTime.Now.Year.ConvertYearToString()
                        };

                        contentGayNghien = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongGayNghien.Body, data);
                        listHTML.Add(content);
                    }
                }
                
            }
            if(phieuLinhThuongKSNK.YeuCauLinhVatTuIds.Where(d => d.LoaiDuocPhamHayVatTu == false).Count() != 0)
            {
                var yeuCauLinhVatTuIds = phieuLinhThuongKSNK.YeuCauLinhVatTuIds
                                        .Where(d => d.LoaiDuocPhamHayVatTu == false)
                                        .Select(d => d.YeuCauLinhVatTuId).Distinct().ToList();
               
                var templateLinhThuongVT = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongVatTu")).First();


                // get tat ca cac phieu can  vua tao theo yeuCauLinhVatTuIds

                var infoYeuCauLinhVatTus = BaseRepository.TableNoTracking
                                                         .Where(d => yeuCauLinhVatTuIds.Contains(d.Id))
                                                                          .Select(d => new {
                                                                              MaVachPhieuLinh = d.SoPhieu,
                                                                              BarCodeImgBase64 = d.SoPhieu,
                                                                              TuNgay = d.ThoiDiemLinhTongHopTuNgay != null ? d.ThoiDiemLinhTongHopTuNgay.GetValueOrDefault().ApplyFormatDate() : "&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                              DenNgay = d.ThoiDiemLinhTongHopDenNgay != null ? d.ThoiDiemLinhTongHopTuNgay.GetValueOrDefault().ApplyFormatDate() : "&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                              DienGiai = d.GhiChu,
                                                                              NoiGiao = d.KhoXuat.Ten,
                                                                              NhapVeKho = d.KhoNhap.Ten,
                                                                              YeuCauLinhChiTietIds = d.YeuCauLinhVatTuChiTiets.Select(g=>g.Id).ToList(),
                                                                              YeuCauLinhId = d.Id
                                                                          }).ToList();

                
                foreach (var item in infoYeuCauLinhVatTus.ToList())
                {
                    var content = string.Empty;
                    var hearder = string.Empty;

                    if (phieuLinhThuongKSNK.Header == true)
                    {
                        hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                                      "<th>PHIẾU LĨNH KNSK</th>" +
                                 "</p>";
                    }

                    var infoLinhDuocChiTiet = string.Empty;
                    var groupThuocBHYT = "Vật Tư BHYT";
                    var groupThuocKhongBHYT = "Vật Tư Không BHYT";

                    var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                                + "</b></tr>";
                    var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                                + "</b></tr>";
                    var STT = 1;
                    var query = new List<ThongTinLinhKSNKChiTiet>();
                    if (phieuLinhThuongKSNK.LoaiPhieuLinh == 1) // lĩnh thường(dự trù)
                    {
                        var queryVT = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                            .Where(p => p.YeuCauLinhVatTuId ==  item.YeuCauLinhId)
                                .Select(s => new ThongTinLinhKSNKChiTiet
                                {
                                    Ma = s.VatTuBenhVien.VatTus.Ma,
                                    TenThuocHoacKSNK = s.VatTuBenhVien.VatTus.Ten
                                                     + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NhaSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "")
                                                     + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NuocSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                    SLYeuCau = s.SoLuong,
                                    LaKSNKBHYT = s.LaVatTuBHYT
                                }).OrderByDescending(z => z.LaKSNKBHYT).ThenBy(z => z.TenThuocHoacKSNK).ToList();
                        query = queryVT.Where(z => z.LaKSNKBHYT).Concat(queryVT.Where(z => !z.LaKSNKBHYT)).ToList();
                    }
                    foreach (var itemChiTiet in query)
                    {
                        infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.Ma
                                                + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.TenThuocHoacKSNK
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + itemChiTiet.DVT
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + itemChiTiet.SLYeuCau
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"/*item.SLThucPhat*/
                                                + "<td style = 'border: 1px solid #020000;text-align: left;'>" + itemChiTiet.GhiChu
                                                + "</tr>";
                        STT++;
                    }
                    var data = new PhieuLinhThuongKSNKData
                    {
                        HeaderPhieuLinhThuoc = hearder,
                        MaVachPhieuLinh = "Số: " + item.MaVachPhieuLinh,
                        ThuocHoacVatTu = infoLinhDuocChiTiet,
                        LogoUrl = phieuLinhThuongKSNK.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(item.BarCodeImgBase64),
                        TuNgay = item.TuNgay,
                        DenNgay = item.DenNgay,
                        DienGiai = item.DienGiai,
                        NoiGiao = item.NoiGiao,
                        NhapVeKho = item.NhapVeKho,
                        CongKhoan = "Cộng khoản: " + (STT - 1).ToString() + " khoản",
                        Ngay = DateTime.Now.Day.ConvertDateToString(),
                        Thang = DateTime.Now.Month.ConvertMonthToString(),
                        Nam = DateTime.Now.Year.ConvertYearToString()
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongVT.Body, data);
                    listHTML.Add(content);
                }
            }
          
            return listHTML.Join("<div class=\"pagebreak\"> </div>");
        }

        public LinhThuongKSNKGridVo LinhThuongKSNKGridVo(LinhThuongKSNKGridVo model)
        {
            double soLuongTonDpHoacVatTu = 0;
            if(model.LoaiDuocPhamHayVatTu == true)
            {
                var sLTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(o => o.DuocPhamBenhVienId == model.VatTuBenhVienId && 
                    o.LaDuocPhamBHYT == model.LaVatTuBHYT &&
                    o.NhapKhoDuocPhams.KhoId == model.KhoXuatId)
                    .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();

                soLuongTonDpHoacVatTu = sLTon;
            }
            else
            {
                var sLTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Where(o => o.VatTuBenhVienId == model.VatTuBenhVienId &&
                    o.LaVatTuBHYT == model.LaVatTuBHYT && 
                    o.NhapKhoVatTu.KhoId == model.KhoXuatId)
                    .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();
                
                soLuongTonDpHoacVatTu = sLTon;
            }
            var result = new LinhThuongKSNKGridVo
            {
                VatTuBenhVienId = model.VatTuBenhVienId,
                Ten = model.Ten,
                Ma = model.Ma,
                DVT = model.DVT,
                NhaSX = model.NhaSX,
                NuocSX = model.NuocSX,
                SLYeuCau = model.SLYeuCau,
                KhoXuatId = model.KhoXuatId,
                SLTon = soLuongTonDpHoacVatTu,
                Nhom = model.LoaiKSNK == 1 ? "Không BHYT" : "BHYT",
                LaVatTuBHYT = model.LaVatTuBHYT,
                TenKhoLinh = model.TenKhoLinh,
                LoaiDuocPhamHayVatTu = model.LoaiDuocPhamHayVatTu
            };
            return result;
        }

        public double GetSoLuongTonKSNKGridVo(long vatTuBenhVienId, long khoXuatId, bool laVatTuBHYT)
        {
            return _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();
        }
        #region linh bu xem truoc
        public string InPhieuLinhBuKSNKXemTruoc(PhieuLinhThuongKSNKXemTruoc phieuLinhThuongKSNKXemTruoc)
        {
            var content = string.Empty;
            var hearder = string.Empty;
            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongVatTu")).First();

            var infoLinhDuocChiTiet = string.Empty;

            var STT = 1;
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                          .Where(x => x.KhoLinhId == phieuLinhThuongKSNKXemTruoc.KhoLinhBuId
                               && x.YeuCauLinhVatTuId == null
                               && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                               && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                               && x.KhongLinhBu != true
                               && x.SoLuong > 0
                               && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                               && phieuLinhThuongKSNKXemTruoc.YeuCauKSNKBenhVienIds.Contains(x.Id))
                           .Select(s => new ThongTinLinhKSNKChiTiet
                           {
                               KSNKBenhVienId = s.VatTuBenhVienId,
                               Ma = s.VatTuBenhVien.Ma,
                               TenThuocHoacKSNK = s.VatTuBenhVien.VatTus.Ten
                                                    + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NhaSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "")
                                                    + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NuocSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                               DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                               SLYeuCau = s.SoLuong,
                               LaKSNKBHYT = s.LaVatTuBHYT
                           }).OrderByDescending(p => p.LaKSNKBHYT).ThenBy(p => !p.LaKSNKBHYT)
                            .GroupBy(x => new { x.KSNKBenhVienId, x.LaKSNKBHYT, x.Ma, x.DVT })
                            .Select(item => new ThongTinLinhKSNKChiTiet()
                            {
                                KSNKBenhVienId = item.First().KSNKBenhVienId,
                                Ma = item.First().Ma,
                                TenThuocHoacKSNK = item.First().TenThuocHoacKSNK,
                                DVT = item.First().DVT,
                                SLYeuCau = item.Sum(x => x.SLYeuCau).Value.MathRoundNumber(2),
                                LaKSNKBHYT = item.First().LaKSNKBHYT
                            }).ToList();
            var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == phieuLinhThuongKSNKXemTruoc.KhoLinhId
                       && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var result = query.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.KSNKBenhVienId && o.LaVatTuBHYT == p.LaKSNKBHYT));

            foreach (var item in result.ToList())
            {
                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                        + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacKSNK
                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                        + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                        + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucPhat
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                        + "</tr>";
                STT++;

            }
            var khoXuat = _khoRepository.TableNoTracking.Where(d => d.Id == phieuLinhThuongKSNKXemTruoc.KhoLinhBuId).Select(d => d.Ten).FirstOrDefault();
            var khoLinh = _khoRepository.TableNoTracking.Where(d => d.Id == phieuLinhThuongKSNKXemTruoc.KhoLinhId).Select(d => d.Ten).FirstOrDefault();

            var data = new PhieuLinhThuongKSNKData
            {
                HeaderPhieuLinhThuoc = hearder,
                MaVachPhieuLinh = "Số: " + "",
                ThuocHoacVatTu = infoLinhDuocChiTiet,
                LogoUrl = phieuLinhThuongKSNKXemTruoc.HostingName + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = "",
                TuNgay = phieuLinhThuongKSNKXemTruoc.ThoiDiemLinhTongHopTuNgay.ApplyFormatDateTimeSACH(),
                DenNgay = phieuLinhThuongKSNKXemTruoc.ThoiDiemLinhTongHopDenNgay.ApplyFormatDateTimeSACH(),
                DienGiai = "",
                NoiGiao = khoLinh,
                NhapVeKho = khoXuat,
                CongKhoan = "Cộng khoản: " + (STT - 1).ToString() + " khoản",
                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString()
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
            return content;
        }
        #endregion
        public List<long> GetIdsYeuCauVT(long KhoLinhTuId, long KhoLinhVeId, long vatTuBenhVienId)
        {
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(x => x.KhoLinhId == KhoLinhVeId
                                && x.YeuCauLinhVatTuId == null
                                && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                                && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                                && x.KhongLinhBu != true
                                && x.SoLuong > 0
                                && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                                && x.VatTuBenhVienId == vatTuBenhVienId)
                    .Select(item => new YeuCauLinhKSNKBuGridParentVo()
                    {
                        Id = item.Id,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        TenVatTu = item.Ten,
                        Nhom = item.LaVatTuBHYT ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                        DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                        SoLuongCanBu = item.SoLuong,
                        SoLuongDaBu = item.SoLuongDaLinhBu
                    });

            var yeuCauLinhVatTuBuGridParentVos = query.ToList();

            var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == KhoLinhTuId
                     && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var result = yeuCauLinhVatTuBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaBHYT));



            return result.Select(s => s.Id).ToList();
        }
        public DateTime GetDateTimeVatTu(long YeuCauVatTuBenhVienId)
        {
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(s => s.Id == YeuCauVatTuBenhVienId).Select(s => (s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh));
            return query.First();
        }
        public string GetNamKhoLinh(long khoLinhId)
        {
            var result = _khoRepository.TableNoTracking
                        .Where(p =>  p.Id == khoLinhId)
                        .Select(s => s.Ten).FirstOrDefault();
            return result;
        }
    }
}
