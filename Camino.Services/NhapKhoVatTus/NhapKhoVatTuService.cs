using Camino.Core.DependencyInjection.Attributes;
using Camino.Data;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Services.Helpers;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoVatTu;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Services.NhapKhoVatTus
{
    [ScopedDependency(ServiceType = typeof(INhapKhoVatTuService))]
    public partial class NhapKhoVatTuService : MasterFileService<NhapKhoVatTu>, INhapKhoVatTuService
    {
        private readonly IRepository<YeuCauNhapKhoVatTu> _yeuCauNhapKhoVatTuRepository;
        private readonly IRepository<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTietRepository;
        private readonly IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        private readonly IRepository<Core.Domain.Entities.NhaThaus.NhaThau> _nhaThau;

        private readonly IRepository<HopDongThauVatTu> _hopDongThauVatTuRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> _vatTuBenhVienRepository;

        private readonly IRepository<HopDongThauVatTu> _hopDongThauVatTu;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;

        private readonly IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        private readonly IRepository<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTietRepository;
        private readonly IRepository<HopDongThauVatTuChiTiet> _hopDongThauVatTuChiTiet;
        private readonly IRepository<Kho> _khoRepository;

        private readonly IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;

        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;

        private readonly IRepository<YeuCauTraVatTu> _yeuCauTraVatTuRepository;
        private readonly IRepository<YeuCauTraVatTuChiTiet> _yeuCauTraVatTuChiTietRepository;
        private readonly IRepository<User> _userRepository;

        public NhapKhoVatTuService(IRepository<NhapKhoVatTu> repository
                                  , IRepository<NhapKhoVatTu> nhapKhoVatTuRepository
                                  , IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository
                                  , IRepository<YeuCauNhapKhoVatTu> yeuCauNhapKhoVatTuRepository, IRepository<HopDongThauVatTu> hopDongThauVatTu
                                  , IRepository<YeuCauNhapKhoVatTuChiTiet> yeuCauNhapKhoVatTuChiTietRepository
                                  , IRepository<HopDongThauVatTu> hopDongThauVatTuRepository, IRepository<Camino.Core.Domain.Entities.VatTus.VatTu> vatTuRepository
                                  , IRepository<Camino.Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> vatTuBenhVienRepository
                                  , IRepository<HopDongThauVatTuChiTiet> hopDongThauVatTuChiTiet
                                  , IRepository<XuatKhoVatTu> xuatKhoVatTuRepository, IRepository<Kho> khoRepository
                                  , IRepository<XuatKhoVatTuChiTiet> xuatKhoVatTuChiTietRepository
                                  , IRepository<YeuCauTraVatTu> yeuCauTraVatTuRepository
                                  , IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository
                                  , IRepository<YeuCauTraVatTuChiTiet> yeuCauTraVatTuChiTietRepository
                                  , IRepository<Camino.Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository
                                  , IRepository<User> userRepository
                                  , IRepository<Template> templateRepository
                                  , IUserAgentHelper userAgentHelper
                                  , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
                                  , IRepository<Core.Domain.Entities.NhaThaus.NhaThau> nhaThau) : base(repository)
        {
            _yeuCauNhapKhoVatTuRepository = yeuCauNhapKhoVatTuRepository;
            _yeuCauNhapKhoVatTuChiTietRepository = yeuCauNhapKhoVatTuChiTietRepository;

            _hopDongThauVatTuRepository = hopDongThauVatTuRepository;
            _vatTuRepository = vatTuRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _hopDongThauVatTuChiTiet = hopDongThauVatTuChiTiet;
            _hopDongThauVatTu = hopDongThauVatTu;

            _khoRepository = khoRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _xuatKhoVatTuChiTietRepository = xuatKhoVatTuChiTietRepository;

            _nhanVienRepository = nhanVienRepository;
            _templateRepository = templateRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;

            _userAgentHelper = userAgentHelper;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _yeuCauTraVatTuRepository = yeuCauTraVatTuRepository;
            _yeuCauTraVatTuChiTietRepository = yeuCauTraVatTuChiTietRepository;
            _userRepository = userRepository;
            _nhaThau = nhaThau;
            _phongBenhVienRepository = phongBenhVienRepository;
        }

        public GridDataSource GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new NhapKhoVatTuSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoVatTuSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
            {
                queryObject.DaDuyet = true;
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
            }

            var query = _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(o=> o.Kho.LoaiKho != Enums.EnumLoaiKhoDuocPham.KhoHanhChinh);
            if (queryObject.DaDuyet == false || queryObject.DangChoDuyet == false || queryObject.TuChoiDuyet == false)
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

            var allDataNhapKhoVatTuGripVo = query
                .Select(s => new NhapKhoVatTuGridVo
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
                    DataYeuCauNhapKhoVatTuChiTiets = s.YeuCauNhapKhoVatTuChiTiets.Select(ct => new DataYeuCauNhapKhoVatTuChiTiet
                    {
                        Id = ct.Id,
                        KhoNhapSauKhiDuyetId = ct.KhoNhapSauKhiDuyetId,
                        HopDongThauVatTuId = ct.HopDongThauVatTuId
                    }).ToList()
                    //TenKho = string.Join("; ", s.YeuCauNhapKhoVatTuChiTiets.Select(z => z.KhoNhapSauKhiDuyet.Ten).Distinct()),

                    ////BVHD-3926
                    //TenNhaCungCap = string.Join("; ", s.YeuCauNhapKhoVatTuChiTiets.Select(z => z.HopDongThauVatTu.NhaThau.Ten).Distinct()),
                }).ToList();

            var dataKho = _khoRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var hopDongThauVatTuIds = allDataNhapKhoVatTuGripVo.SelectMany(o => o.DataYeuCauNhapKhoVatTuChiTiets).Select(ct => ct.HopDongThauVatTuId).Distinct().ToList();
            var dataHopDongThauVatTu = _hopDongThauVatTu.TableNoTracking
                .Where(o => hopDongThauVatTuIds.Contains(o.Id))
                .Select(o => new { o.Id, TenNhaThau = o.NhaThau.Ten })
                .ToList();
            foreach (var dataNhapKhoVatTuGripVo in allDataNhapKhoVatTuGripVo)
            {
                var khoNhapSauKhiDuyetIds = dataNhapKhoVatTuGripVo.DataYeuCauNhapKhoVatTuChiTiets
                    .Where(o => o.KhoNhapSauKhiDuyetId != null)
                    .Select(o => o.KhoNhapSauKhiDuyetId)
                    .Distinct().ToList();
                dataNhapKhoVatTuGripVo.TenKho = string.Join("; ", khoNhapSauKhiDuyetIds.Select(o => dataKho.FirstOrDefault(k => k.Id == o)?.Ten ?? ""));
                var hopDongThauIds = dataNhapKhoVatTuGripVo.DataYeuCauNhapKhoVatTuChiTiets
                    .Select(o => o.HopDongThauVatTuId)
                    .Distinct().ToList();
                dataNhapKhoVatTuGripVo.TenNhaCungCap = string.Join("; ", hopDongThauIds.Select(o => dataHopDongThauVatTu.FirstOrDefault(h => h.Id == o)?.TenNhaThau ?? ""));
            }

            if (!string.IsNullOrEmpty(queryObject.SearchString))
            {
                var searchString = queryObject.SearchString.Trim().ToLower().RemoveVietnameseDiacritics();
                allDataNhapKhoVatTuGripVo = allDataNhapKhoVatTuGripVo.Where(x =>
                    (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.ToLower().RemoveVietnameseDiacritics().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.SoPhieu) && x.SoPhieu.ToLower().RemoveVietnameseDiacritics().Contains(searchString))
                    || (!string.IsNullOrEmpty(x.TenNhaCungCap) && x.TenNhaCungCap.ToLower().RemoveVietnameseDiacritics().Contains(searchString)))
                    .ToList();
            }

            return new GridDataSource { Data = allDataNhapKhoVatTuGripVo.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = allDataNhapKhoVatTuGripVo.Count() };
        }

        public GridDataSource GetDataForGridAsyncOld(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            var queryObject = new NhapKhoVatTuSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoVatTuSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
            {
                queryObject.DaDuyet = true;
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
            }

            var queryDangChoDuyet = getDataYeuCauNhapKhoVatTu(null, queryInfo, queryObject);
            var queryTuChoiDuyet = getDataYeuCauNhapKhoVatTu(false, queryInfo, queryObject);
            var queryTuDaDuyet = getDataYeuCauNhapKhoVatTu(true, queryInfo, queryObject);

            var query = new List<NhapKhoVatTuGridVo>().AsQueryable();
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

            if (queryInfo.SortString != null
                && !queryInfo.SortString.Equals("TinhTrang asc")
                && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = 0 };

        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long nhapKhoVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;

            var query = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking
                .Include(p => p.VatTuBenhVien)
                .Where(p => p.YeuCauNhapKhoVatTuId == nhapKhoVatTuId)
               .Select(s => new NhapKhoVatTuChiTietGripVo()
               {
                   Id = s.Id,
                   TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                   NhaThauDisplay = s.HopDongThauVatTu.NhaThau.Ten,
                   TenHDThau = s.HopDongThauVatTu.HeThongTuPhatSinh != true ? s.HopDongThauVatTu.SoHopDong : string.Empty,
                   SoLo = s.Solo,
                   HanSuDung = s.HanSuDung,
                   MaVach = s.MaVach,
                   SL = s.SoLuongNhap,
                   GiaNhap = s.DonGiaNhap,
                   VAT = s.VAT,
                   ViTri = s.KhoViTri.Ten,
                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung != null ? s.VatTuBenhVien.LoaiSuDung.GetDescription() : "",
                   TiLeBHYTThanhToan = s.TiLeBHYTThanhToan,
               });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public List<VatTuNhapKhoTemplateVo> GetDropDownListVatTu(NhapKhoDuocPhamVatTuTheoHopDongThau nhapKhoInput, DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listVatTu = _vatTuBenhVienRepository.TableNoTracking
                    //.Include(x => x.VatTus).ThenInclude(p => p.HopDongThauVatTuChiTiets)
                    .ApplyLike(model.Query, x => x.VatTus.Ten, x => x.VatTus.NhaSanXuat, x => x.VatTus.Ma)
                    .Where(x => x.HieuLuc
                                && x.VatTus.HopDongThauVatTuChiTiets.Any(y => y.HopDongThauVatTuId == nhapKhoInput.HopDongThauVatTuId))
                    .OrderBy(x => x.VatTus.Ten)
                    .Take(model.Take)
                    .Select(item => new VatTuNhapKhoTemplateVo
                    {
                        Gia = item.VatTus.HopDongThauVatTuChiTiets.First(i => i.VatTuId == item.Id && i.HopDongThauVatTuId == nhapKhoInput.HopDongThauVatTuId).Gia,
                        DisplayName = item.VatTus.Ten,
                        KeyId = item.Id,
                        Ten = item.VatTus.Ten,
                        Ma = item.VatTus.Ma,
                        NhaSanXuat = item.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTus.NuocSanXuat,
                        DVT = item.VatTus.DonViTinh,
                        HeSoDinhMucDonViTinh = item.VatTus.HeSoDinhMucDonViTinh,
                        SoLuongChuaNhap = item.VatTus.HopDongThauVatTuChiTiets.Where(x => x.VatTuId == item.Id && x.HopDongThauVatTuId == nhapKhoInput.HopDongThauVatTuId).Select(i => i.SoLuong - i.SoLuongDaCap).Sum(),
                        NhomVatTuId = item.VatTus.NhomVatTuId,
                        TenNhomVatTu = item.VatTus.NhomVatTu.Ten,
                        SLTon = item.NhapKhoVatTuChiTiets.Any() ? item.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == nhapKhoInput.KhoId
                     && nkct.LaVatTuBHYT == false && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    }).ToList();
                return listVatTu;
            }
            else
            {
                var lstColumnNameSearch = new List<string>();
                lstColumnNameSearch.Add("Ten");
                lstColumnNameSearch.Add("Ma");
                //lstColumnNameSearch.Add("NhaSanXuat");

                var lstVatTuBenhVienId = _vatTuRepository
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                    .Select(p => p.Id)
                    .Take(model.Take).ToList();

                var listDuocPham = _vatTuBenhVienRepository
                    .TableNoTracking
                    .Where(x => x.HieuLuc
                                && x.VatTus.HopDongThauVatTuChiTiets.Any(y => y.HopDongThauVatTuId == nhapKhoInput.HopDongThauVatTuId)
                                && lstVatTuBenhVienId.Any(a => a == x.Id))

                    //.ApplyFulltext(model.Query, "VatTu", lstColumnNameSearch)
                    //.Include(x => x.DuocPham).ThenInclude(p => p.HopDongThauDuocPhamChiTiets)
                    .Take(model.Take)
                    .Select(item => new VatTuNhapKhoTemplateVo
                    {
                        Gia = item.VatTus.HopDongThauVatTuChiTiets.First(i => i.VatTuId == item.Id && i.HopDongThauVatTuId == nhapKhoInput.HopDongThauVatTuId).Gia,
                        DisplayName = item.VatTus.Ten,
                        KeyId = item.Id,
                        Ten = item.VatTus.Ten,
                        Ma = item.VatTus.Ma,
                        NhaSanXuat = item.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTus.NuocSanXuat,
                        DVT = item.VatTus.DonViTinh,
                        HeSoDinhMucDonViTinh = item.VatTus.HeSoDinhMucDonViTinh,
                        SoLuongChuaNhap = item.VatTus.HopDongThauVatTuChiTiets.Where(x => x.VatTuId == item.Id && x.HopDongThauVatTuId == nhapKhoInput.HopDongThauVatTuId).Select(i => i.SoLuong - i.SoLuongDaCap).Sum(),
                        NhomVatTuId = item.VatTus.NhomVatTuId,
                        TenNhomVatTu = item.VatTus.NhomVatTu.Ten,
                        SLTon = item.NhapKhoVatTuChiTiets.Any() ? item.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == nhapKhoInput.KhoId
                  && nkct.LaVatTuBHYT == false && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    })
                    .OrderBy(x => lstVatTuBenhVienId.IndexOf(x.KeyId))
                    .ToList();
                return listDuocPham;
            }
        }

        public List<VatTuNhapKhoTemplateVo> GetDropDownListVatTuFromNhaThau(DropDownListRequestModel model)
        {
            List<VatTuNhapKhoTemplateVo> result = null;
            var khoId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listVatTu = _vatTuBenhVienRepository.TableNoTracking
                    .Where(x => x.HieuLuc)
                    .OrderBy(x => x.VatTus.Ten)
                    .Select(item => new VatTuNhapKhoTemplateVo
                    {
                        DisplayName = item.VatTus.Ten,
                        KeyId = item.Id,
                        Ten = item.VatTus.Ten,
                        Ma = item.VatTus.Ma,
                        DVT = item.VatTus.DonViTinh,
                        NhomVatTuId = item.VatTus.NhomVatTuId,
                        TenNhomVatTu = item.VatTus.NhomVatTu.Ten,
                        NhaSanXuat = item.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTus.NuocSanXuat,
                        SLTon = item.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoId
                        && nkct.LaVatTuBHYT == false && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2),
                    })
                    .ApplyLike(model.Query, x => x.Ten, x => x.DVT, x => x.Ma)
                    .Take(model.Take)
                    .ToList();
                result = listVatTu;
            }
            else
            {
                var lstColumnNameSearch = new List<string>
                {
                   nameof(Core.Domain.Entities.VatTus.VatTu.Ten),
                   nameof(Core.Domain.Entities.VatTus.VatTu.Ma)
                };

                //var listVatTu = _vatTuRepository
                //    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                //    .Where(x => x.VatTuBenhVien.HieuLuc)
                //    .Take(model.Take)
                //    .Select(item => new VatTuNhapKhoTemplateVo
                //    {
                //        DisplayName = item.Ten,
                //        KeyId = item.VatTuBenhVien.Id,
                //        Ten = item.Ten,
                //        DVT = item.DonViTinh,
                //    }).ToList();
                //result = listVatTu;
                var lstVatTuId = _vatTuRepository
                     .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                     .Select(s => s.Id).ToList();

                var dct = lstVatTuId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                var listVatTu = _vatTuRepository
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                    .Where(x => x.VatTuBenhVien.HieuLuc)
                    .Select(item => new VatTuNhapKhoTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.VatTuBenhVien.Id,
                        Ten = item.Ten,
                        Ma = item.Ma,
                        DVT = item.DonViTinh,
                        NhomVatTuId = item.NhomVatTuId,
                        NhaSanXuat = item.NhaSanXuat,
                        NuocSanXuat = item.NuocSanXuat,
                        TenNhomVatTu = item.NhomVatTu.Ten,
                        SLTon = item.VatTuBenhVien != null && item.VatTuBenhVien.NhapKhoVatTuChiTiets.Any() ? item.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoId
                         && nkct.LaVatTuBHYT == false && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    })
                    .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                    .Take(model.Take).ToList();
                result = listVatTu;
            }
            if (model.Id > 0 && result.All(o => o.KeyId != model.Id))
            {
                var item1 = _vatTuRepository.TableNoTracking
                    .Where(p => p.Id == model.Id)
                    .Include(p => p.HopDongThauVatTuChiTiets)
                    .Where(x => x.VatTuBenhVien.HieuLuc)
                    .Select(item => new VatTuNhapKhoTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.VatTuBenhVien.Id,
                        Ten = item.Ten,
                        Ma = item.Ma,
                        DVT = item.DonViTinh,
                        NhomVatTuId = item.NhomVatTuId,
                        TenNhomVatTu = item.NhomVatTu.Ten,
                        NhaSanXuat = item.NhaSanXuat,
                        NuocSanXuat = item.NuocSanXuat,
                        SLTon = item.VatTuBenhVien != null && item.VatTuBenhVien.NhapKhoVatTuChiTiets.Any() ? item.VatTuBenhVien.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoId
                        && nkct.LaVatTuBHYT == false && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                    });
                if (item1.Any())
                {
                    result.AddRange(item1);
                }
            }
            return result;
        }

        public async Task<long> GetKhoTongVatTu2()
        {
            return (await _khoRepository.TableNoTracking.FirstOrDefaultAsync(p => p.LoaiKho == Core.Domain.Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2))?.Id ?? 0;
        }

        public Task<List<LookupItemVo>> GetListLoaiSuDung(LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.LoaiSuDung>().Select(item => new LookupItemVo()
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item)
            }).ToList();

            return Task.FromResult(listEnum);
        }

        public async Task<List<NhaThauHopDongTemplateVo>> GetListNhaThauNhapKho(LookupQueryInfo model)
        {
            var listKhoa = await _hopDongThauVatTu.TableNoTracking.Include(p => p.NhaThau)
                .ApplyLike(model.Query, g => g.NhaThau.Ten, g => g.NhaThau.DiaChi, g => g.SoHopDong)
                .Where(x => x.NgayHieuLuc.Date <= DateTime.Now.Date && DateTime.Now.Date <= x.NgayHetHan.Date)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoa.Select(item => new NhaThauHopDongTemplateVo
            {
                DisplayName = item.SoHopDong,
                KeyId = item.Id,
                DiaChi = item.NhaThau.DiaChi,
                Ten = item.NhaThau.Ten,
                SoHopDong = item.SoHopDong
            }).ToList();

            return query;
        }

        public async Task<decimal> GetPriceOnContract(long hopDongThauId, long vatTuId)
        {
            var vtId = _vatTuRepository.TableNoTracking.FirstOrDefault(p => p.VatTuBenhVien.Id == vatTuId)?.Id;
            if (vtId == null || vtId == 0) return 0;

            return (await _hopDongThauVatTuChiTiet.TableNoTracking.FirstOrDefaultAsync(p => p.HopDongThauVatTuId == hopDongThauId
                && p.VatTuId == vtId))?.Gia ?? 0;
        }

        public async Task UpdateNhapKhoVatTuChiTiet(long id, double soLuongDaXuat)
        {
            var nhapKhoChiTiet = await _nhapKhoVatTuChiTietRepository.Table.FirstOrDefaultAsync(p => p.Id == id);
            if (nhapKhoChiTiet != null)
            {
                nhapKhoChiTiet.SoLuongDaXuat = nhapKhoChiTiet.SoLuongDaXuat + soLuongDaXuat;
                await _nhapKhoVatTuChiTietRepository.UpdateAsync(nhapKhoChiTiet);
            }
        }

        public GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new NhapKhoVatTuSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<NhapKhoVatTuSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DaDuyet == false && queryObject.DangChoDuyet == false && queryObject.TuChoiDuyet == false)
            {
                queryObject.DaDuyet = true;
                queryObject.DangChoDuyet = true;
                queryObject.TuChoiDuyet = true;
            }

            var queryDangChoDuyet = getDataYeuCauNhapKhoVatTu(null, queryInfo, queryObject);
            var queryTuChoiDuyet = getDataYeuCauNhapKhoVatTu(false, queryInfo, queryObject);
            var queryTuDaDuyet = getDataYeuCauNhapKhoVatTu(true, queryInfo, queryObject);

            var query = new List<NhapKhoVatTuGridVo>().AsQueryable();
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

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long nhapKhoVatTuId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;

            var query = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking
                .Include(p => p.VatTuBenhVien)
                .Where(p => p.YeuCauNhapKhoVatTuId == nhapKhoVatTuId)
               .Select(s => new NhapKhoVatTuChiTietGripVo()
               {
                   Id = s.Id,
                   TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                   TenHDThau = s.HopDongThauVatTu.HeThongTuPhatSinh != true ? s.HopDongThauVatTu.SoHopDong : string.Empty,
                   SoLo = s.Solo,
                   HanSuDung = s.HanSuDung,
                   MaVach = s.MaVach,
                   SL = s.SoLuongNhap,
                   GiaNhap = s.DonGiaNhap,
                   VAT = s.VAT,
                   ViTri = s.KhoViTri.Ten,
                   LoaiSuDung = s.VatTuBenhVien.LoaiSuDung != null ? s.VatTuBenhVien.LoaiSuDung.GetDescription() : "",
                   MaRef = s.MaRef
               });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<YeuCauNhapKhoVatTuChiTietGridVo>> YeuCauNhapKhoVatTuChiTiets(long Id, string kyHieuHD, string soChungTu, List<YeuCauNhapKhoVatTuChiTietGridVo> models)
        {
            var yeuCauNhapKhoDuocPhamChiTietGridVos = new List<YeuCauNhapKhoVatTuChiTietGridVo>();
            var yeuCauNhapKhoVatTus = await _yeuCauNhapKhoVatTuRepository.TableNoTracking.Include(x => x.YeuCauNhapKhoVatTuChiTiets).ThenInclude(z => z.HopDongThauVatTu).Where(z => z.DuocKeToanDuyet != false && z.Id != Id).ToListAsync();
            foreach (var model in models)
            {
                if (yeuCauNhapKhoVatTus.Any(z => z.SoChungTu.RemoveUniKeyAndToLower().Contains(soChungTu.RemoveUniKeyAndToLower()) && z.KyHieuHoaDon.RemoveUniKeyAndToLower().Contains(kyHieuHD.RemoveUniKeyAndToLower()) && z.YeuCauNhapKhoVatTuChiTiets.Any(y => y.HopDongThauVatTu.NhaThauId == model.NhaThauId)))
                {
                    yeuCauNhapKhoDuocPhamChiTietGridVos.Add(model);
                }
            }
            return yeuCauNhapKhoDuocPhamChiTietGridVos;
        }

        public async Task<YeuCauNhapKhoVatTuChiTietGridVo> GetVatTuGrid(YeuCauNhapKhoVatTuChiTietGridVo model)
        {
            model.HopDongThauDisplay = model.HopDongThauVatTuId != null ? (await _hopDongThauVatTuRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.HopDongThauVatTuId))?.SoHopDong : string.Empty;
            model.NhaThauDisplay = model.NhaThauId != null ? (await _nhaThau.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.NhaThauId))?.Ten :
                (await _hopDongThauVatTuRepository.TableNoTracking.Include(e => e.NhaThau).FirstOrDefaultAsync(e => e.Id == model.HopDongThauVatTuId))?.NhaThau.Ten;
            model.NhaThauId = model.NhaThauId ?? (await _hopDongThauVatTuRepository.TableNoTracking.Include(e => e.NhaThau).FirstOrDefaultAsync(e =>
                                  e.Id == model.HopDongThauVatTuId))?.NhaThauId;
            model.VatTuDisplay = (await _vatTuBenhVienRepository.TableNoTracking.Include(p => p.VatTus).FirstOrDefaultAsync(p => p.Id == model.VatTuBenhVienId))?.VatTus.Ten;
            model.LoaiDisplay = model.LaVatTuBHYT == true ? "BHYT" : "Không BHYT";
            //model.LoaiSuDung = model.LoaiSuDung;
            model.LoaiSuDungDisplay = model.LoaiSuDung.GetDescription();
            model.Solo = model.Solo;
            model.HanSuDungDisplay = (model.HanSuDung ?? DateTime.Now).ApplyFormatDate();
            model.MaVach = model.MaVach;
            model.MaRef = model.MaRef;
            model.SoLuongNhapDisplay = ((long)model.SoLuongNhap).ApplyNumber();
            model.KhoNhapSauKhiDuyetId = model.KhoNhapSauKhiDuyetId;
            model.NguoiNhapSauKhiDuyetId = model.NguoiNhapSauKhiDuyetId;
            model.TenKhoNhapSauKhiDuyet = (await _khoRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.KhoNhapSauKhiDuyetId))?.Ten;
            model.TenNguoiNhapSauKhiDuyet = (await _userRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.NguoiNhapSauKhiDuyetId))?.HoTen;

            model.DVT = (await _vatTuBenhVienRepository.TableNoTracking.Include(p => p.VatTus).FirstOrDefaultAsync(p => p.Id == model.VatTuBenhVienId))?.VatTus.DonViTinh;
            model.ThanhTienTruocVat = model.ThanhTienTruocVat;
            model.ThanhTienSauVat = model.LoaiNhap == 1 && model.LaVatTuBHYT == true ? model.ThanhTienTruocVat : model.ThanhTienSauVat;
            model.GhiChu = model.GhiChu;
            return model;
        }

        public async Task<Enums.LoaiSuDung?> SuggestLoaiSuDung(long vatTuId)
        {
            var entity = await _vatTuBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == vatTuId);
            if (entity != null)
            {
                return entity.LoaiSuDung;
            }

            return null;
        }

        public async Task<NhapKhoVatTuChiTiet> GetNhapKhoVatTuChiTietById(long id)
        {
            var result = await _nhapKhoVatTuChiTietRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return result;
        }


        #region private class
        private IQueryable<NhapKhoVatTuGridVo> getDataYeuCauNhapKhoVatTu(bool? duocKeToanDuyet, QueryInfo queryInfo, NhapKhoVatTuSearch queryObject)
        {
            var result = _yeuCauNhapKhoVatTuRepository.TableNoTracking
                .Where(p => p.DuocKeToanDuyet == duocKeToanDuyet && p.Kho.LoaiKho != Enums.EnumLoaiKhoDuocPham.KhoHanhChinh)
                .Select(s => new NhapKhoVatTuGridVo
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
                    TenKho = string.Join("; ", s.YeuCauNhapKhoVatTuChiTiets.Select(z => z.KhoNhapSauKhiDuyet.Ten).Distinct()),

                    //BVHD-3926
                    TenNhaCungCap = string.Join("; ", s.YeuCauNhapKhoVatTuChiTiets.Select(z => z.HopDongThauVatTu.NhaThau.Ten).Distinct()),
                });
            //result = result.ApplyLike(queryObject.SearchString, g => g.SoChungTu, g => g.TenNguoiNhap, g => g.TenNguoiGiao, g => g.NguoiDuyet, g => g.SoPhieu, g => g.TenKho);
            //result = result.OrderBy(queryInfo.SortString).ThenBy(z => z.NgayNhap);

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
                //if (queryObject.RangeNhap != null && queryObject.RangeNhap.startDate != null && queryObject.RangeNhap.endDate != null)
                //{
                //    var tuNgay = queryObject.RangeNhap.startDate.GetValueOrDefault().Date;
                //    var denNgay = queryObject.RangeNhap.endDate.GetValueOrDefault().Date;
                //    result = result.Where(p => tuNgay <= p.NgayNhap.Value.Date && denNgay >= p.NgayNhap.Value.Date);
                //}
                //if (queryObject.RangeDuyet != null && queryObject.RangeDuyet.startDate != null && queryObject.RangeDuyet.endDate != null)
                //{
                //    var tuNgay = queryObject.RangeDuyet.startDate.GetValueOrDefault().Date;
                //    var denNgay = queryObject.RangeDuyet.endDate.GetValueOrDefault().Date;
                //    result = result.Where(p => tuNgay <= p.NgayDuyet.Value.Date && denNgay >= p.NgayDuyet.Value.Date);
                //}
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
        #endregion private class

        public string InPhieuYeuCauNhapKhoVatTu(InPhieuNhapKhoVatTu inPhieuNhapKhoVatTu)
        {
            var content = string.Empty;
            var contentBenhVien = string.Empty;
            var contentThongTu = string.Empty;
            if (inPhieuNhapKhoVatTu.CoTheoThongTu)
            {
                var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuNhapKhoVatTu"));
                var yeuCauNhapKhoVatTuChiTiets = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking
                                              .Where(p => p.YeuCauNhapKhoVatTuId == inPhieuNhapKhoVatTu.YeuCauNhapKhoVatTuId)
                                              .Select(s => new YeuCauNhapKhoVatTuChiTietData
                                              {
                                                  Ten = s.VatTuBenhVien.VatTus.Ten + (s.VatTuBenhVien.VatTus.NhaSanXuat != null && s.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                                                                                       (s.VatTuBenhVien.VatTus.NuocSanXuat != null && s.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                                  DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                  SoLo = s.Solo,
                                                  VAT = s.VAT,
                                                  LaVatTuBHYT = s.LaVatTuBHYT,
                                                  SLTheoChungTu = s.SoLuongNhap,
                                                  DonGia = s.DonGiaNhap,
                                                  HanSuDung = s.HanSuDung,
                                                  TenNguoiNhap = s.YeuCauNhapKhoVatTu.NguoiNhap.User.HoTen,
                                                  NgayNhap = s.YeuCauNhapKhoVatTu.NgayNhap,
                                                  NCC = s.HopDongThauVatTu.NhaThau.Ten,
                                                  TheoSoHoaDon = s.YeuCauNhapKhoVatTu.SoChungTu,
                                                  KyHieuHoaDon = s.YeuCauNhapKhoVatTu.KyHieuHoaDon,
                                                  NgayHoaDon = s.YeuCauNhapKhoVatTu.NgayHoaDon,
                                                  KhoNhap = s.KhoNhapSauKhiDuyet.Ten,
                                                  KhoNhapSauDuyetId = s.KhoNhapSauKhiDuyetId,
                                                  NguoiNhan = s.NguoiNhapSauKhiDuyet.HoTen,
                                                  SoPhieu = s.YeuCauNhapKhoVatTu.SoPhieu,
                                                  ThanhTienSauVAT = s.ThanhTienSauVat,
                                                  ThanhTienTruocVAT = s.ThanhTienTruocVat,
                                                  ThueVatLamTron = s.ThueVatLamTron
                                              }).OrderBy(c => c.Ten).ToList();

                var khoNhapSauDuyetIds = yeuCauNhapKhoVatTuChiTiets.Select(c => c.KhoNhapSauDuyetId).Distinct().ToList();
                var range = khoNhapSauDuyetIds.Count();
                if (range <= 1)
                {
                    var loaiBHYT = yeuCauNhapKhoVatTuChiTiets.Any(z => z.LaVatTuBHYT) ? "BHYT" : "";
                    var loaiKhongBHYT = yeuCauNhapKhoVatTuChiTiets.Any(z => !z.LaVatTuBHYT) ? "Viện phí" : "";
                    var loai = !string.IsNullOrEmpty(loaiBHYT) && !string.IsNullOrEmpty(loaiKhongBHYT) ? loaiBHYT + ", " + loaiKhongBHYT
                                : (!string.IsNullOrEmpty(loaiBHYT) && string.IsNullOrEmpty(loaiKhongBHYT) ? loaiBHYT
                                 : (string.IsNullOrEmpty(loaiBHYT) && !string.IsNullOrEmpty(loaiKhongBHYT) ? loaiKhongBHYT : ""));
                    decimal thanhTienTruocVAT = yeuCauNhapKhoVatTuChiTiets.Sum(z => z.ThanhTienTruocVAT);
                    decimal tongThanhTienSauVAT = yeuCauNhapKhoVatTuChiTiets.Sum(z => z.ThueVatLamTron) ?? 0;

                    var STT = 1;
                    var duocPhamHoacVatTus = string.Empty;
                    foreach (var item in yeuCauNhapKhoVatTuChiTiets)
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

                    var data = new YeuCauNhapKhoVatTuData
                    {
                        NgayNhapKho = yeuCauNhapKhoVatTuChiTiets.First().NgayNhapDisplay,
                        LoaiBHYT = loai,
                        NCC = yeuCauNhapKhoVatTuChiTiets.First().NCC,
                        SoHoaDon = yeuCauNhapKhoVatTuChiTiets.First().KyHieuHoaDonHienThi,
                        NgayHoaDon = yeuCauNhapKhoVatTuChiTiets.First().NgayHoaDonDisplay,
                        KhoNhap = yeuCauNhapKhoVatTuChiTiets.First().KhoNhap,
                        NguoiNhan = yeuCauNhapKhoVatTuChiTiets.First().NguoiNhan,
                        DuocPhamHoacVatTus = duocPhamHoacVatTus,
                        TienHangDecimal = thanhTienTruocVAT,
                        ThueVATDecimal = tongThanhTienSauVAT,
                        VAT = string.Join("/ ", yeuCauNhapKhoVatTuChiTiets.Select(z => z.VAT + "%").Distinct()),
                        ChietKhau = "0,00",
                        NguoiLap = yeuCauNhapKhoVatTuChiTiets.First().TenNguoiNhap,
                        SoPhieu = yeuCauNhapKhoVatTuChiTiets.First().SoPhieu,
                    };
                    contentBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                }
                else
                {
                    var count = 1;//dùng để check phiếu in cuối cùng ko cần thêm page break
                    var contentChild = string.Empty;
                    foreach (var khoNhapSauDuyetId in khoNhapSauDuyetIds)
                    {
                        var yeuCauNhapKhoVatTuChiTietTheoKhoDuyets = yeuCauNhapKhoVatTuChiTiets.Where(c => c.KhoNhapSauDuyetId == khoNhapSauDuyetId).OrderBy(c => c.Ten).ToList();

                        var loaiBHYT = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.Any(z => z.LaVatTuBHYT) ? "BHYT" : "";
                        var loaiKhongBHYT = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.Any(z => !z.LaVatTuBHYT) ? "Viện phí" : "";
                        var loai = !string.IsNullOrEmpty(loaiBHYT) && !string.IsNullOrEmpty(loaiKhongBHYT) ? loaiBHYT + ", " + loaiKhongBHYT
                                    : (!string.IsNullOrEmpty(loaiBHYT) && string.IsNullOrEmpty(loaiKhongBHYT) ? loaiBHYT
                                     : (string.IsNullOrEmpty(loaiBHYT) && !string.IsNullOrEmpty(loaiKhongBHYT) ? loaiKhongBHYT : ""));
                        decimal thanhTienTruocVAT = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.Sum(z => z.ThanhTienTruocVAT);
                        decimal tongThanhTienSauVAT = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.Sum(z => z.ThueVatLamTron) ?? 0;
                        var STT = 1;
                        var duocPhamHoacVatTus = string.Empty;
                        foreach (var item in yeuCauNhapKhoVatTuChiTietTheoKhoDuyets)
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

                        var data = new YeuCauNhapKhoVatTuData
                        {
                            NgayNhapKho = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().NgayNhapDisplay,
                            LoaiBHYT = loai,
                            NCC = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().NCC,
                            SoHoaDon = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().KyHieuHoaDonHienThi,
                            NgayHoaDon = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().NgayHoaDonDisplay,
                            KhoNhap = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().KhoNhap,
                            NguoiNhan = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().NguoiNhan,
                            DuocPhamHoacVatTus = duocPhamHoacVatTus,
                            TienHangDecimal = thanhTienTruocVAT,
                            ThueVATDecimal = tongThanhTienSauVAT,
                            VAT = string.Join("/ ", yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.Select(z => z.VAT + "%").Distinct()),
                            ChietKhau = "0,00",
                            NguoiLap = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().TenNguoiNhap,
                            SoPhieu = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().SoPhieu,
                        };
                        contentChild = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                        if (count < range && contentChild != "")
                        {
                            contentChild = contentChild + "<div class=\"pagebreak\"> </div>";
                        }
                        count++;
                        contentBenhVien += contentChild;
                    }
                }

            }
            if (inPhieuNhapKhoVatTu.CoTheoBenhVien)// bị ngược (do đặt sai tên template) sai do UI truyền nhầm biến CoTheoBenhVien chính là biến CoTheoThongTu và CoTheoThongTu chính CoTheoBenhVien.
            {
                var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuNhapKhoVatTuThongTu"));
                var ngayNhap = _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(z => z.Id == inPhieuNhapKhoVatTu.YeuCauNhapKhoVatTuId).Select(z => z.NgayNhap).First();
                var yeuCauNhapKhoVatTuIds = _yeuCauNhapKhoVatTuRepository.TableNoTracking.Where(z => z.NgayNhap.Month == ngayNhap.Month && z.NgayNhap.Year == ngayNhap.Year).OrderBy(z => z.NgayNhap).Select(z => z.Id).ToList();
                var index = yeuCauNhapKhoVatTuIds.FindIndex(z => z == inPhieuNhapKhoVatTu.YeuCauNhapKhoVatTuId) + 1;
                var soPhieuFomat = string.Empty;
                if (index < 10)
                {
                    soPhieuFomat = "1" + ngayNhap.Year.ToString().Substring(2, 2) + (ngayNhap.Month < 10 ? "0" + ngayNhap.Month.ToString() : ngayNhap.Month.ToString()) + index.ToString().PadLeft(5, '0'); // 1 => 00001
                }
                else if (10 <= index && index < 100)
                {
                    soPhieuFomat = "1" + ngayNhap.Year.ToString().Substring(2, 2) + (ngayNhap.Month < 10 ? "0" + ngayNhap.Month.ToString() : ngayNhap.Month.ToString()) + index.ToString().PadLeft(4, '0'); // 10 => 00010
                }
                else if (100 <= index && index < 1000)
                {
                    soPhieuFomat = "1" + ngayNhap.Year.ToString().Substring(2, 2) + (ngayNhap.Month < 10 ? "0" + ngayNhap.Month.ToString() : ngayNhap.Month.ToString()) + index.ToString().PadLeft(3, '0'); // 100 => 00100
                }
                else if (1000 <= index && index < 10000)
                {
                    soPhieuFomat = "1" + ngayNhap.Year.ToString().Substring(2, 2) + (ngayNhap.Month < 10 ? "0" + ngayNhap.Month.ToString() : ngayNhap.Month.ToString()) + index.ToString().PadLeft(2, '0'); // 1000 => 01000
                }
                else
                {
                    soPhieuFomat = "1" + ngayNhap.Year.ToString().Substring(2, 2) + (ngayNhap.Month < 10 ? "0" + ngayNhap.Month.ToString() : ngayNhap.Month.ToString()) + index.ToString(); // 10000 => 10000
                }
                var yeuCauNhapKhoVatTuChiTiets = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking
                                              .Where(p => p.YeuCauNhapKhoVatTuId == inPhieuNhapKhoVatTu.YeuCauNhapKhoVatTuId)
                                              .Select(s => new YeuCauNhapKhoVatTuChiTietDataThongTu
                                              {
                                                  Ma = s.VatTuBenhVien.Ma,
                                                  Ten = s.VatTuBenhVien.VatTus.Ten + (s.VatTuBenhVien.VatTus.NhaSanXuat != null && s.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                                                                                       (s.VatTuBenhVien.VatTus.NuocSanXuat != null && s.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                                                  DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                                                  SLTheoChungTu = s.SoLuongNhap,
                                                  DonGia = s.DonGiaNhap,
                                                  TenNguoiNhap = s.YeuCauNhapKhoVatTu.NguoiNhap.User.HoTen,
                                                  NgayNhap = s.YeuCauNhapKhoVatTu.NgayNhap,
                                                  TheoSoHoaDon = s.YeuCauNhapKhoVatTu.SoChungTu,
                                                  KyHieuHoaDon = s.YeuCauNhapKhoVatTu.KyHieuHoaDon,
                                                  KhoNhap = s.KhoNhapSauKhiDuyet.Ten,
                                                  KhoNhapSauDuyetId = s.KhoNhapSauKhiDuyetId,
                                                  NguoiNhap = s.NguoiNhapSauKhiDuyet.HoTen,
                                                  SoPhieu = s.YeuCauNhapKhoVatTu.SoPhieu,
                                                  KhoaPhong = s.KhoNhapSauKhiDuyet.KhoaPhong != null ? s.KhoNhapSauKhiDuyet.KhoaPhong.Ten : "",
                                                  DiaChiBoPhan = s.KhoNhapSauKhiDuyet.KhoaPhong != null ? s.KhoNhapSauKhiDuyet.KhoaPhong.Ten : "",
                                                  ThanhTienTruocVAT = s.ThanhTienTruocVat,

                                                  //BVHD-3857
                                                  DonViGiaoHang = s.HopDongThauVatTu.NhaThau.Ten,
                                                  SoHoaDonPhieuNhap = s.YeuCauNhapKhoVatTu.SoChungTu,
                                                  NgayHoaDon = s.YeuCauNhapKhoVatTu.NgayHoaDon
                                              }).OrderBy(c => c.Ten).ToList();
                var khoNhapSauDuyetIds = yeuCauNhapKhoVatTuChiTiets.Select(c => c.KhoNhapSauDuyetId).Distinct().ToList();
                var range = khoNhapSauDuyetIds.Count();
                if (range <= 1)
                {
                    decimal thanhTien = yeuCauNhapKhoVatTuChiTiets.Sum(z => z.ThanhTienTruocVAT);
                    var STT = 1;
                    var duocPhamHoacVatTus = string.Empty;
                    foreach (var item in yeuCauNhapKhoVatTuChiTiets)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SLTheoChungTu.ApplyNumber()
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SLThucNhap.ApplyNumber()
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTienTruocVAT.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "</tr>";
                        STT++;
                    }
                    duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td colspan='3' style = 'border: 1px solid #020000;text-align: center;'>" + "<b>TỔNG CỘNG</b>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + thanhTien.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "</tr>";

                    var data = new YeuCauNhapKhoVatTuDataThongTu
                    {
                        NgayNhapKho = yeuCauNhapKhoVatTuChiTiets.First().NgayNhapDisplay,
                        LogoUrl = inPhieuNhapKhoVatTu.HostingName + "/assets/img/logo-bacha-full.png",
                        SoHoaDon = yeuCauNhapKhoVatTuChiTiets.First().KyHieuHoaDonHienThi,
                        KhoNhap = yeuCauNhapKhoVatTuChiTiets.First().KhoNhap,
                        NguoiNhap = yeuCauNhapKhoVatTuChiTiets.First().NguoiNhap,
                        DuocPhamHoacVatTus = duocPhamHoacVatTus,
                        SoPhieu = soPhieuFomat,
                        DiaChiBoPhan = yeuCauNhapKhoVatTuChiTiets.First().DiaChiBoPhan,
                        KhoaPhong = yeuCauNhapKhoVatTuChiTiets.First().KhoaPhong,
                        ThanhTien = thanhTien,

                        //BVHD-3857
                        DonViGiaoHang = yeuCauNhapKhoVatTuChiTiets.First().DonViGiaoHang,
                        SoHoaDonPhieuNhap = yeuCauNhapKhoVatTuChiTiets.First().SoHoaDonPhieuNhap,
                        NgayHoaDon = yeuCauNhapKhoVatTuChiTiets.First().NgayHoaDon
                    };
                    contentThongTu = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                }
                else
                {
                    var count = 1;//dùng để check phiếu in cuối cùng ko cần thêm page break
                    var contentChild = string.Empty;
                    foreach (var khoNhapSauDuyetId in khoNhapSauDuyetIds)
                    {
                        var yeuCauNhapKhoVatTuChiTietTheoKhoDuyets = yeuCauNhapKhoVatTuChiTiets.Where(c => c.KhoNhapSauDuyetId == khoNhapSauDuyetId).OrderBy(c => c.Ten).ToList();
                        decimal thanhTien = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.Sum(z => z.ThanhTienTruocVAT);
                        var STT = 1;
                        var duocPhamHoacVatTus = string.Empty;
                        foreach (var item in yeuCauNhapKhoVatTuChiTietTheoKhoDuyets)
                        {
                            duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                       + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                       + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SLTheoChungTu.ApplyNumber()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.SLThucNhap.ApplyNumber()
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTienTruocVAT.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                       + "</tr>";
                            STT++;
                        }
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                       + "<td colspan='3' style = 'border: 1px solid #020000;text-align: center;'>" + "<b>TỔNG CỘNG</b>"
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                       + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + thanhTien.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                       + "</tr>";

                        var data = new YeuCauNhapKhoVatTuDataThongTu
                        {
                            NgayNhapKho = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().NgayNhapDisplay,
                            LogoUrl = inPhieuNhapKhoVatTu.HostingName + "/assets/img/logo-bacha-full.png",
                            SoHoaDon = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().KyHieuHoaDonHienThi,
                            KhoNhap = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().KhoNhap,
                            NguoiNhap = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().NguoiNhap,
                            DuocPhamHoacVatTus = duocPhamHoacVatTus,
                            SoPhieu = soPhieuFomat,
                            DiaChiBoPhan = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().DiaChiBoPhan,
                            KhoaPhong = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().KhoaPhong,
                            ThanhTien = thanhTien,

                            //BVHD-3857
                            DonViGiaoHang = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().DonViGiaoHang,
                            SoHoaDonPhieuNhap = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().SoHoaDonPhieuNhap,
                            NgayHoaDon = yeuCauNhapKhoVatTuChiTietTheoKhoDuyets.First().NgayHoaDon
                        };
                        contentChild = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
                        if (count < range && contentChild != "")
                        {
                            contentChild = contentChild + "<div class=\"pagebreak\"> </div>";
                        }
                        count++;
                        contentThongTu += contentChild;
                    }
                }

            }
            if (contentBenhVien != "")
            {
                contentBenhVien = contentBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThongTu != "")
            {
                contentThongTu = contentThongTu + "<div class=\"pagebreak\"> </div>";
            }
            content = contentBenhVien + contentThongTu;
            return content;
        }

        public LookupItemVo KhoVatTuYTe()
        {
            var khoVatTuYte = new LookupItemVo();
            var entityKhoVTYT = _khoRepository.TableNoTracking
                                              .Where(c => c.LoaiVatTu == true && c.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                                              .FirstOrDefault();
            if (entityKhoVTYT != null)
            {
                khoVatTuYte.KeyId = entityKhoVTYT.Id;
                khoVatTuYte.DisplayName = entityKhoVTYT.Ten;
            }
            return khoVatTuYte;
        }

        public bool KiemTraNgayHetHanHopDong(long? hopDongThauVatTuId)
        {
            return _hopDongThauVatTuRepository.TableNoTracking.Include(p => p.NhaThau)
                 .Any(x => x.Id == hopDongThauVatTuId &&
                 x.NgayHieuLuc.Date <= DateTime.Now.Date && DateTime.Now.Date <= x.NgayHetHan.Date);
        }

        public NhapKhoDuocPhamTemplateVo SoLuongHopDongThauVatTu(long? hopDongThauVatTuId, long? vatTuBenhVienId, long khoId, bool? laVatTuBHYT)
        {
            var vatTu = _vatTuBenhVienRepository.TableNoTracking
                   .Include(x => x.VatTus).ThenInclude(p => p.HopDongThauVatTuChiTiets)
                   .Where(x =>  x.Id == vatTuBenhVienId &&  x.HieuLuc
                               && x.VatTus.HopDongThauVatTuChiTiets.Any(y => y.HopDongThauVatTuId == hopDongThauVatTuId))
                   .Select(item => new NhapKhoDuocPhamTemplateVo
                   {
                       SoLuongChuaNhap = item.VatTus.HopDongThauVatTuChiTiets.Where(x => x.HopDongThauVatTuId == hopDongThauVatTuId
                       && x.VatTuId == item.Id).Select(i => i.SoLuong - i.SoLuongDaCap).Sum(),
                       SLTon = item.NhapKhoVatTuChiTiets.Any() ? item.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoId
                      && nkct.LaVatTuBHYT == laVatTuBHYT && nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat).MathRoundNumber(2) : 0,
                   }).FirstOrDefault();

            return vatTu;
        }
    }
}
