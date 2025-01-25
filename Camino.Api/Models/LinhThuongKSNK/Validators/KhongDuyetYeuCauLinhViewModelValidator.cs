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
    [TransientDependency(ServiceType = typeof(IValidator<KhongDuyetYeuCauLinhKSNKViewModel>))]
    public class KhongDuyetYeuCauLinhViewModelValidator : AbstractValidator<KhongDuyetYeuCauLinhKSNKViewModel>
    {
        public KhongDuyetYeuCauLinhViewModelValidator(ILocalizationService localizationService, IValidator<DuyetYeuCauLinhKSNKChiTietViewModel> yeuCauLinhVatTuChiTietValidator)
        {
            RuleFor(x => x.LyDoKhongDuyet).NotEmpty()
                .WithMessage(localizationService.GetResource("DuyetYeuCauLinhVatTu.LyDoKhongDuyet.Required"));

            RuleForEach(x => x.DuyetYeuCauLinhVatTuChiTiets).SetValidator(yeuCauLinhVatTuChiTietValidator);
        }
    }
}
