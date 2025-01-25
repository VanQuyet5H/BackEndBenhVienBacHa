using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GridDataSource GetDataForGridDanhSachTruyenDich(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachTruyenDich(QueryInfo queryInfo);
        GridDataSource GetDataForGridDanhSachTruyenDichKhoTong(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachTruyenDichKhoTong(QueryInfo queryInfo);
        Task ThemThuocTruyenDich(ThuocTruyenDichBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        Task CapNhatThuocTruyenDich(ThuocTruyenDichBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        List<LookupItemVo> GetDonViTocDoTruyen(DropDownListRequestModel queryInfo);
        Task<CoDonThuocKhoLeKhoTong> KiemTraCoDonTruyenDich(long noiTruPhieuDieuTriId);

    }
}
