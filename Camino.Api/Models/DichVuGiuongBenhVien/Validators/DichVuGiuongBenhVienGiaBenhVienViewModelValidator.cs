using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuGiuongBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuGiuongBenhVienGiaBenhVienViewModel>))]
    public class DichVuGiuongBenhVienGiaBenhVienViewModelValidator : AbstractValidator<DichVuGiuongBenhVienGiaBenhVienViewModel>
    {
        public DichVuGiuongBenhVienGiaBenhVienViewModelValidator(ILocalizationService localizationService)
        {
            //|| (viewModel.Gia != null && viewModel.Gia > 0) Giá  khách hàng muốn nhâp giá === 0
            RuleFor(x => x.Gia)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.Gia.Required"));
               // .Must((viewModel, d) => viewModel.Gia == null).WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.Gia.Range"));
            RuleFor(x => x.NhomGiaDichVuGiuongBenhVienId)
                 .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.NhomGiaDichVuKyThuatBenhVienId.Required"));
            RuleFor(x => x.TuNgay)
                //.MustAsync(async (model, input, s) => (await dichVuKhamBenhService.IsTuNgayBenhVienValid(model.TuNgay, model.Id, model.NhomGiaDichVuKhamBenhBenhVienId).ConfigureAwait(false))).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.TuNgay.NotValidate"))
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.TuNgay.Required"));
        }
    }
}
