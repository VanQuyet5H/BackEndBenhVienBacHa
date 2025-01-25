using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhauThuatThuThuat;
using FluentValidation;

namespace Camino.Api.Models.PhauThuatThuThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhauThuatThuThuatTuongTrinhViewModel>))]
    public class PhauThuatThuThuatTuongTrinhViewModelValidator : AbstractValidator<PhauThuatThuThuatTuongTrinhViewModel>
    {
        public PhauThuatThuThuatTuongTrinhViewModelValidator(ILocalizationService localizationService, IPhauThuatThuThuatService phauThuatThuThuatService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.ThoiGianBatDauGayMe)
                .Must((model, input, s) => phauThuatThuThuatService.KiemTraThoiGianVoiThoiDiemTiepNhan(model.ThoiGianBatDauGayMe, model.YeuCauDichVuKyThuatId)).WithMessage(localizationService.GetResource("PTTT.ThoiGianGayMe.LessThan.ThoiDiemTiepNhan"));

            RuleFor(p => p.ThoiGianPt)
                .Must((model, input, s) => model.ThoiGianPt >= model.ThoiGianBatDauGayMe).WithMessage(localizationService.GetResource("PTTT.ThoiGianBatDauPhauThuat.LessThan.ThoiGianGayMe")).When(p => p.ThoiGianBatDauGayMe != null && p.ThoiGianPt != null, ApplyConditionTo.CurrentValidator)
                .Must((model, input, s) => phauThuatThuThuatService.KiemTraThoiGianVoiThoiDiemTiepNhan(model.ThoiGianPt, model.YeuCauDichVuKyThuatId)).WithMessage(localizationService.GetResource("PTTT.ThoiGianBatDauPhauThuat.LessThan.ThoiDiemTiepNhan"));

            RuleFor(p => p.ThoiGianKetThucPt)
                .Must((model, input, s) => model.ThoiGianKetThucPt >= model.ThoiGianBatDauGayMe).WithMessage(localizationService.GetResource("PTTT.ThoiGianKetThucPhauThuat.LessThan.ThoiGianGayMe")).When(p => p.ThoiGianBatDauGayMe != null && p.ThoiGianKetThucPt != null, ApplyConditionTo.CurrentValidator)
                .Must((model, input, s) => model.ThoiGianKetThucPt >= model.ThoiGianPt).WithMessage(localizationService.GetResource("PTTT.ThoiGianKetThucPhauThuat.LessThan.ThoiGianBatDauPhauThuat")).When(p => p.ThoiGianPt != null && p.ThoiGianKetThucPt != null, ApplyConditionTo.CurrentValidator)
                .Must((model, input, s) => phauThuatThuThuatService.KiemTraThoiGianVoiThoiDiemTiepNhan(model.ThoiGianKetThucPt, model.YeuCauDichVuKyThuatId)).WithMessage(localizationService.GetResource("PTTT.ThoiGianKetThucPhauThuat.LessThan.ThoiDiemTiepNhan"));
        }
    }
}