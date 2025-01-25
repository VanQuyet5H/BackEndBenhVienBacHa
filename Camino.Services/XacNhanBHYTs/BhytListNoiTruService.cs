using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(IBhytListNoiTruService))]
    public class BhytListNoiTruService : YeuCauTiepNhanBaseService, IBhytListNoiTruService
    {

        public BhytListNoiTruService(IRepository<YeuCauTiepNhan> repository, IUserAgentHelper userAgentHelper, ILocalizationService localizationService, ICauHinhService cauHinhService, ITaiKhoanBenhNhanService taiKhoanBenhNhanService)
            : base(repository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        { }

        public Task<GridDataSource> GetDataForXacNhanBhytNoiTruAsync(QueryInfo queryInfo)
        {
            throw new System.NotImplementedException();
        }

        public Task<GridDataSource> GetTotalPageForXacNhanBhytNoiTruAsync(QueryInfo queryInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}
