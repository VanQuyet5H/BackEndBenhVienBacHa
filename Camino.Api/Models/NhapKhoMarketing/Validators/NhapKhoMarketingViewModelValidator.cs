using Camino.Api.Models.NhapKhoVatTuChiTiets;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhapKhoQuaTangMarketing;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoMarketing.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhapKhoMarketingViewModel>))]
    public class NhapKhoMarketingViewModelValidator : AbstractValidator<NhapKhoMarketingViewModel>
    {
        public NhapKhoMarketingViewModelValidator(ILocalizationService localizationService, INhapKhoQuaTangMarketingService nhapKhoQuaTangMarketingService, IValidator<DanhSachQuaTangDuocThem> danhSachQuaTangDuocThemIValidator)
        {
            RuleFor(x => x.SoChungTu)
              .NotEmpty().WithMessage(localizationService.GetResource("Marketing.SoChungTu.Required"));
            RuleFor(x => x.TenNguoiGiao)
          .NotEmpty().WithMessage(localizationService.GetResource("Marketing.NguoiGiaoId.Required"));
            RuleFor(x => x.NguoiNhapId)
           .NotEmpty().WithMessage(localizationService.GetResource("Marketing.NguoiNhapId.Required"));
            RuleFor(x => x.NgayNhap)
          .NotEmpty().WithMessage(localizationService.GetResource("Marketing.NgayNhap.Required"));
          //  RuleFor(x => x.DanhSachQuaTangDuocThemList)
          //.NotEmpty().WithMessage(localizationService.GetResource("Marketing.DanhSachQuaTangDuocThemList.Required"));
            RuleForEach(x => x.DanhSachQuaTangDuocThemList).SetValidator(danhSachQuaTangDuocThemIValidator);
        }
    }
}
