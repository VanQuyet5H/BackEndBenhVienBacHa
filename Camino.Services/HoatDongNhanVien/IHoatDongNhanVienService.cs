using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.HeThong;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.HoatDongNhanVien
{
    public interface IHoatDongNhanVienService : IMasterFileService<Core.Domain.Entities.NhanViens.HoatDongNhanVien>
    {
        Core.Domain.Entities.NhanViens.HoatDongNhanVien GetHoatDongNhanVienByNhanVien(long nhanVienId);
        Core.Domain.Entities.NhanViens.HoatDongNhanVien IsNhanVienHoatDongExists(long currentUserId, long phongKhamId);
        Task<Core.Domain.Entities.NhanViens.HoatDongNhanVien> LuuHoatDongNhanVienAsync(long currentUserId,long phongKhamId);
        Task<LookupItemVo> GetPhongBenhVienByCurrentUser();
        Task<LookupItemVo> GetPhongChinhLamViec();
    }
}
