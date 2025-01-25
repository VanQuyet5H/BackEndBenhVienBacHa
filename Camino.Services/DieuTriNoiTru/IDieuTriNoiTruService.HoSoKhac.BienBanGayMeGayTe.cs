using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        string GetTenNhanVienGiaiThich(long nhanVienId);
        string GetTenQuanHeThanNhan(long? quanHeThanNhanId);
        string InBienBanGayMeGayTe(long noiTruHoSoKhacId);
        Task<ThongTinNhanVienDangNhap> ThongTinNhanVienDangNhap();
    }
}
