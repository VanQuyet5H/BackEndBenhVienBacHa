using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using Camino.Services.TiepNhanBenhNhan;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThongTinDoiTuongTiepNhanViewModel>))]
    public class ThongTinDoiTuongTiepNhanViewModelValidator : AbstractValidator<ThongTinDoiTuongTiepNhanViewModel>
    {
        public ThongTinDoiTuongTiepNhanViewModelValidator(ILocalizationService localizationService, ITiepNhanBenhNhanService tiepNhanBenhNhanService, IValidator<NoiTruYeuCauTiepNhanTheBHYTViewModel> theBhytValidator,
            IDieuTriNoiTruService dieuTriNoiTruService)
        {
            //RuleFor(x => x.BHYTMaSoThe)
            //    .Must((model, input) => model.CoBHYT != true || (model.CoBHYT == true && !string.IsNullOrEmpty(input)))
            //    .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTMaSoThe.Required"));
            //RuleFor(x => x.BHYTMaDKBD)
            //    .Must((model, input) => model.CoBHYT != true || (model.CoBHYT == true && !string.IsNullOrEmpty(input)))
            //    .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTMaDKBD.Required"));
            //RuleFor(x => x.NoiDangKyBHYT)
            //    .Must((model, input) => model.CoBHYT != true || (model.CoBHYT == true && !string.IsNullOrEmpty(input)))
            //    .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.NoiDangKyBHYT.Required"));
            //RuleFor(x => x.BHYTMucHuong)
            //    .Must((model, input) => model.CoBHYT != true || (model.CoBHYT == true && input != null))
            //    .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTMucHuong.Required"));
            //RuleFor(x => x.BHYTDiaChi)
            //    .Must((model, input) => model.CoBHYT != true || (model.CoBHYT == true && !string.IsNullOrEmpty(input)))
            //    .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTDiaChi.Required"));
            //RuleFor(x => x.BHYTNgayHieuLuc)
            //    .Must((model, input) => model.CoBHYT != true || (model.CoBHYT == true && input != null))
            //    .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTNgayHieuLuc.Required"));
            //RuleFor(x => x.BHYTNgayHetHan)
            //    .Must((model, input) => model.CoBHYT != true || (model.CoBHYT == true && input != null))
            //    .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTNgayHetHan.Required"));


            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.HoTen.Required"));
            RuleFor(x => x.NgayThangNamSinh)
                .Must((model, input) => input != null || model.NamSinh != null)
                .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.NgayThangNamSinh.Required"));


            RuleFor(x => x.NguoiLienHeHoTen)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.HoTen.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result);

            RuleFor(x => x.NguoiLienHeQuanHeNhanThanId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeQuanHeNhanThanId.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result);

            RuleFor(x => x.NguoiLienHeSoDienThoai)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeSoDienThoai.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result);

            RuleFor(x => x.Email)
                .Must((model, s) => CommonHelper.IsMailValid(model.Email))
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.Emai.WrongEmail"));

            RuleFor(x => x.NguoiLienHeEmail)
                .Must((model, s) => CommonHelper.IsMailValid(model.NguoiLienHeEmail))
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeEmail.WrongEmail"));

            RuleForEach(x => x.YeuCauTiepNhanTheBHYTs)
                .SetValidator(theBhytValidator);

            RuleFor(x => x.YeuCauGoiDichVuId)
                .MustAsync(async (viewModel, input, f) => !await dieuTriNoiTruService.KiemTraYeuCauGoiDichVuDaSuDungAsync(input, viewModel.BenhNhanId, false, viewModel.YeuCauTiepNhanMeId))
                .WithMessage(localizationService.GetResource("BenhAnSoSinh.YeuCauGoiDichVuId.DaSuDung"));
        }
    }
}
