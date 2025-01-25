using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.PhieuKhaiThacTienSuDiUng;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public PhieuKhaiThacTienSuDiUngConfig PhieuKhaiThacTienSuDiUngConfig()
        {
            var query = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.PhieuKhaiThacTienSuDiUng").Select(s => new PhieuKhaiThacTienSuDiUngConfig() { Value = s.Value }).FirstOrDefault();
            return query;
        }
        public List<string> GetDanhSachDuocPhamQuocGia()
        {
            var lstDsDuocPhamQuocGia =
                _duocPhamBenhVienRepository.TableNoTracking
                   .Where(x => !string.IsNullOrEmpty(x.DuocPham.Ten.Trim()))
                   .Select(x => x.DuocPham.Ten).ToList();
            return lstDsDuocPhamQuocGia;
        }
        public List<string> GetDanhSachDuocPhamQuocGiaDeNghiTest()
        {
            var lstDsDuocPhamQuocGia =
                _duocPhamBenhVienRepository.TableNoTracking
                   .Where(x => !string.IsNullOrEmpty(x.DuocPham.Ten.Trim()))
                   .Select(x => (x.DuocPham.Ten + (x.DuocPham.HamLuong != null ?", " + x.DuocPham.HamLuong  :"") 
                                                + (x.DuocPham.DonViTinh.Ten != null ? ", " + x.DuocPham.DonViTinh.Ten : "")
                                                + (x.DuocPham.NuocSanXuat != null ? ", " + x.DuocPham.NuocSanXuat : "")
                                                + (x.DuocPham.SoDangKy != null ? ", " + x.DuocPham.SoDangKy : ""))
                   ).ToList();
            return lstDsDuocPhamQuocGia;
        }
        public PhieuKhaiThacTienSuDiUngGridVo GetThongTinPhieuKhaiThacTienSuBenh(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuKhaiThacTienSuDiUng)
                                                                  .Select(s => new PhieuKhaiThacTienSuDiUngGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.TrichBienBanHoiChan,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuKhaiThacTienSuDiUngGridVo()
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
        public async Task<string> PhieuKhaiThacTienSuBenh(XacNhanInTienSu xacNhanIn)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanIn.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<PhieuKhaiThacTienSuDiUngVo>(thongtinIn);
            var test = queryString.PhieuKhaiThacTienSuDiUngList.Substring(1, queryString.PhieuKhaiThacTienSuDiUngList.Length - 2);
            var list = JsonConvert.DeserializeObject<List<ThongTinPhieuKhaiThacTienSuDiUng>>(queryString.PhieuKhaiThacTienSuDiUngList);
            var content = "";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuKhaiThacTienSuDiUng"));
            bool inLanDau = true;
            var htmlDanhSachDichVu = "";
            if (inLanDau == true)
            {
                htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px; width:5%;word-break: break-word;'>STT </th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px;width: 40%;word-break: break-word;'>Nội dung</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px;width: 20%;word-break: break-word;'>Tên thuốc,dị nguyên gây dị ứng</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px ; width: 12%;word-break: break-word;'>Có/số lần</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px ; width: 12%;word-break: break-word;'>Không</th>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px; width:11%;word-break: break-word;'>Biểu hiện<br>lâm sàng- <br>xử trí</th>";
                htmlDanhSachDichVu += "</tr>";
                inLanDau = false;
            }
            string co = "&nbsp;";
            string khong = "&nbsp;";
            htmlDanhSachDichVu += "<tbody>";
            foreach (var item in list)
            {
                
                if (item.CoKhong == false)
                {
                    khong = "Không";
                }
                htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
                htmlDanhSachDichVu += "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px; width:4%;word-break: break-word;'>" + item.Stt.ToString() + "</td>";
                htmlDanhSachDichVu += "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: float;font-size: 13px;width: 40%;word-break: break-word;'><div style='padding:5px;padding-top:0px;padding-bottom:0px'>" + item.NoiDung.ToString() +"</div></td>";
                htmlDanhSachDichVu += "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px;width: 20%;word-break: break-word;'>" + item.TenThuoc + item.DiNguyenGayDiUng + "</td>";
                if (item.CoKhong == true)
                {
                    co = "Có";
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px ; width: 10%;word-break: break-word;'>" + co + '/' + item.SoLan + "</td>";
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px ; width: 10%;word-break: break-word;'>" + "&nbsp;" + "</td>";
                }
                else
                {
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px ; width: 10%;word-break: break-word;'>" + "&nbsp;" + "</td>";
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px ; width: 10%;word-break: break-word;'>" + khong + "</td>";
                }
                
                htmlDanhSachDichVu += "<td style='border: 1px solid #020000; border-collapse: collapse;text-align: center;font-size: 13px; width:16%;word-break: break-word;'>" + (!string.IsNullOrEmpty(item.BieuHienLamSang) && !string.IsNullOrEmpty(item.XuTri) ? item.BieuHienLamSang +" - "+ item.XuTri : item.BieuHienLamSang  + item.XuTri) + "</td>";
                htmlDanhSachDichVu += "</tr>";
               
            }
            htmlDanhSachDichVu += "</tbody>";
            htmlDanhSachDichVu += "</table>";
            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new
                                                                              {
                                                                                  HoTen = x.YeuCauTiepNhan.HoTen,
                                                                                  NamSinh = x.YeuCauTiepNhan.NamSinh,
                                                                                  NgaySinh = x.YeuCauTiepNhan.NgaySinh,
                                                                                  ThangSinh = x.YeuCauTiepNhan.ThangSinh,
                                                                                  Khoa = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderByDescending(p => p.ThoiDiemVaoKhoa).First().KhoaPhongChuyenDen.Ten,
                                                                                  Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(v => v.Id).Select(c => c.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                  Giuong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(v => v.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(v => v.Id).Select(v => v.GiuongBenh.Ten).FirstOrDefault(),
                                                                                  ChanDoan = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any(p => p.ChanDoanVaoKhoaICDId != null)
                                                                                            ? x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(p => p.ChanDoanVaoKhoaICDId != null)
                                                                                                //.Select(p => p.ChanDoanVaoKhoaICD.Ma + " - " + p.ChanDoanVaoKhoaICD.TenTiengViet)
                                                                                                .Select(p => p.ChanDoanVaoKhoaGhiChu)
                                                                                                .Join(", ")
                                                                                            : "",
                                                                                  table = htmlDanhSachDichVu,
                                                                                  NamNu = x.YeuCauTiepNhan.GioiTinh != null ? x.YeuCauTiepNhan.GioiTinh.GetValueOrDefault().GetDescription() :"",
                                                                                  BacSyKhaiThacTienSuDiUng = queryString.BSKhaiThac,
                                                                                  NguoiBenh = x.YeuCauTiepNhan.HoTen,
                                                                                  MaNB = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                                 
                                                                                  MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan
                                                                              }).FirstOrDefault();

            string[] listStringGiuongBenhs = new string[] { "Giường", "GIƯỜNG" };
            var giuong = string.Empty;
            if (!string.IsNullOrEmpty(data.Giuong))
            {
                foreach (var item in listStringGiuongBenhs)
                {
                    giuong = data.Giuong.Replace(item, "").ToString();
                }
            }
            var infoPhieuKhaiThac = new InfoPhieuInPhieuKhaiThacTienSuDiUng();

            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            infoPhieuKhaiThac.KhoaPhongDangIn = tenKhoa;

            infoPhieuKhaiThac.BarCodeImgBase64 = !string.IsNullOrEmpty(data.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(data.MaYeuCauTiepNhan) : "";
            infoPhieuKhaiThac.MaNB = data.MaYeuCauTiepNhan;



            infoPhieuKhaiThac.HoTen = !string.IsNullOrEmpty(data.HoTen) ? "<div class='container'><div class='label'>- Họ tên người bệnh:&nbsp;</div><div class='values'><b>" + data.HoTen +"</b></div></div>"
                : "<div class='container'><div class='label'>- Họ tên người bệnh:&nbsp;</div><div class='value'><b>" + data.HoTen + "</b></div></div>";

            var namSinh = string.Empty;

            namSinh = DateHelper.DOBFormat(data.NgaySinh, data.ThangSinh, data.NamSinh);
            infoPhieuKhaiThac.Tuoi = !string.IsNullOrEmpty(namSinh) ? "<div class='container'><div class='label'>Năm sinh:&nbsp;</div><div class='values'><b>" + namSinh + "</b></div><div class='label'>Giới tính:&nbsp;<b>" + data.NamNu +"</b></div></div>" 
                : "<div class='container'><div class='label'>Năm sinh:&nbsp;</div><div class='value'><b>" + namSinh + "</b></div><div class='label'>Giới tính:&nbsp;<b>" + data.NamNu + "</b></div></div>";


            //infoPhieuKhaiThac.NamNu = data.NamNu;
            infoPhieuKhaiThac.Khoa = !string.IsNullOrEmpty(data.Khoa) ? "<div class='container'><div class='label'>- Khoa:&nbsp;</div><div class='values'>" + data.Khoa + "</div></div>"
                : "<div class='container'><div class='label'>- Khoa:&nbsp;</div><div class='value'>" + data.Khoa + "</div></div>";


            infoPhieuKhaiThac.Buong = !string.IsNullOrEmpty(data.Buong) ? "<div class='container'><div class='label'>Buồng:&nbsp;</div><div class='values'>" + data.Buong + "</div></div>"
                :"<div class='container'><div class='label'>Buồng:&nbsp;</div><div class='value'>" + data.Buong + "</div></div>";


            if(!string.IsNullOrEmpty(giuong))
            {
                infoPhieuKhaiThac.Giuong = "<div class='container'><div class='label'>Giường:&nbsp;</div><div class='values'>" + giuong +"</div></div>";
            }
            else
            {
                infoPhieuKhaiThac.Giuong = !string.IsNullOrEmpty(data.Giuong) ? "<div class='container'><div class='label'>Giường:&nbsp;</div><div class='values'>" + data.Giuong + "</div></div>"
                    : "<div class='container'><div class='label'>Giường:&nbsp;</div><div class='value'>" + data.Giuong + "</div></div>";
            }

            infoPhieuKhaiThac.ChanDoan = !string.IsNullOrEmpty(data.ChanDoan) ? "<div class='container'><div class='label'>- Chẩn đoán:&nbsp;</div><div class='values'>" + data.ChanDoan + "</div></div>" 
                : "<div class='container'><div class='label'>- Chẩn đoán:&nbsp;</div><div class='value'>" + data.ChanDoan + "</div></div></td>";


            infoPhieuKhaiThac.Table = data.table;

            infoPhieuKhaiThac.BacSyKhaiThacTienSuDiUng = data.BacSyKhaiThacTienSuDiUng;
            infoPhieuKhaiThac.NguoiBenh = data.NguoiBenh;



            //BarCodeImgBase64 = !string.IsNullOrEmpty(x.YeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(x.YeuCauTiepNhan.MaYeuCauTiepNhan) : "",

            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, infoPhieuKhaiThac);
            return content;
        }
    }
}
