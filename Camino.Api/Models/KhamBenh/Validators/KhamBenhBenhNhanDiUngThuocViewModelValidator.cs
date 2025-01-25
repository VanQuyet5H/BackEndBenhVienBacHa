using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhBenhNhanDiUngThuocViewModel>))]
    public class KhamBenhBenhNhanDiUngThuocViewModelValidator : AbstractValidator<KhamBenhBenhNhanDiUngThuocViewModel>
    {
        public KhamBenhBenhNhanDiUngThuocViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LoaiDiUng)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.LoaiDiUng.Required"));

            RuleFor(x => x.TenDiUng)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.ThuocId.Required"));

            RuleFor(x => x.MucDo)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.MucDo.Required"));

            RuleFor(x => x.BieuHienDiUng)
                .NotEmpty().WithMessage(localizationService.GetResource("DiUngThuoc.BieuHienDiUng.Required"));
        }
    }
}
