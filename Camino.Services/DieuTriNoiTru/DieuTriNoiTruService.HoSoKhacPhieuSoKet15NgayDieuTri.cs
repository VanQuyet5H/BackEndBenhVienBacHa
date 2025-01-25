using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhieuSoKet15NgayDieuTri;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Globalization;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public PhieuSoKet15NgayDieuTriVo GetThongTinPhieuSoKet15NgayDieuTri(long idNguoiLogin, long yeuCauTiepNhanId)
        {
            var ngayHienTai = new DateTime();
            ngayHienTai = DateTime.Now;
            var query = BaseRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => new
            {
                SoYTe = "HÀ NỘI",
                BV = "ĐKQT BẮC HÀ",
                SoVaoVien = s.MaYeuCauTiepNhan,
                HoTenNgBenh = s.HoTen,
                Tuoi = DateTime.Now.Year - s.NamSinh,
                GioiTinh = s.GioiTinh.GetDescription(),
                DiaChi = s.DiaChi,
                taiSoGiuong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.Ten).FirstOrDefault(),
                phong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                Khoa = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault(),
                chanDoan = s.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(x => x.Id).Select(x => x.ChanDoanVaoKhoaGhiChu).FirstOrDefault(),
            }).FirstOrDefault();

            var nguoiLoginNgayThucHien = _nhanVienRepository.TableNoTracking.Where(x => x.Id == idNguoiLogin).Select(s => new PhieuSoKet15NgayDieuTriVo()
            {
                SoYTe = query.SoYTe,
                BV = query.BV,
                SoVaoVien = query.SoVaoVien,
                HoTenNgBenh = query.HoTenNgBenh,
                TuoiNgBenh = query.HoTenNgBenh,
                GTNgBenh = query.GioiTinh,
                DiaChi = query.DiaChi,
                TaiKhoanDangNhap = s.User.HoTen,
                Khoa = query.Khoa,
                ChanDoan = query.chanDoan,
                Buong = query.phong,
                Giuong = query.taiSoGiuong,
                NgayThucHienText = ngayHienTai.ApplyFormatDateTime()
            }).FirstOrDefault();
            return nguoiLoginNgayThucHien;
        }
        public TrichBienBanHoiChanGridVo GetThongTinPhieuSoKet15NgayDieuTriSave(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri)
                                                                  .Select(s => new TrichBienBanHoiChanGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyBangKiemAnToanGridVo()
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
                                                                  }).OrderBy(z=>z.Id).LastOrDefault();
            return query;
        }
        public TrichBienBanHoiChanGridVo GetThongTinPhieuSoKet15NgayDieuTriViewDS(long noiTruHoSoKhacId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == noiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri)
                                                                  .Select(s => new TrichBienBanHoiChanGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyBangKiemAnToanGridVo()
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
        public async Task<string> PhieuSoKet15NgayDieuTri(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriVo>(thongtinIn);
            var content = "";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuSoKet15NgayDieuTri"));
            DateTime tuNgayPart = DateTime.Now;
            xacNhanInTrichBienBanHoiChan.TuNgay.TryParseExactCustom(out tuNgayPart);
            //DateTime.TryParseExact(xacNhanInTrichBienBanHoiChan.TuNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgayPart);
            DateTime denNgayPart = DateTime.Now;
            xacNhanInTrichBienBanHoiChan.DenNgay.TryParseExactCustom(out denNgayPart);
            //DateTime.TryParseExact(xacNhanInTrichBienBanHoiChan.DenNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgayPart);
            // texarea xuong hang enter
            var tmp = "\n";
            var replace = "<br>";
            string chanDoan = "";
            string dienBienLS = "";
            string xetNghiemCLS = "";
            string quaTrinhDieuTri = "";
            string danhGiaKetQua = "";
            string huongDieuTriVaTienLuong = "";
            if (queryString.ChanDoan != null)
            {
                chanDoan = queryString.ChanDoan.Replace(tmp, replace);
            }
            if (queryString.DienBienLS != null)
            {
                dienBienLS = queryString.DienBienLS.Replace(tmp, replace);
            }
            if (queryString.XetNghiemCLS != null)
            {
                xetNghiemCLS = queryString.XetNghiemCLS.Replace(tmp, replace);
            }
            if (queryString.QuaTrinhDieuTri != null)
            {
                quaTrinhDieuTri = queryString.QuaTrinhDieuTri.Replace(tmp, replace);
            }
            if (queryString.DanhGiaKetQua != null)
            {
                danhGiaKetQua = queryString.DanhGiaKetQua.Replace(tmp, replace);
            }
            if (queryString.HuongDieuTriTiep != null)
            {
                huongDieuTriVaTienLuong = queryString.HuongDieuTriTiep.Replace(tmp, replace);
            }
            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new 
                                                                              {
                                                                                  SoYTe = queryString.SoYTe,
                                                                                  BV = queryString.BV,
                                                                                  SoVaoVien = x.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                                                                                  HoTenNgBenh = queryString.HoTenNgBenh,
                                                                                  TuoiNgBenh = DateTime.Now.Year - x.YeuCauTiepNhan.NamSinh,
                                                                                  GTNgBenh = x.YeuCauTiepNhan.GioiTinh != null ? x.YeuCauTiepNhan.GioiTinh.GetDescription():"",
                                                                                  DiaChi = x.YeuCauTiepNhan.DiaChiDayDu,
                                                                                  Khoa = queryString.Khoa,
                                                                                  Buong = queryString.Buong,
                                                                                  Giuong = queryString.Giuong,
                                                                                  ChanDoan = chanDoan,
                                                                                  DienBienLS = dienBienLS,
                                                                                  XetNghiemCLS = xetNghiemCLS,
                                                                                  QuaTrinhDieuTri = quaTrinhDieuTri,
                                                                                  DanhGiaKetQua = danhGiaKetQua,
                                                                                  HuongDieuTriTiep = huongDieuTriVaTienLuong,
                                                                                  NgayK = DateTime.Now.Day,
                                                                                  ThangK = DateTime.Now.Month,
                                                                                  NamK = DateTime.Now.Year,
                                                                                  Ngay = DateTime.Now.Day,
                                                                                  Thang = DateTime.Now.Month,
                                                                                  Nam = DateTime.Now.Year,
                                                                                  HoTenTruongKhoa = queryString.TruongKhoa,
                                                                                  HoTenBacSi = queryString.BSDieuTri,
                                                                                  TuNgay= tuNgayPart.ApplyFormatDateTimeSACH(),
                                                                                  DenNgay = denNgayPart.ApplyFormatDateTimeSACH()
                                                                              }).FirstOrDefault();
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        public string GetChanDoanVaoVien15Ngay(long yeuCauTiepNhanId)
        {
            var result = ThongTinBenhNhan15Ngay(yeuCauTiepNhanId);
            return result.Result.ChanDoan;
        }
        private async Task<DataInPhieuDieuTri15NgayVaSerivcesVo> ThongTinBenhNhan15Ngay(long yeuCauTiepNhanId)
        {
            var thongTinBenhNhanPhieuThuoc = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(s => s.Id == yeuCauTiepNhanId)
                .Select(s => new DataInPhieuDieuTri15NgayVaSerivcesVo
                {
                    HoTenNgBenh = s.HoTen,
                    NamSinh = s.NamSinh,
                    NgaySinh =s.NgaySinh,
                    ThangSinh = s.ThangSinh,
                    GTNgBenh = s.GioiTinh.GetDescription(),
                    GioiTinh = s.GioiTinh,
                    DiaChi = s.BenhNhan.DiaChiDayDu,
                    Cmnd = s.SoChungMinhThu,
                    MaBn = s.BenhNhan.MaBN,
                    NhomMau = s.NhomMau != null ? s.NhomMau.GetDescription() : string.Empty,
                    MaSoTiepNhan = s.MaYeuCauTiepNhan,
                    NgayVaoVien = s.NoiTruBenhAn.ThoiDiemNhapVien,
                    NgayRaVien = s.NoiTruBenhAn.ThoiDiemRaVien,
                    ChanDoanRaVien = s.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu,
                    ChanDoanVaoVien = s.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    Buong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                    Giuong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.Ten).FirstOrDefault(),
                    YeuCauTiepNhanId =s.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    YeuCauKhamBenhId= s.YeuCauNhapVienId != null ? s.YeuCauNhapVien.YeuCauKhamBenhId : null
                }).First();

            if(thongTinBenhNhanPhieuThuoc.YeuCauTiepNhanId != null)
            {

                var cdkt = _yeuCauKhamBenhRepository.TableNoTracking.Where(d => d.Id == thongTinBenhNhanPhieuThuoc.YeuCauKhamBenhId)
                             .Select(d => new {
                                 ChanDoanChinh = d.GhiChuICDChinh,
                                 ChanDoanKemTheos = d.YeuCauKhamBenhICDKhacs.Where(g=>g.ICDId != null).Select(dg => dg.ICD.TenTiengViet).ToList()
                             }).FirstOrDefault();


                if (cdkt.ChanDoanChinh != null)
                {
                    thongTinBenhNhanPhieuThuoc.ChanDoan += cdkt.ChanDoanChinh;
                }
                if (cdkt.ChanDoanKemTheos != null)
                {
                    var cdktd = cdkt.ChanDoanKemTheos.Join(", ");
                    if (!string.IsNullOrEmpty(cdktd))
                    {


                        thongTinBenhNhanPhieuThuoc.ChanDoan += "(Chẩn đoán kèm theo:" + cdktd + ")";
                    }
                    
                }
            }

           
            return  thongTinBenhNhanPhieuThuoc;
        }
        public async Task<string> PhieuSoKet15NgayDieuTriUpdate(PhieuDieuTriVaServicesHttpParams15Ngay xacNhanInTrichBienBanHoiChan)
        {
            var infoBn = await ThongTinBenhNhan15Ngay(xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId);

            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriVo>(thongtinIn);
            var content = "";

            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuSoKet15NgayDieuTri"));


            var tmp = "\n";
            var replace = "<br>";

            if (queryString.ChanDoan != null)
            {
                queryString.ChanDoan = queryString.ChanDoan.Replace(tmp, replace);
            }
            if (queryString.DienBienLS != null)
            {
                queryString.DienBienLS = queryString.DienBienLS.Replace(tmp, replace);
            }
            if (queryString.XetNghiemCLS != null)
            {
                queryString.XetNghiemCLS = queryString.XetNghiemCLS.Replace(tmp, replace);
            }
            if (queryString.QuaTrinhDieuTri != null)
            {
                queryString.QuaTrinhDieuTri = queryString.QuaTrinhDieuTri.Replace(tmp, replace);
            }
            if (queryString.DanhGiaKQ != null)
            {
                queryString.DanhGiaKQ = queryString.DanhGiaKQ.Replace(tmp, replace);
            }
            if (queryString.HuongDieuTriTiep != null)
            {
                queryString.HuongDieuTriTiep = queryString.HuongDieuTriTiep.Replace(tmp, replace);
            }

            var data = new In15NgayVo();
            data.BarCodeImgBase64 = !string.IsNullOrEmpty(infoBn.MaSoTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(infoBn.MaSoTiepNhan.ToString()) : "";


            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();

            data.KhoaDangIn = tenKhoa;

            var tuNgay = string.Empty;
            if (!string.IsNullOrEmpty(queryString.TuNgayString))
            {
                DateTime ngay = DateTime.Now;
                DateTime.TryParseExact(queryString.TuNgayString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngay);
                data.TuNgay = $"&nbsp;{ngay.ApplyFormatDate()}&nbsp;";
            }

            var denNgay = string.Empty;
            if (!string.IsNullOrEmpty(queryString.DenNgayString))
            {
                DateTime ngay = DateTime.Now;
                DateTime.TryParseExact(queryString.DenNgayString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngay);
                data.DenNgay = $"&nbsp;{ngay.ApplyFormatDate()}";
            }

            data.MaTN = infoBn.MaSoTiepNhan;


            data.HoTenNgBenh = !string.IsNullOrEmpty(infoBn.HoTenNgBenh) ? "<div class='container'>"+
                                                                              "<div class='label'>- Họ tên người bệnh:&nbsp;</div>" +
                                                                              $"<div class='values'><b>{infoBn.HoTenNgBenh}</b></div>"+
                                                                              "</div>"
                                                                         : "<div class='container'>" +
                                                                              "<div class='label'>- Họ tên người bệnh:&nbsp;</div>" +
                                                                              $"<div class='value'><b>&nbsp;</b></div>" +
                                                                              "</div>";

           



            var ns = DateHelper.DOBFormat(infoBn.NgaySinh, infoBn.ThangSinh, infoBn.NamSinh);



            data.HoTenNgBenh = !string.IsNullOrEmpty(infoBn.HoTenNgBenh) ? "<div class='container'>" +
                                                                             "<div class='label'>Họ tên NB:&nbsp;</div>" +
                                                                             $"<div class='values'><b>{infoBn.HoTenNgBenh}</b></div>" +
                                                                             "</div>"
                                                                        : "<div class='container'>" +
                                                                             "<div class='label'>Họ tên NB:&nbsp;</div>" +
                                                                             $"<div class='value'><b>&nbsp;</b></div>" +
                                                                             "</div>";

            data.TuoiNgBenh = !string.IsNullOrEmpty(ns) ? "<div class='container'>" +
                                                                            "<div class='label'>Ngày/tháng/năm sinh:&nbsp;</div>" +
                                                                            $"<div class='values'><b>{ns}</b></div>" +
                                                                            "</div>"
                                                                       : "<div class='container'>" +
                                                                            "<div class='label'>Ngày/tháng/năm sinh:&nbsp;</div>" +
                                                                            $"<div class='value'><b>&nbsp;</b></div>" +
                                                                            "</div>";

            data.GT = !string.IsNullOrEmpty(infoBn.GioiTinh?.GetDescription()) ? "<div class='container'>" +
                                                                   "<div class='label'>Giới tính:&nbsp;</div>" +
                                                                   $"<div class='values'><b>{infoBn.GioiTinh?.GetDescription()}</b></div>" +
                                                                   "</div>"
                                                                : "<div class='container'>" +
                                                                   "<div class='label'>Giới tính:&nbsp;</div>" +
                                                                   $"<div class='value'><b>&nbsp;</b></div>" +
                                                                   "</div>";

            data.DiaChi = !string.IsNullOrEmpty(infoBn.DiaChi) ? "<div class='container'>" +
                                                                   "<div class='label'>Địa chỉ:&nbsp;</div>" +
                                                                   $"<div class='values'>{infoBn.DiaChi}</div>" +
                                                                   "</div>"
                                                                : "<div class='container'>" +
                                                                   "<div class='label'>Địa chỉ:&nbsp;</div>" +
                                                                   $"<div class='value'>&nbsp;</div>" +
                                                                   "</div>";

            data.Khoa = !string.IsNullOrEmpty(tenKhoa) ? "<div class='container'>" +
                                                                 "<div class='label'>Khoa:&nbsp;</div>" +
                                                                 $"<div class='values'>{tenKhoa}</div>" +
                                                                 "</div>"
                                                              : "<div class='container'>" +
                                                                 "<div class='label'>Khoa:&nbsp;</div>" +
                                                                 $"<div class='value'>&nbsp;</div>" +
                                                                 "</div>";


            data.Buong = !string.IsNullOrEmpty(infoBn.Buong) ? "<div class='container'>" +
                                                                 "<div class='label'>Buồng:&nbsp;</div>" +
                                                                 $"<div class='values'>{infoBn.Buong}</div>" +
                                                                 "</div>"
                                                              : "<div class='container'>" +
                                                                 "<div class='label'>Buồng:&nbsp;</div>" +
                                                                 $"<div class='value'>&nbsp;</div>" +
                                                                 "</div>";



            var giuong = string.Empty;

            if(infoBn.Giuong != null )
            {
                giuong = infoBn.Giuong.Replace("Giường", "");
                giuong = giuong.Replace("GIƯỜNG", "");
            }

            data.Giuong = !string.IsNullOrEmpty(giuong) ? "<div class='container'>" +
                                                                 "<div class='label'>Giường:&nbsp;</div>" +
                                                                 $"<div class='values'>{giuong}</div>" +
                                                                 "</div>"
                                                              : "<div class='container'>" +
                                                                 "<div class='label'>Giường:&nbsp;</div>" +
                                                                 $"<div class='value'>&nbsp;</div>" +
                                                                 "</div>";

            data.ChanDoan = !string.IsNullOrEmpty(infoBn.ChanDoan) ? "<div class='container'>" +
                                                                "<div class='label'></div>" +
                                                                $"<div class='values'>Chẩn đoán:&nbsp;{queryString.ChanDoan}</div>" +
                                                                "</div>"
                                                             : "<div class='container'>" +
                                                                "<div class='label'>Chẩn đoán:&nbsp;</div>" +
                                                                $"<div class='value'>&nbsp;</div>" +
                                                                "</div>";

            data.DienBienLS = !string.IsNullOrEmpty(queryString.DienBienLS) ? "<div class='container'>" +
                                                                "<div class='label'>1. </div>" +
                                                                $"<div class='values'><b>Diễn biến lâm sàng trong đợt điều trị:&nbsp;</b>{queryString.DienBienLS}</div>" +
                                                                "</div>"
                                                             : "<div class='container'>" +
                                                                "<div class='label'><b>1. Diễn biến lâm sàng trong đợt điều trị:</b> </div>" +
                                                                $"<div class='value'>&nbsp;</div>" +
                                                                "</div>";
            data.XetNghiemCLS = !string.IsNullOrEmpty(queryString.XetNghiemCLS) ? "<div class='container'>" +
                                                                "<div class='label'>2.  </div>" +
                                                                $"<div class='values'><b>Xét nghiệm cận lâm sàng:&nbsp;</b>{queryString.XetNghiemCLS}</div>" +
                                                                "</div>"
                                                             : "<div class='container'>" +
                                                                "<div class='label'><b>2. Xét nghiệm cận lâm sàng:</b> </div>" +
                                                                $"<div class='value'>&nbsp;</div>" +
                                                                "</div>";

            data.QuaTrinhDieuTri = !string.IsNullOrEmpty(queryString.QuaTrinhDieuTri) ? "<div class='container'>" +
                                                                "<div class='label'>3.  </div>" +
                                                                $"<div class='values'><b>Quá trình điều trị:&nbsp;</b>{queryString.QuaTrinhDieuTri}</div>" +
                                                                "</div>"
                                                             : "<div class='container'>" +
                                                                "<div class='label'><b>3. Quá trình điều trị: </b></div>" +
                                                                $"<div class='value'>&nbsp;</div>" +
                                                                "</div>";

            data.DanhGiaKetQua = !string.IsNullOrEmpty(queryString.DanhGiaKQ) ? "<div class='container'>" +
                                                               "<div class='label'>4.   </div>" +
                                                               $"<div class='values'><b>Đánh giá kết quả:&nbsp;</b>{queryString.DanhGiaKQ}</div>" +
                                                               "</div>"
                                                            : "<div class='container'>" +
                                                               "<div class='label'><b>4. Đánh giá kết quả:</b></div>" +
                                                               $"<div class='value'>&nbsp;</div>" +
                                                               "</div>";

            data.HuongDieuTriTiep = !string.IsNullOrEmpty(queryString.HuongDieuTriTiep) ? "<div class='container'>" +
                                                             "<div class='label'>5.   </div>" +
                                                             $"<div class='values'><b>Hướng điều trị tiếp và tiên lượng:&nbsp;</b>{queryString.HuongDieuTriTiep}</div>" +
                                                             "</div>"
                                                          : "<div class='container'>" +
                                                             "<div class='label'><b>5. Hướng điều trị tiếp và tiên lượng:</b>  </div>" +
                                                             $"<div class='value'>&nbsp;</div>" +
                                                             "</div>";


            if (!string.IsNullOrEmpty(queryString.NgayThucHienString))
            {
                DateTime ngay = DateTime.Now;
                DateTime.TryParseExact(queryString.NgayThucHienString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngay);

                data.Ngay = ngay.Day > 9 ? ngay.Day + "" : "0" + ngay.Day;
                data.Thang = ngay.Month > 9 ? ngay.Month + "" : "0" + ngay.Month;
                data.Nam = ngay.Year > 9 ? ngay.Year + "" : "0" + ngay.Year;

                data.NgayK = ngay.Day > 9 ? ngay.Day + "" : "0" + ngay.Day;
                data.ThangK = ngay.Month > 9 ? ngay.Month + "" : "0" + ngay.Month;
                data.NamK = ngay.Year > 9 ? ngay.Year + "" : "0" + ngay.Year;

            }

            if(queryString.NhanVienTrongBVHayNgoaiBV != null && queryString.NhanVienTrongBVHayNgoaiBV == true)
            {
                if(queryString.BSDieuTriId != null)
                {
                    var infoNhanVien = _nhanVienRepository.TableNoTracking.Where(d => d.Id == queryString.BSDieuTriId)
                                                                          .Select(d => (d.HocHamHocViId != null ? d.HocHamHocVi.Ten : "")).FirstOrDefault();

                    data.HoTenBacSi = (!string.IsNullOrEmpty(infoNhanVien) ? infoNhanVien + ". " :"") + queryString.BSDieuTri;
                }
            }
            else
            {
                data.HoTenBacSi = (!string.IsNullOrEmpty(queryString.HocHamHocViBsDieuTri) ? queryString.HocHamHocViBsDieuTri + ". " : "") + queryString.BSDieuTri;
            }


            if (queryString.NhanVienTrongBVHayNgoaiBVTruongKhoa != null && queryString.NhanVienTrongBVHayNgoaiBVTruongKhoa == true)
            {
                if (queryString.TruongKhoaId != null)
                {
                    var infoNhanVien = _nhanVienRepository.TableNoTracking.Where(d => d.Id == queryString.TruongKhoaId)
                                                                          .Select(d => (d.HocHamHocViId != null ? d.HocHamHocVi.Ten : "")).FirstOrDefault();

                    data.HoTenTruongKhoa = (!string.IsNullOrEmpty(infoNhanVien) ? infoNhanVien + ". " : "") + queryString.TruongKhoa;
                }
            }
            else
            {
                data.HoTenTruongKhoa = (!string.IsNullOrEmpty(queryString.HocHamHocViTruongKhoa) ? queryString.HocHamHocViTruongKhoa + ". " : "") + queryString.TruongKhoa;
            }

            //data.HoTenBacSi = queryString.BSDieuTri;
            //data.HoTenTruongKhoa = queryString.TruongKhoa;



            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        public async Task<GridDataSource> GetDanhSachPhieuSoKet15NgayDieuTri(QueryInfo queryInfo)
        {
            //
            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri)
                 .Select(s => new DanhSachSoKet15NgayGridVo()
            {
                Id = s.Id,
                ThongTinHoSo = s.ThongTinHoSo
            }).ToList();
           
            var dataOrderBy = query.AsQueryable().OrderBy(cc => cc.Id);
            var quaythuoc =  dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();
            foreach(var item in quaythuoc)
            {
                var queryString = JsonConvert.DeserializeObject<PhieuInSoKet15NgayDieuTriVo>(item.ThongTinHoSo);
                DateTime tuNgay = Convert.ToDateTime(queryString.TuNgay, CultureInfo.InvariantCulture);
                DateTime denNgay = Convert.ToDateTime(queryString.DenNgay, CultureInfo.InvariantCulture);
               
                item.TuNgay = tuNgay;
                item.DenNgay = denNgay;
            }
            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
        }

        public string GetTenDangNhap()
        {
            var nhanVienDangNhap = _userAgentHelper.GetCurrentUserId();
            var ten = _nhanVienRepository.TableNoTracking.Where(x => x.Id == nhanVienDangNhap).Select(x => x.User.HoTen).FirstOrDefault();
            return ten;
        }
        #region validate
        public async Task<bool> KiemTraNgay(DateTime? tuNgay, DateTime? denNgay)
        {
            if (tuNgay != null && denNgay != null)
            {
                if (denNgay < tuNgay)
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<bool> KiemTraNgayNhan(DateTime? ngayNhan , long yeuCauTiepNhanId)
        {
            var thoiDiemTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == yeuCauTiepNhanId).Select(d => d.ThoiDiemTiepNhan).FirstOrDefault();
            if (thoiDiemTiepNhan > ngayNhan)
            {
                return false;
                
            }
            return true;
        }
        #endregion
        public async Task<ThongTinHoSoGetInfo> GetNoiTruHoSoKhac15Ngay(long id, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo)
        {
            var thongTinHoSo = _noiTruHoSoKhacRepository.TableNoTracking
                .Where(q => q.Id == id &&
                            q.LoaiHoSoDieuTriNoiTru == loaiHoSo)
                .Select(q => new ThongTinHoSoGetInfo
                {
                    Id = q.Id,
                    ThongTinHoSo = q.ThongTinHoSo
                });
            return await thongTinHoSo.LastOrDefaultAsync();
        }
        public async Task<List<ThongTinHoSoGetInfo>> GetNoiTruHoSoKhac15Ngays(long yctn, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo)
        {
            var thongTinHoSo = _noiTruHoSoKhacRepository.TableNoTracking
                .Where(q => q.YeuCauTiepNhanId == yctn &&
                            q.LoaiHoSoDieuTriNoiTru == loaiHoSo)
                .Select(q => new ThongTinHoSoGetInfo
                {
                    Id = q.Id,
                    ThongTinHoSo = q.ThongTinHoSo
                });
            return await thongTinHoSo.ToListAsync();
        }
    }
}
