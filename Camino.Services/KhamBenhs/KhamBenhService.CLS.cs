using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.DichVuKhamBenh;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.VatTuBenhViens;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using static Camino.Core.Helpers.EnumHelper;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.CauHinh;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Localization;
using PhongKhamTemplateVo = Camino.Core.Domain.ValueObject.KhoaPhongNhanVien.PhongKhamTemplateVo;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using System.Globalization;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;

namespace Camino.Services.KhamBenhs
{
    public partial class KhamBenhService
    {
        #region Yêu Cầu Dịch Vụ Chỉ Định Khác

        public GridDataSource GetDataForGridChiTietAsync(QueryInfo queryInfo, long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenh)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)//?.ThenInclude(p => p.KhoaPhong)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiThucHien)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiThucHien)?.ThenInclude(p => p.User)

                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)

                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)

                 //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu)
                 .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)
                 .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            // setup data chp grip
            var reulst = SetDataGripViewYeuCauChiDinhKhac(yeuCauTiepNhan, yeuCauKhamBenhId);

            var queryIqueryable = reulst.AsQueryable();
            var queryTask = queryIqueryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = 0 };
        }
        public async Task<List<KhamBenhGoiDichVuGridVo>> GetDichVuKhacByTiepNhanBenhNhan(GridChiDinhDichVuQueryInfoVo queryInfo)
        {
            // todo:có cập nhật bỏ await
            var yeuCauTiepNhan = new YeuCauTiepNhan();
            if (queryInfo.IsKhamDoanTatCa == true)
            {
                yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuXetNghiem)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NoiThucHien)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomDichVuBenhVien)?.ThenInclude(p => p.NhomDichVuBenhVienCha)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.TaiKhoanBenhNhanChis)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                     .Where(p => p.Id == queryInfo.YeuCauTiepNhanId).FirstOrDefault();
            }
            else
            {
                yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenh)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiThucHien)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiThucHien)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiDangKy)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.TaiKhoanBenhNhanChis)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)

                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuXetNghiem)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)?.ThenInclude(p => p.NhomDichVuBenhVienCha)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.TaiKhoanBenhNhanChis)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                     // chỉ hiển thị dv khám và kỹ thuật
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.GiuongBenh)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)

                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)
                     .Where(p => p.Id == queryInfo.YeuCauTiepNhanId).FirstOrDefault();
            }




            // setup data chp grip
            if (yeuCauTiepNhan != null)
            {
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

                var dichVus = SetDataGripViewYeuCauChiDinhKhac(yeuCauTiepNhan, queryInfo.YeuCauKhamBenhId, queryInfo.NhomDichVuId, queryInfo.IsKhamDoanTatCa, goiDichVus);
                return dichVus;
            }

            return null;
        }
        private List<KhamBenhGoiDichVuGridVo> SetDataGripViewYeuCauChiDinhKhac(YeuCauTiepNhan yeuCauTiepNhan, long yeuCauKhamBenhId, int? nhomId = null, bool? isKhamDoanTatCa = false, List<YeuCauGoiDichVu> goiDichVus = null)
        {
            long userId = _userAgentHelper.GetCurrentUserId();

            // list chỉ định dịch vụ khám
            var lstYeuCauKhamBenhChiDinh = yeuCauTiepNhan.YeuCauKhamBenhs
                                            .Where(x => x.YeuCauKhamBenhTruocId == yeuCauKhamBenhId
                                                        //&& x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                                        // cập nhật cho phép hiện dịch vụ hủy khi hủy thanh toán
                                                        && (x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)) //BVHD-3284:  || !string.IsNullOrEmpty(x.LyDoHuyDichVu)

                                            //&& x.NhanVienChiDinhId == userId)
                                            .OrderBy(x => x.CreatedOn);

            // danh sách dịch vụ
            var lstDichVuKyThuat = new List<YeuCauDichVuKyThuat>();
            if (isKhamDoanTatCa == true)
            {
                lstDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats
                  .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) //BVHD-3284: || !string.IsNullOrEmpty(x.LyDoHuyDichVu)
                  .ToList();
            }
            else
            {
                // yêu cầu khám hiện tại
                var yeuCauKhamBenhHienTai = yeuCauTiepNhan.YeuCauKhamBenhs.First(x => x.Id == yeuCauKhamBenhId);
                lstDichVuKyThuat = yeuCauKhamBenhHienTai.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) //BVHD-3284: || !string.IsNullOrEmpty(x.LyDoHuyDichVu)
                    .ToList();
            }

            // get list nhóm dịch vụ trong gói
            var lstNhomDichVu = EnumHelper.GetListEnum<EnumNhomGoiDichVu>()
                .Select(item => new LookupItemVo()
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).OrderByDescending(x => nhomId == null || x.KeyId == nhomId).ThenBy(x => x.DisplayName).ToList();

            if (goiDichVus == null)
            {
                goiDichVus = new List<YeuCauGoiDichVu>();
            }

            var goiDichVuKhamBenh = new List<KhamBenhGoiDichVuGridVo>();
            var stt = 1;

            foreach (var item in lstNhomDichVu)
            {
                switch (item.KeyId)
                {
                    case (int)EnumNhomGoiDichVu.DichVuKhamBenh:
                        //var sttKB = 1;
                        goiDichVuKhamBenh.AddRange(lstYeuCauKhamBenhChiDinh.Select(p => new KhamBenhGoiDichVuGridVo
                        {
                            STT = stt++,
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
                            //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                            //ThanhTien = 0,
                            BHYTThanhToan = 0,
                            //BNThanhToan = 0,
                            NoiThucHien = p.NoiThucHien == null ? p.NoiDangKy?.Ten : p.NoiThucHien?.Ten,//p.NoiThucHien == null ? String.Format("{0} - {1}", p.NoiDangKy?.Ma, p.NoiDangKy?.Ten) : String.Format("{0} - {1}", p.NoiThucHien.Ma, p.NoiThucHien.Ten),
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
                            //TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper() + (p.MienGiamChiPhis.Any() ? " (Khuyến mãi)" : "") : null,
                            TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                                "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
                                : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                    p.MienGiamChiPhis
                                        .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                        .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
                                        .First() : null),
                            ThoiGianChiDinh = p.ThoiDiemChiDinh,
                            ThanhTien = p.KhongTinhPhi == true ? 0 : (p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia),
                            IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                            LyDoHuyDichVu = p.LyDoHuyDichVu,
                            NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,

                            // gói marketing
                            CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                            CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                            CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                        }));
                        break;
                    case (int)EnumNhomGoiDichVu.DichVuKyThuat:
                        //var sttKT = 1;
                        var lstSortNhomDichVuKyThuat = lstDichVuKyThuat.OrderBy(x => x.CreatedOn)
                            .Select(x => x.NhomDichVuBenhVienId).Distinct().ToList();
                        goiDichVuKhamBenh.AddRange(
                            //yeuCauKhamBenhHienTai.YeuCauDichVuKyThuats?
                            //.Where(p => //p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) //p.NhanVienChiDinhId == userId && 
                            //    // cập nhật cho phép hiện dịch vụ hủy khi hủy thanh toán
                            //    p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy || !string.IsNullOrEmpty(p.LyDoHuyDichVu))
                            lstDichVuKyThuat
                            //.OrderBy(x => x.CreatedOn)

                            // cập nhật 18/05/2021: sắp xếp lại các dịch vụ xét nghiệm theo số thứ tự
                            .OrderBy(x => lstSortNhomDichVuKyThuat.IndexOf(x.NhomDichVuBenhVienId))
                            .ThenBy(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ? (x.DichVuKyThuatBenhVien.DichVuXetNghiem?.SoThuTu ?? (x.DichVuKyThuatBenhVien.DichVuXetNghiemId ?? 0)) : 0)
                            .ThenBy(x => x.CreatedOn)
                            .Select(p => new KhamBenhGoiDichVuGridVo
                            {
                                STT = stt++,
                                Id = p.Id,
                                Nhom = (string.IsNullOrEmpty(p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten) ? "" : p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten + " - ") + p.NhomDichVuBenhVien.Ten, //EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                                NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                                LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien?.Id,
                                NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                                Ma = p.DichVuKyThuatBenhVien?.Ma,
                                MaGiaDichVu = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaGia,
                                MaTT37 = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.Ma4350,
                                TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                                TenTT43 = p.TenGiaDichVu,
                                TenLoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                                LoaiGia = p.NhomGiaDichVuKyThuatBenhVienId,
                                DonGia = p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia,
                                //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                                //ThanhTien = 0,
                                BHYTThanhToan = 0,
                                //BNThanhToan = 0,
                                NoiThucHien = p.NoiThucHien?.Ten,//String.Format("{0} - {1}", p.NoiThucHien?.Ma, p.NoiThucHien?.Ten),
                                NoiThucHienId = p.NoiThucHienId ?? 0,
                                TenNguoiThucHien = p.NhanVienThucHien?.User.HoTen,
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
                                KhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT != null && p.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true,
                                LyDoKhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT?.LyDoKhongThucHien,
                                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                                TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                                    "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
                                    : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                        p.MienGiamChiPhis
                                            .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                            .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
                                            .First() : null),
                                IsDichVuXetNghiem = p.NhomDichVuBenhVien.Ma == "XN" || p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ma == "XN",
                                BenhPhamXetNghiem = p.BenhPhamXetNghiem,
                                ThoiGianChiDinh = p.ThoiDiemChiDinh,
                                KhongTinhPhi = p.KhongTinhPhi,
                                ThanhTien = p.KhongTinhPhi == true ? 0 : ((p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia) * (decimal)p.SoLan),
                                IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                                LyDoHuyDichVu = p.LyDoHuyDichVu,
                                NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,

                                // gói marketing
                                CoDichVuNayTrongGoi = !goiDichVus.Any() ? false : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                CoDichVuNayTrongGoiKhuyenMai = !goiDichVus.Any() ? false : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),

                                // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
                                LoaiDichVuKyThuat = p.LoaiDichVuKyThuat,
                                LyDoHuyTrangThaiDaThucHien = p.LyDoHuyTrangThaiDaThucHien,
                                ThoiDiemThucHien = p.ThoiDiemThucHien,

                                //BVHD-3598
                                HighLightClass = (isKhamDoanTatCa == true && p.GoiKhamSucKhoeId == null) ? "bg-row-lightblue" : ""
                            }));
                        break;
                        //case (int)EnumNhomGoiDichVu.DichVuGiuongBenh:
                        //    //var sttG = 1;
                        //    goiDichVuKhamBenh.AddRange(yeuCauKhamBenhHienTai.YeuCauDichVuGiuongBenhViens?
                        //        .Where(p => p.TrangThai != EnumTrangThaiGiuongBenh.DaHuy) //p.NhanVienChiDinhId == userId && 
                        //        .OrderBy(x => x.CreatedOn)
                        //        .Select(p => new KhamBenhGoiDichVuGridVo
                        //        {
                        //            STT = stt++,
                        //            Id = p.Id,
                        //            Nhom = EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                        //            NhomId = (int)EnumNhomGoiDichVu.DichVuGiuongBenh,
                        //            LoaiYeuCauDichVuId = p.DichVuGiuongBenhVien?.Id ?? 0,
                        //            NhomGiaDichVuBenhVienId = p.NhomGiaDichVuGiuongBenhVien?.Id ?? 0,
                        //            Ma = p.DichVuGiuongBenhVien?.Ma,
                        //            TenDichVu = p.DichVuGiuongBenhVien?.Ten,
                        //            TenLoaiGia = p.NhomGiaDichVuGiuongBenhVien?.Ten,
                        //            LoaiGia = p.NhomGiaDichVuGiuongBenhVienId ?? 0,
                        //            DonGia = p.Gia,
                        //            //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                        //            NoiThucHien = p.NoiThucHien?.Ten,//String.Format("{0} - {1}", p.NoiThucHien?.Ma ?? "", p.NoiThucHien?.Ten),
                        //            NoiThucHienId = p.NoiThucHienId ?? 0,
                        //            TenNguoiThucHien = " ",
                        //            TrangThaiDichVu = p.TrangThai.GetDescription(),
                        //            TrangThaiDichVuId = (int)p.TrangThai,
                        //            ThanhTien = p.KhongTinhPhi == true ? 0 : p.Gia,
                        //            BHYTThanhToan = 0,
                        //            SoLuong = 1,
                        //            KhongTinhPhi = p.KhongTinhPhi,
                        //            KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                        //            isCheckRowItem = false,
                        //            GiuongBenhId = p.GiuongBenhId,
                        //            TenGiuongBenh = p.GiuongBenh != null ? p.GiuongBenh.Ten : "",
                        //            DaThanhToan = p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan,
                        //            DonGiaBaoHiem = p.DonGiaBaoHiem,
                        //            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                        //            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                        //            TenGoiDichVu = p.YeuCauGoiDichVu != null ? "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper() : null,
                        //            ThoiGianChiDinh = p.ThoiDiemChiDinh,
                        //            NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen
                        //        }));
                        //    break;
                }
            }
            return goiDichVuKhamBenh;
        }

        public async Task<List<KhamBenhGoiDichVuGridVo>> GetDichVuKhacByTiepNhanBenhNhanVer2(GridChiDinhDichVuQueryInfoVo queryInfo)
        {
            // get list nhóm dịch vụ trong gói
            var lstNhomDichVu = EnumHelper.GetListEnum<EnumNhomGoiDichVu>()
                .Select(item => new LookupItemVo()
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).OrderByDescending(x => queryInfo.NhomDichVuId == null || x.KeyId == queryInfo.NhomDichVuId).ThenBy(x => x.DisplayName).ToList();

            var goiDichVuKhamBenh = new List<KhamBenhGoiDichVuGridVo>();
            var stt = 1;

            foreach (var item in lstNhomDichVu)
            {
                switch (item.KeyId)
                {
                    case (int)EnumNhomGoiDichVu.DichVuKhamBenh:
                        //var sttKB = 1;
                        goiDichVuKhamBenh.AddRange(
                            _yeuCauKhamBenhRepository.TableNoTracking
                                .Where(x => x.YeuCauKhamBenhTruocId == queryInfo.YeuCauKhamBenhId
                                            // cập nhật cho phép hiện dịch vụ hủy khi hủy thanh toán
                                            && (x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham))
                                .OrderBy(x => x.CreatedOn)
                                .Select(p => new KhamBenhGoiDichVuGridVo
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
                                    NoiThucHien = p.NoiThucHien.Ten ?? p.NoiDangKy.Ten,
                                    NoiThucHienId = p.NoiThucHienId == null ? (p.NoiDangKyId ?? 0) : (p.NoiThucHienId ?? 0),
                                    TenNguoiThucHien = p.BacSiThucHien.User.HoTen ?? p.BacSiDangKy.User.HoTen,
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
                                    ChuongTrinhGoiDichVuId = p.YeuCauGoiDichVu.ChuongTrinhGoiDichVuId,

                                    //Cập nhật 02/12/2022
                                    //TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                                    //    "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper()
                                    //    : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                    //        p.MienGiamChiPhis
                                    //            .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                    //            .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + a.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper())
                                    //            .First() : null),
                                    ThoiGianChiDinh = p.ThoiDiemChiDinh,
                                    ThanhTien = p.KhongTinhPhi == true ? 0 : (p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia),

                                    //Cập nhật 02/12/2022
                                    //IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),

                                    LyDoHuyDichVu = p.LyDoHuyDichVu,
                                    NguoiChiDinhDisplay = p.NhanVienChiDinh.User.HoTen,

                                    // gói marketing
                                    //CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                                    //CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),

                                    //Cập nhật 02/12/2022
                                    //CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),

                                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId
                                }).ToList());
                        break;
                    case (int)EnumNhomGoiDichVu.DichVuKyThuat:
                        var lstDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                            && x.YeuCauTiepNhanId == queryInfo.YeuCauTiepNhanId
                                            && (queryInfo.IsKhamDoanTatCa == true || (queryInfo.IsKhamDoanTatCa != true && x.YeuCauKhamBenhId == queryInfo.YeuCauKhamBenhId)))
                            // cập nhật 18/05/2021: sắp xếp lại các dịch vụ xét nghiệm theo số thứ tự
                            //.OrderBy(x => lstSortNhomDichVuKyThuat.IndexOf(x.NhomDichVuBenhVienId))
                            //.OrderBy(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ? (x.DichVuKyThuatBenhVien.DichVuXetNghiem.SoThuTu ?? (x.DichVuKyThuatBenhVien.DichVuXetNghiemId ?? 0)) : 0)
                            //.ThenBy(x => x.CreatedOn)
                            .Select(p => new KhamBenhGoiDichVuGridVo
                            {
                                Id = p.Id,
                                Nhom = (string.IsNullOrEmpty(p.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten) ? "" : p.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten + " - ") + p.NhomDichVuBenhVien.Ten,
                                NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                                LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien.Id,
                                NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien.Id,
                                Ma = p.DichVuKyThuatBenhVien.Ma,
                                MaGiaDichVu = p.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia,
                                MaTT37 = p.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350,
                                TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                                TenTT43 = p.TenGiaDichVu,
                                TenLoaiGia = p.NhomGiaDichVuKyThuatBenhVien.Ten,
                                LoaiGia = p.NhomGiaDichVuKyThuatBenhVienId,
                                DonGia = p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia,
                                BHYTThanhToan = 0,
                                NoiThucHien = p.NoiThucHien.Ten,
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
                                KhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT != null && p.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true,
                                LyDoKhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT.LyDoKhongThucHien,
                                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                                ChuongTrinhGoiDichVuId = p.YeuCauGoiDichVu.ChuongTrinhGoiDichVuId,

                                //Cập nhật 02/12/2022
                                //TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                                //    "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + p.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper()
                                //    : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                //        p.MienGiamChiPhis
                                //            .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                //            .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + a.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper())
                                //            .First() : null),
                                IsDichVuXetNghiem = p.NhomDichVuBenhVien.Ma == "XN" || p.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN",
                                BenhPhamXetNghiem = p.BenhPhamXetNghiem,
                                ThoiGianChiDinh = p.ThoiDiemChiDinh,
                                KhongTinhPhi = p.KhongTinhPhi,
                                ThanhTien = p.KhongTinhPhi == true ? 0 : ((p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia) * (decimal)p.SoLan),

                                //Cập nhật 02/12/2022
                                //IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),

                                LyDoHuyDichVu = p.LyDoHuyDichVu,
                                NguoiChiDinhDisplay = p.NhanVienChiDinh.User.HoTen,

                                // gói marketing
                                //CoDichVuNayTrongGoi = !goiDichVus.Any() ? false : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                //    CoDichVuNayTrongGoiKhuyenMai = !goiDichVus.Any() ? false : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),

                                //Cập nhật 02/12/2022
                                //CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),

                                // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
                                LoaiDichVuKyThuat = p.LoaiDichVuKyThuat,
                                LyDoHuyTrangThaiDaThucHien = p.LyDoHuyTrangThaiDaThucHien,
                                ThoiDiemThucHien = p.ThoiDiemThucHien,

                                //BVHD-3598
                                HighLightClass = (queryInfo.IsKhamDoanTatCa == true && p.GoiKhamSucKhoeId == null) ? "bg-row-lightblue" : "",

                                BenhNhanId = p.YeuCauTiepNhan.BenhNhanId,
                                CreatedOn = p.CreatedOn,
                                NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
                                SoThuTuXetNghiem = p.DichVuKyThuatBenhVien.DichVuXetNghiem.SoThuTu,
                                DichVuXetNghiemId = p.DichVuKyThuatBenhVien.DichVuXetNghiemId,

                                //BVHD-3905
                                TiLeThanhToanBHYT = p.DichVuKyThuatBenhVien.TiLeThanhToanBHYT
                            }).ToList();

                        var lstSortNhomDichVuKyThuat = lstDichVuKyThuat.OrderBy(x => x.CreatedOn)
                            .Select(x => x.NhomDichVuBenhVienId).Distinct().ToList();

                        lstDichVuKyThuat = lstDichVuKyThuat
                            // cập nhật 18/05/2021: sắp xếp lại các dịch vụ xét nghiệm theo số thứ tự
                            .OrderBy(x => lstSortNhomDichVuKyThuat.IndexOf(x.NhomDichVuBenhVienId))
                            .ThenBy(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ? (x.SoThuTuXetNghiem ?? (x.DichVuXetNghiemId ?? 0)) : 0)
                            .ThenBy(x => x.CreatedOn)
                            .ToList();

                        goiDichVuKhamBenh.AddRange(lstDichVuKyThuat);
                        break;
                }
            }

            if (goiDichVuKhamBenh.Any())
            {
                #region Kiểm tra gói dịch vụ
                var benhNhanId = goiDichVuKhamBenh.Where(x => x.BenhNhanId != null).Select(x => x.BenhNhanId).FirstOrDefault();
                if (benhNhanId != null)
                {
                    var goiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                        //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs)
                        //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
                        //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                        //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                        .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                                    && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                                    && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                        .Select(x => x.ChuongTrinhGoiDichVuId)
                        .Distinct()
                        .ToList();

                    if (goiDichVus.Any())
                    {
                        #region //Cập nhật 02/12/2022
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
                                        && ((x.YeuCauKhamBenhId != null && lstYeuCauKhamId.Contains(x.YeuCauKhamBenhId))
                                            || (x.YeuCauDichVuKyThuatId != null && lstYeuCauKyThuatId.Contains(x.YeuCauDichVuKyThuatId)))
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
                            //Cập nhật 02/12/2022
                            //yeuCauDichVu.CoDichVuNayTrongGoi = (Enums.EnumNhomGoiDichVu)yeuCauDichVu.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh 
                            //    ? goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == yeuCauDichVu.LoaiYeuCauDichVuId))
                            //    : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == yeuCauDichVu.LoaiYeuCauDichVuId));
                            //yeuCauDichVu.CoDichVuNayTrongGoiKhuyenMai = (Enums.EnumNhomGoiDichVu)yeuCauDichVu.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh
                            //    ? goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == yeuCauDichVu.LoaiYeuCauDichVuId))
                            //    : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == yeuCauDichVu.LoaiYeuCauDichVuId));

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

                if(lstYeuCauKhamChuaThanhToanId.Any() || lstYeuCauKyThuatChuaThanhToanId.Any())
                {
                    var lstYeuCauIdHuyThanhToan = _taiKhoanBenhNhanChiRepository.TableNoTracking
                        .Where(x => (x.YeuCauKhamBenhId != null && lstYeuCauKhamChuaThanhToanId.Contains(x.YeuCauKhamBenhId))
                                    || (x.YeuCauDichVuKyThuatId != null && lstYeuCauKyThuatChuaThanhToanId.Contains(x.YeuCauDichVuKyThuatId)))
                        .Select(x => new
                        {
                            YeuCauKhamBenhId = x.YeuCauKhamBenhId,
                            YeuCauDichVuKyThuatId = x.YeuCauDichVuKyThuatId
                        }).Distinct().ToList();

                    if(lstYeuCauIdHuyThanhToan.Any())
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

            return goiDichVuKhamBenh;
        }
        #endregion


        #region Yêu Cầu Chỉ Định Gói Có Chiết Khấu

        public List<GoiDichVuChiDinhGridVo> GetGoiDichVuKhamBenhKhac(long goiDichVuId, bool coChietkhau)
        {
            var goiDichVuKhamBenh = new List<KhamBenhGoiDichVuGridVo>();
            var goiDichVu = _goiDichVuRepository.TableNoTracking
                .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenh)
                .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)//.ThenInclude(p => p.KhoaPhong).ThenInclude(p => p.KhoaPhongNhanViens).ThenInclude(p => p.NhanVien)
                .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)//.ThenInclude(p => p.Khoa).ThenInclude(p=>p.KhoaPhongNhanViens).ThenInclude(p=>p.NhanVien)
                .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

                .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuong)
                .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)//.ThenInclude(p => p.Khoa).ThenInclude(p => p.KhoaPhongNhanViens).ThenInclude(p => p.NhanVien)
                .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

                //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
                //.Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham)

                //.Include(p => p.GoiDichVuChiTietVatTus).ThenInclude(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)


                .Where(p => p.Id == goiDichVuId && p.IsDisabled != true //&& p.CoChietKhau == coChietkhau
                                       );
            var reulst = SetDataGripViewGoiDichVu(goiDichVu.ToList());
            return reulst;
        }

        public List<GoiDichVuChiDinhGridVo> GetGoiDichChietKhau(long yeuCauTiepNhanId, bool coChietkhau)
        {
            var goiDichVuKhamBenh = new GoiDichVuChiDinhGridVo();
            //TODO: need update goi dv
            //var listGoi = _yeuCauGoiDichVuRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(x => x.GoiDichVuId).ToList();
            //var goiDichVu = _goiDichVuRepository.TableNoTracking
            //    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
            //    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)
            //    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DoiTuongUuDaiDichVuKhamBenhBenhViens)
            //    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenh)
            //    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)//.ThenInclude(p => p.KhoaPhong)
            //    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

            //    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
            //    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)//.ThenInclude(p => p.Khoa)
            //    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

            //    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
            //    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
            //    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuong)
            //    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)//.ThenInclude(p => p.Khoa)
            //    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
            //    .Include(p => p.GoiDichVuChiTietDuocPhams).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPham)
            //    .Include(p => p.GoiDichVuChiTietVatTus).ThenInclude(p => p.VatTuBenhVien).ThenInclude(p => p.VatTus)//    
            //    .Where(p => listGoi.Contains(p.Id) && p.IsDisabled != true && p.CoChietKhau == coChietkhau);

            //var reulst = SetDataGripViewGoiDichVu(goiDichVu.ToList());
            //return reulst;
            return null;
        }

        private List<GoiDichVuChiDinhGridVo> SetDataGripViewGoiDichVu(List<GoiDichVu> goiDichVu)
        {
            var listGoiChietKhau = new List<GoiDichVuChiDinhGridVo>();
            foreach (var item in goiDichVu)
            {
                var goiDichVuKhamBenh = new List<KhamBenhGoiDichVuGridVo>();
                var goiChietKhau = new GoiDichVuChiDinhGridVo();

                // gói bác sĩ ko có dịch vụ khám
                goiDichVuKhamBenh.AddRange(item.GoiDichVuChiTietDichVuKhamBenhs.Select(p => new KhamBenhGoiDichVuGridVo
                {
                    Id = p.Id,
                    Nhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                    NhomId = (int)EnumNhomGoiDichVu.DichVuKhamBenh,
                    LoaiYeuCauDichVuId = p.DichVuKhamBenhBenhVien.Id,
                    NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKhamBenhBenhVien.Id,
                    Ma = p.DichVuKhamBenhBenhVien?.Ma,
                    MaTT37 = p.DichVuKhamBenhBenhVien?.DichVuKhamBenh?.MaTT37,
                    TenDichVu = p.DichVuKhamBenhBenhVien?.Ten,
                    TenLoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                    LoaiGia = p.DichVuKhamBenhBenhVienId,
                    DonGia = p.DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens
                                            .Where(X => X.TuNgay <= DateTime.Now.Date && (X.DenNgay >= DateTime.Now.Date || X.DenNgay == null))?.FirstOrDefault()?.Gia ?? 0,
                    ThanhTien = 0,
                    DonGiaTrongGoi = 0,
                    TLChietKhau = 0,
                    ThanhTienTrongGoi = 0,
                    //NoiThucHien = p.DichVuKhamBenhBenhVien?.KhoaPhong?.Ten,
                    //NoiThucHienId = p.DichVuKhamBenhBenhVien?.KhoaPhong?.Id ?? 0,
                    SoLuong = 1,
                    TrangThaiDichVu = EnumTrangThaiYeuCauKhamBenh.DangKham.GetDescription(),
                }));

                goiDichVuKhamBenh.AddRange(item.GoiDichVuChiTietDichVuKyThuats.Select(p => new KhamBenhGoiDichVuGridVo
                {
                    Id = p.Id,
                    Nhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                    NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                    LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien.Id,
                    NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien.Id,
                    Ma = p.DichVuKyThuatBenhVien.Ma,
                    MaTT37 = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.Ma4350,
                    MaGiaDichVu = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaGia,
                    TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                    TenLoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                    LoaiGia = p.NhomGiaDichVuKyThuatBenhVienId,
                    DonGia = p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens?
                                     .Where(X => X.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId && X.TuNgay <= DateTime.Now.Date
                                                && (X.DenNgay >= DateTime.Now.Date || X.DenNgay == null))?.FirstOrDefault()?.Gia ?? 0,
                    ThanhTien = 0,
                    DonGiaTrongGoi = 0,
                    TLChietKhau = 0,
                    ThanhTienTrongGoi = 0,
                    //NoiThucHien = p.DichVuKyThuatBenhVien?.Khoa?.Ten,
                    //NoiThucHienId = p.DichVuKyThuatBenhVien?.Khoa?.Id ?? 0,
                    SoLuong = Convert.ToDouble(p.SoLan),
                    TrangThaiDichVu = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien.GetDescription(),
                    NhomDichVuKyThuatBenhVienId = p.DichVuKyThuatBenhVien?.NhomDichVuBenhVienId
                }));

                goiDichVuKhamBenh.AddRange(item.GoiDichVuChiTietDichVuGiuongs.Select(p => new KhamBenhGoiDichVuGridVo
                {
                    Id = p.Id,
                    Nhom = EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                    NhomId = (int)EnumNhomGoiDichVu.DichVuGiuongBenh,
                    LoaiYeuCauDichVuId = p.DichVuGiuongBenhVien.Id,
                    NhomGiaDichVuBenhVienId = p.NhomGiaDichVuGiuongBenhVien.Id,
                    Ma = p.DichVuGiuongBenhVien.Ma,
                    TenDichVu = p.DichVuGiuongBenhVien.Ten,
                    TenLoaiGia = p.NhomGiaDichVuGiuongBenhVien?.Ten,
                    LoaiGia = p.NhomGiaDichVuGiuongBenhVienId,
                    DonGia = p.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens?.Where(X => X.NhomGiaDichVuGiuongBenhVienId == p.NhomGiaDichVuGiuongBenhVienId && X.TuNgay <= DateTime.Now.Date
                                                                                           && (X.DenNgay == null || X.DenNgay >= DateTime.Now.Date))?.FirstOrDefault()?.Gia ?? 0,
                    MaTT37 = p.DichVuGiuongBenhVien?.DichVuGiuong?.MaTT37,
                    ThanhTien = 0,
                    DonGiaTrongGoi = 0,
                    TLChietKhau = 0,
                    ThanhTienTrongGoi = 0,
                    //NoiThucHien = p.DichVuGiuongBenhVien?.Khoa?.Ten,
                    //NoiThucHienId = p.DichVuGiuongBenhVien?.Khoa?.Id ?? 0,
                    SoLuong = 1, // do khong co col soluong, và chi co default =1
                    TrangThaiDichVu = EnumTrangThaiGiuongBenh.ChuaThucHien.GetDescription(),
                }));

                //goiDichVuKhamBenh.AddRange(item.GoiDichVuChiTietDuocPhams.Select(p => new KhamBenhGoiDichVuGridVo
                //{
                //    Id = p.Id,
                //    Nhom = EnumNhomGoiDichVu.DuocPham.GetDescription(),
                //    NhomId = (int)EnumNhomGoiDichVu.DuocPham,
                //    LoaiYeuCauDichVuId = p.DuocPhamBenhVien.Id,
                //    Ma = p.DuocPhamBenhVien.DuocPham?.MaHoatChat,
                //    TenDichVu = p.DuocPhamBenhVien.DuocPham?.Ten,
                //    TenLoaiGia = "Bệnh Viện", //todo: cần update
                //    DonGia = p.DuocPhamBenhVien.DuocPhamBenhVienGiaBaoHiems?
                //                         .Where(X => X.TuNgay <= DateTime.Now.Date && (X.DenNgay >= DateTime.Now.Date || X.DenNgay == null))?.FirstOrDefault()?.Gia ?? 0,

                //    ThanhTien = 0,
                //    DonGiaTrongGoi = 0,
                //    TLChietKhau = 0,
                //    ThanhTienTrongGoi = 0,
                //    NoiThucHien = "",
                //    SoLuong = p.SoLuong,
                //    TrangThaiDichVu = EnumYeuCauDuocPhamBenhVien.ChuaThucHien.GetDescription(),
                //}));

                //goiDichVuKhamBenh.AddRange(item.GoiDichVuChiTietVatTus.Select(p => new KhamBenhGoiDichVuGridVo
                //{
                //    Id = p.Id,
                //    Nhom = EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                //    NhomId = (int)EnumNhomGoiDichVu.VatTuTieuHao,
                //    LoaiYeuCauDichVuId = p.VatTuBenhVien.Id,
                //    Ma = p.VatTuBenhVien.Ma,
                //    TenDichVu = p.VatTuBenhVien.VatTus?.Ten,
                //    TenLoaiGia = "Bệnh Viện", //todo: cần update
                //    DonGia = 10000,   //todo: cần update
                //    ThanhTien = 0,
                //    DonGiaTrongGoi = 0,
                //    TLChietKhau = 0,
                //    ThanhTienTrongGoi = 0,
                //    NoiThucHien = "",
                //    SoLuong = p.SoLuong,
                //    TrangThaiDichVu = EnumYeuCauVatTuBenhVien.ChuaThucHien.GetDescription()
                //}));


                //tinh tien cho dich vu
                // TODO: cần Update Tính Tiền Gói Dịch Vụ Chiết Khấu
                #region Tính Tiền Gói Dịch Vụ Chiết Khấu
                goiChietKhau.Ten = item.Ten;
                //update goi dv 10/21
                //goiChietKhau.TongThanhTienTrongGoi = item.ChiPhiGoiDichVu ?? 0;
                foreach (var itemx in goiDichVuKhamBenh)
                    itemx.ThanhTien = itemx.DonGia * (decimal)itemx.SoLuong ?? 0;
                goiChietKhau.TongThanhTien = goiDichVuKhamBenh.Select(p => p.ThanhTien).Sum();
                decimal tempHieuTien = (goiChietKhau.TongThanhTien ?? 0 - goiChietKhau.TongThanhTienTrongGoi ?? 0);
                decimal tempTongTien = goiChietKhau.TongThanhTien ?? 0;
                if (tempHieuTien > 0 && tempTongTien > 0)
                    goiChietKhau.TLChietKhau = (tempHieuTien / tempTongTien) * 100;
                else
                    goiChietKhau.TLChietKhau = 0;
                foreach (var itemx in goiDichVuKhamBenh)
                {
                    itemx.DonGiaTrongGoi = ToPercentage((int)goiChietKhau.TLChietKhau) * itemx.DonGia ?? 0;
                    itemx.ThanhTienTrongGoi = ToPercentage((int)goiChietKhau.TLChietKhau) * (decimal)itemx.SoLuong;
                }
                #endregion

                goiChietKhau.GoiChietKhaus.AddRange(goiDichVuKhamBenh);
                listGoiChietKhau.Add(goiChietKhau);

            }
            return listGoiChietKhau;
        }

        #endregion

        #region GetLookup
        public List<LookupItemVo> GetDataLoaiGiaDichVu()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumDanhMucNhomTheoChiPhi>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            return listEnum;
        }
        public List<LookupItemVo> GetDataNoiThucHienDichVu()
        {
            var listKhoaPhong = _phongBenhVienRepository.TableNoTracking
                 .Where(x => x.IsDisabled != true).Distinct()
                 .Select(i => new LookupItemVo
                 {
                     DisplayName = i.Ten,
                     KeyId = i.Id
                 }).ToList();
            return listKhoaPhong;
        }

        public async Task<List<PhongKhamTemplateVo>> GetPhongThucHienChiDinhGiuong(DropDownListRequestModel model)
        {
            var dvGiuongId = CommonHelper.GetIdFromRequestDropDownList(model); //model.Id == 0 ? CommonHelper.GetIdFromRequestDropDownList(model) : model.Id;
            var phongThucHienDangChonId = model.Id;
            if (dvGiuongId == 0)
            {
                return new List<PhongKhamTemplateVo>();
            }

            var dvGiuongBenhVien =
                await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).FirstOrDefaultAsync(x => x.Id == dvGiuongId);
            var listKhoaPhongId = new List<string>();
            var listPhongBenhVienId = new List<string>();
            var coNoiThucHien = dvGiuongBenhVien != null && dvGiuongBenhVien.DichVuGiuongBenhVienNoiThucHiens.Any();
            if (coNoiThucHien)
            {
                listKhoaPhongId = dvGiuongBenhVien.DichVuGiuongBenhVienNoiThucHiens.Where(x => x.KhoaPhongId != null)
                    .Select(x => x.KhoaPhongId.ToString()).ToList();
                listPhongBenhVienId = dvGiuongBenhVien.DichVuGiuongBenhVienNoiThucHiens
                    .Where(x => x.PhongBenhVienId != null)
                    .Select(x => x.PhongBenhVienId.ToString()).ToList();
            }

            var query = await _phongBenhVienRepository.TableNoTracking
                .ApplyLike(model.Query, x => x.Ma, x => x.Ten)
                .Where(x => x.IsDisabled != true && (coNoiThucHien == false || listKhoaPhongId.Contains(x.KhoaPhongId.ToString()) || listPhongBenhVienId.Contains(x.Id.ToString())))
                .OrderByDescending(x => phongThucHienDangChonId == 0 || x.Id == phongThucHienDangChonId).ThenBy(x => x.Id)
                .Take(model.Take).Select(item => new PhongKhamTemplateVo()
                {
                    DisplayName = item.Ma + " - " + item.Ten,
                    KeyId = item.Id,
                    TenPhong = item.Ten,
                    MaPhong = item.Ma,
                    PhongKhamId = item.Id
                }).ToListAsync();

            return query;
        }

        public async Task<List<LookupItemGiuongBenhVo>> GetListGiuongBenhTheoPhongAsync(DropDownListRequestModel model)
        {
            var phongKhamId = CommonHelper.GetIdFromRequestDropDownList(model);//model.Id == 0 ? CommonHelper.GetIdFromRequestDropDownList(model) : model.Id;
            var giuongThucHienDangChonId = model.Id;
            if (phongKhamId == 0)
            {
                return new List<LookupItemGiuongBenhVo>();
            }

            var query = await _giuongBenhRepository.TableNoTracking
                .Include(x => x.HoatDongGiuongBenhs)
                .ApplyLike(model.Query, x => x.Ten)
                .Where(x => x.PhongBenhVienId == phongKhamId && x.IsDisabled != true) // && !x.HoatDongGiuongBenhs.Any(y => y.GiuongBenhId == x.Id && y.ThoiDiemKetThuc == null))
                .OrderByDescending(x => giuongThucHienDangChonId == 0 || x.Id == giuongThucHienDangChonId)
                .ThenBy(x => x.HoatDongGiuongBenhs.Count(y => y.ThoiDiemKetThuc == null))
                .ThenBy(x => x.Ten)
                .Take(model.Take).Select(item => new LookupItemGiuongBenhVo()
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    SoBenhNhanHienTai = item.HoatDongGiuongBenhs.Count(i => i.ThoiDiemKetThuc == null)
                }).ToListAsync();

            return query;
        }


        public async Task<List<PhongKhamTemplateVo>> GetPhongThucHienChiDinhDuocOrVatTu(DropDownListRequestModel model)
        {
            var listKhoaPhong = await _phongBenhVienRepository.TableNoTracking
                .Include(p => p.KhoaPhong).ThenInclude(p => p.KhoaPhongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
                .Where(x => x.IsDisabled != true).Distinct().ToListAsync();

            var query = listKhoaPhong.Select(item => new PhongKhamTemplateVo
            {
                DisplayName = item.Ma + " - " + item.Ten,
                KeyId = item.Id,
                TenPhong = item.Ten,
                MaPhong = item.Ma,
                TenNhanVien = item.KhoaPhong?.KhoaPhongNhanViens?.FirstOrDefault()?.NhanVien?.User?.HoTen ?? "",
                NhanVienId = item.KhoaPhong?.KhoaPhongNhanViens?.FirstOrDefault()?.NhanVien?.User?.Id ?? 0,
                PhongKhamId = item.Id,
            }).Distinct().ToList();

            return query;
        }
        public async Task<List<PhongKhamTemplateVo>> GetPhongThucHienChiDinhKhamOrDichVuKyThuat(DropDownListRequestModel model, string selectedItems = null)
        {
            var dvKyThuatId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (dvKyThuatId == 0)
            {
                return new List<PhongKhamTemplateVo>();
            }

            var dvKyThuatBenhVien =
                await _dichVuKyThuatBenhVienRepository.TableNoTracking.Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).FirstOrDefaultAsync(x => x.Id == dvKyThuatId);
            if (dvKyThuatBenhVien != null && dvKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens.Any())
            {
                var lstKhoaPhongId = dvKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens
                    .Where(x => x.KhoaPhongId != null)
                    .Select(x => x.KhoaPhongId.ToString()).ToList();

                var lstPhongBenhVienId = dvKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens
                    .Where(x => x.PhongBenhVienId != null)
                    .Select(x => x.PhongBenhVienId.ToString()).ToList();

                if (model.Id != 0)
                {
                    lstPhongBenhVienId.Add(model.Id.ToString());
                }
                // mutilselect
                var selectedItemIds = new List<long>();
                if (!string.IsNullOrEmpty(selectedItems))
                {
                    selectedItemIds = selectedItems.Split(",").Select(long.Parse).ToList();
                }
                //get phong benh vien theo DichVuKyThuatBenhVienId
                var query = await _phongBenhVienRepository.TableNoTracking
                    .Where(x => x.IsDisabled != true && (lstPhongBenhVienId.Contains(x.Id.ToString()) || lstKhoaPhongId.Contains(x.KhoaPhongId.ToString()) || selectedItemIds.Any(z => z == x.Id)))
                    .OrderByDescending(x => x.Id == model.Id || selectedItemIds.Any(z => z == x.Id)).ThenBy(x => x.Id)
                    .Select(item => new PhongKhamTemplateVo
                    {
                        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                        KeyId = item.Id,
                        TenPhong = item.Ten,
                        MaPhong = item.Ma,
                        PhongKhamId = item.Id,
                    }).ApplyLike(model.Query, g => g.TenPhong, g => g.MaPhong)
                    .Distinct()
                    .Take(model.Take)
                    .ToListAsync();
                return query;
            }
            else // get all phong benh vien
            {
                var listPhong = await _phongBenhVienRepository.TableNoTracking
                    //.Include(p => p.KhoaPhong).ThenInclude(p => p.KhoaPhongNhanViens)
                    .Where(x => x.IsDisabled != true || x.Id == model.Id)
                    .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Id)
                    .ApplyLike(model.Query, g => g.Ten, g => g.Ma).Select(item => new PhongKhamTemplateVo
                    {
                        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                        KeyId = item.Id,
                        TenPhong = item.Ten,
                        MaPhong = item.Ma,
                        PhongKhamId = item.Id,
                    })
                    .Distinct()
                    .Take(model.Take)
                    .ToListAsync();
                return listPhong;
            }
        }

        public async Task<List<LookupItemVo>> GetGoiChietKhau(DropDownListRequestModel model, bool coChietkhau)
        {
            //update goi dv 10/21
            var listKhoaPhong = await _goiDichVuRepository.TableNoTracking
                .Where(x => x.IsDisabled != true
                            //&& x.CoChietKhau == coChietkhau
                            && x.LoaiGoiDichVu == EnumLoaiGoiDichVu.TrongPhongBacSy
                            //&& x.NgayBatDau.Date <= DateTime.Now.Date && (x.NgayKetThuc == null || x.NgayKetThuc.Value.Date >= DateTime.Now.Date)
                            ).ToListAsync();
            var query = listKhoaPhong.Select(i => new LookupItemVo
            {
                DisplayName = i.Ten,
                KeyId = i.Id
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetListBacSiThucHienAsync(DropDownListRequestModel model)
        {
            var phongThucHienId = CommonHelper.GetIdFromRequestDropDownList(model);
            var phongBenhVien =
                await _phongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == phongThucHienId);
            var lstBacSi = new List<LookupItemVo>();
            if (phongBenhVien != null)
            {
                lstBacSi = await _userRepository.TableNoTracking
                    .Include(x => x.NhanVien).ThenInclude(y => y.KhoaPhongNhanViens)
                    .Include(x => x.NhanVien).ThenInclude(y => y.ChucDanh).ThenInclude(z => z.NhomChucDanh)
                    .ApplyLike(model.Query, x => x.HoTen)
                    .Where(x => x.NhanVien.KhoaPhongNhanViens.Any(z => z.KhoaPhongId == phongBenhVien.KhoaPhongId) && (x.NhanVien.ChucDanh != null && x.NhanVien.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi))
                    .Take(model.Take)
                    .Select(item => new LookupItemVo()
                    {
                        KeyId = item.Id,
                        DisplayName = item.HoTen
                    }).ToListAsync();
            }

            return lstBacSi;
        }

        public async Task<List<DichVuTheoGoiVo>> GetListDichVuTheoGoiAsync(DropDownListRequestModel model)
        {
            var lstDichVu = new List<DichVuTheoGoiVo>();
            var goiDichVuId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (goiDichVuId != 0)
            {
                var goiDichVu = GetGoiDichVuKhamBenhKhac(goiDichVuId, false);
                lstDichVu = goiDichVu.SelectMany(x => x.GoiChietKhaus).Select(item => new DichVuTheoGoiVo()
                {
                    KeyId = "{'Id': " + item.Id.Value + ", 'NhomDichVu': " + item.NhomId + "}",
                    DisplayName = item.Ma + " - " + item.TenDichVu,
                    NhomDichVu = item.NhomId,
                    Ma = item.Ma,
                    Ten = item.TenDichVu
                }).Where(x => string.IsNullOrEmpty(model.Query) || (!string.IsNullOrEmpty(model.Query) && (x.Ma.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())
                                                                                                           || x.Ten.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower()))))
                .Take(model.Take).ToList();
            }

            return lstDichVu;
        }

        public async Task<List<LookupCheckItemVo>> GetListIdDichVuTheoGoiAsync(long goiDichVuId)
        {
            var lstDichVu = new List<LookupCheckItemVo>();
            if (goiDichVuId != 0)
            {
                var goiDichVu = GetGoiDichVuKhamBenhKhac(goiDichVuId, false);
                lstDichVu = goiDichVu.SelectMany(x => x.GoiChietKhaus)
                    .Select(item => new LookupCheckItemVo()
                    {
                        KeyId = "{'Id': " + item.Id.Value + ", 'NhomDichVu': " + item.NhomId + "}",
                        DisplayName = item.Ma + " - " + item.TenDichVu,
                        IsCheck = true // mặc định check all khi chọn mới 1 gói dịch vụ
                    }).ToList();
            }

            return lstDichVu;
        }

        public async Task<List<LookupItemVo>> GetNhomGiaTheoLoaiDichVuKyThuatAsync(DropDownListRequestModel model)
        {
            var query = await _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking
                .Select(item => new LookupItemVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten
                }).Take(model.Take).ToListAsync();
            return query;
        }

        public async Task<List<LookupItemVo>> GetListNhomDichVuKyThuat(DropDownListRequestModel model)
        {
            var lstNhomDichVuCls = await _nhomDichVuKyThuatRepository.TableNoTracking
               //.Where(x=>x.DichVuKyThuats.Any( dvkt => dvkt.NhomChiPhi == EnumDanhMucNhomTheoChiPhi.XetNghiem
               //                                     || dvkt.NhomChiPhi == EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh
               //                                     || dvkt.NhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThamDoChucNang))
               .ApplyLike(model.Query, o => o.Ten)
               .Take(model.Take).ToListAsync();

            var query = lstNhomDichVuCls.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id
            }).ToList();
            return query;
        }

        public async Task<List<NhomDichVuBenhVienTreeViewVo>> GetListNhomDichVuBenhVienAsync(DropDownListRequestModel model, bool isLoadNhomTiemChung = false)
        {
            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            long? nhomTiemChungId = null;
            if (!isLoadNhomTiemChung)
            {
                var cauHinhNhomTiemChung = await _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhTiemChung.NhomDichVuTiemChung").FirstOrDefaultAsync();
                nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;
            }

            var lstNhomDichVu = await _nhomDichVuBenhVienRepository.TableNoTracking
                .Select(item => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                    ParentId = item.NhomDichVuBenhVienChaId,
                    Ma = item.Ma,
                    IsDefault = item.IsDefault
                })
                .Where(x => (nhomTiemChungId == null || x.KeyId != nhomTiemChungId))
                .ToListAsync();
            if (!string.IsNullOrEmpty(model.ParameterDependencies))
            {
                var info = JsonConvert.DeserializeObject<NhomDichVuBenhVienJSONVo>(model.ParameterDependencies);
                if (info.LaPhieuDieuTri)
                {
                    lstNhomDichVu = lstNhomDichVu.Where(p => p.KeyId != (long)LoaiDichVuKyThuat.SuatAn).ToList(); // SA => Suất Ăn
                }
            }
            var query = lstNhomDichVu.Select(item => new NhomDichVuBenhVienTreeViewVo
            {
                KeyId = item.KeyId,
                DisplayName = item.DisplayName,
                ParentId = item.ParentId,
                IsDefault = item.IsDefault,
                Ma = item.Ma,
                Items = GetChildrenTree(lstNhomDichVu, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
            })
            .Where(x =>
                x.ParentId == null && (string.IsNullOrEmpty(model.Query) ||
                                       (!string.IsNullOrEmpty(model.Query) && (x.Items.Any() || x.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())))))
            .Take(model.Take).ToList();
            return query;
        }

        public static List<NhomDichVuBenhVienTreeViewVo> GetChildrenTree(List<NhomDichVuBenhVienTreeViewVo> comments, long Id, string queryString, string parentDisplay)
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id)
                .Select(c => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Ma = c.Ma,
                    Items = GetChildrenTree(comments, c.KeyId, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString)
                            || (!string.IsNullOrEmpty(queryString) && (parentDisplay.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any())))
                .ToList();
            return query;
        }

        public async Task<List<DichVuKyThuatBenhVienTemplateVo>> GetListDichVuKyThuat(long id, DropDownListRequestModel model)
        {
            var lstNhomDichVuBenhVien = await GetListNhomBenhVienTheoNhomChaId(id);
            var lstNhomDichVubenhVien = GetListNhomBenhVienIdTheoNhomChaId(id, lstNhomDichVuBenhVien);

            var lstDichVuKyThuat = new List<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien>();

            //var list = await _dichVuKyThuatBenhVienRepository.TableNoTracking
            //  .Include(p => p.DichVuKyThuat).ThenInclude(p => p.DichVuKyThuatThongTinGias)
            //  .Include(p => p.DichVuKyThuatVuBenhVienGiaBenhViens).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //  .Where(x => (id == 0 || lstNhomDichVubenhVien.Any(y => y == x.NhomDichVuBenhVienId)) && x.HieuLuc != false
            //        && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)))

            //  .ApplyLike(model.Query, o => o.Ten, o => o.Ma)
            //  .Take(model.Take)
            //  .ToListAsync();

            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                lstDichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                  .Include(p => p.DichVuKyThuat).ThenInclude(p => p.DichVuKyThuatThongTinGias)
                  .Include(p => p.DichVuKyThuatVuBenhVienGiaBenhViens).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                  .Where(x => (id == 0 || lstNhomDichVubenhVien.Any(y => y == x.NhomDichVuBenhVienId)) && x.HieuLuc != false
                        && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)))

                  .ApplyLike(model.Query, o => o.Ten, o => o.Ma)
                  .Take(model.Take)
                  .ToListAsync();
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");
                lstDichVuKyThuat = await _dichVuKyThuatBenhVienRepository
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien), lstColumnNameSearch)
                    .Where(x =>
                        (id == 0 || lstNhomDichVubenhVien.Any(y => y == x.NhomDichVuBenhVienId))
                        && x.HieuLuc
                        && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.Gia > 0 && o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)))
                    .Take(model.Take)
                    .ToListAsync();
            }

            var query = lstDichVuKyThuat.Select(item => new DichVuKyThuatBenhVienTemplateVo
            {
                DisplayName = item.Ma + " - " + item.Ten,
                KeyId = item.Id,
                DichVu = item.Ten,
                Ma = item.Ma,
                //NhomDichVuKyThuatId = item.NhomDichVuBenhVienId, // item.DichVuKyThuat?.NhomDichVuKyThuatId ?? 0,
                //MaGiaDichVu = item.DichVuKyThuat?.MaGia,
                //Gia = item.DichVuKyThuatVuBenhVienGiaBenhViens?
                //                        .Where(X => X.TuNgay <= DateTime.Now.Date && X.DenNgay >= DateTime.Now.Date
                //                                             || (X.TuNgay <= DateTime.Now.Date && X.DenNgay == null))?.FirstOrDefault()?.Gia,
                //NhomGiaDichVuKyThuatBenhVienId = item.DichVuKyThuatVuBenhVienGiaBenhViens?
                //                                     .Where(p => p.TuNgay <= DateTime.Now.Date && (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date))
                //                                     .FirstOrDefault()?.NhomGiaDichVuKyThuatBenhVienId,
                //NhomChiPhi = item.DichVuKyThuat?.NhomChiPhi ?? EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                //LoaiDichVuKyThuat = GetLoaiDichVuKyThuatChiDinh(item.DichVuKyThuat?.NhomChiPhi ?? EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe),
                //NhomDichVuBenhVienId = item.NhomDichVuBenhVienId
            }).ToList();
            return query;
        }

        public async Task<List<DichVuKyThuatBenhVienMultiSelectTemplateVo>> GetListDichVuKyThuatMultiSelectAsync(MultiselectQueryInfo model, bool isPTTT, bool isPhieuDieuTri = false)
        {
            var id = CommonHelper.GetIdFromRequestMultiSelect(model);
            var selectedItems = new DichVuKyThuatChiDinhDangChonVo();
            if (id != 0 && !string.IsNullOrEmpty(model.SelectedItems) && model.SelectedItems != "[]")
            {
                var strSelectedItem = "{DichVuChiDinhIds:" + model.SelectedItems.Replace("\"{", "{").Replace("}\"", "}").Replace(" ", "").Replace("\\\"", "\"") + "}";
                selectedItems = JsonConvert.DeserializeObject<DichVuKyThuatChiDinhDangChonVo>(strSelectedItem);
            }

            var lstNhomDichVuBenhVien = await GetListNhomBenhVienTheoNhomChaId(id);
            var lstNhomDichVuBenhVienId = GetListNhomBenhVienIdTheoNhomChaId(id, lstNhomDichVuBenhVien);

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = await _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhTiemChung.NhomDichVuTiemChung").FirstOrDefaultAsync();
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            //KH yêu cầu lấy tất cả
            ////Phẫu thuật thủ thuật
            //if (id == 0 && isPTTT)
            //{
            //    //CDHA & TDCN & Xét nghiệm
            //    var lstNhomDichVuBenhVienIds = await _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.Ma == "XN" || p.Ma == "CDHA" || p.Ma == "TDCN")
            //                                                                                      .Select(p => p.Id)
            //                                                                                      .ToListAsync();

            //    foreach(var item in lstNhomDichVuBenhVienIds)
            //    {
            //        var lstNhomDichVuBenhVienTemp = await GetListNhomBenhVienTheoNhomChaId(item);
            //        var lstNhomDichVubenhVienTemp = GetListNhomBenhVienIdTheoNhomChaId(item, lstNhomDichVuBenhVienTemp);

            //        lstNhomDichVuBenhVienId.AddRange(lstNhomDichVubenhVienTemp);
            //    }
            //}

            var lstDichVuKyThuat = new List<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien>();

            //string templateKeyId = "\"DichVuId\": {0}, \"NhomId\": {1},  \"TenNhom\": \"{2}\"";

            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                lstDichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Include(p => p.NhomDichVuBenhVien)
                    .Include(p => p.DichVuKyThuat)
                    //.Where(x => ((id == 0 && !isPTTT) || lstNhomDichVuBenhVienId.Any(y => y == x.NhomDichVuBenhVienId)) 
                    .Where(x => (id == 0 || lstNhomDichVuBenhVienId.Any(y => y == x.NhomDichVuBenhVienId))
                               //&& (((id == 0 && !isPhieuDieuTri) || x.NhomDichVuBenhVien.Ma != "SA") && x.NhomDichVuBenhVien.IsDefault)
                               && (!isPhieuDieuTri || (isPhieuDieuTri && x.NhomDichVuBenhVienId != (long)LoaiDichVuKyThuat.SuatAn))
                               && x.HieuLuc
                               && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))

                               // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
                               && (nhomTiemChungId == null || x.NhomDichVuBenhVienId != nhomTiemChungId))
                    .OrderByDescending(x => selectedItems.DichVuChiDinhIds.Any(a => a.DichVuId == x.Id)).ThenBy(x => x.Ten)
                    .ApplyLike(model.Query, o => o.Ten, o => o.Ma)
                    .Distinct()
                    .Take(model.Take)
                  .ToListAsync();
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");

                // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
                var lstDichVuTiemChungId = new List<long>();
                if (nhomTiemChungId != null)
                {
                    lstDichVuTiemChungId = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                        .Where(x => x.NhomDichVuBenhVienId == nhomTiemChungId)
                        .Select(x => x.Id)
                        .ToListAsync();
                }

                var lstDichVuKyThuatBenhVienId = await _dichVuBenhVienTongHopRepository
                    .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch)
                    .Where(p => p.LoaiDichVuBenhVien == EnumDichVuTongHop.KyThuat
                                && p.HieuLuc

                                // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
                                && !lstDichVuTiemChungId.Contains(p.DichVuKyThuatBenhVienId.Value))
                    .Select(x => x.DichVuKyThuatBenhVienId)
                    .Take(model.Take).ToListAsync();

                lstDichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    //.ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien), lstColumnNameSearch)
                    .Include(p => p.NhomDichVuBenhVien)
                    .Include(p => p.DichVuKyThuat)
                    .Where(x =>
                        //((id == 0 && !isPTTT) || lstNhomDichVuBenhVienId.Any(y => y == x.NhomDichVuBenhVienId))
                        (id == 0 || lstNhomDichVuBenhVienId.Any(y => y == x.NhomDichVuBenhVienId))
                        && (!isPhieuDieuTri || (isPhieuDieuTri && x.NhomDichVuBenhVienId != (long)LoaiDichVuKyThuat.SuatAn))
                        && x.HieuLuc
                        && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                        && lstDichVuKyThuatBenhVienId.Any(a => a == x.Id))
                    .OrderBy(x => lstDichVuKyThuatBenhVienId.IndexOf(x.Id))
                    //.Take(model.Take)
                    .ToListAsync();

                //lstDichVuKyThuat = await _dichVuKyThuatBenhVienRepository
                //    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien), lstColumnNameSearch)
                //    .Include(p => p.NhomDichVuBenhVien)
                //    .Include(p => p.DichVuKyThuat)
                //    .Where(x =>
                //        (id == 0 || lstNhomDichVubenhVien.Any(y => y == x.NhomDichVuBenhVienId))
                //        && x.HieuLuc
                //        && x.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.Gia > 0 && o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)))
                //    .Take(model.Take)
                //    .ToListAsync();
            }

            var query = lstDichVuKyThuat.Select(item => new DichVuKyThuatBenhVienMultiSelectTemplateVo
            {
                DisplayName = item.Ten, //item.Ma + " - " + item.Ten,
                //KeyId = "{" + string.Format(templateKeyId, item.Id, item.NhomDichVuBenhVienId, item.NhomDichVuBenhVien.Ten) + "}",
                DichVuKyThuatBenhVienId = item.Id,
                NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                TenNhomDichVuBenhVien = item.NhomDichVuBenhVien.Ten.Trim(),//(item.NhomDichVuBenhVien.Ma + " - " + item.NhomDichVuBenhVien.Ten).Trim(),
                //NoiThucHien = GetNoiThucHienUuTienTheoDichVuKyThuatAsync(item.Id).Result,
                Ma = item.Ma,
                Ten = item.Ten,
                TenTT43 = item.DichVuKyThuat?.TenGia
            }).ToList();
            
            var dichVuKyThuatBenhVienIds = query.Select(o => o.DichVuKyThuatBenhVienId).ToList();
            var dichVuKyThuatBenhVienNoiThucHienUuTiens = _dichVuKyThuatBenhVienNoiThucHienUuTienRepository.TableNoTracking
                .Where(o => dichVuKyThuatBenhVienIds.Contains(o.DichVuKyThuatBenhVienId))
                .Select(o => new { o.DichVuKyThuatBenhVienId, o.LoaiNoiThucHienUuTien, o.PhongBenhVienId })
                .ToList();
            var dichVuKyThuatBenhVienNoiThucHiens = _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking
                .Where(o => dichVuKyThuatBenhVienIds.Contains(o.DichVuKyThuatBenhVienId))
                .Select(o => new { o.DichVuKyThuatBenhVienId, o.KhoaPhongId, o.PhongBenhVienId })
                .ToList();
            var phongBenhViens = _phongBenhVienRepository.TableNoTracking                
                .Select(o => new { o.Id, o.KhoaPhongId, o.Ten, o.IsDisabled })
                .ToList();

            foreach (var item in query)
            {
                long noiThucHienId = 0;
                var noiThucHienUuTiens = dichVuKyThuatBenhVienNoiThucHienUuTiens.Where(o => o.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId).ToList();
                if (noiThucHienUuTiens.Any())
                {
                    var nguoiDungUuTien = noiThucHienUuTiens.FirstOrDefault(o => o.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung);
                    if (nguoiDungUuTien != null)
                    {
                        noiThucHienId = nguoiDungUuTien.PhongBenhVienId;
                    }
                    else
                    {
                        var heThongUuTien = noiThucHienUuTiens.First();
                        noiThucHienId = heThongUuTien.PhongBenhVienId;
                    }
                }
                else
                {                    
                    var noiThucHiens = dichVuKyThuatBenhVienNoiThucHiens.Where(o => o.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId).ToList();
                    if (noiThucHiens.Any())
                    {
                        var lstKhoaPhongId = noiThucHiens
                            .Where(x => x.KhoaPhongId != null)
                            .Select(x => x.KhoaPhongId.ToString()).ToList();

                        var lstPhongBenhVienId = noiThucHiens
                            .Where(x => x.PhongBenhVienId != null)
                            .Select(x => x.PhongBenhVienId.ToString()).ToList();

                        
                        
                        //get phong benh vien theo DichVuKyThuatBenhVienId
                        var phongBenhVienId = phongBenhViens
                            .Where(x => x.IsDisabled != true && (lstPhongBenhVienId.Contains(x.Id.ToString()) || lstKhoaPhongId.Contains(x.KhoaPhongId.ToString())))
                            .OrderBy(x => x.Id)
                            .Select(o => o.Id)
                            .FirstOrDefault();                        
                        noiThucHienId = phongBenhVienId;                        
                    }
                    if (noiThucHienId == 0) // get all phong benh vien
                    {
                        noiThucHienId = phongBenhViens
                            .Where(x => x.IsDisabled != true)
                            .OrderBy(x => x.Id)
                            .Select(o => o.Id)
                            .FirstOrDefault();
                    }
                }
                var noiThucHien = phongBenhViens.FirstOrDefault(x => x.Id == noiThucHienId);

                var noiThucHienVo = new NoiThucHienUuTienVo();
                if (noiThucHien != null)
                {
                    noiThucHienVo.NoiThucHienId = noiThucHien.Id;
                    noiThucHienVo.DisplayNoiThucHien = noiThucHien.Ten;//noiThucHien.Ma + " - " + noiThucHien.Ten;
                }
                item.NoiThucHien = noiThucHienVo;
            }
            
            return query;
        }

        private async Task<NoiThucHienUuTienVo> GetNoiThucHienUuTienTheoDichVuKyThuatAsync(long dichVuKyThuatBenhVienId)
        {
            long noiThucHienId = 0;
            var dichVuKyThuatBenhVien = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).FirstOrDefaultAsync(x => x.Id == dichVuKyThuatBenhVienId);

            if (dichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHienUuTiens.Any())
            {
                // nơi thực hiện uuw tiên của người dùng thiết lập
                var nguoiDungUuTien =
                    dichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHienUuTiens.FirstOrDefault(x =>
                        x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung);
                if (nguoiDungUuTien != null)
                {
                    noiThucHienId = nguoiDungUuTien.PhongBenhVienId;
                }
                else
                {
                    var heThongUuTien = dichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHienUuTiens.First();
                    noiThucHienId = heThongUuTien.PhongBenhVienId;
                }
            }
            else
            {
                var query = new DropDownListRequestModel
                {
                    ParameterDependencies = "{DichVuId: " + dichVuKyThuatBenhVienId + "}",
                    Take = 1
                };
                var noiThucHienTheoDichVuKyThuats =
                    await GetPhongThucHienChiDinhKhamOrDichVuKyThuat(query);
                if (noiThucHienTheoDichVuKyThuats.Any())
                {
                    noiThucHienId = noiThucHienTheoDichVuKyThuats.First().KeyId;
                }
            }

            // thông tin nơi thực hiện
            var noiThucHien =
                await _phongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == noiThucHienId);

            var noiThucHienVo = new NoiThucHienUuTienVo();
            if (noiThucHien != null)
            {
                noiThucHienVo.NoiThucHienId = noiThucHien.Id;
                noiThucHienVo.DisplayNoiThucHien = noiThucHien.Ten;//noiThucHien.Ma + " - " + noiThucHien.Ten;
            }
            return noiThucHienVo;
        }

        private async Task<List<NhomDichVuBenhVienTreeViewVo>> GetListNhomBenhVienTheoNhomChaId(long nhomChaId)
        {
            //var listNhomBenhVien = 

            var query = await _nhomDichVuBenhVienRepository.TableNoTracking
                .Select(item => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ma + " - " + item.Ten,
                    ParentId = item.NhomDichVuBenhVienChaId
                })
                .ToListAsync();

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
        public async Task<List<DichVuKhamBenhBenhVienTemplateVo>> GetListDichVuKhamBenh(DropDownListRequestModel model)
        {
            var listDichVuKhamBenhs = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Include(p => p.DichVuKhamBenh).ThenInclude(p => p.DichVuKhamBenhThongTinGias)
                .Include(p => p.DichVuKhamBenhBenhVienGiaBenhViens).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                .ApplyLike(model.Query, o => o.Ten)
                .Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") && p.HieuLuc != false)
                .Take(model.Take)
                .ToListAsync();

            var query = listDichVuKhamBenhs.Select(item => new DichVuKhamBenhBenhVienTemplateVo
            {
                DisplayName = item.Ma + " - " + item.Ten,
                KeyId = item.Id,
                Ma = item.Ma,
                DichVu = item.Ten,
                MaDichVuTT37 = item.DichVuKhamBenh?.MaTT37,
                Gia = item.DichVuKhamBenhBenhVienGiaBenhViens?
                                        .Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay))?.FirstOrDefault()?.Gia,
                NhomGiaDichVuKhamBenhBenhVienId = item.DichVuKhamBenhBenhVienGiaBenhViens?
                                                     .Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay))
                                                     .FirstOrDefault()?.NhomGiaDichVuKhamBenhBenhVienId,
            }).ToList();

            return query;
        }
        public async Task<List<DichVuGiuongBenhVienTemplateVo>> GetListDichVuGiuongBenhVien(DropDownListRequestModel model)
        {
            //var listDichVuKhamBenhs = await _dichVuGiuongBenhVienRepository.TableNoTracking
            //    .Include(p => p.DichVuGiuong).ThenInclude(p => p.DichVuGiuongThongTinGias)
            //    .Include(p => p.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
            //    .Include(p => p.DichVuGiuongBenhVienGiaBaoHiems)
            //    .ApplyLike(model.Query, o => o.Ten, o => o.Ma)
            //    .Where(p => (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")) && p.HieuLuc != false
            //        && p.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay))
            //    )
            //    .Take(model.Take)
            //    .ToListAsync();

            var thoiGianChiDinhDichVuGiuongVo = new ThoiGianChiDinhDichVuGiuongVo();

            if (!string.IsNullOrEmpty(model.ParameterDependencies))
            {
                thoiGianChiDinhDichVuGiuongVo = JsonConvert.DeserializeObject<ThoiGianChiDinhDichVuGiuongVo>(model.ParameterDependencies.Trim());
                thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan = thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan == null ? DateTime.Now : thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan;
                //thoiGianChiDinhDichVuGiuongVo.ThoiGianTra = thoiGianChiDinhDichVuGiuongVo.ThoiGianTra == null ? DateTime.Now : thoiGianChiDinhDichVuGiuongVo.ThoiGianTra;
            }

            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listDichVuKhamBenhs = await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .ApplyLike(model.Query, o => o.Ten, o => o.Ma)
                    //.Where(p => p.HieuLuc
                    //        && p.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay)))
                    .Where(p => p.HieuLuc
                            && p.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date && (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date)))
                    .Take(model.Take)
                    .Select(item => new DichVuGiuongBenhVienTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ma = item.Ma,
                        DichVu = item.Ten,
                        MaTT37 = item.DichVuGiuong.MaTT37,
                        //Gia = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).Select(o => o.Gia).FirstOrDefault(),
                        Gia = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                               (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date))
                                                                   .Select(o => o.Gia)
                                                                   .FirstOrDefault(),
                        //LoaiGiaCoHieuLuc = item.DichVuGiuongBenhVienGiaBenhViens.Count(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)),
                        LoaiGiaCoHieuLuc = item.DichVuGiuongBenhVienGiaBenhViens.Count(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                                            (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date)),
                        //NhomGiaDichVuGiuongBenhVienId = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).Select(o => o.NhomGiaDichVuGiuongBenhVienId).FirstOrDefault(),
                        NhomGiaDichVuGiuongBenhVienId = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                                                         (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date))
                                                                                             .Select(o => o.NhomGiaDichVuGiuongBenhVienId)
                                                                                             .FirstOrDefault(),
                        //BaoPhong = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).Any(o => o.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng"),
                        BaoPhong = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                                    (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date))
                                                                        .Any(o => o.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng"),
                        //CoGiaBaoHiem = item.DichVuGiuongBenhVienGiaBaoHiems.Any(o => o.TuNgay <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay)),
                        CoGiaBaoHiem = item.DichVuGiuongBenhVienGiaBaoHiems.Any(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                                     (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date)),
                        LoaiGiuong = item.LoaiGiuong,
                        //BaoPhong = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).Select(o => o.BaoPhong).FirstOrDefault() ?? false,
                    })
                    .ToListAsync();
                return listDichVuKhamBenhs;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");

                var listDichVuKhamBenhs = await _dichVuGiuongBenhVienRepository
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien), lstColumnNameSearch)
                    .Include(p => p.DichVuGiuong).ThenInclude(p => p.DichVuGiuongThongTinGias)
                    .Include(p => p.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                    .Include(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                    //.Where(x => x.HieuLuc
                    //            && x.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                    .Where(x => x.HieuLuc
                                && x.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date && (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date)))
                    .Take(model.Take)
                    .Select(item => new DichVuGiuongBenhVienTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ma = item.Ma,
                        DichVu = item.Ten,
                        MaTT37 = item.DichVuGiuong.MaTT37,
                        //Gia = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).Select(o => o.Gia).FirstOrDefault(),
                        Gia = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                               (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date))
                                                                   .Select(o => o.Gia)
                                                                   .FirstOrDefault(),
                        LoaiGiaCoHieuLuc = item.DichVuGiuongBenhVienGiaBenhViens.Count(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                                          (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date)),

                        //NhomGiaDichVuGiuongBenhVienId = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).Select(o => o.NhomGiaDichVuGiuongBenhVienId).FirstOrDefault(),
                        NhomGiaDichVuGiuongBenhVienId = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                                                         (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date))
                                                                                             .Select(o => o.NhomGiaDichVuGiuongBenhVienId)
                                                                                             .FirstOrDefault(),
                        //BaoPhong = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).Any(o => o.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng"),
                        BaoPhong = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                                    (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date))
                                                                        .Any(o => o.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng"),
                        //CoGiaBaoHiem = item.DichVuGiuongBenhVienGiaBaoHiems.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)),
                        CoGiaBaoHiem = item.DichVuGiuongBenhVienGiaBaoHiems.Any(o => o.TuNgay.Date <= thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date &&
                                                                                     (o.DenNgay == null || thoiGianChiDinhDichVuGiuongVo.ThoiGianNhan.Value.Date <= o.DenNgay.Value.Date)),
                        LoaiGiuong = item.LoaiGiuong,
                        //BaoPhong = item.DichVuGiuongBenhVienGiaBenhViens.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).Select(o => o.BaoPhong).FirstOrDefault() ?? false,
                    })
                    .ToListAsync();
                return listDichVuKhamBenhs;
            }


            //var query = listDichVuKhamBenhs.Select(item => new DichVuGiuongBenhVienTemplateVo
            //{
            //    DisplayName = item.Ma + " - " + item.Ten,
            //    KeyId = item.Id,
            //    Ma = item.Ma,
            //    DichVu = item.Ten,
            //    MaTT37 = item.DichVuGiuong?.MaTT37,
            //    Gia = item.DichVuGiuongBenhVienGiaBenhViens?.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay))?.FirstOrDefault()?.Gia ?? 0,
            //    NhomGiaDichVuGiuongBenhVienId = item.DichVuGiuongBenhVienGiaBenhViens?.Where(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay)).FirstOrDefault()?.NhomGiaDichVuGiuongBenhVienId,
            //    CoGiaBaoHiem = item.DichVuGiuongBenhVienGiaBaoHiems.Any(o => o.TuNgay <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay)),
            //    LoaiGiuong = item.LoaiGiuong
            //}).ToList();
        }
        public async Task<List<VatTuBenhVienTemplateVo>> GetListVatTuYTeBenhVien(DropDownListRequestModel model)
        {
            var listDichVuKhamBenhs = await _vatTuBenhVienRepository.TableNoTracking
                .Include(p => p.VatTus)
                .ApplyLike(model.Query, o => o.VatTus.Ten, o => o.Ma)
                .Where(p => p.VatTus.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")
                      && p.HieuLuc != false
                      //&& p.DichVuGiuongBenhVienGiaBenhViens.Any(X => X.TuNgay <= DateTime.Now.Date && X.DenNgay >= DateTime.Now.Date
                      //                                     || (X.TuNgay <= DateTime.Now.Date && X.DenNgay == null))
                      )
                .Take(model.Take)
                .ToListAsync();

            var query = listDichVuKhamBenhs.Select(item => new VatTuBenhVienTemplateVo
            {
                DisplayName = item.Ma + " - " + item.VatTus.Ten,
                KeyId = item.Id,
                Ma = item.Ma,
                DichVu = item.VatTus.Ten,
                Gia = 10000, // con Update lai
            }).ToList();

            return query;
        }
        public async Task<List<YeuCauDuocPhamBenhVienTemplateVo>> GetListDuocPhamBenhVien(DropDownListRequestModel model)
        {
            //Cần update do chưa có db đầy đủ
            var listDichVuKhamBenhs = await _duocPhamBenhVienService.TableNoTracking
                .Include(p => p.DuocPham)
                //.Include(p => p.DuocPhamBenhVienGiaBaoHiems)
                .ApplyLike(model.Query, o => o.DuocPham.Ten, o => o.Ma)
                //.Where(p => (p.DuocPham.Ten.Contains(model.Query ?? "") || p.DuocPham.MaHoatChat.Contains(model.Query ?? ""))
                //    && p.HieuLuc != false
                //    && p.DuocPhamBenhVienGiaBaoHiems.Any(X => X.TuNgay <= DateTime.Now.Date && X.DenNgay >= DateTime.Now.Date
                //                                             || (X.TuNgay <= DateTime.Now.Date && X.DenNgay == null))
                //)
                .Take(model.Take)
                .ToListAsync();

            var query = listDichVuKhamBenhs.Select(item => new YeuCauDuocPhamBenhVienTemplateVo
            {
                DisplayName = item.Ma + " - " + item.DuocPham.Ten,
                KeyId = item.Id,
                Ma = item.Ma,
                DichVu = item.DuocPham.Ten,
                HoatChat = item.DuocPham.HoatChat,
                SoDangKy = item.DuocPham.SoDangKy,
                Gia = 1000, // chưa biet gia nam o dau
                NhomGiaDuocPhamBenhVienId = 1 // chưa chua co nhom nay
            }).ToList();

            return query;
        }

        #endregion GetLookup

        #region private funciton

        private LoaiDichVuKyThuat GetLoaiDichVuKyThuatChiDinh(EnumDanhMucNhomTheoChiPhi NhomChiPhi)
        {
            switch (NhomChiPhi)
            {
                case EnumDanhMucNhomTheoChiPhi.XetNghiem:
                    return LoaiDichVuKyThuat.XetNghiem;
                case EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh:
                    return LoaiDichVuKyThuat.ChuanDoanHinhAnh;
                case EnumDanhMucNhomTheoChiPhi.ThuThuatPhauThuat:
                    return LoaiDichVuKyThuat.ThuThuatPhauThuat;
                default:
                    return LoaiDichVuKyThuat.Khac;
            }
        }
        private static decimal ToPercentage(int percent)
        {
            return (decimal)(percent) / 100;
        }
        private static decimal TinhTienBaoHiemThanhToan(decimal? giaBaoHiemThanhToan, decimal? tlBaoHiemThanhToan, decimal? mucHuongBaoHiem)
        {
            decimal? bhytthantoan = 0;

            //if(tlBaoHiemThanhToan == 0 && mucHuongBaoHiem != 0)
            //    bhytthantoan = giaBaoHiemThanhToan * mucHuongBaoHiem;

            //if(tlBaoHiemThanhToan != 0 && mucHuongBaoHiem == 0)
            //    bhytthantoan = giaBaoHiemThanhToan * tlBaoHiemThanhToan;

            //if (tlBaoHiemThanhToan == 0 && mucHuongBaoHiem == 0)
            //    bhytthantoan = 0; // bhytthantoan = giaBaoHiemThanhToan * tlBaoHiemThanhToan * mucHuongBaoHiem;

            //if (tlBaoHiemThanhToan != 0 && mucHuongBaoHiem != 0)
            bhytthantoan = giaBaoHiemThanhToan * tlBaoHiemThanhToan * mucHuongBaoHiem;

            return bhytthantoan ?? 0;
        }
        private static decimal TinhSoTienMG(decimal? bhytthantoan, decimal? tLMG)
        {
            decimal? SoTienMG = 0;
            SoTienMG = bhytthantoan * tLMG;
            return SoTienMG ?? 0;
        }

        #endregion
        private string AddPhieuInDichVu(string content, Enums.LoaiDichVuKyThuat loaiDichVuKyThuat, long yeuCauTiepNhanId, long yeuCauKhamBenhId, string hostingName, List<ListDichVuChiDinh> lst, string ghiChuCLS)
        {
            var yeuCauKham = _yeuCauKhamBenhRepository.TableNoTracking
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                         .Include(p=>p.YeuCauDichVuKyThuats)?.ThenInclude(p=>p.YeuCauKhamBenh)?.ThenInclude(p=>p.ChanDoanSoBoICD)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)

                         .Include(p => p.ChanDoanSoBoICD)

                         .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.NguoiLienHeQuanHeNhanThan)
                         .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                         .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                         .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
                         .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
                         .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                         .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
                         .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                         .Include(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                         //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
                         .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
                         .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                         .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                         .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc).ThenInclude(p => p.KhoaPhong)
                         .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

                         .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
                         .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                         .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                         .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu).ThenInclude(p => p.KhoaPhong)
                         .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)
                         .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.BenhNhan)
                         .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                         .Include(p => p.YeuCauTiepNhan).ThenInclude(cc => cc.PhuongXa)
                         .Include(p => p.YeuCauTiepNhan).ThenInclude(cc => cc.QuanHuyen)
                         .Include(p => p.YeuCauTiepNhan).ThenInclude(cc => cc.TinhThanh)
                         .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham);

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            foreach (var itemYeuCauKham in yeuCauKham)
            {
                listDVKT.AddRange(itemYeuCauKham.YeuCauDichVuKyThuats.ToList());
            }
            List<YeuCauDichVuGiuongBenhVien> listDVGiuong = new List<YeuCauDichVuGiuongBenhVien>();

            foreach (var itemYeuCauKham in yeuCauKham)
            {
                listDVGiuong.AddRange(itemYeuCauKham.YeuCauDichVuGiuongBenhViens.ToList());
            }

            var dienGiaiChanDoanSoBo = new List<string>(); 
            var chanDoanSoBos = new List<string>();

            long userId = _userAgentHelper.GetCurrentUserId();
            var tenNguoiChiDinh = _userRepository.TableNoTracking.Where(p => p.Id == userId)
               .Select(p => p.HoTen)
               .FirstOrDefault();
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
            //htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>VP</th>";
            //htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>BHYT</th>";
            htmlDanhSachDichVu += "</tr>";
            var i = 1;
            int indexDVKT = 1;
            if (lst.Count() == 1)
            {
                foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                               && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.Id == lst.First().dichVuChiDinhId))
                {
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    isHave = true;
                    if(ycdvkt.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                    {
                        dienGiaiChanDoanSoBo.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);
                        if(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                        {
                            chanDoanSoBos.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet);
                        }
                       
                    }
                    if (ycdvkt.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                    {
                        dienGiaiChanDoanSoBo.Add(ycdvkt.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu);
                        if(ycdvkt.NoiTruPhieuDieuTri.ChanDoanChinhICD != null)
                        {
                            chanDoanSoBos.Add(ycdvkt.NoiTruPhieuDieuTri.ChanDoanChinhICD?.Ma + "-" + ycdvkt.NoiTruPhieuDieuTri.ChanDoanChinhICD?.TenTiengViet);
                        }
                        
                    }

                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                    htmlDanhSachDichVu += " </tr>";
                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                    //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    htmlDanhSachDichVu += " </tr>";
                    i++;
                    indexDVKT++;
                }

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.MaYeuCauTiepNhan,
                    MaBN = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BenhNhan != null ? yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BenhNhan.MaBN : "",
                    HoTen = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.HoTen ?? "",
                    GioiTinhString = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.DiaChiDayDu,
                    //Ngay = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.ThoiDiemTiepNhan.ToString("dd"),
                    //Thang = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.ThoiDiemTiepNhan.ToString("MM"),
                    //Nam = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.ThoiDiemTiepNhan.ToString("yyyy"),
                    Ngay = DateTime.Now.Day,
                    Thang = DateTime.Now.Month,
                    Nam = DateTime.Now.Year,
                    DienThoai = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauKham.FirstOrDefault().YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTMaSoThe,
                    HanThe = (yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTNgayHieuLuc != null || yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                    //Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //NowTime = DateTime.Now.ApplyFormatTime(),,
                    NoiYeuCau =  tenPhong,
                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,
                    PhieuThu = "DichVuKyThuat",
                    GhiChuCanLamSang = ghiChuCLS,
                    ChuanDoanSoBo = chanDoanSoBos.Where(s => s != null).ToList().Join(";"),
                    DienGiai = dienGiaiChanDoanSoBo.Where(s=>s != null ).ToList().Join(";")
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
                foreach (var itx in lst)
                {
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                            && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {
                            var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                            isHave = true;
                            if (indexDVKT == 1)
                            {
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                            }

                            // chẩn đoán sơ bộ và diễn giải
                            if (ycdvkt.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                            {
                                dienGiaiChanDoanSoBo.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);
                                if (ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                                {
                                    chanDoanSoBos.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet);
                                }

                            }
                            if (ycdvkt.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                            {
                                dienGiaiChanDoanSoBo.Add(ycdvkt.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu);
                                if (ycdvkt.NoiTruPhieuDieuTri.ChanDoanChinhICD != null)
                                {
                                    chanDoanSoBos.Add(ycdvkt.NoiTruPhieuDieuTri.ChanDoanChinhICD?.Ma + "-" + ycdvkt.NoiTruPhieuDieuTri.ChanDoanChinhICD?.TenTiengViet);
                                }

                            }



                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            indexDVKT++;
                            nhomDichVu = "";
                        }


                    }

                }
                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.MaYeuCauTiepNhan,
                    MaBN = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BenhNhan != null ? yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BenhNhan.MaBN : "",
                    HoTen = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.HoTen ?? "",
                    GioiTinhString = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.DiaChiDayDu,
                    Ngay = DateTime.Now.Day,
                    Thang = DateTime.Now.Month,
                    Nam = DateTime.Now.Year,
                    DienThoai = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauKham?.FirstOrDefault().YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTMaSoThe,
                    HanThe = (yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTNgayHieuLuc != null || yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                    //Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //NowTime = DateTime.Now.ApplyFormatTime(),,
                    NoiYeuCau =  tenPhong,
                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauKham.FirstOrDefault().YeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauKham?.FirstOrDefault().YeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,
                    PhieuThu = "DichVuKyThuat",
                    GhiChuCanLamSang = ghiChuCLS,
                    ChuanDoanSoBo = chanDoanSoBos.Where(s => s != null).ToList().Join(";"),
                    DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null).ToList().Join(";")
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

        public string InBaoCaoChiDinh(long yeuCauTiepNhanId, long yeuCauKhamBenhId, string hostingName, List<ListDichVuChiDinh> lst, long inChungChiDinh, bool KieuInChung, string ghiChuCLS, bool isKhamDoanTatCa,bool? inDichVuBacSiChiDinh)

        {
            long userId = _userAgentHelper.GetCurrentUserId();
            var nguoiChiDinh = _userRepository.GetById(userId);
            var content = "";


            var listSarsCov2CauHinh = GetListSarsCauHinh();

            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)

            //            .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)

            //            //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)

            //            .Include(p => p.BenhNhan)
            //            .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //            .Include(cc => cc.PhuongXa)
            //            .Include(cc => cc.QuanHuyen)
            //            .Include(cc => cc.TinhThanh)
            //            .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            var thongTinTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.Id,
                    DichVuKhams = o.YeuCauKhamBenhs.Select(kham => new { kham.Id, kham.TrangThai}).ToList(),
                    DichVuKyThuats = o.YeuCauDichVuKyThuats.Select(kt => new { kt.Id, kt.DichVuKyThuatBenhVienId, kt.TrangThai }).ToList(),
                })
                .FirstOrDefault();


            if (isKhamDoanTatCa == true)
            {
                //List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

                //listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.ToList());

                //List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

                //listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(d=>!listSarsCov2CauHinh.Contains(d.DichVuKyThuatBenhVienId)).ToList());

                var tenNguoiChiDinh = _userRepository.TableNoTracking.Where(p => p.Id == userId)
               .Select(p => p.HoTen)
               .FirstOrDefault();
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
                        content = InChiDinhInChungKhamDoanTatCa(yeuCauTiepNhanId,ghiChuCLS,true,lst,hostingName,content); // in chung theo nhóm và người chỉ định
                        
                    }
                    else
                    { 
                        content = InChiDinhInChungKhamDoanTatCa(yeuCauTiepNhanId, ghiChuCLS, false, lst, hostingName, content); // in chung theo stt và người chỉ định

                    }

                }
                // update xong dịch vụ giường khám đoàn
                //in tung phiếu
                if (inChungChiDinh == 0)
                {
                    //if (listDVK != null || listDVKT != null)
                    //{
                    //    content = InChiDinhInTungPhieuKhamDoanTatCa(yeuCauTiepNhanId,ghiChuCLS,lst,hostingName);
                    //}
                    content = InChiDinhInTungPhieuKhamDoanTatCa(yeuCauTiepNhanId, ghiChuCLS, lst, hostingName);
                }
            }
            if (isKhamDoanTatCa == false)
            {
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
                        content = InChiDinhInChungTatCa(yeuCauTiepNhanId, yeuCauKhamBenhId, ghiChuCLS, true, lst, hostingName, content, inDichVuBacSiChiDinh); // in chung theo nhóm và người chỉ định
                    }
                    else
                    {
                        content = InChiDinhInChungTatCa(yeuCauTiepNhanId, yeuCauKhamBenhId, ghiChuCLS, false, lst, hostingName, content, inDichVuBacSiChiDinh); // in chung theo stt và người chỉ định

                    }

                }

                // in tung phiếu
                if (inChungChiDinh == 0)
                {
                    content = InChiDinhInTungPhieuTatCa(yeuCauTiepNhanId, yeuCauKhamBenhId, ghiChuCLS, lst, hostingName, inDichVuBacSiChiDinh);
                }
            }
            return content;
        }


        #region  chỉ định  khám đoàn  
        // in cung nhóm (nhiều nhóm) cùng người chỉ định cùng 1 phiếu **
        private string AddChiDinhTheoNguoiChiDinhVaNhom(long yeuCauTiepNhanId, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh, 
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT , string content,string ghiChuCLS, string hostingName)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p=>p.HocHamHocVi)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p=>p.ChucDanh)?.ThenInclude(p=>p.NhomChucDanh)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)

            //            .Include(p => p.NguoiLienHeQuanHeNhanThan)
                      
            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //            .Include(p => p.BenhNhan)
            //            .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //            .Include(cc => cc.PhuongXa)
            //            .Include(cc => cc.QuanHuyen)
            //            .Include(cc => cc.TinhThanh)
            //            .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();

            // chẩn đoán sơ 
            var chanDoanSoBo = string.Empty;
            // diễn giải
            var dienGiai = string.Empty;
            var valueCD = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhKhamSucKhoe.IcdKhamSucKhoe").Select(d => d.Value).ToList();
            if (valueCD.Any())
            {
                var query = _iCDRepository.TableNoTracking.Where(d => d.Id == long.Parse(valueCD.First()))
                    .Select(d => new {
                        cd = d.Ma + "-" + d.TenTiengViet,
                        dg = d.TenTiengViet
                    }).ToList();
                chanDoanSoBo = query.Any() ? query.First().cd : "";
                dienGiai = query.Any() ? query.First().dg : "";
            }


            var lstInThuTuTheoNhomDichVu = listDichVuTheoNguoiChiDinh.First().nhomChiDinhId; // kiểm tra list đầu tiền là dịch vụ gì in nhóm dịch vụ đó trước

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            // từng item phiếu in theo người  chỉ định => tất cả dịch vụ khám bệnh và dịch vụ kỹ thuật đều cùng 1  người chỉ định
            var nhanVienChiDinh = "";
            // tên người chỉ định theo phiếu in 
            string ngay = "";
            string thang = "";
            string nam = "";
            if (lstInThuTuTheoNhomDichVu == (long)Enums.EnumNhomGoiDichVu.DichVuKyThuat)
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
                var lstDVKT = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat);
                foreach (var itx in lstDVKT)
                {
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))   // lấy data Dịch vụ kỹ thuật dựa vào nhóm id và dichVuid trong list Ui trả lên
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {

                            listInDichVuKyThuat.Add(ycdvkt);
                        }

                    }
                }
                List<ListDichVuChiDinh> lstDichVuCungChidinh = new List<ListDichVuChiDinh>();
                List<ListDichVuChiDinh> lstDichVuChidinhTungPhieu = new List<ListDichVuChiDinh>();

                List<ListDichVuChiDinh> lstDichVuChidinh = new List<ListDichVuChiDinh>();
                foreach (var ycdvkt in listInDichVuKyThuat)
                {
                    var lstDichVuKT = new ListDichVuChiDinh();
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    lstDichVuKT.TenNhom = nhomDichVu;
                    lstDichVuKT.nhomChiDinhId = ycdvkt.NhomDichVuBenhVien.Id;
                    lstDichVuKT.dichVuChiDinhId = ycdvkt.Id;
                    lstDichVuChidinh.Add(lstDichVuKT);
                }
                var lstdvkt = listDVKT.Where(o => o.DichVuKyThuatBenhVien != null);

                decimal tongCong = 0;
                int soLuong = 0;
                foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
                {
                    if (dv.Count() > 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.dichVuChiDinhId).ToList();
                        var thanhTienDVKT = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();
                        CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormatDVKT = string.Format(culDVKT, "{0:n0}", thanhTienDVKT);
                        tongCong += thanhTienDVKT.GetValueOrDefault(); 

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Any())
                            {
                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d=>d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId);
                                if (maHocHamViId.Any(d=>d != null))
                                {
                                    maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId.First()).Select(d => d.Ma).FirstOrDefault();
                                }
                                nhanVienChiDinh = returnStringTen(maHocHamVi,"",
                                                                  lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NhanVienChiDinh != null ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NhanVienChiDinh.User.HoTen : "");
                                if (indexDVKT == 1)
                                {
                                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormatDVKT}</b></td>";
                                    htmlDanhSachDichVu += " </tr>";
                                }
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                    if (dv.Count() == 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.dichVuChiDinhId).ToList();
                        var thanhTienDVKT = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();
                        CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormatDVKT = string.Format(culDVKT, "{0:n0}", thanhTienDVKT);
                        tongCong += thanhTienDVKT.GetValueOrDefault();

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First() != null)
                            {
                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId);
                                if (maHocHamViId.Any(d=>d != null))
                                {
                                    maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId.First()).Select(d => d.Ma).FirstOrDefault();
                                }

                                nhanVienChiDinh = returnStringTen(maHocHamVi,
                                                                 "",
                                                                 lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NhanVienChiDinh != null ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NhanVienChiDinh.User.HoTen : "");
                                //var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormatDVKT}</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                }


                //DỊCH VỤ KHÁM BỆNH
                var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                int indexDVKB = 1;
                var listInDichVuKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
                foreach (var itx in lstDVKB)
                {
                    var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                       && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                        ).OrderBy(x => x.CreatedOn); // to do nam ho;

                    if (lstYeuCauKhamBenhChiDinh != null)
                    {
                        foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                        {
                            if (itx.dichVuChiDinhId == yckb.Id)
                            {
                                listInDichVuKhamBenh.Add(yckb); // lấy data Dịch vụ khám bệnh dựa vào nhóm id và dichVuid trong list Ui trả lên
                            }
                        }
                    }
                }
                
                // BVHD-3939 // == 1 
                if(listInDichVuKhamBenh.Count() != 0)
                {
                    var thanhTienDv = listInDichVuKhamBenh
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                    .FirstOrDefault();
                    CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                    var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                    tongCong += thanhTienDv.GetValueOrDefault();



                    foreach (var yckb in listInDichVuKhamBenh)
                    {
                        ngay = yckb.ThoiDiemDangKy.Day.ToString();
                        thang = yckb.ThoiDiemDangKy.Month.ToString();
                        nam = yckb.ThoiDiemDangKy.Year.ToString();

                        var maHocHamVi = string.Empty;
                        var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                        if (maHocHamViId != null)
                        {
                            maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                        }

                        nhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);
                        if (indexDVKB == 1)
                        {
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                            htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                            htmlDanhSachDichVu += " </tr>";
                        }
                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                        htmlDanhSachDichVu += " </tr>";
                        i++;
                        indexDVKB++;
                        soLuong++;
                    }
                    // từng item phiếu in theo người  chỉ định => tất cả dịch vụ khám bệnh và dịch vụ kỹ thuật đều cùng 1  người chỉ định
                }



                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan.MaBN,
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
                    NoiYeuCau =  tenPhong,
                    ChuanDoanSoBo = chanDoanSoBo, // fist đâu tiền dịch vụ != hủy và gói khám != null
                    DienGiai = dienGiai,
                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = nhanVienChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                    GhiChuCanLamSang = ghiChuCLS,
                    NgayThangNam =DateTime.Now.ApplyFormatDateTimeSACH()
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
            if (lstInThuTuTheoNhomDichVu == (long)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                var isHave = false;
                var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
                htmlDanhSachDichVu += "</tr>";
                var i = 1;

                //DỊCH VỤ KHÁM BỆNH

                var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                int indexDVKB = 1;
                var listInDichVuKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
                foreach (var itx in lstDVKB)
                {
                    var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                     && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                      ).OrderBy(x => x.CreatedOn); // to do nam ho;

                    if (lstYeuCauKhamBenhChiDinh != null)
                    {
                        foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                        {
                            if (itx.dichVuChiDinhId == yckb.Id)
                            {
                                listInDichVuKhamBenh.Add(yckb);
                            }
                        }
                    }
                }

                decimal tongCong = 0;
                int soLuong = 0;
                // BVHD-3939 // == 1 
                var thanhTienDv = listInDichVuKhamBenh
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                    .FirstOrDefault();
                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
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

                    nhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>"; 
                    htmlDanhSachDichVu += " </tr>";
                    i++;
                    indexDVKB++;
                }

                // DỊCH VỤ KỸ THUẬT

                int indexDVKT = 1;
                var listInDichVuKyThuat = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat>();
                var lstDVKT = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat);
                foreach (var itx in lstDVKT)
                {
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {

                            listInDichVuKyThuat.Add(ycdvkt);
                        }

                    }
                }
                List<ListDichVuChiDinh> lstDichVuCungChidinh = new List<ListDichVuChiDinh>();
                List<ListDichVuChiDinh> lstDichVuChidinhTungPhieu = new List<ListDichVuChiDinh>();

                List<ListDichVuChiDinh> lstDichVuChidinh = new List<ListDichVuChiDinh>();
                foreach (var ycdvkt in listInDichVuKyThuat)
                {
                    var lstDichVuKT = new ListDichVuChiDinh();
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    lstDichVuKT.TenNhom = nhomDichVu;
                    lstDichVuKT.nhomChiDinhId = ycdvkt.NhomDichVuBenhVien.Id;
                    lstDichVuKT.dichVuChiDinhId = ycdvkt.Id;
                    lstDichVuChidinh.Add(lstDichVuKT);
                }
                foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
                {

                    if (dv.Count() > 1)
                    {
                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                        {
                            if (dv.Where(p => p.dichVuChiDinhId == ycdvkt.Id).Any())
                            {
                                ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                                thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                                nam = ycdvkt.ThoiDiemDangKy.Year.ToString();

                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                if (indexDVKT == 1)
                                {
                                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                                    htmlDanhSachDichVu += " </tr>";
                                }

                                var maHocHamVi = string.Empty;
                                var maHocHamViId = ycdvkt?.NhanVienChiDinh?.HocHamHocViId;
                                if (maHocHamViId != null)
                                {
                                    maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                                }

                                nhanVienChiDinh = returnStringTen(maHocHamVi, "", ycdvkt.NhanVienChiDinh?.User?.HoTen);

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                nhomDichVu = "";
                            }
                        }
                        indexDVKT = 1;
                    }
                    if (dv.Count() == 1)
                    {
                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                        {
                            if (dv.Where(p => p.dichVuChiDinhId == ycdvkt.Id).Any())
                            {
                                var maHocHamVi = string.Empty;
                                var maHocHamViId = ycdvkt?.NhanVienChiDinh?.HocHamHocViId;
                                if (maHocHamViId != null)
                                {
                                    maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                                }


                                nhanVienChiDinh = returnStringTen(maHocHamVi,"", ycdvkt.NhanVienChiDinh?.User?.HoTen);

                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                nhomDichVu = "";
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
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan.MaBN,
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
                    NoiYeuCau =  tenPhong,
                    ChuanDoanSoBo = chanDoanSoBo, // fist đâu tiền dịch vụ != hủy và gói khám != null
                    DienGiai = dienGiai,
                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = nhanVienChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                    GhiChuCanLamSang = ghiChuCLS,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH()
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
        // in chung tất cả dịch dụ theo stt theo người chỉ định *
        private string AddTungPhieuTheoNguoiChiDinhVaTheoSTT(long yeuCauTiepNhanId, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string ghiChuCLS, string hostingName)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)

            //            .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //            //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)

            //            .Include(p => p.BenhNhan)
            //            .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //            .Include(cc => cc.PhuongXa)
            //            .Include(cc => cc.QuanHuyen)
            //            .Include(cc => cc.TinhThanh)
            //            .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();

            // chẩn đoán sơ 
            var chanDoanSoBo = string.Empty;
            // diễn giải
            var dienGiai = string.Empty;
            var valueCD = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhKhamSucKhoe.IcdKhamSucKhoe").Select(d => d.Value).ToList();
            if (valueCD.Any())
            {
                var query = _iCDRepository.TableNoTracking.Where(d => d.Id == long.Parse(valueCD.First()))
                    .Select(d => new {
                        cd = d.Ma + "-" + d.TenTiengViet,
                        dg = d.TenTiengViet
                    }).ToList();
                chanDoanSoBo = query.Any() ? query.First().cd : "";
                dienGiai = query.Any() ? query.First().dg : "";
            }


            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
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
            List<ListDichVuChiDinh> lstDichVuChidinhTheoSoThuTu = new List<ListDichVuChiDinh>();

            var lstDVKB = listDVK.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).OrderBy(x => x.CreatedOn);
            var lstdvkt = listDVKT.Where(o => o.DichVuKyThuatBenhVien != null);
            string ngay = "";
            string thang = "";
            string nam = "";
            var tenNhanVienChiDinh = "";
            decimal tongCong = 0;
            int soLuong = 0;
            if (listDichVuTheoNguoiChiDinh.Count() > 0)
            {
                // BVHD-3939 // == 1 
                var listDichVuIds = listDichVuTheoNguoiChiDinh.Select(d => d.dichVuChiDinhId).ToList();
                var thanhTienDv = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                    .Sum();
                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();


                foreach (var Itemx in listDichVuTheoNguoiChiDinh)
                {

                    if (Itemx.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                    {
                        if (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Any())
                        {
                            ngay = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.ThoiDiemDangKy.Day.ToString()).First();
                            thang = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.ThoiDiemDangKy.Month.ToString()).First();
                            nam = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.ThoiDiemDangKy.Year.ToString()).First();

                            var maHocHamVi = string.Empty;
                            var maHocHamViId = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId);
                            if (maHocHamViId.Any(d=>d != null))
                            {
                                maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId.First()).Select(d => d.Ma).FirstOrDefault();
                            }

                            tenNhanVienChiDinh = returnStringTen(maHocHamVi,
                                                                 "",
                                                                 lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).FirstOrDefault());



                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().SoLan + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            soLuong += lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().SoLan;
                        }
                    }
                    if (Itemx.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                    {
                        if (lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Any())
                        {
                            ngay = lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s=>s.ThoiDiemDangKy.Day.ToString()).First();
                            thang = lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.ThoiDiemDangKy.Month.ToString()).First();
                            nam = lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.ThoiDiemDangKy.Year.ToString()).First();

                            var maHocHamVi = string.Empty;
                            var maHocHamViId = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId);
                            if (maHocHamViId.Any(d=>d != null))
                            {
                                maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId.First()).Select(d => d.Ma).FirstOrDefault();
                            }

                            tenNhanVienChiDinh = returnStringTen(maHocHamVi,
                                                                 "",
                                                                 lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).FirstOrDefault());


                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).First().TenDichVu + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiDangKy != null ? lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiDangKy?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>"; 
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            soLuong++;
                        }
                    }
                }
            }
        

            var data = new
            {
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan.MaBN,
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
                NoiYeuCau =  tenPhong,
                ChuanDoanSoBo = chanDoanSoBo, // cấu hình
                DienGiai = dienGiai,
                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = tenNhanVienChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                GhiChuCanLamSang = ghiChuCLS,
                NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH()
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

        // in từng phiếu theo người chỉ định  **
        private string AddPhieuInDichVuChiDinhTheoNguoiChiDinh(List<YeuCauDichVuKyThuat> listAllDVKT, string content, Enums.LoaiDichVuKyThuat loaiDichVuKyThuat, long yeuCauTiepNhanId, string hostingName, List<ListDichVuChiDinhTheoNguoiChiDinh> lst, string ghiChuCLS)
        {
            var listSarsCov2CauHinh = GetListSarsCauHinh();

            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)

            //             .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


            //             .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //             .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //             .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //             //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc).ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu).ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)

            //             .Include(p => p.BenhNhan)
            //             .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //             .Include(cc => cc.PhuongXa)
            //             .Include(cc => cc.QuanHuyen)
            //             .Include(cc => cc.TinhThanh)
            //             .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();


            // chẩn đoán sơ 
            var chanDoanSoBo = string.Empty;
            // diễn giải
            var dienGiai = string.Empty;
            var valueCD = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhKhamSucKhoe.IcdKhamSucKhoe").Select(d => d.Value).ToList();
            if (valueCD.Any())
            {
                var query = _iCDRepository.TableNoTracking.Where(d => d.Id == long.Parse(valueCD.First()))
                    .Select(d => new {
                        cd = d.Ma + "-" + d.TenTiengViet,
                        dg = d.TenTiengViet
                    }).ToList();
                chanDoanSoBo = query.Any() ? query.First().cd : "";
                dienGiai = query.Any() ? query.First().dg : "";
            }

            List<YeuCauDichVuKyThuat> listDVKT = listAllDVKT.Where(d => !listSarsCov2CauHinh.Contains(d.DichVuKyThuatBenhVienId)).ToList();

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
                var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();

                foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                               && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.Id == lst.First().dichVuChiDinhId))
                {
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    isHave = true;

                    var maHocHamVi = string.Empty;
                    var maHocHamViId = ycdvkt?.NhanVienChiDinh?.HocHamHocViId;
                    if (maHocHamViId != null)
                    {
                        maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                    }

                    tenNguoiChiDinh = returnStringTen(maHocHamVi,"", ycdvkt.NhanVienChiDinh?.User?.HoTen);

                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienDv}</b></td>";
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
                }
                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan?.MaBN,
                    HoTen = yeuCauTiepNhan.HoTen ?? "",
                    GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                    Ngay = listDVKT.Any(o => o.DichVuKyThuatBenhVien != null 
                                              && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                              && o.Id == lst.First().dichVuChiDinhId) ?
                                                                                            listDVKT.Where(o => o.DichVuKyThuatBenhVien != null 
                                                                                                        && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                                                                                        && o.Id == lst.First().dichVuChiDinhId)
                                                                                                   .Select(s=>s.ThoiDiemDangKy.Day).First() : 0,
                                                                                                   
                    Thang = listDVKT.Any(o => o.DichVuKyThuatBenhVien != null
                                              && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                              && o.Id == lst.First().dichVuChiDinhId) ?
                                                                                            listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                                        && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                                        && o.Id == lst.First().dichVuChiDinhId)
                                                                                                   .Select(s => s.ThoiDiemDangKy.Month).First() : 0,
                    Nam = listDVKT.Any(o => o.DichVuKyThuatBenhVien != null
                                               && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                               && o.Id == lst.First().dichVuChiDinhId) ?
                                                                                            listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                                        && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                                        && o.Id == lst.First().dichVuChiDinhId)
                                                                                                   .Select(s => s.ThoiDiemDangKy.Year).First() : 0,
                    DienThoai = yeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = yeuCauTiepNhan?.BHYTMaSoThe,
                    HanThe = (yeuCauTiepNhan?.BHYTNgayHieuLuc != null || yeuCauTiepNhan?.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan?.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan?.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                    //Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //NowTime = DateTime.Now.ApplyFormatTime(),,
                    NoiYeuCau =  tenPhong,
                    ChuanDoanSoBo = chanDoanSoBo, // fist đâu tiền dịch vụ != hủy và gói khám != null
                    DienGiai = dienGiai,
                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan?.TenQuanHeThanNhan,
                    PhieuThu = "DichVuKyThuat",
                    GhiChuCanLamSang = ghiChuCLS,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH()
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
                string ngay = "";
                string thang = "";
                string nam = "";

                decimal tongCong = 0;
                int soLuong = 0;
                // BVHD-3939 // == 1 
                var listDichVuIds = lst.Select(d => d.dichVuChiDinhId).ToList();
                var thanhTienDv = listDVKT.Where(d => listDichVuIds.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                    .Sum();

                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();

                foreach (var itx in lst)
                {
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                            && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {
                            ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                            thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                            nam = ycdvkt.ThoiDiemDangKy.Year.ToString();

                            var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                            isHave = true;
                            if (indexDVKT == 1)
                            {
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                            }
                            var maHocHamVi = string.Empty;
                            var maHocHamViId = ycdvkt?.NhanVienChiDinh?.HocHamHocViId;
                            if (maHocHamViId != null)
                            {
                                maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                            }

                            tenNguoiChiDinh = returnStringTen(maHocHamVi, "", ycdvkt.NhanVienChiDinh?.User?.HoTen);
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: right;'></td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            indexDVKT++;
                            nhomDichVu = "";
                            soLuong += ycdvkt.SoLan;
                        }


                    }

                }
                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939
                var data = new 
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan?.MaBN,
                    HoTen = yeuCauTiepNhan.HoTen ?? "",
                    GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                    Ngay = ngay,

                    Thang = thang,
                    Nam = nam,
                    DienThoai = yeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = yeuCauTiepNhan?.BHYTMaSoThe,
                    HanThe = (yeuCauTiepNhan?.BHYTNgayHieuLuc != null || yeuCauTiepNhan?.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan?.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan?.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                    //Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //NowTime = DateTime.Now.ApplyFormatTime(),,
                    NoiYeuCau = tenPhong,
                    ChuanDoanSoBo = chanDoanSoBo, // fist đâu tiền dịch vụ != hủy và gói khám != null
                    DienGiai = dienGiai,
                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan?.TenQuanHeThanNhan,
                    PhieuThu = "DichVuKyThuat",
                    GhiChuCanLamSang = ghiChuCLS,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH()
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

        // in theo từng phiếu nhóm  theo người chỉ định **
        private string AddTungPhieuTheoNguoiChiDinh(long yeuCauTiepNhanId, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
          List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
          List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string ghiChuCLS, string hostingName)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)
            //             .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)

            //             .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


            //             .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //             .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //             //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc).ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu).ThenInclude(p => p.KhoaPhong)
            //             .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)

            //             .Include(p => p.BenhNhan)
            //             .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //             .Include(cc => cc.PhuongXa)
            //             .Include(cc => cc.QuanHuyen)
            //             .Include(cc => cc.TinhThanh)
            //             .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();

            // chẩn đoán sơ 
            var chanDoanSoBo = string.Empty;
            // diễn giải
            var dienGiai = string.Empty;
            var valueCD = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhKhamSucKhoe.IcdKhamSucKhoe").Select(d => d.Value).ToList();
            if (valueCD.Any())
            {
                var query = _iCDRepository.TableNoTracking.Where(d => d.Id == long.Parse(valueCD.First()))
                    .Select(d => new {
                        cd = d.Ma + "-" + d.TenTiengViet,
                        dg = d.TenTiengViet
                    }).ToList();
                chanDoanSoBo = query.Any() ? query.First().cd : "";
                dienGiai = query.Any() ? query.First().dg : "";
            }



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
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
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
                                content = AddPhieuInDichVuChiDinhTheoNguoiChiDinh(listDVKT, content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhanId, hostingName, lstDichVuCungChidinhXN, ghiChuCLS);
                            }
                            else if (itemIn.Count() > 1)
                            {
                                List<ListDichVuChiDinhTheoNguoiChiDinh> lstDichVuCungChidinhXN = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                                lstDichVuCungChidinhXN.AddRange(itemIn);
                                content = AddPhieuInDichVuChiDinhTheoNguoiChiDinh(listDVKT, content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhanId, hostingName, lstDichVuCungChidinhXN, ghiChuCLS);
                            }
                        }
                    }


                    var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);

                   

                    if (lstDVKB.Any())
                    {
                        string ngay = "";
                        string thang = "";
                        string nam = "";
                        if (listDichVuTheoNguoiChiDinh.Count() == 1)
                        {
                            decimal tongCong = 0;
                            int soLuong = 0;
                            // BVHD-3939 // == 1 
                            var listDichVuIds = lstDVKB.Select(d => d.dichVuChiDinhId).ToList();
                            var thanhTienDv = listDVK.Where(d => listDichVuIds.Contains(d.Id))
                                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                .FirstOrDefault();
                            CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                            var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                            tongCong += thanhTienDv.GetValueOrDefault();

                            var lstYeuCauKhamBenhChiDinh = listDVK.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && x.Id == listDichVuTheoNguoiChiDinh.First().dichVuChiDinhId).OrderBy(x => x.CreatedOn); // to do nam ho;
                            if (lstYeuCauKhamBenhChiDinh != null)
                            {
                                int indexDVKT = 1;
                                foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                                {
                                    ngay = yckb.ThoiDiemDangKy.Day.ToString();
                                    thang = yckb.ThoiDiemDangKy.Month.ToString();
                                    nam = yckb.ThoiDiemDangKy.Year.ToString();

                                    var maHocHamVi = string.Empty;
                                    var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                                    if (maHocHamViId != null)
                                    {
                                        maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                                    }

                                    tenNhanVienChiDinh = returnStringTen(maHocHamVi,"", yckb.NhanVienChiDinh?.User?.HoTen);

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
                                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                                htmlDanhSachDichVu += " </tr>";
                                // end BVHD-3939

                                var data = new
                                {
                                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = yeuCauTiepNhan.MaBN,
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
                                    NoiYeuCau =  tenPhong,
                                    ChuanDoanSoBo = chanDoanSoBo, // fist đâu tiền dịch vụ != hủy và gói khám != null
                                    DienGiai = dienGiai,
                                    DanhSachDichVu = htmlDanhSachDichVu,
                                    NguoiChiDinh = tenNhanVienChiDinh,
                                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                                    PhieuThu = "YeuCauKhamBenh",
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH()
                                };


                                if (data.PhieuThu == "YeuCauKhamBenh")
                                {
                                    var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
                                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                    {
                                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                        content = content.Replace(tmpKB, tampKB);
                                    }
                                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                    content = content.Replace(tmp, tamp);
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
                            // BVHD-3939 // == 1 
                            var listDichVuIds = lstDVKB.Select(d => d.dichVuChiDinhId).ToList();
                            var thanhTienDv = listDVK.Where(d => listDichVuIds.Contains(d.Id))
                                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                .FirstOrDefault();
                            CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                            var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                            tongCong += thanhTienDv.GetValueOrDefault();

                            foreach (var itx in lstDVKB)
                            {

                                if (listDVK != null)
                                {
                                    int indexDVK = 1;
                                    foreach (var yckb in listDVK)
                                    {
                                        if (itx.dichVuChiDinhId == yckb.Id)
                                        {
                                            ngay = yckb.ThoiDiemDangKy.Day.ToString();
                                            thang = yckb.ThoiDiemDangKy.Month.ToString();
                                            nam = yckb.ThoiDiemDangKy.Year.ToString();

                                            var maHocHamVi = string.Empty;
                                            var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                                            if (maHocHamViId != null)
                                            {
                                                maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                                            }

                                            tenNhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                            htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                                            htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right'><b>{thanhTienFormat}</b></td>";
                                            htmlDanhSachDichVu += " </tr>";

                                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>"; 
                                            htmlDanhSachDichVu += " </tr>";
                                            i++;
                                            indexDVK++;
                                            soLuong++;
                                        }
                                    }
                                }
                                // BVHD-3939- page -total
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                                // BVHD-3939 - số lượng
                                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                                htmlDanhSachDichVu += " </tr>";
                                // end BVHD-3939

                                var data = new
                                {
                                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = yeuCauTiepNhan.MaBN,
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
                                    NoiYeuCau =  tenPhong,
                                    ChuanDoanSoBo = chanDoanSoBo, // fist đâu tiền dịch vụ != hủy và gói khám != null
                                    DienGiai = dienGiai,
                                    DanhSachDichVu = htmlDanhSachDichVu,
                                    NguoiChiDinh = tenNhanVienChiDinh,
                                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                                    PhieuThu = "YeuCauKhamBenh",
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH()
                                };


                                if (data.PhieuThu == "YeuCauKhamBenh")
                                {
                                    var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
                                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                    {
                                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                        content = content.Replace(tmpKB, tampKB);
                                    }
                                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                    content = content.Replace(tmp, tamp);
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
                if (listInThuTuGDV == (long)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                    string Ngay;
                    string Thang;
                    string Nam;
                    //=> in theo người chỉ định và theo thòi gian chỉ định chỉ có in và dịch vụ chỉ có 1  => ngày tháng năm luôn luôn 1 thời điểm 
                    if (lstDVKB.Any())
                    {
                        if (listDichVuTheoNguoiChiDinh.Count() == 1)
                        {
                            decimal tongCong = 0;
                            int soLuong = 0;
                            // BVHD-3939 // == 1 
                            var listDichVuIds = lstDVKB.Select(d => d.dichVuChiDinhId).ToList();
                            var thanhTienDv = listDVK.Where(d => listDichVuIds.Contains(d.Id))
                                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                .FirstOrDefault();
                            CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                            var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                            tongCong += thanhTienDv.GetValueOrDefault();

                            var lstYeuCauKhamBenhChiDinh = listDVK.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && x.Id == listDichVuTheoNguoiChiDinh.First().dichVuChiDinhId).OrderBy(x => x.CreatedOn); // to do nam ho;
                            if (lstYeuCauKhamBenhChiDinh != null)
                            {
                                int indexDVKT = 1;
                                foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                                {
                                    //tenNhanVienChiDinh = yckb.NhanVienChiDinh.User.HoTen;
                                    var maHocHamVi = string.Empty;
                                    var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                                    if (maHocHamViId != null)
                                    {
                                        maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                                    }

                                    tenNhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                                    Ngay = yckb.ThoiDiemDangKy.Day.ToString();
                                    Thang = yckb.ThoiDiemDangKy.Month.ToString();
                                    Nam = yckb.ThoiDiemDangKy.Year.ToString();

                                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;'><b>{thanhTienFormat}</b></td>";
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
                                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                                htmlDanhSachDichVu += " </tr>";
                                // end BVHD-3939

                                var data = new
                                {
                                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = yeuCauTiepNhan.MaBN,
                                    HoTen = yeuCauTiepNhan.HoTen ?? "",
                                    GioiTinhString = yeuCauTiepNhan.GioiTinh.GetDescription(),
                                    NamSinh = yeuCauTiepNhan.NamSinh ?? null,
                                    DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                                    Ngay = lstYeuCauKhamBenhChiDinh.Select(s=>s.ThoiDiemDangKy.Day).First(), // bởi vì lstYeuCauKhamBenhChiDinh luôn luôn khác null
                                    Thang = lstYeuCauKhamBenhChiDinh.Select(s => s.ThoiDiemDangKy.Month).First(),//// bởi vì lstYeuCauKhamBenhChiDinh luôn luôn khác null
                                    Nam = lstYeuCauKhamBenhChiDinh.Select(s => s.ThoiDiemDangKy.Year).First(),//// bởi vì lstYeuCauKhamBenhChiDinh luôn luôn khác null
                                    DienThoai = yeuCauTiepNhan.SoDienThoai,
                                    DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                                    SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                                    HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                                    //Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //NowTime = DateTime.Now.ApplyFormatTime(),,
                                    NoiYeuCau =  tenPhong,
                                    ChuanDoanSoBo = chanDoanSoBo, // fist đâu tiền dịch vụ != hủy và gói khám != null
                                    DienGiai = dienGiai,
                                    DanhSachDichVu = htmlDanhSachDichVu,
                                    NguoiChiDinh = tenNhanVienChiDinh,
                                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                                    PhieuThu = "YeuCauKhamBenh",
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH()
                                };


                                if (data.PhieuThu == "YeuCauKhamBenh")
                                {
                                    var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";

                                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                    {
                                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                        content = content.Replace(tmpKB, tampKB);
                                    }
                                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                    content = content.Replace(tmp, tamp);
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
                            // BVHD-3939 // == 1 
                            var listDichVuIds = lstDVKB.Select(d => d.dichVuChiDinhId).ToList();
                            var thanhTienDv = listDVK.Where(d => listDichVuIds.Contains(d.Id))
                                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                .FirstOrDefault();
                            CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                            var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                            tongCong += thanhTienDv.GetValueOrDefault();

                            foreach (var itx in lstDVKB)
                            {
                                var lstYeuCauKhamBenhChiDinh = listDVK
                                    .Where(x =>
                                    x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && lstDVKB.Any(y => y.dichVuChiDinhId == x.Id)).OrderBy(x => x.CreatedOn); // to do nam ho;
                                if (lstYeuCauKhamBenhChiDinh != null)
                                {
                                    int indexDVKT = 1;
                                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                                    {
                                        if (itx.dichVuChiDinhId == yckb.Id)
                                        {
                                            var maHocHamVi = string.Empty;
                                            var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
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
                                        }
                                    }
                                }

                                // BVHD-3939- page -total
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                                // BVHD-3939 - số lượng
                                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                                htmlDanhSachDichVu += " </tr>";
                                // end BVHD-3939

                                var data = new
                                {
                                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = yeuCauTiepNhan.MaBN,
                                    HoTen = yeuCauTiepNhan.HoTen ?? "",
                                    GioiTinhString = yeuCauTiepNhan.GioiTinh.GetDescription(),
                                    NamSinh = yeuCauTiepNhan.NamSinh ?? null,
                                    DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                                    Ngay = lstYeuCauKhamBenhChiDinh != null ? lstYeuCauKhamBenhChiDinh.Select(s => s.ThoiDiemDangKy.Day).First() : 0 , // bởi vì lstYeuCauKhamBenhChiDinh luôn luôn khác null
                                    Thang = lstYeuCauKhamBenhChiDinh != null ? lstYeuCauKhamBenhChiDinh.Select(s => s.ThoiDiemDangKy.Month).First():0 ,//// bởi vì lstYeuCauKhamBenhChiDinh luôn luôn khác null
                                    Nam = lstYeuCauKhamBenhChiDinh != null ? lstYeuCauKhamBenhChiDinh.Select(s => s.ThoiDiemDangKy.Year).First():0 ,//// bởi vì lstYeuCauKhamBenhChiDinh luôn luôn khác null
                                    DienThoai = yeuCauTiepNhan.SoDienThoai,
                                    DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                                    SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                                    HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                                    //Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //NowTime = DateTime.Now.ApplyFormatTime(),,
                                    NoiYeuCau = tenPhong,
                                    ChuanDoanSoBo = chanDoanSoBo, // fist đâu tiền dịch vụ != hủy và gói khám != null
                                    DienGiai = dienGiai,
                                    DanhSachDichVu = htmlDanhSachDichVu,
                                    NguoiChiDinh = tenNhanVienChiDinh,
                                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                                    PhieuThu = "YeuCauKhamBenh",
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH()
                                };


                                if (data.PhieuThu == "YeuCauKhamBenh")
                                {
                                    var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
                                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                    {
                                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                        content = content.Replace(tmpKB, tampKB);
                                    }
                                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                    content = content.Replace(tmp, tamp);
                                }
                                htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                                htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
                                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
                                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
                                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
                                htmlDanhSachDichVu += "</tr>";
                                i = 1;
                            }

                        }
                    }
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
                                content = AddPhieuInDichVuChiDinhTheoNguoiChiDinh(listDVKT, content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhanId, hostingName, lstDichVuCungChidinhXN, ghiChuCLS);
                            }
                            else if (itemIn.Count() > 1)
                            {
                                List<ListDichVuChiDinhTheoNguoiChiDinh> lstDichVuCungChidinhXN = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                                lstDichVuCungChidinhXN.AddRange(itemIn);
                                content = AddPhieuInDichVuChiDinhTheoNguoiChiDinh(listDVKT, content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhanId, hostingName, lstDichVuCungChidinhXN, ghiChuCLS);
                            }
                        }
                    }

                }
            }
            return content;
        }

        // in chung Chỉ định Khám đoàn **
        private string InChiDinhInChungKhamDoanTatCa(long yeuCauTiepNhanId, string ghiChuCLS, bool KieuInChung, List<ListDichVuChiDinh> lst, string hostingName,string content)
        {
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
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)

                         .Include(p => p.NguoiLienHeQuanHeNhanThan)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


                         .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                         .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                         .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)

                         
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
                         .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(s=>s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && !listSarsCov2CauHinh.Contains(s.DichVuKyThuatBenhVienId)).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1

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
                        objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0);
                        listTheoNguoiChiDinh.Add(objNguoiChidinh);
                    }

                }
            }

            var lstDVKB = lst.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
            foreach (var itx in lstDVKB)
            {
                var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                   && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    ).OrderBy(x => x.CreatedOn); // to do nam ho;

                if (lstYeuCauKhamBenhChiDinh != null)
                {
                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                    {
                        if (itx.dichVuChiDinhId == yckb.Id)
                        {
                            var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                            objNguoiChidinh.dichVuChiDinhId = itx.dichVuChiDinhId;
                            objNguoiChidinh.nhomChiDinhId = itx.nhomChiDinhId;
                            objNguoiChidinh.TenNhom = itx.TenNhom;
                            objNguoiChidinh.ThuTuIn = itx.ThuTuIn;
                            objNguoiChidinh.NhanVienChiDinhId = yckb.NhanVienChiDinhId;
                            objNguoiChidinh.ThoiDiemChiDinh = new DateTime(yckb.ThoiDiemDangKy.Year, yckb.ThoiDiemDangKy.Month, yckb.ThoiDiemDangKy.Day, 0, 0, 0);
                            listTheoNguoiChiDinh.Add(objNguoiChidinh);
                        }
                    }
                }
            }

            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s =>new { s.NhanVienChiDinhId ,s.ThoiDiemChiDinh}).OrderBy(d=>d.Key.ThoiDiemChiDinh).ToList();
            if (KieuInChung == true)
            {
                // lấy từng nhóm listInChiDinhTheoNguoiChiDinh vào 1 mảng list cần in 
                foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                {
                    var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                    listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                    content = AddChiDinhTheoNguoiChiDinhVaNhom(yeuCauTiepNhanId,listCanIn,listDVK,listDVKT,content, ghiChuCLS, hostingName);
                }
            }
            else
            {   /// in theo STT và Người chỉ định

                foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                {
                    var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                    listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                    content = AddTungPhieuTheoNguoiChiDinhVaTheoSTT(yeuCauTiepNhanId, listCanIn, listDVK, listDVKT, content, ghiChuCLS, hostingName);
                }
            }
            return content;
        }
        // in từng phiếu **
        private string InChiDinhInTungPhieuKhamDoanTatCa(long yeuCauTiepNhanId, string ghiChuCLS, List<ListDichVuChiDinh> lst, string hostingName)
        {
            string content = "";
            var listSarsCov2CauHinh = GetListSarsCauHinh();

            //KieuInChung => in 1. In Theo dịch vụ chỉ định (cùng người chỉ định dịch vụ) 2. In theo số thứ tự (cùng người chỉ định dịch vụ)
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                         //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                         //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                         //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
 

                         .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)

                         .Include(p => p.NguoiLienHeQuanHeNhanThan)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
                         //.Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


                         .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                         .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                         .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


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
                         .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(s=>s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && !listSarsCov2CauHinh.Contains(s.DichVuKyThuatBenhVienId)).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1

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
                        objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0);

                        listTheoNguoiChiDinh.Add(objNguoiChidinh);
                    }

                }
            }

            var lstDVKB = lst.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
            foreach (var itx in lstDVKB)
            {
                var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                   && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    ).OrderBy(x => x.CreatedOn); // to do nam ho;

                if (lstYeuCauKhamBenhChiDinh != null)
                {
                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                    {
                        if (itx.dichVuChiDinhId == yckb.Id)
                        {
                            var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                            objNguoiChidinh.dichVuChiDinhId = itx.dichVuChiDinhId;
                            objNguoiChidinh.nhomChiDinhId = itx.nhomChiDinhId;
                            objNguoiChidinh.TenNhom = itx.TenNhom;
                            objNguoiChidinh.ThuTuIn = itx.ThuTuIn;
                            objNguoiChidinh.NhanVienChiDinhId = yckb.NhanVienChiDinhId;
                            objNguoiChidinh.ThoiDiemChiDinh = new DateTime(yckb.ThoiDiemDangKy.Year, yckb.ThoiDiemDangKy.Month, yckb.ThoiDiemDangKy.Day, 0, 0, 0);

                            listTheoNguoiChiDinh.Add(objNguoiChidinh);
                        }
                    }
                }
            }

            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();


            foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
            {
                var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                content = AddTungPhieuTheoNguoiChiDinh(yeuCauTiepNhanId, listCanIn, listDVK, listDVKT, content, ghiChuCLS, hostingName);
            }
            return content;
        }
        #endregion

        #region  chỉ định  khám bệnh
       //**
        private string AddChiDinhKhamBenhTheoNguoiChiDinhVaNhom(long yeuCauTiepNhanId,long yeuCauKhamBenhId, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string ghiChuCLS, string hostingName,bool? inDichVuBacSiChiDinh)
         {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)


            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)

            //            .Include(p => p.NguoiLienHeQuanHeNhanThan)
            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //            .Include(p => p.BenhNhan)
            //            .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //            .Include(cc => cc.PhuongXa)
            //            .Include(cc => cc.QuanHuyen)
            //            .Include(cc => cc.TinhThanh)
            //            //BVHD-3800
            //            .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            //            .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o=>new
                {
                    LaCapCuu = o.LaCapCuu,
                    LaCapCuuNgoaiTru = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null ? o.YeuCauTiepNhanNgoaiTruCanQuyetToan.LaCapCuu : null,
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();

            var laCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.LaCapCuuNgoaiTru;
          

            var dienGiaiChanDoanSoBo = new List<string>();
            var chanDoanSoBos = new List<string>();

            var lstInThuTuTheoNhomDichVu = listDichVuTheoNguoiChiDinh.First().nhomChiDinhId; // kiểm tra list đầu tiền là dịch vụ gì in nhóm dịch vụ đó trước

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            // từng item phiếu in theo người  chỉ định => tất cả dịch vụ khám bệnh và dịch vụ kỹ thuật đều cùng 1  người chỉ định
            var nhanVienChiDinh = "";
            // tên người chỉ định theo phiếu in 
            string ngay = "";
            string thang = "";
            string nam = "";
            if (lstInThuTuTheoNhomDichVu == (long)Enums.EnumNhomGoiDichVu.DichVuKyThuat)
            {
                // BVHD-3939
                decimal tongCong = 0;
                int soLuong = 0;

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
                var lstDVKT = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat);
                foreach (var itx in lstDVKT)
                {
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))   // lấy data Dịch vụ kỹ thuật dựa vào nhóm id và dichVuid trong list Ui trả lên
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {

                            listInDichVuKyThuat.Add(ycdvkt);
                        }

                    }
                }
                List<ListDichVuChiDinh> lstDichVuCungChidinh = new List<ListDichVuChiDinh>();
                List<ListDichVuChiDinh> lstDichVuChidinhTungPhieu = new List<ListDichVuChiDinh>();

                List<ListDichVuChiDinh> lstDichVuChidinh = new List<ListDichVuChiDinh>();
                foreach (var ycdvkt in listInDichVuKyThuat)
                {
                    var lstDichVuKT = new ListDichVuChiDinh();
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    lstDichVuKT.TenNhom = nhomDichVu;
                    lstDichVuKT.nhomChiDinhId = ycdvkt.NhomDichVuBenhVien.Id;
                    lstDichVuKT.dichVuChiDinhId = ycdvkt.Id;
                    lstDichVuChidinh.Add(lstDichVuKT);
                }
                var lstdvkt = listDVKT.Where(o => o.DichVuKyThuatBenhVien != null);
                foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
                {

                    if (dv.Count() > 1)
                    {
                        // BVHD-3939
                        var listDichVuIds = dv.Select(d => d.dichVuChiDinhId).ToList();
                        
                        var thanhTienDv = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? d.DonGiaSauChietKhau * d.SoLan : d.Gia * d.SoLan)).Sum(d=> d);
                        CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormat = string.Format(culDVKT, "{0:n0}", thanhTienDv);
                        tongCong += thanhTienDv.GetValueOrDefault();
                        // end BVHD-3939

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Any() )
                            {
                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d=>d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId);
                                if (maHocHamViId.Any(d=>d != null))
                                {
                                    maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId.First()).Select(d => d.Ma).FirstOrDefault();
                                }

                                nhanVienChiDinh = returnStringTen(maHocHamVi, "",
                                                                 lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NhanVienChiDinh != null ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NhanVienChiDinh?.User?.HoTen : "");

                                if (indexDVKT == 1)
                                {
                                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() +"</b></td>";
                                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                                    
                                    htmlDanhSachDichVu += " </tr>";
                                }

                                if(lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD).Any())
                                {
                                    dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).FirstOrDefault());

                                    chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD).Any()  ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma).FirstOrDefault() + "-" +
                                                                                                                                                                   lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet).FirstOrDefault() : "");
                                }

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                    if (dv.Count() == 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.dichVuChiDinhId).ToList();
                        var thanhTienDv = lstdvkt.Where(d => d.Id == dv.First().dichVuChiDinhId)
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();
                        CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                        tongCong += thanhTienDv.GetValueOrDefault();
                        // end BVHD-3939
                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Any())
                            {
                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                if (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD).Any())
                                {
                                    dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).FirstOrDefault());

                                    chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD).Any() ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma).FirstOrDefault() + "-" +
                                                                                                                                                                   lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet).FirstOrDefault() : "");
                                }

                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId);
                                if (maHocHamViId.Any(d=>d != null))
                                {
                                    maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId.First()).Select(d => d.Ma).FirstOrDefault();
                                }
                                
                               
                                nhanVienChiDinh = returnStringTen(maHocHamVi, "",
                                                                 
                                                                 lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NhanVienChiDinh != null ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NhanVienChiDinh?.User?.HoTen : "");

                                //var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                // BVHD-3939
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                                //end BVHD-3939

                                htmlDanhSachDichVu += " </tr>";
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.dichVuChiDinhId).First().SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                }


                //DỊCH VỤ KHÁM BỆNH
                var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                int indexDVKB = 1;
                var listInDichVuKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
                foreach (var itx in lstDVKB)
                {
                    var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                       && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                        ).OrderBy(x => x.CreatedOn); // to do nam ho;

                    if (lstYeuCauKhamBenhChiDinh != null)
                    {
                        foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                        {
                            if (itx.dichVuChiDinhId == yckb.Id)
                            {
                                listInDichVuKhamBenh.Add(yckb); // lấy data Dịch vụ khám bệnh dựa vào nhóm id và dichVuid trong list Ui trả lên
                            }
                        }
                    }
                }


                // BVHD-3939
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");

                var thanhTienDvKham = listInDichVuKhamBenh
                    .Select(d => (d.YeuCauGoiDichVuId != null ? d.DonGiaSauChietKhau * 1 : d.Gia * 1)).Sum(d => d);

                var thanhTienDVKhamFormat = string.Format(cul, "{0:n0}", thanhTienDvKham);
                tongCong += thanhTienDvKham.GetValueOrDefault();

                foreach (var yckb in listInDichVuKhamBenh)
                {

                    var maHocHamVi = string.Empty;
                    var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                    if (maHocHamViId != null)
                    {
                        maHocHamVi = MaHocHamHocVi((long)maHocHamViId);
                    }

                    nhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);
                    if(yckb.ChanDoanSoBoICD != null)
                    {
                        dienGiaiChanDoanSoBo.Add(yckb.ChanDoanSoBoGhiChu);

                        chanDoanSoBos.Add(yckb.ChanDoanSoBoICD != null ? yckb.ChanDoanSoBoICD?.Ma + "-" + yckb.ChanDoanSoBoICD?.TenTiengViet :"");
                    }
                    ngay = yckb.ThoiDiemDangKy.Day.ToString();
                    thang = yckb.ThoiDiemDangKy.Month.ToString();
                    nam = yckb.ThoiDiemDangKy.Year.ToString();
                    if (indexDVKB == 1)
                    {
                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                        // BVHD-3939
                        htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                        htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienDVKhamFormat}</b></td>";
                        // end BVHD-3939
                        htmlDanhSachDichVu += " </tr>";
                    }

                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                    htmlDanhSachDichVu += " </tr>";
                    i++;
                    indexDVKB++;
                    soLuong++;
                }
                // từng item phiếu in theo người  chỉ định => tất cả dịch vụ khám bệnh và dịch vụ kỹ thuật đều cùng 1  người chỉ định

                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan.MaBN,
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

                    ChuanDoanSoBo = chanDoanSoBos.Where(s=>s != null && s != "" && s !="-").Distinct().ToList().Join(";"), // khám bệnh 
                    DienGiai= dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"), // khám bệnh

                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = nhanVienChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                    GhiChuCanLamSang = ghiChuCLS,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //BVHD-3800
                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
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
            if (lstInThuTuTheoNhomDichVu == (long)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                // BVHD-3939
                decimal tongCong = 0;
                int soLuong = 0;

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
                var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                int indexDVKB = 1;
                var listInDichVuKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

             

                foreach (var itx in lstDVKB)
                {
                    var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                     && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                      ).OrderBy(x => x.CreatedOn); // to do nam ho;

                    if (lstYeuCauKhamBenhChiDinh != null)
                    {
                        foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                        {
                            if (itx.dichVuChiDinhId == yckb.Id)
                            {
                                listInDichVuKhamBenh.Add(yckb);
                            }
                        }
                    }
                }

                // BVHD-3939
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienDv = listInDichVuKhamBenh
                    .Select(d => (d.YeuCauGoiDichVuId != null ? d.DonGiaSauChietKhau * 1 : d.Gia * 1)).Sum(d => d);
                var thanhTienFormat = string.Format(cul, "{0:n0}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();

                foreach (var yckb in listInDichVuKhamBenh)
                {
                    if (yckb.ChanDoanSoBoICD != null)
                    {
                        dienGiaiChanDoanSoBo.Add(yckb.ChanDoanSoBoGhiChu);

                        chanDoanSoBos.Add(yckb.ChanDoanSoBoICD != null ? yckb.ChanDoanSoBoICD?.Ma + "-" + yckb.ChanDoanSoBoICD?.TenTiengViet : "");
                    }
                     
                    
                    ngay = yckb.ThoiDiemDangKy.Day.ToString();
                    thang = yckb.ThoiDiemDangKy.Month.ToString();
                    nam = yckb.ThoiDiemDangKy.Year.ToString();
                    if (indexDVKB == 1)
                    {
                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                        // BVHD-3939
                        htmlDanhSachDichVu += "<td style='border: 1px solid #020000;position:relative;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                        htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;position:relative;text-align: right;'><b>{thanhTienFormat}</b></td>";
                        // end BVHD-3939
                        htmlDanhSachDichVu += " </tr>";
                    }

                    var maHocHamVi = string.Empty;
                    var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                    if (maHocHamViId != null)
                    {
                        maHocHamVi = MaHocHamHocVi((long)maHocHamViId);
                    }

                    nhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                    if (yckb.ChanDoanSoBoICD != null)
                    {
                        dienGiaiChanDoanSoBo.Add(yckb.ChanDoanSoBoGhiChu);

                        chanDoanSoBos.Add(yckb.ChanDoanSoBoICD != null ? yckb.ChanDoanSoBoICD?.Ma + "-" + yckb.ChanDoanSoBoICD?.TenTiengViet : "");
                    }

                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                    htmlDanhSachDichVu += " </tr>";
                    i++;
                    indexDVKB++;
                    soLuong++;
                }

                // DỊCH VỤ KỸ THUẬT

                int indexDVKT = 1;
                var listInDichVuKyThuat = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat>();
                var lstDVKT = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat);
                foreach (var itx in lstDVKT)
                {
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {

                            listInDichVuKyThuat.Add(ycdvkt);
                        }

                    }
                }
                List<ListDichVuChiDinh> lstDichVuCungChidinh = new List<ListDichVuChiDinh>();
                List<ListDichVuChiDinh> lstDichVuChidinhTungPhieu = new List<ListDichVuChiDinh>();

                List<ListDichVuChiDinh> lstDichVuChidinh = new List<ListDichVuChiDinh>();
                foreach (var ycdvkt in listInDichVuKyThuat)
                {
                    var lstDichVuKT = new ListDichVuChiDinh();
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    lstDichVuKT.TenNhom = nhomDichVu;
                    lstDichVuKT.nhomChiDinhId = ycdvkt.NhomDichVuBenhVien.Id;
                    lstDichVuKT.dichVuChiDinhId = ycdvkt.Id;
                    lstDichVuChidinh.Add(lstDichVuKT);
                }
                foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
                {

                    if (dv.Count() > 1)
                    {
                        // BVHD-3939
                        var listDichVuIds = dv.Select(d => d.dichVuChiDinhId).ToList();

                        var thanhTienDvKT = listDVKT.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? d.DonGiaSauChietKhau * d.SoLan : d.Gia * d.SoLan)).Sum(d => d);
                        CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienDVKTFormat = string.Format(culDVKT, "{0:n0}", thanhTienDvKT);
                        tongCong += thanhTienDvKT.GetValueOrDefault();
                        // end BVHD-3939

                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                        {
                            if (dv.Where(p => p.dichVuChiDinhId == ycdvkt.Id).Any())
                            {
                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();

                                var maHocHamVi = string.Empty;
                                var maHocHamViId = ycdvkt.NhanVienChiDinh?.HocHamHocViId;
                                if (maHocHamViId != null)
                                {
                                    maHocHamVi = MaHocHamHocVi((long)maHocHamViId);
                                }
                                nhanVienChiDinh = returnStringTen(maHocHamVi, "", ycdvkt?.NhanVienChiDinh?.User?.HoTen);

                                if (ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                                {
                                    dienGiaiChanDoanSoBo.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);

                                    chanDoanSoBos.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null ? ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet :"");
                                }

                                ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                                thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                                nam = ycdvkt.ThoiDiemDangKy.Year.ToString();
                                if (indexDVKT == 1)
                                {
                                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() +"</b></td>";
                                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienDVKTFormat}</b></td>";
                                    htmlDanhSachDichVu += " </tr>";
                                }
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
                                soLuong+= ycdvkt.SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                    if (dv.Count() == 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.dichVuChiDinhId).ToList();
                        var thanhTienDVKT = listDVKT.Where(d => d.Id == dv.First().dichVuChiDinhId)
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();
                        CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienDVKTFormat = string.Format(culDVKT, "{0:n0}", thanhTienDVKT);
                        tongCong += thanhTienDVKT.GetValueOrDefault(); ;
                        // end BVHD-3939
                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                        {
                            if (dv.Where(p => p.dichVuChiDinhId == ycdvkt.Id).Any())
                            {
                                if (ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                                {
                                    dienGiaiChanDoanSoBo.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);

                                    chanDoanSoBos.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null ? ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet :"");
                                }
                                var maHocHamVi = string.Empty;
                                var maHocHamViId = ycdvkt.NhanVienChiDinh?.HocHamHocViId;
                                if (maHocHamViId != null)
                                {
                                    maHocHamVi = MaHocHamHocVi((long)maHocHamViId);
                                }
                                nhanVienChiDinh = returnStringTen(maHocHamVi, "", ycdvkt?.NhanVienChiDinh?.User?.HoTen);

                                ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                                thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                                nam = ycdvkt.ThoiDiemDangKy.Year.ToString();

                                var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() +  "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienDVKTFormat}</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'</td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                nhomDichVu = "";
                                soLuong += ycdvkt.SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                }
                // từng item phiếu in theo người  chỉ định => tất cả dịch vụ khám bệnh và dịch vụ kỹ thuật đều cùng 1  người chỉ định

                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan.MaBN,
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

                    ChuanDoanSoBo = chanDoanSoBos.Where(s=>s != null && s != "" && s !="-").ToList().Distinct().Join(";"), // khám bệnh 
                    DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").ToList().Distinct().Join(";"), // khám bệnh

                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = nhanVienChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                    GhiChuCanLamSang = ghiChuCLS,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //BVHD-3800
                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""

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
        //*
        private string AddTungPhieuKhamBenhTheoNguoiChiDinhVaTheoSTT(long yeuCauTiepNhanId, long yeuCauKhamBenhId, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string ghiChuCLS, string hostingName, bool? inDichVuBacSiChiDinh)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

 

            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)

            //            .Include(p => p.NguoiLienHeQuanHeNhanThan)


            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)


            //            .Include(p => p.BenhNhan)
            //            .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //            .Include(cc => cc.PhuongXa)
            //            .Include(cc => cc.QuanHuyen)
            //            .Include(cc => cc.TinhThanh)

            //            //BVHD-3800
            //            .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            //            .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();


            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    LaCapCuu = o.LaCapCuu,
                    LaCapCuuNgoaiTru = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null ? o.YeuCauTiepNhanNgoaiTruCanQuyetToan.LaCapCuu : null,
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();


            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.LaCapCuuNgoaiTru;


            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            var dienGiaiChanDoanSoBo = new List<string>();
            var chanDoanSoBos = new List<string>();

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

            var lstDVKB = listDVK.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).OrderBy(x => x.CreatedOn);
            var lstdvkt = listDVKT.Where(o => o.DichVuKyThuatBenhVien != null);
           
            string ngay = "";
            string thang = "";
            string nam = "";

            var tenNhanVienChiDinh = "";
            decimal tongCong = 0;
            int soLuong = 0;
            if (listDichVuTheoNguoiChiDinh.Count() > 0)
            {
                var dvKTIds = listDichVuTheoNguoiChiDinh.Where(d => d.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).Select(df => df.dichVuChiDinhId).ToList();
                // BVHD-3939 // == 2 
                var thanhTienDVKT = lstdvkt.Where(d => dvKTIds.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                    .Sum();
                tongCong += thanhTienDVKT.GetValueOrDefault(); 


                var dvKTIdKhams = listDichVuTheoNguoiChiDinh.Where(d => d.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh).Select(df => df.dichVuChiDinhId).ToList();
                // BVHD-3939 // == 1
                var thanhTienDVK = lstDVKB.Where(d => dvKTIdKhams.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                    .Sum();
                tongCong += thanhTienDVK.GetValueOrDefault();

                foreach (var Itemx in listDichVuTheoNguoiChiDinh)
                {
                    

                    if (Itemx.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                    {
                        if (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Any())
                        {
                            if (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).Any())
                            {
                                dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).FirstOrDefault());
                                //chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma ).FirstOrDefault());
                            }
                            if (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD).Any())
                            {
                                dienGiaiChanDoanSoBo.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).FirstOrDefault());
                                chanDoanSoBos.Add(lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + s.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet).FirstOrDefault());
                            }

                            var maHocHamVi = string.Empty;
                            var maHocHamViId = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId).Where(d=>d != null);
                            if (maHocHamViId.Any(d=>d != null))
                            {
                                maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                            }
                            tenNhanVienChiDinh = returnStringTen(maHocHamVi,
                                                                 "",
                                                                 lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).FirstOrDefault());

                            ngay = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d=>d.ThoiDiemDangKy.Day.ToString()).First();
                            thang = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                            nam = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiThucHien != null  ? lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().SoLan + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            soLuong += lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).First().SoLan;
                        }
                    }
                    if (Itemx.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                    {
                        if (lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).First() != null)
                        {
                            if (lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s=>s.ChanDoanSoBoICD).Any())
                            {
                                chanDoanSoBos.Add(lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.ChanDoanSoBoICD?.Ma +"-"+s.ChanDoanSoBoICD?.TenTiengViet).First());
                            }

                            if (lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.ChanDoanSoBoGhiChu).Any())
                            {
                                dienGiaiChanDoanSoBo.Add(lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.ChanDoanSoBoGhiChu).First());
                            }

                            var maHocHamVi = string.Empty;
                            var maHocHamViId = lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.NhanVienChiDinh?.HocHamHocViId);
                            if (maHocHamViId.Any(d=>d != null))
                            {
                                maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                            }
                            tenNhanVienChiDinh = returnStringTen(maHocHamVi,
                                                                 "",
                                                                 lstdvkt.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User?.HoTen).FirstOrDefault());

                            ngay = lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                            thang = lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                            nam = lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).First().TenDichVu + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiDangKy != null ? lstDVKB.Where(p => p.Id == Itemx.dichVuChiDinhId).First().NoiDangKy?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>"; 
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            soLuong ++;
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

            }
            var data = new
            {
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan.MaBN,
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
                NoiYeuCau =  tenPhong,
                ChuanDoanSoBo = chanDoanSoBos.Where(s => s != null && s != "" && s != "-").Distinct().ToList().Join(";"), // khám bệnh 
                DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"), // khám bệnh

                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = tenNhanVienChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                GhiChuCanLamSang = ghiChuCLS,
                NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                //BVHD-3800
                CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
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
        //*
        private string AddPhieuInKhamBenhDichVuChiDinhTheoNguoiChiDinh(List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> allListDVKT, string content, Enums.LoaiDichVuKyThuat loaiDichVuKyThuat, long yeuCauTiepNhanId, string hostingName, List<ListDichVuChiDinhTheoNguoiChiDinh> lst, string ghiChuCLS, long yeuCauKhamBenhId,bool? inDichVuBacSiChiDinh)
        {
            var listSarsCov2CauHinh = GetListSarsCauHinh();

            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    LaCapCuu = o.LaCapCuu,
                    LaCapCuuNgoaiTru = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null ? o.YeuCauTiepNhanNgoaiTruCanQuyetToan.LaCapCuu : null,
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();

            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)
 
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)

            //            .Include(p => p.NguoiLienHeQuanHeNhanThan)


            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //            .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //            .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

            //            .Include(p => p.BenhNhan)
            //            .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //            .Include(cc => cc.PhuongXa)
            //            .Include(cc => cc.QuanHuyen)
            //            .Include(cc => cc.TinhThanh)

            //            //BVHD-3800
            //            .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            //            .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.LaCapCuuNgoaiTru;
           


            List<YeuCauDichVuKyThuat> listDVKT = allListDVKT.Where(d => !listSarsCov2CauHinh.Contains(d.DichVuKyThuatBenhVienId)).ToList();

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
                var dienGiaiChanDoanSoBo = new List<string>();
                var chanDoanSoBos = new List<string>();
                // BVHD-3939 // == 1 
                var listDichVuIds = lst.Select(d => d.dichVuChiDinhId).ToList();
                var thanhTienDv = listDVKT.Where(d => listDichVuIds.Contains(d.Id))
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                    .Sum();
                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault(); 

                foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                               && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.Id == lst.First().dichVuChiDinhId))
                {
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    isHave = true;

                    var maHocHamVi = string.Empty;
                    var maHocHamViId = ycdvkt.NhanVienChiDinh?.HocHamHocViId;
                    if (maHocHamViId != null)
                    {
                        maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                    }

                    tenNguoiChiDinh = returnStringTen(maHocHamVi, "", ycdvkt.NhanVienChiDinh?.User?.HoTen);

                    if(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD  != null)
                    {
                        chanDoanSoBos.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet);
                    }
                   
                    dienGiaiChanDoanSoBo.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);

                    ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                    thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                    nam = ycdvkt.ThoiDiemDangKy.Year.ToString();

                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
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
                }
                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939
                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan?.MaBN,
                    HoTen = yeuCauTiepNhan.HoTen ?? "",
                    GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                    Ngay = ngay,
                    Thang = thang,
                    Nam = nam,
                    DienThoai = yeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = yeuCauTiepNhan?.BHYTMaSoThe,
                    HanThe = (yeuCauTiepNhan?.BHYTNgayHieuLuc != null || yeuCauTiepNhan?.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan?.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan?.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                    
                    NoiYeuCau =  tenPhong,

                    ChuanDoanSoBo = chanDoanSoBos.Where(s=>s != null && s !="").Distinct().ToList().Join(";"), // khám bệnh 
                    DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"),

                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan?.TenQuanHeThanNhan,
                    PhieuThu = "DichVuKyThuat",
                    GhiChuCanLamSang = ghiChuCLS,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //BVHD-3800
                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
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
                    }
                    if (!string.IsNullOrEmpty(content))
                    {
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

                CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVKT, "{0:n0}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault(); ;
           
                var dienGiaiChanDoanSoBo = new List<string>();
                var chanDoanSoBos = new List<string>();

                foreach (var itx in lst)
                {
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null
                                                                                            && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {
                            var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();
                            isHave = true;

                            var maHocHamVi = string.Empty;
                            var maHocHamViId = ycdvkt.NhanVienChiDinh?.HocHamHocViId;
                            if (maHocHamViId != null)
                            {
                                maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                            }

                            tenNguoiChiDinh = returnStringTen(maHocHamVi, "", ycdvkt.NhanVienChiDinh?.User?.HoTen);

                            if (ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                            {
                                chanDoanSoBos.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet);
                            }

                            dienGiaiChanDoanSoBo.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);

                            ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                            thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                            nam = ycdvkt.ThoiDiemDangKy.Year.ToString();

                            if (indexDVKT == 1)
                            {
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                // BVHD-3939
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                                // end BVHD-3939
                                htmlDanhSachDichVu += " </tr>";
                            }

                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'</td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            indexDVKT++;
                            nhomDichVu = "";
                            soLuong += ycdvkt.SoLan;
                        }


                    }

                }

                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";
                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = yeuCauTiepNhan?.MaBN,
                    HoTen = yeuCauTiepNhan.HoTen ?? "",
                    GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                    NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                    DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                    Ngay = ngay,
                    Thang = thang,
                    Nam = nam,
                    DienThoai = yeuCauTiepNhan?.SoDienThoai,
                    DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = yeuCauTiepNhan?.BHYTMaSoThe,
                    HanThe = (yeuCauTiepNhan?.BHYTNgayHieuLuc != null || yeuCauTiepNhan?.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan?.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan?.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                   
                    NoiYeuCau = tenPhong,

                    ChuanDoanSoBo = chanDoanSoBos.Where(s=>s != null && s != "" && s !="-").Distinct().ToList().Join(";"), // khám bệnh 
                    DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"),

                    DanhSachDichVu = htmlDanhSachDichVu,
                    NguoiChiDinh = tenNguoiChiDinh,
                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = yeuCauTiepNhan?.TenQuanHeThanNhan,
                    PhieuThu = "DichVuKyThuat",
                    GhiChuCanLamSang = ghiChuCLS,
                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                    //BVHD-3800
                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
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
                       
                    }
                    if (!string.IsNullOrEmpty(content)) {

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

        //*
        private string AddTungPhieuKhamBenhTheoNguoiChiDinh(long yeuCauTiepNhanId,long yeuCauKhamBenhId, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
          List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
          List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string ghiChuCLS, string hostingName, bool? inDichVuBacSiChiDinh)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)

 


            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
            //          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)

            //          .Include(p => p.NguoiLienHeQuanHeNhanThan)


            //          .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //          .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
            //          .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
            //          .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.HocHamHocVi)
            //          .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.ChucDanh)?.ThenInclude(p => p.NhomChucDanh)


            //          .Include(p => p.BenhNhan)
            //          .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
            //          .Include(cc => cc.PhuongXa)
            //          .Include(cc => cc.QuanHuyen)
            //          .Include(cc => cc.TinhThanh)
            //          // BVHD - 3800
            //          .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            //          .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    LaCapCuu = o.LaCapCuu,
                    LaCapCuuNgoaiTru = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null ? o.YeuCauTiepNhanNgoaiTruCanQuyetToan.LaCapCuu : null,
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();

            //BVHD-3800
            var laCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.LaCapCuuNgoaiTru;
           

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
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
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

                string ngay = "";
                string thang = "";
                string nam = "";

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
                                content = AddPhieuInKhamBenhDichVuChiDinhTheoNguoiChiDinh(listDVKT, content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhanId, hostingName, lstDichVuCungChidinhXN, ghiChuCLS, yeuCauKhamBenhId, inDichVuBacSiChiDinh);
                            }
                            else if (itemIn.Count() > 1)
                            {
                                List<ListDichVuChiDinhTheoNguoiChiDinh> lstDichVuCungChidinhXN = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                                lstDichVuCungChidinhXN.AddRange(itemIn);
                                content = AddPhieuInKhamBenhDichVuChiDinhTheoNguoiChiDinh(listDVKT, content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhanId, hostingName, lstDichVuCungChidinhXN, ghiChuCLS, yeuCauKhamBenhId, inDichVuBacSiChiDinh);
                            }
                        }
                    }


                    var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);


                    if (lstDVKB.Any())
                    {
                       
                        if (listDichVuTheoNguoiChiDinh.Count() == 1)
                        {
                            decimal tongCong = 0;
                            int soLuong = 0;

                            // BVHD-3939 // == 1 
                            var thanhTienDv = listDVK.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && x.Id == listDichVuTheoNguoiChiDinh.First().dichVuChiDinhId)
                                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                .FirstOrDefault();
                            CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                            var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                            tongCong += thanhTienDv.GetValueOrDefault();


                            var dienGiaiChanDoanSoBo = new List<string>();
                            var chanDoanSoBos = new List<string>();
                            var lstYeuCauKhamBenhChiDinh = listDVK.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && x.Id == listDichVuTheoNguoiChiDinh.First().dichVuChiDinhId).OrderBy(x => x.CreatedOn); // to do nam ho;
                            
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
                                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'>{thanhTienFormat}</td>";
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
                                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                                htmlDanhSachDichVu += " </tr>";
                                // end BVHD-3939

                                var data = new
                                {
                                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = yeuCauTiepNhan.MaBN,
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
                                    NoiYeuCau =  tenPhong,

                                    ChuanDoanSoBo = chanDoanSoBos.Where(s=>s != null && s != "" && s !="-").Distinct().ToList().Join(";"), // khám bệnh 
                                    DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"),

                                    DanhSachDichVu = htmlDanhSachDichVu,
                                    NguoiChiDinh = tenNhanVienChiDinh,
                                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                                    PhieuThu = "YeuCauKhamBenh",
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //BVHD-3800
                                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
                                };


                                if (data.PhieuThu == "YeuCauKhamBenh")
                                {
                                    var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
                                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                    {
                                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                        content = content.Replace(tmpKB, tampKB);
                                    }
                                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                    content = content.Replace(tmp, tamp);
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
                           

                            foreach (var itx in lstDVKB)
                            {
                                decimal tongCong = 0;
                                int soLuong = 0;
                                // BVHD-3939 // == 1 
                                var listDichVuIds = lstDVKB.Select(d => d.dichVuChiDinhId).ToList();
                                var thanhTienDv = listDVK.Where(d => listDichVuIds.Contains(d.Id))
                                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                    .FirstOrDefault();
                                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                                var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                                tongCong += thanhTienDv.GetValueOrDefault();

                                var dienGiaiChanDoanSoBo = new List<string>();
                                var chanDoanSoBos = new List<string>();
                                if (listDVK != null)
                                {
                                    int indexDVK = 1;
                                    foreach (var yckb in listDVK)
                                    {
                                        if (itx.dichVuChiDinhId == yckb.Id)
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
                                            indexDVK++;
                                            soLuong++;
                                        }
                                    }
                                }
                                // BVHD-3939- page -total
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                                // BVHD-3939 - số lượng
                                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                                htmlDanhSachDichVu += " </tr>";
                                // end BVHD-3939
                                var data = new
                                {
                                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = yeuCauTiepNhan.MaBN,
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
                                    NoiYeuCau =  tenPhong,

                                    ChuanDoanSoBo = chanDoanSoBos.Where(s=>s != null && s != "" && s !="-").Distinct().ToList().Join(";"), // khám bệnh 
                                    DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"),

                                    DanhSachDichVu = htmlDanhSachDichVu,
                                    NguoiChiDinh = tenNhanVienChiDinh,
                                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                                    PhieuThu = "YeuCauKhamBenh",
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //BVHD-3800
                                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
                                };


                                if (data.PhieuThu == "YeuCauKhamBenh")
                                {
                                    var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
                                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                    {
                                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                        content = content.Replace(tmpKB, tampKB);
                                    }
                                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                    content = content.Replace(tmp, tamp);
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
                if (listInThuTuGDV == (long)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                  
                    var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                    if (lstDVKB.Any())
                    {
                        
                        if (listDichVuTheoNguoiChiDinh.Count() == 1)
                        {
                            decimal tongCong = 0;
                            int soLuong = 0;

                            // BVHD-3939 // == 1 
                            var thanhTienDv = listDVK.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && x.Id == listDichVuTheoNguoiChiDinh.First().dichVuChiDinhId)
                                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                .FirstOrDefault();
                            CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                            var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                            tongCong += thanhTienDv.GetValueOrDefault();

                            var dienGiaiChanDoanSoBo = new List<string>();
                            var chanDoanSoBos = new List<string>();

                            var lstYeuCauKhamBenhChiDinh = listDVK.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && x.Id == listDichVuTheoNguoiChiDinh.First().dichVuChiDinhId).OrderBy(x => x.CreatedOn); // to do nam ho;
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
                                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                                htmlDanhSachDichVu += " </tr>";
                                // end BVHD-3939
                                var data = new
                                {
                                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = yeuCauTiepNhan.MaBN,
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

                                    ChuanDoanSoBo =chanDoanSoBos.Where(s=>s != null && s != "" && s !="-").Distinct().ToList().Join(";"), // khám bệnh 
                                    DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"),

                                    DanhSachDichVu = htmlDanhSachDichVu,
                                    NguoiChiDinh = tenNhanVienChiDinh,
                                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                                    PhieuThu = "YeuCauKhamBenh",
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //BVHD-3800
                                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
                                };


                                if (data.PhieuThu == "YeuCauKhamBenh")
                                {
                                    var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";

                                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                    {
                                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                        content = content.Replace(tmpKB, tampKB);
                                    }
                                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                    content = content.Replace(tmp, tamp);
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
                            foreach (var itx in lstDVKB)
                            {
                                var lstYeuCauKhamBenhChiDinh = listDVK
                                    .Where(x =>
                                    x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && lstDVKB.Any(y => y.dichVuChiDinhId == x.Id)).OrderBy(x => x.CreatedOn); // to do nam ho;

                                var dienGiaiChanDoanSoBo = new List<string>();
                                var chanDoanSoBos = new List<string>();

                                decimal tongCong = 0;
                                int soLuong = 0;
                                // BVHD-3939 // == 1 
                                var listDichVuIds = lstDVKB.Select(d => d.dichVuChiDinhId).ToList();
                                var thanhTienDv = listDVK.Where(d => listDichVuIds.Contains(d.Id))
                                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                                    .FirstOrDefault();
                                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                                var thanhTienFormat = string.Format(culDVK, "{0:n0}", thanhTienDv);
                                tongCong += thanhTienDv.GetValueOrDefault();

                                if (lstYeuCauKhamBenhChiDinh != null)
                                {
                                    int indexDVKT = 1;
                                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                                    {
                                        if (itx.dichVuChiDinhId == yckb.Id)
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
                                            htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;'><b>{thanhTienFormat}</b></td>";
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
                                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

                                htmlDanhSachDichVu += " </tr>";
                                // end BVHD-3939
                                var data = new
                                {
                                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                                    MaBN = yeuCauTiepNhan.MaBN,
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
                                    NoiYeuCau =  tenPhong,

                                    ChuanDoanSoBo = chanDoanSoBos.Where(s=>s != null && s != "" && s !="-").Distinct().ToList().Join(";"), // khám bệnh 
                                    DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").Distinct().ToList().Join(";"),

                                    DanhSachDichVu = htmlDanhSachDichVu,
                                    NguoiChiDinh = tenNhanVienChiDinh,
                                    NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                                    TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                                    PhieuThu = "YeuCauKhamBenh",
                                    NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                                    //BVHD-3800
                                    CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""
                                };


                                if (data.PhieuThu == "YeuCauKhamBenh")
                                {
                                    var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
                                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
                                    if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
                                    {
                                        var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                                        var tmpKB = "<tr id=\"NguoiGiamHo\">";
                                        content = content.Replace(tmpKB, tampKB);
                                    }
                                    var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                                    content = content.Replace(tmp, tamp);
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
                                content = AddPhieuInKhamBenhDichVuChiDinhTheoNguoiChiDinh(listDVKT, content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhanId, hostingName, lstDichVuCungChidinhXN, ghiChuCLS, yeuCauKhamBenhId, inDichVuBacSiChiDinh);
                            }
                            else if (itemIn.Count() > 1)
                            {
                                List<ListDichVuChiDinhTheoNguoiChiDinh> lstDichVuCungChidinhXN = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                                lstDichVuCungChidinhXN.AddRange(itemIn);
                                content = AddPhieuInKhamBenhDichVuChiDinhTheoNguoiChiDinh(listDVKT, content, Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh, yeuCauTiepNhanId, hostingName, lstDichVuCungChidinhXN, ghiChuCLS, yeuCauKhamBenhId, inDichVuBacSiChiDinh);
                            }
                        }
                    }

                }
            }
            return content;
        }

        //*
        private string InChiDinhInChungTatCa(long yeuCauTiepNhanId, long yeuCauKhamBenhId, string ghiChuCLS, bool KieuInChung, List<ListDichVuChiDinh> lst, string hostingName, string content,bool? inDichVuBacSiChiDinh)
        {
            var listSarsCov2CauHinh = GetListSarsCauHinh();

            //KieuInChung => in 1. In Theo dịch vụ chỉ định (cùng người chỉ định dịch vụ) 2. In theo số thứ tự (cùng người chỉ định dịch vụ)
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)

                      .Include(p => p.NguoiLienHeQuanHeNhanThan)


                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


                      .Include(p => p.BenhNhan)
                      .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                      .Include(cc => cc.PhuongXa)
                      .Include(cc => cc.QuanHuyen)
                      .Include(cc => cc.TinhThanh)
                      .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();


            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && !listSarsCov2CauHinh.Contains(s.DichVuKyThuatBenhVienId)).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;

            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1

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
                        objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0);

                        listTheoNguoiChiDinh.Add(objNguoiChidinh);
                    }

                }
            }

            var lstDVKB = lst.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
            foreach (var itx in lstDVKB)
            {
                var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                   && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    ).OrderBy(x => x.CreatedOn); // to do nam ho;

                if (lstYeuCauKhamBenhChiDinh != null)
                {
                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                    {
                        if (itx.dichVuChiDinhId == yckb.Id)
                        {
                            var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                            objNguoiChidinh.dichVuChiDinhId = itx.dichVuChiDinhId;
                            objNguoiChidinh.nhomChiDinhId = itx.nhomChiDinhId;
                            objNguoiChidinh.TenNhom = itx.TenNhom;
                            objNguoiChidinh.ThuTuIn = itx.ThuTuIn;
                            objNguoiChidinh.NhanVienChiDinhId = yckb.NhanVienChiDinhId;
                            objNguoiChidinh.ThoiDiemChiDinh = new DateTime(yckb.ThoiDiemDangKy.Year, yckb.ThoiDiemDangKy.Month, yckb.ThoiDiemDangKy.Day, 0, 0, 0);
                            listTheoNguoiChiDinh.Add(objNguoiChidinh);
                        }
                    }
                }
            }

            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d=>d.Key.ThoiDiemChiDinh).ToList();
            if (KieuInChung == true)
            {
                // lấy từng nhóm listInChiDinhTheoNguoiChiDinh vào 1 mảng list cần in 
                foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                {
                    var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                    listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                    content = AddChiDinhKhamBenhTheoNguoiChiDinhVaNhom(yeuCauTiepNhanId, yeuCauKhamBenhId, listCanIn, listDVK, listDVKT, content, ghiChuCLS, hostingName, inDichVuBacSiChiDinh);
                }
            }
            else
            {   /// in theo STT và Người chỉ định

                foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                {
                    var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                    listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                    content = AddTungPhieuKhamBenhTheoNguoiChiDinhVaTheoSTT(yeuCauTiepNhanId, yeuCauKhamBenhId, listCanIn, listDVK, listDVKT, content, ghiChuCLS, hostingName, inDichVuBacSiChiDinh);
                }
            }
            return content;
        }
        //*
        private string InChiDinhInTungPhieuTatCa(long yeuCauTiepNhanId, long yeuCauKhamBenhId, string ghiChuCLS, List<ListDichVuChiDinh> lst, string hostingName,bool? inDichVuBacSiChiDinh)
        {
            string content = "";
            //KieuInChung => in 1. In Theo dịch vụ chỉ định (cùng người chỉ định dịch vụ) 2. In theo số thứ tự (cùng người chỉ định dịch vụ)
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                      //.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)

                      .Include(p => p.NguoiLienHeQuanHeNhanThan)


                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


                      .Include(p => p.BenhNhan)
                      .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                      .Include(cc => cc.PhuongXa)
                      .Include(cc => cc.QuanHuyen)
                      .Include(cc => cc.TinhThanh)
                      .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();


            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(s=>s.TrangThai !=  Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(s=>s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";
            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1

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
                        objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0);

                        listTheoNguoiChiDinh.Add(objNguoiChidinh);
                    }

                }
            }

            var lstDVKB = lst.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
            foreach (var itx in lstDVKB)
            {
                var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                   && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    ).OrderBy(x => x.CreatedOn); // to do nam ho;

                if (lstYeuCauKhamBenhChiDinh != null)
                {
                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                    {
                        if (itx.dichVuChiDinhId == yckb.Id)
                        {
                            var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                            objNguoiChidinh.dichVuChiDinhId = itx.dichVuChiDinhId;
                            objNguoiChidinh.nhomChiDinhId = itx.nhomChiDinhId;
                            objNguoiChidinh.TenNhom = itx.TenNhom;
                            objNguoiChidinh.ThuTuIn = itx.ThuTuIn;
                            objNguoiChidinh.NhanVienChiDinhId = yckb.NhanVienChiDinhId;
                            objNguoiChidinh.ThoiDiemChiDinh = new DateTime(yckb.ThoiDiemDangKy.Year, yckb.ThoiDiemDangKy.Month, yckb.ThoiDiemDangKy.Day, 0, 0, 0);

                            listTheoNguoiChiDinh.Add(objNguoiChidinh);
                        }
                    }
                }
            }

            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();


            foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
            {
                var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                content = AddTungPhieuKhamBenhTheoNguoiChiDinh(yeuCauTiepNhanId,yeuCauKhamBenhId, listCanIn, listDVK, listDVKT, content, ghiChuCLS, hostingName,inDichVuBacSiChiDinh);
            }
            return content;
        }
        #endregion


        #region update tính giá bhtt

        //private async Task KiemTraChiTietBaoHiemThanhToan(ChiTietBaoHiemThanhToanVo chiTietThanhToan)
        //{
        //    chiTietThanhToan.MucHuong = 1;
        //    var lstLanKham = await _yeuCauTiepNhanRepository.TableNoTracking.Where(x => x.Id == chiTietThanhToan.YeuCauTiepNhanId)
        //        .SelectMany(x => x.YeuCauKhamBenhs.Where(y => y.DuocHuongBaoHiem)).ToListAsync();
        //    if (lstLanKham.Any()) //&& lstLanKham.Count < 6) // bảo hiểm chỉ thanh toán cho 5 lằn đầu tiên trong ngày
        //    {
        //        chiTietThanhToan.LanKhamCoBHTrongNgay = lstLanKham.FindIndex(x => x.Id == chiTietThanhToan.YeuCauKhamBenhId) + 1;
        //    }

        //    switch (chiTietThanhToan.LanKhamCoBHTrongNgay)
        //    {
        //        case 1: chiTietThanhToan.TiLeBaoHiemThanhToan = 1; break;
        //        case 2:
        //        case 3:
        //        case 4: chiTietThanhToan.TiLeBaoHiemThanhToan = decimal.Parse("0.3"); break;
        //        case 5: chiTietThanhToan.TiLeBaoHiemThanhToan = decimal.Parse("0.1"); break;
        //        default: chiTietThanhToan.TiLeBaoHiemThanhToan = 0; break;
        //    }
        //}

        /*
        public async Task<ChiTietBaoHiemThanhToanVo> XuLyChiTietBaoHiemThanhToanAsync(ChiTietBaoHiemThanhToanVo chiTietThanhToan)
        {
            // kiểm tra lần khám trong ngày
            await KiemTraChiTietBaoHiemThanhToan(chiTietThanhToan);

            if (chiTietThanhToan.LanKhamCoBHTrongNgay > 0 && chiTietThanhToan.LanKhamCoBHTrongNgay < 6)
            {
                var yeuCauKhamBenh = await _yeuCauKhamBenhRepository.Table
                    .Include(p => p.YeuCauTiepNhan)
                    .Include(p => p.DichVuKhamBenhBenhVien).ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)
                    .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                    .Include(p => p.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                    .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.DuocPhamBenhVien).ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
                    .Include(p => p.YeuCauVatTuBenhViens).ThenInclude(p => p.VatTuBenhVien)
                    .Where(p => p.Id == chiTietThanhToan.YeuCauKhamBenhId && p.YeuCauTiepNhanId == chiTietThanhToan.YeuCauTiepNhanId && p.DuocHuongBaoHiem)
                    .FirstOrDefaultAsync();

                if (yeuCauKhamBenh != null && (chiTietThanhToan.IsUpdate || (chiTietThanhToan.IsUpdate != true && chiTietThanhToan.DonGiaThems.Any())))
                {
                    chiTietThanhToan.DuocHuongBaoHiem = true;
                    var tongTienBHTT = (yeuCauKhamBenh.GiaBaoHiemThanhToan ?? 0) +
                                       yeuCauKhamBenh.YeuCauDichVuKyThuats?.Where(x => x.DuocHuongBaoHiem && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(x => (x.GiaBaoHiemThanhToan ?? 0) * x.SoLan) +
                                       yeuCauKhamBenh.YeuCauDichVuGiuongBenhViens?.Where(x => x.DuocHuongBaoHiem && x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy).Sum(x => x.GiaBaoHiemThanhToan) +
                                       yeuCauKhamBenh.YeuCauDuocPhamBenhViens?.Where(x => x.DuocHuongBaoHiem && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Sum(x => (x.GiaBaoHiemThanhToan ?? 0) * (decimal)x.SoLuong) +
                                       yeuCauKhamBenh.YeuCauVatTuBenhViens?.Where(x => x.DuocHuongBaoHiem && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Sum(x => (x.GiaBaoHiemThanhToan ?? 0) * (decimal)x.SoLuong);
                    if (chiTietThanhToan.DonGiaThems.Any())
                    {
                        foreach (var item in chiTietThanhToan.DonGiaThems)
                        {
                            tongTienBHTT += item * chiTietThanhToan.TiLeBaoHiemThanhToan * chiTietThanhToan.MucHuong;
                        }
                    }

                    if (tongTienBHTT > chiTietThanhToan.SoTienBHTTToanBo) // nếu tổng tiền BHTT > SoTienBHTTToanBo: muc hưởng = múc hưởng của tiep nhan
                    {
                        chiTietThanhToan.MucHuong = yeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong == null
                            ? 0
                            : (decimal) yeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong / 100; // xử lý gán lại mức hưởng
                    }

                    if (tongTienBHTT > chiTietThanhToan.SoTienBHTTToanBo || chiTietThanhToan.IsUpdate)
                    {
                        // xử lý tính lại giá BHTT cho tất cả các dịch vụ (hiện tại chỉ mới cập nhật dịch vụ kỹ thuật và giường)
                        await XuLyCapNhatLaiGiaBHTTTheoLanKhamAsync(chiTietThanhToan, yeuCauKhamBenh);
                    }

                }
                else
                {
                    chiTietThanhToan.DuocHuongBaoHiem = false;
                }
            }

            return chiTietThanhToan;
        }

        private async Task XuLyCapNhatLaiGiaBHTTTheoLanKhamAsync(ChiTietBaoHiemThanhToanVo chiTietBHTT, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKhamBenh)
        {
            if (yeuCauKhamBenh != null) //(hiện tại chỉ mới cập nhật dịch vụ kỹ thuật và giường)
            {
                // cập nhật lại BHTT dịch vụ khám bệnh
                var dichVuKhamBenhGiaBaoHiem =
                    yeuCauKhamBenh.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(o =>
                        o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));

                // giá BH thực = Giá * Tỉ lệ thanh toán
                var giaBHTT = dichVuKhamBenhGiaBaoHiem == null ? 0 : (dichVuKhamBenhGiaBaoHiem.Gia * (decimal)dichVuKhamBenhGiaBaoHiem.TiLeBaoHiemThanhToan / 100) * chiTietBHTT.TiLeBaoHiemThanhToan * chiTietBHTT.MucHuong;
                if (yeuCauKhamBenh.GiaBaoHiemThanhToan != giaBHTT)
                {
                    yeuCauKhamBenh.GiaBaoHiemThanhToan = giaBHTT;
                    yeuCauKhamBenh.TrangThaiThanhToan = yeuCauKhamBenh.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan ? TrangThaiThanhToan.CapNhatThanhToan : yeuCauKhamBenh.TrangThaiThanhToan;
                }

                // cập nhật lại BHTT dịch vụ kỹ thuật
                foreach (var item in yeuCauKhamBenh.YeuCauDichVuKyThuats)
                {
                    giaBHTT = 0;
                    if (item.DuocHuongBaoHiem && item.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    {
                        var yeuCauDichVuKyThuatGiaBaoHiem = item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(o =>
                                                                o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value))?.Gia ?? 0;
                        giaBHTT = yeuCauDichVuKyThuatGiaBaoHiem * chiTietBHTT.TiLeBaoHiemThanhToan * chiTietBHTT.MucHuong;
                        if(item.GiaBaoHiemThanhToan != giaBHTT)
                        {
                            item.GiaBaoHiemThanhToan = giaBHTT;
                            item.TrangThaiThanhToan = item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan ? TrangThaiThanhToan.CapNhatThanhToan : item.TrangThaiThanhToan;
                        }
                    }
                }

                // cập nhật lại BHTT dịch vụ giường
                foreach (var item in yeuCauKhamBenh.YeuCauDichVuGiuongBenhViens)
                {
                    giaBHTT = 0;
                    if (item.DuocHuongBaoHiem && item.TrangThai != EnumTrangThaiGiuongBenh.DaHuy)
                    {
                        var yeuCauDichVuGiuongGiaBaoHiem = item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o =>
                                                                o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value))?.Gia ?? 0;
                        giaBHTT = yeuCauDichVuGiuongGiaBaoHiem * chiTietBHTT.TiLeBaoHiemThanhToan * chiTietBHTT.MucHuong;
                        if(item.GiaBaoHiemThanhToan != giaBHTT)
                        {
                            item.GiaBaoHiemThanhToan = giaBHTT;
                            item.TrangThaiThanhToan = item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan ? TrangThaiThanhToan.CapNhatThanhToan : item.TrangThaiThanhToan;
                        }
                    }
                }

                // cập nhật lại BHTT vật tư

                // cập nhật lại BHTT dược phẩm

                await _yeuCauKhamBenhRepository.UpdateAsync(yeuCauKhamBenh);
            }
        }
        */
        public async Task<decimal> GetDonGiaBenhVienDichVuKyThuatAsync(long dichVuKyThuatBenhVienId, long nhomGia)
        {
            var giaBenhVien = await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                .FirstOrDefaultAsync(x => x.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId
                                          && x.NhomGiaDichVuKyThuatBenhVienId == nhomGia
                                          && x.TuNgay <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay >= DateTime.Now.Date));
            return giaBenhVien != null ? giaBenhVien.Gia : 0;
        }

        public async Task<decimal> GetDonGiaBenhVienDichVuKhamBenhAsync(long dichVuKhamBenhBenhVienId, long nhomGia)
        {
            var giaBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .SelectMany(x => x.DichVuKhamBenhBenhVienGiaBenhViens)
                .FirstOrDefaultAsync(x => x.DichVuKhamBenhBenhVienId == dichVuKhamBenhBenhVienId
                                          && x.NhomGiaDichVuKhamBenhBenhVienId == nhomGia
                                          && x.TuNgay <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay >= DateTime.Now.Date));
            return giaBenhVien != null ? giaBenhVien.Gia : 0;
        }

        #endregion

        #region getdata

        public async Task<ActionResult<DichVuKyThuatBenhVienTemplateVo>> GetChiDinhThongTinDichVuKyThuatAsync(long dichVuKyThuatBenhVienId)
        {
            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            var dichVuBenhVien = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Include(p => p.NhomDichVuBenhVien)
                .Include(p => p.DichVuKyThuat).ThenInclude(p => p.DichVuKyThuatThongTinGias)
              .Include(p => p.DichVuKyThuatVuBenhVienGiaBenhViens).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
              .Where(x => x.Id == dichVuKyThuatBenhVienId)
              .Select(item => new DichVuKyThuatBenhVienTemplateVo
              {
                  DisplayName = item.Ma + " - " + item.Ten,
                  KeyId = item.Id,
                  DichVu = item.Ten,
                  Ma = item.Ma,
                  NhomDichVuKyThuatId = item.NhomDichVuBenhVienId,
                  MaGiaDichVu = item.DichVuKyThuat != null ? item.DichVuKyThuat.MaGia : null,
                  Gia = item.DichVuKyThuatVuBenhVienGiaBenhViens.
                      First(X => X.TuNgay <= DateTime.Now.Date && X.DenNgay >= DateTime.Now.Date || (X.TuNgay <= DateTime.Now.Date && X.DenNgay == null)).Gia,

                  NhomGiaDichVuKyThuatBenhVienId = item.DichVuKyThuatVuBenhVienGiaBenhViens
                      .First(p => p.TuNgay <= DateTime.Now.Date && (p.DenNgay == null || p.DenNgay >= DateTime.Now.Date)).NhomGiaDichVuKyThuatBenhVienId,
                  NhomChiPhi = item.DichVuKyThuat != null ? item.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe, //item.DichVuKyThuat.NhomChiPhi,
                  LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                  NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                  TenNhomDichVuBenhVien = item.NhomDichVuBenhVien != null ? item.NhomDichVuBenhVien.Ma + " - " + item.NhomDichVuBenhVien.Ten : null
              })
              .FirstOrDefaultAsync();
            return dichVuBenhVien;
        }

        public async Task<ActionResult<List<SoDoGiuongBenhTheoPhongKhamVo>>> GetSoDoGiuongBenhTheoPhongKhamAsync(long phongBenhVienId = 0, bool giuongTrong = false, bool giuongDangSuDung = false,
            long dichVuGiuongBenhVienId = 0, long noiThucHienId = 0, long giuongBenhId = 0)
        {
            var lstPhongId = new List<long>();
            if (dichVuGiuongBenhVienId != 0)
            {
                var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong)
                    .ThenInclude(z => z.PhongBenhViens)
                    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
                    .Where(x => x.Id == dichVuGiuongBenhVienId)
                    .FirstOrDefaultAsync();
                if (dichVuGiuongBenhVien != null)
                {
                    lstPhongId = dichVuGiuongBenhVien.DichVuGiuongBenhVienNoiThucHiens
                        .Where(x => x.KhoaPhong != null && x.KhoaPhong.IsDisabled != true)
                        .SelectMany(x => x.KhoaPhong.PhongBenhViens.Where(y => y.IsDisabled != true).Select(y => y.Id))
                        .Union(dichVuGiuongBenhVien.DichVuGiuongBenhVienNoiThucHiens
                            .Where(x => x.PhongBenhVien != null && x.PhongBenhVien.IsDisabled != true)
                            .Select(y => y.PhongBenhVien.Id))
                        .Distinct().ToList();
                }
            }

            var lstPhongBenhVien = await _phongBenhVienRepository.TableNoTracking
                .Include(x => x.GiuongBenhs).ThenInclude(y => y.HoatDongGiuongBenhs)
                .Where(x => x.IsDisabled != true && (!lstPhongId.Any() || lstPhongId.Any(y => y == x.Id)) && (phongBenhVienId == 0 || x.Id == phongBenhVienId))
                .OrderByDescending(x => noiThucHienId == 0 || x.Id == noiThucHienId)
                .ThenBy(x => x.Ma + " - " + x.Ten)
                .ToListAsync();

            var lstHoatDongGiuong =
                lstPhongBenhVien.SelectMany(x => x.GiuongBenhs.SelectMany(y => y.HoatDongGiuongBenhs.Where(z => z.ThoiDiemKetThuc == null)));

            var soDoGiuong = lstPhongBenhVien.Select(item => new SoDoGiuongBenhTheoPhongKhamVo()
            {
                PhongBenhVienId = item.Id,
                TenPhongBenhVien = item.Ten,
                MaPhongBenhVien = item.Ma,
                DisplayName = item.Ma + " - " + item.Ten,
                GiuongBenhs = item.GiuongBenhs.Select(items => new GiuongBenhTheoPhongBenhVienVo()
                {
                    GiuongBenhId = items.Id,
                    TenGiuong = items.Ten,
                    DisplayName = items.Ten,
                    SoNguoiHienTai = lstHoatDongGiuong.Count(i => i.GiuongBenhId == items.Id),
                    IsAvailable =
                            !items.HoatDongGiuongBenhs.Any(i =>
                                i.GiuongBenhId == items.Id && i.ThoiDiemKetThuc == null)
                })
                        .Where(y => (giuongTrong && y.IsAvailable == true) || (giuongDangSuDung && y.IsAvailable == false))
                        .ToList()
            })
                .Where(x => x.GiuongBenhs.Any())
                .ToList();

            return soDoGiuong;
        }

        public async Task<List<PhongKhamTemplateVo>> GetAllPhongBenhVienDangHoatDongAsync(DropDownListRequestModel model)
        {
            var lstPhongId = new List<long>();
            var dichVuGiuongBenhVienId = CommonHelper.GetIdFromRequestDropDownList(model); //model.Id;

            var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong)
                .ThenInclude(z => z.PhongBenhViens)
                .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
                .Where(x => x.Id == dichVuGiuongBenhVienId)
                .FirstOrDefaultAsync();
            if (dichVuGiuongBenhVien != null)
            {
                lstPhongId = dichVuGiuongBenhVien.DichVuGiuongBenhVienNoiThucHiens
                    .Where(x => x.KhoaPhong != null && x.KhoaPhong.IsDisabled != true)
                    .SelectMany(x => x.KhoaPhong.PhongBenhViens.Where(y => y.IsDisabled != true).Select(y => y.Id))
                    .Union(dichVuGiuongBenhVien.DichVuGiuongBenhVienNoiThucHiens
                        .Where(x => x.PhongBenhVien != null && x.PhongBenhVien.IsDisabled != true)
                        .Select(y => y.PhongBenhVien.Id))
                    .Distinct().ToList();
            }

            var query = await _phongBenhVienRepository.TableNoTracking
                .ApplyLike(model.Query, x => x.Ma, x => x.Ten)
                .Where(x => x.IsDisabled != true && (!lstPhongId.Any() || lstPhongId.Any(y => y == x.Id)))
                .Take(model.Take).Select(item => new PhongKhamTemplateVo()
                {
                    DisplayName = item.Ma + " - " + item.Ten,
                    KeyId = item.Id,
                    TenPhong = item.Ten,
                    MaPhong = item.Ma
                }).ToListAsync();

            return query;
        }
        #endregion

        public async Task CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId, long phongBenhVienId)
        {
            #region Code xử lý cập nhật hàng đợi cũ
            //var lstHangDoi = await _phongBenhVienHangDoiRepository.Table
            //    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhLichSuTrangThais)
            //    .Where(x => x.PhongBenhVienId == phongBenhVienId
            //                && x.YeuCauKhamBenh != null
            //                && x.YeuCauDichVuKyThuatId == null)
            //    .OrderBy(x => x.SoThuTu).ToListAsync();
            //if (lstHangDoi.Any())
            //{
            //    var lstHangDoiLamChiDinh = lstHangDoi
            //        .Where(x => x.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
            //        .OrderBy(x => x.SoThuTu).ToList();
            //    var sttLamChiDinh = lstHangDoiLamChiDinh.Any() ? lstHangDoiLamChiDinh.Last().SoThuTu + 1 : 1;
            //    foreach (var hangDoi in lstHangDoi)
            //    {
            //        if (hangDoi.YeuCauTiepNhanId == yeuCauTiepNhanId && hangDoi.YeuCauKhamBenhId == yeuCauKhamBenhId
            //                                                         && hangDoi.YeuCauKhamBenh.TrangThai != EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
            //        {
            //            hangDoi.SoThuTu = sttLamChiDinh;
            //            hangDoi.YeuCauKhamBenh.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh;
            //            var lichSu = new YeuCauKhamBenhLichSuTrangThai()
            //            {
            //                TrangThaiYeuCauKhamBenh = hangDoi.YeuCauKhamBenh.TrangThai,
            //                MoTa = hangDoi.YeuCauKhamBenh.TrangThai.GetDescription()
            //            };
            //            hangDoi.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSu);
            //            break;
            //        }
            //    }

            //    await _phongBenhVienHangDoiRepository.UpdateAsync(lstHangDoi);
            //}
            #endregion

            #region Cập nhật ngày 25/03/2022

            var hangDoi = await _phongBenhVienHangDoiRepository.Table
                .Where(x => x.PhongBenhVienId == phongBenhVienId
                            && x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.YeuCauKhamBenhId == yeuCauKhamBenhId
                            && x.YeuCauKhamBenh.TrangThai != EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
                .FirstOrDefaultAsync();
            if (hangDoi != null)
            {
                hangDoi.YeuCauKhamBenh.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh;
                var lichSu = new YeuCauKhamBenhLichSuTrangThai()
                {
                    TrangThaiYeuCauKhamBenh = hangDoi.YeuCauKhamBenh.TrangThai,
                    MoTa = hangDoi.YeuCauKhamBenh.TrangThai.GetDescription()
                };
                hangDoi.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSu);
                await _phongBenhVienHangDoiRepository.UpdateAsync(hangDoi);
            }
            #endregion
        }


        public async Task<GridDataSource> GetChiDinhCuaBacSiKhacDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var arrayId = queryInfo.AdditionalSearchString.Split(';');

            var query = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauTiepNhanId == long.Parse(arrayId[0])
                            && x.YeuCauKhamBenhTruocId != long.Parse(arrayId[1]))
                //&& x.YeuCauKhamBenhTruocId != null)
                //&& x.NhanVienChiDinhId != currentUserId)
                .Select(x => new ChiDinhDichVuCuaBacSiKhacVo
                {
                    NhomId = EnumNhomGoiDichVu.DichVuKhamBenh,
                    Nhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                    TenNhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                    MaDichVu = x.MaDichVu,
                    TenDichVu = x.TenDichVu,
                    TenLoaiGia = x.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    SoLuong = 1,
                    TenNguoiChiDinh = x.NhanVienChiDinh.User.HoTen,
                    Id = x.Id,
                    CheckRowItem = false
                })
                .Union(_yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.YeuCauTiepNhanId == long.Parse(arrayId[0])
                                && x.YeuCauKhamBenhId != long.Parse(arrayId[1]))
                    //&& x.YeuCauKhamBenhId != null)
                    //&& x.NhanVienChiDinhId != currentUserId)
                    .Select(x => new ChiDinhDichVuCuaBacSiKhacVo
                    {
                        NhomId = EnumNhomGoiDichVu.DichVuKyThuat,
                        Nhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                        TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                        MaDichVu = x.MaDichVu,
                        TenDichVu = x.TenDichVu,
                        TenLoaiGia = x.NhomGiaDichVuKyThuatBenhVien.Ten,
                        SoLuong = x.SoLan,
                        TenNguoiChiDinh = x.NhanVienChiDinh.User.HoTen,
                        Id = x.Id,
                        CheckRowItem = false
                    }))
                .Union(_yeuCauDichVuGiuongRepository.TableNoTracking
                    .Where(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                && x.YeuCauTiepNhanId == long.Parse(arrayId[0])
                                && x.YeuCauKhamBenhId != long.Parse(arrayId[1]))
                    //&& x.YeuCauKhamBenhId != null)
                    //&& x.NhanVienChiDinhId != currentUserId)
                    .Select(x => new ChiDinhDichVuCuaBacSiKhacVo
                    {
                        TenNhom = EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                        MaDichVu = x.Ma,
                        TenDichVu = x.Ten,
                        TenLoaiGia = x.NhomGiaDichVuGiuongBenhVien.Ten,
                        SoLuong = 1,
                        TenNguoiChiDinh = x.NhanVienChiDinh.User.HoTen
                        //CheckRowItem = false == null vì in chỉ định không cho check
                    }))
                .ApplyLike(queryInfo.SearchTerms, x => x.MaDichVu, x => x.TenDichVu, x => x.TenNguoiChiDinh);

            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetChiDinhBacSiKhacTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var arrayId = queryInfo.AdditionalSearchString.Split(';');

            var query = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauTiepNhanId == long.Parse(arrayId[0])
                            && x.YeuCauKhamBenhTruocId != long.Parse(arrayId[1]))
                //&& x.YeuCauKhamBenhTruocId != null)
                // && x.NhanVienChiDinhId != currentUserId)
                .Select(x => new ChiDinhDichVuCuaBacSiKhacVo
                {
                    MaDichVu = x.MaDichVu,
                    TenDichVu = x.TenDichVu,
                })
                .Union(_yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.YeuCauTiepNhanId == long.Parse(arrayId[0])
                                && x.YeuCauKhamBenhId != long.Parse(arrayId[1]))
                    // && x.YeuCauKhamBenhId != null)
                    //&& x.NhanVienChiDinhId != currentUserId)
                    .Select(x => new ChiDinhDichVuCuaBacSiKhacVo
                    {
                        MaDichVu = x.MaDichVu,
                        TenDichVu = x.TenDichVu,
                    }))
                .Union(_yeuCauDichVuGiuongRepository.TableNoTracking
                    .Where(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                && x.YeuCauTiepNhanId == long.Parse(arrayId[0])
                                && x.YeuCauKhamBenhId != long.Parse(arrayId[1]))
                    //&& x.YeuCauKhamBenhId != null)
                    // && x.NhanVienChiDinhId != currentUserId)
                    .Select(x => new ChiDinhDichVuCuaBacSiKhacVo
                    {
                        MaDichVu = x.Ma,
                        TenDichVu = x.Ten,
                    }))
                .ApplyLike(queryInfo.SearchTerms, x => x.MaDichVu, x => x.TenDichVu, x => x.TenNguoiChiDinh);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<NoiThucHienDichVuAutoVo> AutoGetThongTinNoiThucHienDichVuGiuong(long dichVuGiuongBenhVienId)
        {
            var lisNoiThucHienTheoDichVuGiuong = await _dichVuGiuongBenhVienRepository.TableNoTracking
                .Where(x => x.Id == dichVuGiuongBenhVienId)
                .SelectMany(x => x.DichVuGiuongBenhVienNoiThucHiens)
                .Include(x => x.KhoaPhong).ThenInclude(y => y.PhongBenhViens).ToListAsync();
            if (!lisNoiThucHienTheoDichVuGiuong.Any())
            {
                return new NoiThucHienDichVuAutoVo();
            }

            var lstPhongId = lisNoiThucHienTheoDichVuGiuong
                .Where(x => x.PhongBenhVienId != null)
                .Select(x => x.PhongBenhVienId.Value)
                .Union(lisNoiThucHienTheoDichVuGiuong
                    .Where(x => x.KhoaPhongId != null)
                    .SelectMany(x => x.KhoaPhong.PhongBenhViens)
                    .Select(x => x.Id)
                ).Distinct().ToList();


            var noiThucHien = await _giuongBenhRepository.TableNoTracking
                .Where(x => x.PhongBenhVien.IsDisabled != true && lstPhongId.Any(y => y == x.PhongBenhVienId))
                .OrderBy(x => x.HoatDongGiuongBenhs.Count(y => y.ThoiDiemKetThuc == null))
                .Select(x => new NoiThucHienDichVuAutoVo()
                {
                    PhongBenhVienId = x.PhongBenhVienId,
                    GiuongBenhVienId = x.Id
                }).FirstOrDefaultAsync();
            return noiThucHien;
        }

        public async Task XuLyThemYeuCauDichVuKyThuatMultiselectAsync(ChiDinhDichVuKyThuatMultiselectVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet)
        {
            //todo: có cập nhật bỏ await
            var coBHYT = yeuCauTiepNhanChiTiet.CoBHYT ?? false;
            var yeuCauKhamBenh = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == yeuCauVo.YeuCauKhamBenhId);
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentPhongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var yeuCauDichVuKyThuatCuoiCung = yeuCauKhamBenh.YeuCauDichVuKyThuats.OrderByDescending(x => x.Id).FirstOrDefault();
            yeuCauVo.YeuCauDichVuKyThuatCuoiCungId = yeuCauDichVuKyThuatCuoiCung == null ? 0 : yeuCauDichVuKyThuatCuoiCung.Id;


            var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();

            var lstDichVuObj = new List<ItemChiDinhDichVuKyThuatVo>();
            var lstDichVuKyThuatBenhVienEntity = new List<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien>();
            foreach (var item in yeuCauVo.DichVuKyThuatBenhVienChiDinhs)
            {
                var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(item);
                lstDichVuObj.Add(itemObj);
            }

            if (lstDichVuObj.Any())
            {
                var lstDichVuKyThuatId = lstDichVuObj.Select(x => x.DichVuId).Distinct().ToList();
                lstDichVuKyThuatBenhVienEntity = _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Include(o => o.DichVuKyThuatBenhVienGiaBaoHiems)
                    .Include(o => o.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Include(o => o.DichVuKyThuat)
                    .Where(x => lstDichVuKyThuatId.Contains(x.Id))
                    .ToList();
            }

            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            // xử lý get thông tin dịch vụ kỹ thuật và thêm mới
            //foreach (var item in yeuCauVo.DichVuKyThuatBenhVienChiDinhs)
            foreach (var itemObj in lstDichVuObj)
            {
                //var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(item);

                var newYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
                {
                    DichVuKyThuatBenhVienId = itemObj.DichVuId,
                    NhomDichVuBenhVienId = itemObj.NhomId,
                    NoiThucHienId = itemObj.NoiThucHienId
                };

                // trường hợp đăng nhập ngoại viện
                if (yeuCauTiepNhanChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe &&
                    yeuCauVo.LoaiDangNhap == HinhThucKhamBenh.KhamDoanNgoaiVien)
                {
                    newYeuCauDichVuKyThuat.NoiThucHienId = yeuCauVo.NoiThucHienNgoaiVienTheoHopDongs.Select(x => x.KeyId).FirstOrDefault();
                }

                //var dvkt = _dichVuKyThuatBenhVienRepository.GetById(newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId, x => x.Include(o => o.DichVuKyThuatBenhVienGiaBaoHiems)
                //    .Include(o => o.DichVuKyThuatVuBenhVienGiaBenhViens)
                //    .Include(o => o.DichVuKyThuat));

                var dvkt = lstDichVuKyThuatBenhVienEntity.First(x => x.Id == newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId);

                var dvktGiaBH = dvkt.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date));
                
                var dvktGiaBV = dvkt.DichVuKyThuatVuBenhVienGiaBenhViens
                    .Where(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                    .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                    .ThenBy(x => x.CreatedOn)
                    .First();
                    //.First(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));

                var dtudDVKTBV = yeuCauTiepNhanChiTiet.DoiTuongUuDai?.DoiTuongUuDaiDichVuKyThuatBenhViens?.FirstOrDefault(o =>
                                            o.DichVuKyThuatBenhVienId == newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVuKyThuatBenhVien.CoUuDai == true);

                var duocHuongBaoHiem = coBHYT && yeuCauKhamBenh.DuocHuongBaoHiem && dvktGiaBH != null && dvktGiaBH.Gia != 0;

                if (duocHuongBaoHiem)
                {
                    newYeuCauDichVuKyThuat.DuocHuongBaoHiem = true;
                    newYeuCauDichVuKyThuat.BaoHiemChiTra = null;
                }
                else
                {
                    newYeuCauDichVuKyThuat.DuocHuongBaoHiem = false;
                    newYeuCauDichVuKyThuat.BaoHiemChiTra = null;
                }

                newYeuCauDichVuKyThuat.YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId;
                newYeuCauDichVuKyThuat.MaDichVu = dvkt.Ma;
                newYeuCauDichVuKyThuat.TenDichVu = dvkt.Ten;
                newYeuCauDichVuKyThuat.DuocHuongBaoHiem = duocHuongBaoHiem;
                newYeuCauDichVuKyThuat.Gia = dvktGiaBV.Gia;
                newYeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId = dvktGiaBV.NhomGiaDichVuKyThuatBenhVienId;
                newYeuCauDichVuKyThuat.NhomChiPhi = dvkt.DichVuKyThuat != null ? dvkt.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe;
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

                if (!yeuCauVo.ChuyenHangDoiSangLamChiDinh && yeuCauKhamBenh.TrangThai != EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh && (newYeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                    || newYeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                    || newYeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang
                    || newYeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem))
                {
                    yeuCauVo.ChuyenHangDoiSangLamChiDinh = true;
                }

                yeuCauKhamBenh.YeuCauDichVuKyThuats.Add(newYeuCauDichVuKyThuat);
            }

            if (yeuCauKhamBenh != null && yeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                yeuCauKhamBenh.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                yeuCauKhamBenh.NoiThucHienId = yeuCauKhamBenh.NoiDangKyId; // _userAgentHelper.GetCurrentNoiLLamViecId();
                yeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;

                YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                    MoTa = yeuCauKhamBenh.TrangThai.GetDescription()
                };
                yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
            }
        }

        private async Task<LookupItemVo> GetBacSiThucHienMacDinh(long noiThucHienId)
        {
            //todo: có cập nhật bỏ await
            var nhanVien = _hoatDongNhanVienRepository.TableNoTracking
                .Where(hd => hd.PhongBenhVienId == noiThucHienId && hd.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi)
                .Include(hd => hd.PhongBenhVien)
                .Select(hd => hd.NhanVien)
                .Select(s => new LookupItemVo
                {
                    DisplayName = s.User.HoTen,
                    KeyId = s.Id
                })
                .FirstOrDefault();
            return nhanVien;
        }

        #region Ghi nhận VTTH/Thuốc
        public async Task<List<KhoSapXepUuTienLookupItemVo>> GetListKhoSapXepUutienAsync(DropDownListRequestModel queryInfo)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var phongLamViecHienTai = await _phongBenhVienRepository.TableNoTracking.FirstAsync(x => x.Id == phongHienTaiId);

            //Update: Lấy tất cả các kho lẻ mà user dc phân quyền thuộc khoa đang login và kho tổng cấp 2
            var lstKhoUuTien = await _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || 
                                                                               p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 ||
                                                                               (
                                                                                    p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                                                                                    p.KhoaPhongId == phongLamViecHienTai.KhoaPhongId &&
                                                                                    p.KhoNhanVienQuanLys.Any(o => o.NhanVienId == currentUserId)
                                                                               ))
                                                                   .ApplyLike(queryInfo.Query, x => x.Ten)
                                                                   .OrderBy(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe)
                                                                   .ThenBy(p => p.Ten)
                                                                   .Select(item => new KhoSapXepUuTienLookupItemVo()
                                                                   {
                                                                       KeyId = item.Id,
                                                                       DisplayName = item.Ten,
                                                                       LoaiKho = item.LoaiKho
                                                                   })
                                                                   .Take(queryInfo.Take)
                                                                   .ToListAsync();

            return lstKhoUuTien;

            //var lstKhoUuTien =
            //    await _khoRepository.TableNoTracking
            //        .ApplyLike(queryInfo.Query, x => x.Ten)
            //        .Where(x => x.PhongBenhVienId == phongHienTaiId)
            //        //.OrderByDescending(x => x.PhongBenhVienId == phongHienTaiId).ThenBy(x => x.Ten)
            //        .Take(1)
            //        .Select(item => new KhoSapXepUuTienLookupItemVo()
            //        {
            //            KeyId = item.Id,
            //            DisplayName = item.Ten,
            //            LoaiKho = item.LoaiKho
            //        })
            //        .Union(_khoRepository.TableNoTracking
            //            .ApplyLike(queryInfo.Query, x => x.Ten)
            //            .Where(x => x.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
            //                    && x.KhoNhanVienQuanLys.Any(y => y.NhanVienId == currentUserId))
            //        .OrderBy(x => x.Ten)
            //        .Select(item => new KhoSapXepUuTienLookupItemVo()
            //        {
            //            KeyId = item.Id,
            //            DisplayName = item.Ten,
            //            LoaiKho = item.LoaiKho
            //        }))
            //        .Union(_khoRepository.TableNoTracking
            //            .ApplyLike(queryInfo.Query, x => x.Ten)
            //            .Where(x => x.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || x.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
            //            .OrderBy(x => x.Ten)
            //            .Select(item => new KhoSapXepUuTienLookupItemVo()
            //            {
            //                KeyId = item.Id,
            //                DisplayName = item.Ten,
            //                LoaiKho = item.LoaiKho
            //            }))
            //        .Distinct()
            //        .Take(queryInfo.Take)
            //        .ToListAsync();

            //lstKhoUuTien = lstKhoUuTien.GroupBy(x => x.KeyId).Select(x => x.First()).ToList();

            //return lstKhoUuTien;
        }

        public async Task<EnumLoaiKhoDuocPham> GetLoaiKhoAsync(long khoId)
        {
            return await _khoRepository.TableNoTracking.Where(p => p.Id == khoId).Select(p => p.LoaiKho).FirstAsync();
        }

        public async Task<List<DichVuCanGhiNhanVTTHThuocVo>> GetListDichVuCanGhiNhanVTTHThuocAsync(DropDownListRequestModel queryInfo)
        {
            var yeuCauKhamBenhId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);

            var yeuCauKhamBenh =
                await _yeuCauKhamBenhRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == yeuCauKhamBenhId);

            if (yeuCauKhamBenh == null)
            {
                return new List<DichVuCanGhiNhanVTTHThuocVo>();
            }


            //var lstDichVu = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .ApplyLike(queryInfo.Query, x => x.Ten, x => x.Ma)
            //    .Where(x => x.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId)
            //    .Select(item => new DichVuCanGhiNhanVTTHThuocVo
            //    {
            //        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
            //        Ma = item.Ma,
            //        Ten = item.Ten,
            //        Id = yeuCauKhamBenhId,
            //        NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKhamBenh
            //    })
            //    .Union(_dichVuKyThuatBenhVienRepository.TableNoTracking
            //        .ApplyLike(queryInfo.Query, x => x.Ten, x => x.Ma)
            //        .Where(x => x.YeuCauDichVuKyThuats.Any(y => y.YeuCauKhamBenhId == yeuCauKhamBenhId && y.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            //        .SelectMany(x => x.YeuCauDichVuKyThuats.Where(y => y.YeuCauKhamBenhId == yeuCauKhamBenhId))
            //        .Select(item => new DichVuCanGhiNhanVTTHThuocVo
            //        {
            //            DisplayName = item.DichVuKyThuatBenhVien.Ten,//item.DichVuKyThuatBenhVien.Ma + " - " + item.DichVuKyThuatBenhVien.Ten,
            //            Ma = item.DichVuKyThuatBenhVien.Ma,
            //            Ten = item.DichVuKyThuatBenhVien.Ten,
            //            Id = item.Id,
            //            NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKyThuat
            //        })).Distinct()
            //        .Take(queryInfo.Take).ToListAsync();

            var lstDichVu = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .ApplyLike(queryInfo.Query, x => x.Ten, x => x.Ma)
                .Where(x => x.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId)
                .Select(item => new DichVuCanGhiNhanVTTHThuocVo
                {
                    DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    Id = yeuCauKhamBenhId,
                    NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKhamBenh
                })
                .Take(1)
                .ToList();

            var lstDvKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Select(item => new DichVuCanGhiNhanVTTHThuocVo
                    {
                        DisplayName = item.DichVuKyThuatBenhVien.Ten,//item.DichVuKyThuatBenhVien.Ma + " - " + item.DichVuKyThuatBenhVien.Ten,
                        Ma = item.DichVuKyThuatBenhVien.Ma,
                        Ten = item.DichVuKyThuatBenhVien.Ten,
                        Id = item.Id,
                        NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKyThuat
                    })
                    .ApplyLike(queryInfo.Query, x => x.Ten, x => x.Ma)
                    .Take(queryInfo.Take)
                    .ToList();

            lstDichVu.AddRange(lstDvKyThuat);

            return lstDichVu.Take(queryInfo.Take).ToList();
        }

        public async Task<List<VatTuThuocTieuHaoVo>> GetListVatTuTieuHaoThuocAsync(DropDownListRequestModel queryInfo)
        {
            var queryObj = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(queryInfo.ParameterDependencies);
            var khoId = queryObj.KhoId;
            var isBHYT = queryObj.LaDuocPhamBHYT;

            //todo: cần update lại search fulltext
            //var lstVatTuThuoc = await _duocPhamRepository.TableNoTracking
            //    .Where(x => x.DuocPhamBenhVien != null 
            //                //&& x.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(y => y.NhapKhoDuocPhams.KhoId == khoId) 
            //                && x.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(y => y.NhapKhoDuocPhams.KhoId == khoId && y.LaDuocPhamBHYT == isBHYT && (y.SoLuongDaXuat < y.SoLuongNhap)))
            //    .Select(item => new VatTuThuocTieuHaoVo
            //    {
            //        DisplayName = item.Ten,
            //        Id = item.DuocPhamBenhVien.Id,
            //        DonViTinh = item.DonViTinh.Ma == "Khác" ? item.DonViTinhThamKhao : item.DonViTinh.Ten,
            //        SoLuongTon = item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == khoId && x.NhapKhoDuocPhams.DaHet != true && x.LaDuocPhamBHYT == isBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
            //        NhomDichVu = (int)EnumNhomGoiDichVu.DuocPham
            //    })
            //    .Union(_vatTuRepository.TableNoTracking
            //        .Where(x => x.VatTuBenhVien != null 
            //                    && x.VatTuBenhVien.NhapKhoVatTuChiTiets.Any(y => y.NhapKhoVatTu.KhoId == khoId) 
            //                    && x.VatTuBenhVien.NhapKhoVatTuChiTiets.Any(y => y.LaVatTuBHYT == isBHYT && (y.SoLuongDaXuat < y.SoLuongNhap)))
            //        .Select(item => new VatTuThuocTieuHaoVo
            //        {
            //            DisplayName = item.VatTuBenhVien.VatTus.Ten,
            //            Id = item.VatTuBenhVien.Id,
            //            DonViTinh = item.DonViTinh,
            //            SoLuongTon = item.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(x => x.NhapKhoVatTu.KhoId == khoId && x.NhapKhoVatTu.DaHet != true && x.LaVatTuBHYT == isBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
            //            NhomDichVu = (int)EnumNhomGoiDichVu.VatTuTieuHao
            //        })).Distinct().Take(queryInfo.Take).ToListAsync();


            var duocPhamVaVatTus = await _duocPhamVaVatTuBenhVienService.GetDuocPhamVaVatTuTrongKho(isBHYT, queryInfo.Query, khoId ?? 0, queryInfo.Take);
            var lstVatTuThuoc = duocPhamVaVatTus
                .Where(x => x.SoLuongTon > 0)
                .Select(s => new VatTuThuocTieuHaoVo
                {
                    //DisplayName = s.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? s.Ten + " - " + s.HoatChat : s.Ten,
                    DisplayName = s.Ten,
                    Id = s.Id,
                    DonViTinh = s.DonViTinh,
                    SoLuongTon = s.SoLuongTon,
                    NhomDichVu = s.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? (int)EnumNhomGoiDichVu.DuocPham : (int)EnumNhomGoiDichVu.VatTuTieuHao,
                    HamLuong = s.HamLuong,
                    NhaSanXuat = s.NhaSanXuat,
                    HoatChat = s.HoatChat,
                    DuongDung = s.DuongDung
                }).ToList();


            return lstVatTuThuoc;
        }

        public async Task XuLyThemGhiNhanVatTuBenhVienAsync(ChiDinhGhiNhanVatTuThuocTieuHaoVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet)
        {
            var thongTinDichVuChiDinh = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(yeuCauVo.DichVuChiDinhId);
            var thongTinDichVuGhiNhan = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(yeuCauVo.DichVuGhiNhanId);
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var kho = await _khoRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauVo.KhoId);

            if (kho == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // trường hợp ghi nhận dược phẩm
            if (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham) // xử lý ghi nhận dược phẩm
            {
                var yeuCauDuocPham = new YeuCauDuocPhamBenhVien()
                {
                    YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                    KhoLinhId = yeuCauVo.KhoId
                };

                var yeuCauKhamBenh = await _yeuCauKhamBenhRepository.TableNoTracking
                    .Include(x => x.YeuCauTiepNhan)
                    .FirstAsync(x => x.Id == yeuCauVo.YeuCauKhamBenhId);

                if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    yeuCauDuocPham.YeuCauKhamBenhId = thongTinDichVuChiDinh.Id;
                }
                else if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    yeuCauDuocPham.YeuCauDichVuKyThuatId = thongTinDichVuChiDinh.Id;
                }

                var duocPhamBenhVien = await _duocPhamBenhVienService.TableNoTracking
                    .Include(x => x.DuocPham).ThenInclude(y => y.HopDongThauDuocPhamChiTiets)
                    //.Include(x => x.DuocPhamBenhVienGiaBaoHiems)
                    //.ThenInclude(y => y.HopDongThauDuocPhamChiTiets)
                    .FirstOrDefaultAsync(x => x.Id == thongTinDichVuGhiNhan.Id);
                if (duocPhamBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.DichVuChiDinhId.NotExists"));
                }

                if (kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ||
                    kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                {
                    yeuCauDuocPham.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
                }
                else
                {
                    yeuCauDuocPham.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu;
                    yeuCauVo.LaLinhBu = true;
                }

                yeuCauDuocPham.DuocPhamBenhVienId = duocPhamBenhVien.Id;
                yeuCauDuocPham.Ten = duocPhamBenhVien.DuocPham.Ten;
                yeuCauDuocPham.TenTiengAnh = duocPhamBenhVien.DuocPham.TenTiengAnh;
                yeuCauDuocPham.SoDangKy = duocPhamBenhVien.DuocPham.SoDangKy;
                yeuCauDuocPham.STTHoatChat = duocPhamBenhVien.DuocPham.STTHoatChat;
                yeuCauDuocPham.MaHoatChat = duocPhamBenhVien.DuocPham.MaHoatChat;
                yeuCauDuocPham.HoatChat = duocPhamBenhVien.DuocPham.HoatChat;
                yeuCauDuocPham.LoaiThuocHoacHoatChat = duocPhamBenhVien.DuocPham.LoaiThuocHoacHoatChat;
                yeuCauDuocPham.NhaSanXuat = duocPhamBenhVien.DuocPham.NhaSanXuat;
                yeuCauDuocPham.NuocSanXuat = duocPhamBenhVien.DuocPham.NuocSanXuat;
                yeuCauDuocPham.DuongDungId = duocPhamBenhVien.DuocPham.DuongDungId;
                yeuCauDuocPham.HamLuong = duocPhamBenhVien.DuocPham.HamLuong;
                yeuCauDuocPham.QuyCach = duocPhamBenhVien.DuocPham.QuyCach;
                yeuCauDuocPham.TieuChuan = duocPhamBenhVien.DuocPham.TieuChuan;
                yeuCauDuocPham.DangBaoChe = duocPhamBenhVien.DuocPham.DangBaoChe;
                yeuCauDuocPham.DonViTinhId = duocPhamBenhVien.DuocPham.DonViTinhId;
                yeuCauDuocPham.HuongDan = duocPhamBenhVien.DuocPham.HuongDan;
                yeuCauDuocPham.MoTa = duocPhamBenhVien.DuocPham.MoTa;
                yeuCauDuocPham.ChiDinh = duocPhamBenhVien.DuocPham.ChiDinh;
                yeuCauDuocPham.ChongChiDinh = duocPhamBenhVien.DuocPham.ChongChiDinh;
                yeuCauDuocPham.LieuLuongCachDung = duocPhamBenhVien.DuocPham.LieuLuongCachDung;
                yeuCauDuocPham.TacDungPhu = duocPhamBenhVien.DuocPham.TacDungPhu;
                yeuCauDuocPham.ChuYdePhong = duocPhamBenhVien.DuocPham.ChuYDePhong;


                //thông tin thầu
                //yeuCauDuocPham.HopDongThauDuocPhamId = duocPhamBenhVien.DuocPham.DangBaoChe;
                //yeuCauDuocPham.NhaThauId = duocPhamBenhVien.DuocPham.DonViTinhId;
                //yeuCauDuocPham.SoHopDongThau = duocPhamBenhVien.DuocPham.HuongDan;
                //yeuCauDuocPham.SoQuyetDinhThau = duocPhamBenhVien.DuocPham.MoTa;
                //yeuCauDuocPham.LoaiThau = duocPhamBenhVien.DuocPham.ChiDinh;
                //yeuCauDuocPham.LoaiThuocThau = duocPhamBenhVien.DuocPham.ChongChiDinh;
                //yeuCauDuocPham.NhomThau = duocPhamBenhVien.DuocPham.HamLuong;
                //yeuCauDuocPham.GoiThau = duocPhamBenhVien.DuocPham.QuyCach;
                //yeuCauDuocPham.NamThau = duocPhamBenhVien.DuocPham.TieuChuan;


                yeuCauDuocPham.KhongTinhPhi = !yeuCauVo.TinhPhi;
                yeuCauDuocPham.LaDuocPhamBHYT = yeuCauVo.LaDuocPhamBHYT;


                yeuCauDuocPham.SoLuong = yeuCauVo.SoLuong.Value;
                yeuCauDuocPham.NhanVienChiDinhId = currentUserId;
                yeuCauDuocPham.NoiChiDinhId = phongHienTaiId;
                yeuCauDuocPham.ThoiDiemChiDinh = DateTime.Now;

                yeuCauDuocPham.DaCapThuoc = false;
                yeuCauDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.ChuaThucHien;
                yeuCauDuocPham.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;

                // thông tin bảo hiểm
                //var giaBaoHiem = duocPhamBenhVien.DuocPhamBenhVienGiaBaoHiems.FirstOrDefault(x => x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));

                //if (giaBaoHiem != null)
                //{
                //    yeuCauDuocPham.DonGiaBaoHiem = giaBaoHiem.Gia;
                //    yeuCauDuocPham.TiLeBaoHiemThanhToan = giaBaoHiem.TiLeBaoHiemThanhToan;

                //}
                yeuCauDuocPham.DuocHuongBaoHiem = yeuCauVo.LaDuocPhamBHYT;

                var lstNhapChiTietTheoDuocPham = new List<NhapKhoDuocPhamChiTiet>();
                if (!yeuCauVo.NhapKhoDuocPhamChiTiets.Any())
                {
                    lstNhapChiTietTheoDuocPham = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauVo.KhoId
                                    && x.DuocPhamBenhVienId == duocPhamBenhVien.Id
                                    && x.NhapKhoDuocPhams.DaHet != true
                                    && x.LaDuocPhamBHYT == yeuCauVo.LaDuocPhamBHYT
                                    && x.SoLuongNhap > x.SoLuongDaXuat

                                    //BVHD-3821
                                    // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                                    && x.HanSuDung.Date >= DateTime.Now.Date)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToListAsync();
                }
                else
                {
                    lstNhapChiTietTheoDuocPham = yeuCauVo.NhapKhoDuocPhamChiTiets.Where(x =>
                            x.DuocPhamBenhVienId == duocPhamBenhVien.Id
                            && x.LaDuocPhamBHYT == yeuCauVo.LaDuocPhamBHYT
                            && x.SoLuongNhap > x.SoLuongDaXuat

                            //BVHD-3821
                            // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                            && x.HanSuDung.Date >= DateTime.Now.Date)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToList();
                }
                //if ((lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) || (lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVo.SoLuong))
                //{
                //    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                //}

                if (yeuCauDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                {
                    var soLuongTonTrongKho = Math.Round(lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat), 2);
                    if ((soLuongTonTrongKho < 0) || (soLuongTonTrongKho < yeuCauVo.SoLuong))
                    {
                        throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }

                    // xử lý thêm yêu cầu dược phẩm, với mỗi thông tin giá, VAT, tỉ lệ tháp giá khác nhau tạo 1 yêu cầu
                    // thêm xuất kho dược phẩm chi tiết
                    var yeuCauNew = yeuCauDuocPham.Clone();

                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                    {
                        DuocPhamBenhVienId = thongTinDichVuGhiNhan.Id
                    };

                    var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();
                    foreach (var item in lstNhapChiTietTheoDuocPham)
                    {
                        if (yeuCauVo.SoLuong > 0)
                        {
                            var giaTheoHopDong = duocPhamBenhVien.DuocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == item.HopDongThauDuocPhamId).Gia;
                            var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;

                            var tileBHYTThanhToanTheoNhap = item.LaDuocPhamBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;
                            if (yeuCauNew.DonGiaNhap != 0
                                && (yeuCauNew.DonGiaNhap != item.DonGiaNhap || yeuCauNew.VAT != item.VAT || yeuCauNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap))
                            {
                                yeuCauNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                                yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat), 2);
                                lstYeuCau.Add(yeuCauNew);

                                yeuCauNew = yeuCauDuocPham.Clone();
                                yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauNew.VAT = item.VAT;
                                yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap; //item.TiLeBHYTThanhToan ?? 100;

                                xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                                {
                                    DuocPhamBenhVienId = thongTinDichVuGhiNhan.Id
                                };
                            }
                            else
                            {
                                yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauNew.VAT = item.VAT;
                                yeuCauNew.PhuongPhapTinhGiaTriTonKho = item.PhuongPhapTinhGiaTriTonKho;
                                yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap; //item.TiLeBHYTThanhToan ?? 100;
                            }

                            var xuatViTri = new XuatKhoDuocPhamChiTietViTri()
                            {
                                NhapKhoDuocPhamChiTietId = item.Id
                            };

                            //var tonTheoItem = item.SoLuongNhap - item.SoLuongDaXuat;
                            var tonTheoItem = Math.Round(item.SoLuongNhap - item.SoLuongDaXuat, 2);
                            if (yeuCauVo.SoLuong < tonTheoItem || yeuCauVo.SoLuong.Value.AlmostEqual(tonTheoItem))
                            {
                                xuatViTri.SoLuongXuat = yeuCauVo.SoLuong.Value;
                                item.SoLuongDaXuat = Math.Round(item.SoLuongDaXuat + yeuCauVo.SoLuong.Value, 2);
                                yeuCauVo.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                yeuCauVo.SoLuong = Math.Round(yeuCauVo.SoLuong.Value - tonTheoItem, 2);
                            }

                            xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);

                            if (yeuCauVo.SoLuong.Value.AlmostEqual(0))
                            {
                                yeuCauNew.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                                yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat), 2);
                                lstYeuCau.Add(yeuCauNew);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    //await _yeuCauDuocPhamBenhVienRepository.AddRangeAsync(lstYeuCau);
                    //await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(lstNhapKhoDuocPhamChiTiet);

                    foreach (var item in lstYeuCau)
                    {
                        yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Add(item);
                    }

                    yeuCauVo.NhapKhoDuocPhamChiTiets = lstNhapChiTietTheoDuocPham;
                }

                if (yeuCauDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                {
                    if ((lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) || (lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVo.SoLuong))
                    {
                        throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }

                    // tạo tạm thông tin yêu cầu dược phẩm khi ghi nhận trực tiếp
                    // khi duyệt phiếu lĩnh trực tiếp sẽ xử lý cập nhật lại thông tin đúng theo nhập chi tiết
                    // cập nhật yêu cầu dược phẩm (sửa, thêm), tạo phiếu xuất
                    if (yeuCauVo.SoLuong != null && yeuCauVo.SoLuong > 0)
                    {
                        var yeuCauNew = yeuCauDuocPham.Clone();

                        var thongTinNhapDuocPham = lstNhapChiTietTheoDuocPham.OrderByDescending(x => x.HanSuDung).First();
                        var giaTheoHopDong = duocPhamBenhVien.DuocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == thongTinNhapDuocPham.HopDongThauDuocPhamId).Gia;
                        var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                        yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                        yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                        yeuCauNew.PhuongPhapTinhGiaTriTonKho = thongTinNhapDuocPham.PhuongPhapTinhGiaTriTonKho;
                        yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                        yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                        yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                        yeuCauNew.SoLuong = yeuCauVo.SoLuong ?? 0;
                        yeuCauVo.SoLuong = 0;
                        yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Add(yeuCauNew);
                    }

                    //var lstYeuCau = new List<YeuCauDuocPhamBenhVien>();
                    //double soLuongXuat = 0;
                    //foreach (var item in lstNhapChiTietTheoDuocPham)
                    //{
                    //    if (yeuCauVo.SoLuong > 0)
                    //    {
                    //        if (yeuCauNew.DonGiaNhap != 0
                    //            && (yeuCauNew.DonGiaNhap != item.DonGiaNhap || yeuCauNew.VAT != item.VAT || yeuCauNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != item.TiLeBHYTThanhToan))
                    //        {
                    //            //yeuCauNew.SoLuong = soLuongXuat;
                    //            //lstYeuCau.Add(yeuCauNew);

                    //            yeuCauNew = yeuCauDuocPham.Clone();
                    //            yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                    //            yeuCauNew.VAT = item.VAT;
                    //            yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                    //            yeuCauNew.DonGiaBaoHiem = item.DonGiaNhap;
                    //            yeuCauNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                    //        }
                    //        else
                    //        {
                    //            yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                    //            yeuCauNew.VAT = item.VAT;
                    //            yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                    //            yeuCauNew.DonGiaBaoHiem = item.DonGiaNhap;
                    //            yeuCauNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                    //        }

                    //        var tonTheoItem = item.SoLuongNhap - item.SoLuongDaXuat;
                    //        if (yeuCauVo.SoLuong <= tonTheoItem)
                    //        {
                    //            soLuongXuat += yeuCauVo.SoLuong.Value;
                    //            yeuCauVo.SoLuong = 0;
                    //        }
                    //        else
                    //        {
                    //            soLuongXuat += item.SoLuongNhap;
                    //            yeuCauVo.SoLuong -= tonTheoItem;
                    //        }

                    //        if (yeuCauVo.SoLuong == 0)
                    //        {
                    //            yeuCauNew.SoLuong = soLuongXuat;
                    //            lstYeuCau.Add(yeuCauNew);
                    //            break;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        break;
                    //    }
                    //}

                    //foreach (var item in lstYeuCau)
                    //{
                    //    yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Add(item);
                    //}
                }
            }

            // trường hợp ghi nhận vật tư
            else //thongTinDichVuGhiNhan.NhomId == (int) EnumNhomGoiDichVu.VatTu
            {
                var yeuCauVatTu = new YeuCauVatTuBenhVien()
                {
                    YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                    KhoLinhId = yeuCauVo.KhoId
                };

                var yeuCauKhamBenh = await _yeuCauKhamBenhRepository.TableNoTracking
                    .Include(x => x.YeuCauTiepNhan)
                    .FirstAsync(x => x.Id == yeuCauVo.YeuCauKhamBenhId);

                if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    yeuCauVatTu.YeuCauKhamBenhId = thongTinDichVuChiDinh.Id;
                }
                else if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    yeuCauVatTu.YeuCauDichVuKyThuatId = thongTinDichVuChiDinh.Id;
                }

                var vatTuBenhVien = await _vatTuBenhVienRepository.TableNoTracking
                    .Include(x => x.VatTus)
                    .ThenInclude(y => y.HopDongThauVatTuChiTiets)
                    .FirstOrDefaultAsync(x => x.Id == thongTinDichVuGhiNhan.Id);
                if (vatTuBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.DichVuChiDinhId.NotExists"));
                }

                if (kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 ||
                    kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                {
                    yeuCauVatTu.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
                }
                else
                {
                    yeuCauVatTu.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhBu;
                    yeuCauVo.LaLinhBu = true;
                }

                yeuCauVatTu.VatTuBenhVienId = vatTuBenhVien.Id;
                yeuCauVatTu.Ten = vatTuBenhVien.VatTus.Ten;
                yeuCauVatTu.Ma = vatTuBenhVien.VatTus.Ma;
                yeuCauVatTu.NhomVatTuId = vatTuBenhVien.VatTus.NhomVatTuId;
                yeuCauVatTu.DonViTinh = vatTuBenhVien.VatTus.DonViTinh;
                yeuCauVatTu.NhaSanXuat = vatTuBenhVien.VatTus.NhaSanXuat;
                yeuCauVatTu.NuocSanXuat = vatTuBenhVien.VatTus.NuocSanXuat;
                yeuCauVatTu.QuyCach = vatTuBenhVien.VatTus.QuyCach;
                yeuCauVatTu.MoTa = vatTuBenhVien.VatTus.MoTa;


                //thông tin thầu
                //yeuCauDuocPham.HopDongThauDuocPhamId = duocPhamBenhVien.DuocPham.DangBaoChe;
                //yeuCauDuocPham.NhaThauId = duocPhamBenhVien.DuocPham.DonViTinhId;
                //yeuCauDuocPham.SoHopDongThau = duocPhamBenhVien.DuocPham.HuongDan;
                //yeuCauDuocPham.SoQuyetDinhThau = duocPhamBenhVien.DuocPham.MoTa;
                //yeuCauDuocPham.LoaiThau = duocPhamBenhVien.DuocPham.ChiDinh;
                //yeuCauDuocPham.LoaiThuocThau = duocPhamBenhVien.DuocPham.ChongChiDinh;
                //yeuCauDuocPham.NhomThau = duocPhamBenhVien.DuocPham.HamLuong;
                //yeuCauDuocPham.GoiThau = duocPhamBenhVien.DuocPham.QuyCach;
                //yeuCauDuocPham.NamThau = duocPhamBenhVien.DuocPham.TieuChuan;

                yeuCauVatTu.KhongTinhPhi = !yeuCauVo.TinhPhi;
                yeuCauVatTu.LaVatTuBHYT = yeuCauVo.LaDuocPhamBHYT;

                yeuCauVatTu.SoLuong = yeuCauVo.SoLuong.Value;


                yeuCauVatTu.NhanVienChiDinhId = currentUserId;
                yeuCauVatTu.NoiChiDinhId = phongHienTaiId;
                yeuCauVatTu.ThoiDiemChiDinh = DateTime.Now;

                yeuCauVatTu.DaCapVatTu = false;
                yeuCauVatTu.TrangThai = EnumYeuCauVatTuBenhVien.ChuaThucHien;
                yeuCauVatTu.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;

                // thông tin bảo hiểm
                //var giaBaoHiem = vatTuBenhVien.VatTuBenhVienGiaBaoHiems.FirstOrDefault(x => x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));

                //            if (giaBaoHiem != null)
                //            {
                //                yeuCauVatTu.DonGiaBaoHiem = giaBaoHiem.Gia;
                //                yeuCauVatTu.TiLeBaoHiemThanhToan = giaBaoHiem.TiLeBaoHiemThanhToan;

                //            }
                yeuCauVatTu.DuocHuongBaoHiem = yeuCauVo.LaDuocPhamBHYT;
                yeuCauVatTu.LoaiNoiChiDinh = yeuCauVo.LoaiNoiChiDinh;

                var lstNhapChiTietTheoVatTu = new List<NhapKhoVatTuChiTiet>();
                if (!yeuCauVo.NhapKhoVatTuChiTiets.Any())
                {
                    lstNhapChiTietTheoVatTu = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoVatTu.KhoId == yeuCauVo.KhoId
                                    && x.VatTuBenhVienId == vatTuBenhVien.Id
                                    && x.NhapKhoVatTu.DaHet != true
                                    && x.LaVatTuBHYT == yeuCauVo.LaDuocPhamBHYT
                                    && x.SoLuongNhap > x.SoLuongDaXuat

                                    //BVHD-3821
                                    // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                                    && x.HanSuDung.Date >= DateTime.Now.Date)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        .ToListAsync();
                }
                else
                {
                    lstNhapChiTietTheoVatTu = yeuCauVo.NhapKhoVatTuChiTiets.Where(x =>
                            x.VatTuBenhVienId == vatTuBenhVien.Id
                            && x.LaVatTuBHYT == yeuCauVo.LaDuocPhamBHYT
                            && x.SoLuongNhap > x.SoLuongDaXuat

                            //BVHD-3821
                            // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                            && x.HanSuDung.Date >= DateTime.Now.Date)
                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                }

                //if ((lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) || (lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVo.SoLuong))
                //{
                //    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                //}

                if (yeuCauVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                {
                    var soLuongTonTrongKho = Math.Round(lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat), 2);
                    if ((soLuongTonTrongKho < 0) || (soLuongTonTrongKho < yeuCauVo.SoLuong))
                    {
                        throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }

                    var yeuCauNew = yeuCauVatTu.Clone();
                    var xuatChiTiet = new XuatKhoVatTuChiTiet()
                    {
                        VatTuBenhVienId = thongTinDichVuGhiNhan.Id
                    };

                    var lstYeuCau = new List<YeuCauVatTuBenhVien>();
                    foreach (var item in lstNhapChiTietTheoVatTu)
                    {
                        if (yeuCauVo.SoLuong > 0)
                        {
                            var giaTheoHopDong = vatTuBenhVien.VatTus.HopDongThauVatTuChiTiets.First(o => o.HopDongThauVatTuId == item.HopDongThauVatTuId).Gia;
                            var donGiaBaoHiem = item.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : item.DonGiaNhap;

                            var tileBHYTThanhToanTheoNhap = item.LaVatTuBHYT ? item.TiLeBHYTThanhToan ?? 100 : 0;
                            if (yeuCauNew.DonGiaNhap != 0
                                && (yeuCauNew.DonGiaNhap != item.DonGiaNhap || yeuCauNew.VAT != item.VAT || yeuCauNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != tileBHYTThanhToanTheoNhap))
                            {
                                yeuCauNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                                yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat), 2);
                                lstYeuCau.Add(yeuCauNew);

                                yeuCauNew = yeuCauVatTu.Clone();
                                yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauNew.VAT = item.VAT;
                                yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap;

                                xuatChiTiet = new XuatKhoVatTuChiTiet()
                                {
                                    VatTuBenhVienId = thongTinDichVuGhiNhan.Id
                                };
                            }
                            else
                            {
                                yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                                yeuCauNew.VAT = item.VAT;
                                yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tileBHYTThanhToanTheoNhap; //item.TiLeBHYTThanhToan ?? 100;
                            }

                            var xuatViTri = new XuatKhoVatTuChiTietViTri()
                            {
                                NhapKhoVatTuChiTietId = item.Id
                            };

                            //var tonTheoItem = item.SoLuongNhap - item.SoLuongDaXuat;
                            var tonTheoItem = Math.Round(item.SoLuongNhap - item.SoLuongDaXuat, 2);
                            if (yeuCauVo.SoLuong < tonTheoItem || yeuCauVo.SoLuong.Value.AlmostEqual(tonTheoItem))
                            {
                                xuatViTri.SoLuongXuat = yeuCauVo.SoLuong.Value;
                                item.SoLuongDaXuat = Math.Round(item.SoLuongDaXuat + yeuCauVo.SoLuong.Value, 2);
                                yeuCauVo.SoLuong = 0;
                            }
                            else
                            {
                                xuatViTri.SoLuongXuat = tonTheoItem;
                                item.SoLuongDaXuat = item.SoLuongNhap;
                                yeuCauVo.SoLuong = Math.Round(yeuCauVo.SoLuong.Value - tonTheoItem, 2);
                            }

                            xuatChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatViTri);

                            if (yeuCauVo.SoLuong.Value.AlmostEqual(0))
                            {
                                yeuCauNew.XuatKhoVatTuChiTiet = xuatChiTiet;
                                yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat), 2);
                                lstYeuCau.Add(yeuCauNew);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    //yeuCauVatTu.XuatKhoVatTuChiTiet = xuatChiTiet;
                    //await _yeuCauVatTuBenhVienRepository.AddAsync(yeuCauVatTu);

                    //await _yeuCauVatTuBenhVienRepository.AddRangeAsync(lstYeuCau);
                    //await _nhapKhoVatTuChiTietRepository.UpdateAsync(lstNhapKhoVatTuChiTiet);

                    foreach (var item in lstYeuCau)
                    {
                        yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Add(item);
                    }
                    yeuCauVo.NhapKhoVatTuChiTiets = lstNhapChiTietTheoVatTu;
                }

                if (yeuCauVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                {
                    if ((lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) || (lstNhapChiTietTheoVatTu.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVo.SoLuong))
                    {
                        throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                    }

                    // tạo tạm thông tin yêu cầu dược phẩm khi ghi nhận trực tiếp
                    // khi duyệt phiếu lĩnh trực tiếp sẽ xử lý cập nhật lại thông tin đúng theo nhập chi tiết
                    // cập nhật yêu cầu dược phẩm (sửa, thêm), tạo phiếu xuất
                    if (yeuCauVo.SoLuong != null && yeuCauVo.SoLuong > 0)
                    {
                        var yeuCauNew = yeuCauVatTu.Clone();

                        var thongTinNhapDuocPham = lstNhapChiTietTheoVatTu.First();
                        var giaTheoHopDong = vatTuBenhVien.VatTus.HopDongThauVatTuChiTiets.First(o => o.HopDongThauVatTuId == thongTinNhapDuocPham.HopDongThauVatTuId).Gia;
                        var donGiaBaoHiem = thongTinNhapDuocPham.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : thongTinNhapDuocPham.DonGiaNhap;

                        yeuCauNew.DonGiaNhap = thongTinNhapDuocPham.DonGiaNhap;
                        yeuCauNew.VAT = thongTinNhapDuocPham.VAT;
                        yeuCauNew.TiLeTheoThapGia = thongTinNhapDuocPham.TiLeTheoThapGia;
                        yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                        yeuCauNew.TiLeBaoHiemThanhToan = thongTinNhapDuocPham.TiLeBHYTThanhToan ?? 100;
                        yeuCauNew.SoLuong = yeuCauVo.SoLuong ?? 0;
                        yeuCauVo.SoLuong = 0;
                        yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Add(yeuCauNew);
                    }

                    //var yeuCauNew = yeuCauVatTu.Clone();

                    //var lstYeuCau = new List<YeuCauVatTuBenhVien>();
                    //double soLuongXuat = 0;
                    //foreach (var item in lstNhapChiTietTheoVatTu)
                    //{
                    //    if (yeuCauVo.SoLuong > 0)
                    //    {
                    //        if (yeuCauNew.DonGiaNhap != 0
                    //            && (yeuCauNew.DonGiaNhap != item.DonGiaNhap || yeuCauNew.VAT != item.VAT || yeuCauNew.TiLeTheoThapGia != item.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != item.TiLeBHYTThanhToan))
                    //        {
                    //            //yeuCauNew.SoLuong = soLuongXuat;
                    //            //lstYeuCau.Add(yeuCauNew);

                    //            yeuCauNew = yeuCauVatTu.Clone();
                    //            yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                    //            yeuCauNew.VAT = item.VAT;
                    //            yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                    //            yeuCauNew.DonGiaBaoHiem = item.DonGiaNhap;
                    //            yeuCauNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                    //        }
                    //        else
                    //        {
                    //            yeuCauNew.DonGiaNhap = item.DonGiaNhap;
                    //            yeuCauNew.VAT = item.VAT;
                    //            yeuCauNew.TiLeTheoThapGia = item.TiLeTheoThapGia;
                    //            yeuCauNew.DonGiaBaoHiem = item.DonGiaNhap;
                    //            yeuCauNew.TiLeBaoHiemThanhToan = item.TiLeBHYTThanhToan ?? 100;
                    //        }

                    //        var tonTheoItem = item.SoLuongNhap - item.SoLuongDaXuat;
                    //        if (yeuCauVo.SoLuong <= tonTheoItem)
                    //        {
                    //            soLuongXuat += yeuCauVo.SoLuong.Value;
                    //            yeuCauVo.SoLuong = 0;
                    //        }
                    //        else
                    //        {
                    //            soLuongXuat += item.SoLuongNhap;
                    //            yeuCauVo.SoLuong -= tonTheoItem;
                    //        }

                    //        if (yeuCauVo.SoLuong == 0)
                    //        {
                    //            yeuCauNew.SoLuong = soLuongXuat;
                    //            lstYeuCau.Add(yeuCauNew);
                    //            break;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        break;
                    //    }
                    //}

                    //foreach (var item in lstYeuCau)
                    //{
                    //    yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Add(item);
                    //}
                }
            }
        }


        public async Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHThuocAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            var yeuCauKhamBenh =
                await _yeuCauKhamBenhRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == yeuCauKhamBenhId);

            // get list dịch vụ cần ghi nhận VTTH/Thuốc theo yêu cầu khám hiện tại
            var lstDichVu = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Where(x => x.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId)
                .Select(item => new DichVuCanGhiNhanVTTHThuocVo
                {
                    Id = yeuCauKhamBenhId,
                    NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKhamBenh
                }).Union(_dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => x.YeuCauDichVuKyThuats.Any(y => y.YeuCauKhamBenhId == yeuCauKhamBenhId && y.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                    .SelectMany(x => x.YeuCauDichVuKyThuats.Where(y => y.YeuCauKhamBenhId == yeuCauKhamBenhId))
                    .Select(item => new DichVuCanGhiNhanVTTHThuocVo
                    {
                        Id = item.Id,
                        NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKyThuat
                    })).Distinct().ToListAsync();

            var lstGhiNhanVTHHThuoc =
                await _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                            && (lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh && y.Id == x.YeuCauKhamBenhId)
                                || lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat && y.Id == x.YeuCauDichVuKyThuatId)))
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)EnumNhomGoiDichVu.DuocPham,
                    TenNhomYeuCau = EnumNhomGoiDichVu.DuocPham.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuat.Id != null ? item.YeuCauDichVuKyThuatId : item.YeuCauKhamBenhId,
                    NhomChiDinhId = (int)(item.YeuCauDichVuKyThuat != null ? Enums.EnumNhomGoiDichVu.DichVuKyThuat : Enums.EnumNhomGoiDichVu.DichVuKhamBenh),

                    MaDichVuYeuCau = item.DuocPhamBenhVien.DuocPham.SoDangKy,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,
                    DonViTinh = item.DonViTinh.Ten,
                    TenDuongDung = item.DuongDung.Ten,

                    DonGia = item.DonGiaBan,//item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaDuocPhamBHYT,

                    PhieuLinh = item.YeuCauLinhDuocPham != null ?
                        item.YeuCauLinhDuocPham.SoPhieu : (item.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false) ? item.YeuCauLinhDuocPhamChiTiets.Where(a => a.YeuCauLinhDuocPham.DuocDuyet != false).Select(a => a.YeuCauLinhDuocPham.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    // cập nhật 24/05/2021
                    //Ma = item.Ma,
                    Ten = item.Ten,
                    //DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.DuocPhamBenhVienId,
                    TinhTrang = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                    IsKhoTong = item.KhoLinh != null && (item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2),
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any(),
                })
                .Union(_yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                            && (lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh && y.Id == x.YeuCauKhamBenhId)
                                || lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat && y.Id == x.YeuCauDichVuKyThuatId)))
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)EnumNhomGoiDichVu.VatTuTieuHao,
                    TenNhomYeuCau = EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuat.Id != null ? item.YeuCauDichVuKyThuatId : item.YeuCauKhamBenhId,
                    NhomChiDinhId = (int)(item.YeuCauDichVuKyThuat != null ? Enums.EnumNhomGoiDichVu.DichVuKyThuat : Enums.EnumNhomGoiDichVu.DichVuKhamBenh),

                    MaDichVuYeuCau = item.Ma,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,

                    DonViTinh = item.DonViTinh,
                    TenDuongDung = "", //item.DuongDung,

                    DonGia = item.DonGiaBan, //item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaVatTuBHYT,

                    PhieuLinh = item.YeuCauLinhVatTu != null ?
                        item.YeuCauLinhVatTu.SoPhieu : (item.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false) ? item.YeuCauLinhVatTuChiTiets.Where(a => a.YeuCauLinhVatTu.DuocDuyet != false).Select(a => a.YeuCauLinhVatTu.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoVatTuChiTiet.XuatKhoVatTu != null ? item.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    // cập nhật 24/05/2021
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.VatTuBenhVienId,
                    TinhTrang = item.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                    IsKhoTong = item.KhoLinh != null && (item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2),
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraVatTuTuBenhNhanChiTiets.Any()

                }))
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToListAsync();

            return lstGhiNhanVTHHThuoc;
        }
        public async Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocAsyncVer2(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            //var yeuCauKhamBenh =
            //     _yeuCauKhamBenhRepository.TableNoTracking.FirstOrDefault(x => x.Id == yeuCauKhamBenhId);

            // get list dịch vụ cần ghi nhận VTTH/Thuốc theo yêu cầu khám hiện tại
            #region Cập nhật 05/12/2022
            //var lstDichVu = _dichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .Where(x => x.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId)
            //    .Select(item => new DichVuCanGhiNhanVTTHThuocVo
            //    {
            //        Id = yeuCauKhamBenhId,
            //        NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKhamBenh
            //    }).Union(_dichVuKyThuatBenhVienRepository.TableNoTracking
            //        .Where(x => x.YeuCauDichVuKyThuats.Any(y => y.YeuCauKhamBenhId == yeuCauKhamBenhId && y.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            //        .SelectMany(x => x.YeuCauDichVuKyThuats.Where(y => y.YeuCauKhamBenhId == yeuCauKhamBenhId))
            //        .Select(item => new DichVuCanGhiNhanVTTHThuocVo
            //        {
            //            Id = item.Id,
            //            NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKyThuat
            //        })).Distinct().ToList();

            var lstDichVu = new List<DichVuCanGhiNhanVTTHThuocVo>()
            {
                new DichVuCanGhiNhanVTTHThuocVo
                {
                    Id = yeuCauKhamBenhId,
                    NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKhamBenh
                }
            };

            var lstYcDichVuKyThuatId =
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                .Select(item => new DichVuCanGhiNhanVTTHThuocVo
                {
                    Id = item.Id,
                    NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKyThuat
                }).ToList();
            lstDichVu.AddRange(lstYcDichVuKyThuatId);
            #endregion

            var lstYeuCauKhamBenhId = lstDichVu.Where(x => x.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.Id).ToList();
            var lstYeuDichVuKyThuatId = lstDichVu.Where(x => x.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.Id).ToList();

            var lstGhiNhanVTHHThuoc = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                            //&& (lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh && y.Id == x.YeuCauKhamBenhId)
                            //    || lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat && y.Id == x.YeuCauDichVuKyThuatId)))
                            && ((x.YeuCauKhamBenhId != null && lstYeuCauKhamBenhId.Contains(x.YeuCauKhamBenhId.Value))
                                || (x.YeuCauDichVuKyThuatId != null && lstYeuDichVuKyThuatId.Contains(x.YeuCauDichVuKyThuatId.Value))))
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)EnumNhomGoiDichVu.DuocPham,
                    TenNhomYeuCau = EnumNhomGoiDichVu.DuocPham.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuat.Id != null ? item.YeuCauDichVuKyThuatId : item.YeuCauKhamBenhId,
                    NhomChiDinhId = (int)(item.YeuCauDichVuKyThuat != null ? Enums.EnumNhomGoiDichVu.DichVuKyThuat : Enums.EnumNhomGoiDichVu.DichVuKhamBenh),

                    MaDichVuYeuCau = item.DuocPhamBenhVien.DuocPham.SoDangKy,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,
                    DonViTinh = item.DonViTinh.Ten,
                    TenDuongDung = item.DuongDung.Ten,

                    DonGia = item.DonGiaBan,//item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaDuocPhamBHYT,

                    PhieuLinh = item.YeuCauLinhDuocPham != null ?
                        item.YeuCauLinhDuocPham.SoPhieu : (item.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false) ? item.YeuCauLinhDuocPhamChiTiets.Where(a => a.YeuCauLinhDuocPham.DuocDuyet != false).Select(a => a.YeuCauLinhDuocPham.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    // cập nhật 24/05/2021
                    //Ma = item.Ma,
                    Ten = item.Ten,
                    //DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.DuocPhamBenhVienId,
                    TinhTrang = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                    IsKhoTong = item.KhoLinh != null && (item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2),
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any(),

                    //BVHD-3905
                    TiLeThanhToanBHYT = item.DuocPhamBenhVien.TiLeThanhToanBHYT
                })
                .Union(_yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                            //&& (lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh && y.Id == x.YeuCauKhamBenhId)
                            //    || lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat && y.Id == x.YeuCauDichVuKyThuatId)))
                            && ((x.YeuCauKhamBenhId != null && lstYeuCauKhamBenhId.Contains(x.YeuCauKhamBenhId.Value))
                                || (x.YeuCauDichVuKyThuatId != null && lstYeuDichVuKyThuatId.Contains(x.YeuCauDichVuKyThuatId.Value))))
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)EnumNhomGoiDichVu.VatTuTieuHao,
                    TenNhomYeuCau = EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuat.Id != null ? item.YeuCauDichVuKyThuatId : item.YeuCauKhamBenhId,
                    NhomChiDinhId = (int)(item.YeuCauDichVuKyThuat != null ? Enums.EnumNhomGoiDichVu.DichVuKyThuat : Enums.EnumNhomGoiDichVu.DichVuKhamBenh),

                    MaDichVuYeuCau = item.Ma,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,

                    DonViTinh = item.DonViTinh,
                    TenDuongDung = "", //item.DuongDung,

                    DonGia = item.DonGiaBan, //item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaVatTuBHYT,

                    PhieuLinh = item.YeuCauLinhVatTu != null ?
                        item.YeuCauLinhVatTu.SoPhieu : (item.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false) ? item.YeuCauLinhVatTuChiTiets.Where(a => a.YeuCauLinhVatTu.DuocDuyet != false).Select(a => a.YeuCauLinhVatTu.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoVatTuChiTiet.XuatKhoVatTu != null ? item.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    // cập nhật 24/05/2021
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.VatTuBenhVienId,
                    TinhTrang = item.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu,
                    IsKhoTong = item.KhoLinh != null && (item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap1 || item.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2),
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraVatTuTuBenhNhanChiTiets.Any(),

                    //BVHD-3905
                    TiLeThanhToanBHYT = item.VatTuBenhVien.TiLeThanhToanBHYT
                }))
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToList();

            var result = lstGhiNhanVTHHThuoc
                .GroupBy(x => new { x.NhomYeuCauId, x.MaDichVuYeuCau, x.TenDichVuYeuCau, x.TenKho, x.YeuCauDichVuChiDinhId, x.NhomChiDinhId, x.TinhPhi, x.CreatedOn })
                .Select(item => new GhiNhanVatTuTieuHaoThuocGroupParentGridVo()
                {
                    NhomYeuCauId = (int)item.First().NhomYeuCauId,
                    TenDichVu = item.First().TenDichVu,
                    DichVuId = item.First().DichVuId,

                    YeuCauDichVuChiDinhId = item.First().YeuCauDichVuChiDinhId,
                    NhomChiDinhId = item.First().NhomChiDinhId,

                    MaDichVuYeuCau = item.First().MaDichVuYeuCau,
                    TenDichVuYeuCau = item.First().TenDichVuYeuCau,

                    KhoId = item.First().KhoId,
                    TenKho = item.First().TenKho,
                    LoaiKho = item.First().LoaiKho,

                    DonViTinh = item.First().DonViTinh,
                    TenDuongDung = item.First().TenDuongDung,
                    SoLuong = item.Sum(a => a.SoLuong),

                    ThanhTien = item.Sum(a => a.ThanhTien ?? 0),

                    //DonGiaBaoHiem = item.Select(a => a.DonGiaBaoHiem).Distinct().Count() > 1 ? null : item.Select(a => a.DonGiaBaoHiem).First(),
                    DuocHuongBaoHiem = item.First().DuocHuongBaoHiem,
                    LaBHYT = item.First().LaBHYT,

                    PhieuLinh = item.First().PhieuLinh,
                    PhieuXuat = item.First().PhieuXuat,

                    TinhPhi = item.First().TinhPhi,
                    CreatedOn = item.First().CreatedOn,
                    ThoiGianChiDinh = item.First().ThoiGianChiDinh,
                    TenNhanVienChiDinh = item.First().TenNhanVienChiDinh,

                    // cập nhật 24/05/2021
                    Ma = item.First().Ma,
                    Ten = item.First().Ten,
                    DVT = item.First().DonViTinh,
                    NhaSX = item.First().NhaSX,
                    NuocSX = item.First().NuocSX,

                    VatTuThuocBenhVienId = item.First().VatTuThuocBenhVienId,
                    TinhTrang = item.First().TinhTrang,
                    IsKhoLe = item.First().IsKhoLe,
                    IsKhoTong = item.First().IsKhoTong,
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.Any(p => p.IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet),

                    YeuCauGhiNhanVTTHThuocs = item.ToList(),
                    ThongTinGias = item.GroupBy(a => new {a.DonGia, a.DonGiaBaoHiem}).Select(a => new GhiNhanVatTuTieuHaoThuocGroupGiaGridVo
                    {
                        IsTinhPhi = a.First().TinhPhi,
                        DonGia = a.First().DonGia,
                        SoLuong = a.Sum(b => b.SoLuong),
                        DonGiaBaoHiem = a.First().DonGiaBaoHiem
                    }).ToList(),

                    //BVHD-3905
                    TooltipTiLeBHYT = item.First().TooltipTiLeBHYT
                })
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToList();

            return result;
        }

        public async Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHcAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            var yeuCauKhamBenh =
                await _yeuCauKhamBenhRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == yeuCauKhamBenhId);

            // get list dịch vụ cần ghi nhận VTTH/Thuốc theo yêu cầu khám hiện tại
            var lstDichVu = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Where(x => x.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId)
                .Select(item => new DichVuCanGhiNhanVTTHThuocVo
                {
                    Id = yeuCauKhamBenhId,
                    NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKhamBenh
                }).Union(_dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => x.YeuCauDichVuKyThuats.Any(y => y.YeuCauKhamBenhId == yeuCauKhamBenhId && y.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                    .SelectMany(x => x.YeuCauDichVuKyThuats.Where(y => y.YeuCauKhamBenhId == yeuCauKhamBenhId))
                    .Select(item => new DichVuCanGhiNhanVTTHThuocVo
                    {
                        Id = item.Id,
                        NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKyThuat
                    })).Distinct().ToListAsync();

            var lstYeuCauKhamBenhId = lstDichVu.Where(x => x.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.Id).ToList();
            var lstYeuDichVuKyThuatId = lstDichVu.Where(x => x.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.Id).ToList();

            var lstGhiNhanVTHH =
                await _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                            //&& (lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh && y.Id == x.YeuCauKhamBenhId)
                            //    || lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat && y.Id == x.YeuCauDichVuKyThuatId)))
                            && ((x.YeuCauKhamBenhId != null && lstYeuCauKhamBenhId.Contains(x.YeuCauKhamBenhId.Value))
                                || (x.YeuCauDichVuKyThuatId != null && lstYeuDichVuKyThuatId.Contains(x.YeuCauDichVuKyThuatId.Value))))
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)EnumNhomGoiDichVu.VatTuTieuHao,
                    TenNhomYeuCau = EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    MaDichVuYeuCau = item.Ma,
                    TenDichVuYeuCau = item.Ten,

                    //KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,

                    DonViTinh = item.DonViTinh,
                    TenDuongDung = "", //item.DuongDung,

                    DonGia = item.DonGiaBan,//item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,

                    PhieuLinh = item.YeuCauLinhVatTu != null ?
                        item.YeuCauLinhVatTu.SoPhieu : (item.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false) ? item.YeuCauLinhVatTuChiTiets.Where(a => a.YeuCauLinhVatTu.DuocDuyet != false).Select(a => a.YeuCauLinhVatTu.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoVatTuChiTiet.XuatKhoVatTu != null ? item.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn
                })
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToListAsync();

            return lstGhiNhanVTHH;
        }

        public async Task XuLyXoaYeuCauGhiNhanVTTHThuocAsync(YeuCauTiepNhan yeuCauTiepNhanChiTiet, string yeuCauGhiNhanId)
        {
            //var yeuCauGhiNhanObj = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(yeuCauGhiNhanId);
            var lstYeuCauCanXoaId = yeuCauGhiNhanId.Split(";");
            var yeuCauGhiNhanObjs = new List<DichVuGhiNhanVo>();
            foreach (var strId in lstYeuCauCanXoaId)
            {
                yeuCauGhiNhanObjs.Add(JsonConvert.DeserializeObject<DichVuGhiNhanVo>(strId));
            }

            foreach (var yeuCauGhiNhanObj in yeuCauGhiNhanObjs)
            {
                if (yeuCauGhiNhanObj.NhomId == (int)EnumNhomGoiDichVu.DuocPham)
                {
                    var yeuCauDuocPham = yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.FirstOrDefault(x => x.Id == yeuCauGhiNhanObj.Id);
                    if (yeuCauDuocPham == null)
                    {
                        throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.NotExists"));
                    }

                    //if (yeuCauDuocPham.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    //{
                    //    throw new Exception(_localizationService.GetResource("XoaGhiNhanVTTHThuoc.YeuCauDuocPham.DaThanhToan"));
                    //}

                    //cập nhật 24/05/2021
                    //if (yeuCauDuocPham.XuatKhoDuocPhamChiTiet?.XuatKhoDuocPhamId != null)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaXuat"));
                    //}

                    if (yeuCauDuocPham.YeuCauLinhDuocPhamId != null || yeuCauDuocPham.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false))
                    {
                        throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaLinh"));
                    }

                    if (yeuCauDuocPham.XuatKhoDuocPhamChiTiet != null) // trường hợp lĩnh bù
                    {
                        foreach (var thongTinXuat in yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
                        {
                            thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = Math.Round(thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - thongTinXuat.SoLuongXuat, 2);
                        }

                        var xuatKhoDpViTris = yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.ToList();
                        foreach (var item in xuatKhoDpViTris)
                        {
                            var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                            {
                                XuatKhoDuocPhamChiTietId = item.XuatKhoDuocPhamChiTietId,
                                NhapKhoDuocPhamChiTietId = item.NhapKhoDuocPhamChiTietId,
                                SoLuongXuat = -item.SoLuongXuat,
                                NgayXuat = DateTime.Now,
                                GhiChu = yeuCauDuocPham.TrangThai.GetDescription()
                            };
                            yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                        }
                    }

                    //yeuCauDuocPham.WillDelete = true;
                    //todo: cần kiểm tra lại trạng thái sau khi hủy

                    //cập nhật 24/05/2021
                    yeuCauDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
                }
                else if (yeuCauGhiNhanObj.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    var yeuCauVatTu = yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.FirstOrDefault(x => x.Id == yeuCauGhiNhanObj.Id);
                    if (yeuCauVatTu == null)
                    {
                        throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.NotExists"));
                    }

                    //if (yeuCauVatTu.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    //{
                    //    throw new Exception(_localizationService.GetResource("XoaGhiNhanVTTHThuoc.YeuCauVatTu.DaThanhToan"));
                    //}

                    //cập nhật 24/05/2021
                    //if (yeuCauVatTu.XuatKhoVatTuChiTiet?.XuatKhoVatTuId != null)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaXuat"));
                    //}

                    if (yeuCauVatTu.YeuCauLinhVatTuId != null || yeuCauVatTu.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false))
                    {
                        throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaLinh"));
                    }

                    if (yeuCauVatTu.XuatKhoVatTuChiTiet != null)  // trường hợp lĩnh bù
                    {
                        foreach (var thongTinXuat in yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris)
                        {
                            thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat = Math.Round(thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat - thongTinXuat.SoLuongXuat, 2);
                        }

                        var xuatKhoVtViTris = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.ToList();
                        foreach (var item in xuatKhoVtViTris)
                        {
                            var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                            {
                                XuatKhoVatTuChiTietId = item.XuatKhoVatTuChiTietId,
                                NhapKhoVatTuChiTietId = item.NhapKhoVatTuChiTietId,
                                SoLuongXuat = -item.SoLuongXuat,
                                NgayXuat = DateTime.Now,
                                GhiChu = yeuCauVatTu.TrangThai.GetDescription()
                            };

                            yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                        }
                    }

                    //yeuCauVatTu.WillDelete = true;

                    //cập nhật 24/05/2021
                    yeuCauVatTu.TrangThai = EnumYeuCauVatTuBenhVien.DaHuy;
                }
                else
                {
                    throw new Exception(_localizationService.GetResource("XoaGhiNhanVTTHThuoc.NotExists"));
                }
            }
        }

        public async Task CapNhatGridItemGhiNhanVTTHThuocAsync(YeuCauTiepNhan yeuCauTiepNhanChiTiet, ChiDinhGhiNhanVatTuThuocTieuHaoVo ghiNhanVo)
        {
            string templateKeyId = "\"Id\": {0}, \"NhomId\": {1}";
            //var yeuCauGhiNhanObj = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(ghiNhanVo.YeuCauGhiNhanVTTHThuocId);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            var lstYeuCauCanXoaId = ghiNhanVo.YeuCauGhiNhanVTTHThuocId.Split(";");
            var yeuCauGhiNhanObjs = new List<DichVuGhiNhanVo>();
            foreach (var strId in lstYeuCauCanXoaId)
            {
                yeuCauGhiNhanObjs.Add(JsonConvert.DeserializeObject<DichVuGhiNhanVo>(strId));
            }

            foreach (var yeuCauGhiNhanObj in yeuCauGhiNhanObjs)
            {
                if (yeuCauGhiNhanObj.NhomId == (int) EnumNhomGoiDichVu.DuocPham)
                {
                    var yeuCauDuocPham =
                        yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.FirstOrDefault(x => x.Id == yeuCauGhiNhanObj.Id);
                    if (yeuCauDuocPham == null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.NotExists"));
                    }

                    //if (yeuCauDuocPham.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaThanhToan"));
                    //}

                    // bỏ bắt trường hợp đã xuất, vì lĩnh bù thêm là xuất luôn
                    //if (yeuCauDuocPham.XuatKhoDuocPhamChiTiet?.XuatKhoDuocPhamId != null)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaXuat"));
                    //}

                    if (yeuCauDuocPham.YeuCauLinhDuocPhamId != null ||
                        yeuCauDuocPham.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false))
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauDuocPham.DaLinh"));
                    }

                    // trường hợp cập nhật số lượng
                    if (ghiNhanVo.IsCapNhatSoLuong)
                    {
                        if (yeuCauDuocPham.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                        {
                            // get thông tin nhập kho chi tiết
                            var lstNhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauDuocPham.KhoLinhId && x.HanSuDung >= DateTime.Now
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                .OrderBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                .ThenBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .ToListAsync();


                            var chiTietXuat =
                                yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.OrderByDescending(
                                        x =>
                                            cauHinhChung.UuTienXuatKhoTheoHanSuDung
                                                ? x.NhapKhoDuocPhamChiTiet.HanSuDung
                                                : x.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien)
                                    .ThenByDescending(x =>
                                        cauHinhChung.UuTienXuatKhoTheoHanSuDung
                                            ? x.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien
                                            : x.NhapKhoDuocPhamChiTiet.HanSuDung);
                            var soLuongHienTai = chiTietXuat.Sum(x => x.SoLuongXuat);

                            // trường hợp tăng số lượng
                            if (soLuongHienTai < ghiNhanVo.SoLuongCapNhat)
                            {
                                var soLuongTang = ghiNhanVo.SoLuongCapNhat.Value - soLuongHienTai;
                                foreach (var thongTinXuat in chiTietXuat)
                                {
                                    if (soLuongTang > 0)
                                    {
                                        var soLuongTon = thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongNhap -
                                                         thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                                        if (soLuongTon > 0)
                                        {
                                            if (soLuongTon <= soLuongTang)
                                            {
                                                thongTinXuat.SoLuongXuat +=
                                                    (thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongNhap -
                                                     thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat);
                                                thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat =
                                                    thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongNhap;

                                                var nhapChiTiet = lstNhapKhoDuocPhamChiTiet.First(x =>
                                                    x.Id == thongTinXuat.NhapKhoDuocPhamChiTietId);
                                                nhapChiTiet.SoLuongDaXuat =
                                                    thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat;

                                                soLuongTang -= soLuongTon;
                                            }
                                            else
                                            {
                                                thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongTang;
                                                thongTinXuat.SoLuongXuat += soLuongTang;

                                                var nhapChiTiet = lstNhapKhoDuocPhamChiTiet.First(x =>
                                                    x.Id == thongTinXuat.NhapKhoDuocPhamChiTietId);
                                                nhapChiTiet.SoLuongDaXuat += soLuongTang;

                                                soLuongTang = 0;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                yeuCauDuocPham.SoLuong =
                                    yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris
                                        .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);

                                // nếu vẫn còn dư số lượng, xử lý thêm mới YeuCauDuocPham
                                // kiếm tra nếu tồn dược phẩm ko đủ, thông báo
                                if (soLuongTang > 0)
                                {
                                    if ((lstNhapKhoDuocPhamChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) ||
                                        (lstNhapKhoDuocPhamChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                                         soLuongTang))
                                    {
                                        throw new Exception(
                                            _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                                    }

                                    var yeuCauNew = new YeuCauDuocPhamBenhVien();
                                    var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                                    {
                                        DuocPhamBenhVienId = yeuCauDuocPham.DuocPhamBenhVienId
                                    };

                                    bool flagTaoMoi = false;
                                    bool flagChangeTonNhapChiTiet = false;
                                    foreach (var item in lstNhapKhoDuocPhamChiTiet)
                                    {
                                        if (soLuongTang > 0)
                                        {
                                            if (yeuCauDuocPham.DonGiaNhap == item.DonGiaNhap &&
                                                yeuCauDuocPham.VAT == item.VAT &&
                                                yeuCauDuocPham.TiLeTheoThapGia == item.TiLeTheoThapGia)
                                            {
                                                flagChangeTonNhapChiTiet = true;
                                                var newXuatViTri = new XuatKhoDuocPhamChiTietViTri()
                                                {
                                                    NhapKhoDuocPhamChiTietId = item.Id
                                                };

                                                var slTon = item.SoLuongNhap - item.SoLuongDaXuat;
                                                if (slTon >= soLuongTang)
                                                {
                                                    newXuatViTri.SoLuongXuat = soLuongTang;
                                                    item.SoLuongDaXuat += soLuongTang;
                                                    soLuongTang = 0;
                                                }
                                                else
                                                {
                                                    newXuatViTri.SoLuongXuat = item.SoLuongNhap - item.SoLuongDaXuat;
                                                    soLuongTang -= slTon;
                                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                                }

                                                yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(
                                                    newXuatViTri);
                                                yeuCauDuocPham.SoLuong = yeuCauDuocPham.XuatKhoDuocPhamChiTiet
                                                    .XuatKhoDuocPhamChiTietViTris.Where(p => p.WillDelete != true)
                                                    .Sum(x => x.SoLuongXuat);
                                            }
                                            else
                                            {
                                                // gọi function tạo mới yêu cầu dược phẩm
                                                flagTaoMoi = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }

                                        yeuCauDuocPham.SoLuong = yeuCauDuocPham.XuatKhoDuocPhamChiTiet
                                            .XuatKhoDuocPhamChiTietViTris.Where(p => p.WillDelete != true)
                                            .Sum(x => x.SoLuongXuat);
                                    }

                                    if (soLuongTang > 0 && flagTaoMoi)
                                    {
                                        ghiNhanVo.SoLuong = soLuongTang;
                                        ghiNhanVo.LaDuocPhamBHYT = yeuCauDuocPham.LaDuocPhamBHYT;
                                        ghiNhanVo.KhoId = yeuCauDuocPham.KhoLinhId;
                                        ghiNhanVo.NhapKhoDuocPhamChiTiets = lstNhapKhoDuocPhamChiTiet;
                                        ghiNhanVo.TinhPhi = yeuCauDuocPham.KhongTinhPhi == null
                                            ? true
                                            : !yeuCauDuocPham.KhongTinhPhi;

                                        long dichVuId = 0;
                                        string loaiDichVu = "";
                                        int loaiDichVuId = 0;
                                        if (yeuCauDuocPham.YeuCauKhamBenhId != null)
                                        {
                                            dichVuId = yeuCauDuocPham.YeuCauKhamBenhId.Value;
                                            loaiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                                            loaiDichVuId = (int) EnumNhomGoiDichVu.DichVuKhamBenh;
                                        }
                                        else if (yeuCauDuocPham.YeuCauDichVuKyThuatId != null)
                                        {
                                            dichVuId = yeuCauDuocPham.YeuCauDichVuKyThuatId.Value;
                                            loaiDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                            loaiDichVuId = (int) EnumNhomGoiDichVu.DichVuKyThuat;
                                        }

                                        ghiNhanVo.DichVuChiDinhId =
                                            "{" + string.Format(templateKeyId, dichVuId, loaiDichVuId) +
                                            "}"; // dịch chỉ định trong khám bệnh
                                        ghiNhanVo.DichVuGhiNhanId = "{" + string.Format(templateKeyId,
                                                                        yeuCauDuocPham.DuocPhamBenhVienId,
                                                                        (int) EnumNhomGoiDichVu.DuocPham) +
                                                                    "}"; // dịch vụ dược phẩm/vật tư

                                        await XuLyThemGhiNhanVatTuBenhVienAsync(ghiNhanVo, yeuCauTiepNhanChiTiet);
                                    }
                                    else
                                    {
                                        if (flagChangeTonNhapChiTiet)
                                        {
                                            ghiNhanVo.NhapKhoDuocPhamChiTiets = lstNhapKhoDuocPhamChiTiet;
                                        }
                                    }
                                }
                                else
                                {
                                    yeuCauDuocPham.SoLuong =
                                        yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris
                                            .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                                }
                            }
                            // trường hợp giảm số lượng
                            else if (soLuongHienTai > ghiNhanVo.SoLuongCapNhat)
                            {
                                var soLuongGiam = soLuongHienTai - ghiNhanVo.SoLuongCapNhat.Value;
                                foreach (var thongTinXuat in chiTietXuat)
                                {
                                    if (soLuongGiam > 0)
                                    {
                                        if (thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat > soLuongGiam)
                                        {
                                            thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= soLuongGiam;
                                            thongTinXuat.SoLuongXuat -= soLuongGiam;
                                            soLuongGiam = 0;
                                        }
                                        else
                                        {
                                            soLuongGiam -= thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                                            thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = 0;
                                            thongTinXuat.WillDelete = true;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                yeuCauDuocPham.SoLuong =
                                    yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris
                                        .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                            }
                        }
                        else // lĩnh trực tiếp
                        {
                            // get thông tin nhập kho chi tiết
                            var lstNhapKhoDuocPhamChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauDuocPham.KhoLinhId
                                            && x.DuocPhamBenhVienId == yeuCauDuocPham.DuocPhamBenhVienId
                                            && x.HanSuDung >= DateTime.Now
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.LaDuocPhamBHYT == yeuCauDuocPham.LaDuocPhamBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap)
                                .OrderBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                .ThenBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .ToListAsync();

                            if (lstNhapKhoDuocPhamChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                                ghiNhanVo.SoLuongCapNhat)
                            {
                                throw new Exception(
                                    _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                            }

                            yeuCauDuocPham.SoLuong = ghiNhanVo.SoLuongCapNhat ?? 0;
                        }
                    }

                    // trường hợp cập nhật tính phí
                    if (ghiNhanVo.IsCapNhatTinhPhi)
                    {
                        yeuCauDuocPham.KhongTinhPhi = !ghiNhanVo.TinhPhi;
                    }

                }
                else if (yeuCauGhiNhanObj.NhomId == (int) EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    var yeuCauVatTu =
                        yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.FirstOrDefault(x => x.Id == yeuCauGhiNhanObj.Id);
                    if (yeuCauVatTu == null)
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.NotExists"));
                    }

                    //if (yeuCauVatTu.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaThanhToan"));
                    //}

                    // bỏ bắt trường hợp đã xuất, vì lĩnh bù thêm là xuất luôn
                    //if (yeuCauVatTu.XuatKhoVatTuChiTiet?.XuatKhoVatTuId != null)
                    //{
                    //    throw new Exception(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaXuat"));
                    //}

                    if (yeuCauVatTu.YeuCauLinhVatTuId != null ||
                        yeuCauVatTu.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false))
                    {
                        throw new Exception(
                            _localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.YeuCauVatTu.DaLinh"));
                    }

                    // trường hợp cập nhật số lượng
                    if (ghiNhanVo.IsCapNhatSoLuong)
                    {
                        if (yeuCauVatTu.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu)
                        {
                            //get thông tin nhập chi tiết
                            var lstNhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauVatTu.KhoLinhId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.SoLuongNhap > x.SoLuongDaXuat)
                                .OrderBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                .ThenBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .ToListAsync();

                            var chiTietXuat = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                .OrderByDescending(x =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung
                                        ? x.NhapKhoVatTuChiTiet.HanSuDung
                                        : x.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien).ThenByDescending(x =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung
                                        ? x.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien
                                        : x.NhapKhoVatTuChiTiet.HanSuDung);
                            var soLuongHienTai = chiTietXuat.Sum(x => x.SoLuongXuat);

                            // trường hợp tăng số lượng
                            if (soLuongHienTai < ghiNhanVo.SoLuongCapNhat)
                            {
                                var soLuongTang = ghiNhanVo.SoLuongCapNhat.Value - soLuongHienTai;
                                foreach (var thongTinXuat in chiTietXuat)
                                {
                                    if (soLuongTang > 0)
                                    {
                                        var soLuongTon = thongTinXuat.NhapKhoVatTuChiTiet.SoLuongNhap -
                                                         thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat;
                                        if (soLuongTon > 0)
                                        {
                                            if (soLuongTon <= soLuongTang)
                                            {
                                                thongTinXuat.SoLuongXuat +=
                                                    (thongTinXuat.NhapKhoVatTuChiTiet.SoLuongNhap -
                                                     thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat);
                                                thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat =
                                                    thongTinXuat.NhapKhoVatTuChiTiet.SoLuongNhap;

                                                var nhapChiTiet = lstNhapKhoVatTuChiTiet.First(x =>
                                                    x.Id == thongTinXuat.NhapKhoVatTuChiTietId);
                                                nhapChiTiet.SoLuongDaXuat =
                                                    thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat;

                                                soLuongTang -= soLuongTon;
                                            }
                                            else
                                            {
                                                thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongTang;
                                                thongTinXuat.SoLuongXuat += soLuongTang;

                                                var nhapChiTiet = lstNhapKhoVatTuChiTiet.First(x =>
                                                    x.Id == thongTinXuat.NhapKhoVatTuChiTietId);
                                                nhapChiTiet.SoLuongDaXuat += soLuongTang;

                                                soLuongTang = 0;
                                            }

                                            yeuCauVatTu.SoLuong = yeuCauVatTu.XuatKhoVatTuChiTiet
                                                .XuatKhoVatTuChiTietViTris
                                                .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                // nếu vẫn còn dư số lượng, xử lý thêm mới YeuCauDuocPham
                                // kiếm tra nếu tồn dược phẩm ko đủ, thông báo
                                if (soLuongTang > 0)
                                {
                                    if ((lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < 0) ||
                                        (lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                                         soLuongTang))
                                    {
                                        throw new Exception(
                                            _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                                    }

                                    bool flagTaoMoi = false;
                                    bool flagChangeTonNhapChiTiet = false;
                                    foreach (var item in lstNhapKhoVatTuChiTiet)
                                    {
                                        if (soLuongTang > 0)
                                        {
                                            if (yeuCauVatTu.DonGiaNhap == item.DonGiaNhap &&
                                                yeuCauVatTu.VAT == item.VAT &&
                                                yeuCauVatTu.TiLeTheoThapGia == item.TiLeTheoThapGia)
                                            {
                                                flagChangeTonNhapChiTiet = true;
                                                var newXuatViTri = new XuatKhoVatTuChiTietViTri()
                                                {
                                                    NhapKhoVatTuChiTietId = item.Id
                                                };

                                                var slTon = item.SoLuongNhap - item.SoLuongDaXuat;
                                                if (slTon >= soLuongTang)
                                                {
                                                    newXuatViTri.SoLuongXuat = soLuongTang;
                                                    item.SoLuongDaXuat += soLuongTang;
                                                    soLuongTang = 0;
                                                }
                                                else
                                                {
                                                    newXuatViTri.SoLuongXuat = item.SoLuongNhap - item.SoLuongDaXuat;
                                                    soLuongTang -= slTon;
                                                    item.SoLuongDaXuat = item.SoLuongNhap;
                                                }

                                                yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(
                                                    newXuatViTri);
                                                yeuCauVatTu.SoLuong = yeuCauVatTu.XuatKhoVatTuChiTiet
                                                    .XuatKhoVatTuChiTietViTris
                                                    .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                                            }
                                            else
                                            {
                                                // gọi function tạo mới yêu cầu vật tư
                                                flagTaoMoi = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }

                                        yeuCauVatTu.SoLuong = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                            .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                                    }

                                    if (soLuongTang > 0 && flagTaoMoi)
                                    {
                                        ghiNhanVo.SoLuong = soLuongTang;
                                        ghiNhanVo.LaDuocPhamBHYT = yeuCauVatTu.LaVatTuBHYT;
                                        ghiNhanVo.KhoId = yeuCauVatTu.KhoLinhId;
                                        ghiNhanVo.NhapKhoVatTuChiTiets = lstNhapKhoVatTuChiTiet;
                                        ghiNhanVo.TinhPhi = yeuCauVatTu.KhongTinhPhi == null
                                            ? true
                                            : !yeuCauVatTu.KhongTinhPhi;

                                        long dichVuId = 0;
                                        string loaiDichVu = "";
                                        int loaiDichVuId = 0;
                                        if (yeuCauVatTu.YeuCauKhamBenhId != null)
                                        {
                                            dichVuId = yeuCauVatTu.YeuCauKhamBenhId.Value;
                                            loaiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                                            loaiDichVuId = (int) EnumNhomGoiDichVu.DichVuKhamBenh;
                                        }
                                        else if (yeuCauVatTu.YeuCauDichVuKyThuatId != null)
                                        {
                                            dichVuId = yeuCauVatTu.YeuCauDichVuKyThuatId.Value;
                                            loaiDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                            loaiDichVuId = (int) EnumNhomGoiDichVu.DichVuKyThuat;
                                        }

                                        ghiNhanVo.DichVuChiDinhId =
                                            "{" + string.Format(templateKeyId, dichVuId, loaiDichVuId) +
                                            "}"; // dịch chỉ định trong khám bệnh
                                        ghiNhanVo.DichVuGhiNhanId = "{" + string.Format(templateKeyId,
                                                                        yeuCauVatTu.VatTuBenhVienId,
                                                                        (int) EnumNhomGoiDichVu.VatTuTieuHao) +
                                                                    "}"; // dịch vụ dược phẩm/vật tư

                                        await XuLyThemGhiNhanVatTuBenhVienAsync(ghiNhanVo, yeuCauTiepNhanChiTiet);
                                    }
                                    else
                                    {
                                        if (flagChangeTonNhapChiTiet)
                                        {
                                            ghiNhanVo.NhapKhoVatTuChiTiets = lstNhapKhoVatTuChiTiet;
                                        }
                                    }
                                }
                                else
                                {
                                    yeuCauVatTu.SoLuong =
                                        yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                            .Where(p => p.WillDelete != true).Sum(x => x.SoLuongXuat);
                                }
                            }
                            // trường hợp giảm số lượng
                            else if (soLuongHienTai > ghiNhanVo.SoLuongCapNhat)
                            {
                                var soLuongGiam = soLuongHienTai - ghiNhanVo.SoLuongCapNhat.Value;
                                foreach (var thongTinXuat in chiTietXuat)
                                {
                                    if (soLuongGiam > 0)
                                    {
                                        if (thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat > soLuongGiam)
                                        {
                                            thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat -= soLuongGiam;
                                            thongTinXuat.SoLuongXuat -= soLuongGiam;
                                            soLuongGiam = 0;
                                        }
                                        else
                                        {
                                            soLuongGiam -= thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat;
                                            thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat = 0;
                                            thongTinXuat.WillDelete = true;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                yeuCauVatTu.SoLuong =
                                    yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                                        .Where(p => p.WillDelete != true)
                                        .Sum(x => x.SoLuongXuat);
                            }
                        }
                        else
                        {
                            //get thông tin nhập chi tiết
                            var lstNhapKhoVatTuChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauVatTu.KhoLinhId
                                            && x.VatTuBenhVienId == yeuCauVatTu.VatTuBenhVienId
                                            && x.NhapKhoVatTu.DaHet != true
                                            && x.LaVatTuBHYT == yeuCauVatTu.LaVatTuBHYT
                                            && x.SoLuongNhap > x.SoLuongDaXuat)
                                .OrderBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                .ThenBy(p =>
                                    cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                .ToListAsync();

                            if (lstNhapKhoVatTuChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                                ghiNhanVo.SoLuongCapNhat)
                            {
                                throw new Exception(
                                    _localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
                            }

                            yeuCauVatTu.SoLuong = ghiNhanVo.SoLuongCapNhat ?? 0;
                        }
                    }

                    // trường hợp cập nhật tính phí
                    if (ghiNhanVo.IsCapNhatTinhPhi)
                    {
                        yeuCauVatTu.KhongTinhPhi = !ghiNhanVo.TinhPhi;
                    }

                }
                else
                {
                    throw new Exception(_localizationService.GetResource("XoaGhiNhanVTTHThuoc.NotExists"));
                }
            }
        }

        public async Task CapNhatSoLuongTonKhiGhiNhanVTTHThuocAsync(List<NhapKhoDuocPhamChiTiet> lstNhapKhoDuocPhamChiTiet, List<NhapKhoVatTuChiTiet> lstNhapKhoVatTuChiTiet)
        {
            // todo: có cập nhật bỏ await
            if (lstNhapKhoDuocPhamChiTiet.Any())
            {
                var ids = lstNhapKhoDuocPhamChiTiet.Select(o => o.Id).ToList();
                var lstNhap = _nhapKhoDuocPhamChiTietRepository.Table
                    .Where(x => ids.Contains(x.Id)).ToList();
                if (lstNhap.Any())
                {
                    foreach (var item in lstNhapKhoDuocPhamChiTiet)
                    {
                        foreach (var nhapChiTiet in lstNhap)
                        {
                            if (nhapChiTiet.Id == item.Id)
                            {
                                nhapChiTiet.SoLuongDaXuat = item.SoLuongDaXuat;
                                break;
                            }
                        }
                    }
                }
                await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(lstNhap);
            }

            if (lstNhapKhoVatTuChiTiet.Any())
            {
                var ids = lstNhapKhoVatTuChiTiet.Select(o => o.Id).ToList();
                var lstNhap = _nhapKhoVatTuChiTietRepository.Table
                    .Where(x => ids.Contains(x.Id)).ToList();
                if (lstNhap.Any())
                {
                    foreach (var item in lstNhapKhoVatTuChiTiet)
                    {
                        foreach (var nhapChiTiet in lstNhap)
                        {
                            if (nhapChiTiet.Id == item.Id)
                            {
                                nhapChiTiet.SoLuongDaXuat = item.SoLuongDaXuat;
                                break;
                            }
                        }
                    }
                }
                await _nhapKhoVatTuChiTietRepository.UpdateAsync(lstNhap);
            }
        }

        public async Task XuLyXuatYeuCauGhiNhanVTTHThuocAsync(ChiDinhGhiNhanVatTuThuocTieuHaoVo yeuCauVo)
        {
            var yeuCauKhamBenh =
               await _yeuCauKhamBenhRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == yeuCauVo.YeuCauKhamBenhId);
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            bool coPhieuXuat = false;

            // get list dịch vụ cần ghi nhận VTTH/Thuốc theo yêu cầu khám hiện tại
            //var lstDichVu = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .Where(x => x.Id == yeuCauKhamBenh.DichVuKhamBenhBenhVienId)
            //    .Select(item => new DichVuCanGhiNhanVTTHThuocVo
            //    {
            //        Id = yeuCauVo.YeuCauKhamBenhId,
            //        NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKhamBenh
            //    }).Union(_dichVuKyThuatBenhVienRepository.TableNoTracking
            //        .Where(x => x.YeuCauDichVuKyThuats.Any(y => y.YeuCauKhamBenhId == yeuCauVo.YeuCauKhamBenhId && y.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            //        .SelectMany(x => x.YeuCauDichVuKyThuats.Where(y => y.YeuCauKhamBenhId == yeuCauVo.YeuCauKhamBenhId))
            //        .Select(item => new DichVuCanGhiNhanVTTHThuocVo
            //        {
            //            Id = item.Id,
            //            NhomDichVu = (int)EnumNhomGoiDichVu.DichVuKyThuat
            //        })).Distinct().ToListAsync();

            var lstDichVuKham = new List<long>();
            lstDichVuKham.Add(yeuCauVo.YeuCauKhamBenhId);

            //var lstDichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking
            //    .Where(x => x.YeuCauDichVuKyThuats.Any(y => y.YeuCauKhamBenhId == yeuCauVo.YeuCauKhamBenhId && y.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            //    .SelectMany(x => x.YeuCauDichVuKyThuats.Where(y => y.YeuCauKhamBenhId == yeuCauVo.YeuCauKhamBenhId))
            //    .Select(item => item.Id)
            //    .Distinct().ToListAsync();

            var lstDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauKhamBenhId == yeuCauVo.YeuCauKhamBenhId)
                .Select(item => item.Id).Distinct().ToList();

            var lstAllGhiNhanDuocPham = await _yeuCauDuocPhamBenhVienRepository.Table
                .Include(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(y => y.XuatKhoDuocPham)
                .Include(x => x.YeuCauTiepNhan)
                .Where(x => x.KhoLinh != null
                            && x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                            && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                            //&& x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null
                            && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            //&& (lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh && y.Id == x.YeuCauKhamBenhId)
                            //    || lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat && y.Id == x.YeuCauDichVuKyThuatId)))
                            && ((x.YeuCauKhamBenhId != null && lstDichVuKham.Contains(x.YeuCauKhamBenhId.Value))
                                || (x.YeuCauDichVuKyThuatId != null && lstDichVuKyThuat.Contains(x.YeuCauDichVuKyThuatId.Value))))
                .ToListAsync();

            var lstPhieuXuatDaXuat = lstAllGhiNhanDuocPham.Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null)
                .Select(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham)
                .Distinct()
                .ToList();

            var lstGhiNhanDuocPhamChuaXuat = lstAllGhiNhanDuocPham.Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null).ToList();
            if (lstGhiNhanDuocPhamChuaXuat.Any())
            {
                var lstPhieuXuatDuocPham = new List<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham>();
                var lstKhoId = lstGhiNhanDuocPhamChuaXuat.Where(x => x.KhoLinhId != null).Select(x => x.KhoLinhId.Value).Distinct().ToList();

                var phieuXuatTemp = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham()
                {
                    //KhoXuatId = khoId,
                    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                    LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                    NguoiXuatId = currentUserId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now
                };

                foreach (var khoId in lstKhoId)
                {
                    var phieuXuatNew = phieuXuatTemp.Clone();
                    phieuXuatNew.KhoXuatId = khoId;

                    var lstYeuCauTheoKho = lstGhiNhanDuocPhamChuaXuat.Where(x => x.KhoLinhId == khoId && x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null).ToList();
                    if (lstYeuCauTheoKho.Any())
                    {
                        coPhieuXuat = true;
                        foreach (var yeuCau in lstYeuCauTheoKho)
                        {
                            // lấy thông tin tên người bệnh
                            var tenBenhNhan = yeuCau.YeuCauTiepNhan.HoTen;
                            //if (yeuCau.YeuCauKhamBenh != null)
                            //{
                            //    tenBenhNhan = yeuCau.YeuCauKhamBenh.YeuCauTiepNhan.HoTen;
                            //}
                            //else if (yeuCau.YeuCauDichVuKyThuat != null)
                            //{
                            //    tenBenhNhan = yeuCau.YeuCauDichVuKyThuat.YeuCauTiepNhan.HoTen;
                            //}

                            // xử lý kiểm tra gộp phiếu xuất
                            var phieuXuatDaXuat = lstPhieuXuatDaXuat.FirstOrDefault(x =>
                                x.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatChoBenhNhan
                                && x.TenNguoiNhan != null
                                && x.TenNguoiNhan.Trim().ToLower() == tenBenhNhan.Trim().ToLower()
                                && x.NguoiXuatId == currentUserId
                                && x.KhoXuatId == khoId);
                            if (phieuXuatDaXuat != null)
                            {
                                phieuXuatNew = phieuXuatDaXuat;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                                {
                                    phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                                }
                                else
                                {
                                    if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                                    {
                                        phieuXuatNew = phieuXuatTemp.Clone();
                                        phieuXuatNew.KhoXuatId = khoId;
                                    }
                                }
                            }


                            yeuCau.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                            yeuCau.TrangThai = EnumYeuCauDuocPhamBenhVien.DaThucHien;
                            yeuCau.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = phieuXuatNew;
                            //phieuXuatNew.XuatKhoDuocPhamChiTiets.Add(yeuCau.XuatKhoDuocPhamChiTiet);
                        }

                        //lstPhieuXuatDuocPham.Add(phieuXuatNew);
                    }
                }

                if (coPhieuXuat)
                {
                    await _yeuCauDuocPhamBenhVienRepository.UpdateAsync(lstAllGhiNhanDuocPham);
                }
            }

            var lstAllGhiNhanVatTu = await _yeuCauVatTuBenhVienRepository.Table
                .Include(x => x.XuatKhoVatTuChiTiet).ThenInclude(y => y.XuatKhoVatTu)
                .Include(x => x.YeuCauTiepNhan)
                .Where(x => x.KhoLinh != null
                            && x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                            && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                            //&& x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null
                            && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhBu
                            //&& (lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh && y.Id == x.YeuCauKhamBenhId)
                            //    || lstDichVu.Any(y => y.NhomDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat && y.Id == x.YeuCauDichVuKyThuatId)))
                            && ((x.YeuCauKhamBenhId != null && lstDichVuKham.Contains(x.YeuCauKhamBenhId.Value))
                                || (x.YeuCauDichVuKyThuatId != null && lstDichVuKyThuat.Contains(x.YeuCauDichVuKyThuatId.Value))))
                .ToListAsync();

            var lstPhieuXuatVatTuDaXuat = lstAllGhiNhanVatTu.Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null)
                .Select(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTu)
                .Distinct()
                .ToList();

            var lstGhiNhanVatTuChuaXuat = lstAllGhiNhanVatTu.Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null).ToList();
            if (lstGhiNhanVatTuChuaXuat.Any())
            {
                var lstPhieuXuatVatTu = new List<XuatKhoVatTu>();
                var lstKhoId = lstGhiNhanVatTuChuaXuat.Where(x => x.KhoLinhId != null).Select(x => x.KhoLinhId.Value).Distinct().ToList();


                var phieuXuatTemp = new XuatKhoVatTu()
                {
                    //KhoXuatId = khoId,
                    LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                    LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                    NguoiXuatId = currentUserId,
                    LoaiNguoiNhan = LoaiNguoiGiaoNhan.NgoaiHeThong,
                    NgayXuat = DateTime.Now
                };

                foreach (var khoId in lstKhoId)
                {
                    var phieuXuatNew = phieuXuatTemp.Clone();
                    phieuXuatNew.KhoXuatId = khoId;

                    var lstYeuCauTheoKho = lstGhiNhanVatTuChuaXuat.Where(x => x.KhoLinhId == khoId && x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null).ToList();
                    if (lstYeuCauTheoKho.Any())
                    {
                        coPhieuXuat = true;
                        foreach (var yeuCau in lstYeuCauTheoKho)
                        {
                            // lấy thông tin tên người bệnh
                            var tenBenhNhan = yeuCau.YeuCauTiepNhan.HoTen;
                            //if (yeuCau.YeuCauKhamBenh != null)
                            //{
                            //    tenBenhNhan = yeuCau.YeuCauKhamBenh.YeuCauTiepNhan.HoTen;
                            //}
                            //else if (yeuCau.YeuCauDichVuKyThuat != null)
                            //{
                            //    tenBenhNhan = yeuCau.YeuCauDichVuKyThuat.YeuCauTiepNhan.HoTen;
                            //}
                            var phieuXuatDaXuat = lstPhieuXuatVatTuDaXuat.FirstOrDefault(x =>
                                x.LoaiXuatKho == EnumLoaiXuatKho.XuatChoBenhNhan
                                && x.TenNguoiNhan != null
                                && x.TenNguoiNhan.Trim().ToLower() == tenBenhNhan.Trim().ToLower()
                                && x.NguoiXuatId == currentUserId
                                && x.KhoXuatId == khoId);
                            if (phieuXuatDaXuat != null)
                            {
                                phieuXuatNew = phieuXuatDaXuat;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                                {
                                    phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                                }
                                else
                                {
                                    if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                                    {
                                        phieuXuatNew = phieuXuatTemp.Clone();
                                        phieuXuatNew.KhoXuatId = khoId;
                                    }
                                }
                            }

                            yeuCau.XuatKhoVatTuChiTiet.NgayXuat = DateTime.Now;
                            yeuCau.TrangThai = EnumYeuCauVatTuBenhVien.DaThucHien;
                            yeuCau.XuatKhoVatTuChiTiet.XuatKhoVatTu = phieuXuatNew;
                            //phieuXuatNew.XuatKhoVatTuChiTiets.Add(yeuCau.XuatKhoVatTuChiTiet);
                        }

                        //lstPhieuXuatVatTu.Add(phieuXuatNew);
                    }
                }

                if (coPhieuXuat)
                {
                    await _yeuCauVatTuBenhVienRepository.UpdateAsync(lstAllGhiNhanVatTu);
                }
            }

            if (!coPhieuXuat)
            {
                throw new Exception(_localizationService.GetResource("XuatGhiNhatVTTHThuoc.DanhSachYeuCauChuaXuat.KhongCo"));
            }
        }

        public async Task<int> GetSoLuongConLaiDichVuKyThuatTrongGoiMarketingBenhNhanAsync(long yeuCauGoiDichVuId, long dichVuKyThuatBenhVienId)
        {
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauDichVuKyThuats)
                .Where(x => x.Id == yeuCauGoiDichVuId)
                .FirstOrDefault();
            if (yeuCauGoiDichVu == null)
            {
                throw new Exception(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.NotExists"));
            }

            var dichVuKyThuatTrongGoi = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(x => x.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId);
            if (dichVuKyThuatTrongGoi == null)
            {
                throw new Exception(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.NotExists"));
            }
            var soLanYeuCau = yeuCauGoiDichVu.YeuCauDichVuKyThuats
                .Where(o => o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId)
                .Sum(x => x.SoLan);
            return dichVuKyThuatTrongGoi.SoLan - soLanYeuCau;

            //return dichVuKyThuatTrongGoi.SoLan - dichVuKyThuatTrongGoi.DichVuKyThuatBenhVien.YeuCauDichVuKyThuats
            //    .Where(x => x.YeuCauGoiDichVuId == yeuCauGoiDichVuId
            //                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            //    .Sum(x => x.SoLan);
        }

        public async Task<int> GetSoLuongConLaiDichVuKyThuatKhuyenMaiTrongGoiMarketingBenhNhanAsync(long yeuCauGoiDichVuId, long dichVuKyThuatBenhVienId)
        {
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .Include(a => a.MienGiamChiPhiKhuyenMais).ThenInclude(y => y.YeuCauDichVuKyThuat)
                .Include(a => a.MienGiamChiPhiKhuyenMais).ThenInclude(y => y.TaiKhoanBenhNhanThu)
                .Where(x => x.Id == yeuCauGoiDichVuId)
                .FirstOrDefault();
            if (yeuCauGoiDichVu == null)
            {
                throw new Exception(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.NotExists"));
            }

            var dichVuKyThuatTrongGoi = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.FirstOrDefault(x => x.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId);
            if (dichVuKyThuatTrongGoi == null)
            {
                throw new Exception(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.NotExists"));
            }

            var soLanMienGiam = yeuCauGoiDichVu.MienGiamChiPhiKhuyenMais
                .Where(o => o.DaHuy != true && o.YeuCauDichVuKyThuat != null && o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId
                && (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true))
                .Sum(x => x.YeuCauDichVuKyThuat.SoLan);

            return dichVuKyThuatTrongGoi.SoLan - soLanMienGiam;
        }

        #endregion

        #region Nhóm dịch vụ thường dùng

        #region grid
        public async Task<GridDataSource> GetNhomDichVuThuongDungDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var boPhanId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? Int32.Parse(queryInfo.AdditionalSearchString) : (int?)null;
            var query = _goiDichVuRepository.TableNoTracking
                .Where(x => x.LoaiGoiDichVu == EnumLoaiGoiDichVu.TrongPhongBacSy
                            && x.IsDisabled != true && (x.BoPhanId == null || x.BoPhanId == (BoPhan)boPhanId)) 
                .Select(item => new NhomGoiDichVuThuongDungGridVo()
                {
                    Id = item.Id,
                    TenNhom = item.Ten,
                    MoTa = item.MoTa
                })
                .ApplyLike(queryInfo.SearchTerms, x => x.TenNhom, x => x.MoTa);

            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString)
                //.Skip(queryInfo.Skip)
                //.Take(queryInfo.Take)
                .ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetNhomDichVuThuongDungTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var boPhanId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? Int32.Parse(queryInfo.AdditionalSearchString) : (int?)null;
            var query = _goiDichVuRepository.TableNoTracking
                .Where(x => x.LoaiGoiDichVu == EnumLoaiGoiDichVu.TrongPhongBacSy
                             && x.IsDisabled != true && (x.BoPhanId == null || x.BoPhanId == (BoPhan)boPhanId))
                .Select(item => new NhomGoiDichVuThuongDungGridVo()
                {
                    Id = item.Id,
                    TenNhom = item.Ten,
                    MoTa = item.MoTa
                })
                .ApplyLike(queryInfo.SearchTerms, x => x.TenNhom, x => x.MoTa);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetChiTietDichVuThuongDungTrongGoiDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var goiDichVuId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _goiDichVuChiTietDichVuKhamBenhRepository.TableNoTracking
                .Where(x => x.GoiDichVuId == goiDichVuId)
                .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                {
                    NhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                    TenNhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                    MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                    LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    DonGia = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens
                                    .Where(a => a.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId
                                                && a.TuNgay.Date <= DateTime.Now.Date
                                                && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date))
                                    .Select(a => a.Gia).FirstOrDefault(),
                    SoLan = item.SoLan
                })
                .Union(
                    _goiDichVuChiTietDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.GoiDichVuId == goiDichVuId)
                        .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                        {
                            NhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                            TenNhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                            MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                            TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                            LoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            DonGia = item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens
                                .Where(a => a.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId
                                            && a.TuNgay.Date <= DateTime.Now.Date
                                            && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date))
                                .Select(a => a.Gia).FirstOrDefault(),
                            SoLan = item.SoLan
                        })
                    )
                .Union(
                    _goiDichVuChiTietDichVuGiuongRepository.TableNoTracking
                        .Where(x => x.GoiDichVuId == goiDichVuId)
                        .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                        {
                            Id = item.DichVuGiuongBenhVienId,
                            NhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                            TenNhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                            MaDichVu = item.DichVuGiuongBenhVien.Ma,
                            TenDichVu = item.DichVuGiuongBenhVien.Ten,
                            LoaiGia = item.NhomGiaDichVuGiuongBenhVien.Ten,
                            DonGia = item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens
                                .Where(a => a.NhomGiaDichVuGiuongBenhVienId == item.NhomGiaDichVuGiuongBenhVienId
                                            && a.TuNgay.Date <= DateTime.Now.Date
                                            && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date))
                                .Select(a => a.Gia).FirstOrDefault(),
                            SoLan = item.SoLan
                        })
                )
                .ApplyLike(queryInfo.SearchTerms, x => x.MaDichVu, x => x.TenDichVu, x => x.LoaiGia);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString)
                .Skip(queryInfo.Skip).Take(queryInfo.Take)
                .ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetChiTietDichVuThuongDungTrongGoiTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var goiDichVuId = long.Parse(queryInfo.AdditionalSearchString);
            var query = _goiDichVuChiTietDichVuKhamBenhRepository.TableNoTracking
                .Where(x => x.GoiDichVuId == goiDichVuId)
                .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                {
                    NhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                    TenNhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                    MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                    LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    DonGia = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens
                                    .Where(a => a.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId
                                                && a.TuNgay.Date <= DateTime.Now.Date
                                                && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date))
                                    .Select(a => a.Gia).FirstOrDefault(),
                    SoLan = item.SoLan
                })
                .Union(
                    _goiDichVuChiTietDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.GoiDichVuId == goiDichVuId)
                        .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                        {
                            NhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                            TenNhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                            MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                            TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                            LoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            DonGia = item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens
                                .Where(a => a.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId
                                            && a.TuNgay.Date <= DateTime.Now.Date
                                            && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date))
                                .Select(a => a.Gia).FirstOrDefault(),
                            SoLan = item.SoLan
                        })
                    )
                .Union(
                    _goiDichVuChiTietDichVuGiuongRepository.TableNoTracking
                        .Where(x => x.GoiDichVuId == goiDichVuId)
                        .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                        {
                            Id = item.DichVuGiuongBenhVienId,
                            NhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                            TenNhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                            MaDichVu = item.DichVuGiuongBenhVien.Ma,
                            TenDichVu = item.DichVuGiuongBenhVien.Ten,
                            LoaiGia = item.NhomGiaDichVuGiuongBenhVien.Ten,
                            DonGia = item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens
                                .Where(a => a.NhomGiaDichVuGiuongBenhVienId == item.NhomGiaDichVuGiuongBenhVienId
                                            && a.TuNgay.Date <= DateTime.Now.Date
                                            && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date))
                                .Select(a => a.Gia).FirstOrDefault(),
                            SoLan = item.SoLan
                        })
                )
                .ApplyLike(queryInfo.SearchTerms, x => x.MaDichVu, x => x.TenDichVu, x => x.LoaiGia);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetGoiDichVuCuaBenhNhanDataForGridAsync(QueryInfo queryInfo)
        {
            //Có cập nhật bỏ await
            BuildDefaultSortExpression(queryInfo);

            var arrParam = queryInfo.AdditionalSearchString.Split(";");
            var benhNhanId = long.Parse(arrParam[0]);
            bool isCapGiuong = arrParam[1].ToLower() == "true";


            var dichVuKhamBenhDaChiDinhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauGoiDichVuId != null)
                #region cập nhật 06/12/2022
                .Select(x => new
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId,
                    DichVuKhamBenhBenhVienId = x.DichVuKhamBenhBenhVienId,
                    TrangThai = x.TrangThai,
                    DonGiaSauChietKhau = x.DonGiaSauChietKhau
                })
                #endregion
                .ToList();
            var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId != null)
                #region cập nhật 06/12/2022
                .Select(x => new
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId,
                    DichVuKyThuatBenhVienId = x.DichVuKyThuatBenhVienId,
                    TrangThai = x.TrangThai,
                    DonGiaSauChietKhau = x.DonGiaSauChietKhau,
                    SoLan = x.SoLan
                })
                #endregion
                .ToList();

            var query = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
                #region cập nhật 06/12/2022
                //.Include(x => x.YeuCauDichVuKyThuats)
                //.Include(x => x.YeuCauKhamBenhs)
                #endregion
                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                            && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && x.DaQuyetToan != true // cập nhật 10/06/2021: ko hiển thị gói đã quyết toán
                            && x.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                    )
                .ToList();

            var result = query
                .Where(x => !isCapGiuong && (x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
                                                .Any(a => a.SoLan > dichVuKhamBenhDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuKhamBenhBenhVienId == a.DichVuKhamBenhBenhVienId).Count())
                                            || x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                                                .Any(a => a.SoLan > dichVuKyThuatDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuKyThuatBenhVienId == a.DichVuKyThuatBenhVienId).Sum(b => b.SoLan))
                                            || x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any())
                            || (isCapGiuong && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any())
                            )
                .Select(item => new GoiDichVuTheoBenhNhanGridVo()
                {
                    Id = item.Id,
                    // cập nhật 25/05/2021: chỉ hiện tên chương trình
                    TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten, // + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                    TongCong = item.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaGoi = item.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    BenhNhanDaThanhToan = item.SoTienBenhNhanDaChi ?? 0,

                    #region cập nhật 06/12/2022 tăng tốc độ loading
                    //DangDung = item.YeuCauKhamBenhs.Where(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    //                                           // cập nhật: tính tổng dv đã sử dụng trong gói -> bệnh nhân có cần đóng thêm tiền ko
                    //                                           //&& a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                    //                                           && a.YeuCauGoiDichVuId == item.Id).Sum(a => a.DonGiaSauChietKhau ?? 0)
                    //           + item.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                    //                                                  //&& a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                    //                                                  && a.YeuCauGoiDichVuId == item.Id).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0))
                    //            + item.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                    //                                                          //&& a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                    //                                                          && a.YeuCauGoiDichVuId == item.Id).Sum(a => a.DonGiaSauChietKhau ?? 0)

                    DangDung = dichVuKhamBenhDaChiDinhs.Where(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                   // cập nhật: tính tổng dv đã sử dụng trong gói -> bệnh nhân có cần đóng thêm tiền ko
                                                                   && a.YeuCauGoiDichVuId == item.Id).Sum(a => a.DonGiaSauChietKhau ?? 0)
                               + dichVuKyThuatDaChiDinhs.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                      && a.YeuCauGoiDichVuId == item.Id).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0))
                    #endregion
                })
                .ToArray();

            //tạm thời chưa xử lý dịch vụ giường
            //var dichVuGiuongDaChiDinhs = await _yeuCauDichVuGiuongRepository.TableNoTracking
            //    .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
            //                && x.TrangThai != EnumTrangThaiGiuongBenh.ChuaThucHien
            //                && x.YeuCauGoiDichVuId != null)
            //    .ToListAsync();

            //var query = _yeuCauGoiDichVuRepository.TableNoTracking
            //    .Where(x => x.BenhNhanId == benhNhanId
            //                && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
            //                && (x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
            //                        .Any(a => a.SoLan > dichVuKhamBenhDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuKhamBenhBenhVienId == a.DichVuKhamBenhBenhVienId).Count())
            //                    || x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
            //                        .Any(a => a.SoLan > dichVuKyThuatDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuKyThuatBenhVienId == a.DichVuKyThuatBenhVienId).Sum(b => b.SoLan))
            //                    //|| x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
            //                    //    .Any(a => a.SoLan > dichVuGiuongDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuGiuongBenhVienId == a.DichVuGiuongBenhVienId).Count())
            //                ))
            //    .Select(item => new GoiDichVuTheoBenhNhanGridVo()
            //    {
            //        Id = item.Id,
            //        TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
            //        TongCong = item.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
            //        GiaGoi = item.ChuongTrinhGoiDichVu.GiaSauChietKhau,
            //        BenhNhanDaThanhToan = item.SoTienBenhNhanDaChi ?? 0,
            //        DangDung = item.YeuCauKhamBenhs.Where(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Sum(a => a.DonGiaSauChietKhau ?? 0) 
            //                   + item.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(a => a.SoLan * a.DonGiaSauChietKhau ?? 0)
            //                   //+ item.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy).Sum(a => a.DonGiaSauChietKhau ?? 0)
            //    });

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString)
            //    .Skip(queryInfo.Skip).Take(queryInfo.Take)
            //    .ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);

            //return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

            return new GridDataSource { Data = result, TotalRowCount = result.Length };
        }
        public async Task<GridDataSource> GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //Có cập nhật bỏ await
            var arrParam = queryInfo.AdditionalSearchString.Split(";");
            var benhNhanId = long.Parse(arrParam[0]);
            bool isCapGiuong = arrParam[1].ToLower() == "true";

            var dichVuKhamBenhDaChiDinhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauGoiDichVuId != null)
                #region cập nhật 06/12/2022
                .Select(x => new
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId,
                    DichVuKhamBenhBenhVienId = x.DichVuKhamBenhBenhVienId,
                    TrangThai = x.TrangThai,
                    DonGiaSauChietKhau = x.DonGiaSauChietKhau
                })
                #endregion
                .ToList();
            var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId != null)
                #region cập nhật 06/12/2022
                .Select(x => new
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId,
                    DichVuKyThuatBenhVienId = x.DichVuKyThuatBenhVienId,
                    TrangThai = x.TrangThai,
                    DonGiaSauChietKhau = x.DonGiaSauChietKhau,
                    SoLan = x.SoLan
                })
                #endregion
                .ToList();

            var query = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
                #region cập nhật 06/12/2022
                    //.Include(x => x.YeuCauDichVuKyThuats)
                    //.Include(x => x.YeuCauKhamBenhs)
                #endregion
                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                            && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && x.DaQuyetToan != true // cập nhật 10/06/2021: ko hiển thị gói đã quyết toán
                            && x.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                    )
                .ToList();

            var result = query
                .Where(x => !isCapGiuong && (x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
                                                 .Any(a => a.SoLan > dichVuKhamBenhDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuKhamBenhBenhVienId == a.DichVuKhamBenhBenhVienId).Count())
                                             || x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                                                 .Any(a => a.SoLan > dichVuKyThuatDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuKyThuatBenhVienId == a.DichVuKyThuatBenhVienId).Sum(b => b.SoLan))
                                             || x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any())
                            || (isCapGiuong && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any())
                )
                .Select(item => new GoiDichVuTheoBenhNhanGridVo()
                {
                    Id = item.Id,
                    TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten,// + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu
                })
                .ToArray();

            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = result.Length };
        }

        public async Task<GridDataSourceChiTietGoiDichVuTheoBenhNhan> GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(QueryInfo queryInfo, bool isDieuTriNoiTru = false, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null)
        {
            BuildDefaultSortExpression(queryInfo);

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = await _yeuCauGoiDichVuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;

            var lstDichVuDangChon = new List<ChiTietGoiDichVuChiDinhTheoBenhNhanVo>();
            if (!string.IsNullOrEmpty(searchObj[1]) && searchObj[1] != "undefined" && searchObj[1] != "null")
            {
                lstDichVuDangChon = JsonConvert.DeserializeObject<List<ChiTietGoiDichVuChiDinhTheoBenhNhanVo>>(searchObj[1]);
            }

            var dichVuKhamBenhDaChiDinhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId //yeuCauGoiDichVu.BenhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                #region Cập nhật 06/12/2022
                 .Select(x => new
                 {
                     DichVuKhamBenhBenhVienId = x.DichVuKhamBenhBenhVienId
                 })
                #endregion
                .ToList();
            var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId //yeuCauGoiDichVu.BenhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                #region Cập nhật 06/12/2022
                 .Select(x => new
                 {
                     DichVuKyThuatBenhVienId = x.DichVuKyThuatBenhVienId,
                     SoLan = x.SoLan
                 })
                #endregion
                .ToList();
            //var dichVuGiuongDaChiDinhs = await _yeuCauDichVuGiuongRepository.TableNoTracking
            //    .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId //yeuCauGoiDichVu.BenhNhanId
            //                && x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
            //                && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
            //    .ToListAsync();

            //if (isDieuTriNoiTru)
            //{
            //    var yeuCauTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking
            //        .Include(x => x.NoiTruBenhAn)
            //        .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.KhoaPhong)
            //        .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.PhongBenhVien)
            //        .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.GiuongBenh)
            //        .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.KhoaPhong)
            //        .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.PhongBenhVien)
            //        .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.GiuongBenh)
            //        .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.GiuongBenh).ThenInclude(gb => gb.PhongBenhVien).ThenInclude(gb => gb.KhoaPhong)
            //        .Where(x => x.BenhNhanId == benhNhanId
            //                    && x.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
            //        .ToList();
            //    foreach (var yeuCauTiepNhan in yeuCauTiepNhans)
            //    {
            //        if (yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)
            //        {
            //            var chiPhiGiuong = TinhChiPhiDichVuGiuong(yeuCauTiepNhan);
            //        }
            //    }
            //}

            var isPTTT = searchObj[2].ToLower() == "true";
            var isCapGiuong = searchObj[3].ToLower() == "true";

            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            var query = _chuongTrinhGoiDichVuKhamBenhRepository.TableNoTracking
                .Where(x => (!isCapGiuong && x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId) || (isCapGiuong && x.ChuongTrinhGoiDichVuId == 0)) // cheat điều kiện, nếu là cấp giường thì ko hiện dv khác dv giường
                .ApplyLike(queryInfo.SearchTerms, x => x.DichVuKhamBenhBenhVien.Ma, x => x.DichVuKhamBenhBenhVien.Ten)
                .Select(item => new ChiTietGoiDichVuTheoBenhNhanGridVo()
                {
                    YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                    TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                    ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                    ChuongTrinhGoiDichVuChiTietId = item.Id,
                    DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                    MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                    TenLoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    SoLuong = item.SoLan,
                    DonGia = item.DonGiaSauChietKhau,
                    SoLuongDaDung = dichVuKhamBenhDaChiDinhs.Where(a => a.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId).Count(),
                    SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                 && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                 && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                                                                                                 && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh).SoLuongSuDung : 0,
                    IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                           && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh),
                    IsPTTT = isPTTT,
                    IsDieuTriNoiTru = isDieuTriNoiTru
                })
                .Union(
                    _chuongTrinhGoiDichVuKyThuatRepository.TableNoTracking
                        .Where(x => (!isCapGiuong && x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId) || (isCapGiuong && x.ChuongTrinhGoiDichVuId == 0))// cheat điều kiện, nếu là cấp giường thì ko hiện dv khác dv giường
                        .ApplyLike(queryInfo.SearchTerms, x => x.DichVuKyThuatBenhVien.Ma, x => x.DichVuKyThuatBenhVien.Ten)
                        .Select(item => new ChiTietGoiDichVuTheoBenhNhanGridVo()
                        {
                            YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                            TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                            ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                            ChuongTrinhGoiDichVuChiTietId = item.Id,
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                            TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                            NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                            TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            SoLuong = item.SoLan,
                            DonGia = item.DonGiaSauChietKhau,
                            SoLuongDaDung = dichVuKyThuatDaChiDinhs.Where(a => a.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId).Sum(b => b.SoLan),
                            SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                                                                                                                       && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung : 0,
                            IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat),
                            IsPTTT = isPTTT,
                            IsDieuTriNoiTru = isDieuTriNoiTru,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                            IsNhomTiemChung = nhomTiemChungId == null ? false : item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId
                        })
                    )
                .Union(
                    _chuongTrinhGoiDichVuGiuongRepository.TableNoTracking
                        .Where(x => x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId)
                        .ApplyLike(queryInfo.SearchTerms, x => x.DichVuGiuongBenhVien.Ma, x => x.DichVuGiuongBenhVien.Ten)
                        .Select(item => new ChiTietGoiDichVuTheoBenhNhanGridVo()
                        {
                            YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                            TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                            ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                            ChuongTrinhGoiDichVuChiTietId = item.Id,
                            DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                            MaDichVu = item.DichVuGiuongBenhVien.Ma,
                            TenDichVu = item.DichVuGiuongBenhVien.Ten,
                            NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                            TenLoaiGia = item.NhomGiaDichVuGiuongBenhVien.Ten,
                            SoLuong = item.SoLan,
                            DonGia = item.DonGiaSauChietKhau,
                            SoLuongDaDung = dichVuGiuongDaChiDinhs.Where(a => a.DichVuBenhVienId == item.DichVuGiuongBenhVienId 
                                                                              && a.YeuCauGoiDichVuId == yeuCauGoiDichVuId 
                                                                              && a.NhomGiaDichVuBenhVienId == item.NhomGiaDichVuGiuongBenhVienId).Sum(b => b.SoLuongDaSuDung),
                            SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                                                                                                                       && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh).SoLuongSuDung : 0,
                            IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh),
                            IsDichVuGiuong = true
                        })
                    );

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString)
                .Skip(queryInfo.Skip).Take(queryInfo.Take)
                .ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSourceChiTietGoiDichVuTheoBenhNhan { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSourceChiTietGoiDichVuTheoBenhNhan> GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(QueryInfo queryInfo, bool isDieuTriNoiTru = false)
        {
            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = await _yeuCauGoiDichVuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;

            var lstDichVuDangChon = new List<ChiTietGoiDichVuChiDinhTheoBenhNhanVo>();
            if (!string.IsNullOrEmpty(searchObj[1]) && searchObj[1] != "undefined" && searchObj[1] != "null")
            {
                lstDichVuDangChon = JsonConvert.DeserializeObject<List<ChiTietGoiDichVuChiDinhTheoBenhNhanVo>>(searchObj[1]);
            }

            var dichVuKhamBenhDaChiDinhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId //yeuCauGoiDichVu.BenhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                #region Cập nhật 06/12/2022
                 .Select(x => new
                 {
                     DichVuKhamBenhBenhVienId = x.DichVuKhamBenhBenhVienId
                 })
                #endregion
                .ToList();
            var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId //yeuCauGoiDichVu.BenhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                #region Cập nhật 06/12/2022
                 .Select(x => new
                 {
                     DichVuKyThuatBenhVienId = x.DichVuKyThuatBenhVienId,
                     SoLan = x.SoLan
                 })
                #endregion
                .ToList();
            var dichVuGiuongDaChiDinhs = _yeuCauDichVuGiuongRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId //yeuCauGoiDichVu.BenhNhanId
                            && x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                            && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                .ToList();

            var isPTTT = searchObj[2].ToLower() == "true";
            var isCapGiuong = searchObj[3].ToLower() == "true";

            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            var query = _chuongTrinhGoiDichVuKhamBenhRepository.TableNoTracking
                .Where(x => (!isCapGiuong && x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId) || (isCapGiuong && x.ChuongTrinhGoiDichVuId == 0))// cheat điều kiện, nếu là cấp giường thì ko hiện dv khác dv giường
                .ApplyLike(queryInfo.SearchTerms, x => x.DichVuKhamBenhBenhVien.Ma, x => x.DichVuKhamBenhBenhVien.Ten)
                .Select(item => new ChiTietGoiDichVuTheoBenhNhanGridVo()
                {
                    YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                    TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                    ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                    ChuongTrinhGoiDichVuChiTietId = item.Id,
                    DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                    MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                    TenLoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    SoLuong = item.SoLan,
                    DonGia = item.DonGiaSauChietKhau,
                    SoLuongDaDung = dichVuKhamBenhDaChiDinhs.Where(a => a.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId).Count(),
                    SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                 && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                 && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                                                                                                 && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh).SoLuongSuDung : 0,
                    IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                           && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh),
                    IsPTTT = isPTTT
                })
                .Union(
                    _chuongTrinhGoiDichVuKyThuatRepository.TableNoTracking
                        .Where(x => (!isCapGiuong && x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId) || (isCapGiuong && x.ChuongTrinhGoiDichVuId == 0))// cheat điều kiện, nếu là cấp giường thì ko hiện dv khác dv giường
                        .ApplyLike(queryInfo.SearchTerms, x => x.DichVuKyThuatBenhVien.Ma, x => x.DichVuKyThuatBenhVien.Ten)
                        .Select(item => new ChiTietGoiDichVuTheoBenhNhanGridVo()
                        {
                            YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                            TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                            ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                            ChuongTrinhGoiDichVuChiTietId = item.Id,
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                            TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                            NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                            TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            SoLuong = item.SoLan,
                            DonGia = item.DonGiaSauChietKhau,
                            SoLuongDaDung = dichVuKyThuatDaChiDinhs.Where(a => a.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId).Sum(b => b.SoLan),
                            SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                                                                                                                       && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung : 0,
                            IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat),
                            IsPTTT = isPTTT,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien)

                        })
                    )
                .Union(
                    _chuongTrinhGoiDichVuGiuongRepository.TableNoTracking
                        .Where(x => x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId)
                        .ApplyLike(queryInfo.SearchTerms, x => x.DichVuGiuongBenhVien.Ma, x => x.DichVuGiuongBenhVien.Ten)
                        .Select(item => new ChiTietGoiDichVuTheoBenhNhanGridVo()
                        {
                            YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                            TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                            ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                            ChuongTrinhGoiDichVuChiTietId = item.Id,
                            DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                            MaDichVu = item.DichVuGiuongBenhVien.Ma,
                            TenDichVu = item.DichVuGiuongBenhVien.Ten,
                            NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                            TenLoaiGia = item.NhomGiaDichVuGiuongBenhVien.Ten,
                            SoLuong = item.SoLan,
                            DonGia = item.DonGiaSauChietKhau,
                            SoLuongDaDung = dichVuGiuongDaChiDinhs.Where(a => a.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId).Sum(b => 1),
                            SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                                                                                                                       && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh).SoLuongSuDung : 0,
                            IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh),
                            IsDichVuGiuong = true
                        })
                    );
            //.ApplyLike(queryInfo.SearchTerms, x => x.MaDichVu, x => x.TenDichVu);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSourceChiTietGoiDichVuTheoBenhNhan { TotalRowCount = countTask.Result };
        }

        #endregion

        public async Task<List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>> KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, List<long> lstGoiDichVuId, List<DichVuDaChonYCTNVo> danhSachDichVuChons = null, bool laPTTT = false, long? phieuDieuTriId = null)
        {
            var lstDichVuLoi = new List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>();

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var lstDichVuTrongGoi = await _goiDichVuChiTietDichVuKhamBenhRepository.TableNoTracking
                .Where(x => lstGoiDichVuId.Contains(x.GoiDichVuId)) // lstGoiDichVuId.Any(a => a == x.GoiDichVuId))
                .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                {
                    GoiDichVuId = item.GoiDichVuId,
                    TenGoiDichVu = item.GoiDichVu.Ten,
                    Id = item.DichVuKhamBenhBenhVienId,
                    TenNhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                    NhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                    MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                    LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    DonGia = item.DichVuKhamBenhBenhVien
                                 .DichVuKhamBenhBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId
                                                                                         && a.TuNgay.Date <= DateTime.Now.Date
                                                                                         && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                        item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId
                                                                                                           && a.TuNgay.Date <= DateTime.Now.Date
                                                                                                           && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    SoLan = item.SoLan,
                })
            .Union(
                _goiDichVuChiTietDichVuKyThuatRepository.TableNoTracking
                    .Where(x => lstGoiDichVuId.Contains(x.GoiDichVuId) // lstGoiDichVuId.Any(a => a == x.GoiDichVuId)

                                // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
                                && (nhomTiemChungId == null || x.DichVuKyThuatBenhVien.NhomDichVuBenhVienId != nhomTiemChungId))
                    .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                    {
                        GoiDichVuId = item.GoiDichVuId,
                        TenGoiDichVu = item.GoiDichVu.Ten,
                        Id = item.DichVuKyThuatBenhVienId,
                        TenNhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                        NhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        LaSuatAn = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == (int)Enums.LoaiDichVuKyThuat.SuatAn,
                        MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                        TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                        LoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                        DonGia = item.DichVuKyThuatBenhVien
                                     .DichVuKyThuatVuBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId
                                                                                             && a.TuNgay.Date <= DateTime.Now.Date
                                                                                             && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                            item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId
                                                                                                               && a.TuNgay.Date <= DateTime.Now.Date
                                                                                                               && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                        SoLan = item.SoLan
                    })
            )
            .Union(
                _goiDichVuChiTietDichVuGiuongRepository.TableNoTracking
                    .Where(x => lstGoiDichVuId.Contains(x.GoiDichVuId)) // lstGoiDichVuId.Any(a => a == x.GoiDichVuId))
                    .Select(item => new ChiTietNhomGoiDichVuThuongDungGridVo()
                    {
                        GoiDichVuId = item.GoiDichVuId,
                        TenGoiDichVu = item.GoiDichVu.Ten,
                        Id = item.DichVuGiuongBenhVienId,
                        TenNhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh.GetDescription(),
                        NhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                        MaDichVu = item.DichVuGiuongBenhVien.Ma,
                        TenDichVu = item.DichVuGiuongBenhVien.Ten,
                        LoaiGia = item.NhomGiaDichVuGiuongBenhVien.Ten,
                        DonGia = item.DichVuGiuongBenhVien
                                     .DichVuGiuongBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuGiuongBenhVienId == item.NhomGiaDichVuGiuongBenhVienId
                                                                                              && a.TuNgay.Date <= DateTime.Now.Date
                                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                            item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuGiuongBenhVienId == item.NhomGiaDichVuGiuongBenhVienId
                                                                                                               && a.TuNgay.Date <= DateTime.Now.Date
                                                                                                               && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                        SoLan = item.SoLan
                    })
            ).ToListAsync();

            if (laPTTT)
            {
                lstDichVuTrongGoi = lstDichVuTrongGoi.Where(x => x.NhomDichVu == EnumNhomGoiDichVu.DichVuKyThuat)
                    .ToList();
            }

            if (phieuDieuTriId != null)
            {
                //BVHD-3575: cập nhật cho phép chỉ định dv khám từ nội trú
                lstDichVuTrongGoi = lstDichVuTrongGoi.Where(x => (x.NhomDichVu == EnumNhomGoiDichVu.DichVuKyThuat && !x.LaSuatAn) 
                                                                 || x.NhomDichVu == EnumNhomGoiDichVu.DichVuKhamBenh)
                    .ToList();
            }

            var lstDichVuTrungDichVuDaThem = new List<ChiTietNhomGoiDichVuThuongDungDangChonVo>();
            if (yeuCauTiepNhanId != 0)
            {
                DateTime? ngayDieuTri = null;
                NoiTruPhieuDieuTri noiTruPhieuDieuTri = null;
                if(phieuDieuTriId != null)
                {
                    noiTruPhieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking
                        .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.YeuCauTiepNhan)
                        .First(x => x.Id == phieuDieuTriId);
                    ngayDieuTri = noiTruPhieuDieuTri.NgayDieuTri.Date;
                }

                #region //Cập nhật 02/12/2022
                var lstDichVuKhamIdDaChon = lstDichVuTrongGoi.Where(x => x.NhomDichVu == EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.Id).Distinct().ToList();
                var lstDichVuKyThuatIdDaChon = lstDichVuTrongGoi.Where(x => x.NhomDichVu == EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.Id).Distinct().ToList();
                var lstDichVuGiuongIdDaChon = lstDichVuTrongGoi.Where(x => x.NhomDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh).Select(x => x.Id).Distinct().ToList();

                lstDichVuTrungDichVuDaThem = new List<ChiTietNhomGoiDichVuThuongDungDangChonVo>();
                if (lstDichVuKyThuatIdDaChon.Any())
                {
                    var kts = lstDichVuTrungDichVuDaThem = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId

                                //Cập nhật 02/12/2022
                                //&& lstDichVuTrongGoi.Any(a => a.NhomDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                                //                                   && a.Id == x.DichVuKyThuatBenhVienId)
                                && lstDichVuKyThuatIdDaChon.Contains(x.DichVuKyThuatBenhVienId)


                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy

                                //&& x.NoiTruPhieuDieuTriId == phieuDieuTriId)
                                && (phieuDieuTriId == null || x.NoiTruPhieuDieuTri.NgayDieuTri.Date == ngayDieuTri)
                                )
                    .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                    {
                        NhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuId = item.DichVuKyThuatBenhVienId,
                        TenDichVu = item.TenDichVu
                    })
                    .ToList();
                    if (kts.Any())
                    {
                        lstDichVuTrungDichVuDaThem.AddRange(kts);
                    }
                }

                if (lstDichVuKhamIdDaChon.Any())
                {
                    var khams = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(x => //BVHD-3575: bổ sung dịch vụ khám từ nội trú
                                        x.YeuCauTiepNhanId == (phieuDieuTriId == null ? yeuCauTiepNhanId : (noiTruPhieuDieuTri.NoiTruBenhAn.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId ?? yeuCauTiepNhanId))

                                        //Cập nhật 02/12/2022
                                        //&& lstDichVuTrongGoi.Any(a => a.NhomDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                                        //                                   && a.Id == x.DichVuKhamBenhBenhVienId)
                                        && lstDichVuKhamIdDaChon.Contains(x.DichVuKhamBenhBenhVienId)

                                        && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham

                                        //BVHD-3575: bổ sung dịch vụ khám từ nội trú
                                        //&& (phieuDieuTriId == null || x.Id == 0)) // cheat trường hợp trong phiếu điều trị -> chỉ chỉ định dvkt
                                        && (phieuDieuTriId == null || (x.LaChiDinhTuNoiTru != null && x.LaChiDinhTuNoiTru == true && x.ThoiDiemDangKy.Date == ngayDieuTri))
                                        )
                            .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                            {
                                NhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                DichVuId = item.DichVuKhamBenhBenhVienId,
                                TenDichVu = item.TenDichVu
                            })
                        .ToList();
                    if (khams.Any())
                    {
                        lstDichVuTrungDichVuDaThem.AddRange(khams);
                    }
                }

                if (lstDichVuGiuongIdDaChon.Any())
                {
                    var giuongs = _yeuCauDichVuGiuongRepository.TableNoTracking
                            .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId

                                        //Cập nhật 02/12/2022
                                        //&& lstDichVuTrongGoi.Any(a => a.NhomDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh
                                        //                                   && a.Id == x.DichVuGiuongBenhVienId)
                                        && lstDichVuGiuongIdDaChon.Contains(x.DichVuGiuongBenhVienId)

                                        && x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                        && (phieuDieuTriId == null || x.Id == 0)) // cheat trường hợp trong phiếu điều trị -> chỉ chỉ định dvkt
                            .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                            {
                                NhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                                DichVuId = item.DichVuGiuongBenhVienId,
                                TenDichVu = item.Ten
                            })
                            .ToList();
                    if (giuongs.Any())
                    {
                        lstDichVuTrungDichVuDaThem.AddRange(giuongs);
                    }
                }
                lstDichVuTrungDichVuDaThem = lstDichVuTrungDichVuDaThem.Distinct().ToList();
                #endregion
            }
            else
            {
                lstDichVuTrungDichVuDaThem = danhSachDichVuChons
                    .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                    {
                        NhomDichVu = EnumHelper.GetValueFromDescription<EnumNhomGoiDichVu>(item.Nhom), //(EnumNhomGoiDichVu)Enum.Parse(typeof(EnumNhomGoiDichVu), item.Nhom),
                        DichVuId = item.MaDichVuId,
                        TenDichVu = item.TenDichVu
                    })
                    .Distinct()
                    .ToList();
            }

            // kiểm tra dịch vụ lỗi
            var lstDichVuTrungTrongGoi = lstDichVuTrongGoi
                .GroupBy(x => new { x.Id, x.NhomDichVu })
                .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                {
                    NhomDichVu = item.First().NhomDichVu,
                    DichVuId = item.First().Id,
                    TenDichVu = item.First().TenDichVu,
                    SoLanChon = item.Count()
                }).Where(x => x.SoLanChon > 1)
                .ToList();
            var lstDichVuChuaNhapDonGia = lstDichVuTrongGoi.Where(x => x.DonGia == null).ToList(); //|| x.DonGia == 0 <= có cập nhật cho phép nhập đơn giá bệnh viện = 0
            foreach (var dichVu in lstDichVuTrongGoi)
            {
                var dichVuLoi = lstDichVuLoi.FirstOrDefault(x => x.GoiDichVuId == dichVu.GoiDichVuId
                                                                 && x.DichVuId == dichVu.Id
                                                                 && x.NhomGoiDichVu == dichVu.NhomDichVu);
                if (dichVuLoi == null)
                {
                    dichVuLoi = new ChiDinhGoiDichVuThuongDungDichVuLoiVo()
                    {
                        GoiDichVuId = dichVu.GoiDichVuId,
                        DichVuId = dichVu.Id,
                        TenDichVu = dichVu.TenDichVu,
                        NhomGoiDichVu = dichVu.NhomDichVu,
                        TenGoiDichVu = dichVu.TenGoiDichVu,
                        //LoaiLoi = LoaiLoiGoiDichVu.Trung
                    };
                }
                if (lstDichVuTrungTrongGoi.Any(x => x.DichVuId == dichVu.Id
                                                    && x.NhomDichVu == dichVu.NhomDichVu))
                {
                    //var dichVuLoi = new ChiDinhGoiDichVuThuongDungDichVuLoiVo()
                    //{
                    //    GoiDichVuId = dichVu.GoiDichVuId,
                    //    DichVuId = dichVu.Id,
                    //    TenDichVu = dichVu.TenDichVu,
                    //    NhomGoiDichVu = dichVu.NhomDichVu,
                    //    TenGoiDichVu = dichVu.TenGoiDichVu,
                    //    LoaiLoi = LoaiLoiGoiDichVu.Trung
                    //};
                    dichVuLoi.LoaiLois.Add(LoaiLoiGoiDichVu.Trung.GetDescription());
                    lstDichVuLoi.Add(dichVuLoi);
                }
                if (lstDichVuTrungDichVuDaThem.Any(x => x.DichVuId == dichVu.Id
                                                        && x.NhomDichVu == dichVu.NhomDichVu))
                {
                    //var dichVuLoi = new ChiDinhGoiDichVuThuongDungDichVuLoiVo()
                    //{
                    //    GoiDichVuId = dichVu.GoiDichVuId,
                    //    DichVuId = dichVu.Id,
                    //    TenDichVu = dichVu.TenDichVu,
                    //    NhomGoiDichVu = dichVu.NhomDichVu,
                    //    TenGoiDichVu = dichVu.TenGoiDichVu,
                    //    LoaiLoi = LoaiLoiGoiDichVu.Trung
                    //};
                    dichVuLoi.LoaiLois.Add(LoaiLoiGoiDichVu.Trung.GetDescription());
                    lstDichVuLoi.Add(dichVuLoi);
                }
                if (lstDichVuChuaNhapDonGia.Any(x => x.Id == dichVu.Id
                                                        && x.NhomDichVu == dichVu.NhomDichVu))
                {
                    //var dichVuLoi = new ChiDinhGoiDichVuThuongDungDichVuLoiVo()
                    //{
                    //    GoiDichVuId = dichVu.GoiDichVuId,
                    //    DichVuId = dichVu.Id,
                    //    TenDichVu = dichVu.TenDichVu,
                    //    NhomGoiDichVu = dichVu.NhomDichVu,
                    //    TenGoiDichVu = dichVu.TenGoiDichVu,
                    //    LoaiLoi = LoaiLoiGoiDichVu.ChuaNhapGia,
                    //    KhongThem = true
                    //};
                    dichVuLoi.LoaiLois.Add(LoaiLoiGoiDichVu.ChuaNhapGia.GetDescription());
                    dichVuLoi.KhongThem = true;
                    dichVuLoi.IsDisabled = true;
                    lstDichVuLoi.Add(dichVuLoi);
                }
            }
            return lstDichVuLoi.Distinct().ToList();
        }

        public async Task XuLyThemGoiDichVuThuongDungAsync(YeuCauTiepNhan yeuCauTiepNhan, YeuCauThemGoiDichVuThuongDungVo yeuCauVo)
        {
            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var coBHYT = yeuCauTiepNhan.CoBHYT ?? false;
            var duocHuongBaoHiem = yeuCauTiepNhan.NoiTruBenhAn == null && yeuCauTiepNhan.QuyetToanTheoNoiTru != true ? false : true; // <== áp dụng đối với trường hợp thêm trực tiếp dv vào YCTN
            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauVo.YeuCauKhamBenhId);
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thoiDiemHienTai = DateTime.Now;
            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            var lstDichVuKhamBenhTrongGoi = await _goiDichVuChiTietDichVuKhamBenhRepository.TableNoTracking
                .Where(x => yeuCauVo.GoiDichVuIds.Contains(x.GoiDichVuId) // yeuCauVo.GoiDichVuIds.Any(a => a == x.GoiDichVuId)
                            && !yeuCauVo.DichVuKhongThems.Any(a => a.GoiDichVuId == x.GoiDichVuId
                                                                   && a.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                                                                   && a.DichVuId == x.DichVuKhamBenhBenhVienId))
                .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                {
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                    DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                    MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                    MaGiaDichVu = item.DichVuKhamBenhBenhVien.DichVuKhamBenh != null ? item.DichVuKhamBenhBenhVien.DichVuKhamBenh.MaTT37 : "",
                    NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
                    #region Cập nhật 16/12/2022: gán giá BH và bệnh viện
                    //DonGiaBenhVien = item.DichVuKhamBenhBenhVien
                    //                     .DichVuKhamBenhBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId
                    //                                                                             && a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                             && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                    //                item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId
                    //                                                                                       && a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                       && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    //DonGiaBaoHiem = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems
                    //                    .FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                    //                                         && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                    //                item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                      && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    #endregion
                    //BVHD-3575: xử lý thêm trường hợp chỉ định dv khám từ nội trú
                    //thì sẽ lấy thông tin thẻ của YCTN ngoại trú nếu có
                    CoBHYT = yeuCauVo.TiepNhanNgoaiTruCoBHYT ?? coBHYT,
                    SoLuong = 1,
                    #region Cập nhật 16/12/2022: gán giá BH và bệnh viện
                    //TiLeBaoHiemThanhToan = item.DichVuKhamBenhBenhVien
                    //                           .DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                  && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                    //                        item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                      && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                    #endregion
                })
                .Union(
                    _goiDichVuChiTietDichVuKyThuatRepository.TableNoTracking
                .Where(x => yeuCauVo.GoiDichVuIds.Contains(x.GoiDichVuId) // yeuCauVo.GoiDichVuIds.Any(a => a == x.GoiDichVuId)
                            && !yeuCauVo.DichVuKhongThems.Any(a => a.GoiDichVuId == x.GoiDichVuId
                                                                  && a.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                                                                  && a.DichVuId == x.DichVuKyThuatBenhVienId)
                            && (nhomTiemChungId == null || x.DichVuKyThuatBenhVien.NhomDichVuBenhVienId != nhomTiemChungId))
                .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                {
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                    LaSuatAn = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == (int)Enums.LoaiDichVuKyThuat.SuatAn,
                    DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                    MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                    MaGiaDichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia : "",
                    TenGia = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.TenGia : "",
                    Ma4350 = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350 : "",
                    NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                    #region Cập nhật 16/12/2022: gán giá BH và bệnh viện
                    //DonGiaBenhVien = item.DichVuKyThuatBenhVien
                    //                     .DichVuKyThuatVuBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId
                    //                                                                              && a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                    //                item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId
                    //                                                                                                   && a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                                   && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    //DonGiaBaoHiem = item.DichVuKyThuatBenhVien
                    //                    .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                    //                item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                                && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    #endregion
                    CoBHYT = coBHYT,
                    SoLuong = item.SoLan,
                    #region Cập nhật 16/12/2022: gán giá BH và bệnh viện
                    //TiLeBaoHiemThanhToan = item.DichVuKyThuatBenhVien
                    //                           .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                 && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                    //                        item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                                        && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                    #endregion
                    NhomChiPhiDichVuKyThuat = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                    NhomDichVuBenhVienId = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId
                }))
                //.Union(
                //    _goiDichVuChiTietDichVuGiuongRepository.TableNoTracking
                //.Where(x => yeuCauVo.GoiDichVuIds.Any(a => a == x.GoiDichVuId)
                //            && !yeuCauVo.DichVuKhongThems.Any(a => a.GoiDichVuId == x.GoiDichVuId
                //                                              && a.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh
                //                                              && a.DichVuId == x.DichVuGiuongBenhVienId))
                //.Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                //{
                //    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                //    DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                //    MaDichVu = item.DichVuGiuongBenhVien.Ma,
                //    TenDichVu = item.DichVuGiuongBenhVien.Ten,
                //    MaGiaDichVu = item.DichVuGiuongBenhVien.DichVuGiuong != null ? item.DichVuGiuongBenhVien.DichVuGiuong.MaTT37 : "",
                //    NhomGiaDichVuBenhVienId = item.NhomGiaDichVuGiuongBenhVienId,
                //    DonGiaBenhVien = item.DichVuGiuongBenhVien
                //                         .DichVuGiuongBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuGiuongBenhVienId == item.NhomGiaDichVuGiuongBenhVienId
                //                                                                               && a.TuNgay.Date <= DateTime.Now.Date
                //                                                                               && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                //                    item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuGiuongBenhVienId == item.NhomGiaDichVuGiuongBenhVienId
                //                                                                                                   && a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                                   && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                //    DonGiaBaoHiem = item.DichVuGiuongBenhVien
                //                        .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                             && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                //                    item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                                  && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                //    CoBHYT = coBHYT,
                //    SoLuong = item.SoLan,
                //    TiLeBaoHiemThanhToan = item.DichVuGiuongBenhVien
                //                               .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                    && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                //                            item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                //}))
                .ToListAsync();

            if (!lstDichVuKhamBenhTrongGoi.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.DichVu.Required"));
            }

            if (yeuCauVo.LaPhauThuatThuThuat == true)
            {
                lstDichVuKhamBenhTrongGoi = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat).ToList();
                if (!lstDichVuKhamBenhTrongGoi.Any())
                {
                    throw new Exception(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.NhomDichVu.KhongCoDichVuKyThuat"));
                }
            }

            #region Cập nhật 16/12/2022: gán giá BH và bệnh viện
            var lstDichVuKhamId = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.DichVuBenhVienId).Distinct().ToList();
            var lstDichVuKyThuatId = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.DichVuBenhVienId).Distinct().ToList();

            if(lstDichVuKhamId.Any())
            {
                var lstGiaBH = _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                    .Where(x => lstDichVuKhamId.Contains(x.DichVuKhamBenhBenhVienId))
                    .Select(x => new
                    {
                        x.DichVuKhamBenhBenhVienId,
                        x.TuNgay,
                        x.DenNgay,
                        x.TiLeBaoHiemThanhToan,
                        x.Gia
                    }).ToList();

                var lstGiaBV = _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                    .Where(x => lstDichVuKhamId.Contains(x.DichVuKhamBenhBenhVienId))
                    .Select(x => new
                    {
                        x.DichVuKhamBenhBenhVienId,
                        x.NhomGiaDichVuKhamBenhBenhVienId,
                        x.TuNgay,
                        x.DenNgay,
                        x.Gia
                    }).ToList();

                var lstKham = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh).ToList();
                foreach(var dv in lstKham)
                {
                    var giaBV = lstGiaBV.FirstOrDefault(x => x.DichVuKhamBenhBenhVienId == dv.DichVuBenhVienId
                                                            && x.NhomGiaDichVuKhamBenhBenhVienId == dv.NhomGiaDichVuBenhVienId
                                                            && x.TuNgay.Date <= DateTime.Now.Date
                                                            && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    var giaBH = lstGiaBH.FirstOrDefault(x => x.DichVuKhamBenhBenhVienId == dv.DichVuBenhVienId
                                                            && x.TuNgay.Date <= DateTime.Now.Date
                                                            && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    if(giaBV != null)
                    {
                        dv.DonGiaBenhVien = giaBV.Gia;
                    }

                    if(giaBH != null)
                    {
                        dv.DonGiaBaoHiem = giaBH.Gia;
                        dv.TiLeBaoHiemThanhToan = giaBH.TiLeBaoHiemThanhToan;
                    }
                }
            }

            if (lstDichVuKyThuatId.Any())
            {
                var lstGiaBH = _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                    .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                    .Select(x => new
                    {
                        x.DichVuKyThuatBenhVienId,
                        x.TuNgay,
                        x.DenNgay,
                        x.TiLeBaoHiemThanhToan,
                        x.Gia
                    }).ToList();

                var lstGiaBV = _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                    .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                    .Select(x => new
                    {
                        x.DichVuKyThuatBenhVienId,
                        x.NhomGiaDichVuKyThuatBenhVienId,
                        x.TuNgay,
                        x.DenNgay,
                        x.Gia
                    }).ToList();

                var lstKyThuat = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat).ToList();
                foreach (var dv in lstKyThuat)
                {
                    var giaBV = lstGiaBV.FirstOrDefault(x => x.DichVuKyThuatBenhVienId == dv.DichVuBenhVienId
                                                            && x.NhomGiaDichVuKyThuatBenhVienId == dv.NhomGiaDichVuBenhVienId
                                                            && x.TuNgay.Date <= DateTime.Now.Date
                                                            && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    var giaBH = lstGiaBH.FirstOrDefault(x => x.DichVuKyThuatBenhVienId == dv.DichVuBenhVienId
                                                            && x.TuNgay.Date <= DateTime.Now.Date
                                                            && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                    if (giaBV != null)
                    {
                        dv.DonGiaBenhVien = giaBV.Gia;
                    }

                    if (giaBH != null)
                    {
                        dv.DonGiaBaoHiem = giaBH.Gia;
                        dv.TiLeBaoHiemThanhToan = giaBH.TiLeBaoHiemThanhToan;
                    }
                }
            }
            #endregion

            if (yeuCauVo.PhieuDieuTriId != null)
            {
                lstDichVuKhamBenhTrongGoi = lstDichVuKhamBenhTrongGoi.Where(x => 
                    //BVHD-3575: cập nhật cho phép chỉ định dv khám từ nội trú
                    (x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat && !x.LaSuatAn) 
                    || x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh).ToList();
                if (!lstDichVuKhamBenhTrongGoi.Any())
                {
                    throw new Exception(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.NhomDichVu.KhongCoDichVuKyThuat"));
                }
            }

            var bacSiDangKys = await _hoatDongNhanVienRepository.TableNoTracking
                            .Where(x => x.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi
                                        && x.NhanVien.User.IsActive).ToListAsync();
            //var noiThucHienDVKBs = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToListAsync();
            //var noiThucHienDVKTs = await _dichVuKyThuatBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToListAsync();
            //var noiThucHienDVGs = await _dichVuGiuongBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToListAsync();

            #region cập nhật 02/12/2022: xử lý get list noi thực hiện theo dịch vụ
            var noiThucHienDVKBs = new List<DichVuKhamBenhBenhVienNoiThucHien>();
            var noiThucHienDVKTs = new List<DichVuKyThuatBenhVienNoiThucHien>();
            var dvktNoiThucHienUuTiens = new List<DichVuKyThuatBenhVienNoiThucHienUuTien>();
            #region Cập nhật 16/12/2022: gán giá BH và bệnh viện -> khai báo chung ở trên luôn
            //var lstDichVuKhamId = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.DichVuBenhVienId).Distinct().ToList();
            //var lstDichVuKyThuatId = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.DichVuBenhVienId).Distinct().ToList();
            #endregion

            if (lstDichVuKhamId.Any())
            {
                noiThucHienDVKBs = _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking
                    .Include(x => x.PhongBenhVien)
                    .Include(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                    .Where(x => lstDichVuKhamId.Contains(x.DichVuKhamBenhBenhVienId))
                    .ToList();
            }

            if (lstDichVuKyThuatId.Any())
            {
                dvktNoiThucHienUuTiens = _dichVuKyThuatBenhVienNoiThucHienUuTienRepository.TableNoTracking
                    .Include(x => x.PhongBenhVien)
                    .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                    .ToList();

                if (!dvktNoiThucHienUuTiens.Any())
                {
                    noiThucHienDVKTs = _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking
                        .Include(x => x.PhongBenhVien)
                        .Include(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                        .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                        .ToList();
                }
            }
            #endregion

            foreach (var dichVuBenhVien in lstDichVuKhamBenhTrongGoi)
            {
                switch (dichVuBenhVien.NhomGoiDichVu)
                {
                    case EnumNhomGoiDichVu.DichVuKhamBenh:
                        if (dichVuBenhVien.DonGiaBenhVien == null)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.GiaBenhVien.NotExists"), dichVuBenhVien.TenDichVu));
                        }

                        var lstPhongId = new List<long>();
                        //var lstNoiThucHienDVKBS = noiThucHienDVKBs.Where(x => x.Id == dichVuBenhVien.DichVuBenhVienId).SelectMany(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ToList();
                        var lstNoiThucHienDVKBS = noiThucHienDVKBs.Where(x => x.DichVuKhamBenhBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                        lstPhongId.AddRange(lstNoiThucHienDVKBS.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList());
                        lstPhongId.AddRange(lstNoiThucHienDVKBS.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong).SelectMany(x => x.PhongBenhViens).Select(x => x.Id).ToList());
                        lstPhongId.Sort();
                        if (!lstPhongId.Any())
                        {
                            lstPhongId.Add(phongHienTaiId);
                        }
                        var phongThucHienId = lstPhongId.First();

                        var bacSiDangKyDVKB = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == phongThucHienId);
                        var entityYeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh()
                        {
                            YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                            YeuCauKhamBenhTruocId = yeuCauVo.YeuCauKhamBenhId == 0 ? (long?)null : yeuCauVo.YeuCauKhamBenhId,
                            DichVuKhamBenhBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                            MaDichVu = dichVuBenhVien.MaDichVu,
                            TenDichVu = dichVuBenhVien.TenDichVu,
                            MaDichVuTT37 = dichVuBenhVien.MaGiaDichVu,
                            NhomGiaDichVuKhamBenhBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                            Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                            DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                            DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem 
                                               && ((yeuCauKhamBenh == null && (yeuCauVo.LaPhauThuatThuThuat != true || duocHuongBaoHiem)) || (yeuCauKhamBenh != null && yeuCauKhamBenh.DuocHuongBaoHiem)),
                            TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,

                            TrangThai = EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                            BaoHiemChiTra = null,

                            NhanVienChiDinhId = currentUserId,
                            NoiChiDinhId = phongHienTaiId,
                            ThoiDiemChiDinh = thoiDiemHienTai,

                            BacSiDangKyId = bacSiDangKyDVKB?.NhanVienId,
                            ThoiDiemDangKy = thoiDiemHienTai,
                            NoiDangKyId = phongThucHienId
                        };

                        //BVHD-3575
                        // trường hợp chỉ định trong nội trú
                        if (yeuCauVo.PhieuDieuTriId != null)
                        {
                            var ngayDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == yeuCauVo.PhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;
                            var newThoiDiemDangKy = ngayDieuTri.Date == thoiDiemHienTai.Date ? thoiDiemHienTai : ngayDieuTri;// ngayDieuTri.Date.AddSeconds(thoiDiemHienTai.Hour * 3600 + thoiDiemHienTai.Minute * 60);
                            entityYeuCauKhamBenh.ThoiDiemDangKy = newThoiDiemDangKy;
                            entityYeuCauKhamBenh.LaChiDinhTuNoiTru = true;
                        }

                        yeuCauVo.YeuCauKhamBenhNews.Add(entityYeuCauKhamBenh);

                        //BVHD-3575
                        if (yeuCauVo.PhieuDieuTriId == null)
                        {
                            yeuCauTiepNhan.YeuCauKhamBenhs.Add(entityYeuCauKhamBenh);
                        }

                        break;
                    case EnumNhomGoiDichVu.DichVuKyThuat:
                        if (dichVuBenhVien.DonGiaBenhVien == null)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.GiaBenhVien.NotExists"), dichVuBenhVien.TenDichVu));
                        }
                        var lstPhongDVKTId = new List<long>();
                        //var noiThucHienDVKTUuTiens = noiThucHienDVKTs.SelectMany(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId)
                        //    .OrderByDescending(x => x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung).ThenBy(x => x.CreatedOn).ToList();
                        var noiThucHienDVKTUuTiens = dvktNoiThucHienUuTiens.Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId)
                                .OrderByDescending(x => x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung).ThenBy(x => x.CreatedOn).ToList();
                        if (noiThucHienDVKTUuTiens.Any())
                        {
                            lstPhongDVKTId.Add(noiThucHienDVKTUuTiens.Select(x => x.PhongBenhVienId).First());
                        }
                        else
                        {
                            //var noiThucHienDVKTByIds = noiThucHienDVKTs.SelectMany(x => x.DichVuKyThuatBenhVienNoiThucHiens).Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            var noiThucHienDVKTByIds = noiThucHienDVKTs.Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            lstPhongDVKTId.AddRange(noiThucHienDVKTByIds.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList());
                            lstPhongDVKTId.AddRange(noiThucHienDVKTByIds.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong).SelectMany(x => x.PhongBenhViens).Select(x => x.Id).ToList());
                            lstPhongDVKTId.Sort();
                        }
                        // trường hợp dv chưa có nơi thực hiện
                        if (!lstPhongDVKTId.Any())
                        {
                            long noiThucHienId = 0;
                            var query = new DropDownListRequestModel
                            {
                                ParameterDependencies = "{DichVuId: " + dichVuBenhVien.DichVuBenhVienId + "}",
                                Take = 1
                            };
                            var noiThucHienTheoDichVuKyThuats =
                                await GetPhongThucHienChiDinhKhamOrDichVuKyThuat(query);
                            if (noiThucHienTheoDichVuKyThuats.Any())
                            {
                                noiThucHienId = noiThucHienTheoDichVuKyThuats.First().KeyId;
                            }

                            lstPhongDVKTId.Add(noiThucHienId);
                        }
                        var phongThucHienDVKTId = lstPhongDVKTId.First();
                        var bacSiDangKyDVKT = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == phongThucHienDVKTId);

                        var entityYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
                        {
                            YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                            YeuCauKhamBenhId = yeuCauVo.YeuCauKhamBenhId == 0 ? (long?)null : yeuCauVo.YeuCauKhamBenhId,
                            DichVuKyThuatBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                            NhomDichVuBenhVienId = dichVuBenhVien.NhomDichVuBenhVienId,
                            MaDichVu = dichVuBenhVien.MaDichVu,
                            TenDichVu = dichVuBenhVien.TenDichVu,
                            MaGiaDichVu = dichVuBenhVien.MaGiaDichVu,
                            TenGiaDichVu = dichVuBenhVien.TenGia,
                            Ma4350DichVu = dichVuBenhVien.Ma4350,
                            NhomGiaDichVuKyThuatBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                            Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                            DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                            DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem 
                                               && ((yeuCauKhamBenh == null && (yeuCauVo.LaPhauThuatThuThuat != true || duocHuongBaoHiem)) || (yeuCauKhamBenh != null && yeuCauKhamBenh.DuocHuongBaoHiem)),
                            TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,
                            NoiThucHienId = phongThucHienDVKTId,
                            NhomChiPhi = dichVuBenhVien.NhomChiPhiDichVuKyThuat,
                            SoLan = dichVuBenhVien.SoLuong,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dichVuBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),

                            TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                            BaoHiemChiTra = null,

                            NhanVienChiDinhId = currentUserId,
                            NoiChiDinhId = phongHienTaiId,
                            ThoiDiemChiDinh = thoiDiemHienTai,
                            NhanVienThucHienId = bacSiDangKyDVKT?.NhanVienId,

                            ThoiDiemDangKy = thoiDiemHienTai,
                            TiLeUuDai = null // todo: cần kiểm tra lại
                        };

                        // trường hợp chỉ định trong nội trú
                        if (yeuCauVo.PhieuDieuTriId != null)
                        {
                            var ngayDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == yeuCauVo.PhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;
                            var newThoiDiemDangKy = ngayDieuTri.Date == thoiDiemHienTai.Date ? thoiDiemHienTai : ngayDieuTri;// ngayDieuTri.Date.AddSeconds(thoiDiemHienTai.Hour * 3600 + thoiDiemHienTai.Minute * 60);
                            entityYeuCauDichVuKyThuat.ThoiDiemDangKy = newThoiDiemDangKy;
                            entityYeuCauDichVuKyThuat.NoiTruPhieuDieuTriId = yeuCauVo.PhieuDieuTriId;
                        }

                        yeuCauVo.YeuCauDichVuKyThuatNews.Add(entityYeuCauDichVuKyThuat);
                        yeuCauTiepNhan.YeuCauDichVuKyThuats.Add(entityYeuCauDichVuKyThuat);
                        break;
                    case EnumNhomGoiDichVu.DichVuGiuongBenh:
                        break;
                }
            }
        }

        public async Task<bool> KiemTraDangKyGoiDichVuTheoBenhNhanAsync(long benhNhanId)
        {
            #region get dv trong gói đã dùng
            // todo: có cập nhật bỏ await
            //var dichVuKhamBenhDaChiDinhs = _yeuCauKhamBenhRepository.TableNoTracking
            //    .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
            //                && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
            //                && x.YeuCauGoiDichVuId != null)
            //    .Select(item => new ThongTinSuDungDichVuTronGoiVo()
            //    {
            //        YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
            //        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
            //        SoLan = 1
            //    })
            //    .ToList();
            //var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
            //    .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
            //                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
            //                && x.YeuCauGoiDichVuId != null)
            //    .Select(item => new ThongTinSuDungDichVuTronGoiVo()
            //    {
            //        YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
            //        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
            //        SoLan = item.SoLan
            //    })
            //    .ToList();

            //var dichVuGiuongDaChiDinhs = _yeuCauDichVuGiuongRepository.TableNoTracking
            //    .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
            //                && x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
            //                && x.YeuCauGoiDichVuId != null)
            //    .Select(item => new ThongTinSuDungDichVuTronGoiVo()
            //    {
            //        YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
            //        DichVuBenhVienId = item.DichVuGiuongBenhVienId,
            //        SoLan = 1
            //    })
            //    .ToList();
            #endregion

            //var goiDichVuDaDangKyKhaDung = _yeuCauGoiDichVuRepository.TableNoTracking
            //    .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
            //                && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
            //                && x.DaQuyetToan != true
            //                && (x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
            //                                .Any(a => a.SoLan > dichVuKhamBenhDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuBenhVienId == a.DichVuKhamBenhBenhVienId).Count())
            //                    || x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
            //                        .Any(a => a.SoLan > dichVuKyThuatDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuBenhVienId == a.DichVuKyThuatBenhVienId).Sum(b => b.SoLan))
            //                    || x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
            //                        .Any(a => a.SoLan > dichVuGiuongDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuBenhVienId == a.DichVuGiuongBenhVienId).Count())
            //               )
            //                && x.NgungSuDung != true  // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
            //    )
            //    .Any();


            #region cập nhật xử lý kiểm tra gói dịch vụ
            var goiDichVuDaDangKyKhaDung = false;
            var yeCauGois = _yeuCauGoiDichVuRepository.TableNoTracking
                                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuDichKhamBenhs)
                                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuDichVuKyThuats)
                                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuDichVuGiuongs)
                                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                                            && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                                            && x.DaQuyetToan != true
                                            && x.NgungSuDung != true
                                ).ToList();
            if (yeCauGois.Any())
            {
                #region kiểm tra số lượng dv
                var dichVuKhamBenhDaChiDinhs = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                                && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.YeuCauGoiDichVuId != null)
                    .Select(item => new ThongTinSuDungDichVuTronGoiVo()
                    {
                        YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        SoLan = 1
                    })
                    .ToList();
                #endregion

                if (dichVuKhamBenhDaChiDinhs.Any())
                {
                    goiDichVuDaDangKyKhaDung = yeCauGois
                        .Any(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
                                    .Any(a => a.SoLan > dichVuKhamBenhDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuBenhVienId == a.DichVuKhamBenhBenhVienId).Count()));
                }
                else
                {
                    goiDichVuDaDangKyKhaDung = yeCauGois
                        .Any(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any());
                }

                if (!goiDichVuDaDangKyKhaDung)
                {
                    #region kiểm tra số lượng dv
                    var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                                    && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && x.YeuCauGoiDichVuId != null)
                        .Select(item => new ThongTinSuDungDichVuTronGoiVo()
                        {
                            YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            SoLan = item.SoLan
                        })
                        .ToList();
                    #endregion

                    if (dichVuKyThuatDaChiDinhs.Any())
                    {
                        goiDichVuDaDangKyKhaDung = yeCauGois
                            .Any(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                                        .Any(a => a.SoLan > dichVuKyThuatDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuBenhVienId == a.DichVuKyThuatBenhVienId).Sum(b => b.SoLan)));
                    }
                    else
                    {
                        goiDichVuDaDangKyKhaDung = yeCauGois
                               .Any(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any());

                    }

                    if (!goiDichVuDaDangKyKhaDung)
                    {
                        #region kiểm tra số lượng dv
                        var dichVuGiuongDaChiDinhs = _yeuCauDichVuGiuongRepository.TableNoTracking
                            .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                                        && x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                        && x.YeuCauGoiDichVuId != null)
                            .Select(item => new ThongTinSuDungDichVuTronGoiVo()
                            {
                                YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                                DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                                SoLan = 1
                            })
                            .ToList();
                        #endregion

                        if (dichVuGiuongDaChiDinhs.Any())
                        {
                            goiDichVuDaDangKyKhaDung = yeCauGois
                                .Any(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                                            .Any(a => a.SoLan > dichVuGiuongDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id && b.DichVuBenhVienId == a.DichVuGiuongBenhVienId).Count()));
                        }
                        else
                        {
                            goiDichVuDaDangKyKhaDung = yeCauGois
                                .Any(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any());
                        }
                    }
                }
            }
            #endregion

            return goiDichVuDaDangKyKhaDung;
        }

        public async Task<List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>> KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync(long yeuCauTiepNhanId, List<string> lstGoiDichVuId, long? noiTruPhieuDieuTriId = null)
        {
            var lstDichVuCanhBao = new List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>();
            var lstDichVuDaChon = new List<ChiTietGoiDichVuChiDinhTheoBenhNhanVo>();
            foreach (var dichVu in lstGoiDichVuId)
            {
                var dichVuObj = JsonConvert.DeserializeObject<ChiTietGoiDichVuChiDinhTheoBenhNhanVo>(dichVu);
                lstDichVuDaChon.Add(dichVuObj);
            }

            var lstTenDichVuBiTrung = new List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>();
            var lstDichVuTrungDaChon = lstDichVuDaChon
                .GroupBy(x => new { x.DichVuBenhVienId, x.NhomGoiDichVu })
                .Select(item => new ChiTietGoiDichVuChiDinhTheoBenhNhanVo()
                {
                    DichVuBenhVienId = item.First().DichVuBenhVienId,
                    NhomGoiDichVu = item.First().NhomGoiDichVu,
                    TenDichVu = item.First().TenDichVu,
                    SoLuongSuDung = item.Count()
                }).Where(x => x.SoLuongSuDung > 1)
                .ToList();

            #region BVHD-3575
            DateTime? ngayDieuTri = null;
            long? tiepNhanNgoaiTruId = null;
            if (noiTruPhieuDieuTriId != null)
            {
                var phieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.YeuCauTiepNhan)
                    .First(x => x.Id == noiTruPhieuDieuTriId);
                ngayDieuTri = phieuDieuTri?.NgayDieuTri.Date;
                tiepNhanNgoaiTruId = phieuDieuTri?.NoiTruBenhAn?.YeuCauTiepNhan?.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
            }


            #endregion

            #region //Cập nhật 02/12/2022
            var lstDichVuKhamIdDaChon = lstDichVuDaChon.Where(x => x.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.DichVuBenhVienId).Distinct().ToList();
            var lstDichVuKyThuatIdDaChon = lstDichVuDaChon.Where(x => x.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.DichVuBenhVienId).Distinct().ToList();
            var lstDichVuGiuongIdDaChon = lstDichVuDaChon.Where(x => x.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh).Select(x => x.DichVuBenhVienId).Distinct().ToList();

            var lstDichVuTrungDichVuDaThem = new List<ChiTietNhomGoiDichVuThuongDungDangChonVo>();
            if(lstDichVuKyThuatIdDaChon.Any())
            {
                var kts = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                && x.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId

                                //Cập nhật 02/12/2022
                                //&& lstDichVuDaChon.Any(a => a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat
                                //                                   && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId)
                                && lstDichVuKyThuatIdDaChon.Contains(x.DichVuKyThuatBenhVienId)

                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                    {
                        NhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuId = item.DichVuKyThuatBenhVienId,
                        TenDichVu = item.TenDichVu
                    }).ToList();
                if(kts.Any())
                {
                    lstDichVuTrungDichVuDaThem.AddRange(kts);
                }
            }

            if (lstDichVuKhamIdDaChon.Any())
            {
                var khams = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(x => ((noiTruPhieuDieuTriId == null && x.YeuCauTiepNhanId == yeuCauTiepNhanId)

                                         //BVHD-3575: cập nhật cho phép chỉ định dv khám từ nội trú
                                         //|| (noiTruPhieuDieuTriId != null && x.Id == 0)) // nội trú thì chỉ kiểm tra dịch vụ kỹ thuật
                                         || (noiTruPhieuDieuTriId != null
                                             && x.YeuCauTiepNhanId == tiepNhanNgoaiTruId
                                             && x.LaChiDinhTuNoiTru != null && x.LaChiDinhTuNoiTru == true
                                             && x.ThoiDiemDangKy.Date == ngayDieuTri))

                                        //Cập nhật 02/12/2022
                                        //&& lstDichVuDaChon.Any(a => a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh
                                        //                                   && a.DichVuBenhVienId == x.DichVuKhamBenhBenhVienId)
                                        && lstDichVuKhamIdDaChon.Contains(x.DichVuKhamBenhBenhVienId)


                                        && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                            .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                            {
                                NhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                DichVuId = item.DichVuKhamBenhBenhVienId,
                                TenDichVu = item.TenDichVu
                            })
                        .ToList();
                if(khams.Any())
                {
                    lstDichVuTrungDichVuDaThem.AddRange(khams);
                }
            }

            if(lstDichVuGiuongIdDaChon.Any())
            {
                var giuongs = _yeuCauDichVuGiuongRepository.TableNoTracking
                            .Where(x => ((noiTruPhieuDieuTriId == null && x.YeuCauTiepNhanId == yeuCauTiepNhanId) || (noiTruPhieuDieuTriId != null && x.Id == 0)) // nội trú thì chỉ kiểm tra dịch vụ kỹ thuật

                                        //Cập nhật 02/12/2022
                                        //&& lstDichVuDaChon.Any(a => a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh
                                        //                                   && a.DichVuBenhVienId == x.DichVuGiuongBenhVienId)
                                        && lstDichVuGiuongIdDaChon.Contains(x.DichVuGiuongBenhVienId)


                                        && x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy)
                            .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                            {
                                NhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                                DichVuId = item.DichVuGiuongBenhVienId,
                                TenDichVu = item.Ten
                            })
                            .ToList();
                if (giuongs.Any())
                {
                    lstDichVuTrungDichVuDaThem.AddRange(giuongs);
                }
            }

            lstDichVuTrungDichVuDaThem = lstDichVuTrungDichVuDaThem.Distinct().ToList();
            #endregion

            foreach (var dichVu in lstDichVuDaChon)
            {
                var dichVuCanhBao = lstDichVuCanhBao.FirstOrDefault(x => x.YeuCauGoiDichVuId == dichVu.YeuCauGoiDichVuId
                                                                       && x.ChuongTrinhGoiDichVuId == dichVu.ChuongTrinhGoiDichVuId
                                                                       && x.ChuongTrinhGoiDichVuChiTietId == dichVu.ChuongTrinhGoiDichVuChiTietId
                                                                       && x.DichVuId == dichVu.DichVuBenhVienId
                                                                       && (int)x.NhomGoiDichVu == dichVu.NhomGoiDichVu);
                if (dichVuCanhBao == null)
                {
                    dichVuCanhBao = new ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo()
                    {
                        YeuCauGoiDichVuId = dichVu.YeuCauGoiDichVuId,
                        ChuongTrinhGoiDichVuId = dichVu.ChuongTrinhGoiDichVuId,
                        ChuongTrinhGoiDichVuChiTietId = dichVu.ChuongTrinhGoiDichVuChiTietId,
                        DichVuId = dichVu.DichVuBenhVienId,
                        TenDichVu = dichVu.TenDichVu,
                        NhomGoiDichVuValue = dichVu.NhomGoiDichVu,
                        TenGoiDichVu = dichVu.TenGoiDichVu
                    };
                }

                if (lstDichVuTrungDaChon.Any(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId
                                                  && x.NhomGoiDichVu == dichVu.NhomGoiDichVu))
                {
                    dichVuCanhBao.LoaiLois.Add(Enums.LoaiLoiGoiDichVu.Trung.GetDescription());
                    lstDichVuCanhBao.Add(dichVuCanhBao);
                }
                else if (lstDichVuTrungDichVuDaThem.Any(x => x.DichVuId == dichVu.DichVuBenhVienId
                                                  && (int)x.NhomDichVu == dichVu.NhomGoiDichVu))
                {
                    dichVuCanhBao.LoaiLois.Add(Enums.LoaiLoiGoiDichVu.Trung.GetDescription());
                    lstDichVuCanhBao.Add(dichVuCanhBao);
                }
            }

            return lstDichVuCanhBao.Distinct().ToList();
        }
        public async Task KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(ChiDinhGoiDichVuTheoBenhNhanVo yeuCauVo)
        {
            var lstDichVuDaChon = yeuCauVo.DichVus;
            var lstDichVuKhamBenhTrongGoi = new List<DichVuBenhVienTheoGoiDichVuVo>();
            var lstDichVuSoLuongConLaiKhongDu = new List<string>();
            var lstDichVuKhongCoTrongGoi = new List<string>();

            var lstYeuCauGoiDichVuIds = lstDichVuDaChon.Select(a => a.YeuCauGoiDichVuId).ToList();
            var yeuCauGoiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien)//.ThenInclude(t => t.YeuCauKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien)//.ThenInclude(t => t.YeuCauDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(z => z.DichVuGiuongBenhVien)//.ThenInclude(t => t.YeuCauDichVuGiuongBenhViens)
                .Where(x => lstYeuCauGoiDichVuIds.Contains(x.Id))
                .ToList();

            #region BVHD-3575
            var dvKhamDaDungs = _yeuCauKhamBenhRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                                    && x.YeuCauGoiDichVuId != null
                                                                                    && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuKhamBenhBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = 1
                })
                .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();

            var dvKyThuatDaDungs = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                    && x.YeuCauGoiDichVuId != null
                                                                                    && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuKyThuatBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = x.SoLan
                })
                .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();

            var dvGiuongDaDungs = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                                                                             && x.YeuCauGoiDichVuId != null
                                                                                             && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuGiuongBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuGiuongBenhVienId ?? 0,
                    SoLuong = 1
                })
                .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();
            #endregion

            foreach (var yeuCauGoiDichVu in yeuCauGoiDichVus)
            {
                // dịch vụ khám
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKhamBenhBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
                    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,

                        //BVHD-3575
                        //SoLanDaSuDung = item.DichVuKhamBenhBenhVien.YeuCauKhamBenhs.Count(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                        //                                                                     && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id),
                        SoLanDaSuDung = dvKhamDaDungs.Where(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                 && a.DichVuBenhVienId == item.DichVuKhamBenhBenhVienId
                                                                 && a.NhomGiaId == item.NhomGiaDichVuKhamBenhBenhVienId)
                            .Sum(a => a.SoLuong),

                        SoLanTheoGoi = item.SoLan
                    }));

                // dịch vụ kỹ thuật
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat))
                    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,

                        //BVHD-3575
                        //SoLanDaSuDung = item.DichVuKyThuatBenhVien.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                        //                                                                     && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id).Sum(a => a.SoLan),
                        SoLanDaSuDung = dvKyThuatDaDungs.Where(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                 && a.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                                 && a.NhomGiaId == item.NhomGiaDichVuKyThuatBenhVienId)
                            .Sum(a => a.SoLuong),

                        SoLanTheoGoi = item.SoLan
                    }));

                // dịch vụ giường
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuGiuongBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh))
                    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                        DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                        TenDichVu = item.DichVuGiuongBenhVien.Ten,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,

                        //BVHD-3575
                        //SoLanDaSuDung = item.DichVuGiuongBenhVien.YeuCauDichVuGiuongBenhViens.Count(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                        //                                                                         && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id),
                        SoLanDaSuDung = dvGiuongDaDungs.Count(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                    && a.DichVuBenhVienId == item.DichVuGiuongBenhVienId
                                                                    && a.NhomGiaId == item.NhomGiaDichVuGiuongBenhVienId),

                        SoLanTheoGoi = item.SoLan
                    }));
            }

            foreach (var dichVuDaChon in lstDichVuDaChon)
            {
                var dichVuTrongGoi = lstDichVuKhamBenhTrongGoi.FirstOrDefault(x => x.YeuCauGoiDichVuId == dichVuDaChon.YeuCauGoiDichVuId
                                                                                   && x.DichVuBenhVienId == dichVuDaChon.DichVuBenhVienId);
                if (dichVuTrongGoi == null)
                {
                    lstDichVuKhongCoTrongGoi.Add(dichVuDaChon.TenDichVu);
                }
                else if (dichVuTrongGoi.SoLanConLai < dichVuDaChon.SoLuongSuDung)
                {
                    lstDichVuSoLuongConLaiKhongDu.Add(dichVuDaChon.TenDichVu);
                }
            }

            if (lstDichVuKhongCoTrongGoi.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.NotExists"), lstDichVuKhongCoTrongGoi.Join(",")));
            }
            if (lstDichVuSoLuongConLaiKhongDu.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVuSoLuongConLai.NotEnough"), lstDichVuSoLuongConLaiKhongDu.Join(",")));
            }

            var lstGoiDaQuyetToan = yeuCauGoiDichVus.Where(x => x.DaQuyetToan == true).ToList();
            if (lstGoiDaQuyetToan.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.DaQuetToan"), lstGoiDaQuyetToan.Select(x => x.TenChuongTrinh).Join(",")));
            }

            var lstGoiDangNgungSuDung = yeuCauGoiDichVus.Where(x => x.NgungSuDung == true).ToList();
            if (lstGoiDangNgungSuDung.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.NgungSuDung"), lstGoiDangNgungSuDung.Select(x => x.TenChuongTrinh).Join(",")));
            }
        }
        public async Task XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(YeuCauTiepNhan yeuCauTiepNhan, ChiDinhGoiDichVuTheoBenhNhanVo yeuCauVo)
        {
            //todo: có cập nhật bỏ await
            var coBHYT = yeuCauTiepNhan.CoBHYT ?? false;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thoiDiemHienTai = DateTime.Now;
            var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
            var lstDichVuDaChon = yeuCauVo.DichVus;

            var lstDichVuKhamBenhTrongGoi = new List<DichVuBenhVienTheoGoiDichVuVo>();

            //có cập nhật: bỏ await
            var lstYeuCauGoiDichVuIds = lstDichVuDaChon.Select(x => x.YeuCauGoiDichVuId).ToList();
            var yeuCauGoiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien).ThenInclude(t => t.DichVuKhamBenh)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien)//.ThenInclude(t => t.YeuCauKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien).ThenInclude(t => t.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.DichVuKyThuat)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien)//.ThenInclude(t => t.YeuCauDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.DichVuKyThuatBenhVienGiaBaoHiems)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(z => z.DichVuGiuongBenhVien).ThenInclude(t => t.DichVuGiuong)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(z => z.DichVuGiuongBenhVien).ThenInclude(t => t.YeuCauDichVuGiuongBenhViens)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(z => z.DichVuGiuongBenhVien).ThenInclude(t => t.DichVuGiuongBenhVienGiaBaoHiems)

                //BVHD-3575
                //.Include(x => x.YeuCauKhamBenhs)
                //.Include(x => x.YeuCauDichVuKyThuats)
                //.Include(x => x.YeuCauDichVuGiuongBenhViens)
                .Where(x => lstYeuCauGoiDichVuIds.Contains(x.Id)) //lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == x.Id))
                .ToList();

            #region BVHD-3575
            var dvKhamDaDungs = _yeuCauKhamBenhRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                                    && x.YeuCauGoiDichVuId != null
                                                                                    && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuKhamBenhBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = 1
                })
                .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();

            var dvKyThuatDaDungs = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                    && x.YeuCauGoiDichVuId != null
                                                                                    && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuKyThuatBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = x.SoLan
                })
                .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();

            //var dvGiuongDaDungs = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
            //                                                                                 && x.YeuCauGoiDichVuId != null
            //                                                                                 && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
            //    .Select(x => new DichVuTrongGoiDaDungVo()
            //    {
            //        YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
            //        DichVuBenhVienId = x.DichVuGiuongBenhVienId,
            //        NhomGiaId = x.NhomGiaDichVuGiuongBenhVienId ?? 0,
            //        SoLuong = 1
            //    })
            //    .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId })
            //    .Select(x => new DichVuTrongGoiDaDungVo()
            //    {
            //        YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
            //        DichVuBenhVienId = x.Key.DichVuBenhVienId,
            //        NhomGiaId = x.Key.NhomGiaId,
            //        SoLuong = x.Sum(a => a.SoLuong)
            //    })
            //    .ToList();
            #endregion

            foreach (var yeuCauGoiDichVu in yeuCauGoiDichVus)
            {
                // dịch vụ khám
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKhamBenhBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                                && !yeuCauVo.DichVuKhongThems.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                       && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                                       && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                                       && a.DichVuId == x.DichVuKhamBenhBenhVienId
                                                                       && a.NhomGoiDichVuValue == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
                    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                        TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                        MaGiaDichVu = item.DichVuKhamBenhBenhVien.DichVuKhamBenh != null ? item.DichVuKhamBenhBenhVien.DichVuKhamBenh.MaTT37 : "",
                        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
                        DonGiaBenhVien = item.DonGia,
                        DonGiaTruocChietKhau = item.DonGiaTruocChietKhau,
                        DonGiaSauChietKhau = item.DonGiaSauChietKhau,
                        DonGiaBaoHiem = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems
                                    .FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                         && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                      && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                        CoBHYT = coBHYT,
                        SoLuong = 1,
                        TiLeBaoHiemThanhToan = item.DichVuKhamBenhBenhVien
                                               .DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                      && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                            item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,

                        //BVHD-3575
                        //SoLanDaSuDung = item.DichVuKhamBenhBenhVien.YeuCauKhamBenhs.Count(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                        //                                                                     && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id),
                        SoLanDaSuDung = dvKhamDaDungs.Where(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                 && a.DichVuBenhVienId == item.DichVuKhamBenhBenhVienId 
                                                                 && a.NhomGiaId == item.NhomGiaDichVuKhamBenhBenhVienId).Sum(a => a.SoLuong),

                        SoLanTheoGoi = item.SoLan,

                        //BVHD-3575
                        NoiDangKyId = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                          && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                                          && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                          && b.DichVuBenhVienId == item.DichVuKhamBenhBenhVienId
                                                                          && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)?.NoiDangKyId,
                    }));

                // dịch vụ kỹ thuật
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                                && !yeuCauVo.DichVuKhongThems.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                        && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                                        && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                                        && a.DichVuId == x.DichVuKyThuatBenhVienId
                                                                        && a.NhomGoiDichVuValue == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat))
                    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                        TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                        MaGiaDichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia : "",
                        TenGia = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.TenGia : "",
                        Ma4350 = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350 : "",
                        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                        DonGiaBenhVien = item.DonGia,
                        DonGiaTruocChietKhau = item.DonGiaTruocChietKhau,
                        DonGiaSauChietKhau = item.DonGiaSauChietKhau,
                        DonGiaBaoHiem = item.DichVuKyThuatBenhVien
                                        .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                    && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                        CoBHYT = coBHYT,
                        SoLuong = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                             && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                             && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                             && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                             && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung,
                        TiLeBaoHiemThanhToan = item.DichVuKyThuatBenhVien
                                               .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                     && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                            item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                            && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                        NhomChiPhiDichVuKyThuat = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                        NhomDichVuBenhVienId = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,

                        //BVHD-3575
                        //SoLanDaSuDung = item.DichVuKyThuatBenhVien.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                        //                                                                     && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id).Sum(a => a.SoLan),
                        SoLanDaSuDung = dvKyThuatDaDungs.Where(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                 && a.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                                 && a.NhomGiaId == item.NhomGiaDichVuKyThuatBenhVienId).Sum(a => a.SoLuong),

                        SoLanTheoGoi = item.SoLan
                    }));

                // dịch vụ giường
                //lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                //    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                //                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                //                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                //                                         && a.DichVuBenhVienId == x.DichVuGiuongBenhVienId
                //                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                //                && !yeuCauVo.DichVuKhongThems.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                //                                                        && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                //                                                        && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                //                                                        && a.DichVuId == x.DichVuGiuongBenhVienId
                //                                                        && a.NhomGoiDichVuValue == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh))
                //    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                //    {
                //        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                //        DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                //        MaDichVu = item.DichVuGiuongBenhVien.Ma,
                //        TenDichVu = item.DichVuGiuongBenhVien.Ten,
                //        MaGiaDichVu = item.DichVuGiuongBenhVien.DichVuGiuong != null ? item.DichVuGiuongBenhVien.DichVuGiuong.MaTT37 : "",
                //        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuGiuongBenhVienId,
                //        DonGiaBenhVien = item.DonGia,
                //        DonGiaBaoHiem = item.DichVuGiuongBenhVien
                //                        .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                             && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                //                    item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                                  && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                //        CoBHYT = coBHYT,
                //        SoLuong = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                //                                             && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                //                                             && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                //                                             && b.DichVuBenhVienId == item.DichVuGiuongBenhVienId
                //                                             && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).SoLuongSuDung,//item.SoLan,
                //        TiLeBaoHiemThanhToan = item.DichVuGiuongBenhVien
                //                               .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                    && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                //                            item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                //        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,

                //        //BVHD-3575
                //        //SoLanDaSuDung = item.DichVuGiuongBenhVien.YeuCauDichVuGiuongBenhViens.Count(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                //        //                                                                         && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id),
                //        SoLanDaSuDung = dvGiuongDaDungs.Where(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                //                                                   && a.DichVuBenhVienId == item.DichVuGiuongBenhVienId
                //                                                   && a.NhomGiaId == item.NhomGiaDichVuGiuongBenhVienId).Sum(a => a.SoLuong),

                //        SoLanTheoGoi = item.SoLan
                //    }));
            }

            if (!lstDichVuKhamBenhTrongGoi.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.DichVu.Required"));
            }

            #region Code lỗi mà tiếc nên ko xóa

            //var lstDichVuKhamBenhTrongGoi = await _yeuCauGoiDichVuRepository.TableNoTracking
            //    .SelectMany(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs)
            //    .Where(x => lstDichVuDaChon.Any(a => a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
            //                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
            //                                         && a.DichVuBenhVienId == x.DichVuKhamBenhBenhVienId
            //                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
            //    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
            //    {
            //        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
            //        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
            //        MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
            //        TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
            //        MaGiaDichVu = item.DichVuKhamBenhBenhVien.DichVuKhamBenh != null ? item.DichVuKhamBenhBenhVien.DichVuKhamBenh.MaTT37 : "",
            //        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
            //        DonGiaBenhVien = item.DonGia,
            //        DonGiaBaoHiem = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems
            //                            .FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                 && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
            //                        item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
            //        CoBHYT = coBHYT,
            //        SoLuong = 1,
            //        TiLeBaoHiemThanhToan = item.DichVuKhamBenhBenhVien
            //                                   .DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
            //                                item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
            //        YeuCauGoiDichVuId = lstDichVuDaChon.First(b => b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
            //                                                       && b.ChuongTrinhGoiDichVuChiTietId == item.Id
            //                                                       && b.DichVuBenhVienId == item.DichVuKhamBenhBenhVienId
            //                                                       && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh).YeuCauGoiDichVuId,
            //        SoLanDaSuDung = item.DichVuKhamBenhBenhVien.YeuCauKhamBenhs.Count(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
            //                                                                             && a.YeuCauGoiDichVuId == lstDichVuDaChon.First(b => b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
            //                                                                                                                                && b.ChuongTrinhGoiDichVuChiTietId == item.Id
            //                                                                                                                                && b.DichVuBenhVienId == item.DichVuKhamBenhBenhVienId
            //                                                                                                                                && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh).YeuCauGoiDichVuId),
            //        SoLanTheoGoi = item.SoLan
            //    })
            //    .Union(
            //        _yeuCauGoiDichVuRepository.TableNoTracking
            //    .SelectMany(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats)
            //    .Where(x => lstDichVuDaChon.Any(a => a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
            //                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
            //                                         && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId
            //                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat))
            //    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
            //    {
            //        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
            //        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
            //        MaDichVu = item.DichVuKyThuatBenhVien.Ma,
            //        TenDichVu = item.DichVuKyThuatBenhVien.Ten,
            //        MaGiaDichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia : "",
            //        TenGia = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.TenGia : "",
            //        Ma4350 = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350 : "",
            //        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
            //        DonGiaBenhVien = item.DonGia,
            //        DonGiaBaoHiem = item.DichVuKyThuatBenhVien
            //                            .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                  && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
            //                        item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                                        && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
            //        CoBHYT = coBHYT,
            //        SoLuong = lstDichVuDaChon.First(b => b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
            //                                             && b.ChuongTrinhGoiDichVuChiTietId == item.Id
            //                                             && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
            //                                             && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung,//item.SoLan,
            //        TiLeBaoHiemThanhToan = item.DichVuKyThuatBenhVien
            //                                   .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                         && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
            //                                item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                                                && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
            //        NhomChiPhiDichVuKyThuat = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
            //        NhomDichVuBenhVienId = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId,
            //        YeuCauGoiDichVuId = lstDichVuDaChon.First(b => b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
            //                                                       && b.ChuongTrinhGoiDichVuChiTietId == item.Id
            //                                                       && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
            //                                                       && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).YeuCauGoiDichVuId,
            //        SoLanDaSuDung = item.DichVuKyThuatBenhVien.YeuCauDichVuKyThuats.Count(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
            //                                                                             && a.YeuCauGoiDichVuId == lstDichVuDaChon.First(b => b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
            //                                                                                                                                  && b.ChuongTrinhGoiDichVuChiTietId == item.Id
            //                                                                                                                                  && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
            //                                                                                                                                  && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).YeuCauGoiDichVuId),
            //        SoLanTheoGoi = item.SoLan
            //    }))
            //    .Union(
            //        _yeuCauGoiDichVuRepository.TableNoTracking.SelectMany(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs)
            //    .Where(x => lstDichVuDaChon.Any(a => a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
            //                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
            //                                         && a.DichVuBenhVienId == x.DichVuGiuongBenhVienId
            //                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh))
            //    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
            //    {
            //        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
            //        DichVuBenhVienId = item.DichVuGiuongBenhVienId,
            //        MaDichVu = item.DichVuGiuongBenhVien.Ma,
            //        TenDichVu = item.DichVuGiuongBenhVien.Ten,
            //        MaGiaDichVu = item.DichVuGiuongBenhVien.DichVuGiuong != null ? item.DichVuGiuongBenhVien.DichVuGiuong.MaTT37 : "",
            //        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuGiuongBenhVienId,
            //        DonGiaBenhVien = item.DonGia,
            //        DonGiaBaoHiem = item.DichVuGiuongBenhVien
            //                            .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                 && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
            //                        item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                                      && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
            //        CoBHYT = coBHYT,
            //        SoLuong = lstDichVuDaChon.First(b => b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
            //                                             && b.ChuongTrinhGoiDichVuChiTietId == item.Id
            //                                             && b.DichVuBenhVienId == item.DichVuGiuongBenhVienId
            //                                             && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).SoLuongSuDung,//item.SoLan,
            //        TiLeBaoHiemThanhToan = item.DichVuGiuongBenhVien
            //                                   .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                        && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
            //                                item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
            //                                                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
            //        YeuCauGoiDichVuId = lstDichVuDaChon.First(b => b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
            //                                                       && b.ChuongTrinhGoiDichVuChiTietId == item.Id
            //                                                       && b.DichVuBenhVienId == item.DichVuGiuongBenhVienId
            //                                                       && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).YeuCauGoiDichVuId,
            //        SoLanDaSuDung = item.DichVuGiuongBenhVien.YeuCauDichVuGiuongBenhViens.Count(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
            //                                                                                 && a.YeuCauGoiDichVuId == lstDichVuDaChon.First(b => b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
            //                                                                                                                                      && b.ChuongTrinhGoiDichVuChiTietId == item.Id
            //                                                                                                                                      && b.DichVuBenhVienId == item.DichVuGiuongBenhVienId
            //                                                                                                                                      && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).YeuCauGoiDichVuId),
            //        SoLanTheoGoi = item.SoLan
            //    }))
            //    .ToListAsync();
            #endregion

            var bacSiDangKys = await _hoatDongNhanVienRepository.TableNoTracking
                            .Where(x => x.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi
                                        && x.NhanVien.User.IsActive).ToListAsync();
            //var noiThucHienDVKBs = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToListAsync();
            //var noiThucHienDVKTs = await _dichVuKyThuatBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToListAsync();
            //var noiThucHienDVGs = await _dichVuGiuongBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToListAsync();

            #region cập nhật 02/12/2022: xử lý get list noi thực hiện theo dịch vụ
            var noiThucHienDVKBs = new List<DichVuKhamBenhBenhVienNoiThucHien>();
            var noiThucHienDVKTs = new List<DichVuKyThuatBenhVienNoiThucHien>();
            var dvktNoiThucHienUuTiens = new List<DichVuKyThuatBenhVienNoiThucHienUuTien>();
            var lstDichVuKhamId = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.DichVuBenhVienId).Distinct().ToList();
            var lstDichVuKyThuatId = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.DichVuBenhVienId).Distinct().ToList();

            if(lstDichVuKhamId.Any())
            {
                noiThucHienDVKBs = _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking
                    .Include(x => x.PhongBenhVien)
                    .Include(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                    .Where(x => lstDichVuKhamId.Contains(x.DichVuKhamBenhBenhVienId))
                    .ToList();
            }

            if (lstDichVuKyThuatId.Any())
            {
                dvktNoiThucHienUuTiens = _dichVuKyThuatBenhVienNoiThucHienUuTienRepository.TableNoTracking
                    .Include(x => x.PhongBenhVien)
                    .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                    .ToList();

                if (!dvktNoiThucHienUuTiens.Any())
                {
                    noiThucHienDVKTs = _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking
                        .Include(x => x.PhongBenhVien)
                        .Include(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                        .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                        .ToList();
                }
            }
            #endregion

            foreach (var dichVuBenhVien in lstDichVuKhamBenhTrongGoi)
            {
                switch (dichVuBenhVien.NhomGoiDichVu)
                {
                    case EnumNhomGoiDichVu.DichVuKhamBenh:
                        if (dichVuBenhVien.SoLanConLai < dichVuBenhVien.SoLuong)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongConLai.NotEnough"), dichVuBenhVien.TenDichVu));
                        }

                        var lstPhongId = new List<long>();
                        //var lstNoiThucHienDVKBS = noiThucHienDVKBs.Where(x => x.Id == dichVuBenhVien.DichVuBenhVienId).SelectMany(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ToList();
                        var lstNoiThucHienDVKBS = noiThucHienDVKBs.Where(x => x.DichVuKhamBenhBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                        lstPhongId.AddRange(lstNoiThucHienDVKBS.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList());
                        lstPhongId.AddRange(lstNoiThucHienDVKBS.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong).SelectMany(x => x.PhongBenhViens).Select(x => x.Id).ToList());
                        lstPhongId.Sort();
                        var phongThucHienId = lstPhongId.Any() ? lstPhongId.First() : phongHienTaiId;

                        var bacSiDangKyDVKB = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == phongThucHienId);
                        var entityYeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh()
                        {
                            YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                            YeuCauKhamBenhTruocId = yeuCauVo.YeuCauKhamBenhId,
                            DichVuKhamBenhBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                            MaDichVu = dichVuBenhVien.MaDichVu,
                            TenDichVu = dichVuBenhVien.TenDichVu,
                            MaDichVuTT37 = dichVuBenhVien.MaGiaDichVu,
                            NhomGiaDichVuKhamBenhBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                            Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                            DonGiaTruocChietKhau = dichVuBenhVien.DonGiaTruocChietKhau,
                            DonGiaSauChietKhau = dichVuBenhVien.DonGiaSauChietKhau,
                            DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                            DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem,
                            TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,

                            TrangThai = EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                            BaoHiemChiTra = null,

                            NhanVienChiDinhId = currentUserId,
                            NoiChiDinhId = phongHienTaiId,
                            ThoiDiemChiDinh = thoiDiemHienTai,

                            BacSiDangKyId = bacSiDangKyDVKB?.NhanVienId,
                            ThoiDiemDangKy = thoiDiemHienTai,
                            NoiDangKyId = dichVuBenhVien.NoiDangKyId ?? phongThucHienId,

                            YeuCauGoiDichVuId = dichVuBenhVien.YeuCauGoiDichVuId
                        };

                        // trường hợp chỉ định trong nội trú
                        if (yeuCauVo.NoiTruPhieuDieuTriId != null)
                        {
                            var ngayDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == yeuCauVo.NoiTruPhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;
                            var newThoiDiemDangKy = yeuCauVo.NgayDieuTri ?? (ngayDieuTri.Date == thoiDiemHienTai.Date ? thoiDiemHienTai : ngayDieuTri); //ngayDieuTri.Date.AddSeconds(thoiDiemHienTai.Hour * 3600 + thoiDiemHienTai.Minute * 60);
                            entityYeuCauKhamBenh.ThoiDiemDangKy = newThoiDiemDangKy;
                            entityYeuCauKhamBenh.LaChiDinhTuNoiTru = true;
                        }

                        yeuCauVo.YeuCauKhamBenhNews.Add(entityYeuCauKhamBenh);

                        //BVHD-3575: trường hợp chỉ định dv khám từ nội trú sẽ xử lý lưu vào YeucauTiepNhan ngoại trú ở chỗ khác
                        if (yeuCauVo.NoiTruPhieuDieuTriId == null)
                        {
                            yeuCauTiepNhan.YeuCauKhamBenhs.Add(entityYeuCauKhamBenh);
                        }
                        break;
                    case EnumNhomGoiDichVu.DichVuKyThuat:
                        if (dichVuBenhVien.SoLanConLai < dichVuBenhVien.SoLuong)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongConLai.NotEnough"), dichVuBenhVien.TenDichVu));
                        }
                        var lstPhongDVKTId = new List<long>();
                        //var noiThucHienDVKTUuTiens = noiThucHienDVKTs.SelectMany(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId)
                        //    .OrderByDescending(x => x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung).ThenBy(x => x.CreatedOn).ToList();
                        var noiThucHienDVKTUuTiens = dvktNoiThucHienUuTiens.Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId)
                                    .OrderByDescending(x => x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung).ThenBy(x => x.CreatedOn).ToList();
                        if (noiThucHienDVKTUuTiens.Any())
                        {
                            lstPhongDVKTId.Add(noiThucHienDVKTUuTiens.Select(x => x.PhongBenhVienId).First());
                        }
                        else
                        {
                            //var noiThucHienDVKTByIds = noiThucHienDVKTs.SelectMany(x => x.DichVuKyThuatBenhVienNoiThucHiens).Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            var noiThucHienDVKTByIds = noiThucHienDVKTs.Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            lstPhongDVKTId.AddRange(noiThucHienDVKTByIds.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList());
                            lstPhongDVKTId.AddRange(noiThucHienDVKTByIds.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong).SelectMany(x => x.PhongBenhViens).Select(x => x.Id).ToList());
                            lstPhongDVKTId.Sort();
                        }

                        // trường hợp dv chưa có nơi thực hiện
                        if (!lstPhongDVKTId.Any())
                        {
                            long noiThucHienId = 0;
                            var query = new DropDownListRequestModel
                            {
                                ParameterDependencies = "{DichVuId: " + dichVuBenhVien.DichVuBenhVienId + "}",
                                Take = 1
                            };
                            var noiThucHienTheoDichVuKyThuats =
                                await GetPhongThucHienChiDinhKhamOrDichVuKyThuat(query);
                            if (noiThucHienTheoDichVuKyThuats.Any())
                            {
                                noiThucHienId = noiThucHienTheoDichVuKyThuats.First().KeyId;
                            }

                            lstPhongDVKTId.Add(noiThucHienId);
                        }
                        var phongThucHienDVKTId = lstPhongDVKTId.First();
                        var bacSiDangKyDVKT = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == phongThucHienDVKTId);

                        var entityYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
                        {
                            YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                            YeuCauKhamBenhId = yeuCauVo.YeuCauKhamBenhId,
                            DichVuKyThuatBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                            NhomDichVuBenhVienId = dichVuBenhVien.NhomDichVuBenhVienId,
                            MaDichVu = dichVuBenhVien.MaDichVu,
                            TenDichVu = dichVuBenhVien.TenDichVu,
                            MaGiaDichVu = dichVuBenhVien.MaGiaDichVu,
                            TenGiaDichVu = dichVuBenhVien.TenGia,
                            Ma4350DichVu = dichVuBenhVien.Ma4350,
                            NhomGiaDichVuKyThuatBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                            Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                            DonGiaTruocChietKhau = dichVuBenhVien.DonGiaTruocChietKhau,
                            DonGiaSauChietKhau = dichVuBenhVien.DonGiaSauChietKhau,
                            DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                            DuocHuongBaoHiem = yeuCauVo.ISPTTT ? false : dichVuBenhVien.DuocHuongBaoHiem,
                            TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,
                            NoiThucHienId = phongThucHienDVKTId,
                            NhomChiPhi = dichVuBenhVien.NhomChiPhiDichVuKyThuat,
                            SoLan = dichVuBenhVien.SoLuong,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dichVuBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),

                            TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                            BaoHiemChiTra = null,

                            NhanVienChiDinhId = currentUserId,
                            NoiChiDinhId = phongHienTaiId,
                            ThoiDiemChiDinh = thoiDiemHienTai,
                            NhanVienThucHienId = bacSiDangKyDVKT?.NhanVienId,

                            ThoiDiemDangKy = thoiDiemHienTai,
                            TiLeUuDai = null, // todo: cần kiểm tra lại

                            YeuCauGoiDichVuId = dichVuBenhVien.YeuCauGoiDichVuId,
                        };

                        // trường hợp chỉ định trong nội trú
                        if (yeuCauVo.NoiTruPhieuDieuTriId != null)
                        {
                            var ngayDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == yeuCauVo.NoiTruPhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;
                            var newThoiDiemDangKy = ngayDieuTri.Date == thoiDiemHienTai.Date ? thoiDiemHienTai : ngayDieuTri; //ngayDieuTri.Date.AddSeconds(thoiDiemHienTai.Hour * 3600 + thoiDiemHienTai.Minute * 60);
                            entityYeuCauDichVuKyThuat.ThoiDiemDangKy = newThoiDiemDangKy;
                            entityYeuCauDichVuKyThuat.NoiTruPhieuDieuTriId = yeuCauVo.NoiTruPhieuDieuTriId;
                        }
                        yeuCauVo.YeuCauDichVuKyThuatNews.Add(entityYeuCauDichVuKyThuat);
                        yeuCauTiepNhan.YeuCauDichVuKyThuats.Add(entityYeuCauDichVuKyThuat);
                        break;
                        //case EnumNhomGoiDichVu.DichVuGiuongBenh:
                        //    if (dichVuBenhVien.SoLanConLai < dichVuBenhVien.SoLuong)
                        //    {
                        //        throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongConLai.NotEnough"), dichVuBenhVien.TenDichVu));
                        //    }

                        //    var entityYeuCauGiuongBenh = new YeuCauDichVuGiuongBenhVien()
                        //    {
                        //        YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                        //        YeuCauKhamBenhId = yeuCauVo.YeuCauKhamBenhId,
                        //        DichVuGiuongBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                        //        Ma = dichVuBenhVien.MaDichVu,
                        //        Ten = dichVuBenhVien.TenDichVu,
                        //        MaTT37 = dichVuBenhVien.MaGiaDichVu,
                        //        NhomGiaDichVuGiuongBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                        //        Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                        //        DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                        //        DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem,
                        //        BaoHiemChiTra = null,
                        //        TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,
                        //        LoaiGiuong = dichVuBenhVien.LoaiGiuong.Value,

                        //        TrangThai = EnumTrangThaiGiuongBenh.ChuaThucHien,
                        //        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,

                        //        NhanVienChiDinhId = currentUserId,
                        //        NoiChiDinhId = phongHienTaiId,
                        //        ThoiDiemChiDinh = thoiDiemHienTai,

                        //        YeuCauGoiDichVuId = dichVuBenhVien.YeuCauGoiDichVuId
                        //    };

                        //    yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Add(entityYeuCauGiuongBenh);
                        //    yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Add(entityYeuCauGiuongBenh);
                        //    break;
                }
            }

            #region kiểm tra có gói nào chỉ định dịch vụ vượt quá số tiền bảo lãnh còn lại

            var lstTongChiTheoGoiDichVu = lstDichVuKhamBenhTrongGoi
                .GroupBy(x => new {x.YeuCauGoiDichVuId})
                .Select(x => new {x.First().YeuCauGoiDichVuId, Sum = x.Sum(y => (y.DonGiaSauChietKhau ?? 0) * y.SoLuong)})
                .ToList();

            #region BVHD-3575
            var lstGoiId = lstTongChiTheoGoiDichVu.Select(x => x.YeuCauGoiDichVuId).Distinct().ToList();
            var chiPhiDVKhamDaDungs = _yeuCauKhamBenhRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                                    && x.YeuCauGoiDichVuId != null
                                                                                    && lstGoiId.Contains(x.YeuCauGoiDichVuId.Value)
                                                                                    && x.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan)
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuKhamBenhBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = 1,
                    DonGiaSauChietKhau = x.DonGiaSauChietKhau
                })
                .ToList();

            var chiPhiDVKyThuatDaDungs = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                    && x.YeuCauGoiDichVuId != null
                                                                                    && lstGoiId.Contains(x.YeuCauGoiDichVuId.Value)
                                                                                    && x.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan)
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuKyThuatBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = x.SoLan,
                    DonGiaSauChietKhau = x.DonGiaSauChietKhau
                })
                .ToList();

            var chiPhiDVGiuongDaDungs = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                                                                                   && x.YeuCauGoiDichVuId != null
                                                                                                   && lstGoiId.Contains(x.YeuCauGoiDichVuId.Value)
                                                                                                   && x.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan)
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuGiuongBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuGiuongBenhVienId ?? 0,
                    SoLuong = 1,
                    DonGiaSauChietKhau = x.DonGiaSauChietKhau
                })
                .ToList();
            #endregion

            foreach (var goi in lstTongChiTheoGoiDichVu)
            {
                var yeuCauGoiDichVu = yeuCauGoiDichVus.Where(x => x.Id == goi.YeuCauGoiDichVuId).First();
                //var tongDaChis = yeuCauGoiDichVu.YeuCauKhamBenhs.Where(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                //                                                           && a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                //                                                           && a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.DonGiaSauChietKhau ?? 0)
                //              + yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                //                                                     && a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                //                                                     && a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0))
                //              + yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                //                                                            && a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                //                                                            && a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.DonGiaSauChietKhau ?? 0);

                //BVHD-3575
                var tongDaChi = chiPhiDVKhamDaDungs.Where(a => a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.ThanhTienSauChietKhau ?? 0)
                                + chiPhiDVKyThuatDaDungs.Where(a => a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.ThanhTienSauChietKhau ?? 0)
                                + chiPhiDVGiuongDaDungs.Where(a => a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.ThanhTienSauChietKhau ?? 0);

                var soTienBaoLanhConLai = (yeuCauGoiDichVu.SoTienBenhNhanDaChi ?? 0) - tongDaChi;
                if (soTienBaoLanhConLai < goi.Sum)
                {
                    yeuCauVo.IsVuotQuaBaoLanhGoi = true;
                    break;
                }
            }


            #endregion

            if (yeuCauVo.YeuCauKhamBenhId != null)
            {
                var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauVo.YeuCauKhamBenhId
                                                                               && x.YeuCauTiepNhanId == yeuCauVo.YeuCauTiepNhanId
                                                                               && x.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham);
                if (yeuCauKhamBenh != null)
                {
                    yeuCauKhamBenh.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                    yeuCauKhamBenh.NoiThucHienId = yeuCauKhamBenh.NoiDangKyId; // _userAgentHelper.GetCurrentNoiLLamViecId();
                    yeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                    yeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;

                    YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                    {
                        TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                        MoTa = yeuCauKhamBenh.TrangThai.GetDescription()
                    };
                    yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
                }
            }
        }

        public async Task KiemTraDichVuChiDinhCoTrongGoiCuaBenhNhanAsync(DichVuChiDinhCoTrongGoiCuaBenhNhanVo dichVuChiDinhCoTrongGoiCuaBenhNhanVo)
        {
            // todo: có cập nhật bỏ await
            var yeuCauGoiTheoBenhNhans = _yeuCauGoiDichVuRepository.Table
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien)//.ThenInclude(a => a.YeuCauKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien)//.ThenInclude(a => a.YeuCauDichVuKyThuats)
                .Where(x => ((x.BenhNhanId == dichVuChiDinhCoTrongGoiCuaBenhNhanVo.BenhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == dichVuChiDinhCoTrongGoiCuaBenhNhanVo.BenhNhanId && x.GoiSoSinh == true))//x.BenhNhanId == dichVuChiDinhCoTrongGoiCuaBenhNhanVo.BenhNhanId
                            && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && (x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(a => dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuKhamBenhIds.Any(b => b == a.DichVuKhamBenhBenhVienId))
                                || x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(a => dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuKyThuatIds.Any(b => b == a.DichVuKyThuatBenhVienId))))
                .OrderBy(x => x.CreatedOn)
                .ToList();

            //BVHD-3575
            var yeuCauGoiIds = yeuCauGoiTheoBenhNhans.Select(x => x.Id).Distinct().ToList();
            var dvKhamDaDungs = _yeuCauKhamBenhRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham 
                                                                                    && x.YeuCauGoiDichVuId != null 
                                                                                    && yeuCauGoiIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuKhamBenhBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = 1
                })
                .GroupBy(x => new {x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();

            var dvKyThuatDaDungs = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                    && x.YeuCauGoiDichVuId != null
                                                                                    && yeuCauGoiIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.DichVuKyThuatBenhVienId,
                    NhomGiaId = x.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = x.SoLan
                })
                .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();

            foreach (var yeuCauGoi in yeuCauGoiTheoBenhNhans)
            {
                if (dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuKhamBenhIds.Any())
                {
                    foreach (var dichVuKhamBenh in yeuCauGoi.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs)
                    {
                        if (dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuKhamBenhIds.Any(x => x == dichVuKhamBenh.DichVuKhamBenhBenhVienId))
                        {
                            var newItem = new ChiTietGoiDichVuTheoBenhNhanGridVo()
                            {
                                YeuCauGoiDichVuId = yeuCauGoi.Id,
                                TenGoiDichVu = yeuCauGoi.ChuongTrinhGoiDichVu.Ten + " - " + yeuCauGoi.ChuongTrinhGoiDichVu.TenGoiDichVu,
                                ChuongTrinhGoiDichVuId = yeuCauGoi.ChuongTrinhGoiDichVuId,
                                ChuongTrinhGoiDichVuChiTietId = dichVuKhamBenh.Id,
                                DichVuBenhVienId = dichVuKhamBenh.DichVuKhamBenhBenhVienId,
                                MaDichVu = dichVuKhamBenh.DichVuKhamBenhBenhVien.Ma,
                                TenDichVu = dichVuKhamBenh.DichVuKhamBenhBenhVien.Ten,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                //TenLoaiGia = dichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVien.Ten,
                                SoLuong = dichVuKhamBenh.SoLan,
                                DonGia = dichVuKhamBenh.DonGia,

                                //BVHD-3575
                                //SoLuongDaDung = dichVuKhamBenh.DichVuKhamBenhBenhVien.YeuCauKhamBenhs.Count(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && a.YeuCauGoiDichVuId == yeuCauGoi.Id),
                                SoLuongDaDung = dvKhamDaDungs.Where(a => a.YeuCauGoiDichVuId == yeuCauGoi.Id 
                                                                         && a.DichVuBenhVienId == dichVuKhamBenh.DichVuKhamBenhBenhVienId
                                                                         && a.NhomGiaId == dichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVienId).Sum(a => a.SoLuong),
                            };
                            if ((newItem.SoLuong > newItem.SoLuongDaDung) && !dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuChiDinhCoTrongGois.Any(x => x.DichVuBenhVienId == newItem.DichVuBenhVienId && x.NhomGoiDichVu == newItem.NhomGoiDichVu))
                            {
                                dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuChiDinhCoTrongGois.Add(newItem);
                            }
                        }
                    }
                }
                if (dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuKyThuatIds.Any())
                {
                    foreach (var dichVuKyThuat in yeuCauGoi.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats)
                    {
                        if (dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuKyThuatIds.Any(x => x == dichVuKyThuat.DichVuKyThuatBenhVienId))
                        {
                            var newItem = new ChiTietGoiDichVuTheoBenhNhanGridVo()
                            {
                                YeuCauGoiDichVuId = yeuCauGoi.Id,
                                TenGoiDichVu = yeuCauGoi.ChuongTrinhGoiDichVu.Ten + " - " + yeuCauGoi.ChuongTrinhGoiDichVu.TenGoiDichVu,
                                ChuongTrinhGoiDichVuId = yeuCauGoi.ChuongTrinhGoiDichVuId,
                                ChuongTrinhGoiDichVuChiTietId = dichVuKyThuat.Id,
                                DichVuBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId,
                                MaDichVu = dichVuKyThuat.DichVuKyThuatBenhVien.Ma,
                                TenDichVu = dichVuKyThuat.DichVuKyThuatBenhVien.Ten,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                //TenLoaiGia = dichVuKyThuat.NhomGiaDichVuKyThuatBenhVien.Ten,
                                SoLuong = dichVuKyThuat.SoLan,
                                DonGia = dichVuKyThuat.DonGia,

                                //BVHD-3575
                                //SoLuongDaDung = dichVuKyThuat.DichVuKyThuatBenhVien.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && a.YeuCauGoiDichVuId == yeuCauGoi.Id).Sum(a => a.SoLan),
                                SoLuongDaDung = dvKyThuatDaDungs.Where(a => a.YeuCauGoiDichVuId == yeuCauGoi.Id 
                                                                            && a.DichVuBenhVienId == dichVuKyThuat.DichVuKyThuatBenhVienId
                                                                            && a.NhomGiaId == dichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId).Sum(a => a.SoLuong),
                            };
                            if ((newItem.SoLuong > newItem.SoLuongDaDung) && !dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuChiDinhCoTrongGois.Any(x => x.DichVuBenhVienId == newItem.DichVuBenhVienId && x.NhomGoiDichVu == newItem.NhomGoiDichVu))
                            {
                                dichVuChiDinhCoTrongGoiCuaBenhNhanVo.DichVuChiDinhCoTrongGois.Add(newItem);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public async Task<List<string>> GetGhiChuDichVuCanLamSangsAsync(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored.Value)
            };
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") ||
                string.IsNullOrEmpty(queryInfo.Query))
            {
                var lstValues = _inputStringStoredRepository.TableNoTracking
                    .Where(p => p.Set == Enums.InputStringStoredKey.GhiChuCanLamSang)
                    .Select(p => p.Value)
                    .ApplyLike(queryInfo.Query, o => o)
                    .Take(queryInfo.Take);

                return await lstValues.ToListAsync();
            }
            else
            {
                var lstIds = _inputStringStoredRepository
                    .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored),
                        lstColumnNameSearch)
                    .Select(p => p.Id).ToList();

                var dictionary = lstIds.Select((id, index) => new
                {
                    keys = id,
                    rank = index,
                }).ToDictionary(o => o.keys, o => o.rank);

                var lstValues = await _inputStringStoredRepository
                    .TableNoTracking
                    .Where(p => p.Set == Enums.InputStringStoredKey.GhiChuCanLamSang)
                    .Take(queryInfo.Take)
                    .Select(item => new InputStringStoredTemplateVo
                    {
                        Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                        DisplayName = item.Value,
                        KeyId = item.Id,
                    }).ToListAsync();
                var listValueStrings = lstValues.Select(p => p.DisplayName).ToList();
                return listValueStrings;
            }
        }

        public async Task CapNhatGhiChuCanLamSangAsync(UpdateGhiChuCanLamSangVo updateVo)
        {
            var yeuCauKhamBenh =
                await _yeuCauKhamBenhRepository.Table.FirstOrDefaultAsync(x => x.Id == updateVo.YeuCauKhamBenhId);
            if (yeuCauKhamBenh == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            yeuCauKhamBenh.GhiChuCanLamSang = updateVo.GhiChuCanLamSang;
            await _yeuCauKhamBenhRepository.Context.SaveChangesAsync();


            // xử lý lưu inputs
            if (!string.IsNullOrEmpty(updateVo.GhiChuCanLamSang))
            {
                var isExists = await _inputStringStoredRepository
                    .TableNoTracking
                    .AnyAsync(p => p.Value.Trim().ToLower() == updateVo.GhiChuCanLamSang.Trim().ToLower()
                                   && p.Set == Enums.InputStringStoredKey.GhiChuCanLamSang);
                if (!isExists)
                {
                    var inputStringStored = new Core.Domain.Entities.InputStringStoreds.InputStringStored
                    {
                        Set = Enums.InputStringStoredKey.GhiChuCanLamSang,
                        Value = updateVo.GhiChuCanLamSang
                    };
                    await _inputStringStoredRepository.AddAsync(inputStringStored);
                }
            }
        }

        public async Task GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(ThongTinDichVuTrongGoi thongTinChiDinhVo)
        {
            var yeuCauGoiDichVus = await _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs)
                //.Include(x => x.YeuCauKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
                //.Include(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
                //.Include(x => x.YeuCauDichVuGiuongBenhViens)
                //.Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .Where(x => x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && ((x.BenhNhanId == thongTinChiDinhVo.BenhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == thongTinChiDinhVo.BenhNhanId && x.GoiSoSinh == true))
                            && x.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                            )
                .ToListAsync();
            if (!yeuCauGoiDichVus.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDinh.GoiDichVu.NotExists"));
            }

            var yeuCauGoiDichVuTheoDichVus = yeuCauGoiDichVus
                .Where(x => (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                             && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(a => a.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId))
                           || (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                               && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(a => a.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId))
                            || (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(a => a.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId)))
                .ToList();
            if (!yeuCauGoiDichVuTheoDichVus.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDinh.GoiDichVuCoDichVu.NotExists"));
            }

            #region Cập nhật 21/12/2022: get số lượng dv trong gói đã dùng
            var lstYeuCauGoiDichVuId = yeuCauGoiDichVuTheoDichVus.Select(x => x.Id).ToList();
            long? YeuCauGoiDichVuId = null;
            long YeuCauDichVuId = 0;
            double SoLan = 0;
            var lstSuDungDichVu = new[] { new { YeuCauGoiDichVuId, YeuCauDichVuId, SoLan } }.ToList();
            if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                lstSuDungDichVu = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.YeuCauGoiDichVuId != null
                                && x.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId
                                && lstYeuCauGoiDichVuId.Contains(x.YeuCauGoiDichVuId.Value))
                    .Select(x => new
                    {
                        x.YeuCauGoiDichVuId,
                        YeuCauDichVuId = x.Id,
                        SoLan = (double)1
                    }).ToList();
            }
            else if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat)
            {
                lstSuDungDichVu = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.YeuCauGoiDichVuId != null
                                && x.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId
                                && lstYeuCauGoiDichVuId.Contains(x.YeuCauGoiDichVuId.Value))
                    .Select(x => new
                    {
                        x.YeuCauGoiDichVuId,
                        YeuCauDichVuId = x.Id,
                        SoLan = (double)x.SoLan
                    }).ToList();
            }
            else if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh)
            {
                lstSuDungDichVu = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                    .Where(x => x.YeuCauGoiDichVuId != null
                                && x.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId
                                && lstYeuCauGoiDichVuId.Contains(x.YeuCauGoiDichVuId.Value))
                    .Select(x => new
                    {
                        x.YeuCauGoiDichVuId,
                        YeuCauDichVuId = x.Id,
                        SoLan = x.SoLuong
                    }).ToList();
            }
            #endregion

            var yeuCauGoiDichVu = yeuCauGoiDichVuTheoDichVus
                .Where(x => (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
                                    .Where(a => a.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId)
                                    .Sum(a => a.SoLan) >= (
                                                          #region Cập nhật 21/12/2022: get số lượng dv trong gói đã dùng
                                                          //x.YeuCauKhamBenhs.Count(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                          //                            && a.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId
                                                          //                            && a.YeuCauGoiDichVuId == x.Id) + thongTinChiDinhVo.SoLuong)
                                                          lstSuDungDichVu.Where(a => a.YeuCauGoiDichVuId == x.Id).Count() + thongTinChiDinhVo.SoLuong)
                                                          #endregion
                                                          )
                            || (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                                    .Where(a => a.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId)
                                    .Sum(a => a.SoLan) >= (
                                                            #region Cập nhật 21/12/2022: get số lượng dv trong gói đã dùng
                                                            //x.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            //                                 && a.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId
                                                            //                                && a.YeuCauGoiDichVuId == x.Id)
                                                            //                    .Sum(a => a.SoLan) + thongTinChiDinhVo.SoLuong)
                                                            Math.Round(lstSuDungDichVu.Where(a => a.YeuCauGoiDichVuId == x.Id).Sum(a => a.SoLan), 2) + thongTinChiDinhVo.SoLuong)
                                                            #endregion
                                                          )
                            || (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                                    .Where(a => a.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId)
                                    .Sum(a => a.SoLan) >= (
                                                            #region Cập nhật 21/12/2022: get số lượng dv trong gói đã dùng
                                                            //x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Where(a => a.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId
                                                            //                                 && a.YeuCauGoiDichVuId == x.Id)
                                                            //   .Sum(a => a.SoLuong) + thongTinChiDinhVo.SoLuong)
                                                            Math.Round(lstSuDungDichVu.Where(a => a.YeuCauGoiDichVuId == x.Id).Sum(a => a.SoLan), 2) + thongTinChiDinhVo.SoLuong)
                                                            #endregion
                                                           ))
                .FirstOrDefault();
            if (yeuCauGoiDichVu == null)
            {
                throw new Exception(_localizationService.GetResource("ChiDinh.SoLuongDichVuConLaiTrongGoi.NotEnough"));
            }

            if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat)
            {
                var dichVuTrongGoi =
                    yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.First(x => x.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId);
                thongTinChiDinhVo.YeuCauGoiDichVuId = yeuCauGoiDichVu.Id;
                thongTinChiDinhVo.DonGia = dichVuTrongGoi.DonGia;
                thongTinChiDinhVo.DonGiaTruocChietKhau = dichVuTrongGoi.DonGiaTruocChietKhau;
                thongTinChiDinhVo.DonGiaSauChietKhau = dichVuTrongGoi.DonGiaSauChietKhau;
            }
            else if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                var dichVuTrongGoi =
                    yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.First(x => x.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId);
                thongTinChiDinhVo.YeuCauGoiDichVuId = yeuCauGoiDichVu.Id;
                thongTinChiDinhVo.DonGia = dichVuTrongGoi.DonGia;
                thongTinChiDinhVo.DonGiaTruocChietKhau = dichVuTrongGoi.DonGiaTruocChietKhau;
                thongTinChiDinhVo.DonGiaSauChietKhau = dichVuTrongGoi.DonGiaSauChietKhau;
            }
            else if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh)
            {
                var dichVuTrongGoi =
                    yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(x => x.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId);
                thongTinChiDinhVo.YeuCauGoiDichVuId = yeuCauGoiDichVu.Id;
                thongTinChiDinhVo.DonGia = dichVuTrongGoi.DonGia;
                thongTinChiDinhVo.DonGiaTruocChietKhau = dichVuTrongGoi.DonGiaTruocChietKhau;
                thongTinChiDinhVo.DonGiaSauChietKhau = dichVuTrongGoi.DonGiaSauChietKhau;
            }
        }

        #region Cập nhật ghi nhận VTTH/Thuốc 24/05/2021

        public async Task<bool> KiemTraSoLuongTonCuaThuocVTTHHienTaiAsync(UpdateSoLuongItemGhiNhanVTTHThuocVo updateVo)
        {
            if (updateVo.SoLuong == 0 || updateVo.VatTuThuocBenhVienId == 0 || updateVo.KhoId == 0)
            {
                return true;
            }

            var soLuongTon = 0.0;
            if (updateVo.LaDuocPham == true)
            {
                soLuongTon = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.NhapKhoDuocPhams.KhoId == updateVo.KhoId
                                && x.NhapKhoDuocPhams.DaHet != true
                                && x.DuocPhamBenhVienId == updateVo.VatTuThuocBenhVienId
                                && x.LaDuocPhamBHYT == updateVo.LaBHYT
                                && x.SoLuongDaXuat < x.SoLuongNhap)
                    .Select(x => x.SoLuongNhap - x.SoLuongDaXuat).SumAsync();
            }
            else
            {
                soLuongTon = await _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Where(x => x.NhapKhoVatTu.KhoId == updateVo.KhoId
                                && x.NhapKhoVatTu.DaHet != true
                                && x.VatTuBenhVienId == updateVo.VatTuThuocBenhVienId
                                && x.LaVatTuBHYT == updateVo.LaBHYT
                                && x.SoLuongDaXuat < x.SoLuongNhap)
                    .Select(x => x.SoLuongNhap - x.SoLuongDaXuat).SumAsync();
            }

            return Math.Round((Math.Round(updateVo.SoLuongBanDau, 2) + Math.Round(soLuongTon, 2)), 2) >= Math.Round(updateVo.SoLuong, 2);
        }

        public bool KiemTraTrungGhiNhanVTTHThuoc(VTTHThuocCanKiemTraTrungKhiThemVo info, long yeuCauTiepNhanId)
        {
            var thongTinDichVuChiDinh = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(info.DichVuChiDinhId);
            var thongTinDichVuGhiNhan = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(info.DichVuGhiNhanId);

            var kiemTraTrung = false;
            if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                if(thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham)
                {
                    kiemTraTrung = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(o => o.Id == yeuCauTiepNhanId)
                    .SelectMany(o => o.YeuCauDuocPhamBenhViens)
                    .Where(o => o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                                o.DuocPhamBenhVienId == thongTinDichVuGhiNhan.Id &&
                                o.YeuCauKhamBenhId == thongTinDichVuChiDinh.Id)
                    .Any();
                }
                else if(thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    kiemTraTrung = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(o => o.Id == yeuCauTiepNhanId)
                    .SelectMany(o => o.YeuCauVatTuBenhViens)
                    .Where(o => o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                                o.VatTuBenhVienId == thongTinDichVuGhiNhan.Id &&
                                o.YeuCauKhamBenhId == thongTinDichVuChiDinh.Id)
                    .Any();
                }

                //kiemTraTrung = (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham
                //                && yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                //                    .Any(x => x.YeuCauKhamBenhId == thongTinDichVuChiDinh.Id
                //                              && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                //                              && x.DuocPhamBenhVienId == thongTinDichVuGhiNhan.Id))
                //               || (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao
                //                   && yeuCauTiepNhan.YeuCauVatTuBenhViens
                //                       .Any(x => x.YeuCauKhamBenhId == thongTinDichVuChiDinh.Id
                //                                 && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                //                                 && x.VatTuBenhVienId == thongTinDichVuGhiNhan.Id));
            }
            else if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKyThuat)
            {
                if (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham)
                {
                    kiemTraTrung = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(o => o.Id == yeuCauTiepNhanId)
                    .SelectMany(o => o.YeuCauDuocPhamBenhViens)
                    .Where(o => o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy &&
                                o.DuocPhamBenhVienId == thongTinDichVuGhiNhan.Id &&
                                o.YeuCauDichVuKyThuatId == thongTinDichVuChiDinh.Id)
                    .Any();
                }
                else if (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    kiemTraTrung = _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(o => o.Id == yeuCauTiepNhanId)
                    .SelectMany(o => o.YeuCauVatTuBenhViens)
                    .Where(o => o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy &&
                                o.VatTuBenhVienId == thongTinDichVuGhiNhan.Id &&
                                o.YeuCauDichVuKyThuatId == thongTinDichVuChiDinh.Id)
                    .Any();
                }

                //kiemTraTrung = (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham
                //                && yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                //                    .Any(x => x.YeuCauDichVuKyThuatId == thongTinDichVuChiDinh.Id
                //                              && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                //                              && x.DuocPhamBenhVienId == thongTinDichVuGhiNhan.Id))
                //               || (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao
                //                   && yeuCauTiepNhan.YeuCauVatTuBenhViens
                //                       .Any(x => x.YeuCauDichVuKyThuatId == thongTinDichVuChiDinh.Id
                //                                 && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                //                                 && x.VatTuBenhVienId == thongTinDichVuGhiNhan.Id));
            }

            return kiemTraTrung;
        }

        public bool KiemTraTrungGhiNhanVTTHThuoc(VTTHThuocCanKiemTraTrungKhiThemVo info, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var thongTinDichVuChiDinh = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(info.DichVuChiDinhId);
            var thongTinDichVuGhiNhan = JsonConvert.DeserializeObject<DichVuGhiNhanVo>(info.DichVuGhiNhanId);

            var kiemTraTrung = false;
            if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                kiemTraTrung = (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham
                                && yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                                    .Any(x => x.YeuCauKhamBenhId == thongTinDichVuChiDinh.Id
                                              && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                              && x.DuocPhamBenhVienId == thongTinDichVuGhiNhan.Id))
                               || (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao
                                   && yeuCauTiepNhan.YeuCauVatTuBenhViens
                                       .Any(x => x.YeuCauKhamBenhId == thongTinDichVuChiDinh.Id
                                                 && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                                 && x.VatTuBenhVienId == thongTinDichVuGhiNhan.Id));
            }
            else if (thongTinDichVuChiDinh.NhomId == (int)EnumNhomGoiDichVu.DichVuKyThuat)
            {
                kiemTraTrung = (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.DuocPham
                                && yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                                    .Any(x => x.YeuCauDichVuKyThuatId == thongTinDichVuChiDinh.Id
                                              && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                              && x.DuocPhamBenhVienId == thongTinDichVuGhiNhan.Id))
                               || (thongTinDichVuGhiNhan.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao
                                   && yeuCauTiepNhan.YeuCauVatTuBenhViens
                                       .Any(x => x.YeuCauDichVuKyThuatId == thongTinDichVuChiDinh.Id
                                                 && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                                 && x.VatTuBenhVienId == thongTinDichVuGhiNhan.Id));
            }

            return kiemTraTrung;
        }
        #endregion

        #region search grid popup in xet nghiem
        public async Task<List<TimKiemPopupInKhamBenhKhamBenhGoiDichVuGridVo>> GetDanhSachSearchPopupInChiDinhKhamBenhForGrid(TimKiemPopupInKhamBenhGoiDichVuVo model)
        {
            var queryObjects = new List<TimKiemPopupInKhamBenhKhamBenhGoiDichVuGridVo>();
            if (model.DanhSachCanSearchs != null)
            {

                queryObjects = JsonConvert.DeserializeObject<List<TimKiemPopupInKhamBenhKhamBenhGoiDichVuGridVo>>(model.DanhSachCanSearchs);
            }

            if (!string.IsNullOrEmpty(model.Searching))
            {
                queryObjects = queryObjects.Where(s => s.TenDichVu.RemoveVietnameseDiacritics().ToLower().Trim().Contains(model.Searching.RemoveVietnameseDiacritics().ToLower().Trim())).ToList();
            }


            return queryObjects;
        }
        #endregion

        #region gét danh sách tất cả dịch vụ in chỉ định khác hủy
        public GridDataSource GetDanhSachDichVuChiDinhCuaBenhNhan( long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            var yeucauKhambenh = _yeuCauKhamBenhRepository.TableNoTracking
                 .Include(p=>p.YeuCauDichVuKyThuats).ThenInclude(p=>p.NhomDichVuBenhVien).ThenInclude(p=>p.NhomDichVuBenhVienCha)
                 .Include(p=>p.YeuCauDichVuKyThuats)?.ThenInclude(p=>p.DichVuKyThuatBenhVien)

                 .Where(p => p.Id == yeuCauKhamBenhId && p.YeuCauTiepNhanId == yeuCauTiepNhanId);

            // setup data chp grip
            //KhamBenhTatCaDichVuChiDinhGridVo
            var listDVKT = new List<YeuCauDichVuKyThuat>();

            var listDVKTVaDVK = new List<KhamBenhTatCaDichVuChiDinhGridVo>();
           
            if(yeucauKhambenh.Select(d=>d.YeuCauDichVuKyThuats).Any())
            {
                var lstYCDVKT = yeucauKhambenh.First().YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();
                listDVKT.AddRange(lstYCDVKT);
            }
            var listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
            if (yeucauKhambenh.Any())
            {
                listDVK.AddRange(yeucauKhambenh.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham));
            }

            if(listDVKT.Any())
            {
                foreach(var item  in listDVKT)
                {
                    var objDVKDVKT = new KhamBenhTatCaDichVuChiDinhGridVo();
                    objDVKDVKT.Nhom = (string.IsNullOrEmpty(item.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten) ? "" : item.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten + " - ") + item.NhomDichVuBenhVien.Ten;
                    objDVKDVKT.Id = item.Id;
                    objDVKDVKT.TenDichVu = item.DichVuKyThuatBenhVien.Ten;
                    objDVKDVKT.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                    listDVKTVaDVK.Add(objDVKDVKT);
                }
            }
            if(listDVK.Any())
            {
                 foreach (var item in listDVK)
                {
                    var objDVKDVKT = new KhamBenhTatCaDichVuChiDinhGridVo();
                    objDVKDVKT.Nhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                    objDVKDVKT.Id = item.Id;
                    objDVKDVKT.TenDichVu =item.TenDichVu;
                    objDVKDVKT.NhomId =  EnumNhomGoiDichVu.DichVuKhamBenh;
                    //objDVKDVKT.NhomId = item.
                    listDVKTVaDVK.Add(objDVKDVKT);
                }
            }

            var queryIqueryable = listDVKTVaDVK.AsQueryable();
            var queryTask = queryIqueryable.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = 0 };
        }
        #endregion

        #region BVHD-3298: [PHÁT SINH TRIỂN KHAI] [Chuyển DV khám] Chỉ cho phép chuyển phòng cho DV khám trong khám bệnh

        public async Task<List<LookupItemTemplateVo>> GetListPhongThucHienDichVuTrongKhoaHienTaiAsync(DropDownListRequestModel model)
        {
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var lstNoiThucHienGoiKhamSucKhoeId = new List<long>();

            var arrHangDoiIdStr = !string.IsNullOrEmpty(model.ParameterDependencies) ? model.ParameterDependencies.Split(";").Select(x => long.Parse(x)).ToList() : new List<long>();
            var khoaHienTai = _khoaPhongrepository.TableNoTracking
                .Where(x => x.PhongBenhViens.Any(y => y.Id == phongHienTaiId))
                .First();
            //var DichVuBenhViens = _yeuCauKhamBenhRepository.TableNoTracking
            //    .Where(x => x.PhongBenhVienHangDois.Any(y => arrHangDoiIdStr.Contains(y.Id))
            //                && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
            //    .Select(x => x.DichVuKhamBenhBenhVien)
            //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
            //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
            //    .ToList();
            var yeuCauKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.PhongBenhVienHangDois.Any(y => arrHangDoiIdStr.Contains(y.Id))
                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                .Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
                .Include(x => x.GoiKhamSucKhoe).ThenInclude(x => x.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(x => x.GoiKhamSucKhoeNoiThucHiens)
                .ToList();

            var DichVuBenhViens = yeuCauKhamBenhs.Select(x => x.DichVuKhamBenhBenhVien).ToList();
            DichVuBenhViens = DichVuBenhViens
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .Distinct()
                .ToList();
            var soLuongDichVuBenhVien = DichVuBenhViens.Count;
            lstNoiThucHienGoiKhamSucKhoeId = yeuCauKhamBenhs
                .Where(x => x.GoiKhamSucKhoe != null && x.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.Any())
                .SelectMany(x => x.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs
                                    .Where(a => a.DichVuKhamBenhBenhVienId == x.DichVuKhamBenhBenhVienId)
                                    .SelectMany(a => a.GoiKhamSucKhoeNoiThucHiens.Where(b => b.GoiKhamSucKhoeDichVuKhamBenhId != null).Select(b => b.PhongBenhVienId).ToList()))
                .Distinct()
                .ToList();

            var lstAllPhongTheoDichVu = new List<LookupItemTemplateVo>();
            foreach (var dichVuBenhVien in DichVuBenhViens)
            {
                var lstPhongTheoDichVu = dichVuBenhVien.DichVuKhamBenhBenhVienNoiThucHiens
                    .Where(x => x.PhongBenhVien != null 
                                && x.PhongBenhVien.KhoaPhongId == khoaHienTai.Id
                                && x.PhongBenhVien.IsDisabled != true)
                    .Select(item => new LookupItemTemplateVo()
                    {
                        KeyId = item.PhongBenhVien.Id,
                        Ma = item.PhongBenhVien.Ma,
                        Ten = item.PhongBenhVien.Ten,
                        DisplayName = item.PhongBenhVien.Ten
                    })
                    .Union(
                        dichVuBenhVien.DichVuKhamBenhBenhVienNoiThucHiens
                            .Where(x => x.KhoaPhong != null 
                                        && x.KhoaPhongId == khoaHienTai.Id)
                            .SelectMany(x => x.KhoaPhong.PhongBenhViens)
                            .Where(x => x.IsDisabled != true)
                            .Select(item => new LookupItemTemplateVo()
                            {
                                KeyId = item.Id,
                                Ma = item.Ma,
                                Ten = item.Ten,
                                DisplayName = item.Ten
                            })
                    ).Distinct().ToList();
                lstAllPhongTheoDichVu.AddRange(lstPhongTheoDichVu);
            }

            var queryString = (model.Query ?? "").Trim().ToLower().RemoveVietnameseDiacritics();
            lstAllPhongTheoDichVu = lstAllPhongTheoDichVu.GroupBy(x => x.KeyId)
                .Where(x => x.Count() >= soLuongDichVuBenhVien)
                .Select(x => x.First())
                .Where(p => (p.Ten.Trim().ToLower().RemoveVietnameseDiacritics().Contains(queryString)
                             || p.Ma.Trim().ToLower().RemoveVietnameseDiacritics().Contains(queryString))
                            && p.KeyId != phongHienTaiId)
                .OrderByDescending(x => lstNoiThucHienGoiKhamSucKhoeId.Contains(x.KeyId)).ThenBy(x => x.KeyId)
                .ToList();

            return lstAllPhongTheoDichVu;
        }

        public async Task XuLyChuyenPhongThucHienDichVuKhamAsync(PhongKhamChuyenDenInfoVo phongKhamChuyenDenInfoVo)
        {
            var yeuCauKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.PhongBenhVienHangDois.Any(y => phongKhamChuyenDenInfoVo.HangDoiIds.Contains(y.Id)))
                .Include(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauKhamBenhLichSuTrangThais)
                .Include(x => x.YeuCauTiepNhan)
                .ToList();
            //hiện tại khám đoàn khi bắt đầu khám bệnh nhân đang gán luôn NoiThucHIenId
            if (yeuCauKhamBenhs.Any(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham || (x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.NoiThucHienId != null)))
            {
                throw new Exception(_localizationService.GetResource("ChuyenPhongKhamBenh.YeuCauKham.DaThucHien"));
            }

            var maxSTTHangDoiTrongPhong = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.PhongBenhVienId == phongKhamChuyenDenInfoVo.PhongThucHienId)
                .Select(x => x.SoThuTu)
                .OrderByDescending(x => x)
                .FirstOrDefault();

            foreach (var yeuCauKhamBenh in yeuCauKhamBenhs)
            {
                // lịch sử chuyển
                yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(new YeuCauKhamBenhLichSuTrangThai()
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                    MoTa = string.Format("Chuyển nơi đăng ký thực hiện từ phòng {0} sang {1}", yeuCauKhamBenh.NoiDangKyId, phongKhamChuyenDenInfoVo.PhongThucHienId)
                });

                // cập nhật thông tin
                yeuCauKhamBenh.NoiDangKyId = phongKhamChuyenDenInfoVo.PhongThucHienId;

                //hiện tại khám đoàn khi bắt đầu khám bệnh nhân đang gán luôn NoiThucHIenId
                if (yeuCauKhamBenh.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                {
                    yeuCauKhamBenh.NoiThucHienId = phongKhamChuyenDenInfoVo.PhongThucHienId;
                }
                
                foreach (var hangDoi in yeuCauKhamBenh.PhongBenhVienHangDois)
                {
                    if (phongKhamChuyenDenInfoVo.HangDoiIds.Contains(hangDoi.Id))
                    {
                        hangDoi.PhongBenhVienId = phongKhamChuyenDenInfoVo.PhongThucHienId;
                        hangDoi.SoThuTu = ++maxSTTHangDoiTrongPhong;
                        hangDoi.TrangThai = EnumTrangThaiHangDoi.ChoKham;
                    }
                    else
                    {
                        hangDoi.WillDelete = true;
                    }
                }
            }

            await _yeuCauKhamBenhRepository.UpdateAsync(yeuCauKhamBenhs);
        }
        #endregion
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
        private string MaHocHamHocVi (long id)
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
        #region BVHD-3761
        public void UpdateDichVuKyThuatSarsCoVTheoYeuCauTiepNhan(long yeuCauTiepNhanId, string bieuHienLamSang, string dichTe)
        {
            var yctn = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId);
            yctn.BieuHienLamSang = bieuHienLamSang;
            yctn.DichTeSarsCoV2 = dichTe;
            BaseRepository.Context.SaveChanges();
        }
        #endregion
        #region BVHD-3939
        private double TinhTienDichVuTheoNhom (double giaTien , int soLan)
        {
            return giaTien * soLan;
        }
        #endregion
    }
}

