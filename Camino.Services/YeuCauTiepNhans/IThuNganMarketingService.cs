using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public interface IThuNganMarketingService : IMasterFileService<BenhNhan>
    {
        ICollection<LookupItemTemplateVo> GetListGoiMarketingChuyenGoi(DropDownListRequestModel queryInfo , long yeuCauGoiDichVuId);
        List<DichVuTrongGoiMarketingModel> GetThongTinGoiDichVuMoiMuonChuyen(long chonGoiMarketing);
        List<DichVuTrongGoiMarketingModel> GetThongTinGoiDichVuCuaBenhNhan(long yeuCauGoiDichVuId);      

        Task<GridDataSource> GetDanhSachChuaQuyetToanAsync(ThuNganMarketingQueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageDanhSachChuaQuyetToanAsync(ThuNganMarketingQueryInfo queryInfo);

        Task<GridDataSource> GetDanhSachDaQuyetToanAsync(ThuNganMarketingQueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageDanhSachDaQuyetToanAsync(ThuNganMarketingQueryInfo queryInfo);        

        Task<GridDataSource> GetGoiChuaQuyetToanTheoBenhNhanAsync(long benhNhanId);
        Task<GridDataSource> GetGoiDaQuyetToanTheoBenhNhanAsync(long benhNhanId);
        Task<(long, string)> ThuTienGoiDichVu(ThongTinThuTienGoi thongTinThuTienGoi);
        Task<ThongTinPhieuThu> GetThongTinPhieuThu(long phieuThuId, LoaiPhieuThuChiThuNgan loaiPhieuThuChiThuNgan);
        Task HuyPhieuThuGoiDichVu(ThongTinHuyPhieuVo thongTinHuyPhieuVo);
        Task CapnhatNguoiThuHoiPhieuThu(ThongTinHuyPhieuVo thongTinHuyPhieuVo);
        Task<long?> QuyetToanGoiDichVu(ThongTinYeuCauGoiDichVu thongTinYeuCauGoiDichVu);
        Task HuyQuyetToanGoiDichVu(HuyQuyetToanGoi huyQuyetToanGoi);
        string GetHtmlPhieuThuMarketing(long taiKhoanThuId, string hostingName);
        YeuCauTiepNhan YeuCauTiepNhanCuoiBenhNhan(long benhNhanId);
        Task<GridDataSource> GetDanhSachPhieuThuMarketingAsync(ThongTinPhieuMarketingQueryInfo queryInfo);
        Task<ThongTinYeuCauGoiDichVu> GetThongTinGoiDichVu(long yeuCauGoiDichVuId);
        string GetHtmBangKeSuDung(long yeuCauGoiDichVuId, string hostingName);
        string GetHtmlPhieuThuTamUngMarketing(long id, string hostingName);

        Task LuuThongTinMuonChuyenGoiMoi(LuuThongTinChuyenGoiMoi thongTinChuyenGois);
    }
}
