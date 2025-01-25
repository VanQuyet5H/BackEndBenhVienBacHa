using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject.PhieuInXetNghiem;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        public async Task<List<NhomDichVuBenhVienTreeViewVo>> GetListNhomDichVuCLSPTTT(DropDownListRequestModel model)
        {
            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            //var lstNhomDichVuCLSPTTT = await _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.Ma == "XN" || p.Ma == "CDHA" || p.Ma == "TDCN").Select(p => p.Id).ToListAsync();
            var lstNhomDichVuCLSPTTT = _nhomDichVuBenhVienRepository.TableNoTracking
                .Where(p => nhomTiemChungId == null || p.Id != nhomTiemChungId)
                .Select(p => p.Id).ToList();

            var lstNhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking
                .Where(p => nhomTiemChungId == null || p.Id != nhomTiemChungId)
                .Select(item => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,
                    ParentId = item.NhomDichVuBenhVienChaId
                })
                .ToList();

            var query = lstNhomDichVu.Select(item => new NhomDichVuBenhVienTreeViewVo
            {
                KeyId = item.KeyId,
                DisplayName = item.DisplayName,
                ParentId = item.ParentId,
                //Items = Services.KhamBenhs.KhamBenhService.GetChildrenTree(lstNhomDichVu, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
                Items = GetChildrenTree(lstNhomDichVu, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
            })
            .Where(x => x.ParentId == null &&
                        lstNhomDichVuCLSPTTT.Any(p => p == x.KeyId) &&
                        (string.IsNullOrEmpty(model.Query) || (!string.IsNullOrEmpty(model.Query) && (x.Items.Any() || x.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())))))
            .Take(model.Take).ToList();

            return query;
        }

        private List<NhomDichVuBenhVienTreeViewVo> GetChildrenTree(List<NhomDichVuBenhVienTreeViewVo> comments, long Id, string queryString, string parentDisplay)
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id)
                .Select(c => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetChildrenTree(comments, c.KeyId, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString)
                            || (!string.IsNullOrEmpty(queryString) && (parentDisplay.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any())))
                .ToList();
            return query;
        }

        public async Task XuLyThemYeuCauDichVuKyThuatMultiselectAsync(ChiDinhDichVuKyThuatMultiselectVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet)
        {
            var coBHYT = yeuCauTiepNhanChiTiet.CoBHYT ?? false;
            //var yeuCauKhamBenh = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == yeuCauVo.YeuCauKhamBenhId);
            //var yeuCauKyThuat = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.Add;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentPhongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var yeuCauDichVuKyThuatCuoiCung = BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanChiTiet.Id && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                                            .OrderByDescending(p => p.Id)
                                                                            .FirstOrDefault();
            //var yeuCauDichVuKyThuatCuoiCung = yeuCauKhamBenh.YeuCauDichVuKyThuats.OrderByDescending(x => x.Id).FirstOrDefault();
            yeuCauVo.YeuCauDichVuKyThuatCuoiCungId = yeuCauDichVuKyThuatCuoiCung == null ? 0 : yeuCauDichVuKyThuatCuoiCung.Id;


            var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();

            // xử lý get thông tin dịch vụ kỹ thuật và thêm mới
            foreach (var item in yeuCauVo.DichVuKyThuatBenhVienChiDinhs)
            {
                var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(item);

                var newYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
                {
                    DichVuKyThuatBenhVienId = itemObj.DichVuId,
                    NhomDichVuBenhVienId = itemObj.NhomId,
                    NoiThucHienId = itemObj.NoiThucHienId
                };

                var dvkt = _dichVuKyThuatBenhVienRepository.GetById(newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId, x => x.Include(o => o.DichVuKyThuatBenhVienGiaBaoHiems)
                    .Include(o => o.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Include(o => o.DichVuKyThuat));

                //var dvktGiaBH = dvkt.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));
                //var dvktGiaBV = dvkt.DichVuKyThuatVuBenhVienGiaBenhViens.First(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));
                var dvktGiaBH = dvkt.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || DateTime.Now.Date <= p.DenNgay.Value.Date));
                var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
                long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);
                var dvktGiaBV = dvkt.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p => p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || DateTime.Now.Date <= p.DenNgay.Value.Date))
                                                                        .OrderByDescending(p => p.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                                                                        .ThenBy(p => p.CreatedOn)
                                                                        .First();

                var dtudDVKTBV = yeuCauTiepNhanChiTiet.DoiTuongUuDai?.DoiTuongUuDaiDichVuKyThuatBenhViens?.FirstOrDefault(o =>
                                            o.DichVuKyThuatBenhVienId == newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVuKyThuatBenhVien.CoUuDai == true);

                var duocHuongBaoHiem = yeuCauTiepNhanChiTiet.NoiTruBenhAn == null && yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru != true ? false : (coBHYT && dvktGiaBH != null && dvktGiaBH.Gia != 0);

                //if (duocHuongBaoHiem)
                //{
                //    newYeuCauDichVuKyThuat.DuocHuongBaoHiem = true;
                //    newYeuCauDichVuKyThuat.BaoHiemChiTra = null;
                //}
                //else
                //{
                //    newYeuCauDichVuKyThuat.DuocHuongBaoHiem = false;
                //    newYeuCauDichVuKyThuat.BaoHiemChiTra = null;
                //}
                newYeuCauDichVuKyThuat.DuocHuongBaoHiem = false;
                newYeuCauDichVuKyThuat.BaoHiemChiTra = null;

                newYeuCauDichVuKyThuat.YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId;
                newYeuCauDichVuKyThuat.MaDichVu = dvkt.Ma;
                newYeuCauDichVuKyThuat.TenDichVu = dvkt.Ten;
                newYeuCauDichVuKyThuat.DuocHuongBaoHiem = duocHuongBaoHiem;
                newYeuCauDichVuKyThuat.Gia = dvktGiaBV.Gia;
                newYeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId = dvktGiaBV.NhomGiaDichVuKyThuatBenhVienId;
                newYeuCauDichVuKyThuat.NhomChiPhi = dvkt.DichVuKyThuat != null ? dvkt.DichVuKyThuat.NhomChiPhi : EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe;
                newYeuCauDichVuKyThuat.SoLan = 1;
                newYeuCauDichVuKyThuat.TiLeUuDai = dtudDVKTBV?.TiLeUuDai;
                newYeuCauDichVuKyThuat.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
                newYeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                newYeuCauDichVuKyThuat.NhanVienChiDinhId = currentUserId;
                newYeuCauDichVuKyThuat.NoiChiDinhId = currentPhongLamViecId;
                newYeuCauDichVuKyThuat.ThoiDiemChiDinh = DateTime.Now;
                newYeuCauDichVuKyThuat.ThoiDiemDangKy = DateTime.Now;
                newYeuCauDichVuKyThuat.NhomDichVuBenhVienId = dvkt.NhomDichVuBenhVienId;
                newYeuCauDichVuKyThuat.LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(newYeuCauDichVuKyThuat.NhomDichVuBenhVienId, lstNhomDichVuBenhVien);
                newYeuCauDichVuKyThuat.MaGiaDichVu = dvkt.DichVuKyThuat?.MaGia;
                newYeuCauDichVuKyThuat.TenGiaDichVu = dvkt.DichVuKyThuat?.TenGia;

                // get người thực hiện mặc định

                var nguoiThucHien = await GetBacSiThucHienMacDinh(newYeuCauDichVuKyThuat.NoiThucHienId ?? 0);
                if (nguoiThucHien != null)
                {
                    newYeuCauDichVuKyThuat.NhanVienThucHienId = nguoiThucHien.KeyId;
                }

                if (dvktGiaBH != null)
                {
                    newYeuCauDichVuKyThuat.DonGiaBaoHiem = dvktGiaBH.Gia;
                    newYeuCauDichVuKyThuat.TiLeBaoHiemThanhToan = dvktGiaBH.TiLeBaoHiemThanhToan;
                }

                yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.Add(newYeuCauDichVuKyThuat);
                //BaseRepository.Add(newYeuCauDichVuKyThuat);
                //await BaseRepository.AddAsync(newYeuCauDichVuKyThuat);
            }
        }

        private async Task<LookupItemVo> GetBacSiThucHienMacDinh(long noiThucHienId)
        {
            var nhanVien = _hoatDongNhanVienRepository.TableNoTracking
                .Include(hd => hd.NhanVien).ThenInclude(nv => nv.User)
                .Include(hd => hd.NhanVien).ThenInclude(nv => nv.ChucDanh)
                .Include(hd => hd.PhongBenhVien)
                .Where(hd => hd.PhongBenhVienId == noiThucHienId && hd.NhanVien.ChucDanh.NhomChucDanhId == (long)EnumNhomChucDanh.BacSi)
                .Select(hd => hd.NhanVien)
                .Select(s => new LookupItemVo
                {
                    DisplayName = s.User.HoTen,
                    KeyId = s.Id
                })
                .FirstOrDefault();

            return nhanVien;
        }

        public async Task<int> GetSoLuongDichVuKyThuatDaHoanThanhCanLamSang(long yeuCauTiepNhanId)
        {
            return BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            (p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) &&
            (p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)).Count();
        }

        public async Task<int> GetSoLuongDichVuKyThuatCanLamSang(long yeuCauTiepNhanId)
        {
            return BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            (p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)).Count();
        }


        public async Task<int> GetSoLuongDichVuKyThuatDaHoanThanh(long yeuCauTiepNhanId)
        {
            //return await BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            //                                                       (p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem || p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) &&
            //                                                       p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
            //                                           .CountAsync();

            return BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                   p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                       .Count();
        }

        public async Task<int> GetSoLuongDichVuKyThuat(long yeuCauTiepNhanId)
        {
            //return await BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
            //                                                       (p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem || p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang))
            //                                           .CountAsync();

            return BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                       .Count();
        }

        public async Task<(int, int)> GetTienTrinhHoanThanhDichVuKyThuat(long yeuCauTiepNhanId)
        {
            var lstTrangThai = BaseRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                .Select(x => x.TrangThai)
                .ToList();
            var slHoanThanh = lstTrangThai.Where(x => x == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien).Count();
            var slTong = lstTrangThai.Count();
            return (slHoanThanh, slTong);
        }

        //nhomDichVuBenhVienChaId == null
        public async Task<long> GetNhomDichVuBenhVienChaTheoNhomConId(long nhomDichVuBenhVienConId)
        {
            var nhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.Id == nhomDichVuBenhVienConId).FirstOrDefault();

            if (nhomDichVuBenhVien.NhomDichVuBenhVienChaId != null)
            {
                return await GetNhomDichVuBenhVienChaTheoNhomConId(nhomDichVuBenhVien.NhomDichVuBenhVienChaId ?? 0);
            }
            else
            {
                return nhomDichVuBenhVien.Id;
            }

        }

        public async Task<List<PhauThuatThuThuatCanLamSanGridVo>> GetDichVuKyThuatsByYeuCauTiepNhan(long yeuCauTiepNhanId, long? phieuDieuTriId = null)
        {
            //todo: có cập nhật bỏ await
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.Table
                .Include(p => p.KetQuaNhomXetNghiems)?.ThenInclude(p => p.FileKetQuaCanLamSangs)
                .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris)
                .Include(p => p.YeuCauDichVuKyThuats)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)?.ThenInclude(p => p.NhomDichVuBenhVienCha)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.FileKetQuaCanLamSangs)                
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.PhienXetNghiemChiTiets)?.ThenInclude(p => p.PhienXetNghiem)?.ThenInclude(p => p.YeuCauChayLaiXetNghiems)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.TaiKhoanBenhNhanChis)                
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)

                .Where(p => p.Id == yeuCauTiepNhanId)
                .FirstOrDefault();
            

            // setup data chp grip
            if (yeuCauTiepNhan != null)
            {
                //Explicit loading
                var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.YeuCauDichVuKyThuats);
                yeuCauDichVuKyThuats.Query()
                    //.Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                    //.Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                    //.Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                    .Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                    .Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                    .Include(p => p.NhomGiaDichVuKyThuatBenhVien)
                    .Include(p => p.NhomDichVuBenhVien).ThenInclude(p => p.NhomDichVuBenhVienCha)
                    .Include(p => p.NoiThucHien)
                    .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                    .Include(p => p.FileKetQuaCanLamSangs)
                    .Include(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                    //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.PhienXetNghiem).ThenInclude(p => p.YeuCauChayLaiXetNghiems)
                    .Include(p => p.TaiKhoanBenhNhanChis)
                    .Include(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                    .Include(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                    .Include(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
                    .Load();

                #region Kiểm tra gói dịch vụ

                var goiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                    .Where(x => ((x.BenhNhanId == yeuCauTiepNhan.BenhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == yeuCauTiepNhan.BenhNhanId && x.GoiSoSinh == true))
                                && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                                && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                    .ToList();
                #endregion

                var result = SetDataGripViewYeuCauDichVu(yeuCauTiepNhan, phieuDieuTriId, goiDichVus);
                return result;
            }

            return null;
        }

        private List<PhauThuatThuThuatCanLamSanGridVo> SetDataGripViewYeuCauDichVu(YeuCauTiepNhan yeuCauTiepNhan, long? phieuDieuTriId = null, List<YeuCauGoiDichVu> goiDichVus = null)
        {
            long userId = _userAgentHelper.GetCurrentUserId();

            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var goiDichVuKhamBenh = new List<PhauThuatThuThuatCanLamSanGridVo>();
            var stt = 1;

            var ngayDieuTri = DateTime.Now;
            if (phieuDieuTriId != null)
            {
                ngayDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == phieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;
            }

            if (goiDichVus == null)
            {
                goiDichVus = new List<YeuCauGoiDichVu>();
            }

            //var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
            //        .Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //        .Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //        .Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //        .Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
            //        .Include(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
            //        .Include(p => p.NhomGiaDichVuKyThuatBenhVien)
            //        .Include(p => p.NhomDichVuBenhVien).ThenInclude(p => p.NhomDichVuBenhVienCha)
            //        .Include(p => p.NoiThucHien)
            //        .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
            //        .Include(p => p.FileKetQuaCanLamSangs)
            //        .Include(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
            //        //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.PhienXetNghiem).ThenInclude(p => p.YeuCauChayLaiXetNghiems)
            //        .Include(p => p.TaiKhoanBenhNhanChis)
            //        .Include(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
            //        .Include(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
            //        .Include(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //        .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhan.Id && (p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) && (phieuDieuTriId == null || (phieuDieuTriId != null && p.LoaiDichVuKyThuat != LoaiDichVuKyThuat.SuatAn)))
            //        .ToList();

            //var phauThuatThuThuatCanLamSanGridVos = yeuCauDichVuKyThuats
            //            .OrderBy(x => x.NhomDichVuBenhVien.Ten)
            //            .ThenBy(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ? (x.DichVuKyThuatBenhVien.DichVuXetNghiem?.SoThuTu ?? (x.DichVuKyThuatBenhVien.DichVuXetNghiemId ?? 0)) : 0)
            //            .ThenBy(x => x.CreatedOn)
            //                                                               .Select(p => new PhauThuatThuThuatCanLamSanGridVo
            //                                                               {
            //                                                                   phieu dieu tri
            //                                                                   LoaiDichVuKyThuatEnum = p.LoaiDichVuKyThuat,
            //                                                                   LoaiDichVuKyThuatEnumDecription = p.LoaiDichVuKyThuat.GetDescription(),
            //                                                                   PhieuDieuTriId = p.NoiTruPhieuDieuTriId,
            //                                                                   GioBatDau = p.ThoiDiemDangKy,
            //                                                                   CoBHYT = yeuCauTiepNhan.CoBHYT ?? false,
            //                                                                   NgayYLenh = ngayDieuTri.ApplyFormatDate(),
            //                                                                   Id = p.Id,
            //                                                                   Nhom = GetTenNhomCha(p.NhomDichVuBenhVienId),
            //                                                                   NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
            //                                                                   NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
            //                                                                   LoaiDichVuKyThuat = (int)p.LoaiDichVuKyThuat,
            //                                                                   LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien?.Id,
            //                                                                   NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
            //                                                                   Ma = p.DichVuKyThuatBenhVien?.Ma,
            //                                                                   MaGiaDichVu = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaGia,
            //                                                                   MaTT37 = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.Ma4350,
            //                                                                   TenTT43 = p.TenGiaDichVu,
            //                                                                   TenDichVu = p.DichVuKyThuatBenhVien?.Ten,
            //                                                                   TenLoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
            //                                                                   LoaiGia = p.NhomGiaDichVuKyThuatBenhVienId,
            //                                                                   DonGia = p.YeuCauGoiDichVu != null ? p.DonGiaSauChietKhau : p.Gia, //p.Gia,
            //                                                                   ThanhTien = 0,
            //                                                                   BHYTThanhToan = 0,
            //                                                                   BNThanhToan = 0,
            //                                                                   NoiThucHien = String.Format("{0} - {1}", p.NoiThucHien?.Ma, p.NoiThucHien?.Ten),

            //                                                                   NoiThucHienId = p.NoiThucHienId ?? 0,
            //                                                                   TenNguoiThucHien = p.NhanVienThucHien?.User?.HoTen,
            //                                                                   NguoiThucHienId = p.NhanVienThucHienId,
            //                                                                   SoLuong = Convert.ToDouble(p.SoLan),

            //                                                                   TrangThaiDichVu = p.TrangThai.GetDescription(),
            //                                                                   TrangThaiDichVuId = (int)p.TrangThai,
            //                                                                   NhomChiPhiDichVuKyThuatId = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.NhomChiPhi,
            //                                                                   KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
            //                                                                   isCheckRowItem = false,
            //                                                                   DaThanhToan = p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan,
            //                                                                   DonGiaBaoHiem = p.DonGiaBaoHiem,
            //                                                                   DuocHuongBaoHiem = p.DuocHuongBaoHiem,
            //                                                                   NhanVienTaoYeuCauDichVuKyThuatId = p.CreatedById,
            //                                                                   YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,

            //                                                                   TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper() : null,
            //                                                                   TenGoiDichVu = p.YeuCauGoiDichVu != null ?
            //                                                                       "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
            //                                                                       : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
            //                                                                           p.MienGiamChiPhis
            //                                                                               .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
            //                                                                               .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
            //                                                                               .First() : null),
            //                                                                   KetQuaCanLamSanPTTT = p.FileKetQuaCanLamSangs.Select(p2 => new KetQuaCanLamSanPTTT
            //                                                                   {
            //                                                                       TenFile = p2.Ten,
            //                                                                       Url = _taiLieuDinhKemService.GetTaiLieuUrl(p2.DuongDan, p2.TenGuid),
            //                                                                       LoaiTapTin = p2.LoaiTapTin,
            //                                                                       MoTa = p2.MoTa,
            //                                                                       TenGuid = p2.TenGuid,
            //                                                                       DuongDan = p2.DuongDan
            //                                                                   }).ToList(),
            //                                                                   IsDichVuXetNghiem = p.NhomDichVuBenhVien?.Ma == "XN" || p.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "XN",
            //                                                                   BenhPhamXetNghiem = p.BenhPhamXetNghiem,
            //                                                                   ThoiDiemChiDinh = p.ThoiDiemChiDinh,
            //                                                                   IsCheckPhieuIn = true,
            //                                                                   KhongTinhPhi = p.KhongTinhPhi,
            //                                                                   PhienXetNghiemId = p.PhienXetNghiemChiTiets?.OrderByDescending(o => o.LanThucHien)?.FirstOrDefault()?.PhienXetNghiemId ?? 0,
            //                                                                   LanThucHien = p.PhienXetNghiemChiTiets?.OrderByDescending(o => o.LanThucHien)?.FirstOrDefault()?.LanThucHien ?? 0,
            //                                                                   LichSuPhienXetNghiemIds = p.PhienXetNghiemChiTiets?.Select(o => o.PhienXetNghiem)
            //                                                                                                                      .SelectMany(o => o.YeuCauChayLaiXetNghiems)
            //                                                                                                                      .Where(o => o.DuocDuyet != null)
            //                                                                                                                      .GroupBy(o => o.PhienXetNghiemId)
            //                                                                                                                      .Select(o => o.Key)
            //                                                                                                                      .ToList(),

            //                                                                   IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
            //                                                                   LyDoHuyDichVu = p.LyDoHuyDichVu,
            //                                                                   NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,

            //                                                                    gói marketing
            //                                                                   CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
            //                                                                   CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
            //                                                                   CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),
            //                                                                   LyDoKhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT?.LyDoKhongThucHien,
            //                                                                   KhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien,

            //                                                                   YeuCauDichVuKyThuatKhamSangLocTiemChungId = p.YeuCauDichVuKyThuatKhamSangLocTiemChungId,
            //                                                                   IsThuocNhomDichVuTiemChung = nhomTiemChungId != null && nhomTiemChungId == p.NhomDichVuBenhVienId && p.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null,

            //                                                                   BVHD-3654
            //                                                                   TinhPhi = p.KhongTinhPhi == null ? true : !p.KhongTinhPhi.Value,

            //                                                                   BVHD-3905
            //                                                                   TiLeThanhToanBHYT = p.DichVuKyThuatBenhVien.TiLeThanhToanBHYT,
            //                                                                   ThoiGianDienBien = p.ThoiGianDienBien
            //                                                               }).ToList();
            //goiDichVuKhamBenh.AddRange(phauThuatThuThuatCanLamSanGridVos);


            goiDichVuKhamBenh.AddRange(
                        yeuCauTiepNhan.YeuCauDichVuKyThuats?.Where(p => (p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                                                    && (phieuDieuTriId == null || (phieuDieuTriId != null && p.LoaiDichVuKyThuat != LoaiDichVuKyThuat.SuatAn)))
                        .OrderBy(x => x.NhomDichVuBenhVien.Ten)
                        .ThenBy(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ? (x.DichVuKyThuatBenhVien.DichVuXetNghiem?.SoThuTu ?? (x.DichVuKyThuatBenhVien.DichVuXetNghiemId ?? 0)) : 0)
                        .ThenBy(x => x.CreatedOn)
                                                                           .Select(p => new PhauThuatThuThuatCanLamSanGridVo
                                                                           {
                                                                               //phieu dieu tri
                                                                               LoaiDichVuKyThuatEnum = p.LoaiDichVuKyThuat,
                                                                               LoaiDichVuKyThuatEnumDecription = p.LoaiDichVuKyThuat.GetDescription(),
                                                                               PhieuDieuTriId = p.NoiTruPhieuDieuTriId,
                                                                               GioBatDau = p.ThoiDiemDangKy,
                                                                               CoBHYT = p.YeuCauTiepNhan.CoBHYT ?? false,
                                                                               NgayYLenh = ngayDieuTri.ApplyFormatDate(),
                                                                               Id = p.Id,
                                                                               Nhom = GetTenNhomCha(p.NhomDichVuBenhVienId),
                                                                               NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
                                                                               NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                                                                               LoaiDichVuKyThuat = (int)p.LoaiDichVuKyThuat,
                                                                               LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien?.Id,
                                                                               NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                                                                               Ma = p.DichVuKyThuatBenhVien?.Ma,
                                                                               MaGiaDichVu = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaGia,
                                                                               MaTT37 = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.Ma4350,
                                                                               TenTT43 = p.TenGiaDichVu,
                                                                               TenDichVu = p.DichVuKyThuatBenhVien?.Ten,
                                                                               TenLoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                                                                               LoaiGia = p.NhomGiaDichVuKyThuatBenhVienId,
                                                                               DonGia = p.YeuCauGoiDichVu != null ? p.DonGiaSauChietKhau : p.Gia, //p.Gia,
                                                                               ThanhTien = 0,
                                                                               BHYTThanhToan = 0,
                                                                               BNThanhToan = 0,
                                                                               NoiThucHien = String.Format("{0} - {1}", p.NoiThucHien?.Ma, p.NoiThucHien?.Ten),

                                                                               NoiThucHienId = p.NoiThucHienId ?? 0,
                                                                               TenNguoiThucHien = p.NhanVienThucHien?.User?.HoTen,
                                                                               NguoiThucHienId = p.NhanVienThucHienId,
                                                                               SoLuong = Convert.ToDouble(p.SoLan),

                                                                               TrangThaiDichVu = p.TrangThai.GetDescription(),
                                                                               TrangThaiDichVuId = (int)p.TrangThai,
                                                                               NhomChiPhiDichVuKyThuatId = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.NhomChiPhi,
                                                                               KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                                                                               isCheckRowItem = false,
                                                                               DaThanhToan = p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan,
                                                                               DonGiaBaoHiem = p.DonGiaBaoHiem,
                                                                               DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                                                                               NhanVienTaoYeuCauDichVuKyThuatId = p.CreatedById,
                                                                               YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,

                                                                               //TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper() : null,
                                                                               TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                                                                                   "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
                                                                                   : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                                                                       p.MienGiamChiPhis
                                                                                           .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                                                                           .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
                                                                                           .First() : null),
                                                                               KetQuaCanLamSanPTTT = p.FileKetQuaCanLamSangs.Select(p2 => new KetQuaCanLamSanPTTT
                                                                               {
                                                                                   TenFile = p2.Ten,
                                                                                   Url = _taiLieuDinhKemService.GetTaiLieuUrl(p2.DuongDan, p2.TenGuid),
                                                                                   LoaiTapTin = p2.LoaiTapTin,
                                                                                   MoTa = p2.MoTa,
                                                                                   TenGuid = p2.TenGuid,
                                                                                   DuongDan = p2.DuongDan
                                                                               }).ToList(),
                                                                               IsDichVuXetNghiem = p.NhomDichVuBenhVien?.Ma == "XN" || p.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "XN",
                                                                               BenhPhamXetNghiem = p.BenhPhamXetNghiem,
                                                                               ThoiDiemChiDinh = p.ThoiDiemChiDinh,
                                                                               IsCheckPhieuIn = true,
                                                                               KhongTinhPhi = p.KhongTinhPhi,
                                                                               //PhienXetNghiemId = p.PhienXetNghiemChiTiets?.OrderByDescending(o => o.LanThucHien)?.FirstOrDefault()?.PhienXetNghiemId ?? 0,                                                                               
                                                                               //LanThucHien = p.PhienXetNghiemChiTiets?.OrderByDescending(o => o.LanThucHien)?.FirstOrDefault()?.LanThucHien ?? 0,
                                                                               //LichSuPhienXetNghiemIds = p.PhienXetNghiemChiTiets?.Select(o => o.PhienXetNghiem)
                                                                               //                                                   .SelectMany(o => o.YeuCauChayLaiXetNghiems)
                                                                               //                                                   .Where(o => o.DuocDuyet != null)
                                                                               //                                                   .GroupBy(o => o.PhienXetNghiemId)
                                                                               //                                                   .Select(o => o.Key)
                                                                               //                                                   .ToList(),

                                                                               IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                                                                               LyDoHuyDichVu = p.LyDoHuyDichVu,
                                                                               NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,

                                                                               // gói marketing
                                                                               CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                                                               CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                                                               CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),
                                                                               LyDoKhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT?.LyDoKhongThucHien,
                                                                               KhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien,

                                                                               //YeuCauDichVuKyThuatKhamSangLocTiemChungId = p.YeuCauDichVuKyThuatKhamSangLocTiemChungId,
                                                                               IsThuocNhomDichVuTiemChung = nhomTiemChungId != null && nhomTiemChungId == p.NhomDichVuBenhVienId && p.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null,

                                                                               //BVHD-3654
                                                                               TinhPhi = p.KhongTinhPhi == null ? true : !p.KhongTinhPhi.Value,

                                                                               //BVHD-3905
                                                                               TiLeThanhToanBHYT = p.DichVuKyThuatBenhVien.TiLeThanhToanBHYT,
                                                                               ThoiGianDienBien = p.ThoiGianDienBien
                                                                           }));

            if (phieuDieuTriId != null)
            {
                goiDichVuKhamBenh = goiDichVuKhamBenh.Where(p => p.PhieuDieuTriId == phieuDieuTriId).ToList();

                #region BVHD-3575: cập nhật nội trú cho phép chỉ định dv khám

                if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru 
                    && yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
                {
                    var yeuCauTiepNhanNgoaiTruId = yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId;

                    var yeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking
                        //.Include(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                        //.Include(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)
                        //.Include(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                        .Include(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenh)
                        .Include(p => p.DichVuKhamBenhBenhVien)
                        .Include(p => p.NhomGiaDichVuKhamBenhBenhVien)
                        .Include(p => p.NoiThucHien)
                        .Include(p => p.NoiDangKy)
                        .Include(p => p.BacSiThucHien)?.ThenInclude(p => p.User)
                        .Include(p => p.BacSiDangKy)?.ThenInclude(p => p.User)
                        .Include(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                        .Include(p => p.TaiKhoanBenhNhanChis)
                        .Include(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                        .Include(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                        .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruId
                                    && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && x.LaChiDinhTuNoiTru != null
                                    && x.LaChiDinhTuNoiTru == true
                                    && x.ThoiDiemDangKy.Date == ngayDieuTri.Date)
                        .ToList();

                    var lstYeuCauKhamBenhChiDinh = yeuCauKhams
                        .Select(p => new PhauThuatThuThuatCanLamSanGridVo()
                        {
                            Id = p.Id,
                            Nhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                            NhomId = (int)EnumNhomGoiDichVu.DichVuKhamBenh,
                            LoaiYeuCauDichVuId = p.DichVuKhamBenhBenhVienId,
                            NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKhamBenhBenhVienId,
                            Ma = p.MaDichVu,
                            TenDichVu = p.TenDichVu,
                            TenLoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                            LoaiGia = p.NhomGiaDichVuKhamBenhBenhVienId,
                            DonGia = p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia,
                            BHYTThanhToan = 0,
                            NoiThucHien = p.NoiThucHien == null ? p.NoiDangKy?.Ten : p.NoiThucHien?.Ten,
                            NoiThucHienId = p.NoiThucHienId == null ? (p.NoiDangKyId ?? 0) : (p.NoiThucHienId ?? 0),
                            TenNguoiThucHien = p.BacSiThucHien == null ? p.BacSiDangKy?.User?.HoTen : p.BacSiThucHien?.User?.HoTen,
                            NguoiThucHienId = p.BacSiThucHienId == null ? p.BacSiDangKyId : p.BacSiThucHienId,
                            SoLuong = 1,
                            TrangThaiDichVu = p.TrangThai.GetDescription(),
                            TrangThaiDichVuId = (int)p.TrangThai,
                            KhongTinhPhi = p.KhongTinhPhi,
                            KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                            isCheckRowItem = false,
                            DaThanhToan = p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan,
                            DonGiaBaoHiem = p.DonGiaBaoHiem,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                                "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
                                : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                    p.MienGiamChiPhis
                                        .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                        .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
                                        .First() : null),
                            ThoiDiemChiDinh = p.ThoiDiemChiDinh,
                            ThanhTien = p.KhongTinhPhi == true ? 0 : (p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia),
                            IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                            LyDoHuyDichVu = p.LyDoHuyDichVu,
                            NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,

                            // gói marketing
                            CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                            CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                            CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),
                            
                            IsThuocNhomDichVuTiemChung = false,

                            //BVHD-3654
                            TinhPhi = p.KhongTinhPhi == null ? true : !p.KhongTinhPhi.Value,

                            IsCheckPhieuIn = false,
                            NhanVienTaoYeuCauDichVuKyThuatId = p.CreatedById,
                            GioBatDau = p.ThoiDiemDangKy,
                            NgayYLenh = ngayDieuTri.ApplyFormatDate(),
                            YeuCauTiepNhanNgoaiTruId = yeuCauTiepNhanNgoaiTruId,
                            ThoiGianDienBien = p.ThoiGianDienBien
                        }).ToList();

                    goiDichVuKhamBenh.AddRange(lstYeuCauKhamBenhChiDinh);
                }
                #endregion
            }
            //else
            //{
            //    goiDichVuKhamBenh = goiDichVuKhamBenh.Where(p => (p.LoaiDichVuKyThuatEnum == LoaiDichVuKyThuat.XetNghiem
            //    || p.LoaiDichVuKyThuatEnum == LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuatEnum == LoaiDichVuKyThuat.ThamDoChucNang)).ToList();
            //}

            //Sort
            //goiDichVuKhamBenh = goiDichVuKhamBenh.OrderBy(p => p.Nhom.ToUpper()).ToList();

            //tính toán tiền cho các dịch vụ & check xn lại
            foreach (var itemx in goiDichVuKhamBenh)
            {
                itemx.STT = stt++;

                decimal? thanhtien = itemx.DonGia * (decimal)itemx.SoLuong ?? 0;
                decimal? thanhTienBHTT = itemx.GiaBaoHiemThanhToan * (decimal)itemx.SoLuong ?? 0;

                itemx.ThanhTien = itemx.KhongTinhPhi != true ? thanhtien : 0;
                itemx.BHYTThanhToan = thanhTienBHTT;
                itemx.BNThanhToan = itemx.KhongTinhPhi != true ? (thanhtien - thanhTienBHTT) : 0;
                itemx.GoiXetNghiemLai = false;
                //itemx.GoiXetNghiemLai = itemx.PhienXetNghiemId != null ? IsGoiChayLaiXetNghiem(itemx.PhienXetNghiemId.GetValueOrDefault(), itemx.NhomDichVuBenhVienId) : false;
            }

            return goiDichVuKhamBenh;
        }

        #region Cập nhật 14/12/2022: grid dv load chậm
        public async Task<List<PhauThuatThuThuatCanLamSanGridVo>> GetDichVuKyThuatsByYeuCauTiepNhanVer2(long yeuCauTiepNhanId, long? phieuDieuTriId = null)
        {
            long userId = _userAgentHelper.GetCurrentUserId();

            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var goiDichVuKhamBenh = new List<PhauThuatThuThuatCanLamSanGridVo>();
            var stt = 1;

            var ngayDieuTri = DateTime.Now;
            if (phieuDieuTriId != null)
            {
                var phieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking.FirstOrDefault(x => x.Id == phieuDieuTriId);
                ngayDieuTri = phieuDieuTri?.NgayDieuTri ?? DateTime.Now;
            }

            var yeuCauKyThuats = BaseRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId 
                            && (p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                            && (phieuDieuTriId == null || (phieuDieuTriId != null && p.LoaiDichVuKyThuat != LoaiDichVuKyThuat.SuatAn && p.NoiTruPhieuDieuTriId == phieuDieuTriId)))
                .Select(p => new PhauThuatThuThuatCanLamSanGridVo
                {
                    //phieu dieu tri
                    LoaiDichVuKyThuatEnum = p.LoaiDichVuKyThuat,
                    LoaiDichVuKyThuatEnumDecription = p.LoaiDichVuKyThuat.GetDescription(),
                    PhieuDieuTriId = p.NoiTruPhieuDieuTriId,
                    GioBatDau = p.ThoiDiemDangKy,
                    CoBHYT = p.YeuCauTiepNhan.CoBHYT ?? false,
                    NgayYLenh = ngayDieuTri.ApplyFormatDate(),
                    Id = p.Id,
                    //Nhom = GetTenNhomCha(p.NhomDichVuBenhVienId),
                    NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
                    NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                    LoaiDichVuKyThuat = (int)p.LoaiDichVuKyThuat,
                    LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien.Id,
                    NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien.Id,
                    Ma = p.DichVuKyThuatBenhVien.Ma,
                    MaGiaDichVu = p.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia,
                    MaTT37 = p.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350,
                    TenTT43 = p.TenGiaDichVu,
                    TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                    TenLoaiGia = p.NhomGiaDichVuKyThuatBenhVien.Ten,
                    LoaiGia = p.NhomGiaDichVuKyThuatBenhVienId,
                    DonGia = p.YeuCauGoiDichVu != null ? p.DonGiaSauChietKhau : p.Gia, //p.Gia,
                    ThanhTien = 0,
                    BHYTThanhToan = 0,
                    BNThanhToan = 0,
                    NoiThucHien = String.Format("{0} - {1}", p.NoiThucHien.Ma, p.NoiThucHien.Ten),

                    NoiThucHienId = p.NoiThucHienId ?? 0,
                    TenNguoiThucHien = p.NhanVienThucHien.User.HoTen,
                    NguoiThucHienId = p.NhanVienThucHienId,
                    SoLuong = Convert.ToDouble(p.SoLan),

                    TrangThaiDichVu = p.TrangThai.GetDescription(),
                    TrangThaiDichVuId = (int)p.TrangThai,
                    NhomChiPhiDichVuKyThuatId = p.DichVuKyThuatBenhVien.DichVuKyThuat.NhomChiPhi,
                    KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                    isCheckRowItem = false,
                    DaThanhToan = p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan,
                    DonGiaBaoHiem = p.DonGiaBaoHiem,
                    DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                    NhanVienTaoYeuCauDichVuKyThuatId = p.CreatedById,
                    YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,

                    //TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper() : null,
                    //TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                    //    "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
                    //    : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                    //        p.MienGiamChiPhis
                    //            .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                    //            .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
                    //            .First() : null),
                    KetQuaCanLamSanPTTT = p.FileKetQuaCanLamSangs.Select(p2 => new KetQuaCanLamSanPTTT
                    {
                        TenFile = p2.Ten,
                        Url = _taiLieuDinhKemService.GetTaiLieuUrl(p2.DuongDan, p2.TenGuid),
                        LoaiTapTin = p2.LoaiTapTin,
                        MoTa = p2.MoTa,
                        TenGuid = p2.TenGuid,
                        DuongDan = p2.DuongDan
                    }).ToList(),
                    IsDichVuXetNghiem = p.NhomDichVuBenhVien.Ma == "XN" || p.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN",
                    BenhPhamXetNghiem = p.BenhPhamXetNghiem,
                    ThoiDiemChiDinh = p.ThoiDiemChiDinh,
                    IsCheckPhieuIn = true,
                    KhongTinhPhi = p.KhongTinhPhi,
                    //PhienXetNghiemId = p.PhienXetNghiemChiTiets?.OrderByDescending(o => o.LanThucHien)?.FirstOrDefault()?.PhienXetNghiemId ?? 0,                                                                               
                    //LanThucHien = p.PhienXetNghiemChiTiets?.OrderByDescending(o => o.LanThucHien)?.FirstOrDefault()?.LanThucHien ?? 0,
                    //LichSuPhienXetNghiemIds = p.PhienXetNghiemChiTiets?.Select(o => o.PhienXetNghiem)
                    //                                                   .SelectMany(o => o.YeuCauChayLaiXetNghiems)
                    //                                                   .Where(o => o.DuocDuyet != null)
                    //                                                   .GroupBy(o => o.PhienXetNghiemId)
                    //                                                   .Select(o => o.Key)
                    //                                                   .ToList(),

                    //IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                    LyDoHuyDichVu = p.LyDoHuyDichVu,
                    NguoiChiDinhDisplay = p.NhanVienChiDinh.User.HoTen,

                    // gói marketing
                    //CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                    //CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                    //CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),
                    LyDoKhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT.LyDoKhongThucHien,
                    KhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien,

                    //YeuCauDichVuKyThuatKhamSangLocTiemChungId = p.YeuCauDichVuKyThuatKhamSangLocTiemChungId,
                    IsThuocNhomDichVuTiemChung = nhomTiemChungId != null && nhomTiemChungId == p.NhomDichVuBenhVienId && p.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null,

                    //BVHD-3654
                    TinhPhi = p.KhongTinhPhi == null ? true : !p.KhongTinhPhi.Value,

                    //BVHD-3905
                    TiLeThanhToanBHYT = p.DichVuKyThuatBenhVien.TiLeThanhToanBHYT,
                    ThoiGianDienBien = p.ThoiGianDienBien,

                    //Cập nhật 14/12/2022: grid load chậm
                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId,
                    ChuongTrinhGoiDichVuId = p.YeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                    SoThuTuXetNghiem = p.DichVuKyThuatBenhVien.DichVuXetNghiem.SoThuTu,
                    DichVuXetNghiemId = p.DichVuKyThuatBenhVien.DichVuXetNghiemId,
                    TenNhomDichVuBenhVien = p.NhomDichVuBenhVien.Ten,
                    CreatedOn = p.CreatedOn
                }).ToList();

            if(yeuCauKyThuats.Any())
            {
                var lstNhomId = yeuCauKyThuats.Select(x => x.NhomDichVuBenhVienId).Distinct().ToList();
                var lstNhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking
                    .Select(x => new LookupItemTemplate
                    {
                        KeyId = x.Id,
                        NhomChaId = x.NhomDichVuBenhVienChaId,
                        DisplayName = x.Ten
                    }).ToList();
                yeuCauKyThuats.ForEach(x => x.Nhom = GetTenNhomCha(x.NhomDichVuBenhVienId, lstNhomDichVu));
            }

            yeuCauKyThuats = yeuCauKyThuats
                // cập nhật 18/05/2021: sắp xếp lại các dịch vụ xét nghiệm theo số thứ tự
                .OrderBy(x => x.TenNhomDichVuBenhVien)
                .ThenBy(x => x.LoaiDichVuKyThuat == (int)LoaiDichVuKyThuat.XetNghiem ? (x.SoThuTuXetNghiem ?? (x.DichVuXetNghiemId ?? 0)) : 0)
                .ThenBy(x => x.CreatedOn)
                .ToList();

            goiDichVuKhamBenh.AddRange(yeuCauKyThuats);

            if (phieuDieuTriId != null)
            {
                goiDichVuKhamBenh = goiDichVuKhamBenh.Where(p => p.PhieuDieuTriId == phieuDieuTriId).ToList();

                #region BVHD-3575: cập nhật nội trú cho phép chỉ định dv khám
                var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.Id == yeuCauTiepNhanId)
                    .Select(x => new
                    {
                        x.LoaiYeuCauTiepNhan,
                        x.YeuCauTiepNhanNgoaiTruCanQuyetToanId
                    }).FirstOrDefault();

                if (yeuCauTiepNhan != null 
                    && yeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                    && yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
                {
                    var yeuCauTiepNhanNgoaiTruId = yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
                    var lstYeuCauKhamBenhChiDinh = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruId
                                    && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && x.LaChiDinhTuNoiTru != null
                                    && x.LaChiDinhTuNoiTru == true
                                    && x.ThoiDiemDangKy.Date == ngayDieuTri.Date)
                        .Select(p => new PhauThuatThuThuatCanLamSanGridVo()
                        {
                            Id = p.Id,
                            Nhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                            NhomId = (int)EnumNhomGoiDichVu.DichVuKhamBenh,
                            LoaiYeuCauDichVuId = p.DichVuKhamBenhBenhVienId,
                            NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKhamBenhBenhVienId,
                            Ma = p.MaDichVu,
                            TenDichVu = p.TenDichVu,
                            TenLoaiGia = p.NhomGiaDichVuKhamBenhBenhVien.Ten,
                            LoaiGia = p.NhomGiaDichVuKhamBenhBenhVienId,
                            DonGia = p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia,
                            BHYTThanhToan = 0,
                            NoiThucHien = p.NoiThucHien == null ? p.NoiDangKy.Ten : p.NoiThucHien.Ten,
                            NoiThucHienId = p.NoiThucHienId == null ? (p.NoiDangKyId ?? 0) : (p.NoiThucHienId ?? 0),
                            TenNguoiThucHien = p.BacSiThucHien == null ? p.BacSiDangKy.User.HoTen : p.BacSiThucHien.User.HoTen,
                            NguoiThucHienId = p.BacSiThucHienId == null ? p.BacSiDangKyId : p.BacSiThucHienId,
                            SoLuong = 1,
                            TrangThaiDichVu = p.TrangThai.GetDescription(),
                            TrangThaiDichVuId = (int)p.TrangThai,
                            KhongTinhPhi = p.KhongTinhPhi,
                            KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                            isCheckRowItem = false,
                            DaThanhToan = p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan,
                            DonGiaBaoHiem = p.DonGiaBaoHiem,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            //TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                            //    "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
                            //    : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                            //        p.MienGiamChiPhis
                            //            .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                            //            .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
                            //            .First() : null),
                            ThoiDiemChiDinh = p.ThoiDiemChiDinh,
                            ThanhTien = p.KhongTinhPhi == true ? 0 : (p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia),
                            //IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                            LyDoHuyDichVu = p.LyDoHuyDichVu,
                            NguoiChiDinhDisplay = p.NhanVienChiDinh.User.HoTen,

                            // gói marketing
                            //CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                            //CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                            //CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),

                            IsThuocNhomDichVuTiemChung = false,

                            //BVHD-3654
                            TinhPhi = p.KhongTinhPhi == null ? true : !p.KhongTinhPhi.Value,

                            IsCheckPhieuIn = false,
                            NhanVienTaoYeuCauDichVuKyThuatId = p.CreatedById,
                            GioBatDau = p.ThoiDiemDangKy,
                            NgayYLenh = ngayDieuTri.ApplyFormatDate(),
                            YeuCauTiepNhanNgoaiTruId = yeuCauTiepNhanNgoaiTruId,
                            ThoiGianDienBien = p.ThoiGianDienBien,

                            //Cập nhật 14/12/2022: grid load chậm
                            BenhNhanId = p.YeuCauTiepNhan.BenhNhanId,
                            ChuongTrinhGoiDichVuId = p.YeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                        }).ToList();

                    goiDichVuKhamBenh.AddRange(lstYeuCauKhamBenhChiDinh);
                }
                #endregion
            }

            if (goiDichVuKhamBenh.Any())
            {
                #region Kiểm tra gói dịch vụ
                var benhNhanId = goiDichVuKhamBenh.Where(x => x.BenhNhanId != null).Select(x => x.BenhNhanId).FirstOrDefault();
                if (benhNhanId != null)
                {
                    var goiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                        .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                                    && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                                    && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                        .Select(x => x.ChuongTrinhGoiDichVuId)
                        .Distinct()
                        .ToList();

                    if (goiDichVus.Any())
                    {
                        #region //Cập nhật 14/12/2022
                        var chuongTrinhKhamBenhs = _chuongTrinhGoiDichVuKhamBenhRepository.TableNoTracking
                            .Where(x => goiDichVus.Contains(x.ChuongTrinhGoiDichVuId))
                            .Select(x => new
                            {
                                ChuongTrinhId = x.ChuongTrinhGoiDichVuId,
                                TenChuongTrinh = "Dịch vụ chọn từ gói: " + x.ChuongTrinhGoiDichVu.Ten + " - " + x.ChuongTrinhGoiDichVu.TenGoiDichVu,
                                DichVuId = x.DichVuKhamBenhBenhVienId
                            }).Distinct().ToList();
                        var chuongTrinhKhuyenMaiKhamBenhs = _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository.TableNoTracking
                            .Where(x => goiDichVus.Contains(x.ChuongTrinhGoiDichVuId))
                            .Select(x => x.DichVuKhamBenhBenhVienId)
                            .Distinct().ToList();

                        var chuongTrinhKyThuats = _chuongTrinhGoiDichVuKyThuatRepository.TableNoTracking
                            .Where(x => goiDichVus.Contains(x.ChuongTrinhGoiDichVuId))
                            .Select(x => new
                            {
                                ChuongTrinhId = x.ChuongTrinhGoiDichVuId,
                                TenChuongTrinh = "Dịch vụ chọn từ gói: " + x.ChuongTrinhGoiDichVu.Ten + " - " + x.ChuongTrinhGoiDichVu.TenGoiDichVu,
                                DichVuId = x.DichVuKyThuatBenhVienId
                            }).Distinct().ToList();
                        var chuongTrinhKhuyenMaiKyThuats = _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository.TableNoTracking
                            .Where(x => goiDichVus.Contains(x.ChuongTrinhGoiDichVuId))
                            .Select(x => x.DichVuKyThuatBenhVienId)
                            .Distinct().ToList();

                        var lstYeuCauKhamId = goiDichVuKhamBenh.Where(x => (Enums.EnumNhomGoiDichVu)x.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.Id).ToList();
                        var lstYeuCauKyThuatId = goiDichVuKhamBenh.Where(x => (Enums.EnumNhomGoiDichVu)x.NhomId == EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.Id).ToList();

                        var mienGiamChiPhis = _mienGiamChiPhiRepository.TableNoTracking
                            .Where(x => x.DaHuy != true
                                        && x.YeuCauGoiDichVuId != null
                                        && ((x.YeuCauKhamBenhId != null && lstYeuCauKhamId.Contains(x.YeuCauKhamBenhId.Value))
                                            || (x.YeuCauDichVuKyThuatId != null && lstYeuCauKyThuatId.Contains(x.YeuCauDichVuKyThuatId.Value)))
                                   )
                            .Select(x => new
                            {
                                Id = x.Id,
                                YeuCauKhamBenhId = x.YeuCauKhamBenhId,
                                YeuCauDichVuKyThuatId = x.YeuCauDichVuKyThuatId,
                                YeuCauGoiDichVuId = x.YeuCauGoiDichVuId,
                                ChuongTrinhId = x.YeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                                TenChuongTrinh = "Dịch vụ khuyến mãi chọn từ gói: " + x.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + x.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu,
                            }).ToList();

                        #endregion

                        foreach (var yeuCauDichVu in goiDichVuKhamBenh)
                        {
                            #region //Cập nhật 02/12/2022
                            if ((Enums.EnumNhomGoiDichVu)yeuCauDichVu.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh)
                            {
                                // check gói dv của người bệnh có chứa dv chỉ định
                                yeuCauDichVu.CoDichVuNayTrongGoi = chuongTrinhKhamBenhs.Any(b => b.DichVuId == yeuCauDichVu.LoaiYeuCauDichVuId);
                                yeuCauDichVu.CoDichVuNayTrongGoiKhuyenMai = chuongTrinhKhuyenMaiKhamBenhs.Contains(yeuCauDichVu.LoaiYeuCauDichVuId ?? 0);

                                // check miễn giảm
                                var mienGiams = mienGiamChiPhis.Where(x => x.YeuCauKhamBenhId == yeuCauDichVu.Id).ToList();
                                yeuCauDichVu.CoThongTinMienGiam = mienGiams.Any();

                                // check tên gói dv
                                if (yeuCauDichVu.ChuongTrinhGoiDichVuId.GetValueOrDefault() != 0)
                                {
                                    var chuongTrinhTheoDichVu = chuongTrinhKhamBenhs.FirstOrDefault(b => b.ChuongTrinhId == yeuCauDichVu.ChuongTrinhGoiDichVuId && b.DichVuId == yeuCauDichVu.LoaiYeuCauDichVuId);
                                    yeuCauDichVu.TenGoiDichVu = chuongTrinhTheoDichVu?.TenChuongTrinh;
                                }
                                if (string.IsNullOrEmpty(yeuCauDichVu.TenGoiDichVu) && mienGiams.Any())
                                {
                                    var thongTinGoiMienGiam = mienGiams.FirstOrDefault();
                                    yeuCauDichVu.TenGoiDichVu = thongTinGoiMienGiam?.TenChuongTrinh;
                                }
                            }
                            else
                            {
                                // check gói dv của người bệnh có chứa dv chỉ định
                                yeuCauDichVu.CoDichVuNayTrongGoi = chuongTrinhKyThuats.Any(b => b.DichVuId == yeuCauDichVu.LoaiYeuCauDichVuId);
                                yeuCauDichVu.CoDichVuNayTrongGoiKhuyenMai = chuongTrinhKhuyenMaiKyThuats.Contains(yeuCauDichVu.LoaiYeuCauDichVuId ?? 0);

                                // check miễn giảm
                                var mienGiams = mienGiamChiPhis.Where(x => x.YeuCauDichVuKyThuatId == yeuCauDichVu.Id).ToList();
                                yeuCauDichVu.CoThongTinMienGiam = mienGiams.Any();

                                // check tên gói dv
                                if (yeuCauDichVu.ChuongTrinhGoiDichVuId.GetValueOrDefault() != 0)
                                {
                                    var chuongTrinhTheoDichVu = chuongTrinhKyThuats.FirstOrDefault(b => b.ChuongTrinhId == yeuCauDichVu.ChuongTrinhGoiDichVuId && b.DichVuId == yeuCauDichVu.LoaiYeuCauDichVuId);
                                    yeuCauDichVu.TenGoiDichVu = chuongTrinhTheoDichVu?.TenChuongTrinh;
                                }

                                if (string.IsNullOrEmpty(yeuCauDichVu.TenGoiDichVu) && mienGiams.Any())
                                {
                                    var thongTinGoiMienGiam = mienGiams.FirstOrDefault();
                                    yeuCauDichVu.TenGoiDichVu = thongTinGoiMienGiam?.TenChuongTrinh;
                                }
                            }

                            // xử lý in hoa tên gói dịch vụ
                            if (!string.IsNullOrEmpty(yeuCauDichVu.TenGoiDichVu))
                            {
                                yeuCauDichVu.TenGoiDichVu = yeuCauDichVu.TenGoiDichVu.ToUpper();
                            }
                            #endregion
                        }
                    }
                }

                #endregion

                #region kiểm tra hủy thanh toán
                var lstYeuCauKhamChuaThanhToanId = goiDichVuKhamBenh.Where(x => (Enums.EnumNhomGoiDichVu)x.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh && x.DaThanhToan != true).Select(x => x.Id).ToList();
                var lstYeuCauKyThuatChuaThanhToanId = goiDichVuKhamBenh.Where(x => (Enums.EnumNhomGoiDichVu)x.NhomId == EnumNhomGoiDichVu.DichVuKyThuat && x.DaThanhToan != true).Select(x => x.Id).ToList();

                if (lstYeuCauKhamChuaThanhToanId.Any() || lstYeuCauKyThuatChuaThanhToanId.Any())
                {
                    var lstYeuCauIdHuyThanhToan = _taiKhoanBenhNhanChiRepository.TableNoTracking
                        .Where(x => (x.YeuCauKhamBenhId != null && lstYeuCauKhamChuaThanhToanId.Contains(x.YeuCauKhamBenhId.Value))
                                    || (x.YeuCauDichVuKyThuatId != null && lstYeuCauKyThuatChuaThanhToanId.Contains(x.YeuCauDichVuKyThuatId.Value)))
                        .Select(x => new
                        {
                            YeuCauKhamBenhId = x.YeuCauKhamBenhId,
                            YeuCauDichVuKyThuatId = x.YeuCauDichVuKyThuatId
                        }).Distinct().ToList();

                    if (lstYeuCauIdHuyThanhToan.Any())
                    {
                        var lstYeuCauKhamIdHuyThanhToan = lstYeuCauIdHuyThanhToan.Where(x => x.YeuCauKhamBenhId != null).Select(x => x.YeuCauKhamBenhId).Distinct().ToList();
                        var lstYeuCauKyThuatIdHuyThanhToan = lstYeuCauIdHuyThanhToan.Where(x => x.YeuCauDichVuKyThuatId != null).Select(x => x.YeuCauDichVuKyThuatId).Distinct().ToList();

                        goiDichVuKhamBenh.ForEach(x =>
                        {
                            if (x.DaThanhToan != true)
                            {
                                var enumNhomDichVu = (Enums.EnumNhomGoiDichVu)x.NhomId;
                                if (enumNhomDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                                    && lstYeuCauKhamIdHuyThanhToan.Any())
                                {
                                    x.IsDichVuHuyThanhToan = lstYeuCauKhamIdHuyThanhToan.Contains(x.Id);
                                }
                                else if (enumNhomDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                                            && lstYeuCauKyThuatIdHuyThanhToan.Any())
                                {
                                    x.IsDichVuHuyThanhToan = lstYeuCauKyThuatIdHuyThanhToan.Contains(x.Id);
                                }
                            }
                        });
                    }
                }

                #endregion
            }

            //tính toán tiền cho các dịch vụ & check xn lại
            foreach (var itemx in goiDichVuKhamBenh)
            {
                itemx.STT = stt++;

                decimal? thanhtien = itemx.DonGia * (decimal)itemx.SoLuong ?? 0;
                decimal? thanhTienBHTT = itemx.GiaBaoHiemThanhToan * (decimal)itemx.SoLuong ?? 0;

                itemx.ThanhTien = itemx.KhongTinhPhi != true ? thanhtien : 0;
                itemx.BHYTThanhToan = thanhTienBHTT;
                itemx.BNThanhToan = itemx.KhongTinhPhi != true ? (thanhtien - thanhTienBHTT) : 0;
                itemx.GoiXetNghiemLai = false;
                //itemx.GoiXetNghiemLai = itemx.PhienXetNghiemId != null ? IsGoiChayLaiXetNghiem(itemx.PhienXetNghiemId.GetValueOrDefault(), itemx.NhomDichVuBenhVienId) : false;
            }

            return goiDichVuKhamBenh;
        }

        private string GetTenNhomCha(long nhomId, List<LookupItemTemplate> lstNhom)
        {
            var tenNhom = string.Empty;
            var nhomHienTai = lstNhom.FirstOrDefault(x => x.KeyId == nhomId);
            if (nhomHienTai != null)
            {
                tenNhom = nhomHienTai.DisplayName;
                if (nhomHienTai.NhomChaId == null)
                {
                    return tenNhom.ToUpper();
                }
                else
                {
                    return $"{GetTenNhomCha(nhomHienTai.NhomChaId.Value, lstNhom)} - {tenNhom.ToUpper()}";
                }
            }
            return "";
        }
        #endregion

        public void ApDungThoiGianDienBienDichVuKhamVaKyThuat(List<long> yeuCauKhamBenhIds, List<long> yeuCauDichVuKyThuatIds, DateTime? thoiGianDienBien)
        {
            var yeuCauKhamBenhs = yeuCauKhamBenhIds.Any()
                ? _yeuCauKhamBenhRepository.Table.Where(o => yeuCauKhamBenhIds.Contains(o.Id)).ToList()
                : new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            var yeuCauDichVuKyThuats = yeuCauDichVuKyThuatIds.Any()
                ? _yeuCauDichVuKyThuatRepository.Table.Where(o => yeuCauDichVuKyThuatIds.Contains(o.Id)).ToList()
                : new List<YeuCauDichVuKyThuat>();

            if (yeuCauKhamBenhs.Any())
            {
                foreach (var yeuCauKhamBenh in yeuCauKhamBenhs)
                {
                    yeuCauKhamBenh.ThoiGianDienBien = thoiGianDienBien;
                }
            }
            if (yeuCauDichVuKyThuats.Any())
            {
                foreach (var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
                {
                    yeuCauDichVuKyThuat.ThoiGianDienBien = thoiGianDienBien;
                }
            }
            if (yeuCauKhamBenhs.Any() || yeuCauDichVuKyThuats.Any())
            {
                _yeuCauKhamBenhRepository.Context.SaveChanges();
            }
        }

        private string GetTenNhomCha(long? nhomDichVuBenhVienId)
        {
            if (nhomDichVuBenhVienId == null)
            {
                return "";
            }

            var nhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.Id == nhomDichVuBenhVienId).FirstOrDefault();

            if (nhomDichVuBenhVien.NhomDichVuBenhVienChaId == null)
            {
                return nhomDichVuBenhVien.Ten.ToUpper();
            }
            else
            {
                return $"{GetTenNhomCha(nhomDichVuBenhVien.NhomDichVuBenhVienChaId)} - {nhomDichVuBenhVien.Ten.ToUpper()}";
            }
        }
        private string InChiDinhInChungTatCa(long yeuCauTiepNhanId, bool KieuInChung, List<ListDichVuChiDinhCLSPTTT> lst, string hostingName, string content, bool? IsFromPhieuDieuTri, long? PhieuDieuTriId)
        {
            //KieuInChung => in 1. In Theo dịch vụ chỉ định (cùng người chỉ định dịch vụ) 2. In theo số thứ tự (cùng người chỉ định dịch vụ)
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)
                      .Include(p => p.NguoiLienHeQuanHeNhanThan)
                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)
                      .Include(p => p.BenhNhan)
                      .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                      .Include(cc => cc.PhuongXa)
                      .Include(cc => cc.QuanHuyen)
                      .Include(cc => cc.TinhThanh)
                      .Include(cc => cc.NoiTruBenhAn)
                      .Include(cc => cc.YeuCauTiepNhanTheBHYTs)
                      .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(s=>s.TrangThai !=  Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1

            var listInDichVuKyThuat = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat>();
            var listTheoNguoiChiDinh = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
            var lstDVKT = lst.Where(x => x.NhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat); // lấy ra những item dịch vụ kỹ thuật

            foreach (var itx in lstDVKT)
            {
                foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                {
                    if (itx.DichVuChiDinhId == ycdvkt.Id)
                    {
                        var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                        objNguoiChidinh.dichVuChiDinhId = itx.DichVuChiDinhId;
                        objNguoiChidinh.nhomChiDinhId = itx.NhomChiDinhId;
                        objNguoiChidinh.TenNhom = itx.TenNhom;
                        objNguoiChidinh.ThuTuIn = itx.ThuTuIn;
                        objNguoiChidinh.NhanVienChiDinhId = ycdvkt.NhanVienChiDinhId;
                        objNguoiChidinh.ThoiDiemChiDinh = ycdvkt.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru !=null ? new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0)
                                                                                       : new DateTime(ycdvkt.NoiTruPhieuDieuTri.NgayDieuTri.Year, ycdvkt.NoiTruPhieuDieuTri.NgayDieuTri.Month, ycdvkt.NoiTruPhieuDieuTri.NgayDieuTri.Day, 0, 0, 0);

                        listTheoNguoiChiDinh.Add(objNguoiChidinh);
                    }

                }
            }

            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();
            if (KieuInChung == true)
            {
                // lấy từng nhóm listInChiDinhTheoNguoiChiDinh vào 1 mảng list cần in 
                foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                {
                    var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                    listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                    content = AddChiDinhKhamBenhTheoNguoiChiDinhVaNhom(yeuCauTiepNhan, listCanIn, listDVK, listDVKT, content, hostingName, IsFromPhieuDieuTri, PhieuDieuTriId);
                }
            }
            else
            {   /// in theo STT và Người chỉ định

                foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                {
                    var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                    listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                    content = AddTungPhieuKhamBenhTheoNguoiChiDinhVaTheoSTT(yeuCauTiepNhan, listCanIn, listDVK, listDVKT, content, hostingName, IsFromPhieuDieuTri, PhieuDieuTriId);
                }
            }
            return content;
        }
        private string AddTungPhieuKhamBenhTheoNguoiChiDinhVaTheoSTT(YeuCauTiepNhan yeuCauTiepNhan, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
           List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
           List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string hostingName, bool? IsFromPhieuDieuTri, long? PhieuDieuTriId)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.ChanDoanSoBoICD)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauTiepNhan)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //    .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)
            //    .Include(p => p.NguoiLienHeQuanHeNhanThan)

            //    .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //    .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //    .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //    .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //    .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //    .Include(p => p.BenhNhan)
            //    .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //    .Include(cc => cc.PhuongXa)
            //    .Include(cc => cc.QuanHuyen)
            //    .Include(cc => cc.TinhThanh)
            //    .Include(cc => cc.NoiTruBenhAn).ThenInclude(pp => pp.NoiTruPhieuDieuTris)
            //    .Include(cc => cc.YeuCauTiepNhanTheBHYTs)
            //    // BVHD - 3800
            //    .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            //    .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            //BVHD-3916 ghi chú chỉ có DVKT
            var ghiChuDVKTs = new List<string>();

            


            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu;
            if (laCapCuu == null && yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
            {
                laCapCuu = _yeuCauTiepNhanRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId).Select(o => o.LaCapCuu).FirstOrDefault();
            }

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;

            string tenNguoiChiDinh = "";

            content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            var i = 1;

            var chanDoanSoBos = new List<string>();
            var dienGiaiChanDoanSoBo = new List<string>();
            List<ListDichVuChiDinhCLSPTTT> lstDichVuChidinhTheoSoThuTu = new List<ListDichVuChiDinhCLSPTTT>();
           
            var lstdvkt = yeuCauTiepNhan?.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null).ToList();

            string ngay = "";
            string thang = "";
            string nam = "";

            decimal tongCong = 0;
            int soLuong = 0;
          
            if (listDichVuTheoNguoiChiDinh.Count() > 0)
            {
                // BVHD-3939 // == 1 
                var listDichVuIds = listDichVuTheoNguoiChiDinh.Select(d => d.dichVuChiDinhId).ToList();
                var thanhTienDv = listDVKT.Where(d => listDichVuIds.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                    .Sum();

                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();

                foreach (var Itemx in listDichVuTheoNguoiChiDinh)
                {

                    if (Itemx.nhomChiDinhId == (int)EnumNhomGoiDichVu.DichVuKyThuat)
                    {
                        if (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Any())
                        {
                            var maHocHamVi = string.Empty;
                            var maHocHamViId = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.NhanVienChiDinh?.HocHamHocViId);
                            if (maHocHamViId.Any(d=>d != null))
                            {
                                maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                            }

                            tenNguoiChiDinh = returnStringTen(maHocHamVi,
                                                              "",
                                                              lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).Any() ? lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).First() : "");

                            dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).FirstOrDefault());
                            dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).FirstOrDefault());
                            if(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhICD != null).Any())
                            {
                                chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + a.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet ).FirstOrDefault());
                            }
                            if (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoICD != null).Any())
                            {
                                chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + a.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet).FirstOrDefault());
                            }

                            ngay = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d=>d.ThoiDiemDangKy.Day.ToString()).First();
                            thang = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                            nam = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().SoLan + "</td>";
                            htmlDanhSachDichVu += "<td style='border: 1px solid #020000;text-align: center;'></td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            soLuong += lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().SoLan;
                            // BVHD-3916
                            // nội trú
                            var noiTruPhieuDieuTriId = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTriId).FirstOrDefault();
                            if (noiTruPhieuDieuTriId != null)
                            {
                                if (!string.IsNullOrWhiteSpace(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTri.GhiChuCanLamSang).FirstOrDefault())){
                                    ghiChuDVKTs.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTri.GhiChuCanLamSang.Replace("\n", "<br>")).FirstOrDefault());
                                }
                            }
                            //  ngoại trú
                            var yeuCauKhamBenhId = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(a => a.YeuCauKhamBenhId).FirstOrDefault();
                            if (yeuCauKhamBenhId != null)
                            {
                                if (!string.IsNullOrWhiteSpace(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.YeuCauKhamBenh.GhiChuCanLamSang).FirstOrDefault()))
                                {
                                    ghiChuDVKTs.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.YeuCauKhamBenh.GhiChuCanLamSang.Replace("\n", "<br>")).FirstOrDefault());
                                }
                            }

                        }
                    }
                }
            }

            // BVHD-3939- page -total
            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
            // BVHD-3939 - số lượng
            htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
            htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

            htmlDanhSachDichVu += " </tr>";
            // end BVHD-3939


            var doiTuongs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                  .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                  .Select(a => a.MucHuong.ToString()).ToList();
            var dt = doiTuongs.Any() ? doiTuongs.First() : "";

            var soBHYTs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                              .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                              .Select(a => a.MaSoThe.ToString()).ToList();
            var soBHYT = soBHYTs.Any() ? soBHYTs.First() : "";


            var data = new
            {
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan?.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan?.BenhNhan != null ? yeuCauTiepNhan?.BenhNhan.MaBN : "",
                HoTen = yeuCauTiepNhan?.HoTen ?? "",
                GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                Ngay = ngay,
                Thang = thang,
                Nam = nam,
                DienThoai = yeuCauTiepNhan?.SoDienThoai,
                DoiTuong = yeuCauTiepNhan.CoBHYT == true ? "BHYT (" + yeuCauTiepNhan.BHYTMucHuong + "%)" : "Viện phí",

                SoTheBHYT = yeuCauTiepNhan?.BHYTMaSoThe,

                HanThe = yeuCauTiepNhan.BHYTNgayHieuLuc != null && yeuCauTiepNhan.BHYTNgayHetHan != null ? "từ ngày: " + yeuCauTiepNhan.BHYTNgayHieuLuc.GetValueOrDefault().ToString("dd/MM/yyyy")
                                                                                                          + " đến ngày: " + yeuCauTiepNhan.BHYTNgayHetHan.GetValueOrDefault().ToString("dd/MM/yyyy") : "",

                NoiYeuCau = tenPhong,
                
                 ChuanDoanSoBo = chanDoanSoBos.Where(item => item != null && item !="-").ToList().Distinct().Join("; "),
                DienGiai = dienGiaiChanDoanSoBo.Where(item => item != null).ToList().Distinct().Join("; "),

                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = tenNguoiChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,
                PhieuThu = "DichVuKyThuat",
                //BVHD-3800
                CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : "",
                // BVHD-3916
                GhiChuCanLamSang = ghiChuDVKTs.Distinct().ToList().Join(", ")
            };

            var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
            content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
            if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
            {
                var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                var tmpKB = "<tr id=\"NguoiGiamHo\">";
                var test = content.IndexOf(tmp);
                content = content.Replace(tmpKB, tampKB);
            }

            htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            i = 1;
            return content;

        }
        private string AddChiDinhKhamBenhTheoNguoiChiDinhVaNhom(YeuCauTiepNhan yeuCauTiepNhan, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
          List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
          List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string hostingName, bool? IsFromPhieuDieuTri, long? PhieuDieuTriId)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)


            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.ChanDoanSoBoICD)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauTiepNhan)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)
            //                      .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //                      .Include(p => p.BenhNhan)
            //                      .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //                      .Include(cc => cc.PhuongXa)
            //                      .Include(cc => cc.QuanHuyen)
            //                      .Include(cc => cc.TinhThanh)
            //                      .Include(cc => cc.NoiTruBenhAn).ThenInclude(pp => pp.NoiTruPhieuDieuTris)
            //                      .Include(cc=>cc.YeuCauTiepNhanTheBHYTs)
            //                      //BVHD-3800
            //                      .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            //                      .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            //BVHD-3916 ghi chú chỉ có DVKT
            var ghiChuDVKTs = new List<string>();

           
            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu;
            if (laCapCuu == null && yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
            {
                laCapCuu = _yeuCauTiepNhanRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId).Select(o => o.LaCapCuu).FirstOrDefault();
            }


            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;

            string tenNguoiChiDinh = "";
            content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";

            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            var lstInThuTuTheoNhomDichVu = listDichVuTheoNguoiChiDinh.First().nhomChiDinhId;

            var chanDoanSoBos = new List<string>();
            var dienGiaiChanDoanSoBo = new List<string>();

            string ngay = "";
            string thang = "";
            string nam = "";

            if (lstInThuTuTheoNhomDichVu == (long)EnumNhomGoiDichVu.DichVuKyThuat)
            {
                var isHave = false;
                var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
                htmlDanhSachDichVu += "</tr>";
                var i = 1;
                int indexDVKT = 1;
                var listInDichVuKyThuat = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat>();
                var lstDVKT = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)EnumNhomGoiDichVu.DichVuKyThuat).ToList();

                foreach (var itx in lstDVKT)
                {
                    foreach (var ycdvkt in yeuCauTiepNhan?.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {
                            listInDichVuKyThuat.Add(ycdvkt);
                        }

                    }
                }
                List<ListDichVuChiDinhCLSPTTT> lstDichVuCungChidinh = new List<ListDichVuChiDinhCLSPTTT>();
                List<ListDichVuChiDinhCLSPTTT> lstDichVuChidinhTungPhieu = new List<ListDichVuChiDinhCLSPTTT>();

                List<ListDichVuChiDinhCLSPTTT> lstDichVuChidinh = new List<ListDichVuChiDinhCLSPTTT>();
                foreach (var ycdvkt in listInDichVuKyThuat)
                {
                    var lstDichVuKT = new ListDichVuChiDinhCLSPTTT();
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    lstDichVuKT.TenNhom = nhomDichVu;
                    lstDichVuKT.NhomChiDinhId = ycdvkt.NhomDichVuBenhVien.Id;
                    lstDichVuKT.DichVuChiDinhId = ycdvkt.Id;
                    lstDichVuChidinh.Add(lstDichVuKT);
                }
                var lstdvkt = yeuCauTiepNhan?.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null);
                decimal tongCong = 0;
                int soLuong = 0;

                foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
                {

                    if (dv.Count() > 1)
                    {
                       
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.DichVuChiDinhId).ToList();
                        var thanhTienDv = listDVKT.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();

                        CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                        tongCong += thanhTienDv.GetValueOrDefault();

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Any())
                            {
                                //var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();


                                dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).FirstOrDefault());

                                dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).FirstOrDefault());
                                
                                if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhICD != null).Any())
                                {
                                    chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + a.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet).FirstOrDefault());
                                }
                                if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoICD != null).Any())
                                {
                                    chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + a.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet).FirstOrDefault());
                                }

                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d=>d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                if (indexDVKT == 1)
                                {
                                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                                    htmlDanhSachDichVu += " </tr>";
                                }

                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.HocHamHocViId);
                                if (maHocHamViId.Any(d=>d != null))
                                {
                                    maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                                }

                                tenNguoiChiDinh = returnStringTen(maHocHamVi,
                                                                   ""
                                                                    , lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).Any() ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).First() : "");

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan;
                                // BVHD-3916
                                // nội trú
                                var noiTruPhieuDieuTriId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTriId).FirstOrDefault();
                                if (noiTruPhieuDieuTriId != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTri.GhiChuCanLamSang).FirstOrDefault())){
                                        ghiChuDVKTs.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTri.GhiChuCanLamSang.Replace("\n", "<br>")).FirstOrDefault());
                                    }
                                }
                                //  ngoại trú
                                var yeuCauKhamBenhId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.YeuCauKhamBenhId).FirstOrDefault();
                                if (yeuCauKhamBenhId != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.YeuCauKhamBenh.GhiChuCanLamSang).FirstOrDefault()))
                                    {
                                        ghiChuDVKTs.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.YeuCauKhamBenh.GhiChuCanLamSang.Replace("\n", "<br>")).FirstOrDefault());
                                    }
                                }
                            }
                        }
                        indexDVKT = 1;
                    }
                    if (dv.Count() == 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.DichVuChiDinhId).ToList();
                        var thanhTienDv = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();

                        CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                        tongCong += thanhTienDv.GetValueOrDefault();

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First() != null)
                            {
                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.HocHamHocViId);
                                if (maHocHamViId.Any(d=>d != null))
                                {
                                    maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                                }
                                tenNguoiChiDinh = returnStringTen(maHocHamVi,
                                                                    ""
                                                                     , lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).Any() ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).First() : "");

                                dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).FirstOrDefault());

                                dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).FirstOrDefault());

                                if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhICD != null).Any())
                                {
                                    chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + a.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet).FirstOrDefault());
                                }
                                if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoICD != null).Any())
                                {
                                    chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + a.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet).FirstOrDefault());
                                }

                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan;
                                // BVHD-3916
                                var noiTruPhieuDieuTriId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTriId).FirstOrDefault();
                                if (noiTruPhieuDieuTriId != null)
                                {
                                    if(!string.IsNullOrWhiteSpace(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTri.GhiChuCanLamSang).FirstOrDefault())){
                                        ghiChuDVKTs.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTri.GhiChuCanLamSang.Replace("\n","<br>")).FirstOrDefault());
                                    }
                                }
                                var yeuCauKhamBenhId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(a => a.YeuCauKhamBenhId).FirstOrDefault();
                                if (yeuCauKhamBenhId != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.YeuCauKhamBenh.GhiChuCanLamSang).FirstOrDefault()))
                                    {
                                        ghiChuDVKTs.Add(lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.YeuCauKhamBenh.GhiChuCanLamSang.Replace("\n", "<br>")).FirstOrDefault());
                                    }
                                }
                            }
                        }
                        indexDVKT = 1;
                    }
                }

                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var doiTuongs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                  .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                  .Select(a => a.MucHuong.ToString()).ToList();
                var dt = doiTuongs.Any() ? doiTuongs.First() : "";

                var soBHYTs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                  .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                  .Select(a => a.MaSoThe.ToString()).ToList();
                var soBHYT = soBHYTs.Any() ? soBHYTs.First() : "";

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan?.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan?.BenhNhan != null ? yeuCauTiepNhan?.BenhNhan.MaBN : "",
                    HoTen = yeuCauTiepNhan?.HoTen ?? "",
                    GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                    Ngay = ngay,
                    Thang = thang,
                    Nam = nam,
                    DienThoai = yeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauTiepNhan.CoBHYT == true ? "BHYT (" + yeuCauTiepNhan.BHYTMucHuong + "%)" : "Viện phí",

                    SoTheBHYT = yeuCauTiepNhan?.BHYTMaSoThe,

                    HanThe = yeuCauTiepNhan.BHYTNgayHieuLuc != null && yeuCauTiepNhan.BHYTNgayHetHan != null ? "từ ngày: " + yeuCauTiepNhan.BHYTNgayHieuLuc.GetValueOrDefault().ToString("dd/MM/yyyy")
                                                                                                          + " đến ngày: " + yeuCauTiepNhan.BHYTNgayHetHan.GetValueOrDefault().ToString("dd/MM/yyyy") : "",

                    NoiYeuCau = tenPhong,
                    ChuanDoanSoBo = chanDoanSoBos.Where(item => item != null && item !="-").ToList().Distinct().Join("; "),
                    DienGiai = dienGiaiChanDoanSoBo.Where(item => item != null).ToList().Distinct().Join("; "),
                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,
                    PhieuThu = "DichVuKyThuat",
                    //BVHD-3800
                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : "",
                    // BVHD-3916
                    GhiChuCanLamSang = ghiChuDVKTs.Distinct().ToList().Join(", ")
                };

                var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
                if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                {
                    var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                    var tmpKB = "<tr id=\"NguoiGiamHo\">";
                    var test = content.IndexOf(tmp);
                    content = content.Replace(tmpKB, tampKB);
                }

                htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
                htmlDanhSachDichVu += "</tr>";
                i = 1;
            }
            return content;

        }


    
        public string InBaoCaoChiDinh(long yeuCauTiepNhanId, string hostingName, List<ListDichVuChiDinhCLSPTTT> lst
            , long inChungChiDinh, bool KieuInChung, bool? IsFromPhieuDieuTri, long? PhieuDieuTriId , List<long> listChonLoaiPhieuIn)
        {
            long userId = _userAgentHelper.GetCurrentUserId();
            var nguoiChiDinh = _userRepository.GetById(userId);
            var content = "";

            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)
            //        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)
            //        .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
            //        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
            //        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
            //        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
            //        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
            //        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
            //        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


            //        .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //        .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //        .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)

            //        //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
            //        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
            //        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc).ThenInclude(p => p.KhoaPhong)
            //        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

            //        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
            //        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu).ThenInclude(p => p.KhoaPhong)
            //        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)

            //        .Include(p => p.BenhNhan)
            //        .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //        .Include(cc => cc.PhuongXa)
            //        .Include(cc => cc.QuanHuyen)
            //        .Include(cc => cc.TinhThanh)
            //        .Include(cc => cc.NoiTruBenhAn).ThenInclude(pp => pp.NoiTruPhieuDieuTris)
            //        .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            var tenNguoiChiDinh = _userRepository.TableNoTracking.Where(p => p.Id == userId).Select(p => p.HoTen).FirstOrDefault();
            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
            
            
            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1
            if (inChungChiDinh == 1)
            {
                if (KieuInChung == true)
                {
                    content = InChiDinhInChungTatCa(yeuCauTiepNhanId, true, lst, hostingName, content, IsFromPhieuDieuTri, PhieuDieuTriId);

                    if (lst.Where(d=>d.NhomChiDinhId == 1).Count() != 0)
                    {
                        var newModelIn = new InChiDinhDichVuKhamNoiTruGridVo()
                        {
                            DichVuKhamNoiTruCanIns = lst.Where(d => d.NhomChiDinhId == 1)
                                                        .Select(d => new ListDichVuChiDinhTheoNguoiChiDinh() {
                                                            dichVuChiDinhId = d.DichVuChiDinhId,
                                                            NhanVienChiDinhId = d.NhanVienChiDinhId,
                                                            nhomChiDinhId = d.NhomChiDinhId,
                                                            TenNhom = d.TenNhom,
                                                            ThuTuIn = d.ThuTuIn
                                                        }).ToList(), // dv khams
                            HosTingName = hostingName,
                            inChungChiDinh = inChungChiDinh,
                            KieuIn = true,
                            YeuCauTiepNhanId = yeuCauTiepNhanId
                        };
                        if (!string.IsNullOrEmpty(content))
                        {
                            content += "<div class=\"pagebreak\"> </div>";
                        }
                        content += InChiDinhDichVuKham(newModelIn);
                    }
                }
                else
                {

                    content = InChiDinhInChungTatCa(yeuCauTiepNhanId, false, lst, hostingName, content, IsFromPhieuDieuTri, PhieuDieuTriId);

                    if (lst.Where(d => d.NhomChiDinhId == 1).Count() != 0)
                    {
                        var newModelIn = new InChiDinhDichVuKhamNoiTruGridVo()
                        {
                            DichVuKhamNoiTruCanIns = lst.Where(d => d.NhomChiDinhId == 1)
                                                        .Select(d => new ListDichVuChiDinhTheoNguoiChiDinh()
                                                        {
                                                            dichVuChiDinhId = d.DichVuChiDinhId,
                                                            NhanVienChiDinhId = d.NhanVienChiDinhId,
                                                            nhomChiDinhId = d.NhomChiDinhId,
                                                            TenNhom = d.TenNhom,
                                                            ThuTuIn = d.ThuTuIn
                                                        }).ToList(), // dv khams
                            HosTingName = hostingName,
                            inChungChiDinh = inChungChiDinh,
                            KieuIn = false,
                            YeuCauTiepNhanId = yeuCauTiepNhanId
                        };
                        if (!string.IsNullOrEmpty(content))
                        {
                            content += "<div class=\"pagebreak\"> </div>";
                        }
                       
                        content += InChiDinhDichVuKham(newModelIn);
                    }
                }
            }

            // in tung phiếu
            if (inChungChiDinh == 0)
            {
                var listDichVuChiDinh = new List<ListDichVuChiDinh>();
                foreach (var item in lst)
                {
                    var obj = new ListDichVuChiDinh();
                    obj.dichVuChiDinhId = item.DichVuChiDinhId;
                    obj.nhomChiDinhId = item.NhomChiDinhId;
                    obj.TenNhom = item.TenNhom;
                    obj.ThuTuIn = item.ThuTuIn;
                    listDichVuChiDinh.Add(obj);
                }
                content = InChiDinhInTungPhieuTatCa(yeuCauTiepNhanId, listDichVuChiDinh, hostingName, IsFromPhieuDieuTri, PhieuDieuTriId);

                if (lst.Where(d => d.NhomChiDinhId == 1).Count() != 0)
                {
                    var newModelIn = new InChiDinhDichVuKhamNoiTruGridVo()
                    {
                        DichVuKhamNoiTruCanIns = lst.Where(d => d.NhomChiDinhId == 1)
                                                        .Select(d => new ListDichVuChiDinhTheoNguoiChiDinh()
                                                        {
                                                            dichVuChiDinhId = d.DichVuChiDinhId,
                                                            NhanVienChiDinhId = d.NhanVienChiDinhId,
                                                            nhomChiDinhId = d.NhomChiDinhId,
                                                            TenNhom = d.TenNhom,
                                                            ThuTuIn = d.ThuTuIn
                                                        }).ToList(), // dv khams
                        HosTingName = hostingName,
                        inChungChiDinh = inChungChiDinh,
                        KieuIn = false,
                        YeuCauTiepNhanId = yeuCauTiepNhanId
                    };
                    if (!string.IsNullOrEmpty(content))
                    {
                        content += "<div class=\"pagebreak\"> </div>";
                    }
                    content += InChiDinhDichVuKham(newModelIn);
                }
            }

            return content;
        }
        private string AddPhieuInKhamBenhDichVuChiDinhTheoNguoiChiDinh(string content, Enums.LoaiDichVuKyThuat loaiDichVuKyThuat, YeuCauTiepNhan yeuCauTiepNhan, string hostingName, List<ListDichVuChiDinhTheoNguoiChiDinh> lst, bool? IsFromPhieuDieuTri, long? PhieuDieuTriId)
        {
            var listSarsCov2CauHinh = GetListSarsCauHinh();

            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

 


            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.ChanDoanSoBoICD)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauTiepNhan)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)
            //                      .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //                      .Include(p => p.BenhNhan)
            //                      .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //                      .Include(cc => cc.PhuongXa)
            //                      .Include(cc => cc.QuanHuyen)
            //                      .Include(cc => cc.TinhThanh)
            //                      .Include(cc => cc.NoiTruBenhAn).ThenInclude(pp => pp.NoiTruPhieuDieuTris)
            //                      .Include(cc => cc.YeuCauTiepNhanTheBHYTs)
            //                       //BVHD-3800
            //                       .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            //                      .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();
            //BVHD-3916 ghi chú chỉ có DVKT
            var ghiChuDVKTs = new List<string>();
            

            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu;
            if(laCapCuu == null && yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
            {
                laCapCuu = _yeuCauTiepNhanRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId).Select(o=>o.LaCapCuu).FirstOrDefault();
            }

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(d=> !listSarsCov2CauHinh.Contains(d.DichVuKyThuatBenhVienId)).ToList()); // tất cả dịch vụ kỹ thuật theo yctn

            string tenNguoiChiDinh = "";

            var isHave = false;
            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            string tampTenNhomDichVu = "";
            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            var i = 1;
            int indexDVKT = 1;

            string ngay = "";
            string thang = "";
            string nam = "";

            if (lst.Count() == 1)
            {
                decimal tongCong = 0;
                int soLuong = 0;
                // BVHD-3939 // == 1 
                var listDichVuIds = lst.Select(d => d.dichVuChiDinhId).ToList();
                var thanhTienDv = listDVKT.Where(d => listDichVuIds.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                    .Sum();

                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();

                var chanDoanSoBos = new List<string>();
                var dienGiaiChanDoanSoBo = new List<string>();
                foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                               && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.Id == lst.First().dichVuChiDinhId))
                {
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    isHave = true;
                    var maHocHamVi = string.Empty;
                    var maHocHamViId = ycdvkt.NhanVienChiDinh?.HocHamHocViId;
                    if (maHocHamViId != null)
                    {
                        maHocHamVi = MaHocHamHocVi((long)maHocHamViId) ;
                    }
                    tenNguoiChiDinh = returnStringTen(maHocHamVi, "", ycdvkt.NhanVienChiDinh?.User?.HoTen);

                    dienGiaiChanDoanSoBo.Add(ycdvkt?.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);
                    dienGiaiChanDoanSoBo.Add(ycdvkt?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu);
                    if (ycdvkt?.NoiTruPhieuDieuTri?.ChanDoanChinhICD != null)
                    {
                        chanDoanSoBos.Add(ycdvkt?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + ycdvkt?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet);
                    }
                    if (ycdvkt?.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                    {
                        chanDoanSoBos.Add(ycdvkt?.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt?.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet);
                    }

                    ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                    thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                    nam = ycdvkt.ThoiDiemDangKy.Year.ToString();

                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;' ><b>{thanhTienFormat}</b></td>";
                    htmlDanhSachDichVu += " </tr>";
                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                    htmlDanhSachDichVu += " </tr>";
                    i++;
                    indexDVKT++;
                    soLuong += ycdvkt.SoLan;
                    // BVHD-3916
                    if(ycdvkt.NoiTruPhieuDieuTriId != null)
                    {
                        if (!string.IsNullOrWhiteSpace(ycdvkt?.NoiTruPhieuDieuTri?.GhiChuCanLamSang))
                        {
                            ghiChuDVKTs.Add(ycdvkt?.NoiTruPhieuDieuTri?.GhiChuCanLamSang.Replace("\n","<br>"));
                        }
                        
                    }
                    if (!string.IsNullOrEmpty(ycdvkt?.YeuCauKhamBenh?.GhiChuCanLamSang))
                    {
                        ghiChuDVKTs.Add(ycdvkt?.YeuCauKhamBenh?.GhiChuCanLamSang);
                    }
                }

                var doiTuongs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                  .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                  .Select(a => a.MucHuong.ToString()).ToList();
                var dt = doiTuongs.Any() ? doiTuongs.First() : "";

                var soBHYTs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                  .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                  .Select(a => a.MaSoThe.ToString()).ToList();
                var soBHYT = soBHYTs.Any() ? soBHYTs.First() : "";

                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan?.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan?.BenhNhan != null ? yeuCauTiepNhan?.BenhNhan.MaBN : "",
                    HoTen = yeuCauTiepNhan?.HoTen ?? "",
                    GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                    Ngay = ngay,
                    Thang = thang,
                    Nam = nam,
                    DienThoai = yeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                    HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",

                    NoiYeuCau = tenPhong,

                     ChuanDoanSoBo = chanDoanSoBos.Where(item => item != null && item !="-").ToList().Distinct().Join("; "),
                    DienGiai = dienGiaiChanDoanSoBo.Where(item => item != null).ToList().Distinct().Join("; "),

                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,
                    PhieuThu = "DichVuKyThuat",
                    //BVHD-3800
                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : "",
                    // BVHD-3916
                    GhiChuCanLamSang =ghiChuDVKTs.Distinct().ToList().Join(", ")
                };
                if (isHave)
                {
                    var result1 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result1.Body, data);
                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                    {
                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                        content = content.Replace(tmpKB, tampKB);
                        content += "<div class=\"pagebreak\"> </div>";
                    }
                }

                if (data.PhieuThu == "DichVuKyThuat")
                {
                    var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
                    var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                    content = content.Replace(tmp, tamp);
                }
            }
            else
            {
                decimal tongCong = 0;
                int soLuong = 0;
                // BVHD-3939 // == 1 
                var listDichVuIds = lst.Select(d => d.dichVuChiDinhId).ToList();
                var thanhTienDv = listDVKT.Where(d => listDichVuIds.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                    .Sum();

                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();

                var chanDoanSoBos = new List<string>();
                var dienGiaiChanDoanSoBo = new List<string>();
                foreach (var itx in lst)
                {
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                            && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {
                            var maHocHamVi = string.Empty;
                            var maHocHamViId = ycdvkt.NhanVienChiDinh?.HocHamHocViId;
                            if (maHocHamViId != null)
                            {
                                maHocHamVi = MaHocHamHocVi((long)maHocHamViId);
                            }

                            tenNguoiChiDinh = returnStringTen(maHocHamVi, "", ycdvkt.NhanVienChiDinh?.User?.HoTen);
                            var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();

                            dienGiaiChanDoanSoBo.Add(ycdvkt?.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);
                            dienGiaiChanDoanSoBo.Add(ycdvkt?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu);
                            if (ycdvkt?.NoiTruPhieuDieuTri?.ChanDoanChinhICD != null)
                            {
                                chanDoanSoBos.Add(ycdvkt?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + ycdvkt?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet);
                            }
                            if (ycdvkt?.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                            {
                                chanDoanSoBos.Add(ycdvkt?.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt?.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet);
                            }

                            ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                            thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                            nam = ycdvkt.ThoiDiemDangKy.Year.ToString();

                            isHave = true;
                            if (indexDVKT == 1)
                            {
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                            }

                            tenNguoiChiDinh = ycdvkt.NhanVienChiDinh.User.HoTen;

                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            indexDVKT++;
                            nhomDichVu = "";
                            soLuong += ycdvkt.SoLan;
                            // BVHD-3916
                            // nội trú
                            if (ycdvkt.NoiTruPhieuDieuTriId != null)
                            {
                                if (!string.IsNullOrWhiteSpace(ycdvkt?.NoiTruPhieuDieuTri?.GhiChuCanLamSang))
                                {
                                    ghiChuDVKTs.Add(ycdvkt?.NoiTruPhieuDieuTri?.GhiChuCanLamSang.Replace("\n", "<br>"));
                                }

                            }
                            // ngoại trú
                            if (!string.IsNullOrEmpty(ycdvkt?.YeuCauKhamBenh?.GhiChuCanLamSang))
                            {
                                ghiChuDVKTs.Add(ycdvkt?.YeuCauKhamBenh?.GhiChuCanLamSang);
                            }
                        }


                    }

                }

                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var doiTuongs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                    .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                    .Select(a => a.MucHuong.ToString()).ToList();
                var dt = doiTuongs.Any() ? doiTuongs.First() : "";

                var soBHYTs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                  .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                  .Select(a => a.MaSoThe.ToString()).ToList();
                var soBHYT = soBHYTs.Any() ? soBHYTs.First() : "";

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan?.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan?.BenhNhan != null ? yeuCauTiepNhan?.BenhNhan.MaBN : "",
                    HoTen = yeuCauTiepNhan?.HoTen ?? "",
                    GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                    Ngay = ngay,
                    Thang = thang,
                    Nam = nam,
                    DienThoai = yeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                    HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",

                    NoiYeuCau =  tenPhong,

                     ChuanDoanSoBo = chanDoanSoBos.Where(item => item != null && item !="-").ToList().Distinct().Join("; "),
                    DienGiai = dienGiaiChanDoanSoBo.Where(item => item != null).ToList().Distinct().Join("; "),

                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,
                    PhieuThu = "DichVuKyThuat",
                    //BVHD-3800
                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : "",
                    // BVHD-3916
                    GhiChuCanLamSang = ghiChuDVKTs.Distinct().ToList().Join(", ")
                };
                if (isHave)
                {
                    var result1 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result1.Body, data);
                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                    {
                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                        content = content.Replace(tmpKB, tampKB);
                        content += "<div class=\"pagebreak\"> </div>";
                    }
                }
                if (data.PhieuThu == "DichVuKyThuat")
                {
                    var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
                    var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                    content = content.Replace(tmp, tamp);
                }
            }
            return content;
        }


        private string AddTungPhieuKhamBenhTheoNguoiChiDinh(YeuCauTiepNhan yeuCauTiepNhan, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
          List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
          List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string hostingName, bool? IsFromPhieuDieuTri, long? PhieuDieuTriId)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.ChanDoanSoBoICD)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauTiepNhan)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //   .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)
            //   .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //   .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //   .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //   .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //   .Include(p => p.BenhNhan)
            //   .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //   .Include(cc => cc.PhuongXa)
            //   .Include(cc => cc.QuanHuyen)
            //   .Include(cc => cc.TinhThanh)
            //   .Include(cc => cc.NoiTruBenhAn).ThenInclude(pp => pp.NoiTruPhieuDieuTris)
            //   .Include(cc => cc.YeuCauTiepNhanTheBHYTs)
            //   .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();


            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            string tenNhanVienChiDinh = "";

            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            if (listDVK != null || listDVKT != null)
            {

                var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
                //htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>VP</th>";
                //htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>BHYT</th>";
                htmlDanhSachDichVu += "</tr>";
                var i = 1;
                // nếu nhom ch dinh = 1 , 2 ,3 ,4
                var results = listDichVuTheoNguoiChiDinh
                         .GroupBy(x => x.nhomChiDinhId)
                         .Select(grp => new
                         {
                             Id = grp.Key,
                             ListChiDinh = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == grp.Key).ToList()
                         })
                         .ToList();
                var listEnum = EnumHelper.GetListEnum<Enums.LoaiDichVuKyThuat>().Select(item => new LookupItemVo()
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();
                // kiemr tra phàn tử đầu tiên trong list nằm trong gói dịch vụ nào  in đó trước
                var listInThuTuGDV = listDichVuTheoNguoiChiDinh.First().nhomChiDinhId;

                if (listInThuTuGDV == (long)Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    if (listDichVuTheoNguoiChiDinh.Any(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat))
                    {
                        List<ListDichVuChiDinhTheoNguoiChiDinh> lstDichVuChidinh = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                        var count = 0;
                        foreach (var itemx in listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).ToList())
                        {
                            itemx.ThuTuIn = count + 1;
                            foreach (var itemdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan))
                            {
                                if (itemx.dichVuChiDinhId == itemdvkt.Id)
                                {
                                    if (itemdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                                    {
                                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                           && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                        {
                                            if (itemx.dichVuChiDinhId == ycdvkt.Id)
                                            {
                                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                                itemx.TenNhom = nhomDichVu;
                                            }

                                        }
                                        lstDichVuChidinh.Add(itemx);
                                    }
                                    if (itemdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                    {
                                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                          && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                        {
                                            if (itemx.dichVuChiDinhId == ycdvkt.Id)
                                            {
                                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                                itemx.TenNhom = nhomDichVu;
                                            }

                                        }
                                        lstDichVuChidinh.Add(itemx);
                                    }
                                    if (itemdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.Khac)
                                    {
                                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                            && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                        {
                                            if (itemx.dichVuChiDinhId == ycdvkt.Id)
                                            {
                                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                                itemx.TenNhom = nhomDichVu;
                                            }

                                        }
                                        lstDichVuChidinh.Add(itemx);
                                    }
                                    if (itemdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)
                                    {
                                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                            && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                        {
                                            if (itemx.dichVuChiDinhId == ycdvkt.Id)
                                            {
                                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                                itemx.TenNhom = nhomDichVu;
                                            }

                                        }
                                        lstDichVuChidinh.Add(itemx);
                                    }
                                    if (itemdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.TheoYeuCau)
                                    {
                                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                          && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                        {
                                            if (itemx.dichVuChiDinhId == ycdvkt.Id)
                                            {
                                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                                itemx.TenNhom = nhomDichVu;
                                            }

                                        }
                                        lstDichVuChidinh.Add(itemx);
                                    }

                                    if (itemdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                    {
                                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                           && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                        {
                                            if (itemx.dichVuChiDinhId == ycdvkt.Id)
                                            {
                                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                                itemx.TenNhom = nhomDichVu;
                                            }

                                        }
                                        lstDichVuChidinh.Add(itemx);
                                    }
                                    #region cập nhật dvkt suất  ăn và tiêm chủng 1/11/2021
                                    if (itemdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SuatAn)
                                    {
                                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                           && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                        {
                                            if (itemx.dichVuChiDinhId == ycdvkt.Id)
                                            {
                                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                                itemx.TenNhom = nhomDichVu;
                                            }

                                        }
                                        lstDichVuChidinh.Add(itemx);
                                    }

                                    if (itemdvkt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung)
                                    {
                                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                           && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                                        {
                                            if (itemx.dichVuChiDinhId == ycdvkt.Id)
                                            {
                                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                                itemx.TenNhom = nhomDichVu;
                                            }

                                        }
                                        lstDichVuChidinh.Add(itemx);
                                    }
                                    #endregion cập nhật dvkt suất  ăn và tiêm chủng 1/11/2021
                                }
                            }
                        }
                        foreach (var itemIn in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
                        {
                            if (itemIn.Count() == 1)
                            {
                                List<ListDichVuChiDinhTheoNguoiChiDinh> lstDichVuCungChidinhXN = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                                lstDichVuCungChidinhXN.AddRange(itemIn);
                                content = AddPhieuInKhamBenhDichVuChiDinhTheoNguoiChiDinh(content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhan, hostingName, lstDichVuCungChidinhXN,IsFromPhieuDieuTri,PhieuDieuTriId);
                            }
                            else if (itemIn.Count() > 1)
                            {
                                List<ListDichVuChiDinhTheoNguoiChiDinh> lstDichVuCungChidinhXN = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                                lstDichVuCungChidinhXN.AddRange(itemIn);
                                content = AddPhieuInKhamBenhDichVuChiDinhTheoNguoiChiDinh(content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhan, hostingName, lstDichVuCungChidinhXN, IsFromPhieuDieuTri, PhieuDieuTriId);
                            }
                        }
                    }
                }
            }
            return content;
        }
        private string InChiDinhInTungPhieuTatCa(long yeuCauTiepNhanId, List<ListDichVuChiDinh> lst, string hostingName, bool? IsFromPhieuDieuTri, long? PhieuDieuTriId)
        {
            string content = "";

            var listSarsCov2CauHinh = GetListSarsCauHinh();
            //KieuInChung => in 1. In Theo dịch vụ chỉ định (cùng người chỉ định dịch vụ) 2. In theo số thứ tự (cùng người chỉ định dịch vụ)
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                     //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                     //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                     //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.ChanDoanSoBoICD)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
                     .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)
                     //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p=>p.YeuCauTiepNhan)

                     .Include(p => p.NguoiLienHeQuanHeNhanThan)
                     //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                     //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                     //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
                     //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
                     //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                     //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
                     //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                     .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
                     .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

                     //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
                     //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
                     //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                     //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                     //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc).ThenInclude(p => p.KhoaPhong)
                     //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

                     //.Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
                     //.Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                     //.Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                     //.Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu).ThenInclude(p => p.KhoaPhong)
                     //.Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)

                     .Include(p => p.BenhNhan)
                     .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                     .Include(cc => cc.PhuongXa)
                     .Include(cc => cc.QuanHuyen)
                     .Include(cc => cc.TinhThanh)
                     .Include(cc => cc.NoiTruBenhAn)
                     .Include(cc => cc.YeuCauTiepNhanTheBHYTs)
                     .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && !listSarsCov2CauHinh.Contains(s.DichVuKyThuatBenhVienId)).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1
            var chanDoanSoBos = new List<string>();

            var listInDichVuKyThuat = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat>();
            var listTheoNguoiChiDinh = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
            var lstDVKT = lst.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat); // lấy ra những item dịch vụ kỹ thuật

            foreach (var itx in lstDVKT)
            {
                foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                {
                    if (itx.dichVuChiDinhId == ycdvkt.Id)
                    {
                        var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                        objNguoiChidinh.dichVuChiDinhId = itx.dichVuChiDinhId;
                        objNguoiChidinh.nhomChiDinhId = itx.nhomChiDinhId;
                        objNguoiChidinh.TenNhom = itx.TenNhom;
                        objNguoiChidinh.ThuTuIn = itx.ThuTuIn;
                        objNguoiChidinh.NhanVienChiDinhId = ycdvkt.NhanVienChiDinhId;
                        objNguoiChidinh.ThoiDiemChiDinh = ycdvkt.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru != null ? new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0)
                                                                                       : new DateTime(ycdvkt.NoiTruPhieuDieuTri.NgayDieuTri.Year, ycdvkt.NoiTruPhieuDieuTri.NgayDieuTri.Month, ycdvkt.NoiTruPhieuDieuTri.NgayDieuTri.Day, 0, 0, 0);

                        listTheoNguoiChiDinh.Add(objNguoiChidinh);
                    }

                }
            }

            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();

            foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
            {
                var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                content = AddTungPhieuKhamBenhTheoNguoiChiDinh(yeuCauTiepNhan, listCanIn, listDVK, listDVKT, content, hostingName, IsFromPhieuDieuTri, PhieuDieuTriId);
            }
            return content;
        }


        public bool HuyYeuCauChayLaiXetNghiemTheoNhomDichVu(long phienXetNghiemId, long nhomDichVuBenhVienId)
        {
            var yeuCauChayLaiXetNghiemChuaDuyet = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(p => p.PhienXetNghiemId == phienXetNghiemId &&
                                                                                                               p.NhomDichVuBenhVienId == nhomDichVuBenhVienId &&
                                                                                                               p.DuocDuyet == null);
            if (yeuCauChayLaiXetNghiemChuaDuyet.Any())
            {
                _yeuCauChayLaiXetNghiemRepository.Delete(yeuCauChayLaiXetNghiemChuaDuyet);
                return true;
            }

            return false;
        }

        public bool IsGoiChayLaiXetNghiem(long phienXetNghiemId, long nhomDichVuBenhVienId)
        {
            return _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(p => p.PhienXetNghiemId == phienXetNghiemId &&
                                                                                p.NhomDichVuBenhVienId == nhomDichVuBenhVienId &&
                                                                                p.DuocDuyet == null)
                                                                    .Any();
        }

        public async Task<List<LichSuYeuCauChayLai>> LichSuYeuCauChayLaiXetNghiem(LichSuChayLaiXetNghiemVo lichSuChayLaiXetNghiem)
        {
            var query = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(p => p.NhomDichVuBenhVienId == lichSuChayLaiXetNghiem.NhomDichVuBenhVienId &&
                                                                                           lichSuChayLaiXetNghiem.LichSuPhienXetNghiemIds.Contains(p.PhienXetNghiemId))
                                                                               .Select(p => new LichSuYeuCauChayLai
                                                                               {
                                                                                   NguoiYeuCau = p.NhanVienYeuCau.User.HoTen,
                                                                                   NgayYeuCau = p.NgayYeuCau.ApplyFormatDateTimeSACH(),
                                                                                   LyDoYeuCau = p.LyDoYeuCau,
                                                                                   NguoiTuChoi = p.NhanVienDuyetId != null ? p.NhanVienDuyet.User.HoTen : string.Empty,
                                                                                   NgayTuChoi = p.NgayDuyet != null ? p.NgayDuyet.Value.ApplyFormatDateTimeSACH() : string.Empty,
                                                                                   LyDoTuChoi = p.LyDoKhongDuyet,
                                                                                   TrangThai = p.DuocDuyet
                                                                               })
                                                                               .ToList();

            return query;
        }
        #region format tên người chỉ định 
        private string returnStringTen(string maHocHamHocVi, string maNhomChucDanh, string ten)
        {
            var stringTen = string.Empty;
            //chỗ này show theo format: Mã học vị học hàm + dấu cách + Tên bác sĩ
            if (!string.IsNullOrEmpty(maHocHamHocVi))
            {
                stringTen = maHocHamHocVi + " " + ten;
            }
            if (string.IsNullOrEmpty(maHocHamHocVi))
            {
                stringTen = ten;
            }
            return stringTen;
        }
        private string MaHocHamHocVi(long id)
        {
            var maHocHamVi = string.Empty;
            maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == id).Select(d => d.Ma).FirstOrDefault();
            return maHocHamVi;
        }
        #endregion
        private List<long> GetListSarsCauHinh()
        {
            var lstDichVuSarCoVs = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhTiepNhan.DichVuTestSarsCovid")
                .Select(d => d.Value).FirstOrDefault();

            var json = JsonConvert.DeserializeObject<List<DichVuKyThuatBenhVienIdsSarsCoV>>(lstDichVuSarCoVs);
            var dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham = new DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham();
            dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.Ids = json.Select(d => d.DichVuKyThuatBenhVienId).ToList();
            return dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.Ids;
        }

        #region BVHD-3860
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.Table.Where(x => x.Id == yeuCauTiepNhanId)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhLichSuTrangThais)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauDuocPhamBenhViens)
                .Include(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                .Include(x => x.DoiTuongUuDai).ThenInclude(x => x.DoiTuongUuDaiDichVuKyThuatBenhViens)
                .Include(x => x.DoiTuongUuDai).ThenInclude(x => x.DoiTuongUuDaiDichVuKhamBenhBenhViens)

                // KSK
                .Include(x => x.HopDongKhamSucKhoeNhanVien)

                //PTTT
                .Include(x => x.NoiTruBenhAn)
                .First();
            return yeuCauTiepNhan;
        }


        #endregion
    
        #region BVHD-3575
        private string InChiDinhDichVuKham(InChiDinhDichVuKhamNoiTruGridVo vo)
        {
            var content = string.Empty;

            var kiemTraYeuCauTiepNhanNoiTruCoDichVuKham = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == vo.YeuCauTiepNhanId).Where(d => d.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
                    .Select(d => d.YeuCauTiepNhanNgoaiTruCanQuyetToanId);

            var yeuCauKhamBenhCanInIds = vo.DichVuKhamNoiTruCanIns.Select(d => d.dichVuChiDinhId).ToList();

            var yeuCauKhamBenhInFoTheoYeuCauTiepNhanNgoaiTruCanQuyetToanIds = new List<ListDichVuChiDinhInffo>();
            long yctnQuyetToanId = 0;
            if (kiemTraYeuCauTiepNhanNoiTruCoDichVuKham != null)
            {
                yctnQuyetToanId = (long)kiemTraYeuCauTiepNhanNoiTruCoDichVuKham.First();
                //yeuCauKhamBenhInFoTheoYeuCauTiepNhanNgoaiTruCanQuyetToanIds = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == kiemTraYeuCauTiepNhanNoiTruCoDichVuKham.First())
                //                                                           .SelectMany(d => d.YeuCauKhamBenhs)
                //                                                           .Select(d => new ListDichVuChiDinhInffo()
                //                                                           {
                //                                                               DichVuChiDinhId = d.Id,
                //                                                               NhanVienChiDinhId = d.NhanVienChiDinhId,
                //                                                               NhomChiDinhId = EnumNhomGoiDichVu.DichVuKhamBenh,
                //                                                               ThoiDiemDangKy = d.ThoiDiemDangKy
                //                                                           }).ToList();
                // .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruId
                //&& x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham

                yeuCauKhamBenhInFoTheoYeuCauTiepNhanNgoaiTruCanQuyetToanIds = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(d => d.YeuCauTiepNhanId == yctnQuyetToanId &&
                                d.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    )
                                                                           .Select(d => new ListDichVuChiDinhInffo()
                                                                           {
                                                                               DichVuChiDinhId = d.Id,
                                                                               NhanVienChiDinhId = d.NhanVienChiDinhId,
                                                                               NhomChiDinhId = EnumNhomGoiDichVu.DichVuKhamBenh,
                                                                               ThoiDiemDangKy = d.ThoiDiemDangKy
                                                                           }).ToList();
            }

            if (yeuCauKhamBenhCanInIds.Count() != 0)
            {
                yeuCauKhamBenhInFoTheoYeuCauTiepNhanNgoaiTruCanQuyetToanIds = yeuCauKhamBenhInFoTheoYeuCauTiepNhanNgoaiTruCanQuyetToanIds.Where(d => yeuCauKhamBenhCanInIds.Contains(d.DichVuChiDinhId)).ToList();
            }

            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

          
            listDVK.AddRange(_yeuCauKhamBenhRepository.TableNoTracking.Include(p => p.ChanDoanSoBoICD)
                        .Include(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                        .Include(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                        .Include(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
                        .Include(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)
                    .Where(d => d.YeuCauTiepNhanId == yctnQuyetToanId &&
                                d.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    ).ToList());
            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = yeuCauKhamBenhInFoTheoYeuCauTiepNhanNgoaiTruCanQuyetToanIds.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();

            if (vo.inChungChiDinh == 0)
            {
                foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                {
                    var listCanIn = new List<ListDichVuChiDinhInffo>();
                    listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                    var newModelIn = new AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru
                    {
                        YeuCauTiepNhanId = (long)kiemTraYeuCauTiepNhanNoiTruCoDichVuKham.First(),
                        ListDVK = listDVK,
                        Content = content,
                        HostingName = vo.HosTingName,
                        ListDichVuTheoNguoiChiDinh = listCanIn
                    };

                    content = AddTungPhieuKhamBenhTheoNguoiChiDinh(newModelIn);
                }
            }
            else
            {
               
                if (vo.KieuIn == true)
                {
                    // lấy từng nhóm listInChiDinhTheoNguoiChiDinh vào 1 mảng list cần in 
                    foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                    {
                        var listCanIn = new List<ListDichVuChiDinhInffo>();
                        listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);

                        var newModelIn = new AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru
                        {
                            YeuCauTiepNhanId = (long)kiemTraYeuCauTiepNhanNoiTruCoDichVuKham.First(),
                            ListDVK = listDVK,
                            Content = content,
                            HostingName = vo.HosTingName,
                            ListDichVuTheoNguoiChiDinh = listCanIn
                        };
                        content = AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru(newModelIn);
                    }
                }
                else
                {   /// in theo STT và Người chỉ định

                    foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                    {
                        var listCanIn = new List<ListDichVuChiDinhInffo>();
                        listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                        var newModelIn = new AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru
                        {
                            YeuCauTiepNhanId = (long)kiemTraYeuCauTiepNhanNoiTruCoDichVuKham.First(),
                            ListDVK = listDVK,
                            Content = content,
                            HostingName = vo.HosTingName,
                            ListDichVuTheoNguoiChiDinh = listCanIn
                        };
                        content = AddTungPhieuTheoNguoiChiDinhVaTheoSTTDichVuKhamNoiTru(newModelIn);
                    }
                }
            }

           


            return content;
        }
        private string AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru(AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru vo)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Include(p => p.NguoiLienHeQuanHeNhanThan)

                        //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                        //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                        //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                        //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
                        //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

                        .Include(p => p.BenhNhan)
                        .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                        .Include(cc => cc.PhuongXa)
                        .Include(cc => cc.QuanHuyen)
                        .Include(cc => cc.TinhThanh)
                      // BVHD - 3800
                      .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                        .Where(p => p.Id == vo.YeuCauTiepNhanId).FirstOrDefault();

            // chẩn đoán sơ 
            var chanDoanSoBos = new List<string>();
            // diễn giải
            var dienGiais = new List<string>();


            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;

            vo.Content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            // từng item phiếu in theo người  chỉ định => tất cả dịch vụ khám bệnh và dịch vụ kỹ thuật đều cùng 1  người chỉ định
            var nhanVienChiDinh = "";
            // tên người chỉ định theo phiếu in 
            string ngay = "";
            string thang = "";
            string nam = "";

            var isHave = false;
            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            var i = 1;

            //DỊCH VỤ KHÁM BỆNH

            var lstDVKB = vo.ListDichVuTheoNguoiChiDinh;

            int indexDVKB = 1;
            var listInDichVuKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
            foreach (var itx in lstDVKB)
            {
                var lstYeuCauKhamBenhChiDinh = vo.ListDVK.Where(s => s.Id == itx.DichVuChiDinhId
                 && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                  ).OrderBy(x => x.CreatedOn); // to do nam ho;

                if (lstYeuCauKhamBenhChiDinh != null)
                {
                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                    {
                        if (itx.DichVuChiDinhId == yckb.Id)
                        {
                            listInDichVuKhamBenh.Add(yckb);
                        }
                    }
                }
            }
            decimal tongCong = 0;
            int soLuong = 0;
            if (listInDichVuKhamBenh.ToList().Count() != 0)
            {
                
                // BVHD-3939 // == 1 
                var thanhTienDv = listInDichVuKhamBenh
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                    .Sum();
                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();


                foreach (var yckb in listInDichVuKhamBenh)
                {
                    ngay = yckb.ThoiDiemDangKy.Day.ToString();
                    thang = yckb.ThoiDiemDangKy.Month.ToString();
                    nam = yckb.ThoiDiemDangKy.Year.ToString();
                    if (indexDVKB == 1)
                    {
                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                        htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                        htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                        htmlDanhSachDichVu += " </tr>";
                    }
                    var maHocHamVi = string.Empty;
                    var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                    if (maHocHamViId != null)
                    {
                        maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                    }

                    if (yckb.ChanDoanSoBoICD != null)
                    {
                        dienGiais.Add(yckb.ChanDoanSoBoGhiChu);

                        chanDoanSoBos.Add(yckb.ChanDoanSoBoICD != null ? yckb.ChanDoanSoBoICD?.Ma + "-" + yckb.ChanDoanSoBoICD?.TenTiengViet : "");
                    }

                    nhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>"; // so lan kham
                    htmlDanhSachDichVu += " </tr>";
                    i++;
                    indexDVKB++;
                    soLuong++;
                }
            }

            // BVHD-3939- page -total
            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
            // BVHD-3939 - số lượng
            htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
            htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

            htmlDanhSachDichVu += " </tr>";
            // end BVHD-3939

            var data = new
            {
                LogoUrl = vo.HostingName + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan.BenhNhan != null ? yeuCauTiepNhan.BenhNhan.MaBN : "",
                HoTen = yeuCauTiepNhan.HoTen ?? "",
                GioiTinhString = yeuCauTiepNhan.GioiTinh.GetDescription(),
                NamSinh = yeuCauTiepNhan.NamSinh ?? null,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                Ngay = ngay,
                Thang = thang,
                Nam = nam,
                DienThoai = yeuCauTiepNhan.SoDienThoai,
                DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                ////Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                ////NowTime = DateTime.Now.ApplyFormatTime(),,
                NoiYeuCau = tenPhong,
                ChuanDoanSoBo = chanDoanSoBos.Where(d=>d != null).Join(";"), // fist đâu tiền dịch vụ != hủy và gói khám != null
                DienGiai = dienGiais.Where(d => d != null).Join(";"),
                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = nhanVienChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.Ten,
                GhiChuCanLamSang = "",
                NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                //BVHD-3800
                CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
            };

            var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));

            vo.Content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
            
            if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
            {
                var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                var tmpKB = "<tr id=\"NguoiGiamHo\">";
                var test = vo.Content.IndexOf(tmp);
                vo.Content = vo.Content.Replace(tmpKB, tampKB);
            }

            htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            
            return vo.Content;

        }
        // in chung tất cả dịch dụ theo stt theo người chỉ định *
        private string AddTungPhieuTheoNguoiChiDinhVaTheoSTTDichVuKhamNoiTru(AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru vo)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                     

                        .Include(p => p.NguoiLienHeQuanHeNhanThan)
                   


                        //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                        //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                        //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                        //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
                        //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

                        .Include(p => p.BenhNhan)
                        .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                        .Include(cc => cc.PhuongXa)
                        .Include(cc => cc.QuanHuyen)
                        .Include(cc => cc.TinhThanh)
                      // BVHD - 3800
                      .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                        .Where(p => p.Id == vo.YeuCauTiepNhanId).FirstOrDefault();

            // chẩn đoán sơ 
            var chanDoanSoBos = new List<string>();
            // diễn giải
            var dienGiais = new List<string>();

            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            vo.Content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            var i = 1;
            List<ListDichVuChiDinh> lstDichVuChidinhTheoSoThuTu = new List<ListDichVuChiDinh>();

            var lstDVKB = vo.ListDVK.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).OrderBy(x => x.CreatedOn);
       
            string ngay = "";
            string thang = "";
            string nam = "";
            var tenNhanVienChiDinh = "";
            if (vo.ListDichVuTheoNguoiChiDinh.Count() > 0)
            {
                decimal tongCong = 0;
                int soLuong = 0;
                // BVHD-3939 // == 1 
                var listDichVuIds = vo.ListDichVuTheoNguoiChiDinh.Select(d => d.DichVuChiDinhId).ToList();
                var thanhTienDv = lstDVKB.Where(d => listDichVuIds.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                    .Sum();
                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();

           
                foreach (var Itemx in vo.ListDichVuTheoNguoiChiDinh)
                {

                    if (lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Any())
                    {
                        ngay = lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(s => s.ThoiDiemDangKy.Day.ToString()).First();
                        thang = lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(s => s.ThoiDiemDangKy.Month.ToString()).First();
                        nam = lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(s => s.ThoiDiemDangKy.Year.ToString()).First();

                        var maHocHamVi = string.Empty;
                        var maHocHamViId = lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId);
                        if (maHocHamViId.Any(d => d != null))
                        {
                            maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId.First()).Select(d => d.Ma).FirstOrDefault();
                        }

                        tenNhanVienChiDinh = returnStringTen(maHocHamVi,
                                                             "",
                                                             lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).FirstOrDefault());

                        if (lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(s => s.ChanDoanSoBoICD).Count() != 0)
                        {
                            chanDoanSoBos.Add(lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(s => s.ChanDoanSoBoICD?.Ma + "-" + s.ChanDoanSoBoICD?.TenTiengViet).First());
                        }

                        if (lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(s => s.ChanDoanSoBoGhiChu).Count() != 0)
                        {
                            chanDoanSoBos.Add(lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).Select(s => s.ChanDoanSoBoGhiChu).First());
                        }

                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).First().TenDichVu + "</td>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).First().NoiDangKy != null ? lstDVKB.Where(p => p.Id == Itemx.DichVuChiDinhId).First().NoiDangKy?.Ten : "") + "</td>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>"; 
                        htmlDanhSachDichVu += " </tr>";
                        i++;
                        soLuong++;
                    }
                }

                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939
            }


            var data = new
            {
                LogoUrl = vo.HostingName + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan.BenhNhan != null ? yeuCauTiepNhan.BenhNhan.MaBN : "",
                HoTen = yeuCauTiepNhan.HoTen ?? "",
                GioiTinhString = yeuCauTiepNhan.GioiTinh.GetDescription(),
                NamSinh = yeuCauTiepNhan.NamSinh ?? null,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                Ngay = ngay,
                Thang = thang,
                Nam = nam,
                DienThoai = yeuCauTiepNhan.SoDienThoai,
                DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                //Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                //NowTime = DateTime.Now.ApplyFormatTime(),,
                NoiYeuCau = tenPhong,
                ChuanDoanSoBo = chanDoanSoBos.Where(d=> d!= null).Join(";"), // cấu hình
                DienGiai = dienGiais.Where(d => d != null).Join(";"),
                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = tenNhanVienChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.Ten,
                GhiChuCanLamSang = "",
                NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                //BVHD-3800
                CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
            };
            var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
            vo.Content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
            if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
            {
                var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                var tmpKB = "<tr id=\"NguoiGiamHo\">";
                var test = vo.Content.IndexOf(tmp);
                vo.Content = vo.Content.Replace(tmpKB, tampKB);
            }

            htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
           
            return vo.Content;

        }

        private string AddTungPhieuKhamBenhTheoNguoiChiDinh(AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru vo)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                    

                      .Include(p => p.NguoiLienHeQuanHeNhanThan)

                      
                      //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                      //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                      //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                      //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
                      //.Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)


                      .Include(p => p.BenhNhan)
                      .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                      .Include(cc => cc.PhuongXa)
                      .Include(cc => cc.QuanHuyen)
                      .Include(cc => cc.TinhThanh)
                      // BVHD - 3800
                      .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                      .Where(p => p.Id == vo.YeuCauTiepNhanId).FirstOrDefault();

            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;


            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            string tenNhanVienChiDinh = "";

            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            if (vo.ListDVK != null )
            {

                var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
                htmlDanhSachDichVu += "</tr>";
                var i = 1;
               
                string ngay = "";
                string thang = "";
                string nam = "";

                var lstDVKB = vo.ListDichVuTheoNguoiChiDinh;
                if (lstDVKB.Any())
                {
                   
                   

                    if (vo.ListDichVuTheoNguoiChiDinh.Count() == 1)
                    {
                        decimal tongCong = 0;
                        int soLuong = 0;
                        var dienGiaiChanDoanSoBo = new List<string>();
                        var chanDoanSoBos = new List<string>();

                        var lstYeuCauKhamBenhChiDinh = vo.ListDVK.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && x.Id == vo.ListDichVuTheoNguoiChiDinh.First().DichVuChiDinhId).OrderBy(x => x.CreatedOn); // to do nam ho;

                        // BVHD-3939 // == 1 
                        var thanhTienDv = lstYeuCauKhamBenhChiDinh
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                            .Sum();
                        CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                        tongCong += thanhTienDv.GetValueOrDefault();

                        if (lstYeuCauKhamBenhChiDinh != null)
                        {
                            int indexDVKT = 1;
                            foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                            {
                                if (yckb.ChanDoanSoBoICD != null)
                                {
                                    chanDoanSoBos.Add(yckb.ChanDoanSoBoICD?.Ma + "-" + yckb.ChanDoanSoBoICD?.TenTiengViet);
                                }

                                dienGiaiChanDoanSoBo.Add(yckb.ChanDoanSoBoGhiChu);

                                ngay = yckb.ThoiDiemDangKy.Day.ToString();
                                thang = yckb.ThoiDiemDangKy.Month.ToString();
                                nam = yckb.ThoiDiemDangKy.Year.ToString();

                                var maHocHamVi = string.Empty;
                                var maHocHamViId = yckb?.NhanVienChiDinh?.HocHamHocViId;
                                if (maHocHamViId != null)
                                {
                                    maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                                }

                                tenNhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                                htmlDanhSachDichVu += " </tr>";

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                soLuong++;
                            }

                            // BVHD-3939- page -total
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                            // BVHD-3939 - số lượng
                            htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                            htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

                            htmlDanhSachDichVu += " </tr>";
                            // end BVHD-3939

                            var data = new
                            {
                                LogoUrl = vo.HostingName + "/assets/img/logo-bacha-full.png",
                                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBN = yeuCauTiepNhan.BenhNhan != null ? yeuCauTiepNhan.BenhNhan.MaBN : "",
                                HoTen = yeuCauTiepNhan.HoTen ?? "",
                                GioiTinhString = yeuCauTiepNhan.GioiTinh.GetDescription(),
                                NamSinh = yeuCauTiepNhan.NamSinh ?? null,
                                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                                Ngay = ngay,
                                Thang = thang,
                                Nam = nam,
                                DienThoai = yeuCauTiepNhan.SoDienThoai,
                                DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                                SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                                HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                                NoiYeuCau = tenPhong,

                                ChuanDoanSoBo = chanDoanSoBos.Where(s => s != null && s != "" && s != "-").Distinct().ToList().Join(";"), // khám bệnh 
                                DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"),

                                DanhSachDichVu = htmlDanhSachDichVu,
                                NguoiChiDinh = tenNhanVienChiDinh,
                                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                TenQuanHeThanNhan = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.Ten,
                                PhieuThu = "YeuCauKhamBenh",
                                NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                //BVHD-3800
                                CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
                            };


                            if (data.PhieuThu == "YeuCauKhamBenh")
                            {
                                var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                vo.Content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";

                                if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                {
                                    var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                    var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                    vo.Content = vo.Content.Replace(tmpKB, tampKB);
                                }
                                var test = vo.Content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                vo.Content = vo.Content.Replace(tmp, tamp);
                            }

                            htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
                            htmlDanhSachDichVu += "</tr>";
                            i = 1;
                        }
                    }
                    else
                    {
                        decimal tongCong = 0;
                        int soLuong = 0;

                        foreach (var itx in lstDVKB)
                        {
                            var lstYeuCauKhamBenhChiDinh = vo.ListDVK
                                .Where(x =>
                                x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && lstDVKB.Any(y => y.DichVuChiDinhId == x.Id)).OrderBy(x => x.CreatedOn); // to do nam ho;

                            var dienGiaiChanDoanSoBo = new List<string>();
                            var chanDoanSoBos = new List<string>();

                           
                            // BVHD-3939 // == 1 
                            
                            var thanhTienDv = lstYeuCauKhamBenhChiDinh
                                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                .Sum();
                            CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                            var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                            tongCong += thanhTienDv.GetValueOrDefault();

                            if (lstYeuCauKhamBenhChiDinh != null)
                            {
                                int indexDVKT = 1;
                                foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                                {
                                    if (itx.DichVuChiDinhId == yckb.Id)
                                    {
                                        var maHocHamVi = string.Empty;
                                        var maHocHamViId = yckb?.NhanVienChiDinh?.HocHamHocViId;
                                        if (maHocHamViId != null)
                                        {
                                            maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                                        }

                                        tenNhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                                        ngay = yckb.ThoiDiemDangKy.Day.ToString();
                                        thang = yckb.ThoiDiemDangKy.Month.ToString();
                                        nam = yckb.ThoiDiemDangKy.Year.ToString();

                                        if (yckb.ChanDoanSoBoICD != null)
                                        {
                                            chanDoanSoBos.Add(yckb.ChanDoanSoBoICD?.Ma + "-" + yckb.ChanDoanSoBoICD?.TenTiengViet);
                                        }

                                        dienGiaiChanDoanSoBo.Add(yckb.ChanDoanSoBoGhiChu);

                                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                        htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                                        htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienDv}</b></td>";
                                        htmlDanhSachDichVu += " </tr>";

                                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>"; 
                                        htmlDanhSachDichVu += " </tr>";
                                        i++;
                                        indexDVKT++;
                                        soLuong++;
                                    }
                                }
                            }

                            // BVHD-3939- page -total
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                            // BVHD-3939 - số lượng
                            htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                            htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

                            htmlDanhSachDichVu += " </tr>";
                            // end BVHD-3939

                            var data = new
                            {
                                LogoUrl = vo.HostingName + "/assets/img/logo-bacha-full.png",
                                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBN = yeuCauTiepNhan.BenhNhan != null ? yeuCauTiepNhan.BenhNhan.MaBN : "",
                                HoTen = yeuCauTiepNhan.HoTen ?? "",
                                GioiTinhString = yeuCauTiepNhan.GioiTinh.GetDescription(),
                                NamSinh = yeuCauTiepNhan.NamSinh ?? null,
                                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                                Ngay = ngay,
                                Thang = thang,
                                Nam = nam,
                                DienThoai = yeuCauTiepNhan.SoDienThoai,
                                DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                                SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                                HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                                NoiYeuCau = tenPhong,

                                ChuanDoanSoBo = chanDoanSoBos.Where(s => s != null && s != "" && s != "-").Distinct().ToList().Join(";"), // khám bệnh 
                                DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"),

                                DanhSachDichVu = htmlDanhSachDichVu,
                                NguoiChiDinh = tenNhanVienChiDinh,
                                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                TenQuanHeThanNhan = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.Ten,
                                PhieuThu = "YeuCauKhamBenh",
                                NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                //BVHD-3800
                                CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
                            };


                            if (data.PhieuThu == "YeuCauKhamBenh")
                            {
                                var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                vo.Content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
                                if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                {
                                    var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                    var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                    vo.Content = vo.Content.Replace(tmpKB, tampKB);
                                }
                                var test = vo.Content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                vo.Content = vo.Content.Replace(tmp, tamp);
                            }
                            htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
                            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
                            htmlDanhSachDichVu += "</tr>";
                            i = 1;
                        }

                    }
                }
            }
            return vo.Content;
        }
        #endregion
    }
}
