using System;
using System.Linq;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Camino.Services.TiepNhanBenhNhan;
using FluentValidation;

namespace Camino.Api.Models.YeuCauKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiepNhanBenhNhanViewModel>))]
    public class TiepNhanBenhNhanViewModelValidator : AbstractValidator<TiepNhanBenhNhanViewModel>
    {
        public TiepNhanBenhNhanViewModelValidator(ILocalizationService localizationService, IValidator<BenhNhanTiepNhanBenhNhanViewModel> benhNhanValidator, ITiepNhanBenhNhanService tiepNhanBenhNhanService)
        {
            #region feedback's client on 25/5/2020
            // bổ sung check thông tin thẻ BHYT ở cả trường hợp quét thẻ // 30/07/2021
            RuleFor(x => x.BHYTMaDKBD)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTMaDKBD.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            //RuleFor(x => x.NoiDangKyBHYT)
            //    .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NoiDangKyBHYT.Required")).When(x => x.TuNhap == true);

            RuleFor(x => x.BHYTMucHuong)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTMucHuong.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTDiaChi)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTDiaChi.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTNgayHieuLuc)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTNgayHieuLuc.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTNgayHetHan)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTNgayHetHan.Required")).When(x => x.TuNhap == true || x.CoBHYT == true);

            RuleFor(x => x.BHYTNgayHetHan)
                .Must((model, s) => model.BHYTNgayHieuLuc <= model.BHYTNgayHetHan)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTNgayHetHan.MoreThanBHYTNgayHieuLuc")).When(x => (x.TuNhap == true || x.CoBHYT == true) && x.BHYTNgayHetHan != null && x.BHYTNgayHieuLuc != null);

            RuleFor(x => x.BHYTNgayHetHan)
                .Must((model, s) => model.BHYTNgayHetHan.GetValueOrDefault().Date >= DateTime.Now.Date )
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTNgayHetHan.MoreThanDatetimeNow")).When(x => (x.TuNhap == true || x.CoBHYT == true) && x.BHYTNgayHetHan != null);

            #endregion feedback's client on 25/5/2020

            RuleFor(x => x.ThoiGianTiepNhan)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.ThoiGianTiepNhan.Required"))
                .Must((model, s) => model.ThoiGianTiepNhan <= DateTime.Now).WithMessage(localizationService.GetResource("TiepNhanBenhNhan.ThoiGianTiepNhan.LessThanDatetimeNow")).When(x => x.ThoiGianTiepNhan != null);
            //RuleFor(x => x.LyDoKhamBenhId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.LyDoKhamBenhId.Required"));
            ////RuleFor(x => x.LaKhamTheoYeuCau)
            ////    .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.LaKhamTheoYeuCau.Required"));
            //RuleFor(x => x.KhoaKhamId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.KhoaKhamId.Required"));
            //RuleFor(x => x.PhongKhamVaBacSiId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.PhongKhamVaBacSiId.Required"));

            //RuleFor(x => x.BenhNhan).SetValidator(benhNhanValidator);

            RuleFor(x => x.LyDoTiepNhanId)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.LyDoTiepNhan.Required"));

            RuleFor(x => x.BHYTMaSoThe)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.BHYTMaSoThe.Required")).When(x => x.CoBHYT == true);

            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.HoTen.Required"));

            RuleFor(x => x.NamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinh.Required"))
                .When(x => x.NgayThangNamSinh != null)
                ;

            RuleFor(x => x.NamSinh)
                .Must((model, s) => model.NamSinh <= DateTime.Now.Year)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NamSinhMoreThanNow.Required"))
                .When(x => x.NamSinh != null)
                ;

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.Required"))
                .When(x => x.CoBHYT == true && x.NamSinh == null)
                ;

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinhOrNamSinh.Required"))
                .When(x => x.CoBHYT != true && x.NamSinh == null)
                ;


            RuleFor(x => x.NgayThangNamSinh).Must((model, s) => model.NgayThangNamSinh.GetValueOrDefault() <= DateTime.Now.Date)
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.DatetimeNow"))
                .When(x => x.NgayThangNamSinh != null);

            RuleFor(x => x.Email)
                .Length(0, 200).WithMessage(localizationService.GetResource("TiepNhanBenhNhan.Email.Range"))
                .Must((model, s) => CommonHelper.IsMailValid(model.Email))
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.Emai.WrongEmail"));

            RuleFor(x => x.NguoiLienHeEmail)
                .Length(0, 200).WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeEmail.Range"))
                .Must((model, s) => CommonHelper.IsMailValid(model.NguoiLienHeEmail))
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeEmail.WrongEmail"));

            RuleFor(x => x.CongTyUuDaiId)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.CongTyUuDaiId.Required")).When(x => !x.LstMaVoucher.Any() && ( x.DuocUuDai == true));

            RuleFor(x => x.DoiTuongUuDaiId)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.DoiTuongUuDaiId.Required")).When(x => !x.LstMaVoucher.Any() && (x.DuocUuDai == true));

            //RuleFor(x => x.LstMaVoucher)
            //    .Must((model, s) => model.LstMaVoucher.Any()).WithMessage(localizationService.GetResource("TiepNhanBenhNhan.LstVoucherId.Required")).When(x => x.DuocUuDai == true && x.LoaiMienGiam == 2);

            RuleFor(x => x.NguoiLienHeHoTen)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeHoTen.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result)
                ;

            RuleFor(x => x.NguoiLienHeQuanHeNhanThanId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeQuanHeNhanThanId.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result)
                ;

            RuleFor(x => x.NguoiLienHeSoDienThoai)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NguoiLienHeSoDienThoai.Required"))
                .When(x => tiepNhanBenhNhanService.IsUnder6YearOld(x.NgayThangNamSinh, x.NamSinh).Result)
                ;

            RuleFor(x => x.NoiLamViec)
             .NotEmpty().WithMessage(localizationService.GetResource("BenhNhan.NoiLamViec.Required"))
             .When(x => x.CoBHYT == true);

            #region BVHD-3872
            RuleFor(x => x.QuocTichId)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.QuocTichId.Required"));
            RuleFor(x => x.TinhThanhId)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.TinhThanhId.Required"));
            RuleFor(x => x.QuanHuyenId)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.QuanHuyenId.Required"));
            RuleFor(x => x.PhuongXaId)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.PhuongXaId.Required"));
            #endregion

            #region BVHD-3920
            RuleFor(x => x.HinhThucDenId)
                .NotEmpty().WithMessage(localizationService.GetResource("TiepNhanBenhNhan.HinhThucDenId.Required"));

            RuleFor(x => x.NoiGioiThieuId)
                .MustAsync(async (model, input, f) => await tiepNhanBenhNhanService.KiemTraBatBuocNhapNoiGioiThieuAsync(model.HinhThucDenId, input))
                .WithMessage(localizationService.GetResource("TiepNhanBenhNhan.NoiGioiThieuId.Required"));
            #endregion
        }
    }
}