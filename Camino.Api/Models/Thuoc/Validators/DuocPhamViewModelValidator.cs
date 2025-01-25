using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.Users;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.Thuocs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Models.Thuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuocPhamViewModel>))]
    public class DuocPhamViewModelValidator: AbstractValidator<DuocPhamViewModel>
    {
        public DuocPhamViewModelValidator(ILocalizationService localizationService, IDuocPhamService thuocBenhVienService, IValidator<DuocPhamBenhVienModel> duocPhamBenhViewModelValidator)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"));
                //.Must((model, s) => !thuocBenhVienService.CheckTenExits(model.Ten, model.Id))
                //.WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.TenTiengAnh)
                .Length(0, 250).WithMessage(localizationService.GetResource("ThuocBenhVien.TenTiengAnh.Length"));

            RuleFor(x => x.SoDangKy)
                //.NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.SoDangKy.Required"))
                //.Length(0, 100).WithMessage(localizationService.GetResource("ThuocBenhVien.SoDangKy.Length"))
                .MustAsync(async (model, input, a) => !await thuocBenhVienService.KiemTraSoDangKyTonTaiAsync(input, model.Id)).WithMessage(localizationService.GetResource("ThuocBenhVien.SoDangKy.Exists"));

            RuleFor(x => x.MaHoatChat)
                //.NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.MaHoatChat.Required"))
                .Length(0, 100).WithMessage(localizationService.GetResource("ThuocBenhVien.MaHoatChat.Length"));
                //.MustAsync(async (model, input, s) => !await thuocBenhVienService.ChecMaHoatChatAsync(model.MaHoatChat, model.Id).ConfigureAwait(false))
                //    .WithMessage(localizationService.GetResource("ThuocBenhVien.MaHoatChat.Exists"));


            RuleFor(x => x.HoatChat)
                //.NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.HoatChat.Required"))
                .Length(0, 550).WithMessage(localizationService.GetResource("ThuocBenhVien.HoatChat.Length"));

            RuleFor(x => x.LoaiThuocHoacHoatChat)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("ThuocBenhVien.LoaiThuocHoacHoatChat.Reqiured"));

            RuleFor(x => x.NhaSanXuat)
                .Length(0, 250).WithMessage(localizationService.GetResource("ThuocBenhVien.NhaSanXuat.Length"));

            RuleFor(x => x.NuocSanXuat)
                .Length(0, 250).WithMessage(localizationService.GetResource("ThuocBenhVien.NuocSanXuat.Length"));

            RuleFor(x => x.DuongDungId)
                .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DuongDung.Required"));

            RuleFor(x => x.HamLuong)
                .Length(0, 500).WithMessage(localizationService.GetResource("ThuocBenhVien.HamLuong.Length"));

            RuleFor(x => x.QuyCach)
                .Length(0, 250).WithMessage(localizationService.GetResource("ThuocBenhVien.QuyCach.Length"));

            RuleFor(x => x.TieuChuan)
                .Length(0, 50).WithMessage(localizationService.GetResource("ThuocBenhVien.TieuChuan.Length"));

            RuleFor(x => x.HuongDan)
                .Length(0, 250).WithMessage(localizationService.GetResource("ThuocBenhVien.BaoChe.Length"));

            RuleFor(x => x.DonViTinhId)
                .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DonViTinh.Required"));

            //RuleFor(x => x.HanSuDung)
            //    .GreaterThan(DateTime.Now.Date)
            //    .WithMessage(localizationService.GetResource("ThuocBenhVien.HanSuDung.Range"));

            RuleFor(x => x.HuongDan)
                .Length(0, 1000).WithMessage(localizationService.GetResource("ThuocBenhVien.HuongDan.Length"));
            RuleFor(x => x.MoTa)
                .Length(0, 1000).WithMessage(localizationService.GetResource("ThuocBenhVien.MieuTa.Length"));
            RuleFor(x => x.ChiDinh)
                .Length(0, 1000).WithMessage(localizationService.GetResource("ThuocBenhVien.ChiDinh.Length"));
            RuleFor(x => x.ChongChiDinh)
                .Length(0, 1000).WithMessage(localizationService.GetResource("ThuocBenhVien.ChongChiDinh.Length"));
            RuleFor(x => x.LieuLuongCachDung)
                .Length(0, 1000).WithMessage(localizationService.GetResource("ThuocBenhVien.LieuLuongCachDung.Length"));
            RuleFor(x => x.TacDungPhu)
                .Length(0, 1000).WithMessage(localizationService.GetResource("ThuocBenhVien.TacDungPhu.Length"));
            RuleFor(x => x.ChuYDePhong)
                .Length(0, 1000).WithMessage(localizationService.GetResource("ThuocBenhVien.ChuYDePhong.Length"));
            // update ngày 5/2/2021
            RuleFor(p => p.DuocPhamBenhVienModel).SetValidator(duocPhamBenhViewModelValidator).When(p => p.SuDungThuocBenhVien == true);
            //RuleFor(x => x.MaDuocPhamBenhVien)
            //    .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.Required"));
            //RuleFor(x => x.DuocPhamBenhVienPhanNhomId)
            //   .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId.Required"));
        }
    }
}
