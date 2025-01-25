using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhDuocPham;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuocPhamGridViewModel>))]
    public class DuocPhamGridViewModelValidator : AbstractValidator<DuocPhamGridViewModel>
    {
        public DuocPhamGridViewModelValidator(ILocalizationService localizationService, IYeuCauLinhDuocPhamService ycLinhThuongDuocPhamService)
        {
            RuleFor(x => x.DuocPhamBenhVienId)
               .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DuocPham.Required"))
               .MustAsync(async (viewModel, soLuongTon, id) =>
               {
                   var kiemTraExists = await ycLinhThuongDuocPhamService.CheckDuocPhamExists(viewModel.DuocPhamBenhVienId, viewModel.LaDuocPhamBHYT, viewModel.DuocPhamBenhViens);
                   return kiemTraExists;
               })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.DuocPham.Exists"));

            RuleFor(x => x.SLYeuCau)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraSLTon = await ycLinhThuongDuocPhamService.CheckSoLuongTonDuocPhamDuocPhamGridVo(viewModel.DuocPhamBenhVienId, viewModel.SLYeuCau, viewModel.KhoXuatId, viewModel.LaDuocPhamBHYT);
                    return kiemTraSLTon;
                })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));
        }
    }
}
