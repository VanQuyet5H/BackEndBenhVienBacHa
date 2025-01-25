using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiVatTu;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
      
        private string GetHtmlTableCongKhaiVatTuTheoRangeDate(long yeuCauTiepNhanId, DateTime start, DateTime end, List<PhieuCongKhaiVatTuGridVo> dsVatTuTheoKhoas)
        {
            var returnHtml = "";
            TimeSpan difference = end - start; //create TimeSpan object
            //BVHD-3876
            var dsVatTu = new List<PhieuCongKhaiVatTuGridVo>();
            if (dsVatTuTheoKhoas.Any())
            {
                dsVatTu = dsVatTuTheoKhoas.Where(p => p.NgayThang >= start && p.NgayThang <= end).ToList();
            }
            else
            {
                dsVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.NoiTruPhieuDieuTri.NgayDieuTri >= start && p.NoiTruPhieuDieuTri.NgayDieuTri <= end &&
                            p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                .OrderBy(p => p.Id)
               .Select(s => new PhieuCongKhaiVatTuGridVo
               {
                   TenVatTu = s.Ten,
                   DonVi = s.DonViTinh,
                   SoLuong = s.SoLuong,
                   DonGia = s.DonGiaBan,
                   ThanhTien = (decimal)(s.SoLuong * (double)s.DonGiaBan),
                   GhiChu = s.GhiChu,
                   NgayThang = s.NoiTruPhieuDieuTri.NgayDieuTri,
               }).ToList();
                //.GroupBy(x => new { x.TenVatTu, x.DonVi, x.DonGia })
                //.Select(item => new PhieuCongKhaiVatTuGridVo
                //{
                //    TenVatTu = item.First().TenVatTu,
                //    DonVi = item.First().DonVi,
                //    SoLuong = item.First().SoLuong,
                //    TongSo = item.Sum(x => x.SoLuong),
                //    DonGia = item.First().DonGia,
                //    ThanhTien = item.Sum(x => x.ThanhTien),
                //    GhiChu = item.First().GhiChu,
                //    NgayThang = item.First().NgayThang,
                //}).Distinct()
            }

            returnHtml += "<table style='border-collapse: collapse;width:100%'> " +
                     "<thead>" +
                     "<tr>" +
                     "<th  rowspan ='2' style='border: 1px solid black; width: 5%; padding: 0px; margin: 0px; '><b>SỐ <br>TT</b> </th>" +
                     "<th rowspan ='2'  style='border: 1px solid black; width: 17%;'><b>Tên thuốc, hàm lượng</b></th>" +
                     "<th  rowspan = '2' style ='border: 1px solid black;width: 8%;text-align:center;' ><b> Đơn <br> vị </b> </th >" +
                     "<th colspan='" + (difference.TotalDays + 1) + "' style='border: 1px solid black; width: 28%; text - align:center;'><b>Ngày,tháng</b></th>" +
                     "<th rowspan ='2' style='border: 1px solid black; width: 10%; text-align:center; '><b>Tổng số</b></th>" +
                     "<th rowspan ='2' style='border: 1px solid black; width: 10%; text-align:center; '><b>Đơn giá</b></th>" +
                     "<th rowspan ='2' style='border: 1px solid black; width: 10%; text-align:center; '><b>Thành tiền</b></th>" +
                     "<th rowspan ='2' style='border: 1px solid black; width: 12%; text-align:center; '><b>Ghi chú</b></th>" +
                     "</tr>";
            returnHtml += "<tr>";
            for (int i = 0; i <= difference.TotalDays; i++)
            {
                returnHtml += "<td class='ngayThang' style = 'border: 1px solid black;width: 4%;text-align:center;'>" + start.AddDays(i).Day + "/" + start.AddDays(i).Month + "</td>";
            }
            returnHtml += "</tr></thead>";

            returnHtml += "<tbody>";
            var dsVatTuGroup = dsVatTu.GroupBy(s => new { s.TenVatTu  ,s.DonVi})
                .Select(z => new PhieuCongKhaiVatTuGridVo()
                {
                    TenVatTu = z.First().TenVatTu,
                    DonVi = z.First().DonVi,
                    NgayThang = z.First().NgayThang,
                    SoLuong = z.First().SoLuong,
                    TongSo = z.Sum(x => x.SoLuong),
                    DonGia = z.First().DonGia,
                    ThanhTien = z.Sum(x => x.ThanhTien),
                    GhiChu = z.First().GhiChu,
                    NgayThangSlVatTus = z.Select(s => new VatTuNgayThang() { SoLuong = s.SoLuong, NgayThang = s.NgayThang }).GroupBy(c => c.NgayThang).Select(bb => new VatTuNgayThang() { SoLuong = bb.Sum(s=>s.SoLuong), NgayThang = bb.First().NgayThang }).ToList()
                }).ToList();
            var stt = 1;
            foreach (var item in dsVatTuGroup)
            {
                returnHtml += "<tr>"
                                                        + "<td  style = 'border: 1px solid black; width: 5%; padding: 0px; margin: 0px;text-align:center'>" + stt + "</td>"
                                                        + "<td class='tenVatTu' style = 'border: 1px solid black; width: 17%;padding:5px;'>" + item.TenVatTu + "</td>"
                                                        + "<td class='donVi' style ='border: 1px solid black;width: 8%;text-align:center;'>" + item.DonVi + "</td>";
                for (int i = 0; i <= difference.TotalDays; i++)
                {
                    if (item.NgayThangSlVatTus.Any(x => x.NgayThang == start.AddDays(i)))
                    {
                        returnHtml += "<td   class='ngayThang' style = 'border: 1px solid black;width: 4%;text-align:center;'>" + item.NgayThangSlVatTus.Where(x => x.NgayThang == start.AddDays(i)).Select(s => s.SoLuong).FirstOrDefault() + "</td>";
                    }
                    else
                    {
                        returnHtml += "<td  class='ngayThang' style = 'border: 1px solid black;width: 4%;text-align:center;'>" + "</td>";
                    }
                }

                returnHtml += "<td class ='tongSo' style = 'border: 1px solid black;width: 10%;text-align:center;'>" + item.TongSo + "</td>"
                          + "<td class ='donGia'  style='border: 1px solid black;width: 10%;text-align:center;'>" + "</td>"
                          + "<td class ='thanhTien' style ='border: 1px solid black;width: 10%;text-align:center;'>" + "</td>"
                          + "<td style ='border: 1px solid black;width: 12%;text-align:center;word-break: break-word;'>" + "</td>"
                          + "</tr>";
                stt++;
            }
            if (!dsVatTuGroup.Any() || (dsVatTuGroup.Any() && dsVatTuGroup.Count < 10))
            {
                for (int j = 0; j < (dsVatTuGroup.Any() ? 10 - dsVatTuGroup.Count : 10); j++)
                {
                    returnHtml += "<tr>"
                                  + "<td style=' border: 1px solid black; width: 5%; padding: 0px; margin: 0px;text-align:center '>" + stt + " </td>"
                                  + "<td style ='border: 1px solid black;text-align:center;'>" + "</td>"
                    + "<td style ='border: 1px solid black;width: 8%;text-align:center;'>" + "</td>";
                    for (int i = 0; i <= difference.TotalDays; i++)
                    {
                        returnHtml += "<td  style = 'border: 1px solid black;width: 4%;text-align:center;'></td>";
                    }

                    returnHtml += "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                                  + "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                                  + "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                                  + "<td style='border: 1px solid black; width: 12%; text-align:right;'>" + "</td>"
                                  + "</tr>";
                    stt++;
                }
            }

            var tongSoKhanThuocDung = dsVatTuGroup.Count();
            returnHtml += "<tr>"
                       + "<td colspan='2' style=' border: 1px solid black; width: 22%; padding: 0px; margin: 0px;padding:5px; '>" + "Tổng số khoản thuốc dùng: " + tongSoKhanThuocDung + "</td>"
                       + "<td style ='border: 1px solid black;width: 8%;text-align:center;'>" + "</td>";
            for (int i = 0; i <= difference.TotalDays; i++)
            {
                returnHtml += "<td  style = 'border: 1px solid black;width: 4%;text-align:center;'>" + dsVatTu.Where(o => o.NgayThang == start.AddDays(i)).Sum(o => o.SoLuong) + "</td>";
            }

            returnHtml += "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                       + "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                       + "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                       + "<td style='border: 1px solid black; width: 12%; text-align:right;'>" + "</td>"
                       + "</tr>";

            returnHtml += "<tr>"
                          + "<td colspan='2' style=' border: 1px solid black; width: 22%; padding: 0px; margin: 0px;padding:5px; '>" + "Người bệnh/</br>Người nhà kí tên" + " </td>"
                          + "<td style ='border: 1px solid black;width: 8%;text-align:center;'>" + "</td>";
            for (int i = 0; i <= difference.TotalDays; i++)
            {
                returnHtml += "<td  style = 'border: 1px solid black;width: 4%;text-align:center;'></td>";
            }

            returnHtml += "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                          + "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                          + "<td style='border: 1px solid black; width: 10%; text-align:right;'>" + "</td>"
                          + "<td style='border: 1px solid black; width: 12%; text-align:right;'>" + "</td>"
                          + "</tr>";
            returnHtml += "</tbody></table>";
            returnHtml += "<div style='break-after:page'></div><br/>";
            return returnHtml;
        }

        public List<string> GetDataPhieuCongKhaiVatTu(PhieuCongKhaiVatTu phieuCongKhaiVatTu)
        {
            DateTime start = new DateTime(phieuCongKhaiVatTu.NgayVaoVien.Year, phieuCongKhaiVatTu.NgayVaoVien.Month, phieuCongKhaiVatTu.NgayVaoVien.Day);
            DateTime end = new DateTime(phieuCongKhaiVatTu.NgayRaVien.Year, phieuCongKhaiVatTu.NgayRaVien.Month, phieuCongKhaiVatTu.NgayRaVien.Day);

            List<string> returnHtml = new List<string>();
            TimeSpan difference = end - start; //create TimeSpan object
            //Tách ra mỗi tờ gồm 7 ngày
            if (difference.TotalDays > 7)
            {

                for (int i = 0; i < difference.TotalDays; i = i + 7)
                {
                    returnHtml.Add(GetHtmlTableCongKhaiVatTuTheoRangeDate(phieuCongKhaiVatTu.YeuCauTiepNhanId, start.AddDays(i), start.AddDays(difference.TotalDays - i > 7 ? i + 6 : difference.TotalDays), phieuCongKhaiVatTu.DanhSachVatTus));
                }
            }
            else
            {
                //Lấy tất cả thuốc có ngày điều trị >= từ ngày và <=đến ngày
                returnHtml.Add(GetHtmlTableCongKhaiVatTuTheoRangeDate(phieuCongKhaiVatTu.YeuCauTiepNhanId, start, end, phieuCongKhaiVatTu.DanhSachVatTus));
            }
            return returnHtml;
        }

        public PhieuCongKhaiVatTuObject GetThongTinPhieuCongKhaiVatTu(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiVatTu)
                                                                  .Select(s => new PhieuCongKhaiVatTuObject()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiVatTu,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuCongKhaiVatTunGridVo()
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
            if(query != null)
            {
                var queryString = JsonConvert.DeserializeObject<InPhieuCongKhaiVatTu>(query.ThongTinHoSo);
                if (queryString.NgayRaVien != null && queryString.NgayVaoVien != null)
                {
                    var newphieuCongKhaiObject = new PhieuCongKhaiVatTu()
                    {
                        NgayRaVien = (DateTime)queryString.NgayRaVien?.ToLocalTime(),
                        NgayVaoVien = (DateTime)queryString.NgayVaoVien?.ToLocalTime(),
                        YeuCauTiepNhanId = query.YeuCauTiepNhanId
                    };
                    var htmls = GetDataPhieuCongKhaiVatTu(newphieuCongKhaiObject);
                    queryString.TableString = new List<Table>();
                    var newListTable = new List<Table>();
                    foreach (var item in htmls)
                    {
                        var newTable = new Table();
                        newTable.Html = item;
                        queryString.TableString.Add(newTable);
                    }
                }
                var jsonString = JsonConvert.SerializeObject(queryString);
                query.ThongTinHoSo = jsonString;
            }
           
            return query;
        }
        public async Task<string> InPhieuCongKhaiVatTu(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var content = "";
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InPhieuCongKhaiVatTu>(thongtinIn);
            var dataThongTinNguoibenh = ThongTinChungBenhNhanPhieuDieuTri(xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId);
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuCongKhaiVatTuHoSoKhac"));
            DateTime tuNgayPart = DateTime.Now; ;
            tuNgayPart = queryString.NgayVaoVien.GetValueOrDefault().AddDays(1);
            DateTime denNgayPart = DateTime.Now; ;
            denNgayPart = queryString.NgayRaVien.GetValueOrDefault().AddDays(1);

            var phieuCongKhaiVatTu = new PhieuCongKhaiVatTu()
            {
                YeuCauTiepNhanId = xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId,
                NgayVaoVien = tuNgayPart,
                NgayRaVien = denNgayPart
            };

            if (xacNhanInTrichBienBanHoiChan.LoaiPhieuIn == 1)
            {
                var htmlVatTu = GetDataPhieuCongKhaiVatTu(phieuCongKhaiVatTu);
                var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                                                  .Select(x => new
                                                                                  {
                                                                                      So = dataThongTinNguoibenh.MaTN,
                                                                                      HoTenNguoiBenh = dataThongTinNguoibenh.HoTenNguoiBenh,
                                                                                      Tuoi = dataThongTinNguoibenh.Tuoi,
                                                                                      Gioi = dataThongTinNguoibenh.Gioi,
                                                                                      NgayVaoVien = tuNgayPart.ApplyFormatDate(),
                                                                                      NgayRaVien = denNgayPart.ApplyFormatDate(),
                                                                                      CoBHYT = dataThongTinNguoibenh.CoBHYT,
                                                                                      SoGiuong = dataThongTinNguoibenh.SoGiuong,
                                                                                      SoBA = dataThongTinNguoibenh.SoBA,
                                                                                      Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(p => p.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                      MaTheBHYT = dataThongTinNguoibenh.MaTheBHYT,
                                                                                      Khoa = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderByDescending(p => p.ThoiDiemVaoKhoa).First().KhoaPhongChuyenDen.Ten,
                                                                                      TableString = htmlVatTu != null ? htmlVatTu.ToList().Join("") : "",
                                                                                      ChanDOan = queryString.ChanDoan,
                                                                                  }).FirstOrDefault();
                content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            }
            else
            {
                DateTime start = new DateTime(phieuCongKhaiVatTu.NgayVaoVien.Year, phieuCongKhaiVatTu.NgayVaoVien.Month, phieuCongKhaiVatTu.NgayVaoVien.Day);
                DateTime end = new DateTime(phieuCongKhaiVatTu.NgayRaVien.Year, phieuCongKhaiVatTu.NgayRaVien.Month, phieuCongKhaiVatTu.NgayRaVien.Day);

                var dsVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(p => p.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId
                                && p.NoiTruPhieuDieuTri.NgayDieuTri >= start 
                                && p.NoiTruPhieuDieuTri.NgayDieuTri <= end 
                                && p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                    .OrderBy(p => p.Id)
                    .Select(s => new PhieuCongKhaiVatTuGridVo
                    {
                        TenVatTu = s.Ten,
                        DonVi = s.DonViTinh,
                        SoLuong = s.SoLuong,
                        DonGia = s.DonGiaBan,
                        ThanhTien = (decimal)(s.SoLuong * (double)s.DonGiaBan),
                        GhiChu = s.GhiChu,
                        NgayThang = s.NoiTruPhieuDieuTri.NgayDieuTri,

                        //BVHD-3876
                        KhoaId = s.NoiChiDinh.KhoaPhongId,
                        TenKhoa = s.NoiChiDinh.KhoaPhong.Ten
                    }).ToList();

                var lstKhoa = dsVatTu.Where(x => x.KhoaId != null)
                    .Select(x => new LookupItemVo()
                    {
                        KeyId = x.KhoaId.Value,
                        DisplayName = x.TenKhoa
                    }).Distinct().ToList();
                var lstKhoaId = lstKhoa.Select(x => x.KeyId).Distinct().ToList();

                var giuong = _noiTruHoSoKhacRepository.TableNoTracking
                    .Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId
                                && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                    .SelectMany(x => x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens)
                    .Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                    .OrderBy(x => x.Id)
                    .Select(x => x.GiuongBenh.PhongBenhVien.Ten)
                    .FirstOrDefault();

                foreach (var khoaId in lstKhoaId)
                {
                    var dsVatTuTheoKhoa = dsVatTu.Where(x => x.KhoaId == khoaId).ToList();
                    var tenKhoa = lstKhoa.Where(x => x.KeyId == khoaId).Select(x => x.DisplayName).First();

                    phieuCongKhaiVatTu.DanhSachVatTus = new List<PhieuCongKhaiVatTuGridVo>();
                    phieuCongKhaiVatTu.DanhSachVatTus.AddRange(dsVatTuTheoKhoa);

                    var htmlVatTu = GetDataPhieuCongKhaiVatTu(phieuCongKhaiVatTu);
                    var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                                                      .Select(x => new
                                                                                      {
                                                                                          So = dataThongTinNguoibenh.MaTN,
                                                                                          HoTenNguoiBenh = dataThongTinNguoibenh.HoTenNguoiBenh,
                                                                                          Tuoi = dataThongTinNguoibenh.Tuoi,
                                                                                          Gioi = dataThongTinNguoibenh.Gioi,
                                                                                          NgayVaoVien = tuNgayPart.ApplyFormatDate(),
                                                                                          NgayRaVien = denNgayPart.ApplyFormatDate(),
                                                                                          CoBHYT = dataThongTinNguoibenh.CoBHYT,
                                                                                          SoGiuong = dataThongTinNguoibenh.SoGiuong,
                                                                                          SoBA = dataThongTinNguoibenh.SoBA,
                                                                                          //Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(p => p.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                          Buong = giuong,
                                                                                          MaTheBHYT = dataThongTinNguoibenh.MaTheBHYT,
                                                                                          //Khoa = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderByDescending(p => p.ThoiDiemVaoKhoa).First().KhoaPhongChuyenDen.Ten,
                                                                                          Khoa = tenKhoa,
                                                                                          TableString = htmlVatTu != null ? htmlVatTu.ToList().Join("") : "",
                                                                                          ChanDOan = queryString.ChanDoan,
                                                                                      }).FirstOrDefault();
                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                }
            }
            return content;
        }
    }
}
