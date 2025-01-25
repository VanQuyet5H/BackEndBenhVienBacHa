using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Services.Localization;
using Camino.Services.XuatKhoKhacs;
using FluentValidation;

namespace Camino.Api.Models.XuatKhos.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoKhacVatTuChiTietVo>))]

    public class XuatKhoVatTuKhacChiTietViewModelValidator : AbstractValidator<XuatKhoKhacVatTuChiTietVo>
    {
        public XuatKhoVatTuKhacChiTietViewModelValidator(ILocalizationService localizationService, IXuatKhoVatTuKhacService xuatKhoKhacService)
        {
            RuleFor(x => x.SoLuongXuat)
                 .MustAsync(async (viewModel, soLuongTon, id) =>
                 {
                     var kiemTraSLTon = await xuatKhoKhacService.CheckSoLuongTonVatTu(viewModel.VatTuBenhVienId, viewModel.LaVatTuBHYT, viewModel.KhoXuatId, viewModel.SoLuongXuat, viewModel.SoLo, viewModel.HanSuDung);
                     return kiemTraSLTon;
                 })
                 .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));
        }
    }
}
