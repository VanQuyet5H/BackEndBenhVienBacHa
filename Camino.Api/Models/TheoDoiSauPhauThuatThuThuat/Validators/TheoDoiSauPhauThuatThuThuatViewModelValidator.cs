using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhauThuatThuThuat;
using FluentValidation;
using Org.BouncyCastle.Asn1.Nist;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.PhauThuatThuThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TheoDoiSauPhauThuatThuThuatViewModel>))]
    public class TheoDoiSauPhauThuatThuThuatViewModelValidator : AbstractValidator<TheoDoiSauPhauThuatThuThuatViewModel>
    {
        public TheoDoiSauPhauThuatThuThuatViewModelValidator(ILocalizationService localizationService, ITheoDoiSauPhauThuatThuThuatService theoDoiSauPhauThuatThuThuatService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.BacSiPhuTrachTheoDoiId)
                .NotNull().WithMessage(localizationService.GetResource("PTTT.TheoDoiSauPhauThuatThuThuat.BacSiPhuTrachTheoDoi.Required"))
                .Must((model, s) => model.BacSiPhuTrachTheoDoiId != 0).WithMessage(localizationService.GetResource("PTTT.TheoDoiSauPhauThuatThuThuat.BacSiPhuTrachTheoDoi.Required"))
                .When(model => model.TrangThaiPhauThuatThuThuat == EnumTrangThaiPhauThuatThuThuat.TheoDoi, ApplyConditionTo.AllValidators);

            RuleFor(x => x.DieuDuongPhuTrachTheoDoiId)
                .NotNull().WithMessage(localizationService.GetResource("PTTT.TheoDoiSauPhauThuatThuThuat.DieuDuongPhuTrachTheoDoi.Required"))
                .Must((model, s) => model.DieuDuongPhuTrachTheoDoiId != 0).WithMessage(localizationService.GetResource("PTTT.TheoDoiSauPhauThuatThuThuat.DieuDuongPhuTrachTheoDoi.Required"))
                .When(model => model.TrangThaiPhauThuatThuThuat == EnumTrangThaiPhauThuatThuThuat.TheoDoi, ApplyConditionTo.AllValidators);

            RuleFor(x => x.ThoiDiemBatDauTheoDoi)
                .NotNull().WithMessage(localizationService.GetResource("PTTT.TheoDoiSauPhauThuatThuThuat.ThoiDiemBatDauTheoDoi.Required"))
                .When(model => model.TrangThaiPhauThuatThuThuat == EnumTrangThaiPhauThuatThuThuat.TheoDoi, ApplyConditionTo.AllValidators);

            RuleFor(x => x.ThoiDiemKetThucTheoDoi)
                .NotNull().WithMessage(localizationService.GetResource("PTTT.TheoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi.Required"))
                .Must((model, s) => model.ThoiDiemKetThucTheoDoi >= model.ThoiDiemBatDauTheoDoi).WithMessage(localizationService.GetResource("PTTT.TheoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi.Validate"))
                .When(model => model.TrangThaiPhauThuatThuThuat == EnumTrangThaiPhauThuatThuThuat.TheoDoi, ApplyConditionTo.AllValidators);

            RuleFor(x => x.GhiChuTheoDoi)
                .MaximumLength(4000).WithMessage(localizationService.GetResource("PTTT.KetLuan.GhiChuTheoDoi.Range"));
        }
    }
}
