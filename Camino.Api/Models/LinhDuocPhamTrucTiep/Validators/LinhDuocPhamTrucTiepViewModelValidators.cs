using Camino.Api.Models.LinhBuDuocPham;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhDuocPhamTrucTiep.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhDuocPhamTrucTiepViewModel>))]
    public class LinhDuocPhamTrucTiepViewModelValidators : AbstractValidator<LinhDuocPhamTrucTiepViewModel>
    {
        public LinhDuocPhamTrucTiepViewModelValidators(ILocalizationService localizationService)
        {
            //RuleFor(x => x.KhoNhapId)
            //   .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhVeKho.Required"));

            RuleFor(x => x.KhoXuatId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhTuKho.Required"));

            //RuleForEach(x => x.YeuCauLinhDuocPhamChiTiets).SetValidator(linhThuongDuocPhamChiTietValidator);

        }
    }
}
