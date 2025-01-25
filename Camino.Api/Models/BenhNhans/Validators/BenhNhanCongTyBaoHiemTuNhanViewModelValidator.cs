using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BenhNhans;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.BenhNhans.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BenhNhanBaoHiemTuNhansViewModel>))]
    public class BenhNhanCongTyBaoHiemTuNhanViewModelValidator : AbstractValidator<BenhNhanBaoHiemTuNhansViewModel>
    {
        public BenhNhanCongTyBaoHiemTuNhanViewModelValidator(ILocalizationService localizationService, IBenhNhanService benhNhanService)
        {
            RuleFor(x => x.CongTyBaoHiemTuNhanId)
              .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.CongTyBaoHiemTuNhan.Required"))
              .MustAsync(async (viewModel, congTyBHTNId, id) =>
              {
                  var kiemTraExists = await benhNhanService.CheckCongTyBHTNExists(viewModel.CongTyBaoHiemTuNhanId, viewModel.CongTyBHTNIds);
                  return kiemTraExists;
              })
              .WithMessage(localizationService.GetResource("BenhNhan.CongTyBaoHiemTuNhan.Exists"));

            RuleFor(x => x.NgayHetHan).GreaterThan(x=>x.NgayHieuLuc)
                .WithMessage(localizationService.GetResource("BenhNhan.NgayHetHan.GreaterThan.NgayHieuLuc"));
        }

    }
}
