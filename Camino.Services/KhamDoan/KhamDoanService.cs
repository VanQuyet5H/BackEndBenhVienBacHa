using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.Localization;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Services.KhamDoan
{
    [ScopedDependency(ServiceType = typeof(IKhamDoanService))]
    [System.Runtime.InteropServices.Guid("500EFD51-AB61-4596-BF12-C84FF07DB8AD")]
    public partial class KhamDoanService : YeuCauTiepNhanBaseService, IKhamDoanService
    {
        private readonly IRepository<CongTyKhamSucKhoe> _congTyKhamSucKhoeRepository;
        private readonly IRepository<HopDongKhamSucKhoe> _hopDongKhamSucKhoeRepository;
        private readonly IRepository<HopDongKhamSucKhoeDiaDiem> _hopDongKhamSucKhoeDiaDiemRepository;
        private readonly IRepository<KetQuaSinhHieu> _ketQuaSinhHieuRepository;
        private readonly IRepository<GoiKhamSucKhoeDichVuKhamBenh> _goiKhamSucKhoeDichVuKhamBenhRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<GoiKhamSucKhoeDichVuDichVuKyThuat> _goiKhamSucKhoeDichVuDichVuKyThuatRepository;
        private readonly IRepository<NhomGiaDichVuKhamBenhBenhVien> _nhomGiaDichVuKhamBenhBenhVienRepository;
        private readonly IRepository<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IRepository<NhomGiaDichVuKyThuatBenhVien> _nhomGiaDichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienRepository;
        private readonly IRepository<YeuCauNhanSuKhamSucKhoe> _yeuCauNhanSuKhamSucKhoeRepository;
        private readonly IRepository<GoiKhamSucKhoe> _goiKhamSucKhoeRepository;
        private readonly IRepository<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanVienRepository;
        private readonly IRepository<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private readonly IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;
        private readonly IRepository<DichVuKhamBenhBenhVienNoiThucHien> _dichVuKhamBenhBenhVienNoiThucHienRepository;
        private readonly IRepository<DichVuKyThuatBenhVienNoiThucHien> _dichVuKyThuatBenhVienNoiThucHienRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<GoiKhamSucKhoeNoiThucHien> _goiKhamSucKhoeNoiThucHienRepository;
        private readonly IRepository<NhomGiaDichVuKhamBenhBenhVien> _nhomGiaDichVuKhamBenhRepository;
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        IRepository<Template> _templateRepository;
        private readonly IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> _inputStringStoredRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiem> _phienXetNghiemRepository;
        IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;
        IRepository<Camino.Core.Domain.Entities.XetNghiems.KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietRepository;
        IRepository<TemplateDichVuKhamSucKhoe> _templateDichVuKhamSucKhoeRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSoRepository;
        private readonly IRepository<ICD> _icdRepository;
        private readonly IRepository<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository;
        private readonly IRepository<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository;
        //Cập nhật xong gói khám chung 14/09/2021.
        private readonly IRepository<GoiKhamSucKhoeChung> _goiKhamChungSucKhoeRepository;
        private readonly IRepository<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuRepository;

        private IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> _hocViHocHamRepository;

        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly IRepository<Core.Domain.Entities.ICDs.ICD> _iCDRepository;

        private readonly IRepository<PhongBenhVienHangDoi> _phongBenhVienHangDoiRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> _dichVuXetNghiemRepository;
        private readonly IRepository<Core.Domain.Entities.MayXetNghiems.MayXetNghiem> _mayXetNghiemRepository;

        private readonly IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> _yeuCauDichVuKyThuatTuongTrinhPTTTRepository;
        public KhamDoanService(
            IRepository<YeuCauTiepNhan> repository,
            IUserAgentHelper userAgentHelper,
            ICauHinhService cauHinhService,
            ILocalizationService localizationService,
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            IRepository<HopDongKhamSucKhoeDiaDiem> hopDongKhamSucKhoeDiaDiemRepository,
            IRepository<CongTyKhamSucKhoe> congTyKhamSucKhoeRepository,
            IRepository<HopDongKhamSucKhoe> hopDongKhamSucKhoeRepository,
            IRepository<GoiKhamSucKhoeDichVuKhamBenh> goiKhamSucKhoeDichVuKhamBenhRepository,
            IRepository<GoiKhamSucKhoeDichVuDichVuKyThuat> goiKhamSucKhoeDichVuDichVuKyThuatRepository,
            IRepository<NhomGiaDichVuKhamBenhBenhVien> nhomGiaDichVuKhamBenhBenhVienRepository,
            IRepository<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
            IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<YeuCauNhanSuKhamSucKhoe> yeuCauNhanSuKhamSucKhoeRepository,
            IRepository<GoiKhamSucKhoe> goiKhamSucKhoeRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<NhomGiaDichVuKyThuatBenhVien> nhomGiaDichVuKyThuatBenhVienRepository,
            IRepository<DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
            IRepository<HopDongKhamSucKhoeNhanVien> hopDongKhamSucKhoeNhanVienRepository,
            IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
            IRepository<User> userRepository,
            IRepository<DichVuKhamBenhBenhVienNoiThucHien> dichVuKhamBenhBenhVienNoiThucHienRepository,
            IRepository<DichVuKyThuatBenhVienNoiThucHien> dichVuKyThuatBenhVienNoiThucHienRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<GoiKhamSucKhoeNoiThucHien> goiKhamSucKhoeNoiThucHienRepository,
            IRepository<NhomGiaDichVuKhamBenhBenhVien> nhomGiaDichVuKhamBenhRepository,
            IRepository<Template> templateRepository,
            IRepository<KetQuaSinhHieu> ketQuaSinhHieuRepository,
            IRepository<LocaleStringResource> localeStringResourceRepository,
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
            IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> inputStringStoredRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
            IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiem> phienXetNghiemRepository,
            IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
            IRepository<Camino.Core.Domain.Entities.XetNghiems.KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTietRepository,
            IRepository<TemplateDichVuKhamSucKhoe> templateDichVuKhamSucKhoeRepository,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
            IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSoRepository,
            IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> yeuCauTiepNhanRepository,
            IRepository<ICD> icdRepository,
            IRepository<GoiKhamSucKhoeChung> goiKhamChungSucKhoeRepository,
            IRepository<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository,
            IRepository<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository,
            IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> hocViHocHamRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            IRepository<Core.Domain.Entities.ICDs.ICD> iCDRepository,
            IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThuRepository,
            IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> dichVuXetNghiemRepository,
            IRepository<Core.Domain.Entities.MayXetNghiems.MayXetNghiem> mayXetNghiemRepository,
            IRepository<PhongBenhVienHangDoi> phongBenhVienHangDoiRepository,
            IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> yeuCauDichVuKyThuatTuongTrinhPTTTRepository
            ) : base(repository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        {
            _ketQuaSinhHieuRepository = ketQuaSinhHieuRepository;
            _congTyKhamSucKhoeRepository = congTyKhamSucKhoeRepository;
            _hopDongKhamSucKhoeRepository = hopDongKhamSucKhoeRepository;
            _goiKhamSucKhoeDichVuKhamBenhRepository = goiKhamSucKhoeDichVuKhamBenhRepository;
            _goiKhamSucKhoeDichVuDichVuKyThuatRepository = goiKhamSucKhoeDichVuDichVuKyThuatRepository;
            _nhomGiaDichVuKhamBenhBenhVienRepository = nhomGiaDichVuKhamBenhBenhVienRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _khoaPhongNhanVienRepository = khoaPhongNhanVienRepository;
            _yeuCauNhanSuKhamSucKhoeRepository = yeuCauNhanSuKhamSucKhoeRepository;
            _goiKhamSucKhoeRepository = goiKhamSucKhoeRepository;
            _hopDongKhamSucKhoeNhanVienRepository = hopDongKhamSucKhoeNhanVienRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _nhomGiaDichVuKyThuatBenhVienRepository = nhomGiaDichVuKyThuatBenhVienRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _dichVuKhamBenhBenhVienNoiThucHienRepository = dichVuKhamBenhBenhVienNoiThucHienRepository;
            _dichVuKyThuatBenhVienNoiThucHienRepository = dichVuKyThuatBenhVienNoiThucHienRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _goiKhamSucKhoeNoiThucHienRepository = goiKhamSucKhoeNoiThucHienRepository;
            _nhomGiaDichVuKhamBenhRepository = nhomGiaDichVuKhamBenhRepository;
            _templateRepository = templateRepository;
            _localeStringResourceRepository = localeStringResourceRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _inputStringStoredRepository = inputStringStoredRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _hopDongKhamSucKhoeDiaDiemRepository = hopDongKhamSucKhoeDiaDiemRepository;
            _nhanVienRepository = nhanVienRepository;
            _userRepository = userRepository;
            _phienXetNghiemRepository = phienXetNghiemRepository;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _ketQuaXetNghiemChiTietRepository = ketQuaXetNghiemChiTietRepository;
            _templateDichVuKhamSucKhoeRepository = templateDichVuKhamSucKhoeRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _dichVuXetNghiemKetNoiChiSoRepository = dichVuXetNghiemKetNoiChiSoRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _icdRepository = icdRepository;
            _goiKhamChungSucKhoeRepository = goiKhamChungSucKhoeRepository;
            _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository = goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository;
            _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository = goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository;
            _hocViHocHamRepository = hocViHocHamRepository;
            _cauHinhRepository = cauHinhRepository;
            _iCDRepository = iCDRepository;
            _taiKhoanBenhNhanThuRepository = taiKhoanBenhNhanThuRepository;
            _phongBenhVienHangDoiRepository = phongBenhVienHangDoiRepository;
            _dichVuXetNghiemRepository = dichVuXetNghiemRepository;
            _mayXetNghiemRepository = mayXetNghiemRepository;                
            _yeuCauDichVuKyThuatTuongTrinhPTTTRepository = yeuCauDichVuKyThuatTuongTrinhPTTTRepository;
        }

        #region Get lookup
        public async Task<List<LookupItemTemplateVo>> GetCongTy(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(CongTyKhamSucKhoe.Ma),
                nameof(CongTyKhamSucKhoe.Ten),
            };
            var lstCongTys = new List<LookupItemTemplateVo>();
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstCongTys = await _congTyKhamSucKhoeRepository.TableNoTracking
                    .Where(x => x.CoHoatDong == true && x.Id == queryInfo.Id)
                    .Select(item => new LookupItemTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = item.Ma,
                    })
                    .Union(
                        _congTyKhamSucKhoeRepository.TableNoTracking
                        .Where(x => x.CoHoatDong == true && x.Id != queryInfo.Id)
                        .Select(item => new LookupItemTemplateVo
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            Ten = item.Ten,
                            Ma = item.Ma,
                        }))
                        .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                        .OrderByDescending(x => x.KeyId == queryInfo.Id).ThenBy(x => x.DisplayName)
                        .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstCongTyId = await _congTyKhamSucKhoeRepository
                    .ApplyFulltext(queryInfo.Query, nameof(CongTyKhamSucKhoe), lstColumnNameSearch)
                    .Where(x => x.CoHoatDong == true && queryInfo.Id == 0 || x.Id == queryInfo.Id)
                    .Select(x => x.Id)
                    .ToListAsync();
                lstCongTys = await _congTyKhamSucKhoeRepository.TableNoTracking
                    .Where(x => x.CoHoatDong == true && lstCongTyId.Contains(x.Id))
                    .OrderByDescending(x => x.Id == queryInfo.Id)
                    .ThenBy(p => lstCongTyId.IndexOf(p.Id) != -1 ? lstCongTyId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new LookupItemTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = item.Ma,
                    }).ToListAsync();
            }
            return lstCongTys;
        }
        public async Task<List<LookupItemTemplateVo>> GetHopDong(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(HopDongKhamSucKhoe.SoHopDong),
            };
            var lstHopDongs = new List<LookupItemTemplateVo>();
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstHopDongs = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => queryInfo.Id == 0 || x.Id == queryInfo.Id)
                    .Select(item => new LookupItemTemplateVo
                    {
                        DisplayName = item.SoHopDong,
                        KeyId = item.Id,
                    })
                    .ApplyLike(queryInfo.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == queryInfo.Id).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstHopDongId = await _hopDongKhamSucKhoeRepository
                    .ApplyFulltext(queryInfo.Query, nameof(HopDongKhamSucKhoe), lstColumnNameSearch)
                    .Where(x => queryInfo.Id == 0 || x.Id == queryInfo.Id)
                    .Select(x => x.Id)
                    .ToListAsync();
                lstHopDongs = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => lstHopDongId.Contains(x.Id))
                    .OrderByDescending(x => x.Id == queryInfo.Id)
                    .ThenBy(p => lstHopDongId.IndexOf(p.Id) != -1 ? lstHopDongId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new LookupItemTemplateVo
                    {
                        DisplayName = item.SoHopDong,
                        KeyId = item.Id,
                    }).ToListAsync();
            }
            return lstHopDongs;
        }

        public async Task<List<LookupItemTemplateVo>> GetListBacSiAsync(DropDownListRequestModel queryInfo)
        {
            var lstBacSi = _nhanVienRepository.TableNoTracking
                    .Select(item => new LookupItemTemplateVo
                    {
                        DisplayName = item.User.HoTen,
                        KeyId = item.Id,
                    })
                .ApplyLike(queryInfo.Query, o => o.DisplayName)
                .Take(queryInfo.Take);

            return await lstBacSi.ToListAsync();
        }

        //public async Task<List<LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo>> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens(DropDownListRequestModel queryInfo, bool chonDichVuKham = true, bool chonDichVuKyThuat = true, bool fullNhomDichVu = false, List<long> dichVuKhamIds = null, List<long> dichVuKyThuatIds = null)
        public async Task<List<LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo>> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens(DropDownListRequestModel queryInfo, DichVuKhamBenhBVHoacDVKTBenhVienParams dvKBHoacDVKTBvParam = null)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(DichVuKhamBenhBenhVien.Ma),
                nameof(DichVuKhamBenhBenhVien.Ten),
                nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien.Ma),
                nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien.Ten),

            };
            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            long? nhomTiemChungId = null;
            if (dvKBHoacDVKTBvParam != null && dvKBHoacDVKTBvParam.KhongLayTiemChung)
            {
                var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
                nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;
            }

            var lstDichVuKhamBenhBenhVienDichVuKTBvs = new List<LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo>();
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstDichVuKhamBenhBenhVienDichVuKTBvs = _dichVuKhamBenhBenhVienRepository.TableNoTracking
               .Where(x => (dvKBHoacDVKTBvParam != null && (dvKBHoacDVKTBvParam.CoGoiPhatSinh || (!dvKBHoacDVKTBvParam.CoGoiPhatSinh && x.ChuyenKhoaKhamSucKhoe != null)))
                           && x.HieuLuc
                           && ((dvKBHoacDVKTBvParam != null && dvKBHoacDVKTBvParam.ChonDichVuKham && (dvKBHoacDVKTBvParam.DichVuKhamIds == null || dvKBHoacDVKTBvParam.DichVuKhamIds.Contains(x.Id))) || x.Id == 0))
               .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                   .Select(item => new LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo
                   {
                       DisplayName = item.Ten,
                       KeyId = item.Id,
                       Ten = item.Ten,
                       Ma = item.Ma,
                       Loai = 1,
                       ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                       //NhomDichVu = NhomDichVuChiDinhKhamSucKhoe.KhamBenh
                   })
                .Union(
                  _dichVuKyThuatBenhVienRepository.TableNoTracking
                   .Where(x => x.HieuLuc
                               && ((dvKBHoacDVKTBvParam != null && dvKBHoacDVKTBvParam.FullNhomDichVu)
                                   ||
                                   //(x.NhomDichVuBenhVien.Ma == "XN"
                                   //     || x.NhomDichVuBenhVien.Ma == "CĐHA"
                                   //     || x.NhomDichVuBenhVien.Ma == "TDCN"
                                   //     || x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                                   //     || x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "CĐHA"
                                   //     || x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "TDCN")
                                   CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVienId, lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.XetNghiem
                                   || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVienId, lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                   || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVienId, lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.ThamDoChucNang
                                   || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId.GetValueOrDefault(), lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.XetNghiem
                                   || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId.GetValueOrDefault(), lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                   || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId.GetValueOrDefault(), lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.ThamDoChucNang
                                        )
                               && ((dvKBHoacDVKTBvParam != null && dvKBHoacDVKTBvParam.ChonDichVuKyThuat && (dvKBHoacDVKTBvParam.DichVuKyThuatIds == null || dvKBHoacDVKTBvParam.DichVuKyThuatIds.Contains(x.Id))) || x.Id == 0)

                               // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
                               && (nhomTiemChungId == null || x.NhomDichVuBenhVienId != nhomTiemChungId))
                   .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                   .Select(item => new LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo
                   {
                       DisplayName = item.Ten,
                       KeyId = item.Id,
                       Ten = item.Ten,
                       Ma = item.Ma,
                       Loai = 2,
                       MaNhomDichVuBenhVien = item.NhomDichVuBenhVien.Ma,
                       MaNhomDichVuBenhVienCha = item.NhomDichVuBenhVien.NhomDichVuBenhVienCha != null ? item.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma : "",
                       LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                       //NhomDichVu = item.NhomDichVuBenhVien.Ma == "XN" ? NhomDichVuChiDinhKhamSucKhoe.XetNghiem : (item.NhomDichVuBenhVien.Ma == "CĐHA" ? NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh : NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang)
                   })
                   .OrderByDescending(x => x.KeyId == queryInfo.Id).ThenBy(x => x.KeyId)
                   .Take(queryInfo.Take)).ToList();
            }
            else
            {
                var lstDvKhamBenhBVId = await _dichVuKhamBenhBenhVienRepository
                    .ApplyFulltext(queryInfo.Query, nameof(DichVuKhamBenhBenhVien), lstColumnNameSearch)
                      .Select(s => s.Id).ToListAsync();

                var lstDvKTBVId = await _dichVuKyThuatBenhVienRepository
                     .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien), lstColumnNameSearch)
                     .Select(s => s.Id).ToListAsync();

                var lstDVKhamBenhKyThuatId = lstDvKhamBenhBVId.Concat(lstDvKTBVId);

                //var dct = lstDVKhamBenhKyThuatId.Select((p, i) => new
                //{
                //    key = p,
                //    rank = i
                //}).ToDictionary(o => o.key, o => o.rank);

                var dctDVKB = lstDvKhamBenhBVId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                var dctDVKT = lstDvKTBVId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                lstDichVuKhamBenhBenhVienDichVuKTBvs = _dichVuKhamBenhBenhVienRepository
                                      .ApplyFulltext(queryInfo.Query, nameof(DichVuKhamBenhBenhVien), lstColumnNameSearch)
                                      .Where(x => (dvKBHoacDVKTBvParam != null && (dvKBHoacDVKTBvParam.CoGoiPhatSinh || (!dvKBHoacDVKTBvParam.CoGoiPhatSinh && x.ChuyenKhoaKhamSucKhoe != null)))
                                                  && x.HieuLuc
                                                  && ((dvKBHoacDVKTBvParam != null && dvKBHoacDVKTBvParam.ChonDichVuKham && (dvKBHoacDVKTBvParam.DichVuKhamIds == null || dvKBHoacDVKTBvParam.DichVuKhamIds.Contains(x.Id))) || x.Id == 0))
                                          .Select(item => new LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo
                                          {
                                              DisplayName = item.Ten,
                                              KeyId = item.Id,
                                              Ten = item.Ten,
                                              Ma = item.Ma,
                                              Loai = 1,
                                              ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                                          })
                                        .OrderBy(p => dctDVKB.Any(a => a.Key == p.KeyId) ? dctDVKB[p.KeyId] : dctDVKB.Count)
                                       .Union(
                                         _dichVuKyThuatBenhVienRepository
                                          .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien), lstColumnNameSearch)
                                          .Where(x => x.HieuLuc
                                                      && ((dvKBHoacDVKTBvParam != null && dvKBHoacDVKTBvParam.FullNhomDichVu)
                                                          ||
                                                          //(x.NhomDichVuBenhVien.Ma == "XN"
                                                          //   || x.NhomDichVuBenhVien.Ma == "CĐHA"
                                                          //   || x.NhomDichVuBenhVien.Ma == "TDCN"
                                                          //   || x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN"
                                                          //   || x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "CĐHA"
                                                          //   || x.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "TDCN")
                                                          CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVienId, lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.XetNghiem
                                                       || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVienId, lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                       || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVienId, lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.ThamDoChucNang
                                                       || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId.GetValueOrDefault(), lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.XetNghiem
                                                       || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId.GetValueOrDefault(), lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                       || CalculateHelper.GetLoaiDichVuKyThuat(x.NhomDichVuBenhVien.NhomDichVuBenhVienChaId.GetValueOrDefault(), lstNhomDichVuBenhVien) == LoaiDichVuKyThuat.ThamDoChucNang
                                                             )
                                                      && ((dvKBHoacDVKTBvParam != null && dvKBHoacDVKTBvParam.ChonDichVuKyThuat && (dvKBHoacDVKTBvParam.DichVuKyThuatIds == null || dvKBHoacDVKTBvParam.DichVuKyThuatIds.Contains(x.Id))) || x.Id == 0)

                                                      // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
                                                      && (nhomTiemChungId == null || x.NhomDichVuBenhVienId != nhomTiemChungId))
                                          .Select(item => new LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo
                                          {
                                              DisplayName = item.Ten,
                                              KeyId = item.Id,
                                              Ten = item.Ten,
                                              Ma = item.Ma,
                                              Loai = 2,
                                              MaNhomDichVuBenhVien = item.NhomDichVuBenhVien.Ma,
                                              MaNhomDichVuBenhVienCha = item.NhomDichVuBenhVien.NhomDichVuBenhVienCha != null ? item.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma : "",
                                              LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),

                                          })
                                          .OrderBy(p => dctDVKT.Any(a => a.Key == p.KeyId) ? dctDVKT[p.KeyId] : dctDVKT.Count)
                                          //.OrderByDescending(x => x.KeyId == queryInfo.Id).ThenBy(x => x.KeyId
                                          //.OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                                          .Take(queryInfo.Take)).ToList();
            }
            return lstDichVuKhamBenhBenhVienDichVuKTBvs;
        }

        public async Task<List<LookupItemDVKhamBenhKyThuatBvVo>> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienTaoGoiKSKs(DropDownListRequestModel queryInfo, DichVuKhamBenhBVHoacDVKTBenhVienParams dvKBHoacDVKTBvParam = null)
        {
            var dichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens = await GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens(queryInfo, dvKBHoacDVKTBvParam);
            var lstResult = new List<LookupItemDVKhamBenhKyThuatBvVo>();
            if (dichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens.Any())
            {
                foreach (var item in dichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens)
                {
                    var itemResult = new LookupItemDVKhamBenhKyThuatBvVo
                    {
                        KeyId = JsonConvert.SerializeObject(new KeyIdStringGoiDichVuKhamSucKhoeVo()
                        {
                            DichVuId = item.KeyId,
                            Loai = item.Loai,
                        }),
                        DisplayName = item.DisplayName,
                        Ten = item.Ten,
                        Ma = item.Ma,
                        Loai = item.Loai,
                        ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                        MaNhomDichVuBenhVien = item.MaNhomDichVuBenhVien,
                        MaNhomDichVuBenhVienCha = item.MaNhomDichVuBenhVienCha,
                        LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                        NhomDichVu = item.NhomDichVu,
                        TenNhom = item.TenNhom,
                        TenLoaiDichVu = item.TenLoaiDichVu
                    };
                    lstResult.Add(itemResult);
                }
            }
            return lstResult;

        }

        public async Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> GetHopDongKhamSucKhoe(DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(HopDongKhamSucKhoe.SoHopDong),
            };
            var lstHopDongKhamSucKhoes = new List<LookupItemHopDingKhamSucKhoeTemplateVo>();
            var congTyId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstHopDongKhamSucKhoes = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => x.CongTyKhamSucKhoeId == congTyId
                               && (
                                    (!LaHopDongKetThuc && x.NgayHieuLuc.Date <= DateTime.Now.Date && (x.NgayKetThuc == null || DateTime.Now <= x.NgayKetThuc) && !x.DaKetThuc)
                                    || (LaHopDongKetThuc && (x.DaKetThuc || (x.NgayKetThuc != null && DateTime.Now.Date > x.NgayKetThuc)))
                                    )
                            )
                    .Select(item => new LookupItemHopDingKhamSucKhoeTemplateVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.SoHopDong,
                        CongTyKhamSucKhoeId = item.CongTyKhamSucKhoeId,
                        SoHopDong = item.SoHopDong,
                        NgayHieuLuc = item.NgayHieuLuc,
                        NgayKetThuc = item.NgayKetThuc
                    })
                    .ApplyLike(queryInfo.Query, x => x.SoHopDong)
                    //.OrderByDescending(x => x.CongTyKhamSucKhoeId == congTyId).ThenBy(x => x.KeyId)
                    .OrderByDescending(x => x.NgayHieuLuc).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstHopDongKhamSucKhoeId = await _hopDongKhamSucKhoeRepository
                    .ApplyFulltext(queryInfo.Query, nameof(HopDongKhamSucKhoe), lstColumnNameSearch)
                    .Where(x => x.CongTyKhamSucKhoeId == congTyId
                               && (
                                    (!LaHopDongKetThuc && x.NgayHieuLuc.Date <= DateTime.Now.Date && (x.NgayKetThuc == null || DateTime.Now <= x.NgayKetThuc) && !x.DaKetThuc)
                                    || (LaHopDongKetThuc && (x.DaKetThuc || (x.NgayKetThuc != null && DateTime.Now > x.NgayKetThuc)))
                                )
                          )
                    .Select(x => x.Id)
                    .ToListAsync();
                lstHopDongKhamSucKhoes = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => lstHopDongKhamSucKhoeId.Contains(x.Id))
                    //.OrderByDescending(x => x.CongTyKhamSucKhoeId == congTyId)
                    .OrderByDescending(x => x.NgayHieuLuc)
                    .ThenBy(p => lstHopDongKhamSucKhoeId.IndexOf(p.Id) != -1 ? lstHopDongKhamSucKhoeId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new LookupItemHopDingKhamSucKhoeTemplateVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.SoHopDong,
                        SoHopDong = item.SoHopDong,
                        NgayHieuLuc = item.NgayHieuLuc,
                        NgayKetThuc = item.NgayKetThuc
                    }).ToListAsync();
            }
            return lstHopDongKhamSucKhoes;
        }

        public async Task<List<string>> GetListNhomDoiTuongKhamSucKhoeAsync(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored.Value)
            };
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                var lstValues = _inputStringStoredRepository.TableNoTracking
                    .Where(p => p.Set == Enums.InputStringStoredKey.NhomDoiTuongKhamSucKhoe)
                    .Select(p => p.Value)
                    .ApplyLike(queryInfo.Query, o => o)
                    .Take(queryInfo.Take);

                return await lstValues.ToListAsync();
            }
            else
            {
                var lstIds = _inputStringStoredRepository
                                .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.InputStringStoreds.InputStringStored), lstColumnNameSearch)
                                .Select(p => p.Id).ToList();

                var dictionary = lstIds.Select((id, index) => new
                {
                    keys = id,
                    rank = index,
                }).ToDictionary(o => o.keys, o => o.rank);

                var lstValues = await _inputStringStoredRepository
                                        .TableNoTracking
                                        .Where(p => p.Set == Enums.InputStringStoredKey.NhomDoiTuongKhamSucKhoe)
                                        .Take(queryInfo.Take)
                                        .Select(item => new InputStringStoredTemplateVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Value,
                                            KeyId = item.Id,
                                        }).ToListAsync();
                var listValueStrings = lstValues.Select(p => p.DisplayName).ToList();
                return listValueStrings;
            }
        }

        public async Task<List<LookupItemTemplateVo>> GetKhoaPhongGoiKham(DropDownListRequestModel queryInfo, string selectedItems = null)
        {
            if (string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<KhoaPhongJsonVo>(queryInfo.ParameterDependencies);
            if (info.DichVuStringId == null || info.HopDongKhamSucKhoeId == null)
            {
                return null;
            }

            var selectedItemIds = new List<long>();
            if (!string.IsNullOrEmpty(selectedItems))
            {
                selectedItemIds = selectedItems.Split(",").Select(long.Parse).ToList();
            }
            var khoaPhongTemplateVos = _phongBenhVienRepository.TableNoTracking
                .Where(p => p.IsDisabled != true && (p.Id == queryInfo.Id || p.HopDongKhamSucKhoeId == info.HopDongKhamSucKhoeId || selectedItemIds.Any(z => z == p.Id)))
                .OrderByDescending(p => p.Id == queryInfo.Id || selectedItemIds.Any(z => z == p.Id)).ThenBy(p => p.Id)
                .Select(item => new LookupItemTemplateVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,
                    Ma = item.Ma,
                    Ten = item.Ten,
                }).ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten).Take(queryInfo.Take);
            return await khoaPhongTemplateVos.ToListAsync();
            //if (info.LaDichVuKham == true)
            //{
            //    var noiThucHiens = await _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking
            //            .Where(p => p.DichVuKhamBenhBenhVienId == info.DichVuStringId.DichVuId)
            //            .ToListAsync();
            //    var lstKhoaPhongKhamId = new List<long>();
            //    var lstPhongBenhVienId = new List<long>();

            //    if (info.DichVuStringId.DichVuId != 0 && noiThucHiens.Any())
            //    {
            //        lstKhoaPhongKhamId = noiThucHiens.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhongId.Value).ToList();
            //        lstPhongBenhVienId = noiThucHiens.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList();

            //        if (lstKhoaPhongKhamId.Any())
            //        {
            //            var lstPhongIdTheoKhoa = await _phongBenhVienRepository.TableNoTracking
            //                .Where(x => x.IsDisabled != true && lstKhoaPhongKhamId.Any(y => y == x.KhoaPhongId))
            //                .Select(x => x.Id).ToListAsync();
            //            lstPhongBenhVienId.AddRange(lstPhongIdTheoKhoa);
            //        }
            //    }
            //    else
            //    {
            //        lstPhongBenhVienId.AddRange(_phongBenhVienRepository.TableNoTracking.Where(p => p.IsDisabled != true)
            //            .Take(queryInfo.Take).Select(p => p.Id));
            //    }

            //    if (queryInfo.Id != 0)
            //    {
            //        lstPhongBenhVienId.Add(queryInfo.Id);
            //    }

            //    //multiselect
            //    var selectedItemStrs = new List<long>();
            //    if (!string.IsNullOrEmpty(selectedItems))
            //    {
            //        selectedItemStrs = selectedItems.Split(",").Select(long.Parse).ToList();
            //    }

            //    var result = await _phongBenhVienRepository.TableNoTracking
            //        .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
            //        .Where(x => x.IsDisabled != true && (lstPhongBenhVienId.Any(y => y == x.Id) || x.HopDongKhamSucKhoeId == info.HopDongKhamSucKhoeId || selectedItemStrs.Any(z => z == x.Id)))
            //        .Distinct()
            //        .OrderByDescending(x => x.Id == queryInfo.Id || selectedItemStrs.Any(z => z == x.Id)).ThenBy(x => x.Id)
            //        .Take(queryInfo.Take)
            //        .Select(item => new LookupItemTemplateVo
            //        {
            //            KeyId = item.Id,
            //            DisplayName = item.Ten,
            //            Ma = item.Ma,
            //            Ten = item.Ten,
            //        })
            //        .ToListAsync();
            //    return result;
            //}
            //else
            {
                var dvKyThuatBenhVien =
                await _dichVuKyThuatBenhVienRepository.TableNoTracking.Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).FirstOrDefaultAsync(x => x.Id == info.DichVuStringId.DichVuId);
                if (dvKyThuatBenhVien != null && dvKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens.Any())
                {
                    var lstKhoaPhongId = dvKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens
                        .Where(x => x.KhoaPhongId != null)
                        .Select(x => x.KhoaPhongId.ToString()).ToList();

                    var lstPhongBenhVienId = dvKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens
                        .Where(x => x.PhongBenhVienId != null)
                        .Select(x => x.PhongBenhVienId.ToString()).ToList();

                    if (queryInfo.Id != 0)
                    {
                        lstPhongBenhVienId.Add(queryInfo.Id.ToString());
                    }
                    var query = await _phongBenhVienRepository.TableNoTracking
                        .Where(x => x.IsDisabled != true && (lstPhongBenhVienId.Contains(x.Id.ToString()) || x.HopDongKhamSucKhoeId == info.HopDongKhamSucKhoeId || lstKhoaPhongId.Contains(x.KhoaPhongId.ToString()) || selectedItemIds.Any(z => z == x.Id)))
                        .OrderByDescending(x => x.Id == queryInfo.Id || selectedItemIds.Any(z => z == x.Id)).ThenBy(x => x.Id)
                        .Select(item => new LookupItemTemplateVo
                        {
                            KeyId = item.Id,
                            DisplayName = item.Ten,
                            Ma = item.Ma,
                            Ten = item.Ten,
                        }).ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                        .Distinct()
                        .Take(queryInfo.Take)
                        .ToListAsync();
                    return query;
                }
                else // get all phong benh vien
                {
                    var listPhong = await _phongBenhVienRepository.TableNoTracking
                        .Where(x => x.IsDisabled != true || x.Id == queryInfo.Id)
                        .OrderByDescending(x => x.Id == queryInfo.Id).ThenBy(x => x.Id)
                        .ApplyLike(queryInfo.Query, g => g.Ten, g => g.Ma).Select(item => new LookupItemTemplateVo
                        {
                            KeyId = item.Id,
                            DisplayName = item.Ten,
                            Ma = item.Ma,
                            Ten = item.Ten,
                        })
                        .Distinct()
                        .Take(queryInfo.Take)
                        .ToListAsync();
                    return listPhong;
                }
            }
        }

        public List<LookupItemVo> GetDiaDiemKhamCongTyTonTai(DropDownListRequestModel queryInfo)
        {

            var lstDiaDiemKhamCongTyTonTais = new List<LookupItemTemplateVo>();
            var congTyKham = JsonConvert.DeserializeObject<CongTyKhamSucKhoe>(queryInfo.ParameterDependencies);
            var hopDongKhamSucKhoes = _congTyKhamSucKhoeRepository.TableNoTracking.Where(c => c.Id == congTyKham.Id)
                                                                .SelectMany(cc => cc.HopDongKhamSucKhoes);
            if (hopDongKhamSucKhoes.Any())
            {

                var hopDongKhamSucKhoeDiaDiems = hopDongKhamSucKhoes.SelectMany(cc => cc.HopDongKhamSucKhoeDiaDiems)
                                                             .Select(item => new LookupItemVo
                                                             {
                                                                 KeyId = item.Id,
                                                                 DisplayName = item.DiaDiem
                                                             }).ApplyLike(queryInfo.Query, x => x.DisplayName)
                                                               .Take(queryInfo.Take).ToList();
                return hopDongKhamSucKhoeDiaDiems;
            }

            return new List<LookupItemVo>();
        }

        public async Task<List<LookupItemVo>> GetLoaiGiaDichVuKyThuat(DropDownListRequestModel model)
        {
            var lstEntity = await _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public List<LookupItemVo> GetThongTinGoiKhamTheoHopDong(DropDownListRequestModel queryInfo)
        {
            var lstDiaDiemKhamCongTyTonTais = new List<LookupItemTemplateVo>();
            var hopDongKhamSucKhoe = JsonConvert.DeserializeObject<HopDongKhamSucKhoe>(queryInfo.ParameterDependencies);

            var goiKhamTheoHopDongs = _goiKhamSucKhoeRepository.TableNoTracking.Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoe.Id
                                                                && c.GoiChung != true && c.GoiDichVuPhatSinh != true)
                                                               .Select(item => new LookupItemVo
                                                               {
                                                                   KeyId = item.Id,
                                                                   DisplayName = item.Ten
                                                               }).ApplyLike(queryInfo.Query, x => x.DisplayName)
                                                                 .ToList();

            return goiKhamTheoHopDongs;
        }


        public List<LookupItemTemplateVo> GetGoiKhamChungs(DropDownListRequestModel queryInfo)
        {
            var goiKhamChungSucKhoes = _goiKhamChungSucKhoeRepository.TableNoTracking
                                                               .Select(item => new LookupItemTemplateVo
                                                               {
                                                                   KeyId = item.Id,
                                                                   DisplayName = item.Ten,
                                                                   Ma = item.Ma,
                                                                   Ten = item.Ten,
                                                               }).ApplyLike(queryInfo.Query, x => x.DisplayName)
                                                                 .ToList();
            return goiKhamChungSucKhoes;
        }

        public bool KiemTraGoiKhamDungDungGoiChung(long goiKhamId)
        {
            var goiChung = _goiKhamSucKhoeRepository.TableNoTracking.Where(c => c.Id == goiKhamId).Any(c => c.GoiChung == true);
            return goiChung;
        }

        public List<LookupItemVo> GetPhongBenhVienTheoTen(DropDownListRequestModel model)
        {
            var lstEntity = _phongBenhVienRepository.TableNoTracking.Where(p => p.Ten.Contains(model.Query ?? ""))
                                                    .Take(model.Take)
                                                    .ToList();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public List<LookupItemVo> GetGoiKhamTheoTen(DropDownListRequestModel model)
        {
            var lstEntity = _goiKhamSucKhoeRepository.TableNoTracking.Where(p => p.Ten.Contains(model.Query ?? ""))
                                                    .Take(model.Take)
                                                    .ToList();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public List<LookupItemVo> GetPhanLoaiSucKhoe(DropDownListRequestModel model)
        {
            var listPhanLoaiSucKhoe = Enum.GetValues(typeof(PhanLoaiSucKhoe)).Cast<Enum>();
            var result = listPhanLoaiSucKhoe.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            });
            if (!string.IsNullOrEmpty(model.Query))
            {
                result = result.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return result.ToList();
        }

        public List<LookupItemVo> GetListDanhSachNhanSuMultiSelect(DropDownListRequestModel model, long hopDongKhamSucKhoeId)
        {
            var hopDongKhamSucKhoeNhanVien = _yeuCauNhanSuKhamSucKhoeRepository.TableNoTracking
                                                                               .Where(c => c.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId
                                                                                && c.DuocNhanSuDuyet == true && c.DuocKHTHDuyet == true
                                                                                && c.DuocGiamDocDuyet == true)
                                                                               .SelectMany(c => c.YeuCauNhanSuKhamSucKhoeChiTiets);
            var listNhanVien = hopDongKhamSucKhoeNhanVien.Where(c => c.NhanVienId != null).ApplyLike(model.Query, g => g.HoTen)
                         .Select(i => new LookupItemVo
                         {
                             KeyId = (long)i.NhanVienId,
                             DisplayName = i.HoTen
                         });

            return listNhanVien.ToList();
        }

        public async Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> TimKiemHopDongConHieuLucNhanVien(DropDownListRequestModel queryInfo, string phoneNumberOrEmail)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(HopDongKhamSucKhoe.SoHopDong),
            };
            var lstHopDongKhamSucKhoes = new List<LookupItemHopDingKhamSucKhoeTemplateVo>();

            var user = _userRepository.TableNoTracking.FirstOrDefault(o => (o.SoDienThoai == phoneNumberOrEmail || o.Email == phoneNumberOrEmail));

            var khoaPhongKhamDoanNoiVienId = _khoaPhongRepository.TableNoTracking.Where(c => c.Ma.ToUpper() == ("KKDNV").ToUpper()).Select(cc => cc.Id).FirstOrDefault();
            var phongBenhViens = _khoaPhongNhanVienRepository.TableNoTracking.Where(c => c.NhanVienId == user.Id && c.KhoaPhong.Id == khoaPhongKhamDoanNoiVienId).Select(c => c.PhongBenhVien);
            var phongBenhVienCoHopDongs = phongBenhViens.Where(c => c.HopDongKhamSucKhoeId != null).Select(c => c.HopDongKhamSucKhoeId);

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstHopDongKhamSucKhoes = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => phongBenhVienCoHopDongs.Contains(x.Id) && x.NgayHieuLuc <= DateTime.Now && (x.NgayKetThuc == null || DateTime.Now <= x.NgayKetThuc))
                    .Select(item => new LookupItemHopDingKhamSucKhoeTemplateVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.CongTyKhamSucKhoe.Ten + "-" + item.SoHopDong,
                        CongTyKhamSucKhoeId = item.CongTyKhamSucKhoeId,
                        SoHopDong = item.SoHopDong,
                        NgayHieuLuc = item.NgayHieuLuc,
                        NgayKetThuc = item.NgayKetThuc
                    })
                    .ApplyLike(queryInfo.Query, x => x.SoHopDong)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstHopDongKhamSucKhoeId = await _hopDongKhamSucKhoeRepository
                    .ApplyFulltext(queryInfo.Query, nameof(HopDongKhamSucKhoe), lstColumnNameSearch)
                    .Where(x => phongBenhVienCoHopDongs.Contains(x.Id) && x.NgayHieuLuc <= DateTime.Now && (x.NgayKetThuc == null || DateTime.Now <= x.NgayKetThuc))
                    .Select(x => x.Id)
                    .ToListAsync();
                lstHopDongKhamSucKhoes = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => lstHopDongKhamSucKhoeId.Contains(x.Id))
                    .Select(item => new LookupItemHopDingKhamSucKhoeTemplateVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.CongTyKhamSucKhoe.Ten + "-" + item.SoHopDong,
                        SoHopDong = item.SoHopDong,
                        NgayHieuLuc = item.NgayHieuLuc,
                        NgayKetThuc = item.NgayKetThuc
                    }).ToListAsync();
            }
            return lstHopDongKhamSucKhoes;
        }

        public async Task<List<LookupItemNhomDichVuChiDinhKhamSucKhoeVo>> GetLoaiDichVuTrenHeThongVaGoiChungs(DropDownListRequestModel model)
        {
            var hopDongKhamSucKhoeId = CommonHelper.GetIdFromRequestDropDownList(model);

            var lstItemVos = await _goiKhamSucKhoeRepository.TableNoTracking
                .Where(p => (p.GoiChung == true || p.GoiDichVuPhatSinh == true)
                            && p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                .ApplyLike(model.Query ?? "", p => p.Ma, p => p.Ten)
                .Select(item => new LookupItemNhomDichVuChiDinhKhamSucKhoeVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    LaGoiPhatSinh = item.GoiDichVuPhatSinh.GetValueOrDefault()
                })
                .Take(model.Take)
                .ToListAsync();

            lstItemVos.Insert(0, new LookupItemNhomDichVuChiDinhKhamSucKhoeVo()
            {
                KeyId = 0,
                DisplayName = "Hệ thống",
                Ten = "Hệ thống"
            });
            return lstItemVos;
        }

        public (List<long>, List<long>) GetListDichVuIdTTrongGoiKhamSucKhoe(string lookupInfo)
        {
            var lstDVKhamId = new List<long>();
            var lstDVKTId = new List<long>();

            var hopDongQueryInfo = JsonConvert.DeserializeObject<TiepNhanDichVuChiDinhQueryVo>(lookupInfo);
            var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .FirstOrDefault(x => x.Id == hopDongQueryInfo.HopDongKhamSucKhoeNhanVienId);
            var goiKhamSucKhoe = _goiKhamSucKhoeRepository.TableNoTracking
                .Include(x => x.GoiKhamSucKhoeDichVuKhamBenhs)
                .Include(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats)
                .FirstOrDefault(x => x.Id == hopDongQueryInfo.GoiKhamSucKhoeId);

            if (hopDongKhamSucKhoeNhanVien != null && goiKhamSucKhoe != null)
            {
                lstDVKhamId = goiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs
                .Where(x => ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                            && ((!x.CoMangThai && !x.KhongMangThai) || hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && hopDongQueryInfo.CoMangThai) || (x.KhongMangThai && !hopDongQueryInfo.CoMangThai))
                            && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !hopDongQueryInfo.DaLapGiaDinh) || (x.DaLapGiaDinh && hopDongQueryInfo.DaLapGiaDinh))
                            && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (hopDongQueryInfo.Tuoi != null && ((x.SoTuoiTu == null || hopDongQueryInfo.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || hopDongQueryInfo.Tuoi <= x.SoTuoiDen)))))
                .Select(item => item.DichVuKhamBenhBenhVienId)
                .ToList();
                lstDVKTId = goiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats
                    .Where(x => ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                && ((!x.CoMangThai && !x.KhongMangThai) || hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && hopDongQueryInfo.CoMangThai) || (x.KhongMangThai && !hopDongQueryInfo.CoMangThai))
                                && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !hopDongQueryInfo.DaLapGiaDinh) || (x.DaLapGiaDinh && hopDongQueryInfo.DaLapGiaDinh))
                                && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (hopDongQueryInfo.Tuoi != null && ((x.SoTuoiTu == null || hopDongQueryInfo.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || hopDongQueryInfo.Tuoi <= x.SoTuoiDen)))))
                    .Select(item => item.DichVuKyThuatBenhVienId)
                .ToList();
            }

            return (lstDVKhamId, lstDVKTId);
        }

        public async Task<long> GetDVKBVaDVKT(DropDownListRequestModel queryInfo)

        {
            var lstDvKhamBenhBVId = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Where(c => c.Ten.Contains(queryInfo.Query)).Select(c => c.Id).FirstOrDefaultAsync();
            var lstDvKTBVId = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(c => c.Ten.Contains(queryInfo.Query)).Select(c => c.Id).FirstOrDefaultAsync();
            return lstDvKhamBenhBVId != 0 ? lstDvKhamBenhBVId : lstDvKTBVId;
        }


        #endregion

        #region Get data
        public async Task<KhamDoanThongTinHanhChinhVo> GetThongTinHanhChinhAsync(long yeuCauTiepNhanId)
        {
            var thongTinHanhChinh = BaseRepository.TableNoTracking
                .Where(x => x.Id == yeuCauTiepNhanId)
                .Select(item => new KhamDoanThongTinHanhChinhVo()
                {
                    MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                    MaBenhNhan = item.BenhNhan.MaBN,
                    HoTen = item.HoTen,
                    TenGioiTinh = item.GioiTinh.GetDescription(),
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,
                    TenNgheNghiep = item.NgheNghiep.Ten,
                    TenDanToc = item.DanToc.Ten,
                    DiaChiDisplay = item.DiaChiDayDu,
                    TenCongTy = item.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten
                }).FirstOrDefault();
            return thongTinHanhChinh;
        }

        public async Task<HopDongKhamSucKhoe> TimKiemThongTinHopDongKhamTheoCongTyAsync(long hopDongId)
        {
            var hopDong =
                await _hopDongKhamSucKhoeRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == hopDongId);
            return hopDong;
        }

        public List<HopDongKhamSucKhoeNhanVien> GetHopDongKhamSucKhoeNhanViens(List<long> hopDongKhamNhanVienIds)
        {
            return _hopDongKhamSucKhoeNhanVienRepository.Table
                        .Where(o=> hopDongKhamNhanVienIds.Contains(o.Id))
                        .Include(y => y.BenhNhan)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.DichVuKyThuatBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.NhomGiaDichVuKyThuatBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.DichVuKhamBenhBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.NhomGiaDichVuKhamBenhBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien)
                        .ToList();
        }

        public async Task<YeuCauTiepNhan> GetThongTinHanhChinhNhanVienAsync(long hopDongKhamNhanVienId)
        {
            var thongTinNhanVienKham =
                await BaseRepository.Table
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.BenhNhan)
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.DichVuKyThuatBenhVien)
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.NhomGiaDichVuKyThuatBenhVien)
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien)
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.DichVuKhamBenhBenhVien)
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.NhomGiaDichVuKhamBenhBenhVien)
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(x => x.HopDongKhamSucKhoeNhanVienId == hopDongKhamNhanVienId);
            if (thongTinNhanVienKham == null) // trường hợp chưa bắt đầu khám
            {
                var hopDongKham =
                    await _hopDongKhamSucKhoeNhanVienRepository.Table
                        .Include(y => y.BenhNhan)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.DichVuKyThuatBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.NhomGiaDichVuKyThuatBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.DichVuKhamBenhBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.NhomGiaDichVuKhamBenhBenhVien)
                        .Include(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien)
                        .FirstOrDefaultAsync(x => x.Id == hopDongKhamNhanVienId);
                if (hopDongKham != null)
                {
                    thongTinNhanVienKham = new YeuCauTiepNhan();
                    thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien = hopDongKham;
                }
            }
            return thongTinNhanVienKham;
        }

        public bool CheckHopDongKhamNhanVienDaBatDauKham(List<long> hopDongKhamNhanVienIds)
        {
            return BaseRepository.TableNoTracking
                .Any(o => o.HopDongKhamSucKhoeNhanVienId != null && hopDongKhamNhanVienIds.Contains(o.HopDongKhamSucKhoeNhanVienId.Value) && o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy);
        }

        public async Task<decimal> GetDonGiaBenhVien(DichVuKhamBenhGiaBenhVienVo dichVuKhamBenhGiaBenhVienVo)
        {
            if (dichVuKhamBenhGiaBenhVienVo.LaDichVuKham)
            {
                var donGiaBV = await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                        .Where(p => p.DichVuKhamBenhBenhVienId == dichVuKhamBenhGiaBenhVienVo.DichVuKhamBenhHoacKyThuatBenhVienId
                                    && p.NhomGiaDichVuKhamBenhBenhVienId == dichVuKhamBenhGiaBenhVienVo.NhomGiaDichVuKhamBenhHoacKyThuatBenhVienId
                                    && p.TuNgay < DateTime.Now && (p.DenNgay == null || DateTime.Now < p.DenNgay))
                        .Select(p => p.Gia).FirstOrDefaultAsync();
                return donGiaBV;
            }
            else
            {
                var donGiaBV = await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                      .Where(p => p.DichVuKyThuatBenhVienId == dichVuKhamBenhGiaBenhVienVo.DichVuKhamBenhHoacKyThuatBenhVienId
                                  && p.NhomGiaDichVuKyThuatBenhVienId == dichVuKhamBenhGiaBenhVienVo.NhomGiaDichVuKhamBenhHoacKyThuatBenhVienId
                                  && p.TuNgay < DateTime.Now && (p.DenNgay == null || DateTime.Now < p.DenNgay))
                      .Select(p => p.Gia)
                      .FirstOrDefaultAsync();
                return donGiaBV;
            }
        }

        public async Task<string> GetTenNhomGiaDichVuKhamBenhBenhVien(long nhomGiaDichVuKhamBenhBenhVienId, bool laDichVuKham)
        {
            if (laDichVuKham)
            {
                return await _nhomGiaDichVuKhamBenhRepository.TableNoTracking.Where(p => p.Id == nhomGiaDichVuKhamBenhBenhVienId).Select(p => p.Ten).FirstOrDefaultAsync();
            }
            else
            {
                return await _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Id == nhomGiaDichVuKhamBenhBenhVienId).Select(p => p.Ten).FirstOrDefaultAsync();
            }
        }

        public async Task<GoiKhamSucKhoe> GetGoiKhamTheoTenVaHopDong(string tenGoiKham, long hopDongKhamSucKhoeId)
        {
            return await _goiKhamSucKhoeRepository.TableNoTracking.Where(p => p.Ten.Trim().TrimStart().TrimEnd().ToUpper().Contains(tenGoiKham.Trim().TrimStart().TrimEnd().ToUpper()) && p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                                                  .FirstOrDefaultAsync();
        }

        public bool KiemTraMaGoiKham(string maGoiKham, long hopDongKhamSucKhoeId)
        {
            return _goiKhamSucKhoeRepository.TableNoTracking.Where(p => p.Ma.Trim().TrimStart().TrimEnd().ToUpper().Contains(maGoiKham.Trim().TrimStart().TrimEnd().ToUpper()) && p.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                                                                  .Any();
        }


        //private Enums.LoaiDichVuKyThuat GetLoaiDichVuKyThuat(long nhomDichVuBenhVienId)
        //{
        //    return CalculateHelper.GetLoaiDichVuKyThuat(nhomDichVuBenhVienId, _nhomDichVuBenhVienRepository.TableNoTracking.ToList());
        //}

        public async Task<ThongTinGiaDichVuTrongGoiKhamSucKhoeVo> GetThongTinGiaDichVuTrongGoi(DichVuKhamBenhGiaBenhVienVo dichVuKhamBenhGiaBenhVienVo)
        {
            if (dichVuKhamBenhGiaBenhVienVo.LaDichVuKham)
            {
                var thongTinGia = await _goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                        .Where(x => x.GoiKhamSucKhoeId == dichVuKhamBenhGiaBenhVienVo.GoiKhamSucKhoeId.Value
                                    && x.DichVuKhamBenhBenhVienId == dichVuKhamBenhGiaBenhVienVo.DichVuKhamBenhHoacKyThuatBenhVienId)
                        .Select(item => new ThongTinGiaDichVuTrongGoiKhamSucKhoeVo
                        {
                            ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                            NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
                            DonGiaBenhVien = item.DonGiaBenhVien,
                            DonGiaUuDai = item.DonGiaUuDai,
                            DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                            TenGoiKhamSucKhoe = item.GoiKhamSucKhoe.Ten,
                            NoiThucHienId = item.GoiKhamSucKhoeNoiThucHiens.Select(a => a.PhongBenhVienId).FirstOrDefault()
                        })
                        .FirstOrDefaultAsync();
                return thongTinGia;
            }
            else
            {
                var thongTinGia = await _goiKhamSucKhoeDichVuDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.GoiKhamSucKhoeId == dichVuKhamBenhGiaBenhVienVo.GoiKhamSucKhoeId.Value
                                    && x.DichVuKyThuatBenhVienId == dichVuKhamBenhGiaBenhVienVo.DichVuKhamBenhHoacKyThuatBenhVienId)
                        .Select(item => new ThongTinGiaDichVuTrongGoiKhamSucKhoeVo
                        {
                            NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                            DonGiaBenhVien = item.DonGiaBenhVien,
                            DonGiaUuDai = item.DonGiaUuDai,
                            DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                            TenGoiKhamSucKhoe = item.GoiKhamSucKhoe.Ten,
                            NoiThucHienId = item.GoiKhamSucKhoeNoiThucHiens.Select(a => a.PhongBenhVienId).FirstOrDefault()
                        })
                        .FirstOrDefaultAsync();
                return thongTinGia;
            }
        }
        #endregion
    }
}
