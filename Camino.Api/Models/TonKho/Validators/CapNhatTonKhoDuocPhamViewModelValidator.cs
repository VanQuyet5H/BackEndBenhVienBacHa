using Camino.Api.Models.TonKho;
using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.Thuocs;

namespace Camino.Api.Models.TonKho.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CapNhatTonKhoDuocPhamViewModel>))]

    public class CapNhatTonKhoDuocPhamViewModelValidator : AbstractValidator<CapNhatTonKhoDuocPhamViewModel>
    {

        public CapNhatTonKhoDuocPhamViewModelValidator(ILocalizationService localizationService,
            IDuocPhamService thuocBenhVienService,
            IValidator<CapNhatTonKhoDuocPhamChiTietViewModel> capNhatTonKhoDuocPhamChiTietViewModelValidator)
        {
            RuleFor(x => x.SoDangKy)
                //.NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.SoDangKy.Required"))
                //Cập nhật 11/07/2022: sửa maxlength số đăng ký thành 255
                .Length(0, 255).WithMessage(localizationService.GetResource("ThuocBenhVien.SoDangKy.Length"));
                //.MustAsync(async (model, input, a) => !await thuocBenhVienService.KiemTraSoDangKyTonTaiAsync(input, model.DuocPhamId)).WithMessage(localizationService.GetResource("ThuocBenhVien.SoDangKy.Exists"));

            RuleFor(x => x.DuocPhamId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DuocPham.Required"));

            RuleForEach(x => x.CapNhatTonKhoDuocPhamChiTiets).SetValidator(capNhatTonKhoDuocPhamChiTietViewModelValidator);
        }
    }
}
