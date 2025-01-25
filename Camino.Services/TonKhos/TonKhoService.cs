using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TonKhos;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Services.CauHinh;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.DinhMucVatTuTonKhos;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Services.Helpers;
using Camino.Services.YeuCauKhamBenh;

namespace Camino.Services.TonKhos
{
    [ScopedDependency(ServiceType = typeof(ITonKhoService))]
    public partial class TonKhoService : MasterFileService<Core.Domain.Entities.KhoDuocPhams.Kho>, ITonKhoService
    {
        IRepository<DuocPham> _duocPhamRepository;
        IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        IRepository<Kho> _khoDuocPhamRepository;
        IRepository<NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        IRepository<XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        IRepository<XuatKhoDuocPhamChiTiet> _xuatKhoDuocPhamChiTietRepository;
        IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        IRepository<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTietRepository;
        IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;
        IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        IRepository<DinhMucVatTuTonKho> _dinhMucVatTuTonKhoRepository;
        IRepository<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho> _dinhMucDuocPhamTonKhoRepository;
        IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;
        IRepository<Core.Domain.Entities.NhomVatTus.NhomVatTu> _nhomVatTuRepository;

        IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;

        IRepository<Template> _templateRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ICauHinhService _cauhinhService;


        public TonKhoService(IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> repository
            , IRepository<Template> templateRepository, ICauHinhService cauhinhService, IRepository<DuocPham> duocPhamRepository
            , IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository
            , IRepository<Core.Domain.Entities.NhomVatTus.NhomVatTu> nhomVatTuRepository
            , IRepository<NhapKhoDuocPham> nhapKhoDuocPhamRepository, IRepository<Kho> khoDuocPhamRepository
            , IRepository<XuatKhoDuocPham> xuatKhoDuocPhamRepository, IRepository<XuatKhoDuocPhamChiTiet> xuatKhoDuocPhamChiTietRepository, IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository
            , IRepository<XuatKhoVatTu> xuatKhoVatTuRepository
            , IRepository<XuatKhoVatTuChiTiet> xuatKhoVatTuChiTietRepository, IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository
            , IRepository<NhapKhoVatTu> nhapKhoVatTuRepository
            , IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository
            , IRepository<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho> dinhMucDuocPhamTonKhoRepository
            , IRepository<DinhMucVatTuTonKho> dinhMucVatTuTonKhoRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository
            ,
            IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository
            ) : base(repository)
        {
            _duocPhamRepository = duocPhamRepository;
            _vatTuRepository = vatTuRepository;
            _khoDuocPhamRepository = khoDuocPhamRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _cauhinhService = cauhinhService;
            _templateRepository = templateRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _xuatKhoDuocPhamChiTietRepository = xuatKhoDuocPhamChiTietRepository;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _xuatKhoVatTuChiTietRepository = xuatKhoVatTuChiTietRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _dinhMucVatTuTonKhoRepository = dinhMucVatTuTonKhoRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;
            _userAgentHelper = userAgentHelper;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _nhomVatTuRepository = nhomVatTuRepository;
            _dinhMucDuocPhamTonKhoRepository = dinhMucDuocPhamTonKhoRepository;
        }
        
    }
}
