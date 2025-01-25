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
    [TransientDependency(ServiceType = typeof(IValidator<KhongDuyetYeuCauLinhVatTuViewModel>))]
    public class KhongDuyetYeuCauLinhViewModelValidator : AbstractValidator<KhongDuyetYeuCauLinhVatTuViewModel>
    {
        public KhongDuyetYeuCauLinhViewModelValidator(ILocalizationService localizationService, IValidator<DuyetYeuCauLinhVatTuChiTietViewModel> yeuCauLinhVatTuChiTietValidator)
        {
            RuleFor(x => x.LyDoKhongDuyet).NotEmpty()
                .WithMessage(localizationService.GetResource("DuyetYeuCauLinhVatTu.LyDoKhongDuyet.Required"));

            RuleForEach(x => x.DuyetYeuCauLinhVatTuChiTiets).SetValidator(yeuCauLinhVatTuChiTietValidator);
        }
    }
}
