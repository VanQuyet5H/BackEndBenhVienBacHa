using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DuocPhamBenhVien;
using Camino.Services.Localization;
using Camino.Services.Thuocs;
using FluentValidation;

namespace Camino.Api.Models.DuocPhamBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuocPhamBenhVienViewModel>))]
    public class DuocPhamBenhVienViewModelValidator : AbstractValidator<DuocPhamBenhVienViewModel>
    {
        public DuocPhamBenhVienViewModelValidator(ILocalizationService localizationService, IDuocPhamService duocPhamService, IDuocPhamBenhVienService duocPhamBenhVienService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.MaDuocPhamBenhVien)
               .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.Required"))

               // BVHD-3454
               // mã bệnh viện tối thiểu 7 ký tự
               .Must((viewModel, input, d) => string.IsNullOrEmpty(input) || (!string.IsNullOrEmpty(input) && input.Length >= 7))
                    .WithMessage(localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.Length"))
               .MustAsync(async (viewModel,input,d) => !await duocPhamBenhVienService.KiemTraTrungMaDuocPhamBenhVienAsync(viewModel.Id, input))
                    .WithMessage(localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.IsExists"));

            RuleFor(x => x.DuocPhamBenhVienPhanNhomChaId)
              .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.LoaiThuocHoacHoatChat.Reqiured"));

            RuleFor(x => x.DuongDungId)
               .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DuongDung.Required"));

            RuleFor(x => x.DonViTinhId)
              .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DonViTinh.Required"));


            RuleFor(x => x.SoDangKy)
                .Must((model, input, f) => duocPhamBenhVienService.KiemTraNhomDuocPhamBatBuocNhapSoDangKy(input, model.DuocPhamBenhVienPhanNhomId))
                .WithMessage(localizationService.GetResource("ThuocBenhVien.SoDangKy.Required"));
        }
    }
}