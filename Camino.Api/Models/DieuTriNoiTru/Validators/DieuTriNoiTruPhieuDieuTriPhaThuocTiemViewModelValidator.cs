using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel>))]

    public class DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModelValidator : AbstractValidator<DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel>
    {
        public DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModelValidator(ILocalizationService localizationService, IValidator<PhaThuocTiemBenhVienChiTietVo> noiTruChiDinhDuocPhamValidator)
        {
            RuleFor(x => x.ThoiGianBatDauTiem)
              .NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.ThoiGianBatDauTruyen.Required"))
               .When(x => !x.IsDelete);

            RuleFor(x => x.CachGioTiem).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.CachGioTruyenDich.Required"))
           .When(x => x.SoLanDungTrongNgay != null && x.SoLanDungTrongNgay > 1 && x.LaDichTruyen != true && !x.IsDelete);

            RuleForEach(x => x.NoiTruChiDinhDuocPhams).SetValidator(noiTruChiDinhDuocPhamValidator);
        }
    }
}
