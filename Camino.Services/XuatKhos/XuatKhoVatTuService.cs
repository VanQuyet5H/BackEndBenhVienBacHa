using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Services.CauHinh;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Services.Helpers;
using System.Globalization;
using Camino.Services.Localization;

namespace Camino.Services.XuatKhos
{
    [ScopedDependency(ServiceType = typeof(IXuatKhoVatTuService))]
    public class XuatKhoVatTuService : MasterFileService<XuatKhoVatTu>, IXuatKhoVatTuService
    {

        private readonly IRepository<Kho> _khoRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;

        private readonly IRepository<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTietRepository;
        private readonly IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;

        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;

        private readonly IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;

        private readonly IRepository<VatTuBenhVien> _vatTuBenhVienRepository;

        private readonly IRepository<Template> _templateRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly ILocalizationService _localizationService;
        private new readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;

        public XuatKhoVatTuService(IRepository<XuatKhoVatTu> repository, IRepository<Kho> khoRepository
            , IRepository<XuatKhoVatTuChiTiet> xuatKhoVatTuChiTietRepository, IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository
            , IRepository<NhapKhoVatTu> nhapKhoVatTuRepository, IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository
            , IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository, IRepository<Template> templateRepository
            , ICauHinhService cauHinhService, IUserAgentHelper userAgentHelper
            , ILocalizationService localizationService
            , IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository) : base(repository)
        {
            _khoRepository = khoRepository;
            _nhanVienRepository = nhanVienRepository;
            _xuatKhoVatTuChiTietRepository = xuatKhoVatTuChiTietRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _templateRepository = templateRepository;
            _cauHinhService = cauHinhService;
            _phongBenhVienRepository = phongBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _localizationService = localizationService;
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

            var queryObject = new XuatKhoVatTuSearch();

            string searchString = null;
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.Replace("\t", "").Trim();
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<XuatKhoVatTuSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    searchString = queryObject.SearchString.Replace("\t", "").Trim();
                }
            }

            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                .Where(p => p.LoaiXuatKho != Enums.EnumLoaiXuatKho.XuatKhac && p.KhoVatTuXuat != null && (phongBenhVien != null && p.KhoVatTuXuat.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoVatTuXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2))
                //.Where(p => p.LoaiXuatKho != Enums.EnumLoaiXuatKho.XuatHuy)
                .Select(s => new XuatKhoVatTuGridVo
                {
                    Id = s.Id,
                    KhoXuat = s.KhoVatTuXuat != null ? s.KhoVatTuXuat.Ten : "",
                    KhoNhap = s.KhoVatTuNhap != null ? s.KhoVatTuNhap.Ten : "",
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NguoiNhan = s.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong ? (s.NguoiNhan != null ? (s.NguoiNhan.User != null ? s.NguoiNhan.User.HoTen : s.TenNguoiNhan) : s.TenNguoiNhan) : s.TenNguoiNhan,
                    NguoiXuat = s.NguoiXuat != null ? (s.NguoiXuat.User != null ? s.NguoiXuat.User.HoTen : "") : "",

                    NgayXuat = s.NgayXuat,
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.ApplyLike(searchString, g => g.KhoXuat, g => g.KhoNhap, g => g.SoPhieu, g => g.NguoiNhan, g => g.NguoiXuat, g => g.LyDoXuatKho);
            }
            if (queryObject != null)
            {
                if (queryObject.RangeXuat != null &&
                               (!string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay)))
                {
                    //DateTime.TryParseExact(queryObject.RangeXuat.TuNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    //DateTime.TryParseExact(queryObject.RangeXuat.DenNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    queryObject.RangeXuat.TuNgay.TryParseExactCustom(out var tuNgay);
                    queryObject.RangeXuat.DenNgay.TryParseExactCustom(out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay) || p.NgayXuat <= denNgay));
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);


            var queryObject = new XuatKhoVatTuSearch();

            string searchString = null;
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.Replace("\t", "").Trim();
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<XuatKhoVatTuSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    searchString = queryObject.SearchString.Replace("\t", "").Trim();
                }
            }

            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                  .Where(p => p.LoaiXuatKho != Enums.EnumLoaiXuatKho.XuatKhac && p.KhoVatTuXuat != null && (phongBenhVien != null && p.KhoVatTuXuat.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoVatTuXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2))
                .Select(s => new XuatKhoVatTuGridVo
                {
                    Id = s.Id,
                    KhoXuat = s.KhoVatTuXuat != null ? s.KhoVatTuXuat.Ten : "",
                    KhoNhap = s.KhoVatTuNhap != null ? s.KhoVatTuNhap.Ten : "",
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NguoiNhan = s.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong ? (s.NguoiNhan != null ? (s.NguoiNhan.User != null ? s.NguoiNhan.User.HoTen : s.TenNguoiNhan) : s.TenNguoiNhan) : s.TenNguoiNhan,
                    NguoiXuat = s.NguoiXuat != null ? (s.NguoiXuat.User != null ? s.NguoiXuat.User.HoTen : "") : "",

                    NgayXuat = s.NgayXuat,
                });
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.ApplyLike(searchString, g => g.KhoXuat, g => g.KhoNhap, g => g.SoPhieu, g => g.NguoiNhan, g => g.NguoiXuat, g => g.LyDoXuatKho);
            }
            if (queryObject != null)
            {
                if (queryObject.RangeXuat != null &&
                               (!string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay)))
                {
                    //DateTime.TryParseExact(queryObject.RangeXuat.TuNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    //DateTime.TryParseExact(queryObject.RangeXuat.DenNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    queryObject.RangeXuat.TuNgay.TryParseExactCustom(out var tuNgay);
                    queryObject.RangeXuat.DenNgay.TryParseExactCustom(out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay) || p.NgayXuat <= denNgay));
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? XuatKhoVatTuId = null, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "VatTu", Dir = "asc" } };
            }

            long par = 0;
            if (XuatKhoVatTuId != null && XuatKhoVatTuId != 0)
            {
                par = XuatKhoVatTuId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }


            var query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == par)
                .Select(s => new XuatKhoVatTuChildrenGridVo()
                {
                    Id = s.Id,
                    VatTu = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    LoaiSuDung = s.XuatKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() ?? "CHƯA PHÂN NHÓM",
                    DVT = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    Loai = s.NhapKhoVatTuChiTiet.LaVatTuBHYT ? "BHYT" : "Không BHYT",
                    SoLuongXuat = s.SoLuongXuat.ApplyNumber(),
                    SoPhieu = s.NhapKhoVatTuChiTiet.NhapKhoVatTu.SoPhieu,
                });



            var queryString = queryInfo.AdditionalSearchString;

            if (!string.IsNullOrEmpty(queryString))
            {
                query = query.ApplyLike(queryString, g => g.VatTu, g => g.DVT, g => g.Loai);
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            //var stt = 1;
            //foreach (var item in queryTask.Result)
            //{
            //    item.STT = stt;
            //    stt++;
            //}

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == long.Parse(queryInfo.SearchTerms))
                .Select(s => new XuatKhoVatTuChildrenGridVo()
                {
                    Id = s.Id,
                    VatTu = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    LoaiSuDung = s.XuatKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() ?? "CHƯA PHÂN NHÓM",
                    DVT = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    Loai = s.NhapKhoVatTuChiTiet.LaVatTuBHYT ? "BHYT" : "Không BHYT",
                    SoLuongXuat = s.SoLuongXuat.ApplyFormatMoneyToDouble(false),
                    SoPhieu = s.NhapKhoVatTuChiTiet.NhapKhoVatTu.SoPhieu,
                });



            var queryString = queryInfo.AdditionalSearchString;

            if (!string.IsNullOrEmpty(queryString))
            {
                query = query.ApplyLike(queryString, g => g.VatTu, g => g.DVT, g => g.Loai);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<bool> IsKhoExists(long id)
        {
            var kho = await _khoRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return kho != null;
        }

        public async Task<bool> IsValidateUpdateOrRemove(long id)
        {
            var result = true;
            var nhapKhoEntity = await _nhapKhoVatTuRepository.TableNoTracking
                                                .Include(p => p.NhapKhoVatTuChiTiets)
                                                .FirstOrDefaultAsync(p => p.XuatKhoVatTuId == id);
            if (nhapKhoEntity != null && nhapKhoEntity.NhapKhoVatTuChiTiets != null)
            {
                if (nhapKhoEntity.NhapKhoVatTuChiTiets.Any(p => p.SoLuongDaXuat != 0))
                {
                    result = false;
                }
            }
            return result;
        }

        public async Task DeleteXuatKho(XuatKhoVatTu entity)
        {
            _nhapKhoVatTuChiTietRepository.AutoCommitEnabled = false;
            BaseRepository.AutoCommitEnabled = false;

            foreach (var item in entity.XuatKhoVatTuChiTiets)
            {
                foreach (var viTri in item.XuatKhoVatTuChiTietViTris)
                {
                    var nhapKhoChiTietRevert = await _nhapKhoVatTuChiTietRepository.Table.FirstOrDefaultAsync(p =>
                            p.Id == viTri.NhapKhoVatTuChiTietId);
                    if (nhapKhoChiTietRevert != null)
                    {
                        nhapKhoChiTietRevert.SoLuongDaXuat = nhapKhoChiTietRevert.SoLuongDaXuat - viTri.SoLuongXuat;
                        await _nhapKhoVatTuChiTietRepository.UpdateAsync(nhapKhoChiTietRevert);
                    }
                }
            }

            await BaseRepository.DeleteAsync(entity);
            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task<XuatKhoVatTuChiTiet> GetVatTu(ThemVatTu model)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var result = new XuatKhoVatTuChiTiet();
            result.VatTuBenhVienId = model.VatTuBenhVienId ?? 0;
            var vatTuBenhVien = await _vatTuBenhVienRepository.TableNoTracking
                    .Include(p => p.NhapKhoVatTuChiTiets)
                    .ThenInclude(p => p.NhapKhoVatTu)
                .FirstOrDefaultAsync(p => p.Id == model.VatTuBenhVienId);
            var soLuongXuat = model.SoLuongXuat;
            long tempId = 0;
            var loaiSuDung = model.LoaiSuDung;
            foreach (var item in vatTuBenhVien.NhapKhoVatTuChiTiets.Where(p => p.LaVatTuBHYT == model.LaVatTuBHYT
            && p.VatTuBenhVien.LoaiSuDung == loaiSuDung
            && p.NhapKhoVatTu.KhoId == (model.KhoId ?? tempId)).OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung))
            {
                //var isDatChatLuong = model.ChatLuong == 1;
                //if (item.DatChatLuong != isDatChatLuong) continue;
                if (soLuongXuat == null || soLuongXuat == 0) break;
                var soLuongCon = item.SoLuongNhap - item.SoLuongDaXuat;
                if (soLuongXuat < soLuongCon)
                {
                    var chiTietViTri = new XuatKhoVatTuChiTietViTri
                    {
                        SoLuongXuat = soLuongXuat ?? 0,
                        NhapKhoVatTuChiTietId = item.Id,
                    };
                    result.XuatKhoVatTuChiTietViTris.Add(chiTietViTri);
                    soLuongXuat = 0;
                    break;
                }
                else if (soLuongCon == 0)
                {
                    continue;
                }
                else if (soLuongCon < 0)
                {
                    continue;
                }
                else
                {
                    soLuongXuat = soLuongXuat - soLuongCon;
                    var chiTietViTri = new XuatKhoVatTuChiTietViTri
                    {
                        SoLuongXuat = item.SoLuongNhap - item.SoLuongDaXuat,
                        NhapKhoVatTuChiTietId = item.Id,
                    };
                    result.XuatKhoVatTuChiTietViTris.Add(chiTietViTri);
                }
            }

            if (soLuongXuat != 0)
            {
                return null;
            }

            return result;
        }

        public async Task<NhapKhoVatTuChiTiet> GetNhapKhoVatTuChiTietById(long id)
        {
            var result = await _nhapKhoVatTuChiTietRepository.TableNoTracking
               .Include(p => p.KhoViTri)
               .FirstOrDefaultAsync(p => p.Id == id);
            return result;
        }

        //public async Task<GridDataSource> GetAllVatTuDataOld(QueryInfo queryInfo)
        //{
        //    BuildDefaultSortExpression(queryInfo);

        //    var lstIdString = string.Empty;
        //    long khoXuatId = 0;
        //    var lstDaChon = new List<DaSuaSoLuongXuat>();

        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
        //        long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
        //        lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|")[2]);

        //    }
        //    var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
        //    var lstNhom = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.SoLuongNhap - p.SoLuongDaXuat > 0
        //            && p.NhapKhoVatTu.Kho.Id == khoXuatId
        //            )
        //        .Select(p => new
        //        {
        //            name = p.VatTuBenhVien.LoaiSuDung.GetDescription(),
        //            key = p.VatTuBenhVien.LoaiSuDung
        //        }).Distinct().OrderBy(p => p.name).ToList();

        //    var query = _vatTuBenhVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new VatTuXuatGridVo { }).AsQueryable();

        //    foreach (var nhom in lstNhom)
        //    {
        //        var queryCoBHYT = _vatTuBenhVienRepository.TableNoTracking
        //            .Include(p => p.NhapKhoVatTuChiTiets).ThenInclude(p => p.KhoViTri).ThenInclude(p => p.KhoDuocPham)
        //            .Where(p => p.NhapKhoVatTuChiTiets.Any(x => x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
        //                            //&& x.HanSuDung >= DateTime.Now
        //                            && x.NhapKhoVatTu.Kho.Id == khoXuatId
        //                            && x.VatTuBenhVien.LoaiSuDung == nhom.key))
        //            .Select(s => new VatTuXuatGridVo
        //            {
        //                Id = s.Id + "," + nhom.key + "," + "true",
        //                TenVatTu = s.VatTus.Ten,
        //                DVT = s.VatTus.DonViTinh,
        //                LaVatTuBHYT = true,

        //                LoaiSuDung = nhom.key,
        //                LoaiSuDungDisplay = nhom.name + "",

        //                MaVatTu = s.Ma,

        //                SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 //&& nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.Solo).FirstOrDefault(),

        //                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 //&& nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.HanSuDung).FirstOrDefault(),

        //                DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == true
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 //&& nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.DonGiaNhap).FirstOrDefault(),

        //            });
        //        queryCoBHYT = queryCoBHYT.ApplyLike(queryInfo.SearchTerms, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);

        //        var queryKhongBHYT = _vatTuBenhVienRepository.TableNoTracking
        //            .Include(p => p.NhapKhoVatTuChiTiets).ThenInclude(p => p.KhoViTri).ThenInclude(p => p.KhoDuocPham)
        //            .Where(p => p.NhapKhoVatTuChiTiets.Any(x => !x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
        //                            //&& x.HanSuDung >= DateTime.Now
        //                            && x.NhapKhoVatTu.Kho.Id == khoXuatId
        //                            && x.VatTuBenhVien.LoaiSuDung == nhom.key))
        //            .Select(s => new VatTuXuatGridVo
        //            {
        //                Id = s.Id + "," + nhom.key + "," + "false",
        //                TenVatTu = s.VatTus.Ten,
        //                DVT = s.VatTus.DonViTinh,
        //                LaVatTuBHYT = false,

        //                LoaiSuDung = nhom.key,
        //                LoaiSuDungDisplay = nhom.name + "",

        //                MaVatTu = s.Ma,
        //                SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 //&& nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.Solo).FirstOrDefault(),

        //                HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 //&& nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.HanSuDung).FirstOrDefault(),

        //                DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
        //                                                                 && nkct.LaVatTuBHYT == false
        //                                                                 && nkct.VatTuBenhVienId == s.Id
        //                                                                 //&& nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.DonGiaNhap).FirstOrDefault(),
        //            });
        //        queryKhongBHYT = queryKhongBHYT.ApplyLike(queryInfo.SearchTerms, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);

        //        query = query.Concat(queryCoBHYT).Concat(queryKhongBHYT);
        //    }

        //    if (!string.IsNullOrEmpty(lstIdString))
        //    {
        //        var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
        //        query = query.Where(p => !lstId.Contains(p.Id));
        //    }

        //    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
        //    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
        //        .Take(queryInfo.Take).ToArrayAsync();
        //    await Task.WhenAll(countTask, queryTask);

        //    var stt = 1;
        //    foreach (var item in queryTask.Result)
        //    {
        //        //
        //        var id = long.Parse(item.Id.Split(",")[0]);
        //        var nhapKho = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.LaVatTuBHYT == item.LaVatTuBHYT && x.VatTuBenhVienId == id
        //        //&& x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 
        //        && x.NhapKhoVatTu.Kho.Id == khoXuatId
        //        && x.VatTuBenhVien.LoaiSuDung == item.LoaiSuDung);

        //        item.SoLuongTon = nhapKho.Sum(o => o.SoLuongNhap) - nhapKho.Sum(o => o.SoLuongDaXuat);
        //        if (lstDaChon.Any(p => p.Id == item.Id))
        //        {
        //            item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
        //        }
        //        else
        //        {
        //            item.SoLuongXuat = item.SoLuongTon;
        //        }
        //        //
        //        item.STT = stt;
        //        stt++;
        //    }

        //    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        //}

        public async Task<GridDataSource> GetAllVatTuData(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;
            var lstDaChon = new List<DaSuaSoLuongXuat>();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
                lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|")[2]);

            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            var allDataNhapVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.VatTuBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT
                }).GroupBy(o => new
                {
                    o.VatTuBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT,
                }, o => o,
                    (k, v) => new VatTuXuatGridVo
                    {
                        VatTuBenhVienId = k.VatTuBenhVienId,
                        SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        DonGia = k.DonGiaTonKho,
                        SoLo = k.Solo,
                        HanSuDung = k.HanSuDung,
                        LaVatTuBHYT = k.LaVatTuBHYT
                    }).Where(o => o.SoLuongTon >= 0.01).ToList();

            var vatTuBenhVienIds = allDataNhapVatTu.Select(o => o.VatTuBenhVienId).Distinct().ToList();
            
            var thongTinVatTus = _vatTuBenhVienRepository.TableNoTracking
                .Where(o => vatTuBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.VatTus.Ten,
                    LoaiSuDung = o.LoaiSuDung,
                    DVT = o.VatTus.DonViTinh
                }).ToList();

            var allData = new List<VatTuXuatGridVo>();
            foreach (var vatTuXuatGridVo in allDataNhapVatTu)
            {
                var thongTinVatTu = thongTinVatTus.First(o => o.Id == vatTuXuatGridVo.VatTuBenhVienId);
                vatTuXuatGridVo.MaVatTu = thongTinVatTu.Ma;
                vatTuXuatGridVo.TenVatTu = thongTinVatTu.Ten;
                vatTuXuatGridVo.LoaiSuDung = thongTinVatTu.LoaiSuDung;
                vatTuXuatGridVo.LoaiSuDungDisplay = thongTinVatTu.LoaiSuDung?.GetDescription() + "";
                vatTuXuatGridVo.DVT = thongTinVatTu.DVT;

                if (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                    (vatTuXuatGridVo.TenVatTu != null && vatTuXuatGridVo.TenVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (vatTuXuatGridVo.MaVatTu != null && vatTuXuatGridVo.MaVatTu.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (vatTuXuatGridVo.SoLo != null && vatTuXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())))
                {
                    if (!lstIdString.Contains(vatTuXuatGridVo.Id))
                        allData.Add(vatTuXuatGridVo);
                }
            }

            var dataReturn = allData.OrderBy(o=>o.LoaiSuDungDisplay).ThenBy(o => o.MaVatTu).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            var stt = 1;
            foreach (var item in dataReturn)
            {
                item.SoLuongXuat = item.SoLuongTon;
                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = dataReturn, TotalRowCount = allData.Count };
        }

        public async Task<GridDataSource> GetAllVatTuTotal(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
            }

            //if (khoXuatId == 0)
            //{
            //    return new GridDataSource { Data = null, TotalRowCount = 0 };
            //}

            var lstNhom = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.SoLuongNhap - p.SoLuongDaXuat > 0
                    //&& p.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1
                    && p.NhapKhoVatTu.Kho.Id == khoXuatId
                    )
                .Select(p => new
                {
                    name = p.VatTuBenhVien.LoaiSuDung.GetDescription(),
                    key = p.VatTuBenhVien.LoaiSuDung
                }).Distinct().OrderBy(p => p.name).ToList();

            var query = _vatTuBenhVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new VatTuXuatGridVo { }).AsQueryable();
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            foreach (var nhom in lstNhom)
            {
                var queryCoBHYT = _vatTuBenhVienRepository.TableNoTracking
                    .Include(p => p.NhapKhoVatTuChiTiets).ThenInclude(p => p.KhoViTri).ThenInclude(p => p.KhoDuocPham)
                    .Where(p => p.NhapKhoVatTuChiTiets.Any(x => x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
                                    //&& x.HanSuDung >= DateTime.Now
                                    && x.NhapKhoVatTu.Kho.Id == khoXuatId
                                    && x.VatTuBenhVien.LoaiSuDung == nhom.key))
                    .Select(s => new VatTuXuatGridVo
                    {
                        //Id = s.Id + "," + nhom.key + "," + "true",
                        TenVatTu = s.VatTus.Ten,
                        DVT = s.VatTus.DonViTinh,
                        LaVatTuBHYT = true,
                        LoaiSuDung = nhom.key,
                        LoaiSuDungDisplay = nhom.name + "",
                        MaVatTu = s.Ma,
                        SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == true
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         //&& nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.Solo).FirstOrDefault(),

                        HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == true
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         //&& nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.HanSuDung).FirstOrDefault(),

                        DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == true
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         //&& nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.DonGiaNhap).FirstOrDefault(),

                    });
                queryCoBHYT = queryCoBHYT.ApplyLike(queryInfo.SearchTerms, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);

                var queryKhongBHYT = _vatTuBenhVienRepository.TableNoTracking
                    .Include(p => p.NhapKhoVatTuChiTiets).ThenInclude(p => p.KhoViTri).ThenInclude(p => p.KhoDuocPham)
                    .Where(p => p.NhapKhoVatTuChiTiets.Any(x => !x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
                                    //&& x.HanSuDung >= DateTime.Now
                                    && x.NhapKhoVatTu.Kho.Id == khoXuatId
                                    && x.VatTuBenhVien.LoaiSuDung == nhom.key))
                    .Select(s => new VatTuXuatGridVo
                    {
                        //Id = s.Id + "," + nhom.key + "," + "false",
                        TenVatTu = s.VatTus.Ten,
                        DVT = s.VatTus.DonViTinh,
                        LaVatTuBHYT = false,

                        LoaiSuDung = nhom.key,
                        LoaiSuDungDisplay = nhom.name + "",

                        MaVatTu = s.Ma,
                        SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == false
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         //&& nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.Solo).FirstOrDefault(),

                        HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == false
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         //&& nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.HanSuDung).FirstOrDefault(),

                        DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
                                                                         && nkct.LaVatTuBHYT == false
                                                                         && nkct.VatTuBenhVienId == s.Id
                                                                         //&& nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.DonGiaNhap).FirstOrDefault(),
                    });
                queryKhongBHYT = queryKhongBHYT.ApplyLike(queryInfo.SearchTerms, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);

                query = query.Concat(queryCoBHYT).Concat(queryKhongBHYT);
            }

            if (!string.IsNullOrEmpty(lstIdString))
            {
                var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
                query = query.Where(p => !lstId.Contains(p.Id));
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<VatTuXuatGridVo>> GetVatTuOnGroup(Enums.LoaiSuDung? groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon)
        {
            var allDataNhapVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.VatTuBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT
                }).GroupBy(o => new
                {
                    o.VatTuBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaVatTuBHYT,
                }, o => o,
                    (k, v) => new VatTuXuatGridVo
                    {
                        VatTuBenhVienId = k.VatTuBenhVienId,
                        SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        DonGia = k.DonGiaTonKho,
                        SoLo = k.Solo,
                        HanSuDung = k.HanSuDung,
                        LaVatTuBHYT = k.LaVatTuBHYT
                    }).Where(o => o.SoLuongTon >= 0.01).ToList();

            var vatTuBenhVienIds = allDataNhapVatTu.Select(o => o.VatTuBenhVienId).Distinct().ToList();

            var thongTinVatTus = _vatTuBenhVienRepository.TableNoTracking
                .Where(o => vatTuBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.VatTus.Ten,
                    LoaiSuDung = o.LoaiSuDung,
                    DVT = o.VatTus.DonViTinh
                }).ToList();

            var dataReturn = new List<VatTuXuatGridVo>();

            foreach (var vatTuXuatGridVo in allDataNhapVatTu)
            {
                var thongTinVatTu = thongTinVatTus.First(o => o.Id == vatTuXuatGridVo.VatTuBenhVienId);

                vatTuXuatGridVo.MaVatTu = thongTinVatTu.Ma;
                vatTuXuatGridVo.TenVatTu = thongTinVatTu.Ten;
                vatTuXuatGridVo.LoaiSuDung = thongTinVatTu.LoaiSuDung;
                vatTuXuatGridVo.LoaiSuDungDisplay = thongTinVatTu.LoaiSuDung?.GetDescription() + "";
                vatTuXuatGridVo.DVT = thongTinVatTu.DVT;

                if (string.IsNullOrEmpty(searchString) ||
                    (vatTuXuatGridVo.TenVatTu != null && vatTuXuatGridVo.TenVatTu.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())) ||
                    (vatTuXuatGridVo.MaVatTu != null && vatTuXuatGridVo.MaVatTu.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())) ||
                    (vatTuXuatGridVo.SoLo != null && vatTuXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())))
                {
                    if (vatTuXuatGridVo.LoaiSuDung == groupId)
                        dataReturn.Add(vatTuXuatGridVo);
                }
            }

            foreach (var item in dataReturn)
            {
                item.SoLuongXuat = item.SoLuongTon;
                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
            }
            return dataReturn;

            //var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            //var queryCoBHYT = _vatTuBenhVienRepository.TableNoTracking
            //        .Where(p => p.NhapKhoVatTuChiTiets.Any(x => x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
            //                        //&& x.HanSuDung >= DateTime.Now
            //                        && x.NhapKhoVatTu.Kho.Id == khoXuatId
            //                        && x.VatTuBenhVien.LoaiSuDung == groupId))
            //        .Select(s => new VatTuXuatGridVo
            //        {
            //            //Id = s.Id + "," + groupId + "," + "true",
            //            TenVatTu = s.VatTus.Ten,
            //            DVT = s.VatTus.DonViTinh,
            //            LaVatTuBHYT = true,

            //            LoaiSuDung = groupId,
            //            LoaiSuDungDisplay = groupId.GetDescription(),

            //            MaVatTu = s.Ma,
            //            SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
            //                                                             && nkct.LaVatTuBHYT == true
            //                                                             && nkct.VatTuBenhVienId == s.Id
            //                                                             //&& nkct.HanSuDung >= DateTime.Now
            //                                                             && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
            //                                                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //                                                            .Select(z => z.Solo).FirstOrDefault(),

            //            HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
            //                                                             && nkct.LaVatTuBHYT == true
            //                                                             && nkct.VatTuBenhVienId == s.Id
            //                                                             //&& nkct.HanSuDung >= DateTime.Now
            //                                                             && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
            //                                                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //                                                            .Select(z => z.HanSuDung).FirstOrDefault(),

            //            DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
            //                                                             && nkct.LaVatTuBHYT == true
            //                                                             && nkct.VatTuBenhVienId == s.Id
            //                                                             //&& nkct.HanSuDung >= DateTime.Now
            //                                                             && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
            //                                                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //                                                            .Select(z => z.DonGiaNhap).FirstOrDefault(),
            //        });
            ////gap qua nen cheat
            //if (searchString != "undefined")
            //{
            //    queryCoBHYT = queryCoBHYT.ApplyLike(searchString, g => g.MaVatTu, g => g.TenVatTu, g => g.DVT);
            //}

            //var queryKhongBHYT = _vatTuBenhVienRepository.TableNoTracking
            //    .Where(p => p.NhapKhoVatTuChiTiets.Any(x => !x.LaVatTuBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
            //                    //&& x.HanSuDung >= DateTime.Now
            //                    && x.NhapKhoVatTu.Kho.Id == khoXuatId
            //                    && x.VatTuBenhVien.LoaiSuDung == groupId))
            //    .Select(s => new VatTuXuatGridVo
            //    {
            //        //Id = s.Id + "," + groupId + "," + "false",
            //        TenVatTu = s.VatTus.Ten,
            //        DVT = s.VatTus.DonViTinh,
            //        LaVatTuBHYT = false,

            //        LoaiSuDung = groupId,
            //        LoaiSuDungDisplay = groupId.GetDescription(),

            //        MaVatTu = s.Ma,
            //        SoLo = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
            //                                                             && nkct.LaVatTuBHYT == false
            //                                                             && nkct.VatTuBenhVienId == s.Id
            //                                                             //&& nkct.HanSuDung >= DateTime.Now
            //                                                             && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
            //                                                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //                                                            .Select(z => z.Solo).FirstOrDefault(),

            //        HanSuDung = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
            //                                                         && nkct.LaVatTuBHYT == false
            //                                                         && nkct.VatTuBenhVienId == s.Id
            //                                                         //&& nkct.HanSuDung >= DateTime.Now
            //                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
            //                                                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //                                                            .Select(z => z.HanSuDung).FirstOrDefault(),

            //        DonGia = s.NhapKhoVatTuChiTiets.Where(nkct => nkct.NhapKhoVatTu.KhoId == khoXuatId
            //                                                         && nkct.LaVatTuBHYT == false
            //                                                         && nkct.VatTuBenhVienId == s.Id
            //                                                         //&& nkct.HanSuDung >= DateTime.Now
            //                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
            //                                                            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //                                                            .Select(z => z.DonGiaNhap).FirstOrDefault(),
            //    });
            ////gap qua nen cheat
            //if (searchString != "undefined")
            //{
            //    queryKhongBHYT = queryKhongBHYT.ApplyLike(searchString, g => g.TenVatTu, g => g.DVT);
            //}

            //var query = queryCoBHYT.Concat(queryKhongBHYT).ToList();

            //foreach (var item in query)
            //{
            //    //
            //    var vatTuId = long.Parse(item.Id.Split(",")[0]);
            //    //
            //    var nhapKho = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.LaVatTuBHYT == item.LaVatTuBHYT && x.VatTuBenhVienId == vatTuId
            //    && x.NhapKhoVatTu.Kho.Id == khoXuatId && x.VatTuBenhVien.LoaiSuDung == groupId);

            //    item.SoLuongTon = nhapKho.Sum(o => o.SoLuongNhap) - nhapKho.Sum(o => o.SoLuongDaXuat);

            //    if (lstDaChon.Any(p => p.Id == item.Id))
            //    {
            //        item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
            //    }
            //    else
            //    {
            //        item.SoLuongXuat = item.SoLuongTon;
            //    }
            //}

            //return query;
        }

        public async Task<List<LookupItemVo>> GetKhoVatTu(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);

            var khos = _khoRepository.TableNoTracking.Where(p =>
                             ((p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe
                         || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin)
                           && p.KhoaPhongId == phongBenhVien.KhoaPhongId
                           && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId) || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                           && p.LoaiVatTu == true)
                .ApplyLike(model.Query, p => p.Ten)
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                })
                .Take(model.Take);
            return await khos.ToListAsync();
        }

        public async Task<long> XuatKhoVatTu(ThongTinXuatKhoVatTuVo thongTinXuatKhoVatTuVo)
        {
            foreach (var thongTinXuatKhoVatTuChiTiet in thongTinXuatKhoVatTuVo.ThongTinXuatKhoVatTuChiTietVos)
            {
                var thongTinChiTiet = thongTinXuatKhoVatTuChiTiet.Id.Split(',');
                thongTinXuatKhoVatTuChiTiet.VatTuId = long.Parse(thongTinChiTiet[0]);
                thongTinXuatKhoVatTuChiTiet.LaVTBHYT = thongTinChiTiet[1] == "1";
                thongTinXuatKhoVatTuChiTiet.HanSuDung = DateTime.ParseExact(thongTinChiTiet[2], "yyyyMMdd", null);
                thongTinXuatKhoVatTuChiTiet.SoLo = thongTinChiTiet[3];
            }

            var xuatKhoVatTuChiTietVos = thongTinXuatKhoVatTuVo.ThongTinXuatKhoVatTuChiTietVos;

            var vatTuBenhVienIds = xuatKhoVatTuChiTietVos.Select(o => o.VatTuId).ToList();
            var soLos = xuatKhoVatTuChiTietVos.Select(o => o.SoLo).ToList();

            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                .Where(o => o.NhapKhoVatTu.KhoId == thongTinXuatKhoVatTuVo.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                .ToList();
            //xuat kho
            var xuatKhoVatTu = new XuatKhoVatTu
            {
                LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatQuaKhoKhac,
                KhoXuatId = thongTinXuatKhoVatTuVo.KhoXuatId,
                KhoNhapId = thongTinXuatKhoVatTuVo.KhoNhapId,
                LyDoXuatKho = thongTinXuatKhoVatTuVo.LyDoXuatKho,
                NguoiXuatId = thongTinXuatKhoVatTuVo.NguoiXuatId,
                LoaiNguoiNhan = thongTinXuatKhoVatTuVo.LoaiNguoiNhan,
                TenNguoiNhan = thongTinXuatKhoVatTuVo.TenNguoiNhan,
                NguoiNhanId = thongTinXuatKhoVatTuVo.NguoiNhanId,
                NgayXuat = thongTinXuatKhoVatTuVo.NgayXuat
            };
            foreach (var chiTietVo in xuatKhoVatTuChiTietVos)
            {
                var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                    .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuId && o.LaVatTuBHYT == chiTietVo.LaVTBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Date);
                var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat) && slTon < chiTietVo.SoLuongXuat)
                {
                    throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                }
                double soLuongCanXuat = chiTietVo.SoLuongXuat;
                while (!soLuongCanXuat.Equals(0))
                {
                    // tinh so luong xuat
                    var nhapKhoVatTuChiTiet = nhapKhoVatTuChiTietXuats
                        .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                    var soLuongTon = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                    var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                    nhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongXuat;
                    var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                    {
                        SoLuongXuat = soLuongXuat,
                        NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                        NgayXuat = thongTinXuatKhoVatTuVo.NgayXuat
                    };
                    var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                    {
                        VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                        NgayXuat = thongTinXuatKhoVatTuVo.NgayXuat
                    };
                    xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                    xuatKhoVatTu.XuatKhoVatTuChiTiets.Add(xuatKhoVatTuChiTiet);

                    soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                }
            }
            //nhap kho
            var nhapKho = new NhapKhoVatTu();
            nhapKho.XuatKhoVatTu = xuatKhoVatTu;
            nhapKho.KhoId = thongTinXuatKhoVatTuVo.KhoNhapId;
            nhapKho.NguoiGiaoId = thongTinXuatKhoVatTuVo.NguoiXuatId;
            nhapKho.NguoiNhapId = thongTinXuatKhoVatTuVo.NguoiNhanId ?? _userAgentHelper.GetCurrentUserId();
            nhapKho.NgayNhap = thongTinXuatKhoVatTuVo.NgayXuat;
            nhapKho.LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong;
            foreach (var item in xuatKhoVatTu.XuatKhoVatTuChiTiets)
            {
                foreach (var viTri in item.XuatKhoVatTuChiTietViTris)
                {
                    var nhapKhoChiTiet = new NhapKhoVatTuChiTiet();
                    nhapKhoChiTiet.HopDongThauVatTuId = viTri.NhapKhoVatTuChiTiet.HopDongThauVatTuId;
                    nhapKhoChiTiet.Solo = viTri.NhapKhoVatTuChiTiet.Solo;
                    nhapKhoChiTiet.LaVatTuBHYT = viTri.NhapKhoVatTuChiTiet.LaVatTuBHYT;
                    nhapKhoChiTiet.HanSuDung = viTri.NhapKhoVatTuChiTiet.HanSuDung;
                    nhapKhoChiTiet.SoLuongNhap = viTri.SoLuongXuat;
                    nhapKhoChiTiet.DonGiaNhap = viTri.NhapKhoVatTuChiTiet.DonGiaNhap;
                    nhapKhoChiTiet.VAT = viTri.NhapKhoVatTuChiTiet.VAT;
                    nhapKhoChiTiet.TiLeBHYTThanhToan = viTri.NhapKhoVatTuChiTiet.TiLeBHYTThanhToan;
                    nhapKhoChiTiet.MaVach = viTri.NhapKhoVatTuChiTiet.MaVach;
                    nhapKhoChiTiet.MaRef = viTri.NhapKhoVatTuChiTiet.MaRef;
                    nhapKhoChiTiet.VatTuBenhVienId = viTri.NhapKhoVatTuChiTiet.VatTuBenhVienId;
                    nhapKhoChiTiet.SoLuongDaXuat = 0;
                    nhapKhoChiTiet.NgayNhap = thongTinXuatKhoVatTuVo.NgayXuat;
                    nhapKhoChiTiet.NgayNhapVaoBenhVien = viTri.NhapKhoVatTuChiTiet.NgayNhapVaoBenhVien;
                    nhapKhoChiTiet.TiLeTheoThapGia = viTri.NhapKhoVatTuChiTiet.TiLeTheoThapGia;
                    nhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = viTri.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho;
                    nhapKho.NhapKhoVatTuChiTiets.Add(nhapKhoChiTiet);
                }
            }
            _nhapKhoVatTuRepository.Add(nhapKho);
            return xuatKhoVatTu.Id;
        }

        public async Task SaveChange()
        {
            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task<double> GetSoLuongTon(long vatTuId, bool isDatChatLuong, long khoNhapId)
        {
            var vatTu = await _vatTuBenhVienRepository.TableNoTracking
                .Include(p => p.NhapKhoVatTuChiTiets)
                .ThenInclude(p => p.NhapKhoVatTu)
                .FirstOrDefaultAsync(p => p.Id == vatTuId);
            double total = 0;
            foreach (var item in vatTu.NhapKhoVatTuChiTiets.Where(p => p.NhapKhoVatTu.KhoId == khoNhapId))
            {
                total = total + item.SoLuongNhap - item.SoLuongDaXuat;
            }
            return total;
        }

        public async Task<List<LookupItemVo>> GetKhoVatTuNhap(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var khoXuatId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (khoXuatId == 0) return new List<LookupItemVo>();
            var khoXuat = _khoRepository.TableNoTracking.First(p => p.Id == khoXuatId);

            var lst = _khoRepository.TableNoTracking.Where(p => (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc || (khoXuatId == (long)Enums.EnumKhoDuocPham.KhoVatTuYTe && p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoKSNK))
                       && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId) && p.LoaiVatTu == true)
                            .ApplyLike(model.Query, p => p.Ten)
                            .Take(model.Take).Select(item => new LookupItemVo
                            {
                                DisplayName = item.Ten,
                                KeyId = item.Id
                            });
            return await lst.ToListAsync(); ;
        }

        public async Task<bool> IsKhoLeOrNhaThuoc(long id)
        {
            var kho = await _khoRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return (kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2);
        }

        public async Task<string> InPhieuXuat(long id, string hostingName)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatKhoVatTu"));

            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == id)
                .Select(item => new ThongTinInXuatKhoVatTuVo()
                {
                    TenNguoiNhanHang = item.NguoiNhan.User.HoTen,
                    BoPhan = item.KhoVatTuNhap.PhongBenhVien != null
                            ? item.KhoVatTuNhap.PhongBenhVien.Ma + " - " + item.KhoVatTuNhap.PhongBenhVien.Ten
                            : (item.KhoVatTuNhap.KhoaPhong != null ? item.KhoVatTuNhap.KhoaPhong.Ma + " - " + item.KhoVatTuNhap.KhoaPhong.Ten : ""),
                    LyDoXuatKho = item.LyDoXuatKho,
                    XuatTaiKho = item.KhoVatTuXuat.Ten,
                    DiaDiem = "",
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU XUẤT</th>" +
                         "</p>";

            var query = await _xuatKhoVatTuChiTietViTriRepository.TableNoTracking.Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == id)
                .Select(s => new ThongTinInXuatKhoVatTuChiTietVo
                {
                    DuocPhamBenhVienId = s.XuatKhoVatTuChiTiet.VatTuBenhVienId,
                    Ma = s.XuatKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    TenThuoc = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    //DVT = s.DuocPhamBenhVien.DuocPham.DonViTinhThamKhao ?? (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                    DVT = s.XuatKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    SLYeuCau = s.SoLuongXuat.ApplyNumber(),
                    SLThucXuat = s.SoLuongXuat.ApplyNumber(),
                    SLYC = s.SoLuongXuat,
                    SLTX = s.SoLuongXuat,
                    //LaDuocPhamBHYT = s.LaDuocPhamBHYT
                    TenNhom = s.XuatKhoVatTuChiTiet.VatTuBenhVien.LoaiSuDung.GetDescription() ?? "CHƯA PHÂN NHÓM"
                }).OrderBy(z => z.TenThuoc)
                .ToListAsync();

            data.SoLuongThucXuatTong = query.Sum(p => p.SLTX).ApplyNumber();
            data.SoLuongYeuCauTong = query.Sum(p => p.SLYC).ApplyNumber();

            var totalTenNhom = query.Select(p => p.TenNhom).Distinct().ToList();

            var info = string.Empty;

            var STT = 1;
            foreach (var tenNhom in totalTenNhom)
            {
                var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + tenNhom.ToUpper()
                                        + "</b></tr>";
                info += headerNhom;
                var queryNhom = query.Where(p => p.TenNhom == tenNhom).ToList();
                foreach (var item in queryNhom)
                {
                    info = info
                                           + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                           + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                           + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucXuat
                                           + "</tr>";
                    STT++;
                }
            }

            data.Header = hearder;
            data.DanhSachThuoc = info;
            ;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
    }
}
