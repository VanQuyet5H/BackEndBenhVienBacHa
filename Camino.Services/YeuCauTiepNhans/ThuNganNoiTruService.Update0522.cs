using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial class ThuNganNoiTruService
    {
        public async Task ThayDoiLoaiGiaChuaThu(DoiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru)
        {
            var ycTiepNhan = BaseRepository.GetById(doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.YeuCauTiepNhanId,
                x => x.Include(o => o.YeuCauKhamBenhs).ThenInclude(g => g.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(g => g.MienGiamChiPhis)                    
                    .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(g => g.MienGiamChiPhis)
                    .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(g => g.YeuCauDichVuGiuongBenhVienChiPhiBHYTs));

            long? yeuCauTiepNhanNgoaiTruCanQuyetToanId = ycTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
            YeuCauTiepNhan yeuCauTiepNhanNgoaiTruCanQuyetToan = null;
            if (yeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
            {
                yeuCauTiepNhanNgoaiTruCanQuyetToan = BaseRepository.GetById(yeuCauTiepNhanNgoaiTruCanQuyetToanId.Value,
                    x => x
                        .Include(o => o.YeuCauKhamBenhs).ThenInclude(g => g.MienGiamChiPhis)
                        .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(g => g.MienGiamChiPhis));
            }

            var dichVuKhamBenhBenhVienIds = ycTiepNhan.YeuCauKhamBenhs.Select(o => o.DichVuKhamBenhBenhVienId).ToList();
            var dichVuKyThuatBenhVienIds = ycTiepNhan.YeuCauDichVuKyThuats.Select(o => o.DichVuKyThuatBenhVienId).ToList();
            var dichVuGiuongBenhVienIds = ycTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Select(o => o.DichVuGiuongBenhVienId).ToList();
            if (yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
            {
                dichVuKhamBenhBenhVienIds.AddRange(yeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauKhamBenhs.Select(o => o.DichVuKhamBenhBenhVienId).ToList());
                dichVuKyThuatBenhVienIds.AddRange(yeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDichVuKyThuats.Select(o => o.DichVuKyThuatBenhVienId).ToList());
            }

            var dichVuKhamBenhBenhVienGiaBenhViens = _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking.Where(o => dichVuKhamBenhBenhVienIds.Contains(o.DichVuKhamBenhBenhVienId)).ToList();
            var dichVuKyThuatBenhVienGiaBenhViens = _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking.Where(o => dichVuKyThuatBenhVienIds.Contains(o.DichVuKyThuatBenhVienId)).ToList();
            var dichVuGiuongBenhVienGiaBenhViens = _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking.Where(o => dichVuGiuongBenhVienIds.Contains(o.DichVuGiuongBenhVienId)).ToList();

            var coDvCapNhat = false;
            if (doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKhamBenhBenhVienId != null)
            {
                foreach (var yc in ycTiepNhan.YeuCauKhamBenhs.Where(yc => yc.YeuCauGoiDichVuId == null && yc.GoiKhamSucKhoeId == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                {                    
                    if (yc.NhomGiaDichVuKhamBenhBenhVienId != doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKhamBenhBenhVienId &&
                        doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.ChiPhiKhamChuaBenhNoiTruVos.Where(o => o.CheckedDefault && o.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh).Select(o => o.Id).Contains(yc.Id))
                    {
                        var giaMoi = dichVuKhamBenhBenhVienGiaBenhViens
                            .Where(o => o.NhomGiaDichVuKhamBenhBenhVienId == doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKhamBenhBenhVienId &&
                                        o.DichVuKhamBenhBenhVienId == yc.DichVuKhamBenhBenhVienId && o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                            .OrderBy(o => o.TuNgay).FirstOrDefault();
                        if (giaMoi != null)
                        {
                            coDvCapNhat = true;
                            yc.Gia = giaMoi.Gia;
                            yc.NhomGiaDichVuKhamBenhBenhVienId = doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKhamBenhBenhVienId.Value;
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
                if (yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
                {
                    foreach (var yc in yeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauKhamBenhs.Where(yc => yc.YeuCauGoiDichVuId == null && yc.GoiKhamSucKhoeId == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                    {
                        if (yc.NhomGiaDichVuKhamBenhBenhVienId != doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKhamBenhBenhVienId &&
                            doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.ChiPhiKhamChuaBenhNoiTruVos.Where(o => o.CheckedDefault && o.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh).Select(o => o.Id).Contains(yc.Id))
                        {
                            var giaMoi = dichVuKhamBenhBenhVienGiaBenhViens
                                .Where(o => o.NhomGiaDichVuKhamBenhBenhVienId == doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKhamBenhBenhVienId &&
                                            o.DichVuKhamBenhBenhVienId == yc.DichVuKhamBenhBenhVienId && o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                                .OrderBy(o => o.TuNgay).FirstOrDefault();
                            if (giaMoi != null)
                            {
                                coDvCapNhat = true;
                                yc.Gia = giaMoi.Gia;
                                yc.NhomGiaDichVuKhamBenhBenhVienId = doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKhamBenhBenhVienId.Value;
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
            }
            if (doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKyThuatBenhVienId != null)
            {
                foreach (var yc in ycTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.YeuCauGoiDichVuId == null && yc.GoiKhamSucKhoeId == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                {
                    if (yc.NhomGiaDichVuKyThuatBenhVienId != doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKyThuatBenhVienId &&
                        doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.ChiPhiKhamChuaBenhNoiTruVos.Where(o => o.CheckedDefault && o.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat).Select(o => o.Id).Contains(yc.Id))
                    {
                        var giaMoi = dichVuKyThuatBenhVienGiaBenhViens
                            .Where(o => o.NhomGiaDichVuKyThuatBenhVienId == doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKyThuatBenhVienId &&
                                        o.DichVuKyThuatBenhVienId == yc.DichVuKyThuatBenhVienId && o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                            .OrderBy(o => o.TuNgay).FirstOrDefault();
                        if (giaMoi != null)
                        {
                            coDvCapNhat = true;
                            yc.Gia = giaMoi.Gia;
                            yc.NhomGiaDichVuKyThuatBenhVienId = doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKyThuatBenhVienId.Value;
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
                if (yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
                {
                    foreach (var yc in yeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDichVuKyThuats.Where(yc => yc.YeuCauGoiDichVuId == null && yc.GoiKhamSucKhoeId == null && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                    {
                        if (yc.NhomGiaDichVuKyThuatBenhVienId != doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKyThuatBenhVienId &&
                            doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.ChiPhiKhamChuaBenhNoiTruVos.Where(o => o.CheckedDefault && o.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat).Select(o => o.Id).Contains(yc.Id))
                        {
                            var giaMoi = dichVuKyThuatBenhVienGiaBenhViens
                                .Where(o => o.NhomGiaDichVuKyThuatBenhVienId == doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKyThuatBenhVienId &&
                                            o.DichVuKyThuatBenhVienId == yc.DichVuKyThuatBenhVienId && o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                                .OrderBy(o => o.TuNgay).FirstOrDefault();
                            if (giaMoi != null)
                            {
                                coDvCapNhat = true;
                                yc.Gia = giaMoi.Gia;
                                yc.NhomGiaDichVuKyThuatBenhVienId = doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuKyThuatBenhVienId.Value;
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
            }
            if (doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuGiuongBenhVienId != null)
            {
                foreach (var yc in ycTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Where(yc => yc.YeuCauGoiDichVuId == null && 
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                {
                    if (yc.NhomGiaDichVuGiuongBenhVienId != doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuGiuongBenhVienId &&
                        doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.ChiPhiKhamChuaBenhNoiTruVos.Where(o => o.CheckedDefault && o.LoaiNhom == NhomChiPhiNoiTru.DichVuGiuong).Select(o => o.Id).Contains(yc.Id))
                    {
                        var giaMoi = dichVuGiuongBenhVienGiaBenhViens
                            .Where(o => o.NhomGiaDichVuGiuongBenhVienId == doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuGiuongBenhVienId &&
                                        o.DichVuGiuongBenhVienId == yc.DichVuGiuongBenhVienId && o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                            .OrderBy(o => o.TuNgay).FirstOrDefault();
                        if (giaMoi != null)
                        {
                            coDvCapNhat = true;
                            yc.Gia = giaMoi.Gia;
                            yc.NhomGiaDichVuGiuongBenhVienId = doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru.LoaiGiaDichVuGiuongBenhVienId.Value;
                            //cap nhat mien giam
                            foreach (var mienGiam in yc.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null))
                            {
                                if (mienGiam.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe)
                                {
                                    var chiPhiBHYT = ycTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Where(o => o.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId == yc.Id).FirstOrDefault();
                                    decimal bhytThanhToan = 0;
                                    if(chiPhiBHYT != null)
                                    {
                                        bhytThanhToan = (chiPhiBHYT.BaoHiemChiTra == true ? (decimal)((decimal)chiPhiBHYT.SoLuong * (chiPhiBHYT.DonGiaBaoHiem.GetValueOrDefault() * chiPhiBHYT.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * chiPhiBHYT.MucHuongBaoHiem.GetValueOrDefault() / 100)) : 0);
                                    }    
                                    var soTienTruocMienGiam = ((decimal)yc.SoLuong * yc.Gia) - bhytThanhToan;
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
                BaseRepository.Context.SaveChanges();
            }
        }
    }
}
