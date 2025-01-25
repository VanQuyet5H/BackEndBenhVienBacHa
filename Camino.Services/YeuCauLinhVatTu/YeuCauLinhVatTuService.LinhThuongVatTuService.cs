using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial class YeuCauLinhVatTuService
    {
        public async Task<List<LookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaPhongId = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == noiLamViecCurrentId).Select(z => z.KhoaPhongId).FirstOrDefault();
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                        .Where(p => p.NhanVienId == userCurrentId && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) && p.Kho.KhoaPhongId == khoaPhongId
                        && p.Kho.LoaiVatTu == true)
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
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var result = _khoRepository.TableNoTracking
                        .Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && p.LoaiVatTu == true)
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

        public async Task<NhanVienYeuCauVo> GetCurrentUser()
        {
            var getHoTen = _userRepository.TableNoTracking.Where(p => p.Id == _userAgentHelper.GetCurrentUserId()).Select(p => new NhanVienYeuCauVo
            {
                NhanVienYeuCauId = _userAgentHelper.GetCurrentUserId(),
                HoTen = p.HoTen
            });

            return await getHoTen.FirstOrDefaultAsync();
        }
        public async Task<List<VatTuLookupVo>> GetVatTu(DropDownListRequestModel queryInfo)
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

            var info = JsonConvert.DeserializeObject<VatTuJsonVo>(queryInfo.ParameterDependencies);
            var laVatTuBHYT = false;
            if (info.LaVatTuBHYT == 2) // vattu BHYT
            {
                laVatTuBHYT = true;
            }

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var vatTuLookupVos = _vatTuBenhVienRepository.TableNoTracking
                   .Where(p => p.NhapKhoVatTuChiTiets.Any(ct => ct.NhapKhoVatTu.KhoId == info.KhoId && ct.SoLuongDaXuat < ct.SoLuongNhap && ct.LaVatTuBHYT == laVatTuBHYT
                                && ct.HanSuDung >= DateTime.Now && ct.NhapKhoVatTu.DaHet != true) && p.VatTus.IsDisabled != true && p.HieuLuc == true)
                   .Select(s => new VatTuLookupVo
                   {
                       KeyId = s.Id,
                       //DisplayName = s.VatTus.Ma + " - " + s.VatTus.Ten,
                       DisplayName = s.VatTus.Ten,
                       Ten = s.VatTus.Ten,
                       Ma = s.VatTus.Ma,
                       DVT = s.VatTus.DonViTinh,
                       //SLTon = s.NhapKhoVatTuChiTiets
                       //             .Where(nkct =>
                       //                 nkct.NhapKhoVatTu.KhoId == info.KhoId &&
                       //                 nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                       //             .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                       //HanSuDung = s.NhapKhoVatTuChiTiets
                       // .Where(nkct => nkct.NhapKhoVatTu.KhoId == info.KhoId &&
                       //         nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                       // .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                       //     .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault() != null ? s.NhapKhoVatTuChiTiets.Where(nkct =>
                       //                     nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                       //     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault().HanSuDung.ApplyFormatDate() : null,

                       NhaSanXuat = s.VatTus.NhaSanXuat,
                       NuocSanXuat = s.VatTus.NuocSanXuat,
                       LaVatTuBHYT = laVatTuBHYT,
                   })
                   .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.Ma)
                   .Take(queryInfo.Take);

                var vatTuTrongKhos = await vatTuLookupVos.ToListAsync();
                if (vatTuTrongKhos.Any())
                {
                    var vatTuBenhVienIds = vatTuTrongKhos.Select(o => o.KeyId).ToList();

                    var vatTuTrongKhoChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoVatTu.KhoId == info.KhoId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && o.LaVatTuBHYT == laVatTuBHYT && o.HanSuDung >= DateTime.Now)
                        .Select(o => new
                        {
                            Id = o.Id,
                            VatTuBenhVienId = o.VatTuBenhVienId,
                            SoLuongDaXuat = o.SoLuongDaXuat,
                            SoLuongNhap = o.SoLuongNhap,
                            HanSuDung = o.HanSuDung,
                            NgayNhapVaoBenhVien = o.NgayNhapVaoBenhVien,
                        })
                        .ToList();
                    foreach (var vt in vatTuTrongKhos)
                    {
                        vt.SLTon = vatTuTrongKhoChiTiets.Where(o => o.VatTuBenhVienId == vt.KeyId).Select(o => o.SoLuongNhap - o.SoLuongDaXuat).DefaultIfEmpty().Sum().MathRoundNumber(2);
                        var hanSuDung = vatTuTrongKhoChiTiets
                            .Where(o => o.VatTuBenhVienId == vt.KeyId && o.SoLuongDaXuat.MathRoundNumber(2) < o.SoLuongNhap.MathRoundNumber(2))
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).FirstOrDefault();

                        if (hanSuDung != null)
                            vt.HanSuDung = hanSuDung.ApplyFormatDate();
                    }
                }
                return vatTuTrongKhos;
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

                var vatTuLookupVos = _vatTuBenhVienRepository.TableNoTracking
                 .Where(dv => dv.HieuLuc == true)
                 .Where(o => o.NhapKhoVatTuChiTiets.Any(kho => kho.NhapKhoVatTu.KhoId == info.KhoId
                                                                       && kho.SoLuongDaXuat < kho.SoLuongNhap
                                                                       && kho.LaVatTuBHYT == laVatTuBHYT
                                                                       && kho.HanSuDung >= DateTime.Now))
                 .Where(p => lstVatTuId.Any(x => x == p.Id)).Select(s => new VatTuLookupVo
                 {
                     KeyId = s.Id,
                     DisplayName = s.VatTus.Ten,
                     Ten = s.VatTus.Ten,
                     Ma = s.VatTus.Ma,
                     DVT = s.VatTus.DonViTinh,
                     HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                      nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laVatTuBHYT &&
                      nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault() != null ?
                            s.NhapKhoVatTuChiTiets.Where(nkct =>
                     nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laVatTuBHYT &&
                     nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault().HanSuDung.ApplyFormatDate() : null,
                     SLTon = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laVatTuBHYT).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                     NhaSanXuat = s.VatTus.NhaSanXuat,
                     NuocSanXuat = s.VatTus.NuocSanXuat,
                     LaVatTuBHYT = laVatTuBHYT,
                 })
                  .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                  .Take(queryInfo.Take);
                return await vatTuLookupVos.ToListAsync();
            }

        }
        public async Task<List<VatTuLookupVo>> GetVatTuOld(DropDownListRequestModel queryInfo)
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

            var info = JsonConvert.DeserializeObject<VatTuJsonVo>(queryInfo.ParameterDependencies);
            var laVatTuBHYT = false;
            if (info.LaVatTuBHYT == 2) // vattu BHYT
            {
                laVatTuBHYT = true;
            }

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var vatTuLookupVos = _vatTuBenhVienRepository.TableNoTracking
                   .Where(p => p.NhapKhoVatTuChiTiets.Any(ct => ct.NhapKhoVatTu.KhoId == info.KhoId && ct.SoLuongDaXuat < ct.SoLuongNhap && ct.LaVatTuBHYT == laVatTuBHYT
                                && ct.HanSuDung >= DateTime.Now && ct.NhapKhoVatTu.DaHet != true) && p.VatTus.IsDisabled != true && p.HieuLuc == true)
                   .Select(s => new VatTuLookupVo
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
                                        nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                                    .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                       HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                        nkct.NhapKhoVatTu.KhoId == info.KhoId &&
                        nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault() != null ? s.NhapKhoVatTuChiTiets.Where(nkct =>
                          nkct.NhapKhoVatTu.KhoId == info.KhoId &&
                          nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault().HanSuDung.ApplyFormatDate() : null,

                       NhaSanXuat = s.VatTus.NhaSanXuat,
                       NuocSanXuat = s.VatTus.NuocSanXuat,
                       LaVatTuBHYT = laVatTuBHYT,
                   })
                   .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.Ma)
                   .Take(queryInfo.Take);

                return await vatTuLookupVos.ToListAsync();
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

                var vatTuLookupVos = _vatTuBenhVienRepository.TableNoTracking
                 .Where(dv => dv.HieuLuc == true)
                 .Where(o => o.NhapKhoVatTuChiTiets.Any(kho => kho.NhapKhoVatTu.KhoId == info.KhoId
                                                                       && kho.SoLuongDaXuat < kho.SoLuongNhap
                                                                       && kho.LaVatTuBHYT == laVatTuBHYT
                                                                       && kho.HanSuDung >= DateTime.Now))
                 .Where(p => lstVatTuId.Any(x => x == p.Id)).Select(s => new VatTuLookupVo
                 {
                     KeyId = s.Id,
                     DisplayName = s.VatTus.Ten,
                     Ten = s.VatTus.Ten,
                     Ma = s.VatTus.Ma,
                     DVT = s.VatTus.DonViTinh,
                     HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                      nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laVatTuBHYT &&
                      nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault() != null ?
                            s.NhapKhoVatTuChiTiets.Where(nkct =>
                     nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laVatTuBHYT &&
                     nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap && s.VatTus.IsDisabled != true && s.HieuLuc == true)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).FirstOrDefault().HanSuDung.ApplyFormatDate() : null,
                     SLTon = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == info.KhoId && nkct.LaVatTuBHYT == laVatTuBHYT).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(1),
                     NhaSanXuat = s.VatTus.NhaSanXuat,
                     NuocSanXuat = s.VatTus.NuocSanXuat,
                     LaVatTuBHYT = laVatTuBHYT,
                 })
                  .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                  .Take(queryInfo.Take);
                return await vatTuLookupVos.ToListAsync();
            }

        }

        public async Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long phieuLinhId)
        {
            var yeuCauLinhVatTu = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).FirstAsync();
            var trangThaiVo = new TrangThaiDuyetVo();
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

        #region CheckValidator

        public async Task<bool> CheckVatTuExists(long? vatTuBenhVienId, bool? laVatTuBHYT, List<VatTuGridViewModelValidator> vatTuBenhVienIds)
        {
            if (vatTuBenhVienId == null)
            {
                return true;
            }
            if (vatTuBenhVienIds != null)
            {
                foreach (var item in vatTuBenhVienIds)
                {
                    if (vatTuBenhVienIds.Any(p => p.VatTuBenhVienId == vatTuBenhVienId && p.LaVatTuBHYT == laVatTuBHYT))
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
        public async Task<bool> CheckSoLuongTonVatTuGridVo(long? vatTuBenhVienId, double? soLuongYeuCau, long khoXuatId, bool laVatTuBHYT)
        {
            if (soLuongYeuCau == null)
            {
                return true;
            }
            var soLuongTonVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            soLuongTonVatTu = soLuongTonVatTu.MathRoundNumber(2);
            if (soLuongYeuCau != null && vatTuBenhVienId != null && soLuongTonVatTu >= soLuongYeuCau)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckSoLuongTonVatTu(long? vatTuBenhVienId, double? soLuongYeuCau, bool? duocDuyet, double? soLuongCoTheXuat, long? khoXuatId, bool? laVatTuBHYT, bool? isValidator)
        {
            var soLuongTonVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            soLuongTonVatTu = soLuongTonVatTu.MathRoundNumber(2);
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

        public string InPhieuLinhThuongVatTu(PhieuLinhThuongVatTu phieuLinhThuongVatTu)
        {
            var content = string.Empty;
            var hearder = string.Empty;
            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongVatTu")).First();
            var getPhieuLinh = BaseRepository.GetById(phieuLinhThuongVatTu.YeuCauLinhVatTuId, s => s.Include(u => u.KhoNhap).Include(u => u.KhoXuat));
            if (phieuLinhThuongVatTu.Header == true)
            {
                //hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                //              "<th>PHIẾU LĨNH VẬT TƯ</th>" +
                //         "</p>";
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
            var query = new List<ThongTinLinhVatTuChiTiet>();
            if (phieuLinhThuongVatTu.LoaiPhieuLinh == 1) // lĩnh thường(dự trù)
            {
                var queryVT = _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(p => p.YeuCauLinhVatTuId == phieuLinhThuongVatTu.YeuCauLinhVatTuId)
                        .Select(s => new ThongTinLinhVatTuChiTiet
                        {
                            Ma = s.VatTuBenhVien.VatTus.Ma,
                            TenThuocHoacVatTu = s.VatTuBenhVien.VatTus.Ten
                                             + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NhaSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "")
                                             + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NuocSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                            DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                            SLYeuCau = s.SoLuong,
                            LaVatTuBHYT = s.LaVatTuBHYT
                        }).OrderByDescending(z => z.LaVatTuBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();
                query = queryVT.Where(z => z.LaVatTuBHYT).Concat(queryVT.Where(z => !z.LaVatTuBHYT)).ToList();
            }
            else // lĩnh bù
            {
                query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.YeuCauLinhVatTuId == phieuLinhThuongVatTu.YeuCauLinhVatTuId
                                    && x.YeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien
                                    //&& x.YeuCauVatTuBenhVien.KhongLinhBu != true
                                    //&& (x.YeuCauVatTuBenhVien.SoLuongDaLinhBu == null || x.YeuCauVatTuBenhVien.SoLuongDaLinhBu < x.YeuCauVatTuBenhVien.SoLuong)
                                    )
                     .Select(s => new ThongTinLinhVatTuChiTiet
                     {
                         VatTuBenhVienId = s.VatTuBenhVienId,
                         Ma = s.VatTuBenhVien.Ma,
                         TenThuocHoacVatTu = s.VatTuBenhVien.VatTus.Ten
                                             + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NhaSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "")
                                             + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NuocSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                         DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                         SLYeuCau = s.SoLuong,
                         LaVatTuBHYT = s.LaVatTuBHYT
                     }).OrderByDescending(p => p.LaVatTuBHYT).ThenBy(p => !p.LaVatTuBHYT)
                     .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.DVT })
                     .Select(item => new ThongTinLinhVatTuChiTiet()
                     {
                         Ma = item.First().Ma,
                         TenThuocHoacVatTu = item.First().TenThuocHoacVatTu,
                         DVT = item.First().DVT,
                         SLYeuCau = item.Sum(x => x.SLYeuCau).Value.MathRoundNumber(2),
                         LaVatTuBHYT = item.First().LaVatTuBHYT
                     }).OrderByDescending(z => z.LaVatTuBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();
            }
            if (phieuLinhThuongVatTu.LoaiPhieuLinh == 1)
            {
                if (query.Any(p => p.LaVatTuBHYT == true))
                {
                    infoLinhDuocChiTiet += headerBHYT;
                    foreach (var item in query)
                    {
                        if (item.LaVatTuBHYT == true)
                        {
                            infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucPhat
                                                + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                                + "</tr>";
                            STT++;
                            groupThuocBHYT = string.Empty;
                        }

                    }
                }
                if (query.Any(p => p.LaVatTuBHYT == false))
                {
                    infoLinhDuocChiTiet += headerKhongBHYT;
                    foreach (var item in query)
                    {
                        if (item.LaVatTuBHYT == false)
                        {
                            infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                               + "<tr style = 'border: 1px solid #020000;text-align: center; '>"
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                               + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                               + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                               + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucPhat
                                               + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                               + "</tr>";
                            STT++;
                            groupThuocKhongBHYT = string.Empty;
                        }
                    }
                }
                var data = new PhieuLinhThuongDuocPhamData
                {
                    HeaderPhieuLinhThuoc = hearder,
                    MaVachPhieuLinh = "Số: " + getPhieuLinh.SoPhieu,
                    ThuocHoacVatTu = infoLinhDuocChiTiet,
                    LogoUrl = phieuLinhThuongVatTu.HostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(getPhieuLinh.SoPhieu),
                    TuNgay = getPhieuLinh.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDate(),
                    DenNgay = getPhieuLinh.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDate(),
                    DienGiai = getPhieuLinh.GhiChu,
                    NoiGiao = getPhieuLinh.KhoXuat.Ten,
                    NhapVeKho = getPhieuLinh.KhoNhap.Ten,
                    CongKhoan = "Cộng khoản: " + (STT - 1).ToString() + " khoản",
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
            }
            else
            {
                foreach (var item in query)
                {
                    infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                            + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                            + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"/*item.SLThucPhat*/
                                            + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                            + "</tr>";
                    STT++;
                }
                var data = new PhieuLinhThuongDuocPhamData
                {
                    HeaderPhieuLinhThuoc = hearder,
                    MaVachPhieuLinh = "Số: " + getPhieuLinh.SoPhieu,
                    ThuocHoacVatTu = infoLinhDuocChiTiet,
                    LogoUrl = phieuLinhThuongVatTu.HostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(getPhieuLinh.SoPhieu),
                    TuNgay = getPhieuLinh.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDate(),
                    DenNgay = getPhieuLinh.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDate(),
                    DienGiai = getPhieuLinh.GhiChu,
                    NoiGiao = getPhieuLinh.KhoXuat.Ten,
                    NhapVeKho = getPhieuLinh.KhoNhap.Ten,
                    CongKhoan = "Cộng khoản: " + (STT - 1).ToString() + " khoản",
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
            }
            return content;
        }

        public LinhThuongVatTuGridVo LinhThuongVatTuGridVo(LinhThuongVatTuGridVo model)
        {
            var soLuongTonVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == model.VatTuBenhVienId && o.LaVatTuBHYT == model.LaVatTuBHYT && o.NhapKhoVatTu.KhoId == model.KhoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();
            var result = new LinhThuongVatTuGridVo
            {
                VatTuBenhVienId = model.VatTuBenhVienId,
                Ten = model.Ten,
                Ma = model.Ma,
                DVT = model.DVT,
                NhaSX = model.NhaSX,
                NuocSX = model.NuocSX,
                SLYeuCau = model.SLYeuCau,
                KhoXuatId = model.KhoXuatId,
                SLTon = soLuongTonVatTu,
                Nhom = model.LoaiVatTu == 1 ? "Vật Tư Không BHYT" : "Vật Tư BHYT",
                LaVatTuBHYT = model.LaVatTuBHYT
            };
            return result;
        }

        public double GetSoLuongTonVatTuGridVo(long vatTuBenhVienId, long khoXuatId, bool laVatTuBHYT)
        {
            return _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();
        }
        #region linh bu xem truoc
        public string InPhieuLinhBuVatTuXemTruoc(PhieuLinhThuongVatTuXemTruoc phieuLinhThuongVatTuXemTruoc)
        {
            var content = string.Empty;
            var hearder = string.Empty;
            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongVatTu")).First();

            var infoLinhDuocChiTiet = string.Empty;

            var STT = 1;
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                          .Where(x => x.KhoLinhId == phieuLinhThuongVatTuXemTruoc.KhoLinhBuId
                               && x.YeuCauLinhVatTuId == null
                               && x.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien
                               && (!x.YeuCauLinhVatTuChiTiets.Any() || x.YeuCauLinhVatTuChiTiets.Any(p => p.YeuCauLinhVatTu.DuocDuyet != null))
                               && x.KhongLinhBu != true
                               && x.SoLuong > 0
                               && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                               && phieuLinhThuongVatTuXemTruoc.YeuCauVatTuBenhVienIds.Contains(x.Id))
                           .Select(s => new ThongTinLinhVatTuChiTiet
                           {
                               VatTuBenhVienId = s.VatTuBenhVienId,
                               Ma = s.VatTuBenhVien.Ma,
                               TenThuocHoacVatTu = s.VatTuBenhVien.VatTus.Ten
                                                    + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NhaSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "")
                                                    + (!string.IsNullOrEmpty(s.VatTuBenhVien.VatTus.NuocSanXuat) ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                               DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                               SLYeuCau = s.SoLuong,
                               LaVatTuBHYT = s.LaVatTuBHYT
                           }).OrderByDescending(p => p.LaVatTuBHYT).ThenBy(p => !p.LaVatTuBHYT)
                            .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.DVT })
                            .Select(item => new ThongTinLinhVatTuChiTiet()
                            {
                                VatTuBenhVienId = item.First().VatTuBenhVienId,
                                Ma = item.First().Ma,
                                TenThuocHoacVatTu = item.First().TenThuocHoacVatTu,
                                DVT = item.First().DVT,
                                SLYeuCau = item.Sum(x => x.SLYeuCau).Value.MathRoundNumber(2),
                                LaVatTuBHYT = item.First().LaVatTuBHYT
                            }).ToList();
            var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == phieuLinhThuongVatTuXemTruoc.KhoLinhId
                       && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var result = query.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaVatTuBHYT));

            foreach (var item in result.ToList())
            {
                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                        + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                        + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                        + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                        + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucPhat
                                        + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                        + "</tr>";
                STT++;

            }
            var khoXuat = _khoRepository.TableNoTracking.Where(d => d.Id == phieuLinhThuongVatTuXemTruoc.KhoLinhBuId).Select(d => d.Ten).FirstOrDefault();
            var khoLinh = _khoRepository.TableNoTracking.Where(d => d.Id == phieuLinhThuongVatTuXemTruoc.KhoLinhId).Select(d => d.Ten).FirstOrDefault();

            var data = new PhieuLinhThuongDuocPhamData
            {
                HeaderPhieuLinhThuoc = hearder,
                MaVachPhieuLinh = "Số: " + "",
                ThuocHoacVatTu = infoLinhDuocChiTiet,
                LogoUrl = phieuLinhThuongVatTuXemTruoc.HostingName + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = "",
                TuNgay = phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopTuNgay.ApplyFormatDateTimeSACH(),
                DenNgay = phieuLinhThuongVatTuXemTruoc.ThoiDiemLinhTongHopDenNgay.ApplyFormatDateTimeSACH(),
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
    }
}
