using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Services.KhamBenhs
{
    public interface IYeuCauDichVuGiuongBenhService : IMasterFileService<YeuCauDichVuGiuongBenhVien>
    {
        bool KiemTraLoaiGiaDangSuDung(long dichVuGiuongBenhVienId, long nhomGiaDichVuGiuongBenhVienId);
    }
}
