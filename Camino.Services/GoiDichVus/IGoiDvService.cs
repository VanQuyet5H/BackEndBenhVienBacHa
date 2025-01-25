using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.GoiDichVus
{
    public interface IGoiDvService : IMasterFileService<GoiDichVu>
    {
        Task<List<DichVuKyThuatChoBenhVienTemplateVo>> GetListDichVuKyThuat(DropDownListRequestModel model);

        Task<List<DichVuGiuongTemplateVo>> GetListDichVuGiuong(DropDownListRequestModel model);

        Task<long> GetChiPhiHienTaiDichVuKhamBenh(long dichVuKhamBenhBenhVienId, long nhomGiaDichVuKhamBenhBenhVienId);

        Task<long> GetChiPhiHienTaiDichVuKyThuat(long dichVuKyThuatBenhVienId,
            long nhomGiaDichVuKyThuatBenhVienId);

        Task<long> GetChiPhiChoDichVuGiuong(long dichVuGiuongBenhVienId, long nhomGiaId);

        GridDataSource GetDataForGridChiTietAsync(QueryInfo queryInfo);

        GridDataSource GetTotalPageForGridChiTietAsync(QueryInfo queryInfo);

        Task<List<LookupItemVo>> LoaiGiaNhomGiaDichVuKyThuatBenhVien(long? dichVuKyThuatId);
        Task<List<LookupItemVo>> LoaiGiaNhomGiaGiuongBenhVien(long? dichVuGiuongBenhVienId);
        Task<List<LookupItemVo>> GetLoaiGiaDichVuKhamBenh(long? idDichVuKhamBenhId);


        Task<List<LookupItemVo>> LoaiGiaNhomGiaDichVuKyThuatBenhVienGrid(long? dichVuKyThuatId);
        Task<List<LookupItemVo>> LoaiGiaNhomGiaGiuongBenhVienGrid(long? dichVuGiuongBenhVienId);
        Task<List<LookupItemVo>> GetLoaiGiaDichVuKhamBenhGrid(long? idDichVuKhamBenhId);
    }
}
