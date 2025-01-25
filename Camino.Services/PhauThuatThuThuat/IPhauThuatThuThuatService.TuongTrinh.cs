using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        Task<ThongTinKhoaPhongVo> GetThongTinKhoa(long phongBenhVienId, long? ycdvktId);
        Task<ThongTinChiDinhDichVuVo> GetThongTinChiDinhDichVu(long yeuCauDichVuKyThuatId);
        List<LookupItemVo> GetListThoiGianTuVongPTTTTrongNgay(LookupQueryInfo queryInfo);
        List<LookupItemVo> GetListTuVongPTTTTrongNgay(LookupQueryInfo queryInfo);

        Task<List<LookupTrangThaiPtttVo>> GetListPtttBn(LookupQueryInfo queryInfo, long noiThucHienId, long yctnId, bool IsTuongTrinhLai);

        Task<KetQuaPhauThuatThuThuatVo> StartPhauThuat(long idDichVuKyThuat);
        Task<KetQuaPhauThuatThuThuatVo> FinishOperation(long idDichVuKyThuat);

        bool KiemTraThoiGianVoiThoiDiemTiepNhan(DateTime? thoiGian, long yeuCauDichVuKyThuatId);
    }
}
