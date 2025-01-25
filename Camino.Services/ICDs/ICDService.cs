using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Camino.Services.ICDs
{
    [ScopedDependency(ServiceType = typeof(IICDService))]
    public class ICDService : MasterFileService<ICD>, IICDService
    {
        private readonly IRepository<LoaiICD> _loaiICDRepository;
        private readonly IRepository<Khoa> _khoaRepository;

        public ICDService(
            IRepository<ICD> repository,
            IRepository<LoaiICD> loaiICDRepository,
            IRepository<Khoa> khoaRepository
            ) : base(repository)
        {
            _loaiICDRepository = loaiICDRepository;
            _khoaRepository = khoaRepository;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = BaseRepository.TableNoTracking
                .Select(s => new QuanLyICDGridVo
                {
                    Id = s.Id,
                    LoaiICDId = s.LoaiICDId,
                    TenLoaiICD = s.LoaiICD != null ? s.LoaiICD.TenTiengViet : null,
                    Ma = s.Ma,
                    MaChiTiet = s.MaChiTiet,
                    TenTiengViet = s.TenTiengViet,
                    TenTiengAnh = s.TenTiengAnh,
                    GioiTinh = s.GioiTinh,
                    GioiTinhDisplay = s.GioiTinh != null ? s.GioiTinh.GetDescription() : null,
                    LoiDanCuaBacSi = s.LoiDanCuaBacSi,
                    MoTa = s.MoTa,
                    ChuanDoanLamSan = s.ChuanDoanLamSan,
                    KhoaId = s.KhoaId,
                    TenKhoa = s.Khoa != null ? s.Khoa.Ten : null,
                    ManTinh = s.ManTinh == true ? "Có" : "Không",
                    BenhThuongGap = s.BenhThuongGap == true ? "Có" : "Không",
                    BenhNam = s.BenhNam == true ? "Có" : "Không",
                    KhongBaoHiem = s.KhongBaoHiem == true ? "Có" : "Không",
                    NgoaiDinhSuat = s.NgoaiDinhSuat == true ? "Có" : "Không",
                    HieuLuc = s.HieuLuc,
                    ThongTinThamKhaoChoBenhNhan = s.ThongTinThamKhaoChoBenhNhan,
                    TenGoiKhac = s.TenGoiKhac
                });

            query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.TenLoaiICD,
                    g => g.Ma,
                    g => g.MaChiTiet,
                    g => g.TenTiengViet,
                    g => g.TenTiengAnh,
                    g => g.LoiDanCuaBacSi,
                    g => g.MoTa,
                    g => g.TenKhoa,
                    g => g.ChuanDoanLamSan,
                    g => g.ThongTinThamKhaoChoBenhNhan,
                    g => g.TenGoiKhac
                    );

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new QuanLyICDGridVo
                {
                    Id = s.Id,
                    LoaiICDId = s.LoaiICDId,
                    TenLoaiICD = s.LoaiICD != null ? s.LoaiICD.TenTiengViet : null,
                    Ma = s.Ma,
                    MaChiTiet = s.MaChiTiet,
                    TenTiengViet = s.TenTiengViet,
                    TenTiengAnh = s.TenTiengAnh,
                    GioiTinh = s.GioiTinh,
                    GioiTinhDisplay = s.GioiTinh != null ? s.GioiTinh.GetDescription() : null,
                    LoiDanCuaBacSi = s.LoiDanCuaBacSi,
                    MoTa = s.MoTa,
                    ChuanDoanLamSan = s.ChuanDoanLamSan,
                    KhoaId = s.KhoaId,
                    TenKhoa = s.Khoa != null ? s.Khoa.Ten : null,
                    ManTinh = s.ManTinh == true ? "Có" : "Không",
                    BenhThuongGap = s.BenhThuongGap == true ? "Có" : "Không",
                    BenhNam = s.BenhNam == true ? "Có" : "Không",
                    KhongBaoHiem = s.KhongBaoHiem == true ? "Có" : "Không",
                    NgoaiDinhSuat = s.NgoaiDinhSuat == true ? "Có" : "Không",
                    HieuLuc = s.HieuLuc,
                    ThongTinThamKhaoChoBenhNhan = s.ThongTinThamKhaoChoBenhNhan,
                    TenGoiKhac = s.TenGoiKhac
                });

            query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.TenLoaiICD,
                    g => g.Ma,
                    g => g.MaChiTiet,
                    g => g.TenTiengViet,
                    g => g.TenTiengAnh,
                    g => g.LoiDanCuaBacSi,
                    g => g.MoTa,
                    g => g.TenKhoa,
                    g => g.ChuanDoanLamSan,
                    g => g.ThongTinThamKhaoChoBenhNhan,
                    g => g.TenGoiKhac
                    );

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<QuanLyICDTemplateVo>> GetTenLoaiICD(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(LoaiICD.Ma),
                nameof(LoaiICD.TenTiengViet),
            };
            var lstICDs = new List<QuanLyICDTemplateVo>();
            var loaiICDId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                lstICDs = await _loaiICDRepository.TableNoTracking
                    .Select(item => new QuanLyICDTemplateVo
                    {
                        DisplayName = item.TenTiengViet,
                        KeyId = item.Id,
                        Ten = item.TenTiengViet,
                        Ma = item.Ma,
                    })
                    .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                    .OrderByDescending(x => x.KeyId == loaiICDId).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstICDId = _loaiICDRepository
                    .ApplyFulltext(queryInfo.Query, nameof(LoaiICD), lstColumnNameSearch)
                    .Select(x => x.Id)
                    .ToList();

                lstICDs = await _loaiICDRepository.TableNoTracking
                    .Where(x => loaiICDId == 0 || lstICDId.Contains(x.Id))
                    .OrderByDescending(x => x.Id == loaiICDId)
                    .ThenBy(p => lstICDId.IndexOf(p.Id) != -1 ? lstICDId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new QuanLyICDTemplateVo
                    {
                        DisplayName = item.TenTiengViet,
                        KeyId = item.Id,
                        Ten = item.TenTiengViet,
                        Ma = item.Ma,
                    }).ToListAsync();
            }
            return lstICDs;
        }

        public async Task<List<KhoaQuanLyICDTemplateVo>> GetTenKhoa(DropDownListRequestModel queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                var lstKhoas = _khoaRepository.TableNoTracking
                .OrderByDescending(x => long.Parse(queryInfo.ParameterDependencies) == 0
                                        || x.Id == long.Parse(queryInfo.ParameterDependencies)).ThenBy(x => x.Id)
                .Select(item => new KhoaQuanLyICDTemplateVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    Ten = item.Ten,
                    Ma = item.Ma,
                })
                .ApplyLike(queryInfo.Query, o => o.Ma, o => o.Ten, o => o.DisplayName)
                .Take(queryInfo.Take);
                return await lstKhoas.ToListAsync();
            }
            else
            {
                var lstKhoas = _khoaRepository.TableNoTracking
                .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                .Select(item => new KhoaQuanLyICDTemplateVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    Ten = item.Ten,
                    Ma = item.Ma,
                })
                .ApplyLike(queryInfo.Query, o => o.Ma, o => o.Ten, o => o.DisplayName)
                .Take(queryInfo.Take);

                return await lstKhoas.ToListAsync();
            }
        }

        public async Task<bool> CheckLoaiICDExist(long? id)
        {
            var loaiICDs = await _loaiICDRepository.TableNoTracking
                .Where(x => x.Id == id).ToListAsync();
            if (loaiICDs.Any())
            {
                return true;
            }
            return false;
        }
        public async Task<bool> CheckKhoaExist(long? id)
        {
            var khoas = await _khoaRepository.TableNoTracking
                .Where(x => x.Id == id).ToListAsync();
            if (khoas.Any() || id == 0)
            {
                return true;
            }
            return false;
        }

        public async Task<string> GetMaICD(long id)
        {
            var entity = await BaseRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);

            return entity?.Ma ?? "";
        }

        public async Task<bool> IsMaExist(string ma = null, long Id = 0)
        {
            var result = false;
            if (ma == null)
            {
                return result;
            }
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(ma) && p.Id != Id);
            }
            return result;
        }

        public async Task<bool> IsTenExist(string ten = null, long Id = 0)
        {
            var result = false;
            if (ten == null)
            {
                return result;
            }
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengViet.Equals(ten));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengViet.Equals(ten) && p.Id != Id);
            }
            return result;
        }

    }
}
