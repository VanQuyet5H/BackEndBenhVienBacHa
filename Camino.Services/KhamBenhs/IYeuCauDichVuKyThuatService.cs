using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhuongPhapVoCams;
using Camino.Core.Domain.ValueObject.KetQuaCLS;
using Camino.Core.Domain.ValueObject.PhieuInXetNghiem;

namespace Camino.Services.KhamBenhs
{
    public interface IYeuCauDichVuKyThuatService : IMasterFileService<YeuCauDichVuKyThuat>
    {
        #region Kết quả cận lâm sàng 25/05/2021

        GridDataSource GetDataKetQuaCDHATDCN(QueryInfo queryInfo);
        GridDataSource GetTotalKetQuaCDHATDCN(QueryInfo queryInfo);
        GridDataSource GetDataKetQuaXetNghiem(long yeuCauTiepNhanId);

        #endregion


        Task<GridDataSource> GetDataForGridAsyncKetQuaCLS(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsyncKetQuaCLS(QueryInfo queryInfo);

        Task<ThongTinPhauThuat> GetThongTinTuongTrinh(long phongBenhVienId, long yeuCauKhamBenhId);

        Task<List<KhamBenhPhauThuatThuThuatGridVo>> GetListPhauThuatThuThuat(long phongBenhVienId, long nhanVienId, long yeuCauKhamBenhId);

        Task<bool> WillShowTabPhauThuat(long phongBenhVienId, long nhanVienId, long yeuCauKhamBenhId);

        Task<List<ICDTemplateVo>> GetListICD(DropDownListRequestModel model);

        Task<List<PhuongPhapPTTTTemplateVo>> GetListPhuongPhapPTTT(DropDownListRequestModel model);

        Task<List<string>> GetListPhuongPhapPtttAutoComplete(DropDownListRequestModel model);

        Task<LoaiPhauThuatThuThuatResultVo> GetLoaiPtttDisplay(string ma);

        Task<List<PhuongPhapVoCamTemplateVo>> GetListPhuongPhapVoCam(DropDownListRequestModel model);

        List<LookupItemVo> GetTinhHinhPttt(LookupQueryInfo queryInfo);

        List<LookupItemVo> GetLoaiPttt(LookupQueryInfo queryInfo);

        List<LookupItemVo> GetListBoPhanCoThe(LookupQueryInfo queryInfo);

        List<LookupItemVo> GetTaiBienPttt(LookupQueryInfo queryInfo);

        string GetHinhPhauThuatDuaTrenBoPhan(string boPhan);

        List<LookupItemVo> GetTuVongPttt(LookupQueryInfo queryInfo);

        Task<string> ModifyTenPhauThuatThuThuatEntity(string maPhuongPhapPTTT);

        Task<SavePhauThuatThuThuatResultVo> UpdateForThisPhauThuat(List<YeuCauDichVuKyThuatTuongTrinhPTTT> yeuCauDichVuKyThuatTuongTrinhPttt, long yeuCauKhamBenhId);

        Task<List<LuocDoPhauThuatThuThuatResultVo>> GetListLuocDoPhauThuat(
            ListDichVuKyThuatParameterGridVo listIdDichVuKyThuatModel);
        Task<bool> IsExitsDVKTPTTT(long yeuCauKhamBenhId);

        // update 17/8/2020
        Task<GridDataSource> GetDataForGridAsyncKetQuaCLSDetail(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsyncKetQuaCLSDeTail(QueryInfo queryInfo);

        Task<List<HuyTuongTrinhVo>> GetListTuongTrinhHuy(long? noiThucHienId, long? yctnId);

        bool GoiYeuCauChayLaiXetNghiem(KetQuaGoiLaiXetNghiem modelGoiYCChayLaiXetNghhiem);
        bool HuyYeuCauChayLaiXetNghiem(long phienXetNghiemId);
        Task<List<PhieuInXetNghiemModel>> InDuyetKetQuaXetNghiem(PhieuInXetNghiemVo ketQuaXetNghiemPhieuIn);
        List<LichSuYeuCauChayLai> LichSuYeuCauChayLai(long yeuCauTiepNhanId);
        bool GoiPhienXetNghiemLai(long phienXetNghiemId);
    }
}
