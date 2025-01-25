using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using System.Threading.Tasks;

namespace Camino.Services.KhamBenhs
{
    public interface IYeuCauDuocPhamBenhVienService : IMasterFileService<YeuCauDuocPhamBenhVien>
    {
        Task ThemYeuCauDuocPhamBenhVien(YeuCauDuocPhamBenhVien yeuCauDuocPhamBenhVien);
        Task<string> XoaYeuCauDuocPhamBenhVien(long YeuCauDuocPhamId);
    }
}
