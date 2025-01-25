using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuGiuongBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuGiuongBenhVienGiaBaoHiemViewModel>))]
    public class DichVuGiuongBenhVienGiaBaoHiemViewModelValidator : AbstractValidator<DichVuGiuongBenhVienGiaBaoHiemViewModel>
    {
        public DichVuGiuongBenhVienGiaBaoHiemViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Gia)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.Gia.Required"))
                .Must((viewModel,d) => viewModel.Gia == null || (viewModel.Gia != null && viewModel.Gia > 0)).WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.Gia.Range"));
            RuleFor(x => x.TiLeBaoHiemThanhToan)
                 .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.TiLeBaoHiemThanhToan.Required"));
            RuleFor(x => x.TuNgay)
                //.MustAsync(async (model, input, s) => (await dichVuKhamBenhService.IsTuNgayBenhVienValid(model.TuNgay, model.Id, model.NhomGiaDichVuKhamBenhBenhVienId).ConfigureAwait(false))).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.TuNgay.NotValidate"))
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.TuNgay.Required"));
        }
    }
}
