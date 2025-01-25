using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoDuocPhams
{
    public interface IYeuCauNhapKhoDuocPhamService : IMasterFileService<YeuCauNhapKhoDuocPham>
    {
        Task<long> GetKhoTong1Id();

        Task UpdateDuocPhamBenhVienPhanNhom(long id, long  duocPhamBenhVienPhanNhomId);

        Task<long?> SuggestNhomDuocPham(long duocPhamId);
        Task<bool> KiemTraNgayHoaDon(DateTime? ngayHoaDon, DateTime? NgayHienTai);
    }
}
