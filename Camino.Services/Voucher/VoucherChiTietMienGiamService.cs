using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Data;

namespace Camino.Services.Voucher
{
    [ScopedDependency(ServiceType = typeof(IVoucherChiTietMienGiamService))]
    public class VoucherChiTietMienGiamService : MasterFileService<VoucherChiTietMienGiam>, IVoucherChiTietMienGiamService
    {
        public VoucherChiTietMienGiamService(IRepository<VoucherChiTietMienGiam> repository) : base(repository)
        {
        }
    }
}
