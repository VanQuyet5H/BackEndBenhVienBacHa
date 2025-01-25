using System;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.QuayThuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhachVangLaiViewModel>))]

    public class KhachVangLaiViewModelValidator : AbstractValidator<KhachVangLaiViewModel>
    {
        public KhachVangLaiViewModelValidator(
            ILocalizationService localizationService
        )
        {
            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"));

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

            RuleFor(x => x.NamSinh)
                .Must((model, input, s) =>
                {
                    if (input > DateTime.Now.Year)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.NamSinh.NotValidate"));

            RuleFor(x => x.TienMat)
                .Must((model, input, s) =>
                {
                    if (input + model.ChuyenKhoan + model.POS + model.SoTienCongNo != model.TongTien)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.CheckMoney.Exists"));

            RuleFor(x => x.ChuyenKhoan)
                .Must((model, input, s) =>
                {
                    if (model.TienMat + input + model.POS + model.SoTienCongNo != model.TongTien)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.CheckMoney.Exists"));

            RuleFor(x => x.POS)
                .Must((model, input, s) =>
                {
                    if (model.TienMat + model.ChuyenKhoan + input + model.SoTienCongNo != model.TongTien)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("KhachVangLai.CheckMoney.Exists"));

            RuleFor(x => x.SoTienCongNo)
              .Must((model, input, s) =>
              {
                  if (model.TienMat + model.ChuyenKhoan + model.POS + input  != model.TongTien)
                  {
                      return false;
                  }

                  return true;
              })
              .WithMessage(localizationService.GetResource("KhachVangLai.CheckMoney.Exists"));

            //RuleFor(x => x.BenhNhanDua)
            //    .Must((model, input, s) =>
            //    {
            //        if (input == 0)
            //        {
            //            return true;
            //        }
            //        return true;
            //    })
            //    .WithMessage(localizationService.GetResource("KhachVangLai.CheckBenhNhanDua.Exists"));

            RuleFor(x => x.NgayThu)
                .NotEmpty().WithMessage(localizationService.GetResource("KhachVangLai.NgayThu.Required"));

            RuleFor(x => x.NoiDungThu)
                .NotEmpty().WithMessage(localizationService.GetResource("KhachVangLai.NoiDungThu.Required"));
        }

    }
}
