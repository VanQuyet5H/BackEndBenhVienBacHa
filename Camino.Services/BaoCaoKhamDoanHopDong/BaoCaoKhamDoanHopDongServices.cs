using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using System.Globalization;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System.Collections.Generic;
using Camino.Core.Helpers;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.ExportImport.Help;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System.Text.RegularExpressions;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.KhamDoan;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDuTruSoLuongNguoiThucHienDvLSCLS;
using Camino.Core.Domain.ValueObject.DSDichVuNgoaiGoiKeToan;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDoanhThuKhamDoanTheoNhomDichVu;
using Camino.Services.CauHinh;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using System.Collections;

namespace Camino.Services.BaoCaoKhamDoanHopDong
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoKhamDoanHopDongServices))]


    public class BaoCaoKhamDoanHopDongServices : MasterFileService<HopDongKhamSucKhoeNhanVien>, IBaoCaoKhamDoanHopDongServices
    {
        private readonly IRepository<CongTyKhamSucKhoe> _congTyKhamSucKhoeRepository;
        private readonly IRepository<HopDongKhamSucKhoe> _hopDongKhamSucKhoeRepository;
        private readonly IRepository<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanVienRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<GoiKhamSucKhoe> _goiKhamSucKhoeRepository;
        private readonly IRepository<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuRepository;
        IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;

        private readonly IRepository<GoiKhamSucKhoeDichVuKhamBenh> _goiKhamSucKhoeDichVuKhamBenhRepository;
        private readonly IRepository<GoiKhamSucKhoeDichVuDichVuKyThuat> _goiKhamSucKhoeDichVuDichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;

        private readonly IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSoRepository;

        private readonly IKhamDoanService _iKhamDoanService;

        private readonly IRepository<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository;
        private readonly IRepository<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository;
        private readonly IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private readonly IRepository<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;
        private readonly IRepository<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private readonly IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;
        private readonly IRepository<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietRepository;

        ICauHinhService _cauHinhService;
        private readonly IRepository<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;

        private readonly IRepository<KetQuaSinhHieu> _chiSoSinhHieuRepository;

        IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;

        private readonly IRepository<Camino.Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;

        private readonly IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> _yeuCauDichVuKyThuatTuongTrinhPTTTRepository;
        public BaoCaoKhamDoanHopDongServices(
            IRepository<HopDongKhamSucKhoeNhanVien> repository,
            IRepository<CongTyKhamSucKhoe> congTyKhamSucKhoeRepository,
            IRepository<HopDongKhamSucKhoe> hopDongKhamSucKhoeRepository,
            IRepository<HopDongKhamSucKhoeNhanVien> hopDongKhamSucKhoeNhanVienRepository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
             IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
             IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThuRepository,
        IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
        IRepository<GoiKhamSucKhoe> goiKhamSucKhoeRepository,
        IRepository<Template> templateRepository,
         IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSoRepository,
         IKhamDoanService iKhamDoanService,
         IRepository<GoiKhamSucKhoeDichVuKhamBenh> goiKhamSucKhoeDichVuKhamBenhRepository,
         IRepository<GoiKhamSucKhoeDichVuDichVuKyThuat> goiKhamSucKhoeDichVuDichVuKyThuatRepository,
         IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
          IRepository<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository,
          IRepository<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository,
            IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
            IRepository<YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository,
            IRepository<DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
            IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
            IRepository<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTietRepository,
        ICauHinhService cauHinhService,
         IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository,
         IRepository<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
         IRepository<KetQuaSinhHieu> chiSoSinhHieuRepository,
           IRepository<Camino.Core.Domain.Entities.XetNghiems.PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
           IRepository<Camino.Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
           IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> yeuCauDichVuKyThuatTuongTrinhPTTTRepository
            ) : base(repository)
        {
            _congTyKhamSucKhoeRepository = congTyKhamSucKhoeRepository;
            _hopDongKhamSucKhoeRepository = hopDongKhamSucKhoeRepository;
            _hopDongKhamSucKhoeNhanVienRepository = hopDongKhamSucKhoeNhanVienRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _dichVuXetNghiemKetNoiChiSoRepository = dichVuXetNghiemKetNoiChiSoRepository;
            _iKhamDoanService = iKhamDoanService;
            _templateRepository = templateRepository;
            _goiKhamSucKhoeRepository = goiKhamSucKhoeRepository;
            _taiKhoanBenhNhanThuRepository = taiKhoanBenhNhanThuRepository;
            _goiKhamSucKhoeDichVuKhamBenhRepository = goiKhamSucKhoeDichVuKhamBenhRepository;
            _goiKhamSucKhoeDichVuDichVuKyThuatRepository = goiKhamSucKhoeDichVuDichVuKyThuatRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository = goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository;
            _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository = goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository;
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _cauHinhService = cauHinhService;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _ketQuaXetNghiemChiTietRepository = ketQuaXetNghiemChiTietRepository;
            _chiSoSinhHieuRepository = chiSoSinhHieuRepository;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _yeuCauDichVuKyThuatTuongTrinhPTTTRepository = yeuCauDichVuKyThuatTuongTrinhPTTTRepository;
        }

        //public GridDataSource GetDataForGridAsyncTheoHopDong(QueryInfo queryInfo)
        //{
        //    BuildDefaultSortExpression(queryInfo);
        //    var hopDongs = new List<BaoCaoKhamDoanHopDongTheoHopDongVo>();
        //    IQueryable<BaoCaoKhamDoanHopDongTheoHopDongVo> query = null;
        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {

        //        var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongTheoHopDongVo>(queryInfo.AdditionalSearchString);

        //        if (!string.IsNullOrEmpty(queryString.FromDate1) || !string.IsNullOrEmpty(queryString.ToDate1))
        //        {
        //            DateTime denNgay;
        //            queryString.FromDate1.TryParseExactCustom(out var tuNgay);

        //            if (string.IsNullOrEmpty(queryString.ToDate1))
        //            {
        //                denNgay = DateTime.Now;
        //            }
        //            else
        //            {
        //                queryString.ToDate1.TryParseExactCustom(out denNgay);
        //            }
        //            denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
        //            query = _hopDongKhamSucKhoeRepository.TableNoTracking
        //                    .Where(z => z.HopDongKhamSucKhoeNhanViens.Any(x => x.YeuCauTiepNhans.OrderByDescending(c => c.Id).FirstOrDefault().ThoiDiemTiepNhan >= tuNgay
        //                                                           && x.YeuCauTiepNhans.OrderByDescending(c => c.Id).FirstOrDefault().ThoiDiemTiepNhan <= denNgay))
        //                    .Select(s => new BaoCaoKhamDoanHopDongTheoHopDongVo
        //                    {
        //                        Id = s.Id,
        //                        TenCongTyKhamSucKhoe = s.CongTyKhamSucKhoe.Ten,
        //                        TenHopDongKhamSucKhoe = s.SoHopDong,
        //                        SoLuongNhanVienTheoHopDong = s.HopDongKhamSucKhoeNhanViens.Count(),
        //                    });
        //        }
        //        else
        //        {

        //            query = _hopDongKhamSucKhoeRepository.TableNoTracking
        //                .Select(s => new BaoCaoKhamDoanHopDongTheoHopDongVo
        //                {
        //                    Id = s.Id,
        //                    TenCongTyKhamSucKhoe = s.CongTyKhamSucKhoe.Ten,
        //                    TenHopDongKhamSucKhoe = s.SoHopDong,
        //                    SoLuongNhanVienTheoHopDong = s.HopDongKhamSucKhoeNhanViens.Count(),
        //                });
        //        }

        //        if (!string.IsNullOrEmpty(queryString.SearchString))
        //        {
        //            var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
        //            query = query.ApplyLike(searchTerms,
        //                 g => g.TenCongTyKhamSucKhoe,
        //                 g => g.TenHopDongKhamSucKhoe
        //           );
        //        }
        //        hopDongs = query.ToList();
        //        var hopDongKhamSucKhoeIds = hopDongs.Select(o => o.Id).ToList();
        //        if (hopDongs.Any())
        //        {
        //            var hopDongKhamSucKhoeNhanViens = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(o => hopDongKhamSucKhoeIds.Contains(o.HopDongKhamSucKhoeId)).Select(o => o).ToList();

        //            #region Query Cach Toi Uu By aTuan

        //            var hopDongKhamSucKhoeNhanVienData = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
        //                .Where(o => hopDongKhamSucKhoeIds.Contains(o.HopDongKhamSucKhoeId))
        //                .Select(o => new HopDongKhamSucKhoeNhanVienQueryDataVo
        //                {
        //                    HopDongKhamSucKhoeId = o.HopDongKhamSucKhoeId,
        //                    HopDongKhamSucKhoeNhanVienId = o.Id,
        //                    YeuCauTiepNhans = o.YeuCauTiepNhans.Select(tn => new HopDongKhamSucKhoeNhanVienYeuCauTiepNhanQueryDataVo
        //                    {
        //                        YeuCauTiepNhanId = tn.Id,
        //                        YeuCauKhams = tn.YeuCauKhamBenhs.Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
        //                        {
        //                            YeuCauKhamBenhId = k.Id,
        //                            TrangThai = k.TrangThai,
        //                            ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe
        //                        }).ToList()
        //                    }).ToList()
        //                }).ToList();



        //            var hopDongKhamSucKhoeNhanVienYeuCauKhams1 = new List<HopDongKhamSucKhoeNhanVienYeuCauKhamVo>();

        //            foreach (var hopDongKhamSucKhoeNhanVienItem in hopDongKhamSucKhoeNhanVienData.Where(o => o.YeuCauTiepNhans.Any()))
        //            {
        //                var hopDongKhamSucKhoeNhanVienYeuCauKhamDatas = hopDongKhamSucKhoeNhanVienItem.YeuCauTiepNhans.OrderByDescending(tn => tn.YeuCauTiepNhanId).First()
        //                    .YeuCauKhams.Select(l => new HopDongKhamSucKhoeNhanVienYeuCauKhamVo
        //                    {
        //                        Id = hopDongKhamSucKhoeNhanVienItem.HopDongKhamSucKhoeNhanVienId,
        //                        HopDongKhamSucKhoeId = hopDongKhamSucKhoeNhanVienItem.HopDongKhamSucKhoeId,
        //                        YeuCauKhamBenhId = l.YeuCauKhamBenhId,
        //                        TrangThai = l.TrangThai,
        //                        ChuyenKhoaKhamSucKhoe = l.ChuyenKhoaKhamSucKhoe
        //                    });
        //                hopDongKhamSucKhoeNhanVienYeuCauKhams1.AddRange(hopDongKhamSucKhoeNhanVienYeuCauKhamDatas);
        //            }
        //            #endregion
        //            //var hopDongKhamSucKhoeNhanVienYeuCauKhams = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(o => hopDongs.Any(i => i.Id == o.HopDongKhamSucKhoeId) && o.YeuCauTiepNhans.Any())
        //            //    .SelectMany(o => o.YeuCauTiepNhans.OrderByDescending(c => c.Id).FirstOrDefault().YeuCauKhamBenhs
        //            //    .Select(l => new HopDongKhamSucKhoeNhanVienYeuCauKhamVo
        //            //    {
        //            //        Id = o.Id,
        //            //        HopDongKhamSucKhoeId = o.HopDongKhamSucKhoeId,
        //            //        YeuCauKhamBenhId = l.Id,
        //            //        TrangThai = l.TrangThai,
        //            //        ChuyenKhoaKhamSucKhoe = l.ChuyenKhoaKhamSucKhoe
        //            //    })).ToList();
        //            foreach (var hopDong in hopDongs)
        //            {
        //                var hopDongKhamSucKhoeNhanVienTheoHopDongs = hopDongKhamSucKhoeNhanViens.Where(z => z.HopDongKhamSucKhoeId == hopDong.Id);
        //                var hopDongKhamSucKhoeNhanVienYeuCauKhamTheoHopDongs = hopDongKhamSucKhoeNhanVienYeuCauKhams1.Where(z => z.HopDongKhamSucKhoeId == hopDong.Id);

        //                hopDong.SoLuongNhanVienThucTe = hopDongKhamSucKhoeNhanVienTheoHopDongs
        //                                        .Where(z => hopDongKhamSucKhoeNhanVienYeuCauKhamTheoHopDongs.Where(v => z.Id == v.Id).Any(v => (v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan)
        //                                                                                                                                         && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
        //                                                                                                                                         || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
        //                                                                                                                                         || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
        //                                                                                                                                         || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
        //                                                                                                                                         || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
        //                                                                                                                                         || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
        //                                                                                                                                         || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa
        //                                                                                                                                         ))).Count();
        //                hopDong.SoLuongNhanVienChuaKhamMotDichVu = hopDongKhamSucKhoeNhanVienTheoHopDongs
        //                                        .Where(z => hopDongKhamSucKhoeNhanVienYeuCauKhamTheoHopDongs.Where(v => z.Id == v.Id).All(v => (v.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
        //                                                                                                                                          && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
        //                                                                                                                                          || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
        //                                                                                                                                          || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
        //                                                                                                                                          || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
        //                                                                                                                                          || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
        //                                                                                                                                          || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
        //                                                                                                                                          || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa
        //                                                                                                                                          ))).Count();
        //            }
        //        }
        //        //if (queryString.ChuaKham == false && queryString.DaKham == true)
        //        //{
        //        //    hopDongs = hopDongs.Where(p => p.TinhTrang == 1).ToList();
        //        //}
        //        //else if (queryString.ChuaKham == true && queryString.DaKham == false)
        //        //{
        //        //    hopDongs = hopDongs.Where(p => p.TinhTrang == 0).ToList();
        //        //}
        //    }
        //    var countTask = queryInfo.LazyLoadPage == true || hopDongs == null ? 0 : hopDongs.Count();
        //    var queryTask = hopDongs.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
        //        .Take(queryInfo.Take).ToArray();
        //    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        //}

        public GridDataSource GetDataForGridAsyncTheoHopDongVer1(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var hopDongs = new List<BaoCaoKhamDoanHopDongTheoHopDongVo>();
            var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongTheoHopDongVo>(queryInfo.AdditionalSearchString);
            var query = _hopDongKhamSucKhoeRepository.TableNoTracking
                        .Select(s => new BaoCaoKhamDoanHopDongTheoHopDongVo
                        {
                            Id = s.Id,
                            TenCongTyKhamSucKhoe = s.CongTyKhamSucKhoe.Ten,
                            TenHopDongKhamSucKhoe = s.SoHopDong,
                            SoLuongNhanVienTheoHopDong = s.HopDongKhamSucKhoeNhanViens.Count(),
                            NgayHopDong = s.NgayHopDong
                        });
            hopDongs = query.ToList();
            var hopDongKhamSucKhoeIds = hopDongs.Select(o => o.Id).ToList();
            if (hopDongs.Any())
            {
                var hopDongKhamSucKhoeNhanViens = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Include(nv => nv.YeuCauTiepNhans).Where(o => hopDongKhamSucKhoeIds.Contains(o.HopDongKhamSucKhoeId)).Select(o => o).ToList();

                var hopDongKhamSucKhoeNhanVienData = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                    .Where(o => hopDongKhamSucKhoeIds.Contains(o.HopDongKhamSucKhoeId))
                    .Select(o => new HopDongKhamSucKhoeNhanVienQueryDataVo
                    {
                        HopDongKhamSucKhoeId = o.HopDongKhamSucKhoeId,
                        HopDongKhamSucKhoeNhanVienId = o.Id,
                        YeuCauTiepNhans = o.YeuCauTiepNhans.Select(tn => new HopDongKhamSucKhoeNhanVienYeuCauTiepNhanQueryDataVo
                        {
                            YeuCauTiepNhanId = tn.Id,
                            YeuCauKhams = tn.YeuCauKhamBenhs.Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                            .Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
                            {
                                YeuCauKhamBenhId = k.Id,
                                TrangThai = k.TrangThai,
                                ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
                                ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
                            }).ToList(),
                            YeuCauDichVuKyThuatVos = tn.YeuCauDichVuKyThuats.Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                            .Select(kt => new HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
                            {
                                YeuCauDichVuKyThuatId = kt.Id,
                                TrangThai = kt.TrangThai,
                                LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
                                TenDichVu = kt.TenDichVu,
                                ThoiDiemThucHien = kt.ThoiDiemThucHien,
                                CoKetQuaCDHA = !string.IsNullOrEmpty(kt.DataKetQuaCanLamSang),
                                DaDuyetXetNghiem = kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                                ThoiDiemDuyetKetQua = kt.KetQuaXetNghiemChiTiets.Any() ? kt.KetQuaXetNghiemChiTiets.OrderByDescending(c => c.Id).FirstOrDefault().ThoiDiemDuyetKetQua : null
                            }).ToList()
                        }).ToList()
                    }).ToList();

                var hopDongKhamSucKhoeNhanVienTheoDichVuVos = new List<HopDongKhamSucKhoeNhanVienTheoDichVuVo>();
                DateTime tuNgay = new DateTime(1970, 1, 1);
                DateTime denNgay = DateTime.Now;
                if (!string.IsNullOrEmpty(queryString.FromDate1) || !string.IsNullOrEmpty(queryString.ToDate1))
                {
                    queryString.FromDate1.TryParseExactCustom(out tuNgay);

                    if (!string.IsNullOrEmpty(queryString.ToDate1))
                    {
                        queryString.ToDate1.TryParseExactCustom(out denNgay);
                    }
                }
                foreach (var hopDongKhamSucKhoeNhanVienItem in hopDongKhamSucKhoeNhanVienData.Where(o => o.YeuCauTiepNhans.Any()))
                {
                    var yeuTiepNhan = hopDongKhamSucKhoeNhanVienItem.YeuCauTiepNhans.OrderByDescending(tn => tn.YeuCauTiepNhanId).First();
                    var hopDongKhamSucKhoeNhanVienDichVuKyThuatDatas = yeuTiepNhan.YeuCauDichVuKyThuatVos
                        .Where(kt => queryString.ChuaKham == true || (kt.ThoiDiemThucHien == null || tuNgay <= kt.ThoiDiemThucHien && kt.ThoiDiemThucHien <= denNgay ||
                        kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && tuNgay <= kt.ThoiDiemDuyetKetQua && kt.ThoiDiemDuyetKetQua <= denNgay) && queryString.DaKham == true)
                        .Select(kyThuat => new HopDongKhamSucKhoeNhanVienTheoDichVuVo
                        {
                            HopDongKhamSucKhoeNhanVienId = hopDongKhamSucKhoeNhanVienItem.HopDongKhamSucKhoeNhanVienId,
                            HopDongKhamSucKhoeId = hopDongKhamSucKhoeNhanVienItem.HopDongKhamSucKhoeId,
                            YeuCauDichVuKyThuatId = kyThuat.YeuCauDichVuKyThuatId,
                            TrangThaiKyThuat = kyThuat.TrangThai,
                            DaDuyetXetNghiem = kyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                            CoKetQuaCDHA = kyThuat.CoKetQuaCDHA,
                            LaDichVuKhamBenh = false,
                            ThoiDiemThucHien = kyThuat.ThoiDiemThucHien,
                            TenDichVu = kyThuat.TenDichVu,
                            ThoiDiemDuyetKetQua = kyThuat.ThoiDiemDuyetKetQua
                        }).ToList();

                    var hopDongKhamSucKhoeNhanVienDichVuKhamDatas = yeuTiepNhan.YeuCauKhams
                        .Where(kb => queryString.ChuaKham == true || (kb.ThoiDiemHoanThanh == null || tuNgay <= kb.ThoiDiemHoanThanh && kb.ThoiDiemHoanThanh <= denNgay) && queryString.DaKham == true)
                        .Select(l => new HopDongKhamSucKhoeNhanVienTheoDichVuVo
                        {
                            HopDongKhamSucKhoeNhanVienId = hopDongKhamSucKhoeNhanVienItem.HopDongKhamSucKhoeNhanVienId,
                            HopDongKhamSucKhoeId = hopDongKhamSucKhoeNhanVienItem.HopDongKhamSucKhoeId,
                            YeuCauKhamBenhId = l.YeuCauKhamBenhId,
                            TrangThaiKhamBenh = l.TrangThai,
                            ChuyenKhoaKhamSucKhoe = l.ChuyenKhoaKhamSucKhoe,
                            LaDichVuKhamBenh = true,
                            ThoiDiemHoanThanh = l.ThoiDiemHoanThanh,
                            TenDichVu = l.TenDichVu
                        }).ToList();
                    hopDongKhamSucKhoeNhanVienTheoDichVuVos.AddRange(hopDongKhamSucKhoeNhanVienDichVuKhamDatas.Concat(hopDongKhamSucKhoeNhanVienDichVuKyThuatDatas));
                }

                foreach (var hopDong in hopDongs)
                {
                    var nhanVienTheoHopDongs = hopDongKhamSucKhoeNhanViens.Where(z => z.HopDongKhamSucKhoeId == hopDong.Id && z.YeuCauTiepNhans.Any()).ToList();//hopDong.Id
                    var dichVuCuaNhanViens = hopDongKhamSucKhoeNhanVienTheoDichVuVos.Where(z => z.HopDongKhamSucKhoeId == hopDong.Id).ToList();
                    if (queryString.ChuaKham == true)
                    {
                        //=> chưa khám bất kì 1 dịch vụ nào (kb/kt)
                        hopDong.SoLuongNhanVienThucTe = dichVuCuaNhanViens.Any() ? nhanVienTheoHopDongs
                                          .Where(nv =>
                                          dichVuCuaNhanViens.Where(dv => nv.Id == dv.HopDongKhamSucKhoeNhanVienId && dv.LaDichVuKhamBenh).All(v =>
                                          (v.TrangThaiKhamBenh == EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                          || v.TrangThaiKhamBenh == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                                          || v.TrangThaiKhamBenh == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                                          || v.TrangThaiKhamBenh == EnumTrangThaiYeuCauKhamBenh.DangKham
                                          )
                                            && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
                                            | v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa
                                            ))
                                            &&
                                            dichVuCuaNhanViens.Where(dv => nv.Id == dv.HopDongKhamSucKhoeNhanVienId && !dv.LaDichVuKhamBenh).All(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                            || v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                                                                                                                            ).Count() : 0;

                    }
                    else if (queryString.DaKham == true)
                    {
                        // Chỉ cần 1 dịch vụ đã khám/thực hiện
                        hopDong.SoLuongNhanVienThucTe = dichVuCuaNhanViens.Any() ? nhanVienTheoHopDongs
                                          .Where(nv => dichVuCuaNhanViens.Where(dv => dv.HopDongKhamSucKhoeId == hopDong.Id && dv.LaDichVuKhamBenh && tuNgay <= dv.ThoiDiemHoanThanh && dv.ThoiDiemHoanThanh <= denNgay).Count() > 0
                                                     && dichVuCuaNhanViens.Where(dv => nv.Id == dv.HopDongKhamSucKhoeNhanVienId && dv.LaDichVuKhamBenh).Any(v => v.TrangThaiKhamBenh == EnumTrangThaiYeuCauKhamBenh.DaKham
                                                                                                                                            && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
                                                                                                                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                                                                                                                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
                                                                                                                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
                                                                                                                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
                                                                                                                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
                                                                                                                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa
                                                                                                                                            )
                                                                                                                                            )
                                                  ||
                                        dichVuCuaNhanViens.Where(dv => dv.HopDongKhamSucKhoeId == hopDong.Id && !dv.LaDichVuKhamBenh && (tuNgay <= dv.ThoiDiemThucHien && dv.ThoiDiemThucHien <= denNgay ||
                                                  dv.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && tuNgay <= dv.ThoiDiemDuyetKetQua && dv.ThoiDiemDuyetKetQua <= denNgay)).Count() > 0
                                                  && dichVuCuaNhanViens.Where(dv => nv.Id == dv.HopDongKhamSucKhoeNhanVienId && !dv.LaDichVuKhamBenh).Any(kt => kt.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                                                                                                       )
                                                                                                                                            ).Count() : 0;
                    }

                }
            }
            //if (queryString.ChuaKham == false && queryString.DaKham == true)
            //{
            //    hopDongs = hopDongs.Where(p => p.TinhTrang == 1).ToList();
            //}
            //else if (queryString.ChuaKham == true && queryString.DaKham == false)
            //{
            //    hopDongs = hopDongs.Where(p => p.TinhTrang == 0).ToList();
            //}
            var countTask = queryInfo.LazyLoadPage == true || hopDongs == null ? 0 : hopDongs.Where(c => c.SoLuongNhanVienThucTe > 0).Count();
            var queryTask = hopDongs.Where(c => c.SoLuongNhanVienThucTe > 0).AsQueryable().OrderBy(queryInfo.SortString).ToArray();
            //.Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take)
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public GridDataSource GetTotalPageForGridAsyncTheoHopDong(QueryInfo queryInfo)
        {
            return null;
        }

        public GridDataSource GetDataForGridAsyncTheoHopDongVer2(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = null, TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongTheoHopDongVo>(queryInfo.AdditionalSearchString);
            
            var hopDongs = _hopDongKhamSucKhoeRepository.TableNoTracking
                        .Where(o=>o.DaKetThuc != true)
                        .Select(s => new BaoCaoKhamDoanHopDongTheoHopDongVo
                        {
                            Id = s.Id,
                            TenCongTyKhamSucKhoe = s.CongTyKhamSucKhoe.Ten,
                            TenHopDongKhamSucKhoe = s.SoHopDong,
                            SoLuongNhanVienTheoHopDong = s.HopDongKhamSucKhoeNhanViens.Count(),
                            NgayHopDong = s.NgayHopDong,
                            HopDongKhamSucKhoeNhanVienIds = s.HopDongKhamSucKhoeNhanViens.Select(o=>o.Id).ToList(),
                        }).ToList();

            var hopDongKhamSucKhoeIds = hopDongs.Select(o => o.Id).ToList();
            if (hopDongs.Any())
            {
                #region Cập nhật 09/12/2022: grid load chậm
                //var yeuCauTiepNhanKhamSucKhoeData = _yeuCauTiepNhanRepository.TableNoTracking
                //    .Where(o => o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && o.HopDongKhamSucKhoeNhanVienId != null && hopDongKhamSucKhoeIds.Contains(o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId))
                //    .Select(o => new
                //    {
                //        o.Id,
                //        HopDongKhamSucKhoeNhanVienId = o.HopDongKhamSucKhoeNhanVienId,
                //        YeuCauKhams = o.YeuCauKhamBenhs.Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && kb.GoiKhamSucKhoeId != null)
                //            .Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
                //            {
                //                YeuCauKhamBenhId = k.Id,
                //                TrangThai = k.TrangThai,
                //                ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
                //                ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
                //            }).ToList(),
                //        YeuCauDichVuKyThuatVos = o.YeuCauDichVuKyThuats.Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && kt.GoiKhamSucKhoeId != null)
                //            .Select(kt => new HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
                //            {
                //                YeuCauDichVuKyThuatId = kt.Id,
                //                TrangThai = kt.TrangThai,
                //                LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
                //                TenDichVu = kt.TenDichVu,
                //                ThoiDiemThucHien = kt.ThoiDiemThucHien,
                //                CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
                //                ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh,
                //                //DaDuyetXetNghiem = kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                //                //ThoiDiemDuyetKetQua = kt.KetQuaXetNghiemChiTiets.Any() ? kt.KetQuaXetNghiemChiTiets.OrderByDescending(c => c.Id).FirstOrDefault().ThoiDiemDuyetKetQua : null
                //            }).ToList()
                //    }).ToList();

                var lstYCKB = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && kb.GoiKhamSucKhoeId != null
                                && kb.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && kb.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                && hopDongKhamSucKhoeIds.Contains(kb.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId))
                        .Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
                        {
                            YeuCauTiepNhanId = k.YeuCauTiepNhanId,
                            HopDongKhamSucKhoeNhanVienId = k.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                            YeuCauKhamBenhId = k.Id,
                            TrangThai = k.TrangThai,
                            ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
                            ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
                        }).ToList();

                var lstYCKT = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && kt.GoiKhamSucKhoeId != null
                                && kt.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                && hopDongKhamSucKhoeIds.Contains(kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId))
                        .Select(kt => new HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
                        {
                            YeuCauTiepNhanId = kt.YeuCauTiepNhanId,
                            HopDongKhamSucKhoeNhanVienId = kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                            YeuCauDichVuKyThuatId = kt.Id,
                            TrangThai = kt.TrangThai,
                            LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
                            TenDichVu = kt.TenDichVu,
                            ThoiDiemThucHien = kt.ThoiDiemThucHien,
                            CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
                            ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh
                        }).ToList();

                var lstYCTNId = lstYCKB.Select(x => new { x.YeuCauTiepNhanId, x.HopDongKhamSucKhoeNhanVienId })
                            .Union(lstYCKT.Select(x => new { x.YeuCauTiepNhanId, x.HopDongKhamSucKhoeNhanVienId }))
                            .Distinct().ToList();

                var lstYCKBDic = lstYCKB.GroupBy(x => x.YeuCauTiepNhanId).ToDictionary(x => x.Key, x => x.Select(a => a).ToList());
                var lstYCKTDic = lstYCKT.GroupBy(x => x.YeuCauTiepNhanId).ToDictionary(x => x.Key, x => x.Select(a => a).ToList());

                var yeuCauTiepNhanKhamSucKhoeData = lstYCTNId.Select(x => new
                {
                    Id = x.YeuCauTiepNhanId,
                    HopDongKhamSucKhoeNhanVienId = x.HopDongKhamSucKhoeNhanVienId,
                    YeuCauKhams = lstYCKBDic.TryGetValue(x.YeuCauTiepNhanId, out var lstKB) ? lstKB : new List<HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo>(),
                    YeuCauDichVuKyThuatVos = lstYCKTDic.TryGetValue(x.YeuCauTiepNhanId, out var lstKT) ? lstKT : new List<HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo>()
                }).ToList();

                lstYCKB = null;
                lstYCKT = null;
                lstYCKBDic = null;
                lstYCKTDic = null;
                lstYCTNId = null;
                #endregion

                if (queryString.ChuaKham == true)
                {
                    //=> chưa khám bất kì 1 dịch vụ nào (kb/kt)
                    var hopDongKhamSucKhoeNhanVienChuaKham = yeuCauTiepNhanKhamSucKhoeData
                        .Where(o => o.YeuCauKhams.All(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang).All(kt => !kt.CoKetQuaCDHA))
                        .Select(o=>o.HopDongKhamSucKhoeNhanVienId).ToList();
                    foreach (var hopDong in hopDongs)
                    {
                        hopDong.SoLuongNhanVienThucTe = hopDong.HopDongKhamSucKhoeNhanVienIds.Where(o => hopDongKhamSucKhoeNhanVienChuaKham.Contains(o)).Count();
                    }
                }
                else if (queryString.DaKham == true)
                {
                    DateTime tuNgay = new DateTime(1970, 1, 1);
                    DateTime denNgay = DateTime.Now;
                    if (!string.IsNullOrEmpty(queryString.FromDate1) || !string.IsNullOrEmpty(queryString.ToDate1))
                    {
                        queryString.FromDate1.TryParseExactCustom(out tuNgay);

                        if (!string.IsNullOrEmpty(queryString.ToDate1))
                        {
                            queryString.ToDate1.TryParseExactCustom(out denNgay);
                        }
                    }
                    // Chỉ cần 1 dịch vụ đã khám/thực hiện 
                    var hopDongKhamSucKhoeNhanVienDaKham = yeuCauTiepNhanKhamSucKhoeData
                        .Where(o => o.YeuCauKhams.Any(kb=>kb.ThoiDiemHoanThanh >= tuNgay && kb.ThoiDiemHoanThanh <= denNgay && kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                || o.YeuCauDichVuKyThuatVos
                                        .Any(kt => ((kt.ThoiDiemDuyetKetQua != null && kt.ThoiDiemDuyetKetQua >= tuNgay && kt.ThoiDiemDuyetKetQua <= denNgay) || (kt.ThoiDiemDuyetKetQua == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay))
                                                    && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                || o.YeuCauDichVuKyThuatVos
                                        .Any(kt => ((kt.ThoiDiemDuyetKetQua != null && kt.ThoiDiemDuyetKetQua >= tuNgay && kt.ThoiDiemDuyetKetQua <= denNgay) || (kt.ThoiDiemDuyetKetQua == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay))
                                                    && (kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && kt.CoKetQuaCDHA))
                        .Select(o => o.HopDongKhamSucKhoeNhanVienId).ToList();
                    foreach (var hopDong in hopDongs)
                    {
                        hopDong.SoLuongNhanVienThucTe = hopDong.HopDongKhamSucKhoeNhanVienIds.Where(o => hopDongKhamSucKhoeNhanVienDaKham.Contains(o)).Count();
                    }
                }
            }
            var returnData = hopDongs.Where(c => c.SoLuongNhanVienThucTe > 0).AsQueryable().OrderBy(queryInfo.SortString).ToArray();

            return new GridDataSource { Data = returnData, TotalRowCount = returnData.Count() };
        }

        public async Task<GridDataSource> GetDataForGridAsyncTheoNhanVienOld(QueryInfo queryInfo, bool exportExcel)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<BaoCaoKhamDoanHopDongTheoNhanVienVo>().ToArray(), TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongTheoNhanVienVo>(queryInfo.AdditionalSearchString);
            IQueryable<BaoCaoKhamDoanHopDongTheoNhanVienVo> queryChuaKham = null;
            IQueryable<BaoCaoKhamDoanHopDongTheoNhanVienVo> queryDangKham = null;
            IQueryable<BaoCaoKhamDoanHopDongTheoNhanVienVo> queryDaKham = null;

            if (queryString.ChuaKhamNhanVien == true)
            {
                queryChuaKham = queryable(queryInfo.AdditionalSearchString, null);
            }
            if (queryString.DangKhamNhanVien == true)
            {
                queryDangKham = queryable(queryInfo.AdditionalSearchString, false);
            }
            if (queryString.DaKhamNhanVien == true)
            {
                queryDaKham = queryable(queryInfo.AdditionalSearchString, true);
            }
            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new BaoCaoKhamDoanHopDongTheoNhanVienVo());
            if (queryChuaKham != null)
            {
                query = query.Concat(queryChuaKham);
            }
            if (queryDangKham != null)
            {
                query = query.Concat(queryDangKham);
            }
            if (queryDaKham != null)
            {
                query = query.Concat(queryDaKham);
            }
            //var queryTask = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            //return new GridDataSource { Data = queryTask, TotalRowCount = queryTask.Count() };
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridAsyncTheoNhanVien(QueryInfo queryInfo, bool exportExcel)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<BaoCaoKhamDoanHopDongTheoNhanVienVo>().ToArray(), TotalRowCount = 0 };
            }

            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> 
                { 
                    new Sort { Field = nameof(BaoCaoKhamDoanHopDongTheoNhanVienVo.NgayHoanThanhTatCa), Dir = "desc" }, 
                    new Sort { Field = nameof(BaoCaoKhamDoanHopDongTheoNhanVienVo.NgayHopDong), Dir = "desc" },
                    new Sort { Field = nameof(BaoCaoKhamDoanHopDongTheoNhanVienVo.Id), Dir = "desc" },
                };
            }

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var baoCaoKhamDoanHopDongTheoNhanVienVos = new List<BaoCaoKhamDoanHopDongTheoNhanVienVo>();
            var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongTheoNhanVienVo>(queryInfo.AdditionalSearchString);
            
            if (queryString.HopDongKhamSucKhoeId != null)
            {
                DateTime tuNgay = new DateTime(1970, 1, 1);
                DateTime denNgay = DateTime.Now;
                if (!string.IsNullOrEmpty(queryString.FromDate1) || !string.IsNullOrEmpty(queryString.ToDate1))
                {
                    queryString.FromDate1.TryParseExactCustom(out tuNgay);

                    if (!string.IsNullOrEmpty(queryString.ToDate1))
                    {
                        queryString.ToDate1.TryParseExactCustom(out denNgay);
                    }
                }

                var hopDong = _hopDongKhamSucKhoeRepository.TableNoTracking
                        .Where(o=>o.Id == queryString.HopDongKhamSucKhoeId)
                        .Select(s => new
                        {
                            Id = s.Id,
                            TenCongTyKhamSucKhoe = s.CongTyKhamSucKhoe.Ten,
                            TenHopDongKhamSucKhoe = s.SoHopDong,
                            SoLuongNhanVienTheoHopDong = s.HopDongKhamSucKhoeNhanViens.Count(),
                            NgayHopDong = s.NgayHopDong,
                            HopDongKhamSucKhoeNhanViens = s.HopDongKhamSucKhoeNhanViens.Select(o => new
                            {
                                o.Id,
                                MaBN = o.BenhNhan.MaBN,
                                HoTen = o.HoTen,
                                MaNV = o.MaNhanVien,
                                NamSinh = o.NamSinh,
                                GioiTinh = o.GioiTinh,
                            }).ToList(),
                        }).FirstOrDefault();

                #region Cập nhật 09/12/2022: grid load chậm
                //var yeuCauTiepNhanKhamSucKhoeData = _yeuCauTiepNhanRepository.TableNoTracking
                //            .Where(o => o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && o.HopDongKhamSucKhoeNhanVienId != null && o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId)
                //            .Select(o => new
                //            {
                //                o.Id,
                //                MaTN = o.MaYeuCauTiepNhan,
                //                o.ThoiDiemTiepNhan,
                //                HopDongKhamSucKhoeNhanVienId = o.HopDongKhamSucKhoeNhanVienId,

                //                YeuCauKhams = o.YeuCauKhamBenhs.Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && kb.GoiKhamSucKhoeId != null)
                //                    .Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
                //                    {
                //                        YeuCauKhamBenhId = k.Id,
                //                        TrangThai = k.TrangThai,
                //                        TenDichVu = k.TenDichVu,
                //                        ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
                //                        ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
                //                    }).ToList(),
                //                YeuCauDichVuKyThuatVos = o.YeuCauDichVuKyThuats.Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && kt.GoiKhamSucKhoeId != null)
                //                    .Select(kt => new HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
                //                    {
                //                        YeuCauDichVuKyThuatId = kt.Id,
                //                        TrangThai = kt.TrangThai,
                //                        LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
                //                        TenDichVu = kt.TenDichVu,
                //                        ThoiDiemThucHien = kt.ThoiDiemThucHien,
                //                        CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
                //                        ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh,
                //                    }).ToList()
                //            }).ToList();

                var lstYCKB = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && kb.GoiKhamSucKhoeId != null
                                && kb.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && kb.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                && kb.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId)
                        .Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
                        {
                            YeuCauTiepNhanId = k.YeuCauTiepNhanId,
                            MaTN = k.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ThoiDiemTiepNhan = k.YeuCauTiepNhan.ThoiDiemTiepNhan,
                            HopDongKhamSucKhoeId = k.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                            HopDongKhamSucKhoeNhanVienId = k.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                            YeuCauKhamBenhId = k.Id,
                            TrangThai = k.TrangThai,
                            TenDichVu = k.TenDichVu,
                            ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
                            ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
                        }).ToList();

                var lstYCKT = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && kt.GoiKhamSucKhoeId != null
                                && kt.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                && kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId)
                        .Select(kt => new HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
                        {
                            YeuCauTiepNhanId = kt.YeuCauTiepNhanId,
                            MaTN = kt.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ThoiDiemTiepNhan = kt.YeuCauTiepNhan.ThoiDiemTiepNhan,
                            HopDongKhamSucKhoeId = kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                            HopDongKhamSucKhoeNhanVienId = kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                            YeuCauDichVuKyThuatId = kt.Id,
                            TrangThai = kt.TrangThai,
                            LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
                            TenDichVu = kt.TenDichVu,
                            ThoiDiemThucHien = kt.ThoiDiemThucHien,
                            CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
                            ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh
                        }).ToList();

                var lstYCTN = lstYCKB.Select(x => new { x.YeuCauTiepNhanId, x.HopDongKhamSucKhoeNhanVienId, x.MaTN, x.ThoiDiemTiepNhan, x.HopDongKhamSucKhoeId })
                            .Union(lstYCKT.Select(x => new { x.YeuCauTiepNhanId, x.HopDongKhamSucKhoeNhanVienId, x.MaTN, x.ThoiDiemTiepNhan, x.HopDongKhamSucKhoeId }))
                            .Distinct().ToList();

                var lstYCKBDic = lstYCKB.GroupBy(x => x.YeuCauTiepNhanId).ToDictionary(x => x.Key, x => x.Select(a => a).ToList());
                var lstYCKTDic = lstYCKT.GroupBy(x => x.YeuCauTiepNhanId).ToDictionary(x => x.Key, x => x.Select(a => a).ToList());

                var yeuCauTiepNhanKhamSucKhoeData = lstYCTN.Select(x => new
                {
                    Id = x.YeuCauTiepNhanId,
                    MaTN = x.MaTN,
                    ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                    HopDongKhamSucKhoeId = x.HopDongKhamSucKhoeId,
                    HopDongKhamSucKhoeNhanVienId = x.HopDongKhamSucKhoeNhanVienId,
                    YeuCauKhams = lstYCKBDic.TryGetValue(x.YeuCauTiepNhanId, out var lstKB) ? lstKB : new List<HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo>(),
                    YeuCauDichVuKyThuatVos = lstYCKTDic.TryGetValue(x.YeuCauTiepNhanId, out var lstKT) ? lstKT : new List<HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo>()
                }).ToList();

                lstYCKB = null;
                lstYCKT = null;
                lstYCKBDic = null;
                lstYCKTDic = null;
                lstYCTN = null;
                #endregion

                if (queryString.DaKhamNhanVien != true) // Chưa có 1 dịch vụ nào đã hoàn thành/ thực hiện của nhân viên theo hợp đồng
                {                    
                    var hopDongKhamSucKhoeNhanVienChuaKhams = yeuCauTiepNhanKhamSucKhoeData
                        .Where(o => o.YeuCauKhams.All(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang).All(kt => !kt.CoKetQuaCDHA))
                        .ToList();

                    foreach (var hopDongKhamSucKhoeNhanVienChuaKham in hopDongKhamSucKhoeNhanVienChuaKhams)
                    {

                        var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
                        {
                            Id = hopDongKhamSucKhoeNhanVienChuaKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
                            MaTN = hopDongKhamSucKhoeNhanVienChuaKham.MaTN,
                            DichVuKhamBenhChuaKham = string.Join(", ", hopDongKhamSucKhoeNhanVienChuaKham.YeuCauKhams.Select(o => o.TenDichVu)),
                            DichVuKyThuatChuaThucHien = string.Join(", ", hopDongKhamSucKhoeNhanVienChuaKham.YeuCauDichVuKyThuatVos.Select(o => o.TenDichVu)),
                            ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienChuaKham.ThoiDiemTiepNhan
                        };

                        if(hopDong != null)
                        {
                            var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienChuaKham.HopDongKhamSucKhoeNhanVienId);
                            if (hopDongKhamSucKhoeNhanVien != null)
                            {
                                baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                            }
                        }

                        baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
                    }
                }
                else //Có ít nhất 1 dịch vụ đã hoàn thành/ thực hiện của nhân viên trong hợp đồng
                {
                    var hopDongKhamSucKhoeNhanVienCoDvHoanThanhs = yeuCauTiepNhanKhamSucKhoeData
                        .Where(o => o.YeuCauKhams.Any(kb => kb.ThoiDiemHoanThanh >= tuNgay && kb.ThoiDiemHoanThanh <= denNgay && kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                || o.YeuCauDichVuKyThuatVos
                                        .Any(kt => ((kt.ThoiDiemDuyetKetQua != null && kt.ThoiDiemDuyetKetQua >= tuNgay && kt.ThoiDiemDuyetKetQua <= denNgay) || (kt.ThoiDiemDuyetKetQua == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay))
                                                    && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                || o.YeuCauDichVuKyThuatVos
                                        .Any(kt => ((kt.ThoiDiemDuyetKetQua != null && kt.ThoiDiemDuyetKetQua >= tuNgay && kt.ThoiDiemDuyetKetQua <= denNgay) || (kt.ThoiDiemDuyetKetQua == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay))
                                                    && (kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && kt.CoKetQuaCDHA))
                        .ToList();

                    foreach (var hopDongKhamSucKhoeNhanVienDangKham in hopDongKhamSucKhoeNhanVienCoDvHoanThanhs)
                    {

                        var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
                        {
                            Id = hopDongKhamSucKhoeNhanVienDangKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
                            MaTN = hopDongKhamSucKhoeNhanVienDangKham.MaTN,
                            DichVuKhamBenhChuaKham = string.Join(", ", hopDongKhamSucKhoeNhanVienDangKham.YeuCauKhams.Where(o => o.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham).Select(o => o.TenDichVu)),
                            DichVuKyThuatChuaThucHien = string.Join(", ", hopDongKhamSucKhoeNhanVienDangKham.YeuCauDichVuKyThuatVos
                                                                            .Where(kt => (kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                                                    || ((kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && !kt.CoKetQuaCDHA))
                                                                            .Select(o => o.TenDichVu)),
                            //lấy thời điểm hoàn thành chuyên khoa đầu tiên
                            //ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienDangKham.ThoiDiemTiepNhan
                        };

                        var thoiDiemHoanThanhs = hopDongKhamSucKhoeNhanVienDangKham.YeuCauKhams
                            .Where(o => o.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham && o.ThoiDiemHoanThanh != null).Select(o => o.ThoiDiemHoanThanh)
                            .Concat(hopDongKhamSucKhoeNhanVienDangKham.YeuCauDichVuKyThuatVos
                                        .Where(o => (o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                    || ((o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && o.CoKetQuaCDHA))
                                        .Select(o => o.ThoiDiemDuyetKetQua ?? o.ThoiDiemThucHien))
                            .ToList();
                        baoCaoKhamDoanHopDongTheoNhanVienVo.ThoiDiemThucHien = thoiDiemHoanThanhs.Where(o => o != null).OrderBy(o => o).FirstOrDefault();
                        baoCaoKhamDoanHopDongTheoNhanVienVo.NgayHoanThanhTatCa = thoiDiemHoanThanhs.Where(o => o != null).OrderBy(o => o).LastOrDefault();

                        if (hopDong != null)
                        {
                            var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDangKham.HopDongKhamSucKhoeNhanVienId);
                            if (hopDongKhamSucKhoeNhanVien != null)
                            {
                                baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                            }
                        }

                        baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
                    }
                }
            }
            else
            {
                DateTime tuNgay = new DateTime(1970, 1, 1);
                DateTime denNgay = DateTime.Now;
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    queryString.FromDate.TryParseExactCustom(out tuNgay);

                    if (!string.IsNullOrEmpty(queryString.ToDate))
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }
                }

                var hopDongs = _hopDongKhamSucKhoeRepository.TableNoTracking
                        .Where(o => o.DaKetThuc != true)
                        .Select(s => new
                        {
                            Id = s.Id,
                            TenCongTyKhamSucKhoe = s.CongTyKhamSucKhoe.Ten,
                            TenHopDongKhamSucKhoe = s.SoHopDong,
                            SoLuongNhanVienTheoHopDong = s.HopDongKhamSucKhoeNhanViens.Count(),
                            NgayHopDong = s.NgayHopDong,
                            HopDongKhamSucKhoeNhanViens = s.HopDongKhamSucKhoeNhanViens.Select(o => new
                            {
                                o.Id,
                                MaBN = o.BenhNhan.MaBN,
                                HoTen = o.HoTen,
                                MaNV = o.MaNhanVien,
                                NamSinh = o.NamSinh,
                                GioiTinh = o.GioiTinh,
                            }).ToList(),
                        }).ToList();
                var hopDongKhamSucKhoeIds = hopDongs.Select(o => o.Id).ToList();

                #region Cập nhật 09/12/2022: grid load chậm
                //var yeuCauTiepNhanKhamSucKhoeData = _yeuCauTiepNhanRepository.TableNoTracking
                //            .Where(o => o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && o.HopDongKhamSucKhoeNhanVienId != null && hopDongKhamSucKhoeIds.Contains(o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId))
                //            .Select(o => new
                //            {
                //                o.Id,
                //                MaTN = o.MaYeuCauTiepNhan,
                //                o.ThoiDiemTiepNhan,
                //                HopDongKhamSucKhoeNhanVienId = o.HopDongKhamSucKhoeNhanVienId,

                //                YeuCauKhams = o.YeuCauKhamBenhs.Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && kb.GoiKhamSucKhoeId != null)
                //                    .Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
                //                    {
                //                        YeuCauKhamBenhId = k.Id,
                //                        TrangThai = k.TrangThai,
                //                        TenDichVu = k.TenDichVu,
                //                        ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
                //                        ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
                //                    }).ToList(),
                //                YeuCauDichVuKyThuatVos = o.YeuCauDichVuKyThuats.Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && kt.GoiKhamSucKhoeId != null)
                //                    .Select(kt => new HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
                //                    {
                //                        YeuCauDichVuKyThuatId = kt.Id,
                //                        TrangThai = kt.TrangThai,
                //                        LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
                //                        TenDichVu = kt.TenDichVu,
                //                        ThoiDiemThucHien = kt.ThoiDiemThucHien,
                //                        CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
                //                        ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh,
                //                    }).ToList()
                //            }).ToList();

                var lstYCKB = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && kb.GoiKhamSucKhoeId != null
                                && kb.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && kb.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                && hopDongKhamSucKhoeIds.Contains(kb.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId))
                        .Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
                        {
                            YeuCauTiepNhanId = k.YeuCauTiepNhanId,
                            MaTN = k.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ThoiDiemTiepNhan = k.YeuCauTiepNhan.ThoiDiemTiepNhan,
                            HopDongKhamSucKhoeId = k.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                            HopDongKhamSucKhoeNhanVienId = k.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                            YeuCauKhamBenhId = k.Id,
                            TrangThai = k.TrangThai,
                            TenDichVu = k.TenDichVu,
                            ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
                            ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
                        }).ToList();

                var lstYCKT = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && kt.GoiKhamSucKhoeId != null
                                && kt.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                && hopDongKhamSucKhoeIds.Contains(kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId))
                        .Select(kt => new HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
                        {
                            YeuCauTiepNhanId = kt.YeuCauTiepNhanId,
                            MaTN = kt.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            ThoiDiemTiepNhan = kt.YeuCauTiepNhan.ThoiDiemTiepNhan,
                            HopDongKhamSucKhoeId = kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                            HopDongKhamSucKhoeNhanVienId = kt.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                            YeuCauDichVuKyThuatId = kt.Id,
                            TrangThai = kt.TrangThai,
                            LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
                            TenDichVu = kt.TenDichVu,
                            ThoiDiemThucHien = kt.ThoiDiemThucHien,
                            CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
                            ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh
                        }).ToList();

                var lstYCTN = lstYCKB.Select(x => new { x.YeuCauTiepNhanId, x.HopDongKhamSucKhoeNhanVienId, x.MaTN, x.ThoiDiemTiepNhan, x.HopDongKhamSucKhoeId })
                            .Union(lstYCKT.Select(x => new { x.YeuCauTiepNhanId, x.HopDongKhamSucKhoeNhanVienId, x.MaTN, x.ThoiDiemTiepNhan, x.HopDongKhamSucKhoeId }))
                            .Distinct().ToList();

                var lstYCKBDic = lstYCKB.GroupBy(x => x.YeuCauTiepNhanId).ToDictionary(x => x.Key, x => x.Select(a => a).ToList());
                var lstYCKTDic = lstYCKT.GroupBy(x => x.YeuCauTiepNhanId).ToDictionary(x => x.Key, x => x.Select(a => a).ToList());

                var yeuCauTiepNhanKhamSucKhoeData = lstYCTN.Select(x => new
                {
                    Id = x.YeuCauTiepNhanId,
                    MaTN = x.MaTN,
                    ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,
                    HopDongKhamSucKhoeId = x.HopDongKhamSucKhoeId,
                    HopDongKhamSucKhoeNhanVienId = x.HopDongKhamSucKhoeNhanVienId,
                    YeuCauKhams = lstYCKBDic.TryGetValue(x.YeuCauTiepNhanId, out var lstKB) ? lstKB : new List<HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo>(),
                    YeuCauDichVuKyThuatVos = lstYCKTDic.TryGetValue(x.YeuCauTiepNhanId, out var lstKT) ? lstKT : new List<HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo>()
                }).ToList();

                lstYCKB = null;
                lstYCKT = null;
                lstYCKBDic = null;
                lstYCKTDic = null;
                lstYCTN = null;
                #endregion

                if (queryString.ChuaKhamNhanVien == true)// Chưa có 1 dịch vụ (khám bệnh/ kỹ thuật) nào được hoàn thành/ thực hiện
                {
                    var hopDongKhamSucKhoeNhanVienChuaKhams = yeuCauTiepNhanKhamSucKhoeData
                        .Where(o => o.YeuCauKhams.All(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang).All(kt => !kt.CoKetQuaCDHA))
                        .ToList();

                    foreach (var hopDongKhamSucKhoeNhanVienChuaKham in hopDongKhamSucKhoeNhanVienChuaKhams)
                    {

                        var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
                        {
                            Id = hopDongKhamSucKhoeNhanVienChuaKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
                            MaTN = hopDongKhamSucKhoeNhanVienChuaKham.MaTN,
                            DichVuKhamBenhChuaKham = string.Join(", ", hopDongKhamSucKhoeNhanVienChuaKham.YeuCauKhams.Select(o=>o.TenDichVu)),
                            DichVuKyThuatChuaThucHien = string.Join(", ", hopDongKhamSucKhoeNhanVienChuaKham.YeuCauDichVuKyThuatVos.Select(o => o.TenDichVu)),
                            ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienChuaKham.ThoiDiemTiepNhan
                        };

                        #region Cập nhật 09/12/2022: grid load chậm
                        //foreach (var hopDong in hopDongs)
                        //{
                        //    var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienChuaKham.HopDongKhamSucKhoeNhanVienId);
                        //    if (hopDongKhamSucKhoeNhanVien != null)
                        //    {
                        //        baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                        //        baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                        //        baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                        //        baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                        //        baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                        //        baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                        //        baoCaoKhamDoanHopDongTheoNhanVienVo.NgayHopDong = hopDong.NgayHopDong;
                        //        break;
                        //    }
                        //}

                        var hopDong = hopDongs.FirstOrDefault(x => x.Id == hopDongKhamSucKhoeNhanVienChuaKham.HopDongKhamSucKhoeId);
                        if (hopDong != null)
                        {
                            var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienChuaKham.HopDongKhamSucKhoeNhanVienId);
                            if (hopDongKhamSucKhoeNhanVien != null)
                            {
                                baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                                baoCaoKhamDoanHopDongTheoNhanVienVo.NgayHopDong = hopDong.NgayHopDong;
                            }
                        }
                        #endregion

                        baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
                    }
                }
                else
                {
                    var hopDongKhamSucKhoeNhanVienCoDvHoanThanhs = yeuCauTiepNhanKhamSucKhoeData
                        .Where(o => o.YeuCauKhams.Any(kb => kb.ThoiDiemHoanThanh >= tuNgay && kb.ThoiDiemHoanThanh <= denNgay && kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                || o.YeuCauDichVuKyThuatVos
                                        .Any(kt => ((kt.ThoiDiemDuyetKetQua != null && kt.ThoiDiemDuyetKetQua >= tuNgay && kt.ThoiDiemDuyetKetQua <= denNgay) || (kt.ThoiDiemDuyetKetQua == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay))
                                                    && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                || o.YeuCauDichVuKyThuatVos
                                        .Any(kt => ((kt.ThoiDiemDuyetKetQua != null && kt.ThoiDiemDuyetKetQua >= tuNgay && kt.ThoiDiemDuyetKetQua <= denNgay) || (kt.ThoiDiemDuyetKetQua == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay))
                                                    && (kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && kt.CoKetQuaCDHA))
                        .ToList();

                    if (queryString.DaKhamNhanVien == true) // Hoàn thành tất cả dịch vụ (khám bệnh/ kỹ thuật)
                    {
                        var hopDongKhamSucKhoeNhanVienDaKhams = hopDongKhamSucKhoeNhanVienCoDvHoanThanhs
                        .Where(o => o.YeuCauKhams.All(kb => kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.CoKetQuaCDHA))
                        .ToList();

                        foreach (var hopDongKhamSucKhoeNhanVienDaKham in hopDongKhamSucKhoeNhanVienDaKhams)
                        {
                            var thoiDiemHoanThanhs = hopDongKhamSucKhoeNhanVienDaKham.YeuCauKhams
                            .Where(o => o.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham && o.ThoiDiemHoanThanh != null).Select(o => o.ThoiDiemHoanThanh)
                            .Concat(hopDongKhamSucKhoeNhanVienDaKham.YeuCauDichVuKyThuatVos
                                        .Where(o => (o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                    || ((o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && o.CoKetQuaCDHA))
                                        .Select(o => o.ThoiDiemDuyetKetQua ?? o.ThoiDiemThucHien))
                            .ToList();

                            if(thoiDiemHoanThanhs.All(o=> o == null || o <= denNgay))
                            {
                                var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
                                {
                                    Id = hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
                                    MaTN = hopDongKhamSucKhoeNhanVienDaKham.MaTN,
                                    DichVuKhamBenhDaKham = string.Join(", ", hopDongKhamSucKhoeNhanVienDaKham.YeuCauKhams.Select(o => o.TenDichVu)),
                                    DichVuKyThuatDaThucHien = string.Join(", ", hopDongKhamSucKhoeNhanVienDaKham.YeuCauDichVuKyThuatVos.Select(o => o.TenDichVu)),
                                    ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienDaKham.ThoiDiemTiepNhan
                                };
                                baoCaoKhamDoanHopDongTheoNhanVienVo.NgayHoanThanhTatCa = thoiDiemHoanThanhs.Where(o => o != null).OrderBy(o => o).LastOrDefault();

                                #region Cập nhật 09/12/2022: grid load chậm
                                //foreach (var hopDong in hopDongs)
                                //{
                                //    var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeNhanVienId);
                                //    if (hopDongKhamSucKhoeNhanVien != null)
                                //    {
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                                //        break;
                                //    }
                                //}

                                var hopDong = hopDongs.FirstOrDefault(x => x.Id == hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeId);
                                if (hopDong != null)
                                {
                                    var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeNhanVienId);
                                    if (hopDongKhamSucKhoeNhanVien != null)
                                    {
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                                    }
                                }
                                #endregion

                                baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
                            }
                        }
                    }
                    else // Có ít nhất 1 dịch vụ (khám bệnh/ kỹ thuật) đã hoàn thành/ thực hiện
                    {
                        var hopDongKhamSucKhoeNhanVienDangKhams = hopDongKhamSucKhoeNhanVienCoDvHoanThanhs
                        .Where(o => !(o.YeuCauKhams.All(kb => kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.CoKetQuaCDHA)))
                        .ToList();

                        foreach (var hopDongKhamSucKhoeNhanVienDangKham in hopDongKhamSucKhoeNhanVienDangKhams)
                        {

                            var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
                            {
                                Id = hopDongKhamSucKhoeNhanVienDangKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
                                MaTN = hopDongKhamSucKhoeNhanVienDangKham.MaTN,
                                DichVuKhamBenhChuaKham = string.Join(", ", hopDongKhamSucKhoeNhanVienDangKham.YeuCauKhams.Where(o=> o.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham || o.ThoiDiemHoanThanh > denNgay).Select(o => o.TenDichVu)),
                                DichVuKyThuatChuaThucHien = string.Join(", ", hopDongKhamSucKhoeNhanVienDangKham.YeuCauDichVuKyThuatVos
                                                                                .Where(kt =>  (kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                                                        || ((kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && !kt.CoKetQuaCDHA)
                                                                                        || (kt.ThoiDiemDuyetKetQua > denNgay || kt.ThoiDiemThucHien > denNgay))
                                                                                .Select(o => o.TenDichVu)),
                                ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienDangKham.ThoiDiemTiepNhan
                            };

                            #region Cập nhật 09/12/2022: grid load chậm
                            //foreach (var hopDong in hopDongs)
                            //{
                            //    var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDangKham.HopDongKhamSucKhoeNhanVienId);
                            //    if (hopDongKhamSucKhoeNhanVien != null)
                            //    {
                            //        baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                            //        baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                            //        baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                            //        baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                            //        baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                            //        baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                            //        baoCaoKhamDoanHopDongTheoNhanVienVo.NgayHopDong = hopDong.NgayHopDong;
                            //        break;
                            //    }
                            //}

                            var hopDong = hopDongs.FirstOrDefault(x => x.Id == hopDongKhamSucKhoeNhanVienDangKham.HopDongKhamSucKhoeId);
                            if (hopDong != null)
                            {
                                var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDangKham.HopDongKhamSucKhoeNhanVienId);
                                if (hopDongKhamSucKhoeNhanVien != null)
                                {
                                    baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                                    baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                                    baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                                    baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                                    baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                                    baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                                    baoCaoKhamDoanHopDongTheoNhanVienVo.NgayHopDong = hopDong.NgayHopDong;
                                }
                            }
                            #endregion

                            baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
                        }

                        var hopDongKhamSucKhoeNhanVienDaKhams = hopDongKhamSucKhoeNhanVienCoDvHoanThanhs
                        .Where(o => o.YeuCauKhams.All(kb => kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.CoKetQuaCDHA))
                        .ToList();

                        foreach (var hopDongKhamSucKhoeNhanVienDaKham in hopDongKhamSucKhoeNhanVienDaKhams)
                        {
                            var thoiDiemHoanThanhs = hopDongKhamSucKhoeNhanVienDaKham.YeuCauKhams
                            .Where(o => o.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham && o.ThoiDiemHoanThanh != null).Select(o => o.ThoiDiemHoanThanh)
                            .Concat(hopDongKhamSucKhoeNhanVienDaKham.YeuCauDichVuKyThuatVos
                                        .Where(o => (o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                    || ((o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && o.CoKetQuaCDHA))
                                        .Select(o => o.ThoiDiemDuyetKetQua ?? o.ThoiDiemThucHien))
                            .ToList();

                            if (thoiDiemHoanThanhs.Any(o => o > denNgay))
                            {
                                var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
                                {
                                    Id = hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
                                    MaTN = hopDongKhamSucKhoeNhanVienDaKham.MaTN,
                                    DichVuKhamBenhChuaKham = string.Join(", ", hopDongKhamSucKhoeNhanVienDaKham.YeuCauKhams.Where(o => o.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham || o.ThoiDiemHoanThanh > denNgay).Select(o => o.TenDichVu)),
                                    DichVuKyThuatChuaThucHien = string.Join(", ", hopDongKhamSucKhoeNhanVienDaKham.YeuCauDichVuKyThuatVos
                                                                                .Where(kt => (kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                                                        || ((kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && !kt.CoKetQuaCDHA)
                                                                                        || (kt.ThoiDiemDuyetKetQua > denNgay || kt.ThoiDiemThucHien > denNgay))
                                                                                .Select(o => o.TenDichVu)),
                                    ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienDaKham.ThoiDiemTiepNhan
                                };

                                #region Cập nhật 09/12/2022: grid load chậm
                                //foreach (var hopDong in hopDongs)
                                //{
                                //    var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeNhanVienId);
                                //    if (hopDongKhamSucKhoeNhanVien != null)
                                //    {
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                                //        baoCaoKhamDoanHopDongTheoNhanVienVo.NgayHopDong = hopDong.NgayHopDong;
                                //        break;
                                //    }
                                //}

                                var hopDong = hopDongs.FirstOrDefault(x => x.Id == hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeId);
                                if (hopDong != null)
                                {
                                    var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeNhanVienId);
                                    if (hopDongKhamSucKhoeNhanVien != null)
                                    {
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
                                        baoCaoKhamDoanHopDongTheoNhanVienVo.NgayHopDong = hopDong.NgayHopDong;
                                    }
                                }
                                #endregion

                                baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
                            }
                        }
                    }
                }
            }

            return new GridDataSource { Data = baoCaoKhamDoanHopDongTheoNhanVienVos.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = baoCaoKhamDoanHopDongTheoNhanVienVos.Count() };
        }

        //private List<BaoCaoKhamDoanHopDongTheoNhanVienVo> GetDataBaoCaoKhamDoanHopDongTheoNhanVien(string additionalSearchString, bool? trangThai)
        //{
        //    var baoCaoKhamDoanHopDongTheoNhanVienVos = new List<BaoCaoKhamDoanHopDongTheoNhanVienVo>();
        //    var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongTheoNhanVienVo>(additionalSearchString);
        //    DateTime tuNgay = new DateTime(1970, 1, 1);
        //    DateTime denNgay = DateTime.Now;
        //    if (!string.IsNullOrEmpty(queryString.FromDate1) || !string.IsNullOrEmpty(queryString.ToDate1))
        //    {
        //        queryString.FromDate1.TryParseExactCustom(out tuNgay);

        //        if (!string.IsNullOrEmpty(queryString.ToDate1))
        //        {
        //            queryString.ToDate1.TryParseExactCustom(out denNgay);
        //        }
        //    }

        //    if (queryString.HopDongKhamSucKhoeId != null)
        //    {
        //        var hopDongKhamSucKhoeNhanViens = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
        //                    .Where(o => o.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId)
        //                    .Select(o => new
        //                    {
        //                        HopDongKhamSucKhoeId = o.HopDongKhamSucKhoeId,
        //                        HopDongKhamSucKhoeNhanVienId = o.Id,
        //                        MaBN = o.BenhNhan.MaBN,
        //                        HoTen = o.HoTen,
        //                        MaNV = o.MaNhanVien,
        //                        NamSinh = o.NamSinh,
        //                        GioiTinh = o.GioiTinh,
        //                        TenCongTyKhamSucKhoe = o.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
        //                        YeuCauTiepNhans = o.YeuCauTiepNhans.Where(tn=>tn.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
        //                                            .Select(tn=>new
        //                                            {
        //                                                tn.Id,
        //                                                MaTN = tn.MaYeuCauTiepNhan,
        //                                                tn.ThoiDiemTiepNhan
        //                                            }).ToList()
                                
        //                    }).ToList();
        //        var yeuCauTiepNhanIds = hopDongKhamSucKhoeNhanViens.SelectMany(o=>o.YeuCauTiepNhans).Select(o=>o.Id).ToList();

        //        if (trangThai != true) // Chưa có 1 dịch vụ nào đã hoàn thành/ thực hiện của nhân viên theo hợp đồng
        //        {
        //            var yeuCauKhamVos = _yeuCauKhamBenhRepository.TableNoTracking.Where(kb => yeuCauTiepNhanIds.Contains(kb.YeuCauTiepNhanId) && kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && kb.GoiKhamSucKhoeId != null)
        //                    .Select(k => new
        //                    {
        //                        YeuCauTiepNhanId = k.YeuCauTiepNhanId,
        //                        TenDichVu = k.TenDichVu,
        //                        YeuCauKhamBenhId = k.Id,
        //                        TrangThai = k.TrangThai,
        //                        ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
        //                        ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
        //                    }).ToList();
        //            var yeuCauDichVuKyThuatVos = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(kt => yeuCauTiepNhanIds.Contains(kt.YeuCauTiepNhanId) && kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && kt.GoiKhamSucKhoeId != null)
        //                   .Select(kt => new
        //                   {
        //                       YeuCauTiepNhanId = kt.YeuCauTiepNhanId,
        //                       YeuCauDichVuKyThuatId = kt.Id,
        //                       TrangThai = kt.TrangThai,
        //                       LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
        //                       TenDichVu = kt.TenDichVu,
        //                       ThoiDiemThucHien = kt.ThoiDiemThucHien,
        //                       CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
        //                       ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh,
        //                    }).ToList();

        //            foreach(var hopDongKhamSucKhoeNhanVien in hopDongKhamSucKhoeNhanViens)
        //            {
        //                var yctn = hopDongKhamSucKhoeNhanVien.YeuCauTiepNhans.OrderBy(o => o.Id).FirstOrDefault();
        //                if(yctn != null)
        //                {
        //                    var yeuCauKhamTheoTiepNhans = yeuCauKhamVos.Where(o => o.YeuCauTiepNhanId == yctn.Id).ToList();
        //                    var yeuCauDichVuKyThuatTheoTiepNhans = yeuCauDichVuKyThuatVos.Where(o => o.YeuCauTiepNhanId == yctn.Id).ToList();
        //                    if(yeuCauKhamTheoTiepNhans.All(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham)
        //                        && yeuCauDichVuKyThuatTheoTiepNhans.Where(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
        //                        && yeuCauDichVuKyThuatTheoTiepNhans.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang).All(kt => !kt.CoKetQuaCDHA))
        //                    {
        //                        var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
        //                        {
        //                            Id = hopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeNhanVienId,
        //                            MaBN = hopDongKhamSucKhoeNhanVien.MaBN,
        //                            MaTN = yctn.MaTN,
        //                            HoTen = hopDongKhamSucKhoeNhanVien.HoTen,
        //                            MaNV = hopDongKhamSucKhoeNhanVien.MaNV,
        //                            NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh,
        //                            GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh,
        //                            TenCongTyKhamSucKhoe = hopDongKhamSucKhoeNhanVien.TenCongTyKhamSucKhoe,
        //                            //DichVuKhamBenhChuaKham = dichVuKhamBenhChuaKham,
        //                            //DichVuKhamBenhDaKham = dichVuKhamBenhDaKham,
        //                            ThoiDiemThucHien = yctn.ThoiDiemTiepNhan
        //                        };
        //                        baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
        //                    }
        //                }
        //            }
        //        }
        //        else //Có ít nhất 1 dịch vụ đã hoàn thành/ thực hiện của nhân viên trong hợp đồng
        //        {
        //            var yeuCauKhamVos = _yeuCauKhamBenhRepository.TableNoTracking.Where(kb => yeuCauTiepNhanIds.Contains(kb.YeuCauTiepNhanId) && kb.ThoiDiemHoanThanh >= tuNgay && kb.ThoiDiemHoanThanh <= denNgay && kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham && kb.GoiKhamSucKhoeId != null)
        //                    .Select(k => new
        //                    {
        //                        YeuCauTiepNhanId = k.YeuCauTiepNhanId,
        //                        TenDichVu = k.TenDichVu,
        //                        YeuCauKhamBenhId = k.Id,
        //                        TrangThai = k.TrangThai,
        //                        ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
        //                        ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
        //                    }).ToList();
        //            var yeuCauDichVuKyThuatVos = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(kt => yeuCauTiepNhanIds.Contains(kt.YeuCauTiepNhanId) && kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && kt.GoiKhamSucKhoeId != null
        //                                                                                                    && ((kt.ThoiDiemHoanThanh != null && kt.ThoiDiemHoanThanh >= tuNgay && kt.ThoiDiemHoanThanh <= denNgay) || (kt.ThoiDiemHoanThanh == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay)))
        //                   .Select(kt => new
        //                   {
        //                       YeuCauTiepNhanId = kt.YeuCauTiepNhanId,
        //                       YeuCauDichVuKyThuatId = kt.Id,
        //                       TrangThai = kt.TrangThai,
        //                       LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
        //                       TenDichVu = kt.TenDichVu,
        //                       ThoiDiemThucHien = kt.ThoiDiemThucHien,
        //                       CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
        //                       ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh,
        //                   }).ToList();

        //            foreach (var hopDongKhamSucKhoeNhanVien in hopDongKhamSucKhoeNhanViens)
        //            {
        //                var yctn = hopDongKhamSucKhoeNhanVien.YeuCauTiepNhans.OrderBy(o => o.Id).FirstOrDefault();
        //                if (yctn != null)
        //                {
        //                    var yeuCauKhamTheoTiepNhans = yeuCauKhamVos.Where(o => o.YeuCauTiepNhanId == yctn.Id).ToList();
        //                    var yeuCauDichVuKyThuatTheoTiepNhans = yeuCauDichVuKyThuatVos.Where(o => o.YeuCauTiepNhanId == yctn.Id).ToList();
        //                    if (yeuCauKhamTheoTiepNhans.Any()
        //                        || yeuCauDichVuKyThuatTheoTiepNhans.Any(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
        //                        || yeuCauDichVuKyThuatTheoTiepNhans.Any(kt => (kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && kt.CoKetQuaCDHA))
        //                    {
        //                        var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
        //                        {
        //                            Id = hopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeNhanVienId,
        //                            MaBN = hopDongKhamSucKhoeNhanVien.MaBN,
        //                            MaTN = yctn.MaTN,
        //                            HoTen = hopDongKhamSucKhoeNhanVien.HoTen,
        //                            MaNV = hopDongKhamSucKhoeNhanVien.MaNV,
        //                            NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh,
        //                            GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh,
        //                            TenCongTyKhamSucKhoe = hopDongKhamSucKhoeNhanVien.TenCongTyKhamSucKhoe,
        //                            //DichVuKhamBenhChuaKham = dichVuKhamBenhChuaKham,
        //                            //DichVuKhamBenhDaKham = dichVuKhamBenhDaKham,
        //                            ThoiDiemThucHien = yctn.ThoiDiemTiepNhan
        //                        };
        //                        baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var hopDongs = _hopDongKhamSucKhoeRepository.TableNoTracking
        //                .Select(s => new
        //                {
        //                    Id = s.Id,
        //                    TenCongTyKhamSucKhoe = s.CongTyKhamSucKhoe.Ten,
        //                    TenHopDongKhamSucKhoe = s.SoHopDong,
        //                    SoLuongNhanVienTheoHopDong = s.HopDongKhamSucKhoeNhanViens.Count(),
        //                    NgayHopDong = s.NgayHopDong,
        //                    HopDongKhamSucKhoeNhanViens = s.HopDongKhamSucKhoeNhanViens.Select(o => new
        //                    {
        //                        o.Id,
        //                        MaBN = o.BenhNhan.MaBN,
        //                        HoTen = o.HoTen,
        //                        MaNV = o.MaNhanVien,
        //                        NamSinh = o.NamSinh,
        //                        GioiTinh = o.GioiTinh,
        //                    }).ToList(),
        //                }).ToList();
        //        var yeuCauTiepNhanKhamSucKhoeData = _yeuCauTiepNhanRepository.TableNoTracking
        //                    .Where(o => o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && o.HopDongKhamSucKhoeNhanVienId != null)
        //                    .Select(o => new
        //                    {
        //                        o.Id,
        //                        MaTN = o.MaYeuCauTiepNhan,
        //                        o.ThoiDiemTiepNhan,
        //                        HopDongKhamSucKhoeNhanVienId = o.HopDongKhamSucKhoeNhanVienId,

        //                        YeuCauKhams = o.YeuCauKhamBenhs.Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && kb.GoiKhamSucKhoeId != null)
        //                            .Select(k => new HopDongKhamSucKhoeNhanVienYeuCauKhamQueryDataVo
        //                            {
        //                                YeuCauKhamBenhId = k.Id,
        //                                TrangThai = k.TrangThai,
        //                                ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
        //                                ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
        //                            }).ToList(),
        //                        YeuCauDichVuKyThuatVos = o.YeuCauDichVuKyThuats.Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && kt.GoiKhamSucKhoeId != null)
        //                            .Select(kt => new HopDongKhamSucKhoeNhanVienYeuCauDichVuKyThuatQueryDataVo
        //                            {
        //                                YeuCauDichVuKyThuatId = kt.Id,
        //                                TrangThai = kt.TrangThai,
        //                                LoaiDichVuKyThuat = kt.LoaiDichVuKyThuat,
        //                                TenDichVu = kt.TenDichVu,
        //                                ThoiDiemThucHien = kt.ThoiDiemThucHien,
        //                                CoKetQuaCDHA = kt.DataKetQuaCanLamSang != null && kt.DataKetQuaCanLamSang != "",
        //                                ThoiDiemDuyetKetQua = kt.ThoiDiemHoanThanh,
        //                            }).ToList()
        //                    }).ToList();

        //        if (trangThai == null)// Chưa có 1 dịch vụ (khám bệnh/ kỹ thuật) nào được hoàn thành/ thực hiện
        //        {
        //            var hopDongKhamSucKhoeIds = hopDongs.Select(o => o.Id).ToList();

        //            var hopDongKhamSucKhoeNhanVienChuaKhams = yeuCauTiepNhanKhamSucKhoeData
        //                .Where(o => o.YeuCauKhams.All(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham)
        //                        && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang).All(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
        //                        && o.YeuCauDichVuKyThuatVos.Where(kt => kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang).All(kt => !kt.CoKetQuaCDHA))
        //                .ToList();

        //            foreach (var hopDongKhamSucKhoeNhanVienChuaKham in hopDongKhamSucKhoeNhanVienChuaKhams)
        //            {

        //                var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
        //                {
        //                    Id = hopDongKhamSucKhoeNhanVienChuaKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
        //                    MaTN = hopDongKhamSucKhoeNhanVienChuaKham.MaTN,
        //                    //DichVuKhamBenhChuaKham = dichVuKhamBenhChuaKham,
        //                    //DichVuKhamBenhDaKham = dichVuKhamBenhDaKham,
        //                    ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienChuaKham.ThoiDiemTiepNhan
        //                };

        //                foreach (var hopDong in hopDongs)
        //                {
        //                    var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienChuaKham.HopDongKhamSucKhoeNhanVienId);
        //                    if (hopDongKhamSucKhoeNhanVien != null)
        //                    {
        //                        baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
        //                        baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
        //                        baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
        //                        baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
        //                        baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
        //                        baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
        //                        break;
        //                    }
        //                }

        //                baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
        //            }
        //        }
        //        else
        //        {
        //            var hopDongKhamSucKhoeNhanVienCoDvHoanThanhs = yeuCauTiepNhanKhamSucKhoeData
        //                .Where(o => o.YeuCauKhams.Any(kb => kb.ThoiDiemHoanThanh >= tuNgay && kb.ThoiDiemHoanThanh <= denNgay && kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
        //                        || o.YeuCauDichVuKyThuatVos
        //                                .Any(kt => ((kt.ThoiDiemDuyetKetQua != null && kt.ThoiDiemDuyetKetQua >= tuNgay && kt.ThoiDiemDuyetKetQua <= denNgay) || (kt.ThoiDiemDuyetKetQua == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay))
        //                                            && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
        //                        || o.YeuCauDichVuKyThuatVos
        //                                .Any(kt => ((kt.ThoiDiemDuyetKetQua != null && kt.ThoiDiemDuyetKetQua >= tuNgay && kt.ThoiDiemDuyetKetQua <= denNgay) || (kt.ThoiDiemDuyetKetQua == null && kt.ThoiDiemThucHien != null && kt.ThoiDiemThucHien >= tuNgay && kt.ThoiDiemThucHien <= denNgay))
        //                                            && (kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && kt.CoKetQuaCDHA))
        //                .ToList();                    

        //            if (trangThai == true) // Hoàn thành tất cả dịch vụ (khám bệnh/ kỹ thuật)
        //            {
        //                var hopDongKhamSucKhoeNhanVienDaKhams = hopDongKhamSucKhoeNhanVienCoDvHoanThanhs
        //                .Where(o => o.YeuCauKhams.All(kb => kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
        //                        && o.YeuCauDichVuKyThuatVos.All(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
        //                        && o.YeuCauDichVuKyThuatVos.All(kt => (kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && kt.CoKetQuaCDHA))
        //                .ToList();

        //                foreach (var hopDongKhamSucKhoeNhanVienDaKham in hopDongKhamSucKhoeNhanVienDaKhams)
        //                {

        //                    var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
        //                    {
        //                        Id = hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
        //                        MaTN = hopDongKhamSucKhoeNhanVienDaKham.MaTN,
        //                        //DichVuKhamBenhChuaKham = dichVuKhamBenhChuaKham,
        //                        //DichVuKhamBenhDaKham = dichVuKhamBenhDaKham,
        //                        ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienDaKham.ThoiDiemTiepNhan
        //                    };

        //                    foreach (var hopDong in hopDongs)
        //                    {
        //                        var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDaKham.HopDongKhamSucKhoeNhanVienId);
        //                        if (hopDongKhamSucKhoeNhanVien != null)
        //                        {
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
        //                            break;
        //                        }
        //                    }

        //                    baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
        //                }
        //            }
        //            else // Có ít nhất 1 dịch vụ (khám bệnh/ kỹ thuật) đã hoàn thành/ thực hiện
        //            {
        //                var hopDongKhamSucKhoeNhanVienDangKhams = hopDongKhamSucKhoeNhanVienCoDvHoanThanhs
        //                .Where(o => !(o.YeuCauKhams.All(kb => kb.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
        //                        && o.YeuCauDichVuKyThuatVos.All(kt => kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ChuanDoanHinhAnh && kt.LoaiDichVuKyThuat != LoaiDichVuKyThuat.ThamDoChucNang && kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
        //                        && o.YeuCauDichVuKyThuatVos.All(kt => (kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh || kt.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang) && kt.CoKetQuaCDHA)))
        //                .ToList();

        //                foreach (var hopDongKhamSucKhoeNhanVienDangKham in hopDongKhamSucKhoeNhanVienDangKhams)
        //                {

        //                    var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
        //                    {
        //                        Id = hopDongKhamSucKhoeNhanVienDangKham.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault(),
        //                        MaTN = hopDongKhamSucKhoeNhanVienDangKham.MaTN,
        //                        //DichVuKhamBenhChuaKham = dichVuKhamBenhChuaKham,
        //                        //DichVuKhamBenhDaKham = dichVuKhamBenhDaKham,
        //                        ThoiDiemThucHien = hopDongKhamSucKhoeNhanVienDangKham.ThoiDiemTiepNhan
        //                    };

        //                    foreach (var hopDong in hopDongs)
        //                    {
        //                        var hopDongKhamSucKhoeNhanVien = hopDong.HopDongKhamSucKhoeNhanViens.FirstOrDefault(o => o.Id == hopDongKhamSucKhoeNhanVienDangKham.HopDongKhamSucKhoeNhanVienId);
        //                        if (hopDongKhamSucKhoeNhanVien != null)
        //                        {
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.MaBN = hopDongKhamSucKhoeNhanVien.MaBN;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.HoTen = hopDongKhamSucKhoeNhanVien.HoTen;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.MaNV = hopDongKhamSucKhoeNhanVien.MaNV;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.NamSinh = hopDongKhamSucKhoeNhanVien.NamSinh;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.GioiTinh = hopDongKhamSucKhoeNhanVien.GioiTinh;
        //                            baoCaoKhamDoanHopDongTheoNhanVienVo.TenCongTyKhamSucKhoe = hopDong.TenCongTyKhamSucKhoe;
        //                            break;
        //                        }
        //                    }

        //                    baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
        //                }
        //            }
        //        }
        //    }
        //    return baoCaoKhamDoanHopDongTheoNhanVienVos;
        //}

        private IQueryable<BaoCaoKhamDoanHopDongTheoNhanVienVo> queryable(string additionalSearchString, bool? trangThai)
        {
            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new BaoCaoKhamDoanHopDongTheoNhanVienVo());
            var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongTheoNhanVienVo>(additionalSearchString);
            var hopDongKhamSucKhoeNhanVienDatas = new List<HopDongKhamSucKhoeNhanVienChiTietQueryDataVo>();
            var baoCaoKhamDoanHopDongTheoNhanVienVos = new List<BaoCaoKhamDoanHopDongTheoNhanVienVo>();
            if (queryString.HopDongKhamSucKhoeId != null)
            {
                hopDongKhamSucKhoeNhanVienDatas = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                            .Where(o => o.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId)
                            .Select(o => new HopDongKhamSucKhoeNhanVienChiTietQueryDataVo
                            {
                                HopDongKhamSucKhoeId = o.HopDongKhamSucKhoeId,
                                HopDongKhamSucKhoeNhanVienId = o.Id,
                                MaBN = o.BenhNhan.MaBN,
                                HoTen = o.HoTen,
                                MaNV = o.MaNhanVien,
                                NamSinh = o.NamSinh,
                                GioiTinh = o.GioiTinh,
                                TenCongTyKhamSucKhoe = o.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                                YeuCauTiepNhans = o.YeuCauTiepNhans.Select(tn => new HopDongKhamSucKhoeNhanVienChiTietYeuCauTiepNhanQueryDataVo
                                {
                                    YeuCauTiepNhanId = tn.Id,
                                    MaTN = tn.MaYeuCauTiepNhan,
                                    ThoiDiemTiepNhan = tn.ThoiDiemTiepNhan,
                                    YeuCauKhams = tn.YeuCauKhamBenhs.Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(k => new HopDongKhamSucKhoeNhanVienChiTietYeuCauKhamQueryDataVo
                                    {
                                        YeuCauKhamBenhId = k.Id,
                                        TrangThai = k.TrangThai,
                                        ChuyenKhoaKhamSucKhoe = k.ChuyenKhoaKhamSucKhoe,
                                        TenDichVu = k.TenDichVu,
                                        ThoiDiemHoanThanh = k.ThoiDiemHoanThanh
                                    }).ToList(),
                                    YeuCauDichVuKyThuatVos = tn.YeuCauDichVuKyThuats.Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(kt => new HopDongKhamSucKhoeNhanVienChiTietYeuCauDichVuKyThuatQueryDataVo
                                    {
                                        YeuCauDichVuKyThuatId = kt.Id,
                                        TrangThaiKyThuat = kt.TrangThai,
                                        CoKetQuaCDHA = !string.IsNullOrEmpty(kt.DataKetQuaCanLamSang),
                                        DaDuyetXetNghiem = kt.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                                        TenDichVu = kt.TenDichVu,
                                        ThoiDiemThucHien = kt.ThoiDiemThucHien
                                    }).ToList()
                                }).ToList()
                            }).ToList();

                if (trangThai == null) // Chưa có 1 dịch vụ nào đã hoàn thành/ thực hiện của nhân viên theo hợp đồng
                {

                    var hopDongKhamSucKhoeNhanVienDatalst = hopDongKhamSucKhoeNhanVienDatas.Where(z =>
                            z.YeuCauTiepNhans.Any()
                           && (z.YeuCauTiepNhans.OrderByDescending(p => p.YeuCauTiepNhanId).First().YeuCauKhams.All(v => v.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                                                                                                     || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                                                                                                                     || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                                                                                                                     || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham
                                                                                                                     )
                           && z.YeuCauTiepNhans.OrderByDescending(p => p.YeuCauTiepNhanId).First().YeuCauDichVuKyThuatVos.All(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien))
                                    ).ToList();
                    foreach (var hopDongKhamSucKhoeNhanVienData in hopDongKhamSucKhoeNhanVienDatalst)
                    {
                        var yeuCauTiepNhanVo = hopDongKhamSucKhoeNhanVienData.YeuCauTiepNhans.OrderByDescending(tn => tn.YeuCauTiepNhanId).First();
                        var dichVuKhamBenhChuaKham = string.Join(", ", yeuCauTiepNhanVo.YeuCauKhams
                                                                        .Where(z => z.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                                                                                 || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham)
                                                                        .Select(z => z.TenDichVu).Distinct()) +
                                                    string.Join(", ", yeuCauTiepNhanVo.YeuCauDichVuKyThuatVos
                                                                        .Where(z => z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                                                        .Select(z => z.TenDichVu).Distinct());
                        var dichVuKhamBenhDaKham = string.Join(", ", yeuCauTiepNhanVo.YeuCauKhams.Where(z => z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham).Select(z => z.TenDichVu).Distinct()) +
                        string.Join(", ", yeuCauTiepNhanVo.YeuCauDichVuKyThuatVos.Where(z => z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien).Select(z => z.TenDichVu).Distinct());
                        var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
                        {
                            Id = hopDongKhamSucKhoeNhanVienData.HopDongKhamSucKhoeNhanVienId,
                            MaBN = hopDongKhamSucKhoeNhanVienData.MaBN,
                            MaTN = yeuCauTiepNhanVo.MaTN,
                            HoTen = hopDongKhamSucKhoeNhanVienData.HoTen,
                            MaNV = hopDongKhamSucKhoeNhanVienData.MaNV,
                            NamSinh = hopDongKhamSucKhoeNhanVienData.NamSinh,
                            GioiTinh = hopDongKhamSucKhoeNhanVienData.GioiTinh,
                            TenCongTyKhamSucKhoe = hopDongKhamSucKhoeNhanVienData.TenCongTyKhamSucKhoe,
                            DichVuKhamBenhChuaKham = dichVuKhamBenhChuaKham,
                            DichVuKhamBenhDaKham = dichVuKhamBenhDaKham,
                            ThoiDiemThucHien = yeuCauTiepNhanVo.ThoiDiemTiepNhan
                        };
                        baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
                    }
                    query = baoCaoKhamDoanHopDongTheoNhanVienVos.AsQueryable();
                }
                else if (trangThai == true) //Có ít nhất 1 dịch vụ đã hoàn thành/ thực hiện của nhân viên trong hợp đồng
                {
                    DateTime tuNgay = new DateTime(1970, 1, 1);
                    DateTime denNgay = DateTime.Now;
                    if (!string.IsNullOrEmpty(queryString.FromDate1) || !string.IsNullOrEmpty(queryString.ToDate1))
                    {
                        queryString.FromDate1.TryParseExactCustom(out tuNgay);

                        if (!string.IsNullOrEmpty(queryString.ToDate1))
                        {
                            queryString.ToDate1.TryParseExactCustom(out denNgay);
                        }
                    }
                    var hopDongKhamSucKhoeNhanVienDataAll = new List<HopDongKhamSucKhoeNhanVienChiTietQueryDataVo>();
                    if (queryString.Loai == 1)
                    {
                        hopDongKhamSucKhoeNhanVienDataAll = hopDongKhamSucKhoeNhanVienDatas.Where(z =>
                            z.YeuCauTiepNhans.Any()
                                && (z.YeuCauTiepNhans.OrderByDescending(p => p.YeuCauTiepNhanId).First().YeuCauKhams.Any(v => v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham && tuNgay <= v.ThoiDiemHoanThanh && v.ThoiDiemHoanThanh <= denNgay)
                                || z.YeuCauTiepNhans.OrderByDescending(p => p.YeuCauTiepNhanId).First().YeuCauDichVuKyThuatVos.Any(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && tuNgay <= v.ThoiDiemThucHien && v.ThoiDiemThucHien <= denNgay))
                                ).ToList();
                    }
                    else
                    {
                        hopDongKhamSucKhoeNhanVienDataAll = hopDongKhamSucKhoeNhanVienDatas.Where(z =>
                            z.YeuCauTiepNhans.Any()
                                && (z.YeuCauTiepNhans.OrderByDescending(p => p.YeuCauTiepNhanId).First().YeuCauKhams.Any(v => v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                || z.YeuCauTiepNhans.OrderByDescending(p => p.YeuCauTiepNhanId).First().YeuCauDichVuKyThuatVos.Any(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
                                ).ToList();
                    }
                    foreach (var hopDongKhamSucKhoeNhanVienData in hopDongKhamSucKhoeNhanVienDataAll)
                    {
                        var yeuCauTiepNhanVo = hopDongKhamSucKhoeNhanVienData.YeuCauTiepNhans.OrderByDescending(tn => tn.YeuCauTiepNhanId).First();
                        var dichVuKhamBenhChuaKham = string.Join(", ", yeuCauTiepNhanVo.YeuCauKhams
                                                                       .Where(z => z.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                                                                                 || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham)
                                                                        .Select(z => z.TenDichVu).Distinct()) +
                                                    string.Join(", ", yeuCauTiepNhanVo.YeuCauDichVuKyThuatVos
                                                                        .Where(z => z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                                                        .Select(z => z.TenDichVu).Distinct());
                        var dichVuKhamBenhDaKham = string.Empty;
                        if (queryString.Loai == 1)
                        {
                            dichVuKhamBenhDaKham = string.Join(", ", yeuCauTiepNhanVo.YeuCauKhams
                                                            .Where(z => z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham && tuNgay <= z.ThoiDiemHoanThanh && z.ThoiDiemHoanThanh <= denNgay)
                                                            .Select(z => z.TenDichVu).Distinct()) +
                                                  string.Join(", ", yeuCauTiepNhanVo.YeuCauDichVuKyThuatVos
                                                          .Where(z => z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && tuNgay <= z.ThoiDiemThucHien && z.ThoiDiemThucHien <= denNgay)
                                                          .Select(z => z.TenDichVu).Distinct());
                        }
                        else
                        {
                            dichVuKhamBenhDaKham = string.Join(", ", yeuCauTiepNhanVo.YeuCauKhams
                                                            .Where(z => z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                                            .Select(z => z.TenDichVu).Distinct()) +
                                                  string.Join(", ", yeuCauTiepNhanVo.YeuCauDichVuKyThuatVos
                                                          .Where(z => z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                                          .Select(z => z.TenDichVu).Distinct());
                        }

                        var baoCaoKhamDoanHopDongTheoNhanVienVo = new BaoCaoKhamDoanHopDongTheoNhanVienVo
                        {
                            Id = hopDongKhamSucKhoeNhanVienData.HopDongKhamSucKhoeNhanVienId,
                            MaBN = hopDongKhamSucKhoeNhanVienData.MaBN,
                            MaTN = yeuCauTiepNhanVo.MaTN,
                            HoTen = hopDongKhamSucKhoeNhanVienData.HoTen,
                            MaNV = hopDongKhamSucKhoeNhanVienData.MaNV,
                            NamSinh = hopDongKhamSucKhoeNhanVienData.NamSinh,
                            GioiTinh = hopDongKhamSucKhoeNhanVienData.GioiTinh,
                            TenCongTyKhamSucKhoe = hopDongKhamSucKhoeNhanVienData.TenCongTyKhamSucKhoe,
                            DichVuKhamBenhChuaKham = dichVuKhamBenhChuaKham,
                            DichVuKhamBenhDaKham = dichVuKhamBenhDaKham,
                            ThoiDiemThucHien = yeuCauTiepNhanVo.ThoiDiemTiepNhan
                        };
                        baoCaoKhamDoanHopDongTheoNhanVienVos.Add(baoCaoKhamDoanHopDongTheoNhanVienVo);
                    }
                    query = baoCaoKhamDoanHopDongTheoNhanVienVos.AsQueryable();
                }
            }
            else
            {
                var hopDongKSKNhanVienIds = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Select(nv => nv.Id).ToList();
                DateTime tuNgay = new DateTime(1970, 1, 1);
                DateTime denNgay = DateTime.Now;
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    queryString.FromDate.TryParseExactCustom(out tuNgay);

                    if (!string.IsNullOrEmpty(queryString.ToDate))
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }
                }
                var maxValue = 20000;
                var yeuCauTiepNhanVos = new List<YeuCauTiepNhanQueryDataVo>();
                var hopDongKSKNhanVienIdsCount = hopDongKSKNhanVienIds.Count();
                var range = Math.Ceiling(hopDongKSKNhanVienIdsCount * 1.0 / maxValue);
                for (int i = 0; i < range; i++)
                {
                    var hopDongKSKNhanVienIdsTake = hopDongKSKNhanVienIds.Skip(i * maxValue).Take(maxValue).ToList(); //Giới hạn giá trị request xuống sql
                    var yeuCauTiepNhanVo = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(tn => tn.HopDongKhamSucKhoeNhanVienId != null && tn.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy
                        && hopDongKSKNhanVienIdsTake.Contains(tn.HopDongKhamSucKhoeNhanVienId.GetValueOrDefault()))
                        .Select(tn => new YeuCauTiepNhanQueryDataVo
                        {
                            YeuCauTiepNhanId = tn.Id,
                            HopDongKhamSucKhoeNhanVienId = tn.HopDongKhamSucKhoeNhanVienId,
                            BenhNhanId = tn.BenhNhanId,
                            MaTN = tn.MaYeuCauTiepNhan,
                            MaBN = tn.BenhNhan.MaBN,
                            GioiTinh = tn.GioiTinh,
                            NamSinh = tn.NamSinh,
                            HoTen = tn.HoTen,
                            MaNV = tn.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                            TenCongTyKhamSucKhoe = tn.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                            ThoiDiemTiepNhan = tn.ThoiDiemTiepNhan,
                            YeuCauKhams = tn.YeuCauKhamBenhs.Where(kb => kb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                            .Select(kb => new HopDongKhamSucKhoeNhanVienChiTietYeuCauKhamQueryDataVo
                            {
                                YeuCauKhamBenhId = kb.Id,
                                TrangThai = kb.TrangThai,
                                ChuyenKhoaKhamSucKhoe = kb.ChuyenKhoaKhamSucKhoe,
                                TenDichVu = kb.TenDichVu,
                                ThoiDiemHoanThanh = kb.ThoiDiemHoanThanh
                            }).ToList(),
                            YeuCauDichVuKyThuatVos = tn.YeuCauDichVuKyThuats.Where(kt => kt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                            .Select(kt => new HopDongKhamSucKhoeNhanVienChiTietYeuCauDichVuKyThuatQueryDataVo
                            {
                                YeuCauDichVuKyThuatId = kt.Id,
                                TrangThaiKyThuat = kt.TrangThai,
                                TenDichVu = kt.TenDichVu,
                                ThoiDiemThucHien = kt.ThoiDiemThucHien
                            }).ToList()
                        })
                        ;
                    yeuCauTiepNhanVos.AddRange(yeuCauTiepNhanVo);
                }
                if (trangThai == null)// Chưa có 1 dịch vụ (khám bệnh/ kỹ thuật) nào được hoàn thành/ thực hiện
                {
                    query = yeuCauTiepNhanVos.Where(z =>
                                 z.YeuCauKhams.All(v => (v.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham
                                                      || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan)
                                                      && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
                                                      || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                                                      || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
                                                      || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
                                                      || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
                                                      || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
                                                      || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa)
                                                      )
                                 && z.YeuCauKhams.Any(v => v.ThoiDiemHoanThanh == null || tuNgay <= v.ThoiDiemHoanThanh && v.ThoiDiemHoanThanh <= denNgay)

                                 && (z.YeuCauDichVuKyThuatVos.All(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                                    || v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                 && z.YeuCauDichVuKyThuatVos.Any(v => v.ThoiDiemThucHien == null || tuNgay <= v.ThoiDiemThucHien && v.ThoiDiemThucHien <= denNgay))
                                 ).Select(s => new BaoCaoKhamDoanHopDongTheoNhanVienVo
                                 {
                                     Id = s.HopDongKhamSucKhoeNhanVienId ?? 0,
                                     MaBN = s.MaBN,
                                     MaTN = s.MaTN,
                                     HoTen = s.HoTen,
                                     MaNV = s.MaNV,
                                     NamSinh = s.NamSinh,
                                     GioiTinh = s.GioiTinh,
                                     ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                                     TenCongTyKhamSucKhoe = s.TenCongTyKhamSucKhoe,
                                     DichVuKhamBenhChuaKham = string.Join(", ", s.YeuCauKhams.Where(z => (z.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                                                                                                     || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham
                                                                                                                     || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                                                                                                                     || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan)
                                                                                                                     && (z.ThoiDiemHoanThanh == null || tuNgay <= z.ThoiDiemHoanThanh
                                                                                                                     && z.ThoiDiemHoanThanh <= denNgay))
                                                                                                            .Select(z => z.TenDichVu).Distinct()),
                                     DichVuKyThuatChuaThucHien = string.Join(", ", s.YeuCauDichVuKyThuatVos.Where(z => (z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                     || z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                     && (z.ThoiDiemThucHien == null || tuNgay <= z.ThoiDiemThucHien && z.ThoiDiemThucHien <= denNgay)).Select(z => z.TenDichVu).Distinct()),
                                 }).AsQueryable();

                }
                else if (trangThai == true) // Hoàn thành tất cả dịch vụ (khám bệnh/ kỹ thuật)
                {
                    query = yeuCauTiepNhanVos.Where(z =>
                        (z.YeuCauKhams.All(v => (v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                            && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa)
                                            )
                                 && z.YeuCauKhams.Any(v => tuNgay <= v.ThoiDiemHoanThanh && v.ThoiDiemHoanThanh <= denNgay)
                                 && !z.YeuCauDichVuKyThuatVos.Any())

                                 || (!z.YeuCauKhams.Any()
                                     && z.YeuCauDichVuKyThuatVos.All(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                     && z.YeuCauDichVuKyThuatVos.Any(v => tuNgay <= v.ThoiDiemThucHien && v.ThoiDiemThucHien <= denNgay)
                                 )
                                 || (
                                 z.YeuCauKhams.All(v => (v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                                            && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
                                            || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa)
                                            )
                                 && z.YeuCauKhams.Any(v => tuNgay <= v.ThoiDiemHoanThanh && v.ThoiDiemHoanThanh <= denNgay)
                                 && (z.YeuCauDichVuKyThuatVos.All(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                 && z.YeuCauDichVuKyThuatVos.Any(v => tuNgay <= v.ThoiDiemThucHien && v.ThoiDiemThucHien <= denNgay))
                                 )
                                 ).Select(s => new BaoCaoKhamDoanHopDongTheoNhanVienVo
                                 {
                                     Id = s.HopDongKhamSucKhoeNhanVienId ?? 0,
                                     MaBN = s.MaBN,
                                     MaTN = s.MaTN,
                                     HoTen = s.HoTen,
                                     MaNV = s.MaNV,
                                     NamSinh = s.NamSinh,
                                     GioiTinh = s.GioiTinh,
                                     ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                                     TenCongTyKhamSucKhoe = s.TenCongTyKhamSucKhoe,
                                     DichVuKhamBenhDaKham = string.Join(", ", s.YeuCauKhams.Where(z => z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham && tuNgay <= z.ThoiDiemHoanThanh && z.ThoiDiemHoanThanh <= denNgay).Select(z => z.TenDichVu).Distinct()),
                                     DichVuKyThuatDaThucHien = string.Join(", ", s.YeuCauDichVuKyThuatVos.Where(z => z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && tuNgay <= z.ThoiDiemThucHien && z.ThoiDiemThucHien <= denNgay).Select(z => z.TenDichVu).Distinct())
                                 }).AsQueryable();
                }
                else // Có ít nhất 1 dịch vụ (khám bệnh/ kỹ thuật) đã hoàn thành/ thực hiện
                {
                    query = yeuCauTiepNhanVos.Where(z =>
                            //dvkb
                            z.YeuCauKhams.Any(v => (v.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham
                            || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                            || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
                           && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa
                           ))
                           && z.YeuCauKhams.Any(v => tuNgay <= v.ThoiDiemHoanThanh && v.ThoiDiemHoanThanh <= denNgay)
                           && z.YeuCauKhams.Any(v => v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                           && !z.YeuCauKhams.All(v => v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                           && z.YeuCauDichVuKyThuatVos.Any(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                             || v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                           //dvkt
                           || (z.YeuCauDichVuKyThuatVos.Any(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                             || v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien
                                                             || v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                           && z.YeuCauDichVuKyThuatVos.Any(v => tuNgay <= v.ThoiDiemThucHien && v.ThoiDiemThucHien <= denNgay)
                           && z.YeuCauDichVuKyThuatVos.Any(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                           && !z.YeuCauDichVuKyThuatVos.All(v => v.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                           && z.YeuCauKhams.Any(v => (v.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham
                            || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                            || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh || v.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
                           && (v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu
                           || v.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.SanPhuKhoa
                           ))
                           )
                                 ).Select(s => new BaoCaoKhamDoanHopDongTheoNhanVienVo
                                 {
                                     Id = s.HopDongKhamSucKhoeNhanVienId ?? 0,
                                     MaBN = s.MaBN,
                                     MaTN = s.MaTN,
                                     HoTen = s.HoTen,
                                     MaNV = s.MaNV,
                                     NamSinh = s.NamSinh,
                                     GioiTinh = s.GioiTinh,
                                     ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                                     TenCongTyKhamSucKhoe = s.TenCongTyKhamSucKhoe,
                                     DichVuKhamBenhChuaKham = string.Join(", ", s.YeuCauKhams.Where(z => (z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh || z.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                                        && (z.ThoiDiemHoanThanh == null || tuNgay <= z.ThoiDiemHoanThanh && z.ThoiDiemHoanThanh <= denNgay)).Select(z => z.TenDichVu).Distinct()),
                                     DichVuKyThuatChuaThucHien = string.Join(", ", s.YeuCauDichVuKyThuatVos.Where(z => (z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                     || z.TrangThaiKyThuat == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                           && (z.ThoiDiemThucHien == null || tuNgay <= z.ThoiDiemThucHien && z.ThoiDiemThucHien <= denNgay)).Select(z => z.TenDichVu).Distinct())
                                 }).AsQueryable();
                }
            }
            return query;
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncTheoNhanVien(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<BaoCaoKhamDoanHopDongTheoNhanVienVo>().ToArray(), TotalRowCount = 0 };
            }
            var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongTheoNhanVienVo>(queryInfo.AdditionalSearchString);
            IQueryable<BaoCaoKhamDoanHopDongTheoNhanVienVo> queryChuaKham = null;
            IQueryable<BaoCaoKhamDoanHopDongTheoNhanVienVo> queryDangKham = null;
            IQueryable<BaoCaoKhamDoanHopDongTheoNhanVienVo> queryDaKham = null;

            if (queryString.ChuaKhamNhanVien == true)
            {
                queryChuaKham = queryable(queryInfo.AdditionalSearchString, null);
            }
            if (queryString.DangKhamNhanVien == true)
            {
                queryDangKham = queryable(queryInfo.AdditionalSearchString, false);
            }
            if (queryString.DaKhamNhanVien == true)
            {
                queryDaKham = queryable(queryInfo.AdditionalSearchString, true);
            }
            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new BaoCaoKhamDoanHopDongTheoNhanVienVo());
            if (queryChuaKham != null)
            {
                query = query.Concat(queryChuaKham);
            }
            if (queryDangKham != null)
            {
                query = query.Concat(queryDangKham);
            }
            if (queryDaKham != null)
            {
                query = query.Concat(queryDaKham);
            }
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetDataForGridAsyncDichVuNgoai(QueryInfo queryInfo, bool exportExcel)
        {
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<BaoCaoKhamDoanHopDongDichVuNgoaiVo>().ToArray(), TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var khamDoanHopDongDichVuNgoaiVos = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(z => z.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && z.GoiKhamSucKhoeId == null && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                 .Select(s => new BaoCaoKhamDoanHopDongDichVuNgoaiVo
                 {
                     Id = s.Id,
                     YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                     MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                     MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                     HoTen = s.YeuCauTiepNhan.HoTen,
                     MaNV = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                     NamSinh = s.YeuCauTiepNhan.NamSinh,
                     GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                     ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
                     TenDichVu = s.TenDichVu,
                     SoLan = 1,
                     DonGiaBV = s.Gia,
                     HopDongKhamSucKhoeId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                     CongTyKhamSucKhoeId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                     TenCongTyKhamSucKhoe = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                     LaDichVuKhamBenh = true,
                     CreateOn = s.CreatedOn,
                     TongSoTienCongNo = 0,
                 }).Union(
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(z => z.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && z.GoiKhamSucKhoeId == null && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                     .Select(s => new BaoCaoKhamDoanHopDongDichVuNgoaiVo
                     {
                         Id = s.Id,
                         YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                         MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                         MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                         HoTen = s.YeuCauTiepNhan.HoTen,
                         MaNV = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                         NamSinh = s.YeuCauTiepNhan.NamSinh,
                         GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                         ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
                         TenDichVu = s.TenDichVu,
                         SoLan = s.SoLan,
                         DonGiaBV = s.GiaBenhVienTaiThoiDiemChiDinh != null ? s.GiaBenhVienTaiThoiDiemChiDinh : s.Gia,
                         DonGiaMoi = s.Gia,
                         SoTienMienGiam = s.SoTienMienGiam,
                         SoTienCongNo = s.SoTienBaoHiemTuNhanChiTra,
                         HopDongKhamSucKhoeId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                         CongTyKhamSucKhoeId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                         TenCongTyKhamSucKhoe = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                         LaDichVuKhamBenh = false,
                         CreateOn = s.CreatedOn,
                         TongSoTienCongNo = 0
                     }
                ));

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongDichVuNgoaiVo>(queryInfo.AdditionalSearchString);
                if (queryString.CongTyKhamSucKhoeId != null)
                {
                    khamDoanHopDongDichVuNgoaiVos = khamDoanHopDongDichVuNgoaiVos.Where(p => p.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId);
                }
                if (queryString.HopDongKhamSucKhoeId != null)
                {
                    khamDoanHopDongDichVuNgoaiVos = khamDoanHopDongDichVuNgoaiVos.Where(p => p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId);
                }

                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    khamDoanHopDongDichVuNgoaiVos = khamDoanHopDongDichVuNgoaiVos.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
            }
            var listResult = await khamDoanHopDongDichVuNgoaiVos.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            var yeuCauTiepNhanIds = listResult.Select(z => z.YeuCauTiepNhanId).Distinct().ToList();
            if (yeuCauTiepNhanIds.Any())
            {
                var taiKhoanBenhNhanThus = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(tk => tk.DaHuy != true && yeuCauTiepNhanIds.Contains(tk.YeuCauTiepNhanId)).ToList();
                var taiKhoanBenhNhanThuIds = taiKhoanBenhNhanThus.Select(tk => tk.Id).ToList();

                var phieuTraNos = _taiKhoanBenhNhanThuRepository.TableNoTracking
                                    .Where(tk => tk.DaHuy != true && taiKhoanBenhNhanThuIds.Contains(tk.ThuNoPhieuThuId.GetValueOrDefault()))
                                    .Select(s => new
                                    {
                                        s.YeuCauTiepNhanId,
                                        SoTienTraNo = s.TienMat.GetValueOrDefault() + s.ChuyenKhoan.GetValueOrDefault() + s.POS.GetValueOrDefault()
                                    }).ToList();

                foreach (var yeuCauTiepNhanId in yeuCauTiepNhanIds)
                {
                    var sumCongNo = taiKhoanBenhNhanThus.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Sum(z => z.CongNo.GetValueOrDefault());
                    var sumSoTienTraNo = phieuTraNos.Where(pn => pn.YeuCauTiepNhanId == yeuCauTiepNhanId).Sum(pn => pn.SoTienTraNo);
                    var baoCaoKhamDoanHopDongDichVuNgoai = listResult.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).OrderBy(z => z.CreateOn).FirstOrDefault();
                    if (baoCaoKhamDoanHopDongDichVuNgoai != null)
                    {
                        baoCaoKhamDoanHopDongDichVuNgoai.TongSoTienCongNo = sumCongNo - sumSoTienTraNo;
                    }

                    baoCaoKhamDoanHopDongDichVuNgoai.TongThanhTienThucThuTheoBenhNhan = listResult.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Sum(z => z.SoTienThucThu);
                }
            }
            return new GridDataSource { Data = listResult, TotalRowCount = listResult.Count() };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDichVuNgoai(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<BaoCaoKhamDoanHopDongDichVuNgoaiVo>().ToArray(), TotalRowCount = 0 };
            }
            var khamDoanHopDongDichVuNgoaiVos = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(z => z.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && z.GoiKhamSucKhoeId == null && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                 .Select(s => new BaoCaoKhamDoanHopDongDichVuNgoaiVo
                 {
                     Id = s.Id,
                     MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                     MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                     HoTen = s.YeuCauTiepNhan.HoTen,
                     MaNV = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                     NamSinh = s.YeuCauTiepNhan.NamSinh,
                     GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                     ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
                     TenDichVu = s.TenDichVu,
                     SoLan = 1,
                     DonGiaBV = s.Gia,
                     HopDongKhamSucKhoeId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                     CongTyKhamSucKhoeId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                     LaDichVuKhamBenh = true
                 }).Union(
                _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(z => z.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && z.GoiKhamSucKhoeId == null && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                     .Select(s => new BaoCaoKhamDoanHopDongDichVuNgoaiVo
                     {
                         Id = s.Id,
                         MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                         MaTN = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                         HoTen = s.YeuCauTiepNhan.HoTen,
                         MaNV = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                         NamSinh = s.YeuCauTiepNhan.NamSinh,
                         GioiTinh = s.YeuCauTiepNhan.GioiTinh,
                         ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
                         TenDichVu = s.TenDichVu,
                         SoLan = s.SoLan,
                         DonGiaBV = s.GiaBenhVienTaiThoiDiemChiDinh != null ? s.GiaBenhVienTaiThoiDiemChiDinh : s.Gia,
                         DonGiaMoi = s.Gia,
                         SoTienMienGiam = s.SoTienMienGiam,
                         SoTienCongNo = s.SoTienBaoHiemTuNhanChiTra,
                         HopDongKhamSucKhoeId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                         CongTyKhamSucKhoeId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                         LaDichVuKhamBenh = false
                     }
                ));
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongDichVuNgoaiVo>(queryInfo.AdditionalSearchString);
                if (queryString.CongTyKhamSucKhoeId != null)
                {
                    khamDoanHopDongDichVuNgoaiVos = khamDoanHopDongDichVuNgoaiVos.Where(p => p.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId);
                }
                if (queryString.HopDongKhamSucKhoeId != null)
                {
                    khamDoanHopDongDichVuNgoaiVos = khamDoanHopDongDichVuNgoaiVos.Where(p => p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId);
                }

                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    khamDoanHopDongDichVuNgoaiVos = khamDoanHopDongDichVuNgoaiVos.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
            }
            var countTask = khamDoanHopDongDichVuNgoaiVos.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public virtual byte[] ExportBaoCaoHoatDongDichVuNgoai(QueryInfo queryInfo, List<BaoCaoKhamDoanHopDongDichVuNgoaiVo> datas)
        {
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH DỊCH VỤ NGOÀI HỢP ĐỒNG");

                    // set row
                    //worksheet.DefaultRowHeight = 16;                    

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 10;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 20;
                    worksheet.DefaultColWidth = 7;

                    // SET title head cho bảng excel
                    var congTyKhamSucKhoe = new CongTyKhamSucKhoe();
                    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
                    {
                        var queryString = JsonConvert.DeserializeObject<BaoCaoKhamDoanHopDongDichVuNgoaiVo>(queryInfo.AdditionalSearchString);
                        congTyKhamSucKhoe = _congTyKhamSucKhoeRepository.TableNoTracking.Where(z => z.Id == queryString.CongTyKhamSucKhoeId).FirstOrDefault();
                    }

                    using (var range = worksheet.Cells["A2:M2"])
                    {
                        range.Worksheet.Cells["A2:M2"].Merge = true;
                        range.Worksheet.Cells["A2:M2"].Value = "DANH SÁCH DỊCH VỤ NGOÀI HỢP ĐỒNG";
                        range.Worksheet.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:M2"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A2:M2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:M2"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["A3:M3"])
                    {
                        range.Worksheet.Cells["A3:M3"].Merge = true;
                        range.Worksheet.Cells["A3:M3"].Value = congTyKhamSucKhoe != null ? congTyKhamSucKhoe.Ten : string.Empty;
                        range.Worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A3:M3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:M3"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A4"])
                    {
                        range.Worksheet.Cells["A4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A4"].Value = "STT";
                        range.Worksheet.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["B4"])
                    {
                        range.Worksheet.Cells["B4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B4"].Value = "Tên công ty";
                        range.Worksheet.Cells["B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["C4"])
                    {
                        range.Worksheet.Cells["C4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C4"].Value = "Mã nhân viên";
                        range.Worksheet.Cells["C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["C4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["D4"])
                    {
                        range.Worksheet.Cells["D4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D4"].Value = "Mã người bệnh";
                        range.Worksheet.Cells["D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["E4"])
                    {
                        range.Worksheet.Cells["E4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E4"].Value = "Mã tiếp nhận";
                        range.Worksheet.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["E4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["F4"])
                    {
                        range.Worksheet.Cells["F4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F4"].Value = "Họ tên";
                        range.Worksheet.Cells["F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["F4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["G4"])
                    {
                        range.Worksheet.Cells["G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G4"].Value = "Giới tính";
                        range.Worksheet.Cells["G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["G4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["G4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["H4"])
                    {
                        range.Worksheet.Cells["H4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H4"].Value = "Năm sinh";
                        range.Worksheet.Cells["H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["H4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["H4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["H4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["I4"])
                    {
                        range.Worksheet.Cells["I4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I4"].Value = "Dịch vụ";
                        range.Worksheet.Cells["I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["I4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["I4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["I4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["J4"])
                    {
                        range.Worksheet.Cells["J4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J4"].Value = "Đơn giá BV";
                        range.Worksheet.Cells["J4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["J4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["J4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["J4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["J4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["K4"])
                    {
                        range.Worksheet.Cells["K4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K4"].Value = "Đơn giá mới";
                        range.Worksheet.Cells["K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["K4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["K4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["K4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["L4"])
                    {
                        range.Worksheet.Cells["L4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L4"].Value = "Số tiền được miễn giảm";
                        range.Worksheet.Cells["L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["L4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["L4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["M4"])
                    {
                        range.Worksheet.Cells["M4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M4"].Value = "Số tiền thực thu";
                        range.Worksheet.Cells["M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["M4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["M4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["M4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["N4"])
                    {
                        range.Worksheet.Cells["N4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N4"].Value = "Công nợ";
                        range.Worksheet.Cells["N4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["N4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["N4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["N4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["N4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["O4"])
                    {
                        range.Worksheet.Cells["O4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O4"].Value = "Tổng cộng";
                        range.Worksheet.Cells["O4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["O4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["O4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["O4"].Style.Font.Bold = true;
                    }

                    var index = 5; // bắt đầu đổ data từ dòng 5
                    var indexMerge = 5;
                    var indexMergeEnd = 5;
                    var STT = 1;
                    var listYeuCauTiepNhanId = datas.Select(z => z.YeuCauTiepNhanId).Distinct().ToList();
                    bool isShowCompanyName = true;

                    foreach (var yeuCauTiepNhanId in listYeuCauTiepNhanId)
                    {
                        foreach (var chitiet in datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId))
                        {
                            using (var range = worksheet.Cells["A" + index + ":O" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":O" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["A" + index + ":O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                range.Worksheet.Cells["A" + index].Value = STT;
                                range.Worksheet.Cells["B" + index].Value = isShowCompanyName ? chitiet.TenCongTyKhamSucKhoe : string.Empty;
                                range.Worksheet.Cells["C" + index].Value = chitiet.MaNV;
                                range.Worksheet.Cells["D" + index].Value = chitiet.MaBN;
                                range.Worksheet.Cells["E" + index].Value = chitiet.MaTN;
                                range.Worksheet.Cells["F" + index].Value = chitiet.HoTen;
                                range.Worksheet.Cells["G" + index].Value = chitiet.GioiTinhDisplay;
                                range.Worksheet.Cells["H" + index].Value = chitiet.NamSinh;
                                range.Worksheet.Cells["I" + index].Value = chitiet.TenDichVu;

                                range.Worksheet.Cells["J" + index].Value = chitiet.DonGiaBV;
                                range.Worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["K" + index].Value = chitiet.DonGiaMoi;
                                range.Worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["L" + index].Value = chitiet.SoTienMienGiam;
                                range.Worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";

                                range.Worksheet.Cells["M" + index].Value = chitiet.ThanhTienThucThu;
                                range.Worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            }
                            STT++;
                            index++;
                            isShowCompanyName = false;
                        }
                        indexMergeEnd = index - 1;


                        using (var range = worksheet.Cells["N" + indexMerge + ":N" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["N" + indexMerge + ":N" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Sum(z => z.TongSoTienCongNo);
                            range.Worksheet.Cells["N" + indexMerge + ":N" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["N" + indexMerge + ":N" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["N" + indexMerge + ":N" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["N" + indexMerge + ":N" + indexMergeEnd].Style.Numberformat.Format = "#,##0.00";
                            range.Worksheet.Cells["N" + indexMerge + ":N" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["N" + indexMerge + ":N" + indexMergeEnd].Merge = true;
                        }

                        using (var range = worksheet.Cells["O" + indexMerge + ":O" + indexMergeEnd])
                        {
                            range.Worksheet.Cells["O" + indexMerge + ":O" + indexMergeEnd].Value = datas.Where(z => z.YeuCauTiepNhanId == yeuCauTiepNhanId).Sum(z => z.TongThanhTienThucThuTheoBenhNhan);
                            range.Worksheet.Cells["O" + indexMerge + ":O" + indexMergeEnd].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["O" + indexMerge + ":O" + indexMergeEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["O" + indexMerge + ":O" + indexMergeEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            range.Worksheet.Cells["O" + indexMerge + ":O" + indexMergeEnd].Style.Numberformat.Format = "#,##0.00";
                            range.Worksheet.Cells["O" + indexMerge + ":O" + indexMergeEnd].Style.WrapText = true;
                            range.Worksheet.Cells["O" + indexMerge + ":O" + indexMergeEnd].Merge = true;
                        }

                        indexMerge = index;
                    }

                    #region row tổng cộng                 

                    worksheet.Cells["A" + index + ":O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":O" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A" + index + ":O" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":O" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":O" + index].Style.Font.Bold = true;

                    worksheet.Cells["A" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["A" + index + ":I" + index].Merge = true;
                    worksheet.Cells["A" + index + ":I" + index].Value = "Tổng tiền:";

                    worksheet.Cells["J" + index + ":J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index + ":J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Value = datas.Sum(x => x.DonGiaBV);

                    worksheet.Cells["K" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index + ":K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Value = datas.Sum(x => x.DonGiaMoi);

                    worksheet.Cells["L" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index + ":L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Value = datas.Sum(x => x.SoTienMienGiam);

                    worksheet.Cells["M" + index + ":M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index + ":M" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["M" + index].Value = datas.Sum(x => x.SoTienThucThu);

                    worksheet.Cells["N" + index + ":N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["N" + index + ":N" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["N" + index].Value = datas.Sum(x => x.SoTienCongNo);

                    worksheet.Cells["O" + index + ":O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["O" + index + ":O" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["O" + index].Value = datas.Sum(x => x.TongThanhTienThucThuTheoBenhNhan);
                    #endregion

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }


        public async Task<List<LookupItemVo>> GetLoaiNhanVienHoacHopDong(DropDownListRequestModel queryInfo)
        {
            var result = new List<LookupItemVo>();
            var item1 = new LookupItemVo
            {
                KeyId = 1,
                DisplayName = "Hợp đồng"
            };
            var item2 = new LookupItemVo
            {
                KeyId = 2,
                DisplayName = "Nhân viên"
            };
            result.Add(item1);
            result.Add(item2);
            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                result = result.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return result.ToList();
        }

        public string InDanhSachNhanVien(InDanhSachNhanVien inDanhSachNhanVien)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("InCaoBaoNhanVienKhamDoanTrongGoi")).First();
            var STT = 1;
            var datas = string.Empty;
            foreach (var item in inDanhSachNhanVien.Datas)
            {
                datas = datas
                                    + "<tr style = 'border: 1px solid #020000;text-align: center;'>"
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.MaNV
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.MaBN
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.MaTN
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HoTen
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.NamSinh
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.GioiTinhDisplay
                                    + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.ThoiDiemThucHien?.ApplyFormatDateTime()
                                    + "</tr>";
                STT++;
            }
            var data = new DataNhanVienChiTiet
            {
                TrangThai = inDanhSachNhanVien.TrangThai,
                LogoUrl = inDanhSachNhanVien.HostingName + "/assets/img/logo-bacha-full.png",
                NhanViens = datas
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public async Task<GridDataSource> GetDataForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            if (!string.IsNullOrEmpty(queryInfo.FromDateString))
            {
                queryInfo.FromDateString.TryParseExactCustom(out DateTime tuNgayTemp);
                queryInfo.FromDate = tuNgayTemp;
            }
            if (!string.IsNullOrEmpty(queryInfo.ToDateString))
            {
                queryInfo.ToDateString.TryParseExactCustom(out DateTime denNgayTemp);
                if (denNgayTemp == null)
                {
                    denNgayTemp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                }
                queryInfo.ToDate = denNgayTemp;
            }
            else
            {
                DateTime denNgay = DateTime.Now;
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                queryInfo.ToDate = denNgay;
            }
            var query = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.GoiKhamSucKhoeId != null && o.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham &&
                            o.NoiThucHienId != null
                            && (
                                 (o.ThoiDiemThucHien != null && (queryInfo.FromDate == null || o.ThoiDiemThucHien >= queryInfo.FromDate) && o.ThoiDiemThucHien < queryInfo.ToDate)
                                 || (o.ThoiDiemThucHien == null && o.ThoiDiemHoanThanh != null && (queryInfo.FromDate == null || o.ThoiDiemHoanThanh >= queryInfo.FromDate) && o.ThoiDiemHoanThanh < queryInfo.ToDate)
                               // (o.ThoiDiemHoanThanh != null && (queryInfo.FromDate == null || o.ThoiDiemHoanThanh >= queryInfo.FromDate) && o.ThoiDiemHoanThanh < queryInfo.ToDate)
                               //|| (o.ThoiDiemHoanThanh == null && o.ThoiDiemThucHien != null && (queryInfo.FromDate == null || o.ThoiDiemThucHien >= queryInfo.FromDate) && o.ThoiDiemThucHien < queryInfo.ToDate)
                               )
                            //&& o.ThoiDiemThucHien < queryInfo.ToDate
                            );
            //if (queryInfo.FromDate != null)
            //{
            //    query = query.Where(z => z.ThoiDiemThucHien >= queryInfo.FromDate);
            //}
            if (queryInfo.CongTyKhamSucKhoeId.GetValueOrDefault() != 0)
            {
                query = queryInfo.HopDongKhamSucKhoeId.GetValueOrDefault() != 0
                    ? query.Where(o => o.GoiKhamSucKhoe.HopDongKhamSucKhoeId == queryInfo.HopDongKhamSucKhoeId)
                    : query.Where(o => o.GoiKhamSucKhoe.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryInfo.CongTyKhamSucKhoeId);
            }

            var queryData = query.Select(o => new NguoiBenhKhamDichVuTheoPhongQueryData
            {
                Id = o.Id,
                HopDongKhamSucKhoeId = o.GoiKhamSucKhoe.HopDongKhamSucKhoeId,
                TenHopDongKhamSucKhoe = o.GoiKhamSucKhoe.HopDongKhamSucKhoe.SoHopDong,
                CongTyKhamSucKhoeId = o.GoiKhamSucKhoe.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                TenCongTyKhamSucKhoe = o.GoiKhamSucKhoe.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                NoiThucHienId = o.NoiThucHienId.Value,
                TenNoiThucHien = o.NoiThucHien.Ten
            });
            var noiThucHienCuaNguoiBenhs = queryData.GroupBy(o => o.NoiThucHienId).Select(o =>
                new NoiThucHienCuaNguoiBenh()
                {
                    NoiThucHienId = o.Key,
                    TenNoiThucHien = o.First().TenNoiThucHien
                }).ToList();

            var dataSource = new List<NguoiBenhKhamDichVuTheoPhong>();
            foreach (var groupHopDongKhamSucKhoe in queryData.GroupBy(o => o.HopDongKhamSucKhoeId))
            {
                var nguoiBenhKhamDichVuTheoPhong = new NguoiBenhKhamDichVuTheoPhong()
                {
                    Id = groupHopDongKhamSucKhoe.Key,
                    CongTyKhamSucKhoeId = groupHopDongKhamSucKhoe.First().CongTyKhamSucKhoeId,
                    HopDongKhamSucKhoeId = groupHopDongKhamSucKhoe.Key,
                    TenCongTyKhamSucKhoe = groupHopDongKhamSucKhoe.First().TenCongTyKhamSucKhoe,
                    TenHopDongKhamSucKhoe = groupHopDongKhamSucKhoe.First().TenHopDongKhamSucKhoe
                };
                foreach (var noiThucHien in noiThucHienCuaNguoiBenhs)
                {
                    nguoiBenhKhamDichVuTheoPhong.NoiThucHienCuaNguoiBenhs.Add(new NoiThucHienCuaNguoiBenh
                    {
                        NoiThucHienId = noiThucHien.NoiThucHienId,
                        TenNoiThucHien = noiThucHien.TenNoiThucHien,
                        SoLan = groupHopDongKhamSucKhoe.Count(o => o.NoiThucHienId == noiThucHien.NoiThucHienId)
                    });
                }
                dataSource.Add(nguoiBenhKhamDichVuTheoPhong);
            }

            return new GridDataSource { Data = dataSource.ToArray(), TotalRowCount = dataSource.Count };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo)
        {
            return null;
        }

        public virtual byte[] ExportBaoCaoHoatDongKhamDoan(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo, List<NguoiBenhKhamDichVuTheoPhong> nguoiBenhKhamDichVuTheoPhongs)
        {
            if (!string.IsNullOrEmpty(queryInfo.FromDateString))
            {
                queryInfo.FromDateString.TryParseExactCustom(out DateTime tuNgayTemp);
                queryInfo.FromDate = tuNgayTemp;
            }
            if (!string.IsNullOrEmpty(queryInfo.ToDateString))
            {
                queryInfo.ToDateString.TryParseExactCustom(out DateTime denNgayTemp);
                if (denNgayTemp == null)
                {
                    denNgayTemp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                }
                queryInfo.ToDate = denNgayTemp;
            }
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO HOẠT ĐỘNG KHÁM ĐOÀN");
                    var noiThucHienCuaNguoiBenhs = new List<NoiThucHienCuaNguoiBenh>();
                    foreach (var item in nguoiBenhKhamDichVuTheoPhongs)
                    {
                        foreach (var nth in item.NoiThucHienCuaNguoiBenhs)
                        {
                            var noiThucHienCuaNguoiBenh = new NoiThucHienCuaNguoiBenh
                            {
                                NoiThucHienId = nth.NoiThucHienId,
                                TenNoiThucHien = nth.TenNoiThucHien,
                                SoLan = nth.SoLan
                            };
                            noiThucHienCuaNguoiBenhs.Add(noiThucHienCuaNguoiBenh);
                        }
                    }
                    var noiThucHienCuaNguoiBenhHeaders = noiThucHienCuaNguoiBenhs.GroupBy(g => new
                    {
                        g.NoiThucHienId,
                        g.TenNoiThucHien
                    }).Select(g => g.First()).OrderBy(z => z.NoiThucHienId).ThenBy(z => z.TenNoiThucHien).ToList();
                    var tongDichVus = noiThucHienCuaNguoiBenhHeaders.Count();
                    var danhSachKyTuXuLy = new List<string>();
                    if (tongDichVus > 26)
                    {
                        danhSachKyTuXuLy = XuLyDanhSachKyTu(tongDichVus, 26);
                    }
                    else
                    {
                        danhSachKyTuXuLy = KyTus();
                    }
                    worksheet.Row(danhSachKyTuXuLy.Count()).Height = 24.5;
                    for (int i = 1; i <= tongDichVus; i++)
                    {
                        worksheet.Column(i).Width = 20;
                        worksheet.Column(i + 5).Width = 20;
                        worksheet.Column(i + 6).Width = 20;
                    }


                    var index = 3; // bắt đầu từ A3(dòng 3)
                    using (var range = worksheet.Cells["A" + index])
                    {
                        range.Worksheet.Cells["A" + index].Value = "STT";
                        range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["B" + index])
                    {
                        range.Worksheet.Cells["B" + index].Value = "Tên công ty";
                        range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B" + index].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["C" + index])
                    {
                        range.Worksheet.Cells["C" + index].Value = "Số hợp đồng";
                        range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C" + index].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["D" + index])
                    {
                        range.Worksheet.Cells["D" + index].Value = "Tổng số";
                        range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D" + index].Style.Font.Bold = true;
                    }
                    var noiThucHienCuaNguoiBenhHeaderClones = new List<NoiThucHienCuaNguoiBenh>(); // Dùng để tính tổng ngang số lần từng phòng
                                                                                                   // Hiển thị các dịch vụ theo column
                    var indexKyTu = 4;
                    var kyTuCuoiCung = "E";
                    foreach (var dv in noiThucHienCuaNguoiBenhHeaders)
                    {
                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[i] + index])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Value = dv.TenNoiThucHien;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.Font.Bold = true;
                                range.Worksheet.Cells[danhSachKyTuXuLy[i] + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            }
                            kyTuCuoiCung = danhSachKyTuXuLy[indexKyTu];
                            break;
                        }
                        indexKyTu++;
                        var soLan = noiThucHienCuaNguoiBenhs.Where(z => z.NoiThucHienId == dv.NoiThucHienId).Sum(z => z.SoLan) ?? 0;
                        var noiThucHienCuaNguoiBenhHeaderCloneNew = new NoiThucHienCuaNguoiBenh
                        {
                            NoiThucHienId = dv.NoiThucHienId,
                            TenNoiThucHien = dv.TenNoiThucHien,
                            SoLan = soLan
                        };
                        noiThucHienCuaNguoiBenhHeaderClones.Add(noiThucHienCuaNguoiBenhHeaderCloneNew);
                    }

                    using (var range = worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"])
                    {
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Merge = true;
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Value = "BÁO CÁO HOẠT ĐỘNG KHÁM ĐOÀN";
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1" + ":" + kyTuCuoiCung + "1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"])
                    {
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Merge = true;
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Value = "Từ ngày: " + queryInfo.FromDate?.ApplyFormatGioPhutNgay(true)
                                                          + " đến " + queryInfo.ToDate?.ApplyFormatGioPhutNgay(true);
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2" + ":" + kyTuCuoiCung + "2"].Style.Font.Color.SetColor(Color.Black);
                    }

                    index++;
                    var STT = 1;
                    foreach (var data in nguoiBenhKhamDichVuTheoPhongs)
                    {
                        indexKyTu = 0;// gán lại để đổ data
                        using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                        {
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = STT;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexKyTu++;
                        using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                        {
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = data.TenCongTyKhamSucKhoe;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexKyTu++;
                        using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                        {
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = data.TenHopDongKhamSucKhoe;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexKyTu++;

                        using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                        {
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = data.TongSo;
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexKyTu++;
                        foreach (var item in data.NoiThucHienCuaNguoiBenhs.OrderBy(z => z.NoiThucHienId).ThenBy(z => z.TenNoiThucHien))
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = item.SoLan;
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexKyTu++;
                        }
                        STT++;
                        index++;
                    }
                    //Tổng cộng
                    using (var range = worksheet.Cells["A" + index + ":C" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":C" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":C" + index].Value = "TỔNG CỘNG: ";
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A" + index + ":C" + index].Style.Font.Bold = true;

                    }
                    indexKyTu = 3; // gán lại cho tổng số
                    using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                    {
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = nguoiBenhKhamDichVuTheoPhongs.Sum(z => z.TongSo);
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Bold = true;

                    }
                    indexKyTu++;

                    foreach (var item in noiThucHienCuaNguoiBenhHeaderClones.OrderBy(z => z.NoiThucHienId).ThenBy(z => z.TenNoiThucHien))
                    {
                        for (int i = indexKyTu; i < danhSachKyTuXuLy.Count();)
                        {
                            using (var range = worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index])
                            {
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Value = item.SoLan;
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells[danhSachKyTuXuLy[indexKyTu] + index].Style.Font.Bold = true;
                            }
                            break;
                        }
                        indexKyTu++;
                    }
                    index = index + 2;
                    var indexChuKy = index;
                    using (var range = worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy])
                    {
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Value = DateTime.Now.ApplyFormatNgayThangNam();
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                    }

                    indexChuKy++;
                    index++;
                    using (var range = worksheet.Cells["B" + indexChuKy])
                    {
                        range.Worksheet.Cells["B" + index + ":B" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["B" + indexChuKy].Value = "NGƯỜI LẬP PHIẾU";
                        range.Worksheet.Cells["B" + indexChuKy].Style.WrapText = true;
                        range.Worksheet.Cells["B" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["D" + indexChuKy])
                    {
                        range.Worksheet.Cells["D" + index + ":D" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["D" + indexChuKy].Value = "TRƯỞNG PHÒNG KTTH";
                        range.Worksheet.Cells["D" + indexChuKy].Style.WrapText = true;
                        range.Worksheet.Cells["D" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.Bold = true;

                    }

                    using (var range = worksheet.Cells["F" + index + ":G" + indexChuKy])
                    {
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Value = "GIÁM ĐỐC";
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.Bold = true;
                    }

                    indexChuKy++;
                    using (var range = worksheet.Cells["B" + indexChuKy])
                    {
                        range.Worksheet.Cells["B" + indexChuKy].Value = "(Ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["B" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D" + indexChuKy])
                    {
                        range.Worksheet.Cells["D" + indexChuKy].Value = "(Ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["D" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["F" + index + ":G" + indexChuKy])
                    {
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Merge = true;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Value = "(Ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F" + indexChuKy + ":G" + indexChuKy].Style.Font.Color.SetColor(Color.Black);
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        private List<string> XuLyDanhSachKyTu(int soDichVu, int tongSoKyTu)
        {
            var danhSachKyTuSauKhiXuKy = new List<string>();
            var danhSachKyTuPre = KyTus();
            var danhSachKyTuResult = KyTus();
            var soLanLap = soDichVu / tongSoKyTu;
            danhSachKyTuPre.Take(soLanLap);
            for (int i = 0; i < soLanLap; i++)
            {
                foreach (var result in danhSachKyTuPre)
                {
                    foreach (var item in danhSachKyTuResult)
                    {
                        danhSachKyTuSauKhiXuKy.Add(result + item);
                    }
                }
            }
            danhSachKyTuResult.AddRange(danhSachKyTuResult);
            return danhSachKyTuResult;
        }

        private List<string> KyTus()
        {
            var kyTus = new List<string>()
                    { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };// 26 ký tự
            return kyTus;
        }

        #region BÁO CÁO TỔNG HỢP KẾT QUẢ KHÁM SỨC KHỎE

        public virtual byte[] ExportBaoCaoTongHopKetQuaKSK(List<BaoCaoTongHopKetQuaKhamDoanGridVo> list, List<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo> listTheoNhanVien, string tenCongTy, string soHopdong)
        {
            var datas = (ICollection<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo>)listTheoNhanVien;
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoXuatNhapTonGridVo>("STT", p => ind++)
            };
            list = list.GroupBy(d => d.TenDichVu)
                .Select(vc => new BaoCaoTongHopKetQuaKhamDoanGridVo()
                {
                    IdDichVuTrongGois = vc.Select(d => d.IdDichVuTrongGoi).ToList(),
                    TenDichVu = vc.First().TenDichVu,
                    NhomId = vc.First().NhomId,
                    KetQua = vc.First().KetQua,
                    ListDichVuIds =vc.First().ListDichVuIds
                }).OrderBy(o => o.NhomId).ToList();

            var listTheoNhanViens = new List<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo>();
            foreach (var iu in listTheoNhanVien)
            {
                iu.BaoCaoTongHopKetQuaKhamDoanGridVos = iu.BaoCaoTongHopKetQuaKhamDoanGridVos.GroupBy(d => d.TenDichVu)
                .Select(vc => new BaoCaoTongHopKetQuaKhamDoanGridVo()
                {
                    IdDichVuTrongGoi = vc.First().IdDichVuTrongGoi,
                    TenDichVu = vc.First().TenDichVu,
                    NhomId = vc.First().NhomId,
                    KetQua = vc.First().KetQua,
                    Id = vc.First().Id,
                    ListDichVuIds = vc.First().ListDichVuIds
                }).ToList();
                listTheoNhanViens.Add(iu);
            }

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO TỔNG HỢP KẾT QUẢ KHÁM SỨC KHỎE");

                    // set row
                    //worksheet.DefaultRowHeight = 16;                    

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 25;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 10;
                    worksheet.Column(8).Width = 10;

                    if (list.Any())
                    {
                        var soLuongDv = list.Count();
                        var cotHienTai = 8;
                        foreach (var item in list)
                        {
                            cotHienTai = cotHienTai + 1;
                            worksheet.Column(cotHienTai).Width = 15;
                        }
                        // phân loại ket luan de nghi
                        for (int i = 0; i <= 3; i++)
                        {
                            cotHienTai = cotHienTai + 1;
                            worksheet.Column(cotHienTai).Width = 15;
                        }
                    }
                    else
                    {
                        worksheet.Column(9).Width = 15;
                        worksheet.Column(10).Width = 15;
                        worksheet.Column(11).Width = 15;

                        worksheet.DefaultColWidth = 15;
                    }

                    using (var range = worksheet.Cells["A1:J1"])
                    {
                        range.Worksheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:J1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:J1"].Style.Font.Bold = true;

                        range.Worksheet.Cells["A1:A1"].Value = "STT";

                        range.Worksheet.Cells["B1:B1"].Value = "Mã NB";

                        range.Worksheet.Cells["C1:C1"].Value = "Mã TN";

                        range.Worksheet.Cells["D1:D1"].Value = "Họ tên";

                        range.Worksheet.Cells["E1:E1"].Value = "Thời gian chuyên khoa đầu tiên";

                        range.Worksheet.Cells["F1:F1"].Value = "Năm sinh";

                        range.Worksheet.Cells["G1:G1"].Value = "Giới tính";

                        range.Worksheet.Cells["H1:H1"].Value = "Chiều cao (mét)";

                        range.Worksheet.Cells["I1:I1"].Value = "Cân nặng (kg)";

                        range.Worksheet.Cells["J1:J1"].Value = "Huyết áp (mmhg)";

                    }
                    // Hiển thị các dịch vụ theo column
                    var tatCaDichVus = list.ToList();

                    var danhSachKyTuXuLy = new List<string>();
                    var kyTuCuoiCung = string.Empty;
                    var index = 1; // bắt đầu từ F3(dòng 3)
                    if (list.Any())
                    {
                        var indexColumnDichVu = 11;
                        foreach (var dv in list)
                        {
                            var columnName = ExcelHelper.ColumnIndexToColumnLetter(indexColumnDichVu);
                            using (var range = worksheet.Cells[columnName + index + ":" + columnName + index])
                            {
                                range.Worksheet.Cells[columnName + index + ":" + columnName + index].Value = dv.TenDichVu;
                                range.Worksheet.Cells[columnName + index + ":" + columnName + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells[columnName + index + ":" + columnName + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells[columnName + index + ":" + columnName + index].Style.Font.Bold = true;
                                //range.Worksheet.Cells[columnName + index + ":" + columnName + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }
                            indexColumnDichVu++;
                        }

                        var columnName1 = ExcelHelper.ColumnIndexToColumnLetter(indexColumnDichVu);
                        using (var range = worksheet.Cells[columnName1 + index + ":" + columnName1 + index])
                        {
                            range.Worksheet.Cells[columnName1 + index + ":" + columnName1 + index].Value = "Phân loại";
                            range.Worksheet.Cells[columnName1 + index + ":" + columnName1 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[columnName1 + index + ":" + columnName1 + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[columnName1 + index + ":" + columnName1 + index].Style.Font.Bold = true;
                            //range.Worksheet.Cells[columnName1 + index + ":" + columnName1 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexColumnDichVu++;

                        var columnName2 = ExcelHelper.ColumnIndexToColumnLetter(indexColumnDichVu);
                        using (var range = worksheet.Cells[columnName2 + index + ":" + columnName2 + index])
                        {
                            range.Worksheet.Cells[columnName2 + index + ":" + columnName2 + index].Value = "Kết luận";
                            range.Worksheet.Cells[columnName2 + index + ":" + columnName2 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[columnName2 + index + ":" + columnName2 + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[columnName2 + index + ":" + columnName2 + index].Style.Font.Bold = true;
                            //range.Worksheet.Cells[columnName2 + index + ":" + columnName2 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexColumnDichVu++;

                        var columnName3 = ExcelHelper.ColumnIndexToColumnLetter(indexColumnDichVu);
                        using (var range = worksheet.Cells[columnName3 + index + ":" + columnName3 + index])
                        {
                            range.Worksheet.Cells[columnName3 + index + ":" + columnName3 + index].Value = "Đề nghị";
                            range.Worksheet.Cells[columnName3 + index + ":" + columnName3 + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells[columnName3 + index + ":" + columnName3 + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells[columnName3 + index + ":" + columnName3 + index].Style.Font.Bold = true;
                            //range.Worksheet.Cells[columnName3 + index + ":" + columnName3 + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        indexColumnDichVu++;

                        // đổ data
                        int stt = 1;
                        int indexData = 2;
                        var indexBind = 0; //
                        int viTriKyTu = 0;
                        if (listTheoNhanVien.Any())
                        {
                            foreach (var itemNguoiBenh in listTheoNhanVien.OrderBy(d => d.STT).ToList())
                            {
                                viTriKyTu = 0;
                                worksheet.Cells["A" + indexData + ":" + "A" + indexData].Value = itemNguoiBenh.STT;

                                worksheet.Cells["A" + indexData + ":" + "A" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["A" + indexData + ":" + "A" + indexData].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells["A" + indexData + ":" + "A" + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["B" + indexData + ":" + "B" + indexData].Value = itemNguoiBenh.MaNB;
                                worksheet.Cells["B" + indexData + ":" + "B" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["B" + indexData + ":" + "B" + indexData].Style.Font.Color.SetColor(Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["C" + indexData + ":" + "C" + indexData].Value = itemNguoiBenh.MaTN;
                                worksheet.Cells["C" + indexData + ":" + "C" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["C" + indexData + ":" + "C" + indexData].Style.Font.Color.SetColor(Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["D" + indexData + ":" + "D" + indexData].Value = itemNguoiBenh.HoTen;
                                worksheet.Cells["D" + indexData + ":" + "D" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["D" + indexData + ":" + "D" + indexData].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells["B" + indexData + ":" + "B" + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["E" + indexData + ":" + "E" + indexData].Value = itemNguoiBenh.ThoiGianChuyenKhoaDauTienFormat;
                                worksheet.Cells["E" + indexData + ":" + "E" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["E" + indexData + ":" + "E" + indexData].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells["C" + indexData + ":" + "C" + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["F" + indexData + ":" + "F" + indexData].Value = itemNguoiBenh.NamSinh;
                                worksheet.Cells["F" + indexData + ":" + "F" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["F" + indexData + ":" + "F" + indexData].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells["D" + indexData + ":" + "D" + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["G" + indexData + ":" + "G" + indexData].Value = itemNguoiBenh.GioiTinh;
                                worksheet.Cells["G" + indexData + ":" + "G" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["G" + indexData + ":" + "G" + indexData].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells["E" + indexData + ":" + "E" + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["H" + indexData + ":" + "H" + indexData].Value = itemNguoiBenh.ChieuCao;
                                worksheet.Cells["H" + indexData + ":" + "H" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["H" + indexData + ":" + "H" + indexData].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells["F" + indexData + ":" + "F" + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["I" + indexData + ":" + "I" + indexData].Value = itemNguoiBenh.CanNang;
                                worksheet.Cells["I" + indexData + ":" + "I" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["I" + indexData + ":" + "I" + indexData].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells["G" + indexData + ":" + "G" + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                viTriKyTu++;
                                worksheet.Cells["J" + indexData + ":" + "J" + indexData].Value = itemNguoiBenh.HuyetAp;
                                worksheet.Cells["J" + indexData + ":" + "J" + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells["J" + indexData + ":" + "J" + indexData].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells["H" + indexData + ":" + "H" + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                viTriKyTu++;

                                var indexColumnDichVu1 = 11;
                                foreach (var dv in list)
                                {
                                    var columnName = ExcelHelper.ColumnIndexToColumnLetter(indexColumnDichVu1);
                                    using (var range = worksheet.Cells[columnName + indexData + ":" + columnName + indexData])
                                    {
                                        var dichVu = itemNguoiBenh.BaoCaoTongHopKetQuaKhamDoanGridVos.Where(d => d.NhomId == dv.NhomId && dv.ListDichVuIds.Contains(d.IdDichVuTrongGoi));
                                        if (dichVu.Any())
                                        {
                                            //var ketQua = itemNguoiBenh.BaoCaoTongHopKetQuaKhamDoanGridVos.Where(d => d.NhomId == dv.NhomId && d.TenDichVu.ToLower() == dv.TenDichVu.ToLower().Trim()).Select(d => d.KetQua).LastOrDefault();
                                            var ketQua = itemNguoiBenh.BaoCaoTongHopKetQuaKhamDoanGridVos.Where(d => d.NhomId == dv.NhomId && dv.ListDichVuIds.Contains(d.IdDichVuTrongGoi)).Select(d => d.KetQua).LastOrDefault();
                                            if (!string.IsNullOrEmpty(ketQua))
                                            {
                                                ketQua = CommonHelper.StripHTML(Regex.Replace(ketQua, "</p>(?![\n\r]+)", Environment.NewLine));
                                                if (ketQua.Length > 2 && ketQua.Substring(ketQua.Length - 2) == "\r\n")
                                                {
                                                    ketQua = ketQua.Remove(ketQua.Length - 2);
                                                }
                                            }
                                            range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Value = ketQua;
                                            range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Style.Font.Color.SetColor(Color.Black);
                                            //range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                            range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Style.WrapText = true;

                                        }
                                        else
                                        {
                                            range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Value = "";
                                            range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                            range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Style.Font.Color.SetColor(Color.Black);
                                            //range.Worksheet.Cells[columnName + indexData + ":" + columnName + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        }
                                    }
                                    indexColumnDichVu1++;
                                }

                                var columnName4 = ExcelHelper.ColumnIndexToColumnLetter(indexColumnDichVu1);
                                using (var range = worksheet.Cells[columnName4 + indexData + ":" + columnName4 + indexData])
                                {
                                    range.Worksheet.Cells[columnName4 + indexData + ":" + columnName4 + indexData].Value = itemNguoiBenh.PhanLoai;
                                    range.Worksheet.Cells[columnName4 + indexData + ":" + columnName4 + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells[columnName4 + indexData + ":" + columnName4 + indexData].Style.Font.Color.SetColor(Color.Black);
                                    //range.Worksheet.Cells[columnName4 + indexData + ":" + columnName4 + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells[columnName4 + indexData + ":" + columnName4 + indexData].Style.WrapText = true;
                                }
                                indexColumnDichVu1++;

                                var columnName5 = ExcelHelper.ColumnIndexToColumnLetter(indexColumnDichVu1);
                                using (var range = worksheet.Cells[columnName5 + indexData + ":" + columnName5 + indexData])
                                {
                                    range.Worksheet.Cells[columnName5 + indexData + ":" + columnName5 + indexData].Value = itemNguoiBenh.KetLuan;
                                    range.Worksheet.Cells[columnName5 + indexData + ":" + columnName5 + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells[columnName5 + indexData + ":" + columnName5 + indexData].Style.Font.Color.SetColor(Color.Black);
                                    //range.Worksheet.Cells[columnName5 + indexData + ":" + columnName5 + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells[columnName5 + indexData + ":" + columnName5 + indexData].Style.WrapText = true;
                                }
                                indexColumnDichVu1++;

                                var columnName6 = ExcelHelper.ColumnIndexToColumnLetter(indexColumnDichVu1);
                                using (var range = worksheet.Cells[columnName6 + indexData + ":" + columnName6 + indexData])
                                {
                                    range.Worksheet.Cells[columnName6 + indexData + ":" + columnName6 + indexData].Value = itemNguoiBenh.DeNghi;
                                    range.Worksheet.Cells[columnName6 + indexData + ":" + columnName6 + indexData].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells[columnName6 + indexData + ":" + columnName6 + indexData].Style.Font.Color.SetColor(Color.Black);
                                    //range.Worksheet.Cells[columnName6 + indexData + ":" + columnName6 + indexData].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells[columnName5 + indexData + ":" + columnName5 + indexData].Style.WrapText = true;
                                }
                                indexColumnDichVu1++;

                                indexData++;
                                stt++;
                            }
                        }


                        xlPackage.Save();
                    }

                }

                return stream.ToArray();
            }

        }

        public async Task<List<BaoCaoTongHopKetQuaKhamDoanGridVo>> ListDichVu(ModelVoNhanVien model)
        {
            var hopDong = _hopDongKhamSucKhoeRepository.GetById(model.HopDongId, o =>
            o.Include(x => x.GoiKhamSucKhoes).ThenInclude(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats)
            .Include(x => x.GoiKhamSucKhoes).ThenInclude(x => x.GoiKhamSucKhoeDichVuKhamBenhs)
             //.Include(x => x.GoiKhamSucKhoes).ThenInclude(x => x.YeuCauDichVuKyThuats)
             //.Include(x => x.GoiKhamSucKhoes).ThenInclude(x => x.YeuCauKhamBenhs)
            );

            var inFoKhams = hopDong.GoiKhamSucKhoes.Select(d => new TongHopKetQuaKhamDoanGridVo
            {
                GoiKhamSucKhoeDichVuDichVuKyThuats = d.GoiKhamSucKhoeDichVuDichVuKyThuats.ToList(),
                GoiKhamSucKhoeDichVuKhamBenhs = d.GoiKhamSucKhoeDichVuKhamBenhs.ToList()
            }).ToList();

            var listGoiKhamIds = hopDong.GoiKhamSucKhoes.Select(d => d.Id).ToList();

            var layTatCaYeuCauDVKTTheoGoiKhamIds = _yeuCauDichVuKyThuatRepository.TableNoTracking
                  .Where(d => listGoiKhamIds.Contains((long)d.GoiKhamSucKhoeId))
                    .Select(g => new InFoYeuCauDichVuKyThuat
                    {
                        Id = g.Id,
                        DichVuKyThuatBenhVienId = g.DichVuKyThuatBenhVienId,
                    }).ToList();


            var layTatCaYeuCauKhamTheoGoiKhamIds = _yeuCauKhamBenhRepository.TableNoTracking
                       .Where(d => listGoiKhamIds.Contains((long)d.GoiKhamSucKhoeId))
                       .Select(g => new InFoYeuCauKham
                       {
                           Id = g.Id,
                           DichVuKhamBenhVienId = g.DichVuKhamBenhBenhVienId,
                       }).ToList();

            // get info dịch vu khám và dịch vụ kỹ thuật bv
            var dichVuKhamBenhIds = layTatCaYeuCauKhamTheoGoiKhamIds.Select(d => d.DichVuKhamBenhVienId).ToList();

            var infoDichVuKhamBenhs = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                 .Where(d => dichVuKhamBenhIds.Contains(d.Id))
                   .Select(g => new InFoDichVuKhamBenh
                   {
                       Id = g.Id,
                       Ten = g.Ten,
                       MaDichVu = g.Ma
                   }).ToList();

            var dichVuKyThuatIds = layTatCaYeuCauDVKTTheoGoiKhamIds.Select(d => d.DichVuKyThuatBenhVienId).ToList();

            var infoDichVuKyThuats = _dichVuKyThuatBenhVienRepository.TableNoTracking
                       .Where(d => dichVuKyThuatIds.Contains(d.Id))
                       .Select(g => new InFoDichVuKyThuat
                       {
                           Id = g.Id,
                           Ten = g.Ten,
                           MaDichVu = g.Ma
                       }).ToList();


            var dv = new List<BaoCaoTongHopKetQuaKhamDoanGridVo>();

            foreach (var item in inFoKhams)
            {
                if (item.GoiKhamSucKhoeDichVuDichVuKyThuats.Count() != 0)
                {

                    var infoDVKT = item.GoiKhamSucKhoeDichVuDichVuKyThuats.Where(d=> dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId))

                          .Select(d => new BaoCaoTongHopKetQuaKhamDoanGridVo
                          {
                              //TenDichVu = d.DichVuKyThuatBenhVien.Ten,
                              IdDichVuTrongGoi = d.DichVuKyThuatBenhVienId,
                              NhomId = EnumNhomGoiDichVu.DichVuKyThuat,
                              //MaDichVu = d.DichVuKyThuatBenhVien.Ma,
                          }).ToList();

                    foreach (var dvkt in infoDVKT)
                    {
                        // IdDichVuTrongGoi -> Dịch vụ kỹ thuật bệnh viện
                        if (infoDichVuKyThuats.Where(d=>d.Id  == dvkt.IdDichVuTrongGoi).Count() > 0)
                        {
                            dvkt.TenDichVu = infoDichVuKyThuats.Where(d => d.Id == dvkt.IdDichVuTrongGoi).Select(d => d.Ten).First();
                            dvkt.MaDichVu = infoDichVuKyThuats.Where(d => d.Id == dvkt.IdDichVuTrongGoi).Select(d => d.MaDichVu).First();
                        }
                    }

              

                    if (infoDVKT.Count() != 0)
                    {

                        foreach (var dvv in layTatCaYeuCauDVKTTheoGoiKhamIds.ToList())
                        {
                            foreach (var itemDichVu in infoDVKT)
                            {
                                if (itemDichVu.IdDichVuTrongGoi == dvv.DichVuKyThuatBenhVienId)
                                {
                                    itemDichVu.ListDichVuIds.Add(dvv.Id);
                                }
                            }
                        }
                    }
                    dv.AddRange(infoDVKT);
                }


                if (item.GoiKhamSucKhoeDichVuKhamBenhs.Count() != 0)
                {

                    var infoDVK = item.GoiKhamSucKhoeDichVuKhamBenhs.Where(d=> dichVuKhamBenhIds.Contains(d.DichVuKhamBenhBenhVienId))

                          .Select(d => new BaoCaoTongHopKetQuaKhamDoanGridVo
                          {
                              //TenDichVu = d.DichVuKhamBenhBenhVien.Ten,
                              IdDichVuTrongGoi = d.DichVuKhamBenhBenhVienId,
                              NhomId = EnumNhomGoiDichVu.DichVuKhamBenh,
                              //MaDichVu = d.DichVuKhamBenhBenhVien.Ma,
                          }).ToList();


                    foreach (var dvkt in infoDVK)
                    {
                        // IdDichVuTrongGoi -> Dịch vụ Khám bệnh viện Id
                        if (infoDichVuKhamBenhs.Where(d => d.Id == dvkt.IdDichVuTrongGoi).Count() > 0)
                        {
                            dvkt.TenDichVu = infoDichVuKhamBenhs.Where(d => d.Id == dvkt.IdDichVuTrongGoi).Select(d => d.Ten).First();
                            dvkt.MaDichVu = infoDichVuKhamBenhs.Where(d => d.Id == dvkt.IdDichVuTrongGoi).Select(d => d.MaDichVu).First();
                        }
                    }

                    if (infoDVK.Count() != 0)
                    {

                        foreach (var itemDichVu in infoDVK)
                        {
                            switch (itemDichVu.IdDichVuTrongGoi)
                            {
                                case 1: //Khám Nội
                                    var id1s = layTatCaYeuCauKhamTheoGoiKhamIds.Where(d => d.DichVuKhamBenhVienId == itemDichVu.IdDichVuTrongGoi).Select(d => d.Id).ToList();
                                    itemDichVu.ListDichVuIds.AddRange(id1s);
                                    break;
                                case 2: //Khám Ngoại

                                    var id2s = layTatCaYeuCauKhamTheoGoiKhamIds.Where(d => d.DichVuKhamBenhVienId == itemDichVu.IdDichVuTrongGoi).Select(d => d.Id).ToList();
                                    itemDichVu.ListDichVuIds.AddRange(id2s);
                                    break;
                                case 3: // Khám Phụ Sản

                                    var id3s = layTatCaYeuCauKhamTheoGoiKhamIds.Where(d => d.DichVuKhamBenhVienId == itemDichVu.IdDichVuTrongGoi).Select(d => d.Id).ToList();
                                    itemDichVu.ListDichVuIds.AddRange(id3s);
                                    break;
                                case 4: //Khám Nhi

                                    var id4s = layTatCaYeuCauKhamTheoGoiKhamIds.Where(d => d.DichVuKhamBenhVienId == itemDichVu.IdDichVuTrongGoi).Select(d => d.Id).ToList();
                                    itemDichVu.ListDichVuIds.AddRange(id4s);
                                    break;
                                case 5: //Khám Mắt

                                    var id5s = layTatCaYeuCauKhamTheoGoiKhamIds.Where(d => d.DichVuKhamBenhVienId == itemDichVu.IdDichVuTrongGoi).Select(d => d.Id).ToList();
                                    itemDichVu.ListDichVuIds.AddRange(id5s);
                                    break;
                                case 6: //Khám Răng Hàm Mặt

                                    var id6s = layTatCaYeuCauKhamTheoGoiKhamIds.Where(d => d.DichVuKhamBenhVienId == itemDichVu.IdDichVuTrongGoi).Select(d => d.Id).ToList();
                                    itemDichVu.ListDichVuIds.AddRange(id6s);
                                    break;
                                case 7: //Khám Tai Mũi Họng

                                    var id7s = layTatCaYeuCauKhamTheoGoiKhamIds.Where(d => d.DichVuKhamBenhVienId == itemDichVu.IdDichVuTrongGoi).Select(d => d.Id).ToList();
                                    itemDichVu.ListDichVuIds.AddRange(id7s);
                                    break;
                                case 8: //Khám Da liễu

                                    var id8s = layTatCaYeuCauKhamTheoGoiKhamIds.Where(d => d.DichVuKhamBenhVienId == itemDichVu.IdDichVuTrongGoi).Select(d => d.Id).ToList();
                                    itemDichVu.ListDichVuIds.AddRange(id8s);
                                    break;
                                default:

                                    //do a different thing
                                    break;
                            }
                        }







                    }
                    dv.AddRange(infoDVK);
                }

            }





            return dv.GroupBy(s => new { s.TenDichVu })
                .Select(d => new BaoCaoTongHopKetQuaKhamDoanGridVo()
                {
                    TenDichVu = d.First().TenDichVu,
                    IdDichVuTrongGoi = d.First().IdDichVuTrongGoi,
                    NhomId = d.First().NhomId,
                    MaDichVu = d.First().MaDichVu,
                    ListDichVuIds = d.First().ListDichVuIds
                }).ToList();
        }
        public async Task<List<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo>> ListDichVuNhanVien(ModelVoNhanVien model)
        {
            #region Id tiep nhan va Hop dong nhan vien da tao
            var infoTiepNhans = _yeuCauTiepNhanRepository.TableNoTracking
                                        .OrderByDescending(x => x.Id)
                                        .Where(x => x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == model.HopDongId)
                                        .Select(o => new { o.Id, o.HopDongKhamSucKhoeNhanVienId}).ToList();

            var hopDongNhanVienDataoIds = infoTiepNhans.Select(d => d.HopDongKhamSucKhoeNhanVienId).ToList();

            var yeuCauTiepNhanIds = infoTiepNhans.Select(d => d.Id).ToList();

            var hopDongKhamSucKhoeNhanVienDaTaos = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                                             .Where(o => hopDongNhanVienDataoIds.Contains(o.Id))
                                             .Select(s => new BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo
                                             {
                                                 Id = s.Id,
                                                 STT = s.STTNhanVien
                                             })
                                             .ToList();

            #endregion
            #region HopDong nhan vien chua tao tiep nhan
            var hopDongKhamSucKhoeNhanVienChuaTaos = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                                             .Where(o => o.HopDongKhamSucKhoeId == model.HopDongId && !hopDongNhanVienDataoIds.Contains(o.Id))
                                             .Select(s => new BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo
                                             {
                                                 Id = s.Id,
                                                 HoTen = s.HoTen,
                                                 GioiTinh = s.GioiTinh != null ? s.GioiTinh.GetDescription() : "",
                                                 NamSinh = s.NamSinh,
                                                 MaNB = "",
                                                 MaTN = "",
                                                 BaoCaoTongHopKetQuaKhamDoanGridVos = new List<BaoCaoTongHopKetQuaKhamDoanGridVo>(),
                                                 STT = s.STTNhanVien
                                             })
                                             .ToList();

            var hopDongNhanVienChuaTaoIds = hopDongKhamSucKhoeNhanVienChuaTaos.Select(d => d.Id).ToList();
            #endregion


            var query = _yeuCauTiepNhanRepository.TableNoTracking
                                   .Where(p => yeuCauTiepNhanIds.Contains(p.Id)
                         ).Select(s => new BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo
                         {
                             Id = s.Id,
                             HoTen = s.HoTen,
                             GioiTinh = s.GioiTinh != null ? s.GioiTinh.GetDescription() : "",
                             NamSinh = s.NamSinh,
                             KetQuaKhamSucKhoeData = s.KetQuaKhamSucKhoeData,
                             KSKKetLuanData = s.KSKKetLuanData,
                             HopDongKhamSucKhoeNhanVienId = s.HopDongKhamSucKhoeNhanVienId,
                             // BVHD-3676
                             MaNB = s.BenhNhan.MaBN,
                             MaTN = s.MaYeuCauTiepNhan,
                             KSKKetLuanPhanLoaiSucKhoe = s.KSKKetLuanPhanLoaiSucKhoe,
                             KSKKetLuanCacBenhTat = s.KSKKetLuanCacBenhTat,
                             KSKKetLuanGhiChu = s.KSKKetLuanGhiChu,
                             
                         }).ToList();

            


            var chiSos = _chiSoSinhHieuRepository.TableNoTracking.Where(d => yeuCauTiepNhanIds.Contains(d.YeuCauTiepNhanId))
                       .Select(d => new ChiSoNguoibenh
                       {
                           Id = d.YeuCauTiepNhanId,
                           ChieuCao = d.ChieuCao,
                           CanNang = d.CanNang,
                           HuyetApTamTruong = d.HuyetApTamTruong,
                           HuyetApTamThu = d.HuyetApTamThu
                       }).ToList();



            var thongTinThoiGianKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking.Where(d => yeuCauTiepNhanIds.Contains(d.YeuCauTiepNhanId))
                .Where(d => d.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && d.GoiKhamSucKhoeId != null && d.ThoiDiemThucHien != null).Select(d => new ThongTinThoiGianKhamBenh
                {
                    Id = d.YeuCauTiepNhanId,
                    ThoiDiemThucHien = d.ThoiDiemThucHien
                }).ToList();

            var result = new List<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo>();

            foreach (var item in query)
            {
                item.STT = hopDongKhamSucKhoeNhanVienDaTaos.Where(d => d.Id == item.HopDongKhamSucKhoeNhanVienId).Select(d=>d.STT).FirstOrDefault();

                item.ChiSos = chiSos.Where(d => d.Id == item.Id).ToList();

                item.ThongTinThoiGianKhamBenhs = thongTinThoiGianKhamBenhs.Where(d => d.Id == item.Id).ToList();

            }

            if (query != null && query.Count() > 0)
            {
                var queryHopDong = !(model.ToDate != null || model.FromDate != null) ? _iKhamDoanService.GetDataKetQuaKSKDoanEditByHopDongNew(model.HopDongId, yeuCauTiepNhanIds) : new List<DanhSachDichVuKhamGrid>();
                var queryKetLuan = _iKhamDoanService.GetGridPhanLoaiVaCacBenhtatDenghiByHopDong(model.HopDongId);
                var queryketQua = model.ToDate != null || model.FromDate != null ? GetDataKetQuaKSKDoanEditFillterThoiDiemThucHienByHopDongNew(model.HopDongId, yeuCauTiepNhanIds) : new List<DanhSachDichVuKhamGrid>();


                foreach (var item in query)
                {
                    if (model.ToDate != null || model.FromDate != null)
                    {
                        var ketQua = queryketQua.Where(o => o.HopDongKhamSucKhoeNhanVienId == (long)item.HopDongKhamSucKhoeNhanVienId).ToList();
                        var resultketQuaTheoNguoiBenhs = new List<BaoCaoTongHopKetQuaKhamDoanGridVo>();
                        if (ketQua.Any())
                        {
                            var filterThoiDiemThucHien = ketQua.Where(d => (model.ToDate == null || d.ThoiDiemThucHien >= model.ToDate) && (model.FromDate == null || d.ThoiDiemThucHien <= model.FromDate));
                            foreach (var dvkq in filterThoiDiemThucHien.ToList())
                            {
                                var resultketQuaTheoNguoiBenhObj = new BaoCaoTongHopKetQuaKhamDoanGridVo();
                                resultketQuaTheoNguoiBenhObj.IdDichVuTrongGoi = dvkq.Id;
                                resultketQuaTheoNguoiBenhObj.TenDichVu = dvkq.TenDichVu;
                                resultketQuaTheoNguoiBenhObj.NhomId = dvkq.NhomId;
                                resultketQuaTheoNguoiBenhObj.KetQua = dvkq.KetQuaDichVu;
                                resultketQuaTheoNguoiBenhs.Add(resultketQuaTheoNguoiBenhObj);
                            }

                        }
                        item.BaoCaoTongHopKetQuaKhamDoanGridVos = resultketQuaTheoNguoiBenhs;
                    }
                    else
                    {
                        var ketQua = queryHopDong.Where(o => o.HopDongKhamSucKhoeNhanVienId == (long)item.HopDongKhamSucKhoeNhanVienId).ToList();
                        var resultketQuaTheoNguoiBenhs = new List<BaoCaoTongHopKetQuaKhamDoanGridVo>();
                        if (ketQua.Any())
                        {
                            foreach (var dvkq in ketQua.ToList())
                            {
                                var resultketQuaTheoNguoiBenhObj = new BaoCaoTongHopKetQuaKhamDoanGridVo();
                                resultketQuaTheoNguoiBenhObj.IdDichVuTrongGoi = dvkq.Id;
                                resultketQuaTheoNguoiBenhObj.TenDichVu = dvkq.TenDichVu;
                                resultketQuaTheoNguoiBenhObj.NhomId = dvkq.NhomId;
                                resultketQuaTheoNguoiBenhObj.KetQua = dvkq.KetQuaDichVu;
                                resultketQuaTheoNguoiBenhs.Add(resultketQuaTheoNguoiBenhObj);
                            }

                        }
                        item.BaoCaoTongHopKetQuaKhamDoanGridVos = resultketQuaTheoNguoiBenhs;
                    }

                    if (!string.IsNullOrEmpty(item.KSKKetLuanData))
                    {
                        var jsonKetLuan = queryKetLuan.Where(o => o.HopDongKhamSucKhoeNhanVienId == (long)item.HopDongKhamSucKhoeNhanVienId).ToList();
                        if (jsonKetLuan.Any())
                        {
                            foreach (var dvkq in jsonKetLuan.ToList())
                            {
                                if (dvkq.LoaiKetLuan == EnumTypeLoaiKetLuan.PhanLoai)
                                {
                                    item.PhanLoai = dvkq.KetQua;
                                }
                                if (dvkq.LoaiKetLuan == EnumTypeLoaiKetLuan.CacBenhTatNeuCo)
                                {
                                    item.KetLuan = dvkq.KetQua;
                                }
                                if (dvkq.LoaiKetLuan == EnumTypeLoaiKetLuan.DeNghi)
                                {
                                    item.DeNghi = dvkq.KetQua;
                                }
                            }

                        }
                    }
                    else
                    {
                        #region phân loại
                        item.PhanLoai = item.KSKKetLuanPhanLoaiSucKhoe;
                        #endregion
                        #region các bệnh tật khác
                        item.KetLuan = item.KSKKetLuanCacBenhTat;
                        #endregion
                        #region dê nghi
                        item.DeNghi = item.KSKKetLuanGhiChu;
                        #endregion
                    }
                    if (item.ChiSos.Any())
                    {
                        item.ChieuCao = item.ChiSos.Where(k => k.ChieuCao != null).Select(p => p.ChieuCao).Any() ? item.ChiSos.Select(p => p.ChieuCao).FirstOrDefault().ToString() : "";
                        item.CanNang = item.ChiSos.Where(k => k.CanNang != null).Select(p => p.CanNang).Any() ? item.ChiSos.Select(p => p.CanNang).FirstOrDefault().ToString() : "";
                        var huyetApTamThu = item.ChiSos.Where(k => k.HuyetApTamThu != null).Select(p => p.HuyetApTamThu).Any() ? item.ChiSos.Select(p => p.HuyetApTamThu).FirstOrDefault().ToString() : "";
                        var huyetApTamTruong = item.ChiSos.Where(k => k.HuyetApTamTruong != null).Select(p => p.HuyetApTamTruong).Any() ? item.ChiSos.Select(p => p.HuyetApTamTruong).FirstOrDefault().ToString() : "";
                        item.HuyetAp = huyetApTamThu + "/" + huyetApTamTruong;
                    }
                    #region thời gian chuyên khoa đầu tiên -> lấy theo thời điểm thực hiện 
                    item.ThoiGianChuyenKhoaDauTien = item.ThongTinThoiGianKhamBenhs.Any() ? item.ThongTinThoiGianKhamBenhs.OrderBy(o => o.ThoiDiemThucHien).FirstOrDefault().ThoiDiemThucHien : null;
                    #endregion
                    result.Add(item);
                }
            }
            // hợp đồng nhân viên chưa bắt đầu khám -> k có yêu cầu tiếp nhận
            result.AddRange(hopDongKhamSucKhoeNhanVienChuaTaos.Select(s => new BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo
            {
                Id = s.Id,
                HoTen = s.HoTen,
                GioiTinh = s.GioiTinh,
                NamSinh = s.NamSinh,
                MaNB = "",
                MaTN = "",
                BaoCaoTongHopKetQuaKhamDoanGridVos = new List<BaoCaoTongHopKetQuaKhamDoanGridVo>(),
                STT = s.STT
            }));

            return result.OrderBy(d => d.STT).ToList();
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
                    .OrderByDescending(x => x.CongTyKhamSucKhoeId == congTyId).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstHopDongKhamSucKhoeId = await _hopDongKhamSucKhoeRepository
                    .ApplyFulltext(queryInfo.Query, nameof(HopDongKhamSucKhoe), lstColumnNameSearch)
                    .Where(x => x.CongTyKhamSucKhoeId == congTyId
                          )
                    .Select(x => x.Id)
                    .ToListAsync();
                lstHopDongKhamSucKhoes = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .Where(x => lstHopDongKhamSucKhoeId.Contains(x.Id))
                    .OrderByDescending(x => x.CongTyKhamSucKhoeId == congTyId)
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
        public string GetNameCongTy(long congTyId)
        {
            var query = _congTyKhamSucKhoeRepository.TableNoTracking.Where(d => d.Id == congTyId).Select(d => d.Ten).FirstOrDefault();
            return query;
        }
        public string GetNameHopDongKhamSucKhoe(long hopDongId)
        {
            var query = _hopDongKhamSucKhoeRepository.TableNoTracking.Where(d => d.Id == hopDongId).Select(d => d.SoHopDong).FirstOrDefault();
            return query;
        }

        private List<DanhSachDichVuKhamGrid> GetDataKetQuaKSKDoanEditFillterThoiDiemThucHienByHopDong(long hopDongKhamSucKhoeId)
        {
            var yeuCauTiepNhanIds =
             _yeuCauTiepNhanRepository.TableNoTracking
                .OrderByDescending(x => x.Id)
                .Where(x => x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId)
                .Select(o => o.Id).ToList();





            var thongTinInfos = new List<ThongTinInfo>();
            var entity = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => yeuCauTiepNhanIds.Contains(d.Id))
                .Select(d => new InfoDichVu
                {
                    YeuCauKhamBenhs = d.YeuCauKhamBenhs.ToList(),
                    YeuCauDichVuKyThuats = d.YeuCauDichVuKyThuats.ToList(),
                    ThongTinNhanVienKhamKetQuaKhamSucKhoeData = d.KetQuaKhamSucKhoeData,
                    ThongTinNhanVienKhamLoaiLuuInKetQuaKSK = d.LoaiLuuInKetQuaKSK,
                    HopDongKhamSucKhoeNhanVienId = d.HopDongKhamSucKhoeNhanVienId,
                    YeuCauDichVuKyThuatIds = d.YeuCauDichVuKyThuats.Select(g => new InfoDVKT { Id = g.Id, LoaiDichVuKyThuat = g.LoaiDichVuKyThuat }).ToList(),
                    YeuCauDichVuKyThuatPhauThuatThuThuats = d.YeuCauDichVuKyThuats.Select(h => new KetQuaDichVuKyThuatPTTT() { Id = h.Id, ketQua = h.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuCaPTTT }).ToList()
                }).ToList();

            #region get list tên dịch vụ khám
            var selectTenDichVuKhamBenhViens = _dichVuKhamBenhBenhVienRepository.TableNoTracking.Select(d => new { Id = d.Id, TenDichVu = d.Ten }).ToList();
            #endregion get list tên dịch vụ khám
            #region get list dịch vụ kỹ thuật
            var ids = entity.SelectMany(d => d.YeuCauDichVuKyThuatIds).Where(d => d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).Select(d => d.Id).ToList();
            //var listPhienXetNghiemChiTietss = _phienXetNghiemChiTietRepository.TableNoTracking
            //           .Include(d => d.KetQuaXetNghiemChiTiets)
            //           .Where(s => ids.Contains(s.YeuCauDichVuKyThuatId)).ToList();
            var listPhienXetNghiemChiTietss = _phienXetNghiemChiTietRepository.TableNoTracking
                  .Include(d => d.KetQuaXetNghiemChiTiets)
                  .Where(s => ids.Contains(s.YeuCauDichVuKyThuatId)).ToList();

            #endregion get list dịch vụ kỹ thuật

            #region dịch vụ xét nghiệm kết nối chỉ số
            var dichVuXetNghiemKetNoiChiSos = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Select(s => new
            {
                Id = s.Id,
                TenKetNoi = s.TenKetNoi
            }).ToList();
            #endregion dịch vụ xét nghiệm kết nối chỉ số

            foreach (var info in entity)
            {
                var thongTinInfo = new ThongTinInfo();

                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs = info.YeuCauKhamBenhs.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                                                                       .Select(v => new ThongTinNhanVienKhamTheoYeuCauKhamBenh
                                                                                       {
                                                                                           Id = v.Id,
                                                                                           TenDichVuId = v.DichVuKhamBenhBenhVienId,
                                                                                           ThongTinKhamTheoDichVuTemplate = v.ThongTinKhamTheoDichVuTemplate,
                                                                                           ThongTinKhamTheoDichVuData = v.ThongTinKhamTheoDichVuData,
                                                                                           TrangThaiDVKham = (int)v.TrangThai,
                                                                                           HopDongKhamSucKhoeNhanVienId = v.YeuCauTiepNhanId,
                                                                                           ThoiDiemThucHien = v.ThoiDiemThucHien
                                                                                       }).ToList();
                // lấy tên dịch vụ dịch vụ khám
                if (thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.Any())
                {
                    foreach (var infoYeuCauKhamBenh in thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.ToList())
                    {
                        infoYeuCauKhamBenh.TenDichVu = selectTenDichVuKhamBenhViens.Where(d => d.Id == infoYeuCauKhamBenh.TenDichVuId).Select(d => d.TenDichVu).FirstOrDefault();
                    }
                }
                thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs = info.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                                                                                     .Select(z => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatTDCNCDHA
                                                                                     {
                                                                                         DataKetQuaCanLamSang = z.DataKetQuaCanLamSang,
                                                                                         TenDichVuKyThuat = z.TenDichVu,
                                                                                         Id = z.Id,
                                                                                         GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                                                                                         TrangThaiDVKham = (int)z.TrangThai,
                                                                                         ThoiDiemThucHien = z.ThoiDiemThucHien
                                                                                     }).ToList();
                thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs = info.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                                                                    .Select(z => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatXetNghiem
                                                                                    {
                                                                                        //DataKetQuaCanLamSangVo = z.PhienXetNghiemChiTiets
                                                                                        //                        .Select(v => new DataKetQuaCanLamSangVo
                                                                                        //                        {
                                                                                        //                            KetQuaXetNghiemChiTiets = v.KetQuaXetNghiemChiTiets.ToList(),
                                                                                        //                            LanThucHien = v.LanThucHien,
                                                                                        //                            KetLuan = v.KetLuan
                                                                                        //                        }).OrderBy(s => s.LanThucHien)
                                                                                        //                        .LastOrDefault(),
                                                                                        TenDichVuKyThuat = z.TenDichVu,
                                                                                        Id = z.Id,
                                                                                        GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                                                                                        TrangThaiDVKham = (int)z.TrangThai,
                                                                                        ThoiDiemThucHien = z.ThoiDiemThucHien
                                                                                    }).ToList();
                // lấy info phiên kêt qua xet nghien
                if (thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.Any())
                {
                    foreach (var idDichVuKyThuat in thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.ToList())
                    {
                        //idDichVuKyThuat.DataKetQuaCanLamSangVo = listPhienXetNghiemChiTietss.Where(d => d.Id == idDichVuKyThuat.Id)
                        //                                                                    .Select(z => z.PhienXetNghiemChiTiets.Select(v => new DataKetQuaCanLamSangVo
                        //                                                                                                         {
                        //                                                                                                         KetQuaXetNghiemChiTiets = v.KetQuaXetNghiemChiTiets.ToList(),
                        //                                                                                                         LanThucHien = v.LanThucHien,
                        //                                                                                                         KetLuan = v.KetLuan
                        //                                                                                                         }).OrderBy(s => s.LanThucHien).LastOrDefault()
                        //                                                                     ).FirstOrDefault();
                        idDichVuKyThuat.DataKetQuaCanLamSangVo = listPhienXetNghiemChiTietss.Where(d => d.YeuCauDichVuKyThuatId == idDichVuKyThuat.Id)
                                                                                          .Select(v => new DataKetQuaCanLamSangVo
                                                                                          {
                                                                                              KetQuaXetNghiemChiTiets = v.KetQuaXetNghiemChiTiets.ToList(),
                                                                                              LanThucHien = v.LanThucHien,
                                                                                              KetLuan = v.KetLuan
                                                                                          }).OrderBy(s => s.LanThucHien)
                                                                                          .LastOrDefault();
                    }
                }

                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats = info.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang && s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                    .Select(d => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuat
                    {
                        Id = d.Id,
                        GoiKhamSucKhoeId = d.GoiKhamSucKhoeId,
                        TenDichVuKyThuat = d.TenDichVu,
                        TrangThaiDVKham = (int)d.TrangThai,
                        ThoiDiemThucHien = d.ThoiDiemThucHien
                    }).ToList();


                // BVHD-3877 thủ thuật phẩu thuật
                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats = info.YeuCauDichVuKyThuats.Where(s => s.GoiKhamSucKhoeId != null && s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                 .Select(d => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuat
                 {
                     Id = d.Id,
                     GoiKhamSucKhoeId = d.GoiKhamSucKhoeId,
                     TenDichVuKyThuat = d.TenDichVu,
                     TrangThaiDVKham = (int)d.TrangThai,
                     KetQua = d.YeuCauDichVuKyThuatTuongTrinhPTTT != null ? d.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuCaPTTT : ""
                 }).ToList();

                foreach (var item in thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats)
                {
                    if (info.YeuCauDichVuKyThuatPhauThuatThuThuats.Where(d => d.Id == item.Id).Count() != 0)
                    {
                        item.KetQua = info.YeuCauDichVuKyThuatPhauThuatThuThuats.Where(d => d.Id == item.Id).Select(d => d.ketQua).FirstOrDefault();
                    }
                }
                //END BVHD-3877 thủ thuật phẩu thuật


                thongTinInfo.ThongTinNhanVienKhamKetQuaKhamSucKhoeData = info.ThongTinNhanVienKhamKetQuaKhamSucKhoeData;
                thongTinInfo.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK = info.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK;
                thongTinInfo.HopDongKhamSucKhoeNhanVienId = info.HopDongKhamSucKhoeNhanVienId;
                thongTinInfos.Add(thongTinInfo);
            }

   

            List<DanhSachDichVuKhamGrid> listDichVu = new List<DanhSachDichVuKhamGrid>();
            if (yeuCauTiepNhanIds != null)
            {
                foreach (var thongTinNhanVienKham in thongTinInfos)
                {
                    var data = new KetQuaKhamSucKhoeVo();
                    var tableKham = "";
                    var tableKyThuat = "";
                    List<DanhSachDichVuKhamGrid> listDichVuCu = new List<DanhSachDichVuKhamGrid>();
                    List<long> listDichVuKyThuatTheoGoi = new List<long>();
                    if (thongTinNhanVienKham != null)
                    {

                        // DV Kham
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.Any())
                        {
                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDv.Id;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKhamBenh;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVu;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                                dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;
                                dvObject.ThoiDiemThucHien = itemDv.ThoiDiemThucHien;
                                if (itemDv.ThongTinKhamTheoDichVuTemplate != null && itemDv.ThongTinKhamTheoDichVuData != null  && itemDv.TrangThaiDVKham != 6) 
                                {
                                    var jsonOjbectTemplate = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(itemDv.ThongTinKhamTheoDichVuTemplate);
                                    var jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);

                                    //cập nhật BVHD-3880 cập nhật trạng thái dịch vụ
                                    if (jsonOjbectData.DataKhamTheoTemplate.Count() == 0)
                                    {
                                        if (dvObject.TenDichVu == "Khám Ngoại")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa);
                                        }
                                        if (dvObject.TenDichVu == "Khám Mắt")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.Mat);
                                        }

                                        if (dvObject.TenDichVu == "Khám Răng Hàm Mặt")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.RangHamMat);
                                        }

                                        if (dvObject.TenDichVu == "Khám Tai Mũi Họng")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                        }
                                        if (dvObject.TenDichVu == "Khám Da liễu")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                        }
                                        if (dvObject.TenDichVu == "Nội khoa")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa);
                                        }
                                        if (dvObject.TenDichVu == "Sản phụ khoa")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa);
                                        }
                                        jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);
                                    }


                                    foreach (var itemx in jsonOjbectTemplate.ComponentDynamics)
                                    {
                                        var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemx.Id);
                                        if (kiemTra.Any())
                                        {
                                            switch (itemx.Id)
                                            {
                                                case "TuanHoan":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;
                                                case "HoHap":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "TieuHoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "ThanTietLieu":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "NoiTiet":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "CoXuongKhop":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "ThanKinh":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "TamThan":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "NgoaiKhoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenNgoaiKhoa;
                                                    break;
                                                case "SanPhuKhoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenSanPhuKhoa;
                                                    break;


                                                case "CacBenhVeMat":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaMat;
                                                    break;
                                                case "CacBenhTaiMuiHong":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaTaiMuiHong;
                                                    break;

                                                case "CacBenhRangHamMat":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;
                                                case "HamTren":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;
                                                case "HamDuoi":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;

                                                case "DaLieu":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenDaLieu;
                                                    break;
                                                default:
                                                    //do a different thing
                                                    break;
                                            }
                                        }


                                    }
                                }
                                dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVu;
                                listDichVu.Add(dvObject);
                            }

                        }

                        // list theo yêu cầu tiếp nhận
                        // CDHA TDCN
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs.Any())
                        {

                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDv.Id;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;
                                dvObject.ThoiDiemThucHien = itemDv.ThoiDiemThucHien;

                                if (itemDv.DataKetQuaCanLamSang != null && itemDv.TrangThaiDVKham == 3) // != 1 => dịch vụ chưa thực hiện
                                {
                                    var jsonOjbect = JsonConvert.DeserializeObject<DataCLS>(itemDv.DataKetQuaCanLamSang);

                                    //dvObject.KetQuaDichVuDefault = jsonOjbect.KetQua;
                                    //dvObject.KetQuaDichVu = jsonOjbect.KetQua;
                                    var ketLuan = jsonOjbect.KetLuan;
                                    if (!string.IsNullOrEmpty(ketLuan))
                                    {
                                        ketLuan = CommonHelper.StripHTML(Regex.Replace(ketLuan, "</p>(?![\n\r]+)", Environment.NewLine));
                                        if (ketLuan.Length > 2 && ketLuan.Substring(ketLuan.Length - 2) == "\r\n")
                                        {
                                            ketLuan = ketLuan.Remove(ketLuan.Length - 2);
                                        }
                                    }
                                    dvObject.KetQuaDichVuDefault = ketLuan;
                                    dvObject.KetQuaDichVu = ketLuan;
                                }

                                listDichVu.Add(dvObject);
                            }
                        }

                        // xét nghiệm
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.Any())
                        {

                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();


                                dvObject.Id = itemDv.Id;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                                dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                                dvObject.ThoiDiemThucHien = itemDv.ThoiDiemThucHien;
                                if (itemDv.DataKetQuaCanLamSangVo != null && itemDv.TrangThaiDVKham == 3) // !=1 => chưa thực hiện
                                {
                                    // phiên xét nghiệm chi tiết orderby cuoi cung
                                    if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets != null)
                                    {
                                        if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Any())
                                        {
                                            if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count == 1)
                                            {
                                                var itemGiaTriMin = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMin).First();
                                                var itemGiaTriMax = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMax).First();
                                                var itemGTDuyet = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriDuyet).First();
                                                var itemGiaTriNhapTay = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriNhapTay).First();
                                                var itemGiaTriTuMay = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriTuMay).First();
                                                var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                                                                                                                                                                                                                                       //double ketQua = !string.IsNullOrEmpty(value)  ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;

                                                if (!string.IsNullOrEmpty(value) && IsInt(value))
                                                {
                                                    double ketQua = !string.IsNullOrEmpty(value) ? Convert.ToDouble(value) : 0;
                                                    double cSBTMin = 0;
                                                    double cSBTMax = 0;
                                                    if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                    {
                                                        dvObject.KetQuaDichVu = ketQua.ToString() + "";
                                                        dvObject.KetQuaDichVuDefault = ketQua.ToString() + "";
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                    if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                    {
                                                        if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                            }
                                                            else
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                }
                                                            }

                                                        }

                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                }
                                                if (!string.IsNullOrEmpty(value) && !IsInt(value))
                                                {
                                                    string ketQua = !string.IsNullOrEmpty(value) ? value : " ";
                                                    dvObject.KetQuaDichVu = ketQua.ToString() + " ";
                                                    dvObject.KetQuaDichVuDefault = ketQua.ToString() + " ";
                                                }
                                            }
                                            else
                                            {
                                                int itemCongKyTu = 0;
                                                foreach (var itemKetQuaListCon in itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.OrderByDescending(d => d.CapDichVu == 1 ? 1 : 0).ThenBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList())
                                                {
                                                    var dichVuXetNghiemId = itemKetQuaListCon.DichVuXetNghiemId;
                                                    var mauMayXetNghiemId = itemKetQuaListCon.MauMayXetNghiemId;
                                                    var tenketQua = "";
                                                    // nếu  mẫu máy xét nghiệm khác null => lấy ten dich vụ xét nghiệm trong db.DichVuXetNghiemKetNoiChiSo => field : TenKetNoi
                                                    // DichVuXetNghiemKetNoiChiSoId != null
                                                    if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null)
                                                    {
                                                        tenketQua = dichVuXetNghiemKetNoiChiSos.Where(s => s.Id == itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId.GetValueOrDefault()).Select(s => s.TenKetNoi).FirstOrDefault();
                                                        dvObject.KetQuaDichVu += tenketQua + ": ";
                                                        dvObject.KetQuaDichVuDefault += tenketQua + ": ";

                                                        var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                        var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                        var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                        var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                        var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                        var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                        double ketQua;
                                                        bool KieuSo = false;
                                                        if (value != null)
                                                        {
                                                            KieuSo = IsInt(value) ? true : false;
                                                        }
                                                        else
                                                        {
                                                            KieuSo = false;
                                                        }
                                                        if (KieuSo == true)
                                                        {
                                                            double cSBTMin = 0;
                                                            double cSBTMax = 0;
                                                            ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                            if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                            if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                            {
                                                                if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                        if (!string.IsNullOrEmpty(max))
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                        }
                                                                        else
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                        }
                                                                    }

                                                                }

                                                            }
                                                            if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                                {
                                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                    if (!string.IsNullOrEmpty(max))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE

                                                        }
                                                        if (KieuSo == false)
                                                        {
                                                            dvObject.KetQuaDichVu += value + "";
                                                            dvObject.KetQuaDichVuDefault += value + "";
                                                        }

                                                        if (itemCongKyTu < itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count())
                                                        {
                                                            dvObject.KetQuaDichVu += "; ";
                                                            dvObject.KetQuaDichVuDefault += "; ";
                                                        }

                                                    }
                                                    // nếu mẫu máy xét nghiệm == null => tên dịch vụ xét nghiệm trong db.KetQuaXetNghiemChiTiet => field :DichVuXetNghiemTen 
                                                    // DichVuXetNghiemKetNoiChiSoId == null
                                                    if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId == null)
                                                    {
                                                        dvObject.KetQuaDichVu += itemKetQuaListCon.DichVuXetNghiemTen + ": ";
                                                        dvObject.KetQuaDichVuDefault += itemKetQuaListCon.DichVuXetNghiemTen + ": ";

                                                        var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                        var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                        var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                        var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                        var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                        //var value = itemGTDuyet ?? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? null));
                                                        var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                        double ketQua;
                                                        bool KieuSo = false;
                                                        if (value != null)
                                                        {
                                                            KieuSo = IsInt(value) ? true : false;
                                                        }
                                                        else
                                                        {
                                                            KieuSo = false;
                                                        }
                                                        double cSBTMin = 0;
                                                        double cSBTMax = 0;
                                                        if (KieuSo == true)
                                                        {
                                                            ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                            if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                            if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                            {
                                                                if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                        if (!string.IsNullOrEmpty(max))
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                        }
                                                                        else
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                        }
                                                                    }

                                                                }

                                                            }
                                                            if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                                {
                                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                    if (!string.IsNullOrEmpty(max))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                        }
                                                        if (KieuSo == false)
                                                        {
                                                            dvObject.KetQuaDichVu += value + "";
                                                            dvObject.KetQuaDichVuDefault += value + "";
                                                        }

                                                        if (itemCongKyTu < itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count())
                                                        {
                                                            dvObject.KetQuaDichVu += "; ";
                                                            dvObject.KetQuaDichVuDefault += "; ";
                                                        }
                                                    }
                                                    itemCongKyTu++;
                                                }
                                            }

                                            if (dvObject.KetQuaDichVu == "" || dvObject.KetQuaDichVu == "0")
                                            {
                                                dvObject.KetQuaDichVu = "";
                                                dvObject.KetQuaDichVuDefault = "";
                                            }
                                        }
                                    }
                                }

                                listDichVu.Add(dvObject);
                            }
                        }

                        // BVHD-3668 -> lấy những dịch vụ kỹ thuật khác cls -> kết quả để null
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats.Any())
                        {
                            foreach (var itemDVKTKhacCLS in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDVKTKhacCLS.Id;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThaiDVKham;
                                dvObject.KetQuaDichVu = string.Empty; // để tự nhập
                                dvObject.KetQuaDichVuDefault = string.Empty; // để tự nhập
                                dvObject.ThoiDiemThucHien = itemDVKTKhacCLS.ThoiDiemThucHien;
                                listDichVu.Add(dvObject);
                            }
                        }


                        // BVHD-3877 -> thủ thuật phẩu thuật
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats.Any())
                        {
                            foreach (var itemDVKTKhacCLS in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDVKTKhacCLS.Id;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThaiDVKham;
                                dvObject.KetQuaDichVu = itemDVKTKhacCLS.KetQua; // để tự nhập
                                dvObject.KetQuaDichVuDefault = itemDVKTKhacCLS.KetQua; // để tự nhập
                                dvObject.ThoiDiemThucHien = itemDVKTKhacCLS.ThoiDiemThucHien;
                                listDichVu.Add(dvObject);
                            }
                        }



                        if (thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData != null)
                        {
                            // chạy những data cũ  chưa lưu người thực hiện , và thời điểm thực hiện trong josn
                            if (thongTinNhanVienKham.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK == null)
                            {
                                listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData);
                                // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                                foreach (var itemxDvMoi in listDichVu)
                                {
                                    foreach (var dvcu in listDichVuCu)
                                    {
                                        if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                        {
                                            itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var jonKetLuan = JsonConvert.DeserializeObject<KetQuaKhamSucKhoeDaTa>(thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData);
                                if (!string.IsNullOrEmpty(jonKetLuan.KetQuaKhamSucKhoe))
                                {
                                    listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(jonKetLuan.KetQuaKhamSucKhoe);
                                    // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                                    foreach (var itemxDvMoi in listDichVu)
                                    {
                                        foreach (var dvcu in listDichVuCu)
                                        {
                                            if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                            {
                                                itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                            }
                                        }
                                    }
                                }
                            }


                        }

                    }
                    // xử lý list string trùng nhau (dịch vụ khám)
                    //List<DanhSachDichVuKhamGrid> listDichVuLoaiTrung = new List<DanhSachDichVuKhamGrid>();

                    foreach (var itemTrung in listDichVu.Where(s => s.NhomId != EnumNhomGoiDichVu.DichVuKyThuat).ToList())
                    {
                        if (!string.IsNullOrEmpty(itemTrung.KetQuaDichVu) && !string.IsNullOrEmpty(itemTrung.KetQuaDichVuDefault))
                        {
                            var catstring = itemTrung.KetQuaDichVu.Split('.');
                            var catstringdefault = itemTrung.KetQuaDichVuDefault.Split('.');
                            itemTrung.KetQuaDichVu = catstring.Where(d => d != null && d != "").Distinct().Join(".");
                            itemTrung.KetQuaDichVuDefault = catstringdefault.Where(d => d != null && d != "").Distinct().Join(".");

                        }
                    }
                }

            }


            return listDichVu.OrderBy(o => o.TenDichVu).ToList(); // trả về 1 list dịch vụ (dịch vụ khám , cls (dịch vụ kỹ thuật))
        }
        private List<DanhSachDichVuKhamGrid> GetDataKetQuaKSKDoanEditFillterThoiDiemThucHienByHopDongNew(long hopDongKhamSucKhoeId, List<long> tiepNhanIds)
        {
            var thongTinInfos = new List<ThongTinInfo>();


            var entity = _yeuCauTiepNhanRepository.TableNoTracking.Where(x => tiepNhanIds.Contains(x.Id))
                .Select(d => new InfoDichVu
                {
                    YeuCauKhamBenhIds = d.YeuCauKhamBenhs.Select(f => f.Id).ToList(),
                    YeuCauDVKTIds = d.YeuCauDichVuKyThuats.Select(g => g.Id).ToList(),
                    ThongTinNhanVienKhamKetQuaKhamSucKhoeData = d.KetQuaKhamSucKhoeData,
                    ThongTinNhanVienKhamLoaiLuuInKetQuaKSK = d.LoaiLuuInKetQuaKSK,
                    HopDongKhamSucKhoeNhanVienId = d.HopDongKhamSucKhoeNhanVienId,

                    YeuCauDichVuKyThuatIds = d.YeuCauDichVuKyThuats.Select(g => new InfoDVKT { Id = g.Id, LoaiDichVuKyThuat = g.LoaiDichVuKyThuat }).ToList(),
                }).ToList();

            // get info YeuCauKham
            var yckhamIds = entity.SelectMany(d => d.YeuCauKhamBenhIds).Select(d => d).ToList();
            var infoYeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(d => yckhamIds.Contains(d.Id) &&
                            d.GoiKhamSucKhoeId != null &&
                            d.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Select(v => new ThongTinNhanVienKhamTheoYeuCauKhamBenh
                {
                    Id = v.Id,
                    TenDichVuId = v.DichVuKhamBenhBenhVienId,
                    ThongTinKhamTheoDichVuTemplate = v.ThongTinKhamTheoDichVuTemplate,
                    ThongTinKhamTheoDichVuData = v.ThongTinKhamTheoDichVuData,
                    TrangThaiDVKham = (int)v.TrangThai,
                    HopDongKhamSucKhoeNhanVienId = v.YeuCauTiepNhanId,
                    ThoiDiemThucHien = v.ThoiDiemThucHien
                }).ToList();

            // get info Yeu cầu dịch vụ kỹ thuật TDCN/CDHA
            var ycDVKIds = entity.SelectMany(d => d.YeuCauDichVuKyThuatIds).Select(d => d.Id).ToList();
            var infoDVKTTDCNCDHAs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => ycDVKIds.Contains(s.Id) &&
                            s.GoiKhamSucKhoeId != null &&
                            s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                            (s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
               .Select(z => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatTDCNCDHA
               {
                   DataKetQuaCanLamSang = z.DataKetQuaCanLamSang,
                   TenDichVuKyThuat = z.TenDichVu,
                   Id = z.Id,
                   GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                   TrangThaiDVKham = (int)z.TrangThai,
                   ThoiDiemThucHien = z.ThoiDiemThucHien
               }).ToList();
            // get in fo yêu cầu dịch vụ kỹ thuật xét nghiệm

            var infoXetNghiems = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => ycDVKIds.Contains(s.Id) &&
                            s.GoiKhamSucKhoeId != null &&
                            s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                            s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                .Select(z => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatXetNghiem
                {
                    TenDichVuKyThuat = z.TenDichVu,
                    Id = z.Id,
                    GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                    TrangThaiDVKham = (int)z.TrangThai,
                    ThoiDiemThucHien = z.ThoiDiemThucHien
                }).ToList();


            // get dịch vụ kỹ thuật khác CLS , ! thủ thuật phẩu huật
            var infoDichVuKhacClSVaKhacThuThuatPTs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => ycDVKIds.Contains(s.Id) &&
                            s.GoiKhamSucKhoeId != null &&
                            s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                            s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.XetNghiem &&
                            s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh &&
                            s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThamDoChucNang &&
                            s.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                .Select(d => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuat
                {
                    Id = d.Id,
                    GoiKhamSucKhoeId = d.GoiKhamSucKhoeId,
                    TenDichVuKyThuat = d.TenDichVu,
                    TrangThaiDVKham = (int)d.TrangThai,
                    ThoiDiemThucHien = d.ThoiDiemThucHien
                }).ToList();

            // get in fo yêu cầu dịch vụ kỹ thuật  ! xét nghiệm !TDCN/CDHA
            var infopttts = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => ycDVKIds.Contains(s.Id) &&
                            s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                            s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                .Select(d => new ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuat
                {
                    Id = d.Id,
                    GoiKhamSucKhoeId = d.GoiKhamSucKhoeId,
                    TenDichVuKyThuat = d.TenDichVu,
                    TrangThaiDVKham = (int)d.TrangThai,
                    ThoiDiemThucHien = d.ThoiDiemThucHien
                }).ToList();

            var dichVuKyThuatTuongTrinhPTTTIds = entity.SelectMany(d => d.YeuCauDichVuKyThuatIds)
                .Where(d => d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                .Select(d => d.Id).ToList();

            var ketQuaPTTTs = _yeuCauDichVuKyThuatTuongTrinhPTTTRepository.TableNoTracking.Where(d => dichVuKyThuatTuongTrinhPTTTIds.Contains(d.Id))
                      .Select(d => new KetQuaDichVuKyThuatPTTT
                      {
                          Id = d.Id,
                          ketQua = d.GhiChuCaPTTT
                      }).ToList();


            #region get list tên dịch vụ khám
            var selectTenDichVuKhamBenhViens = _dichVuKhamBenhBenhVienRepository.TableNoTracking.Select(d => new { Id = d.Id, TenDichVu = d.Ten }).ToList();
            #endregion get list tên dịch vụ khám
            #region get list dịch vụ kỹ thuật
            var ids = entity.SelectMany(d => d.YeuCauDichVuKyThuatIds).Where(d => d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).Select(d => d.Id).ToList();

            var listPhienXetNghiemChiTietss = _phienXetNghiemChiTietRepository.TableNoTracking
                .Include(d => d.KetQuaXetNghiemChiTiets)
                .Where(s => ids.Contains(s.YeuCauDichVuKyThuatId)).ToList();

            //var listPhienXetNghiemChiTietss = _ketQuaXetNghiemChiTietRepository.TableNoTracking
            //    .Where(s => ids.Contains(s.PhienXetNghiemChiTiet.YeuCauDichVuKyThuatId))
            //    .Select(o=> new
            //    {
            //        YeuCauDichVuKyThuatId = o.PhienXetNghiemChiTiet.YeuCauDichVuKyThuatId,
            //        o.LanThucHien,
            //        o.PhienXetNghiemChiTiet.KetLuan,
            //        KetQuaXetNghiemChiTiet = o
            //    })
            //    .ToList();


            #region dịch vụ xét nghiệm kết nối chỉ số
            var dichVuXetNghiemKetNoiChiSos = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Where(d => d.DichVuXetNghiemId != null && d.MauMayXetNghiemId != null).Select(s => new
            {
                Id = s.Id,
                TenKetNoi = s.TenKetNoi
            }).ToList();
            #endregion dịch vụ xét nghiệm kết nối chỉ số

            #endregion get list tên dịch vụ khám

            foreach (var info in entity)
            {
                var thongTinInfo = new ThongTinInfo();

                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs = infoYeuCauKhams.Where(d => info.YeuCauKhamBenhIds.Contains(d.Id)).ToList();
                // lấy tên dịch vụ dịch vụ khám
                if (thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.Any())
                {
                    foreach (var infoYeuCauKhamBenh in thongTinInfo.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.ToList())
                    {
                        infoYeuCauKhamBenh.TenDichVu = selectTenDichVuKhamBenhViens.Where(d => d.Id == infoYeuCauKhamBenh.TenDichVuId).Select(d => d.TenDichVu).FirstOrDefault();
                    }
                }

                thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs = infoDVKTTDCNCDHAs.Where(d => info.YeuCauDVKTIds.Contains(d.Id)).ToList();

                thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs = infoXetNghiems.Where(d => info.YeuCauDVKTIds.Contains(d.Id)).ToList();

                // lấy info phiên kêt qua xet nghien
                if (thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.Any())
                {
                    foreach (var idDichVuKyThuat in thongTinInfo.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.ToList())
                    {
                        idDichVuKyThuat.DataKetQuaCanLamSangVo = null;
                        idDichVuKyThuat.DataKetQuaCanLamSangVo = listPhienXetNghiemChiTietss.Where(d => d.YeuCauDichVuKyThuatId == idDichVuKyThuat.Id)
                                                                                                                .Select(v => new DataKetQuaCanLamSangVo
                                                                                                                {
                                                                                                                    KetQuaXetNghiemChiTiets = v.KetQuaXetNghiemChiTiets.ToList(),
                                                                                                                    LanThucHien = v.LanThucHien,
                                                                                                                    KetLuan = v.KetLuan
                                                                                                                }).OrderBy(s => s.LanThucHien)
                                                                                                                .LastOrDefault();

                    }
                }

                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats = infoDichVuKhacClSVaKhacThuThuatPTs.Where(d => info.YeuCauDVKTIds.Contains(d.Id)).ToList();



                // BVHD-3877 thủ thuật phẩu thuật
                thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats = infopttts.Where(d => info.YeuCauDVKTIds.Contains(d.Id)).ToList();

                foreach (var item in thongTinInfo.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats)
                {
                    if (ketQuaPTTTs.Where(d => d.Id == item.Id).Count() != 0)
                    {
                        item.KetQua = ketQuaPTTTs.Where(d => d.Id == item.Id).Select(d => d.ketQua).FirstOrDefault();
                    }
                }




                thongTinInfo.ThongTinNhanVienKhamKetQuaKhamSucKhoeData = info.ThongTinNhanVienKhamKetQuaKhamSucKhoeData;
                thongTinInfo.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK = info.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK;
                thongTinInfo.HopDongKhamSucKhoeNhanVienId = info.HopDongKhamSucKhoeNhanVienId;
                thongTinInfos.Add(thongTinInfo);
            }

            List<DanhSachDichVuKhamGrid> listDichVu = new List<DanhSachDichVuKhamGrid>();


            if (thongTinInfos != null)
            {
                foreach (var thongTinNhanVienKham in thongTinInfos)
                {
                    var data = new KetQuaKhamSucKhoeVo();
                    var tableKham = "";
                    var tableKyThuat = "";
                    List<DanhSachDichVuKhamGrid> listDichVuCu = new List<DanhSachDichVuKhamGrid>();
                    List<long> listDichVuKyThuatTheoGoi = new List<long>();
                    if (thongTinNhanVienKham != null)
                    {
                        // DV Kham
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauKhamBenhs.Any())
                        {
                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauKhamBenhs)
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDv.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKhamBenh;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVu;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                                dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;
                                dvObject.ThoiDiemThucHien = itemDv.ThoiDiemThucHien;
                                //dvObject.TrangThaiThucHienDichVu = itemDv.TrangThaiThucHienDichVu == 5 ? 3 :0;// 3 đã khám

                                if (itemDv.ThongTinKhamTheoDichVuTemplate != null && itemDv.ThongTinKhamTheoDichVuData != null && itemDv.TrangThaiDVKham != 6)
                                {
                                    var jsonOjbectTemplate = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(itemDv.ThongTinKhamTheoDichVuTemplate);
                                    var jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);

                                    //cập nhật BVHD-3880 cập nhật trạng thái dịch vụ
                                    if (jsonOjbectData.DataKhamTheoTemplate.Count() == 0)
                                    {
                                        if (dvObject.TenDichVu == "Khám Ngoại")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa);
                                        }
                                        if (dvObject.TenDichVu == "Khám Mắt")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.Mat);
                                        }

                                        if (dvObject.TenDichVu == "Khám Răng Hàm Mặt")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.RangHamMat);
                                        }

                                        if (dvObject.TenDichVu == "Khám Tai Mũi Họng")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                        }
                                        if (dvObject.TenDichVu == "Khám Da liễu")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong);
                                        }
                                        if (dvObject.TenDichVu == "Nội khoa")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa);
                                        }
                                        if (dvObject.TenDichVu == "Sản phụ khoa")
                                        {
                                            itemDv.ThongTinKhamTheoDichVuData = SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa);
                                        }
                                        jsonOjbectData = JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(itemDv.ThongTinKhamTheoDichVuData);
                                    }


                                    foreach (var itemx in jsonOjbectTemplate.ComponentDynamics)
                                    {
                                        var kiemTra = jsonOjbectData.DataKhamTheoTemplate.Where(s => s.Id == itemx.Id);
                                        if (kiemTra.Any())
                                        {
                                            switch (itemx.Id)
                                            {
                                                case "TuanHoan":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;
                                                case "HoHap":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "TieuHoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "ThanTietLieu":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "NoiTiet":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "CoXuongKhop":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "ThanKinh":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "TamThan":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaNoi;
                                                    break;

                                                case "NgoaiKhoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenNgoaiKhoa;
                                                    break;
                                                case "SanPhuKhoa":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenSanPhuKhoa;
                                                    break;


                                                case "CacBenhVeMat":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaMat;
                                                    break;
                                                case "CacBenhTaiMuiHong":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaTaiMuiHong;
                                                    break;

                                                case "CacBenhRangHamMat":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;
                                                case "HamTren":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;
                                                case "HamDuoi":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenKhoaRangHamMat;
                                                    break;

                                                case "DaLieu":
                                                    if (kiemTra.FirstOrDefault().Value != null)
                                                    {
                                                        if (dvObject.TrangThaiDVKham == 5)
                                                        {
                                                            dvObject.KetQuaDichVu += kiemTra.FirstOrDefault().Value + ".";
                                                        }
                                                    }
                                                    dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.ChuyenDaLieu;
                                                    break;
                                                default:
                                                    //do a different thing
                                                    break;
                                            }
                                        }


                                    }
                                }
                                dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVu;
                                listDichVu.Add(dvObject);
                            }

                        }

                        // list theo yêu cầu tiếp nhận
                        // CDHA TDCN
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs.Any())
                        {

                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatTDCNCDHAs.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDv.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;
                                dvObject.ThoiDiemThucHien = itemDv.ThoiDiemThucHien;
                                if (itemDv.DataKetQuaCanLamSang != null && itemDv.TrangThaiDVKham == 3) // ==3 => dịch vụ đã thực hiện
                                {
                                    var jsonOjbect = JsonConvert.DeserializeObject<DataCLS>(itemDv.DataKetQuaCanLamSang);

                                    var ketLuan = jsonOjbect.KetLuan;
                                    if (!string.IsNullOrEmpty(ketLuan))
                                    {
                                        ketLuan = CommonHelper.StripHTML(Regex.Replace(ketLuan, "</p>(?![\n\r]+)", Environment.NewLine));
                                        if (ketLuan.Length > 2 && ketLuan.Substring(ketLuan.Length - 2) == "\r\n")
                                        {
                                            ketLuan = ketLuan.Remove(ketLuan.Length - 2);
                                        }
                                    }
                                    dvObject.KetQuaDichVuDefault = ketLuan;
                                    dvObject.KetQuaDichVu = ketLuan;
                                }

                                listDichVu.Add(dvObject);
                            }
                        }

                        // xét nghiệm
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.Any())
                        {

                            foreach (var itemDv in thongTinNhanVienKham.ThongTinNhanVienKhamYeuCauDichVuKyThuatXNs.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();


                                dvObject.Id = itemDv.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatXN;
                                dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                                dvObject.ThoiDiemThucHien = itemDv.ThoiDiemThucHien;
                                if (itemDv.DataKetQuaCanLamSangVo != null && itemDv.TrangThaiDVKham == 3) // ==3 => đã thực hiện
                                {
                                    // phiên xét nghiệm chi tiết orderby cuoi cung
                                    if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets != null)
                                    {
                                        if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Any())
                                        {
                                            if (itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count == 1)
                                            {
                                                var itemGiaTriMin = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMin).First();
                                                var itemGiaTriMax = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMax).First();
                                                var itemGTDuyet = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriDuyet).First();
                                                var itemGiaTriNhapTay = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriNhapTay).First();
                                                var itemGiaTriTuMay = itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriTuMay).First();
                                                var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;

                                                if (!string.IsNullOrEmpty(value) && IsInt(value))
                                                {
                                                    double ketQua = !string.IsNullOrEmpty(value) ? Convert.ToDouble(value) : 0;

                                                    if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                    {
                                                        dvObject.KetQuaDichVu = ketQua.ToString() + "";
                                                        dvObject.KetQuaDichVuDefault = ketQua.ToString() + "";
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                    if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                    {
                                                        if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                            }
                                                            else
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                }
                                                            }

                                                        }

                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                            }
                                                            else
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                }
                                                if (!string.IsNullOrEmpty(value) && !IsInt(value))
                                                {
                                                    string ketQua = !string.IsNullOrEmpty(value) ? value : " ";
                                                    dvObject.KetQuaDichVu = ketQua.ToString() + " ";
                                                    dvObject.KetQuaDichVuDefault = ketQua.ToString() + " ";
                                                }
                                            }
                                            else
                                            {
                                                int itemCongKyTu = 0;
                                                foreach (var itemKetQuaListCon in itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.OrderByDescending(d => d.CapDichVu == 1 ? 1 : 0).ThenBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList())
                                                {
                                                    var dichVuXetNghiemId = itemKetQuaListCon.DichVuXetNghiemId;
                                                    var mauMayXetNghiemId = itemKetQuaListCon.MauMayXetNghiemId;
                                                    var tenketQua = "";
                                                    // nếu  mẫu máy xét nghiệm khác null => lấy ten dich vụ xét nghiệm trong db.DichVuXetNghiemKetNoiChiSo => field : TenKetNoi
                                                    // DichVuXetNghiemKetNoiChiSoId != null 
                                                    if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null)
                                                    {
                                                        tenketQua = dichVuXetNghiemKetNoiChiSos.Where(s => s.Id == itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId.GetValueOrDefault()).Select(s => s.TenKetNoi).FirstOrDefault();
                                                        if (tenketQua != null)
                                                        {
                                                            dvObject.KetQuaDichVu += tenketQua + ": ";
                                                            dvObject.KetQuaDichVuDefault += tenketQua + ": ";
                                                        }

                                                        var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                        var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                        var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                        var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                        var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                        var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                        double ketQua;
                                                        bool KieuSo = false;
                                                        if (value != null)
                                                        {
                                                            KieuSo = IsInt(value) ? true : false;
                                                        }
                                                        else
                                                        {
                                                            KieuSo = false;
                                                        }
                                                        if (KieuSo == true)
                                                        {
                                                            double cSBTMin = 0;
                                                            double cSBTMax = 0;
                                                            ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                            if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                            if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                            {
                                                                if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                        if (!string.IsNullOrEmpty(max))
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                        }
                                                                        else
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                        }
                                                                    }

                                                                }

                                                            }
                                                            if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                                {
                                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                    if (!string.IsNullOrEmpty(max))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE

                                                        }
                                                        if (KieuSo == false)
                                                        {
                                                            dvObject.KetQuaDichVu += value + "";
                                                            dvObject.KetQuaDichVuDefault += value + "";
                                                        }

                                                        if (itemCongKyTu < itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count())
                                                        {
                                                            dvObject.KetQuaDichVu += "; ";
                                                            dvObject.KetQuaDichVuDefault += "; ";
                                                        }

                                                    }
                                                    // nếu mẫu máy xét nghiệm == null => tên dịch vụ xét nghiệm trong db.KetQuaXetNghiemChiTiet => field :DichVuXetNghiemTen 
                                                    // DichVuXetNghiemKetNoiChiSoId == null  
                                                    if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId == null)
                                                    {
                                                        dvObject.KetQuaDichVu += itemKetQuaListCon.DichVuXetNghiemTen + ": ";
                                                        dvObject.KetQuaDichVuDefault += itemKetQuaListCon.DichVuXetNghiemTen + ": ";

                                                        var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                        var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                        var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                        var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                        var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                        //var value = itemGTDuyet ?? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? null));
                                                        var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty;
                                                        double ketQua;
                                                        bool KieuSo = false;
                                                        if (value != null)
                                                        {
                                                            KieuSo = IsInt(value) ? true : false;
                                                        }
                                                        else
                                                        {
                                                            KieuSo = false;
                                                        }
                                                        double cSBTMin = 0;
                                                        double cSBTMax = 0;
                                                        if (KieuSo == true)
                                                        {
                                                            ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                            if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                            {
                                                                dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                            if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                            {
                                                                if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " (Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                        if (!string.IsNullOrEmpty(max))
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + " (Tăng)";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                        }
                                                                        else
                                                                        {
                                                                            dvObject.KetQuaDichVu += ketQua.ToString() + "";
                                                                            dvObject.KetQuaDichVuDefault += ketQua.ToString() + "";
                                                                        }
                                                                    }

                                                                }

                                                            }
                                                            if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                                {
                                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                    if (!string.IsNullOrEmpty(min))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + "( Giảm)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + "( Giảm)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                            {
                                                                if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                                {
                                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                    if (!string.IsNullOrEmpty(max))
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ( Tăng)";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " (Tăng)";
                                                                    }
                                                                    else
                                                                    {
                                                                        dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                        dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dvObject.KetQuaDichVu += ketQua.ToString() + " ";
                                                                    dvObject.KetQuaDichVuDefault += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                        }
                                                        if (KieuSo == false)
                                                        {
                                                            dvObject.KetQuaDichVu += value + "";
                                                            dvObject.KetQuaDichVuDefault += value + "";
                                                        }

                                                        if (itemCongKyTu < itemDv.DataKetQuaCanLamSangVo.KetQuaXetNghiemChiTiets.Count())
                                                        {
                                                            dvObject.KetQuaDichVu += "; ";
                                                            dvObject.KetQuaDichVuDefault += "; ";
                                                        }
                                                    }
                                                    itemCongKyTu++;
                                                }
                                            }

                                            if (dvObject.KetQuaDichVu == "" || dvObject.KetQuaDichVu == "0")
                                            {
                                                dvObject.KetQuaDichVu = "";
                                                dvObject.KetQuaDichVuDefault = "";
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(dvObject.KetQuaDichVu))
                                {
                                    dvObject.KetQuaDichVu = dvObject.KetQuaDichVu.Split(";").Where(d => d != "" && d != " ").ToList().Distinct().Join(";");
                                }
                                if (!string.IsNullOrEmpty(dvObject.KetQuaDichVuDefault))
                                {
                                    dvObject.KetQuaDichVuDefault = dvObject.KetQuaDichVuDefault.Split(";").Where(d => d != "" && d != " ").ToList().Distinct().Join(";");
                                }
                                listDichVu.Add(dvObject);
                            }
                        }

                        // BVHD-3668 -> lấy những dịch vụ kỹ thuật khác cls -> kết quả để null
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats.Any())
                        {
                            foreach (var itemDVKTKhacCLS in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuats.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDVKTKhacCLS.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                //dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThaiDVKham;
                                dvObject.KetQuaDichVu = string.Empty; // để tự nhập
                                dvObject.KetQuaDichVuDefault = string.Empty; // để tự nhập
                                dvObject.ThoiDiemThucHien = itemDVKTKhacCLS.ThoiDiemThucHien;
                                listDichVu.Add(dvObject);
                            }
                        }
                        // BVHD-3877 ->thủ thuật phẩu thuật
                        if (thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats.Any())
                        {
                            foreach (var itemDVKTKhacCLS in thongTinNhanVienKham.ThongTinNhanVienKhamTheoYeuCauDichVuKyThuatThuThuatPhauThuats.ToList())
                            {
                                DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                                dvObject.Id = itemDVKTKhacCLS.Id;
                                dvObject.HopDongKhamSucKhoeNhanVienId = thongTinNhanVienKham.HopDongKhamSucKhoeNhanVienId;
                                dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                                dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                                dvObject.TenDichVu = itemDVKTKhacCLS.TenDichVuKyThuat;
                                dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                                //dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                                dvObject.GoiKhamSucKhoeId = itemDVKTKhacCLS.GoiKhamSucKhoeId;
                                dvObject.TrangThaiDVKham = (int)itemDVKTKhacCLS.TrangThaiDVKham;
                                dvObject.KetQuaDichVu = itemDVKTKhacCLS.KetQua;
                                dvObject.KetQuaDichVuDefault = itemDVKTKhacCLS.KetQua;
                                dvObject.ThoiDiemThucHien = itemDVKTKhacCLS.ThoiDiemThucHien;
                                listDichVu.Add(dvObject);
                            }
                        }

                        if (thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData != null)
                        {
                            // chạy những data cũ  chưa lưu người thực hiện , và thời điểm thực hiện trong josn
                            if (thongTinNhanVienKham.ThongTinNhanVienKhamLoaiLuuInKetQuaKSK == null)
                            {
                                listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData);
                                // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                                foreach (var itemxDvMoi in listDichVu)
                                {
                                    foreach (var dvcu in listDichVuCu)
                                    {
                                        if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                        {
                                            if (dvcu.KetQuaDaDuocLuu == true)
                                            {
                                                itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var jonKetLuan = JsonConvert.DeserializeObject<KetQuaKhamSucKhoeDaTa>(thongTinNhanVienKham.ThongTinNhanVienKhamKetQuaKhamSucKhoeData);
                                if (!string.IsNullOrEmpty(jonKetLuan.KetQuaKhamSucKhoe))
                                {
                                    listDichVuCu = JsonConvert.DeserializeObject<List<DanhSachDichVuKhamGrid>>(jonKetLuan.KetQuaKhamSucKhoe);
                                    // xử lý lấy những dịch vụ có trong json , field KetQuaDichVuDefault = json.KetQuaDichVuDefault , còn lại lấy từ dịch vụ kết luận mới nhất
                                    // data mới KetQuaDaDuocLuu = true, false
                                    if (listDichVuCu.Where(d => d.KetQuaDaDuocLuu != null).ToList().Count() != 0)
                                    {
                                        foreach (var itemxDvMoi in listDichVu)
                                        {
                                            foreach (var dvcu in listDichVuCu)
                                            {
                                                if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                                {
                                                    if (dvcu.KetQuaDaDuocLuu == true)
                                                    {
                                                        itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else // Trường hợp data cũ  KetQuaDaDuocLuu null
                                    {
                                        foreach (var itemxDvMoi in listDichVu)
                                        {
                                            foreach (var dvcu in listDichVuCu)
                                            {
                                                if (itemxDvMoi.Id == dvcu.Id && itemxDvMoi.NhomId == dvcu.NhomId)
                                                {
                                                    itemxDvMoi.KetQuaDichVu = dvcu.KetQuaDichVu;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    // xử lý list string trùng nhau (dịch vụ khám)
                    //List<DanhSachDichVuKhamGrid> listDichVuLoaiTrung = new List<DanhSachDichVuKhamGrid>();

                    foreach (var itemTrung in listDichVu.Where(s => s.NhomId != EnumNhomGoiDichVu.DichVuKyThuat).ToList())
                    {
                        if (!string.IsNullOrEmpty(itemTrung.KetQuaDichVu) && !string.IsNullOrEmpty(itemTrung.KetQuaDichVuDefault))
                        {
                            var catstring = itemTrung.KetQuaDichVu.Split('.');
                            var catstringdefault = itemTrung.KetQuaDichVuDefault.Split('.');
                            itemTrung.KetQuaDichVu = catstring.Where(d => d != null && d != "").Distinct().Join(".");
                            itemTrung.KetQuaDichVuDefault = catstringdefault.Where(d => d != null && d != "").Distinct().Join(".");

                        }
                    }
                }


            }

            return listDichVu.OrderBy(o => o.TenDichVu).ToList(); // trả về 1 list dịch vụ (dịch vụ khám , cls (dịch vụ kỹ thuật))
        }
        private bool IsInt(string sVal)
        {
            double test;
            return double.TryParse(sVal, out test);
        }
        public async Task<string> GetHTMLBaoCaoTongHopKetQuaKSK(List<BaoCaoTongHopKetQuaKhamDoanGridVo> tatCaDichVu, List<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo> listDichVuTheoNB)
        {
            var html = string.Empty;
            html += "<table style='width:100%' class='table table-border'>";
            #region title table orderby dịch vụ khám => dịch vụ kỹ thuật
            html += "<thead>";
            html += "<tr>";
            html += "<th class='stt'>STT</th>";
            html += "<th >MÃ NB</th>";
            html += "<th >MÃ TN</th>";
            html += "<th >HỌ TÊN</th>";
            html += "<th class='namsinh'>NĂM SINH</th>";
            html += "<th class='gioiTinh'>GIỚI TÍNH</th>";
            html += "<th class='chieucao'>CHIỀU CAO</th>";
            html += "<th class='cannang'>CÂN NẶNG</th>";
            html += "<th class='huyetap'>HUYẾT ÁP</th>";
            foreach (var item in tatCaDichVu.OrderBy(d => d.NhomId).ToList())
            {
                html += "<th class='dichvu'>" + item.TenDichVu.ToUpper() + "</th>";
            }
            html += "<th class='phanloai'>PHÂN LOẠI</th>";
            html += "<th class='ketluan'>KẾT LUẬN</th>";
            html += "<th class='denghi'>ĐỀ NGHỊ</th>";
            html += "</tr>";
            html += "</thead>";
            #endregion
            #region bind value
            html += "<tbody>";
            int stt = 1;
            foreach (var dv in listDichVuTheoNB.OrderBy(d => d.STT))
            {
                html += "<tr>";
                html += "<td>" + (dv.STT != null ? dv.STT + "" : "") + "</td>";
                html += "<td>" + dv.MaNB + "</td>";
                html += "<td>" + dv.MaTN + "</td>";
                html += "<td>" + dv.HoTen + "</td>";
                html += "<td>" + dv.NamSinh + "</td>";
                html += "<td>" + dv.GioiTinh + "</td>";
                html += "<td>" + dv.ChieuCao + "</td>";
                html += "<td>" + dv.CanNang + "</td>";
                html += "<td>" + dv.HuyetAp + "</td>";
                foreach (var item in tatCaDichVu.OrderBy(d => d.NhomId).ToList())
                {
                    var dichVu = dv.BaoCaoTongHopKetQuaKhamDoanGridVos.Where(d => d.NhomId == item.NhomId && item.ListDichVuIds.Contains(d.IdDichVuTrongGoi));
                    if (dichVu.Any())
                    {
                        var ketQua = dv.BaoCaoTongHopKetQuaKhamDoanGridVos.Where(d => d.NhomId == item.NhomId  && item.ListDichVuIds.Contains(d.IdDichVuTrongGoi)).Select(d => d.KetQua).FirstOrDefault();
                        html += "<td>" + ketQua + "</td>";
                    }
                    else
                    {
                        html += "<td>" + "&nbsp;&nbsp;" + "</td>";
                    }
                }
                html += "<td>" + dv.PhanLoai + "</td>";
                html += "<td>" + dv.KetLuan + "</td>";
                html += "<td>" + dv.DeNghi + "</td>";
                html += "</tr>";
                stt++;
            }
            html += "</tbody>";
            #endregion
            html += "</table>";
            return html;
        }
        #endregion BÁO CÁO TỔNG HỢP KẾT QUẢ KHÁM SỨC KHỎE
        #region   báo cáo dự trù số lượng người thực hiện dv ls -cls 22/10/2021
        public async Task<GridDataSource> ListDichVuBenhNhanDangKy(BaoCaoTongHopKetQuaKhamDoanQueryInfoQueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var grid = new List<BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo>();

            var hopDong = _hopDongKhamSucKhoeRepository.GetById((long)queryInfo.HopDongId, o =>
            o.Include(x => x.GoiKhamSucKhoes).ThenInclude(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(b => b.NhomDichVuBenhVien)
            .Include(x => x.GoiKhamSucKhoes).ThenInclude(x => x.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(x => x.DichVuKhamBenhBenhVien)
            .Include(x => x.HopDongKhamSucKhoeNhanViens)
            );
            var hopDongDongKhamSucKhoeNhanVien =
                 _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.Where(x =>
                    x.Id == queryInfo.HopDongId).ToList();
            var listNhanVienTheoHopDong = new List<TiepNhanDichVuChiDinhKhamDoanQueryVo>();

            if (hopDong.HopDongKhamSucKhoeNhanViens.Any())
            {
                foreach (var itemNV in hopDong.HopDongKhamSucKhoeNhanViens)
                {
                    var newItem = new TiepNhanDichVuChiDinhKhamDoanQueryVo();
                    newItem.HopDongKhamSucKhoeNhanVienId = itemNV.Id;
                    if ((itemNV.NamSinh != null && itemNV.NamSinh != 0) && (itemNV.ThangSinh != null && itemNV.ThangSinh != 0) && (itemNV.NgaySinh != null && itemNV.NgaySinh != 0))
                    {
                        newItem.NgayThangNamSinh = new DateTime((int)itemNV.NamSinh, (int)itemNV.ThangSinh, (int)itemNV.NgaySinh);
                    }
                    newItem.NamSinh = itemNV.NamSinh;
                    newItem.GioiTinh = itemNV.GioiTinh;
                    newItem.TinhTrangHonNhan = itemNV.DaLapGiaDinh ? TinhTrangHonNhan.CoGiaDinh : TinhTrangHonNhan.ChuaCoGiaDinh;
                    newItem.CoMangThai = itemNV.CoMangThai;
                    newItem.GoiKhamSucKhoeId = itemNV.GoiKhamSucKhoeId;
                    listNhanVienTheoHopDong.Add(newItem);
                }
            }

            var listGoiKhamSucKhoe = hopDong.GoiKhamSucKhoes.Select(d => d.Id).ToList();

            #region // xử lý gói khám chung
            var hopDongNhanVienSuDungGoiChungs = _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository.TableNoTracking.Where(o => o.HopDongKhamSucKhoeNhanVien != null && o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == (long)queryInfo.HopDongId)
                                                 .Select(item => new BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo()
                                                 {
                                                     NhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                                                     TenDV = item.DichVuKhamBenhBenhVien.Ten,
                                                     GioiTinhNam = item.HopDongKhamSucKhoeNhanVien.GioiTinh == LoaiGioiTinh.GioiTinhNam ? true : false,
                                                     GioiTinhNu = item.HopDongKhamSucKhoeNhanVien.GioiTinh == LoaiGioiTinh.GioiTinhNu ? true : false,
                                                     //Id = item.Id
                                                 })
                                                 .Union(_goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository.TableNoTracking.Where(o => o.HopDongKhamSucKhoeNhanVien != null && o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == (long)queryInfo.HopDongId)
                                                 .Select(item => new BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo()
                                                 {
                                                     NhomDichVu = (string.IsNullOrEmpty(item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten) ? "" : item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten + " - ") + item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                                                     TenDV = item.DichVuKyThuatBenhVien.Ten,
                                                     GioiTinhNam = item.HopDongKhamSucKhoeNhanVien.GioiTinh == LoaiGioiTinh.GioiTinhNam ? true : false,
                                                     GioiTinhNu = item.HopDongKhamSucKhoeNhanVien.GioiTinh == LoaiGioiTinh.GioiTinhNu ? true : false,
                                                     //Id = item.Id
                                                 })).ToList();

            grid.AddRange(hopDongNhanVienSuDungGoiChungs); // hợp đồng nhân viên đang dùng gòi chung

            #endregion

            //var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            if (listGoiKhamSucKhoe.Any())
            {
                var queryDVGoiKham = await _goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                     .Where(x => listGoiKhamSucKhoe.Contains(x.GoiKhamSucKhoeId))
                     .Select(item => new
                     {
                         NhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                         TenDV = item.DichVuKhamBenhBenhVien.Ten,
                         GioiTinhNam = item.GioiTinhNam,
                         GioiTinhNu = item.GioiTinhNu,
                         CoMangThai = item.CoMangThai,
                         KhongMangThai = item.KhongMangThai,
                         DaLapGiaDinh = item.DaLapGiaDinh,
                         ChuaLapGiaDinh = item.ChuaLapGiaDinh,
                         SoTuoiTu = item.SoTuoiTu,
                         SoTuoiDen = item.SoTuoiDen,
                         GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                         //Id= item.Id
                     }).Union(_goiKhamSucKhoeDichVuDichVuKyThuatRepository.TableNoTracking
                         .Where(x => listGoiKhamSucKhoe.Contains(x.GoiKhamSucKhoeId)
                             )
                         .Select(item => new
                         {
                             NhomDichVu = (string.IsNullOrEmpty(item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten) ? "" : item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten + " - ") + item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                             TenDV = item.DichVuKyThuatBenhVien.Ten,
                             GioiTinhNam = item.GioiTinhNam,
                             GioiTinhNu = item.GioiTinhNu,
                             CoMangThai = item.CoMangThai,
                             KhongMangThai = item.KhongMangThai,
                             DaLapGiaDinh = item.DaLapGiaDinh,
                             ChuaLapGiaDinh = item.ChuaLapGiaDinh,
                             SoTuoiTu = item.SoTuoiTu,
                             SoTuoiDen = item.SoTuoiDen,
                             GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                             //Id = item.Id
                         }))
                     .OrderBy(x => x.NhomDichVu)
                     .ToListAsync();
                var gridDVNhanVienThucHien = new List<BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo>();
                if (queryDVGoiKham.Any())
                {
                    #region // list nhân viên sử dụng gói
                    foreach (var itemNV in listNhanVienTheoHopDong)
                    {
                        var query = queryDVGoiKham
                                 .Where(x => x.GoiKhamSucKhoeId == itemNV.GoiKhamSucKhoeId
                                 && ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && itemNV.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && itemNV.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                                       && ((!x.CoMangThai && !x.KhongMangThai) || itemNV.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && itemNV.CoMangThai) || (x.KhongMangThai && !itemNV.CoMangThai))
                                                       && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !itemNV.DaLapGiaDinh) || (x.DaLapGiaDinh && itemNV.DaLapGiaDinh))
                                                       && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (itemNV.Tuoi != null && ((x.SoTuoiTu == null || itemNV.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || itemNV.Tuoi <= x.SoTuoiDen))))

                                 )
                                 .Select(item => new BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo()
                                 {
                                     NhomDichVu = item.NhomDichVu,
                                     TenDV = item.TenDV,
                                     GioiTinhNam = itemNV.GioiTinh == LoaiGioiTinh.GioiTinhNam ? true : false,
                                     GioiTinhNu = itemNV.GioiTinh == LoaiGioiTinh.GioiTinhNu ? true : false,
                                     //Id = item.Id
                                 })
                                 .OrderBy(x => x.TenNhomDichVu)
                                 .ToList();
                        grid.AddRange(query);
                    }
                    #endregion

                }
                #region // get những dịch vụ không thực hiện tỏng gói
                //var dvIds = grid.Select(d => d.Id).ToList();
                var dvTenDichVus = grid.Select(d => d.TenDV).ToList();
                if (grid.Any())
                {
                    var dvNhanVienKhongThucHien = queryDVGoiKham.Where(d => !dvTenDichVus.Contains(d.TenDV)).Select(item => new BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo()
                    {
                        NhomDichVu = item.NhomDichVu,
                        TenDV = item.TenDV,
                        GioiTinhNam = null,
                        GioiTinhNu = null,
                        //Id = item.Id
                    })
                                 .OrderBy(x => x.TenNhomDichVu)
                                 .ToList();
                    if (dvNhanVienKhongThucHien.Any())
                    {
                        grid.AddRange(dvNhanVienKhongThucHien);
                    }
                }
                #endregion
            }








            var queryGroupDVNhom = grid.GroupBy(x => new
            {
                x.TenDV,
                x.NhomDichVu
            }).Select(d => new BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo
            {
                TenDV = d.First().TenDV,
                NhomDichVu = d.First().NhomDichVu,
                SLNam = d.Where(g => g.GioiTinhNam == true).Count(),
                SLNu = d.Where(g => g.GioiTinhNu == true).Count()
            });


            var dv = queryGroupDVNhom.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : queryGroupDVNhom.Count();
            return new GridDataSource { Data = dv, TotalRowCount = countTask };
        }
        public virtual byte[] ExportBaoCaoDuTruSLNguoiThucHienDichVu(IList<BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo> list)
        {

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo>("STT", p => ind++)
            };
            var listGroup = list.GroupBy(d => d.NhomDichVu).ToList();


            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO DỰ TRÙ SỐ LƯỢNG NGƯỜI THỰC HIỆN DV LS-CLS");

                    // set row

                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 50;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;
                    worksheet.Column(16).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E" };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;

                    var worksheetTitleHeader = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 4;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "BÁO CÁO DỰ TRÙ SỐ LƯỢNG NGƯỜI THỰC HIỆN DV LS-CLS".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }


                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        //range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        //range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        //range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        //range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;
                        //range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        //range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns = { { "A", "STT" }, { "B", "Tên DV" }, { "C", "Nam" }, { "D", "Nữ" }, { "E", "Tổng" } };

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 2 + ":" + (SetColumns[i, 0]).ToString() + 4).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo>(requestProperties);
                    int index = 5;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var dt in listGroup)
                    {
                        if (dt.Any())
                        {
                            using (var range = worksheet.Cells["A" + index + ":E" + index])
                            {

                                range.Worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;
                                //range.Worksheet.Cells["A" + index + ":E" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);


                                string[,] SetColumnLoai = { { "A", dt.Select(d => d.NhomDichVu).FirstOrDefault() } };

                                for (int i = 0; i < SetColumnLoai.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLoai[i, 0]).ToString() + index + ":" + (SetColumnLoai[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLoai[i, 1];
                                }
                                index++;
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            var indexMain = index;
                            int stt = 1;
                            foreach (var item in dt)
                            {

                                manager.CurrentObject = item;
                                manager.WriteToXlsx(worksheet, index);
                                worksheet.Cells["A" + index].Value = stt;
                                worksheet.Cells["B" + index].Value = item.TenDV;
                                worksheet.Cells["C" + index].Value = item.SLNam;
                                worksheet.Cells["D" + index].Value = item.SLNu;
                                worksheet.Cells["E" + index].Value = item.Tong;
                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                    //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                                }
                                stt++;
                                index++;
                            }
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }

        }
        #endregion  báo cáo dự trù số lượng người thực hiện dv ls -cls 22/10/2021
        #region  Báo cáo dịch vu ngoài gói 2/11/2021 
        public string GetTenCongTy(long id)
        {
            var ten = _hopDongKhamSucKhoeRepository.TableNoTracking.Where(d => d.Id == id).Select(d => d.CongTyKhamSucKhoe.Ten);
            return ten.FirstOrDefault();
        }
        public async Task<List<BaoCaoDVNgoaiGoiKeToanGridVo>> GetAllForBaoCaoBenhNhanKhamDicVuNgoaiGoi(BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;
            var hopDongKhamSucKhoeId = queryInfo.HopDongId.GetValueOrDefault();
            var congTyId = queryInfo.CongTyId.GetValueOrDefault();

            var phieuThuDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId &&
                (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) &&
                (queryInfo.TuNgay == null || tuNgay <= o.NgayThu) &&
                (queryInfo.DenNgay == null || o.NgayThu < denNgay)
                );

            var phieuHuyHoanDataQuery = _taiKhoanBenhNhanThuRepository.TableNoTracking.Where(o => o.LoaiNoiThu == Enums.LoaiNoiThu.ThuNgan && o.DaHuy == true && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId &&
                (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng || o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo) &&
                (queryInfo.TuNgay == null || tuNgay <= o.NgayHuy) &&
                (queryInfo.DenNgay == null || o.NgayHuy < denNgay)
                );

            var phieuChiDataQuery = _taiKhoanBenhNhanChiRepository.TableNoTracking.Where(o => (o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) &&
                o.YeuCauTiepNhan != null && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId &&
                (queryInfo.TuNgay == null || tuNgay <= o.NgayChi) &&
                (queryInfo.DenNgay == null || o.NgayChi < denNgay)
                );

            var phieuHuyChiDataQuery = _taiKhoanBenhNhanChiRepository.TableNoTracking.Where(o => o.DaHuy == true &&
                (o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanUng || o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu) &&
                o.YeuCauTiepNhan != null && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongKhamSucKhoeId &&
                (queryInfo.TuNgay == null || tuNgay <= o.NgayHuy) &&
                (queryInfo.DenNgay == null || o.NgayHuy < denNgay)
                );



            //// dịch vụ khám ngoài gói khám đoàn chỉ có dịch vụ kỹ thuật
            //// lấy tất cả dịch vụ theo hớp đồng 
            //var yeuCauDichVuKyThuatEntitys = _yeuCauTiepNhanRepository.TableNoTracking
            //                                                    .Where(p => (p.HopDongKhamSucKhoeNhanVien != null && (p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryInfo.CongTyId
            //                                                                           && p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryInfo.HopDongId
            //                                                                           && p.BenhNhanId != null)))
            //                                                    .SelectMany(d => d.YeuCauDichVuKyThuats).Include(d=>d.DichVuKyThuatBenhVien).ThenInclude(d=>d.DichVuKyThuatVuBenhVienGiaBenhViens);

            //// lấy tất cả dịch vụ ngoài gói của công ty hơp đồng
            //var ycdvkts = yeuCauDichVuKyThuatEntitys.Where(d => d.GoiKhamSucKhoeId == null)
            //                                        .Select(d => d.Id).ToList();

            //// ycdvkts info đơn giá bv , TenDichVu, Dơn giá mới 
            //var ycdvktInfo = yeuCauDichVuKyThuatEntitys.Select(d => new DichVuKyThuatInFo
            //{
            //      Id = d.Id,
            //      TenDichVu = d.TenDichVu,
            //      DonGiaBenhVien = d.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p => p.NhomGiaDichVuKyThuatBenhVienId == d.NhomGiaDichVuKyThuatBenhVienId &&
            //                                                                                                   p.DichVuKyThuatBenhVienId == d.DichVuKyThuatBenhVienId && (p.DenNgay == null || p.DenNgay >= DateTime.Now) )
            //                                                                                  .Select(p => p.Gia).LastOrDefault(), // lấy đơn giá first giá còn hiều lực
            //      DonGiaMoi = d.DonGiaUuDai, // đơn giá mới => đơn giá ưu đãi
            //});








            var phieuThu = phieuThuDataQuery.Select(o => new BaoCaoDVNgoaiGoiKeToanGridVo
            {
                Id = o.NhanVienThucHienId,
                LoaiThuTienBenhNhan = o.LoaiThuTienBenhNhan,
                LaPhieuHuy = false,
                //
                NgayBienLai = o.NgayThu,
                SoBienLai = o.SoPhieuHienThi,
                MaNhanVien = o.NhanVienThucHien.User.HoTen,
                MaNguoiBenh = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh != null ? o.YeuCauTiepNhan.GioiTinh.Value.GetDescription() : "",


                NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,

                GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,

                TongChiPhiBNTT = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                //ChiTietBHYTs = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => new ChiTietBHYT { SoLuong = chi.SoLuong, DonGiaBaoHiem = chi.DonGiaBaoHiem, MucHuongBaoHiem = chi.MucHuongBaoHiem, TiLeBaoHiemThanhToan = chi.TiLeBaoHiemThanhToan } ).ToList(),
                CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                CongNoCaNhan = o.CongNo.GetValueOrDefault(),
                SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                SoTienThuChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                SoTienThuPos = o.POS.GetValueOrDefault(),
                SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,

                ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.CongTyBaoHiemTuNhan.Ten).ToList(),
                DataPhieuChis = o.TaiKhoanBenhNhanChis
                    .Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && chi.SoLuong != 0)
                    .Select(chi => new BaoCaoDVNgoaiGoiKeDataPhieuChi
                    {
                        Id = chi.Id,
                        NgayChi = chi.NgayChi,
                        TienChiPhi = chi.TienChiPhi,
                        SoTienBaoHiemTuNhanChiTra = chi.SoTienBaoHiemTuNhanChiTra,
                        SoTienMienGiam = chi.SoTienMienGiam,
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        YeuCauKhamBenhId = chi.YeuCauKhamBenhId,
                        YeuCauDichVuKyThuatId = chi.YeuCauDichVuKyThuatId,
                        YeuCauDuocPhamBenhVienId = chi.YeuCauDuocPhamBenhVienId,
                        YeuCauVatTuBenhVienId = chi.YeuCauVatTuBenhVienId,
                        DonThuocThanhToanChiTietId = chi.DonThuocThanhToanChiTietId,
                        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = chi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                        YeuCauTruyenMauId = chi.YeuCauTruyenMauId,
                    }).ToList(),
            }).ToList();
            var phieuHuyHoan = phieuHuyHoanDataQuery.Select(o => new BaoCaoDVNgoaiGoiKeToanGridVo
            {
                Id = o.NhanVienHuyId.GetValueOrDefault(),
                LoaiThuTienBenhNhan = o.LoaiThuTienBenhNhan,
                LaPhieuHuy = true,
                //
                NgayBienLai = o.NgayHuy ?? o.NgayThu,
                SoBienLai = o.SoPhieuHienThi,
                MaNhanVien = o.NhanVienThucHien.User.HoTen,
                MaNguoiBenh = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh != null ? o.YeuCauTiepNhan.GioiTinh.Value.GetDescription() : "",


                NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                //SoBenhAn = o.YeuCauTiepNhan.NoiTruBenhAn != null ? o.YeuCauTiepNhan.NoiTruBenhAn.SoBenhAn : string.Empty,
                //BenhAnSoSinh = o.YeuCauTiepNhan.YeuCauNhapVien != null ? (o.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMeId != null) : false,
                GoiDichVu = o.ThuTienGoiDichVu != null && o.ThuTienGoiDichVu == true,

                TongChiPhiBNTT = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => chi.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum(),
                //ChiTietBHYTs = o.TaiKhoanBenhNhanChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => new ChiTietBHYT { SoLuong = chi.SoLuong, DonGiaBaoHiem = chi.DonGiaBaoHiem, MucHuongBaoHiem = chi.MucHuongBaoHiem, TiLeBaoHiemThanhToan = chi.TiLeBaoHiemThanhToan }).ToList(),
                CongNoTuNhan = o.CongTyBaoHiemTuNhanCongNos.Select(cn => cn.SoTien).DefaultIfEmpty().Sum(),
                CongNoCaNhan = o.CongNo.GetValueOrDefault(),
                SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                SoTienThuChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                SoTienThuPos = o.POS.GetValueOrDefault(),
                SoPhieuThuGhiNo = o.ThuNoPhieuThu != null ? o.ThuNoPhieuThu.SoPhieuHienThi : string.Empty,
                //NguoiThu = o.NhanVienHuy != null ? o.NhanVienHuy.User.HoTen : string.Empty,
                ChiTietCongNoTuNhans = o.CongTyBaoHiemTuNhanCongNos.Select(cntn => cntn.CongTyBaoHiemTuNhan.Ten).ToList(),
                DataPhieuChis = o.TaiKhoanBenhNhanChis
                    .Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi && chi.SoLuong != 0)
                    .Select(chi => new BaoCaoDVNgoaiGoiKeDataPhieuChi
                    {
                        Id = chi.Id,
                        NgayChi = chi.NgayChi,
                        TienChiPhi = chi.TienChiPhi,
                        SoTienBaoHiemTuNhanChiTra = chi.SoTienBaoHiemTuNhanChiTra,
                        SoTienMienGiam = chi.SoTienMienGiam,
                        TienMat = 0,
                        ChuyenKhoan = 0,
                        YeuCauKhamBenhId = chi.YeuCauKhamBenhId,
                        YeuCauDichVuKyThuatId = chi.YeuCauDichVuKyThuatId,
                        YeuCauDuocPhamBenhVienId = chi.YeuCauDuocPhamBenhVienId,
                        YeuCauVatTuBenhVienId = chi.YeuCauVatTuBenhVienId,
                        DonThuocThanhToanChiTietId = chi.DonThuocThanhToanChiTietId,
                        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = chi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                        YeuCauTruyenMauId = chi.YeuCauTruyenMauId,
                    }).ToList(),
            }).ToList();

            var phieuChi = phieuChiDataQuery.Select(o => new BaoCaoDVNgoaiGoiKeToanGridVo
            {
                Id = o.NhanVienThucHienId.GetValueOrDefault(),
                LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                LaPhieuHuy = false,
                //
                NgayBienLai = o.NgayChi,
                SoBienLai = o.SoPhieuHienThi,
                MaNhanVien = o.NhanVienThucHien.User.HoTen,
                MaNguoiBenh = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh != null ? o.YeuCauTiepNhan.GioiTinh.Value.GetDescription() : "",

                NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                GoiDichVu = o.YeuCauGoiDichVuId != null,
                SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
                DataPhieuChis = new List<BaoCaoDVNgoaiGoiKeDataPhieuChi>(){ new BaoCaoDVNgoaiGoiKeDataPhieuChi
                {
                    Id = o.Id,
                    NgayChi = o.NgayChi,
                    TienChiPhi = o.PhieuThanhToanChiPhi != null ? o.PhieuThanhToanChiPhi.TienChiPhi : 0,
                    SoTienBaoHiemTuNhanChiTra = o.PhieuThanhToanChiPhi != null ? o.PhieuThanhToanChiPhi.SoTienBaoHiemTuNhanChiTra : 0,
                    SoTienMienGiam = o.PhieuThanhToanChiPhi != null ? o.PhieuThanhToanChiPhi.SoTienMienGiam : 0,
                    TienMat = 0,
                    ChuyenKhoan = 0,
                    YeuCauKhamBenhId = o.YeuCauKhamBenhId,
                    YeuCauDichVuKyThuatId = o.YeuCauDichVuKyThuatId,
                    YeuCauDuocPhamBenhVienId = o.YeuCauDuocPhamBenhVienId,
                    YeuCauVatTuBenhVienId = o.YeuCauVatTuBenhVienId,
                    DonThuocThanhToanChiTietId = o.DonThuocThanhToanChiTietId,
                    YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                    YeuCauTruyenMauId = o.YeuCauTruyenMauId,
                } },

            }).ToList();
            var phieuHuyChi = phieuHuyChiDataQuery.Select(o => new BaoCaoDVNgoaiGoiKeToanGridVo
            {
                Id = o.NhanVienHuyId.GetValueOrDefault(),
                LoaiChiTienBenhNhan = o.LoaiChiTienBenhNhan,
                LaPhieuHuy = true,
                //
                NgayBienLai = o.NgayHuy ?? o.NgayChi,
                SoBienLai = o.SoPhieuHienThi,
                MaNhanVien = o.NhanVienThucHien.User.HoTen,
                MaNguoiBenh = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTiepNhan = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = o.YeuCauTiepNhan.HoTen,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh != null ? o.YeuCauTiepNhan.GioiTinh.Value.GetDescription() : "",

                NamSinh = o.YeuCauTiepNhan.NamSinh != null ? o.YeuCauTiepNhan.NamSinh.ToString() : string.Empty,
                NguoiGioiThieu = o.YeuCauTiepNhan.NoiGioiThieu != null ? o.YeuCauTiepNhan.NoiGioiThieu.Ten : string.Empty,
                GoiDichVu = o.YeuCauGoiDichVuId != null,
                SoTienThuTienMat = o.TienMat.GetValueOrDefault(),
            }).ToList();

            var allData = new List<BaoCaoDVNgoaiGoiKeToanGridVo>();
            allData.AddRange(phieuThu);
            allData.AddRange(phieuHuyHoan);
            allData.AddRange(phieuChi);
            allData.AddRange(phieuHuyChi);

            var dichVuKhamBenhIds = allData.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauKhamBenhId != null).Select(o => o.YeuCauKhamBenhId).Distinct().ToList();
            var dichVuKyThuatIds = allData.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDichVuKyThuatId != null).Select(o => o.YeuCauDichVuKyThuatId).Distinct().ToList();
            var yeuCauDuocPhamIds = allData.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauDuocPhamBenhVienId != null).Select(o => o.YeuCauDuocPhamBenhVienId).Distinct().ToList();
            var yeuCauVatTuIds = allData.SelectMany(o => o.DataPhieuChis).Where(o => o.YeuCauVatTuBenhVienId != null).Select(o => o.YeuCauVatTuBenhVienId).Distinct().ToList();

            var dataDichVuKhamBenh = _yeuCauKhamBenhRepository.TableNoTracking.Where(o => dichVuKhamBenhIds.Contains(o.Id))
                .Select(o => new BaoCaoDVNgoaiGoiDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    DichVuBenhVienId = o.DichVuKhamBenhBenhVienId,
                    DichVuNhomGiaId = o.NhomGiaDichVuKhamBenhBenhVienId,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    YeuCauKhamBenh = true,
                    NoiThucHienId = o.NoiThucHienId ?? o.NoiDangKyId,
                    NoiChiDinhId = o.NoiChiDinhId,
                    Gia = o.Gia,
                    DonGiaUuDai = o.DonGiaUuDai,
                    DonGiaChuaUuDai = o.DonGiaChuaUuDai
                }).ToList();
            var dataDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(o => dichVuKyThuatIds.Contains(o.Id))
                .Select(o => new BaoCaoDVNgoaiGoiDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.TenDichVu,
                    DichVuBenhVienId = o.DichVuKyThuatBenhVienId,
                    DichVuNhomGiaId = o.NhomGiaDichVuKyThuatBenhVienId,
                    ThoiDiemChiDinh = o.ThoiDiemChiDinh,
                    YeuCauDichVuKyThuat = true,
                    LoaiDichVuKyThuat = o.LoaiDichVuKyThuat,
                    NhomDichVuBenhVienId = o.NhomDichVuBenhVienId,
                    Gia = o.Gia,
                    DonGiaUuDai = o.DonGiaUuDai,
                    DonGiaChuaUuDai = o.DonGiaChuaUuDai
                }).ToList();

            var dataYeuCauDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(o => yeuCauDuocPhamIds.Contains(o.Id))
                .Select(o => new BaoCaoDVNgoaiGoiDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    YeuCauDuocPham = true,
                    Gia = o.DonGiaBan
                }).ToList();
            var dataYeuCauVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(o => yeuCauVatTuIds.Contains(o.Id))
                .Select(o => new BaoCaoDVNgoaiGoiDataDichVu
                {
                    Id = o.Id,
                    TenDichVu = o.Ten,
                    YeuCauVatTu = true,
                    Gia = o.DonGiaBan
                }).ToList();

            var dichVuKhamBenhBenhVienIds = dataDichVuKhamBenh.Select(o => o.DichVuBenhVienId).Distinct().ToList();
            var dichVuKyThuatBenhVienIds = dataDichVuKyThuat.Select(o => o.DichVuBenhVienId).Distinct().ToList();

            var dichVuKhamBenhBenhVienGiaBenhViens = _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => dichVuKhamBenhBenhVienIds.Contains(o.DichVuKhamBenhBenhVienId)).ToList();
            var dichVuKyThuatBenhVienGiaBenhViens = _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                .Where(o => dichVuKyThuatBenhVienIds.Contains(o.DichVuKyThuatBenhVienId)).ToList();

            foreach (var dataPhieuThu in allData)
            {
                dataPhieuThu.TenDichVus = new List<DichVuKyThuatInFo>();
                if (dataPhieuThu.DataPhieuChis == null || dataPhieuThu.DataPhieuChis.Count == 0)
                {
                    dataPhieuThu.TenDichVus.Add(new DichVuKyThuatInFo());
                }
                else
                {
                    foreach (var dataPhieuChi in dataPhieuThu.DataPhieuChis)
                    {
                        var dichVuKyThuatInFo = new DichVuKyThuatInFo
                        {
                            Id = dataPhieuChi.Id,
                        };
                        decimal giaBenhVien = 0;
                        BaoCaoDVNgoaiGoiDataDichVu thongTinDichVu = null;
                        if (dataPhieuChi.YeuCauKhamBenhId != null)
                        {
                            thongTinDichVu = dataDichVuKhamBenh.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauKhamBenhId);
                            giaBenhVien = dichVuKhamBenhBenhVienGiaBenhViens.LastOrDefault(p =>
                                p.NhomGiaDichVuKhamBenhBenhVienId == thongTinDichVu.DichVuNhomGiaId &&
                                p.DichVuKhamBenhBenhVienId == thongTinDichVu.DichVuBenhVienId &&
                                p.TuNgay.Date <= thongTinDichVu.ThoiDiemChiDinh &&
                                (p.DenNgay == null || p.DenNgay >= thongTinDichVu.ThoiDiemChiDinh))?.Gia ?? 0;
                        }
                        else if (dataPhieuChi.YeuCauDichVuKyThuatId != null)
                        {
                            thongTinDichVu = dataDichVuKyThuat.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDichVuKyThuatId);
                            giaBenhVien = dichVuKyThuatBenhVienGiaBenhViens.LastOrDefault(p =>
                                p.NhomGiaDichVuKyThuatBenhVienId == thongTinDichVu.DichVuNhomGiaId &&
                                p.DichVuKyThuatBenhVienId == thongTinDichVu.DichVuBenhVienId &&
                                p.TuNgay.Date <= thongTinDichVu.ThoiDiemChiDinh &&
                                (p.DenNgay == null || p.DenNgay >= thongTinDichVu.ThoiDiemChiDinh))?.Gia ?? 0;
                        }
                        else if (dataPhieuChi.YeuCauDuocPhamBenhVienId != null)
                        {
                            thongTinDichVu = dataYeuCauDuocPham.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauDuocPhamBenhVienId);
                            giaBenhVien = thongTinDichVu.Gia;
                        }
                        else if (dataPhieuChi.YeuCauVatTuBenhVienId != null)
                        {
                            thongTinDichVu = dataYeuCauVatTu.FirstOrDefault(o => o.Id == dataPhieuChi.YeuCauVatTuBenhVienId);
                            giaBenhVien = thongTinDichVu.Gia;
                        }

                        var thucThu = ((dataPhieuThu.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.HoanThu || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * (dataPhieuChi.TienChiPhi.GetValueOrDefault() + dataPhieuChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault());
                        if (thongTinDichVu != null)
                        {
                            dichVuKyThuatInFo.TenDichVu = thongTinDichVu.TenDichVu;
                            dichVuKyThuatInFo.DonGiaBenhVien = giaBenhVien;
                            dichVuKyThuatInFo.DonGiaMoi = thongTinDichVu.Gia;
                            dichVuKyThuatInFo.SoTienDuocMienGiam = ((dataPhieuThu.LoaiChiTienBenhNhan == LoaiChiTienBenhNhan.HoanThu || dataPhieuThu.LaPhieuHuy) ? (-1) : 1) * dataPhieuChi.SoTienMienGiam.GetValueOrDefault();
                            dichVuKyThuatInFo.SoTienThucThu = thucThu;
                        }
                        dataPhieuThu.TenDichVus.Add(dichVuKyThuatInFo);
                    }
                }
            }

            return allData;
        }

        public virtual byte[] ExportBaoCaoChiTietThuTienDichVuNgoaiGoi(IList<BaoCaoDVNgoaiGoiKeToanGridVo> baoCaoThuPhiVienPhiGridVos, BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo, string tenCongTy, TotalBaoCaoThuPhiVienPhiGridVo datatotal)
        {
            var tuNgay = queryInfo.TuNgay == DateTime.MinValue ? DateTime.Now : queryInfo.TuNgay;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var dataBaoCaos = baoCaoThuPhiVienPhiGridVos.ToList();

            int ind = 1;

            var requestProperties = new[]
            {
                //new PropertyByName<BaoCaoThuPhiVienPhiGridVo>("STT",0),
                new PropertyByName<BaoCaoThuPhiVienPhiGridVo>("Thu Ngân", p => p.NguoiThu)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BC05");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // set column
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;

                    worksheet.Column(17).Width = 15;
                    worksheet.Column(18).Width = 15;
                    worksheet.Column(19).Width = 15;
                    worksheet.Column(20).Width = 15;
                    worksheet.Column(21).Width = 15;
                    worksheet.Column(22).Width = 15;
                    worksheet.Column(23).Width = 15;
                    worksheet.Column(24).Width = 39;
                    worksheet.Column(25).Width = 15;
                    worksheet.Column(26).Width = 15;
                    worksheet.Column(27).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA" };
                    var worksheetTitleBacHa = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleThuPhi = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleNgay = SetColumnItems[0] + 4 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 4;
                    var worksheetTitleQuay = SetColumnItems[0] + 5 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 5;
                    using (var range = worksheet.Cells[worksheetTitleThuPhi])
                    {
                        range.Worksheet.Cells[worksheetTitleBacHa].Merge = true;
                        range.Worksheet.Cells[worksheetTitleBacHa].Value = "";
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleBacHa].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells[worksheetTitleThuPhi])
                    {
                        range.Worksheet.Cells[worksheetTitleThuPhi].Merge = true;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Value = "DANH SÁCH DỊCH VỤ NGOÀI HỢP ĐỒNG".ToUpper() + " " + tenCongTy.ToUpper();
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + queryInfo.TuNgay?.FormatNgayGioTimKiemTrenBaoCao() + " đến ngày: " + queryInfo.DenNgay?.FormatNgayGioTimKiemTrenBaoCao();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        //range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleQuay])
                    {
                        range.Worksheet.Cells[worksheetTitleQuay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleQuay].Value = "";
                        range.Worksheet.Cells[worksheetTitleQuay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Bold = true;
                        //range.Worksheet.Cells[worksheetTitleQuay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A7:AA9"])
                    {
                        range.Worksheet.Cells["A7:AA9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:AA9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:AA9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:AA9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:AA9"].Style.Font.Bold = true;

                        //Set column A to I 
                        string[,] SetColumns = { { "A" , "STT" }, { "B", "Ngày biên lai" } , { "C", "Số biên lai" },
                           { "D", "Mã nhân viên" }, { "E", "Mã người bệnh" },  { "F", "Mã tiếp nhận" }, { "G", "Họ tên" }, { "H", "Giới tính" },
                           { "I", "NĂM SINH" },  { "J", "Dịch vụ" },
                           { "K", "Đơn giá BV" } , { "L", "Đơn giá mới" }, { "M", "Số tiền được miễn giảm" },
                           { "N", "Số tiền thực thu" },  { "O", "Tạm ứng " }, { "P", "Hoàn ứng" }, { "Q", "Huỷ/Hoàn" },
                           { "Z", "CHI TIẾT CÔNG NỢ" },{ "AA", "SỐ HÓA ĐƠN"} };

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 7 + ":" + (SetColumns[i, 0]).ToString() + 9).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        //Set column K to O 
                        range.Worksheet.Cells["R7:U7"].Merge = true;
                        range.Worksheet.Cells["R7:U7"].Value = "HÌNH THỨC THANH TOÁN";

                        range.Worksheet.Cells["R8:R9"].Merge = true;
                        range.Worksheet.Cells["R8:R9"].Value = "CÔNG NỢ";

                        range.Worksheet.Cells["S8:S9"].Merge = true;
                        range.Worksheet.Cells["S8:S9"].Value = "POS";

                        range.Worksheet.Cells["T8:T9"].Merge = true;
                        range.Worksheet.Cells["T8:T8"].Value = "CHUYỂN KHOẢN";

                        range.Worksheet.Cells["U8:U9"].Merge = true;
                        range.Worksheet.Cells["U8:U9"].Value = "TIỀN MẶT";


                        //Set column K to RSTU
                        range.Worksheet.Cells["V7:Y7"].Merge = true;
                        range.Worksheet.Cells["V7:Y7"].Value = "CẬP NHẬT CÔNG NỢ";

                        range.Worksheet.Cells["V8:V9"].Merge = true;
                        range.Worksheet.Cells["V8:V9"].Value = "TIỀN MẶT";

                        range.Worksheet.Cells["W8:W9"].Merge = true;
                        range.Worksheet.Cells["W8:W9"].Value = "CHUYỂN KHOẢN";

                        range.Worksheet.Cells["X8:X9"].Merge = true;
                        range.Worksheet.Cells["X8:X9"].Value = "POS";

                        range.Worksheet.Cells["Y8:Y9"].Merge = true;
                        range.Worksheet.Cells["Y8:Y9"].Value = "SỐ PHIẾU THU";


                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoThuPhiVienPhiGridVo>(requestProperties);

                    int index = 10;
                    decimal tongSoTienTamUng = 0;
                    decimal tongSoTienHoanUng = 0;
                    decimal tongSoTienHuyThu = 0;
                    decimal tongSoTienThu = 0;
                    decimal tongSoTienCongNo = 0;
                    decimal tongSoTienPos = 0;
                    decimal tongSoTienChuyenKhoan = 0;
                    decimal tongSoTienTienMat = 0;

                    decimal tongSoTienThuNoTienMat = 0;
                    decimal tongSoTienThuNoChuyenKhoan = 0;
                    decimal tongSoTienThuNoPos = 0;

                    var thuNgans = dataBaoCaos.ToArray();




                    var indexMain = index;

                    decimal tongSoTienTamUngRow = 0;
                    decimal tongSoTienHoanUngRow = 0;
                    decimal tongSoTienHuyThuRow = 0;
                    decimal tongSoTienThuRow = 0;
                    decimal tongSoTienCongNoRow = 0;
                    decimal tongSoTienPosRow = 0;
                    decimal tongSoTienChuyenKhoanRow = 0;
                    decimal tongSoTienTienMatRow = 0;

                    decimal tongSoTienThuNoTienMatRow = 0;
                    decimal tongSoTienThuNoChuyenKhoanRow = 0;
                    decimal tongSoTienThuNoPosRow = 0;


                    var dataByThuNgans = dataBaoCaos.ToArray();

                    int Stt = 1;
                    var listExel = new List<BaoCaoDVNgoaiGoiKeToanExelGridVo>();
                    foreach (var vb in dataByThuNgans)
                    {
                        foreach (var dv in vb.TenDichVus)
                        {
                            listExel.Add(new BaoCaoDVNgoaiGoiKeToanExelGridVo
                            {
                                TenDichVu = dv.TenDichVu,
                                DonGiaBenhVien = dv.DonGiaBenhVien,
                                DonGiaMoi = dv.DonGiaMoi,
                                SoTienDuocMienGiam = dv.SoTienDuocMienGiam,
                                SoTienThucThu = dv.SoTienThucThu,
                                BHYT = vb.BHYT,
                                ChiTietBHYTs = vb.ChiTietBHYTs,
                                ChiTietCongNoTuNhans = vb.ChiTietCongNoTuNhans,
                                CongNoCaNhan = vb.CongNoCaNhan,
                                CongNoTuNhan = vb.CongNoTuNhan,
                                CoTiemChung = vb.CoTiemChung,
                                DataPhieuChis = vb.DataPhieuChis,
                                GioiTinh = vb.GioiTinh,
                                GoiDichVu = vb.GoiDichVu,
                                HoTen = vb.HoTen,
                                Id = vb.Id,
                                LaPhieuHuy = vb.LaPhieuHuy,
                                LoaiChiTienBenhNhan = vb.LoaiChiTienBenhNhan,
                                LoaiThuTienBenhNhan = vb.LoaiThuTienBenhNhan,
                                LyDo = vb.LyDo,
                                MaNguoiBenh = vb.MaNguoiBenh,
                                MaNhanVien = vb.MaNhanVien,
                                MaTiepNhan = vb.MaTiepNhan,
                                NamSinh = vb.NamSinh,
                                NgayBienLai = vb.NgayBienLai,

                                NguoiGioiThieu = vb.NguoiGioiThieu,
                                SoBienLai = vb.SoBienLai,
                                SoHoaDon = vb.SoHoaDon,
                                SoHoaDonChiTiet = vb.SoHoaDonChiTiet,
                                SoPhieuThuGhiNo = vb.SoPhieuThuGhiNo,
                                SoTienThuChuyenKhoan = vb.SoTienThuChuyenKhoan,
                                SoTienThuPos = vb.SoTienThuPos,
                                SoTienThuTamUng = vb.SoTienThuTamUng,
                                SoTienThuTienMat = vb.SoTienThuTienMat,
                                STT = vb.STT,
                                TenDichVus = vb.TenDichVus,
                                TongChiPhiBNTT = vb.TongChiPhiBNTT,
                                Voucher = vb.Voucher,

                            });
                        }
                    }

                    var length = 0;
                    var lenghtValue = 0;
                    foreach (var itemgroup in listExel.OrderBy(d => d.NgayBienLai).ThenBy(d => d.SoBienLai).ToList())
                    {

                        lenghtValue = listExel.Where(d => d.NgayBienLai == itemgroup.NgayBienLai && d.SoBienLai == itemgroup.SoBienLai).Count();


                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                        }
                        // nếu list dv == 1 =? bind bình thường


                        tongSoTienTamUng += itemgroup.TamUng ?? 0;
                        tongSoTienHoanUng += itemgroup.HoanUng ?? 0;
                        tongSoTienThu += itemgroup.SoTienThu ?? 0;
                        tongSoTienHuyThu += itemgroup.HuyThu ?? 0;
                        tongSoTienCongNo += itemgroup.CongNo ?? 0;
                        tongSoTienPos += itemgroup.Pos ?? 0;
                        tongSoTienChuyenKhoan += itemgroup.ChuyenKhoan ?? 0;
                        tongSoTienTienMat += itemgroup.TienMat ?? 0;

                        tongSoTienThuNoTienMat += itemgroup.ThuNoTienMat ?? 0;
                        tongSoTienThuNoChuyenKhoan += itemgroup.ThuNoChuyenKhoan ?? 0;
                        tongSoTienThuNoPos += itemgroup.ThuNoPos ?? 0;

                        tongSoTienTamUngRow += itemgroup.TamUng ?? 0;
                        tongSoTienHoanUngRow += itemgroup.HoanUng ?? 0;
                        tongSoTienThuRow += itemgroup.SoTienThu ?? 0;
                        tongSoTienHuyThuRow += itemgroup.HuyThu ?? 0;
                        tongSoTienCongNoRow += itemgroup.CongNo ?? 0;
                        tongSoTienPosRow += itemgroup.Pos ?? 0;
                        tongSoTienChuyenKhoanRow += itemgroup.ChuyenKhoan ?? 0;
                        tongSoTienTienMatRow += itemgroup.TienMat ?? 0;

                        tongSoTienThuNoTienMatRow += itemgroup.ThuNoTienMat ?? 0;
                        tongSoTienThuNoChuyenKhoanRow += itemgroup.ThuNoChuyenKhoan ?? 0;
                        tongSoTienThuNoPosRow += itemgroup.ThuNoPos ?? 0;


                        worksheet.Cells["A" + index].Value = Stt;
                        worksheet.Cells["B" + index].Value = itemgroup.NgayBienLaiStr;
                        worksheet.Cells["C" + index].Value = itemgroup.SoBienLai;

                        worksheet.Cells["D" + index].Value = itemgroup.MaNhanVien;
                        worksheet.Cells["E" + index].Value = itemgroup.MaNguoiBenh;

                        worksheet.Cells["F" + index].Value = itemgroup.MaTiepNhan;
                        worksheet.Cells["G" + index].Value = itemgroup.HoTen;
                        worksheet.Cells["H" + index].Value = itemgroup.GioiTinh;

                        worksheet.Cells["I" + index].Value = itemgroup.NamSinh;



                        // FOR MAT 
                        worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";



                        worksheet.Cells["J" + index].Value = itemgroup.TenDichVu;
                        worksheet.Cells["K" + index].Value = Convert.ToDouble(itemgroup.DonGiaBenhVien);
                        worksheet.Cells["L" + index].Value = Convert.ToDouble(itemgroup.DonGiaMoi);
                        worksheet.Cells["M" + index].Value = Convert.ToDouble(itemgroup.SoTienDuocMienGiam);
                        worksheet.Cells["N" + index].Value = Convert.ToDouble(itemgroup.SoTienThucThu);




                        if (length == lenghtValue - 1 && lenghtValue > 1)
                        {
                            if (itemgroup.HuyThu != null)
                            {
                                if (itemgroup.HuyThu > 0)
                                {
                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Red);
                                    }
                                }
                                else
                                {
                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                    }
                                }
                            }
                            else
                            {
                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                }
                            }


                            var indO = IndexValues(lenghtValue, "O", index);
                            worksheet.Cells[indO].Merge = true;
                            worksheet.Cells[indO].Value = Convert.ToDouble(itemgroup.TamUng);

                            var indP = IndexValues(lenghtValue, "P", index);
                            worksheet.Cells[indP].Merge = true;
                            worksheet.Cells[indP].Value = Convert.ToDouble(itemgroup.HoanUng);

                            var indQ = IndexValues(lenghtValue, "Q", index);
                            worksheet.Cells[indQ].Merge = true;
                            worksheet.Cells[indQ].Value = Convert.ToDouble(itemgroup.HuyThu);

                            var indR = IndexValues(lenghtValue, "R", index);
                            worksheet.Cells[indR].Merge = true;
                            worksheet.Cells[indR].Value = Convert.ToDouble(itemgroup.CongNo);


                            var indS = IndexValues(lenghtValue, "S", index);
                            worksheet.Cells[indS].Merge = true;
                            worksheet.Cells[indS].Value = Convert.ToDouble(itemgroup.Pos);

                            var indT = IndexValues(lenghtValue, "T", index);
                            worksheet.Cells[indT].Merge = true;
                            worksheet.Cells[indT].Value = Convert.ToDouble(itemgroup.ChuyenKhoan);

                            var indU = IndexValues(lenghtValue, "U", index);
                            worksheet.Cells[indU].Merge = true;
                            worksheet.Cells[indU].Value = Convert.ToDouble(itemgroup.TienMat);

                            var indV = IndexValues(lenghtValue, "V", index);
                            worksheet.Cells[indV].Merge = true;
                            worksheet.Cells[indV].Value = Convert.ToDouble(itemgroup.ThuNoTienMat);

                            var indW = IndexValues(lenghtValue, "W", index);
                            worksheet.Cells[indW].Merge = true;
                            worksheet.Cells[indW].Value = Convert.ToDouble(itemgroup.ThuNoChuyenKhoan);

                            var indX = IndexValues(lenghtValue, "X", index);
                            worksheet.Cells[indX].Merge = true;
                            worksheet.Cells[indX].Value = Convert.ToDouble(itemgroup.ThuNoPos);

                            var indY = IndexValues(lenghtValue, "Y", index);
                            worksheet.Cells[indY].Merge = true;
                            worksheet.Cells[indY].Value = itemgroup.SoPhieuThuGhiNo;
                            index++;
                            length = 0;
                            // to mau phieeus huy?

                        }
                        else
                        {

                            worksheet.Cells["O" + index].Value = Convert.ToDouble(itemgroup.TamUng);
                            worksheet.Cells["P" + index].Value = Convert.ToDouble(itemgroup.HoanUng);
                            worksheet.Cells["Q" + index].Value = Convert.ToDouble(itemgroup.HuyThu);


                            worksheet.Cells["R" + index].Value = Convert.ToDouble(itemgroup.CongNo);
                            worksheet.Cells["S" + index].Value = Convert.ToDouble(itemgroup.Pos);
                            worksheet.Cells["T" + index].Value = Convert.ToDouble(itemgroup.ChuyenKhoan);
                            worksheet.Cells["U" + index].Value = Convert.ToDouble(itemgroup.TienMat);


                            worksheet.Cells["V" + index].Value = Convert.ToDouble(itemgroup.ThuNoTienMat);
                            worksheet.Cells["W" + index].Value = Convert.ToDouble(itemgroup.ThuNoChuyenKhoan);
                            worksheet.Cells["X" + index].Value = Convert.ToDouble(itemgroup.ThuNoPos);
                            worksheet.Cells["Y" + index].Value = itemgroup.SoPhieuThuGhiNo;

                            // to mau phieeus huy?
                            if (itemgroup.HuyThu != null)
                            {
                                if (itemgroup.HuyThu > 0)
                                {
                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Red);
                                    }
                                }
                                else
                                {
                                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                    {
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                    }
                                }
                            }
                            else
                            {
                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                }
                            }
                            index++;
                            length++;
                        }




                        // FOR MAT 
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";


                        if (lenghtValue == 1)
                        {
                            length = 0;
                        }
                        Stt++;



                    }




                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    ///
                    worksheet.Cells[worksheetFirstLast].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells[worksheetFirstLast].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[worksheetFirstLast].Style.Font.Bold = true;
                    worksheet.Cells[worksheetFirstLast].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells[worksheetFirstLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;





                    ////
                    ///

                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                    {
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                    }

                    // total grid
                    if (datatotal != null)
                    {
                        using (var range = worksheet.Cells["B" + index + ":J" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":J" + index].Merge = true;
                            range.Worksheet.Cells["B" + index + ":J" + index].Value = "Tổng cộng".ToUpper();
                        }
                        // STYLE 
                        // FOR MAT 
                        worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                        //
                        worksheet.Cells["K" + index].Value = Convert.ToDouble(datatotal.DonGiaBenhVien);
                        worksheet.Cells["L" + index].Value = Convert.ToDouble(datatotal.DonGiaMoi);
                        worksheet.Cells["M" + index].Value = Convert.ToDouble(datatotal.SoTienDuocMienGiam);
                        worksheet.Cells["N" + index].Value = Convert.ToDouble(datatotal.SoTienThucThu);
                        worksheet.Cells["O" + index].Value = Convert.ToDouble(datatotal.TamUng);
                        worksheet.Cells["P" + index].Value = Convert.ToDouble(datatotal.HoanUng);
                        worksheet.Cells["Q" + index].Value = Convert.ToDouble(datatotal.HuyThu);


                        worksheet.Cells["R" + index].Value = Convert.ToDouble(datatotal.CongNo);
                        worksheet.Cells["S" + index].Value = Convert.ToDouble(datatotal.Pos);
                        worksheet.Cells["T" + index].Value = Convert.ToDouble(datatotal.ChuyenKhoan);
                        worksheet.Cells["U" + index].Value = Convert.ToDouble(datatotal.TienMat);


                        worksheet.Cells["V" + index].Value = Convert.ToDouble(datatotal.ThuNoTienMat);
                        worksheet.Cells["W" + index].Value = Convert.ToDouble(datatotal.ThuNoChuyenKhoan);
                        worksheet.Cells["X" + index].Value = Convert.ToDouble(datatotal.ThuNoPos);

                    }
                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        public string IndexValues(int lengthGrid, string kyTu, int indexx)
        {
            var indexString = string.Empty;
            if (lengthGrid > 1)
            {
                indexString = kyTu + (indexx - (lengthGrid - 1)) + ":" + kyTu + (indexx);
            }
            else
            {
                indexString = kyTu + indexx;
            }
            return indexString;
        }
        public async Task<TotalBaoCaoThuPhiVienPhiGridVo> GetTotalBaoCaoChiTietThuTienDichVuNgoaiGoiForGridAsync(BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo)
        {
            var allData = await GetAllForBaoCaoBenhNhanKhamDicVuNgoaiGoi(queryInfo);

            return new TotalBaoCaoThuPhiVienPhiGridVo
            {
                TamUng = allData.Select(o => o.TamUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HoanUng = allData.Select(o => o.HoanUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                SoTienThu = allData.Select(o => o.SoTienThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HuyThu = allData.Select(o => o.HuyThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                CongNo = allData.Select(o => o.CongNo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TienMat = allData.Select(o => o.TienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ChuyenKhoan = allData.Select(o => o.ChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                Pos = allData.Select(o => o.Pos.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoTienMat = allData.Select(o => o.ThuNoTienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoChuyenKhoan = allData.Select(o => o.ThuNoChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoPos = allData.Select(o => o.ThuNoPos.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                DonGiaBenhVien = allData.Select(o => o.TenDichVus.Select(d => d.DonGiaBenhVien.GetValueOrDefault()).DefaultIfEmpty(0).Sum()).Sum(),
                DonGiaMoi = allData.Select(o => o.TenDichVus.Select(d => d.DonGiaMoi.GetValueOrDefault()).DefaultIfEmpty(0).Sum()).Sum(),
                SoTienThucThu = allData.Select(o => o.TenDichVus.Select(d => d.SoTienThucThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum()).Sum(),
                SoTienDuocMienGiam = allData.Select(o => o.TenDichVus.Select(d => d.SoTienDuocMienGiam.GetValueOrDefault()).DefaultIfEmpty(0).Sum()).Sum(),

            };
        }
        public async Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> GetHopDongKhamDoan(DropDownListRequestModel queryInfo)
        {

            var lstHopDongKhamSucKhoes = await _hopDongKhamSucKhoeRepository.TableNoTracking
                    .ApplyLike(queryInfo.Query, x => x.SoHopDong)
                    .Select(item => new LookupItemHopDingKhamSucKhoeTemplateVo
                    {
                        KeyId = item.Id,
                        DisplayName = item.SoHopDong,
                        SoHopDong = item.SoHopDong,
                        NgayHieuLuc = item.NgayHieuLuc,
                        NgayKetThuc = item.NgayKetThuc,
                        CongTyKhamSucKhoeId = item.CongTyKhamSucKhoeId
                    })
                    .OrderByDescending(x => x.CongTyKhamSucKhoeId).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            return lstHopDongKhamSucKhoes;
        }
        #endregion Báo cáo dịch vu ngoài gói 2/11/2021 
        #region BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ 22/11/2021

        public async Task<GridDataSource> BaoCaoDoanhThuKhamDoanTheoNhomDichVu(BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo queryInfo)
        {
            var lstDoanhThuDichVuKhamQuery = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(x => x.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham
                                        && ((x.ThoiDiemHoanThanh != null && x.ThoiDiemHoanThanh >= queryInfo.FromDate && x.ThoiDiemHoanThanh < queryInfo.ToDate) || (x.ThoiDiemHoanThanh == null && x.ThoiDiemThucHien != null && x.ThoiDiemThucHien >= queryInfo.FromDate && x.ThoiDiemThucHien < queryInfo.ToDate))
                                        && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null);
            var lstDoanhThuDichVuKyThuatQuery = _yeuCauDichVuKyThuatRepository.TableNoTracking
                            .Where(x => (x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || (x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien))
                                        && ((x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem && x.ThoiDiemThucHien != null && x.PhienXetNghiemChiTiets.Any(p => p.ThoiDiemNhanMau != null && p.ThoiDiemNhanMau >= queryInfo.FromDate && p.ThoiDiemNhanMau < queryInfo.ToDate))
                                            || (x.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem && ((x.ThoiDiemHoanThanh != null && x.ThoiDiemHoanThanh >= queryInfo.FromDate && x.ThoiDiemHoanThanh < queryInfo.ToDate) || (x.ThoiDiemHoanThanh == null && x.ThoiDiemThucHien != null && x.ThoiDiemThucHien >= queryInfo.FromDate && x.ThoiDiemThucHien < queryInfo.ToDate))))
                                        && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null);
            if (queryInfo.CongTyId != 0)
            {
                if (queryInfo.HopDongId != 0)
                {
                    lstDoanhThuDichVuKhamQuery = lstDoanhThuDichVuKhamQuery.Where(o => o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryInfo.HopDongId);
                    lstDoanhThuDichVuKyThuatQuery = lstDoanhThuDichVuKyThuatQuery.Where(o => o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryInfo.HopDongId);
                }
                else
                {
                    lstDoanhThuDichVuKhamQuery = lstDoanhThuDichVuKhamQuery.Where(o => o.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryInfo.CongTyId);
                }
            }

            var lstDoanhThuDichVuKham = lstDoanhThuDichVuKhamQuery
                            .Select(item => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataVo()
                            {
                                YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                YeucauKhamBenhId = item.Id,
                                TenDichVu = item.TenDichVu,
                                GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                                Gia = item.Gia,
                                SoLan = 1,
                                DonGiaUuDai = item.DonGiaUuDai,
                                SoTienMienGiam = item.SoTienMienGiam,
                            })
                            .ToList();
            var lstDoanhThuDichVuKyThuat = lstDoanhThuDichVuKyThuatQuery
                            .Select(item => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataVo()
                            {
                                YeucauTiepNhanId = item.YeuCauTiepNhanId,
                                YeucauDichVuKyThuatId = item.Id,
                                NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                                LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                                //BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataXetNghiemVos = item.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ?
                                //    item.PhienXetNghiemChiTiets.Select(p => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataXetNghiemVo { Id = p.Id, ThoiDiemNhanMau = p.ThoiDiemNhanMau}).ToList() :
                                //    new List<BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataXetNghiemVo>(),
                                TenDichVu = item.TenDichVu,
                                GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                                Gia = item.Gia,
                                SoLan = item.SoLan,
                                DonGiaUuDai = item.DonGiaUuDai,
                                SoTienMienGiam = item.SoTienMienGiam,
                            })
                            .ToList();
            //var lstDoanhThuDichVuKyThuat = lstDoanhThuDichVuKyThuatAll.Where(o =>
            //    o.LoaiDichVuKyThuat != LoaiDichVuKyThuat.XetNghiem ||
            //    (o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem &&
            //     o.BaoCaoDoanhThuKhamDoanTheoNhomDichVuDataXetNghiemVos.Any(x=>x.ThoiDiemNhanMau != null && x.ThoiDiemNhanMau >= queryInfo.FromDate && x.ThoiDiemNhanMau < queryInfo.ToDate))).ToList();
            var allDichVu = lstDoanhThuDichVuKham.Concat(lstDoanhThuDichVuKyThuat).ToList();
            var yctnIds = allDichVu.Select(o => o.YeucauTiepNhanId).Distinct().ToList();
            var yctnDatas = _yeuCauTiepNhanRepository.TableNoTracking.Where(o => yctnIds.Contains(o.Id))
                .Select(o => new BaoCaoDoanhThuKhamDoanTheoNhomDichVuYCTNDataVo
                {
                    YeucauTiepNhanId = o.Id,
                    HopDongId = o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                    CongTyId = o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                    MaNB = o.BenhNhan.MaBN,
                    MaTN = o.MaYeuCauTiepNhan,
                    HoVaTen = o.HoTen,
                    NamSinh = o.NamSinh,
                    GioiTinh = o.GioiTinh,
                    TenCongTy = o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    SoHopDong = o.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.SoHopDong
                }).ToList();
            var cauHinhBaoCao = _cauHinhService.LoadSetting<CauHinhBaoCao>();

            var gridData = new List<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo>();
            foreach (var dv in allDichVu)
            {
                var yctn = yctnDatas.FirstOrDefault(o => o.YeucauTiepNhanId == dv.YeucauTiepNhanId);
                var gridItem = new BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo
                {
                    MaTN = yctn.MaTN,
                    MaNB = yctn.MaNB,
                    HopDongId = yctn.HopDongId,
                    CongTyId = yctn.CongTyId,
                    HoVaTen = yctn.HoVaTen,
                    NamSinh = yctn.NamSinh?.ToString(),
                    GioiTinh = yctn.GioiTinh?.GetDescription(),
                    TenCongTy = $"{yctn.TenCongTy} - {yctn.SoHopDong}"
                };

                if (dv.GoiKhamSucKhoeId != null)
                {
                    var doanhThu = dv.DonGiaUuDai.GetValueOrDefault() * dv.SoLan;
                    if (dv.YeucauKhamBenhId != null)
                    {
                        gridItem.SoTienDVKhamBenh = doanhThu;
                    }
                    else
                    {
                        if (dv.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
                        {
                            gridItem.SoTienDVXetNghiem = doanhThu;
                        }
                        else
                        {
                            if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoi)
                            {
                                gridItem.SoTienDVThamDoChucNangNoiSoi = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoiTMH)
                            {
                                gridItem.SoTienDVTDCNNoiSoiTMH = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomSieuAm)
                            {
                                gridItem.SoTienDVCDHASieuAm = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangThuong || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa)
                            {
                                gridItem.SoTienDVCDHAXQuangThuongXQuangSoHoa = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomCTScanner)
                            {
                                gridItem.SoTienDVCTScan = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomMRI)
                            {
                                gridItem.SoTienDVMRI = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienTim || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienNao)
                            {
                                gridItem.SoTienDVDienTimDienNao = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoLoangXuong || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoHoHap)
                            {
                                gridItem.SoTienDVTDCNDoLoangXuong = doanhThu;
                            }
                            else
                            {
                                gridItem.SoTienDVKhac = doanhThu;
                            }
                        }
                    }
                    gridItem.TongCong = doanhThu;
                }
                else
                {
                    var doanhThu = dv.Gia * dv.SoLan - dv.SoTienMienGiam.GetValueOrDefault();
                    if (dv.YeucauKhamBenhId != null)
                    {
                        gridItem.SoTienDVKhamBenhNG = doanhThu;
                    }
                    else
                    {
                        if (dv.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
                        {
                            gridItem.SoTienDVXetNghiemNG = doanhThu;
                        }
                        if (dv.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat)
                        {
                            if (dv.TenDichVu != null &&
                                dv.TenDichVu.ToLower().Contains("phẫu thuật"))
                            {
                                gridItem.SoTienDVPhauThuatNG = doanhThu;
                            }
                            else
                            {
                                gridItem.SoTienDVThuThuatNG = doanhThu;
                            }
                        }
                        else
                        {
                            if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoi)
                            {
                                gridItem.SoTienDVThamDoChucNangNoiSoiNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomNoiSoiTMH)
                            {
                                gridItem.SoTienDVTDCNNoiSoiTMHNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomSieuAm)
                            {
                                gridItem.SoTienDVCDHASieuAmNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangThuong || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomXQuangSoHoa)
                            {
                                gridItem.SoTienDVCDHAXQuangThuongXQuangSoHoaNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomCTScanner)
                            {
                                gridItem.SoTienDVCTScanNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomMRI)
                            {
                                gridItem.SoTienDVMRING = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienTim || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDienNao)
                            {
                                gridItem.SoTienDVDienTimDienNaoNG = doanhThu;
                            }
                            else if (dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoLoangXuong || dv.NhomDichVuBenhVienId == cauHinhBaoCao.NhomDoHoHap)
                            {
                                gridItem.SoTienDVTDCNDoLoangXuongNG = doanhThu;
                            }
                            else
                            {
                                gridItem.SoTienDVKhacNG = doanhThu;
                            }
                        }
                    }
                    gridItem.TongCongNG = doanhThu;
                }
                gridData.Add(gridItem);
            }

            var returnData = new List<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo>();
            foreach (var yctn in gridData.GroupBy(o => o.MaTN))
            {
                var returnItem = new BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo
                {
                    MaTN = yctn.Key,
                    MaNB = yctn.First().MaNB,
                    HopDongId = yctn.First().HopDongId,
                    CongTyId = yctn.First().CongTyId,
                    HoVaTen = yctn.First().HoVaTen,
                    NamSinh = yctn.First().NamSinh,
                    GioiTinh = yctn.First().GioiTinh,
                    TenCongTy = yctn.First().TenCongTy
                };
                returnItem.SoTienDVKhamBenh = yctn.All(o => o.SoTienDVKhamBenh == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVKhamBenh.GetValueOrDefault()).Sum();

                returnItem.SoTienDVXetNghiem = yctn.All(o => o.SoTienDVXetNghiem == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVXetNghiem.GetValueOrDefault()).Sum();

                returnItem.SoTienDVThamDoChucNangNoiSoi = yctn.All(o => o.SoTienDVThamDoChucNangNoiSoi == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVThamDoChucNangNoiSoi.GetValueOrDefault()).Sum();

                returnItem.SoTienDVTDCNNoiSoiTMH = yctn.All(o => o.SoTienDVTDCNNoiSoiTMH == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVTDCNNoiSoiTMH.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCDHASieuAm = yctn.All(o => o.SoTienDVCDHASieuAm == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCDHASieuAm.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCDHAXQuangThuongXQuangSoHoa = yctn.All(o => o.SoTienDVCDHAXQuangThuongXQuangSoHoa == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCDHAXQuangThuongXQuangSoHoa.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCTScan = yctn.All(o => o.SoTienDVCTScan == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCTScan.GetValueOrDefault()).Sum();

                returnItem.SoTienDVMRI = yctn.All(o => o.SoTienDVMRI == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVMRI.GetValueOrDefault()).Sum();

                returnItem.SoTienDVDienTimDienNao = yctn.All(o => o.SoTienDVDienTimDienNao == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVDienTimDienNao.GetValueOrDefault()).Sum();

                returnItem.SoTienDVTDCNDoLoangXuong = yctn.All(o => o.SoTienDVTDCNDoLoangXuong == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVTDCNDoLoangXuong.GetValueOrDefault()).Sum();

                returnItem.SoTienDVKhac = yctn.All(o => o.SoTienDVKhac == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVKhac.GetValueOrDefault()).Sum();

                returnItem.TongCong = yctn.All(o => o.TongCong == null) ? (decimal?)null : yctn.Select(o => o.TongCong.GetValueOrDefault()).Sum();

                returnItem.SoTienDVKhamBenhNG = yctn.All(o => o.SoTienDVKhamBenhNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVKhamBenhNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVXetNghiemNG = yctn.All(o => o.SoTienDVXetNghiemNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVXetNghiemNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVPhauThuatNG = yctn.All(o => o.SoTienDVPhauThuatNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVPhauThuatNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVThuThuatNG = yctn.All(o => o.SoTienDVThuThuatNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVThuThuatNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVThamDoChucNangNoiSoiNG = yctn.All(o => o.SoTienDVThamDoChucNangNoiSoiNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVThamDoChucNangNoiSoiNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVTDCNNoiSoiTMHNG = yctn.All(o => o.SoTienDVTDCNNoiSoiTMHNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVTDCNNoiSoiTMHNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCDHASieuAmNG = yctn.All(o => o.SoTienDVCDHASieuAmNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCDHASieuAmNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCDHAXQuangThuongXQuangSoHoaNG = yctn.All(o => o.SoTienDVCDHAXQuangThuongXQuangSoHoaNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCDHAXQuangThuongXQuangSoHoaNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVCTScanNG = yctn.All(o => o.SoTienDVCTScanNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVCTScanNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVMRING = yctn.All(o => o.SoTienDVMRING == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVMRING.GetValueOrDefault()).Sum();

                returnItem.SoTienDVDienTimDienNaoNG = yctn.All(o => o.SoTienDVDienTimDienNaoNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVDienTimDienNaoNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVTDCNDoLoangXuongNG = yctn.All(o => o.SoTienDVTDCNDoLoangXuongNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVTDCNDoLoangXuongNG.GetValueOrDefault()).Sum();

                returnItem.SoTienDVKhacNG = yctn.All(o => o.SoTienDVKhacNG == null) ? (decimal?)null : yctn.Select(o => o.SoTienDVKhacNG.GetValueOrDefault()).Sum();

                returnItem.TongCongNG = yctn.All(o => o.TongCongNG == null) ? (decimal?)null : yctn.Select(o => o.TongCongNG.GetValueOrDefault()).Sum();

                returnData.Add(returnItem);
            }

            return new GridDataSource { Data = returnData.OrderBy(o => o.MaTN).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = returnData.Count };
        }
        public virtual byte[] ExportBaoCaoDoanhThuKhamDoanTheoNhomDichVu(IList<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo> bc, BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.ToDate;
            var denNgay = queryInfo.FromDate;

            var dataBaoCaos = bc.ToList();

            int ind = 1;

            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo>("STT",p => ind++),
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // set column
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 15;
                    worksheet.Column(10).Width = 15;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 15;
                    worksheet.Column(13).Width = 15;
                    worksheet.Column(14).Width = 15;
                    worksheet.Column(15).Width = 15;
                    worksheet.Column(16).Width = 15;

                    worksheet.Column(17).Width = 15;
                    worksheet.Column(18).Width = 15;
                    worksheet.Column(19).Width = 15;
                    worksheet.Column(20).Width = 15;
                    worksheet.Column(21).Width = 15;
                    worksheet.Column(22).Width = 15;
                    worksheet.Column(23).Width = 15;
                    worksheet.Column(24).Width = 15;
                    worksheet.Column(25).Width = 15;
                    worksheet.Column(26).Width = 15;
                    worksheet.Column(27).Width = 15;
                    worksheet.Column(28).Width = 15;
                    worksheet.Column(29).Width = 15;
                    worksheet.Column(30).Width = 15;
                    worksheet.Column(31).Width = 15;
                    worksheet.Column(32).Width = 15;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF" };
                    var worksheetTitleThuPhi = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 4;
                    var worksheetTitleNgay = SetColumnItems[0] + 5 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 5;

                    using (var range = worksheet.Cells[worksheetTitleThuPhi])
                    {
                        range.Worksheet.Cells[worksheetTitleThuPhi].Merge = true;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Value = "BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ".ToUpper();
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleThuPhi].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + queryInfo.FromDate.ApplyFormatDate() + " đến ngày: " + queryInfo.ToDate.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        //range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }


                    using (var range = worksheet.Cells["A6:AF8"])
                    {
                        range.Worksheet.Cells["A6:AF8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:AF8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:AF8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A6:AF8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:AF8"].Style.Font.Bold = true;



                        //Set column A to A
                        range.Worksheet.Cells["A6:A8"].Merge = true;
                        range.Worksheet.Cells["A6:A8"].Value = "STT";
                        //Set column B to B
                        range.Worksheet.Cells["B6:B8"].Merge = true;
                        range.Worksheet.Cells["B6:B8"].Value = "Mã NB";
                        //Set column C to C
                        range.Worksheet.Cells["C6:C8"].Merge = true;
                        range.Worksheet.Cells["B6:C8"].Value = "Mã TN";
                        //Set column D to D
                        range.Worksheet.Cells["D6:D8"].Merge = true;
                        range.Worksheet.Cells["D6:D8"].Value = "Họ và Tên";
                        //Set column E to E
                        range.Worksheet.Cells["E6:E8"].Merge = true;
                        range.Worksheet.Cells["E6:E8"].Value = "Năm sinh";
                        //Set column F to F
                        range.Worksheet.Cells["F6:F8"].Merge = true;
                        range.Worksheet.Cells["F6:F8"].Value = "Giới tính";



                        #region DOANH THU DỊCH VỤ TRONG GÓI
                        //Set column F to Q
                        range.Worksheet.Cells["G6:R7"].Merge = true;
                        range.Worksheet.Cells["G6:R7"].Value = "DOANH THU DỊCH VỤ TRONG GÓI";

                        range.Worksheet.Cells["G8:G8"].Merge = true;
                        range.Worksheet.Cells["G8:G8"].Value = "Khám bệnh";



                        range.Worksheet.Cells["H8:H8"].Merge = true;
                        range.Worksheet.Cells["H8:H8"].Value = "Xét nghiệm";

                        range.Worksheet.Cells["I8:I8"].Merge = true;
                        range.Worksheet.Cells["I8:I8"].Value = "Nội soi";

                        range.Worksheet.Cells["J8:J8"].Merge = true;
                        range.Worksheet.Cells["J8:J8"].Value = "Nội soi TMH";

                        range.Worksheet.Cells["K8:K8"].Merge = true;
                        range.Worksheet.Cells["K8:K8"].Value = "Siêu âm";

                        range.Worksheet.Cells["L8:L8"].Merge = true;
                        range.Worksheet.Cells["L8:L8"].Value = "X-Quang";


                        range.Worksheet.Cells["M8:M8"].Merge = true;
                        range.Worksheet.Cells["M8:M8"].Value = "CT Scan";

                        range.Worksheet.Cells["N8:N8"].Merge = true;
                        range.Worksheet.Cells["N8:N8"].Value = "MRI";

                        range.Worksheet.Cells["O8:O8"].Merge = true;
                        range.Worksheet.Cells["O8:O8"].Value = "ĐiệnTim + Điện Não";

                        range.Worksheet.Cells["P8:P8"].Merge = true;
                        range.Worksheet.Cells["P8:P8"].Value = "TDCN + Đo loãng xương";

                        range.Worksheet.Cells["Q8:Q8"].Merge = true;
                        range.Worksheet.Cells["Q8:Q8"].Value = "DV khác";

                        range.Worksheet.Cells["R8:R8"].Merge = true;
                        range.Worksheet.Cells["R8:R8"].Value = "Tổng cộng";
                        #endregion
                        #region DOANH THU DỊCH VỤ PHÁT SINH NGOÀI GÓI
                        //Set column R to AE
                        range.Worksheet.Cells["S6:AF7"].Merge = true;
                        range.Worksheet.Cells["S6:AF7"].Value = "DOANH THU DỊCH VỤ PHÁT SINH NGOÀI GÓI";

                        range.Worksheet.Cells["S8:S8"].Merge = true;
                        range.Worksheet.Cells["S8:S8"].Value = "Khám bệnh";



                        range.Worksheet.Cells["T8:T8"].Merge = true;
                        range.Worksheet.Cells["T8:T8"].Value = "Xét nghiệm";

                        range.Worksheet.Cells["U8:U8"].Merge = true;
                        range.Worksheet.Cells["U8:U8"].Value = "Nội soi";

                        range.Worksheet.Cells["V8:V8"].Merge = true;
                        range.Worksheet.Cells["V8:V8"].Value = "Nội soi TMH";

                        range.Worksheet.Cells["W8:W8"].Merge = true;
                        range.Worksheet.Cells["W8:W8"].Value = "Siêu âm";

                        range.Worksheet.Cells["X8:X8"].Merge = true;
                        range.Worksheet.Cells["X8:X8"].Value = "X-Quang";


                        range.Worksheet.Cells["Y8:Y8"].Merge = true;
                        range.Worksheet.Cells["Y8:Y8"].Value = "CT Scan";

                        range.Worksheet.Cells["Z8:Z8"].Merge = true;
                        range.Worksheet.Cells["Z8:Z8"].Value = "MRI";

                        range.Worksheet.Cells["AA8:AA8"].Merge = true;
                        range.Worksheet.Cells["AA8:AA8"].Value = "ĐiệnTim + Điện Não";

                        range.Worksheet.Cells["AB8:AB8"].Merge = true;
                        range.Worksheet.Cells["AB8:AB8"].Value = "TDCN + Đo loãng xương";

                        range.Worksheet.Cells["AC8:AC8"].Merge = true;
                        range.Worksheet.Cells["AC8:AC8"].Value = "Thủ thuật";

                        range.Worksheet.Cells["AD8:AD8"].Merge = true;
                        range.Worksheet.Cells["AD8:AD8"].Value = "Phẩu thuật";

                        range.Worksheet.Cells["AE8:AE8"].Merge = true;
                        range.Worksheet.Cells["AE8:AE8"].Value = "DV khác";

                        range.Worksheet.Cells["AF8:AF8"].Merge = true;
                        range.Worksheet.Cells["AF8:AF8"].Value = "Tổng cộng";
                        #endregion

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo>(requestProperties);


                    var dataBind = dataBaoCaos.GroupBy(d => d.TenCongTy).ToArray();
                    var dataTotal = dataBaoCaos.GroupBy(d => d.TenCongTy)
                        .Select(d => new
                        {
                            // trong gói
                            SumKhamBenh = d.Sum(f => f.SoTienDVKhamBenh),
                            SumXetNghiem = d.Sum(f => f.SoTienDVXetNghiem),
                            SumNoiSoi = d.Sum(f => f.SoTienDVThamDoChucNangNoiSoi),
                            SumNoiSoiTMH = d.Sum(f => f.SoTienDVTDCNNoiSoiTMH),
                            SumSieuAm = d.Sum(f => f.SoTienDVCDHASieuAm),
                            SumXQuang = d.Sum(f => f.SoTienDVCDHAXQuangThuongXQuangSoHoa),
                            SumCTScan = d.Sum(f => f.SoTienDVCTScan),
                            SumMRI = d.Sum(f => f.SoTienDVMRI),
                            SumDienTimDienNao = d.Sum(f => f.SoTienDVDienTimDienNao),
                            SumTDCNDoLoangXuong = d.Sum(f => f.SoTienDVTDCNDoLoangXuong),
                            SumDVKhac = d.Sum(f => f.SoTienDVKhac),
                            SumTongCong = d.Sum(f => f.TongCong),
                            // ngoài gói
                            SumKhamBenhNG = d.Sum(f => f.SoTienDVKhamBenhNG),
                            SumXetNghiemNG = d.Sum(f => f.SoTienDVXetNghiemNG),
                            SumNoiSoiNG = d.Sum(f => f.SoTienDVThamDoChucNangNoiSoiNG),
                            SumNoiSoiTMHNG = d.Sum(f => f.SoTienDVTDCNNoiSoiTMHNG),
                            SumSieuAmNG = d.Sum(f => f.SoTienDVCDHASieuAmNG),
                            SumXQuangNG = d.Sum(f => f.SoTienDVCDHAXQuangThuongXQuangSoHoaNG),
                            SumCTScanNG = d.Sum(f => f.SoTienDVCTScanNG),
                            SumMRING = d.Sum(f => f.SoTienDVMRING),
                            SumDienTimDienNaoNG = d.Sum(f => f.SoTienDVDienTimDienNaoNG),
                            SumTDCNDoLoangXuongNG = d.Sum(f => f.SoTienDVTDCNDoLoangXuongNG),
                            SumDVKhacNG = d.Sum(f => f.SoTienDVKhacNG),
                            SumTongCongNG = d.Sum(f => f.TongCongNG),
                            TenCongTy = d.First().TenCongTy,
                            SumThuThuat = d.Sum(f => f.SoTienDVThuThuatNG),
                            SumPhauThuat = d.Sum(f => f.SoTienDVPhauThuatNG)
                        }).ToArray();


                    int index = 9;

                    var listExel = new List<BaoCaoDVNgoaiGoiKeToanExelGridVo>();


                    var length = 1;
                    var lenghtValue = 0;
                    int lengthIndexArray = 0;
                    foreach (var itemgroup in dataBind)
                    {
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));

                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":" + "F" + index].Style.Font.Bold = true;
                        worksheet.Cells["A" + index + ":" + "F" + index].Merge = true;
                        worksheet.Cells["A" + index + ":" + "F" + index].Value = dataTotal[lengthIndexArray].TenCongTy;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                        }

                        #region dịch vụ trong gói
                        worksheet.Cells["G" + index + ":" + "G" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["G" + index + ":" + "G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["H" + index + ":" + "H" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["H" + index + ":" + "H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["I" + index + ":" + "I" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["I" + index + ":" + "I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["J" + index + ":" + "J" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["J" + index + ":" + "J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["K" + index + ":" + "K" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["K" + index + ":" + "K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["L" + index + ":" + "L" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["L" + index + ":" + "L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["M" + index + ":" + "M" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["M" + index + ":" + "M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["N" + index + ":" + "N" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["N" + index + ":" + "N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["O" + index + ":" + "O" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["O" + index + ":" + "O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["P" + index + ":" + "P" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["P" + index + ":" + "P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["Q" + index + ":" + "Q" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Q" + index + ":" + "Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells["R" + index + ":" + "R" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["R" + index + ":" + "R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


                        worksheet.Cells["G" + index + ":" + "G" + index].Value = dataTotal[lengthIndexArray].SumKhamBenh;
                        worksheet.Cells["H" + index + ":" + "H" + index].Value = dataTotal[lengthIndexArray].SumXetNghiem;
                        worksheet.Cells["I" + index + ":" + "I" + index].Value = dataTotal[lengthIndexArray].SumNoiSoi;
                        worksheet.Cells["J" + index + ":" + "J" + index].Value = dataTotal[lengthIndexArray].SumNoiSoiTMH;
                        worksheet.Cells["K" + index + ":" + "K" + index].Value = dataTotal[lengthIndexArray].SumSieuAm;
                        worksheet.Cells["L" + index + ":" + "L" + index].Value = dataTotal[lengthIndexArray].SumXQuang;
                        worksheet.Cells["M" + index + ":" + "M" + index].Value = dataTotal[lengthIndexArray].SumCTScan;
                        worksheet.Cells["N" + index + ":" + "N" + index].Value = dataTotal[lengthIndexArray].SumMRI;
                        worksheet.Cells["O" + index + ":" + "O" + index].Value = dataTotal[lengthIndexArray].SumDienTimDienNao;
                        worksheet.Cells["P" + index + ":" + "P" + index].Value = dataTotal[lengthIndexArray].SumTDCNDoLoangXuong;
                        worksheet.Cells["Q" + index + ":" + "Q" + index].Value = dataTotal[lengthIndexArray].SumDVKhac;
                        worksheet.Cells["R" + index + ":" + "R" + index].Value = dataTotal[lengthIndexArray].SumTongCong;
                        #endregion

                        #region DỊCH VỤ NGOÀI GÓI
                        worksheet.Cells["S" + index + ":" + "S" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["T" + index + ":" + "T" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["U" + index + ":" + "U" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["V" + index + ":" + "V" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["W" + index + ":" + "W" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["X" + index + ":" + "X" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Y" + index + ":" + "Y" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["Z" + index + ":" + "Z" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AA" + index + ":" + "AA" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AB" + index + ":" + "AB" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AC" + index + ":" + "AC" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AD" + index + ":" + "AD" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AE" + index + ":" + "AE" + index].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells["AF" + index + ":" + "AF" + index].Style.Numberformat.Format = "#,##0.00";



                        worksheet.Cells["S" + index + ":" + "S" + index].Value = dataTotal[lengthIndexArray].SumKhamBenhNG;
                        worksheet.Cells["T" + index + ":" + "T" + index].Value = dataTotal[lengthIndexArray].SumXetNghiemNG;
                        worksheet.Cells["U" + index + ":" + "U" + index].Value = dataTotal[lengthIndexArray].SumNoiSoiNG;
                        worksheet.Cells["V" + index + ":" + "V" + index].Value = dataTotal[lengthIndexArray].SumNoiSoiTMHNG;
                        worksheet.Cells["W" + index + ":" + "W" + index].Value = dataTotal[lengthIndexArray].SumSieuAmNG;
                        worksheet.Cells["X" + index + ":" + "X" + index].Value = dataTotal[lengthIndexArray].SumXQuangNG;
                        worksheet.Cells["Y" + index + ":" + "Y" + index].Value = dataTotal[lengthIndexArray].SumCTScanNG;
                        worksheet.Cells["Z" + index + ":" + "Z" + index].Value = dataTotal[lengthIndexArray].SumMRING;
                        worksheet.Cells["AA" + index + ":" + "AA" + index].Value = dataTotal[lengthIndexArray].SumDienTimDienNaoNG;
                        worksheet.Cells["AB" + index + ":" + "AB" + index].Value = dataTotal[lengthIndexArray].SumTDCNDoLoangXuongNG;
                        worksheet.Cells["AC" + index + ":" + "AC" + index].Value = dataTotal[lengthIndexArray].SumThuThuat;
                        worksheet.Cells["AD" + index + ":" + "AD" + index].Value = dataTotal[lengthIndexArray].SumPhauThuat;
                        worksheet.Cells["AE" + index + ":" + "AE" + index].Value = dataTotal[lengthIndexArray].SumDVKhacNG;
                        worksheet.Cells["AF" + index + ":" + "AF" + index].Value = dataTotal[lengthIndexArray].SumTongCongNG;
                        #endregion
                        index++;
                        int Stt = 1;
                        foreach (var item in itemgroup.ToList())
                        {
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index].Value = Stt;

                            worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["B" + index].Value = item.MaNB;

                            worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["C" + index].Value = item.MaTN;

                            worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["D" + index].Value = item.HoVaTen;

                            worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["E" + index].Value = item.NamSinh;

                            worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["F" + index].Value = item.GioiTinh;

                            worksheet.Cells["A" + index + ":" + "D" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            for (int ii = 0; ii < SetColumnItems.Length; ii++)
                            {
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                                //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            }
                            #region DỊCH VỤ TRONG GÓI
                            #region css
                            worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["N" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                            #endregion css
                            #region bind item
                            worksheet.Cells["G" + index].Value = SoTien(item.SoTienDVKhamBenh);
                            worksheet.Cells["H" + index].Value = SoTien(item.SoTienDVXetNghiem);
                            worksheet.Cells["I" + index].Value = SoTien(item.SoTienDVThamDoChucNangNoiSoi);
                            worksheet.Cells["J" + index].Value = SoTien(item.SoTienDVTDCNNoiSoiTMH);
                            worksheet.Cells["K" + index].Value = SoTien(item.SoTienDVCDHASieuAm);
                            worksheet.Cells["L" + index].Value = SoTien(item.SoTienDVCDHAXQuangThuongXQuangSoHoa);
                            worksheet.Cells["M" + index].Value = SoTien(item.SoTienDVCTScan);
                            worksheet.Cells["N" + index].Value = SoTien(item.SoTienDVMRI);
                            worksheet.Cells["O" + index].Value = SoTien(item.SoTienDVDienTimDienNao);
                            worksheet.Cells["P" + index].Value = SoTien(item.SoTienDVTDCNDoLoangXuong);
                            worksheet.Cells["Q" + index].Value = SoTien(item.SoTienDVKhac);
                            worksheet.Cells["R" + index].Value = SoTien(item.TongCong);
                            #endregion bind item
                            #endregion
                            #region DỊCH VỤ ngoài GÓI
                            #region css
                            worksheet.Cells["S" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["T" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["U" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["V" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["W" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["X" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["Y" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["Z" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AA" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AB" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AC" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AD" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AE" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["AF" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Z" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AA" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AB" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AC" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AD" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AE" + index].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AF" + index].Style.Numberformat.Format = "#,##0.00";
                            #endregion  css
                            #region bind item 
                            worksheet.Cells["S" + index].Value = SoTien(item.SoTienDVKhamBenhNG);
                            worksheet.Cells["T" + index].Value = SoTien(item.SoTienDVXetNghiemNG);
                            worksheet.Cells["U" + index].Value = SoTien(item.SoTienDVThamDoChucNangNoiSoiNG);
                            worksheet.Cells["V" + index].Value = SoTien(item.SoTienDVTDCNNoiSoiTMHNG);
                            worksheet.Cells["W" + index].Value = SoTien(item.SoTienDVCDHASieuAmNG);
                            worksheet.Cells["X" + index].Value = SoTien(item.SoTienDVCDHAXQuangThuongXQuangSoHoaNG);
                            worksheet.Cells["Y" + index].Value = SoTien(item.SoTienDVCTScanNG);
                            worksheet.Cells["Z" + index].Value = SoTien(item.SoTienDVMRING);
                            worksheet.Cells["AA" + index].Value = SoTien(item.SoTienDVDienTimDienNaoNG);
                            worksheet.Cells["AB" + index].Value = SoTien(item.SoTienDVTDCNDoLoangXuongNG);
                            worksheet.Cells["AC" + index].Value = SoTien(item.SoTienDVThuThuatNG);
                            worksheet.Cells["AD" + index].Value = SoTien(item.SoTienDVPhauThuatNG);
                            worksheet.Cells["AE" + index].Value = SoTien(item.SoTienDVKhacNG);
                            worksheet.Cells["AF" + index].Value = SoTien(item.TongCongNG);
                            #endregion item
                            #endregion
                            index++;
                            Stt++;

                        }
                        lengthIndexArray++;

                    }

                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    ///
                    worksheet.Cells[worksheetFirstLast].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    worksheet.Cells[worksheetFirstLast].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells[worksheetFirstLast].Style.Font.Bold = true;
                    worksheet.Cells[worksheetFirstLast].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    worksheet.Cells[worksheetFirstLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;





                    ////
                    ///

                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
                    {
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                    }

                    // total grid
                    #region dịch vụ trong gói
                    var totalSoTienDVKhamBenh = bc.Sum(d => d.SoTienDVKhamBenh);
                    var totalSoTienDVXetNghiem = bc.Sum(d => d.SoTienDVXetNghiem);
                    var totalSoTienDVThamDoChucNangNoiSoi = bc.Sum(d => d.SoTienDVThamDoChucNangNoiSoi);
                    var totalSoTienDVTDCNNoiSoiTMH = bc.Sum(d => d.SoTienDVTDCNNoiSoiTMH);
                    var totalSoTienDVCDHASieuAm = bc.Sum(d => d.SoTienDVCDHASieuAm);
                    var totalSoTienDVCDHAXQuangThuongXQuangSoHoa = bc.Sum(d => d.SoTienDVCDHAXQuangThuongXQuangSoHoa);
                    var totalSoTienDVCTScan = bc.Sum(d => d.SoTienDVCTScan);
                    var totalSoTienDVMRI = bc.Sum(d => d.SoTienDVMRI);
                    var totalSoTienDVDienTimDienNao = bc.Sum(d => d.SoTienDVDienTimDienNao);
                    var totalSoTienDVTDCNDoLoangXuong = bc.Sum(d => d.SoTienDVTDCNDoLoangXuong);
                    var totalSoTienDVKhac = bc.Sum(d => d.SoTienDVKhac);
                    var totalTongCong = bc.Sum(d => d.TongCong);
                    #endregion
                    #region ngoài gói
                    var totalSoTienDVKhamBenhNG = bc.Sum(d => d.SoTienDVKhamBenhNG);
                    var totalSoTienDVXetNghiemNG = bc.Sum(d => d.SoTienDVXetNghiemNG);
                    var totalSoTienDVThamDoChucNangNoiSoiNG = bc.Sum(d => d.SoTienDVThamDoChucNangNoiSoiNG);
                    var totalSoTienDVTDCNNoiSoiTMHNG = bc.Sum(d => d.SoTienDVTDCNNoiSoiTMHNG);
                    var totalSoTienDVCDHASieuAmNG = bc.Sum(d => d.SoTienDVCDHASieuAmNG);
                    var totalSoTienDVCDHAXQuangThuongXQuangSoHoaNG = bc.Sum(d => d.SoTienDVCDHAXQuangThuongXQuangSoHoaNG);
                    var totalSoTienDVCTScanNG = bc.Sum(d => d.SoTienDVCTScanNG);
                    var totalSoTienDVMRING = bc.Sum(d => d.SoTienDVMRING);
                    var totalSoTienDVDienTimDienNaoNG = bc.Sum(d => d.SoTienDVDienTimDienNaoNG);
                    var totalSoTienDVTDCNDoLoangXuongNG = bc.Sum(d => d.SoTienDVTDCNDoLoangXuongNG);
                    var totalSoTienDVThuThuatNG = bc.Sum(d => d.SoTienDVThuThuatNG);
                    var totalSoTienDVPhauThuatNG = bc.Sum(d => d.SoTienDVPhauThuatNG);
                    var totalSoTienDVKhacNG = bc.Sum(d => d.SoTienDVKhacNG);
                    var totalTongCongNG = bc.Sum(d => d.TongCongNG);
                    #endregion

                    using (var range = worksheet.Cells["A" + index + ":F" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":F" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":F" + index].Value = "Tổng cộng";
                    }
                    // STYLE 
                    // FOR MAT 
                    #region trong gói
                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["L" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["M" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["N" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["O" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["P" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Q" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["R" + index].Style.Numberformat.Format = "#,##0.00";
                    #endregion
                    #region ngoài gói
                    worksheet.Cells["S" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["T" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["U" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["V" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["W" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["X" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Y" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["Z" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AA" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AB" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AC" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AD" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AE" + index].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells["AF" + index].Style.Numberformat.Format = "#,##0.00";
                    #endregion
                    //
                    worksheet.Cells["G" + index].Value = totalSoTienDVKhamBenh;
                    worksheet.Cells["H" + index].Value = totalSoTienDVXetNghiem;
                    worksheet.Cells["I" + index].Value = totalSoTienDVThamDoChucNangNoiSoi;
                    worksheet.Cells["J" + index].Value = totalSoTienDVTDCNNoiSoiTMH;
                    worksheet.Cells["K" + index].Value = totalSoTienDVCDHASieuAm;
                    worksheet.Cells["L" + index].Value = totalSoTienDVCDHAXQuangThuongXQuangSoHoa;
                    worksheet.Cells["M" + index].Value = totalSoTienDVCTScan;
                    worksheet.Cells["N" + index].Value = totalSoTienDVMRI;
                    worksheet.Cells["O" + index].Value = totalSoTienDVDienTimDienNao;
                    worksheet.Cells["P" + index].Value = totalSoTienDVTDCNDoLoangXuong;
                    worksheet.Cells["Q" + index].Value = totalSoTienDVKhac;
                    worksheet.Cells["R" + index].Value = totalTongCong;

                    // ngoài gói
                    worksheet.Cells["S" + index].Value = totalSoTienDVKhamBenhNG;
                    worksheet.Cells["T" + index].Value = totalSoTienDVXetNghiemNG;
                    worksheet.Cells["U" + index].Value = totalSoTienDVThamDoChucNangNoiSoiNG;
                    worksheet.Cells["V" + index].Value = totalSoTienDVTDCNNoiSoiTMHNG;
                    worksheet.Cells["W" + index].Value = totalSoTienDVCDHASieuAmNG;
                    worksheet.Cells["X" + index].Value = totalSoTienDVCDHAXQuangThuongXQuangSoHoaNG;
                    worksheet.Cells["Y" + index].Value = totalSoTienDVCTScanNG;
                    worksheet.Cells["Z" + index].Value = totalSoTienDVMRING;
                    worksheet.Cells["AA" + index].Value = totalSoTienDVDienTimDienNaoNG;
                    worksheet.Cells["AB" + index].Value = totalSoTienDVTDCNDoLoangXuongNG;
                    worksheet.Cells["AC" + index].Value = totalSoTienDVThuThuatNG;
                    worksheet.Cells["AD" + index].Value = totalSoTienDVPhauThuatNG;
                    worksheet.Cells["AE" + index].Value = totalSoTienDVKhacNG;
                    worksheet.Cells["AF" + index].Value = totalTongCongNG;
                    xlPackage.Save();
                }

                return stream.ToArray();
            }

        }
        private decimal? SoTien(decimal? soTien)
        {
            return soTien;
        }
        #endregion
        // BVHD-3880 cập nhật 
        public string SetValueDataYeuKhamKhamVeNull(Enums.ChuyenKhoaKhamSucKhoe? chuyenKhoaKhamSucKhoe)
        {
            var thongTinKhamData = string.Empty;
            switch (chuyenKhoaKhamSucKhoe)
            {
                case Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"TuanHoan\",\"Value\":\"\"},{\"Id\":\"TuanHoanPhanLoai\",\"Value\":\"\"},{\"Id\":\"HoHap\",\"Value\":\"\"},{\"Id\":\"HoHapPhanLoai\",\"Value\":\"\"},{\"Id\":\"TieuHoa\",\"Value\":\"\"},{\"Id\":\"TieuHoaPhanLoai\",\"Value\":\"\"},{\"Id\":\"ThanTietLieu\",\"Value\":\"\"},{\"Id\":\"ThanTietLieuPhanLoai\",\"Value\":\"\"},{\"Id\":\"NoiTiet\",\"Value\":\"\"},{\"Id\":\"NoiTietPhanLoai\",\"Value\":\"\"},{\"Id\":\"CoXuongKhop\",\"Value\":\"\"},{\"Id\":\"CoXuongKhopPhanLoai\",\"Value\":\"\"},{\"Id\":\"ThanKinh\",\"Value\":\"\"},{\"Id\":\"ThanKinhPhanLoai\",\"Value\":\"\"},{\"Id\":\"TamThan\",\"Value\":\"\"},{\"Id\":\"TamThanPhanLoai\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"NgoaiKhoa\",\"Value\":\"\"},{\"Id\":\"NgoaiKhoaPhanLoai\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"SanPhuKhoa\",\"Value\":\"\"},{\"Id\":\"\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.Mat:
                    //thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"KhongKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"KhongKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CacBenhVeMat\",\"Value\":\"Bình thường\"},{\"Id\":\"MatPhanLoai\",\"Value\":1}]}";
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"KhongKinhMatPhai\",\"Value\":\"\"},{\"Id\":\"KhongKinhMatTrai\",\"Value\":\"\"},{\"Id\":\"CacBenhVeMat\",\"Value\":\"\"},{\"Id\":\"MatPhanLoai\",\"Value\":\"\"}]}"; //{\"Id\":\"CoKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatTrai\",\"Value\":\"10/10\"},
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.RangHamMat:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"HamTren\",\"Value\":\"\"},{\"Id\":\"HamDuoi\",\"Value\":\"\"},{\"Id\":\"CacBenhRangHamMat\",\"Value\":\"\"},{\"Id\":\"RangHamMatPhanLoai\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"TaiPhaiNoiThuong\",\"Value\":\"\"},{\"Id\":\"TaiPhaiNoiTham\",\"Value\":\"\"},{\"Id\":\"TaiTraiNoiThuong\",\"Value\":\"\"},{\"Id\":\"TaiTraiNoiTham\",\"Value\":\"\"},{\"Id\":\"CacBenhTaiMuiHong\",\"Value\":\"\"},{\"Id\":\"TaiMuiHongPhanLoai\",\"Value\":\"\"}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.DaLieu:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"DaLieu\",\"Value\":\"\"},{\"Id\":\"DaLieuPhanLoai\",\"Value\":\"\"}]}";
                    break;
                default:
                    thongTinKhamData = null;
                    break;
            }

            return thongTinKhamData;
        }
        #region BVHD 3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE   
        public static string GetStatusForXetNghiemGiaTriMin(string strGiaTriMin, string strGiaTriSoSanh)
        {
            var result = string.Empty;
            if (double.TryParse(strGiaTriSoSanh, out var giaTriSoSanh))
            {
                if (double.TryParse(strGiaTriMin, out var giaTriMin))
                {
                    if (giaTriSoSanh < giaTriMin)
                        result = " (Giảm)";
                }
                else if (KiemTraKhacThuong(giaTriSoSanh, strGiaTriMin))
                {

                    strGiaTriMin = strGiaTriMin.Trim();
                    if (strGiaTriMin.StartsWith(">="))
                    {
                        if (double.TryParse(strGiaTriMin.Replace(">=", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith("<="))
                    {
                        if (double.TryParse(strGiaTriMin.Replace("<=", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith("≥"))
                    {
                        if (double.TryParse(strGiaTriMin.Replace("≥", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith("≤"))
                    {
                        if (double.TryParse(strGiaTriMin.Replace("≤", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith(">"))
                    {
                        if (double.TryParse(strGiaTriMin.Replace(">", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                    if (strGiaTriMin.StartsWith("<"))
                    {
                        if (double.TryParse(strGiaTriMin.Replace("<", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh < giaTri)
                                result = " (Giảm)";
                        }
                    }
                }
            }
            return result;
        }
        public static string GetStatusForXetNghiemGiaTriMax(string strGiaTriMax, string strGiaTriSoSanh)
        {
            var result = string.Empty;
            if (double.TryParse(strGiaTriSoSanh, out var giaTriSoSanh))
            {
                if (double.TryParse(strGiaTriMax, out var giaTriMax))
                {
                    if (giaTriSoSanh > giaTriMax)
                        result = " (Tăng)";
                }
                else if (KiemTraKhacThuong(giaTriSoSanh, strGiaTriMax))
                {
                    strGiaTriMax = strGiaTriMax.Trim();
                    if (strGiaTriMax.StartsWith(">="))
                    {
                        if (double.TryParse(strGiaTriMax.Replace(">=", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith("<="))
                    {
                        if (double.TryParse(strGiaTriMax.Replace("<=", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith("≥"))
                    {
                        if (double.TryParse(strGiaTriMax.Replace("≥", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith("≤"))
                    {
                        if (double.TryParse(strGiaTriMax.Replace("≤", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith(">"))
                    {
                        if (double.TryParse(strGiaTriMax.Replace(">", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                    if (strGiaTriMax.StartsWith("<"))
                    {
                        if (double.TryParse(strGiaTriMax.Replace("<", "").Replace(" ", ""), out var giaTri))
                        {
                            if (giaTriSoSanh > giaTri)
                                result = " (Tăng)";
                        }
                    }
                }

            }
            return result;
        }

        private static bool KiemTraKhacThuong(double giaTriSoSanh, string str)
        {
            if (str == null)
            {
                return false;
            }
            str = str.Trim();
            if (str.StartsWith(">="))
            {
                return true;
            }
            if (str.StartsWith("<="))
            {
                return true;
            }
            if (str.StartsWith("≥"))
            {
                return true;
            }
            if (str.StartsWith("≤"))
            {
                return true;
            }
            if (str.StartsWith(">"))
            {
                return true;
            }
            if (str.StartsWith("<"))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
