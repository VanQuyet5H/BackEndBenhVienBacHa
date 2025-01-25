using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using static Camino.Core.Domain.Enums;
using System.Globalization;

namespace Camino.Services.NhapKhoVatTus
{
    public partial class NhapKhoVatTuService
    {
        #region Danh Sach Vật Tư Và Vật Tư Chi Tiết

        public async Task<ThongTinDuyetKhoVatTu> GetThongTinDuyetKhoVatTu(long yeuCauNhapKhooVatTuId)
        {
            var thongTinDuyetKhooVatTu = await _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(cc => cc.Id == yeuCauNhapKhooVatTuId)
                                                                           .Select(cc => new ThongTinDuyetKhoVatTu
                                                                           {
                                                                               NgayNhap = cc.NgayNhap,
                                                                               NguoiNhapId = cc.NguoiNhapId,
                                                                               SoChungTu = cc.SoChungTu,
                                                                               TenNguoiNhap = cc.NguoiNhap.User.HoTen,
                                                                               TinhTrang = cc.DuocKeToanDuyet,

                                                                               NgayDuyet = cc.NgayDuyet,
                                                                               NguoiDuyet = cc.NhanVienDuyet.User.HoTen,
                                                                               NguoiDuyetId = cc.NhanVienDuyetId
                                                                           }).FirstOrDefaultAsync();
            return thongTinDuyetKhooVatTu;
        }
        public async Task<bool> TuChoiDuyetVatTuNhapKho(ThongTinLyDoHuyNhapKhoVatTu thongTinLyDoHuyNhapKhooVatTu)
        {
            var yeuCauNhapKhoVatTu = await _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(o => o.Id == thongTinLyDoHuyNhapKhooVatTu.YeuCauNhapKhoVatTuId)
                                                                           .FirstOrDefaultAsync();

            if (yeuCauNhapKhoVatTu != null)
            {
                yeuCauNhapKhoVatTu.DuocKeToanDuyet = false;
                yeuCauNhapKhoVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauNhapKhoVatTu.NgayDuyet = DateTime.Now;
                yeuCauNhapKhoVatTu.LyDoKhongDuyet = thongTinLyDoHuyNhapKhooVatTu.LyDoHuy;
                _yeuCauNhapKhoVatTuRepository.Update(yeuCauNhapKhoVatTu);
                return true;
            }
            return false;
        }
        public async Task<string> DuyetVatTuNhapKho(long id)
        {
            _yeuCauNhapKhoVatTuRepository.AutoCommitEnabled = false;
            BaseRepository.AutoCommitEnabled = false;

            var yeuCauNhapKhoVatTu = await _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(o => o.Id == id)
                                                                              .Include(cc => cc.YeuCauNhapKhoVatTuChiTiets)
                                                                              .ThenInclude(cc => cc.VatTuBenhVien)
                                                                              .ThenInclude(c => c.VatTus)
                                                                              .FirstOrDefaultAsync();

            if (yeuCauNhapKhoVatTu.DuocKeToanDuyet == true)
                throw new Exception("Yêu cầu nhập kho vật tư đã được kế toán duyệt, Vui lòng tải lại trang!");

            var kiemTraSLNhapKhoVatTu = KiemTraSoLuongNhap(yeuCauNhapKhoVatTu);

            if (!String.IsNullOrEmpty(kiemTraSLNhapKhoVatTu)) return kiemTraSLNhapKhoVatTu;

            if (yeuCauNhapKhoVatTu != null)
            {
                yeuCauNhapKhoVatTu.DuocKeToanDuyet = true;
                yeuCauNhapKhoVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauNhapKhoVatTu.NgayDuyet = DateTime.Now;
                _yeuCauNhapKhoVatTuRepository.Update(yeuCauNhapKhoVatTu);


                //Tạo phiếu nhập và kho tổng cấp 1
                var nhapKhoVatTuModel = MapNhapKhoVatTu(yeuCauNhapKhoVatTu);

                //Cập nhật số lượng hợp đồng thầu
                CapNhatSoLuongDaCapHopDongThau(yeuCauNhapKhoVatTu);

                //Tạo phiếu xuất và phiếu xuất chi tiết kho tổng cấp 1 xuống cấp 2
                //TaoPhieuXuatKhoVatTu(nhapKhoVatTuModel);

                //Tạo phiếu Nhập và phiếu nhập chi tiết kho tổng cấp 2
                //TaoPhieuNhapKhoVatTu(nhapKhoVatTuModel);

                BaseRepository.Add(nhapKhoVatTuModel);
                XuatVeKhoSauKhiNhap(nhapKhoVatTuModel);
                BaseRepository.Context.SaveChanges();
                return string.Empty;
            }
            return string.Empty;
        }
        public void CapNhatSoLuongDaCapHopDongThau(YeuCauNhapKhoVatTu yeuCauNhapKhoVatTu)
        {
            var HopDongThauVTIds = yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Select(cc => cc.HopDongThauVatTuId).Distinct().ToList();
            var hopDongThauVTChiTiets = _hopDongThauVatTuChiTiet.Table.Include(cc => cc.VatTu).Where(p => HopDongThauVTIds.Contains(p.HopDongThauVatTuId)).ToList();
            if (yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Any())
            {
                foreach (var hopDongThauVTChiTiet in hopDongThauVTChiTiets)
                {
                    var vatTus = yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Where(cc => cc.HopDongThauVatTuId == hopDongThauVTChiTiet.HopDongThauVatTuId).Select(cc => cc.VatTuBenhVien.VatTus.Id).Distinct();
                    if (vatTus.Any())
                    {
                        foreach (var vt in vatTus)
                        {
                            var groupVTBenhVienIds = yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Where(cc => cc.HopDongThauVatTuId == hopDongThauVTChiTiet.HopDongThauVatTuId).GroupBy(cc => cc.VatTuBenhVien.VatTus.Id)
                                                                    .Select(c => new { DuocPhamId = c.Key, SoLuongNhap = c.Sum(ccc => ccc.SoLuongNhap) })
                                                                    .ToDictionary(cc => cc.DuocPhamId, cc => cc.SoLuongNhap);
                            if (groupVTBenhVienIds.Any())
                            {
                                if (hopDongThauVTChiTiet.VatTuId == vt && groupVTBenhVienIds[vt] > 0)
                                {
                                    if (hopDongThauVTChiTiet.SoLuong - hopDongThauVTChiTiet.SoLuongDaCap >= groupVTBenhVienIds[vt])
                                    {
                                        hopDongThauVTChiTiet.SoLuongDaCap += groupVTBenhVienIds[vt];
                                        _hopDongThauVatTuChiTiet.Update(hopDongThauVTChiTiet);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void XuatVeKhoSauKhiNhap(NhapKhoVatTu nhapKhoVatTuModel)
        {
            var listKhoNhapSauKhiDuyet = nhapKhoVatTuModel.NhapKhoVatTuChiTiets
                 .Where(p => p.KhoNhapSauKhiDuyetId != null && p.KhoNhapSauKhiDuyetId > 0)
                 .Select(p => p.KhoNhapSauKhiDuyetId).Distinct();
            if (listKhoNhapSauKhiDuyet != null && listKhoNhapSauKhiDuyet.Any())
            {
                foreach (var khoId in listKhoNhapSauKhiDuyet)
                {
                    var xuatKhoVatTu = new XuatKhoVatTu
                    {
                        LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatQuaKhoKhac,
                        LyDoXuatKho = Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet,
                        //TenNguoiNhan=
                        //NguoiNhanId
                        NguoiXuatId = nhapKhoVatTuModel.NguoiNhapId,
                        LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                        NgayXuat = DateTime.Now,
                        KhoXuatId = nhapKhoVatTuModel.KhoId,
                        KhoNhapId = (long)khoId

                    };
                    foreach (var item in nhapKhoVatTuModel.NhapKhoVatTuChiTiets.Where(p => p.KhoNhapSauKhiDuyetId == khoId))
                    {
                        xuatKhoVatTu.NguoiNhanId = item.NguoiNhapSauKhiDuyetId ?? 1;
                        if (xuatKhoVatTu.NguoiNhanId != null && xuatKhoVatTu.NguoiNhanId > 0)
                        {
                            var nguoiNhapSauKhiDuyet = _userRepository.GetById((long)xuatKhoVatTu.NguoiNhanId);
                            xuatKhoVatTu.TenNguoiNhan = nguoiNhapSauKhiDuyet?.HoTen;
                        }
                        //                        var nhapKho =
                        //                            nhapKhoVatTuModel.NhapKhoVatTuChiTiets.FirstOrDefault(o =>
                        //                                o.VatTuBenhVienId == item.VatTuBenhVienId);
                        var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            NgayXuat = DateTime.Now

                        };
                        xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(new XuatKhoVatTuChiTietViTri
                        {
                            NhapKhoVatTuChiTietId = item.Id,
                            SoLuongXuat = item.SoLuongNhap,
                            NgayXuat = DateTime.Now

                        });
                        xuatKhoVatTu.XuatKhoVatTuChiTiets.Add(xuatKhoVatTuChiTiet);
                    }
                    _xuatKhoVatTuRepository.Add(xuatKhoVatTu);

                    var nhapKhoVatTu = new NhapKhoVatTu
                    {
                        KhoId = (long)khoId,
                        XuatKhoVatTuId = xuatKhoVatTu.Id,
                        SoChungTu = null,
                        TenNguoiGiao = xuatKhoVatTu.NguoiXuat?.User?.HoTen,
                        NguoiGiaoId = xuatKhoVatTu.NguoiXuatId,
                        NguoiNhapId = xuatKhoVatTu.NguoiNhanId ?? 0,
                        DaHet = false,
                        NgayNhap = DateTime.Now,
                        LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong
                    };
                    foreach (var item in xuatKhoVatTu.XuatKhoVatTuChiTiets)
                    {
                        foreach (var viTri in item.XuatKhoVatTuChiTietViTris)
                        {
                            var vatTuNhapChiTietCu = nhapKhoVatTuModel.NhapKhoVatTuChiTiets.FirstOrDefault(o => o.Id == viTri.NhapKhoVatTuChiTietId);
                            if (vatTuNhapChiTietCu != null)
                            {
                                var nhapKhoVatTuChiTiet = new NhapKhoVatTuChiTiet();
                                nhapKhoVatTuChiTiet.HopDongThauVatTuId = vatTuNhapChiTietCu.HopDongThauVatTuId;
                                nhapKhoVatTuChiTiet.Solo = vatTuNhapChiTietCu.Solo;

                                nhapKhoVatTuChiTiet.LaVatTuBHYT = vatTuNhapChiTietCu.LaVatTuBHYT;
                                nhapKhoVatTuChiTiet.HanSuDung = vatTuNhapChiTietCu.HanSuDung;
                                nhapKhoVatTuChiTiet.SoLuongNhap = viTri.SoLuongXuat;
                                nhapKhoVatTuChiTiet.DonGiaNhap = vatTuNhapChiTietCu.DonGiaNhap;


                                nhapKhoVatTuChiTiet.VAT = vatTuNhapChiTietCu.VAT;
                                nhapKhoVatTuChiTiet.TiLeBHYTThanhToan = vatTuNhapChiTietCu.TiLeBHYTThanhToan;

                                nhapKhoVatTuChiTiet.MaVach = vatTuNhapChiTietCu.MaVach;
                                nhapKhoVatTuChiTiet.MaRef = vatTuNhapChiTietCu.MaRef;
                                nhapKhoVatTuChiTiet.VatTuBenhVienId = vatTuNhapChiTietCu.VatTuBenhVienId;
                                //need update
                                nhapKhoVatTuChiTiet.SoLuongDaXuat = 0;
                                nhapKhoVatTuChiTiet.NgayNhap = DateTime.Now;

                                //
                                nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien = vatTuNhapChiTietCu.NgayNhapVaoBenhVien;
                                nhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho = vatTuNhapChiTietCu.PhuongPhapTinhGiaTriTonKho;
                                nhapKhoVatTuChiTiet.TiLeTheoThapGia = vatTuNhapChiTietCu.TiLeTheoThapGia;


                                nhapKhoVatTu.NhapKhoVatTuChiTiets.Add(nhapKhoVatTuChiTiet);
                            }
                        }
                    }
                    _nhapKhoVatTuRepository.Add(nhapKhoVatTu);
                }
            }
        }
        public string KiemTraSoLuongNhap(YeuCauNhapKhoVatTu yeuCauNhapKhoVatTu)
        {
            var HopDongThauVTIds = yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Select(cc => cc.HopDongThauVatTuId).Distinct().ToList();
            var hopDongThauVTChiTiets = _hopDongThauVatTuChiTiet.Table.Include(cc => cc.VatTu).Where(p => HopDongThauVTIds.Contains(p.HopDongThauVatTuId)).ToList();
            if (yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Any())
            {
                foreach (var hopDongThauVTChiTiet in hopDongThauVTChiTiets)
                {
                    var vatTus = yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Where(cc => cc.HopDongThauVatTuId == hopDongThauVTChiTiet.HopDongThauVatTuId).Select(cc => cc.VatTuBenhVien.VatTus.Id).Distinct();
                    if (vatTus.Any())
                    {
                        foreach (var vt in vatTus)
                        {
                            var groupVTBenhVienIds = yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Where(cc => cc.HopDongThauVatTuId == hopDongThauVTChiTiet.HopDongThauVatTuId).GroupBy(cc => cc.VatTuBenhVien.VatTus.Id)
                                                                    .Select(c => new { DuocPhamId = c.Key, SoLuongNhap = c.Sum(ccc => ccc.SoLuongNhap) })
                                                                    .ToDictionary(cc => cc.DuocPhamId, cc => cc.SoLuongNhap);
                            if (groupVTBenhVienIds.Any())
                            {
                                if (hopDongThauVTChiTiet.VatTuId == vt && groupVTBenhVienIds[vt] > 0)
                                {
                                    if (hopDongThauVTChiTiet.SoLuong - hopDongThauVTChiTiet.SoLuongDaCap < groupVTBenhVienIds[vt])
                                    {
                                        return "Số Lượng nhập của " + hopDongThauVTChiTiet.VatTu.Ten + " lớn hơn số lượng chưa nhập trong hợp đồng thầu, Bạn hãy kiểm tra lại.";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }
        public void TaoPhieuXuatKhoVatTu(NhapKhoVatTu nhapKhoVatTu)
        {
            var xuatKhoVatTu = _xuatKhoVatTuRepository.Table;
            if (nhapKhoVatTu != null)
            {
                var khoTong1Id = _khoRepository.TableNoTracking.First(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1).Id;
                var khoTong2Id = _khoRepository.TableNoTracking.First(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2).Id;
                var tenNguoiNhan = _nhanVienRepository.TableNoTracking.Include(p => p.User).FirstOrDefault(p => p.Id == nhapKhoVatTu.NguoiNhapId)?.User.HoTen;
                nhapKhoVatTu.XuatKhoVatTu = new Core.Domain.Entities.XuatKhoVatTus.XuatKhoVatTu()
                {
                    //KhoXuatId = (int)EnumLoaiKhoDuocPham.KhoTongVTYTCap1,
                    //KhoNhapId = (int)EnumLoaiKhoDuocPham.KhoTongVTYTCap2,

                    KhoXuatId = khoTong1Id,
                    KhoNhapId = khoTong2Id,

                    LoaiXuatKho = Core.Domain.Enums.EnumLoaiXuatKho.XuatQuaKhoKhac,
                    LyDoXuatKho = Core.Domain.Enums.EnumLoaiXuatKho.XuatQuaKhoKhac.GetDescription(),

                    NguoiNhanId = nhapKhoVatTu.NguoiNhapId,
                    TenNguoiNhan = tenNguoiNhan,
                    NguoiXuatId = nhapKhoVatTu.NguoiNhapId,
                    LoaiNguoiNhan = Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong,

                    NgayXuat = DateTime.Now

                };

                if (nhapKhoVatTu.NhapKhoVatTuChiTiets.Any())
                {
                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTu.NhapKhoVatTuChiTiets)
                    {
                        var xuatKhoVatTuChiTiet = new Core.Domain.Entities.XuatKhoVatTus.XuatKhoVatTuChiTiet()
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                            NgayXuat = DateTime.Now
                        };
                        nhapKhoVatTu.XuatKhoVatTu.XuatKhoVatTuChiTiets.Add(xuatKhoVatTuChiTiet);
                    }
                    TaoXuatKhoVatTuChiTietViTri(nhapKhoVatTu);
                }
            }
        }
        public void TaoXuatKhoVatTuChiTietViTri(NhapKhoVatTu nhapKhoVatTu)
        {
            foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTu.NhapKhoVatTuChiTiets)
            {
                foreach (var xuatKhoVatTuChiTiets in nhapKhoVatTu.XuatKhoVatTu.XuatKhoVatTuChiTiets)
                {
                    if (nhapKhoVatTuChiTiet.VatTuBenhVienId == xuatKhoVatTuChiTiets.VatTuBenhVienId)
                    {
                        var xuatKhoVatTuChiTietViTri = new Core.Domain.Entities.XuatKhoVatTus.XuatKhoVatTuChiTietViTri()
                        {
                            SoLuongXuat = nhapKhoVatTuChiTiet.SoLuongNhap,
                            NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                            //NhapKhoVatTuChiTietId = nhapKhoVatTuChiTiet.Id,
                        };
                        xuatKhoVatTuChiTiets.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                    }
                }
                nhapKhoVatTuChiTiet.SoLuongDaXuat = nhapKhoVatTuChiTiet.SoLuongNhap;
            }
        }
        public void TaoPhieuNhapKhoVatTu(NhapKhoVatTu nhapKhoVatTu)
        {

            if (nhapKhoVatTu != null)
            {
                //yeuCauNhapKhoVatTu.DuocKeToanDuyet = true;
                //yeuCauNhapKhoVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                //yeuCauNhapKhoVatTu.NgayDuyet = DateTime.Now;
                //var nhapKhoVatTuModel = MapNhapKhoVatTu(yeuCauNhapKhoVatTu);
                var nhapKhoVatTuNew = new NhapKhoVatTu();
                nhapKhoVatTuNew.XuatKhoVatTuId = nhapKhoVatTu.XuatKhoVatTu.Id;
                nhapKhoVatTuNew.KhoId = nhapKhoVatTu.XuatKhoVatTu.KhoNhapId ?? 0;
                nhapKhoVatTuNew.SoChungTu = nhapKhoVatTu.SoChungTu;
                nhapKhoVatTuNew.YeuCauNhapKhoVatTuId = nhapKhoVatTu.YeuCauNhapKhoVatTuId;
                nhapKhoVatTuNew.XuatKhoVatTu = nhapKhoVatTu.XuatKhoVatTu;
                //nhapKhoVatTuNew. = Enums.EnumLoaiNhapKho.NhapTuKhoKhac;

                var tenNguoiXuat = _nhanVienRepository.TableNoTracking.Include(p => p.User).FirstOrDefault(p => p.Id == nhapKhoVatTu.XuatKhoVatTu.NguoiXuatId)?.User.HoTen;
                nhapKhoVatTuNew.TenNguoiGiao = tenNguoiXuat;


                nhapKhoVatTuNew.NguoiGiaoId = nhapKhoVatTu.XuatKhoVatTu.NguoiXuatId;
                nhapKhoVatTuNew.NguoiNhapId = nhapKhoVatTu.XuatKhoVatTu.NguoiNhanId ?? 0;
                nhapKhoVatTuNew.DaHet = false;
                nhapKhoVatTuNew.NgayNhap = nhapKhoVatTu.XuatKhoVatTu.NgayXuat;
                nhapKhoVatTuNew.LoaiNguoiGiao = nhapKhoVatTu.XuatKhoVatTu.LoaiNguoiNhan;

                foreach (var item in nhapKhoVatTu.XuatKhoVatTu.XuatKhoVatTuChiTiets)
                {
                    foreach (var viTri in item.XuatKhoVatTuChiTietViTris)
                    {
                        //var duocPhamNhapChiTietCu = await _nhapKhoDuocPhamService.GetNhapKhoDuocPhamChiTietById(viTri.NhapKhoDuocPhamChiTietId ?? 0);
                        var duocPhamNhapChiTietCu = viTri.NhapKhoVatTuChiTiet;
                        var nhapKhoDuocPhamChiTiet = new NhapKhoVatTuChiTiet();
                        nhapKhoDuocPhamChiTiet.HopDongThauVatTuId = duocPhamNhapChiTietCu.HopDongThauVatTuId;
                        nhapKhoDuocPhamChiTiet.Solo = duocPhamNhapChiTietCu.Solo;

                        nhapKhoDuocPhamChiTiet.LaVatTuBHYT = duocPhamNhapChiTietCu.LaVatTuBHYT;
                        nhapKhoDuocPhamChiTiet.HanSuDung = duocPhamNhapChiTietCu.HanSuDung;
                        nhapKhoDuocPhamChiTiet.SoLuongNhap = viTri.SoLuongXuat;
                        nhapKhoDuocPhamChiTiet.DonGiaNhap = duocPhamNhapChiTietCu.DonGiaNhap;

                        //nhapKhoDuocPhamChiTiet.DonGiaBan = item.DonGiaBan ?? duocPhamNhapChiTietCu.DonGiaBan;

                        nhapKhoDuocPhamChiTiet.VAT = duocPhamNhapChiTietCu.VAT;

                        //nhapKhoDuocPhamChiTiet.ChietKhau = item.ChietKhau ?? duocPhamNhapChiTietCu.ChietKhau;
                        nhapKhoDuocPhamChiTiet.MaVach = duocPhamNhapChiTietCu.MaVach;
                        nhapKhoDuocPhamChiTiet.MaRef = duocPhamNhapChiTietCu.MaRef;
                        nhapKhoDuocPhamChiTiet.VatTuBenhVienId = item.VatTuBenhVienId;

                        //nhapKhoDuocPhamChiTiet.KhoDuocPhamViTriId = null;
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat = 0;
                        nhapKhoDuocPhamChiTiet.NgayNhap = DateTime.Now;
                        //
                        nhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien = duocPhamNhapChiTietCu.NgayNhapVaoBenhVien;
                        nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho = duocPhamNhapChiTietCu.PhuongPhapTinhGiaTriTonKho;
                        nhapKhoDuocPhamChiTiet.TiLeTheoThapGia = duocPhamNhapChiTietCu.TiLeTheoThapGia;
                        //nhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId = duocPhamNhapChiTietCu.DuocPhamBenhVienPhanNhomId;

                        nhapKhoVatTuNew.NhapKhoVatTuChiTiets.Add(nhapKhoDuocPhamChiTiet);
                    }
                }
                BaseRepository.Add(nhapKhoVatTuNew);
            }
        }
        private NhapKhoVatTu MapNhapKhoVatTu(YeuCauNhapKhoVatTu yeuCauNhapKhoVatTu)
        {

            var nhapKhoVatTu = new NhapKhoVatTu();

            //nhapKhoVatTu.KhoId = (int)EnumLoaiKhoDuocPham.KhoTongVTYTCap2;
            nhapKhoVatTu.KhoId = yeuCauNhapKhoVatTu.KhoId;
            nhapKhoVatTu.SoChungTu = yeuCauNhapKhoVatTu.SoChungTu;
            nhapKhoVatTu.TenNguoiGiao = yeuCauNhapKhoVatTu.TenNguoiGiao;
            nhapKhoVatTu.NguoiGiaoId = yeuCauNhapKhoVatTu.NguoiGiaoId;
            nhapKhoVatTu.NguoiNhapId = yeuCauNhapKhoVatTu.NguoiNhapId;
            nhapKhoVatTu.LoaiNguoiGiao = yeuCauNhapKhoVatTu.LoaiNguoiGiao;
            nhapKhoVatTu.YeuCauNhapKhoVatTuId = yeuCauNhapKhoVatTu.Id;
            nhapKhoVatTu.NgayNhap = yeuCauNhapKhoVatTu.NgayNhap;

            foreach (var item in yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets)
            {
                var nhapKhoVTChiTiet = new NhapKhoVatTuChiTiet();
                nhapKhoVTChiTiet.NgayNhap = item.NgayNhap;
                nhapKhoVTChiTiet.HopDongThauVatTuId = item.HopDongThauVatTuId;
                nhapKhoVTChiTiet.LaVatTuBHYT = item.LaVatTuBHYT;
                nhapKhoVTChiTiet.Solo = item.Solo;
                nhapKhoVTChiTiet.HanSuDung = item.HanSuDung;
                nhapKhoVTChiTiet.SoLuongNhap = item.SoLuongNhap;
                nhapKhoVTChiTiet.DonGiaNhap = item.DonGiaNhap;
                nhapKhoVTChiTiet.TiLeTheoThapGia = item.TiLeTheoThapGia;
                nhapKhoVTChiTiet.VAT = item.VAT;
                nhapKhoVTChiTiet.MaVach = item.MaVach;
                nhapKhoVTChiTiet.MaRef = item.MaRef;
                nhapKhoVTChiTiet.KhoViTriId = item.KhoViTriId;
                nhapKhoVTChiTiet.MaVach = item.MaVach;
                nhapKhoVTChiTiet.NgayNhapVaoBenhVien = item.NgayNhap;
                if (nhapKhoVTChiTiet.KhoNhapSauKhiDuyetId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc)
                {
                    nhapKhoVTChiTiet.PhuongPhapTinhGiaTriTonKho = Enums.PhuongPhapTinhGiaTriTonKho.KhongApVAT;
                }
                else
                {
                    nhapKhoVTChiTiet.PhuongPhapTinhGiaTriTonKho = Enums.PhuongPhapTinhGiaTriTonKho.ApVAT;
                }
                nhapKhoVTChiTiet.VatTuBenhVienId = item.VatTuBenhVienId;
                nhapKhoVTChiTiet.TiLeBHYTThanhToan = item.TiLeBHYTThanhToan;
                nhapKhoVTChiTiet.KhoNhapSauKhiDuyetId = item.KhoNhapSauKhiDuyetId;
                nhapKhoVTChiTiet.NguoiNhapSauKhiDuyetId = item.NguoiNhapSauKhiDuyetId;
                if (item.KhoNhapSauKhiDuyetId != null && item.KhoNhapSauKhiDuyetId > 0)
                {
                    nhapKhoVTChiTiet.SoLuongDaXuat = item.SoLuongNhap;
                }


                nhapKhoVatTu.NhapKhoVatTuChiTiets.Add(nhapKhoVTChiTiet);
            }
            return nhapKhoVatTu;
        }
        public async Task<GridDataSource> GetDanhSachDuyetKhoVatTuForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {

            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(z => z.Id == 0).Select(s => new DanhSachDuyetKhoVatTuVo { });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachDuyetKhoVatTuVo>(queryInfo.AdditionalSearchString);
                IQueryable<DanhSachDuyetKhoVatTuVo> queryChoDuyet = null;
                IQueryable<DanhSachDuyetKhoVatTuVo> queryDaDuyet = null;
                IQueryable<DanhSachDuyetKhoVatTuVo> queryTuChoiDuyet = null;
                if (queryString.DangChoDuyet == false && queryString.DaDuyet == false && queryString.TuChoiDuyet == false)
                {
                    queryChoDuyet = Queryable(null, queryInfo.AdditionalSearchString);
                    queryDaDuyet = Queryable(true, queryInfo.AdditionalSearchString);
                    queryTuChoiDuyet = Queryable(false, queryInfo.AdditionalSearchString);
                }
                else
                {
                    if (queryString.DangChoDuyet == true)
                    {
                        queryChoDuyet = Queryable(null, queryInfo.AdditionalSearchString);
                    }
                    if (queryString.DaDuyet == true)
                    {
                        queryDaDuyet = Queryable(true, queryInfo.AdditionalSearchString);
                    }
                    if (queryString.TuChoiDuyet == true)
                    {
                        queryTuChoiDuyet = Queryable(false, queryInfo.AdditionalSearchString);
                    }
                }

                if (queryChoDuyet != null)
                {
                    query = query.Concat(queryChoDuyet);
                }
                if (queryDaDuyet != null)
                {
                    query = query.Concat(queryDaDuyet);
                }
                if (queryTuChoiDuyet != null)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
            }
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };

        }

        private IQueryable<DanhSachDuyetKhoVatTuVo> Queryable(bool? tinhTrang, string additionalSearchString)
        {
            var queryString = JsonConvert.DeserializeObject<DanhSachDuyetKhoVatTuVo>(additionalSearchString);
            var query = _yeuCauNhapKhoVatTuRepository.TableNoTracking
                            .Where(s => s.DuocKeToanDuyet == tinhTrang)
                                     .Select(s => new DanhSachDuyetKhoVatTuVo
                                     {
                                         Id = s.Id,
                                         SoChungTu = s.SoChungTu,
                                         NguoiNhapId = s.NguoiNhap.Id,
                                         TenNguoiNhap = s.NguoiNhap.User.HoTen,
                                         TinhTrang = s.DuocKeToanDuyet,
                                         NguoiGiao = s.NguoiGiao.User.HoTen,
                                         NguoiDuyet = s.NhanVienDuyet.User.HoTen,
                                         NgayNhap = s.NgayNhap,
                                         NgayDuyet = s.NgayDuyet,
                                         NgayHoaDon = s.NgayHoaDon,
                                         TenKho = string.Join("; ", s.YeuCauNhapKhoVatTuChiTiets.Select(z => z.KhoNhapSauKhiDuyet.Ten).Distinct()),
                                         DuyetLai = tinhTrang != null ? s.NhapKhoVatTus.Any(z => z.NhapKhoVatTuChiTiets.Any(x => x.XuatKhoVatTuChiTietViTris.Any(c => c.XuatKhoVatTuChiTiet.XuatKhoVatTu.NhapKhoVatTus.Any(v => v.NhapKhoVatTuChiTiets.Any(y => y.XuatKhoVatTuChiTietViTris.Any()))))) : (bool?)null,

                                         //BVHD-3926
                                         TenNhaCungCap = string.Join("; ", s.YeuCauNhapKhoVatTuChiTiets.Select(z => z.HopDongThauVatTu.NhaThau.Ten).Distinct())
                                     });

            if (queryString.RangeNhap != null &&
                            (!string.IsNullOrEmpty(queryString.RangeNhap.TuNgay) || !string.IsNullOrEmpty(queryString.RangeNhap.DenNgay)))
            {
                DateTime.TryParseExact(queryString.RangeNhap.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(queryString.RangeNhap.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeNhap.TuNgay) || p.NgayNhap >= tuNgay)
                                         && (string.IsNullOrEmpty(queryString.RangeNhap.DenNgay) || p.NgayNhap <= denNgay));
            }

            if (queryString.RangeDuyet != null &&
                        (!string.IsNullOrEmpty(queryString.RangeDuyet.TuNgay) || !string.IsNullOrEmpty(queryString.RangeDuyet.DenNgay)))
            {
                DateTime.TryParseExact(queryString.RangeDuyet.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(queryString.RangeDuyet.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeDuyet.TuNgay) || p.NgayDuyet >= tuNgay)
                                         && (string.IsNullOrEmpty(queryString.RangeDuyet.DenNgay) || p.NgayDuyet <= denNgay));
            }

            #region //BVHD-3926
            if (queryString.RangeHoaDon != null &&
                (!string.IsNullOrEmpty(queryString.RangeHoaDon.TuNgay) || !string.IsNullOrEmpty(queryString.RangeHoaDon.DenNgay)))
            {
                DateTime.TryParseExact(queryString.RangeHoaDon.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(queryString.RangeHoaDon.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeHoaDon.TuNgay) || (p.NgayHoaDon != null && p.NgayHoaDon >= tuNgay))
                                         && (string.IsNullOrEmpty(queryString.RangeHoaDon.DenNgay) || (p.NgayHoaDon != null && p.NgayHoaDon <= denNgay)));
            }
            #endregion

            //if (!string.IsNullOrEmpty(queryString.SearchString))
            //{
            //    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            //    query = query.ApplyLike(searchTerms,
            //         g => g.SoChungTu,
            //         g => g.TenKho,
            //         g => g.NguoiGiao,
            //         g => g.NguoiDuyet,
            //         g => g.TenNguoiNhap
            //   );
            //}

            #region //BVHD-3926
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchString = queryString.SearchString.Trim().ToLower().RemoveVietnameseDiacritics();
                query = query.Where(x =>
                    (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.ToLower().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.TenNhaCungCap) && x.TenNhaCungCap.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchString)));
            }
            #endregion
            return query;
        }
        public async Task<GridDataSource> GetTotalDanhSachDuyetKhoVatTuForGridAsync(QueryInfo queryInfo)
        {
            var query = _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(z => z.Id == 0).Select(s => new DanhSachDuyetKhoVatTuVo { });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachDuyetKhoVatTuVo>(queryInfo.AdditionalSearchString);
                IQueryable<DanhSachDuyetKhoVatTuVo> queryChoDuyet = null;
                IQueryable<DanhSachDuyetKhoVatTuVo> queryDaDuyet = null;
                IQueryable<DanhSachDuyetKhoVatTuVo> queryTuChoiDuyet = null;
                if (queryString.DangChoDuyet == false && queryString.DaDuyet == false && queryString.TuChoiDuyet == false)
                {
                    queryChoDuyet = Queryable(null, queryInfo.AdditionalSearchString);
                    queryDaDuyet = Queryable(true, queryInfo.AdditionalSearchString);
                    queryTuChoiDuyet = Queryable(false, queryInfo.AdditionalSearchString);
                }
                else
                {
                    if (queryString.DangChoDuyet == true)
                    {
                        queryChoDuyet = Queryable(null, queryInfo.AdditionalSearchString);
                    }
                    if (queryString.DaDuyet == true)
                    {
                        queryDaDuyet = Queryable(true, queryInfo.AdditionalSearchString);
                    }
                    if (queryString.TuChoiDuyet == true)
                    {
                        queryTuChoiDuyet = Queryable(false, queryInfo.AdditionalSearchString);
                    }
                }

                if (queryChoDuyet != null)
                {
                    query = query.Concat(queryChoDuyet);
                }
                if (queryDaDuyet != null)
                {
                    query = query.Concat(queryDaDuyet);
                }
                if (queryTuChoiDuyet != null)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
            }
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetDanhSachDuyetKhoVatTuChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            long yeuCauNhapKhoVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var query = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking.Where(cc => cc.YeuCauNhapKhoVatTuId == yeuCauNhapKhoVatTuId)
                                                               .Select(s => new DanhSachDuyetKhoVatTuChiTietVo
                                                               {
                                                                   Id = s.Id,
                                                                   VatTu = s.VatTuBenhVien.VatTus.Ten,
                                                                   NhaCungCap = s.HopDongThauVatTu.NhaThau.Ten,
                                                                   HopDongThau = (s.HopDongThauVatTu.HeThongTuPhatSinh != null && s.HopDongThauVatTu.HeThongTuPhatSinh == true) ? string.Empty : s.HopDongThauVatTu.SoHopDong,
                                                                   LoaiBHYT = s.LaVatTuBHYT,
                                                                   SoLo = s.Solo,
                                                                   HanSuDung = s.HanSuDung.ApplyFormatDate(),
                                                                   MaVach = s.MaVach,
                                                                   SLConLaiHD = (s.HopDongThauVatTu.HeThongTuPhatSinh != null && s.HopDongThauVatTu.HeThongTuPhatSinh == true) ? null :
                                                                   s.HopDongThauVatTu.HopDongThauVatTuChiTiets.Where(cc => cc.VatTuId == s.VatTuBenhVien.VatTus.Id).Sum(cc => cc.SoLuong - cc.SoLuongDaCap).ApplyNumber(),
                                                                   SoLuongNhap = s.SoLuongNhap.ApplyNumber(),
                                                                   DonGiaNhap = s.DonGiaNhap,
                                                                   VAT = s.VAT.ToString(),
                                                                   GiaBan = s.DonGiaBan,
                                                                   TiLeBHYTThanhToan = s.TiLeBHYTThanhToan,
                                                                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung != null ? s.VatTuBenhVien.LoaiSuDung.GetDescription() : "",
                                                               });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalDanhSachDuyetKhoVatTuChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long yeuCauNhapKhoVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var query = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking.Where(cc => cc.YeuCauNhapKhoVatTuId == yeuCauNhapKhoVatTuId)
                                                               .Select(s => new DanhSachDuyetKhoVatTuChiTietVo
                                                               {
                                                                   Id = s.Id,
                                                                   VatTu = s.VatTuBenhVien.VatTus.Ten,
                                                                   NhaCungCap = s.HopDongThauVatTu.NhaThau.Ten,
                                                                   HopDongThau = (s.HopDongThauVatTu.HeThongTuPhatSinh != null && s.HopDongThauVatTu.HeThongTuPhatSinh == true) ? string.Empty : s.HopDongThauVatTu.SoHopDong,
                                                                   LoaiBHYT = s.LaVatTuBHYT,
                                                                   SoLo = s.Solo,
                                                                   HanSuDung = s.HanSuDung.ApplyFormatDate(),
                                                                   MaVach = s.MaVach,
                                                                   SLConLaiHD = (s.HopDongThauVatTu.HeThongTuPhatSinh != null && s.HopDongThauVatTu.HeThongTuPhatSinh == true) ? null :
                                                                   s.HopDongThauVatTu.HopDongThauVatTuChiTiets.Where(cc => cc.VatTuId == s.VatTuBenhVien.VatTus.Id).Sum(cc => cc.SoLuong - cc.SoLuongDaCap).ApplyNumber(),
                                                                   SoLuongNhap = s.SoLuongNhap.ApplyNumber(),
                                                                   DonGiaNhap = s.DonGiaNhap,
                                                                   VAT = s.VAT.ToString(),
                                                                   GiaBan = s.DonGiaBan,
                                                                   TiLeBHYTThanhToan = s.TiLeBHYTThanhToan,
                                                                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung != null ? s.VatTuBenhVien.LoaiSuDung.GetDescription() : "",
                                                               });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<YeuCauNhapKhoVatTu> GetYeuCauNhapKhoVatTu(long id)
        {
            var yeuCauNhapKhoVatTu = await _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(o => o.Id == id)
                                                                              .Include(cc => cc.YeuCauNhapKhoVatTuChiTiets)
                                                                              .FirstOrDefaultAsync();
            return yeuCauNhapKhoVatTu;
        }

        #endregion      

    }
}