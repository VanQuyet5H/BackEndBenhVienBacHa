using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TrieuChungs;

namespace Camino.Services.TrieuChung
{
    public interface ITrieuChungService : IMasterFileService<Core.Domain.Entities.TrieuChungs.TrieuChung>
    {
        Task<List<TrieuChungGridVo>> GetDataTreeView(QueryInfo queryInfo);
        Task<bool> IsTenExists(string ten, long id = 0);
        Task<List<LookupItemTemplate>> GetListTrieuChungCha(DropDownListRequestModel model);
        bool KiemTraExists(long id);
        Task<List<LookupItemVo>> GetListDanhMucChuanDoan(DropDownListRequestModel model);
        Task<IEnumerable<LookupItemVo>> GetDataTreeViewChildren(long Id);
        Task<List<Core.Domain.Entities.TrieuChungs.TrieuChung>> FindChildren(long Id);
        Task<List<Core.Domain.Entities.TrieuChungs.TrieuChung>> GetCapNhom(long? Id);
        Task<List<Core.Domain.Entities.TrieuChungs.TrieuChung>> GetNameTrieuChung(long? TrieuChungChaId);
       
        bool IsKiemTra(List<long> ids);
        TrieuChungGridVo GetThongTinCha(long id);
        Task<List<LookupItemTemplate>> GetListTrieuChungCha1(DropDownListRequestModel model);
    }
    
}
