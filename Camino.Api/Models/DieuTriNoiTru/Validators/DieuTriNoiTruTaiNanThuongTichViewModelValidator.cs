using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DieuTriNoiTruTaiNanThuongTichViewModel>))]
    public class DieuTriNoiTruTaiNanThuongTichViewModelValidator : AbstractValidator<DieuTriNoiTruTaiNanThuongTichViewModel>
    {
        public DieuTriNoiTruTaiNanThuongTichViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.ThoiGianXayRaTaiNan)
                .Must((model, s) => dieuTriNoiTruService.KiemTraThoiGianXayRaTaiNanVoiHienTai(model.ThoiGianXayRaTaiNan)).WithMessage(localizationService.GetResource("DieuTriNoiTru.TaiNanThuongTich.ThoiGianXayRaTaiNan.LessThan.Now"));

            RuleFor(x => x.ThoiGianDenCapCuu)
                .Must((model, s) => dieuTriNoiTruService.KiemTraThoiGianDenCapCuuVoiHienTai(model.ThoiGianDenCapCuu)).WithMessage(localizationService.GetResource("DieuTriNoiTru.TaiNanThuongTich.ThoiGianDenCapCuu.LessThan.Now"))
                .Must((model, s) => dieuTriNoiTruService.KiemTraThoiGianDenCapCuuVoiThoiGianXayRaTaiNan(model.ThoiGianXayRaTaiNan, model.ThoiGianDenCapCuu)).WithMessage(localizationService.GetResource("DieuTriNoiTru.TaiNanThuongTich.ThoiGianDenCapCuu.GreaterThan.ThoiGianXayRaTaiNan"));
        }
    }
}