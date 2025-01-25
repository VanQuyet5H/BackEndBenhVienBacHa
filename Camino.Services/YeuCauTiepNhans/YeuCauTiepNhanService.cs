using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Core.Domain.Entities.KetQuaVaKetLuanMaus;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;

namespace Camino.Services.YeuCauTiepNhans
{
    [ScopedDependency(ServiceType = typeof(IYeuCauTiepNhanService))]
    public partial class YeuCauTiepNhanService : YeuCauTiepNhanBaseService, IYeuCauTiepNhanService
    {
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<CongTyBaoHiemTuNhan> _congTyBaoHiemTuNhanRepository;
        private readonly IRepository<PhongBenhVienHangDoi> _phongBenhVienHangDoi;
        private readonly IRepository<User> _useRepository;
        private readonly IRepository<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThu;
        private readonly IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChi;
        private readonly IRepository<TaiKhoanBenhNhan> _taiKhoanBenhNhan;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;

        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private readonly IRepository<TheVoucher> _theVoucher;
        private readonly IRepository<TaiKhoanBenhNhanHuyDichVu> _taiKhoanBenhNhanHuyDichVu;
        private readonly IRepository<TheVoucherYeuCauTiepNhan> _theVoucherYeuCauTiepNhan;
        private readonly IRepository<MienGiamChiPhi> _mienGiamChiPhiRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Core.Domain.Entities.Vouchers.Voucher> _voucherRepository;
        private readonly IRepository<PhongBenhVienHangDoi> _phongBenhVienHangDoiRepository;
        private IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private IRepository<Camino.Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem> _ketQuaNhomXetNghiemRepository;
        private IRepository<FileKetQuaCanLamSang> _fileKetQuaCanLamSangRepository;
        private IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private IRepository<Core.Domain.Entities.KetQuaVaKetLuanMaus.KetQuaVaKetLuanMau> _ketQuaVaKetLuanMauRepository;
        private IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> _inputStringStoredRepository;
        private IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private IRepository<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiamRepository;
        private IRepository<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> _noiGioiThieuRepository;
        private IRepository<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;
        private readonly IPdfService _pdfService;

        public YeuCauTiepNhanService(IRepository<User> useRepository, IRepository<YeuCauTiepNhan> repository, IRepository<PhongBenhVienHangDoi> phongBenhVienHangDoi, IRepository<CongTyBaoHiemTuNhan> congTyBaoHiemTuNhanRepository,
              IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThu, IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository,
              IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository, ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
              IRepository<Camino.Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem> ketQuaNhomXetNghiemRepository,
              IRepository<TaiKhoanBenhNhanHuyDichVu> taiKhoanBenhNhanHuyDichVu, ILocalizationService localizationService,
              IRepository<PhongBenhVienHangDoi> phongBenhVienHangDoiRepository, IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
              IRepository<Core.Domain.Entities.Vouchers.Voucher> voucherRepository, ICauHinhService cauHinhService, IRepository<TheVoucher> theVoucher, IRepository<FileKetQuaCanLamSang> fileKetQuaCanLamSangRepository,
              IRepository<TheVoucherYeuCauTiepNhan> theVoucherYeuCauTiepNhan, IRepository<MienGiamChiPhi> mienGiamChiPhiRepository,
              IRepository<TaiKhoanBenhNhan> taiKhoanBenhNhan,
              IPdfService pdfService,
              IRepository<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> noiGioiThieuRepository,
              IRepository<NoiGioiThieuChiTietMienGiam> noiGioiThieuChiTietMienGiamRepository,
              IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
              IRepository<Template> templateRepository, IUserAgentHelper userAgentHelper, IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChi,
              IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository, IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository, IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
              IRepository<Core.Domain.Entities.KetQuaVaKetLuanMaus.KetQuaVaKetLuanMau> ketQuaVaKetLuanMauRepository,
              IRepository<DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
              IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
        IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> inputStringStoredRepository) : base(repository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        {
            _templateRepository = templateRepository;
            _phongBenhVienHangDoi = phongBenhVienHangDoi;
            _userAgentHelper = userAgentHelper;
            _useRepository = useRepository;
            _taiKhoanBenhNhanThu = taiKhoanBenhNhanThu;
            _khoaPhongRepository = khoaPhongRepository;
            _taiKhoanBenhNhanChi = taiKhoanBenhNhanChi;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _taiKhoanBenhNhanHuyDichVu = taiKhoanBenhNhanHuyDichVu;
            _congTyBaoHiemTuNhanRepository = congTyBaoHiemTuNhanRepository;
            _benhVienRepository = benhVienRepository;
            _voucherRepository = voucherRepository;
            _cauHinhService = cauHinhService;
            _theVoucher = theVoucher;
            _theVoucherYeuCauTiepNhan = theVoucherYeuCauTiepNhan;
            _mienGiamChiPhiRepository = mienGiamChiPhiRepository;
            _phongBenhVienHangDoiRepository = phongBenhVienHangDoiRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _ketQuaNhomXetNghiemRepository = ketQuaNhomXetNghiemRepository;
            _taiKhoanBenhNhan = taiKhoanBenhNhan;
            _fileKetQuaCanLamSangRepository = fileKetQuaCanLamSangRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _ketQuaVaKetLuanMauRepository = ketQuaVaKetLuanMauRepository;
            _inputStringStoredRepository = inputStringStoredRepository;
            _pdfService = pdfService;
            _phongBenhVienRepository = phongBenhVienRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _noiGioiThieuRepository = noiGioiThieuRepository;
            _noiGioiThieuChiTietMienGiamRepository = noiGioiThieuChiTietMienGiamRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
        }
    }
}
