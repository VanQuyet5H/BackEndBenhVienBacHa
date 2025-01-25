using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<LookupItemVo> GetTatKhoaTiaPlasmaHoTroVetThuong(DropDownListRequestModel queryInfo)
        {
            var khoaPhongs = _KhoaPhongRepository.TableNoTracking
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take)
               .ToList();

            if (khoaPhongs.Count > 0)
            {
                khoaPhongs.Insert(0, new LookupItemVo()
                {
                    KeyId = 0,
                    DisplayName = "Toàn viện"
                });
            }

            return khoaPhongs;
        }

        public async Task<GridDataSource> GetDataBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(QueryInfo queryInfo)
        {
            var lstYeuCauDichVu = new List<DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong>();
            var timKiemNangCaoObj = new BaoCaoDichVuTiaPlasMaHoTroDeuTriVetThuongQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDichVuTiaPlasMaHoTroDeuTriVetThuongQueryInfoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (timKiemNangCaoObj.DichVuId != null && timKiemNangCaoObj.DichVuId != 0 && tuNgay != null && denNgay != null)
            {
                if (timKiemNangCaoObj.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    lstYeuCauDichVu = await _yeuCauKhamBenhRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                        .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                    && (timKiemNangCaoObj.KhoaId == null || timKiemNangCaoObj.KhoaId == 0 || x.NoiChiDinh.KhoaPhongId == timKiemNangCaoObj.KhoaId)
                                    && x.DichVuKhamBenhBenhVienId == timKiemNangCaoObj.DichVuId
                                    && x.ThoiDiemChiDinh >= tuNgay
                                    && x.ThoiDiemChiDinh <= denNgay)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(item => new DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong()
                        {
                            Id = item.Id,
                            MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                            MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            SoBenhAn = string.Empty,
                            HoTen = item.YeuCauTiepNhan.HoTen,
                            NamSinh = item.YeuCauTiepNhan.NamSinh,
                            GioiTinh = item.YeuCauTiepNhan.GioiTinh,
                            NoiDungThucHien = item.TenDichVu,
                            ChanDoanICDChinh = item.Icdchinh != null ? $"{item.Icdchinh.Ma} - {item.Icdchinh.TenTiengViet}" : null,
                            ChanDoanSoBo = item.ChanDoanSoBoICD != null ? $"{item.ChanDoanSoBoICD.Ma} - {item.ChanDoanSoBoICD.TenTiengViet}" : null,
                            SoLuong = 1,
                            DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : (item.GoiKhamSucKhoeId != null ? item.DonGiaUuDai.Value : item.Gia),
                            SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                            DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true, // item.DuocHuongBaoHiem,
                            MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                            DonGiaBHYT = item.DonGiaBaoHiem,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),

                            NoiChiDinh = item.NoiChiDinh.Ten,
                            NoiThucHien = item.NoiThucHien.Ten,
                            NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                            NgayChiDinh = item.ThoiDiemChiDinh,
                            NgayThucHien = item.ThoiDiemThucHien ?? item.ThoiDiemHoanThanh
                        })
                        .Skip(queryInfo.Skip).Take(queryInfo.Take)
                        .ToListAsync();
                }
                else if (timKiemNangCaoObj.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    lstYeuCauDichVu = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                        .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                    && (timKiemNangCaoObj.KhoaId == null || timKiemNangCaoObj.KhoaId == 0 || x.NoiChiDinh.KhoaPhongId == timKiemNangCaoObj.KhoaId)
                                    && x.DichVuKyThuatBenhVienId == timKiemNangCaoObj.DichVuId
                                    && x.ThoiDiemChiDinh >= tuNgay
                                    && x.ThoiDiemChiDinh <= denNgay)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(item => new DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong()
                        {
                            Id = item.Id,
                            MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                            MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            SoBenhAn = item.NoiTruPhieuDieuTri.NoiTruBenhAn.SoBenhAn,
                            NoiTruBenhAnId = item.NoiTruPhieuDieuTri.NoiTruBenhAnId,
                            HoTen = item.YeuCauTiepNhan.HoTen,
                            NamSinh = item.YeuCauTiepNhan.NamSinh,
                            GioiTinh = item.YeuCauTiepNhan.GioiTinh,
                            NoiDungThucHien = item.TenDichVu,
                            ChanDoanRaVien = (item.NoiTruPhieuDieuTriId != null && item.NoiTruPhieuDieuTri.NoiTruBenhAn.ChanDoanChinhRaVienICDId != null)
                                    ? $"{item.NoiTruPhieuDieuTri.NoiTruBenhAn.ChanDoanChinhRaVienICD.Ma} - {item.NoiTruPhieuDieuTri.NoiTruBenhAn.ChanDoanChinhRaVienICD.TenTiengViet}" : null,
                            //ChanDoanTheoPhieuDieuTriCuoiCung = (item.NoiTruPhieuDieuTriId != null && item.NoiTruPhieuDieuTri.NoiTruBenhAn.ChanDoanChinhRaVienICDId == null)
                            //    ? _noiTruPhieuDieuTriRepository.TableNoTracking
                            //        .Where(x => x.NoiTruBenhAnId == item.NoiTruPhieuDieuTri.NoiTruBenhAnId
                            //                    && x.ChanDoanChinhICDId != null)
                            //        .OrderByDescending(x => x.NgayDieuTri)
                            //        .Select(x => $"{x.ChanDoanChinhICD.Ma} - {x.ChanDoanChinhICD.TenTiengViet}").FirstOrDefault() : null,
                            ChanDoanNhapVien = (item.YeuCauTiepNhan.YeuCauNhapVienId != null && item.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICDId != null)
                                ? $"{item.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD.Ma} - {item.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet}" : null,
                            ChanDoanICDChinh = (item.YeuCauKhamBenhId != null && item.YeuCauKhamBenh.IcdchinhId != null) ? $"{item.YeuCauKhamBenh.Icdchinh.Ma} - {item.YeuCauKhamBenh.Icdchinh.TenTiengViet}" : null,
                            ChanDoanSoBo = (item.YeuCauKhamBenhId != null && item.YeuCauKhamBenh.ChanDoanSoBoICDId != null) ? $"{item.YeuCauKhamBenh.ChanDoanSoBoICD.Ma} - {item.YeuCauKhamBenh.ChanDoanSoBoICD.TenTiengViet}" : null,
                            SoLuong = item.SoLan,
                            DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau.Value : (item.GoiKhamSucKhoeId != null ? item.DonGiaUuDai.Value : item.Gia),
                            SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                            DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true, // item.DuocHuongBaoHiem,
                            MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                            DonGiaBHYT = item.DonGiaBaoHiem,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            NoiChiDinh = item.NoiChiDinh.Ten,
                            NoiThucHien = item.NoiThucHien.Ten,
                            NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                            NgayChiDinh = item.ThoiDiemChiDinh,
                            NgayThucHien = item.ThoiDiemThucHien ?? item.ThoiDiemKetLuan ?? item.ThoiDiemHoanThanh
                        })
                        .Skip(queryInfo.Skip).Take(queryInfo.Take)
                        .ToListAsync();
                }
                else if (timKiemNangCaoObj.NhomDichVu == Enums.EnumNhomGoiDichVu.TruyenMau)
                {
                    lstYeuCauDichVu = await _yeuCauTruyenMauRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                        .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien
                                    && (timKiemNangCaoObj.KhoaId == null || timKiemNangCaoObj.KhoaId == 0 || x.NoiChiDinh.KhoaPhongId == timKiemNangCaoObj.KhoaId)
                                    && x.MauVaChePhamId == timKiemNangCaoObj.DichVuId
                                    && x.ThoiDiemChiDinh >= tuNgay
                                    && x.ThoiDiemChiDinh <= denNgay)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(item => new DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong()
                        {
                            Id = item.Id,
                            MaNB = item.YeuCauTiepNhan.BenhNhan.MaBN,
                            MaTN = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            SoBenhAn = item.NoiTruPhieuDieuTri.NoiTruBenhAn.SoBenhAn,
                            NoiTruBenhAnId = item.NoiTruPhieuDieuTri.NoiTruBenhAnId,
                            HoTen = item.YeuCauTiepNhan.HoTen,
                            NamSinh = item.YeuCauTiepNhan.NamSinh,
                            GioiTinh = item.YeuCauTiepNhan.GioiTinh,
                            NoiDungThucHien = item.TenDichVu,
                            ChanDoanRaVien = item.NoiTruPhieuDieuTri.NoiTruBenhAn.ChanDoanChinhRaVienICDId != null
                                    ? $"{item.NoiTruPhieuDieuTri.NoiTruBenhAn.ChanDoanChinhRaVienICD.Ma} - {item.NoiTruPhieuDieuTri.NoiTruBenhAn.ChanDoanChinhRaVienICD.TenTiengViet}" : null,
                            //ChanDoanTheoPhieuDieuTriCuoiCung = item.NoiTruPhieuDieuTri.NoiTruBenhAn.ChanDoanChinhRaVienICDId == null
                            //    ? _noiTruPhieuDieuTriRepository.TableNoTracking
                            //        .Where(x => x.NoiTruBenhAnId == item.NoiTruPhieuDieuTri.NoiTruBenhAnId
                            //                    && x.ChanDoanChinhICDId != null)
                            //        .OrderByDescending(x => x.NgayDieuTri)
                            //        .Select(x => $"{x.ChanDoanChinhICD.Ma} - {x.ChanDoanChinhICD.TenTiengViet}").FirstOrDefault() : null,
                            ChanDoanNhapVien = (item.YeuCauTiepNhan.YeuCauNhapVienId != null && item.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICDId != null)
                                ? $"{item.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD.Ma} - {item.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet}" : null,
                            SoLuong = 1,
                            DonGia = item.DonGiaBan.Value,
                            SoTienMienGiamTheoDichVu = item.SoTienMienGiam,
                            DuocHuongBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true, // item.DuocHuongBaoHiem,
                            MucHuongBaoHiem = item.MucHuongBaoHiem.GetValueOrDefault(),
                            DonGiaBHYT = item.DonGiaBaoHiem,
                            TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            NoiChiDinh = item.NoiChiDinh.Ten,
                            NoiThucHien = item.NoiThucHien.Ten,
                            NguoiChiDinh = item.NhanVienChiDinh.User.HoTen,
                            NgayChiDinh = item.ThoiDiemChiDinh,
                            NgayThucHien = item.ThoiDiemThucHien ?? item.ThoiDiemHoanThanh
                        })
                        .Skip(queryInfo.Skip).Take(queryInfo.Take)
                        .ToListAsync();
                }

                if (lstYeuCauDichVu.Any())
                {
                    var lstNoiTruBenhAnId = lstYeuCauDichVu.Where(x => x.NoiTruBenhAnId != null)
                        .Select(x => x.NoiTruBenhAnId.Value).Distinct().ToList();

                    var lstChanDoanTheoNgayDieuTri = await _noiTruPhieuDieuTriRepository.TableNoTracking
                        .Where(x => lstNoiTruBenhAnId.Contains(x.NoiTruBenhAnId)
                                    && x.ChanDoanChinhICDId != null)
                        .Select(x => new ChanDoanIcdTheoNgayDieuTriVo()
                        {
                            NoiTruBenhAnId = x.NoiTruBenhAnId,
                            NgayDieuTri = x.NgayDieuTri,
                            ChanDoanIcd = $"{x.ChanDoanChinhICD.Ma} - {x.ChanDoanChinhICD.TenTiengViet}"
                        })
                        .Distinct().ToListAsync();

                    foreach (var yeuCauDichVu in lstYeuCauDichVu)
                    {
                        if (yeuCauDichVu.NoiTruBenhAnId != null)
                        {
                            var chanDoanCuoiCungTheoNgayDieuTri = lstChanDoanTheoNgayDieuTri
                                .Where(x => x.NoiTruBenhAnId == yeuCauDichVu.NoiTruBenhAnId)
                                .OrderByDescending(x => x.NgayDieuTri).Select(x => x.ChanDoanIcd).FirstOrDefault();
                            yeuCauDichVu.ChanDoanTheoPhieuDieuTriCuoiCung = chanDoanCuoiCungTheoNgayDieuTri;
                        }
                    }
                }
            }

            return new GridDataSource
            {
                Data = lstYeuCauDichVu.ToArray(),
                TotalRowCount = lstYeuCauDichVu.Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuongForGrid(QueryInfo queryInfo)
        {
            var lstYeuCauDichVu = new List<DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong>();
            var timKiemNangCaoObj = new BaoCaoDichVuTiaPlasMaHoTroDeuTriVetThuongQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDichVuTiaPlasMaHoTroDeuTriVetThuongQueryInfoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            if (timKiemNangCaoObj.DichVuId != null && timKiemNangCaoObj.DichVuId != 0 && tuNgay != null && denNgay != null)
            {
                if (timKiemNangCaoObj.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    lstYeuCauDichVu = await _yeuCauKhamBenhRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                        .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                    && (timKiemNangCaoObj.KhoaId == null || timKiemNangCaoObj.KhoaId == 0 || x.NoiChiDinh.KhoaPhongId == timKiemNangCaoObj.KhoaId)
                                    && x.DichVuKhamBenhBenhVienId == timKiemNangCaoObj.DichVuId
                                    && x.ThoiDiemChiDinh >= tuNgay
                                    && x.ThoiDiemChiDinh <= denNgay)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(item => new DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong()
                        {
                            Id = item.Id
                        })
                        .ToListAsync();
                }
                else if (timKiemNangCaoObj.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    lstYeuCauDichVu = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                        .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                    && (timKiemNangCaoObj.KhoaId == null || timKiemNangCaoObj.KhoaId == 0 || x.NoiChiDinh.KhoaPhongId == timKiemNangCaoObj.KhoaId)
                                    && x.DichVuKyThuatBenhVienId == timKiemNangCaoObj.DichVuId
                                    && x.ThoiDiemChiDinh >= tuNgay
                                    && x.ThoiDiemChiDinh <= denNgay)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(item => new DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong()
                        {
                            Id = item.Id
                        })
                        .ToListAsync();
                }
                else if (timKiemNangCaoObj.NhomDichVu == Enums.EnumNhomGoiDichVu.TruyenMau)
                {
                    lstYeuCauDichVu = await _yeuCauTruyenMauRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                        .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauTruyenMau.DaThucHien
                                    && (timKiemNangCaoObj.KhoaId == null || timKiemNangCaoObj.KhoaId == 0 || x.NoiChiDinh.KhoaPhongId == timKiemNangCaoObj.KhoaId)
                                    && x.MauVaChePhamId == timKiemNangCaoObj.DichVuId
                                    && x.ThoiDiemChiDinh >= tuNgay
                                    && x.ThoiDiemChiDinh <= denNgay)
                        .OrderBy(x => x.ThoiDiemChiDinh)
                        .Select(item => new DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong()
                        {
                            Id = item.Id
                        })
                        .ToListAsync();
                }
            }

            return new GridDataSource
            {
                Data = lstYeuCauDichVu.ToArray(),
                TotalRowCount = lstYeuCauDichVu.Count()
            };
        }

        public virtual byte[] ExportBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong(GridDataSource gridDataSource, QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoDichVuTiaPlasMaHoTroDeuTriVetThuongQueryInfoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDichVuTiaPlasMaHoTroDeuTriVetThuongQueryInfoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var datas = (ICollection<DanhSachDichVuTiaPlasMaHoTroDeuTriVetThuong>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoNguoiBenhDenKhamGridVo>("STT", p => ind++)
            };

            var formatCurrency = "#,##0.00";

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO DỊCH VỤ ĐÃ Thực Hiện");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 10;
                    worksheet.Column(3).Width = 10;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 25;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 10;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 10;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 25;
                    worksheet.Column(16).Width = 25;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;

                    using (var range = worksheet.Cells["A1:P1"])
                    {
                        range.Worksheet.Cells["A1:E1"].Merge = true;
                        range.Worksheet.Cells["A1:E1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:E1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:E1"].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["A2:P2"])
                    {
                        range.Worksheet.Cells["A2:P2"].Merge = true;
                        range.Worksheet.Cells["A2:P2"].Value = "BÁO CÁO DỊCH VỤ " + timKiemNangCaoObj.TenDichVu?.ToUpper();
                        range.Worksheet.Cells["A2:P2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:P2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:P2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:P2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:P2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:P3"])
                    {
                        range.Worksheet.Cells["A3:P3"].Merge = true;
                        range.Worksheet.Cells["A3:P3"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A3:P3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:P3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:P3"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A3:P3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:P3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A5:S5"])
                    {
                        range.Worksheet.Cells["A5:S5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:S5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:S5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:S5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:S5"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A5:S5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A5:A5"].Merge = true;
                        range.Worksheet.Cells["A5:A5"].Value = "STT";
                        range.Worksheet.Cells["A5:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A5:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B5:B5"].Merge = true;
                        range.Worksheet.Cells["B5:B5"].Value = "Mã NB";
                        range.Worksheet.Cells["B5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B5:B5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C5:C5"].Merge = true;
                        range.Worksheet.Cells["C5:C5"].Value = "Mã TN";
                        range.Worksheet.Cells["C5:C5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C5:C5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D5:D5"].Merge = true;
                        range.Worksheet.Cells["D5:D5"].Value = "Số BA";
                        range.Worksheet.Cells["D5:D5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D5:D5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E5:E5"].Merge = true;
                        range.Worksheet.Cells["E5:E5"].Value = "Họ và tên";
                        range.Worksheet.Cells["E5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E5:E5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F5:F5"].Merge = true;
                        range.Worksheet.Cells["F5:F5"].Value = "Năm sinh";
                        range.Worksheet.Cells["F5:F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F5:F5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G5:G5"].Merge = true;
                        range.Worksheet.Cells["G5:G5"].Value = "Giới tính";
                        range.Worksheet.Cells["G5:G5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G5:G5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H5:H5"].Merge = true;
                        range.Worksheet.Cells["H5:H5"].Value = "Nội dung thực hiện";
                        range.Worksheet.Cells["H5:H5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H5:H5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I5:I5"].Merge = true;
                        range.Worksheet.Cells["I5:I5"].Value = "Chẩn đoán";
                        range.Worksheet.Cells["I5:I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I5:I5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J5:J5"].Merge = true;
                        range.Worksheet.Cells["J5:J5"].Value = "Số lượng";
                        range.Worksheet.Cells["J5:J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J5:J5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K5:K5"].Merge = true;
                        range.Worksheet.Cells["K5:K5"].Value = "Đơn giá BV";
                        range.Worksheet.Cells["K5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K5:K5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L5:L5"].Merge = true;
                        range.Worksheet.Cells["L5:L5"].Value = "Số tiền miễn giảm";
                        range.Worksheet.Cells["L5:L5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L5:L5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M5:M5"].Merge = true;
                        range.Worksheet.Cells["M5:M5"].Value = "Số tiền BHYT";
                        range.Worksheet.Cells["M5:M5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M5:M5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N5:N5"].Merge = true;
                        range.Worksheet.Cells["N5:N5"].Value = "Thanh toán";
                        range.Worksheet.Cells["N5:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N5:N5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O5:O5"].Merge = true;
                        range.Worksheet.Cells["O5:O5"].Value = "Nơi chỉ định";
                        range.Worksheet.Cells["O5:O5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O5:O5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P5:P5"].Merge = true;
                        range.Worksheet.Cells["P5:P5"].Value = "Nơi thực hiện";
                        range.Worksheet.Cells["P5:P5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P5:P5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q5:Q5"].Merge = true;
                        range.Worksheet.Cells["Q5:Q5"].Value = "Người chỉ định";
                        range.Worksheet.Cells["Q5:Q5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q5:Q5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R5:R5"].Merge = true;
                        range.Worksheet.Cells["R5:R5"].Value = "Ngày chỉ định";
                        range.Worksheet.Cells["R5:R5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["R5:R5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S5:S5"].Merge = true;
                        range.Worksheet.Cells["S5:S5"].Value = "Ngày thực hiện";
                        range.Worksheet.Cells["S5:S5"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["S5:S5"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<BaoCaoNguoiBenhDenKhamGridVo>(requestProperties);
                    int index = 6;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":S" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":S" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaNB;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaTN;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.SoBenhAn;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.HoTen;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.NamSinh;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.GioiTinhStr;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.NoiDungThucHien;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.ChanDoan;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.SoLuong;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.DonGia;
                                worksheet.Cells["K" + index].Style.Numberformat.Format = formatCurrency;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.SoTienMienGiam;
                                worksheet.Cells["L" + index].Style.Numberformat.Format = formatCurrency;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.BHYTThanhToan;
                                worksheet.Cells["M" + index].Style.Numberformat.Format = formatCurrency;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.ThanhToan;
                                worksheet.Cells["N" + index].Style.Numberformat.Format = formatCurrency;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.NoiChiDinh;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.NoiThucHien;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.NguoiChiDinh;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.NgayChiDinhStr;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.NgayThucHienStr;

                                index++;
                                stt++;
                            }
                        }

                        using (var range = worksheet.Cells["A" + index + ":S" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":S" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":S" + index].Style.Font.Bold = true;

                            range.Worksheet.Cells["A" + index + ":I" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":I" + index].Value = "Tổng cộng";

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Value = datas.Sum(x => x.SoLuong);

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Value = datas.Sum(x => x.ThanhToan);
                            worksheet.Cells["N" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            index++;
                        }
                    }


                    index++;

                    worksheet.Cells["A" + index + ":p" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":P" + index].Style.Font.Bold = true;

                    worksheet.Cells["M" + index + ":P" + index].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    worksheet.Cells["M" + index + ":P" + index].Style.Font.Italic = true;
                    worksheet.Cells["M" + index + ":P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["M" + index + ":P" + index].Merge = true;

                    index++;
                    worksheet.Cells["A" + index + ":p" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":P" + index].Style.Font.Bold = true;
           
                    worksheet.Cells["A" + index + ":H" + index].Value = "Trưởng khoa";
                    worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":H" + index].Merge = true;
                    worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;

        
                    worksheet.Cells["I" + index + ":L" + index].Value = "Đại diện kế toán";
                    worksheet.Cells["I" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":L" + index].Merge = true;
                    worksheet.Cells["I" + index + ":L" + index].Style.Font.Bold = true;


                   

                    index++;

                    worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":H" + index].Value = "(Ký và ghi rõ họ tên)";
                    worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":H" + index].Merge = true;
                    worksheet.Cells["A" + index + ":H" + index].Style.Font.Italic = true;


                    worksheet.Cells["I" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["I" + index + ":L" + index].Value = "(Ký và ghi rõ họ tên)";
                    worksheet.Cells["I" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["I" + index + ":L" + index].Merge = true;
                    worksheet.Cells["I" + index + ":L" + index].Style.Font.Bold = true;



                    worksheet.Cells["M" + index + ":P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["M" + index + ":P" + index].Value = "Người Lập";
                    worksheet.Cells["M" + index + ":P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["M" + index + ":P" + index].Merge = true;
                    worksheet.Cells["M" + index + ":P" + index].Style.Font.Bold = true;

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        public async Task<List<LookupItemTongHopDichVuVo>> GetTongHopDichVuBaoCaoAsync(DropDownListRequestModel model)
        {
            var lstEntity = new List<LookupItemTongHopDichVuVo>();
            //if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            //{
                var lstDichVuKhamBenh = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                    .Select(p => new LookupItemTongHopDichVuVo
                    {
                        DisplayName = p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        KeyId = p.Id,
                        NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh
                    }).ApplyLike(model.Query?.Trim(), x => x.Ma, x => x.Ten)
                    .Take(model.Take)
                    .ToListAsync();


                var lstDichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Select(p => new LookupItemTongHopDichVuVo
                    {
                        DisplayName = p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        KeyId = p.Id,
                        NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat
                    })
                    .ApplyLike(model.Query?.Trim(), x=> x.Ma, x => x.Ten)
                    .Take(model.Take)
                    .ToListAsync();

                var lstChePhamMau = await _mauVaChePhamRepository.TableNoTracking
                    .Select(p => new LookupItemTongHopDichVuVo
                    {
                        DisplayName = p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        KeyId = p.Id,
                        NhomDichVu = Enums.EnumNhomGoiDichVu.TruyenMau
                    }).ApplyLike(model.Query?.Trim(), x => x.Ma, x => x.Ten)
                    .Take(model.Take)
                    .ToListAsync();

                lstEntity.AddRange(lstDichVuKhamBenh);
                lstEntity.AddRange(lstDichVuKyThuat);
                lstEntity.AddRange(lstChePhamMau);
            //}
            //else
            //{
            //    var lstColumnNameSearch = new List<string>();
            //    lstColumnNameSearch.Add("Ten");
            //    lstColumnNameSearch.Add("Ma");


            //    var lstDichVuBenhVienTongHop = await _dichVuBenhVienTongHopRepository
            //        .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch)
            //        .Take(model.Take).ToListAsync();

            //    var lstDVKBId = lstDichVuBenhVienTongHop
            //        .Where(p => p.LoaiDichVuBenhVien == Enums.EnumDichVuTongHop.KhamBenh)
            //        .Select(p => p.DichVuKhamBenhBenhVienId).ToList();
            //    var lstDichVuKhamBenhBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
            //            .Where(p => lstDVKBId.Contains(p.Id))
            //            .Select(p => new LookupItemTongHopDichVuVo
            //            {
            //                DisplayName = p.Ten,
            //                Ten = p.Ten,
            //                Ma = p.Ma,
            //                KeyId = p.Id,
            //                NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh
            //            })
            //            .ToListAsync();

            //    var lstDVKTId = lstDichVuBenhVienTongHop
            //        .Where(p => p.LoaiDichVuBenhVien == Enums.EnumDichVuTongHop.KyThuat)
            //        .Select(p => p.DichVuKyThuatBenhVienId).ToList();
            //    var lstDichVuKyThuatBenhVien = await _dichVuKyThuatBenhVienRepository.TableNoTracking
            //            .Where(p => lstDVKTId.Contains(p.Id))
            //            .Select(p => new LookupItemTongHopDichVuVo
            //            {
            //                DisplayName = p.Ten,
            //                Ten = p.Ten,
            //                Ma = p.Ma,
            //                KeyId = p.Id,
            //                NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat
            //            })
            //            .ToListAsync();

            //    var lstMauVaChePhamId = await _mauVaChePhamRepository
            //        .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.MauVaChePhams.MauVaChePham), lstColumnNameSearch)
            //        .Select(x => x.Id)
            //        .Take(model.Take)
            //        .ToListAsync();
            //    var lstMauVaChePham = await _mauVaChePhamRepository.TableNoTracking
            //        .OrderBy(p => lstMauVaChePhamId.IndexOf(p.Id) != -1 ? lstMauVaChePhamId.IndexOf(p.Id) : model.Take + 1)
            //        .Select(item => new LookupItemTongHopDichVuVo
            //        {
            //            DisplayName = item.Ten,
            //            KeyId = item.Id,
            //            Ten = item.Ten,
            //            Ma = item.Ma,
            //            NhomDichVu = Enums.EnumNhomGoiDichVu.TruyenMau
            //        }).ToListAsync();


            //    lstEntity.AddRange(lstDichVuKhamBenhBenhVien);
            //    lstEntity.AddRange(lstDichVuKyThuatBenhVien);
            //    lstEntity.AddRange(lstMauVaChePham);
            //}

            return lstEntity.Take(model.Take).ToList();
        }
    }
}
