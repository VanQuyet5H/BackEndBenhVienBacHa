using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DoiTuongUuDais.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel>))]
    public class DoiTuongUuDaiDichVuKhamBenhBenhVienViewModelValidator : AbstractValidator<DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel>
    {
        public DoiTuongUuDaiDichVuKhamBenhBenhVienViewModelValidator(ILocalizationService localizationService, IValidator<DoiTuongUuDaiViewModel> doituongUuDaiValidator)
        {

            RuleFor(x => x.ListDichVuKhamBenhBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel.DichVuKhamBenhBenhVienId.Required"));
            RuleForEach(x => x.DoiTuongUuDai).SetValidator(doituongUuDaiValidator);
        }
    }
}
