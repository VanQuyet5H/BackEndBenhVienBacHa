using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenDich;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        PhieuTheoDoiTruyenDichGrid GetDataDanhSachTruyenDichForGridAsync(QueryInfo queryInfo);
        List<LookupItemVo> GetDataDanhSachTruyenDichTheoNgayForGridAsync(DropDownListRequestModel queryInfo);
        DuocPhamPhieuDieuTriTheoNgay GetDataBindTruyenDichTheoNgayForGridAsync(QueryInfo queryInfo);
        PhieuTheoDoiTruyenDichGridInfo GetThongTinPhieuTheoDoiTruyenDich(long yeuCauTiepNhan);
        Task<string> InPhieuTheoDoiTruyenDich(XacNhanInPhieuTheoDoiTruyenDich xacNhanIn);
        Task<bool> ValidateSoLuongChangeDichTruyen(long yeuCauTiepNhanId, double soLuong, string ngayThu,long duocPhamId, double batDau);
        Task<bool> ValidatorTotalSlKhongVuotTongBanDau(ValidatetorTruyenDichVo vo);
    }
}
