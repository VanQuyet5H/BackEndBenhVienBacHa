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
    [TransientDependency(ServiceType = typeof(IValidator<DuyetYeuCauLinhVatTuViewModel>))]
    public class DuyetYeuCauLinhVatTuThuongViewModelValidator: AbstractValidator<DuyetYeuCauLinhVatTuViewModel>
    {
        public DuyetYeuCauLinhVatTuThuongViewModelValidator(ILocalizationService localizationService, IValidator<DuyetYeuCauLinhVatTuChiTietViewModel> chiTietValidator)
        {
            RuleFor(x => x.NguoiNhapKhoId).NotEmpty().WithMessage(localizationService.GetResource("DuyetYeuCauLinhVatTu.NguoiNhapKhoId.Required"));
            RuleFor(x => x.NguoiXuatKhoId).NotEmpty().WithMessage(localizationService.GetResource("DuyetYeuCauLinhVatTu.NguoiXuatKhoId.Required"));

            RuleForEach(x => x.DuyetYeuCauLinhVatTuChiTiets).SetValidator(chiTietValidator);
        }
    }
}
