using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoaPhongChuyenKhoas;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject.ChucVu;

namespace Camino.Services.NhanVien
{
    public interface INhanVienService : IMasterFileService<Core.Domain.Entities.NhanViens.NhanVien>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> CheckIsExistPhone(string sdt, long id = 0);
        Task<bool> CheckIsExistChungMinh(string cmt, long id = 0);
        Task<bool> CheckIsExistEmail(string email, long id = 0);
        Task<long[]> GetNhanVienRoles(long nhanVienId);
        Task<List<LookupItemVo>> GetListNhanVien(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListTenNhanVien(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListLookupNhanVienIsBacSi(LookupQueryInfo queryInfo);

        //Thấy hàm GetListLookupNhanVienIsBacSi thua mẹ nó luôn model.Id = khoaphongId, hết đường nói
        Task<List<LookupItemVo>> GetListLookupNhanVienIsBacSiClone(LookupQueryInfo queryInfo);

        Task<List<LookupItemVo>> GetListLookupNhanVienIsYta(LookupQueryInfo queryInfo);

        Task<List<KhoaKhamTemplateVo>> GetListKhoaPhongByHoSoNhanVien(DropDownListRequestModel queryInfo);
        Task<bool> CheckMaChungChiAsync(string ma, long id);
        Task<bool> CheckVanBangChuyenMonAsync(long idVanBang);
        Task<bool> CheckChucDanhAsync(long idChucDanh);
        Task<List<LookupItemVo>> GetListPhongNhanVienByHoSoNhanVien(DropDownListRequestModel model, long nhanVienId, string khoaphongIds);
        List<long> kiemTraPhongThuocKhoa(List<long> phongBenhVienIds, long khoaPhongId);
        List<LookupItemVo> GetTatCaPhongCuaNhanVienLogin(LookupQueryInfo model);
        List<LookupItemVo> GetTatCaKhoLeCuaNhanVienLogin(LookupQueryInfo model);
        Task<GridDataSource> GetListKhoaPhongDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetListKhoTheoPhongDataForGridAsync(QueryInfo queryInfo);
        void kiemTraKhoNhanVien(long nhanVienId);
        List<long> KhoaTheoPhong(List<long> phongBenhVienIds);
        List<long> KhoTheoNhanVien(long NhanVienId);
        Task<List<LookupItemVo>> GetListTenChucDanhNhanVien(DropDownListRequestModel model);

        Task<string> GetNameNhanVienWithNhanVienId(long? id);
        Task<List<LookupItemTemplateVo>> GetListChucVu(DropDownListRequestModel model);
        Task<List<LookupItemTextVo>> GetListMaBacSi(LookupQueryInfo model);
    }
}
