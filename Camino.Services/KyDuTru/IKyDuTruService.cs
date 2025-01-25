using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Threading.Tasks;

namespace Camino.Services.KyDuTru
{
    public interface IKyDuTruService : IMasterFileService<KyDuTruMuaDuocPhamVatTu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        bool KiemTraTuNgayDaTonTai(DateTime tuNgay, long kyDuTruId);
        bool KiemTraDenNgayDaTonTai(DateTime denNgay, long kyDuTruId);
        bool KiemTraNgayBatDauLapVoiHienTai(DateTime ngayBatDauLap, long kyDuTruId);
        bool KiemTraNgayKetThucLapVoiHienTai(DateTime ngayKetThucLap, long kyDuTruId);
        Task<bool> IsDaDuocSuDung(long kyDuTruId);
        Task<bool> IsDaDuocSuDungChoDuTruMuaDuocPham(long kyDuTruId);
        Task<bool> IsDaDuocSuDungChoDuTruMuaVatTu(long kyDuTruId);
    }
}