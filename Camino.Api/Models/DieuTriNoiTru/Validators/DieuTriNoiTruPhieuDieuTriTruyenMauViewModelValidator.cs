using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DieuTriNoiTruPhieuDieuTriTruyenMauViewModel>))]

    public class DieuTriNoiTruPhieuDieuTriTruyenMauViewModelValidator : AbstractValidator<DieuTriNoiTruPhieuDieuTriTruyenMauViewModel>
    {
        public DieuTriNoiTruPhieuDieuTriTruyenMauViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.MauVaChePhamId).NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DuocPham.Required"));

            RuleFor(x => x.NhomMau).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.NhomMau.Required"));

            //RuleFor(x => x.ThoiGianBatDauTruyen).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.ThoiGianBatDauTruyen.Required"));

        }
    }
}
