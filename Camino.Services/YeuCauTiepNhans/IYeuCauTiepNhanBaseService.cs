using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public interface IYeuCauTiepNhanBaseService : IMasterFileService<YeuCauTiepNhan>
    {
        Task PrepareForAddDichVuAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan);
        Task PrepareForEditDichVuAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan);
        Task PrepareForDeleteDichVuAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan, bool isSaveChange = true);
        Task PrepareDichVuAndAddAsync(YeuCauTiepNhan yeuCauTiepNhan);
        //Task BaoLanhDichVuAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan);
        Task PrepareForEditYeuCauTiepNhanAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan);
        //Task PrepareThuNganHuyThanhToanAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan, bool huyDvBHYT, bool huyKham,
        //    int indexYeuCauKhamHuy, decimal soDuTk);
        //Task YeuCauNhapVienTuKhamNgoaiTruAsync(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKhamBenh);

        Task<bool> KiemTraVaLuuYeuCauNhapVienTuKhamNgoaiTruAsync(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKhamBenh);

        Task XuLyHuyTatCaDichVuTruocKhiChuyenKhamAsync(YeuCauTiepNhan yeuCauTiepNhan, long yeuCauKhamBenhId);
        Task<decimal> GetSoTienDaTamUngAsync(long yeuCauTiepNhanId);
        Task CapNhatThongTinHanhChinhVaoNoiTru(YeuCauTiepNhan yeuCauTiepNhanUpdate, string theBHYTCuTrongYCTN);
        void CapNhatThongTinHanhChinhVaoNgoaiTru(long yeuCauTiepNhanId);
    }
}
