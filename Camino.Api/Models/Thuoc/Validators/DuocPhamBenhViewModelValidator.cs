using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.DuocPhamBenhVien;

namespace Camino.Api.Models.Thuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuocPhamBenhVienModel>))]
    public class DuocPhamBenhViewModelValidator : AbstractValidator<DuocPhamBenhVienModel>
    {
        public DuocPhamBenhViewModelValidator(ILocalizationService localizationService, IDuocPhamBenhVienService duocPhamBenhVienService)
        {
            RuleFor(x => x.MaDuocPhamBenhVien)
                .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.Required"))

                // BVHD-3454
                // mã bệnh viện tối thiểu 7 ký tự
                .Must((viewModel, input, d) => string.IsNullOrEmpty(input) || (!string.IsNullOrEmpty(input) && input.Length >= 7))
                .WithMessage(localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.Length"))
                .MustAsync(async (viewModel, input, d) => !await duocPhamBenhVienService.KiemTraTrungMaDuocPhamBenhVienAsync(viewModel.Id, input))
                .WithMessage(localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.IsExists"));

            RuleFor(x => x.DuocPhamBenhVienPhanNhomId)
               .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId.Required"));
        }
    }
}
