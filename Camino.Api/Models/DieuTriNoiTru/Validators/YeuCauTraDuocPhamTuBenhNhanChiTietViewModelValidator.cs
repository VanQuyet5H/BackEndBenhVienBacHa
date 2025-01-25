using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    //[TransientDependency(ServiceType = typeof(IValidator<YeuCauTraDuocPhamTuBenhNhanChiTietViewModel>))]

    //public class YeuCauTraDuocPhamTuBenhNhanChiTietViewModelValidator : AbstractValidator<YeuCauTraDuocPhamTuBenhNhanChiTietViewModel>
    //{
    //    public YeuCauTraDuocPhamTuBenhNhanChiTietViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService, IValidator<YcDuocPhamBvVo> ycDuocPhamBvVoValidator)
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

    //        //RuleForEach(x => x.YeuCauDuocPhamBenhViens).SetValidator(ycDuocPhamBvVoValidator);

    //    }
    //}
}
