using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Helpers;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCao;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Data;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoKetQuaKhamChuaBenh;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiThuoc;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoVienPhiThuTien;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoThongKeDonThuoc;
using System.Runtime.InteropServices;
using OfficeOpenXml.Drawing;
using System.Net;
using System.Text;
using Camino.Core.Domain.ValueObject.BaoCaoLuuketQuaXeNghiemTrongNgay;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        #region Báo cáo thu tiền 

        #region Báo cáo thu tiền viện phí

        private async Task<List<BaoCaoThuPhiVienPhiGridVo>> GetAllForBaoCaoThuPhiVienPhi(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            queryInfo.PhongBenhVienId = 0;//khong loc theo phong
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var allPhieuThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan &&
                             (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) &&
                             (queryInfo.TuNgay == null || tuNgay < o.NgayThu || tuNgay < o.NgayHuy) &&
                             (queryInfo.DenNgay == null || o.NgayThu <= denNgay || o.NgayHuy <= denNgay) &&
                             (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienThucHienId == queryInfo.NhanVienId || (o.DaHuy == true && o.NhanVienHuyId == queryInfo.NhanVienId)))
                .Select(o => new
                {
                    Id = o.NhanVienThucHienId,
                    LoaiThuTienBenhNhan = o.LoaiThuTienBenhNhan,
                    //LaPhieuHuy = false,
                    //MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                    NgayThu = o.NgayThu,
                    DaHuy = o.DaHuy,
                    NgayHuy = o.NgayHuy,
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    //TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                    //NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                    //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                    //MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                    //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                    GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,
                    SoBLHD = o.SoPhieuHienThi,
                    PhieuChis = o.TaiKhoanBenhNhanChis.Select(chi => new PhieuChiDataVo { LoaiChiTienBenhNhan = chi.LoaiChiTienBenhNhan, TienChiPhi = chi.TienChiPhi }).ToList(),
                    //CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                    CongNoCaNhan = o.CongNo.GetValueOrDefault(),
                    SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                    SoTienThuChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                    SoTienThuPos = o.POS.GetValueOrDefault(),
                    //SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,
                    SoPhieuThuGhiNoId = o.ThuNoPhieuThuId,
                    //NguoiThu = o.NhanVienThucHien.User.HoTen,
                    NhanVienThucHienId = o.NhanVienThucHienId,
                    NhanVienHuyId = o.NhanVienHuyId,
                    //ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.CongTyBaoHiemTuNhan.Ten).ToList(),
                    CongTyBaoHiemTuNhanCongNos = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => new { cntn.CongTyBaoHiemTuNhan.Ten, cntn.SoTien }).ToList()
                }).ToList();

            var allPhieuChi = _taiKhoanBenhNhanChiRepository.TableNoTracking
                .Where(o => (o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) &&
                            (queryInfo.TuNgay == null || tuNgay < o.NgayChi || tuNgay < o.NgayHuy) &&
                            (queryInfo.DenNgay == null || o.NgayChi <= denNgay || o.NgayHuy <= denNgay) &&
                            (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienThucHienId == queryInfo.NhanVienId || (o.DaHuy == true && o.NhanVienHuyId == queryInfo.NhanVienId)))
                .Select(o => new
                {
                    Id = o.NhanVienThucHienId.GetValueOrDefault(),
                    LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                    NgayChi = o.NgayChi,
                    NgayHuy = o.NgayHuy,
                    DaHuy = o.DaHuy,
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    //LaPhieuHuy = false,
                    //MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,

                    //TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                    //NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                    //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                    //MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                    //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                    GoiDichVu = o.YeuCauGoiDichVuId != null,
                    SoBLHD = o.SoPhieuHienThi,
                    SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                    NhanVienThucHienId = o.NhanVienThucHienId,
                    NhanVienHuyId = o.NhanVienHuyId
                    //NguoiThu = o.NhanVienThucHien != null ? o.NhanVienThucHien.User.HoTen : string.Empty,
                }).ToList();

            var soPhieuThuGhiNoIds = allPhieuThu.Select(o => o.SoPhieuThuGhiNoId.GetValueOrDefault()).Distinct().ToList();
            var soPhieuThuGhiNos = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => soPhieuThuGhiNoIds.Contains(o.Id)).Select(o => new { o.Id, o.SoPhieuHienThi }).ToList();

            var yeuCauTiepNhanIds = allPhieuThu.Select(o => o.YeuCauTiepNhanId).ToList().Concat(allPhieuChi.Where(o=> o.YeuCauTiepNhanId != null).Select(o => o.YeuCauTiepNhanId.Value).ToList()).Distinct().ToList();

            var yeuCauTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.Id))
                .Select(o => new
                {
                    Id = o.Id,
                    YeuCauTiepNhanNgoaiTruCanQuyetToanId = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    MaBenhNhan = o.BenhNhan.MaBN,
                    TenBenhNhan = o.HoTen,
                    NamSinh = o.NamSinh,
                    NguoiGioiThieu = o.NoiGioiThieuId != null ? o.NoiGioiThieu.Ten : "",
                    MaYeuCauTiepNhan = o.MaYeuCauTiepNhan,
                    SoBenhAn = (o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.NoiTruBenhAn != null) ? o.NoiTruBenhAn.SoBenhAn : "",
                    YeuCauTiepNhanMeId = o.YeuCauNhapVien != null ? o.YeuCauNhapVien.YeuCauTiepNhanMeId : null,
                }).ToList();
            var hotenUsers = _userRepository.TableNoTracking.Select(o => new { o.Id, o.HoTen }).ToList();

            var allData = new List<BaoCaoThuPhiVienPhiGridVo>();
            var phieuThus = allPhieuThu
                .Where(o => (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) &&
                            (queryInfo.TuNgay == null || tuNgay < o.NgayThu) &&
                            (queryInfo.DenNgay == null || o.NgayThu <= denNgay) &&
                            (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienThucHienId == queryInfo.NhanVienId))
                .Select(o => new BaoCaoThuPhiVienPhiGridVo
                {
                    Id = o.Id,
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    LoaiThuTienBenhNhan = o.LoaiThuTienBenhNhan,
                    LaPhieuHuy = false,                    
                    NgayThu = o.NgayThu,
                    //MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                    //TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                    //NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                    //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                    //MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                    //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                    GoiDichVu = o.GoiDichVu,
                    SoBLHD = o.SoBLHD,
                    PhieuChis = o.PhieuChis,
                    CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                    CongNoCaNhan = o.CongNoCaNhan,
                    SoTienThuTienMat = o.SoTienThuTienMat,
                    SoTienThuChuyenKhoan = o.SoTienThuChuyenKhoan,
                    SoTienThuPos = o.SoTienThuPos,
                    //SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,
                    //NguoiThu = o.NhanVienThucHien.User.HoTen,
                    SoPhieuThuGhiNoId = o.SoPhieuThuGhiNoId,
                    NguoiThuId = o.NhanVienThucHienId,
                    ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.Ten).ToList(),
                    TongChiPhiBNTT = o.PhieuChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum()
                }).ToList();
            foreach(var phieuThu in phieuThus)
            {
                var yctn = yeuCauTiepNhans.First(o => o.Id == phieuThu.YeuCauTiepNhanId);
                phieuThu.MaBenhNhan = yctn.MaBenhNhan;
                phieuThu.TenBenhNhan = yctn.TenBenhNhan;
                phieuThu.NamSinh = yctn.NamSinh != null ? yctn.NamSinh.ToString() : string.Empty;
                phieuThu.NguoiGioiThieu = yctn.NguoiGioiThieu;
                phieuThu.MaYTe = yctn.MaYeuCauTiepNhan;
                phieuThu.SoBenhAn = yctn.SoBenhAn;
                phieuThu.BenhAnSoSinh = yctn.YeuCauTiepNhanMeId != null;
                phieuThu.SoPhieuThuGhiNo = phieuThu.SoPhieuThuGhiNoId != null ? soPhieuThuGhiNos.FirstOrDefault(o => o.Id == phieuThu.SoPhieuThuGhiNoId)?.SoPhieuHienThi : string.Empty;
                phieuThu.NguoiThu = phieuThu.NguoiThuId != null ? hotenUsers.FirstOrDefault(o => o.Id == phieuThu.NguoiThuId)?.HoTen : string.Empty;
                
                //if (string.IsNullOrEmpty(phieuThu.SoBenhAn))
                //{
                //    phieuThu.SoBenhAn = yeuCauTiepNhans.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null && o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == phieuThu.YeuCauTiepNhanId)?.SoBenhAn;
                //    if (!string.IsNullOrEmpty(phieuThu.SoBenhAn))
                //    {
                //    }
                //}
                allData.Add(phieuThu);
            }

            var phieuHuyHoans = allPhieuThu
                .Where(o => o.DaHuy == true &&
                (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) &&
                (queryInfo.TuNgay == null || tuNgay < o.NgayHuy) &&
                (queryInfo.DenNgay == null || o.NgayHuy <= denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienHuyId == queryInfo.NhanVienId))
                .Select(o => new BaoCaoThuPhiVienPhiGridVo
                {
                    Id = o.NhanVienHuyId.GetValueOrDefault(),
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    LoaiThuTienBenhNhan = o.LoaiThuTienBenhNhan,
                    LaPhieuHuy = true,                    
                    NgayThu = o.NgayHuy ?? o.NgayThu,
                    //MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                    //TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                    //NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                    //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                    //MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                    //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                    GoiDichVu = o.GoiDichVu,
                    SoBLHD = o.SoBLHD,
                    PhieuChis = o.PhieuChis,
                    CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                    CongNoCaNhan = o.CongNoCaNhan,
                    SoTienThuTienMat = o.SoTienThuTienMat,
                    SoTienThuChuyenKhoan = o.SoTienThuChuyenKhoan,
                    SoTienThuPos = o.SoTienThuPos,
                    //SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,
                    //NguoiThu = o.NhanVienHuy != null ? o.NhanVienHuy.User.HoTen : string.Empty,
                    SoPhieuThuGhiNoId = o.SoPhieuThuGhiNoId,
                    NguoiThuId = o.NhanVienHuyId,
                    ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.Ten).ToList(),
                    TongChiPhiBNTT = o.PhieuChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum()
                }).ToList();

            foreach (var phieuHuyHoan in phieuHuyHoans)
            {
                var yctn = yeuCauTiepNhans.First(o => o.Id == phieuHuyHoan.YeuCauTiepNhanId);
                phieuHuyHoan.MaBenhNhan = yctn.MaBenhNhan;
                phieuHuyHoan.TenBenhNhan = yctn.TenBenhNhan;
                phieuHuyHoan.NamSinh = yctn.NamSinh != null ? yctn.NamSinh.ToString() : string.Empty;
                phieuHuyHoan.NguoiGioiThieu = yctn.NguoiGioiThieu;
                phieuHuyHoan.MaYTe = yctn.MaYeuCauTiepNhan;
                phieuHuyHoan.SoBenhAn = yctn.SoBenhAn;
                phieuHuyHoan.BenhAnSoSinh = yctn.YeuCauTiepNhanMeId != null;
                phieuHuyHoan.SoPhieuThuGhiNo = phieuHuyHoan.SoPhieuThuGhiNoId != null ? soPhieuThuGhiNos.FirstOrDefault(o => o.Id == phieuHuyHoan.SoPhieuThuGhiNoId)?.SoPhieuHienThi : string.Empty;
                phieuHuyHoan.NguoiThu = phieuHuyHoan.NguoiThuId != null ? hotenUsers.FirstOrDefault(o => o.Id == phieuHuyHoan.NguoiThuId)?.HoTen : string.Empty;

                //if (string.IsNullOrEmpty(phieuHuyHoan.SoBenhAn))
                //{
                //    phieuHuyHoan.SoBenhAn = yeuCauTiepNhans.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null && o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == phieuHuyHoan.YeuCauTiepNhanId)?.SoBenhAn;
                //    if (!string.IsNullOrEmpty(phieuHuyHoan.SoBenhAn))
                //    {
                //    }
                //}
                allData.Add(phieuHuyHoan);
            }

            var phieuChis = allPhieuChi
                .Where(o => (o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) &&
                (queryInfo.TuNgay == null || tuNgay < o.NgayChi) &&
                (queryInfo.DenNgay == null || o.NgayChi <= denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienThucHienId == queryInfo.NhanVienId))
                .Select(o => new BaoCaoThuPhiVienPhiGridVo
            {
                Id = o.Id,
                YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                LaPhieuHuy = false,
                //MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NgayThu = o.NgayChi,
                //TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                //NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                //MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                GoiDichVu = o.GoiDichVu,
                SoBLHD = o.SoBLHD,
                SoTienThuTienMat = o.SoTienThuTienMat,
                NguoiThuId = o.NhanVienThucHienId,
            }).ToList();
            foreach (var phieuChi in phieuChis)
            {
                var yctn = yeuCauTiepNhans.First(o => o.Id == phieuChi.YeuCauTiepNhanId);
                phieuChi.MaBenhNhan = yctn.MaBenhNhan;
                phieuChi.TenBenhNhan = yctn.TenBenhNhan;
                phieuChi.NamSinh = yctn.NamSinh != null ? yctn.NamSinh.ToString() : string.Empty;
                phieuChi.NguoiGioiThieu = yctn.NguoiGioiThieu;
                phieuChi.MaYTe = yctn.MaYeuCauTiepNhan;
                phieuChi.SoBenhAn = yctn.SoBenhAn;
                phieuChi.BenhAnSoSinh = yctn.YeuCauTiepNhanMeId != null;
                phieuChi.NguoiThu = phieuChi.NguoiThuId != null ? hotenUsers.FirstOrDefault(o => o.Id == phieuChi.NguoiThuId)?.HoTen : string.Empty;

                //if (string.IsNullOrEmpty(phieuChi.SoBenhAn))
                //{
                //    phieuChi.SoBenhAn = yeuCauTiepNhans.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null && o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == phieuChi.YeuCauTiepNhanId)?.SoBenhAn;
                //    if (!string.IsNullOrEmpty(phieuChi.SoBenhAn))
                //    {
                //    }
                //}
                allData.Add(phieuChi);
            }

            var phieuHuyChis = allPhieuChi
                .Where(o => o.DaHuy == true &&
                (o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) &&
                (queryInfo.TuNgay == null || tuNgay < o.NgayHuy) &&
                (queryInfo.DenNgay == null || o.NgayHuy <= denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienHuyId == queryInfo.NhanVienId))
                .Select(o => new BaoCaoThuPhiVienPhiGridVo
                {
                    Id = o.NhanVienHuyId.GetValueOrDefault(),
                    YeuCauTiepNhanId = o.YeuCauTiepNhanId,
                    LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                    LaPhieuHuy = true,
                    //MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                    NgayThu = o.NgayHuy ?? o.NgayChi,
                    //TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                    //NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                    //NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                    //MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                    //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                    GoiDichVu = o.GoiDichVu,
                    SoBLHD = o.SoBLHD,
                    SoTienThuTienMat = o.SoTienThuTienMat,
                    NguoiThuId = o.NhanVienHuyId
                }).ToList();
            foreach (var phieuHuyChi in phieuHuyChis)
            {
                var yctn = yeuCauTiepNhans.First(o => o.Id == phieuHuyChi.YeuCauTiepNhanId);
                phieuHuyChi.MaBenhNhan = yctn.MaBenhNhan;
                phieuHuyChi.TenBenhNhan = yctn.TenBenhNhan;
                phieuHuyChi.NamSinh = yctn.NamSinh != null ? yctn.NamSinh.ToString() : string.Empty;
                phieuHuyChi.NguoiGioiThieu = yctn.NguoiGioiThieu;
                phieuHuyChi.MaYTe = yctn.MaYeuCauTiepNhan;
                phieuHuyChi.SoBenhAn = yctn.SoBenhAn;
                phieuHuyChi.BenhAnSoSinh = yctn.YeuCauTiepNhanMeId != null;
                phieuHuyChi.NguoiThu = phieuHuyChi.NguoiThuId != null ? hotenUsers.FirstOrDefault(o => o.Id == phieuHuyChi.NguoiThuId)?.HoTen : string.Empty;

                //if (string.IsNullOrEmpty(phieuHuyChi.SoBenhAn))
                //{
                //    phieuHuyChi.SoBenhAn = yeuCauTiepNhans.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null && o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == phieuHuyChi.YeuCauTiepNhanId)?.SoBenhAn;
                //    if (!string.IsNullOrEmpty(phieuHuyChi.SoBenhAn))
                //    {
                //    }
                //}
                allData.Add(phieuHuyChi);
            }

            var dsMaYCTN = allData.Select(o => o.MaYTe).Distinct().ToList();
            var dsMaYCTNChuaCoSoBenhAns = allData.Where(o => string.IsNullOrEmpty(o.SoBenhAn)).Select(o => o.MaYTe).Distinct().ToList();

            //var dsMaYCTNCoTiemChung1 = _yeuCauDichVuKyThuatRepository.TableNoTracking
            //    .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.YeuCauDichVuKyThuatKhamSangLocTiemChung != null && dsMaYCTN.Contains(o.YeuCauTiepNhan.MaYeuCauTiepNhan))
            //    .Select(o => new { o.YeuCauTiepNhan.MaYeuCauTiepNhan , o.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id }).Distinct().ToList();

            var dsMaYCTNCoTiemChung = _yeuCauDichVuKyThuatKhamSangLocTiemChungRepository.TableNoTracking
                .Where(o => o.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && dsMaYCTN.Contains(o.YeuCauDichVuKyThuat.YeuCauTiepNhan.MaYeuCauTiepNhan))
                .Select(o => o.YeuCauDichVuKyThuat.YeuCauTiepNhan.MaYeuCauTiepNhan).Distinct().ToList();


            var dsSoBenhAn = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.NoiTruBenhAn != null && dsMaYCTNChuaCoSoBenhAns.Contains(o.MaYeuCauTiepNhan))
                .Select(o => new { o.MaYeuCauTiepNhan, o.NoiTruBenhAn.SoBenhAn }).ToList();

            foreach (var item in allData)
            {
                if (string.IsNullOrEmpty(item.SoBenhAn))
                {
                    item.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == item.MaYTe)?.SoBenhAn ?? string.Empty;
                }
                item.CoTiemChung = dsMaYCTNCoTiemChung.Contains(item.MaYTe);                  
            }
            
            return allData;
        }

        private async Task<List<BaoCaoThuPhiVienPhiGridVo>> GetAllForBaoCaoThuPhiVienPhiOld(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            queryInfo.PhongBenhVienId = 0;//khong loc theo phong
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var phieuThuDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan &&
                (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) &&
                (queryInfo.TuNgay == null || tuNgay < o.NgayThu) &&
                (queryInfo.DenNgay == null || o.NgayThu <= denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienThucHienId == queryInfo.NhanVienId) &&
                (queryInfo.PhongBenhVienId.GetValueOrDefault() == 0 || o.NoiThucHienId == queryInfo.PhongBenhVienId));

            var phieuHuyHoanDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && o.DaHuy == true &&
                (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) &&
                (queryInfo.TuNgay == null || tuNgay < o.NgayHuy) &&
                (queryInfo.DenNgay == null || o.NgayHuy <= denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienHuyId == queryInfo.NhanVienId) &&
                (queryInfo.PhongBenhVienId.GetValueOrDefault() == 0 || o.NoiHuyId == queryInfo.PhongBenhVienId));

            var phieuChiDataQuery = _taiKhoanBenhNhanChiRepository.TableNoTracking.Where(o => (o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) &&
                (queryInfo.TuNgay == null || tuNgay < o.NgayChi) &&
                (queryInfo.DenNgay == null || o.NgayChi <= denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienThucHienId == queryInfo.NhanVienId) &&
                (queryInfo.PhongBenhVienId.GetValueOrDefault() == 0 || o.NoiThucHienId == queryInfo.PhongBenhVienId));

            var phieuHuyChiDataQuery = _taiKhoanBenhNhanChiRepository.TableNoTracking.Where(o => o.DaHuy == true &&
                (o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) &&
                (queryInfo.TuNgay == null || tuNgay < o.NgayHuy) &&
                (queryInfo.DenNgay == null || o.NgayHuy <= denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienHuyId == queryInfo.NhanVienId) &&
                (queryInfo.PhongBenhVienId.GetValueOrDefault() == 0 || o.NoiHuyId == queryInfo.PhongBenhVienId));

            var phieuThu = phieuThuDataQuery.Select(o => new BaoCaoThuPhiVienPhiGridVo
            {
                Id = o.NhanVienThucHienId,
                LoaiThuTienBenhNhan = o.LoaiThuTienBenhNhan,
                LaPhieuHuy = false,
                MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NgayThu = o.NgayThu,
                TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,
                SoBLHD = o.SoPhieuHienThi,
                //TongChiPhiBNTT = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                PhieuChis = o.TaiKhoanBenhNhanChis.Select(chi => new PhieuChiDataVo { LoaiChiTienBenhNhan = chi.LoaiChiTienBenhNhan, TienChiPhi = chi.TienChiPhi }).ToList(),
                CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                CongNoCaNhan = o.CongNo.GetValueOrDefault(),
                SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                SoTienThuChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                SoTienThuPos = o.POS.GetValueOrDefault(),
                SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,
                NguoiThu = o.NhanVienThucHien.User.HoTen,                
                ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.CongTyBaoHiemTuNhan.Ten).ToList(),
            }).ToList();
            phieuThu.ForEach(o => o.TongChiPhiBNTT = o.PhieuChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum());
            
            var phieuHuyHoan = phieuHuyHoanDataQuery.Select(o => new BaoCaoThuPhiVienPhiGridVo
            {
                Id = o.NhanVienHuyId.GetValueOrDefault(),
                LoaiThuTienBenhNhan = o.LoaiThuTienBenhNhan,
                LaPhieuHuy = true,
                MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NgayThu = o.NgayHuy ?? o.NgayThu,
                TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,
                SoBLHD = o.SoPhieuHienThi,
                //TongChiPhiBNTT = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                PhieuChis = o.TaiKhoanBenhNhanChis.Select(chi => new PhieuChiDataVo { LoaiChiTienBenhNhan = chi.LoaiChiTienBenhNhan, TienChiPhi = chi.TienChiPhi }).ToList(),
                CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                CongNoCaNhan = o.CongNo.GetValueOrDefault(),
                SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                SoTienThuChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                SoTienThuPos = o.POS.GetValueOrDefault(),
                SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,
                NguoiThu = o.NhanVienHuy != null ? o.NhanVienHuy.User.HoTen : string.Empty,
                ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.CongTyBaoHiemTuNhan.Ten).ToList()
            }).ToList();
            phieuHuyHoan.ForEach(o => o.TongChiPhiBNTT = o.PhieuChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum());
            
            var phieuChi = phieuChiDataQuery.Select(o => new BaoCaoThuPhiVienPhiGridVo
            {
                Id = o.NhanVienThucHienId.GetValueOrDefault(),
                LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                LaPhieuHuy = false,
                MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NgayThu = o.NgayChi,
                TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                GoiDichVu = o.YeuCauGoiDichVuId != null,
                SoBLHD = o.SoPhieuHienThi,
                SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                NguoiThu = o.NhanVienThucHien != null ? o.NhanVienThucHien.User.HoTen : string.Empty,
            }).ToList();
            var phieuHuyChi = phieuHuyChiDataQuery.Select(o => new BaoCaoThuPhiVienPhiGridVo
            {
                Id = o.NhanVienHuyId.GetValueOrDefault(),
                LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                LaPhieuHuy = true,
                MaBenhNhan = o.YeuCauTiepNhan.BenhNhan.MaBN,
                NgayThu = o.NgayHuy ?? o.NgayChi,
                TenBenhNhan = o.YeuCauTiepNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                MaYTe = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                GoiDichVu = o.YeuCauGoiDichVuId != null,
                SoBLHD = o.SoPhieuHienThi,
                SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                NguoiThu = o.NhanVienHuy != null ? o.NhanVienHuy.User.HoTen : string.Empty,
            }).ToList();

            var allData = new List<BaoCaoThuPhiVienPhiGridVo>();
            allData.AddRange(phieuThu);
            allData.AddRange(phieuHuyHoan);
            allData.AddRange(phieuChi);
            allData.AddRange(phieuHuyChi);

            var dsMaYCTN = allData.Select(o => o.MaYTe).Distinct().ToList();
            var dsMaYCTNChuaCoSoBenhAns = allData.Where(o => string.IsNullOrEmpty(o.SoBenhAn)).Select(o => o.MaYTe).Distinct().ToList();

            var dsMaYCTNCoTiemChung = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.YeuCauDichVuKyThuatKhamSangLocTiemChung != null && dsMaYCTN.Contains(o.YeuCauTiepNhan.MaYeuCauTiepNhan))
                .Select(o => o.YeuCauTiepNhan.MaYeuCauTiepNhan).Distinct().ToList();

            var dsSoBenhAn = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.NoiTruBenhAn != null && dsMaYCTNChuaCoSoBenhAns.Contains(o.MaYeuCauTiepNhan))
                .Select(o => new { o.MaYeuCauTiepNhan, o.NoiTruBenhAn.SoBenhAn }).ToList();

            foreach (var item in allData)
            {
                if (string.IsNullOrEmpty(item.SoBenhAn))
                {
                    item.SoBenhAn = dsSoBenhAn.FirstOrDefault(o => o.MaYeuCauTiepNhan == item.MaYTe)?.SoBenhAn ?? string.Empty;
                }
                item.CoTiemChung = dsMaYCTNCoTiemChung.Contains(item.MaYTe);
                //item.BHYT = (item.ChiTietBHYTs == null || !item.ChiTietBHYTs.Any()) ?
                //    (decimal?)null :
                //    item.ChiTietBHYTs.Select(o => (decimal)o.SoLuong.GetValueOrDefault() * o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100).DefaultIfEmpty().Sum();
            }

            return allData;
        }

        public async Task<GridDataSource> GetBaoCaoChiTietThuTienVienPhiForGridAsync(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            var allData = await GetAllForBaoCaoThuPhiVienPhi(queryInfo);

            var groupThuNgan = allData.OrderBy(o => o.NgayThu).GroupBy(p => p.Id, p => p).ToArray();

            var gridData = new List<BaoCaoThuPhiVienPhiGridVo>();
            var num = 0;
            foreach (var thuNgan in groupThuNgan)
            {
                num++;
                foreach (var baoCaoThuPhiVienPhiGridVo in thuNgan.ToList())
                {
                    baoCaoThuPhiVienPhiGridVo.STT = num;
                    gridData.Add(baoCaoThuPhiVienPhiGridVo);
                }
            }

            return new GridDataSource { Data = gridData.ToArray(), TotalRowCount = groupThuNgan.Count() };
        }

        public async Task<GridItem> GetTotalBaoCaoChiTietThuTienVienPhiForGridAsync(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            var allData = await GetAllForBaoCaoThuPhiVienPhi(queryInfo);

            return new TotalBaoCaoThuPhiVienPhiGridVo
            {
                TamUng = allData.Select(o => o.TamUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HoanUng = allData.Select(o => o.HoanUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                SoTienThu = allData.Select(o => o.SoTienThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HuyThu = allData.Select(o => o.HuyThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                CongNo = allData.Select(o => o.CongNo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TienMat = allData.Select(o => o.TienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ChuyenKhoan = allData.Select(o => o.ChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                Pos = allData.Select(o => o.Pos.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoTienMat = allData.Select(o => o.ThuNoTienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoChuyenKhoan = allData.Select(o => o.ThuNoChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoPos = allData.Select(o => o.ThuNoPos.GetValueOrDefault()).DefaultIfEmpty(0).Sum()
            };
        }

        public async Task<GridDataSource> GetBaoCaoChiTietThuTienVienPhiForMasterGridAsync(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            var allData = await GetAllForBaoCaoThuPhiVienPhi(queryInfo);
            var groupThuNgan = allData.OrderBy(o => o.NgayThu).GroupBy(p => p.Id, p => p).ToArray();

            var gridData = new List<BaoCaoChiTietThuVienPhiMasterGridVo>();
            var num = 0;
            foreach (var thuNgan in groupThuNgan)
            {
                num++;
                gridData.Add(new BaoCaoChiTietThuVienPhiMasterGridVo
                {
                    NhanVienId = thuNgan.Key,
                    HoTenNhanVien = thuNgan.First().NguoiThu.Trim(),
                    TotalTamUng = thuNgan.Select(o => o.TamUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalHoanUng = thuNgan.Select(o => o.HoanUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalSoTienThu = thuNgan.Select(o => o.SoTienThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalHuyThu = thuNgan.Select(o => o.HuyThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalCongNo = thuNgan.Select(o => o.CongNo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalTienMat = thuNgan.Select(o => o.TienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalChuyenKhoan = thuNgan.Select(o => o.ChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalPos = thuNgan.Select(o => o.Pos.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalThuNoTienMat = thuNgan.Select(o => o.ThuNoTienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalThuNoChuyenKhoan = thuNgan.Select(o => o.ThuNoChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                    TotalThuNoPos = thuNgan.Select(o => o.ThuNoPos.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                });
            }

            return new GridDataSource { Data = gridData.OrderBy(o => o.NhanVienId).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = gridData.Count };
        }

        public async Task<GridDataSource> GetBaoCaoChiTietThuTienVienPhiForDetailGridAsync(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            var allData = await GetAllForBaoCaoThuPhiVienPhi(queryInfo);

            var gridData = allData.OrderBy(o => o.NgayThu).Skip(queryInfo.Skip).Take(queryInfo.Take).Select((o, i) =>
            {
                o.STT = i + 1 + queryInfo.Skip;
                return o;
            });
            return new GridDataSource { Data = gridData.ToArray(), TotalRowCount = allData.Count };
        }
        public async Task<string> GetNameNhanVien(long? nhanVienId)
        {
            LookupItemVo lookupItemVo = new LookupItemVo { KeyId = 0, DisplayName = "Tất cả nhân viên" };
            var dsNhanVien = await _nhanVienRepository.TableNoTracking.Include(o => o.ChucDanh).ThenInclude(cd => cd.NhomChucDanh).Include(o => o.User)
               .Select(s => new LookupItemVo
               {
                   DisplayName = s.User.HoTen,
                   KeyId = s.Id
               }).ToListAsync();
            dsNhanVien.Add(lookupItemVo);
            var query = dsNhanVien.Where(x => x.KeyId == nhanVienId).FirstOrDefault()?.DisplayName != null ? dsNhanVien.Where(x => x.KeyId == nhanVienId).FirstOrDefault()?.DisplayName : ""; // tên nhân viên

            return query;
        }
        public async Task<string> GetNamePhongBenhVien(long? phongBenhId)
        {
            LookupItemVo lookupItemVo = new LookupItemVo { KeyId = 0, DisplayName = "Tất cả các quầy" };
            var listKhoaPhong = await _KhoaPhongRepository.TableNoTracking.Where(o => o.Id == phongBenhId)
               .Select(item => new LookupItemVo
               {
                   DisplayName = item.Ten,
                   KeyId = item.Id
               }).ToListAsync();

            listKhoaPhong.Add(lookupItemVo);
            var query = listKhoaPhong.Where(x => x.KeyId == phongBenhId).FirstOrDefault()?.DisplayName != null ? listKhoaPhong.Where(x => x.KeyId == phongBenhId).FirstOrDefault()?.DisplayName : ""; // tên nhân viên

            return query;
        }
        #endregion

        #region Báo cáo thu tiền người bệnh

        public async Task<GridDataSource> GetBaoCaoChiTietThuTienBenhNhanForGridAsync(BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienTiepNhanId == queryInfo.NhanVienId) &&
                (queryInfo.PhongBenhVienId.GetValueOrDefault() == 0 || o.NoiTiepNhanId == queryInfo.PhongBenhVienId));

            ycTiepNhanQuery = ycTiepNhanQuery.Where(o =>
                o.YeuCauKhamBenhs.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true) ||
                o.YeuCauDichVuKyThuats.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null) ||
                o.YeuCauDichVuGiuongBenhViens.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true) ||
                o.YeuCauDuocPhamBenhViens.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true) ||
                o.YeuCauVatTuBenhViens.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true) ||
                //TODO: need update goi dv
                //o.YeuCauGoiDichVus.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan) ||
                o.DonThuocThanhToans.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan) ||
                o.DonVTYTThanhToans.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan));

            var totalRowCount = await ycTiepNhanQuery.CountAsync();
            if (totalRowCount == 0)
            {
                return new GridDataSource { Data = new List<GridItem>(), TotalRowCount = totalRowCount };
            }

            var ycTiepNhanDataQuery = queryInfo.LayTatCa ? ycTiepNhanQuery.OrderBy(o => o.Id) : ycTiepNhanQuery.OrderBy(o => o.Id).Skip(queryInfo.Skip).Take(queryInfo.Take);

            var ycTiepNhanData = await ycTiepNhanDataQuery
                .Include(o => o.BenhNhan)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.TaiKhoanBenhNhanChis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.TaiKhoanBenhNhanChis)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.TaiKhoanBenhNhanChis)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.TaiKhoanBenhNhanChis)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.TaiKhoanBenhNhanChis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.TaiKhoanBenhNhanChis)
                .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.TaiKhoanBenhNhanChis)
                .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.TaiKhoanBenhNhanChis)
                .ToListAsync();

            var nhanViens = await _nhanVienRepository.TableNoTracking.Include(o => o.ChucDanh).ThenInclude(cd => cd.NhomChucDanh).Include(o => o.User).ToListAsync();

            var gridData = new List<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>();
            for (int i = 0; i < ycTiepNhanData.Count; i++)
            {
                gridData.AddRange(ycTiepNhanData[i].YeuCauKhamBenhs.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        STT = i + 1,
                        MaTiepNhan = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBenhNhan = ycTiepNhanData[i].BenhNhan.MaBN,
                        TenBenhNhan = ycTiepNhanData[i].BenhNhan.HoTen,
                        SoBenhAn = "",
                        TenDichVu = o.TenDichVu,
                        BacSiChiDinhThucHien = nhanViens.FirstOrDefault(nv => o.BacSiThucHienId != null && nv.Id == o.BacSiThucHienId.Value && nv.ChucDanh?.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi)?.User.HoTen,
                        DoanhThu = o.Gia,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        Pos = 0,
                        DanhSachNguoiThu = o.TaiKhoanBenhNhanChis.Where(chi => chi.NhanVienThucHienId != null).Select(chi => nhanViens.First(nv => nv.Id == chi.NhanVienThucHienId.Value).User.HoTen).ToList()
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauDichVuKyThuats.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        STT = i + 1,
                        MaTiepNhan = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBenhNhan = ycTiepNhanData[i].BenhNhan.MaBN,
                        TenBenhNhan = ycTiepNhanData[i].BenhNhan.HoTen,
                        SoBenhAn = "",
                        TenDichVu = o.TenDichVu,
                        BacSiChiDinhThucHien = o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ? nhanViens.FirstOrDefault(nv => o.NhanVienThucHienId != null && nv.Id == o.NhanVienThucHienId.Value && nv.ChucDanh?.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi)?.User.HoTen : nhanViens.FirstOrDefault(nv => nv.Id == o.NhanVienChiDinhId && nv.ChucDanh?.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi)?.User.HoTen,
                        DoanhThu = o.Gia * o.SoLan,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * o.SoLan,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        Pos = 0,
                        DanhSachNguoiThu = o.TaiKhoanBenhNhanChis.Where(chi => chi.NhanVienThucHienId != null).Select(chi => nhanViens.First(nv => nv.Id == chi.NhanVienThucHienId.Value).User.HoTen).ToList()
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauDichVuGiuongBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        STT = i + 1,
                        MaTiepNhan = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBenhNhan = ycTiepNhanData[i].BenhNhan.MaBN,
                        TenBenhNhan = ycTiepNhanData[i].BenhNhan.HoTen,
                        SoBenhAn = "",
                        TenDichVu = o.Ten,
                        BacSiChiDinhThucHien = null,
                        DoanhThu = o.Gia,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        Pos = 0,
                        DanhSachNguoiThu = o.TaiKhoanBenhNhanChis.Where(chi => chi.NhanVienThucHienId != null).Select(chi => nhanViens.First(nv => nv.Id == chi.NhanVienThucHienId.Value).User.HoTen).ToList()
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        STT = i + 1,
                        MaTiepNhan = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBenhNhan = ycTiepNhanData[i].BenhNhan.MaBN,
                        TenBenhNhan = ycTiepNhanData[i].BenhNhan.HoTen,
                        SoBenhAn = "",
                        TenDichVu = o.Ten,
                        BacSiChiDinhThucHien = null,
                        DoanhThu = o.DonGiaBan * (decimal)o.SoLuong,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        Pos = 0,
                        DanhSachNguoiThu = o.TaiKhoanBenhNhanChis.Where(chi => chi.NhanVienThucHienId != null).Select(chi => nhanViens.First(nv => nv.Id == chi.NhanVienThucHienId.Value).User.HoTen).ToList()
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        STT = i + 1,
                        MaTiepNhan = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBenhNhan = ycTiepNhanData[i].BenhNhan.MaBN,
                        TenBenhNhan = ycTiepNhanData[i].BenhNhan.HoTen,
                        SoBenhAn = "",
                        TenDichVu = o.Ten,
                        BacSiChiDinhThucHien = null,
                        DoanhThu = o.DonGiaBan * (decimal)o.SoLuong,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        Pos = 0,
                        DanhSachNguoiThu = o.TaiKhoanBenhNhanChis.Where(chi => chi.NhanVienThucHienId != null).Select(chi => nhanViens.First(nv => nv.Id == chi.NhanVienThucHienId.Value).User.HoTen).ToList()
                    }));
                //TODO: need update goi dv
                //gridData.AddRange(ycTiepNhanData[i].YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                //    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                //    {
                //        STT = i + 1,
                //        MaTiepNhan = ycTiepNhanData[i].MaYeuCauTiepNhan,
                //        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                //        MaBenhNhan = ycTiepNhanData[i].BenhNhan.MaBN,
                //        TenBenhNhan = ycTiepNhanData[i].BenhNhan.HoTen,
                //        SoBenhAn = "",
                //        TenDichVu = o.Ten,
                //        BacSiChiDinhThucHien = null,
                //        DoanhThu = o.ChiPhiGoiDichVu,
                //        BHYTChiTra = null,
                //        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                //        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                //        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                //        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                //        TienMat = 0,
                //        ChuyenKhoan = 0,
                //        Pos = 0,
                //        DanhSachNguoiThu = o.TaiKhoanBenhNhanChis.Where(chi => chi.NhanVienThucHienId != null).Select(chi => nhanViens.First(nv => nv.Id == chi.NhanVienThucHienId.Value).User.HoTen).ToList()
                //    }));
                gridData.AddRange(ycTiepNhanData[i].DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(dt => dt.DonThuocThanhToanChiTiets)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        STT = i + 1,
                        MaTiepNhan = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBenhNhan = ycTiepNhanData[i].BenhNhan.MaBN,
                        TenBenhNhan = ycTiepNhanData[i].BenhNhan.HoTen,
                        SoBenhAn = "",
                        TenDichVu = o.Ten,
                        BacSiChiDinhThucHien = null,
                        DoanhThu = o.DonGiaBan * (decimal)o.SoLuong,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        Pos = 0,
                        DanhSachNguoiThu = o.TaiKhoanBenhNhanChis.Where(chi => chi.NhanVienThucHienId != null).Select(chi => nhanViens.First(nv => nv.Id == chi.NhanVienThucHienId.Value).User.HoTen).ToList()
                    }));
                gridData.AddRange(ycTiepNhanData[i].DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(dt => dt.DonVTYTThanhToanChiTiets)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        STT = i + 1,
                        MaTiepNhan = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBenhNhan = ycTiepNhanData[i].BenhNhan.MaBN,
                        TenBenhNhan = ycTiepNhanData[i].BenhNhan.HoTen,
                        SoBenhAn = "",
                        TenDichVu = o.Ten,
                        BacSiChiDinhThucHien = null,
                        DoanhThu = o.DonGiaBan * (decimal)o.SoLuong,
                        BHYTChiTra = 0,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        Pos = 0,
                        DanhSachNguoiThu = o.TaiKhoanBenhNhanChis.Where(chi => chi.NhanVienThucHienId != null).Select(chi => nhanViens.First(nv => nv.Id == chi.NhanVienThucHienId.Value).User.HoTen).ToList()
                    }));
            }

            return new GridDataSource { Data = gridData.ToArray(), TotalRowCount = totalRowCount };
        }
        public async Task<GridItem> GetTotalBaoCaoChiTietThuTienBenhNhanForGridAsync(BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay) &&
                (queryInfo.NhanVienId.GetValueOrDefault() == 0 || o.NhanVienTiepNhanId == queryInfo.NhanVienId) &&
                (queryInfo.PhongBenhVienId.GetValueOrDefault() == 0 || o.NoiTiepNhanId == queryInfo.PhongBenhVienId));

            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToans).ThenInclude(yc => yc.DonThuocThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.DonVTYTThanhToans).ThenInclude(yc => yc.DonVTYTThanhToanChiTiets).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            var gridData = new List<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>();
            for (int i = 0; i < ycTiepNhanData.Count; i++)
            {
                gridData.AddRange(ycTiepNhanData[i].YeuCauKhamBenhs.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.KhongTinhPhi != true)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        DoanhThu = o.Gia,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = null,
                        ChuyenKhoan = null,
                        Pos = null
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauDichVuKyThuats.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        DoanhThu = o.Gia * o.SoLan,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * o.SoLan,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = null,
                        ChuyenKhoan = null,
                        Pos = null,
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauDichVuGiuongBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.KhongTinhPhi != true)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        DoanhThu = o.Gia,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = null,
                        ChuyenKhoan = null,
                        Pos = null,
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauDuocPhamBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.KhongTinhPhi != true)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        DoanhThu = o.DonGiaBan * (decimal)o.SoLuong,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = null,
                        ChuyenKhoan = null,
                        Pos = null,
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauVatTuBenhViens.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.KhongTinhPhi != true)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        DoanhThu = o.DonGiaBan * (decimal)o.SoLuong,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = null,
                        ChuyenKhoan = null,
                        Pos = null,
                    }));
                //TODO: need update goi dv
                //gridData.AddRange(ycTiepNhanData[i].YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                //    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                //    {
                //        DoanhThu = o.ChiPhiGoiDichVu,
                //        BHYTChiTra = null,
                //        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                //        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                //        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                //        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                //        TienMat = null,
                //        ChuyenKhoan = null,
                //        Pos = null,
                //    }));
                gridData.AddRange(ycTiepNhanData[i].DonThuocThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(dt => dt.DonThuocThanhToanChiTiets)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        DoanhThu = o.DonGiaBan * (decimal)o.SoLuong,
                        BHYTChiTra = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * (decimal)o.SoLuong,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = null,
                        ChuyenKhoan = null,
                        Pos = null,
                    }));
                gridData.AddRange(ycTiepNhanData[i].DonVTYTThanhToans.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan).SelectMany(dt => dt.DonVTYTThanhToanChiTiets)
                    .Select(o => new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
                    {
                        DoanhThu = o.DonGiaBan * (decimal)o.SoLuong,
                        BHYTChiTra = 0,
                        BHYTTuNhanChiTra = o.SoTienBaoHiemTuNhanChiTra,
                        MiemGiam = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        Voucher = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        ThuTuBenhNhan = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                        TienMat = null,
                        ChuyenKhoan = null,
                        Pos = null,
                    }));
            }
            return new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo
            {
                DoanhThu = gridData.Sum(o => o.DoanhThu.GetValueOrDefault()),
                BHYTChiTra = gridData.Sum(o => o.BHYTChiTra.GetValueOrDefault()),
                BHYTTuNhanChiTra = gridData.Sum(o => o.BHYTTuNhanChiTra.GetValueOrDefault()),
                MiemGiam = gridData.Sum(o => o.MiemGiam.GetValueOrDefault()),
                Voucher = gridData.Sum(o => o.Voucher.GetValueOrDefault()),
                ThuTuBenhNhan = gridData.Sum(o => o.ThuTuBenhNhan.GetValueOrDefault()),
                TienMat = gridData.Sum(o => o.TienMat.GetValueOrDefault()),
                ChuyenKhoan = gridData.Sum(o => o.ChuyenKhoan.GetValueOrDefault()),
                Pos = gridData.Sum(o => o.Pos.GetValueOrDefault()),
            };
        }
        //public List<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo> BaoCaoThuTienBenhNhan(QueryInfo queryInfo)
        //{

        //    var noiTruModel = new List<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo>();
        //    for (int i = 1; i < 20; i++)
        //    {
        //        var data = new BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo()
        //        {          
        //            STT = i < 5 ? 1 : i < 10 ? 2 : i < 15 ? 3 : 4,
        //            MaTiepNhan = "00" + i,
        //            NgayThu = i < 5 ? DateTime.Now.AddDays(-2) : i < 10 ? DateTime.Now.AddDays(-8) : i < 15 ? DateTime.Now.AddDays(-9) : DateTime.Now,                
        //            MaBenhNhan = i < 5 ? "Mã Người Bệnh 001" : i < 10 ? "Mã Người Bệnh 002" : i < 15 ? "Mã Người Bệnh 003" : "Mã Người Bệnh 004",
        //            TenBenhNhan = i < 5 ? "Nguyễn văn A" : i < 10 ? "Nguyễn văn B" : i < 15 ? "Nguyễn văn C" : "Nguyễn văn C",
        //            SoBenhAn = "Số Bệnh Án" + i,
        //            TenDichVu = "Tên Dịch Vụ 000" + i,
        //            BacSiChiDinhThucHien = "Bac si 000" + i,
        //            DoanhThu = 100 + i,
        //            BHYTChiTra = 100 + i,
        //            BHYTTuNhanChiTra = 100 + i,
        //            MiemGiam = 100 + i,
        //            Voucher = 100 + i,
        //            ThuTuBenhNhan = 100 + i,
        //            TienMat = 100 + i,
        //            ChuyenKhoan = 100 + i,
        //            Pos = 100 + i
        //        };                
        //        noiTruModel.Add(data);
        //    }
        //    return noiTruModel;

        //}

        #endregion

        #endregion
        #region Báo cáo lưu trữ hồ sơ bệnh án 19/2/2021
        #region grid 
        public async Task<GridDataSource> GetBaoCaoLuuTruHoSoBenhAnForGridAsync(BaoCaoLuuHoSoBenhAnVo queryInfo)
        {
            var entityData = _yeuCauTiepNhanRepository.TableNoTracking
                                                      .Include(q => q.BenhNhan)
                                                      .Include(w => w.NoiTruBenhAn)
                                                      .Where(s => s.NoiTruBenhAn.ThoiDiemNhapVien >= queryInfo.NgayVaoVien && s.NoiTruBenhAn.ThoiDiemRaVien <= queryInfo.NgayRaVien && s.NoiTruBenhAn.KhoaPhongNhapVienId == queryInfo.KhoaId && s.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                                                      .Select(o => new BaoCaoLuuHoSoBenhAnGridVo()
                                                      {
                                                          Id = o.Id,
                                                          ThuTuSap = o.NoiTruBenhAn.ThuTuSapXepLuuTru,
                                                          SoLuuTru = o.NoiTruBenhAn.SoLuuTru,
                                                          HoTen = o.HoTen,
                                                          GioiTinh = o.GioiTinh != null ? o.GioiTinh.GetDescription() : "",
                                                          ThoiGianVaoVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
                                                          ThoiGianRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,
                                                          ChanDoan = "",// note,
                                                          ICD = "",
                                                          ThongTinRaVien = o.NoiTruBenhAn.ThongTinRaVien
                                                      });
            List<BaoCaoLuuHoSoBenhAnGridVo> listHoSoBenhAn = new List<BaoCaoLuuHoSoBenhAnGridVo>();
            foreach (var itemHoSoBenhAn in entityData.ToList())
            {
                if (itemHoSoBenhAn.ThongTinRaVien != null)
                {
                    var dataJson = JsonConvert.DeserializeObject<ChuanDoanVaICD>(itemHoSoBenhAn.ThongTinRaVien);
                    if (dataJson != null)
                    {
                        itemHoSoBenhAn.ICD = dataJson.TenChuanDoanRaVien;
                        itemHoSoBenhAn.ChanDoan = dataJson.GhiChuChuanDoanRaVien;
                        listHoSoBenhAn.Add(itemHoSoBenhAn);
                    }
                }

            }
            var dataOrderBy = listHoSoBenhAn.AsQueryable();
            var luuTruHoSoBenhAn = dataOrderBy.ToArray();
            var countTask = dataOrderBy.Count();

            return new GridDataSource { Data = luuTruHoSoBenhAn, TotalRowCount = countTask };
        }
        #endregion
        #region in 
        public async Task<string> InBaoCaoLuuTruHoSoBenhAn(BaoCaoLuuHoSoBenhAnVo baoCaoLuuHoSoBenhAnVo)
        {
            var content = "";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("BaoCaoLuuTruHoSoBenhAn"));
            var entityData = _yeuCauTiepNhanRepository.TableNoTracking
                                                      .Include(q => q.BenhNhan)
                                                      .Include(w => w.NoiTruBenhAn)
                                                      .Where(s => s.NoiTruBenhAn.ThoiDiemNhapVien >= baoCaoLuuHoSoBenhAnVo.NgayVaoVien && s.NoiTruBenhAn.ThoiDiemRaVien <= baoCaoLuuHoSoBenhAnVo.NgayRaVien && s.NoiTruBenhAn.KhoaPhongNhapVienId == baoCaoLuuHoSoBenhAnVo.KhoaId)
                                                      .Select(o => new BaoCaoLuuHoSoBenhAnGridVo()
                                                      {
                                                          Id = o.Id,
                                                          ThuTuSap = o.NoiTruBenhAn.ThuTuSapXepLuuTru,
                                                          SoLuuTru = o.NoiTruBenhAn.SoLuuTru,
                                                          HoTen = o.HoTen,
                                                          GioiTinh = o.GioiTinh != null ? o.GioiTinh.GetDescription() : "",
                                                          ThoiGianVaoVien = o.NoiTruBenhAn.ThoiDiemNhapVien,
                                                          ThoiGianRaVien = o.NoiTruBenhAn.ThoiDiemRaVien,
                                                          ChanDoan = "",// note,
                                                          ICD = "",
                                                          Khoa = o.NoiTruBenhAn.KhoaPhongNhapVien.Ten,
                                                          ThongTinRaVien = o.NoiTruBenhAn.ThongTinRaVien
                                                      }).ToList();
            List<BaoCaoLuuHoSoBenhAnGridVo> listHoSoBenhAn = new List<BaoCaoLuuHoSoBenhAnGridVo>();
            foreach (var itemHoSoBenhAn in entityData.ToList())
            {
                if (itemHoSoBenhAn.ThongTinRaVien != null)
                {
                    var dataJson = JsonConvert.DeserializeObject<ChuanDoanVaICD>(itemHoSoBenhAn.ThongTinRaVien);
                    if (dataJson != null)
                    {
                        itemHoSoBenhAn.ICD = dataJson.TenChuanDoanRaVien;
                        itemHoSoBenhAn.ChanDoan = dataJson.GhiChuChuanDoanRaVien;
                        listHoSoBenhAn.Add(itemHoSoBenhAn);
                    }
                }

            }
            //var rowTotalTabledefault = 15;
            //if(listHoSoBenhAn.Count() < rowTotalTabledefault)
            //{
            //    var rowConLai = rowTotalTabledefault - listHoSoBenhAn.Count();
            //    for (int i = 0; i < rowConLai; i++)
            //    {
            //        BaoCaoLuuHoSoBenhAnGridVo hoSoBenhAn = new BaoCaoLuuHoSoBenhAnGridVo();
            //        hoSoBenhAn.ICD = "";
            //        hoSoBenhAn.ChanDoan = "";
            //        listHoSoBenhAn.Add(hoSoBenhAn);
            //    }
            //}
            var table = "";
            if (entityData.Any())
            {
                int stt = 1;
                foreach (var item in listHoSoBenhAn)
                {
                    table += "<tr>" +
                             "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + stt + "</td>"
                             + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.ThuTuSap + "</td>"
                             + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.SoLuuTru + "</td>"
                              + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.HoTen + "</td>"
                                + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.GioiTinh + "</td>"
                             + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.Tuoi + "</td>"
                              + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.ThoiGianVaoVienString + "</td>"
                             + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.ThoiGianRaVienString + "</td>"

                             + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.ChanDoan + "</td>"
                              + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + item.ICD + "</td>"
                              + "</tr>";
                    stt++;
                }
            }
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            var data = new
            {
                LogoUrl = baoCaoLuuHoSoBenhAnVo.Hosting + "/assets/img/logo-bacha-full.png",
                TuNgayBaoCao = baoCaoLuuHoSoBenhAnVo.NgayVaoVien.GetValueOrDefault().ApplyFormatDateTimeSACH(),
                DenNgayBaoCao = baoCaoLuuHoSoBenhAnVo.NgayRaVien.GetValueOrDefault().ApplyFormatDateTimeSACH(),
                Khoa = entityData.Any() ? entityData.LastOrDefault().Khoa : "",
                NguoiLap = nguoiLogin,
                BaoCaoLuuTruHoSoBenhAn = table,
                NgayHienTai = DateTime.Now.Day,
                ThangHienTai = DateTime.Now.Month,
                NamHienTai = DateTime.Now.Year
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        #endregion
        #region Export 
        public virtual byte[] ExportBaoCaoLuuTruHoSoBenhAn(ICollection<BaoCaoLuuHoSoBenhAnGridVo> data, DateTime? tuNgay, DateTime? denNgay, long khoaId, string hosting)
        {
            var queryInfo = new BaoCaoLuuHoSoBenhAnGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoLuuHoSoBenhAnGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO LƯU TRỮ HỒ SƠ BỆNH ÁN");

                    // set row

                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 10;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 40;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 60;
                    worksheet.Column(11).Width = 60;

                    worksheet.DefaultColWidth = 11;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 4 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        var url = hosting + "/assets/img/logo-bacha-full.png";
                        WebClient wc = new WebClient();
                        byte[] bytes = wc.DownloadData(url); // download file từ server
                        MemoryStream ms = new MemoryStream(bytes); //
                        Image img = Image.FromStream(ms); // chuyển đổi thành img
                        ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                        pic.SetPosition(0, 0, 0, 0);
                        var height = 80; // chiều cao từ A1 đến A6
                        var width = 400; // chiều rộng từ A1 đến D1
                        pic.SetSize(width, height);
                        range.Worksheet.Protection.IsProtected = false;
                        range.Worksheet.Protection.AllowSelectLockedCells = false;
                    }

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "BÁO CÁO LƯU TRỮ HỒ SƠ BỆNH ÁN".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }
                    //string khoa = _KhoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(s => s.Ten).FirstOrDefault();
                    //using (var range = worksheet.Cells[worksheetPhong])
                    //{
                    //    range.Worksheet.Cells[worksheetPhong].Merge = true;
                    //    range.Worksheet.Cells[worksheetPhong].Value = "Phòng khám: " + khoa == null ? "Tất cả các phòng" : khoa;
                    //    range.Worksheet.Cells[worksheetPhong].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells[worksheetPhong].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells[worksheetPhong].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                    //    range.Worksheet.Cells[worksheetPhong].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells[worksheetPhong].Style.Font.Bold = true;
                    //    range.Worksheet.Cells[worksheetPhong].Style.Font.Italic = true;
                    //}

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "STT" }, { "B", "Thứ tự sắp" }, { "C", "Số lưu trữ" } , { "D", "Họ tên" },
                                    { "E", "Giới tính" }, { "F", "Tuổi" },{ "G", "Thời gian" },{ "H", "Thời gian ra viện" },{ "I", "Chẩn đoán" },
                                    { "J", "ICD" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoLuuHoSoBenhAnGridVo>(requestProperties);
                    int index = 7;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int stt = 1;

                    foreach (var baoCao in data)
                    {
                        manager.CurrentObject = baoCao;
                        manager.WriteToXlsx(worksheet, index);
                        worksheet.Cells["A" + index].Value = stt;
                        worksheet.Cells["B" + index].Value = baoCao.ThuTuSap;
                        worksheet.Cells["C" + index].Value = baoCao.SoLuuTru;
                        worksheet.Cells["D" + index].Value = baoCao.HoTen;
                        worksheet.Cells["E" + index].Value = baoCao.GioiTinh;
                        worksheet.Cells["F" + index].Value = baoCao.Tuoi;
                        worksheet.Cells["G" + index].Value = baoCao.ThoiGianVaoVienString;
                        worksheet.Cells["H" + index].Value = baoCao.ThoiGianRaVienString;
                        worksheet.Cells["I" + index].Value = baoCao.ChanDoan;
                        worksheet.Cells["J" + index].Value = baoCao.ICD;
                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }
                        index++;
                        stt++;
                        var indexMain = index;
                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        #endregion
        #endregion

        #region Báo cáo người bệnh khám ngoại trú 22/2/2021
        #region list phòng theo khoa
        public async Task<List<ChucDanhItemVo>> GetTaCaPhongTheoKhoa(DropDownListRequestModel model)
        {
            var lstPhongBenhVien = await _phongBenhVienRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();
            var query = lstPhongBenhVien.Select(item => new ChucDanhItemVo()
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma,
            }).ToList();
            return query;
        }
        #endregion
        #region grid 
        //public async Task<GridDataSource> GetBaoCaoBenhNhanKhamNgoaiTruForGridAsync(BaoCaoBenhNhanKhamNgoaiTruVo queryInfo)
        //{
        //    if (queryInfo.PhongId == 0)
        //    {
        //        var entityData = _yeuCauKhamBenhRepository.TableNoTracking
        //                                               .Include(c => c.TaiKhoanBenhNhanChis)
        //                                              .Where(s => s.YeuCauTiepNhan.ThoiDiemTiepNhan >= queryInfo.TuNgay && s.YeuCauTiepNhan.ThoiDiemTiepNhan <= queryInfo.DenNgay && s.NoiDangKy.KhoaPhongId == queryInfo.KhoaId && s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
        //                                              .Select(o => new BaoCaoBenhNhanKhamNgoaiTruGridVo()
        //                                              {
        //                                                  ThoiGianTiepNhan = o.YeuCauTiepNhan.ThoiDiemTiepNhan,
        //                                                  MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
        //                                                  HoTenBn = o.YeuCauTiepNhan.HoTen,
        //                                                  GioiTinh = o.YeuCauTiepNhan.GioiTinh != null ? o.YeuCauTiepNhan.GioiTinh.GetValueOrDefault().GetDescription() : "",
        //                                                  TheBHYT = o.YeuCauTiepNhan.BHYTMaSoThe,
        //                                                  PhieuKham = o.TenDichVu,
        //                                                  PhongKham = o.NoiThucHienId != null ? o.NoiThucHien.Ten : "",
        //                                                  PhongKhamId = o.NoiThucHienId != null ? o.NoiThucHienId : 0,
        //                                                  NoiThucHien = o.NoiThucHienId != null ? o.NoiThucHien.Ten : "",
        //                                                  ICD = o.Icdchinh.Ma + "-" + o.Icdchinh.TenTiengViet,
        //                                                  TrangThai = o.TrangThai.GetDescription(),
        //                                                  BsKham = o.BacSiThucHienId != null ? o.BacSiThucHien.User.HoTen : "",
        //                                                  BsKetLuan = o.BacSiKetLuanId != null ? o.BacSiKetLuan.User.HoTen : "",
        //                                                  /*ThoiGianThanhToan = o.TaiKhoanBenhNhanChis.Any() ?  o.TaiKhoanBenhNhanChis.Where(d=>d.DaHuy != true).Select(s=>s.NgayChi.ApplyFormatDateTimeSACH()).LastOrDefault() : "",*/
        //                                                  CachGiaiQuyet = o.CachGiaiQuyet,
        //                                                  NgoaiGioHanhChinh = (o.ThoiDiemThucHien.GetValueOrDefault().Hour >= 16 && o.ThoiDiemThucHien.GetValueOrDefault().Minute >= 45) &&
        //                                                                     (o.ThoiDiemThucHien.GetValueOrDefault().Hour <= 7 && o.ThoiDiemThucHien.GetValueOrDefault().Minute == 0) ? "V" : ""
        //                                              }).ToList();
        //        var totalGroup = 0;
        //        var length = entityData.GroupBy(x => new
        //        {
        //            x.PhongKham
        //        }).Select(s => new
        //        {
        //            PhongKhamId = s.FirstOrDefault().PhongKhamId,
        //            TenPhongKham = s.FirstOrDefault().PhongKham
        //        }).ToList();

        //        List<BaoCaoBenhNhanKhamNgoaiTruGridVo> list = new List<BaoCaoBenhNhanKhamNgoaiTruGridVo>();
        //        foreach (var item in length)
        //        {
        //            foreach (var itemx in entityData.ToList())
        //            {
        //                if (item.PhongKhamId == itemx.PhongKhamId)
        //                {
        //                    totalGroup++;
        //                }
        //            }
        //            foreach (var itemk in entityData.ToList())
        //            {
        //                if (itemk.PhongKhamId == item.PhongKhamId)
        //                {
        //                    itemk.PhongKham = "";
        //                    itemk.PhongKham = item.TenPhongKham + "/" + totalGroup;
        //                    list.Add(itemk);
        //                }
        //            }
        //            totalGroup = 0;
        //        }
        //        var dataOrderBy = list.AsQueryable();
        //        var luuTruHoSoBenhAn = dataOrderBy.ToArray();
        //        var countTask = dataOrderBy.Count();

        //        return new GridDataSource { Data = luuTruHoSoBenhAn, TotalRowCount = countTask };
        //    }
        //    else
        //    {
        //        var entityData = _yeuCauKhamBenhRepository.TableNoTracking
        //                                               .Include(c => c.TaiKhoanBenhNhanChis)
        //                                              .Where(s => s.YeuCauTiepNhan.ThoiDiemTiepNhan >= queryInfo.TuNgay && s.YeuCauTiepNhan.ThoiDiemTiepNhan <= queryInfo.DenNgay && s.NoiDangKyId == queryInfo.PhongId && s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
        //                                              .Select(o => new BaoCaoBenhNhanKhamNgoaiTruGridVo()
        //                                              {
        //                                                  ThoiGianTiepNhan = o.YeuCauTiepNhan.ThoiDiemTiepNhan,
        //                                                  MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
        //                                                  HoTenBn = o.YeuCauTiepNhan.HoTen,
        //                                                  GioiTinh = o.YeuCauTiepNhan.GioiTinh != null ? o.YeuCauTiepNhan.GioiTinh.GetValueOrDefault().GetDescription() : "",
        //                                                  TheBHYT = o.YeuCauTiepNhan.BHYTMaSoThe,
        //                                                  PhieuKham = o.TenDichVu,
        //                                                  PhongKham = o.NoiThucHienId != null ? o.NoiThucHien.Ten : "",
        //                                                  PhongKhamId = o.NoiThucHienId != null ? o.NoiThucHienId : 0,
        //                                                  NoiThucHien = o.NoiThucHienId != null ? o.NoiThucHien.Ten : "",
        //                                                  ICD = o.Icdchinh.Ma + "-" + o.Icdchinh.TenTiengViet,
        //                                                  TrangThai = o.TrangThai.GetDescription(),
        //                                                  BsKham = o.BacSiThucHienId != null ? o.BacSiThucHien.User.HoTen : "",
        //                                                  BsKetLuan = o.BacSiKetLuanId != null ? o.BacSiKetLuan.User.HoTen : "",
        //                                                  /*ThoiGianThanhToan = o.TaiKhoanBenhNhanChis.Any() ?  o.TaiKhoanBenhNhanChis.Where(d=>d.DaHuy != true).Select(s=>s.NgayChi.ApplyFormatDateTimeSACH()).LastOrDefault() : "",*/
        //                                                  CachGiaiQuyet = o.CachGiaiQuyet,
        //                                                  NgoaiGioHanhChinh = (o.ThoiDiemThucHien.GetValueOrDefault().Hour >= 16 && o.ThoiDiemThucHien.GetValueOrDefault().Minute >= 45) &&
        //                                                                     (o.ThoiDiemThucHien.GetValueOrDefault().Hour <= 7 && o.ThoiDiemThucHien.GetValueOrDefault().Minute == 0) ? "V" : ""
        //                                              }).ToList();
        //        var totalGroup = 0;
        //        var length = entityData.GroupBy(x => new
        //        {
        //            x.PhongKham
        //        }).Select(s => new
        //        {
        //            PhongKhamId = s.FirstOrDefault().PhongKhamId,
        //            TenPhongKham = s.FirstOrDefault().PhongKham
        //        }).ToList();

        //        List<BaoCaoBenhNhanKhamNgoaiTruGridVo> list = new List<BaoCaoBenhNhanKhamNgoaiTruGridVo>();
        //        foreach (var item in length)
        //        {
        //            foreach (var itemx in entityData.ToList())
        //            {
        //                if (item.PhongKhamId == itemx.PhongKhamId)
        //                {
        //                    totalGroup++;
        //                }
        //            }
        //            foreach (var itemk in entityData.ToList())
        //            {
        //                if (itemk.PhongKhamId == item.PhongKhamId)
        //                {
        //                    itemk.PhongKham = "";
        //                    itemk.PhongKham = item.TenPhongKham + "/" + totalGroup;
        //                    list.Add(itemk);
        //                }
        //            }
        //            totalGroup = 0;
        //        }
        //        var dataOrderBy = list.AsQueryable();
        //        var luuTruHoSoBenhAn = dataOrderBy.ToArray();
        //        var countTask = dataOrderBy.Count();

        //        return new GridDataSource { Data = luuTruHoSoBenhAn, TotalRowCount = countTask };
        //    }
        //    return null;
        //}

        public async Task<GridDataSource> GetDataBaoCaoBenhNhanKhamNgoaiTruForGridAsync(BaoCaoBenhNhanKhamNgoaiTruQueryInfo queryInfo)
        {
            var yeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking                
                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.ThoiDiemThucHien >= queryInfo.FromDate
                            && x.ThoiDiemThucHien <= queryInfo.ToDate
                            && (queryInfo.KhoaId == null || x.NoiThucHien.KhoaPhongId == queryInfo.KhoaId)
                            && (queryInfo.PhongId == null || queryInfo.PhongId == 0 || x.NoiThucHienId == queryInfo.PhongId))                
                .Select(item => new BaoCaoBenhNhanKhamNgoaiTruDemoGridVo()
                {
                    YeuCauGoiDichVuId = item.YeuCauGoiDichVuId,
                    GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                    ThoiGianKham = item.ThoiDiemThucHien,
                    CongTyKhamSucKhoe = item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null ? item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten : "",
                    MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    LoaiGioiTinh = item.YeuCauTiepNhan.GioiTinh,
                    TheBHYT = item.YeuCauTiepNhan.BHYTMaSoThe,
                    PhieuKham = item.TenDichVu,
                    PhongKham = item.NoiThucHien.Ten,
                    ICD = item.IcdchinhId != null
                        ? $"{item.Icdchinh.Ma} - {item.Icdchinh.TenTiengViet}"
                        : (item.ChanDoanSoBoICDId != null ? $"{item.ChanDoanSoBoICD.Ma} - {item.ChanDoanSoBoICD.TenTiengViet}" : ""),
                    TrangThai = item.TrangThai.GetDescription(),
                    BacSiKham = item.BacSiThucHienId != null ? item.BacSiThucHien.User.HoTen : "",
                    BacSiKetLuan = item.BacSiKetLuanId != null ? item.BacSiKetLuan.User.HoTen : "",
                    CachGiaiQuyet = item.CachGiaiQuyet,
                    KhoaNhapVien = item.KhoaPhongNhapVienId != null ? item.KhoaPhongNhapVien.Ten : "",
                    SuDungGoi = item.YeuCauGoiDichVuId != null,

                    PhongKhamId = item.NoiThucHienId,
                    NoiThucHien = item.NoiThucHienId != null ? item.NoiThucHien.Ten : "",
                    Id = item.Id
                })                
                .ToList();

            return new GridDataSource { Data = yeuCauKhams.OrderByDescending(x => x.YeuCauGoiDichVuId == null && x.GoiKhamSucKhoeId == null)
                .ThenByDescending(x => x.GoiKhamSucKhoeId != null)
                .ThenByDescending(x => x.YeuCauGoiDichVuId != null)
                .ThenBy(x => x.ThoiGianKham).ToArray(), TotalRowCount = yeuCauKhams.Count };
        }

        public async Task<GridDataSource> GetDataBaoCaoBenhNhanKhamNgoaiTruForGridAsyncOld(BaoCaoBenhNhanKhamNgoaiTruQueryInfo queryInfo)
        {
            var yeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.HopDongKhamSucKhoe).ThenInclude(x => x.CongTyKhamSucKhoe)
                .Include(x => x.Icdchinh)
                .Include(x => x.ChanDoanSoBoICD)
                .Include(x => x.BacSiThucHien).ThenInclude(x => x.User)
                .Include(x => x.BacSiKetLuan).ThenInclude(x => x.User)
                .Include(x => x.KhoaPhongNhapVien)
                .Include(x => x.NoiThucHien)
                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.ThoiDiemThucHien >= queryInfo.FromDate
                            && x.ThoiDiemThucHien <= queryInfo.ToDate
                            && (queryInfo.KhoaId == null || x.NoiThucHien.KhoaPhongId == queryInfo.KhoaId)
                            && (queryInfo.PhongId == null || queryInfo.PhongId == 0 || x.NoiThucHienId == queryInfo.PhongId))
                .OrderByDescending(x => x.YeuCauGoiDichVuId == null && x.GoiKhamSucKhoeId == null)
                .ThenByDescending(x => x.GoiKhamSucKhoeId != null)
                .ThenByDescending(x => x.YeuCauGoiDichVuId != null)
                .ThenBy(x => x.ThoiDiemThucHien)
                .ToList();

            var result = yeuCauKhams
                .Select(item => new BaoCaoBenhNhanKhamNgoaiTruDemoGridVo()
                {
                    ThoiGianKham = item.ThoiDiemThucHien,
                    CongTyKhamSucKhoe = item.YeuCauTiepNhan?.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoe?.CongTyKhamSucKhoe?.Ten,
                    MaYeuCauTiepNhan = item.YeuCauTiepNhan?.MaYeuCauTiepNhan,
                    HoTen = item.YeuCauTiepNhan?.HoTen,
                    NgaySinh = item.YeuCauTiepNhan?.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan?.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan?.NamSinh,
                    //GioiTinh = item.YeuCauTiepNhan?.GioiTinh?.GetDescription(),
                    TheBHYT = item.YeuCauTiepNhan?.BHYTMaSoThe,
                    PhieuKham = item.TenDichVu,
                    PhongKham = item.NoiThucHien.Ten,
                    ICD = item.Icdchinh != null 
                        ? $"{item.Icdchinh.Ma} - {item.Icdchinh.TenTiengViet}" 
                        : (item.ChanDoanSoBoICD != null ? $"{item.ChanDoanSoBoICD.Ma} - {item.ChanDoanSoBoICD.TenTiengViet}" : string.Empty),
                    TrangThai = item.TrangThai.GetDescription(),
                    BacSiKham = item.BacSiThucHien?.User?.HoTen,
                    BacSiKetLuan = item.BacSiKetLuan?.User?.HoTen,
                    CachGiaiQuyet = item.CachGiaiQuyet,
                    KhoaNhapVien = item.KhoaPhongNhapVien?.Ten,
                    SuDungGoi = item.YeuCauGoiDichVuId != null,

                    PhongKhamId = item.NoiThucHienId,
                    NoiThucHien = item.NoiThucHien?.Ten,
                    Id = item.Id
                })
                .ToList();

            return new GridDataSource { Data = result.ToArray(), TotalRowCount = result.Count };
        }
        #endregion
        #region in 
        public async Task<string> InBaoCaoBenhNhanKhamNgoaiTru(BaoCaoBenhNhanKhamNgoaiTruVo baoCaoBenhNhanKhamNgoaiTruVo)
        {
            var content = "";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("BaoCaoDanhSachBenhNhanKhamNgoaiTru"));
            var entityData = _yeuCauKhamBenhRepository.TableNoTracking
                                                       .Include(c => c.TaiKhoanBenhNhanChis)
                                                      .Where(s => s.YeuCauTiepNhan.ThoiDiemTiepNhan >= baoCaoBenhNhanKhamNgoaiTruVo.TuNgay && s.YeuCauTiepNhan.ThoiDiemTiepNhan <= baoCaoBenhNhanKhamNgoaiTruVo.DenNgay && (baoCaoBenhNhanKhamNgoaiTruVo.PhongId != 0 ? s.NoiDangKyId == baoCaoBenhNhanKhamNgoaiTruVo.PhongId : s.NoiDangKy.KhoaPhongId == baoCaoBenhNhanKhamNgoaiTruVo.KhoaId) && s.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                                      .Select(o => new BaoCaoBenhNhanKhamNgoaiTruGridVo()
                                                      {
                                                          ThoiGianTiepNhan = o.YeuCauTiepNhan.ThoiDiemTiepNhan,
                                                          MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                          HoTenBn = o.YeuCauTiepNhan.HoTen,
                                                          GioiTinh = o.YeuCauTiepNhan.GioiTinh != null ? o.YeuCauTiepNhan.GioiTinh.GetValueOrDefault().GetDescription() : "",
                                                          TheBHYT = o.YeuCauTiepNhan.BHYTMaSoThe,
                                                          PhieuKham = o.TenDichVu,
                                                          PhongKham = o.NoiThucHienId != null ? o.NoiThucHien.Ten : "",
                                                          NoiThucHien = o.NoiThucHienId != null ? o.NoiThucHien.Ten : "",
                                                          PhongKhamId = o.NoiThucHienId != null ? o.NoiThucHienId : 0,
                                                          ICD = o.Icdchinh.Ma + "-" + o.Icdchinh.TenTiengViet,
                                                          TrangThai = o.TrangThai.GetDescription(),
                                                          BsKham = o.BacSiThucHienId != null ? o.BacSiThucHien.User.HoTen : "",
                                                          BsKetLuan = o.BacSiKetLuanId != null ? o.BacSiKetLuan.User.HoTen : "",
                                                          /*ThoiGianThanhToan = o.TaiKhoanBenhNhanChis.Any() ?  o.TaiKhoanBenhNhanChis.Where(d=>d.DaHuy != true).Select(s=>s.NgayChi.ApplyFormatDateTimeSACH()).LastOrDefault() : "",*/
                                                          CachGiaiQuyet = o.CachGiaiQuyet,
                                                          NgoaiGioHanhChinh = (o.ThoiDiemThucHien.GetValueOrDefault().Hour >= 16 && o.ThoiDiemThucHien.GetValueOrDefault().Minute >= 45) &&
                                                                             (o.ThoiDiemThucHien.GetValueOrDefault().Hour <= 7 && o.ThoiDiemThucHien.GetValueOrDefault().Minute == 0) ? "V" : ""
                                                      }).ToList();
            var totalGroup = 0;
            var phongKhams = entityData.GroupBy(x => new
            {
                x.PhongKham
            }).Select(s => new
            {
                PhongKhamId = s.FirstOrDefault().PhongKhamId,
                TenPhongKham = s.FirstOrDefault().PhongKham
            }).ToList();

            List<BaoCaoBenhNhanKhamNgoaiTruGridVo> list = new List<BaoCaoBenhNhanKhamNgoaiTruGridVo>();
            foreach (var item in phongKhams)
            {
                foreach (var itemx in entityData.ToList())
                {
                    if (item.PhongKhamId == itemx.PhongKhamId)
                    {
                        totalGroup++;
                    }
                }
                foreach (var itemk in entityData.ToList())
                {
                    if (itemk.PhongKhamId == item.PhongKhamId)
                    {
                        itemk.PhongKham = "";
                        itemk.PhongKham = item.TenPhongKham + "/" + totalGroup;
                        list.Add(itemk);
                    }
                }
                totalGroup = 0;
            }
            var table = "";
            if (list.Any())
            {
                int stt = 1;
                string taltolGroup = "";
                foreach (var item in phongKhams)
                {
                    foreach (var itemx in list)
                    {
                        if (item.PhongKhamId == itemx.PhongKhamId)
                        {
                            var total = itemx.PhongKham.Split('/');

                            if (total[1] != null)
                            {
                                taltolGroup = total[1];
                            }
                            else
                            {
                                taltolGroup = "";
                            }
                            table += "<tr>" +
                                     "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + stt + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.ThoiGianTiepNhanString + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.MaTN + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.HoTenBn + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.NgaySinh + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.GioiTinh + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.TheBHYT + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.PhieuKham + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.NoiThucHien + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.ICD + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.TrangThai + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.BsKham + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.BsKetLuan + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.ThoiGianThanhToan + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.CachGiaiQuyet + "</td>"
                                     + "<td style='border: 1px solid #020000; border-collapse: collapse;'>" + itemx.NgoaiGioHanhChinh + "</td>"
                                     + "</tr>";
                            stt++;
                        }

                    }
                    table += "<tr>"
                             + "<td colspan='8' style='border: 1px solid #020000; border-collapse: collapse;'>" + item.TenPhongKham + "</td>"
                             + "<td  style='border: 1px solid #020000; border-collapse: collapse;text-align:right;'>" + taltolGroup + "</td>"
                             + "<td  style='border: 1px solid #020000; border-collapse: collapse;text-align:right;'>&nbsp;</td>"
                             + "<td  style='border: 1px solid #020000; border-collapse: collapse;text-align:right;'>&nbsp;</td>"
                             + "<td  style='border: 1px solid #020000; border-collapse: collapse;text-align:right;'>&nbsp;</td>"
                             + "<td  style='border: 1px solid #020000; border-collapse: collapse;text-align:right;'>&nbsp;</td>"
                             + "<td  style='border: 1px solid #020000; border-collapse: collapse;text-align:right;'>&nbsp;</td>"
                             + "<td  style='border: 1px solid #020000; border-collapse: collapse;text-align:right;'>&nbsp;</td>"
                             + "<td  style='border: 1px solid #020000; border-collapse: collapse;text-align:right;'>&nbsp;</td>"
                             + "</tr>";
                    taltolGroup = "";
                }
            }
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            string phongBenhVien = _phongBenhVienRepository.TableNoTracking.Where(s => s.Id == baoCaoBenhNhanKhamNgoaiTruVo.PhongId).Select(s => s.Ten).FirstOrDefault();
            var data = new
            {
                LogoUrl = baoCaoBenhNhanKhamNgoaiTruVo.Hosting + "/assets/img/logo-bacha-full.png",
                ThoiGianChonBaoCao = baoCaoBenhNhanKhamNgoaiTruVo.TuNgay.GetValueOrDefault().ApplyFormatDateTimeSACH() + " - " + baoCaoBenhNhanKhamNgoaiTruVo.DenNgay.GetValueOrDefault().ApplyFormatDateTimeSACH(),
                PhongKham = phongBenhVien,
                NguoiLap = nguoiLogin,
                BaoCaoBenhNhanKhamNgoaiTru = table,
                NgayHienTai = DateTime.Now.Day,
                ThangHienTai = DateTime.Now.Month,
                NamHienTai = DateTime.Now.Year
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        #endregion
        #region Báo cáo bệnh nhân khám ngoại trú
        //public virtual byte[] ExportBaoCaoBenhNhanKhamNgoaiTru(ICollection<BaoCaoBenhNhanKhamNgoaiTruGridVo> datalinhs, DateTime? tuNgay, DateTime? denNgay, long PhongId,string hosting)
        //{
        //    var queryInfo = new BaoCaoBenhNhanKhamNgoaiTruGridVo();
        //    int ind = 1;
        //    var requestProperties = new[]
        //    {
        //        new PropertyByName<BaoCaoBenhNhanKhamNgoaiTruGridVo>("STT", p => ind++)
        //    };
        //    using (var stream = new MemoryStream())
        //    {
        //        using (var xlPackage = new ExcelPackage(stream))
        //        {
        //            var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO NGƯỜI BỆNH KHÁM NGOẠI TRÚ");

        //            // set row

        //            worksheet.Row(9).Height = 24.5;
        //            worksheet.DefaultRowHeight = 25;
        //            worksheet.Column(1).Width = 10;
        //            worksheet.Column(2).Width = 30;
        //            worksheet.Column(3).Width = 30;
        //            worksheet.Column(4).Width = 30;
        //            worksheet.Column(5).Width = 30;
        //            worksheet.Column(6).Width = 30;
        //            worksheet.Column(7).Width = 30;
        //            worksheet.Column(8).Width = 30;
        //            worksheet.Column(9).Width = 30;
        //            worksheet.Column(10).Width = 30;
        //            worksheet.Column(11).Width = 30;
        //            worksheet.Column(12).Width = 30;
        //            worksheet.Column(13).Width = 30;
        //            worksheet.Column(14).Width = 30;
        //            worksheet.Column(15).Width = 30;
        //            worksheet.Column(16).Width = 30;
        //            worksheet.Column(17).Width = 30;

        //            worksheet.DefaultColWidth = 17;

        //            //set column 
        //            string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P" };
        //            var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
        //            var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
        //            var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
        //            var worksheetPhong = SetColumnItems[0] + 4 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 4;
        //            var worksheetTitleHeader = SetColumnItems[0] + 4 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 7;

        //            using (var range = worksheet.Cells["A1:C1"])
        //            {
        //                range.Worksheet.Cells[worksheetTitle].Merge = true;
        //                var url = hosting + "/assets/img/logo-bacha-full.png";
        //                WebClient wc = new WebClient();
        //                byte[] bytes = wc.DownloadData(url); // download file từ server
        //                MemoryStream ms = new MemoryStream(bytes); //
        //                Image img = Image.FromStream(ms); // chuyển đổi thành img
        //                ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
        //                pic.SetPosition(0, 0, 0, 0);
        //                var height = 80; // chiều cao từ A1 đến A6
        //                var width = 400; // chiều rộng từ A1 đến D1
        //                pic.SetSize(width, height);
        //                range.Worksheet.Protection.IsProtected = false;
        //                range.Worksheet.Protection.AllowSelectLockedCells = false;
        //            }

        //            using (var range = worksheet.Cells[worksheetTitle])
        //            {
        //                range.Worksheet.Cells[worksheetTitle].Merge = true;
        //                range.Worksheet.Cells[worksheetTitle].Value = "BÁO CÁO NGƯỜI BỆNH KHÁM NGOẠI TRÚ".ToUpper();
        //                range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
        //                range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
        //                range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
        //            }

        //            using (var range = worksheet.Cells[worksheetTitleStatus])
        //            {
        //                range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
        //                range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
        //                range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
        //                range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
        //            }

        //            using (var range = worksheet.Cells[worksheetTitleNgay])
        //            {
        //                range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
        //                range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
        //                range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
        //                range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
        //                range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
        //                range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
        //                range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
        //            }
        //            string phongBenhVien = _phongBenhVienRepository.TableNoTracking.Where(s => s.Id == PhongId).Select(s => s.Ten).FirstOrDefault();
        //            using (var range = worksheet.Cells[worksheetPhong])
        //            {
        //                range.Worksheet.Cells[worksheetPhong].Merge = true;
        //                range.Worksheet.Cells[worksheetPhong].Value = "Phòng khám: " + phongBenhVien == null ? "Tất cả các phòng" : phongBenhVien;
        //                range.Worksheet.Cells[worksheetPhong].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Worksheet.Cells[worksheetPhong].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
        //                range.Worksheet.Cells[worksheetPhong].Style.Font.SetFromFont(new Font("Times New Roman", 14));
        //                range.Worksheet.Cells[worksheetPhong].Style.Font.Color.SetColor(Color.Black);
        //                range.Worksheet.Cells[worksheetPhong].Style.Font.Bold = true;
        //                range.Worksheet.Cells[worksheetPhong].Style.Font.Italic = true;
        //            }

        //            using (var range = worksheet.Cells[worksheetTitleHeader])
        //            {
        //                range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
        //                range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
        //                range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

        //                string[,] SetColumns ={ { "A" , "STT" }, { "B", "Thời gian TN" }, { "C", "Mã TN" } , { "D", "Họn tên BN" },
        //                            { "E", "Ngày sinh" }, { "F", "Giới tính" },{ "G", "Thẻ BHYT" },{ "H", "Phiếu khám" },{ "I", "Phòng khám" },
        //                            { "J", "ICD" },{ "K", "Trạng thái" },{ "L", "BS khám" },{ "M", "BS kết luận" },{ "N", "Thời gian thanh toán" },
        //                            { "O", "Cách giải quyết" },{ "P", "Ngoài giờ hành chính" }};

        //                for (int i = 0; i < SetColumns.Length / 2; i++)
        //                {
        //                    var setColumn = ((SetColumns[i, 0]).ToString() + 5 + ":" + (SetColumns[i, 0]).ToString() + 7).ToString();
        //                    range.Worksheet.Cells[setColumn].Merge = true;
        //                    range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
        //                }

        //                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            }

        //            var manager = new PropertyManager<BaoCaoBenhNhanKhamNgoaiTruGridVo>(requestProperties);
        //            int index = 7;
        //            var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

        //            int stt = 1;
        //            var totalGroup = 0;
        //            var phongKhams = datalinhs.GroupBy(x => new
        //            {
        //                x.PhongKham
        //            }).Select(s => new
        //            {
        //                PhongKhamId = s.FirstOrDefault().PhongKhamId,
        //                TenPhongKham = s.FirstOrDefault().NoiThucHien
        //            }).ToList();

        //            List<BaoCaoBenhNhanKhamNgoaiTruGridVo> list = new List<BaoCaoBenhNhanKhamNgoaiTruGridVo>();
        //            foreach (var item in phongKhams)
        //            {
        //                foreach (var itemx in datalinhs.ToList())
        //                {
        //                    if (item.PhongKhamId == itemx.PhongKhamId)
        //                    {
        //                        totalGroup++;
        //                    }
        //                }
        //                foreach (var itemk in datalinhs.ToList())
        //                {
        //                    if (itemk.PhongKhamId == item.PhongKhamId)
        //                    {
        //                        itemk.PhongKham = "";
        //                        itemk.PhongKham = item.TenPhongKham + "/" + totalGroup;
        //                        list.Add(itemk);
        //                    }
        //                }
        //                totalGroup = 0;
        //            }
        //            var taltolGroups = "";
        //            foreach (var baoCaoGroup in phongKhams)
        //            {
        //                foreach (var baoCao in list)
        //                {
        //                    manager.CurrentObject = baoCao;
        //                    manager.WriteToXlsx(worksheet, index);
        //                    if (baoCaoGroup.PhongKhamId == baoCao.PhongKhamId)
        //                    {
        //                        var total = baoCao.PhongKham.Split('/');

        //                        if (total[1] != null)
        //                        {
        //                            taltolGroups = total[1];
        //                        }
        //                        else
        //                        {
        //                            taltolGroups = "";
        //                        }
        //                        worksheet.Cells["A" + index].Value = stt;
        //                        worksheet.Cells["B" + index].Value = baoCao.ThoiGianTiepNhanString;
        //                        worksheet.Cells["C" + index].Value = baoCao.MaTN;
        //                        worksheet.Cells["D" + index].Value = baoCao.HoTenBn;
        //                        worksheet.Cells["E" + index].Value = baoCao.NgaySinh;
        //                        worksheet.Cells["F" + index].Value = baoCao.GioiTinh;
        //                        worksheet.Cells["G" + index].Value = baoCao.TheBHYT;
        //                        worksheet.Cells["H" + index].Value = baoCao.PhieuKham;
        //                        worksheet.Cells["I" + index].Value = baoCao.NoiThucHien;
        //                        worksheet.Cells["J" + index].Value = baoCao.ICD;
        //                        worksheet.Cells["K" + index].Value = baoCao.TrangThai;
        //                        worksheet.Cells["L" + index].Value = baoCao.BsKham;
        //                        worksheet.Cells["M" + index].Value = baoCao.BsKetLuan;
        //                        worksheet.Cells["N" + index].Value = baoCao.ThoiGianThanhToan;
        //                        worksheet.Cells["O" + index].Value = baoCao.CachGiaiQuyet;
        //                        worksheet.Cells["P" + index].Value = baoCao.NgoaiGioHanhChinh;
        //                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
        //                        {
        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
        //                        }
        //                        index++;
        //                        stt++;
        //                    }
        //                }
        //                worksheet.Cells["A" + index].Value = "";
        //                worksheet.Cells["B" + index].Value = baoCaoGroup.TenPhongKham;
        //                worksheet.Cells["C" + index].Value = "";
        //                worksheet.Cells["D" + index].Value = "";
        //                worksheet.Cells["E" + index].Value = "";
        //                worksheet.Cells["F" + index].Value = "";
        //                worksheet.Cells["G" + index].Value = "";
        //                worksheet.Cells["H" + index].Value = "";
        //                worksheet.Cells["I" + index].Value = taltolGroups;
        //                worksheet.Cells["J" + index].Value = "";
        //                worksheet.Cells["K" + index].Value = "";
        //                worksheet.Cells["L" + index].Value = "";
        //                worksheet.Cells["M" + index].Value = "";
        //                worksheet.Cells["N" + index].Value = "";
        //                worksheet.Cells["O" + index].Value = "";
        //                worksheet.Cells["P" + index].Value = "";
        //                worksheet.Row(index).Height = 20.5;
        //                for (int ii = 0; ii < SetColumnItems.Length; ii++)
        //                {
        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
        //                }
        //                index++;
        //                var indexMain = index;
        //                for (int inde = indexMain + 1; inde <= index - 1; inde++)
        //                {
        //                    worksheet.Row(inde).OutlineLevel = 1;
        //                }
        //            }

        //            xlPackage.Save();
        //        }

        //        return stream.ToArray();
        //    }
        //}

        public virtual byte[] ExportBaoCaoBenhNhanKhamNgoaiTru(GridDataSource gridDataSource, BaoCaoBenhNhanKhamNgoaiTruQueryInfo query)
        {
            var phongBenhVien =
                _phongBenhVienRepository.TableNoTracking.FirstOrDefault(x => x.Id == query.PhongId);

            var datas = (ICollection<BaoCaoBenhNhanKhamNgoaiTruDemoGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO NGƯỜI BỆNH KHÁM NGOẠI TRÚ");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 25;
                    worksheet.Column(7).Width = 10;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 25;
                    worksheet.Column(14).Width = 25;
                    worksheet.Column(15).Width = 25;
                    worksheet.Column(16).Width = 25;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 25;
                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:E1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:N3"])
                    {
                        range.Worksheet.Cells["A3:N3"].Merge = true;
                        range.Worksheet.Cells["A3:N3"].Value = "BÁO CÁO NGƯỜI BỆNH KHÁM NGOẠI TRÚ";
                        range.Worksheet.Cells["A3:N3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:N3"].Style.Font.SetFromFont(new Font("Times New Roman", 17));
                        range.Worksheet.Cells["A3:N3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:N3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:N4"])
                    {
                        range.Worksheet.Cells["A4:N4"].Merge = true;
                        range.Worksheet.Cells["A4:N4"].Value = "Từ ngày: " + query.FromDate.FormatNgayGioTimKiemTrenBaoCao()
                                                         + " - đến ngày: " + query.ToDate.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:N4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:N4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:N4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:N4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:N4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:N5"])
                    {
                        range.Worksheet.Cells["A5:N5"].Merge = true;
                        range.Worksheet.Cells["A5:N5"].Value = "Phòng khám: Tất cả phòng khám:" + (phongBenhVien != null ? phongBenhVien.Ten : "");
                        range.Worksheet.Cells["A5:N5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:N5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:N5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:N5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:R7"])
                    {
                        range.Worksheet.Cells["A7:R7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:R7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:R7"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A7:R7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:R7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:R7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "Thời gian khám";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Đoàn KSK";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Mã TN";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Họ tên NB";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Ngày sinh";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "Giới tính";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "Thẻ BHYT";

                        range.Worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Phiếu khám";

                        range.Worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7"].Value = "Phòng khám";

                        range.Worksheet.Cells["K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K7"].Value = "ICD";

                        range.Worksheet.Cells["L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7"].Value = "Trang thái";

                        range.Worksheet.Cells["M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M7"].Value = "BS khám";

                        range.Worksheet.Cells["N7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N7"].Value = "BS kết luận";

                        range.Worksheet.Cells["O7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O7"].Value = "Cách giải quyết";

                        range.Worksheet.Cells["P7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P7"].Value = "Khoa nhập viện";

                        range.Worksheet.Cells["Q7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q7"].Value = "NB gói";

                        range.Worksheet.Cells["R7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R7"].Value = "Ngoài giờ hành chính";
                    }

                    int index = 8;
                    //Đổ data vào bảng excel
                    var dataTheoPhong = datas.GroupBy(x => x.PhongKhamId).Select(x => x.Key);
                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var data in dataTheoPhong)
                        {
                            var listNhomTheoPhong = datas.Where(o => o.PhongKhamId == data.Value).ToList();
                            if (listNhomTheoPhong.Any())
                            {
                                foreach (var benhNhan in listNhomTheoPhong)
                                {
                                    // format border, font chữ,....
                                    //worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    //worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                    //worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                    worksheet.Row(index).Height = 20.5;
                                    // Đổ data
                                    worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Cells["A" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["A" + index].Value = stt;

                                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["B" + index].Value = benhNhan.ThoiGianKhamDisplay;

                                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["C" + index].Value = benhNhan.CongTyKhamSucKhoe;

                                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["D" + index].Value = benhNhan.MaYeuCauTiepNhan;

                                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["E" + index].Value = benhNhan.HoTen;

                                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["F" + index].Value = benhNhan.NgaySinhDisplay;

                                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["G" + index].Value = benhNhan.GioiTinh;

                                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["H" + index].Value = benhNhan.TheBHYT;

                                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["I" + index].Value = benhNhan.PhieuKham;

                                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["J" + index].Value = benhNhan.PhongKham;

                                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["K" + index].Value = benhNhan.ICD;

                                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["L" + index].Value = benhNhan.TrangThai;

                                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["M" + index].Value = benhNhan.BacSiKham;

                                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["N" + index].Value = benhNhan.BacSiKetLuan;

                                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["O" + index].Value = benhNhan.CachGiaiQuyet;

                                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells["P" + index].Value = benhNhan.KhoaNhapVien;

                                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells["Q" + index].Value = benhNhan.SuDungGoi ? "X" : string.Empty;

                                    worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells["R" + index].Value = benhNhan.NgoaiGioHanhChinh ? "X" : string.Empty;
                                    index++;
                                    stt++;
                                }

                                worksheet.Cells["B" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["B" + index + ":I" + index].Style.Font.Bold = true;
                                worksheet.Cells["B" + index + ":I" + index].Merge = true;
                                worksheet.Cells["B" + index + ":I" + index].Value = listNhomTheoPhong.FirstOrDefault().PhongKham;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["J" + index].Style.Font.Bold = true;
                                worksheet.Cells["J" + index].Value = listNhomTheoPhong.Count(); //listNhomTheoPhong.FirstOrDefault().PhongKhamId;
                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                index++;
                            }
                        }

                        xlPackage.Save();
                    }
                    return stream.ToArray();
                }
            }
        }
        #endregion
        #endregion
        #region báo cáo kết quả khám chưa bệnh 24/2/2021
        public string GetDataTemplateBaoCaoKetQuaKhamChuaBenhAsync(BaoCaoKetQuaKhamChuaBenhVo baoCaoKetQuaKhamChuaBenhVo)
        {
            // data test
            List<KetQuaKCBVaDoanhThuNhaThuoc> listKetQuaKCBVaDoanhThuNhaThuoc = new List<KetQuaKCBVaDoanhThuNhaThuoc>();

            // 1.  kết quả khám chữa bệnh
            KetQuaKCBVaDoanhThuNhaThuoc ketQuaKCBVaDoanhThuNhaThuoc = new KetQuaKCBVaDoanhThuNhaThuoc();
            ketQuaKCBVaDoanhThuNhaThuoc.Ten = "Kết quả khám chữa bệnh";
            //KetQuaKCBVaDoanhThuNhaThuoc.KetQuaKCBList = new List<KetQuaKCB>();
            List<KetQuaKCB> ketQuaKCBs = new List<KetQuaKCB>();
            KetQuaKCB ketQuaKCB = new KetQuaKCB();
            ketQuaKCB.MaHinhThucDen = "Ma1";
            ketQuaKCB.TenHinhThucDen = "- Theo hình thức đến";
            // create loại hình thức
            LoaiHinhThuc loaiHinhThuc = new LoaiHinhThuc();
            loaiHinhThuc.TenLoaiHinhThuc = "+ Tự đến : tự đến";
            loaiHinhThuc.MaLoaiHinhThuc = "MATD";
            ketQuaKCB.LoaiHinhThucList.Add(loaiHinhThuc);

            LoaiHinhThuc loaiHinhThucs = new LoaiHinhThuc();
            loaiHinhThucs.TenLoaiHinhThuc = "+ Giới thiệu : BS Hòa";
            loaiHinhThucs.MaLoaiHinhThuc = "BSHoa";
            ketQuaKCB.LoaiHinhThucList.Add(loaiHinhThucs);

            LoaiHinhThuc loaiHinhThucss = new LoaiHinhThuc();
            loaiHinhThucss.TenLoaiHinhThuc = "+ Giới thiệu : BS An";
            loaiHinhThucss.MaLoaiHinhThuc = "BSAn";
            ketQuaKCB.LoaiHinhThucList.Add(loaiHinhThucss);

            LoaiHinhThuc loaiHinhThucsss = new LoaiHinhThuc();
            loaiHinhThucsss.TenLoaiHinhThuc = "+ Hình thức khác : Hình thức khác";
            loaiHinhThucsss.MaLoaiHinhThuc = "HTK";
            ketQuaKCB.LoaiHinhThucList.Add(loaiHinhThucsss);
            // crate đối tượng người bệnh
            List<DoiTuongBN> listDoiTuongBNs = new List<DoiTuongBN>();
            DoiTuongBN doiTuongBN = new DoiTuongBN();

            doiTuongBN.MaDoiTuongBN = " + Người bệnh nhi:";
            doiTuongBN.MaDoiTuongBN = "BNN";
            ketQuaKCB.DoiTuongBNList.Add(doiTuongBN);

            doiTuongBN.MaDoiTuongBN = " + Răng hàm mặt:";
            doiTuongBN.MaDoiTuongBN = "RHM";
            ketQuaKCB.DoiTuongBNList.Add(doiTuongBN);

            doiTuongBN.MaDoiTuongBN = " + Thai sản:";
            doiTuongBN.MaDoiTuongBN = "TS";
            ketQuaKCB.DoiTuongBNList.Add(doiTuongBN);

            // tạo  kết quả khám chữa bệnh
            ketQuaKCBs.Add(ketQuaKCB);
            ketQuaKCBVaDoanhThuNhaThuoc.KetQuaKCBList.Add(ketQuaKCB);
            listKetQuaKCBVaDoanhThuNhaThuoc.Add(ketQuaKCBVaDoanhThuNhaThuoc);

            // data doanh thu nhà thuốc
            KetQuaKCBVaDoanhThuNhaThuoc ketQuaKCBVaDoanhThuNhaThuocss = new KetQuaKCBVaDoanhThuNhaThuoc();
            ketQuaKCBVaDoanhThuNhaThuocss.Ten = "Doanh thu thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.SoDuDauKy = 100000;
            //ketQuaKCBVaDoanhThuNhaThuoc.DoanhThuNhaThuocList
            DoanhThuNhaThuoc doanhThuNhaThuoc = new DoanhThuNhaThuoc();

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 1);
            doanhThuNhaThuoc.SoNguoi = 100;
            doanhThuNhaThuoc.DoanhThu = 100000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 2);
            doanhThuNhaThuoc.SoNguoi = 101;
            doanhThuNhaThuoc.DoanhThu = 110000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 3);
            doanhThuNhaThuoc.SoNguoi = 103;
            doanhThuNhaThuoc.DoanhThu = 130000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 4);
            doanhThuNhaThuoc.SoNguoi = 104;
            doanhThuNhaThuoc.DoanhThu = 140000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 5);
            doanhThuNhaThuoc.SoNguoi = 105;
            doanhThuNhaThuoc.DoanhThu = 150000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 6);
            doanhThuNhaThuoc.SoNguoi = 106;
            doanhThuNhaThuoc.DoanhThu = 160000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            listKetQuaKCBVaDoanhThuNhaThuoc.Add(ketQuaKCBVaDoanhThuNhaThuocss);
            // tổng tiền mặt
            var ngayVaoVien = baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().ApplyFormatDate();
            var ngayRaVien = baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().ApplyFormatDate();
            DateTime start = new DateTime(baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Year, baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Month, baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Day);
            DateTime end = new DateTime(baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Year, baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Month, baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Day);
            TimeSpan difference = end - start; //create TimeSpan object
            var tmp = difference.TotalDays + 1;
            List<NgayThangNamTongTien> listNgayInItems = new List<NgayThangNamTongTien>();

            for (var i = 0; i < tmp; i++)
            {
                NgayThangNamTongTien listNgayIn = new NgayThangNamTongTien();
                listNgayIn.NgayThangNam = baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().AddDays(i);
                listNgayInItems.Add(listNgayIn);
            }
            List<TongTienMat> tongTienMats = new List<TongTienMat>();
            // mã Tm
            TongTienMat tongTienMatObj = new TongTienMat();
            tongTienMatObj.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObj.MaTongTienMat = "MaTM";
            tongTienMatObj.SoDuDauKy = 100;
            tongTienMatObj.SoNguoi = 10;
            tongTienMatObj.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObj);

            TongTienMat tongTienMatObjs = new TongTienMat();
            tongTienMatObjs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjs.MaTongTienMat = "MaTM";
            tongTienMatObjs.SoDuDauKy = 100;
            tongTienMatObjs.SoNguoi = 10;
            tongTienMatObjs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjs);

            TongTienMat tongTienMatObjss = new TongTienMat();
            tongTienMatObjss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjss.MaTongTienMat = "MaTM";
            tongTienMatObjss.SoDuDauKy = 100;
            tongTienMatObjss.SoNguoi = 10;
            tongTienMatObjss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjss);

            TongTienMat tongTienMatObjsss = new TongTienMat();
            tongTienMatObjsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjsss.MaTongTienMat = "MaTM";
            tongTienMatObjsss.SoDuDauKy = 100;
            tongTienMatObjsss.SoNguoi = 10;
            tongTienMatObjsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjsss);

            TongTienMat tongTienMatObjssss = new TongTienMat();
            tongTienMatObjssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjssss.MaTongTienMat = "MaTM";
            tongTienMatObjssss.SoDuDauKy = 100;
            tongTienMatObjssss.SoNguoi = 10;
            tongTienMatObjssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjssss);

            TongTienMat tongTienMatObjsssss = new TongTienMat();
            tongTienMatObjsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjsssss.MaTongTienMat = "MaTM";
            tongTienMatObjsssss.SoDuDauKy = 100;
            tongTienMatObjsssss.SoNguoi = 10;
            tongTienMatObjsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjsssss);

            // CK
            TongTienMat tongTienMatObjCK = new TongTienMat();
            tongTienMatObjCK.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObjCK.MaTongTienMat = "MaCK";
            tongTienMatObjCK.SoDuDauKy = 100;
            tongTienMatObjCK.SoNguoi = 10;
            tongTienMatObjCK.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCK);

            TongTienMat tongTienMatObjCKs = new TongTienMat();
            tongTienMatObjCKs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjCKs.MaTongTienMat = "MaCK";
            tongTienMatObjCKs.SoDuDauKy = 100;
            tongTienMatObjCKs.SoNguoi = 10;
            tongTienMatObjCKs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKs);

            TongTienMat tongTienMatObjCKss = new TongTienMat();
            tongTienMatObjCKss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjCKss.MaTongTienMat = "MaCK";
            tongTienMatObjCKss.SoDuDauKy = 100;
            tongTienMatObjCKss.SoNguoi = 10;
            tongTienMatObjCKss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKss);

            TongTienMat tongTienMatObjCKsss = new TongTienMat();
            tongTienMatObjCKsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjCKsss.MaTongTienMat = "MaCK";
            tongTienMatObjCKsss.SoDuDauKy = 100;
            tongTienMatObjCKsss.SoNguoi = 10;
            tongTienMatObjCKsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKsss);

            TongTienMat tongTienMatObjCKssss = new TongTienMat();
            tongTienMatObjCKssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjCKssss.MaTongTienMat = "MaCK";
            tongTienMatObjCKssss.SoDuDauKy = 100;
            tongTienMatObjCKssss.SoNguoi = 10;
            tongTienMatObjCKssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKssss);

            TongTienMat tongTienMatObjCKsssss = new TongTienMat();
            tongTienMatObjCKsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjCKsssss.MaTongTienMat = "MaCK";
            tongTienMatObjCKsssss.SoDuDauKy = 100;
            tongTienMatObjCKsssss.SoNguoi = 10;
            tongTienMatObjCKsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKsssss);

            // pos
            TongTienMat tongTienMatObjP = new TongTienMat();
            tongTienMatObjP.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObjP.MaTongTienMat = "MaQP";
            tongTienMatObjP.SoDuDauKy = 100;
            tongTienMatObjP.SoNguoi = 10;
            tongTienMatObjP.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjP);

            TongTienMat tongTienMatObjPs = new TongTienMat();
            tongTienMatObjPs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjPs.MaTongTienMat = "MaQP";
            tongTienMatObjPs.SoDuDauKy = 100;
            tongTienMatObjPs.SoNguoi = 10;
            tongTienMatObjPs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPs);

            TongTienMat tongTienMatObjPss = new TongTienMat();
            tongTienMatObjPss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjPss.MaTongTienMat = "MaQP";
            tongTienMatObjPss.SoDuDauKy = 100;
            tongTienMatObjPss.SoNguoi = 10;
            tongTienMatObjPss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPss);

            TongTienMat tongTienMatObjPsss = new TongTienMat();
            tongTienMatObjPsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjPsss.MaTongTienMat = "MaQP";
            tongTienMatObjPsss.SoDuDauKy = 100;
            tongTienMatObjPsss.SoNguoi = 10;
            tongTienMatObjPsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPsss);

            TongTienMat tongTienMatObjPssss = new TongTienMat();
            tongTienMatObjPssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjPssss.MaTongTienMat = "MaQP";
            tongTienMatObjPssss.SoDuDauKy = 100;
            tongTienMatObjPssss.SoNguoi = 10;
            tongTienMatObjPssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPssss);

            TongTienMat tongTienMatObjPsssss = new TongTienMat();
            tongTienMatObjPsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjPsssss.MaTongTienMat = "MaQP";
            tongTienMatObjPsssss.SoDuDauKy = 100;
            tongTienMatObjPsssss.SoNguoi = 10;
            tongTienMatObjPsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPsssss);

            // MACT
            TongTienMat tongTienMatObjCT = new TongTienMat();
            tongTienMatObjCT.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObjCT.MaTongTienMat = "MACT";
            tongTienMatObjCT.SoDuDauKy = 100;
            tongTienMatObjCT.SoNguoi = 10;
            tongTienMatObjCT.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCT);

            TongTienMat tongTienMatObjCTs = new TongTienMat();
            tongTienMatObjCTs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjCTs.MaTongTienMat = "MACT";
            tongTienMatObjCTs.SoDuDauKy = 100;
            tongTienMatObjCTs.SoNguoi = 10;
            tongTienMatObjCTs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTs);

            TongTienMat tongTienMatObjCTss = new TongTienMat();
            tongTienMatObjCTss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjCTss.MaTongTienMat = "MACT";
            tongTienMatObjCTss.SoDuDauKy = 100;
            tongTienMatObjCTss.SoNguoi = 10;
            tongTienMatObjCTss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTss);

            TongTienMat tongTienMatObjCTsss = new TongTienMat();
            tongTienMatObjCTsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjCTsss.MaTongTienMat = "MACT";
            tongTienMatObjCTsss.SoDuDauKy = 100;
            tongTienMatObjCTsss.SoNguoi = 10;
            tongTienMatObjCTsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTsss);

            TongTienMat tongTienMatObjCTssss = new TongTienMat();
            tongTienMatObjCTssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjCTssss.MaTongTienMat = "MACT";
            tongTienMatObjCTssss.SoDuDauKy = 100;
            tongTienMatObjCTssss.SoNguoi = 10;
            tongTienMatObjCTssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTssss);

            TongTienMat tongTienMatObjCTsssss = new TongTienMat();
            tongTienMatObjCTsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjCTsssss.MaTongTienMat = "MACT";
            tongTienMatObjCTsssss.SoDuDauKy = 100;
            tongTienMatObjCTsssss.SoNguoi = 10;
            tongTienMatObjCTsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTsssss);

            //MG
            TongTienMat tongTienMatObjMG = new TongTienMat();
            tongTienMatObjMG.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObjMG.MaTongTienMat = "MaMG";
            tongTienMatObjMG.SoDuDauKy = 100;
            tongTienMatObjMG.SoNguoi = 10;
            tongTienMatObjMG.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMG);

            TongTienMat tongTienMatObjMGs = new TongTienMat();
            tongTienMatObjMGs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjMGs.MaTongTienMat = "MaMG";
            tongTienMatObjMGs.SoDuDauKy = 100;
            tongTienMatObjMGs.SoNguoi = 10;
            tongTienMatObjMGs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGs);

            TongTienMat tongTienMatObjMGss = new TongTienMat();
            tongTienMatObjMGss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjMGss.MaTongTienMat = "MaMG";
            tongTienMatObjMGss.SoDuDauKy = 100;
            tongTienMatObjMGss.SoNguoi = 10;
            tongTienMatObjMGss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGss);

            TongTienMat tongTienMatObjMGsss = new TongTienMat();
            tongTienMatObjMGsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjMGsss.MaTongTienMat = "MaMG";
            tongTienMatObjMGsss.SoDuDauKy = 100;
            tongTienMatObjMGsss.SoNguoi = 10;
            tongTienMatObjMGsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGsss);

            TongTienMat tongTienMatObjMGssss = new TongTienMat();
            tongTienMatObjMGssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjMGssss.MaTongTienMat = "MaMG";
            tongTienMatObjMGssss.SoDuDauKy = 100;
            tongTienMatObjMGssss.SoNguoi = 10;
            tongTienMatObjMGssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGssss);

            TongTienMat tongTienMatObjMGsssss = new TongTienMat();
            tongTienMatObjMGsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjMGsssss.MaTongTienMat = "MaMG";
            tongTienMatObjMGsssss.SoDuDauKy = 100;
            tongTienMatObjMGsssss.SoNguoi = 10;
            tongTienMatObjMGsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGsssss);


            List<LoaiPhieuThu> loaiPhieuThus = new List<LoaiPhieuThu>();

            LoaiPhieuThu loaiPhieuThuObj = new LoaiPhieuThu();

            loaiPhieuThuObj.TenLoaiPhieuThu = "Tiền mặt";
            loaiPhieuThuObj.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObj.MaTongTienMat = "MaTM";
            loaiPhieuThuObj.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObj);

            LoaiPhieuThu loaiPhieuThuObjs = new LoaiPhieuThu();

            loaiPhieuThuObjs.TenLoaiPhieuThu = "Thẻ: Chuyển khoản";
            loaiPhieuThuObjs.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObjs.MaTongTienMat = "MaCK";
            loaiPhieuThuObjs.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObjs);

            LoaiPhieuThu loaiPhieuThuObjss = new LoaiPhieuThu();

            loaiPhieuThuObjss.TenLoaiPhieuThu = "Pos:Quẹt Pos";
            loaiPhieuThuObjss.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObjss.MaTongTienMat = "MaQP";
            loaiPhieuThuObjss.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObjss);

            LoaiPhieuThu loaiPhieuThuObjsss = new LoaiPhieuThu();

            loaiPhieuThuObjsss.TenLoaiPhieuThu = "BHYT: PhanBHYTChiTra";
            loaiPhieuThuObjsss.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObjsss.MaTongTienMat = "MACT";
            loaiPhieuThuObjsss.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObjsss);

            LoaiPhieuThu loaiPhieuThuObjssss = new LoaiPhieuThu();

            loaiPhieuThuObjssss.TenLoaiPhieuThu = "Miễn giảm";
            loaiPhieuThuObjssss.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObjssss.MaTongTienMat = "MaMG";
            loaiPhieuThuObjssss.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObjssss);

            //// công nợ -----------------------------
            List<CongNo> listCongNos = new List<CongNo>();
            CongNo congNoObj = new CongNo();
            congNoObj.SoDuDauKy = 100;
            congNoObj.SoNguoi = 100;
            congNoObj.DoanhThu = 100;
            congNoObj.NgayThangNam = new DateTime(2020, 3, 1);
            listCongNos.Add(congNoObj);

            CongNo congNoObjs = new CongNo();
            congNoObjs.SoDuDauKy = 100;
            congNoObjs.SoNguoi = 100;
            congNoObjs.DoanhThu = 100;
            congNoObjs.NgayThangNam = new DateTime(2020, 3, 2);
            listCongNos.Add(congNoObjs);

            CongNo congNoObjss = new CongNo();
            congNoObjss.SoDuDauKy = 100;
            congNoObjss.SoNguoi = 100;
            congNoObjss.DoanhThu = 100;
            congNoObjss.NgayThangNam = new DateTime(2020, 3, 3);
            listCongNos.Add(congNoObjss);

            CongNo congNoObjsss = new CongNo();
            congNoObjsss.SoDuDauKy = 100;
            congNoObjsss.SoNguoi = 100;
            congNoObjsss.DoanhThu = 100;
            congNoObjsss.NgayThangNam = new DateTime(2020, 3, 4);
            listCongNos.Add(congNoObjsss);

            CongNo congNoObjssss = new CongNo();
            congNoObjssss.SoDuDauKy = 100;
            congNoObjssss.SoNguoi = 100;
            congNoObjssss.DoanhThu = 100;
            congNoObjsss.NgayThangNam = new DateTime(2020, 3, 5);
            listCongNos.Add(congNoObjssss);


            CongNo congNoObjsssss = new CongNo();
            congNoObjsssss.SoDuDauKy = 100;
            congNoObjsssss.SoNguoi = 100;
            congNoObjsssss.DoanhThu = 100;
            congNoObjssss.NgayThangNam = new DateTime(2020, 3, 6);
            listCongNos.Add(congNoObjsssss);

            List<CongNoPhaiThu> listCongNoPhaiThus = new List<CongNoPhaiThu>();
            CongNoPhaiThu congNoPhaiThuObj = new CongNoPhaiThu();
            congNoPhaiThuObj.TenCongNo = "Công nợ còn phải thu của khách hàng";
            congNoPhaiThuObj.ListCongNo = listCongNos;
            listCongNoPhaiThus.Add(congNoPhaiThuObj);
            ///---------------------------------------

            ///
            var STT = 1;
            var baoCao = "";
            //var ngayVaoVien = baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().ApplyFormatDate();
            //var ngayRaVien = baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().ApplyFormatDate();
            //DateTime start = new DateTime(baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Year, baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Month, baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Day);
            //DateTime end = new DateTime(baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Year, baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Month, baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Day);
            //TimeSpan difference = end - start; //create TimeSpan object
            //var tmp = difference.TotalDays;
            int idSoPhieu = 1;
            List<ListDatePhieuBaoCao> listNgayInArr = new List<ListDatePhieuBaoCao>();
            List<DateTime> listNgayInItem = new List<DateTime>();
            for (var j = 0; j <= tmp; j++)
            {
                listNgayInItem.Add(baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().AddDays(j));
            }
            baoCao += "<table style='width:100%'>" +
                         "<tr>" +
                         "<th rowspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>STT</th>" +
                         "<th rowspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>Nội dung</th>" +
                         "<th rowspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>Số dư đầu kỳ(từ đến)</th>";

            foreach (var columsTitle in listNgayInItem)
            {
                baoCao += "<th  colspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + columsTitle + "</th>";
            }
            baoCao += "<th  colspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>Cộng</ th >" +
                      "</tr>" +
                      "<tr>";
            var length = listNgayInItem.Count();
            for (var columsTitle = 0; columsTitle <= length; columsTitle++)
            {
                baoCao += "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>Số người</th>" +
                          "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>Doanh thu</th>";
            }
            baoCao += "</tr>";
            // kết quả khám chữa người bệnh và daonh thu nhà thuốc
            baoCao += "<tbody>" +
                       "<tr>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + "I" + "</td>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: left;padding: 5px;text-align: center;'>" + "Kết quả KCB và doanh thu nhà thuốc" + "</td>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "</td>" +
                       "<td " + "colspan='" + ((length * 2) + 2) + "'style='border: 1px solid #020000; border-collapse: collapse;text-align: left;padding: 5px;text-align: center;'>" + "</td>" +
                       "</tr>";
            //+ "</tbody>";

            int stt = 1;
            foreach (var item in listKetQuaKCBVaDoanhThuNhaThuoc)
            {
                foreach (var itemx in item.KetQuaKCBList)
                {
                    baoCao += "<tr>" +
                             "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + 1 + "</th> " +
                             "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemx.TenHinhThucDen + "</th>" +
                             "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                    {
                        baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    }
                    baoCao += "</tr>";
                    foreach (var itemxx in itemx.LoaiHinhThucList)
                    {
                        baoCao += "<tr>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "&nbsp;" + "</th> " +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemxx.TenLoaiHinhThuc + "</th>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                        for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                        {
                            baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                        }
                    }
                    baoCao += "</tr>";
                    baoCao += "<tr>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "&nbsp;" + "</th> " +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    baoCao += "</tr>";
                    baoCao += "<tr>" +
                               "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "&nbsp;" + "</th> " +
                               "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>" +
                               "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";

                    for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                    {
                        baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    }
                    baoCao += "</tr>";
                    foreach (var itemxx in itemx.DoiTuongBNList)
                    {
                        baoCao += "<tr>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "&nbsp;" + "</th> " +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemxx.TenDoiTuongBN + "</th>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                        for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                        {
                            baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                        }
                        baoCao += "</tr>";
                    }
                }
                // doanh thu nhà thuốc
                foreach (var itemx in item.DoanhThuNhaThuocList.GroupBy(s => s.TenDoanhThuNhaThuoc))
                {

                    baoCao += "<tr>" +
                        "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "2" + "</th> " +
                        "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemx.FirstOrDefault().TenDoanhThuNhaThuoc + "</th>" +
                        "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                    {
                        baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    }
                }

            }
            // tổng tiền mặt
            baoCao += "<tr>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + "II" + "</td>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: left;padding: 5px;text-align: center;'>" + "Tổng tiền mặt và thẻ thu được" + "</td>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "</td>" +
                       "<td " + "colspan='" + ((length * 2) + 2) + "'style='border: 1px solid #020000; border-collapse: collapse;text-align: left;padding: 5px;text-align: center;'>" + "</td>" +
                       "</tr>";
            int sttTongTienMat = 1;
            int ngay = 1;
            foreach (var itemLoaiPhieuThu in loaiPhieuThus)
            {
                List<NgayThangNamTongTien> cvd = new List<NgayThangNamTongTien>();
                NgayThangNamTongTien ngayThangNamTongTien = new NgayThangNamTongTien();
                ngayThangNamTongTien.NgayThangNam = itemLoaiPhieuThu.NgayThangNamList.LastOrDefault().NgayThangNam.AddDays(1);
                for (int itemx = 0; itemx < itemLoaiPhieuThu.NgayThangNamList.Count() + 1; itemx++)
                {
                    if (itemx < itemLoaiPhieuThu.NgayThangNamList.Count())
                    {
                        cvd.Add(itemLoaiPhieuThu.NgayThangNamList[itemx]);
                    }
                    if (((itemLoaiPhieuThu.NgayThangNamList.Count() + 1) - itemx) == 1)
                    {
                        ngayThangNamTongTien.NgayThangNam = itemLoaiPhieuThu.NgayThangNamList.LastOrDefault().NgayThangNam.AddDays(1);
                        cvd.Add(ngayThangNamTongTien);
                    }
                }

                baoCao += "<tr>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + sttTongTienMat + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: left;padding: 5px;text-align: center;'>" + itemLoaiPhieuThu.TenLoaiPhieuThu + "</td>";
                baoCao += "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "số dư đầu kỳ" + "</td>";
                if (itemLoaiPhieuThu.TongTienMatList.Where(s => s.MaTongTienMat == itemLoaiPhieuThu.MaTongTienMat).ToList().Count() < (length * 2))
                {
                    var tes = length - itemLoaiPhieuThu.TongTienMatList.Where(s => s.MaTongTienMat == itemLoaiPhieuThu.MaTongTienMat).Count();
                    for (int ix = 0; ix < tes; ix++)
                    {
                        TongTienMat tongTienMat = new TongTienMat();
                        tongTienMat.MaTongTienMat = itemLoaiPhieuThu.MaTongTienMat;
                        tongTienMatObjMGsss.NgayThangNam = null;
                        tongTienMatObjMGsss.SoDuDauKy = null;
                        tongTienMatObjMGsss.SoNguoi = null;
                        tongTienMatObjMGsss.DoanhThu = null;
                        itemLoaiPhieuThu.TongTienMatList.Add(tongTienMat);
                    }

                }
                foreach (var itemNgay in itemLoaiPhieuThu.TongTienMatList.Where(s => s.MaTongTienMat == itemLoaiPhieuThu.MaTongTienMat).ToList())
                {
                    baoCao +=
                            "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + itemNgay.SoNguoi + "</td>" +
                            "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + itemNgay.DoanhThu + "</td>";

                }
                baoCao += "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</td>" +
                         "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</td>" +
                         "</tr>";
                sttTongTienMat++;
            }
            // cong no
            foreach (var itemCongNo in listCongNoPhaiThus)
            {
                baoCao +=
                       "<tr>" +
                       "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>III</th>" +
                       "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemCongNo.TenCongNo + "</th>" +
                       "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'> Số dư kỳ đầu</th>";

                if (itemCongNo.ListCongNo.Count() < length)
                {
                    var colLength = length - itemCongNo.ListCongNo.Count();
                    for (int ix = 0; ix < colLength; ix++)
                    {
                        CongNo congNo = new CongNo();
                        congNo.NgayThangNam = null;
                        congNo.NgayThangNam = null;
                        congNo.SoDuDauKy = null;
                        congNo.SoNguoi = null;
                        congNo.DoanhThu = null;
                        itemCongNo.ListCongNo.Add(congNo);
                    }
                }
                foreach (var colCongNo in itemCongNo.ListCongNo)
                {
                    baoCao += "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + colCongNo.SoNguoi + "</td>" +
                              "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + colCongNo.DoanhThu + "</td>";
                }
                baoCao += "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</td>" +
                             "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</td>" +
                             "</tr>";
            }
            return baoCao;
        }
        public virtual byte[] ExportBaoCaoKetQuaKhamChuaBenhAsync(DataTongHopBaoCaoKetQuaKhamChuaBenhVo dataBaoCao, BaoCaoKetQuaKhamChuaBenhVo query)
        {
            int indexSTT = 1;
            var ngayHienTai = DateTime.Now.Day;
            var thangHienTai = DateTime.Now.Month;
            var namHienTai = DateTime.Now.Year;
            var hostingName = query.Hosting;
            //var requestProperties = new[]
            //{
            //    new PropertyByName<BaoCaoDoanhThuNhaThuocGridVo>("STT", p => indexSTT++)
            //};

            #region data test
            dataBaoCao.Doc.Add(new DataDocBaoCaoKetQuaKhamChuaBenhVo()
            {
                KeyDoc = "1",
                NoiDung = "Kết quả KCB và DT thuốc",
                TT = "I"
            });
            dataBaoCao.Doc.Add(new DataDocBaoCaoKetQuaKhamChuaBenhVo()
            {
                KeyDoc = "1.1",
                NoiDung = "Khám sức khỏe đoàn: Khi thêm hợp đồng KSK chọn Loại hợp đồng = Khám sức khỏe công ty",
                TT = "1"
            });


            dataBaoCao.Data.Add(new DataBaoCaoKetQuaKhamChuaBenhVo()
            {
                KeyDoc = "1.1",
                KeyNgang = "20210514LK",
                Value = 50
            });
            dataBaoCao.Data.Add(new DataBaoCaoKetQuaKhamChuaBenhVo()
            {
                KeyDoc = "1.1",
                KeyNgang = "20210514DT",
                Value = 25151121
            });
            #endregion


            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO KẾT QUẢ KHÁM CHỮA BỆNH");

                    // set row
                    worksheet.Row(7).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 40;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.DefaultColWidth = 20;

                    //SET img 
                    //using (var range = worksheet.Cells["A1:C1"])
                    //{
                    //    var url = hostingName + "/assets/img/logo-bacha-full.png";
                    //    WebClient wc = new WebClient();
                    //    byte[] bytes = wc.DownloadData(url); // download file từ server
                    //    MemoryStream ms = new MemoryStream(bytes); //
                    //    Image img = Image.FromStream(ms); // chuyển đổi thành img
                    //    ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                    //    pic.SetPosition(0, 0, 0, 0);
                    //    var height = 120; // chiều cao từ A1 đến A6
                    //    var width = 510; // chiều rộng từ A1 đến D1
                    //    pic.SetSize(width, height);
                    //    range.Worksheet.Protection.IsProtected = false;
                    //    range.Worksheet.Protection.AllowSelectLockedCells = false;
                    //}

                    // đổi logo thành tên bệnh viện
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }



                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A7:P7"])
                    {
                        range.Worksheet.Cells["A7:P7"].Merge = true;
                        range.Worksheet.Cells["A7:P7"].Value = "BÁO CÁO KẾT QUẢ KHÁM CHỮA BỆNH TỪ NGÀY " + (query.TuNgay == null ? "" : query.TuNgay.Value.ToString("dd/MM/yyyy")) + " ĐẾN NGÀY " + (query.DenNgay == null ? "" : query.DenNgay.Value.ToString("dd/MM/yyyy"));
                        range.Worksheet.Cells["A7:P7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:P7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:P7"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A7:P7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:P7"].Style.Font.Bold = true;
                    }

                    //using (var range = worksheet.Cells["A9:P9"])
                    //{
                    //    range.Worksheet.Cells["A9:P9"].Merge = true;
                    //    range.Worksheet.Cells["A9:P9"].Value = "Thời gian từ: " + (query.TuNgay == null ? "" : query.TuNgay.Value.ToString("dd/MM/yyyy"))
                    //                                      + " - đến " + (query.DenNgay == null ? "" : query.DenNgay.Value.ToString("dd/MM/yyyy"));
                    //    range.Worksheet.Cells["A9:P9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells["A9:P9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    //    range.Worksheet.Cells["A9:P9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    //    range.Worksheet.Cells["A9:P9"].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells["A9:P9"].Style.Font.Italic = true;
                    //}

                    // tạo list chứa key của từng col cần bind data trong excel

                    #region khởi tạo list key column
                    var keyColTheoNgays = new List<ColumnExcelInfoVo>();
                    string[] arrColumnDefault = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                    var colLuyKe = new ColumnExcelInfoVo()
                    {
                        ColumnKey = "LuyKe",
                        ColumnName = "C"
                    };
                    keyColTheoNgays.Add(colLuyKe); //col lũy kế là mặc định

                    var coKetQua = new ColumnExcelInfoVo()
                    {
                        ColumnKey = "KetQua",
                        ColumnName = "D"
                    };
                    keyColTheoNgays.Add(coKetQua); // col kết quả là mặc định
                    #endregion

                    #region header table 

                    string[] arrKeyDocCanInDam = { "1", "2", "3" };
                    var colLuotKham = "LK";
                    var colDoanhThu = "DT";
                    using (var range = worksheet.Cells["A10:P11"])
                    {
                        range.Worksheet.Cells["A10:D11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A10:D11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A10:D11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:D11"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A10:D11"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A10:D11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A10:A11"].Merge = true;
                        range.Worksheet.Cells["A10:A11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A10:A11"].Value = "TT";
                        range.Worksheet.Cells["A10:A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A10:A11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        range.Worksheet.Cells["B10:B11"].Merge = true;
                        range.Worksheet.Cells["B10:B11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B10:B11"].Value = "NỘI DUNG";
                        range.Worksheet.Cells["B10:B11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B10:B11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        range.Worksheet.Cells["C10:C11"].Merge = true;
                        range.Worksheet.Cells["C10:C11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C10:C11"].Value = "Số dư lũy kế từ đầu tháng đến hiện tại";
                        range.Worksheet.Cells["C10:C11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C10:C11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;


                        range.Worksheet.Cells["D10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D10"].Value = "Ngày";
                        range.Worksheet.Cells["D10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D10"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        range.Worksheet.Cells["D11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D11"].Value = "Kết quả";
                        range.Worksheet.Cells["D11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;


                        // xử lý tạo colimn động theo ngày nhập vào
                        int lanLapTenCot = 0;
                        for (DateTime ngayKetQua = query.TuNgay.Value; ngayKetQua <= query.DenNgay.Value; ngayKetQua = ngayKetQua.AddDays(1))
                        {
                            var column1 = string.Empty;
                            var column2 = string.Empty;
                            for (int j = 0; j < 2; j++)
                            {
                                string keyByDate = ngayKetQua.ToString("yyyyMMdd");
                                string columnNameNew = "";

                                if (lanLapTenCot > 0)
                                {
                                    //columnNameNew = "A";
                                    for (int k = 0; k < lanLapTenCot; k++)
                                    {
                                        columnNameNew += "A";
                                    }
                                }

                                var lastColumnName = keyColTheoNgays.Last().ColumnName;
                                if (lastColumnName.EndsWith('Z'))
                                {
                                    columnNameNew += (lanLapTenCot == 0 ? "AA" : (columnNameNew + "A"));
                                    lanLapTenCot++;
                                }
                                else
                                {
                                    if (lanLapTenCot > 0)
                                    {
                                        lastColumnName = lastColumnName.Substring(lastColumnName.Length - 1, 1);
                                    }
                                    var indexLastColumnName = arrColumnDefault.IndexOf(lastColumnName);
                                    var columnNameNext = arrColumnDefault[indexLastColumnName + 1];
                                    columnNameNew += columnNameNext;
                                }

                                var newColumn = new ColumnExcelInfoVo()
                                {
                                    ColumnKey = keyByDate + (j == 0 ? colLuotKham : colDoanhThu),
                                    ColumnName = columnNameNew
                                };
                                keyColTheoNgays.Add(newColumn);

                                if (j == 0)
                                {
                                    column1 = columnNameNew;
                                }
                                else
                                {
                                    column2 = columnNameNew;
                                }
                            }

                            // xử lý add cột mới vào file excel
                            var thu = ngayKetQua.DayOfWeek;
                            var ngay = ngayKetQua.ToString("dd/MM/yyyy");
                            range.Worksheet.Cells[column1 + "10:" + column2 + "10"].Merge = true;
                            range.Worksheet.Cells[column1 + "10:" + column2 + "10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[column1 + "10:" + column2 + "10"].Value = string.Format("Thứ {0} Ngày {1}", ChuyenTenThuTiengVietTrongTuan(thu), ngay);
                            range.Worksheet.Cells[column1 + "10:" + column2 + "10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[column1 + "10:" + column2 + "10"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells[column1 + "10:" + column2 + "10"].Style.Font.Bold = true;


                            range.Worksheet.Cells[column1 + "11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[column1 + "11"].Value = "Lượt khám";
                            range.Worksheet.Cells[column1 + "11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[column1 + "11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                            range.Worksheet.Cells[column2 + "11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[column2 + "11"].Value = "Doanh thu";
                            range.Worksheet.Cells[column2 + "11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[column2 + "11"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        }

                    }
                    #endregion

                    var manager = new PropertyManager<DataDocBaoCaoKetQuaKhamChuaBenhVo>(new List<PropertyByName<DataDocBaoCaoKetQuaKhamChuaBenhVo>>());
                    int index = 12; // bắt đầu đổ data từ dòng 12

                    ///////Đổ data vào bảng excel
                    var numberFormat = "#,##0.00";

                    #region đổ data
                    var lastColumnNameInRange = keyColTheoNgays.Last().ColumnName;
                    foreach (var data in dataBaoCao.Doc)
                    {
                        manager.CurrentObject = data;
                        //// format border, font chữ,....
                        worksheet.Cells["A" + index + ":" + lastColumnNameInRange + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        worksheet.Cells["A" + index + ":" + lastColumnNameInRange + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + index + ":" + lastColumnNameInRange + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["A" + index + ":" + lastColumnNameInRange + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":" + lastColumnNameInRange + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Row(index).Height = 20.5;
                        manager.WriteToXlsx(worksheet, index);


                        #region xử lý kiểm tra dòng cần in đậm
                        var canInDam = arrKeyDocCanInDam.Any(x => x == data.KeyDoc);
                        if (canInDam)
                        {
                            worksheet.Cells["A" + index].Style.Font.Bold = true;
                            worksheet.Cells["B" + index].Style.Font.Bold = true;
                        }
                        #endregion

                        // Đổ data
                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["A" + index].Value = data.TT;
                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Value = data.NoiDung;
                        foreach (var col in keyColTheoNgays)
                        {
                            worksheet.Cells[col.ColumnName + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            var dataNgang = dataBaoCao.Data.FirstOrDefault(x => x.KeyDoc == data.KeyDoc && x.KeyNgang == col.ColumnKey);
                            if (dataNgang != null)
                            {
                                if (col.ColumnKey.EndsWith(colDoanhThu))
                                {
                                    worksheet.Cells[col.ColumnName + index].Style.Numberformat.Format = numberFormat;
                                }
                                worksheet.Cells[col.ColumnName + index].Value = dataNgang.Value;
                            }
                        }
                        index++;
                    }
                    #endregion

                    #region Ghi chú
                    // xử lý giá trị
                    var ngayDauTienTrongThang = new DateTime(query.TuNgay.Value.Year, query.TuNgay.Value.Month, 1);
                    var ngayDauTienTrongNam = new DateTime(query.TuNgay.Value.Year, 1, 1);

                    #region row Ghi chú
                    using (var range = worksheet.Cells["A" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index].Value = "Ghi chú:";
                        range.Worksheet.Cells["B" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["B" + index].Style.Font.UnderLine = true;
                        index++;
                    }
                    #endregion
                    #region Doanh thu
                    using (var range = worksheet.Cells["A" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index + ":F" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":F" + index].Value =
                            string.Format("- Doanh thu ngày {0} đến ngày {1}: {2} đồng", ngayDauTienTrongThang.ApplyFormatDate(), query.TuNgay?.ApplyFormatDate(), dataBaoCao.DoanhThuTrongThang, dataBaoCao.DoanhThuTrongThang.ApplyNumber());
                        index++;
                    }
                    #endregion
                    #region Doanh thu lũy kế
                    using (var range = worksheet.Cells["A" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index + ":F" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":F" + index].Value =
                            string.Format("- Doanh thu lũy kế từ ngày {0} đến ngày {1}: {2} đồng", ngayDauTienTrongNam.ApplyFormatDate(), query.TuNgay?.ApplyFormatDate(), dataBaoCao.DoanhThuLuyKeTrongNam, dataBaoCao.DoanhThuLuyKeTrongNam.ApplyNumber());
                        index++;
                    }
                    #endregion
                    #region Số lượt tiếp nhận thực tế
                    using (var range = worksheet.Cells["A" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":F" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index + ":F" + index].Merge = true;
                        range.Worksheet.Cells["B" + index + ":F" + index].Value =
                            string.Format("- Số lượt TN thực tế đến khám chữa bệnh từ ngày {0} đến ngày {1} (bao gồm cả người bệnh do Bác sĩ hợp tác giới thiệu): {2} người", query.TuNgay?.ApplyFormatDate(), query.DenNgay?.ApplyFormatDate(), dataBaoCao.SoLuotTiepNhan);
                        index++;
                    }
                    #endregion
                    #endregion

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        private string ChuyenTenThuTiengVietTrongTuan(DayOfWeek thuTiengAnh)
        {
            var thuTiengViet = string.Empty;
            switch (thuTiengAnh)
            {
                case DayOfWeek.Monday:
                    thuTiengViet = "Hai"; break;
                case DayOfWeek.Tuesday:
                    thuTiengViet = "Ba"; break;
                case DayOfWeek.Wednesday:
                    thuTiengViet = "Tư"; break;
                case DayOfWeek.Thursday:
                    thuTiengViet = "Năm"; break;
                case DayOfWeek.Friday:
                    thuTiengViet = "Sáu"; break;
                case DayOfWeek.Saturday:
                    thuTiengViet = "Bảy"; break;
                case DayOfWeek.Sunday:
                    thuTiengViet = "Chủ Nhật"; break;
            }
            return thuTiengViet;
        }

        #region In 
        public async Task<string> InBaoCaoKetQuaKhamChuaBenh(BaoCaoKetQuaKhamChuaBenhVo baoCaoKetQuaKhamChuaBenhVo)
        {
            // data test
            List<KetQuaKCBVaDoanhThuNhaThuoc> listKetQuaKCBVaDoanhThuNhaThuoc = new List<KetQuaKCBVaDoanhThuNhaThuoc>();

            // 1.  kết quả khám chữa bệnh
            KetQuaKCBVaDoanhThuNhaThuoc ketQuaKCBVaDoanhThuNhaThuoc = new KetQuaKCBVaDoanhThuNhaThuoc();
            ketQuaKCBVaDoanhThuNhaThuoc.Ten = "Kết quả khám chữa bệnh";
            //KetQuaKCBVaDoanhThuNhaThuoc.KetQuaKCBList = new List<KetQuaKCB>();
            List<KetQuaKCB> ketQuaKCBs = new List<KetQuaKCB>();
            KetQuaKCB ketQuaKCB = new KetQuaKCB();
            ketQuaKCB.MaHinhThucDen = "Ma1";
            ketQuaKCB.TenHinhThucDen = "- Theo hình thức đến";
            // create loại hình thức
            LoaiHinhThuc loaiHinhThuc = new LoaiHinhThuc();
            loaiHinhThuc.TenLoaiHinhThuc = "+ Tự đến : tự đến";
            loaiHinhThuc.MaLoaiHinhThuc = "MATD";
            ketQuaKCB.LoaiHinhThucList.Add(loaiHinhThuc);

            LoaiHinhThuc loaiHinhThucs = new LoaiHinhThuc();
            loaiHinhThucs.TenLoaiHinhThuc = "+ Giới thiệu : BS Hòa";
            loaiHinhThucs.MaLoaiHinhThuc = "BSHoa";
            ketQuaKCB.LoaiHinhThucList.Add(loaiHinhThucs);

            LoaiHinhThuc loaiHinhThucss = new LoaiHinhThuc();
            loaiHinhThucss.TenLoaiHinhThuc = "+ Giới thiệu : BS An";
            loaiHinhThucss.MaLoaiHinhThuc = "BSAn";
            ketQuaKCB.LoaiHinhThucList.Add(loaiHinhThucss);

            LoaiHinhThuc loaiHinhThucsss = new LoaiHinhThuc();
            loaiHinhThucsss.TenLoaiHinhThuc = "+ Hình thức khác : Hình thức khác";
            loaiHinhThucsss.MaLoaiHinhThuc = "HTK";
            ketQuaKCB.LoaiHinhThucList.Add(loaiHinhThucsss);
            // crate đối tượng người bệnh
            List<DoiTuongBN> listDoiTuongBNs = new List<DoiTuongBN>();
            DoiTuongBN doiTuongBN = new DoiTuongBN();

            doiTuongBN.MaDoiTuongBN = " + Người bệnh nhi:";
            doiTuongBN.MaDoiTuongBN = "BNN";
            ketQuaKCB.DoiTuongBNList.Add(doiTuongBN);

            doiTuongBN.MaDoiTuongBN = " + Răng hàm mặt:";
            doiTuongBN.MaDoiTuongBN = "RHM";
            ketQuaKCB.DoiTuongBNList.Add(doiTuongBN);

            doiTuongBN.MaDoiTuongBN = " + Thai sản:";
            doiTuongBN.MaDoiTuongBN = "TS";
            ketQuaKCB.DoiTuongBNList.Add(doiTuongBN);

            // tạo  kết quả khám chữa bệnh
            ketQuaKCBs.Add(ketQuaKCB);
            ketQuaKCBVaDoanhThuNhaThuoc.KetQuaKCBList.Add(ketQuaKCB);
            listKetQuaKCBVaDoanhThuNhaThuoc.Add(ketQuaKCBVaDoanhThuNhaThuoc);

            // data doanh thu nhà thuốc
            KetQuaKCBVaDoanhThuNhaThuoc ketQuaKCBVaDoanhThuNhaThuocss = new KetQuaKCBVaDoanhThuNhaThuoc();
            ketQuaKCBVaDoanhThuNhaThuocss.Ten = "Doanh thu thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.SoDuDauKy = 100000;
            //ketQuaKCBVaDoanhThuNhaThuoc.DoanhThuNhaThuocList
            DoanhThuNhaThuoc doanhThuNhaThuoc = new DoanhThuNhaThuoc();

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 1);
            doanhThuNhaThuoc.SoNguoi = 100;
            doanhThuNhaThuoc.DoanhThu = 100000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 2);
            doanhThuNhaThuoc.SoNguoi = 101;
            doanhThuNhaThuoc.DoanhThu = 110000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 3);
            doanhThuNhaThuoc.SoNguoi = 103;
            doanhThuNhaThuoc.DoanhThu = 130000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 4);
            doanhThuNhaThuoc.SoNguoi = 104;
            doanhThuNhaThuoc.DoanhThu = 140000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 5);
            doanhThuNhaThuoc.SoNguoi = 105;
            doanhThuNhaThuoc.DoanhThu = 150000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            doanhThuNhaThuoc.NgayThangNam = new DateTime(2020, 2, 6);
            doanhThuNhaThuoc.SoNguoi = 106;
            doanhThuNhaThuoc.DoanhThu = 160000;
            doanhThuNhaThuoc.TenDoanhThuNhaThuoc = "Doanh thu nhà thuốc";
            ketQuaKCBVaDoanhThuNhaThuocss.DoanhThuNhaThuocList.Add(doanhThuNhaThuoc);

            listKetQuaKCBVaDoanhThuNhaThuoc.Add(ketQuaKCBVaDoanhThuNhaThuocss);
            // tổng tiền mặt
            var ngayVaoVien = baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().ApplyFormatDate();
            var ngayRaVien = baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().ApplyFormatDate();
            DateTime start = new DateTime(baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Year, baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Month, baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Day);
            DateTime end = new DateTime(baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Year, baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Month, baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Day);
            TimeSpan difference = end - start; //create TimeSpan object
            var tmp = difference.TotalDays;
            List<NgayThangNamTongTien> listNgayInItems = new List<NgayThangNamTongTien>();

            for (var i = 0; i < tmp; i++)
            {
                NgayThangNamTongTien listNgayIn = new NgayThangNamTongTien();
                listNgayIn.NgayThangNam = baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().AddDays(i);
                listNgayInItems.Add(listNgayIn);
            }
            List<TongTienMat> tongTienMats = new List<TongTienMat>();
            // mã Tm
            TongTienMat tongTienMatObj = new TongTienMat();
            tongTienMatObj.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObj.MaTongTienMat = "MaTM";
            tongTienMatObj.SoDuDauKy = 100;
            tongTienMatObj.SoNguoi = 10;
            tongTienMatObj.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObj);

            TongTienMat tongTienMatObjs = new TongTienMat();
            tongTienMatObjs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjs.MaTongTienMat = "MaTM";
            tongTienMatObjs.SoDuDauKy = 100;
            tongTienMatObjs.SoNguoi = 10;
            tongTienMatObjs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjs);

            TongTienMat tongTienMatObjss = new TongTienMat();
            tongTienMatObjss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjss.MaTongTienMat = "MaTM";
            tongTienMatObjss.SoDuDauKy = 100;
            tongTienMatObjss.SoNguoi = 10;
            tongTienMatObjss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjss);

            TongTienMat tongTienMatObjsss = new TongTienMat();
            tongTienMatObjsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjsss.MaTongTienMat = "MaTM";
            tongTienMatObjsss.SoDuDauKy = 100;
            tongTienMatObjsss.SoNguoi = 10;
            tongTienMatObjsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjsss);

            TongTienMat tongTienMatObjssss = new TongTienMat();
            tongTienMatObjssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjssss.MaTongTienMat = "MaTM";
            tongTienMatObjssss.SoDuDauKy = 100;
            tongTienMatObjssss.SoNguoi = 10;
            tongTienMatObjssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjssss);

            TongTienMat tongTienMatObjsssss = new TongTienMat();
            tongTienMatObjsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjsssss.MaTongTienMat = "MaTM";
            tongTienMatObjsssss.SoDuDauKy = 100;
            tongTienMatObjsssss.SoNguoi = 10;
            tongTienMatObjsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjsssss);

            // CK
            TongTienMat tongTienMatObjCK = new TongTienMat();
            tongTienMatObjCK.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObjCK.MaTongTienMat = "MaCK";
            tongTienMatObjCK.SoDuDauKy = 100;
            tongTienMatObjCK.SoNguoi = 10;
            tongTienMatObjCK.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCK);

            TongTienMat tongTienMatObjCKs = new TongTienMat();
            tongTienMatObjCKs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjCKs.MaTongTienMat = "MaCK";
            tongTienMatObjCKs.SoDuDauKy = 100;
            tongTienMatObjCKs.SoNguoi = 10;
            tongTienMatObjCKs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKs);

            TongTienMat tongTienMatObjCKss = new TongTienMat();
            tongTienMatObjCKss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjCKss.MaTongTienMat = "MaCK";
            tongTienMatObjCKss.SoDuDauKy = 100;
            tongTienMatObjCKss.SoNguoi = 10;
            tongTienMatObjCKss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKss);

            TongTienMat tongTienMatObjCKsss = new TongTienMat();
            tongTienMatObjCKsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjCKsss.MaTongTienMat = "MaCK";
            tongTienMatObjCKsss.SoDuDauKy = 100;
            tongTienMatObjCKsss.SoNguoi = 10;
            tongTienMatObjCKsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKsss);

            TongTienMat tongTienMatObjCKssss = new TongTienMat();
            tongTienMatObjCKssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjCKssss.MaTongTienMat = "MaCK";
            tongTienMatObjCKssss.SoDuDauKy = 100;
            tongTienMatObjCKssss.SoNguoi = 10;
            tongTienMatObjCKssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKssss);

            TongTienMat tongTienMatObjCKsssss = new TongTienMat();
            tongTienMatObjCKsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjCKsssss.MaTongTienMat = "MaCK";
            tongTienMatObjCKsssss.SoDuDauKy = 100;
            tongTienMatObjCKsssss.SoNguoi = 10;
            tongTienMatObjCKsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCKsssss);

            // pos
            TongTienMat tongTienMatObjP = new TongTienMat();
            tongTienMatObjP.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObjP.MaTongTienMat = "MaQP";
            tongTienMatObjP.SoDuDauKy = 100;
            tongTienMatObjP.SoNguoi = 10;
            tongTienMatObjP.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjP);

            TongTienMat tongTienMatObjPs = new TongTienMat();
            tongTienMatObjPs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjPs.MaTongTienMat = "MaQP";
            tongTienMatObjPs.SoDuDauKy = 100;
            tongTienMatObjPs.SoNguoi = 10;
            tongTienMatObjPs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPs);

            TongTienMat tongTienMatObjPss = new TongTienMat();
            tongTienMatObjPss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjPss.MaTongTienMat = "MaQP";
            tongTienMatObjPss.SoDuDauKy = 100;
            tongTienMatObjPss.SoNguoi = 10;
            tongTienMatObjPss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPss);

            TongTienMat tongTienMatObjPsss = new TongTienMat();
            tongTienMatObjPsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjPsss.MaTongTienMat = "MaQP";
            tongTienMatObjPsss.SoDuDauKy = 100;
            tongTienMatObjPsss.SoNguoi = 10;
            tongTienMatObjPsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPsss);

            TongTienMat tongTienMatObjPssss = new TongTienMat();
            tongTienMatObjPssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjPssss.MaTongTienMat = "MaQP";
            tongTienMatObjPssss.SoDuDauKy = 100;
            tongTienMatObjPssss.SoNguoi = 10;
            tongTienMatObjPssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPssss);

            TongTienMat tongTienMatObjPsssss = new TongTienMat();
            tongTienMatObjPsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjPsssss.MaTongTienMat = "MaQP";
            tongTienMatObjPsssss.SoDuDauKy = 100;
            tongTienMatObjPsssss.SoNguoi = 10;
            tongTienMatObjPsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjPsssss);

            // MACT
            TongTienMat tongTienMatObjCT = new TongTienMat();
            tongTienMatObjCT.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObjCT.MaTongTienMat = "MACT";
            tongTienMatObjCT.SoDuDauKy = 100;
            tongTienMatObjCT.SoNguoi = 10;
            tongTienMatObjCT.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCT);

            TongTienMat tongTienMatObjCTs = new TongTienMat();
            tongTienMatObjCTs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjCTs.MaTongTienMat = "MACT";
            tongTienMatObjCTs.SoDuDauKy = 100;
            tongTienMatObjCTs.SoNguoi = 10;
            tongTienMatObjCTs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTs);

            TongTienMat tongTienMatObjCTss = new TongTienMat();
            tongTienMatObjCTss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjCTss.MaTongTienMat = "MACT";
            tongTienMatObjCTss.SoDuDauKy = 100;
            tongTienMatObjCTss.SoNguoi = 10;
            tongTienMatObjCTss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTss);

            TongTienMat tongTienMatObjCTsss = new TongTienMat();
            tongTienMatObjCTsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjCTsss.MaTongTienMat = "MACT";
            tongTienMatObjCTsss.SoDuDauKy = 100;
            tongTienMatObjCTsss.SoNguoi = 10;
            tongTienMatObjCTsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTsss);

            TongTienMat tongTienMatObjCTssss = new TongTienMat();
            tongTienMatObjCTssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjCTssss.MaTongTienMat = "MACT";
            tongTienMatObjCTssss.SoDuDauKy = 100;
            tongTienMatObjCTssss.SoNguoi = 10;
            tongTienMatObjCTssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTssss);

            TongTienMat tongTienMatObjCTsssss = new TongTienMat();
            tongTienMatObjCTsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjCTsssss.MaTongTienMat = "MACT";
            tongTienMatObjCTsssss.SoDuDauKy = 100;
            tongTienMatObjCTsssss.SoNguoi = 10;
            tongTienMatObjCTsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjCTsssss);

            //MG
            TongTienMat tongTienMatObjMG = new TongTienMat();
            tongTienMatObjMG.NgayThangNam = new DateTime(2020, 3, 1);
            tongTienMatObjMG.MaTongTienMat = "MaMG";
            tongTienMatObjMG.SoDuDauKy = 100;
            tongTienMatObjMG.SoNguoi = 10;
            tongTienMatObjMG.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMG);

            TongTienMat tongTienMatObjMGs = new TongTienMat();
            tongTienMatObjMGs.NgayThangNam = new DateTime(2020, 3, 2);
            tongTienMatObjMGs.MaTongTienMat = "MaMG";
            tongTienMatObjMGs.SoDuDauKy = 100;
            tongTienMatObjMGs.SoNguoi = 10;
            tongTienMatObjMGs.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGs);

            TongTienMat tongTienMatObjMGss = new TongTienMat();
            tongTienMatObjMGss.NgayThangNam = new DateTime(2020, 3, 3);
            tongTienMatObjMGss.MaTongTienMat = "MaMG";
            tongTienMatObjMGss.SoDuDauKy = 100;
            tongTienMatObjMGss.SoNguoi = 10;
            tongTienMatObjMGss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGss);

            TongTienMat tongTienMatObjMGsss = new TongTienMat();
            tongTienMatObjMGsss.NgayThangNam = new DateTime(2020, 3, 4);
            tongTienMatObjMGsss.MaTongTienMat = "MaMG";
            tongTienMatObjMGsss.SoDuDauKy = 100;
            tongTienMatObjMGsss.SoNguoi = 10;
            tongTienMatObjMGsss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGsss);

            TongTienMat tongTienMatObjMGssss = new TongTienMat();
            tongTienMatObjMGssss.NgayThangNam = new DateTime(2020, 3, 5);
            tongTienMatObjMGssss.MaTongTienMat = "MaMG";
            tongTienMatObjMGssss.SoDuDauKy = 100;
            tongTienMatObjMGssss.SoNguoi = 10;
            tongTienMatObjMGssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGssss);

            TongTienMat tongTienMatObjMGsssss = new TongTienMat();
            tongTienMatObjMGsssss.NgayThangNam = new DateTime(2020, 3, 6);
            tongTienMatObjMGsssss.MaTongTienMat = "MaMG";
            tongTienMatObjMGsssss.SoDuDauKy = 100;
            tongTienMatObjMGsssss.SoNguoi = 10;
            tongTienMatObjMGsssss.DoanhThu = 100;
            tongTienMats.Add(tongTienMatObjMGsssss);


            List<LoaiPhieuThu> loaiPhieuThus = new List<LoaiPhieuThu>();

            LoaiPhieuThu loaiPhieuThuObj = new LoaiPhieuThu();

            loaiPhieuThuObj.TenLoaiPhieuThu = "Tiền mặt";
            loaiPhieuThuObj.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObj.MaTongTienMat = "MaTM";
            loaiPhieuThuObj.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObj);

            LoaiPhieuThu loaiPhieuThuObjs = new LoaiPhieuThu();

            loaiPhieuThuObjs.TenLoaiPhieuThu = "Thẻ: Chuyển khoản";
            loaiPhieuThuObjs.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObjs.MaTongTienMat = "MaCK";
            loaiPhieuThuObjs.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObjs);

            LoaiPhieuThu loaiPhieuThuObjss = new LoaiPhieuThu();

            loaiPhieuThuObjss.TenLoaiPhieuThu = "Pos:Quẹt Pos";
            loaiPhieuThuObjss.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObjss.MaTongTienMat = "MaQP";
            loaiPhieuThuObjss.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObjss);

            LoaiPhieuThu loaiPhieuThuObjsss = new LoaiPhieuThu();

            loaiPhieuThuObjsss.TenLoaiPhieuThu = "BHYT: PhanBHYTChiTra";
            loaiPhieuThuObjsss.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObjsss.MaTongTienMat = "MACT";
            loaiPhieuThuObjsss.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObjsss);

            LoaiPhieuThu loaiPhieuThuObjssss = new LoaiPhieuThu();

            loaiPhieuThuObjssss.TenLoaiPhieuThu = "Miễn giảm";
            loaiPhieuThuObjssss.NgayThangNamList = listNgayInItems;
            loaiPhieuThuObjssss.MaTongTienMat = "MaMG";
            loaiPhieuThuObjssss.TongTienMatList = tongTienMats;
            loaiPhieuThus.Add(loaiPhieuThuObjssss);

            //// công nợ -----------------------------
            List<CongNo> listCongNos = new List<CongNo>();
            CongNo congNoObj = new CongNo();
            congNoObj.SoDuDauKy = 100;
            congNoObj.SoNguoi = 100;
            congNoObj.DoanhThu = 100;
            congNoObj.NgayThangNam = new DateTime(2020, 3, 1);
            listCongNos.Add(congNoObj);

            CongNo congNoObjs = new CongNo();
            congNoObjs.SoDuDauKy = 100;
            congNoObjs.SoNguoi = 100;
            congNoObjs.DoanhThu = 100;
            congNoObjs.NgayThangNam = new DateTime(2020, 3, 2);
            listCongNos.Add(congNoObjs);

            CongNo congNoObjss = new CongNo();
            congNoObjss.SoDuDauKy = 100;
            congNoObjss.SoNguoi = 100;
            congNoObjss.DoanhThu = 100;
            congNoObjss.NgayThangNam = new DateTime(2020, 3, 3);
            listCongNos.Add(congNoObjss);

            CongNo congNoObjsss = new CongNo();
            congNoObjsss.SoDuDauKy = 100;
            congNoObjsss.SoNguoi = 100;
            congNoObjsss.DoanhThu = 100;
            congNoObjsss.NgayThangNam = new DateTime(2020, 3, 4);
            listCongNos.Add(congNoObjsss);

            CongNo congNoObjssss = new CongNo();
            congNoObjssss.SoDuDauKy = 100;
            congNoObjssss.SoNguoi = 100;
            congNoObjssss.DoanhThu = 100;
            congNoObjsss.NgayThangNam = new DateTime(2020, 3, 5);
            listCongNos.Add(congNoObjssss);


            CongNo congNoObjsssss = new CongNo();
            congNoObjsssss.SoDuDauKy = 100;
            congNoObjsssss.SoNguoi = 100;
            congNoObjsssss.DoanhThu = 100;
            congNoObjssss.NgayThangNam = new DateTime(2020, 3, 6);
            listCongNos.Add(congNoObjsssss);

            List<CongNoPhaiThu> listCongNoPhaiThus = new List<CongNoPhaiThu>();
            CongNoPhaiThu congNoPhaiThuObj = new CongNoPhaiThu();
            congNoPhaiThuObj.TenCongNo = "Công nợ còn phải thu của khách hàng";
            congNoPhaiThuObj.ListCongNo = listCongNos;
            listCongNoPhaiThus.Add(congNoPhaiThuObj);
            ///---------------------------------------

            ///
            var STT = 1;
            var baoCao = "";
            //var ngayVaoVien = baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().ApplyFormatDate();
            //var ngayRaVien = baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().ApplyFormatDate();
            //DateTime start = new DateTime(baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Year, baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Month, baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().Day);
            //DateTime end = new DateTime(baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Year, baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Month, baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().Day);
            //TimeSpan difference = end - start; //create TimeSpan object
            //var tmp = difference.TotalDays;
            int idSoPhieu = 1;
            List<ListDatePhieuBaoCao> listNgayInArr = new List<ListDatePhieuBaoCao>();
            List<DateTime> listNgayInItem = new List<DateTime>();
            for (var j = 0; j <= tmp; j++)
            {
                listNgayInItem.Add(baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().AddDays(j));
            }
            baoCao += "<table style='width:100%'>" +
                         "<tr>" +
                         "<th rowspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>STT</th>" +
                         "<th rowspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>Nội dung</th>" +
                         "<th rowspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>Số dư đầu kỳ(từ đến)</th>";

            foreach (var columsTitle in listNgayInItem)
            {
                baoCao += "<th  colspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + columsTitle + "</th>";
            }
            baoCao += "<th  colspan='2' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>Cộng</ th >" +
                      "</tr>" +
                      "<tr>";
            var length = listNgayInItem.Count();
            for (var columsTitle = 0; columsTitle <= length; columsTitle++)
            {
                baoCao += "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>Số người</th>" +
                          "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>Doanh thu</th>";
            }
            baoCao += "</tr>";
            // kết quả khám chữa người bệnh và daonh thu nhà thuốc
            baoCao += "<tbody>" +
                       "<tr>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + "I" + "</td>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: left;padding: 5px;text-align: center;'>" + "Kết quả KCB và doanh thu nhà thuốc" + "</td>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "</td>" +
                       "<td " + "colspan='" + ((length * 2) + 2) + "'style='border: 1px solid #020000; border-collapse: collapse;text-align: left;padding: 5px;text-align: center;'>" + "</td>" +
                       "</tr>";
            //+ "</tbody>";

            int stt = 1;
            foreach (var item in listKetQuaKCBVaDoanhThuNhaThuoc)
            {
                foreach (var itemx in item.KetQuaKCBList)
                {
                    baoCao += "<tr>" +
                             "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + 1 + "</th> " +
                             "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemx.TenHinhThucDen + "</th>" +
                             "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                    {
                        baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    }
                    baoCao += "</tr>";
                    foreach (var itemxx in itemx.LoaiHinhThucList)
                    {
                        baoCao += "<tr>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "&nbsp;" + "</th> " +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemxx.TenLoaiHinhThuc + "</th>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                        for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                        {
                            baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                        }
                    }
                    baoCao += "</tr>";
                    baoCao += "<tr>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "&nbsp;" + "</th> " +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    baoCao += "</tr>";
                    baoCao += "<tr>" +
                               "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "&nbsp;" + "</th> " +
                               "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>" +
                               "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";

                    for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                    {
                        baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    }
                    baoCao += "</tr>";
                    foreach (var itemxx in itemx.DoiTuongBNList)
                    {
                        baoCao += "<tr>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "&nbsp;" + "</th> " +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemxx.TenDoiTuongBN + "</th>" +
                            "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                        for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                        {
                            baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                        }
                        baoCao += "</tr>";
                    }
                }
                // doanh thu nhà thuốc
                foreach (var itemx in item.DoanhThuNhaThuocList.GroupBy(s => s.TenDoanhThuNhaThuoc))
                {

                    baoCao += "<tr>" +
                        "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: right;' >" + "2" + "</th> " +
                        "<th style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemx.FirstOrDefault().TenDoanhThuNhaThuoc + "</th>" +
                        "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    for (var columsTitle = 0; columsTitle < ((length * 2) + 2); columsTitle++)
                    {
                        baoCao += "<th style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</th>";
                    }
                }

            }
            // tổng tiền mặt
            baoCao += "<tr>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + "II" + "</td>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: left;padding: 5px;text-align: center;'>" + "Tổng tiền mặt và thẻ thu được" + "</td>" +
                       "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "</td>" +
                       "<td " + "colspan='" + ((length * 2) + 2) + "'style='border: 1px solid #020000; border-collapse: collapse;text-align: left;padding: 5px;text-align: center;'>" + "</td>" +
                       "</tr>";
            int sttTongTienMat = 1;
            int ngay = 1;
            foreach (var itemLoaiPhieuThu in loaiPhieuThus)
            {
                List<NgayThangNamTongTien> cvd = new List<NgayThangNamTongTien>();
                NgayThangNamTongTien ngayThangNamTongTien = new NgayThangNamTongTien();
                ngayThangNamTongTien.NgayThangNam = itemLoaiPhieuThu.NgayThangNamList.LastOrDefault().NgayThangNam.AddDays(1);
                for (int itemx = 0; itemx < itemLoaiPhieuThu.NgayThangNamList.Count() + 1; itemx++)
                {
                    if (itemx < itemLoaiPhieuThu.NgayThangNamList.Count())
                    {
                        cvd.Add(itemLoaiPhieuThu.NgayThangNamList[itemx]);
                    }
                    if (((itemLoaiPhieuThu.NgayThangNamList.Count() + 1) - itemx) == 1)
                    {
                        ngayThangNamTongTien.NgayThangNam = itemLoaiPhieuThu.NgayThangNamList.LastOrDefault().NgayThangNam.AddDays(1);
                        cvd.Add(ngayThangNamTongTien);
                    }
                }

                baoCao += "<tr>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + sttTongTienMat + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: left;padding: 5px;text-align: center;'>" + itemLoaiPhieuThu.TenLoaiPhieuThu + "</td>";
                baoCao += "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "" + "</td>";
                if (itemLoaiPhieuThu.TongTienMatList.Where(s => s.MaTongTienMat == itemLoaiPhieuThu.MaTongTienMat).ToList().Count() < (length * 2))
                {
                    var tes = length - itemLoaiPhieuThu.TongTienMatList.Where(s => s.MaTongTienMat == itemLoaiPhieuThu.MaTongTienMat).Count();
                    for (int ix = 0; ix < tes; ix++)
                    {
                        TongTienMat tongTienMat = new TongTienMat();
                        tongTienMat.MaTongTienMat = itemLoaiPhieuThu.MaTongTienMat;
                        tongTienMatObjMGsss.NgayThangNam = null;
                        tongTienMatObjMGsss.SoDuDauKy = null;
                        tongTienMatObjMGsss.SoNguoi = null;
                        tongTienMatObjMGsss.DoanhThu = null;
                        itemLoaiPhieuThu.TongTienMatList.Add(tongTienMat);
                    }

                }
                foreach (var itemNgay in itemLoaiPhieuThu.TongTienMatList.Where(s => s.MaTongTienMat == itemLoaiPhieuThu.MaTongTienMat).ToList())
                {
                    baoCao +=
                            "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + itemNgay.SoNguoi + "</td>" +
                            "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + itemNgay.DoanhThu + "</td>";

                }
                baoCao += "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</td>" +
                         "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</td>" +
                         "</tr>";
                sttTongTienMat++;
            }
            // cong no
            foreach (var itemCongNo in listCongNoPhaiThus)
            {
                baoCao +=
                       "<tr>" +
                       "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>III</th>" +
                       "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 12%;text-align: left;padding: 5px;text-align: center;'>" + itemCongNo.TenCongNo + "</th>" +
                       "<th  style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'></th>";

                if (itemCongNo.ListCongNo.Count() < length)
                {
                    var colLength = length - itemCongNo.ListCongNo.Count();
                    for (int ix = 0; ix < colLength; ix++)
                    {
                        CongNo congNo = new CongNo();
                        congNo.NgayThangNam = null;
                        congNo.NgayThangNam = null;
                        congNo.SoDuDauKy = null;
                        congNo.SoNguoi = null;
                        congNo.DoanhThu = null;
                        itemCongNo.ListCongNo.Add(congNo);
                    }
                }
                foreach (var colCongNo in itemCongNo.ListCongNo)
                {
                    baoCao += "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + colCongNo.SoNguoi + "</td>" +
                              "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + colCongNo.DoanhThu + "</td>";
                }
                baoCao += "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</td>" +
                             "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + "&nbsp;" + "</td>" +
                             "</tr>";
            }
            baoCao += "</tbody>" + "</table>";
            var data = new
            {
                LogoUrl = baoCaoKetQuaKhamChuaBenhVo.Hosting + "/assets/img/logo-bacha-full.png",
                ThoiGianChonBaoCao = "Từ ngày " + baoCaoKetQuaKhamChuaBenhVo.TuNgay.GetValueOrDefault().ApplyFormatDateTimeSACH() + " đến ngày " + baoCaoKetQuaKhamChuaBenhVo.DenNgay.GetValueOrDefault().ApplyFormatDateTimeSACH(),
                BaoCaoKetQuaKhamChuaBenh = baoCao,

            };
            var content = "";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("BaoCaoKetQuaKhamChuaBenh"));
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        #endregion
        #endregion
        #region báo cáo viện phí thu tiền
        public async Task<GridDataSource> GetBaoCaoThuTienVienPhiForGridAsync(BaoCaoVienPhiThuTienVo queryInfo)
        {

            List<BaoCaoVienPhiThuTienGridVo> noiTruModel = new List<BaoCaoVienPhiThuTienGridVo>();
            for (int i = 1; i < 3; i++)
            {
                var data = new BaoCaoVienPhiThuTienGridVo()
                {
                    Id = i,
                    STT = i,
                    Ngay = new DateTime(2021, 3, i),
                    SoBLHD = "ML0000" + i,
                    MaYTe = "MLML0000" + i,
                    TenBenhNhan = i == 0 ? "Nguyễn văn A" : i == 1 ? "Nguyễn văn B" : i == 2 ? "Nguyễn văn C" : "Nguyễn văn C",
                    SoBenhAn = "Số Bệnh Án" + i,
                    TamUng = 1000000 + i,
                    HoanUng = 1000000 + i,
                    ThuTien = 1000000 + i,
                    Hoan = 1000000 + i,
                    GoiDichVu = 1000000 + i,
                    CongNo = 1000000 + i,
                    Pos = 1000000 + i,
                    ChuyenKhoan = 1000000 + i,
                    Tienmat = 1000000 + i,
                    SoPhieuThu = "1000001" + i,
                    PosCapNhat = 1000001 + i,
                    ChuyenKhoanCapNhat = 1000001 + i,
                    TienmatCapNhat = 1000001 + i,
                };
                noiTruModel.Add(data);
            }
            return new GridDataSource { Data = noiTruModel.ToArray(), TotalRowCount = noiTruModel.Count() };

        }
        public async Task<GridDataSource> GetBaoCaoThuTienVienPhiChildForGridAsync(BaoCaoVienPhiThuTienVo queryInfo)
        {

            List<BaoCaoVienPhiThuTienGridVo> noiTruModel = new List<BaoCaoVienPhiThuTienGridVo>();
            for (int i = 1; i < 16; i++)
            {
                var data = new BaoCaoVienPhiThuTienGridVo()
                {
                    Id = i < 5 ? 1 : i < 10 ? 2 : i < 15 ? 3 : 3,
                    STT = i,
                    Ngay = new DateTime(2021, 3, i),
                    SoBLHD = "ML0000" + i,
                    MaYTe = "MLML0000" + i,
                    TenBenhNhan = i < 5 ? "Nguyễn văn A" : i < 10 ? "Nguyễn văn B" : i < 15 ? "Nguyễn văn C" : "Nguyễn văn C",
                    SoBenhAn = "Số Bệnh Án" + i,
                    TamUng = 1000000 + i,
                    HoanUng = 1000000 + i,
                    ThuTien = 1000000 + i,
                    Hoan = 1000000 + i,
                    GoiDichVu = 1000000 + i,
                    CongNo = 1000000 + i,
                    Pos = 1000000 + i,
                    ChuyenKhoan = 1000000 + i,
                    Tienmat = 1000000 + i,
                    SoPhieuThu = "1000001" + i,
                    PosCapNhat = 1000001 + i,
                    ChuyenKhoanCapNhat = 1000001 + i,
                    TienmatCapNhat = 1000001 + i,
                };
                noiTruModel.Add(data);
            }
            if (queryInfo.NhanVienThuNganId != 0)
            {
                noiTruModel.Where(s => s.Id == queryInfo.NhanVienThuNganId).ToList();
            }
            return new GridDataSource { Data = noiTruModel.ToArray(), TotalRowCount = noiTruModel.Count() };
        }
        private List<BaoCaoVienPhiThuTienGridVo> GetDataInBaoCaoThuTienVienPhiForGridAsync(BaoCaoVienPhiThuTienVo queryInfo)
        {

            List<BaoCaoVienPhiThuTienGridVo> noiTruModel = new List<BaoCaoVienPhiThuTienGridVo>();
            for (int i = 1; i < 3; i++)
            {
                var data = new BaoCaoVienPhiThuTienGridVo()
                {
                    Id = i,
                    STT = i,
                    Ngay = new DateTime(2021, 3, i),
                    SoBLHD = "ML0000" + i,
                    MaYTe = "MLML0000" + i,
                    TenBenhNhan = i == 0 ? "Nguyễn văn A" : i == 1 ? "Nguyễn văn B" : i == 2 ? "Nguyễn văn C" : "Nguyễn văn C",
                    SoBenhAn = "Số Bệnh Án" + i,
                    TamUng = 1000000 + i,
                    HoanUng = 1000000 + i,
                    ThuTien = 1000000 + i,
                    Hoan = 1000000 + i,
                    GoiDichVu = 1000000 + i,
                    CongNo = 1000000 + i,
                    Pos = 1000000 + i,
                    ChuyenKhoan = 1000000 + i,
                    Tienmat = 1000000 + i,
                    SoPhieuThu = "1000001" + i,
                    PosCapNhat = 1000001 + i,
                    ChuyenKhoanCapNhat = 1000001 + i,
                    TienmatCapNhat = 1000001 + i,
                };
                noiTruModel.Add(data);
            }
            return noiTruModel;

        }
        private List<BaoCaoVienPhiThuTienGridVo> GetDataInBaoCaoThuTienVienPhiChildForGridAsync(BaoCaoVienPhiThuTienVo queryInfo)
        {

            List<BaoCaoVienPhiThuTienGridVo> noiTruModel = new List<BaoCaoVienPhiThuTienGridVo>();
            for (int i = 1; i < 16; i++)
            {
                var data = new BaoCaoVienPhiThuTienGridVo()
                {
                    Id = i < 5 ? 1 : i < 10 ? 2 : i < 15 ? 3 : 3,
                    STT = i,
                    Ngay = new DateTime(2021, 3, i),
                    SoBLHD = "ML0000" + i,
                    MaYTe = "MLML0000" + i,
                    TenBenhNhan = i < 5 ? "Nguyễn văn A" : i < 10 ? "Nguyễn văn B" : i < 15 ? "Nguyễn văn C" : "Nguyễn văn C",
                    SoBenhAn = "Số Bệnh Án" + i,
                    TamUng = 1000000 + i,
                    HoanUng = 1000000 + i,
                    ThuTien = 1000000 + i,
                    Hoan = 1000000 + i,
                    GoiDichVu = 1000000 + i,
                    CongNo = 1000000 + i,
                    Pos = 1000000 + i,
                    ChuyenKhoan = 1000000 + i,
                    Tienmat = 1000000 + i,
                    SoPhieuThu = "1000001" + i,
                    PosCapNhat = 1000001 + i,
                    ChuyenKhoanCapNhat = 1000001 + i,
                    TienmatCapNhat = 1000001 + i,
                };
                noiTruModel.Add(data);
            }
            if (queryInfo.NhanVienThuNganId != 0)
            {
                noiTruModel.Where(s => s.Id == queryInfo.NhanVienThuNganId).ToList();
            }
            return noiTruModel;
        }
        #region in
        public async Task<string> InBaoCaoVienPhiThuTien(BaoCaoVienPhiThuTienVo baoCaoVienPhiThuTienVo)
        {
            var contents = "";
            var content = "";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("BaoCaoVienPhiThuTien"));
            var listDataCha = GetDataInBaoCaoThuTienVienPhiForGridAsync(baoCaoVienPhiThuTienVo);
            if (listDataCha.Count() > 0)
            {
                foreach (var item in listDataCha)
                {
                    content += "<tr>" +
                           "<td colspan='6' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: left;font-weight: bold;'>" + item.TenBenhNhan + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.TamUng + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.HoanUng + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.ThuTien + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.Hoan + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.GoiDichVu + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.CongNo + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.Pos + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.ChuyenKhoan + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.Tienmat + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.TienmatCapNhat + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.ChuyenKhoanCapNhat + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.PosCapNhat + "</td>" +
                           "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + item.SoPhieuThu + "</td>" +
                           "</tr>";
                    baoCaoVienPhiThuTienVo.NhanVienThuNganId = item.Id;
                    var listChild = GetDataInBaoCaoThuTienVienPhiChildForGridAsync(baoCaoVienPhiThuTienVo);
                    foreach (var itemx in listChild.ToList())
                    {
                        content += "<tr>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.STT + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.Ngay + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.SoBLHD + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.MaYTe + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.TenBenhNhan + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.SoBenhAn + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.TamUng + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.HoanUng + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.ThuTien + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.Hoan + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.GoiDichVu + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.CongNo + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.Pos + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.ChuyenKhoan + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.Tienmat + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.TienmatCapNhat + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.ChuyenKhoanCapNhat + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.PosCapNhat + "</td>" +
                               "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;'>" + item.SoPhieuThu + "</td>" +
                               "</tr>";
                    }
                }

            }
            content += "<tr>" +
                          "<td colspan='6' style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: left;font-weight: bold;'>" + "Tổng" + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "<td style='border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: left;padding: 5px;text-align: center;font-weight: bold;'>" + 0 + "</td>" +
                          "</tr>";
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            string nhanVienChon = _nhanVienRepository.TableNoTracking.Where(s => s.Id == baoCaoVienPhiThuTienVo.NhanVienThuNganId).Select(s => s.User.HoTen).FirstOrDefault(); ;
            var data = new
            {
                LogoUrl = baoCaoVienPhiThuTienVo.hosting + "/assets/img/logo-bacha-full.png",
                ThoiGianChonBaoCao = baoCaoVienPhiThuTienVo.TuNgay.ApplyFormatDateTimeSACH() + " - " + baoCaoVienPhiThuTienVo.DenNgay.ApplyFormatDateTimeSACH(),
                HienThiNhanVien = baoCaoVienPhiThuTienVo.NhanVienThuNganId == 0 ? "Tất cả nhân viên" : nhanVienChon,
                BaoCaoVienPhiThuTien = content,
                NgayHienTai = DateTime.Now.Day,
                ThangHienTai = DateTime.Now.Month,
                NamHienTai = DateTime.Now.Year,
                KeToan = "",
                NguoiLap = nguoiLogin
            };
            contents = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return contents;
        }
        #endregion
        #endregion
        #region báo cáo thống kê đơn thuốc 4/03/2021
        public async Task<GridDataSource> GetBaoCaoThongKeDonThuocForGridAsync(BaoCaoThongKeDonThuocVo queryInfo)
        {

            List<BaoCaoThongKeDonThuocGridVo> noiTruModel = new List<BaoCaoThongKeDonThuocGridVo>();
            for (int i = 1; i < 20; i++)
            {
                var data = new BaoCaoThongKeDonThuocGridVo()
                {
                    Id = i,
                    STT = i,
                    MaYT = "2001456" + i,
                    SoBenhAn = "Số Bệnh Án" + i,
                    HoVaTen = "Nguyễn văn " + i,
                    NamSinh = 1990 + i,
                    GioiTinh = i < 10 ? "Nam" : "Nữ",
                    DiaChi = "Số 3/139/1 Nguyễn Văn Cừ Tổ 11 Ngọc Lâm, Phường Ngọc Lâm, Quận Long Biên, Thành phố Hà Nội" + i,
                    MaBHYT = "GD401012303973" + i,
                    MaDKBD = "108" + i,
                    MaBenh = "Chuyển dạ lần 2, thai 37 tuần 4 ngày lần " + i,
                    NgayVao = new DateTime(2021, 3, i),
                    NgayRa = new DateTime(2021, 3, i),
                    ChanDoan = "Chuyển dạ lần 2, thai 37 tuần 4 ngày" + i,
                    TienSuBenh = "Khỏe mạnh lân " + i,
                    KhoaRa = "Khoa Phụ Sản lần " + i,
                    TrangThai = "Ra Viện lần " + i,
                    BsKeToa = "Bs. Trần Minh Hải lần " + i,
                    TenThuoc = "Ringer lactat & glucose 5% 500ml 5% ( Việt Nam) lần " + i,
                    HamLuong = i + "%",
                    SoLuong = i,
                    Sang = i,
                    Trua = i,
                    Chieu = i,
                    Toi = i,
                    Tra = i - 1,
                    GhiChu = "ghi chú " + i,
                    KhoPhat = "Kho Thuốc Bệnh Viện ( New ) lan " + i,
                    BHYT = "X",
                    NgayDuyetPhieu = new DateTime(2021, 3, i),
                };
                noiTruModel.Add(data);
            }
            return new GridDataSource { Data = noiTruModel.ToArray(), TotalRowCount = noiTruModel.Count() };

        }
        public virtual byte[] ExportBaoCaoThongKeDonThuoc(ICollection<BaoCaoThongKeDonThuocGridVo> data, DateTime? tuNgay, DateTime? denNgay, string hosting)
        {
            var queryInfo = new BaoCaoThongKeDonThuocGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoThongKeDonThuocGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO THỐNG KÊ ĐƠN THUỐC");

                    // set row


                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 20;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 20;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 20;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;
                    worksheet.Column(23).Width = 20;
                    worksheet.Column(24).Width = 30;
                    worksheet.Column(25).Width = 30;
                    worksheet.Column(26).Width = 30;
                    worksheet.Column(27).Width = 30;
                    worksheet.Column(28).Width = 30;

                    worksheet.DefaultColWidth = 28;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AC", "AD" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 4 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        var url = hosting + "/assets/img/logo-bacha-full.png";
                        WebClient wc = new WebClient();
                        byte[] bytes = wc.DownloadData(url); // download file từ server
                        MemoryStream ms = new MemoryStream(bytes); //
                        Image img = Image.FromStream(ms); // chuyển đổi thành img
                        ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                        pic.SetPosition(0, 0, 0, 0);
                        var height = 80; // chiều cao từ A1 đến A6
                        var width = 400; // chiều rộng từ A1 đến D1
                        pic.SetSize(width, height);
                        range.Worksheet.Protection.IsProtected = false;
                        range.Worksheet.Protection.AllowSelectLockedCells = false;
                    }

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "BÁO CÁO THỐNG KÊ ĐƠN THUỐC".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }
                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "STT" }, { "B", "Mã Y Tế" }, { "C", "Số Bệnh Án" } , { "D", "Họ Và Tên" },
                                    { "E", "Năm Sinh" }, { "F", "Giới Tính" },{ "G", "Địa Chỉ" },{ "H", "Mã BHYT" },{ "I", "Mã ĐKBD" },
                                    { "J", "Mã Bệnh" },{ "K", "Ngày Vào" },{ "L", "NgàyRra" },{ "M", "Chẩn Đoán" },{ "N", "Tiền Sử Bệnh" },
                                    { "O", "Khoa Ra" },{ "P", "Trạng Thái" },{ "Q", "BS Kê Toa" },{ "R", "Tên Thuốc" },{ "S", "Hàm Lượng" },
                                    { "T", "Số Lượng" },{ "U", "Sáng" },{ "V", "Trưa" },{ "W", "Chiều" },{ "X", "Tối" },
                                    { "Y", "Trả" },{ "Z", "Ghi Chú" },{ "AA", "Kho Phát" },{ "AB", "BHYT" },{ "AC", "Ngày Duyệt Phiếu" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoThongKeDonThuocGridVo>(requestProperties);
                    int index = 7;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int stt = 1;

                    foreach (var baoCao in data)
                    {
                        stt++;
                        manager.CurrentObject = baoCao;
                        manager.WriteToXlsx(worksheet, index);
                        worksheet.Cells["A" + index].Value = stt;
                        worksheet.Cells["B" + index].Value = baoCao.MaYT;
                        worksheet.Cells["C" + index].Value = baoCao.SoBenhAn;
                        worksheet.Cells["D" + index].Value = baoCao.HoVaTen;
                        worksheet.Cells["E" + index].Value = baoCao.NamSinh;
                        worksheet.Cells["F" + index].Value = baoCao.GioiTinh;
                        worksheet.Cells["G" + index].Value = baoCao.GioiTinh;
                        worksheet.Cells["H" + index].Value = baoCao.DiaChi;
                        worksheet.Cells["I" + index].Value = baoCao.MaBHYT;
                        worksheet.Cells["J" + index].Value = baoCao.MaDKBD;
                        worksheet.Cells["K" + index].Value = baoCao.MaBenh;
                        worksheet.Cells["L" + index].Value = baoCao.NgayVaoString;
                        worksheet.Cells["M" + index].Value = baoCao.NgayRaString;
                        worksheet.Cells["N" + index].Value = baoCao.ChanDoan;
                        worksheet.Cells["O" + index].Value = baoCao.TienSuBenh;
                        worksheet.Cells["P" + index].Value = baoCao.KhoaRa;

                        worksheet.Cells["Q" + index].Value = baoCao.TrangThai;
                        worksheet.Cells["R" + index].Value = baoCao.BsKeToa;
                        worksheet.Cells["S" + index].Value = baoCao.TenThuoc;
                        worksheet.Cells["T" + index].Value = baoCao.HamLuong;
                        worksheet.Cells["U" + index].Value = baoCao.SoLuong;
                        worksheet.Cells["V" + index].Value = baoCao.Sang;
                        worksheet.Cells["W" + index].Value = baoCao.Trua;
                        worksheet.Cells["X" + index].Value = baoCao.Chieu;
                        worksheet.Cells["Y" + index].Value = baoCao.Toi;
                        worksheet.Cells["Z" + index].Value = baoCao.Tra;
                        worksheet.Cells["AA" + index].Value = baoCao.GhiChu;
                        worksheet.Cells["AB" + index].Value = baoCao.KhoPhat;
                        worksheet.Cells["AC" + index].Value = baoCao.BHYT;
                        worksheet.Cells["AD" + index].Value = baoCao.NgayDuyetPhieuString;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }
                        index++;
                        var indexMain = index;
                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        #endregion
        public async Task<TotalBaoCaoThuPhiVienPhiGridVo> GetTotalBaoCaoChiTietThuTienVienPhi(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            var allData = await GetAllForBaoCaoThuPhiVienPhi(queryInfo);

            return new TotalBaoCaoThuPhiVienPhiGridVo
            {
                TamUng = allData.Select(o => o.TamUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HoanUng = allData.Select(o => o.HoanUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                SoTienThu = allData.Select(o => o.SoTienThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HuyThu = allData.Select(o => o.HuyThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                CongNo = allData.Select(o => o.CongNo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TienMat = allData.Select(o => o.TienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ChuyenKhoan = allData.Select(o => o.ChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                Pos = allData.Select(o => o.Pos.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoTienMat = allData.Select(o => o.ThuNoTienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoChuyenKhoan = allData.Select(o => o.ThuNoChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoPos = allData.Select(o => o.ThuNoPos.GetValueOrDefault()).DefaultIfEmpty(0).Sum()
            };
        }


        //#region báo cáo Lưu kết quả xe nghiệm người bệnh hàng ngày
        //public async Task<GridDataSource> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayForGridAsync(QueryInfo queryInfo, bool exportExcel)
        //{
        //    //BaoCaoLuuketQuaXeNghiemTrongNgayGridVo
        //    return new GridDataSource { Data = null, TotalRowCount = 0 };
        //}

        //public async Task<GridDataSource> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayTotalPageForGridAsync(QueryInfo queryInfo)
        //{
        //    //BaoCaoLuuketQuaXeNghiemTrongNgayGridVo
        //    return new GridDataSource { TotalRowCount = 0 };
        //}
        public virtual byte[] ExportBaoCaoLuuKetQuaXetNghiemHangNgay(ICollection<BaoCaoLuuketQuaXeNghiemTrongNgayGridVo> data, BaoCaoKetQuaXetNghiemQueryInfo baoCaoKetQuaXetNghiemQueryInfo)
        {
            var queryInfo = new BaoCaoLuuketQuaXeNghiemTrongNgayGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoLuuketQuaXeNghiemTrongNgayGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("LƯU KẾT QUẢ XÉT NGHIỆM HÀNG NGÀY");

                    // set row


                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 40;

                    worksheet.DefaultColWidth = 28;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                    var worksheetTitle = "A1:C1";
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 4 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 4;
                    var worksheetTitleHeader = SetColumnItems[0] + 5 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 5;

                    //using (var range = worksheet.Cells[worksheetTitle])
                    //{
                    //    range.Worksheet.Cells[worksheetTitle].Merge = true;
                    //    range.Worksheet.Cells[worksheetTitle].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                    //    range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    //    range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    //}
                    using (var range = worksheet.Cells["A2:C2"])
                    {
                        range.Worksheet.Cells["A2:C2"].Merge = true;
                        range.Worksheet.Cells["A2:C2"].Value = "KHOA:XÉT NGHIỆM";
                        range.Worksheet.Cells["A2:C2"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A2:C2"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["A3:K3"])
                    {
                        range.Worksheet.Cells["A3:K3"].Merge = true;
                        range.Worksheet.Cells["A3:K3"].Value = "LƯU KẾT QUẢ XÉT NGHIỆM HÀNG NGÀY".ToUpper();
                        range.Worksheet.Cells["A3:K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:K3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["A4:K4"])
                    {
                        range.Worksheet.Cells["A4:K4"].Merge = true;
                        range.Worksheet.Cells["A4:K4"].Value = "Từ giờ " + baoCaoKetQuaXetNghiemQueryInfo.TuNgay?.ApplyFormatTime() + " ngày " + baoCaoKetQuaXetNghiemQueryInfo.TuNgay?.ApplyFormatDate() + " đến giờ " + baoCaoKetQuaXetNghiemQueryInfo.DenNgay?.ApplyFormatTime() + " ngày " + baoCaoKetQuaXetNghiemQueryInfo.DenNgay?.ApplyFormatDate();
                        range.Worksheet.Cells["A4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:K4"].Style.Font.Italic = true;
                        range.Worksheet.Cells["A4:K4"].Style.Font.Bold = false;
                    }
                    //using (var range = worksheet.Cells[worksheetTitleStatus])
                    //{
                    //    range.Worksheet.Cells["A4:K4"].Merge = true;
                    //    range.Worksheet.Cells["A4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    range.Worksheet.Cells["A4:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //    range.Worksheet.Cells["A4:K4"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    //    range.Worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);
                    //    range.Worksheet.Cells["A4:K4"].Style.Font.Bold = true;
                    //}


                    using (var range = worksheet.Cells["K5:Z5"])
                    {
                        range.Worksheet.Cells["K5:Z5"].Merge = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "STT" }, { "B", "SID" }, { "C", "HỌ VÀ TÊN" } , { "D", "NĂM SINH" },
                                    { "E", "GIỚI TÍNH" }, { "F", "NƠI CHỈ ĐỊNH" },{ "G", "BHYT" },{ "H", "KSK" },{ "I", "BÁC SĨ" },
                                    { "J", "CHẨN ĐOÁN" },{ "K", "KẾT QUẢ" }};


                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 5 + ":" + (SetColumns[i, 0]).ToString() + 5).ToString();
                            //range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoLuuketQuaXeNghiemTrongNgayGridVo>(requestProperties);
                    int index = 6;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int stt = 1;

                    foreach (var baoCao in data)
                    {

                        manager.CurrentObject = baoCao;
                        manager.WriteToXlsx(worksheet, 1);
                        worksheet.Cells["A" + index].Value = stt;
                        worksheet.Cells["B" + index].Value = baoCao.Sid;
                        worksheet.Cells["C" + index].Value = baoCao.HoVaTen;
                        worksheet.Cells["D" + index].Value = baoCao.NamSinhDisplay;
                        worksheet.Cells["E" + index].Value = baoCao.LoaiGioiTinhDisplay;
                        worksheet.Cells["F" + index].Value = baoCao.NoiChiDinh;
                        worksheet.Cells["G" + index].Value = baoCao.BHYTDisplay;
                        worksheet.Cells["H" + index].Value = baoCao.KhamSucKhoeDisplay;
                        worksheet.Cells["I" + index].Value = baoCao.HoTenBacSi;
                        worksheet.Cells["J" + index].Value = baoCao.ChanDoan;
                        using (var range = worksheet.Cells["K" + index + ":" + "Z" + index])
                        {
                            range.Worksheet.Cells["K" + index + ":" + "Z" + index].Merge = true;
                        }
                        worksheet.Cells["K" + index].Value = baoCao.KetQua;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }
                        index++;
                        stt++;
                        var indexMain = index;
                        //for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        //{
                        //    worksheet.Row(inde).OutlineLevel = 1;
                        //}
                    }


                    // NGƯỜI LẬP BẢNG
                    using (var range = worksheet.Cells["K" + index + ":" + "Z" + index])
                    {
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Merge = true;
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Value = "Người lập bảng";
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.Font.Bold = true;
                        index++;
                    }

                    using (var range = worksheet.Cells["K" + index + ":" + "Z" + index])
                    {
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Merge = true;
                        index++;
                    }
                    using (var range = worksheet.Cells["K" + index + ":" + "Z" + index])
                    {
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Merge = true;
                        index++;
                    }
                    using (var range = worksheet.Cells["K" + index + ":" + "Z" + index])
                    {
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Merge = true;
                        index++;
                    }
                    long userId = _userAgentHelper.GetCurrentUserId();
                    string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
                    using (var range = worksheet.Cells["K" + index + ":" + "Z" + index])
                    {
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Merge = true;
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Value = nguoiLogin;
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["K" + index + ":" + "Z" + index].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }
                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        //#endregion
    }
}
