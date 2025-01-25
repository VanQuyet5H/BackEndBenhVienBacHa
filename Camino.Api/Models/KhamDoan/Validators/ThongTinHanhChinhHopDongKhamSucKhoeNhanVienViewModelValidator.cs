using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.KhamDoan;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel>))]
    public class ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModelValidator : AbstractValidator<ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel>
    {
        public ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModelValidator(ILocalizationService localizationService, IKhamDoanService _khamDoanService, IValidator<TiepNhanDichVuChiDinhViewModel> dichVuValidator)
        {
            RuleFor(x => x.MaNhanVien)
                .MustAsync(async (model,input,f)=> !await _khamDoanService.KiemTraTrungMaNhanVienTheoHopDongAsync(model.HopDongKhamSucKhoeId, model.Id, input))
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.MaNhanVien.IsExists"));
            RuleFor(x => x.HoTen).NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.HoTen.Required"));
            RuleFor(x => x.NgayThangNamSinh)
                .Must((model, input) => input != null || model.NamSinh != null)
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.NgayThangNamSinh.Required"));
            RuleFor(x => x.GioiTinh).NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.GioiTinh.Required"));
            RuleFor(x => x.SoChungMinhThu).NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.SoChungMinhThu.Required"));
            RuleFor(x => x.Email)
                .Must((model, s) => CommonHelper.IsMailValid(model.Email))
                .WithMessage(localizationService.GetResource("Common.WrongEmailFormat"));

            RuleForEach(x => x.DichVuChiDinhThems).SetValidator(dichVuValidator);
            RuleForEach(x => x.DichVuChiDinhTrongGois).SetValidator(dichVuValidator);
        }
    }
}
