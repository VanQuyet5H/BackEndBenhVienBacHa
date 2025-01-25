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
    [TransientDependency(ServiceType = typeof(IValidator<HoSoChiTietDVKTViewModel>))]
    public class HoSoChiTietDVKTViewModelValidator : AbstractValidator<HoSoChiTietDVKTViewModel>
    {
        public HoSoChiTietDVKTViewModelValidator(ILocalizationService iLocalizationService, IBaoHiemYTeService _BHYTService)
        {
            RuleFor(x => x.MaDichVu)
           .MustAsync(async (model, input, s) => !await _BHYTService.CheckMaDichVuHoSoDVKTRequired(input, model.MaNhom).ConfigureAwait(false))
          .WithMessage(iLocalizationService.GetResource("BHYT.XML3.MaDichVu.Required"))
         .MustAsync(async (model, input, s) => !await _BHYTService.CheckMaDichVuHoSoDVKT(input,model.MaNhom).ConfigureAwait(false))
          .WithMessage(iLocalizationService.GetResource("BHYT.XML3.SoDangKy.Required.VC"));
            RuleFor(x => x.MaNhom)
           .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.Thuoc.MaNhom.Required"));
            RuleFor(x => x.MaVatTu)
           .MustAsync(async (model, input, s) => !await _BHYTService.CheckMaVatTuHoSoDVKTRequired(input, model.MaNhom).ConfigureAwait(false))
          .WithMessage(iLocalizationService.GetResource("BHYT.XML3.MaVatTu.Required"));
            RuleFor(x => x.GoiVatTuYTe)
           .MustAsync(async (model, input, s) => !await _BHYTService.CheckValidGoiVatTuXML3(input).ConfigureAwait(false))
          .WithMessage(iLocalizationService.GetResource("BHYT.XML3.GoiVatTuYTe.Valid"));
            RuleFor(x => x.TenVatTu)
        .MustAsync(async (model, input, s) => !await _BHYTService.CheckMaVatTuHoSoDVKTRequired(input, model.MaNhom).ConfigureAwait(false))
       .WithMessage(iLocalizationService.GetResource("BHYT.XML3.TenVatTu.Required"));
            RuleFor(x => x.TenDichVu)
          .MustAsync(async (model, input, s) => !await _BHYTService.CheckMaDichVuHoSoDVKTRequired(input, model.MaNhom).ConfigureAwait(false))
         .WithMessage(iLocalizationService.GetResource("BHYT.XML3.TenDichVu.Required"));
            RuleFor(x => x.DonViTinh)
        .MustAsync(async (model, input, s) => !await _BHYTService.CheckMaVatTuHoSoDVKTRequired(input, model.MaNhom).ConfigureAwait(false))
       .WithMessage(iLocalizationService.GetResource("BHYT.DonViTinh.Required"));
            RuleFor(x => x.PhamVi)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.PhamVi.Required"));
            RuleFor(x => x.SoLuong)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.SoLuong.Required"));
            RuleFor(x => x.DonGia)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.DonGia.Required"));
            RuleFor(x => x.ThongTinThau)
          .MustAsync(async (model, input, s) => !await _BHYTService.CheckMaVatTuHoSoDVKTRequired(input, model.MaNhom).ConfigureAwait(false))
         .WithMessage(iLocalizationService.GetResource("BHYT.XML3.ThongTinThau.Required"));
            RuleFor(x => x.ThongTinThau)
           .MustAsync(async (model, input, s) => !await _BHYTService.CheckThongTinThauValid(input,model.MaVatTu, model.MaNhom).ConfigureAwait(false))
         .WithMessage(iLocalizationService.GetResource("BHYT.XML3.ThongTinThau.Valid"));
            RuleFor(x => x.TyLeThanhToan)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TyLeThanhToan.Required"));
            RuleFor(x => x.ThanhTien)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.ThanhTien.Required"));
            RuleFor(x => x.MucHuong)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MucHuong.Required"));
            RuleFor(x => x.TienNguonKhac)
       .MustAsync(async (model, input, s) => !await _BHYTService.CheckValueTienNguonKhac(input, model.ThanhTien).ConfigureAwait(false))
         .WithMessage(iLocalizationService.GetResource("BHYT.Thuoc.TienNguonKhac.Valid"));
            RuleFor(x => x.TienBaoHiemThanhToan)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TienBaoHiemTuTra.Required"));
            
            RuleFor(x => x.MaKhoa)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaKhoa.Required"))
         .Length(3, 15).WithMessage(iLocalizationService.GetResource("BHYT.MaKhoa.Length"));
            RuleFor(x => x.MaBacSi)
        .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaBacSi.Required"));
            RuleFor(x => x.MaBenh)
       .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaBenh.Required"));

            RuleFor(x => x.NgayYLenhTime)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NgayYLenhTime.Required"));
         // .MustAsync(async (model, input, s) => !await _BHYTService.CheckValidNgayVaoRa(input).ConfigureAwait(false))
         //.WithMessage(iLocalizationService.GetResource("BHYT.NgayYLenhTime.Valid"));
            RuleFor(x => x.NgayKetQuaTime)
         .MustAsync(async (model, input, s) => !await _BHYTService.CheckValidNgayVaoRa(input).ConfigureAwait(false)).When(x=>x.NgayKetQuaTime!=null)
        .WithMessage(iLocalizationService.GetResource("BHYT.NgayKetQuaTime.Valid"));
            RuleFor(x => x.MaPhuongThucThanhToan)
          .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaPhuongThucThanhToan.Required"));
        } 
    }
}
