using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhauThuatThuThuat;
using FluentValidation;

namespace Camino.Api.Models.PhauThuatThuThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhauThuatThuThuatLichSuThuePhongViewModel>))]
    public class PhauThuatThuThuatLichSuThuePhongViewModelValidators : AbstractValidator<PhauThuatThuThuatLichSuThuePhongViewModel>
    {
        public PhauThuatThuThuatLichSuThuePhongViewModelValidators(ILocalizationService localizationService, IPhauThuatThuThuatService phauThuatThuThuatService)
        {
            RuleFor(x => x.YeuCauDichVuKyThuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("PhauThuatThuThuatLichSuThuePhong.YeuCauDichVuKyThuatId.Required"));
            RuleFor(x => x.CauHinhThuePhongId)
                .NotEmpty().When(x => x.CoThuePhong == true).WithMessage(localizationService.GetResource("PhauThuatThuThuatThuePhong.CauHinhThuePhongId.Required"));
            RuleFor(x => x.ThoiDiemBatDau)
                .NotEmpty().When(x => x.CoThuePhong == true).WithMessage(localizationService.GetResource("PhauThuatThuThuatThuePhong.ThoiDiemBatDau.Required"));
            RuleFor(x => x.ThoiDiemBatDau)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MustAsync(async (model, input, f) => await phauThuatThuThuatService.KiemTraThoiGianBatDauThuePhongTheoTiepNhanAsync(model.YeuCauTiepNhanId, input))
                .When(x => x.CoThuePhong == true && x.ThoiDiemBatDau != null)
                .WithMessage(localizationService.GetResource("PhauThuatThuThuatThuePhong.ThoiDiemBatDau.LessThanTiepNhan"))

                .Must(x => phauThuatThuThuatService.KiemTraThoiGianThuePhongVoiNgayHienTai(x))
                .When(x => x.CoThuePhong == true && x.ThoiDiemBatDau != null)
                .WithMessage(localizationService.GetResource("PhauThuatThuThuatThuePhong.ThoiDiemBatDau.GreaterThanNgayHomSau"));


            RuleFor(x => x.ThoiDiemKetThuc)
                .NotEmpty().When(x => x.CoThuePhong == true).WithMessage(localizationService.GetResource("PhauThuatThuThuatThuePhong.ThoiDiemKetThuc.Required"));
            RuleFor(x => x.ThoiDiemKetThuc)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(x => x.ThoiDiemBatDau)
                .When(x => x.CoThuePhong == true && x.ThoiDiemBatDau != null && x.ThoiDiemKetThuc != null)
                .WithMessage(localizationService.GetResource("PhauThuatThuThuatThuePhong.ThoiDiemKetThuc.LessThenBatDau"))

                .Must(x => phauThuatThuThuatService.KiemTraThoiGianThuePhongVoiNgayHienTai(x))
                .When(x => x.CoThuePhong == true && x.ThoiDiemBatDau != null)
                .WithMessage(localizationService.GetResource("PhauThuatThuThuatThuePhong.ThoiDiemKetThuc.GreaterThanNgayHomSau"));
        }
    }
}
