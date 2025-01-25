using System;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.QuanHeThanNhan.Validators
{
    //[TransientDependency(ServiceType = typeof(IValidator<ThuPhiBenhNhanViewModel>))]
    //public class ThuPhiBenhNhanViewModelValidator : AbstractValidator<ThuPhiBenhNhanViewModel>
    //{
    //    public ThuPhiBenhNhanViewModelValidator(
    //        ILocalizationService localizationService
    //    )
    //    {
    //        RuleFor(x => x.NgayThu)
    //            .NotEmpty().WithMessage(localizationService.GetResource("Common.NgayThu.Required"));

    //        RuleFor(x => x.NoiDungThu)
    //            .NotEmpty().WithMessage(localizationService.GetResource("Common.NoiDung.Required"));
    //    }
    //}
}