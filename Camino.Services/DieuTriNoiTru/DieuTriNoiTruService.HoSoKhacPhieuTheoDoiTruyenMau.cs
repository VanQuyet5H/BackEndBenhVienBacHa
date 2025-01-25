using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<LookupItemVo>> GetDanhSachMaDonViMau(DropDownListRequestModel queryInfo, long? yeuCauTiepNhanId, long? longMaDVMID)
        {
            if (yeuCauTiepNhanId != null && longMaDVMID != null)
            {
                var query = await _nhapKhoMauChiTietRepository.TableNoTracking
                       .Include(s => s.YeuCauTruyenMau).ThenInclude(p => p.YeuCauTiepNhan)
                       .Include(s => s.NhapKhoMau)
                       .Where(s => s.YeuCauTruyenMau.YeuCauTiepNhanId == yeuCauTiepNhanId && s.NhapKhoMau.DuocKeToanDuyet == true)
                       .ToListAsync();

                var queryDonViMauDaDuocKeToanDuyet = query.Select(s => new LookupItemVo()
                {
                    KeyId = s.Id,
                    DisplayName = s.MaTuiMau
                }).Where(s => s.KeyId == longMaDVMID).ToList(); ;
                return queryDonViMauDaDuocKeToanDuyet;
            }
            else
            {
                //long.TryParse(queryInfo.ParameterDependencies, out long yeuCauTiepNhanId);
                var ycauTiepNhanId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);

                if (ycauTiepNhanId != 0)
                {
                    var query = await _nhapKhoMauChiTietRepository.TableNoTracking
                        .Include(s => s.YeuCauTruyenMau).ThenInclude(p => p.YeuCauTiepNhan)
                        .Include(s => s.NhapKhoMau)
                        .Where(s => s.YeuCauTruyenMau.YeuCauTiepNhanId == ycauTiepNhanId && s.NhapKhoMau.DuocKeToanDuyet == true)
                         .Take(queryInfo.Take)
                         .ApplyLike(queryInfo.Query, g => g.MaTuiMau)
                        .ToListAsync();

                    var queryDonViMauDaDuocKeToanDuyet = query.Select(s => new LookupItemVo()
                    {
                        KeyId = s.Id,
                        DisplayName = s.MaTuiMau
                    }).ToList(); ;
                    return queryDonViMauDaDuocKeToanDuyet;
                }
            }
            return null;
        }
        public PhieuTheoDoiTruyenMauGrid GetThongTinPhieuTheoDoiTruyenMau(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau)
                                                                  .Select(s => new PhieuTheoDoiTruyenMauGrid()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuTheoDoiTruyenMauGridVo()
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
        public PhieuTheoDoiTruyenMauGrid GetThongTinPhieuTheoDoiTruyenMauSoSanhMaDonViMau(long yeuCauTiepNhanId,long maDonViMauId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau)
                                                                  .Select(s => new PhieuTheoDoiTruyenMauGrid()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuTheoDoiTruyenMauGridVo()
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
                                                                  }).ToList();
            List<PhieuTheoDoiTruyenMauGrid> list = new List<PhieuTheoDoiTruyenMauGrid>();
            foreach (var item in query)
            {
                if(item.ThongTinHoSo != null)
                {
                    var queryString = JsonConvert.DeserializeObject<InPhieuTheoDoiTruyenMau>(item.ThongTinHoSo);
                     if(queryString.MaDonViMauTruyenId == maDonViMauId)
                    {
                        list.Add(item);
                    }
                }
            }

            return list.FirstOrDefault();
        }
        public ThongTinDefaultPhieuTheoDoiTruyenMauCreate GetThongTinDefaultPhieuTheoDoiTruyenMauCreate(long yeuCauTiepNhanId)
        {
            var ngayHienTai = new DateTime();
            ngayHienTai = DateTime.Now;
            var query = BaseRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => new
            {
                chanDoan = s.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(x => x.Id).Select(x => x.ChanDoanVaoKhoaGhiChu).FirstOrDefault()
            }).FirstOrDefault();
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            ThongTinDefaultPhieuTheoDoiTruyenMauCreate thongTinDefaultPhieuTheoDoiTruyenMauCreate = new ThongTinDefaultPhieuTheoDoiTruyenMauCreate();
            thongTinDefaultPhieuTheoDoiTruyenMauCreate.ChanDoan = query.chanDoan;
            thongTinDefaultPhieuTheoDoiTruyenMauCreate.TenNhanVien = nguoiLogin;
            thongTinDefaultPhieuTheoDoiTruyenMauCreate.NgayThucHien = ngayHienTai.ApplyFormatDateTimeSACH();
            return thongTinDefaultPhieuTheoDoiTruyenMauCreate;
        }
        public async Task<string> InPhieuTheoDoiTruyenMau(XacNhanInPhieuTheoDoiTruyenMau xacNhanIn)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanIn.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InPhieuTheoDoiTruyenMau>(thongtinIn);
            var content = "";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuTruyenMauHoSoKhac"));
            var batdauTruyenHoi = "&nbsp;&nbsp;&nbsp;&nbsp;" + "giờ" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "phút," + "&nbsp;&nbsp;&nbsp;&nbsp;" + "ngày" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "tháng" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "năm";
            var ngungTruyenHoi = "&nbsp;&nbsp;&nbsp;&nbsp;" + "giờ" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "phút," + "&nbsp;&nbsp;&nbsp;&nbsp;" + "ngay" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "tháng" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "năm";
            if (queryString.BatDauTruyenHoiStringUTC != null)
            {
                DateTime batDauTruyenHoiStringUTC  = new DateTime();
                DateTime.TryParseExact(queryString.BatDauTruyenHoiStringUTC, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out batDauTruyenHoiStringUTC);
                batdauTruyenHoi = "";
                batdauTruyenHoi = batDauTruyenHoiStringUTC != null ? (batDauTruyenHoiStringUTC.Hour + " giờ " + (batDauTruyenHoiStringUTC.Minute + " phút, " + "ngày " + batDauTruyenHoiStringUTC.Day + " tháng " + batDauTruyenHoiStringUTC.Month + " năm" + batDauTruyenHoiStringUTC.Year)) : "";
            }
            if (queryString.NgungTruyenHoiStringUTC != null)
            {
                DateTime ngungTruyenHoiStringUTC = new DateTime();
                DateTime.TryParseExact(queryString.NgungTruyenHoiStringUTC, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out ngungTruyenHoiStringUTC);
                ngungTruyenHoi = "";
                ngungTruyenHoi = ngungTruyenHoiStringUTC != null ? (ngungTruyenHoiStringUTC.Hour + " giờ " + (ngungTruyenHoiStringUTC.Minute + " phút, " + "ngày " + ngungTruyenHoiStringUTC.Day + " tháng " + ngungTruyenHoiStringUTC.Month + " năm" + ngungTruyenHoiStringUTC.Year)) : "";
            }
            bool inLanDau = true;
            var phieuTheoDoiTruyenDichVaTruyenMau = "";
            if (queryString.DachSachTruyenMauArr != null)
            {
                foreach (var item in queryString.DachSachTruyenMauArr)
                {
                    var thoigian = "";
                    if (item.ThoiGian != null)
                    {
                        thoigian = item.ThoiGian.GetValueOrDefault().ConvertIntSecondsToTime12h();
                    }
                    else
                    {
                        thoigian = "&nbsp;";
                    }
                    var huyetAp = "";
                    if (item.HuyetAp1 != null && item.HuyetAp2 != null)
                    {
                        huyetAp = item.HuyetAp;
                    }
                    else
                    {
                        huyetAp = "&nbsp;";
                    }
                    phieuTheoDoiTruyenDichVaTruyenMau += "<tr>" +
                                                           "<td style=' border: 1px solid black;width: 10%;padding: 0px;margin: 0px;text-align:center;'>" + thoigian + "</td>"
                                                           + " <td style=' border: 1px solid black;width: 20%;text-align:center;'>" + item.TocDoTruyen + "</td>"
                                                           + "<td style='border: 1px solid black;width: 20%;text-align:center;'>" + item.MauSacDaNiemMac + "</td>"
                                                           + "<td  style='border: 1px solid black;width: 10%;text-align:center;'>" + item.NhipTho + "</td>"
                                                           + "<td  style='border: 1px solid black;width: 10%;text-align:center;'>" + item.ThanNhiet + "</td>"
                                                           + "<td   style='border: 1px solid black;width: 10%;text-align:center;'>" + huyetAp + "</td>"
                                                           + "<td style='border: 1px solid black;width: 10%;text-align:center;'>" + item.Mach + "</td>"
                                                           + "<td   style='border: 1px solid black;width: 10%;text-align:center;'>" + item.DienBienKhac + "</td>"
                                                      + "</tr>";
                }
            }
            var tenDDTruyenMau = "";
            if(queryString.DDTruyenMauId != null)
            {
                tenDDTruyenMau = _useRepository.TableNoTracking.Where(s => s.Id == queryString.DDTruyenMauId).Select(d => d.HoTen).FirstOrDefault();
            }
           
            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new
                                                                              {
                                                                                  HoTen = x.YeuCauTiepNhan.HoTen,
                                                                                  Tuoi = DateTime.Now.Year - x.YeuCauTiepNhan.NamSinh,
                                                                                  GioiTinh = x.YeuCauTiepNhan.GioiTinh.GetDescription(),
                                                                                  SoGiuong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan) ? x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.OrderByDescending(p => p.Id).FirstOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.Ten : "",
                                                                                  Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(p => p.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                  ChanDoan = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(s => s.Id).Select(s =>s.ChanDoanVaoKhoaGhiChu).FirstOrDefault(),
                                                                                  PhieuTheoDoiTruyenDichVaTruyenMau = phieuTheoDoiTruyenDichVaTruyenMau,
                                                                                  LanTruyenMauThu = queryString.LanTruyenMauThu,
                                                                                  DinhNhomMauChePhamMau = queryString.DinhNhomDonViMauChePhamMau,
                                                                                  DinhNhomMauNguoiNhan =queryString.DinhNhomMauNguoiNhan,
                                                                                  PhanUngHoaHopTaiGiuong =queryString.PhanUngHoaHopTaiGiuong,
                                                                                  BatDauTryenHoi = batdauTruyenHoi,
                                                                                  SoLuongThucTe = queryString.SLMauTruyenThucTe,
                                                                                  HoTenBSDieuTri = queryString.BSDieuTri,
                                                                                  HotenDieuDuongTruyenMau = tenDDTruyenMau,
                                                                                  NgungTruyenHoi = ngungTruyenHoi,
                                                                              }).FirstOrDefault();

            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
    }
}
