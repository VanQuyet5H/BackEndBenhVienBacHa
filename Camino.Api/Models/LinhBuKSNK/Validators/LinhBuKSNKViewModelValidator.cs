using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhBuKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhBuKSNKViewModel>))]
    public class LinhBuKSNKViewModelValidator : AbstractValidator<LinhBuKSNKViewModel>
    {
        public LinhBuKSNKViewModelValidator(ILocalizationService localizationService, IValidator<YeuCauKSNKBenhViensViewModel> linhThuongVatTuChiTietValidator)
        {
            RuleFor(x => x.KhoNhapId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhVeKho.Required"));

            RuleFor(x => x.KhoXuatId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhTuKho.Required"));

            RuleForEach(x => x.YeuCauVatTuBenhViens).SetValidator(linhThuongVatTuChiTietValidator);

        }
    }
}
