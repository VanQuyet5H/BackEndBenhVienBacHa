using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo;
using Camino.Core.Helpers;
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
        public GiayChungNhanSinhMangThaiHoGrid GetThongTinGiayChungSinhMangThaiHo(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinhMangThaiHo)
                                                                  .Select(s => new GiayChungNhanSinhMangThaiHoGrid()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinhMangThaiHo,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                  }).FirstOrDefault();
            return query;
        }
        public ThongTinGiayChungNhanSinhMangThaiHo GetDataGiayChungSinhMangThaiHoCreate(long yeuCauTiepNhanId)
        {
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            var tongKetBenhAnKhoaSan = _noiTruBenhAnRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => s.ThongTinTongKetBenhAn).FirstOrDefault();
            if(tongKetBenhAnKhoaSan != null)
            {
                var thongtinDacDiemTreSoSinhs = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnKhoaSanGrid>(tongKetBenhAnKhoaSan);
                List<ThongTinDacDiemTreSoSinhGridVo> list = new List<ThongTinDacDiemTreSoSinhGridVo>();
               
                foreach (var item in thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs)
                {
                    ThongTinDacDiemTreSoSinhGridVo listItem = new ThongTinDacDiemTreSoSinhGridVo();
                    listItem.CanNang = item.CanNang;
                    listItem.GioiTinh = item.GioiTinh;
                    listItem.TinhTrang = item.TinhTrang;
                    listItem.Id = item.Id;
                    listItem.DeLuc = item.DeLuc;
                    listItem.DeLucDisplayName = item.DeLucDisplayName;
                    list.Add(listItem);
                }
                var data = BaseRepository.TableNoTracking.Where(x => x.Id == yeuCauTiepNhanId)
                    .Select(s => new ThongTinGiayChungNhanSinhMangThaiHo()
                    {
                        HoTenVoMangThaiHo = s.HoTen,
                        NamSinhVoMangThaiHo = s.NamSinh,
                        CMNDHoChieuVomangThaiHo = s.BenhNhan.SoChungMinhThu,
                        DanTocVoMangThaiHo = s.DanToc.Ten,
                        NoiDangKyThuongTruVoMangThaiHo = s.BenhNhan.DiaChiDayDu,
                        TaiKhoanDangNhap = nguoiLogin,
                        DacDiemTreSoSinhs = list,
                        NgayThucHienDisplay = DateTime.Now.ApplyFormatDateTimeSACH(),
                        NgayThucHienString = DateTime.Now.ApplyFormatDateTimeSACH()
                    }).FirstOrDefault();
                return data;
            }
            if (tongKetBenhAnKhoaSan == null)
            {
                List<ThongTinDacDiemTreSoSinhGridVo> list = new List<ThongTinDacDiemTreSoSinhGridVo>();
                var data = BaseRepository.TableNoTracking.Where(x => x.Id == yeuCauTiepNhanId)
                    .Select(s => new ThongTinGiayChungNhanSinhMangThaiHo()
                    {
                        HoTenVoMangThaiHo = s.HoTen,
                        NamSinhVoMangThaiHo = s.NamSinh,
                        CMNDHoChieuVomangThaiHo = s.BenhNhan.SoChungMinhThu,
                        DanTocVoMangThaiHo = s.DanToc.Ten,
                        NoiDangKyThuongTruVoMangThaiHo = s.BenhNhan.DiaChiDayDu,
                        TaiKhoanDangNhap = nguoiLogin,
                        DacDiemTreSoSinhs = list,
                        NgayThucHienDisplay = DateTime.Now.ApplyFormatDateTimeSACH(),
                        NgayThucHienString = DateTime.Now.ApplyFormatDateTimeSACH()
                    }).FirstOrDefault();
                return data;
            }
            return null;
        }
        public async Task<string> InGiayChungSinhMangThaiHo(XacNhanInPhieuGiaySinhMangThaiHo xacNhanIn)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanIn.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<ThongTinGiayChungNhanSinhMangThaiHo>(thongtinIn);
            var thoiGianRaVien = JsonConvert.DeserializeObject<ThongTinRaVien>(thongtinIn);
            var content = "";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("GiayChungSinhMangThaiHo"));
            var soLien1 = "";
            int tongCon = 0;
            var giayChungSinh1 = "";
            var giayChungSinh2 = "";
            if (queryString.DacDiemTreSoSinhs.Count > 0)
            {
                tongCon = queryString.DacDiemTreSoSinhs.Count();
                var countPage = 1;
                foreach (var itemCon in queryString.DacDiemTreSoSinhs)
                {
                    var resultChungSinh1 = _templateRepository.TableNoTracking
                                          .FirstOrDefault(x => x.Name.Equals("GiayChungSinhLoai1"));
                    var resultChungSinh2 = _templateRepository.TableNoTracking
                                         .FirstOrDefault(x => x.Name.Equals("GiayChungSinhLoai2"));

                    var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru)
                                                                             .Select(x => new
                                                                             {
                                                                                So = queryString.So,
                                                                                QuyenSo = queryString.QuyenSo,
                                                                                HoTenVoNho  = queryString.HoTenVoNhoMangThaiHo,
                                                                                NamSinhVoNho = queryString.NamSinhVoNhoMangThaiHo,
                                                                                 CMNDHoChieuVoNho = queryString.CMNDHoChieuVoNhomangThaiHo,
                                                                                 DanTocVoNho = DanTocDisplay(queryString.DanTocVoNhoMangThaiHo),
                                                                                 NoiDangKyThuongTruVoNho = queryString.NoiDangKyThuongTruVoNhoMangThaiHo,
                                                                                 HoTenChongNho = queryString.HoTenChongNhoMangThaiHo,
                                                                                 NamSinhChongNho = queryString.NamSinhChongNhoMangThaiHo,
                                                                                 CMNDHoChieuChongNho = queryString.CMNDHoChieuChongNhomangThaiHo,
                                                                                 DanTocChongNho = DanTocDisplay(queryString.DanTocChongNhoMangThaiHo),
                                                                                 NoiDangKyThuongTruChongNho = queryString.NoiDangKyThuongTruChongNhoMangThaiHo,
                                                                                 //
                                                                                 HoTenVo = queryString.HoTenVoMangThaiHo,
                                                                                 NamSinhVo =queryString.NamSinhVoMangThaiHo,
                                                                                 CMNDHoChieuVo = queryString.CMNDHoChieuVomangThaiHo,
                                                                                 DanTocVo = DanTocDisplay(queryString.DanTocVoMangThaiHo),
                                                                                 NoiDangKyThuongTruVo = queryString.NoiDangKyThuongTruVoMangThaiHo,
                                                                                 HoTenChong = queryString.HoTenChongMangThaiHo,
                                                                                 NamSinhChong = queryString.NamSinhChongMangThaiHo,
                                                                                 CMNDHoChieuChong = queryString.CMNDHoChieuChongmangThaiHo,
                                                                                 DanTocChong = DanTocDisplay(queryString.DanTocChongMangThaiHo),
                                                                                 NoiDangKyThuongTruChong = queryString.NoiDangKyThuongTruChongMangThaiHo,
                                                                                 DaSinhConVaoLuc = itemCon.DeLuc != null ? itemCon.DeLuc.GetValueOrDefault().Hour + " giờ " + itemCon.DeLuc.GetValueOrDefault().Minute +" phút " + "Hà nội" + " ngày "+ itemCon.DeLuc.GetValueOrDefault().Day + " tháng " + itemCon.DeLuc.GetValueOrDefault().Month + " năm " + itemCon.DeLuc.GetValueOrDefault().Year  
                                                                                                                                         : "&nbsp;&nbsp;&nbsp;" + " giờ " + "&nbsp;&nbsp;&nbsp;" + " phút    " + "Hà nội" + " ngày " + "&nbsp;&nbsp;&nbsp;" + " tháng " + "&nbsp;&nbsp;&nbsp;" + " năm " + "&nbsp;&nbsp;&nbsp;",

                                                                                 SoLanSinh = queryString.SoLanSinh,
                                                                                 SoConHienSong = queryString.SoConHienSong,
                                                                                 SoConTrongLanSinhNay = tongCon, 
                                                                                 GioiTinhCuaCon = itemCon.GioiTinh,
                                                                                 CanNang = itemCon.CanNang,
                                                                                 HienTrangSKCuaCon =itemCon.TinhTrang,
                                                                                 DuDinhDatTenCon = itemCon.DuDinhDatTenCon,
                                                                                 NguoiDoDe = queryString.NguoiDoDe,
                                                                                 NgayHienTai =DateTime.Now.Day,
                                                                                 ThangHienTai = DateTime.Now.Month,
                                                                                 NamHienTai = DateTime.Now.Year
                                                                             }).FirstOrDefault();


                    giayChungSinh1 = TemplateHelpper.FormatTemplateWithContentTemplate(resultChungSinh1.Body, data);
                    giayChungSinh2 = TemplateHelpper.FormatTemplateWithContentTemplate(resultChungSinh2.Body, data);
                    var giayChungSinhMangThaiHo = new GiayChunGSinhMangThaiHo{
                        HTMLGiayChungSinh1 = giayChungSinh1,
                        HTMLGiayChungSinh2 = giayChungSinh2
                    };
                    if(countPage > 1)
                    {
                        content +="<div style='break-after:page'></div>";
                    }
                content += TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, giayChungSinhMangThaiHo);
                    countPage++;
            }
            }
            else
            {
                tongCon = queryString.DacDiemTreSoSinhs.Count();
                var resultChungSinh1 = _templateRepository.TableNoTracking
                                           .FirstOrDefault(x => x.Name.Equals("GiayChungSinhLoai1"));
                var resultChungSinh2 = _templateRepository.TableNoTracking
                                     .FirstOrDefault(x => x.Name.Equals("GiayChungSinhLoai2"));

                var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru)
                                                                         .Select(x => new
                                                                         {
                                                                             So = queryString.So,
                                                                             QuyenSo = queryString.QuyenSo,
                                                                             HoTenVoNho = queryString.HoTenVoNhoMangThaiHo,
                                                                             NamSinhVoNho = queryString.NamSinhVoNhoMangThaiHo,
                                                                             CMNDHoChieuVoNho = queryString.CMNDHoChieuVoNhomangThaiHo,
                                                                             DanTocVoNho = DanTocDisplay(queryString.DanTocVoNhoMangThaiHo),
                                                                             NoiDangKyThuongTruVoNho = queryString.NoiDangKyThuongTruVoNhoMangThaiHo,
                                                                             HoTenChongNho = queryString.HoTenChongNhoMangThaiHo,
                                                                             NamSinhChongNho = queryString.NamSinhChongNhoMangThaiHo,
                                                                             CMNDHoChieuChongNho = queryString.CMNDHoChieuChongNhomangThaiHo,
                                                                             DanTocChongNho = DanTocDisplay(queryString.DanTocChongNhoMangThaiHo),
                                                                             NoiDangKyThuongTruChongNho = queryString.NoiDangKyThuongTruChongNhoMangThaiHo,
                                                                                 //
                                                                                 HoTenVo = queryString.HoTenVoMangThaiHo,
                                                                             NamSinhVo = queryString.NamSinhVoMangThaiHo,
                                                                             CMNDHoChieuVo = queryString.CMNDHoChieuVomangThaiHo,
                                                                             DanTocVo = DanTocDisplay(queryString.DanTocVoMangThaiHo),
                                                                             NoiDangKyThuongTruVo = queryString.NoiDangKyThuongTruVoMangThaiHo,
                                                                             HoTenChong = queryString.HoTenChongMangThaiHo,
                                                                             NamSinhChong = queryString.NamSinhChongMangThaiHo,
                                                                             CMNDHoChieuChong = queryString.CMNDHoChieuChongmangThaiHo,
                                                                             DanTocChong = DanTocDisplay(queryString.DanTocChongMangThaiHo),
                                                                             NoiDangKyThuongTruChong = queryString.NoiDangKyThuongTruChongMangThaiHo,
                                                                             DaSinhConVaoLuc = thoiGianRaVien.ThoiGianRaVien != null ? thoiGianRaVien.ThoiGianRaVien.GetValueOrDefault().Hour + " giờ " + thoiGianRaVien.ThoiGianRaVien.GetValueOrDefault().Minute + " phút " + "Hà nội" + " ngày " + thoiGianRaVien.ThoiGianRaVien.GetValueOrDefault().Day + " tháng " + thoiGianRaVien.ThoiGianRaVien.GetValueOrDefault().Month + " năm " + thoiGianRaVien.ThoiGianRaVien.GetValueOrDefault().Year
                                                                                                                                     : "&nbsp;&nbsp;&nbsp;" + " giờ " + "&nbsp;&nbsp;&nbsp;" + " phút    " + "Hà nội" + " ngày " + "&nbsp;&nbsp;&nbsp;" + " tháng " + "&nbsp;&nbsp;&nbsp;" + " năm " + "&nbsp;&nbsp;&nbsp;",
                                                                             SoLanSinh = queryString.SoLanSinh,
                                                                             SoConHienSong = queryString.SoConHienSong,
                                                                             SoConTrongLanSinhNay = tongCon,
                                                                             GioiTinhCuaCon = "",
                                                                             CanNang = "",
                                                                             HienTrangSKCuaCon = "",
                                                                             DuDinhDatTenCon = "",
                                                                             NguoiDoDe = queryString.NguoiDoDe,
                                                                             NgayHienTai = DateTime.Now.Day,
                                                                             ThangHienTai = DateTime.Now.Month,
                                                                             NamHienTai = DateTime.Now.Year
                                                                         }).FirstOrDefault();


                giayChungSinh1 = TemplateHelpper.FormatTemplateWithContentTemplate(resultChungSinh1.Body, data);
                giayChungSinh2 = TemplateHelpper.FormatTemplateWithContentTemplate(resultChungSinh2.Body, data);
                var giayChungSinhMangThaiHo = new GiayChunGSinhMangThaiHo
                {
                    HTMLGiayChungSinh1 = giayChungSinh1,
                    HTMLGiayChungSinh2 = giayChungSinh2
                };
                content += TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, giayChungSinhMangThaiHo);
            }
            return content;
        }
        private string DanTocDisplay(string danTocId)
        {
            if(IsInt(danTocId))
            {
                long id;
                long.TryParse(danTocId, out id);
                var result = _danTocRepository.TableNoTracking.Where(d => d.Id == id).Select(d => d.Ten);
                return result.FirstOrDefault();
            }
            return null;
        }
        private bool IsInt(string sVal)
        {
            double test;
            return double.TryParse(sVal, out test);
        }
        public List<ThongTinDacDiemTreSoSinhGridVo> GetDacDiemTreSoSinh(long yeuCauTiepNhanId)
        {
            var tongKetBenhAnKhoaSan = _noiTruBenhAnRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => s.ThongTinTongKetBenhAn).FirstOrDefault();
            if (tongKetBenhAnKhoaSan != null)
            {
                var thongtinDacDiemTreSoSinhs = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnKhoaSanGrid>(tongKetBenhAnKhoaSan);
                List<ThongTinDacDiemTreSoSinhGridVo> list = new List<ThongTinDacDiemTreSoSinhGridVo>();

                foreach (var item in thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs)
                {
                    ThongTinDacDiemTreSoSinhGridVo listItem = new ThongTinDacDiemTreSoSinhGridVo();
                    listItem.CanNang = item.CanNang;
                    listItem.GioiTinh = item.GioiTinh;
                    listItem.TinhTrang = item.TinhTrang;
                    listItem.Id = item.Id;
                    list.Add(listItem);
                }
                return list;
            }
            if (tongKetBenhAnKhoaSan == null)
            {
                List<ThongTinDacDiemTreSoSinhGridVo> list = new List<ThongTinDacDiemTreSoSinhGridVo>();
               
                return list;
            }
            return null;
        }
    }
}
