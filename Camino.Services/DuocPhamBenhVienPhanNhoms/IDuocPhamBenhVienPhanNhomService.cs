using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuocPhamBenhVienPhanNhoms;

namespace Camino.Services.DuocPhamBenhVienPhanNhoms
{
    public interface IDuocPhamBenhVienPhanNhomService : IMasterFileService<DuocPhamBenhVienPhanNhom>
    {
        Task<List<DuocPhamBenhVienPhanNhomGridVo>> GetDataTreeView(QueryInfo queryInfo);

        Task<bool> IsTenExists(string ten, long id = 0);

        List<DuocPhamBenhVienPhanNhomTemplateVo> GetListDuocPhamBenhVienPhanNhomCha(DropDownListRequestModel model, long id);

        Task<IEnumerable<DuocPhamBenhVienPhanNhom>> GetDataTreeViewChildren(long id);

        Task<int> GetCapNhom(long? id);

        Task<bool> CheckChiDinhVong(long id, long? nhomChaId);
    }
}
