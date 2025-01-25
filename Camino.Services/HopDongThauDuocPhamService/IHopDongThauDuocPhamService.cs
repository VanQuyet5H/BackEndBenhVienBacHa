using System;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Core.Domain.ValueObject.Thuoc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.HopDongThauDuocPhamService
{
    public interface IHopDongThauDuocPhamService
        : IMasterFileService<HopDongThauDuocPham>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? hopDongThauId = null, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        Task<List<DuocPhamTemplateVo>> GetListDuocPham(DropDownListRequestModel model);

        Task<List<NhaThauTemplateVo>> GetListNhaThau(DropDownListRequestModel model);

        List<LookupItemVo> GetListLoaiThau(LookupQueryInfo queryInfo);

        List<LookupItemVo> GetListLoaiThuocThau(LookupQueryInfo queryInfo);

        Task<List<LookupItemVo>> GetListTenHopDongThau(DropDownListRequestModel model);

        Task<bool> IsNhaThauExists(long nhaThauId = 0, long id = 0);

        Task<bool> IsSoHopDongExists(string soHopDong = null, long id = 0);

        Task<bool> IsSoQuyetDinhExists(string soQuyetDinh = null, long id = 0);

        Task<bool> GetHieuLucDuocPham(long id);

        Task<List<long>> GetHopDongThauChiTiet(long hopDongThauDuocPhamId, long duocPhamId);

        Task<bool> KiemTraHieuLucHopDongThau(long hopDongThauId);

        Task<bool> KiemTraHetHieuLucHopDongThau(long[] arrHopDongThauId);

        Task<bool> KiemTraConDuocPham(long id);
        Task<bool> CheckDuocPhamBenhVienExist(long duocPhamId);
        Task<bool> IsMaDuocPhamBVExists(string maDuocPham, long id = 0);
    }
}
