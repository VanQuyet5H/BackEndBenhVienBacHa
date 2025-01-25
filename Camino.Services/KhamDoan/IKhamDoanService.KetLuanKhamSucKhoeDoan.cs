using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataForGridAsyncDanhSachKetLuanKhamSucKhoe(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachKetLuanKhamSucKhoe(QueryInfo queryInfo);
        YeuCauTiepNhan GetYeuCauTiepNhan(long yeuCauTiepNhanId, long hopDongKhamSucKhoeNhanVienId);
        void SaveBanInKhamDoanTiepNhan(long yeuCauTiepNhanId, string ketQuaKhamSucKhoeData, string ketLuanData);
        List<KetLuanKhamSucKhoeDoanDichVuKhamTemplateGroupVo> KetLuanKhamSucKhoeDoanDichVuKhamVos(YeuCauTiepNhan yeuCauTiepNhan);
        string InSoKSKDinhKy(InSoKSKVaKetQua inSoKSKVaKetQua);
        Task<List<KetQuaMauVo>> GetKetQuaMau(YeuCauTiepNhan yeuCauTiepNhan);

        Task<List<DanhSachDichVuKhamGrid>> GetDataKetQuaKSKDoanEdit(long hopDongKhamSucKhoeId);
        //List<DanhSachDichVuKhamGrid> GetDataKetQuaKSKDoanEditByHopDong(long hopDongKhamSucKhoeId);
        List<DanhSachDichVuKhamGrid> GetDataKetQuaKSKDoanEditByHopDongNew(long hopDongKhamSucKhoeId, List<long> tiepNhanIds);
        //Task<KetQuaTatCaKhoa> GetDataKetQuaKhamTheoKhoa(long id, long type);

        bool CheckDungDichVuCuaBenhNhan(DichVuGridVos vo);
    }
}
