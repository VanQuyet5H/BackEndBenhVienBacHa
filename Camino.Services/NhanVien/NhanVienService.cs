using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject.NhanVien;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using System.Text.RegularExpressions;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.KhoaPhongChuyenKhoas;
using Camino.Services.Helpers;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.ValueObject.ChucVu;

namespace Camino.Services.NhanVien
{
    [ScopedDependency(ServiceType = typeof(INhanVienService))]
    public class NhanVienService : MasterFileService<Core.Domain.Entities.NhanViens.NhanVien>, INhanVienService
    {
        public IRepository<Core.Domain.Entities.Users.User> _userRepository;
        public IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        public IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienRepository;
        public IRepository<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon> _vanBangChuyenMonRepository;
        public IRepository<Core.Domain.Entities.ChucDanhs.ChucDanh> _chucDanhRepository;
        public IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> _khoRepository;
        private readonly IRepository<KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        public IRepository<Core.Domain.Entities.ChucVus.ChucVu> _chucVuRepository;

        public NhanVienService(IRepository<Core.Domain.Entities.NhanViens.NhanVien> repository
            , IRepository<Core.Domain.Entities.Users.User> userRepository
            , IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienRepository
            , IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository
            , IRepository<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon> vanBangChuyenMonRepository
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            , IRepository<Core.Domain.Entities.ChucDanhs.ChucDanh> chucDanhRepository
            , IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> khoRepository
            , IRepository<KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.ChucVus.ChucVu> chucVuRepository) : base(repository)
        {
            _userRepository = userRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _vanBangChuyenMonRepository = vanBangChuyenMonRepository;
            _chucDanhRepository = chucDanhRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _khoaPhongNhanVienRepository = khoaPhongNhanVienRepository;
            _userAgentHelper = userAgentHelper;
            _khoRepository = khoRepository;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _chucVuRepository = chucVuRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            //RenameSortForFormatColumn(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = _userRepository.TableNoTracking.Include(p => p.NhanVien).Where(p => p.IsDefault != true).Select(s => new NhanVienGridVo
            {
                Id = s.NhanVien.Id,
                HoTen = s.HoTen,
                SoChungMinhThu = s.SoChungMinhThu,
                DiaChi = s.DiaChi,
                NgaySinh = s.NgaySinh,
                SoDienThoai = s.SoDienThoai,
                SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                KichHoat = s.IsActive,
                Email = s.Email

            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.HoTen, g => g.DiaChi, g => g.Email, g => g.SoChungMinhThu, g => g.SoDienThoaiDisplay, g => g.SoDienThoai);


            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _userRepository.TableNoTracking.Include(p => p.NhanVien).Where(p => p.IsDefault != true).Select(s => new NhanVienGridVo
            {
                Id = s.NhanVien.Id,
                HoTen = s.HoTen,
                SoChungMinhThu = s.SoChungMinhThu,
                DiaChi = s.DiaChi,
                NgaySinh = s.NgaySinh,
                SoDienThoai = s.SoDienThoai,
                SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                KichHoat = s.IsActive,
                Email = s.Email

            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.HoTen, g => g.DiaChi, g => g.Email, g => g.SoChungMinhThu, g => g.SoDienThoaiDisplay, g => g.SoDienThoai);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<bool> CheckIsExistPhone(string sdt, long id = 0)
        {
            bool result;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.Include(o => o.User).AnyAsync(p => p.User.SoDienThoai.Equals(sdt));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.Include(o => o.User).AnyAsync(p => p.User.SoDienThoai.Equals(sdt) && p.Id != id);
            }
            if (result)
                return false;
            return true;
        }
        public async Task<bool> CheckIsExistChungMinh(string cmt, long id = 0)
        {
            bool result;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.Include(o => o.User).AnyAsync(p => p.User.SoChungMinhThu.Equals(cmt));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.Include(o => o.User).AnyAsync(p => p.User.SoChungMinhThu.Equals(cmt) && p.Id != id);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<long[]> GetNhanVienRoles(long nhanVienId)
        {
            return await BaseRepository.TableNoTracking.Include(o => o.NhanVienRoles).Where(o => o.Id == nhanVienId)
                .SelectMany(o => o.NhanVienRoles.Select(nvr => nvr.RoleId)).ToArrayAsync();
        }

        public async Task<bool> CheckIsExistEmail(string email, long id = 0)
        {
            bool result;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.Include(o => o.User).AnyAsync(p => p.User.Email.Equals(email) && p.User.Email != "" && p.User.Email != null);

            }
            else
            {
                result = await BaseRepository.TableNoTracking.Include(o => o.User).AnyAsync(p => p.User.Email.Equals(email) && p.Id != id && p.User.Email != "" && p.User.Email != null);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<List<LookupItemVo>> GetListNhanVien(DropDownListRequestModel model)
        {
            var listNhanVien = BaseRepository.TableNoTracking
                .Where(p => p.User.HoTen.Contains(model.Query ?? ""))
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.User.HoTen,
                    KeyId = i.User.Id
                });
            return await listNhanVien.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetListTenNhanVien(DropDownListRequestModel model)
        {
            var list = BaseRepository.TableNoTracking.Include(p => p.User).ApplyLike(model.Query, g => g.User.HoTen)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.User.HoTen,
                    KeyId = i.User.Id
                });
            return await list.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetListTenChucDanhNhanVien(DropDownListRequestModel model)
        {
            var list = _chucDanhRepository.TableNoTracking.ApplyLike(model.Query, g => g.Ten)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                });
            return await list.ToListAsync();
        }


