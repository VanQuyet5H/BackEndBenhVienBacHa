using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaSinhHieus;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IYeuCauDichVuKyThuatTuongTrinhPTTTService))]
    public class YeuCauDichVuKyThuatTuongTrinhPTTTService : MasterFileService<YeuCauDichVuKyThuatTuongTrinhPTTT>, IYeuCauDichVuKyThuatTuongTrinhPTTTService
    {
        private readonly IRepository<KetQuaSinhHieu> _ketQuaSinhHieuRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        public YeuCauDichVuKyThuatTuongTrinhPTTTService(
            IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> repository,
            IRepository<KetQuaSinhHieu> ketQuaSinhHieuRepository,
            IUserAgentHelper userAgentHelper
        ) :
            base(repository)
        {
            _ketQuaSinhHieuRepository = ketQuaSinhHieuRepository;
            _userAgentHelper = userAgentHelper;
        }

        public async Task<TuongTrinhTuVongResultVo> GetTuVong(long idDvkt)
        {
            var tuVong = await BaseRepository.TableNoTracking
                .Where(e => e.Id == idDvkt)
                .Select(w => new TuongTrinhTuVongResultVo
                {
                    TgTuVong = w.KhoangThoiGianTuVong,
                    TuVong = w.TuVongTrongPTTT
                }).FirstOrDefaultAsync();

            return tuVong;
        }

        public async Task KetThucTuongTrinh(long ycdvktId)
        {
            var tuongTrinh = await BaseRepository.TableNoTracking.Include(o=>o.YeuCauDichVuKyThuat)
                .FirstOrDefaultAsync(w => w.Id == ycdvktId);
            if (tuongTrinh.ThoiDiemPhauThuat == null)
            {
                tuongTrinh.ThoiDiemPhauThuat = DateTime.Now;
            }
            tuongTrinh.ThoiDiemKetThucTuongTrinh = DateTime.Now;

            var currentUser = _userAgentHelper.GetCurrentUserId();
            tuongTrinh.YeuCauDichVuKyThuat.ThoiDiemThucHien = tuongTrinh.ThoiDiemPhauThuat;
            tuongTrinh.YeuCauDichVuKyThuat.NhanVienThucHienId = currentUser;
            await BaseRepository.UpdateAsync(tuongTrinh);
        }

        public async Task<GridDataSource> LoadChiSoSinhHieu(long yctnId)
        {
            var chiSoSinhHieuList = await _ketQuaSinhHieuRepository.TableNoTracking
                .Where(e => e.YeuCauTiepNhanId == yctnId)
                .Select(q => new KetQuaSinhHieuPtttVo
                {
                    Id = q.Id,
                    NgayThucHien = q.LastTime,
                    Bmi = q.Bmi,
                    CanNang = q.CanNang,
                    NhipTho = q.NhipTho,
                    NhipTim = q.NhipTim,
                    ChieuCao = q.ChieuCao,
                    Glassgow = q.Glassgow,
                    HuyetApTamThu = q.HuyetApTamThu,
                    HuyetApTamTruong = q.HuyetApTamTruong,
                    SpO2 = q.SpO2,
                    ThanNhiet = q.ThanNhiet
                }).ToListAsync();
            return new GridDataSource
            {
                Data = chiSoSinhHieuList.ToArray(),
                TotalRowCount = chiSoSinhHieuList.Count
            };
        }
    }
}
