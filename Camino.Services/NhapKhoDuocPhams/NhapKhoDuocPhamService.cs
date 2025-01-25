using System;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoDuocPham;
using Camino.Services.Helpers;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain;
using Camino.Services.CauHinh;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Services.YeuCauKhamBenh;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Services.NhapKhoDuocPhams
{
    [ScopedDependency(ServiceType = typeof(INhapKhoDuocPhamService))]
    public partial class NhapKhoDuocPhamService : MasterFileService<NhapKhoDuocPham>, INhapKhoDuocPhamService
    {
        private readonly IRepository<NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.NhaThaus.NhaThau> _nhaThauRepository;
        private readonly IRepository<HopDongThauDuocPham> _hopDongThauDuocPhamRepository;
        private readonly IRepository<HopDongThauDuocPhamChiTiet> _hopDongThauDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri> _khoDuocPhamViTriRepository;
        private readonly IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> _khoRepository;
        private readonly IRepository<DuocPham> _duocPhamRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;

        private readonly IRepository<YeuCauNhapKhoDuocPham> _yeuCauNhapKhoDuocPhamRepository;
        private readonly IRepository<YeuCauNhapKhoVatTu> _yeuCauNhapKhoVatTuRepository;
        private readonly IRepository<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTietRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<NhomThuoc> _nhomThuocRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<YeuCauTraDuocPham> _yeuCauTraDuocPhamRepository;
        private readonly IRepository<YeuCauTraDuocPhamChiTiet> _yeuCauTraDuocPhamChiTietRepository;
        private readonly IRepository<XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        private readonly IRepository<XuatKhoDuocPhamChiTiet> _xuatKhoDuocPhamChiTietRepository;
        private readonly IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly IRepository<YeuCauTraVatTu> _yeuCauTraVatTuRepository;
        private readonly IRepository<YeuCauTraVatTuChiTiet> _yeuCauTraVatTuChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
        private readonly IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTietRepository;
        private readonly IRepository<YeuCauTraDuocPhamTuBenhNhan> _yeuCauTraDuocPhamTuBenhNhanRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan> _yeuCauTraVatTuTuBenhNhanRepository;
        public NhapKhoDuocPhamService(IRepository<NhapKhoDuocPham> repository
            , IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> khoRepository
            , IRepository<Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri> khoDuocPhamViTri
            , IRepository<HopDongThauDuocPham> hopDongThauDuocPhamRepository
            , IRepository<NhapKhoDuocPham> nhapKhoDuocPhamRepository
            , IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository
            , IRepository<HopDongThauDuocPhamChiTiet> hopDongThauDuocPhamChiTietRepository
            , IRepository<DuocPham> duocPhamRepository
            , IRepository<YeuCauNhapKhoDuocPham> yeuCauNhapKhoDuocPhamRepository
            , IRepository<YeuCauNhapKhoVatTu> yeuCauNhapKhoVatTuRepository
            , IRepository<YeuCauNhapKhoDuocPhamChiTiet> yeuCauNhapKhoDuocPhamChiTietRepository
            , IRepository<NhomThuoc> nhomThuocRepository
            , IUserAgentHelper userAgentHelper, IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository
            , IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository
            , IRepository<YeuCauTraDuocPham> yeuCauTraDuocPhamRepository
            , IRepository<YeuCauTraDuocPhamChiTiet> yeuCauTraDuocPhamChiTietRepository
            , IRepository<XuatKhoDuocPham> xuatKhoDuocPhamRepository
            , IRepository<XuatKhoDuocPhamChiTiet> xuatKhoDuocPhamChiTietRepository
            , IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTri
            , IRepository<Template> templateRepository
            , IRepository<Core.Domain.Entities.NhaThaus.NhaThau> nhaThauRepository,
              ICauHinhService cauHinhService
            , IRepository<User> userRepository,
              IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
              IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
              IRepository<YeuCauTraVatTu> yeuCauTraVatTuRepository,
              IRepository<YeuCauTraVatTuChiTiet> yeuCauTraVatTuChiTietRepository,
              IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
              IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> yeuCauTraDuocPhamTuBenhNhanChiTietRepository,
              IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> yeuCauTraVatTuTuBenhNhanChiTietRepository,
               IRepository<YeuCauTraDuocPhamTuBenhNhan> yeuCauTraDuocPhamTuBenhNhanRepository,
               IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan> yeuCauTraVatTuTuBenhNhanRepository,
        IYeuCauKhamBenhService yeuCauKhamBenhService) : base(repository)
        {
            _khoDuocPhamViTriRepository = khoDuocPhamViTri;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _hopDongThauDuocPhamRepository = hopDongThauDuocPhamRepository;
            _hopDongThauDuocPhamChiTietRepository = hopDongThauDuocPhamChiTietRepository;
            _khoRepository = khoRepository;
            _duocPhamRepository = duocPhamRepository;
            _yeuCauNhapKhoDuocPhamRepository = yeuCauNhapKhoDuocPhamRepository;
            _yeuCauNhapKhoVatTuRepository = yeuCauNhapKhoVatTuRepository;
            _yeuCauNhapKhoDuocPhamChiTietRepository = yeuCauNhapKhoDuocPhamChiTietRepository;
            _nhomThuocRepository = nhomThuocRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;
            _yeuCauTraDuocPhamRepository = yeuCauTraDuocPhamRepository;
            _yeuCauTraDuocPhamChiTietRepository = yeuCauTraDuocPhamChiTietRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _xuatKhoDuocPhamChiTietRepository = xuatKhoDuocPhamChiTietRepository;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTri;
            _userRepository = userRepository;
            _templateRepository = templateRepository;
            _nhaThauRepository = nhaThauRepository;
            _cauHinhService = cauHinhService;
            _cauHinhRepository = cauHinhRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _yeuCauTraVatTuRepository = yeuCauTraVatTuRepository;
            _yeuCauTraVatTuChiTietRepository = yeuCauTraVatTuChiTietRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _yeuCauTraDuocPhamTuBenhNhanChiTietRepository = yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
            _yeuCauTraVatTuTuBenhNhanChiTietRepository = yeuCauTraVatTuTuBenhNhanChiTietRepository;
            _yeuCauTraDuocPhamTuBenhNhanRepository = yeuCauTraDuocPhamTuBenhNhanRepository;
            _yeuCauTraVatTuTuBenhNhanRepository = yeuCauTraVatTuTuBenhNhanRepository;
        }
        public GridDataSource GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new NhapKhoDuocPhamSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoDuocPhamSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
            {
                queryObject.DaDuyet = true;
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
            }
            var query = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking;
            if(queryObject.DaDuyet == false || queryObject.DangChoDuyet == false || queryObject.TuChoiDuyet == false)
            {
                query = query.Where(o => (queryObject.DangChoDuyet == true && o.DuocKeToanDuyet == null)
                                        || (queryObject.DaDuyet == true && o.DuocKeToanDuyet == true)
                                        || (queryObject.TuChoiDuyet == true && o.DuocKeToanDuyet == false));
            }
            if (queryObject != null)
            {
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;

                    query = query.Where(p => tuNgay <= p.NgayNhap.Date);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => denNgay >= p.NgayNhap.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayDuyet != null && tuNgay <= p.NgayDuyet.Value.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    //var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    query = query.Where(p => p.NgayDuyet != null && denNgay >= p.NgayDuyet.Value.Date);
                }

                #region //BVHD-3926
                if (queryObject.RangeHoaDon != null)
                {
                    if (queryObject.RangeHoaDon.startDate != null)
                    {
                        var tuNgay = queryObject.RangeHoaDon.startDate.GetValueOrDefault().Date;
                        query = query.Where(p => p.NgayHoaDon != null
                                                   && tuNgay.Date <= p.NgayHoaDon.Value.Date);
                    }
                    if (queryObject.RangeHoaDon.endDate != null)
                    {
                        var denNgay = queryObject.RangeHoaDon.endDate.GetValueOrDefault().Date;
                        query = query.Where(p => p.NgayHoaDon != null
                                                   && denNgay.Date >= p.NgayHoaDon.Value.Date);
                    }
                }
                #endregion
            }

            var allDataNhapKhoDuocPhamGripVo = query
            .Select(s => new NhapKhoDuocPhamGripVo
            {
                Id = s.Id,
                SoChungTu = s.SoChungTu,
                SoPhieu = s.SoPhieu,
                NguoiNhapId = s.NguoiNhapId,
                TenNguoiNhap = s.NguoiNhap.User.HoTen,
                LoaiNguoiGiao = s.LoaiNguoiGiao,
                NguoiGiaoId = s.NguoiGiaoId,
                TenNguoiGiao = s.NguoiGiao != null ? s.NguoiGiao.User.HoTen : s.TenNguoiGiao,
                NgayNhap = s.NgayNhap,
                DuocKeToanDuyet = s.DuocKeToanDuyet,
                NguoiDuyet = s.NhanVienDuyet != null ? s.NhanVienDuyet.User.HoTen : "",
                NgayDuyet = s.NgayDuyet,
                NgayHoaDon = s.NgayHoaDon,
                KhoId = s.KhoId,
                DataYeuCauNhapKhoDuocPhamChiTiets = s.YeuCauNhapKhoDuocPhamChiTiets.Select(ct=>new DataYeuCauNhapKhoDuocPhamChiTiet
                {
                    Id = ct.Id,
                    KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                    HopDongThauDuocPhamId = ct.HopDongThauDuocPhamId
                }).ToList()
                //TenKho = string.Join("; ", s.YeuCauNhapKhoDuocPhamChiTiets.Select(z => z.KhoNhapSauKhiDuyet.Ten).Distinct()),

                ////BVHD-3926
                //TenNhaCungCap = string.Join("; ", s.YeuCauNhapKhoDuocPhamChiTiets.Select(a => a.HopDongThauDuocPham.NhaThau.Ten).Distinct())
            }).ToList();

            var dataKho = _khoRepository.TableNoTracking.Select(o=>new {o.Id, o.Ten}).ToList();
            var hopDongThauDuocPhamIds = allDataNhapKhoDuocPhamGripVo.SelectMany(o => o.DataYeuCauNhapKhoDuocPhamChiTiets).Select(ct => ct.HopDongThauDuocPhamId).Distinct().ToList();
            var dataHopDongThauDuocPham = _hopDongThauDuocPhamRepository.TableNoTracking
                .Where(o=> hopDongThauDuocPhamIds.Contains(o.Id))
                .Select(o => new { o.Id, TenNhaThau = o.NhaThau.Ten })
                .ToList();
            foreach(var dataNhapKhoDuocPhamGripVo in allDataNhapKhoDuocPhamGripVo)
            {
                var khoNhapSauKhiDuyetIds = dataNhapKhoDuocPhamGripVo.DataYeuCauNhapKhoDuocPhamChiTiets
                    .Where(o=>o.KhoNhapSauKhiDuyetId != null)
                    .Select(o=>o.KhoNhapSauKhiDuyetId)
                    .Distinct().ToList();
                dataNhapKhoDuocPhamGripVo.TenKho = string.Join("; ", khoNhapSauKhiDuyetIds.Select(o => dataKho.FirstOrDefault(k => k.Id == o)?.Ten ?? ""));
                var hopDongThauIds = dataNhapKhoDuocPhamGripVo.DataYeuCauNhapKhoDuocPhamChiTiets
                    .Select(o => o.HopDongThauDuocPhamId)
                    .Distinct().ToList();
                dataNhapKhoDuocPhamGripVo.TenNhaCungCap = string.Join("; ", hopDongThauIds.Select(o => dataHopDongThauDuocPham.FirstOrDefault(h => h.Id == o)?.TenNhaThau ?? ""));
            }

            if (!string.IsNullOrEmpty(queryObject.SearchString))
            {
                var searchString = queryObject.SearchString.Trim().ToLower().RemoveVietnameseDiacritics();
                allDataNhapKhoDuocPhamGripVo = allDataNhapKhoDuocPhamGripVo.Where(x =>
                    (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.ToLower().RemoveVietnameseDiacritics().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.SoPhieu) && x.SoPhieu.ToLower().RemoveVietnameseDiacritics().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.TenNhaCungCap) && x.TenNhaCungCap.ToLower().RemoveVietnameseDiacritics().Contains(searchString)))
                    .ToList();
            }

            return new GridDataSource { Data = allDataNhapKhoDuocPhamGripVo.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = allDataNhapKhoDuocPhamGripVo.Count() };
        }
        public GridDataSource GetDataForGridAsyncOld(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new NhapKhoDuocPhamSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoDuocPhamSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
            {
                queryObject.DaDuyet = true;
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
            }

            var queryDangChoDuyet = getDataYeuCauNhapKhoDuocPham(null, queryInfo, queryObject);
            var queryTuChoiDuyet = getDataYeuCauNhapKhoDuocPham(false, queryInfo, queryObject);
            var queryTuDaDuyet = getDataYeuCauNhapKhoDuocPham(true, queryInfo, queryObject);

            var query = new List<NhapKhoDuocPhamGripVo>().AsQueryable();
            var isHaveQuery = false;
            if (queryObject.DangChoDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangChoDuyet);
                }
                else
                {
                    query = queryDangChoDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.TuChoiDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaDuyet);
                }
                else
                {
                    query = queryTuDaDuyet;
                    isHaveQuery = true;
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayNhap desc,Id asc") && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var queryTask = query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = 0 };

        }

        private IQueryable<NhapKhoDuocPhamGripVo> getDataYeuCauNhapKhoDuocPham(bool? duocKeToanDuyet, QueryInfo queryInfo, NhapKhoDuocPhamSearch queryObject)
        {
            var result = _yeuCauNhapKhoDuocPhamRepository.TableNoTracking
                .Where(p => p.DuocKeToanDuyet == duocKeToanDuyet)
            .Select(s => new NhapKhoDuocPhamGripVo
            {
                Id = s.Id,
                SoChungTu = s.SoChungTu,
                SoPhieu = s.SoPhieu,
                NguoiNhapId = s.NguoiNhapId,
                TenNguoiNhap = s.NguoiNhap.User.HoTen,
                LoaiNguoiGiao = s.LoaiNguoiGiao,
                NguoiGiaoId = s.NguoiGiaoId,
                TenNguoiGiao = s.NguoiGiao != null ? s.NguoiGiao.User.HoTen : s.TenNguoiGiao,
                NgayNhap = s.NgayNhap,
                DuocKeToanDuyet = s.DuocKeToanDuyet,
                NguoiDuyet = s.NhanVienDuyet != null ? s.NhanVienDuyet.User.HoTen : "",
                NgayDuyet = s.NgayDuyet,
                NgayHoaDon = s.NgayHoaDon,
                KhoId = s.KhoId,
                TenKho = string.Join("; ", s.YeuCauNhapKhoDuocPhamChiTiets.Select(z => z.KhoNhapSauKhiDuyet.Ten).Distinct()),

                //BVHD-3926
                TenNhaCungCap = string.Join("; ", s.YeuCauNhapKhoDuocPhamChiTiets.Select(a => a.HopDongThauDuocPham.NhaThau.Ten).Distinct())
            });
            //result = result.ApplyLike(queryObject.SearchString?.Trim(), g => g.SoChungTu, g => g.SoPhieu, g => g.TenNhaCungCap);
            //g => g.TenNguoiNhap, g => g.TenNguoiGiao, g => g.NguoiDuyet, g => g.TenKho,


            #region //BVHD-3926
            if (!string.IsNullOrEmpty(queryObject.SearchString))
            {
                var searchString = queryObject.SearchString.Trim().ToLower().RemoveVietnameseDiacritics();
                result = result.Where(x =>
                    (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.ToLower().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.SoPhieu) && x.SoPhieu.ToLower().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.TenNhaCungCap) && x.TenNhaCungCap.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchString)));
            }
            #endregion

            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayNhap desc, Id asc"))
            {
                result = result.OrderBy(queryInfo.SortString);
            }

            if (queryObject != null)
            {
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null)
                {
                    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => tuNgay <= p.NgayNhap.Value.Date);
                }
                if (queryObject.RangeNhap != null && queryObject.RangeNhap.endDate != null)
                {
                    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayNhap.Value.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null)
                {
                    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    result = result.Where(p => tuNgay <= p.NgayDuyet.Value.Date);
                }
                if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.endDate != null)
                {
                    //var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayDuyet.Value.Date);
                }

                #region //BVHD-3926
                if (queryObject.RangeHoaDon != null)
                {
                    if (queryObject.RangeHoaDon.startDate != null)
                    {
                        var tuNgay = queryObject.RangeHoaDon.startDate.GetValueOrDefault().Date;
                        result = result.Where(p => p.NgayHoaDon != null 
                                                   && tuNgay.Date <= p.NgayHoaDon.Value.Date);
                    }
                    if (queryObject.RangeHoaDon.endDate != null)
                    {
                        var denNgay = queryObject.RangeHoaDon.endDate.GetValueOrDefault().Date;
                        result = result.Where(p => p.NgayHoaDon != null 
                                                   && denNgay.Date >= p.NgayHoaDon.Value.Date);
                    }
                }
                #endregion
            }

            return result;
        }

        public GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new NhapKhoDuocPhamSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoDuocPhamSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
            {
                queryObject.DaDuyet = true;
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
            }

            var queryDangChoDuyet = getDataYeuCauNhapKhoDuocPham(null, queryInfo, queryObject);
            var queryTuChoiDuyet = getDataYeuCauNhapKhoDuocPham(false, queryInfo, queryObject);
            var queryTuDaDuyet = getDataYeuCauNhapKhoDuocPham(true, queryInfo, queryObject);

            var query = new List<NhapKhoDuocPhamGripVo>().AsQueryable();
            var isHaveQuery = false;
            if (queryObject.DangChoDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangChoDuyet);
                }
                else
                {
                    query = queryDangChoDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.TuChoiDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuDaDuyet);
                }
                else
                {
                    query = queryTuDaDuyet;
                    isHaveQuery = true;
                }
            }
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }


        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long nhapKhoDuocPhamId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;

            var query = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauNhapKhoDuocPhamId == nhapKhoDuocPhamId)
               .Select(s => new NhapKhoDuocPhamChiTietGripVo()
               {
                   Id = s.Id,
                   TenDuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                   NhaThauDisplay = s.HopDongThauDuocPham.NhaThau.Ten,
                   TenHDThau = s.HopDongThauDuocPham.HeThongTuPhatSinh != true ? s.HopDongThauDuocPham.SoHopDong : string.Empty,
                   SoLo = s.Solo,
                   HanSuDung = s.HanSuDung,
                   MaVach = s.MaVach,
                   SL = s.SoLuongNhap,
                   GiaNhap = s.DonGiaNhap,
                   VAT = s.VAT,
                   ViTri = s.KhoViTri.Ten,
                   Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   GiaNhapDisplay = s.DonGiaNhap.ApplyVietnameseFloatNumber(),
                   SLDisplay = s.SoLuongNhap.ApplyNumber(),
                   LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                   TiLeBHYTThanhToan = s.TiLeBHYTThanhToan,
               });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            long nhapKhoDuocPhamId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var query = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(p => p.YeuCauNhapKhoDuocPhamId == nhapKhoDuocPhamId)
                .Select(s => new NhapKhoDuocPhamChiTietGripVo()
                {
                    Id = s.Id,
                    TenDuocPham = s.DuocPhamBenhVien.DuocPham.Ten,
                    TenHDThau = s.HopDongThauDuocPham.HeThongTuPhatSinh != true ? s.HopDongThauDuocPham.SoHopDong : string.Empty,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    MaVach = s.MaVach,
                    SL = s.SoLuongNhap,
                    GiaNhapDisplay = s.DonGiaNhap.ApplyVietnameseFloatNumber(),
                    SLDisplay = s.SoLuongNhap.ApplyFormatMoneyToDouble(false),
                    GiaNhap = s.DonGiaNhap,
                    VAT = s.VAT,
                    ViTri = s.KhoViTri.Ten,
                    Nhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                    MaRef = s.MaRef
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        //clone XuatKhoService
        public async Task UpdateNhapKhoDuocPhamChiTiet(long id, double soLuongDaXuat)
        {
            var nhapKhoChiTiet = await _nhapKhoDuocPhamChiTietRepository.Table.FirstOrDefaultAsync(p => p.Id == id);
            if (nhapKhoChiTiet != null)
            {
                nhapKhoChiTiet.SoLuongDaXuat = nhapKhoChiTiet.SoLuongDaXuat + soLuongDaXuat;
                await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(nhapKhoChiTiet);
            }
        }

        //clone XuatKhoService
        public async Task<NhapKhoDuocPhamChiTiet> GetNhapKhoDuocPhamChiTietById(long id)
        {
            var result = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return result;
        }

        public async Task NhapKhoDuocPhamChiTietUpdateViTri(long id, long? vitriId)
        {

            var nhapKhoChiTiet = await _nhapKhoDuocPhamChiTietRepository.Table.FirstOrDefaultAsync(p => p.Id == id);
            if (nhapKhoChiTiet != null)
            {
                nhapKhoChiTiet.KhoDuocPhamViTri = null;
                nhapKhoChiTiet.KhoViTriId = vitriId == 0 ? null : vitriId;
                await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(nhapKhoChiTiet);
            }
        }
        public List<NhapKhoDuocPhamTemplateVo> GetDropDownListDuocPham(NhapKhoDuocPhamVatTuTheoHopDongThau nhapKhoInput, DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listDuocPham = _duocPhamBenhVienRepository.TableNoTracking
                    .Include(x => x.DuocPham).ThenInclude(p => p.HopDongThauDuocPhamChiTiets)
                    .Include(x => x.DuocPham).ThenInclude(p => p.DuongDung)
                    .ApplyLike(model.Query, x => x.DuocPham.Ten)
                    //.ApplyLike(model.Query, x => x.DuocPham.Ten, x => x.DuocPham.HoatChat, x => x.DuocPham.NhaSanXuat, x => x.DuocPham.DonViTinh.Ten)
                    .Where(x => x.HieuLuc
                                && x.DuocPham.HopDongThauDuocPhamChiTiets.Any(y => y.HopDongThauDuocPhamId == nhapKhoInput.HopDongThauDuocPhamId))
                    .OrderBy(x => x.DuocPham.Ten)
                    .Take(model.Take)
                    .Select(item => new NhapKhoDuocPhamTemplateVo
                    {
                        Gia = item.DuocPham.HopDongThauDuocPhamChiTiets.First(i => i.DuocPhamId == item.Id && i.HopDongThauDuocPhamId == nhapKhoInput.HopDongThauDuocPhamId).Gia,
                        DisplayName = item.DuocPham.Ten,
                        KeyId = item.Id,
                        Ten = item.DuocPham.Ten,
                        Ma = item.MaDuocPhamBenhVien,
                        HoatChat = item.DuocPham.HoatChat,
                        NhaSanXuat = item.DuocPham.NhaSanXuat,
                        DVT = item.DuocPham.DonViTinh.Ten,
                        HeSoDinhMucDonViTinh = item.DuocPham.HeSoDinhMucDonViTinh,
                        HamLuong = item.DuocPham.HamLuong,
                        DuongDung = item.DuocPham.DuongDung != null ? item.DuocPham.DuongDung.Ten : "",
                        SoLuongChuaNhap = item.DuocPham.HopDongThauDuocPhamChiTiets.Where(x => x.HopDongThauDuocPhamId == nhapKhoInput.HopDongThauDuocPhamId && x.DuocPhamId == item.Id).Select(i => i.SoLuong - i.SoLuongDaCap).Sum(),
                        SLTon = item.NhapKhoDuocPhamChiTiets.Any() ? item.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == nhapKhoInput.KhoId
                       && nkct.LaDuocPhamBHYT == nhapKhoInput.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    }).ToList();
                return listDuocPham;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("HoatChat");
                lstColumnNameSearch.Add("NhaSanXuat");

                var listDuocPham = _duocPhamRepository
                    .ApplyFulltext(model.Query, "DuocPham", lstColumnNameSearch)
                    .Include(p => p.HopDongThauDuocPhamChiTiets)
                    .Include(p => p.DuongDung)
                    //.Include(x => x.DuocPham).ThenInclude(p => p.HopDongThauDuocPhamChiTiets)
                    .Where(x => x.DuocPhamBenhVien.HieuLuc
                                && x.HopDongThauDuocPhamChiTiets.Any(y => y.HopDongThauDuocPhamId == nhapKhoInput.HopDongThauDuocPhamId))
                    .Take(model.Take)
                    .Select(item => new NhapKhoDuocPhamTemplateVo
                    {
                        Gia = item.HopDongThauDuocPhamChiTiets.First(i => i.DuocPhamId == item.Id && i.HopDongThauDuocPhamId == nhapKhoInput.HopDongThauDuocPhamId).Gia,
                        //DisplayName = item.DuocPham.Ten + " - " + item.DuocPham.HoatChat + " - " + (item.DuocPham.NhaSanXuat ?? ""),
                        DisplayName = item.Ten,
                        KeyId = item.DuocPhamBenhVien.Id,
                        Ten = item.Ten,
                        Ma = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.MaDuocPhamBenhVien : "",
                        HoatChat = item.HoatChat,
                        NhaSanXuat = item.NhaSanXuat,
                        DVT = item.DonViTinh.Ten,
                        HeSoDinhMucDonViTinh = item.HeSoDinhMucDonViTinh,
                        HamLuong = item.HamLuong,
                        DuongDung = item.DuongDung != null ? item.DuongDung.Ten : "",
                        SoLuongChuaNhap = item.HopDongThauDuocPhamChiTiets.Where(x => x.HopDongThauDuocPhamId == nhapKhoInput.HopDongThauDuocPhamId && x.DuocPhamId == item.Id).Select(i => i.SoLuong - i.SoLuongDaCap).Sum(),
                        SLTon = item.DuocPhamBenhVien != null && item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any() ? item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == nhapKhoInput.KhoId
                  && nkct.LaDuocPhamBHYT == nhapKhoInput.LaDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    }).ToList();
                return listDuocPham;
            }
        }

        public List<NhapKhoDuocPhamTemplateVo> GetDropDownListDuocPhamFromNhaThau(DropDownListRequestModel model)
        {
            List<NhapKhoDuocPhamTemplateVo> result = null;
            var khoId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listDuocPham = _duocPhamBenhVienRepository.TableNoTracking
                    .ApplyLike(model.Query, x => x.DuocPham.Ten, x => x.MaDuocPhamBenhVien)
                    //.ApplyLike(model.Query, x => x.DuocPham.Ten, x => x.DuocPham.HoatChat, x => x.DuocPham.NhaSanXuat, x => x.DuocPham.DonViTinh.Ten)
                    .Where(x => x.HieuLuc)
                    .OrderBy(x => x.DuocPham.Ten)
                    .Take(model.Take)
                    .Select(item => new NhapKhoDuocPhamTemplateVo
                    {
                        DisplayName = item.DuocPham.Ten,
                        KeyId = item.Id,
                        Ten = item.DuocPham.Ten,
                        Ma = item.MaDuocPhamBenhVien,
                        HoatChat = item.DuocPham.HoatChat,
                        NhaSanXuat = item.DuocPham.NhaSanXuat,
                        DVT = item.DuocPham.DonViTinh.Ten,
                        HamLuong = item.DuocPham.HamLuong,
                        DuongDung = item.DuocPham.DuongDung != null ? item.DuocPham.DuongDung.Ten : "",
                        SLTon = item.NhapKhoDuocPhamChiTiets.Any() ? item.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoId
                        && nkct.LaDuocPhamBHYT == false && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    }).ToList();
                result = listDuocPham;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("HoatChat");
                lstColumnNameSearch.Add("NhaSanXuat");

                var listDuocPham = _duocPhamRepository
                    .ApplyFulltext(model.Query, "DuocPham", lstColumnNameSearch)
                    .Include(p => p.HopDongThauDuocPhamChiTiets)
                    .Include(p => p.DuongDung)
                    .Where(x => x.DuocPhamBenhVien.HieuLuc)
                    .Take(model.Take)
                    .Select(item => new NhapKhoDuocPhamTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.DuocPhamBenhVien.Id,
                        Ten = item.Ten,
                        Ma = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.MaDuocPhamBenhVien : "",
                        HoatChat = item.HoatChat,
                        NhaSanXuat = item.NhaSanXuat,
                        DVT = item.DonViTinh.Ten,
                        HamLuong = item.HamLuong,
                        DuongDung = item.DuongDung != null ? item.DuongDung.Ten : "",
                        SLTon = item.DuocPhamBenhVien != null && item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any() ? item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoId
                      && nkct.LaDuocPhamBHYT == false && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    }).ToList();
                result = listDuocPham;
            }
            if (model.Id > 0 && result.All(o => o.KeyId != model.Id))
            {
                var item1 = _duocPhamRepository.TableNoTracking
                    .Where(p => p.Id == model.Id)
                    .Include(p => p.HopDongThauDuocPhamChiTiets)
                    .Include(p => p.DuongDung)
                    .Select(item => new NhapKhoDuocPhamTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.DuocPhamBenhVien.Id,
                        Ten = item.Ten,
                        Ma = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.MaDuocPhamBenhVien : "",
                        HoatChat = item.HoatChat,
                        NhaSanXuat = item.NhaSanXuat,
                        DVT = item.DonViTinh.Ten,
                        HamLuong = item.HamLuong,
                        DuongDung = item.DuongDung != null ? item.DuongDung.Ten : "",
                        SLTon = item.DuocPhamBenhVien != null && item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any() ? item.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoId
                      && nkct.LaDuocPhamBHYT == false && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    });
                if (item1.Any())
                {
                    result.AddRange(item1);
                }
            }
            return result;
        }

        public async Task<List<NhaThauHopDongTemplateVo>> GetListNhaThauNhapKho(LookupQueryInfo model)
        {
            var listKhoa = await _hopDongThauDuocPhamRepository.TableNoTracking.Include(p => p.NhaThau)
                .ApplyLike(model.Query, g => g.NhaThau.Ten, g => g.NhaThau.DiaChi, g => g.SoHopDong)
                .Where(x => x.NgayHieuLuc.Date <= DateTime.Now.Date && DateTime.Now.Date <= x.NgayHetHan.Date && x.HeThongTuPhatSinh != true)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoa.Select(item => new NhaThauHopDongTemplateVo
            {
                DisplayName = item.SoHopDong,
                KeyId = item.Id,
                DiaChi = item.NhaThau.DiaChi,
                Ten = item.NhaThau.Ten,
                SoHopDong = item.SoHopDong,
                NhauThauId = item.NhaThauId
            }).ToList();

            return query;
        }

        public async Task<bool> CheckViTriKhoDuocPhamAsync(long idVitri)
        {
            return await _khoDuocPhamViTriRepository.TableNoTracking.AnyAsync(o => o.Id == idVitri);
        }
        public async Task<bool> CheckKhoDuocPhamAsync(long idKhoduoc)
        {
            return await _khoRepository.TableNoTracking.AnyAsync(o => o.Id == idKhoduoc);
        }
        public async Task<bool> CheckMaVachAsync(long nhapKhoId, string mavach, long? duocPhamBenhVienId)
        {
            if (string.IsNullOrEmpty(mavach) || duocPhamBenhVienId == null || duocPhamBenhVienId == 0)
                return false;
            return await _nhapKhoDuocPhamChiTietRepository.
                TableNoTracking.AnyAsync(o => (nhapKhoId == 0 && o.MaVach.Trim() == mavach.Trim() && o.DuocPhamBenhVienId != duocPhamBenhVienId.Value)
                                                    || (nhapKhoId != 0 && o.Id != nhapKhoId && o.MaVach.Trim() == mavach.Trim() && o.DuocPhamBenhVienId != duocPhamBenhVienId.Value));
        }
        public async Task<bool> CheckSoChungTuAsync(string sochungtu, long idNhapKho)
        {
            return await BaseRepository.TableNoTracking.AnyAsync(o => o.SoChungTu == sochungtu && o.Id != idNhapKho);
        }

        public async Task<List<LookupItemVo>> GetListViTriKhoDuocPhamTheoKho(long id, LookupQueryInfo model)
        {
            var list = _khoDuocPhamViTriRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ten)
                .Where(p => p.IsDisabled != true && p.KhoId == id)
                .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                });

            return await list.ToListAsync();
        }

        public async Task<NhapKhoDuocPham> UpdateNhapKho(NhapKhoDuocPham nhapKhoDuocPham)
        {
            BaseRepository.AutoCommitEnabled = false;
            _hopDongThauDuocPhamChiTietRepository.AutoCommitEnabled = false;

            var currentEntity = await BaseRepository.TableNoTracking
                .Include(p => p.NhapKhoDuocPhamChiTiets)
                .FirstOrDefaultAsync(p => p.Id == nhapKhoDuocPham.Id);

            var nhapKhoDuocPhamChiTietOld = new List<NhapKhoDuocPhamChiTiet>();

            foreach (var nhapKhoOld in currentEntity.NhapKhoDuocPhamChiTiets)
            {
                var soLuong = nhapKhoOld.SoLuongNhap;
                nhapKhoDuocPhamChiTietOld.Add(new NhapKhoDuocPhamChiTiet
                {
                    Id = nhapKhoOld.Id,
                    DuocPhamBenhVienId = nhapKhoOld.DuocPhamBenhVienId,
                    HopDongThauDuocPhamId = nhapKhoOld.HopDongThauDuocPhamId,
                    SoLuongNhap = soLuong
                });
            }

            foreach (var nhapKhoChiTiet in nhapKhoDuocPham.NhapKhoDuocPhamChiTiets)
            {
                var flagVisit = false;
                var lstHopDongThauDuocPhamChiTietId = await GetHopDongThauChiTiet(nhapKhoChiTiet.HopDongThauDuocPhamId,
                    nhapKhoChiTiet.DuocPhamBenhVienId);
                var hopDongThauDuocPhamChiTiet = await _hopDongThauDuocPhamChiTietRepository.Table
                    .Where(p => lstHopDongThauDuocPhamChiTietId.Any(i => i == p.Id))
                    .OrderBy(p => p.SoLuongDaCap).ToListAsync();

                var slNhapTemp = nhapKhoChiTiet.SoLuongNhap;
                var slNhapOldTemp = nhapKhoDuocPhamChiTietOld.FirstOrDefault(x => x.Id == nhapKhoChiTiet.Id);
                if (slNhapOldTemp == null)
                {
                    slNhapOldTemp = new NhapKhoDuocPhamChiTiet()
                    {
                        SoLuongNhap = 0
                    };
                }
                if (hopDongThauDuocPhamChiTiet.Any())
                {
                    foreach (var chiTiet in hopDongThauDuocPhamChiTiet)
                    {
                        if (nhapKhoChiTiet.WillDelete)
                        {
                            if (slNhapTemp < chiTiet.SoLuongDaCap)
                            {
                                chiTiet.SoLuongDaCap -= slNhapTemp;
                                slNhapTemp = 0;
                            }
                            else
                            {
                                if (slNhapTemp > 0)
                                {
                                    slNhapTemp -= chiTiet.SoLuongDaCap;
                                }
                                chiTiet.SoLuongDaCap = 0;
                            }
                            //await _hopDongThauDuocPhamChiTiet.UpdateAsync(hopDongThauDuocPhamChiTiet);
                            continue;
                        }


                        if (nhapKhoChiTiet.Id != 0 && nhapKhoChiTiet.Id == slNhapOldTemp.Id
                            && nhapKhoChiTiet.HopDongThauDuocPhamId == slNhapOldTemp.HopDongThauDuocPhamId
                            && nhapKhoChiTiet.DuocPhamBenhVienId == slNhapOldTemp.DuocPhamBenhVienId)
                        {
                            flagVisit = true;
                            if ((slNhapOldTemp.SoLuongNhap > 0 || slNhapTemp > 0) && slNhapOldTemp.SoLuongNhap != slNhapTemp)
                            {
                                if (chiTiet.SoLuongDaCap <= slNhapOldTemp.SoLuongNhap)
                                {
                                    slNhapOldTemp.SoLuongNhap -= chiTiet.SoLuongDaCap;
                                    chiTiet.SoLuongDaCap = 0;
                                    if (chiTiet.SoLuong > slNhapTemp)
                                    {
                                        chiTiet.SoLuongDaCap = slNhapTemp;
                                        slNhapTemp = 0;
                                    }
                                    else
                                    {
                                        slNhapTemp -= chiTiet.SoLuong;
                                        chiTiet.SoLuongDaCap = chiTiet.SoLuong;
                                    }
                                }
                                else
                                {
                                    chiTiet.SoLuongDaCap -= slNhapOldTemp.SoLuongNhap;
                                    slNhapOldTemp.SoLuongNhap = 0;
                                    var soLuongDaCapMoi = chiTiet.SoLuongDaCap + slNhapTemp;
                                    if (chiTiet.SoLuong >= soLuongDaCapMoi)
                                    {
                                        chiTiet.SoLuongDaCap = soLuongDaCapMoi;
                                        slNhapTemp = 0;
                                    }
                                    else
                                    {
                                        chiTiet.SoLuongDaCap = chiTiet.SoLuong;
                                        slNhapTemp -= chiTiet.SoLuong;
                                    }
                                }
                            }
                        }

                        if (flagVisit == false)
                        {
                            if (slNhapTemp + chiTiet.SoLuongDaCap <= chiTiet.SoLuong)
                            {
                                if (slNhapTemp > 0)
                                {
                                    chiTiet.SoLuongDaCap += slNhapTemp;
                                    slNhapTemp = 0;
                                }
                            }
                            else
                            {
                                if (slNhapTemp > 0)
                                {
                                    slNhapTemp -= chiTiet.SoLuong - chiTiet.SoLuongDaCap;
                                }
                                chiTiet.SoLuongDaCap = chiTiet.SoLuong;
                            }
                        }

                    }
                    await _hopDongThauDuocPhamChiTietRepository.UpdateAsync(hopDongThauDuocPhamChiTiet);
                }
            }
            await BaseRepository.UpdateAsync(nhapKhoDuocPham);
            await BaseRepository.Context.SaveChangesAsync();
            return currentEntity;
        }

        private async Task<List<long>> GetHopDongThauChiTiet(long hopDongThauDuocPhamId, long duocPhamId)
        {
            var lstHopDongThauChiTietId = await _hopDongThauDuocPhamChiTietRepository.TableNoTracking
                .Where(p => p.HopDongThauDuocPhamId == hopDongThauDuocPhamId && p.DuocPhamId == duocPhamId)
                .Select(p => p.Id).ToListAsync();
            return lstHopDongThauChiTietId;
        }

        public async Task CapNhatSoLuongThauSauKhiXoaNhapKhoAsync(List<NhapKhoDuocPhamChiTiet> nhapKhiDuocPhamChiTiets)
        {
            if (nhapKhiDuocPhamChiTiets.Any())
            {
                _hopDongThauDuocPhamChiTietRepository.AutoCommitEnabled = false;
                foreach (var item in nhapKhiDuocPhamChiTiets)
                {
                    var soLuongDaNhap = item.SoLuongNhap;

                    var hopDongThauChiTiet = await _hopDongThauDuocPhamChiTietRepository.Table
                        .Where(x => x.HopDongThauDuocPhamId == item.HopDongThauDuocPhamId && x.DuocPhamId == item.DuocPhamBenhVienId)
                        .OrderBy(x => x.SoLuongDaCap).ToListAsync();
                    foreach (var chiTiet in hopDongThauChiTiet)
                    {
                        if (soLuongDaNhap < chiTiet.SoLuongDaCap)
                        {
                            chiTiet.SoLuongDaCap -= soLuongDaNhap;
                            soLuongDaNhap = 0;
                        }
                        else
                        {
                            if (soLuongDaNhap > 0)
                            {
                                soLuongDaNhap -= chiTiet.SoLuongDaCap;
                            }
                            chiTiet.SoLuongDaCap = 0;
                        }
                    }

                    await _hopDongThauDuocPhamChiTietRepository.UpdateAsync(hopDongThauChiTiet);
                }

                await _hopDongThauDuocPhamChiTietRepository.Context.SaveChangesAsync();
            }
        }

        #region kiểm tra data

        public async Task<bool> KiemTraNhapKhoDaCoChiTietXuatKhoAsync(long nhapKhoId)
        {
            var thongTinXuat = await BaseRepository.TableNoTracking
                .Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(z => z.XuatKhoDuocPhamChiTietViTris)
                .AnyAsync(x => x.Id == nhapKhoId
                            && x.NhapKhoDuocPhamChiTiets.Any(y => y.XuatKhoDuocPhamChiTietViTris != null && y.XuatKhoDuocPhamChiTietViTris.Count > 0));
            return thongTinXuat;
        }

        public async Task<bool> KiemTraSoLuongNhapDuocPhamTheoHopDongThau(long? nhapKhoDuocPhamChiTiet, long? nhapKhoDuocPhamId, long? hopDongThauId, long? duocPhamId, double? soLuongNhap, double soLuongNhapTrongGrid, double soLuongHienTaiDuocPhamTheoHopDongThauDaLuu)
        {
            if (hopDongThauId == null || duocPhamId == null || soLuongNhap == null || hopDongThauId == 0 || duocPhamId == 0 || soLuongNhap == 0)
                return true;

            var lstHopDongThauChiTiet = await _hopDongThauDuocPhamRepository.TableNoTracking
                .Include(x => x.HopDongThauDuocPhamChiTiets)
                .Where(x => x.Id == hopDongThauId)
                .SelectMany(x => x.HopDongThauDuocPhamChiTiets.Where(y => y.DuocPhamId == duocPhamId)).ToListAsync();

            //double soLuongNhapChiTietHienTai = 0;
            double soLuongChenhLechDaLuuSoVoiHienTai = 0;
            if (nhapKhoDuocPhamId != null && nhapKhoDuocPhamId != 0)
            {
                //var nhapKhoChiTiet = await _nhapKhoDuocPhamChiTiet.TableNoTracking.FirstAsync(x => x.Id == nhapKhoDuocPhamChiTiet);
                //if (nhapKhoChiTiet != null)
                //{
                //    soLuongNhapChiTietHienTai = nhapKhoChiTiet.SoLuongNhap;
                //}


                // kiểm tra chệnh lệch số lượng đang nhập trên FE và số lượng đã lưu
                var soLuongNhapDuocPhamDaLuu = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.NhapKhoDuocPhamId == nhapKhoDuocPhamId && x.HopDongThauDuocPhamId == hopDongThauId && x.DuocPhamBenhVienId == duocPhamId)
                        .SumAsync(x => x.SoLuongNhap);
                if (soLuongHienTaiDuocPhamTheoHopDongThauDaLuu < soLuongNhapDuocPhamDaLuu)
                {
                    soLuongChenhLechDaLuuSoVoiHienTai = soLuongNhapDuocPhamDaLuu - soLuongHienTaiDuocPhamTheoHopDongThauDaLuu;
                }
            }

            var soLuongChuaNhap = lstHopDongThauChiTiet.Select(x => x.SoLuong).Sum() - lstHopDongThauChiTiet.Select(x => x.SoLuongDaCap).Sum() + soLuongChenhLechDaLuuSoVoiHienTai;
            return (soLuongNhap + soLuongNhapTrongGrid) <= soLuongChuaNhap;
        }
        #endregion

        public async Task<List<NhomThuocTreeViewVo>> GetListNhomThuocAsync(DropDownListRequestModel model)
        {
            var lstNhomDichVu = await _duocPhamBenhVienPhanNhomRepository.TableNoTracking
                .Select(item => new NhomThuocTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,
                    ParentId = item.NhomChaId
                })
                .ToListAsync();

            var query = lstNhomDichVu.Select(item => new NhomThuocTreeViewVo
            {
                KeyId = item.KeyId,
                DisplayName = item.DisplayName,
                ParentId = item.ParentId,
                Items = GetNhomThuocChildrenTree(lstNhomDichVu, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
            })
            .Where(x =>
                x.ParentId == null && (string.IsNullOrEmpty(model.Query) ||
                                       (!string.IsNullOrEmpty(model.Query) && (x.Items.Any() || x.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())))))
            .Take(model.Take).ToList();
            return query;
        }

        private List<NhomThuocTreeViewVo> GetNhomThuocChildrenTree(List<NhomThuocTreeViewVo> comments, long Id, string queryString, string parentDisplay)
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id)
                .Select(c => new NhomThuocTreeViewVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetNhomThuocChildrenTree(comments, c.KeyId, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString)
                            || (!string.IsNullOrEmpty(queryString) && (parentDisplay.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any())))
                .ToList();
            return query;
        }

        public async Task<List<LookupItemVo>> GetListViTriKhoTong1(LookupQueryInfo model)
        {
            var lstColumnNameSearch = new List<string>();
            //lstColumnNameSearch.Add("Ten");

            var list = _khoDuocPhamViTriRepository.TableNoTracking
               .ApplyLike(model.Query, g => g.Ten)
               .Where(p => p.IsDisabled != true && p.KhoDuocPham != null && p.KhoDuocPham.LoaiKho == Core.Domain.Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1)
               .Take(model.Take)
               .Select(i => new LookupItemVo
               {
                   DisplayName = i.Ten,
                   KeyId = i.Id
               });

            return await list.ToListAsync();
        }

        public async Task<List<YeuCauNhapKhoDuocPhamChiTietGridVo>> YeuCauNhapKhoDuocPhamChiTiets(long Id, string kyHieuHD, string soChungTu, List<YeuCauNhapKhoDuocPhamChiTietGridVo> models)
        {
            var yeuCauNhapKhoDuocPhamChiTietGridVos = new List<YeuCauNhapKhoDuocPhamChiTietGridVo>();
            var yeuCauNhapKhoDuocPhams = await _yeuCauNhapKhoDuocPhamRepository.TableNoTracking.Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(z => z.HopDongThauDuocPham).Where(z => z.DuocKeToanDuyet != false && z.Id != Id && z.SoChungTu.RemoveUniKeyAndToLower().Equals(soChungTu.RemoveUniKeyAndToLower()) && z.KyHieuHoaDon.RemoveUniKeyAndToLower().Equals(kyHieuHD.RemoveUniKeyAndToLower())).ToListAsync();
            foreach (var model in models)
            {
                if (yeuCauNhapKhoDuocPhams.Any(z => z.YeuCauNhapKhoDuocPhamChiTiets.Any(y => y.HopDongThauDuocPham.NhaThauId == model.NhaThauId)))
                {
                    yeuCauNhapKhoDuocPhamChiTietGridVos.Add(model);
                }
            }
            return yeuCauNhapKhoDuocPhamChiTietGridVos;
        }

        public async Task<YeuCauNhapKhoDuocPhamChiTietGridVo> GetDuocPhamGrid(YeuCauNhapKhoDuocPhamChiTietGridVo model)
        {
            var dp = _duocPhamBenhVienRepository.TableNoTracking.Include(p => p.DuocPham).ThenInclude(p => p.DonViTinh).FirstOrDefault(p => p.Id == model.DuocPhamBenhVienId)?.DuocPham;
            model.HopDongThauDisplay = model.HopDongThauDuocPhamId != null ? (await _hopDongThauDuocPhamRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.HopDongThauDuocPhamId))?.SoHopDong : string.Empty;
            model.NhaThauDisplay = model.NhaThauId != null ? (await _nhaThauRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.NhaThauId))?.Ten :
                (await _hopDongThauDuocPhamRepository.TableNoTracking.Include(e => e.NhaThau).FirstOrDefaultAsync(e => e.Id == model.HopDongThauDuocPhamId))?.NhaThau.Ten;
            model.NhaThauId = model.NhaThauId != null
                ? model.NhaThauId
                : (await _hopDongThauDuocPhamRepository.TableNoTracking.Include(e => e.NhaThau).FirstOrDefaultAsync(e =>
                    e.Id == model.HopDongThauDuocPhamId))?.NhaThauId;
            model.DuocPhamDisplay = dp?.Ten;
            model.LoaiDisplay = model.LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT";
            model.NhomDisplay = await _duocPhamBenhVienRepository.TableNoTracking.Where(z => z.Id == model.DuocPhamBenhVienId).Select(z => z.DuocPhamBenhVienPhanNhom.Ten).FirstOrDefaultAsync() ?? "CHƯA PHÂN NHÓM";
            model.Solo = model.Solo;
            model.HanSuDungDisplay = (model.HanSuDung ?? DateTime.Now).ApplyFormatDate();
            model.MaVach = model.MaVach;
            model.MaRef = model.MaRef;
            model.SoLuongNhapDisplay = ((long)model.SoLuongNhap).ApplyNumber();
            model.ViTriDisplay = (await _khoDuocPhamViTriRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.KhoViTriId))?.Ten;
            model.KhoNhapSauKhiDuyetId = model.KhoNhapSauKhiDuyetId;
            model.NguoiNhapSauKhiDuyetId = model.NguoiNhapSauKhiDuyetId;
            model.TenKhoNhapSauKhiDuyet = (await _khoRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.KhoNhapSauKhiDuyetId))?.Ten;
            model.TenNguoiNhapSauKhiDuyet = (await _userRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.NguoiNhapSauKhiDuyetId))?.HoTen;
            model.DVT = dp?.DonViTinh.Ten;
            model.HamLuong = dp?.HamLuong;
            model.ThanhTienTruocVat = model.ThanhTienTruocVat;
            model.ThanhTienSauVat = model.LoaiNhap == 1 && model.LaDuocPhamBHYT == true ? model.ThanhTienTruocVat : model.ThanhTienSauVat;
            model.KyHieuHoaDon = model.KyHieuHoaDon;
            model.HopDongThauDuocPhamId = model.HopDongThauDuocPhamId;
            model.GhiChu = model.GhiChu;
            return model;
        }

        public async Task<decimal> GetPriceOnContract(long hopDongThauId, long duocPhamId)
        {
            var dpId = _duocPhamRepository.TableNoTracking.FirstOrDefault(p => p.DuocPhamBenhVien.Id == duocPhamId)?.Id;
            if (dpId == null || dpId == 0) return 0;
            return (await _hopDongThauDuocPhamChiTietRepository.TableNoTracking.FirstOrDefaultAsync(p => p.HopDongThauDuocPhamId == hopDongThauId
                && p.DuocPhamId == dpId))?.Gia ?? 0;
        }
        public async Task<List<LookupItemVo>> GetKhoLoaiVatTus(DropDownListRequestModel model)
        {
            var list = _khoRepository.TableNoTracking.Where(p => p.LoaiVatTu == true).ApplyLike(model.Query, p => p.Ten)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                });
            return await list.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model)
        {
            var list = _khoRepository.TableNoTracking.Where(z => z.LoaiDuocPham == true).ApplyLike(model.Query, p => p.Ten)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                }).Take(model.Take);
            return await list.ToListAsync();
        }

        public string InPhieuYeuCauNhapKhoDuocPham(long yeuCauNhapKhoDuocPhamId)
        {
            var content = string.Empty;
            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            int? nhomVatTu = 0;
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuNhapKhoDuocPham"));
            var yeuCauNhapKhoDuocPhamChiTiets = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking
                                          .Where(p => p.YeuCauNhapKhoDuocPhamId == yeuCauNhapKhoDuocPhamId)
                                          .Select(s => new YeuCauNhapKhoDuocPhamChiTietData
                                          {
                                              //Ten = s.DuocPhamBenhVien.DuocPham.Ten ,
                                              DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                              SoLo = s.Solo,
                                              VAT = s.VAT,
                                              LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                                              SLTheoChungTu = s.SoLuongNhap,
                                              DonGia = s.DonGiaNhap,
                                              HanSuDung = s.HanSuDung,
                                              TenNguoiNhap = s.YeuCauNhapKhoDuocPham.NguoiNhap.User.HoTen,
                                              NgayNhap = s.YeuCauNhapKhoDuocPham.NgayNhap,
                                              NCC = s.HopDongThauDuocPham.NhaThau.Ten,
                                              TheoSoHoaDon = s.YeuCauNhapKhoDuocPham.SoChungTu,
                                              KyHieuHoaDon = s.YeuCauNhapKhoDuocPham.KyHieuHoaDon,
                                              NgayHoaDon = s.YeuCauNhapKhoDuocPham.NgayHoaDon,
                                              KhoNhap = s.KhoNhapSauKhiDuyet.Ten,
                                              KhoNhapSauDuyetId = s.KhoNhapSauKhiDuyetId,
                                              NguoiNhan = s.NguoiNhapSauKhiDuyet.HoTen,
                                              SoPhieu = s.YeuCauNhapKhoDuocPham.SoPhieu,
                                              ThueVatLamTron = s.ThueVatLamTron,
                                              ThanhTienSauVAT = s.ThanhTienSauVat,
                                              ThanhTienTruocVAT = s.ThanhTienTruocVat,
                                              Ten = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)(nhomVatTu) ?
                                                                (s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "")) :
                                                                s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                                          }).OrderBy(c => c.Ten).ToList();
            var khoNhapSauDuyetIds = yeuCauNhapKhoDuocPhamChiTiets.Select(c => c.KhoNhapSauDuyetId).Distinct().ToList();
            var range = khoNhapSauDuyetIds.Count();
            if (range <= 1)
            {
                var loaiBHYT = yeuCauNhapKhoDuocPhamChiTiets.Any(z => z.LaDuocPhamBHYT) ? "BHYT" : "";
                var loaiKhongBHYT = yeuCauNhapKhoDuocPhamChiTiets.Any(z => !z.LaDuocPhamBHYT) ? "Viện phí" : "";
                var loai = !string.IsNullOrEmpty(loaiBHYT) && !string.IsNullOrEmpty(loaiKhongBHYT) ? loaiBHYT + ", " + loaiKhongBHYT
                            : (!string.IsNullOrEmpty(loaiBHYT) && string.IsNullOrEmpty(loaiKhongBHYT) ? loaiBHYT
                             : (string.IsNullOrEmpty(loaiBHYT) && !string.IsNullOrEmpty(loaiKhongBHYT) ? loaiKhongBHYT : ""));
                decimal thanhTienTruocVAT = yeuCauNhapKhoDuocPhamChiTiets.Sum(z => z.ThanhTienTruocVAT);
                decimal tongThanhTienSauVAT = yeuCauNhapKhoDuocPhamChiTiets.Sum(z => z.ThueVatLamTron) ?? 0;
                var STT = 1;
                var duocPhamHoacVatTus = string.Empty;
                foreach (var item in yeuCauNhapKhoDuocPhamChiTiets)
                {
                    duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                               + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                               + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLo
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HanSuDungDisplay
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SLTheoChungTu.ApplyNumber()
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SLThucNhap.ApplyNumber()
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                               + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTienTruocVAT.ApplyFormatMoneyVND().Replace(" ₫", "")
                                               + "</tr>";
                    STT++;
                }
                var data = new YeuCauNhapKhoDuocPhamtData
                {
                    NgayNhapKho = yeuCauNhapKhoDuocPhamChiTiets.First().NgayNhapDisplay,
                    LoaiBHYT = loai,
                    NCC = yeuCauNhapKhoDuocPhamChiTiets.First().NCC,
                    SoHoaDon = yeuCauNhapKhoDuocPhamChiTiets.First().KyHieuHoaDonHienThi,
                    NgayHoaDon = yeuCauNhapKhoDuocPhamChiTiets.First().NgayHoaDonDisplay,
                    KhoNhap = yeuCauNhapKhoDuocPhamChiTiets.First().KhoNhap,
                    NguoiNhan = yeuCauNhapKhoDuocPhamChiTiets.First().NguoiNhan,
                    DuocPhamHoacVatTus = duocPhamHoacVatTus,
                    TienHangDecimal = thanhTienTruocVAT,
                    ThueVATDecimal = tongThanhTienSauVAT,
                    VAT = string.Join("/ ", yeuCauNhapKhoDuocPhamChiTiets.Select(z => z.VAT + "%").Distinct()),
                    ChietKhau = "0,00",
                    NguoiLap = yeuCauNhapKhoDuocPhamChiTiets.First().TenNguoiNhap,
                    SoPhieu = yeuCauNhapKhoDuocPhamChiTiets.First().SoPhieu,
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            }
            else
            {
                var count = 1;//dùng để check phiếu in cuối cùng ko cần thêm page break
                var contentChild = string.Empty;
                foreach (var khoNhapSauDuyetId in khoNhapSauDuyetIds)
                {
                    var yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet = yeuCauNhapKhoDuocPhamChiTiets.Where(c => c.KhoNhapSauDuyetId == khoNhapSauDuyetId).OrderBy(c => c.Ten).ToList();
                    var loaiBHYT = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.Any(z => z.LaDuocPhamBHYT) ? "BHYT" : "";
                    var loaiKhongBHYT = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.Any(z => !z.LaDuocPhamBHYT) ? "Viện phí" : "";
                    var loai = !string.IsNullOrEmpty(loaiBHYT) && !string.IsNullOrEmpty(loaiKhongBHYT) ? loaiBHYT + ", " + loaiKhongBHYT
                                : (!string.IsNullOrEmpty(loaiBHYT) && string.IsNullOrEmpty(loaiKhongBHYT) ? loaiBHYT
                                 : (string.IsNullOrEmpty(loaiBHYT) && !string.IsNullOrEmpty(loaiKhongBHYT) ? loaiKhongBHYT : ""));
                    decimal thanhTienTruocVAT = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.Sum(z => z.ThanhTienTruocVAT);
                    decimal tongThanhTienSauVAT = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.Sum(z => z.ThueVatLamTron) ?? 0;
                    var STT = 1;
                    var duocPhamHoacVatTus = string.Empty;
                    foreach (var item in yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SLTheoChungTu.ApplyNumber()
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SLThucNhap.ApplyNumber()
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTienTruocVAT.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "</tr>";
                        STT++;
                    }
                    var data = new YeuCauNhapKhoDuocPhamtData
                    {
                        NgayNhapKho = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.First().NgayNhapDisplay,
                        LoaiBHYT = loai,
                        NCC = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.First().NCC,
                        SoHoaDon = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.First().KyHieuHoaDonHienThi,
                        NgayHoaDon = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.First().NgayHoaDonDisplay,
                        KhoNhap = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.First().KhoNhap,
                        NguoiNhan = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.First().NguoiNhan,
                        DuocPhamHoacVatTus = duocPhamHoacVatTus,
                        TienHangDecimal = thanhTienTruocVAT,
                        ThueVATDecimal = tongThanhTienSauVAT,
                        VAT = string.Join("/ ", yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.Select(z => z.VAT + "%").Distinct()),
                        ChietKhau = "0,00",
                        NguoiLap = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.First().TenNguoiNhap,
                        SoPhieu = yeuCauNhapKhoDuocPhamChiTietTheoKhoDuyet.First().SoPhieu,
                    };
                    contentChild = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                    if (count < range && contentChild != "")
                    {
                        contentChild = contentChild + "<div class=\"pagebreak\"> </div>";
                    }
                    count++;
                    content += contentChild;
                }
            }
            return content;
        }


        public bool KiemTraNgayHetHanHopDong(long? hopDongThauDuocPhamId)
        {
            return _hopDongThauDuocPhamRepository.TableNoTracking.Include(p => p.NhaThau)
                 .Any(x => x.Id == hopDongThauDuocPhamId && x.NgayHieuLuc.Date <= DateTime.Now.Date && DateTime.Now.Date <= x.NgayHetHan.Date);
        }

        public NhapKhoDuocPhamTemplateVo SoLuongHopDongThauDuocPham(long? hopDongThauDuocPhamId, long? duocPhamBenhVienId, long khoId, bool? laDuocPhamBHYT)
        {
            var duocPham = _duocPhamBenhVienRepository.TableNoTracking
                   .Include(x => x.DuocPham).ThenInclude(p => p.HopDongThauDuocPhamChiTiets)
                   .Where(x => x.HieuLuc && x.Id == duocPhamBenhVienId
                               && x.DuocPham.HopDongThauDuocPhamChiTiets.Any(y => y.HopDongThauDuocPhamId == hopDongThauDuocPhamId))
                   .Select(item => new NhapKhoDuocPhamTemplateVo
                   {
                       SoLuongChuaNhap = item.DuocPham.HopDongThauDuocPhamChiTiets.Where(x => x.HopDongThauDuocPhamId == hopDongThauDuocPhamId && x.DuocPhamId == item.Id).Select(i => i.SoLuong - i.SoLuongDaCap).Sum(),
                       SLTon = item.NhapKhoDuocPhamChiTiets.Any() ? item.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoId
                      && nkct.LaDuocPhamBHYT == laDuocPhamBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                   }).FirstOrDefault();

            return duocPham;
        }
    }
}
