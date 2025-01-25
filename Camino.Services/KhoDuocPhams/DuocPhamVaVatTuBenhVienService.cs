using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.KhoDuocPhams
{
    [ScopedDependency(ServiceType = typeof(IDuocPhamVaVatTuBenhVienService))]
    public class DuocPhamVaVatTuBenhVienService : IDuocPhamVaVatTuBenhVienService
    {
        private IRepository<DuocPham> _duocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        private IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private IRepository<DuocPhamVaVatTuBenhVien> _duocPhamVaVatTuBenhVienRepository;
        private IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly ICauHinhService _cauHinhService;

        private readonly IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        IUserAgentHelper _userAgentHelper;
        public DuocPhamVaVatTuBenhVienService(IRepository<DuocPham> duocPhamRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository,
            IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<DuocPhamVaVatTuBenhVien> duocPhamVaVatTuBenhVienRepository,
            ICauHinhService cauHinhService,
            IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
            IUserAgentHelper userAgentHelper)
        {
            _duocPhamRepository = duocPhamRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _vatTuRepository = vatTuRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _duocPhamVaVatTuBenhVienRepository = duocPhamVaVatTuBenhVienRepository;
            _cauHinhService = cauHinhService;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _userAgentHelper = userAgentHelper;
        }
        public async Task<List<DuocPhamVaVatTuTrongKhoVo>> GetDuocPhamVaVatTuTrongKho(bool laDuocPhamVatTuBHYT, string searchString, long khoId, int take)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            //searchString = "Betadine sol";

            if (string.IsNullOrEmpty(searchString) || !searchString.Contains(" "))
            {
                var query = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        kho.NhapKhoDuocPhams.KhoId == khoId && kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .ApplyLike(searchString, g => g.DuocPham.Ten)
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        DuongDung = s.DuocPham.DuongDung != null ? s.DuocPham.DuongDung.Ten : null,
                        //SoLuongTon = s.NhapKhoDuocPhamChiTiets
                        //    .Where(nkct =>
                        //        nkct.NhapKhoDuocPhams.KhoId == khoId && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        //        nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        //HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                        //        nkct.NhapKhoDuocPhams.KhoId == khoId && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT && nkct.SoLuongDaXuat < nkct.SoLuongNhap &&
                        //        nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        //    .Select(p => p.HanSuDung).First(),
                        
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                kho.NhapKhoVatTu.KhoId == khoId && kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .ApplyLike(searchString, g => g.VatTus.Ten)
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                //SoLuongTon =
                                //    s.NhapKhoVatTuChiTiets
                                //        .Where(nkct =>
                                //            nkct.NhapKhoVatTu.KhoId == khoId &&
                                //            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                //        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                //HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                //        nkct.NhapKhoVatTu.KhoId == khoId && nkct.SoLuongDaXuat < nkct.SoLuongNhap &&
                                //        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                //        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                //    .Select(p => p.HanSuDung).First(),
                                
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            })).OrderBy(o => o.Ten).Take(take);
                var duocPhamVaVatTuTrongKhos = await query.ToListAsync();
                var vatTuTrongKhos = duocPhamVaVatTuTrongKhos.Where(o => o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien);
                if (vatTuTrongKhos.Any())
                {
                    var vatTuBenhVienIds = vatTuTrongKhos.Select(o => o.Id).ToList();

                    var vatTuTrongKhoChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoVatTu.KhoId == khoId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && o.LaVatTuBHYT == laDuocPhamVatTuBHYT && o.HanSuDung >= DateTime.Now)
                        .Select(o => new VatTuTrongKhoChiTiet
                        {
                            Id = o.Id,
                            VatTuBenhVienId = o.VatTuBenhVienId,
                            SoLuongDaXuat = o.SoLuongDaXuat,
                            SoLuongNhap = o.SoLuongNhap,
                            HanSuDung = o.HanSuDung,
                            NgayNhapVaoBenhVien = o.NgayNhapVaoBenhVien,
                        })
                        .ToList();
                    foreach (var vt in vatTuTrongKhos)
                    {
                        vt.SoLuongTon = vatTuTrongKhoChiTiets.Where(o => o.VatTuBenhVienId == vt.Id).Select(o => o.SoLuongNhap - o.SoLuongDaXuat).DefaultIfEmpty().Sum().MathRoundNumber(2);
                        vt.HanSuDung = vatTuTrongKhoChiTiets
                            .Where(o => o.VatTuBenhVienId == vt.Id && o.SoLuongDaXuat.MathRoundNumber(2) < o.SoLuongNhap.MathRoundNumber(2))
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).FirstOrDefault();
                    }
                }

                var duocPhamTrongKhos = duocPhamVaVatTuTrongKhos.Where(o => o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien);
                if (duocPhamTrongKhos.Any())
                {
                    var duocPhamBenhVienIds = duocPhamTrongKhos.Select(o => o.Id).ToList();

                    var duocPhamTrongKhoChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoDuocPhams.KhoId == khoId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && o.LaDuocPhamBHYT == laDuocPhamVatTuBHYT && o.HanSuDung >= DateTime.Now)
                        .Select(o => new DuocPhamTrongKhoChiTiet
                        {
                            Id = o.Id,
                            DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                            SoLuongDaXuat = o.SoLuongDaXuat,
                            SoLuongNhap = o.SoLuongNhap,
                            HanSuDung = o.HanSuDung,
                            NgayNhapVaoBenhVien = o.NgayNhapVaoBenhVien,
                        })
                        .ToList();
                    foreach(var dp in duocPhamTrongKhos)
                    {
                        dp.SoLuongTon = duocPhamTrongKhoChiTiets.Where(o => o.DuocPhamBenhVienId == dp.Id).Select(o => o.SoLuongNhap - o.SoLuongDaXuat).DefaultIfEmpty().Sum().MathRoundNumber(2);
                        dp.HanSuDung = duocPhamTrongKhoChiTiets
                            .Where(o => o.DuocPhamBenhVienId == dp.Id && o.SoLuongDaXuat.MathRoundNumber(2) < o.SoLuongNhap.MathRoundNumber(2))
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).FirstOrDefault();
                    }
                }

                return duocPhamVaVatTuTrongKhos;
            }


            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ma));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ten));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.HoatChat));


            var duocPhamVaVatTuBenhViens = await _duocPhamVaVatTuBenhVienRepository
                .ApplyFulltext(searchString, nameof(DuocPhamVaVatTuBenhVien), lstColumnNameSearch).Where(p => p.HieuLuc)
                .Select(s => new DuocPhamVaVatTuTrongKhoVo
                {
                    Id = s.DuocPhamBenhVienId ?? s.VatTuBenhVienId.Value,
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu
                }).ToListAsync();

            var dctDuocPham = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);
            var dctVatTu = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var queryFullText = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien && p.Id == o.Id))
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        kho.NhapKhoDuocPhams.KhoId == khoId && kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        SoLuongTon = s.NhapKhoDuocPhamChiTiets
                            .Where(nkct =>
                                nkct.NhapKhoDuocPhams.KhoId == khoId && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                nkct.NhapKhoDuocPhams.KhoId == khoId && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT && nkct.SoLuongDaXuat < nkct.SoLuongNhap &&
                                nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).First()
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien && p.Id == o.Id))
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                kho.NhapKhoVatTu.KhoId == khoId && kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            nkct.NhapKhoVatTu.KhoId == khoId &&
                                            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        nkct.NhapKhoVatTu.KhoId == khoId && nkct.SoLuongDaXuat < nkct.SoLuongNhap &&
                                        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First()
                            }))
                .OrderBy(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? (dctDuocPham.Any(a => a.Key == p.Id) ? dctDuocPham[p.Id] : duocPhamVaVatTuBenhViens.Count) : (dctVatTu.Any(a => a.Key == p.Id) ? dctVatTu[p.Id] : duocPhamVaVatTuBenhViens.Count))
                .Take(take);
            return await queryFullText.ToListAsync();
        }
        public async Task<List<DuocPhamVaVatTuTrongKhoVo>> GetDuocPhamVaVatTuTrongKhoOld(bool laDuocPhamVatTuBHYT, string searchString, long khoId, int take)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            if (string.IsNullOrEmpty(searchString) || !searchString.Contains(" "))
            {
                var query = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        kho.NhapKhoDuocPhams.KhoId == khoId && kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .ApplyLike(searchString, g => g.DuocPham.Ten)
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        DuongDung = s.DuocPham.DuongDung != null ? s.DuocPham.DuongDung.Ten : null,
                        SoLuongTon = s.NhapKhoDuocPhamChiTiets
                            .Where(nkct =>
                                nkct.NhapKhoDuocPhams.KhoId == khoId && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                nkct.NhapKhoDuocPhams.KhoId == khoId && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT && nkct.SoLuongDaXuat < nkct.SoLuongNhap &&
                                nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).First(),
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                kho.NhapKhoVatTu.KhoId == khoId && kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .ApplyLike(searchString, g => g.VatTus.Ten)
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            nkct.NhapKhoVatTu.KhoId == khoId &&
                                            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        nkct.NhapKhoVatTu.KhoId == khoId && nkct.SoLuongDaXuat < nkct.SoLuongNhap &&
                                        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First(),
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            })).OrderBy(o => o.Ten).Take(take);
                return await query.ToListAsync();
            }


            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ma));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ten));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.HoatChat));


            var duocPhamVaVatTuBenhViens = await _duocPhamVaVatTuBenhVienRepository
                .ApplyFulltext(searchString, nameof(DuocPhamVaVatTuBenhVien), lstColumnNameSearch).Where(p => p.HieuLuc)
                .Select(s => new DuocPhamVaVatTuTrongKhoVo
                {
                    Id = s.DuocPhamBenhVienId ?? s.VatTuBenhVienId.Value,
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu
                }).ToListAsync();

            var dctDuocPham = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);
            var dctVatTu = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var queryFullText = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien && p.Id == o.Id))
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        kho.NhapKhoDuocPhams.KhoId == khoId && kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        SoLuongTon = s.NhapKhoDuocPhamChiTiets
                            .Where(nkct =>
                                nkct.NhapKhoDuocPhams.KhoId == khoId && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                nkct.NhapKhoDuocPhams.KhoId == khoId && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT && nkct.SoLuongDaXuat < nkct.SoLuongNhap &&
                                nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).First()
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien && p.Id == o.Id))
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                kho.NhapKhoVatTu.KhoId == khoId && kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            nkct.NhapKhoVatTu.KhoId == khoId &&
                                            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        nkct.NhapKhoVatTu.KhoId == khoId && nkct.SoLuongDaXuat < nkct.SoLuongNhap &&
                                        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First()
                            }))
                .OrderBy(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? (dctDuocPham.Any(a => a.Key == p.Id) ? dctDuocPham[p.Id] : duocPhamVaVatTuBenhViens.Count) : (dctVatTu.Any(a => a.Key == p.Id) ? dctVatTu[p.Id] : duocPhamVaVatTuBenhViens.Count))
                .Take(take);
            return await queryFullText.ToListAsync();
        }
        public async Task<List<DuocPhamVaVatTuTrongKhoVo>> GetDuocPhamVaVatTuTrongNhieuKho(bool laDuocPhamVatTuBHYT, string searchString, int take, params long[] khoIds)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            if (string.IsNullOrEmpty(searchString) || !searchString.Contains(" "))
            {
                var query = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        khoIds.Contains(kho.NhapKhoDuocPhams.KhoId) && kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .ApplyLike(searchString, g => g.DuocPham.Ten, g => g.DuocPham.HoatChat)
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        DuongDung = s.DuocPham.DuongDung != null ? s.DuocPham.DuongDung.Ten : null,
                        //SoLuongTon = s.NhapKhoDuocPhamChiTiets
                        //    .Where(nkct =>
                        //        khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        //        nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        //HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                        //        khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        //        nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                        //    .Select(p => p.HanSuDung).First(),
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat,
                        LoaiThuocTheoQuanLy = s.LoaiThuocTheoQuanLy
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                khoIds.Contains(kho.NhapKhoVatTu.KhoId) && kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .ApplyLike(searchString, g => g.VatTus.Ten)
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                //SoLuongTon =
                                //    s.NhapKhoVatTuChiTiets
                                //        .Where(nkct =>
                                //            khoIds.Contains(nkct.NhapKhoVatTu.KhoId) &&
                                //            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                //        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                //HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                //        khoIds.Contains(nkct.NhapKhoVatTu.KhoId) &&
                                //        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                //        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                //    .Select(p => p.HanSuDung).First(),
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            })).OrderBy(o => o.Ten).Take(take);

                var duocPhamVaVatTuTrongKhos = await query.ToListAsync();
                var vatTuTrongKhos = duocPhamVaVatTuTrongKhos.Where(o => o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien);
                if (vatTuTrongKhos.Any())
                {
                    var vatTuBenhVienIds = vatTuTrongKhos.Select(o => o.Id).ToList();

                    var vatTuTrongKhoChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(o => khoIds.Contains(o.NhapKhoVatTu.KhoId) && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && o.LaVatTuBHYT == laDuocPhamVatTuBHYT && o.HanSuDung >= DateTime.Now)
                        .Select(o => new VatTuTrongKhoChiTiet
                        {
                            Id = o.Id,
                            VatTuBenhVienId = o.VatTuBenhVienId,
                            SoLuongDaXuat = o.SoLuongDaXuat,
                            SoLuongNhap = o.SoLuongNhap,
                            HanSuDung = o.HanSuDung,
                            NgayNhapVaoBenhVien = o.NgayNhapVaoBenhVien,
                        })
                        .ToList();
                    foreach (var vt in vatTuTrongKhos)
                    {
                        vt.SoLuongTon = vatTuTrongKhoChiTiets.Where(o => o.VatTuBenhVienId == vt.Id).Select(o => o.SoLuongNhap - o.SoLuongDaXuat).DefaultIfEmpty().Sum().MathRoundNumber(2);
                        vt.HanSuDung = vatTuTrongKhoChiTiets
                            .Where(o => o.VatTuBenhVienId == vt.Id && o.SoLuongDaXuat.MathRoundNumber(2) < o.SoLuongNhap.MathRoundNumber(2))
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).FirstOrDefault();
                    }
                }

                var duocPhamTrongKhos = duocPhamVaVatTuTrongKhos.Where(o => o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien);
                if (duocPhamTrongKhos.Any())
                {
                    var duocPhamBenhVienIds = duocPhamTrongKhos.Select(o => o.Id).ToList();

                    var duocPhamTrongKhoChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(o => khoIds.Contains(o.NhapKhoDuocPhams.KhoId) && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && o.LaDuocPhamBHYT == laDuocPhamVatTuBHYT && o.HanSuDung >= DateTime.Now)
                        .Select(o => new DuocPhamTrongKhoChiTiet
                        {
                            Id = o.Id,
                            DuocPhamBenhVienId = o.DuocPhamBenhVienId,
                            SoLuongDaXuat = o.SoLuongDaXuat,
                            SoLuongNhap = o.SoLuongNhap,
                            HanSuDung = o.HanSuDung,
                            NgayNhapVaoBenhVien = o.NgayNhapVaoBenhVien,
                        })
                        .ToList();
                    foreach (var dp in duocPhamTrongKhos)
                    {
                        dp.SoLuongTon = duocPhamTrongKhoChiTiets.Where(o => o.DuocPhamBenhVienId == dp.Id).Select(o => o.SoLuongNhap - o.SoLuongDaXuat).DefaultIfEmpty().Sum().MathRoundNumber(2);
                        dp.HanSuDung = duocPhamTrongKhoChiTiets
                            .Where(o => o.DuocPhamBenhVienId == dp.Id && o.SoLuongDaXuat.MathRoundNumber(2) < o.SoLuongNhap.MathRoundNumber(2))
                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).FirstOrDefault();
                    }
                }

                return duocPhamVaVatTuTrongKhos;
            }


            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ma));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ten));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.HoatChat));


            var duocPhamVaVatTuBenhViens = await _duocPhamVaVatTuBenhVienRepository
                .ApplyFulltext(searchString, nameof(DuocPhamVaVatTuBenhVien), lstColumnNameSearch).Where(p => p.HieuLuc)
                .Select(s => new DuocPhamVaVatTuTrongKhoVo
                {
                    Id = s.DuocPhamBenhVienId ?? s.VatTuBenhVienId.Value,
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu
                }).ToListAsync();

            var dctDuocPham = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);
            var dctVatTu = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var queryFullText = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien && p.Id == o.Id))
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        khoIds.Contains(kho.NhapKhoDuocPhams.KhoId) && kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        SoLuongTon = s.NhapKhoDuocPhamChiTiets
                            .Where(nkct =>
                                khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).First(),
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat,
                        LoaiThuocTheoQuanLy = s.LoaiThuocTheoQuanLy
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien && p.Id == o.Id))
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                khoIds.Contains(kho.NhapKhoVatTu.KhoId) && kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            khoIds.Contains(nkct.NhapKhoVatTu.KhoId) &&
                                            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        khoIds.Contains(nkct.NhapKhoVatTu.KhoId) &&
                                        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First(),
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            }))
                .OrderBy(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? (dctDuocPham.Any(a => a.Key == p.Id) ? dctDuocPham[p.Id] : duocPhamVaVatTuBenhViens.Count) : (dctVatTu.Any(a => a.Key == p.Id) ? dctVatTu[p.Id] : duocPhamVaVatTuBenhViens.Count))
                .Take(take);
            return await queryFullText.ToListAsync();
        }
        public async Task<List<DuocPhamVaVatTuTrongKhoVo>> GetDuocPhamVaVatTuTrongNhieuKhoOld(bool laDuocPhamVatTuBHYT, string searchString, int take, params long[] khoIds)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            if (string.IsNullOrEmpty(searchString) || !searchString.Contains(" "))
            {
                var query = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        khoIds.Contains(kho.NhapKhoDuocPhams.KhoId) && kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .ApplyLike(searchString, g => g.DuocPham.Ten, g => g.DuocPham.HoatChat)
                    //.ApplyLike(searchString, g => g.Ma, g => g.DuocPham.Ten, g => g.DuocPham.HoatChat)
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        DuongDung = s.DuocPham.DuongDung != null ? s.DuocPham.DuongDung.Ten : null,
                        SoLuongTon = s.NhapKhoDuocPhamChiTiets
                            .Where(nkct =>
                                khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).First(),
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat,
                        LoaiThuocTheoQuanLy = s.LoaiThuocTheoQuanLy
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                khoIds.Contains(kho.NhapKhoVatTu.KhoId) && kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .ApplyLike(searchString, g => g.VatTus.Ten)
                            //.ApplyLike(searchString, g => g.Ma, g => g.VatTus.Ten)
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            khoIds.Contains(nkct.NhapKhoVatTu.KhoId) &&
                                            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        khoIds.Contains(nkct.NhapKhoVatTu.KhoId) &&
                                        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First(),
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            })).OrderBy(o => o.Ten).Take(take);
                return await query.ToListAsync();
            }


            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ma));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ten));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.HoatChat));


            var duocPhamVaVatTuBenhViens = await _duocPhamVaVatTuBenhVienRepository
                .ApplyFulltext(searchString, nameof(DuocPhamVaVatTuBenhVien), lstColumnNameSearch).Where(p => p.HieuLuc)
                .Select(s => new DuocPhamVaVatTuTrongKhoVo
                {
                    Id = s.DuocPhamBenhVienId ?? s.VatTuBenhVienId.Value,
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu
                }).ToListAsync();

            var dctDuocPham = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);
            var dctVatTu = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var queryFullText = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien && p.Id == o.Id))
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        khoIds.Contains(kho.NhapKhoDuocPhams.KhoId) && kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        SoLuongTon = s.NhapKhoDuocPhamChiTiets
                            .Where(nkct =>
                                khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).First(),
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat,
                        LoaiThuocTheoQuanLy = s.LoaiThuocTheoQuanLy
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien && p.Id == o.Id))
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                khoIds.Contains(kho.NhapKhoVatTu.KhoId) && kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            khoIds.Contains(nkct.NhapKhoVatTu.KhoId) &&
                                            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                        khoIds.Contains(nkct.NhapKhoVatTu.KhoId) &&
                                        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First(),
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            }))
                .OrderBy(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? (dctDuocPham.Any(a => a.Key == p.Id) ? dctDuocPham[p.Id] : duocPhamVaVatTuBenhViens.Count) : (dctVatTu.Any(a => a.Key == p.Id) ? dctVatTu[p.Id] : duocPhamVaVatTuBenhViens.Count))
                .Take(take);
            return await queryFullText.ToListAsync();
        }
        #region get list  Dược phẩm vat tư KhoTuTrucNhanVien // kho lẻ
        public async Task<List<DuocPhamVaVatTuTrongKhoVo>> GetDuocPhamVaVatTuTrongKhoTuTrucNhanVien(bool laDuocPhamVatTuBHYT, string searchString, long khoId, int take)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            //long nhanVienDangLogin = _userAgentHelper.GetCurrentUserId();
            //var khoNhanVienQuanLys = _khoNhanVienQuanLyRepository.TableNoTracking.Where(s => s.NhanVienId == nhanVienDangLogin).Select(x => x.KhoId).ToList();

            if (string.IsNullOrEmpty(searchString) || !searchString.Contains(" "))
            {
                var query = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                        //kho.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                        //khoNhanVienQuanLys.Contains(kho.NhapKhoDuocPhams.KhoId) &&

                        kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .ApplyLike(searchString, g => g.DuocPham.Ten)
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        DuongDung = s.DuocPham.DuongDung != null ? s.DuocPham.DuongDung.Ten : null,
                        SoLuongTon = s.NhapKhoDuocPhamChiTiets
                            .Where(nkct =>
                                //nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                                //khoNhanVienQuanLys.Contains(nkct.NhapKhoDuocPhams.KhoId) &&

                                nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                               //nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                               //khoNhanVienQuanLys.Contains(nkct.NhapKhoDuocPhams.KhoId) &&

                                nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).First(),
                        HamLuong = s.DuocPham.HamLuong,
                        NhaSanXuat = s.DuocPham.NhaSanXuat
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                                //kho.NhapKhoVatTu.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                                 //khoNhanVienQuanLys.Contains(kho.NhapKhoVatTu.KhoId) &&

                                kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                                kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .ApplyLike(searchString, g => g.VatTus.Ten)
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                            //nkct.NhapKhoVatTu.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                                             //khoNhanVienQuanLys.Contains(nkct.NhapKhoVatTu.KhoId) &&

                                            nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                         //nkct.NhapKhoVatTu.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                                         //khoNhanVienQuanLys.Contains(nkct.NhapKhoVatTu.KhoId) &&

                                        nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First(),
                                NhaSanXuat = s.VatTus.NhaSanXuat
                            })).OrderBy(o => o.Ten).Take(take);
                return await query.ToListAsync();
            }


            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ma));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.Ten));
            lstColumnNameSearch.Add(nameof(DuocPhamVaVatTuBenhVien.HoatChat));


            var duocPhamVaVatTuBenhViens = await _duocPhamVaVatTuBenhVienRepository
                .ApplyFulltext(searchString, nameof(DuocPhamVaVatTuBenhVien), lstColumnNameSearch).Where(p => p.HieuLuc)
                .Select(s => new DuocPhamVaVatTuTrongKhoVo
                {
                    Id = s.DuocPhamBenhVienId ?? s.VatTuBenhVienId.Value,
                    LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu
                }).ToListAsync();

            var dctDuocPham = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);
            var dctVatTu = duocPhamVaVatTuBenhViens.Where(o => o.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select((p, i) => new
            {
                key = p.Id,
                rank = i
            }).ToDictionary(o => o.key, o => o.rank);


            var queryFullText = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien && p.Id == o.Id))
                    .Where(o => o.NhapKhoDuocPhamChiTiets.Any(kho =>
                         //kho.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                         //khoNhanVienQuanLys.Contains(kho.NhapKhoDuocPhams.KhoId) &&

                        kho.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                    .Select(s => new DuocPhamVaVatTuTrongKhoVo
                    {
                        Id = s.Id,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                        Ma = s.Ma,
                        Ten = s.DuocPham.Ten,
                        HoatChat = s.DuocPham.HoatChat,
                        DonViTinh = s.DuocPham.DonViTinh != null ? s.DuocPham.DonViTinh.Ten : null,
                        SoLuongTon = s.NhapKhoDuocPhamChiTiets
                            .Where(nkct =>
                               //nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                               //khoNhanVienQuanLys.Contains(nkct.NhapKhoDuocPhams.KhoId) &&

                                nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct =>
                                //nkct.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.KhoLe &&
                                //khoNhanVienQuanLys.Contains(nkct.NhapKhoDuocPhams.KhoId) &&

                                nkct.LaDuocPhamBHYT == laDuocPhamVatTuBHYT &&
                                nkct.HanSuDung >= DateTime.Now).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                            .Select(p => p.HanSuDung).First()
                    })
                    .Union(
                        _vatTuBenhVienRepository.TableNoTracking
                            .Where(o => duocPhamVaVatTuBenhViens.Any(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien && p.Id == o.Id))
                            .Where(o => o.NhapKhoVatTuChiTiets.Any(kho =>
                             //kho.NhapKhoVatTu.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                             //khoNhanVienQuanLys.Contains(kho.NhapKhoVatTu.KhoId) &&

                             kho.LaVatTuBHYT == laDuocPhamVatTuBHYT &&
                             kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now))
                            .Select(s => new DuocPhamVaVatTuTrongKhoVo
                            {
                                Id = s.Id,
                                LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
                                Ma = s.Ma,
                                Ten = s.VatTus.Ten,
                                HoatChat = null,
                                DonViTinh = s.VatTus.DonViTinh,
                                SoLuongTon =
                                    s.NhapKhoVatTuChiTiets
                                        .Where(nkct =>
                                           //nkct.NhapKhoVatTu.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                                           //khoNhanVienQuanLys.Contains(nkct.NhapKhoVatTu.KhoId) &&

                                           nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat),
                                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct =>
                                          //nkct.NhapKhoVatTu.Kho.LoaiKho != EnumLoaiKhoDuocPham.KhoLe &&
                                          //khoNhanVienQuanLys.Contains(nkct.NhapKhoVatTu.KhoId) &&

                                         nkct.LaVatTuBHYT == laDuocPhamVatTuBHYT && nkct.HanSuDung >= DateTime.Now)
                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                    .Select(p => p.HanSuDung).First()
                            }))
                .OrderBy(p => p.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien ? (dctDuocPham.Any(a => a.Key == p.Id) ? dctDuocPham[p.Id] : duocPhamVaVatTuBenhViens.Count) : (dctVatTu.Any(a => a.Key == p.Id) ? dctVatTu[p.Id] : duocPhamVaVatTuBenhViens.Count))
                .Take(take);
            return queryFullText.ToList();
        }
        #endregion get list  Dược phẩm vat tư KhoTuTrucNhanVien
    }
}
