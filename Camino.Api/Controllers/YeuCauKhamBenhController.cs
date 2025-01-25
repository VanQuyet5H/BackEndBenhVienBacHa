using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using Microsoft.EntityFrameworkCore;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Api.Extensions;
using System;
using Camino.Core.Helpers;
using System.Collections.Generic;
using Camino.Services.BenhVien;
using Camino.Services.KhoaPhong;

namespace Camino.Api.Controllers
{
    public class YeuCauKhamBenhController : CaminoBaseController
    {
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly ILocalizationService _localizationService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IBenhVienService _benhVienService;


        public YeuCauKhamBenhController(IYeuCauKhamBenhService yeuCauKhamBenhService, ILocalizationService localizationService, IJwtFactory iJwtFactory
                , IKhoaPhongService khoaPhongService, IBenhVienService benhVienService)
        {
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _localizationService = localizationService;
            _khoaPhongService = khoaPhongService;
            _benhVienService = benhVienService;
        }
        
    }
}