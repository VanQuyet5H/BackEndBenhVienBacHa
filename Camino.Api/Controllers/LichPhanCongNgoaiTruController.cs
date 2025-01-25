using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.LichPhanCongNgoaiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Data;
using Camino.Core.Domain.ValueObject.LichPhanCongNgoaiTru;
using Camino.Services.LichPhanCongNgoaiTru;
using Microsoft.AspNetCore.Mvc;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.Localization;
using Camino.Services.PhongBenhVien;

namespace Camino.Api.Controllers
{
    public partial class LichPhanCongNgoaiTruController : CaminoBaseController
    {
        private readonly ILichPhanCongNgoaiTruService _lichPhanCongNgoaiTruService;
        private readonly IRepository<LichPhanCongNgoaiTru> _lichPhanNgoaiTruRepository;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IPhongBenhVienService _phongBenhVienService;
        private readonly ILocalizationService _localizationService;
        public LichPhanCongNgoaiTruController(
            ILichPhanCongNgoaiTruService lichPhanCongNgoaiTruService,
            IRepository<LichPhanCongNgoaiTru> lichPhanNgoaiTruRepository,
            IPhongBenhVienService phongBenhVienService,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            ILocalizationService localizationService)
        {
            _lichPhanCongNgoaiTruService = lichPhanCongNgoaiTruService;
            _lichPhanNgoaiTruRepository = lichPhanNgoaiTruRepository;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _phongBenhVienService = phongBenhVienService;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTuanAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLichPhanCongNgoaiTru)]
        public ActionResult<List<LichTuanGridVo>> GetDataTuanAsync
            (DateTime date)
        {
            var gridData = _lichPhanCongNgoaiTruService.GetDataForTuanAsync(date);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListKhoaPhongXepLich")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLichPhanCongNgoaiTru)]
        public async Task<ActionResult> GetListKhoaPhongXepLich
            (DateTime date, int khoaId)
        {
            var gridData = await _lichPhanCongNgoaiTruService.XepLich(date, khoaId);
            return Ok(gridData);
        }

        [HttpPost("GetListKhoaPhongXepLichPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhongXepLichPhong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _lichPhanCongNgoaiTruService.GetListKhoaPhong(model);
            return Ok(lookup);
        }
    }
}