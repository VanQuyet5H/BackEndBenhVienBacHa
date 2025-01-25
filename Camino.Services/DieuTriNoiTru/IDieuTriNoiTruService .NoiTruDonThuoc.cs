using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GridDataSource GetDataForGridDanhSachNoiTruDonThuoc(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachNoiTruDonThuoc(QueryInfo queryInfo);
        Task<string> ThemNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietVo donThuocChiTiet);
        Task<string> CapNhatNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietVo donThuocChiTiet);
        Task<string> XoaNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietVo donThuocChiTiet);
        Task<string> XuLySoThuTuNoiTruDonThuocRaVien(long yeuCauTiepNhanId);
        GetDuocPhamTonKhoGridVoItem GetNoiTruDuocPhamInfoById(ThongTinThuocNoiTruVo thongTinThuocVo);
        string InDonThuocRaVien(InToaThuocRaVien inToaThuoc);
    }
}
