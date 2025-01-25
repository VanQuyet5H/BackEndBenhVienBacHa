using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.XetNghiems;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using ZXing;

namespace Camino.Services.DuyetKetQuaXetNghiems
{
    [ScopedDependency(ServiceType = typeof(IDuyetKetQuaXetNghiemService))]
    public class DuyetKetQuaXetNghiemService : MasterFileService<PhienXetNghiem>, IDuyetKetQuaXetNghiemService
    {
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;
        private readonly IRepository<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> _dichVuXetNghiemRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<User> _user;
        private readonly IRepository<CauHinhNguoiDuyetTheoNhomDichVu> _cauHinhNguoiDuyetTheoNhomDichVuRepository;

        private readonly IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.XetNghiems.PhieuGoiMauXetNghiem> _phieuGoiMauXetNghiemRepo;
        private readonly IRepository<MauXetNghiem> _mauXetNghiemRepo;
        private readonly IRepository<MayXetNghiem> _mayXetNghiemRepo;
        private readonly IRepository<Camino.Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepo;
        private readonly IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepo;
        private readonly IRepository<Camino.Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepo;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepo;
        private readonly IRepository<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanVienRepo;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;


        public DuyetKetQuaXetNghiemService(
            IRepository<PhienXetNghiem> repository,
            IRepository<Template> templateRepository,
            IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<User> user,
            IRepository<CauHinhNguoiDuyetTheoNhomDichVu> cauHinhNguoiDuyetTheoNhomDichVuRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            IRepository<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTietRepository,
            IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> dichVuXetNghiemRepository,
             IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> yeuCauTiepNhanRepository,
             IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
             IRepository<Camino.Core.Domain.Entities.XetNghiems.PhieuGoiMauXetNghiem> phieuGoiMauXetNghiemRepo,
             IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
             IRepository<MauXetNghiem> mauXetNghiemRepo,
             IRepository<MayXetNghiem> mayXetNghiemRepo,
             IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepo,
             IRepository<HopDongKhamSucKhoeNhanVien> hopDongKhamSucKhoeNhanVienRepo,
             IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepo,
             IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
             IRepository<Camino.Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepo,
             IRepository<Camino.Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepo
        ) : base(repository)
        {
            _templateRepository = templateRepository;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _userAgentHelper = userAgentHelper;
            _user = user;
            _ketQuaXetNghiemChiTietRepository = ketQuaXetNghiemChiTietRepository;
            _dichVuXetNghiemRepository = dichVuXetNghiemRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _cauHinhRepository = cauHinhRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _cauHinhNguoiDuyetTheoNhomDichVuRepository = cauHinhNguoiDuyetTheoNhomDichVuRepository;
            _phieuGoiMauXetNghiemRepo = phieuGoiMauXetNghiemRepo;
            _mauXetNghiemRepo = mauXetNghiemRepo;
            _mayXetNghiemRepo = mayXetNghiemRepo;
            _nhanVienRepo = nhanVienRepo;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _hopDongKhamSucKhoeNhanVienRepo = hopDongKhamSucKhoeNhanVienRepo;
            _nhomDichVuBenhVienRepo = nhomDichVuBenhVienRepo;
            _noiTruPhieuDieuTriRepo = noiTruPhieuDieuTriRepo;
            _phongBenhVienRepo = phongBenhVienRepo;
        }

        public PhienXetNghiem GetChiTietById(long id)
        {
            var result = BaseRepository.GetById(id
                ,
                u => u
                .Include(x => x.MauXetNghiems)

                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiThucHien)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.HopDongKhamSucKhoe).ThenInclude(x => x.CongTyKhamSucKhoe)
                .Include(x => x.BenhNhan)
                .Include(x => x.PhienXetNghiemChiTiets)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.MayXetNghiem)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                .Include(o => o.MauXetNghiems)

