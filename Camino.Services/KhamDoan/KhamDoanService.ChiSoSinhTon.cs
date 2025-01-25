using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaSinhHieus;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public async Task<GridDataSource> GetDataListForChiSoSinhTon(long id)
        {         
            var query = _ketQuaSinhHieuRepository.TableNoTracking.Where(o => o.YeuCauTiepNhanId == id).Select(s => new KetQuaSinhHieuGridVo
            {
                Id = s.Id,
                NhipTim = s.NhipTim,
                NhipTho = s.NhipTho,
                ThanNhiet = s.ThanNhiet,
                HuyetAp = s.HuyetApTamThu != null && s.HuyetApTamTruong != null
                        ? s.HuyetApTamThu + "/" + s.HuyetApTamTruong
                        : null,

                HuyetApTamThu = s.HuyetApTamThu,
                HuyetApTamTruong = s.HuyetApTamTruong,

                ChieuCao = s.ChieuCao,
                CanNang = s.CanNang,
                BMI = s.Bmi,
                SpO2 = s.SpO2,
                NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.GetValueOrDefault().ApplyFormatDateTime() : string.Empty,
                IsUpdate = false,
                IsDelete = false,
                IsSave = s.Id != 0 ? true : false,
                Glassgow = s.Glassgow,

                KSKPhanLoaiTheLuc = (int)s.KSKPhanLoaiTheLuc
            });

            var queryTask = query.ToArrayAsync();

            return new GridDataSource
            {
                Data = await queryTask
            };
        }

        public async Task<HopDongKhamSucKhoeNhanVien> GetByHopDongKhamSucKhoeIdAsync
            (long id, Func<IQueryable<HopDongKhamSucKhoeNhanVien>, IIncludableQueryable<HopDongKhamSucKhoeNhanVien, object>> includes)
        {
            return await _hopDongKhamSucKhoeNhanVienRepository.GetByIdAsync(id, includes);
        }

        public async Task UpdateChiSoSinhTonAsync(HopDongKhamSucKhoeNhanVien entity)
        {
            throw new NotImplementedException();
        }
    }
}
