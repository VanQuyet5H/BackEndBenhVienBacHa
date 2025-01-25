using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BangKiemAnToanNBPT;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
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
        public ThuocTienMeVaNhanVienGrid GetThuocTienMeVaNhanVienVaChiSoSinhTon(QueryInfo queryInfo)
        {
            var lstNhanVien =
                   _nhanVienRepository.TableNoTracking
                    .Where(x => !string.IsNullOrEmpty(x.User.HoTen.Trim()))
                    .Select(x => x.User.HoTen).ToList();
            var lstDuocPham =
              _duocPhamBenhVienRepository.TableNoTracking
                 .Where(x => !string.IsNullOrEmpty(x.DuocPham.Ten.Trim()))
                 .Select(x => x.DuocPham.Ten).ToList();
            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhan);
            var lstBangKiemAnToanNguoiBenhh = _ketQuaSinhHieuRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhan)
                                                                                       .Select(q => new ChiSoSinhTonBangKiemAnToan()
                                                                                       {
                                                                                           Id = q.Id,
                                                                                           NgayThucHien = q.LastTime != null ? q.LastTime.Value.ApplyFormatDateTime() : "",
                                                                                           BMI = q.Bmi,
                                                                                           CanNang = q.CanNang,
                                                                                           NhipTho = q.NhipTho,
                                                                                           NhipTim = q.NhipTim,
                                                                                           ChieuCao = q.ChieuCao,
                                                                                           Glassgow = q.Glassgow,
                                                                                           HuyetApTamThu = q.HuyetApTamThu,
                                                                                           HuyetApTamTruong = q.HuyetApTamTruong,
                                                                                           SpO2 = q.SpO2,
                                                                                           ThanNhiet = q.ThanNhiet
                                                                                       }).ToList();
            var query = new ThuocTienMeVaNhanVienGrid();
            query.ListNhanVien = lstNhanVien;
            query.ListThuocTienMe = lstDuocPham;
            query.ListChiSoSinhTonBangKiemAnToan = lstBangKiemAnToanNguoiBenhh;
            return query;
        }
        public List<long> GetListChiSoSinhHieu(long yeuCauTiepNhanId)
        {
            var lst = _ketQuaSinhHieuRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                                                                                      .Select(q => q.Id).ToList();
            return lst;
        }
        public BangKiemAnToanNBPTGridVo GetThongTinBangKiemATNBPT(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat)
                                                                  .Select(s => new BangKiemAnToanNBPTGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                  }).FirstOrDefault();
            return query;
        }
        public async Task<GridDataSource> GetDanhSachBangKiemAnToanNBPT(QueryInfo queryInfo)
        {
            //
            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat)
                 .Select(s => new DanhSachBangKiemAnToanNBPTGridVo()
                 {
                     Id = s.Id,
                     ThongTinHoSo = s.ThongTinHoSo
                 }).ToList();

            var dataOrderBy = query.AsQueryable().OrderByDescending(cc => cc.Id);
            var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();
            foreach (var item in quaythuoc)
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachBangKiemAnToanNBPTGridVo>(item.ThongTinHoSo);
                DateTime ngayGioDuaBNDiPT = new DateTime();
                if(queryString.NgayGioDuaBNDiPTUTC != null)
                {
                    DateTime.TryParseExact(queryString.NgayGioDuaBNDiPTUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayGioDuaBNDiPT);
                    item.NgayGioDuaBNDiPTString = ngayGioDuaBNDiPT.ApplyFormatDateTime();
                }
                else
                {
                    item.NgayGioDuaBNDiPTString = "";
                }
                
            }
            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
        }
        public BangKiemAnToanNBPTGridVo GetDanhSachBangKiemAnToanNBPTSave(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat)
                                                                  .Select(s => new BangKiemAnToanNBPTGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new BangKiemAnToanGridVo()
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
                                                                  }).OrderBy(z => z.Id).LastOrDefault();
            return query;
        }
        public BangKiemAnToanNBPTGridVo GetThongTinBangKiemAnToanNBPTViewDS(long noiTruHoSoKhacId)
        {
            var listFileChuKy = _noiTruHoSoKhacFileDinhKemRepository.TableNoTracking.Where(x => x.NoiTruHoSoKhacId == noiTruHoSoKhacId)
                .Select(s => new BangKiemAnToanGridVo()
                {
                    Id = s.Id,
                    DuongDan = s.DuongDan,
                    KichThuoc = s.KichThuoc,
                    LoaiTapTin = s.LoaiTapTin,
                    Ma = s.Ma,
                    MoTa = s.MoTa,
                    Ten = s.Ten,
                    TenGuid = s.TenGuid
                }).ToList();
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == noiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat)
                                                                  .Select(s => new BangKiemAnToanNBPTGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = listFileChuKy
                                                                  }).FirstOrDefault();
            return query;
        }
        public async Task<string> BangKiemAnToanNBPT(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InModelBangKiemAnToanNBPT>(thongtinIn);
            var content = "";
            // Icd chính, icd kem theo
            string benhkemTheo = "";
            var getChanDoanChinh = _noiTruPhieuDieuTriRepository.TableNoTracking
               .Where(e => e.NgayDieuTri.Day == DateTime.Now.Day && e.NgayDieuTri.Month == DateTime.Now.Month && e.NgayDieuTri.Year == DateTime.Now.Year && e.NoiTruBenhAnId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId)
               .Select(x => x.ChanDoanChinhGhiChu).LastOrDefault();
            var getChanDoanKemTheoList = _noiTruPhieuDieuTriRepository.TableNoTracking
              .Where(e => e.NgayDieuTri.Day == DateTime.Now.Day && e.NgayDieuTri.Month == DateTime.Now.Month && e.NgayDieuTri.Year == DateTime.Now.Year && e.NoiTruBenhAnId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId)
              .Select(x => x.Id).ToList();
            foreach (var itemKemTheo in getChanDoanKemTheoList)
            {
                var benhkemTheoItem = _noiTruThamKhamChanDoanKemTheoRepository.TableNoTracking.Where(x => x.NoiTruPhieuDieuTriId == itemKemTheo).Select(x => x.ICD.TenTiengViet).FirstOrDefault();
                benhkemTheo += benhkemTheoItem != null ? benhkemTheoItem + "," : "";
            }
            // tien su di ung
            var tienSuDiUng = string.Empty;
            tienSuDiUng = queryString.TienSuDiUng;
            // thuôc đang dùng
            var thuocDangDung = string.Empty;
            if(!string.IsNullOrEmpty(queryString.ThuocDangDung))
            {
                thuocDangDung = queryString.ThuocDangDung;
            }
            //var thoiDiemThucHien = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru).Select(x => x.ThoiDiemThucHien).FirstOrDefault();
           
            
            var arr1 = string.Empty;
            var arr2 = string.Empty;
            var arr3 = string.Empty;

            if (queryString.ThoiDiemKhamString != null)
            {
                DateTime thoiDiemKham;
                DateTime.TryParseExact(queryString.ThoiDiemKhamString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out thoiDiemKham);
                arr1 = (thoiDiemKham.Day < 9 ? "0" + thoiDiemKham.Day : thoiDiemKham.Day.ToString());
                arr2 = (thoiDiemKham.Month < 9 ? "0" + thoiDiemKham.Month : thoiDiemKham.Month.ToString());
                arr3 = thoiDiemKham.Year.ToString();
            }

          
           
            /// dấu cách xuống hàng
            /// 
            string chuoiEnter = "\n";
            string chuoiEnterThayThe = "<br>";
            string nguyCoSuyHoHapMatMau = "";
            string lamSangCLSCanLuuY = "";
            string nhungLuuYKhac = "";
            string tiepXuc = "";
            string khac = "";
            string yKienNguoiNguoiNhanNguoiBenh = "";
            if (queryString.NguyCoSuyHoHapMatMau != null)
            {
                nguyCoSuyHoHapMatMau = queryString.NguyCoSuyHoHapMatMau.Replace(chuoiEnter, chuoiEnterThayThe);
            }
            if (queryString.LamSangCLSCanLuuY != null)
            {
                lamSangCLSCanLuuY = queryString.LamSangCLSCanLuuY.Replace(chuoiEnter, chuoiEnterThayThe);
            }
            if (queryString.NhungLuuYKhac != null)
            {
                nhungLuuYKhac = queryString.NhungLuuYKhac.Replace(chuoiEnter, chuoiEnterThayThe);
            }
            if (queryString.TiepXuc != null)
            {
                tiepXuc = queryString.TiepXuc.Replace(chuoiEnter, chuoiEnterThayThe);
            }
            if (queryString.ThongSoKhac != null)
            {
                khac = queryString.ThongSoKhac.Replace(chuoiEnter, chuoiEnterThayThe);
            }
            if (queryString.YKienCuaNguoiNhanNguoiBenhTaiPhongGMHS != null)
            {
                yKienNguoiNguoiNhanNguoiBenh = queryString.YKienCuaNguoiNhanNguoiBenhTaiPhongGMHS.Replace(chuoiEnter, chuoiEnterThayThe);
            }
            //
            string GioDuaBenhNhan="";
            string ngayDuaBenhNhan = string.Empty;
            string thangDuaBenhNhan = string.Empty;
            string namDuaBenhNhan = string.Empty;
            if (queryString.NgayGioDuaBNDiPTUTC != null)
            {
                DateTime ngayGioDuaBNDiPT;
                DateTime.TryParseExact(queryString.NgayGioDuaBNDiPTUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayGioDuaBNDiPT);
                //GioDuaBenhNhan = ngayGioDuaBNDiPT.ApplyFormatTime();
                GioDuaBenhNhan = (ngayGioDuaBNDiPT.Hour < 9 ? "0"+ ngayGioDuaBNDiPT.Hour.ToString() : ngayGioDuaBNDiPT.Hour.ToString()) + " giờ " + (ngayGioDuaBNDiPT.Minute < 9 ? "0" + ngayGioDuaBNDiPT.Minute.ToString() + " phút" : ngayGioDuaBNDiPT.Minute.ToString()+ " phút");
                ngayDuaBenhNhan = (ngayGioDuaBNDiPT.Day < 9 ? "0"+ ngayGioDuaBNDiPT.Day: ngayGioDuaBNDiPT.Day.ToString());
                thangDuaBenhNhan = (ngayGioDuaBNDiPT.Month < 9 ? "0" + ngayGioDuaBNDiPT.Month : ngayGioDuaBNDiPT.Month.ToString());
                namDuaBenhNhan = ngayGioDuaBNDiPT.Year.ToString();
            }
            string GioGayMe = string.Empty ;
            string ngayGayMe = string.Empty;
            string thangGayMe = string.Empty;
            string namGayMe = string.Empty;

            if (queryString.NgayGioDuDinhGayMeUTC != null)
            {
                DateTime ngayGioGayme;
                DateTime.TryParseExact(queryString.NgayGioDuDinhGayMeUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayGioGayme);
                GioGayMe = (ngayGioGayme.Hour < 9 ? "0" + ngayGioGayme.Hour.ToString() : ngayGioGayme.Hour.ToString()) + " giờ " + (ngayGioGayme.Minute < 9 ? "0" + ngayGioGayme.Minute.ToString() + " phút" : ngayGioGayme.Minute.ToString() + " phút"); ;
                ngayGayMe = (ngayGioGayme.Day < 9 ? "0" + ngayGioGayme.Day : ngayGioGayme.Day.ToString()); 
                thangGayMe = (ngayGioGayme.Month < 9 ? "0" + ngayGioGayme.Month : ngayGioGayme.Month.ToString());
                namGayMe = ngayGioGayme.Year.ToString();
            }
            // chi so sinh ton
            string HuyetApTamThu = "";
            string HuyetApTamTruong = "";
            var chiSoSinhTon = _ketQuaSinhHieuRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId).LastOrDefault();
            if(chiSoSinhTon != null)
            {
                HuyetApTamThu = chiSoSinhTon.HuyetApTamThu.GetValueOrDefault().ToString();
                HuyetApTamTruong = chiSoSinhTon.HuyetApTamTruong.GetValueOrDefault().ToString();
            }
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuBangKiemAnToanNguoiBenhPhauThuat"));
            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new
                                                                              {   NgaySinh = x.YeuCauTiepNhan.NgaySinh,
                                                                                  ThangSinh = x.YeuCauTiepNhan.ThangSinh,
                                                                                  NamSinh = x.YeuCauTiepNhan.NamSinh,
                                                                                  LogoUrl = xacNhanInTrichBienBanHoiChan.Hosting + "/assets/img/logo-bacha-full.png",
                                                                                  Ngay = arr1,
                                                                                  Thang = arr2,
                                                                                  Nam = arr3,
                                                                                  SoVaoVien = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                                  HoTenNguoiBenh = x.YeuCauTiepNhan.HoTen,
                                                                                  Tuoi = DateTime.Now.Year - x.YeuCauTiepNhan.NamSinh,
                                                                                  GioiTinhNam = x.YeuCauTiepNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam ? "X" : "&nbsp;&nbsp;",
                                                                                  GioiTinhNu = x.YeuCauTiepNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu ? "X" : "&nbsp;&nbsp;",
                                                                                  KhoaPhong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(l => l.GiuongBenh.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault(),
                                                                                  NgheNghiep = x.YeuCauTiepNhan.NgheNghiepId != null ? x.YeuCauTiepNhan.NgheNghiep.Ten : "",
                                                                                  BenhChinh = queryString.ChanDoanICDChinh,
                                                                                  BenhKemTheo = queryString.ChanDoanICDPhu,
                                                                                  ThuocDangDung = queryString.ThuocTienMe,
                                                                                  TienSuDiUng = tienSuDiUng,
                                                                                  NhacBenhNhanNhinAn = queryString.NhacBenhNhanNhinAn == true ? "Có" : "Không",
                                                                                  GioPhauThuat = GioDuaBenhNhan,
                                                                                  NgayPhauThuat = ngayDuaBenhNhan,
                                                                                  ThangPhauThuat = thangDuaBenhNhan,
                                                                                  NamPhauThuat = namDuaBenhNhan,
                                                                                  GioGayMe = GioGayMe,
                                                                                  NgayGayMe = ngayGayMe,
                                                                                  ThangGayMe = thangGayMe,
                                                                                  NamGayMe = namGayMe,
                                                                                  YKienNguoiNguoiNhanNguoiBenh = yKienNguoiNguoiNhanNguoiBenh,
                                                                                  KhamVaTuVanCuaPTV = queryString.KhamVaTuVanCuaPTV == true ? "Có" : "",
                                                                                  BenhKemTheoString = queryString.BenhKemTheo == true ? "Có" : "",
                                                                                  DaiThaoDuong = queryString.DaiThaoDuong == true ? "Có" : "",
                                                                                  TangHuyetAp = queryString.TangHuyetAp == true ? "Có" : "",
                                                                                  Khac = queryString.Khac == true ? "Có" : "",
                                                                                  VeSinhTamGoi = queryString.VeSinhTamGoi == true ? "Có" : "",
                                                                                  VatLieuCayGhep = queryString.VatLieuCayGhep == true ? "Có" : "",
                                                                                  CatMongTayMongChan = queryString.CatMongTayMongChan == true ? "Có" : "",
                                                                                  DoTrangSuc = queryString.DoTrangSuc == true ? "Có" : "",
                                                                                  ThaoRangGia = queryString.ThaoRangGia == true ? "Có" : "",
                                                                                  QuanAoSachMoiThay = queryString.QuanAoSachMoiThay == true ? "Có" : "",
                                                                                  VetThuongHo = queryString.VetThuongHo == true ? "Có" : "",
                                                                                  VeSinhDaVungMo = queryString.VeSinhDaVungMo == true ? "Có" : "",
                                                                                  BangVoTrungDanhDauViTriPhauThuat = queryString.BangVoTrungDanhDauViTriPhauThuat == true ? "Có" : "",
                                                                                  KhamGayMe = queryString.KhamGayMe == true ? "Có" : "",
                                                                                  PTTTGMHS = queryString.PTTTGMHS == true ? "Có" : "",
                                                                                  PhieuXetNghiemNhomMauDongMau = queryString.PhieuXetNghiemNhomMauDongMau == true ? "Có" : "",
                                                                                  PhimChupPhoiSoLuong = queryString.PhimChupPhoiSL == true ? "Có" : "",
                                                                                  CacLoaiPhimAnhKhacSoLuong = queryString.CacLoaiPhimAnh == true ? "Có" : "",
                                                                                  DienTim = queryString.DienTim == true ? "Có" : "",
                                                                                  XacNhanThanhVienGayMePhauThuat = queryString.XacNhanThanhVienGayMePhauThuat == true ? "Có" : "",
                                                                                  KhangSinhDuPhong = queryString.KhangSinhDuPhong == true ? "Có" : "",
                                                                                  DuyetPhauThuat = queryString.DuyetPhauThuat == true ? "Có" : "",
                                                                                  KyCamKetSuDungKTCao = queryString.KyCamKetSuDungKTCao == true?"Có":"",

                                                                                  //
                                                                                  KhamVaTuVanCuaPTVKhong = queryString.KhamVaTuVanCuaPTV == false ? "Không" : "",
                                                                                  BenhKemTheoStringKhong = queryString.BenhKemTheo == false ? "Không" : "",
                                                                                  DaiThaoDuongKhong = queryString.DaiThaoDuong == false ? "Không" : "",
                                                                                  TangHuyetApKhong = queryString.TangHuyetAp == false ? "Không" : "",
                                                                                  KhacKhong = queryString.Khac == false ? "Không" : "",
                                                                                  VeSinhTamGoiKhong = queryString.VeSinhTamGoi == false ? "Không" : "",
                                                                                  VatLieuCayGhepKhong = queryString.VatLieuCayGhep == false ? "Không" : "",
                                                                                  CatMongTayMongChanKhong = queryString.CatMongTayMongChan == false ? "Không" : "",
                                                                                  DoTrangSucKhong = queryString.DoTrangSuc == false ? "Không" : "",
                                                                                  ThaoRangGiaKhong = queryString.ThaoRangGia == false ? "Không" : "",
                                                                                  QuanAoSachMoiThayKhong = queryString.QuanAoSachMoiThay == false ? "Không" : "",
                                                                                  VetThuongHoKhong = queryString.VetThuongHo == false ? "Không" : "",
                                                                                  VeSinhDaVungMoKhong = queryString.VeSinhDaVungMo == false ? "Không" : "",
                                                                                  BangVoTrungDanhDauViTriPhauThuatKhong = queryString.BangVoTrungDanhDauViTriPhauThuat == false ? "Không" : "",
                                                                                  KhamGayMeKhong = queryString.KhamGayMe == false ? "Không" : "",
                                                                                  PTTTGMHSKhong = queryString.PTTTGMHS == false ? "Không" : "",
                                                                                  PhieuXetNghiemNhomMauDongMauKhong = queryString.PhieuXetNghiemNhomMauDongMau == false ? "Không" : "",
                                                                                  PhimChupPhoiSoLuongKhong = queryString.PhimChupPhoiSL == false ? "Không" : "",
                                                                                  CacLoaiPhimAnhKhacSoLuongKhong = queryString.CacLoaiPhimAnh == false ? "Không" : "",
                                                                                  DienTimKhong = queryString.DienTim == false ? "Không" : "",
                                                                                  XacNhanThanhVienGayMePhauThuatKhong = queryString.XacNhanThanhVienGayMePhauThuat == false ? "Không" : "",
                                                                                  KhangSinhDuPhongKhong = queryString.KhangSinhDuPhong == false ? "Không" : "",
                                                                                  DuyetPhauThuatKhong = queryString.DuyetPhauThuat == false ? "Không" : "",
                                                                                  KyCamKetSuDungKTCaoKhong = queryString.KyCamKetSuDungKTCao == false ? "Không" : "",
                                                                                  TiepXuc = tiepXuc,
                                                                                  ChieuCao = chiSoSinhTon != null ? chiSoSinhTon.ChieuCao : null,
                                                                                  CanNang = chiSoSinhTon != null ? chiSoSinhTon.CanNang : null,
                                                                                  Mach = chiSoSinhTon != null ? chiSoSinhTon.NhipTim : null,
                                                                                  NhietDo = chiSoSinhTon != null ? chiSoSinhTon.ThanNhiet : null,
                                                                                  HuyetAp = !string.IsNullOrEmpty(HuyetApTamThu) && !string.IsNullOrEmpty(HuyetApTamTruong) ? HuyetApTamThu + "/" + HuyetApTamTruong : HuyetApTamThu + "" + HuyetApTamTruong,
                                                                                  NhipTho = chiSoSinhTon != null ? chiSoSinhTon.NhipTho : null,
                                                                                  NhomMau = x.YeuCauTiepNhan.NhomMau != null ? x.YeuCauTiepNhan.NhomMau.GetDescription() : "",
                                                                                  ThongSoKhac = khac,
                                                                                  NguyCoSuyHoHapMatMau = nguyCoSuyHoHapMatMau,
                                                                                  LamSangCLSCanLuuY = lamSangCLSCanLuuY,
                                                                                  NhungLuuYKhac = nhungLuuYKhac,
                                                                                  DDNhanBNTaiPhongPTGMHS = queryString.DDNhanBNTaiPhongPTGMHS,
                                                                                  DDChuanBiNBDenPhongPT = queryString.DDChuanBiNBDenPhongPT,
                                                                                  DDChuanBiNBTruocPT = queryString.DDChuanBiNBTruocPT,
                                                                                  MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                                  Giuong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(s => s.Id).Select(s => s.GiuongBenh.Ten).FirstOrDefault(),
                                                                                  BenhChinhStringCo = queryString.BenhChinh == true ? "Có" : "",
                                                                                  BenhChinhStringKhong = queryString.BenhChinh == false ? "Không" : "",
                                                                                  TheDinhDanhCo = queryString.TheDinhDanh == true ? "Có" : "",
                                                                                  TheDinhDanhKhong = queryString.TheDinhDanh == false ? "Không" : "",
                                                                                  DanNguoiBenhCo = queryString.NhacBenhNhanNhinAn == true ? "Có" : "",
                                                                                  DanNguoiBenhKhong = queryString.NhacBenhNhanNhinAn == false ? "Không" : "",
                                                                                  ChanDoan= x.YeuCauTiepNhan.YeuCauNhapVien != null ?
                                                                                           (string.IsNullOrEmpty(x.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienGhiChu) ? x.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + x.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet : x.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienGhiChu) : null
                                                                              }).FirstOrDefault();

            #region BVHD-3802
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(d => d.Id == khoaId).Select(d => d.Ten).FirstOrDefault();

            var dataIn = new InBangKiemAnToanNBPT();
            if (!string.IsNullOrEmpty(tenKhoa))
            {
                tenKhoa = tenKhoa.Replace("Khoa", "");
            }
            dataIn.KhoaPhongDangIn = tenKhoa;


            dataIn.ChieuCao = data.ChieuCao +"";
            dataIn.NhomMau = data.NhomMau;

            dataIn.LogoUrl = data.LogoUrl;

            dataIn.MaTN = data.MaYeuCauTiepNhan;
            dataIn.Ngay = arr1;
            dataIn.Thang = arr2;
            dataIn.Nam = arr3;



            dataIn.HoTenNguoiBenh = !string.IsNullOrEmpty(data.HoTenNguoiBenh) ? "<div class='container'><div class='label'>Họ tên người bệnh:</div><div class='values'><b>" + data.HoTenNguoiBenh+"<b></div></div>"
                : "<div class='container'><div class='label'>Họ tên người bệnh:</div><div class='value'><b>" + data.HoTenNguoiBenh + "<b></div></div>";

            var namSinh = DateHelper.DOBFormat(data.NgaySinh, data.ThangSinh, data.NamSinh);

            

            dataIn.Tuoi = !string.IsNullOrEmpty(namSinh) ? "<div class='container'><div class='label'>NS: </div><div class='values'><b>" + namSinh + "</b></div></div>"
                : "<div class='container'><div class='label'>NS: </div><div class='value'><b>" + namSinh + "</b></div></div>";

            dataIn.GioiTinhNam = data.GioiTinhNam;
            dataIn.GioiTinhNu = data.GioiTinhNu;

             

            dataIn.KhoaPhong = !string.IsNullOrEmpty(data.KhoaPhong) ? "<div class='container'><div class='label'>Khoa phòng:</div><div class='values'>" + data.KhoaPhong + "</div></div>"
                : "<div class='container'><div class='label'>Khoa phòng:</div><div class='value'>" + data.KhoaPhong + "</div></div>";

            string[] listStringGiuongBenhs = new string[] { "Giường", "GIƯỜNG" };
            var giuong = string.Empty;
            if (!string.IsNullOrEmpty(data.Giuong))
            {
                foreach (var item in listStringGiuongBenhs)
                {
                    giuong = data.Giuong.ToUpper().Replace(item.ToUpper(), "").ToString();
                }
            }


            dataIn.Giuong = !string.IsNullOrEmpty(giuong) ? "<div class='container'><div class='label'>Số giường:</div><div class='values'>" + giuong + "</div></div>"
                : "<div class='container'><div class='label'>Số giường:</div><div class='value'>" + giuong + "</div></div>";


            dataIn.ChanDoan = !string.IsNullOrEmpty(data.ChanDoan)   ? "<div class='container'><div class='label'>Chẩn đoán:</div><div class='values'>" + data.ChanDoan + "</div></div>" 
                : "<div class='container'><div class='label'>Chẩn đoán:</div><div class='value'>" + data.ChanDoan + "</div></div>";

            
            dataIn.ThuocDangDung = !string.IsNullOrEmpty(thuocDangDung) ? "<div class='container'><div class='label'>Thuốc đang dùng:</div><div class='values' >" + thuocDangDung +"</div> </div>"
                : "<div class='container'><div class='label'>Thuốc đang dùng:</div><div class='value'>" + thuocDangDung + "</div> </div>";


           
            dataIn.TienSuDiUng = !string.IsNullOrEmpty(tienSuDiUng) ? "<div class='container'><div class='label'>Tiền sử dị ứng:</div><div class='values' >" + tienSuDiUng + "</div></div>"
                : "<div class='container'><div class='label'>Tiền sử dị ứng:</div><div class='value'>" + tienSuDiUng + "</div></div>";

            dataIn.KhamVaTuVanCuaPTV = data.KhamVaTuVanCuaPTV;

            dataIn.KhamVaTuVanCuaPTVKhong = data.KhamVaTuVanCuaPTVKhong;
            dataIn.TiepXuc = data.TiepXuc;
            dataIn.BenhChinhStringCo = data.BenhChinhStringCo;
            dataIn.BenhChinhStringKhong = data.BenhChinhStringKhong;
            dataIn.BenhKemTheoString = data.BenhKemTheoString;
            dataIn.BenhKemTheoStringKhong = data.BenhKemTheoStringKhong;
            dataIn.CanNang = data.CanNang + "";
            dataIn.DaiThaoDuong = data.DaiThaoDuong;
            dataIn.DaiThaoDuongKhong = data.DaiThaoDuongKhong;
            dataIn.Mach = data.Mach + "";
            dataIn.TangHuyetAp = data.TangHuyetAp;
            dataIn.TangHuyetApKhong = data.TangHuyetApKhong;
            dataIn.NhietDo = data.NhietDo + "";
            dataIn.Khac = data.Khac;
            dataIn.KhacKhong = data.KhacKhong;
            dataIn.HuyetAp = data.HuyetAp;
            dataIn.TheDinhDanhCo = data.TheDinhDanhCo;
            dataIn.TheDinhDanhKhong = data.TheDinhDanhKhong;
            dataIn.NhipTho = data.NhipTho + "";
            dataIn.DanNguoiBenhCo = data.DanNguoiBenhCo;
            dataIn.DanNguoiBenhKhong = data.DanNguoiBenhKhong;
            dataIn.VeSinhTamGoi = data.VeSinhTamGoi;
            dataIn.VeSinhTamGoiKhong = data.VeSinhTamGoiKhong;
            dataIn.VatLieuCayGhep = data.VatLieuCayGhep;
            dataIn.VatLieuCayGhepKhong = data.VatLieuCayGhepKhong;
            dataIn.NhomMau = data.NhomMau;
            dataIn.CatMongTayMongChan = data.CatMongTayMongChan;
            dataIn.CatMongTayMongChanKhong = data.CatMongTayMongChanKhong;
            dataIn.ThongSoKhac = data.ThongSoKhac;
            dataIn.DoTrangSuc = data.DoTrangSuc;

            dataIn.DoTrangSucKhong = data.DoTrangSucKhong;
            dataIn.ThaoRangGia = data.ThaoRangGia;
            dataIn.ThaoRangGiaKhong = data.ThaoRangGiaKhong;
            dataIn.QuanAoSachMoiThay = data.QuanAoSachMoiThay;
            dataIn.QuanAoSachMoiThayKhong = data.QuanAoSachMoiThayKhong;
            dataIn.VetThuongHo = data.VetThuongHo;


            dataIn.VetThuongHoKhong = data.VetThuongHoKhong;
            dataIn.VeSinhDaVungMo = data.VeSinhDaVungMo;
            dataIn.VeSinhDaVungMoKhong = data.VeSinhDaVungMoKhong;
            dataIn.BangVoTrungDanhDauViTriPhauThuat = data.BangVoTrungDanhDauViTriPhauThuat;
            dataIn.BangVoTrungDanhDauViTriPhauThuatKhong = data.BangVoTrungDanhDauViTriPhauThuatKhong;

            dataIn.KhamGayMe = data.KhamGayMe;

            dataIn.KhamGayMeKhong = data.KhamGayMeKhong;


            dataIn.PTTTGMHS = data.PTTTGMHS;
            dataIn.PTTTGMHSKhong = data.PTTTGMHSKhong;
            dataIn.PhieuXetNghiemNhomMauDongMau = data.PhieuXetNghiemNhomMauDongMau;
            dataIn.PhieuXetNghiemNhomMauDongMauKhong = data.PhieuXetNghiemNhomMauDongMauKhong;
            dataIn.LamSangCLSCanLuuY = data.LamSangCLSCanLuuY;
            dataIn.PhimChupPhoiSoLuong = data.PhimChupPhoiSoLuong;
            dataIn.PhimChupPhoiSoLuongKhong = data.PhimChupPhoiSoLuongKhong;
            dataIn.CacLoaiPhimAnhKhacSoLuong = data.CacLoaiPhimAnhKhacSoLuong;
            dataIn.CacLoaiPhimAnhKhacSoLuongKhong = data.CacLoaiPhimAnhKhacSoLuongKhong;
            dataIn.DienTim = data.DienTim;
            dataIn.DienTimKhong = data.DienTimKhong;
            dataIn.XacNhanThanhVienGayMePhauThuat = data.XacNhanThanhVienGayMePhauThuat;
            dataIn.XacNhanThanhVienGayMePhauThuatKhong = data.XacNhanThanhVienGayMePhauThuatKhong;
            
            dataIn.KyCamKetSuDungKTCao = data.KyCamKetSuDungKTCao;
            dataIn.KyCamKetSuDungKTCaoKhong = data.KyCamKetSuDungKTCaoKhong;

            dataIn.KhangSinhDuPhong = data.KhangSinhDuPhong;
            dataIn.KhangSinhDuPhongKhong = data.KhangSinhDuPhongKhong;
            dataIn.NhungLuuYKhac = data.NhungLuuYKhac;
            dataIn.DuyetPhauThuat = data.DuyetPhauThuat;
            dataIn.DuyetPhauThuatKhong = data.DuyetPhauThuatKhong;
            dataIn.NguyCoSuyHoHapMatMau = data.NguyCoSuyHoHapMatMau;

            dataIn.BarCodeImgBase64 = !string.IsNullOrEmpty(data.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(data.MaYeuCauTiepNhan) : "";

           

            dataIn.GioPhauThuat = !string.IsNullOrEmpty(data.GioPhauThuat) && !string.IsNullOrEmpty(data.NgayPhauThuat) && !string.IsNullOrEmpty(data.ThangPhauThuat) && !string.IsNullOrEmpty(data.NamPhauThuat) ?
                "Giờ đưa người bệnh đi phẫu thuật: " + data.GioPhauThuat  + " ngày " + data.NgayPhauThuat + " tháng " + data.ThangPhauThuat + " năm " + data.NamPhauThuat

                : "Giờ đưa người bệnh đi phẫu thuật:  Giờ........ngày........tháng..........năm..........";
           


            

            dataIn.GioGayMe = !string.IsNullOrEmpty(data.GioGayMe) && !string.IsNullOrEmpty(data.NgayGayMe) && !string.IsNullOrEmpty(data.ThangGayMe) && !string.IsNullOrEmpty(data.NamGayMe) ?
                "Dự định giờ gây mê người bệnh (Bác sỹ GMHS ghi): " +data.GioGayMe + " ngày " + data.NgayGayMe + " tháng " + data.ThangGayMe + " năm " + data.NamGayMe
                : "Dự định giờ gây mê người bệnh (Bác sỹ GMHS ghi): .....giờ........ngày.......tháng......năm........";
          
            dataIn.YKienNguoiNguoiNhanNguoiBenh = !string.IsNullOrEmpty(data.YKienNguoiNguoiNhanNguoiBenh)? "<div class='container'><div class='values'>" + data.YKienNguoiNguoiNhanNguoiBenh+"</div></div>"
                : "<div class='container'><div class='value'>" + data.YKienNguoiNguoiNhanNguoiBenh + "</div></div>";

            dataIn.DDChuanBiNBTruocPT = data.DDChuanBiNBTruocPT;
            dataIn.DDChuanBiNBDenPhongPT = data.DDChuanBiNBDenPhongPT;
            dataIn.DDNhanBNTaiPhongPTGMHS = data.DDNhanBNTaiPhongPTGMHS;
           
            #endregion



            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, dataIn);
            return content;
        }
        public NhanVienNgayThucHien GetThongTinCreate(long idNguoiLogin, long yeuCauTiepNhanId)
        {
            var ngayHienTai = new DateTime();
            ngayHienTai = DateTime.Now;
            var query = BaseRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => new
            {
                daDieuTriTuNgay = s.ThoiDiemTiepNhan,
                taiSoGiuong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.Ten).FirstOrDefault(),
                phong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                Khoa = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault(),
                chanDoan = s.YeuCauNhapVien != null ?
                          (string.IsNullOrEmpty(s.YeuCauNhapVien.ChanDoanNhapVienGhiChu) ? s.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + s.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet : s.YeuCauNhapVien.ChanDoanNhapVienGhiChu) : null
            }).FirstOrDefault();
            var getChanDoanChinh = _noiTruPhieuDieuTriRepository.TableNoTracking
               .Where(e => e.NgayDieuTri.Day == DateTime.Now.Day && e.NgayDieuTri.Month == DateTime.Now.Month && e.NgayDieuTri.Year == DateTime.Now.Year && e.NoiTruBenhAnId == yeuCauTiepNhanId)
               .Select(x => x.ChanDoanChinhGhiChu).LastOrDefault();
            var getChanDoanKemTheoList = _noiTruPhieuDieuTriRepository.TableNoTracking
              .Where(e => e.NgayDieuTri.Day == DateTime.Now.Day && e.NgayDieuTri.Month == DateTime.Now.Month && e.NgayDieuTri.Year == DateTime.Now.Year && e.NoiTruBenhAnId == yeuCauTiepNhanId)
              .Select(x => x.Id).ToList();
            string benhKemTheo = "";
            foreach (var itemKemTheo in getChanDoanKemTheoList)
            {
                var benhkemTheoItem = _noiTruThamKhamChanDoanKemTheoRepository.TableNoTracking.Where(x => x.NoiTruPhieuDieuTriId == itemKemTheo).Select(x => x.ICD.TenTiengViet).FirstOrDefault();
                benhKemTheo += benhkemTheoItem != null ? benhkemTheoItem + "," : "";
            }
            // getChanDoan.LastOrDefaultAsync();
            var benhNhanId = BaseRepository.TableNoTracking.Where(d => d.Id == yeuCauTiepNhanId).Select(d => d.BenhNhanId).FirstOrDefault();
            var tienSuDiUngCuaBenhNhan = _benhNhanDiUngThuocRepository.TableNoTracking.Where(d => d.BenhNhanId == benhNhanId)
                .Select(d => d.TenDiUng).ToList();
    

            var nguoiLoginNgayThucHien = _nhanVienRepository.TableNoTracking.Where(x => x.Id == idNguoiLogin).Select(s => new NhanVienNgayThucHien()
            {
                TenNhanVien = s.User.HoTen,
                NgayThucHien = ngayHienTai.ApplyFormatDateTimeSACH(),
                DaDieuTriTuNgay = query.daDieuTriTuNgay,
                TaiSoGiuong = query.taiSoGiuong,
                Phong = query.taiSoGiuong,
                Khoa = query.Khoa,
                ChanDoan = query.chanDoan,
                ChanDoanICDChinh = getChanDoanChinh != null ? getChanDoanChinh.ToString() :"",
                ChanDoanICDPhu = benhKemTheo,
                ThoiDiemKham = new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,0,0,0),
                TienSuDiUng = tienSuDiUngCuaBenhNhan.Join(",")
            }).FirstOrDefault();
            return nguoiLoginNgayThucHien;
        }
        #region BVHD-3802
        public bool KiemTraThoaDieuKien(long yeuCauTiepNhanId, DateTime? ngay, int loai)
        {
            //Bắt điều kiện: Thời điểm khám: phải sau thời gian tiếp nhận NB và trước thời gian hiện tại
            var thoiGianTiepNhanCuaBN = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == yeuCauTiepNhanId).Select(d => d.ThoiDiemTiepNhan).FirstOrDefault();
            var result = true;
            switch (loai)
            {

                case 1:
                    //Ngày giờ đưa NB đi phẫu thuật và ngày giờ dự định gây mê phải sau ngày thời gian tiếp nhận
                    result = ngay > thoiGianTiepNhanCuaBN  ? true : false;
                    break;
                case 2:
                    //Bắt điều kiện: Thời điểm khám: phải sau thời gian tiếp nhận NB và trước thời gian hiện tại

                    result= ngay > thoiGianTiepNhanCuaBN && ngay < DateTime.Now ? true : false;

                    break;
                case 3:
                    //Ngày tháng năm mặc định là ngày hiện tại cho phép người dùng sửa nhưng không được trước ngày tiếp nhận và sau ngày hiện tại (cho lấy bằng ngày tiếp nhận)
                    if(ngay != null)
                    {
                        var tgTiepNhan = new DateTime(thoiGianTiepNhanCuaBN.Year, thoiGianTiepNhanCuaBN.Month, thoiGianTiepNhanCuaBN.Day);
                        result = ((DateTime)ngay >= tgTiepNhan) && (DateTime)ngay < DateTime.Now ? true : false;
                    }
                    break;

                default:
                break;

            }
            return result; 
        }
        #endregion
    }
}