                //BVHD-3800
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                );

            //Explicit loading
            var phienXetNghiemChiTiets = BaseRepository.Context.Entry(result).Collection(o => o.PhienXetNghiemChiTiets);
            phienXetNghiemChiTiets.Query()
                .Include(x => x.KetQuaXetNghiemChiTiets)
                //.Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                //.Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                //.Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
                //.Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                //.Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
                //.Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.MayXetNghiem)
                .Include(x => x.NhomDichVuBenhVien)
                .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
                .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
                .Load();

            foreach(var phienXetNghiemChiTiet in result.PhienXetNghiemChiTiets)
            {
                var ketQuaXetNghiemChiTiets = BaseRepository.Context.Entry(phienXetNghiemChiTiet).Collection(o => o.KetQuaXetNghiemChiTiets);
                ketQuaXetNghiemChiTiets.Query()
                    .Include(x => x.DichVuXetNghiem)
                    .Include(x => x.NhomDichVuBenhVien)
                    .Include(x => x.YeuCauDichVuKyThuat)
                    .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                    .Include(x => x.DichVuKyThuatBenhVien)
                    .Include(x => x.MayXetNghiem)
                    .Load();
            }

            return result;
        }
        public List<LookupItemVo> GetTenMayXetNghiems()
        {
            return _mayXetNghiemRepo.TableNoTracking
                .Select(o => new LookupItemVo
                {
                    KeyId = o.Id,
                    DisplayName = o.Ten
                })
                .ToList();
        }
        public List<LookupItemVo> GetTenNhanViens()
        {
            return _nhanVienRepo.TableNoTracking
                .Select(o => new LookupItemVo
                {
                    KeyId = o.Id,
                    DisplayName = o.User.HoTen
                })
                .ToList();
        }
        public List<LookupItemVo> GetTenDichVuXetNghiems()
        {
            return _dichVuXetNghiemRepository.TableNoTracking
                .Select(o => new LookupItemVo
                {
                    KeyId = o.Id,
                    DisplayName = o.Ten
                })
                .ToList();
        }
        public PhienXetNghiemDataVo GetPhienXetNghiemData(long id)
        {
            var thongTinPhienXetNghiem = BaseRepository.TableNoTracking
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    PhienXetNghiemChiTiets = o.PhienXetNghiemChiTiets.Select(ct => new
                    {
                        ct.Id,
                        ct.YeuCauDichVuKyThuatId,
                        ct.NhanVienKetLuanId
                    }).ToList(),
                    o.YeuCauTiepNhanId,
                    o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.YeuCauTiepNhan.DiaChiDayDu,
                    o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                    o.YeuCauTiepNhan.HoTen,
                    o.YeuCauTiepNhan.NgaySinh,
                    o.YeuCauTiepNhan.ThangSinh,
                    o.YeuCauTiepNhan.NamSinh,
                    o.YeuCauTiepNhan.CoBHYT,
                    o.YeuCauTiepNhan.CoBHTN,
                    o.YeuCauTiepNhan.BHYTMucHuong,
                    o.YeuCauTiepNhan.BHYTMaSoThe,
                    o.YeuCauTiepNhan.GioiTinh,
                    o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    o.YeuCauTiepNhan.SoDienThoaiDisplay,
                    o.YeuCauTiepNhan.NguoiLienHeSoDienThoai,
                    o.YeuCauTiepNhan.LaCapCuu,
                    o.YeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    o.BarCodeId,
                    o.GhiChu,
                    o.KetLuan,
                    o.NhanVienThucHienId,
                    HoTenNhanVienThucHien = o.NhanVienThucHienId != null ? o.NhanVienThucHien.User.HoTen : null
                })
                .FirstOrDefault();

            if (thongTinPhienXetNghiem == null)
                return null;

            var mauXetNghiems = _mauXetNghiemRepo.TableNoTracking
                .Where(o => o.PhienXetNghiemId == thongTinPhienXetNghiem.Id && o.BarCodeId == thongTinPhienXetNghiem.BarCodeId)
                .Select(o => new
                {
                    o.Id,
                    SoPhieu = o.PhieuGoiMauXetNghiemId != null ? o.PhieuGoiMauXetNghiem.SoPhieu : ""
                })
                .ToList();

            //var nhanVienThucHien = _nhanVienRepo.TableNoTracking
            //    .Where(o => o.Id == thongTinPhienXetNghiem.NhanVienThucHienId)
            //    .Select(o => new
            //    {
            //        HocHamHocVi = o.HocHamHocViId != null ? o.HocHamHocVi.Ma : "",
            //        NhomChucDanh = o.ChucDanhId != null ? o.ChucDanh.NhomChucDanh.Ma : "",
            //        o.User.HoTen
            //    })
            //    .FirstOrDefault();

            //var nguoiThucHien = returnStringTen(nhanVienThucHien.HocHamHocVi, nhanVienThucHien.NhomChucDanh, nhanVienThucHien.HoTen);

            //var phienXetNghiemChiTietIds = thongTinPhienXetNghiem.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).Select(o => o.Id).ToList();
            //var yeuCauDichVuKyThuatIds = thongTinPhienXetNghiem.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).Select(o => o.YeuCauDichVuKyThuatId).ToList();
            var phienXetNghiemChiTietIds = thongTinPhienXetNghiem.PhienXetNghiemChiTiets.Select(o => o.Id).ToList();
            var yeuCauDichVuKyThuatIds = thongTinPhienXetNghiem.PhienXetNghiemChiTiets.Select(o => o.YeuCauDichVuKyThuatId).ToList();

            var phienXetNghiemChiTietDatas = _phienXetNghiemChiTietRepository.TableNoTracking
                .Where(o => phienXetNghiemChiTietIds.Contains(o.Id))
                .Select(o => new PhienXetNghiemChiTietDataVo
                {
                    Id = o.Id,
                    YeuCauDichVuKyThuatId = o.YeuCauDichVuKyThuatId,
                    TenDichVu = o.YeuCauDichVuKyThuat.TenDichVu,
                    NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
                    TenNhomDichVuBenhVien = o.NhomDichVuBenhVien.Ten,
                    LoaiKitThu = o.YeuCauDichVuKyThuat.LoaiKitThu,
                    NoiChiDinhId = o.YeuCauDichVuKyThuat.NoiChiDinhId,
                    NhanVienChiDinhId = o.YeuCauDichVuKyThuat.NhanVienChiDinhId,
                    LoaiMauXetNghiem = o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                    YeuCauKhamBenhId = o.YeuCauDichVuKyThuat.YeuCauKhamBenhId,
                    NoiTruPhieuDieuTriId = o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId,
                    DaGoiDuyet = o.DaGoiDuyet,
                    ThoiDiemNhanMau = o.ThoiDiemNhanMau,
                    ThoiDiemKetLuan = o.ThoiDiemKetLuan,
                    ThoiDiemLayMau = o.ThoiDiemLayMau,
                    NhanVienLayMauId = o.NhanVienLayMauId,
                    NhanVienNhanMauId = o.NhanVienNhanMauId,
                    KetQuaXetNghiemChiTiets = o.KetQuaXetNghiemChiTiets.ToList()
                }).ToList();

            var phongBenhViens = _phongBenhVienRepo.TableNoTracking.Select(o => new
            {
                o.Id,
                o.Ten,
                TenKhoaPhong = o.KhoaPhong.Ten
            }).ToList();

            var resultData = new PhienXetNghiemDataVo
            {
                Id = id,
                YeuCauTiepNhanId = thongTinPhienXetNghiem.YeuCauTiepNhanId,
                MaSoBHYT = thongTinPhienXetNghiem.BHYTMaSoThe,
                CoBHYT = thongTinPhienXetNghiem.CoBHYT,
                CoBHTN = thongTinPhienXetNghiem.CoBHTN,
                BarCodeID = thongTinPhienXetNghiem.BarCodeId,
                MaYeuCauTiepNhan = thongTinPhienXetNghiem.MaYeuCauTiepNhan,
                MaBN = thongTinPhienXetNghiem.MaBN,
                HoTen = thongTinPhienXetNghiem.HoTen,
                NgaySinh = thongTinPhienXetNghiem.NgaySinh,
                ThangSinh = thongTinPhienXetNghiem.ThangSinh,
                NamSinh = thongTinPhienXetNghiem.NamSinh,
                GioiTinh = thongTinPhienXetNghiem.GioiTinh,
                BHYTMucHuong = thongTinPhienXetNghiem.BHYTMucHuong,
                LoaiYeuCauTiepNhan = thongTinPhienXetNghiem.LoaiYeuCauTiepNhan,
                SoDienThoai = thongTinPhienXetNghiem.SoDienThoaiDisplay ?? thongTinPhienXetNghiem.NguoiLienHeSoDienThoai?.ApplyFormatPhone(),
                DiaChi = thongTinPhienXetNghiem.DiaChiDayDu,
                NguoiThucHienId = thongTinPhienXetNghiem.NhanVienThucHienId,
                NguoiThucHien = thongTinPhienXetNghiem.HoTenNhanVienThucHien,
                GhiChu = thongTinPhienXetNghiem.GhiChu,
                KetLuan = thongTinPhienXetNghiem.KetLuan,
                PhienXetNghiemChiTietDataVos = phienXetNghiemChiTietDatas
            };

            if (thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
            {
                //var yeuCauKhamBenhIds = phienXetNghiemChiTietDatas
                //    .Where(o => o.YeuCauKhamBenhId != null)
                //    .Select(o => o.YeuCauKhamBenhId.Value)
                //    .ToList();

                var yeuCauKhamBenhChanDoans = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(o => o.YeuCauTiepNhanId == thongTinPhienXetNghiem.YeuCauTiepNhanId && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.ChanDoanSoBoICDId != null)
                    .Select(o => new
                    {
                        o.Id,
                        //ChanDoan = o.ChanDoanSoBoICDId != null ? (o.ChanDoanSoBoICD.Ma + "-" + o.ChanDoanSoBoICD.TenTiengViet) : "",
                        DienGiai = o.ChanDoanSoBoGhiChu
                    }).ToList();

                resultData.ChanDoan = yeuCauKhamBenhChanDoans.Select(o=>o.DienGiai).Distinct().Join("; ");
            }
            else if (thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                var noiTruPhieuDieuTriIds = phienXetNghiemChiTietDatas
                    .Where(o => o.NoiTruPhieuDieuTriId != null)
                    .Select(o => o.NoiTruPhieuDieuTriId.Value)
                    .ToList();

                var noiTruPhieuDieuTriChanDoans = _noiTruPhieuDieuTriRepo.TableNoTracking
                    .Where(o => noiTruPhieuDieuTriIds.Contains(o.Id))
                    .Select(o => new
                    {
                        o.Id,
                        //ChanDoan = o.ChanDoanChinhICDId != null ? (o.ChanDoanChinhICD.Ma + "-" + o.ChanDoanChinhICD.TenTiengViet) : "",
                        DienGiai = o.ChanDoanChinhGhiChu
                    }).ToList();
                resultData.ChanDoan = noiTruPhieuDieuTriChanDoans.Select(o => o.DienGiai).Distinct().Join("; ");
            }
            else if (thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            {
                resultData.ChanDoan = "Khám sức khỏe";
            }

            if (thongTinPhienXetNghiem.HopDongKhamSucKhoeNhanVienId != null)
            {                
                var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepo.TableNoTracking
                    .Where(o => o.Id == thongTinPhienXetNghiem.HopDongKhamSucKhoeNhanVienId)
                    .Select(o => new
                    {
                        o.Id,
                        o.STTNhanVien,
                        TenCty = o.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten
                    }).First();

                resultData.TenCongTy = hopDongKhamSucKhoeNhanVien.TenCty;
            }
            resultData.LaCapCuu = thongTinPhienXetNghiem.LaCapCuu;
            if (resultData.LaCapCuu == null && thongTinPhienXetNghiem.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
            {
                resultData.LaCapCuu = _yeuCauTiepNhanRepository.TableNoTracking.Where(o => o.Id == thongTinPhienXetNghiem.YeuCauTiepNhanNgoaiTruCanQuyetToanId).Select(o => o.LaCapCuu).FirstOrDefault();
            }

            resultData.Phong = phienXetNghiemChiTietDatas.Where(o=>o.NoiChiDinhId != null)
                .Select(o => phongBenhViens.FirstOrDefault(p=>p.Id == o.NoiChiDinhId)?.Ten).Distinct().Join(", ");
            resultData.KhoaChiDinh = phienXetNghiemChiTietDatas.Where(o => o.NoiChiDinhId != null)
                .Select(o => phongBenhViens.FirstOrDefault(p => p.Id == o.NoiChiDinhId)?.TenKhoaPhong).Distinct().Join(", ");

            if (phienXetNghiemChiTietDatas.All(z => z.ThoiDiemKetLuan != null))
            {
                resultData.TrangThai = null;
            }
            else if (phienXetNghiemChiTietDatas.All(z => z.ThoiDiemKetLuan == null && z.KetQuaXetNghiemChiTiets.All(kq => string.IsNullOrEmpty(kq.GiaTriTuMay) && string.IsNullOrEmpty(kq.GiaTriNhapTay))))
            {
                resultData.TrangThai = true;
            }
            else
            {
                resultData.TrangThai = false;
            }
            return resultData;
            //var lstYeuCauDichVuKyThuatId = result.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

            //var listChiTiet = new List<KetQuaXetNghiemChiTiet>();

            //foreach (var ycId in lstYeuCauDichVuKyThuatId)
            //{
            //    if (!result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
            //    var res = result.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
            //    listChiTiet.AddRange(res);
            //}

            //listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            //resultData.ChiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<DuyetKqXetNghiemChiTietViewModel>(), true);


            //// BVHD-3919
            //if (resultData.ChiTietKetQuaXetNghiems.Count() != 0)
            //{
            //    foreach (var item in resultData.ChiTietKetQuaXetNghiems)
            //    {
            //        item.NguoiThucHien = result?.NhanVienThucHien?.User?.HoTen;
            //    }
            //}
            //// 


            //var dichVuSarV2Id2 = await _duyetKqXetNghiemService.CauHinhDichVuTestSarsCovids();
            //var dropDownModel = new DropDownListRequestModel();
            //var loaiKitThus = await _duyetKqXetNghiemService.DichVuTestSarsCovids(dropDownModel);
            //foreach (var detail in resultData.ChiTietKetQuaXetNghiems)
            //{
            //    detail.DanhSachLoaiMauDaCoKetQua = resultData.ChiTietKetQuaXetNghiems
            //        .Where(p => p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).Select(p => p.LoaiMau)
            //        .Distinct().ToList();

            //    if (dichVuSarV2Id2.FirstOrDefault(c => c.DichVuKyThuatBenhVienId == detail.DichVuKyThuatBenhVienId) != null)
            //    {
            //        detail.LaDichVuSarCovid2 = true;
            //    }
            //    if (loaiKitThus.FirstOrDefault(c => c.DisplayName == detail.LoaiKitThu) != null)
            //    {
            //        detail.LoaiKitThuId = loaiKitThus.FirstOrDefault(c => c.DisplayName.Contains(detail.LoaiKitThu))?.KeyId;
            //    }
            //    var lstTongCong = result.YeuCauTiepNhan.YeuCauDichVuKyThuats
            //        .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
            //        .Select(p => p.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription())
            //        .Distinct().Where(p => p != null).ToList();


            //    var lstLoaiMauKhongDat = new List<string>();

            //    foreach (var loaiMau in lstTongCong)
            //    {
            //        var mauXetNghiem = result.MauXetNghiems
            //            .Where(p => p.LoaiMauXetNghiem.GetDescription() == loaiMau && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).LastOrDefault();
            //        if (mauXetNghiem != null && mauXetNghiem.DatChatLuong != true)
            //        {
            //            lstLoaiMauKhongDat.Add(mauXetNghiem.LoaiMauXetNghiem.GetDescription());
            //        }
            //    }

            //    detail.DanhSachLoaiMau = lstTongCong;
            //    detail.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
            //    detail.IsParent = detail.DaGoiDuyet == true && detail.IdChilds.Count == 0;
            //}

            //#region BVHD-3941
            //if (result.YeuCauTiepNhan?.CoBHTN == true)
            //{
            //    resultData.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.YeuCauTiepNhanId).Result;
            //}
            //#endregion
        }

        public List<CauHinhNguoiDuyetTheoNhomDichVu> GetCauHinhNguoiDuyetTheoNhomDichVu()
        {
            return _cauHinhNguoiDuyetTheoNhomDichVuRepository.TableNoTracking.ToList();
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryObject = JsonConvert.DeserializeObject<DuyetKetQuaXetNghiemSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryObject.FromDate) || !string.IsNullOrEmpty(queryObject.ToDate))
                {
                    DateTime denNgay;
                    queryObject.FromDate.TryParseExactCustom(out fromDate);

                    if (string.IsNullOrEmpty(queryObject.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryObject.ToDate.TryParseExactCustom(out denNgay);
                    }
                    toDate = denNgay.AddSeconds(59).AddMilliseconds(999);
                    //query = query.Where(p => p.NgayThucHien >= tuNgay && p.NgayThucHien <= denNgay);
                }
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.StartDate != null)
                //{
                //    fromDate = queryObject.RangeDuyet.StartDate.GetValueOrDefault().Date;
                //    //query = query.Where(p => tuNgay <= p.NgayDuyetKq);
                //}
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.EndDate != null)
                //{
                //    toDate = queryObject.RangeDuyet.EndDate.GetValueOrDefault().Date;
                //    //query = query.Where(p => denNgay >= p.NgayDuyetKq);
                //}
            }

            var query = BaseRepository.TableNoTracking
                            .Where(p => fromDate <= p.ThoiDiemBatDau && toDate >= p.ThoiDiemBatDau)
                            //.Where(p => p.PhienXetNghiemChiTiets.Any(z => z.KetQuaXetNghiemChiTiets.Any()))
                            .Select(s => new DuyetKetQuaXetNghiemGridVo
                            {
                                Id = s.Id,
                                YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                Barcode = s.BarCodeId,
                                BarcodeNumber = s.BarCodeNumber + "",
                                MaTn = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBn = s.BenhNhan.MaBN,
                                HoTen = s.BenhNhan.HoTen,
                                GioiTinh = s.BenhNhan.GioiTinh,
                                NamSinh = s.BenhNhan.NamSinh != null ? s.BenhNhan.NamSinh.ToString() : string.Empty,
                                DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                                NguoiThucHien = s.NhanVienThucHien.User.HoTen,
                                NgayThucHien = s.ThoiDiemBatDau,
                                NguoiDuyetKq = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : "",
                                NgayDuyetKq = s.ThoiDiemKetLuan,
                                ChoKetQua = s.ChoKetQua,//s.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan == null && z.KetQuaXetNghiemChiTiets.All(kq => string.IsNullOrEmpty(kq.GiaTriTuMay) && string.IsNullOrEmpty(kq.GiaTriNhapTay))),
                                DaCoKetQua = s.ThoiDiemKetLuan != null,
                            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryObject = JsonConvert.DeserializeObject<DuyetKetQuaXetNghiemSearch>(queryInfo.AdditionalSearchString);

                //if (!string.IsNullOrEmpty(queryObject.FromDate) || !string.IsNullOrEmpty(queryObject.ToDate))
                //{
                //    DateTime denNgay;
                //    queryObject.FromDate.TryParseExactCustom(out var tuNgay);

                //    if (string.IsNullOrEmpty(queryObject.ToDate))
                //    {
                //        denNgay = DateTime.Now;
                //    }
                //    else
                //    {
                //        queryObject.ToDate.TryParseExactCustom(out denNgay);
                //    }
                //    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                //    query = query.Where(p => p.NgayThucHien >= tuNgay && p.NgayThucHien <= denNgay);
                //}
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.StartDate != null)
                //{
                //    var tuNgay = queryObject.RangeDuyet.StartDate.GetValueOrDefault().Date;
                //    query = query.Where(p => tuNgay <= p.NgayDuyetKq);
                //}
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.EndDate != null)
                //{
                //    var denNgay = queryObject.RangeDuyet.EndDate.GetValueOrDefault().Date;
                //    query = query.Where(p => denNgay >= p.NgayDuyetKq);
                //}

                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.HoTen,
                         g => g.Barcode,
                         g => g.MaTn,
                         g => g.MaBn
                   );
                }

                if (queryObject.DangThucHien == true && queryObject.ChoDuyet == false && queryObject.DaDuyet == false)
                {
                    query = query.Where(p => p.ChoKetQua == true);
                }
                else if (queryObject.DangThucHien == true && queryObject.ChoDuyet == true && queryObject.DaDuyet == false)
                {
                    query = query.Where(p => p.DaCoKetQua == false);
                }
                else if (queryObject.DangThucHien == true && queryObject.ChoDuyet == false && queryObject.DaDuyet == true)
                {
                    query = query.Where(p => p.ChoKetQua == true || p.DaCoKetQua == true);
                }
                else if (queryObject.DangThucHien == false && queryObject.ChoDuyet == true && queryObject.DaDuyet == false)
                {
                    query = query.Where(p => p.ChoKetQua == false && p.DaCoKetQua == false);
                }
                else if (queryObject.DangThucHien == false && queryObject.ChoDuyet == true && queryObject.DaDuyet == true)
                {
                    query = query.Where(p => p.ChoKetQua == false);
                }
                else if (queryObject.DangThucHien == false && queryObject.ChoDuyet == false && queryObject.DaDuyet == true)
                {
                    query = query.Where(p => p.DaCoKetQua == true);
                }
            }
            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //Task<List<DuyetKetQuaXetNghiemGridVo>> queryTask;
            IQueryable<DuyetKetQuaXetNghiemGridVo> queryTask;
            //if (queryInfo.Sort != null && queryInfo.Sort.Any(o => o.Field == nameof(DuyetKetQuaXetNghiemGridVo.TrangThai)))
            //{
            //    if (queryInfo.Sort.First(o => o.Field == nameof(DuyetKetQuaXetNghiemGridVo.TrangThai)).Dir == "desc")
            //    {
            //        //queryTask = query.OrderByDescending(o => (o.ChoKetQua == false && o.DaCoKetQua == false) ? 1 : (o.ChoKetQua == true ? 2 : 3))
            //        //    .ThenByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip)
            //        //    .Take(queryInfo.Take).ToListAsync();
            //        queryTask = query.OrderByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip).Take(queryInfo.Take);
            //    }
            //    else
            //    {
            //        //queryTask = query.OrderBy(o => (o.ChoKetQua == false && o.DaCoKetQua == false) ? 1 : (o.ChoKetQua == true ? 2 : 3))
            //        //    .ThenByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip)
            //        //    .Take(queryInfo.Take).ToListAsync();
            //        queryTask = query.OrderByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip).Take(queryInfo.Take);
            //    }

            //}
            //else
            //{
            //    //queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    //    .Take(queryInfo.Take).ToListAsync();
            //    queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take);
            //}
            queryTask = query.OrderByDescending(z => z.NgayThucHien).Skip(queryInfo.Skip).Take(queryInfo.Take);
            //await Task.WhenAll(countTask, queryTask);
            var data = queryTask.ToList();
            var count = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            data.ForEach(o => o.ChoDuyetKetQua = (o.ChoKetQua == false && o.DaCoKetQua == false));
            return new GridDataSource { Data = data.ToArray(), TotalRowCount = count };
        }

        //public async Task<GridDataSource> GetDataForGridAsyncOld(QueryInfo queryInfo, bool forExportExcel = false)
        //{
        //    BuildDefaultSortExpression(queryInfo);
        //    ReplaceDisplayValueSortExpression(queryInfo);

        //    if (forExportExcel)
        //    {
        //        queryInfo.Skip = 0;
        //        queryInfo.Take = 20000;
        //    }

        //    var queryObject = new DuyetKetQuaXetNghiemSearch();

        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        queryObject = JsonConvert.DeserializeObject<DuyetKetQuaXetNghiemSearch>(queryInfo.AdditionalSearchString);
        //    }

        //    if (queryObject != null && queryObject.ChoDuyet == false && queryObject.DaDuyet == false)
        //    {
        //        queryObject.ChoDuyet = true;
        //        queryObject.DaDuyet = true;
        //    }

        //    var queryChoDuyetChayLai = GetData(true, false, false, queryInfo, queryObject);
        //    var queryChoDuyet = GetData(false, true, false, queryInfo, queryObject);
        //    var queryDaDuyet = GetData(false, false, true, queryInfo, queryObject);

        //    var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuyetKetQuaXetNghiemGridVo()).AsQueryable();

        //    if (queryObject != null && queryObject.ChoDuyet)
        //    {
        //        query = query.Concat(queryChoDuyet);
        //        query = query.Concat(queryChoDuyetChayLai);
        //    }

        //    if (queryObject != null && queryObject.DaDuyet)
        //    {
        //        query = query.Concat(queryDaDuyet);
        //    }

        //    if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayThucHien asc,Id asc") && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
        //    {
        //        query = query.OrderBy(queryInfo.SortString);
        //    }

        //    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
        //    var queryTask = query.Skip(queryInfo.Skip)
        //        .Take(queryInfo.Take).ToArrayAsync();
        //    await Task.WhenAll(countTask, queryTask);

        //    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        //}

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryObject = JsonConvert.DeserializeObject<DuyetKetQuaXetNghiemSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryObject.FromDate) || !string.IsNullOrEmpty(queryObject.ToDate))
                {
                    DateTime denNgay;
                    queryObject.FromDate.TryParseExactCustom(out fromDate);

                    if (string.IsNullOrEmpty(queryObject.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryObject.ToDate.TryParseExactCustom(out denNgay);
                    }
                    toDate = denNgay.AddSeconds(59).AddMilliseconds(999);
                    //query = query.Where(p => p.NgayThucHien >= tuNgay && p.NgayThucHien <= denNgay);
                }
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.StartDate != null)
                //{
                //    fromDate = queryObject.RangeDuyet.StartDate.GetValueOrDefault().Date;
                //    //query = query.Where(p => tuNgay <= p.NgayDuyetKq);
                //}
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.EndDate != null)
                //{
                //    toDate = queryObject.RangeDuyet.EndDate.GetValueOrDefault().Date;
                //    //query = query.Where(p => denNgay >= p.NgayDuyetKq);
                //}
            }
            var query = BaseRepository.TableNoTracking
                            .Where(p => fromDate <= p.ThoiDiemBatDau && toDate >= p.ThoiDiemBatDau)
                            .Select(s => new DuyetKetQuaXetNghiemGridVo
                            {
                                Id = s.Id,
                                YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                Barcode = s.BarCodeId,
                                BarcodeNumber = s.BarCodeNumber + "",
                                MaTn = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                MaBn = s.BenhNhan.MaBN,
                                HoTen = s.BenhNhan.HoTen,
                                ChoKetQua = s.ChoKetQua, //ChoKetQua = s.PhienXetNghiemChiTiets.All(z => z.ThoiDiemKetLuan == null && z.KetQuaXetNghiemChiTiets.All(kq => string.IsNullOrEmpty(kq.GiaTriTuMay) && string.IsNullOrEmpty(kq.GiaTriNhapTay))),
                                DaCoKetQua = s.ThoiDiemKetLuan != null,
                                NgayThucHien = s.ThoiDiemBatDau,
                            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryObject = JsonConvert.DeserializeObject<DuyetKetQuaXetNghiemSearch>(queryInfo.AdditionalSearchString);

                //if (!string.IsNullOrEmpty(queryObject.FromDate) || !string.IsNullOrEmpty(queryObject.ToDate))
                //{
                //    DateTime denNgay;
                //    queryObject.FromDate.TryParseExactCustom(out var tuNgay);

                //    if (string.IsNullOrEmpty(queryObject.ToDate))
                //    {
                //        denNgay = DateTime.Now;
                //    }
                //    else
                //    {
                //        queryObject.ToDate.TryParseExactCustom(out denNgay);
                //    }
                //    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                //    query = query.Where(p => p.NgayThucHien >= tuNgay && p.NgayThucHien <= denNgay);
                //}
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.StartDate != null)
                //{
                //    var tuNgay = queryObject.RangeDuyet.StartDate.GetValueOrDefault().Date;
                //    query = query.Where(p => tuNgay <= p.NgayDuyetKq);
                //}
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.EndDate != null)
                //{
                //    var denNgay = queryObject.RangeDuyet.EndDate.GetValueOrDefault().Date;
                //    query = query.Where(p => denNgay >= p.NgayDuyetKq);
                //}


                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.HoTen,
                         g => g.Barcode,
                         g => g.MaTn,
                         g => g.MaBn
                   );
                }

                if (queryObject.DangThucHien == true && queryObject.ChoDuyet == false && queryObject.DaDuyet == false)
                {
                    query = query.Where(p => p.ChoKetQua == true);
                }
                else if (queryObject.DangThucHien == true && queryObject.ChoDuyet == true && queryObject.DaDuyet == false)
                {
                    query = query.Where(p => p.DaCoKetQua == false);
                }
                else if (queryObject.DangThucHien == true && queryObject.ChoDuyet == false && queryObject.DaDuyet == true)
                {
                    query = query.Where(p => p.ChoKetQua == true || p.DaCoKetQua == true);
                }
                else if (queryObject.DangThucHien == false && queryObject.ChoDuyet == true && queryObject.DaDuyet == false)
                {
                    query = query.Where(p => p.ChoKetQua == false && p.DaCoKetQua == false);
                }
                else if (queryObject.DangThucHien == false && queryObject.ChoDuyet == true && queryObject.DaDuyet == true)
                {
                    query = query.Where(p => p.ChoKetQua == false);
                }
                else if (queryObject.DangThucHien == false && queryObject.ChoDuyet == false && queryObject.DaDuyet == true)
                {
                    query = query.Where(p => p.DaCoKetQua == true);
                }
            }

            //var countTask = query.CountAsync();

            return new GridDataSource { TotalRowCount = query.Count() };
        }

        public async Task<GridDataSource> GetDataChildrenAsync(QueryInfo queryInfo, long? phienXetNghiemId = null, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par;
            if (phienXetNghiemId != null && phienXetNghiemId != 0)
            {
                par = (long)phienXetNghiemId;
            }
            else
            {
                par = long.Parse(queryInfo.AdditionalSearchString);
            }

            var query = _phienXetNghiemChiTietRepository.TableNoTracking
               .Where(p => p.KetQuaXetNghiemChiTiets.Any() && p.PhienXetNghiemId == par)
               .Select(s => new DuyetKetQuaXetNghiemDetailGridVo
               {
                   Id = s.Id,
                   ThoiGianChiDinh = s.YeuCauDichVuKyThuat.ThoiDiemChiDinh,
                   NguoiChiDinh = s.YeuCauDichVuKyThuat.NhanVienChiDinh.User.HoTen,
                   BenhPham = s.YeuCauDichVuKyThuat.BenhPhamXetNghiem,
                   LoaiMau = s.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription(),
                   MaDv = s.DichVuKyThuatBenhVien.Ma,
                   TenDv = s.DichVuKyThuatBenhVien.Ten,
                   YeuCauChayLai = s.ChayLaiKetQua,
                   DaDuyet = s.YeuCauChayLaiXetNghiem != null ? s.YeuCauChayLaiXetNghiem.DuocDuyet : false,
                   NhomXetNghiemDisplay = s.NhomDichVuBenhVien.Ten,
                   NhomXetNghiemId = s.NhomDichVuBenhVienId
               });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            foreach (var item in queryTask.Result)
            {
                var lstLoaiMau = queryTask.Result.Where(p => p.NhomXetNghiemId == item.NhomXetNghiemId).Select(p => p.LoaiMau).Distinct().ToList();
                item.DanhSachLoaiMau = lstLoaiMau;

                var lstLoaiMauTongCong = BaseRepository.TableNoTracking
                    .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                    .FirstOrDefault(p => p.Id == par)?.YeuCauTiepNhan.YeuCauDichVuKyThuats
                    .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.NhomDichVuBenhVienId == item.NhomXetNghiemId)
                    .Select(p => p.DichVuKyThuatBenhVien).Select(p => p.LoaiMauXetNghiem.GetDescription()).Distinct()
                    .Where(p => p != null).ToList();

                var lstLoaiMauKhongDat = new List<string>();

                if (lstLoaiMauTongCong != null)
                {
                    foreach (var loaiMau in lstLoaiMauTongCong)
                    {
                        var mauXetNghiem = (BaseRepository.TableNoTracking
                                                .Include(p => p.MauXetNghiems)
                                                .FirstOrDefault(p => p.Id == par)?.MauXetNghiems ??
                                            throw new InvalidOperationException())
                            .LastOrDefault(p =>
                                p.LoaiMauXetNghiem.GetDescription() == loaiMau &&
                                p.NhomDichVuBenhVienId == item.NhomXetNghiemId);
                        if (mauXetNghiem != null && mauXetNghiem.DatChatLuong != true)
                        {
                            lstLoaiMauKhongDat.Add(mauXetNghiem.LoaiMauXetNghiem.GetDescription());
                        }
                    }

                    item.DanhSachLoaiMauTongCong = lstLoaiMauTongCong;
                }

                item.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageChildrenAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var par = long.Parse(queryInfo.AdditionalSearchString);

            var query = _phienXetNghiemChiTietRepository.TableNoTracking
                .Where(p => p.KetQuaXetNghiemChiTiets.Any() && p.PhienXetNghiemId == par)
                .Select(s => new DuyetKetQuaXetNghiemDetailGridVo
                {
                    Id = s.Id,
                    ThoiGianChiDinh = s.YeuCauDichVuKyThuat.ThoiDiemChiDinh,
                    NguoiChiDinh = s.YeuCauDichVuKyThuat.NhanVienChiDinh.User.HoTen,
                    BenhPham = s.YeuCauDichVuKyThuat.BenhPhamXetNghiem,
                    LoaiMau = s.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription(),
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        //private IQueryable<DuyetKetQuaXetNghiemGridVo> GetData(bool choDuyetChayLai, bool choDuyet, bool daDuyet, QueryInfo queryInfo, DuyetKetQuaXetNghiemSearch queryObject)
        //{
        //    var result = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuyetKetQuaXetNghiemGridVo()).AsQueryable();

        //    if (choDuyetChayLai)
        //    {
        //        var query = BaseRepository.TableNoTracking
        //            .Where(p => p.PhienXetNghiemChiTiets.Any(o => o.KetQuaXetNghiemChiTiets.Any())
        //                        && p.PhienXetNghiemChiTiets.Any(o => o.ThoiDiemKetLuan == null && o.DaGoiDuyet == true)
        //            );

        //        result = query
        //       .Select(s => new DuyetKetQuaXetNghiemGridVo
        //       {
        //           Id = s.Id,
        //           YeuCauTiepNhanId = s.YeuCauTiepNhanId,
        //           Barcode = s.BarCodeId,
        //           BarcodeNumber = s.BarCodeNumber + "",
        //           MaTn = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
        //           MaBn = s.BenhNhan.MaBN,
        //           HoTen = s.BenhNhan.HoTen,
        //           GioiTinh = s.BenhNhan.GioiTinh,
        //           NamSinh = s.BenhNhan.NamSinh != null ? s.BenhNhan.NamSinh.ToString() : string.Empty,
        //           DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
        //           NguoiThucHien = s.NhanVienThucHien.User.HoTen,
        //           NgayThucHien = s.ThoiDiemBatDau,
        //           NguoiDuyetKq = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : "",
        //           NgayDuyetKq = s.ThoiDiemKetLuan,
        //           TrangThai = s.PhienXetNghiemChiTiets.Any(o => o.ChayLaiKetQua == true),
        //       }).Where(w => w.TrangThai == true);

        //        result = result.ApplyLike(queryInfo.SearchTerms, g => g.MaTn, g => g.MaBn, g => g.HoTen, g => g.BarcodeNumber, g => g.Barcode, g => g.DiaChi, g => g.NguoiThucHien, g => g.NguoiDuyetKq, g => g.NamSinh);
        //    }
        //    else if (choDuyet)
        //    {
        //        var query = BaseRepository.TableNoTracking
        //            .Where(p => p.PhienXetNghiemChiTiets.Any(o => o.KetQuaXetNghiemChiTiets.Any())
        //                        && p.PhienXetNghiemChiTiets.Any(o => o.DaGoiDuyet == true)
        //                        && !p.PhienXetNghiemChiTiets.All(o => o.ThoiDiemKetLuan != null)
        //            );

        //        result = query
        //            .Select(s => new DuyetKetQuaXetNghiemGridVo
        //            {
        //                Id = s.Id,
        //                YeuCauTiepNhanId = s.YeuCauTiepNhanId,
        //                Barcode = s.BarCodeId,
        //                BarcodeNumber = s.BarCodeNumber + "",
        //                MaTn = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
        //                MaBn = s.BenhNhan.MaBN,
        //                HoTen = s.BenhNhan.HoTen,
        //                GioiTinh = s.BenhNhan.GioiTinh,
        //                NamSinh = s.BenhNhan.NamSinh != null ? s.BenhNhan.NamSinh.ToString() : string.Empty,
        //                DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
        //                NguoiThucHien = s.NhanVienThucHien.User.HoTen,
        //                NgayThucHien = s.ThoiDiemBatDau,
        //                NguoiDuyetKq = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : "",
        //                NgayDuyetKq = s.ThoiDiemKetLuan,
        //                TrangThai = s.PhienXetNghiemChiTiets.Any(o => o.ChayLaiKetQua == true),
        //            }).Where(w => w.TrangThai == false);

        //        result = result.ApplyLike(queryInfo.SearchTerms, g => g.MaTn, g => g.MaBn, g => g.HoTen, g => g.BarcodeNumber, g => g.Barcode, g => g.DiaChi, g => g.NguoiThucHien, g => g.NguoiDuyetKq, g => g.NamSinh);
        //    }
        //    else if (daDuyet)
        //    {
        //        var query = BaseRepository.TableNoTracking
        //            .Where(p => p.PhienXetNghiemChiTiets.Any(o => o.KetQuaXetNghiemChiTiets.Any())
        //                        && p.PhienXetNghiemChiTiets.All(o => o.ThoiDiemKetLuan != null)
        //            );

        //        result = query
        //            .Select(s => new DuyetKetQuaXetNghiemGridVo
        //            {
        //                Id = s.Id,
        //                YeuCauTiepNhanId = s.YeuCauTiepNhanId,
        //                Barcode = s.BarCodeId,
        //                BarcodeNumber = s.BarCodeNumber + "",
        //                MaTn = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
        //                MaBn = s.BenhNhan.MaBN,
        //                HoTen = s.BenhNhan.HoTen,
        //                GioiTinh = s.BenhNhan.GioiTinh,
        //                NamSinh = s.BenhNhan.NamSinh != null ? s.BenhNhan.NamSinh.ToString() : string.Empty,
        //                DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
        //                NguoiThucHien = s.NhanVienThucHien.User.HoTen,
        //                NgayThucHien = s.ThoiDiemBatDau,
        //                NguoiDuyetKq = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : "",
        //                NgayDuyetKq = s.ThoiDiemKetLuan,
        //                TrangThai = null,
        //            });

        //        result = result.ApplyLike(queryInfo.SearchTerms, g => g.MaTn, g => g.MaBn, g => g.HoTen, g => g.BarcodeNumber, g => g.Barcode, g => g.DiaChi, g => g.NguoiThucHien, g => g.NguoiDuyetKq, g => g.NamSinh);
        //    }

        //    if (queryObject != null)
        //    {
        //        if (queryObject.RangeThucHien != null && queryObject.RangeThucHien.StartDate != null)
        //        {
        //            var tuNgay = queryObject.RangeThucHien.StartDate.GetValueOrDefault().Date;

        //            result = result.Where(p => tuNgay <= p.NgayThucHien.Date);
        //        }
        //        if (queryObject.RangeThucHien != null && queryObject.RangeThucHien.EndDate != null)
        //        {
        //            var denNgay = queryObject.RangeThucHien.EndDate.GetValueOrDefault().Date;
        //            result = result.Where(p => denNgay >= p.NgayThucHien.Date);
        //        }
        //        if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.StartDate != null)
        //        {
        //            var tuNgay = queryObject.RangeDuyet.StartDate.GetValueOrDefault().Date;
        //            result = result.Where(p => tuNgay <= p.NgayDuyetKq.Value.Date);
        //        }
        //        if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.EndDate != null)
        //        {
        //            var denNgay = queryObject.RangeDuyet.EndDate.GetValueOrDefault().Date;
        //            result = result.Where(p => denNgay >= p.NgayDuyetKq.Value.Date);
        //        }
        //    }

        //    return result;
        //}

        public List<PhieuInXetNghiemModel> InPhieuXetNghiemTheoYeuCauKyThuatVaNhom(LayPhieuXetNghiemTheoYCKTVaNhomDVBVVo ketQuaXetNghiemPhieuIn)
        {
            var lstContent = new List<PhieuInXetNghiemModel>();
            var numberPage = 1;

            string ngay = string.Empty;
            string thang = string.Empty;
            string nam = string.Empty;
            string gio = string.Empty;

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKetQuaXetNghiemNew"));
            List<long> listDefault = new List<long>();

            #region lấy tất cả phiên xét nghiệm  theo yêu cầu tiếp nhận và NhomDichVuBenhVienId , phieuDieuTriHienTaiId
            var phienXetNghiemChiTiets = BaseRepository.TableNoTracking
                                    .Where(c => c.YeuCauTiepNhanId == ketQuaXetNghiemPhieuIn.YeuCauTiepNhanId)
                                    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
                                    .SelectMany(d => d.PhienXetNghiemChiTiets);

            var phienXetNghiemIds = phienXetNghiemChiTiets.Where(cc => cc.YeuCauDichVuKyThuat.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId &&
                                                                      cc.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == ketQuaXetNghiemPhieuIn.phieuDieuTriHienTaiId)
                                                          .Select(d => d.PhienXetNghiemId).ToList();
            #endregion


            

           
            var phienXetNghiemTheoYckts = BaseRepository.TableNoTracking.Where(c => phienXetNghiemIds.Contains(c.Id))
                                                                       
                                                                        
                                        .Include(p => p.BenhNhan)
                                        .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem)
                                        .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienLayMau).ThenInclude(q => q.User)
                                        .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienNhanMau).ThenInclude(q => q.User)
                                        .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem).ThenInclude(q => q.NhanVienGoiMau).ThenInclude(q => q.User)
                                        .Include(q => q.NhanVienKetLuan).ThenInclude(q => q.User)
                                        .Include(q => q.NhanVienThucHien).ThenInclude(q => q.User)


                                        .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)

                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(d => d.ChanDoanSoBoICD)

                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)

                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.MayXetNghiem)

                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NoiChiDinh)
                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)

                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauTiepNhan)
                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NoiChiDinh)
                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)
                                        .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauKhamBenhs)
                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuKyThuatBenhVien)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri).ThenInclude(d => d.ChanDoanChinhICD)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.User)
                                        .Include(x => x.YeuCauTiepNhan).ThenInclude(d => d.YeuCauKhamBenhs).ThenInclude(d => d.ChanDoanSoBoICD)
                                        .Include(x => x.YeuCauTiepNhan).ThenInclude(d => d.HopDongKhamSucKhoeNhanVien).ThenInclude(d => d.HopDongKhamSucKhoe).ThenInclude(d => d.CongTyKhamSucKhoe)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.User)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.HocHamHocVi)
                                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.HocHamHocVi)
                                        .Include(x => x.NhanVienThucHien).ThenInclude(x => x.HocHamHocVi)
                                        .Include(x => x.NhanVienThucHien).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                                        .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuXetNghiem).ThenInclude(p => p.HDPP)
                                        .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.HopDongKhamSucKhoe).ThenInclude(p => p.CongTyKhamSucKhoe);





            var thongTinPhieuIns = new List<ThongTinBacSiTheoPhienVo>();
            foreach (var item in phienXetNghiemTheoYckts.ToList())
            {
                var thongTinBacSi = item.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                    .Where(cc => cc.YeuCauKhamBenh != null || cc.NoiTruPhieuDieuTriId != null)
                    .OrderBy(cc => cc.ThoiDiemHoanThanh)
                    .Select(q => new ThongTinBacSiTheoPhienVo
                    {
                        BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
                        BacSiChiDinhId = q.NhanVienChiDinhId,
                        KhoaPhongChiDinh = q.NoiChiDinh?.Ten,
                        PhienXetNghiem = item,
                        PhienXetNghiemChiTiets = item.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).ToList(),
                        PhienXetNghiemId = item.Id
                    })
                    .ToList();

                var thongTinLeTan = item.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                    .Where(cc => cc.YeuCauKhamBenh == null && cc.NoiTruPhieuDieuTriId == null)
                    .OrderBy(cc => cc.ThoiDiemHoanThanh)
                    .Select(q => new ThongTinBacSiTheoPhienVo
                    {
                        BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
                        BacSiChiDinhId = q.NhanVienChiDinhId,
                        KhoaPhongChiDinh = q.NoiChiDinh?.Ten,
                        FromLeTan = true,
                        PhienXetNghiem = item,
                        PhienXetNghiemChiTiets = item.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).ToList(),
                        PhienXetNghiemId = item.Id
                    })
                    .ToList();

                thongTinPhieuIns.AddRange(thongTinBacSi);
                thongTinPhieuIns.AddRange(thongTinLeTan);
            }
           

            // BVHD-3897
            var thongTinPhieuInTheoNhanVienChiDinhs = thongTinPhieuIns.GroupBy(d => new { d.BacSiChiDinhId, d.PhienXetNghiemId })
                                                                   .Select(d => new ThongTinBacSiTheoPhienVo
                                                                   {
                                                                       BacSiChiDinh = d.First().BacSiChiDinh,
                                                                       BacSiChiDinhId = d.First().BacSiChiDinhId,
                                                                       KhoaPhongChiDinh = d.First().KhoaPhongChiDinh, // lấy first
                                                                       PhienXetNghiem = d.First().PhienXetNghiem,
                                                                       PhienXetNghiemChiTiets = d.First().PhienXetNghiemChiTiets
                                                                   })
                                                                   .ToList();

            var lstDichVuSarCoVs = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhTiepNhan.DichVuTestSarsCovid")
             .Select(d => d.Value).FirstOrDefault();

            var json = JsonConvert.DeserializeObject<List<DichVuKyThuatBenhVienIdsSarsCoV>>(lstDichVuSarCoVs);
            var listDichVuKyThuatSarsCov2s = json.Select(d => d.DichVuKyThuatBenhVienId).ToList();

            foreach (var thongTin in thongTinPhieuInTheoNhanVienChiDinhs)
            {
                var data = new DuyetKetQuaXetNghiemPhieuInResultVo
                {
                    SoPhieu = thongTin.PhienXetNghiem.MauXetNghiems.LastOrDefault(q => q.BarCodeId == thongTin.PhienXetNghiem.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
                    //MaBHYT = phienXetNghiemData.YeuCauTiepNhan.BHYTMaSoThe,
                    SoVaoVien = thongTin.PhienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    LogoUrl = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha-full.png",
                    MaYTe = thongTin.PhienXetNghiem.BenhNhan.MaBN,
                   
                    HoTen = thongTin.PhienXetNghiem.YeuCauTiepNhan.HoTen,
                    DiaChi = thongTin.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? thongTin.PhienXetNghiem.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoe?.CongTyKhamSucKhoe?.Ten : thongTin.PhienXetNghiem.YeuCauTiepNhan.DiaChiDayDu,
                    GhiChuDV = thongTin.PhienXetNghiem.GhiChu,
                    NamSinh = thongTin.PhienXetNghiem.YeuCauTiepNhan.NamSinh,
                    GioiTinh = thongTin.PhienXetNghiem.YeuCauTiepNhan.GioiTinh != null ? thongTin.PhienXetNghiem.YeuCauTiepNhan.GioiTinh : null,
                    NamSinhString = DateHelper.DOBFormat(thongTin.PhienXetNghiem.YeuCauTiepNhan.NgaySinh, thongTin.PhienXetNghiem.YeuCauTiepNhan.ThangSinh, thongTin.PhienXetNghiem.YeuCauTiepNhan.NamSinh),

                    DoiTuong = thongTin.PhienXetNghiem.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT " + (thongTin.PhienXetNghiem.YeuCauTiepNhan.BHYTMucHuong != null ? "(" + thongTin.PhienXetNghiem.YeuCauTiepNhan.BHYTMucHuong + "%)" : ""),

                    MaBHYT = thongTin.PhienXetNghiem.YeuCauTiepNhan?.BHYTMaSoThe,

                    BsChiDinh = returnStringTenTheoNhanVien(thongTin.BacSiChiDinhId),
                    KhoaPhong = thongTin.KhoaPhongChiDinh,
                    Barcode = thongTin.PhienXetNghiem.BarCodeId.Length == 10 ? thongTin.PhienXetNghiem.BarCodeId.Substring(0, 6) + "-" + thongTin.PhienXetNghiem.BarCodeId.Substring(6) : thongTin.PhienXetNghiem.BarCodeId
                };

                if (thongTin.PhienXetNghiem.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null)
                {
                    data.STT = thongTin.PhienXetNghiem.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{thongTin.PhienXetNghiem.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
                }
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(thongTin.PhienXetNghiem.Id.ToString()) ? BarcodeHelper.GenerateBarCode(thongTin.PhienXetNghiem.Id.ToString()) : "";
                var lstYeuCauDichVuKyThuatIdTrongPhieu = new List<long>();

                var tenNhom = thongTin.PhienXetNghiemChiTiets.Where(c => c.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId
                                                                      && c.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId)
                                                           .Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).FirstOrDefault();
                
                var info = string.Empty;
                if(tenNhom != null)
                {
                    var STT = 1;
                    var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b>" +
                                       tenNhom.ToUpper()
                                       + "</b></tr>";



                    var listKqXN = new List<string>();
                    var listGhiChuTheoDVXNISOs = new List<KetQuaXetNghiemChiTiet>();
                    var listGhiChuTheoDVChuyenGois = new List<KetQuaXetNghiemChiTiet>();


                    foreach (var phienXetNghiemTheoYckt in phienXetNghiemTheoYckts)
                    {
                        var queryNhom = new List<KetQuaXetNghiemChiTiet>();
                        List<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiets = phienXetNghiemTheoYckt.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).ToList();
                        if (ketQuaXetNghiemChiTiets.Any())
                        {
                            var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom
                                                                                   && p.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId)
                                                                                  .Select(p => p.PhienXetNghiemChiTiet)
                                                                                  .Select(p => p.YeuCauDichVuKyThuatId)
                                                                                  .Distinct().ToList();

                            foreach (var ycId in lstYeuCauDichVuKyThuatId.Where(d => !listDichVuKyThuatSarsCov2s.Contains(d)).ToList())
                            {
                                if (!phienXetNghiemTheoYckt.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                                var res = phienXetNghiemTheoYckt.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                                queryNhom.AddRange(res);
                            }

                            lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);


                            // BVHD-3901 
                            //ghiChu = string.Empty;

                            if (queryNhom.Where(p => p.DichVuXetNghiemChaId == null).ToList().Count() != 0)
                            {
                                var dvIns = queryNhom.Where(p => p.DichVuXetNghiemChaId == null).ToList();
                                var chiSoXNISO = dvIns.Where(d => d.DichVuXetNghiem?.LaChuanISO == true).ToList().Count() != 0 ? true : false;

                                if (chiSoXNISO == true)
                                {
                                    listGhiChuTheoDVXNISOs.AddRange(dvIns.Where(d => d.DichVuXetNghiem?.LaChuanISO == true).ToList());
                                }

                                var chiSoXNChuyenGui = dvIns.Where(d => d.DichVuKyThuatBenhVien?.DichVuChuyenGoi == true).ToList().Count() != 0 ? true : false;

                                if (chiSoXNChuyenGui == true)
                                {
                                    listGhiChuTheoDVChuyenGois.AddRange(dvIns.Where(d => d.DichVuKyThuatBenhVien?.DichVuChuyenGoi == true).ToList());
                                }

                            }


                            foreach (var dataParent in queryNhom.Where(p => p.DichVuXetNghiemChaId == null).OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                            {
                                info = info + GetPhieuIn("", STT,
                                           queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId)
                                               .ToList(), dataParent);
                            }
                        }
                    }

                    data.DanhSach = headerNhom + info;
                    data.NguoiThucHien = returnStringTen(thongTin.PhienXetNghiem.NhanVienThucHien?.HocHamHocVi?.Ma, thongTin.PhienXetNghiem.NhanVienThucHien?.ChucDanh?.NhomChucDanh?.Ma, thongTin.PhienXetNghiem.NhanVienThucHien?.User?.HoTen);

                    //var chanDoans = phienXetNghiemData.PhienXetNghiemChiTiets
                    //    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                    //    .Select(o => o.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "Khám sức khỏe" :
                    //        (o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null
                    //            ? o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu
                    //            : o.YeuCauDichVuKyThuat.YeuCauKhamBenh?.ChanDoanSoBoGhiChu)).Distinct();
                    // lấy chẩn icd mới nhất của bác sĩ .. cái cuối cùng

                    data.ChanDoan = thongTin.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                            (thongTin.PhienXetNghiem.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                                ? thongTin.PhienXetNghiem.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                                    .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Where(p => p.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh.IcdchinhId != null)
                                    .Select(p => (p.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + p.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet)).Where(p => p != null && p != "-" && p != "").Distinct().LastOrDefault()
                                : "") :
                                (thongTin.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                                thongTin.PhienXetNghiem.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => (o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet))
                                                                         .Where(p => p != null && p != "-" && p != "").LastOrDefault() :
                                (thongTin.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : ""));// ksk de trong



                    data.DienGiai = thongTin.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                                 (thongTin.PhienXetNghiem.YeuCauTiepNhan.YeuCauKhamBenhs.SelectMany(d => d.YeuCauDichVuKyThuats)
                                    .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Any(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                                   ? thongTin.PhienXetNghiem.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                                       .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id))
                                       .Where(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                                       .Select(p => p.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).ToList().Distinct().LastOrDefault()
                                   : "") :
                                   (thongTin.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                                   thongTin.PhienXetNghiem.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).ToList().Distinct().LastOrDefault() :
                                   (thongTin.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : "")); // ksk de trong

                    var enumLoaiMauXetNghiems = thongTin.PhienXetNghiem.PhienXetNghiemChiTiets
                        .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                        .Select(o => o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem).Distinct();

                    //data.ChanDoan = string.Join("; ", chanDoans.Where(o => !string.IsNullOrEmpty(o)));
                    data.LoaiMau = string.Join("; ", enumLoaiMauXetNghiems.Select(o => o.GetDescription()));

                    var phienXetNghiemChiTietLast = thongTin.PhienXetNghiem.PhienXetNghiemChiTiets
                        .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();
                    if (phienXetNghiemChiTietLast != null)
                    {
                        data.TgLayMau = phienXetNghiemChiTietLast.ThoiDiemLayMau?.ApplyFormatDateTimeSACH();

                        data.NguoiLayMau = returnStringTen(phienXetNghiemChiTietLast.NhanVienLayMau?.HocHamHocVi?.Ma, phienXetNghiemChiTietLast.NhanVienLayMau?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemChiTietLast.NhanVienLayMau?.User?.HoTen);

                        data.TgNhanMau = phienXetNghiemChiTietLast.ThoiDiemNhanMau?.ApplyFormatDateTimeSACH();

                        data.NguoiNhanMau = returnStringTen(phienXetNghiemChiTietLast.NhanVienNhanMau?.HocHamHocVi?.Ma, phienXetNghiemChiTietLast.NhanVienNhanMau?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemChiTietLast.NhanVienNhanMau?.User?.HoTen);
                    }

                    if (phienXetNghiemChiTietLast != null)
                    {
                        data.Ngay = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                        ngay = data.Ngay;
                        data.Thang = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                        thang = data.Thang;
                        data.Nam = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                        nam = data.Nam;
                        data.Gio = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";
                        gio = data.Gio;
                    }

                    data.NgayThangNamFooter = DateTime.Now.ApplyFormatDateTimeSACH();
                    var lstContentItem = new PhieuInXetNghiemModel();
                    // 
                    if (!string.IsNullOrEmpty(data.DanhSach))
                    {
                        if (listGhiChuTheoDVXNISOs.Count() != 0)
                        {
                            data.LogoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                            data.LogoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";
                        }

                        lstContentItem.Html = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, data);

                        var templatefootertableXetNghiem = _templateRepository.TableNoTracking.First(x => x.Name.Equals("FooterXetNghiemTable"));

                        data.Ngay = data.Ngay == null ? ngay : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                        data.Thang = data.Thang == null ? thang : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                        data.Nam = data.Nam == null ? nam : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                        data.Gio = data.Gio == null ? gio : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";

                        //Phần ghi chú sắp xếp như sau:
                        //Dòng đầu tiên là dòng ghi chú mặc định (footer template)
                        //Dòng thứ 2 là ghi chú ISO (ghiChu) 
                        //Dòng thứ 3 là ghi chú Dịch vụ gửi ngoài (ghiChu) 
                        //Dòng thứ 4 là dòng ghi chú của dịch vụ  (Ghi chú DV)


                        var ghiChu = string.Empty;
                        var logoBV1 = string.Empty;
                        var logoBV2 = string.Empty;



                        if (listGhiChuTheoDVXNISOs.Count() != 0)
                        {

                            if (!string.IsNullOrEmpty(ghiChu))
                            {
                                ghiChu += "<br>";
                            }
                        //update 16/05: - Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012 THAY ĐỔI THÀNH - Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012
                        //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";
                        ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";

                            logoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                            logoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";

                            data.LogoBV1 = logoBV1;
                            data.LogoBV2 = logoBV2;
                        }

                        if (listGhiChuTheoDVChuyenGois.Count() != 0)
                        {
                            if (!string.IsNullOrEmpty(ghiChu))
                            {
                                ghiChu += "<br>";
                            }
                        //update 16/05: - Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi  THAY ĐỔI THÀNH  - Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.
                        //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi.</span>";
                        ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.</span>";
                        }




                        data.GhiChu = (!string.IsNullOrEmpty(ghiChu) ? ghiChu + "<br>" : "") + (!string.IsNullOrEmpty(data.GhiChuDV) ? "- " + data.GhiChuDV : "");

                        var htmlfooter = TemplateHelpper.FormatTemplateWithContentTemplate(templatefootertableXetNghiem.Body, data);
                        lstContentItem.Html += htmlfooter;
                        lstContentItem.Html += "<div style='margin-left: 2cm;'><table width='100%'><tr><td style='width:36%'></td><td style='width:35%'></td><td style='width:29%'><b>" + data.NguoiThucHien + "</b></td>";
                        lstContentItem.Html += "</tr></table></div>";
                        lstContent.Add(lstContentItem);
                    }

                    numberPage++;
                }
                
            }

            return lstContent;
        }


        public async Task<List<PhieuInXetNghiemModel>> InDuyetKetQuaXetNghiem(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn)
        {
            var lstContent = new List<PhieuInXetNghiemModel>();

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuInKetQuaXetNghiem"));
            List<long> listDefault = new List<long>();
            var phienXetNghiemData = BaseRepository.TableNoTracking
                .Include(p => p.BenhNhan)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienLayMau).ThenInclude(q => q.User)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienNhanMau).ThenInclude(q => q.User)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem).ThenInclude(q => q.NhanVienGoiMau).ThenInclude(q => q.User)
                .Include(q => q.NhanVienKetLuan).ThenInclude(q => q.User)
                .Include(q => q.NhanVienThucHien).ThenInclude(q => q.User)

                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.TinhThanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)

                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.MayXetNghiem)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NoiChiDinh)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauTiepNhan)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NoiChiDinh)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)

                 .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)

                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri).ThenInclude(d => d.ChanDoanChinhICD)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.User)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.YeuCauTiepNhanTheBHYTs)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.HopDongKhamSucKhoe).ThenInclude(p => p.CongTyKhamSucKhoe)
                .FirstOrDefault(q => q.Id == ketQuaXetNghiemPhieuIn.Id);

            var phienXetNghiemChiTietEntities = phienXetNghiemData.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).ToList();

            var thongTinBacSi = phienXetNghiemData.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                .Where(cc => cc.YeuCauKhamBenh != null || cc.NoiTruPhieuDieuTriId != null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    //YeuCauKhamBenhId = q.Id,
                    BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinh?.Ten
                }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinLeTan = phienXetNghiemData.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                .Where(cc => cc.YeuCauKhamBenh == null && cc.NoiTruPhieuDieuTriId == null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    //YeuCauKhamBenhId = q.Id,
                    BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinh?.Ten,
                    FromLeTan = true,
                }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinPhieuIns = thongTinBacSi;
            thongTinPhieuIns.AddRange(thongTinLeTan);

            var thongTinPhieuInTheoNhanVienChiDinhs = thongTinPhieuIns.GroupBy(d => d.BacSiChiDinhId)
                                                                     .Select(d => new ThongTinBacSiVo
                                                                     {
                                                                         BacSiChiDinh = d.First().BacSiChiDinh,
                                                                         BacSiChiDinhId = d.First().BacSiChiDinhId,
                                                                         KhoaPhongChiDinh = d.First().KhoaPhongChiDinh // lấy first
                                                                      })
                                                                     .ToList();

            foreach (var thongTin in thongTinPhieuInTheoNhanVienChiDinhs)
            {
                var data = new DuyetKetQuaXetNghiemPhieuInResultVo
                {
                    SoPhieu = phienXetNghiemData.MauXetNghiems.LastOrDefault(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
                    //MaBHYT = phienXetNghiemData.YeuCauTiepNhan.BHYTMaSoThe,
                    SoVaoVien = phienXetNghiemData.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    LogoUrl = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha-full.png",
                    MaYTe = phienXetNghiemData.BenhNhan.MaBN,
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString(),
                    HoTen = phienXetNghiemData.YeuCauTiepNhan.HoTen,
                    DiaChi = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? phienXetNghiemData.YeuCauTiepNhan?.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoe?.CongTyKhamSucKhoe?.Ten : phienXetNghiemData.YeuCauTiepNhan.DiaChiDayDu,
                    GhiChu = phienXetNghiemData.GhiChu,
                    NamSinh = phienXetNghiemData.YeuCauTiepNhan.NamSinh,
                    NamSinhString = DateHelper.DOBFormat(phienXetNghiemData.YeuCauTiepNhan.NgaySinh, phienXetNghiemData.YeuCauTiepNhan.ThangSinh, phienXetNghiemData.YeuCauTiepNhan.NamSinh),
                    GioiTinh = phienXetNghiemData.YeuCauTiepNhan.GioiTinh != null ? phienXetNghiemData.YeuCauTiepNhan.GioiTinh : null,
                    Gio = DateTime.Now.Hour + " giờ " + DateTime.Now.Minute + " phút",
                    DoiTuong = phienXetNghiemData.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT " + (phienXetNghiemData.YeuCauTiepNhan.BHYTMucHuong != null ? "(" + phienXetNghiemData.YeuCauTiepNhan.BHYTMucHuong + "%)" : ""),

                    MaBHYT = phienXetNghiemData.YeuCauTiepNhan?.BHYTMaSoThe,
                    BsChiDinh = returnStringTenTheoNhanVien(thongTin.BacSiChiDinhId),
                    KhoaPhong = thongTin.KhoaPhongChiDinh,
                    Barcode = phienXetNghiemData.BarCodeId
                };

                if (phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null)
                {
                    data.STT = phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
                }
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(ketQuaXetNghiemPhieuIn.Id.ToString())
                    ? BarcodeHelper.GenerateBarCode(ketQuaXetNghiemPhieuIn.Id.ToString())
                    : "";

                List<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiets = phienXetNghiemChiTietEntities
                    .SelectMany(p => p.KetQuaXetNghiemChiTiets)
                    .Where(p => p.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId).ToList();

                var lstYeuCauDichVuKyThuatIdTrongPhieu = new List<long>();
                if (ketQuaXetNghiemChiTiets.Any())
                {
                    if (ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds.Any())
                    {
                        var info = string.Empty;
                        foreach (var nhomDichVuBvId in ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds)
                        {
                            var totalTenNhom = ketQuaXetNghiemChiTiets
                                .Where(p => p.NhomDichVuBenhVienId == nhomDichVuBvId)
                                .Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();
                            var STT = 1;
                            foreach (var tenNhom in totalTenNhom)
                            {
                                var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                 + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b>" +
                                                 tenNhom.ToUpper()
                                                 + "</b></tr>";
                                info += headerNhom;
                                //var queryNhom = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom).ToList();

                                var queryNhom = new List<KetQuaXetNghiemChiTiet>();

                                var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets
                                    .Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                                    .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId)
                                    .Distinct().ToList();

                                foreach (var ycId in lstYeuCauDichVuKyThuatId)
                                {
                                    if (!phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                                    var res = phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                                    queryNhom.AddRange(res);
                                }
                                lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);
                                //var theFist = false;
                                foreach (var dataParent in queryNhom.Where(p => p.DichVuXetNghiemChaId == null)
                                    .OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                                {
                                    info = info + GetPhieuIn("", STT,
                                               queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId)
                                                   .ToList(), dataParent);
                                }
                            }
                        }
                        data.DanhSach = info;

                    }
                    else
                    {
                        List<string> totalTenNhom = new List<string>();
                        if (ketQuaXetNghiemPhieuIn.LoaiIn == 1) // in tất cả
                        {
                            totalTenNhom = ketQuaXetNghiemChiTiets.Select(p => p.NhomDichVuBenhVien.Ten).Distinct()
                                .OrderBy(o => o).ToList();
                        }
                        else if (ketQuaXetNghiemPhieuIn.LoaiIn == 2) // in theo nhóm
                        {
                            foreach (var item in ketQuaXetNghiemPhieuIn.ListIn)
                            {
                                var queryItemNhom = ketQuaXetNghiemChiTiets
                                    .Where(s => s.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                                    .Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();
                                totalTenNhom.AddRange(queryItemNhom);
                            }
                        }
                        else if (ketQuaXetNghiemPhieuIn.LoaiIn == 3) // in theo dịch vụ
                        {
                            foreach (var item in ketQuaXetNghiemPhieuIn.ListIn)
                            {
                                var queryItemNhom = ketQuaXetNghiemChiTiets
                                    .Where(s => s.Id == item.Id &&
                                                s.YeuCauDichVuKyThuatId == item.YeuCauDichVuKyThuatId &&
                                                !totalTenNhom.Contains(s.NhomDichVuBenhVien.Ten))
                                    .Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();
                                totalTenNhom.AddRange(queryItemNhom);
                            }
                        }
                        //var totalTenNhom = ketQuaXetNghiemChiTiets.Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();

                        var info = string.Empty;
                        var STT = 1;
                        foreach (var tenNhom in totalTenNhom.Distinct())
                        {
                            var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                             + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b>" +
                                             tenNhom.ToUpper()
                                             + "</b></tr>";
                            info += headerNhom;
                            //var queryNhom = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom).ToList();

                            var queryNhom = new List<KetQuaXetNghiemChiTiet>();
                            if (ketQuaXetNghiemPhieuIn.LoaiIn == 1 || ketQuaXetNghiemPhieuIn.LoaiIn == 2)
                            {
                                var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets
                                    .Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                                    .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId)
                                    .Distinct().ToList();
                                foreach (var ycId in lstYeuCauDichVuKyThuatId)
                                {
                                    if (!phienXetNghiemData.PhienXetNghiemChiTiets
                                        .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                        .Any()) continue;
                                    var res = phienXetNghiemData.PhienXetNghiemChiTiets
                                        .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                        .ToList();
                                    queryNhom.AddRange(res);
                                }
                                lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);
                            }
                            else
                            {

                                var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets
                                    .Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                                    .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId)
                                    .Distinct().ToList();
                                List<long> listId = new List<long>();
                                foreach (var item in lstYeuCauDichVuKyThuatId)
                                {
                                    foreach (var itemIn in ketQuaXetNghiemPhieuIn.ListIn)
                                    {
                                        if (item == itemIn.YeuCauDichVuKyThuatId)
                                        {
                                            var kiemTraDaCoChua = listDefault.Where(x => x == item);
                                            if (!kiemTraDaCoChua.Any())
                                            {
                                                listId.Add(item);
                                            }
                                        }
                                    }
                                }
                                foreach (var ycId in listId)
                                {
                                    if (!phienXetNghiemData.PhienXetNghiemChiTiets
                                        .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                        .Any()) continue;
                                    var res = phienXetNghiemData.PhienXetNghiemChiTiets
                                        .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                        .ToList();
                                    queryNhom.AddRange(res);
                                }
                                lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(listId);
                            }


                            //var theFist = false;
                            foreach (var dataParent in queryNhom.Where(p => p.DichVuXetNghiemChaId == null)
                                .OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                            {
                                info = info + GetPhieuIn("", STT,
                                           queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId)
                                               .ToList(), dataParent);
                            }
                        }
                        data.DanhSach = info;
                    }
                }

                data.NguoiThucHien = phienXetNghiemData.NhanVienThucHien?.User?.HoTen;

                //var chanDoans = phienXetNghiemData.PhienXetNghiemChiTiets
                //    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                //    .Select(o => o.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "Khám sức khỏe" :
                //        (o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null
                //            ? o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu
                //            : o.YeuCauDichVuKyThuat.YeuCauKhamBenh?.ChanDoanSoBoGhiChu)).Distinct();
                // lấy chẩn icd mới nhất của bác sĩ .. cái cuối cùng

                data.ChanDoan = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                       (phienXetNghiemData.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                           ? phienXetNghiemData.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                               .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Where(p => p.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh.IcdchinhId != null)
                               .Select(p => (p.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + p.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet)).Where(p => p != null && p != "-" && p != "").Distinct().LastOrDefault()
                           : "") :
                           (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                           phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => (o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet))
                                                                    .Where(p => p != null && p != "-" && p != "").LastOrDefault() :
                           (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : ""));// ksk de trong



                data.DienGiai = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                             (phienXetNghiemData.YeuCauTiepNhan.YeuCauKhamBenhs.SelectMany(d => d.YeuCauDichVuKyThuats)
                                .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Any(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                               ? phienXetNghiemData.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                                   .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id))
                                   .Where(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                                   .Select(p => p.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).ToList().Distinct().LastOrDefault()
                               : "") :
                               (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                               phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).ToList().Distinct().LastOrDefault() :
                               (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : "")); // ksk de trong

                var enumLoaiMauXetNghiems = phienXetNghiemData.PhienXetNghiemChiTiets
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                    .Select(o => o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem).Distinct();

                //data.ChanDoan = string.Join("; ", chanDoans.Where(o => !string.IsNullOrEmpty(o)));
                data.LoaiMau = string.Join("; ", enumLoaiMauXetNghiems.Select(o => o.GetDescription()));

                var phienXetNghiemChiTietLast = phienXetNghiemData.PhienXetNghiemChiTiets
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();
                if (phienXetNghiemChiTietLast != null)
                {
                    data.TgLayMau = phienXetNghiemChiTietLast.ThoiDiemLayMau?.ApplyFormatDateTimeSACH();

                    data.NguoiLayMau = phienXetNghiemChiTietLast.NhanVienLayMau?.User.HoTen;

                    data.TgNhanMau = phienXetNghiemChiTietLast.ThoiDiemNhanMau?.ApplyFormatDateTimeSACH();

                    data.NguoiNhanMau = phienXetNghiemChiTietLast.NhanVienNhanMau?.User.HoTen;
                }

                var lstContentItem = new PhieuInXetNghiemModel();
                lstContentItem.Html = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, data);
                lstContent.Add(lstContentItem);
            }

            return lstContent;
        }

        public async Task<List<PhieuInXetNghiemModel>> InKetQuaXetNghiem(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn)
        {
            var lstContent = new List<PhieuInXetNghiemModel>();

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuInKetQuaXetNghiem"));

            var phienXetNghiemData = await BaseRepository.TableNoTracking
                .Include(p => p.BenhNhan)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienLayMau).ThenInclude(q => q.User)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienNhanMau).ThenInclude(q => q.User)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem).ThenInclude(q => q.NhanVienGoiMau).ThenInclude(q => q.User)
                .Include(q => q.NhanVienKetLuan).ThenInclude(q => q.User)
                 .Include(q => q.NhanVienThucHien).ThenInclude(q => q.User)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.NhanVienKetLuan).ThenInclude(q => q.User)

                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.TinhThanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.HopDongKhamSucKhoe).ThenInclude(p => p.CongTyKhamSucKhoe)

                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.MayXetNghiem)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NoiChiDinh)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauTiepNhan)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NoiChiDinh)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)

                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri).ThenInclude(x => x.ChanDoanChinhICD)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.User)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(d => d.DichVuXetNghiem).ThenInclude(p => p.DichVuKyThuatBenhViens)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)

                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.User)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.HocHamHocVi)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.HocHamHocVi)
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.HocHamHocVi)
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.YeuCauTiepNhanTheBHYTs)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.HopDongKhamSucKhoe).ThenInclude(p => p.CongTyKhamSucKhoe)

                .FirstOrDefaultAsync(q => q.Id == ketQuaXetNghiemPhieuIn.Id);

            var phienXetNghiemChiTietEntities = phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => o.NhanVienKetLuanId != null).ToList();

            var thongTinBacSi = phienXetNghiemData.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                .Where(cc => cc.YeuCauKhamBenh != null || cc.NoiTruPhieuDieuTriId != null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                //.Select(p => p.YeuCauKhamBenh)
                .Select(q => new ThongTinBacSiVo
                {
                    //YeuCauKhamBenhId = q.Id,
                    BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinh?.Ten
                }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinLeTan = phienXetNghiemData.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                .Where(cc => cc.YeuCauKhamBenh == null && cc.NoiTruPhieuDieuTriId == null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    //YeuCauKhamBenhId = q.Id,
                    BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinh?.Ten,
                    FromLeTan = true,
                }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinPhieuIns = thongTinBacSi;
            thongTinPhieuIns.AddRange(thongTinLeTan);

            // BVHD-3897
            var thongTinPhieuInTheoNhanVienChiDinhs = thongTinPhieuIns.GroupBy(d => d.BacSiChiDinhId)
                                                                   .Select(d => new ThongTinBacSiVo
                                                                   {
                                                                       BacSiChiDinh = d.First().BacSiChiDinh,
                                                                       BacSiChiDinhId = d.First().BacSiChiDinhId,
                                                                       KhoaPhongChiDinh = d.First().KhoaPhongChiDinh // lấy first
                                                                   })
                                                                   .ToList();

            foreach (var thongTin in thongTinPhieuInTheoNhanVienChiDinhs)
            {
                var data = new DuyetKetQuaXetNghiemPhieuInResultVo
                {
                    SoPhieu = phienXetNghiemData.MauXetNghiems.LastOrDefault(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
                    
                    SoVaoVien = phienXetNghiemData.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    LogoUrl = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha-full.png",
                    MaYTe = phienXetNghiemData.BenhNhan.MaBN,
                    
                    HoTen = phienXetNghiemData.YeuCauTiepNhan.HoTen,
                    DiaChi = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoe?.CongTyKhamSucKhoe?.Ten : phienXetNghiemData.YeuCauTiepNhan.DiaChiDayDu,
                    GhiChu = phienXetNghiemData.GhiChu,
                    NamSinh = phienXetNghiemData.YeuCauTiepNhan.NamSinh,
                    GioiTinh = phienXetNghiemData.YeuCauTiepNhan.GioiTinh != null ? phienXetNghiemData.YeuCauTiepNhan.GioiTinh : null,
                    NamSinhString = DateHelper.DOBFormat(phienXetNghiemData.YeuCauTiepNhan.NgaySinh, phienXetNghiemData.YeuCauTiepNhan.ThangSinh, phienXetNghiemData.YeuCauTiepNhan.NamSinh),
                    
                    DoiTuong = phienXetNghiemData.YeuCauTiepNhan.CoBHYT == true ? "BHYT (" + phienXetNghiemData.YeuCauTiepNhan.BHYTMucHuong + "%)" : "Viện phí",
                   
                            
                    MaBHYT =  phienXetNghiemData.YeuCauTiepNhan?.BHYTMaSoThe,
                    BsChiDinh = returnStringTenTheoNhanVien(thongTin.BacSiChiDinhId),
                    KhoaPhong = thongTin.KhoaPhongChiDinh,
                    Barcode = phienXetNghiemData.BarCodeId.Length == 10 ? phienXetNghiemData.BarCodeId.Substring(0, 6) + "-" + phienXetNghiemData.BarCodeId.Substring(6) : phienXetNghiemData.BarCodeId
                };

                if (phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null)
                {
                    data.STT = phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
                }

                data.BarCodeImgBase64 = !string.IsNullOrEmpty(ketQuaXetNghiemPhieuIn.Id.ToString())
                    ? BarcodeHelper.GenerateBarCode(ketQuaXetNghiemPhieuIn.Id.ToString())
                    : "";

                List<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiets = phienXetNghiemChiTietEntities
                    .SelectMany(p => p.KetQuaXetNghiemChiTiets)
                    .Where(p => p.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId).ToList();

                var lstYeuCauDichVuKyThuatIdTrongPhieu = new List<long>();
                if (ketQuaXetNghiemChiTiets.Any())
                {
                    if (ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds.Any())
                    {
                        var info = string.Empty;
                        foreach (var nhomDichVuBvId in ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds)
                        {
                            var totalTenNhom = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVienId == nhomDichVuBvId).Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();
                            var STT = 1;
                            foreach (var tenNhom in totalTenNhom)
                            {
                                var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b>" + tenNhom.ToUpper()
                                                        + "</b></tr>";
                                info += headerNhom;
                                var queryNhom = new List<KetQuaXetNghiemChiTiet>();
                                var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                                    .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

                                foreach (var ycId in lstYeuCauDichVuKyThuatId)
                                {
                                    if (!phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                                    var res = phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                                    queryNhom.AddRange(res);
                                }
                                lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);

                                foreach (var dataParent in queryNhom.Where(p => p.DichVuXetNghiemChaId == null).OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                                {
                                    info = info + GetPhieuIn("", STT, queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList(), dataParent);
                                }
                            }
                        }
                        data.DanhSach = info;

                    }
                    else
                    {
                        var totalTenNhom = ketQuaXetNghiemChiTiets.Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();
                        var info = string.Empty;
                        var STT = 1;
                        foreach (var tenNhom in totalTenNhom)
                        {
                            var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                    + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b>" + tenNhom.ToUpper()
                                                    + "</b></tr>";

                            info += headerNhom;
                            var queryNhom = new List<KetQuaXetNghiemChiTiet>();
                            var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                                .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

                            foreach (var ycId in lstYeuCauDichVuKyThuatId)
                            {
                                if (!phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                                var res = phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                                queryNhom.AddRange(res);
                            }
                            lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);

                            foreach (var dataParent in queryNhom.Where(p => p.DichVuXetNghiemChaId == null).OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                            {
                                info = info + GetPhieuIn("", STT, queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList(), dataParent);
                            }



                        }

                        data.DanhSach = info;
                    }
                }

                data.NguoiThucHien = returnStringTen(phienXetNghiemData.NhanVienThucHien?.HocHamHocVi?.Ma, phienXetNghiemData.NhanVienThucHien?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemData.NhanVienThucHien?.User?.HoTen);

                //var chanDoans = phienXetNghiemData.PhienXetNghiemChiTiets
                //    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                //    .Select(o => o.PhienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "Khám sức khỏe" :
                //        (o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null
                //            ? o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu
                //            : o.YeuCauDichVuKyThuat.YeuCauKhamBenh?.ChanDoanSoBoGhiChu)).Distinct();
                // lấy chẩn icd mới nhất của bác sĩ .. cái cuối cùng

                data.ChanDoan = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                        (phienXetNghiemData.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                            ? phienXetNghiemData.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                                .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Where(p => p.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh.IcdchinhId != null)
                                .Select(p => (p.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + p.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet)).Where(p => p != null && p != "-" && p != "").Distinct().LastOrDefault()
                            : "") :
                            (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                            phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => (o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet))
                                                                     .Where(p => p != null && p != "-" && p != "").LastOrDefault() :
                            (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : ""));// ksk de trong



                data.DienGiai = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                             (phienXetNghiemData.YeuCauTiepNhan.YeuCauKhamBenhs.SelectMany(d => d.YeuCauDichVuKyThuats)
                                .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Any(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                               ? phienXetNghiemData.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                                   .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id))
                                   .Where(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                                   .Select(p => p.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).ToList().Distinct().LastOrDefault()
                               : "") :
                               (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                               phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).ToList().Distinct().LastOrDefault() :
                               (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : "")); // ksk de trong

                var enumLoaiMauXetNghiems = phienXetNghiemData.PhienXetNghiemChiTiets
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                    .Select(o => o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem).Distinct();

                //data.ChanDoan = string.Join("; ", chanDoans.Where(o => !string.IsNullOrEmpty(o)));
                data.LoaiMau = string.Join("; ", enumLoaiMauXetNghiems.Select(o => o.GetDescription()));

                var phienXetNghiemChiTietLast = phienXetNghiemData.PhienXetNghiemChiTiets
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();
                if (phienXetNghiemChiTietLast != null)
                {
                    data.TgLayMau = phienXetNghiemChiTietLast.ThoiDiemLayMau?.ApplyFormatDateTimeSACH();

                    data.NguoiLayMau = returnStringTen(phienXetNghiemChiTietLast.NhanVienLayMau?.HocHamHocVi?.Ma, phienXetNghiemChiTietLast.NhanVienLayMau?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemChiTietLast.NhanVienLayMau?.User?.HoTen);

                    data.TgNhanMau = phienXetNghiemChiTietLast.ThoiDiemNhanMau?.ApplyFormatDateTimeSACH();

                    data.NguoiNhanMau = returnStringTen(phienXetNghiemChiTietLast.NhanVienNhanMau?.HocHamHocVi?.Ma, phienXetNghiemChiTietLast.NhanVienNhanMau?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemChiTietLast.NhanVienNhanMau?.User?.HoTen);
                }
                data.Ngay = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                data.Thang = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                data.Nam = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                data.Gio = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";
                data.NgayThangNamFooter = DateTime.Now.ApplyFormatDateTimeSACH();
                var lstContentItem = new PhieuInXetNghiemModel();
                // 
                if (!string.IsNullOrEmpty(data.DanhSach))
                {
                    lstContentItem.Html = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, data);
                    lstContent.Add(lstContentItem);
                }
            }

            return lstContent;
        }

        private string GetPhieuInKetQua(string info, int stt, List<KetQuaXetNghiemChiTiet> queryNhom, KetQuaXetNghiemChiTiet queryIn, bool isLev2 = false)
        {
            if (queryIn == null) return info;

            if (queryNhom.Any(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId) && queryIn.DichVuXetNghiemChaId == null)
            {
                info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b style='margin-left: 20px;'>" +
                                queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                                + "</b></td></tr>";
            }
            else
            {
                var toDam = queryIn.ToDamGiaTri;
                var capDichVu = queryIn.DichVuXetNghiem?.CapDichVu;
                var kq = !string.IsNullOrEmpty(queryIn.GiaTriDuyet) ? queryIn.GiaTriDuyet : !string.IsNullOrEmpty(queryIn.GiaTriNhapTay) ? queryIn.GiaTriNhapTay : !string.IsNullOrEmpty(queryIn.GiaTriTuMay) ? queryIn.GiaTriTuMay : string.Empty;
                if (capDichVu == 2)
                {
                    bool capDichVuCoCap3 = false;
                    var index = _dichVuXetNghiemRepository.TableNoTracking.Where(s => s.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId).ToList();
                    if (index.Any())
                    {
                        capDichVuCoCap3 = true;
                    }
                    info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                             + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 20px;'>" +
                                              (capDichVuCoCap3 == true ? "<b>" + queryIn.DichVuXetNghiem?.Ten + "</b>" : queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault())

                                             + "</p></td>"
                                             + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                               "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                             + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                             + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                             + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                             + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                             + "</tr>";
                }
                else if (capDichVu == 3)
                {
                    info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                             + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 30px;'>" +
                                             queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                                             + "</p></td>"
                                             + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                               "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                             + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                             + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                             + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                             + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                             + "</tr>";
                }
                else
                {
                    string bienDinhNhomMau = "Định nhóm máu";
                    var kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau = queryIn.DichVuKyThuatBenhVien?.Ten.RemoveVietnameseDiacritics().Contains(bienDinhNhomMau.RemoveVietnameseDiacritics());
                    if (kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau == true)
                    {
                        info = info + "<tr style='border: 1px solid #020000;text-align: center;height:50px;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                                       queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                                       + "</p></td>"
                                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                       + "</tr>";
                    }
                    else
                    {
                        info = info + "<tr style='border: 1px solid #020000;text-align: center;'>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                                      queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                                      + "</p></td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                        "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                      + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                      + "</tr>";
                    }
                }
                //if (isLev2)
                //{

                //    info = info              + "<tr style='border: 1px solid #020000;text-align: center; '>"
                //                             + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 20px;'>" +
                //                             queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                //                             + "</p></td>"
                //                             + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                //                                               "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                //                             + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                //                             + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                //                             + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                //                             + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                //                             + "</tr>";
                //}
                //else
                //{
                //    // chỉ có 2 cấp 
                //    string bienDinhNhomMau = "Định nhóm máu";
                //    var kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau = queryIn.DichVuKyThuatBenhVien?.Ten.RemoveVietnameseDiacritics().Contains(bienDinhNhomMau.RemoveVietnameseDiacritics());
                //    if (kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau == true)
                //    {
                //        info = info + "<tr style='border: 1px solid #020000;text-align: center;height:50px;'>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                //                       queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                //                       + "</p></td>"
                //                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                //                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                //                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                //                       + "</tr>";
                //    }
                //    else
                //    {
                //        info = info + "<tr style='border: 1px solid #020000;text-align: center;'>"
                //                      + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                //                      queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                //                      + "</p></td>"
                //                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                //                                        "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                //                      + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                //                      + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                //                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                //                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                //                      + "</tr>";
                //    }

                //}

                stt++;
            }

            var lstChild = queryNhom.Where(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId).ToList();
            foreach (var item in lstChild)
            {
                info += GetPhieuInKetQua("", stt, queryNhom, item, true);
            }

            return info;
        }

        private string GetPhieuIn(string info, int stt, List<KetQuaXetNghiemChiTiet> queryNhom, KetQuaXetNghiemChiTiet queryIn, bool isLev2 = false)
        {

            if (queryIn == null) return info;

            if (queryNhom.Any(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId) && queryIn.DichVuXetNghiemChaId == null) // xét nghiệm k có nhóm con
            {
                var toDam = queryIn.ToDamGiaTri;
                var isCapDichVu = queryIn.DichVuXetNghiem?.CapDichVu;
                var kq = !string.IsNullOrEmpty(queryIn.GiaTriDuyet) ? queryIn.GiaTriDuyet : !string.IsNullOrEmpty(queryIn.GiaTriNhapTay) ? queryIn.GiaTriNhapTay : !string.IsNullOrEmpty(queryIn.GiaTriTuMay) ? queryIn.GiaTriTuMay : string.Empty;
                var valueMin = queryIn.GiaTriMin;
                var valueMax = queryIn.GiaTriMax;
                if (string.IsNullOrEmpty(kq)) //dịch vụ cấp 1 ket qua = null => 1 dòng
                {
                    info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                               + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b style='margin-left: 10px;'>" +
                               //queryIn.DichVuXetNghiem?.Ten
                               queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                               + "</b></td></tr>";
                }
                else //dịch vụ cấp 1 ket qua != null => in kết quả 
                {
                    info = info + "<tr style='border: 1px solid #020000;text-align: center; height:50px;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;font-weight: bold;'><p style='margin-left: 10px;'>" +
                                       (queryIn.CapDichVu == 1 ? queryIn.DichVuKyThuatBenhVien?.Ten : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                }

            }
            else // xét nghiệm có nhóm con
            {
                //var dd = queryIn.
                var toDam = queryIn.ToDamGiaTri;
                var isCapDichVu = queryIn.DichVuXetNghiem?.CapDichVu;
                var kq = !string.IsNullOrEmpty(queryIn.GiaTriDuyet) ? queryIn.GiaTriDuyet : !string.IsNullOrEmpty(queryIn.GiaTriNhapTay) ? queryIn.GiaTriNhapTay : !string.IsNullOrEmpty(queryIn.GiaTriTuMay) ? queryIn.GiaTriTuMay : string.Empty;

                //Bước thực hiện: Danh sách duyệt kết quả - In kết quả xét nghiệm
                //Hiện tại: Những dịch vụ có cấp(Bố - Con - Cháu) khi không có kết quả nhập ở trường con thì in phiếu trả kết quả xét nghiệm bị chia ô như có kết quả.
                //Mong muốn: Dịch vụ mà có cấp bậc(Bố -Con - Cháu) không nhập kết quả ở trường con thì in ra sẽ meager thành 1 dòng, 
                //nếu mà có kết quả nhập ở trường kết quả ở trường con sẽ không mager lại.Chỉ áp dụng những dịch vụ có cấp bậc(Bố - Con - Cháu).không áp dụng vs những dịch vụ khác.
                bool ketquaNull = true;
                if (string.IsNullOrEmpty(kq) && KiemTraDichVuXetNghiemDichjVuCaphaiCoDichVuCapBa(queryIn, queryNhom) == true)
                {
                    ketquaNull = false;
                }

                if (isCapDichVu == 2)
                {  // lv2
                    bool capDichVuCoCap3 = false;
                    var index = _dichVuXetNghiemRepository.TableNoTracking.Where(s => s.DichVuXetNghiemChaId == (long)queryIn.DichVuXetNghiemId).ToList();
                    if (index.Any())
                    {
                        capDichVuCoCap3 = true;
                    }
                    if (ketquaNull == true)
                    {
                        info = info
                                           + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                           + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 20px;'>" +
                                            (capDichVuCoCap3 == true ? "<b>" + queryIn.DichVuXetNghiem?.Ten + "</b>" : queryIn.DichVuXetNghiem?.Ten)
                                           + "</p>" + "</td>"
                                           + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                               "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                           + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                           + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                           + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                           + "</tr>";
                    }
                    else
                    {
                        info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b style='margin-left: 10px;'><p style='margin-left: 20px;'>" +
                                queryIn.DichVuXetNghiem?.Ten + "</p>"
                               + "</b></td></tr>";
                    }

                }
                else if (isCapDichVu == 3)
                {
                    info = info
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 30px;'>" +
                                        queryIn.DichVuXetNghiem?.Ten
                                       + "</p>" + "</td>"
                                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                           "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                }
                else
                {
                    //lv1
                    string bienDinhNhomMau = "Định nhóm máu";
                    var kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau = queryIn.DichVuKyThuatBenhVien?.Ten.RemoveVietnameseDiacritics().Contains(bienDinhNhomMau.RemoveVietnameseDiacritics());
                    if (kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau == true)
                    {


                        info = info + "<tr style='border: 1px solid #020000;text-align: center; height:50px;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                                       (queryIn.CapDichVu == 1 ? queryIn.DichVuKyThuatBenhVien?.Ten : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                                 "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                    }
                    else
                    {
                        info = info + "<tr style='border: 1px solid #020000;text-align: center;'>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                                      (queryIn.CapDichVu == 1 ? queryIn.DichVuKyThuatBenhVien?.Ten : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                                "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                      + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                      + "</tr>";
                    }
                }


                //if (isLev2)
                //{  // lv2
                //    info = info
                //                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 20px;'>" +
                //                        queryIn.DichVuXetNghiem?.Ten
                //                       + "</p>" + "</td>"
                //                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                //                                           "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" 
                //                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                //                       + "</tr>";
                //}
                //else
                //{
                //    //lv1
                //    string bienDinhNhomMau = "Định nhóm máu";
                //    var kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau = queryIn.DichVuKyThuatBenhVien?.Ten.RemoveVietnameseDiacritics().Contains(bienDinhNhomMau.RemoveVietnameseDiacritics());
                //    if (kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau == true)
                //    {


                //        info = info + "<tr style='border: 1px solid #020000;text-align: center; height:50px;'>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" + 
                //                       (queryIn.CapDichVu == 1 ? queryIn.DichVuKyThuatBenhVien?.Ten : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                //                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                //                                                 "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                //                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "")+ "</td>"
                //                       + "</tr>";
                //    }
                //    else
                //    {
                //        info = info + "<tr style='border: 1px solid #020000;text-align: center;'>"
                //                      + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                //                      (queryIn.CapDichVu == 1 ? queryIn.DichVuKyThuatBenhVien?.Ten : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                //                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                //                                                "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                //                      + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                //                      + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                //                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                //                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                //                      + "</tr>";
                //    }

                //}

                stt++;
            }

            var lstChild = queryNhom.Where(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId).ToList();
            foreach (var item in lstChild)
            {
                info += GetPhieuIn("", stt, queryNhom, item, true);
            }

            return info;
        }

        public async Task<PhienXetNghiem> DuyetOnGrid(DuyetKetQuaXetNghiemForServiceProcessingVo requestUpdateModelOnGrid, PhienXetNghiem phienXetNghiem, bool currentCheck, long? idParent = null)
        {
            if (idParent == null || idParent == 0) return phienXetNghiem;

            //step 1 : update entity
            var userId = _userAgentHelper.GetCurrentUserId();
            var user = await
                _user.TableNoTracking.FirstOrDefaultAsync(q => q.Id == userId);
            long dichVuXetNghiemCurrentId = 0;
            long currentYcdvktId = 0;
            if (phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Any(p => p.Id == idParent))
            {
                var kqXetNghiemChiTietNeedUpdate = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                    .First(p => p.Id == idParent);
                if (requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any())
                {
                    kqXetNghiemChiTietNeedUpdate.ToDamGiaTri = requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.Id == idParent).ToDamGiaTri;
                    kqXetNghiemChiTietNeedUpdate.GiaTriDuyet = requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.Id == idParent).GiaTriDuyet;
                    requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.Id == idParent).Duyet = currentCheck;
                }
                if (!currentCheck)
                {
                    kqXetNghiemChiTietNeedUpdate.DaDuyet = null;
                }
                else
                {
                    kqXetNghiemChiTietNeedUpdate.DaDuyet = currentCheck;
                }

                if (currentCheck)
                {
                    if (requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any())
                    {
                        requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.Id == idParent).NguoiDuyet = user.HoTen;
                        requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.Id == idParent).ThoiDiemDuyetKetQua =
                            DateTime.Now;
                    }
                    //Update PhienXetNghiemChiTiet
                    var ketQuaXetNghiemChiTiets = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(z => z.KetQuaXetNghiemChiTiets.Where(x => x.LanThucHien == z.LanThucHien && x.PhienXetNghiemChiTietId == z.Id));
                    if (ketQuaXetNghiemChiTiets.All(x => x.NhanVienDuyetId == null))
                    {
                        ketQuaXetNghiemChiTiets.First().PhienXetNghiemChiTiet.NhanVienKetLuanId = userId;
                        ketQuaXetNghiemChiTiets.First().PhienXetNghiemChiTiet.ThoiDiemKetLuan = DateTime.Now;
                    }
                    kqXetNghiemChiTietNeedUpdate.ThoiDiemDuyetKetQua = DateTime.Now;
                    kqXetNghiemChiTietNeedUpdate.NhanVienDuyetId = userId;
                }
                else
                {
                    kqXetNghiemChiTietNeedUpdate.ThoiDiemDuyetKetQua = null;
                    kqXetNghiemChiTietNeedUpdate.NhanVienDuyetId = null;
                    if (requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any())
                    {
                        requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.Id == idParent).NguoiDuyet = string.Empty;
                        requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.Id == idParent).ThoiDiemDuyetKetQua =
                            null;
                    }
                    var ketQuaXetNghiemChiTiets = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(z => z.KetQuaXetNghiemChiTiets.Where(x => x.LanThucHien == z.LanThucHien && x.PhienXetNghiemChiTietId == z.Id));
                    if (ketQuaXetNghiemChiTiets.All(x => x.NhanVienDuyetId == null))
                    {
                        ketQuaXetNghiemChiTiets.First().PhienXetNghiemChiTiet.NhanVienKetLuanId = null;
                        ketQuaXetNghiemChiTiets.First().PhienXetNghiemChiTiet.ThoiDiemKetLuan = null;
                    }
                }

                dichVuXetNghiemCurrentId = kqXetNghiemChiTietNeedUpdate.DichVuXetNghiemId;
                currentYcdvktId = kqXetNghiemChiTietNeedUpdate.YeuCauDichVuKyThuatId;


            }

            //find a list child entity
            if (phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                .Any(p => p.DichVuXetNghiemChaId == dichVuXetNghiemCurrentId && p.YeuCauDichVuKyThuatId == currentYcdvktId))
            {
                var lstItemChild = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                    .Where(p => p.DichVuXetNghiemChaId == dichVuXetNghiemCurrentId && p.YeuCauDichVuKyThuatId == currentYcdvktId)
                    .Select(p => new ChildrenInfo { Id = p.Id, YcdvktId = p.YeuCauDichVuKyThuatId, DichVuXetNghiemId = p.DichVuXetNghiemId }).Distinct();

                var lstItemChildInList = lstItemChild.ToList();

                if (requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any(e => e.Id == idParent && e.Level == 1) &&
                    !requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(e => e.Id == idParent && e.Level == 1).IdChilds.Except(lstItemChildInList.Select(q => q.Id)).Any())
                {
                    foreach (var idChild in requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(e => e.Id == idParent && e.Level == 1).IdChilds)
                    {
                        await DuyetOnGrid(requestUpdateModelOnGrid, phienXetNghiem, currentCheck, idChild);

                        if (phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                            .Any(p => p.DichVuXetNghiemChaId == lstItemChildInList.First(w => w.Id == idChild).DichVuXetNghiemId &&
                                      p.YeuCauDichVuKyThuatId == lstItemChildInList.First(w => w.Id == idChild).YcdvktId))
                        {
                            var lstIdLvlTroisChildren = phienXetNghiem.PhienXetNghiemChiTiets
                                .SelectMany(p => p.KetQuaXetNghiemChiTiets)
                                .Where(p => p.DichVuXetNghiemChaId == lstItemChildInList.First(w => w.Id == idChild).DichVuXetNghiemId &&
                                            p.YeuCauDichVuKyThuatId == lstItemChildInList.First(w => w.Id == idChild).YcdvktId)
                                .Select(p => p.Id).Distinct();
                            if (requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any(e =>
                                    e.Id == idChild && e.Level == 2) &&
                                !requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems
                                    .First(e => e.Id == idChild && e.Level == 2).IdChilds.Except(lstIdLvlTroisChildren)
                                    .Any())
                            {
                                foreach (var idChildLvlTrois in requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(e =>
                                    e.Id == idChild && e.Level == 2).IdChilds)
                                {
                                    await DuyetOnGrid(requestUpdateModelOnGrid, phienXetNghiem, currentCheck, idChildLvlTrois);
                                }
                            }
                        }
                    }
                }
            }

            return phienXetNghiem;
        }

        public async Task<PhienXetNghiem> ApproveOnGroup(
            DuyetKetQuaXetNghiemForServiceProcessingVo requestUpdateModelOnGrid,
            PhienXetNghiem entity, bool currentCheck, long currentNhomId)
        {
            var userId = _userAgentHelper.GetCurrentUserId();
            var user = await
                _user.TableNoTracking.FirstOrDefaultAsync(q => q.Id == userId);
            foreach (var chiTietKetQuaXetNghiem in requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems)
            {
                if (entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Any(p => p.Id == chiTietKetQuaXetNghiem.Id))
                {
                    var kqXetNghiemChiTietNeedUpdate = entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                        .First(p => p.Id == chiTietKetQuaXetNghiem.Id);
                    kqXetNghiemChiTietNeedUpdate.ToDamGiaTri = chiTietKetQuaXetNghiem.ToDamGiaTri;
                    kqXetNghiemChiTietNeedUpdate.GiaTriDuyet = chiTietKetQuaXetNghiem.GiaTriDuyet;
                    kqXetNghiemChiTietNeedUpdate.DaDuyet = currentCheck;
                    chiTietKetQuaXetNghiem.Duyet = currentCheck;

                    if (currentCheck)
                    {
                        kqXetNghiemChiTietNeedUpdate.ThoiDiemDuyetKetQua = DateTime.Now;
                        kqXetNghiemChiTietNeedUpdate.NhanVienDuyetId = userId;
                        chiTietKetQuaXetNghiem.NguoiDuyet = user.HoTen;
                        chiTietKetQuaXetNghiem.ThoiDiemDuyetKetQua =
                            DateTime.Now;
                    }
                    else
                    {
                        kqXetNghiemChiTietNeedUpdate.ThoiDiemDuyetKetQua = null;
                        kqXetNghiemChiTietNeedUpdate.NhanVienDuyetId = null;
                        chiTietKetQuaXetNghiem.NguoiDuyet = string.Empty;
                        chiTietKetQuaXetNghiem.ThoiDiemDuyetKetQua = null;
                    }
                }
            }

            return entity;
        }

        public PhienXetNghiem DuyetOnLevelOne(DuyetKetQuaXetNghiemForServiceProcessingVo requestUpdateModelOnGrid, PhienXetNghiem entity, long idCurrent, bool currentCheck)
        {
            var userId = _userAgentHelper.GetCurrentUserId();
            if (requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any(w => w.IdChilds.Contains(idCurrent) && w.Level == 1))
            {
                var levelOneOnScreenNeedUpdate = requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.IdChilds.Contains(idCurrent) && w.Level == 1);

                if (levelOneOnScreenNeedUpdate.IdChilds.All(idLvlDeux =>
                    requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any(lvlDeux => lvlDeux.Id == idLvlDeux && lvlDeux.Duyet == true
                    && lvlDeux.IdChilds.All(idLvlTrois => requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.
                        Any(lvlTrois => lvlTrois.Id == idLvlTrois && lvlTrois.Duyet == true)))) && currentCheck)
                {
                    levelOneOnScreenNeedUpdate.Duyet = true;

                    if (entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                        .Any(p => p.Id == levelOneOnScreenNeedUpdate.Id))
                    {
                        var entityNeedToUpdate = entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                            .First(p => p.Id == levelOneOnScreenNeedUpdate.Id);
                        entityNeedToUpdate.DaDuyet = true;
                        entityNeedToUpdate.ThoiDiemDuyetKetQua = DateTime.Now;
                        entityNeedToUpdate.NhanVienDuyetId = userId;
                    }
                }
                else
                {
                    levelOneOnScreenNeedUpdate.Duyet = false;

                    if (entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                        .Any(p => p.Id == levelOneOnScreenNeedUpdate.Id))
                    {
                        var entityNeedToUpdate = entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                            .First(p => p.Id == levelOneOnScreenNeedUpdate.Id);
                        entityNeedToUpdate.DaDuyet = null;
                        entityNeedToUpdate.ThoiDiemDuyetKetQua = null;
                        entityNeedToUpdate.NhanVienDuyetId = null;
                    }
                }
            }

            if (requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any(
                w => w.IdChilds.Contains(idCurrent) && w.Level == 2))
            {
                var levelDeuxOnScreenNeedUpdate = requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.IdChilds.Contains(idCurrent) && w.Level == 2);
                var levelOneOnScreenNeedUpdate = requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.First(w => w.IdChilds.Contains(levelDeuxOnScreenNeedUpdate.Id) && w.Level == 1);

                if (levelOneOnScreenNeedUpdate.IdChilds.All(idLvlDeux =>
                        requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.Any(lvlDeux => lvlDeux.Id == idLvlDeux && lvlDeux.Duyet == true
                                                                                                                && lvlDeux.IdChilds.All(idLvlTrois => requestUpdateModelOnGrid.ChiTietKetQuaXetNghiems.
                                                                                                                    Any(lvlTrois => lvlTrois.Id == idLvlTrois && lvlTrois.Duyet == true)))) && currentCheck)
                {
                    levelOneOnScreenNeedUpdate.Duyet = true;

                    if (entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                        .Any(p => p.Id == levelOneOnScreenNeedUpdate.Id))
                    {
                        var entityNeedToUpdate = entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                            .First(p => p.Id == levelOneOnScreenNeedUpdate.Id);
                        entityNeedToUpdate.DaDuyet = true;
                        entityNeedToUpdate.ThoiDiemDuyetKetQua = DateTime.Now;
                        entityNeedToUpdate.NhanVienDuyetId = userId;
                    }
                }
                else
                {
                    levelOneOnScreenNeedUpdate.Duyet = false;

                    if (entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                        .Any(p => p.Id == levelOneOnScreenNeedUpdate.Id))
                    {
                        var entityNeedToUpdate = entity.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets)
                            .First(p => p.Id == levelOneOnScreenNeedUpdate.Id);
                        entityNeedToUpdate.DaDuyet = null;
                        entityNeedToUpdate.ThoiDiemDuyetKetQua = null;
                        entityNeedToUpdate.NhanVienDuyetId = null;
                    }
                }
            }
            return entity;
        }

        public async Task<List<NhomDichVuXetNghiemDuyetVo>> GetNhomDichVuDuyets(long phienXetNghiemId)
        {
            var ketQuaXNs = await _phienXetNghiemChiTietRepository.TableNoTracking
                            .Where(p => p.PhienXetNghiemId == phienXetNghiemId)
                            .Select(s => new
                            {
                                s.NhomDichVuBenhVienId,
                                s.NhomDichVuBenhVien.Ten
                            }).Distinct().ToListAsync();
            var nhomDVXNs = new List<NhomDichVuXetNghiemDuyetVo>();
            foreach (var item in ketQuaXNs)
            {
                var nhomDVXN = new NhomDichVuXetNghiemDuyetVo
                {
                    IsCheck = true,
                    NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                    TenNhomDichVu = item.Ten
                };
                nhomDVXNs.Add(nhomDVXN);
            }
            return nhomDVXNs;
        }

        public async Task<long?> TimKiemPhienXetNghiemGanNhat(PhienXNGanNhatVo phienXNGanNhatVo)
        {
            var query = BaseRepository.TableNoTracking;
            
            if (phienXNGanNhatVo.SearchStringBarCode.Length == 4)
            {
                if (Int32.TryParse(phienXNGanNhatVo.SearchStringBarCode, out var barCodeNumber))
                {
                    query = query.Where(o => o.BarCodeNumber == barCodeNumber);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                query = query.Where(o => o.BarCodeId == phienXNGanNhatVo.SearchStringBarCode);
            }

            if (!string.IsNullOrEmpty(phienXNGanNhatVo.FromDate) || !string.IsNullOrEmpty(phienXNGanNhatVo.ToDate))
            {
                DateTime tuNgay = DateTime.MinValue;
                DateTime denNgay = DateTime.Now;

                if (!string.IsNullOrEmpty(phienXNGanNhatVo.FromDate))
                {
                    phienXNGanNhatVo.FromDate.TryParseExactCustom(out tuNgay);
                }

                if (!string.IsNullOrEmpty(phienXNGanNhatVo.ToDate))
                {
                    phienXNGanNhatVo.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                query = query.Where(p => p.ThoiDiemBatDau >= tuNgay && p.ThoiDiemBatDau <= denNgay);
            }
            var phienXNs = query.Select(o => new
            {
                o.Id,
                KetQuaXetNghiemChiTietIds = o.PhienXetNghiemChiTiets.SelectMany(ct => ct.KetQuaXetNghiemChiTiets).Select(kq => kq.Id).ToList(),
                o.CreatedOn
            }).ToList();

            return phienXNs.Where(o=>o.KetQuaXetNghiemChiTietIds.Any()).OrderByDescending(z => z.CreatedOn).FirstOrDefault()?.Id;
        }
        public async Task<long?> TimKiemPhienXetNghiemGanNhatOld(PhienXNGanNhatVo phienXNGanNhatVo)
        {
            var query = BaseRepository.TableNoTracking
                            .Where(p => p.PhienXetNghiemChiTiets.Any(z => z.KetQuaXetNghiemChiTiets.Any())//); 
                             && (phienXNGanNhatVo.SearchStringBarCode.Length == 4 ? phienXNGanNhatVo.SearchStringBarCode == p.BarCodeNumber.ToString("D4") : phienXNGanNhatVo.SearchStringBarCode == p.BarCodeId));

            if (!string.IsNullOrEmpty(phienXNGanNhatVo.FromDate) || !string.IsNullOrEmpty(phienXNGanNhatVo.ToDate))
            {
                DateTime tuNgay = DateTime.MinValue;
                DateTime denNgay = DateTime.Now;

                if (!string.IsNullOrEmpty(phienXNGanNhatVo.FromDate))
                {
                    phienXNGanNhatVo.FromDate.TryParseExactCustom(out tuNgay);
                }

                if (!string.IsNullOrEmpty(phienXNGanNhatVo.ToDate))
                {
                    phienXNGanNhatVo.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                query = query.Where(p => p.ThoiDiemBatDau >= tuNgay && p.ThoiDiemBatDau <= denNgay);
            }
            return query.OrderByDescending(z => z.CreatedOn).FirstOrDefault()?.Id;
        }
        #region update phiếu in (15072021) : in những dịch vụ chỉ định được check trên grid

        public async Task<List<PhieuInXetNghiemModel>> InDuyetKetQuaXetNghiemManHinhDuyet(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn)
        {
            var lstContent = new List<PhieuInXetNghiemModel>();

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKetQuaXetNghiemNew"));
            List<long> listDefault = new List<long>();
            //var phienXetNghiemData = BaseRepository.Table
            //    .Include(p => p.BenhNhan)
            //    .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem)
            //    .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienLayMau).ThenInclude(q => q.User)
            //    .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienNhanMau).ThenInclude(q => q.User)
            //    .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem).ThenInclude(q => q.NhanVienGoiMau).ThenInclude(q => q.User)
            //    .Include(q => q.NhanVienKetLuan).ThenInclude(q => q.User)
            //    .Include(q => q.NhanVienThucHien).ThenInclude(q => q.User)
            //    .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.TinhThanh)
            //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)

            //    .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
            //    .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)
            //    .Include(p => p.PhienXetNghiemChiTiets)                
            //    .Include(x => x.NhanVienThucHien).ThenInclude(x => x.HocHamHocVi)
            //    .Include(x => x.NhanVienThucHien).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
            //    .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.YeuCauTiepNhanTheBHYTs)
            //    .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.HopDongKhamSucKhoe).ThenInclude(p => p.CongTyKhamSucKhoe)
            //    .FirstOrDefault(q => q.Id == ketQuaXetNghiemPhieuIn.Id);

            //if (phienXetNghiemData != null)
            //{
            //    //Explicit loading
            //    var phienXetNghiemChiTiets = BaseRepository.Context.Entry(phienXetNghiemData).Collection(o => o.PhienXetNghiemChiTiets);
            //    phienXetNghiemChiTiets.Query()
            //        .Include(x => x.KetQuaXetNghiemChiTiets)
            //        .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh)
            //        .Include(p => p.NhomDichVuBenhVien)
            //        .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
            //        .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien)
            //        .Include(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
            //        .Include(x => x.NhanVienLayMau).ThenInclude(x => x.User)
            //        .Include(x => x.NhanVienNhanMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
            //        .Include(x => x.NhanVienLayMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
            //        .Include(x => x.NhanVienNhanMau).ThenInclude(x => x.HocHamHocVi)
            //        .Include(x => x.NhanVienLayMau).ThenInclude(x => x.HocHamHocVi)
            //        .Load();

            //    foreach (var phienXetNghiemChiTiet in phienXetNghiemData.PhienXetNghiemChiTiets)
            //    {
            //        var ketQuaXetNghiemChiTiets = BaseRepository.Context.Entry(phienXetNghiemChiTiet).Collection(o => o.KetQuaXetNghiemChiTiets);
            //        ketQuaXetNghiemChiTiets.Query()
            //            .Include(p => p.DichVuKyThuatBenhVien)
            //            .Include(x => x.DichVuXetNghiem)
            //            .Include(p => p.MayXetNghiem)
            //            .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NoiChiDinh)
            //            .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauTiepNhan)
            //            .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NoiChiDinh)
            //            .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
            //            .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
            //            .Include(p => p.NhomDichVuBenhVien)
            //            .Include(p => p.DichVuXetNghiem).ThenInclude(p => p.HDPP)
            //            .Load();
            //    }
            //}

            var thongTinPhienXetNghiem = BaseRepository.TableNoTracking
                .Where(o => o.Id == ketQuaXetNghiemPhieuIn.Id)
                .Select(o => new
                {
                    o.Id,
                    PhienXetNghiemChiTiets = o.PhienXetNghiemChiTiets.Select(ct => new
                    {
                        ct.Id,
                        ct.YeuCauDichVuKyThuatId,
                        ct.NhanVienKetLuanId
                    }).ToList(),
                    //Mau = o.MauXetNghiems.Select(m => new
                    //{
                    //    m.Id,
                    //    m.BarCodeId
                    //}).ToList(),
                    o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.YeuCauTiepNhan.DiaChiDayDu,
                    o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                    o.YeuCauTiepNhan.HoTen,
                    o.YeuCauTiepNhan.NgaySinh,
                    o.YeuCauTiepNhan.ThangSinh,
                    o.YeuCauTiepNhan.NamSinh,
                    o.YeuCauTiepNhan.CoBHYT,
                    o.YeuCauTiepNhan.BHYTMucHuong,
                    o.YeuCauTiepNhan.BHYTMaSoThe,
                    o.YeuCauTiepNhan.GioiTinh,
                    o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    o.BarCodeId,
                    o.GhiChu,
                    o.NhanVienThucHienId
                })
                .FirstOrDefault();
            //SoPhieu = phienXetNghiemData.MauXetNghiems.LastOrDefault(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
            var mauXetNghiems = _mauXetNghiemRepo.TableNoTracking
                .Where(o => o.PhienXetNghiemId == thongTinPhienXetNghiem.Id && o.BarCodeId == thongTinPhienXetNghiem.BarCodeId)
                .Select(o => new
                {
                    o.Id,
                    SoPhieu = o.PhieuGoiMauXetNghiemId != null ? o.PhieuGoiMauXetNghiem.SoPhieu : ""
                })
                .ToList();

            var nhanVienThucHien = _nhanVienRepo.TableNoTracking
                .Where(o => o.Id == thongTinPhienXetNghiem.NhanVienThucHienId)
                .Select(o => new
                {
                    HocHamHocVi = o.HocHamHocViId != null ? o.HocHamHocVi.Ma : "",
                    NhomChucDanh = o.ChucDanhId != null ? o.ChucDanh.NhomChucDanh.Ma : "",
                    o.User.HoTen
                })
                .FirstOrDefault();

            var nguoiThucHien = returnStringTen(nhanVienThucHien.HocHamHocVi, nhanVienThucHien.NhomChucDanh, nhanVienThucHien.HoTen);

            var phienXetNghiemChiTietIds = thongTinPhienXetNghiem.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).Select(o => o.Id).ToList();
            var yeuCauDichVuKyThuatIds = thongTinPhienXetNghiem.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).Select(o => o.YeuCauDichVuKyThuatId).ToList();

            var phienXetNghiemChiTietDatas = _phienXetNghiemChiTietRepository.TableNoTracking
                .Where(o => phienXetNghiemChiTietIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.YeuCauDichVuKyThuatId,
                    o.YeuCauDichVuKyThuat.NhanVienChiDinhId,
                    o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                    o.YeuCauDichVuKyThuat.YeuCauKhamBenhId,
                    o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId,
                    o.ThoiDiemNhanMau,
                    o.ThoiDiemKetLuan,
                    o.ThoiDiemLayMau,
                    o.NhanVienLayMauId,
                    o.NhanVienNhanMauId,
                    KetQuaXetNghiemChiTiets = o.KetQuaXetNghiemChiTiets.ToList()
                }).ToList();

            var dichVuXetNghiems = _dichVuXetNghiemRepository.TableNoTracking.Include(o => o.HDPP).ToList();
            var mayXetNghiems = _mayXetNghiemRepo.TableNoTracking.ToList();

            var dichVuKyThuatBenhVienIds = phienXetNghiemChiTietDatas.SelectMany(o => o.KetQuaXetNghiemChiTiets).Select(o => o.DichVuKyThuatBenhVienId).Distinct().ToList();

            var thongTinDichVuKyThuatBenhViens = _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Where(o => dichVuKyThuatBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ten,
                    o.DichVuChuyenGoi
                }).ToList();

            var tenDichVuKyThuatBenhViens = thongTinDichVuKyThuatBenhViens
                .Select(o => new LookupItemVo
                {
                    KeyId = o.Id,
                    DisplayName = o.Ten
                }).ToList();

            var nhomDichVuBenhViens = _nhomDichVuBenhVienRepo.TableNoTracking.Select(o => new
            {
                o.Id,
                o.Ten
            }).ToList();

            //var phienXetNghiemChiTietEntities = phienXetNghiemData.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).ToList();

            var thongTinBacSi = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o=> yeuCauDichVuKyThuatIds.Contains(o.Id) && (o.YeuCauKhamBenhId != null || o.NoiTruPhieuDieuTriId != null))
                .OrderBy(o => o.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    BacSiChiDinh = q.NhanVienChiDinh.User.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinhId != null ? q.NoiChiDinh.Ten : ""
                })//.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinLeTan = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => yeuCauDichVuKyThuatIds.Contains(o.Id) && o.YeuCauKhamBenhId == null && o.NoiTruPhieuDieuTriId == null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    BacSiChiDinh = q.NhanVienChiDinh.User.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinhId != null ? q.NoiChiDinh.Ten : "",
                    FromLeTan = true,
                })//.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            //var thongTinBacSi = phienXetNghiemData.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
            //    .Where(cc => cc.YeuCauKhamBenh != null || cc.NoiTruPhieuDieuTriId != null)
            //    .OrderBy(cc => cc.ThoiDiemHoanThanh)
            //    .Select(q => new ThongTinBacSiVo
            //    {
            //        BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
            //        BacSiChiDinhId = q.NhanVienChiDinhId,
            //        KhoaPhongChiDinh = q.NoiChiDinh?.Ten
            //    }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
            //    .ToList();

            //var thongTinLeTan = phienXetNghiemData.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
            //    .Where(cc => cc.YeuCauKhamBenh == null && cc.NoiTruPhieuDieuTriId == null)
            //    .OrderBy(cc => cc.ThoiDiemHoanThanh)
            //    .Select(q => new ThongTinBacSiVo
            //    {
            //        BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
            //        BacSiChiDinhId = q.NhanVienChiDinhId,
            //        KhoaPhongChiDinh = q.NoiChiDinh?.Ten,
            //        FromLeTan = true,
            //    }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
            //    .ToList();

            var thongTinPhieuIns = thongTinBacSi.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First()).ToList();
            thongTinPhieuIns.AddRange(thongTinLeTan.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First()).ToList());
            var numberPage = 1;

            string ngay = DateTime.Now.Day.ToString();
            string thang = DateTime.Now.Month.ToString();
            string nam = DateTime.Now.Year.ToString();
            string gio = DateTime.Now.Hour + " giờ " + DateTime.Now.Minute + " phút";

            var listYeuCauDichVuKyThuatThuocNhomSarsCov2s = GetListYeuCauTrongNhomSars(ketQuaXetNghiemPhieuIn.ListIn);

            var thongTinPhieuInTheoNhanVienChiDinhs = thongTinPhieuIns.GroupBy(d => d.BacSiChiDinhId)
                                                                      .Select(d => new ThongTinBacSiVo
                                                                      {
                                                                          BacSiChiDinh = d.First().BacSiChiDinh,
                                                                          BacSiChiDinhId = d.First().BacSiChiDinhId,
                                                                          KhoaPhongChiDinh = d.First().KhoaPhongChiDinh // lấy first
                                                                      })
                                                                      .ToList();

            foreach (var thongTin in thongTinPhieuInTheoNhanVienChiDinhs)
            {


                var data = new DuyetKetQuaXetNghiemPhieuInResultVo
                {
                    //SoPhieu = phienXetNghiemData.MauXetNghiems.LastOrDefault(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
                    SoPhieu = mauXetNghiems.LastOrDefault()?.SoPhieu,
                    SoVaoVien = thongTinPhienXetNghiem.MaYeuCauTiepNhan,
                    LogoUrl = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha-full.png",
                    MaYTe = thongTinPhienXetNghiem.MaBN,

                    HoTen = thongTinPhienXetNghiem.HoTen,
                    //DiaChi = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoe?.CongTyKhamSucKhoe?.Ten : phienXetNghiemData.YeuCauTiepNhan.DiaChiDayDu,
                    GhiChuDV = thongTinPhienXetNghiem.GhiChu,
                    NamSinh = thongTinPhienXetNghiem.NamSinh,
                    NamSinhString = DateHelper.DOBFormat(thongTinPhienXetNghiem.NgaySinh, thongTinPhienXetNghiem.ThangSinh, thongTinPhienXetNghiem.NamSinh),
                    GioiTinh = thongTinPhienXetNghiem.GioiTinh != null ? thongTinPhienXetNghiem.GioiTinh : null,

                    DoiTuong = thongTinPhienXetNghiem.CoBHYT == true ? "BHYT (" + thongTinPhienXetNghiem.BHYTMucHuong + "%)" : "Viện phí",

                    MaBHYT = thongTinPhienXetNghiem.BHYTMaSoThe,

                    BsChiDinh = returnStringTenTheoNhanVien(thongTin.BacSiChiDinhId),
                    KhoaPhong = thongTin.KhoaPhongChiDinh,
                    Barcode = thongTinPhienXetNghiem.BarCodeId.Length == 10 ? thongTinPhienXetNghiem.BarCodeId.Substring(0, 6) + "-" + thongTinPhienXetNghiem.BarCodeId.Substring(6) : thongTinPhienXetNghiem.BarCodeId,

                };

                if(thongTinPhienXetNghiem.HopDongKhamSucKhoeNhanVienId != null)
                {
                    var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepo.TableNoTracking
                        .Where(o => o.Id == thongTinPhienXetNghiem.HopDongKhamSucKhoeNhanVienId)
                        .Select(o => new
                        {
                            o.Id,
                            o.STTNhanVien,
                            TenCty = o.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten
                        }).First();

                    data.DiaChi = hopDongKhamSucKhoeNhanVien.TenCty;
                    data.STT = hopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{hopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
                }
                else
                {
                    data.DiaChi = thongTinPhienXetNghiem.DiaChiDayDu;
                    data.STT = string.Empty;
                }

                //if (phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null)
                //{
                //    data.STT = phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
                //}
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(ketQuaXetNghiemPhieuIn.Id.ToString())
                    ? BarcodeHelper.GenerateBarCode(ketQuaXetNghiemPhieuIn.Id.ToString())
                    : "";

                List<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiets = phienXetNghiemChiTietDatas
                    .Where(o=>o.NhanVienChiDinhId == thongTin.BacSiChiDinhId)
                    .SelectMany(p => p.KetQuaXetNghiemChiTiets).ToList();

                //List<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiets = phienXetNghiemChiTietEntities
                //    .SelectMany(p => p.KetQuaXetNghiemChiTiets)
                //    .Where(p => p.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId).ToList();


                var listGhiChuTheoDVXNISOs = new List<KetQuaXetNghiemChiTiet>();
                var listGhiChuTheoDVChuyenGois = new List<KetQuaXetNghiemChiTiet>();



                var lstYeuCauDichVuKyThuatIdTrongPhieu = new List<long>();
                if (ketQuaXetNghiemChiTiets.Any())
                {
                    if (ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds.Any())
                    {
                        if (ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds.Any())
                        {                            
                        }
                    }
                    else
                    {
                        List<string> totalTenNhom = new List<string>();
                        if (ketQuaXetNghiemPhieuIn.LoaiIn == 3) // in theo dịch vụ
                        {
                            foreach (var item in ketQuaXetNghiemPhieuIn.ListIn.Where(d => !listYeuCauDichVuKyThuatThuocNhomSarsCov2s.Result.Contains(d.YeuCauDichVuKyThuatId)).ToList())
                            {
                                var queryItemNhom = ketQuaXetNghiemChiTiets
                                    .Where(s => s.Id == item.Id &&
                                                s.YeuCauDichVuKyThuatId == item.YeuCauDichVuKyThuatId &&
                                                !totalTenNhom.Contains(nhomDichVuBenhViens.First(n=>n.Id == s.NhomDichVuBenhVienId).Ten))
                                                //&& s.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId)
                                    .Select(p => nhomDichVuBenhViens.First(n => n.Id == p.NhomDichVuBenhVienId).Ten).Distinct().OrderBy(o => o).ToList();
                                totalTenNhom.AddRange(queryItemNhom);
                            }
                        }

                        var info = string.Empty;
                        var STT = 1;



                        foreach (var tenNhom in totalTenNhom.Distinct())
                        {
                            var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                             + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b>" +
                                             tenNhom.ToUpper()
                                             + "</b></tr>";
                            info += headerNhom;

                            var queryNhom = new List<KetQuaXetNghiemChiTiet>();
                            if (ketQuaXetNghiemPhieuIn.LoaiIn == 1 || ketQuaXetNghiemPhieuIn.LoaiIn == 2)
                            {                                
                            }
                            else
                            {

                                var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets
                                    .Where(p => nhomDichVuBenhViens.First(n => n.Id == p.NhomDichVuBenhVienId).Ten == tenNhom)
                                    .Select(p => p.YeuCauDichVuKyThuatId)
                                    .Distinct().ToList();
                                List<long> listId = new List<long>();
                                foreach (var item in lstYeuCauDichVuKyThuatId)
                                {
                                    foreach (var itemIn in ketQuaXetNghiemPhieuIn.ListIn)
                                    {
                                        if (item == itemIn.YeuCauDichVuKyThuatId)
                                        {
                                            var kiemTraDaCoChua = listDefault.Where(x => x == item);
                                            if (!kiemTraDaCoChua.Any())
                                            {
                                                listId.Add(item);
                                            }
                                        }
                                    }
                                }
                                foreach (var ycId in listId.Distinct().ToList())
                                {
                                    //if (!phienXetNghiemData.PhienXetNghiemChiTiets
                                    //    .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                    //    .Any()) continue;
                                    //var res = phienXetNghiemData.PhienXetNghiemChiTiets
                                    //    .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                    //    .ToList();

                                    var res = phienXetNghiemChiTietDatas
                                        .Where(p => p.YeuCauDichVuKyThuatId == ycId).LastOrDefault()?.KetQuaXetNghiemChiTiets.ToList();
                                    if (res != null && res.Any())
                                    {
                                        queryNhom.AddRange(res);
                                    }
                                    
                                }
                                lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(listId);
                            }
                            var queryNhoms = new List<KetQuaXetNghiemChiTiet>();
                            foreach (var dvcheck in ketQuaXetNghiemPhieuIn.ListIn)
                            {
                                queryNhoms.AddRange(queryNhom.Where(d => d.Id == dvcheck.Id).ToList());
                            }
                            //var theFist = false;

                            var dvXetNghiemChaIds = queryNhoms.Where(d => d.DichVuXetNghiemChaId == null)
                                .Select(d => d.DichVuXetNghiemId).ToList();


                            // BVHD-3901 
                            //ghiChu = string.Empty;

                            if (queryNhoms.Where(d => !dvXetNghiemChaIds.Contains(d.DichVuXetNghiemChaId.GetValueOrDefault())).ToList().Count() != 0)
                            {
                                var dvIns = queryNhoms.Where(d => !dvXetNghiemChaIds.Contains(d.DichVuXetNghiemChaId.GetValueOrDefault())).ToList();

                                var dvIsos = dvIns.Where(d => dichVuXetNghiems.FirstOrDefault(xn => xn.Id == d.DichVuXetNghiemId)?.LaChuanISO == true).ToList();

                                if (dvIsos.Any())
                                {
                                    listGhiChuTheoDVXNISOs.AddRange(dvIsos);
                                }

                                var dvChuyenGuis = dvIns.Where(d => thongTinDichVuKyThuatBenhViens.FirstOrDefault(kt=>kt.Id == d.DichVuKyThuatBenhVienId)?.DichVuChuyenGoi == true).ToList();

                                if (dvChuyenGuis.Any())
                                {
                                    listGhiChuTheoDVChuyenGois.AddRange(dvChuyenGuis);
                                }
                            }

                            foreach (var dataParent in queryNhoms.Where(d => d.DichVuXetNghiemChaId == null && !dvXetNghiemChaIds.Contains(d.DichVuXetNghiemChaId.GetValueOrDefault()))
                                .OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).Distinct())
                            {
                                info = info + GetPhieuInKetQuaDuyet("", STT,
                                           queryNhoms.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).Distinct()
                                               .ToList(), dataParent,dichVuXetNghiems,mayXetNghiems,tenDichVuKyThuatBenhViens);
                            }

                        }
                        data.DanhSach = info;
                    }
                }
                data.NguoiThucHien = nguoiThucHien;
                //data.NguoiThucHien = returnStringTen(phienXetNghiemData.NhanVienThucHien?.HocHamHocVi?.Ma, phienXetNghiemData.NhanVienThucHien?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemData.NhanVienThucHien?.User?.HoTen);



                //data.ChanDoan = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                //        (phienXetNghiemData.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                //            ? phienXetNghiemData.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                //                .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Where(p => p.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh.IcdchinhId != null)
                //                .Select(p => (p.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + p.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet)).Where(p => p != null && p != "-" && p != "").Distinct().LastOrDefault()
                //            : "") :
                //            (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                //            phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => (o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet))
                //                                                     .Where(p => p != null && p != "-" && p != "").LastOrDefault() :
                //            (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : ""));// ksk de trong



                //data.DienGiai = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                //             (phienXetNghiemData.YeuCauTiepNhan.YeuCauKhamBenhs.SelectMany(d => d.YeuCauDichVuKyThuats)
                //                .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Any(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                //               ? phienXetNghiemData.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                //                   .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id))
                //                   .Where(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                //                   .Select(p => p.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).ToList().Distinct().LastOrDefault()
                //               : "") :
                //               (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                //               phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).ToList().Distinct().LastOrDefault() :
                //               (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : "")); // ksk de trong

                if(thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                {
                    var yeuCauKhamBenhIds = phienXetNghiemChiTietDatas
                        .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId) && o.YeuCauKhamBenhId != null)
                        .Select(o => o.YeuCauKhamBenhId.Value)
                        .ToList();

                    var noiTruPhieuDieuTris = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(o => yeuCauKhamBenhIds.Contains(o.Id) && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.ChanDoanSoBoICDId != null)
                        .Select(o => new
                        {
                            o.Id,
                            ChanDoan = o.ChanDoanSoBoICDId != null ? (o.ChanDoanSoBoICD.Ma + "-" + o.ChanDoanSoBoICD.TenTiengViet) : "",
                            DienGiai = o.ChanDoanSoBoGhiChu
                        }).ToList();
                    data.ChanDoan = noiTruPhieuDieuTris.Where(p => p.ChanDoan != null && p.ChanDoan != "-" && p.ChanDoan != "").OrderBy(o => o.Id).LastOrDefault()?.ChanDoan;
                    data.DienGiai = noiTruPhieuDieuTris.OrderBy(o => o.Id).LastOrDefault()?.DienGiai;
                }
                else if (thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                {
                    var noiTruPhieuDieuTriIds = phienXetNghiemChiTietDatas
                        .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId) && o.NoiTruPhieuDieuTriId != null)
                        .Select(o => o.NoiTruPhieuDieuTriId.Value)
                        .ToList();

                    var noiTruPhieuDieuTris = _noiTruPhieuDieuTriRepo.TableNoTracking
                        .Where(o => noiTruPhieuDieuTriIds.Contains(o.Id))
                        .Select(o => new
                        {
                            o.Id,
                            ChanDoan = o.ChanDoanChinhICDId != null ? (o.ChanDoanChinhICD.Ma + "-" + o.ChanDoanChinhICD.TenTiengViet) : "",
                            DienGiai = o.ChanDoanChinhGhiChu
                        }).ToList();
                    data.ChanDoan = noiTruPhieuDieuTris.Where(p => p.ChanDoan != null && p.ChanDoan != "-" && p.ChanDoan != "").OrderBy(o=>o.Id).LastOrDefault()?.ChanDoan;
                    data.DienGiai = noiTruPhieuDieuTris.OrderBy(o => o.Id).LastOrDefault()?.DienGiai;
                }
                else if (thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                {
                    data.ChanDoan = "";
                    data.DienGiai = "";
                }

                //var enumLoaiMauXetNghiems = phienXetNghiemData.PhienXetNghiemChiTiets
                //    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                //    .Select(o => o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem).Distinct();
                var enumLoaiMauXetNghiems = phienXetNghiemChiTietDatas
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                    .Select(o => o.LoaiMauXetNghiem).Distinct();

                
                data.LoaiMau = string.Join("; ", enumLoaiMauXetNghiems.Select(o => o.GetDescription()));

                //var phienXetNghiemChiTietLast = phienXetNghiemData.PhienXetNghiemChiTiets
                //    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();

                var phienXetNghiemChiTietLast = phienXetNghiemChiTietDatas
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();

                if (phienXetNghiemChiTietLast != null)
                {
                    var nhanViens = _nhanVienRepo.TableNoTracking
                    .Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienNhanMauId || o.Id == phienXetNghiemChiTietLast.NhanVienLayMauId)
                    .Select(o => new
                    {
                        o.Id,
                        HocHamHocVi = o.HocHamHocViId != null ? o.HocHamHocVi.Ma : "",
                        NhomChucDanh = o.ChucDanhId != null ? o.ChucDanh.NhomChucDanh.Ma : "",
                        o.User.HoTen
                    })
                    .ToList();

                    data.TgLayMau = phienXetNghiemChiTietLast.ThoiDiemLayMau?.ApplyFormatDateTimeSACH();
                    if(phienXetNghiemChiTietLast.NhanVienLayMauId != null)
                    {
                        var nguoiLayMau = nhanViens.Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienLayMauId).FirstOrDefault();
                        data.NguoiLayMau = returnStringTen(nguoiLayMau?.HocHamHocVi, nguoiLayMau?.NhomChucDanh, nguoiLayMau?.HoTen);
                    }
                    data.TgNhanMau = phienXetNghiemChiTietLast.ThoiDiemNhanMau?.ApplyFormatDateTimeSACH();

                    if (phienXetNghiemChiTietLast.NhanVienNhanMauId != null)
                    {
                        var nguoiNhanMau = nhanViens.Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienNhanMauId).FirstOrDefault();
                        data.NguoiNhanMau = returnStringTen(nguoiNhanMau?.HocHamHocVi, nguoiNhanMau?.NhomChucDanh, nguoiNhanMau?.HoTen);
                    }
                }
                if (phienXetNghiemChiTietLast != null)
                {
                    data.Ngay = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                    ngay = data.Ngay;
                    data.Thang = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                    thang = data.Thang;
                    data.Nam = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                    nam = data.Nam;
                    data.Gio = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";
                    gio = data.Gio;
                }
                data.NgayThangNamFooter = DateTime.Now.ApplyFormatDateTimeSACH();
                var lstContentItem = new PhieuInXetNghiemModel();
                // 
                if (!string.IsNullOrEmpty(data.DanhSach))
                {
                    if (listGhiChuTheoDVXNISOs.Count() != 0)
                    {
                        data.LogoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                        data.LogoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";
                    }

                    lstContentItem.Html = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, data);

                    #region BVHD-3919
                    var templatefootertableXetNghiem = _templateRepository.TableNoTracking.First(x => x.Name.Equals("FooterXetNghiemTable"));

                    data.Ngay = data.Ngay == null ? ngay : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                    data.Thang = data.Thang == null ? thang : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                    data.Nam = data.Nam == null ? nam : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                    data.Gio = data.Gio == null ? gio : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";

                    //Phần ghi chú sắp xếp như sau:
                    //Dòng đầu tiên là dòng ghi chú mặc định (footer template)
                    //Dòng thứ 2 là ghi chú ISO (ghiChu) 
                    //Dòng thứ 3 là ghi chú Dịch vụ gửi ngoài (ghiChu) 
                    //Dòng thứ 4 là dòng ghi chú của dịch vụ  (Ghi chú DV)

                    var ghiChu = string.Empty;
                    var logoBV1 = string.Empty;
                    var logoBV2 = string.Empty;



                    if (listGhiChuTheoDVXNISOs.Count() != 0)
                    {

                        if (!string.IsNullOrEmpty(ghiChu))
                        {
                            ghiChu += "<br>";
                        }
                        //update 16/05: - Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi  THAY ĐỔI THÀNH  - Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.
                        //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";
                        ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";

                        logoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                        logoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";

                        data.LogoBV1 = logoBV1;
                        data.LogoBV2 = logoBV2;
                    }

                    if (listGhiChuTheoDVChuyenGois.Count() != 0)
                    {
                        if (!string.IsNullOrEmpty(ghiChu))
                        {
                            ghiChu += "<br>";
                        }
                        //update 16/05: - Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012 THAY ĐỔI THÀNH - Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012
                        //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi.</span>";
                        ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.</span>";
                    }

                    data.GhiChu = (!string.IsNullOrEmpty(ghiChu) ? ghiChu + "<br>" : "") + (!string.IsNullOrEmpty(data.GhiChuDV) ? "- " + data.GhiChuDV : "");


                    var htmlfooter = TemplateHelpper.FormatTemplateWithContentTemplate(templatefootertableXetNghiem.Body, data);
                    lstContentItem.Html += htmlfooter;
                    lstContentItem.Html += "<div style='margin-left: 2cm;'><table width='100%'><tr><td style='width:36%'></td><td style='width:35%'></td><td style='width:29%'><b>" + data.NguoiThucHien + "</b></td>";
                    lstContentItem.Html += "</tr></table></div>";

                    #endregion BVHD-3919

                    lstContent.Add(lstContentItem);
                }                
                numberPage++;
            }

            return lstContent;
        }

        public async Task<List<PhieuInXetNghiemModel>> InDuyetKetQuaXetNghiemManHinhDuyetOld(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn)
        {
            var lstContent = new List<PhieuInXetNghiemModel>();

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKetQuaXetNghiemNew"));
            List<long> listDefault = new List<long>();
            var phienXetNghiemData = BaseRepository.Table
                .Include(p => p.BenhNhan)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienLayMau).ThenInclude(q => q.User)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.NhanVienNhanMau).ThenInclude(q => q.User)
                .Include(q => q.MauXetNghiems).ThenInclude(q => q.PhieuGoiMauXetNghiem).ThenInclude(q => q.NhanVienGoiMau).ThenInclude(q => q.User)
                .Include(q => q.NhanVienKetLuan).ThenInclude(q => q.User)
                .Include(q => q.NhanVienThucHien).ThenInclude(q => q.User)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.TinhThanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)
                .Include(p => p.PhienXetNghiemChiTiets)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuKyThuatBenhVien)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.MayXetNghiem)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NoiChiDinh)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauTiepNhan)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NoiChiDinh)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuXetNghiem).ThenInclude(p => p.HDPP)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh)
                //.Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)                
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.User)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.HocHamHocVi)
                //.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.HocHamHocVi)
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.HocHamHocVi)
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.YeuCauTiepNhanTheBHYTs)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(p => p.HopDongKhamSucKhoeNhanVien).ThenInclude(p => p.HopDongKhamSucKhoe).ThenInclude(p => p.CongTyKhamSucKhoe)                
                .FirstOrDefault(q => q.Id == ketQuaXetNghiemPhieuIn.Id);

            if(phienXetNghiemData != null)
            {
                //Explicit loading
                var phienXetNghiemChiTiets = BaseRepository.Context.Entry(phienXetNghiemData).Collection(o => o.PhienXetNghiemChiTiets);
                phienXetNghiemChiTiets.Query()
                    .Include(x => x.KetQuaXetNghiemChiTiets)
                    .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh)
                    .Include(p => p.NhomDichVuBenhVien)
                    .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
                    .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien)
                    .Include(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
                    .Include(x => x.NhanVienLayMau).ThenInclude(x => x.User)
                    .Include(x => x.NhanVienNhanMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                    .Include(x => x.NhanVienLayMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                    .Include(x => x.NhanVienNhanMau).ThenInclude(x => x.HocHamHocVi)
                    .Include(x => x.NhanVienLayMau).ThenInclude(x => x.HocHamHocVi)
                    .Load();

                foreach (var phienXetNghiemChiTiet in phienXetNghiemData.PhienXetNghiemChiTiets)
                {
                    var ketQuaXetNghiemChiTiets = BaseRepository.Context.Entry(phienXetNghiemChiTiet).Collection(o => o.KetQuaXetNghiemChiTiets);
                    ketQuaXetNghiemChiTiets.Query()
                        .Include(p => p.DichVuKyThuatBenhVien)
                        .Include(x => x.DichVuXetNghiem)
                        .Include(p => p.MayXetNghiem)
                        .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NoiChiDinh)
                        .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                        .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauTiepNhan)
                        .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NoiChiDinh)
                        .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                        .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuKyThuat)
                        .Include(p => p.NhomDichVuBenhVien)
                        .Include(p => p.DichVuXetNghiem).ThenInclude(p => p.HDPP)
                        .Load();
                }
            }

            var phienXetNghiemChiTietEntities = phienXetNghiemData.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).ToList();

            var thongTinBacSi = phienXetNghiemData.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                .Where(cc => cc.YeuCauKhamBenh != null || cc.NoiTruPhieuDieuTriId != null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    //YeuCauKhamBenhId = q.Id,
                    BacSiChiDinh =  q.NhanVienChiDinh?.User?.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinh?.Ten
                }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinLeTan = phienXetNghiemData.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                .Where(cc => cc.YeuCauKhamBenh == null && cc.NoiTruPhieuDieuTriId == null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    //YeuCauKhamBenhId = q.Id,
                    // BacSiChiDinh = "Lễ tân",
                    BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinh?.Ten,
                    FromLeTan = true,
                }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinPhieuIns = thongTinBacSi;
            thongTinPhieuIns.AddRange(thongTinLeTan);
            var numberPage = 1;

            string ngay = DateTime.Now.Day.ToString();
            string thang = DateTime.Now.Month.ToString();
            string nam = DateTime.Now.Year.ToString();
            string gio = DateTime.Now.Hour + " giờ " + DateTime.Now.Minute + " phút";

            var listYeuCauDichVuKyThuatThuocNhomSarsCov2s = GetListYeuCauTrongNhomSars(ketQuaXetNghiemPhieuIn.ListIn);

            var thongTinPhieuInTheoNhanVienChiDinhs = thongTinPhieuIns.GroupBy(d => d.BacSiChiDinhId)
                                                                      .Select(d => new ThongTinBacSiVo
                                                                      {
                                                                          BacSiChiDinh = d.First().BacSiChiDinh,
                                                                          BacSiChiDinhId = d.First().BacSiChiDinhId,
                                                                          KhoaPhongChiDinh =d.First().KhoaPhongChiDinh // lấy first
                                                                      })
                                                                      .ToList();

            foreach (var thongTin in thongTinPhieuInTheoNhanVienChiDinhs)
            {


                var data = new DuyetKetQuaXetNghiemPhieuInResultVo
                {
                    SoPhieu = phienXetNghiemData.MauXetNghiems.LastOrDefault(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
                  
                    SoVaoVien = phienXetNghiemData.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    LogoUrl = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha-full.png",
                    MaYTe = phienXetNghiemData.BenhNhan.MaBN,
                    
                    HoTen = phienXetNghiemData.YeuCauTiepNhan.HoTen,
                    DiaChi = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoe?.CongTyKhamSucKhoe?.Ten : phienXetNghiemData.YeuCauTiepNhan.DiaChiDayDu,
                    GhiChuDV = phienXetNghiemData.GhiChu,
                    NamSinh = phienXetNghiemData.YeuCauTiepNhan.NamSinh,
                    NamSinhString = DateHelper.DOBFormat(phienXetNghiemData.YeuCauTiepNhan.NgaySinh, phienXetNghiemData.YeuCauTiepNhan.ThangSinh, phienXetNghiemData.YeuCauTiepNhan.NamSinh),
                    GioiTinh = phienXetNghiemData.YeuCauTiepNhan.GioiTinh != null ? phienXetNghiemData.YeuCauTiepNhan.GioiTinh : null,
                   
                    DoiTuong = phienXetNghiemData.YeuCauTiepNhan.CoBHYT == true ? "BHYT (" + phienXetNghiemData.YeuCauTiepNhan.BHYTMucHuong + "%)" : "Viện phí",

                    MaBHYT = phienXetNghiemData.YeuCauTiepNhan?.BHYTMaSoThe,
                    
                    BsChiDinh = returnStringTenTheoNhanVien(thongTin.BacSiChiDinhId),
                    KhoaPhong = thongTin.KhoaPhongChiDinh,
                    Barcode = phienXetNghiemData.BarCodeId.Length == 10 ? phienXetNghiemData.BarCodeId.Substring(0, 6) + "-" + phienXetNghiemData.BarCodeId.Substring(6) : phienXetNghiemData.BarCodeId,

                };
                if (phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null)
                {
                    data.STT = phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
                }
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(ketQuaXetNghiemPhieuIn.Id.ToString())
                    ? BarcodeHelper.GenerateBarCode(ketQuaXetNghiemPhieuIn.Id.ToString())
                    : "";

                List<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiets = phienXetNghiemChiTietEntities
                    .SelectMany(p => p.KetQuaXetNghiemChiTiets)
                    .Where(p => p.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId).ToList();

               
                var listGhiChuTheoDVXNISOs = new List<KetQuaXetNghiemChiTiet>();
                var listGhiChuTheoDVChuyenGois = new List<KetQuaXetNghiemChiTiet>();

              

                var lstYeuCauDichVuKyThuatIdTrongPhieu = new List<long>();
                if (ketQuaXetNghiemChiTiets.Any())
                {
                    if (ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds.Any())
                    {
                        if (ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds.Any())
                        {
                            // k dùng nữa
                            //var info = string.Empty;
                            //foreach (var nhomDichVuBvId in ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds)
                            //{
                            //    var totalTenNhom = ketQuaXetNghiemChiTiets
                            //        .Where(p => p.NhomDichVuBenhVienId == nhomDichVuBvId)
                            //        .Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();
                            //    var STT = 1;
                            //    foreach (var tenNhom in totalTenNhom)
                            //    {
                            //        var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                            //                         + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b>" +
                            //                         tenNhom.ToUpper()
                            //                         + "</b></tr>";
                            //        info += headerNhom;
                            //        //var queryNhom = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom).ToList();

                            //        var queryNhom = new List<KetQuaXetNghiemChiTiet>();

                            //        var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets
                            //            .Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                            //            .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId)
                            //            .Distinct().ToList();

                            //        foreach (var ycId in lstYeuCauDichVuKyThuatId)
                            //        {
                            //            if (!phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                            //            var res = phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                            //            queryNhom.AddRange(res);
                            //        }
                            //        lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);
                            //        //var theFist = false;
                            //        foreach (var dataParent in queryNhom.Where(p => p.DichVuXetNghiemChaId == null)
                            //            .OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                            //        {
                            //            info = info + GetPhieuInKetQuaDuyet("", STT,
                            //                       queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId)
                            //                           .ToList(), dataParent);
                            //        }
                            //    }
                            //}
                            //data.DanhSach = info;
                        }
                    }
                    else
                    {
                        List<string> totalTenNhom = new List<string>();
                        // k dùng nữa
                        //if (ketQuaXetNghiemPhieuIn.LoaiIn == 1) // in tất cả
                        //{
                        //    totalTenNhom = ketQuaXetNghiemChiTiets.Select(p => p.NhomDichVuBenhVien.Ten).Distinct()
                        //        .OrderBy(o => o).ToList();
                        //}
                        //else if (ketQuaXetNghiemPhieuIn.LoaiIn == 2) // in theo nhóm
                        //{
                        //    foreach (var item in ketQuaXetNghiemPhieuIn.ListIn)
                        //    {
                        //        var queryItemNhom = ketQuaXetNghiemChiTiets
                        //            .Where(s => s.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId)
                        //            .Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();
                        //        totalTenNhom.AddRange(queryItemNhom);
                        //    }
                        //}
                        //else
                        if (ketQuaXetNghiemPhieuIn.LoaiIn == 3) // in theo dịch vụ
                        {
                            foreach (var item in ketQuaXetNghiemPhieuIn.ListIn.Where(d=> !listYeuCauDichVuKyThuatThuocNhomSarsCov2s.Result.Contains(d.YeuCauDichVuKyThuatId)).ToList())
                            {
                                var queryItemNhom = ketQuaXetNghiemChiTiets
                                    .Where(s => s.Id == item.Id &&
                                                s.YeuCauDichVuKyThuatId == item.YeuCauDichVuKyThuatId &&
                                                !totalTenNhom.Contains(s.NhomDichVuBenhVien.Ten) 
                                                &&  s.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId)
                                    .Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();
                                totalTenNhom.AddRange(queryItemNhom);
                            }
                        }
                        //var totalTenNhom = ketQuaXetNghiemChiTiets.Select(p => p.NhomDichVuBenhVien.Ten).Distinct().OrderBy(o => o).ToList();

                        var info = string.Empty;
                        var STT = 1;

                      

                        foreach (var tenNhom in totalTenNhom.Distinct())
                        {
                            var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                             + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b>" +
                                             tenNhom.ToUpper()
                                             + "</b></tr>";
                            info += headerNhom;
                            //var queryNhom = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom).ToList();

                            var queryNhom = new List<KetQuaXetNghiemChiTiet>();
                            if (ketQuaXetNghiemPhieuIn.LoaiIn == 1 || ketQuaXetNghiemPhieuIn.LoaiIn == 2)
                            {
                                // k dùng nữa
                                //var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets
                                //    .Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                                //    .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId)
                                //    .Distinct().ToList();
                                //foreach (var ycId in lstYeuCauDichVuKyThuatId)
                                //{
                                //    if (!phienXetNghiemData.PhienXetNghiemChiTiets
                                //        .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                //        .Any()) continue;
                                //    var res = phienXetNghiemData.PhienXetNghiemChiTiets
                                //        .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                //        .ToList();
                                //    queryNhom.AddRange(res);
                                //}
                                //lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);
                            }
                            else
                            {

                                var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets
                                    .Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                                    .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId)
                                    .Distinct().ToList();
                                List<long> listId = new List<long>();
                                foreach (var item in lstYeuCauDichVuKyThuatId)
                                {
                                    foreach (var itemIn in ketQuaXetNghiemPhieuIn.ListIn)
                                    {
                                        if (item == itemIn.YeuCauDichVuKyThuatId)
                                        {
                                            var kiemTraDaCoChua = listDefault.Where(x => x == item);
                                            if (!kiemTraDaCoChua.Any())
                                            {
                                                listId.Add(item);
                                            }
                                        }
                                    }
                                }
                                foreach (var ycId in listId.Distinct().ToList())
                                {
                                    if (!phienXetNghiemData.PhienXetNghiemChiTiets
                                        .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                        .Any()) continue;
                                    var res = phienXetNghiemData.PhienXetNghiemChiTiets
                                        .Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets
                                        .ToList();
                                    queryNhom.AddRange(res);
                                }
                                lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(listId);
                            }
                            var queryNhoms = new List<KetQuaXetNghiemChiTiet>();
                            foreach (var dvcheck in ketQuaXetNghiemPhieuIn.ListIn)
                            {
                                queryNhoms.AddRange(queryNhom.Where(d => d.Id == dvcheck.Id).ToList());
                            }
                            //var theFist = false;

                            var dvXetNghiemChaIds = queryNhoms.Where(d => d.DichVuXetNghiemChaId == null)
                                .Select(d => d.DichVuXetNghiemId).ToList();


                            // BVHD-3901 
                            //ghiChu = string.Empty;

                            if (queryNhoms.Where(d => !dvXetNghiemChaIds.Contains(d.DichVuXetNghiemChaId.GetValueOrDefault())).ToList().Count() != 0)
                            {
                                var dvIns = queryNhoms.Where(d => !dvXetNghiemChaIds.Contains(d.DichVuXetNghiemChaId.GetValueOrDefault())).ToList();
                                var chiSoXNISO = dvIns.Where(d => d.DichVuXetNghiem?.LaChuanISO == true).ToList().Count() != 0 ? true : false;


                                if(chiSoXNISO == true)
                                {
                                    listGhiChuTheoDVXNISOs.AddRange(dvIns.Where(d => d.DichVuXetNghiem?.LaChuanISO == true).ToList());
                                }

                                var chiSoXNChuyenGui = dvIns.Where(d => d.DichVuKyThuatBenhVien?.DichVuChuyenGoi == true).ToList().Count() != 0 ? true : false;

                                if (chiSoXNChuyenGui == true)
                                {
                                    listGhiChuTheoDVChuyenGois.AddRange(dvIns.Where(d => d.DichVuKyThuatBenhVien?.DichVuChuyenGoi == true).ToList());
                                }

                            }
                            
                            foreach (var dataParent in queryNhoms.Where(d=> d.DichVuXetNghiemChaId == null && !dvXetNghiemChaIds.Contains(d.DichVuXetNghiemChaId.GetValueOrDefault()))
                                .OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).Distinct())
                            {
                                info = info + GetPhieuInKetQuaDuyetOld("", STT,
                                           queryNhoms.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).Distinct()
                                               .ToList(), dataParent);
                            }

                        }
                        data.DanhSach = info;
                    }
                }

                data.NguoiThucHien = returnStringTen(phienXetNghiemData.NhanVienThucHien?.HocHamHocVi?.Ma, phienXetNghiemData.NhanVienThucHien?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemData.NhanVienThucHien?.User?.HoTen);



                data.ChanDoan = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                        (phienXetNghiemData.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                            ? phienXetNghiemData.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                                .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Where(p => p.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh.IcdchinhId != null)
                                .Select(p => (p.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + p.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet)).Where(p => p != null && p != "-" && p != "").Distinct().LastOrDefault()
                            : "") :
                            (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                            phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => (o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.Ma + "-" + o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhICD?.TenTiengViet))
                                                                     .Where(p => p != null && p != "-" && p != "").LastOrDefault() :
                            (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : ""));// ksk de trong



                data.DienGiai = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                             (phienXetNghiemData.YeuCauTiepNhan.YeuCauKhamBenhs.SelectMany(d => d.YeuCauDichVuKyThuats)
                                .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id)).Any(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                               ? phienXetNghiemData.YeuCauTiepNhan?.YeuCauKhamBenhs?.SelectMany(d => d.YeuCauDichVuKyThuats)
                                   .Where(d => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(d.Id))
                                   .Where(p => p.YeuCauKhamBenh?.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauKhamBenh?.IcdchinhId != null)
                                   .Select(p => p.YeuCauKhamBenh?.ChanDoanSoBoGhiChu).ToList().Distinct().LastOrDefault()
                               : "") :
                               (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                               phienXetNghiemData.PhienXetNghiemChiTiets.Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).Select(o => o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).ToList().Distinct().LastOrDefault() :
                               (phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "" : "")); // ksk de trong


                var enumLoaiMauXetNghiems = phienXetNghiemData.PhienXetNghiemChiTiets
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                    .Select(o => o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem).Distinct();

                //data.ChanDoan = string.Join("; ", chanDoans.Where(o => !string.IsNullOrEmpty(o)));
                data.LoaiMau = string.Join("; ", enumLoaiMauXetNghiems.Select(o => o.GetDescription()));

                var phienXetNghiemChiTietLast = phienXetNghiemData.PhienXetNghiemChiTiets
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();
                if (phienXetNghiemChiTietLast != null)
                {
                    data.TgLayMau = phienXetNghiemChiTietLast.ThoiDiemLayMau?.ApplyFormatDateTimeSACH();

                    data.NguoiLayMau = returnStringTen(phienXetNghiemChiTietLast.NhanVienLayMau?.HocHamHocVi?.Ma, phienXetNghiemChiTietLast.NhanVienLayMau?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemChiTietLast.NhanVienLayMau?.User?.HoTen);

                    data.TgNhanMau = phienXetNghiemChiTietLast.ThoiDiemNhanMau?.ApplyFormatDateTimeSACH();

                    data.NguoiNhanMau = returnStringTen(phienXetNghiemChiTietLast.NhanVienNhanMau?.HocHamHocVi?.Ma, phienXetNghiemChiTietLast.NhanVienNhanMau?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemChiTietLast.NhanVienNhanMau?.User?.HoTen);
                }
                if (phienXetNghiemChiTietLast != null)
                {
                    data.Ngay = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                    ngay = data.Ngay;
                    data.Thang = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                    thang = data.Thang;
                    data.Nam = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                    nam = data.Nam;
                    data.Gio = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";
                    gio = data.Gio;
                }
                data.NgayThangNamFooter = DateTime.Now.ApplyFormatDateTimeSACH();
                var lstContentItem = new PhieuInXetNghiemModel();
                // 
                if (!string.IsNullOrEmpty(data.DanhSach))
                {
                    if (listGhiChuTheoDVXNISOs.Count() != 0)
                    {
                        data.LogoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                        data.LogoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";
                    }

                    lstContentItem.Html = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, data);

                    #region BVHD-3919
                    var templatefootertableXetNghiem = _templateRepository.TableNoTracking.First(x => x.Name.Equals("FooterXetNghiemTable"));

                    data.Ngay = data.Ngay == null ? ngay : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                    data.Thang = data.Thang == null ? thang : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                    data.Nam = data.Nam == null ? nam : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                    data.Gio = data.Gio == null ? gio : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";

                    //Phần ghi chú sắp xếp như sau:
                    //Dòng đầu tiên là dòng ghi chú mặc định (footer template)
                    //Dòng thứ 2 là ghi chú ISO (ghiChu) 
                    //Dòng thứ 3 là ghi chú Dịch vụ gửi ngoài (ghiChu) 
                    //Dòng thứ 4 là dòng ghi chú của dịch vụ  (Ghi chú DV)

                    var ghiChu = string.Empty;
                    var logoBV1 = string.Empty;
                    var logoBV2 = string.Empty;



                    if (listGhiChuTheoDVXNISOs.Count() != 0)
                    {
                     
                        if (!string.IsNullOrEmpty(ghiChu))
                        {
                            ghiChu += "<br>";
                        }
                        //update 16/05: - Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi  THAY ĐỔI THÀNH  - Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.
                        //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";
                        ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";

                        logoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                        logoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";

                        data.LogoBV1 = logoBV1;
                        data.LogoBV2 = logoBV2;
                    }

                    if (listGhiChuTheoDVChuyenGois.Count() != 0)
                    {
                        if (!string.IsNullOrEmpty(ghiChu))
                        {
                            ghiChu += "<br>";
                        }
                        //update 16/05: - Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012 THAY ĐỔI THÀNH - Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012
                        //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi.</span>";
                        ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.</span>";
                    }




                    data.GhiChu = (!string.IsNullOrEmpty(ghiChu) ? ghiChu + "<br>" : "") + (!string.IsNullOrEmpty(data.GhiChuDV) ? "- " + data.GhiChuDV : "");


                    var htmlfooter = TemplateHelpper.FormatTemplateWithContentTemplate(templatefootertableXetNghiem.Body, data);
                    lstContentItem.Html += htmlfooter;
                    lstContentItem.Html += "<div style='margin-left: 2cm;'><table width='100%'><tr><td style='width:36%'></td><td style='width:35%'></td><td style='width:29%'><b>" + data.NguoiThucHien + "</b></td>";
                    lstContentItem.Html += "</tr></table></div>";

                    #endregion BVHD-3919

                    lstContent.Add(lstContentItem);
                }
                //if (lstContent.Any() && numberPage == thongTinPhieuIns.Count())
                //{
                //    var templatefootertableXetNghiem = _templateRepository.TableNoTracking.First(x => x.Name.Equals("FooterXetNghiemTable"));

                //    data.Ngay = data.Ngay == null ? ngay : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                //    data.Thang = data.Thang == null ? thang : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                //    data.Nam = data.Nam == null ? nam : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                //    data.Gio = data.Gio == null ? gio : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";

                //    //Phần ghi chú sắp xếp như sau:
                //    //Dòng đầu tiên là dòng ghi chú mặc định (footer template)
                //    //Dòng thứ 2 là ghi chú ISO (ghiChu) 
                //    //Dòng thứ 3 là ghi chú Dịch vụ gửi ngoài (ghiChu) 
                //    //Dòng thứ 4 là dòng ghi chú của dịch vụ  (Ghi chú DV)

                //    data.GhiChu =  (!string.IsNullOrEmpty(ghiChu) ? ghiChu  + "<br>" : "") + (!string.IsNullOrEmpty(data.GhiChuDV) ? "- " + data.GhiChuDV : "");


                //    var htmlfooter = TemplateHelpper.FormatTemplateWithContentTemplate(templatefootertableXetNghiem.Body, data);
                //    lstContent.LastOrDefault().Html += htmlfooter;
                //    lstContent.LastOrDefault().Html += "<div style='margin-left: 2cm;'><table width='100%'><tr><td style='width:36%'></td><td style='width:35%'></td><td style='width:29%'><b>" + data.NguoiThucHien + "</b></td>";
                //    lstContent.LastOrDefault().Html += "</tr></table></div>";
                //}
                numberPage++;
            }

            return lstContent;
        }

        private bool KiemTraDichVuXetNghiemDichjVuCaphaiCoDichVuCapBa(KetQuaXetNghiemChiTiet queryIn, List<KetQuaXetNghiemChiTiet> queryNhom)
        {
            if (queryIn != null && queryNhom != null)
            {
                var lstChild = queryNhom.Where(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId && p.CapDichVu == 3).ToList();
                if (lstChild.Any())
                {
                    return true;
                }
            }
            return false;
        }
        private string GetPhieuInKetQuaDuyet(string info, int stt, List<KetQuaXetNghiemChiTiet> queryNhom, KetQuaXetNghiemChiTiet queryIn,List<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> dichVuXetNghiems, List<MayXetNghiem> mayXetNghiems, List<LookupItemVo> tenDichVus, bool isLev2 = false)
        {

            if (queryIn == null) return info;

            var dichVuXetNghiemIn = dichVuXetNghiems.First(o => o.Id == queryIn.DichVuXetNghiemId);
            var tenDichVuKyThuatBenhVien = tenDichVus.FirstOrDefault(o => o.KeyId == queryIn.DichVuKyThuatBenhVienId)?.DisplayName;
            var mayXetNghiem = mayXetNghiems.FirstOrDefault(o => o.Id == queryIn.MayXetNghiemId);

            if (queryNhom.Any(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId) && queryIn.DichVuXetNghiemChaId == null) // xét nghiệm k có nhóm con
            {
                var toDam = queryIn.ToDamGiaTri;
                var isCapDichVu = dichVuXetNghiemIn.CapDichVu;
                var kq = !string.IsNullOrEmpty(queryIn.GiaTriDuyet) ? queryIn.GiaTriDuyet : !string.IsNullOrEmpty(queryIn.GiaTriNhapTay) ? queryIn.GiaTriNhapTay : !string.IsNullOrEmpty(queryIn.GiaTriTuMay) ? queryIn.GiaTriTuMay : string.Empty;
                var valueMin = queryIn.GiaTriMin;
                var valueMax = queryIn.GiaTriMax;
                if (string.IsNullOrEmpty(kq)) //dịch vụ cấp 1 ket qua = null => 1 dòng
                {
                    info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b style='margin-left: 10px;'>" +
                                tenDichVuKyThuatBenhVien
                                + "</b></td></tr>";
                }
                else //dịch vụ cấp 1 ket qua != null => in kết quả 
                {
                    info = info + "<tr style='border: 1px solid #020000;text-align: center; height:50px;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;font-weight: bold;'><p style='margin-left: 10px;'>" +
                                       (queryIn.CapDichVu == 1 ? tenDichVuKyThuatBenhVien : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (mayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (dichVuXetNghiemIn.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                }

            }
            else // xét nghiệm có nhóm con
            {
                //var dd = queryIn.
                var toDam = queryIn.ToDamGiaTri;
                var isCapDichVu = dichVuXetNghiemIn.CapDichVu;
                var kq = !string.IsNullOrEmpty(queryIn.GiaTriDuyet) ? queryIn.GiaTriDuyet : !string.IsNullOrEmpty(queryIn.GiaTriNhapTay) ? queryIn.GiaTriNhapTay : !string.IsNullOrEmpty(queryIn.GiaTriTuMay) ? queryIn.GiaTriTuMay : string.Empty;
                var valueMin = queryIn.GiaTriMin;
                var valueMax = queryIn.GiaTriMax;
                //Bước thực hiện: Danh sách duyệt kết quả - In kết quả xét nghiệm
                //Hiện tại: Những dịch vụ có cấp(Bố - Con - Cháu) khi không có kết quả nhập ở trường con thì in phiếu trả kết quả xét nghiệm bị chia ô như có kết quả.
                //Mong muốn: Dịch vụ mà có cấp bậc(Bố -Con - Cháu) không nhập kết quả ở trường con thì in ra sẽ meager thành 1 dòng, 
                //nếu mà có kết quả nhập ở trường kết quả ở trường con sẽ không mager lại.Chỉ áp dụng những dịch vụ có cấp bậc(Bố - Con - Cháu).không áp dụng vs những dịch vụ khác.
                bool ketquaNull = true;
                if (string.IsNullOrEmpty(kq) && KiemTraDichVuXetNghiemDichjVuCaphaiCoDichVuCapBa(queryIn, queryNhom) == true)
                {
                    ketquaNull = false;
                }


                if (isCapDichVu == 2)
                {  // lv2
                    bool capDichVuCoCap3 = false;
                    var index = dichVuXetNghiems.Where(s => s.DichVuXetNghiemChaId == (long)queryIn.DichVuXetNghiemId).ToList();
                    if (index.Any())
                    {
                        capDichVuCoCap3 = true;
                    }
                    if (ketquaNull == true)
                    {
                        info = info
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 20px;'>" +
                                        (capDichVuCoCap3 == true ? "<b>" + dichVuXetNghiemIn.Ten + "</b>" : dichVuXetNghiemIn.Ten)
                                       + "</p>" + "</td>"
                                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (mayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (dichVuXetNghiemIn.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                    }
                    else
                    {
                        info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b style='margin-left: 10px;'><p style='margin-left: 20px;'>" +
                                dichVuXetNghiemIn.Ten + "</p>"
                                + "</b></td></tr>";
                    }


                }
                else if (isCapDichVu == 3)
                {
                    info = info
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 30px;'>" +
                                        dichVuXetNghiemIn.Ten
                                       + "</p>" + "</td>"
                                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (mayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (dichVuXetNghiemIn.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                }
                else
                {
                    //lv1
                    string bienDinhNhomMau = "Định nhóm máu";
                    var kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau = tenDichVuKyThuatBenhVien.RemoveVietnameseDiacritics().Contains(bienDinhNhomMau.RemoveVietnameseDiacritics());
                    if (kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau == true)
                    {


                        info = info + "<tr style='border: 1px solid #020000;text-align: center; height:50px;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                                       (queryIn.CapDichVu == 1 ? tenDichVuKyThuatBenhVien : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (mayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (dichVuXetNghiemIn.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                    }
                    else
                    {
                        info = info + "<tr style='border: 1px solid #020000;text-align: center;'>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                                      (queryIn.CapDichVu == 1 ? tenDichVuKyThuatBenhVien : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                      + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (mayXetNghiem?.Ten ?? "") + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (dichVuXetNghiemIn.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                      + "</tr>";
                    }
                }



                stt++;
            }

            var lstChild = queryNhom.Where(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId).ToList();
            foreach (var item in lstChild)
            {
                info += GetPhieuInKetQuaDuyet("", stt, queryNhom, item, dichVuXetNghiems, mayXetNghiems, tenDichVus, true);
            }

            return info;
        }
        private string GetPhieuInKetQuaDuyetOld(string info, int stt, List<KetQuaXetNghiemChiTiet> queryNhom, KetQuaXetNghiemChiTiet queryIn, bool isLev2 = false)
        {

            if (queryIn == null) return info;

            if (queryNhom.Any(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId) && queryIn.DichVuXetNghiemChaId == null) // xét nghiệm k có nhóm con
            {
                var toDam = queryIn.ToDamGiaTri;
                var isCapDichVu = queryIn.DichVuXetNghiem?.CapDichVu;
                var kq = !string.IsNullOrEmpty(queryIn.GiaTriDuyet) ? queryIn.GiaTriDuyet : !string.IsNullOrEmpty(queryIn.GiaTriNhapTay) ? queryIn.GiaTriNhapTay : !string.IsNullOrEmpty(queryIn.GiaTriTuMay) ? queryIn.GiaTriTuMay : string.Empty;
                var valueMin = queryIn.GiaTriMin;
                var valueMax = queryIn.GiaTriMax;
                if (string.IsNullOrEmpty(kq)) //dịch vụ cấp 1 ket qua = null => 1 dòng
                {
                    info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b style='margin-left: 10px;'>" +
                                //queryIn.DichVuXetNghiem?.Ten
                                queryIn.DichVuXetNghiem?.DichVuKyThuatBenhViens?.Select(p => p.Ten).FirstOrDefault()
                                + "</b></td></tr>";
                }
                else //dịch vụ cấp 1 ket qua != null => in kết quả 
                {
                    info = info + "<tr style='border: 1px solid #020000;text-align: center; height:50px;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;font-weight: bold;'><p style='margin-left: 10px;'>" +
                                       (queryIn.CapDichVu == 1 ? queryIn.DichVuKyThuatBenhVien?.Ten : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                }

            }
            else // xét nghiệm có nhóm con
            {
                //var dd = queryIn.
                var toDam = queryIn.ToDamGiaTri;
                var isCapDichVu = queryIn.DichVuXetNghiem?.CapDichVu;
                var kq = !string.IsNullOrEmpty(queryIn.GiaTriDuyet) ? queryIn.GiaTriDuyet : !string.IsNullOrEmpty(queryIn.GiaTriNhapTay) ? queryIn.GiaTriNhapTay : !string.IsNullOrEmpty(queryIn.GiaTriTuMay) ? queryIn.GiaTriTuMay : string.Empty;
                var valueMin = queryIn.GiaTriMin;
                var valueMax = queryIn.GiaTriMax;
                //Bước thực hiện: Danh sách duyệt kết quả - In kết quả xét nghiệm
                //Hiện tại: Những dịch vụ có cấp(Bố - Con - Cháu) khi không có kết quả nhập ở trường con thì in phiếu trả kết quả xét nghiệm bị chia ô như có kết quả.
                //Mong muốn: Dịch vụ mà có cấp bậc(Bố -Con - Cháu) không nhập kết quả ở trường con thì in ra sẽ meager thành 1 dòng, 
                //nếu mà có kết quả nhập ở trường kết quả ở trường con sẽ không mager lại.Chỉ áp dụng những dịch vụ có cấp bậc(Bố - Con - Cháu).không áp dụng vs những dịch vụ khác.
                bool ketquaNull = true;
                if (string.IsNullOrEmpty(kq) && KiemTraDichVuXetNghiemDichjVuCaphaiCoDichVuCapBa(queryIn, queryNhom) == true)
                {
                    ketquaNull = false;
                }


                if (isCapDichVu == 2)
                {  // lv2
                    bool capDichVuCoCap3 = false;
                    var index = _dichVuXetNghiemRepository.TableNoTracking.Where(s => s.DichVuXetNghiemChaId == (long)queryIn.DichVuXetNghiemId).ToList();
                    if (index.Any())
                    {
                        capDichVuCoCap3 = true;
                    }
                    if (ketquaNull == true)
                    {
                        info = info
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 20px;'>" +
                                        (capDichVuCoCap3 == true ? "<b>" + queryIn.DichVuXetNghiem?.Ten + "</b>" : queryIn.DichVuXetNghiem?.Ten)
                                       + "</p>" + "</td>"
                                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                    }
                    else
                    {
                        info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b style='margin-left: 10px;'><p style='margin-left: 20px;'>" +
                                queryIn.DichVuXetNghiem?.Ten + "</p>"
                                + "</b></td></tr>";
                    }


                }
                else if (isCapDichVu == 3)
                {
                    info = info
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 30px;'>" +
                                        queryIn.DichVuXetNghiem?.Ten
                                       + "</p>" + "</td>"
                                       + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                }
                else
                {
                    //lv1
                    string bienDinhNhomMau = "Định nhóm máu";
                    var kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau = queryIn.DichVuKyThuatBenhVien?.Ten.RemoveVietnameseDiacritics().Contains(bienDinhNhomMau.RemoveVietnameseDiacritics());
                    if (kiemTraXetNghiemHuyetHocThuocNhomDinhNhoMau == true)
                    {


                        info = info + "<tr style='border: 1px solid #020000;text-align: center; height:50px;'>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                                       (queryIn.CapDichVu == 1 ? queryIn.DichVuKyThuatBenhVien?.Ten : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                       + "</tr>";
                    }
                    else
                    {
                        info = info + "<tr style='border: 1px solid #020000;text-align: center;'>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 10px;'>" +
                                      (queryIn.CapDichVu == 1 ? queryIn.DichVuKyThuatBenhVien?.Ten : queryIn.DichVuXetNghiemTen) + "</p>" + "</td>"
                                      + (toDam == true ? "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq + "</td>" :
                                                         "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq + "</td>")

                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                      + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax) + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: center;'>" + queryIn.DonVi + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.MayXetNghiem?.Ten ?? "") + "</td>"
                                      + "<td style = 'border: 1px solid #020000;text-align: left;'>" + (queryIn.DichVuXetNghiem?.HDPP?.Ten ?? "") + "</td>" // BVHD-3901
                                      + "</tr>";
                    }
                }



                stt++;
            }

            var lstChild = queryNhom.Where(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId).ToList();
            foreach (var item in lstChild)
            {
                info += GetPhieuInKetQuaDuyetOld("", stt, queryNhom, item, true);
            }

            return info;
        }
        private bool IsSo(string sVal)
        {
            double test;
            return double.TryParse(sVal, out test);
        }
        private string returnStringTen(string maHocHamHocVi, string maNhomChucDanh, string ten)
        {
            //[2:40 PM] Tram N. Pham
            //chỗ này show theo format: Mã học vị học hàm + dấu cách + Tên bác sĩ
            return (!string.IsNullOrEmpty(maHocHamHocVi) ? maHocHamHocVi + " " : "") + ten;
        }
        #endregion in những dịch vụ chỉ định được check trên grid
        #region BVHD-3761
        public List<PhieuInXetNghiemModel> InDiChXetNghiemTestNhanhKhangNguyenSarsCoV2(List<long> yeuCauDichVuKyThuatIds, long phienXetNghiemId, string hostingName)
        {

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKetQuaXetNghiemTestNhanhKhangNguyenSarsCov2"));

            List<long> listDefault = new List<long>();
            var models = new List<PhieuInXetNghiemModel>();

            var yeuCauTiepNhanId = BaseRepository.TableNoTracking
                .Where(q => q.Id == phienXetNghiemId)
                .Select(d => d.YeuCauTiepNhanId).FirstOrDefault();

            var infoBN = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(d => d.Id == yeuCauTiepNhanId).Select(d => new
                {
                    HoTen = d.HoTen,
                    NamSinh = DateHelper.DOBFormat(d.NgaySinh, d.ThangSinh, d.NamSinh),
                    GioiTinh = d.GioiTinh,
                    DiaChi = d.DiaChiDayDu,
                    SoTheBHYT = d.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                                 d.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                      .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                      .Select(a => a.MaSoThe.ToString()).FirstOrDefault()
                                 : d.BHYTMaSoThe,

                }).First();

            var phienXetNghiemData = BaseRepository.TableNoTracking
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.User)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.User)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.ChucDanh).ThenInclude(x => x.NhomChucDanh)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienNhanMau).ThenInclude(x => x.HocHamHocVi)
                .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhanVienLayMau).ThenInclude(x => x.HocHamHocVi)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuXetNghiem)//.ThenInclude(p => p.DichVuKyThuatBenhViens)
                .FirstOrDefault(q => q.Id == phienXetNghiemId);

            var thoiGianChiDinhDichVus = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                        .Where(d => yeuCauDichVuKyThuatIds.Contains(d.Id))
                                        .Select(d => new {
                                            YeuCauDichVuKyThuatId = d.Id,
                                            ThoiGianChiDinhDichVu = d.ThoiDiemChiDinh
                                        }).ToList();
                                        

            var newOBJ = new PhieuInXetNghiemTestCovidViewModel();

            newOBJ.HoTen = infoBN.HoTen;
            newOBJ.NamSinh = infoBN.NamSinh;
            newOBJ.GioiTinhString = infoBN.GioiTinh?.GetDescription();
            newOBJ.DiaChi = infoBN.DiaChi;
            newOBJ.SoTheBHYT = infoBN.SoTheBHYT;

            newOBJ.NoiLayMau = "Bệnh viện Đa khoa Quốc tế Bắc Hà";
            newOBJ.LoaiBenhPham = Enums.EnumLoaiMauXetNghiem.DichTyHau.GetDescription();
            newOBJ.LogoUrl = hostingName + "/assets/img/logo-bacha-full.png";

            foreach (var yeuCauDichVuKyThuatId in yeuCauDichVuKyThuatIds)
            {
                newOBJ.DanhSach = string.Empty;
                var content = string.Empty;
                var phienXetNghiemChiTietLast = phienXetNghiemData.PhienXetNghiemChiTiets
                    .Where(o =>o.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();

                if (phienXetNghiemChiTietLast != null)
                {
                    newOBJ.ThoiGianLayMau = phienXetNghiemChiTietLast.ThoiDiemNhanMau?.Hour + " giờ "
                                        + phienXetNghiemChiTietLast.ThoiDiemNhanMau?.Minute + " "
                                        + " ngày " + phienXetNghiemChiTietLast.ThoiDiemNhanMau?.Day
                                        + " tháng " + phienXetNghiemChiTietLast.ThoiDiemNhanMau?.Month
                                        + " năm " + phienXetNghiemChiTietLast.ThoiDiemNhanMau?.Year;

                    newOBJ.NguoiLayMau = returnStringTen(phienXetNghiemChiTietLast.NhanVienLayMau?.HocHamHocVi?.Ma, phienXetNghiemChiTietLast.NhanVienLayMau?.ChucDanh?.NhomChucDanh?.Ma, phienXetNghiemChiTietLast.NhanVienLayMau?.User?.HoTen);
                }
                else
                {
                    newOBJ.ThoiGianLayMau = "........." + " giờ "
                                        + "........." + " "
                                        + " ngày " + "........."
                                        + " tháng " + "........."
                                        + " năm " + "202";
                }
                var res = phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.ToList();
                foreach (var item in res.ToList())
                {
                    newOBJ.DanhSach += GetPhieuInKetQuaDuyetTestCovid(item);
                }
                #region ngay gio chi dinh dich vu
                
                var thoiDiemKetLuan = phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().ThoiDiemKetLuan;
               
                if (thoiDiemKetLuan != null)
                {
                    newOBJ.Gio = thoiDiemKetLuan?.Hour + "";

                    newOBJ.Phut = thoiDiemKetLuan?.Minute + "";
                    newOBJ.Ngay = thoiDiemKetLuan?.Day + "";
                    newOBJ.Thang = thoiDiemKetLuan?.Month + "";
                    newOBJ.Nam = thoiDiemKetLuan?.Year + "";
                }

                #endregion
                var model = new PhieuInXetNghiemModel();

                content = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, newOBJ);
                
                model.Html = content;
                models.Add(model);
            }
            
            return models;
        }
        private string GetPhieuInKetQuaDuyetTestCovid(KetQuaXetNghiemChiTiet queryIn)
        {
            var info = string.Empty;

            info = info + "<tr style='border: 1px solid #020000;text-align: center;'>"

                                 + "<td style='border: 1px solid #020000;text-align: center;height:130px;padding:5px;'>" +
                                 "XÉT NGHIỆM TEST <br> NHANH KHÁNG NGUYÊN  <br>SARS - CoV - 2"
                                 + "</td>"

                                  + "<td style='border: 1px solid #020000;text-align: center;height:130px;padding:5px;'>" +
                                 queryIn.YeuCauDichVuKyThuat?.LoaiKitThu
                                 + "</td>"

                                  + "<td style='border: 1px solid #020000;text-align: center;'>" 
                                  // để trống
                                 + "</td>"

                                 + "</tr>";
            return info;
        }

        public List<long> GetListPhienXetNghiemIdChoIn(long yeuCauTiepNhanId, long? phienXetNghiemId)
        {
            if (phienXetNghiemId != null)
            {
                return new List<long> { phienXetNghiemId.Value };
            }
            return BaseRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.PhienXetNghiemChiTiets.Any(ct => ct.NhanVienKetLuanId != null))
                .Select(o => o.Id).ToList();
        }

        public async Task<List<long>> GetListYeuCauTrongNhomSars(List<DuyetKqXetNghiemChiTietModel> listIn)
        {
            var yeuCauDichVuKyThuatIds = listIn.Select(d => d.YeuCauDichVuKyThuatId).ToList();

            var dichVuKyThuatBenhVienInfos = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(d => yeuCauDichVuKyThuatIds.Contains(d.Id))
                .Select(d => new { 
                    DichVuKyThuatBenhVienId = d.DichVuKyThuatBenhVienId,
                    YeuCauDichVuKyThuatId = d.Id
                }).ToList();

            var lstDichVuSarCoVs = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhTiepNhan.DichVuTestSarsCovid")
                .Select(d => d.Value).FirstOrDefault();

            var json = JsonConvert.DeserializeObject<List<DichVuKyThuatBenhVienIdsSarsCoV>>(lstDichVuSarCoVs);

            var nhomDichVuSarsCov2s=json.Select(d => d.DichVuKyThuatBenhVienId).ToList();

            return dichVuKyThuatBenhVienInfos.Where(d=> nhomDichVuSarsCov2s.Contains(d.DichVuKyThuatBenhVienId))
                .Select(d=>d.YeuCauDichVuKyThuatId).ToList();
        }
        public DichVuKyThuatBenhVienThuocXNGridVo YeuCauDichVuKyThuatIdTheoPhienXetNghiemSars(LayPhieuXetNghiemTheoYCKTVaNhomDVBVVo ketQuaXetNghiemPhieuIn)
        {
            var phienXetNghiemChiTiets = BaseRepository.TableNoTracking
                                   .Where(c => c.YeuCauTiepNhanId == ketQuaXetNghiemPhieuIn.YeuCauTiepNhanId)
                                   .Include(x => x.PhienXetNghiemChiTiets)
                                   .ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
                                   .SelectMany(d => d.PhienXetNghiemChiTiets);

            var phienXetNghiemInfoIds = phienXetNghiemChiTiets.Where(cc => cc.YeuCauDichVuKyThuat.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId &&
                                                                      cc.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == ketQuaXetNghiemPhieuIn.phieuDieuTriHienTaiId)
                                                          .Select(d => new DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPhamTheoPhienXetNghiem
                                                          { 
                                                              PhienXetNghiemId =d.PhienXetNghiemId,
                                                              DichVuKyThuatBenhVienId = d.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                                                              YeuCauDichVuKyThuatId =d.YeuCauDichVuKyThuatId
                                                          }).ToList();

            var lstDichVuSarCoVs = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhTiepNhan.DichVuTestSarsCovid")
                .Select(d => d.Value).FirstOrDefault();

            var json = JsonConvert.DeserializeObject<List<DichVuKyThuatBenhVienIdsSarsCoV>>(lstDichVuSarCoVs);

            var nhomDichVuSarsCov2s = json.Select(d => d.DichVuKyThuatBenhVienId).ToList();

            var xnNormal = phienXetNghiemInfoIds.Where(d => !nhomDichVuSarsCov2s.Contains(d.DichVuKyThuatBenhVienId)).ToList();
            var xnSarsCov2s = phienXetNghiemInfoIds.Where(d => nhomDichVuSarsCov2s.Contains(d.DichVuKyThuatBenhVienId)).ToList();
            
            var dichVuKyThuatBenhVienThuocXNGridVo = new DichVuKyThuatBenhVienThuocXNGridVo();

            dichVuKyThuatBenhVienThuocXNGridVo.XetNghiemThuocNhomSarsCov = xnSarsCov2s;

            dichVuKyThuatBenhVienThuocXNGridVo.XetNghiemKhongThuocNhomSarsCov = xnNormal;
            
            return dichVuKyThuatBenhVienThuocXNGridVo;
        }
        #endregion in những dịch vụ chỉ định được check trên grid
        #region
        public async Task<List<CauHinhDichVuTestSarsCovid>> CauHinhDichVuTestSarsCovids()
        {
            var cauHinhDichVuTestSarsCovid = await _cauHinhRepository.TableNoTracking.FirstAsync(x => x.Name == "CauHinhTiepNhan.DichVuTestSarsCovid");
            return JsonConvert.DeserializeObject<List<CauHinhDichVuTestSarsCovid>>(cauHinhDichVuTestSarsCovid.Value);
        }

        public async Task<List<LookupItemVo>> DichVuTestSarsCovids(DropDownListRequestModel model)
        {
            var cauHinhLoaiKitTestSarsCovid = await _cauHinhRepository.TableNoTracking.FirstAsync(x => x.Name == "CauHinhTiepNhan.LoaiKitTestSarsCovid");
            var cauHinhLoaiKitTestSars = JsonConvert.DeserializeObject<List<CauHinhLoaiKitTestSarsCovid>>(cauHinhLoaiKitTestSarsCovid.Value);
            var result = cauHinhLoaiKitTestSars.Select(s => new LookupItemVo
            {
                KeyId = s.LoaiKitThuId,
                DisplayName = s.LoaiKitThu
            }).ToList();
            if (!string.IsNullOrEmpty(model.Query))
            {
                result = result.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return result;
        }

        public async Task<string> GetNgayDuyetKetQuaCu(KetQuaXetNghiemVo ketQuaXetNghiemVo)
        {
            var yeuCauTiepNhan=_yeuCauTiepNhanRepository.GetById(ketQuaXetNghiemVo.YeuCauTiepNhanId);
            var benhNhanId = yeuCauTiepNhan?.BenhNhanId ?? 0;
            var ngayDuyetKetQuaXNChiTiets = await _ketQuaXetNghiemChiTietRepository.TableNoTracking
                                            .Where(c => c.DaDuyet == true
                                            && c.PhienXetNghiemChiTiet.PhienXetNghiem.BenhNhanId == benhNhanId
                                            && c.DichVuXetNghiemId == ketQuaXetNghiemVo.DichVuXetNghiemId
                                            && c.Id<ketQuaXetNghiemVo.KetQuaXetNghiemChiTietId
                                            //&& c.DichVuXetNghiemTen == ketQuaXetNghiemVo.DichVuXetNghiemTen
                                            )
                                            .Select(c => c.ThoiDiemDuyetKetQua).Distinct().OrderBy(c => c).ToListAsync();
            if (ngayDuyetKetQuaXNChiTiets.Any())
            {
                return ngayDuyetKetQuaXNChiTiets.Last()?.ApplyFormatDate();
            }
            return null;
        }
        #endregion
        private string returnStringTenTheoNhanVien(long nhanVienId)
        {

            var result = string.Empty;

            var queryInfo = _user.TableNoTracking.Where(d => d.Id == nhanVienId)
                .Select(d => new
                {
                    MaHocHamHocVi = d.NhanVien.HocHamHocViId != null ? d.NhanVien.HocHamHocVi.Ma : "",
                    TenNhanVien = d.HoTen
                }).FirstOrDefault();
            if (queryInfo != null)
            {
                result = (!string.IsNullOrEmpty(queryInfo.MaHocHamHocVi) ? queryInfo.MaHocHamHocVi + " " : "") + queryInfo.TenNhanVien;
            }


            return result;
        }

        public async Task<List<PhieuInXetNghiemModel>> InKetQuaXetNghiemNew(DuyetKetQuaXetNghiemPhieuInVo ketQuaXetNghiemPhieuIn)
        {
            var lstContent = new List<PhieuInXetNghiemModel>();

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKetQuaXetNghiemNew"));
            List<long> listDefault = new List<long>();

            var thongTinPhienXetNghiem = BaseRepository.TableNoTracking
                .Where(o => o.Id == ketQuaXetNghiemPhieuIn.Id)
                .Select(o => new
                {
                    o.Id,
                    PhienXetNghiemChiTiets = o.PhienXetNghiemChiTiets.Select(ct => new
                    {
                        ct.Id,
                        ct.YeuCauDichVuKyThuatId,
                        ct.NhanVienKetLuanId
                    }).ToList(),
                    //Mau = o.MauXetNghiems.Select(m => new
                    //{
                    //    m.Id,
                    //    m.BarCodeId
                    //}).ToList(),
                    o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.YeuCauTiepNhan.DiaChiDayDu,
                    o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                    o.YeuCauTiepNhan.HoTen,
                    o.YeuCauTiepNhan.NgaySinh,
                    o.YeuCauTiepNhan.ThangSinh,
                    o.YeuCauTiepNhan.NamSinh,
                    o.YeuCauTiepNhan.CoBHYT,
                    o.YeuCauTiepNhan.BHYTMucHuong,
                    o.YeuCauTiepNhan.BHYTMaSoThe,
                    o.YeuCauTiepNhan.GioiTinh,
                    o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                    o.BarCodeId,
                    o.GhiChu,
                    o.NhanVienThucHienId
                })
                .FirstOrDefault();
            //SoPhieu = phienXetNghiemData.MauXetNghiems.LastOrDefault(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
            var mauXetNghiems = _mauXetNghiemRepo.TableNoTracking
                .Where(o => o.PhienXetNghiemId == thongTinPhienXetNghiem.Id && o.BarCodeId == thongTinPhienXetNghiem.BarCodeId)
                .Select(o => new
                {
                    o.Id,
                    SoPhieu = o.PhieuGoiMauXetNghiemId != null ? o.PhieuGoiMauXetNghiem.SoPhieu : ""
                })
                .ToList();

            var nhanVienThucHien = _nhanVienRepo.TableNoTracking
                .Where(o => o.Id == thongTinPhienXetNghiem.NhanVienThucHienId)
                .Select(o => new
                {
                    HocHamHocVi = o.HocHamHocViId != null ? o.HocHamHocVi.Ma : "",
                    NhomChucDanh = o.ChucDanhId != null ? o.ChucDanh.NhomChucDanh.Ma : "",
                    o.User.HoTen
                })
                .FirstOrDefault();

            var nguoiThucHien = returnStringTen(nhanVienThucHien.HocHamHocVi, nhanVienThucHien.NhomChucDanh, nhanVienThucHien.HoTen);

            var phienXetNghiemChiTietIds = thongTinPhienXetNghiem.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).Select(o => o.Id).ToList();
            var yeuCauDichVuKyThuatIds = thongTinPhienXetNghiem.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).Select(o => o.YeuCauDichVuKyThuatId).ToList();

            var phienXetNghiemChiTietDatas = _phienXetNghiemChiTietRepository.TableNoTracking
                .Where(o => phienXetNghiemChiTietIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.YeuCauDichVuKyThuatId,
                    o.YeuCauDichVuKyThuat.NhanVienChiDinhId,
                    o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                    o.YeuCauDichVuKyThuat.YeuCauKhamBenhId,
                    o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId,
                    o.ThoiDiemNhanMau,
                    o.ThoiDiemKetLuan,
                    o.ThoiDiemLayMau,
                    o.NhanVienLayMauId,
                    o.NhanVienNhanMauId,
                    KetQuaXetNghiemChiTiets = o.KetQuaXetNghiemChiTiets.ToList()
                }).ToList();

            var dichVuXetNghiems = _dichVuXetNghiemRepository.TableNoTracking.Include(o => o.HDPP).ToList();
            var mayXetNghiems = _mayXetNghiemRepo.TableNoTracking.ToList();

            var dichVuKyThuatBenhVienIds = phienXetNghiemChiTietDatas.SelectMany(o => o.KetQuaXetNghiemChiTiets).Select(o => o.DichVuKyThuatBenhVienId).Distinct().ToList();

            var tenDichVuKyThuatBenhViens = _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Where(o => dichVuKyThuatBenhVienIds.Contains(o.Id))
                .Select(o => new LookupItemVo
                {
                    KeyId = o.Id,
                    DisplayName = o.Ten
                }).ToList();

            var nhomDichVuBenhViens = _nhomDichVuBenhVienRepo.TableNoTracking.Select(o => new
            {
                o.Id,
                o.Ten
            }).ToList();

            //var phienXetNghiemChiTietEntities = phienXetNghiemData.PhienXetNghiemChiTiets.Where(x => x.NhanVienKetLuanId != null).ToList();

            var thongTinBacSi = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => yeuCauDichVuKyThuatIds.Contains(o.Id) && (o.YeuCauKhamBenhId != null || o.NoiTruPhieuDieuTriId != null))
                .OrderBy(o => o.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    BacSiChiDinh = q.NhanVienChiDinh.User.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinhId != null ? q.NoiChiDinh.Ten : ""
                })//.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinLeTan = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => yeuCauDichVuKyThuatIds.Contains(o.Id) && o.YeuCauKhamBenhId == null && o.NoiTruPhieuDieuTriId == null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    BacSiChiDinh = q.NhanVienChiDinh.User.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinhId != null ? q.NoiChiDinh.Ten : "",
                    FromLeTan = true,
                })//.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();


            var thongTinPhieuIns = thongTinBacSi.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First()).ToList();
            thongTinPhieuIns.AddRange(thongTinLeTan.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First()).ToList());
            var numberPage = 1;

            string ngay = DateTime.Now.Day.ToString();
            string thang = DateTime.Now.Month.ToString();
            string nam = DateTime.Now.Year.ToString();
            string gio = DateTime.Now.Hour + " giờ " + DateTime.Now.Minute + " phút";

            var listYeuCauDichVuKyThuatThuocNhomSarsCov2s = GetListYeuCauTrongNhomSars(ketQuaXetNghiemPhieuIn.ListIn);

            var thongTinPhieuInTheoNhanVienChiDinhs = thongTinPhieuIns.GroupBy(d => d.BacSiChiDinhId)
                                                                      .Select(d => new ThongTinBacSiVo
                                                                      {
                                                                          BacSiChiDinh = d.First().BacSiChiDinh,
                                                                          BacSiChiDinhId = d.First().BacSiChiDinhId,
                                                                          KhoaPhongChiDinh = d.First().KhoaPhongChiDinh // lấy first
                                                                      })
                                                                      .ToList();

            foreach (var thongTin in thongTinPhieuInTheoNhanVienChiDinhs)
            {


                var data = new DuyetKetQuaXetNghiemPhieuInResultVo
                {
                    //SoPhieu = phienXetNghiemData.MauXetNghiems.LastOrDefault(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
                    SoPhieu = mauXetNghiems.LastOrDefault()?.SoPhieu,
                    SoVaoVien = thongTinPhienXetNghiem.MaYeuCauTiepNhan,
                    LogoUrl = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha-full.png",
                    MaYTe = thongTinPhienXetNghiem.MaBN,

                    HoTen = thongTinPhienXetNghiem.HoTen,
                    //DiaChi = phienXetNghiemData.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? phienXetNghiemData.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoe?.CongTyKhamSucKhoe?.Ten : phienXetNghiemData.YeuCauTiepNhan.DiaChiDayDu,
                    GhiChuDV = thongTinPhienXetNghiem.GhiChu,
                    NamSinh = thongTinPhienXetNghiem.NamSinh,
                    NamSinhString = DateHelper.DOBFormat(thongTinPhienXetNghiem.NgaySinh, thongTinPhienXetNghiem.ThangSinh, thongTinPhienXetNghiem.NamSinh),
                    GioiTinh = thongTinPhienXetNghiem.GioiTinh != null ? thongTinPhienXetNghiem.GioiTinh : null,

                    DoiTuong = thongTinPhienXetNghiem.CoBHYT == true ? "BHYT (" + thongTinPhienXetNghiem.BHYTMucHuong + "%)" : "Viện phí",

                    MaBHYT = thongTinPhienXetNghiem.BHYTMaSoThe,

                    BsChiDinh = returnStringTenTheoNhanVien(thongTin.BacSiChiDinhId),
                    KhoaPhong = thongTin.KhoaPhongChiDinh,
                    Barcode = thongTinPhienXetNghiem.BarCodeId.Length == 10 ? thongTinPhienXetNghiem.BarCodeId.Substring(0, 6) + "-" + thongTinPhienXetNghiem.BarCodeId.Substring(6) : thongTinPhienXetNghiem.BarCodeId,

                };

                if (thongTinPhienXetNghiem.HopDongKhamSucKhoeNhanVienId != null)
                {
                    var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepo.TableNoTracking
                        .Where(o => o.Id == thongTinPhienXetNghiem.HopDongKhamSucKhoeNhanVienId)
                        .Select(o => new
                        {
                            o.Id,
                            o.STTNhanVien,
                            TenCty = o.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten
                        }).First();

                    data.DiaChi = hopDongKhamSucKhoeNhanVien.TenCty;
                    data.STT = hopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{hopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
                }
                else
                {
                    data.DiaChi = thongTinPhienXetNghiem.DiaChiDayDu;
                    data.STT = string.Empty;
                }

               
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(ketQuaXetNghiemPhieuIn.Id.ToString())
                    ? BarcodeHelper.GenerateBarCode(ketQuaXetNghiemPhieuIn.Id.ToString())
                    : "";

                List<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiets = phienXetNghiemChiTietDatas
                    .Where(o => o.NhanVienChiDinhId == thongTin.BacSiChiDinhId)
                    .SelectMany(p => p.KetQuaXetNghiemChiTiets).ToList();

                


                var listGhiChuTheoDVXNISOs = new List<KetQuaXetNghiemChiTiet>();
                var listGhiChuTheoDVChuyenGois = new List<KetQuaXetNghiemChiTiet>();

              


                var lstYeuCauDichVuKyThuatIdTrongPhieu = new List<long>();
                if (ketQuaXetNghiemChiTiets.Any())
                {
                    if (ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds.Any())
                    {
                        if (ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds.Any())
                        {
                            var info = string.Empty;
                            foreach (var nhomDichVuBvId in ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienIds)
                            {
                                var totalTenNhom = nhomDichVuBenhViens.Where(p => p.Id == nhomDichVuBvId).Select(p => p.Ten).Distinct().OrderBy(o => o).ToList();
                                var STT = 1;
                                foreach (var tenNhom in totalTenNhom)
                                {
                                    var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                            + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b>" + tenNhom.ToUpper()
                                                            + "</b></tr>";
                                    info += headerNhom;
                                    var queryNhom = new List<KetQuaXetNghiemChiTiet>();
                                    //var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                                     //   .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

                                    var lstYeuCauDichVuKyThuatId = ketQuaXetNghiemChiTiets.Where(d=>d.NhomDichVuBenhVienId == nhomDichVuBvId).Select(d => d.YeuCauDichVuKyThuatId).Distinct().ToList();


                                    foreach (var ycId in lstYeuCauDichVuKyThuatId)
                                    {
                                        if (!phienXetNghiemChiTietDatas.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                                        var res = phienXetNghiemChiTietDatas.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                                        queryNhom.AddRange(res);
                                    }
                                    lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);
                                   
                                    var dvXetNghiemChaIds = queryNhom.Where(d => d.DichVuXetNghiemChaId == null)
                                .Select(d => d.DichVuXetNghiemId).ToList();

                                    foreach (var dataParent in queryNhom.Where(d => d.DichVuXetNghiemChaId == null && !dvXetNghiemChaIds.Contains(d.DichVuXetNghiemChaId.GetValueOrDefault()))
                               .OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).Distinct())
                                    {
                                        info = info + GetPhieuInKetQuaDuyet("", STT,
                                                   queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).Distinct()
                                                       .ToList(), dataParent, dichVuXetNghiems, mayXetNghiems, tenDichVuKyThuatBenhViens);
                                    }
                                }
                            }
                            data.DanhSach = info;
                        }
                    }
                }
                data.NguoiThucHien = nguoiThucHien;
                

                if (thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                {
                    var yeuCauKhamBenhIds = phienXetNghiemChiTietDatas
                        .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId) && o.YeuCauKhamBenhId != null)
                        .Select(o => o.YeuCauKhamBenhId.Value)
                        .ToList();

                    var noiTruPhieuDieuTris = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(o => yeuCauKhamBenhIds.Contains(o.Id) && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.ChanDoanSoBoICDId != null)
                        .Select(o => new
                        {
                            o.Id,
                            ChanDoan = o.ChanDoanSoBoICDId != null ? (o.ChanDoanSoBoICD.Ma + "-" + o.ChanDoanSoBoICD.TenTiengViet) : "",
                            DienGiai = o.ChanDoanSoBoGhiChu
                        }).ToList();
                    data.ChanDoan = noiTruPhieuDieuTris.Where(p => p.ChanDoan != null && p.ChanDoan != "-" && p.ChanDoan != "").OrderBy(o => o.Id).LastOrDefault()?.ChanDoan;
                    data.DienGiai = noiTruPhieuDieuTris.OrderBy(o => o.Id).LastOrDefault()?.DienGiai;
                }
                else if (thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                {
                    var noiTruPhieuDieuTriIds = phienXetNghiemChiTietDatas
                        .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId) && o.NoiTruPhieuDieuTriId != null)
                        .Select(o => o.NoiTruPhieuDieuTriId.Value)
                        .ToList();

                    var noiTruPhieuDieuTris = _noiTruPhieuDieuTriRepo.TableNoTracking
                        .Where(o => noiTruPhieuDieuTriIds.Contains(o.Id))
                        .Select(o => new
                        {
                            o.Id,
                            ChanDoan = o.ChanDoanChinhICDId != null ? (o.ChanDoanChinhICD.Ma + "-" + o.ChanDoanChinhICD.TenTiengViet) : "",
                            DienGiai = o.ChanDoanChinhGhiChu
                        }).ToList();
                    data.ChanDoan = noiTruPhieuDieuTris.Where(p => p.ChanDoan != null && p.ChanDoan != "-" && p.ChanDoan != "").OrderBy(o => o.Id).LastOrDefault()?.ChanDoan;
                    data.DienGiai = noiTruPhieuDieuTris.OrderBy(o => o.Id).LastOrDefault()?.DienGiai;
                }
                else if (thongTinPhienXetNghiem.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                {
                    data.ChanDoan = "";
                    data.DienGiai = "";
                }

               
                var enumLoaiMauXetNghiems = phienXetNghiemChiTietDatas
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                    .Select(o => o.LoaiMauXetNghiem).Distinct();


                data.LoaiMau = string.Join("; ", enumLoaiMauXetNghiems.Select(o => o.GetDescription()));


                var phienXetNghiemChiTietLast = phienXetNghiemChiTietDatas
                    .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();

                if (phienXetNghiemChiTietLast != null)
                {
                    var nhanViens = _nhanVienRepo.TableNoTracking
                    .Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienNhanMauId || o.Id == phienXetNghiemChiTietLast.NhanVienLayMauId)
                    .Select(o => new
                    {
                        o.Id,
                        HocHamHocVi = o.HocHamHocViId != null ? o.HocHamHocVi.Ma : "",
                        NhomChucDanh = o.ChucDanhId != null ? o.ChucDanh.NhomChucDanh.Ma : "",
                        o.User.HoTen
                    })
                    .ToList();

                    data.TgLayMau = phienXetNghiemChiTietLast.ThoiDiemLayMau?.ApplyFormatDateTimeSACH();
                    if (phienXetNghiemChiTietLast.NhanVienLayMauId != null)
                    {
                        var nguoiLayMau = nhanViens.Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienLayMauId).FirstOrDefault();
                        data.NguoiLayMau = returnStringTen(nguoiLayMau?.HocHamHocVi, nguoiLayMau?.NhomChucDanh, nguoiLayMau?.HoTen);
                    }
                    data.TgNhanMau = phienXetNghiemChiTietLast.ThoiDiemNhanMau?.ApplyFormatDateTimeSACH();

                    if (phienXetNghiemChiTietLast.NhanVienNhanMauId != null)
                    {
                        var nguoiNhanMau = nhanViens.Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienNhanMauId).FirstOrDefault();
                        data.NguoiNhanMau = returnStringTen(nguoiNhanMau?.HocHamHocVi, nguoiNhanMau?.NhomChucDanh, nguoiNhanMau?.HoTen);
                    }
                }
                if (phienXetNghiemChiTietLast != null)
                {
                    data.Ngay = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                    ngay = data.Ngay;
                    data.Thang = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                    thang = data.Thang;
                    data.Nam = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                    nam = data.Nam;
                    data.Gio = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";
                    gio = data.Gio;
                }
                data.NgayThangNamFooter = DateTime.Now.ApplyFormatDateTimeSACH();
                var lstContentItem = new PhieuInXetNghiemModel();
                // 
                if (!string.IsNullOrEmpty(data.DanhSach))
                {
                    if (listGhiChuTheoDVXNISOs.Count() != 0)
                    {
                        data.LogoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                        data.LogoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";
                    }

                    lstContentItem.Html = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, data);

                    #region BVHD-3919
                    var templatefootertableXetNghiem = _templateRepository.TableNoTracking.First(x => x.Name.Equals("FooterXetNghiemTable"));

                    data.Ngay = data.Ngay == null ? ngay : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                    data.Thang = data.Thang == null ? thang : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                    data.Nam = data.Nam == null ? nam : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                    data.Gio = data.Gio == null ? gio : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";

                    //Phần ghi chú sắp xếp như sau:
                    //Dòng đầu tiên là dòng ghi chú mặc định (footer template)
                    //Dòng thứ 2 là ghi chú ISO (ghiChu) 
                    //Dòng thứ 3 là ghi chú Dịch vụ gửi ngoài (ghiChu) 
                    //Dòng thứ 4 là dòng ghi chú của dịch vụ  (Ghi chú DV)

                    var ghiChu = string.Empty;
                    var logoBV1 = string.Empty;
                    var logoBV2 = string.Empty;



                    if (listGhiChuTheoDVXNISOs.Count() != 0)
                    {

                        if (!string.IsNullOrEmpty(ghiChu))
                        {
                            ghiChu += "<br>";
                        }
                        //update 16/05: - Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi  THAY ĐỔI THÀNH  - Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.
                        //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";
                        ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";

                        logoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                        logoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";

                        data.LogoBV1 = logoBV1;
                        data.LogoBV2 = logoBV2;
                    }

                    if (listGhiChuTheoDVChuyenGois.Count() != 0)
                    {
                        if (!string.IsNullOrEmpty(ghiChu))
                        {
                            ghiChu += "<br>";
                        }
                        //update 16/05: - Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012 THAY ĐỔI THÀNH - Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012
                        //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi.</span>";
                        ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.</span>";
                    }

                    data.GhiChu = (!string.IsNullOrEmpty(ghiChu) ? ghiChu + "<br>" : "") + (!string.IsNullOrEmpty(data.GhiChuDV) ? "- " + data.GhiChuDV : "");


                    var htmlfooter = TemplateHelpper.FormatTemplateWithContentTemplate(templatefootertableXetNghiem.Body, data);
                    lstContentItem.Html += htmlfooter;
                    lstContentItem.Html += "<div style='margin-left: 2cm;'><table width='100%'><tr><td style='width:36%'></td><td style='width:35%'></td><td style='width:29%'><b>" + data.NguoiThucHien + "</b></td>";
                    lstContentItem.Html += "</tr></table></div>";

                    #endregion BVHD-3919

                    lstContent.Add(lstContentItem);
                }
                numberPage++;
            }

            return lstContent;
        }

        public List<PhieuInXetNghiemModel> InPhieuXetNghiemTheoYeuCauKyThuatVaNhomNew(LayPhieuXetNghiemTheoYCKTVaNhomDVBVVo ketQuaXetNghiemPhieuIn)
        {
            var lstContent = new List<PhieuInXetNghiemModel>();
            var numberPage = 1;

            string ngay = string.Empty;
            string thang = string.Empty;
            string nam = string.Empty;
            string gio = string.Empty;

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKetQuaXetNghiemNew"));
            List<long> listDefault = new List<long>();

            #region lấy tất cả phiên xét nghiệm  theo yêu cầu tiếp nhận và NhomDichVuBenhVienId , phieuDieuTriHienTaiId

            var phienXetNghiemIdTheoBenhNhans = BaseRepository.TableNoTracking
                            .Where(c => c.YeuCauTiepNhanId == ketQuaXetNghiemPhieuIn.YeuCauTiepNhanId)
                            .Select(d => d.Id).ToList();






            //var phienXetNghiemChiTiets = BaseRepository.TableNoTracking
            //                        .Where(c => c.YeuCauTiepNhanId == ketQuaXetNghiemPhieuIn.YeuCauTiepNhanId)
            //                        .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
            //                        .SelectMany(d => d.PhienXetNghiemChiTiets);

            //var phienXetNghiemIds = phienXetNghiemChiTiets.Where(cc => cc.YeuCauDichVuKyThuat.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId &&
            //                                                          cc.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId == ketQuaXetNghiemPhieuIn.phieuDieuTriHienTaiId)
            //                                              .Select(d => d.PhienXetNghiemId).ToList();
            #endregion

            var inFophienXetNghiemTheoYckts = BaseRepository.TableNoTracking.Where(c => phienXetNghiemIdTheoBenhNhans.Contains(c.Id))
                .Select(d => new ThongTinPhienXNTheoBenhNhanVo
                {
                    Id = d.Id,
                    PhienXetNghiemChiTietIds = d.PhienXetNghiemChiTiets.Select(f => f.Id).ToList(),
                    YeuCauDichVuKyThuatIds = d.PhienXetNghiemChiTiets.Select(f => f.YeuCauDichVuKyThuatId).ToList()
                }).ToList();


            var yeuCauDichVuKyThuatIds = inFophienXetNghiemTheoYckts.
                SelectMany(d => d.YeuCauDichVuKyThuatIds).Select(d => d).ToList();

            var yeuCauDichVuKyThuatXNIds = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(d => yeuCauDichVuKyThuatIds.Contains(d.Id) &&
                d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem &&
                d.NhomDichVuBenhVienId  == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId)
                .Select(d => d.Id).ToList();

            // all tat ca dich vu xn cua tat ca cac phien
            var listPhienXetNghiemChiTietss = _phienXetNghiemChiTietRepository.TableNoTracking
                  .Include(d => d.KetQuaXetNghiemChiTiets)
                  .Where(s => yeuCauDichVuKyThuatXNIds.Contains(s.YeuCauDichVuKyThuatId)).ToList();

            //// get phien xet nghiem chi tiet tac ca cac phien
            //var tatCaCacPhienXetNghiemChiTietIds = inFophienXetNghiemTheoYckts.
            //    SelectMany(d => d.PhienXetNghiemChiTietIds)
            //    .Select(d => d).ToList();

            // get tat ca ket qua xet nghiem chi tiet cac phien
            var yeuCauDichVuKyThuatIdTatCaCaPhien = _phienXetNghiemChiTietRepository.TableNoTracking
                .Where(d => yeuCauDichVuKyThuatXNIds.Contains(d.YeuCauDichVuKyThuatId))
                .Select(c => new ThongTinBSTheoTungPhienVo { 
                    YeuCauDichVuKyThuatId= c.YeuCauDichVuKyThuatId,
                    PhienXetNghiemId = c.PhienXetNghiemId })
                .ToList();

            var groupYeuCauDichVuKyThuatIdTatCaCaPhien =
                yeuCauDichVuKyThuatIdTatCaCaPhien.GroupBy(d => d.PhienXetNghiemId)
                .Select(item => new ThongTinBSTheoTungPhienVo
                {
                    YeuCauDichVuKyThuatIds = item.Select(d=>d.YeuCauDichVuKyThuatId).ToList(),
                    PhienXetNghiemId = item.First().PhienXetNghiemId
                }).ToList();

            var thongTinPhieuInNews = new List<ThongTinBacSiVo>();

            var thongTinBacSiNew = _yeuCauDichVuKyThuatRepository.TableNoTracking
                           .Where(o => yeuCauDichVuKyThuatXNIds.Contains(o.Id) &&
                           (o.YeuCauKhamBenhId != null || o.NoiTruPhieuDieuTriId != null))
                           .OrderBy(o => o.ThoiDiemHoanThanh)
                           .Select(q => new ThongTinBacSiVo
                           {
                               BacSiChiDinh = q.NhanVienChiDinh.User.HoTen,
                               BacSiChiDinhId = q.NhanVienChiDinhId,
                               KhoaPhongChiDinh = q.NoiChiDinhId != null ? q.NoiChiDinh.Ten : "",
                               //PhienXetNghiemId = q.PhienXetNghiemId,
                               YeuCauDichVuKyThuatId = q.Id
                           })
                           .ToList();

            var thongTinLeTanNew = _yeuCauDichVuKyThuatRepository.TableNoTracking
            .Where(o => yeuCauDichVuKyThuatXNIds.Contains(o.Id) &&
                    o.YeuCauKhamBenhId == null &&
                    o.NoiTruPhieuDieuTriId == null)
            .OrderBy(cc => cc.ThoiDiemHoanThanh)
            .Select(q => new ThongTinBacSiVo
            {
                BacSiChiDinh = q.NhanVienChiDinh.User.HoTen,
                BacSiChiDinhId = q.NhanVienChiDinhId,
                KhoaPhongChiDinh = q.NoiChiDinhId != null ? q.NoiChiDinh.Ten : "",
                FromLeTan = true,
                //PhienXetNghiemId = item.PhienXetNghiemId
                YeuCauDichVuKyThuatId = q.Id
            })
            .ToList();

            foreach (var item in groupYeuCauDichVuKyThuatIdTatCaCaPhien)
            {
                thongTinBacSiNew = thongTinBacSiNew.Where(d => item.YeuCauDichVuKyThuatIds.Contains(d.YeuCauDichVuKyThuatId))
                    .Select(q => new ThongTinBacSiVo
                    {
                        BacSiChiDinh = q.BacSiChiDinh,
                        BacSiChiDinhId = q.BacSiChiDinhId,
                        KhoaPhongChiDinh = q.KhoaPhongChiDinh,
                        PhienXetNghiemId = item.PhienXetNghiemId,
                        YeuCauDichVuKyThuatId = q.YeuCauDichVuKyThuatId
                    }).ToList();

                thongTinLeTanNew = thongTinLeTanNew.Where(d => item.YeuCauDichVuKyThuatIds.Contains(d.YeuCauDichVuKyThuatId))
                     .Select(q => new ThongTinBacSiVo
                     {
                         BacSiChiDinh = q.BacSiChiDinh,
                         BacSiChiDinhId = q.BacSiChiDinhId,
                         KhoaPhongChiDinh = q.KhoaPhongChiDinh,
                         PhienXetNghiemId = item.PhienXetNghiemId,
                         FromLeTan = q.FromLeTan,
                         YeuCauDichVuKyThuatId = q.YeuCauDichVuKyThuatId
                     }).ToList();

                thongTinPhieuInNews.AddRange(thongTinBacSiNew.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First()).ToList());
                thongTinPhieuInNews.AddRange(thongTinLeTanNew.GroupBy(p => p.BacSiChiDinhId).Select(p => p.First()).ToList());
            }


            var thongTinPhieuInTheoNhanVienChiDinhNews = thongTinPhieuInNews.GroupBy(d => new { d.BacSiChiDinhId, d.PhienXetNghiemId })
                                                       .Select(d => new ThongTinBacSiTheoPhienVo
                                                       {
                                                           BacSiChiDinh = d.First().BacSiChiDinh,
                                                           BacSiChiDinhId = d.First().BacSiChiDinhId,
                                                           KhoaPhongChiDinh = d.First().KhoaPhongChiDinh, // lấy first
                                                           PhienXetNghiemId = d.First().PhienXetNghiemId,
                                                           //PhienXetNghiemChiTiets = d.First().PhienXetNghiemChiTiets
                                                       })
                                                       .ToList();



            var phienXetNghiemIds = inFophienXetNghiemTheoYckts.Select(d => d.Id).ToList();


            var thongTinPhienXetNghiem = BaseRepository.TableNoTracking
                 .Where(o => phienXetNghiemIds.Contains(o.Id))
                 .Select(o => new ThongTinBenhNhanTungPhienVo
                 {
                     Id =o.Id,
                     PhienXetNghiemChiTiets = o.PhienXetNghiemChiTiets.Select(ct => new PhienXetNghiemChiTietVO
                     {
                         Id =ct.Id,
                         YeuCauDichVuKyThuatId = ct.YeuCauDichVuKyThuatId,
                         NhanVienKetLuanId = ct.NhanVienKetLuanId,
                     }).ToList(),

                     MaYeuCauTiepNhan= o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                     MaBN = o.BenhNhan.MaBN,
                     DiaChiDayDu = o.YeuCauTiepNhan.DiaChiDayDu,
                     HopDongKhamSucKhoeNhanVienId = o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                     HoTen = o.YeuCauTiepNhan.HoTen,
                     NgaySinh = o.YeuCauTiepNhan.NgaySinh,
                     ThangSinh = o.YeuCauTiepNhan.ThangSinh,
                     NamSinh = o.YeuCauTiepNhan.NamSinh,
                     CoBHYT = o.YeuCauTiepNhan.CoBHYT,
                     BHYTMucHuong = o.YeuCauTiepNhan.BHYTMucHuong,
                     BHYTMaSoThe = o.YeuCauTiepNhan.BHYTMaSoThe,
                     GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                     LoaiYeuCauTiepNhan = o.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                     BarCodeId = o.BarCodeId,
                     GhiChu = o.GhiChu,
                     NhanVienThucHienId = o.NhanVienThucHienId,
                     
                 })
                 .ToList();
            //SoPhieu = phienXetNghiemData.MauXetNghiems.LastOrDefault(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
            var mauXetNghiems = _mauXetNghiemRepo.TableNoTracking
                .Select(o => new
                {
                    o.Id,
                    SoPhieu = o.PhieuGoiMauXetNghiemId != null ? o.PhieuGoiMauXetNghiem.SoPhieu : "",
                    o.PhienXetNghiemId,
                    o.BarCodeId
                })
                .ToList();

            var nhanVienThucHien = _nhanVienRepo.TableNoTracking
                .Select(o => new
                {
                    HocHamHocVi = o.HocHamHocViId != null ? o.HocHamHocVi.Ma : "",
                    NhomChucDanh = o.ChucDanhId != null ? o.ChucDanh.NhomChucDanh.Ma : "",
                    o.User.HoTen,
                    o.Id
                })
                .ToList();

            var hopDongKhamSucKhoeNhanVienIds = thongTinPhienXetNghiem.Select(d => d.HopDongKhamSucKhoeNhanVienId).ToList();

            

            var hopDongKhamSucKhoeNhanViens = _hopDongKhamSucKhoeNhanVienRepo.TableNoTracking
                   .Where(o => hopDongKhamSucKhoeNhanVienIds.Contains(o.Id))
                   .Select(o => new
                   {
                       o.Id,
                       o.STTNhanVien,
                       TenCty = o.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten
                   }).ToList();
            foreach (var item in thongTinPhienXetNghiem.ToList())
            {
                item.SoPhieu = mauXetNghiems.Where(d => d.PhienXetNghiemId == item.Id && d.BarCodeId == item.BarCodeId).LastOrDefault()?.SoPhieu + "";

                if (item.HopDongKhamSucKhoeNhanVienId != null)
                {
                    var hopDongKhamSucKhoeNhanVien = hopDongKhamSucKhoeNhanViens.Where(d=>d.Id == item.HopDongKhamSucKhoeNhanVienId).FirstOrDefault();
                    if(hopDongKhamSucKhoeNhanVien != null){
                        item.DiaChi = hopDongKhamSucKhoeNhanVien.TenCty;
                        item.STT = hopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{hopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
                    }
                    
                }
                else
                {
                    item.DiaChi = item.DiaChiDayDu;
                    item.STT = string.Empty;
                }

            }

            var phienXetNghiemChiTietIds = thongTinPhienXetNghiem.SelectMany(d=>d.PhienXetNghiemChiTiets).Where(x => x.NhanVienKetLuanId != null).Select(o => o.Id).ToList();

            #region phienXetNghiemChiTietDatas
            var phienXetNghiemChiTietDatas = _phienXetNghiemChiTietRepository.TableNoTracking
                .Where(o => phienXetNghiemChiTietIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.YeuCauDichVuKyThuatId,
                    o.YeuCauDichVuKyThuat.NhanVienChiDinhId,
                    o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem,
                    o.YeuCauDichVuKyThuat.YeuCauKhamBenhId,
                    o.YeuCauDichVuKyThuat.NoiTruPhieuDieuTriId,
                    o.ThoiDiemNhanMau,
                    o.ThoiDiemKetLuan,
                    o.ThoiDiemLayMau,
                    o.NhanVienLayMauId,
                    o.NhanVienNhanMauId,
                    KetQuaXetNghiemChiTiets = o.KetQuaXetNghiemChiTiets.ToList(),
                    o.PhienXetNghiemId
                }).ToList();
            #endregion
            var dichVuXetNghiems = _dichVuXetNghiemRepository.TableNoTracking.Include(o => o.HDPP).ToList();
            var mayXetNghiems = _mayXetNghiemRepo.TableNoTracking.ToList();

            var dichVuKyThuatBenhVienIds = phienXetNghiemChiTietDatas
                .SelectMany(o => o.KetQuaXetNghiemChiTiets)
                .Select(o => o.DichVuKyThuatBenhVienId).Distinct().ToList();

            var thongTinDichVuKyThuatBenhViens = _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Where(o => dichVuKyThuatBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ten,
                    o.DichVuChuyenGoi
                }).ToList();

            var tenDichVuKyThuatBenhViens = thongTinDichVuKyThuatBenhViens
                .Select(o => new LookupItemVo
                {
                    KeyId = o.Id,
                    DisplayName = o.Ten
                }).ToList();            

            var yeuCauKhamBenhAlls = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(o =>  o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.ChanDoanSoBoICDId != null)
                            .Select(o => new
                            {
                                o.Id,
                                ChanDoan = o.ChanDoanSoBoICDId != null ? (o.ChanDoanSoBoICD.Ma + "-" + o.ChanDoanSoBoICD.TenTiengViet) : "",
                                DienGiai = o.ChanDoanSoBoGhiChu,
                                TrangThai = o.TrangThai,
                                ChanDoanSoBoICDId = o.ChanDoanSoBoICDId
                            }).ToList();

            var noiTruPhieuDieuTriAlls = _noiTruPhieuDieuTriRepo.TableNoTracking
                           .Select(o => new
                           {
                               o.Id,
                               ChanDoan = o.ChanDoanChinhICDId != null ? (o.ChanDoanChinhICD.Ma + "-" + o.ChanDoanChinhICD.TenTiengViet) : "",
                               DienGiai = o.ChanDoanChinhGhiChu
                           }).ToList();

            



            var lstDichVuSarCoVs = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhTiepNhan.DichVuTestSarsCovid")
             .Select(d => d.Value).FirstOrDefault();

            var json = JsonConvert.DeserializeObject<List<DichVuKyThuatBenhVienIdsSarsCoV>>(lstDichVuSarCoVs);
            var listDichVuKyThuatSarsCov2s = json.Select(d => d.DichVuKyThuatBenhVienId).ToList();

            var tenNhoms = _yeuCauDichVuKyThuatRepository.TableNoTracking
                         .Where(c => yeuCauDichVuKyThuatXNIds.Contains(c.Id) &&
                                    c.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId)
                         .Select(b => new ThongTinNhomDichVuBenhVienVo
                         {
                             NhomDichVuBenhVienId = b.NhomDichVuBenhVienId,
                             NhanVienChiDinhId = b.NhanVienChiDinhId,
                             YeuCauDichVuKyThuatId = b.Id
                         }).ToList();

            var tenNhomDichVuBenhViens = _nhomDichVuBenhVienRepo.TableNoTracking.Select(d => new { d.Ten, d.Id }).ToList();


            var templatefootertableXetNghiem = _templateRepository.TableNoTracking.First(x => x.Name.Equals("FooterXetNghiemTable"));

            foreach (var infoTenNhom in tenNhoms)
            {
                infoTenNhom.TenNhom = tenNhomDichVuBenhViens.
                                    Where(d => d.Id == infoTenNhom.NhomDichVuBenhVienId)
                                    .Select(d => d.Ten).FirstOrDefault();
            }

            foreach (var thongTin in thongTinPhieuInTheoNhanVienChiDinhNews)
            {
                var data = new DuyetKetQuaXetNghiemPhieuInResultVo
                {
                    SoPhieu = thongTinPhienXetNghiem.Where(d=>d.Id == thongTin.PhienXetNghiemId).Select(d=>d.SoPhieu).FirstOrDefault(),
                   
                    SoVaoVien = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.MaYeuCauTiepNhan).FirstOrDefault(),

                    LogoUrl = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha-full.png",

                    MaYTe = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.MaBN).FirstOrDefault(),

                    HoTen = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.HoTen).FirstOrDefault(),
                    
                    DiaChi = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.DiaChi).FirstOrDefault(),
                    
                    GhiChuDV = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.GhiChu).FirstOrDefault(),

                    NamSinh = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.NamSinh).FirstOrDefault(),

                    GioiTinh = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.GioiTinh).FirstOrDefault(),

                    NamSinhString = DateHelper.DOBFormat(thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.NgaySinh).FirstOrDefault(),
                                                         thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.ThangSinh).FirstOrDefault(),
                                                         thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.NamSinh).FirstOrDefault()),

                    DoiTuong = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.CoBHYT).FirstOrDefault() == true ? "BHYT (" + thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.BHYTMucHuong).FirstOrDefault() + "%)" : "Viện phí",

                    MaBHYT = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.BHYTMaSoThe).FirstOrDefault(),

                    BsChiDinh = returnStringTenTheoNhanVien(thongTin.BacSiChiDinhId),
                    
                    KhoaPhong = thongTin.KhoaPhongChiDinh,
                    //Barcode = thongTin.PhienXetNghiem.BarCodeId.Length == 10 ? thongTin.PhienXetNghiem.BarCodeId.Substring(0, 6) + "-" + thongTin.PhienXetNghiem.BarCodeId.Substring(6) : thongTin.PhienXetNghiem.BarCodeId
                };

                if(thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.BarCodeId).FirstOrDefault() != null)
                {
                    data.Barcode = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.BarCodeId).First().Length == 10 ? thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.BarCodeId).First().Substring(0, 6) + "-" + thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.BarCodeId).First().Substring(6) : thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.BarCodeId).First();
                }

                if (thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.STT).FirstOrDefault() != null)
                {
                    data.STT = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.STT).FirstOrDefault();
                }
                data.BarCodeImgBase64 = !string.IsNullOrEmpty(thongTin.PhienXetNghiemId.ToString()) ? BarcodeHelper.GenerateBarCode(thongTin.PhienXetNghiemId.ToString()) : "";
                var lstYeuCauDichVuKyThuatIdTrongPhieu = new List<long>();

                var tenNhom = tenNhoms.Where(c => c.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId
                                                                      && c.NhanVienChiDinhId == thongTin.BacSiChiDinhId)
                                                           .Select(p => p.TenNhom).Distinct().OrderBy(o => o).FirstOrDefault();

                var info = string.Empty;
                if (tenNhom != null)
                {
                    var STT = 1;
                    var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style='border: 1px solid #020000;text-align: left;' colspan='6'><b>" +
                                       tenNhom.ToUpper()
                                       + "</b></tr>";



                    var listKqXN = new List<string>();
                    var listGhiChuTheoDVXNISOs = new List<KetQuaXetNghiemChiTiet>();
                    var listGhiChuTheoDVChuyenGois = new List<KetQuaXetNghiemChiTiet>();

                    var queryNhom = new List<KetQuaXetNghiemChiTiet>();

                    var lstYeuCauDichVuKyThuatId = tenNhoms.Where(c => c.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId
                                                                      && c.NhanVienChiDinhId == thongTin.BacSiChiDinhId)
                                                           .Select(p => p.YeuCauDichVuKyThuatId);


                    foreach (var ycId in lstYeuCauDichVuKyThuatId.Where(d => !listDichVuKyThuatSarsCov2s.Contains(d)).ToList())
                    {
                        if (!listPhienXetNghiemChiTietss.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                        var res = listPhienXetNghiemChiTietss.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                        queryNhom.AddRange(res);
                    }

                    lstYeuCauDichVuKyThuatIdTrongPhieu.AddRange(lstYeuCauDichVuKyThuatId);


                    // BVHD-3901 
                    //ghiChu = string.Empty;

                    if (queryNhom.Where(p => p.DichVuXetNghiemChaId == null).ToList().Count() != 0)
                    {
                        var dvIns = queryNhom.Where(p => p.DichVuXetNghiemChaId == null).ToList();
                        var dvIsos = dvIns.Where(d => dichVuXetNghiems.FirstOrDefault(xn => xn.Id == d.DichVuXetNghiemId)?.LaChuanISO == true).ToList();

                        if (dvIsos.Any())
                        {
                            listGhiChuTheoDVXNISOs.AddRange(dvIsos);
                        }

                        var dvChuyenGuis = dvIns.Where(d => thongTinDichVuKyThuatBenhViens.FirstOrDefault(kt => kt.Id == d.DichVuKyThuatBenhVienId)?.DichVuChuyenGoi == true).ToList();

                        if (dvChuyenGuis.Any())
                        {
                            listGhiChuTheoDVChuyenGois.AddRange(dvChuyenGuis);
                        }

                        //var chiSoXNISO = dvIns.Where(d => d.DichVuXetNghiem?.LaChuanISO == true).ToList().Count() != 0 ? true : false;

                        //if (chiSoXNISO == true)
                        //{
                        //    listGhiChuTheoDVXNISOs.AddRange(dvIns.Where(d => d.DichVuXetNghiem?.LaChuanISO == true).ToList());
                        //}

                        //var chiSoXNChuyenGui = dvIns.Where(d => d.DichVuKyThuatBenhVien?.DichVuChuyenGoi == true).ToList().Count() != 0 ? true : false;

                        //if (chiSoXNChuyenGui == true)
                        //{
                        //    listGhiChuTheoDVChuyenGois.AddRange(dvIns.Where(d => d.DichVuKyThuatBenhVien?.DichVuChuyenGoi == true).ToList());
                        //}
                    }


                    var dvXetNghiemChaIds = queryNhom.Where(d => d.DichVuXetNghiemChaId == null)
                               .Select(d => d.DichVuXetNghiemId).ToList();

                    foreach (var dataParent in queryNhom.Where(d => d.DichVuXetNghiemChaId == null && !dvXetNghiemChaIds.Contains(d.DichVuXetNghiemChaId.GetValueOrDefault()))
               .OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).Distinct())
                    {
                        info = info + GetPhieuInKetQuaDuyet("", STT,
                                   queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).Distinct()
                                       .ToList(), dataParent, dichVuXetNghiems, mayXetNghiems, tenDichVuKyThuatBenhViens);
                    }





                    data.DanhSach = headerNhom + info;
                    if (thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.NhanVienThucHienId).FirstOrDefault() != null)
                    {
                        var nhanVienThucHienId = thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.NhanVienThucHienId).First();

                        var thongTinNguoithucHien = nhanVienThucHien.Where(ff => ff.Id == nhanVienThucHienId).First();
                        data.NguoiThucHien = returnStringTen(thongTinNguoithucHien.HocHamHocVi, thongTinNguoithucHien.NhomChucDanh, thongTinNguoithucHien.HoTen);
                    }


                    if (thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.LoaiYeuCauTiepNhan).FirstOrDefault() == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                    {
                        var yeuCauKhamBenhIds = phienXetNghiemChiTietDatas
                            .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId) && o.YeuCauKhamBenhId != null)
                            .Select(o => o.YeuCauKhamBenhId.Value)
                            .ToList();

                        var noiTruPhieuDieuTris = yeuCauKhamBenhAlls
                            .Where(o => yeuCauKhamBenhIds.Contains(o.Id) && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.ChanDoanSoBoICDId != null)
                            .Select(o => new
                            {
                                o.Id,
                                ChanDoan = o.ChanDoan,
                                DienGiai = o.DienGiai
                            }).ToList();
                        data.ChanDoan = noiTruPhieuDieuTris.Where(p => p.ChanDoan != null && p.ChanDoan != "-" && p.ChanDoan != "").OrderBy(o => o.Id).LastOrDefault()?.ChanDoan;
                        data.DienGiai = noiTruPhieuDieuTris.OrderBy(o => o.Id).LastOrDefault()?.DienGiai;
                    }
                    else if (thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.LoaiYeuCauTiepNhan).FirstOrDefault() == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                    {
                        var noiTruPhieuDieuTriIds = phienXetNghiemChiTietDatas
                            .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId) && o.NoiTruPhieuDieuTriId != null)
                            .Select(o => o.NoiTruPhieuDieuTriId.Value)
                            .ToList();

                        var noiTruPhieuDieuTris = noiTruPhieuDieuTriAlls
                            .Where(o => noiTruPhieuDieuTriIds.Contains(o.Id))
                            .Select(o => new
                            {
                                o.Id,
                                ChanDoan = o.ChanDoan,
                                DienGiai = o.DienGiai
                            }).ToList();
                        data.ChanDoan = noiTruPhieuDieuTris.Where(p => p.ChanDoan != null && p.ChanDoan != "-" && p.ChanDoan != "").OrderBy(o => o.Id).LastOrDefault()?.ChanDoan;
                        data.DienGiai = noiTruPhieuDieuTris.OrderBy(o => o.Id).LastOrDefault()?.DienGiai;
                    }
                    else if (thongTinPhienXetNghiem.Where(d => d.Id == thongTin.PhienXetNghiemId).Select(d => d.LoaiYeuCauTiepNhan).FirstOrDefault() == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                    {
                        data.ChanDoan = "";
                        data.DienGiai = "";
                    }


                    var enumLoaiMauXetNghiems = phienXetNghiemChiTietDatas.Where(d=>d.PhienXetNghiemId == thongTin.PhienXetNghiemId)
                        .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId))
                        .Select(o => o.LoaiMauXetNghiem).Distinct();


                    data.LoaiMau = string.Join("; ", enumLoaiMauXetNghiems.Select(o => o.GetDescription()));


                    var phienXetNghiemChiTietLast = phienXetNghiemChiTietDatas.Where(d => d.PhienXetNghiemId == thongTin.PhienXetNghiemId)
                        .Where(o => lstYeuCauDichVuKyThuatIdTrongPhieu.Contains(o.YeuCauDichVuKyThuatId)).OrderBy(o => o.ThoiDiemNhanMau).LastOrDefault();

                    if (phienXetNghiemChiTietLast != null)
                    {
                        var nhanViens = nhanVienThucHien
                        .Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienNhanMauId || o.Id == phienXetNghiemChiTietLast.NhanVienLayMauId)
                        .Select(o => new
                        {
                            o.Id,
                            HocHamHocVi = o.HocHamHocVi,
                            NhomChucDanh = o.NhomChucDanh,
                            HoTen = o.HoTen
                        })
                        .ToList();

                        data.TgLayMau = phienXetNghiemChiTietLast.ThoiDiemLayMau?.ApplyFormatDateTimeSACH();
                        if (phienXetNghiemChiTietLast.NhanVienLayMauId != null)
                        {
                            var nguoiLayMau = nhanViens.Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienLayMauId).FirstOrDefault();
                            data.NguoiLayMau = returnStringTen(nguoiLayMau?.HocHamHocVi, nguoiLayMau?.NhomChucDanh, nguoiLayMau?.HoTen);
                        }
                        data.TgNhanMau = phienXetNghiemChiTietLast.ThoiDiemNhanMau?.ApplyFormatDateTimeSACH();

                        if (phienXetNghiemChiTietLast.NhanVienNhanMauId != null)
                        {
                            var nguoiNhanMau = nhanViens.Where(o => o.Id == phienXetNghiemChiTietLast.NhanVienNhanMauId).FirstOrDefault();
                            data.NguoiNhanMau = returnStringTen(nguoiNhanMau?.HocHamHocVi, nguoiNhanMau?.NhomChucDanh, nguoiNhanMau?.HoTen);
                        }
                    }
                    if (phienXetNghiemChiTietLast != null)
                    {
                        data.Ngay = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                        ngay = data.Ngay;
                        data.Thang = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                        thang = data.Thang;
                        data.Nam = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                        nam = data.Nam;
                        data.Gio = phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";
                        gio = data.Gio;
                    }
                    data.NgayThangNamFooter = DateTime.Now.ApplyFormatDateTimeSACH();
                    var lstContentItem = new PhieuInXetNghiemModel();
                    // 
                    if (!string.IsNullOrEmpty(data.DanhSach))
                    {
                        if (listGhiChuTheoDVXNISOs.Count() != 0)
                        {
                            data.LogoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                            data.LogoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";
                        }

                        lstContentItem.Html = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, data);

                        #region BVHD-3919
                       

                        data.Ngay = data.Ngay == null ? ngay : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Day + "";
                        data.Thang = data.Thang == null ? thang : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Month + "";
                        data.Nam = data.Nam == null ? nam : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Year + "";
                        data.Gio = data.Gio == null ? gio : phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Hour + " giờ " + phienXetNghiemChiTietLast?.ThoiDiemKetLuan?.Minute + " phút";

                        //Phần ghi chú sắp xếp như sau:
                        //Dòng đầu tiên là dòng ghi chú mặc định (footer template)
                        //Dòng thứ 2 là ghi chú ISO (ghiChu) 
                        //Dòng thứ 3 là ghi chú Dịch vụ gửi ngoài (ghiChu) 
                        //Dòng thứ 4 là dòng ghi chú của dịch vụ  (Ghi chú DV)

                        var ghiChu = string.Empty;
                        var logoBV1 = string.Empty;
                        var logoBV2 = string.Empty;



                        if (listGhiChuTheoDVXNISOs.Count() != 0)
                        {

                            if (!string.IsNullOrEmpty(ghiChu))
                            {
                                ghiChu += "<br>";
                            }
                            //update 16/05: - Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi  THAY ĐỔI THÀNH  - Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.
                            //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";
                            ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012.</span>";

                            logoBV1 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha1-full.png";
                            logoBV2 = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha2-full.png";

                            data.LogoBV1 = logoBV1;
                            data.LogoBV2 = logoBV2;
                        }

                        if (listGhiChuTheoDVChuyenGois.Count() != 0)
                        {
                            if (!string.IsNullOrEmpty(ghiChu))
                            {
                                ghiChu += "<br>";
                            }
                            //update 16/05: - Chỉ số XN CÓ dấu * là xét nghiệm được công nhận ISO 15189:2012 THAY ĐỔI THÀNH - Chỉ số xét nghiệm đánh dấu * là xét nghiệm được công nhận ISO 15189:2012
                            //ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số XN CÓ dấu ** là xét nghiệm chuyển gửi.</span>";
                            ghiChu += "<span style='font-size: 17px;font-weight:normal;'>- Chỉ số xét nghiệm đánh dấu ** là xét nghiệm chuyển gửi.</span>";
                        }

                        data.GhiChu = (!string.IsNullOrEmpty(ghiChu) ? ghiChu + "<br>" : "") + (!string.IsNullOrEmpty(data.GhiChuDV) ? "- " + data.GhiChuDV : "");


                        var htmlfooter = TemplateHelpper.FormatTemplateWithContentTemplate(templatefootertableXetNghiem.Body, data);
                        lstContentItem.Html += htmlfooter;
                        lstContentItem.Html += "<div style='margin-left: 2cm;'><table width='100%'><tr><td style='width:36%'></td><td style='width:35%'></td><td style='width:29%'><b>" + data.NguoiThucHien + "</b></td>";
                        lstContentItem.Html += "</tr></table></div>";

                        #endregion BVHD-3919

                        lstContent.Add(lstContentItem);
                    }
                    numberPage++;
                }

            }

            return lstContent;
        }
    }
}
