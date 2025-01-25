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
    [TransientDependency(ServiceType = typeof(IValidator<DuyetYeuCauLinhKSNKViewModel>))]
    public class DuyetYeuCauLinhKSNKThuongViewModelValidator : AbstractValidator<DuyetYeuCauLinhKSNKViewModel>
    {
        public DuyetYeuCauLinhKSNKThuongViewModelValidator(ILocalizationService localizationService, IValidator<DuyetYeuCauLinhKSNKChiTietViewModel> chiTietValidator)
        {
            RuleFor(x => x.NguoiNhapKhoId).NotEmpty().WithMessage(localizationService.GetResource("DuyetYeuCauLinh.NguoiNhapKhoId.Required"));
            RuleFor(x => x.NguoiXuatKhoId).NotEmpty().WithMessage(localizationService.GetResource("DuyetYeuCauLinh.NguoiXuatKhoId.Required"));

            RuleForEach(x => x.DuyetYeuCauLinhVatTuChiTiets).SetValidator(chiTietValidator);
        }
    }
}
