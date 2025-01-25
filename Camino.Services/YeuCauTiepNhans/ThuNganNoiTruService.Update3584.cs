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
        public long HoanUng(long phieuTamUngId)
        {
            var phieuTamUng = _taiKhoanBenhNhanThuRepository.GetById(phieuTamUngId);
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
        //BACHA-458
        //khách hàng muốn in  bảng kê nội trú chờ thu
        public string GetHtmlBangKeNoiTruChoThu(ThuPhiKhamChuaBenhNoiTruVo model, string hostingName, out List<ChiPhiKhamChuaBenhNoiTruVo> danhSachTatCaChiPhi)
        {
            var yeuCauTiepNhanId = model.Id;
            danhSachTatCaChiPhi = GetTatCaDichVuKhamChuaBenh(yeuCauTiepNhanId).Result.Where(o=>!o.Soluong.AlmostEqual(0)).ToList();
            //chỉ lấy những dịch vụ chưa thu tiền
            var danhSachChiPhi = danhSachTatCaChiPhi.Where(o => o.YeuCauGoiDichVuId == null && o.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan).ToList();
            //if (danhSachChiPhi.Count == 0)
            //{
            //    return string.Empty;
            //}
            //tinh miễn giảm theo FE
            var dsChiPhiDaChon = model.DanhSachChiPhiKhamChuaBenhDaChons;
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
            foreach (var chiPhiKhamChuaBenhNoiTruVo in danhSachChiPhi)
            {
                var chiPhiDaChon = dsChiPhiDaChon.FirstOrDefault(o => o.LoaiNhom == chiPhiKhamChuaBenhNoiTruVo.LoaiNhom && o.Id == chiPhiKhamChuaBenhNoiTruVo.Id);
                if (chiPhiDaChon != null)
                {
                    chiPhiKhamChuaBenhNoiTruVo.SoTienMG = chiPhiDaChon.SoTienMG;
                }
                else
                {

                }
            }

            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId,
                x => x.Include(o => o.NoiTruBenhAn).ThenInclude(o => o.ChuyenDenBenhVien)
                    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.KhoaPhongNhapVien)
                    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.ChanDoanChinhRaVienICD)
                    .Include(o => o.YeuCauTiepNhanTheBHYTs)
                    .Include(o => o.YeuCauNhapVien).ThenInclude(o => o.YeuCauKhamBenh).ThenInclude(o => o.YeuCauTiepNhan)
                    .Include(o => o.BenhNhan)
                    .Include(o => o.PhuongXa)
                    .Include(o => o.QuanHuyen)
                    .Include(o => o.TinhThanh)
                    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.NoiTruPhieuDieuTris).ThenInclude(o => o.NoiTruThamKhamChanDoanKemTheos).ThenInclude(c => c.ICD)
                    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.NoiTruPhieuDieuTris).ThenInclude(c => c.ChanDoanChinhICD)
                    .Include(o => o.NoiChuyen).Include(o => o.NoiTruBenhAn).ThenInclude(o => o.NoiTruKhoaPhongDieuTris).ThenInclude(c => c.KhoaPhongChuyenDen));

            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeNoiTruChoThu"));

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            string chuanDoanXacDinhSanKhoa = string.Empty;
            string maICD10 = string.Empty;

            if (yeuCauTiepNhan.NoiTruBenhAn != null && (yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong))
            {
                if (yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.Count() > 0)
                {
                    var phieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).FirstOrDefault();

                    var icdKemTheoIds = phieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Select(c => c.ICD.Ma);
                    var tenICDKemTheos = phieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Select(c => c.GhiChu);

                    maICDKemTheo = string.Join(", ", icdKemTheoIds);
                    tenICDKemTheo = string.Join(", ", tenICDKemTheos);

                    maICD10 = phieuDieuTri.ChanDoanChinhICD?.Ma;
                    chuanDoanXacDinhSanKhoa = phieuDieuTri.ChanDoanChinhGhiChu;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienICDId))
                {
                    var maICDKemTheos = new List<string>();

                    var icdKemTheoIds = yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator);
                    var tenICDKemTheos = yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienGhiChu?.Split(Constants.ICDSeparator);

                    if (icdKemTheoIds.Length > 0)
                    {
                        foreach (var icdKemTheoId in icdKemTheoIds)
                        {
                            var icdKemTheo = _icdRepository.TableNoTracking.FirstOrDefault(o => o.Id == long.Parse(icdKemTheoId));
                            if (icdKemTheo != null)
                            {
                                maICDKemTheos.Add(icdKemTheo.Ma);
                            }

                            //if (long.TryParse(yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator).First(), out var icdKemTheoId))
                            //{
                            //    var icdKemTheo = _icdRepository.TableNoTracking.FirstOrDefault(o => o.Id == icdKemTheoId);
                            //    if (icdKemTheo != null)
                            //    {
                            //        maICDKemTheo = icdKemTheo.Ma;
                            //        tenICDKemTheo = yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienGhiChu?.Split(Constants.ICDSeparator).First();
                            //    }
                            //}
                        }
                    }

                    maICDKemTheo = string.Join(", ", maICDKemTheos);
                    tenICDKemTheo = string.Join(", ", tenICDKemTheos);
                }
            }


            YeuCauTiepNhanTheBHYT theBHYT = null;
            if (yeuCauTiepNhan.CoBHYT == true && yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
            {
                theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.First();
                if (!String.IsNullOrEmpty(theBHYT.MaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(theBHYT.MaDKBD));
                }
                maBHYT = theBHYT.MaSoThe;
                bHYTTuNgay = theBHYT.NgayHieuLuc.ApplyFormatDate();
                bHYTDenNgay = theBHYT.NgayHetHan?.ApplyFormatDate();
                mucHuong = theBHYT.MucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = theBHYT.MaDKBD;
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

            var dsChiPhiBangKe = new List<BangKeKhamBenhBenhVienVo>();
            var dateItemChiPhis = string.Empty;
            int indexItem = 1;
            var groupChiPhiTheoThe = danhSachChiPhi.GroupBy(o => o.MaSoTheBHYT);
            if (groupChiPhiTheoThe.Count() > 2 ||
                (groupChiPhiTheoThe.All(o => o.Key != string.Empty) && groupChiPhiTheoThe.Count() > 1))
            {
                var groupNgayPhatSinhTheoThe = groupChiPhiTheoThe.Where(o => o.Key != string.Empty).Select(o => new { o.Key, o.OrderBy(j => j.NgayPhatSinh).First().NgayPhatSinh }).OrderBy(o => o.NgayPhatSinh).ToArray();
                for (int i = 0; i < groupNgayPhatSinhTheoThe.Count(); i++)
                {
                    var ngayPhatSinhTheoThe = groupNgayPhatSinhTheoThe[i];
                    var theBHYTChiTra = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.First(o => o.MaSoThe == ngayPhatSinhTheoThe.Key);
                    var chiPhis = groupChiPhiTheoThe.FirstOrDefault(o => o.Key == ngayPhatSinhTheoThe.Key).ToList();
                    DateTime? ngayPhatSinhTiepTheo = i < groupNgayPhatSinhTheoThe.Count() - 1
                        ? groupNgayPhatSinhTheoThe[i + 1].NgayPhatSinh
                        : null;
                    chiPhis.AddRange(groupChiPhiTheoThe.FirstOrDefault(o => o.Key == string.Empty).Where(j => (i == 0 || j.NgayPhatSinh >= ngayPhatSinhTheoThe.NgayPhatSinh) && (ngayPhatSinhTiepTheo != null && j.NgayPhatSinh < ngayPhatSinhTiepTheo)).ToList());
                    var mucHuongPT = theBHYTChiTra != null && theBHYTChiTra.MucHuong > 0 ? theBHYTChiTra?.MucHuong + "%" : string.Empty;

                    dateItemChiPhis += "<table style='width: 100 %; '>" +
                                       "<tbody>" +
                                       "<tr>" +
                                       $"<td style='padding-left:-5px; width: 40 %; '>Mã thẻ BHYT {i + 1}: <b>{theBHYTChiTra.MaSoThe}</b> " +
                                       $"<span style='padding-left:20px'>Giá trị từ: <b>{theBHYTChiTra.NgayHieuLuc.ApplyFormatDate()}</b> đến <b>{theBHYTChiTra.NgayHetHan?.ApplyFormatDate()}</b></span>" +
                                       $"<span style='padding-left:20px'>Mức hưởng: <b>{mucHuongPT}</b></span></td>" +
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


                    var dsChiPhiBangKeTheoThe = new List<BangKeKhamBenhBenhVienVo>();

                    foreach (var chiPhi in chiPhis.Where(c => c.NhomChiPhiBangKe != NhomChiPhiBangKe.GoiVatTu && c.YeuCauGoiDichVuId == null).GroupBy(c => new { c.DonGia, c.SoTienMG, KhongTinhPhi = c.KhongTinhPhi.GetValueOrDefault(), c.NhomChiPhiBangKe, c.TenDichVu, c.DonViTinh, c.DuocHuongBHYT, c.DonGiaBHYT, c.MucHuongBaoHiem, c.TiLeBaoHiemThanhToan }).OrderByDescending(o => o.Key.DuocHuongBHYT))
                    {
                        var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                        {
                            Nhom = chiPhi.Key.NhomChiPhiBangKe,
                            Id = chiPhi.FirstOrDefault().Id,
                            NoiDung = chiPhi.Key.TenDichVu,
                            DonViTinh = chiPhi.Key.DonViTinh,
                            SoLuong = (decimal)chiPhi.Sum(c => c.Soluong),
                            DuocHuongBaoHiem = chiPhi.Key.DuocHuongBHYT,
                            DonGiaBH = chiPhi.Key.DuocHuongBHYT ? chiPhi.Key.DonGiaBHYT : 0,
                            MucHuongBaoHiem = chiPhi.Key.MucHuongBaoHiem,
                            TiLeThanhToanTheoDV = chiPhi.Key.TiLeBaoHiemThanhToan,
                            BaoHiemChiTra = true,
                            DonGiaBV = chiPhi.Key.DonGia
                        };
                        if (chiPhi.Key.KhongTinhPhi == true)
                        {
                            chiphiBangKe.Khac = chiPhi.Key.MucHuongBaoHiem > 0
                                ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                                : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                        }
                        else
                        {
                            chiphiBangKe.Khac = chiPhi.Sum(c => c.SoTienMG);
                        }
                        dsChiPhiBangKeTheoThe.Add(chiphiBangKe);
                    }

                    foreach (var chiPhi in chiPhis.Where(c => !(c.NhomChiPhiBangKe != NhomChiPhiBangKe.GoiVatTu && c.YeuCauGoiDichVuId == null)))
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
                        dsChiPhiBangKeTheoThe.Add(chiphiBangKe);
                    }
                    //Update BACHA-458
                    //Trường hợp NB có miễn giảm 100% tiền giường chỉ hiển thị tại cột khác
                    //Riêng cột NB cùng chi trả và NB tự trả là 0đ
                    //foreach (var chiphi in dsChiPhiBangKeTheoThe.Where(o => o.Nhom == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay ||
                    //                                                        o.Nhom == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru))
                    //{
                    //    if (chiphi.QuyBHYT > 0 && chiphi.Khac.GetValueOrDefault().SoTienTuongDuong(chiphi.ThanhTienBV.GetValueOrDefault() - chiphi.QuyBHYT))
                    //    {
                    //        chiphi.Khac = chiphi.ThanhTienBV;
                    //    }
                    //}

                    dsChiPhiBangKe.AddRange(dsChiPhiBangKeTheoThe);
                    var groupChiPhiBangKes = dsChiPhiBangKeTheoThe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
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
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b></b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true)}</b> </td>" +
                                       "</tr>" +
                                       "</table>";
                }
            }
            else
            {
                YeuCauTiepNhanTheBHYT theBHYTChiTra = null;
                var maSoThe = groupChiPhiTheoThe.Where(o => o.Key != string.Empty).FirstOrDefault()?.Key;
                if (!string.IsNullOrEmpty(maSoThe))
                {
                    theBHYTChiTra = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.First(o => o.MaSoThe == maSoThe);
                }

                var chiPhis = groupChiPhiTheoThe.SelectMany(o => o).ToList();
                var mucHuongPT = theBHYTChiTra != null && theBHYTChiTra?.MucHuong > 0 ? theBHYTChiTra?.MucHuong + "%" : string.Empty;

                dateItemChiPhis += "<table style='width: 100 %; '>" +
                                    "<tbody>" +
                                    "<tr>" +
                                       $"<td style='padding-left:-5px; width: 40 %; '>Mã thẻ BHYT: <b>{theBHYTChiTra?.MaSoThe}</b> " +
                                       $"<span style='padding-left:20px'>Giá trị từ: <b>{theBHYTChiTra?.NgayHieuLuc.ApplyFormatDate()}</b> đến <b>{theBHYTChiTra?.NgayHetHan?.ApplyFormatDate()}</b></span>" +
                                       $"<span style='padding-left:20px'>Mức hưởng: <b>{mucHuongPT}</b></span></td>" +
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
                                                "<b>Người bệnh cùng chi " +
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


                //foreach (var chiPhi in chiPhis)
                //{
                //    dsChiPhiBangKe.Add(new BangKeKhamBenhBenhVienVo
                //    {
                //        Nhom = chiPhi.NhomChiPhiBangKe,
                //        Id = chiPhi.Id,
                //        NoiDung = chiPhi.TenDichVu,
                //        DonViTinh = chiPhi.DonViTinh,
                //        SoLuong = (decimal)chiPhi.Soluong,
                //        DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                //        DonGiaBH = chiPhi.DuocHuongBHYT ? chiPhi.DonGiaBHYT : 0,
                //        MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                //        TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                //        BaoHiemChiTra = true,
                //        DonGiaBV = chiPhi.DonGia
                //    });
                //}

                foreach (var chiPhi in chiPhis.Where(c => c.NhomChiPhiBangKe != NhomChiPhiBangKe.GoiVatTu && c.YeuCauGoiDichVuId == null).GroupBy(c => new { c.DonGia, c.SoTienMG, KhongTinhPhi = c.KhongTinhPhi.GetValueOrDefault(), c.NhomChiPhiBangKe, c.TenDichVu, c.DonViTinh, c.DuocHuongBHYT, c.DonGiaBHYT, c.MucHuongBaoHiem, c.TiLeBaoHiemThanhToan }).OrderByDescending(o => o.Key.DuocHuongBHYT))
                {
                    var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                    {
                        Nhom = chiPhi.Key.NhomChiPhiBangKe,
                        Id = chiPhi.FirstOrDefault().Id,
                        NoiDung = chiPhi.Key.TenDichVu,
                        DonViTinh = chiPhi.Key.DonViTinh,
                        SoLuong = (decimal)chiPhi.Sum(c => c.Soluong),
                        DuocHuongBaoHiem = chiPhi.Key.DuocHuongBHYT,
                        DonGiaBH = chiPhi.Key.DuocHuongBHYT ? chiPhi.Key.DonGiaBHYT : 0,
                        MucHuongBaoHiem = chiPhi.Key.MucHuongBaoHiem,
                        TiLeThanhToanTheoDV = chiPhi.Key.TiLeBaoHiemThanhToan,
                        BaoHiemChiTra = true,
                        DonGiaBV = chiPhi.Key.DonGia
                    };
                    if (chiPhi.Key.KhongTinhPhi == true)
                    {
                        chiphiBangKe.Khac = chiPhi.Key.MucHuongBaoHiem > 0
                            ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                            : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                    }
                    else
                    {
                        chiphiBangKe.Khac = chiPhi.Sum(c => c.SoTienMG);
                    }
                    dsChiPhiBangKe.Add(chiphiBangKe);
                }

                foreach (var chiPhi in chiPhis.Where(c => !(c.NhomChiPhiBangKe != NhomChiPhiBangKe.GoiVatTu && c.YeuCauGoiDichVuId == null)))
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

                //Update BACHA-458
                //Trường hợp NB có miễn giảm 100% tiền giường chỉ hiển thị tại cột khác
                //Riêng cột NB cùng chi trả và NB tự trả là 0đ
                //foreach (var chiphi in dsChiPhiBangKe.Where(o => o.Nhom == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay ||
                //                                                 o.Nhom == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru))
                //{
                //    if (chiphi.QuyBHYT > 0 && chiphi.Khac.GetValueOrDefault().SoTienTuongDuong(chiphi.ThanhTienBV.GetValueOrDefault() - chiphi.QuyBHYT))
                //    {
                //        chiphi.Khac = chiphi.ThanhTienBV;
                //    }
                //}

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

            }

            var khoaPhong = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruKhoaPhongDieuTris.OrderByDescending(c => c.ThoiDiemVaoKhoa).Select(c => c.KhoaPhongChuyenDen).FirstOrDefault();
          
            //BVHD-3792           
            var chuyenKhoa = GetChuyenKhoa(khoaPhong?.Id);

            var checkedLyDoVaoVien = "<input id='demo_box_2' class='css - checkbox' type='checkbox' checked  disabled='true'/>";
            var unCheckedLyDoVaoVien = "<input id='demo_box_2' class='css - checkbox' type='checkbox' disabled='true' />";


            // tinh luon trong goi
            var tongQuyBHYTThanhToan = Convert.ToDouble(danhSachTatCaChiPhi.Where(o => o.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && o.KiemTraBHYTXacNhan).Sum(o => o.BHYTThanhToan));
            var tongBHTN = Convert.ToDouble(danhSachTatCaChiPhi.Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan).Sum(o => o.TongCongNo));
            string tongBHTNTrongGoiText = $"</br> - BHTN bảo lãnh: {tongBHTN.ApplyFormatMoneyToDouble()}";
            var tongTamUng = GetSoTienDaTamUngAsync(yeuCauTiepNhanId).Result;
            //Tổng chi phí - Tạm ứng - Quỹ BH - Nguồn khác
            var soTienCanThu = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)) - Convert.ToDouble(tongTamUng) - tongQuyBHYTThanhToan - tongBHTN - Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac));
            string soTienNBTuTraHoacTraLaiBN = soTienCanThu < 0 ? $"- Phải trả lại NB: {(soTienCanThu * (-1)).ApplyFormatMoneyToDouble()}" : $"- Người bệnh tự trả: {(soTienCanThu).ApplyFormatMoneyToDouble()}";

            var data = new
            {
                TitleBangKe = "KHÁM CHỮA BỆNH </br>CHỜ THU",
                SoBangKe = "3",
                yeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = "Số bệnh án: " + yeuCauTiepNhan.NoiTruBenhAn?.SoBenhAn,
                yeuCauTiepNhan.HoTen,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                NamSinh = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh),
                GioiTinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "",
                //TenKhoa = "Khoa: " + yeuCauTiepNhan.NoiTruBenhAn?.KhoaPhongNhapVien?.Ten,
                //MaKhoa = "Mã khoa: " + yeuCauTiepNhan.NoiTruBenhAn?.KhoaPhongNhapVien?.Ma,

                TenKhoa = "Khoa: " + khoaPhong?.Ten,
                MaKhoa = "Mã khoa: " + chuyenKhoa?.Ma,

                MaBHYT = theBHYT?.MaSoThe,
                BHYTTuNgay = bHYTTuNgay,
                BHYTDenNgay = bHYTDenNgay,
                NoiDKKCBBanDau,
                MaKCBBanDau,
                NgayDenKham = yeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay(),
                DieuTriKNT = yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemNhapVien.ApplyFormatGioPhutNgay(),
                KetThucKhamNgoaiTru = yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien?.ApplyFormatGioPhutNgay(),
                SoNgayDTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(yeuCauTiepNhan.NoiTruBenhAn)?.ToString() ?? string.Empty,//yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien == null ? "" : (yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien.Value.Date.AddDays(1) - yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.Date).Days.ToString(),
                TinhTrangRaVien = yeuCauTiepNhan.NoiTruBenhAn?.TinhTrangRaVien == EnumTinhTrangRaVien.RaVien && (yeuCauTiepNhan.NoiTruBenhAn?.KetQuaDieuTri == EnumKetQuaDieuTri.Do || yeuCauTiepNhan.NoiTruBenhAn?.KetQuaDieuTri == EnumKetQuaDieuTri.Khoi) ? 1 : 2,
                MucHuong = mucHuong,
                NoiChuyenDen = yeuCauTiepNhan.NoiChuyen?.Ten,
                NoiChuyenDi = yeuCauTiepNhan.NoiTruBenhAn?.ChuyenDenBenhVien?.Ten,
                MKV = theBHYT?.MaKhuVuc,
                NgayMiemCungTC = theBHYT?.NgayDuocMienCungChiTra?.ApplyFormatDate(),
                ThoiGian5Nam = theBHYT?.NgayDu5Nam?.ApplyFormatDate(),
                CoCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,

                ChuanDoanXacDinh = yeuCauTiepNhan.NoiTruBenhAn != null && (yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong) ? chuanDoanXacDinhSanKhoa : yeuCauTiepNhan.NoiTruBenhAn?.ChanDoanChinhRaVienGhiChu,
                MaICD10 = yeuCauTiepNhan.NoiTruBenhAn != null && (yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong) ? maICD10 : yeuCauTiepNhan.NoiTruBenhAn?.ChanDoanChinhRaVienICD?.Ma,

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
                //BHYTChiTra = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(),
                //CacKhoanTraKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true),
                SoTienKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(),
                NgayHienTai = DateTime.Now.ApplyFormatNgayThangNam(),
                Notice = "<div style='color: red; font-weight:800; margin-top:200px;'>Lưu ý : Người bệnh chưa quyết toán </div>",

                //ToDo 
                TongBHTNTrongGoi = tongBHTNTrongGoiText,
                TongTamUng = Convert.ToDouble(tongTamUng).ApplyFormatMoneyToDouble(),
                SoTienNBTuTraHoacTraLaiBN = soTienNBTuTraHoacTraLaiBN,
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }

        public string GetHtmlBangKeNoiTruChoThuTrongGoi(QuyetToanDichVuTrongGoiVo model, string hostingName)
        {
            var yeuCauTiepNhanId = model.Id;
            var danhSachTatCaChiPhi = GetTatCaDichVuKhamChuaBenh(yeuCauTiepNhanId).Result.Where(o => !o.Soluong.AlmostEqual(0));
            //chỉ lấy những dịch vụ chưa thu tiền
            var danhSachChiPhi = danhSachTatCaChiPhi.Where(o => o.YeuCauGoiDichVuId != null && o.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan).ToList();


            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId,
                x => x.Include(o => o.NoiTruBenhAn).ThenInclude(o => o.ChuyenDenBenhVien)
                    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.KhoaPhongNhapVien)
                    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.ChanDoanChinhRaVienICD)
                    .Include(o => o.YeuCauTiepNhanTheBHYTs)
                    .Include(o => o.YeuCauNhapVien).ThenInclude(o => o.YeuCauKhamBenh).ThenInclude(o => o.YeuCauTiepNhan)
                    .Include(o => o.BenhNhan)
                    .Include(o => o.PhuongXa)
                    .Include(o => o.QuanHuyen)
                    .Include(o => o.TinhThanh)
                    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.NoiTruPhieuDieuTris).ThenInclude(o => o.NoiTruThamKhamChanDoanKemTheos).ThenInclude(c => c.ICD)
                    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.NoiTruPhieuDieuTris).ThenInclude(c => c.ChanDoanChinhICD)
                    .Include(o => o.NoiChuyen).Include(o => o.NoiTruBenhAn).ThenInclude(o => o.NoiTruKhoaPhongDieuTris).ThenInclude(c => c.KhoaPhongChuyenDen));

            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeNoiTruChoThu"));

            var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();

            string maBHYT = string.Empty;
            string bHYTTuNgay = string.Empty;
            string bHYTDenNgay = string.Empty;
            string mucHuong = string.Empty;
            string NoiDKKCBBanDau = string.Empty;
            string MaKCBBanDau = string.Empty;


            string maICDKemTheo = string.Empty;
            string tenICDKemTheo = string.Empty;

            string chuanDoanXacDinhSanKhoa = string.Empty;
            string maICD10 = string.Empty;

            if (yeuCauTiepNhan.NoiTruBenhAn != null && (yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong))
            {
                if (yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.Count() > 0)
                {
                    var phieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).FirstOrDefault();

                    var icdKemTheoIds = phieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Select(c => c.ICD.Ma);
                    var tenICDKemTheos = phieuDieuTri.NoiTruThamKhamChanDoanKemTheos.Select(c => c.GhiChu);

                    maICDKemTheo = string.Join(", ", icdKemTheoIds);
                    tenICDKemTheo = string.Join(", ", tenICDKemTheos);

                    maICD10 = phieuDieuTri.ChanDoanChinhICD?.Ma;
                    chuanDoanXacDinhSanKhoa = phieuDieuTri.ChanDoanChinhGhiChu;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienICDId))
                {
                    var maICDKemTheos = new List<string>();

                    var icdKemTheoIds = yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator);
                    var tenICDKemTheos = yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienGhiChu?.Split(Constants.ICDSeparator);

                    if (icdKemTheoIds.Length > 0)
                    {
                        foreach (var icdKemTheoId in icdKemTheoIds)
                        {
                            var icdKemTheo = _icdRepository.TableNoTracking.FirstOrDefault(o => o.Id == long.Parse(icdKemTheoId));
                            if (icdKemTheo != null)
                            {
                                maICDKemTheos.Add(icdKemTheo.Ma);
                            }

                            //if (long.TryParse(yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator).First(), out var icdKemTheoId))
                            //{
                            //    var icdKemTheo = _icdRepository.TableNoTracking.FirstOrDefault(o => o.Id == icdKemTheoId);
                            //    if (icdKemTheo != null)
                            //    {
                            //        maICDKemTheo = icdKemTheo.Ma;
                            //        tenICDKemTheo = yeuCauTiepNhan.NoiTruBenhAn?.DanhSachChanDoanKemTheoRaVienGhiChu?.Split(Constants.ICDSeparator).First();
                            //    }
                            //}
                        }
                    }

                    maICDKemTheo = string.Join(", ", maICDKemTheos);
                    tenICDKemTheo = string.Join(", ", tenICDKemTheos);
                }
            }

            YeuCauTiepNhanTheBHYT theBHYT = null;
            if (yeuCauTiepNhan.CoBHYT == true && yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
            {
                theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.First();
                if (!String.IsNullOrEmpty(theBHYT.MaDKBD))
                {
                    benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma.Equals(theBHYT.MaDKBD));
                }
                maBHYT = theBHYT.MaSoThe;
                bHYTTuNgay = theBHYT.NgayHieuLuc.ApplyFormatDate();
                bHYTDenNgay = theBHYT.NgayHetHan?.ApplyFormatDate();
                mucHuong = theBHYT.MucHuong.ToString() + "%";
                NoiDKKCBBanDau = benhVien != null ? benhVien?.Ten : "Chưa xác định";
                MaKCBBanDau = theBHYT.MaDKBD;
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

            var dsChiPhiBangKe = new List<BangKeKhamBenhBenhVienVo>();
            var dateItemChiPhis = string.Empty;
            int indexItem = 1;
            var groupChiPhiTheoThe = danhSachChiPhi.GroupBy(o => o.MaSoTheBHYT);
            if (groupChiPhiTheoThe.Count() > 2 ||
                (groupChiPhiTheoThe.All(o => o.Key != string.Empty) && groupChiPhiTheoThe.Count() > 1))
            {
                var groupNgayPhatSinhTheoThe = groupChiPhiTheoThe.Where(o => o.Key != string.Empty).Select(o => new { o.Key, o.OrderBy(j => j.NgayPhatSinh).First().NgayPhatSinh }).OrderBy(o => o.NgayPhatSinh).ToArray();
                for (int i = 0; i < groupNgayPhatSinhTheoThe.Count(); i++)
                {
                    var ngayPhatSinhTheoThe = groupNgayPhatSinhTheoThe[i];
                    var theBHYTChiTra = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.First(o => o.MaSoThe == ngayPhatSinhTheoThe.Key);
                    var chiPhis = groupChiPhiTheoThe.FirstOrDefault(o => o.Key == ngayPhatSinhTheoThe.Key).ToList();
                    DateTime? ngayPhatSinhTiepTheo = i < groupNgayPhatSinhTheoThe.Count() - 1
                        ? groupNgayPhatSinhTheoThe[i + 1].NgayPhatSinh
                        : null;
                    chiPhis.AddRange(groupChiPhiTheoThe.FirstOrDefault(o => o.Key == string.Empty).Where(j => (i == 0 || j.NgayPhatSinh >= ngayPhatSinhTheoThe.NgayPhatSinh) && (ngayPhatSinhTiepTheo != null && j.NgayPhatSinh < ngayPhatSinhTiepTheo)).ToList());
                    var mucHuongPT = theBHYTChiTra != null && theBHYTChiTra.MucHuong > 0 ? theBHYTChiTra?.MucHuong + "%" : string.Empty;

                    dateItemChiPhis += "<table style='width: 100 %; '>" +
                                       "<tbody>" +
                                       "<tr>" +
                                       $"<td style='padding-left:-5px; width: 40 %; '>Mã thẻ BHYT {i + 1}: <b>{theBHYTChiTra.MaSoThe}</b> " +
                                       $"<span style='padding-left:20px'>Giá trị từ: <b>{theBHYTChiTra.NgayHieuLuc.ApplyFormatDate()}</b> đến <b>{theBHYTChiTra.NgayHetHan?.ApplyFormatDate()}</b></span>" +
                                       $"<span style='padding-left:20px'>Mức hưởng: <b>{mucHuongPT}</b></span></td>" +
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


                    var dsChiPhiBangKeTheoThe = new List<BangKeKhamBenhBenhVienVo>();

                    foreach (var chiPhi in chiPhis.Where(c => c.NhomChiPhiBangKe != NhomChiPhiBangKe.GoiVatTu && c.YeuCauGoiDichVuId == null).GroupBy(c => new { c.DonGia, c.SoTienMG, KhongTinhPhi = c.KhongTinhPhi.GetValueOrDefault(), c.NhomChiPhiBangKe, c.TenDichVu, c.DonViTinh, c.DuocHuongBHYT, c.DonGiaBHYT, c.MucHuongBaoHiem, c.TiLeBaoHiemThanhToan }).OrderByDescending(o => o.Key.DuocHuongBHYT))
                    {
                        var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                        {
                            Nhom = chiPhi.Key.NhomChiPhiBangKe,
                            Id = chiPhi.FirstOrDefault().Id,
                            NoiDung = chiPhi.Key.TenDichVu,
                            DonViTinh = chiPhi.Key.DonViTinh,
                            SoLuong = (decimal)chiPhi.Sum(c => c.Soluong),
                            DuocHuongBaoHiem = chiPhi.Key.DuocHuongBHYT,
                            DonGiaBH = chiPhi.Key.DuocHuongBHYT ? chiPhi.Key.DonGiaBHYT : 0,
                            MucHuongBaoHiem = chiPhi.Key.MucHuongBaoHiem,
                            TiLeThanhToanTheoDV = chiPhi.Key.TiLeBaoHiemThanhToan,
                            BaoHiemChiTra = true,
                            DonGiaBV = chiPhi.Key.DonGia
                        };
                        if (chiPhi.Key.KhongTinhPhi == true)
                        {
                            chiphiBangKe.Khac = chiPhi.Key.MucHuongBaoHiem > 0
                                ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                                : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                        }
                        else
                        {
                            chiphiBangKe.Khac = chiPhi.Sum(c => c.SoTienMG);
                        }
                        dsChiPhiBangKeTheoThe.Add(chiphiBangKe);
                    }

                    foreach (var chiPhi in chiPhis.Where(c => !(c.NhomChiPhiBangKe != NhomChiPhiBangKe.GoiVatTu && c.YeuCauGoiDichVuId == null)))
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
                        dsChiPhiBangKeTheoThe.Add(chiphiBangKe);
                    }
                    //Update BACHA-458
                    //Trường hợp NB có miễn giảm 100% tiền giường chỉ hiển thị tại cột khác
                    //Riêng cột NB cùng chi trả và NB tự trả là 0đ
                    //foreach (var chiphi in dsChiPhiBangKeTheoThe.Where(o => o.Nhom == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay ||
                    //                                                        o.Nhom == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru))
                    //{
                    //    if (chiphi.QuyBHYT > 0 && chiphi.Khac.GetValueOrDefault().SoTienTuongDuong(chiphi.ThanhTienBV.GetValueOrDefault() - chiphi.QuyBHYT))
                    //    {
                    //        chiphi.Khac = chiphi.ThanhTienBV;
                    //    }
                    //}

                    dsChiPhiBangKe.AddRange(dsChiPhiBangKeTheoThe);
                    var groupChiPhiBangKes = dsChiPhiBangKeTheoThe.GroupBy(x => x.Nhom).OrderBy(o => (int)o.Key * (o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay || o.Key == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru ? 1 : 10));
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
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.ThanhTienBV)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b></b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.ThanhTienBH)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.QuyBHYT)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                       $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsChiPhiBangKeTheoThe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true)}</b> </td>" +
                                       "</tr>" +
                                       "</table>";
                }
            }
            else
            {
                YeuCauTiepNhanTheBHYT theBHYTChiTra = null;
                var maSoThe = groupChiPhiTheoThe.Where(o => o.Key != string.Empty).FirstOrDefault()?.Key;
                if (!string.IsNullOrEmpty(maSoThe))
                {
                    theBHYTChiTra = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.First(o => o.MaSoThe == maSoThe);
                }

                var chiPhis = groupChiPhiTheoThe.SelectMany(o => o).ToList();
                var mucHuongPT = theBHYTChiTra != null && theBHYTChiTra?.MucHuong > 0 ? theBHYTChiTra?.MucHuong + "%" : string.Empty;

                dateItemChiPhis += "<table style='width: 100 %; '>" +
                                    "<tbody>" +
                                    "<tr>" +
                                       $"<td style='padding-left:-5px; width: 40 %; '>Mã thẻ BHYT: <b>{theBHYTChiTra?.MaSoThe}</b> " +
                                       $"<span style='padding-left:20px'>Giá trị từ: <b>{theBHYTChiTra?.NgayHieuLuc.ApplyFormatDate()}</b> đến <b>{theBHYTChiTra?.NgayHetHan?.ApplyFormatDate()}</b></span>" +
                                       $"<span style='padding-left:20px'>Mức hưởng: <b>{mucHuongPT}</b></span></td>" +
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
                                                "<b>Người bệnh cùng chi " +
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


                //foreach (var chiPhi in chiPhis)
                //{
                //    dsChiPhiBangKe.Add(new BangKeKhamBenhBenhVienVo
                //    {
                //        Nhom = chiPhi.NhomChiPhiBangKe,
                //        Id = chiPhi.Id,
                //        NoiDung = chiPhi.TenDichVu,
                //        DonViTinh = chiPhi.DonViTinh,
                //        SoLuong = (decimal)chiPhi.Soluong,
                //        DuocHuongBaoHiem = chiPhi.DuocHuongBHYT,
                //        DonGiaBH = chiPhi.DuocHuongBHYT ? chiPhi.DonGiaBHYT : 0,
                //        MucHuongBaoHiem = chiPhi.MucHuongBaoHiem,
                //        TiLeThanhToanTheoDV = chiPhi.TiLeBaoHiemThanhToan,
                //        BaoHiemChiTra = true,
                //        DonGiaBV = chiPhi.DonGia
                //    });
                //}

                foreach (var chiPhi in chiPhis.Where(c => c.NhomChiPhiBangKe != NhomChiPhiBangKe.GoiVatTu && c.YeuCauGoiDichVuId == null).GroupBy(c => new { c.DonGia, c.SoTienMG, KhongTinhPhi = c.KhongTinhPhi.GetValueOrDefault(), c.NhomChiPhiBangKe, c.TenDichVu, c.DonViTinh, c.DuocHuongBHYT, c.DonGiaBHYT, c.MucHuongBaoHiem, c.TiLeBaoHiemThanhToan }).OrderByDescending(o => o.Key.DuocHuongBHYT))
                {
                    var chiphiBangKe = new BangKeKhamBenhBenhVienVo
                    {
                        Nhom = chiPhi.Key.NhomChiPhiBangKe,
                        Id = chiPhi.FirstOrDefault().Id,
                        NoiDung = chiPhi.Key.TenDichVu,
                        DonViTinh = chiPhi.Key.DonViTinh,
                        SoLuong = (decimal)chiPhi.Sum(c => c.Soluong),
                        DuocHuongBaoHiem = chiPhi.Key.DuocHuongBHYT,
                        DonGiaBH = chiPhi.Key.DuocHuongBHYT ? chiPhi.Key.DonGiaBHYT : 0,
                        MucHuongBaoHiem = chiPhi.Key.MucHuongBaoHiem,
                        TiLeThanhToanTheoDV = chiPhi.Key.TiLeBaoHiemThanhToan,
                        BaoHiemChiTra = true,
                        DonGiaBV = chiPhi.Key.DonGia
                    };
                    if (chiPhi.Key.KhongTinhPhi == true)
                    {
                        chiphiBangKe.Khac = chiPhi.Key.MucHuongBaoHiem > 0
                            ? (chiphiBangKe.ThanhTienBV.GetValueOrDefault() - chiphiBangKe.ThanhTienBH.GetValueOrDefault())
                            : chiphiBangKe.ThanhTienBV.GetValueOrDefault();
                    }
                    else
                    {
                        chiphiBangKe.Khac = chiPhi.Sum(c => c.SoTienMG);
                    }
                    dsChiPhiBangKe.Add(chiphiBangKe);
                }

                foreach (var chiPhi in chiPhis.Where(c => !(c.NhomChiPhiBangKe != NhomChiPhiBangKe.GoiVatTu && c.YeuCauGoiDichVuId == null)))
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

                //Update BACHA-458
                //Trường hợp NB có miễn giảm 100% tiền giường chỉ hiển thị tại cột khác
                //Riêng cột NB cùng chi trả và NB tự trả là 0đ
                //foreach (var chiphi in dsChiPhiBangKe.Where(o => o.Nhom == NhomChiPhiBangKe.NgayGiuongDieuTriBanNgay ||
                //                                                 o.Nhom == NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru))
                //{
                //    if (chiphi.QuyBHYT > 0 && chiphi.Khac.GetValueOrDefault().SoTienTuongDuong(chiphi.ThanhTienBV.GetValueOrDefault() - chiphi.QuyBHYT))
                //    {
                //        chiphi.Khac = chiphi.ThanhTienBV;
                //    }
                //}

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

            }

            var khoaPhong = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruKhoaPhongDieuTris.OrderByDescending(c => c.ThoiDiemVaoKhoa).Select(c => c.KhoaPhongChuyenDen).FirstOrDefault();

            //BVHD-3792           
            var chuyenKhoa = GetChuyenKhoa(khoaPhong?.Id);

            var checkedLyDoVaoVien = "<input id='demo_box_2' class='css - checkbox' type='checkbox' checked  disabled='true'/>";
            var unCheckedLyDoVaoVien = "<input id='demo_box_2' class='css - checkbox' type='checkbox' disabled='true' />";


            // tinh luon trong goi
            var tongQuyBHYTThanhToan = Convert.ToDouble(danhSachChiPhi.Sum(o => o.BHYTThanhToan));
            var tongTamUng = 0;//GetSoTienDaTamUngAsync(yeuCauTiepNhanId).Result;
            //Tổng chi phí - Tạm ứng - Quỹ BH - Nguồn khác
            var soTienCanThu = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.ThanhTienBV)) - Convert.ToDouble(tongTamUng) - tongQuyBHYTThanhToan - Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac));
            string soTienNBTuTraHoacTraLaiBN = soTienCanThu < 0 ? $"- Phải trả lại NB: {(soTienCanThu * (-1)).ApplyFormatMoneyToDouble()}" : $"- Người bệnh tự trả: {(soTienCanThu).ApplyFormatMoneyToDouble()}";

            var data = new
            {
                TitleBangKe = "KHÁM CHỮA BỆNH </br>CHỜ THU",
                SoBangKe = "3",
                yeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                SoBenhAn = "Số bệnh án: " + yeuCauTiepNhan.NoiTruBenhAn?.SoBenhAn,
                yeuCauTiepNhan.HoTen,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                NamSinh = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh),
                GioiTinh = yeuCauTiepNhan.GioiTinh != null ? yeuCauTiepNhan.GioiTinh.GetDescription() : "",
                //TenKhoa = "Khoa: " + yeuCauTiepNhan.NoiTruBenhAn?.KhoaPhongNhapVien?.Ten,
                //MaKhoa = "Mã khoa: " + yeuCauTiepNhan.NoiTruBenhAn?.KhoaPhongNhapVien?.Ma,

                TenKhoa = "Khoa: " + khoaPhong?.Ten,
                MaKhoa = "Mã khoa: " + chuyenKhoa?.Ma,

                MaBHYT = theBHYT?.MaSoThe,
                BHYTTuNgay = bHYTTuNgay,
                BHYTDenNgay = bHYTDenNgay,
                NoiDKKCBBanDau,
                MaKCBBanDau,
                NgayDenKham = yeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatGioPhutNgay(),
                DieuTriKNT = yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemNhapVien.ApplyFormatGioPhutNgay(),
                KetThucKhamNgoaiTru = yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien?.ApplyFormatGioPhutNgay(),
                SoNgayDTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(yeuCauTiepNhan.NoiTruBenhAn)?.ToString() ?? string.Empty,//yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien == null ? "" : (yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien.Value.Date.AddDays(1) - yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.Date).Days.ToString(),
                TinhTrangRaVien = yeuCauTiepNhan.NoiTruBenhAn?.TinhTrangRaVien == EnumTinhTrangRaVien.RaVien && (yeuCauTiepNhan.NoiTruBenhAn?.KetQuaDieuTri == EnumKetQuaDieuTri.Do || yeuCauTiepNhan.NoiTruBenhAn?.KetQuaDieuTri == EnumKetQuaDieuTri.Khoi) ? 1 : 2,
                MucHuong = mucHuong,
                NoiChuyenDen = yeuCauTiepNhan.NoiChuyen?.Ten,
                NoiChuyenDi = yeuCauTiepNhan.NoiTruBenhAn?.ChuyenDenBenhVien?.Ten,
                MKV = theBHYT?.MaKhuVuc,
                NgayMiemCungTC = theBHYT?.NgayDuocMienCungChiTra?.ApplyFormatDate(),
                ThoiGian5Nam = theBHYT?.NgayDu5Nam?.ApplyFormatDate(),
                CoCapCuu = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.CapCuu ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoDungTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.DungTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoThongTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.ThongTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,
                CoTraiTuyen = yeuCauTiepNhan.LyDoVaoVien == Enums.EnumLyDoVaoVien.TraiTuyen ? checkedLyDoVaoVien : unCheckedLyDoVaoVien,

                ChuanDoanXacDinh = yeuCauTiepNhan.NoiTruBenhAn != null && (yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong) ? chuanDoanXacDinhSanKhoa : yeuCauTiepNhan.NoiTruBenhAn?.ChanDoanChinhRaVienGhiChu,
                MaICD10 = yeuCauTiepNhan.NoiTruBenhAn != null && (yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong) ? maICD10 : yeuCauTiepNhan.NoiTruBenhAn?.ChanDoanChinhRaVienICD?.Ma,

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
                //BHYTChiTra = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhCungChiTra)).ApplyFormatMoneyToDouble(),
                //CacKhoanTraKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.NguoiBenhTuTra)).ApplyFormatMoneyToDouble(true),
                SoTienKhac = Convert.ToDouble(dsChiPhiBangKe.Sum(o => o.Khac)).ApplyFormatMoneyToDouble(),
                NgayHienTai = DateTime.Now.ApplyFormatNgayThangNam(),
                Notice = "<div style='color: red; font-weight:800; margin-top:200px;'>Lưu ý : Người bệnh chưa quyết toán </div>",

                TongTamUng = Convert.ToDouble(tongTamUng).ApplyFormatMoneyToDouble(),
                SoTienNBTuTraHoacTraLaiBN = soTienNBTuTraHoacTraLaiBN,
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }
    }
}
