using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.QuayThuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThuTienMuaDuocPhamViewModel>))]
    public class ThuTienMuaDuocPhamViewModelValidator : AbstractValidator<ThuTienMuaDuocPhamViewModel>
    {
        public ThuTienMuaDuocPhamViewModelValidator(
            ILocalizationService localizationService
        )
        {
            RuleFor(x => x.HinhThucThanhToan)
                .Must((model, input, s) =>
                {
                    if (input.Length != 0)
                    {
                        return true;
                    }

                    return false;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.HinhThucThanhToan.Exists"));

            RuleFor(x => x.TienMat)
                .Must((model, input, s) =>
                {
                    if (input + model.ChuyenKhoan + model.POS + model.SoTIenCongNo != model.TongTien)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.CheckMoney.Exists"));

            RuleFor(x => x.ChuyenKhoan)
                .Must((model, input, s) =>
                {
                    if (model.TienMat + input + model.POS + model.SoTIenCongNo != model.TongTien)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.CheckMoney.Exists"));

            RuleFor(x => x.POS)
                .Must((model, input, s) =>
                {
                    if (model.TienMat + model.ChuyenKhoan + input + model.SoTIenCongNo != model.TongTien)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.CheckMoney.Exists"));

            RuleFor(x => x.SoTIenCongNo)
              .Must((model, input, s) =>
              {
                  if (model.TienMat + model.ChuyenKhoan + input + model.POS != model.TongTien)
                  {
                      return false;
                  }

                  return true;
              })
              .WithMessage(localizationService.GetResource("KhachVangLai.CheckMoney.Exists"));


            RuleFor(x => x.BenhNhanDua)
                .Must((model, input, s) =>
                {
                    if (input == 0)
                    {
                        return true;
                    }

                    if (input < model.TienMat)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.CheckBenhNhanDua.Exists"));

            RuleFor(x => x.NgayThu)
                .NotEmpty().WithMessage(localizationService.GetResource("KhachVangLai.NgayThu.Required"));

            RuleFor(x => x.NoiDungThu)
                .NotEmpty().WithMessage(localizationService.GetResource("KhachVangLai.NoiDungThu.Required"));
        }
    }
}
