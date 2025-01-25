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
using Camino.Core.Domain.Entities.XuatKhos;
using System.Globalization;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;

namespace Camino.Services.NhapKhoDuocPhams
{
    public partial class NhapKhoDuocPhamService
    {
        #region Danh Sach Dược Phẩm Và Dược Phẩm Chi Tiết

        public async Task<ThongTinDuyetKhoDuocPham> GetThongTinDuyetKhoDuocPham(long yeuCauNhapKhoDuocPhamId)
        {
            var thongTinDuyetKhoDuocPham = await _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(cc => cc.Id == yeuCauNhapKhoDuocPhamId)
                                                                           .Select(cc => new ThongTinDuyetKhoDuocPham
                                                                           {
                                                                               NgayNhap = cc.NgayNhap,
                                                                               NguoiNhapId = cc.NguoiNhapId,
                                                                               SoChungTu = cc.SoChungTu,
                                                                               TenNguoiNhap = cc.NguoiNhap.User.HoTen,
                                                                               TinhTrang = cc.DuocKeToanDuyet,
                                                                               NguoiDuyetId = cc.NhanVienDuyetId,
                                                                               NguoiDuyet = cc.NhanVienDuyet.User.HoTen,
                                                                               NgayDuyet = cc.NgayDuyet
                                                                           }).FirstOrDefaultAsync();
            return thongTinDuyetKhoDuocPham;
        }
        public async Task<bool> TuChoiDuyetDuocPhamNhapKho(ThongTinLyDoHuyNhapKhoDuocPham thongTinLyDoHuyNhapKhoDuocPham)
        {
            var yeuCauNhapKhoDuocPham = await _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(o => o.Id == thongTinLyDoHuyNhapKhoDuocPham.YeuCauNhapKhoDuocPhamId).FirstOrDefaultAsync();

            if (yeuCauNhapKhoDuocPham != null)
            {
                yeuCauNhapKhoDuocPham.DuocKeToanDuyet = false;
                yeuCauNhapKhoDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauNhapKhoDuocPham.NgayDuyet = DateTime.Now;
                yeuCauNhapKhoDuocPham.LyDoKhongDuyet = thongTinLyDoHuyNhapKhoDuocPham.LyDoHuy;

                _yeuCauNhapKhoDuocPhamRepository.Update(yeuCauNhapKhoDuocPham);
                return true;
            }
            return false;
        }
        public async Task<string> DuyetDuocPhamNhapKho(long id)
        {
            var yeuCauNhapKhoDuocPham = await _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(o => o.Id == id)
                                                                              .Include(cc => cc.YeuCauNhapKhoDuocPhamChiTiets)
                                                                              .ThenInclude(cc => cc.DuocPhamBenhVien)
                                                                              .ThenInclude(c => c.DuocPham)
                                                                              .FirstOrDefaultAsync();

            if (yeuCauNhapKhoDuocPham.DuocKeToanDuyet == true)
                throw new Exception("Yêu cầu nhập kho dược phẩm đã được kế toán duyệt, Vui lòng tải lại trang!");

            var kiemTraSLNhapKhoDuocPham = KiemTraSoLuongNhap(yeuCauNhapKhoDuocPham);
            if (!String.IsNullOrEmpty(kiemTraSLNhapKhoDuocPham)) return kiemTraSLNhapKhoDuocPham;
            if (yeuCauNhapKhoDuocPham != null)
            {
                yeuCauNhapKhoDuocPham.DuocKeToanDuyet = true;
                yeuCauNhapKhoDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
                yeuCauNhapKhoDuocPham.NgayDuyet = DateTime.Now;
                _yeuCauNhapKhoDuocPhamRepository.Update(yeuCauNhapKhoDuocPham);
                var nhapKhoDuocPhamModel = MapNhapKhoDuocPham(yeuCauNhapKhoDuocPham);

                //Cập nhật số lượng hợp đồng thầu
                CapNhatSoLuongDaCapHopDongThau(yeuCauNhapKhoDuocPham);
                BaseRepository.Add(nhapKhoDuocPhamModel);
                XuatVeKhoSauKhiNhap(nhapKhoDuocPhamModel);
                return string.Empty;
            }
            return string.Empty;
        }

