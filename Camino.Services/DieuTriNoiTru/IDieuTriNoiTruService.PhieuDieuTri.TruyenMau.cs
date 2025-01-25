using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<GridDataSource> GetDataForGridAsyncTruyenMau(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncTruyenMau(QueryInfo queryInfo);
        void ApDungThoiGianDienBienTruyenMau(List<long> yeuCauTruyenMauIds, DateTime? thoiGianDienBien);
        Task<List<MauVaChePhamTemplateVo>> GetMauVaChePham(DropDownListRequestModel model);
        Task<List<NhomMauTemplateVo>> GetNhomMauRH(DropDownListRequestModel model);
        Task ThemYeuCauTruyenMau(PhieuDieuTriTruyenMauVo truyenMauVo, YeuCauTiepNhan yeuCauTiepNhan);
        Task CapNhatYeuCauTruyenMau(PhieuDieuTriTruyenMauVo truyenMauVo, YeuCauTiepNhan yeuCauTiepNhan);
        Task XoaYeuCauTruyenMau(long ycTruyenMauId, YeuCauTiepNhan yeuCauTiepNhan);
        Task<string> InPhieuTruyenMau(XacNhanInPhieuTruyenMau xacNhanIn);
    }
}
