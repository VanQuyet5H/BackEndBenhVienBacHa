using Camino.Api.Models.TonKho;
using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.TonKhos;

namespace Camino.Api.Models.TonKho.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CapNhatTonKhoDuocPhamChiTietViewModel>))]

    public class CapNhatTonKhoDuocPhamChiTietViewModelValidator : AbstractValidator<CapNhatTonKhoDuocPhamChiTietViewModel>
    {
        public CapNhatTonKhoDuocPhamChiTietViewModelValidator(ILocalizationService localizationService, ITonKhoService tonKhoService)
        {

            RuleFor(x => x.SoLo)
                .NotEmpty().WithMessage(localizationService.GetResource("nhapkhochitiet.solo.required"))
                .When(z => z.Loai == 1);

            RuleFor(x => x.SoLuong)
              .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
              .MustAsync(async (viewModel, soLuongTon, id) =>
              {
                  var kiemTraSLTon = await tonKhoService.KiemTraSoLuongHopLe(viewModel.SoLuong, viewModel.SoLuongXuat);
                  return kiemTraSLTon;
              })
              .WithMessage(localizationService.GetResource("VatTu.SoLuongNhap.Valid"))
              .When(z => z.Loai == 1);


            RuleFor(x => x.DonGiaNhap)
              .NotEmpty().WithMessage(localizationService.GetResource("Common.DonGiaNhap.Required"))
              .When(z => z.Loai == 1);


            RuleFor(x => x.HanSuDung)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapkhochitiet.hansudung.required"))
             .When(z => z.Loai == 1);


            RuleFor(x => x.TiLeBHYTThanhToan)
            .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVienGiaBaoHiem.TiLeBaoHiemThanhToan.Required"))
            .When(z => z.LaVatTuBHYT && z.Loai == 1);

            RuleFor(x => x.VAT)
           .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.VAT.Required"))
           .When(z => !z.LaVatTuBHYT && z.Loai == 1);

            RuleFor(x => x.DuocPhamBenhVienPhanNhomId)
                .NotEmpty().WithMessage(localizationService.GetResource("nhapkhochitiet.DuocPhamBenhVienPhanNhomId.required"))
                .When(z => z.Loai == 1);
        }
    }
}
