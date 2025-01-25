using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetYeuCauLinhDuocPhamChiTietViewModel>))]
    public class DuyetYeuCauLinhDuocPhamChiTietViewModelValidator : AbstractValidator<DuyetYeuCauLinhDuocPhamChiTietViewModel>
    {
        public DuyetYeuCauLinhDuocPhamChiTietViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SoLuongCoTheXuat).NotEmpty().WithMessage(
                localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuongChiTiet.SoLuongCoTheXuat.Required"));
        }
    }
}
