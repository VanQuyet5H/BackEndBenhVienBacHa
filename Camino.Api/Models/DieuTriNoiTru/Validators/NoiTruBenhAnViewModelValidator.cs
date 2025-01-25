using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiTruBenhAnViewModel>))]
    public class NoiTruBenhAnViewModelValidator : AbstractValidator<NoiTruBenhAnViewModel>
    {
        public NoiTruBenhAnViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LoaiBenhAn)
                .NotEmpty().WithMessage(localizationService.GetResource("NoiTruBenhAn.LoaiBenhAn.Required"));
            RuleFor(x => x.ThoiDiemTaoBenhAn)
                .NotEmpty().WithMessage(localizationService.GetResource("NoiTruBenhAn.ThoiDiemTaoBenhAn.Required"))
                .LessThanOrEqualTo(DateTime.Now).WithMessage(localizationService.GetResource("NoiTruBenhAn.ThoiDiemTaoBenhAn.Range"))
                .Must((viewModel, input) => viewModel.ThoiDiemTiepNhanNgoaiTru == null 
                                            || input == null 
                                            || (new DateTime(input.Value.Year, input.Value.Month, input.Value.Day, input.Value.Hour, input.Value.Minute, 0) 
                                                >= new DateTime(viewModel.ThoiDiemTiepNhanNgoaiTru.Value.Year, viewModel.ThoiDiemTiepNhanNgoaiTru.Value.Month, viewModel.ThoiDiemTiepNhanNgoaiTru.Value.Day, viewModel.ThoiDiemTiepNhanNgoaiTru.Value.Hour, viewModel.ThoiDiemTiepNhanNgoaiTru.Value.Minute, 0)))
                    .WithMessage(localizationService.GetResource("NoiTruBenhAn.ThoiDiemTaoBenhAn.NhoHonTiepNhan"));
            RuleFor(x => x.ThoiDiemNhapVien)
                .NotEmpty().WithMessage(localizationService.GetResource("NoiTruBenhAn.ThoiDiemNhapVien.Required"))
                //.LessThanOrEqualTo(DateTime.Now).WithMessage(localizationService.GetResource("NoiTruBenhAn.ThoiDiemNhapVien.Range"));
                .Must((viewModel, input) => input == null || viewModel.ThoiDiemTaoBenhAn == null
                                                          //|| input.Value.AddSeconds(-input.Value.Second).AddMilliseconds(-input.Value.Millisecond) 
                                                          //      >= viewModel.ThoiDiemTaoBenhAn.Value.AddSeconds(-viewModel.ThoiDiemTaoBenhAn.Value.Second).AddMilliseconds(-viewModel.ThoiDiemTaoBenhAn.Value.Millisecond))
                                                          || (new DateTime(input.Value.Year, input.Value.Month, input.Value.Day, input.Value.Hour, input.Value.Minute, 0)
                                                            >= new DateTime(viewModel.ThoiDiemTaoBenhAn.Value.Year, viewModel.ThoiDiemTaoBenhAn.Value.Month, viewModel.ThoiDiemTaoBenhAn.Value.Day, viewModel.ThoiDiemTaoBenhAn.Value.Hour, viewModel.ThoiDiemTaoBenhAn.Value.Minute, 0)))
                    .WithMessage(localizationService.GetResource("NoiTruBenhAn.ThoiDiemNhapVien.Range"));
        }
    }
}
