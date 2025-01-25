using Camino.Core.Domain.ValueObject;
using System.Collections.Generic;

namespace Camino.Services.KetQuaNhomXetNghiem
{
    public interface IKetQuaNhomXetNghiemService : IMasterFileService<Camino.Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem>
    {
        void DeleteFileNhomXetNghiems(long yeuCauTiepNhanId, long nhomDichVuBenhVienId);
    }
}