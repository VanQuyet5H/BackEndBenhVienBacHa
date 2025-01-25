using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamDoan;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GoiKhamDichVuKhamSucKhoeDoanViewModel>))]

    public class DichVuKhamSucKhoeDoanViewModelValidator : AbstractValidator<GoiKhamDichVuKhamSucKhoeDoanViewModel>
    {
        public DichVuKhamSucKhoeDoanViewModelValidator(ILocalizationService localizationService, IKhamDoanService khamDoanService)
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

            //update BVHD-3944
            RuleFor(x => x.DonGiaUuDai)
                        .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.DonGiaHopDong.Required"))
                        .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.DonGiaHopDong.GreaterThanOrEqualTo"));

            RuleFor(x => x.DonGiaThucTe)
                          .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.DonGiaThucTe.Required"))
                          .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.DonGiaThucTe.GreaterThanOrEqualTo"));

            //update end BVHD-3944


            //RuleFor(x => x.DonGiaChuaUuDai)
            //       .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.DonGiaChuaUuDai.Required"));

            RuleFor(x => x.SoTuoiDen)
                .Must((model, s) => model.SoTuoiTu <= model.SoTuoiDen)
                .WithMessage(localizationService.GetResource("KhamDoan.SoTuoiDen.GreaterThan"))
                .When(x => x.SoTuoiTu != null && x.SoTuoiDen != null && x.SoTuoiTu > x.SoTuoiDen);

            RuleFor(x => x.GoiKhamSucKhoeNoiThucHienIds)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.NoiThucHienIds.Required"));

            RuleFor(x => x.SoLan)
               .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.SoLan.Required"))
               .When(x => !x.LaDichVuKham);
        }

    }
}
