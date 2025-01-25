using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.NoiGioiThieu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiGioiThieuChiTietMienGiamViewModel>))]
    public class NoiGioiThieuChiTietMienGiamViewModelValidator : AbstractValidator<NoiGioiThieuChiTietMienGiamViewModel>
    {
        public NoiGioiThieuChiTietMienGiamViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DichVuKhamBenhBenhVienId)
                .NotEmpty().When(x => x.LaDichVuKham).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.DichVuKhamBenhBenhVienId.required"));

            RuleFor(x => x.DichVuKyThuatBenhVienId)
                .NotEmpty().When(x => x.LaDichVuKyThuat).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.DichVuKyThuatBenhVienId.required"));

            RuleFor(x => x.DichVuGiuongBenhVienId)
                .NotEmpty().When(x => x.LaDichVuGiuong).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.DichVuGiuongBenhVienId.required"));

            RuleFor(x => x.DuocPhamBenhVienId)
                .NotEmpty().When(x => x.LaDuocPham).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.DuocPhamBenhVienId.required"));

            RuleFor(x => x.VatTuBenhVienId)
                .NotEmpty().When(x => x.LaVatTu).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.VatTuBenhVienId.required"));

            RuleFor(x => x.NhomGiaDichVuKhamBenhBenhVienId)
                .NotEmpty().When(x => x.LaDichVuKham).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.NhomGiaDichVuKhamBenhBenhVienId.required"));

            RuleFor(x => x.NhomGiaDichVuKyThuatBenhVienId)
                .NotEmpty().When(x => x.LaDichVuKyThuat).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.NhomGiaDichVuKyThuatBenhVienId.required"));

            RuleFor(x => x.NhomGiaDichVuGiuongBenhVienId)
                .NotEmpty().When(x => x.LaDichVuGiuong).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.NhomGiaDichVuGiuongBenhVienId.required"));

            RuleFor(x => x.TiLeChietKhau)
                .NotEmpty().When(x => x.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.TiLeChietKhau.required"));
            RuleFor(x => x.SoTienChietKhau)
                .NotEmpty().When(x => x.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien).WithMessage(localizationService.GetResource("NoiGioiThieuChiTietMienGiam.SoTienChietKhau.required"));
        }
    }
}
