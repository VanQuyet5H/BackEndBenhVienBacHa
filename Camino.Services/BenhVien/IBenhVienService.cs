using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BenhVien
{
    public interface IBenhVienService : IMasterFileService<Core.Domain.Entities.BenhVien.BenhVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        bool CheckMaSoBenhVienExits(string ma, long id);
        bool CheckTenBenhVienExits(string ten, long id,bool isTen = true);
        Task<Core.Domain.Entities.BenhVien.BenhVien> GetBenhVienWithMaBenhVien(string maBenhVien);
        Task<Core.Domain.Entities.BenhVien.BenhVien> GetBenhVienById(long id);

        List<LookupItemVo> GetHangBenhVienDescription(DropDownListRequestModel model);
        List<LookupItemVo> GetTuyenChuyenMonKyThuatDescription(DropDownListRequestModel model);
        Task<bool> CheckLoaiBenhVienAsync(long idLoai);
    }
}
