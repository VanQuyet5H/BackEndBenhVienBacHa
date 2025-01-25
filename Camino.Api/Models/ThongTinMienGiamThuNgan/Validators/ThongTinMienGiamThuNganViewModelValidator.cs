using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Services.Localization;
using Camino.Services.YeuCauTiepNhans;
using FluentValidation;

namespace Camino.Api.Models.ThongTinMienGiamThuNgan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThongTinMienGiamThuNganViewModel>))]
    public class ThongTinMienGiamThuNganViewModelValidator : AbstractValidator<ThongTinMienGiamThuNganViewModel>
    {
        public ThongTinMienGiamThuNganViewModelValidator(ILocalizationService localizationService, IYeuCauTiepNhanService yeuCauTiepNhanService)
        {
            RuleFor(x => x.ListVouchers)
                .Must((model, input, s) =>
                {
                    if (model.ValidateVoucher && input.Length == 0)
                    {
                        return false; //fail
                    }

                    return true; //pass
                })
                .WithMessage(localizationService.GetResource("ThuNgan.ListVouchers.NotEmpty"))
                .MustAsync(async (model, input, s) => await yeuCauTiepNhanService.IsTrungLoaiVoucher(input).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("ThuNgan.ListVouchers.Exists"));         

            //RuleFor(x => x.SoTienMG)
            //    .Must((model, input, s) =>
            //    {
            //        if (model.ValidateMGThem)
            //        {
            //            if (model.LoaiMienGiamThem == Enums.LoaiMienGiamThem.MienGiamTheoSoTien && (input == 0 || input == null))
            //            {
            //                return false; //fail
            //            }
            //        }

            //        return true; //pass
            //    })
            //    .WithMessage(localizationService.GetResource("ThuNgan.SoTienMG.NotEmpty"))
            //    .Must((model, input, s) =>
            //    {
            //        if (model.ValidateMGThem && model.LoaiMienGiamThem != Enums.LoaiMienGiamThem.MienGiamTheoSoTien && model.LoaiMienGiamThem != Enums.LoaiMienGiamThem.MienGiamTheoTiLe)
            //        {
            //            return false; //fail
            //        }

            //        return true; //pass
            //    })
            //    .WithMessage(localizationService.GetResource("ThuNgan.LoaiMienGiamThem.NotEmpty"));

            //RuleFor(x => x.TiLeMienGiam)
            //    .Must((model, input, s) =>
            //    {
            //        if (model.ValidateMGThem)
            //        {
            //            if (model.LoaiMienGiamThem == Enums.LoaiMienGiamThem.MienGiamTheoTiLe && (input == 0 || input == null))
            //            {
            //                return false; //fail
            //            }
            //        }

            //        return true; //pass
            //    })
            //    .WithMessage(localizationService.GetResource("ThuNgan.TiLeMienGiam.NotEmpty"))
            //    .Must((model, input, s) =>
            //    {
            //        if (model.ValidateMGThem && model.LoaiMienGiamThem != Enums.LoaiMienGiamThem.MienGiamTheoSoTien && model.LoaiMienGiamThem != Enums.LoaiMienGiamThem.MienGiamTheoTiLe)
            //        {

            //            return false; //fail
            //        }

            //        return true; //pass
            //    })
            //    .WithMessage(localizationService.GetResource("ThuNgan.LoaiMienGiamThem.NotEmpty"));

            RuleFor(x => x.LyDoMienGiam)
                .Must((model, input, s) =>
                {
                    if (model.ValidateMGThem && string.IsNullOrEmpty(input))
                    {
                        return false; //fail
                    }

                    return true; //pass
                })
                .WithMessage(localizationService.GetResource("ThuNgan.LyDoMienGiam.NotEmpty"));

            RuleFor(x => x.TaiLieuDinhKem)
                .Must((model, input, s) =>
                {
                    if (input == null) { return false; }
                    if (model.ValidateMGThem && input.Ten == null && input.DuongDan == null)
                    {
                        return false; //fail
                    }

                    return true; //pass
                })
                .WithMessage(localizationService.GetResource("ThuNgan.TaiLieuDinhKem.NotEmpty"));

            //RuleFor(x => x.SoTienMG)
            //    .MustAsync(async (model, input, s) => await yeuCauTiepNhanService.KiemTraSoTienMiemGiam(model.IdYeuCauTiepNhan, model.LoaiMienGiamThem, input ?? 0).ConfigureAwait(false))
            //    .WithMessage(localizationService.GetResource("ThuNgan.SoTienMG.MacthValueSoTienMG"));
        }
    }
}
