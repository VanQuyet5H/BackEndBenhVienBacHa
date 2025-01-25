using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YcVTBvVo>))]

    public class YcVTBvVoValidator : AbstractValidator<YcVTBvVo>
    {
        public YcVTBvVoValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            RuleFor(x => x.SoLuongTra)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraSLTon = await dieuTriNoiTruService.CheckSoLuongTra(viewModel.SoLuong, viewModel.SoLuongDaTra, viewModel.SoLuongTra);
                    return kiemTraSLTon;
                })
                .WithMessage(localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));

        }
    }
}
