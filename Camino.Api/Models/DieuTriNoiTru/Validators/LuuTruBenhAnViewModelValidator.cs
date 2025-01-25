using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LuuTruBenhAnViewModel>))]

    public class LuuTruBenhAnViewModelValidator : AbstractValidator<LuuTruBenhAnViewModel>
    {
        public LuuTruBenhAnViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
             RuleFor(x => x.ThuTuSapXepLuuTru)
            //.NotEmpty().WithMessage(localizationService.GetResource("LuuTruBenhAn.ThuTuSapXepLuuTru.Required"))            
            .MustAsync(async (model, input, s) => !await dieuTriNoiTruService.KiemTraThuTuSapXepLuuTruBATrung(model.NoiTruBenhAnId, model.ThuTuSapXepLuuTru).ConfigureAwait(false))
            .WithMessage(localizationService.GetResource("DieuTriNoiTru.ThuTuSapXepLuuTru.ThuTuSapXepLuuTruTrung"));
        }
    }
}
