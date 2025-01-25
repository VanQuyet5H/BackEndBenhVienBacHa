using System.Threading.Tasks;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Services.XacNhanBHYTs
{
    public interface IXacNhanBHYTService : IMasterFileService<YeuCauTiepNhan>
    {
        string GetNoiDKBD(string maDk);

        string GetGiayChuyenVien(long? id);

        string GetGiayMienCungChiTra(long? id);

        Task<GiayMienCungChiTra> GetDocumentChoGiayMienCungChiTra(long id);

        Task<GiayChuyenVien> GetDocumentChoGiayChuyenVien(long id);

        Task<string> PhieuLinhThuocBenhNhanXacNhanBHYT(long baoHiemYteId, string hostingName);
        Task<string> PhieuLinhThuocBenhNhanBHYTTheoYCTN(long yeuCauTiepNhanId, string hostingName);
    }
}