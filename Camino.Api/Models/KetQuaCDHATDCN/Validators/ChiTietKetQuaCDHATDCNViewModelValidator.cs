using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KetQuaCDHATDCN.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChiTietKetQuaCDHATDCNViewModel>))]
    public class ChiTietKetQuaCDHATDCNViewModelValidator : AbstractValidator<ChiTietKetQuaCDHATDCNViewModel>
    {
        public ChiTietKetQuaCDHATDCNViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TenKetQua)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.TenKetQua.Required"));

            RuleFor(x => x.KetQua)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.KetQua.Required"));

            RuleFor(x => x.KetLuan)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.KetLuan.Required"));
        }
    }
}
