using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiThuoc;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        private ThongTinChungBenhNhanPhieuDieuTri ThongTinChungBenhNhan(long yeuCauTiepNhanId)
        {
            var thongTin = BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                .Select(s => new ThongTinChungBenhNhanPhieuDieuTri
                {
                    MaTN = s.MaYeuCauTiepNhan,
                    HoTenNguoiBenh = s.HoTen,
                    Tuoi = s.NamSinh != null && s.NamSinh != 0 ? DateTime.Now.Year - s.NamSinh : null,
                    Gioi = s.GioiTinh.GetValueOrDefault().GetDescription(),
                    NgayVaoVien = s.ThoiDiemTiepNhan.ApplyFormatDate(),
                    CoBHYT = s.CoBHYT,
                    SoGiuong = s.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan) ? s.YeuCauDichVuGiuongBenhViens.OrderByDescending(p => p.Id).FirstOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.Ten : "",
                    SoBA = s.NoiTruBenhAn.SoBenhAn,
                    KhoaPhong = s.NoiTruBenhAn.KhoaPhongNhapVien.Ten,
                    MaTheBHYT = s.BHYTMaSoThe
                }).First();
            return thongTin;
        }
        private string GetHtmlTableCongKhaiThuocTheoRangeDate(long yeuCauTiepNhanId,DateTime start, DateTime end, List<PhieuCongKhaiThuocGridVo> dsThuocTheoKhoas)
        {
            var returnHtml = "";
            TimeSpan difference = end - start; //create TimeSpan object

            //BVHD-3876
            var dsThuoc = new List<PhieuCongKhaiThuocGridVo>();
            if (dsThuocTheoKhoas.Any())
            {
                dsThuoc = dsThuocTheoKhoas.Where(p => p.NgayThang >= start && p.NgayThang <= end).ToList();
            }
            else
            {
                dsThuoc = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                   .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.NoiTruPhieuDieuTri.NgayDieuTri >= start && p.NoiTruPhieuDieuTri.NgayDieuTri <= end &&
                               p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                   .Select(s => new PhieuCongKhaiThuocGridVo
                   {
                       TenThuoc = s.Ten,
                       DonVi = s.DonViTinh.Ten,
                       SoLuong = s.SoLuong,
                       DonGia = s.DonGiaBan,
                       ThanhTien = (decimal)(s.SoLuong * (double)s.DonGiaBan),
                       GhiChu = s.GhiChu,
                       NgayThang = s.NoiTruPhieuDieuTri.NgayDieuTri,
                       LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                       HoatChat = s.HoatChat,
                       HamLuong = s.HamLuong,
                       DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId
                   }).ToList();
                //.GroupBy(x => new { x.TenThuoc, x.DonVi, x.DonGia,x.NgayThang })
                //.Select(item => new PhieuCongKhaiThuocGridVo
                //{
                //    TenThuoc = item.First().TenThuoc,
                //    DonVi = item.First().DonVi,
                //    SoLuong = item.First().SoLuong,
                //    TongSo = item.Sum(x => x.SoLuong),
                //    DonGia = item.First().DonGia,
                //    ThanhTien = item.Sum(x => x.ThanhTien),
                //    GhiChu = item.First().GhiChu,
                //    NgayThang = item.First().NgayThang,
                //}).Distinct().ToList();
            }

            returnHtml += "<table style='border-collapse: collapse;width:100%'> " +
                     "<thead>" +
                     "<tr>" +
                     "<th  rowspan ='2' style='border: 1px solid black; width: 5%; padding: 0px; margin: 0px; '><b>SỐ <br>TT</b> </th>" +
                     "<th rowspan ='2'  style='border: 1px solid black; width: 17%;'><b>Tên thuốc, hàm lượng</b></th>" +
                     "<th  rowspan = '2' style ='border: 1px solid black;width: 8%;text-align:center;' ><b> Đơn <br> vị </b> </th >" +
                     "<th colspan='"+ (difference.TotalDays+1) + "' style='border: 1px solid black; width: 28%; text - align:center;'><b>Ngày,tháng</b></th>" +
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
            var dsThuocGroup = dsThuoc.GroupBy(s => new { s.TenThuoc, s.DonVi })
                .Select(z => new PhieuCongKhaiThuocGridVo()
                {
                    TenThuoc = z.First().TenThuoc,
                    DonVi = z.First().DonVi,
                    NgayThang = z.First().NgayThang,
                    SoLuong = z.Sum(sv=>sv.SoLuong),
                    TongSo = z.Sum(x => x.SoLuong),
                    DonGia = z.First().DonGia,
                    ThanhTien = z.Sum(x => x.ThanhTien),
                    GhiChu = z.First().GhiChu,
                    LoaiThuocTheoQuanLy = z.First().LoaiThuocTheoQuanLy,
                    HoatChat = z.First().HoatChat,
                    HamLuong = z.First().HamLuong,
                    DuocPhamBenhVienPhanNhomId = z.First().DuocPhamBenhVienPhanNhomId,
                    NgayThangSlThuocs = z.Select(s => new ThuocNgayThang() { SoLuong = s.SoLuong, NgayThang = s.NgayThang }).GroupBy(c => c.NgayThang).Select(bb => new ThuocNgayThang() { SoLuong = bb.Sum(s=>s.SoLuong), NgayThang = bb.First().NgayThang }).ToList()
                }).ToList();
            var stt = 1;
            foreach (var item in dsThuocGroup)
            {
                returnHtml += "<tr>"
                                                        + "<td  style = 'border: 1px solid black; width: 5%; padding: 0px; margin: 0px;text-align:center'>" + stt + "</td>"
                                                        + "<td class='tenThuoc' style = 'border: 1px solid black; width: 17%;padding:5px;'>" + _yeuCauKhamBenhService.FormatTenDuocPham(item.TenThuoc, item.HoatChat, item.HamLuong,item.DuocPhamBenhVienPhanNhomId) + "</td>"
                                                        + "<td class='donVi' style ='border: 1px solid black;width: 8%;text-align:center;'>" + item.DonVi + "</td>";
                for (int i = 0; i <= difference.TotalDays; i++)
                {
                    if (item.NgayThangSlThuocs.Any(x=>x.NgayThang == start.AddDays(i)))
                    {
                        returnHtml += "<td   class='ngayThang' style = 'border: 1px solid black;width: 4%;text-align:center;'>" + _yeuCauKhamBenhService.FormatSoLuong (item.NgayThangSlThuocs.Where(x => x.NgayThang == start.AddDays(i)).Select(s=>s.SoLuong).FirstOrDefault(),item.LoaiThuocTheoQuanLy) + "</td>";
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
            if (!dsThuocGroup.Any() || (dsThuocGroup.Any() && dsThuocGroup.Count < 10))
            {
                for (int j = 0; j < (dsThuocGroup.Any() ? 10 - dsThuocGroup.Count:10); j++)
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

            var tongKhoanThuocDung = dsThuocGroup.Count();
            returnHtml += "<tr>"
                       + "<td colspan='2' style=' border: 1px solid black; width: 22%; padding: 0px; margin: 0px;padding:5px; '>" + "Tổng số khoản thuốc dùng: " + tongKhoanThuocDung + "</td>"
                       + "<td style ='border: 1px solid black;width: 8%;text-align:center;'>" + "</td>";
            for (int i = 0; i <= difference.TotalDays; i++)
            {
                returnHtml += "<td  style = 'border: 1px solid black;width: 4%;text-align:center;'>" + dsThuoc.Where(o => o.NgayThang == start.AddDays(i)).Sum(o => o.SoLuong) + "</td>";
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
        public List<string> GetDataPhieuCongKhaiThuoc(PhieuCongKhaiThuoc phieuCongKhaiThuoc)
        {
            DateTime start = new DateTime(phieuCongKhaiThuoc.NgayVaoVien.Year, phieuCongKhaiThuoc.NgayVaoVien.Month, phieuCongKhaiThuoc.NgayVaoVien.Day);
            DateTime end = new DateTime(phieuCongKhaiThuoc.NgayRaVien.Year, phieuCongKhaiThuoc.NgayRaVien.Month, phieuCongKhaiThuoc.NgayRaVien.Day);

            List<string> returnHtml = new List<string>();
            TimeSpan difference = end - start; //create TimeSpan object
            //Tách ra mỗi tờ gồm 7 ngày
            bool kiemTraChayLanDau = true;
            if (difference.TotalDays > 7)
            {

                for (int i = 0; i < difference.TotalDays; i=i+7)
                {
                    returnHtml.Add(GetHtmlTableCongKhaiThuocTheoRangeDate(phieuCongKhaiThuoc.YeuCauTiepNhanId, start.AddDays(i), start.AddDays(difference.TotalDays - i > 7 ? i + 6 : difference.TotalDays), phieuCongKhaiThuoc.DanhSachThuocs));
                }    
            }
            else
            {
                //Lấy tất cả thuốc có ngày điều trị >= từ ngày và <=đến ngày
                returnHtml.Add(GetHtmlTableCongKhaiThuocTheoRangeDate(phieuCongKhaiThuoc.YeuCauTiepNhanId, start, end, phieuCongKhaiThuoc.DanhSachThuocs));
            }
            return returnHtml;
        }
        public PhieuCongKhaiThuocObject GetThongTinPhieuCongKhaiThuoc(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiThuoc)
                                                                  .Select(s => new PhieuCongKhaiThuocObject()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuCongKhaiThuoc,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuCongKhaiThuocnGridVo()
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
            if( query != null)
            {
                var queryString = JsonConvert.DeserializeObject<InPhieuCongKhaiThuoc>(query.ThongTinHoSo);
                if (queryString.NgayRaVien != null && queryString.NgayVaoVien != null)
                {
                    var newphieuCongKhaiObject = new PhieuCongKhaiThuoc()
                    {
                        NgayRaVien = queryString.NgayRaVien.ToLocalTime(),
                        NgayVaoVien = queryString.NgayVaoVien.ToLocalTime(),
                        YeuCauTiepNhanId = query.YeuCauTiepNhanId
                    };
                    var htmls = GetDataPhieuCongKhaiThuoc(newphieuCongKhaiObject);
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
        public async Task<string> InPhieuCongKhaiThuoc(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var content = "";
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InPhieuCongKhaiThuoc>(thongtinIn);
            var dataThongTinNguoibenh = ThongTinChungBenhNhanPhieuDieuTri(xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId);
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuCongKhaiThuocHoSoKhac"));
            DateTime tuNgayPart = DateTime.Now; ;
             tuNgayPart = queryString.NgayVaoVien.AddDays(1);
            DateTime denNgayPart = DateTime.Now; ;
            denNgayPart = queryString.NgayRaVien.AddDays(1);

            var phieuCongKhaiThuoc = new PhieuCongKhaiThuoc()
            {
                YeuCauTiepNhanId = xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId,
                NgayVaoVien = tuNgayPart,
                NgayRaVien = denNgayPart
            };

            if (xacNhanInTrichBienBanHoiChan.LoaiPhieuIn == 1)
            {
                var htmlThuoc = GetDataPhieuCongKhaiThuoc(phieuCongKhaiThuoc);
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
                                                                                      Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(v => v.Id).Select(c => c.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                      MaTheBHYT = dataThongTinNguoibenh.MaTheBHYT,
                                                                                      TableString = htmlThuoc != null ? htmlThuoc.Select(s => s).ToList().Join("") : "",
                                                                                      ChanDOan = queryString.ChanDoan,
                                                                                      Khoa = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderByDescending(p => p.ThoiDiemVaoKhoa).First().KhoaPhongChuyenDen.Ten
                                                                                  }).FirstOrDefault();
                content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            }
            else
            {
                DateTime start = new DateTime(phieuCongKhaiThuoc.NgayVaoVien.Year, phieuCongKhaiThuoc.NgayVaoVien.Month, phieuCongKhaiThuoc.NgayVaoVien.Day);
                DateTime end = new DateTime(phieuCongKhaiThuoc.NgayRaVien.Year, phieuCongKhaiThuoc.NgayRaVien.Month, phieuCongKhaiThuoc.NgayRaVien.Day);

                var dsThuoc = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(p => p.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId 
                                && p.NoiTruPhieuDieuTri.NgayDieuTri >= start 
                                && p.NoiTruPhieuDieuTri.NgayDieuTri <= end 
                                && p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .Select(s => new PhieuCongKhaiThuocGridVo
                    {
                        TenThuoc = s.Ten,
                        DonVi = s.DonViTinh.Ten,
                        SoLuong = s.SoLuong,
                        DonGia = s.DonGiaBan,
                        ThanhTien = (decimal)(s.SoLuong * (double)s.DonGiaBan),
                        GhiChu = s.GhiChu,
                        NgayThang = s.NoiTruPhieuDieuTri.NgayDieuTri,
                        LoaiThuocTheoQuanLy = s.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                        HoatChat = s.HoatChat,
                        HamLuong = s.HamLuong,
                        DuocPhamBenhVienPhanNhomId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,

                        //BVHD-3876
                        KhoaId = s.NoiChiDinh.KhoaPhongId,
                        TenKhoa = s.NoiChiDinh.KhoaPhong.Ten
                    }).ToList();

                var lstKhoa = dsThuoc.Where(x => x.KhoaId != null)
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
                    var dsThuocTheoKhoa = dsThuoc.Where(x => x.KhoaId == khoaId).ToList();
                    var tenKhoa = lstKhoa.Where(x => x.KeyId == khoaId).Select(x => x.DisplayName).First();

                    phieuCongKhaiThuoc.DanhSachThuocs = new List<PhieuCongKhaiThuocGridVo>();
                    phieuCongKhaiThuoc.DanhSachThuocs.AddRange(dsThuocTheoKhoa);

                    var htmlThuoc = GetDataPhieuCongKhaiThuoc(phieuCongKhaiThuoc);

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
                                                                                          //Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(v => v.Id).Select(c => c.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                          Buong = giuong,
                                                                                          MaTheBHYT = dataThongTinNguoibenh.MaTheBHYT,
                                                                                          TableString = htmlThuoc != null ? htmlThuoc.Select(s => s).ToList().Join("") : "",
                                                                                          ChanDOan = queryString.ChanDoan,
                                                                                          Khoa = tenKhoa
                                                                                      }).FirstOrDefault();
                    content +=  TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
                    //(!string.IsNullOrEmpty(content) ? "<div style='break-after:page'></div><br/>" : "") +
                }
            }
            return content;
        }
    }
}
