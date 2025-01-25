using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BenhAnDienTus;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.GayBenhAn.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GayBenhAnViewModel>))]
    public class GayBenhAnViewModelValidator : AbstractValidator<GayBenhAnViewModel>
    {
        public GayBenhAnViewModelValidator(ILocalizationService localizationService, IBenhAnDienTuService benhAnDienTuService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required")).MustAsync(async (request, ten, id) =>
                {
                    var checkExistsTen = await benhAnDienTuService.IsMaTenExists(request.Ma, request.Id, false);
                    return checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, tenVT, id) =>
                {
                    var checkExistsTenVT = await benhAnDienTuService.IsMaTenExists(request.Ten, request.Id, true);
                    return checkExistsTenVT;
                })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.ViTriGay)
            .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.KhoDuocPhamViTriId.Required"));

            RuleFor(x => x.GayBenhAnPhieuHoSoIds)
             .NotEmpty().WithMessage(localizationService.GetResource("GayBenhAn.GayBenhAnPhieuHoSoIds.Required"));
        }
    }
}
