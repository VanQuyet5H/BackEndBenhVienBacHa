using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;
using FluentValidation;


namespace Camino.Api.Models.YeuCauTiepNhan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NghiHuongBHXHTiepNhanViewModel>))]
    public class NghiHuongBHXHTiepNhanViewModelValidator : AbstractValidator<NghiHuongBHXHTiepNhanViewModel>
    {

        public NghiHuongBHXHTiepNhanViewModelValidator(ILocalizationService localizationService, IDanhSachChoKhamService danhSachChoKhamService)
        {
            RuleFor(x => x.ThoiDiemTiepNhan)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.TuNgay.Required"))
              .MustAsync(async (viewModel, ngayTiepNhan, id) =>
              {
                  var kiemTraNgayTiepNhan = await danhSachChoKhamService.KiemTraNgayTiepNhan(viewModel.ThoiDiemTiepNhan, viewModel.DenNgay, viewModel.YeuCauTiepNhanId);
                  return kiemTraNgayTiepNhan;
              })
              .WithMessage(localizationService.GetResource("YeuCauKhamBenh.TuNgay.LessThanOrEqualTo"));

            RuleFor(x => x.DenNgay)
             .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DenNgay.Required"))
             .MustAsync(async (viewModel, ngayNghiHuong, id) =>
             {
                 var kiemTraNgayTiepNhan = await danhSachChoKhamService.KiemTraDenNgay(viewModel.ThoiDiemTiepNhan, viewModel.DenNgay, viewModel.YeuCauTiepNhanId);
                 return kiemTraNgayTiepNhan;
             })
             .WithMessage(localizationService.GetResource("YeuCauKhamBenh.DenNgay.LessThanOrEqualTo"));
        }
    }
}
