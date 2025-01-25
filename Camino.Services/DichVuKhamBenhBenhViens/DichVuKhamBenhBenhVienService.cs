using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.DichVuKhamBenh;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Services.Localization;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Camino.Services.DichVuKhamBenhBenhViens
{
    [ScopedDependency(ServiceType = typeof(IDichVuKhamBenhBenhVienService))]
    public class DichVuKhamBenhBenhVienService : MasterFileService<DichVuKhamBenhBenhVien>, IDichVuKhamBenhBenhVienService
    {
        IRepository<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh> _dichVuKhamBenhrepository;
        IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongrepository;
        IRepository<Core.Domain.Entities.KhoaPhongChuyenKhoas.KhoaPhongChuyenKhoa> _khoaPhongChuyenKhoarepository;
        IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBaoHiem> _dichVuKhamBenhBenhVienGiaBaoHiemrepository;
        IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienrepository;
        IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.NhomGiaDichVuKhamBenhBenhVien> _nhomdichVuKhamBenhrepository;
        IRepository<DichVuKhamBenhBenhVienNoiThucHien> _dichVuKhamBenhBenhVienNoiThucHienRepository;
        IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongbenhvienRepository;
         IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
         private ILocalizationService _localizationService;
        public DichVuKhamBenhBenhVienService(IRepository<DichVuKhamBenhBenhVien> repository, 
            IRepository<Core.Domain.Entities.KhoaPhongChuyenKhoas.KhoaPhongChuyenKhoa> khoaPhongChuyenKhoarepository, 
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongrepository, 
            IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.NhomGiaDichVuKhamBenhBenhVien> nhomdichVuKhamBenhrepository, 
            IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienrepository, 
            IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBaoHiem> dichVuKhamBenhBenhVienGiaBaoHiemrepository, 
            IRepository<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh> dichVuKhamBenhrepository,
            IRepository<DichVuKhamBenhBenhVienNoiThucHien> dichVuKhamBenhBenhVienNoiThucHienRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongbenhvienRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            ILocalizationService localizationService) : base(repository)
        {
            _khoaPhongChuyenKhoarepository = khoaPhongChuyenKhoarepository;
            _khoaPhongrepository = khoaPhongrepository;
            _dichVuKhamBenhrepository = dichVuKhamBenhrepository;
            _dichVuKhamBenhBenhVienGiaBaoHiemrepository = dichVuKhamBenhBenhVienGiaBaoHiemrepository;
            _dichVuKhamBenhBenhVienGiaBenhVienrepository = dichVuKhamBenhBenhVienGiaBenhVienrepository;
            _nhomdichVuKhamBenhrepository = nhomdichVuKhamBenhrepository;
            _dichVuKhamBenhBenhVienNoiThucHienRepository = dichVuKhamBenhBenhVienNoiThucHienRepository;
            _phongbenhvienRepository = phongbenhvienRepository;
            _cauHinhRepository = cauHinhRepository;
            _localizationService = localizationService;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Include(x => x.DichVuKhamBenh)
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.KhoaPhong)
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
                .Select(s => new DichVuKhamBenhBenhVienVO
                {
                    
                    Id = s.Id,
                    Ma = s.Ma,
                    HangBenhVien=s.DichVuKhamBenh.HangBenhVien.GetDescription(),
                    MaTT37=s.DichVuKhamBenh == null ? string.Empty : s.DichVuKhamBenh.MaTT37,
                    MoTa=s.MoTa,
                    Ten=s.Ten,
                    TenNoiThucHien = "",
                    DichVuKhamBenhId = s.DichVuKhamBenhId.GetValueOrDefault(),
                    HieuLucHienThi = s.HieuLuc == true ? "Có" : "Không"
                });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.MaTT37, g => g.MoTa, g => g.Ten);

            var queryList = query.ToList();

            var listDVKB = await BaseRepository.TableNoTracking
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong)
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
                .Where(x => queryList.Select(a => a.Id.ToString()).Contains(x.Id.ToString())).ToListAsync();
            if (listDVKB.Any())
            {
                foreach (var item in queryList)
                {
                    //var dichVuKhamBenh = await BaseRepository.TableNoTracking
                    //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong)
                    //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien).FirstOrDefaultAsync(x => x.Id == item.Id);
                    var dichVuKhamBenh = listDVKB.FirstOrDefault(x => x.Id == item.Id);
                    if (dichVuKhamBenh != null)
                    {
                        item.TenNoiThucHien = string.Join("; ", dichVuKhamBenh.DichVuKhamBenhBenhVienNoiThucHiens.Where(a => a.KhoaPhongId != null).Select(a => a.KhoaPhong.Ma + " - " + a.KhoaPhong.Ten))
                                              + (dichVuKhamBenh.DichVuKhamBenhBenhVienNoiThucHiens.Any(x => x.KhoaPhongId != null) ? "; " : "")
                                              + string.Join("; ", dichVuKhamBenh.DichVuKhamBenhBenhVienNoiThucHiens.Where(a => a.PhongBenhVienId != null).Select(a => a.PhongBenhVien.Ma + " - " + a.PhongBenhVien.Ten));
                    }
                }
            }
            query = queryList.AsQueryable();
            
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            await Task.WhenAll(countTask);

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                 .Include(x => x.DichVuKhamBenh)
                 .Select(s => new DichVuKhamBenhBenhVienVO
                 {
                     Id = s.Id,
                     Ma = s.Ma,
                     HangBenhVien = s.DichVuKhamBenh.HangBenhVien.GetDescription(),
                     MaTT37 = s.DichVuKhamBenh == null ? string.Empty : s.DichVuKhamBenh.MaTT37,
                     MoTa = s.MoTa,
                     Ten = s.Ten,
                     TenNoiThucHien = "",
                     DichVuKhamBenhId = s.DichVuKhamBenhId.GetValueOrDefault(),
                     HieuLucHienThi = s.HieuLuc == true ? "Có" : "Không"
                 });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.MaTT37, g => g.MoTa, g => g.Ten);

            //var queryList = query.ToList();
            //foreach (var item in queryList)
            //{
            //    var dichVuKhamBenh = await BaseRepository.TableNoTracking
            //        .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong)
            //        .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien).FirstOrDefaultAsync(x => x.Id == item.Id);
            //    if (dichVuKhamBenh != null)
            //    {
            //        item.TenNoiThucHien = string.Join("; ", dichVuKhamBenh.DichVuKhamBenhBenhVienNoiThucHiens.Where(a => a.KhoaPhongId != null).Select(a => a.KhoaPhong.Ma + " - " + a.KhoaPhong.Ten))
            //                              + (dichVuKhamBenh.DichVuKhamBenhBenhVienNoiThucHiens.Any(x => x.KhoaPhongId != null) ? "; " : "")
            //                              + string.Join("; ", dichVuKhamBenh.DichVuKhamBenhBenhVienNoiThucHiens.Where(a => a.PhongBenhVienId != null).Select(a => a.PhongBenhVien.Ma + " - " + a.PhongBenhVien.Ten));
            //    }
            //}
            //query = queryList.AsQueryable();

            
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            string sort = queryInfo.SortString.Replace("HienThi", "");
            var query = _dichVuKhamBenhBenhVienGiaBaoHiemrepository.TableNoTracking.Include(x=>x.DichVuKhamBenhBenhVien)
                .Where(x => x.DichVuKhamBenhBenhVienId == long.Parse(queryInfo.SearchTerms))
                .Select(s => new DichVuKhamBenhBenhVienGiaBaoHiemVO()
                {
                    Id = s.Id,
                    Gia = s.Gia,
                    GiaHienThi =Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                    TiLeBaoHiemThanhToan=s.TiLeBaoHiemThanhToan,
                    TuNgayHienThi = s.TuNgay.ApplyFormatDate(),
                    TuNgay= s.TuNgay,
                    DenNgayHienThi =s.DenNgay!=null? Convert.ToDateTime(s.DenNgay).ApplyFormatDate():null
                });
           // query = query.ApplyLike(queryInfo.SearchTerms, g => g.Gia.ToString(), g => g.TiLeBaoHiemThanhToan.ToString());
            //query = query.OrderByDescending(x => x.TuNgay);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sort).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _dichVuKhamBenhBenhVienGiaBaoHiemrepository.TableNoTracking.Include(x => x.DichVuKhamBenhBenhVien)
                  .Where(x => x.DichVuKhamBenhBenhVienId == long.Parse(queryInfo.SearchTerms))
                  .Select(s => new DichVuKhamBenhBenhVienGiaBaoHiemVO()
                  {
                      Id = s.Id,
                      Gia = s.Gia,
                      GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                      TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan,
                      TuNgay = s.TuNgay,
                      TuNgayHienThi = s.TuNgay.ApplyFormatDate(),
                      DenNgayHienThi = s.DenNgay != null ? Convert.ToDateTime(s.DenNgay).ApplyFormatDate() : null
                  });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Gia.ToString(), g => g.TiLeBaoHiemThanhToan.ToString());
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataForGridChildGiaBenhVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            string sort = queryInfo.SortString.Replace("HienThi", "");

            var query = _dichVuKhamBenhBenhVienGiaBenhVienrepository.TableNoTracking.Include(x => x.DichVuKhamBenhBenhVien).Include(x => x.NhomGiaDichVuKhamBenhBenhVien)
                 .Where(x => x.DichVuKhamBenhBenhVienId == long.Parse(queryInfo.SearchTerms))
                 .Select(s => new DichVuKhamBenhBenhVienGiaBenhVienVO()
                 {
                     Id = s.Id,
                     Gia = s.Gia,
                     LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                     GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                     TuNgay = s.TuNgay,
                     TuNgayHienThi = s.TuNgay.ApplyFormatDate(),
                     DenNgayHienThi = s.DenNgay != null ? Convert.ToDateTime(s.DenNgay).ApplyFormatDate() : null
                 });
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.LoaiGia);
            //query = query.OrderByDescending(x => x.TuNgay);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(sort).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildGiaBenhVienAsync(QueryInfo queryInfo)
        {
            var query = _dichVuKhamBenhBenhVienGiaBenhVienrepository.TableNoTracking.Include(x => x.DichVuKhamBenhBenhVien).Include(x=>x.NhomGiaDichVuKhamBenhBenhVien)
                  .Where(x => x.DichVuKhamBenhBenhVienId == long.Parse(queryInfo.SearchTerms))
                  .Select(s => new DichVuKhamBenhBenhVienGiaBenhVienVO()
                  {
                      Id = s.Id,
                      Gia = s.Gia,
                      LoaiGia=s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                      GiaHienThi = Convert.ToDouble(s.Gia).ApplyFormatMoneyVNDToDouble(),
                      TuNgay = s.TuNgay,
                      TuNgayHienThi = s.TuNgay.ApplyFormatDate(),
                      DenNgayHienThi = s.DenNgay != null ? Convert.ToDateTime(s.DenNgay).ApplyFormatDate() : null
                  });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Gia.ToString());
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public Task<long> GetIdKhoaPhongFromRequestDropDownList(DropDownListRequestModel model)
        {
            var parameter = model != null ? model.ParameterDependencies : "";
            if (string.IsNullOrEmpty(parameter)) return Task.FromResult((long)0);
            var getValue = JsonConvert.DeserializeObject<Dictionary<string, long>>(parameter);
            var toaNhaId = getValue.Values.First();
            return Task.FromResult(toaNhaId);
        }
        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuKhamBenh(DropDownListRequestModel model)
        {
            //update 18-04-2022 bỏ query theo hạng bệnh viện
            //var cauHinhHangBenhVien = await _cauHinhRepository.TableNoTracking
            //    .Where(x => x.Name == "BaoHiemYTe.HangBenhVien").FirstOrDefaultAsync();
            //var hangBenhVien = cauHinhHangBenhVien == null ? 0 : int.Parse(cauHinhHangBenhVien.Value);
            return await _dichVuKhamBenhrepository.TableNoTracking
                //.Where(o => BaseRepository.TableNoTracking.All(p => p.DichVuKhamBenhId != o.Id) && (int)o.HangBenhVien == hangBenhVien)
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.TenChung)
                .Select(item => new DichVuKyThuatTemplateVo
                {
                    DisplayName = item.TenChung,//item.MaChung + " - " + item.TenChung,
                    KeyId = item.Id,
                    DichVu = item.TenChung,
                    Ma = item.MaChung
                }).ApplyLike(model.Query, o => o.DisplayName, o => o.Ma)
                .Take(model.Take)
                .ToListAsync();
        }

        public async Task<List<DichVuKyThuatTemplateVo>> GetKhoaPhong(DropDownListRequestModel model)
        {
            var lst = await _khoaPhongrepository.TableNoTracking
                .Where(p => p.IsDisabled != true)
                .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
                .Take(model.Take)
                .ToListAsync();

            var lstKhoaPhongDaChon = await _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking
                .Where(x => x.DichVuKhamBenhBenhVienId == model.Id && x.KhoaPhongId != null
                                                                   && !lst.Select(a => a.Id.ToString()).Contains(x.KhoaPhongId.ToString())).Select(x => x.KhoaPhongId.ToString()).ToListAsync();
            if (lstKhoaPhongDaChon.Any())
            {
                var lstDaChon = await _khoaPhongrepository.TableNoTracking
                    .Where(p => lstKhoaPhongDaChon.Contains(p.Id.ToString()))
                    .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
                    .ToListAsync();

                lst.AddRange(lstDaChon);
            }

            var query = lst.Select(item => new DichVuKyThuatTemplateVo
            {
                DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                KeyId = item.Id,
                DichVu = item.Ten,
                Ma = item.Ma,
            }).ToList();

            return query;
        }

        public async Task<List<NoiThucHienDichVuBenhVienVo>> GetPhongBenhVienDichVuKhamBenh(DropDownListRequestModel model)
        {
            //var lst = await _phongbenhvienRepository.TableNoTracking
            //    .Where(p => p.IsDisabled != true)
            //    .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstPhongBenhVienDaChon = await _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking
            //    .Where(x => x.DichVuKhamBenhBenhVienId == model.Id && x.PhongBenhVienId != null 
            //                                                       && !lst.Select(a => a.Id.ToString()).Contains(x.PhongBenhVienId.ToString())).Select(x => x.PhongBenhVienId.ToString()).ToListAsync();
            //if (lstPhongBenhVienDaChon.Any())
            //{
            //    var lstDaChon = await _phongbenhvienRepository.TableNoTracking
            //        .Where(p => lstPhongBenhVienDaChon.Contains(p.Id.ToString()))
            //        .ApplyLike(model.Query, p => p.Ma, p => p.Ten)
            //        .ToListAsync();

            //    lst.AddRange(lstDaChon);
            //}

            //var query = lst.Select(item => new DichVuKyThuatTemplateVo
            //{
            //    DisplayName = item.Ma + " - " + item.Ten,
            //    KeyId = item.Id,
            //    DichVu = item.Ten,
            //    Ma = item.Ma,
            //}).ToList();

            //return query;


            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return new List<NoiThucHienDichVuBenhVienVo>();
            }

            var lstKhoaIdDaChon = JsonConvert.DeserializeObject<List<KhoaDaChonVo>>(model.ParameterDependencies);
            var lstKhoa = await _khoaPhongrepository.TableNoTracking
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

                    lstPhongTheoKhoa.AddRange(lstPhong.Where(x => x.IsDisabled != true).Select(items =>
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

        public async Task<List<DichVuKyThuatTemplateVo>> GetDichVuKhamBenhById(DropDownListRequestModel model,long id, long? khoaPhongId)
        {
           
            var lst = await _dichVuKhamBenhrepository.TableNoTracking.Include(x => x.Khoa)
                .Where(p => p.TenChung.Contains(model.Query ?? "") || p.MaChung.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();
            var entity = await _dichVuKhamBenhrepository.TableNoTracking.Include(x => x.Khoa)
                .Where(x=>x.Id==id)
                .FirstOrDefaultAsync();
            var lstDichVuKhamBenhBenhVien = await BaseRepository.TableNoTracking.Distinct()
                .ToListAsync();
           
            for (int i = 0; i < lstDichVuKhamBenhBenhVien.Count; i++)
            {
                var check = lst.Where(x => x.Id == lstDichVuKhamBenhBenhVien[i].DichVuKhamBenhId).FirstOrDefault();
                if(check !=null)
                {
                    lst.RemoveAll(x => x.Id == lstDichVuKhamBenhBenhVien[i].DichVuKhamBenhId);
                }
            }
            lst.Add(entity);
            var query = lst.Select(item => new DichVuKyThuatTemplateVo
            {
                DisplayName = item.MaChung + " - " + item.TenChung + " - " + item.Khoa.Ten,
                KeyId = item.Id,
                DichVu = item.TenChung,
                Ma = item.MaChung,
                TenKhoa = item.Khoa.Ten
            }).ToList();

            return query;
        }
        public async Task<List<LookupItemVo>> GetNhomDichVu()
        {
            var lst = await _nhomdichVuKhamBenhrepository.TableNoTracking
                .ToListAsync();

            var query = lst.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id
            }).ToList();

            return query;
        }
        public async Task UpdateLastEntity(long id, DateTime tuNgay,long dichVuKhamBenhId)
        {
            var lastEntity = await BaseRepository.Table.Where(p => p.Id < id && p.DichVuKhamBenhId.Equals(dichVuKhamBenhId)).OrderByDescending(p => p.Id)
                .ToListAsync();
            //if (lastEntity.Count >0) {
            //    lastEntity.FirstOrDefault().DenNgay = tuNgay.AddDays(-1);
            //    await BaseRepository.UpdateAsync(lastEntity);
            //}
            
        }
        public DichVuKhamBenhBenhVien CheckExistDichVuKhamBenhBenhVien(long id)
        {
            var result= BaseRepository.TableNoTracking.Where(x => x.DichVuKhamBenhId == id).FirstOrDefault();
            if(result !=null)
            {
                return result;
            }
            return null;
        }
        public async Task AddAndUpdateLastEntity(DateTime tuNgayBaoHiem, ICollection<DichVuKhamBenhBenhVienGiaBenhVien> giaBenhVienEntity)
        {
            var nhomDichvu = _nhomdichVuKhamBenhrepository.Table.ToList();
            var baohiemEntity = await _dichVuKhamBenhBenhVienGiaBaoHiemrepository.Table.OrderByDescending(p => p.Id)
                .ToListAsync();
           
            if (baohiemEntity.Count > 0) {
                var baohiem = baohiemEntity.FirstOrDefault();
                baohiem.DenNgay = tuNgayBaoHiem.Date.AddDays(-1);
                await _dichVuKhamBenhBenhVienGiaBaoHiemrepository.UpdateAsync(baohiem);
            }
            foreach(var item in nhomDichvu) {
                var giaTheoNhom = giaBenhVienEntity.Where(x => x.NhomGiaDichVuKhamBenhBenhVienId.Equals(item.Id)).ToList();
                if (giaTheoNhom.Count > 0) {
                    for (int i = 1; i < giaTheoNhom.Count; i++)
                    {
                        giaTheoNhom[i - 1].DenNgay = Convert.ToDateTime(giaTheoNhom[i].TuNgay).Date.AddDays(-1);
                    }
                }
            }
           
        }
        public async Task UpdateDayGiaBenhVienEntity(ICollection<DichVuKhamBenhBenhVienGiaBenhVien> giaBenhVienEntity)
        {
            var nhomDichvu = _nhomdichVuKhamBenhrepository.Table.ToList();
          
            foreach (var item in nhomDichvu)
            {
                var giaTheoNhom = giaBenhVienEntity.Where(x => x.NhomGiaDichVuKhamBenhBenhVienId.Equals(item.Id)).ToList();
                if (giaTheoNhom.Count > 0)
                {
                    for (int i = 1; i < giaTheoNhom.Count; i++)
                    {
                        if (giaTheoNhom[i - 1].DenNgay == null)
                        {
                            giaTheoNhom[i - 1].DenNgay = Convert.ToDateTime(giaTheoNhom[i].TuNgay).Date.AddDays(-1);
                        }
                    }
                }
            }

        }
        public async Task UpdateDeleteEntity(long id , int loai , long nhom=0)
        {
           if(loai == 1) {
                var result =await _dichVuKhamBenhBenhVienGiaBaoHiemrepository.Table.Where(x=>x.Id <id).OrderByDescending(p => p.Id).FirstOrDefaultAsync();
                if(result!=null) {
                    result.DenNgay = null;
                    await _dichVuKhamBenhBenhVienGiaBaoHiemrepository.UpdateAsync(result);
                }
            }
            else
            {
                var result = await _dichVuKhamBenhBenhVienGiaBenhVienrepository.Table.Where(x => x.Id < id &&  x.NhomGiaDichVuKhamBenhBenhVienId==nhom).OrderByDescending(p => p.Id).FirstOrDefaultAsync();
                if (result != null)
                {
                    result.DenNgay = null;
                    await _dichVuKhamBenhBenhVienGiaBenhVienrepository.UpdateAsync(result);
                }
            }

        }

        public async Task<DichVuKhamBenhBenhVien> GetAfterEntity(long id,long dichVuKhamBenhId)
        {
            var lstEntity = await BaseRepository.Table.Where(p => p.Id < id && p.DichVuKhamBenhId.Equals(dichVuKhamBenhId)).OrderByDescending(p => p.Id).ToListAsync();
            if (lstEntity.Count > 0) {
                return lstEntity.FirstOrDefault();
            }

            return null;
        }
        public async Task<List<NhomGiaDichVuKhamBenhBenhVien>> GetNhomGiaDichVuKhamBenhBenhVien()
        {
            var lstEntity = await _nhomdichVuKhamBenhrepository.Table.ToListAsync();
         

            return lstEntity;
        }
        public async Task<bool> IsTuNgayValid(DateTime? tuNgay, long? id, long? dichVuKhamBenhId)
        {
            if (tuNgay == null || dichVuKhamBenhId == null || dichVuKhamBenhId == 0) return true;
            //update
            if (id != null && id != 0)
            {
                var lstEntity = await BaseRepository.TableNoTracking.Where(p => p.DichVuKhamBenhId == dichVuKhamBenhId).OrderByDescending(p => p.Id).ToListAsync();
                var isGetItem = false;
                foreach (var item in lstEntity)
                {
                    //if (isGetItem)
                    //{
                    //    var denNgayTemp = tuNgay.GetValueOrDefault().Date.AddDays(-1);
                    //    if (item?.TuNgay.GetValueOrDefault().Date == denNgayTemp.Date ||
                    //        tuNgay.GetValueOrDefault().Date <= item?.TuNgay.GetValueOrDefault().Date
                    //        || (item?.DenNgay != null &&
                    //            tuNgay.GetValueOrDefault().Date <= item?.DenNgay.GetValueOrDefault().Date))
                    //    {
                    //        return false;
                    //    }
                    //    else
                    //    {
                    //        return true;
                    //    }
                    //}
                    if (item?.Id == id)
                    {
                        isGetItem = true;
                    }
                }
            }
            //create
            else
            {
                var item = await BaseRepository.TableNoTracking.Where(p => p.DichVuKhamBenhId == dichVuKhamBenhId).OrderByDescending(p => p.Id).FirstOrDefaultAsync();
                var denNgayTemp = tuNgay.GetValueOrDefault().Date.AddDays(-1);
                //if (item?.TuNgay.GetValueOrDefault().Date == denNgayTemp.Date ||
                //    tuNgay.GetValueOrDefault().Date <= item?.TuNgay.GetValueOrDefault().Date
                //    || (item?.DenNgay != null &&
                //        tuNgay.GetValueOrDefault().Date <= item?.DenNgay.GetValueOrDefault().Date))
                //{
                //    return false;
                //}
                //else
                //{
                //    return true;
                //}
            }

            return true;
        }
        public async Task<bool> IsTuNgayBenhVienValid(DateTime? tuNgay, long? id, long? nhom)
        {
            if (tuNgay == null || nhom == null || nhom == 0) return true;
            //update
            if (id != null && id != 0)
            {
                var lstEntity = await _dichVuKhamBenhBenhVienGiaBenhVienrepository.TableNoTracking.Where(p => p.NhomGiaDichVuKhamBenhBenhVienId == nhom && p.Id <id).OrderByDescending(p => p.Id).ToListAsync();
                if (lstEntity.Count>0)
                {
                    if (lstEntity.First().TuNgay.Date > Convert.ToDateTime(tuNgay).Date)
                    {
                        return false;
                    }
                    if (lstEntity.First().DenNgay != null)
                    {
                        if (Convert.ToDateTime(lstEntity.First().DenNgay).Date > tuNgay)
                        {
                            return false;
                        }
                    }
                }
            }
            //create
            else
            {
                var item = await _dichVuKhamBenhBenhVienGiaBenhVienrepository.TableNoTracking.OrderByDescending(p => p.Id).FirstOrDefaultAsync();
                var denNgayTemp = tuNgay.GetValueOrDefault().Date.AddDays(-1);
                if (item.TuNgay.Date == denNgayTemp) { return false; }
                if (item.TuNgay.Date > Convert.ToDateTime(tuNgay).Date) { return false; }
            }

            return true;
        }
        public async Task<bool> IsTuNgayBaoHiemValid(DateTime? tuNgay, long? id)
        {
            if (tuNgay == null) return true;
            //update
            if (id != null && id != 0)
            {
                //lstEntity.First().DenNgay != null ? Convert.ToDateTime(lstEntity.First().DenNgay).Date > tuNgay : ""
                var lstEntity = await _dichVuKhamBenhBenhVienGiaBaoHiemrepository.TableNoTracking.Where(p => p.Id < id).OrderByDescending(p => p.Id).ToListAsync();
                if (lstEntity.Count > 0)
                {
                    if(lstEntity.First().TuNgay.Date > Convert.ToDateTime(tuNgay).Date)
                    {
                        return false;
                    }
                    if(lstEntity.First().DenNgay != null) {
                        if (Convert.ToDateTime(lstEntity.First().DenNgay).Date > tuNgay) {
                            return false;
                        }
                    }
                }
            }
            //create
            else
            {
                var item = await _dichVuKhamBenhBenhVienGiaBaoHiemrepository.TableNoTracking.OrderByDescending(p => p.Id).FirstOrDefaultAsync();
                var denNgayTemp = tuNgay.GetValueOrDefault().Date.AddDays(-1);
                if (item.TuNgay.Date == denNgayTemp) { return false; }
                if(item.TuNgay.Date > Convert.ToDateTime(tuNgay).Date) { return false; }
            }

            return true;
        }

        public async Task<bool> IsExistsMaDichVuKhamBenhBenhVien(long dichVuKhamBenhBenhVienId, string ma)
        {
            if (dichVuKhamBenhBenhVienId != 0 || string.IsNullOrEmpty(ma))
                return false;
            var dichVuKhamBenhBenhVien =
                await BaseRepository.TableNoTracking.AnyAsync(x => x.Ma.Trim().ToLower() == ma.Trim().ToLower());
            return dichVuKhamBenhBenhVien;
        }
        public async Task<bool> KiemTraNgay(DateTime? tuNgay, DateTime? denNgay)
        {
            if (tuNgay != null && denNgay != null)
            {
                if (denNgay < tuNgay)
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<bool> kiemTraCoBaoNhieuLoaiGiaBenhVien(DateTime? tuNgay, DateTime? denNgay, bool? validate)
        {
            if(validate == true)
            {
                if (denNgay == null)
                {
                    return false;
                }
            }
            return true;
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
                    var lstDichVuKhamBenhVien = BaseRepository.TableNoTracking
                        .Select(x => new ThongTinDichVuBenhVienVo()
                        {
                            DichVuBenhVienId = x.Id,
                            MaDichVu = x.Ma,
                            TenDichVu = x.Ten
                        }).ToList();
                    lstDichVuKhamBenhVien = lstDichVuKhamBenhVien
                        .Where(x => lstLookupDichVu.Any(a => a.MaDichVu.Equals(x.MaDichVu.Trim().ToLower())
                                                              && a.TenDichVu.Equals(x.TenDichVu.Trim().ToLower())))
                        .ToList();
                    if (lstDichVuKhamBenhVien.Any())
                    {
                        var lstDichVuId = lstDichVuKhamBenhVien.Select(x => x.DichVuBenhVienId).Distinct().ToList();
                        var lstGiaBenhVien = _dichVuKhamBenhBenhVienGiaBenhVienrepository.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuKhamBenhBenhVienId))
                            .Select(a => new ThongTinGiaBenhVienVo()
                            {
                                DichVuBenhVienId = a.DichVuKhamBenhBenhVienId,
                                NhomGiaId = a.NhomGiaDichVuKhamBenhBenhVienId,
                                TenLoaiGia = a.NhomGiaDichVuKhamBenhBenhVien.Ten,
                                Gia = a.Gia,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();
                        var lstGiaBaoHiem = _dichVuKhamBenhBenhVienGiaBaoHiemrepository.TableNoTracking
                            .Where(x => lstDichVuId.Contains(x.DichVuKhamBenhBenhVienId))
                            .Select(a => new ThongTinGiaBaoHiemVo()
                            {
                                DichVuBenhVienId = a.DichVuKhamBenhBenhVienId,
                                Gia = a.Gia,
                                TiLeBaoHiemThanhToan = a.TiLeBaoHiemThanhToan,
                                TuNgay = a.TuNgay,
                                DenNgay = a.DenNgay
                            })
                            .ToList();

                        foreach (var dichVuBenhVien in lstDichVuKhamBenhVien)
                        {
                            var lstGiaTheoBenhVien = lstGiaBenhVien.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            var lstGiaTheoBaoHiem = lstGiaBaoHiem.Where(x => x.DichVuBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();


                            dichVuBenhVien.ThongTinGiaBenhViens = lstGiaTheoBenhVien;
                            dichVuBenhVien.ThongTinGiaBaoHiems = lstGiaTheoBaoHiem;
                        }
                        lstDichVuBenhVien = lstDichVuBenhVien.Concat(lstDichVuKhamBenhVien).ToList();
                    }
                }

                var lstNhomGiaBenhVien = _nhomdichVuKhamBenhrepository.TableNoTracking
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
                                var newNhomGia = new NhomGiaDichVuKhamBenhBenhVien()
                                {
                                    Ten = dichVu.LoaiGia.Trim()
                                };
                                _nhomdichVuKhamBenhrepository.Add(newNhomGia);
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
                                                    //&& dichVu.LoaiGiaId == giaBenhVienTheoDichVuHienTai.NhomGiaId 
                                                    //&& dichVu.GiaBenhVienValue == giaBenhVienTheoDichVuHienTai.Gia)
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
                                                   //&& dichVu.TiLeBaoHiemThanhToanValue == giaBaoHiemTheoDichVuHienTai.TiLeBaoHiemThanhToan
                                                   //&& dichVu.GiaBaoHiemValue == giaBaoHiemTheoDichVuHienTai.Gia)
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
                    .Include(x => x.DichVuKhamBenhBenhVienGiaBaoHiems)
                    .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens)
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
                    var giaBaoHiemCuoiCungHienTai = dichVuBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.OrderByDescending(x => x.TuNgay).FirstOrDefault();
                    if (lstGiaBaoHiemImport.Any())
                    {
                        var tuNgayHopLe = giaBaoHiemCuoiCungHienTai == null 
                                          || lstGiaBaoHiemImport.Any(x => x.TuNgayValue == giaBaoHiemCuoiCungHienTai?.TuNgay)
                                                                        //&& x.TiLeBaoHiemThanhToanValue == giaBaoHiemCuoiCungHienTai?.TiLeBaoHiemThanhToan
                                                                        //&& x.GiaBaoHiemValue == giaBaoHiemCuoiCungHienTai?.Gia)
                                          || lstGiaBaoHiemImport.Any(x => x.TuNgayValue != giaBaoHiemCuoiCungHienTai?.TuNgay
                                                                        && x.TuNgayValue > giaBaoHiemCuoiCungHienTai?.TuNgay.AddDays(1));

                        //if (giaBaoHiemCuoiCungHienTai != null && lstGiaBaoHiemImport.Any(x => x.TuNgayValue <= giaBaoHiemCuoiCungHienTai.TuNgay.AddDays(1)))
                        if(!tuNgayHopLe)
                        {
                            throw new Exception(_localizationService.GetResource("ImportGiaDichVu.TuNgay.Invalid"));
                        }
                        
                        //xử lý gán giá trị từ ngày đến ngày
                        foreach (var item in lstGiaBaoHiemImport.Select((value, index) => new {index, value }))
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
                            dichVuBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.Add(new DichVuKhamBenhBenhVienGiaBaoHiem()
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
                                var giaBenhVienCuoiCungHienTaiTheoLoaiGia = dichVuBenhVien.DichVuKhamBenhBenhVienGiaBenhViens
                                    .Where(x => x.NhomGiaDichVuKhamBenhBenhVienId == item.value.LoaiGiaId)
                                    .OrderByDescending(x => x.TuNgay).FirstOrDefault();

                                var tuNgayHopLe = giaBenhVienCuoiCungHienTaiTheoLoaiGia == null
                                                  || (item.value.TuNgayValue == giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay)
                                                            //&& item.value.GiaBenhVienValue == giaBenhVienCuoiCungHienTaiTheoLoaiGia?.Gia)
                                                  || (item.value.TuNgayValue != giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay
                                                        && item.value.TuNgayValue > giaBenhVienCuoiCungHienTaiTheoLoaiGia?.TuNgay.AddDays(1));

                                //if (giaBenhVienCuoiCungHienTaiTheoLoaiGia != null && item.value.TuNgayValue <= giaBenhVienCuoiCungHienTaiTheoLoaiGia.TuNgay.AddDays(1))
                                if(!tuNgayHopLe)
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
                            dichVuBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.Add(new DichVuKhamBenhBenhVienGiaBenhVien()
                            {
                                NhomGiaDichVuKhamBenhBenhVienId = gia.LoaiGiaId.Value,
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

        public async Task<List<LookupItemVo>> GetNhomDichVuTheoDichVuKhamBenh(DropDownListRequestModel queryInfo)
        {
            var dsKhamBenhBenhViens = new List<LookupItemVo>();
            var dichVuKhamBenhBenhVien = JsonConvert.DeserializeObject<DichVuKhamBenhBenhVienJSON>(queryInfo.ParameterDependencies.Replace("undefined", "null"));
            if (dichVuKhamBenhBenhVien != null)
            {
                var lst = await _nhomdichVuKhamBenhrepository.TableNoTracking.Where(c =>
                         c.DichVuKhamBenhBenhVienGiaBenhViens.Any(x => x.DichVuKhamBenhBenhVienId == dichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienId &&
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
