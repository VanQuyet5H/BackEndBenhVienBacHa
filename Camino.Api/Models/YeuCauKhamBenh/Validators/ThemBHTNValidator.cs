using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Localization;
using FluentValidation;
using System;

namespace Camino.Api.Models.YeuCauKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThemBaoHiemTuNhan>))]
    public class ThemBHTNValidator : AbstractValidator<ThemBaoHiemTuNhan>
    {
        public ThemBHTNValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CongTyBaoHiemTuNhanId)
                .NotEmpty().WithMessage(localizationService.GetResource("themBaoHiemTuNhanModel.CongTyBaoHiemTuNhanId.Required"));
            RuleFor(x => x.NgayHetHan)
                .MustAsync(async (model, input, s) => (model.NgayHieuLuc.GetValueOrDefault() < model.NgayHetHan.GetValueOrDefault()) && (model.NgayHieuLuc != null))
                    .WithMessage(localizationService.GetResource("themBaoHiemTuNhanModel.NgayHetHan.MoreThanTuNgay")).When(p => p.NgayHetHan != null)

                    .MustAsync(async (model, input, s) => (DateTime.Now.Date < model.NgayHetHan.GetValueOrDefault()))
                    .WithMessage(localizationService.GetResource("themBaoHiemTuNhanModel.NgayHetHan.MoreThanDatetimeNow")).When(p => p.NgayHetHan != null);
            //RuleFor(x => x.MaSoThe)
            //    .NotEmpty().WithMessage(localizationService.GetResource("themBaoHiemTuNhanModel.MaSoThe.Required"));

        }
    }
}