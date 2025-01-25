using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.LoaiThuePhongNoiThucHiens;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LoaiThuePhongNoiThucHien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LoaiThuePhongNoiThucHienViewModel>))]
    public class LoaiThuePhongNoiThucHienViewModelValidator : AbstractValidator<LoaiThuePhongNoiThucHienViewModel>
    {
        public LoaiThuePhongNoiThucHienViewModelValidator(ILocalizationService localizationService, ILoaiThuePhongNoiThucHienService _loaiThuePhongNoiThucHienService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .MustAsync(async (viewModel, input, f) => await _loaiThuePhongNoiThucHienService.KiemTraTrungTenAsync(viewModel.Id, input))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));
        }
    }
}
