using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TaoBenhAnSoSinhKhacKhoaViewModel>))]
    public class TaoBenhAnSoSinhKhacKhoaViewModelValidator : AbstractValidator<TaoBenhAnSoSinhKhacKhoaViewModel>
    {
        public TaoBenhAnSoSinhKhacKhoaViewModelValidator(IValidator<BenhAnSoSinhChiTietViewModel> benhAnSoSinhChiTietValidator)
        {
            RuleFor(x => x.BenhAnSoSinhChiTietViewModel).SetValidator(benhAnSoSinhChiTietValidator);
        }
    }
}
