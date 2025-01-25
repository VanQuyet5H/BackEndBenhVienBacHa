using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaSinhHieu;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        Task<GridDataSource> GetDataForGridAsyncLichSuKhamBenh(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsyncLichSuKhamBenh(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncLichSuKhamBenhBHYT(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncLichSuKhamBenhBHYT(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncChiSoSinhHieu(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncDiUngThuoc(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncTienSuBenh(QueryInfo queryInfo);

        Task<List<DanhSachDaKhamKhamBenhGridVo>> GetDataForGridAsyncKhamBenh(long yeuCauKhamBenhId);

        List<LichSuKhamBenhGridVo> GetDataForGridAsyncChiDinDichVuKhac(long yeuCauKhamBenhId,long yeuCauTiepNhanId);
        Task<List<KetLuanGridVo>> GetDataForGridAsyncKetLuan(long yeuCauKhamBenhId);
        GridDataSource GetDataForGridAsyncToaMau(QueryInfo queryInfo);
        GridDataSource GetDataForGridAsyncChildToaMau(QueryInfo queryInfo);
        GridDataSource GetDataForGridAsyncChildToaThuoc(QueryInfo queryInfo);
        List<ToaThuocGridVo> GetDataForGridAsyncChildToaThuocList(long yeuCauKhamBenhId);
        Task<List<DanhSachDaKhamGridVo>> GetDataForGridAsyncKhamBenhRangHamMat(long yeuCauKhamBenhId);
        Task<string> GetTrieuChungLamSang(long trieuChungId);
        Task<string> GetChuanDoanBanDau(long chuanDoanId);
        Task<List<KetLuanGridVo>> GetChuanDoanICDPhu(long yeuCauKhamBenhId);
        Task<List<TrieuChungBenhSu>> GetDataForGridAsyncTrieuChungBenhSu(long ycKhamBenhId);
        ListAll GetDataKhamCoQuanKhacTatCaChuyenKhoaTheoBenhNhan(long yeuCauTiepNhanId);
        Task<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> GetByIdHaveInclude1(long id);
        Task<ThongTinPhauThuat> GetLichSuThongTinTuongTrinh(long yeuCauKhamBenhId);
        Task<List<KhamBenhPhauThuatThuThuatGridVo>> GetLichSuListPhauThuatThuThuat(long yeuCauKhamBenhId);
      
    }
}
