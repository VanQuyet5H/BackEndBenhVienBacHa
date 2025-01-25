using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.LinhVatTu;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetYeuCauLinhVatTuChiTietViewModel>))]
    public class DuyetYeuCauLinhVatTuChiTietViewModelValidator : AbstractValidator<DuyetYeuCauLinhVatTuChiTietViewModel>
    {
        public DuyetYeuCauLinhVatTuChiTietViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SoLuongCoTheXuat).NotEmpty().WithMessage(
                localizationService.GetResource("DuyetYeuCauLinhVatTuThuongChiTiet.SoLuongCoTheXuat.Required"));
        }
    }
}
