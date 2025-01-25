using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauDieuChuyenDuocPhams;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauDieuChuyenKhoThuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo>))]

    public class XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVoValidator : AbstractValidator<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo>
    {
        public XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SoLuongDieuChuyen)
                .Must((model, s) => model.SoLuongDieuChuyen > 0)
                .WithMessage(localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongXuatMoreThan0"));
        }
    }
}
