using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChiDinhDichVuKhamBenhViewModel>))]
    public class ChiDinhDichVuKhamBenhViewModelValidator : AbstractValidator<ChiDinhDichVuKhamBenhViewModel>
    {
        public ChiDinhDichVuKhamBenhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DichVuKhamBenhBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("ChiDinhDichVuKhamBenh.DichVuKhamBenhBenhVienId.Required"));
            RuleFor(x => x.NhomGiaDichVuKhamBenhBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("ChiDinhDichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVienId.Required"));
            RuleFor(x => x.NoiDangKyId)
                .NotEmpty().WithMessage(localizationService.GetResource("ChiDinhDichVuKhamBenh.NoiDangKyId.Required"));
            //RuleFor(x => x.BacSiDangKyId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("ChiDinhDichVuKhamBenh.BacSiDangKyId.Required"));
        }
    }
}
