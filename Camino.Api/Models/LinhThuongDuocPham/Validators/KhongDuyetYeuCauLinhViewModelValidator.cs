using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhongDuyetYeuCauLinhViewModel>))]
    public class KhongDuyetYeuCauLinhViewModelValidator : AbstractValidator<KhongDuyetYeuCauLinhViewModel>
    {
        public KhongDuyetYeuCauLinhViewModelValidator(ILocalizationService localizationService, IValidator<DuyetYeuCauLinhDuocPhamChiTietViewModel> yeuCauLinhChiTietValidator)
        {
            RuleFor(x => x.LyDoKhongDuyet).NotEmpty()
                .WithMessage(localizationService.GetResource("DuyetYeuCauLinhDuocPham.LyDoKhongDuyet.Required"));

            RuleForEach(x => x.DuyetYeuCauLinhDuocPhamChiTiets).SetValidator(yeuCauLinhChiTietValidator);
        }
    }
}
