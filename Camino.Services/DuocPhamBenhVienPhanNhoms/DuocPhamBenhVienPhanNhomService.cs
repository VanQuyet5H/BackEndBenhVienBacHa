using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuocPhamBenhVienPhanNhoms;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.DuocPhamBenhVienPhanNhoms
{
    [ScopedDependency(ServiceType = typeof(IDuocPhamBenhVienPhanNhomService))]
    public class DuocPhamBenhVienPhanNhomService : MasterFileService<DuocPhamBenhVienPhanNhom>, IDuocPhamBenhVienPhanNhomService
    {
        public DuocPhamBenhVienPhanNhomService(IRepository<DuocPhamBenhVienPhanNhom> repository) : base(repository)
        {
        }

        public async Task<List<DuocPhamBenhVienPhanNhomGridVo>> GetDataTreeView(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new DuocPhamBenhVienPhanNhomGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    CapNhom = s.CapNhom,
                    NhomChaId = s.NhomChaId
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var list = query
                    .Select(k => new DuocPhamBenhVienPhanNhomGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        NhomChaId = k.NhomChaId,
                        DuocPhamBenhVienPhanNhomChildren = GetChildrenLoadLastFirst(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);
                return await list.ToListAsync();
            }
            else
            {
                var list = query.Where(x => x.NhomChaId == 0 || x.NhomChaId == null)
                    .Select(k => new DuocPhamBenhVienPhanNhomGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        NhomChaId = k.NhomChaId,
                        DuocPhamBenhVienPhanNhomChildren = GetChildrenLoadLastFirst(query.ToList(), k.Id, k.CapNhom)
                    });
                return await list.ToListAsync();
            }
        }

        private List<DuocPhamBenhVienPhanNhomGridVo> GetChildrenLoadLastFirst(List<DuocPhamBenhVienPhanNhomGridVo> duocPhamBenhVienPhanNhomList, long id, long capNhom)
        {
            var query = duocPhamBenhVienPhanNhomList.Where(c => c.Id != id && c.CapNhom > capNhom && c.NhomChaId == id)
                .Select(c => new DuocPhamBenhVienPhanNhomGridVo
                {
                    Id = c.Id,
                    Ten = c.Ten,
                    CapNhom = c.CapNhom,
                    NhomChaId = c.NhomChaId,
                    DuocPhamBenhVienPhanNhomChildren = GetChildrenLoadLastFirst(duocPhamBenhVienPhanNhomList, c.Id, c.CapNhom)
                });
            return query.ToList();
        }

        public async Task<bool> IsTenExists(string ten, long id = 0)
        {
            bool result;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
            }
            return !result;
        }

        public List<DuocPhamBenhVienPhanNhomTemplateVo> GetListDuocPhamBenhVienPhanNhomCha(DropDownListRequestModel model, long id)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new DuocPhamBenhVienPhanNhomGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    CapNhom = s.CapNhom,
                    NhomChaId = s.NhomChaId
                });
            var data = query.Where(x => x.NhomChaId == 0 || x.NhomChaId == null)
                .Select(k => new DuocPhamBenhVienPhanNhomGridVo
                {
                    Id = k.Id,
                    Ten = k.Ten,
                    CapNhom = k.CapNhom,
                    NhomChaId = k.NhomChaId,
                    DuocPhamBenhVienPhanNhomChildren = GetChildren(query.ToList(), k.Id, k.CapNhom, model.Id)
                }).ApplyLike(model.Query, g => g.Ten);

            var list = new List<DuocPhamBenhVienPhanNhomTemplateVo>();
            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    list.Add(new DuocPhamBenhVienPhanNhomTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        CapNhom = item.CapNhom,
                        NhomChaId = item.NhomChaId
                    });
                    AddChildToLookup(list, item.DuocPhamBenhVienPhanNhomChildren);
                }
            }
            var allListDuocPhamBvPhanNhom = Task.FromResult(list);
            return FilterListDuocPhamBenhVienPhanNhom(allListDuocPhamBvPhanNhom.Result, id);
        }

        private void AddChildToLookup(List<DuocPhamBenhVienPhanNhomTemplateVo> list,
            List<DuocPhamBenhVienPhanNhomGridVo> trieuChungChildrens)
        {
            if (trieuChungChildrens != null && trieuChungChildrens.Count > 0)
            {
                foreach (var item in trieuChungChildrens)
                {
                    list.Add(new DuocPhamBenhVienPhanNhomTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        CapNhom = item.CapNhom,
                        NhomChaId = item.NhomChaId
                    });
                    AddChildToLookup(list, item.DuocPhamBenhVienPhanNhomChildren);
                }
            }
        }

        private List<DuocPhamBenhVienPhanNhomTemplateVo> FilterListDuocPhamBenhVienPhanNhom
            (List<DuocPhamBenhVienPhanNhomTemplateVo> list, long id)
        {
            if (list.Any(e => e.KeyId == id))
            {
                foreach (var listFilterParent in list.Where(e => e.KeyId == id))
                {
                    listFilterParent.WillRemove = true;
                }
            }
            foreach (var listFilter in list.Where(e => e.NhomChaId == id))
            {
                listFilter.WillRemove = true;
                CheckRemoveChildren(list, listFilter.KeyId);
            }

            return list.Where(w => w.WillRemove == false).ToList();
        }

        private void CheckRemoveChildren(List<DuocPhamBenhVienPhanNhomTemplateVo> list, long id)
        {
            if (list.Any(e => e.NhomChaId == id))
            {
                foreach (var listFilter in list.Where(e => e.NhomChaId == id))
                {
                    listFilter.WillRemove = true;
                    CheckRemoveChildren(list, listFilter.KeyId);
                }
            }
        }

        public static List<DuocPhamBenhVienPhanNhomGridVo> GetChildren(List<DuocPhamBenhVienPhanNhomGridVo> comments, long id, long capNhom, long chaId)
        {
            var query = comments.Where(c => c.Id != id && c.CapNhom > capNhom && c.NhomChaId == id && c.Id != chaId)
                .Select(c => new DuocPhamBenhVienPhanNhomGridVo
                {
                    Id = c.Id,
                    Ten = c.Ten,
                    CapNhom = c.CapNhom,
                    NhomChaId = c.NhomChaId,
                    DuocPhamBenhVienPhanNhomChildren = GetChildren(comments, c.Id, c.CapNhom, chaId)
                });
            return query.ToList();
        }

        public async Task<IEnumerable<DuocPhamBenhVienPhanNhom>> GetDataTreeViewChildren(long id)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new DuocPhamBenhVienPhanNhomGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    CapNhom = s.CapNhom,
                    NhomChaId = s.NhomChaId
                });

            var queryData = await BaseRepository.TableNoTracking.Where(r => r.NhomChaId == id)
                .Select(k => new DuocPhamBenhVienPhanNhomGridVo
                {
                    Id = k.Id,
                    Ten = k.Ten,
                    CapNhom = k.CapNhom,
                    NhomChaId = k.NhomChaId,
                    DuocPhamBenhVienPhanNhomChildren = GetChildrenLoadLastFirst(query.ToList(), k.Id, k.CapNhom)
                }).ToListAsync();
            var list = new List<DuocPhamBenhVienPhanNhom>();
            if (queryData == null || !queryData.Any()) return null;

            foreach (var item in queryData)
            {
                if (item.DuocPhamBenhVienPhanNhomChildren.Any())
                {
                    foreach (var itemChildren in item.DuocPhamBenhVienPhanNhomChildren)
                    {
                        var duocPhamBvPhanNhom = await BaseRepository.GetByIdAsync(itemChildren.Id);
                        list.Add(duocPhamBvPhanNhom);
                    }
                }
                var duocPhamBvPhanNhomItem = await BaseRepository.GetByIdAsync(item.Id);
                list.Add(duocPhamBvPhanNhomItem);
            }
            return list;
        }

        public async Task<int> GetCapNhom(long? id)
        {
            var query = BaseRepository.TableNoTracking.Where(r => r.Id == id.GetValueOrDefault())
                .Select(c => c.CapNhom);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> CheckChiDinhVong(long id, long? nhomDpChaId)
        {
            if (nhomDpChaId == null)
            {
                return true;
            }

            long? nhomChaId;

            do
            {
                nhomChaId = await BaseRepository.TableNoTracking
                    .Where(p => p.Id == nhomDpChaId.GetValueOrDefault())
                    .Select(p => p.NhomChaId)
                    .FirstOrDefaultAsync();

                if (nhomChaId == id)
                {
                    return false;
                }

                nhomDpChaId = nhomChaId.GetValueOrDefault();
            } while (nhomChaId != null);

            return true;
        }

    }
}
