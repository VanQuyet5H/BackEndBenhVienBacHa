using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.XetNghiems;
using Camino.Core.Infrastructure;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.XetNghiems
{
    [ScopedDependency(ServiceType = typeof(IKetNoiMayXetNghiemService))]
    public class KetNoiMayXetNghiemService : PhienXetNghiemBaseService, IKetNoiMayXetNghiemService
    {
        private readonly LISConfig _lisConfig;
        private readonly ILoggerManager _logger;
        private IRepository<MayXetNghiem> _mayXetNghiemRepository;

        public KetNoiMayXetNghiemService(IRepository<PhienXetNghiem> phienXetNghiemRepository, IUserAgentHelper userAgentHelper, ICauHinhService cauHinhService, ILocalizationService localizationService,
            LISConfig lisConfig, ILoggerManager logger, IRepository<MayXetNghiem> mayXetNghiemRepository) 
            : base(phienXetNghiemRepository, userAgentHelper, cauHinhService, localizationService)
        {
            _lisConfig = lisConfig;
            _logger = logger;
            _mayXetNghiemRepository = mayXetNghiemRepository;
        }

        public async Task<List<MayXetNghiem>> GetDanhSachMayXetNghiem()
        {
            return await _mayXetNghiemRepository.TableNoTracking.Include(x=>x.MauMayXetNghiem).Where(o=>o.HieuLuc).ToListAsync();
        }

        public async Task<List<string>> GetDanhSachChiSoXetNghiem(string barCodeNumber, long mauMayXetNghiemId)
        {
            if (Int32.TryParse(barCodeNumber, out var b))
            {
                var thoiDiemBatDau = DateTime.Now.AddHours((-1) * _lisConfig.PhienXetNghiemSoGioToiDa);
                var phienXetNghiem = await BaseRepository.TableNoTracking.Where(o=>o.ThoiDiemKetLuan == null && o.ThoiDiemBatDau >= thoiDiemBatDau && o.BarCodeNumber == b)
                    .Include(o => o.PhienXetNghiemChiTiets)
                    .ThenInclude(o => o.KetQuaXetNghiemChiTiets).ThenInclude(o => o.DichVuXetNghiem).ThenInclude(o => o.DichVuXetNghiemKetNoiChiSos)
                    .OrderBy(o=>o.Id)
                    .LastOrDefaultAsync();
                if (phienXetNghiem != null)
                {
                    var g = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(o => o.KetQuaXetNghiemChiTiets)
                        .Where(o => o.DaDuyet != true && string.IsNullOrEmpty(o.GiaTriTuMay) && string.IsNullOrEmpty(o.GiaTriNhapTay) && o.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Any(cs => cs.HieuLuc && cs.NotSendOrder != true && cs.MauMayXetNghiemId == mauMayXetNghiemId))
                        .GroupBy(o => o.PhienXetNghiemChiTiet.LanThucHien);
                    if (g.Any())
                    {
                        return g.OrderBy(o => o.Key).Last().Select(o => o.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.First(cs => cs.HieuLuc && cs.NotSendOrder != true && cs.MauMayXetNghiemId == mauMayXetNghiemId).MaChiSo).ToList();
                    }
                }
            }
            return new List<string>();
        }
    }

}
