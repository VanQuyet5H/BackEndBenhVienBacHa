using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Newtonsoft.Json;

using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public string InGiayChungSinh(long noiTruHoSoKhacId,string hos)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayChungSinhNoiTru")).First();
            var noiTruHoSoKhac = _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.Id == noiTruHoSoKhacId).FirstOrDefault();
            var yeuCauTiepNhan = BaseRepository.TableNoTracking
                                                .Include(p => p.NoiTruBenhAn)
                                                .Include(p => p.DanToc)
                                                .Include(p => p.YeuCauTiepNhanTheBHYTs)
                                                .Include(p => p.YeuCauNhapVien).ThenInclude(p => p.YeuCauTiepNhanMe)
                                                .Where(p => p.Id == noiTruHoSoKhac.YeuCauTiepNhanId).FirstOrDefault();

            var thongTinHoSo = JsonConvert.DeserializeObject<GiayChungSinhNewJSONVo>(noiTruHoSoKhac.ThongTinHoSo);
            var nhanVienDoDe = string.Empty;
            var nhanVienGhiPhieu = string.Empty;
            var giamDocChuyenMon = string.Empty;

            if (thongTinHoSo.NhanVienDoDeId != null)
            {
                nhanVienDoDe = _useRepository.TableNoTracking
                           .Where(u => u.Id == thongTinHoSo.NhanVienDoDeId).Select(u =>
                           (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                         + u.HoTen).FirstOrDefault();
            }
            if (thongTinHoSo.NhanVienGhiPhieuId != null)
            {
                nhanVienGhiPhieu = _useRepository.TableNoTracking
                           .Where(u => u.Id == thongTinHoSo.NhanVienGhiPhieuId).Select(u =>
                           (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                         + u.HoTen).FirstOrDefault();
            }

            if (thongTinHoSo.GiamDocChuyenMonId != null)
            {
                giamDocChuyenMon = _useRepository.TableNoTracking
                           .Where(u => u.Id == thongTinHoSo.GiamDocChuyenMonId).Select(u =>
                           (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                         + u.HoTen).FirstOrDefault();  
            }
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            var tongKetBenhAnKhoaSan = _noiTruBenhAnRepository.TableNoTracking.Where(s => s.Id == noiTruHoSoKhac.YeuCauTiepNhanId).Select(s => s.ThongTinTongKetBenhAn).FirstOrDefault();
            var soBenhAnCon = string.Empty;
            if (tongKetBenhAnKhoaSan != null)
            {
                var thongtinDacDiemTreSoSinhs = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnKhoaSanGrid>(tongKetBenhAnKhoaSan);
                soBenhAnCon = thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs.Any() ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs.Count()), false) : "";
            }
            var soBHYTs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                    .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                    .Select(a => a.MaSoThe.ToString()).ToList();
            var soBHYT = soBHYTs.Any() ? soBHYTs.First() : "";

            var gt = string.Empty;
            switch (thongTinHoSo.GioiTinh)
            {
                case "Trai":
                    gt = "Nam";
                    break;
                case "Gái":
                    gt = "Nữ";
                    break;
                case "Nam":
                    gt = "Nam";
                    break;
                case "Nữ":
                    gt = "Nữ";
                    break;

                default:
                    gt = "Không xác định";
                    break;
            }

            var data = new GiayChungSinhVo
            {
                So = thongTinHoSo.So,
                Quyen = thongTinHoSo.QuyenSo,
                HoTenMe = yeuCauTiepNhan.HoTen,
                NamSinh = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh),
                NoiDangKyThuongTru = yeuCauTiepNhan.DiaChiDayDu,
                BHYTMaSoThe = soBHYT,
                CMND = thongTinHoSo.CMND,
                DanToc = yeuCauTiepNhan.DanToc?.Ten,
                HoTenCha = thongTinHoSo.HoVaTenCha,
                HoTenCon = thongTinHoSo.DuDinhDatTenCon,
                SoConSinh = soBenhAnCon,
                GTCon = gt,
                CanNangCon = thongTinHoSo.CanNang != null ? thongTinHoSo.CanNang + "Gram" : "",
                GhiChu = thongTinHoSo.GhiChu,
                NhanVienDoDe = nhanVienDoDe,
                NhanVienGhiPhieu = nhanVienGhiPhieu,
                GiamDocChuyenMon = giamDocChuyenMon,
                Ngay = thongTinHoSo.NgayCapGiayChungSinh.GetValueOrDefault().Day.ConvertDateToString(),
                Thang = thongTinHoSo.NgayCapGiayChungSinh.GetValueOrDefault().Month.ConvertMonthToString(),
                Nam = thongTinHoSo.NgayCapGiayChungSinh.GetValueOrDefault().Year.ConvertYearToString(),
                GioSDCon = thongTinHoSo.ThoiGianDe?.ConvertDatetimeToString(),
                LogoUrl = hos + "/assets/img/logo-bacha-full.png",
                NoiCap = thongTinHoSo.NoiCap,
                NgayCap = thongTinHoSo.NgayCap.Value.ApplyFormatDate()
                };
            content += TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;

        }

        public async Task<ThongTinNhanVienDangNhap> ThongTinNhanVienDangNhapIdTreSoSinh(long id)
        {
            var nhanVienId = _userAgentHelper.GetCurrentUserId();
            var tenNhanVien = await _nhanVienRepository.TableNoTracking.Where(p => p.Id == nhanVienId).Select(p => p.User.HoTen).FirstAsync();

            var noitruBA = await _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == id).FirstOrDefaultAsync();

            var noiTruHoSoKhac = await _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == id && p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.GiayChungSinh).FirstOrDefaultAsync();
            var dacDiemTreSoSinhs = new List<HoSoKhacTreSoSinhJSON>();
            var soCon = 0;
            if (noiTruHoSoKhac == null)
            {
                if (!string.IsNullOrEmpty(noitruBA.ThongTinTongKetBenhAn))
                {
                    var tongKetBA = JsonConvert.DeserializeObject<TongKetBenhAnJSON>(noitruBA.ThongTinTongKetBenhAn);
                    if (tongKetBA.DacDiemTreSoSinhs != null && tongKetBA.DacDiemTreSoSinhs.Any())
                    {
                        soCon = tongKetBA.DacDiemTreSoSinhs.Count();//z => z.TinhTrangId == 1
                        foreach (var item in tongKetBA.DacDiemTreSoSinhs)//.Where(z => z.TinhTrangId == 1)
                        {
                            var gt = GioiTinhDisplay(item.GioiTinh);
                            var dacDiemTreSoSinh = new HoSoKhacTreSoSinhJSON
                            {
                                DeLuc = item.DeLuc,
                                GioiTinh = gt,
                                CanNang = item.CanNang,
                                HoTenCon = null,
                                GhiChu = null,
                                TinhTrangId = item.TinhTrangId
                            };
                            dacDiemTreSoSinhs.Add(dacDiemTreSoSinh);
                        }
                    }
                }
                return new ThongTinNhanVienDangNhap
                {
                    NhanVienDangNhapId = nhanVienId,
                    NhanVienDangNhap = tenNhanVien,
                    NgayThucHien = DateTime.Now,
                    HoSoKhacTreSoSinhs = dacDiemTreSoSinhs,
                    SoConHienTai = soCon
                };
            }
            else
            {
                if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo))
                {
                    var thongTinHoSo = JsonConvert.DeserializeObject<TongKetBenhAnJSON>(noiTruHoSoKhac.ThongTinHoSo);
                    foreach (var item in thongTinHoSo.HoSoKhacTreSoSinhs)
                    {
                        var dacDiemTreSoSinh = new HoSoKhacTreSoSinhJSON
                        {
                            DeLuc = item.DeLuc,
                            GioiTinh = item.GioiTinh,
                            CanNang = item.CanNang,
                            HoTenCon = item.HoTenCon,
                            GhiChu = item.GhiChu,
                            TinhTrangId = item.TinhTrangId
                        };
                        dacDiemTreSoSinhs.Add(dacDiemTreSoSinh);
                    }
                }
                return new ThongTinNhanVienDangNhap
                {
                    NhanVienDangNhapId = nhanVienId,
                    NhanVienDangNhap = tenNhanVien,
                    NgayThucHien = DateTime.Now,
                    HoSoKhacTreSoSinhs = dacDiemTreSoSinhs,
                    SoConHienTai = soCon
                };
            }
        }
        public bool CheckNgayCapGiayChungSinh(long yeuCauTiepNhan, DateTime? ngayCapChungSinh)
        {
            //Ngày thực hiện mặc định là ngày hiện tại cho phép người dùng sửa, 
            //nhưng không được trước thời gian tiếp nhận và sau thời gian hiện tại
            bool kq = true;
            var thoiGianTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == yeuCauTiepNhan).Select(d => d.ThoiDiemTiepNhan).FirstOrDefault();
          if(thoiGianTiepNhan != null && ngayCapChungSinh != null )
            {
                kq = ngayCapChungSinh < thoiGianTiepNhan || ngayCapChungSinh > DateTime.Now ? false : true;  
            }
          return kq;
        }
        public string InGiayChungSinhTatCa(long noiTruHoSoKhacId, string hos)
        {
            //noiTruHoSoKhacId -> yêu cầu tiếp nhận
            var yeuCauTiepNhanId = noiTruHoSoKhacId;
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayChungSinhNoiTru")).First();
            var noiTruHoSoKhac = _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.GiayChungSinh).Select(d=>d.ThongTinHoSo).ToList();
            var yeuCauTiepNhan = BaseRepository.TableNoTracking
                                                .Include(p => p.NoiTruBenhAn)
                                                .Include(p => p.DanToc)
                                                .Include(p => p.YeuCauTiepNhanTheBHYTs)
                                                .Include(p => p.YeuCauNhapVien).ThenInclude(p => p.YeuCauTiepNhanMe)
                                                .Where(p => p.Id == yeuCauTiepNhanId).First();
            var itemSoBA = 0;
            foreach (var item in noiTruHoSoKhac)
            {
                var thongTinHoSo = JsonConvert.DeserializeObject<GiayChungSinhNewJSONVo>(item);
                var nhanVienDoDe = string.Empty;
                var nhanVienGhiPhieu = string.Empty;
                var giamDocChuyenMon = string.Empty;

                if (thongTinHoSo.NhanVienDoDeId != null)
                {
                    nhanVienDoDe = _useRepository.TableNoTracking
                               .Where(u => u.Id == thongTinHoSo.NhanVienDoDeId).Select(u =>
                               (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                             + u.HoTen).FirstOrDefault();
                }
                if (thongTinHoSo.NhanVienGhiPhieuId != null)
                {
                    nhanVienGhiPhieu = _useRepository.TableNoTracking
                               .Where(u => u.Id == thongTinHoSo.NhanVienGhiPhieuId).Select(u =>
                               (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                             + u.HoTen).FirstOrDefault();
                }

                if (thongTinHoSo.GiamDocChuyenMonId != null)
                {
                    giamDocChuyenMon = _useRepository.TableNoTracking
                               .Where(u => u.Id == thongTinHoSo.GiamDocChuyenMonId).Select(u =>
                               (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                             + u.HoTen).FirstOrDefault();
                }
                long userId = _userAgentHelper.GetCurrentUserId();
                string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
                var tongKetBenhAnKhoaSan = _noiTruBenhAnRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => s.ThongTinTongKetBenhAn).FirstOrDefault();
                var soBenhAnCon = string.Empty;
                if (tongKetBenhAnKhoaSan != null)
                {
                    var thongtinDacDiemTreSoSinhs = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnKhoaSanGrid>(tongKetBenhAnKhoaSan);
                    soBenhAnCon = thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs.Any() ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs.Count()), false) : "";
                }
                var soBHYTs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                    .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                    .Select(a => a.MaSoThe.ToString()).ToList();
                var soBHYT = soBHYTs.Any() ? soBHYTs.First() : "";
                var gt = GioiTinhDisplay(thongTinHoSo.GioiTinh);

                var data = new GiayChungSinhVo
                {
                    So = thongTinHoSo.So,
                    Quyen = thongTinHoSo.QuyenSo,
                    HoTenMe = yeuCauTiepNhan.HoTen,
                    NamSinh = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh),
                    NoiDangKyThuongTru = yeuCauTiepNhan.DiaChiDayDu,
                    BHYTMaSoThe = yeuCauTiepNhan.BHYTMaSoThe,
                    CMND = yeuCauTiepNhan.SoChungMinhThu,
                    DanToc = yeuCauTiepNhan.DanToc?.Ten,
                    HoTenCha = thongTinHoSo.HoVaTenCha,
                    HoTenCon = thongTinHoSo.DuDinhDatTenCon,
                    SoConSinh = soBenhAnCon,
                    GTCon = gt,
                    CanNangCon = thongTinHoSo.CanNang != null ? thongTinHoSo.CanNang + "Gram" : "",
                    GhiChu = thongTinHoSo.GhiChu,
                    NhanVienDoDe = nhanVienDoDe,
                    NhanVienGhiPhieu = nhanVienGhiPhieu,
                    GiamDocChuyenMon = giamDocChuyenMon,
                    Ngay = thongTinHoSo.NgayCapGiayChungSinh.GetValueOrDefault().Day.ConvertDateToString(),
                    Thang = thongTinHoSo.NgayCapGiayChungSinh.GetValueOrDefault().Month.ConvertMonthToString(),
                    Nam = thongTinHoSo.NgayCapGiayChungSinh.GetValueOrDefault().Year.ConvertYearToString(),
                    GioSDCon = thongTinHoSo.ThoiGianDe?.ConvertDatetimeToString(),
                    LogoUrl = hos + "/assets/img/logo-bacha-full.png",
                    NoiCap = thongTinHoSo.NoiCap,
                    NgayCap = thongTinHoSo.NgayCap?.ApplyFormatDate()
                };
                
                content += TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                if(itemSoBA < noiTruHoSoKhac.Count())
                {
                    content += "<div class=\"pagebreak\"> </div>";
                }
                itemSoBA++;
            }
            return content;

        }

        public bool kiemTrungSoChungSinh(string so,long yctn, long? noiTruHoSoKhacGiayChungSinhId)
        {
            bool kq = true;
            if(noiTruHoSoKhacGiayChungSinhId == null)
            {
                var dsGiayChungSinhs = _noiTruHoSoKhacRepository.TableNoTracking
                .Where(d => d.YeuCauTiepNhanId == yctn && d.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh).Select(d => d.ThongTinHoSo).ToList();
                var listSoChungSinhs = new List<string>();
                if (dsGiayChungSinhs.Any())
                {
                    foreach (var item in dsGiayChungSinhs)
                    {
                        var thongTinHoSo = JsonConvert.DeserializeObject<GiayChungSinhNewJSONVo>(item);
                        if (!string.IsNullOrEmpty(thongTinHoSo.So))
                        {
                            listSoChungSinhs.Add(thongTinHoSo.So);
                        }

                    }
                }
                return listSoChungSinhs.Any(d => d == so) ? false : true;
            }
            return kq;
        }
        #region list info BA con
        public async Task<List<InfoBAConGridVo>> GetDataInfoBACon(DropDownListRequestModel model, long yeuCauTiepNhanMeId)
        {
            var tongKetBenhAnKhoaSan = _noiTruBenhAnRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanMeId).Select(s => s.ThongTinTongKetBenhAn).FirstOrDefault();
            List<InfoDacDiemTreSoSinhGridVo> listBACons = new List<InfoDacDiemTreSoSinhGridVo>();
            if (tongKetBenhAnKhoaSan != null)
            {
                var thongtinDacDiemTreSoSinhs = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnKhoaSanGrid>(tongKetBenhAnKhoaSan);
                foreach (var item in thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs)
                {
                    var gt = GioiTinhDisplay(item.GioiTinh);
                  
                    InfoDacDiemTreSoSinhGridVo itemInfo = new InfoDacDiemTreSoSinhGridVo();
                    itemInfo.CanNang = item.CanNang;
                    itemInfo.GioiTinh = gt;
                    itemInfo.TinhTrang = item.TinhTrang;
                    itemInfo.Id = item.Id;
                    itemInfo.DeLuc = item.DeLuc;
                    itemInfo.DeLucDisplayName = item.DeLucDisplayName;
                    itemInfo.YeuCauTiepNhanConId = item.YeuCauTiepNhanConId;
                    listBACons.Add(itemInfo);
                }
            }
            listBACons = listBACons.Where(d => d.YeuCauTiepNhanConId != null).ToList();

            // get all tất cả BA con có tiếp nhận me id
                var listTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.YeuCauNhapVien != null && d.YeuCauNhapVien.YeuCauTiepNhanMeId != null)
                .Select(d => new { Id = d.Id,HoTen = d.HoTen }).ToList();
            
            if(listBACons.Any())
            {
                foreach (var item in listBACons)
                {
                    var kiemTra = listTiepNhans.Where(d => d.Id == (long)item.YeuCauTiepNhanConId).ToList();
                    if (kiemTra.Any())
                    {
                        item.DuDinhDatTenCon = kiemTra.First().HoTen;
                    }
                }
            }
            var query = listBACons.Select(item => new InfoBAConGridVo()
            {
                DisplayName = item.DuDinhDatTenCon,
                KeyId = (long)item.YeuCauTiepNhanConId,
                Ten = item.DuDinhDatTenCon,
                CanNang = item.CanNang,
                GioiTinh = item.GioiTinh,
                DuDinhDatTenCon = item.DuDinhDatTenCon,
                ThoiGianDe = item.DeLuc
            }).ToList();
            return query;
        }
        public string GetNameBacSi(long id)
        {
            var name = _useRepository.TableNoTracking.Where(d => d.Id == id).Select(d => d.HoTen).FirstOrDefault();
            return name;
        }
        private string GioiTinhDisplay(string gioiTinh)
        {
            var gt = string.Empty;
            switch (gioiTinh)
            {
                case "Trai":
                    gt = "Nam";
                    break;
                case "Gái":
                    gt = "Nữ";
                    break;
                case "Nam":
                    gt = "Nam";
                    break;
                case "Nữ":
                    gt = "Nữ";
                    break;

                default:
                    gt = "Không xác định";
                    break;
            }
            return gt;
        }
        #endregion list info BA con
    }
}
