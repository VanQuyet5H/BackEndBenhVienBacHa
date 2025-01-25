using System;
using System.Collections.Generic;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Core.Domain.ValueObject.KetQuaCLS;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhuongPhapVoCams;
using Camino.Services.Helpers;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.PhieuInXetNghiem;
using Newtonsoft.Json;

namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IYeuCauDichVuKyThuatService))]
    public class YeuCauDichVuKyThuatService : MasterFileService<YeuCauDichVuKyThuat>, IYeuCauDichVuKyThuatService
    {
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<Core.Domain.Entities.PhuongPhapVoCams.PhuongPhapVoCam> _phuongPhapVoCamRepository;
        private readonly IRepository<ICD> _icdRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> _dichVuKyThuatRepository;
        private readonly IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> _yeuCauDichVuKyThuatTuongTrinhPtttRepository;
        private readonly IRepository<YeuCauDichVuKyThuatLuocDoPhauThuat> _yeuCauDichVuKyThuatLuocDoPhauThuatRepository;
        private readonly IRepository<Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem> _ketQuaNhomXetNghiemsRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.Users.User> _userRepository;
        private readonly IRepository<Core.Domain.Entities.FileKetQuaCanLamSangs.FileKetQuaCanLamSang> _fileKetQuaCanLamSangRepository;
        IRepository<YeuCauChayLaiXetNghiem> _yeuCauChayLaiXetNghiemRepository;
        IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiem> _phienXetNghiemRepository;
        IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;
        IRepository<Camino.Core.Domain.Entities.XetNghiems.KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietRepository;
        private IRepository<Camino.Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private IRepository<Template> _templateRepository;

        public YeuCauDichVuKyThuatService(IRepository<YeuCauDichVuKyThuat> repository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<ICD> icdRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> dichVuKyThuatRepository,
            IRepository<Core.Domain.Entities.PhuongPhapVoCams.PhuongPhapVoCam> phuongPhapVoCamRepository,
            IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> yeuCauDichVuKyThuatTuongTrinhPtttRepository,
            IRepository<YeuCauDichVuKyThuatLuocDoPhauThuat> yeuCauDichVuKyThuatLuocDoPhauThuatRepository,
            IRepository<FileKetQuaCanLamSang> fileKetQuaCanLamSangRepository,
            ILocalizationService localizationService,
            IRepository<Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem> ketQuaNhomXetNghiemsRepository,
            IRepository<Core.Domain.Entities.Users.User> userRepository,
            IRepository<Core.Domain.Entities.FileKetQuaCanLamSangs.FileKetQuaCanLamSang> fileKQuaCanLamSangRepository,
            IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
            IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiem> phienXetNghiemRepository,
            IRepository<Camino.Core.Domain.Entities.XetNghiems.KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTietRepository,
            IUserAgentHelper userAgentHelper, IRepository<Template> templateRepository,
            IRepository<Camino.Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> yeuCauTiepNhanRepository,
            IRepository<YeuCauChayLaiXetNghiem> yeuCauChayLaiXetNghiemRepository) : base(repository)
        {
            _phongBenhVienRepository = phongBenhVienRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _icdRepository = icdRepository;
            _dichVuKyThuatRepository = dichVuKyThuatRepository;
            _localizationService = localizationService;
            _phuongPhapVoCamRepository = phuongPhapVoCamRepository;
            _yeuCauDichVuKyThuatTuongTrinhPtttRepository = yeuCauDichVuKyThuatTuongTrinhPtttRepository;
            _yeuCauDichVuKyThuatLuocDoPhauThuatRepository = yeuCauDichVuKyThuatLuocDoPhauThuatRepository;
            _ketQuaNhomXetNghiemsRepository = ketQuaNhomXetNghiemsRepository;
            _userRepository = userRepository;
            _fileKetQuaCanLamSangRepository = fileKQuaCanLamSangRepository;
            _yeuCauChayLaiXetNghiemRepository = yeuCauChayLaiXetNghiemRepository;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _phienXetNghiemRepository = phienXetNghiemRepository;
            _userAgentHelper = userAgentHelper;
            _ketQuaXetNghiemChiTietRepository = ketQuaXetNghiemChiTietRepository;
            _templateRepository = templateRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
        }


        #region Kết quả cận lâm sàng 25/05/2021

        #region Kết quả chuẩn đoán hình ảnh và thăm dò chức năng

        public GridDataSource GetDataKetQuaCDHATDCN(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKyThuats = BaseRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == long.Parse(queryInfo.AdditionalSearchString))
                                                              .Include(cc => cc.PhienXetNghiemChiTiets)
                                                              .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                                                              .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                                                              .Include(cc => cc.NhomDichVuBenhVien);

            var query = yeuCauKyThuats.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                                  (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
                                                               .Select(s => new KetQuaCLSGridVo()
                                                               {
                                                                   Id = s.Id,
                                                                   NoiDung = s.TenDichVu,
                                                                   NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                                                                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                                                                   BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                                                                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                                                                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                                                                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                                                                   YeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString),
                                                                   IsDisable = true,
                                                               });

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);
            var dataCanLamSangs = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = dataCanLamSangs };
        }

        public GridDataSource GetTotalKetQuaCDHATDCN(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKyThuats = BaseRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == long.Parse(queryInfo.AdditionalSearchString))
                .Include(cc => cc.PhienXetNghiemChiTiets)
                .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhomDichVuBenhVien);

            var query = yeuCauKyThuats.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                                  (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
                                                               .Select(s => new KetQuaCLSGridVo()
                                                               {
                                                                   Id = s.Id,
                                                                   NoiDung = s.TenDichVu,
                                                                   NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                                                                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                                                                   BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                                                                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                                                                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                                                                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                                                                   YeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString),
                                                                   IsDisable = true,
                                                               });

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);

            var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
            var countTask = dataOrderBy.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        #endregion

        #region Kết quả xét nghiệm
        public GridDataSource GetDataKetQuaXetNghiem(long yeuCauTiepNhanId)
        {
            var yeuCauDichVuKyThuats = BaseRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                         (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                                                         .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                                                                         .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User);
            var lstYeuCauDichVuKyThuatId = yeuCauDichVuKyThuats.Select(c => c.Id).ToList();
            var kqXetNghiemCTs = new List<KetQuaXetNghiemChiTiet>();
            if (lstYeuCauDichVuKyThuatId.Any())
            {
                kqXetNghiemCTs = _ketQuaXetNghiemChiTietRepository.TableNoTracking
                .Where(o => lstYeuCauDichVuKyThuatId.Contains(o.YeuCauDichVuKyThuatId) && o.PhienXetNghiemChiTiet.ThoiDiemKetLuan != null)
                .Include(o => o.PhienXetNghiemChiTiet).ThenInclude(o => o.NhomDichVuBenhVien)
                .Include(x => x.NhomDichVuBenhVien)
                .Include(x => x.YeuCauDichVuKyThuat)
                .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                .Include(x => x.DichVuKyThuatBenhVien)
                .Include(x => x.MayXetNghiem)
                .Include(x => x.DichVuXetNghiem)
                .ToList();
            }    

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();
            var chiTietKetQuaXetNghiems = new List<KQXetNghiemChiTiet>();

            if (kqXetNghiemCTs.Any())
            {
                foreach (var yeuCauDichVuKyThuatId in lstYeuCauDichVuKyThuatId)
                {
                    var allkqXetNghiemCTs = kqXetNghiemCTs.Where(o => o.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId);
                    if(!allkqXetNghiemCTs.Any()) continue;

                    var res = allkqXetNghiemCTs.GroupBy(o => o.PhienXetNghiemChiTietId).OrderBy(o => o.Key).Last().ToList();

                    listChiTiet.AddRange(res);
                }
            }


            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            chiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<KQXetNghiemChiTiet>(), true);

            return new GridDataSource { Data = chiTietKetQuaXetNghiems.ToArray() };
        }
        public GridDataSource GetDataKetQuaXetNghiemOld(long yeuCauTiepNhanId)
        {
            var yeuCauDichVuKyThuats = BaseRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                         (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                                                         .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                                                                         .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User);
            var lstYeuCauDichVuKyThuatId = yeuCauDichVuKyThuats.Select(c => c.Id);
            var phienXetNghiemCTs = yeuCauDichVuKyThuats.SelectMany(c => c.PhienXetNghiemChiTiets).Where(c => c.ThoiDiemKetLuan != null)
                                            .Include(x => x.KetQuaXetNghiemChiTiets)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.MayXetNghiem)
                                            .Include(x => x.NhomDichVuBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                                            .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                            .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User);

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();
            var chiTietKetQuaXetNghiems = new List<KQXetNghiemChiTiet>();

            if (phienXetNghiemCTs.Any())
            {
                foreach (var yeuCauDichVuKyThuatId in lstYeuCauDichVuKyThuatId)
                {
                    if (!phienXetNghiemCTs.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                    var res = phienXetNghiemCTs.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.ToList();

                    listChiTiet.AddRange(res);
                }
            }
           

            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            chiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<KQXetNghiemChiTiet>(), true);

            return new GridDataSource { Data = chiTietKetQuaXetNghiems.ToArray() };
        }

        private List<KQXetNghiemChiTiet> AddDetailDataChild(List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai,
                                                            List<KetQuaXetNghiemChiTiet> lstChiTietNhomChild, List<KQXetNghiemChiTiet> result,
                                                            bool theFirst = false, int level = 1)
        {
            if (!lstChiTietNhomChild.Any() && theFirst != true) return result;

            List<long> lstIdSearch = new List<long>();
            //add root
            if (theFirst)
            {
                var lstParent = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == null).ToList();
                foreach (var parent in lstParent)
                {
                    var ketQua = new KQXetNghiemChiTiet
                    {
                        Id = parent.Id,
                        Ten = parent.YeuCauDichVuKyThuat.TenDichVu,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        Csbt = (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        DonVi = parent.DonVi,
                        //Duyet = parent.DaDuyet ?? false,
                        Duyet = parent.NhanVienDuyetId != null,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        LoaiMau = parent.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription(),
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        DaGoiDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet,
                        //structure tree
                        Level = level,
                        Nhom = parent.NhomDichVuBenhVien.Ten,
                        NhomId = parent.NhomDichVuBenhVienId,
                        IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                            && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),
                        NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId,
                    };
                    lstIdSearch.Add(parent.DichVuXetNghiemId);
                    result.Add(ketQua);
                }
            }
            else
            {
                var lstReOrderBySTT = lstChiTietNhomChild.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
                foreach (var parent in lstReOrderBySTT)
                {
                    var ketQua = new KQXetNghiemChiTiet
                    {
                        Id = parent.Id,
                        Ten = parent.DichVuXetNghiem?.Ten,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        Csbt = (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        DonVi = parent.DonVi,
                        Duyet = parent.NhanVienDuyetId != null,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        DaGoiDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet,
                        LoaiMau = parent.NhomDichVuBenhVien.Ten,
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        //structure tree
                        Level = level,

                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                            , parent.GiaTriTuMay),
                        NhomId = parent.NhomDichVuBenhVienId,
                        Nhom = parent.NhomDichVuBenhVien.Ten,
                        IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                                                                    && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),
                        NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId,
                        DichVuXetNghiemChaId = parent.DichVuXetNghiemChaId,

                    };
                    lstIdSearch.Add(parent.DichVuXetNghiemId);
                    var index = result.FindIndex(x => x.DichVuXetNghiemId == parent.DichVuXetNghiemChaId);
                    if (index >= 0)
                    {
                        var listChilds = result.Count(x => x.DichVuXetNghiemChaId == parent.DichVuXetNghiemChaId);
                        result.Insert(index + 1 + listChilds, ketQua);
                    }
                }
            }

            lstIdSearch = lstIdSearch.Distinct().ToList();
            var lstChiTietChild = lstChiTietNhomConLai.Where(p => lstIdSearch.Any(o => o == p.DichVuXetNghiemChaId)).ToList();
            level++;
            return AddDetailDataChild(lstChiTietNhomConLai, lstChiTietChild, result, false, level);
        }

        #endregion


        #endregion

        #region CLS

        public async Task<GridDataSource> GetDataForGridAsyncKetQuaCLS(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKyThuat = BaseRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == long.Parse(queryInfo.AdditionalSearchString))
                .Include(cc => cc.PhienXetNghiemChiTiets)
                .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhomDichVuBenhVien);

            var query = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                  && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
               .Select(s => new KetQuaCLSGridVo()
               {
                   Id = s.Id,
                   NoiDung = s.TenDichVu,
                   NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                   BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                   YeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString),
                   //ChuanDoan = s.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                   IsDisable = true,
               });

            //List<KetQuaCLSGridVo> listTDCNCDHA = new List<KetQuaCLSGridVo>();
            //Chuẩn đoán hình ảnh và thăm dò chức năng
            //foreach (var itemx in query)
            //{
            //    var fileKetQuaCanLamSang = _fileKetQuaCanLamSangRepository.TableNoTracking.Where(cc => cc.YeuCauDichVuKyThuatId == itemx.Id);
            //    if (fileKetQuaCanLamSang.Any())
            //    {
            //        itemx.XemKetQua = fileKetQuaCanLamSang.FirstOrDefault().DuongDan;
            //        itemx.TenGuid = fileKetQuaCanLamSang.FirstOrDefault().TenGuid;
            //        itemx.TenFilePDF = fileKetQuaCanLamSang.FirstOrDefault().Ten;
            //        itemx.LoaiTapTin = (int)fileKetQuaCanLamSang.FirstOrDefault().LoaiTapTin;
            //        itemx.TenGuidList = fileKetQuaCanLamSang.Select(s => new KetQuaItemCLS()
            //        {
            //            LoaiTapTin = (int)s.LoaiTapTin,
            //            DuongDan = s.DuongDan,
            //            TenGuid = s.TenGuid,
            //            TenFile = s.Ten
            //        }).ToList();
            //        itemx.KiemTraListPDFImage = itemx.TenGuidList.Any(x => x.LoaiTapTin == 1 || x.LoaiTapTin == 2) ? true : false;
            //        listTDCNCDHA.Add(itemx);
            //    }
            //}

            //listTDCNCDHA = listTDCNCDHA.Where(x => x.TenGuidList.Count() > 0).ToList();

            //group by theo phiên và nhóm dịch vu bệnh viện ID
            var yeuCauKyThuatOfXNs = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                   && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem));

            var yeuCauKyThuatIdsOfXN = yeuCauKyThuatOfXNs.Select(cc => cc.Id);
            if (yeuCauKyThuatIdsOfXN.Any())
            {
                var phienXetNghiemChiIds = _phienXetNghiemRepository.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(cc => cc.Id).ToList();

                List<KetQuaCLSGridVo> listKQXN = new List<KetQuaCLSGridVo>();

                var phienXetNghiemChiTiets = _phienXetNghiemChiTietRepository.TableNoTracking.Where(o => yeuCauKyThuatIdsOfXN.Contains(o.YeuCauDichVuKyThuatId)
                                                                                                && o.ThoiDiemKetLuan != null
                                                                                                //&& o.PhienXetNghiem.ThoiDiemKetLuan != null
                                                                                                && phienXetNghiemChiIds.Contains(o.PhienXetNghiemId))
                                                                        .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhomDichVuBenhVien)
                                                                        .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                                                                        .Include(x => x.PhienXetNghiem).ThenInclude(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                                                                        .Include(x => x.PhienXetNghiem).ThenInclude(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                                                                        .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                        .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                        .Include(p => p.NhanVienKetLuan).ThenInclude(q => q.User)
                                                                        .OrderBy(p => p.DichVuKyThuatBenhVien.DichVuXetNghiem.SoThuTu ?? 0).ThenBy(p => p.DichVuKyThuatBenhVien.DichVuXetNghiem.Id)
                                                                        .GroupBy(cc => new { cc.NhomDichVuBenhVienId, cc.PhienXetNghiemId });

                if (phienXetNghiemChiTiets.Any())
                {

                    foreach (var items in phienXetNghiemChiTiets)
                    {
                        var data = new KetQuaCLSGridVo();
                        var phienXetNghiemChiTiet = items.Select(cc => cc).LastOrDefault();
                        if (phienXetNghiemChiTiet != null)
                        {
                            data.Id = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.Id;
                            data.NoiDung = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NhomDichVuBenhVien.Ten;
                            data.LoaiKetQuaId = phienXetNghiemChiTiet.NhomDichVuBenhVienId;
                            data.LoaiKetQuaCLS = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.LoaiDichVuKyThuat.GetDescription();
                            data.NguoiThucHien = phienXetNghiemChiTiet.PhienXetNghiem.NhanVienThucHien?.User?.HoTen;
                            data.NgayThucHien = phienXetNghiemChiTiet.PhienXetNghiem.ThoiDiemBatDau.ApplyFormatDateTimeSACH();
                            data.BacSiKetLuan = phienXetNghiemChiTiet.NhanVienKetLuan?.User?.HoTen;
                            data.NgayKetLuan = phienXetNghiemChiTiet.ThoiDiemKetLuan?.ApplyFormatDateTimeSACH();
                            data.ChuanDoan = phienXetNghiemChiTiet.PhienXetNghiem?.KetLuan;
                            data.PhienXetNghiemId = phienXetNghiemChiTiet.PhienXetNghiem.Id;
                            data.YeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
                            data.NhomDichVuBenhVienId = phienXetNghiemChiTiet.NhomDichVuBenhVienId;
                            data.GoiPhienXetNghiemLai = GoiPhienXetNghiemLai(phienXetNghiemChiTiet.PhienXetNghiem.Id);
                            data.TrangThaiXetNghiemLai = TrangThaiChayLaiXetNghiem(phienXetNghiemChiTiet.PhienXetNghiem.Id, phienXetNghiemChiTiet.NhomDichVuBenhVienId);
                        }

                        var phienXetNghiemChiTietCuoiCungIds = items.GroupBy(cc => cc.YeuCauDichVuKyThuatId).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();
                      
                        if (phienXetNghiemChiTietCuoiCungIds.Any())
                        {
                            foreach (var yeuCaukyThuatChaySauCung in phienXetNghiemChiTietCuoiCungIds)
                            {
                                data.LanThucHien = yeuCaukyThuatChaySauCung.LanThucHien;
                                var ketqua = _ketQuaXetNghiemChiTietRepository.TableNoTracking.Where(c => c.PhienXetNghiemChiTietId == yeuCaukyThuatChaySauCung.Id).Include(x => x.DichVuXetNghiem)
                                                                                             .Include(x => x.MayXetNghiem).Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                                                                                             .Include(x => x.NhanVienDuyet).ThenInclude(cc => cc.User)
                                                                                             .OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId)
                                                                                             .ToList(); ;

                                data.KetQuaCLSGridChiTietVos.AddRange(addDetailDataChild(ketqua, new List<KetQuaXetNghiemChiTiet>(), true));
                                listKQXN.Add(data);
                            }
                        }
                    }
                }

                query = query.Union(listKQXN); //listTDCNCDHA.AsQueryable().Union(listKQXN);
            }

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);
            var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);

            var dataCLS = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = dataCLS };
        }
        public bool GoiPhienXetNghiemLai(long phienXetNghiemId)
        {
            return _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId && cc.DuocDuyet == null).Any();
        }
        public bool TrangThaiChayLaiXetNghiem(long phienXetNghiemId, long nhomDichVuBenhVienId)
        {
            var phienXetNghiemChiTiets = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId
                                    && cc.NhomDichVuBenhVienId == nhomDichVuBenhVienId)
                                    .SelectMany(cc => cc.PhienXetNghiemChiTiets);

            if (phienXetNghiemChiTiets.Any())
                return phienXetNghiemChiTiets.Where(cc => cc.ThoiDiemKetLuan == null && cc.ChayLaiKetQua == true).Any();
            else
                return false;
        }
        private List<KetQuaCLSGridChiTietVo> addDetailDataChild(List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai
        , List<KetQuaXetNghiemChiTiet> lstChiTietNhomChild
        , bool theFirst = false)
        {
            var result = new List<KetQuaCLSGridChiTietVo>();
            if (!lstChiTietNhomChild.Any() && theFirst != true) return null;

            //add root
            if (theFirst)
            {
                var lstParent = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == null).ToList();
                foreach (var parent in lstParent)
                {
                    var ketQua = new KetQuaCLSGridChiTietVo
                    {
                        Id = parent.Id,
                        TenDichVu = parent.YeuCauDichVuKyThuat.TenDichVu,
                        KetQuaCu = parent.GiaTriCu,
                        KetQuaMoi = !string.IsNullOrEmpty(parent.GiaTriDuyet) ? parent.GiaTriDuyet : !string.IsNullOrEmpty(parent.GiaTriNhapTay) ? parent.GiaTriNhapTay : !string.IsNullOrEmpty(parent.GiaTriTuMay) ? parent.GiaTriTuMay : string.Empty,
                        CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax), //(parent.GiaTriMin != null || parent.GiaTriMax != null) ? parent.GiaTriMin + " - " + parent.GiaTriMax : "",
                        DonVi = parent.DonVi,
                        MayXN = parent.MayXetNghiem?.Ten.ToString(),
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        NgayDuyet = parent.ThoiDiemDuyetKetQua != null ? parent.ThoiDiemDuyetKetQua.Value.ApplyFormatDateTimeSACH() : null,
                        //structure tree                       
                        IsRoot = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                        IsParent = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                        IsBold = parent.ToDamGiaTri
                    };
                    var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                    ketQua.Items = addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild);
                    //
                    result.Add(ketQua);
                }
            }
            else
            {
                if (lstChiTietNhomChild != null)
                {
                    //var lstReOrderBySTT = lstChiTietNhomChild.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
                    foreach (var parent in lstChiTietNhomChild.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                    {
                        var ketQua = new KetQuaCLSGridChiTietVo
                        {
                            Id = parent.Id,
                            TenDichVu = parent.DichVuXetNghiem?.Ten,
                            KetQuaCu = parent.GiaTriCu,
                            KetQuaMoi = !string.IsNullOrEmpty(parent.GiaTriDuyet) ? parent.GiaTriDuyet : !string.IsNullOrEmpty(parent.GiaTriNhapTay) ? parent.GiaTriNhapTay : !string.IsNullOrEmpty(parent.GiaTriTuMay) ? parent.GiaTriTuMay : string.Empty,
                            CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),//(parent.GiaTriMin != null || parent.GiaTriMax != null) ? parent.GiaTriMin + " - " + parent.GiaTriMax : "",
                            DonVi = parent.DonVi,
                            MayXN = parent.MayXetNghiem?.Ten.ToString(),
                            NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                            NgayDuyet = parent.ThoiDiemDuyetKetQua != null ? parent.ThoiDiemDuyetKetQua.Value.ApplyFormatDateTimeSACH() : null,
                            //structure tree
                            IsRoot = false,
                            IsParent = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                            IsBold = parent.ToDamGiaTri,
                            DichVuXetNghiemChaId = (long)parent.DichVuXetNghiemChaId
                        };
                        var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                        ketQua.Items = addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild);

                        result.Add(ketQua);

                        var index = result.FindIndex(x => x.DichVuXetNghiemChaId == parent.DichVuXetNghiemChaId);
                        if (index >= 0)
                        {
                            var listChilds = result.Count(x => x.DichVuXetNghiemChaId == parent.DichVuXetNghiemChaId);
                            //result.Insert(index + 1 + listChilds, ketQua);
                        }
                    }
                }
            }

            return result;
        }

        public string LoaiDuoiTapTin(string tenFilePdf)
        {
            if (string.IsNullOrEmpty(tenFilePdf)) return "";
            string[] arrListStr = tenFilePdf.Split('.');
            var vitri = arrListStr.Count();
            if (vitri > 0)
            {
                return '.' + arrListStr[vitri - 1];
            }
            return "";
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncKetQuaCLS(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauKyThuat = BaseRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == long.Parse(queryInfo.AdditionalSearchString))
                .Include(cc => cc.PhienXetNghiemChiTiets)
                .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhomDichVuBenhVien);

            var query = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                 && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
                .Select(s => new KetQuaCLSGridVo()
                {
                    Id = s.Id,
                    NoiDung = s.TenDichVu,
                    NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                    NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                    BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                    NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                    LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                    LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                    YeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString),
                    //ChuanDoan = s.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                    IsDisable = true,
                });

            //List<KetQuaCLSGridVo> listTDCNCDHA = new List<KetQuaCLSGridVo>();
            ////Chuẩn đoán hình ảnh và thăm dò chức năng
            //foreach (var itemx in query)
            //{
            //    var fileKetQuaCanLamSang = _fileKetQuaCanLamSangRepository.TableNoTracking.Where(cc => cc.YeuCauDichVuKyThuatId == itemx.Id);
            //    if (fileKetQuaCanLamSang.Any())
            //    {
            //        itemx.XemKetQua = fileKetQuaCanLamSang.FirstOrDefault().DuongDan;
            //        itemx.TenGuid = fileKetQuaCanLamSang.FirstOrDefault().TenGuid;
            //        itemx.TenFilePDF = fileKetQuaCanLamSang.FirstOrDefault().Ten;
            //        itemx.LoaiTapTin = (int)fileKetQuaCanLamSang.FirstOrDefault().LoaiTapTin;
            //        itemx.TenGuidList = fileKetQuaCanLamSang.Select(s => new KetQuaItemCLS()
            //        {
            //            LoaiTapTin = (int)s.LoaiTapTin,
            //            DuongDan = s.DuongDan,
            //            TenGuid = s.TenGuid,
            //            TenFile = s.Ten
            //        }).ToList();
            //        itemx.KiemTraListPDFImage = itemx.TenGuidList.Any(x => x.LoaiTapTin == 1 || x.LoaiTapTin == 2) ? true : false;
            //        listTDCNCDHA.Add(itemx);
            //    }
            //}

            //listTDCNCDHA = listTDCNCDHA.Where(x => x.TenGuidList.Count() > 0).ToList();

            //group by theo phiên và nhóm dịch vu bệnh viện ID
            var yeuCauKyThuatOfXNs = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                   && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem));

            var yeuCauKyThuatIdsOfXN = yeuCauKyThuatOfXNs.Select(cc => cc.Id);
            if (yeuCauKyThuatIdsOfXN.Any())
            {
                var phienXetNghiemChiIds = _phienXetNghiemRepository.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == long.Parse(queryInfo.AdditionalSearchString))
                .Select(cc => cc.Id).ToList();

                List<KetQuaCLSGridVo> listKQXN = new List<KetQuaCLSGridVo>();

                var phienXetNghiemChiTiets = _phienXetNghiemChiTietRepository.TableNoTracking.Where(o => yeuCauKyThuatIdsOfXN.Contains(o.YeuCauDichVuKyThuatId)
                                                                                                && o.ThoiDiemKetLuan != null
                                                                                                && o.PhienXetNghiem.ThoiDiemKetLuan != null
                                                                                                && phienXetNghiemChiIds.Contains(o.PhienXetNghiemId))
                                                                        .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhomDichVuBenhVien)
                                                                        .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
                                                                        .Include(x => x.PhienXetNghiem).ThenInclude(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                                                                        .Include(x => x.PhienXetNghiem).ThenInclude(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                                                                        .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                        .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                        .GroupBy(cc => cc.NhomDichVuBenhVienId);
                if (phienXetNghiemChiTiets.Any())
                {

                    foreach (var items in phienXetNghiemChiTiets)
                    {
                        var data = new KetQuaCLSGridVo();
                        var phienXetNghiemChiTiet = items.Select(cc => cc).LastOrDefault();
                        if (phienXetNghiemChiTiet != null)
                        {
                            data.Id = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.Id;
                            data.NoiDung = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NhomDichVuBenhVien.Ten;
                            data.LoaiKetQuaId = phienXetNghiemChiTiet.NhomDichVuBenhVienId;
                            data.LoaiKetQuaCLS = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.LoaiDichVuKyThuat.GetDescription();
                            data.NguoiThucHien = phienXetNghiemChiTiet.PhienXetNghiem.NhanVienThucHien.User.HoTen;
                            data.NgayThucHien = phienXetNghiemChiTiet.PhienXetNghiem.ThoiDiemBatDau.ApplyFormatDateTimeSACH();
                            data.BacSiKetLuan = phienXetNghiemChiTiet.PhienXetNghiem.NhanVienKetLuan.User.HoTen;
                            data.NgayKetLuan = phienXetNghiemChiTiet.ThoiDiemKetLuan?.ApplyFormatDateTimeSACH();
                            data.ChuanDoan = phienXetNghiemChiTiet.PhienXetNghiem.KetLuan;
                            data.PhienXetNghiemId = phienXetNghiemChiTiet.PhienXetNghiem.Id;
                            data.YeuCauTiepNhanId = long.Parse(queryInfo.AdditionalSearchString);
                            data.NhomDichVuBenhVienId = phienXetNghiemChiTiet.NhomDichVuBenhVienId;
                            data.GoiPhienXetNghiemLai = GoiPhienXetNghiemLai(phienXetNghiemChiTiet.PhienXetNghiem.Id);
                            data.TrangThaiXetNghiemLai = TrangThaiChayLaiXetNghiem(phienXetNghiemChiTiet.PhienXetNghiem.Id, phienXetNghiemChiTiet.NhomDichVuBenhVienId);
                        }

                        var phienXetNghiemChiTietCuoiCungIds = items.GroupBy(cc => cc.YeuCauDichVuKyThuatId).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();
                        if (phienXetNghiemChiTietCuoiCungIds.Any())
                        {
                            foreach (var yeuCaukyThuatChaySauCung in phienXetNghiemChiTietCuoiCungIds)
                            {
                                data.LanThucHien = yeuCaukyThuatChaySauCung.LanThucHien;
                                var ketqua = _ketQuaXetNghiemChiTietRepository.TableNoTracking.Where(c => c.PhienXetNghiemChiTietId == yeuCaukyThuatChaySauCung.Id)
                                    .Include(x => x.DichVuXetNghiem)
                                    .Include(x => x.MayXetNghiem).Include(x => x.YeuCauDichVuKyThuat).ToList();
                                data.KetQuaCLSGridChiTietVos.AddRange(addDetailDataChild(ketqua, new List<KetQuaXetNghiemChiTiet>(), true));
                                listKQXN.Add(data);
                            }
                        }
                    }
                }
                query = query.Union(listKQXN); //listTDCNCDHA.AsQueryable().Union(listKQXN);
            }

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);
            var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
            var countTask = dataOrderBy.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetDataForGridAsyncKetQuaCLSDetail(QueryInfo queryInfo)
        {
            var queryString = queryInfo.AdditionalSearchString.Split('-');

            var yeuCauTiepNhanId = long.Parse(queryString[0]);
            var nhomDichVuBenhVienId = long.Parse(queryString[1]);
            var phienXetNghiemId = long.Parse(queryString[2]);

            var phienXetNghiemChiTiet = _phienXetNghiemChiTietRepository.TableNoTracking.Where(c => c.NhomDichVuBenhVienId == nhomDichVuBenhVienId && c.PhienXetNghiemId == phienXetNghiemId)
                                                                         .Include(cc => cc.KetQuaXetNghiemChiTiets);

            var ketQuaXetNghiemChiTiets = phienXetNghiemChiTiet.SelectMany(cc => cc.KetQuaXetNghiemChiTiets);


            var query = ketQuaXetNghiemChiTiets.Select(s => new KetQuaCLSGridChiTietVo()
            {
            });

            var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);

            return new GridDataSource { Data = null };
        }

        private string NhanVienKetLuan(long? nhanVienKetLuanId)
        {
            var tenNV = _userRepository.TableNoTracking.Where(s => s.Id == nhanVienKetLuanId).Select(p => p.HoTen).FirstOrDefault();
            return tenNV;
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncKetQuaCLSDeTail(QueryInfo queryInfo)
        {
            return null;
        }
        #endregion
        public async Task<GridDataSource> GetListKetQuaCLS(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                  .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                   .Include(x => x.DichVuKyThuatBenhVien).ThenInclude(x => x.DichVuKyThuat).ThenInclude(x => x.NhomDichVuKyThuat)
                   .Where(x => x.YeuCauKhamBenhId == long.Parse(queryInfo.AdditionalSearchString)) // to do Nam Ho :trang thai da thuc hien
                  .Select(s => new KetQuaCLSGridVo()
                  {
                      Id = s.Id,
                      //XemKetQua = "",
                      NoiDung = s.TenDichVu,
                      NgayThucHien = s.ThoiDiemChiDinh.ApplyFormatDate(),
                      BacSiKetLuan = s.NhanVienChiDinh.User.HoTen

                  }).ApplyLike(queryInfo.SearchTerms, g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<ThongTinPhauThuat> GetThongTinTuongTrinh(long phongBenhVienId, long yeuCauKhamBenhId)
        {
            if (phongBenhVienId == 0)
            {
                return null;
            }

            var query = await _phongBenhVienRepository.GetByIdAsync(phongBenhVienId,
                x => x.Include(u => u.KhoaPhong));

            var thongTinPhauThuat = await _yeuCauKhamBenhRepository.TableNoTracking
                .Include(w => w.Icdchinh)
                .Where(p => p.Id == yeuCauKhamBenhId)
                .Select(p => new ThongTinPhauThuat
                {
                    TenKhoaPhong = query.KhoaPhong.Ten,
                    ChanDoanVaoKhoa = p.Icdchinh != null ? p.Icdchinh.TenTiengViet : null,
                    MoTa = p.GhiChuICDChinh
                }).FirstOrDefaultAsync();

            return thongTinPhauThuat;
        }

        public async Task<List<KhamBenhPhauThuatThuThuatGridVo>> GetListPhauThuatThuThuat(long phongBenhVienId, long nhanVienId, long yeuCauKhamBenhId)
        {
            var phauThuatThuThuats = await BaseRepository.TableNoTracking
                .Include(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(x => x.ICDTruocPhauThuat)
                .Include(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(x => x.ICDSauPhauThuat)
                .Include(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(x => x.PhuongPhapVoCam)
                .Where(p => p.NoiThucHienId == phongBenhVienId && p.NhanVienThucHienId == nhanVienId && p.YeuCauKhamBenhId == yeuCauKhamBenhId && p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                .Select(o =>
                    new KhamBenhPhauThuatThuThuatGridVo
                    {
                        Ten = o.TenDichVu,
                        Id = o.Id,
                        ICDTruocPhauThuatId = o.YeuCauDichVuKyThuatTuongTrinhPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuatId : null,
                        ICDTruocPhauThuatDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.Ma + " - " + o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat.TenTiengViet : null,
                        ICDSauPhauThuatId = o.YeuCauDichVuKyThuatTuongTrinhPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuatId : null,
                        ICDSauPhauThuatDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.Ma + " - " + o.YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat.TenTiengViet : null,
                        GhiChuICDTruocPhauThuat = o.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDTruocPhauThuat,
                        GhiChuICDSauPhauThuat = o.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuICDSauPhauThuat,
                        PhuongPhapPhauThuatThuThuatKey = o.YeuCauDichVuKyThuatTuongTrinhPTTT.MaPhuongPhapPTTT,
                        PhuongPhapPhauThuatThuThuatDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.MaPhuongPhapPTTT + " - " + o.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT,
                        LoaiPTTTEnum = o.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat,
                        LoaiPTTTDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.LoaiPhauThuatThuThuat.GetDescription() : string.Empty,
                        PhuongPhapVoCamId = o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCamId : null,
                        PhuongPhapVoCamDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam.Ma + " - " + o.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam.Ten : string.Empty,
                        TinhHinhPttt = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT,
                        TinhHinhPtttDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.TinhHinhPTTT.GetDescription() : string.Empty,
                        TaiBienPttt = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT,
                        TaiBienPtttDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.TaiBienPTTT.GetDescription() : string.Empty,
                        TuVongPttt = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT,
                        TuVongPtttDisplay = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT != null ? o.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.GetDescription() : string.Empty,
                        TrinhTuPttt = o.YeuCauDichVuKyThuatTuongTrinhPTTT.TrinhTuPhauThuat,
                        NhanVienThucHienDisplay = o.NhanVienThucHien != null ? o.NhanVienThucHien.User.HoTen : string.Empty
                    }).ToListAsync();

            return phauThuatThuThuats;
        }

        public async Task<bool> WillShowTabPhauThuat(long phongBenhVienId, long nhanVienId, long yeuCauKhamBenhId)
        {
            var willShowTabPhauThuat = await BaseRepository.TableNoTracking
                .AnyAsync(p =>
                    p.NoiThucHienId == phongBenhVienId && p.NhanVienThucHienId == nhanVienId &&
                    p.YeuCauKhamBenhId == yeuCauKhamBenhId && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                    p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat);
            return willShowTabPhauThuat;
        }

        public async Task<List<ICDTemplateVo>> GetListICD(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listDichVuKhamBenhs = await _icdRepository.TableNoTracking
                    .Select(item => new ICDTemplateVo
                    {
                        KeyId = item.Id,
                        Ma = item.Ma,
                        TenBenh = item.TenTiengViet
                    })
                    .ApplyLike(model.Query, x => x.DisplayName, x => x.Ma, x => x.TenBenh)
                    .Take(model.Take)
                    .ToListAsync();
                return listDichVuKhamBenhs;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("TenTiengViet");
                lstColumnNameSearch.Add("Ma");

                var listDichVuKhamBenhs = await _icdRepository
                    .ApplyFulltext(model.Query, nameof(ICD), lstColumnNameSearch)
                    .Take(model.Take)
                    .Select(item => new ICDTemplateVo
                    {
                        KeyId = item.Id,
                        Ma = item.Ma,
                        TenBenh = item.TenTiengViet
                    })
                    .ToListAsync();
                return listDichVuKhamBenhs;
            }
        }

        public async Task<List<PhuongPhapPTTTTemplateVo>> GetListPhuongPhapPTTT(DropDownListRequestModel model)
        {
             if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
             {
                var lstPhuongPhapEntity = await _dichVuKyThuatRepository.TableNoTracking
                    .Select(item => new PhuongPhapPTTTTemplateVo
                    {
                        KeyId = item.MaChung,
                        Ma = item.MaChung,
                        Ten = item.TenChung
                    })
                    .ApplyLike(model.Query, x => x.DisplayName, x => x.Ten, x => x.Ma, x => x.Ten)
                    .Take(model.Take)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(model.ParameterDependencies))
                {
                    var jsonConVert = JsonConvert.DeserializeObject<VoMuTiSelect>(model.ParameterDependencies) ;

                    var lists = jsonConVert.PhuongThucPhauThuats.Split(",");
                    var listPPTT = lists.Select(d => d).ToList();
                    lstPhuongPhapEntity.AddRange(_dichVuKyThuatRepository.TableNoTracking
                     .Where(d => listPPTT.Contains(d.MaChung))
                    .Select(item => new PhuongPhapPTTTTemplateVo
                    {
                        KeyId = item.MaChung,
                        Ma = item.MaChung,
                        Ten = item.TenChung
                    })
                    .ToList());
                }

                return lstPhuongPhapEntity;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("MaChung");
                lstColumnNameSearch.Add("TenChung");

                var lstPhuongPhapEntity = await _dichVuKyThuatRepository
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat), lstColumnNameSearch)
                    .Take(model.Take)
                    .Select(item => new PhuongPhapPTTTTemplateVo
                    {
                        KeyId = item.MaChung,
                        Ma = item.MaChung,
                        Ten = item.TenChung
                    })
                    .ToListAsync();

                return lstPhuongPhapEntity;
            }
        }

        public async Task<List<string>> GetListPhuongPhapPtttAutoComplete(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstPhuongPhapEntity = await _dichVuKyThuatRepository.TableNoTracking
                    .Select(item => item.TenChung)
                    .ApplyLike(model.Query, x => x)
                    .Take(model.Take)
                    .ToListAsync();

                return lstPhuongPhapEntity;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("MaChung");
                lstColumnNameSearch.Add("TenChung");

                var lstPhuongPhapEntity = await _dichVuKyThuatRepository
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat), lstColumnNameSearch)
                    .Take(model.Take)
                    .Select(item => item.TenChung)
                    .ToListAsync();

                return lstPhuongPhapEntity;
            }
        }

        public async Task<LoaiPhauThuatThuThuatResultVo> GetLoaiPtttDisplay(string ma)
        {
            var loaiPttt = await _dichVuKyThuatRepository.TableNoTracking
                .Where(p => p.MaChung == ma)
                .Select(p => new LoaiPhauThuatThuThuatResultVo
                {
                    LoaiPTTTDisplay = p.LoaiPhauThuatThuThuat.GetDescription(),
                    LoaiPTTTEnum = p.LoaiPhauThuatThuThuat
                }).FirstOrDefaultAsync();

            return loaiPttt;
        }

        public async Task<List<PhuongPhapVoCamTemplateVo>> GetListPhuongPhapVoCam(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listPhuongPhapEntity = await _phuongPhapVoCamRepository.TableNoTracking
                    .Select(item => new PhuongPhapVoCamTemplateVo
                    {
                        KeyId = item.Id,
                        Ma = item.Ma,
                        Ten = item.Ten
                    })
                    .ApplyLike(model.Query, x => x.DisplayName, x => x.Ma, x => x.Ten)
                    .Take(model.Take)
                    .ToListAsync();

                return listPhuongPhapEntity;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ma");
                lstColumnNameSearch.Add("Ten");

                var listPhuongPhapEntity = await _phuongPhapVoCamRepository
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.PhuongPhapVoCams.PhuongPhapVoCam), lstColumnNameSearch)
                    .Take(model.Take)
                    .Select(item => new PhuongPhapVoCamTemplateVo
                    {
                        KeyId = item.Id,
                        Ma = item.Ma,
                        Ten = item.Ten
                    })
                    .ToListAsync();

                return listPhuongPhapEntity;
            }
        }

        public List<LookupItemVo> GetTinhHinhPttt(LookupQueryInfo queryInfo)
        {
            var listEnumTinhHinhPttt = EnumHelper.GetListEnum<EnumTinhHinhPhauThuatThuThuat>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumTinhHinhPttt = listEnumTinhHinhPttt.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                                             .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumTinhHinhPttt;
        }

        public List<LookupItemVo> GetLoaiPttt(LookupQueryInfo queryInfo)
        {
            var listLoaiPttt = EnumHelper.GetListEnum<LoaiPhauThuatThuThuat>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listLoaiPttt = listLoaiPttt.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                                           .Contains(queryInfo.Query.ToLower().Trim())).ToList();
            }

            return listLoaiPttt;
        }

        public List<LookupItemVo> GetListBoPhanCoThe(LookupQueryInfo queryInfo)
        {
            var listEnumTinhHinhPttt = EnumHelper.GetListEnum<EnumBoPhanCoThe>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumTinhHinhPttt = listEnumTinhHinhPttt.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                                           .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumTinhHinhPttt;
        }

        public List<LookupItemVo> GetTaiBienPttt(LookupQueryInfo queryInfo)
        {
            var listTaiBien = EnumHelper.GetListEnum<EnumTaiBienPTTT>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listTaiBien = listTaiBien.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                                           .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listTaiBien;
        }

        public string GetHinhPhauThuatDuaTrenBoPhan(string boPhan)
        {
            var boPhanList = ResourceHelper.GetBoPhanCoTheFileJSON();

            foreach (var boPhanItem in boPhanList)
            {
                if (boPhanItem.TenBoPhanCoThe == boPhan)
                {
                    return boPhanItem.AnhBoPhanCoThe;
                }
            }

            return null;
        }

        public List<LookupItemVo> GetTuVongPttt(LookupQueryInfo queryInfo)
        {
            var listTuVong = EnumHelper.GetListEnum<EnumTuVongTrongPTTT>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listTuVong = listTuVong.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                         .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listTuVong;
        }

        public async Task<string> ModifyTenPhauThuatThuThuatEntity(string maPhuongPhapPttt)
        {
            var tenPhuongPhap = await _dichVuKyThuatRepository.TableNoTracking
                .Where(p => p.MaChung == maPhuongPhapPttt)
                .Select(p => p.TenChung)
                .FirstOrDefaultAsync();
            return tenPhuongPhap;
        }

        public async Task<SavePhauThuatThuThuatResultVo> UpdateForThisPhauThuat(List<YeuCauDichVuKyThuatTuongTrinhPTTT> yeuCauDichVuKyThuatTuongTrinhPttt, long yeuCauKhamBenhId)
        {
            _yeuCauDichVuKyThuatTuongTrinhPtttRepository.AutoCommitEnabled = false;

            var yeuCauKhamBenhEntity = await _yeuCauKhamBenhRepository
                .GetByIdAsync(yeuCauKhamBenhId, x => x.Include(w => w.YeuCauKhamBenhLichSuTrangThais));

            if (yeuCauKhamBenhEntity.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                yeuCauKhamBenhEntity.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                yeuCauKhamBenhEntity.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauKhamBenhEntity.NoiThucHienId = yeuCauKhamBenhEntity.NoiDangKyId; // _userAgentHelper.GetCurrentNoiLLamViecId();
                yeuCauKhamBenhEntity.ThoiDiemThucHien = DateTime.Now;

                var lichSuNew = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenhEntity.TrangThai,
                    MoTa = yeuCauKhamBenhEntity.TrangThai.GetDescription()
                };

                yeuCauKhamBenhEntity.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
            }

            foreach (var yeuCauDichVuKyThuatItem in yeuCauDichVuKyThuatTuongTrinhPttt)
            {
                var isExist = await _yeuCauDichVuKyThuatTuongTrinhPtttRepository.TableNoTracking
                    .AnyAsync(p => p.Id == yeuCauDichVuKyThuatItem.Id);
                if (isExist == false)
                {
                    await _yeuCauDichVuKyThuatTuongTrinhPtttRepository.AddAsync(yeuCauDichVuKyThuatItem);
                }
                else
                {
                    await _yeuCauDichVuKyThuatTuongTrinhPtttRepository.UpdateAsync(yeuCauDichVuKyThuatItem);
                }
            }

            await _yeuCauDichVuKyThuatTuongTrinhPtttRepository.Context.SaveChangesAsync();

            return new SavePhauThuatThuThuatResultVo
            {
                Error = false,
                Message = _localizationService.GetResource("PhauThuatThuThuatTuongTrinh.Save")
            };
        }

        public async Task<List<LuocDoPhauThuatThuThuatResultVo>> GetListLuocDoPhauThuat(
            ListDichVuKyThuatParameterGridVo listIdDichVuKyThuatModel)
        {
            var listLuocDoPhauThuatResult = new List<LuocDoPhauThuatThuThuatResultVo>();

            foreach (var idDichVuKyThuat in listIdDichVuKyThuatModel.Ids)
            {
                var listLuocDoPhauThuatResultItem = await _yeuCauDichVuKyThuatLuocDoPhauThuatRepository.TableNoTracking
                    .Where(p => p.YeuCauDichVuKyThuatTuongTrinhPTTTId == idDichVuKyThuat)
                    .Select(s => new LuocDoPhauThuatThuThuatResultVo
                    {
                        Id = s.Id,
                        IdYeuCauDichVuKyThuat = idDichVuKyThuat,
                        LuocDoPhauThuat = s.LuocDo,
                        MoTa = s.MoTa
                    }).ToListAsync();

                listLuocDoPhauThuatResult.AddRange(listLuocDoPhauThuatResultItem);
            }

            return listLuocDoPhauThuatResult;
        }
        public async Task<bool> IsExitsDVKTPTTT(long yeuCauKhamBenhId)
        {
            var willShowDvktpttt = await BaseRepository.TableNoTracking
                .AnyAsync(p =>
                    p.YeuCauKhamBenhId == yeuCauKhamBenhId && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                    p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat);
            return willShowDvktpttt;
        }

        public async Task<List<HuyTuongTrinhVo>> GetListTuongTrinhHuy(long? noiThucHienId, long? yctnId)
        {
            var tuongTrinhHuyListQuery = BaseRepository.TableNoTracking
                .Where(e => e.NoiThucHienId == noiThucHienId.GetValueOrDefault() &&
                            (e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ||
                             e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                            e.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            e.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true &&
                            e.YeuCauTiepNhanId == yctnId.GetValueOrDefault())
                .Select(w => new HuyTuongTrinhVo
                {
                    Id = w.Id,
                    TenDv = w.TenDichVu,
                    LyDoHuy = w.YeuCauDichVuKyThuatTuongTrinhPTTT.LyDoKhongThucHien,
                    NguoiHuy = w.YeuCauDichVuKyThuatTuongTrinhPTTT.NhanVienTuongTrinh.User.HoTen
                });
            var tuongTrinhHuyList = await tuongTrinhHuyListQuery.ToListAsync();
            return tuongTrinhHuyList;
        }

        public bool GoiYeuCauChayLaiXetNghiem(KetQuaGoiLaiXetNghiem modelGoiYCChayLaiXetNghiem)
        {
            if (modelGoiYCChayLaiXetNghiem != null && modelGoiYCChayLaiXetNghiem.DanhSachGoiXetNghiemLais.Any())
            {
                var nhomDichVuBenhVienIds = modelGoiYCChayLaiXetNghiem.DanhSachGoiXetNghiemLais.Select(cc => cc.NhomDichVuBenhVienId).ToList();
                var phienXetNghiemId = modelGoiYCChayLaiXetNghiem.DanhSachGoiXetNghiemLais[0].PhienXetNghiemId;
                var kiemTraPhienChayLaiXN = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId
                                && nhomDichVuBenhVienIds.Contains(cc.NhomDichVuBenhVienId) && cc.NgayDuyet == null).Any();
                if (!kiemTraPhienChayLaiXN)
                {
                    foreach (var item in modelGoiYCChayLaiXetNghiem.DanhSachGoiXetNghiemLais)
                    {
                        var model = new YeuCauChayLaiXetNghiem
                        {
                            PhienXetNghiemId = item.PhienXetNghiemId,
                            NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                            NhanVienYeuCauId = _userAgentHelper.GetCurrentUserId(),
                            NgayYeuCau = DateTime.Now,
                            LanThucHien = item.LanThucHien,
                            LyDoYeuCau = modelGoiYCChayLaiXetNghiem.LyDo
                        };
                        _yeuCauChayLaiXetNghiemRepository.Add(model);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool HuyYeuCauChayLaiXetNghiem(long phienXetNghiemId)
        {
            var yeuCauChayLaiXetNghiemChuaDuyet = _yeuCauChayLaiXetNghiemRepository.TableNoTracking.Where(cc => cc.PhienXetNghiemId == phienXetNghiemId && cc.DuocDuyet == null);
            if (yeuCauChayLaiXetNghiemChuaDuyet.Any())
            {
                _yeuCauChayLaiXetNghiemRepository.Delete(yeuCauChayLaiXetNghiemChuaDuyet);
                return true;
            }
            return false;
        }

        //====================================== Phiếu in cho xét nghiêm show ra nhóm ==============================================
        public List<LichSuYeuCauChayLai> LichSuYeuCauChayLai(long yeuCauTiepNhanId)
        {
            var lichSuYeuCauChayLais = new List<LichSuYeuCauChayLai>();
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                          .Include(cc => cc.PhienXetNghiems)
                                                          .ThenInclude(cc => cc.YeuCauChayLaiXetNghiems)
                                                          .Include(cc => cc.PhienXetNghiems)
                                                          .ThenInclude(cc => cc.YeuCauChayLaiXetNghiems)
                                                          .ThenInclude(c => c.NhanVienDuyet).ThenInclude(cc => cc.User);
            var phienXetNghiems = yeuCauTiepNhan.SelectMany(cc => cc.PhienXetNghiems);
            var groupByyeuCauChayLais = phienXetNghiems.SelectMany(cc => cc.YeuCauChayLaiXetNghiems)
                                        .Where(cce => cce.DuocDuyet != null)
                                        .Include(c => c.NhanVienYeuCau).ThenInclude(cc => cc.User)
                                        .Include(c => c.NhanVienDuyet).ThenInclude(cc => cc.User)
                                        .OrderBy(cc => cc.Id).GroupBy(cc => new { cc.LanThucHien, cc.PhienXetNghiemId }).Select(cc => cc.FirstOrDefault()).ToList();

            foreach (var yeuCauChayLai in groupByyeuCauChayLais)
            {
                var model = new LichSuYeuCauChayLai();
                model.NguoiYeuCau = yeuCauChayLai.NhanVienYeuCau.User.HoTen;
                model.NgayYeuCau = yeuCauChayLai.NgayYeuCau.ApplyFormatDateTimeSACH();
                model.LyDoYeuCau = yeuCauChayLai.LyDoYeuCau;
                model.NguoiTuChoi = yeuCauChayLai.NhanVienDuyet?.User.HoTen;
                model.NgayTuChoi = yeuCauChayLai.NgayDuyet.Value.ApplyFormatDateTimeSACH();
                model.LyDoTuChoi = yeuCauChayLai.LyDoKhongDuyet;
                model.TrangThai = yeuCauChayLai.DuocDuyet;
                lichSuYeuCauChayLais.Add(model);
            }

            return lichSuYeuCauChayLais;
        }
        public async Task<List<PhieuInXetNghiemModel>> InDuyetKetQuaXetNghiem(PhieuInXetNghiemVo ketQuaXetNghiemPhieuIn)
        {
            var lstContent = new List<PhieuInXetNghiemModel>();

            var lstYeuCauDaIn = new List<long>();
            //string content = string.Empty;

            var templatePhieuInKetQua = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuInKetQuaXetNghiem"));

            var phienXetNghiemData = await _phienXetNghiemRepository.TableNoTracking
                .Include(cc => cc.MauXetNghiems).ThenInclude(o => o.NhanVienLayMau).ThenInclude(o => o.User)
                .Include(cc => cc.MauXetNghiems).ThenInclude(o => o.PhieuGoiMauXetNghiem)
                .ThenInclude(o => o.NhanVienGoiMau).ThenInclude(o => o.User)

                .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                .Include(p => p.BenhNhan)

                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.TinhThanh)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.YeuCauKhamBenh)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.NhomDichVuBenhVien)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets)
                    .ThenInclude(p => p.MayXetNghiem)
                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuXetNghiem)
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

                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauKhamBenhs)

                .Include(p => p.PhienXetNghiemChiTiets).ThenInclude(p => p.KetQuaXetNghiemChiTiets).ThenInclude(p => p.DichVuKyThuatBenhVien)

                .FirstOrDefaultAsync(q => q.Id == ketQuaXetNghiemPhieuIn.PhienXetNghiemId);

            var phienXetNghiemChiTietEntities = phienXetNghiemData.PhienXetNghiemChiTiets
                                                                  .Where(cc => cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId).ToList();

            var ketQuaXetNghiemChiTiets = phienXetNghiemData.PhienXetNghiemChiTiets
                                                            .Where(cc => cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId).SelectMany(p => p.KetQuaXetNghiemChiTiets).ToList();

            var thongTinBacSi = phienXetNghiemData.PhienXetNghiemChiTiets
                                                  .Where(cc => cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId && cc.PhienXetNghiemId == ketQuaXetNghiemPhieuIn.PhienXetNghiemId)
                                                  .SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                .Where(cc => cc.YeuCauKhamBenh != null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                //.Select(p => p.YeuCauKhamBenh)
                .Select(q => new ThongTinBacSiVo
                {
                    YeuCauKhamBenhId = q.Id,
                    BacSiChiDinh = q.NhanVienChiDinh?.User?.HoTen,
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinh?.Ten
                }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinLeTan = phienXetNghiemData.PhienXetNghiemChiTiets
                                                  .Where(cc => cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId && cc.PhienXetNghiemId == ketQuaXetNghiemPhieuIn.PhienXetNghiemId)
                                                  .SelectMany(p => p.KetQuaXetNghiemChiTiets).Select(p => p.YeuCauDichVuKyThuat)
                .Where(cc => cc.YeuCauKhamBenh == null)
                .OrderBy(cc => cc.ThoiDiemHoanThanh)
                .Select(q => new ThongTinBacSiVo
                {
                    YeuCauKhamBenhId = q.Id,
                    BacSiChiDinh = "Lễ tân",
                    BacSiChiDinhId = q.NhanVienChiDinhId,
                    KhoaPhongChiDinh = q.NoiChiDinh?.Ten,
                    FromLeTan = true,
                }).GroupBy(p => p.BacSiChiDinhId).Select(p => p.First())
                .ToList();

            var thongTinPhieuIns = thongTinBacSi;
            thongTinPhieuIns.AddRange(thongTinLeTan);

            foreach (var thongTin in thongTinPhieuIns)
            {
                var data = new DuyetKetQuaXetNghiemPhieuInResultVo
                {
                    SoPhieu = phienXetNghiemData.MauXetNghiems.Last(q => q.BarCodeId == phienXetNghiemData.BarCodeId)?.PhieuGoiMauXetNghiem?.SoPhieu,
                    MaYTe = phienXetNghiemData.BenhNhan.MaBN,
                    SoVaoVien = phienXetNghiemData.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    LogoUrl = ketQuaXetNghiemPhieuIn.HostingName + "/assets/img/logo-bacha-full.png",
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString(),
                    HoTen = phienXetNghiemData.BenhNhan.HoTen,
                    DiaChi = phienXetNghiemData.YeuCauTiepNhan.DiaChiDayDu,
                    GhiChu = phienXetNghiemData.GhiChu,
                    NamSinh = phienXetNghiemData.BenhNhan.NamSinh,
                    GioiTinh = phienXetNghiemData.BenhNhan.GioiTinh,
                    Gio = DateTime.Now.Hour + " giờ " + DateTime.Now.Minute + " phút",
                    DoiTuong = phienXetNghiemData.YeuCauTiepNhan.CoBHYT == true ? "BHYT (" + phienXetNghiemData.YeuCauTiepNhan.BHYTMucHuong + "%)" : "Viện phí",
                    ChanDoan = phienXetNghiemData.KetLuan,
                    BsChiDinh = thongTin.BacSiChiDinh,
                    KhoaPhong = thongTin.KhoaPhongChiDinh
                };

                data.BarCodeImgBase64 = !string.IsNullOrEmpty(ketQuaXetNghiemPhieuIn.PhienXetNghiemId.ToString())
                    ? BarcodeHelper.GenerateBarCode(ketQuaXetNghiemPhieuIn.PhienXetNghiemId.ToString())
                    : "";

                var chiTietIn = new List<KetQuaXetNghiemChiTiet>();

                if (thongTin.FromLeTan)
                {
                    chiTietIn = phienXetNghiemChiTietEntities
                        .SelectMany(p => p.KetQuaXetNghiemChiTiets)
                        .Where(p => p.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId).ToList();
                }
                else
                {
                    chiTietIn = phienXetNghiemChiTietEntities
                        .SelectMany(p => p.KetQuaXetNghiemChiTiets)
                        .Where(p => p.YeuCauDichVuKyThuat?.YeuCauKhamBenh != null
                        && p.YeuCauDichVuKyThuat?.NhanVienChiDinhId == thongTin.BacSiChiDinhId).ToList();
                }

                if (chiTietIn.Any())
                {
                    var totalTenNhom = chiTietIn.Select(p => p.NhomDichVuBenhVien.Ten).Distinct().ToList();

                    var info = string.Empty;

                    var STT = 1;

                    foreach (var tenNhom in totalTenNhom)
                    {
                        var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b>" + tenNhom.ToUpper()
                                                + "</b></tr>";
                        info += headerNhom;
                        //var queryNhom = chiTietIn.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom).ToList();

                        var queryNhom = new List<KetQuaXetNghiemChiTiet>();
                        var lstYeuCauDichVuKyThuatId = chiTietIn.Where(p => p.NhomDichVuBenhVien.Ten == tenNhom)
                            .Select(p => p.PhienXetNghiemChiTiet).Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();
                        foreach (var ycId in lstYeuCauDichVuKyThuatId)
                        {
                            if (!phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                            var res = phienXetNghiemData.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                            queryNhom.AddRange(res);
                        }

                        //var theFist = false;
                        foreach (var dataParent in queryNhom.Where(p => p.DichVuXetNghiemChaId == null))
                        {
                            info = info + GetPhieuIn("", STT, queryNhom.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList(), dataParent);
                        }

                    }
                    data.DanhSach = info;
                }

                data.NguoiThucHien = phienXetNghiemData.NhanVienKetLuan.User.HoTen;

                var loaiMaus = phienXetNghiemData.MauXetNghiems.Where(cc => cc.PhienXetNghiemId == ketQuaXetNghiemPhieuIn.PhienXetNghiemId &&
                                                                      cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId)
                                                               .Select(cc => cc.LoaiMauXetNghiem.GetDescription());
                data.LoaiMau = loaiMaus.Any() ? string.Join(";", loaiMaus) : String.Empty;

                data.TgLayMau = phienXetNghiemData.MauXetNghiems.Where(cc => cc.PhienXetNghiemId == ketQuaXetNghiemPhieuIn.PhienXetNghiemId &&
                                                                       cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId).LastOrDefault()
                                                                .ThoiDiemLayMau.Value.ApplyFormatDateTimeSACH();

                data.NguoiLayMau = phienXetNghiemData.MauXetNghiems.Where(cc => cc.PhienXetNghiemId == ketQuaXetNghiemPhieuIn.PhienXetNghiemId &&
                                                                      cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId).LastOrDefault()
                                                                .NhanVienLayMau.User.HoTen;

                data.TgNhanMau = phienXetNghiemData.MauXetNghiems.Where(cc => cc.PhienXetNghiemId == ketQuaXetNghiemPhieuIn.PhienXetNghiemId &&
                                                                   cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId).Select(cc => cc.PhieuGoiMauXetNghiem)
                                                                   .LastOrDefault().ThoiDiemGoiMau.ApplyFormatDateTimeSACH();

                data.NguoiNhanMau = phienXetNghiemData.MauXetNghiems.Where(cc => cc.PhienXetNghiemId == ketQuaXetNghiemPhieuIn.PhienXetNghiemId &&
                                                                 cc.NhomDichVuBenhVienId == ketQuaXetNghiemPhieuIn.NhomDichVuBenhVienId).Select(cc => cc.PhieuGoiMauXetNghiem)
                                                                 .LastOrDefault().NhanVienGoiMau.User.HoTen;

                var lstContentItem = new PhieuInXetNghiemModel();
                lstContentItem.Html = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuInKetQua.Body, data);
                lstContent.Add(lstContentItem);
            }

            return lstContent;
        }

        private string GetPhieuIn(string info, int STT, List<KetQuaXetNghiemChiTiet> queryNhom, KetQuaXetNghiemChiTiet queryIn, bool isLev2 = false)
        {
            if (queryIn == null) return info;

            if (queryNhom.Any(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId) && queryIn.DichVuXetNghiemChaId == null)
            {
                info = info + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                + "<td style='border: 1px solid #020000;text-align: left;' colspan='5'><b style='margin-left: 20px;'>" + queryIn.DichVuXetNghiem?.Ten
                                + "</b></tr>";
            }
            else
            {
                var toDam = queryIn.ToDamGiaTri;

                var kq = !string.IsNullOrEmpty(queryIn.GiaTriDuyet) ? queryIn.GiaTriDuyet : !string.IsNullOrEmpty(queryIn.GiaTriNhapTay) ? queryIn.GiaTriNhapTay : !string.IsNullOrEmpty(queryIn.GiaTriTuMay) ? queryIn.GiaTriTuMay : string.Empty;

                if (isLev2)
                {
                    info = toDam == true ? info
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 40px;'>" + queryIn.DichVuXetNghiem?.Ten + "</p>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax)//(!string.IsNullOrEmpty(queryIn.GiaTriMin) && !string.IsNullOrEmpty(queryIn.GiaTriMax) ? queryIn.GiaTriMin + " - " + queryIn.GiaTriMax : "")
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + queryIn.DonVi
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (queryIn.MayXetNghiem?.Ten ?? "")
                                       + "</tr>" : info
                                                   + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 40px;'>" + queryIn.DichVuXetNghiem?.Ten + "</p>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                                   + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax)//(!string.IsNullOrEmpty(queryIn.GiaTriMin) && !string.IsNullOrEmpty(queryIn.GiaTriMax) ? queryIn.GiaTriMin + " - " + queryIn.GiaTriMax : "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right;'>" + queryIn.DonVi
                                                   + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (queryIn.MayXetNghiem?.Ten ?? "")
                                                   + "</tr>";
                }
                else
                {
                    info = toDam == true ? info
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 20px;'>" + queryIn.DichVuXetNghiem?.Ten + "</p>"
                                       + "<td style = 'border: 1px solid #020000;text-align: center;font-weight: bold;'>" + kq
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                       + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax)//(!string.IsNullOrEmpty(queryIn.GiaTriMin) && !string.IsNullOrEmpty(queryIn.GiaTriMax) ? queryIn.GiaTriMin + " - " + queryIn.GiaTriMax : "")
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + queryIn.DonVi
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (queryIn.MayXetNghiem?.Ten ?? "")
                                       + "</tr>" : info
                                                   + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: left;'><p style='margin-left: 20px;'>" + queryIn.DichVuXetNghiem?.Ten + "</p>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + kq
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>"
                                                   + LISHelper.GetChiSoTrungBinh(queryIn.GiaTriMin, queryIn.GiaTriMax)//(!string.IsNullOrEmpty(queryIn.GiaTriMin) && !string.IsNullOrEmpty(queryIn.GiaTriMax) ? queryIn.GiaTriMin + " - " + queryIn.GiaTriMax : "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right;'>" + queryIn.DonVi
                                                   + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (queryIn.MayXetNghiem?.Ten ?? "")
                                                   + "</tr>";
                }

                STT++;
            }

            var lstChild = queryNhom.Where(p => p.DichVuXetNghiemChaId == queryIn.DichVuXetNghiemId).ToList();
            foreach (var item in lstChild)
            {
                info = info + GetPhieuIn("", STT, queryNhom, item, true);
            }

            return info;
        }
        //====================================== END Phiếu in cho xét nghiêm show ra nhóm ==========================================
    }
}
