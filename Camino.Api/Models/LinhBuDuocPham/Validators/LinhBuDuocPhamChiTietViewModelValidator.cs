using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhDuocPham;
using FluentValidation;

namespace Camino.Api.Models.LinhBuDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhBuDuocPhamChiTietViewModel>))]

    public class LinhBuDuocPhamChiTietViewModelValidator : AbstractValidator<LinhBuDuocPhamChiTietViewModel>
    {
        public LinhBuDuocPhamChiTietViewModelValidator(ILocalizationService localizationService, IYeuCauLinhDuocPhamService ycLinhBuDuocPhamService)
        {
            //RuleFor(x => x.SLYeuCauLinhThucTe)
            //    .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
            //    .MustAsync(async (viewModel, soLuongTon, id) =>
            //    {
            //        var kiemTraSLTon = await ycLinhBuDuocPhamService.CheckSoLuongTonDuocPhamLinhBu(viewModel.DuocPhamBenhVienId.GetValueOrDefault(), viewModel.LaDuocPhamBHYT, viewModel.KhoLinhId,viewModel.SLYeuCauLinhThucTe);
            //        return kiemTraSLTon;
            //    })
            //    .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));
        }
    }
}
