using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.Thuocs
{
    [ScopedDependency(ServiceType = typeof(IDuocPhamService))]
    public class DuocPhamService : MasterFileService<DuocPham>, IDuocPhamService
    {
        private readonly IRepository<DuongDung> _duongDungRepository;
        private readonly IRepository<Core.Domain.Entities.DonViTinhs.DonViTinh> _dVTRepository;
        private readonly IRepository<Core.Domain.Entities.NhaSanXuats.NhaSanXuat> _nhaSanXuatRepository;
        private readonly IRepository<Core.Domain.Entities.QuocGias.QuocGia> _quocGiaRepository;
        private readonly IRepository<Core.Domain.Entities.Thuocs.ThuocHoacHoatChat> _thuocHoacHoatChatRepository;
        public DuocPhamService(IRepository<DuocPham> repository,
            IRepository<DuongDung> duongDungRepository,
            IRepository<Core.Domain.Entities.DonViTinhs.DonViTinh> dVTRepository,
            IRepository<Core.Domain.Entities.NhaSanXuats.NhaSanXuat> nhaSanXuatRepository,
            IRepository<Core.Domain.Entities.QuocGias.QuocGia> quocGiaRepository,
            IRepository<Core.Domain.Entities.Thuocs.ThuocHoacHoatChat> thuocHoacHoatChatRepository) : base(repository)
        {
            _duongDungRepository = duongDungRepository;
            _nhaSanXuatRepository = nhaSanXuatRepository;
            _quocGiaRepository = quocGiaRepository;
            _dVTRepository = dVTRepository;
            _thuocHoacHoatChatRepository = thuocHoacHoatChatRepository;
        }

        #region Danh sách dược phẩm
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking
                .Include(x => x.DonViTinh)
                .Include(x => x.DuongDung)
                .Include(x => x.LoaiThuocHoacHoatChat)
                .ApplyLike(queryInfo.SearchTerms, x => x.Ten, x => x.MaHoatChat, x => x.DuongDung.Ten, x => x.DonViTinh.Ten,
                         x => x.SoDangKy, x => x.MaHoatChat, x => x.HoatChat, x => x.QuyCach)
                .Select(s => new DuocPhamGridVo()
                {
                    Id = s.Id,
                    //  MaThuocBenhVien = s.MaThuocBenhVien,
                    Ten = s.Ten,
                    SoDangKy = s.SoDangKy,
                    MaHoatChat = s.MaHoatChat,
                    HoatChat = s.HoatChat,
                    QuyCach = s.QuyCach,
                    TenDonViTinh = s.DonViTinh.Ten,
                    TenDuongDung = s.DuongDung.Ten,
                    TenLoaiThuocHoacHoatChat = s.LoaiThuocHoacHoatChat.GetDescription(),
                    HeSoDinhMucDonViTinh = s.HeSoDinhMucDonViTinh
                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Include(x => x.DonViTinh)
                .Include(x => x.DuongDung).Include(x => x.LoaiThuocHoacHoatChat)
                .ApplyLike(queryInfo.SearchTerms, x => x.Ten, x => x.MaHoatChat, x => x.DuongDung.Ten, x => x.DonViTinh.Ten,
                         x => x.SoDangKy, x => x.MaHoatChat, x => x.HoatChat, x => x.QuyCach)
                .Select(s => new DuocPhamGridVo()
                {
                    Id = s.Id,
                    //MaThuocBenhVien = s.MaThuocBenhVien,
                    Ten = s.Ten,
                    SoDangKy = s.SoDangKy,
                    MaHoatChat = s.MaHoatChat,
                    HoatChat = s.HoatChat,
                    QuyCach = s.QuyCach,
                    TenDonViTinh = s.DonViTinh.Ten,
                    TenDuongDung = s.DuongDung.Ten,
                    TenLoaiThuocHoacHoatChat = s.LoaiThuocHoacHoatChat.GetDescription(),
                    HeSoDinhMucDonViTinh = s.HeSoDinhMucDonViTinh
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region get thông tin chung
        public List<LookupItemVo> GetDanhSachLoaiThuocHoacHoatChat(DropDownListRequestModel queryInfo)
        {
            var thuocHoacHoatChats = EnumHelper.GetListEnum<Enums.LoaiThuocHoacHoatChat>()
                        .Select(s => new LookupItemVo()
                        {
                            KeyId = (long)s,
                            DisplayName = s.GetDescription()
                        }).OrderByDescending(s => s.KeyId == (long)Enums.LoaiThuocHoacHoatChat.ThuocTanDuoc).ThenBy(p=>p.DisplayName).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                thuocHoacHoatChats = thuocHoacHoatChats.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return thuocHoacHoatChats;
        }

        public async Task<ICollection<DuongDungTemplateVo>> GetDanhSachDuongDungAsync(DropDownListRequestModel model)
        {
            var lstDuongDung = await _duongDungRepository.TableNoTracking
                .ApplyLike(model.Query, g => g.Ten, g => g.Ma)
                .Where(p => p.IsDisabled != true)
                .Take(model.Take)
                .ToListAsync();

            var lst = lstDuongDung
                    .Select(item => new DuongDungTemplateVo()
                    {
                        //DisplayName = item.Ma + " - " + item.Ten,
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = item.Ma
                    }).OrderBy(x => x.DisplayName).ToList();
            return lst;
        }

        public async Task<ICollection<string>> GetListTenNhaSanXuatAsync()
        {
            var lstTenNhaSanXuat =
                await _nhaSanXuatRepository.TableNoTracking
                    .Where(x => !string.IsNullOrEmpty(x.Ten.Trim()))
                    .Select(x => x.Ten).ToListAsync();
            return lstTenNhaSanXuat;
        }

        public async Task<ICollection<string>> GetListTenNuocSanXuatAsync()
        {
            var lstTenNuocSanXuat =
                await _quocGiaRepository.TableNoTracking
                    .Where(x => !string.IsNullOrEmpty(x.Ten.Trim()))
                    .Select(x => x.Ten).ToListAsync();
            return lstTenNuocSanXuat;
        }
        public async Task<ICollection<MaHoatChatHoatChatDuongDungTemplateVo>> GetListTenMaHoatChatAsync(DropDownListRequestModel queryInfo)
        {
            var lstTenMaHoatChat = await _thuocHoacHoatChatRepository.TableNoTracking
              .Take(queryInfo.Take)
              .ApplyLike(queryInfo.Query, g => g.Ma, g => g.Ten)
              .ToListAsync();

            var query = lstTenMaHoatChat.Select(item => new MaHoatChatHoatChatDuongDungTemplateVo()
            {
                DuongDung = item.DuongDung.Ten,
                HoatChat = item.Ten,
                MaHoatChat = item.Ma
            });
            return query.ToList();
        }
        public async Task<ICollection<string>> GetListTenHoatChatVaDuongDungAsync()
        {
            var lstTenHoatChat =
                await _thuocHoacHoatChatRepository.TableNoTracking
                    .Where(x => !string.IsNullOrEmpty(x.Ten.Trim()))
                    .Select(x => x.Ten + '-' + x.DuongDung.Ten).ToListAsync();
            return lstTenHoatChat;
        }
        public async Task<string> GetTenHoatChatAsync(string tenHoatChatDuongDung)
        {
            var query = tenHoatChatDuongDung.Split('-');
            var lstTenHoatChat =
                 _thuocHoacHoatChatRepository.TableNoTracking
                    .Where(x => x.Ten == query[0] && x.DuongDung.Ten == query[1])
                    .Select(x => x.Ma).FirstOrDefault();
            return lstTenHoatChat;
        }
        #endregion

        #region xử lý

        public async Task<bool> KiemTraSoDangKyTonTaiAsync(string soDangKy, long? id)
        {
            if (string.IsNullOrEmpty(soDangKy))
            {
                return false;
            }
            var isExits = await BaseRepository.TableNoTracking.AnyAsync(x => x.Id != id && x.SoDangKy.ToLower().Trim() == soDangKy.ToLower().Trim());
            return isExits;
        }

        public bool CheckTenExits(string ten, long id)
        {
            bool isChecked = false;
            if (string.IsNullOrEmpty(ten) || id != 0)
            {
                string tenCheck = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ten == tenCheck.Trim() && x.Id != id);
                if (entity.Any())
                    isChecked = true;
            }
            else
            {
                string tenCheck = !string.IsNullOrEmpty(ten) ? ten.TrimEnd().TrimStart() : "";
                var entity = BaseRepository.TableNoTracking.Where(x => x.Ten == tenCheck.Trim());
                if (entity.Any())
                    isChecked = true;
            }
            return isChecked;
        }


        #endregion


        public async Task<List<LookupItemVo>> GetListLookupDuocPham(DropDownListRequestModel model)
        {
            var list = BaseRepository.TableNoTracking
                .Include(p => p.DuocPhamBenhVien)
                .Where(p => p.DuocPhamBenhVien.HieuLuc == true && p.DuocPhamBenhVien != null)
                .ApplyLike(model.Query, g => g.Ten)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.DuocPhamBenhVien.Id
                });
            return await list.ToListAsync();
        }
        public async Task<List<LookupItemVo>> GetListNhaSanXuat(DropDownListRequestModel model)
        {
            var list = BaseRepository.TableNoTracking.Include(p => p.DuocPhamBenhVien)
                .Where(p => p.DuocPhamBenhVien.HieuLuc == true && p.DuocPhamBenhVien != null)
                .ApplyLike(model.Query, g => g.NhaSanXuat)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.NhaSanXuat,
                    KeyId = i.Id
                });
            return await list.ToListAsync();
        }

        public async Task<bool> CheckDuongDungAsync(long Idduongdung)
        {
            return await _duongDungRepository.TableNoTracking.AnyAsync(o => o.Id == Idduongdung);
        }
        public async Task<bool> CheckDVTAsync(long Id)
        {
            return await _dVTRepository.TableNoTracking.AnyAsync(o => o.Id == Id);
        }
        public async Task<bool> ChecMaHoatChatAsync(string ma, long id)
        {
            return await BaseRepository.TableNoTracking.AnyAsync(o => o.MaHoatChat == ma && o.Id != id);
        }

        public async Task<bool> IsTenExists(string ten = null, long id = 0)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
            }
            if (result)
                return false;
            return true;
        }
    }
}
