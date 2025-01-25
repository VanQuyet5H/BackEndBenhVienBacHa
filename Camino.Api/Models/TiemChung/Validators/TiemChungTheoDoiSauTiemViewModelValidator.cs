using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.TiemChung;
using FluentValidation;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiemChungTheoDoiSauTiemViewModel>))]
    public class TiemChungTheoDoiSauTiemViewModelValidator : AbstractValidator<TiemChungTheoDoiSauTiemViewModel>
    {
        public TiemChungTheoDoiSauTiemViewModelValidator(ILocalizationService localizationService, ITiemChungService tiemChungService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.ThoiDiemTheoDoiSauTiem)
                .Must((model, input, s) => tiemChungService.KiemTraThoiGianTheoDoiTiemChungVoiLanTiemKhac(input, model.Id)).WithMessage(localizationService.GetResource("TiemChung.TheoDoi.ThoiDiemTheoDoi.LessThan.TiemChung"));
                //.Must((model, input, s) => tiemChungService.KiemTraThoiGianTheoDoiTiemChungVoiKhamSangLoc(input, model.Id)).WithMessage(localizationService.GetResource("TiemChung.TheoDoi.ThoiDiemTheoDoi.LessThan.ThoiDiemHoanThanhKhamSangLoc"));

            RuleFor(p => p.TiemChungThongTinPhanUngSauTiem.TinhTrangHienTai)
                .NotNull().WithMessage(localizationService.GetResource("TiemChung.TheoDoi.TinhTrangHienTai.Required"))
                    .When(p => p.LoaiPhanUngSauTiem == LoaiPhanUngSauTiem.TaiBienNang);
        }
    }
}
