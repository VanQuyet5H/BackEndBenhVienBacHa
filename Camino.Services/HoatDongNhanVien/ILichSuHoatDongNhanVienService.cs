using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Services.HoatDongNhanVien
{
    public interface ILichSuHoatDongNhanVienService : IMasterFileService<Core.Domain.Entities.NhanViens.LichSuHoatDongNhanVien>
    {
        LichSuHoatDongNhanVien GetLSHoatDongNhanVienByNhanVien(int nhanVienId);
    }
}
