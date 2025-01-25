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
    [TransientDependency(ServiceType = typeof(IValidator<DichVuKhamBenhBenhVienGiaBenhVienViewModel>))]
    public class DichVuKhamBenhBenhVienGiaBenhVienViewModelValidator : AbstractValidator<DichVuKhamBenhBenhVienGiaBenhVienViewModel>
    {
        public DichVuKhamBenhBenhVienGiaBenhVienViewModelValidator(ILocalizationService localizationService, IDichVuKhamBenhBenhVienService dichVuKhamBenhService)
        {
            //|| (viewModel.Gia != null && viewModel.Gia > 0) Giá  khách hàng muốn nhâp giá === 0
            RuleFor(x => x.Gia)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.Gia.Required"));
                //.Must((viewModel, d) => viewModel.Gia == null).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.Gia.Range"));
            RuleFor(x => x.NhomGiaDichVuKhamBenhBenhVienId)
                 .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.NhomGiaDichVuKhamBenhBenhVienId.Required"));
            RuleFor(x => x.TuNgay)
                //.MustAsync(async (model, input, s) => (await dichVuKhamBenhService.IsTuNgayBenhVienValid(model.TuNgay, model.Id, model.NhomGiaDichVuKhamBenhBenhVienId).ConfigureAwait(false))).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.TuNgay.NotValidate"))
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.TuNgay.Required"));
            //RuleFor(x => x.DenNgay)
            //   .MustAsync(async (model, input, s) => (await dichVuKhamBenhService.KiemTraNgay(model.TuNgay, model.DenNgay).ConfigureAwait(false))).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.TuNgay.NotValidate"))
            //   .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.DenNgay.Required"));

            RuleFor(x => x.DenNgay).MustAsync(async (request, ten, id) =>
            {
                if (request.TuNgay != null && request.DenNgay != null)
                {
                    if (request.DenNgay < request.TuNgay)
                    {
                        return false;
                    }
                }
                return true;
            }).WithMessage(localizationService.GetResource("DenNgayNhoHonTuNgay"));
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
