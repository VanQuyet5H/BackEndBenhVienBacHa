using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BHYT;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<HoSoChiTietThuocViewModel>))]
    public class HoSoChiTietThuocViewModelValidator : AbstractValidator<HoSoChiTietThuocViewModel>
    {
        public HoSoChiTietThuocViewModelValidator(ILocalizationService iLocalizationService, IBaoHiemYTeService _BHYTService)
        {
            RuleFor(x => x.MaThuoc)
           .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaThuoc.Required"));
            RuleFor(x => x.MaNhom)
           .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.Thuoc.MaNhom.Required"));
            RuleFor(x => x.TenThuoc)
           .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TenThuoc.Required"));
            RuleFor(x => x.PhamVi)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.PhamVi.Required"));
            RuleFor(x => x.DonViTinh)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.DonViTinh.Required"));
            RuleFor(x => x.SoDangKy)
          .MustAsync(async (model, input, s) => !await _BHYTService.CheckSpace(input).ConfigureAwait(false))
          .WithMessage(iLocalizationService.GetResource("BHYT.Thuoc.SoDangKy.Required"));
            RuleFor(x => x.SoLuong)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.SoLuong.Required"));
            RuleFor(x => x.DonGia)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.DonGia.Required"));
            RuleFor(x => x.TyLeThanhToan)
        .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TyLeThanhToan.Required"))
        .MustAsync(async (model, input, s) => !await _BHYTService.CheckValueTyLeThanhToan(input,model.PhamVi).ConfigureAwait(false))
          .WithMessage(iLocalizationService.GetResource("BHYT.Thuoc.TyLeThanhToan.Valid"));
            RuleFor(x => x.ThanhTien)
           .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.ThanhTien.Required"));
            RuleFor(x => x.MucHuong)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MucHuong.Required"));
            RuleFor(x => x.TienNguonKhac)
       .MustAsync(async (model, input, s) => !await _BHYTService.CheckValueTienNguonKhac(input, model.ThanhTien).ConfigureAwait(false))
         .WithMessage(iLocalizationService.GetResource("BHYT.Thuoc.TienNguonKhac.Valid"));
            RuleFor(x => x.TienBaoHiemTuTra)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TienBaoHiemTuTra.Required"));
            RuleFor(x => x.MaKhoa)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaKhoa.Required"))
          .Length(3, 15).WithMessage(iLocalizationService.GetResource("BHYT.MaKhoa.Length"));
            RuleFor(x => x.MaBacSi)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaBacSi.Required"));
            RuleFor(x => x.MaBenh)
        .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaBenh.Required"));
            RuleFor(x => x.MaPhuongThucThanhToan)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaPhuongThucThanhToan.Required"));
            RuleFor(x => x.NgayYLenhTime)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NgayYLenhTime.Required"));
          // .MustAsync(async (model, input, s) => !await _BHYTService.CheckValidNgayVaoRa(input).ConfigureAwait(false))
          //.WithMessage(iLocalizationService.GetResource("BHYT.NgayYLenhTime.Valid"));
            
        }
    }
}
