using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
   public partial class DieuTriNoiTruService
    {
        public PhieuDeNghiTestTruocKhiDungThuocGridVo GetThongTinPhieuDeNghiTestTruocKhiDung(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuDeNghiTestTruocKhiDungThuoc)
                                                                  .Select(s => new PhieuDeNghiTestTruocKhiDungThuocGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuDeNghiTestTruocKhiDungThuoc,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuDeNghiTestTruocKhiDungThuocGridVo()
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
        public async Task<bool> KiemTraNamSinhHople(int? namSinh, long id = 0)
        {
            var result = false;
            var yearNow = DateTime.Now.Year;
            if(namSinh > yearNow)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<string> InPhieuDeNghiTestTruocKhiDungThuoc(InPhieuDeNghiTestTruocKhiDungThuoc inPhieuDeNghiTestTruocKhiDungThuoc)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == inPhieuDeNghiTestTruocKhiDungThuoc.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == inPhieuDeNghiTestTruocKhiDungThuoc.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == inPhieuDeNghiTestTruocKhiDungThuoc.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InPhieuDeNghiTestTruocKhiDungThuocobject>(thongtinIn);
            var content = "";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
           
            var dataThongTinNguoibenh = ThongTinChungBenhNhanPhieuDieuTri(inPhieuDeNghiTestTruocKhiDungThuoc.YeuCauTiepNhanId);
            
            if(inPhieuDeNghiTestTruocKhiDungThuoc.LoaiPhieuIn == 1) // giấy đề nghị thử test khi dùng thuốc
            {
                string tinhTrangDungThuocCuaBenhNhan ="";

                if (queryString.DanhSachThuocCanTestArr.Count() > 0)
                {
                    var lengthDaCongThuoc = 0;
                    foreach (var itemColumns in queryString.DanhSachThuocCanTestArr)
                    {
                        tinhTrangDungThuocCuaBenhNhan += " " + ShowThongTinDuocPham(itemColumns.Thuoc);

                        if (lengthDaCongThuoc < queryString.DanhSachThuocCanTestArr.Count())
                        {
                            tinhTrangDungThuocCuaBenhNhan += ";";
                        }
                        lengthDaCongThuoc++;
                    }
                         
                }
                string gioiTinh = "";
                string danToc = "";
                string ngheNghiep = "";
                string ngoaiKieu = "";
                if(queryString.SelectBenhNhanHoacNguoiNha == false)
                {
                    if (queryString.HoTen != null)
                    {
                        if (queryString.GioiTinh == true)
                        {
                            gioiTinh = "Nam";
                        }
                        if (queryString.GioiTinh == false)
                        {
                            gioiTinh = "Nữ";
                        }
                    }

                    if (queryString.DanToc != null)
                    {
                        danToc = _danTocRepository.TableNoTracking.Where(s => s.Id == long.Parse(queryString.DanToc)).Select(s => s.Ten).FirstOrDefault();
                    }

                    if (queryString.NgheNghiep != null)
                    {
                        ngheNghiep = _ngheNghiepRepository.TableNoTracking.Where(s => s.Id == long.Parse(queryString.NgheNghiep)).Select(s => s.Ten).FirstOrDefault();
                    }

                    if (queryString.NgoaiKieu != null)
                    {
                        ngoaiKieu = _quocGiaRepository.TableNoTracking.Where(s => s.Id == long.Parse(queryString.NgoaiKieu)).Select(s => s.QuocTich).FirstOrDefault();
                    }
                }
                var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == inPhieuDeNghiTestTruocKhiDungThuoc.NoiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == inPhieuDeNghiTestTruocKhiDungThuoc.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new
                                                                              {
                                                                                  TenToiLa = queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.HoTen : queryString.HoTen,
                                                                                  Tuoi = queryString.SelectBenhNhanHoacNguoiNha == true ? (DateTime.Now.Year - x.YeuCauTiepNhan.NamSinh).ToString() : queryString.NamSinh != null ? (DateTime.Now.Year - queryString.NamSinh).ToString() : "",
                                                                                  GioiTinh = queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.GioiTinh != null ? x.YeuCauTiepNhan.GioiTinh.GetDescription():"" :gioiTinh,
                                                                                  DanToc = queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.DanTocId != null ? x.YeuCauTiepNhan.DanToc.Ten:"":danToc,
                                                                                  NgoaiKieu = queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.QuocTichId != null ? x.YeuCauTiepNhan.QuocTich.Ten:"": ngoaiKieu,
                                                                                  NgheNghiep = queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.NgheNghiepId != null ? x.YeuCauTiepNhan.NgheNghiep.Ten:"": ngheNghiep,
                                                                                  NoiLamViec = queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.NoiLamViec : queryString.NoiLamViec,
                                                                                  DiaChi= queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.DiaChiDayDu : queryString.DiaChi,
                                                                                  LaNguoiBenhDaiDienGiaDinhNguoiBenhHoTenLa = dataThongTinNguoibenh.HoTenNguoiBenh,
                                                                                  HienDangDuocDieuTriTaiKhoa = dataThongTinNguoibenh.KhoaPhong,
                                                                                  BenhVien = "BVDKQT Bắc Hà",
                                                                                  TinhTrangDungThuocCuaBenhNhan = tinhTrangDungThuocCuaBenhNhan,
                                                                                  HoTen = queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.HoTen : queryString.HoTen,
                                                                                  SoVaoVien= x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                                  DongYTest = queryString.DongYDeNghiTest == true ? "&boxtimes;" : "&#9744;",
                                                                                  KhongDongYTest = queryString.DongYDeNghiTest == true ? "&#9744;" : "&boxtimes;",
                                                                                  NgayHienTai = DateTime.Now.Day,
                                                                                  ThangHienTai = DateTime.Now.Month,
                                                                                  NamHienTai = DateTime.Now.Year
                                                                              }).FirstOrDefault();
                var result = _templateRepository.TableNoTracking
              .FirstOrDefault(x => x.Name.Equals("GiayDeNghiTestTruocKhiDung"));
                content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            }
            else // giấy phản ứng thuốc
            {
                var columnTable = "";
                if (queryString.DanhSachThuocCanTestArr.Count() > 0)
                {
                    foreach(var itemColumns in queryString.DanhSachThuocCanTestArr)
                    {
                        DateTime ngayThu = DateTime.Now;
                        DateTime.TryParseExact(itemColumns.NgayThuUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayThu);
                        DateTime ngayDocKQ = DateTime.Now;
                        DateTime.TryParseExact(itemColumns.NgayDocKQUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayDocKQ);
                        var stringNgayThu = string.Empty;
                        var tenDuocPham = string.Empty;
                        if (itemColumns.NgayThuUTC != null)
                        {
                            stringNgayThu = ngayThu.ApplyFormatDateTime();
                        }
                        if (itemColumns.NgayThuUTC == null)
                        {
                            stringNgayThu = "";
                        }
                 
                        columnTable += "<tr>" +
                                      "<td style = 'font-size: 15px ; text-align:center;height: 25px;'>" + stringNgayThu + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + ShowThongTinDuocPham(itemColumns.Thuoc) + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + itemColumns.PhuongPhapThu + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + itemColumns.BacSiChiDinh + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + itemColumns.NguoiThu + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + itemColumns.BSDocVaKiemTra + "</td>" 
                                    + "<td style ='font - size: 15px; text - align:center'>" + (ngayDocKQ != null ? ngayDocKQ.Hour + " giờ " +ngayDocKQ.Minute +" phút " + " Ngày " + ngayDocKQ.Day + " tháng " + ngayDocKQ.Month + " năm" + ngayDocKQ.Year :"")  + "</td>"
                                    + "</tr>";
                    }
                    var soDong = 20 - queryString.DanhSachThuocCanTestArr.Count();
                    if (soDong > 0)
                    {
                        for (int i = 0; i < soDong; i++)
                        {
                            columnTable += "<tr>" +
                                          "<td style = 'font-size: 15px ; text-align:center;height: 25px;'>" + "" + "</td>"
                                        + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                        + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                        + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                        + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                        + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                        + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                        + "</tr>";
                        }
                    }
                }
                else
                {
                    for(int i = 0; i < 30; i++)
                    {
                        columnTable += "<tr>" +
                                      "<td style = 'font-size: 15px ; text-align:center;height: 25px;'>" + "" + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                    + "<td style ='font - size: 15px; text - align:center'>" + "" + "</td>"
                                    + "</tr>";
                    }
                }
                var ketQuaSinhHieuObject = _ketQuaSinhHieuRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == inPhieuDeNghiTestTruocKhiDungThuoc.YeuCauTiepNhanId)
                    .Select(s => new { 
                        CanNang = s.CanNang,
                        ChieuCao = s.ChieuCao
                   }).LastOrDefault();
                var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == inPhieuDeNghiTestTruocKhiDungThuoc.NoiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == inPhieuDeNghiTestTruocKhiDungThuoc.LoaiHoSoDieuTriNoiTru)
                                                                             .Select(x => new
                                                                             {
                                                                                 HoTen = dataThongTinNguoibenh.HoTenNguoiBenh,
                                                                                 Tuoi = DateTime.Now.Year - x.YeuCauTiepNhan.NamSinh,
                                                                                 GioiTinh = x.YeuCauTiepNhan.GioiTinh != null ? x.YeuCauTiepNhan.GioiTinh.GetDescription() :"",
                                                                                 CanNang = ketQuaSinhHieuObject != null ? ketQuaSinhHieuObject.CanNang.GetValueOrDefault() : 0,
                                                                                 ChieuCao = ketQuaSinhHieuObject != null ? ketQuaSinhHieuObject.ChieuCao.GetValueOrDefault() : 0,
                                                                                 DiaChi = x.YeuCauTiepNhan.DiaChiDayDu,
                                                                                 Khoa = dataThongTinNguoibenh.KhoaPhong,
                                                                                 Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(p => p.GiuongBenh.PhongBenhVien.Ten).LastOrDefault(),
                                                                                 Giuong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(k => k.GiuongBenh.Ten).LastOrDefault(),
                                                                                 ChanDoan = queryString.ChanDoan,
                                                                                 columnTable = columnTable
                                                                             }).FirstOrDefault();

                var result = _templateRepository.TableNoTracking
              .FirstOrDefault(x => x.Name.Equals("GiayPhanUngThuoc"));
                content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            }
            
            return content;
        }
        public string GetFormatDuocPham(string tenThuongMai)
        {
            var tenFormat = string.Empty;
            //GetThongTinDuocPham
            if (!string.IsNullOrEmpty(tenThuongMai))
            {
                var getDataDuocPham = _duocPhamRepository.TableNoTracking.Where(d => d.Ten == tenThuongMai)
                                       .Select(d => new GetThongTinDuocPham
                                       {
                                           Ten = tenThuongMai,
                                           HoatChat = d.HoatChat,
                                           DuocPhamBenhVienPhanNhomId = d.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                           HamLuong = d.HamLuong,
                                           LoaiThuocTheoQuanLy = d.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                                       });
                if(getDataDuocPham.Any())
                {
                    tenFormat = _yeuCauKhamBenhService.FormatTenDuocPham(getDataDuocPham.First().Ten, getDataDuocPham.First().HoatChat, getDataDuocPham.First().HamLuong, getDataDuocPham.First().DuocPhamBenhVienPhanNhomId);
                }
            }
            return tenFormat;
        }
        public string GetFormatSoLuong(string tenThuongMai,double soLuong)
        {
            var tenFormat = string.Empty;
            //GetThongTinDuocPham
            if (!string.IsNullOrEmpty(tenThuongMai))
            {
                var getDataDuocPham = _duocPhamRepository.TableNoTracking.Where(d => d.Ten == tenThuongMai)
                                       .Select(d => new GetThongTinDuocPham
                                       {
                                           Ten = tenThuongMai,
                                           HoatChat = d.HoatChat,
                                           DuocPhamBenhVienPhanNhomId = d.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                           HamLuong = d.HamLuong,
                                           LoaiThuocTheoQuanLy = d.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                           SoLuong = soLuong
                                       });
                if (getDataDuocPham.Any())
                {
                    tenFormat = _yeuCauKhamBenhService.FormatSoLuong(getDataDuocPham.First().SoLuong, getDataDuocPham.First().LoaiThuocTheoQuanLy);
                }
            }
            return tenFormat;
        }
        public string ShowThongTinDuocPham(string tenThuongMai)
        {
            var tenFormat = string.Empty;
            var tenDuocPham = string.Empty;
            //GetThongTinDuocPham
            if (!string.IsNullOrEmpty(tenThuongMai))
            {
                if (!string.IsNullOrEmpty(tenThuongMai))
                {
                    var duocpham = tenThuongMai.Split((','));
                    if (duocpham.Any())
                    {
                        tenDuocPham = duocpham.Count() > 1 ? duocpham[0].ToString() : tenThuongMai;
                    }

                }
                var getDataDuocPham = _duocPhamRepository.TableNoTracking.Where(d => d.Ten == tenDuocPham)
                                       .Select(d => new 
                                       {
                                           Ten = tenDuocPham,
                                           DonVi = d.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                           NuocSanXuat = d.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                           HamLuong = d.HamLuong,
                                           SoSanXuat = d.DuocPhamBenhVien.DuocPham.SoDangKy,
                                       });
                if (getDataDuocPham.Any())
                {

                    tenFormat = getDataDuocPham.First().Ten + (getDataDuocPham.First().HamLuong != null ? ", " + getDataDuocPham.First().HamLuong:"") +
                                                              (getDataDuocPham.First().DonVi != null ? ", " + getDataDuocPham.First().DonVi : "") +
                                                              (getDataDuocPham.First().NuocSanXuat != null ? ", " + getDataDuocPham.First().NuocSanXuat : "") +
                                                              (getDataDuocPham.First().SoSanXuat != null ? ", " + getDataDuocPham.First().SoSanXuat : "");
                }
            }
            return tenFormat;
        }
    }
}
