using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.ExportImport.Help;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.DichVuGiuongBenhVien
{
    [ScopedDependency(ServiceType = typeof(IDichVuGiuongBenhVienService))]
    public class DichVuGiuongBenhVienService
        : MasterFileService<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien>, IDichVuGiuongBenhVienService
    {
        private readonly IRepository<Core.Domain.Entities.DichVuGiuongs.DichVuGiuong> _dichVuGiuong;
        private readonly IRepository<DichVuGiuongBenhVienGiaBaoHiem> _dichVuGiuongGiaBaoHiem;
        private readonly IRepository<DichVuGiuongBenhVienGiaBenhVien> _dichVuGiuongGiaBenhVien;
        private readonly IRepository<NhomGiaDichVuGiuongBenhVien> _nhomGiaBenhVien;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<DichVuGiuongBenhVienNoiThucHien> _dichVuGiuongBenhVienNoiThucHienRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private ILocalizationService _localizationService;
        public DichVuGiuongBenhVienService(
            IRepository<DichVuGiuongBenhVienGiaBenhVien> dichVuGiuongGiaBenhVien,
            IRepository<NhomGiaDichVuGiuongBenhVien> nhomGiaBenhVien,
            IRepository<DichVuGiuongBenhVienGiaBaoHiem> dichVuGiuongGiaBaoHiem,
            IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> repository,
            IRepository<Core.Domain.Entities.DichVuGiuongs.DichVuGiuong> dichVuGiuong,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
            IRepository<DichVuGiuongBenhVienNoiThucHien> dichVuGiuongBenhVienNoiThucHienRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            ILocalizationService localizationService) : base(repository)
        {
            _nhomGiaBenhVien = nhomGiaBenhVien;
            _dichVuGiuongGiaBenhVien = dichVuGiuongGiaBenhVien;
            _dichVuGiuong = dichVuGiuong;
            _dichVuGiuongGiaBaoHiem = dichVuGiuongGiaBaoHiem;
            _khoaPhongRepository = khoaPhongRepository;
            _dichVuGiuongBenhVienNoiThucHienRepository = dichVuGiuongBenhVienNoiThucHienRepository;
            _cauHinhRepository = cauHinhRepository;
            _localizationService = localizationService;
        }

        //Todo: need update DichVuGiuongGridVo -> DichVuGiuongBenhVienGridVo
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var danhMucGiuongBenhVien = new JsonDichVuGiuong();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                danhMucGiuongBenhVien = JsonConvert.DeserializeObject<JsonDichVuGiuong>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Include(x => x.DichVuGiuong)
                .Where(x => (danhMucGiuongBenhVien.HieuLuc == null && x.HieuLuc != null) || (danhMucGiuongBenhVien.HieuLuc != null && x.HieuLuc == danhMucGiuongBenhVien.HieuLuc))
                .Where(x => (danhMucGiuongBenhVien.AnhXa != true && x.DichVuGiuongId == null) || (danhMucGiuongBenhVien.AnhXa != false && x.DichVuGiuongId != null))
                .Select(s => new DichVuGiuongGridVo
                {
                    Id = s.Id,
                    DichVuGiuongId = s.DichVuGiuongId,
                    Ma = s.Ma,
                    MaTT37 = s.DichVuGiuong == null ? string.Empty : s.DichVuGiuong.MaTT37,
                    Ten = s.Ten,
                    //HangBenhVien = s.DichVuGiuong.HangBenhVien,
                    //HangBenhVienDisplay = s.DichVuGiuong.HangBenhVien.GetDescription(),
                    AnhXa = s.DichVuGiuongId != null,
                    LoaiGiuong = s.LoaiGiuong,
                    LoaiGiuongDisplay = s.LoaiGiuong.GetDescription(),
                    MoTa = s.MoTa,
                    HieuLucHienThi = s.HieuLuc ? "Có" : "Không",

                    DichVuGiuongBenhVienGiaBaoHiems = s.DichVuGiuongBenhVienGiaBaoHiems,
                    DichVuGiuongBenhVienGiaBenhViens = s.DichVuGiuongBenhVienGiaBenhViens
                });

            if (!string.IsNullOrEmpty(danhMucGiuongBenhVien.SearchString))
            {
                query = query.ApplyLike(danhMucGiuongBenhVien.SearchString, g => g.Ma, g => g.MaTT37, g => g.Ten, g => g.MoTa);
            }            

            var queryList = query.ToList();
            var listDvg = await BaseRepository.TableNoTracking
                .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong)
                .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
                .Where(x => queryList.Select(a => a.Id.ToString()).Contains(x.Id.ToString())).ToListAsync();

            if (listDvg.Any())
            {
                foreach (var item in queryList)
                {
                    var dichVuKyThuat = listDvg.FirstOrDefault(x => x.Id == item.Id);
                    if (dichVuKyThuat != null)
                    {
                        item.TenNoiThucHien = string.Join("; ", dichVuKyThuat.DichVuGiuongBenhVienNoiThucHiens.Where(a => a.KhoaPhongId != null).Select(a => a.KhoaPhong.Ma + " - " + a.KhoaPhong.Ten))
                                              + (dichVuKyThuat.DichVuGiuongBenhVienNoiThucHiens.Any(x => x.KhoaPhongId != null) ? "; " : "")
                                              + string.Join("; ", dichVuKyThuat.DichVuGiuongBenhVienNoiThucHiens.Where(a => a.PhongBenhVienId != null).Select(a => a.PhongBenhVien.Ma + " - " + a.PhongBenhVien.Ten));
                    }
                }
            }
            query = queryList.AsQueryable();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            await Task.WhenAll(countTask);

            var nhomGiaThuongBenhVienId = _nhomGiaBenhVien.TableNoTracking.Where(z => z.Ten.Trim().ToUpper() == ("Thường").Trim().ToUpper()).Select(x => x.Id).FirstOrDefault();

            foreach (var item in queryTask)
            {
                item.TLTT = item.AnhXa ? string.Join(", ", item.DichVuGiuongBenhVienGiaBaoHiems.Where(bh => bh.TuNgay.Date <= DateTime.Now.Date && (bh.DenNgay == null || bh.DenNgay >= DateTime.Now)).Select(x => x.TiLeBaoHiemThanhToan + "%").ToList()) : string.Empty;
                item.GiaBaoHiems = item.AnhXa ? string.Join(", ", item.DichVuGiuongBenhVienGiaBaoHiems.Where(bh => bh.TuNgay.Date <= DateTime.Now.Date && (bh.DenNgay == null || bh.DenNgay >= DateTime.Now)).Select(x => x.Gia.ApplyFormatMoneyVND()).ToList()): string.Empty;

                item.GiaThuongBenhViens = string.Join(", ", item.DichVuGiuongBenhVienGiaBenhViens.Where(bh => bh.NhomGiaDichVuGiuongBenhVienId == nhomGiaThuongBenhVienId && bh.TuNgay.Date <= DateTime.Now.Date && (bh.DenNgay == null || bh.DenNgay >= DateTime.Now)).Select(x => x.Gia.ApplyFormatMoneyVND()).ToList());
            }

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var danhMucGiuongBenhVien = new JsonDichVuGiuong();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                danhMucGiuongBenhVien = JsonConvert.DeserializeObject<JsonDichVuGiuong>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Include(x => x.DichVuGiuong)
                .Where(x => (danhMucGiuongBenhVien.HieuLuc == null && x.HieuLuc != null) || (danhMucGiuongBenhVien.HieuLuc != null && x.HieuLuc == danhMucGiuongBenhVien.HieuLuc))
                .Where(x => (danhMucGiuongBenhVien.AnhXa != true && x.DichVuGiuongId == null) || (danhMucGiuongBenhVien.AnhXa != false && x.DichVuGiuongId != null))
                .Select(s => new DichVuGiuongGridVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    DichVuGiuongId = s.DichVuGiuongId,
                    MaTT37 = s.DichVuGiuong == null ? string.Empty : s.DichVuGiuong.MaTT37,
                    Ten = s.Ten,
                    //HangBenhVien = s.DichVuGiuong.HangBenhVien,
                    //HangBenhVienDisplay = s.DichVuGiuong.HangBenhVien.GetDescription(),
                    LoaiGiuong = s.LoaiGiuong,
                    LoaiGiuongDisplay = s.LoaiGiuong.GetDescription(),
                    MoTa = s.MoTa,
                    HieuLucHienThi = s.HieuLuc ? "Có" : "Không"
                });


            if (!string.IsNullOrEmpty(danhMucGiuongBenhVien.SearchString))
            {
                query = query.ApplyLike(danhMucGiuongBenhVien.SearchString, g => g.Ma, g => g.MaTT37, g => g.Ten, g => g.MoTa);
            }

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? dichVuGiuongId, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par;
            if (dichVuGiuongId != null && dichVuGiuongId != 0)
            {
                par = (long)dichVuGiuongId;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }

            var sortString = RemoveDisplaySort(queryInfo);
            var query = _dichVuGiuongGiaBaoHiem.TableNoTracking
                .Where(x => x.DichVuGiuongBenhVienId == par)
                .Select(s => new DichVuKhamBenhBenhVienGiaBaoHiemVO
                {
                    Id = s.Id,
                    Gia = s.Gia,
                    GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan,
                    TuNgayHienThi = s.TuNgay.ApplyFormatDate(),
                    TuNgay = s.TuNgay,
                    DenNgayHienThi = s.DenNgay != null ? Convert.ToDateTime(s.DenNgay).ApplyFormatDate() : null
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _dichVuGiuongGiaBaoHiem.TableNoTracking
                  .Where(x => x.DichVuGiuongBenhVienId == long.Parse(queryInfo.SearchTerms))
                  .Select(s => new DichVuKhamBenhBenhVienGiaBaoHiemVO
                  {
                      Id = s.Id,
                      Gia = s.Gia,
                      TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan,
                      TuNgayHienThi = s.TuNgay.ApplyFormatDate(),
                      TuNgay = s.TuNgay,
                      DenNgayHienThi = s.DenNgay != null ? Convert.ToDateTime(s.DenNgay).ApplyFormatDate() : null
                  });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildBenhVienAsync(QueryInfo queryInfo, long? dichVuGiuongId, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par;
            if (dichVuGiuongId != null && dichVuGiuongId != 0)
            {
                par = (long)dichVuGiuongId;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }
            var sortString = RemoveDisplaySort(queryInfo);
            var query = _dichVuGiuongGiaBenhVien.TableNoTracking.Include(x => x.DichVuGiuongBenhVien).Include(x => x.NhomGiaDichVuGiuongBenhVien)
                .Where(x => x.DichVuGiuongBenhVienId == par)
               .Select(s => new DichVuKhamBenhBenhVienGiaBenhVienVO
               {
                   Id = s.Id,
                   Gia = s.Gia,
                   GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                   LoaiGia = s.NhomGiaDichVuGiuongBenhVien.Ten,
                   TuNgay = s.TuNgay,
                   TuNgayHienThi = s.TuNgay.ApplyFormatDate(),
                   DenNgayHienThi = s.DenNgay != null ? Convert.ToDateTime(s.DenNgay).ApplyFormatDate() : null
               });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildBenhVienAsync(QueryInfo queryInfo)
        {
            var query = _dichVuGiuongGiaBenhVien.TableNoTracking.Include(x => x.DichVuGiuongBenhVien).Include(x => x.NhomGiaDichVuGiuongBenhVien)
                .Where(x => x.DichVuGiuongBenhVienId == long.Parse(queryInfo.SearchTerms))
               .Select(s => new DichVuKhamBenhBenhVienGiaBenhVienVO()
               {
                   Id = s.Id,
                   Gia = s.Gia,
                   GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                   LoaiGia = s.NhomGiaDichVuGiuongBenhVien.Ten,
                   TuNgay = s.TuNgay,
                   TuNgayHienThi = s.TuNgay.ApplyFormatDate(),
                   DenNgayHienThi = s.DenNgay != null ? Convert.ToDateTime(s.DenNgay).ApplyFormatDate() : null
               });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<List<DichVuGiuongTemplateVo>> GetDichVuGiuongById(DropDownListRequestModel model, long id)
        {

            var lst = await _dichVuGiuong.TableNoTracking.Include(x => x.Khoa)
                .Where(p => p.TenChung.Contains(model.Query ?? "") || p.MaChung.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();
            var entity = await _dichVuGiuong.TableNoTracking.Include(x => x.Khoa)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            var lstDichVuKhamBenhBenhVien = await BaseRepository.TableNoTracking.Distinct()
                .ToListAsync();

            for (int i = 0; i < lstDichVuKhamBenhBenhVien.Count; i++)
            {
                var check = lst.Where(x => x.Id == lstDichVuKhamBenhBenhVien[i].DichVuGiuongId).FirstOrDefault();
                if (check != null)
                {
                    lst.RemoveAll(x => x.Id == lstDichVuKhamBenhBenhVien[i].DichVuGiuongId);
                }
            }
            lst.Add(entity);
            var query = lst.Select(item => new DichVuGiuongTemplateVo
            {
                DisplayName = item.MaChung + " - " + item.TenChung + " - " + item.Khoa.Ten,
                KeyId = item.Id,
                Ma = item.MaChung,
                Ten = item.TenChung,
            }).ToList();

            return query;
        }
        public Task<long> GetIdKhoaPhongFromRequestDropDownList(DropDownListRequestModel model)
        {
            var parameter = model != null ? model.ParameterDependencies : "";
            if (string.IsNullOrEmpty(parameter)) return Task.FromResult((long)0);
            var getValue = JsonConvert.DeserializeObject<Dictionary<string, long>>(parameter);
            var toaNhaId = getValue.Values.First();
            return Task.FromResult(toaNhaId);
        }
        public async Task<List<DichVuGiuongTemplateVo>> GetDichVuGiuong(DropDownListRequestModel model)
        {
            var cauHinhHangBenhVien = await _cauHinhRepository.TableNoTracking
                .Where(x => x.Name == "BaoHiemYTe.HangBenhVien").FirstOrDefaultAsync();
            var hangBenhVien = cauHinhHangBenhVien == null ? 0 : int.Parse(cauHinhHangBenhVien.Value);
            return await _dichVuGiuong.TableNoTracking
                .Where(o => //BaseRepository.TableNoTracking.All(p => p.DichVuGiuongId != o.Id) && 
                            (int)o.HangBenhVien == hangBenhVien)
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.TenChung)
                .Select(item => new DichVuGiuongTemplateVo
                {
                    DisplayName = item.MaChung + " - " + item.TenChung,
                    KeyId = item.Id,
                    Ten = item.TenChung,
                    Ma = item.MaChung
                }).ApplyLike(model.Query, o => o.DisplayName)
                .Take(model.Take).ToListAsync();
        }
        public async Task<List<DichVuGiuongTemplateVo>> GetListDichVuGiuong(DropDownListRequestModel model)
        {
            return await _dichVuGiuong.TableNoTracking
                .Where(p => p.TenChung.Contains(model.Query ?? "") || p.MaChung.Contains(model.Query ?? ""))
                .Select(item => new DichVuGiuongTemplateVo
                {
                    DisplayName = item.MaChung + " - " + item.TenChung,
                    KeyId = item.Id,
                    Ten = item.TenChung,
                    Ma = item.MaChung
                })
                .Take(model.Take)
                .ToListAsync();
        }
        public async Task<List<NhomGiaDichVuGiuongBenhVien>> GetNhomGiaDichVuKyThuatBenhVien()
        {
            var lstEntity = await _nhomGiaBenhVien.Table.ToListAsync();


            return lstEntity;
        }
        public void UpdateDayGiaBenhVienEntity(ICollection<DichVuGiuongBenhVienGiaBenhVien> giaBenhVienEntity)
        {
            var nhomDichvu = _nhomGiaBenhVien.Table.ToList();

            foreach (var item in nhomDichvu)
            {
                var giaTheoNhom = giaBenhVienEntity.Where(x => x.NhomGiaDichVuGiuongBenhVienId.Equals(item.Id)).ToList();
                if (giaTheoNhom.Count > 0)
                {
                    for (int i = 1; i < giaTheoNhom.Count; i++)
                    {
                        giaTheoNhom[i - 1].DenNgay = Convert.ToDateTime(giaTheoNhom[i].TuNgay).Date.AddDays(-1);
                    }
                }
            }

        }
        public async Task<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> GetAfterEntity(long id)
        {
            var lstEntity = await BaseRepository.Table.OrderByDescending(p => p.Id).ToListAsync();
            var isGetItem = false;
            foreach (var item in lstEntity)
            {
                if (isGetItem)
                {
                    return item;
                }

                if (item.Id == id)
                {
                    isGetItem = true;
                }
            }

            return new Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien();
        }
        public async Task<List<LookupItemVo>> GetNhomDichVu()
        {
            var lst = await _nhomGiaBenhVien.TableNoTracking
                .ToListAsync();

            var query = lst.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id
            }).ToList();

            return query;
        }
        public async Task<List<NhomGiaDichVuGiuongBenhVien>> NhomGiaDichVuGiuongBenhVien()
        {
            var lstEntity = await _nhomGiaBenhVien.Table.ToListAsync();


            return lstEntity;
        }
        public Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien CheckExistDichVuGiuongBenhVien(long id)
        {
            var result = BaseRepository.TableNoTracking.Where(x => x.DichVuGiuongId == id).FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public async Task UpdateLastEntity(long id, DateTime tuNgay, long? dichVuGiuongId)
        {
            var lastEntity = await BaseRepository.Table
                .Where(s => s.DichVuGiuongId == dichVuGiuongId)
                //.OrderByDescending(p => p.TuNgay)
                .Skip(1)
                .FirstOrDefaultAsync();

            if (lastEntity != null)
            {
                //lastEntity.DenNgay = tuNgay.AddDays(-1);
                await BaseRepository.UpdateAsync(lastEntity);
            }
        }

        public async Task<bool> IsTuNgayValid(DateTime? tuNgay, long? id, long? dichVuGiuongId)
        {
            if (tuNgay == null) return true;

            if (id == 0)
            {
                var item = await BaseRepository.TableNoTracking
                    .Where(s => s.DichVuGiuongId == dichVuGiuongId).FirstOrDefaultAsync();
                /* .OrderByDescending(p => p.TuNgay).FirstOrDefaultAsync()*/
                if (item != null)
                {
                    //if (tuNgay.GetValueOrDefault().Date <= item.TuNgay.GetValueOrDefault().Date)
                    //{
                    //    return false;
                    //}
                    //if (item.DenNgay != null)
                    //{
                    //    if (tuNgay.GetValueOrDefault().Date <= item.DenNgay.GetValueOrDefault().Date)
                    //    {
                    //        return false;
                    //    }
                    //}
                }
                return true;
            }
            return true;
        }

        private string RemoveDisplaySort(QueryInfo queryInfo)
        {
            var result = queryInfo.SortString;
            if (queryInfo.SortString.Contains("HienThi"))
            {
                result = queryInfo.SortString.Replace("HienThi", "");
            }
            return result;
        }

        public async Task<List<KhoaKhamTemplateVo>> GetKhoaKhamTheoDichVuGiuongBenhVien(DropDownListRequestModel model)
        {
            var lst = await _khoaPhongRepository.TableNoTracking
                .Where(p => p.IsDisabled != true)
                .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
                .Take(model.Take)
                .ToListAsync();

            var lstKhoaPhongDaChon = await _dichVuGiuongBenhVienNoiThucHienRepository.TableNoTracking
                .Where(x => x.DichVuGiuongBenhVienId == model.Id && x.KhoaPhongId != null
                                                                   && !lst.Select(a => a.Id.ToString()).Contains(x.KhoaPhongId.ToString())).Select(x => x.KhoaPhongId.ToString()).ToListAsync();
            if (lstKhoaPhongDaChon.Any())
            {
                var lstDaChon = await _khoaPhongRepository.TableNoTracking
                    .Where(p => lstKhoaPhongDaChon.Contains(p.Id.ToString()))
                    .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
                    .ToListAsync();

                lst.AddRange(lstDaChon);
            }

            var query = lst.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma,
            }).ToList();

            return query;
        }

        public async Task<List<NoiThucHienDichVuBenhVienVo>> GetPhongKhamTheoDichVuGiuongBenhVien(DropDownListRequestModel model)
        {
            //var lst = await _phongBenhVienRepository.TableNoTracking
            //    .Where(p => p.IsDisabled != true)
            //    .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstPhongBenhVienDaChon = await _dichVuGiuongBenhVienNoiThucHienRepository.TableNoTracking
            //    .Where(x => x.DichVuGiuongBenhVienId == model.Id && x.PhongBenhVienId != null
            //                                                       && !lst.Select(a => a.Id.ToString()).Contains(x.PhongBenhVienId.ToString())).Select(x => x.PhongBenhVienId.ToString()).ToListAsync();
            //if (lstPhongBenhVienDaChon.Any())
            //{
            //    var lstDaChon = await _phongBenhVienRepository.TableNoTracking
            //        .Where(p => lstPhongBenhVienDaChon.Contains(p.Id.ToString()))
            //        .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
            //        .ToListAsync();

            //    lst.AddRange(lstDaChon);
            //}

            //var query = lst.Select(item => new KhoaKhamTemplateVo
            //{
            //    DisplayName = item.Ma + " - " + item.Ten,
            //    KeyId = item.Id,
            //    Ten = item.Ten,
            //    Ma = item.Ma,
            //}).ToList();

            //return query;

            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return new List<NoiThucHienDichVuBenhVienVo>();
            }

            var lstKhoaIdDaChon = JsonConvert.DeserializeObject<List<KhoaDaChonVo>>(model.ParameterDependencies);
            var lstKhoa = await _khoaPhongRepository.TableNoTracking
                .Include(x => x.PhongBenhViens)
                .Where(x => lstKhoaIdDaChon.Any(i => i.KhoaId == x.Id)
                            //&& (!lstKhoaDaLuu.Any() || lstKhoaDaLuu.Any(i => i == x.Id))
                            && (string.IsNullOrEmpty(model.Query) || !string.IsNullOrEmpty(model.Query) && ((x.Ma + " - " + x.Ten).ToLower().RemoveVietnameseDiacritics().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())
                            || x.PhongBenhViens.Any(y => (y.Ma + " - " + y.Ten).ToLower().RemoveVietnameseDiacritics().Contains(model.Query.RemoveVietnameseDiacritics().ToLower()))))) // thiếu điều kiện
                .ToArrayAsync();

            var lstPhongTheoKhoa = new List<NoiThucHienDichVuBenhVienVo>();
            var templateKeyId = "\"KhoaId\": {0}, \"PhongId\": {1}";
            foreach (var item in lstKhoa)
            {
                // get list phòng thuộc khoa hiện tại
                var lstItems = item.PhongBenhViens.Where(x => x.IsDisabled != true).Select(items =>
                    new NoiThucHienDichVuBenhVienVo()
                    {
                        DisplayName = items.Ten,//items.Ma + " - " + items.Ten,
                        KeyId = "{" + string.Format(templateKeyId, items.KhoaPhongId, items.Id) + "}",
                        Ma = items.Ma,
                        Ten = items.Ten,
                        KhoaId = items.KhoaPhongId
                    }).ToList();

                // thêm khoa vào list nơi thực hiện
                var khoa = new NoiThucHienDichVuBenhVienVo()
                {
                    DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                    KeyId = "{" + string.Format(templateKeyId, item.Id, "\"\"") + "}",
                    Ma = item.Ma,
                    Ten = item.Ten,
                    KhoaId = null,
                    Items = lstItems
                };
                lstPhongTheoKhoa.Add(khoa);


                // thêm phòng thuộc khoa vào list nơi thực hiện
                if (item.PhongBenhViens.Any())
                {
                    var lstPhong = item.PhongBenhViens.Where(x => x.IsDisabled != true);
                    if (!string.IsNullOrEmpty(model.Query))
                    {
                        if (!khoa.DisplayName.ToLower().RemoveVietnameseDiacritics().Contains(model.Query.RemoveVietnameseDiacritics().ToLower()))
                        {
                            lstPhong = lstPhong.Where(x =>
                                (x.Ma + " - " + x.Ten).ToLower().RemoveVietnameseDiacritics()
                                .Contains(model.Query.RemoveVietnameseDiacritics().ToLower()));
                        }
                    }
                    lstPhongTheoKhoa.AddRange(lstPhong.Select(items =>
                        new NoiThucHienDichVuBenhVienVo()
                        {
                            DisplayName = items.Ten,//items.Ma + " - " + items.Ten,
                            KeyId = "{" + string.Format(templateKeyId, items.KhoaPhongId, items.Id) + "}",
                            Ma = items.Ma,
                            Ten = items.Ten,
                            KhoaId = items.KhoaPhongId,
                            KhoaKeyId = "{" + string.Format(templateKeyId, items.KhoaPhongId, "\"\"") + "}",
                            Items = lstItems,
                            CountItems = lstItems.Count
                        }));
                }
            }

            return lstPhongTheoKhoa;
        }

        public async Task<bool> IsExistsMaDichVuGiuongBenhVien(long dichVuGiuongBenhVienId, string ma)
        {
            if (dichVuGiuongBenhVienId != 0 || string.IsNullOrEmpty(ma))
                return false;
            var dichVuGiuongBenhVien =
                await BaseRepository.TableNoTracking.AnyAsync(x => x.Ma.Trim().ToLower() == ma.Trim().ToLower());
            return dichVuGiuongBenhVien;
        }

        public List<LookupItemVo> GetListLoaiGiuongAsync(DropDownListRequestModel model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumLoaiGiuong>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return result;
        }

        public virtual byte[] ExportDichVuGiuongBenhVien(GridDataSource gridDataSource)
        {
            var datas = (ICollection<DichVuGiuongGridVo>)gridDataSource.Data;

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DichVuGiuongGridVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Dịch Vụ Giường Tại Bệnh Viện");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 20;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 20;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A1:Q1"])
                    {
                        range.Worksheet.Cells["A1:Q1"].Merge = true;
                        range.Worksheet.Cells["A1:Q1"].Value = "Dịch Vụ Giường Tại Bệnh Viện";
                        range.Worksheet.Cells["A1:Q1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:Q1"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A1:Q1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:Q1"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A2:Q3"])
                    {
                        range.Worksheet.Cells["A2:Q3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:Q3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:Q3"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A2:Q3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:Q3"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A2:Q3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A2:A3"].Merge = true;
                        range.Worksheet.Cells["A2:A3"].Value = "STT";
                        range.Worksheet.Cells["A2:A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:A3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B2:B3"].Merge = true;
                        range.Worksheet.Cells["B2:B3"].Value = "Ánh xạ";
                        range.Worksheet.Cells["B2:B3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B2:B3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C2:F2"].Merge = true;
                        range.Worksheet.Cells["C2:F2"].Value = "Thông tin dịch vụ giường ánh xạ";
                        range.Worksheet.Cells["C2:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C2:F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C3:C3"].Merge = true;
                        range.Worksheet.Cells["C3:C3"].Value = "Mã Giường";
                        range.Worksheet.Cells["C3:C3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C3:C3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D3:D3"].Merge = true;
                        range.Worksheet.Cells["D3:D3"].Value = "Tên Giường";
                        range.Worksheet.Cells["D3:D3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D3:D3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E3:E3"].Merge = true;
                        range.Worksheet.Cells["E3:E3"].Value = "Mã TT37";
                        range.Worksheet.Cells["E3:E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E3:E3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F3:F3"].Merge = true;
                        range.Worksheet.Cells["F3:F3"].Value = "Mô tả";
                        range.Worksheet.Cells["F3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F3:F3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G2:G3"].Merge = true;
                        range.Worksheet.Cells["G2:G3"].Value = "Mã Giường";
                        range.Worksheet.Cells["G2:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G2:G3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["H2:H3"].Merge = true;
                        range.Worksheet.Cells["H2:H3"].Value = "Tên Giường";
                        range.Worksheet.Cells["H2:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H2:H3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I2:I3"].Merge = true;
                        range.Worksheet.Cells["I2:I3"].Value = "Đơn giá BH";
                        range.Worksheet.Cells["I2:I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["I2:I3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J2:J3"].Merge = true;
                        range.Worksheet.Cells["J2:J3"].Value = "TLTT";
                        range.Worksheet.Cells["J2:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["J2:J3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K2:K3"].Merge = true;
                        range.Worksheet.Cells["K2:K3"].Value = "Ngày bắt đầu";
                        range.Worksheet.Cells["K2:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K2:K3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L2:L3"].Merge = true;
                        range.Worksheet.Cells["L2:L3"].Value = "Đơn giá thường";
                        range.Worksheet.Cells["L2:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L2:L3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M2:M3"].Merge = true;
                        range.Worksheet.Cells["M2:M3"].Value = "Ngày bắt đầu";
                        range.Worksheet.Cells["M2:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["M2:M3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N2:N3"].Merge = true;
                        range.Worksheet.Cells["N2:N3"].Value = "Đơn giá bao phòng";
                        range.Worksheet.Cells["N2:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["N2:N3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O2:O3"].Merge = true;
                        range.Worksheet.Cells["O2:O3"].Value = "Ngày bắt đầu";
                        range.Worksheet.Cells["O2:O3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O2:O3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P2:P3"].Merge = true;
                        range.Worksheet.Cells["P2:P3"].Value = "Hiệu lực";
                        range.Worksheet.Cells["P2:P3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["P2:P3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q2:Q3"].Merge = true;
                        range.Worksheet.Cells["Q2:Q3"].Value = "Loại Giường";
                        range.Worksheet.Cells["Q2:Q3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q2:Q3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<DichVuGiuongGridVo>(requestProperties);
                    var stt = 1;
                    int index = 4;  

                    if (datas.Any())
                    {
                        var nhomdichVuKhamBenh = _nhomGiaBenhVien.TableNoTracking.ToList();

                        var nhomThuongId = nhomdichVuKhamBenh.Where(z => z.Ten.ToUpper() == ("Thường").ToUpper()).Select(c => c.Id).FirstOrDefault();
                        var nhomBaoPhongId = nhomdichVuKhamBenh.Where(z => z.Ten.ToUpper() == ("Bao phòng").ToUpper()).Select(c => c.Id).FirstOrDefault();

                        foreach (var item in datas)
                        {
                            var dichVuGiuongBVGiaBHs = item.DichVuGiuongBenhVienGiaBaoHiems.Where(o => o.TuNgay.Date <= DateTime.Now.Date && 
                                 (o.DenNgay == null || o.DenNgay?.Date >= DateTime.Now)).ToList();
                            var dichVuGiuongBVs = item.DichVuGiuongBenhVienGiaBenhViens.Where(z => z.TuNgay.Date <= DateTime.Now.Date && 
                                 (z.DenNgay == null || z.DenNgay?.Date >= DateTime.Now)).ToList();

                            var ngayTiLeBaoHiemThanhToan = string.Join("; ", dichVuGiuongBVGiaBHs.Select(c => c.TuNgay.FormatNgayTimKiemTrenBaoCao()));
                            var tiLeBaoHiemThanhToan = string.Join("; ", dichVuGiuongBVGiaBHs.Select(c => c.TiLeBaoHiemThanhToan));

                            var ngayDonGiaGiuongThuong = string.Join("; ", dichVuGiuongBVs.Where(c => c.NhomGiaDichVuGiuongBenhVienId == nhomThuongId && c.TuNgay != null).Select(c => c.TuNgay.FormatNgayTimKiemTrenBaoCao()));
                            var giaGiuongThuong = string.Join("; ", dichVuGiuongBVs.Where(c => c.NhomGiaDichVuGiuongBenhVienId == nhomThuongId).Select(c => c.Gia.ApplyFormatMoneyVND()));

                            var ngayDonGiaGiuongBaoPhong = string.Join("; ", dichVuGiuongBVs.Where(c => c.NhomGiaDichVuGiuongBenhVienId == nhomBaoPhongId && c.TuNgay != null).Select(c => c.TuNgay.FormatNgayTimKiemTrenBaoCao()));
                            var giaGiuongBaoPhong = string.Join("; ", dichVuGiuongBVs.Where(c => c.NhomGiaDichVuGiuongBenhVienId == nhomBaoPhongId).Select(c => c.Gia.ApplyFormatMoneyVND()));

                            using (var range = worksheet.Cells["A" + index + ":Q" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["B" + index].Value = item.AnhXa == true ? "x" : string.Empty;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.AnhXa == true ? item.Ma : string.Empty;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.AnhXa == true ? item.Ten : string.Empty;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.AnhXa == true ? item.MaTT37 : string.Empty;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.MoTa;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.Ma;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.Ten;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.GiaBaoHiems;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["J" + index].Value = tiLeBaoHiemThanhToan;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["K" + index].Value = ngayTiLeBaoHiemThanhToan;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = giaGiuongThuong;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["M" + index].Value = ngayDonGiaGiuongThuong;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = giaGiuongBaoPhong;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["O" + index].Value = ngayDonGiaGiuongThuong;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["P" + index].Value = item.HieuLucHienThi;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["Q" + index].Value = item.LoaiGiuongDisplay;

                                index++;
                                stt++;
                            }
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        #region BVHD-3937
        public async Task<GiaDichVuBenhVieDataImportVo> XuLyKiemTraDataGiaDichVuBenhVienImportAsync(GiaDichVuBenhVienFileImportVo info)
        {
            var result = new GiaDichVuBenhVieDataImportVo();
            var datas = new List<ThongTinGiaDichVuTuFileExcelVo>();

            #region get data từ file excel
            using (ExcelPackage package = new ExcelPackage(info.Path))
            {
                ExcelWorksheet workSheeGiaDichVu = package.Workbook.Worksheets["Giá dịch vụ"];
                if (workSheeGiaDichVu == null)
                {
                    throw new Exception("Thông tin file không đúng");
                }

                int totalRowKham = workSheeGiaDichVu.Dimension.Rows; // count row có data
                if (totalRowKham >= 2) // row 1 là title, data bắt đầu từ row 2
                {
                    for (int i = 2; i <= totalRowKham; i++)
                    {
                        var dichVu = GanDataTheoRow(workSheeGiaDichVu, i);
                        if (dichVu == null)
                        {
                            break;
                        }
                        datas.Add(dichVu);
                    }
                }
            }
            #endregion

            #region Kiểm tra data
            if (datas.Any())
            {
                await KiemTraDataGiaDichVuBenhVienImportAsync(datas, result);
            }
            #endregion

            return result;
        }

        private ThongTinGiaDichVuTuFileExcelVo GanDataTheoRow(ExcelWorksheet workSheet, int index)
        {
            var dichVu = new ThongTinGiaDichVuTuFileExcelVo()
            {
                MaDichVuBenhVien = workSheet.Cells[index, 1].Text?.Trim(),
                TenDichVuBenhVien = workSheet.Cells[index, 2].Text?.Trim(),
                LoaiGia = workSheet.Cells[index, 3].Text?.Trim(),
                GiaBaoHiem = workSheet.Cells[index, 4].Text?.Trim(),
                TiLeBaoHiemThanhToan = workSheet.Cells[index, 5].Text?.Trim(),
                GiaBenhVien = workSheet.Cells[index, 6].Text?.Trim(),
                TuNgay = workSheet.Cells[index, 7].Text?.Trim(),
                DenNgay = workSheet.Cells[index, 8].Text?.Trim()
            };

            if (string.IsNullOrEmpty(dichVu.MaDichVuBenhVien)
                && string.IsNullOrEmpty(dichVu.TenDichVuBenhVien)
                && string.IsNullOrEmpty(dichVu.LoaiGia)
                && string.IsNullOrEmpty(dichVu.GiaBaoHiem)
                && string.IsNullOrEmpty(dichVu.TiLeBaoHiemThanhToan)
                && string.IsNullOrEmpty(dichVu.GiaBenhVien)
                && string.IsNullOrEmpty(dichVu.TuNgay)
                && string.IsNullOrEmpty(dichVu.DenNgay))
            {
                dichVu = null;
            }
            return dichVu;
        }

        public async Task KiemTraDataGiaDichVuBenhVienImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas, GiaDichVuBenhVieDataImportVo result)
        {
            if (datas.Any())
            {
                var lstLookupDichVu = datas
                    .Select(x => new LookupDichVuBenhVienVo()
                    {
                        MaDichVu = x.MaDichVuBenhVien?.Trim().ToLower(),
                        TenDichVu = x.TenDichVuBenhVien?.Trim().ToLower()
                    }).Distinct().ToList();

                var lstDichVuBenhVien = new List<ThongTinDichVuBenhVienVo>();
                if (lstLookupDichVu.Any())
                {
                    var lstDichVuGiuongBenhVien = BaseRepository.TableNoTracking
                        .Select(x => new ThongTinDichVuBenhVienVo()
                        {
                            DichVuBenhVienId = x.Id,
                            MaDichVu = x.Ma,
                            TenDichVu = x.Ten
                        }).ToList();
                    lstDichVuGiuongBenhVien = lstDichVuGiuongBenhVien
                        .Where(x => lstLookupDichVu.Any(a => a.MaDichVu.Equals(x.MaDichVu.Trim().ToLower())
                                                              && a.TenDichVu.Equals(x.TenDichVu.Trim().ToLower())))
                        .ToList();
                    if (lstDichVuGiuongBenhVien.Any())
                    {
                        var lstDichVuId = lstDichVuGiuongBenhVien.Select(x => x.DichVuBenhVienId).Distinct().ToList();
                        var lstGiaBenhVien = _dichVuGiuongGiaBenhVien.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuGiuongBenhVienId))
                            .Select(a => new ThongTinGiaBenhVienVo()
                            {
                                DichVuBenhVienId = a.DichVuGiuongBenhVienId,
                                NhomGiaId = a.NhomGiaDichVuGiuongBenhVienId,
                                TenLoaiGia = a.NhomGiaDichVuGiuongBenhVien.Ten,
                                Gia = a.Gia,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();
                        var lstGiaBaoHiem = _dichVuGiuongGiaBaoHiem.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuGiuongBenhVienId))
                            .Select(a => new ThongTinGiaBaoHiemVo()
                            {
                                DichVuBenhVienId = a.DichVuGiuongBenhVienId,
                                Gia = a.Gia,
                                TiLeBaoHiemThanhToan = a.TiLeBaoHiemThanhToan,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();

                        foreach (var dichVuBenhVien in lstDichVuGiuongBenhVien)
                        {
                            var lstGiaTheoBenhVien = lstGiaBenhVien.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            var lstGiaTheoBaoHiem = lstGiaBaoHiem.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();


                            dichVuBenhVien.ThongTinGiaBenhViens = lstGiaTheoBenhVien;
                            dichVuBenhVien.ThongTinGiaBaoHiems = lstGiaTheoBaoHiem;
                        }
                        lstDichVuBenhVien = lstDichVuBenhVien.Concat(lstDichVuGiuongBenhVien).ToList();
                    }
                }

                var lstNhomGiaBenhVien = _nhomGiaBenhVien.TableNoTracking
                    .Select(x => new LookupItemVo()
                    {
                        KeyId = x.Id,
                        DisplayName = x.Ten
                    }).ToList();

                #region Message
                var maDichVuBenhVienRequired = _localizationService.GetResource("ImportGiaDichVu.MaDichVuBenhVien.Required"); //"Mã dịch vụ bệnh viện yêu cầu nhập"
                var tenDichVuBenhVienRequired = _localizationService.GetResource("ImportGiaDichVu.TenDichVuBenhVien.Required"); //"Tên dịch vụ bệnh viện yêu cầu nhập"
                var dichVuBenhVienNotExists = _localizationService.GetResource("ImportGiaDichVu.TenDichVuBenhVien.NotExists"); //"Dịch vụ bệnh viện không tồn tại"

                var loaiGiaRequired = _localizationService.GetResource("ImportGiaDichVu.LoaiGia.Required"); // "Loại giá yêu cầu nhập"
                var loaiGiaNotExists = _localizationService.GetResource("ImportGiaDichVu.LoaiGia.NotExists"); //"Loại giá không tồn tại"
                var loaiGiaRequiredGiaBenhVien = _localizationService.GetResource("ImportGiaDichVu.LoaiGia.RequiredGiaBenhVien"); //"Loại giá chỉ nhập đối với giá bệnh viện"

                var giaBaoHiemRequired = _localizationService.GetResource("ImportGiaDichVu.GiaBaoHiem.Required"); // "Giá bảo hiểm yêu cầu nhập"
                var giaBaoHiemOrBenhVienRequired = _localizationService.GetResource("ImportGiaDichVu.GiaBaoHiemOrBenhVien.Required"); //"Yêu cầu nhập giá bảo hiểm hoặc bệnh viện"
                var giaBaoHiemFormat = _localizationService.GetResource("ImportGiaDichVu.GiaBaoHiem.Format"); // "Giá bảo hiểm nhập sai định dạng"

                var tiLeBaoHiemRequired = _localizationService.GetResource("ImportGiaDichVu.TiLeBaoHiem.Required"); // "Tỉ lệ bảo hiểm thanh toán yêu cầu nhập"
                var tiLeBaoHiemOnlyForBaoHiem = _localizationService.GetResource("ImportGiaDichVu.TiLeBaoHiem.OnlyForBaoHiem"); //"Tỉ lệ bảo hiểm thanh toán chỉ nhập với giá bảo hiểm"
                var tiLeBaoHiemFormat = _localizationService.GetResource("ImportGiaDichVu.TiLeBaoHiem.Format"); // "Tỉ lệ bảo hiểm thanh toán nhập sai định dạng"

                var giaBenhVienRequired = _localizationService.GetResource("ImportGiaDichVu.GiaBenhVien.Required"); // "Giá bệnh viện yêu cầu nhập"
                var giaBenhVienFormat = _localizationService.GetResource("ImportGiaDichVu.GiaBenhVien.Format"); // "Giá bệnh viện nhập sai định dạng"

                var tuNgayRequired = _localizationService.GetResource("ImportGiaDichVu.TuNgay.Required"); // "Từ ngày yêu cầu nhập"
                var tuNgayFormat = _localizationService.GetResource("ImportGiaDichVu.TuNgay.Format"); // "Từ ngày nhập sai định dạng"
                var tuNgayLessThanDenNgay = _localizationService.GetResource("ImportGiaDichVu.TuNgay.LessThanDenNgay"); // "Từ ngày phải nhỏ hơn đến ngày"
                var tuNgayInvalid = _localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"); // "Từ ngày không hợp lệ"
                var tuNgayDuplicate = _localizationService.GetResource("ImportGiaDichVu.TuNgay.Duplicate"); // "Từ ngày bị trùng"

                var denNgayFormat = _localizationService.GetResource("ImportGiaDichVu.DenNgay.Format"); // "Đến ngày nhập sai định dạng"
                var denNgayInvalid = _localizationService.GetResource("ImportGiaDichVu.DenNgay.Invalid"); // "Đến ngày không hợp lệ"
                #endregion

                foreach (var dichVu in datas)
                {
                    dichVu.ValidationErrors = new List<ValidationErrorGiaDichVuVo>();

                    #region Gán lại thông tin giá cho data dịch vụ từ file excel

                    var thongTinDichVu = lstDichVuBenhVien.FirstOrDefault(x =>
                        x.MaDichVu.Trim().ToLower().Equals(dichVu.MaDichVuBenhVien.ToLower())
                        && x.TenDichVu.Trim().ToLower().Equals(dichVu.TenDichVuBenhVien.ToLower()));
                    if (thongTinDichVu != null)
                    {
                        dichVu.DichVuBenhVienId = thongTinDichVu.DichVuBenhVienId;

                        if (!string.IsNullOrEmpty(dichVu.LoaiGia))
                        {
                            var loaiGia = lstNhomGiaBenhVien.FirstOrDefault(x =>
                                x.DisplayName.Trim().ToLower().Equals(dichVu.LoaiGia.ToLower()));
                            if (loaiGia != null)
                            {
                                dichVu.LoaiGiaId = loaiGia.KeyId;
                            }

                            //Cập nhật 13/06/2022: Cập nhật theo yêu cầu tạo mới loại giá nếu chưa có
                            else
                            {
                                var newNhomGia = new NhomGiaDichVuGiuongBenhVien()
                                {
                                    Ten = dichVu.LoaiGia.Trim()
                                };
                                _nhomGiaBenhVien.Add(newNhomGia);
                                dichVu.LoaiGiaId = newNhomGia.Id;
                                lstNhomGiaBenhVien.Add(new LookupItemVo()
                                {
                                    KeyId = newNhomGia.Id,
                                    DisplayName = newNhomGia.Ten
                                });
                            }
                        }
                    }

                    #endregion
                }
                foreach (var dichVu in datas)
                {
                    #region Kiểm tra yêu cầu nhập và format dữ liệu

                    #region Mã dịch vụ bệnh viện
                    if (string.IsNullOrEmpty(dichVu.MaDichVuBenhVien))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                        {
                            Field = nameof(dichVu.MaDichVuBenhVien),
                            Message = maDichVuBenhVienRequired
                        });
                    }
                    #endregion

                    #region Tên dịch vụ bệnh viện
                    if (string.IsNullOrEmpty(dichVu.TenDichVuBenhVien))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                        {
                            Field = nameof(dichVu.TenDichVuBenhVien),
                            Message = tenDichVuBenhVienRequired
                        });
                    }
                    else
                    {
                        if (dichVu.DichVuBenhVienId == null)
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.TenDichVuBenhVien),
                                Message = dichVuBenhVienNotExists
                            });
                        }
                    }
                    #endregion

                    #region Loại giá
                    if (string.IsNullOrEmpty(dichVu.LoaiGia))
                    {
                        if (!string.IsNullOrEmpty(dichVu.GiaBenhVien))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.LoaiGia),
                                Message = loaiGiaRequired
                            });
                        }
                    }
                    else
                    {
                        if (!lstNhomGiaBenhVien.Any(x => x.DisplayName.ToLower().Equals(dichVu.LoaiGia.ToLower())))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.LoaiGia),
                                Message = loaiGiaNotExists
                            });
                        }
                        else if (!string.IsNullOrEmpty(dichVu.GiaBaoHiem))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.LoaiGia),
                                Message = loaiGiaRequiredGiaBenhVien
                            });
                        }
                    }
                    #endregion

                    #region Giá bảo hiểm
                    if (string.IsNullOrEmpty(dichVu.GiaBaoHiem))
                    {
                        if (!string.IsNullOrEmpty(dichVu.TiLeBaoHiemThanhToan))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.GiaBaoHiem),
                                Message = giaBaoHiemRequired
                            });
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(dichVu.LoaiGia) && string.IsNullOrEmpty(dichVu.GiaBenhVien))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                {
                                    Field = nameof(dichVu.GiaBaoHiem),
                                    Message = giaBaoHiemOrBenhVienRequired
                                });
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dichVu.LoaiGia) || !string.IsNullOrEmpty(dichVu.GiaBenhVien))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.GiaBaoHiem),
                                Message = giaBaoHiemOrBenhVienRequired
                            });
                        }
                        else
                        {
                            var isNumeric = decimal.TryParse(dichVu.GiaBaoHiem, out decimal giaBaoHiem);
                            if (isNumeric)
                            {
                                dichVu.GiaBaoHiemValue = giaBaoHiem;
                            }
                            else
                            {
                                if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.GiaBaoHiem)))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                    {
                                        Field = nameof(dichVu.GiaBaoHiem),
                                        Message = giaBaoHiemFormat
                                    });
                                }
                            }
                        }
                    }
                    #endregion

                    #region Tỉ lệ bảo hiểm thanh toán
                    if (string.IsNullOrEmpty(dichVu.TiLeBaoHiemThanhToan))
                    {
                        if (!string.IsNullOrEmpty(dichVu.GiaBaoHiem))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.TiLeBaoHiemThanhToan),
                                Message = tiLeBaoHiemRequired
                            });
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dichVu.LoaiGia) || !string.IsNullOrEmpty(dichVu.GiaBenhVien))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.TiLeBaoHiemThanhToan),
                                Message = tiLeBaoHiemOnlyForBaoHiem
                            });
                        }
                        else
                        {
                            var isNumeric = int.TryParse(dichVu.TiLeBaoHiemThanhToan, out int tlbhtt);
                            if (isNumeric)
                            {
                                dichVu.TiLeBaoHiemThanhToanValue = tlbhtt;
                            }
                            else
                            {
                                if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TiLeBaoHiemThanhToan)))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                    {
                                        Field = nameof(dichVu.TiLeBaoHiemThanhToan),
                                        Message = tiLeBaoHiemFormat
                                    });
                                }
                            }
                        }
                    }
                    #endregion

                    #region Giá bệnh viện
                    if (string.IsNullOrEmpty(dichVu.GiaBenhVien))
                    {
                        if (!string.IsNullOrEmpty(dichVu.LoaiGia))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.GiaBenhVien),
                                Message = giaBenhVienRequired
                            });
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dichVu.LoaiGia))
                        {
                            var isNumeric = decimal.TryParse(dichVu.GiaBenhVien, out decimal giaBenhVien);
                            if (isNumeric)
                            {
                                dichVu.GiaBenhVienValue = giaBenhVien;
                            }
                            else
                            {
                                if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.GiaBenhVien)))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                    {
                                        Field = nameof(dichVu.GiaBenhVien),
                                        Message = giaBenhVienFormat
                                    });
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(dichVu.GiaBaoHiem) || !string.IsNullOrEmpty(dichVu.TiLeBaoHiemThanhToan))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.GiaBenhVien),
                                Message = giaBaoHiemOrBenhVienRequired
                            });
                        }
                    }
                    #endregion

                    #region Từ ngày
                    if (string.IsNullOrEmpty(dichVu.TuNgay))
                    {
                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                        {
                            Field = nameof(dichVu.TuNgay),
                            Message = tuNgayRequired
                        });
                    }
                    else
                    {
                        //var isDate = DateTime.TryParse(dichVu.TuNgay, out DateTime tuNgay);
                        DateTime tuNgay;
                        var strNgay = KiemTraFormatNgay(dichVu.TuNgay);

                        var isDate = DateTime.TryParseExact(strNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tuNgay);
                        if (isDate)
                        {
                            dichVu.TuNgayValue = tuNgay;
                        }
                        else
                        {
                            if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                {
                                    Field = nameof(dichVu.TuNgay),
                                    Message = tuNgayFormat
                                });
                            }
                        }
                    }
                    #endregion

                    #region Đến ngày
                    if (!string.IsNullOrEmpty(dichVu.DenNgay))
                    {
                        //var isDate = DateTime.TryParse(dichVu.DenNgay, out DateTime denNgay);
                        DateTime denNgay;
                        var strNgay = KiemTraFormatNgay(dichVu.DenNgay);

                        var isDate = DateTime.TryParseExact(strNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out denNgay);
                        if (isDate)
                        {
                            dichVu.DenNgayValue = denNgay;
                        }
                        else
                        {
                            if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.DenNgay)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                {
                                    Field = nameof(dichVu.DenNgay),
                                    Message = denNgayFormat
                                });
                            }
                        }
                    }
                    #endregion

                    #region Từ ngày đến ngày
                    if (dichVu.TuNgayValue != null && dichVu.DenNgayValue != null && dichVu.TuNgayValue >= dichVu.DenNgayValue)
                    {
                        if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                        {
                            dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                            {
                                Field = nameof(dichVu.TuNgay),
                                Message = tuNgayLessThanDenNgay
                            });
                        }
                    }
                    #endregion

                    #region Kiểm tra thời gian hiệu lực theo từng dịch vụ
                    var dichVuBenhVien = lstDichVuBenhVien.FirstOrDefault(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId);
                    if (dichVuBenhVien != null)
                    {
                        if (dichVu.LoaiGiaId != null)
                        {
                            var giaBenhVienTheoDichVuHienTai = dichVuBenhVien.ThongTinGiaBenhViens.Where(x => x.NhomGiaId == dichVu.LoaiGiaId).OrderByDescending(x => x.TuNgay).FirstOrDefault();
                            if (giaBenhVienTheoDichVuHienTai != null)
                            {
                                // nếu từ ngày excel = từ ngày hiện tại => gán đến ngày theo file excel
                                // nếu từ ngày excel != từ ngày hiện tại => gán đến ngày = từ ngày file excel -1
                                //if (dichVu.TuNgayValue <= giaBenhVienTheoDichVuHienTai.TuNgay.AddDays(1))

                                var tuNgayHopLe = (dichVu.TuNgayValue == giaBenhVienTheoDichVuHienTai.TuNgay)
                                                  || (dichVu.TuNgayValue != giaBenhVienTheoDichVuHienTai.TuNgay
                                                      && dichVu.TuNgayValue > giaBenhVienTheoDichVuHienTai.TuNgay.AddDays(1));
                                if (!tuNgayHopLe)
                                {
                                    if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                                    {
                                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                        {
                                            Field = nameof(dichVu.TuNgay),
                                            Message = tuNgayInvalid
                                        });
                                    }
                                }

                                if (dichVu.DenNgayValue != null && dichVu.DenNgayValue <= giaBenhVienTheoDichVuHienTai.TuNgay.AddDays(1))
                                {
                                    if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.DenNgay)))
                                    {
                                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                        {
                                            Field = nameof(dichVu.DenNgay),
                                            Message = denNgayInvalid
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            var giaBaoHiemTheoDichVuHienTai = dichVuBenhVien.ThongTinGiaBaoHiems.OrderByDescending(x => x.TuNgay).FirstOrDefault();
                            if (giaBaoHiemTheoDichVuHienTai != null)
                            {
                                // nếu từ ngày excel = từ ngày hiện tại => gán đến ngày theo file excel
                                // nếu từ ngày excel != từ ngày hiện tại => gán đến ngày = từ ngày file excel -1
                                //if (dichVu.TuNgayValue <= giaBaoHiemTheoDichVuHienTai.TuNgay.AddDays(1))

                                var tuNgayHopLe = (dichVu.TuNgayValue == giaBaoHiemTheoDichVuHienTai.TuNgay)
                                                  || (dichVu.TuNgayValue != giaBaoHiemTheoDichVuHienTai.TuNgay
                                                      && dichVu.TuNgayValue > giaBaoHiemTheoDichVuHienTai.TuNgay.AddDays(1));
                                if (!tuNgayHopLe)
                                {
                                    if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                                    {
                                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                        {
                                            Field = nameof(dichVu.TuNgay),
                                            Message = tuNgayInvalid
                                        });
                                    }
                                }

                                if (dichVu.DenNgayValue != null && dichVu.DenNgayValue <= giaBaoHiemTheoDichVuHienTai.TuNgay.AddDays(1))
                                {
                                    if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.DenNgay)))
                                    {
                                        dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                        {
                                            Field = nameof(dichVu.DenNgay),
                                            Message = denNgayInvalid
                                        });
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion
                }

                #region Kiểm tra thời gian hiệu lực theo từng dịch vụ import
                foreach (var dichVu in datas)
                {
                    var dichVuBenhVien =
                        lstDichVuBenhVien.FirstOrDefault(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId);
                    if (dichVuBenhVien != null)
                    {
                        if (!KiemTraDaCoValidationErrors(dichVu.ValidationErrors, nameof(dichVu.TuNgay)))
                        {
                            bool laGiaBenhVien = (dichVu.LoaiGiaId != null || dichVu.GiaBenhVienValue != null)
                                                 && dichVu.GiaBaoHiemValue == null
                                                 && dichVu.TiLeBaoHiemThanhToanValue == null;
                            bool laGiaBaoHiem = (dichVu.GiaBaoHiemValue != null || dichVu.TiLeBaoHiemThanhToanValue != null)
                                                && dichVu.LoaiGiaId == null
                                                && dichVu.GiaBenhVienValue == null;

                            var lstGiaBaoHiemImport = datas.Where(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId
                                                                       && x.TuNgayValue != null
                                                                       && x.LoaiGiaId == null
                                                                       && x.GiaBenhVienValue == null
                                                                       && (x.GiaBaoHiemValue != null
                                                                           || x.TiLeBaoHiemThanhToanValue != null))
                                .ToList();

                            var lstGiaBenhVienImport = datas.Where(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId
                                                                       && x.TuNgayValue != null
                                                                       && x.LoaiGiaId == dichVu.LoaiGiaId
                                                                       && x.GiaBaoHiemValue == null
                                                                       && x.TiLeBaoHiemThanhToanValue == null
                                                                       && (x.LoaiGiaId != null || x.GiaBenhVienValue != null))
                                .ToList();

                            if ((laGiaBaoHiem && lstGiaBaoHiemImport.GroupBy(x => new { x.TuNgayValue }).Any(x => x.Count() > 1))
                                || (laGiaBenhVien && lstGiaBenhVienImport.GroupBy(x => new { x.TuNgayValue }).Any(x => x.Count() > 1)))
                            {
                                dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                {
                                    Field = nameof(dichVu.TuNgay),
                                    Message = tuNgayDuplicate
                                });
                            }
                            else
                            {
                                // giữa 2 dòng import giá của cùng 1 dịch vụ, thì từ ngày sau phải cách từ ngày trước ít nhất 1 ngày
                                var tuNgayBenhVienTruocs = lstGiaBenhVienImport.Select(x => x.TuNgayValue.Value)
                                    .Where(x => x <= dichVu.TuNgayValue.Value)
                                    .OrderByDescending(x => x).Skip(1).ToList();
                                var tuNgayBaoHiemTruocs = lstGiaBaoHiemImport.Select(x => x.TuNgayValue.Value)
                                    .Where(x => x <= dichVu.TuNgayValue.Value)
                                    .OrderByDescending(x => x).Skip(1).ToList();

                                if ((laGiaBenhVien && tuNgayBenhVienTruocs.Any() && tuNgayBenhVienTruocs.First().AddDays(1) >= dichVu.TuNgayValue.Value)
                                    || (laGiaBaoHiem && tuNgayBaoHiemTruocs.Any() && tuNgayBaoHiemTruocs.First().AddDays(1) >= dichVu.TuNgayValue.Value))
                                {
                                    dichVu.ValidationErrors.Add(new ValidationErrorGiaDichVuVo()
                                    {
                                        Field = nameof(dichVu.TuNgay),
                                        Message = tuNgayInvalid
                                    });
                                }
                            }
                        }
                    }
                }
                #endregion

                result.DuLieuDungs = datas.Where(x => !x.ValidationErrors.Any()).OrderBy(x => x.MaDichVuBenhVien).ToList();
                result.DuLieuSais = datas.Where(x => x.ValidationErrors.Any()).OrderBy(x => x.MaDichVuBenhVien).ToList();
            }
        }

        private bool KiemTraDaCoValidationErrors(List<ValidationErrorGiaDichVuVo> validationErrors, string filed)
        {
            return validationErrors.Any(a => a.Field.Equals(filed));
        }

        private string KiemTraFormatNgay(string ngayKiemTra)
        {
            var ngaySauKiemTra = ngayKiemTra;
            var arrTuNgay = ngayKiemTra.Split("/");
            if (arrTuNgay.Length == 3)
            {
                var ngay = arrTuNgay[0];
                var thang = arrTuNgay[1];
                if (ngay.Length == 1)
                {
                    ngay = $"0{ngay}";
                }
                if (thang.Length == 1)
                {
                    thang = $"0{thang}";
                }

                ngaySauKiemTra = $"{ngay}/{thang}/{arrTuNgay[2]}";
            }

            return ngaySauKiemTra;
        }

        public async Task XuLyLuuGiaDichVuImportAsync(List<ThongTinGiaDichVuTuFileExcelVo> datas)
        {
            var lstDichVuBenhVienId = datas.Where(x => x.DichVuBenhVienId != null)
                .Select(x => x.DichVuBenhVienId.Value).Distinct().ToList();
            if (lstDichVuBenhVienId.Any())
            {
                var lstDichVuBenhVien = BaseRepository.Table
                    .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                    .Include(x => x.DichVuGiuongBenhVienGiaBenhViens)
                    .Where(x => lstDichVuBenhVienId.Contains(x.Id))
                    .ToList();
                foreach (var dichVuBenhVien in lstDichVuBenhVien)
                {
                    var lstGiaBaoHiemImport = datas.Where(x => x.DichVuBenhVienId == dichVuBenhVien.Id
                                                               && x.GiaBaoHiemValue != null
                                                               && x.TiLeBaoHiemThanhToanValue != null)
                        .OrderBy(x => x.TuNgayValue).ToList();
                    var lstGiaBenhVienImport = datas.Where(x => x.DichVuBenhVienId == dichVuBenhVien.Id
                                                                && x.LoaiGiaId != null
                                                                && x.GiaBenhVienValue != null)
                        .OrderBy(x => x.TuNgayValue).ToList();

                    #region Xử lý giá Bảo hiểm
                    var giaBaoHiemCuoiCungHienTai = dichVuBenhVien.DichVuGiuongBenhVienGiaBaoHiems.OrderByDescending(x => x.TuNgay).FirstOrDefault();
                    if (lstGiaBaoHiemImport.Any())
                    {
                        //if (giaBaoHiemCuoiCungHienTai != null && lstGiaBaoHiemImport.Any(x => x.TuNgayValue <= giaBaoHiemCuoiCungHienTai.TuNgay.AddDays(1)))

                        var tuNgayHopLe = giaBaoHiemCuoiCungHienTai == null
                                          || lstGiaBaoHiemImport.Any(x => x.TuNgayValue == giaBaoHiemCuoiCungHienTai?.TuNgay)
                                          || lstGiaBaoHiemImport.Any(x => x.TuNgayValue != giaBaoHiemCuoiCungHienTai?.TuNgay
                                                                          && x.TuNgayValue > giaBaoHiemCuoiCungHienTai?.TuNgay.AddDays(1));
                        if (!tuNgayHopLe)
                        {
                            throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                        }

                        //xử lý gán giá trị từ ngày đến ngày
                        foreach (var item in lstGiaBaoHiemImport.Select((value, index) => new { index, value }))
                        {
                            // đối với dòng giá import đầu tiên, thì so sánh với từ ngày giá trong DB
                            // đối với các dòng giá từ thứ 2 trở đi, thì so sánh với từ ngày của giá trước nó
                            if (item.index == 0)
                            {
                                if (giaBaoHiemCuoiCungHienTai != null)
                                {
                                    if (item.value.TiLeBaoHiemThanhToanValue == giaBaoHiemCuoiCungHienTai.TiLeBaoHiemThanhToan
                                        && item.value.GiaBaoHiemValue == giaBaoHiemCuoiCungHienTai.Gia)
                                    {
                                        giaBaoHiemCuoiCungHienTai.DenNgay = item.value.DenNgayValue;
                                        item.value.LaCapNhatDenNgayTruocDo = true;
                                    }
                                    else
                                    {
                                        //giaBaoHiemCuoiCungHienTai.DenNgay = item.value.TuNgayValue.Value.AddDays(-1);

                                        if (item.value.TuNgayValue == giaBaoHiemCuoiCungHienTai.TuNgay)
                                        {
                                            giaBaoHiemCuoiCungHienTai.DenNgay = DateTime.Now.Date.AddDays(-1);
                                            item.value.TuNgayValue = DateTime.Now.Date;
                                            if (item.value.DenNgayValue != null && item.value.TuNgayValue.Value.AddDays(1) >= item.value.DenNgayValue)
                                            {
                                                throw new Exception(_localizationService.GetResource("ImportGiaDichVu.DenNgay.Invalid"));
                                            }
                                        }
                                        else
                                        {
                                            giaBaoHiemCuoiCungHienTai.DenNgay = item.value.TuNgayValue.Value.AddDays(-1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (item.value.TuNgayValue <= lstGiaBaoHiemImport[item.index - 1].TuNgayValue.Value.AddDays(1))
                                {
                                    throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                                }
                                else
                                {
                                    lstGiaBaoHiemImport[item.index - 1].DenNgayValue = item.value.TuNgayValue.Value.AddDays(-1);
                                    if (giaBaoHiemCuoiCungHienTai != null && lstGiaBaoHiemImport[item.index - 1].LaCapNhatDenNgayTruocDo)
                                    {
                                        giaBaoHiemCuoiCungHienTai.DenNgay = lstGiaBaoHiemImport[item.index - 1].DenNgayValue;   
                                    }
                                }
                            }
                        }

                        foreach (var gia in lstGiaBaoHiemImport.Where(x => !x.LaCapNhatDenNgayTruocDo))
                        {
                            dichVuBenhVien.DichVuGiuongBenhVienGiaBaoHiems.Add(new DichVuGiuongBenhVienGiaBaoHiem()
                            {
                                Gia = gia.GiaBaoHiemValue.Value,
                                TiLeBaoHiemThanhToan = gia.TiLeBaoHiemThanhToanValue.Value,
                                TuNgay = gia.TuNgayValue.Value,
                                DenNgay = gia.DenNgayValue
                            });
                        }
                    }

                    #endregion

                    #region Xử lý giá Bệnh viện
                    if (lstGiaBenhVienImport.Any())
                    {
                        //xử lý gán giá trị từ ngày đến ngày
                        var lstLoaiGiaId = lstGiaBenhVienImport.Select(x => x.LoaiGiaId.Value).Distinct().ToList();
                        foreach (var loaiGiaId in lstLoaiGiaId)
                        {
                            var lstGiaBenhVienImportTheoLoaiGia = lstGiaBenhVienImport.Where(x => x.LoaiGiaId == loaiGiaId).ToList();
                            foreach (var item in lstGiaBenhVienImportTheoLoaiGia.Select((value, index) => new { index, value }))
                            {
                                var giaBenhVienCuoiCungHienTaiTheoLoaiGia = dichVuBenhVien.DichVuGiuongBenhVienGiaBenhViens
                                    .Where(x => x.NhomGiaDichVuGiuongBenhVienId == item.value.LoaiGiaId)
                                    .OrderByDescending(x => x.TuNgay).FirstOrDefault();

                                //if (giaBenhVienCuoiCungHienTaiTheoLoaiGia != null && item.value.TuNgayValue <= giaBenhVienCuoiCungHienTaiTheoLoaiGia.TuNgay.AddDays(1))

                                var tuNgayHopLe = giaBenhVienCuoiCungHienTaiTheoLoaiGia == null
                                                  || (item.value.TuNgayValue == giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay)
                                                  || (item.value.TuNgayValue != giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay
                                                      && item.value.TuNgayValue > giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay.AddDays(1));
                                if (!tuNgayHopLe)
                                {
                                    throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                                }

                                // đối với dòng giá import đầu tiên, thì so sánh với từ ngày giá trong DB
                                // đối với các dòng giá từ thứ 2 trở đi, thì so sánh với từ ngày của giá trước nó
                                if (item.index == 0)
                                {
                                    if (giaBenhVienCuoiCungHienTaiTheoLoaiGia != null)
                                    {
                                        if (item.value.GiaBenhVienValue == giaBenhVienCuoiCungHienTaiTheoLoaiGia.Gia)
                                        {
                                            giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = item.value.DenNgayValue;
                                            item.value.LaCapNhatDenNgayTruocDo = true;
                                        }
                                        else
                                        {
                                            //giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = item.value.TuNgayValue.Value.AddDays(-1);

                                            if (item.value.TuNgayValue == giaBenhVienCuoiCungHienTaiTheoLoaiGia.TuNgay)
                                            {
                                                giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = DateTime.Now.Date.AddDays(-1);
                                                item.value.TuNgayValue = DateTime.Now.Date;
                                                if (item.value.DenNgayValue != null && item.value.TuNgayValue.Value.AddDays(1) >= item.value.DenNgayValue)
                                                {
                                                    throw new Exception(_localizationService.GetResource("ImportGiaDichVu.DenNgay.Invalid"));
                                                }
                                            }
                                            else
                                            {
                                                giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = item.value.TuNgayValue.Value.AddDays(-1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.value.TuNgayValue <= lstGiaBenhVienImportTheoLoaiGia[item.index - 1].TuNgayValue.Value.AddDays(1))
                                    {
                                        throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                                    }
                                    else
                                    {
                                        lstGiaBenhVienImportTheoLoaiGia[item.index - 1].DenNgayValue = item.value.TuNgayValue.Value.AddDays(-1);
                                        if (giaBenhVienCuoiCungHienTaiTheoLoaiGia != null && lstGiaBenhVienImportTheoLoaiGia[item.index - 1].LaCapNhatDenNgayTruocDo)
                                        {
                                            giaBenhVienCuoiCungHienTaiTheoLoaiGia.DenNgay = lstGiaBenhVienImportTheoLoaiGia[item.index - 1].DenNgayValue;
                                        }
                                    }
                                }
                            }
                        }

                        foreach (var gia in lstGiaBenhVienImport.Where(x => !x.LaCapNhatDenNgayTruocDo))
                        {
                            dichVuBenhVien.DichVuGiuongBenhVienGiaBenhViens.Add(new DichVuGiuongBenhVienGiaBenhVien()
                            {
                                NhomGiaDichVuGiuongBenhVienId = gia.LoaiGiaId.Value,
                                Gia = gia.GiaBenhVienValue.Value,
                                TuNgay = gia.TuNgayValue.Value,
                                DenNgay = gia.DenNgayValue
                            });
                        }
                    }
                    #endregion
                }

                BaseRepository.Context.SaveChanges();
            }
        }

        public async Task<List<LookupItemVo>> GetNhomDichVuTheoDichVuKyThuat(DropDownListRequestModel queryInfo)
        {
            var dsKhamBenhBenhViens = new List<LookupItemVo>();
            var dichVuKyThuatBenhVien = JsonConvert.DeserializeObject<DichVuGiuongBenhVienJSON>(queryInfo.ParameterDependencies.Replace("undefined", "null"));
            if (dichVuKyThuatBenhVien != null)
            {
                var lst = await _nhomGiaBenhVien.TableNoTracking.Where(c =>
                         c.DichVuGiuongBenhVienGiaBenhViens.Any(x => x.DichVuGiuongBenhVienId == dichVuKyThuatBenhVien.DichVuGiuongBenhVienId &&
                         x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date))).ToListAsync();

                dsKhamBenhBenhViens.AddRange(lst.Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id
                }).ToList());
            }
            return dsKhamBenhBenhViens;
        }

        #endregion
    }
}

