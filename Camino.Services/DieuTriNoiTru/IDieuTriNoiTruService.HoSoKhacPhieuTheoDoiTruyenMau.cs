using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<List<LookupItemVo>> GetDanhSachMaDonViMau(DropDownListRequestModel queryInfo,long? yeuCauTiepNhanId, long? longMaDVMID);
        PhieuTheoDoiTruyenMauGrid GetThongTinPhieuTheoDoiTruyenMau(long yeuCauTiepNhan);
        PhieuTheoDoiTruyenMauGrid GetThongTinPhieuTheoDoiTruyenMauSoSanhMaDonViMau(long yeuCauTiepNhan, long maDonViMauId);
        ThongTinDefaultPhieuTheoDoiTruyenMauCreate GetThongTinDefaultPhieuTheoDoiTruyenMauCreate(long yeuCauTiepNhanId);
        Task<string> InPhieuTheoDoiTruyenMau(XacNhanInPhieuTheoDoiTruyenMau xacNhanIn);
    }
}
