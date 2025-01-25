using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhDuocPham;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.YeuCauLinhKSNK
{
    [ScopedDependency(ServiceType = typeof(IYeuCauLinhKSNKService))]
    public partial class YeuCauLinhKSNKService : MasterFileService<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu>, IYeuCauLinhKSNKService
    {
        private readonly IRepository<Template> _templateRepository;

        private readonly IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        private readonly IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> _khoRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTuChiTiet> _yeuCauLinhVatTuChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu> _yeuCauLinhVatTuRepository;
        private readonly IRepository<Core.Domain.Entities.NhapKhoVatTus.NhapKhoVatTu> _nhapKhoVatTuRepository;
        private readonly IRepository<Core.Domain.Entities.XuatKhoVatTus.XuatKhoVatTu> _xuatKhoVatTuRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Core.Domain.Entities.NhapKhoVatTus.NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> _vatTuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.Localization.LocaleStringResource> _localeStringResourceRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;

        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<Template> _templateRepo;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;


        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;

        private readonly IRepository<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham> _yeuCauLinhDuocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet> _yeuCauLinhDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.NhapKhoDuocPhams.NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        public YeuCauLinhKSNKService(
           IRepository<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu> repository,
           IRepository<Template> templateRepository,
           IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository,
           IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> vatTuBenhVienRepository,
           IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
           IRepository<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTuChiTiet> yeuCauLinhVatTuChiTietRepository,
           IRepository<Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu> yeuCauLinhVatTuRepository,
           IRepository<Core.Domain.Entities.NhapKhoVatTus.NhapKhoVatTu> nhapKhoVatTuRepository,
           IRepository<Core.Domain.Entities.XuatKhoVatTus.XuatKhoVatTu> xuatKhoVatTuRepository,
           IRepository<User> userRepository,
           IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> khoRepository,
           IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienRepository,
           IRepository<Core.Domain.Entities.NhapKhoVatTus.NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
           IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
           IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository,
           IRepository<Core.Domain.Entities.Localization.LocaleStringResource> localeStringResourceRepository,
           ILocalizationService localizationService,
           IUserAgentHelper userAgentHelper,
           IRepository<Template> templateRepo,
           IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
           ICauHinhService cauHinhService,
           IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
           IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
           IRepository<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
           IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham> yeuCauLinhDuocPhamRepository,
           IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet> yeuCauLinhDuocPhamChiTietRepository,
           IRepository<Core.Domain.Entities.NhapKhoDuocPhams.NhapKhoDuocPham> nhapKhoDuocPhamRepository,
           IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
           IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository
           ) : base(repository)
        {
            _templateRepository = templateRepository;
            _vatTuRepository = vatTuRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _khoRepository = khoRepository;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _yeuCauLinhVatTuChiTietRepository = yeuCauLinhVatTuChiTietRepository;
            _yeuCauLinhVatTuRepository = yeuCauLinhVatTuRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _userRepository = userRepository;
            _khoaPhongNhanVienRepository = khoaPhongNhanVienRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _localeStringResourceRepository = localeStringResourceRepository;
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _templateRepo = templateRepo;
            _nhanVienRepository = nhanVienRepository;
            _cauHinhService = cauHinhService;
            _khoaPhongRepository = khoaPhongRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _yeuCauLinhDuocPhamRepository = yeuCauLinhDuocPhamRepository;
            _yeuCauLinhDuocPhamChiTietRepository = yeuCauLinhDuocPhamChiTietRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _cauHinhRepository = cauHinhRepository;

        }
    }
}
