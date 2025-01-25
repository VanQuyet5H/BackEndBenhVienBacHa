using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TongHopYLenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TongHopYLenhThemMoiViewModel>))]
    public class TongHopYLenhThemMoiViewModelValidator : AbstractValidator<TongHopYLenhThemMoiViewModel>
    {
        public TongHopYLenhThemMoiViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.GioYLenh)
                .NotEmpty().WithMessage(localizationService.GetResource("TongHopYLenh.GioYLenh.Required"));
            RuleFor(x => x.MoTaYLenh)
                .NotEmpty().WithMessage(localizationService.GetResource("TongHopYLenh.MoTaYLenh.Required"));
            RuleFor(x => x.NhanVienXacNhanThucHienId)
                .Must((model, input) => model.XacNhanThucHien != true || input != null).WithMessage(localizationService.GetResource("TongHopYLenh.NhanVienXacNhanThucHienId.Required"));

            //Cập nhật 06/06/2022: Cập nhật giờ thực hiện -> đổi từ chỉ nhập giờ thành nhập ngày giờ (Áp dụng cho trường hợp check vào xác nhận thực hiện trên grid)
            RuleFor(x => x.GioThucHien)
                .Must((model, input) => model.XacNhanThucHien != true || (model.XacNhanThucHien == true && (input != null || model.ThoiDiemXacNhanThucHien != null)))
                .WithMessage(localizationService.GetResource("TongHopYLenh.ThoiDiemXacNhanThucHien.Required"));
        }
    }
}
