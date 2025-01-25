using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using FluentValidation;

namespace Camino.Api.Models.YeuCauKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NghiHuongBHXHViewModel>))]

    public class NghiHuongBHXHViewModelValidator : AbstractValidator<NghiHuongBHXHViewModel>
    {
        public NghiHuongBHXHViewModelValidator(ILocalizationService localizationService, IYeuCauKhamBenhService ycKhamBenhService)
        {
            RuleFor(x => x.ThoiDiemTiepNhan)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.TuNgay.Required"))
              .MustAsync(async (viewModel, ngayTiepNhan, id) =>
              {
                  var kiemTraNgayTiepNhan = await ycKhamBenhService.KiemTraNgayTiepNhan(viewModel.ThoiDiemTiepNhan, viewModel.DenNgay,viewModel.YeuCauKhamBenhId.Value);
                  return kiemTraNgayTiepNhan;
              })
              .WithMessage(localizationService.GetResource("YeuCauKhamBenh.TuNgay.LessThanOrEqualTo"));

            RuleFor(x => x.DenNgay)
             .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DenNgay.Required"))
             .MustAsync(async (viewModel, ngayNghiHuong, id) =>
             {
                 var kiemTraNgayTiepNhan = await ycKhamBenhService.KiemTraDenNgay(viewModel.ThoiDiemTiepNhan, viewModel.DenNgay, viewModel.YeuCauKhamBenhId.Value);
                 return kiemTraNgayTiepNhan;
             })
             .WithMessage(localizationService.GetResource("YeuCauKhamBenh.DenNgay.LessThanOrEqualTo"));

            RuleFor(x => x.PhuongPhapDieuTriNghiHuongBHYT)
              .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.PhuongPhapDieuTriNghiHuongBHYT.Required"));

            RuleFor(x => x.ICDChinhNghiHuongBHYT)
                         .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.ICDChinhNghiHuongBHYT.Required"));
        }
    }
}
