using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;

namespace Camino.Api.Models.TongHopYLenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TongHopYLenhPhieuDieuTriViewModel>))]
    public class TongHopYLenhPhieuDieuTriViewModelValidator : AbstractValidator<TongHopYLenhPhieuDieuTriViewModel>
    {
        public TongHopYLenhPhieuDieuTriViewModelValidator(IValidator<TongHopYLenhDienBienViewModel> dienBienValidator)
        {
            RuleForEach(x => x.TongHopYLenhDienBiens).SetValidator(dienBienValidator);
        }
    }
}
