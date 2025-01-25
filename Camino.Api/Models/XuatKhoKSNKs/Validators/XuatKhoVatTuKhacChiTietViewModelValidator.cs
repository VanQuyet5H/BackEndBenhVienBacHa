using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.XuatKhoKSNK;
using Camino.Services.Localization;
using Camino.Services.XuatKhoKhacKSNKs;
using FluentValidation;

namespace Camino.Api.Models.XuatKhoKSNKs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoKhacKSNKChiTietVo>))]

    public class XuatKhoVatTuKhacChiTietViewModelValidator : AbstractValidator<XuatKhoKhacKSNKChiTietVo>
    {
        public XuatKhoVatTuKhacChiTietViewModelValidator(ILocalizationService localizationService)
        {
            //check tồn kho khi duyệt xuất
            //RuleFor(x => x.SoLuongXuat)
            //     .MustAsync(async (viewModel, soLuongTon, id) =>
            //     {
            //         var kiemTraSLTon = await xuatKhoKhacService.CheckSoLuongTonVatTu(viewModel.VatTuBenhVienId, viewModel.LaVatTuBHYT, viewModel.KhoXuatId, viewModel.SoLuongXuat, viewModel.SoLo, viewModel.HanSuDung);
            //         return kiemTraSLTon;
            //     })
            //     .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));
        }
    }
}
