using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChiDinhNhomGoiDichVuThuongDungViewModel>))]
    public class ChiDinhNhomGoiDichVuThuongDungViewModelValidator : AbstractValidator<ChiDinhNhomGoiDichVuThuongDungViewModel>
    {
        public ChiDinhNhomGoiDichVuThuongDungViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.GoiDichVuIds).NotEmpty()
                .WithMessage(localizationService.GetResource("ChiDihNhomDichVuThuongDung.GoiDichVuIds.Required"));
        }
    }
}
