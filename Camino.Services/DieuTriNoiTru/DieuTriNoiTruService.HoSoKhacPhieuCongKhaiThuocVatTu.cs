using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiDichVuKyThuat;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiThuocVatTu;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiThuocVatTu;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        private List<string> GetHtmlTableCongKhaiThuocVatTuTheoRangeDate(long yeuCauTiepNhanId)
        {
            List<string> returnHtmlAll = new List<string>();
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(x =>
                                                           x.Id == yeuCauTiepNhanId)
                                                           .Include(c => c.NoiTruPhieuDieuTris).ThenInclude(c => c.YeuCauVatTuBenhViens)
                                                           .Include(c => c.NoiTruPhieuDieuTris).ThenInclude(c => c.YeuCauDuocPhamBenhViens)
                                                           .Include(c => c.NoiTruPhieuDieuTris)
                                                           .ThenInclude(c => c.ChanDoanChinhICD).FirstOrDefault();

            var ngayDieuTriBatDau = noiTruBenhAn.ThoiDiemNhapVien.ToString("dd/MM/yyyy");
            var ngayDieuTriKetThuc = noiTruBenhAn.ThoiDiemRaVien?.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy");



            var tatCaNDT = noiTruBenhAn.NoiTruPhieuDieuTris.Where(c => c.YeuCauVatTuBenhViens.Any() || c.YeuCauDuocPhamBenhViens.Any())
                                       .OrderBy(c => c.NgayDieuTri).Select(c => c.NgayDieuTri).Distinct();

           
            //kiểm tra ngày đầu tiền và ngày kết thuc có vật tư và thuốc
            if (tatCaNDT.Count() > 13)
            {
                var tachPhieus = tatCaNDT.ToArray();

                for (int ii = 0; ii < tachPhieus.Length; ii = ii + 13)
                {
                    var start = tachPhieus[ii];
                    var end = tachPhieus.Length - ii > 13 ? tachPhieus[ii + 12] : tachPhieus[tachPhieus.Length - 1];

                    #region Yêu cầu dịch vụ kỹ thuật được bảo hiểm chi trả

                    var danhSachDichVuKyThuats = new List<PhieuCongKhaiThuocVatTuGridVo>();

                    danhSachDichVuKyThuats.AddRange(_yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.BaoHiemChiTra == true &&
                                    p.NoiTruPhieuDieuTri.NgayDieuTri >= start && p.NoiTruPhieuDieuTri.NgayDieuTri <= end &&
                                    p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                                                        .Select(s => new PhieuCongKhaiThuocVatTuGridVo
                                                        {
                                                            TenThuocVatTu = s.Ten,
                                                            DonVi = s.DonViTinh.Ten,
                                                            SoLuong = s.SoLuong,
                                                            DonGia = (decimal)s.DonGiaBaoHiem,
                                                            ThanhTien = (decimal)(s.SoLuong * (double)(s.DonGiaBaoHiem.GetValueOrDefault() * s.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * s.MucHuongBaoHiem.GetValueOrDefault() / 100)),
                                                            GhiChu = s.GhiChu,
                                                            NgayThang = s.NoiTruPhieuDieuTri.NgayDieuTri,
                                                            LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                                            DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                                            HoatChat = s.HoatChat,
                                                            HamLuong = s.HamLuong,
                                                            LaDuocPham = true // mặc định
                                                        })
                                                        .GroupBy(x => new { x.TenThuocVatTu, x.NgayThang })
                                                        .Select(item => new PhieuCongKhaiThuocVatTuGridVo
                                                        {
                                                            TenThuocVatTu = item.First().TenThuocVatTu,
                                                            DonVi = item.First().DonVi,
                                                            SoLuong = item.First().SoLuong,
                                                            TongSo = item.Sum(x => x.SoLuong),
                                                            DonGia = item.First().DonGia,
                                                            ThanhTien = item.Sum(x => x.ThanhTien),
                                                            GhiChu = item.First().GhiChu,
                                                            NgayThang = item.First().NgayThang,
                                                            LoaiThuocTheoQuanLy = item.First().LoaiThuocTheoQuanLy,
                                                            DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVienPhanNhomId,
                                                            HoatChat = item.First().HoatChat,
                                                            HamLuong = item.First().HamLuong,
                                                            LaDuocPham = item.First().LaDuocPham
                                                        }).Distinct().ToList());

                    danhSachDichVuKyThuats.AddRange(_yeuCauVatTuBenhVienRepository.TableNoTracking
                                                        .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.BaoHiemChiTra == true &&
                                                                    p.NoiTruPhieuDieuTri.NgayDieuTri >= start && p.NoiTruPhieuDieuTri.NgayDieuTri <= end &&
                                                                    p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                                                        .Select(s => new PhieuCongKhaiThuocVatTuGridVo
                                                        {
                                                            TenThuocVatTu = s.Ten,
                                                            DonVi = s.DonViTinh,
                                                            SoLuong = s.SoLuong,
                                                            DonGia = (decimal)s.DonGiaBaoHiem,
                                                            ThanhTien = (decimal)(s.SoLuong * (double)(s.DonGiaBaoHiem.GetValueOrDefault() * s.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * s.MucHuongBaoHiem.GetValueOrDefault() / 100)),
                                                            GhiChu = s.GhiChu,
                                                            NgayThang = s.NoiTruPhieuDieuTri.NgayDieuTri,
                                                        })
                                                        .GroupBy(x => new { x.TenThuocVatTu, x.DonVi, x.DonGia, x.NgayThang, x.SoLuong })
                                                        .Select(item => new PhieuCongKhaiThuocVatTuGridVo
                                                        {
                                                            TenThuocVatTu = item.First().TenThuocVatTu,
                                                            DonVi = item.First().DonVi,
                                                            SoLuong = item.First().SoLuong,
                                                            TongSo = item.Sum(x => x.SoLuong),
                                                            DonGia = item.First().DonGia,
                                                            ThanhTien = item.Sum(x => x.ThanhTien),
                                                            GhiChu = item.First().GhiChu,
                                                            NgayThang = item.First().NgayThang,
                                                        }).Distinct().ToList());

                    #endregion


                    var ngayDieuTriTheo13Phieus = tatCaNDT.Where(p => p >= start && p <= end);
                    var ngayDieuTri = new List<DateTime>();
                    var returnHtml = string.Empty;

                    if (ngayDieuTriTheo13Phieus != null && ngayDieuTriTheo13Phieus.Any())
                    {
                        foreach (var item in ngayDieuTriTheo13Phieus)
                        {
                            ngayDieuTri.Add(item);
                        }
                    }
                    var column = ngayDieuTri.Any() ? ngayDieuTri.Count() : 1;


                    returnHtml += "<table id='dichvu' style='border-collapse: collapse;width:100%'> " +
                                                "<tr>" +
                                                "<th  rowspan ='2' width='30px'><b>TT</b> </th>" +
                                                "<th rowspan ='2'><b>TÊN DỊCH VỤ KHÁM BỆNH ,CHỮA BỆNH</b></th>" +
                                                "<th  rowspan = '2'width='50px'><b> ĐVT </b> </th >" +
                                                "<th colspan='" + column + "' style='min-width:180px'><b>Đợt điều trị từ ngày " + ngayDieuTriBatDau + " đến " + ngayDieuTriKetThuc + "</b></th>" +
                                                "<th rowspan ='2' width='40px'><b>Tổng<br/>số</b></th>" +
                                                "<th rowspan ='2' width='80px' '><b>Đơn<br/>giá</b></th>" +
                                                "<th rowspan ='2' width='80px'><b>Thành<br/>tiền</b></th>" +
                                                "<th rowspan ='2' width='80px'><b>Ghi chú</b></th>" +
                                                "</tr>";

                    returnHtml += "<tr>";
                    for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                    {
                        returnHtml += "<td class='ngayThang' align='center'>" + ngayDieuTri[i].Day + "/" + ngayDieuTri[i].Month + "</td>";
                    }
                    returnHtml += "</tr>";

                    returnHtml += "<tbody>";
                    var dsDichVuKyThuatGroup = danhSachDichVuKyThuats.GroupBy(s => new { s.TenThuocVatTu }).Select(z => new PhieuCongKhaiThuocVatTuGridVo()
                    {
                        TenThuocVatTu = z.First().TenThuocVatTu,
                        DonVi = z.First().DonVi,
                        NgayThang = z.First().NgayThang,
                        SoLuong = z.First().SoLuong,
                        TongSo = z.Sum(x => x.TongSo),
                        DonGia = z.First().DonGia,
                        ThanhTien = z.Sum(x => x.ThanhTien),
                        GhiChu = z.First().GhiChu,
                        LoaiThuocTheoQuanLy = z.First().LoaiThuocTheoQuanLy,
                        DuocPhamBenhVienPhanNhomId = z.First().DuocPhamBenhVienPhanNhomId,
                        HoatChat = z.First().HoatChat,
                        HamLuong = z.First().HamLuong,
                        NgayThangSlThuocVatTus = z.Select(bb => new ThuocVatTuNgayThang() { SoLuong = bb.TongSo, NgayThang = bb.NgayThang })
                                      .GroupBy(c => c.NgayThang)
                                      .Select(bb => new ThuocVatTuNgayThang() { SoLuong = bb.Sum(c => c.SoLuong), NgayThang = bb.First().NgayThang }).ToList(),
                        LaDuocPham = z.First().LaDuocPham
                    }).ToList();
                    var stt = 1;

                    foreach (var item in dsDichVuKyThuatGroup)
                    {
                        returnHtml += "<tr>"
                                    + "<td  align='center'>" + stt + "</td>"
                                    + "<td class='TenThuocVatTu'>" + (item.LaDuocPham == true ? _yeuCauKhamBenhService.FormatTenDuocPham(item.TenThuocVatTu, item.HoatChat, item.HamLuong, item.DuocPhamBenhVienPhanNhomId) : item.TenThuocVatTu) + "</td>"
                                    + "<td class='donVi' align='center'>" + item.DonVi + "</td>";

                        for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                        {
                            if (item.NgayThangSlThuocVatTus.Where(c => c.NgayThang == ngayDieuTri[i]).Any())
                            {
                                returnHtml += "<td   class='ngayThang' align='center' width='40px'>" + (item.LaDuocPham == true ? _yeuCauKhamBenhService.FormatSoLuong(item.NgayThangSlThuocVatTus.Where(c => c.NgayThang == ngayDieuTri[i]).Sum(c => c.SoLuong), item.LoaiThuocTheoQuanLy) : item.NgayThangSlThuocVatTus.Where(c => c.NgayThang == ngayDieuTri[i]).Sum(c => c.SoLuong).ToString()) + "</td>";
                            }
                            else
                            {
                                returnHtml += "<td  class='ngayThang' align='center'>" + "</td>";
                            }
                        }

                        returnHtml += "<td class='tongSo' align='center'>" + item.TongSo.ApplyNumber() + "</td>"
                                  + "<td class='donGia'  align='right'>" + ((double)item.DonGia).ApplyNumber() + "</td>"
                                  + "<td class='thanhTien' align='right'>" + ((double)item.ThanhTien).ApplyNumber() + "</td>"
                                  + "<td align='center'>" + "</td>"
                                  + "</tr>";
                        stt++;
                    }
                                     
                    returnHtml += "<tr>"
                             + "<td colspan='2' align='center'>" + "TỔNG CỘNG" + "</td>"
                             + "<td align='center'>" + "</td>";

                    for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                    {
                        var data = dsDichVuKyThuatGroup.Select(c => c.NgayThangSlThuocVatTus.Where(cc => cc.NgayThang == ngayDieuTri[i]).Sum(ccc => ccc.SoLuong));
                        var count = data.Where(c => c != 0).Count() != 0 ? data.Where(c => c != 0).Count() : (int?)null;

                        returnHtml += "<td align='center' class='text_right'>" + count + "</td>";
                    }

                    if (!ngayDieuTri.Any())
                    {
                        returnHtml += "<td  align='center'></td>";
                    }

                    returnHtml += "<td align='center' class='text_right'></td>"
                               + "<td align='center' class='text_right'>" + "</td>"
                               + "<td align='center' class='text_right'><b>" + ((double)dsDichVuKyThuatGroup.Sum(o => o.ThanhTien)).ApplyNumber() + "</b></td>"
                               + "<td align='center' class='text_right'>" + "</td>"
                               + "</tr>";

                    returnHtml += "<tr>"
                                  + "<td colspan='2' align='center'>" + "Ký xác nhận người bệnh <span class='square2'></span>  người nhà <span class='square2'> </span>" + " </td>"
                                  + "<td align='center'>" + "</td>";
                    for (int i = 0; i <= column - 1; i++)
                    {
                        returnHtml += "<td  class='text_right'></td>";
                    }

                    returnHtml += "<td align='center' class='text_right'>" + "</td>"
                                  + "<td align='center' class='text_right'>" + "</td>"
                                  + "<td align='center' class='text_right'>" + "</td>"
                                  + "<td align='center' class='text_right'>" + "</td>"
                                  + "</tr>";
                    returnHtml += "</tbody></table>";
                    returnHtml += "<br/>";

                    returnHtmlAll.Add(returnHtml);

                }
            }
            else
            {
                #region Yêu cầu dịch vụ kỹ thuật được bảo hiểm chi trả

                var danhSachDichVuKyThuats = new List<PhieuCongKhaiThuocVatTuGridVo>();

                danhSachDichVuKyThuats.AddRange(_yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.BaoHiemChiTra == true &&
                                p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                                                    .Select(s => new PhieuCongKhaiThuocVatTuGridVo
                                                    {
                                                        TenThuocVatTu = s.Ten,
                                                        DonVi = s.DonViTinh.Ten,
                                                        SoLuong = s.SoLuong,
                                                        DonGia = (decimal)s.DonGiaBaoHiem,
                                                        ThanhTien = (decimal)(s.SoLuong * (double)(s.DonGiaBaoHiem.GetValueOrDefault() * s.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * s.MucHuongBaoHiem.GetValueOrDefault() / 100)),
                                                        GhiChu = s.GhiChu,
                                                        NgayThang = s.NoiTruPhieuDieuTri.NgayDieuTri,
                                                        LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                                        DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                                        HoatChat = s.HoatChat,
                                                        HamLuong = s.HamLuong,
                                                        LaDuocPham = true // mặc định
                                                    })
                                                    .GroupBy(x => new { x.TenThuocVatTu, x.NgayThang })
                                                    .Select(item => new PhieuCongKhaiThuocVatTuGridVo
                                                    {
                                                        TenThuocVatTu = item.First().TenThuocVatTu,
                                                        DonVi = item.First().DonVi,
                                                        SoLuong = item.First().SoLuong,
                                                        TongSo = item.Sum(x => x.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                        ThanhTien = item.Sum(x => x.ThanhTien),
                                                        GhiChu = item.First().GhiChu,
                                                        NgayThang = item.First().NgayThang,
                                                        LoaiThuocTheoQuanLy = item.First().LoaiThuocTheoQuanLy,
                                                        DuocPhamBenhVienPhanNhomId = item.First().DuocPhamBenhVienPhanNhomId,
                                                        HoatChat = item.First().HoatChat,
                                                        HamLuong = item.First().HamLuong,
                                                        LaDuocPham = item.First().LaDuocPham
                                                    }).Distinct().ToList());

                danhSachDichVuKyThuats.AddRange(_yeuCauVatTuBenhVienRepository.TableNoTracking
                                                    .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.BaoHiemChiTra == true &&
                                                                p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                                                    .Select(s => new PhieuCongKhaiThuocVatTuGridVo
                                                    {
                                                        TenThuocVatTu = s.Ten,
                                                        DonVi = s.DonViTinh,
                                                        SoLuong = s.SoLuong,
                                                        DonGia = (decimal)s.DonGiaBaoHiem,
                                                        ThanhTien = (decimal)(s.SoLuong * (double)(s.DonGiaBaoHiem.GetValueOrDefault() * s.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * s.MucHuongBaoHiem.GetValueOrDefault() / 100)),
                                                        GhiChu = s.GhiChu,
                                                        NgayThang = s.NoiTruPhieuDieuTri.NgayDieuTri,
                                                    })
                                                    .GroupBy(x => new { x.TenThuocVatTu, x.DonVi, x.DonGia, x.NgayThang, x.SoLuong })
                                                    .Select(item => new PhieuCongKhaiThuocVatTuGridVo
                                                    {
                                                        TenThuocVatTu = item.First().TenThuocVatTu,
                                                        DonVi = item.First().DonVi,
                                                        SoLuong = item.First().SoLuong,
                                                        TongSo = item.Sum(x => x.SoLuong),
                                                        DonGia = item.First().DonGia,
                                                        ThanhTien = item.Sum(x => x.ThanhTien),
                                                        GhiChu = item.First().GhiChu,
                                                        NgayThang = item.First().NgayThang,
                                                    }).Distinct().ToList());

                #endregion


                var ngayDieuTri = new List<DateTime>();
                var returnHtml = string.Empty;

                if (tatCaNDT != null && tatCaNDT.Any())
                {
                    foreach (var item in tatCaNDT)
                    {
                        ngayDieuTri.Add(item);
                    }
                }
                var column = ngayDieuTri.Any() ? ngayDieuTri.Count() : 1;



                returnHtml += "<table id='dichvu' style='border-collapse: collapse;width:100%'> " +
                            "<tr>" +
                            "<th  rowspan ='2' width='30px'><b>TT</b> </th>" +
                            "<th rowspan ='2'><b>TÊN DỊCH VỤ KHÁM BỆNH ,CHỮA BỆNH</b></th>" +
                            "<th  rowspan = '2'width='50px'><b> ĐVT </b> </th >" +
                            "<th colspan='" + column + "' style='min-width:180px'><b>Đợt điều trị từ ngày " + ngayDieuTriBatDau + " đến " + ngayDieuTriKetThuc + "</b></th>" +
                            "<th rowspan ='2' width='40px'><b>Tổng<br/>số</b></th>" +
                            "<th rowspan ='2' width='80px' '><b>Đơn<br/>giá</b></th>" +
                            "<th rowspan ='2' width='80px'><b>Thành<br/>tiền</b></th>" +
                            "<th rowspan ='2' width='80px'><b>Ghi chú</b></th>" +
                            "</tr>";

                returnHtml += "<tr>";
                for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                {
                    returnHtml += "<td class='ngayThang' align='center' width='40px'>" + ngayDieuTri[i].Day + "/" + ngayDieuTri[i].Month + "</td>";
                }
                returnHtml += "</tr>";

                returnHtml += "<tbody>";
                var dsDichVuKyThuatGroup = danhSachDichVuKyThuats.GroupBy(s => new { s.TenThuocVatTu }).Select(z => new PhieuCongKhaiThuocVatTuGridVo()
                {
                    TenThuocVatTu = z.First().TenThuocVatTu,
                    DonVi = z.First().DonVi,
                    NgayThang = z.First().NgayThang,
                    SoLuong = z.First().SoLuong,
                    TongSo = z.Sum(x => x.TongSo),
                    DonGia = z.First().DonGia,
                    ThanhTien = z.Sum(x => x.ThanhTien),
                    GhiChu = z.First().GhiChu,
                    LoaiThuocTheoQuanLy = z.First().LoaiThuocTheoQuanLy,
                    DuocPhamBenhVienPhanNhomId = z.First().DuocPhamBenhVienPhanNhomId,
                    HoatChat = z.First().HoatChat,
                    HamLuong = z.First().HamLuong,
                    NgayThangSlThuocVatTus = z.Select(bb => new ThuocVatTuNgayThang() { SoLuong = bb.TongSo, NgayThang = bb.NgayThang })
                                  .GroupBy(c => c.NgayThang)
                                  .Select(bb => new ThuocVatTuNgayThang() { SoLuong = bb.Sum(c => c.SoLuong), NgayThang = bb.First().NgayThang }).ToList(),
                    LaDuocPham = z.First().LaDuocPham
                }).ToList();
                var stt = 1;

                foreach (var item in dsDichVuKyThuatGroup)
                {
                    returnHtml += "<tr>"
                                + "<td  align='center'>" + stt + "</td>"
                                + "<td class='TenThuocVatTu'>" + (item.LaDuocPham == true ? _yeuCauKhamBenhService.FormatTenDuocPham(item.TenThuocVatTu, item.HoatChat, item.HamLuong, item.DuocPhamBenhVienPhanNhomId) : item.TenThuocVatTu) + "</td>"
                                + "<td class='donVi' align='center'>" + item.DonVi + "</td>";

                    for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                    {
                        if (item.NgayThangSlThuocVatTus.Where(c => c.NgayThang == ngayDieuTri[i]).Any())
                        {
                            returnHtml += "<td   class='ngayThang' align='center'>" + (item.LaDuocPham == true ? _yeuCauKhamBenhService.FormatSoLuong(item.NgayThangSlThuocVatTus.Where(c => c.NgayThang == ngayDieuTri[i]).Sum(c => c.SoLuong), item.LoaiThuocTheoQuanLy) : item.NgayThangSlThuocVatTus.Where(c => c.NgayThang == ngayDieuTri[i]).Sum(c => c.SoLuong).ToString()) + "</td>";
                        }
                        else
                        {
                            returnHtml += "<td  class='ngayThang' align='center'>" + "</td>";
                        }
                    }

                    returnHtml += "<td class='tongSo'align='center'>" + item.TongSo.ApplyNumber() + "</td>"
                              + "<td class='donGia' align='right'>" + ((double)item.DonGia).ApplyNumber() + "</td>"
                              + "<td class='thanhTien'align='right'>" + ((double)item.ThanhTien).ApplyNumber() + "</td>"
                              + "<td align='center'>" + "</td>"
                              + "</tr>";
                    stt++;
                }
                              
                returnHtml += "<tr>"
                         + "<td colspan='2'align='center'>" + "TỔNG CỘNG" + "</td>"
                         + "<td align='center'>" + "</td>";

                for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                {
                    var data = dsDichVuKyThuatGroup.Select(c => c.NgayThangSlThuocVatTus.Where(cc => cc.NgayThang == ngayDieuTri[i]).Sum(ccc => ccc.SoLuong));
                    var count = data.Where(c => c != 0).Count() != 0 ? data.Where(c => c != 0).Count() : (int?)null;

                    returnHtml += "<td align='center'>" + count + "</td>";
                }

                if (!ngayDieuTri.Any())
                {
                    returnHtml += "<td align='center'></td>";
                }

                returnHtml += "<td align='center' class='text_right'></td>"
                           + "<td align='center' class='text_right'>" + "</td>"
                           + "<td align='center' class='text_right'><b>" + ((double)dsDichVuKyThuatGroup.Sum(o => o.ThanhTien)).ApplyNumber() + "</b></td>"
                           + "<td align='center' class='text_right'>" + "</td>"
                           + "</tr>";

                returnHtml += "<tr>"
                              + "<td colspan='2' align='center'>" + "Ký xác nhận người bệnh <span class='square2'></span>  người nhà <span class='square2'> </span>" + " </td>"
                              + "<td align='center'>" + "</td>";
                for (int i = 0; i <= column - 1; i++)
                {
                    returnHtml += "<td  align='center'></td>";
                }

                returnHtml += "<td align='center' class='text_right'>" + "</td>"
                              + "<td align='center' class='text_right'>" + "</td>"
                              + "<td align='center' class='text_right'>" + "</td>"
                              + "<td align='center'>" + "</td>"
                              + "</tr>";
                returnHtml += "</tbody></table>";
                returnHtml += "<br/>";

                returnHtmlAll.Add(returnHtml);

            }
            return returnHtmlAll;
        }

        public List<string> GetDataPhieuCongKhaiThuocVatTu(PhieuCongKhaiThuocVatTu phieuCongKhaiThuocVatTu)
        {
            return GetHtmlTableCongKhaiThuocVatTuTheoRangeDate(phieuCongKhaiThuocVatTu.YeuCauTiepNhanId);
        }

        public PhieuCongKhaiThuocVatTuObject GetThongTinPhieuCongKhaiThuocVatTu(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiThuocVatTu)
                                                                  .Select(s => new PhieuCongKhaiThuocVatTuObject()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiThuocVatTu,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuCongKhaiThuocVatTuThuatGridVo()
                                                                      {
                                                                          Id = k.Id,
                                                                          DuongDan = k.DuongDan,
                                                                          KichThuoc = k.KichThuoc,
                                                                          LoaiTapTin = k.LoaiTapTin,
                                                                          Ma = k.Ma,
                                                                          MoTa = k.MoTa,
                                                                          Ten = k.Ten,
                                                                          TenGuid = k.TenGuid
                                                                      }).ToList()
                                                                  }).FirstOrDefault();
            return query;
        }

        public async Task<string> InPhieuCongKhaiThuocVatTu(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var noiTruBenhAn = await _noiTruBenhAnRepository.TableNoTracking.Where(x =>
                                                              x.Id == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId).Include(c => c.YeuCauTiepNhan)
                                                              .Include(c => c.NoiTruPhieuDieuTris).ThenInclude(c => c.ChanDoanChinhICD).FirstOrDefaultAsync();
            var content = "";

            var phieuCongKhaiThuocVatTu = new PhieuCongKhaiThuocVatTu();
            phieuCongKhaiThuocVatTu.YeuCauTiepNhanId= xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId;
            var htmlPhieuCongKhaiThuocVatTu = GetDataPhieuCongKhaiThuocVatTu(phieuCongKhaiThuocVatTu);

            var thongtinIn = await _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                      .Select(x => x.ThongTinHoSo).FirstOrDefaultAsync();

            var queryString = JsonConvert.DeserializeObject<InPhieuCongKhaiThuocVatTu>(thongtinIn);
            var dataThongTinNguoibenh = ThongTinChungBenhNhanPhieuDieuTri(xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId);

            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuCongKhaiThuocVatTuYTe"));



            DateTime tuNgayPart = DateTime.Now; ;
            tuNgayPart = noiTruBenhAn.ThoiDiemNhapVien;
            DateTime denNgayPart = DateTime.Now; ;
            denNgayPart = (noiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now);


            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId)
                                                          .Include(c => c.BenhNhan)
                                                          .Include(c => c.NoiTruBenhAn).ThenInclude(c => c.KhoaPhongNhapVien).ThenInclude(c => c.YeuCauKhamBenhs).ThenInclude(c => c.Icdchinh)
                                                         .Include(c => c.NoiTruBenhAn).ThenInclude(c => c.KhoaPhongNhapVien).ThenInclude(c => c.YeuCauKhamBenhs).ThenInclude(c => c.ChanDoanSoBoICD)
                                                          .Include(c => c.YeuCauTiepNhanTheBHYTs).FirstOrDefault();

            //var theBHYTs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
            //           ? yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
            //               .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc) : null;

            var bhytMaDKBD = _benhVienRepository.TableNoTracking.Where(a => a.Ma == yeuCauTiepNhan.BenhNhan.BHYTMaDKBD).FirstOrDefault();
            var soTheBHYT = yeuCauTiepNhan != null ? yeuCauTiepNhan.BHYTMaSoThe + "-" + bhytMaDKBD?.Ma : string.Empty;

            var hanThe = yeuCauTiepNhan != null ?
              " từ " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") +
              " đến " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : string.Empty;

            var SoNgayDieuTri = NoiTruBenhAnHelper.TinhSoNgayDieuTri(noiTruBenhAn);


            var chanDoanChinhs = new List<string>();
            var tatCaChuanDoan = string.Empty;

            if (noiTruBenhAn.ThongTinRaVien != null)
            {
                var thongTinRaVien = JsonConvert.DeserializeObject<RaVien>(noiTruBenhAn.ThongTinRaVien);

                if (thongTinRaVien != null)
                {
                    if (!string.IsNullOrEmpty(thongTinRaVien.TenChuanDoanRaVien))
                    {
                        chanDoanChinhs.Add(thongTinRaVien.TenChuanDoanRaVien);

                    }
                    if (thongTinRaVien.ChuanDoanKemTheos != null && thongTinRaVien.ChuanDoanKemTheos.Any())
                    {
                        if (noiTruBenhAn != null)
                        {
                            foreach (var item in thongTinRaVien.ChuanDoanKemTheos.Where(c => c.TenICD != null))
                            {
                                chanDoanChinhs.Add(item.TenICD);
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(queryString.ChanDoan))
                {
                    chanDoanChinhs.Add(queryString.ChanDoan);
                }
            }


            if (chanDoanChinhs != null && chanDoanChinhs.Any())
            {
                tatCaChuanDoan = chanDoanChinhs.Where(s => s != null).Distinct().ToList().Join(" | ");
            }

            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                             .Select(x => new
                                                             {
                                                                 So = dataThongTinNguoibenh.MaTN,
                                                                 LogoUrl = xacNhanInTrichBienBanHoiChan.Hosting + "/assets/img/logo-bacha-full.png",

                                                                 SoTheBHYT = soTheBHYT,
                                                                 HanThe = hanThe,
                                                                 SoNgayDieuTri,

                                                                 dataThongTinNguoibenh.HoTenNguoiBenh,
                                                                 dataThongTinNguoibenh.Tuoi,
                                                                 dataThongTinNguoibenh.Gioi,
                                                                 NgayVaoVien = tuNgayPart.ApplyFormatDate(),
                                                                 NgayRaVien = denNgayPart.ApplyFormatDate(),
                                                                 dataThongTinNguoibenh.CoBHYT,
                                                                 dataThongTinNguoibenh.SoGiuong,
                                                                 dataThongTinNguoibenh.SoBA,
                                                                 Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(p => p.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                 dataThongTinNguoibenh.MaTheBHYT,
                                                                 Khoa = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderByDescending(p => p.ThoiDiemVaoKhoa).First().KhoaPhongChuyenDen.Ten,
                                                                 TableString = htmlPhieuCongKhaiThuocVatTu != null ? htmlPhieuCongKhaiThuocVatTu.Select(s => s).ToList().Join("") : "",
                                                                 ChanDOan = tatCaChuanDoan,
                                                             }).FirstOrDefault();

            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
    }
}
