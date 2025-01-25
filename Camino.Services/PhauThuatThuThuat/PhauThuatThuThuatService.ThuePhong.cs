using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        public async Task<bool> KiemTraThoiGianBatDauThuePhongAsync(long yeuCauDichVuKyThuatId, DateTime? batDau)
        {
            if (batDau == null)
            {
                return true;
            }

            //Cập nhật 06/06/2022: kiểm tra thuê phòng theo thời điểm tiếp nhận ngoại trú, nếu không có thì lấy thời điểm tạo bệnh án
            //var thoiDiemTiepNhan = await _yeuCauDichVuKyThuatRepository.TableNoTracking
            //    .Where(x => x.Id == yeuCauDichVuKyThuatId)
            //    .Select(x => x.YeuCauTiepNhan.ThoiDiemTiepNhan)
            //    .FirstAsync();

            DateTime? thoiDiemTiepNhan = null;
            var thongTinTiepNhan = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.Id == yeuCauDichVuKyThuatId)
                .Select(item => new LookupThoiDiemTiepNhanThuePhongVo()
                {
                    LaNoiTru = item.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru,
                    CoTiepNhanNgoaiTru = item.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null,
                    ThoiDiemTaoBenhAn = item.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemTaoBenhAn,
                    ThoiDiemTiepNhanCanQuyetToan = item.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToan.ThoiDiemTiepNhan,
                    ThoiDiemTiepNhan = item.YeuCauTiepNhan.ThoiDiemTiepNhan
                })
                .FirstAsync();

            if (thongTinTiepNhan.LaNoiTru)
            {
                if (thongTinTiepNhan.CoTiepNhanNgoaiTru)
                {
                    thoiDiemTiepNhan = thongTinTiepNhan.ThoiDiemTiepNhanCanQuyetToan;
                }
                else
                {
                    thoiDiemTiepNhan = thongTinTiepNhan.ThoiDiemTaoBenhAn;
                }
            }
            else
            {
                thoiDiemTiepNhan = thongTinTiepNhan.ThoiDiemTiepNhan;
            }
            
            return thoiDiemTiepNhan < batDau;
        }

        public bool KiemTraThoiGianThuePhongVoiNgayHienTai(DateTime? thoiGianThue)
        {
            if (thoiGianThue == null)
            {
                return true;
            }

            var ngayHomSau = DateTime.Now.Date.AddDays(1);
            var cuoiNgayHomSau = new DateTime(ngayHomSau.Year, ngayHomSau.Month, ngayHomSau.Day, 23, 59, 59);
            return thoiGianThue <= cuoiNgayHomSau;
        }

        public async Task<bool> KiemTraThoiGianBatDauThuePhongTheoTiepNhanAsync(long yeuCauTiepNhanId, DateTime? batDau)
        {
            if (batDau == null)
            {
                return true;
            }

            //Cập nhật 06/06/2022: kiểm tra thuê phòng theo thời điểm tiếp nhận ngoại trú, nếu không có thì lấy thời điểm tạo bệnh án
            //var thoiDiemTiepNhan = await _yeuCauTiepNhanRepository.TableNoTracking
            //    .Where(x => x.Id == yeuCauTiepNhanId)
            //    .Select(x => x.ThoiDiemTiepNhan)
            //    .FirstAsync();

            DateTime? thoiDiemTiepNhan = null;
            var thongTinTiepNhan = await _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.Id == yeuCauTiepNhanId)
                .Select(item => new LookupThoiDiemTiepNhanThuePhongVo()
                {
                    LaNoiTru = item.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru,
                    CoTiepNhanNgoaiTru = item.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null,
                    ThoiDiemTaoBenhAn = item.NoiTruBenhAn.ThoiDiemTaoBenhAn,
                    ThoiDiemTiepNhanCanQuyetToan = item.YeuCauTiepNhanNgoaiTruCanQuyetToan.ThoiDiemTiepNhan,
                    ThoiDiemTiepNhan = item.ThoiDiemTiepNhan
                })
                .FirstAsync();

            if (thongTinTiepNhan.LaNoiTru)
            {
                if (thongTinTiepNhan.CoTiepNhanNgoaiTru)
                {
                    thoiDiemTiepNhan = thongTinTiepNhan.ThoiDiemTiepNhanCanQuyetToan;
                }
                else
                {
                    thoiDiemTiepNhan = thongTinTiepNhan.ThoiDiemTaoBenhAn;
                }
            }
            else
            {
                thoiDiemTiepNhan = thongTinTiepNhan.ThoiDiemTiepNhan;
            }

            return thoiDiemTiepNhan < batDau;
        }

        public async Task XuLyLuuThongTinThuePhongAsync(ThongTinThuePhongVo thongTinThuePhong)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thoiDiemHienTai = DateTime.Now;

            var yeuCauDichVuKyThuat = await _yeuCauDichVuKyThuatRepository.Table
                // dùng để tính tỉ lệ ưu đãi theo dich vụ, tjam thời chưa cần
                //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.DoiTuongUuDai).ThenInclude(x => x.DoiTuongUuDaiDichVuKyThuatBenhViens)
                //.Include(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .Where(x => x.Id == thongTinThuePhong.YeuCauDichVuKyThuatId)
                .FirstOrDefaultAsync();
            if (yeuCauDichVuKyThuat == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            // tách câu query nhằm mục đích giảm thiểu include
            var thuePhong = await _thuePhongRepository.Table
                .Include(x => x.YeuCauDichVuKyThuatTinhChiPhi)
                .Where(x => x.YeuCauDichVuKyThuatThuePhongId == yeuCauDichVuKyThuat.Id)
                .FirstOrDefaultAsync();

            ThuePhong thuePhongEntity = null;
            if (thongTinThuePhong.CoThuePhong)
            {
                // kiểm tra tạo mới
                if (thuePhong == null || thongTinThuePhong.CauHinhThuePhongId == null || thuePhong.CauHinhThuePhongId != thongTinThuePhong.CauHinhThuePhongId)
                {
                    if (thuePhong != null)
                    {
                        if (thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                        {
                            throw new Exception(
                                _localizationService.GetResource("PhauThuatThuThuatThuePhong.DichVu.DaHuy"));
                        }

                        if (thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new Exception(_localizationService.GetResource("PhauThuatThuThuatThuePhong.ThuePhong.DaThanhToan"));
                        }
                        thuePhong.WillDelete = true;
                        thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;
                    }

                    //Xử lý tạo mới thông tin Thuê phòng
                    var cauHinhThuePhong = await _cauHinhThuePhongRepository.TableNoTracking
                        .Where(x => x.Id == thongTinThuePhong.CauHinhThuePhongId.Value)
                        .FirstOrDefaultAsync();

                    if (cauHinhThuePhong == null)
                    {
                        throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                    }

                    thuePhongEntity = new ThuePhong()
                    {
                        YeuCauTiepNhanId = thongTinThuePhong.YeuCauTiepNhanId,
                        YeuCauDichVuKyThuatThuePhongId = thongTinThuePhong.YeuCauDichVuKyThuatId,
                        CauHinhThuePhongId = cauHinhThuePhong.Id,
                        TenCauHinhThuePhong = cauHinhThuePhong.Ten,
                        LoaiThuePhongPhauThuatId = cauHinhThuePhong.LoaiThuePhongPhauThuatId,
                        LoaiThuePhongNoiThucHienId = cauHinhThuePhong.LoaiThuePhongNoiThucHienId,
                        BlockThoiGianTheoPhut = cauHinhThuePhong.BlockThoiGianTheoPhut,
                        GiaThue = cauHinhThuePhong.GiaThue,
                        PhanTramNgoaiGio = cauHinhThuePhong.PhanTramNgoaiGio,
                        PhanTramLeTet = cauHinhThuePhong.PhanTramLeTet,
                        GiaThuePhatSinh = cauHinhThuePhong.GiaThuePhatSinh,
                        PhanTramPhatSinhNgoaiGio = cauHinhThuePhong.PhanTramPhatSinhNgoaiGio,
                        PhanTramPhatSinhLeTet = cauHinhThuePhong.PhanTramPhatSinhLeTet,
                        ThoiDiemBatDau = thongTinThuePhong.ThoiDiemBatDau,
                        ThoiDiemKetThuc = thongTinThuePhong.ThoiDiemKetThuc,
                        NhanVienChiDinhId = currentUserId,
                        NoiChiDinhId = phongHienTaiId
                    };

                    var dichVuThuePhong = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                        .Include(o => o.DichVuKyThuatVuBenhVienGiaBenhViens)
                        .Include(o => o.DichVuKyThuat)
                        .Where(x => x.Id == (long)Enums.EnumDichVuThuePhong.Id)
                        .FirstOrDefaultAsync();

                    if (dichVuThuePhong == null)
                    {
                        throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                    }

                    //tính tỉ lệ ưu đãi theo dịch vụ
                    //tạm thời chưa cần
                    //var dtudDVKTBV = yeuCauDichVuKyThuat.YeuCauTiepNhan.DoiTuongUuDai?.DoiTuongUuDaiDichVuKyThuatBenhViens?.FirstOrDefault(o =>
                    //    o.DichVuKyThuatBenhVienId == dichVuThuePhong.Id && o.DichVuKyThuatBenhVien.CoUuDai == true);

                    var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
                    long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);
                    var dvktGiaBV = dichVuThuePhong.DichVuKyThuatVuBenhVienGiaBenhViens
                        .Where(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                        .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                        .ThenBy(x => x.CreatedOn)
                        .First();
                    var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
                    //var noiThucHienId = await GetNoiThucHienDichVuKyThuatAsync(dichVuThuePhong.Id);
                    long? noiThucHienId = null;
                    if (noiThucHienId == null)
                    {
                        noiThucHienId = phongHienTaiId;
                    }

                    var donGiaThue = await _cauHinhService.GetDonGiaThuePhongAsync(thuePhongEntity);
                    var newYeuCauTinhChiPhi = new YeuCauDichVuKyThuat()
                    {
                        YeuCauTiepNhanId = thongTinThuePhong.YeuCauTiepNhanId,
                        DichVuKyThuatBenhVienId = dichVuThuePhong.Id,
                        MaDichVu = dichVuThuePhong.Ma,
                        TenDichVu = dichVuThuePhong.Ten,
                        Gia = donGiaThue,
                        NhomGiaDichVuKyThuatBenhVienId = dvktGiaBV.NhomGiaDichVuKyThuatBenhVienId,
                        NhomChiPhi = dichVuThuePhong.DichVuKyThuat != null ? dichVuThuePhong.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                        SoLan = 1,
                        //TiLeUuDai = dtudDVKTBV?.TiLeUuDai,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                        TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,

                        NhanVienChiDinhId = currentUserId,
                        ThoiDiemChiDinh = thongTinThuePhong.ThoiDiemBatDau,
                        NoiChiDinhId = phongHienTaiId,

                        ThoiDiemDangKy = thongTinThuePhong.ThoiDiemBatDau,

                        NoiThucHienId = noiThucHienId,
                        NhanVienThucHienId = currentUserId,
                        ThoiDiemThucHien = thongTinThuePhong.ThoiDiemBatDau,

                        NhanVienKetLuanId = currentUserId,
                        ThoiDiemKetLuan = thongTinThuePhong.ThoiDiemKetThuc,
                        ThoiDiemHoanThanh = thongTinThuePhong.ThoiDiemKetThuc,

                        NhomDichVuBenhVienId = dichVuThuePhong.NhomDichVuBenhVienId,
                        LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dichVuThuePhong.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                        MaGiaDichVu = dichVuThuePhong.DichVuKyThuat?.MaGia,
                        TenGiaDichVu = dichVuThuePhong.DichVuKyThuat?.TenGia
                    };

                    var nguoiThucHien = await GetBacSiThucHienMacDinh(newYeuCauTinhChiPhi.NoiThucHienId ?? 0);
                    if (nguoiThucHien != null)
                    {
                        newYeuCauTinhChiPhi.NhanVienThucHienId = nguoiThucHien.KeyId;
                    }

                    thuePhongEntity.YeuCauDichVuKyThuatTinhChiPhi = newYeuCauTinhChiPhi;
                    yeuCauDichVuKyThuat.ThuePhongs.Add(thuePhongEntity);
                }
                // cập nhật lại thời gian
                else
                {
                    thuePhongEntity = thuePhong;
                    if (thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    {
                        throw new Exception(
                            _localizationService.GetResource("PhauThuatThuThuatThuePhong.DichVu.DaHuy"));
                    }

                    if (thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                    {
                        throw new Exception(_localizationService.GetResource("PhauThuatThuThuatThuePhong.ThuePhong.DaThanhToan"));
                    }

                    thuePhongEntity.ThoiDiemBatDau = thongTinThuePhong.ThoiDiemBatDau;
                    thuePhongEntity.ThoiDiemKetThuc = thongTinThuePhong.ThoiDiemKetThuc;
                    thuePhongEntity.YeuCauDichVuKyThuatTinhChiPhi.Gia = await _cauHinhService.GetDonGiaThuePhongAsync(thuePhongEntity);

                    thuePhongEntity.YeuCauDichVuKyThuatTinhChiPhi.ThoiDiemChiDinh = thongTinThuePhong.ThoiDiemBatDau;
                    thuePhongEntity.YeuCauDichVuKyThuatTinhChiPhi.ThoiDiemDangKy = thongTinThuePhong.ThoiDiemBatDau;
                    thuePhongEntity.YeuCauDichVuKyThuatTinhChiPhi.ThoiDiemThucHien = thongTinThuePhong.ThoiDiemBatDau;
                    thuePhongEntity.YeuCauDichVuKyThuatTinhChiPhi.ThoiDiemKetLuan = thongTinThuePhong.ThoiDiemKetThuc;
                    thuePhongEntity.YeuCauDichVuKyThuatTinhChiPhi.ThoiDiemHoanThanh = thongTinThuePhong.ThoiDiemKetThuc;
                }

                //Cập nhật bỏ đồng bộ thời gian thuê phòng và thời gian PTTT
                //if (yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null)
                //{
                //    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat = thongTinThuePhong.ThoiDiemBatDau;
                //    yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat = thongTinThuePhong.ThoiDiemKetThuc;
                //}
            }
            else
            {
                if (thuePhong != null)
                {
                    if (thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                    {
                        throw new Exception(_localizationService.GetResource("PhauThuatThuThuatThuePhong.ThuePhong.DaThanhToan"));
                    }
                    thuePhong.WillDelete = true;
                    thuePhong.YeuCauDichVuKyThuatTinhChiPhi.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;
                }
            }

            await _thuePhongRepository.Context.SaveChangesAsync();

            if (thuePhongEntity != null)
            {
                thongTinThuePhong.ThuePhongId = thuePhongEntity.Id;
            }
        }

        private async Task<long?> GetNoiThucHienDichVuKyThuatAsync(long dichVuKyThuatBenhVienId)
        {
            long? phongBenhVienId = null;
            var noiThucHienUuTiens = await _dichVuKyThuatBenhVienNoiThucHienUuTienRepository.TableNoTracking
                .Where(x => x.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId)
                .ToListAsync();
            if (noiThucHienUuTiens.Any())
            {
                phongBenhVienId = noiThucHienUuTiens
                    .OrderByDescending(x => x.LoaiNoiThucHienUuTien == Enums.LoaiNoiThucHienUuTien.NguoiDung)
                    .Select(x => x.PhongBenhVienId).First();
            }
            else
            {
                var noiThucHiens = await _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking
                    .Where(x => x.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId)
                    .ToListAsync();

                var lstPhongThucHien = noiThucHiens
                    .Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList();
                if (lstPhongThucHien.Any())
                {
                    phongBenhVienId = lstPhongThucHien.First();
                }
                else
                {
                    var lstKhoaId = noiThucHiens
                        .Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhongId.Value).ToList();
                    if (lstKhoaId.Any())
                    {
                        var phongThucHienIdTheoKhoa = await _phongBenhVienRepository.TableNoTracking
                            .Where(x => lstKhoaId.Contains(x.KhoaPhongId))
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();
                        phongBenhVienId = phongThucHienIdTheoKhoa;
                    }
                }
            }

            return phongBenhVienId;
        }

        public async Task<ThuePhong> GetThongTinThuePhongTheoDichVuKyThuatAsync(long yeuCauDichVuKyThuatId)
        {
            var thuePhong = await _thuePhongRepository.TableNoTracking
                .Where(x => x.YeuCauDichVuKyThuatThuePhongId == yeuCauDichVuKyThuatId)
                .FirstOrDefaultAsync();
            return thuePhong;
        }

        public async Task<ThuePhong> GetThongTinThuePhongTheoDichVuKyThuatFoUpdateAsync(long yeuCauDichVuKyThuatId)
        {
            var thuePhong = await _thuePhongRepository.Table
                .Include(x => x.YeuCauDichVuKyThuatTinhChiPhi)
                .Where(x => x.YeuCauDichVuKyThuatThuePhongId == yeuCauDichVuKyThuatId)
                .FirstOrDefaultAsync();
            return thuePhong;
        }

        #region Grid lịch sử thuê phòng
        public async Task<GridDataSource> GetDataForGridLichSuThuePhongAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var timKiemNangCaoObj = new LichSuThuePhongTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LichSuThuePhongTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (!string.IsNullOrEmpty(timKiemNangCaoObj.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var results = _thuePhongRepository.TableNoTracking
                .Where(x => (timKiemNangCaoObj.CauHinhThuePhongId == null || x.CauHinhThuePhongId == timKiemNangCaoObj.CauHinhThuePhongId)
                            && (timKiemNangCaoObj.NoiThucHienId == null || x.YeuCauDichVuKyThuatTinhChiPhi.NoiThucHienId == timKiemNangCaoObj.NoiThucHienId)
                            && (tuNgay == null || x.ThoiDiemBatDau >= tuNgay)
                            && (denNgay == null || x.ThoiDiemKetThuc <= denNgay))
                //.Skip(queryInfo.Skip).Take(queryInfo.Take)
                .Select(s => new LichSuThuePhongGridVo()
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauDichVuKyThuatThuePhongId = s.YeuCauDichVuKyThuatThuePhongId,
                    YeuCauDichVuKyThuatTuongTrinhPTTTId = s.YeuCauDichVuKyThuatThuePhong.YeuCauDichVuKyThuatTuongTrinhPTTT.Id,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaNB = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    TenNB = s.YeuCauTiepNhan.HoTen,
                    NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = s.YeuCauTiepNhan.ThangSinh,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                    CoBHYT = s.YeuCauTiepNhan.CoBHYT,
                    DichVuThue = s.YeuCauDichVuKyThuatThuePhong.TenDichVu,
                    LoaiPhongThue = s.TenCauHinhThuePhong,
                    BatDauThue = s.ThoiDiemBatDau,
                    KetThucThue = s.ThoiDiemKetThuc,
                    PhongThucHien = s.YeuCauDichVuKyThuatTinhChiPhi.NoiThucHien.Ten,
                    BacSiGayMe = string.Empty,
                    PhauThuatVien = string.Empty,
                    CauHinhThuePhongId = s.CauHinhThuePhongId
                })
                .OrderBy(queryInfo.SortString)
                .ToList();

            if (results.Any())
            {
                var lstYeuCauDichVuPTTTId = results
                    .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId != null)
                    .Select(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId).Distinct().ToList();
                if (lstYeuCauDichVuPTTTId.Any())
                {
                    var lstBacSi = _phauThuatThuThuatEkipBacSiRepository.TableNoTracking
                        .Where(x => lstYeuCauDichVuPTTTId.Contains(x.YeuCauDichVuKyThuatTuongTrinhPTTTId)
                                    && (x.VaiTroBacSi == Enums.EnumVaiTroBacSi.GayMeTeChinh || x.VaiTroBacSi == Enums.EnumVaiTroBacSi.GayMeTePhu || x.VaiTroBacSi == Enums.EnumVaiTroBacSi.PhauThuatVienChinh))
                        .Select(x => new ThongTinEkipVo()
                        {
                            YeuCauDichVuKyThuatTuongTrinhPTTTId = x.YeuCauDichVuKyThuatTuongTrinhPTTTId,
                            HoTen = x.NhanVien.User.HoTen,
                            LaPhauThuatVien = x.VaiTroBacSi == Enums.EnumVaiTroBacSi.PhauThuatVienChinh
                        }).Distinct().ToList();

                    var lstDieuDuong = _phauThuatThuThuatEkipDieuDuongRepository.TableNoTracking
                        .Where(x => lstYeuCauDichVuPTTTId.Contains(x.YeuCauDichVuKyThuatTuongTrinhPTTTId)
                                    && x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.PhauThuatVienChinh)
                        .Select(x => new ThongTinEkipVo()
                        {
                            YeuCauDichVuKyThuatTuongTrinhPTTTId = x.YeuCauDichVuKyThuatTuongTrinhPTTTId,
                            HoTen = x.NhanVien.User.HoTen,
                            LaPhauThuatVien = true
                        }).Distinct().ToList();

                    if (lstBacSi.Any() || lstDieuDuong.Any())
                    {
                        foreach (var thuePhong in results)
                        {
                            if (thuePhong.YeuCauDichVuKyThuatTuongTrinhPTTTId != null)
                            {
                                var lstBacSiGayMeTheoLanPT = lstBacSi
                                    .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == thuePhong.YeuCauDichVuKyThuatTuongTrinhPTTTId.Value
                                                && !x.LaPhauThuatVien)
                                    .Select(x => x.HoTen)
                                    .Distinct().ToList();
                                var lstPhauThuatVienTheoLanPT = lstBacSi
                                    .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == thuePhong.YeuCauDichVuKyThuatTuongTrinhPTTTId.Value
                                                && x.LaPhauThuatVien)
                                    .Select(x => x.HoTen)
                                    .Union(
                                        lstDieuDuong
                                            .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == thuePhong.YeuCauDichVuKyThuatTuongTrinhPTTTId.Value
                                                        && x.LaPhauThuatVien)
                                            .Select(x => x.HoTen)
                                            .ToList()
                                        )
                                    .Distinct().ToList();
                                if (lstBacSiGayMeTheoLanPT.Any())
                                {
                                    thuePhong.BacSiGayMe = string.Join(", ", lstBacSiGayMeTheoLanPT);
                                }

                                if (lstPhauThuatVienTheoLanPT.Any())
                                {
                                    thuePhong.PhauThuatVien = string.Join(", ", lstPhauThuatVienTheoLanPT);
                                }
                            }
                        }

                        //xử lý tìm kiếm theo thông tin searchstring
                        if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                        {
                            var searchString = timKiemNangCaoObj.SearchString.Trim().ToLower().RemoveVietnameseDiacritics();
                            results = results
                                .Where(x => x.MaNB.ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                            || x.TenNB.ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                            || x.MaYeuCauTiepNhan.ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                            || x.BacSiGayMe.ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                            || x.PhauThuatVien.ToLower().RemoveVietnameseDiacritics().Contains(searchString))
                                .ToList();
                        }
                    }
                }

                results = results.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
            }
            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();
            //await Task.WhenAll(countTask, queryTask);

            var queryTask = results.ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = queryTask.Length };

        }
        public async Task<GridDataSource> GetTotalPageForGridLichSuThuePhongAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new LichSuThuePhongTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LichSuThuePhongTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (!string.IsNullOrEmpty(timKiemNangCaoObj.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var results = _thuePhongRepository.TableNoTracking
                .Where(x => (timKiemNangCaoObj.CauHinhThuePhongId == null || x.CauHinhThuePhongId == timKiemNangCaoObj.CauHinhThuePhongId)
                            && (timKiemNangCaoObj.NoiThucHienId == null || x.YeuCauDichVuKyThuatTinhChiPhi.NoiThucHienId == timKiemNangCaoObj.NoiThucHienId)
                            && (tuNgay == null || x.ThoiDiemBatDau >= tuNgay)
                            && (denNgay == null || x.ThoiDiemKetThuc <= denNgay))
                .Select(s => new LichSuThuePhongGridVo()
                {
                    Id = s.Id,
                    YeuCauDichVuKyThuatTuongTrinhPTTTId = s.YeuCauDichVuKyThuatThuePhong.YeuCauDichVuKyThuatTuongTrinhPTTT.Id,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaNB = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    TenNB = s.YeuCauTiepNhan.HoTen,
                    BacSiGayMe = string.Empty,
                    PhauThuatVien = string.Empty
                })
                .ToList();

            if (results.Any() && !string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
            {
                var lstYeuCauDichVuPTTTId = results
                    .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId != null)
                    .Select(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId).Distinct().ToList();
                if (lstYeuCauDichVuPTTTId.Any())
                {
                    var lstBacSi = _phauThuatThuThuatEkipBacSiRepository.TableNoTracking
                        .Where(x => lstYeuCauDichVuPTTTId.Contains(x.YeuCauDichVuKyThuatTuongTrinhPTTTId)
                                    && (x.VaiTroBacSi == Enums.EnumVaiTroBacSi.GayMeTeChinh || x.VaiTroBacSi == Enums.EnumVaiTroBacSi.GayMeTePhu || x.VaiTroBacSi == Enums.EnumVaiTroBacSi.PhauThuatVienChinh))
                        .Select(x => new ThongTinEkipVo()
                        {
                            YeuCauDichVuKyThuatTuongTrinhPTTTId = x.YeuCauDichVuKyThuatTuongTrinhPTTTId,
                            HoTen = x.NhanVien.User.HoTen,
                            LaPhauThuatVien = x.VaiTroBacSi == Enums.EnumVaiTroBacSi.PhauThuatVienChinh
                        }).Distinct().ToList();

                    var lstDieuDuong = _phauThuatThuThuatEkipDieuDuongRepository.TableNoTracking
                        .Where(x => lstYeuCauDichVuPTTTId.Contains(x.YeuCauDichVuKyThuatTuongTrinhPTTTId)
                                    && x.VaiTroDieuDuong == Enums.EnumVaiTroDieuDuong.PhauThuatVienChinh)
                        .Select(x => new ThongTinEkipVo()
                        {
                            YeuCauDichVuKyThuatTuongTrinhPTTTId = x.YeuCauDichVuKyThuatTuongTrinhPTTTId,
                            HoTen = x.NhanVien.User.HoTen,
                            LaPhauThuatVien = true
                        }).Distinct().ToList();

                    if (lstBacSi.Any() || lstDieuDuong.Any())
                    {
                        foreach (var thuePhong in results)
                        {
                            if (thuePhong.YeuCauDichVuKyThuatTuongTrinhPTTTId != null)
                            {
                                var lstBacSiGayMeTheoLanPT = lstBacSi
                                    .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == thuePhong.YeuCauDichVuKyThuatTuongTrinhPTTTId.Value
                                                && !x.LaPhauThuatVien)
                                    .Select(x => x.HoTen)
                                    .Distinct().ToList();
                                var lstPhauThuatVienTheoLanPT = lstBacSi
                                    .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == thuePhong.YeuCauDichVuKyThuatTuongTrinhPTTTId.Value
                                                && x.LaPhauThuatVien)
                                    .Select(x => x.HoTen)
                                    .Union(
                                        lstDieuDuong
                                            .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == thuePhong.YeuCauDichVuKyThuatTuongTrinhPTTTId.Value
                                                        && x.LaPhauThuatVien)
                                            .Select(x => x.HoTen)
                                            .ToList()
                                        )
                                    .Distinct().ToList();
                                if (lstBacSiGayMeTheoLanPT.Any())
                                {
                                    thuePhong.BacSiGayMe = string.Join(", ", lstBacSiGayMeTheoLanPT);
                                }

                                if (lstPhauThuatVienTheoLanPT.Any())
                                {
                                    thuePhong.PhauThuatVien = string.Join(", ", lstPhauThuatVienTheoLanPT);
                                }
                            }
                        }

                        //xử lý tìm kiếm theo thông tin searchstring
                        if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                        {
                            var searchString = timKiemNangCaoObj.SearchString.Trim().ToLower().RemoveVietnameseDiacritics();
                            results = results
                                .Where(x => x.MaNB.ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                            || x.TenNB.ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                            || x.MaYeuCauTiepNhan.ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                            || x.BacSiGayMe.ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                            || x.PhauThuatVien.ToLower().RemoveVietnameseDiacritics().Contains(searchString))
                                .ToList();
                        }
                    }
                }
            }

            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);
            var queryTask = results.ToArray();
            return new GridDataSource { TotalRowCount = queryTask.Length };
        }

        #endregion

        #region Chi tiết lịch sử thuê phòng
        public async Task<LichSuThuePhongThongTinHanhChinhVo> GetThongTinHanhChinh(long yeuCauTiepNhanId)
        {
            var result = await _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.Id == yeuCauTiepNhanId)
                .Select(x => new LichSuThuePhongThongTinHanhChinhVo()
                {
                    MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                    MaNB = x.BenhNhan.MaBN,
                    HoTen = x.HoTen,
                    GioiTinh = x.GioiTinh != null ? x.GioiTinh.GetDescription() : "",
                    Tuoi = x.NamSinh != null ? (DateTime.Now.Year - x.NamSinh.GetValueOrDefault()) : (int?)null,
                    SoDienThoai = x.SoDienThoaiDisplay,
                    DanToc = x.DanToc.Ten,
                    DiaChi = x.DiaChiDayDu,
                    NgheNghiep = x.NgheNghiep.Ten,
                    TuyenKham = x.LyDoVaoVien != null ? x.LyDoVaoVien.GetDescription() : "",
                    SoBHYT = x.BHYTMaSoThe,
                    MucHuong = x.BHYTMucHuong,
                    CoBHYT = x.CoBHYT != null && x.CoBHYT == true,
                    BHYTNgayHieuLuc = x.BHYTNgayHieuLuc,
                    BHYTNgayHetHan = x.BHYTNgayHetHan,
                    LyDoTiepNhan = x.LyDoTiepNhan.Ten,
                    NgayTiepNhan = x.ThoiDiemTiepNhan.ApplyFormatDateTime(),
                    LaCapCuu = x.LaCapCuu != null && x.LaCapCuu == true,
                    QuyetToanTheoNoiTru = x.QuyetToanTheoNoiTru != null && x.QuyetToanTheoNoiTru == true,

                    //BVHD-3941
                    YeuCauTiepNhanId = x.Id,
                    CoBaoHiemTuNhan = x.CoBHTN
                }).FirstOrDefaultAsync();

            if (result != null && !result.LaCapCuu && result.QuyetToanTheoNoiTru)
            {
                var yeuCauTiepNhanNoiTru = await _yeuCauTiepNhanRepository.TableNoTracking
                    .Where(x => x.MaYeuCauTiepNhan.Contains(result.MaYeuCauTiepNhan)
                                && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    .FirstOrDefaultAsync();
                result.LaCapCuu = yeuCauTiepNhanNoiTru?.LaCapCuu == true;
            }
            return result;
        }

        public async Task<List<LookupYeuCauCoThuePhongVo>> GetListDichVuCoThuePhongTheoTiepNhan(DropDownListRequestModel queryInfo)
        {
            var yeuCauTiepNhanId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            var lookups = await _thuePhongRepository.TableNoTracking
                .Where(x => x.YeuCauDichVuKyThuatThuePhong.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            //&& x.YeuCauDichVuKyThuatThuePhong.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                            && x.YeuCauDichVuKyThuatThuePhong.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true
                            && x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                .OrderByDescending(x => x.YeuCauDichVuKyThuatThuePhongId == queryInfo.Id).ThenBy(x => x.YeuCauDichVuKyThuatThuePhong.TenDichVu)
                .ApplyLike(queryInfo.Query?.Trim(), e => e.YeuCauDichVuKyThuatThuePhong.TenDichVu)
                .Select(item => new LookupYeuCauCoThuePhongVo
                {
                    KeyId = item.YeuCauDichVuKyThuatThuePhongId,
                    DisplayName = item.YeuCauDichVuKyThuatThuePhong.TenDichVu,
                    ThuePhongId = item.Id,
                    YeuCauDichVuKyThuatThuePhongId = item.YeuCauDichVuKyThuatThuePhongId,
                    CauHinhThuePhongId = item.CauHinhThuePhongId,
                    TenCauHinhThuePhong = item.TenCauHinhThuePhong,
                    ThoiDiemBatDau = item.ThoiDiemBatDau,
                    ThoiDiemKetThuc = item.ThoiDiemKetThuc,
                    NoiThucHienId = item.YeuCauDichVuKyThuatTinhChiPhi.NoiThucHienId,
                    TenNoiThucHien = item.YeuCauDichVuKyThuatTinhChiPhi.NoiThucHien.Ten
                })
                .Take(queryInfo.Take)
                .ToListAsync();

            return lookups;
        }
        #endregion
    }
}
