using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiTruBenhAnThongTinHanhChinhViewModel>))]
    public class NoiTruBenhAnThongTinHanhChinhViewModelValidator : AbstractValidator<NoiTruBenhAnThongTinHanhChinhViewModel>
    {
        public NoiTruBenhAnThongTinHanhChinhViewModelValidator(ILocalizationService localizationService, IValidator<NoiTruBenhAnViewModel> noiTruBenhAnValidator)
        {
            RuleFor(x => x.NoiTruBenhAn).SetValidator(noiTruBenhAnValidator);
        }
    }
}
