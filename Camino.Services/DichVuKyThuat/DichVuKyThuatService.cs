using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DichVuKyThuat
{
    [ScopedDependency(ServiceType = typeof(IDichVuKyThuatService))]
    public class DichVuKyThuatService : MasterFileService<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat>, IDichVuKyThuatService
    {
        private readonly IRepository<DichVuKyThuatThongTinGia> _dichVuKyThuatThongTinGia;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat> _nhomDichVuKyThuatRepository;
        public DichVuKyThuatService(IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> repository,
            IRepository<DichVuKyThuatThongTinGia> dichVuKyThuatThongTinGia, IRepository<Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat> nhomDichVuKyThuatRepository) : base(repository)
        {
            _dichVuKyThuatThongTinGia = dichVuKyThuatThongTinGia;
            _nhomDichVuKyThuatRepository = nhomDichVuKyThuatRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Include(x => x.NhomDichVuKyThuat)
                .Select(s => new DichVuKyThuatGridVo
                {
                    Id = s.Id,
                    Ten = s.TenChung,
                    TenTiengAnh = s.TenTiengAnh,
                    Ma = s.MaChung,
                    Ma4350 = s.Ma4350,
                    MaGia = s.MaGia,
                    TenGia = s.TenGia,
                    TenNhomChiPhi = s.NhomChiPhi.GetDescription(),
                    TenNhomDichVuKyThuat = s.NhomDichVuKyThuat.Ten,
                    TenLoaiPhauThuatThuThuat = s.LoaiPhauThuatThuThuat.GetDescription(),
                    MoTa = s.MoTa
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenTiengAnh, g => g.Ma, g => g.Ma4350, g => g.MaGia, g => g.TenGia,
                g => g.TenNhomDichVuKyThuat);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Include(x => x.NhomDichVuKyThuat)
                .Select(s => new DichVuKyThuatGridVo
                {
                    Id = s.Id,
                    Ten = s.TenChung,
                    TenTiengAnh = s.TenTiengAnh,
                    Ma = s.MaChung,
                    Ma4350 = s.Ma4350,
                    MaGia = s.MaGia,
                    TenGia = s.TenGia,
                    TenNhomChiPhi = s.NhomChiPhi.GetDescription(),
                    TenNhomDichVuKyThuat = s.NhomDichVuKyThuat.Ten,
                    TenLoaiPhauThuatThuThuat = s.LoaiPhauThuatThuThuat.GetDescription(),
                    MoTa = s.MoTa
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenTiengAnh, g => g.Ma, g => g.Ma4350, g => g.MaGia, g => g.TenGia,
                g => g.TenNhomDichVuKyThuat);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? dichVuKyThuatId, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par = 0;
            if (dichVuKyThuatId != null && dichVuKyThuatId != 0)
            {
                par = dichVuKyThuatId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }

            var query = _dichVuKyThuatThongTinGia.TableNoTracking
                //.Where(x => x.DichVuKyThuatId == long.Parse(queryInfo.SearchTerms))
                .Where(o => o.DichVuKyThuatId == par)
                .Select(s => new DichVuKyThuatThongTinGiaGridVo()
                {
                    Id = s.Id,
                    TenHangBenhVien = s.HangBenhVien == null ? null : s.HangBenhVien.GetDescription(),
                    Gia = s.Gia,
                    GiaFormat = Convert.ToDouble(s.Gia).ApplyNumber(),
                    TuNgay = s.TuNgay,
                    TuNgayFormat = s.TuNgay.ApplyFormatDate(),
                    DenNgay = s.DenNgay,
                    DenNgayFormat = s.DenNgay == null ? null : s.DenNgay.Value.ApplyFormatDate(),
                    ThongTu = s.ThongTu,
                    QuyetDinh = s.QuyetDinh,
                    MoTa = s.MoTa,
                    TenHieuLuc = s.HieuLuc ? "Còn hiệu lực" : "Hết hiệu lực"
                });
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenHangBenhVien, g => g.GiaFormat, g => g.TuNgayFormat, g => g.DenNgayFormat, g => g.ThongTu, g => g.QuyetDinh,
            //    g => g.MoTa, g => g.TenHieuLuc);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _dichVuKyThuatThongTinGia.TableNoTracking
                .Where(x => x.Id == long.Parse(queryInfo.SearchTerms))
                .Select(s => new DichVuKyThuatThongTinGiaGridVo()
                {
                    Id = s.Id,
                    TenHangBenhVien = s.HangBenhVien == null ? null : s.HangBenhVien.GetDescription(),
                    Gia = s.Gia,
                    GiaFormat = Convert.ToDouble(s.Gia).ApplyNumber(),
                    TuNgay = s.TuNgay,
                    TuNgayFormat = s.TuNgay.ApplyFormatDate(),
                    DenNgay = s.DenNgay,
                    DenNgayFormat = s.DenNgay == null ? null : s.DenNgay.Value.ApplyFormatDate(),
                    ThongTu = s.ThongTu,
                    QuyetDinh = s.QuyetDinh,
                    MoTa = s.MoTa,
                    TenHieuLuc = s.HieuLuc ? "Còn hiệu lực" : "Hết hiệu lực"
                });
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenHangBenhVien, g => g.GiaFormat, g => g.TuNgayFormat, g => g.DenNgayFormat, g => g.ThongTu, g => g.QuyetDinh,
            //    g => g.MoTa, g => g.TenHieuLuc);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public List<DichVuKyThuatExportExcel> DataGridForExcel(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new DichVuKyThuatExportExcel
                {
                    Id = s.Id,
                    Ten = s.TenChung,
                    TenTiengAnh = s.TenTiengAnh,
                    Ma = s.MaChung,
                    Ma4350 = s.Ma4350,
                    MaGia = s.MaGia,
                    TenGia = s.TenGia,
                    TenNhomChiPhi = s.NhomChiPhi.GetDescription(),
                    TenNhomDichVuKyThuat = s.NhomDichVuKyThuat.Ten,
                    TenLoaiPhauThuatThuThuat = s.LoaiPhauThuatThuThuat.GetDescription(),
                    MoTa = s.MoTa,
                    DichVuKyThuatExportExcelChild = s.DichVuKyThuatThongTinGias
                                                    .Select(c => new DichVuKyThuatExportExcelChild
                                                    {
                                                        TenHangBenhVien = c.HangBenhVien == null ? null : c.HangBenhVien.GetDescription(),
                                                        GiaFormat = Convert.ToDouble(c.Gia).ApplyNumber(),
                                                        TuNgayFormat = c.TuNgay.ApplyFormatDate(),
                                                        DenNgayFormat = c.DenNgay == null ? null : c.DenNgay.Value.ApplyFormatDate(),
                                                        ThongTu = c.ThongTu,
                                                        QuyetDinh = c.QuyetDinh,
                                                        MoTa = c.MoTa,
                                                        TenHieuLuc = c.HieuLuc ? "Còn hiệu lực" : "Hết hiệu lực"
                                                    }).ToList()

                }).ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenTiengAnh, g => g.Ma, g => g.Ma4350, g => g.MaGia, g => g.TenGia,
                g => g.TenNhomDichVuKyThuat);
            queryInfo.Take = int.MaxValue;
            return query.OrderBy(queryInfo.SortString).Take(queryInfo.Take).ToList();
        }
        public async Task<List<LookupItemVo>> GetListNhomDichVuKyThuat(DropDownListRequestModel model)
        {
            var lstNhomDichVuCls = await _nhomDichVuKyThuatRepository.TableNoTracking
               .ApplyLike(model.Query, o => o.Ten)
               .Take(model.Take).ToListAsync();

            var query = lstNhomDichVuCls.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id
            }).ToList();
            return query;
        }

        public Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat GetDVKTByMa(string ma)
        {
            var data = BaseRepository.TableNoTracking.Include(s => s.DichVuKyThuatThongTinGias).FirstOrDefault(s => s.MaChung == ma);
            return data;
        }

        public async Task<List<NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo>> NhomDichVuKyThuatBenhVienPhanNhomTreeViews(DropDownListRequestModel model)
        {
            var nhomDichVus = await _nhomDichVuKyThuatRepository.TableNoTracking
                .Select(item => new NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                    ParentId = item.NhomDichVuKyThuatChaId
                }).ToListAsync();

            var query = nhomDichVus
              .Select(item => new NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo
              {
                  KeyId = item.KeyId,
                  DisplayName = item.DisplayName,
                  ParentId = item.ParentId,
                  Items = GetChildrenTree(nhomDichVus, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
              })
          .Where(x =>
              x.ParentId == null && (string.IsNullOrEmpty(model.Query) ||
                                     (!string.IsNullOrEmpty(model.Query) && (x.Items.Any() || x.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())))))
          .ToList(); //.Take(model.Take) khách hang muốn xem hết
            return query;
        }

        public static List<NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo> GetChildrenTree(List<NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo> comments, long Id, string queryString, string parentDisplay)
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id)
                .Select(c => new NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetChildrenTree(comments, c.KeyId, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString)
                            || (!string.IsNullOrEmpty(queryString) && (parentDisplay.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any())))
                .ToList();
            return query;
        }

        public List<LookupItemVo> GetNhomDVKTs(DropDownListRequestModel model)
        {
            var enumDanhMucNhomTheoChiPhis = EnumHelper.GetListEnum<EnumDanhMucNhomTheoChiPhi>();
            var query = enumDanhMucNhomTheoChiPhis
                .Where(x => x != EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT
                          && x != EnumDanhMucNhomTheoChiPhi.ThuocDieuTriUngThuVaChongThaiGhepNgoaiDanhMuc
                          && x != EnumDanhMucNhomTheoChiPhi.ThuocThanhToanTheoTyLe
                          && x != EnumDanhMucNhomTheoChiPhi.MauVaChePhamMau)
                .Select(s => new LookupItemVo
                {
                    KeyId = (long)s,
                    DisplayName = s.GetDescription()
                }).ToList();
            if (!string.IsNullOrEmpty(model.Query))
            {
                query = query.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return query;
        }

        public async Task<bool> KiemTraTrungMaHoacTen(string maHoacTen = null, long dichVuKyThuatId = 0, bool flag = false)
        {
            var result = false;
            if (dichVuKyThuatId == 0)
            {
                result = !flag ? await BaseRepository.TableNoTracking.AnyAsync(p => p.MaChung.Equals(maHoacTen)) : await BaseRepository.TableNoTracking.AnyAsync(p => p.TenChung.Equals(maHoacTen));
            }
            else
            {
                result = !flag ? await BaseRepository.TableNoTracking.AnyAsync(p => p.MaChung.Equals(maHoacTen) && p.Id != dichVuKyThuatId) : await BaseRepository.TableNoTracking.AnyAsync(p => p.TenChung.Equals(maHoacTen) && p.Id != dichVuKyThuatId);
            }
            if (result)
                return false;
            return true;
        }
    }
}
