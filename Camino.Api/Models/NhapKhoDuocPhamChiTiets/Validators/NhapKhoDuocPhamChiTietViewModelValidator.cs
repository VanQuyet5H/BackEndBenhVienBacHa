using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhapKhoDuocPhams;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoDuocPhamChiTiets.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhapKhoDuocPhamChiTietViewModel>))]
    public class NhapKhoDuocPhamChiTietViewModelValidator : AbstractValidator<NhapKhoDuocPhamChiTietViewModel>
    {
        public NhapKhoDuocPhamChiTietViewModelValidator(ILocalizationService localizationService, INhapKhoDuocPhamService iNhapKhoDuocPhamService)
        {
          
            RuleFor(x => x.DuocPhamBenhVienId)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId.Required"));

            RuleFor(x => x.HopDongThauDuocPhamId)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId.Required"))
              .Must((request, soLuongNhap, id) => {
                 if (request.HopDongThauDuocPhamId == 0)
                     return false;
                 return true;
              }).WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId.Required"));

            RuleFor(x => x.Solo)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.Solo.Required"));

            RuleFor(x => x.HanSuDung)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.HanSuDung.Required"))
                 .Must((request, ngay, id) => {
                      if (ngay != null && ngay.Value.Date <= DateTime.Now.Date)
                          return false;
                      return true;
                  }).WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.HanSuDung.LessThanNow"));

            RuleFor(x => x.SoLuongNhap) 
             .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.SoLuongNhap.Required"))
             .Must((request, soLuongNhap, id) => {
                 if (soLuongNhap <=0)
                     return false;
                 return true;
             }).WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.SoLuongNhap.LessThan0"))
             .MustAsync(async (model, input, s) => await iNhapKhoDuocPhamService.KiemTraSoLuongNhapDuocPhamTheoHopDongThau(model.Id, model.NhapKhoDuocPhamId, model.HopDongThauDuocPhamId, model.DuocPhamBenhVienId, input, model.SoLuongNhapTrongGrid, model.SoLuongHienTaiDuocPhamTheoHopDongThauDaLuu))
             .WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.SoLuongNhap.NotValid"));
            

            RuleFor(x => x.DonGiaNhap)
             .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.DonGia.Required"));

            //RuleFor(x => x.DonGiaBan)
            // .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.DonGia.Required"));

            //RuleFor(x => x.VAT)
            // .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.VAT.Required"));

            RuleFor(x => x.KhoDuocPhamViTriId)
             .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.KhoDuocPhamViTriId.Required"));

            RuleFor(x => x.MaVach)
            .MustAsync(async (model, input, s) =>!await iNhapKhoDuocPhamService.CheckMaVachAsync(model.Id, model.MaVach, model.DuocPhamBenhVienId).ConfigureAwait(false))
            .WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.MaVach.Exists"));

            RuleFor(x => x.KhoNhapSauKhiDuyetId)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.KhoNhapSauKhiDuyetId.Required"));

            RuleFor(x => x.NguoiNhapSauKhiDuyetId)
                .Must((req, nguoiNhapSauKhiDuyetId, id) =>
                {
                    if (req.KhoNhapSauKhiDuyetId > 0)
                    {
                        return nguoiNhapSauKhiDuyetId != null && nguoiNhapSauKhiDuyetId != 0;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("nhapKhoChiTiet.NguoiNhapSauKhiDuyetId.Required"));

            //RuleFor(x => x.ChietKhau)
            //.NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.NhapKhoDuocPhamId.Required"));
        }
    }
}
