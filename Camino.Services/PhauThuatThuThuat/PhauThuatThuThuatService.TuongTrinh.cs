using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        public async Task<ThongTinKhoaPhongVo> GetThongTinKhoa(long phongBenhVienId, long? ycdvktId)
        {
            if (phongBenhVienId == 0)
            {
                return null;
            }

            var tenKpQuery = _phongBenhVienRepository.TableNoTracking
                .Where(w => w.Id == phongBenhVienId)
                .Select(p => p.KhoaPhong.Ten);

            var tenKp = tenKpQuery.FirstOrDefaultAsync();

            if (ycdvktId == -1 || ycdvktId == null)
            {
                await tenKp;
                return new ThongTinKhoaPhongVo
                {
                    KhoaPhong = tenKp.Result
                };
            }

            var isNoiTru = await LaNoiTru(ycdvktId.GetValueOrDefault());

            var isPhauThuat = await LaPhauThuat(ycdvktId.GetValueOrDefault());

            var icdKpQuery = BaseRepository.TableNoTracking
                .Where(w => w.Id == ycdvktId)
                .Select(p => new ThongTinKhoaPhongVo
                {
                    //ChanDoanKhoa = isNoiTru ? p.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet : p.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                    ChanDoanKhoa = isNoiTru ? p.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu : p.YeuCauKhamBenh.ChanDoanSoBoGhiChu,
                    GhiChuChanDoanKhoa = p.YeuCauKhamBenh.GhiChuICDChinh,
                    //ICDTruocId = isNoiTru ? p.NoiTruPhieuDieuTri.ChanDoanChinhICDId.GetValueOrDefault() : p.YeuCauKhamBenh.IcdchinhId.GetValueOrDefault(),
                    ICDTruocId = p.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuatId ??
                    (
                        isNoiTru ?
                            (p.NoiTruPhieuDieuTri != null ? p.NoiTruPhieuDieuTri.ChanDoanChinhICDId : null) :
                            (p.YeuCauKhamBenh != null ? p.YeuCauKhamBenh.ChanDoanSoBoICDId : null)
                    ),
                    //ICDTruoc = isNoiTru ? p.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet : p.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                    ICDTruoc = p.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuatId != null ? $"{p.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.Ma} - {p.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.TenTiengViet}" :
                    (
                        isNoiTru ?
                            (p.NoiTruPhieuDieuTri != null && p.NoiTruPhieuDieuTri.ChanDoanChinhICD != null ? $"{p.NoiTruPhieuDieuTri.ChanDoanChinhICD.Ma} - {p.NoiTruPhieuDieuTri.ChanDoanChinhICD.TenTiengViet}" : string.Empty) :
                            (p.YeuCauKhamBenh != null && p.YeuCauKhamBenh.ChanDoanSoBoICDId != null ? $"{p.YeuCauKhamBenh.ChanDoanSoBoICD.Ma} - {p.YeuCauKhamBenh.ChanDoanSoBoICD.TenTiengViet}" : string.Empty)
                    ),
                    ThoiGianPt = p.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat,
                    ThoiGianBatDauGayMe = p.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiGianBatDauGayMe,
                    ThoiGianKetThucPt = p.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat,
                    MoTaCDTruocPhauThuat = p.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDTruocPhauThuat,
                    ICDSauId = p.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuatId,
                    ICDSau = p.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat != null ? $"{p.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.Ma} - {p.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.TenTiengViet}" : string.Empty,
                    MoTaCDSauPhauThuat = p.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDSauPhauThuat,
                    MaPttt = p.YeuCauDichVuKyThuatTuongTrinhPTTT.MaPhuongPhapPTTT,
                    PhuongPhapPttt = p.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT,
                    //LoaiPttt = p.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat,
                    LoaiPhauThuatThuThuat = p.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat, //Update loại PTTT
                    PpVoCamId = p.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCamId,
                    PpVoCam = p.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam.Ten,
                    TinhHinhPttt = p.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT,
                    TaiBienPttt = p.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT,
                    TrinhTuPttt = p.YeuCauDichVuKyThuatTuongTrinhPTTT.TrinhTuPhauThuat,
                    NhanVienThucHienId = p.NhanVienThucHienId,
                    NhanVienThucHienDisplay = p.NhanVienThucHien.User.HoTen ?? string.Empty,

                    //BVHD-3877
                    GhiChuCaPTTT = p.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuCaPTTT
                });

            var luocDoPtsQuery = _ycDvKtLdPtRepository.TableNoTracking
                .Where(q => q.YeuCauDichVuKyThuatTuongTrinhPTTTId == ycdvktId)
                .Select(e => new LuocDoPhauThuatTaiLieuDinhKemVo
                {
                    Id = e.Id,
                    IdYeuCauDichVuKyThuat = ycdvktId,
                    LuocDoPhauThuat = e.LuocDo,
                    MoTa = e.MoTa
                });

            var icdKp = icdKpQuery.FirstOrDefaultAsync();
            var lds = luocDoPtsQuery.ToListAsync();
            await Task.WhenAll(tenKp, icdKp, lds);

            var maPttt = icdKp.Result.MaPttt;

            string[] lstMaPttt = maPttt?.Split('|');

            return new ThongTinKhoaPhongVo
            {
                KhoaPhong = tenKp.Result,
                ChanDoanKhoa = icdKp.Result.ChanDoanKhoa,
                GhiChuChanDoanKhoa = icdKp.Result.GhiChuChanDoanKhoa,
                ICDTruocId = icdKp.Result.ICDTruocId,
                ICDTruoc = icdKp.Result.ICDTruoc,
                ThoiGianPt = icdKp.Result.ThoiGianPt,
                ThoiGianBatDauGayMe = icdKp.Result.ThoiGianBatDauGayMe,
                ThoiGianKetThucPt = icdKp.Result.ThoiGianKetThucPt,
                MoTaCDTruocPhauThuat = icdKp.Result.MoTaCDTruocPhauThuat,
                ICDSauId = icdKp.Result.ICDSauId,
                ICDSau = icdKp.Result.ICDSau,
                MoTaCDSauPhauThuat = icdKp.Result.MoTaCDSauPhauThuat,
                MaPttt = maPttt,
                MaPtttFormat = lstMaPttt != null ? lstMaPttt.ToList() : new List<string>(),
                PhuongPhapPttt = icdKp.Result.PhuongPhapPttt,
                //LoaiPttt = icdKp.Result.LoaiPttt,
                LoaiPhauThuatThuThuat = icdKp.Result.LoaiPhauThuatThuThuat, //Update loại PTTT
                PpVoCamId = icdKp.Result.PpVoCamId,
                PpVoCam = icdKp.Result.PpVoCam,
                TinhHinhPttt = icdKp.Result.TinhHinhPttt,
                TaiBienPttt = icdKp.Result.TaiBienPttt,
                TrinhTuPttt = icdKp.Result.TrinhTuPttt,
                LuocDoPhauThuats = lds.Result,
                IsPhauThuat = isPhauThuat,
                NhanVienThucHienId = icdKp.Result.NhanVienThucHienId,
                NhanVienThucHienDisplay = icdKp.Result.NhanVienThucHienDisplay,

                //BVHD-3877
                GhiChuCaPTTT = icdKp.Result.GhiChuCaPTTT
            };
        }

        public async Task<ThongTinChiDinhDichVuVo> GetThongTinChiDinhDichVu(long yeuCauDichVuKyThuatId)
        {
            var yeuCauDichVuKyThuat = await BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauDichVuKyThuatId)
                                                                          .Select(p => new ThongTinChiDinhDichVuVo
                                                                          {
                                                                              NhanVienChiDinhId = p.NhanVienChiDinhId,
                                                                              NhanVienChiDinhDisplay = p.NhanVienChiDinh.User.HoTen,
                                                                              NoiChiDinhId = p.NoiChiDinhId,
                                                                              NoiChiDinhDisplay = p.NoiChiDinh.Ten,
                                                                              IsDichVuKhongCanTuongTrinh = p.DichVuKyThuatBenhVien.DichVuKhongKetQua,

                                                                              //BVHD-3917
                                                                              HinhThucDenId = p.YeuCauTiepNhan.HinhThucDenId,
                                                                              BacSiGioiThieu = p.YeuCauTiepNhan.NoiGioiThieu.Ten
                                                                          })
                                                                          .FirstOrDefaultAsync();
            if (yeuCauDichVuKyThuat != null)
            {
                var thuePhong = await _thuePhongRepository.TableNoTracking
                    .Where(x => x.YeuCauDichVuKyThuatThuePhongId == yeuCauDichVuKyThuatId)
                    .FirstOrDefaultAsync();
                if (thuePhong != null)
                {
                    yeuCauDichVuKyThuat.ThuePhongId = thuePhong.Id;
                    yeuCauDichVuKyThuat.CauHinhThuePhongId = thuePhong.CauHinhThuePhongId;
                    yeuCauDichVuKyThuat.ThoiDiemBatDau = thuePhong.ThoiDiemBatDau;
                    yeuCauDichVuKyThuat.ThoiDiemKetThuc = thuePhong.ThoiDiemKetThuc;
                }

                if (yeuCauDichVuKyThuat.HinhThucDenId != null)
                {
                    var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                    long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

                    yeuCauDichVuKyThuat.LaHinhThucDenGioiThieu = yeuCauDichVuKyThuat.HinhThucDenId == hinhThucDenGioiThieuId;
                }
            }
            return yeuCauDichVuKyThuat;
        }

        private async Task<bool> LaNoiTru(long ycdvktId)
        {
            var ycdvktEntity = await BaseRepository.GetByIdAsync(ycdvktId,
                q => q.Include(w => w.YeuCauTiepNhan).ThenInclude(w => w.NoiTruBenhAn));
            return ycdvktEntity.YeuCauTiepNhan?.NoiTruBenhAn != null;
        }

        public async Task<bool> LaPhauThuat(long ycdvktId)
        {
            var ycdvktEntity = await BaseRepository.GetByIdAsync(ycdvktId);
            return IsPhauThuat(ycdvktEntity.DichVuKyThuatBenhVienId);
        }

        public List<LookupItemVo> GetListThoiGianTuVongPTTTTrongNgay(LookupQueryInfo queryInfo)
        {
            var lstThoiGianTuVong = EnumHelper.GetListEnum<EnumThoiGianTuVongPTTTTheoNgay>()
                .Select(item => new LookupItemVo
                {
                    KeyId = (int)item,
                    DisplayName = item.GetDescription()
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstThoiGianTuVong = lstThoiGianTuVong.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                                     .ToList();
            }

            return lstThoiGianTuVong;
        }

        public List<LookupItemVo> GetListTuVongPTTTTrongNgay(LookupQueryInfo queryInfo)
        {
            var lstTuVong = EnumHelper.GetListEnum<EnumTuVongPTTTTheoNgay>()
                .Select(item => new LookupItemVo
                {
                    KeyId = (int)item,
                    DisplayName = item.GetDescription()
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstTuVong = lstTuVong.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                     .ToList();
            }

            return lstTuVong;
        }

        public async Task<List<LookupTrangThaiPtttVo>> GetListPtttBn(LookupQueryInfo queryInfo, long noiThucHienId, long yctnId, bool IsTuongTrinhLai)
        {
            // rw
            // var bnPtttsQuery = (from phauThuat in BaseRepository.TableNoTracking
            //                     .Where(e => e.NoiThucHienId == noiThucHienId &&
            //                     (e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ||
            //                      e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
            //                     e.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
            //                     e.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
            //                     e.YeuCauTiepNhanId == yctnId)
            // 
            //                     join ekipBacSi in _phauThuatThuThuatEkipBacSiRepository.TableNoTracking on phauThuat.Id equals ekipBacSi.YeuCauDichVuKyThuatTuongTrinhPTTT.Id into ekipBacSiInto
            //                     from ekipBacSi in ekipBacSiInto.DefaultIfEmpty()
            // 
            //                     join nhanVien in _nhanVienRepository.TableNoTracking on ekipBacSi.NhanVienId equals nhanVien.Id into nhanVienInto
            //                     from nhanVien in nhanVienInto.DefaultIfEmpty()
            // 
            //                     select new { phauThuat, nhanVien, ekipBacSi }).Select(w => new LookupTrangThaiPtttVo
            //                     {
            //                         KeyId = w.phauThuat.Id,
            //                         BacSi = w.nhanVien.User != null ? w.nhanVien.User.HoTen : string.Empty,
            //                         TenDv = w.phauThuat.TenDichVu,
            //                         IsPhauThuatVienChinh = w.ekipBacSi.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh,
            //                         IsDaTuongTrinh = w.phauThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null
            //                     }).ApplyLike(queryInfo.Query,
            //         e => e.TenDv);

            #region cập nhật 13/12/2022
            //var users = _userRepository.TableNoTracking.ToDictionary(cc => cc.Id, cc => cc.HoTen);

            var ycktHoanThanhCoHangDoiIds = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.PhongBenhVienId == noiThucHienId
                            && x.YeuCauTiepNhanId == yctnId
                            && x.YeuCauDichVuKyThuatId != null
                            && x.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                            && x.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                .Select(x => x.YeuCauDichVuKyThuatId)
                .ToList();
            #endregion

            var bnPtttsQuery = BaseRepository.TableNoTracking
                .Where(e => e.NoiThucHienId == noiThucHienId &&
                            //Update tường trình lại
                            (
                                #region cập nhật 13/12/2022
                                //IsTuongTrinhLai == true ?
                                //    (e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && e.PhongBenhVienHangDois.Any()) :
                                //    (e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)

                                IsTuongTrinhLai == true ?
                                        ycktHoanThanhCoHangDoiIds.Contains(e.Id) :
                                        (e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                #endregion
                            ) &&
                            e.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            e.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            e.YeuCauTiepNhanId == yctnId)
                .Select(w => new LookupTrangThaiPtttVo
                {
                    KeyId = w.Id,

                    #region cập nhật 13/12/2022
                    //BacSiChinhId = w.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis.Any(s => s.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh) ?
                    //                    w.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis.FirstOrDefault(s => s.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh).NhanVienId :
                    //                    (w.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipDieuDuongs.Any(s => s.VaiTroDieuDuong == EnumVaiTroDieuDuong.PhauThuatVienChinh) ?
                    //                        w.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipDieuDuongs.FirstOrDefault(s => s.VaiTroDieuDuong == EnumVaiTroDieuDuong.PhauThuatVienChinh).NhanVienId :
                    //                        (long?)null),
                    #endregion

                    TenDv = w.TenDichVu,
                    IsDaTuongTrinh = w.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null,
                    LoaiPhauThuatThuThuat = w.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat,
                    DichVuKyThuatBenhVienId = w.DichVuKyThuatBenhVienId,
                    TrangThaiYeuCauDichVuKyThuat = w.TrangThai, //Update tường trình lại

                    NhomDichVuBenhVienKiemTraId = w.NhomDichVuBenhVienId
                }).ApplyLike(queryInfo.Query, e => e.TenDv);

            var ptttBnResult = await bnPtttsQuery.Take(queryInfo.Take)
                .OrderBy(w => w.IsDaTuongTrinh).ThenBy(e => e.KeyId)
                .ToListAsync();

            #region cập nhật 13/12/2022
            if (ptttBnResult.Any())
            {
                var lstYeuCauDichVuKyThuatId = ptttBnResult.Select(x => x.KeyId).ToList();
                var lstEkipBacSi = _phauThuatThuThuatEkipBacSiRepository.TableNoTracking
                        .Where(x => lstYeuCauDichVuKyThuatId.Contains(x.YeuCauDichVuKyThuatTuongTrinhPTTTId) 
                                && x.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh)
                        .Select(x => new
                        {
                            YeuCauDichVuKyThuatId = x.YeuCauDichVuKyThuatTuongTrinhPTTTId,
                            BacSiChinhId = x.NhanVienId,
                            TenBacSi = x.NhanVien.User.HoTen
                        }).ToList();

                var lstEkipDieuDuong = _phauThuatThuThuatEkipDieuDuongRepository.TableNoTracking
                        .Where(x => lstYeuCauDichVuKyThuatId.Contains(x.YeuCauDichVuKyThuatTuongTrinhPTTTId)
                                && x.VaiTroDieuDuong == EnumVaiTroDieuDuong.PhauThuatVienChinh)
                        .Select(x => new
                        {
                            YeuCauDichVuKyThuatId = x.YeuCauDichVuKyThuatTuongTrinhPTTTId,
                            BacSiChinhId = x.NhanVienId,
                            TenBacSi = x.NhanVien.User.HoTen
                        }).ToList();

                var lstNhomPhauThuat = await GetListNhomBenhVienTheoNhomChaId(0, true);
                var lstNhomPhauThuatId = GetListNhomBenhVienIdTheoNhomChaId(0, lstNhomPhauThuat);

                foreach (var pt in ptttBnResult)
                {
                    var ekipBacSi = lstEkipBacSi.FirstOrDefault(x => x.YeuCauDichVuKyThuatId == pt.KeyId);
                    if(ekipBacSi != null)
                    {
                        pt.BacSiChinhId = ekipBacSi.BacSiChinhId;
                        pt.BacSi = ekipBacSi.TenBacSi;
                    }
                    else
                    {
                        var ekipDieuDuong = lstEkipDieuDuong.FirstOrDefault(x => x.YeuCauDichVuKyThuatId == pt.KeyId);
                        if(ekipDieuDuong != null)
                        {
                            pt.BacSiChinhId = ekipDieuDuong.BacSiChinhId;
                            pt.BacSi = ekipDieuDuong.TenBacSi;
                        }
                        else
                        {
                            pt.BacSi = string.Empty;
                        }
                    }

                    pt.LoaiPTTT = lstNhomPhauThuatId.Contains(pt.NhomDichVuBenhVienKiemTraId ?? 0) ? "Phẫu thuật" : "Thủ thuật";
                    //pt.LoaiPTTT = IsPhauThuat(pt.DichVuKyThuatBenhVienId) ? "Phẫu thuật" : "Thủ thuật";
                }
            }
            return ptttBnResult;

            //return ptttBnResult.Select(o =>
            //{
            //    o.BacSi = o.BacSiChinhId != null ? users[o.BacSiChinhId.Value] : string.Empty;
            //    o.LoaiPTTT = IsPhauThuat(o.DichVuKyThuatBenhVienId) ? "Phẫu thuật" : "Thủ thuật";
            //    return o;
            //}).ToList();
            #endregion
        }

        #region Operation
        public async Task<KetQuaPhauThuatThuThuatVo> StartPhauThuat(long idDichVuKyThuat)
        {
            var query = await BaseRepository.GetByIdAsync(idDichVuKyThuat);
            if (query.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
            {
                return new KetQuaPhauThuatThuThuatVo
                {
                    ThanhCong = false,
                    Error = "Lỗi! Phẫu thuật thủ thuật này đã xử lý rồi!"
                };
            }

            query.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien;
            await BaseRepository.UpdateAsync(query);
            return new KetQuaPhauThuatThuThuatVo
            {
                ThanhCong = true,
                Error = "Đã cập nhật trạng thái cho phẫu thuật thủ thuật!"
            };
        }

        public async Task<KetQuaPhauThuatThuThuatVo> FinishOperation(long idDichVuKyThuat)
        {
            var query = await BaseRepository.GetByIdAsync(idDichVuKyThuat);
            if (query.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
            {
                return new KetQuaPhauThuatThuThuatVo
                {
                    ThanhCong = false,
                    Error = "Lỗi! Phẫu thuật thủ thuật này đã xử lý rồi!"
                };
            }

            query.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            await BaseRepository.UpdateAsync(query);
            return new KetQuaPhauThuatThuThuatVo
            {
                ThanhCong = true,
                Error = "Đã cập nhật trạng thái cho phẫu thuật thủ thuật!"
            };
        }
        #endregion

        public bool KiemTraThoiGianVoiThoiDiemTiepNhan(DateTime? thoiGian, long yeuCauDichVuKyThuatId)
        {
            if (thoiGian == null)
            {
                return true;
            }

            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatRepository.GetById(yeuCauDichVuKyThuatId, o => o.Include(p => p.YeuCauTiepNhan).ThenInclude(yctn => yctn.YeuCauNhapVien).ThenInclude(ycnv => ycnv.YeuCauKhamBenh)
            .ThenInclude(yckb => yckb.YeuCauTiepNhan));
            if (yeuCauDichVuKyThuat.YeuCauTiepNhan.YeuCauNhapVien != null && yeuCauDichVuKyThuat.YeuCauTiepNhan.YeuCauNhapVien.YeuCauKhamBenh != null)
            {
                return thoiGian >= yeuCauDichVuKyThuat.YeuCauTiepNhan.YeuCauNhapVien.YeuCauKhamBenh.YeuCauTiepNhan.ThoiDiemTiepNhan;
            }
            return thoiGian >= yeuCauDichVuKyThuat.YeuCauTiepNhan.ThoiDiemTiepNhan;
        }
    }
}
