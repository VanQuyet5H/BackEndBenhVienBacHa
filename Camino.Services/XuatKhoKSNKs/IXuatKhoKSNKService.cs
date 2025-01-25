using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
//using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.XuatKhoKSNK;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.XuatKhoKSNKs
{
    public interface IXuatKhoKSNKService : IMasterFileService<XuatKhoVatTu>
    {
        Task<GridDataSource> GetAllDpVtKsnkData(QueryInfo queryInfo);
        //Task<GridDataSource> GetAllVatTuData(QueryInfo queryInfo);
        //Task<GridDataSource> GetAllVatTuTotal(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridVatTuChildAsync(QueryInfo queryInfo, long? XuatKhoVatTuId = null, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridVatTuChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDuocPhamChildAsync(QueryInfo queryInfo, long? XuatKhoDuocPhamId = null, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridDuocPhamChildAsync(QueryInfo queryInfo);

        Task<bool> IsValidateUpdateOrRemove(long id);
        Task DeleteXuatKho(XuatKhoVatTu entity);
        Task<XuatKhoKsnkResultVo> XuatKhoKsnk(ThongTinXuatKhoKsnkVo thongTinXuatKhoKsnkVo);
        Task<XuatKhoVatTuChiTiet> GetVatTu(ThemKSNK model);
        Task<NhapKhoVatTuChiTiet> GetNhapKhoVatTuChiTietById(long id);
        Task<List<DpVtKsnkXuatGridVo>> GetDpVtOnGroup(string tenNhom, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon);
        //Task<List<VatTuXuatGridVo>> GetVatTuOnGroup(Enums.LoaiSuDung? groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon);
        Task<List<LookupItemVo>> GetKhoKSNK(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetKhoKSNKNhap(DropDownListRequestModel model);
        Task<bool> IsKhoExists(long id);
        Task SaveChange();
        Task<double> GetSoLuongTon(long duocPhamId, bool isDatChatLuong, long khoNhapId);
        Task<bool> IsKhoLeOrNhaThuoc(long id);

        Task<string> InPhieuXuatVatTu(long id, string hostingName);
        Task<string> InPhieuXuatDuocPham(long id, string hostingName);
    }
}
