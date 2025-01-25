using System.Collections.Generic;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.ToaThuocMau;

namespace Camino.Services.ToaThuocMau
{
    [ScopedDependency(ServiceType = typeof(IToaThuocMauService))]

    public class ToaThuocMauService : MasterFileService<Core.Domain.Entities.Thuocs.ToaThuocMau>, IToaThuocMauService
    {

        private readonly IRepository<ToaThuocMauChiTiet> _toaThuocMauChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.ICDs.ICD> _iCDRepository;
        private readonly IRepository<Core.Domain.Entities.Users.User> _userRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;

        private readonly IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> _trieuChungRepository;
        private readonly IRepository<Core.Domain.Entities.ICDs.ChuanDoan> _chuanDoanRepository;
        private readonly IRepository<DuocPham> _duocPhamRepository;


        public ToaThuocMauService(
            IRepository<Core.Domain.Entities.Thuocs.ToaThuocMau> repository,
            IRepository<ToaThuocMauChiTiet> toaThuocMauChiTietRepository,
            IRepository<Core.Domain.Entities.ICDs.ICD> iCDRepository,
            IRepository<Core.Domain.Entities.Users.User> userRepository,
            IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> trieuChungRepository,
            IRepository<Core.Domain.Entities.ICDs.ChuanDoan> chuanDoanRepository,
            IRepository<DuocPham> duocPhamRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository
            ) : base(repository)
        {
            _toaThuocMauChiTietRepository = toaThuocMauChiTietRepository;
            _iCDRepository = iCDRepository;
            _userRepository = userRepository;
            _trieuChungRepository = trieuChungRepository;
            _chuanDoanRepository = chuanDoanRepository;
            _duocPhamRepository = duocPhamRepository;
            _nhanVienRepository = nhanVienRepository;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository
                .TableNoTracking
                .Select(s => new ToaThuocMauGridVo
                {
                    Id = s.Id,
                    ICDId = s.ICDId,
                    TrieuChungId = s.TrieuChungId,
                    ChuanDoanId = s.ChuanDoanId,
                    BacSiKeToaId = s.BacSiKeToaId,
                    Ten = s.Ten,
                    //TenICD = s.ICD.Ma + " - " + s.ICD.TenTiengViet,
                    TenICD = s.ICD.TenTiengViet,
                    TenTrieuChung = s.TrieuChung.Ten,
                    //ChuanDoan = s.ChuanDoan.Ma + " - " + ((s.ChuanDoan.TenTiengViet != "" && s.ChuanDoan.TenTiengViet != null) ? s.ChuanDoan.TenTiengViet : s.ChuanDoan.TenTiengAnh),
                    ChuanDoan = ((s.ChuanDoan.TenTiengViet != "" && s.ChuanDoan.TenTiengViet != null) ? s.ChuanDoan.TenTiengViet : s.ChuanDoan.TenTiengAnh),
                    TenBacSiKeToa = s.BacSiKeToa.User.HoTen,
                    GhiChu = s.GhiChu,
                    IsDisabled = s.IsDisabled
                });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenICD, g => g.TenTrieuChung, g => g.ChuanDoan, g => g.TenBacSiKeToa, g => g.GhiChu);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository
                 .TableNoTracking
                 .Select(s => new ToaThuocMauGridVo
                 {
                     Id = s.Id,
                     ICDId = s.ICDId,
                     TrieuChungId = s.TrieuChungId,
                     ChuanDoanId = s.ChuanDoanId,
                     BacSiKeToaId = s.BacSiKeToaId,
                     Ten = s.Ten,
                     //TenICD = s.ICD.Ma + " - " + s.ICD.TenTiengViet,
                     TenICD = s.ICD.TenTiengViet,
                     TenTrieuChung = s.TrieuChung.Ten,
                     //ChuanDoan = s.ChuanDoan.Ma + " - " + ((s.ChuanDoan.TenTiengViet != "" && s.ChuanDoan.TenTiengViet != null) ? s.ChuanDoan.TenTiengViet : s.ChuanDoan.TenTiengAnh),
                     ChuanDoan = ((s.ChuanDoan.TenTiengViet != "" && s.ChuanDoan.TenTiengViet != null) ? s.ChuanDoan.TenTiengViet : s.ChuanDoan.TenTiengAnh),
                     TenBacSiKeToa = s.BacSiKeToa.User.HoTen,
                     GhiChu = s.GhiChu,
                     IsDisabled = s.IsDisabled

                 });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenICD, g => g.TenTrieuChung, g => g.ChuanDoan, g => g.TenBacSiKeToa, g => g.GhiChu);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridToaThuocMauChiTietChildAsync(QueryInfo queryInfo, long? toaThuocMauId, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long par = 0;
            if (toaThuocMauId != null && toaThuocMauId != 0)
            {
                par = toaThuocMauId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }

            var query = _toaThuocMauChiTietRepository
                .TableNoTracking
                //.Where(o => o.ToaThuocMauId == long.Parse(queryInfo.SearchTerms))
                .Where(o => o.ToaThuocMauId == par)
                .Select(s => new ToaThuocMauChiTietGridVo
                {
                    Id = s.Id,
                    ToaThuocMauId = s.ToaThuocMauId,
                    DuocPhamId = s.DuocPhamId,
                    TenDuocPham = s.DuocPham.MaHoatChat + " - " + s.DuocPham.Ten,
                    SoLuong = s.SoLuong,
                    SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                    SoNgayDung = s.SoNgayDung,
                    DungSangDisplay = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                    DungTruaDisplay = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                    DungChieuDisplay = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                    DungToiDisplay = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                    ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                    ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                    GhiChu = s.GhiChu
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridToaThuocMauChiTietChildAsync(QueryInfo queryInfo)
        {
            var query = _toaThuocMauChiTietRepository
                 .TableNoTracking
                 .Where(o => o.ToaThuocMauId == long.Parse(queryInfo.SearchTerms))
                 .Select(s => new ToaThuocMauChiTietGridVo
                 {
                     Id = s.Id,
                     ToaThuocMauId = s.ToaThuocMauId,
                     DuocPhamId = s.DuocPhamId,
                     TenDuocPham = s.DuocPham.MaHoatChat + " - " + s.DuocPham.Ten,
                     SoLuong = s.SoLuong,
                     SoLuongDisplay = ((double?)s.SoLuong).FloatToStringFraction(),
                     SoNgayDung = s.SoNgayDung,
                     DungSangDisplay = s.DungSang == null ? null : s.DungSang.FloatToStringFraction(),
                     DungTruaDisplay = s.DungTrua == null ? null : s.DungTrua.FloatToStringFraction(),
                     DungChieuDisplay = s.DungChieu == null ? null : s.DungChieu.FloatToStringFraction(),
                     DungToiDisplay = s.DungToi == null ? null : s.DungToi.FloatToStringFraction(),
                     ThoiGianDungSangDisplay = s.ThoiGianDungSang == null ? null : "(" + s.ThoiGianDungSang.Value.ConvertIntSecondsToTime12h() + ")",
                     ThoiGianDungTruaDisplay = s.ThoiGianDungTrua == null ? null : "(" + s.ThoiGianDungTrua.Value.ConvertIntSecondsToTime12h() + ")",
                     ThoiGianDungChieuDisplay = s.ThoiGianDungChieu == null ? null : "(" + s.ThoiGianDungChieu.Value.ConvertIntSecondsToTime12h() + ")",
                     ThoiGianDungToiDisplay = s.ThoiGianDungToi == null ? null : "(" + s.ThoiGianDungToi.Value.ConvertIntSecondsToTime12h() + ")",
                     GhiChu = s.GhiChu
                 });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<List<ICDTemplateVos>> GetICDs(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.ICDs.ICD.Ma),
                nameof(Core.Domain.Entities.ICDs.ICD.TenTiengViet),
            };
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var lstICDs = await _iCDRepository.TableNoTracking
                    .OrderByDescending(x => long.Parse(queryInfo.ParameterDependencies) == 0 || x.Id == long.Parse(queryInfo.ParameterDependencies)).ThenBy(x => x.Id)
                    .Select(item => new ICDTemplateVos
                    {
                        DisplayName = item.Ma + " - " + item.TenTiengViet,
                        KeyId = item.Id,
                        Ten = item.TenTiengViet,
                        Ma = item.Ma,
                    })
                    .ApplyLike(queryInfo.Query, o => o.Ma, o => o.Ten, o => o.DisplayName)
                    .Take(queryInfo.Take)
                    .ToListAsync();
                    return lstICDs;
                }
                else
                {
                    var lstICDs = await _iCDRepository.TableNoTracking
                    .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                    .Select(item => new ICDTemplateVos
                    {
                        //DisplayName = item.TenTiengViet,
                        DisplayName = item.Ma + " - " + item.TenTiengViet,
                        KeyId = item.Id,
                        Ten = item.TenTiengViet,
                        Ma = item.Ma,
                    })
                    .ApplyLike(queryInfo.Query, o => o.Ma, o => o.Ten, o => o.DisplayName)
                    .Take(queryInfo.Take)
                    .ToListAsync();
                    return lstICDs;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var listICDIds = _iCDRepository
                               .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.ICDs.ICD), lstColumnNameSearch)
                               .Select(p => p.Id).ToList();

                    var dictionary = listICDIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var lstICDs = await _iCDRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.ICDs.ICD), lstColumnNameSearch)
                                        .OrderByDescending(x => long.Parse(queryInfo.ParameterDependencies) == 0 || x.Id == long.Parse(queryInfo.ParameterDependencies)).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new ICDTemplateVos
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Ma + " - " + item.TenTiengViet,
                                            KeyId = item.Id,
                                            Ten = item.TenTiengViet,
                                            Ma = item.Ma,
                                        }).ToListAsync();
                    return lstICDs;
                }
                else
                {
                    var listICDIds = _iCDRepository
                               .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.ICDs.ICD), lstColumnNameSearch)
                               .Select(p => p.Id).ToList();

                    var dictionary = listICDIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var lstICDs = await _iCDRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.ICDs.ICD), lstColumnNameSearch)
                                        .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new ICDTemplateVos
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Ma + " - " + item.TenTiengViet,
                                            KeyId = item.Id,
                                            Ten = item.TenTiengViet,
                                            Ma = item.Ma,
                                        }).ToListAsync();
                    return lstICDs;
                }
            }
        }

        public async Task<List<NhanVienTemplateVos>> GetBacSiKeToas(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.Users.User.HoTen),
                nameof(Core.Domain.Entities.Users.User.SoDienThoai),

            };
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var nhanViens = _nhanVienRepository.TableNoTracking
                    .Where(p => p.User.SoDienThoai != "0000000000")
                    .OrderByDescending(x => long.Parse(queryInfo.ParameterDependencies) == 0
                    || x.Id == long.Parse(queryInfo.ParameterDependencies)).ThenBy(x => x.Id)
                    .Select(item => new NhanVienTemplateVos
                    {
                        DisplayName = item.User.HoTen,
                        KeyId = item.Id,
                    })
                    .ApplyLike(queryInfo.Query, o => o.DisplayName)
                    .Take(queryInfo.Take);

                    return await nhanViens.ToListAsync();
                }
                else
                {
                    var nhanViens = _nhanVienRepository.TableNoTracking
                    .Where(p => p.User.SoDienThoai != "0000000000")

                    .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                    .Select(item => new NhanVienTemplateVos
                    {
                        DisplayName = item.User.HoTen,
                        KeyId = item.Id,
                    })
                    .ApplyLike(queryInfo.Query, o => o.DisplayName)
                    .Take(queryInfo.Take);

                    
                    return await nhanViens.ToListAsync();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var nhanVienIds = _nhanVienRepository
                               .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                               .Where(p => p.User.SoDienThoai != "0000000000")
                               .Select(p => p.Id).ToList();

                    var dictionary = nhanVienIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var nhanViens = _nhanVienRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                                        .OrderByDescending(x => long.Parse(queryInfo.ParameterDependencies) == 0 || x.Id == long.Parse(queryInfo.ParameterDependencies)).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new NhanVienTemplateVos
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.User.HoTen,
                                            KeyId = item.Id,
                                            Ten = item.User.HoTen,
                                            SoDienThoai = item.User.SoDienThoai,
                                        });
                    return await nhanViens.ToListAsync();
                }
                else
                {
                    var nhanVienIds = _nhanVienRepository
                               .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                               .Where(p => p.User.SoDienThoai != "0000000000")
                               .Select(p => p.Id).ToList();

                    var dictionary = nhanVienIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var nhanViens = _nhanVienRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                                        .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new NhanVienTemplateVos
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.User.HoTen,
                                            KeyId = item.Id,
                                            Ten = item.User.HoTen,
                                            SoDienThoai = item.User.SoDienThoai
                                        });

                    return await nhanViens.ToListAsync();
                }
            }
        }

        public async Task<List<LookupItemVo>> GetTrieuChungs(DropDownListRequestModel queryInfo)
        {
            var lstTrieuChungs = await _trieuChungRepository.TableNoTracking
                .ApplyLike(queryInfo.Query, o => o.Ten).Take(queryInfo.Take)
                .ToListAsync();
            var result = lstTrieuChungs.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();
            return result;
        }

        public async Task<List<ChuanDoanTemplateVo>> GetChuanDoans(DropDownListRequestModel queryInfo)
        {
            var lstChuanDoans = await _chuanDoanRepository.TableNoTracking
                .ApplyLike(queryInfo.Query, o => o.Ma, o => o.TenTiengViet, o => o.TenTiengAnh).Take(queryInfo.Take)
                .ToListAsync();
            var result = lstChuanDoans.Select(item => new ChuanDoanTemplateVo
            {
                //DisplayName = item.Ma + " - " + (item.TenTiengViet != "" ? item.TenTiengViet : item.TenTiengAnh),
                DisplayName = (item.TenTiengViet != "" ? item.TenTiengViet : item.TenTiengAnh),
                KeyId = item.Id,
                Ten = (item.TenTiengViet != "" ? item.TenTiengViet : item.TenTiengAnh),
                Ma = item.Ma,
            }).ToList();
            return result;
        }

        public async Task<List<DuocPhamTemplateGridVo>> GetDuocPhams(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
                {
                    nameof(DuocPham.Ten),
                    nameof(DuocPham.HoatChat)
                };


            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var duocPhamId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                    var duocPhams = _duocPhamRepository.TableNoTracking
                    .OrderByDescending(x => duocPhamId == 0 || x.Id == duocPhamId).ThenBy(x => x.Id)
                    .Select(item => new DuocPhamTemplateGridVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        DVT = item.DonViTinh.Ten,
                        DuongDung = item.DuongDung.Ten,
                        HamLuong = item.HamLuong,
                        NhaSanXuat = item.NhaSanXuat,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    })
                    .ApplyLike(queryInfo.Query, o => o.HoatChat, o => o.Ten, o => o.DisplayName)
                    .Take(queryInfo.Take);
                    return await duocPhams.ToListAsync();
                }
                else
                {
                    var duocPhams = _duocPhamRepository.TableNoTracking
                    .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                    .Select(item => new DuocPhamTemplateGridVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        HoatChat = item.HoatChat,
                        DVT = item.DonViTinh.Ten,
                        DuongDung = item.DuongDung.Ten,
                        HamLuong = item.HamLuong,
                        NhaSanXuat = item.NhaSanXuat,
                        DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                        SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                        MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                        DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                    })
                    .ApplyLike(queryInfo.Query, o => o.HoatChat, o => o.Ten, o => o.DisplayName)
                    .Take(queryInfo.Take);
                    return await duocPhams.ToListAsync();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var duocPhamId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                    var duocPhamIds = _duocPhamRepository
                               .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                               .Select(p => p.Id).ToList();

                    var dictionary = duocPhamIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var duocPhams = _duocPhamRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                                        .OrderByDescending(x => duocPhamId == 0 || x.Id == duocPhamId).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new DuocPhamTemplateGridVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Ten,
                                            KeyId = item.Id,
                                            Ten = item.Ten,
                                            HoatChat = item.HoatChat,
                                            DVT = item.DonViTinh.Ten,
                                            DuongDung = item.DuongDung.Ten,
                                            HamLuong = item.HamLuong,
                                            NhaSanXuat = item.NhaSanXuat,
                                            DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                                            SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                                            MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                                            DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                                        });
                    return await duocPhams.ToListAsync();
                }
                else
                {
                    var duocPhamIds = _duocPhamRepository
                               .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                               .Select(p => p.Id).ToList();

                    var dictionary = duocPhamIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var duocPhams = _duocPhamRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearch)
                                         //.OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                                         .OrderByDescending(x => x.Id == queryInfo.Id)
                                        .ThenBy(p => duocPhamIds.IndexOf(p.Id) != -1 ? duocPhamIds.IndexOf(p.Id) : queryInfo.Take + 1)
                                        .Take(queryInfo.Take)
                                        .Select(item => new DuocPhamTemplateGridVo
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.Ten,
                                            KeyId = item.Id,
                                            Ten = item.Ten,
                                            HoatChat = item.HoatChat,
                                            DVT = item.DonViTinh.Ten,
                                            DuongDung = item.DuongDung.Ten,
                                            HamLuong = item.HamLuong,
                                            NhaSanXuat = item.NhaSanXuat,
                                            DuocPhamBenhVienId = item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Id : 0,
                                            SuDungTaiBenhVien = item.DuocPhamBenhVien != null,
                                            MaDuocPhamBenhVien = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.Ma : ""),
                                            DuocPhamBenhVienPhanNhomId = (item.DuocPhamBenhVien != null ? item.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId : null)
                                        });
                    return await duocPhams.ToListAsync();
                }
            }



        }
    }
}
