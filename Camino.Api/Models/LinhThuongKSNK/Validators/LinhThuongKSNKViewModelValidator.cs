using Camino.Api.Models.LinhThuongVatTu;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhThuongKSNK.Validators
{
    
    [TransientDependency(ServiceType = typeof(IValidator<LinhThuongKSNKViewModel>))]

    public class LinhThuongKSNKViewModelValidator : AbstractValidator<LinhThuongKSNKViewModel>
    {
        public LinhThuongKSNKViewModelValidator(ILocalizationService localizationService, IValidator<LinhThuongKNSKChiTietViewModel> linhThuongKSNKChiTietValidator)
        {
            RuleFor(x => x.KhoNhapId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhVeKho.Required"));

            RuleFor(x => x.KhoXuatId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhTuKho.Required"));

            RuleForEach(x => x.YeuCauLinhVatTuChiTiets).SetValidator(linhThuongKSNKChiTietValidator);


        }
    }
}
