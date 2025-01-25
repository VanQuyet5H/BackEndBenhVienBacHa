using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.LyDoTiepNhan;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Camino.Services.LyDoTiepNhan
{
    public interface ILyDoTiepNhanService : IMasterFileService<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>
    {
        //Task<List<LyDoTiepNhanGridVo>> GetDataTreeView(QueryInfo queryInfo);
        List<LyDoTiepNhanGridVo> GetDataTreeView(QueryInfo queryInfo); //test tìm kiếm con có theo kèm theo cha. => OK

        Task<List<LookupItemTemplate>> GetListLyDoTiepNhanCha(DropDownListRequestModel model);
        Task<List<LookupItemTemplate>> GetListLyDoTiepNhanChaChinhSua(DropDownListRequestModel model);

        Task<List<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>> FindChildren(long Id);
        Task<List<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>> GetCapNhom(long? Id);
        //Task<List<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>> GetLyDoTiepNhanName(long? LyDoTiepNhanChaId);
        Task<IEnumerable<LookupItemVo>> GetDataTreeViewChildren(long Id);
        Task<string> XoaLyDoTiepNhan(long Id);

        Task<bool> IsTenExists(string ten, long id = 0, long? lyDoTiepNhanChaId = null);
        //Task<bool> IsTenVietTatExists(string tenVT, long id = 0);
    }
}
