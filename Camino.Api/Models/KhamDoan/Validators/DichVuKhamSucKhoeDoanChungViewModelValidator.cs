using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamDoan;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GoiKhamDichVuKhamSucKhoeDoanChungViewModel>))]

    public class DichVuKhamSucKhoeDoanChungViewModelValidator : AbstractValidator<GoiKhamDichVuKhamSucKhoeDoanChungViewModel>
    {
        public DichVuKhamSucKhoeDoanChungViewModelValidator(ILocalizationService localizationService, IKhamDoanService khamDoanService)
        {
            RuleFor(x => x.DichVuKyThuatBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("ChiDinhDichVuKyThuat.DichVuKyThuatBenhVienChiDinhs.Required"))
                 .MustAsync(async (viewModel, dichVu, id) =>
                 {
                     var kiemTraExists = await khamDoanService.CheckDichVuExists(viewModel.DichVuKyThuatBenhVienId, viewModel.LaDichVuKham, viewModel.DichVuKhamBenhIds, viewModel.DichVuKyThuatIds);
                     return kiemTraExists;
                 })
                .WithMessage(localizationService.GetResource("ChiDinhDichVuKyThuat.DichVuKyThuatBenhVienChiDinhs.Exists"));

            RuleFor(x => x.DonGiaBenhVien)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.DonGiaBenhVien.Required"));

            RuleFor(x => x.DonGiaUuDai)
                           .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.DonGiaUuDai.Required"));


            RuleFor(x => x.SoTuoiDen)
                .Must((model, s) => model.SoTuoiTu <= model.SoTuoiDen)
                .WithMessage(localizationService.GetResource("KhamDoan.SoTuoiDen.GreaterThan"))
                .When(x => x.SoTuoiTu != null && x.SoTuoiDen != null && x.SoTuoiTu > x.SoTuoiDen);

            RuleFor(x => x.GoiKhamSucKhoeChungNoiThucHienIds)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.NoiThucHienIds.Required"));

            RuleFor(x => x.SoLan)
               .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.SoLan.Required"))
               .When(x => !x.LaDichVuKham);
        }

    }
}
