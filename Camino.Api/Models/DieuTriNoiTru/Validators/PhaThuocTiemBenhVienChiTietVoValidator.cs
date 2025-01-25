using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhaThuocTiemBenhVienChiTietVo>))]

    public class PhaThuocTiemBenhVienChiTietVoValidator : AbstractValidator<PhaThuocTiemBenhVienChiTietVo>
    {
        public PhaThuocTiemBenhVienChiTietVoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoId).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.KhoId.Required"));

            RuleFor(x => x.DuocPhamBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DuocPham.Required"));

            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"));
        }
    }
}
