using Camino.Api.Models.LinhKSNK;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhThuongKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetYeuCauLinhKSNKChiTietViewModel>))]
    public class DuyetYeuCauLinhKSNKChiTietViewModelValidator : AbstractValidator<DuyetYeuCauLinhKSNKChiTietViewModel>
    {
        public DuyetYeuCauLinhKSNKChiTietViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SoLuongCoTheXuat).NotEmpty().WithMessage(
                localizationService.GetResource("DuyetYeuCauLinhKSNKThuongChiTiet.SoLuongCoTheXuat.Required"));
        }
    }
}
