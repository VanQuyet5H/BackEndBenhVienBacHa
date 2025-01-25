using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LoaiGiaDichVus;

namespace Camino.Services.LoaiGiaDichVus
{
    public interface ILoaiGiaDichVuService : IMasterFileService<NhomGiaDichVuKhamBenhBenhVien>
    {
        #region Grid
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);


        #endregion

        #region CRUD
        Task<LoaiGiaDichVuGridVo> GetThongTinLoaiGiaAsync(LoaiGiaDichVuInfoVo info);
        Task ThemLoaiGia(LoaiGiaDichVuGridVo info);
        Task CapNhatLoaiGia(LoaiGiaDichVuGridVo info);
        Task XuLyXoaLoaiGiaAsync(long id, Enums.NhomDichVuLoaiGia nhom);

        Task<bool> KiemTraTrungTenTheoNhom(long id, Enums.NhomDichVuLoaiGia? nhom, string ten);

        #endregion
    }
}