        public async Task<List<LookupItemVo>> GetListLookupNhanVienIsYta(LookupQueryInfo model)
        {
            var listNhanVien = BaseRepository.TableNoTracking.Include(p => p.User)
                .Include(p => p.ChucDanh)
                .ThenInclude(p => p.NhomChucDanhId)
                .Include(p => p.KhoaPhongNhanViens)
                .Where(p => p.KhoaPhongNhanViens.Any(x => x.KhoaPhongId == model.Id)
                                                && ((int)p.ChucDanh.NhomChucDanhId != (int)Enums.EnumNhomChucDanh.BacSi
                                                && (int)p.ChucDanh.NhomChucDanhId != (int)Enums.EnumNhomChucDanh.BacSiDuPhong))
                .ApplyLike(model.Query, g => g.User.HoTen)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.User.HoTen,
                    KeyId = i.User.Id
                });
            return await listNhanVien.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetListLookupNhanVienIsBacSi(LookupQueryInfo model)
        {

            var listNhanVien = BaseRepository.TableNoTracking.Include(p => p.User)
                .Include(p => p.ChucDanh)
                .ThenInclude(p => p.NhomChucDanhId)
                .Include(p => p.KhoaPhongNhanViens)
                .Where(p => p.KhoaPhongNhanViens.Any(x => x.KhoaPhongId == model.Id)
                            && ((int)p.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi
                            || (int)p.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSiDuPhong))
                .ApplyLike(model.Query, g => g.User.HoTen)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.User.HoTen,
                    KeyId = i.User.Id
                });
            return await listNhanVien.ToListAsync();
        }

        //Thấy hàm GetListLookupNhanVienIsBacSi thua mẹ nó luôn model.Id = khoaphongId, hết đường nói
        public async Task<List<LookupItemVo>> GetListLookupNhanVienIsBacSiClone(LookupQueryInfo model)
        {
            var listNhanVien = await BaseRepository.TableNoTracking.Include(p => p.User)
                .Include(p => p.ChucDanh)
                .ThenInclude(p => p.NhomChucDanhId)
                .Include(p => p.KhoaPhongNhanViens)
                .Where(p => ((int)p.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi
                            || (int)p.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSiDuPhong))
                .ApplyLike(model.Query, g => g.User.HoTen)
                .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.User.HoTen,
                    KeyId = i.User.Id
                }).ToListAsync();

            var modelResult = await BaseRepository.TableNoTracking.Include(p => p.User)
                .Include(p => p.ChucDanh)
                .ThenInclude(p => p.NhomChucDanhId)
                .Include(p => p.KhoaPhongNhanViens)
                .Where(p => p.Id == model.Id)
                .ApplyLike(model.Query, g => g.User.HoTen)
                .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.User.HoTen,
                    KeyId = i.User.Id
                }).ToListAsync();

            listNhanVien.AddRange(modelResult);

            return listNhanVien;
        }

        public async Task<List<KhoaKhamTemplateVo>> GetListKhoaPhongByHoSoNhanVien(DropDownListRequestModel model)
        {
            model.Take = 200;
            var listKhoaPhong = await _khoaPhongRepository.TableNoTracking
                .Where(p => p.IsDisabled != true)
                .ApplyLike(model.Query, g => g.Ma, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoaPhong.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return query;
        }

        public async Task<bool> CheckMaChungChiAsync(string ma, long id)
        {
            if (string.IsNullOrEmpty(ma))
                return false;
            return await BaseRepository.TableNoTracking.AnyAsync(o => o.MaChungChiHanhNghe.Trim().ToLower() == ma.Trim().ToLower() && o.Id != id);
        }
        public async Task<bool> CheckVanBangChuyenMonAsync(long idVanBang)
        {
            return await _vanBangChuyenMonRepository.TableNoTracking.AnyAsync(o => o.Id == idVanBang);
        }
        public async Task<bool> CheckChucDanhAsync(long idChucDanh)
        {
            return await _chucDanhRepository.TableNoTracking.AnyAsync(o => o.Id == idChucDanh);
        }

        public async Task<List<LookupItemVo>> GetListPhongNhanVienByHoSoNhanVien(DropDownListRequestModel model, long nhanVienId, string khoaphongIds)
        {
            var query = new List<LookupItemVo>();
            if (!string.IsNullOrEmpty(khoaphongIds))
            {
                var khoaPhongBenhVienIds = JsonConvert.DeserializeObject<long[]>(khoaphongIds);
                var phongBenhViens = await _phongBenhVienRepository.TableNoTracking.Where(cc => khoaPhongBenhVienIds.Contains(cc.KhoaPhongId))
                     .ApplyLike(model.Query, g => g.Ma, g => g.Ten).ToListAsync();
                query = phongBenhViens.Select(item => new LookupItemVo
                {
                    DisplayName = item.Ma + " - " + item.Ten,
                    KeyId = item.Id
                }).ToList();
            }
            else
            {
                var khoaPhongNhanViens = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId).Select(cc => cc.KhoaPhongId);
                if (khoaPhongNhanViens.Any())
                {
                    var phongBenhViens = await _phongBenhVienRepository.TableNoTracking.Where(cc => khoaPhongNhanViens.Contains(cc.KhoaPhongId)).
                        ApplyLike(model.Query, g => g.Ma, g => g.Ten).ToListAsync();
                    query = phongBenhViens.Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ma + " - " + item.Ten,
                        KeyId = item.Id
                    }).ToList();
                }
            }
            return query;
        }

        public List<long> KhoTheoNhanVien(long NhanVienId)
        {
            var khoNhanViens = _khoNhanVienQuanLyRepository.TableNoTracking.Where(o => o.NhanVienId == NhanVienId).Select(p => p.Id).ToList();
            return khoNhanViens;
        }

        public List<long> kiemTraPhongThuocKhoa(List<long> phongBenhVienIds, long khoaPhongId)
        {
            var phongBenhVien = _phongBenhVienRepository.TableNoTracking.Where(o => phongBenhVienIds.Contains(o.Id) && o.KhoaPhongId == khoaPhongId).Select(p => p.Id).ToList();
            return phongBenhVien;
        }

        public List<long> KhoaTheoPhong(List<long> phongBenhVienIds)
        {
            var KhoaPhongIds = _phongBenhVienRepository.TableNoTracking.Where(o => phongBenhVienIds.Contains(o.Id)).Select(p => p.KhoaPhongId).Distinct().ToList();
            return KhoaPhongIds;
        }

        public void kiemTraKhoNhanVien(long nhanVienId)
        {
            var khoNhanViens = _khoNhanVienQuanLyRepository.Table.Where(o => o.NhanVienId == nhanVienId).ToList();
            foreach (var item in khoNhanViens)
            {
                _khoNhanVienQuanLyRepository.Delete(item);
            }
        }

        public List<LookupItemVo> GetTatCaPhongCuaNhanVienLogin(LookupQueryInfo model)
        {

            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var phongLinhVes = _khoaPhongNhanVienRepository.TableNoTracking
                .Where(p => p.NhanVienId == userCurrentId && p.PhongBenhVienId != null)
                .Select(s => new LookupItemVo
                {
                    KeyId = (long)s.PhongBenhVienId,
                    DisplayName = s.PhongBenhVien.Ten
                })
                .Union(_khoaPhongNhanVienRepository.TableNoTracking
                    .Where(p => p.NhanVienId == userCurrentId && p.PhongBenhVienId == null)
                    .SelectMany(s => s.KhoaPhong.PhongBenhViens.Select(o => new LookupItemVo
                    {
                        KeyId = (long)o.Id,
                        DisplayName = o.Ten
                    })))
                .ApplyLike(model.Query, g => g.DisplayName)
                .OrderBy(o => o.DisplayName).ToList();
            return phongLinhVes;
        }

        public async Task<string> GetNameNhanVienWithNhanVienId(long? id)
        {
            if (id == null) return "";
            var result = string.Empty;
            var nhanVien = BaseRepository.TableNoTracking
                .Include(p => p.User)
                .FirstOrDefault(p => p.Id == id);
            result = nhanVien?.User?.HoTen;
            return result;
        }

        public List<LookupItemVo> GetTatCaKhoLeCuaNhanVienLogin(LookupQueryInfo model)
        {

            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var khos = _khoRepository.TableNoTracking
                .Where(p => p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && p.KhoNhanVienQuanLys.Any(o => o.NhanVienId == userCurrentId))
                .Select(s => new LookupItemVo
                {
                    KeyId = (long)s.Id,
                    DisplayName = s.Ten
                })
                .ApplyLike(model.Query, g => g.DisplayName)
                .OrderBy(o => o.DisplayName).ToList();
            return khos;
        }
        public async Task<GridDataSource> GetListKhoaPhongDataForGridAsync(QueryInfo queryInfo)
        {

            BuildDefaultSortExpression(queryInfo);
            long nhanVienId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                nhanVienId = JsonConvert.DeserializeObject<long>(queryInfo.AdditionalSearchString);
            }

            var KhoaIds = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId && cc.PhongBenhVienId == null).Select(cc => cc.KhoaPhongId);
            var bvIds = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId).Select(cc => cc.PhongBenhVienId).ToList();
            var phongBenhViens = _phongBenhVienRepository.TableNoTracking.Where(o => KhoaIds.Contains(o.KhoaPhongId));

            if (phongBenhViens.Any())
            {
                foreach (var phongBenhVien in phongBenhViens)
                {
                    bvIds.Add(phongBenhVien.Id);
                }
            }


            var phongChinhNhanVien = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId && cc.LaPhongLamViecChinh == true).Select(cc => cc.PhongBenhVienId);
            var groupByNhomKhoas = _phongBenhVienRepository.TableNoTracking.Include(cc => cc.KhoaPhong).Include(cc => cc.KhoaPhongNhanViens).GroupBy(cc => cc.KhoaPhong.Ten)
                                                           .ToDictionary(g => g.Key, g => g.Any(ccc => bvIds.Contains(ccc.Id) == false));

            var query = _phongBenhVienRepository.TableNoTracking.Include(cc => cc.KhoaPhong).Include(cc => cc.KhoaPhongNhanViens)
                .Select(s => new KhoaPhongNhanVienVo
                {
                    Id = s.Id,
                    NhomKhoa = s.KhoaPhong.Ten,
                    NhomKhoaId = s.KhoaPhong.Id,
                    MaPhong = s.Ma,
                    TenPhong = s.Ten,
                    PhongChinh = phongChinhNhanVien.Contains(s.Id),
                    IsCheckedParent = !groupByNhomKhoas[s.KhoaPhong.Ten],
                    IsChecked = bvIds.Contains(s.Id),
                }).ApplyLike(queryInfo.SearchTerms, g => g.NhomKhoa, g => g.TenPhong, g => g.MaPhong);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetListKhoTheoPhongDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long nhanVienId = 0;
            var sreachKhoaPhong = new SreachKhoaPhong();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                sreachKhoaPhong = JsonConvert.DeserializeObject<SreachKhoaPhong>(queryInfo.AdditionalSearchString);
                if(sreachKhoaPhong.NhanVienId != null)
                {
                    nhanVienId = sreachKhoaPhong.NhanVienId;
                }
            }

            if(sreachKhoaPhong.PhongBenhVienIds  == null  && sreachKhoaPhong.KhoaPhongIds == null )
            {
                var KhoaIds = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId && cc.PhongBenhVienId == null).Select(cc => cc.KhoaPhongId).ToList(); ;
                var bvIds = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId).Select(cc => cc.PhongBenhVienId).ToList();
                var phongBenhViens = _phongBenhVienRepository.TableNoTracking.Where(o => KhoaIds.Contains(o.KhoaPhongId)).ToList();


                if (phongBenhViens.Any())
                {
                    foreach (var phongBenhVien in phongBenhViens)
                    {
                        bvIds.Add(phongBenhVien.Id);
                    }
                }


                var phongChinhNhanVien = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId && cc.LaPhongLamViecChinh == true).Select(cc => cc.PhongBenhVienId).ToList(); ;
                var groupByNhomKhoas = _phongBenhVienRepository.TableNoTracking.Include(cc => cc.KhoaPhong).Include(cc => cc.KhoaPhongNhanViens).GroupBy(cc => cc.KhoaPhong.Ten)
                                                               .ToDictionary(g => g.Key, g => g.Any(ccc => bvIds.Contains(ccc.Id) == false));

                var queryPhongBenhViens = _phongBenhVienRepository.TableNoTracking.Include(cc => cc.KhoaPhong).Include(cc => cc.KhoaPhongNhanViens)
                    .Select(s => new KhoaPhongNhanVienVo
                    {
                        Id = s.Id,
                        NhomKhoa = s.KhoaPhong.Ten,
                        NhomKhoaId = s.KhoaPhong.Id,
                        MaPhong = s.Ma,
                        TenPhong = s.Ten,
                        PhongChinh = phongChinhNhanVien.Contains(s.Id),
                        IsCheckedParent = !groupByNhomKhoas[s.KhoaPhong.Ten],
                        IsChecked = bvIds.Contains(s.Id),
                    }).ToList();




                sreachKhoaPhong.KhoaPhongIds = queryPhongBenhViens.Where(d => d.IsCheckedParent == true).Select(d => (long?)d.NhomKhoaId).ToList();

                var df = queryPhongBenhViens.Where(d => d.IsChecked == true).Select(d => d.NhomKhoaId).ToList();

                sreachKhoaPhong.PhongBenhVienIds = GetListPhongBenhViens(nhanVienId, df);

                sreachKhoaPhong.NhanVienId = (int)nhanVienId;
            }


            //var sreachKhoaPhong = new SreachKhoaPhong();
            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{
            //    sreachKhoaPhong = JsonConvert.DeserializeObject<SreachKhoaPhong>(queryInfo.AdditionalSearchString);
            //}
            var khoaTheoPhongIds = new List<long>();
            if (sreachKhoaPhong.PhongBenhVienIds != null)
            {
                khoaTheoPhongIds = _phongBenhVienRepository.TableNoTracking.Where(o=> sreachKhoaPhong.PhongBenhVienIds.Contains(o.Id)).Select(o => o.KhoaPhongId).Distinct().ToList();
            }
            var khoaPhongIds = new List<long>();
            var phongIds = new List<long>();
            if(sreachKhoaPhong.KhoaPhongIds != null)
            {
                khoaPhongIds = sreachKhoaPhong.KhoaPhongIds.Where(o => o != null).Select(o=>o.Value).ToList();
            }
            if (sreachKhoaPhong.PhongBenhVienIds != null)
            {
                phongIds = sreachKhoaPhong.PhongBenhVienIds.Where(o => o != null).Select(o => o.Value).ToList();
            }
            var khoNhanVienQuanLyIds = _khoNhanVienQuanLyRepository.TableNoTracking.Where(cc => cc.NhanVienId == (sreachKhoaPhong.NhanVienId)).Select(cc => cc.KhoId).ToList();
            var query = _khoRepository.TableNoTracking.Include(cc => cc.KhoaPhong).Include(cc => cc.PhongBenhVien)
            .Where(cc => khoaPhongIds.Contains(cc.KhoaPhongId.GetValueOrDefault()) || khoaTheoPhongIds.Contains(cc.KhoaPhongId.GetValueOrDefault()) ||
                        phongIds.Contains(cc.PhongBenhVienId.GetValueOrDefault()))
            .Select(s => new KhoNhanVienVo
            {
                Id = s.Id,
                TenKho = s.Ten,
                TenKhoa = s.KhoaPhong.Ten,
                TenPhong = s.PhongBenhVien.Ten,
                DaChon = sreachKhoaPhong.NhanVienId != 0 ? khoNhanVienQuanLyIds.Contains(s.Id) : false,
                
            });


            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenPhong, g => g.TenKhoa, g => g.TenKho);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<List<LookupItemTemplateVo>> GetListChucVu(DropDownListRequestModel model)
        {
            var list = await _chucVuRepository.TableNoTracking
              .Where(p => p.IsDisabled != true)
              .ApplyLike(model.Query, g => g.Ten)
               .Take(model.Take)
              .ToListAsync();
            var query = list.Select(item => new LookupItemTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();
            return query;
        }


        public async Task<List<LookupItemTextVo>> GetListMaBacSi(LookupQueryInfo model)
        {
            var listNhanVien = BaseRepository.TableNoTracking.Include(p => p.User)
                .Include(p => p.ChucDanh)
                .Where(p => !string.IsNullOrEmpty(p.MaChungChiHanhNghe))
                .ApplyLike(model.Query, g => g.User.HoTen, g => g.MaChungChiHanhNghe)
                .Select(i => new LookupItemTextVo
                {
                    DisplayName = i.MaChungChiHanhNghe + "-" + i.User.HoTen,
                    KeyId = i.MaChungChiHanhNghe
                });
            return await listNhanVien.ToListAsync();
        }

        private List<long?> GetListPhongBenhViens(long nhanVienId, List<long> khoaphongIds)
        {
            var query = new List<LookupItemVo>();
            if (khoaphongIds != null && khoaphongIds.Count() > 0)
            {
                var khoaPhongBenhVienIds = khoaphongIds;
                var phongBenhViens =  _phongBenhVienRepository.TableNoTracking.Where(cc => khoaPhongBenhVienIds.Contains(cc.KhoaPhongId))
                    .ToList();
                query = phongBenhViens.Select(item => new LookupItemVo
                {
                    DisplayName = item.Ma + " - " + item.Ten,
                    KeyId = item.Id
                }).ToList();
            }
            else
            {
                var khoaPhongNhanViens = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId).Select(cc => cc.KhoaPhongId);
                if (khoaPhongNhanViens.Any())
                {
                    var phongBenhViens =  _phongBenhVienRepository.TableNoTracking.Where(cc => khoaPhongNhanViens.Contains(cc.KhoaPhongId))
                       .ToList();
                    query = phongBenhViens.Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ma + " - " + item.Ten,
                        KeyId = item.Id
                    }).ToList();
                }
            }
            return query.Select(d => (long?)d.KeyId).ToList(); ;
        }
    }
}
