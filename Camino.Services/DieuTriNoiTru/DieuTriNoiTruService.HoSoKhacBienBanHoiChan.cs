using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using Camino.Core.Helpers;
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
        public async Task<List<string>> GetListTenNhanVienmAsync()
        {
            var lstTenNuocSanXuat =
                 _nhanVienRepository.TableNoTracking
                    .Where(x => !string.IsNullOrEmpty(x.User.HoTen.Trim()))
                    .Select(x => x.User.HoTen).ToList();
            return lstTenNuocSanXuat;
        }
        public async Task<List<LookupItemVo>> GetChuToa(DropDownListRequestModel model)
        {
            if (model.Id != 0)
            {
                var ten = _nhanVienRepository.TableNoTracking
               .Where(p => p.Id == model.Id && p.User.NhanVien.ChucDanh.NhomChucDanhId == 10)
               .Take(model.Take)
               .Select(i => new LookupItemVo
               {
                   DisplayName = i.User.HoTen,
                   KeyId = i.User.Id
               });
                return ten.ToList();
            }
            else
            {
                var listNhanVien = _nhanVienRepository.TableNoTracking
             .Where(p => p.User.HoTen.Contains(model.Query ?? "") && p.User.NhanVien.ChucDanh.NhomChucDanhId == 10)
              .Take(model.Take)
             .Select(i => new LookupItemVo
             {
                 DisplayName = i.User.HoTen,
                 KeyId = i.User.Id
             });
                return listNhanVien.ToList();
            }
        }
        public async Task<List<LookupItemVo>> GetThuKy(DropDownListRequestModel model)
        {
            if (model.Id != 0)
            {
                var ten = _nhanVienRepository.TableNoTracking
               .Where(p => p.Id == model.Id && p.User.NhanVien.ChucDanh.NhomChucDanhId == 11)
               .Take(model.Take)
               .Select(i => new LookupItemVo
               {
                   DisplayName = i.User.HoTen,
                   KeyId = i.User.Id
               });
                return ten.ToList();
            }
            else
            {
                var listNhanVien = _nhanVienRepository.TableNoTracking
             .Where(p => p.User.HoTen.Contains(model.Query ?? "") && p.User.NhanVien.ChucDanh.NhomChucDanhId == 11)
              .Take(model.Take)
             .Select(i => new LookupItemVo
             {
                 DisplayName = i.User.HoTen,
                 KeyId = i.User.Id
             });
                return listNhanVien.ToList();
            }
        }
        public async Task<List<LookupItemVo>> GetThanhVienThamGia(DropDownListRequestModel model)
        {
            var listNhanVien = _nhanVienRepository.TableNoTracking
               .Where(p => p.User.HoTen.Contains(model.Query ?? ""))
               .Select(i => new LookupItemVo
               {
                   DisplayName = i.User.HoTen,
                   KeyId = i.User.Id
               });
            return listNhanVien.ToList();
        }
        public TrichBienBanHoiChanGridVo GetThongTinTrichBienBanHoiChan(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TrichBienBanHoiChan)
                                                                  .Select(s => new TrichBienBanHoiChanGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.TrichBienBanHoiChan,
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
                                                                  }).OrderBy(c => c.Id).LastOrDefault();
            return query;
        }
        public TrichBienBanHoiChanGridVo GetThongTinTrichBienBanHoiChanViewDS(long noiTruHoSoKhacId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == noiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TrichBienBanHoiChan)
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
                                                                  }).OrderBy(c=>c.Id).LastOrDefault();
            return query;
        }
        public NhanVienNgayThucHien GetTenNguoiThucHien(long idNguoiLogin, long yeuCauTiepNhanId)
        {
            var ngayHienTai = new DateTime();
            ngayHienTai = DateTime.Now;
            var query = BaseRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => new
            {
                daDieuTriTuNgay = s.ThoiDiemTiepNhan,
                taiSoGiuong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.Ten).FirstOrDefault(),
                phong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                Khoa = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault(),
                chanDoan = s.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(x => x.Id).Select(x => x.ChanDoanVaoKhoaGhiChu).FirstOrDefault()
            }).FirstOrDefault();

            var nguoiLoginNgayThucHien = _nhanVienRepository.TableNoTracking.Where(x => x.Id == idNguoiLogin).Select(s => new NhanVienNgayThucHien()
            {
                TenNhanVien = s.User.HoTen,
                NgayThucHien = ngayHienTai.ApplyFormatDateTimeSACH(),
                DaDieuTriTuNgay = query.daDieuTriTuNgay,
                TaiSoGiuong = query.taiSoGiuong,
                Phong = query.taiSoGiuong,
                Khoa = query.Khoa,
                ChanDoan = query.chanDoan
            }).FirstOrDefault();
            return nguoiLoginNgayThucHien;
        }
        public long KiemTraTonTai(long noiTruHoSoKhacId, Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == noiTruHoSoKhacId && x.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru).Select(x => x.Id).ToList();
            if (query.Count() > 0)
            {
                return query.FirstOrDefault();
            }
            else
            {
                return 0;
            }
        }

        //in 
        public async Task<string> BienBanHoiChan(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInTrichBienBanHoiChan.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InThongTinTrichBienBanHoiChan>(thongtinIn);
            var content = "";

            string thanhVienThamGia = "";
            if (queryString.NguoiThamGia != null)
            {
                foreach (var item in queryString.NguoiThamGia)
                {
                    thanhVienThamGia += item + ',';
                }
            }
            DateTime ngayHoiChan = DateTime.Now;
            if (queryString.HoiChanLucStringUTC != null)
            {
                DateTime.TryParseExact(queryString.HoiChanLucStringUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayHoiChan);
            }
           
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("TrichBienBanHoiChan"));
            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanInTrichBienBanHoiChan.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInTrichBienBanHoiChan.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new
                                                                              {
                                                                                  Khoa = x.NoiThucHien.KhoaPhong.Ten,
                                                                                  HoTenBenhNhan = x.YeuCauTiepNhan.HoTen,
                                                                                  Tuoi = (DateTime.Now.Year - x.YeuCauTiepNhan.NamSinh).ToString(),
                                                                                  NamNu = x.YeuCauTiepNhan.GioiTinh.GetDescription(),
                                                                                  NgayThangNam = queryString.DaDieuTriTuNgay != null ? queryString.DaDieuTriTuNgay.Value.ApplyFormatDateTime():"",
                                                                                  DenNgayThangNam =x.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null ? x.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien.Value.ApplyFormatDateTime() :"",
                                                                                  TaiSoGiuong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(p => p.Id).Select(p => p.GiuongBenh.Ten).FirstOrDefault(),
                                                                                  Phong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(p => p.Id).Select(p => p.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                  ChanDoan = x.YeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(p=> p.Id).Select(p => p.ChanDoanVaoKhoaGhiChu).FirstOrDefault(),
                                                                                  Gio = queryString.HoiChanLucStringUTC != null ? ngayHoiChan.Hour.ToString() : string.Empty,
                                                                                  Phut = queryString.HoiChanLucStringUTC != null ? ngayHoiChan.Minute.ToString() : string.Empty,
                                                                                  NgayThangNamHoiChan = queryString.HoiChanLucStringUTC != null ? ngayHoiChan.ApplyFormatDate() : string.Empty,
                                                                                  ChuToa = queryString.ChuToa,
                                                                                  ThuKy = queryString.ThuKy,
                                                                                  ThanhVienThamGia = thanhVienThamGia,
                                                                                  TomTatQuaTrinhDienBienBenh = queryString.TomTat,
                                                                                  KetLuan = queryString.KetLuan,
                                                                                  HuongDieuTriTiep = queryString.HuongDieuTriTiep,
                                                                                  NgayHienTai = DateTime.Now.Day,
                                                                                  ThangHienTai = DateTime.Now.Month,
                                                                                  NamHienTai = DateTime.Now.Year,
                                                                                  HoTenThuKy = queryString.ThuKy,
                                                                                  HotenChuToa = queryString.ChuToa,
                                                                              }).FirstOrDefault();
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        public async Task<GridDataSource> GetDanhSachBienBanHoiChan(QueryInfo queryInfo)
        {
            //
            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TrichBienBanHoiChan)
                 .Select(s => new DanhSachTrichBienBanHoiChanyGridVo()
                 {
                     Id = s.Id,
                     ThongTinHoSo = s.ThongTinHoSo
                 }).ToList();

            var dataOrderBy = query.AsQueryable().OrderBy(cc => cc.Id);
            var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();
        
            foreach (var item in quaythuoc)
            {
                if(item.ThongTinHoSo != null)
                {
                    var queryString = JsonConvert.DeserializeObject<DanhSachTrichBienBanHoiChanyGridVo>(item.ThongTinHoSo);
                    DateTime ngayHoiChan = new DateTime();
                    if (queryString.HoiChanLucStringUTC != null)
                    {
                        DateTime.TryParseExact(queryString.HoiChanLucStringUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayHoiChan);
                        item.NgayHoiChan = ngayHoiChan;
                    }
                    item.ChuToa = queryString.ChuToa;
                    item.ThuKy = queryString.ThuKy;
                    item.ChanDoan = queryString.ChanDoan;
                    if (queryString.NguoiThamGia !=  null)
                    {
                        int length = queryString.NguoiThamGia.Count();
                        int itemLan = 1;
                        foreach (var let in queryString.NguoiThamGia)
                        {
                            item.ThanhVienThamGia += let ;
                            if(length > itemLan)
                            {
                                item.ThanhVienThamGia += ",";
                            }
                            itemLan++;
                        }
                    }
                }
                
            }
            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
        }
    }
}