        public string KiemTraSoLuongNhap(YeuCauNhapKhoDuocPham yeuCauNhapKhoDuocPham)
        {
            var HopDongThauDuocPhamIds = yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Select(cc => cc.HopDongThauDuocPhamId).Distinct().ToList();
            var hopDongThauDuocPhamChiTiets = _hopDongThauDuocPhamChiTietRepository.Table.Include(cc => cc.DuocPham).Where(p => HopDongThauDuocPhamIds.Contains(p.HopDongThauDuocPhamId)).ToList();
            if (yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Any())
            {
                foreach (var hopDongThauDuocPhamChiTiet in hopDongThauDuocPhamChiTiets)
                {
                    var duocPhamIds = yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Where(cc => cc.HopDongThauDuocPhamId == hopDongThauDuocPhamChiTiet.HopDongThauDuocPhamId).Select(cc => cc.DuocPhamBenhVien.DuocPham.Id).Distinct();
                    if (duocPhamIds.Any())
                    {
                        foreach (var ycduocPhamId in duocPhamIds)
                        {
                            var groupDuocPhamBenhVienIds = yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Where(cc => cc.HopDongThauDuocPhamId == hopDongThauDuocPhamChiTiet.HopDongThauDuocPhamId).GroupBy(cc => cc.DuocPhamBenhVien.DuocPham.Id)
                                                                    .Select(c => new { DuocPhamId = c.Key, SoLuongNhap = c.Sum(ccc => ccc.SoLuongNhap) })
                                                                    .ToDictionary(cc => cc.DuocPhamId, cc => cc.SoLuongNhap);
                            if (groupDuocPhamBenhVienIds.Any())
                            {
                                if (hopDongThauDuocPhamChiTiet.DuocPhamId == ycduocPhamId && groupDuocPhamBenhVienIds[ycduocPhamId] > 0)
                                {
                                    if (hopDongThauDuocPhamChiTiet.SoLuong - hopDongThauDuocPhamChiTiet.SoLuongDaCap < groupDuocPhamBenhVienIds[ycduocPhamId])
                                    {
                                        return "Số Lượng nhập của " + hopDongThauDuocPhamChiTiet.DuocPham.Ten + " lớn hơn số lượng chưa nhập trong hợp đồng thầu, Bạn hãy kiểm tra lại.";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }
        public void CapNhatSoLuongDaCapHopDongThau(YeuCauNhapKhoDuocPham yeuCauNhapKhoDuocPham)
        {
            var HopDongThauDuocPhamIds = yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Select(cc => cc.HopDongThauDuocPhamId).Distinct().ToList();
            var hopDongThauDuocPhamChiTiets = _hopDongThauDuocPhamChiTietRepository.Table.Include(cc => cc.DuocPham).Where(p => HopDongThauDuocPhamIds.Contains(p.HopDongThauDuocPhamId)).ToList();
            if (yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Any())
            {
                foreach (var hopDongThauDuocPhamChiTiet in hopDongThauDuocPhamChiTiets)
                {
                    var duocPhamIds = yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Where(cc => cc.HopDongThauDuocPhamId == hopDongThauDuocPhamChiTiet.HopDongThauDuocPhamId).Select(cc => cc.DuocPhamBenhVien.DuocPham.Id).Distinct();
                    if (duocPhamIds.Any())
                    {
                        foreach (var ycduocPhamId in duocPhamIds)
                        {
                            var groupDuocPhamBenhVienIds = yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Where(cc => cc.HopDongThauDuocPhamId == hopDongThauDuocPhamChiTiet.HopDongThauDuocPhamId).GroupBy(cc => cc.DuocPhamBenhVien.DuocPham.Id)
                                                                    .Select(c => new { DuocPhamId = c.Key, SoLuongNhap = c.Sum(ccc => ccc.SoLuongNhap) })
                                                                    .ToDictionary(cc => cc.DuocPhamId, cc => cc.SoLuongNhap);
                            if (groupDuocPhamBenhVienIds.Any())
                            {
                                if (hopDongThauDuocPhamChiTiet.DuocPhamId == ycduocPhamId && groupDuocPhamBenhVienIds[ycduocPhamId] > 0)
                                {
                                    if (hopDongThauDuocPhamChiTiet.SoLuong - hopDongThauDuocPhamChiTiet.SoLuongDaCap >= groupDuocPhamBenhVienIds[ycduocPhamId])
                                    {
                                        hopDongThauDuocPhamChiTiet.SoLuongDaCap += groupDuocPhamBenhVienIds[ycduocPhamId];
                                        _hopDongThauDuocPhamChiTietRepository.Update(hopDongThauDuocPhamChiTiet);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void XuatVeKhoSauKhiNhap(NhapKhoDuocPham nhapKhoDuocPhamModel)
        {
            var listKhoNhapSauKhiDuyet = nhapKhoDuocPhamModel.NhapKhoDuocPhamChiTiets
                .Where(p => p.KhoNhapSauKhiDuyetId != null && p.KhoNhapSauKhiDuyetId > 0)
                .Select(p => p.KhoNhapSauKhiDuyetId).Distinct();
            if (listKhoNhapSauKhiDuyet != null && listKhoNhapSauKhiDuyet.Any())
            {
                foreach (var khoId in listKhoNhapSauKhiDuyet)
                {
                    var xuatKhoDuocPham = new XuatKhoDuocPham
                    {
                        LoaiXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                        LyDoXuatKho = Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet,
                        //TenNguoiNhan=
                        //NguoiNhanId
                        NguoiXuatId = nhapKhoDuocPhamModel.NguoiNhapId,
                        LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                        NgayXuat = DateTime.Now,
                        KhoXuatId = nhapKhoDuocPhamModel.KhoId,
                        KhoNhapId = (long)khoId

                    };
                    foreach (var item in nhapKhoDuocPhamModel.NhapKhoDuocPhamChiTiets.Where(p => p.KhoNhapSauKhiDuyetId == khoId))
                    {
                        xuatKhoDuocPham.NguoiNhanId = item.NguoiNhapSauKhiDuyetId ?? 1;
                        if (xuatKhoDuocPham.NguoiNhanId != null && xuatKhoDuocPham.NguoiNhanId > 0)
                        {
                            var nguoiNhapSauKhiDuyet = _userRepository.GetById((long)xuatKhoDuocPham.NguoiNhanId);
                            xuatKhoDuocPham.TenNguoiNhan = nguoiNhapSauKhiDuyet?.HoTen;
                        }
                        //                        var nhapKho =
                        //                            nhapKhoDuocPhamModel.NhapKhoDuocPhamChiTiets.FirstOrDefault(o =>
                        //                                o.DuocPhamBenhVienId == item.DuocPhamBenhVienId);
                        var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                            NgayXuat = DateTime.Now

                        };
                        xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(new XuatKhoDuocPhamChiTietViTri
                        {
                            NhapKhoDuocPhamChiTietId = item.Id,
                            SoLuongXuat = item.SoLuongNhap,
                            NgayXuat = DateTime.Now

                        });
                        xuatKhoDuocPham.XuatKhoDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);
                    }
                    _xuatKhoDuocPhamRepository.Add(xuatKhoDuocPham);

                    var nhapKhoDuocPham = new NhapKhoDuocPham
                    {
                        KhoId = (long)khoId,
                        XuatKhoDuocPhamId = xuatKhoDuocPham.Id,
                        SoChungTu = null,
                        TenNguoiGiao = xuatKhoDuocPham.NguoiXuat?.User?.HoTen,
                        NguoiGiaoId = xuatKhoDuocPham.NguoiXuatId,
                        NguoiNhapId = xuatKhoDuocPham.NguoiNhanId ?? 0,
                        DaHet = false,
                        NgayNhap = DateTime.Now,
                        LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong
                    };
                    foreach (var item in xuatKhoDuocPham.XuatKhoDuocPhamChiTiets)
                    {
                        foreach (var viTri in item.XuatKhoDuocPhamChiTietViTris)
                        {
                            var duocPhamNhapChiTietCu = nhapKhoDuocPhamModel.NhapKhoDuocPhamChiTiets.FirstOrDefault(o => o.Id == viTri.NhapKhoDuocPhamChiTietId);
                            if (duocPhamNhapChiTietCu != null)
                            {
                                var nhapKhoDuocPhamChiTiet = new NhapKhoDuocPhamChiTiet();
                                nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId = duocPhamNhapChiTietCu.HopDongThauDuocPhamId;
                                nhapKhoDuocPhamChiTiet.Solo = duocPhamNhapChiTietCu.Solo;

                                nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT = duocPhamNhapChiTietCu.LaDuocPhamBHYT;
                                nhapKhoDuocPhamChiTiet.HanSuDung = duocPhamNhapChiTietCu.HanSuDung;
                                nhapKhoDuocPhamChiTiet.SoLuongNhap = viTri.SoLuongXuat;
                                nhapKhoDuocPhamChiTiet.DonGiaNhap = duocPhamNhapChiTietCu.DonGiaNhap;
                                nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan = duocPhamNhapChiTietCu.TiLeBHYTThanhToan;

                                //nhapKhoDuocPhamChiTiet.DonGiaBan = item.DonGiaBan ?? duocPhamNhapChiTietCu.DonGiaBan;

                                nhapKhoDuocPhamChiTiet.VAT = duocPhamNhapChiTietCu.VAT;

                                //nhapKhoDuocPhamChiTiet.ChietKhau = item.ChietKhau ?? duocPhamNhapChiTietCu.ChietKhau;
                                nhapKhoDuocPhamChiTiet.MaVach = duocPhamNhapChiTietCu.MaVach;
                                nhapKhoDuocPhamChiTiet.MaRef = duocPhamNhapChiTietCu.MaRef;
                                nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId = duocPhamNhapChiTietCu.DuocPhamBenhVienId;
                                //need update
                                //nhapKhoDuocPhamChiTiet.KhoDuocPhamViTriId = null;
                                nhapKhoDuocPhamChiTiet.SoLuongDaXuat = 0;
                                nhapKhoDuocPhamChiTiet.NgayNhap = DateTime.Now;

                                //
                                nhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien = duocPhamNhapChiTietCu.NgayNhapVaoBenhVien;
                                nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho = duocPhamNhapChiTietCu.PhuongPhapTinhGiaTriTonKho;
                                nhapKhoDuocPhamChiTiet.TiLeTheoThapGia = duocPhamNhapChiTietCu.TiLeTheoThapGia;
                                nhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId = duocPhamNhapChiTietCu.DuocPhamBenhVienPhanNhomId;

                                nhapKhoDuocPham.NhapKhoDuocPhamChiTiets.Add(nhapKhoDuocPhamChiTiet);
                            }
                        }
                    }
                    _nhapKhoDuocPhamRepository.Add(nhapKhoDuocPham);
                }
            }

        }

        private NhapKhoDuocPham MapNhapKhoDuocPham(YeuCauNhapKhoDuocPham yeuCauNhapKhoDuocPham)
        {

            var nhapKhoDuocPham = new NhapKhoDuocPham();
            var nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>();

            nhapKhoDuocPham.KhoId = yeuCauNhapKhoDuocPham.KhoId;
            nhapKhoDuocPham.SoChungTu = yeuCauNhapKhoDuocPham.SoChungTu;
            nhapKhoDuocPham.TenNguoiGiao = yeuCauNhapKhoDuocPham.TenNguoiGiao;
            nhapKhoDuocPham.NguoiGiaoId = yeuCauNhapKhoDuocPham.NguoiGiaoId;
            nhapKhoDuocPham.NguoiNhapId = yeuCauNhapKhoDuocPham.NguoiNhapId;
            nhapKhoDuocPham.LoaiNguoiGiao = yeuCauNhapKhoDuocPham.LoaiNguoiGiao;
            nhapKhoDuocPham.YeuCauNhapKhoDuocPhamId = yeuCauNhapKhoDuocPham.Id;
            nhapKhoDuocPham.NgayNhap = yeuCauNhapKhoDuocPham.NgayNhap;

            foreach (var item in yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets)
            {
                var nhapKhoDuocPhamChiTiet = new NhapKhoDuocPhamChiTiet();
                nhapKhoDuocPhamChiTiet.NgayNhap = item.NgayNhap;
                //nhapKhoDuocPhamChiTiet.NhapKhoDuocPhamId = nhapKhoDuocPham.Id;
                nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId = item.HopDongThauDuocPhamId;
                nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT = item.LaDuocPhamBHYT;
                nhapKhoDuocPhamChiTiet.DuocPhamBenhVienPhanNhomId = item.DuocPhamBenhVienPhanNhomId;
                nhapKhoDuocPhamChiTiet.Solo = item.Solo;
                nhapKhoDuocPhamChiTiet.HanSuDung = item.HanSuDung;
                nhapKhoDuocPhamChiTiet.SoLuongNhap = item.SoLuongNhap;
                nhapKhoDuocPhamChiTiet.DonGiaNhap = item.DonGiaNhap;
                nhapKhoDuocPhamChiTiet.TiLeTheoThapGia = item.TiLeTheoThapGia;
                nhapKhoDuocPhamChiTiet.VAT = item.VAT;
                nhapKhoDuocPhamChiTiet.MaVach = item.MaVach;
                nhapKhoDuocPhamChiTiet.KhoViTriId = item.KhoViTriId;
                nhapKhoDuocPhamChiTiet.MaRef = item.MaRef;
                nhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien = item.NgayNhap;
                nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId = item.DuocPhamBenhVienId;
                nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan = item.TiLeBHYTThanhToan;
                nhapKhoDuocPhamChiTiet.KhoNhapSauKhiDuyetId = item.KhoNhapSauKhiDuyetId;
                if (nhapKhoDuocPhamChiTiet.KhoNhapSauKhiDuyetId == (long)Enums.EnumKhoDuocPham.KhoNhaThuoc)
                {
                    nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho = Enums.PhuongPhapTinhGiaTriTonKho.KhongApVAT;
                }
                else
                {
                    nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho = Enums.PhuongPhapTinhGiaTriTonKho.ApVAT;
                }
                nhapKhoDuocPhamChiTiet.NguoiNhapSauKhiDuyetId = item.NguoiNhapSauKhiDuyetId;
                if (item.KhoNhapSauKhiDuyetId != null && item.KhoNhapSauKhiDuyetId > 0)
                {
                    nhapKhoDuocPhamChiTiet.SoLuongDaXuat = item.SoLuongNhap;
                }

                nhapKhoDuocPham.NhapKhoDuocPhamChiTiets.Add(nhapKhoDuocPhamChiTiet);
            }

            return nhapKhoDuocPham;
        }

        public async Task<GridDataSource> GetDanhSachDuyetKhoDuocPhamForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking;
            //var query = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(z => z.Id == 0).Select(s => new DanhSachDuyetKhoDuocPhamVo { });
            DanhSachDuyetKhoDuocPhamVo queryObject = new DanhSachDuyetKhoDuocPhamVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DanhSachDuyetKhoDuocPhamVo>(queryInfo.AdditionalSearchString);

                if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
                {
                    queryObject.DaDuyet = true;
                    queryObject.DangChoDuyet = true;
                    queryObject.TuChoiDuyet = true;
                }
                if (queryObject.DaDuyet == false || queryObject.DangChoDuyet == false || queryObject.TuChoiDuyet == false)
                {
                    query = query.Where(o => (queryObject.DangChoDuyet == true && o.DuocKeToanDuyet == null)
                                            || (queryObject.DaDuyet == true && o.DuocKeToanDuyet == true)
                                            || (queryObject.TuChoiDuyet == true && o.DuocKeToanDuyet == false));
                }
                if(queryObject != null)
                {
                    if (queryObject.RangeNhap != null &&
                            (!string.IsNullOrEmpty(queryObject.RangeNhap.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeNhap.DenNgay)))
                    {
                        DateTime.TryParseExact(queryObject.RangeNhap.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                        DateTime.TryParseExact(queryObject.RangeNhap.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                        denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                        query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeNhap.TuNgay) || p.NgayNhap >= tuNgay)
                                                 && (string.IsNullOrEmpty(queryObject.RangeNhap.DenNgay) || p.NgayNhap <= denNgay));
                    }

                    if (queryObject.RangeDuyet != null &&
                                (!string.IsNullOrEmpty(queryObject.RangeDuyet.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeDuyet.DenNgay)))
                    {
                        DateTime.TryParseExact(queryObject.RangeDuyet.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                        DateTime.TryParseExact(queryObject.RangeDuyet.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                        denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                        query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeDuyet.TuNgay) || p.NgayDuyet >= tuNgay)
                                                 && (string.IsNullOrEmpty(queryObject.RangeDuyet.DenNgay) || p.NgayDuyet <= denNgay));
                    }

                    #region //BVHD-3926
                    if (queryObject.RangeHoaDon != null &&
                        (!string.IsNullOrEmpty(queryObject.RangeHoaDon.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeHoaDon.DenNgay)))
                    {
                        DateTime.TryParseExact(queryObject.RangeHoaDon.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                        DateTime.TryParseExact(queryObject.RangeHoaDon.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                        denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                        query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeHoaDon.TuNgay) || (p.NgayHoaDon != null && p.NgayHoaDon >= tuNgay))
                                                 && (string.IsNullOrEmpty(queryObject.RangeHoaDon.DenNgay) || (p.NgayHoaDon != null && p.NgayHoaDon <= denNgay)));
                    }


                    #endregion

                    //if (!string.IsNullOrEmpty(queryString.SearchString))
                    //{
                    //    var searchString = queryString.SearchString.Trim().ToLower().RemoveVietnameseDiacritics();
                    //    query = query.Where(x =>
                    //        (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.ToLower().Contains(searchString))
                    //        || (!string.IsNullOrEmpty(x.TenNhaCungCap) && x.TenNhaCungCap.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchString)));
                    //}
                }
            }

            var allDataNhapKhoDuocPhamGripVo = query
            .Select(s => new DanhSachDuyetKhoDuocPhamVo
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
                DataYeuCauNhapKhoDuocPhamChiTiets = s.YeuCauNhapKhoDuocPhamChiTiets.Select(ct => new DataYeuCauNhapKhoDuocPhamChiTiet
                {
                    Id = ct.Id,
                    KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                    HopDongThauDuocPhamId = ct.HopDongThauDuocPhamId
                }).ToList()

                //TenKho = string.Join("; ", s.YeuCauNhapKhoDuocPhamChiTiets.Select(z => z.KhoNhapSauKhiDuyet.Ten).Distinct()),
                //DuyetLai = tinhTrang != true ? s.NhapKhoDuocPhams.Any(z => z.NhapKhoDuocPhamChiTiets.Any(x => x.XuatKhoDuocPhamChiTietViTris.Any(c => c.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NhapKhoDuocPhams.Any(v => v.NhapKhoDuocPhamChiTiets.Any(y => y.XuatKhoDuocPhamChiTietViTris.Any()))))) : (bool?)null,

                //BVHD-3926
                //TenNhaCungCap = string.Join("; ", s.YeuCauNhapKhoDuocPhamChiTiets.Select(z => z.HopDongThauDuocPham.NhaThau.Ten).Distinct())
            }).ToList();

            var dataKho = _khoRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var hopDongThauDuocPhamIds = allDataNhapKhoDuocPhamGripVo.SelectMany(o => o.DataYeuCauNhapKhoDuocPhamChiTiets).Select(ct => ct.HopDongThauDuocPhamId).Distinct().ToList();
            var dataHopDongThauDuocPham = _hopDongThauDuocPhamRepository.TableNoTracking
                .Where(o => hopDongThauDuocPhamIds.Contains(o.Id))
                .Select(o => new { o.Id, TenNhaThau = o.NhaThau.Ten })
                .ToList();
            foreach (var dataNhapKhoDuocPhamGripVo in allDataNhapKhoDuocPhamGripVo)
            {
                var khoNhapSauKhiDuyetIds = dataNhapKhoDuocPhamGripVo.DataYeuCauNhapKhoDuocPhamChiTiets
                    .Where(o => o.KhoNhapSauKhiDuyetId != null)
                    .Select(o => o.KhoNhapSauKhiDuyetId)
                    .Distinct().ToList();
                dataNhapKhoDuocPhamGripVo.TenKho = string.Join("; ", khoNhapSauKhiDuyetIds.Select(o => dataKho.FirstOrDefault(k => k.Id == o)?.Ten ?? ""));
                var hopDongThauIds = dataNhapKhoDuocPhamGripVo.DataYeuCauNhapKhoDuocPhamChiTiets
                    .Select(o => o.HopDongThauDuocPhamId)
                    .Distinct().ToList();
                dataNhapKhoDuocPhamGripVo.TenNhaCungCap = string.Join("; ", hopDongThauIds.Select(o => dataHopDongThauDuocPham.FirstOrDefault(h => h.Id == o)?.TenNhaThau ?? ""));
            }

            if (queryObject != null && !string.IsNullOrEmpty(queryObject.SearchString))
            {
                var searchString = queryObject.SearchString.Trim().ToLower().RemoveVietnameseDiacritics();
                allDataNhapKhoDuocPhamGripVo = allDataNhapKhoDuocPhamGripVo.Where(x =>
                    (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.ToLower().RemoveVietnameseDiacritics().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.TenNhaCungCap) && x.TenNhaCungCap.ToLower().RemoveVietnameseDiacritics().Contains(searchString)))
                    .ToList();
            }

            return new GridDataSource { Data = allDataNhapKhoDuocPhamGripVo.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = allDataNhapKhoDuocPhamGripVo.Count() };
        }

        public async Task<GridDataSource> GetDanhSachDuyetKhoDuocPhamForGridAsyncOld(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(z => z.Id == 0).Select(s => new DanhSachDuyetKhoDuocPhamVo { });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachDuyetKhoDuocPhamVo>(queryInfo.AdditionalSearchString);
                IQueryable<DanhSachDuyetKhoDuocPhamVo> queryChoDuyet = null;
                IQueryable<DanhSachDuyetKhoDuocPhamVo> queryDaDuyet = null;
                IQueryable<DanhSachDuyetKhoDuocPhamVo> queryTuChoiDuyet = null;
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
        private IQueryable<DanhSachDuyetKhoDuocPhamVo> Queryable(bool? tinhTrang, string additionalSearchString)
        {
            var queryString = JsonConvert.DeserializeObject<DanhSachDuyetKhoDuocPhamVo>(additionalSearchString);
            var query = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking
                                      .Where(s => s.DuocKeToanDuyet == tinhTrang)
                                      .Select(s => new DanhSachDuyetKhoDuocPhamVo
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
                                          TenKho = string.Join("; ", s.YeuCauNhapKhoDuocPhamChiTiets.Select(z => z.KhoNhapSauKhiDuyet.Ten).Distinct()),
                                          DuyetLai = tinhTrang != true ? s.NhapKhoDuocPhams.Any(z => z.NhapKhoDuocPhamChiTiets.Any(x => x.XuatKhoDuocPhamChiTietViTris.Any(c => c.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NhapKhoDuocPhams.Any(v => v.NhapKhoDuocPhamChiTiets.Any(y => y.XuatKhoDuocPhamChiTietViTris.Any()))))) : (bool?)null,

                                          //BVHD-3926
                                          TenNhaCungCap = string.Join("; ", s.YeuCauNhapKhoDuocPhamChiTiets.Select(z => z.HopDongThauDuocPham.NhaThau.Ten).Distinct())
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
                    //|| (!string.IsNullOrEmpty(x.TenKho) && x.TenKho.ToLower().Contains(searchString))
                    //|| (!string.IsNullOrEmpty(x.NguoiGiao) && x.NguoiGiao.ToLower().Contains(searchString))
                    //|| (!string.IsNullOrEmpty(x.NguoiDuyet) && x.NguoiDuyet.ToLower().Contains(searchString))
                    //|| (!string.IsNullOrEmpty(x.TenNguoiNhap) && x.TenNguoiNhap.ToLower().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.TenNhaCungCap) && x.TenNhaCungCap.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchString)));
            }
            #endregion
            return query;
        }

        public async Task<GridDataSource> GetTotalDanhSachDuyetKhoDuocPhamForGridAsync(QueryInfo queryInfo)
        {
            var query = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(z => z.Id == 0).Select(s => new DanhSachDuyetKhoDuocPhamVo { });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachDuyetKhoDuocPhamVo>(queryInfo.AdditionalSearchString);
                IQueryable<DanhSachDuyetKhoDuocPhamVo> queryChoDuyet = null;
                IQueryable<DanhSachDuyetKhoDuocPhamVo> queryDaDuyet = null;
                IQueryable<DanhSachDuyetKhoDuocPhamVo> queryTuChoiDuyet = null;
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


        public async Task<GridDataSource> GetDanhSachDuyetKhoDuocPhamChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            long yeuCauNhapKhoDuocPhamId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var query = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(cc => cc.YeuCauNhapKhoDuocPhamId == yeuCauNhapKhoDuocPhamId)
                                                               .Select(s => new DanhSachDuyetKhoDuocPhamChiTietVo
                                                               {
                                                                   Id = s.Id,
                                                                   DuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                                                                   NhaCungCap = s.HopDongThauDuocPham.NhaThau.Ten,
                                                                   HopDongThau = (s.HopDongThauDuocPham.HeThongTuPhatSinh != null && s.HopDongThauDuocPham.HeThongTuPhatSinh == true) ? string.Empty : s.HopDongThauDuocPham.SoHopDong,
                                                                   LoaiBHYT = s.LaDuocPhamBHYT,
                                                                   Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                                                                   SoLo = s.Solo,
                                                                   HanSuDung = s.HanSuDung.ApplyFormatDate(),
                                                                   MaVach = s.MaVach,
                                                                   SLConLaiHD = (s.HopDongThauDuocPham.HeThongTuPhatSinh != null && s.HopDongThauDuocPham.HeThongTuPhatSinh == true) ? null : s.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Where(cc => cc.DuocPhamId == s.DuocPhamBenhVien.DuocPham.Id).Sum(cc => cc.SoLuong - cc.SoLuongDaCap).ApplyNumber(),
                                                                   SoLuongNhap = s.SoLuongNhap.ApplyNumber(),
                                                                   DonGiaNhap = s.DonGiaNhap,
                                                                   VAT = s.VAT.ToString(),
                                                                   ThapGia = s.TiLeTheoThapGia.ToString(),
                                                                   GiaBan = s.DonGiaBan,
                                                                   TiLeBHYTThanhToan = s.TiLeBHYTThanhToan,
                                                               });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalDanhSachDuyetKhoDuocPhamChiTietForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long yeuCauNhapKhoDuocPhamId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var query = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(cc => cc.YeuCauNhapKhoDuocPhamId == yeuCauNhapKhoDuocPhamId)
                                                               .Select(s => new DanhSachDuyetKhoDuocPhamChiTietVo
                                                               {
                                                                   Id = s.Id,
                                                                   NhaCungCap = s.HopDongThauDuocPham.NhaThau.Ten,
                                                                   HopDongThau = (s.HopDongThauDuocPham.HeThongTuPhatSinh != null && s.HopDongThauDuocPham.HeThongTuPhatSinh == true) ? string.Empty : s.HopDongThauDuocPham.SoHopDong,
                                                                   LoaiBHYT = s.LaDuocPhamBHYT,
                                                                   Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten,
                                                                   SoLo = s.Solo,
                                                                   HanSuDung = s.HanSuDung.ApplyFormatDate(),
                                                                   MaVach = s.MaVach,
                                                                   SLConLaiHD = (s.HopDongThauDuocPham.HeThongTuPhatSinh != null && s.HopDongThauDuocPham.HeThongTuPhatSinh == true) ? null : s.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Where(cc => cc.DuocPhamId == s.DuocPhamBenhVien.DuocPham.Id).Sum(cc => cc.SoLuong - cc.SoLuongDaCap).ApplyNumber(),
                                                                   SoLuongNhap = s.SoLuongNhap.ApplyNumber(),
                                                                   DonGiaNhap = s.DonGiaNhap,
                                                                   VAT = s.VAT.ToString(),
                                                                   GiaBan = s.DonGiaBan,
                                                                   TiLeBHYTThanhToan = s.TiLeBHYTThanhToan,
                                                               });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<YeuCauNhapKhoDuocPham> GetYeuCauNhapKhoDuocPham(long id)
        {
            var yeuCauNhapKhoDuocPham = await _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Where(o => o.Id == id)
                                                                              .Include(cc => cc.YeuCauNhapKhoDuocPhamChiTiets)
                                                                              .FirstOrDefaultAsync();
            return yeuCauNhapKhoDuocPham;
        }

        #endregion

        public async Task<string> HuyDuyetNhapKhoDuocPham(long yeuCauNhapKhoDuocPhamId)
        {
            var yeuCauNhapKhoDuocPham = _yeuCauNhapKhoDuocPhamRepository.GetById(yeuCauNhapKhoDuocPhamId, x => x
                    .Include(o => o.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(o => o.HopDongThauDuocPham).ThenInclude(o => o.HopDongThauDuocPhamChiTiets)
                    .Include(o => o.NhapKhoDuocPhams).ThenInclude(o => o.NhapKhoDuocPhamChiTiets)
                    .ThenInclude(o => o.XuatKhoDuocPhamChiTietViTris).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                    .ThenInclude(o => o.NhapKhoDuocPhams).ThenInclude(o => o.NhapKhoDuocPhamChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTris));

            foreach (var nhapKhoDuocPhamChiTiet in yeuCauNhapKhoDuocPham.NhapKhoDuocPhams.SelectMany(o => o.NhapKhoDuocPhamChiTiets))
            {
                if (nhapKhoDuocPhamChiTiet.KhoNhapSauKhiDuyetId == null && nhapKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Any())
                {
                    return "Dược phẩm đã được dùng sau khi duyệt";
                }
                else if (nhapKhoDuocPhamChiTiet.KhoNhapSauKhiDuyetId != null)
                {
                    var xuatKhoDuocPhams = nhapKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(o => o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham);
                    if (xuatKhoDuocPhams.Any(o => o.NhapKhoDuocPhams.Any(n => n.NhapKhoDuocPhamChiTiets.Any(ct => ct.XuatKhoDuocPhamChiTietViTris.Any()))))
                    {
                        return "Dược phẩm đã được dùng sau khi duyệt";
                    }
                }
            }
            yeuCauNhapKhoDuocPham.DuocKeToanDuyet = null;
            yeuCauNhapKhoDuocPham.NhanVienDuyetId = null;
            yeuCauNhapKhoDuocPham.NgayDuyet = null;

            if (yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets.Any())
            {
                foreach (var yeuCauNhapKhoDuocPhamChiTiet in yeuCauNhapKhoDuocPham.YeuCauNhapKhoDuocPhamChiTiets)
                {
                    var hopDongThauDuocPhamChiTiet = yeuCauNhapKhoDuocPhamChiTiet.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.FirstOrDefault(o => o.DuocPhamId == yeuCauNhapKhoDuocPhamChiTiet.DuocPhamBenhVienId);
                    if (hopDongThauDuocPhamChiTiet != null)
                    {
                        hopDongThauDuocPhamChiTiet.SoLuongDaCap -= yeuCauNhapKhoDuocPhamChiTiet.SoLuongNhap;
                    }
                }
            }
            foreach (var nhapKhoDuocPham in yeuCauNhapKhoDuocPham.NhapKhoDuocPhams)
            {
                nhapKhoDuocPham.WillDelete = true;
                foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPham.NhapKhoDuocPhamChiTiets)
                {
                    if (nhapKhoDuocPhamChiTiet.KhoNhapSauKhiDuyetId != null)
                    {
                        foreach (var xuatKhoDuocPhamChiTietViTri in nhapKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
                        {
                            xuatKhoDuocPhamChiTietViTri.WillDelete = true;
                            xuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                            if (xuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
                            {
                                xuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;
                                foreach (var nhapKhoDuocPhamSauKhiDuyet in xuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NhapKhoDuocPhams)
                                {
                                    nhapKhoDuocPhamSauKhiDuyet.WillDelete = true;
                                }
                            }
                        }
                    }
                }
            }
            await BaseRepository.Context.SaveChangesAsync();
            return string.Empty;
        }
        public async Task<string> HuyDuyetNhapKhoVatTu(long yeuCauNhapKhoVatTuId)
        {
            var yeuCauNhapKhoVatTu = _yeuCauNhapKhoVatTuRepository.GetById(yeuCauNhapKhoVatTuId, x => x
                    .Include(o => o.YeuCauNhapKhoVatTuChiTiets).ThenInclude(o => o.HopDongThauVatTu).ThenInclude(o => o.HopDongThauVatTuChiTiets)
                    .Include(o => o.NhapKhoVatTus).ThenInclude(o => o.NhapKhoVatTuChiTiets)
                    .ThenInclude(o => o.XuatKhoVatTuChiTietViTris).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
                    .ThenInclude(o => o.NhapKhoVatTus).ThenInclude(o => o.NhapKhoVatTuChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTris));

            foreach (var nhapKhoVatTuChiTiet in yeuCauNhapKhoVatTu.NhapKhoVatTus.SelectMany(o => o.NhapKhoVatTuChiTiets))
            {
                if (nhapKhoVatTuChiTiet.KhoNhapSauKhiDuyetId == null && nhapKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Any())
                {
                    return "Vật tư đã được dùng sau khi duyệt";
                }
                else if (nhapKhoVatTuChiTiet.KhoNhapSauKhiDuyetId != null)
                {
                    var xuatKhoVatTus = nhapKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Select(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTu);
                    if (xuatKhoVatTus.Any(o => o.NhapKhoVatTus.Any(n => n.NhapKhoVatTuChiTiets.Any(ct => ct.XuatKhoVatTuChiTietViTris.Any()))))
                    {
                        return "Vật tư đã được dùng sau khi duyệt";
                    }
                }
            }
            yeuCauNhapKhoVatTu.DuocKeToanDuyet = null;
            yeuCauNhapKhoVatTu.NhanVienDuyetId = null;
            yeuCauNhapKhoVatTu.NgayDuyet = null;

            if (yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets.Any())
            {
                foreach (var yeuCauNhapKhoVatTuChiTiet in yeuCauNhapKhoVatTu.YeuCauNhapKhoVatTuChiTiets)
                {
                    var hopDongThauVatTuChiTiet = yeuCauNhapKhoVatTuChiTiet.HopDongThauVatTu.HopDongThauVatTuChiTiets.FirstOrDefault(o => o.VatTuId == yeuCauNhapKhoVatTuChiTiet.VatTuBenhVienId);
                    if (hopDongThauVatTuChiTiet != null)
                    {
                        hopDongThauVatTuChiTiet.SoLuongDaCap -= yeuCauNhapKhoVatTuChiTiet.SoLuongNhap;
                    }
                }
            }
            foreach (var nhapKhoVatTu in yeuCauNhapKhoVatTu.NhapKhoVatTus)
            {
                nhapKhoVatTu.WillDelete = true;

                foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTu.NhapKhoVatTuChiTiets)
                {
                    if (nhapKhoVatTuChiTiet.KhoNhapSauKhiDuyetId != null)
                    {
                        foreach (var xuatKhoVatTuChiTietViTri in nhapKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris)
                        {
                            xuatKhoVatTuChiTietViTri.WillDelete = true;
                            xuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                            if (xuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null)
                            {
                                xuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.WillDelete = true;
                                foreach (var nhapKhoVatTuSauKhiDuyet in xuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.NhapKhoVatTus)
                                {
                                    nhapKhoVatTuSauKhiDuyet.WillDelete = true;
                                }
                            }
                        }
                    }
                }

            }
            await _yeuCauNhapKhoVatTuRepository.Context.SaveChangesAsync();
            return string.Empty;
        }

    }
}