using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.ToaThuocMau;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<string> GetTenNhanVien(long? nhanVienId);
        Task<ThongTinNhanVienDangNhap> ThongTinNhanVienDangNhapId(long id);
        string InGiayRaVien(long hoSoKhacId);
        string ChanDoan(long idICD);
        Task<List<LookupItemVo>> GetGhiChuGiayRaVien(DropDownListRequestModel queryInfo);
        bool KiemTraGhiChuRaVienTonTai(string value);
        Task<string> GetGhiChu(long id);
        Task<List<NhanVienTemplateVos>> GetGiamDocChuyenMons(DropDownListRequestModel queryInfo);
    }
}
