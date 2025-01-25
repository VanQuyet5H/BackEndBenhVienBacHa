using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
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
        public List<DanhSachDichVuKyThuatThuocVatTuGrid> GetListGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGridAsync(long yeuCauTiepNhanId) {
            var queryData = new List<DanhSachDichVuKyThuatThuocVatTuGrid>();
            var queryDataYeuCauDichVuKythuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                                                             .Include(s => s.NhomDichVuBenhVien)
                                                                             .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.YeuCauTiepNhan.NoiTruBenhAn != null && o.DuocHuongBaoHiem == false && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
            queryData.AddRange(queryDataYeuCauDichVuKythuat.Select(p => new DanhSachDichVuKyThuatThuocVatTuGrid
            {
                     Id = p.Id,
                     Nhom = p.NhomDichVuBenhVien.Ten,
                     DonGia = p.Gia,
                     SoLuong  = p.SoLan,
                     TenDichVu = p.TenDichVu,
                     TongTien = p.Gia * Convert.ToDecimal(p.SoLan)
                 }));
            var queryyDataThuocDichTruyen = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Include(d=>d.DuocPhamBenhVien).ThenInclude(d=>d.DuocPhamBenhVienPhanNhom)
                                                                   .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.YeuCauTiepNhan.NoiTruBenhAn != null && o.DuocHuongBaoHiem == false && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy);
            queryData.AddRange(queryyDataThuocDichTruyen.Select(p => new DanhSachDichVuKyThuatThuocVatTuGrid
            {
                //STT = stt++,
                Id = p.Id,
                Nhom = "THUỐC , DỊCH TRUYỀN",
                DonGia = p.KhongTinhPhi == true ? 0: p.DonGiaBan,
                SoLuong = (int)p.SoLuong,
                SoLuongDisPlay = FormatSoLuongNoiTru(p.SoLuong,p.DuocPhamBenhVien.LoaiThuocTheoQuanLy),
                TenDichVu = p.Ten,
                //p.Ten,
                TongTien = p.KhongTinhPhi == true ? 0:p.DonGiaBan * Convert.ToDecimal(p.SoLuong),
                FormatSoLuongDuocPhamGayNghienHuongThan = (p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan) ? true : false
            }));
            var queryyDataVatTuYTe= _yeuCauVatTuBenhVienRepository.TableNoTracking
                                                                 .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.YeuCauTiepNhan.NoiTruBenhAn != null && o.DuocHuongBaoHiem == false && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy);
            queryData.AddRange(queryyDataVatTuYTe.Select(p => new DanhSachDichVuKyThuatThuocVatTuGrid
            {
                //STT = stt++,
                Id = p.Id,
                Nhom = "VẬT TƯ Y TẾ",
                DonGia = p.KhongTinhPhi == true ? 0 : p.DonGiaBan,
                SoLuong = (int)p.SoLuong,
                TenDichVu = p.Ten,
                TongTien = p.KhongTinhPhi == true ? 0 : p.DonGiaBan * Convert.ToDecimal(p.SoLuong),
            }));
            var queryDataTruyenMau = _yeuCauTruyenMauRepository.TableNoTracking
                                                                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.YeuCauTiepNhan.NoiTruBenhAn != null && o.DuocHuongBaoHiem == false && o.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy);
            queryData.AddRange(queryDataTruyenMau.Select(p => new DanhSachDichVuKyThuatThuocVatTuGrid
            {
                //STT = stt++,
                Id = p.Id,
                Nhom = "THUỐC , DỊCH TRUYỀN",
                DonGia = p.DonGiaBan != null ? p.DonGiaBan.GetValueOrDefault() : 0,
                SoLuong = 1,
                TenDichVu = p.TenDichVu,
                TongTien = p.DonGiaBan.GetValueOrDefault() * 1,
                TruyenMau = true
            }));
            return queryData
                   .GroupBy(d => new { d.Nhom, d.TenDichVu, d.DonGia,d.TruyenMau })
                   .Select(d => new DanhSachDichVuKyThuatThuocVatTuGrid()
                   {
                       Nhom = d.First().Nhom,
                       DonGia = d.First().DonGia,
                       SoLuong = d.Sum(f => f.SoLuong),
                       TenDichVu = d.First().TenDichVu,
                       TongTien = d.Sum(g => g.TongTien),
                       TruyenMau = d.First().TruyenMau,
                       Id = d.First().Id
                   }).ToList();
        }
        public GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGridVo GetThongTinGiayCamKetTuNguyenSuDungThuocDVNgoaiBHYT(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBenhVien)
                                                                  .Select(s => new GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBenhVien,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGridVo()
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
        public async Task<string> InPhieuGiayCamKetTuNguyenSuDungThuoc(XacNhanInPhieuGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe xacNhanIn)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanIn.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InPhieuGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe>(thongtinIn);
            var content = "";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBHYT"));
            bool inLanDau = true;
            var giayCamKetTuNguyenSuDungThuoc = "";
            var queryGroupTheoNhom = queryString.DanhSachArrPrint.GroupBy(d => new { d.Nhom, d.TenDichVu, d.DonGia, d.TruyenMau })
                .Select(d => new DanhSachDichVuKyThuatThuocVatTuGrid()
                {
                    Nhom = d.First().Nhom,
                    DonGia = d.First().DonGia,
                    SoLuong = d.Sum(f => (int)f.SoLuong),
                    TenDichVu = d.First().TenDichVu,
                    TongTien = d.Sum(g => g.TongTien),
                    TruyenMau = d.First().TruyenMau,
                    Id = d.First().Id
                }).ToList();
            if (queryGroupTheoNhom.Count() > 0)
            {
                int stt = 1;
                foreach (var itemGroup in queryGroupTheoNhom.GroupBy(d=>d.Nhom))
                {
                    bool checkTitleFist = true;
                    
                    decimal tongTotalGroup = 0;
                    foreach (var item in itemGroup)
                    {
                        tongTotalGroup += item.TongTien;

                        if (checkTitleFist == true)
                        {
                            giayCamKetTuNguyenSuDungThuoc += "<tr>" +
                                                           "<td  colspan='5' style='width: 100%; border: 1px solid black;font-weight: bold;'>" + item.Nhom.ToUpper() + "</td>"
                                                       + "</tr>";
                            checkTitleFist = false;
                        }
                        item.STT = stt;
                        giayCamKetTuNguyenSuDungThuoc += "<tr>" +
                                                      "<td style='width: 5%; border: 1px solid black;'>" + item.STT + "</td>";

                        if (item.Nhom == "THUỐC , DỊCH TRUYỀN" && item.TruyenMau == false && (item.Id != 0 || item.Id != null))
                        {
                            var queryyDataThuocDichTruyen = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(d => d.Id == item.Id).Include(d => d.DuocPhamBenhVien).ThenInclude(d => d.DuocPhamBenhVienPhanNhom).FirstOrDefault();

                            if (queryyDataThuocDichTruyen != null)
                            {
                                giayCamKetTuNguyenSuDungThuoc += "<td style='width: 50%; border: 1px solid black;'>" + FormatTenDuocPhamNoiTru(queryyDataThuocDichTruyen, queryyDataThuocDichTruyen.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) + "</td>";
                                giayCamKetTuNguyenSuDungThuoc += "<th style='width: 5%; border: 1px solid black;'>" + (item.FormatSoLuongDuocPhamGayNghienHuongThan == true ? item.SoLuongDisPlay : item.SoLuong.ToString()) + "</td>"
                                                             + "<th style='width: 20%; border: 1px solid black;text-align: right;'>" + item.DonGia.ApplyFormatMoneyVND() + "</td>"
                                                             + "<th style='width: 20%; border: 1px solid black;text-align: right;'>" + item.TongTien.ApplyFormatMoneyVND() + "</td>"
                                                             + "</tr>";
                            }
                            else
                            {
                                giayCamKetTuNguyenSuDungThuoc += "<td style='width: 50%; border: 1px solid black;'>" + item.TenDichVu + "</td>";
                                giayCamKetTuNguyenSuDungThuoc += "<th style='width: 5%; border: 1px solid black;'>" + (item.FormatSoLuongDuocPhamGayNghienHuongThan == true ? item.SoLuongDisPlay : item.SoLuong.ToString()) + "</td>"
                                                             + "<th style='width: 20%; border: 1px solid black;text-align: right;'>" + item.DonGia.ApplyFormatMoneyVND() + "</td>"
                                                             + "<th style='width: 20%; border: 1px solid black;text-align: right;'>" + item.TongTien.ApplyFormatMoneyVND() + "</td>"
                                                             + "</tr>";
                            }

                        }
                        else
                        {
                            giayCamKetTuNguyenSuDungThuoc += "<td style='width: 50%; border: 1px solid black;'>" + item.TenDichVu + "</td>";
                            giayCamKetTuNguyenSuDungThuoc += "<th style='width: 5%; border: 1px solid black;'>" + (item.FormatSoLuongDuocPhamGayNghienHuongThan == true ? item.SoLuongDisPlay : item.SoLuong.ToString()) + "</td>"
                                                         + "<th style='width: 20%; border: 1px solid black;text-align: right;'>" + item.DonGia.ApplyFormatMoneyVND() + "</td>"
                                                         + "<th style='width: 20%; border: 1px solid black;text-align: right;'>" + item.TongTien.ApplyFormatMoneyVND() + "</td>"
                                                         + "</tr>";
                        }

                        stt++;
                        if (stt > itemGroup.Count())
                        {
                            giayCamKetTuNguyenSuDungThuoc += "<tr>" +
                                                           "<td  colspan='5' style='width: 100%; border: 1px solid black;text-align: right;font-weight: bold;'>" + tongTotalGroup.ApplyFormatMoneyVND() + "</td>"
                                                       + "</tr>";
                        }
                    }
                }
            }
            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new
                                                                              {
                                                                                  LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                                                                                  ToiTenLa = queryString.SelectBenhNhanHoacNguoiNha == true ? x.YeuCauTiepNhan.HoTen : queryString.HoTen,
                                                                                  QuanHeBenhNhan = "",
                                                                                  HoTenNguoiBenh = x.YeuCauTiepNhan.HoTen,
                                                                                  GioiTinh = x.YeuCauTiepNhan.GioiTinh.GetValueOrDefault().GetDescription(),
                                                                                  NamSinh = x.YeuCauTiepNhan.NamSinh,
                                                                                  DiaChi = x.YeuCauTiepNhan.DiaChiDayDu,
                                                                                  SoTheBHYT = x.YeuCauTiepNhan.BHYTMaSoThe,
                                                                                  HanSuDung = x.YeuCauTiepNhan.BHYTMaSoThe != null ? "Từ:" + x.YeuCauTiepNhan.BHYTNgayHieuLuc.GetValueOrDefault().ApplyFormatDate() + "đến:" + x.YeuCauTiepNhan.BHYTNgayHetHan.GetValueOrDefault().ApplyFormatDate() : "Từ:" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "đến:",
                                                                                  SoGiuong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan) ? x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.OrderByDescending(p => p.Id).FirstOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.Ten : "",
                                                                                  Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(p => p.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                  ChanDoan = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(s => s.Id).Select(s => s.ChanDoanVaoKhoaGhiChu).FirstOrDefault(),
                                                                                  GiayCamKetTuNguyenSuDungThuoc = giayCamKetTuNguyenSuDungThuoc,
                                                                                  NgayHienTai = DateTime.Now.Day,
                                                                                  ThangHienTai = DateTime.Now.Month,
                                                                                  NamHienTai = DateTime.Now.Year
                                                                              }).FirstOrDefault();

            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }

        //BVHD-3959: Chỉnh lại logic hiển thị tên thuốc theo thông tư 52/2017: 1 hoạt chất và nhiều hoạt chất (từ 2 hoạt chất trở lên)
        public string FormatTenDuocPhamNoiTru(YeuCauDuocPhamBenhVien p, long? duocPhamBenhVienPhanNhomId)
        {
            string tenThuongMai = p.Ten;
            string tenQuocTe = p.HoatChat;
            string hamLuong = p.HamLuong;
            var dp = string.Empty;
            // kiểm tra dược phẩm có mấy hoạt chất
            var soLuongHoatChat = SoluongHoatChatCuaDuocPhamNoiTru(tenQuocTe);
            // dược phẩm Phan nhóm == Sinh phẩm hoặc sinh phẩm chẩn đoán
            var valueDuocPhamPhanNhom = ValueDuocPhamBenhVienNoiTru(duocPhamBenhVienPhanNhomId);
            if (valueDuocPhamPhanNhom == true)
            {
                dp = tenThuongMai + " " + hamLuong;
            }
            else
            {
                if (soLuongHoatChat == 1)
                {
                    //1.1 Tên thương mại trùng với hoạt chất thì hiển thị là: Hoạt chất_Hàm lượng
                    //Ví dụ: Thuốc Paracetamol có hoạt chất Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol 500mg
                    if (tenThuongMai == tenQuocTe)
                    {
                        dp = tenThuongMai + " " + hamLuong;
                    }
                    //1.2 Tên thương mại không trùng với hoạt chất thì hiển thị là: Hoạt chất(Tên thương mại) Hàm lượng
                    //Ví dụ: Thuốc Paradol có hoạt chất là Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol (Paradol) 500mg
                    if (tenThuongMai != tenQuocTe)
                    {
                        dp = tenQuocTe + " " + "<span style='font-weight: bold;'>" + "(" + tenThuongMai + ")" + "</span>" + " " + hamLuong;
                    }
                }
                else
                {
                    dp = tenThuongMai + " " + hamLuong;
                }
            }
            return dp;
        }

        #region  BVHD-3560
        //        Dược phẩm có 1 hoạt chất:

        //1.1 Tên thương mại trùng với hoạt chất thì hiển thị là: Hoạt chất_Hàm lượng
        //Ví dụ: Thuốc Paracetamol có hoạt chất Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol 500mg
        //1.2 Tên thương mại không trùng với hoạt chất thì hiển thị là: Hoạt chất(Tên thương mại )_Hàm lượng
        //Ví dụ: Thuốc Paradol có hoạt chất là Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol(Paradol ) 500mg

        //Dược phẩm có nhiều hoạt chất:

        //2.1 Thuốc có 2 hoạt chất: cách hiển thị giống như phần 1.2
        //Ví dụ: Thuốc A có hoạt chất 1 và hoạt chất 2, hàm lượng 500mg thì hiển thị là: Hoạt chất 1 + Hoạt chất 2 (A ) 500mg
        //2.2 Thuốc có từ 3 hoạt chất trở lên thì hiển thị là: Tên thương mại_Hàm lượng

        //Dược phẩm được phân loại thuốc hoặc hoạt chất là

        //Sinh phẩm hoặc sinh phẩm chẩn đoán trong Danh mục dược phẩm bệnh viện thì hiển thị là: Tên thương mại_Hàm lượng
        //public string FormatTenDuocPhamNoiTru(YeuCauDuocPhamBenhVien p,string tenDuocPham) // =>  tenThuongMai => tên dược phẩm , tenQuocTe => hoạt chất
        //{
        //    string tenThuongMai = p.Ten;
        //    string tenQuocTe = p.HoatChat;
        //    string hamLuong = p.HamLuong;
        //    string tenDuocPhamBenhVienPhanNhom = tenDuocPham;
        //    var dp = string.Empty;
        //    // kiểm tra dược phẩm có mấy hoạt chất
        //    var soLuongHoatChat = SoluongHoatChatCuaDuocPhamNoiTru(tenQuocTe);
        //    var listHoatChat = GetListHoatChatNoiTru(tenQuocTe);
        //    // dược phẩm Phan nhóm == Sinh phẩm hoặc sinh phẩm chẩn đoán
        //    var valueDuocPhamPhanNhom = ValueDuocPhamBenhVienNoiTru(tenDuocPhamBenhVienPhanNhom);
        //    if (valueDuocPhamPhanNhom == true)
        //    {
        //        dp = tenThuongMai + " " + hamLuong;
        //    }
        //    else
        //    {
        //        if (soLuongHoatChat == 1)
        //        {
        //            //1.1 Tên thương mại trùng với hoạt chất thì hiển thị là: Hoạt chất_Hàm lượng
        //            //Ví dụ: Thuốc Paracetamol có hoạt chất Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol 500mg
        //            if (tenThuongMai == tenQuocTe)
        //            {
        //                dp = tenThuongMai + " " + hamLuong;
        //            }
        //            //1.2 Tên thương mại không trùng với hoạt chất thì hiển thị là: Hoạt chất(Tên thương mại )_Hàm lượng
        //            //Ví dụ: Thuốc Paradol có hoạt chất là Paracetamol và hàm lượng 500mg thì hiển thị là: Paracetamol(Paradol ) 500mg
        //            if (tenThuongMai != tenQuocTe)
        //            {
        //                dp = tenQuocTe + " " + "<span style='text-transform: uppercase;font-weight: bold;'>" + "(" + tenThuongMai + ")" + "</span>" + " " + hamLuong;
        //            }
        //        }
        //        else if (soLuongHoatChat == 2)
        //        {
        //            //2.1 Thuốc có 2 hoạt chất: cách hiển thị giống như phần 1.2
        //            //Ví dụ: Thuốc A có hoạt chất 1 và hoạt chất 2, hàm lượng 500mg thì hiển thị là: Hoạt chất 1 + Hoạt chất 2 (A ) 500mg
        //            if (listHoatChat.Any(d => d == tenThuongMai))
        //            {
        //                dp = tenThuongMai + " " + hamLuong;  // tên thương mại tồn tại trong list hoat chất => 1.1
        //            }
        //            else
        //            {
        //                dp = listHoatChat[0] + " + " + listHoatChat[1] + " " + "<span style='text-transform: uppercase;font-weight: bold;'>" + "(" + tenThuongMai + ")" + "</span>" + " " + hamLuong;
        //            }
        //        }
        //        else if (soLuongHoatChat > 2)
        //        {
        //            //2.2 Thuốc có từ 3 hoạt chất trở lên thì hiển thị là: Tên thương mại_Hàm lượng
        //            dp = tenThuongMai + " " + hamLuong;
        //        }
        //        else
        //        {
        //            dp = tenThuongMai + " " + hamLuong;
        //        }
        //    }


        //    return dp;
        //}
        
        public int SoluongHoatChatCuaDuocPhamNoiTru(string hoatChat)
        {
            if (!string.IsNullOrEmpty(hoatChat))
            {
                var slHC = hoatChat.Split('+');
                return slHC.Length;
            }

            return 0;
        }
        public List<string> GetListHoatChatNoiTru(string hoatChat)
        {
            var lst = new List<string>();
            if (!string.IsNullOrEmpty(hoatChat))
            {
                var slHC = hoatChat.Split('+');
                foreach (var item in slHC)
                {
                    lst.Add(item);
                }
            }
            return lst;
        }
        //BVHD-3959: tối ưu code
        public bool ValueDuocPhamBenhVienNoiTru(long? duocphamBenhVienPhanNhomId)
        {
            if (duocphamBenhVienPhanNhomId != null)
            {
                string duocPhamBenhVienPhanNhomSinhPhamIds = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhChung.DuocPhamBenhVienPhanNhomSinhPhamIds").Select(s => s.Value).FirstOrDefault();
                if (!string.IsNullOrEmpty(duocPhamBenhVienPhanNhomSinhPhamIds) && duocPhamBenhVienPhanNhomSinhPhamIds.Split(';').Contains(duocphamBenhVienPhanNhomId.ToString()))
                {
                    return true;
                }
            }
            return false;
        }
        public bool ValueDuocPhamBenhVienNoiTru(string tenDuocPhamPhanNhom)
        {
            bool value = false;
            string sinhPham = "Sinh phẩm";
            string sinhPhamChanDoan = "sinh phẩm chẩn đoán";
            if (tenDuocPhamPhanNhom != null)
            {
                //var duocPhamPhanNhom = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.Where(d => d.Id == duocphamBenhVienPhanNhomId);
                if (tenDuocPhamPhanNhom == sinhPham || tenDuocPhamPhanNhom == sinhPhamChanDoan)
                {
                    value = true;
                }
            }
            return value;
        }
        //        Với số lượng thuốc được kê nhỏ hơn 10 (<10) , hệ thống hiển thị thêm số 0 đằng trước

        //VD: số lượng thuốc là 5 -> Hiển thị ttrong đơn là 05

        //Đối với thuốc gây nghiện: số lượng thuốc phải viết bằng chữ, chữ đầu viết hoa

        //VD: số lượng thuốc 10 ->Hiển thị: Mười
        // format số lượng 
        public string FormatSoLuongNoiTru(double soLuong, Enums.LoaiThuocTheoQuanLy? loaiThuoc)
        {
            var slDP = string.Empty;

            if (soLuong == 0)
            {
                if (loaiThuoc != Enums.LoaiThuocTheoQuanLy.GayNghien && loaiThuoc != Enums.LoaiThuocTheoQuanLy.HuongThan)
                {
                    slDP = "0";
                }
                else
                {
                    slDP = NumberHelper.ChuyenSoRaText(soLuong, false);
                }
            }
            else if (soLuong < 10 && soLuong > 0)
            {
                if (loaiThuoc != Enums.LoaiThuocTheoQuanLy.GayNghien && loaiThuoc != Enums.LoaiThuocTheoQuanLy.HuongThan)
                {
                    slDP = "0" + soLuong;
                }
                else
                {
                    slDP = NumberHelper.ChuyenSoRaText(soLuong, false);
                }

            }
            else if (soLuong >= 10) //&& (loaiThuoc == null || (loaiThuoc != Enums.LoaiThuocTheoQuanLy.GayNghien && loaiThuoc != Enums.LoaiThuocTheoQuanLy.HuongThan)))
            {
                if (loaiThuoc == Enums.LoaiThuocTheoQuanLy.GayNghien || loaiThuoc == Enums.LoaiThuocTheoQuanLy.HuongThan)
                {
                    slDP = NumberHelper.ChuyenSoRaText(soLuong, false);
                }
                else
                {
                    slDP = "" + soLuong;
                }
            }
            return slDP;
        }

        #endregion BVHD-3560
    }
}
