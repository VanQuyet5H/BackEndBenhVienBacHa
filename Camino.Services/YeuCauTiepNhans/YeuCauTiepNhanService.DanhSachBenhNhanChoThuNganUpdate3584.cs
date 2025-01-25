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
        public long HoanUng(long phieuTamUngId)
        {
            var phieuTamUng = _taiKhoanBenhNhanThu.GetById(phieuTamUngId);
            if (phieuTamUng.DaHuy == true)
            {
                throw new Exception("Phiếu tạm ứng đã hủy");
            }

            if (phieuTamUng.PhieuHoanUngId != null)
            {
                throw new Exception("Phiếu tạm ứng đã hoàn ứng");
            }
            var soTienDaTamUng = phieuTamUng.TienMat.GetValueOrDefault(0) + phieuTamUng.ChuyenKhoan.GetValueOrDefault(0) + phieuTamUng.POS.GetValueOrDefault(0);
            TaiKhoanBenhNhanChi phieuHoanUng = new TaiKhoanBenhNhanChi
            {
                TaiKhoanBenhNhanId = phieuTamUng.TaiKhoanBenhNhanId,
                LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.HoanUng,
                TienMat = soTienDaTamUng,
                NoiDungChi = Enums.LoaiChiTienBenhNhan.HoanUng.GetDescription(),
                NgayChi = DateTime.Now,
                YeuCauTiepNhanId = phieuTamUng.YeuCauTiepNhanId,
                SoPhieuHienThi = ResourceHelper.CreateSoHoanUng(),
                NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
            };
            phieuTamUng.PhieuHoanUng = phieuHoanUng;
            BaseRepository.Context.SaveChanges();
            return phieuHoanUng.Id;
        }
        //khách hàng muốn in  bảng kê nội trú chờ thu
        public string GetHtmlBangKeNgoaiTruChoThu(ThuPhiKhamChuaBenhVo thuPhiKhamChuaBenhVo)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(c => c.Id == thuPhiKhamChuaBenhVo.Id)
                 .Include(xx => xx.BenhNhan)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.BenhVienChuyenVien)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.Icdchinh)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhICDKhacs).ThenInclude(o => o.ICD)
                 .Include(cc => cc.PhuongXa)
                 .Include(cc => cc.QuanHuyen)
                 .Include(cc => cc.TinhThanh)
                 .Include(cc => cc.NoiChuyen).FirstOrDefault();

            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeNgoaiTruChoThu"));

            if (yeuCauTiepNhan == null)
                return string.Empty;

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.IcdchinhId != null).OrderBy(cc => cc.ThoiDiemHoanThanh).LastOrDefault();

            if (yeuCauKhamBenh != null)
            {
                var maKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.ICD.Ma).ToList();
                var chuGhiKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.GhiChu).ToList();

                maICDKemTheo = string.Join(", ", maKemTheos);
                tenICDKemTheo = string.Join(", ", chuGhiKemTheos);
            }

            if (yeuCauTiepNhan.CoBHYT == true)
            {
                if (!String.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(yeuCauTiepNhan.BHYTMaDKBD));
                }
                maBHYT = yeuCauTiepNhan.BHYTMaSoThe;
                bHYTTuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc?.ApplyFormatDate();
                bHYTDenNgay = yeuCauTiepNhan.BHYTNgayHetHan?.ApplyFormatDate();
                mucHuong = yeuCauTiepNhan.BHYTMucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = yeuCauTiepNhan.BHYTMaDKBD;
            }
            else
            {
                maBHYT = "";
                bHYTTuNgay = "";
                bHYTDenNgay = "";
                mucHuong = "0%";
                NoiDKKCBBanDau = "Chưa xác định";
                MaKCBBanDau = "Chưa xác định";
            }
            List<ChiPhiKhamChuaBenhVo> chiPhis = new List<ChiPhiKhamChuaBenhVo>();

            var tatCaChiPhis = GetTatCaDichVuKhamChuaBenh(thuPhiKhamChuaBenhVo.Id).Result.ToList();


            //if (chiPhis.Count == 0)
            //{
            //    return string.Empty;
            //}

            //tinh miễn giảm theo FE
            var dsChiPhiDaChon = thuPhiKhamChuaBenhVo.DanhSachChiPhiKhamChuaBenhDaChons;
            foreach (var chiPhiKhamChuaBenhVo in dsChiPhiDaChon)
            {
                //var chiphi = dsChiPhi.FirstOrDefault(o => o.LoaiNhom == chiPhiKhamChuaBenhVo.LoaiNhom && o.Id == chiPhiKhamChuaBenhVo.Id);
                //if (chiphi == null || !chiphi.ThanhTien.AlmostEqual(chiPhiKhamChuaBenhVo.ThanhTien) || !chiphi.BHYTThanhToan.AlmostEqual(chiPhiKhamChuaBenhVo.BHYTThanhToan) || chiPhiKhamChuaBenhVo.TongCongNo > chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan)
                //{
                //    return new KetQuaThuPhiKhamChuaBenhNoiTruVaQuyetToanDichVuTrongGoiVo { Error = "Thông tin dịch vụ thanh toán không hợp lệ, vui lòng tải lại trang" };
                //}
                var soTienTruocMienGiam = chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan;

                decimal soTienMienGiamTheoDv = 0;

                foreach (var mienGiamTheoTiLe in chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe))
                {
                    mienGiamTheoTiLe.SoTien = Math.Round((soTienTruocMienGiam * mienGiamTheoTiLe.TiLe.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                    soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
                }
                foreach (var mienGiamTheoTiLe in chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien))
                {
                    soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
                }
                //if (soTienMienGiamTheoDv > chiPhiKhamChuaBenhVo.ThanhTien)
                //{
                //    return new KetQuaThuPhiKhamChuaBenhNoiTruVaQuyetToanDichVuTrongGoiVo { Error = "Thông tin dịch vụ thanh toán không hợp lệ, vui lòng tải lại trang" };
                //}
                chiPhiKhamChuaBenhVo.SoTienMG = soTienMienGiamTheoDv;
            }
            foreach (var chiPhiKhamChuaBenhVo in tatCaChiPhis)
            {
                var chiPhiDaChon = dsChiPhiDaChon.FirstOrDefault(o => o.LoaiNhom == chiPhiKhamChuaBenhVo.LoaiNhom && o.Id == chiPhiKhamChuaBenhVo.Id);
                if (chiPhiDaChon != null)
                {
                    chiPhiKhamChuaBenhVo.SoTienMG = chiPhiDaChon.SoTienMG;
                    chiPhis.Add(chiPhiKhamChuaBenhVo);
                }
                else
                {

                }
            }

            var dsChiPhiBangKe = new List<BangKeKhamBenhBenhVienVo>();
            var dateItemChiPhis = string.Empty;
            int indexItem = 1;

            dateItemChiPhis += "<table style='width: 100 %; '>" +
                                "<tbody>" +
                                "<tr>" +
                                    $"<td style='padding-left:-5px; width: 40 %; '>Mã thẻ BHYT: <b>{maBHYT}</b> " +
                                    $"<span style='padding-left:20px'>Giá trị từ: <b>{bHYTTuNgay}</b> đến <b>{bHYTDenNgay}</b></span>" +
                                    $"<span style='padding-left:20px'>Mức hưởng: <b>{mucHuong}</b></span></td>" +
                                    "</tr>" +
                                "</tbody>" +
                                "</table>" +
                                "</br>" +
                                "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>" +
                                    "<tr style='border: 1px solid black;  border-collapse: collapse;'>" +
                                    "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;' rowspan='2'>" +
                                            "STT </th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:35%;text-align: center;' rowspan='2'>" +
                                            "Nội dung </th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:5%;text-align: center;' rowspan='2'>" +
                                            "Đơn vị tính</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:3%; text-align: center;' rowspan='2'>" +
                                            "Số lượng</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:3%; text-align: center;' rowspan='2'>" +
                                            "Đơn giá BV <span style='font-weight: 100;'>(đồng)</span></th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:3%; text-align: center;' rowspan='2'>" +
                                            "Đơn giá BH  <span style='font-weight: 100;'>(đồng)</span></th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:200px; text-align: center;'" +
                                            "rowspan='2'>" +
                                            "Tỉ lệ thanh toán theo dich vụ   <span style='font-weight: 100;'>(%)</span></th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:8%;text-align: center;' rowspan='2'>" +
                                            "Thành tiền BV  <span style='font-weight: 100;'>(đồng)</span>" +
                                        "</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:200px; text-align: center;'" +
                                            "rowspan='2'>" +
                                            "Tỷ lệ thanh toán BHYT <span style='font-weight: 100;'>(%)</span>" +
                                        "</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:8%;text-align: center;' rowspan='2'>" +
                                            "Thành tiền BH  <span style='font-weight: 100;'>(đồng)</span>" +
                                        "</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:35%;text-align: center;' colspan='4'>" +
                                            "Nguồn thanh toán  <span style='font-weight: 100;'>(đồng)</span></th>" +
                                    "</tr>" +
                                    "<tr style='border: 1px solid #020000; border-collapse: collapse;'>" +
                                        "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>Quỹ BHYT</b></td>" +
                                        "<td style=' border: 1px solid #020000; border-collapse: collapse;width:80px;text-align: center;'>" +
                                            "<b>Người bệnh cùng chi" +
                                            " trả</b> (đồng)</td>" +
                                        "<td style=' border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>Khác</b> (đồng)</td>" +
                                        "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;width: 90px;'>" +
                                            "<b>Người bệnh" +
                                            "tự trả </b>(đồng)" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b> </b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(1)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(2)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(3)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(4)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(5)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(6)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(7)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(8)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(9)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(10)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(11)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(12)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(13)</b> </td>" +
                                    "</tr>";


            foreach (var chiPhi in chiPhis)
            {
                var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                {
                    Nhom = chiPhi.NhomChiPhiBangKe,
                    Id = chiPhi.Id,
                    NoiDung = chiPhi.TenDichVu,
                    DonViTinh = chiPhi.DonViTinh,
                    SoLuong = (decimal)chiPhi.Soluong,
                    DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                    DonGiaBH = chiPhi.DuocHuongBHYT ? chiPhi.DonGiaBHYT : 0,
                    MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                    TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                    BaoHiemChiTra = true,
                    DonGiaBV = chiPhi.DonGia
                };
                if (chiPhi.KhongTinhPhi == true || chiPhi.YeuCauGoiDichVuId != null)
                {
                    chiphiBangKe.Khac = chiPhi.MucHuongBaoHiem > 0
                        ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                        : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                }
                else
                {
                    chiphiBangKe.Khac = chiPhi.SoTienMG;
                }
                dsChiPhiBangKe.Add(chiphiBangKe);
            }
            var groupChiPhiBangKes = dsChiPhiBangKe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
            foreach (var groupChiPhiBangKe in groupChiPhiBangKes)
            {
                if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay)
                {

                    dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;' colspan='7'>" +
                                        "<span style='float:left;'><b>2. Ngày giường:  </b></span>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "</tr>";
                    dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;' colspan='7'>" +
                                        "<span style='float:left;'><b>2.1." + groupChiPhiBangKe.Key.GetDescription() + "  </b></span>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b> " + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true) + "</b> </td>" +
                                        "</tr>";
                }
                else if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru)
                {
                    if (!groupChiPhiBangKes.Any(o => o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay))
                    {
                        dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                            "<td style='border: 1px solid #020000;' colspan='7'>" +
                                            "<span style='float:left;'><b>2. Ngày giường:  </b></span>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "</tr>";
                    }
                    dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;' colspan='7'>" +
                                        "<span style='float:left;'><b>2.2." + groupChiPhiBangKe.Key.GetDescription() + "  </b></span>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b> " + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true) + "</b> </td>" +
                                        "</tr>";
                }
                else
                {
                    dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;' colspan='7'>" +
                                        "<span style='float:left;'><b>" + (int)groupChiPhiBangKe.Key + "." + groupChiPhiBangKe.Key.GetDescription() + "  </b></span>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b> " + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true) + "</b> </td>" +
                                        "</tr>";
                }
                if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.GoiVatTu)
                {
                    var groupGoiVatTus = groupChiPhiBangKe.ToList().GroupBy(o => o.SoGoiVatTu.GetValueOrDefault()).OrderBy(o => o.Key);
                    int sttGoiVatTu = 1;
                    foreach (var groupGoiVatTu in groupGoiVatTus)
                    {
                        dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                            "<td style='border: 1px solid #020000;' colspan='7'>" +
                                            "<span style='float:left;'><b>10." + sttGoiVatTu + ". " + groupChiPhiBangKe.Key.GetDescription() + " " + sttGoiVatTu + "  </b></span>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.Khac)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b> " + Convert.ToDouble(groupGoiVatTu.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true) + "</b> </td>" +
                                            "</tr>";
                        foreach (var chiPhiBangKe in groupGoiVatTu)
                        {
                            dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                            "<td style='border: 1px solid #020000; text-align: center;'> " + indexItem + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: left;'> " + chiPhiBangKe.NoiDung + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: center;'>  " + chiPhiBangKe.DonViTinh + " </td>" +
                                            "<td style='border: 1px solid #020000;text-align: center;'> " + chiPhiBangKe.SoLuong + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: center;'> " + Convert.ToDouble(chiPhiBangKe.DonGiaBV).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: center;'> " + Convert.ToDouble(chiPhiBangKe.DonGiaBH).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.TiLeThanhToanTheoDV).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.ThanhTienBV).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.ThanhTienBH).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.QuyBHYT).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.NguoiBenhCungChiTra).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.Khac).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.NguoiBenhTuTra).ApplyFormatMoneyToDouble(true) + "</td>" +
                                            "</tr>";
                            indexItem++;
                        }
                    }
                }
                else
                {
                    foreach (var chiPhiBangKe in groupChiPhiBangKe)
                    {
                        dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000; text-align: center;'> " + indexItem + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: left;'> " + chiPhiBangKe.NoiDung + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'>  " + chiPhiBangKe.DonViTinh + " </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'> " + chiPhiBangKe.SoLuong + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'> " + Convert.ToDouble(chiPhiBangKe.DonGiaBV).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'> " + Convert.ToDouble(chiPhiBangKe.DonGiaBH).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.TiLeThanhToanTheoDV).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.ThanhTienBV).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.ThanhTienBH).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.QuyBHYT).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.NguoiBenhCungChiTra).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.Khac).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.NguoiBenhTuTra).ApplyFormatMoneyToDouble(true) + "</td>" +
                                        "</tr>";
                        indexItem++;
                    }
                }

            }

            dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                "<td style='border: 1px solid #020000;' colspan='7'></span>" +
                                "<span style='float:right;padding-right: 50px;'><b>Tổng Cộng</b></span>" +
                                "</td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b></b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true)}</b> </td>" +
                                "</tr>" +
                                "</table>";

            var checkedLyDoVaoVien = "<input id='demo_box_2' class='css - checkbox' type='checkbox' checked  disabled='true'/>";
            var unCheckedLyDoVaoVien = "<input id='demo_box_2' class='css - checkbox' type='checkbox' disabled='true' />";

            // tinh luon trong goi
            var tongQuyBHYTTrongGoi = tatCaChiPhis.Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && o.KiemTraBHYTXacNhan && o.YeuCauGoiDichVuId != null).Sum(o => o.BHYTThanhToan);
            var tongBHTN = Convert.ToDouble(tatCaChiPhis.Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan).Sum(o => o.TongCongNo));
            var tongQuyBHYTThanhToan = Convert.ToDouble(chiPhis.Sum(o => o.BHYTThanhToan) + tongQuyBHYTTrongGoi);
            var tongTamUng = GetSoTienDaTamUngAsync(thuPhiKhamChuaBenhVo.Id).Result;
            //Tổng chi phí - Tạm ứng - Quỹ BH - Nguồn khác
            var soTienCanThu = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)) - Convert.ToDouble(tongTamUng) - tongQuyBHYTThanhToan - tongBHTN - Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac));
            string tongBHTNTrongGoiText = $"</br> - BHTN bảo lãnh: {tongBHTN.ApplyFormatMoneyToDouble()}";
            string soTienNBTuTraHoacTraLaiBN = soTienCanThu < 0 ? $"- Phải trả lại NB: {(soTienCanThu * (-1)).ApplyFormatMoneyToDouble()}" : $"- Người bệnh tự trả: {(soTienCanThu).ApplyFormatMoneyToDouble()}";

            var data = new
            {
                TitleBangKe = "KHÁM BỆNH </br>CHỜ THU",
                SoBangKe = "1",
                yeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = "",
                yeuCauTiepNhan.HoTen,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                NamSinh = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh),
                GioiTinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "",
                TenKhoa = yeuCauKhamBenh?.NoiThucHien?.KhoaPhong?.Ten,
                MaKhoa = yeuCauKhamBenh?.NoiThucHien?.KhoaPhong?.Ma,
                MaBHYT = maBHYT,
                BHYTTuNgay = bHYTTuNgay,
                BHYTDenNgay = bHYTDenNgay,
                NoiDKKCBBanDau,
                MaKCBBanDau,
                NgayDenKham = yeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay(),
                DieuTriKNT = "",
                KetThucKhamNgoaiTru = yeuCauKhamBenh?.ThoiDiemHoanThanh?.ApplyFormatGioPhutNgay(),
                SoNgayDTri = "",
                TinhTrangRaVien = 1,
                MucHuong = mucHuong,
                NoiChuyenDen = yeuCauTiepNhan.NoiChuyen?.Ten,
                NoiChuyenDi = yeuCauKhamBenh?.BenhVienChuyenVien?.Ten,
                MKV = yeuCauTiepNhan.BHYTMaKhuVuc,
                NgayMiemCungTC = yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra?.ApplyFormatDate(),
                ThoiGian5Nam = yeuCauTiepNhan.BHYTNgayDu5Nam?.ApplyFormatDate(),
                CoCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,

                ChuanDoanXacDinh = yeuCauKhamBenh?.GhiChuICDChinh,
                MaICD10 = yeuCauKhamBenh?.Icdchinh?.Ma,
                BenhKemTheo = tenICDKemTheo,
                ICDKemTheo10 = maICDKemTheo,

                DateItemChiPhis = dateItemChiPhis,

                TTDV = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble(),
                TTTTBHYT = "",

                TTTienBH = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble(),
                TQBHYT = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble(),
                TNCCTra = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(),
                TongSoKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(),
                TNBTuTra = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true),

                TongChiPhi = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble(),
                SoTienBangChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV))),

                NguoiBenhPhaiTra = (Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)) + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra))).ApplyFormatMoneyToDouble(),

                TTQuyTT = tongQuyBHYTThanhToan.ApplyFormatMoneyToDouble(),
                //TTQuyTT = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble(),
                //BHYTChiTra = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(),
                //CacKhoanTraKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true),
                SoTienKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(),
                NgayHienTai = DateTime.Now.ApplyFormatNgayThangNam(),


                //ToDo
                TongBHTNTrongGoi = tongBHTNTrongGoiText,
                TongTamUng = Convert.ToDouble(tongTamUng).ApplyFormatMoneyToDouble(),
                SoTienNBTuTraHoacTraLaiBN = soTienNBTuTraHoacTraLaiBN,
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }
        public string GetHtmlBangKeNgoaiTruTrongGoiChoThu(QuyetToanDichVuTrongGoiVo thuPhiKhamChuaBenhVo)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(c => c.Id == thuPhiKhamChuaBenhVo.Id)
                 .Include(xx => xx.BenhNhan)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.BenhVienChuyenVien)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.Icdchinh)
                 .Include(xx => xx.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhICDKhacs).ThenInclude(o => o.ICD)
                 .Include(cc => cc.PhuongXa)
                 .Include(cc => cc.QuanHuyen)
                 .Include(cc => cc.TinhThanh)
                 .Include(cc => cc.NoiChuyen).FirstOrDefault();

            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeNgoaiTruChoThu"));

            if (yeuCauTiepNhan == null)
                return string.Empty;

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(cc => cc.IcdchinhId != null).OrderBy(cc => cc.ThoiDiemHoanThanh).LastOrDefault();

            if (yeuCauKhamBenh != null)
            {
                var maKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.ICD.Ma).ToList();
                var chuGhiKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => c.GhiChu).ToList();

                maICDKemTheo = string.Join(", ", maKemTheos);
                tenICDKemTheo = string.Join(", ", chuGhiKemTheos);
            }

            if (yeuCauTiepNhan.CoBHYT == true)
            {
                if (!String.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(yeuCauTiepNhan.BHYTMaDKBD));
                }
                maBHYT = yeuCauTiepNhan.BHYTMaSoThe;
                bHYTTuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc?.ApplyFormatDate();
                bHYTDenNgay = yeuCauTiepNhan.BHYTNgayHetHan?.ApplyFormatDate();
                mucHuong = yeuCauTiepNhan.BHYTMucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = yeuCauTiepNhan.BHYTMaDKBD;
            }
            else
            {
                maBHYT = "";
                bHYTTuNgay = "";
                bHYTDenNgay = "";
                mucHuong = "0%";
                NoiDKKCBBanDau = "Chưa xác định";
                MaKCBBanDau = "Chưa xác định";
            }
            List<ChiPhiKhamChuaBenhVo> chiPhis;
            //chỉ lấy những dịch vụ chưa thu tiền
            chiPhis = GetTatCaDichVuKhamChuaBenh(thuPhiKhamChuaBenhVo.Id).Result.Where(o => o.YeuCauGoiDichVuId != null && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan).ToList();
            
            var dsChiPhiBangKe = new List<BangKeKhamBenhBenhVienVo>();
            var dateItemChiPhis = string.Empty;
            int indexItem = 1;

            dateItemChiPhis += "<table style='width: 100 %; '>" +
                                "<tbody>" +
                                "<tr>" +
                                    $"<td style='padding-left:-5px; width: 40 %; '>Mã thẻ BHYT: <b>{maBHYT}</b> " +
                                    $"<span style='padding-left:20px'>Giá trị từ: <b>{bHYTTuNgay}</b> đến <b>{bHYTDenNgay}</b></span>" +
                                    $"<span style='padding-left:20px'>Mức hưởng: <b>{mucHuong}</b></span></td>" +
                                    "</tr>" +
                                "</tbody>" +
                                "</table>" +
                                "</br>" +
                                "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>" +
                                    "<tr style='border: 1px solid black;  border-collapse: collapse;'>" +
                                    "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;' rowspan='2'>" +
                                            "STT </th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:35%;text-align: center;' rowspan='2'>" +
                                            "Nội dung </th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:5%;text-align: center;' rowspan='2'>" +
                                            "Đơn vị tính</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:3%; text-align: center;' rowspan='2'>" +
                                            "Số lượng</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:3%; text-align: center;' rowspan='2'>" +
                                            "Đơn giá BV <span style='font-weight: 100;'>(đồng)</span></th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:3%; text-align: center;' rowspan='2'>" +
                                            "Đơn giá BH  <span style='font-weight: 100;'>(đồng)</span></th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:200px; text-align: center;'" +
                                            "rowspan='2'>" +
                                            "Tỉ lệ thanh toán theo dich vụ   <span style='font-weight: 100;'>(%)</span></th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:8%;text-align: center;' rowspan='2'>" +
                                            "Thành tiền BV  <span style='font-weight: 100;'>(đồng)</span>" +
                                        "</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:200px; text-align: center;'" +
                                            "rowspan='2'>" +
                                            "Tỷ lệ thanh toán BHYT <span style='font-weight: 100;'>(%)</span>" +
                                        "</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:8%;text-align: center;' rowspan='2'>" +
                                            "Thành tiền BH  <span style='font-weight: 100;'>(đồng)</span>" +
                                        "</th>" +
                                        "<th style='border: 1px solid #020000; border-collapse: collapse;width:35%;text-align: center;' colspan='4'>" +
                                            "Nguồn thanh toán  <span style='font-weight: 100;'>(đồng)</span></th>" +
                                    "</tr>" +
                                    "<tr style='border: 1px solid #020000; border-collapse: collapse;'>" +
                                        "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>Quỹ BHYT</b></td>" +
                                        "<td style=' border: 1px solid #020000; border-collapse: collapse;width:80px;text-align: center;'>" +
                                            "<b>Người bệnh cùng chi" +
                                            " trả</b> (đồng)</td>" +
                                        "<td style=' border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>Khác</b> (đồng)</td>" +
                                        "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;width: 90px;'>" +
                                            "<b>Người bệnh" +
                                            "tự trả </b>(đồng)" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b> </b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(1)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(2)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(3)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(4)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(5)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(6)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(7)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(8)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(9)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(10)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(11)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(12)</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'><b>(13)</b> </td>" +
                                    "</tr>";


            foreach (var chiPhi in chiPhis)
            {
                var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                {
                    Nhom = chiPhi.NhomChiPhiBangKe,
                    Id = chiPhi.Id,
                    NoiDung = chiPhi.TenDichVu,
                    DonViTinh = chiPhi.DonViTinh,
                    SoLuong = (decimal)chiPhi.Soluong,
                    DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                    DonGiaBH = chiPhi.DuocHuongBHYT ? chiPhi.DonGiaBHYT : 0,
                    MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                    TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                    BaoHiemChiTra = true,
                    DonGiaBV = chiPhi.DonGia
                };
                if (chiPhi.KhongTinhPhi == true || chiPhi.YeuCauGoiDichVuId != null)
                {
                    chiphiBangKe.Khac = chiPhi.MucHuongBaoHiem > 0
                        ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                        : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                }
                else
                {
                    chiphiBangKe.Khac = chiPhi.SoTienMG;
                }
                dsChiPhiBangKe.Add(chiphiBangKe);
            }
            var groupChiPhiBangKes = dsChiPhiBangKe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
            foreach (var groupChiPhiBangKe in groupChiPhiBangKes)
            {
                if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay)
                {

                    dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;' colspan='7'>" +
                                        "<span style='float:left;'><b>2. Ngày giường:  </b></span>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "</tr>";
                    dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;' colspan='7'>" +
                                        "<span style='float:left;'><b>2.1." + groupChiPhiBangKe.Key.GetDescription() + "  </b></span>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b> " + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true) + "</b> </td>" +
                                        "</tr>";
                }
                else if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru)
                {
                    if (!groupChiPhiBangKes.Any(o => o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay))
                    {
                        dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                            "<td style='border: 1px solid #020000;' colspan='7'>" +
                                            "<span style='float:left;'><b>2. Ngày giường:  </b></span>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "</tr>";
                    }
                    dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;' colspan='7'>" +
                                        "<span style='float:left;'><b>2.2." + groupChiPhiBangKe.Key.GetDescription() + "  </b></span>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b> " + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true) + "</b> </td>" +
                                        "</tr>";
                }
                else
                {
                    dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000;' colspan='7'>" +
                                        "<span style='float:left;'><b>" + (int)groupChiPhiBangKe.Key + "." + groupChiPhiBangKe.Key.GetDescription() + "  </b></span>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'><b> " + Convert.ToDouble(groupChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true) + "</b> </td>" +
                                        "</tr>";
                }
                if (groupChiPhiBangKe.Key == NhomChiPhiBangKe.GoiVatTu)
                {
                    var groupGoiVatTus = groupChiPhiBangKe.ToList().GroupBy(o => o.SoGoiVatTu.GetValueOrDefault()).OrderBy(o => o.Key);
                    int sttGoiVatTu = 1;
                    foreach (var groupGoiVatTu in groupGoiVatTus)
                    {
                        dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                            "<td style='border: 1px solid #020000;' colspan='7'>" +
                                            "<span style='float:left;'><b>10." + sttGoiVatTu + ". " + groupChiPhiBangKe.Key.GetDescription() + " " + sttGoiVatTu + "  </b></span>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b>" + Convert.ToDouble(groupGoiVatTu.Sum(o => o.Khac)).ApplyFormatMoneyToDouble() + "</b> </td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'><b> " + Convert.ToDouble(groupGoiVatTu.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true) + "</b> </td>" +
                                            "</tr>";
                        foreach (var chiPhiBangKe in groupGoiVatTu)
                        {
                            dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                            "<td style='border: 1px solid #020000; text-align: center;'> " + indexItem + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: left;'> " + chiPhiBangKe.NoiDung + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: center;'>  " + chiPhiBangKe.DonViTinh + " </td>" +
                                            "<td style='border: 1px solid #020000;text-align: center;'> " + chiPhiBangKe.SoLuong + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: center;'> " + Convert.ToDouble(chiPhiBangKe.DonGiaBV).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: center;'> " + Convert.ToDouble(chiPhiBangKe.DonGiaBH).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.TiLeThanhToanTheoDV).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.ThanhTienBV).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.ThanhTienBH).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.QuyBHYT).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.NguoiBenhCungChiTra).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.Khac).ApplyFormatMoneyToDouble() + "</td>" +
                                            "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.NguoiBenhTuTra).ApplyFormatMoneyToDouble(true) + "</td>" +
                                            "</tr>";
                            indexItem++;
                        }
                    }
                }
                else
                {
                    foreach (var chiPhiBangKe in groupChiPhiBangKe)
                    {
                        dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                        "<td style='border: 1px solid #020000; text-align: center;'> " + indexItem + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: left;'> " + chiPhiBangKe.NoiDung + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'>  " + chiPhiBangKe.DonViTinh + " </td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'> " + chiPhiBangKe.SoLuong + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'> " + Convert.ToDouble(chiPhiBangKe.DonGiaBV).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: center;'> " + Convert.ToDouble(chiPhiBangKe.DonGiaBH).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.TiLeThanhToanTheoDV).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.ThanhTienBV).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'> " + Convert.ToDouble(chiPhiBangKe.MucHuongBaoHiem > 0 ? 100 : 0).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.ThanhTienBH).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.QuyBHYT).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.NguoiBenhCungChiTra).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.Khac).ApplyFormatMoneyToDouble() + "</td>" +
                                        "<td style='border: 1px solid #020000;text-align: right;'>  " + Convert.ToDouble(chiPhiBangKe.NguoiBenhTuTra).ApplyFormatMoneyToDouble(true) + "</td>" +
                                        "</tr>";
                        indexItem++;
                    }
                }

            }

            dateItemChiPhis += "<tr style=' border: 1px solid #020000; '>" +
                                "<td style='border: 1px solid #020000;' colspan='7'></span>" +
                                "<span style='float:right;padding-right: 50px;'><b>Tổng Cộng</b></span>" +
                                "</td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b></b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true)}</b> </td>" +
                                "</tr>" +
                                "</table>";

            var checkedLyDoVaoVien = "<input id='demo_box_2' class='css - checkbox' type='checkbox' checked  disabled='true'/>";
            var unCheckedLyDoVaoVien = "<input id='demo_box_2' class='css - checkbox' type='checkbox' disabled='true' />";

            // tinh luon trong goi
            var tongQuyBHYTThanhToan = Convert.ToDouble(chiPhis.Sum(o => o.BHYTThanhToan));
            var tongTamUng = 0;//GetSoTienDaTamUngAsync(yeuCauTiepNhanId).Result;
            //Tổng chi phí - Tạm ứng - Quỹ BH - Nguồn khác
            var soTienCanThu = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)) - Convert.ToDouble(tongTamUng) - tongQuyBHYTThanhToan - Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac));
            string soTienNBTuTraHoacTraLaiBN = soTienCanThu < 0 ? $"- Phải trả lại NB: {(soTienCanThu * (-1)).ApplyFormatMoneyToDouble()}" : $"- Người bệnh tự trả: {(soTienCanThu).ApplyFormatMoneyToDouble()}";

            var data = new
            {
                TitleBangKe = "KHÁM BỆNH",
                SoBangKe = "1",
                yeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = "",
                yeuCauTiepNhan.HoTen,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                NamSinh = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh),
                GioiTinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "",
                TenKhoa = yeuCauKhamBenh?.NoiThucHien?.KhoaPhong?.Ten,
                MaKhoa = yeuCauKhamBenh?.NoiThucHien?.KhoaPhong?.Ma,
                MaBHYT = maBHYT,
                BHYTTuNgay = bHYTTuNgay,
                BHYTDenNgay = bHYTDenNgay,
                NoiDKKCBBanDau,
                MaKCBBanDau,
                NgayDenKham = yeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay(),
                DieuTriKNT = "",
                KetThucKhamNgoaiTru = yeuCauKhamBenh?.ThoiDiemHoanThanh?.ApplyFormatGioPhutNgay(),
                SoNgayDTri = "",
                TinhTrangRaVien = 1,
                MucHuong = mucHuong,
                NoiChuyenDen = yeuCauTiepNhan.NoiChuyen?.Ten,
                NoiChuyenDi = yeuCauKhamBenh?.BenhVienChuyenVien?.Ten,
                MKV = yeuCauTiepNhan.BHYTMaKhuVuc,
                NgayMiemCungTC = yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra?.ApplyFormatDate(),
                ThoiGian5Nam = yeuCauTiepNhan.BHYTNgayDu5Nam?.ApplyFormatDate(),
                CoCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,

                ChuanDoanXacDinh = yeuCauKhamBenh?.GhiChuICDChinh,
                MaICD10 = yeuCauKhamBenh?.Icdchinh?.Ma,
                BenhKemTheo = tenICDKemTheo,
                ICDKemTheo10 = maICDKemTheo,

                DateItemChiPhis = dateItemChiPhis,

                TTDV = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble(),
                TTTTBHYT = "",

                TTTienBH = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble(),
                TQBHYT = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble(),
                TNCCTra = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(),
                TongSoKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(),
                TNBTuTra = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true),

                TongChiPhi = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble(),
                SoTienBangChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV))),

                NguoiBenhPhaiTra = (Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)) + Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra))).ApplyFormatMoneyToDouble(),

                TTQuyTT = tongQuyBHYTThanhToan.ApplyFormatMoneyToDouble(),
                //TTQuyTT = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble(),
                //BHYTChiTra = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(),
                //CacKhoanTraKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true),
                SoTienKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(),
                NgayHienTai = DateTime.Now.ApplyFormatNgayThangNam(),
                
                TongTamUng = Convert.ToDouble(tongTamUng).ApplyFormatMoneyToDouble(),
                SoTienNBTuTraHoacTraLaiBN = soTienNBTuTraHoacTraLaiBN,
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }
    }
}
