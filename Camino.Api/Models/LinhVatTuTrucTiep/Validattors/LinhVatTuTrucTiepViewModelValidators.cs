using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhVatTuTrucTiep.Validattors
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhVatTuTrucTiepViewModel>))]
    public class LinhVatTuTrucTiepViewModelValidators : AbstractValidator<LinhVatTuTrucTiepViewModel>
    {
        public LinhVatTuTrucTiepViewModelValidators(ILocalizationService localizationService)
        {
            //RuleFor(x => x.KhoNhapId)
            //   .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhVeKho.Required"));

            RuleFor(x => x.KhoXuatId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhTuKho.Required"));

            //RuleForEach(x => x.YeuCauLinhDuocPhamChiTiets).SetValidator(linhThuongDuocPhamChiTietValidator);

        }
    }
}
