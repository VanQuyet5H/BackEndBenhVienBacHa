using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Helpers;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        public async Task<bool> IsExistDoubleKetLuanVaTheoDoi(long yeuCauTiepNhanId, long phongBenhVienId)
        {
            //var isExistTheoDoiSauPhauThuatThuThuat = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
            //                                                                                                          p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            //                                                                                                          p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
            //                                                                                                          p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat != null && //Cập nhật cho theo dõi trước (khi tồn tại 2 theo dõi)
            //                                                                                                          p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
            //                                                                                              .Select(p => p.YeuCauDichVuKyThuat)
            //                                                                                              .AnyAsync();

            //var isNotExistTheoDoiSauPhauThuatThuThuat = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
            //                                                                                                             p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            //                                                                                                             p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
            //                                                                                                             p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat == null &&
            //                                                                                                             p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
            //                                                                                                 .Select(p => p.YeuCauDichVuKyThuat)
            //                                                                                                 .AnyAsync();

            var isExistTheoDoiSauPhauThuatThuThuat = _phongBenhVienHangDoiRepository.TableNoTracking.Any(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                                      p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                                      //p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                                      p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                                      p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat != null && //Cập nhật cho theo dõi trước (khi tồn tại 2 theo dõi)
                                                                                                                      p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true);

            var isNotExistTheoDoiSauPhauThuatThuThuat = _phongBenhVienHangDoiRepository.TableNoTracking.Any(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                                         p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                                         //p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                                         p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                                         p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat == null &&
                                                                                                                         p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true);

            return isExistTheoDoiSauPhauThuatThuThuat && isNotExistTheoDoiSauPhauThuatThuThuat;
        }

        public async Task<bool> KiemTraTatCaYeuCauDichVuKyThuatPTTT(long yeuCauTiepNhanId, long phongBenhVienId)
        {
            //var isExistTheoDoiSauPhauThuatThuThuat = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
            //                                                                                                          p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            //                                                                                                          p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
            //                                                                                                          p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat != null && //Cập nhật cho theo dõi trước (khi tồn tại 2 theo dõi)
            //                                                                                                          p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
            //                                                                                              .Select(p => p.YeuCauDichVuKyThuat)
            //                                                                                              .AnyAsync();

            //var isNotExistTheoDoiSauPhauThuatThuThuat = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
            //                                                                                                             p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            //                                                                                                             p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
            //                                                                                                             p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat == null &&
            //                                                                                                             p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
            //                                                                                                 .Select(p => p.YeuCauDichVuKyThuat)
            //                                                                                                 .AnyAsync();

            if (await IsExistDoubleKetLuanVaTheoDoi(yeuCauTiepNhanId, phongBenhVienId))
            {
                //Theo dõi 2 lần khác nhau -> cho cập nhật
                return true;
            }

            var totalYeuCauDichVuKyThuatPTTT = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongBenhVienId &&
                            p.YeuCauDichVuKyThuat != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            (p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauTiepNhan.Id == yeuCauTiepNhanId &&
                            !(p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true))
                .CountAsync();

            var totalYeuCauDichVuKyThuatPTTTDaHoanTat = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongBenhVienId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.YeuCauDichVuKyThuat != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauTiepNhan.Id == yeuCauTiepNhanId &&
                            !(p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true))
                .CountAsync();

            return totalYeuCauDichVuKyThuatPTTT > totalYeuCauDichVuKyThuatPTTTDaHoanTat ? false : true;
        }

        public async Task ChuyenGiaoSauPhauThuatThuThuat(long yeuCauTiepNhanId, long theoDoiSauPhauThuatThuThuatId, long? nhanVienKetLuanId, long phongBenhVienId, bool IsChuyenGiaoTuTuongTrinh, DateTime? thoiDiemKetThucTheoDoi)
        {
            var lstPhongBenhVienHangDoi = new List<PhongBenhVienHangDoi>();

            if (await IsExistDoubleKetLuanVaTheoDoi(yeuCauTiepNhanId, phongBenhVienId))
            {
                if(IsChuyenGiaoTuTuongTrinh)
                {
                    //Cập nhật thằng đang tường trình khi tất cả là thủ thuật
                    lstPhongBenhVienHangDoi = await _phongBenhVienHangDoiRepository.TableNoTracking
                    .Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                p.YeuCauDichVuKyThuat != null &&
                                p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                                p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat == null &&
                                p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    .ToListAsync();
                }
                else
                {
                    //Theo dõi 2 lần khác nhau -> cập nhật thằng đã tồn tại trước
                    lstPhongBenhVienHangDoi = await _phongBenhVienHangDoiRepository.TableNoTracking
                    .Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                p.YeuCauDichVuKyThuat != null &&
                                p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                                p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat != null && //Chuyển giao cho theo dõi trước (khi tồn tại 2 theo dõi)
                                p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    .ToListAsync();
                }
            }
            else
            {
                //Chỉ theo dõi 1 lần (không chỉ định thêm lúc theo dõi)
                lstPhongBenhVienHangDoi = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongBenhVienId &&
                            p.YeuCauDichVuKyThuat != null &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .ToListAsync();
            }

            await CapNhatTheoDoiPhauThuatThuThuatChoYeuCauDichVuKyThuat(yeuCauTiepNhanId, theoDoiSauPhauThuatThuThuatId, EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien, nhanVienKetLuanId, phongBenhVienId, IsChuyenGiaoTuTuongTrinh, thoiDiemKetThucTheoDoi);
            await _phongBenhVienHangDoiRepository.DeleteAsync(lstPhongBenhVienHangDoi);
        }

        public async Task CapNhatTheoDoiPhauThuatThuThuatChoYeuCauDichVuKyThuat(long yeuCauTiepNhanId, long theoDoiSauPhauThuatThuThuatId, EnumTrangThaiYeuCauDichVuKyThuat? enumTrangThaiYeuCauDichVuKyThuat, long? nhanVienKetLuanId, long phongBenhVienId, bool IsChuyenGiaoTuTuongTrinh, DateTime? thoiDiemKetThucTheoDoi)
        {
            var lstYeuCauDichVuKyThuat = new List<YeuCauDichVuKyThuat>();

            if (await IsExistDoubleKetLuanVaTheoDoi(yeuCauTiepNhanId, phongBenhVienId))
            {
                if(IsChuyenGiaoTuTuongTrinh)
                {
                    //Cập nhật thằng đang tường trình khi tất cả là thủ thuật
                    lstYeuCauDichVuKyThuat = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                              p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                              p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                              p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat == null &&
                                                                                                              p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
                                                                                                  .Select(p => p.YeuCauDichVuKyThuat)
                                                                                                  .ToListAsync();
                }
                else
                {
                    //Theo dõi 2 lần khác nhau -> cập nhật thằng đã tồn tại trước
                    lstYeuCauDichVuKyThuat = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                              p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                              p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                              p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuat != null && //Cập nhật cho theo dõi trước (khi tồn tại 2 theo dõi)
                                                                                                              p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
                                                                                                  .Select(p => p.YeuCauDichVuKyThuat)
                                                                                                  .ToListAsync();
                }
            }
            else
            {
                //Chỉ theo dõi 1 lần (không chỉ định thêm lúc theo dõi)
                lstYeuCauDichVuKyThuat = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                          p.YeuCauDichVuKyThuat.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                          p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                          p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
                                                                                              .Select(p => p.YeuCauDichVuKyThuat)
                                                                                              .Include(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
                                                                                              .ToListAsync();
            }

            var currentUser = _userAgentHelper.GetCurrentUserId();

            foreach (var item in lstYeuCauDichVuKyThuat)
            {
                item.TrangThai = enumTrangThaiYeuCauDichVuKyThuat ?? item.TrangThai;
                item.TheoDoiSauPhauThuatThuThuatId = theoDoiSauPhauThuatThuThuatId;

                if (enumTrangThaiYeuCauDichVuKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                {
                    item.ThoiDiemHoanThanh = thoiDiemKetThucTheoDoi == null ? DateTime.Now : thoiDiemKetThucTheoDoi;
                    item.NhanVienKetLuanId = nhanVienKetLuanId;
                    //Update LanThucHien 09/16
                    //item.LanThucHien = lanThucHienHienTai;

                    //Update NhanVienThucHien & ThoiDiemThucHien
                    if (item.ThoiDiemThucHien == null)
                    {
                        item.ThoiDiemThucHien = item.YeuCauDichVuKyThuatTuongTrinhPTTT?.ThoiDiemPhauThuat ?? DateTime.Now;
                    }

                    if (item.NhanVienThucHienId == null)
                    {
                        item.NhanVienThucHienId = currentUser;
                    }
                }

                await BaseRepository.UpdateAsync(item);
            }
        }

        public async Task<bool> CheckHasPhauThuat(long noiThucHienId, long yctnId, bool isExistTheoDoi)
        {
            //var dvKyThuats = BaseRepository.TableNoTracking.Where(p => p.NoiThucHienId == noiThucHienId &&
            //                                                                 (p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ||
            //                                                                 p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
            //                                                                 p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
            //                                                                 p.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
            //                                                                 p.YeuCauTiepNhanId == yctnId &&
            //                                                                 (isExistTheoDoi == false || p.TheoDoiSauPhauThuatThuThuatId == null))
            //                                                     .Select(p => p.DichVuKyThuatBenhVien)
            //                                                     .ToList();

            //return dvKyThuats.Any(p => IsPhauThuat(p.Id));

            //Cập nhật 12/12/2022
            var lstNhomPhauThuat = await GetListNhomBenhVienTheoNhomChaId(0, true);
            var lstNhomPhauThuatId = GetListNhomBenhVienIdTheoNhomChaId(0, lstNhomPhauThuat);

            var nhomDichVuId = BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yctnId
                                                                        && p.NoiThucHienId == noiThucHienId
                                                                        && (p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                                                        && p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                        && p.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true
                                                                        && (isExistTheoDoi == false || p.TheoDoiSauPhauThuatThuThuatId == null))
                                                               .Select(x => x.NhomDichVuBenhVienId)
                                                               .Distinct()
                                                               .ToList();
            return nhomDichVuId.Any(x => lstNhomPhauThuatId.Contains(x));
        }

        #region Cập nhật 12/12/2022
        private async Task<List<NhomDichVuBenhVienTreeViewVo>> GetListNhomBenhVienTheoNhomChaId(long nhomChaId, bool laPhauThuat = false)
        {

            var query = _nhomDichVuBenhVienRepository.TableNoTracking
                .Select(item => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ma + " - " + item.Ten,
                    ParentId = item.NhomDichVuBenhVienChaId,
                    Ma = item.Ma
                })
                .ToList();


            if (nhomChaId == 0)
            {
                var nhomCha = query.FirstOrDefault(x => x.Ma.Trim().ToLower() == "pt");
                if(nhomCha == null)
                {
                    return new List<NhomDichVuBenhVienTreeViewVo>();
                }

                nhomChaId = nhomCha.KeyId;
            }

            var lstNhomBenhVien = query.Select(item => new NhomDichVuBenhVienTreeViewVo
            {
                KeyId = item.KeyId,
                DisplayName = item.DisplayName,
                ParentId = item.ParentId,
                Items = GetChildrenTree(query, item.KeyId, null, item.DisplayName.RemoveVietnameseDiacritics())
            })
                .Where(x => x.KeyId == nhomChaId).ToList();
            return lstNhomBenhVien;
        }

        private List<long> GetListNhomBenhVienIdTheoNhomChaId(long nhomChaId, List<NhomDichVuBenhVienTreeViewVo> lstNhom)
        {
            List<long> lstNhomId = new List<long>();
            foreach (var t in lstNhom)
            {
                lstNhomId.Add(t.KeyId);
                lstNhomId = lstNhomId.Union(GetListNhomBenhVienIdTheoNhomChaId(t.KeyId, t.Items)).ToList();
            }

            return lstNhomId;
        }
        #endregion

        public bool IsPhauThuat(long dichVuKyThuatBenhVienId)
        {
            //Update kiểm tra loại PTTT
            var nhomDichVuBenhVien = GetNhomDichVuBenhVienPTTTLevel2(dichVuKyThuatBenhVienId);

            if (nhomDichVuBenhVien == null)
            {
                return false;
            }

            return nhomDichVuBenhVien.Ma.Substring(0, 2).ToLower().Equals("pt");

            //return !string.IsNullOrEmpty(loaiPhauThuatThuThuat) && loaiPhauThuatThuThuat.Substring(0, 1).ToLower().Contains("p");
        }

        private Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien GetNhomDichVuBenhVienPTTTLevel2(long dichVuKyThuatBenhVienId)
        {
            var currentNhomDichVuBenhVien = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Id == dichVuKyThuatBenhVienId)
                                                                                            .Select(p => p.NhomDichVuBenhVien)
                                                                                            .Include(p => p.NhomDichVuBenhVienCha)
                                                                                            .FirstOrDefault();

            while (currentNhomDichVuBenhVien != null && currentNhomDichVuBenhVien.NhomDichVuBenhVienChaId != null && currentNhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma != "PTTT")
            {
                currentNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.Id == currentNhomDichVuBenhVien.NhomDichVuBenhVienChaId)
                                                                                         .Include(p => p.NhomDichVuBenhVienCha)
                                                                                         .FirstOrDefault();
            }

            return currentNhomDichVuBenhVien;
        }

        public async Task KhongTuongTrinh(long yeuCauTiepNhanId, long yeuCauDichVuKyThuatId, long phongBenhVienId)
        {
            //Update YCDVKT
            var yeuCauDichVuKyThuat = _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                       p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId &&
                                                                                                       p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                       p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT == null &&
                                                                                                       p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
                                                                                           .Select(p => p.YeuCauDichVuKyThuat)
                                                                                           .Include(p => p.DichVuKyThuatBenhVien)
                                                                                           .FirstOrDefault();

            if(yeuCauDichVuKyThuat == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            if (yeuCauDichVuKyThuat.DichVuKyThuatBenhVien.DichVuKhongKetQua != true)
            {
                throw new Exception(_localizationService.GetResource("PhauThuatThuThuat.KhongTuongTrinh.DichVuKhongKetQua.Required"));
            }

            var currentUser = _userAgentHelper.GetCurrentUserId();
            var currentDateTime = DateTime.Now;

            yeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            yeuCauDichVuKyThuat.ThoiDiemHoanThanh = currentDateTime;
            yeuCauDichVuKyThuat.NhanVienKetLuanId = currentUser;
            yeuCauDichVuKyThuat.ThoiDiemThucHien = currentDateTime;
            yeuCauDichVuKyThuat.NhanVienThucHienId = currentUser;

            await BaseRepository.UpdateAsync(yeuCauDichVuKyThuat);

            //Xoá hàng đợi
            var phongBenhVienHangDoi = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                        p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId &&
                                                                                                        p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                        p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                        p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
                                                                                            .FirstOrDefaultAsync();

            await _phongBenhVienHangDoiRepository.DeleteAsync(phongBenhVienHangDoi);
        }

        public async Task HoanThanhTuongTrinhLai(long phongBenhVienId, long yeuCauTiepNhanId)
        {
            var lstPhongBenhVienHangDoi = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                           p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                           p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                                                                                                           p.YeuCauDichVuKyThuat != null &&
                                                                                                           p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien &&
                                                                                                           p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                                                                               .ToListAsync();

            await _phongBenhVienHangDoiRepository.DeleteAsync(lstPhongBenhVienHangDoi);
        }
    }
}