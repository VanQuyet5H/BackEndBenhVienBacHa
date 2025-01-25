using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Camino.Services.TiepNhanBenhNhan;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhThongTinDoiTuongViewModel>))]
    public class KhamBenhThongTinDoiTuongViewModelValidator : AbstractValidator<KhamBenhThongTinDoiTuongViewModel>
    {
        public KhamBenhThongTinDoiTuongViewModelValidator(ILocalizationService localizationService, ITiepNhanBenhNhanService tiepNhanBenhNhanService)
        {
            RuleFor(x => x.BHYTMaDKBD)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTMaDKBD.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTMucHuong)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTMucHuong.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTDiaChi)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTDiaChi.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTNgayHieuLuc)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTNgayHieuLuc.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTNgayHetHan)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTNgayHetHan.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTNgayHetHan)
                .Must((model, s) => model.BHYTNgayHieuLuc <= model.BHYTNgayHetHan)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTNgayHetHan.MoreThanBHYTNgayHieuLuc")).When(x => (x.TuNhap == true || x.CoBHYT == true) && x.BHYTNgayHetHan != null && x.BHYTNgayHieuLuc != null);

            RuleFor(x => x.BHYTNgayHetHan)
                .Must((model, s) => model.BHYTNgayHetHan.GetValueOrDefault().Date >= DateTime.Now.Date)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTNgayHetHan.MoreThanDatetimeNow")).When(x => (x.TuNhap == true || x.CoBHYT == true) && x.BHYTNgayHetHan != null);

            //RuleFor(x => x.LyDoTiepNhanId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.LyDoTiepNhan.Required"));

            RuleFor(x => x.BHYTMaSoThe)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTMaSoThe.Required")).When(x => x.CoBHYT == true);

            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.HoTen.Required"));

            RuleFor(x => x.NamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinh.Required"))
                .When(x => x.NgayThangNamSinh != null)
                ;

            RuleFor(x => x.NamSinh)
                .Must((model, s) => model.NamSinh <= DateTime.Now.Year)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinhMoreThanNow.Required"))
                .When(x => x.NamSinh != null)
                ;

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.Required"))
                .When(x => x.CoBHYT == true && x.NamSinh == null)
                ;

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinhOrNamSinh.Required"))
                .When(x => x.CoBHYT != true && x.NamSinh == null)
                ;


            RuleFor(x => x.NgayThangNamSinh).Must((model, s) => model.NgayThangNamSinh.GetValueOrDefault() <= DateTime.Now.Date)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.DatetimeNow"))
                .When(x => x.NgayThangNamSinh != null);

            RuleFor(x => x.Email)
                .Length(0, 200).WithMessage(localizationService.GetResource("TiepNhanBenhNhan.Email.Range"))
                .Must((model, s) => CommonHelper.IsMailValid(model.Email))
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.Emai.WrongEmail"));

            RuleFor(x => x.NguoiLienHeEmail)
                .Length(0, 200).WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeEmail.Range"))
                .Must((model, s) => CommonHelper.IsMailValid(model.NguoiLienHeEmail))
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeEmail.WrongEmail"));

            RuleFor(x => x.NguoiLienHeHoTen)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeHoTen.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result)
                ;

            RuleFor(x => x.NguoiLienHeQuanHeNhanThanId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeQuanHeNhanThanId.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result)
                ;

            RuleFor(x => x.NguoiLienHeSoDienThoai)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeSoDienThoai.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result);
        }
    }
}
