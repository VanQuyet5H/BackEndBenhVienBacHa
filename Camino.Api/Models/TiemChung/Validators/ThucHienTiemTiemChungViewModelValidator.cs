using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThucHienTiemTiemChungViewModel>))]
    public class ThucHienTiemTiemChungViewModelValidator : AbstractValidator<ThucHienTiemTiemChungViewModel>
    {
        public ThucHienTiemTiemChungViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TrangThaiTiemChung)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("ThucHienTiem.TrangThaiTiemChung.Required"));

            RuleFor(x => x.NhanVienTiemId)
                .Must((model,input) => model.TrangThaiTiemChung == null || model.TrangThaiTiemChung == Enums.TrangThaiTiemChung.ChuaTiemChung || input != null)
                .WithMessage(localizationService.GetResource("ThucHienTiem.NhanVienTiemId.Required"));

            RuleFor(x => x.ThoiDiemTiem)
                .Must((model, input) => model.TrangThaiTiemChung == null || model.TrangThaiTiemChung == Enums.TrangThaiTiemChung.ChuaTiemChung || input != null)
                .WithMessage(localizationService.GetResource("ThucHienTiem.ThoiDiemTiem.Required"));
        }
    }
}
