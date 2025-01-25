using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhoaPhongNhanVien;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using FluentValidation;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhoDuocPhams.Validators
{

    [TransientDependency(ServiceType = typeof(IValidator<KhoDuocPhamViewModel>))]
    public class KhoDuocPhamViewModelValidator : AbstractValidator<KhoDuocPhamViewModel>
    {
        public KhoDuocPhamViewModelValidator(ILocalizationService localizationService, IKhoDuocPhamService khoDuocPhamService, IKhoaPhongNhanVienService khoaPhongNhanVienService)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Ten)
                .MustAsync(async (model, input, s) => await khoDuocPhamService.CheckIsExistTenKho(model.Ten, model.Id).ConfigureAwait(false)).WithMessage(localizationService.GetResource("Common.Ten.Exists"))
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(p => p.LoaiKho)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Loai.Required"));

            RuleFor(p => p.KhoaPhongId)
                .NotNull().WithMessage(localizationService.GetResource("Kho.KhoDuocPhamVTYT.KhoaPhong.Required"))
                .When(model => model.LoaiKho == EnumLoaiKhoDuocPham.KhoLe);

            RuleFor(x => x.NhanVienIds)
                .NotEmpty().WithMessage(localizationService.GetResource("NhanVien.Id.Required"));
                //.MustAsync(async (model, input, source) => !await khoaPhongNhanVienService.CheckNhanVienExistAsync(model.KhoaPhongId ?? 0, model.NhanVienIds, input)).WithMessage(localizationService.GetResource("IdKhoaPhong.IdNhanVien.Exists"));
        }
    }
}
