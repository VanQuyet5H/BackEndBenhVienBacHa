using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
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
        public GiayChungNhanNghiViecHuongBHXHGrid GetThongTinGiayChungNhanNghiViecHuongBHXH(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH)
                                                                  .Select(s => new GiayChungNhanNghiViecHuongBHXHGrid()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                  }).FirstOrDefault();
            return query;
        }
        public ThongTinChungNhanNghiViecHuongBHXH GetDataChungNhanNghiViecHuongBHXH(long yeuCauTiepNhanId)
        {
            ThongTinChungNhanNghiViecHuongBHXH thongTinChungNhanNghiViecHuongBHXH = new ThongTinChungNhanNghiViecHuongBHXH();
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            thongTinChungNhanNghiViecHuongBHXH.TenNhanVien = nguoiLogin;
            var query = BaseRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => new
            {
                chanDoan = s.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu,
                
            }).FirstOrDefault();
            var entity = _noiTruBenhAnRepository.TableNoTracking.Include(p=>p.ThongTinTongKetBenhAn).Where(s=>s.Id == yeuCauTiepNhanId).Select(o=>o.ThongTinTongKetBenhAn);
            var phuongPhapDieuTri = "";

            if (!string.IsNullOrEmpty(entity.LastOrDefault()))
            {
               var  result = JsonConvert.DeserializeObject<PhuongPhapDieuTriModel>(entity.LastOrDefault());
                phuongPhapDieuTri = result.PhuongPhapDieuTri;
            }
            thongTinChungNhanNghiViecHuongBHXH.ChanDoanVaPhuongPhapDieuTri = query.chanDoan + phuongPhapDieuTri;
            thongTinChungNhanNghiViecHuongBHXH.NgayThucHienDisplay = DateTime.Now.ApplyFormatDateTime();
            return thongTinChungNhanNghiViecHuongBHXH;
        }

        public async Task<string> InGiayChungNhanNghiViecHuongBHXH(XacNhanInPhieuGiayChungNhanNghiViecHuongBHXH xacNhanIn)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => (xacNhanIn.NoiTruHoSoKhacId == 0  || (x.Id == xacNhanIn.NoiTruHoSoKhacId && xacNhanIn.NoiTruHoSoKhacId != 0)) && x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var content = "";
            if (thongtinIn == null)
            {
                return content;
            }
            var queryString = JsonConvert.DeserializeObject<InPhieuGiayChungNhanNghiViecHuongBHXH>(thongtinIn);
           
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("GiayXacNhanNghiViecHuongBHXH"));
            var TenBenhVien = "BỆNH VIỆN ĐKQT BẮC HÀ";
            var chanDoan = "";
            var tmp = "\n";
            var replace = "<br>";
            if(queryString.ChanDoanVaPhuongPhapDieuTri!= null)
            {
                chanDoan = queryString.ChanDoanVaPhuongPhapDieuTri.Replace(tmp, replace);
            }
            var soSeri = "";
            if (!string.IsNullOrEmpty(queryString.SoSeri) && queryString.SoSeri !=" ")
            {
                soSeri = queryString.SoSeri;
            }
            else
            {
                soSeri = ".............................";
            }
            var mauSo = "";
            if (!string.IsNullOrEmpty(queryString.MauSo) && queryString.MauSo != " ")
            {
                mauSo = queryString.SoSeri;
            }
            else
            {
                mauSo = ".............................";
            }
            var tuNgayDisplay = string.Empty;
            var denNgayDisplay = string.Empty;

            if (!string.IsNullOrEmpty(queryString.NghiTuNgayDisplay) && !string.IsNullOrEmpty(queryString.NghiDenNgayDisplay))
            {

                DateTime.TryParseExact(queryString.NghiTuNgayDisplay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgay);
                //tuNgay = new DateTime(tuNgayTemp.Year, tuNgayTemp.Month, tuNgayTemp.Day, 0, 0, 0);
                tuNgayDisplay = tuNgay.ApplyFormatDate();

                DateTime.TryParseExact(queryString.NghiDenNgayDisplay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgay);
                //denNgay = new DateTime(denNgayTemp.Year, denNgayTemp.Month, denNgayTemp.Day, 23, 59, 59);
                denNgayDisplay = denNgay.ApplyFormatDate();
            }
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru)
                .Select(s => new { 
                    So = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTenBenhNhan = s.YeuCauTiepNhan.HoTen,
                    SinhNgay = s.YeuCauTiepNhan.NgaySinh + "/ " + s.YeuCauTiepNhan.ThangSinh +" / " +s.YeuCauTiepNhan.NamSinh,
                    BHYTMaSoThe = s.YeuCauTiepNhan.BHYTMaSoThe,
                    GioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    NoiLamViec = s.YeuCauTiepNhan.NoiLamViec,
                    NgayHienTai =DateTime.Now.Day,
                    ThangHienTai = DateTime.Now.Month,
                    NamHienTai = DateTime.Now.Year,
                    ChanDoanPhuongPhap = chanDoan,
                    SoNgayNghi = queryString.SoNgayNghi,
                    ThoiDiemTiepNhan = tuNgayDisplay,
                    DenNgay = denNgayDisplay,
                    HoTenCha = queryString.HoTenCha,
                    HoTenMe = queryString.HoTenMe,
                    NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                    SoSeRi = soSeri,
                    MauSo =mauSo
                }).FirstOrDefault();
          

            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, query);
            return content;
        }
        public async Task<string> GetMaBS(string searching)
        {
            var maNV= _nhanVienRepository.TableNoTracking
                                                        .Where(d=> d.User.HoTen.ToUpper() == searching.ToUpper())
                                                        .Select(d=>d.MaChungChiHanhNghe)
                                                        .FirstOrDefault();

            return maNV;
        }
      
       
    }
}
