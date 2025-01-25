using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoVatTuNhomKSNK
{
    public partial interface INhapKhoVatTuService
    {
        Task<ThongTinDuyetKhoVatTu> GetThongTinDuyetKhoVatTu(long yeuCauNhapKhoVatTuId);
        Task<bool> TuChoiDuyetVatTuNhapKho(ThongTinLyDoHuyNhapKhoVatTu thongTinLyDoHuyNhapKhoVatTu);
        Task<string> DuyetVatTuNhapKho(long id);
        Task<YeuCauNhapKhoVatTu> GetYeuCauNhapKhoVatTu(long id);
        Task<GridDataSource> GetDanhSachDuyetKhoVatTuForGridAsync(QueryInfo queryInfo,bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachDuyetKhoVatTuForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachDuyetKhoVatTuChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachDuyetKhoVatTuChiTietForGridAsync(QueryInfo queryInfo);
    }
}