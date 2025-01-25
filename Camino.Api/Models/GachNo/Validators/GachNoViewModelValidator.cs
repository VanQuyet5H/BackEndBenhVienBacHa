using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Services.GachNo;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.GachNo.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GachNoViewModel>))]
    public class GachNoViewModelValidator : AbstractValidator<GachNoViewModel>
    {
        public GachNoViewModelValidator(ILocalizationService localizationService, IGachNoService gachNoService)
        {
            RuleFor(x => x.NgayChungTu)
                .NotEmpty().WithMessage(localizationService.GetResource("GachNo.NgayChungTu.Required"))
                .LessThanOrEqualTo(DateTime.Now.Date).WithMessage(localizationService.GetResource("GachNo.NgayChungTu.Range"));
            RuleFor(x => x.NgayThucThu)
                .NotEmpty().WithMessage(localizationService.GetResource("GachNo.NgayThucThu.Required"))
                .LessThanOrEqualTo(DateTime.Now.Date).WithMessage(localizationService.GetResource("GachNo.NgayThucThu.Range"))
                .Must((viewModel,input,f) => input == null || viewModel.NgayChungTu == null || input.Value.Date > DateTime.Now.Date || input.Value.Date <= viewModel.NgayChungTu.Value.Date).WithMessage(localizationService.GetResource("GachNo.NgayThucThu.GreaterThanNgayChungTu"));

            RuleFor(x => x.LoaiChungTu).NotEmpty()
                .WithMessage(localizationService.GetResource("GachNo.LoaiChungTu.Required"));

            RuleFor(x => x.LoaiTienTe).NotEmpty()
                .WithMessage(localizationService.GetResource("GachNo.LoaiTienTe.Required"));

            RuleFor(x => x.TyGia).NotEmpty()
                .WithMessage(localizationService.GetResource("GachNo.TyGia.Required"));

            RuleFor(x => x.LoaiDoiTuong).NotEmpty()
                .WithMessage(localizationService.GetResource("GachNo.LoaiDoiTuong.Required"));

            RuleFor(x => x.CongTyBaoHiemTuNhanId)
                .Must((viewModel, input, f) => viewModel.LoaiDoiTuong == null || viewModel.LoaiDoiTuong == Enums.LoaiDoiTuong.BenhNhan || (viewModel.LoaiDoiTuong == Enums.LoaiDoiTuong.BHTN && input != null))
                .WithMessage(localizationService.GetResource("GachNo.CongTyBaoHiemTuNhanId.Required"));

            RuleFor(x => x.BenhNhanId)
                .Must((viewModel, input, f) => viewModel.LoaiDoiTuong == null || viewModel.LoaiDoiTuong == Enums.LoaiDoiTuong.BHTN ||(viewModel.LoaiDoiTuong == Enums.LoaiDoiTuong.BenhNhan && input != null))
                .WithMessage(localizationService.GetResource("GachNo.BenhNhanId.Required"));

            RuleFor(x => x.LoaiThuChi).NotEmpty()
                .WithMessage(localizationService.GetResource("GachNo.LoaiThuChi.Required"));

            RuleFor(x => x.TienHachToan).NotEmpty()
                .WithMessage(localizationService.GetResource("GachNo.TienHachToan.Required"));
        }
    }
}
