using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DoiTuongUuDais.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DoiTuongUuDaiDichVuKyThuatAddViewModel>))]
    public class DoiTuongUuDaiDichVuKyThuatAddViewModelValidator : AbstractValidator<DoiTuongUuDaiDichVuKyThuatAddViewModel>
    {
        public DoiTuongUuDaiDichVuKyThuatAddViewModelValidator(ILocalizationService localizationService, IValidator<DoiTuongUuDaiViewModel> doituongUuDaiValidator)
        {
            
            RuleFor(x => x.ListDichVuKyThuatBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("DoiTuongUuDaiDichVuKyThuatAdd.DichVuKyThuatBenhVienId.Required"));
            RuleForEach(x => x.DoiTuongUuDai).SetValidator(doituongUuDaiValidator);
        }
    }
}
