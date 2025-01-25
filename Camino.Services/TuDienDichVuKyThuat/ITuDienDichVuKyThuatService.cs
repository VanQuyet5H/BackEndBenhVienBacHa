using System.Collections.Generic;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TuDienKyThuat;

namespace Camino.Services.DichVuXetNghiem
{
    public interface ITuDienDichVuKyThuatService : IMasterFileService<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat>
    {
        List<TuDienKyThuatGridVo> GetDataTreeView(QueryInfo queryInfo);
        List<TuDienKyThuatGridVo> SearchDichVuKyThuatBenhVien(QueryInfo queryInfo);
        void LuuDichVukyThuatBenhVienMauKetQua(TuDienKyThuatGridVo tuDienKyThuatGridVo);
        TuDienKyThuatGridVo GetDichVuKyThuats(long dichVuKyThuatBenhVienId);
    }
}
