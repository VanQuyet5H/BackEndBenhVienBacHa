using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.LoaiThuePhongPhauThuats;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LoaiThuePhongPhauThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LoaiThuePhongPhauThuatViewModel>))]
    public class LoaiThuePhongPhauThuatViewModelValidators : AbstractValidator<LoaiThuePhongPhauThuatViewModel>
    {
        public LoaiThuePhongPhauThuatViewModelValidators(ILocalizationService localizationService, ILoaiThuePhongPhauThuatService _loaiThuePhongPhauThuatService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .MustAsync(async (viewModel,input,f) => await _loaiThuePhongPhauThuatService.KiemTraTrungTenAsync(viewModel.Id, input))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));
        }
    }
}
