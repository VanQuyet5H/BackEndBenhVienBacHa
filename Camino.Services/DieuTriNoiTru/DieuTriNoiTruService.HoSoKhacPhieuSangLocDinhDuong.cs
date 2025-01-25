using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public NoiTruHoSoKhac GetThongTinHoSoKhacPhieuSangLocDinhDuong(long yeuCauTiepNhanId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.PhieuSangLocDinhDuong)
                                                            .Include(p => p.NoiTruHoSoKhacFileDinhKems)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .FirstOrDefault();
        }

        public GridDataSource GetDanhSachNhuCauDinhDuong()
        {
            var lstHoSoKhacPhieuSanLocDinhDuong = new List<HoSoKhacPhieuSangLocDinhDuongGridVo>();
            lstHoSoKhacPhieuSanLocDinhDuong.Add(new HoSoKhacPhieuSangLocDinhDuongGridVo { Id = 1, ChieuCaoTu = 1.45f, ChieuCaoDen = 1.50f, CanNangTu = 47, CanNangDen = 49, NangLuongTu = 1500, NangLuongDen = 1600, DamTu = 55, DamDen = 58 });
            lstHoSoKhacPhieuSanLocDinhDuong.Add(new HoSoKhacPhieuSangLocDinhDuongGridVo { Id = 2, ChieuCaoTu = 1.51f, ChieuCaoDen = 1.55f, CanNangTu = 50, CanNangDen = 52, NangLuongTu = 1600, NangLuongDen = 1700, DamTu = 59, DamDen = 62 });
            lstHoSoKhacPhieuSanLocDinhDuong.Add(new HoSoKhacPhieuSangLocDinhDuongGridVo { Id = 3, ChieuCaoTu = 1.56f, ChieuCaoDen = 1.60f, CanNangTu = 54, CanNangDen = 56, NangLuongTu = 1700, NangLuongDen = 1800, DamTu = 63, DamDen = 66 });
            lstHoSoKhacPhieuSanLocDinhDuong.Add(new HoSoKhacPhieuSangLocDinhDuongGridVo { Id = 4, ChieuCaoTu = 1.61f, ChieuCaoDen = 1.65f, CanNangTu = 57, CanNangDen = 59, NangLuongTu = 1800, NangLuongDen = 1900, DamTu = 67, DamDen = 70 });
            lstHoSoKhacPhieuSanLocDinhDuong.Add(new HoSoKhacPhieuSangLocDinhDuongGridVo { Id = 5, ChieuCaoTu = 1.66f, ChieuCaoDen = 1.70f, CanNangTu = 60, CanNangDen = 62, NangLuongTu = 1900, NangLuongDen = 2000, DamTu = 71, DamDen = 74 });
            lstHoSoKhacPhieuSanLocDinhDuong.Add(new HoSoKhacPhieuSangLocDinhDuongGridVo { Id = 6, ChieuCaoTu = 1.71f, ChieuCaoDen = 1.75f, CanNangTu = 63, CanNangDen = 65, NangLuongTu = 2000, NangLuongDen = 2100, DamTu = 75, DamDen = 78 });
            lstHoSoKhacPhieuSanLocDinhDuong.Add(new HoSoKhacPhieuSangLocDinhDuongGridVo { Id = 7, ChieuCaoTu = 1.75f, ChieuCaoDen = null, CanNangTu = 66, CanNangDen = 70, NangLuongTu = 2100, NangLuongDen = 2200, DamTu = 79, DamDen = 82 });

            return new GridDataSource
            {
                Data = lstHoSoKhacPhieuSanLocDinhDuong.ToArray(),
                TotalRowCount = lstHoSoKhacPhieuSanLocDinhDuong.Count
            };
        }

        public List<LookupItemVo> GetListGiamCan(DropDownListRequestModel queryInfo)
        {
            var lstGiamCan = EnumHelper.GetListEnum<GiamCan>()
                                       .Select(item => new LookupItemVo
                                       {
                                           KeyId = Convert.ToInt32(item),
                                           DisplayName = item.GetDescription()
                                       }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstGiamCan = lstGiamCan.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstGiamCan;
        }

        public List<LookupItemVo> GetListSoKgGiam(DropDownListRequestModel queryInfo)
        {
            var lstSoKgGiam = EnumHelper.GetListEnum<SoKgGiam>()
                                        .Select(item => new LookupItemVo
                                        {
                                            KeyId = Convert.ToInt32(item),
                                            DisplayName = item.GetDescription()
                                        }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstSoKgGiam = lstSoKgGiam.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstSoKgGiam;
        }

        public List<LookupItemVo> GetListAnUongKem(DropDownListRequestModel queryInfo)
        {
            var lstAnUongKem = EnumHelper.GetListEnum<AnUongKem>()
                                         .Select(item => new LookupItemVo
                                         {
                                             KeyId = Convert.ToInt32(item),
                                             DisplayName = item.GetDescription()
                                         }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstAnUongKem = lstAnUongKem.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstAnUongKem;
        }

        public List<LookupItemVo> GetListTinhTrangBenhLyNang(DropDownListRequestModel queryInfo)
        {
            var lstTinhTrangBenhLyNang = EnumHelper.GetListEnum<TinhTrangBenhLyNang>()
                                                   .Select(item => new LookupItemVo
                                                   {
                                                       KeyId = Convert.ToInt32(item),
                                                       DisplayName = item.GetDescription()
                                                   }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstTinhTrangBenhLyNang = lstTinhTrangBenhLyNang.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstTinhTrangBenhLyNang;
        }

        public LookupItemVo GetDefaultTinhTrangBenhLyNang()
        {
            return new LookupItemVo
            {
                KeyId = Convert.ToInt32(TinhTrangBenhLyNang.Khong),
                DisplayName = TinhTrangBenhLyNang.Khong.GetDescription()
            };
        }

        public List<LookupItemVo> GetListKeHoachDinhDuong(DropDownListRequestModel queryInfo)
        {
            var lstKeHoachDinhDuong = EnumHelper.GetListEnum<KeHoachDinhDuong>()
                                                .Select(item => new LookupItemVo
                                                {
                                                    KeyId = Convert.ToInt32(item),
                                                    DisplayName = item.GetDescription()
                                                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstKeHoachDinhDuong = lstKeHoachDinhDuong.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstKeHoachDinhDuong;
        }

        public List<LookupItemVo> GetListTocDoTangCan(DropDownListRequestModel queryInfo)
        {
            var lstTocDoTangCan = EnumHelper.GetListEnum<TocDoTangCan>()
                                            .Select(item => new LookupItemVo
                                            {
                                                KeyId = Convert.ToInt32(item),
                                                DisplayName = item.GetDescription()
                                            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstTocDoTangCan = lstTocDoTangCan.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstTocDoTangCan;
        }

        public List<LookupItemVo> GetListBenhKemTheo(DropDownListRequestModel queryInfo)
        {
            var lstBenhKemTheo = EnumHelper.GetListEnum<BenhKemTheo>()
                                           .Select(item => new LookupItemVo
                                           {
                                               KeyId = Convert.ToInt32(item),
                                               DisplayName = item.GetDescription()
                                           }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstBenhKemTheo = lstBenhKemTheo.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstBenhKemTheo;
        }

        public async Task<string> InPhieuSangLocDinhDuongPhuSan(long yeuCauTiepNhanId , string hosting)
        {
            var today = DateTime.Now;

            var template = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals("PhieuSangLocDinhDuongPhuSan"));

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(p => p.Id.Equals(yeuCauTiepNhanId))
                                                                     .Include(p => p.NoiTruBenhAn)
                                                                     .Include(p => p.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)
                                                                     .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                     .FirstOrDefaultAsync();

            var yeuCauDichVuGiuongBenhVien = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(p => p.ThoiDiemBatDauSuDung <= today &&
                                                                                                   (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today))
                                                                                       .FirstOrDefault();

            var noiTruHoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.PhieuSangLocDinhDuong)
                                                               .FirstOrDefault();
            var ns = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh);

            long khoaId = 0;
            var phonBVs = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phonBVs != null)
            {
                khoaId = phonBVs.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();

            if (noiTruHoSoKhac == null || noiTruHoSoKhac.ThongTinHoSo == null)
            {
                var defaultData = new
                {
                    HoTen = yeuCauTiepNhan.HoTen,
                    NamSinh = ns,
                    DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                    //TuoiThai = thongTin.TuoiThai,
                    //TheoKinhCuoiCung = thongTin.TheoKinhCuoiCung == true ? "checked" : "",
                    //BaThangDauThaiKy = thongTin.SieuAmBaThangDauThaiKy == true ? "checked" : "",
                    SoBuong = $"{yeuCauDichVuGiuongBenhVien?.GiuongBenh?.PhongBenhVien?.Ten}",
                    SoGiuong = $"{yeuCauDichVuGiuongBenhVien?.GiuongBenh?.Ten}",
                    SoBenhAn = yeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                    MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    KhoaDangIn = "<b>" + tenKhoa + "</b>",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan.ToString()) : "",
                    //ChanDoan = thongTin.ChanDoan,
                    //CanNang = thongTin.CanNangTruocMangThai,
                    //ChieuCao = (float)thongTin.ChieuCaoTruocMangThai.GetValueOrDefault() / 100, //cm -> m
                    //BMI = thongTin.BMITruocMangThai,
                    //CanNangHienTai = thongTin.CanNangHienTai,
                    //BMITruocMangThaiBT185N249 = thongTin.DiemBMITruocMangThai >= 18.5 && thongTin.DiemBMITruocMangThai < 25 ? "checked" : "",
                    //BMITruocMangThaiGE25 = thongTin.DiemBMITruocMangThai >= 25 ? "checked" : "",
                    //BMITruocMangThaiLT185 = thongTin.DiemBMITruocMangThai < 18.5 ? "checked" : "",
                    //TocDoTangCanTheoKhuyenNghi = thongTin.TocDoTangCan == TocDoTangCan.TheoKhuyenNghi ? "checked" : "",
                    //TocDoTangCanTrenDuoiMucKhuyenNghi = thongTin.TocDoTangCan == TocDoTangCan.TrenDuoiMucKhuyenNghi ? "checked" : "",
                    //BenhKemTheoKhong = thongTin.BenhKemTheo == BenhKemTheo.Khong ? "checked" : "",
                    //BenhKemTheoTangHuyetAp = thongTin.BenhKemTheo == BenhKemTheo.TangHuyetAp ? "checked" : "",
                    //KetLuanBinhThuong = thongTin.TongDiemTruocMangThai.GetValueOrDefault() < 2 ? "checked" : "",
                    //KetLuanCoNguyCoDinhDuong = thongTin.TongDiemTruocMangThai.GetValueOrDefault() >= 2 ? "checked" : "",
                    Ngay = today.Day,
                    Thang = today.Month,
                    Nam = today.Year,

                    LogoUrl = hosting + "/assets/img/logo-bacha-full.png",


                    //NguoiThucHien = noiTruHoSoKhac.NhanVienThucHien.User.HoTen
                };

                return TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, defaultData);
            }

            var thongTin = JsonConvert.DeserializeObject<HoSoKhacPhieuSangLocDinhDuongVo>(noiTruHoSoKhac.ThongTinHoSo);

            var soGiuong = string.Empty;
            if (!string.IsNullOrEmpty(yeuCauDichVuGiuongBenhVien?.GiuongBenh?.Ten))
            {
                soGiuong = yeuCauDichVuGiuongBenhVien?.GiuongBenh?.Ten.Replace("Giường", "");
                soGiuong = soGiuong.Replace("GIƯỜNG", "");
            }

            var soBuong = string.Empty;
            if (!string.IsNullOrEmpty(yeuCauDichVuGiuongBenhVien?.GiuongBenh?.PhongBenhVien?.Ten))
            {
                soBuong = yeuCauDichVuGiuongBenhVien?.GiuongBenh?.PhongBenhVien?.Ten.Replace("Buồng", "");
                soBuong = soBuong.Replace("BUỒNG", "");
            }
            var cd = string.Empty;

            cd = !string.IsNullOrEmpty(thongTin.ChanDoan) ? "<div class='container'>" +
                                                             "<div class='label'></div>" +
                                                             $"<div class='value breakword'>Chẩn đoán: {thongTin.ChanDoan.Replace("\n", "<br>")}</div>" +
                                                             "</div>"
                                                             :
                                                             "<div class='container'>" +
                                                             "<div class='label'>Chẩn đoán:</div>" +
                                                             $"<div class='value breakword'>{thongTin.ChanDoan.Replace("\n", "<br>")}</div>" +
                                                             "</div>";




            var data = new HoSoKhacPhieuInSangLocDinhDuongChoPhuNuMangThai
            {
                LogoUrl = hosting + "/assets/img/logo-bacha-full.png",
                HoTen = yeuCauTiepNhan.HoTen,
                NamSinh = ns,

                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                TuoiThai = thongTin.TuoiThai,
                TheoKinhCuoiCung = thongTin.TheoKinhCuoiCung == true ? "checked" : "",
                BaThangDauThaiKy = thongTin.SieuAmBaThangDauThaiKy == true ? "checked" : "",
                SoBuong = soBuong,
                SoGiuong = soGiuong,
                SoBenhAn = yeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                ChanDoan = cd,
                CanNang = thongTin.CanNangTruocMangThai,
                ChieuCao = (float)thongTin.ChieuCaoTruocMangThai.GetValueOrDefault() / 100, //cm -> m
                BMIDisplay = thongTin.BMITruocMangThai != null ? formatBMI(Convert.ToDouble(thongTin.BMITruocMangThai)) : "",
                CanNangHienTai = thongTin.CanNangHienTai,
                BMITruocMangThaiBT185N249 = thongTin.BMITruocMangThai >= 18.5 && thongTin.DiemBMITruocMangThai < 25 ? "checked" : "",
                BMITruocMangThaiGE25 = thongTin.BMITruocMangThai >= 25 ? "checked" : "",
                BMITruocMangThaiLT185 = thongTin.BMITruocMangThai < 18.5 ? "checked" : "",
                TocDoTangCanTheoKhuyenNghi = thongTin.TocDoTangCan == TocDoTangCan.TheoKhuyenNghi ? "checked" : "",
                TocDoTangCanTrenDuoiMucKhuyenNghi = thongTin.TocDoTangCan == TocDoTangCan.TrenDuoiMucKhuyenNghi ? "checked" : "",
                BenhKemTheoKhong = thongTin.BenhKemTheo == BenhKemTheo.Khong ? "checked" : "",
                BenhKemTheoTangHuyetAp = thongTin.BenhKemTheo == BenhKemTheo.TangHuyetAp ? "checked" : "",
                KetLuanBinhThuong = thongTin.TongDiemTruocMangThai.GetValueOrDefault() < 2 ? "checked" : "",
                KetLuanCoNguyCoDinhDuong = thongTin.TongDiemTruocMangThai.GetValueOrDefault() >= 2 ? "checked" : "",
                Ngay = thongTin.NgayDanhGia != null ? (thongTin.NgayDanhGia?.Day > 9 ? thongTin.NgayDanhGia?.Day +"" : "0" + thongTin.NgayDanhGia?.Day) :today.Day +"",
                Thang = thongTin.NgayDanhGia != null ? (thongTin.NgayDanhGia?.Month > 9 ? thongTin.NgayDanhGia?.Month + "" : "0" + thongTin.NgayDanhGia?.Month) : today.Month + "",
                Nam = thongTin.NgayDanhGia != null ? thongTin.NgayDanhGia?.Year  +"" : today.Year +"",
                NguoiThucHien = thongTin.NguoiThucHienName,
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                KhoaDangIn = "<b>" + tenKhoa + "</b>",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan.ToString()) : ""
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }

        public async Task<string> InPhieuSangLocDinhDuong(long yeuCauTiepNhanId, string hosting,long noiTruHoSoKhacId)
        {
            var today = DateTime.Now;

            var template = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals("PhieuSangLocDinhDuong"));

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(p => p.Id.Equals(yeuCauTiepNhanId))
                                                                     .Include(p => p.NgheNghiep)
                                                                     .Include(p => p.NoiTruBenhAn)
                                                                     .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                     .FirstOrDefaultAsync();

            var noiTruHoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.PhieuSangLocDinhDuong
                                                                           && p.Id == noiTruHoSoKhacId)
                                                               .FirstOrDefault();

            long khoaId = 0;
            var phonBVs = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phonBVs != null)
            {
                khoaId = phonBVs.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(L => L.Ten).FirstOrDefault();



            var ns = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh);

            var hoTen = string.Empty;

            hoTen = !string.IsNullOrEmpty(yeuCauTiepNhan.HoTen) ? $"<div class='container'>" +
                                                                      $"<div class='label'>Họ và tên:&nbsp; </div>" +
                                                                      $"<div class='values'><b>{yeuCauTiepNhan.HoTen}</b></div>" +
                                                                  $"</div>"
                                                                          :
                                                                  $"<div class='container'>" +
                                                                      $"<div class='label'>Họ và tên: </div>" +
                                                                      $"<div class='value'><b>{yeuCauTiepNhan.HoTen}</b></div>" +
                                                                  $"</div>";

            var gioiTinh = string.Empty;

            //gioiTinh = !string.IsNullOrEmpty(yeuCauTiepNhan.GioiTinh?.GetDescription()) ? $"<div class='container'>" +
            //                                                          $"<div class='label'>Giới tính:&nbsp; </div>" +
            //                                                          $"<div class='values'><b>{yeuCauTiepNhan.GioiTinh?.GetDescription()}</b></div>" +
            //                                                      $"</div>"
            //                                                              :
            //                                                      $"<div class='container'>" +
            //                                                          $"<div class='label'>Giới tính: </div>" +
            //                                                          $"<div class='value'><b>{yeuCauTiepNhan.GioiTinh?.GetDescription()}</b></div>" +
            //                                                      $"</div>";

            gioiTinh = !string.IsNullOrEmpty(yeuCauTiepNhan.GioiTinh?.GetDescription()) ? $"Giới tính:&nbsp;<b>{yeuCauTiepNhan.GioiTinh?.GetDescription()}</b>"
                                                                                           :
                                                                                           $"Giới tính:&nbsp;";

            var ngayThangNamSinh = string.Empty;

            ngayThangNamSinh = !string.IsNullOrEmpty(ns) ? $"<div class='container'>" +
                                                                      $"<div class='label'>Ngày/tháng/năm sinh:&nbsp; </div>" +
                                                                      $"<div class='values'><b>{ns}</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{gioiTinh}</div>" +
                                                                  $"</div>"
                                                                          :
                                                                  $"<div class='container'>" +
                                                                      $"<div class='label'>Ngày/tháng/năm sinh: </div>" +
                                                                      $"<div class='value'><b>{ns}</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{gioiTinh}</div>" +
                                                                  $"</div>";
            



            var arCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan.ToString()) : "";



          

            var thongTin = JsonConvert.DeserializeObject<HoSoKhacPhieuSangLocDinhDuongVo>(noiTruHoSoKhac.ThongTinHoSo);


            var cd = string.Empty;

            cd = !string.IsNullOrEmpty(thongTin.ChanDoan) ? $"<div class='container'>" +
                                                                      $"<div class='label'></div>" +
                                                                      $"<div class='values breakword'>Chẩn đoán:&nbsp; {thongTin.ChanDoan.Replace("\n","<br>")}</div>" +
                                                                  $"</div>"
                                                                          :
                                                                  $"<div class='container'>" +
                                                                      $"<div class='label'>Chẩn đoán: </div>" +
                                                                      $"<div class='value breakword'>&nbsp;</div>" +
                                                                  $"</div>";

            var canNang = string.Empty;
            //canNang += "Cân nặng:";

            if (thongTin.CanNang != null)
            {
                canNang += "&nbsp;" + thongTin.CanNang + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            else
            {
                canNang +=  "..........";
            }

            if (thongTin.ChieuCao != null)
            {
                canNang += "Chiều cao: " + thongTin.ChieuCao + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            else
            {
                canNang += "Chiều cao: ..........";
            }
            if (thongTin.BMI != null)
            {
                canNang += "BMI: " + formatBMI(Convert.ToDouble(thongTin.BMI));
            }
            else
            {
                canNang += "BMI: ..........";
            }


            var sangLocDinhDuong = string.Empty;
            sangLocDinhDuong += "<tr>";
            sangLocDinhDuong += "<td>BMI</td>";
            #region BMI
            if (thongTin.BMI != null)
            {
                if(thongTin.BMI > 18.5)
                {
                    sangLocDinhDuong += "<td class='diemTong'>" +
                                            $"> 18,5 <input type='checkbox' id='BMI' name='bmi' checked disabled style='float: right;'> " +
                                        "</td>";
                    sangLocDinhDuong += "<td class='diemTong'>" +
                                           $"16 - 18,5 <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                       "</td>";

                    sangLocDinhDuong += "<td class='diemTong'>" +
                                          $"< 16  <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                      "</td>";
                }
                if (thongTin.BMI >= 16 && thongTin.BMI <= 18.5)
                {
                    sangLocDinhDuong += "<td class='diemTong'>" +
                                            $"> 18,5 <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                        "</td>";
                    sangLocDinhDuong += "<td class='diemTong'>" +
                                           $"16 - 18,5 <input type='checkbox' id='BMI' name='bmi' checked disabled style='float: right;'> " +
                                       "</td>";

                    sangLocDinhDuong += "<td class='diemTong'>" +
                                          $"< 16  <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                      "</td>";
                }
                if ( thongTin.BMI < 16)
                {
                    sangLocDinhDuong += "<td class='diemTong'>" +
                                            $"> 18,5 <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                        "</td>";
                    sangLocDinhDuong += "<td class='diemTong'>" +
                                           $"16 - 18,5 <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                       "</td>";

                    sangLocDinhDuong += "<td class='diemTong'>" +
                                          $"< 16  <input type='checkbox' id='BMI' name='bmi' checked disabled style='float: right;'> " +
                                      "</td>";
                }

            }
            else
            {
                sangLocDinhDuong += "<td class='diemTong'>" +
                                            $"> 18,5 <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                        "</td>";
                sangLocDinhDuong += "<td class='diemTong'>" +
                                       $"16 - 18,5 <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                   "</td>";

                sangLocDinhDuong += "<td class='diemTong'>" +
                                      $"< 16  <input type='checkbox' id='BMI' name='bmi'  disabled style='float: right;'> " +
                                  "</td>";
            }
            sangLocDinhDuong += "</tr>";
            #endregion end BMI

            #region sụt cân 1 tháng qua
            sangLocDinhDuong += "<tr>";
            sangLocDinhDuong += "<td>Sụt cân 1 tháng qua</td>";
           
            if (thongTin.SutCan1ThangQua != null)
            {

                switch (thongTin.SutCan1ThangQua)
                {
                    case SutCan1ThangQua.KhongDen2Kg:
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                           $" 0 - 2 kg <input type='checkbox' id='sut-can' name='sutcan' checked disabled style='float: right;'> " +
                                       "</td>";
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                               $"2 - 5 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                           "</td>";

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                              $"5 - 10 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                          "</td>";
                        break;
                    case SutCan1ThangQua.HaiDen5Kg:
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                          $" 0 - 2 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                      "</td>";
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                               $"2 - 5 kg <input type='checkbox' id='sut-can' name='sutcan' checked disabled style='float: right;'> " +
                                           "</td>";

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                              $"5 - 10 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                          "</td>";
                        break;
                    case SutCan1ThangQua.NamDen10Kg:

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                         $" 0 - 2 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                     "</td>";
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                               $"2 - 5 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                           "</td>";

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                              $"5 - 10 kg <input type='checkbox' id='sut-can' name='sutcan' checked disabled style='float: right;'> " +
                                          "</td>";
                        break;

                    default:
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                        $"> 0 - 2 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                    "</td>";
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                               $"2 - 5 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                           "</td>";

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                              $"5 - 10 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                          "</td>";
                        break;
                }
            }
            else
            {
                sangLocDinhDuong += "<td class='diemTong'>" +
                                         $"> 0 - 2 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                     "</td>";
                sangLocDinhDuong += "<td class='diemTong'>" +
                                       $"2 - 5 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                   "</td>";

                sangLocDinhDuong += "<td class='diemTong'>" +
                                      $"5 - 10 kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                  "</td>";
            }
            sangLocDinhDuong += "</tr>";
            #endregion end sụt cân 1 tháng qua

            #region Ăn kém > 5 ngày
            sangLocDinhDuong += "<tr>";
            sangLocDinhDuong += "<td>Ăn kém > 5 ngày</td>";

            if (thongTin.AnKemLonHon5 != null)
            {

                switch (thongTin.AnKemLonHon5)
                {
                    case AnKemLonHon5Ngay.Khong:
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                           $" Không <input type='checkbox' id='sut-can' name='sutcan' checked disabled style='float: right;'> " +
                                       "</td>";
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                               $"&#8805; 50% <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                           "</td>";

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                              $"&#8805; 70% kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                          "</td>";
                        break;
                    case AnKemLonHon5Ngay.LonHon50:
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                          $"> Không <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                      "</td>";
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                               $"&#8805; 50% <input type='checkbox' id='sut-can' name='sutcan'  checked disabled style='float: right;'> " +
                                           "</td>";

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                              $"&#8805; 70% kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                          "</td>";
                        break;
                    case AnKemLonHon5Ngay.LonHon70:

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                          $" Không <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                      "</td>";
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                               $"&#8805; 50% <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                           "</td>";

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                              $"&#8805; 70% kg <input type='checkbox' id='sut-can' name='sutcan' checked disabled style='float: right;'> " +
                                          "</td>";
                        break;

                    default:
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                          $" Không <input type='checkbox' id='sut-can' name='sutcan' checked disabled style='float: right;'> " +
                                      "</td>";
                        sangLocDinhDuong += "<td class='diemTong'>" +
                                               $"&#8805; 50% <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                           "</td>";

                        sangLocDinhDuong += "<td class='diemTong'>" +
                                              $"&#8805; 700% kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                          "</td>";
                        break;
                }
            }
            else
            {
                sangLocDinhDuong += "<td class='diemTong'>" +
                                          $" Không <input type='checkbox' id='sut-can' name='sutcan' checked disabled style='float: right;'> " +
                                      "</td>";
                sangLocDinhDuong += "<td class='diemTong'>" +
                                       $"&#8805; 50% <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                   "</td>";

                sangLocDinhDuong += "<td class='diemTong'>" +
                                      $"&#8805; 70% kg <input type='checkbox' id='sut-can' name='sutcan'  disabled style='float: right;'> " +
                                  "</td>";
            }
            sangLocDinhDuong += "</tr>";
            #endregion Ăn kém > 5 ngày

            var diemTong = string.Empty;

            diemTong = thongTin.TongDiemMST + "";

            var nguyCoDinhDuong = string.Empty;

            if(thongTin.TongDiemMST != null)
            {
                if (thongTin.TongDiemMST < 1)
                {
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan' checked disabled >&nbsp; < 1 điểm</td>";
                    nguyCoDinhDuong += "<td>Chưa có nguy cơ</td>";
                    nguyCoDinhDuong += "</tr>";
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; &#8805; 1 điểm</td>";
                    nguyCoDinhDuong += "<td>Nguy cơ trung bình</td>";
                    nguyCoDinhDuong += "</tr>";
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; &#8805; 2 điểm</td>";
                    nguyCoDinhDuong += "<td>Nguy cơ cao</td>";
                    nguyCoDinhDuong += "</tr>";

                }
                if (thongTin.TongDiemMST >= 1 && thongTin.TongDiemMST <2)
                {
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; < 1 điểm</td>";
                    nguyCoDinhDuong += "<td>Chưa có nguy cơ</td>";
                    nguyCoDinhDuong += "</tr>";
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan' checked disabled >&nbsp; &#8805; 1 điểm</td>";
                    nguyCoDinhDuong += "<td>Nguy cơ trung bình</td>";
                    nguyCoDinhDuong += "</tr>";
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; &#8805; 2 điểm</td>";
                    nguyCoDinhDuong += "<td>Nguy cơ cao</td>";
                    nguyCoDinhDuong += "</tr>";
                }
                if (thongTin.TongDiemMST >= 2)
                {
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; < 1 điểm</td>";
                    nguyCoDinhDuong += "<td>Chưa có nguy cơ</td>";
                    nguyCoDinhDuong += "</tr>";
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; &#8805; 1 điểm</td>";
                    nguyCoDinhDuong += "<td>Nguy cơ trung bình</td>";
                    nguyCoDinhDuong += "</tr>";
                    nguyCoDinhDuong += "<tr>";
                    nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan' checked disabled >&nbsp; &#8805; 2 điểm</td>";
                    nguyCoDinhDuong += "<td>Nguy cơ cao</td>";
                    nguyCoDinhDuong += "</tr>";
                }

            }
            else
            {
                nguyCoDinhDuong += "<tr>";
                nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; < 1 điểm</td>";
                nguyCoDinhDuong += "<td>Chưa có nguy cơ</td>";
                nguyCoDinhDuong += "</tr>";
                nguyCoDinhDuong += "<tr>";
                nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; &#8805; 1 điểm</td>";
                nguyCoDinhDuong += "<td>Nguy cơ trung bình</td>";
                nguyCoDinhDuong += "</tr>";
                nguyCoDinhDuong += "<tr>";
                nguyCoDinhDuong += "<td> <input type='checkbox' id='sut-can' name='sutcan'  disabled >&nbsp; &#8805; 2 điểm</td>";
                nguyCoDinhDuong += "<td>Nguy cơ cao</td>";
                nguyCoDinhDuong += "</tr>";
            }

            var cheDoAnUongDisPlay = string.Empty;
          
            if (thongTin.CheDoAnUong != null && thongTin.CheDoAnUong != 0)
            {
                cheDoAnUongDisPlay = _cheDoAnRepository.TableNoTracking.Where(d => d.Id == thongTin.CheDoAnUong).Select(d=>d.KyHieu).FirstOrDefault();
        
            }

            var duongMieng = string.Empty;
            var ongThong = string.Empty;
            var tinhMach = string.Empty;

            if (thongTin.DuongNuoiDuong != null)
            {
                
                switch (thongTin.DuongNuoiDuong)
                {
                    case DuongNuoiDuong.DuongMieng:
                        duongMieng = "checked";
                        break;
                    case DuongNuoiDuong.OngThong:
                        ongThong = "checked";
                        break;
                    case DuongNuoiDuong.TinhMach:
                        tinhMach = "checked";
                        break;

                    default:
                        break;
                }
            }

            var khong = string.Empty;
            var co = string.Empty;

            if (thongTin.HoiChanDinhDuong != null )
            {

                switch (thongTin.HoiChanDinhDuong)
                {
                    case HoiChanDinhDuong.Khong:
                        khong = "checked";
                        break;
                    case HoiChanDinhDuong.Co:
                        co = "checked";
                        break;

                    default:
                        break;
                }
            }

            var taiDanhGiaBaNgay = string.Empty;
            var taiDanhGiaBayNgay = string.Empty;

            if (thongTin.TaiDanhGia != null )
            {

                switch (thongTin.TaiDanhGia)
                {
                    case TaiDanhGia.SauBaNgay:
                        taiDanhGiaBaNgay = "checked";
                        break;
                    case TaiDanhGia.SauBayNgay:
                        taiDanhGiaBayNgay = "checked";
                        break;

                    default:
                        break;
                }
            }



            var ngay = string.Empty;
            var thang = string.Empty;
            var nam = string.Empty;

            if(!string.IsNullOrEmpty(thongTin.NgayDanhGiaString))
            {
                DateTime ngayDG = DateTime.Now;
                DateTime.TryParseExact(thongTin.NgayDanhGiaString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayDG);
                ngay = ngayDG.Day > 9 ? ngayDG.Day + "" : "0" + ngayDG.Day;
                thang = ngayDG.Month > 9 ? ngayDG.Month + "" : "0" + ngayDG.Month;
                nam = ngayDG.Year +"";
            }




            var data = new InPhieuSangLocDinhDuongVo
            {
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                KhoaDangIn = "<b>" + tenKhoa + "</b>",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan.ToString()) : "",
                HoTen = hoTen,
                NgayThangNamSinh = ngayThangNamSinh,
                //GioiTinh = gioiTinh,
                ChanDoan = cd,
                CanNang = canNang,
                SangLocDinhDuong = sangLocDinhDuong,
                DiemTong = diemTong,
                NguyCoDinhDuong = nguyCoDinhDuong,
                CheDoAnUongDisplay = cheDoAnUongDisPlay,
                DuongMieng = duongMieng,
                OngThong = ongThong,
                TinhMach = tinhMach,
                Khong =khong,
                Co = co,
                TaiDanhGiaBaNgay = taiDanhGiaBaNgay,
                TaiDanhGiaBayNgay = taiDanhGiaBayNgay,
                Ngay = ngay ,
                Thang = thang,
                Nam = nam,
                BacSyDieuTri = "<b>"+ thongTin.BacSiDieuTriName + "</b>"
            };




            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }
        // BVHD-3886
        public string GetChanDoanVaoVien(long yeuCauTiepNhanId)
        {
            var result = ThongTinBenhNhanPhieuInSangLocDinhDuong(yeuCauTiepNhanId).Result.ChanDoanNhapVien;
            return result;
        }
        private async Task<DataInPhieuInSangLocDinhDuongVaSerivcesVo> ThongTinBenhNhanPhieuInSangLocDinhDuong(long yeuCauTiepNhanId)
        {
            var thongTinBenhNhanPhieuThuoc = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(s => s.Id == yeuCauTiepNhanId)
                .Select(s => new DataInPhieuInSangLocDinhDuongVaSerivcesVo
                {
                    HoTenNgBenh = s.HoTen,
                    NamSinh = s.NamSinh,
                    GTNgBenh = s.GioiTinh.GetDescription(),
                    GioiTinh = s.GioiTinh,
                    DiaChi = s.BenhNhan.DiaChiDayDu,
                    Cmnd = s.SoChungMinhThu,
                    MaBn = s.BenhNhan.MaBN,
                    NhomMau = s.NhomMau != null ? s.NhomMau.GetDescription() : string.Empty,
                    MaSoTiepNhan = s.MaYeuCauTiepNhan,
                    NgayVaoVien = s.NoiTruBenhAn.ThoiDiemNhapVien,
                    NgayRaVien = s.NoiTruBenhAn.ThoiDiemRaVien,
                    ChanDoanVaoVien = s.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    Buong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                    Giuong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.Ten).FirstOrDefault(),
                    LoaiBenhAn = s.NoiTruBenhAn.LoaiBenhAn,
                    SoBenhAn = s.NoiTruBenhAn.SoBenhAn,
                    YeuCauKhamBenhId = s.YeuCauNhapVien.YeuCauKhamBenhId,

                }).FirstOrDefault();
            if(thongTinBenhNhanPhieuThuoc.YeuCauKhamBenhId != null)
            {
                thongTinBenhNhanPhieuThuoc.ChanDoanNhapVien += _yeuCauKhamBenhRepository.TableNoTracking.Where(d => d.Id == thongTinBenhNhanPhieuThuoc.YeuCauKhamBenhId && d.IcdchinhId != null).Select(d => d.Icdchinh.TenTiengViet ).FirstOrDefault();
                var cd = _yeuCauKhamBenhICDKhacRepository.TableNoTracking.Where(d => d.YeuCauKhamBenhId == thongTinBenhNhanPhieuThuoc.YeuCauKhamBenhId && d.ICDId != null).Select(d => d.ICD.TenTiengViet).ToList();
                if (cd.Count() != 0)
                {
                    thongTinBenhNhanPhieuThuoc.ChanDoanNhapVien += " (chẩn đoán Kèm theo:" + cd.Join(", ") +")";
                }

                
            }
           
      
            return thongTinBenhNhanPhieuThuoc;
        }

        public List<LookupItemVo> GetListSutCanMotThangQua(DropDownListRequestModel queryInfo)
        {
            var lstSutCan1ThangQua = EnumHelper.GetListEnum<SutCan1ThangQua>()
                                        .Select(item => new LookupItemVo
                                        {
                                            KeyId = Convert.ToInt32(item),
                                            DisplayName = item.GetDescription()
                                        }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstSutCan1ThangQua = lstSutCan1ThangQua.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstSutCan1ThangQua;
        }

        public List<LookupItemVo> GetListAnKemLonHon5Ngay(DropDownListRequestModel queryInfo)
        {
            var lstAnKemLonHon5Ngay = EnumHelper.GetListEnum<AnKemLonHon5Ngay>()
                                        .Select(item => new LookupItemVo
                                        {
                                            KeyId = Convert.ToInt32(item),
                                            DisplayName = item.GetDescription()
                                        }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstAnKemLonHon5Ngay = lstAnKemLonHon5Ngay.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstAnKemLonHon5Ngay;
        }
        public List<LookupItemVo> GetListDuongNuoiDuong(DropDownListRequestModel queryInfo)
        {
            var lstDuongNuoiDuong = EnumHelper.GetListEnum<DuongNuoiDuong>()
                                        .Select(item => new LookupItemVo
                                        {
                                            KeyId = Convert.ToInt32(item),
                                            DisplayName = item.GetDescription()
                                        }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstDuongNuoiDuong = lstDuongNuoiDuong.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstDuongNuoiDuong;
        }
        public List<LookupItemVo> GetListHoiChanDinhDuong(DropDownListRequestModel queryInfo)
        {
            var lstHoiChanDinhDuong = EnumHelper.GetListEnum<HoiChanDinhDuong>()
                                        .Select(item => new LookupItemVo
                                        {
                                            KeyId = Convert.ToInt32(item),
                                            DisplayName = item.GetDescription()
                                        }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstHoiChanDinhDuong = lstHoiChanDinhDuong.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstHoiChanDinhDuong;
        }
        public List<LookupItemVo> GetListTaiDanhGia(DropDownListRequestModel queryInfo)
        {
            var lstHoiTaiDanhGia = EnumHelper.GetListEnum<TaiDanhGia>()
                                        .Select(item => new LookupItemVo
                                        {
                                            KeyId = Convert.ToInt32(item),
                                            DisplayName = item.GetDescription()
                                        }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstHoiTaiDanhGia = lstHoiTaiDanhGia.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return lstHoiTaiDanhGia;
        }
      
        public async Task<List<LookupItemVo>> GetListTCheDoAnUongs(DropDownListRequestModel model)
        {
            var lstChucDanh = await _cheDoAnRepository.TableNoTracking.Where(d=>d.IsDisabled == false)
                .ApplyLike(model.Query,  g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = lstChucDanh.Select(item => new LookupItemVo()
            {
                DisplayName = item.KyHieu,
                KeyId = item.Id,
            }).ToList();
            return query;
        }
        public List<NoiTruHoSoKhac> GetThongTinHoSoKhacPhieuSangLocDinhDuongs(long yeuCauTiepNhanId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.PhieuSangLocDinhDuong)
                                                            .Include(p => p.NoiTruHoSoKhacFileDinhKems)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .ToList();
        }
        public NoiTruHoSoKhac ViewThongTinHoSoKhacPhieuSangLocDinhDuong(long noiTruHoSoKhacId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.Id == noiTruHoSoKhacId &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.PhieuSangLocDinhDuong)
                                                            .Include(p => p.NoiTruHoSoKhacFileDinhKems)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .FirstOrDefault();
        }
        private string formatBMI(double bmi)
        {
            var result = string.Empty;
            if (bmi != null)
            {
                var chuoi = bmi + "";

                var mangChuoi = chuoi.Split(".");

                if (mangChuoi != null)
                {
                    if (mangChuoi.Length == 1)
                    {
                        result = mangChuoi[0];
                    }
                    if (mangChuoi.Length == 2)
                    {
                        if (mangChuoi[1].Length == 1)
                        {
                            result = mangChuoi[0] + "." + mangChuoi[1].Substring(0, 1);
                        }
                        if (mangChuoi[1].Length >= 2)
                        {
                            result = mangChuoi[0] + "." + mangChuoi[1].Substring(0, 2);
                        }
                    }
                }
            }
            return result;
        }
    }
}
