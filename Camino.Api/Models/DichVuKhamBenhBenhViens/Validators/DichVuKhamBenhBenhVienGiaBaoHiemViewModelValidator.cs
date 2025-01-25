using Camino.Api.Models.DichVuKhamBenh;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DichVuKhamBenhBenhViens;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKhamBenhBenhViens.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuKhamBenhBenhVienGiaBaoHiemViewModel>))]
    public class DichVuKhamBenhBenhVienGiaBaoHiemViewModelValidator : AbstractValidator<DichVuKhamBenhBenhVienGiaBaoHiemViewModel>
    {
        public DichVuKhamBenhBenhVienGiaBaoHiemViewModelValidator(ILocalizationService localizationService, IDichVuKhamBenhBenhVienService dichVuKhamBenhService)
        {
            RuleFor(x => x.Gia)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.Gia.Required"))
                .Must((viewModel, d) => viewModel.Gia == null || (viewModel.Gia != null && viewModel.Gia > 0)).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.Gia.Range"));
            RuleFor(x => x.TiLeBaoHiemThanhToan)
             .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVienGiaBaoHiem.TiLeBaoHiemThanhToan.Required"));
            RuleFor(x => x.TuNgay)
                //.MustAsync(async (model, input, s) => (await dichVuKhamBenhService.IsTuNgayBaoHiemValid(model.TuNgay, model.Id).ConfigureAwait(false))).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.TuNgay.NotValidate"))
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.TuNgay.Required"));
            RuleFor(x => x.DenNgay)
               //.MustAsync(async (model, input, s) => (await dichVuKhamBenhService.IsTuNgayBaoHiemValid(model.TuNgay, model.Id).ConfigureAwait(false)))
                .MustAsync(async (request, ten, id) => {
                    if (request.TuNgay != null && request.DenNgay != null)
                    {
                        if (request.DenNgay < request.TuNgay)
                        {
                            return false;
                        }
                    }
                    return true;
                })
                .WithMessage(localizationService.GetResource("DenNgayNhoHonTuNgay"));
            //.NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.DenNgay.Required"));
            RuleFor(x => x.DenNgay).MustAsync(async (request, ten, id) =>
            {
                if (request.DenNgayRequired == true)
                {
                    if (request.DenNgay == null)
                    {
                        return false;
                    }
                }
                return true;
            }).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.DenNgay.Required"));
        }
    }
}
