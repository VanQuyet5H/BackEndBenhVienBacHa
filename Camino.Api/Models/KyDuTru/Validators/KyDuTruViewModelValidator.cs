using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KyDuTru;
using Camino.Services.Localization;
using FluentValidation;
using System;

namespace Camino.Api.Models.KyDuTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KyDuTruViewModel>))]
    public class KyDuTruViewModelValidator : AbstractValidator<KyDuTruViewModel>
    {
        public KyDuTruViewModelValidator(ILocalizationService localizationService, IKyDuTruService kyDuTruService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.TuNgay)
                .NotNull().WithMessage(localizationService.GetResource("KyDuTru.TuNgay.Required"))
                .Must((model, s) => !kyDuTruService.KiemTraTuNgayDaTonTai(model.TuNgay.GetValueOrDefault(), model.Id)).WithMessage(localizationService.GetResource("KyDuTru.TuNgay.Existed"));

            RuleFor(p => p.DenNgay)
                .NotNull().WithMessage(localizationService.GetResource("KyDuTru.DenNgay.Required"))
                .Must((model, s) => model.DenNgay.Value.Date > model.TuNgay.Value.Date).WithMessage(localizationService.GetResource("KyDuTru.DenNgay.GreaterThan.TuNgay"))
                .Must((model, s) => !kyDuTruService.KiemTraDenNgayDaTonTai(model.TuNgay.GetValueOrDefault(), model.Id)).WithMessage(localizationService.GetResource("KyDuTru.DenNgay.Existed"));

            RuleFor(p => p.MuaDuocPham)
                .Must((model, s) => !(model.MuaVatTu != true && model.MuaDuocPham != true)).WithMessage(localizationService.GetResource("KyDuTru.MuaDuocPham.MuaVatTu.Required"));

            RuleFor(p => p.MoTa)
                .Length(1, 1000).WithMessage(localizationService.GetResource("KyDuTru.MoTa.Range.1000"));

            RuleFor(p => p.NgayBatDauLap)
                .NotNull().WithMessage(localizationService.GetResource("KyDuTru.NgayBatDauLap.Required"))
                .Must((model, s) => kyDuTruService.KiemTraNgayBatDauLapVoiHienTai(model.NgayBatDauLap.Value, model.Id)).WithMessage(localizationService.GetResource("KyDuTru.NgayBatDauLap.GreaterThan.Now"))
                .Must((model, s) => model.NgayBatDauLap.Value.Date <= model.TuNgay.Value.Date).WithMessage(localizationService.GetResource("KyDuTru.NgayBatDauLap.LessOrEqualThan.TuNgay")).When(model => model.TuNgay.HasValue, ApplyConditionTo.CurrentValidator);

            RuleFor(p => p.NgayKetThucLap)
                .NotNull().WithMessage(localizationService.GetResource("KyDuTru.NgayKetThucLap.Required"))
                .Must((model, s) => kyDuTruService.KiemTraNgayKetThucLapVoiHienTai(model.NgayKetThucLap.Value, model.Id)).WithMessage(localizationService.GetResource("KyDuTru.NgayketThucLap.GreaterThan.Now"))
                .Must((model, s) => model.NgayKetThucLap.Value.Date <= model.TuNgay.Value.Date).WithMessage(localizationService.GetResource("KyDuTru.NgayKetThucLap.LessOrEqualThan.TuNgay")).When(model => model.TuNgay.HasValue, ApplyConditionTo.CurrentValidator)
                .Must((model, s) => model.NgayBatDauLap.Value.Date <= model.NgayKetThucLap.Value.Date).WithMessage(localizationService.GetResource("KyDuTru.NgayKetThucLap.GreaterThan.NgayBatDauLap"));
        }
    }
}