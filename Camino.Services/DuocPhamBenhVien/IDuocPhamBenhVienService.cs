using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.Thuocs;

namespace Camino.Services.DuocPhamBenhVien
{
    public interface IDuocPhamBenhVienService : IMasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo,bool isPrint);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<DuocPhamTemplate>> GetListDuocPham(DropDownListRequestModel model);
        Task<bool> IsIdExists(long id);
        Task<List<NhomDichVuBenhVienPhanNhomTreeViewVo>> GetListDichVuBenhVienPhanNhomAsync(DropDownListRequestModel model);

        Task<List<LookupItemVo>> DuocPhamBenhVienChaPhanNhoms(DropDownListRequestModel queryInfo);
        Task<List<NhomDichVuBenhVienPhanNhomTreeViewVo>> DichVuBenhVienPhanNhomsLv2VaLv3(DropDownListRequestModel model);
        Task<List<LookupItemVo>> PhanLoaiThuocTheoQuanLys(DropDownListRequestModel queryInfo);
        Task<List<DuocPhamBenhVienPhanNhom>> DuocPhamBenhVienPhanNhoms();
        Task<string> GetTenDuocPhamBenhVienPhanNhom(long duocPhamBenhVienPhanNhomId);
        Task<bool> LaDuocPhamBenhVienPhanNhomCon(long duocPhamBenhVienPhanNhomId);
        List<LookupItemVo> GetAllMayXetNghiem(DropDownListRequestModel queryInfo);

        #region //BVHD-345
        Task<string> GetMaTaoMoiDuocPhamAsync(MaDuocPhamTaoMoiInfoVo model);
        Task<bool> KiemTraTrungDuocPhamBenhVienAsync(DuocPham duocPham);
        Task<bool> KiemTraTrungMaDuocPhamBenhVienAsync(long duocPhamBenhVienId, string maDuocPham, List<string> maDuocPhamTemps = null);

        void XuLyCapNhatMaDuocPhamBenhVien();
        #endregion

        #region BVHD-3911
        bool KiemTraNhomDuocPhamBatBuocNhapSoDangKy(string soDangKy, long? duocPhamBenhVienPhanNhomId);

        #endregion
    }
}
