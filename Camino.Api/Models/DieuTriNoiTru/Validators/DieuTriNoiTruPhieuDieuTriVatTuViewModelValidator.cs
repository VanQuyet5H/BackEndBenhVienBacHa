using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DieuTriNoiTruPhieuDieuTriVatTuViewModel>))]

    public class DieuTriNoiTruPhieuDieuTriVatTuViewModelValidator : AbstractValidator<DieuTriNoiTruPhieuDieuTriVatTuViewModel>
    {
        public DieuTriNoiTruPhieuDieuTriVatTuViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoId).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.KhoId.Required"));

            RuleFor(x => x.SoLuong).NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"));

            RuleFor(x => x.VatTuBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongVatTu.TenVatTu.Required"));
        }
    }
}
