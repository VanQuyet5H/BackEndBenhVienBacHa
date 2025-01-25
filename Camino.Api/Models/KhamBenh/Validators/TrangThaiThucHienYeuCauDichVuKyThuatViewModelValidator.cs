using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TrangThaiThucHienYeuCauDichVuKyThuatViewModel>))]
    public class TrangThaiThucHienYeuCauDichVuKyThuatViewModelValidator : AbstractValidator<TrangThaiThucHienYeuCauDichVuKyThuatViewModel>
    {
        public TrangThaiThucHienYeuCauDichVuKyThuatViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NhanVienThucHienId)
                .Must((model, input) => !model.LaThucHienDichVu || input != null ).WithMessage(localizationService.GetResource("CapNhatThucHienDichVuKyThuat.NhanVienThucHienId.Required"));
            RuleFor(x => x.ThoiDiemThucHien)
                .Must((model, input) => !model.LaThucHienDichVu || input != null).WithMessage(localizationService.GetResource("CapNhatThucHienDichVuKyThuat.ThoiDiemThucHien.Required"));

            RuleFor(x => x.LyDoHuyTrangThaiDaThucHien)
                .Must((model, input) => model.LaThucHienDichVu || !string.IsNullOrEmpty(input)).WithMessage(localizationService.GetResource("CapNhatThucHienDichVuKyThuat.LyDoHuyTrangThaiDaThucHien.Required"));
        }
    }
}
