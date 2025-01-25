using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ListKhamBenhPhauThuatThuThuatViewModel>))]
    public class ListKhamBenhPhauThuatThuThuatViewModelValidator : AbstractValidator<ListKhamBenhPhauThuatThuThuatViewModel>
    {
        public ListKhamBenhPhauThuatThuThuatViewModelValidator(ILocalizationService localizationService, IValidator<LuocDoPhauThuatViewModel> listLuocDoPhauThuatThuThuatValidator)
        {
            RuleFor(x => x.GhiChuICDSauPhauThuat)
                .Length(0, 4000).WithMessage(localizationService.GetResource("PhauThuatThuThuatTuongTrinh.GhiChuICDSauPhauThuat.Range.4000"));

            RuleFor(x => x.GhiChuICDTruocPhauThuat)
                .Length(0, 4000).WithMessage(localizationService.GetResource("PhauThuatThuThuatTuongTrinh.GhiChuICDTruocPhauThuat.Range.4000"));

            RuleFor(x => x.TrinhTuPttt)
                .Length(0, 4000).WithMessage(localizationService.GetResource("PhauThuatThuThuatTuongTrinh.TrinhTuPttt.Range.4000"));

            RuleForEach(x => x.LuocDoPttts).SetValidator(listLuocDoPhauThuatThuThuatValidator);
        }
    }
}
