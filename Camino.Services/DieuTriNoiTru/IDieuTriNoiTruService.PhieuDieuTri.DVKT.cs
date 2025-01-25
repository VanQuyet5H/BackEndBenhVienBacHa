using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task XuLyThemYeuCauDichVuKyThuatMultiselectAsync(ChiDinhDichVuKyThuatMultiselectVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet, long? phieuDieuTriId);

        #region BVHD-3575

        Task XuLyTaoYeuCauNgoaiTruTheoNoiTru(YeuCauTiepNhan tiepNhanNoiTru);
        Task<NoiTruPhieuDieuTri> GetNoiTruPhieuDieuTriAsync(long phieuDieuTriId);
        Task<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> GetNoiTruBenhAnAsync(string maTiepNhan);
        #endregion

        #region BVHD-3916
        Task<string> GetGhiChuCanLamSangTheoPhieuDieuTri(long noiTruPhieuDieuTriId);
        Task CapNhatGhiChuCanLamSangAsync(GhiChuCanLamSangVo updateVo);
        #endregion
    }
}
