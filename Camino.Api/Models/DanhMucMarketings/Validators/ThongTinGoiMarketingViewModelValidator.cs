using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DanhMucMarketings.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThongTinGoiMarketingViewModel>))]
    public class ThongTinGoiMarketingViewModelValidator : AbstractValidator<ThongTinGoiMarketingViewModel>
    {
        public ThongTinGoiMarketingViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.HoTen.Required"));

            RuleFor(x => x.NamSinh)
               .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinh.Required"))
               .When(x => x.NgayThangNamSinh != null)
               ;

            RuleFor(x => x.NamSinh)
                .Must((model, s) => model.NamSinh <= DateTime.Now.Year)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinhMoreThanNow.Required"))
                .When(x => x.NamSinh != null)
                ;

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.Required"))
                .When(x => x.NamSinh == null)
                ;

            //RuleFor(x => x.NgayThangNamSinh)
            //    .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinhOrNamSinh.Required"))
            //    .When(x => x.NamSinh == null)
            //    ;


            RuleFor(x => x.NgayThangNamSinh).Must((model, s) => model.NgayThangNamSinh.GetValueOrDefault() <= DateTime.Now.Date)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.DatetimeNow"))
                .When(x => x.NgayThangNamSinh != null);
        }
    }
}
