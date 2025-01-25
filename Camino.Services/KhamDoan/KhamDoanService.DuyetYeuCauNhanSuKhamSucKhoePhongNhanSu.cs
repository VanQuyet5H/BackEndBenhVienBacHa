using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public async Task DuyetPhongNhanSuAsync(YeuCauDuyetPhongNhanSu ycDuyetPhongNhanSu)
        {
            var ycNhanSuKhamSucKhoeEntity = await _yeuCauNhanSuKhamSucKhoeRepository.GetByIdAsync(ycDuyetPhongNhanSu.Id,
                w => w.Include(q => q.YeuCauNhanSuKhamSucKhoeChiTiets));
            ycNhanSuKhamSucKhoeEntity.DuocNhanSuDuyet = true;
            ycNhanSuKhamSucKhoeEntity.NgayNhanSuDuyet = DateTime.Now;
            ycNhanSuKhamSucKhoeEntity.NhanVienNhanSuDuyetId = _userAgentHelper.GetCurrentUserId();
            foreach (var nhanSu in ycDuyetPhongNhanSu.NhanSus)
            {
                if (ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.Any(q => q.Id == nhanSu.Id))
                {
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.First(q => q.Id == nhanSu.Id).DonVi =
                        nhanSu.DonVi;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.First(q => q.Id == nhanSu.Id).SoDienThoai =
                        nhanSu.SoDienThoai;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.First(q => q.Id == nhanSu.Id).GhiChu =
                        nhanSu.GhiChu;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.First(q => q.Id == nhanSu.Id).HoTen =
                        nhanSu.HoTen;
                    ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.First(q => q.Id == nhanSu.Id).NguoiGioiThieuId =
                        nhanSu.NguoiGioiThieuId;
                }
            }

            
            await _yeuCauNhanSuKhamSucKhoeRepository.UpdateAsync(ycNhanSuKhamSucKhoeEntity);
        }

        public async Task TuChoiDuyetPhongNhanSuAsync(TuChoiDuyetKhthRequest tuChoiDuyet)
        {
            var ycNhanSuKhamSucKhoeEntity = await _yeuCauNhanSuKhamSucKhoeRepository.GetByIdAsync(tuChoiDuyet.Id);
            ycNhanSuKhamSucKhoeEntity.DuocNhanSuDuyet = false;
            ycNhanSuKhamSucKhoeEntity.NgayNhanSuDuyet = DateTime.Now;
            ycNhanSuKhamSucKhoeEntity.NhanVienNhanSuDuyetId = _userAgentHelper.GetCurrentUserId();
            ycNhanSuKhamSucKhoeEntity.LyDoNhanSuKhongDuyet = tuChoiDuyet.LyDo;
            await _yeuCauNhanSuKhamSucKhoeRepository.UpdateAsync(ycNhanSuKhamSucKhoeEntity);
        }
    }
}
