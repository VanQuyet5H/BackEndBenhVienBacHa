using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DieuTriNoiTruPhieuDieuTriPhaThuocTruyenViewModel>))]

    public class DieuTriNoiTruPhieuDieuTriPhaThuocTruyenViewModelValidator : AbstractValidator<DieuTriNoiTruPhieuDieuTriPhaThuocTruyenViewModel>
    {
        public DieuTriNoiTruPhieuDieuTriPhaThuocTruyenViewModelValidator(ILocalizationService localizationService, IValidator<PhaThuocTiemBenhVienChiTietVo> noiTruChiDinhDuocPhamValidator)
        {
            RuleFor(x => x.ThoiGianBatDauTruyen)
              .NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.ThoiGianBatDauTruyen.Required"))
               .When(x => !x.IsDelete);


            RuleFor(x => x.CachGioTruyen).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.CachGioTruyenDich.Required"))
           .When(x => x.SoLanDungTrongNgay != null && x.SoLanDungTrongNgay > 1 && x.LaDichTruyen != true && !x.IsDelete);

            RuleForEach(x => x.NoiTruChiDinhDuocPhams).SetValidator(noiTruChiDinhDuocPhamValidator);
        }
    }
}
