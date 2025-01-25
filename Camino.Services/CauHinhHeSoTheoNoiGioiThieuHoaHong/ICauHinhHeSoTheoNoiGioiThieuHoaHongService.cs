using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.CauHinhHeSoTheoNoiGioiThieuHoaHong
{
    public interface ICauHinhHeSoTheoNoiGioiThieuHoaHongService : IMasterFileService<Camino.Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu>
    {
        #region  ds cấu hình hệ số theo nơi giới thiệu/ hoa hồng
        //từ nơi giới thiệu e phải qua 1 hợp đồng
        // mới đến các hệ số
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        #endregion
        Task<List<LookupItemCauHinhHeSoTheoNoiGioiThieuHoaHongVo>> GetNoiGioiThieu(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetNoiGioiThieuHopDong(DropDownListRequestModel model,long? id);
        Task<GridDataSource> GetDataNoiGioiThieuHopDongForGridAsync(NoiGioiThieuHopDongQueryInfo queryInfo);
        Task<List<LookupItemCauHinhHeSoTheoNoiGtHoaHongVo>> GetNoiGioiThieuHopDongAdd(DropDownListRequestModel model, long? id);
        Task<List<LookupItemVo>> GetNoiGioiThieuHopDongAddHoHong(DropDownListRequestModel model, long? id);

        #region 
        Task<NoiGioiThieuHopDongVo> XuLyThemNoiGioiThieuHopDongAsync(NoiGioiThieuHopDong entity);
        Task<NoiGioiThieuHopDongVo> XuLyCapNhatNoiGioiThieuHopDongAsync(NoiGioiThieuHopDong entity);
        Task XuLyDeleteNoiGioiThieuHopDongAsync(NoiGioiThieuHopDong entity);
        Task<NoiGioiThieuHopDong> GetByIdNoiGioiThieuHopDongAsync(long id);
        #endregion

        #region lookup
        Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model);
        Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetDichVuGiuong(DropDownListRequestModel model);
        Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetDichVuKham(DropDownListRequestModel model);
        Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetDuocPham(DropDownListRequestModel model);
        Task<List<DichVuAndThuocAndVTYTTemplateVo>> GetVatTu(DropDownListRequestModel model);
        Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuKham(DropDownListRequestModel model);
        Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuKyThuat(DropDownListRequestModel model);
        Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuGiuong(DropDownListRequestModel model);
        Task GetDonGia(ThongTinGiaNoiGioiThieuVo thongTinDichVu);
        #endregion

        #region CRUD S
        Task XuLyThemCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong.CauHinhHeSoTheoThoiGianHoaHong vo);
        Task XuLyCapNhatCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong.CauHinhHeSoTheoThoiGianHoaHong vo);

        Task XuLyDeleteCauHinhHeSoTheoNoiGioiThieuHoaHongCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(DeleteNoiGioiThieuHopDongVo vo);
        Task XuLyDeleteCauHinhHeSoTheoNoiGioiThieuAsync(DeleteNoiGioiThieuHopDongVo vo);
        Task<CauHinhHeSoTheoThoiGianHoaHong> XuLyGetDataCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(long id,long noiGioiThieuId);
        #endregion

        #region cCRUD cấu hình hoa hồng
        Task XuLyThemCauHinhHoaHongAsync(CauHinhChiTietHoaHong vo);

        Task<CauHinhChiTietHoaHong> XuLyGetDataCauHinhHoaHongAsync(long id, long noiGioiThieuId);
        Task XuLyCapNhatCauHinhHoaHongAsync(CauHinhChiTietHoaHong vo);
        Task XuLyDeleteCauHinhHoaHongAsync(DeleteNoiGioiThieuHopDongVo vo);
        Task XuLyDeleteCauHinhHoaHong(DeleteNoiGioiThieuHopDongVo vo);
        #endregion

        Task<List<ImportNoiGioiThieuDichVu>> ImportDSDichVus(Stream path, long noiGioiThieuHopDongId, long noiGioiThieuId);
        Task<List<ImportNoiGioiThieuDuocPhamVTYT>> ImportNoiGioiThieuDuocPhamVTYTs(Stream path, long noiGioiThieuHopDongId, long noiGioiThieuId);

        Task<List<ImportHoaHongDichVu>> ImportHoaHongDichVus(Stream path, long noiGioiThieuHopDongId, long noiGioiThieuId);
        Task<List<ImportHoaHongDPVTYT>> ImportHoaHongDPVTYTs(Stream path, long noiGioiThieuHopDongId, long noiGioiThieuId);

        long GetNoiGioiThieuHopDongUuTiens(long id);
        string GetNoiGioiThieu(long id);
    }
}
