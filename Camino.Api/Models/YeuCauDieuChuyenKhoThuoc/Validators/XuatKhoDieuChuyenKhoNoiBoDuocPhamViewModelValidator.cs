using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauDieuChuyenDuocPhams;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauDieuChuyenKhoThuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel>))]
    public class XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModelValidator : AbstractValidator<XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel>
    {
        public XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModelValidator(ILocalizationService localizationService, IValidator<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo> yeuCauDieuChuyenDuocPhamChiTietValidator)
        {
            RuleFor(x => x.KhoXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.KhoXuatId.Required"))
                .Must((model, s) => model.KhoNhapId != model.KhoXuatId)
                .WithMessage(localizationService.GetResource("XuatKhoDieuChuyenKhoNoiBoDuocPham.KhoNhapId.Selected"));

            RuleFor(x => x.KhoNhapId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.KhoNhapId.Required"))
                .Must((model, s) => model.KhoNhapId != model.KhoXuatId)
                .WithMessage(localizationService.GetResource("XuatKhoDieuChuyenKhoNoiBoDuocPham.KhoXuatId.Selected"));

            RuleFor(x => x.NguoiNhapId)
                .NotEmpty().WithMessage(localizationService.GetResource("Marketing.NguoiNhapId.Required"));

            RuleForEach(x => x.YeuCauDieuChuyenDuocPhamChiTiets).SetValidator(yeuCauDieuChuyenDuocPhamChiTietValidator);

        }
    }
}
