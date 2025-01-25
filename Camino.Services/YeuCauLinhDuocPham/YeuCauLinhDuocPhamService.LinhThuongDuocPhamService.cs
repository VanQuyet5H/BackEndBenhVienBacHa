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
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.LinhDuocPham;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial class YeuCauLinhDuocPhamService
    {
        public async Task<List<LookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoaPhongId = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == noiLamViecCurrentId).Select(z => z.KhoaPhongId).FirstOrDefault();
            var khoId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);

            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                .Where(p => p.NhanVienId == userCurrentId && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) && p.Kho.KhoaPhongId == khoaPhongId && p.Kho.LoaiDuocPham == true)
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
                        .Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && p.LoaiDuocPham == true)
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
            var getHoTen = await _userRepository.TableNoTracking.Where(p => p.Id == _userAgentHelper.GetCurrentUserId()).Select(p => new NhanVienYeuCauVo
            {
                NhanVienYeuCauId = _userAgentHelper.GetCurrentUserId(),
                HoTen = p.HoTen
            }).FirstOrDefaultAsync();
            return getHoTen;
        }

        public async Task<NhanVienDuyetVo> GetNhanVienDuyet(long? id)
        {
            var result = await BaseRepository.TableNoTracking.Where(p => p.Id == id).Select(s => new NhanVienDuyetVo
            {
                NhanVienDuyetId = s.NhanVienDuyetId,
                HoTenNguoiDuyet = s.NhanVienDuyet.User.HoTen,
                NgayDuyet = s.NgayDuyet
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<DuocPhamLookupVo>> GetDuocPham(DropDownListRequestModel queryInfo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var lstColumnNameSearch = new List<string>
            {
               nameof(DuocPham.Ten),
               nameof(DuocPham.MaHoatChat),
               nameof(DuocPham.HoatChat),
            };
            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                var info = JsonConvert.DeserializeObject<DuocPhamJsonVo>(queryInfo.ParameterDependencies);
                var laDuocPhamBHYT = false;
                if (info.LaDuocPhamBHYT == 2) // Thuốc BHYT
                {
                    laDuocPhamBHYT = true;
                }
                if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
                {
                    var result = _duocPhamBenhVienRepository.TableNoTracking
                       .Where(p => p.NhapKhoDuocPhamChiTiets.Any(ct => ct.NhapKhoDuocPhams.KhoId == info.KhoId && ct.SoLuongDaXuat < ct.SoLuongNhap && ct.LaDuocPhamBHYT == laDuocPhamBHYT
                                    && ct.HanSuDung >= DateTime.Now && ct.NhapKhoDuocPhams.DaHet != true))
                       .Select(s => new DuocPhamLookupVo
                       {
                           KeyId = s.Id,
                           //DisplayName = s.DuocPham.Ten + " - " + s.DuocPham.HoatChat,
                           DisplayName = s.DuocPham.Ten,
                           Ma = s.Ma,
                           Ten = s.DuocPham.Ten,
                           HoatChat = s.DuocPham.HoatChat,
                           DVTId = s.DuocPham.DonViTinhId,
                           DVT = s.DuocPham.DonViTinh.Ten,
                           DuongDungId = s.DuocPham.DuongDungId,
                           DuongDung = s.DuocPham.DuongDung.Ten,
                           //HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                           // nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT &&
                           // nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                           // .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First().HanSuDung.ApplyFormatDate(),
                           //SLTon = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2),
                           NhaSX = s.DuocPham.NhaSanXuat,
                           NuocSX = s.DuocPham.NuocSanXuat,
                           HamLuong = s.DuocPham.HamLuong,
                           LaDuocPhamBHYT = laDuocPhamBHYT,
                           MayXetNghiemTenVaIds = s.DuocPhamBenhVienMayXetNghiems.Select(item => item.MayXetNghiemId + "-" + item.MayXetNghiem.Ten).ToList(),
                       })
                       .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.HoatChat, o => o.Ma)
                       .Take(queryInfo.Take);

                    //return await result.ToListAsync();

                    var duocPhamTrongKhos = await result.ToListAsync();
                    if (duocPhamTrongKhos.Any())
                    {
                        var duocPhamBenhVienIds = duocPhamTrongKhos.Select(o => o.KeyId).ToList();

                        var duocPhamTrongKhoChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(o => o.NhapKhoDuocPhams.KhoId == info.KhoId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && o.LaDuocPhamBHYT == laDuocPhamBHYT && o.HanSuDung >= DateTime.Now)
                            .Select(o => new
                            {
                                Id = o.Id,
                                DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                                SoLuongDaXuat = o.SoLuongDaXuat,
                                SoLuongNhap = o.SoLuongNhap,
                                HanSuDung = o.HanSuDung,
                                NgayNhapVaoBenhVien = o.NgayNhapVaoBenhVien,
                            })
                            .ToList();
                        foreach (var dp in duocPhamTrongKhos)
                        {
                            dp.SLTon = duocPhamTrongKhoChiTiets.Where(o => o.DuocPhamBenhVienId == dp.KeyId).Select(o => o.SoLuongNhap - o.SoLuongDaXuat).DefaultIfEmpty().Sum().MathRoundNumber(2);
                            var hanSuDung = duocPhamTrongKhoChiTiets
                                .Where(o => o.DuocPhamBenhVienId == dp.KeyId && o.SoLuongDaXuat.MathRoundNumber(2) < o.SoLuongNhap.MathRoundNumber(2))
                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .Select(p => p.HanSuDung).FirstOrDefault();
                            if (hanSuDung != null)
                                dp.HanSuDung = hanSuDung.ApplyFormatDate();
                        }
                    }

                    return duocPhamTrongKhos;

                }
                else
                {
                    var lstDuocPhamId = await _duocPhamRepository
                      .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                      .Select(s => s.Id).ToListAsync();

                    var dct = lstDuocPhamId.Select((p, i) => new
                    {
                        key = p,
                        rank = i
                    }).ToDictionary(o => o.key, o => o.rank);

                    var duocPhamBenhViens = _duocPhamBenhVienRepository.TableNoTracking
                     .Where(o => o.DuocPham.DuocPhamBenhVien.HieuLuc == true && lstDuocPhamId.Any(x => x == o.Id) && o.NhapKhoDuocPhamChiTiets.Any(kho => kho.NhapKhoDuocPhams.KhoId == info.KhoId
                                                                           && kho.SoLuongDaXuat < kho.SoLuongNhap
                                                                           && kho.LaDuocPhamBHYT == laDuocPhamBHYT
                                                                           && kho.HanSuDung >= DateTime.Now))
                     .Select(s => new DuocPhamLookupVo
                     {
                         KeyId = s.Id,
                         DisplayName = s.DuocPham.Ten,
                         Ma = s.Ma,
                         Ten = s.DuocPham.Ten,
                         HoatChat = s.DuocPham.HoatChat,
                         DVTId = s.DuocPham.DonViTinhId,
                         DVT = s.DuocPham.DonViTinh.Ten,
                         DuongDungId = s.DuocPham.DuongDungId,
                         DuongDung = s.DuocPham.DuongDung.Ten,
                         HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                          nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT &&
                          nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap).
                            OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First().HanSuDung.ApplyFormatDate(),
                         SLTon = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                         NhaSX = s.DuocPham.NhaSanXuat,
                         NuocSX = s.DuocPham.NuocSanXuat,
                         HamLuong = s.DuocPham.HamLuong,
                         LaDuocPhamBHYT = laDuocPhamBHYT,

                         MayXetNghiemTenVaIds = s.DuocPhamBenhVienMayXetNghiems.Select(item => item.MayXetNghiemId + "-" + item.MayXetNghiem.Ten).ToList(),
                     })
                      .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                      .Take(queryInfo.Take);
                    return await duocPhamBenhViens.ToListAsync();
                }
            }
            else
            {
                return new List<DuocPhamLookupVo>();
            }
        }

        public async Task<List<DuocPhamLookupVo>> GetDuocPhamOld(DropDownListRequestModel queryInfo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var lstColumnNameSearch = new List<string>
            {
               nameof(DuocPham.Ten),
               nameof(DuocPham.MaHoatChat),
               nameof(DuocPham.HoatChat),
            };
            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                var info = JsonConvert.DeserializeObject<DuocPhamJsonVo>(queryInfo.ParameterDependencies);
                var laDuocPhamBHYT = false;
                if (info.LaDuocPhamBHYT == 2) // Thuốc BHYT
                {
                    laDuocPhamBHYT = true;
                }
                if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
                {
                    var result = _duocPhamBenhVienRepository.TableNoTracking
                       .Where(p => p.NhapKhoDuocPhamChiTiets.Any(ct => ct.NhapKhoDuocPhams.KhoId == info.KhoId && ct.SoLuongDaXuat < ct.SoLuongNhap && ct.LaDuocPhamBHYT == laDuocPhamBHYT
                                    && ct.HanSuDung >= DateTime.Now && ct.NhapKhoDuocPhams.DaHet != true))
                       .Select(s => new DuocPhamLookupVo
                       {
                           KeyId = s.Id,
                           //DisplayName = s.DuocPham.Ten + " - " + s.DuocPham.HoatChat,
                           DisplayName = s.DuocPham.Ten,
                           Ma = s.MaDuocPhamBenhVien,
                           Ten = s.DuocPham.Ten,
                           HoatChat = s.DuocPham.HoatChat,
                           DVTId = s.DuocPham.DonViTinhId,
                           DVT = s.DuocPham.DonViTinh.Ten,
                           DuongDungId = s.DuocPham.DuongDungId,
                           DuongDung = s.DuocPham.DuongDung.Ten,
                           HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                            nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT &&
                            nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First().HanSuDung.ApplyFormatDate(),
                           SLTon = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2),
                           NhaSX = s.DuocPham.NhaSanXuat,
                           NuocSX = s.DuocPham.NuocSanXuat,
                           HamLuong = s.DuocPham.HamLuong,
                           LaDuocPhamBHYT = laDuocPhamBHYT,
                           MayXetNghiemTenVaIds = s.DuocPhamBenhVienMayXetNghiems.Select(item => item.MayXetNghiemId + "-" + item.MayXetNghiem.Ten).ToList(),
                       })
                       .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.HoatChat, o => o.Ma)
                       .Take(queryInfo.Take);

                    return await result.ToListAsync();
                }
                else
                {
                    var lstDuocPhamId = await _duocPhamRepository
                      .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                      .Select(s => s.Id).ToListAsync();

                    var dct = lstDuocPhamId.Select((p, i) => new
                    {
                        key = p,
                        rank = i
                    }).ToDictionary(o => o.key, o => o.rank);

                    var duocPhamBenhViens = _duocPhamBenhVienRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                     .Where(o => o.DuocPham.DuocPhamBenhVien.HieuLuc == true && lstDuocPhamId.Any(x => x == o.Id) && o.NhapKhoDuocPhamChiTiets.Any(kho => kho.NhapKhoDuocPhams.KhoId == info.KhoId
                                                                           && kho.SoLuongDaXuat < kho.SoLuongNhap
                                                                           && kho.LaDuocPhamBHYT == laDuocPhamBHYT
                                                                           && kho.HanSuDung >= DateTime.Now))
                     .Select(s => new DuocPhamLookupVo
                     {
                         KeyId = s.Id,
                         DisplayName = s.DuocPham.Ten,
                         Ma = s.DuocPham.DuocPhamBenhVien != null ? s.DuocPham.DuocPhamBenhVien.MaDuocPhamBenhVien : "",// đi đường vòng, fix bug fulltext search
                         Ten = s.DuocPham.Ten,
                         HoatChat = s.DuocPham.HoatChat,
                         DVTId = s.DuocPham.DonViTinhId,
                         DVT = s.DuocPham.DonViTinh.Ten,
                         DuongDungId = s.DuocPham.DuongDungId,
                         DuongDung = s.DuocPham.DuongDung.Ten,
                         HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                          nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT &&
                          nkct.HanSuDung >= DateTime.Now && nkct.SoLuongDaXuat < nkct.SoLuongNhap).
                            OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).First().HanSuDung.ApplyFormatDate(),
                         SLTon = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == info.KhoId && nkct.LaDuocPhamBHYT == laDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                         NhaSX = s.DuocPham.NhaSanXuat,
                         NuocSX = s.DuocPham.NuocSanXuat,
                         HamLuong = s.DuocPham.HamLuong,
                         LaDuocPhamBHYT = laDuocPhamBHYT,

                         MayXetNghiemTenVaIds = s.DuocPhamBenhVienMayXetNghiems.Select(item => item.MayXetNghiemId + "-" + item.MayXetNghiem.Ten).ToList(),
                     })
                      .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                      .Take(queryInfo.Take);
                    return await duocPhamBenhViens.ToListAsync();
                }
            }
            else
            {
                return new List<DuocPhamLookupVo>();
            }
        }

        public LinhThuongDuocPhamVo LinhThuongDuocPhamGridVo(LinhThuongDuocPhamVo model)
        {

            var soLuongTonDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == model.DuocPhamBenhVienId && o.LaDuocPhamBHYT == model.LaDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == model.KhoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();
            var result = new LinhThuongDuocPhamVo
            {
                DuocPhamBenhVienId = model.DuocPhamBenhVienId,
                Ten = model.Ten,
                HamLuong = model.HamLuong,
                HoatChat = model.HoatChat,
                DuongDungId = model.DuongDungId,
                DuongDung = model.DuongDung,
                DVTId = model.DVTId,
                DVT = model.DVT,
                NhaSX = model.NhaSX,
                NuocSX = model.NuocSX,
                SLYeuCau = model.SLYeuCau,
                SLTon = soLuongTonDuocPham,
                Nhom = model.LoaiDuocPham == 1 ? "Thuốc Không BHYT" : "Thuốc BHYT",
                LaDuocPhamBHYT = model.LaDuocPhamBHYT
            };
            return result;
        }

        public string InPhieuLinhThuongDuocPham(PhieuLinhThuongDuocPham phieuLinhThuongDuoc)
        {
            var content = string.Empty;
            var contentGayNghien = string.Empty;
            var hearder = string.Empty;
            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU LĨNH DƯỢC PHẨM</div></div>";
            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongThuocDuocPham")).First();
            var templateLinhThuongGayNghien = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuocGayNghien")).First();

            int? nhomVatTu = 0;
            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }

            var getPhieuLinh = BaseRepository.GetById(phieuLinhThuongDuoc.YeuCauLinhDuocPhamId, s => s.Include(u => u.KhoNhap).Include(u => u.KhoXuat));
            if (phieuLinhThuongDuoc.Header == true)
            {
                //hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                //              "<th>PHIẾU LĨNH THUỐC</th>" +
                //         "</p>";
            }

            var groupThuocBHYT = "Thuốc BHYT";
            var groupThuocKhongBHYT = "Thuốc Không BHYT";

            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                        + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";
            var query = new List<ThongTinLinhDuocPhamChiTiet>();
            var queryGayNghien = new List<ThongTinLinhDuocPhamChiTiet>();

            if (phieuLinhThuongDuoc.LoaiPhieuLinh == 1) // lĩnh thường(dự trù)
            {
                var queryThuoc = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(p => p.YeuCauLinhDuocPhamId == phieuLinhThuongDuoc.YeuCauLinhDuocPhamId
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
                           .Where(p => p.YeuCauLinhDuocPhamId == phieuLinhThuongDuoc.YeuCauLinhDuocPhamId
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
            }
            else // lĩnh bù
            {
                query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.YeuCauLinhDuocPhamId == phieuLinhThuongDuoc.YeuCauLinhDuocPhamId
                                    && x.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                                    && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                     .Select(s => new ThongTinLinhDuocPhamChiTiet
                     {
                         DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                         Ma = s.DuocPhamBenhVien.Ma,
                         TenThuocHoacVatTu = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == nhomVatTu ?
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NhaSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                       (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NuocSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.HamLuong) ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                         DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                         SLYeuCau = s.SoLuong,
                         LaDuocPhamBHYT = s.LaDuocPhamBHYT
                     }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                     .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.DVT })
                     .Select(item => new ThongTinLinhDuocPhamChiTiet()
                     {
                         Ma = item.First().Ma,
                         TenThuocHoacVatTu = item.First().TenThuocHoacVatTu,
                         DVT = item.First().DVT,
                         SLYeuCau = item.Sum(x => x.SLYeuCau).MathRoundNumber(2),
                         LaDuocPhamBHYT = item.First().LaDuocPhamBHYT
                     }).OrderByDescending(z => z.LaDuocPhamBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();

                queryGayNghien = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                           .Where(x => x.YeuCauLinhDuocPhamId == phieuLinhThuongDuoc.YeuCauLinhDuocPhamId
                                   && x.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                                   && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                    .Select(s => new ThongTinLinhDuocPhamChiTiet
                    {
                        DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                        Ma = s.DuocPhamBenhVien.Ma,
                        TenThuocHoacVatTu = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == nhomVatTu ?
                                                                    s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NhaSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                      (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NuocSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                    s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.HamLuong) ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                        DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                        SLYeuCau = s.SoLuong,
                        LaDuocPhamBHYT = s.LaDuocPhamBHYT
                    }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                    .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.DVT })
                    .Select(item => new ThongTinLinhDuocPhamChiTiet()
                    {
                        Ma = item.First().Ma,
                        TenThuocHoacVatTu = item.First().TenThuocHoacVatTu,
                        DVT = item.First().DVT,
                        SLYeuCau = item.Sum(x => x.SLYeuCau).MathRoundNumber(2),
                        LaDuocPhamBHYT = item.First().LaDuocPhamBHYT
                    }).OrderByDescending(z => z.LaDuocPhamBHYT).ThenBy(z => z.TenThuocHoacVatTu).ToList();
            }
            if (phieuLinhThuongDuoc.LoaiPhieuLinh == 1)
            {
                if (query.Any())
                {
                    var STT = 1;
                    var infoLinhDuocChiTiet = string.Empty;
                    if (query.Any(p => p.LaDuocPhamBHYT == true))
                    {
                        infoLinhDuocChiTiet += headerBHYT;
                        foreach (var item in query)
                        {
                            if (item.LaDuocPhamBHYT == true)
                            {
                                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                    + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                    + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau.MathRoundNumber(2)
                                                    + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucPhat
                                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                                    + "</tr>";
                                STT++;
                                groupThuocBHYT = string.Empty;
                            }

                        }
                    }
                    if (query.Any(p => p.LaDuocPhamBHYT == false))
                    {
                        infoLinhDuocChiTiet += headerKhongBHYT;
                        foreach (var item in query)
                        {
                            if (item.LaDuocPhamBHYT == false)
                            {
                                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                   + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                   + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau.MathRoundNumber(2)
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
                        LogoUrl = phieuLinhThuongDuoc.HostingName + "/assets/img/logo-bacha-full.png",
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
                if (queryGayNghien.Any())
                {
                    var STT = 1;
                    var infoLinhDuocChiTiet = string.Empty;
                    if (queryGayNghien.Any(p => p.LaDuocPhamBHYT == true))
                    {
                        infoLinhDuocChiTiet += headerBHYT;
                        foreach (var item in queryGayNghien)
                        {
                            if (item.LaDuocPhamBHYT == true)
                            {
                                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                    + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                    + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLYeuCau.MathRoundNumber(2)), false)
                                                    + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucPhat.MathRoundNumber(2)), false)
                                                    + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                                    + "</tr>";
                                STT++;
                                groupThuocBHYT = string.Empty;
                            }

                        }
                    }
                    if (queryGayNghien.Any(p => p.LaDuocPhamBHYT == false))
                    {
                        infoLinhDuocChiTiet += headerKhongBHYT;
                        foreach (var item in queryGayNghien)
                        {
                            if (item.LaDuocPhamBHYT == false)
                            {
                                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                   + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                   + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLYeuCau.MathRoundNumber(2)), false)
                                                   + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"/*NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucPhat.MathRoundNumber(2)), false)*/
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
                        LogoUrl = phieuLinhThuongDuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(getPhieuLinh.SoPhieu),
                        //TuNgay = getPhieuLinh.NgayYeuCau.ApplyFormatDate(),
                        //DenNgay = getPhieuLinh.NgayDuyet?.ApplyFormatDate(),
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
                    contentGayNghien = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongGayNghien.Body, data);
                }
            }
            else
            {
                if (query.Any())
                {
                    var objData = GetHTMLLinhBu(query, false);

                    var data = new PhieuLinhThuongDuocPhamData
                    {
                        HeaderPhieuLinhThuoc = hearder,
                        MaVachPhieuLinh = "Số: " + getPhieuLinh.SoPhieu,
                        ThuocHoacVatTu = objData.html,
                        LogoUrl = phieuLinhThuongDuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(getPhieuLinh.SoPhieu),
                        TuNgay = getPhieuLinh.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDate(),
                        DenNgay = getPhieuLinh.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDate(),
                        DienGiai = getPhieuLinh.GhiChu,
                        NoiGiao = getPhieuLinh.KhoXuat.Ten,
                        NhapVeKho = getPhieuLinh.KhoNhap.Ten,
                        CongKhoan = "Cộng khoản: " + (objData.Index - 1).ToString() + " khoản",
                        Ngay = DateTime.Now.Day.ConvertDateToString(),
                        Thang = DateTime.Now.Month.ConvertMonthToString(),
                        Nam = DateTime.Now.Year.ConvertYearToString()
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
                }
                if (queryGayNghien.Any())
                {
                    var objData = GetHTMLLinhBu(queryGayNghien, true);

                    var data = new PhieuLinhThuongDuocPhamData
                    {
                        HeaderPhieuLinhThuoc = hearder,
                        MaVachPhieuLinh = "Số: " + getPhieuLinh.SoPhieu,
                        ThuocHoacVatTu = objData.html,
                        LogoUrl = phieuLinhThuongDuoc.HostingName + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(getPhieuLinh.SoPhieu),
                        TuNgay = getPhieuLinh.ThoiDiemLinhTongHopTuNgay?.ApplyFormatDate(),
                        DenNgay = getPhieuLinh.ThoiDiemLinhTongHopDenNgay?.ApplyFormatDate(),
                        DienGiai = getPhieuLinh.GhiChu,
                        NoiGiao = getPhieuLinh.KhoXuat.Ten,
                        NhapVeKho = getPhieuLinh.KhoNhap.Ten,
                        CongKhoan = (objData.Index - 1).ToString() + " khoản",
                        Ngay = DateTime.Now.Day.ConvertDateToString(),
                        Thang = DateTime.Now.Month.ConvertMonthToString(),
                        Nam = DateTime.Now.Year.ConvertYearToString()
                    };
                    contentGayNghien = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongGayNghien.Body, data);
                }
            }


            if (!string.IsNullOrEmpty(content))
            {
                content = headerTitile + content;
            }
            var congPage = string.Empty;
            congPage = !string.IsNullOrEmpty(content) ? "<div style='break-after:page'></div>" : "";
            if (!string.IsNullOrEmpty(contentGayNghien))
            {
                content = content + headerTitile + congPage + contentGayNghien;
            }
            return content;
        }

        public async Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long phieuLinhId)
        {
            var yeuCauLinhDuocPham = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).FirstAsync();
            var trangThaiVo = new TrangThaiDuyetVo();
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

        #region CheckValidator

        public async Task<bool> CheckSoLuongTonDuocPham(long duocPhamBenhVienId, bool laDuocPhamBHYT, long khoXuatId, double? soLuongYeuCau, bool? duocDuyet, double? soLuongCoTheXuat, bool? isValidator)
        {
            var soLuongTonDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId && o.LaDuocPhamBHYT == laDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            soLuongTonDuocPham = soLuongTonDuocPham.MathRoundNumber(2);
            if (isValidator == false && soLuongTonDuocPham >= soLuongYeuCau)
            {
                return true;
            }
            if (isValidator == false && soLuongTonDuocPham < soLuongYeuCau)
            {
                return false;
            }
            if (duocDuyet == null) // đang chờ duyệt
            {
                if (soLuongYeuCau == null)
                {
                    return true;
                }
                if (soLuongYeuCau != null && soLuongTonDuocPham >= soLuongYeuCau)
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

        public async Task<bool> CheckSoLuongTonDuocPhamLinhBu(long? duocPhamBenhVienId, bool? laDuocPhamBHYT, long? khoXuatId, double? soLuongYeuCau)
        {
            if (soLuongYeuCau == null)
            {
                return true;
            }
            var soLuongTonDuocPham = 0.0;
            soLuongTonDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId && o.LaDuocPhamBHYT == laDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            soLuongTonDuocPham = soLuongTonDuocPham.MathRoundNumber(2);
            if (soLuongTonDuocPham >= soLuongYeuCau)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckSoLuongTonDuocPhamDuocPhamGridVo(long? duocPhamBenhVienId, double? soLuongYeuCau, long khoXuatId, bool laDuocPhamBHYT)
        {
            if (soLuongYeuCau == null)
            {
                return true;
            }

            var soLuongTonDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId && o.LaDuocPhamBHYT == laDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            soLuongTonDuocPham = soLuongTonDuocPham.MathRoundNumber(2);
            if (soLuongYeuCau != null && duocPhamBenhVienId != null && soLuongTonDuocPham >= soLuongYeuCau)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckDuocPhamExists(long? duocPhamBenhVienId, bool? laDuocPhamBHYT, List<DuocPhamGridViewModelValidator> duocPhamBenhVienIds)
        {
            if (duocPhamBenhVienId == null)
            {
                return true;
            }
            if (duocPhamBenhVienIds != null)
            {
                foreach (var item in duocPhamBenhVienIds)
                {
                    if (duocPhamBenhVienIds.Any(p => p.DuocPhamBenhVienId == duocPhamBenhVienId && p.LaDuocPhamBHYT == laDuocPhamBHYT))
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

        public async Task CheckPhieuLinhDaDuyetHoacDaHuy(long phieuLinhId)
        {
            var result = await BaseRepository.TableNoTracking.Where(p => p.Id == phieuLinhId).Select(p => p).FirstOrDefaultAsync();
            var resourceName = string.Empty;
            if (result == null)
            {
                resourceName = "YeuCauLinhDuocPham.LinhThuong.NotExists";
            }
            else
            {
                if (result.DuocDuyet == true)
                {
                    resourceName = "YeuCauLinhDuocPham.LinhThuong.DaDuyet";
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

        public async Task<bool> CheckKhoNhanVienQuanLy(long khoNhapId)
        {
            var result = await _khoNhanVienQuanLyRepository.TableNoTracking.Where(p => p.KhoId == khoNhapId).FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }
            return true;
        }
        #endregion

        public double GetSoLuongTonDuocPhamGridVo(long duocPhamBenhVienId, long khoXuatId, bool laDuocPhamBHYT)
        {
            return _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.DuocPhamBenhVienId == duocPhamBenhVienId && o.LaDuocPhamBHYT == laDuocPhamBHYT && o.NhapKhoDuocPhams.KhoId == khoXuatId).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat).MathRoundNumber();
        }
        public string InPhieuLinhBuDuocPhamXemTruoc(XemPhieuLinhBuDuocPham xemPhieuLinhBuDuocPham)
        {
            var content = string.Empty;
            var contentGNHT = string.Empty;
            var hearder = string.Empty;
            var headerTitile = "<div style=\'width: 100%; height: 40px; background: #005dab;color:#fff;text-align: center;font-size: 23px\'> PHIẾU LĨNH DƯỢC PHẨM</div>";
            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuLinhThuongThuocDuocPham")).First();
            var resultGNHT = _templateRepo.TableNoTracking
          .FirstOrDefault(x => x.Name.Equals("PhieuLinhThuocGayNghien"));
            int? nhomVatTu = 0;
            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }


            var groupThuocBHYT = "Thuốc BHYT";
            var groupThuocKhongBHYT = "Thuốc Không BHYT";

            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                        + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";


            var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == xemPhieuLinhBuDuocPham.KhoLinhId
                       && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            #region thuốc dược phẩm bình thường
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                   .Where(x => x.KhoLinhId == xemPhieuLinhBuDuocPham.KhoLinhBuId // kho linh id

                               && x.YeuCauLinhDuocPhamId == null &&
                               (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                               && x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                               && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                               && x.KhongLinhBu != true
                               && x.SoLuong > 0
                               && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                               && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan)
                               && xemPhieuLinhBuDuocPham.YeuCauDuocPhamBenhVienIds.Contains(x.Id))
                   .Select(s => new ThongTinLinhDuocPhamChiTiet()
                   {
                       DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                       Ma = s.DuocPhamBenhVien.Ma,
                       TenThuocHoacVatTu = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == nhomVatTu ?
                                                                               s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NhaSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                                 (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NuocSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                               s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.HamLuong) ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                       DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                       SLYeuCau = s.SoLuong,
                       LaDuocPhamBHYT = s.LaDuocPhamBHYT
                   }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                               .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.DVT })
                               .Select(item => new ThongTinLinhDuocPhamChiTiet()
                               {
                                   DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                                   Ma = item.First().Ma,
                                   TenThuocHoacVatTu = item.First().TenThuocHoacVatTu,
                                   DVT = item.First().DVT,
                                   SLYeuCau = item.Sum(x => x.SLYeuCau).MathRoundNumber(2),
                                   LaDuocPhamBHYT = item.First().LaDuocPhamBHYT
                               }).ToList();


            var result = query.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaDuocPhamBHYT));
            #endregion
            #region thuốc duocj phầm Gây nghiện hướng thần
            var queryGNHT = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
               .Where(x => x.KhoLinhId == xemPhieuLinhBuDuocPham.KhoLinhBuId // kho linh id

                           && x.YeuCauLinhDuocPhamId == null &&
                           (!x.YeuCauLinhDuocPhamChiTiets.Any() || x.YeuCauLinhDuocPhamChiTiets.All(p => p.YeuCauLinhDuocPham.DuocDuyet != null))
                           && x.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien
                           && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                           && x.KhongLinhBu != true
                           && x.SoLuong > 0
                           && (x.SoLuongDaLinhBu == null || x.SoLuongDaLinhBu < x.SoLuong)
                           && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                           && xemPhieuLinhBuDuocPham.YeuCauDuocPhamBenhVienIds.Contains(x.Id))
               .Select(s => new ThongTinLinhDuocPhamChiTiet()
               {
                   DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                   Ma = s.DuocPhamBenhVien.Ma,
                   TenThuocHoacVatTu = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == nhomVatTu ?
                                                                           s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NhaSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                             (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.NuocSanXuat) ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                           s.DuocPhamBenhVien.DuocPham.Ten + (!string.IsNullOrEmpty(s.DuocPhamBenhVien.DuocPham.HamLuong) ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                   DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                   SLYeuCau = s.SoLuong,
                   LaDuocPhamBHYT = s.LaDuocPhamBHYT
               }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                           .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Ma, x.DVT })
                           .Select(item => new ThongTinLinhDuocPhamChiTiet()
                           {
                               DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                               Ma = item.First().Ma,
                               TenThuocHoacVatTu = item.First().TenThuocHoacVatTu,
                               DVT = item.First().DVT,
                               SLYeuCau = item.Sum(x => x.SLYeuCau).MathRoundNumber(2),
                               LaDuocPhamBHYT = item.First().LaDuocPhamBHYT
                           }).ToList();



            var resultGayNghienHT = queryGNHT.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.DuocPhamBenhVienId && o.LaDuocPhamBHYT == p.LaDuocPhamBHYT));
            #endregion

            if (result.Any())
            {
                var objData = GetHTMLLinhBu(result.ToList(), false);

                var khoXuat = _khoRepository.TableNoTracking.Where(d => d.Id == xemPhieuLinhBuDuocPham.KhoLinhBuId).Select(d => d.Ten).FirstOrDefault();
                var khoLinh = _khoRepository.TableNoTracking.Where(d => d.Id == xemPhieuLinhBuDuocPham.KhoLinhId).Select(d => d.Ten).FirstOrDefault();
                var data = new PhieuLinhThuongDuocPhamData
                {
                    HeaderPhieuLinhThuoc = hearder,
                    MaVachPhieuLinh = "Số: " + "",
                    ThuocHoacVatTu = objData.html,
                    LogoUrl = xemPhieuLinhBuDuocPham.HostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = "",
                    TuNgay = xemPhieuLinhBuDuocPham.ThoiDiemLinhTongHopTuNgay.ApplyFormatDateTimeSACH(),
                    DenNgay = xemPhieuLinhBuDuocPham.ThoiDiemLinhTongHopDenNgay.ApplyFormatDateTimeSACH(),
                    DienGiai = "",
                    NoiGiao = khoLinh,
                    NhapVeKho = khoXuat,
                    CongKhoan = (objData.Index - 1).ToString() + " khoản",
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
            }
            if (resultGayNghienHT.Any())
            {
                var objData = GetHTMLLinhBu(resultGayNghienHT.ToList(), true);

                var khoXuat = _khoRepository.TableNoTracking.Where(d => d.Id == xemPhieuLinhBuDuocPham.KhoLinhBuId).Select(d => d.Ten).FirstOrDefault();
                var khoLinh = _khoRepository.TableNoTracking.Where(d => d.Id == xemPhieuLinhBuDuocPham.KhoLinhId).Select(d => d.Ten).FirstOrDefault();
                var data = new PhieuLinhThuongDuocPhamData
                {
                    HeaderPhieuLinhThuoc = hearder,
                    MaVachPhieuLinh = "Số: " + "",
                    ThuocHoacVatTu = objData.html,
                    LogoUrl = xemPhieuLinhBuDuocPham.HostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = "",
                    TuNgay = xemPhieuLinhBuDuocPham.ThoiDiemLinhTongHopTuNgay.ApplyFormatDateTimeSACH(),
                    DenNgay = xemPhieuLinhBuDuocPham.ThoiDiemLinhTongHopDenNgay.ApplyFormatDateTimeSACH(),
                    DienGiai = "",
                    NoiGiao = khoLinh,
                    NhapVeKho = khoXuat,
                    CongKhoan = (objData.Index - 1).ToString() + " khoản",
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };
                contentGNHT = TemplateHelpper.FormatTemplateWithContentTemplate(resultGNHT.Body, data);
            }
            if (!string.IsNullOrEmpty(content))
            {
                content = headerTitile + content;
            }
            if (contentGNHT != "")
            {

                content = content + headerTitile + "<div style='break-after:page'></div>" + contentGNHT;
            }
            return content;
        }
        private OBJList GetHTMLLinhBu(List<ThongTinLinhDuocPhamChiTiet> gridVos, bool loaiThuoc)
        {
            int STT = 1;
            var infoLinhDuocChiTiet = string.Empty;
            foreach (var item in gridVos)
            {
                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                         + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                         + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                         + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                         + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuocHoacVatTu
                                         + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                         + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLYeuCau), false) : item.SLYeuCau.MathRoundNumber(2) + "")
                                         + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"/* (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucPhat), false) : item.SLThucPhat.MathRoundNumber(2) + "")*/
                                         + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.GhiChu
                                         + "</tr>";
                STT++;
            }
            var data = new OBJList
            {
                html = infoLinhDuocChiTiet,
                Index = STT
            };
            return data;
        }

        public List<LookupItemTextVo> GetAllMayXetNghiemYeuCauLinh(DropDownListRequestModel queryInfo, DuocPhamBenhVienMayXetNghiemJson duocPhamBenhVienMayXetNghiemJson)
        {
            var getAllMayXetNghiems = _duocPhamBenhVienMayXetNghiemRepository.TableNoTracking
                .Where(z => z.DuocPhamBenhVienId == duocPhamBenhVienMayXetNghiemJson.DuocPhamBenhVienId && z.MayXetNghiem.HieuLuc)
                .Select(item => new LookupItemTextVo
                {
                    KeyId = item.MayXetNghiem.Id + "-" + item.MayXetNghiem.Ten,
                    DisplayName = item.MayXetNghiem.Ten
                }).ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take);

            return getAllMayXetNghiems.ToList();
        }

        public string GetTrangThaiPhieuLinh(string danhSachMayXetNghiemIdStrs)
        {
            var danhSachTenMayXetNghiems = string.Empty;
            if (!string.IsNullOrEmpty(danhSachMayXetNghiemIdStrs))
            {
                var danhSachMayXetNghiemIds = danhSachMayXetNghiemIdStrs.Split(";").Select(c => long.Parse(c)).ToList();
                if (danhSachMayXetNghiemIds != null &&  danhSachMayXetNghiemIds.Count > 0)
                {
                    var lstTenMayXNEntity = _mayXetNghiemRepository.TableNoTracking
                        .Where(c => c.HieuLuc && danhSachMayXetNghiemIds.Contains(c.Id)).Select(x => x.Ten).ToList();

                    return string.Join(';', lstTenMayXNEntity);
                }
            }
            return danhSachTenMayXetNghiems;
        }
    }
}
