using System;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.QuanHeThanNhan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThongTinHuyPhieuViewModel>))]
    public class ThongTinHuyPhieuViewModelValidator : AbstractValidator<ThongTinHuyPhieuViewModel>
    {
        public ThongTinHuyPhieuViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ThuHoiPhieu).NotNull().WithMessage("Vui lòng chọn thu hồi phiếu.").When(p => p.ThuHoiPhieu != null);

            RuleFor(x => x.NguoiThuHoiId).NotEmpty().WithMessage("Vui lòng chọn người thu hồi.").When(p => p.ThuHoiPhieu == true);
            RuleFor(x => x.ThoiGianThuHoi).NotEmpty().WithMessage("Vui lòng chọn ngày thu hồi.").When(p => p.ThuHoiPhieu == true);

            RuleFor(x => x.NguoiThuHoiId).NotEmpty().WithMessage("Vui lòng chọn người thu hồi.").When(p => p.KiemTraThuHoi == true);
            RuleFor(x => x.ThoiGianThuHoi).NotEmpty().WithMessage("Vui lòng chọn ngày thu hồi.").When(p => p.KiemTraThuHoi == true);


            RuleFor(x => x.ThoiGianThuHoi).NotNull().WithMessage("Vui lòng chọn ngày thu hồi.")
                .Must((model, s) => (model.ThoiGianThuHoi != null && model.ThoiGianThuHoi < DateTime.Now) || model.ThoiGianThuHoi == null)
                .WithMessage("Ngày thu phiếu nhỏ hơn hoặc bằng ngày hiện tại.").When(p => p.ThoiGianThuHoi != null);
            
            RuleFor(x => x.LyDo).NotNull().WithMessage("Vui lòng chọn lý do.");
        }
    }
}