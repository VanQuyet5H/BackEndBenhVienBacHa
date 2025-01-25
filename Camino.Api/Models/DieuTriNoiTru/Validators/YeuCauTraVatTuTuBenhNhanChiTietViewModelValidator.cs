using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    //[TransientDependency(ServiceType = typeof(IValidator<YeuCauTraVatTuTuBenhNhanChiTietViewModel>))]

    //public class YeuCauTraVatTuTuBenhNhanChiTietViewModelValidator : AbstractValidator<YeuCauTraVatTuTuBenhNhanChiTietViewModel>
    //{
    //    public YeuCauTraVatTuTuBenhNhanChiTietViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService, IValidator<YcVTBvVo> ycVTBvVoValidator)
    //    {
    //        RuleFor(x => x.NhanVienYeuCauId).NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.NhanVienYeuCauId.Required"));

    //        RuleFor(x => x.NgayYeuCau)
    //          .NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.NgayYeuCau.Required"))
    //          .MustAsync(async (viewModel, ngayNghiHuong, id) =>
    //          {
    //              var kiemTraNgayYeuCau = await dieuTriNoiTruService.KiemTraNgayTra(viewModel.NgayYeuCau);
    //              return kiemTraNgayYeuCau;
    //          })
    //          .WithMessage(localizationService.GetResource("DieuTriNoiTru.NgayYeuCau.LessThanOrEqualTo"));

    //        //RuleFor(x => x.SoLuongTra)
    //        //    .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
    //        //    .MustAsync(async (viewModel, soLuongTon, id) =>
    //        //    {
    //        //        var kiemTraSLTon = await dieuTriNoiTruService.CheckSoLuongTra(viewModel.SoLuong, viewModel.SoLuongDaTra, viewModel.SoLuongTra);
    //        //        return kiemTraSLTon;
    //        //    })
    //        //    .WithMessage(localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
    //        RuleForEach(x => x.YeuCauVatTuBenhViens).SetValidator(ycVTBvVoValidator);

    //    }
    //}
}
