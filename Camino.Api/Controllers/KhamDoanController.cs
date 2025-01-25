using System.Threading.Tasks;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.ExportImport;
using Camino.Services.KhamDoan;
using Camino.Services.TiepNhanBenhNhan;
using Microsoft.AspNetCore.Mvc;
using Camino.Services.Localization;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using System.Linq;
using System;
using Camino.Api.Models.Error;
using Camino.Services.Helpers;
using Newtonsoft.Json;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.KhamBenhs;
using Camino.Services.KhamDoan.GoiKhamSucKhoes;
using Camino.Services.Users;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class KhamDoanController : CaminoBaseController
    {
        private readonly IKhamDoanService _khamDoanService;
        private readonly IExcelService _excelService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserService _userService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IKhamBenhService _khamBenhService;
        private readonly IPdfService _pdfService;

        private readonly IGoiKhamSucKhoeService _goiKhamSucKhoeService;
        private readonly IGoiKhamSucKhoeChungService _goiKhamSucKhoeChungService;

        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IRoleService _roleService;

        private readonly IPhongBenhVienHangDoiService _phongBenhVienHangDoiService;

        public KhamDoanController(
            IKhamDoanService khamDoanService, IUserService userService, IUserAgentHelper userAgentHelper, IExcelService excelService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanService, ILocalizationService localizationService,
            IYeuCauKhamBenhService yeuCauKhamBenhService, IKhamBenhService khamBenhService, IPdfService pdfService,
            IYeuCauTiepNhanService yeuCauTiepNhanService, IGoiKhamSucKhoeChungService goiKhamSucKhoeChungService,
            IGoiKhamSucKhoeService goiKhamSucKhoeService, ITaiLieuDinhKemService taiLieuDinhKemService,
            IRoleService roleService,
            IPhongBenhVienHangDoiService phongBenhVienHangDoiService)
        {
            _khamDoanService = khamDoanService;
            _excelService = excelService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
            _localizationService = localizationService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _khamBenhService = khamBenhService;
            _pdfService = pdfService;
            _goiKhamSucKhoeService = goiKhamSucKhoeService;
            _userService = userService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _userAgentHelper = userAgentHelper;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _goiKhamSucKhoeChungService = goiKhamSucKhoeChungService;
            _roleService = roleService;
            _phongBenhVienHangDoiService = phongBenhVienHangDoiService;
        }

        #region Get lookup
        [HttpPost("GetLoaiGiaDichVuKhamBenh")]
        public async Task<ActionResult> GetLoaiGiaKhamBenh(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetLoaiGiaKhamBenh(model);
            return Ok(result);
        }

        [HttpPost("GetLoaiGiaDichVuKyThuat")]
        public async Task<ActionResult> GetLoaiGiaDichVuKyThuat(DropDownListRequestModel model)
        {
            var result = await _khamDoanService.GetLoaiGiaDichVuKyThuat(model);
            return Ok(result);
        }

        [HttpPost("GetCongTys")]
        public async Task<ActionResult> GetCongTy([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamDoanService.GetCongTy(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienTaoGoiKSKs")]
        public async Task<ActionResult> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienTaoGoiKSKs([FromBody]DropDownListRequestModel queryInfo)
        {
            var info = JsonConvert.DeserializeObject<DichVuKhamBenhBVHoacDVKTBenhVienJSON>(queryInfo.ParameterDependencies.Replace("undefined", "null"));
            var dichVuKhamBenhBVHoacDVKTBenhVienParams = new DichVuKhamBenhBVHoacDVKTBenhVienParams
            {
                ChonDichVuKham = true,
                ChonDichVuKyThuat = true,
                KhongLayTiemChung = true,
                //FullNhomDichVu = info.CoGoiPhatSinh == true,
                FullNhomDichVu = true,
                CoGoiPhatSinh = info.CoGoiPhatSinh == true,
            };
            var lookup = await _khamDoanService.GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienTaoGoiKSKs(queryInfo, dichVuKhamBenhBVHoacDVKTBenhVienParams);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienFullNhoms")]
        public async Task<ActionResult> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienFullNhoms([FromBody]DropDownListRequestModel queryInfo)
        {
            var dichVuKhamBenhBVHoacDVKTBenhVienParams = new DichVuKhamBenhBVHoacDVKTBenhVienParams
            {
                ChonDichVuKham = false,
                ChonDichVuKyThuat = true,
                FullNhomDichVu = true,
                DichVuKhamIds = null,
                DichVuKyThuatIds = null
            };
            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                var results = _khamDoanService.GetListDichVuIdTTrongGoiKhamSucKhoe(queryInfo.ParameterDependencies);
                //lstDVKhamId = results.Item1;
                //lstDVKTId = results.Item2;
                //getDichVuKham = true;
                dichVuKhamBenhBVHoacDVKTBenhVienParams.DichVuKhamIds = results.Item1;
                dichVuKhamBenhBVHoacDVKTBenhVienParams.DichVuKyThuatIds = results.Item2;
                dichVuKhamBenhBVHoacDVKTBenhVienParams.ChonDichVuKham = true;
            }
            var lookup = await _khamDoanService.GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens(queryInfo, dichVuKhamBenhBVHoacDVKTBenhVienParams);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienFullNhomKeyStrings")]
        public async Task<ActionResult> GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhVienFullNhomKeyStrings([FromBody]DropDownListRequestModel queryInfo)
        {
            var dichVuKhamBenhBVHoacDVKTBenhVienParams = new DichVuKhamBenhBVHoacDVKTBenhVienParams
            {
                ChonDichVuKham = true, //false,
                ChonDichVuKyThuat = true,
                FullNhomDichVu = true,
                DichVuKhamIds = null,
                DichVuKyThuatIds = null,
                KhongLayTiemChung = true,
                CoGoiPhatSinh = true
            };
            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                var results = _khamDoanService.GetListDichVuIdTTrongGoiKhamSucKhoe(queryInfo.ParameterDependencies);
                //lstDVKhamId = results.Item1;
                //lstDVKTId = results.Item2;
                //getDichVuKham = true;
                dichVuKhamBenhBVHoacDVKTBenhVienParams.DichVuKhamIds = results.Item1;
                dichVuKhamBenhBVHoacDVKTBenhVienParams.DichVuKyThuatIds = results.Item2;
                dichVuKhamBenhBVHoacDVKTBenhVienParams.ChonDichVuKham = true;
            }
            var lookup = await _khamDoanService.GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens(queryInfo, dichVuKhamBenhBVHoacDVKTBenhVienParams);

            var lookupKemDonGias = await _khamDoanService.GetDonGiaTheoDichVuKhamSucKhoeAsync(lookup, queryInfo.ParameterDependencies);
            return Ok(lookupKemDonGias);
        }

        [HttpPost("GetDichVuKyThuatBenhViens")]
        public async Task<ActionResult> GetDichVuKyThuatBenhViens([FromBody]DropDownListRequestModel queryInfo)
        {
            var dichVuKhamBenhBVHoacDVKTBenhVienParams = new DichVuKhamBenhBVHoacDVKTBenhVienParams
            {
                ChonDichVuKham = false,
            };
            //var lookup = await _khamDoanService.GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens(queryInfo, false);
            var lookup = await _khamDoanService.GetDichVuKhamBenhBenhVienVaDichVuKyThuatBenhViens(queryInfo, dichVuKhamBenhBVHoacDVKTBenhVienParams);
            return Ok(lookup);
        }

        [HttpPost("GetHopDong")]
        public async Task<ActionResult> GetHopDong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamDoanService.GetHopDong(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetListBacSi")]
        public async Task<ActionResult> GetListBacSiAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamDoanService.GetListBacSiAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetHopDongKhamSucKhoes")]
        public async Task<ActionResult> GetHopDongKhamSucKhoe([FromBody]DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false)
        {
            var lookup = await _khamDoanService.GetHopDongKhamSucKhoe(queryInfo, LaHopDongKetThuc);
            return Ok(lookup);
        }

        [HttpPost("GetKhoaPhongGoiKhams")]
        public async Task<ActionResult> GetKhoaPhongGoiKham([FromBody]DropDownListRequestModel queryInfo, string selectedItems = null, bool isMulti = false)
        {
            var info = JsonConvert.DeserializeObject<KhoaPhongJsonVo>(queryInfo.ParameterDependencies.Replace("undefined", "null"));
            if (!isMulti ? info.DichVuId == null : info.DichVuStringId == null)
            {
                return Ok(new List<LookupItemTemplateVo>());
            }
            if (info.HinhThucKhamBenh == 1)// Khám nội viện
            {
                var dropDownList = new DropDownListRequestModel
                {
                    ParameterDependencies = "{DichVuId: " + (!isMulti ? info.DichVuId : info.DichVuStringId.DichVuId) + "}",
                    Id = queryInfo.Id,
                    Query = queryInfo.Query,
                    Take = queryInfo.Take
                };
                if (info.LaDichVuKham == true) // Là dịch vụ khám bệnh bệnh viện
                {

                    var lookup = await _yeuCauKhamBenhService.GetNoiThucHiens(dropDownList, selectedItems);
                    return Ok(lookup);
                }
                else // Là dịch vụ kỹ thuật bệnh viện
                {
                    var lookup = await _khamBenhService.GetPhongThucHienChiDinhKhamOrDichVuKyThuat(dropDownList, selectedItems);
                    return Ok(lookup);
                }
            }
            else // Khám ngoại viện
            {
                var lookup = await _khamDoanService.GetKhoaPhongGoiKham(queryInfo, selectedItems);
                return Ok(lookup);
            }

        }

        [HttpPost("GetLoaiHopDongs")]
        public Task<List<LookupItemVo>> GetLoaiHopDong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lstLoaiHopDong = EnumHelper.GetListEnum<Enums.LoaiHopDong>();
            var result = lstLoaiHopDong.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            if (queryInfo.Id != 0)
            {
                result = result.Where(c => c.KeyId == queryInfo.Id).ToList();
            }
            return Task.FromResult(result);
        }

        [HttpPost("GetTrangThaiHopDongs")]
        public Task<List<LookupItemVo>> GetTrangThaiHopDong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lstTrangThaiHopDong = EnumHelper.GetListEnum<Enums.TrangThaiHopDongKham>();
            var result = lstTrangThaiHopDong.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            if (queryInfo.Id != 0)
            {
                result = result.Where(c => c.KeyId == queryInfo.Id).ToList();
            }

            return Task.FromResult(result);
        }

        [HttpPost("GetChucDanhs")]
        public ActionResult<ICollection<string>> GetChucDanh(DropDownListRequestModel model)
        {
            var lstChucDanhs = Camino.Services.Helpers.ResourceHelper.GetChucDanh(model).Select(c => c.DisplayName);
            return Ok(lstChucDanhs);
        }

        [HttpPost("GetListViTriLamViec")]
        public ActionResult<ICollection<string>> GetListViTriLamViec(DropDownListRequestModel model)
        {
            var lstChucDanhs = ResourceHelper.GetViTriLamViecs(model).Select(c => c.DisplayName);
            return Ok(lstChucDanhs);
        }

        [HttpPost("GetCongViecKhamDoans")]
        public Task<List<LookupItemVo>> GetCongViecKhamDoan([FromBody]DropDownListRequestModel queryInfo)
        {
            var lstTrangThaiHopDong = EnumHelper.GetListEnum<Enums.CongViecKhamSucKhoe>();
            var result = lstTrangThaiHopDong.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            if (queryInfo.Id != 0)
            {
                result = result.Where(c => c.KeyId == queryInfo.Id).ToList();
            }

            return Task.FromResult(result);
        }

        [HttpPost("GetListDoiTuong")]
        public Task<List<LookupItemVo>> GetListDoiTuongAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var doiTuong = EnumHelper.GetListEnum<Enums.DoiTuongNhanSu>();
            var result = doiTuong.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            //if (queryInfo.Id != 0)
            //{
            //    result = result.Where(c => c.KeyId == queryInfo.Id).ToList();
            //}

            return Task.FromResult(result);
        }

        [HttpPost("GetTinhTrangHonNhans")]
        public Task<List<LookupItemVo>> GetTinhTrangHonNhans([FromBody]DropDownListRequestModel queryInfo)
        {
            var lstTrangThaiHopDong = EnumHelper.GetListEnum<Enums.TinhTrangHonNhan>();
            var result = lstTrangThaiHopDong.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            return Task.FromResult(result);
        }

        [HttpPost("GetListNhomDoiTuongKhamSucKhoe")]
        public async Task<ActionResult<string>> GetListNhomDoiTuongKhamSucKhoeAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamDoanService.GetListNhomDoiTuongKhamSucKhoeAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDiaDiemKhamCongTyTonTais")]
        public ActionResult<ICollection<string>> GetDiaDiemKhamCongTyTonTais([FromBody]DropDownListRequestModel queryInfo)
        {
            var diaDiemCongTyKhamTonTais = _khamDoanService.GetDiaDiemKhamCongTyTonTai(queryInfo);
            if (queryInfo.Id != 0)
            {
                diaDiemCongTyKhamTonTais = diaDiemCongTyKhamTonTais.Where(c => c.KeyId == queryInfo.Id).ToList();
            }
            return diaDiemCongTyKhamTonTais.Select(c => c.DisplayName).ToList();
        }

        [HttpPost("GetGoiKhamTheoHopDongs")]
        public ActionResult<List<LookupItemVo>> GetGoiKhamTheoHopDongs([FromBody]DropDownListRequestModel queryInfo)
        {
            var goiKhamTheoHopDongs = _khamDoanService.GetThongTinGoiKhamTheoHopDong(queryInfo);
            //if (queryInfo.Id != 0)
            //{
            //    goiKhamTheoHopDongs = goiKhamTheoHopDongs.Where(c => c.KeyId == queryInfo.Id).ToList();
            //}
            return goiKhamTheoHopDongs;
        }

        [HttpPost("GetBenhNgheNghieps")]
        public ActionResult<ICollection<string>> GetBenhNgheNghiep(DropDownListRequestModel model)
        {
            var lstBenhNgheNghiep = Camino.Services.Helpers.ResourceHelper.GetBenhNgheNghiep(model).Select(c => c.DisplayName);
            return Ok(lstBenhNgheNghiep);
        }

        [HttpPost("GetPhanLoaiSucKhoes")]
        public ActionResult<ICollection<LookupItemVo>> GetPhanLoaiSucKhoe(DropDownListRequestModel model)
        {
            var lookup = _khamDoanService.GetPhanLoaiSucKhoe(model);
            return Ok(lookup);
        }

        [HttpPost("TimKiemHopDongConHieuLucNhanVien")]
        public async Task<ActionResult> TimKiemHopDongConHieuLucNhanVien([FromBody]DropDownListRequestModel queryInfo, string userName)
        {
            var lookup = await _khamDoanService.TimKiemHopDongConHieuLucNhanVien(queryInfo, userName);
            return Ok(lookup);
        }

        [HttpPost("GetLoaiDichVuTrenHeThongVaGoiChungs")]
        public async Task<ActionResult> GetLoaiDichVuTrenHeThongVaGoiChungs([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamDoanService.GetLoaiDichVuTrenHeThongVaGoiChungs(queryInfo);
            return Ok(lookup);
        }
        #endregion

        [HttpPost("GetGoiKhamChungs")]
        public ActionResult<List<LookupItemTemplateVo>> GetGoiKhamChungs([FromBody]DropDownListRequestModel queryInfo)
        {
            var goiKhamChungs = _khamDoanService.GetGoiKhamChungs(queryInfo);
            return goiKhamChungs;
        }


        #region Get data

        [HttpGet("GetThongTinHanhChinh")]
        public async Task<ActionResult<KhamDoanThongTinHanhChinhVo>> GetThongTinHanhChinhAsync(long yeuCauTiepNhanId)
        {
            var thongTinHanhChinh = await _khamDoanService.GetThongTinHanhChinhAsync(yeuCauTiepNhanId);
            return thongTinHanhChinh;
        }

        [HttpPost("TimKiemThongTinHopDongKhamTheoCongTy")]
        public async Task<ActionResult<TimKiemHopDongKhamTheoCongTyViewModel>> TimKiemThongTinHopDongKhamTheoCongTyAsync(TimKiemHopDongKhamTheoCongTyViewModel timKiem)
        {
            var result = new TimKiemHopDongKhamTheoCongTyViewModel();
            if (timKiem.CongTyId == null || timKiem.HopDongId == null)
                return result;
            // xử lý get
            var hopDong = await _khamDoanService.TimKiemThongTinHopDongKhamTheoCongTyAsync(timKiem.HopDongId.Value);
            return hopDong.ToModel<TimKiemHopDongKhamTheoCongTyViewModel>();
            // return result;
        }

        [HttpGet("GetThongTinHanhChinhNhanVien")]
        public async Task<ActionResult<YeuCauTiepNhanKhamSucKhoeViewModel>> GetThongTinHanhChinhNhanVienAsync(long hopDongKhamNhanVienId)
        {
            var yeuCauKham = await _khamDoanService.GetThongTinHanhChinhNhanVienAsync(hopDongKhamNhanVienId);
            if (yeuCauKham == null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }
            var result = yeuCauKham.ToModel<YeuCauTiepNhanKhamSucKhoeViewModel>();
            return result;
        }

        [HttpPost("GetDonGiaBenhVien")]
        public async Task<ActionResult> GetDonGiaBenhVien(DichVuKhamBenhGiaBenhVienVo dichVuKhamBenhGiaBenhVienVo)
        {
            var donGia = await _khamDoanService.GetDonGiaBenhVien(dichVuKhamBenhGiaBenhVienVo);
            return Ok(donGia);
        }

        [HttpPost("GetThongTinGiaDichVuTrongGoi")]
        public async Task<ActionResult<ThongTinGiaDichVuTrongGoiKhamSucKhoeVo>> GetThongTinGiaDichVuTrongGoi(DichVuKhamBenhGiaBenhVienVo dichVuKhamBenhGiaBenhVienVo)
        {
            var thongTinGia = await _khamDoanService.GetThongTinGiaDichVuTrongGoi(dichVuKhamBenhGiaBenhVienVo);
            return Ok(thongTinGia);
        }
        #endregion
    }
}
