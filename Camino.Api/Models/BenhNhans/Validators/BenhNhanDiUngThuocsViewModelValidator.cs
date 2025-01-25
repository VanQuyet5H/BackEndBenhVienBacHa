using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Services.BenhNhans;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.BenhNhans.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BenhNhanDiUngThuocsViewModel>))]

    public class BenhNhanDiUngThuocsViewModelValidator : AbstractValidator<BenhNhanDiUngThuocsViewModel>
    {
        public BenhNhanDiUngThuocsViewModelValidator(ILocalizationService localizationService, IBenhNhanService benhNhanService)
        {
            RuleFor(x => x.TenDiUng)
              .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.TenDiUng.Required"))
              .When(x => x.LoaiDiUng != Enums.LoaiDiUng.Thuoc || x.LoaiDiUng == null)
              .MustAsync(async (viewModel, ten, id) =>
              {
                  var kiemTraExists = await benhNhanService.CheckDiUngThuocExists(viewModel.LoaiDiUng, viewModel.ThuocId, viewModel.TenDiUng, viewModel.TenDiUngThuocs);
                  return kiemTraExists;
              })
              .WithMessage(localizationService.GetResource("BenhNhan.TenDiUng.Exists"));

            RuleFor(x => x.ThuocId)
              .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.TenDiUng.Required"))
              .When(x => x.LoaiDiUng == Enums.LoaiDiUng.Thuoc || x.LoaiDiUng == null)
              .MustAsync(async (viewModel, ten, id) =>
              {
                  var kiemTraExists = await benhNhanService.CheckDiUngThuocExists(viewModel.LoaiDiUng, viewModel.ThuocId, viewModel.TenDiUng, viewModel.TenDiUngThuocs);
                  return kiemTraExists;
              })
              .WithMessage(localizationService.GetResource("BenhNhan.TenDiUng.Exists"));

            RuleFor(x => x.LoaiDiUng)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.LoaiDiUng.Required"));

            RuleFor(x => x.BieuHienDiUng)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.BieuHienDiUng.Required"));

            RuleFor(x => x.MucDo)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.MucDo.Required"));
        }
    }
}
