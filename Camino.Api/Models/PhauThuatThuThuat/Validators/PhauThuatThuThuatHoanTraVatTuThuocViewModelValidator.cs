using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.PhauThuatThuThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhauThuatThuThuatHoanTraVatTuThuocViewModel>))]
    public class PhauThuatThuThuatHoanTraVatTuThuocViewModelValidator : AbstractValidator<PhauThuatThuThuatHoanTraVatTuThuocViewModel>
    {
        public PhauThuatThuThuatHoanTraVatTuThuocViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService, IValidator<PhauThuatThuThuatHoanTraVatTuThuocChiTietViewModel> validator)
        {
            RuleFor(x => x.NhanVienYeuCauId).NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.NhanVienYeuCauId.Required"));

            RuleFor(x => x.NgayYeuCau)
              .NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.NgayYeuCau.Required"))
              .MustAsync(async (viewModel, ngayNghiHuong, id) =>
              {
                  var kiemTraNgayYeuCau = await dieuTriNoiTruService.KiemTraNgayTra(viewModel.NgayYeuCau);
                  return kiemTraNgayYeuCau;
              })
              .WithMessage(localizationService.GetResource("DieuTriNoiTru.NgayYeuCau.LessThanOrEqualTo"));

            RuleForEach(p => p.YeuCauDuocPhamVatTuBenhViens).SetValidator(validator);
            //RuleFor(x => x.SoLuongTra)
            //    .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
            //    .MustAsync(async (viewModel, soLuongTon, id) =>
            //    {
            //        var kiemTraSLTon = await dieuTriNoiTruService.CheckSoLuongTra(viewModel.SoLuong, viewModel.SoLuongDaTra, viewModel.SoLuongTra);
            //        return kiemTraSLTon;
            //    })
            //    .WithMessage(localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
        }
    }

    [TransientDependency(ServiceType = typeof(IValidator<PhauThuatThuThuatHoanTraVatTuThuocChiTietViewModel>))]
    public class PhauThuatThuThuatHoanTraVatTuThuocChiTietViewModelValidator : AbstractValidator<PhauThuatThuThuatHoanTraVatTuThuocChiTietViewModel>
    {
        public PhauThuatThuThuatHoanTraVatTuThuocChiTietViewModelValidator(ILocalizationService localizationService)
        {
            //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
            RuleFor(x => x.SoLuongTra)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"));
            //RuleFor(x => x.SoLuongTra)
            //    .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
            //    .MustAsync(async (viewModel, soLuongTon, id) =>
            //    {
            //        var kiemTraSLTon = await dieuTriNoiTruService.CheckSoLuongTra(viewModel.SoLuong, viewModel.SoLuongDaTra, viewModel.SoLuongTra);
            //        return kiemTraSLTon;
            //    })
            //    .WithMessage(localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"));
        }
    }
}
