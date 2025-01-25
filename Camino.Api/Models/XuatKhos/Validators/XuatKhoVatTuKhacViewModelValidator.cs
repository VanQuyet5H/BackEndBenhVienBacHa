using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Services.Localization;
using FluentValidation;
using System;


namespace Camino.Api.Models.XuatKhos.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoVatTuKhacViewModel>))]

    public class XuatKhoVatTuKhacViewModelValidator : AbstractValidator<XuatKhoVatTuKhacViewModel>
    {
        public XuatKhoVatTuKhacViewModelValidator(ILocalizationService localizationService, IValidator<XuatKhoKhacVatTuChiTietVo> yeuCauXuatKhoVatTuChiTietsValidator)
        {
            RuleFor(x => x.KhoXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.KhoDuocPhamXuatId.Required"));

            RuleFor(x => x.LyDoXuatKho)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.LyDoXuatKho.Required"));

            RuleFor(x => x.NgayXuat)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.NgayXuat.Required"));

            RuleFor(x => x.NgayXuat)
                 .Must((model, s) => (model.NgayXuat != null && model.NgayXuat < DateTime.Now) || model.NgayXuat == null).WithMessage(localizationService.GetResource("XuatKho.NgayXuat.MoreThanNow"));

            RuleFor(x => x.NhaThauId)
          .NotEmpty().WithMessage(localizationService.GetResource("XuatKhac.NhaCC.Required")).When(z => z.TraNCC == true);

            RuleFor(x => x.NhapKhoVatTuId)
             .NotEmpty().WithMessage(localizationService.GetResource("XuatKhac.SoHoaDon.Required")).When(z => z.TraNCC == true);

            RuleForEach(x => x.YeuCauXuatKhoVatTuChiTiets).SetValidator(yeuCauXuatKhoVatTuChiTietsValidator);

        }
    }
}
