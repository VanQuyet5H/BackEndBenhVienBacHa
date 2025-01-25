using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        GridDataSource GetDataForGridTuVanThuoc(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridTuVanThuoc(QueryInfo queryInfo);
        Task<List<DuocPhamTuVanTemplate>> GetDuocPhamTuVans(DropDownListRequestModel queryInfo);
        GetDuocPhamTonKhoGridVoItem GetDuocPhamTuVanInfoById(ThongTinThuocTuVanVo thongTinThuocVo);
        Task<string> ThemDonThuocTuVanSucKhoe(DonThuocChiTietVo donThuocChiTiet);
        Task<string> CapNhatDonThuocTuVanSucKhoe(DonThuocChiTietVo donThuocChiTiet);
        Task<bool> KiemTraThuocTuVanSucKhoe(long donThuocChiTietId);
        string InTuVanThuoc(InTuVanThuoc inTuVanThuoc);

    }
}
