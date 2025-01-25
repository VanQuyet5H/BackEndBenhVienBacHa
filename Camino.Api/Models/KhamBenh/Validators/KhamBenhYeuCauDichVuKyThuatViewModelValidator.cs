using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhYeuCauDichVuKyThuatViewModel>))]
    public class KhamBenhYeuCauDichVuKyThuatViewModelValidator : AbstractValidator<KhamBenhYeuCauDichVuKyThuatViewModel>
    {
        public KhamBenhYeuCauDichVuKyThuatViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NhomDichVuBenhVienId)
                   .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.NhomDichVuId.Required"));

            RuleFor(x => x.DichVuKyThuatBenhVienId)
                  .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.DichVuKyThuatBenhVienId.Required"));

            //RuleFor(x => x.LoaiDichVuKyThuat)
            //     .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.LoaiDichVuKyThuat.Required"));

            //RuleFor(x => x.SoLan)
            //    .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.SoLuong.Required"));

            RuleFor(x => x.NoiThucHienId)
                .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.NoiThucHienId.Required"));
            //RuleFor(x => x.NhanVienThucHienId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.NhanVienThucHienId.Required"));
        }
    }
}
