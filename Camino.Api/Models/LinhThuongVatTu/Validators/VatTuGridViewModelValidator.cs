using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhVatTu;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<VatTuGridViewModel>))]

    public class VatTuGridViewModelValidator : AbstractValidator<VatTuGridViewModel>
    {
        public VatTuGridViewModelValidator(ILocalizationService localizationService, IYeuCauLinhVatTuService ycLinhThuongVatTuService)
        {
            RuleFor(x => x.VatTuBenhVienId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongVatTu.TenVatTu.Required"))
               .MustAsync(async (viewModel, soLuongTon, id) =>
               {
                   var kiemTraExists = await ycLinhThuongVatTuService.CheckVatTuExists(viewModel.VatTuBenhVienId, viewModel.LaVatTuBHYT, viewModel.VatTuBenhViens);
                   return kiemTraExists;
               })
                .WithMessage(localizationService.GetResource("LinhThuongVatTu.TenVatTu.Exists"))
                ;

            RuleFor(x => x.SLYeuCau)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraSLTon = await ycLinhThuongVatTuService.CheckSoLuongTonVatTuGridVo(viewModel.VatTuBenhVienId, viewModel.SLYeuCau, viewModel.KhoXuatId, viewModel.LaVatTuBHYT);
                    return kiemTraSLTon;
                })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));
        }
    }
}
