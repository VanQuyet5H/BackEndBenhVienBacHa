using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThucHienTiemKhamSangLocViewModel>))]
    public class ThucHienTiemKhamSangLocViewModelValidator : AbstractValidator<ThucHienTiemKhamSangLocViewModel>
    {
        public ThucHienTiemKhamSangLocViewModelValidator(ILocalizationService localizationService, IValidator<ThucHienTiemVacxinViewModel> thucHienTiemVacxinValidator)
        {
            RuleFor(x => x.NoiTheoDoiSauTiemId).NotEmpty()
                .WithMessage(localizationService.GetResource("ThucHienTiem.NoiTheoDoiSauTiemId.Required"));
            RuleForEach(x => x.YeuCauDichVuKyThuats).SetValidator(thucHienTiemVacxinValidator);
        }
    }
}
