using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        private async Task<(List<ChiTietThucHienDichVuGridVo>, int)> GetDataHoatDongKhamBenhAsync(BaoCaoHoatDongKhamBenhTimKiemVo timKiemNangCaoObj, bool isTotalPage = false)
        {
            var lstHoatDongKham = new List<ChiTietThucHienDichVuGridVo>();
            if (isTotalPage)
            {
                var lstNoiDungBaoCao = EnumHelper.GetListEnum<Enums.NoiDungBaoCaoHoatDongKhamBenh>().Select(item => item).ToList();
                return (lstHoatDongKham, lstNoiDungBaoCao.Count);
            }
            else
            {
                //Cập nhật ngày 02/03/2022: Đổi cơ chế ICD KSK -> tất cả ICD có mã bắt đầu là Z00
                //var cauHinhIcdKhamSucKhoe = _cauHinhService.GetSetting("CauHinhKhamSucKhoe.IcdKhamSucKhoe");
                //long.TryParse(cauHinhIcdKhamSucKhoe?.Value, out long icdKhamSucKhoeId);
                var maICDKSK = "Z00";

                //var icdKhamSucKhoe = _icdRepository.TableNoTracking.FirstOrDefault(x => x.Id == icdKhamSucKhoeId);

                var cauHinhTinhHaNoi = _cauHinhService.GetSetting("CauHinhBaoCao.TinhHaNoi");
                long.TryParse(cauHinhTinhHaNoi?.Value, out long tinhHaNoiId);

                lstHoatDongKham = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham 
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                && timKiemNangCaoObj.NhomDichVuKhamBenhIds.Contains(x.DichVuKhamBenhBenhVienId)
                                && ((timKiemNangCaoObj.DangKham == timKiemNangCaoObj.DaHoanThanh)
                                    || (timKiemNangCaoObj.DangKham 
                                        && (x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham || x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh || x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan))
                                    || (timKiemNangCaoObj.DaHoanThanh && x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham))

                                && x.ThoiDiemThucHien != null
                                && x.ThoiDiemThucHien >= timKiemNangCaoObj.TuNgayTimKiemData
                                && x.ThoiDiemThucHien <= timKiemNangCaoObj.DenNgayTimKiemData)
                    .Select(item => new ChiTietThucHienDichVuGridVo()
                    {
                        //LaKhamSucKhoe = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe 
                        //                || (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && (item.ChanDoanSoBoICDId == icdKhamSucKhoeId || item.IcdchinhId == icdKhamSucKhoeId)),
                        //LaKhamBenh = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && item.ChanDoanSoBoICDId != icdKhamSucKhoeId && item.IcdchinhId != icdKhamSucKhoeId,

                        //Cập nhật ngày 02/03/2022: Đổi cơ chế ICD KSK -> tất cả ICD có mã bắt đầu là Z00
                        LaKhamSucKhoe = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                        || (item.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && (item.Icdchinh.Ma ?? item.ChanDoanSoBoICD.Ma ?? "").Contains(maICDKSK)),
                        LaKhamBenh = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe 
                                     && !(item.Icdchinh.Ma ?? item.ChanDoanSoBoICD.Ma ?? "").Contains(maICDKSK),

                        LaKhamTiemChung = false,

                        MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        TenDichVu = item.TenDichVu,
                        TrangThaiKhamBenh = item.TrangThai,
                        NoiThucHienId = item.NoiThucHienId.Value,
                        TenNoiThucHien = item.NoiThucHien.Ten,
                        LaNgoaiVien = item.NoiThucHien.HopDongKhamSucKhoeId != null,
                        CoBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                        ThoiDiemThucHien = item.ThoiDiemThucHien.Value,

                        NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                        ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,

                        LaKhamCapCuu = item.YeuCauNhapViens.Any(a => (a.YeuCauTiepNhans.All(b => b.NoiTruBenhAn == null) && a.LaCapCuu) || (a.YeuCauTiepNhans.Any(b => b.NoiTruBenhAn.LaCapCuu))),


                        TuVong = item.CoTuVong == true || item.YeuCauNhapViens.Any(a => a.YeuCauTiepNhans.Any(b => b.NoiTruBenhAn != null && b.NoiTruBenhAn.KetQuaDieuTri == Enums.EnumKetQuaDieuTri.TuVong)),
                        ChuyenVien = item.CoChuyenVien == true || item.YeuCauNhapViens.Any(a => a.YeuCauTiepNhans.Any(b => b.NoiTruBenhAn != null && b.NoiTruBenhAn.HinhThucRaVien == Enums.EnumHinhThucRaVien.ChuyenVien)),
                        LaNguoiBenhDiaChiKhacHaNoi = tinhHaNoiId != item.YeuCauTiepNhan.TinhThanhId,

                        TongNguoiBenh = 1,

                        //Cập nhật 19/07/2022: sửa logic số người khám bệnh tại mục 1b thành đếm theo mã người bệnh
                        // logic cũ: đếm theo mã YCTN với dịch vụ khám đầu tiên
                        MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN
                    })
                    .Union(
                        _yeuCauDichVuKyThuatRepository.TableNoTracking
                            .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien

                                        && ((x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung) // && x.KhamSangLocTiemChung.KetLuan == Enums.LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem)
                                            || (x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung && x.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null))
                                            
                                        && ((timKiemNangCaoObj.DangKham == timKiemNangCaoObj.DaHoanThanh)
                                            || (timKiemNangCaoObj.DangKham 
                                                && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung 
                                                    ? (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null || x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId == null) 
                                                    : x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien))
                                            || (timKiemNangCaoObj.DaHoanThanh 
                                                && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung 
                                                    ? (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null) 
                                                    : x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)))

                                        && (x.ThoiDiemThucHien ?? x.ThoiDiemKetLuan ?? x.ThoiDiemHoanThanh) >= timKiemNangCaoObj.TuNgayTimKiemData
                                        && (x.ThoiDiemThucHien ?? x.ThoiDiemKetLuan ?? x.ThoiDiemHoanThanh) <= timKiemNangCaoObj.DenNgayTimKiemData)
                            .Select(item => new ChiTietThucHienDichVuGridVo()
                            {
                                LaKhamSucKhoe = false,
                                LaKhamBenh = false,
                                LaKhamTiemChung = true,
                                LaKhamSangLoc = item.YeuCauDichVuKyThuatKhamSangLocTiemChungId == null,

                                MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                                TenDichVu = item.TenDichVu,
                                //TrangThaiKhamBenh = item.TrangThai,
                                DaHoanThanhKhamTiemVacxin = item.KhamSangLocTiemChung != null && item.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && item.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null,
                                DuDieuKienTiemChung = item.KhamSangLocTiemChung != null && item.KhamSangLocTiemChung.KetLuan == Enums.LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem,
                                TrangThaiTiemChung = item.TiemChung != null ? item.TiemChung.TrangThaiTiemChung : Enums.TrangThaiTiemChung.ChuaTiemChung,
                                NoiThucHienId = item.NoiThucHienId.Value,
                                TenNoiThucHien = item.NoiThucHien.Ten,
                                LaNgoaiVien = item.NoiThucHien.HopDongKhamSucKhoeId != null,
                                CoBHYT = item.BaoHiemChiTra != null && item.BaoHiemChiTra == true,
                                ThoiDiemThucHien = (item.ThoiDiemThucHien ?? item.ThoiDiemKetLuan ?? item.ThoiDiemHoanThanh).Value,

                                NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                                ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                                NamSinh = item.YeuCauTiepNhan.NamSinh,

                                LaKhamCapCuu = false,
                                TuVong = false,
                                ChuyenVien = false,
                                LaNguoiBenhDiaChiKhacHaNoi = tinhHaNoiId != item.YeuCauTiepNhan.TinhThanhId,

                                TongNguoiBenh = 1
                            })
                        ).ToList();
            }
            return (lstHoatDongKham, 0);
        }

        private void GanThongTinKhamTheoDichVuKham(ChiTietThucHienDichVuGridVo thongTinKham, BaoCaoHoatDongKhamBenhTheoDichVuGridVo gridItemBaoCao, DichVuKhamTheoNhomId thongTinDichVuKhamId, 
            BaoCaoHoatDongKhamBenhTheoDichVuGridVo gridItemTongBaoCao = null)
        {
            var nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamCapCuu;

            if (thongTinDichVuKhamId.lstDichVuKhamCapCuuId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamCapCuu;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamNoiId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNoi;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamNhiId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNhi;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamTMHId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamTMH;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamRHMId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamRHM;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamMatId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamMat;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamNgoaiId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNgoai;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamDaLieuId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamDaLieu;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamPhuSanId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamPhuSan;
            }
            else if (thongTinDichVuKhamId.lstDichVuKhamThamMyId.Contains(thongTinKham.DichVuBenhVienId))
            {
                nhomDichVuKham = Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamThamMy;
            }

            #region //Cập nhật 19/07/2022: sửa logic số người khám bệnh tại mục 1b thành đếm theo mã người bệnh
            var nguoiBenhChuaDemSoLuong = true;
            if (gridItemBaoCao.LaDemTongNguoiKhamBenh1b)
            {
                if (gridItemBaoCao.NguoiBenhDaDemSoLuongs.Any(a => a.MaBN == thongTinKham.MaBN && a.NhomDichVu == nhomDichVuKham))
                {
                    nguoiBenhChuaDemSoLuong = false;
                }
                else
                {
                    gridItemBaoCao.NguoiBenhDaDemSoLuongs.Add(new NguoiBenhKhamBenhDaDemSoLuongVo()
                    {
                        MaBN = thongTinKham.MaBN,
                        NhomDichVu = nhomDichVuKham
                    });
                }
            }
            #endregion

            if (nguoiBenhChuaDemSoLuong)
            {
                switch (nhomDichVuKham)
                {
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamCapCuu:
                        gridItemBaoCao.KhamCapCuu += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamCapCuu += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNoi:
                        gridItemBaoCao.KhamNoi += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamNoi += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNhi:
                        gridItemBaoCao.KhamNhi += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamNhi += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamTMH:
                        gridItemBaoCao.KhamTaiMuiHong += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamTaiMuiHong += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamRHM:
                        gridItemBaoCao.KhamRangHamMat += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamRangHamMat += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamMat:
                        gridItemBaoCao.KhamMat += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamMat += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNgoai:
                        gridItemBaoCao.KhamNgoai += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamNgoai += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamDaLieu:
                        gridItemBaoCao.KhamDaLieu += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamDaLieu += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamPhuSan:
                        gridItemBaoCao.KhamPhuSan += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamPhuSan += thongTinKham.TongNguoiBenh;
                        }
                        break;
                    case Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamThamMy:
                        gridItemBaoCao.KhamThamMy += thongTinKham.TongNguoiBenh;
                        if (gridItemTongBaoCao != null)
                        {
                            gridItemTongBaoCao.KhamThamMy += thongTinKham.TongNguoiBenh;
                        }
                        break;
                }
            }
        }

        private void GanThongTinKhamTheoKhoaPhongKham(ChiTietThucHienDichVuGridVo thongTinKham, BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo gridItemBaoCao, BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo gridItemTongBaoCao = null)
        {
            gridItemBaoCao.TongSoTheoKhoaPhong += thongTinKham.TongNguoiBenh;
            var thongTinTheoPhong = new ThongTinKhamBenhTheoPhongVo()
            {
                PhongBenhVienId = thongTinKham.NoiThucHienId,
                TenPhongBenhVien = thongTinKham.TenNoiThucHien,
                SoLuong = thongTinKham.TongNguoiBenh
            };
            gridItemBaoCao.ThongTinKhamTheoPhongs.Add(thongTinTheoPhong);
            if (gridItemTongBaoCao != null)
            {
                gridItemTongBaoCao.TongSoTheoKhoaPhong += thongTinKham.TongNguoiBenh;
                gridItemTongBaoCao.ThongTinKhamTheoPhongs.Add(thongTinTheoPhong);
            }
        }

        #region Theo dịch vụ
        public async Task<GridDataSource> GetDataBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync(QueryInfo queryInfo)
        {
            var lstChiTietBaoCao = new List<BaoCaoHoatDongKhamBenhTheoDichVuGridVo>();
            var timKiemNangCaoObj = new BaoCaoHoatDongKhamBenhTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoHoatDongKhamBenhTimKiemVo>(queryInfo.AdditionalSearchString);
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

            if (tuNgay != null && denNgay != null)
            {
                timKiemNangCaoObj.TuNgayTimKiemData = tuNgay.Value;
                timKiemNangCaoObj.DenNgayTimKiemData = denNgay.Value;

                #region Xử lý get lên tất cả id của các nhóm dịch vụ khám trong báo cáo
                var lstDichVuKhamCapCuuId = new List<long>();
                var lstDichVuKhamNoiId = new List<long>();
                var lstDichVuKhamNhiId = new List<long>();
                var lstDichVuKhamTMHId = new List<long>();
                var lstDichVuKhamRHMId = new List<long>();
                var lstDichVuKhamMatId = new List<long>();
                var lstDichVuKhamNgoaiId = new List<long>();
                var lstDichVuKhamDaLieuId = new List<long>();
                var lstDichVuKhamPhuSanId = new List<long>();
                var lstDichVuKhamThamMyId = new List<long>();
                var lstMaCauHinhNhomKham = EnumHelper.GetListEnum<Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh>().Select(item => item.GetDescription()).ToList();
                var lstCauHinhNhomKham = _cauHinhRepository.TableNoTracking
                    .Where(x => lstMaCauHinhNhomKham.Contains(x.Name) && x.DataType == Enums.DataType.String).ToList();



                var cauHinhNhomKhamCapCuu = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamCapCuu.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamCapCuu");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamCapCuu?.Value ?? null))
                {
                    lstDichVuKhamCapCuuId = cauHinhNhomKhamCapCuu.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamNoi = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNoi.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamNoi");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamNoi?.Value ?? null))
                {
                    lstDichVuKhamNoiId = cauHinhNhomKhamNoi.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamNhi = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNhi.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamNhi");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamNhi?.Value ?? null))
                {
                    lstDichVuKhamNhiId = cauHinhNhomKhamNhi.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamTMH = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamTMH.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamTMH");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamTMH?.Value ?? null))
                {
                    lstDichVuKhamTMHId = cauHinhNhomKhamTMH.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamRHM = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamRHM.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamRHM");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamRHM?.Value ?? null))
                {
                    lstDichVuKhamRHMId = cauHinhNhomKhamRHM.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamMat = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamMat.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamMat");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamMat?.Value ?? null))
                {
                    lstDichVuKhamMatId = cauHinhNhomKhamMat.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamNgoai = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNgoai.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamNgoai");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamNgoai?.Value ?? null))
                {
                    lstDichVuKhamNgoaiId = cauHinhNhomKhamNgoai.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamDaLieu = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamDaLieu.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamDaLieu");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamDaLieu?.Value ?? null))
                {
                    lstDichVuKhamDaLieuId = cauHinhNhomKhamDaLieu.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamPhuSan = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamPhuSan.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamPhuSan");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamPhuSan?.Value ?? null))
                {
                    lstDichVuKhamPhuSanId = cauHinhNhomKhamPhuSan.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamThamMy = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamThamMy.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamThamMy");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamThamMy?.Value ?? null))
                {
                    lstDichVuKhamThamMyId = cauHinhNhomKhamThamMy.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }
                #endregion

                timKiemNangCaoObj.NhomDichVuKhamBenhIds.AddRange(lstDichVuKhamCapCuuId
                                                                        .Union(lstDichVuKhamNoiId)
                                                                        .Union(lstDichVuKhamNhiId)
                                                                        .Union(lstDichVuKhamTMHId)
                                                                        .Union(lstDichVuKhamRHMId)
                                                                        .Union(lstDichVuKhamMatId)
                                                                        .Union(lstDichVuKhamNgoaiId)
                                                                        .Union(lstDichVuKhamDaLieuId)
                                                                        .Union(lstDichVuKhamPhuSanId)
                                                                        .Union(lstDichVuKhamThamMyId).ToList());
                var data = await GetDataHoatDongKhamBenhAsync(timKiemNangCaoObj);
                if (data.Item1.Count > 0)
                {

                    var thongTinDichVuKhamId = new DichVuKhamTheoNhomId()
                    {
                        lstDichVuKhamCapCuuId = lstDichVuKhamCapCuuId,
                        lstDichVuKhamNoiId = lstDichVuKhamNoiId,
                        lstDichVuKhamNhiId = lstDichVuKhamNhiId,
                        lstDichVuKhamTMHId = lstDichVuKhamTMHId,
                        lstDichVuKhamRHMId = lstDichVuKhamRHMId,
                        lstDichVuKhamMatId = lstDichVuKhamMatId,
                        lstDichVuKhamNgoaiId = lstDichVuKhamNgoaiId,
                        lstDichVuKhamDaLieuId = lstDichVuKhamDaLieuId,
                        lstDichVuKhamPhuSanId = lstDichVuKhamPhuSanId,
                        lstDichVuKhamThamMyId = lstDichVuKhamThamMyId,
                    };

                    var lstThongTinKhamBenh = data.Item1.Where(x => x.LaKhamBenh || x.LaKhamSucKhoe).ToList();
                    var lstThongTinKhamTiemChung = data.Item1.Where(x => x.LaKhamTiemChung).ToList();

                    //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                    var lstMaYCTNCoKSK = data.Item1.Where(x => x.LaKhamSucKhoe).Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();

                    #region 1. Tổng số người khám

                    var tongSoNguoiKhamItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKham
                    };
                    var tongSoNguoiKhamKSKItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKhamSucKhoe
                    };
                    var tongSoNguoiKhamKSKNoiVienItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKhamSucKhoeNoiVien
                    };
                    var tongSoNguoiKhamKSKNgoaiVienItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKhamSucKhoeNgoaiVien
                    };
                    var tongSoNguoiKhamBenhItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKhamBenh
                    };

                    #region Xử lý theo KSK
                    // lấy dịch vụ khám có số người thực hiện nhiều nhất theo dịch vụ khám

                    var soLanKhamNoiVien = lstThongTinKhamBenh
                        .Where(x => x.LaKhamSucKhoe && !x.LaNgoaiVien)
                        .GroupBy(x => x.DichVuBenhVienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            DichVuBenhVienId = item.Key,
                            TenDichVu = item.First().TenDichVu,

                            TongNguoiBenh = item.Select(x => x.MaYeuCauTiepNhan).Distinct().Count()
                        })
                        .OrderByDescending(x => x.TongNguoiBenh)
                        .FirstOrDefault();

                    var soLanKhamNgoaiVien = lstThongTinKhamBenh
                        .Where(x => x.LaKhamSucKhoe && x.LaNgoaiVien)
                        .GroupBy(x => x.DichVuBenhVienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            DichVuBenhVienId = item.Key,
                            TenDichVu = item.First().TenNoiThucHien,

                            TongNguoiBenh = item.Select(x => x.MaYeuCauTiepNhan).Distinct().Count()
                        })
                        .OrderByDescending(x => x.TongNguoiBenh)
                        .FirstOrDefault();
                    #endregion

                    #region Xử lý theo khám bệnh

                    #region logic cũ
                    // người bệnh khám nhiều dịch vụ, thì lấy dv thực hiện khám đầu tiên
                    //var groupTheoTiepNhan = lstThongTinKhamBenh.OrderBy(x => x.ThoiDiemThucHien).GroupBy(x => x.MaYeuCauTiepNhan)
                    //    .Select(item => new ChiTietThucHienDichVuGridVo
                    //    {
                    //        LaKhamSucKhoe = item.Any(x => x.LaKhamSucKhoe),
                    //        LaKhamBenh = item.All(x => x.LaKhamBenh),
                    //        LaKhamTiemChung = item.First().LaKhamTiemChung,

                    //        MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                    //        DichVuBenhVienId = item.First().DichVuBenhVienId,
                    //        TenDichVu = item.First().TenDichVu,
                    //        TrangThaiKhamBenh = item.First().TrangThaiKhamBenh,
                    //        NoiThucHienId = item.First().NoiThucHienId,
                    //        TenNoiThucHien = item.First().TenNoiThucHien,
                    //        LaNgoaiVien = item.First().LaNgoaiVien,
                    //        CoBHYT = item.First().CoBHYT,
                    //        ThoiDiemThucHien = item.First().ThoiDiemThucHien,

                    //        NgaySinh = item.First().NgaySinh,
                    //        ThangSinh = item.First().ThangSinh,
                    //        NamSinh = item.First().NamSinh,

                    //        LaKhamCapCuu = item.First().LaKhamCapCuu,

                    //        TuVong = item.First().TuVong,
                    //        ChuyenVien = item.First().ChuyenVien,
                    //        LaNguoiBenhDiaChiKhacHaNoi = item.First().LaNguoiBenhDiaChiKhacHaNoi,

                    //        TongNguoiBenh = 1
                    //    })
                    //    .ToList();

                    //var thongTinSoLanKhamBenhs = groupTheoTiepNhan
                    //    .Where(x => x.LaKhamBenh)
                    //    .GroupBy(x => x.DichVuBenhVienId)
                    //    .Select(item => new ChiTietThucHienDichVuGridVo
                    //    {
                    //        LaKhamSucKhoe = item.First().LaKhamSucKhoe,
                    //        LaKhamBenh = item.First().LaKhamBenh,
                    //        LaKhamTiemChung = item.First().LaKhamTiemChung,

                    //        MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                    //        DichVuBenhVienId = item.First().DichVuBenhVienId,
                    //        TenDichVu = item.First().TenDichVu,
                    //        TrangThaiKhamBenh = item.First().TrangThaiKhamBenh,
                    //        NoiThucHienId = item.First().NoiThucHienId,
                    //        TenNoiThucHien = item.First().TenNoiThucHien,
                    //        LaNgoaiVien = item.First().LaNgoaiVien,
                    //        CoBHYT = item.First().CoBHYT,
                    //        ThoiDiemThucHien = item.First().ThoiDiemThucHien,

                    //        NgaySinh = item.First().NgaySinh,
                    //        ThangSinh = item.First().ThangSinh,
                    //        NamSinh = item.First().NamSinh,

                    //        LaKhamCapCuu = item.First().LaKhamCapCuu,

                    //        TuVong = item.First().TuVong,
                    //        ChuyenVien = item.First().ChuyenVien,
                    //        LaNguoiBenhDiaChiKhacHaNoi = item.First().LaNguoiBenhDiaChiKhacHaNoi,

                    //        TongNguoiBenh = item.Count()
                    //    })
                    //    .ToList();

                    //var soLanKhamBenhs = thongTinSoLanKhamBenhs.Where(x => !x.LaKhamSucKhoe).ToList();
                    #endregion

                    #region logic mới
                    //Cập nhật 19/07/2022: sửa logic số người khám bệnh tại mục 1b thành đếm theo mã người bệnh
                    // logic cũ: đếm theo mã YCTN với dịch vụ khám đầu tiên

                    //        LaKhamSucKhoe = item.Any(x => x.LaKhamSucKhoe),
                    //        LaKhamBenh = item.All(x => x.LaKhamBenh),
                    //        LaKhamTiemChung = item.First().LaKhamTiemChung,

                    var lstThongTinNguoiBenhKhamBenh = lstThongTinKhamBenh
                        .GroupBy(x => x.MaYeuCauTiepNhan)
                        .Where(x => !x.Any(a => a.LaKhamSucKhoe)
                                    && x.All(a => a.LaKhamBenh))
                        .SelectMany(x => x).ToList();

                    var thongTinSoLanKhamBenhs = lstThongTinNguoiBenhKhamBenh
                        .Where(x => x.LaKhamBenh
                                    && !x.LaKhamSucKhoe
                                    && !string.IsNullOrEmpty(x.MaBN))
                        .GroupBy(x => new { x.MaBN, x.DichVuBenhVienId })
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            LaKhamSucKhoe = item.First().LaKhamSucKhoe,
                            LaKhamBenh = item.First().LaKhamBenh,
                            LaKhamTiemChung = item.First().LaKhamTiemChung,

                            MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                            DichVuBenhVienId = item.Key.DichVuBenhVienId,
                            TenDichVu = item.First().TenDichVu,
                            TrangThaiKhamBenh = item.First().TrangThaiKhamBenh,
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,
                            LaNgoaiVien = item.First().LaNgoaiVien,
                            CoBHYT = item.First().CoBHYT,
                            ThoiDiemThucHien = item.First().ThoiDiemThucHien,

                            NgaySinh = item.First().NgaySinh,
                            ThangSinh = item.First().ThangSinh,
                            NamSinh = item.First().NamSinh,

                            LaKhamCapCuu = item.First().LaKhamCapCuu,

                            TuVong = item.First().TuVong,
                            ChuyenVien = item.First().ChuyenVien,
                            LaNguoiBenhDiaChiKhacHaNoi = item.First().LaNguoiBenhDiaChiKhacHaNoi,

                            TongNguoiBenh = 1,

                            MaBN = item.Key.MaBN
                        })
                        .ToList();

                    var soLanKhamBenhs = thongTinSoLanKhamBenhs.ToList();
                    #endregion
                    #endregion




                    if (soLanKhamNoiVien != null)
                    {
                        GanThongTinKhamTheoDichVuKham(soLanKhamNoiVien, tongSoNguoiKhamKSKNoiVienItem, thongTinDichVuKhamId, tongSoNguoiKhamKSKItem);
                    }
                    if (soLanKhamNgoaiVien != null)
                    {
                        GanThongTinKhamTheoDichVuKham(soLanKhamNgoaiVien, tongSoNguoiKhamKSKNgoaiVienItem, thongTinDichVuKhamId, tongSoNguoiKhamKSKItem);
                    }
                    if (soLanKhamBenhs.Any())
                    {
                        tongSoNguoiKhamBenhItem.LaDemTongNguoiKhamBenh1b = true;
                        foreach (var soLanKhamBenh in soLanKhamBenhs)
                        {
                            GanThongTinKhamTheoDichVuKham(soLanKhamBenh, tongSoNguoiKhamBenhItem, thongTinDichVuKhamId);
                        }
                    }

                    tongSoNguoiKhamItem.KhamCapCuu = tongSoNguoiKhamKSKItem.KhamCapCuu + tongSoNguoiKhamBenhItem.KhamCapCuu;
                    tongSoNguoiKhamItem.KhamNoi = tongSoNguoiKhamKSKItem.KhamNoi + tongSoNguoiKhamBenhItem.KhamNoi;
                    tongSoNguoiKhamItem.KhamNhi = tongSoNguoiKhamKSKItem.KhamNhi + tongSoNguoiKhamBenhItem.KhamNhi;
                    tongSoNguoiKhamItem.KhamTaiMuiHong = tongSoNguoiKhamKSKItem.KhamTaiMuiHong + tongSoNguoiKhamBenhItem.KhamTaiMuiHong;
                    tongSoNguoiKhamItem.KhamRangHamMat = tongSoNguoiKhamKSKItem.KhamRangHamMat + tongSoNguoiKhamBenhItem.KhamRangHamMat;
                    tongSoNguoiKhamItem.KhamMat = tongSoNguoiKhamKSKItem.KhamMat + tongSoNguoiKhamBenhItem.KhamMat;
                    tongSoNguoiKhamItem.KhamNgoai = tongSoNguoiKhamKSKItem.KhamNgoai + tongSoNguoiKhamBenhItem.KhamNgoai;
                    tongSoNguoiKhamItem.KhamDaLieu = tongSoNguoiKhamKSKItem.KhamDaLieu + tongSoNguoiKhamBenhItem.KhamDaLieu;
                    tongSoNguoiKhamItem.KhamPhuSan = tongSoNguoiKhamKSKItem.KhamPhuSan + tongSoNguoiKhamBenhItem.KhamPhuSan;
                    tongSoNguoiKhamItem.KhamThamMy = tongSoNguoiKhamKSKItem.KhamThamMy + tongSoNguoiKhamBenhItem.KhamThamMy;

                    lstChiTietBaoCao.Add(tongSoNguoiKhamItem);
                    lstChiTietBaoCao.Add(tongSoNguoiKhamKSKItem);
                    lstChiTietBaoCao.Add(tongSoNguoiKhamKSKNoiVienItem);
                    lstChiTietBaoCao.Add(tongSoNguoiKhamKSKNgoaiVienItem);
                    lstChiTietBaoCao.Add(tongSoNguoiKhamBenhItem);

                    #endregion

                    #region 2. Tổng số lần
                    var tongSoLanKhamItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKham
                    };
                    var tongSoLanKhamKSKItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamSucKhoe
                    };
                    var tongSoLanKhamKSKNoiVienItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamSucKhoeNoiVien
                    };
                    var tongSoLanKhamKSKNgoaiVienItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamSucKhoeNgoaiVien
                    };
                    var tongSoLanKhamBenhItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamBenh
                    };
                    var tongSoLanKhamBenhBHYTItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamBenhBHYT
                    };
                    var tongSoLanKhamBenhVienPhiItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamBenhVienPhi
                    };
                    var tongSoLanKhamBenhKhongThuDuocItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamBenhKhongThuDuoc
                    };

                    foreach (var thongTinKhamBenh in lstThongTinKhamBenh)
                    {
                        //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                        if (thongTinKhamBenh.LaKhamSucKhoe || lstMaYCTNCoKSK.Contains(thongTinKhamBenh.MaYeuCauTiepNhan))
                        {
                            GanThongTinKhamTheoDichVuKham(thongTinKhamBenh, thongTinKhamBenh.LaNgoaiVien ? tongSoLanKhamKSKNgoaiVienItem : tongSoLanKhamKSKNoiVienItem, thongTinDichVuKhamId, tongSoLanKhamKSKItem);
                        }
                        else
                        {
                            GanThongTinKhamTheoDichVuKham(thongTinKhamBenh, thongTinKhamBenh.CoBHYT ? tongSoLanKhamBenhBHYTItem : tongSoLanKhamBenhVienPhiItem, thongTinDichVuKhamId, tongSoLanKhamBenhItem);
                        }
                    }

                    tongSoLanKhamItem.KhamCapCuu = tongSoLanKhamKSKItem.KhamCapCuu + tongSoLanKhamBenhItem.KhamCapCuu;
                    tongSoLanKhamItem.KhamNoi = tongSoLanKhamKSKItem.KhamNoi + tongSoLanKhamBenhItem.KhamNoi;
                    tongSoLanKhamItem.KhamNhi = tongSoLanKhamKSKItem.KhamNhi + tongSoLanKhamBenhItem.KhamNhi;
                    tongSoLanKhamItem.KhamTaiMuiHong = tongSoLanKhamKSKItem.KhamTaiMuiHong + tongSoLanKhamBenhItem.KhamTaiMuiHong;
                    tongSoLanKhamItem.KhamRangHamMat = tongSoLanKhamKSKItem.KhamRangHamMat + tongSoLanKhamBenhItem.KhamRangHamMat;
                    tongSoLanKhamItem.KhamMat = tongSoLanKhamKSKItem.KhamMat + tongSoLanKhamBenhItem.KhamMat;
                    tongSoLanKhamItem.KhamNgoai = tongSoLanKhamKSKItem.KhamNgoai + tongSoLanKhamBenhItem.KhamNgoai;
                    tongSoLanKhamItem.KhamDaLieu = tongSoLanKhamKSKItem.KhamDaLieu + tongSoLanKhamBenhItem.KhamDaLieu;
                    tongSoLanKhamItem.KhamPhuSan = tongSoLanKhamKSKItem.KhamPhuSan + tongSoLanKhamBenhItem.KhamPhuSan;
                    tongSoLanKhamItem.KhamThamMy = tongSoLanKhamKSKItem.KhamThamMy + tongSoLanKhamBenhItem.KhamThamMy;

                    lstChiTietBaoCao.Add(tongSoLanKhamItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamKSKItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamKSKNoiVienItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamKSKNgoaiVienItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamBenhItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamBenhBHYTItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamBenhVienPhiItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamBenhKhongThuDuocItem);
                    #endregion

                    #region 3. Trong TS lần khám bệnh
                    var trongTongSoLanKhamBenhItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenh,
                        LaItemTieuDe = true
                    };
                    var trongTongSoLanKhamBenhCapCuuItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenhCapCuu
                    };
                    var trongTongSoLanKhamBenhTreEmItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenhTreEm
                    };
                    var trongTongSoLanKhamBenTreEmDuoi6Tuoi = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenhTreEmDuoi6Tuoi
                    };
                    var trongTongSoLanKhamBenhNgoaiTinh = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenhNgoaiTinh
                    };

                    var thongTinKhamLaTreEms = lstThongTinKhamBenh
                        .Where(x => (x.LaKhamBenh) // || x.LaKhamSucKhoe) 

                                    //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                                    && !lstMaYCTNCoKSK.Contains(x.MaYeuCauTiepNhan)

                                    && x.LaTreEm)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                            DichVuBenhVienId = item.DichVuBenhVienId,

                            TongNguoiBenh = 1
                        })
                        .ToList();

                    var thongTinKhamLaTreEmDuoi6Tuois = lstThongTinKhamBenh
                        .Where(x => (x.LaKhamBenh) // || x.LaKhamSucKhoe) 

                                    //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                                    && !lstMaYCTNCoKSK.Contains(x.MaYeuCauTiepNhan)

                                    && x.LaTreEmDuoi6Tuoi)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                            DichVuBenhVienId = item.DichVuBenhVienId,

                            TongNguoiBenh = 1
                        })
                        .ToList();

                    var thongTinKhamLaNgoaiTinhs = lstThongTinKhamBenh
                        .Where(x => (x.LaKhamBenh) // || x.LaKhamSucKhoe) 

                                    //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                                    && !lstMaYCTNCoKSK.Contains(x.MaYeuCauTiepNhan)

                                    && x.LaNguoiBenhDiaChiKhacHaNoi)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                            DichVuBenhVienId = item.DichVuBenhVienId,

                            TongNguoiBenh = 1
                        })
                        .ToList();

                    // thông tin cấp cứu, tạm thời chưa sử dụng
                    //var thongTinKhamCapCuus = lstThongTinKhamBenh
                    //    .Where(x => (x.LaKhamBenh) // || x.LaKhamSucKhoe) 
                    //                && x.LaKhamCapCuu)
                    //    .Select(item => new ChiTietThucHienDichVuGridVo
                    //    {
                    //        MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                    //        DichVuBenhVienId = item.DichVuBenhVienId,

                    //        TongNguoiBenh = 1
                    //    })
                    //    .ToList();

                    foreach (var thongTinKhamTreEm in thongTinKhamLaTreEms)
                    {
                        GanThongTinKhamTheoDichVuKham(thongTinKhamTreEm, trongTongSoLanKhamBenhTreEmItem, thongTinDichVuKhamId);
                    }
                    foreach (var thongTinKhamTreEmDuoi6Tuoi in thongTinKhamLaTreEmDuoi6Tuois)
                    {
                        GanThongTinKhamTheoDichVuKham(thongTinKhamTreEmDuoi6Tuoi, trongTongSoLanKhamBenTreEmDuoi6Tuoi, thongTinDichVuKhamId);
                    }
                    foreach (var thongTinKhamNgoiTinh in thongTinKhamLaNgoaiTinhs)
                    {
                        GanThongTinKhamTheoDichVuKham(thongTinKhamNgoiTinh, trongTongSoLanKhamBenhNgoaiTinh, thongTinDichVuKhamId);
                    }


                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenhItem);
                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenhCapCuuItem);
                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenhTreEmItem);
                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenTreEmDuoi6Tuoi);
                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenhNgoaiTinh);
                    #endregion

                    #region 4. TS người bệnh chuyển viện
                    var lstThongTinKhamChuyenVien = lstThongTinKhamBenh
                        .Where(x => x.ChuyenVien)
                        .OrderBy(x => x.ThoiDiemThucHien)
                        .GroupBy(x => x.MaYeuCauTiepNhan)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                            DichVuBenhVienId = item.First().DichVuBenhVienId,
                            ChuyenVien = true,

                            TongNguoiBenh = 1
                        })
                        .GroupBy(x => x.DichVuBenhVienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            DichVuBenhVienId = item.First().DichVuBenhVienId,

                            TongNguoiBenh = item.Count()
                        })
                        .ToList();
                    var tongSoNguoiChuyenVienItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhChuyenVien
                    };

                    foreach (var thongTinKhamChuyenVien in lstThongTinKhamChuyenVien)
                    {
                        GanThongTinKhamTheoDichVuKham(thongTinKhamChuyenVien, tongSoNguoiChuyenVienItem, thongTinDichVuKhamId);
                    }

                    lstChiTietBaoCao.Add(tongSoNguoiChuyenVienItem);
                    #endregion

                    #region 5. Số người bệnh tử Vong
                    var lstThongTinKhamTuVong = lstThongTinKhamBenh
                        .Where(x => x.TuVong)
                        .OrderBy(x => x.ThoiDiemThucHien)
                        .GroupBy(x => x.MaYeuCauTiepNhan)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                            DichVuBenhVienId = item.First().DichVuBenhVienId,
                            TuVong = true,

                            TongNguoiBenh = 1
                        })
                        .GroupBy(x => x.DichVuBenhVienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            DichVuBenhVienId = item.First().DichVuBenhVienId,

                            TongNguoiBenh = item.Count()
                        })
                        .ToList();
                    var tongSoNguoiTuVongItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhTuVong
                    };
                    var tongSoNguoiTuVongTruoc24HItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhTuVongTruoc24H
                    };
                    var tongSoNguoiTuVongNgoaiVienItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhTuVongNgoaiVien
                    };

                    foreach (var thongTinKhamTuVong in lstThongTinKhamTuVong)
                    {
                        GanThongTinKhamTheoDichVuKham(thongTinKhamTuVong, tongSoNguoiTuVongItem, thongTinDichVuKhamId);
                    }

                    lstChiTietBaoCao.Add(tongSoNguoiTuVongItem);
                    lstChiTietBaoCao.Add(tongSoNguoiTuVongTruoc24HItem);
                    lstChiTietBaoCao.Add(tongSoNguoiTuVongNgoaiVienItem);


                    #endregion

                    #region 6. TS người khám sàng lọc tiêm chủng
                    var tongSoNguoiBenhKhamSangLocTiemChungItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhKhamSangLocTiemChung
                    };
                    var tongNguoiBenhTiemChungItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhTiemChung
                    };
                    var tongMuiTiemItem = new BaoCaoHoatDongKhamBenhTheoDichVuGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongMuiTiem
                    };


                    tongSoNguoiBenhKhamSangLocTiemChungItem.KhamTiemChung = lstThongTinKhamTiemChung.Where(x => x.LaKhamSangLoc).GroupBy(x => x.MaYeuCauTiepNhan).Select(x => x.First().MaYeuCauTiepNhan).Count();
                    tongNguoiBenhTiemChungItem.KhamTiemChung = lstThongTinKhamTiemChung.Where(x => x.LaKhamSangLoc && x.DuDieuKienTiemChung).GroupBy(x => x.MaYeuCauTiepNhan).Select(x => x.First().MaYeuCauTiepNhan).Count();
                    tongMuiTiemItem.KhamTiemChung = lstThongTinKhamTiemChung.Count(x => !x.LaKhamSangLoc);

                    lstChiTietBaoCao.Add(tongSoNguoiBenhKhamSangLocTiemChungItem);
                    lstChiTietBaoCao.Add(tongNguoiBenhTiemChungItem);
                    lstChiTietBaoCao.Add(tongMuiTiemItem);
                    #endregion
                }
            }

            return new GridDataSource
            {
                Data = lstChiTietBaoCao.ToArray(),
                TotalRowCount = lstChiTietBaoCao.Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageBaoCaoHoatDongKhamBenhTheoDichVuForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoHoatDongKhamBenhTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoHoatDongKhamBenhTimKiemVo>(queryInfo.AdditionalSearchString);
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

            if (tuNgay != null && denNgay != null)
            {
                timKiemNangCaoObj.TuNgayTimKiemData = tuNgay.Value;
                timKiemNangCaoObj.DenNgayTimKiemData = denNgay.Value;
                var result = await GetDataHoatDongKhamBenhAsync(timKiemNangCaoObj, true);
                return new GridDataSource { TotalRowCount = result.Item2 };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoHoatDongKhamBenhTheoDichVu(GridDataSource gridDataSource, QueryInfo query)
        {
            var lstTiepNhanTheoNoiGioiThieu = new List<BaoCaoHoatDongKhamBenhTheoDichVuGridVo>();
            var timKiemNangCaoObj = new BaoCaoHoatDongKhamBenhTimKiemVo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoHoatDongKhamBenhTimKiemVo>(query.AdditionalSearchString);
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


            // var noiGioiThieu = _noiGioiThieuRepository.TableNoTracking.FirstOrDefault(x => x.Id == timKiemNangCaoObj.NoiGioiThieuId);

            var datas = (ICollection<BaoCaoHoatDongKhamBenhTheoDichVuGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO HOẠT ĐỘNG KHÁM BỆNH");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.DefaultColWidth = 15;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:L3"])
                    {
                        range.Worksheet.Cells["A3:L3"].Merge = true;
                        range.Worksheet.Cells["A3:L3"].Value = "BÁO CÁO HOẠT ĐỘNG KHÁM BỆNH";
                        range.Worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:L4"])
                    {
                        range.Worksheet.Cells["A4:L4"].Merge = true;
                            range.Worksheet.Cells["A4:L4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:L4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:N7"])
                    {
                        range.Worksheet.Cells["A7:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:N7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:N7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:N7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:N7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:N7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Tổng Số";

                        range.Worksheet.Cells["D7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D7"].Value = "Cấp Cứu";

                        range.Worksheet.Cells["E7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E7"].Value = "Nội";

                        range.Worksheet.Cells["F7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F7"].Value = "Nhi";

                        range.Worksheet.Cells["G7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G7"].Value = "TMH";

                        range.Worksheet.Cells["H7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H7"].Value = "RHM";

                        range.Worksheet.Cells["I7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I7"].Value = "Mắt";

                        range.Worksheet.Cells["J7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J7"].Value = "Ngoại";

                        range.Worksheet.Cells["K7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K7"].Value = "Da Liễu";

                        range.Worksheet.Cells["L7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L7"].Value = "Phụ Sản";

                        range.Worksheet.Cells["M7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M7"].Value = "Thẩm Mỹ";

                        range.Worksheet.Cells["N7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N7"].Value = "Tiêm Chủng";
                    }

                    //write data from line 8
                    int index = 8;
                    int stt = 1;
                    var formatCurrency = "#,##0.00";
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            // format border, font chữ,....
                            worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":N" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":N" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Value = item.STT;
                            worksheet.Cells["A" + index].Style.Font.Bold = item.InDamNoiDung;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = item.NoiDung;
                            worksheet.Cells["B" + index].Style.Font.Bold = item.InDamNoiDung;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Value = item.LaItemTieuDe ? (int?)null : item.TongSo;
                            worksheet.Cells["C" + index].Style.Font.Bold = item.InDamNoiDung;
                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamCapCuu;
                            worksheet.Cells["D" + index].Style.Font.Bold = item.InDamNoiDung;
                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamNoi;
                            worksheet.Cells["E" + index].Style.Font.Bold = item.InDamNoiDung;
                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamNhi;
                            worksheet.Cells["F" + index].Style.Font.Bold = item.InDamNoiDung;
                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamTaiMuiHong;
                            worksheet.Cells["G" + index].Style.Font.Bold = item.InDamNoiDung;
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamRangHamMat;
                            worksheet.Cells["H" + index].Style.Font.Bold = item.InDamNoiDung;
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamMat;
                            worksheet.Cells["I" + index].Style.Font.Bold = item.InDamNoiDung;
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamNgoai;
                            worksheet.Cells["J" + index].Style.Font.Bold = item.InDamNoiDung;

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamDaLieu;
                            worksheet.Cells["K" + index].Style.Font.Bold = item.InDamNoiDung;

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamPhuSan;
                            worksheet.Cells["L" + index].Style.Font.Bold = item.InDamNoiDung;

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamThamMy;
                            worksheet.Cells["M" + index].Style.Font.Bold = item.InDamNoiDung;

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Value = item.LaItemTieuDe ? (int?)null : item.KhamTiemChung;
                            worksheet.Cells["N" + index].Style.Font.Bold = item.InDamNoiDung;

                            stt++;
                            index++;
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        #endregion

        #region Theo Khoa Phòng
        public async Task<GridDataSource> GetDataBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync(QueryInfo queryInfo)
        {
            var lstChiTietBaoCao = new List<BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo>();
            var timKiemNangCaoObj = new BaoCaoHoatDongKhamBenhTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoHoatDongKhamBenhTimKiemVo>(queryInfo.AdditionalSearchString);
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

            if (tuNgay != null && denNgay != null)
            {
                timKiemNangCaoObj.TuNgayTimKiemData = tuNgay.Value;
                timKiemNangCaoObj.DenNgayTimKiemData = denNgay.Value;

                #region Xử lý get lên tất cả id của các nhóm dịch vụ khám trong báo cáo
                var lstDichVuKhamCapCuuId = new List<long>();
                var lstDichVuKhamNoiId = new List<long>();
                var lstDichVuKhamNhiId = new List<long>();
                var lstDichVuKhamTMHId = new List<long>();
                var lstDichVuKhamRHMId = new List<long>();
                var lstDichVuKhamMatId = new List<long>();
                var lstDichVuKhamNgoaiId = new List<long>();
                var lstDichVuKhamDaLieuId = new List<long>();
                var lstDichVuKhamPhuSanId = new List<long>();
                var lstDichVuKhamThamMyId = new List<long>();
                var lstMaCauHinhNhomKham = EnumHelper.GetListEnum<Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh>().Select(item => item.GetDescription()).ToList();
                var lstCauHinhNhomKham = _cauHinhRepository.TableNoTracking
                    .Where(x => lstMaCauHinhNhomKham.Contains(x.Name) && x.DataType == Enums.DataType.String).ToList();



                var cauHinhNhomKhamCapCuu = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamCapCuu.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamCapCuu");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamCapCuu?.Value ?? null))
                {
                    lstDichVuKhamCapCuuId = cauHinhNhomKhamCapCuu.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamNoi = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNoi.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamNoi");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamNoi?.Value ?? null))
                {
                    lstDichVuKhamNoiId = cauHinhNhomKhamNoi.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamNhi = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNhi.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamNhi");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamNhi?.Value ?? null))
                {
                    lstDichVuKhamNhiId = cauHinhNhomKhamNhi.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamTMH = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamTMH.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamTMH");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamTMH?.Value ?? null))
                {
                    lstDichVuKhamTMHId = cauHinhNhomKhamTMH.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamRHM = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamRHM.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamRHM");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamRHM?.Value ?? null))
                {
                    lstDichVuKhamRHMId = cauHinhNhomKhamRHM.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamMat = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamMat.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamMat");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamMat?.Value ?? null))
                {
                    lstDichVuKhamMatId = cauHinhNhomKhamMat.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamNgoai = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamNgoai.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamNgoai");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamNgoai?.Value ?? null))
                {
                    lstDichVuKhamNgoaiId = cauHinhNhomKhamNgoai.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamDaLieu = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamDaLieu.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamDaLieu");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamDaLieu?.Value ?? null))
                {
                    lstDichVuKhamDaLieuId = cauHinhNhomKhamDaLieu.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamPhuSan = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamPhuSan.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamPhuSan");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamPhuSan?.Value ?? null))
                {
                    lstDichVuKhamPhuSanId = cauHinhNhomKhamPhuSan.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }

                var cauHinhNhomKhamThamMy = lstCauHinhNhomKham.FirstOrDefault(x => x.Name == Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh.KhamThamMy.GetDescription()); //_cauHinhService.GetSetting("CauHinhBaoCao.NhomKhamThamMy");
                if (!string.IsNullOrEmpty(cauHinhNhomKhamThamMy?.Value ?? null))
                {
                    lstDichVuKhamThamMyId = cauHinhNhomKhamThamMy.Value.Split(";")
                        .Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList();
                }
                #endregion

                timKiemNangCaoObj.NhomDichVuKhamBenhIds.AddRange(lstDichVuKhamCapCuuId
                                                                        .Union(lstDichVuKhamNoiId)
                                                                        .Union(lstDichVuKhamNhiId)
                                                                        .Union(lstDichVuKhamTMHId)
                                                                        .Union(lstDichVuKhamRHMId)
                                                                        .Union(lstDichVuKhamMatId)
                                                                        .Union(lstDichVuKhamNgoaiId)
                                                                        .Union(lstDichVuKhamDaLieuId)
                                                                        .Union(lstDichVuKhamPhuSanId)
                                                                        .Union(lstDichVuKhamThamMyId).ToList());
                var data = await GetDataHoatDongKhamBenhAsync(timKiemNangCaoObj);
                if (data.Item1.Count > 0)
                {

                    var thongTinDichVuKhamId = new DichVuKhamTheoNhomId()
                    {
                        lstDichVuKhamCapCuuId = lstDichVuKhamCapCuuId,
                        lstDichVuKhamNoiId = lstDichVuKhamNoiId,
                        lstDichVuKhamNhiId = lstDichVuKhamNhiId,
                        lstDichVuKhamTMHId = lstDichVuKhamTMHId,
                        lstDichVuKhamRHMId = lstDichVuKhamRHMId,
                        lstDichVuKhamMatId = lstDichVuKhamMatId,
                        lstDichVuKhamNgoaiId = lstDichVuKhamNgoaiId,
                        lstDichVuKhamDaLieuId = lstDichVuKhamDaLieuId,
                        lstDichVuKhamPhuSanId = lstDichVuKhamPhuSanId,
                        lstDichVuKhamThamMyId = lstDichVuKhamThamMyId,
                    };

                    var lstThongTinKhamBenh = data.Item1.Where(x => x.LaKhamBenh || x.LaKhamSucKhoe).ToList();
                    var lstThongTinKhamTiemChung = data.Item1.Where(x => x.LaKhamTiemChung).ToList();

                    //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                    var lstMaYCTNCoKSK = data.Item1.Where(x => x.LaKhamSucKhoe).Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();

                    #region 1. Tổng số người khám

                    var tongSoNguoiKhamItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKham
                    };
                    var tongSoNguoiKhamKSKItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKhamSucKhoe
                    };
                    var tongSoNguoiKhamKSKNoiVienItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKhamSucKhoeNoiVien
                    };
                    var tongSoNguoiKhamKSKNgoaiVienItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKhamSucKhoeNgoaiVien
                    };
                    var tongSoNguoiKhamBenhItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoNguoiKhamBenh
                    };

                    #region Xử lý theo KSK
                    // lấy dịch vụ khám có số người thực hiện nhiều nhất theo khoa phòng

                    var soLanKhamNoiVien = lstThongTinKhamBenh
                        .Where(x => x.LaKhamSucKhoe && !x.LaNgoaiVien)
                        .GroupBy(x => x.NoiThucHienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            NoiThucHienId = item.Key,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = item.Select(x => x.MaYeuCauTiepNhan).Distinct().Count()
                        })
                        .OrderByDescending(x => x.TongNguoiBenh)
                        .FirstOrDefault();

                    var soLanKhamNgoaiVien = lstThongTinKhamBenh
                        .Where(x => x.LaKhamSucKhoe && x.LaNgoaiVien)
                        .GroupBy(x => x.NoiThucHienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            NoiThucHienId = item.Key,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = item.Select(x => x.MaYeuCauTiepNhan).Distinct().Count()
                        })
                        .OrderByDescending(x => x.TongNguoiBenh)
                        .FirstOrDefault();

                    //var soLanKhamNoiVien = thongTinSoLanKhamSucKhoes.Where(x => x.LaKhamSucKhoe && !x.LaNgoaiVien)
                    //    .OrderByDescending(x => x.TongNguoiBenh).FirstOrDefault();
                    //var soLanKhamNgoaiVien = thongTinSoLanKhamSucKhoes.Where(x => x.LaKhamSucKhoe && x.LaNgoaiVien)
                    //    .OrderByDescending(x => x.TongNguoiBenh).FirstOrDefault();
                    #endregion

                    #region Xử lý theo Khám bệnh

                    #region Logic cũ
                    // người bệnh khám nhiều dịch vụ khám, thì chỉ lấy dv khám thực hiện đầu tiên

                    //var groupTheoTiepNhan = lstThongTinKhamBenh.OrderBy(x => x.ThoiDiemThucHien)
                    //    .GroupBy(x => x.MaYeuCauTiepNhan)
                    //    .Select(item => new ChiTietThucHienDichVuGridVo
                    //    {
                    //        LaKhamSucKhoe = item.Any(x => x.LaKhamSucKhoe),
                    //        LaKhamBenh = item.All(x => x.LaKhamBenh),
                    //        LaKhamTiemChung = item.First().LaKhamTiemChung,

                    //        MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                    //        DichVuBenhVienId = item.First().DichVuBenhVienId,
                    //        TenDichVu = item.First().TenDichVu,
                    //        TrangThaiKhamBenh = item.First().TrangThaiKhamBenh,
                    //        NoiThucHienId = item.First().NoiThucHienId,
                    //        TenNoiThucHien = item.First().TenNoiThucHien,
                    //        LaNgoaiVien = item.First().LaNgoaiVien,
                    //        CoBHYT = item.First().CoBHYT,
                    //        ThoiDiemThucHien = item.First().ThoiDiemThucHien,

                    //        NgaySinh = item.First().NgaySinh,
                    //        ThangSinh = item.First().ThangSinh,
                    //        NamSinh = item.First().NamSinh,

                    //        LaKhamCapCuu = item.First().LaKhamCapCuu,

                    //        TuVong = item.First().TuVong,
                    //        ChuyenVien = item.First().ChuyenVien,
                    //        LaNguoiBenhDiaChiKhacHaNoi = item.First().LaNguoiBenhDiaChiKhacHaNoi,

                    //        TongNguoiBenh = 1
                    //    }).ToList();

                    //var thongTinSoLanKhamBenhs = groupTheoTiepNhan
                    //    .Where(x => x.LaKhamBenh)
                    //    .GroupBy(x => x.NoiThucHienId)
                    //    .Select(item => new ChiTietThucHienDichVuGridVo
                    //    {
                    //        LaKhamSucKhoe = item.First().LaKhamSucKhoe,
                    //        LaKhamBenh = item.First().LaKhamBenh,
                    //        LaKhamTiemChung = item.First().LaKhamTiemChung,

                    //        MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                    //        DichVuBenhVienId = item.First().DichVuBenhVienId,
                    //        TenDichVu = item.First().TenDichVu,
                    //        TrangThaiKhamBenh = item.First().TrangThaiKhamBenh,
                    //        NoiThucHienId = item.First().NoiThucHienId,
                    //        TenNoiThucHien = item.First().TenNoiThucHien,
                    //        LaNgoaiVien = item.First().LaNgoaiVien,
                    //        CoBHYT = item.First().CoBHYT,
                    //        ThoiDiemThucHien = item.First().ThoiDiemThucHien,

                    //        NgaySinh = item.First().NgaySinh,
                    //        ThangSinh = item.First().ThangSinh,
                    //        NamSinh = item.First().NamSinh,

                    //        LaKhamCapCuu = item.First().LaKhamCapCuu,

                    //        TuVong = item.First().TuVong,
                    //        ChuyenVien = item.First().ChuyenVien,
                    //        LaNguoiBenhDiaChiKhacHaNoi = item.First().LaNguoiBenhDiaChiKhacHaNoi,

                    //        TongNguoiBenh = item.Count()
                    //    })
                    //    .ToList();
                    #endregion

                    #region Logic mới
                    //Cập nhật 19/07/2022: sửa logic số người khám bệnh tại mục 1b thành đếm theo mã người bệnh
                    // logic cũ: đếm theo mã YCTN với dịch vụ khám đầu tiên
                    var lstThongTinNguoiBenhKhamBenh = lstThongTinKhamBenh
                        .GroupBy(x => x.MaYeuCauTiepNhan)
                        .Where(x => !x.Any(a => a.LaKhamSucKhoe)
                                    && x.All(a => a.LaKhamBenh))
                        .SelectMany(x => x).ToList();

                    var thongTinSoLanKhamBenhs = lstThongTinNguoiBenhKhamBenh
                        .Where(x => x.LaKhamBenh
                                    && !x.LaKhamSucKhoe
                                    && !string.IsNullOrEmpty(x.MaBN))
                        .GroupBy(x => x.NoiThucHienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            LaKhamSucKhoe = item.First().LaKhamSucKhoe,
                            LaKhamBenh = item.First().LaKhamBenh,
                            LaKhamTiemChung = item.First().LaKhamTiemChung,

                            MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                            DichVuBenhVienId = item.First().DichVuBenhVienId,
                            TenDichVu = item.First().TenDichVu,
                            TrangThaiKhamBenh = item.First().TrangThaiKhamBenh,
                            NoiThucHienId = item.Key,
                            TenNoiThucHien = item.First().TenNoiThucHien,
                            LaNgoaiVien = item.First().LaNgoaiVien,
                            CoBHYT = item.First().CoBHYT,
                            ThoiDiemThucHien = item.First().ThoiDiemThucHien,

                            NgaySinh = item.First().NgaySinh,
                            ThangSinh = item.First().ThangSinh,
                            NamSinh = item.First().NamSinh,

                            LaKhamCapCuu = item.First().LaKhamCapCuu,

                            TuVong = item.First().TuVong,
                            ChuyenVien = item.First().ChuyenVien,
                            LaNguoiBenhDiaChiKhacHaNoi = item.First().LaNguoiBenhDiaChiKhacHaNoi,

                            TongNguoiBenh = item.Select(a => a.MaBN).Distinct().Count(),
                        })
                        .ToList();
                    #endregion
                    #endregion

                    var soLanKhamBenhs = thongTinSoLanKhamBenhs.Where(x => !x.LaKhamSucKhoe).ToList();

                    if (soLanKhamNoiVien != null)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(soLanKhamNoiVien, tongSoNguoiKhamKSKNoiVienItem, tongSoNguoiKhamKSKItem);
                    }
                    if (soLanKhamNgoaiVien != null)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(soLanKhamNgoaiVien, tongSoNguoiKhamKSKNgoaiVienItem, tongSoNguoiKhamKSKItem);
                    }
                    if (soLanKhamBenhs.Any())
                    {
                        foreach (var soLanKhamBenh in soLanKhamBenhs)
                        {
                            GanThongTinKhamTheoKhoaPhongKham(soLanKhamBenh, tongSoNguoiKhamBenhItem);
                        }
                    }

                    tongSoNguoiKhamItem.TongSoTheoKhoaPhong = tongSoNguoiKhamKSKItem.TongSoTheoKhoaPhong + tongSoNguoiKhamBenhItem.TongSoTheoKhoaPhong;
                    tongSoNguoiKhamItem.ThongTinKhamTheoPhongs = tongSoNguoiKhamKSKItem.ThongTinKhamTheoPhongs.Union(tongSoNguoiKhamBenhItem.ThongTinKhamTheoPhongs).ToList();

                    lstChiTietBaoCao.Add(tongSoNguoiKhamItem);
                    lstChiTietBaoCao.Add(tongSoNguoiKhamKSKItem);
                    lstChiTietBaoCao.Add(tongSoNguoiKhamKSKNoiVienItem);
                    lstChiTietBaoCao.Add(tongSoNguoiKhamKSKNgoaiVienItem);
                    lstChiTietBaoCao.Add(tongSoNguoiKhamBenhItem);

                    #endregion

                    #region 2. Tổng số lần
                    var tongSoLanKhamItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKham
                    };
                    var tongSoLanKhamKSKItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamSucKhoe
                    };
                    var tongSoLanKhamKSKNoiVienItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamSucKhoeNoiVien
                    };
                    var tongSoLanKhamKSKNgoaiVienItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamSucKhoeNgoaiVien
                    };
                    var tongSoLanKhamBenhItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamBenh
                    };
                    var tongSoLanKhamBenhBHYTItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamBenhBHYT
                    };
                    var tongSoLanKhamBenhVienPhiItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamBenhVienPhi
                    };
                    var tongSoLanKhamBenhKhongThuDuocItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongSoLanKhamBenhKhongThuDuoc
                    };

                    foreach (var thongTinKhamBenh in lstThongTinKhamBenh)
                    {
                        //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                        if (thongTinKhamBenh.LaKhamSucKhoe || lstMaYCTNCoKSK.Contains(thongTinKhamBenh.MaYeuCauTiepNhan))
                        {
                            GanThongTinKhamTheoKhoaPhongKham(thongTinKhamBenh, thongTinKhamBenh.LaNgoaiVien ? tongSoLanKhamKSKNgoaiVienItem : tongSoLanKhamKSKNoiVienItem, tongSoLanKhamKSKItem);
                        }
                        else
                        {
                            GanThongTinKhamTheoKhoaPhongKham(thongTinKhamBenh, thongTinKhamBenh.CoBHYT ? tongSoLanKhamBenhBHYTItem : tongSoLanKhamBenhVienPhiItem, tongSoLanKhamBenhItem);
                        }
                    }

                    tongSoLanKhamItem.TongSoTheoKhoaPhong = tongSoLanKhamKSKItem.TongSoTheoKhoaPhong + tongSoLanKhamBenhItem.TongSoTheoKhoaPhong;
                    tongSoLanKhamItem.ThongTinKhamTheoPhongs = tongSoLanKhamKSKItem.ThongTinKhamTheoPhongs.Union(tongSoLanKhamBenhItem.ThongTinKhamTheoPhongs).ToList();

                    lstChiTietBaoCao.Add(tongSoLanKhamItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamKSKItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamKSKNoiVienItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamKSKNgoaiVienItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamBenhItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamBenhBHYTItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamBenhVienPhiItem);
                    lstChiTietBaoCao.Add(tongSoLanKhamBenhKhongThuDuocItem);
                    #endregion

                    #region 3. Trong TS lần khám bệnh
                    var trongTongSoLanKhamBenhItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenh,
                        LaItemTieuDe = true
                    };
                    var trongTongSoLanKhamBenhCapCuuItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenhCapCuu
                    };
                    var trongTongSoLanKhamBenhTreEmItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenhTreEm
                    };
                    var trongTongSoLanKhamBenTreEmDuoi6Tuoi = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenhTreEmDuoi6Tuoi
                    };
                    var trongTongSoLanKhamBenhNgoaiTinh = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TrongTongSoLanKhamBenhNgoaiTinh
                    };

                    var thongTinKhamLaTreEms = lstThongTinKhamBenh
                        .Where(x => (x.LaKhamBenh) // || x.LaKhamSucKhoe) 

                                    //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                                    && !lstMaYCTNCoKSK.Contains(x.MaYeuCauTiepNhan)

                                    && x.LaTreEm)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                            NoiThucHienId = item.NoiThucHienId,
                            TenNoiThucHien = item.TenNoiThucHien,

                            TongNguoiBenh = 1
                        })
                        .ToList();

                    var thongTinKhamLaTreEmDuoi6Tuois = lstThongTinKhamBenh
                        .Where(x => (x.LaKhamBenh) // || x.LaKhamSucKhoe) 

                                    //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                                    && !lstMaYCTNCoKSK.Contains(x.MaYeuCauTiepNhan)

                                    && x.LaTreEmDuoi6Tuoi)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                            NoiThucHienId = item.NoiThucHienId,
                            TenNoiThucHien = item.TenNoiThucHien,

                            TongNguoiBenh = 1
                        })
                        .ToList();

                    var thongTinKhamLaNgoaiTinhs = lstThongTinKhamBenh
                        .Where(x => (x.LaKhamBenh) // || x.LaKhamSucKhoe) 

                                    //Cập nhật ngày 02/03/2022: Đổi cơ chế số lượt KSK -> chỉ cần có 1 dv là KSK thì tất cả dv khác đều tính là KSK
                                    && !lstMaYCTNCoKSK.Contains(x.MaYeuCauTiepNhan)

                                    && x.LaNguoiBenhDiaChiKhacHaNoi)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                            NoiThucHienId = item.NoiThucHienId,
                            TenNoiThucHien = item.TenNoiThucHien,

                            TongNguoiBenh = 1
                        })
                        .ToList();

                    // thông tin cấp cứu, tạm thời chưa sử dụng
                    //var thongTinKhamCapCuus = lstThongTinKhamBenh
                    //    .Where(x => (x.LaKhamBenh) // || x.LaKhamSucKhoe) 
                    //                && x.LaKhamCapCuu)
                    //    .Select(item => new ChiTietThucHienDichVuGridVo
                    //    {
                    //        MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                    //        DichVuBenhVienId = item.DichVuBenhVienId,

                    //        TongNguoiBenh = 1
                    //    })
                    //    .ToList();

                    foreach (var thongTinKhamTreEm in thongTinKhamLaTreEms)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(thongTinKhamTreEm, trongTongSoLanKhamBenhTreEmItem);
                    }
                    foreach (var thongTinKhamTreEmDuoi6Tuoi in thongTinKhamLaTreEmDuoi6Tuois)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(thongTinKhamTreEmDuoi6Tuoi, trongTongSoLanKhamBenTreEmDuoi6Tuoi);
                    }
                    foreach (var thongTinKhamNgoiTinh in thongTinKhamLaNgoaiTinhs)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(thongTinKhamNgoiTinh, trongTongSoLanKhamBenhNgoaiTinh);
                    }


                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenhItem);
                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenhCapCuuItem);
                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenhTreEmItem);
                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenTreEmDuoi6Tuoi);
                    lstChiTietBaoCao.Add(trongTongSoLanKhamBenhNgoaiTinh);
                    #endregion

                    #region 4. TS người bệnh chuyển viện
                    var lstThongTinKhamChuyenVien = lstThongTinKhamBenh
                        .Where(x => x.ChuyenVien)
                        .OrderBy(x => x.ThoiDiemThucHien)
                        .GroupBy(x => x.MaYeuCauTiepNhan)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,
                            ChuyenVien = true,

                            TongNguoiBenh = 1
                        })
                        .GroupBy(x => x.NoiThucHienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = item.Count()
                        })
                        .ToList();
                    var tongSoNguoiChuyenVienItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhChuyenVien
                    };

                    foreach (var thongTinKhamChuyenVien in lstThongTinKhamChuyenVien)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(thongTinKhamChuyenVien, tongSoNguoiChuyenVienItem);
                    }

                    lstChiTietBaoCao.Add(tongSoNguoiChuyenVienItem);
                    #endregion

                    #region 5. Số người bệnh tử Vong
                    var lstThongTinKhamTuVong = lstThongTinKhamBenh
                        .Where(x => x.TuVong)
                        .OrderBy(x => x.ThoiDiemThucHien)
                        .GroupBy(x => x.MaYeuCauTiepNhan)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,
                            TuVong = true,

                            TongNguoiBenh = 1
                        })
                        .GroupBy(x => x.NoiThucHienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = item.Count()
                        })
                        .ToList();
                    var tongSoNguoiTuVongItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhTuVong
                    };
                    var tongSoNguoiTuVongTruoc24HItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhTuVongTruoc24H
                    };
                    var tongSoNguoiTuVongNgoaiVienItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhTuVongNgoaiVien
                    };

                    foreach (var thongTinKhamTuVong in lstThongTinKhamTuVong)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(thongTinKhamTuVong, tongSoNguoiTuVongItem);
                    }

                    lstChiTietBaoCao.Add(tongSoNguoiTuVongItem);
                    lstChiTietBaoCao.Add(tongSoNguoiTuVongTruoc24HItem);
                    lstChiTietBaoCao.Add(tongSoNguoiTuVongNgoaiVienItem);


                    #endregion

                    #region 6. TS người khám sàng lọc tiêm chủng
                    var tongSoNguoiBenhKhamSangLocTiemChungItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhKhamSangLocTiemChung
                    };
                    var tongNguoiBenhTiemChungItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongNguoiBenhTiemChung
                    };
                    var tongMuiTiemItem = new BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
                    {
                        LoaiNoiDung = Enums.NoiDungBaoCaoHoatDongKhamBenh.TongMuiTiem
                    };

                    
                    var lstThongTinKhamSangLoc = lstThongTinKhamTiemChung
                        .Where(x => x.LaKhamSangLoc)
                        .OrderBy(x => x.ThoiDiemThucHien)
                        .GroupBy(x => x.MaYeuCauTiepNhan)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = 1
                        })
                        .GroupBy(x => x.NoiThucHienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = item.Count()
                        })
                        .ToList();
                    var lstThongTinNguoiBenhTiemChung = lstThongTinKhamTiemChung
                        .Where(x => x.LaKhamSangLoc && x.DuDieuKienTiemChung)
                        .OrderBy(x => x.ThoiDiemThucHien)
                        .GroupBy(x => x.MaYeuCauTiepNhan)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = 1
                        })
                        .GroupBy(x => x.NoiThucHienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = item.Count()
                        })
                        .ToList();
                    var lstThongTinMuiTiem = lstThongTinKhamTiemChung
                        .Where(x => !x.LaKhamSangLoc)
                        .OrderBy(x => x.ThoiDiemThucHien)
                        //.GroupBy(x => x.MaYeuCauTiepNhan)
                        //.Select(item => new ChiTietThucHienDichVuGridVo
                        //{
                        //    MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                        //    NoiThucHienId = item.First().NoiThucHienId,
                        //    TenNoiThucHien = item.First().TenNoiThucHien,

                        //    TongNguoiBenh = 1
                        //})
                        .GroupBy(x => x.NoiThucHienId)
                        .Select(item => new ChiTietThucHienDichVuGridVo
                        {
                            NoiThucHienId = item.First().NoiThucHienId,
                            TenNoiThucHien = item.First().TenNoiThucHien,

                            TongNguoiBenh = item.Count()
                        })
                        .ToList();
                    foreach (var thongTinKhamSangLoc in lstThongTinKhamSangLoc)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(thongTinKhamSangLoc, tongSoNguoiBenhKhamSangLocTiemChungItem);
                    }
                    foreach (var thongTinNguoiBenhTiemChung in lstThongTinNguoiBenhTiemChung)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(thongTinNguoiBenhTiemChung, tongNguoiBenhTiemChungItem);
                    }
                    foreach (var thongTinMuiTiem in lstThongTinMuiTiem)
                    {
                        GanThongTinKhamTheoKhoaPhongKham(thongTinMuiTiem, tongMuiTiemItem);
                    }

                    lstChiTietBaoCao.Add(tongSoNguoiBenhKhamSangLocTiemChungItem);
                    lstChiTietBaoCao.Add(tongNguoiBenhTiemChungItem);
                    lstChiTietBaoCao.Add(tongMuiTiemItem);
                    #endregion
                }
            }

            return new GridDataSource
            {
                Data = lstChiTietBaoCao.ToArray(),
                TotalRowCount = lstChiTietBaoCao.Count()
            };
        }

        public async Task<GridDataSource> GetTotalPageBaoCaoHoatDongKhamBenhTheoKhoaPhongForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoHoatDongKhamBenhTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoHoatDongKhamBenhTimKiemVo>(queryInfo.AdditionalSearchString);
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

            if (tuNgay != null && denNgay != null)
            {
                timKiemNangCaoObj.TuNgayTimKiemData = tuNgay.Value;
                timKiemNangCaoObj.DenNgayTimKiemData = denNgay.Value;
                var result = await GetDataHoatDongKhamBenhAsync(timKiemNangCaoObj, true);
                return new GridDataSource { TotalRowCount = result.Item2 };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }

        public virtual byte[] ExportBaoCaoHoatDongKhamBenhTheoKhoaPhong(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoHoatDongKhamBenhTimKiemVo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoHoatDongKhamBenhTimKiemVo>(query.AdditionalSearchString);
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


            // var noiGioiThieu = _noiGioiThieuRepository.TableNoTracking.FirstOrDefault(x => x.Id == timKiemNangCaoObj.NoiGioiThieuId);

            var datas = (ICollection<BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO HOẠT ĐỘNG KHÁM BỆNH");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 12;
                    worksheet.Column(4).Width = 12;
                    //worksheet.Column(5).Width = 20;
                    //worksheet.Column(6).Width = 20;
                    //worksheet.Column(7).Width = 20;
                    //worksheet.Column(8).Width = 20;
                    //worksheet.Column(9).Width = 20;
                    //worksheet.Column(10).Width = 20;
                    //worksheet.Column(11).Width = 20;
                    //worksheet.Column(12).Width = 20;
                    //worksheet.Column(13).Width = 20;
                    //worksheet.Column(14).Width = 20;
                    worksheet.DefaultColWidth = 12;

                    #region khởi tạo list key column
                    var keyColTheoKhoaPhongs = new List<ColumnPhongKhamExcelInfoVo>();
                    string[] arrColumnDefault = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                    var colLuyKe = new ColumnPhongKhamExcelInfoVo()
                    {
                        ColumnName = "C"
                    };
                    keyColTheoKhoaPhongs.Add(colLuyKe); //col tổng cộng là mặc định
                    #endregion

                    //SET title BV
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A3:L3"])
                    {
                        range.Worksheet.Cells["A3:L3"].Merge = true;
                        range.Worksheet.Cells["A3:L3"].Value = "BÁO CÁO HOẠT ĐỘNG KHÁM BỆNH";
                        range.Worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A3:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:L4"])
                    {
                        range.Worksheet.Cells["A4:L4"].Merge = true;
                        range.Worksheet.Cells["A4:L4"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                      + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells["A4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:L4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:C7"])
                    {
                        range.Worksheet.Cells["A7:C7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:C7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:C7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:C7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:C7"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A7:C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A7"].Value = "STT";

                        range.Worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B7"].Value = "";

                        range.Worksheet.Cells["C7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C7"].Value = "Tổng Số";

                        // xử lý tạo column động theo khoa phòng khám
                        var lstKhoaPhong = datas.SelectMany(x => x.ThongTinKhamTheoPhongs)
                            .GroupBy(x => x.PhongBenhVienId)
                            .Select(item => new ThongTinKhamBenhTheoPhongVo()
                            {
                                PhongBenhVienId = item.Key,
                                TenPhongBenhVien = item.First().TenPhongBenhVien
                            }).Distinct()
                            .OrderBy(x => x.TenPhongBenhVien)
                            .ToList();
                        int lanLapTenCot = 0;
                        foreach (var khoaPhong in lstKhoaPhong)
                        {
                            string columnNameNew = "";

                            if (lanLapTenCot > 0)
                            {
                                //columnNameNew = "A";
                                for (int k = 0; k < lanLapTenCot; k++)
                                {
                                    columnNameNew += "A";
                                }
                            }

                            var lastColumnName = keyColTheoKhoaPhongs.Last().ColumnName;
                            if (lastColumnName.EndsWith('Z'))
                            {
                                columnNameNew += (lanLapTenCot == 0 ? "AA" : (columnNameNew + "A"));
                                lanLapTenCot++;
                            }
                            else
                            {
                                if (lanLapTenCot > 0)
                                {
                                    lastColumnName = lastColumnName.Substring(lastColumnName.Length - 1, 1);
                                }
                                var indexLastColumnName = arrColumnDefault.IndexOf(lastColumnName);
                                var columnNameNext = arrColumnDefault[indexLastColumnName + 1];
                                columnNameNew += columnNameNext;
                            }

                            var newColumn = new ColumnPhongKhamExcelInfoVo()
                            {
                                ColumnName = columnNameNew,
                                PhongBenhVienId = khoaPhong.PhongBenhVienId,
                                TenPhongBenhVien = khoaPhong.TenPhongBenhVien
                            };
                            keyColTheoKhoaPhongs.Add(newColumn);

                            // xử lý add cột mới vào file excel
                            range.Worksheet.Cells[columnNameNew + "7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[columnNameNew + "7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[columnNameNew + "7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[columnNameNew + "7"].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[columnNameNew + "7"].Style.Font.Bold = true;
                            range.Worksheet.Cells[columnNameNew + "7"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells[columnNameNew + "7"].Value = khoaPhong.TenPhongBenhVien;
                            range.Worksheet.Cells[columnNameNew + "7"].Style.WrapText = true;

                        }
                    }

                    //write data from line 8
                    int index = 8;
                    int stt = 1;
                    var formatCurrency = "#,##0.00";
                    if (datas.Any())
                    {
                        // bỏ qua giá trị mặc định ban đầu
                        keyColTheoKhoaPhongs = keyColTheoKhoaPhongs.Skip(1).ToList();
                        foreach (var item in datas)
                        {
                            // format border, font chữ,....
                            worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Value = item.STT;
                            worksheet.Cells["A" + index].Style.Font.Bold = item.InDamNoiDung;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = item.NoiDung;
                            worksheet.Cells["B" + index].Style.Font.Bold = item.InDamNoiDung;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Value = item.LaItemTieuDe ? (int?)null : item.TongSo;
                            worksheet.Cells["C" + index].Style.Font.Bold = item.InDamNoiDung;
                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            
                            foreach (var col in keyColTheoKhoaPhongs)
                            {
                                worksheet.Cells[col.ColumnName + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[col.ColumnName + index].Style.Font.Bold = item.InDamNoiDung;
                                worksheet.Cells[col.ColumnName + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                var soLuongTheoPhong = item.ThongTinKhamTheoPhongs.Where(x => x.PhongBenhVienId == col.PhongBenhVienId).Sum(x => x.SoLuong);
                                worksheet.Cells[col.ColumnName + index].Value = item.LaItemTieuDe ? (int?)null : soLuongTheoPhong;
                            }

                            stt++;
                            index++;
                        }
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }


        #endregion
    }
}
