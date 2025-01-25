using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GridDataSource GetDataForGridDanhSachPhaThuocTruyen(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachPhaThuocTruyen(QueryInfo queryInfo);
        Task ThemPhaThuocTruyen(PhaThuocTruyenBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
    }
}
