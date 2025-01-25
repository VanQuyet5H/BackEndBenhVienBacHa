using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using RestSharp.Extensions;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial class YeuCauTiepNhanService
    {
        public async Task ThayDoiLoaiGiaChuaThu(DoiLoaiGiaDanhSachChiPhiKhamChuaBenh doiLoaiGiaDanhSachChiPhiKhamChuaBenh)
        {
            var ycTiepNhan = BaseRepository.GetById(doiLoaiGiaDanhSachChiPhiKhamChuaBenh.YeuCauTiepNhanId,
                x => x
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(g => g.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(g => g.MienGiamChiPhis)
                    .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(g => g.MienGiamChiPhis)
                    .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(g => g.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(g => g.DonThuocThanhToanChiTiets).ThenInclude(g => g.MienGiamChiPhis)
                    .Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan));
            var dichVuKhamBenhBenhVienIds = ycTiepNhan.YeuCauKhamBenhs.Select(o => o.DichVuKhamBenhBenhVienId).ToList();
            var dichVuKyThuatBenhVienIds = ycTiepNhan.YeuCauDichVuKyThuats.Select(o => o.DichVuKyThuatBenhVienId).ToList();

            var dichVuKhamBenhBenhVienGiaBenhViens = _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking.Where(o => dichVuKhamBenhBenhVienIds.Contains(o.DichVuKhamBenhBenhVienId)).ToList();
            var dichVuKyThuatBenhVienGiaBenhViens = _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking.Where(o => dichVuKyThuatBenhVienIds.Contains(o.DichVuKyThuatBenhVienId)).ToList();
            var coDvChuaThanhToan = false;
            var coDvCapNhat = false;
            if (doiLoaiGiaDanhSachChiPhiKhamChuaBenh.LoaiGiaDichVuKhamBenhBenhVienId != null)
            {
                foreach (var yc in ycTiepNhan.YeuCauKhamBenhs.Where(yc => yc.YeuCauGoiDichVuId == null && yc.GoiKhamSucKhoeId == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                {
                    if(yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                    {
                        coDvChuaThanhToan = true;
                    }
                    if (yc.NhomGiaDichVuKhamBenhBenhVienId != doiLoaiGiaDanhSachChiPhiKhamChuaBenh.LoaiGiaDichVuKhamBenhBenhVienId &&
                        doiLoaiGiaDanhSachChiPhiKhamChuaBenh.ChiPhiKhamChuaBenhVos.Where(o => o.CheckedDefault && o.LoaiNhom == NhomChiPhiKhamChuaBenh.DichVuKhamBenh).Select(o => o.Id).Contains(yc.Id))
                    {
                        var giaMoi = dichVuKhamBenhBenhVienGiaBenhViens
                            .Where(o => o.NhomGiaDichVuKhamBenhBenhVienId == doiLoaiGiaDanhSachChiPhiKhamChuaBenh.LoaiGiaDichVuKhamBenhBenhVienId &&
                                        o.DichVuKhamBenhBenhVienId == yc.DichVuKhamBenhBenhVienId && o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                            .OrderBy(o => o.TuNgay).FirstOrDefault();
                        if(giaMoi != null)
                        {
                            coDvCapNhat = true;
                            yc.Gia = giaMoi.Gia;
                            yc.NhomGiaDichVuKhamBenhBenhVienId = doiLoaiGiaDanhSachChiPhiKhamChuaBenh.LoaiGiaDichVuKhamBenhBenhVienId.Value;
                            //cap nhat mien giam
                            foreach (var mienGiam in yc.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null))
                            {
                                if (mienGiam.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe)
                                {
                                    var soTienTruocMienGiam = (yc.KhongTinhPhi == true ? 0 : (decimal)(1 * yc.Gia)) - (yc.BaoHiemChiTra == true ? (decimal)(1 * (yc.DonGiaBaoHiem.GetValueOrDefault() * yc.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yc.MucHuongBaoHiem.GetValueOrDefault() / 100)) : 0);
                                    var soTienMienGiamTheoTiLe = Math.Round((soTienTruocMienGiam * mienGiam.TiLe.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                                    yc.SoTienMienGiam = yc.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    mienGiam.SoTien = soTienMienGiamTheoTiLe;
                                    yc.SoTienMienGiam = yc.SoTienMienGiam.GetValueOrDefault() + soTienMienGiamTheoTiLe;
                                }
                                else
                                {
                                    if (mienGiam.SoTien > yc.Gia)
                                    {
                                        yc.SoTienMienGiam = yc.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                        mienGiam.SoTien = yc.Gia;
                                        yc.SoTienMienGiam = yc.SoTienMienGiam.GetValueOrDefault() + yc.Gia;
                                    }
                                }
                            }
                        }
                        
                    }
                }
            }
            if (doiLoaiGiaDanhSachChiPhiKhamChuaBenh.LoaiGiaDichVuKyThuatBenhVienId != null)
            {
                foreach (var yc in ycTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.YeuCauGoiDichVuId == null && yc.GoiKhamSucKhoeId == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                {
                    if (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                    {
                        coDvChuaThanhToan = true;
                    }
                    if (yc.NhomGiaDichVuKyThuatBenhVienId != doiLoaiGiaDanhSachChiPhiKhamChuaBenh.LoaiGiaDichVuKyThuatBenhVienId &&
                        doiLoaiGiaDanhSachChiPhiKhamChuaBenh.ChiPhiKhamChuaBenhVos.Where(o => o.CheckedDefault && o.LoaiNhom == NhomChiPhiKhamChuaBenh.DichVuKyThuat).Select(o => o.Id).Contains(yc.Id))
                    {
                        var giaMoi = dichVuKyThuatBenhVienGiaBenhViens
                            .Where(o => o.NhomGiaDichVuKyThuatBenhVienId == doiLoaiGiaDanhSachChiPhiKhamChuaBenh.LoaiGiaDichVuKyThuatBenhVienId &&
                                        o.DichVuKyThuatBenhVienId == yc.DichVuKyThuatBenhVienId && o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                            .OrderBy(o => o.TuNgay).FirstOrDefault();
                        if (giaMoi != null)
                        {
                            coDvCapNhat = true;
                            yc.Gia = giaMoi.Gia;
                            yc.NhomGiaDichVuKyThuatBenhVienId = doiLoaiGiaDanhSachChiPhiKhamChuaBenh.LoaiGiaDichVuKyThuatBenhVienId.Value;
                            //cap nhat mien giam
                            foreach (var mienGiam in yc.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null))
                            {
                                if (mienGiam.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe)
                                {
                                    var soTienTruocMienGiam = (yc.KhongTinhPhi == true ? 0 : (decimal)(yc.SoLan * yc.Gia)) - (yc.BaoHiemChiTra == true ? (decimal)(yc.SoLan * (yc.DonGiaBaoHiem.GetValueOrDefault() * yc.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yc.MucHuongBaoHiem.GetValueOrDefault() / 100)) : 0);
                                    var soTienMienGiamTheoTiLe = Math.Round((soTienTruocMienGiam * mienGiam.TiLe.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                                    yc.SoTienMienGiam = yc.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    mienGiam.SoTien = soTienMienGiamTheoTiLe;
                                    yc.SoTienMienGiam = yc.SoTienMienGiam.GetValueOrDefault() + soTienMienGiamTheoTiLe;
                                }
                                else
                                {
                                    if (mienGiam.SoTien > yc.Gia)
                                    {
                                        yc.SoTienMienGiam = yc.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                        mienGiam.SoTien = yc.Gia;
                                        yc.SoTienMienGiam = yc.SoTienMienGiam.GetValueOrDefault() + yc.Gia;
                                    }
                                }
                            }
                        }                        
                    }
                }
            }
            if (coDvCapNhat)
            {
                if(coDvChuaThanhToan)
                {
                    var soTienTamUng = _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(ycTiepNhan.Id).Result;
                    BaoLanhDvNgoaiGoiMarketing(ycTiepNhan, soTienTamUng - GetSoTienCanThanhToanNgoaiTru(ycTiepNhan));
                }
                BaseRepository.Context.SaveChanges();
            }
        }
    }
}
