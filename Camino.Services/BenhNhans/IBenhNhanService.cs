using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Services.BenhNhans
{
    public interface IBenhNhanService : IMasterFileService<BenhNhan>
    {
        Task<List<BenhNhan>> GetBenhNhanByTiepNhanBenhNhanTimKiem(TimKiemBenhNhanGridVo model, TimKiemBenhNhanSearch searchPopup = null);
        Task<BenhNhan> GetBenhNhanByMaBHYT(string maBHYT, long? benhNhanId);
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<ThemBHTNGridVo> ThemBHTN(ThemBHTN model);
        Task<TienSuBenhGridVo> ThemTienSuBenh(ThemTienSuBenh model);
        Task<DiUngThuocGridVo> ThemDiUngThuoc(ThemDiUngThuoc model);
        List<LookupItemVo> GetLoaiTienSuBenhs();
        string TenThuocDiUng(long thuocId);
        string InTheBenhNhanBenhNhan(long benhNhanId, string hostingName);
        Task<bool> CheckCongTyBHTNExists(long? congTyBHTNId, List<long> congTyBHTNIds);
        Task<bool> CheckTienSuBenhExists(Enums.EnumLoaiTienSuBenh? loaiTienSuBenh, string tenBenh, List<BenhNhanTienSuBenhChiTiet> tenBenhs);
        Task<bool> CheckDiUngThuocExists(Enums.LoaiDiUng? loaiDiUng, long? thuocId, string tenDiUng, List<BenhNhanDiUngThuocChiTiet> tenDiUngs);
    }
}