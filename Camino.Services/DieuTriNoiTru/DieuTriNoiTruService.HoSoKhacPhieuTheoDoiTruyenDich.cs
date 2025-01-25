using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenDich;
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
        public PhieuTheoDoiTruyenDichGrid GetDataDanhSachTruyenDichForGridAsync(QueryInfo queryInfo)
        {
            // create object PhieuTheoDoiTruyenDichGrid
            PhieuTheoDoiTruyenDichGrid phieuTheoDoiTruyenDichGrid = new PhieuTheoDoiTruyenDichGrid();
            // create list PhieuTheoDoiTruyenDichGridVo 
            List<PhieuTheoDoiTruyenDichGridVo> listPhieuTheoDoiTruyenDich = new List<PhieuTheoDoiTruyenDichGridVo>();
            long.TryParse(queryInfo.AdditionalSearchString, out long noiTruBenhAnId);
            // dựa vào yêu cầu tiếp nhận  lấy tất cả phiếu điều trị được chỉ định trong màn hình phiếu điều trị 
            var listPhieuDieuTriId = _noiTruBenhAnRepository.TableNoTracking.Where(x => x.Id == noiTruBenhAnId)
                                                                            .SelectMany(s =>s.NoiTruPhieuDieuTris).Select(k=>k.Id).ToList();
            //yêu cầu dược phẩm bệnh viện  : Nơi trữ phiếu điều trị Id , là dịch truyền = true , 
            //số lượng => nếu là dịch tuyền ( có thể tích) số lượng  = số lượng  * thể tích  ngược lại số lượng = số lượng 
            foreach (var itemPhieuDieuTriId in listPhieuDieuTriId)
            {

                var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();


                var queryItemTruyenDichChiDinh = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                                                                           .Where(p => p.NoiTruPhieuDieuTriId == itemPhieuDieuTriId &&
                                                                                                      p.LaDichTruyen == true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                                                                                           .Select(s => new PhieuTheoDoiTruyenDichGridVo()
                                                                                           {
                                                                                               Id = s.Id,
                                                                                               IdTruyenDich = s.DuocPhamBenhVien.Id,
                                                                                               TenTruyenDich = s.Ten,
                                                                                               SoLuong = s.TheTich != null ? (double)(s.TheTich * s.SoLuong) : s.SoLuong,
                                                                                               LoSoSX = s.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                                                               && nkct.LaDuocPhamBHYT == s.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now)
                                                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                                                                .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                                                .Select(o => o.Solo).FirstOrDefault(),
                                                                                               TocDo = s.NoiTruChiDinhDuocPham.TocDoTruyen,
                                                                                               BatDau = s.NoiTruChiDinhDuocPham.ThoiGianBatDauTruyen, // trong table NoiTruChiDinhDuocPham
                                                                                               BSChiDinh = s.NhanVienChiDinh.User.HoTen,
                                                                                               NgayThu = s.NoiTruPhieuDieuTri.NgayDieuTri,
                                                                                               
                                                                                           }).ToList();

                listPhieuTheoDoiTruyenDich.AddRange(queryItemTruyenDichChiDinh);
            }
            // tính tổng sl dược phẩm ban đầu 

            phieuTheoDoiTruyenDichGrid.ListPhieuTheoDoiTruyenDichDefault = listPhieuTheoDoiTruyenDich.GroupBy(s => s.TenTruyenDich)
                                                                                                    .Select(s => new DuocPhamPhieuDieuTriTheoNgayDefault() {
                                                                                                        TenDuocPhamTruyenDich = s.FirstOrDefault().TenTruyenDich,
                                                                                                        TotalSlTheoDuocPham = (int)s.Sum(k=>k.SoLuong),
                                                                                                        NgayThu = s.FirstOrDefault().NgayThu
                                                                                                   }).ToList();
            phieuTheoDoiTruyenDichGrid.ListPhieuTheoDoiTruyenDich = listPhieuTheoDoiTruyenDich;
            // ngày hiện tại 
            var ngayHienTai = new DateTime();
            ngayHienTai = DateTime.Now;
            phieuTheoDoiTruyenDichGrid.NgayThucHien = ngayHienTai.ApplyFormatDateTimeSACH();
            // nhân viên login
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            phieuTheoDoiTruyenDichGrid.TenNhanVien = nguoiLogin;
            // chẩn đoán hiện tại . lấy chản đoán cuối cùng
            phieuTheoDoiTruyenDichGrid.ChanDoan = _noiTruBenhAnRepository.TableNoTracking.Where(x => x.Id == noiTruBenhAnId).SelectMany(s => s.NoiTruPhieuDieuTris).Select(l => l.ChanDoanChinhGhiChu).LastOrDefault();
            return phieuTheoDoiTruyenDichGrid;
        }
        public List<LookupItemVo> GetDataDanhSachTruyenDichTheoNgayForGridAsync(DropDownListRequestModel queryInfo)
        {
            var plitString = queryInfo.ParameterDependencies;
            var queryString = JsonConvert.DeserializeObject<ObjDanhSachThuocTheoNgay>(queryInfo.ParameterDependencies);
            if (queryString.YeuCauTiepNhanId != null  && queryString.ListDanhSach != null)
            {
                List<DuocPhamPhieuDieuTriTheoNgay> list = new List<DuocPhamPhieuDieuTriTheoNgay>();
                List<string> lst = new List<string>();

                var listPhieuDieuTriId = _noiTruBenhAnRepository.TableNoTracking.Where(x => x.Id == queryString.YeuCauTiepNhanId)
                                                                              .SelectMany(s => s.NoiTruPhieuDieuTris).Select(k => k.Id).ToList();

                foreach (var item in listPhieuDieuTriId)
                {
                    var lstDsDuocPhamQuocGia =
                 _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(s => s.NoiTruPhieuDieuTriId == item && s.LaDichTruyen == true && s.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .Select(x => new DuocPhamPhieuDieuTriTheoNgay()
                    {
                        IdTruyenDich = x.DuocPhamBenhVien.Id,
                        TenDuocPhamTruyenDich = x.Ten,
                        NgayPhieuDieuTri = x.NoiTruPhieuDieuTri.NgayDieuTri,
                        Sl = x.TheTich != null ? (double)(x.TheTich * x.SoLuong) : x.SoLuong,
                        BatDau = x.NoiTruChiDinhDuocPham.ThoiGianBatDauTruyen
                    }).ToList();
                    list.AddRange(lstDsDuocPhamQuocGia);
                }
                // dk lấy theo ngày , goup total duoc pham trung sl 
                var ngayPhieuDieuTri = queryString.ListDanhSach.First().NgayThu.GetValueOrDefault().AddDays(1);
                var querylist = list.Where(s => s.NgayPhieuDieuTri.Day == ngayPhieuDieuTri.Day && s.NgayPhieuDieuTri.Month == ngayPhieuDieuTri.Month && s.NgayPhieuDieuTri.Year == ngayPhieuDieuTri.Year )
                    .Select(x => new DuocPhamPhieuDieuTriTheoNgay() 
                    { 
                        TenDuocPhamTruyenDich = x.TenDuocPhamTruyenDich,
                        IdTruyenDich = x.IdTruyenDich,
                        Sl = x.Sl,
                        BatDau = x.BatDau
                    }).GroupBy(s=> new { s.TenDuocPhamTruyenDich, s.IdTruyenDich })
                    .Select(s=> new DuocPhamPhieuDieuTriTheoNgay() {
                        TenDuocPhamTruyenDich = s.First().TenDuocPhamTruyenDich,
                        IdTruyenDich = s.First().IdTruyenDich,
                        Sl = s.Sum(c=>c.Sl),
                        BatDau = s.First().BatDau
                    }).ToList();
                //// dk những dươc phẩm có thể thêm 
                if(queryString.ListDanhSach.FirstOrDefault().IdTruyenDich != null && queryString.ListDanhSach.FirstOrDefault().SL != null && queryString.ListDanhSach.FirstOrDefault().TenDuocPham != null)
                {
                    List<DuocPhamPhieuDieuTriTheoNgay> lists = new List<DuocPhamPhieuDieuTriTheoNgay>();
                    var groupDichTruyenTrung = queryString.ListDanhSach.Select(c => new TenThuoc
                    {
                        IdTruyenDich = c.IdTruyenDich,
                        SL = c.SL,
                        NgayThu = c.NgayThu,
                        TenDuocPham = c.TenDuocPham
                    }).GroupBy(s => new { s.IdTruyenDich, s.TenDuocPham })
                    .Select(item => new TenThuoc
                    {
                        IdTruyenDich = item.First().IdTruyenDich,
                        SL = item.Sum(v=>v.SL),
                        NgayThu = item.First().NgayThu,
                        TenDuocPham = item.First().TenDuocPham
                    });
                    foreach (var item in groupDichTruyenTrung)
                    {
                        // kiem tra soluong change có nhỏ  hơn so luong ban đầu =>  lấy những số lượng còn lại lên combobox
                        if (item.IdTruyenDich != null)
                        {
                            var itemDichTruyen = querylist.Where(x => x.TenDuocPhamTruyenDich == item.TenDuocPham && x.IdTruyenDich == item.IdTruyenDich && x.Sl == item.SL).FirstOrDefault();
                            if(itemDichTruyen != null) // nếu  dịch truyền cùng số lượng kê
                            {
                                querylist.Remove(itemDichTruyen);
                            }
                            lists = querylist;
                        }
                       
                    }
                    return lists.GroupBy(s => s.IdTruyenDich).Select(s => new LookupItemVo
                    {
                        KeyId = s.FirstOrDefault().IdTruyenDich,
                        DisplayName = s.First().TenDuocPhamTruyenDich
                    }).ToList(); ;
                }
                return querylist.GroupBy(s => s.IdTruyenDich).Select(s => new LookupItemVo
                {
                    KeyId = s.FirstOrDefault().IdTruyenDich,
                    DisplayName = s.First().TenDuocPhamTruyenDich
                }).ToList(); ;
            }
            else
            {
                List<DuocPhamPhieuDieuTriTheoNgay> list = new List<DuocPhamPhieuDieuTriTheoNgay>();
                var listPhieuDieuTriId = _noiTruBenhAnRepository.TableNoTracking.Where(x => x.Id == queryString.YeuCauTiepNhanId)
                                                                              .SelectMany(s => s.NoiTruPhieuDieuTris).Select(k => k.Id).ToList();

                foreach (var item in listPhieuDieuTriId)
                {
                    var lstDsDuocPhamQuocGia =
                 _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(s => s.NoiTruPhieuDieuTriId == item && s.LaDichTruyen == true)
                    .Select(x => new DuocPhamPhieuDieuTriTheoNgay()
                    {
                        IdTruyenDich = x.DuocPhamBenhVien.Id,
                        TenDuocPhamTruyenDich = x.Ten,
                        NgayPhieuDieuTri = x.NoiTruPhieuDieuTri.NgayDieuTri
                    }).ToList();
                    list.AddRange(lstDsDuocPhamQuocGia);
                }
                if (queryInfo.Id != null || queryInfo.Id != 0)
                {
                    list = list.Where(s => s.IdTruyenDich == queryInfo.Id).ToList();
                }
                return list.GroupBy(s => s.TenDuocPhamTruyenDich).Select(s => new LookupItemVo
                {
                    KeyId = s.FirstOrDefault().IdTruyenDich,
                    DisplayName = s.First().TenDuocPhamTruyenDich
                }).ToList();
                return null;
            }
           
        }
        public DuocPhamPhieuDieuTriTheoNgay GetDataBindTruyenDichTheoNgayForGridAsync(QueryInfo queryInfo)
        {
            var plitString = queryInfo.AdditionalSearchString.Split('|');
            List<DuocPhamPhieuDieuTriTheoNgay> list = new List<DuocPhamPhieuDieuTriTheoNgay>();
            long.TryParse(plitString[0], out long yeuCauTiepNhanId);
            var ngaySelect = plitString[1].Split('/');
            int.TryParse(ngaySelect[0], out int thang);
            int.TryParse(ngaySelect[1], out int ngay);
            int.TryParse(ngaySelect[2], out int nam);
            bool.TryParse(plitString[3], out bool kieuBind);
            List<string> lst = new List<string>();
            
            long.TryParse(plitString[2], out long duocPhamId);
            // lấy tất cả phiếu điều trị
            var listPhieuDieuTriId = _noiTruBenhAnRepository.TableNoTracking.Where(x => x.Id == yeuCauTiepNhanId)
                                                                          .SelectMany(s => s.NoiTruPhieuDieuTris).Select(k => k.Id).ToList();

            // lấy tất cả thông tin kê dịch truyền từ phiếu điều trị
            foreach (var item in listPhieuDieuTriId)
            {
                var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
                var lstDsDuocPhamQuocGia =
             _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Include(g=>g.NoiTruPhieuDieuTri).Where(s => s.NoiTruPhieuDieuTriId == item && s.NoiTruChiDinhDuocPhamId != null && s.LaDichTruyen == true   && s.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                .Select(x => new DuocPhamPhieuDieuTriTheoNgay()
                {
                    IdTruyenDich = x.DuocPhamBenhVien.Id,
                    TenDuocPhamTruyenDich = x.Ten,
                    NgayPhieuDieuTri = x.NoiTruPhieuDieuTri.NgayDieuTri,
                    Sl = x.TheTich != null ? (double)(x.TheTich * x.SoLuong) : x.SoLuong,
                    LoSoSX = x.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.DuocPhamBenhVienId == x.DuocPhamBenhVienId
                    && nkct.LaDuocPhamBHYT == x.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now)
                                                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien)
                                                                                                .ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                                                .Select(o => o.Solo).FirstOrDefault(),
                    TocDo = x.NoiTruChiDinhDuocPham.TocDoTruyen,
                    BatDau = x.NoiTruChiDinhDuocPham.ThoiGianBatDauTruyen, // trong table NoiTruChiDinhDuocPham
                    BSChiDinh = x.NhanVienChiDinh.User.HoTen,
                    NoiTruChiDinhDuocPhamId = (long)x.NoiTruChiDinhDuocPhamId,
                    NoiTruChiTietYLenhThucHien = (x.NoiTruChiDinhDuocPham.NoiTruPhieuDieuTriChiTietYLenhs.Any(s=>s.XacNhanThucHien == true)  || x.YeuCauLinhDuocPhamId != null) ?  true : false,
                    NoiTruPhieuDieuTriId = item
                }).ToList();
                list.AddRange(lstDsDuocPhamQuocGia);
            }
            // lấy ngay bat dau  ngay ket thuc cua dich truyen
            List<DuocPhamPhieuDieuTriTheoNgay> listDichTruyen = new List<DuocPhamPhieuDieuTriTheoNgay>();
            foreach (var item in list)
            {
                item.TocDo = _noiTruChiDinhDuocPhamRepository.TableNoTracking.Where(s => s.Id == item.NoiTruChiDinhDuocPhamId).FirstOrDefault().TocDoTruyen ;
                item.BatDau = _noiTruChiDinhDuocPhamRepository.TableNoTracking.Where(s => s.Id == item.NoiTruChiDinhDuocPhamId).FirstOrDefault().ThoiGianBatDauTruyen;
                listDichTruyen.Add(item);
            }
            // dk lấy theo ngày
            var querylist = listDichTruyen.Where(s => s.NgayPhieuDieuTri.Day == ngay && s.NgayPhieuDieuTri.Month == thang && s.NgayPhieuDieuTri.Year == nam).Select(x => new DuocPhamPhieuDieuTriTheoNgay() {
                TenDuocPhamTruyenDich = x.TenDuocPhamTruyenDich,
                IdTruyenDich = x.IdTruyenDich,
                Sl = x.Sl,
                LoSoSX = x.LoSoSX,
                TocDo = x.TocDo,
                BatDau = x.BatDau,
                KetThuc = x.KetThuc,
                BSChiDinh = x.BSChiDinh,
                YTaThucHien = x.YTaThucHien,
                NgayPhieuDieuTri =x.NgayPhieuDieuTri,
                NoiTruChiTietYLenhThucHien = x.NoiTruChiTietYLenhThucHien,
                NoiTruPhieuDieuTriId = x.NoiTruPhieuDieuTriId,
            }).GroupBy(s=> new { s.TenDuocPhamTruyenDich,s.IdTruyenDich})
            .Select(x => new DuocPhamPhieuDieuTriTheoNgay()
            {
                TenDuocPhamTruyenDich = x.First().TenDuocPhamTruyenDich,
                IdTruyenDich = x.First().IdTruyenDich,
                Sl = x.Sum(v=>v.Sl),
                LoSoSX = x.First().LoSoSX,
                TocDo = x.First().TocDo,
                BatDau = x.First().BatDau,
                KetThuc = x.First().KetThuc,
                BSChiDinh = x.First().BSChiDinh,
                YTaThucHien = x.First().YTaThucHien,
                NgayPhieuDieuTri = x.First().NgayPhieuDieuTri,
                NoiTruChiTietYLenhThucHien = x.First().NoiTruChiTietYLenhThucHien,
                NoiTruPhieuDieuTriId = x.First().NoiTruPhieuDieuTriId,
            }).ToList();
            // lấy thông tin dich truyền theo duoc pham theo ngày bắt đầu
            var queryItemDichTruyen = querylist.Where(s => s.IdTruyenDich == duocPhamId).Select(x => new DuocPhamPhieuDieuTriTheoNgay()
            {
                Sl = x.Sl,
                TenDuocPhamTruyenDich = x.TenDuocPhamTruyenDich,
                LoSoSX = x.LoSoSX,
                TocDo = x.TocDo,
                BatDau = x.BatDau,
                KetThuc = x.KetThuc,
                BSChiDinh = x.BSChiDinh,
                YTaThucHien = x.YTaThucHien,
                NgayPhieuDieuTri = x.NgayPhieuDieuTri,
                IdTruyenDich = x.IdTruyenDich,
                NoiTruChiTietYLenhThucHien = x.NoiTruChiTietYLenhThucHien,
                NoiTruPhieuDieuTriId = x.NoiTruPhieuDieuTriId,
            }).OrderBy(s => s.BatDau).ToList();

            // trường hợp kiểm tra dich truyền còn không => lấy thông tin dich truyền
            if (kieuBind == true)
            {
                var queryStringListDichTruyen = JsonConvert.DeserializeObject<List<TenThuoc>>(plitString[4]);
                var danhSachDichTruyen = queryStringListDichTruyen.Where(s =>s.IdTruyenDich != null && s.SL != null && s.TenDuocPham != null)
                    .GroupBy(z => new { z.IdTruyenDich, z.NgayThu }).Select(zz => new TenThuoc()
                    {
                        IdTruyenDich = zz.First().IdTruyenDich,
                        NgayThu = zz.First().NgayThu,
                        SL = zz.Sum(g=>g.SL),
                        TenDuocPham = zz.First().TenDuocPham
                    }).ToList();
                List<DuocPhamPhieuDieuTriTheoNgay> listDichTruyenConLai = new List<DuocPhamPhieuDieuTriTheoNgay>();
                foreach (var item in queryItemDichTruyen)
                {
                    if(danhSachDichTruyen.Any())
                    {
                        foreach (var itemx in danhSachDichTruyen)
                        {
                            var itemNgay = itemx.NgayThu.GetValueOrDefault().AddDays(1);
                            DateTime ngayPhieuDieuTriUI = new DateTime(itemNgay.Year, itemNgay.Month, itemNgay.Day);
                            // ngày điều trị default
                            var itemNgayDefault = item.NgayPhieuDieuTri;
                            DateTime ngayPhieuDieuTriDefault = new DateTime(itemNgay.Year, itemNgay.Month, itemNgay.Day);
                            if (item.IdTruyenDich == itemx.IdTruyenDich && itemNgayDefault == ngayPhieuDieuTriUI)
                            {
                                item.Sl = item.Sl - (double)itemx.SL;
                                listDichTruyenConLai.Add(item);
                            }
                            else
                            {
                                listDichTruyenConLai.Add(item);
                            }
                        }
                    }
                    else
                    {
                        listDichTruyenConLai.Add(item);
                    }
                    
                        
                }
                return listDichTruyenConLai.FirstOrDefault();
            }
            
          

            return queryItemDichTruyen.FirstOrDefault();
        }
        public PhieuTheoDoiTruyenDichGridInfo GetThongTinPhieuTheoDoiTruyenDich(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenDich)
                                                                  .Select(s => new PhieuTheoDoiTruyenDichGridInfo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenDich,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuTheoDoiTruyenDichGridVo()
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
            if(query != null)
            {
                InPhieuTheoDoiTruyenDichVo vo = new InPhieuTheoDoiTruyenDichVo();
                // list sau khi chỉnh sửa
                List<PhieuTheoDoiTruyenComPare> listComPare = new List<PhieuTheoDoiTruyenComPare>();

               
                List<DachSachTruyenDich> listComPareJson = new List<DachSachTruyenDich>();
                List<DachSachTruyenDich> listKiemTraChange = new List<DachSachTruyenDich>();
                if (query.ThongTinHoSo != null)
                {
                    var queryString = JsonConvert.DeserializeObject<InPhieuTheoDoiTruyenDichVo>(query.ThongTinHoSo);
                    vo.ChanDoan = queryString.ChanDoan;
                    vo.DachSachTruyenDichArrDefault = queryString.DachSachTruyenDichArrDefault;
                    vo.NgayThucHien = queryString.NgayThucHien;
                    vo.TaiKhoanDangNhap = queryString.TaiKhoanDangNhap;

                    listComPareJson = queryString.DachSachTruyenDichArr;
                }
                foreach(var item in listComPareJson)
                {
                    
                    DateTime.TryParseExact(item.NgayThu, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgayTemp);
                    var tuNgay = new DateTime(tuNgayTemp.Year, tuNgayTemp.Month, tuNgayTemp.Day, 0, 0, 0);
                    var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
                    var queryItemTruyenDichChiDinh = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                                                                          .Where(p => p.DuocPhamBenhVienId == (long)item.IdTruyenDich && 
                                                                                                      p.NoiTruPhieuDieuTriId == item.NoiTruPhieuDieuTriId &&
                                                                                                      p.NoiTruPhieuDieuTri.NgayDieuTri == item.NgayPhieuDieuTri &&
                                                                                                      p.LaDichTruyen == true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)

                                                                                          .Select(s =>  new 
                                                                                          { 
                                                                                              IdTruyenDich = item.IdTruyenDich,
                                                                                              NoiTruChiTietYLenhThucHien =(s.NoiTruChiDinhDuocPham.NoiTruPhieuDieuTriChiTietYLenhs.Any(x=>x.XacNhanThucHien == true)  || s.YeuCauLinhDuocPhamId != null) ?  true : false,
                                                                                          }).GroupBy(s=>s.IdTruyenDich).Select(s=>s.First().NoiTruChiTietYLenhThucHien);
                    if(queryItemTruyenDichChiDinh.Any())
                    {
                        item.NoiTruChiTietYLenhThucHien = queryItemTruyenDichChiDinh.First();
                    }
                    listKiemTraChange.Add(item);
                }
                vo.DachSachTruyenDichArr = listKiemTraChange;
                var jsonString = JsonConvert.SerializeObject(vo);
                query.ThongTinHoSo = jsonString;
            }

            
            return query;
        }
        public async Task<string> InPhieuTheoDoiTruyenDich(XacNhanInPhieuTheoDoiTruyenDich xacNhanIn)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanIn.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<InPhieuTheoDoiTruyenDich>(thongtinIn);
            var content = "";
            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU TRÍCH BIÊN BẢN HỘI CHẨN</th></tr></table>";
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuTruyenDichHoSoKhac"));
            bool inLanDau = true;
            var phieuTheoDoiTruyenDichVaTruyenMau = "";
            if(queryString.DachSachTruyenDichArr != null)
            {
                foreach(var item in queryString.DachSachTruyenDichArr)
                {
                    var batDau = "";
                    var ketThuc = "";
                    if (item.BatDau != null)
                    {
                        batDau = item.BatDau.GetValueOrDefault().ConvertIntSecondsToTime12h();
                    }
                    else
                    {
                        batDau = "&nbsp;";
                    }
                    if (item.KetThuc != null)
                    {
                        ketThuc = item.KetThuc.GetValueOrDefault().ConvertIntSecondsToTime12h();
                    }
                    else
                    {
                        ketThuc = "&nbsp;";
                    }
                    DateTime ngayThu;
                    DateTime.TryParseExact(item.Ngay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayThu);
                    phieuTheoDoiTruyenDichVaTruyenMau += "<tr>" +
                                                           "<td style=' border: 1px solid black;width: 7%;padding: 0px;margin: 0px;'>" + ngayThu.ApplyFormatDate() + "</td>"
                                                           + " <td style=' border: 1px solid black;width: 22%;'>" + GetFormatDuocPham(item.TenTruyenDich) + "</td>"
                                                           + "<td style='border: 1px solid black;width: 7%;text-align:center;'>" + GetFormatSoLuong(item.TenTruyenDich,item.SoLuong) + "</td>"
                                                           + "<td  style='border: 1px solid black;width: 8%;text-align:center;'>" + item.LoSoSX + "</td>"
                                                           + "<td  style='border: 1px solid black;width: 8%;text-align:center;'>" + item.TocDo + "</td>"
                                                           + "<td style='border: 1px solid black;width: 7%;'>" + batDau + "</td>"
                                                           + "<td style='border: 1px solid black;width: 7%;'>" + ketThuc + "</td>"
                                                           + "<td   style='border: 1px solid black;width: 12%;'>" + item.BSChiDinh + "</td>"
                                                           + " <td   style='border: 1px solid black;width: 12%;'>" + item.YTaThucHien + "</td>"
                                                      + "</tr>";
                }
                
            }
            if (!queryString.DachSachTruyenDichArr.Any())
            {
                // default sl hàng 10
                for (int i = 0; i < 10; i++)
                {
                    phieuTheoDoiTruyenDichVaTruyenMau += "<tr>" +
                                                      "<td style=' border: 1px solid black;width: 7%;padding: 0px;margin: 0px;height:25px'>" + "&nbsp;" + "</td>"
                                                      + " <td style=' border: 1px solid black;width: 22%;'>" + "&nbsp;" + "</td>"
                                                      + "<td style='border: 1px solid black;width: 7%;text-align:center;'>" + "&nbsp;" + "</td>"
                                                      + "<td  style='border: 1px solid black;width: 8%;text-align:center;'>" + "&nbsp;" + "</td>"
                                                      + "<td  style='border: 1px solid black;width: 8%;text-align:center;'>" + "&nbsp;" + "</td>"
                                                      + "<td style='border: 1px solid black;width: 7%;'>" + "&nbsp;" + "</td>"
                                                      + "<td style='border: 1px solid black;width: 7%;'>" + "&nbsp;" + "</td>"
                                                      + "<td   style='border: 1px solid black;width: 12%;'>" + "&nbsp;" + "</td>"
                                                      + " <td   style='border: 1px solid black;width: 12%;'>" + "&nbsp;" + "</td>"
                                                 + "</tr>";
                }
            }
            var data = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanIn.LoaiHoSoDieuTriNoiTru)
                                                                              .Select(x => new
                                                                              {
                                                                                  HoTen = x.YeuCauTiepNhan.HoTen,
                                                                                  Tuoi = DateTime.Now.Year - x.YeuCauTiepNhan.NamSinh,
                                                                                  GioiTinh = x.YeuCauTiepNhan.GioiTinh != null ?x.YeuCauTiepNhan.GioiTinh.GetDescription():"",
                                                                                  SoGiuong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan) ? x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.OrderByDescending(p => p.Id).FirstOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.Ten : "",
                                                                                  Buong = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(k => k.Id).Select(p => p.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                                                                                  ChanDoan = queryString.ChanDoan,
                                                                                  PhieuTheoDoiTruyenDichVaTruyenMau = phieuTheoDoiTruyenDichVaTruyenMau,
                                                                                  Khoa = x.YeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(s => s.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(s => s.Id).Select(s => s.GiuongBenh.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault()
                                                                              }).FirstOrDefault();

            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        public async Task<bool> ValidateSoLuongChangeDichTruyen(long yeuCauTiepNhanId, double soLuong, string ngayThus, long duocPhamId, double batDau)
        {
            List<DuocPhamPhieuDieuTriTheoNgay> list = new List<DuocPhamPhieuDieuTriTheoNgay>();
            var listPhieuDieuTriId = _noiTruBenhAnRepository.TableNoTracking.Where(x => x.Id == yeuCauTiepNhanId)
                                                                          .SelectMany(s => s.NoiTruPhieuDieuTris).Select(k => k.Id).ToList();

            foreach (var item in listPhieuDieuTriId)
            {
                var lstDsDuocPham =
             _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(s => s.NoiTruPhieuDieuTriId == item && s.LaDichTruyen == true)
                .Select(x => new DuocPhamPhieuDieuTriTheoNgay()
                {
                    IdTruyenDich = x.DuocPhamBenhVien.Id,
                    TenDuocPhamTruyenDich = x.Ten,
                    NgayPhieuDieuTri = x.NoiTruPhieuDieuTri.NgayDieuTri,
                    Sl = x.TheTich != null ? (double)(x.TheTich * x.SoLuong) : x.SoLuong,
                    BatDau  = x.NoiTruChiDinhDuocPham.ThoiGianBatDauTruyen
                }).ToList();
                list.AddRange(lstDsDuocPham);
            }
            DateTime ngayThu = DateTime.Now;
            DateTime.TryParseExact(ngayThus, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayThu);
            var kiemTraSLThuoVuotBanDau = list.Where(s => s.NgayPhieuDieuTri == ngayThu && s.Sl < soLuong && s.IdTruyenDich == duocPhamId && (s.BatDau != null && s.BatDau == batDau));
            bool validateErro = true;
            if (kiemTraSLThuoVuotBanDau.Any())
            {
                validateErro = false;
            }
            return validateErro;
        }
        public async Task<bool> ValidatorTotalSlKhongVuotTongBanDau(ValidatetorTruyenDichVo vo)
        {
            bool validateErro = true;
            if (vo.listTruyenDich.Any())
            {
                long yeuCauTiepNhanId = vo.listTruyenDich.First().YeuCauTiepNhanId;
                // Tong total duocPhamChange
                var listTotalChange = vo.listTruyenDich.GroupBy(s => new { s.DuocPhamId, s.NgayThu }).Select(s => new ValidateSoLuongTruyenDichVo()
                {
                    DuocPhamId = s.First().DuocPhamId,
                    NgayThu = s.First().NgayThu,
                    SoLuong = s.Sum(v => v.SoLuong),
                    YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                    BatDau = s.First().BatDau
                }).ToList();

                List<DuocPhamPhieuDieuTriTheoNgay> list = new List<DuocPhamPhieuDieuTriTheoNgay>();
                var listPhieuDieuTriId = _noiTruBenhAnRepository.TableNoTracking.Where(x => x.Id == yeuCauTiepNhanId)
                                                                              .SelectMany(s => s.NoiTruPhieuDieuTris).Select(k => k.Id).ToList();

                foreach (var item in listPhieuDieuTriId)
                {
                    var lstDsDuocPham =
                 _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(s => s.NoiTruPhieuDieuTriId == item && s.LaDichTruyen == true && s.NoiTruChiDinhDuocPhamId != null && s.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .Select(x => new DuocPhamPhieuDieuTriTheoNgay()
                    {
                        IdTruyenDich = x.DuocPhamBenhVien.Id,
                        TenDuocPhamTruyenDich = x.Ten,
                        NgayPhieuDieuTri = x.NoiTruPhieuDieuTri.NgayDieuTri,
                        Sl = x.TheTich != null ? (double)(x.TheTich * x.SoLuong) : x.SoLuong,
                        BatDau = x.NoiTruChiDinhDuocPham.ThoiGianBatDauTruyen
                    }).ToList();
                    list.AddRange(lstDsDuocPham);
                }
                // list defaul
                var listDeFault = list.GroupBy(s => new { s.IdTruyenDich, s.NgayPhieuDieuTri })
                    .Select(s => new DuocPhamPhieuDieuTriTheoNgay()
                    {
                        IdTruyenDich = s.First().IdTruyenDich,
                        NgayPhieuDieuTri = s.First().NgayPhieuDieuTri,
                        Sl = s.Sum(v => v.Sl),
                        BatDau = s.First().BatDau
                    }).ToList();
               
                foreach (var item in listTotalChange)
                {
                    DateTime ngayThu = DateTime.Now;
                    DateTime.TryParseExact(item.NgayThu, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayThu);
                    var kiemTraSLThuoVuotBanDau = listDeFault.Where(s => s.NgayPhieuDieuTri == ngayThu && s.IdTruyenDich == item.DuocPhamId);
                    if (kiemTraSLThuoVuotBanDau.Any())
                    {
                        // So luong la Ui , Sl la default
                        if (item.SoLuong < kiemTraSLThuoVuotBanDau.First().Sl)
                        {
                            validateErro = true;
                        }
                        if (item.SoLuong > kiemTraSLThuoVuotBanDau.First().Sl)
                        {
                            validateErro = false;
                            return validateErro;
                        }
                        if (item.SoLuong == kiemTraSLThuoVuotBanDau.First().Sl)
                        {
                            validateErro = true;
                        }
                    }
                }
            }
              
            return validateErro;
        }
    }
}
