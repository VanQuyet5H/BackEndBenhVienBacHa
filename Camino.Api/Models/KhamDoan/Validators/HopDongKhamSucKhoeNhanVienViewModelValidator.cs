using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamDoan;
using Camino.Services.Localization;
using FluentValidation;
using System;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HopDongKhamSucKhoeNhanVienViewModel>))]
    public class HopDongKhamSucKhoeNhanVienViewModelValidator : AbstractValidator<HopDongKhamSucKhoeNhanVienViewModel>
    {
        public HopDongKhamSucKhoeNhanVienViewModelValidator(ILocalizationService localizationService, IKhamDoanService khamDoanService,
            IValidator<NgheCongViecTruocDay> ngheCongViecTruocDayViewModelValidator, IValidator<TienSuBenh> tienSuBenhViewModelValidator)
        {
            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.HopDongKhamSucKhoeHoTen.Required"));

            RuleFor(x => x.GioiTinh)
                           .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.HopDongKhamSucKhoeGioiTinh.Required"));

            RuleFor(p => p.MaNhanVien)
               .MustAsync(async (model, input, s) => await khamDoanService
                   .KiemTraTrungMaNhanVien(model.Id, model.HopDongKhamSucKhoeId, model.MaNhanVien).ConfigureAwait(false))
               .WithMessage(localizationService.GetResource("KhamDoan.HopDongKhamSucKhoeMaNhanVien.Required")).When(c => c.MaNhanVien != null);

            RuleFor(p => p.STTNhanVien)
               .MustAsync(async (model, input, s) => await khamDoanService
                   .KiemTraTrungSTTTrongHopDongKham(model.Id, model.HopDongKhamSucKhoeId, model?.STTNhanVien).ConfigureAwait(false))
               .WithMessage(localizationService.GetResource("KhamDoan.STTNhanVien.Required")).When(c => c.STTNhanVien != null);

            RuleFor(p => p.SoChungMinhThu)
             .MustAsync(async (model, input, s) => await khamDoanService
                 .KiemTraTrungSoChungMinhThu(model.Id, model.HopDongKhamSucKhoeId, model.SoChungMinhThu).ConfigureAwait(false))
             .WithMessage(localizationService.GetResource("KhamDoan.HopDongKhamSucSoChungMinhThu.Required")).When(c => c.SoChungMinhThu != null);

            //RuleFor(x => x.NamSinh)
            //  .Must((model, s) => model.NamSinh <= DateTime.Now.Year)
            //  .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinhMoreThanNow.Required"))
            //  .When(x => x.NamSinh != null);

            //RuleFor(x => x.NgayThangNamSinh).Must((model, s) => model.NgayThangNamSinh.GetValueOrDefault() <= DateTime.Now.Date)
            //   .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.DatetimeNow"))
            //   .When(x => x.NgayThangNamSinh != null);

            //RuleFor(x => x.NgayThangNamSinh)
            //    .Must((model, input) => input != null || model.NamSinh != null)
            //    .WithMessage(localizationService.GetResource("KhamDoan.HopDongKhamSucKhoeNgaySinh.Required"));


            RuleFor(x => x.NamSinh)
               .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinh.Required"))
               .When(x => x.NgayThangNamSinh != null)
               ;

            RuleFor(x => x.NamSinh)
                .Must((model, s) => model.NamSinh <= DateTime.Now.Year)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinhMoreThanNow.Required"))
                .When(x => x.NamSinh != null);

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.Required"))
                .When(x => x.NamSinh == null);

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinhOrNamSinh.Required"))
                .When(x => x.NamSinh == null);


            RuleFor(x => x.NgayThangNamSinh).Must((model, s) => model.NgayThangNamSinh.GetValueOrDefault() <= DateTime.Now.Date)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.DatetimeNow"))
                .When(x => x.NgayThangNamSinh != null);


            RuleFor(x => x.SoChungMinhThu)
                   .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.HopDongKhamSucKhoeCMND.Required"));

            RuleFor(x => x.GoiKhamSucKhoeId)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.HopDongKhamSucKhoeGoiKhamSucKhoeId.Required"));

            RuleForEach(x => x.NgheCongViecTruocDays).SetValidator(ngheCongViecTruocDayViewModelValidator);
            RuleForEach(x => x.TienSuBenhs).SetValidator(tienSuBenhViewModelValidator);
        }
    }
}
