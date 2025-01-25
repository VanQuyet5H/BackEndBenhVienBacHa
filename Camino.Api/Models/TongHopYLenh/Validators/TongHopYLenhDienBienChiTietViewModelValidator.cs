using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TongHopYLenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TongHopYLenhDienBienChiTietViewModel>))]
    public class TongHopYLenhDienBienChiTietViewModelValidator : AbstractValidator<TongHopYLenhDienBienChiTietViewModel>
    {
        public TongHopYLenhDienBienChiTietViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NhanVienXacNhanThucHienId)
                .Must((model, input) => model.XacNhanThucHien != true || input != null).WithMessage(localizationService.GetResource("TongHopYLenh.NhanVienXacNhanThucHienId.Required"));

            //Cập nhật 06/06/2022: Cập nhật giờ thực hiện -> đổi từ chỉ nhập giờ thành nhập ngày giờ (Áp dụng cho trường hợp check vào xác nhận thực hiện trên grid)
            //RuleFor(x => x.GioThucHien)
            //    .Must((model, input) => model.XacNhanThucHien != true || input != null).WithMessage(localizationService.GetResource("TongHopYLenh.ThoiDiemXacNhanThucHien.Required"));

            RuleFor(x => x.ThoiDiemXacNhanThucHien)
                .Must((model, input) => model.XacNhanThucHien != true || input != null)
                .WithMessage(localizationService.GetResource("TongHopYLenh.ThoiDiemXacNhanThucHien.Required"));
        }
    }
}
