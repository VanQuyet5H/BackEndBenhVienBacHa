using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiDichVuKyThuat;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiDichVuKyThuat;
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
        private List<string> GetHtmlTableCongKhaiDichVuKyThuatTheoRangeDate(long yeuCauTiepNhanId)
        {
            List<string> returnHtmlAll = new List<string>();
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(x =>
                                                          x.Id == yeuCauTiepNhanId)
                                                          .Include(c => c.NoiTruPhieuDieuTris).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                          .Include(c => c.NoiTruPhieuDieuTris)
                                                          .ThenInclude(c => c.ChanDoanChinhICD).FirstOrDefault();

            //lấy yêu cầu tiếp nhận ngoại trú
            var yeuCauTiepNhanNgoaiTruCanQuyetToanId = _yeuCauTiepNhanRepository.TableNoTracking.Where(c => c.Id == yeuCauTiepNhanId).Select(c => c.YeuCauTiepNhanNgoaiTruCanQuyetToanId).FirstOrDefault();
            var yeuCauTiepNhanNgoaiTru = _yeuCauTiepNhanRepository.TableNoTracking.Where(c => c.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId).FirstOrDefault();


            var ngayDieuTriBatDau = yeuCauTiepNhanNgoaiTru.ThoiDiemTiepNhan.ToString("dd/MM/yyyy");
            var ngayDieuTriKetThuc = noiTruBenhAn.ThoiDiemRaVien?.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy");



            var tatCaNDT = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                                               .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId 
                                                               || p.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                                                               && p.BaoHiemChiTra == true
                                                               && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                              .OrderBy(c => c.ThoiDiemDangKy.Date).Select(c => c.ThoiDiemDangKy.Date).Distinct();

            if (tatCaNDT.Count() > 13)
            {
                var tachPhieus = tatCaNDT.ToArray();

                for (int ii = 0; ii < tachPhieus.Length; ii = ii + 13)
                {
                    var start = tachPhieus[ii];
                    var end = tachPhieus.Length - ii > 13 ? tachPhieus[ii + 12] : tachPhieus[tachPhieus.Length - 1];

                    #region Yêu cầu dịch vụ kỹ thuật được bảo hiểm chi trả

                    var danhSachDichVuKyThuats = new List<PhieuCongKhaiDichVuKyThuatGridVo>();

                    danhSachDichVuKyThuats.AddRange(_yeuCauDichVuKyThuatRepository.TableNoTracking
                                                               .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                                                               && p.BaoHiemChiTra == true && p.ThoiDiemDangKy >= start && p.ThoiDiemDangKy <= end
                                                               && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                              .Select(s => new PhieuCongKhaiDichVuKyThuatGridVo
                                                              {
                                                                  TenDichVuKyThuat = s.TenDichVu,
                                                                  DonVi = "Lần",
                                                                  SoLuong = s.SoLan,
                                                                  DonGia = (decimal)s.DonGiaBaoHiem,
                                                                  ThanhTien = s.DonGiaBaoHiem.GetValueOrDefault() * s.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * s.MucHuongBaoHiem.GetValueOrDefault() / 100 * s.SoLan,
                                                                  NgayThang = s.ThoiDiemDangKy.Date,
                                                              })
                                                              .GroupBy(x => new { x.TenDichVuKyThuat, x.NgayThang })
                                                              .Select(item => new PhieuCongKhaiDichVuKyThuatGridVo
                                                              {
                                                                  TenDichVuKyThuat = item.First().TenDichVuKyThuat,
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
                        returnHtml += "<td class='ngayThang' align='center' width='40px'>" + ngayDieuTri[i].Day + "/" + ngayDieuTri[i].Month + "</td>";
                    }
                    returnHtml += "</tr>";

                    returnHtml += "<tbody>";

                    var dsDichVuKyThuatGroup = danhSachDichVuKyThuats.GroupBy(s => new { s.TenDichVuKyThuat }).Select(z => new PhieuCongKhaiDichVuKyThuatGridVo()
                    {
                        TenDichVuKyThuat = z.First().TenDichVuKyThuat,
                        DonVi = z.First().DonVi,
                        NgayThang = z.First().NgayThang,
                        SoLuong = z.First().SoLuong,
                        TongSo = z.Sum(x => x.TongSo),
                        DonGia = z.First().DonGia,
                        ThanhTien = z.Sum(x => x.ThanhTien),
                        GhiChu = z.First().GhiChu,
                        NgayThangSlDichVuKyThuats = z.Select(s => new DichVuKyThuatNgayThang() { SoLuong = s.TongSo, NgayThang = s.NgayThang })
                                  .GroupBy(c => c.NgayThang)
                                  .Select(bb => new DichVuKyThuatNgayThang() { SoLuong = bb.Sum(c => c.SoLuong), NgayThang = bb.First().NgayThang }).ToList()
                    }).ToList();

                    var stt = 1;

                    foreach (var item in dsDichVuKyThuatGroup)
                    {
                        returnHtml += "<tr>"
                                    + "<td align='center'>" + stt + "</td>"
                                    + "<td class='TenDichVuKyThuat'>" + item.TenDichVuKyThuat + "</td>"
                                    + "<td class='donVi' align='center'>" + item.DonVi + "</td>";

                        for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                        {
                            if (item.NgayThangSlDichVuKyThuats.Where(c => c.NgayThang.Date == ngayDieuTri[i].Date).Any())
                            {
                                returnHtml += "<td   class='ngayThang' align='center'>" + item.NgayThangSlDichVuKyThuats.Where(c => c.NgayThang.Date == ngayDieuTri[i].Date).Sum(c => c.SoLuong) + "</td>";
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
                        var data = dsDichVuKyThuatGroup.Select(c => c.NgayThangSlDichVuKyThuats.Where(cc => cc.NgayThang.Date == ngayDieuTri[i].Date).Sum(ccc => ccc.SoLuong));
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
                        returnHtml += "<td  align='center' class='text_right'></td>";
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

                var danhSachDichVuKyThuats = new List<PhieuCongKhaiDichVuKyThuatGridVo>();

                danhSachDichVuKyThuats.AddRange(_yeuCauDichVuKyThuatRepository.TableNoTracking
                                                        .Where(p => (p.YeuCauTiepNhanId == yeuCauTiepNhanId || p.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                                                              && p.BaoHiemChiTra == true
                                                              && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                             .Select(s => new PhieuCongKhaiDichVuKyThuatGridVo
                                                             {
                                                                 TenDichVuKyThuat = s.TenDichVu,
                                                                 DonVi = "Lần",
                                                                 SoLuong = s.SoLan,
                                                                 DonGia = (decimal)s.DonGiaBaoHiem,
                                                                 ThanhTien = s.DonGiaBaoHiem.GetValueOrDefault() * s.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * s.MucHuongBaoHiem.GetValueOrDefault() / 100 * s.SoLan,
                                                                 NgayThang = s.ThoiDiemDangKy.Date,
                                                             })
                                                             .GroupBy(x => new { x.TenDichVuKyThuat, x.NgayThang })
                                                             .Select(item => new PhieuCongKhaiDichVuKyThuatGridVo
                                                             {
                                                                 TenDichVuKyThuat = item.First().TenDichVuKyThuat,
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

                var dsDichVuKyThuatGroup = danhSachDichVuKyThuats.GroupBy(s => new { s.TenDichVuKyThuat }).Select(z => new PhieuCongKhaiDichVuKyThuatGridVo()
                {
                    TenDichVuKyThuat = z.First().TenDichVuKyThuat,
                    DonVi = z.First().DonVi,
                    NgayThang = z.First().NgayThang,
                    SoLuong = z.First().SoLuong,
                    TongSo = z.Sum(x => x.TongSo),
                    DonGia = z.First().DonGia,
                    ThanhTien = z.Sum(x => x.ThanhTien),
                    GhiChu = z.First().GhiChu,
                    NgayThangSlDichVuKyThuats = z.Select(s => new DichVuKyThuatNgayThang() { SoLuong = s.TongSo, NgayThang = s.NgayThang })
                                 .GroupBy(c => c.NgayThang)
                                 .Select(bb => new DichVuKyThuatNgayThang() { SoLuong = bb.Sum(c => c.SoLuong), NgayThang = bb.First().NgayThang }).ToList()
                }).ToList();

                var stt = 1;

                foreach (var item in dsDichVuKyThuatGroup)
                {
                    returnHtml += "<tr>"
                                + "<td  align='center'>" + stt + "</td>"
                                  + "<td class='TenDichVuKyThuat'>" + item.TenDichVuKyThuat + "</td>"
                                + "<td class='donVi'align='center'>" + item.DonVi + "</td>";

                    for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                    {
                        if (item.NgayThangSlDichVuKyThuats.Where(c => c.NgayThang.Date == ngayDieuTri[i].Date).Any())
                        {
                            returnHtml += "<td   class='ngayThang' align='center'>" + item.NgayThangSlDichVuKyThuats.Where(c => c.NgayThang.Date == ngayDieuTri[i].Date).Sum(c => c.SoLuong) + "</td>";
                        }
                        else
                        {
                            returnHtml += "<td  class='ngayThang' align='center'>" + "</td>";
                        }
                    }

                    returnHtml += "<td class='tongSo' align='center'>" + item.TongSo.ApplyNumber() + "</td>"
                              + "<td class='donGia'  align='right'>" + ((double)item.DonGia).ApplyNumber() + "</td>"
                              + "<td class='thanhTien' align='right'>" + ((double)item.ThanhTien).ApplyNumber() + "</td>"
                              + "<td>" + "</td>"
                              + "</tr>";
                    stt++;
                }


                returnHtml += "<tr>"
                         + "<td colspan='2' align='center'>" + "TỔNG CỘNG" + "</td>"
                         + "<td>" + "</td>";

                for (int i = 0; i <= ngayDieuTri.Count() - 1; i++)
                {
                    var data = dsDichVuKyThuatGroup.Select(c => c.NgayThangSlDichVuKyThuats.Where(cc => cc.NgayThang.Date == ngayDieuTri[i].Date).Sum(ccc => ccc.SoLuong));
                    var count = data.Where(c => c != 0).Count() != 0 ? data.Where(c => c != 0).Count() : (int?)null;

                    returnHtml += "<td align='center' class='text_right'>" + count + "</td>";
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
                    returnHtml += "<td  align='center' class='text_right'></td>";
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
            return returnHtmlAll;
        }

        public List<string> GetDataPhieuCongKhaiDichVuKyThuat(PhieuCongKhaiDichVuKyThuat phieuCongKhaiDichVuKyThuat)
        {
            return GetHtmlTableCongKhaiDichVuKyThuatTheoRangeDate(phieuCongKhaiDichVuKyThuat.YeuCauTiepNhanId);
        }

        public PhieuCongKhaiDichVuKyThuatObject GetThongTinPhieuCongKhaiDichVuKyThuat(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiDichVuKyThuat)
                                                                  .Select(s => new PhieuCongKhaiDichVuKyThuatObject()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiDichVuKyThuat,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuCongKhaiDichVuKyThuatGridVo()
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

        public async Task<string> InPhieuCongKhaiDichVuKyThuat(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {

            var noiTruBenhAn = await _noiTruBenhAnRepository.TableNoTracking.Where(x =>
                                                            x.Id == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId).Include(c => c.YeuCauTiepNhan)
                                                            .Include(c => c.NoiTruPhieuDieuTris).ThenInclude(c => c.ChanDoanChinhICD).FirstOrDefaultAsync();
            var content = "";
            var thongtinIn = await _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                      .Select(x => x.ThongTinHoSo).FirstOrDefaultAsync();

            var phieuKT = new PhieuCongKhaiDichVuKyThuat();
            phieuKT.YeuCauTiepNhanId = xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId;
            var htmlPhieuCongKhaiThuocVatTu = GetDataPhieuCongKhaiDichVuKyThuat(phieuKT);

            var queryString = JsonConvert.DeserializeObject<InPhieuCongKhaiDichVuKyThuat>(thongtinIn);
            var dataThongTinNguoibenh = ThongTinChungBenhNhanPhieuDieuTri(xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId);


            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuCongKhaiDichVuKyThuatHoSoKhac"));


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
                                                                                  LogoUrl = xacNhanInTrichBienBanHoiChan.Hosting + "/assets/img/logo-bacha-full.png",

                                                                                  SoTheBHYT = soTheBHYT,
                                                                                  HanThe = hanThe,
                                                                                  SoNgayDieuTri,
                                                                                  So = dataThongTinNguoibenh.MaTN,
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
