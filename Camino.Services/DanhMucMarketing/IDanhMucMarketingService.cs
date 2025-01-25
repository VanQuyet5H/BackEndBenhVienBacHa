using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DanhMucMarketing
{
    public interface IDanhMucMarketingService : IMasterFileService<YeuCauGoiDichVu>
    {
        List<long> GetAllGoiTheoBenhNhan(long benhNhanId);
        string AllBangKeGoiDichVu(long yeuCauGoiDichVuId, string hostingName);
        byte[] KetXuatGoiTheoBenhNhanExcel(long yeuCauGoiDichVuIds);

        Task<ICollection<LookupItemTemplateVo>> GetListGoiMarketing(DropDownListRequestModel queryInfo);
        List<ThongTinGoiMarketingGridVo> AddThongTinGoiMarketing(ChonGoiMarketing ChonGoiMarketing);
        Task<GridDataSource> GetThongGoiMRTBenhNhan(long benhNhanId);

        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? idExcel, bool forExportExcel = false);
        Task<GridDataSource> GetTotalForGridChildAsync(QueryInfo queryInfo);

        Task<bool> IsExistsYeuCauGoiDichVuOfBenhNhan(long benhNhanId);

        Task<List<ChuongTrinhGoiDichVu>> GetListChuongTrinhGoiCurrently();

        Task<GridDataSource> GetDataThongTinGoiDaHoanThanhForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalThongTinGoiDaHoanThanhPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataThongTinGoiForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalThongTinGoiPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataDichVuGoiForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDichVuGoiPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataQuaTangGoiForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalQuaTangGoiPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataCacDichVuTrongGoiForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalCacDichVuTrongGoiPageForGridAsync(QueryInfo queryInfo);

        Task<bool> CheckDichVuValidate(List<long> lstDaChon);

        Task<ChuongTrinhGoiDichVu> GetChuongTrinhGoiDichVu(long id);

        Task<List<QuaTangDaXuat>> XuatQuaTang(long benhNhanId, long chuongTrinhGoiDichVuId);

        Task<string> InPhieuXuat(long id, List<QuaTangDaXuat> quaDaXuat);

        Task<YeuCauTiepNhan> GetYCTNDangThucHienOfBenhNhan(long benhNhanId);

        Task<int> GetSoLuongConLaiOfYeuCauGoiDichVu(long benhNhanId, long ycgdvId, long dichVuId, string tenNhom);

        Task<int> GetSoLuongDichVuTrongGoiDichVu(long benhNhanId, long ycgdvId, long dichVuId, string tenNhom, long yeuCauDichVuId);
    }
}
