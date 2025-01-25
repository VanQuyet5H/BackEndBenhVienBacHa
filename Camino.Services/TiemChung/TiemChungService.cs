using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.Templates;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TiemChungs;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.TiemChung
{
    [ScopedDependency(ServiceType = typeof(ITiemChungService))]
    public partial class TiemChungService : YeuCauTiepNhanBaseService, ITiemChungService
    {
        private readonly IRepository<YeuCauDichVuKyThuatKhamSangLocTiemChung> _yeuCauDichVuKyThuatKhamSangLocTiemChungRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<TemplateKhamSangLocTiemChung> _templateKhamSangLocTiemChungRepository;
        private readonly IRepository<Kho> _khoRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IDuocPhamVaVatTuBenhVienService _duocPhamVaVatTuBenhVienService;
        private readonly IRepository<PhongBenhVienHangDoi> _phongBenhVienHangDoiRepository;
        private readonly IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> _hoatDongNhanVienRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<DichVuKyThuatBenhVienTiemChung> _dichVuKyThuatBenhVienTiemChungRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> _quanHeThanNhanRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuKyThuatRepository;
        private readonly IRepository<XuatKhoDuocPhamChiTiet> _xuatKhoDuocPhamChiTietRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;

        public TiemChungService (
            IRepository<YeuCauTiepNhan> repository
            , IUserAgentHelper userAgentHelper
            , ICauHinhService cauHinhService
            , ILocalizationService localizationService
            , ITaiKhoanBenhNhanService taiKhoanBenhNhanService
            , IRepository<YeuCauDichVuKyThuatKhamSangLocTiemChung> yeuCauDichVuKyThuatKhamSangLocTiemChungRepository
            , IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository
            , IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository
            , IRepository<TemplateKhamSangLocTiemChung> templateKhamSangLocTiemChungRepository
            , IRepository<Kho> khoRepository
            , IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository
            , IDuocPhamVaVatTuBenhVienService duocPhamVaVatTuBenhVienService
            , IRepository<PhongBenhVienHangDoi> phongBenhVienHangDoiRepository
            , IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository
            , IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository
            , IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> hoatDongNhanVienRepository
            , IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository
            , IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            , IRepository<DichVuKyThuatBenhVienTiemChung> dichVuKyThuatBenhVienTiemChungRepository
            , IRepository<Template> templateRepository
            , IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> quanHeThanNhanRepository
            , IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> chuongTrinhGoiDichVuKyThuatRepository
            , IRepository<XuatKhoDuocPhamChiTiet> xuatKhoDuocPhamChiTietRepository
            , IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> chuongTrinhGoiDichVuKhamBenhRepository
            , IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository
            , IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository
            ) : base ( 
                repository
                , userAgentHelper
                , cauHinhService
                , localizationService
                , taiKhoanBenhNhanService
            )
        {
            _yeuCauDichVuKyThuatKhamSangLocTiemChungRepository = yeuCauDichVuKyThuatKhamSangLocTiemChungRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _nhanVienRepository = nhanVienRepository;
            _templateKhamSangLocTiemChungRepository = templateKhamSangLocTiemChungRepository;
            _khoRepository = khoRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _duocPhamVaVatTuBenhVienService = duocPhamVaVatTuBenhVienService;
            _phongBenhVienHangDoiRepository = phongBenhVienHangDoiRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _hoatDongNhanVienRepository = hoatDongNhanVienRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _dichVuKyThuatBenhVienTiemChungRepository = dichVuKyThuatBenhVienTiemChungRepository;
            _templateRepository = templateRepository;
            _quanHeThanNhanRepository = quanHeThanNhanRepository;
            _chuongTrinhGoiDichVuKyThuatRepository = chuongTrinhGoiDichVuKyThuatRepository;
            _xuatKhoDuocPhamChiTietRepository = xuatKhoDuocPhamChiTietRepository;
            _chuongTrinhGoiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
        }

        #region Get data

        public async Task<ICollection<HangDoiTiemChungGridVo>> GetDanhSachChoKhamHienTaiAsync(HangDoiTiemChungQuyeryInfo quyeryInfo)
        {
            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Include(x => x.YeuCauTiepNhan)
                    .Include(x => x.PhongBenhVienHangDois)
                    .Include(x => x.YeuCauDichVuKyThuatKhamSangLocTiemChung)
                 .Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId

                             // phòng bệnh viện hàng đợi // check loại hàng đợi
                             && (
                                //x.PhongBenhVienHangDois.OrderByDescending(y => y.Id).First().TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                                (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                                    && x.TiemChung == null
                                    && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && (!x.PhongBenhVienHangDois.Any() || (x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId) 
                                                                            && x.PhongBenhVienHangDois
                                                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)
                                                                            ))
                                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung)
                                    // hàng chờ khám sàng lọc chỉ lấy những dv đang thực hiện
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                    || (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                                        && x.TiemChung != null
                                        && (!x.PhongBenhVienHangDois.Any() 
                                            || x.PhongBenhVienHangDois
                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                                        && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung != null
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                                        && (
                                            // phòng hiện tại có vacxin chưa tiêm
                                            x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                            .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                        && y.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId
                                                        && (
                                                            y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                        ))
                                            // yêu cầu khám sàng lọc còn vacxin chưa tiêm
                                            || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                          && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                          && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien
                                                          && y.NoiThucHienId != quyeryInfo.PhongKhamHienTaiId
                                                          && (
                                                              y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                              || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                              || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                          )))

                                        // kierm tra hàng đợi: nếu có thì tất cả hàng đợi vacxin đều phải đang chờ khám
                                        && (!x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && (
                                                                y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            ))
                                                .SelectMany(y => y.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)).Any()
                                            || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && (
                                                                y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            ))
                                                .SelectMany(y => y.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId))
                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                                        //&& x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                        //&& (x.YeuCauDichVuKyThuatKhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh != null)
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        // hàng chờ thực hiện tiêm,chỉ lấy những dv khác hủy -> khi hoàn thành tiêm thì sẽ xóa hết hàng đợi
                                        )
                             )
                             //==============================================================

                             // dich vụ chưa thực hiện xong
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             //==============================================================

                             // trạng thái thanh toán
                             && (
                                 x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                 )
                             //==============================================================

                            // // check loại hàng đợi
                            // && (
                            //    (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                            //        && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null))
                            //    || (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                            //        && x.KhamSangLocTiemChung != null 
                            //        && x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                            //        && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(a => a.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                            //    )
                            ////==============================================================
                        )
                    .ApplyLike(quyeryInfo.SearchString, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN).ToList();

            var result = query
                .Select(s => new HangDoiTiemChungGridVo
                {
                    Id = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? s.Id : s.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauTiemVacxinId = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? (long?)null : s.Id,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh != null
                        ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault()
                        : 0,
                    NgayKhamSangLoc = s.CreatedOn.Value.Date,
                    SoThuTu = s.PhongBenhVienHangDois.Any() ? s.PhongBenhVienHangDois.OrderByDescending(x => x.Id).Select(x => x.SoThuTu).FirstOrDefault() : Int32.MaxValue
                })
                .GroupBy(x => x.YeuCauTiepNhanId)
                .Select(x => x.First())
                .OrderBy(x => x.SoThuTu)
                .ToList();
            return result;
        }

        public async Task<ICollection<HangDoiTiemChungGridVo>> GetDanhSachChoKhamHienTaiAsyncVer2(HangDoiTiemChungQuyeryInfo quyeryInfo)
        {
            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Include(x => x.YeuCauTiepNhan)
                    .Include(x => x.PhongBenhVienHangDois)
                    .Include(x => x.YeuCauDichVuKyThuatKhamSangLocTiemChung)
                 .Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId

                             // phòng bệnh viện hàng đợi // check loại hàng đợi
                             && (
                                //x.PhongBenhVienHangDois.OrderByDescending(y => y.Id).First().TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                                (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                                    && x.TiemChung == null
                                    && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && (!x.PhongBenhVienHangDois.Any() || (x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                            && x.PhongBenhVienHangDois
                                                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)
                                                                            ))
                                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung)
                                    // hàng chờ khám sàng lọc chỉ lấy những dv đang thực hiện
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                    || (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                                        && x.TiemChung != null
                                        && (!x.PhongBenhVienHangDois.Any()
                                            || x.PhongBenhVienHangDois
                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                                        && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung != null
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null

                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                            .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                        && (
                                                            // phòng hiện tại có vacxin chưa tiêm
                                                            y.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId
                                                            // yêu cầu khám sàng lọc còn vacxin chưa tiêm
                                                            || (y.NoiThucHienId != quyeryInfo.PhongKhamHienTaiId && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                                            )
                                                        && (
                                                            y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                        ))

                                        // kierm tra hàng đợi: nếu có thì tất cả hàng đợi vacxin đều phải đang chờ khám
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && (
                                                                y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            ))
                                                .All(y => !y.PhongBenhVienHangDois.Any() 
                                                          || y.PhongBenhVienHangDois.All(a => a.PhongBenhVienId != quyeryInfo.PhongKhamHienTaiId)
                                                          || y.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId).All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))

                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        // hàng chờ thực hiện tiêm,chỉ lấy những dv khác hủy -> khi hoàn thành tiêm thì sẽ xóa hết hàng đợi
                                        )
                             )
                             //==============================================================

                             // dich vụ chưa thực hiện xong
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             //==============================================================

                             // trạng thái thanh toán
                             && (
                                 x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                 )
                        )
                    .ApplyLike(quyeryInfo.SearchString, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                    .Select(s => new HangDoiTiemChungGridVo
                    {
                        Id = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? s.Id : s.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id,
                        YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                        YeuCauTiemVacxinId = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? (long?)null : s.Id,
                        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        HoTen = s.YeuCauTiepNhan.HoTen,
                        MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                        TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        Tuoi = s.YeuCauTiepNhan.NamSinh != null
                            ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault()
                            : 0,
                        NgayKhamSangLoc = s.CreatedOn.Value.Date,
                        SoThuTu = s.PhongBenhVienHangDois.Any() ? s.PhongBenhVienHangDois.OrderByDescending(x => x.Id).Select(x => x.SoThuTu).FirstOrDefault() : Int32.MaxValue
                    })
                    .ToList();

            query = query
                .GroupBy(x => x.YeuCauTiepNhanId)
                .Select(x => x.First())
                .OrderBy(x => x.SoThuTu)
                .ToList();
            return query;
        }

        //Cập nhật 12/07/2022: fix bug load chậm tiêm chủng
        public async Task<ICollection<HangDoiTiemChungGridVo>> GetDanhSachChoKhamHienTaiAsyncVer3(HangDoiTiemChungQuyeryInfo quyeryInfo)
        {
            var query = new List<HangDoiTiemChungGridVo>();
            if (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc)
            {
                query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId

                             // phòng bệnh viện hàng đợi // check loại hàng đợi
                             && ((x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    //&& (!x.PhongBenhVienHangDois.Any() || (x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                    //                                        && x.PhongBenhVienHangDois
                                    //                                            .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                    //                                            .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)
                                    //                                        ))
                                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                    // hàng chờ khám sàng lọc chỉ lấy những dv đang thực hiện
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                             //==============================================================

                             // dich vụ chưa thực hiện xong
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             //==============================================================

                             // trạng thái thanh toán
                             && (
                                 x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                 )
                        )
                    .ApplyLike(quyeryInfo.SearchString, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                    .Select(s => new HangDoiTiemChungGridVo
                    {
                        //Id = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? s.Id : s.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id,
                        Id = s.Id,
                        YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                        //YeuCauTiemVacxinId = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? (long?)null : s.Id,
                        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        HoTen = s.YeuCauTiepNhan.HoTen,
                        MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                        TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        //Tuoi = s.YeuCauTiepNhan.NamSinh != null
                        //    ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault()
                        //    : 0,
                        Nam = s.YeuCauTiepNhan.NamSinh,
                        NgayKhamSangLoc = s.CreatedOn.Value.Date,
                        //SoThuTu = s.PhongBenhVienHangDois.Any() ? s.PhongBenhVienHangDois.OrderByDescending(x => x.Id).Select(x => x.SoThuTu).FirstOrDefault() : Int32.MaxValue                        
                    })
                    .ToList();
                var yeuCauDichVuKyThuatIds = query.Select(o => o.Id).ToList();
                var allHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
                    .Where(o => o.YeuCauDichVuKyThuatId != null && yeuCauDichVuKyThuatIds.Contains(o.YeuCauDichVuKyThuatId.Value))
                    .Select(h => new ThongTinPhongBenhVienHangDoi
                    {
                        YeuCauDichVuKyThuatId = h.YeuCauDichVuKyThuatId.Value,
                        PhongBenhVienId = h.PhongBenhVienId,
                        TrangThai = h.TrangThai
                    }).ToList();

                query.ForEach(o => o.ThongTinPhongBenhVienHangDois = allHangDoi.Where(h => h.YeuCauDichVuKyThuatId == o.Id).ToList());

                query = query
                    .Where(x => !x.ThongTinPhongBenhVienHangDois.Any() || 
                    (x.ThongTinPhongBenhVienHangDois.Any(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                        && x.ThongTinPhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId).All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)))
                    .ToList();
            }
            else if (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem)
            {
                query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(x => x.NoiThucHienId != null
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                             && x.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId

                             && (x.TiemChung != null
                                        //&& (!x.PhongBenhVienHangDois.Any() || x.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId).All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                                        && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung != null
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null

                                        //&& x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                        //    .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        //                && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                        //                && (
                                        //                    // phòng hiện tại có vacxin chưa tiêm
                                        //                    y.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId
                                        //                    // yêu cầu khám sàng lọc còn vacxin chưa tiêm
                                        //                    || (y.NoiThucHienId != quyeryInfo.PhongKhamHienTaiId && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                        //                    )
                                        //                && (
                                        //                    y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                        //                    || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                        //                    || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                        //                ))

                                        //// kierm tra hàng đợi: nếu có thì tất cả hàng đợi vacxin đều phải đang chờ khám
                                        //&& x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                        //        .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        //                    && (
                                        //                        y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                        //                        || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                        //                        || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                        //                    ))
                                        //        .All(y => !y.PhongBenhVienHangDois.Any()
                                        //                  || y.PhongBenhVienHangDois.All(a => a.PhongBenhVienId != quyeryInfo.PhongKhamHienTaiId)
                                        //                  || y.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId).All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))

                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             // hàng chờ thực hiện tiêm,chỉ lấy những dv khác hủy -> khi hoàn thành tiêm thì sẽ xóa hết hàng đợi

                             )
                             //==============================================================

                             // dich vụ chưa thực hiện xong
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             //==============================================================

                             // trạng thái thanh toán
                             && (
                                 x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                 )
                        )
                    .ApplyLike(quyeryInfo.SearchString, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                    .Select(s => new HangDoiTiemChungGridVo
                    {
                        Id = s.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id,
                        YeuCauTiemChungId = s.TiemChung.Id,
                        YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                        YeuCauTiemVacxinId = s.Id,
                        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        HoTen = s.YeuCauTiepNhan.HoTen,
                        MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                        TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        //Tuoi = s.YeuCauTiepNhan.NamSinh != null
                        //    ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault()
                        //    : 0,
                        Nam = s.YeuCauTiepNhan.NamSinh,
                        NgayKhamSangLoc = s.CreatedOn.Value.Date,
                        //SoThuTu = s.PhongBenhVienHangDois.Any() ? s.PhongBenhVienHangDois.OrderByDescending(x => x.Id).Select(x => x.SoThuTu).FirstOrDefault() : Int32.MaxValue
                    })
                    .ToList();
                var yeuCauDichVuKyThuatKhamSangLocTiemChungIds = query.Select(o => o.Id).ToList();
                var allKhamSangLocTiemChungYeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(o => o.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null && yeuCauDichVuKyThuatKhamSangLocTiemChungIds.Contains(o.YeuCauDichVuKyThuatKhamSangLocTiemChungId.Value))
                    .Select(o => new
                    {
                        o.YeuCauDichVuKyThuatKhamSangLocTiemChungId,
                        o.Id,
                        o.TrangThai,
                        o.NoiThucHienId,
                        o.TrangThaiThanhToan
                    }).ToList();

                var yeuCauDichVuKyThuatTiemVacxinIds = query.Select(o => o.YeuCauTiemVacxinId.GetValueOrDefault()).Concat(allKhamSangLocTiemChungYeuCauDichVuKyThuats.Select(o => o.Id)).Distinct().ToList();
                var allHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
                    .Where(o => o.YeuCauDichVuKyThuatId != null && yeuCauDichVuKyThuatTiemVacxinIds.Contains(o.YeuCauDichVuKyThuatId.Value))
                    .Select(h => new ThongTinPhongBenhVienHangDoi
                    {
                        YeuCauDichVuKyThuatId = h.YeuCauDichVuKyThuatId.Value,
                        PhongBenhVienId = h.PhongBenhVienId,
                        TrangThai = h.TrangThai
                    }).ToList();

                List<long> validIds = new List<long>();
                foreach(var item in query)
                {
                    //hang doi
                    var thongTinPhongBenhVienHangDois = allHangDoi.Where(h => h.YeuCauDichVuKyThuatId == item.Id).ToList();
                    if(!thongTinPhongBenhVienHangDois.Any() ||
                        thongTinPhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId).All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                    {
                        var khamSangLocTiemChungYeuCauDichVuKyThuats = allKhamSangLocTiemChungYeuCauDichVuKyThuats
                            .Where(o => o.YeuCauDichVuKyThuatKhamSangLocTiemChungId == item.Id 
                                        && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                        &&  (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                            || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                            || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                            .ToList();
                        if(khamSangLocTiemChungYeuCauDichVuKyThuats.Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                        && ( // phòng hiện tại có vacxin chưa tiêm
                                                            y.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId
                                                            // yêu cầu khám sàng lọc còn vacxin chưa tiêm
                                                            || (y.NoiThucHienId != quyeryInfo.PhongKhamHienTaiId && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien))))
                        {
                            if(khamSangLocTiemChungYeuCauDichVuKyThuats.All(o=> !allHangDoi.Where(h => h.YeuCauDichVuKyThuatId == o.Id).Any()
                                                                                || allHangDoi.Where(h => h.YeuCauDichVuKyThuatId == o.Id).All(a => a.PhongBenhVienId != quyeryInfo.PhongKhamHienTaiId)
                                                                                || allHangDoi.Where(h => h.YeuCauDichVuKyThuatId == o.Id).Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId).All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)))
                            {
                                validIds.Add(item.Id);
                            }
                        }
                    }
                }
                query = query
                    .Where(x => validIds.Contains(x.Id))
                    .ToList();
            }

            query = query
                .GroupBy(x => x.YeuCauTiepNhanId)
                .Select(x => x.First())
                .OrderBy(x => x.Id)
                .ToList();

            var namHienTai = DateTime.Now.Year;
            foreach (var item in query)
            {
                if (item.Nam.GetValueOrDefault() == 0)
                {
                    item.Tuoi = 0;
                }
                else
                {
                    item.Tuoi = namHienTai - item.Nam.GetValueOrDefault();
                }
            }
            return query;
        }
        public async Task<ICollection<HangDoiTiemChungGridVo>> GetDanhSachChoKhamHienTaiAsyncVer3Old(HangDoiTiemChungQuyeryInfo quyeryInfo)
        {
            var query = new List<HangDoiTiemChungGridVo>();
            if (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc)
            {
                query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId

                             // phòng bệnh viện hàng đợi // check loại hàng đợi
                             && (x.TiemChung == null
                                    && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && (!x.PhongBenhVienHangDois.Any() || (x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                            && x.PhongBenhVienHangDois
                                                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)
                                                                            ))
                                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                    // hàng chờ khám sàng lọc chỉ lấy những dv đang thực hiện
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                             //==============================================================

                             // dich vụ chưa thực hiện xong
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             //==============================================================

                             // trạng thái thanh toán
                             && (
                                 x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                 )
                        )
                    .ApplyLike(quyeryInfo.SearchString, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                    .Select(s => new HangDoiTiemChungGridVo
                    {
                        //Id = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? s.Id : s.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id,
                        Id = s.Id,
                        YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                        //YeuCauTiemVacxinId = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? (long?)null : s.Id,
                        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        HoTen = s.YeuCauTiepNhan.HoTen,
                        MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                        TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        //Tuoi = s.YeuCauTiepNhan.NamSinh != null
                        //    ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault()
                        //    : 0,
                        Nam = s.YeuCauTiepNhan.NamSinh,
                        NgayKhamSangLoc = s.CreatedOn.Value.Date,
                        //SoThuTu = s.PhongBenhVienHangDois.Any() ? s.PhongBenhVienHangDois.OrderByDescending(x => x.Id).Select(x => x.SoThuTu).FirstOrDefault() : Int32.MaxValue
                    })
                    .ToList();
            }
            else if(quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem)
            {
                query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId
                             
                             && (x.TiemChung != null
                                        && (!x.PhongBenhVienHangDois.Any()
                                            || x.PhongBenhVienHangDois
                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                                        && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung != null
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null

                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                            .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                        && (
                                                            // phòng hiện tại có vacxin chưa tiêm
                                                            y.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId
                                                            // yêu cầu khám sàng lọc còn vacxin chưa tiêm
                                                            || (y.NoiThucHienId != quyeryInfo.PhongKhamHienTaiId && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                                            )
                                                        && (
                                                            y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                        ))

                                        // kierm tra hàng đợi: nếu có thì tất cả hàng đợi vacxin đều phải đang chờ khám
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && (
                                                                y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            ))
                                                .All(y => !y.PhongBenhVienHangDois.Any()
                                                          || y.PhongBenhVienHangDois.All(a => a.PhongBenhVienId != quyeryInfo.PhongKhamHienTaiId)
                                                          || y.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId).All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))

                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        // hàng chờ thực hiện tiêm,chỉ lấy những dv khác hủy -> khi hoàn thành tiêm thì sẽ xóa hết hàng đợi
                                        
                             )
                             //==============================================================

                             // dich vụ chưa thực hiện xong
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             //==============================================================

                             // trạng thái thanh toán
                             && (
                                 x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                 )
                        )
                    .ApplyLike(quyeryInfo.SearchString, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                    .Select(s => new HangDoiTiemChungGridVo
                    {
                        Id = s.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id,
                        YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                        YeuCauTiemVacxinId = s.Id,
                        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        HoTen = s.YeuCauTiepNhan.HoTen,
                        MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                        TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        //Tuoi = s.YeuCauTiepNhan.NamSinh != null
                        //    ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault()
                        //    : 0,
                        Nam = s.YeuCauTiepNhan.NamSinh,
                        NgayKhamSangLoc = s.CreatedOn.Value.Date,
                        //SoThuTu = s.PhongBenhVienHangDois.Any() ? s.PhongBenhVienHangDois.OrderByDescending(x => x.Id).Select(x => x.SoThuTu).FirstOrDefault() : Int32.MaxValue
                    })
                    .ToList();
            }

            query = query
                .GroupBy(x => x.YeuCauTiepNhanId)
                .Select(x => x.First())
                .OrderBy(x => x.Id)
                .ToList();

            var namHienTai = DateTime.Now.Year;
            foreach (var item in query)
            {
                if (item.Nam.GetValueOrDefault() == 0)
                {
                    item.Tuoi = 0;
                }
                else
                {
                    item.Tuoi = namHienTai - item.Nam.GetValueOrDefault();
                }
            }
            return query;
        }

        public async Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsync(HangDoiTiemChungDangKhamQuyeryInfo queryInfo)
        {
            var yeuCauKhamTiemChung = _yeuCauDichVuKyThuatRepository.Table
                .Include(x => x.TaiKhoanBenhNhanChis)
                .Include(x => x.DichVuKyThuatBenhVien)
                .Include(x => x.NoiThucHien)
                .Include(x => x.NoiChiDinh)
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                .Include(x => x.NhanVienChiDinh).ThenInclude(x => x.User)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.BenhNhanDiUngThuocs)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.BenhNhanTienSuBenhs)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.PhuongXa)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.LyDoTiepNhan)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.KetQuaSinhHieus)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NhanVienHoanThanhKhamSangLoc).ThenInclude(x => x.User)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NhanVienTheoDoiSauTiem).ThenInclude(x => x.User)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiThucHien)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiChiDinh)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NhanVienChiDinh).ThenInclude(x => x.User)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.NhanVienTiem).ThenInclude(x => x.User)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                .Include(x => x.YeuCauKhamBenh)

                //BVHD-3825
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)

                //BVHD-3800
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)

                .FirstOrDefault(x =>
                    (queryInfo.YeuCauKhamTiemChungId == null || x.Id == queryInfo.YeuCauKhamTiemChungId)
                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung
                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                    && (queryInfo.PhongKhamHienTaiId == null
                        || (x.NoiThucHienId != null))

                    && (
                        // Dùng cho các grid lịch sử -> mục đích là xem chi tiết data
                        queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.LichSuTiem

                        // Dùng cho các grid chưa hoàn thành khám sàng lọc/ tiêm
                        || (queryInfo.LoaiHangDoi != Enums.LoaiHangDoiTiemVacxin.LichSuTiem 
                            && ((queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                                    && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == queryInfo.PhongKhamHienTaiId && a.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham))
                                || (queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                                    && x.KhamSangLocTiemChung != null
                                    && x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                                    && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                        .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                        .Any(y => y.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == queryInfo.PhongKhamHienTaiId
                                                                                   && a.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham)))
                                )
                            )
                        ));
            return yeuCauKhamTiemChung;
        }
        public async Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(HangDoiTiemChungDangKhamQuyeryInfo queryInfo)
        {
            YeuCauDichVuKyThuat yeuCauKhamTiemChung = null;
            var yeuCauKhamTiemChungId = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x =>
                    (queryInfo.YeuCauKhamTiemChungId == null || x.Id == queryInfo.YeuCauKhamTiemChungId)
                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung
                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                    && (queryInfo.PhongKhamHienTaiId == null
                        || (x.NoiThucHienId != null))

                    && (
                        // Dùng cho các grid lịch sử -> mục đích là xem chi tiết data
                        queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.LichSuTiem

                        // Dùng cho các grid chưa hoàn thành khám sàng lọc/ tiêm
                        || (queryInfo.LoaiHangDoi != Enums.LoaiHangDoiTiemVacxin.LichSuTiem
                            && ((queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                                    && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == queryInfo.PhongKhamHienTaiId && a.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham))
                                || (queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                                    && x.KhamSangLocTiemChung != null
                                    && x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                                    && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                        .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                        .Any(y => y.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == queryInfo.PhongKhamHienTaiId
                                                                                   && a.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham)))
                                )
                            )
                        ))
                .Select(x => x.Id)
                .FirstOrDefault();
            if (yeuCauKhamTiemChungId != null && yeuCauKhamTiemChungId != 0)
            {
                yeuCauKhamTiemChung = _yeuCauDichVuKyThuatRepository.Table
                    .Include(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.DichVuKyThuatBenhVien)
                    .Include(x => x.NoiThucHien)
                    .Include(x => x.NoiChiDinh)
                    .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                    .Include(x => x.NhanVienChiDinh).ThenInclude(x => x.User)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.BenhNhanDiUngThuocs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.BenhNhanTienSuBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.PhuongXa)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.LyDoTiepNhan)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.KetQuaSinhHieus)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NhanVienHoanThanhKhamSangLoc).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NhanVienTheoDoiSauTiem).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiThucHien)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiChiDinh)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NhanVienChiDinh).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.NhanVienTiem).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                    .Include(x => x.YeuCauKhamBenh)

                    //BVHD-3825
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)

                    //BVHD-3800
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                    .FirstOrDefault(x => x.Id == yeuCauKhamTiemChungId);

            }
            return yeuCauKhamTiemChung;
        }
        public async Task<YeuCauDichVuKyThuat> GetThongTinYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(HangDoiTiemChungDangKhamQuyeryInfo queryInfo)
        {
            YeuCauDichVuKyThuat yeuCauKhamTiemChung = null;
            var yeuCauKhamTiemChungId = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x =>
                    (queryInfo.YeuCauKhamTiemChungId == null || x.Id == queryInfo.YeuCauKhamTiemChungId)
                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung
                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                    && (queryInfo.PhongKhamHienTaiId == null
                        || (x.NoiThucHienId != null))

                    && (
                        // Dùng cho các grid lịch sử -> mục đích là xem chi tiết data
                        queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.LichSuTiem

                        // Dùng cho các grid chưa hoàn thành khám sàng lọc/ tiêm
                        || (queryInfo.LoaiHangDoi != Enums.LoaiHangDoiTiemVacxin.LichSuTiem
                            && ((queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                                    && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == queryInfo.PhongKhamHienTaiId && a.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham))
                                || (queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                                    && x.KhamSangLocTiemChung != null
                                    && x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                                    && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                        .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                        .Any(y => y.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == queryInfo.PhongKhamHienTaiId
                                                                                   && a.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham)))
                                )
                            )
                        ))
                .Select(x => x.Id)
                .FirstOrDefault();
            if (yeuCauKhamTiemChungId != null && yeuCauKhamTiemChungId != 0)
            {
                yeuCauKhamTiemChung = _yeuCauDichVuKyThuatRepository.Table
                    //.Include(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.DichVuKyThuatBenhVien)
                    .Include(x => x.NoiThucHien)
                    .Include(x => x.NoiChiDinh)
                    .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                    .Include(x => x.NhanVienChiDinh).ThenInclude(x => x.User)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.BenhNhanDiUngThuocs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.BenhNhanTienSuBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.PhuongXa)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.LyDoTiepNhan)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.KetQuaSinhHieus)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NhanVienHoanThanhKhamSangLoc).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NhanVienTheoDoiSauTiem).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiThucHien)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiChiDinh)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NhanVienChiDinh).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.NhanVienTiem).ThenInclude(x => x.User)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                    .Include(x => x.YeuCauKhamBenh)

                    //BVHD-3825
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)

                    //BVHD-3800
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                    .FirstOrDefault(x => x.Id == yeuCauKhamTiemChungId);

            }
            return yeuCauKhamTiemChung;
        }
        public async Task<YeuCauDichVuKyThuat> GetThongTinLuuYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(HangDoiTiemChungDangKhamQuyeryInfo queryInfo)
        {
            YeuCauDichVuKyThuat yeuCauKhamTiemChung = null;
            var yeuCauKhamTiemChungId = queryInfo.YeuCauKhamTiemChungId;
            if (yeuCauKhamTiemChungId == null)
            {
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x =>
                        (queryInfo.YeuCauKhamTiemChungId == null || x.Id == queryInfo.YeuCauKhamTiemChungId)
                        && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung
                        && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                        && (queryInfo.PhongKhamHienTaiId == null
                            || (x.NoiThucHienId != null))

                        && (
                            // Dùng cho các grid lịch sử -> mục đích là xem chi tiết data
                            queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.LichSuTiem

                            // Dùng cho các grid chưa hoàn thành khám sàng lọc/ tiêm
                            || (queryInfo.LoaiHangDoi != Enums.LoaiHangDoiTiemVacxin.LichSuTiem
                                && ((queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                                        && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                        && x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == queryInfo.PhongKhamHienTaiId && a.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham))
                                    || (queryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                                        && x.KhamSangLocTiemChung != null
                                        && x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                                        && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                            .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                            .Any(y => y.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == queryInfo.PhongKhamHienTaiId
                                                                                       && a.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham)))
                                    )
                                )
                            ))
                    .Select(x => x.Id)
                    .FirstOrDefault();
            }
            if (yeuCauKhamTiemChungId != null && yeuCauKhamTiemChungId != 0)
            {
                yeuCauKhamTiemChung = _yeuCauDichVuKyThuatRepository.Table
                    //.Include(x => x.TaiKhoanBenhNhanChis)
                    //.Include(x => x.DichVuKyThuatBenhVien)
                    //.Include(x => x.NoiThucHien)
                    //.Include(x => x.NoiChiDinh)
                    //.Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                    //.Include(x => x.NhanVienChiDinh).ThenInclude(x => x.User)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.BenhNhanDiUngThuocs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.BenhNhanTienSuBenhs)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.PhuongXa)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.LyDoTiepNhan)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.KetQuaSinhHieus)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NhanVienHoanThanhKhamSangLoc).ThenInclude(x => x.User)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NhanVienTheoDoiSauTiem).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiThucHien)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiChiDinh)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NhanVienChiDinh).ThenInclude(x => x.User)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.NhanVienTiem).ThenInclude(x => x.User)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris)//.ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                    //.Include(x => x.YeuCauKhamBenh)

                    //BVHD-3825
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(x => x.ChuongTrinhGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    //.Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)

                    //BVHD-3800
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)

                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                    .FirstOrDefault(x => x.Id == yeuCauKhamTiemChungId);

            }
            return yeuCauKhamTiemChung;
        }

        public async Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungTiepTheoAsync(HangDoiTiemChungQuyeryInfo quyeryInfo)
        {
            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Include(x => x.YeuCauTiepNhan)
                    .Include(x => x.PhongBenhVienHangDois)
                    .Include(x => x.YeuCauDichVuKyThuatKhamSangLocTiemChung)
                 .Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId

                             // phòng bệnh viện hàng đợi // check loại hàng đợi
                             && (
                                //x.PhongBenhVienHangDois.OrderByDescending(y => y.Id).First().TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                                (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                                    && x.TiemChung == null
                                    && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && (!x.PhongBenhVienHangDois.Any() || (x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                            && x.PhongBenhVienHangDois
                                                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)
                                                                            ))
                                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung)
                                    // hàng chờ khám sàng lọc chỉ lấy những dv đang thực hiện
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                    || (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                                        && x.TiemChung != null
                                        && (!x.PhongBenhVienHangDois.Any()
                                            || x.PhongBenhVienHangDois
                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                                        && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung != null
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null

                                        && (
                                            // phòng hiện tại có vacxin chưa tiêm
                                            x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                            .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                        && y.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId
                                                        && (
                                                            y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                        ))
                                            // yêu cầu khám sàng lọc còn vacxin chưa tiêm
                                            || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                          && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                          && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien
                                                          && y.NoiThucHienId != quyeryInfo.PhongKhamHienTaiId
                                                          && (
                                                              y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                              || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                              || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                          )))
                                        && (!x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && (
                                                                y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            ))
                                                .SelectMany(y => y.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)).Any()
                                            || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && (
                                                                y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            ))
                                                .SelectMany(y => y.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId))
                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                                        //&& x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                        //&& (x.YeuCauDichVuKyThuatKhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh != null)
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        // hàng chờ thực hiện tiêm,chỉ lấy những dv khác hủy -> khi hoàn thành tiêm thì sẽ xóa hết hàng đợi
                                        )
                             )
                             //==============================================================

                             // dich vụ chưa thực hiện xong
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             //==============================================================

                             // trạng thái thanh toán
                             && (
                                 x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                 )
                        //==============================================================

                        // // check loại hàng đợi
                        // && (
                        //    (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                        //        && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null))
                        //    || (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                        //        && x.KhamSangLocTiemChung != null 
                        //        && x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                        //        && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(a => a.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                        //    )
                        ////==============================================================
                        )
                    .ApplyLike(quyeryInfo.SearchString, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN).ToList();

            var result = query
                .Select(s => new HangDoiTiemChungGridVo
                {
                    Id = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? s.Id : s.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauTiemVacxinId = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? (long?)null : s.Id,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh != null
                        ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault()
                        : 0,
                    NgayKhamSangLoc = s.CreatedOn.Value.Date,
                    SoThuTu = s.PhongBenhVienHangDois.Any() ? s.PhongBenhVienHangDois.OrderByDescending(x => x.Id).Select(x => x.SoThuTu).FirstOrDefault() : Int32.MaxValue
                })
                .GroupBy(x => x.YeuCauTiepNhanId)
                .Select(x => x.First())
                .OrderBy(x => x.SoThuTu)
                .FirstOrDefault();

            if (result != null)
            {
                var yeuCauKhamTiemChung = await _yeuCauDichVuKyThuatRepository.Table
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.PhuongXa)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.LyDoTiepNhan)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.KetQuaSinhHieus)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung)
                    .FirstOrDefaultAsync(x => x.Id == result.Id);

                return yeuCauKhamTiemChung;
            }

            return null;
        }

        public async Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungTiepTheoAsyncVer2(HangDoiTiemChungQuyeryInfo quyeryInfo)
        {
            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Include(x => x.YeuCauTiepNhan)
                    .Include(x => x.PhongBenhVienHangDois)
                    .Include(x => x.YeuCauDichVuKyThuatKhamSangLocTiemChung)
                 .Where(x => x.NoiThucHienId != null
                             && x.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId

                             // phòng bệnh viện hàng đợi // check loại hàng đợi
                             && (
                                //x.PhongBenhVienHangDois.OrderByDescending(y => y.Id).First().TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                                (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                                    && x.TiemChung == null
                                    && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && (!x.PhongBenhVienHangDois.Any() || (x.PhongBenhVienHangDois.Any(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                            && x.PhongBenhVienHangDois
                                                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham)
                                                                            ))
                                    && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung)
                                    // hàng chờ khám sàng lọc chỉ lấy những dv đang thực hiện
                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                    || (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                                        && x.TiemChung != null
                                        && (!x.PhongBenhVienHangDois.Any()
                                            || x.PhongBenhVienHangDois
                                                .Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId)
                                                .All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))
                                        && x.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung != null
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null

                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                            .Any(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                        && (
                                                            // phòng hiện tại có vacxin chưa tiêm
                                                            y.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId
                                                            // yêu cầu khám sàng lọc còn vacxin chưa tiêm
                                                            || (y.NoiThucHienId != quyeryInfo.PhongKhamHienTaiId && y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                                            )
                                                        && (
                                                            y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                            || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                        ))

                                        // kierm tra hàng đợi: nếu có thì tất cả hàng đợi vacxin đều phải đang chờ khám
                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                            && (
                                                                y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                                                || y.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            ))
                                                .All(y => !y.PhongBenhVienHangDois.Any()
                                                          || y.PhongBenhVienHangDois.All(a => a.PhongBenhVienId != quyeryInfo.PhongKhamHienTaiId)
                                                          || y.PhongBenhVienHangDois.Where(a => a.PhongBenhVienId == quyeryInfo.PhongKhamHienTaiId).All(a => a.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham))

                                        && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        // hàng chờ thực hiện tiêm,chỉ lấy những dv khác hủy -> khi hoàn thành tiêm thì sẽ xóa hết hàng đợi
                                        )
                             )
                             //==============================================================

                             // dich vụ chưa thực hiện xong
                             && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                             //==============================================================

                             // trạng thái thanh toán
                             && (
                                 x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan
                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                 )
                        //==============================================================

                        // // check loại hàng đợi
                        // && (
                        //    (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc
                        //        && (x.KhamSangLocTiemChung == null || x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null))
                        //    || (quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.ThucHienTiem
                        //        && x.KhamSangLocTiemChung != null 
                        //        && x.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                        //        && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(a => a.NoiThucHienId == quyeryInfo.PhongKhamHienTaiId && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                        //    )
                        ////==============================================================
                        )
                    .ApplyLike(quyeryInfo.SearchString, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.BenhNhan.MaBN)
                    .Select(s => new HangDoiTiemChungGridVo
                    {
                        Id = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? s.Id : s.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id,
                        YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                        YeuCauTiemVacxinId = quyeryInfo.LoaiHangDoi == Enums.LoaiHangDoiTiemVacxin.KhamSangLoc ? (long?)null : s.Id,
                        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        HoTen = s.YeuCauTiepNhan.HoTen,
                        MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                        TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                        Tuoi = s.YeuCauTiepNhan.NamSinh != null
                            ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault()
                            : 0,
                        NgayKhamSangLoc = s.CreatedOn.Value.Date,
                        SoThuTu = s.PhongBenhVienHangDois.Any() ? s.PhongBenhVienHangDois.OrderByDescending(x => x.Id).Select(x => x.SoThuTu).FirstOrDefault() : Int32.MaxValue
                    }).ToList();

            var result = query
                .GroupBy(x => x.YeuCauTiepNhanId)
                .Select(x => x.First())
                .OrderBy(x => x.SoThuTu)
                .FirstOrDefault();

            if (result != null)
            {
                var yeuCauKhamTiemChung = await _yeuCauDichVuKyThuatRepository.Table
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.PhuongXa)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.LyDoTiepNhan)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.KetQuaSinhHieus)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung)
                    .FirstOrDefaultAsync(x => x.Id == result.Id);

                return yeuCauKhamTiemChung;
            }

            return null;
        }

        //Cập nhật 12/07/2022: fix bug load chậm tiêm chủng
        public async Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungTiepTheoAsyncVer3(HangDoiTiemChungQuyeryInfo quyeryInfo)
        {
            var query = await GetDanhSachChoKhamHienTaiAsyncVer3(quyeryInfo);
            var result = query.FirstOrDefault();

            if (result != null)
            {
                var yeuCauKhamTiemChung = await _yeuCauDichVuKyThuatRepository.Table
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.PhuongXa)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.LyDoTiepNhan)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.KetQuaSinhHieus)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung)
                    .FirstOrDefaultAsync(x => x.Id == result.Id);

                return yeuCauKhamTiemChung;
            }

            return null;
        }
        #endregion

        #region Get list lookup
        public List<LookupItemVo> GetListTrangThaiTiemVacxin()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.TrangThaiTiemChung>().Select(item => new LookupItemVo()
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item)
            }).ToList();
            return listEnum;
        }

        public async Task<List<LookupItemTemplateVo>> GetListPhongBenhVienAsync(DropDownListRequestModel model)
        {
            var lstPhongBenhVien = await
                _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == model.Id)
                    .Select(item => new LookupItemTemplateVo()
                    {
                        DisplayName = item.Ten,
                        Ma = item.Ma,
                        KeyId = item.Id
                    }).Union(
                        _phongBenhVienRepository.TableNoTracking
                            .Where(x => x.IsDisabled != true && x.Id != model.Id)
                            .Select(item => new LookupItemTemplateVo()
                            {
                                DisplayName = item.Ten,
                                Ma = item.Ma,
                                KeyId = item.Id
                            }))
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName).Distinct()
                    .Take(model.Take)
                    .ToListAsync();
            return lstPhongBenhVien;
        }

        public async Task<List<LookupItemTemplateVo>> GetListVacXinAsync(DropDownListRequestModel model)
        {
            var lstVacxin = await
                _dichVuKyThuatBenhVienTiemChungRepository.TableNoTracking
                    .Select(item => new LookupItemTemplateVo()
                    {
                        KeyId = item.DichVuKyThuatBenhVienId,
                        Ma = item.DichVuKyThuatBenhVien.Ma,
                        DisplayName = item.DichVuKyThuatBenhVien.Ten
                    })
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName).Distinct()
                    .Take(model.Take)
                    .ToListAsync();
            return lstVacxin;
        }
        #endregion

        #region Cập nhật bỏ bớt include

        public async Task<bool?> KiemTraLanTiepNhanLaCapCuuAsync(long yeuCauTiepNhanId)
        {
            var laCapCuu = await BaseRepository.TableNoTracking
                .Where(x => x.Id == yeuCauTiepNhanId)
                .Select(x => x.LaCapCuu).FirstOrDefaultAsync();
            return laCapCuu;
        }

        public async Task<bool> KiemTraCoDichVuKhuyenMaiAsync(long benhNhanId)
        {
            var cokhuyenMai = false;
            var lstChuongTrinhGoiId = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                            && x.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && x.DaQuyetToan != true
                            && x.NgungSuDung != true)
                .Select(x => x.ChuongTrinhGoiDichVuId)
                .Distinct().ToList();

            //nhìn có vẻ hơi phèn phèn, nhưng vài trường hợp sẽ hạn chế được query
            if (lstChuongTrinhGoiId.Any())
            {
                cokhuyenMai = _chuongTrinhGoiDichVuKhamBenhRepository.TableNoTracking.Any(x => lstChuongTrinhGoiId.Contains(x.ChuongTrinhGoiDichVuId));
                if (!cokhuyenMai)
                {
                    cokhuyenMai = _chuongTrinhGoiDichVuKyThuatRepository.TableNoTracking.Any(x => lstChuongTrinhGoiId.Contains(x.ChuongTrinhGoiDichVuId));
                }
                if (!cokhuyenMai)
                {
                    cokhuyenMai = _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository.TableNoTracking.Any(x => lstChuongTrinhGoiId.Contains(x.ChuongTrinhGoiDichVuId));
                }
                if (!cokhuyenMai)
                {
                    cokhuyenMai = _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository.TableNoTracking.Any(x => lstChuongTrinhGoiId.Contains(x.ChuongTrinhGoiDichVuId));
                }
            }

            return cokhuyenMai;
        }

        public async Task<List<ThongTinLoTheoXuatChiTietVo>> GetThongTinSoLoAsync(List<long> xuatChiTietIds)
        {
            var lstThongTinSoLo = new List<ThongTinLoTheoXuatChiTietVo>();
            lstThongTinSoLo = _xuatKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(x => xuatChiTietIds.Contains(x.Id))
                .SelectMany(x => x.XuatKhoDuocPhamChiTietViTris)
                .Select(x => new ThongTinLoTheoXuatChiTietVo()
                {
                    XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietId,
                    NhapKhoChiTietId = x.NhapKhoDuocPhamChiTietId,
                    SoLo = x.NhapKhoDuocPhamChiTiet.Solo
                }).Distinct().ToList();

            return lstThongTinSoLo;
        }
        #endregion
    }
}
