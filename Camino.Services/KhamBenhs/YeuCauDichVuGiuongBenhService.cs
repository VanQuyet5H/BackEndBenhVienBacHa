using System.Linq;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Data;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IYeuCauDichVuGiuongBenhService))]
    public class YeuCauDichVuGiuongBenhService : MasterFileService<YeuCauDichVuGiuongBenhVien>, IYeuCauDichVuGiuongBenhService
    {
        public YeuCauDichVuGiuongBenhService(IRepository<YeuCauDichVuGiuongBenhVien> repository): base(repository)
        {
        }

        public bool KiemTraLoaiGiaDangSuDung(long dichVuGiuongBenhVienId,long nhomGiaDichVuGiuongBenhVienId)
        {
            return BaseRepository.TableNoTracking.Any(o =>
                o.DichVuGiuongBenhVienId == dichVuGiuongBenhVienId &&
                o.NhomGiaDichVuGiuongBenhVienId == nhomGiaDichVuGiuongBenhVienId);
        }
    }
}
