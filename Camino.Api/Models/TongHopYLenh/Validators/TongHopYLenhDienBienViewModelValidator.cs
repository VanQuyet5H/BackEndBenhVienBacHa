using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;

namespace Camino.Api.Models.TongHopYLenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TongHopYLenhDienBienViewModel>))]
    public class TongHopYLenhDienBienViewModelValidator : AbstractValidator<TongHopYLenhDienBienViewModel>
    {
        public TongHopYLenhDienBienViewModelValidator(IValidator<TongHopYLenhDienBienChiTietViewModel> dienBienChiTietValidator)
        {
            RuleForEach(x => x.TongHopYLenhDienBienChiTiets).SetValidator(dienBienChiTietValidator);
        }
    }
}
