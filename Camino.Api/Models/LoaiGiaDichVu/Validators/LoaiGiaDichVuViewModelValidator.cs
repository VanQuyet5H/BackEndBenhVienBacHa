using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.LoaiGiaDichVus;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LoaiGiaDichVu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LoaiGiaDichVuViewModel>))]
    public class LoaiGiaDichVuViewModelValidator : AbstractValidator<LoaiGiaDichVuViewModel>
    {
        public LoaiGiaDichVuViewModelValidator(ILocalizationService localizationService, ILoaiGiaDichVuService loaiGiaDichVuService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .MustAsync(async (model, input, f) =>
                    !await loaiGiaDichVuService.KiemTraTrungTenTheoNhom(model.Id, model.Nhom, input))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.Nhom)
                .NotEmpty().WithMessage(localizationService.GetResource("LoaiGiaDichVu.Nhom.Required"));
        }
    }
}
