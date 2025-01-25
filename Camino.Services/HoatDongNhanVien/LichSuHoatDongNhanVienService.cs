using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Data;
using System.Linq;

namespace Camino.Services.HoatDongNhanVien
{
    [ScopedDependency(ServiceType = typeof(ILichSuHoatDongNhanVienService))]
    public class LichSuHoatDongNhanVienService : MasterFileService<Core.Domain.Entities.NhanViens.LichSuHoatDongNhanVien>, ILichSuHoatDongNhanVienService
    {
        public LichSuHoatDongNhanVienService(IRepository<Core.Domain.Entities.NhanViens.LichSuHoatDongNhanVien> repository) : base(repository)
        {

        }

        public LichSuHoatDongNhanVien GetLSHoatDongNhanVienByNhanVien(int nhanVienId)
        {
            var lichSuHoatDongNhanVien = BaseRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId && cc.ThoiDiemKetThuc == null).LastOrDefault();
            return lichSuHoatDongNhanVien;
        }
    }
}
