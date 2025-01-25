using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Services.Localization;
using FluentValidation;
using System;


namespace Camino.Api.Models.XuatKhos.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoDuocPhamKhacViewModel>))]

    public class XuatKhoDuocPhamKhacViewModelValidator : AbstractValidator<XuatKhoDuocPhamKhacViewModel>
    {
        public XuatKhoDuocPhamKhacViewModelValidator(ILocalizationService localizationService, IValidator<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatKhoDuocPhamChiTietsValidator)
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

            RuleFor(x => x.NhapKhoDuocPhamId)
             .NotEmpty().WithMessage(localizationService.GetResource("XuatKhac.SoHoaDon.Required")).When(z => z.TraNCC == true);

            RuleForEach(x => x.YeuCauXuatKhoDuocPhamChiTiets).SetValidator(yeuCauXuatKhoDuocPhamChiTietsValidator);

        }
    }
}
