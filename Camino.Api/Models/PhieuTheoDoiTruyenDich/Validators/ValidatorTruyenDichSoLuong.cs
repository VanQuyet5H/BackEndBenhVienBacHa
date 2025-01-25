//using Camino.Core.DependencyInjection.Attributes;
//using Camino.Services.DieuTriNoiTru;
//using Camino.Services.Localization;
//using FluentValidation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Camino.Api.Models.PhieuTheoDoiTruyenDich.Validators
//{
  
//    [TransientDependency(ServiceType = typeof(IValidator<ValidateSoLuongTruyenDich>))]
//    public class ValidatorTruyenDichSoLuong : AbstractValidator<ValidateSoLuongTruyenDich>
//    {
//        //public ValidatorTruyenDichSoLuong(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
//        //{
//        //    //RuleFor(x => x.SoLuong)
//        //    //  .MustAsync(async (viewModel, ngayTiepNhan, id) =>
//        //    //  {
//        //    //      var kiemTraSL = await dieuTriNoiTruService.ValidateSoLuongChangeDichTruyen(viewModel.YeuCauTiepNhanId, viewModel.SoLuong, viewModel.NgayThu,viewModel.DuocPhamId, viewModel.BatDau);
//        //    //      return kiemTraSL;
//        //    //  })
//        //    //  .WithMessage(localizationService.GetResource("HoSoKhac.SoLuong.KhongDung"));
//        //}
//    }
//}
