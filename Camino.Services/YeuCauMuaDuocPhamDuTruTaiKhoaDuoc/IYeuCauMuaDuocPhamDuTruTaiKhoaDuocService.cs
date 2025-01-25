using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoaDuoc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuocPhamDuTruTaiKhoaDuoc
{
    public interface IYeuCauMuaDuocPhamDuTruTaiKhoaDuocService :IMasterFileService<DuTruMuaDuocPhamKhoDuoc>
    {
        Task<long> GuiDuTruMuaDuocPhamTaiKhoaDuoc(DuTruMuaDuocPhamChiTietGoiViewGridVo duTruMuaDuocPhamTaiKhoaDuoc);
    }
}
