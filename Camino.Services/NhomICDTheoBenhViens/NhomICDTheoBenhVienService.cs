using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuocGia;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Camino.Services.NhomICDTheoBenhViens
{
    [ScopedDependency(ServiceType = typeof(INhomICDTheoBenhVienService))]
    public class NhomICDTheoBenhVienService : MasterFileService<NhomICDTheoBenhVien>, INhomICDTheoBenhVienService
    {
        private readonly IRepository<ICD> _icdRepository;
        private readonly IRepository<ChuongICD> _chuongICDRepository;
        public NhomICDTheoBenhVienService(IRepository<NhomICDTheoBenhVien> repository, IRepository<ICD> icdRepository, IRepository<ChuongICD> chuongICDRepository) : base(repository)
        {
            _icdRepository = icdRepository;
            _chuongICDRepository = chuongICDRepository;
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
                                    .Select(s => new NhomICDTheoBenhVienGridVo
                                    {
                                        Id = s.Id,
                                        Stt = s.Stt,
                                        Ma = s.Ma,
                                        TenTiengViet = s.TenTiengViet,
                                        HieuLuc = s.HieuLuc,
                                        MaICD = s.MoTa,
                                        TenChuongTiengViet = s.ChuongICD.TenTiengViet
                                    }).ApplyLike(queryInfo.SearchTerms,
                                        g => g.Ma,
                                        g => g.MaICD,
                                        g => g.TenTiengViet,
                                        g => g.TenChuongTiengViet
                                    );

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                                    .Select(s => new NhomICDTheoBenhVienGridVo
                                    {
                                        Id = s.Id,
                                        Stt = s.Stt,
                                        Ma = s.Ma,
                                        TenTiengViet = s.TenTiengViet,
                                        HieuLuc = s.HieuLuc,
                                        MaICD = s.MoTa,
                                        TenChuongTiengViet = s.ChuongICD.TenTiengViet
                                    }).ApplyLike(queryInfo.SearchTerms,
                                        g => g.Ma,
                                        g => g.MaICD,
                                        g => g.TenTiengViet,
                                        g => g.TenChuongTiengViet
                                    );
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public async Task<List<LookupItemVo>> GetMaICD(DropDownListRequestModel model, bool coHienThiMa = false)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listIcd = await _icdRepository.TableNoTracking
               .Where(p => (model.Id == 0 || (model.Id != 0 && model.Id != p.Id)) && (p.TenTiengViet.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")))
               .Take(model.Take)
               .ToListAsync();

                var modelResult = await _icdRepository.TableNoTracking
                    .Where(p => model.Id == p.Id)
                    .Take(model.Take)
                    .ToListAsync();

                listIcd.AddRange(modelResult);
                if (coHienThiMa)
                {
                    var query = listIcd.Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ma,
                        KeyId = item.Id
                    }).ToList();

                    return query;
                }
                else
                {
                    var query = listIcd.Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ma,
                        KeyId = item.Id
                    }).ToList();

                    return query;
                }
            }
            else
            {
                var lstColumnNameSearch = new List<string>
                {
                     nameof(ICD.Ma),
                     nameof(ICD.TenTiengViet),
                };
                var lstId = _icdRepository
                    .ApplyFulltext(model.Query, nameof(ICD), lstColumnNameSearch)
                    .Select(p => p.Id)
                    .Take(model.Take).ToList();

                if (coHienThiMa)
                {
                    var list = _icdRepository
                    .TableNoTracking
                    .Where(x => lstId.Any(a => a == x.Id))
                    .Take(model.Take)
                    .Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ma,
                        KeyId = item.Id
                    })
                    .OrderBy(x => lstId.IndexOf(x.KeyId))
                    .ToList();
                    return list;
                }
                else
                {
                    var list = _icdRepository
                    .TableNoTracking
                    .Where(x => lstId.Any(a => a == x.Id))
                    .Take(model.Take)
                    .Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ma,
                        KeyId = item.Id
                    })
                    .OrderBy(x => lstId.IndexOf(x.KeyId))
                    .ToList();
                    return list;
                }
            }
        }

        public async Task<List<LookupItemTemplateVo>> GetChuong(DropDownListRequestModel model)
        {
            if (!string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listIcd = await _chuongICDRepository.TableNoTracking
                                   .Where(p => (p.TenTiengViet + "-" + p.TenTiengAnh).Contains(model.Query ?? "") ||
                                   (p.TenTiengViet).Contains(model.Query ?? "") || (p.TenTiengAnh).Contains(model.Query ?? "") ||
                                    p.Ma.Contains(model.Query ?? "")).Take(model.Take).ToListAsync();

                var modelResult = await _chuongICDRepository.TableNoTracking
                    .Where(p => model.Id == p.Id)
                    .Take(model.Take)
                    .ToListAsync();

                var query = listIcd.Select(item => new LookupItemTemplateVo
                {
                    DisplayName = item.TenTiengViet + "-" + item.TenTiengAnh,
                    KeyId = item.Id,
                    Ma = item.Ma
                }).ToList();

                return query;
            }
            else
            {
                var lstColumnNameSearch = new List<string>
                {
                     nameof(ICD.Ma),
                     nameof(ICD.TenTiengViet),
                     nameof(ICD.TenTiengAnh),
                };
                var lstId = _chuongICDRepository
                    .ApplyFulltext(model.Query, nameof(ICD), lstColumnNameSearch)
                    .Select(p => p.Id)
                    .Take(model.Take).ToList();

                var list = _chuongICDRepository.TableNoTracking
                                                .Where(x => lstId.Any(a => a == x.Id))
                                                .Take(model.Take)
                                                .Select(item => new LookupItemTemplateVo
                                                {
                                                    DisplayName = item.TenTiengViet + "-" + item.TenTiengAnh,
                                                    KeyId = item.Id,
                                                    Ma = item.Ma
                                                })
                                                .OrderBy(x => lstId.IndexOf(x.KeyId)).ToList();

                return list;
            }
        }

        public List<LookupItemTextVo> GetMaTuTaoICD(DropDownListRequestModel model, JsonMaICD maICDs)
        {
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listIcd = MaTuTaoICD().Where(p => p.DisplayName.Contains(model.Query ?? "") || p.KeyId.Contains(model.Query ?? ""))
                                            .Take(model.Take)
                                            .ToList();

                if (!string.IsNullOrEmpty(maICDs.MaICD))
                {
                    var listMaICD = maICDs.MaICD.Split(";");
                    var modelResult = MaTuTaoICD().Where(p => listMaICD.Select(c => c).Contains(p.KeyId));
                    listIcd.AddRange(modelResult);
                }

                var query = listIcd.Select(item => new LookupItemTextVo
                {
                    DisplayName = item.DisplayName,
                    KeyId = item.KeyId
                }).ToList();

                return query;
            }
            else
            {

                var listIcd = MaTuTaoICD().Take(model.Take).ToList();

                if (!string.IsNullOrEmpty(maICDs.MaICD))
                {
                    var listMaICD = maICDs.MaICD.Split(";");
                    var modelResult = MaTuTaoICD().Where(p => listMaICD.Select(c => c).Contains(p.KeyId));
                    listIcd.AddRange(modelResult);
                }

                var query = listIcd.Select(item => new LookupItemTextVo
                {
                    DisplayName = item.DisplayName,
                    KeyId = item.KeyId
                }).ToList();

                return query;
            }
        }


        public async Task<bool> IsTenVietTatTiengVietExists(string tenTiengVietVietTat = null, long nhomICDTheoBenhVienId = 0)
        {
            var result = false;
            if (nhomICDTheoBenhVienId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengViet.Equals(tenTiengVietVietTat));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengViet.Equals(tenTiengVietVietTat) && p.Id != nhomICDTheoBenhVienId);
            }
            if (result)
                return false;
            return true;
        }


        public async Task<bool> IsTenVietTatTiengAnhExists(string tenTiengAnhtVietTat = null, long nhomICDTheoBenhVienId = 0)
        {
            var result = false;
            if (nhomICDTheoBenhVienId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengAnh.Equals(tenTiengAnhtVietTat));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengAnh.Equals(tenTiengAnhtVietTat) && p.Id != nhomICDTheoBenhVienId);
            }
            if (result)
                return false;
            return true;
        }


        public async Task<bool> IsMaBenhExists(string maBenh = null, long nhomICDTheoBenhVienId = 0)
        {
            var result = false;
            if (nhomICDTheoBenhVienId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maBenh));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maBenh) && p.Id != nhomICDTheoBenhVienId);
            }
            if (result)
                return false;
            return true;
        }


        public async Task<bool> IsSTTExists(string stt = null, long nhomICDTheoBenhVienId = 0)
        {
            var result = false;
            if (nhomICDTheoBenhVienId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Stt.Equals(stt));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Stt.Equals(stt) && p.Id != nhomICDTheoBenhVienId);
            }
            if (result)
                return false;
            return true;
        }


        public List<LookupItemTextVo> MaTuTaoICD()
        {
            var icds = new List<LookupItemTextVo>();

            string[] symbols = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            int counts = 99;
            foreach (var symbol in symbols)
            {
                for (int i = 0; i < counts; i++)
                {
                    var icd = new LookupItemTextVo();
                    icd.KeyId = i < 10 ? symbol + "0" + i : symbol + i;
                    icd.DisplayName = i < 10 ? symbol + "0" + i : symbol + i;
                    icds.Add(icd);
                }
            }
            return icds;
        }
    }
}
