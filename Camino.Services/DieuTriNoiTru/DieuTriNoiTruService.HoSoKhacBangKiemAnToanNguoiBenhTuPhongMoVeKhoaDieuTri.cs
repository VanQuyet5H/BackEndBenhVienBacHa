using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BangKiemAnToanNguoiBenhPTTuPhongDieuTri;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<List<string>> GetDanhSachPhauThuatVien()
        {
            var lstDsNhanVien =
                 _nhanVienRepository.TableNoTracking
                    .Where(x => !string.IsNullOrEmpty(x.User.HoTen.Trim()))
                    .Select(x => x.User.HoTen).ToList();
            return lstDsNhanVien;
        }
        public async Task<List<DichVuKyThuatChoBenhVienTemplateVo>> GetDanhSachXeNghiemCanLam(DropDownListRequestModel model)
        {
            var listDichVuKyThuats = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Include(p => p.DichVuKyThuat)
                .Where(p => (p.Ten.Contains(model.Query ?? "") ||
                             p.Ma.Contains(model.Query ?? "")) && p.HieuLuc && p.LoaiMauXetNghiem != null)
                .Take(model.Take)
                .ToListAsync();
            var query = listDichVuKyThuats.Select(item => new DichVuKyThuatChoBenhVienTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ma = item.Ma,
                Ten = item.Ten
            }).ToList();

            return query;
        }
        public async Task<List<LookupItemVo>> GetListThuocDaDung(DropDownListRequestModel model,long yeuCauTiepNhanId)
        {
            var list = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ten)

                .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            p.SoLuong > 0 &&
                            p.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien &&
                            p.ThoiDiemChiDinh < DateTime.Now)
                .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.DuocPhamBenhVienId
                }).Distinct();
            return await list.ToListAsync();
        }
        public async Task<List<LookupItemVo>> GetListThuocDangDung(DropDownListRequestModel model, long yeuCauTiepNhanId)
        {
            var list = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ten)

                .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            p.SoLuong > 0 &&
                            p.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.ChuaThucHien 
                            )
                 .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.DuocPhamBenhVienId
                });
            return await list.ToListAsync();
        }
        public async Task<List<LookupItemVo>> GetListThuocBanGiao(DropDownListRequestModel model)
        {
            var lstDsDuocPhamQuocGia =
               _duocPhamBenhVienRepository.TableNoTracking
                 .ApplyLike(model.Query, g => g.DuocPham.Ten)
                  .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.DuocPham.Ten,
                    KeyId = i.Id
                });
            return await lstDsDuocPhamQuocGia.ToListAsync();
        }
        public async Task<GridDataSource> GetDanhSachNguoiBenhAnToanPTTuPhongDieuTri(QueryInfo queryInfo)
        {

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhTuPhongMoVePhongDieuTri)
                 .Select(s => new BangKiemAnToanNguoiBenhPTTuPhongDieuTriGridVo()
                 {
                     Id = s.Id,
                     ThongTinHoSo = s.ThongTinHoSo
                 }).ToList();

            var dataOrderBy = query.AsQueryable().OrderBy(cc => cc.Id);
            var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();
            foreach (var item in quaythuoc)
            {
                var queryString = JsonConvert.DeserializeObject<PartObject>(item.ThongTinHoSo);
                DateTime ngaynhan = DateTime.Now;
                DateTime.TryParseExact(queryString.NgayNhanUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngaynhan);
                item.HoTenPTV = queryString.HoTenPTV;
                item.Ngay = ngaynhan;
            }
            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
        }
        public  BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid GetThongTinBangKiemAnToanNguoiBenhPTTuPhongDieuTri(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhTuPhongMoVePhongDieuTri)
                                                                  .Select(s => new BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhTuPhongMoVePhongDieuTri,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyGridVo()
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
                                                                  }).LastOrDefault();
            return query;
        }
        public BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid GetThongTinBenhNhanPtVephongDieuTriViewDS(long noiTruHoSoKhacId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == noiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhTuPhongMoVePhongDieuTri)
                                                                  .Select(s => new BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid()
                                                                  {
                                                                      YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhTuPhongMoVePhongDieuTri,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyGridVo()
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
                                                                  }).LastOrDefault();
            return query;
        }
        public async Task<string> InBangKiemAnToanNguoiBenhPTVeKhoaDieuTri(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();          
            var replaceData = thongtinIn.Replace("\\n", "<br/>");
            var queryString = JsonConvert.DeserializeObject<InBangKiemBenhNhanPhauThuatVeKhoaDieuTri>(replaceData);

            var content = "";

            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var khoa = string.Empty;
            khoa = _khoaPhongRepository.TableNoTracking.Where(d => d.Id == khoaId).Select(d => d.Ten).FirstOrDefault();

            if (!string.IsNullOrEmpty(khoa))
            {
                var tenKhoaPhong = khoa.Replace("Khoa", "");
                khoa = "<b>Khoa</b>" + tenKhoaPhong;
            }
            var ngay = string.Empty;
            var thang = string.Empty;
            var nam = string.Empty;
            var thoiDiemThucHien = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru).Select(d => d.ThoiDiemThucHien).FirstOrDefault();
            if (thoiDiemThucHien != null)
            {
                ngay = thoiDiemThucHien.Day < 9 ? "0"+ thoiDiemThucHien.Day : thoiDiemThucHien.Day +"";
                thang = thoiDiemThucHien.Month < 9 ? "0" + thoiDiemThucHien.Month : thoiDiemThucHien.Month  +"";
                nam = thoiDiemThucHien.Year +"";
            }

            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("BangKiemNhanBenhNhanTuPhongMoVeKhoaDieuTri"));
            var maTn = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId).Select(d => d.MaYeuCauTiepNhan).FirstOrDefault();
            var genBarCode = !string.IsNullOrEmpty(maTn) ? BarcodeHelper.GenerateBarCode(maTn) : "";
            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new
                                                                              {
                                                                                  Khoa = khoa,
                                                                                  MaTN = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                                  KiemTraTruocKhiLenPMCo = queryString.KiemTraTruocKhiLenPM == true ? "X" : "&nbsp;",
                                                                                  KiemTraTruocKhiLenPMKhong = queryString.KiemTraTruocKhiLenPM == false ? "X" : "&nbsp;",

                                                                                  KiemTraTruocKhiRachDaCo = queryString.KiemTraTruocKhiRachDa == true ? "X" : "&nbsp;",
                                                                                  KiemTraTruocKhiRachDaKhong = queryString.KiemTraTruocKhiRachDa == false ? "X" : "&nbsp;",

                                                                                  ThuocDangDungCo = queryString.ThuocDangDungValue == true ? "X" : "&nbsp;",
                                                                                  ThuocDangDungKhong = queryString.ThuocDangDungValue == false ? "X" : "&nbsp;",

                                                                                  ThuocBanGiaoCo = queryString.ThuocBanGiaoValue == true ? "X" : "&nbsp;",
                                                                                  ThuocBanGiaoKhong = queryString.ThuocBanGiaoValue == false ? "X" : "&nbsp;",



                                                                                  KichThichVatVaKhong = queryString.KichThichVatVa == false ? "X" : "&nbsp;",
                                                                                  KichThichVatVaCo = queryString.KichThichVatVa == true ? "X" : "&nbsp;",

                                                                                  TinhTaoKhong = queryString.TinhTao == false ? "X" : "&nbsp;",
                                                                                  TinhTaoCo = queryString.TinhTao == true ? "X" : "&nbsp;",
                                                                                  DHSinhTonOnDinhKhong = queryString.DHSinhTonOnDinh == false ? "X" : "&nbsp;",
                                                                                  DHSinhTonOnDinhCo = queryString.DHSinhTonOnDinh == true ? "X" : "&nbsp;",
                                                                                  NghiNgoChayMauKhong = queryString.NghiNgoChayMau == false ? "X" : "&nbsp;",
                                                                                  NghiNgoChayMauCo = queryString.NghiNgoChayMau == true ? "X" : "&nbsp;",
                                                                                  SuyHoHapKhong = queryString.SuyHoHap == false ? "X" : "&nbsp;",
                                                                                  SuyHoHapCo = queryString.SuyHoHap == true ? "X" : "&nbsp;",
                                                                                  NonNacKhong = queryString.NonNac == false ? "X" : "&nbsp;",
                                                                                  NonNacCo = queryString.NonNac == true ? "X" : "&nbsp;",
                                                                                  CauBangQuangKhong = queryString.CauBangQuang == false ? "X" : "&nbsp;",
                                                                                  CauBangQuangCo = queryString.CauBangQuang == true ? "X" : "&nbsp;",
                                                                                  VanTimTrenDaKhong = queryString.VanTimTrenDa == false ? "X" : "&nbsp;",
                                                                                  VanTimTrenDaCo = queryString.VanTimTrenDa == true ? "X" : "&nbsp;",
                                                                                  DauNhieuKhong = queryString.DauNhieu == false ? "X" : "&nbsp;",
                                                                                  DauNhieuCo = queryString.DauNhieu == true ? "X" : "&nbsp;",
                                                                                  TruyenDichKhong = queryString.TruyenDich == false ? "X" : "&nbsp;",
                                                                                  TruyenDichCo = queryString.TruyenDich == true ? "X" : "&nbsp;",
                                                                                  ViTri = queryString.ViTri ,
                                                                                  //ViTriCo = queryString.ViTri == true ? "X" : "&nbsp;",
                                                                                  LuuThongKhong = queryString.LuuThong == false ? "X" : "&nbsp;",
                                                                                  LuuThongCo = queryString.LuuThong == true ? "X" : "&nbsp;",
                                                                                  SachKhong = queryString.Sach == false ? "X" : "&nbsp;",
                                                                                  SachCo = queryString.Sach == true ? "X" : "&nbsp;",
                                                                                  OngThongDaDayKhong = queryString.OngThongDaDay == false ? "X" : "&nbsp;",
                                                                                  OngThongDaDayCo = queryString.OngThongDaDay == true ? "X" : "&nbsp;",
                                                                                  OngThongTieuKhong = queryString.OngThongTieu == false ? "X" : "&nbsp;",
                                                                                  OngThongTieuCo = queryString.OngThongTieu == true ? "X" : "&nbsp;",
                                                                                  DanLuuKhong = queryString.DanLuu == false ? "X" : "&nbsp;",
                                                                                  DanLuuCo = queryString.DanLuu == true ? "X" : "&nbsp;",
                                                                                  ApLucKhong = queryString.ApLuc == false ? "X" : "&nbsp;",
                                                                                  ApLucCo = queryString.ApLuc == true ? "X" : "&nbsp;",
                                                                                
                                                                                  BangKho = queryString.BangKhoValue ,
                                                                                  ThamDich = queryString.ThamDichValue ,
                                                                                  TuTrangBNKhong = queryString.TuTrangBenhNhan == false ? "X" : "&nbsp;",
                                                                                  TuTrangBNCo = queryString.TuTrangBenhNhan == true ? "X" : "&nbsp;",
                                                                                  BANgoaiKhoaHoanThienKhong = queryString.BAKhoaHoanThien == false ? "X" : "&nbsp;",
                                                                                  BANgoaiKhoaHoanThienCo = queryString.BAKhoaHoanThien == true ? "X" : "&nbsp;",
                                                                                  BVDKQTBacHa = queryString.BV == true ? "X" :"&nbsp;",
                                                                                  HopTac = queryString.HT == true ? "X" : "&nbsp;",
                                                                                  GiaiPhauBenhKhong = queryString.GiaiPhauBenh == false ? "X" : "&nbsp;",
                                                                                  GiaiPhauBenhCo = queryString.GiaiPhauBenh == true ? "X" : "&nbsp;",
                                                                                  SoMauBenh = queryString.SoMauBenhPham,
                                                                                  KetQuaFilm = queryString.KetQuaFilm,
                                                                                  Thuong = queryString.DaKyChonPhong == true ? "X" : "&nbsp;",
                                                                                  Vip = queryString.DaKyChonPhong == false ? "X" : "&nbsp;",
                                                                                  XeNghiemCanLam = queryString.XetNghiemCanLam,
                                                                                  CapMot = queryString.Cap1 == true ? "X" : "&nbsp;",
                                                                                  CapHai = queryString.Cap2 == true ? "X" : "&nbsp;",
                                                                                  CapBa = queryString.Cap3 == true ? "X" : "&nbsp;",
                                                                                  ChiDinhTheoDoi = queryString.ChiDinhTheoDoi,
                                                                                  ThuocDaDung = queryString.ThuocDangDungValue,
                                                                                  ThuocThuongDungKhong = queryString.ThuocDangDungValue == true ? "X" : "&nbsp;",
                                                                                  ThuocThuongDungCo = queryString.ThuocDangDungValue != true ? "X" : "&nbsp;",
                                                                                 
                                                                                  NgayHienTai = ngay,
                                                                                  ThangHienTai = thang,
                                                                                  NamHienTai = nam,
                                                                                  HoTenPTV = queryString.HoTenPTV,
                                                                                  NguoiNhan = queryString.NguoiNhan,
                                                                                  NguoiGiao = queryString.NguoiGiao,
                                                                                  SoLuongMauSacOngThongTieu = queryString.SoLuongMauSacOngThongTieu,
                                                                                  SoLuongMauSacDaDay = queryString.SoLuongMauSacDaDay,
                                                                                  MauSacOngThongTieu = queryString.MauSacOngThongTieu,
                                                                                  MauSacDaDay = queryString.MauSacDaDay,
                                                                                  ViTriDanLuu = queryString.ViTriDanLuu,
                                                                                  NoiCongTac = queryString.NoiCongTac,
                                                                                  CheDoAnUong = queryString.CheDoAnUong,
                                                                                  MauSacDich = queryString.MauSacDich,
                                                                                  SoLuongMauSacDich =queryString.SoLuongMauSacDich,
                                                                                  NhapTuTrang = queryString.NhapTuTrang,


                                                                                  BangTheoDoiGMHSKhong = queryString.BangTheoDoiGMHS == false ? "X" : "&nbsp;",
                                                                                  BangTheoDoiGMHSCo = queryString.BangTheoDoiGMHS == true ? "X" : "&nbsp;",
                                                                                  BangTheoDoiHoiTinhKhong = queryString.BangTheoDoiHoiTinh == false ? "X" : "&nbsp;",
                                                                                  BangTheoDoiHoiTinhCo = queryString.BangTheoDoiHoiTinh == true ? "X" : "&nbsp;",

                                                                                  PhieuDemGacKhong = queryString.PhieuDemGac == false ? "X" : "&nbsp;",
                                                                                  PhieuDemGacCo = queryString.PhieuDemGac == true ? "X" : "&nbsp;",
                                                                                  PhieuTheoDoiTeNMCKhong = queryString.PhieuTheoDoiTeNMC == false ? "X" : "&nbsp;",
                                                                                  PhieuTheoDoiTeNMCCo = queryString.PhieuTheoDoiTeNMC == true ? "X" : "&nbsp;",
                                                                                  PhieuVTNgoaiKhong = queryString.PhieuVTNgoai == false ? "X" : "&nbsp;",
                                                                                  PhieuVTNgoaiCo = queryString.PhieuVTNgoai == true ? "X" : "&nbsp;",
                                                                                  PhieuVTTHGMPTKhong = queryString.PhieuVTTHGMPT == false ? "X" : "&nbsp;",
                                                                                  PhieuVTTHGMPTCo = queryString.PhieuVTTHGMPT == true ? "X" : "&nbsp;",
                                                                                  SoLuongFilmKhong = queryString.SoLuongFilm == false ? "X" : "&nbsp;",
                                                                                  SoLuongFilmCo = queryString.SoLuongFilm == true ? "X" : "&nbsp;",
                                                                                  QuanAo = queryString.QuanAoVay ,
                                                                                  BarCodeImgBase64 = genBarCode
                                                                              }).FirstOrDefault();
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
    }
}
