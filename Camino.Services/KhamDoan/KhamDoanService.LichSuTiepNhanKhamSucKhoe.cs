using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.Entities.KhamDoans;


namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public async Task<GridDataSource> GetDataLichSuTiepNhanKhamSucKhoeForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking.Where(x => (x.HopDongKhamSucKhoeNhanVien != null && x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.DaKetThuc == true))
                .Select(s => new LichSuTiepNhanKhamSucKhoeDoanGridVo
                {
                    Id = s.Id,
                    CongTyId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                    TenCongTy = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    HopDongId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.Id,
                    TenHopDong = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong,
                    SoLuongBenhNhan = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoNguoiKham,
                    SoBenhNhanDaDen = 1,
                    NgayBatDauKham = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayHieuLuc,
                    NgayKetThucKham = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc
                });
            if (queryInfo.AdditionalSearchString != null)
            {
                var queryString = JsonConvert.DeserializeObject<SearchVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.NgayBatDau) || !string.IsNullOrEmpty(queryString.NgayKetThuc))
                {
                    if (queryString.NgayBatDau != null && queryString.NgayKetThuc != null)
                    {
                        DateTime tuNgay = Convert.ToDateTime(queryString.NgayBatDau, CultureInfo.InvariantCulture);
                        DateTime denNgay = Convert.ToDateTime(queryString.NgayKetThuc, CultureInfo.InvariantCulture);

                        query = query.Where(p => p.NgayBatDauKham >= tuNgay && (p.NgayKetThucKham != null && p.NgayKetThucKham <= denNgay));
                        
                    }
                    if (queryString.NgayBatDau != null && queryString.NgayKetThuc == null)
                    {
                        DateTime tuNgay = Convert.ToDateTime(queryString.NgayBatDau, CultureInfo.InvariantCulture);
                        query = query.Where(p => p.NgayBatDauKham >= tuNgay );
                    }
                    if (queryString.NgayBatDau == null && queryString.NgayKetThuc != null)
                    {
                        DateTime denNgay = Convert.ToDateTime(queryString.NgayKetThuc, CultureInfo.InvariantCulture);
                        query = query.Where(p => (p.NgayKetThucKham != null && p.NgayKetThucKham <= denNgay));
                    }
                }
                if (queryString.SearchString != null)
                {
                    query = query.ApplyLike(queryString.SearchString, g => g.TenCongTy, g => g.TenHopDong);
                }
            }
            query = query.GroupBy(s => new { s.CongTyId, s.TenCongTy, s.HopDongId, s.TenHopDong })
            .Select(y => new LichSuTiepNhanKhamSucKhoeDoanGridVo
            {
                Id = y.First().Id,
                CongTyId = y.First().CongTyId,
                TenCongTy = y.First().TenCongTy,
                HopDongId = y.First().HopDongId,
                TenHopDong = y.First().TenHopDong,
                SoLuongBenhNhan = y.First().SoLuongBenhNhan,
                SoBenhNhanDaDen = y.Sum(s => s.SoBenhNhanDaDen),
                NgayBatDauKham = y.First().NgayBatDauKham,
                NgayKetThucKham = y.First().NgayKetThucKham
            });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.OrderBy(queryInfo.SortString).ToArrayAsync() : query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageLichSuTiepNhanKhamSucKhoeForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Where(x => (x.HopDongKhamSucKhoeNhanVien != null && x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.DaKetThuc == true))
               .Select(s => new LichSuTiepNhanKhamSucKhoeDoanGridVo
               {
                   Id = s.Id,
                   CongTyId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                   TenCongTy = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                   HopDongId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.Id,
                   TenHopDong = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong,
                   SoLuongBenhNhan = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoNguoiKham,
                   SoBenhNhanDaDen = 1,
                   NgayBatDauKham = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayHieuLuc,
                   NgayKetThucKham = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc
               });
            if (queryInfo.AdditionalSearchString != null)
            {
                var queryString = JsonConvert.DeserializeObject<SearchVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.NgayBatDau) || !string.IsNullOrEmpty(queryString.NgayKetThuc))
                {
                    if (queryString.NgayBatDau != null && queryString.NgayKetThuc != null)
                    {
                        DateTime tuNgay = Convert.ToDateTime(queryString.NgayBatDau, CultureInfo.InvariantCulture);
                        DateTime denNgay = Convert.ToDateTime(queryString.NgayKetThuc, CultureInfo.InvariantCulture);

                        query = query.Where(p => p.NgayBatDauKham >= tuNgay && (p.NgayKetThucKham != null && p.NgayKetThucKham <= denNgay));
                    }
                    if (queryString.NgayBatDau != null && queryString.NgayKetThuc == null)
                    {
                        DateTime tuNgay = Convert.ToDateTime(queryString.NgayBatDau, CultureInfo.InvariantCulture);
                        query = query.Where(p => p.NgayBatDauKham >= tuNgay);
                    }
                    if (queryString.NgayBatDau == null && queryString.NgayKetThuc != null)
                    {
                        DateTime denNgay = Convert.ToDateTime(queryString.NgayKetThuc, CultureInfo.InvariantCulture);
                        query = query.Where(p => (p.NgayKetThucKham != null && p.NgayKetThucKham <= denNgay));
                    }
                }
                if (queryString.SearchString != null)
                {
                    query = query.ApplyLike(queryString.SearchString, g => g.TenCongTy, g => g.TenHopDong);
                }
            }
            query = query.GroupBy(s => new { s.CongTyId, s.TenCongTy, s.HopDongId, s.TenHopDong })
            .Select(y => new LichSuTiepNhanKhamSucKhoeDoanGridVo
            {
                Id = y.First().Id,
                CongTyId = y.First().CongTyId,
                TenCongTy = y.First().TenCongTy,
                HopDongId = y.First().HopDongId,
                TenHopDong = y.First().TenHopDong,
                SoLuongBenhNhan = y.First().SoLuongBenhNhan,
                SoBenhNhanDaDen = y.Sum(s => s.SoBenhNhanDaDen),
                NgayBatDauKham = y.First().NgayBatDauKham,
                NgayKetThucKham = y.First().NgayKetThucKham
            });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #region In phiếu

        public async Task<string> XuLyInKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("SoKhamSucKhoeKhamDinhKyKhamDoan"));
            var thongTinNhanVienKham =
               await BaseRepository.TableNoTracking
                   .Include(x=>x.KSKNhanVienDanhGiaCanLamSang).ThenInclude(s=>s.User)
                   .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiDangKy)?.ThenInclude(p => p.User)
                    .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiThucHien)?.ThenInclude(p => p.User)
                   .Include(x => x.KetQuaSinhHieus)
                   .Include(x=>x.NgheNghiep)
                   .Include(x=>x.KSKNhanVienKetLuan)
                   .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y=>y.TinhThanh)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.QuanHuyen)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.PhuongXa)
                     .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HoKhauTinhThanh)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HoKhauQuanHuyen)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HoKhauPhuongXa)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.BenhNhan)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                   .OrderByDescending(x => x.Id)
                   .FirstOrDefaultAsync(x => x.HopDongKhamSucKhoeNhanVienId == phieuInNhanVienKhamSucKhoeInfoVo.Id);
            var columTable = "";
            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            var data = new SoDinhKyKhamSucKhoeVo();
            if (thongTinNhanVienKham != null)
            {
                data.HoVaTen = thongTinNhanVienKham.HoTen;
                data.Nam = thongTinNhanVienKham.GioiTinh.GetValueOrDefault() == Enums.LoaiGioiTinh.GioiTinhNam ? "X" : "";
                data.Nu = thongTinNhanVienKham.GioiTinh.GetValueOrDefault() == Enums.LoaiGioiTinh.GioiTinhNu ? "X" : "";
                data.SoCMNDHoacHoChieu = thongTinNhanVienKham.SoChungMinhThu;
                data.CapNgay = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.NgayCapChungMinhThu != null ? thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.NgayCapChungMinhThu.Value.ApplyFormatDate() : ""; // db chưa có 
                data.Tai = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.NoiCapChungMinhThu;
                data.HoKhauThuongTru = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HoKhauDiaChi + ", "
                                  + (thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HoKhauPhuongXa != null ? thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HoKhauPhuongXa.Ten + ", " : "")
                                  + (thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HoKhauQuanHuyen != null ? thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HoKhauQuanHuyen.Ten + ", " : "")
                                  + (thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HoKhauTinhThanh != null ? thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HoKhauTinhThanh.Ten : "");


                data.ChoOHienTai = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.DiaChi + ", "
                                  + (thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.PhuongXa != null ? thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.PhuongXa.Ten + ", " : "")
                                  + (thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.QuanHuyen != null ? thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.QuanHuyen.Ten + ", " : "")
                                  + (thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.TinhThanh != null ? thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.TinhThanh.Ten : "");
                data.NgheNghiep = thongTinNhanVienKham.NgheNghiepId != null ? thongTinNhanVienKham.NgheNghiep.Ten : "";
                data.NoiCongTacHocTap = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan;
                data.NgayBatDauVaoHocLamViecTaiDonViHienNay = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.NgayBatDauLamViec != null ? thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.NgayBatDauLamViec.Value.ApplyFormatDate() : "";// db chưa có; 

                // tien sử bệnh gia đình
                if (thongTinNhanVienKham.BenhNhan != null)
                {
                    if (thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Any())
                    {
                        if (thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Where(s => s.LoaiTienSuBenh == Enums.EnumLoaiTienSuBenh.GiaDinh).Any())
                        {
                            int ii = 1;
                            foreach (var itemTsbGD in thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Where(s => s.LoaiTienSuBenh == Enums.EnumLoaiTienSuBenh.GiaDinh))
                            {
                                data.TienSuBenhTatCuaGiaDinh += itemTsbGD.TenBenh;
                                if (thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Where(s => s.LoaiTienSuBenh == Enums.EnumLoaiTienSuBenh.GiaDinh).Count() > ii)
                                {
                                    data.TienSuBenhTatCuaGiaDinh += ". ";
                                }
                            }

                        }
                    }

                }

                data.NgayHienTai = DateTime.Now.Day;
                data.ThangHienTai = DateTime.Now.Month;
                data.NamHienTai = DateTime.Now.Year;
                data.HoTenNguoiLaoDongXacNhan = thongTinNhanVienKham.HoTen;

                data.HotenNguoiLapSo = nguoiLogin;
                //tiền sử bệnh tật của gia đình và bản than
                if (thongTinNhanVienKham.BenhNhan != null)
                {
                    if (thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs != null)
                    {
                        // tiền sử bản thân
                        int lenghtListBenhSuBanthan = thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Any(s => s.LoaiTienSuBenh == EnumLoaiTienSuBenh.BanThan) ? thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Count() : 0;
                        int lenghtListBenhSuGiaDinh = thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Any(s => s.LoaiTienSuBenh == EnumLoaiTienSuBenh.GiaDinh) ? thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Count() : 0;
                        int soLanChayBanThan = 1;
                        int soLanChayGiaDinh = 1;
                        if (thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Any(s=>s.LoaiTienSuBenh == EnumLoaiTienSuBenh.BanThan))
                        {
                            data.KhamSucKhoeDinhKyTienSuBenhTatBanThan += "Bản thân: ";
                        }
                        if (thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Any(s => s.LoaiTienSuBenh == EnumLoaiTienSuBenh.GiaDinh))
                        {
                            data.KhamSucKhoeDinhKyTienSuBenhTatGiaDinh += "Gia đình: ";
                        }
                        foreach (var itemtsbt in thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs)
                        {
                            if(itemtsbt.LoaiTienSuBenh == EnumLoaiTienSuBenh.BanThan)
                            {
                                data.KhamSucKhoeDinhKyTienSuBenhTatBanThan += itemtsbt.TenBenh;
                                if (lenghtListBenhSuBanthan > soLanChayBanThan)
                                {
                                    data.KhamSucKhoeDinhKyTienSuBenhTatBanThan += ". ";
                                }
                                soLanChayBanThan++;
                            }
                            if (itemtsbt.LoaiTienSuBenh == EnumLoaiTienSuBenh.GiaDinh)
                            {
                                data.KhamSucKhoeDinhKyTienSuBenhTatGiaDinh += itemtsbt.TenBenh;
                                if (lenghtListBenhSuGiaDinh > soLanChayGiaDinh)
                                {
                                    data.KhamSucKhoeDinhKyTienSuBenhTatGiaDinh += ". ";
                                }
                                soLanChayGiaDinh++;
                            }

                        }
                    }
                }

               //  ghi chú tiền sử bệnh bản thân
                if (!string.IsNullOrEmpty(thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.GhiChuTienSuBenh) && thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.GhiChuTienSuBenh != "[]")
                {
                    var jsonTienSuBenh = JsonConvert.DeserializeObject<List<TienSuBenhGridVo>>(thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.GhiChuTienSuBenh);
                    if (jsonTienSuBenh!=null && jsonTienSuBenh.Where(s => s.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan).Any())
                    {
                        var benhThuong = jsonTienSuBenh.Where(o => o.BenhNgheNghiep != true && o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan);
                        var benhNgheNghiep = jsonTienSuBenh.Where(o => o.BenhNgheNghiep == true && o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan);
                        if (benhThuong.Count() > benhNgheNghiep.Count())
                        {
                            var i = 0;
                            foreach (var itemTsbBT in benhThuong)
                            {
                                // phát hiện năm db k có
                                columTable += "<tr>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (itemTsbBT.TenBenh) + "</td>" +
                                              "<td style='width: 20%; border: 1px solid black;padding:5px;'>" + (itemTsbBT.PhatHienNam != null ? itemTsbBT.PhatHienNam.GetValueOrDefault().ApplyFormatDate() : "") + "</td>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (benhNgheNghiep.Count() > i ? benhNgheNghiep.ToArray()[i].TenBenh : "") + "</td>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (benhNgheNghiep.Count() > i ? (benhNgheNghiep.ToArray()[i].PhatHienNam != null ? benhNgheNghiep.ToArray()[i].PhatHienNam.GetValueOrDefault().ApplyFormatDate() : "") : "") + "</td>" +
                                              "</tr>";
                                i++;
                            }

                        }
                        else
                        {
                            var i = 0;
                            foreach (var itemTsbBT in benhNgheNghiep)
                            {
                                // phát hiện năm db k có
                                columTable += "<tr>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (benhThuong.Count() > i ? benhThuong.ToArray()[i].TenBenh : "") + "</td>" +
                                              "<td style='width: 20%; border: 1px solid black;padding:5px;'>" + (benhThuong.Count() > i ? (benhThuong.ToArray()[i].PhatHienNam != null ? benhThuong.ToArray()[i].PhatHienNam.GetValueOrDefault().ApplyFormatDate() : "") : "") + "</td>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (itemTsbBT.TenBenh) + "</td>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (itemTsbBT.PhatHienNam != null ? itemTsbBT.PhatHienNam.GetValueOrDefault().ApplyFormatDate() : "") + "</td>" +
                                              "</tr>";
                                i++;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            columTable += "<tr>" +
                                         "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                         "<td style='width: 20%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                         "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                         "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                         "</tr>";
                        }
                    }
                    //if (jsonTienSuBenh != null &&
                    //    jsonTienSuBenh.Where(s => s.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.GiaDinh).Any())
                    //{
                    //    var benhThuong = jsonTienSuBenh.Where(o => o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.GiaDinh);
                    //    if (benhThuong.Count() > 0)
                    //    {
                    //        var i = 1;
                    //        foreach (var itemTsbBT in benhThuong)
                    //        {
                    //            data.TienSuBenhTatCuaGiaDinh += itemTsbBT.TenBenh;
                    //            if (benhThuong.Count() > i)
                    //            {
                    //                data.TienSuBenhTatCuaGiaDinh += ". ";

                    //            }
                    //        }

                    //    }

                    //}
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        columTable += "<tr>" +
                                     "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                     "<td style='width: 20%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                     "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                     "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                     "</tr>";
                    }
                }
                data.TienSuBanThan = columTable;
                if (thongTinNhanVienKham.KetQuaSinhHieus.Any())
                {
                    data.ChieuCao = thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().ChieuCao != null ? thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().ChieuCao.ToString() : "";
                    data.CanNang = thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().CanNang != null ? thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().CanNang.ToString() : "";
                    data.ChiSoBMI = thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().Bmi != null ? thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().Bmi.ToString() : "";
                    data.Mach = thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().NhipTim != null ? thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().NhipTim.ToString() : "";
                    data.HuyetAp = thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().HuyetApTamThu + "/" + thongTinNhanVienKham.KetQuaSinhHieus.LastOrDefault().HuyetApTamTruong;
                    data.PhanLoaiTheLuc = thongTinNhanVienKham.KSKPhanLoaiTheLuc != null ? thongTinNhanVienKham.KSKPhanLoaiTheLuc.GetDescription() :"";


                }
                var dataTemplate = thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ThongTinKhamTheoDichVuData != null && s.ThongTinKhamTheoDichVuTemplate != null).Select(z => new TemplateAndDataGrid() { ThongTinKhamTheoDichVuTemplate = z.ThongTinKhamTheoDichVuTemplate, ThongTinKhamTheoDichVuData = z.ThongTinKhamTheoDichVuData }).ToList();
                if (dataTemplate.Any())
                {
                    foreach (var item in dataTemplate)
                    {
                        var jsonOjbectTemplate = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(item.ThongTinKhamTheoDichVuTemplate);
                        var jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(item.ThongTinKhamTheoDichVuData);

                        foreach (var itemx in jsonOjbectTemplate.ComponentDynamics)
                        {
                                var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemx.Id);
                                if(kiemTra.Any())
                                {
                                    switch (itemx.Id)
                                    {
                                        case "TuanHoan":
                                            data.TuanHoan = kiemTra.First().Value;
                                            break;
                                        case "TuanHoanPhanLoai":
                                            data.PhanLoaiTuanHoan = !string.IsNullOrEmpty(kiemTra.First().Value) ?((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() :"";
                                            break;
                                        case "HoHap":
                                            data.HoHap = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "HoHapPhanLoai":
                                            data.PhanLoaiHoHap = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "TieuHoa":
                                            data.TieuHoa = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "TieuHoaPhanLoai":
                                            data.PhanLoaiTieuHoa = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "ThanTietLieu":
                                            data.ThanTietNieu = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "ThanTietLieuPhanLoai":
                                            data.PhanLoaiThanTietNieu = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "NoiTiet":
                                            data.NoiTiet = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "NoiTietPhanLoai":
                                            data.PhanLoaiNoiTiet = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "CoXuongKhop":
                                            data.CoXuongKhop = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "CoXuongKhopPhanLoai":
                                            data.PhanLoaiCoXuongKhop = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "ThanKinh":
                                            data.ThanKinh = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "ThanKinhPhanLoai":
                                            data.PhanLoaiThanKinh = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "TamThan":
                                            data.TamThan = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "TamThanPhanLoai":
                                            data.PhanLoaiTamThan = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "NgoaiKhoa":
                                            data.NgoaiKhoa = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "NgoaiKhoaPhanLoai":
                                            data.PhanLoaiNgoaiKhoa = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "SanPhuKhoa":
                                            data.SanPhuKhoa = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "SanPhuKhoaPhanLoai":
                                            data.PhanLoaiSanPhuKhoa = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "KhongKinhMatPhai":
                                            data.KhongKinhMatPhai = kiemTra.FirstOrDefault().Value;
                                            break;
                                        //case "KhongKinhMatTrai":
                                        //    data.KhongKinhMatTrai = kiemTra.FirstOrDefault().Value;
                                        //    break;
                                        //case "CoKinhMatPhai":
                                        //    data.CoKinhMatPhai = kiemTra.FirstOrDefault().Value;
                                        //    break;
                                        //case "CoKinhMatTrai":
                                        //    data.CoKinhMatTrai = kiemTra.FirstOrDefault().Value;
                                        //    break;
                                        case "CacBenhVeMat":
                                            data.CacBenhVeMat = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "MatPhanLoai":
                                            data.PhanLoaiCacBenhVeMat = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "NoiThuongTrai":
                                            data.NoiThuongTrai = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "NoiThamTrai":
                                            data.NoiThamTrai = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "NoiThuongPhai":
                                            data.NoiThuongPhai = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "NoiThamPhai":
                                            data.NoiThamPhai = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "CacBenhVeTaiMuiHong":
                                            data.CacBenhVeTaiMuiHong = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "TaiMuiHongPhanLoai":
                                            data.PhanLoaiTaiMuiHong = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "HamTren":
                                            data.HamTren = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "HamDuoi":
                                            data.HamDuoi = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "CacBenhRangHamMat":
                                            data.CacBenhVeRangHamMat = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "RangHamMatPhanLoai":
                                            data.PhanLoaiRangHamMat = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        case "DaLieu":
                                            data.DaLieu = kiemTra.FirstOrDefault().Value;
                                            break;
                                        case "DaLieuPhanLoai":
                                            data.PhanLoaiDaLieu = !string.IsNullOrEmpty(kiemTra.First().Value) ? ((Enums.PhanLoaiSucKhoe)(int.Parse(kiemTra.First().Value))).GetDescription() : "";
                                            break;
                                        default:
                                            //do a different thing
                                            break;
                                    }
                                }
                           
                          
                        }
                        // GROUP
                        var kiemTraGroupItem = jsonOjbectTemplate.ComponentDynamics.Where(s => s.Id == "group");

                        foreach (var itemx in kiemTraGroupItem)
                        {
                            if(itemx.groupItems != null)
                            {
                                foreach (var itemxx in itemx.groupItems)
                                {
                                    var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemxx.Id);
                                    if (kiemTra.Any())
                                    {
                                        switch (itemxx.Id)
                                        {
                                           
                                            case "KhongKinhMatPhai":
                                                data.KhongKinhMatPhai = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "KhongKinhMatTrai":
                                                data.KhongKinhMatTrai = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "CoKinhMatPhai":
                                                data.CoKinhMatPhai = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "CoKinhMatTrai":
                                                data.CoKinhMatTrai = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "CacBenhVeMat":
                                                data.CacBenhVeMat = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "MatPhanLoai":
                                                data.PhanLoaiCacBenhVeMat = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "TaiTraiNoiThuong":
                                                data.NoiThuongTrai = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "TaiTraiNoiTham":
                                                data.NoiThamTrai = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "TaiPhaiNoiThuong":
                                                data.NoiThuongPhai = kiemTra.FirstOrDefault().Value;
                                                break;
                                            case "TaiPhaiNoiTham":
                                                data.NoiThamPhai = kiemTra.FirstOrDefault().Value;
                                                break;
                                            default:
                                                //do a different thing
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                    }
                } 
                    // bs theo khoa
                    #region khoa nội
                    if (thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa).Any())
                    {
                        data.BsKhoaNoi = thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa).Select(z => z.BacSiThucHien?.User?.HoTen).FirstOrDefault(); ; // 1 yêu cầu khám bệnh ứng với 1 chuyên khoa
                    }


                    #endregion
                    #region khoa ngoại
                    if (thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa).Any())
                    {
                        data.BsNgoaKhoa = thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa).Select(z => z.BacSiThucHien?.User?.HoTen).FirstOrDefault(); ; // 1 yêu cầu khám bệnh ứng với 1 chuyên khoa
                    }
                   
                    #endregion
                    #region bs phụ sản
                    if (thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa).Any())
                    {
                        data.BsSanPhuKhoa = thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa).Select(z => z.BacSiThucHien?.User?.HoTen).FirstOrDefault(); ; // 1 yêu cầu khám bệnh ứng với 1 chuyên khoa
                    }
                  
                    #endregion
                    #region bs mẳt
                    if (thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.Mat).Any())
                    {
                        data.BsMat = thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.Mat).Select(z => z.BacSiThucHien?.User?.HoTen).FirstOrDefault(); ; // 1 yêu cầu khám bệnh ứng với 1 chuyên khoa
                    }
                    
                    #endregion
                    #region bs tai mũi họng
                    if (thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong).Any())
                    {
                        data.BsTaiMuiHong = thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong).Select(z => z.BacSiThucHien?.User?.HoTen).FirstOrDefault(); ; // 1 yêu cầu khám bệnh ứng với 1 chuyên khoa
                    }
                    #endregion
                    #region bs răng hàm mặt
                    if (thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.RangHamMat).Any())
                    {
                        data.BsRangHamMat = thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.RangHamMat).Select(z => z.BacSiThucHien?.User?.HoTen).FirstOrDefault(); ; // 1 yêu cầu khám bệnh ứng với 1 chuyên khoa
                    }
                    #endregion
                    #region da liễu
                    if (thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.DaLieu).Any())
                    {
                        data.BsDaLieu = thongTinNhanVienKham.YeuCauKhamBenhs.Where(s => s.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.DaLieu).Select(z => z.BacSiThucHien?.User?.HoTen).FirstOrDefault(); ; // 1 yêu cầu khám bệnh ứng với 1 chuyên khoa
                    }
                #endregion
                string columCVBenhNhan = "";
                





                List<string> mylist = new List<string>(new string[] { "a", "b", "c","d","e","f","g","h","i","K","l" });
                if (thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.NgheCongViecTruocDay != null)
                {
                    var jsonOjbectCongViecBenhNhan = JsonConvert.DeserializeObject<List<ThongTinViecLamBenNhan>>(thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.NgheCongViecTruocDay);
                    int i = 0;
                    foreach (var item in jsonOjbectCongViecBenhNhan)
                    {
                        columCVBenhNhan += "<div class='container'><div class='label'>" + mylist[i] + ")" + "</div>" + "<div class='value'>" + item.CongViec + "</div></div>";
                        string thoigianLamViec = item.TuNgay != null ? item.TuNgay.Value.ApplyFormatTime() : "";
                        string nam = item.TuNgay != null ? item.TuNgay.Value.Year.ToString() : "";
                        string NgayThangNamTu = item.TuNgay != null ? item.TuNgay.Value.ApplyFormatDate() : "";
                        string NgayThangNamDen = item.DenNgay != null ? item.DenNgay.Value.ApplyFormatDate() : "";

                        string itemCongViec = item.CongViec == null || item.CongViec !="" ? item.CongViec : "&nbsp;&nbsp;&nbsp;&nbsp;";
                        columCVBenhNhan += "<div class='container'><div class='label'>" +
                            "thời gian làm việc" + "<span style= ' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;' >" +
                            "  " +
                            "</span> năm<span style=' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;'>" + "&nbsp;&nbsp;&nbsp;  " +
                            "</span> tháng từ ngày</div><div class='value'>&nbsp;" + NgayThangNamTu + "</div></div>";
                        columCVBenhNhan += "<div class='containerGD'><div class='label'>đến</div><div class='value'>&nbsp;" + NgayThangNamDen + "</div></div>";
                        i++;
                    }
                    if (jsonOjbectCongViecBenhNhan.Count() == 1)
                    {
                        columCVBenhNhan += "<div class='container'><div class='label'>" + mylist[i] + ")" + "</div>" + "<div class='value'>" + "&nbsp;&nbsp;&nbsp;  " + "</div></div>";
                        columCVBenhNhan += "<div class='container'><div class='label'>" +
                            "thời gian làm việc" + "<span style= ' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;' >" +
                            "   " +
                            "</span> năm<span style=' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;'>" + "&nbsp;&nbsp;&nbsp;  " +
                            "</span> tháng từ ngày</div><div class='value'>" + " &nbsp;&nbsp;&nbsp; " + "</div></div>";
                        columCVBenhNhan += "<div class='containerGD'><div class='label'>đến</div><div class='value'>" + "&nbsp;&nbsp;&nbsp;" + "</div></div>";
                    }
                    
                }
                else
                {
                    for (var item = 0; item < 2; item++)
                    {
                        columCVBenhNhan += "<div class='container'><div class='label'>" + mylist[item] + ")" + "</div>" + "<div class='value'>" + "  " + "</div></div>";
                        columCVBenhNhan += "<div class='container'><div class='label'>" +
                            "thời gian làm việc" + "<span style= ' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;' >" +
                            "   " +
                            "</span> năm<span style=' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;'>" + "  " +
                            "</span> tháng từ ngày</div><div class='value'>" + "  " + "</div></div>";
                        columCVBenhNhan += "<div class='containerGD'><div class='label'>đến</div><div class='value'>" + "  " + "</div></div>";
                    }
                }
                data.CongViec = columCVBenhNhan;
                data.KetQua = thongTinNhanVienKham.KSKKetQuaCanLamSang;
                data.DanhGia = thongTinNhanVienKham.KSKDanhGiaCanLamSang;
                data.KetLuanPhanLoaiSucKhoe = thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe;
                data.KetLuanCacBenhTatNeuCo = thongTinNhanVienKham.KSKKetLuanCacBenhTat;
                data.BacSiKetLuanCLS = thongTinNhanVienKham.KSKNhanVienDanhGiaCanLamSangId != null ? thongTinNhanVienKham.KSKNhanVienDanhGiaCanLamSang?.User.HoTen : "";
                data.NguoiKetLuan = thongTinNhanVienKham.KSKNhanVienKetLuanId != null ? thongTinNhanVienKham.KSKNhanVienKetLuan.User.HoTen : "";
            }
            else
            {
                // trường hợp hợp đồng nhân viên chưa bắt đầu khám --> chưa có tạo yêu cầu tiếp nhận 
                var dataHopDongNhanVien = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                                                                       .Include(y => y.TinhThanh)
                                                                       .Include(y => y.QuanHuyen)
                                                                       .Include(y => y.PhuongXa)
                                                                       .Include(y => y.HoKhauTinhThanh)
                                                                       .Include(y => y.HoKhauQuanHuyen)
                                                                       .Include(y => y.HoKhauPhuongXa)
                                                                       .Include(x => x.GoiKhamSucKhoe)
                                                                       .Include(x => x.HopDongKhamSucKhoe)
                                                                       .Include(x=>x.NgheNghiep)
                                                                       .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                                                                       .Include(z => z.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                                                                       .Where(s => s.Id == phieuInNhanVienKhamSucKhoeInfoVo.Id).FirstOrDefaultAsync();
                if (dataHopDongNhanVien.GhiChuTienSuBenh != null && dataHopDongNhanVien.GhiChuTienSuBenh != "[]")
                {
                    var jsonTienSuBenh =
                        JsonConvert.DeserializeObject<List<TienSuBenhGridVo>>(dataHopDongNhanVien.GhiChuTienSuBenh);
                    if (jsonTienSuBenh != null &&
                        jsonTienSuBenh.Where(s => s.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan).Any())
                    {
                        var benhThuong = jsonTienSuBenh.Where(o => o.BenhNgheNghiep != true && o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan);
                        var benhNgheNghiep = jsonTienSuBenh.Where(o => o.BenhNgheNghiep == true && o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan);
                        if (benhThuong.Count() > benhNgheNghiep.Count())
                        {
                            var i = 0;
                            foreach (var itemTsbBT in benhThuong)
                            {
                                // phát hiện năm db k có
                                columTable += "<tr>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" +
                                              (itemTsbBT.TenBenh) + "</td>" +
                                              "<td style='width: 20%; border: 1px solid black;padding:5px;'>" +
                                              (itemTsbBT.PhatHienNam != null
                                                  ? itemTsbBT.PhatHienNam.GetValueOrDefault().ApplyFormatDate()
                                                  : "") + "</td>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" +
                                              (benhNgheNghiep.Count() > i ? benhNgheNghiep.ToArray()[i].TenBenh : "") +
                                              "</td>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" +
                                              (benhNgheNghiep.Count() > i
                                                  ? (benhNgheNghiep.ToArray()[i].PhatHienNam != null
                                                      ? benhNgheNghiep.ToArray()[i].PhatHienNam.GetValueOrDefault()
                                                          .ApplyFormatDate()
                                                      : "")
                                                  : "") + "</td>" +
                                              "</tr>";
                                i++;
                            }

                        }
                        else
                        {
                            var i = 0;
                            foreach (var itemTsbBT in benhNgheNghiep)
                            {
                                // phát hiện năm db k có
                                columTable += "<tr>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" +
                                              (benhThuong.Count() > i ? benhThuong.ToArray()[i].TenBenh : "") +
                                              "</td>" +
                                              "<td style='width: 20%; border: 1px solid black;padding:5px;'>" +
                                              (benhThuong.Count() > i
                                                  ? (benhThuong.ToArray()[i].PhatHienNam != null
                                                      ? benhThuong.ToArray()[i].PhatHienNam.GetValueOrDefault()
                                                          .ApplyFormatDate()
                                                      : "")
                                                  : "") + "</td>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" +
                                              (itemTsbBT.TenBenh) + "</td>" +
                                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" +
                                              (itemTsbBT.PhatHienNam != null
                                                  ? itemTsbBT.PhatHienNam.GetValueOrDefault().ApplyFormatDate()
                                                  : "") + "</td>" +
                                              "</tr>";
                                i++;
                            }
                        }

                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            columTable += "<tr>" +
                                          "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                          "<td style='width: 20%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                          "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                          "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                          "</tr>";
                        }
                    }
                    if (jsonTienSuBenh != null &&
                        jsonTienSuBenh.Where(s => s.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.GiaDinh).Any())
                    {
                        var benhThuong = jsonTienSuBenh.Where(o => o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.GiaDinh);
                        if (benhThuong.Count() > 0)
                        {
                            var i = 1;
                            foreach (var itemTsbBT in benhThuong)
                            {
                                data.TienSuBenhTatCuaGiaDinh += itemTsbBT.TenBenh;
                                if (benhThuong.Count() > i)
                                {
                                    data.TienSuBenhTatCuaGiaDinh += ". ";
                                }
                            }

                        }

                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        columTable += "<tr>" +
                                      "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                      "<td style='width: 20%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                      "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                      "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                                      "</tr>";
                    }
                }


                data.HoVaTen = dataHopDongNhanVien.HoTen;
                data.Nam = dataHopDongNhanVien.GioiTinh.GetValueOrDefault() == Enums.LoaiGioiTinh.GioiTinhNam ? "X" : "";
                data.Nu = dataHopDongNhanVien.GioiTinh.GetValueOrDefault() == Enums.LoaiGioiTinh.GioiTinhNu ? "X" : "";
                data.SoCMNDHoacHoChieu = dataHopDongNhanVien.SoChungMinhThu;
                data.CapNgay = dataHopDongNhanVien.NgayCapChungMinhThu != null ? dataHopDongNhanVien.NgayCapChungMinhThu.GetValueOrDefault().ApplyFormatDate() : ""; // db chưa có 
                data.Tai = dataHopDongNhanVien.NoiCapChungMinhThu;

                data.HoKhauThuongTru = dataHopDongNhanVien.HoKhauDiaChi + ", "
                                 + (dataHopDongNhanVien.HoKhauPhuongXa != null ? dataHopDongNhanVien.HoKhauPhuongXa.Ten + ", " : "")
                                 + (dataHopDongNhanVien.HoKhauQuanHuyen != null ? dataHopDongNhanVien.HoKhauQuanHuyen.Ten + ", " : "")
                                 + (dataHopDongNhanVien.HoKhauTinhThanh != null ? dataHopDongNhanVien.HoKhauTinhThanh.Ten + ", " : "")
                                 ;


                data.ChoOHienTai = dataHopDongNhanVien.DiaChi + ", "
                                  + (dataHopDongNhanVien.PhuongXa != null ? dataHopDongNhanVien.PhuongXa.Ten + ", " : "")
                                  + (dataHopDongNhanVien.QuanHuyen != null ? dataHopDongNhanVien.QuanHuyen.Ten + ", " : "")
                                  + (dataHopDongNhanVien.TinhThanh != null ? dataHopDongNhanVien.TinhThanh.Ten + ", " : "")
                                  ;

                data.NgheNghiep = dataHopDongNhanVien.NgheNghiepId != null ? dataHopDongNhanVien.NgheNghiep.Ten : "";
                data.NoiCongTacHocTap = dataHopDongNhanVien.TenDonViHoacBoPhan;
                data.NgayBatDauVaoHocLamViecTaiDonViHienNay = dataHopDongNhanVien.NgayBatDauLamViec != null ? dataHopDongNhanVien.NgayBatDauLamViec?.ApplyFormatDateTime():"";
               
                //if (!string.IsNullOrEmpty(dataHopDongNhanVien.GhiChuTienSuBenh) && dataHopDongNhanVien.GhiChuTienSuBenh != "[]")
                //{
                //    var jsonTienSuBenh = JsonConvert.DeserializeObject<List<TienSuBenhGridVo>>(dataHopDongNhanVien.GhiChuTienSuBenh);
                //    if (jsonTienSuBenh != null && jsonTienSuBenh.Where(s => s.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan).Any())
                //    {
                //        var benhThuong = jsonTienSuBenh.Where(o => o.BenhNgheNghiep != true && o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan);
                //        var benhNgheNghiep = jsonTienSuBenh.Where(o => o.BenhNgheNghiep == true && o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.BanThan);
                //        if (benhThuong.Count() > benhNgheNghiep.Count())
                //        {
                //            var i = 0;
                //            foreach (var itemTsbBT in benhThuong)
                //            {
                //                // phát hiện năm db k có
                //                columTable += "<tr>" +
                //                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (itemTsbBT.TenBenh) + "</td>" +
                //                              "<td style='width: 20%; border: 1px solid black;padding:5px;'>" + (itemTsbBT.PhatHienNam != null ? itemTsbBT.PhatHienNam.GetValueOrDefault().ApplyFormatDate() : "") + "</td>" +
                //                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (benhNgheNghiep.Count() > i ? benhNgheNghiep.ToArray()[i].TenBenh : "") + "</td>" +
                //                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (benhNgheNghiep.Count() > i ? (benhNgheNghiep.ToArray()[i].PhatHienNam != null ? benhNgheNghiep.ToArray()[i].PhatHienNam.GetValueOrDefault().ApplyFormatDate() : "") : "") + "</td>" +
                //                              "</tr>";
                //                i++;
                //            }

                //        }
                //        else
                //        {
                //            var i = 0;
                //            foreach (var itemTsbBT in benhNgheNghiep)
                //            {
                //                // phát hiện năm db k có
                //                columTable += "<tr>" +
                //                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (benhThuong.Count() > i ? benhThuong.ToArray()[i].TenBenh : "") + "</td>" +
                //                              "<td style='width: 20%; border: 1px solid black;padding:5px;'>" + (benhThuong.Count() > i ? (benhThuong.ToArray()[i].PhatHienNam != null ? benhThuong.ToArray()[i].PhatHienNam.GetValueOrDefault().ApplyFormatDate() : "") : "") + "</td>" +
                //                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (itemTsbBT.TenBenh) + "</td>" +
                //                              "<td style='width: 30%; border: 1px solid black;padding:5px;'>" + (itemTsbBT.PhatHienNam != null ? itemTsbBT.PhatHienNam.GetValueOrDefault().ApplyFormatDate() : "") + "</td>" +
                //                              "</tr>";
                //                i++;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        for (int i = 0; i < 2; i++)
                //        {
                //            columTable += "<tr>" +
                //                         "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                //                         "<td style='width: 20%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                //                         "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                //                         "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                //                         "</tr>";
                //        }
                //    }
                //    if (jsonTienSuBenh != null &&
                //        jsonTienSuBenh.Where(s => s.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.GiaDinh).Any())
                //    {
                //        var benhThuong = jsonTienSuBenh.Where(o => o.LoaiTienSuId == Enums.EnumLoaiTienSuBenh.GiaDinh);
                //        if (benhThuong.Count() > 0)
                //        {
                //            var i = 1;
                //            foreach (var itemTsbBT in benhThuong)
                //            {
                //                data.TienSuBenhTatCuaGiaDinh += itemTsbBT.TenBenh;
                //                if (benhThuong.Count() > i)
                //                {
                //                    data.TienSuBenhTatCuaGiaDinh += ". ";
                //                }
                //            }

                //        }

                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < 2; i++)
                //    {
                //        columTable += "<tr>" +
                //                     "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                //                     "<td style='width: 20%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                //                     "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                //                     "<td style='width: 30%; border: 1px solid black;'>" + "&nbsp;" + "</td>" +
                //                     "</tr>";
                //    }
                //}
                data.TienSuBanThan = columTable;
                data.TienSuBanThan = columTable;
                data.NgayHienTai = DateTime.Now.Day;
                data.ThangHienTai = DateTime.Now.Month;
                data.NamHienTai = DateTime.Now.Year;
                data.HoTenNguoiLaoDongXacNhan = dataHopDongNhanVien.HoTen;
                data.HotenNguoiLapSo = nguoiLogin;
                string columCVBenhNhan = "";






                List<string> mylist = new List<string>(new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "K", "l" });
                if (dataHopDongNhanVien.NgheCongViecTruocDay != null)
                {
                    var jsonOjbectCongViecBenhNhan = JsonConvert.DeserializeObject<List<ThongTinViecLamBenNhan>>(dataHopDongNhanVien.NgheCongViecTruocDay);
                    int i = 0;
                    foreach (var item in jsonOjbectCongViecBenhNhan)
                    {
                        columCVBenhNhan += "<div class='container'><div class='label'>" + mylist[i] + ")" + "</div>" + "<div class='value'>" + item.CongViec + "</div></div>";
                        string thoigianLamViec = item.TuNgay != null ? item.TuNgay.Value.ApplyFormatTime() : "";
                        string nam = item.TuNgay != null ? item.TuNgay.Value.Year.ToString() : "";
                        string NgayThangNamTu = item.TuNgay != null ? item.TuNgay.Value.ApplyFormatDate() : "";
                        string NgayThangNamDen = item.DenNgay != null ? item.DenNgay.Value.ApplyFormatDate() : "";

                        string itemCongViec = item.CongViec == null || item.CongViec != "" ? item.CongViec : "&nbsp;&nbsp;&nbsp;&nbsp;";
                        columCVBenhNhan += "<div class='container'><div class='label'>" +
                            "thời gian làm việc" + "<span style= ' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;' >" +
                            " "  +
                            "</span> năm<span style=' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;'>" + "&nbsp;&nbsp;&nbsp;  " +
                            "</span> tháng từ ngày</div><div class='value'>&nbsp;" + NgayThangNamTu + "</div></div>";
                        columCVBenhNhan += "<div class='containerGD'><div class='label'>đến</div><div class='value'>&nbsp;" + NgayThangNamDen + "</div></div>";
                        i++;
                    }
                    if (jsonOjbectCongViecBenhNhan.Count() == 1)
                    {
                        columCVBenhNhan += "<div class='container'><div class='label'>" + mylist[i] + ")" + "</div>" + "<div class='value'>" + "&nbsp;&nbsp;&nbsp;  " + "</div></div>";
                        columCVBenhNhan += "<div class='container'><div class='label'>" +
                            "thời gian làm việc" + "<span style= ' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;' >" +
                            "   " +
                            "</span> năm<span style=' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;'>" + "&nbsp;&nbsp;&nbsp;  " +
                            "</span> tháng từ ngày</div><div class='value'>" + " &nbsp;&nbsp;&nbsp; " + "</div></div>";
                        columCVBenhNhan += "<div class='containerGD'><div class='label'>đến</div><div class='value'>" + "&nbsp;&nbsp;&nbsp;" + "</div></div>";
                    }

                }
                else
                {
                    for (var item = 0; item < 2; item++)
                    {
                        columCVBenhNhan += "<div class='container'><div class='label'>" + mylist[item] + ")" + "</div>" + "<div class='value'>" + "  " + "</div></div>";
                        columCVBenhNhan += "<div class='container'><div class='label'>" +
                            "thời gian làm việc" + "<span style= ' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;' >" +
                            "   " +
                            "</span> năm<span style=' width: 100%; height: 100%; vertical-align: top; position: relative; box-sizing: border-box; border-bottom: 1px dotted black;'>" + "  " +
                            "</span> tháng từ ngày</div><div class='value'>" + "  " + "</div></div>";
                        columCVBenhNhan += "<div class='containerGD'><div class='label'>đến</div><div class='value'>" + "  " + "</div></div>";
                    }
                }
                data.CongViec = columCVBenhNhan;
                //if (thongTinNhanVienKham != null)
                //{
                //    if (thongTinNhanVienKham.BenhNhan != null)
                //    {
                //        if (thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Where(s => s.LoaiTienSuBenh == Enums.EnumLoaiTienSuBenh.BanThan).Any())
                //        {
                //            foreach (var itemTsbGD in thongTinNhanVienKham.BenhNhan.BenhNhanTienSuBenhs.Where(s => s.LoaiTienSuBenh == Enums.EnumLoaiTienSuBenh.BanThan))
                //            {
                //                data.TienSuBenhTatBN += itemTsbGD.TenBenh;
                //            }
                //        }
                //    }

                //}

                data.ChieuCao = "";
                data.CanNang = "";
                data.ChiSoBMI = "";
                data.Mach = "";
                data.HuyetAp = "";
                data.PhanLoaiTheLuc = "";
                #region khoa nội
                data.BsKhoaNoi = "";
                data.TuanHoan = "";
                data.PhanLoaiTuanHoan = "";
                data.HoHap = "";
                data.PhanLoaiHoHap = "";
                data.TieuHoa = "";
                data.PhanLoaiTieuHoa = "";
                data.ThanTietNieu = "";
                data.PhanLoaiThanTietNieu = "";

                data.NoiTiet = "";
                data.PhanLoaiNoiTiet = "";
                data.CoXuongKhop = "";
                data.PhanLoaiCoXuongKhop = "";
                data.ThanKinh = "";
                data.PhanLoaiThanKinh = "";
                data.TamThan = "";
                data.PhanLoaiTamThan = "";
                #endregion
                #region khoa ngoại
                data.BsNgoaKhoa = "";
                data.NgoaiKhoa = "";
                data.PhanLoaiNgoaiKhoa = "";
                #endregion
                #region bs phụ sản
                data.BsSanPhuKhoa = "";
                data.SanPhuKhoa = "";
                data.PhanLoaiSanPhuKhoa = "";
                #endregion
                #region bs mẳt
                data.BsMat = "";
                data.KhongKinhMatPhai = "";
                data.KhongKinhMatTrai = "";
                data.CoKinhMatPhai = "";
                data.CoKinhMatTrai = "";
                data.CacBenhVeMat = "";
                data.PhanLoaiCacBenhVeMat = "";
                #endregion
                #region bs tai mũi họng
                data.BsTaiMuiHong = "";
                data.NoiThuongTrai = "";
                data.NoiThamTrai = "";
                data.NoiThuongPhai = "";
                data.NoiThamPhai = "";
                data.CacBenhVeTaiMuiHong = "";
                data.PhanLoaiTaiMuiHong = "";
                #endregion
                #region bs răng hàm mặt
                data.BsRangHamMat = "";
                data.HamTren = "";
                data.HamDuoi = "";
                data.CacBenhVeRangHamMat = "";
                data.PhanLoaiRangHamMat = "";
                #endregion
                #region bs răng hàm mặt
                data.BsDaLieu = "";
                data.DaLieu = "";
                data.HamDuoi = "";
                data.PhanLoaiDaLieu = "";
                #endregion
            }
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);// + "<div class='pagebreak'></div>";
            return content;
        }
        public async Task<string> XuLyInPhieuDangKyKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo)
        {
            //
            // ===> BVHD-3929: XuLyInNhieuPhieuDangKyKhamSucKhoeAsync có tham khảo logic từ function này, nếu ai có update gì ở đây thì cũng phải update ở function liên quan <====
            //
            var content = string.Empty;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuDangKyKhamSucKhoeKhamDoan"));
            var thongTinNhanVienKham =
               await BaseRepository.TableNoTracking
                   .Include(x => x.YeuCauKhamBenhs)
                   .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y=>y.NhomDichVuBenhVien)
                   .Include(x => x.KetQuaSinhHieus)
                   
                   .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.BenhNhan)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HopDongKhamSucKhoe).ThenInclude(z => z.CongTyKhamSucKhoe)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                   .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                   .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)
                   .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiThucHien)
                   .Include(p=>p.YeuCauDichVuKyThuats).ThenInclude(p => p.NoiThucHien)
                   .OrderByDescending(x => x.Id)
                   .FirstOrDefaultAsync(x => x.HopDongKhamSucKhoeNhanVienId == phieuInNhanVienKhamSucKhoeInfoVo.Id);
            var columTable = "";
            var data = new PhieuDangKyKhamSucKhoeVo();
            List<DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan> listDichVu = new List<DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan>();
            if (thongTinNhanVienKham != null)
            {
                data.DonViKham = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten;
                data.HoTen = thongTinNhanVienKham.HoTen;
                data.GioiTinh = thongTinNhanVienKham.GioiTinh != null ? thongTinNhanVienKham.GioiTinh.Value.GetDescription() : "";
                data.NamSinh = thongTinNhanVienKham.NamSinh != null ? thongTinNhanVienKham.NamSinh.ToString() : "";
                data.MaNhanVien = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.MaNhanVien;
                data.ChucVu = "";
                data.GhiChu = "";
                data.ViTriCongTac = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan;
                if (thongTinNhanVienKham.YeuCauKhamBenhs != null)
                {
                    // id == 1 DVKB id== 2 DVKT
                    listDichVu.AddRange(thongTinNhanVienKham.YeuCauKhamBenhs.Where(p=>p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(s => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                    {
                        Id = 1,
                        TenDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                        NoiDung = s.TenDichVu,
                        KhoaPhongThucHien = s.NoiThucHien == null ? s.NoiDangKy?.Ten : s.NoiThucHien?.Ten,
                        NgayDV = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : "",
                        GhiChu = "", // chưa bt 
                    }));
                }
                if (thongTinNhanVienKham.YeuCauDichVuKyThuats != null)
                {
                    listDichVu.AddRange(thongTinNhanVienKham.YeuCauDichVuKyThuats.Where(p=>p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.GoiKhamSucKhoeId != null).Select(s => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                    {
                        Id = 2,
                        DichVuKyThuatId = s.Id,
                        NoiDung = s.NhomDichVuBenhVien.Ten,
                        TenDichVu = s.TenDichVu,
                        KhoaPhongThucHien = s.NoiThucHien?.Ten,
                        NgayDV = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : "",
                        GhiChu = "", // chưa bt 
                    }));
                }
                // dịch vụ khám bệnh
                columTable += "<tr>" +
                                 "<td colspan='5'  class='border-table'><strong>" + "KHÁM BỆNH" + "</strong></td>" +
                                 "</tr>";
                int stt = 1;
                foreach (var item in listDichVu.Where(s => s.Id == 1).ToList())
                {
                    columTable += "<tr>" +
                                  "<td  class='border-table'>" + stt + "</td>" +
                                  "<td class='border-table'>" + item.NoiDung + "</td>" +
                                  "<td class='border-table'>" + item.NgayDV + "</td>" +
                                  "<td class='border-table'>" + item.KhoaPhongThucHien + "</td>" +
                                  "<td class='border-table'>" + item.GhiChu + "</td>" +
                                  "</tr>";
                    stt++;
                }
                foreach (var item in listDichVu.Where(z => z.Id == 2).GroupBy(s => s.NoiDung).ToList())
                {
                    columTable += "<tr>" +
                              "<td colspan='5' class='border-table'><strong>" + item.FirstOrDefault().NoiDung + "</strong></td>" +
                              "</tr>";
                    var listDVKT = item.Select(d => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan() {
                        Id = d.Id,
                        DichVuKyThuatId = d.DichVuKyThuatId,
                        NoiDung = d.NoiDung,
                        TenDichVu = d.TenDichVu,
                        KhoaPhongThucHien = d.KhoaPhongThucHien,
                        NgayDV = d.NgayDV,
                        GhiChu = d.GhiChu, 
                    });
                    foreach (var itemx in listDVKT)
                    {
                        var dichVu = listDichVu.Where(z => z.Id == 2 && z.DichVuKyThuatId == itemx.DichVuKyThuatId);
                        if (dichVu.Any())
                        {
                            columTable += "<tr>" +
                                "<td class='border-table'>" + stt + "</td>" +
                                "<td class='border-table'>" + itemx.TenDichVu + "</td>" +
                                "<td class='border-table'>" + itemx.NgayDV + "</td>" +
                                "<td class='border-table'>" + itemx.KhoaPhongThucHien + "</td>" +
                                "<td class='border-table'>" + itemx.GhiChu + "</td>" +
                                "</tr>";
                            stt++;
                        }
                    }
                }
                data.columnTable = columTable;
                data.NhanVienTiepDon = thongTinNhanVienKham.NhanVienTiepNhan != null ? thongTinNhanVienKham.NhanVienTiepNhan.User.HoTen : "";
                data.Ngay = DateTime.Now.Day.ToString();
                data.Thang = DateTime.Now.Month.ToString();
                data.Nam = DateTime.Now.Year.ToString();
                data.NguoiDiKham = thongTinNhanVienKham.HoTen;
                data.LogoUrl = phieuInNhanVienKhamSucKhoeInfoVo.HostingName + "/assets/img/logo-bacha-full.png";
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(thongTinNhanVienKham.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(thongTinNhanVienKham.MaYeuCauTiepNhan) : "";
                data.NgayGioSACH = DateTime.Now.ApplyFormatDateTimeSACH();
                data.MaTN = thongTinNhanVienKham.MaYeuCauTiepNhan;
            }
            else
            {
                var dataGoiKham = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                    .Include(c=>c.BenhNhan)
                    .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc => cc.GoiKhamSucKhoeDichVuKhamBenhs)
                     .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc => cc.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(cc => cc.GoiKhamSucKhoeNoiThucHiens).ThenInclude(cc=>cc.PhongBenhVien)
                    .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc => cc.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(cv=>cv.DichVuKyThuatBenhVien)
                    .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc => cc.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(cc=>cc.GoiKhamSucKhoeNoiThucHiens).ThenInclude(cc => cc.PhongBenhVien)
                    .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc=>cc.YeuCauKhamBenhs).ThenInclude(cc=>cc.NoiThucHien)
                    .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc => cc.YeuCauKhamBenhs).ThenInclude(cc => cc.NoiDangKy)
                    .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc => cc.YeuCauDichVuKyThuats).ThenInclude(cc => cc.NoiThucHien)

                    .Include(c=>c.HopDongKhamSucKhoe).ThenInclude(cc=>cc.CongTyKhamSucKhoe)
                    .Where(s => s.Id == phieuInNhanVienKhamSucKhoeInfoVo.Id);


                data.DonViKham = dataGoiKham.FirstOrDefault() != null ? dataGoiKham.FirstOrDefault().HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten :"";
                data.HoTen = dataGoiKham.FirstOrDefault().HoTen;
                data.GioiTinh = dataGoiKham.Select(s => s.GioiTinh).FirstOrDefault() != null ? dataGoiKham.Select(s => s.GioiTinh).FirstOrDefault().GetDescription() : "";
                data.NamSinh = dataGoiKham.FirstOrDefault().NamSinh != null ? dataGoiKham.FirstOrDefault().NamSinh.ToString() : "";
                data.MaNhanVien = dataGoiKham.FirstOrDefault().MaNhanVien; // chưa khám nên k có mã yêu cầu tiếp nhận
                data.ChucVu = "";
                data.GhiChu = "";
                data.ViTriCongTac = dataGoiKham.Select(s=>s.TenDonViHoacBoPhan).FirstOrDefault();


                var gioiTinh = dataGoiKham.Select(s => s.GioiTinh).FirstOrDefault();
                var coMangThai = dataGoiKham.Select(s => s.CoMangThai).FirstOrDefault();
                var daLapGiaDinh = dataGoiKham.Select(s => s.DaLapGiaDinh).FirstOrDefault();
                int? tuoi = dataGoiKham.Select(s => s.NamSinh).FirstOrDefault() == null ? null : (DateTime.Now.Year - dataGoiKham.Select(s => s.NamSinh).FirstOrDefault());
                if (dataGoiKham.Where(s => s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs != null).Any())
                {
                    // id == 1 DVKB id== 2 DVKT

                    listDichVu.AddRange(_goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                         .Where(x => x.GoiKhamSucKhoeId == dataGoiKham.First().GoiKhamSucKhoeId
                           && ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                           && ((!x.CoMangThai && !x.KhongMangThai) || gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && coMangThai) || (x.KhongMangThai && !coMangThai))
                           && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !daLapGiaDinh) || (x.DaLapGiaDinh && daLapGiaDinh))
                           && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (tuoi != null && ((x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen)))))
                         .OrderBy(x => x.Id)
                     .Select(k => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                     {
                         Id = 1,
                         TenDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                         NoiDung = k.ChuyenKhoaKhamSucKhoe.GetDescription(),
                         KhoaPhongThucHien = k.GoiKhamSucKhoeNoiThucHiens.Any() ? k.GoiKhamSucKhoeNoiThucHiens.Select(d => d.PhongBenhVien.Ten).FirstOrDefault() : "",
                         NgayDV = "", // chưa có tạo yctn  => dịch vụ gói chưa tạo  => thời điểm thực hiện null
                         GhiChu = "", // chưa bt 
                     }));
                }
                if (dataGoiKham.Where(s => s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats != null).Any())
                {
                    listDichVu.AddRange(_goiKhamSucKhoeDichVuDichVuKyThuatRepository.TableNoTracking
              .Where(x => x.GoiKhamSucKhoeId == dataGoiKham.First().GoiKhamSucKhoeId
                          && ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                          && ((!x.CoMangThai && !x.KhongMangThai) || gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && coMangThai) || (x.KhongMangThai && !coMangThai))
                          && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !daLapGiaDinh) || (x.DaLapGiaDinh && daLapGiaDinh))
                          && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (tuoi != null && ((x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen)))))
              .OrderBy(x => x.Id)
                .Select(k => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                {
                    Id = 2,
                    DichVuKyThuatId = k.DichVuKyThuatBenhVienId,
                    NoiDung = k.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                    TenDichVu = k.DichVuKyThuatBenhVien.Ten,
                    KhoaPhongThucHien = k.GoiKhamSucKhoeNoiThucHiens.Any() ? k.GoiKhamSucKhoeNoiThucHiens.Select(d => d.PhongBenhVien.Ten).FirstOrDefault() : "",
                    NgayDV = "",
                    GhiChu = "", // chưa bt 
                }));
                   
                }
                // xử lý gói khám chung
                var listDichVuDangKyGois= _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository.TableNoTracking
                    .Where(o => o.HopDongKhamSucKhoeNhanVienId == phieuInNhanVienKhamSucKhoeInfoVo.Id)
                    .OrderBy(x => x.Id)
                                                .Select(k => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan()
                                                {
                                                    Id = 1,
                                                    TenDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                                                    NoiDung = k.GoiKhamSucKhoeDichVuKhamBenh.ChuyenKhoaKhamSucKhoe.GetDescription(),
                                                    KhoaPhongThucHien = k.GoiKhamSucKhoeDichVuKhamBenh.GoiKhamSucKhoeNoiThucHiens.Any() ? k.GoiKhamSucKhoeDichVuKhamBenh.GoiKhamSucKhoeNoiThucHiens.Select(d => d.PhongBenhVien.Ten).FirstOrDefault() : "",
                                                    NgayDV = "", // chưa có tạo yctn  => dịch vụ gói chưa tạo  => thời điểm thực hiện null
                                                    GhiChu = "", // chưa bt 
                                                })
                                                .Union(_goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository.TableNoTracking
                    .Where(o => o.HopDongKhamSucKhoeNhanVienId == phieuInNhanVienKhamSucKhoeInfoVo.Id)
                    .OrderBy(x => x.Id)
                                                .Select(k => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan()
                                                {
                                                    Id = 2,
                                                    DichVuKyThuatId = k.DichVuKyThuatBenhVienId,
                                                    NoiDung = k.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                                                    TenDichVu = k.DichVuKyThuatBenhVien.Ten,
                                                    KhoaPhongThucHien = k.GoiKhamSucKhoeDichVuDichVuKyThuat.GoiKhamSucKhoeNoiThucHiens.Any() ? k.GoiKhamSucKhoeDichVuDichVuKyThuat.GoiKhamSucKhoeNoiThucHiens.Select(d => d.PhongBenhVien.Ten).FirstOrDefault() : "",
                                                    NgayDV = "",
                                                    GhiChu = "", // chưa bt 
                                                })).ToList();


                listDichVu.AddRange(listDichVuDangKyGois);
                //if (dataGoiKham.Where(d => d.GoiKhamSucKhoe.GoiChung == true).Any())
                //{
                //    listDichVu.AddRange(
                //}
                // dịch vụ khám bệnh
                int stt = 1;
                if (listDichVu.Any(s => s.Id == 1))
                {
                    columTable += "<tr>" +
                                 "<td colspan='5'  class='border-table'><strong>" + "KHÁM BỆNH" + "</strong></td>" +
                                 "</tr>";
                    foreach (var item in listDichVu.Where(s => s.Id == 1).ToList())
                    {
                        columTable += "<tr>" +
                                      "<td  class='border-table'>" + stt + "</td>" +
                                      "<td class='border-table'>" + item.NoiDung + "</td>" +
                                      "<td class='border-table'>" + item.NgayDV + "</td>" +
                                      "<td class='border-table'>" + item.KhoaPhongThucHien + "</td>" +
                                      "<td class='border-table'>" + item.GhiChu + "</td>" +
                                      "</tr>";
                        stt++;
                    }
                }
                
                foreach (var item in listDichVu.Where(z => z.Id == 2).GroupBy(s => s.NoiDung).ToList())
                {
                    columTable += "<tr>" +
                              "<td colspan='5' class='border-table'><strong>" + item.FirstOrDefault().NoiDung + "</strong></td>" +
                              "</tr>";
                    if (item.Count() > 1)
                    {
                        foreach (var itemgroup in item)
                        {
                            foreach (var itemx in listDichVu.Where(z => z.Id == 2).ToList())
                            {
                                if (itemgroup.DichVuKyThuatId == itemx.DichVuKyThuatId)
                                {
                                    columTable += "<tr>" +
                                        "<td class='border-table'>" + stt + "</td>" +
                                        "<td class='border-table'>" + itemx.TenDichVu + "</td>" +
                                        "<td class='border-table'>" + itemx.NgayDV + "</td>" +
                                        "<td class='border-table'>" + itemx.KhoaPhongThucHien + "</td>" +
                                        "<td class='border-table'>" + itemx.GhiChu + "</td>" +
                                        "</tr>";
                                    stt++;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemx in listDichVu.Where(z => z.Id == 2).ToList())
                        {
                            if (item.FirstOrDefault().DichVuKyThuatId == itemx.DichVuKyThuatId)
                            {
                                columTable += "<tr>" +
                                    "<td class='border-table'>" + stt + "</td>" +
                                    "<td class='border-table'>" + itemx.TenDichVu + "</td>" +
                                    "<td class='border-table'>" + itemx.NgayDV + "</td>" +
                                    "<td class='border-table'>" + itemx.KhoaPhongThucHien + "</td>" +
                                    "<td class='border-table'>" + itemx.GhiChu + "</td>" +
                                    "</tr>";
                                stt++;
                            }
                        }
                    }
                }
                data.columnTable = columTable;
                data.NhanVienTiepDon = "";
                data.Ngay = DateTime.Now.Day.ToString();
                data.Thang = DateTime.Now.Month.ToString();
                data.Nam = DateTime.Now.Year.ToString();
                data.NguoiDiKham = dataGoiKham.FirstOrDefault().HoTen;
                data.LogoUrl = phieuInNhanVienKhamSucKhoeInfoVo.HostingName + "/assets/img/logo-bacha-full.png";
                data.BarCodeImgBase64 = "";
                //data.BarCodeImgBase64 = !string.IsNullOrEmpty(thongTinNhanVienKham.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(thongTinNhanVienKham.MaYeuCauTiepNhan) : "";
                data.NgayGioSACH = DateTime.Now.ApplyFormatDateTimeSACH();
                data.MaTN = ""; // chưa có
            }

            //BVHD-3946
            data.GhiChuDV += "<br>Ghi chú:<br>";

            data.GhiChuDV += "1. Phụ nữ có thai hoặc nghi ngờ có thai không thực hiện chụp Xquang.<br>";

            data.GhiChuDV += "2. Siêu âm trước khi Khám Ngoại.<br>";

            data.GhiChuDV += "3. Khám Nội sau khi khám tất cả các chuyên khoa và làm xong các chỉ định cận lâm sàng(XQ, SÂ, ĐTĐ).<br>";

            data.GhiChuDV += "4. Nộp hồ sơ khám sức khoẻ có ký nhận tại quầy lễ tân tầng 2 của Bệnh Viện.<br>";
            //BVHD-3946
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
        public async Task<string> XuLyInKetQuaKhamSucKhoeAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo)
        {
            var content = string.Empty;
            var hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                      "<th>PHIẾU KHÁM SỨC KHỎE</th>" +
                      "</p>";

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("KetQuaKhamSucKhoe"));
            var thongTinNhanVienKham =
               await BaseRepository.TableNoTracking
                   .Include(x => x.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                   .Include(x => x.YeuCauDichVuKyThuats)
                   .Include(x => x.KetQuaSinhHieus)
                   .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                   .Include(x => x.KSKNhanVienKetLuan)?.ThenInclude(y => y.User)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.BenhNhan)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HopDongKhamSucKhoe).ThenInclude(z => z.CongTyKhamSucKhoe)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                   .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                   .OrderByDescending(x => x.Id)
                   .FirstOrDefaultAsync(x => x.HopDongKhamSucKhoeNhanVienId == phieuInNhanVienKhamSucKhoeInfoVo.Id);
            var data = new KetQuaKhamSucKhoeVo();
            var tableKham = "";
            var tableKyThuat = "";
            List<DanhSachDichVuKhamGrid> listDichVu = new List<DanhSachDichVuKhamGrid>();
            List<DanhSachDichVuKyThuatGrid> listDichVuKT = new List<DanhSachDichVuKyThuatGrid>();
            if (thongTinNhanVienKham != null)
            {
                data.HOTEN = thongTinNhanVienKham.HoTen;
                data.GioiTinh = thongTinNhanVienKham.GioiTinh != null ? thongTinNhanVienKham.GioiTinh.Value.GetDescription() : "";
                data.NamSinh = thongTinNhanVienKham.NamSinh != null ? thongTinNhanVienKham.NamSinh.ToString() : "";
                data.MaKhachHang = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.MaNhanVien;
                data.KhachHangDoanhNghiep = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan;
                data.DONVI = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten;


                var chiSoSinhHieu = thongTinNhanVienKham.KetQuaSinhHieus
                  .Select(s => new {
                      Mach = s.NhipTim,
                      HuyetAp = s.HuyetApTamThu + "/" + s.HuyetApTamTruong,
                      CanNang = s.CanNang,
                      ChieuCao = s.ChieuCao,
                      BMI = s.Bmi
                  }).LastOrDefault();
                if (chiSoSinhHieu != null)
                {
                    data.DHST = "Mạch: " + chiSoSinhHieu.Mach + " lần/phút;" + " Huyết áp: " + chiSoSinhHieu.HuyetAp + " mmHg;" + " Cân nặng: " + chiSoSinhHieu.CanNang + " kg;" + " Chiều cao: " + chiSoSinhHieu.ChieuCao + " cm;" + " BMI: ";
                    //data.DHST = "Cân nặng:" + chiSoSinhHieu.CanNang + " Mạch: " + chiSoSinhHieu.Mach + " lần/Phút" + "; Huyết áp: " + chiSoSinhHieu.HuyetAp + " mmHg;" + " Chiều cao: " + chiSoSinhHieu.ChieuCao + " cm;";
                    if (chiSoSinhHieu.BMI != null)
                    {
                        var SoBMI = (double)chiSoSinhHieu.BMI;
                        data.DHST += SoBMI.MathRoundNumber(2);
                    }
                }

                var dataKetQua = GetDataKetQuaKSKDoanEdit(phieuInNhanVienKhamSucKhoeInfoVo.Id);
                if (dataKetQua.Result.Any(s => s.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh))
                {
                    tableKham += "<table id='dichVuKham' style='width: 100%; padding - left: 50px'>";
                    foreach (var itemDVK in dataKetQua.Result.Where(s => s.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh).ToList())
                    {
                        tableKham += "<tr>" +
                                     "<td width='40%'><b>" + " &nbsp; &nbsp; &nbsp; &nbsp;" + "*/." + itemDVK.TenDichVu + "</b></td>" +
                                     "<td>: &nbsp;" + itemDVK.KetQuaDichVu + "</b></td>" +
                                     "</tr>";
                    }
                    tableKham += "</table>";
                }
                var stt = 1;
                if (dataKetQua.Result.Any(s => s.NhomId == EnumNhomGoiDichVu.DichVuKyThuat))
                {
                    tableKyThuat += "<table id='dichVuKyThuat' style='width: 100 %;'>";
                    foreach (var itemDVKT in dataKetQua.Result.Where(s => s.NhomId == EnumNhomGoiDichVu.DichVuKyThuat).ToList())
                    {
                        tableKyThuat += "<tr>" +
                                     "<td width='50%'>" + stt + " " + itemDVKT.TenDichVu + "</td>" +
                                     "<td>" + ": &nbsp;" + "</td>" +
                                     "<td>" + itemDVKT.KetQuaDichVu + "</b></td>" +
                                     "</tr>";
                        stt++;
                    }
                    tableKyThuat += "</table>";
                }
                data.DanhSachDichVuKham = tableKham;
                data.DanhSachDichVuKyThuat = tableKyThuat;
                data.Ngay = DateTime.Now.Day.ToString();
                data.Thang = DateTime.Now.Month.ToString();
                data.Nam = DateTime.Now.Year.ToString();
                data.LogoUrl = phieuInNhanVienKhamSucKhoeInfoVo.HostingName + "/assets/img/logo-bacha-full.png";
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(thongTinNhanVienKham.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(thongTinNhanVienKham.MaYeuCauTiepNhan) : "";
                data.MaTN = thongTinNhanVienKham.MaYeuCauTiepNhan;
                if (!string.IsNullOrEmpty(thongTinNhanVienKham.KSKKetLuanData))
                {
                    List<DanhSachPhanLoaiCacBenhTatGrid> objsCompare = new List<DanhSachPhanLoaiCacBenhTatGrid>();
                    var jsonParse = JsonConvert.DeserializeObject<List<DanhSachPhanLoaiCacBenhTatGrid>>(thongTinNhanVienKham.KSKKetLuanData);
                    if (jsonParse.Any())
                    {
                        foreach (var itemSaveJson in jsonParse)
                        {
                            if (itemSaveJson.LoaiKetLuan == EnumTypeLoaiKetLuan.PhanLoai)
                            {
                                if (itemSaveJson.PhanLoaiIdCapNhat != 0)
                                {
                                    var listPhanLoaiSucKhoe = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
                                    var result = listPhanLoaiSucKhoe.Select(item => new LookupItemVo
                                    {
                                        DisplayName = item.GetDescription(),
                                        KeyId = Convert.ToInt32(item),
                                    });
                                    result = result.Where(o => o.KeyId == itemSaveJson.PhanLoaiIdCapNhat);
                                    if (result.Any())
                                    {
                                        data.PhanLoaiSucKhoe = result.Select(d => d.DisplayName).FirstOrDefault();
                                    }
                                }


                            }
                            if (itemSaveJson.LoaiKetLuan == EnumTypeLoaiKetLuan.DeNghi)
                            {
                                data.DeNghi = itemSaveJson.KetQua;
                            }
                            if (itemSaveJson.LoaiKetLuan == EnumTypeLoaiKetLuan.CacBenhTatNeuCo)
                            {
                                data.KetLuan = itemSaveJson.KetQua;
                            }
                        }

                    }
                }
                else
                {
                    data.KetLuan = thongTinNhanVienKham.KSKKetLuanCacBenhTat;
                    data.PhanLoaiSucKhoe = thongTinNhanVienKham.KSKKetLuanPhanLoaiSucKhoe;
                    data.DeNghi = thongTinNhanVienKham.KSKKetLuanGhiChu;
                }
                data.BacSiKetLuanHoSo = thongTinNhanVienKham.KSKNhanVienKetLuan?.User?.HoTen;
            }
            else
            {
                var dataGoiKham = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                    .Include(x => x.GoiKhamSucKhoe)
                                                                       .Include(x => x.HopDongKhamSucKhoe)
                                                                       .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                                                                       .Include(z => z.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                    .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc => cc.GoiKhamSucKhoeDichVuKhamBenhs)
                    .Include(c => c.GoiKhamSucKhoe).ThenInclude(cc => cc.GoiKhamSucKhoeDichVuDichVuKyThuats)
                    .Where(s => s.Id == phieuInNhanVienKhamSucKhoeInfoVo.Id);

                data.HOTEN = dataGoiKham.Select(s => s.HoTen).FirstOrDefault();
                data.GioiTinh = dataGoiKham.Select(s => s.GioiTinh).FirstOrDefault() != null ? dataGoiKham.Select(s => s.GioiTinh).FirstOrDefault().GetDescription() : "";
                data.NamSinh = dataGoiKham.FirstOrDefault().NamSinh != null ? dataGoiKham.FirstOrDefault().NamSinh.ToString() : "";
                data.MaKhachHang = dataGoiKham.Select(s=>s.MaNhanVien).FirstOrDefault();
                data.KhachHangDoanhNghiep = dataGoiKham.Select(s=>s.TenDonViHoacBoPhan).FirstOrDefault();
                data.DONVI = dataGoiKham.Select(s => s.HopDongKhamSucKhoe != null ? s.HopDongKhamSucKhoe.CongTyKhamSucKhoe != null ? s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten :"" : "").FirstOrDefault();
                data.DHST = "";

                var gioiTinh = dataGoiKham.Select(s => s.GioiTinh).FirstOrDefault();
                var coMangThai = dataGoiKham.Select(s => s.CoMangThai).FirstOrDefault();
                var daLapGiaDinh = dataGoiKham.Select(s => s.DaLapGiaDinh).FirstOrDefault();
                int? tuoi = dataGoiKham.Select(s => s.NamSinh).FirstOrDefault() == null ? null : (DateTime.Now.Year - dataGoiKham.Select(s => s.NamSinh).FirstOrDefault());
                if (dataGoiKham.Where(s => s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs != null).Any())
                {
                    // id == 1 DVKB id== 2 DVKT

                    listDichVu.AddRange(_goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
               .Where(x => x.GoiKhamSucKhoeId == dataGoiKham.First().GoiKhamSucKhoeId
                           && ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                           && ((!x.CoMangThai && !x.KhongMangThai) || gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && coMangThai) || (x.KhongMangThai && !coMangThai))
                           && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !daLapGiaDinh) || (x.DaLapGiaDinh && daLapGiaDinh))
                           && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (tuoi != null && ((x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen)))))
                 .Select(k => new DanhSachDichVuKhamGrid
                 {
                     Id = 1,
                     TenDichVu = k.DichVuKhamBenhBenhVien.Ten
                 }));
                    //listDichVu.AddRange(dataGoiKham.Where(s => s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.Any()
                    //                                           && s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.Any(x =>
                    //                                              ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                    //                                           && ((!x.CoMangThai && !x.KhongMangThai) || gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && coMangThai) || (x.KhongMangThai && !coMangThai))
                    //                                           && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !daLapGiaDinh) || (x.DaLapGiaDinh && daLapGiaDinh))
                    //                                           && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (tuoi != null && ((x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) || (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen))
                    //                                           )))).SelectMany(s=>s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs)
                    //                                           .Select(k => new DanhSachDichVuKhamGrid
                    //                                           {
                    //                                               Id = 1,
                    //                                               TenDichVu = k.DichVuKhamBenhBenhVien.Ten
                    //                                           }));
                }
                if (dataGoiKham.Where(s => s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats != null).Any())
                {
                    listDichVu.AddRange(_goiKhamSucKhoeDichVuDichVuKyThuatRepository.TableNoTracking
              .Where(x => x.GoiKhamSucKhoeId == dataGoiKham.First().GoiKhamSucKhoeId
                          && ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                          && ((!x.CoMangThai && !x.KhongMangThai) || gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && coMangThai) || (x.KhongMangThai && !coMangThai))
                          && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !daLapGiaDinh) || (x.DaLapGiaDinh && daLapGiaDinh))
                          && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (tuoi != null && ((x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen)))))
                .Select(k => new DanhSachDichVuKhamGrid
                {
                    Id = 2,
                    TenDichVu = k.DichVuKyThuatBenhVien.Ten
                }));
                    //listDichVu.AddRange(dataGoiKham.Where(s => s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.Any()
                    //                                           && s.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.Any(x =>
                    //                                              ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                    //                                           && ((!x.CoMangThai && !x.KhongMangThai) || gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && coMangThai) || (x.KhongMangThai && !coMangThai))
                    //                                           && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !daLapGiaDinh) || (x.DaLapGiaDinh && daLapGiaDinh))
                    //                                           && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (tuoi != null && ((x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) || (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen))
                    //                                           )))).SelectMany(p => p.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats)
                    //                                           .Select(k => new DanhSachDichVuKhamGrid
                    //                                           {
                    //                                               Id = 2,
                    //                                               TenDichVu = k.DichVuKyThuatBenhVien.Ten,
                    //                                           }));
                }



                if (listDichVu.Any())
                {
                    tableKham += "<table id='dichVuKham' style='width: 100%; padding - left: 50px'>";
                                   
                    foreach (var itemDVK in listDichVu)
                    {
                        tableKham += "<tr>" +
                                     "<td width='30%'><b>" + " &nbsp; &nbsp; &nbsp; &nbsp;" + "*/." + itemDVK.TenDichVu + "</b></td>" +
                                     "<td>: &nbsp;" + itemDVK.KetQuaDichVu + "</b></td>" +
                                     "</tr>";
                    }
                    tableKham += "<tbody>";
                }
                if (thongTinNhanVienKham.YeuCauDichVuKyThuats != null)
                {
                    tableKyThuat += "<table id='dichVuKyThuat' style='width: 100%;'>";
                    var sttDVKT = 1;
                    foreach (var itemDVKT in listDichVuKT)
                    {
                        tableKyThuat += "<tr>" +
                                     "<td width='50%'>" + sttDVKT + " " + itemDVKT.TenDichVu + "</td>" +
                                     "<td>" + ": &nbsp;" + "</td>" +
                                     "<td>" + itemDVKT.KetQuaDichVu + "</b></td>" +
                                     "</tr>";
                        sttDVKT++;
                    }
                    tableKyThuat += "</table>";
                }
                data.DanhSachDichVuKham = tableKham;
                data.DanhSachDichVuKyThuat = tableKyThuat;
                data.Ngay = DateTime.Now.Day.ToString();
                data.Thang = DateTime.Now.Month.ToString();
                data.Nam = DateTime.Now.Year.ToString();
                data.LogoUrl = phieuInNhanVienKhamSucKhoeInfoVo.HostingName + "/assets/img/logo-bacha-full.png";
                data.BarCodeImgBase64 = "";
                //data.BarCodeImgBase64 = !string.IsNullOrEmpty(thongTinNhanVienKham.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(thongTinNhanVienKham.MaYeuCauTiepNhan) : "";
                data.MaTN = ""; // chưa khám chưa có mã tn 
                data.KetLuan = "";
                
            }
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);// + "<div class='pagebreak'></div>";
            return content;
        }
        public async Task<string> XuLyInKetQuaKhamSucKhoeKetLuanAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo)
        {
            var content = string.Empty;
            var hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                      "<th>PHIẾU KHÁM SỨC KHỎE</th>" +
                      "</p>";

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("KetQuaKhamSucKhoe"));
            //var thongTinNhanVienKham =
            //   BaseRepository.TableNoTracking
            //       .Include(x => x.KetQuaSinhHieus)
            //       .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
            //       .Include(x=>x.KSKNhanVienKetLuan)?.ThenInclude(y=>y.User)
            //       .Include(x => x.HopDongKhamSucKhoeNhanVien)
            //       .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HopDongKhamSucKhoe).ThenInclude(z => z.CongTyKhamSucKhoe)
            //       .OrderByDescending(x => x.Id)
            //       .FirstOrDefault(x => x.HopDongKhamSucKhoeNhanVienId == phieuInNhanVienKhamSucKhoeInfoVo.Id);


            var thongTinNV = _yeuCauTiepNhanRepository.TableNoTracking.Where(x => x.HopDongKhamSucKhoeNhanVienId == phieuInNhanVienKhamSucKhoeInfoVo.Id && x.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .Select(c=> new {
                    KetQuaSinhHieuId = c.KetQuaSinhHieus.Select(d=>d.Id).ToList(),
                    HOTEN = c.HoTen,
                    GioiTinh = c.GioiTinh != null ? c.GioiTinh.GetValueOrDefault().GetDescription():"",
                    NamSinh= c.NamSinh,
                    MaKhachHang = c.MaYeuCauTiepNhan,
                    HopDongKhamSucKhoeNhanVienId = c.HopDongKhamSucKhoeNhanVienId,
                    KSKNhanVienKetLuanId = c.KSKNhanVienKetLuanId,
                    KSKKetLuanData = c.KSKKetLuanData,
                    KSKKetLuanCacBenhTat = c.KSKKetLuanCacBenhTat,
                    KSKKetLuanPhanLoaiSucKhoe = c.KSKKetLuanPhanLoaiSucKhoe,
                    KSKKetLuanGhiChu = c.KSKKetLuanGhiChu
                })
                .FirstOrDefault();

            // kết quả sinh hiệu 
            var dHST = string.Empty;
            if (thongTinNV.KetQuaSinhHieuId != null && thongTinNV.KetQuaSinhHieuId.Any())
            {
                var ketQuaSinhHieu = _ketQuaSinhHieuRepository.TableNoTracking
                    .Where(d => thongTinNV.KetQuaSinhHieuId.Contains(d.Id))
                    .Select(s => new
                    {
                        Mach = s.NhipTim,
                        HuyetAp = s.HuyetApTamThu + "/" + s.HuyetApTamTruong,
                        CanNang = s.CanNang,
                        ChieuCao = s.ChieuCao,
                        BMI = s.Bmi
                    }).LastOrDefault();
                if (ketQuaSinhHieu != null)
                {
                    dHST = "Mạch: " + ketQuaSinhHieu.Mach + " lần/phút;" + " Huyết áp: " + ketQuaSinhHieu.HuyetAp + " mmHg;" + " Cân nặng: " + ketQuaSinhHieu.CanNang + " kg;" + " Chiều cao: " + ketQuaSinhHieu.ChieuCao + " cm;" + " BMI: ";
                    //data.DHST = "Cân nặng:" + chiSoSinhHieu.CanNang + " Mạch: " + chiSoSinhHieu.Mach + " lần/Phút" + "; Huyết áp: " + chiSoSinhHieu.HuyetAp + " mmHg;" + " Chiều cao: " + chiSoSinhHieu.ChieuCao + " cm;";
                    if (ketQuaSinhHieu.BMI != null)
                    {
                        var SoBMI = (double)ketQuaSinhHieu.BMI;
                        dHST += SoBMI.MathRoundNumber(2);
                    }
                }
            }
            // nhân viên kết luận
            var kSKNhanVienKetLuan = string.Empty;
            if (thongTinNV.KSKNhanVienKetLuanId != null && thongTinNV.KSKNhanVienKetLuanId != 0)
            {
                var tenNhanVien = _userRepository.TableNoTracking
                    .Where(d => d.Id == thongTinNV.KSKNhanVienKetLuanId).Select(o => o.HoTen).FirstOrDefault();
                    
                if (!string.IsNullOrEmpty(tenNhanVien))
                {
                    kSKNhanVienKetLuan = tenNhanVien;
                }
            }

            //khách hàng doanh nghiệp vaf don vi
            var khachHangDoanhNghiep = string.Empty;
            var donVi = string.Empty;
            if (thongTinNV.HopDongKhamSucKhoeNhanVienId != null)
            {
                var infoHopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                                                     .Where(d => d.Id == thongTinNV.HopDongKhamSucKhoeNhanVienId)
                                                     .Select(e => new { 
                                                         e.TenDonViHoacBoPhan,e.HopDongKhamSucKhoeId
                                                     }).FirstOrDefault();

                khachHangDoanhNghiep = infoHopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan;

                if(infoHopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId != null){

                    var congtyId = _hopDongKhamSucKhoeRepository.TableNoTracking.Where(d => d.Id == infoHopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId)
                        .Select(d => d.CongTyKhamSucKhoeId).FirstOrDefault();

                    donVi = _congTyKhamSucKhoeRepository.TableNoTracking.Where(d => d.Id == congtyId).Select(d => d.Ten).FirstOrDefault();
                }
            }

            var data = new KetQuaKhamSucKhoeVo();
            var tableKham = "";
            var tableKyThuat = "";
            List<DanhSachDichVuKhamGrid> listDichVu = new List<DanhSachDichVuKhamGrid>();
            List<DanhSachDichVuKyThuatGrid> listDichVuKT = new List<DanhSachDichVuKyThuatGrid>();
            if (thongTinNV != null)
            {
                data.HOTEN = thongTinNV.HOTEN;
                data.GioiTinh = thongTinNV.GioiTinh;
                data.NamSinh = thongTinNV.NamSinh + "";
                data.MaKhachHang = thongTinNV.MaKhachHang;
                data.KhachHangDoanhNghiep = khachHangDoanhNghiep;
                data.DONVI = donVi;

                data.DHST = dHST;


                var dataKetQua = await GetDataKetQuaKSKDoanEdit(phieuInNhanVienKhamSucKhoeInfoVo.Id);
                if (dataKetQua.Any(s=>s.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh))
                {
                    tableKham += "<table id='dichVuKham' style='width: 100%; '>";
                                   
                    foreach (var itemDVK in dataKetQua.Where(s => s.NhomId == EnumNhomGoiDichVu.DichVuKhamBenh).ToList())
                    {
                        tableKham += "<tr>" +
                                     "<td width='40%'><b>" + " &nbsp; &nbsp; &nbsp; &nbsp;" + "*/." + itemDVK.TenDichVu + "</b></td>" +
                                     "<td>: " + (itemDVK.KetQuaDichVu != null ? itemDVK.KetQuaDichVu.Replace("\n", "<br/>") : "") + "</b></td>" +
                                     "</tr>";
                    }
                    tableKham += "</table>";
                }
                var stt = 1;
                if (dataKetQua.Any(s => s.NhomId == EnumNhomGoiDichVu.DichVuKyThuat))
                {
                    tableKyThuat += "<table id='dichVuKyThuat' style='width: 100%;'>";

                    foreach (var itemDVKT in dataKetQua.Where(s => s.NhomId == EnumNhomGoiDichVu.DichVuKyThuat).ToList())
                    {
                        tableKyThuat += "<tr>" +
                                     "<td width='50%'>" + stt + ". " + itemDVKT.TenDichVu + "</td>" +
                                     "<td>" + ":" +"</td>" +
                                     "<td>" + (itemDVKT.KetQuaDichVu != null ? itemDVKT.KetQuaDichVu.Replace("\n", "<br/>") : "") + "</b></td>" +
                                     "</tr>";
                        stt++;
                    }
                    tableKyThuat += "</table>";
                }
                data.DanhSachDichVuKham = tableKham;
                data.DanhSachDichVuKyThuat = tableKyThuat;
                data.Ngay = DateTime.Now.Day.ToString();
                data.Thang = DateTime.Now.Month.ToString();
                data.Nam = DateTime.Now.Year.ToString();
                data.LogoUrl = phieuInNhanVienKhamSucKhoeInfoVo.HostingName + "/assets/img/logo-bacha-full.png";
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(thongTinNV.MaKhachHang) ? BarcodeHelper.GenerateBarCode(thongTinNV.MaKhachHang) : "";
                data.MaTN = thongTinNV.MaKhachHang;

                if (!string.IsNullOrEmpty(thongTinNV.KSKKetLuanData))
                {
                    List<DanhSachPhanLoaiCacBenhTatGrid> objsCompare = new List<DanhSachPhanLoaiCacBenhTatGrid>();
                    var jsonParse = JsonConvert.DeserializeObject<List<DanhSachPhanLoaiCacBenhTatGrid>>(thongTinNV.KSKKetLuanData);
                    if (jsonParse.Any())
                    {
                        foreach (var itemSaveJson in jsonParse)
                        {
                            if (itemSaveJson.LoaiKetLuan == EnumTypeLoaiKetLuan.PhanLoai)
                            {
                                if(itemSaveJson.PhanLoaiIdCapNhat != 0)
                                {
                                    var listPhanLoaiSucKhoe = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
                                    var result = listPhanLoaiSucKhoe.Select(item => new LookupItemVo
                                    {
                                        DisplayName = item.GetDescription(),
                                        KeyId = Convert.ToInt32(item),
                                    });
                                    result = result.Where(o => o.KeyId == itemSaveJson.PhanLoaiIdCapNhat);
                                    if (result.Any())
                                    {
                                        data.PhanLoaiSucKhoe = result.Select(d=>d.DisplayName).FirstOrDefault();
                                    }
                                }
                               

                            }
                            if (itemSaveJson.LoaiKetLuan == EnumTypeLoaiKetLuan.DeNghi)
                            {
                                data.DeNghi = itemSaveJson.KetQua;
                            }
                            if (itemSaveJson.LoaiKetLuan == EnumTypeLoaiKetLuan.CacBenhTatNeuCo)
                            {
                                data.KetLuan = itemSaveJson.KetQua;
                            }
                        }

                    }
                }
                else
                {
                    data.KetLuan = thongTinNV.KSKKetLuanCacBenhTat;
                    data.PhanLoaiSucKhoe = thongTinNV.KSKKetLuanPhanLoaiSucKhoe;
                    data.DeNghi = thongTinNV.KSKKetLuanGhiChu;
                }
               
                data.BacSiKetLuanHoSo = kSKNhanVienKetLuan;
            }
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);// + "<div class='pagebreak'></div>";
            return content;
        }
        #endregion

        #region //BVHD-3929
        public async Task<List<string>> XuLyInNhieuPhieuDangKyKhamSucKhoeAsync(PhieuInDangKyKSKVo phieuInDangKyKSKVo)
        {
            var lstHtml = new List<string>();
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuDangKyKhamSucKhoeKhamDoan"));

            var thongTinNhanVienKhams =
                await BaseRepository.TableNoTracking
                    .Where(x => x.HopDongKhamSucKhoeNhanVienId != null 
                                && phieuInDangKyKSKVo.Ids.Contains(x.HopDongKhamSucKhoeNhanVienId.Value))
                    .Select(x => new PhieuDangKyKhamSucKhoeVo()
                    {
                        YeuCauTiepNhanId = x.Id,
                        HopDongKhamSucKhoeNhanVienId = x.HopDongKhamSucKhoeNhanVienId,
                        DonViKham = x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                        HoTen = x.HoTen,
                        GioiTinh = x.GioiTinh.GetDescription(),
                        NamSinh = x.NamSinh != null ? x.NamSinh.ToString() : "",
                        MaNhanVien = x.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                        ViTriCongTac = x.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan,
                        NhanVienTiepDon = x.NhanVienTiepNhan.User.HoTen,
                        MaTN = x.MaYeuCauTiepNhan
                    })
                    .ToListAsync();

            var lstYeuCauTiepNhanId = thongTinNhanVienKhams.Select(x => x.YeuCauTiepNhanId).ToList();
            var lstYeuCauKhamBenh = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && lstYeuCauTiepNhanId.Contains(p.YeuCauTiepNhanId))
                .Select(s => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                {
                    Id = s.YeuCauTiepNhanId,
                    TenDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                    NoiDung = s.TenDichVu,
                    KhoaPhongThucHien = s.NoiThucHien == null ? s.NoiDangKy.Ten : s.NoiThucHien.Ten,
                    NgayDV = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : "",
                    GhiChu = "", // chưa bt 
                })
                .ToList();

            var lstYeuCauKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                            && p.GoiKhamSucKhoeId != null
                            && lstYeuCauTiepNhanId.Contains(p.YeuCauTiepNhanId))
                .Select(s => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                {
                    Id = s.YeuCauTiepNhanId,
                    DichVuKyThuatId = s.Id,
                    NoiDung = s.NhomDichVuBenhVien.Ten,
                    TenDichVu = s.TenDichVu,
                    KhoaPhongThucHien = s.NoiThucHien.Ten,
                    NgayDV = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : "",
                    GhiChu = "", // chưa bt 
                })
                .ToList();

            var lstPhieuDangKyKSKIdChuaBatDauKham = phieuInDangKyKSKVo.Ids
                .Where(x => !thongTinNhanVienKhams.Where(a => a.HopDongKhamSucKhoeNhanVienId != null).Select(a => a.HopDongKhamSucKhoeNhanVienId).Contains(x)).ToList();
            var thongTinNhanVienKhamChuaBatDauKhams = new List<PhieuDangKyKhamSucKhoeVo>();
            var lstDichVuKhamChuaBatDauKham = new List<GoiKhamSucKhoeDichVuKhamBenh>();
            var lstDichVuKyThuatChuaBatDauKham = new List<GoiKhamSucKhoeDichVuDichVuKyThuat>();
            var lstDichVuKhamGoiChungChuaBatDauKham = new List<DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan>();
            var lstDichVuKyThuatGoiChungChuaBatDauKham = new List<DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan>();
            if (lstPhieuDangKyKSKIdChuaBatDauKham.Any())
            {
                thongTinNhanVienKhamChuaBatDauKhams = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                    .Where(x => lstPhieuDangKyKSKIdChuaBatDauKham.Contains(x.Id))
                    .Select(x => new PhieuDangKyKhamSucKhoeVo()
                    {
                        HopDongKhamSucKhoeNhanVienId = x.Id,
                        DonViKham = x.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                        HoTen = x.HoTen,
                        GioiTinh = x.GioiTinh.GetDescription(),
                        NamSinh = x.NamSinh != null ? x.NamSinh.Value.ToString() : null,
                        MaNhanVien = x.MaNhanVien,
                        ChucVu = "",
                        GhiChu = "",
                        ViTriCongTac = x.TenDonViHoacBoPhan,

                        LoaiGioiTinh = x.GioiTinh,
                        CoMangThai = x.CoMangThai,
                        DaLapGiaDinh = x.DaLapGiaDinh,
                        Tuoi = x.NamSinh == null ? (int?)null : (DateTime.Now.Year - x.NamSinh.GetValueOrDefault()),

                        GoiKhamSucKhoeId = x.GoiKhamSucKhoeId
                    }).ToList();

                var lstGoiKhamSucKhoeId = thongTinNhanVienKhamChuaBatDauKhams.Where(x => x.GoiKhamSucKhoeId != null).Select(x => x.GoiKhamSucKhoeId).Distinct().ToList();

                lstDichVuKhamChuaBatDauKham = _goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                                .Include(x => x.GoiKhamSucKhoeNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
                                .Where(x => lstGoiKhamSucKhoeId.Contains(x.GoiKhamSucKhoeId))
                                .OrderBy(x => x.Id)
                                .ToList();

                lstDichVuKyThuatChuaBatDauKham = _goiKhamSucKhoeDichVuDichVuKyThuatRepository.TableNoTracking
                                .Include(x => x.DichVuKyThuatBenhVien).ThenInclude(x => x.NhomDichVuBenhVien)
                                .Include(x => x.GoiKhamSucKhoeNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
                                .Where(x => lstGoiKhamSucKhoeId.Contains(x.GoiKhamSucKhoeId))
                                .OrderBy(x => x.Id)
                                .ToList();

                lstDichVuKhamGoiChungChuaBatDauKham = _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository.TableNoTracking
                    .Where(o => lstPhieuDangKyKSKIdChuaBatDauKham.Contains(o.HopDongKhamSucKhoeNhanVienId))
                    .OrderBy(x => x.Id)
                    .Select(k => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan()
                    {
                        Id = 1,
                        TenDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                        NoiDung = k.GoiKhamSucKhoeDichVuKhamBenh.ChuyenKhoaKhamSucKhoe.GetDescription(),
                        //KhoaPhongThucHien = k.GoiKhamSucKhoeDichVuKhamBenh.GoiKhamSucKhoeNoiThucHiens.Any()
                        //    ? k.GoiKhamSucKhoeDichVuKhamBenh.GoiKhamSucKhoeNoiThucHiens
                        //        .Select(d => d.PhongBenhVien.Ten).FirstOrDefault()
                        //    : "",
                        NgayDV = "", // chưa có tạo yctn  => dịch vụ gói chưa tạo  => thời điểm thực hiện null
                        GhiChu = "", // chưa bt 

                        HopDongKhamSucKhoeNhanVienId = k.HopDongKhamSucKhoeNhanVienId,
                        TenKhoaPhongThucHiens = k.GoiKhamSucKhoeDichVuKhamBenh.GoiKhamSucKhoeNoiThucHiens.Select(a => a.PhongBenhVien.Ten).ToList()
                    }).ToList();

                lstDichVuKyThuatGoiChungChuaBatDauKham = _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository.TableNoTracking
                    .Where(o => lstPhieuDangKyKSKIdChuaBatDauKham.Contains(o.HopDongKhamSucKhoeNhanVienId))
                    .OrderBy(x => x.Id)
                    .Select(k => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan()
                    {
                        Id = 2,
                        DichVuKyThuatId = k.DichVuKyThuatBenhVienId,
                        NoiDung = k.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                        TenDichVu = k.DichVuKyThuatBenhVien.Ten,
                        //KhoaPhongThucHien =
                        //    k.GoiKhamSucKhoeDichVuDichVuKyThuat.GoiKhamSucKhoeNoiThucHiens.Any()
                        //        ? k.GoiKhamSucKhoeDichVuDichVuKyThuat.GoiKhamSucKhoeNoiThucHiens
                        //            .Select(d => d.PhongBenhVien.Ten).FirstOrDefault()
                        //        : "",
                        NgayDV = "",
                        GhiChu = "", // chưa bt 

                        HopDongKhamSucKhoeNhanVienId = k.HopDongKhamSucKhoeNhanVienId,
                        TenKhoaPhongThucHiens = k.GoiKhamSucKhoeDichVuDichVuKyThuat.GoiKhamSucKhoeNoiThucHiens.Select(a => a.PhongBenhVien.Ten).ToList()
                    }).ToList();
            }

            foreach (var idPhieuIn in phieuInDangKyKSKVo.Ids)
            {
                var thongTinNhanVienKham = thongTinNhanVienKhams.FirstOrDefault(x => x.HopDongKhamSucKhoeNhanVienId == idPhieuIn);

                var columTable = "";
                var data = new PhieuDangKyKhamSucKhoeVo();
                List<DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan> listDichVu = new List<DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan>();
                if (thongTinNhanVienKham != null)
                {
                    data.DonViKham = thongTinNhanVienKham.DonViKham;
                    data.HoTen = thongTinNhanVienKham.HoTen;
                    data.GioiTinh = thongTinNhanVienKham.GioiTinh;
                    data.NamSinh = thongTinNhanVienKham.NamSinh;
                    data.MaNhanVien = thongTinNhanVienKham.MaNhanVien;
                    data.ChucVu = "";
                    data.GhiChu = "";
                    data.ViTriCongTac = thongTinNhanVienKham.ViTriCongTac;

                    var yeuCauKhamTheoTiepNhans = lstYeuCauKhamBenh.Where(x => x.Id == thongTinNhanVienKham.YeuCauTiepNhanId).ToList();
                    if (yeuCauKhamTheoTiepNhans.Any())
                    {
                        // id == 1 DVKB id== 2 DVKT
                        listDichVu.AddRange(yeuCauKhamTheoTiepNhans.Select(s => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                        {
                            Id = 1,
                            TenDichVu = s.TenDichVu,
                            NoiDung = s.NoiDung,
                            KhoaPhongThucHien = s.KhoaPhongThucHien,
                            NgayDV = s.NgayDV,
                            GhiChu = s.GhiChu
                        }));
                    }

                    var yeuCauKyThuatTheoTiepNhans = lstYeuCauKyThuat.Where(x => x.Id == thongTinNhanVienKham.YeuCauTiepNhanId).ToList();
                    if (yeuCauKyThuatTheoTiepNhans.Any())
                    {
                        listDichVu.AddRange(yeuCauKyThuatTheoTiepNhans.Select(s => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                        {
                            Id = 2,
                            DichVuKyThuatId = s.DichVuKyThuatId,
                            NoiDung = s.NoiDung,
                            TenDichVu = s.TenDichVu,
                            KhoaPhongThucHien = s.KhoaPhongThucHien,
                            NgayDV = s.NgayDV,
                            GhiChu = s.GhiChu
                        }));
                    }
                    // dịch vụ khám bệnh
                    columTable += "<tr>" +
                                     "<td colspan='5'  class='border-table'><strong>" + "KHÁM BỆNH" + "</strong></td>" +
                                     "</tr>";
                    int stt = 1;
                    foreach (var item in listDichVu.Where(s => s.Id == 1).ToList())
                    {
                        columTable += "<tr>" +
                                      "<td  class='border-table'>" + stt + "</td>" +
                                      "<td class='border-table'>" + item.NoiDung + "</td>" +
                                      "<td class='border-table'>" + item.NgayDV + "</td>" +
                                      "<td class='border-table'>" + item.KhoaPhongThucHien + "</td>" +
                                      "<td class='border-table'>" + item.GhiChu + "</td>" +
                                      "</tr>";
                        stt++;
                    }
                    foreach (var item in listDichVu.Where(z => z.Id == 2).GroupBy(s => s.NoiDung).ToList())
                    {
                        columTable += "<tr>" +
                                  "<td colspan='5' class='border-table'><strong>" + item.FirstOrDefault().NoiDung + "</strong></td>" +
                                  "</tr>";
                        var listDVKT = item.Select(d => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan()
                        {
                            Id = d.Id,
                            DichVuKyThuatId = d.DichVuKyThuatId,
                            NoiDung = d.NoiDung,
                            TenDichVu = d.TenDichVu,
                            KhoaPhongThucHien = d.KhoaPhongThucHien,
                            NgayDV = d.NgayDV,
                            GhiChu = d.GhiChu,
                        });
                        foreach (var itemx in listDVKT)
                        {
                            var dichVu = listDichVu.Where(z => z.Id == 2 && z.DichVuKyThuatId == itemx.DichVuKyThuatId);
                            if (dichVu.Any())
                            {
                                columTable += "<tr>" +
                                    "<td class='border-table'>" + stt + "</td>" +
                                    "<td class='border-table'>" + itemx.TenDichVu + "</td>" +
                                    "<td class='border-table'>" + itemx.NgayDV + "</td>" +
                                    "<td class='border-table'>" + itemx.KhoaPhongThucHien + "</td>" +
                                    "<td class='border-table'>" + itemx.GhiChu + "</td>" +
                                    "</tr>";
                                stt++;
                            }
                        }
                    }
                    data.columnTable = columTable;
                    data.NhanVienTiepDon = thongTinNhanVienKham.NhanVienTiepDon;
                    data.Ngay = DateTime.Now.Day.ToString();
                    data.Thang = DateTime.Now.Month.ToString();
                    data.Nam = DateTime.Now.Year.ToString();
                    data.NguoiDiKham = thongTinNhanVienKham.HoTen;
                    data.LogoUrl = phieuInDangKyKSKVo.HostingName + "/assets/img/logo-bacha-full.png";
                    data.BarCodeImgBase64 = !string.IsNullOrEmpty(thongTinNhanVienKham.MaTN) ? BarcodeHelper.GenerateBarCode(thongTinNhanVienKham.MaTN) : "";
                    data.NgayGioSACH = DateTime.Now.ApplyFormatDateTimeSACH();
                    data.MaTN = thongTinNhanVienKham.MaTN;
                }
                else
                {
                    var dataGoiKham = thongTinNhanVienKhamChuaBatDauKhams
                        .FirstOrDefault(x => x.HopDongKhamSucKhoeNhanVienId == idPhieuIn);
                    if (dataGoiKham != null)
                    {

                        data.DonViKham = dataGoiKham.DonViKham;
                        data.HoTen = dataGoiKham.HoTen;
                        data.GioiTinh = dataGoiKham.GioiTinh;
                        data.NamSinh = dataGoiKham.NamSinh;
                        data.MaNhanVien = dataGoiKham.MaNhanVien;
                        data.ChucVu = "";
                        data.GhiChu = "";
                        data.ViTriCongTac = dataGoiKham.ViTriCongTac;


                        var gioiTinh = dataGoiKham.LoaiGioiTinh;
                        var coMangThai = dataGoiKham.CoMangThai;
                        var daLapGiaDinh = dataGoiKham.DaLapGiaDinh;
                        int? tuoi = dataGoiKham.Tuoi;

                        var lstDichVuKhamTrongGoi = lstDichVuKhamChuaBatDauKham.Where(x => x.GoiKhamSucKhoeId == dataGoiKham.GoiKhamSucKhoeId).ToList();
                        if (lstDichVuKhamTrongGoi.Any())
                        {
                            // id == 1 DVKB id== 2 DVKT
                            listDichVu.AddRange(lstDichVuKhamTrongGoi
                                .Where(x => ((!x.GioiTinhNam && !x.GioiTinhNu)
                                                || (x.GioiTinhNam && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam)
                                                || (x.GioiTinhNu && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                            && ((!x.CoMangThai && !x.KhongMangThai)
                                                || gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam
                                                || (x.CoMangThai && coMangThai)
                                                || (x.KhongMangThai && !coMangThai))
                                            && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh)
                                                || (x.ChuaLapGiaDinh && !daLapGiaDinh)
                                                || (x.DaLapGiaDinh && daLapGiaDinh))
                                            && (
                                                (x.SoTuoiTu == null && x.SoTuoiDen == null) 
                                                || (tuoi != null 
                                                    && (
                                                        (x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) 
                                                        && (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen)
                                                        )
                                                    )
                                                )
                                            )
                                .Select(k => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                                {
                                    Id = 1,
                                    TenDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                                    NoiDung = k.ChuyenKhoaKhamSucKhoe.GetDescription(),
                                    KhoaPhongThucHien = k.GoiKhamSucKhoeNoiThucHiens.Any()
                                        ? k.GoiKhamSucKhoeNoiThucHiens.Select(d => d.PhongBenhVien.Ten).FirstOrDefault()
                                        : "",
                                    NgayDV = "", // chưa có tạo yctn  => dịch vụ gói chưa tạo  => thời điểm thực hiện null
                                    GhiChu = "", // chưa bt 
                                }));
                        }

                        var lstDichVuKyThuatTrongGoi = lstDichVuKyThuatChuaBatDauKham.Where(x => x.GoiKhamSucKhoeId == dataGoiKham.GoiKhamSucKhoeId).ToList();
                        if (lstDichVuKyThuatTrongGoi.Any())
                        {
                            listDichVu.AddRange(lstDichVuKyThuatTrongGoi
                                .Where(x => ((!x.GioiTinhNam && !x.GioiTinhNu) 
                                                || (x.GioiTinhNam && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) 
                                                || (x.GioiTinhNu && gioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                            && ((!x.CoMangThai && !x.KhongMangThai) 
                                                || gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam 
                                                || (x.CoMangThai && coMangThai) 
                                                || (x.KhongMangThai && !coMangThai))
                                            && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) 
                                                || (x.ChuaLapGiaDinh && !daLapGiaDinh) 
                                                || (x.DaLapGiaDinh && daLapGiaDinh))
                                            && (
                                                (x.SoTuoiTu == null && x.SoTuoiDen == null)
                                                || (tuoi != null
                                                    && (
                                                        (x.SoTuoiTu == null || tuoi >= x.SoTuoiTu)
                                                        && (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen)
                                                    )
                                                )
                                            )
                                )
                                .Select(k => new DichVuKhamVaDichVuKyThuatKhamSucKhoeDoan
                                {
                                    Id = 2,
                                    DichVuKyThuatId = k.DichVuKyThuatBenhVienId,
                                    NoiDung = k.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                                    TenDichVu = k.DichVuKyThuatBenhVien.Ten,
                                    KhoaPhongThucHien = k.GoiKhamSucKhoeNoiThucHiens.Any()
                                        ? k.GoiKhamSucKhoeNoiThucHiens.Select(d => d.PhongBenhVien.Ten).FirstOrDefault()
                                        : "",
                                    NgayDV = "",
                                    GhiChu = "", // chưa bt 
                                }));

                        }

                        // xử lý gói chung
                        var listDichVuDangKyGois = lstDichVuKhamGoiChungChuaBatDauKham.Where(x => x.HopDongKhamSucKhoeNhanVienId == idPhieuIn)
                            .Concat(lstDichVuKyThuatGoiChungChuaBatDauKham.Where(x => x.HopDongKhamSucKhoeNhanVienId == idPhieuIn))
                            .ToList();

                        listDichVuDangKyGois.ForEach(x => x.KhoaPhongThucHien = x.TenKhoaPhongThucHiens.Any() ? x.TenKhoaPhongThucHiens.First() : string.Empty);

                        listDichVu.AddRange(listDichVuDangKyGois);

                        // dịch vụ khám bệnh
                        int stt = 1;
                        if (listDichVu.Any(s => s.Id == 1))
                        {
                            columTable += "<tr>" +
                                          "<td colspan='5'  class='border-table'><strong>" + "KHÁM BỆNH" +
                                          "</strong></td>" +
                                          "</tr>";
                            foreach (var item in listDichVu.Where(s => s.Id == 1).ToList())
                            {
                                columTable += "<tr>" +
                                              "<td  class='border-table'>" + stt + "</td>" +
                                              "<td class='border-table'>" + item.NoiDung + "</td>" +
                                              "<td class='border-table'>" + item.NgayDV + "</td>" +
                                              "<td class='border-table'>" + item.KhoaPhongThucHien + "</td>" +
                                              "<td class='border-table'>" + item.GhiChu + "</td>" +
                                              "</tr>";
                                stt++;
                            }
                        }

                        foreach (var item in listDichVu.Where(z => z.Id == 2).GroupBy(s => s.NoiDung).ToList())
                        {
                            columTable += "<tr>" +
                                          "<td colspan='5' class='border-table'><strong>" +
                                          item.FirstOrDefault().NoiDung + "</strong></td>" +
                                          "</tr>";
                            if (item.Count() > 1)
                            {
                                foreach (var itemgroup in item)
                                {
                                    foreach (var itemx in listDichVu.Where(z => z.Id == 2).ToList())
                                    {
                                        if (itemgroup.DichVuKyThuatId == itemx.DichVuKyThuatId)
                                        {
                                            columTable += "<tr>" +
                                                          "<td class='border-table'>" + stt + "</td>" +
                                                          "<td class='border-table'>" + itemx.TenDichVu + "</td>" +
                                                          "<td class='border-table'>" + itemx.NgayDV + "</td>" +
                                                          "<td class='border-table'>" + itemx.KhoaPhongThucHien +
                                                          "</td>" +
                                                          "<td class='border-table'>" + itemx.GhiChu + "</td>" +
                                                          "</tr>";
                                            stt++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var itemx in listDichVu.Where(z => z.Id == 2).ToList())
                                {
                                    if (item.FirstOrDefault().DichVuKyThuatId == itemx.DichVuKyThuatId)
                                    {
                                        columTable += "<tr>" +
                                                      "<td class='border-table'>" + stt + "</td>" +
                                                      "<td class='border-table'>" + itemx.TenDichVu + "</td>" +
                                                      "<td class='border-table'>" + itemx.NgayDV + "</td>" +
                                                      "<td class='border-table'>" + itemx.KhoaPhongThucHien + "</td>" +
                                                      "<td class='border-table'>" + itemx.GhiChu + "</td>" +
                                                      "</tr>";
                                        stt++;
                                    }
                                }
                            }
                        }

                        data.columnTable = columTable;
                        data.NhanVienTiepDon = "";
                        data.Ngay = DateTime.Now.Day.ToString();
                        data.Thang = DateTime.Now.Month.ToString();
                        data.Nam = DateTime.Now.Year.ToString();
                        data.NguoiDiKham = dataGoiKham.HoTen;
                        data.LogoUrl = phieuInDangKyKSKVo.HostingName + "/assets/img/logo-bacha-full.png";
                        data.BarCodeImgBase64 = "";
                        data.NgayGioSACH = DateTime.Now.ApplyFormatDateTimeSACH();
                        data.MaTN = ""; // chưa có
                    }
                }

                //BVHD-3946
                data.GhiChuDV += "<br>Ghi chú:<br>";

                data.GhiChuDV += "1. Phụ nữ có thai hoặc nghi ngờ có thai không thực hiện chụp Xquang.<br>";

                data.GhiChuDV += "2. Siêu âm trước khi Khám Ngoại.<br>";

                data.GhiChuDV += "3. Khám Nội sau khi khám tất cả các chuyên khoa và làm xong các chỉ định cận lâm sàng(XQ, SÂ, ĐTĐ).<br>";

                data.GhiChuDV += "4. Nộp hồ sơ khám sức khoẻ có ký nhận tại quầy lễ tân tầng 2 của Bệnh Viện.<br>";
                //BVHD-3946

                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                lstHtml.Add(content);
            }
            return lstHtml;
        }
        public string GetTemplatePhieuDangKyKham(int loaiPhieu)
        {
            var content = string.Empty;
            

            switch (loaiPhieu)
            {
                case 1:
                    // Đăng ký khám
                    var templateDKK = _templateRepository.TableNoTracking.First(x => x.Name.Equals("TemPlateFooterPhieuDangKyKham"));

                    var data = new
                    {
                        NgayHienTai = DateTime.Now.ApplyFormatDateTime(),
                        TexTFooTer = "Sau khi khám sức khoẻ xong Anh/Chị nộp lại phiếu tại phòng khám Nội tổng quát"
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(templateDKK.Body, data);
                    return content;
                    break;
                case 2:
                    // phiếu
                    // In kết quả khám
                    var templateInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("TemplateFooterInKetQuaKham"));

                    var dataIKQ = new
                    {
                    };

                    content = TemplateHelpper.FormatTemplateWithContentTemplate(templateInKetQua.Body, dataIKQ);
                    return content;
                    break;
                default:
                    // default
                    var templateDefault = _templateRepository.TableNoTracking.First(x => x.Name.Equals("TemPlateFooterPhieuDangKyKham"));
                    var dataDefault = new
                    {
                    };

                    content = TemplateHelpper.FormatTemplateWithContentTemplate(templateDefault.Body, dataDefault);
                    break;
            }
            return content;
        }
        #endregion
    }
}
