using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhDuocPham;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhThuongDuocPhamChiTietViewModel>))]

    public class LinhThuongDuocPhamChiTietViewModelValidator : AbstractValidator<LinhThuongDuocPhamChiTietViewModel>
    {
        public LinhThuongDuocPhamChiTietViewModelValidator(ILocalizationService localizationService, IYeuCauLinhDuocPhamService ycLinhThuongDuocPhamService)
        {
            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraSLTon = await ycLinhThuongDuocPhamService.CheckSoLuongTonDuocPham(viewModel.DuocPhamBenhVienId, viewModel.LaDuocPhamBHYT, viewModel.KhoXuatId, viewModel.SoLuong, viewModel.DuocDuyet, viewModel.SoLuongCoTheXuat, viewModel.IsValidator);
                    return kiemTraSLTon;
                })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));

        }
    }
}
