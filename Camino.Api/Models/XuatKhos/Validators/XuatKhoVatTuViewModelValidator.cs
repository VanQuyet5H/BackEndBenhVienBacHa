using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.XuatKhos.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoVatTuViewModel>))]
    public class XuatKhoVatTuViewModelValidator : AbstractValidator<XuatKhoVatTuViewModel>
    {
        public XuatKhoVatTuViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.KhoXuatId.Required"));

            RuleFor(x => x.KhoNhapId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.KhoNhapId.Required")).When(p => !p.IsXuatKhac);

            RuleFor(x => x.NgayXuat)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.NgayXuat.Required"));

            RuleFor(x => x.NgayXuat)
                 .Must((model, s) => (model.NgayXuat != null && model.NgayXuat < DateTime.Now) || model.NgayXuat == null).WithMessage(localizationService.GetResource("XuatKho.NgayXuat.MoreThanNow"));

            RuleFor(x => x.NguoiXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.NguoiXuatId.Required"));

//            RuleFor(x => x.NguoiNhanId)
//                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.NguoiNhanId.Required"));

            RuleFor(x => x.LyDoXuatKho)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.LyDoXuatKho.Required"));


        }
    }
}
