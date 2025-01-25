using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauHoanTraVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauHoanTraVatTuViewModel>))]
    public class YeuCauHoanTraVatTuViewModelValidator : AbstractValidator<YeuCauHoanTraVatTuViewModel>
    {
        public YeuCauHoanTraVatTuViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.KhoXuatId.Required"));
            RuleFor(x => x.KhoNhapId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.KhoNhapId.Required"));
            RuleFor(x => x.NhanVienYeuCauId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.NhanVienYeuCauId.Required"));
            RuleFor(x => x.NgayYeuCau)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.NgayYeuCau.Required"));
        }
    }
}
