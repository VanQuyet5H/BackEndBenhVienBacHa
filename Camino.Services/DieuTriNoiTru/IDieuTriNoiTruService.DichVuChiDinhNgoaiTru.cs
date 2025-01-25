using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        #region Get data
        Task<List<KhamBenhGoiDichVuGridVo>> GetDataDefaulDichVuChiDinhNgoaiTru(GridChiDinhDichVuNgoaiTruQueryInfoVo queryInfo);
        Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHThuocChiDinhNgoaiTruAsync(GridChiDinhDichVuNgoaiTruQueryInfoVo queryInfo);
        Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocChiDinhNgoaiTruAsyncVer2(GridChiDinhDichVuNgoaiTruQueryInfoVo queryInfo);

        #region //BVHD-3889
        Task<GridDataSource> GetDataForGridNhomDichVuTheoThoiGianNhapVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNhomDichVuTheoThoiGianNhapVienAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDichVuTheoThoiGianNhapVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDichVuTheoThoiGianNhapVienAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridNhomDichVuCLSTheoThoiGianNhapVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNhomDichVuCLSTheoThoiGianNhapVienAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridKetQuaCDHATheoThoiGianNhapVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridKetQuaCDHATheoThoiGianNhapVienAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridKetQuaXetNghiemTheoThoiGianNhapVienAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridKetQuaXetNghiemTheoThoiGianNhapVienAsync(QueryInfo queryInfo);
        #endregion
        #endregion
    }
}
