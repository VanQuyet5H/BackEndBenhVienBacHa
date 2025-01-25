//using Camino.Core.DependencyInjection.Attributes;
//using Camino.Services.DieuTriNoiTru;
//using Camino.Services.Localization;
//using FluentValidation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;


//namespace Camino.Api.Models.PhieuSoKet15NgayDieuTri.Validators
//{
//    //[TransientDependency(ServiceType = typeof(IValidator<CheckValidationForPhieuSoKet15NgayDieuTriViewModel>))]
//    //public class PhieuSoKet15NgayDieuTriViewModelValidator : AbstractValidator<CheckValidationForPhieuSoKet15NgayDieuTriViewModel>
//    //{
//    //    public PhieuSoKet15NgayDieuTriViewModelValidator(ILocalizationService localizationService,IDieuTriNoiTruService dieuTriNoiTruService)
//    //    {

//    //      //  RuleFor(x => x.TuNgay)
//    //      //      .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.TuNgay.Required"));
//    //      //  RuleFor(x => x.DenNgay)
//    //      //    .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.DenNgay.Required"))
//    //      //    .MustAsync(async (viewModel, ngayTiepNhan, id) =>
//    //      //    {
//    //      //        var kiemTraNgayTiepNhan = await dieuTriNoiTruService.KiemTraNgay(viewModel.TuNgay, viewModel.DenNgay);
//    //      //        return kiemTraNgayTiepNhan;
//    //      //    })
//    //      //    .WithMessage(localizationService.GetResource("HoSoKhac.DenNgay.LessThanOrEqualTo"));
//    //      //  RuleFor(x => x.NgayThucHien)
//    //      //   .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayThucHien.Required"));
//    //      //  RuleFor(x => x.BSDieuTri)
//    //      //  .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.BSDieuTri.Required"));
//    //      //  RuleFor(x => x.TruongKhoa)
//    //      //.NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.TruongKhoa.Required"));
//    //    }
//    //}
//}
