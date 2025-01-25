using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhVatTu;
using FluentValidation;


namespace Camino.Api.Models.LinhThuongVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhThuongVatTuChiTietViewModel>))]

    public class LinhThuongVatTuChiTietViewModelValidator : AbstractValidator<LinhThuongVatTuChiTietViewModel>
    {
        public LinhThuongVatTuChiTietViewModelValidator(ILocalizationService localizationService, IYeuCauLinhVatTuService ycLinhThuongVatTuService)
        {
            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraSLTon = await ycLinhThuongVatTuService.CheckSoLuongTonVatTu(viewModel.VatTuBenhVienId.Value, viewModel.SoLuong, viewModel.DuocDuyet, viewModel.SoLuongCoTheXuat ,viewModel.KhoXuatId,viewModel.LaVatTuBHYT, viewModel.IsValidator);
                    return kiemTraSLTon;
                })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));

        }
    }
}
