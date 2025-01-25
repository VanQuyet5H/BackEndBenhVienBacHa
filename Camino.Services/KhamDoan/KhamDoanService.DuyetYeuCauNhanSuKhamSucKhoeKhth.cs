using System;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public async Task TuChoiDuyetKhthAsync(TuChoiDuyetKhthRequest tuChoiDuyet)
        {
            var ycNhanSuKhamSucKhoeEntity = await _yeuCauNhanSuKhamSucKhoeRepository.GetByIdAsync(tuChoiDuyet.Id);
            ycNhanSuKhamSucKhoeEntity.DuocKHTHDuyet = false;
            ycNhanSuKhamSucKhoeEntity.NgayKHTHDuyet = DateTime.Now;
            ycNhanSuKhamSucKhoeEntity.NhanVienKHTHDuyetId = _userAgentHelper.GetCurrentUserId();
            ycNhanSuKhamSucKhoeEntity.LyDoKHTHKhongDuyet = tuChoiDuyet.LyDo;
            await _yeuCauNhanSuKhamSucKhoeRepository.UpdateAsync(ycNhanSuKhamSucKhoeEntity);
        }

        public async Task DuyetKhthAsync(long id)
        {
            var ycNhanSuKhamSucKhoeEntity = await _yeuCauNhanSuKhamSucKhoeRepository.GetByIdAsync(id);
            ycNhanSuKhamSucKhoeEntity.DuocKHTHDuyet = true;
            ycNhanSuKhamSucKhoeEntity.NgayKHTHDuyet = DateTime.Now;
            ycNhanSuKhamSucKhoeEntity.NhanVienKHTHDuyetId = _userAgentHelper.GetCurrentUserId();
            await _yeuCauNhanSuKhamSucKhoeRepository.UpdateAsync(ycNhanSuKhamSucKhoeEntity);
        }
    }
}
