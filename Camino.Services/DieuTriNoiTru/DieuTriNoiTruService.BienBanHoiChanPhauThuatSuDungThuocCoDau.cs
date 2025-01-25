using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.ToaThuocMau;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<List<ThuocPhieuDieuTriBenhNhanItemVo>> GetDuocPhamBenhNhanNoiTru(DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var listPhieuDieuTriId = _noiTruBenhAnRepository.TableNoTracking.Where(x => x.Id == yeuCauTiepNhanId)
                                                                             .SelectMany(s => s.NoiTruPhieuDieuTris).Select(k => k.Id).ToList();
            var listDuocPhamDaDieuTriIds = new List<long>();
            foreach (var item in listPhieuDieuTriId)
            {
                var lstIds =
                _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(s => s.NoiTruPhieuDieuTriId == item && s.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                   .Select(x => x.DuocPhamBenhVienId);
                if (lstIds.Any())
                {
                    listDuocPhamDaDieuTriIds.AddRange(lstIds);
                }
            }
            var lstColumnNameSearch = new List<string>
                {
                    nameof(DuocPham.Ten),
                    nameof(DuocPham.HoatChat)
                };


            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var duocPhamId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                    var duocPhams = _duocPhamRepository.TableNoTracking
                         .Where(d => listDuocPhamDaDieuTriIds.Any(dx => dx == d.Id))
                    .OrderByDescending(x => duocPhamId == 0 || x.Id == duocPhamId ).ThenBy(x => x.Id)
                    .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id.ToString(),
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        DVT = item.DonViTinh.Ten,
                        DuongDung = item.DuongDung.Ten,
                        HamLuong = item.HamLuong,
                        NhaSanXuat = item.NhaSanXuat,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    })
                    .ApplyLike(queryInfo.Query, o => o.HoatChat, o => o.Ten, o => o.DisplayName)
                    .Take(queryInfo.Take);
                    return await duocPhams.ToListAsync();
                }
                else
                {
                    var duocPhams = _duocPhamRepository.TableNoTracking
                        .Where(d => listDuocPhamDaDieuTriIds.Any(dx => dx == d.Id))
                    .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                    .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id.ToString(),
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        DVT = item.DonViTinh.Ten,
                        DuongDung = item.DuongDung.Ten,
                        HamLuong = item.HamLuong,
                        NhaSanXuat = item.NhaSanXuat,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    })
                    .ApplyLike(queryInfo.Query, o => o.HoatChat, o => o.Ten, o => o.DisplayName)
                    .Take(queryInfo.Take);
                    return await duocPhams.ToListAsync();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var duocPhamId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                    var duocPhamIds = _duocPhamRepository
                               .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                               .Where(d => listDuocPhamDaDieuTriIds.Any(dx => dx == d.Id))
                               .Select(p => p.Id).ToList();

                    var dictionary = duocPhamIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var duocPhams = _duocPhamRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                                         .Where(d => listDuocPhamDaDieuTriIds.Any(dx => dx == d.Id))
                                        .OrderByDescending(x => duocPhamId == 0 || x.Id == duocPhamId ).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Ten,
                                            KeyId = item.Id.ToString(),
                                            Ten = item.Ten,
                                            HoatChat = item.HoatChat,
                                            DVT = item.DonViTinh.Ten,
                                            DuongDung = item.DuongDung.Ten,
                                            HamLuong = item.HamLuong,
                                            NhaSanXuat = item.NhaSanXuat,
                                            DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                                            SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                                            MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                                            DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                                        });
                    return await duocPhams.ToListAsync();
                }
                else
                {
                    var duocPhamIds = _duocPhamRepository
                               .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                               .Where(d=> listDuocPhamDaDieuTriIds.Any(dx => dx == d.Id))
                               .Select(p => p.Id).ToList();

                    var dictionary = duocPhamIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var duocPhams = _duocPhamRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                                         .Where(d => listDuocPhamDaDieuTriIds.Any(dx => dx == d.Id))
                                         //.OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                                         .OrderByDescending(x => x.Id == queryInfo.Id)
                                        .ThenBy(p => duocPhamIds.IndexOf(p.Id) != -1 ? duocPhamIds.IndexOf(p.Id) : queryInfo.Take + 1)
                                        .Take(queryInfo.Take)
                                        .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Ten,
                                            KeyId = item.Id.ToString(),
                                            Ten = item.Ten,
                                            HoatChat = item.HoatChat,
                                            DVT = item.DonViTinh.Ten,
                                            DuongDung = item.DuongDung.Ten,
                                            HamLuong = item.HamLuong,
                                            NhaSanXuat = item.NhaSanXuat,
                                            DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                                            SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                                            MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                                            DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                                        });
                    return await duocPhams.ToListAsync();
                }
            }

        }
        public async Task<List<ThuocPhieuDieuTriBenhNhanItemVo>> GetDuocPhamCoDau(DropDownListRequestModel queryInfo)
        {
           
            var lstColumnNameSearch = new List<string>
                {
                    nameof(DuocPham.Ten),
                    nameof(DuocPham.HoatChat)
                };


            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var duocPhamId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                    var duocPhams = _duocPhamRepository.TableNoTracking
                         .Where(d => d.DuocPhamCoDau == true)
                    .OrderByDescending(x => duocPhamId == 0 || x.Id == duocPhamId).ThenBy(x => x.Id)
                    .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id.ToString(),
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        DVT = item.DonViTinh.Ten,
                        DuongDung = item.DuongDung.Ten,
                        HamLuong = item.HamLuong,
                        NhaSanXuat = item.NhaSanXuat,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    })
                    .ApplyLike(queryInfo.Query, o => o.HoatChat, o => o.Ten, o => o.DisplayName)
                    .Take(queryInfo.Take);
                    return await duocPhams.ToListAsync();
                }
                else
                {
                    var duocPhams = _duocPhamRepository.TableNoTracking
                        .Where(d => d.DuocPhamCoDau == true)
                    .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                    .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id.ToString(),
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        DVT = item.DonViTinh.Ten,
                        DuongDung = item.DuongDung.Ten,
                        HamLuong = item.HamLuong,
                        NhaSanXuat = item.NhaSanXuat,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    })
                    .ApplyLike(queryInfo.Query, o => o.HoatChat, o => o.Ten, o => o.DisplayName)
                    .Take(queryInfo.Take);
                    return await duocPhams.ToListAsync();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var duocPhamId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                    var duocPhamIds = _duocPhamRepository
                               .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                               .Where(d => d.DuocPhamCoDau == true)
                               .Select(p => p.Id).ToList();

                    var dictionary = duocPhamIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var duocPhams = _duocPhamRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                                        .Where(d => d.DuocPhamCoDau == true)
                                        .OrderByDescending(x => duocPhamId == 0 || x.Id == duocPhamId).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Ten,
                                            KeyId = item.Id.ToString(),
                                            Ten = item.Ten,
                                            HoatChat = item.HoatChat,
                                            DVT = item.DonViTinh.Ten,
                                            DuongDung = item.DuongDung.Ten,
                                            HamLuong = item.HamLuong,
                                            NhaSanXuat = item.NhaSanXuat,
                                            DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                                            SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                                            MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                                            DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                                        });
                    return await duocPhams.ToListAsync();
                }
                else
                {
                    var duocPhamIds = _duocPhamRepository
                               .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                               .Where(d => d.DuocPhamCoDau == true)
                               .Select(p => p.Id).ToList();

                    var dictionary = duocPhamIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var duocPhams = _duocPhamRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                                         .Where(d => d.DuocPhamCoDau == true)
                                         //.OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                                         .OrderByDescending(x => x.Id == queryInfo.Id)
                                        .ThenBy(p => duocPhamIds.IndexOf(p.Id) != -1 ? duocPhamIds.IndexOf(p.Id) : queryInfo.Take + 1)
                                        .Take(queryInfo.Take)
                                        .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Ten,
                                            KeyId = item.Id.ToString(),
                                            Ten = item.Ten,
                                            HoatChat = item.HoatChat,
                                            DVT = item.DonViTinh.Ten,
                                            DuongDung = item.DuongDung.Ten,
                                            HamLuong = item.HamLuong,
                                            NhaSanXuat = item.NhaSanXuat,
                                            DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                                            SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                                            MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                                            DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                                        });
                    return await duocPhams.ToListAsync();
                }
            }

        }
        public async Task<ThuocPhieuDieuTriBenhNhanItemVo> GetDuocPhamCoDauVo(long duocPhamId)
        {
            var duocPhams = _duocPhamRepository.TableNoTracking
                                        .Where(d => d.DuocPhamCoDau == true && d.Id == duocPhamId)
                                       .Select(item => new ThuocPhieuDieuTriBenhNhanItemVo
                                       {
                                           DisplayName = item.Ten,
                                           KeyId = item.Id.ToString(),
                                           Ten = item.Ten,
                                           HoatChat = item.HoatChat,
                                           DVT = item.DonViTinh.Ten,
                                           DuongDung = item.DuongDung.Ten,
                                           HamLuong = item.HamLuong,
                                           NhaSanXuat = item.NhaSanXuat,
                                           DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                                           SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                                           MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                                           DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                                       });
            return await duocPhams.FirstOrDefaultAsync();
        }
        private async Task<DataInPhieuDieuTriVaSerivcesVo> ThongTinBenhNhan(long yeuCauTiepNhanId)
        {
            var thongTinBenhNhanPhieuThuoc = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(s => s.Id == yeuCauTiepNhanId)
                .Select(s => new DataInPhieuDieuTriVaSerivcesVo
                {
                    HoTenNgBenh = s.HoTen,
                    NamSinh = s.NamSinh,
                    GTNgBenh = s.GioiTinh.GetDescription(),
                    GioiTinh = s.GioiTinh,
                    DiaChi = s.BenhNhan.DiaChiDayDu,
                    Cmnd = s.SoChungMinhThu,
                    MaBn = s.BenhNhan.MaBN,
                    NhomMau = s.NhomMau != null ? s.NhomMau.GetDescription() : string.Empty,
                    MaSoTiepNhan = s.MaYeuCauTiepNhan,
                    NgayVaoVien = s.NoiTruBenhAn.ThoiDiemNhapVien,
                    NgayRaVien = s.NoiTruBenhAn.ThoiDiemRaVien,
                    ChanDoanRaVien = s.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu,
                    ChanDoanVaoVien = s.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    Buong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                    Giuong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.Ten).FirstOrDefault()
                });
            var thongTinBenhNhan = await thongTinBenhNhanPhieuThuoc.FirstAsync();
            return thongTinBenhNhan;
        }
        public async Task<string> PhieuInBienBanHoiChanPhauThuatCoDau(BangTheoDoiHoiTinhHttpParamsRequest phieuInBienBanHoiChanParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("BienBanHoiChanPhauThuatCoDau"));
            var result = await _yeuCauTiepNhanRepository.GetByIdAsync(phieuInBienBanHoiChanParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );
            var infoBn = await ThongTinBenhNhan(phieuInBienBanHoiChanParams.YeuCauTiepNhanId);
            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;
            infoBn.Giuong = result.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                ? result.YeuCauDichVuGiuongBenhViens.OrderBy(x => x.Id).LastOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)?.GiuongBenh.Ten
                : string.Empty;
            infoBn.Buong = result.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                ? result.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.PhongBenhVien.Ten
                : string.Empty;
            BienBanHoiChanSuDungThuocCoDauGidVo bienBanhoiChanVo;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == phieuInBienBanHoiChanParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuatSuDungThuocCoDau && bv.Id == phieuInBienBanHoiChanParams.IdNoiTruHoSo)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();

            bienBanhoiChanVo = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<BienBanHoiChanSuDungThuocCoDauGidVo>(thongTinHoSo)
                : new BienBanHoiChanSuDungThuocCoDauGidVo();
            var khoa = infoBn.Khoa;
            var giuong = infoBn.Giuong;
            var buong = infoBn.Buong;
            // thuoc đã điều trị
            //to do
            var thuocDaDieuTri = string.Empty;
            if (bienBanhoiChanVo.ThuocDaDieuTris.Any())
            {
                var duocPhams = _duocPhamRepository.TableNoTracking
                        .Where(d => bienBanhoiChanVo.ThuocDaDieuTris.Any(ds => ds == d.Id.ToString()))
                   .Select(item => item.Ten);
                thuocDaDieuTri = duocPhams.ToList().Join("; ");
            }
            //
            //
            // thuốc có dấu
            //to do
            var thuocCoDau = string.Empty;
            if(bienBanhoiChanVo.ListThemThuocCoDau .Any())
            {
                var thuocCD = bienBanhoiChanVo.ListThemThuocCoDau.Select(item => "(" + (!string.IsNullOrEmpty(item.LyDo) ? item.LyDo + ", " : "")
                                      + (!string.IsNullOrEmpty(item.TenThuoc) ? item.TenThuoc + ", " : "")
                                      + (!string.IsNullOrEmpty(item.HamLuong) ? item.HamLuong + ", " : "")
                                      + (!string.IsNullOrEmpty(item.DuongDung) ? item.DuongDung + ", " : "") + ")");
                thuocCoDau = thuocCD.ToList().Join("; ");
            }
            //
            //
            
             var tuNgayDieuTri = result.NoiTruBenhAn?.ThoiDiemNhapVien;
            var uyQuyen = string.Empty;
            var bsDieuTri = string.Empty;
            uyQuyen = _nhanVienRepository.TableNoTracking.Where(d => d.Id == (long)bienBanhoiChanVo.LanhDao).Select(d => d.User.HoTen).FirstOrDefault();

            bsDieuTri = _nhanVienRepository.TableNoTracking.Where(d => d.Id == (long)bienBanhoiChanVo.BsDieuTri).Select(d => d.User.HoTen).FirstOrDefault();

            var data = new 
            {
                Tuoi = DateTime.Now.Year - infoBn.NamSinh,
                HoTen = infoBn.HoTenNgBenh,
                GioiTinh = infoBn.GTNgBenh,
                DiaChi = infoBn.DiaChi,
                KhoaDieuTri = khoa,
                Giuong = giuong,
                Buong = buong,
                NgayVaoVien = tuNgayDieuTri?.ApplyFormatDateTimeSACH(),
                NgayHoiChan = bienBanhoiChanVo.HoiChanLuc?.ApplyFormatDateTimeSACH(),
                ChuanDoanBenh = bienBanhoiChanVo.ChanDoanBenh,
                TomTat = bienBanhoiChanVo.TomTatTinhTrangBenhNhanKhiHoiChan,
                ThuocDaDieuTri = thuocDaDieuTri,

                ChuanDoanBenhSauHoiChan = bienBanhoiChanVo.ChanDoanBenhSauHoiChan,
                ChiDinhThuocCoDau = thuocCoDau,
                Ngay = DateTime.Now.Day,
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                UyQuyen = uyQuyen,
                BsDieuTri = bsDieuTri
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
    }
}
