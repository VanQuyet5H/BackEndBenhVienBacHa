using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.BaoCao.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BaoCaoKetQuaKhamChuaBenhViewModel>))]
    public class BaoCaoKetQuaKhamChuaBenhViewModelValidator : AbstractValidator<BaoCaoKetQuaKhamChuaBenhViewModel>
    {
        public BaoCaoKetQuaKhamChuaBenhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TuNgay)
                .NotEmpty().WithMessage("BaoCaoKetQuaKhamChuaBenh.TuNgay.Required");

            RuleFor(x => x.DenNgay)
                .NotEmpty().WithMessage("BaoCaoKetQuaKhamChuaBenh.DenNgay.Required")
                .Must((model,d) => model.TuNgay == null || d == null || ((d.Value - model.TuNgay.Value).TotalDays <= 31))
                    .WithMessage("BaoCaoKetQuaKhamChuaBenh.DenNgay.Range");
        }
    }
}
