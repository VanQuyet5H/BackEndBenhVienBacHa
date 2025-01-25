using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoaDuoc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaVatTuDuTruTaiKhoaDuoc
{
    public interface IYeuCauMuaVatTuDuTruTaiKhoaDuocService : IMasterFileService<DuTruMuaVatTuKhoDuoc>
    {
        Task<long> GuiDuTruMuaVatTuTaiKhoaDuoc(DuTruMuaVatTuChiTietGoiViewGridVo duTruMuaVatTuTaiKhoaDuoc);
    }
}
