using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKhuyenMai;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.TiemChung;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DichVuKhuyenMai;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Core.Infrastructure;
using Camino.Services.BenhNhans;
using Camino.Services.BenhVien;
using Camino.Services.CauHinh;
using Camino.Services.DanhMucMarketing;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Helpers;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.TiemChung;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.Users;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class TiepNhanBenhNhanController : CaminoBaseController
    {
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly ICauHinhService _cauhinhService;
        private readonly IBenhVienService _benhVienService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly ILocalizationService _localizationService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;

        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IDanhSachChoKhamService _danhSachChoKhamService;

        private readonly IDanhMucMarketingService _danhMucMarketingService;

        private readonly IRoleService _roleService;
        private readonly IKhamBenhService _khamBenhService;
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        private readonly IYeuCauDichVuKyThuatService _yeuCauDichVuKyThuatService;
        private readonly IThuNganNoiTruService _thuNganNoiTruService;
        private readonly ITiemChungService _tiemChungService;
        private readonly ILoggerManager _logger;

        public TiepNhanBenhNhanController(ITiepNhanBenhNhanService tiepNhanBenhNhanService
            , ITaiLieuDinhKemService taiLieuDinhKemService, ICauHinhService cauhinhService
            , IBenhVienService benhVienService, IYeuCauKhamBenhService yeuCauKhamBenhService
            , ILocalizationService localizationService, IBenhNhanService benhNhanService
            , ILoggerManager logger
            , ITaiKhoanBenhNhanService taiKhoanBenhNhanService
            , IRoleService roleService
            , IYeuCauTiepNhanService yeuCauTiepNhanService
            , IUserAgentHelper userAgentHelper
            , IDanhSachChoKhamService danhSachChoKhamService
            , IDanhMucMarketingService danhMucMarketingService
            , IKhamBenhService khamBenhService
            , IDieuTriNoiTruService dieuTriNoiTruService
            , IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService
            , IThuNganNoiTruService thuNganNoiTruService
            , ITiemChungService tiemChungService)
        {
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _cauhinhService = cauhinhService;
            _benhVienService = benhVienService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _localizationService = localizationService;
            _benhNhanService = benhNhanService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _userAgentHelper = userAgentHelper;
            _danhSachChoKhamService = danhSachChoKhamService;
            _roleService = roleService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _danhMucMarketingService = danhMucMarketingService;
            _khamBenhService = khamBenhService;
            _dieuTriNoiTruService = dieuTriNoiTruService;
            _yeuCauDichVuKyThuatService = yeuCauDichVuKyThuatService;
            _thuNganNoiTruService = thuNganNoiTruService;
            _tiemChungService = tiemChungService;
            _logger = logger;
        }
        //[HttpPost("GetLyDoKhamBenh")]
        //public async Task<ActionResult<ICollection<LookupItemVo>>> GetLyDoKhamBenh([FromBody]DropDownListRequestModel queryInfo)
        //{
        //    var lookup = await _tiepNhanBenhNhanService.GetLyDoKhamBenh(queryInfo);
        //    return Ok(lookup);
        //}
        [HttpPost("GetKhoaKham")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan, DocumentType.BaoCaoLuuTruHoSoBenhAn)]
        public async Task<ActionResult> GetKhoaKham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _tiepNhanBenhNhanService.GetKhoaKham(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetPhongKham")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<ICollection<PhongKhamTemplateVo>>> GetPhongKham(DropDownListRequestModel model)
        {
            var settings = _cauhinhService.LoadSetting<BaoHiemYTe>();

            var lookup = await _tiepNhanBenhNhanService.GetPhongKham(model, settings.GioiHanSoNguoiKham);
            return Ok(lookup);
        }

        [HttpPost("GetPhongKhamKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<ICollection<PhongKhamTemplateVo>>> GetPhongKhamKhac(long id, DropDownListRequestModel model)
        {
            model.ParameterDependencies = "{khoaKham:" + id + "}";

            var settings = _cauhinhService.LoadSetting<BaoHiemYTe>();

            var lookup = await _tiepNhanBenhNhanService.GetPhongKham(model, settings.GioiHanSoNguoiKham);
            return Ok(lookup);
        }

        [HttpPost("GetPhongKhamKyThuat")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<ICollection<PhongKhamDVKTTemplateVo>>> GetPhongKhamKyThuat(DropDownListRequestModel model)
        {
            var settings = _cauhinhService.LoadSetting<BaoHiemYTe>();

            var lookup = await _tiepNhanBenhNhanService.GetPhongKhamKyThuat(model, settings.GioiHanSoNguoiKham, 1);
            return Ok(lookup);
        }

        [HttpPost("GetPhongKhamKyThuatKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<ICollection<PhongKhamDVKTTemplateVo>>> GetPhongKhamKyThuatKhac(long id, DropDownListRequestModel model)
        {
            model.ParameterDependencies = "{khoaKham:" + id + "}";

            var settings = _cauhinhService.LoadSetting<BaoHiemYTe>();

            var lookup = await _tiepNhanBenhNhanService.GetPhongKhamKyThuat(model, settings.GioiHanSoNguoiKham, 1);
            return Ok(lookup);
        }

        [HttpPost("GetPhongKhamDichVuGiuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<ICollection<PhongKhamDVKTTemplateVo>>> GetPhongKhamDichVuGiuong(DropDownListRequestModel model)
        {
            var settings = _cauhinhService.LoadSetting<BaoHiemYTe>();

            var lookup = await _tiepNhanBenhNhanService.GetPhongKhamKyThuat(model, settings.GioiHanSoNguoiKham, 2);
            return Ok(lookup);
        }

        [HttpPost("GetPhongKhamDichVuGiuongKhac")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<ICollection<PhongKhamDVKTTemplateVo>>> GetPhongKhamDichVuGiuongKhac(long id, DropDownListRequestModel model)
        {
            model.ParameterDependencies = "{khoaKham:" + id + "}";

            var settings = _cauhinhService.LoadSetting<BaoHiemYTe>();

            var lookup = await _tiepNhanBenhNhanService.GetPhongKhamKyThuat(model, settings.GioiHanSoNguoiKham, 2);
            return Ok(lookup);
        }


        [HttpPost("GetDoiTuongKhamChuaBenhUuTien")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDoiTuongKhamChuaBenhUuTien(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetDoiTuongKhamChuaBenhUuTien(model);
            return Ok(lookup);
        }

        [HttpPost("GetNgheNghiep")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNgheNghiep(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetNgheNghiep(model);
            return Ok(lookup);
        }

        [HttpPost("GetDanToc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanToc(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetDanToc(model);
            return Ok(lookup);
        }

        [HttpPost("GetQuocTich")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetQuocTich(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetQuocTich(model);
            return Ok(lookup);
        }

        [HttpPost("GetTinhThanh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult> GetTinhThanh(DropDownListRequestModel model, long? quanHuyenId)
        {
            var lookup = await _tiepNhanBenhNhanService.GetTinhThanh(model, quanHuyenId);
            return Ok(lookup);
        }

        [HttpPost("GetQuanHuyen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult> GetQuanHuyen(DropDownListRequestModel model, long? phuongXaId)
        {
            var lookup = await _tiepNhanBenhNhanService.GetQuanHuyen(model, phuongXaId);
            return Ok(lookup);
        }

        [HttpPost("GetPhuongXa")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult> GetPhuongXa(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetPhuongXa(model);
            return Ok(lookup);
        }

        [HttpPost("GetQuanHe")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult<List<LookupItemVo>>> GetQuanHe(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetQuanHe(model);
            return Ok(lookup);
        }

        [HttpGet("GetSoTienBHYTSeThanhToan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan, DocumentType.XacNhanBHYT)]
        public async Task<ActionResult> GetSoTienBHYTSeThanhToan()
        {
            var result = await _cauhinhService.SoTienBHYTSeThanhToanToanBo();
            return Ok(result);
        }

        [HttpPost("GetBenhNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetBenhNhan(long benhNhanId)
        {
            var benhNhan = await _benhNhanService.GetByIdAsync(benhNhanId, s => s.Include(o => o.BenhNhanCongTyBaoHiemTuNhans)
                            .Include(o => o.YeuCauTiepNhans)
                            .Include(o => o.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                            .Include(o => o.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                            .Include(o => o.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                            .Include(o => o.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                            .Include(o => o.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                            .Include(o => o.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                            );
            var result = benhNhan.ToModel<BenhNhanTiepNhanBenhNhanViewModel>();
            if (result.BaoHiemTuNhans.Any())
            {
                foreach (var item in result.BaoHiemTuNhans)
                {
                    item.CongTyDisplay = await _tiepNhanBenhNhanService.GetTenCongTyBaoHiemTuNhan(item.BHTNCongTyBaoHiemId ?? 0);
                }
            }
            if (benhNhan.YeuCauGoiDichVus.Any() && benhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                                                                                       z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                                                                                       z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
            {
                result.CoYeuCauGoiDichVu = true;
            }
            if (benhNhan.YeuCauGoiDichVuSoSinhs.Any()
             && benhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() || z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                                                                                       z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
            {
                result.CoYeuCauGoiDichVu = true;

            }

            if (benhNhan.YeuCauTiepNhans.Any())
            {
                var noiHinhThuc = benhNhan.YeuCauTiepNhans.LastOrDefault();
                result.HinhThucDenId = noiHinhThuc.HinhThucDenId;
                result.NoiGioiThieuId = noiHinhThuc.NoiGioiThieuId;
            }

            return Ok(result);
        }

        #region update v2
        [HttpPost("GetIdBenhNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetIdBenhNhan(string maSoBHYT, long? benhNhanId)
        {
            var benhNhan = await _benhNhanService.GetBenhNhanByMaBHYT(maSoBHYT, benhNhanId);
            var result = benhNhan?.Id;
            return Ok(result);
        }

        [HttpPost("GetDoiTuongUuDai")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupItemVo>>> GetDoiTuongUuDai(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetDoiTuongUuDai(model);
            return Ok(lookup);
        }

        [HttpPost("GetCongTyUuDai")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupItemVo>>> GetCongTyUuDai(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetCongTyUuDai(model);
            return Ok(lookup);
        }

        [HttpPost("GetLyDoTiepNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupItemVo>>> GetLyDoTiepNhan(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetLyDoVaoVien(model);
            return Ok(lookup);
        }

        [HttpPost("GetLyDoTiepNhanTreeView")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupTreeItemVo>>> GetLyDoTiepNhanTreeView(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetLyDoTiepNhanTreeView(model);
            return Ok(lookup);
        }

        [HttpPost("GetHinhThucDen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupItemVo>>> GetHinhThucDen(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetHinhThucDen(model);
            return Ok(lookup);
        }

        [HttpPost("GetGioiTinh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult<List<LookupItemVo>>> GetGioiTinh(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetGioiTinh(model);
            return Ok(lookup);
        }

        [HttpPost("GetLoaiTaiLieuDinhKem")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupItemVo>>> GetLoaiTaiLieuDinhKem(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetLoaiTaiLieuDinhKem(model);
            return Ok(lookup);
        }

        [HttpPost("GetNameLoaiTaiLieuDinhKem")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<string>> GetNameLoaiTaiLieuDinhKem(ThemTaiLieu model)
        {
            var result = await _tiepNhanBenhNhanService.GetNameLoaiTaiLieuDinhKem(model.LoaiId ?? 0);
            return Ok(result);
        }

        [HttpPost("GetNoiGioiThieu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupItemVo>>> GetNoiGioiThieu(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.NoiGioiThieu(model);
            return Ok(lookup);
        }

        [HttpGet("GetListTenTrieuChungKhamAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<List<string>> GetListTenTrieuChungKhamAsync()
        {
            var result = await _tiepNhanBenhNhanService.GetListTenTrieuChungKhamAsync();
            return result;
        }

        [HttpPost("GetDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetDichVu(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetDichVu(model);
            return Ok(result);
        }

        [HttpPost("GetDichVuKhamBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetDichVuKhamBenh(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetDichVuKhamBenh(model);
            return Ok(result);
        }

        [HttpPost("GetDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetDichVuKyThuat(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetDichVuKyThuat(model);
            return Ok(result);
        }

        [HttpPost("GetDichVuGiuongBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetDichVuGiuongBenh(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetDichVuGiuongBenh(model);
            return Ok(result);
        }

        [HttpPost("GetLoaiGiaKhamBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetLoaiGiaKhamBenh(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetLoaiGiaKhamBenh(model);
            return Ok(result);
        }

        [HttpPost("ThemDichVu")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemDichVu(ThemDichVuKhamBenhVo model)
        {
            var result = await _tiepNhanBenhNhanService.GetDataForDichVuKhamBenhGridAsync(model);
            return Ok(result);
        }

        [HttpPost("SetTinhThanhQuanHuyen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.None)]
        public async Task<ActionResult> SetTinhThanhQuanHuyen(long phuongXaId)
        {
            var result = await _tiepNhanBenhNhanService.GetTinhThanhQuanHuyen(phuongXaId);
            return Ok(result);
        }

        [HttpPost("ThemDichVuForUpdateView")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemDichVuForUpdateView(ThemDichVuKhamBenhVo model)
        {
            var entity = await _tiepNhanBenhNhanService.GetDataForDichVuKhamBenhForUpdateViewGridAsync(model);

            //entity = UpdateThongTinBHYTFromModel(entity, model.model);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(entity);

            #region cập nhật 23/12/2022
            entity = null;
            entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(model.YeuCauTiepNhanId ?? 0);
            #endregion

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, model.ListDichVuCheckTruocDos, EnumNhomGoiDichVu.DichVuKhamBenh, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);

        }

        [HttpPost("ThemDichVuKyThuatForUpdateView")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemDichVuKyThuatForUpdateView(ThemDichVuKhamBenhVo model)
        {
            var queryDonGiaObj = new GetDonGiaVo()
            {
                DichVuKhamBenhBenhVienId = model.MaDichVuId,
                NhomGiaDichVuKhamBenhBenhVienId = model.LoaiGiaId,
                IsCheckValidDonGia = true
            };
            var kiemTra = await _tiepNhanBenhNhanService.GetDonGiaKyThuat(queryDonGiaObj);

            model.DuocHuongBHYT = false;
            var entity = await _tiepNhanBenhNhanService.GetDataForDichVuKyThuatOrGiuongForUpdateViewGridAsync(model, 1);

            //entity = UpdateThongTinBHYTFromModel(entity, model.model);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(entity);

            #region cập nhật 23/12/2022
            entity = null;
            entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(model.YeuCauTiepNhanId ?? 0);
            #endregion

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, model.ListDichVuCheckTruocDos, EnumNhomGoiDichVu.DichVuKyThuat, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("ThemDichVuGiuongForUpdateView")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemDichVuGiuongForUpdateView(ThemDichVuKhamBenhVo model)
        {
            model.DuocHuongBHYT = false;
            var entity = await _tiepNhanBenhNhanService.GetDataForDichVuKyThuatOrGiuongForUpdateViewGridAsync(model, 2);

            //entity = UpdateThongTinBHYTFromModel(entity, model.model);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(entity);

            entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(entity.Id);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, model.ListDichVuCheckTruocDos, EnumNhomGoiDichVu.DichVuGiuongBenh, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("ThemDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemDichVuKyThuat(ThemDichVuKyThuatVo model)
        {
            // kiểm tra loại giá hiệu lực
            var queryDonGiaObj = new GetDonGiaVo()
            {
                DichVuKhamBenhBenhVienId = model.MaDichVuId,
                NhomGiaDichVuKhamBenhBenhVienId = model.LoaiGiaId,
                IsCheckValidDonGia = true
            };
            var kiemTra = await _tiepNhanBenhNhanService.GetDonGiaKyThuat(queryDonGiaObj);
            var result = await _tiepNhanBenhNhanService.GetDataForDichVuKyThuatGridAsync(model, 1);
            return Ok(result);
        }

        [HttpPost("ThemDichVuGiuong")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemDichVuGiuong(ThemDichVuKyThuatVo model)
        {
            var result = await _tiepNhanBenhNhanService.GetDataForDichVuKyThuatGridAsync(model, 2);
            return Ok(result);
        }

        [HttpGet("GetTyLeSoLanHuongBHYTDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetTyLeSoLanHuongBHYTDichVu()
        {
            var result = await _cauhinhService.GetTyLeHuongBaoHiem5LanKhamDichVuBHYT();
            return Ok(result);
        }

        [HttpPost("ThemGoiKhongChietKhau")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemGoiKhongChietKhau(ThemDichVuKhamBenhVo model)
        {
            var result = await _tiepNhanBenhNhanService.GetDataForGoiCoHoacKhongChietKhauGridAsync(model, false, model.ListDichVuCheckTruocDos);
            return Ok(result);
        }

        [HttpPost("ThemGoiCoChietKhau")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemGoiCoChietKhau(ThemDichVuKhamBenhVo model)
        {
            var result = await _tiepNhanBenhNhanService.GetDataForGoiCoHoacKhongChietKhauGridAsync(model, true, model.ListDichVuCheckTruocDos);
            return Ok(result);
        }
        // cần xem lại
        [HttpPost("ThemGoiKhongChietKhauForUpdate")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemGoiKhongChietKhauForUpdate(ThemDichVuKhamBenhVo model)
        {
            var result = await _tiepNhanBenhNhanService.GetDataForGoiCoHoacKhongChietKhauGridForUpdateAsync(model, false, model.ListDichVuCheckTruocDos);
            return Ok(result);
        }

        [HttpPost("ThemGoiCoChietKhauForUpdate")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemGoiCoChietKhauForUpdate(ThemDichVuKhamBenhVo model)
        {
            var result = await _tiepNhanBenhNhanService.GetDataForGoiCoHoacKhongChietKhauGridForUpdateAsync(model, true, model.ListDichVuCheckTruocDos);
            return Ok(result);
        }

        [HttpPost("ThemGoiKhongChietKhauPopup")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemGoiKhongChietKhauPopup(AddGoiForUpdateView lstGoi)
        {
            if (lstGoi.LstGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId)))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ThemGoi.NoiChiDinhRequired"));

            }
            var entity = await _tiepNhanBenhNhanService.ThemGoiKhongChietKhauPopup(lstGoi.LstGrid, lstGoi.YeuCauTiepNhanId, lstGoi.MucHuong);

            //entity = UpdateThongTinBHYTFromModel(entity, model.model);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("ThemGoiCoChietKhauPopup")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemGoiCoChietKhauPopup(AddGoiForUpdateView lstGoi)
        {
            if (lstGoi.LstGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId)))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ThemGoi.NoiChiDinhRequired"));

            }
            var entity = await _tiepNhanBenhNhanService.ThemGoiCoChietKhauPopup(lstGoi.LstGrid, lstGoi.YeuCauTiepNhanId, lstGoi.MucHuong);

            //entity = UpdateThongTinBHYTFromModel(entity, model.model);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }


        [HttpPost("GetDonGia")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetDonGia(GetDonGiaVo model)
        {
            var donGia = await _tiepNhanBenhNhanService.GetDonGia(model);
            return Ok(donGia);
        }

        [HttpPost("GetDonGiaKyThuat")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetDonGiaKyThuat(GetDonGiaVo model)
        {
            var donGia = await _tiepNhanBenhNhanService.GetDonGiaKyThuat(model);
            return Ok(donGia);
        }

        [HttpPost("GetDonGiaGiuongBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetDonGiaGiuongBenh(GetDonGiaVo model)
        {
            var donGia = await _tiepNhanBenhNhanService.GetDonGiaGiuongBenh(model);
            return Ok(donGia);
        }

        [HttpPost("GetTuyenChuyen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupItemVo>>> GetTuyenChuyen(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetTuyenChuyen(model);
            return Ok(lookup);
        }

        [HttpPost("GetCongTyBaoHiemTuNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<List<LookupItemVo>>> GetCongTyBaoHiemTuNhan(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetCongTyBaoHiemTuNhan(model);
            return Ok(lookup);
        }

        [HttpPost("GetThongTinBHTN")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetThongTinBHTN(long congTyBaoHiemTuNhanId)
        {
            var result = await _tiepNhanBenhNhanService.GetThongTinBHTN(congTyBaoHiemTuNhanId);
            return Ok(result);
        }

        [HttpPost("GetDiaChiBHYT")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetDiaChiBHYT(DiaChiBHYT model)
        {
            var result = await _tiepNhanBenhNhanService.GetDiaChiBHYT(model);
            return Ok(result);
        }

        [HttpPost("ThemThongTinBHTN")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemThongTinBHTN(ThemBaoHiemTuNhan model)
        {
            var result = await _tiepNhanBenhNhanService.ThemThongTinBHTN(model);
            return Ok(result);
        }

        [HttpPost("GetGoiKhamTongHop")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetGoiKhamTongHop(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetGoiKhamTongHop(model);
            return Ok(result);
        }

        [HttpPost("GetGoiKham")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetGoiKham(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetGoiKham(model);
            return Ok(result);
        }

        [HttpPost("GetGoiKhamCoChietKhau")]
        public async Task<ActionResult> GetGoiKhamCoChietKhau(DropDownListRequestModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetGoiKhamCoChietKhau(model);
            return Ok(result);
        }

        [HttpPost("GetThongTinTaiKhoanBenhNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetThongTinTaiKhoanBenhNhan(long idbenhNhan, long idYeuCauTiepNhan)
        {
            var benhNhan = await _benhNhanService.GetByIdAsync(idbenhNhan);

            if (benhNhan == null) return Ok(null);

            //var soDuTaiKhoan = await _taiKhoanBenhNhanService.SoDuTaiKhoan(benhNhan.Id);
            var soDuTaiKhoanBenhNhan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(idYeuCauTiepNhan);
            var result = new ThongTinTaiKhoanBenhNhan
            {
                MaBenhNhan = benhNhan.MaBN,
                SoDuTaiKhoan = soDuTaiKhoanBenhNhan
            };

            return Ok(result);
        }

        [HttpPost("GetThongTinBenhNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetThongTinBenhNhan(string maSoBHYT, long? benhNhanId)
        {
            var benhNhan = await _benhNhanService.GetBenhNhanByMaBHYT(maSoBHYT, benhNhanId);

            if (benhNhan == null) return Ok(null);

            return Ok(benhNhan);
        }

        [HttpPost("GetYeuCauTiepNhanOfBenhNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetYeuCauTiepNhanOfBenhNhan(string maSoBHYT, long? id)
        {
            var yeuCauTiepNhanId = await _tiepNhanBenhNhanService.GetYeuCauTiepNhanIdOfBenhNhan(maSoBHYT, id);
            return Ok(yeuCauTiepNhanId);
        }

        [HttpPost("GetYeuCauTiepNhanOfBenhNhanTrongNgay")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetYeuCauTiepNhanOfBenhNhanTrongNgay(string maSoBHYT, long? id)
        {
            var yeuCauTiepNhanIdTrongNgay = await _tiepNhanBenhNhanService.GetYeuCauTiepNhanIdOfBenhNhanTrongNgay(maSoBHYT, id);

            return Ok(yeuCauTiepNhanIdTrongNgay);
        }

        [HttpPost("GetYeuCauTiepNhanOfBenhNhanNgoaiNgay")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetYeuCauTiepNhanOfBenhNhanNgoaiNgay(string maSoBHYT, long? id)
        {
            var yeuCauTiepNhanIdNgoaiNgay = await _tiepNhanBenhNhanService.GetYeuCauTiepNhanIdOfBenhNhanNgoaiNgay(maSoBHYT, id);

            return Ok(yeuCauTiepNhanIdNgoaiNgay);
        }
        #endregion update v2

        [HttpPost("GetBenhNhanTimKiem")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetBenhNhanTimKiem(TimKiemBenhNhanGridVo model)
        {
            if (string.IsNullOrEmpty(model.DiaChi) && string.IsNullOrEmpty(model.SoDienThoai)
                && string.IsNullOrEmpty(model.SoChungMinhThu) && string.IsNullOrEmpty(model.HoTen)
                && string.IsNullOrEmpty(model.MaBHYT) && model.NgaySinh == null && model.NgaySinh == null
                && model.NamSinh == null)
            {
                return Ok(null);
            }

            var benhNhan = await _benhNhanService.GetBenhNhanByTiepNhanBenhNhanTimKiem(model);

            if (!benhNhan.Any()) return Ok(null);

            var result = new List<TimKiemBenhNhanGridVo>();
            foreach (var item in benhNhan)
            {
                var child = new TimKiemBenhNhanGridVo();
                child = item.Map<TimKiemBenhNhanGridVo>();
                result.Add(child);
            }

            return Ok(result);
        }

        [HttpPost("GetBenhNhanTimKiemPopup")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetBenhNhanTimKiemPopup(TimKiemBenhNhanPopup data)
        {
            var model = data.searchBenhNhan;
            //if (string.IsNullOrEmpty(model.DiaChi) && string.IsNullOrEmpty(model.SoDienThoai)
            //    && string.IsNullOrEmpty(model.SoChungMinhThu) && string.IsNullOrEmpty(model.HoTen)
            //    && string.IsNullOrEmpty(model.MaBHYT) && model.NgaySinh == null)
            //{
            //    return Ok(null);
            //}
            var benhNhan = await _benhNhanService.GetBenhNhanByTiepNhanBenhNhanTimKiem(model, data.searchPopup);

            if (!benhNhan.Any()) return Ok(null);

            var result = new List<TimKiemBenhNhanGridVo>();
            foreach (var item in benhNhan)
            {
                var child = new TimKiemBenhNhanGridVo();
                child = item.Map<TimKiemBenhNhanGridVo>();
                result.Add(child);
            }

            return Ok(result);
        }

        [HttpPost("IsDungTuyen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public ActionResult IsDungTuyen(string maBenhVien)
        {
            var settings = _cauhinhService.LoadSetting<BaoHiemYTe>();
            var result = settings?.BenhVienTiepNhan.Equals(maBenhVien) ?? false;
            return Ok(result);
        }

        [HttpPost("GetTyLeMienGiamKhamBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetTyLeMienGiamKhamBenh(long doiTuongId, long maDichVuId)
        {
            var result = await _tiepNhanBenhNhanService.GetTyLeMienGiamKhamBenh(doiTuongId, maDichVuId);
            return Ok(result);
        }

        [HttpPost("GetTyLeMienGiamKyThuat")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetTyLeMienGiamKyThuat(long doiTuongId, long maDichVuId)
        {
            var result = await _tiepNhanBenhNhanService.GetTyLeMienGiamKyThuat(doiTuongId, maDichVuId);
            return Ok(result);
        }

        [HttpPost("LuuPhieuKham")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> LuuPhieuKham([FromBody]TiepNhanBenhNhanViewModel model)
        {
            if (model != null && model.IsCheckedBHYT == true && model.IsOutOfDate && model.CoBHYT == true)
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.MaBHYT.OutOfDate"));
            }

            if (model != null && model.BHYTMucHuong == null && model.IsCheckedBHYT == true && model.CoBHYT == true)
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.MaBHYT.RemoveBHYT"));

            }

            //if (model != null && model.CoBHYT == true && !string.IsNullOrEmpty(model?.BHYTMaSoThe) && await _tiepNhanBenhNhanService.CheckBenhNhanTNBNExists(model.BHYTMaSoThe))
            //{
            //    throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.MaBHYT.Exists"), 503);
            //}

            // kiểm tra lần 1: trường hợp đã chọn người bệnh ở UI
            // kiểm tra lần 2 sẽ xử lý ở bước kiểm tra thông tin bệnh nhân (trường hợp nhập tay) ở dưới
            //if (model != null && model.CoBHYT == true && !string.IsNullOrEmpty(model?.BHYTMaSoThe) && (model.BenhNhanId != null || (model.BenhNhan != null && model.BenhNhan.Id != 0)))

            // trường hợp bệnh nhân nhập tay, đã check ở FE
            if (model != null && (model.BenhNhanId != null || (model.BenhNhan != null && model.BenhNhan.Id != 0)))
            {
                var benhNhanId = model.BenhNhanId != null ? model.BenhNhanId : model.BenhNhan.Id;
                var kiemTra = await _tiepNhanBenhNhanService.KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(benhNhanId ?? 0);
                if (!string.IsNullOrEmpty(kiemTra.ErrorMessage))
                {
                    throw new ApiException(kiemTra.ErrorMessage);
                }
            }

            if ((model?.BenhNhanId != 0 || model?.BenhNhanId != null) && !await _tiepNhanBenhNhanService.isValidateSoLuongTonTrongGoi(model.YeuCauKhacGrid, model?.BenhNhanId ?? 0))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.SoLuongTon.IsValidate"));
            }

            if (await _tiepNhanBenhNhanService.IsUnder6YearOld(model.NgayThangNamSinh, model.NamSinh))
            {
                model.NguoiLienHeTinhThanhId = model.TinhThanhId;
                model.NguoiLienHeQuanHuyenId = model.QuanHuyenId;
                model.NguoiLienHePhuongXaId = model.PhuongXaId;
                model.NguoiLienHeDiaChi = model.DiaChi;
            }

            //if (model != null && model.CoBHYT == true && model.IsCheckedBHYT == false)
            //{
            //    throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.MaBHYT.RemoveBHYT"));
            //}
            //var entity = model.ToEntity<YeuCauKhamBenh>();

            //var benhNhanExists = await _tiepNhanBenhNhanService.GetBenhNhan(entity.BenhNhan.BHYTMaSoThe);
            //if (benhNhanExists != null)
            //{
            //    entity.BenhNhanId = benhNhanExists.Id;
            //    entity.BenhNhan = null;
            //}

            //if (entity.GiayChuyenVien != null)
            //{
            //    //save s3
            //    entity.GiayChuyenVien.Ma = Guid.NewGuid().ToString();
            //    entity.GiayChuyenVien.LoaiTapTin = LoaiTapTin.Loai1;
            //    await _taiLieuDinhKemService.LuuTaiLieuAsync(entity.GiayChuyenVien.DuongDan, entity.GiayChuyenVien.TenGuid);
            //}

            ////
            //var benhVien = await _benhVienService.GetBenhVienWithMaBenhVien(model.BenhNhan.NoiDangKyBHYT);
            //if (benhVien != null && entity.BenhNhan != null)
            //{
            //    entity.BenhNhan.BHYTNoiDangKyId = benhVien.Id;
            //}
            ////
            //if (!string.IsNullOrEmpty(model.PhongKhamVaBacSiId))
            //{
            //    var idTong = model.PhongKhamVaBacSiId.Split(",");
            //    entity.PhongKhamId = int.Parse(idTong[0]);
            //    entity.BacSiChiDinhId = int.Parse(idTong[1]);
            //}
            ////
            //entity.TrangThaiYeuCauKhamBenh = EnumTrangThaiYeuCauKhamBenh.ChuaKham;
            //await _tiepNhanBenhNhanService.AddAsync(entity);
            //return Ok(entity.Id);

            var isBenhNhanExists = false;
            //check valid chi dinh dich vu
            //if (model.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamBenh && !model.YeuCauKhamBenhGrid.Any())
            //{
            //    throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.LoaiYeuCauTiepNhanMaDichVu.Required"));
            //}
            //else if (model.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.DichVuKhac && !model.YeuCauKhacGrid.Any())
            //{
            //    throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.LoaiYeuCauTiepNhanMaDichVu.Required"));
            //}
            //else if (model.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.ThucHienDichVuKyThuat && !model.YeuCauKyThuatGrid.Any())
            //{
            //    throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.LoaiYeuCauTiepNhanMaDichVu.Required"));
            //}

            //if (!model.YeuCauKhacGrid.Any())
            //{
            //    throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.LoaiYeuCauTiepNhanMaDichVu.Required"));
            //}

            //
            if (model.YeuCauKyThuatGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId))
                || model.YeuCauDichVuKyThuats.Any(p => p.NoiThucHienId == null)
                || model.YeuCauKhacGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId)))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ChiDinhDichVu.NoiThucHienRequired"));
            }

            if (model.YeuCauKyThuatGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId))
                || model.YeuCauDichVuKyThuats.Any(p => p.SoLan == null || p.SoLan == 0)
                || model.YeuCauKhacGrid.Any(p => p.SoLuong == 0 || p.SoLuong == null))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ChiDinhDichVu.SoLuongRequired"));
            }

            if (model.YeuCauKyThuatGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId))
                || model.YeuCauDichVuKyThuats.Any(p => p.NhomGiaDichVuKyThuatBenhVienId == null || p.NhomGiaDichVuKyThuatBenhVienId == 0)
                || model.YeuCauKhacGrid.Any(p => p.LoaiGiaId == 0))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ChiDinhDichVu.LoaiGiaRequired"));
            }

            //nát
            model.YeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuatTiepNhanViewModel>();
            model.YeuCauKhamBenhs = new List<YeuCauKhamBenhTiepNhanViewModel>();
            //

            if (model.HinhThucDenId != 2)
            {
                model.NoiGioiThieuId = null;
            }

            var entity = model.ToEntity<YeuCauTiepNhan>();

            //save s3 for tai lieu dinh kem\
            foreach (var taiLieuDinhKem in entity.HoSoYeuCauTiepNhans)
            {
                //save s3
                taiLieuDinhKem.Ma = Guid.NewGuid().ToString();
                taiLieuDinhKem.LoaiTapTin = LoaiTapTin.Image;
                await _taiLieuDinhKemService.LuuTaiLieuAsync(taiLieuDinhKem.DuongDan, taiLieuDinhKem.TenGuid);
            }

            entity.ThoiDiemCapNhatTrangThai = DateTime.Now;
            entity.ThoiDiemTiepNhan = model.ThoiGianTiepNhan ?? DateTime.Now;
            entity.TrangThaiYeuCauTiepNhan = EnumTrangThaiYeuCauTiepNhan.DangThucHien;

            // chuyen vien và giấy miễn cùng chi trả và có ưu đãi và BHTN và BHYT
            if (entity.CoBHTN == true)
            {
                foreach (var bhtn in model.BaoHiemTuNhans)
                {
                    var baoHiemTuNhan = new YeuCauTiepNhanCongTyBaoHiemTuNhan
                    {
                        CongTyBaoHiemTuNhanId = bhtn.BHTNCongTyBaoHiemId ?? 0,
                        DiaChi = bhtn.BHTNDiaChi,
                        SoDienThoai = bhtn.BHTNSoDienThoai,
                        NgayHetHan = bhtn.BHTNNgayHetHan,
                        NgayHieuLuc = bhtn.BHTNNgayHieuLuc,
                        MaSoThe = bhtn.BHTNMaSoThe,
                        Id = bhtn.Id
                    };
                    entity.YeuCauTiepNhanCongTyBaoHiemTuNhans.Add(baoHiemTuNhan);
                }
            }

            //[BVHD-1581]
            if (entity.DuocChuyenVien == false || entity.DuocChuyenVien == null)
            {
                entity.ThoiGianChuyenVien = null;
                entity.SoChuyenTuyen = null;
                entity.TuyenChuyen = null;
                entity.LyDoChuyen = null;
                entity.GiayChuyenVien = null;
            }
            else
            {
                if (entity.GiayChuyenVien != null)
                {
                    //save s3
                    entity.GiayChuyenVien.Ma = Guid.NewGuid().ToString();
                    entity.GiayChuyenVien.LoaiTapTin = LoaiTapTin.Image;
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(entity.GiayChuyenVien.DuongDan, entity.GiayChuyenVien.TenGuid);
                }
            }
            //

            if (entity.CoBHYT == true)
            {

                if (entity.BHYTDuocMienCungChiTra == false || entity.BHYTDuocMienCungChiTra == null)
                {
                    entity.BHYTNgayDuocMienCungChiTra = null;
                }
                else
                {
                    if (entity.BHYTGiayMienCungChiTra != null)
                    {
                        //save s3
                        entity.BHYTGiayMienCungChiTra.Ma = Guid.NewGuid().ToString();
                        entity.BHYTGiayMienCungChiTra.LoaiTapTin = LoaiTapTin.Image;
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(entity.BHYTGiayMienCungChiTra.DuongDan, entity.BHYTGiayMienCungChiTra.TenGuid);
                    }
                }


                // BVHD-3481: nếu nhập thông tin bệnh nhân bằng tay mà ko chọn bẹnh nhân, thì sẽ xử lý cảnh báo trước khi chạy vào function lưu -> ko cần tự động map bệnh nhân nữa
                ////tạo người bệnh
                //if (model.BenhNhanId == null || model.BenhNhanId == 0)
                //{
                //    var benhNhanExists = await _tiepNhanBenhNhanService.GetBenhNhan(entity.BHYTMaSoThe);
                //    if (benhNhanExists != null)
                //    {
                //        entity.BenhNhanId = benhNhanExists.Id;
                //        entity.BenhNhan = null;
                //        isBenhNhanExists = true;


                //        // đây là kiểm tra lần 2: trường hợp nhập tay thông tin bệnh nhân
                //        var kiemTra = await _tiepNhanBenhNhanService.KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(benhNhanExists.Id);
                //        if (!string.IsNullOrEmpty(kiemTra.ErrorMessage))
                //        {
                //            throw new ApiException(kiemTra.ErrorMessage);
                //        }
                //    }
                //    else
                //    {
                //        entity.BenhNhan = await _tiepNhanBenhNhanService.CreateNewBenhNhan(entity);
                //    }
                //}
                //else
                //{
                //    entity.BenhNhan = null;
                //}

                if (model.BenhNhanId == null || model.BenhNhanId == 0)
                {
                    entity.BenhNhan = await _tiepNhanBenhNhanService.CreateNewBenhNhan(entity);
                }
                else
                {
                    entity.BenhNhan = null;
                }
            }
            else
            {
                //clear data
                entity = RemoveBHYT(entity);

                //tạo người bệnh
                if (model.BenhNhanId == null || model.BenhNhanId == 0)
                {
                    entity.BenhNhan = await _tiepNhanBenhNhanService.CreateNewBenhNhan(entity);
                }
                else
                {
                    entity.BenhNhan = null;
                }
            }



            if (entity.DuocUuDai != true)
            {
                entity.DoiTuongUuDaiId = null;
                entity.CongTyUuDaiId = null;

                model.LstVoucherId = new List<long>();
                model.LstMaVoucher = new List<string>();
            }
            //if (entity.DuocUuDai != true || (entity.DuocUuDai == true && model.LoaiMienGiam == 1))
            //{
            //    model.LstVoucherId = new List<long>();
            //    model.LstMaVoucher = new List<string>();
            //}
            // chuyen vien và giấy miễn cùng chi trả và có ưu đãi và BHTN và BHYT

            // chỉ định dịch vụ
            if (model.YeuCauKhamBenhGrid.Any())
            {
                var lstKhamBenh = await _tiepNhanBenhNhanService.SetListYeuCauKhamBenh(model.YeuCauKhamBenhGrid);
                foreach (var item in lstKhamBenh)
                {
                    entity.YeuCauKhamBenhs.Add(item);
                }
            }
            if (model.YeuCauKyThuatGrid.Any())
            {
                var lstYeuCauKyThuat = await _tiepNhanBenhNhanService.SetListYeuCauKyThuat(model.YeuCauKyThuatGrid);
                foreach (var item in lstYeuCauKyThuat)
                {
                    entity.YeuCauDichVuKyThuats.Add(item);
                }
            }
            if (model.YeuCauKhacGrid.Any())
            {
                entity = await _tiepNhanBenhNhanService.SetListYeuCauKhac(model.YeuCauKhacGrid, entity);
            }
            // chỉ định dịch vụ
            entity.MaYeuCauTiepNhan = ResourceHelper.CreateMaYeuCauTiepNhan();
            entity.NoiTiepNhanId = _userAgentHelper.GetCurrentNoiLLamViecId();
            entity.NhanVienTiepNhanId = _userAgentHelper.GetCurrentUserId();

            //if (model.LstVoucherId != null)
            //{
            //    foreach (var voucherId in model.LstVoucherId)
            //    {
            //        // tạo ra 1 the voucher
            //        var voucher = _tiepNhanBenhNhanService.get
            //        var newTheVocuher = new TheVoucher()
            //        {
            //            Ma = maVocher,
            //            TuNgay = hanSuDungVoucher.FirstOrDefault().TuNgay,
            //            DenNgay = hanSuDungVoucher.FirstOrDefault().DenNgay,
            //            VoucherId = voucherId
            //        };
            //        //_theVoucher.Add(newTheVocuher);

            //        var theVoucherYeuCauTiepNhan = new TheVoucherYeuCauTiepNhan
            //        {
            //            //TheVoucherId = voucherId,
            //            TheVoucher = newTheVocuher,
            //            YeuCauTiepNhan = entity
            //        };

            //        if (model.BenhNhanId != null && model.BenhNhanId != 0)
            //        {
            //            theVoucherYeuCauTiepNhan.BenhNhanId = model.BenhNhanId ?? 0;
            //        }
            //        else
            //        {
            //            theVoucherYeuCauTiepNhan.BenhNhan = entity.BenhNhan;
            //        }

            //        entity.TheVoucherYeuCauTiepNhans.Add(theVoucherYeuCauTiepNhan);
            //    }
            //}

            //update voucher 
            if (model.LstMaVoucher != null && model.LstMaVoucher.Any())
            {
                foreach (var mavoucher in model.LstMaVoucher)
                {
                    // tạo ra 1 the voucher
                    var thongTin = mavoucher.Split("|");
                    if (thongTin.Any())
                    {
                        long voucherId = long.Parse(thongTin[0]);
                        string maVoucher = thongTin[1];

                        var voucher = _yeuCauTiepNhanService.getThongTinVoucher(voucherId);
                        var newTheVocuher = new TheVoucher()
                        {
                            Ma = maVoucher,
                            TuNgay = voucher.TuNgay,
                            DenNgay = voucher.DenNgay,
                            VoucherId = voucherId
                        };

                        var theVoucherYeuCauTiepNhan = new TheVoucherYeuCauTiepNhan
                        {
                            //TheVoucherId = newTheVocuher.Id,
                            TheVoucher = newTheVocuher,
                            YeuCauTiepNhan = entity
                        };

                        if (model.BenhNhanId != null && model.BenhNhanId != 0)
                        {
                            theVoucherYeuCauTiepNhan.BenhNhanId = model.BenhNhanId ?? 0;
                        }
                        else
                        {
                            theVoucherYeuCauTiepNhan.BenhNhan = entity.BenhNhan;
                        }

                        entity.TheVoucherYeuCauTiepNhans.Add(theVoucherYeuCauTiepNhan);
                    }

                }
            }

            // update feedback client 23/5/2020

            if (model.GridLichSuKCB.Any())
            {
                foreach (var item in model.GridLichSuKCB)
                {
                    var lichSu = new YeuCauTiepNhanLichSuKhamBHYT
                    {
                        KetQuaDieuTri = item.KetQuaDieuTriNumber,
                        LyDoVaoVien = item.LyDoVaoVienNumber,
                        MaCSKCB = item.MaCoSoKCB,
                        MaTheBHYT = item.MaTheBHYT,
                        NgayRa = item.NgayRaDateTime,
                        NgayVao = item.NgayVaoDateTime,
                        TenBenh = "",
                        TinhTrangRaVien = item.TinhTrangRaVienNumber,
                        YeuCauTiepNhan = entity
                    };

                    entity.YeuCauTiepNhanLichSuKhamBHYT.Add(lichSu);
                }
            }

            if (model.GridLichSuKiemTraTheBHYT.Any())
            {
                foreach (var item in model.GridLichSuKiemTraTheBHYT)
                {
                    var lichSu = new YeuCauTiepNhanLichSuKiemTraTheBHYT
                    {
                        MaUserKiemTra = item.UserKiemTra,
                        ThoiGianKiemTra = item.thoiGianKTDateTime,
                        ThongBao = item.NoiDungThongBao,
                        YeuCauTiepNhan = entity,

                    };

                    entity.YeuCauTiepNhanLichSuKiemTraTheBHYTs.Add(lichSu);
                }
            }

            //

            // await _tiepNhanBenhNhanService.AddAsync(entity);

            //cheat cho lý do khám bệnh feedback 30/7/2020
            if (entity.CoBHYT == true && entity.LyDoVaoVien == null)
            {
                entity.LyDoVaoVien = EnumLyDoVaoVien.DungTuyen;
            }
            //

            //update bacsic
            entity.Id = 0;
            foreach (var item in entity.YeuCauKhamBenhs)
            {
                item.NhomGiaDichVuKhamBenhBenhVien = null;
                item.Id = 0;
            }
            foreach (var item in entity.YeuCauDichVuGiuongBenhViens)
            {
                item.NhomGiaDichVuGiuongBenhVien = null;
                item.Id = 0;
            }
            foreach (var item in entity.YeuCauDichVuKyThuats)
            {
                item.NhomGiaDichVuKyThuatBenhVien = null;
                item.Id = 0;
            }

            await _tiepNhanBenhNhanService.PrepareDichVuAndAddAsync(entity);

            //update benh nhan
            if ((isBenhNhanExists && (model.BenhNhanId == null || model.BenhNhanId == 0)) || (model.BenhNhanId != null && model.BenhNhanId != 0))
            {
                await _tiepNhanBenhNhanService.UpdateBenhNhan(entity.BenhNhanId ?? 0, entity);
            }

            return Ok(entity.Id);

        }

        private YeuCauTiepNhan RemoveBHYT(YeuCauTiepNhan entity)
        {
            //update [BVHD-1581]
            //clear data
            //entity.ThoiGianChuyenVien = null;
            //entity.SoChuyenTuyen = null;
            //entity.TuyenChuyen = null;
            //entity.LyDoChuyen = null;
            //entity.GiayChuyenVien = null;
            entity.BHYTNgayDuocMienCungChiTra = null;

            entity.BHYTMaDKBD = null;
            entity.BHYTNgayDu5Nam = null;
            entity.BHYTMucHuong = null;
            entity.BHYTDiaChi = null;
            entity.BHYTMaSoThe = null;
            entity.BHYTCoQuanBHXH = null;
            entity.BHYTMaKhuVuc = null;
            entity.BHYTNgayHieuLuc = null;
            entity.BHYTNgayHetHan = null;

            entity.LyDoVaoVien = null;

            //clear history
            if (entity.YeuCauTiepNhanLichSuKiemTraTheBHYTs.Any())
            {
                foreach (var item in entity.YeuCauTiepNhanLichSuKiemTraTheBHYTs)
                {
                    item.WillDelete = true;
                }
            }

            if (entity.YeuCauTiepNhanLichSuKhamBHYT.Any())
            {
                foreach (var item in entity.YeuCauTiepNhanLichSuKhamBHYT)
                {
                    item.WillDelete = true;
                }
            }

            return entity;
        }

        [HttpPost("KiemTraVoucherHopLe")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult KiemTraVoucherHopLe(string maVoucher)
        {
            var result = _yeuCauTiepNhanService.KiemTraVoucherHopLe(maVoucher);
            return Ok(result);
        }

        [HttpPost("GetNoiDangKyBaoHiem")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetNoiDangKyBaoHiem(string ma)
        {
            var benhVien = await _benhVienService.GetBenhVienWithMaBenhVien(ma);
            return Ok(benhVien?.Ten);
        }

        [HttpPost("SetPhongKham")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> SetPhongKham(long KhoaKhamId)
        {
            var settings = _cauhinhService.LoadSetting<BaoHiemYTe>();

            var lookup = await _tiepNhanBenhNhanService.SettPhongKham(KhoaKhamId, settings.GioiHanSoNguoiKham);

            return Ok(lookup.FirstOrDefault()?.KeyId);
        }

        private string AddPhieuInDichVu(string content, LoaiDichVuKyThuat loaiDichVuKyThuat, YeuCauTiepNhan entity, PhieuKhamBenhViewModel data)
        {
            decimal tongCong = 0;
            int soLuong = 0;
            // BVHD-3939 // == 1 
            var thanhTienDv = entity.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null && o.LoaiDichVuKyThuat == loaiDichVuKyThuat && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan)
                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                .Sum();
            CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
            var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
            tongCong += thanhTienDv.GetValueOrDefault();

            var contentPhieuDichVuKyThuatChiDinhCanLamSang = "";
            var headerPhieuChiDinhCLS = "<table id=\'showHeader\' style=\'width: 100%; height: 40px; background: #005dab;color:#fff;\'> <tr><th rowspan = \'3\' style = \'font-size: 20px;\'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var isHave = false;
            var i = 1;
            var htmlDanhSachDichVu = SetHTMLDefine();
            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
            htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>" + loaiDichVuKyThuat.GetDescription() + "</b></td>";
            htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
            htmlDanhSachDichVu += " </tr>";
            foreach (var ycdvkt in entity.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null && o.LoaiDichVuKyThuat == loaiDichVuKyThuat && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan))
            {
                isHave = true;
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null && ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien.Ma : "") + "-" + (ycdvkt.NoiThucHien != null ? "-" + ycdvkt.NoiThucHien.Ten : "") + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                htmlDanhSachDichVu += " </tr>";
                i++;
                soLuong += ycdvkt.SoLan;
            }
            // BVHD-3939- page -total
            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
            // BVHD-3939 - số lượng
            htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
            htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

            htmlDanhSachDichVu += " </tr>";
            // end BVHD-3939
            if (isHave)
            {
                //data.NoiYeuCau = entity.NoiTiepNhan != null ? entity.NoiTiepNhan.Ten : "";
                data.NoiYeuCau = GetNoiYeuCau();

                data.ChuanDoanSoBo = "Đăng ký khám bệnh";
                //data.NguoiChiDinh = entity.NhanVienTiepNhan != null && entity.NhanVienTiepNhan.User != null ? entity.NhanVienTiepNhan.User.HoTen : "";
                data.DanhSachDichVu = htmlDanhSachDichVu;
                var html = _danhSachChoKhamService.GetBodyByName("PhieuChiDinh");

                if (content != "")
                {
                    content = content + "<div class=\"pagebreak\"> </div>";
                }
                content += headerPhieuChiDinhCLS;
                content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
            }

            return content;
        }
        private string AddPhieuInDichVuKyThuatSarsCoV2(string content, LoaiDichVuKyThuat loaiDichVuKyThuat, YeuCauTiepNhan entity,string hostingName,List<long> dichVuCanIns)
        {
            // 1 yêu cầu tiếp nhận chỉ có 1 phiếu test nhanh covid 
            var html = _danhSachChoKhamService.GetBodyByName("PhieuTestNhanhCovid");
            var headerPhieuChiDinhCLS = "<table id=\'showHeader\' style=\'width: 100%; height: 40px; background: #005dab;color:#fff;\'> <tr><th rowspan = \'3\' style = \'font-size: 20px;\'>PHIẾU CHỈ ĐỊNH XÉT NGHIỆM TEST NHANH KHÁNG NGUYÊN SARS-CoV-2 </th></tr></table>";

            var dichVuKyThuatIds = new List<long>();
            var info = _tiepNhanBenhNhanService.GetSarsCoVs();
            if (info.Result.Ids != null)
            {
                dichVuKyThuatIds = info.Result.Ids;
            }
            //entity.YeuCauDichVuKyThuats.Where(d=>d.DichVuKyThuatBenhVienId == )
            var dichVuKyThuats = entity.YeuCauDichVuKyThuats.Where(d => dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId)).ToList();
            if(dichVuCanIns.Count() > 0)
            {
                dichVuKyThuats = dichVuKyThuats.Where(d => dichVuCanIns.Contains(d.Id)).ToList();
            }
            foreach (var dvkt in dichVuKyThuats)
            {
                
                var soThe = string.Empty;
                if (entity.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                {
                    var soBHYTs = entity.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                                      .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                                      .Select(a => a.MaSoThe.ToString()).ToList();
                    soThe = soBHYTs.Any() ? soBHYTs.First() : "";
                }
                else
                {
                    soThe = entity.BHYTMaSoThe;
                }
                
                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    HoTen = entity.HoTen,
                    NamSinh = DateHelper.DOBFormat(entity.NgaySinh, entity.ThangSinh, entity.NamSinh),
                    GioiTinhString = entity.GioiTinh?.GetDescription(),
                    DiaChi = entity.DiaChiDayDu,
                    SoTheBHYT = soThe,
                    BieuHienLamSang = entity.BieuHienLamSang,
                    DichTe = entity.DichTeSarsCoV2,
                    Gio = dvkt.ThoiDiemChiDinh.Hour,
                    Phut = dvkt.ThoiDiemChiDinh.Minute,
                    Ngay = dvkt.ThoiDiemChiDinh.Day,
                    Thang = dvkt.ThoiDiemChiDinh.Month,
                    Nam = dvkt.ThoiDiemChiDinh.Year,
                    NguoiChiDinh = (dvkt.NhanVienChiDinh?.HocHamHocVi != null ? dvkt.NhanVienChiDinh?.HocHamHocVi.Ma + " " : "") + dvkt.NhanVienChiDinh?.User?.HoTen,
                    LoaiBenhPham = Enums.EnumLoaiMauXetNghiem.DichTyHau.GetDescription()
                };
                content += headerPhieuChiDinhCLS +TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                
                if (content != "")
                {
                    content = content + "<div class=\"pagebreak\"> </div>";
                }
            }


            return content;
        }

        [HttpGet("InPhieuKhamBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> InPhieuKhamBenh(long id, string hostingName)
        {
            var entity = await _tiepNhanBenhNhanService.GetByIdAsync(id, s => s.Include(u => u.BHYTGiayMienCungChiTra)
                .Include(u => u.BenhNhan)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien).ThenInclude(o => o.DichVuKhamBenh)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien).ThenInclude(o => o.DichVuKhamBenh)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.DichVuKyThuatBenhVien).ThenInclude(o => o.DichVuKyThuat)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.DichVuGiuongBenhVien).ThenInclude(o => o.DichVuGiuong)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.VatTuBenhVien).ThenInclude(o => o.VatTus)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.DuocPhamBenhVien).ThenInclude(o => o.DuocPham)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiDangKy).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhanVienChiDinh).ThenInclude(o=>o.HocHamHocVi)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(u => u.NoiTiepNhan).Include(u => u.NhanVienTiepNhan).ThenInclude(o => o.User)
                .Include(cc => cc.PhuongXa)
                .Include(cc => cc.QuanHuyen)
                .Include(cc => cc.TinhThanh)
                .Include(u => u.NguoiLienHeQuanHeNhanThan)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhanVienChiDinh).ThenInclude(o => o.HocHamHocVi)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhanVienChiDinh).ThenInclude(o=>o.User)

                .Include(o => o.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
                .Include(o => o.YeuCauKhamBenhs)?.ThenInclude(p => p.ChanDoanSoBoICD)

                //BVHD-3800
                .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            );

            var listSarsCov2CauHinh = _tiepNhanBenhNhanService.GetListSarsCauHinh();

            // cập nhật 28/12/2022
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(entity.YeuCauKhamBenhs.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(entity.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && !listSarsCov2CauHinh.Contains(s.DichVuKyThuatBenhVienId)).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận



            var now = DateTime.Now;
            var content = "";
            var contentPhieuDangKiKham = "";
            var contentPhieuDichVuKyThuatChiDinhCanLamSang = "";
            var headerPhieuDangKyKham = "<table id=\'showHeader\' style=\'width: 100%; height: 40px; background: #005dab;color:#fff;\'> <tr><th rowspan = \'3\' style = \'font-size: 20px;\'>PHIẾU ĐĂNG KÝ KHÁM</th></tr></table>";
            var headerPhieuChiDinhCLS = "<table class=\'showHeader\' style=\'width: 100%; height: 40px; background: #005dab;color:#fff;\'> <tr><th rowspan = \'3\' style = \'font-size: 20px;\'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var kiemtralandau = true;
            if (entity != null)
            {
                var ngayThangNamSinh = string.Empty;
                if (entity.NgaySinh != null && entity.NgaySinh != 0 && entity.ThangSinh != null && entity.ThangSinh != 0 && entity.NamSinh != null)
                {
                    ngayThangNamSinh = new DateTime(entity.NamSinh ?? 1500, entity.ThangSinh ?? 1, entity.NgaySinh ?? 1).ToString("dd/MM/yyyy");
                }
                else if (entity.NamSinh != null && entity.NamSinh != 0)
                {
                    ngayThangNamSinh = entity.NamSinh.ToString();
                }
                else
                {
                    ngayThangNamSinh = null;
                }
                var data = new PhieuKhamBenhViewModel
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(entity.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(entity.MaYeuCauTiepNhan) : "",
                    MaTN = entity.MaYeuCauTiepNhan,
                    MaBN = entity.BenhNhan != null ? entity.BenhNhan.MaBN : "",
                    Ngay = entity.ThoiDiemTiepNhan.ToString("dd"),
                    Thang = entity.ThoiDiemTiepNhan.ToString("MM"),
                    Nam = entity.ThoiDiemTiepNhan.ToString("yyyy"),
                    HoTen = entity.HoTen,
                    GioiTinh = entity?.GioiTinh,
                    NamSinh = ngayThangNamSinh,
                    DiaChi = entity.DiaChiDayDu,
                    DienThoai = entity.SoDienThoai.ApplyFormatPhone(),
                    DoiTuong = entity.CoBHYT != true ? "Viện phí" : "BHYT (" + entity.BHYTMucHuong.ToString() + "%)",
                    SoTheBHYT = entity.BHYTMaSoThe,// + (entity.BHYTMaDKBD == null ? null : " - " + entity.BHYTMaDKBD),
                    HanThe = (entity.BHYTNgayHieuLuc != null || entity.BHYTNgayHetHan != null) ? "từ ngày: " + (entity.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (entity.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                    Now = now.ApplyFormatDateTime(),
                    NowTime = now.ApplyFormatTime(),
                    NgayIn = now.ApplyFormatTime() + " Ngày " + now.ToString("dd") + " tháng " + now.ToString("MM") + " năm " + now.ToString("yyyy"),
                    NguoiGiamHo = entity.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = entity.NguoiLienHeQuanHeNhanThan?.Ten,
                    //GioKhamDuKien = _danhSachChoKhamService.ThoiDiemKhamDuKien(id).Find(p => p.Id == id).ThoiDiemDuKiens.ApplyFormatTime(),

                };
                data.NguoiChiDinh = entity.NhanVienTiepNhan != null && entity.NhanVienTiepNhan.User != null ? entity.NhanVienTiepNhan.User.HoTen : "";
                // BVHD-3705
                if (entity.YeuCauKhamBenhs.Any(o => o.DichVuKhamBenhBenhVien != null) ||
                    entity.YeuCauDichVuKyThuats.Any(o => o.DichVuKyThuatBenhVien != null && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung))
                {
                    //BVHD-3800
                    data.LaCapCuu = entity.LaCapCuu ?? entity.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

                    if (entity.YeuCauKhamBenhs.Any(o => o.DichVuKhamBenhBenhVien != null))
                    {
                        var html = _danhSachChoKhamService.GetBodyByName("PhieuDangKyKham");
                        foreach (var yckb in entity.YeuCauKhamBenhs.Where(o => o.DichVuKhamBenhBenhVien != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan))
                        {
                            //data.DangKyKham = yckb.TenDichVu + "-" + (yckb.NoiDangKy != null && yckb.NoiDangKy.KhoaPhong != null ? yckb.NoiDangKy.KhoaPhong.Ten : "") + "-" + (yckb.NoiDangKy != null ? yckb.NoiDangKy.Ten : "");
                            data.DangKyKham = yckb.TenDichVu + (yckb.NoiDangKy != null &&
                                                             yckb.NoiDangKy.KhoaPhong != null ? " - " + yckb.NoiDangKy.KhoaPhong.Ten : "")
                                                             + (yckb.NoiDangKy != null ? " - P" + yckb.NoiDangKy.Ma : "");
                            if (content != "")
                            {
                                content = content + "<div class=\"pagebreak\"> </div>";
                            }
                            content += headerPhieuDangKyKham + TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                        }
                    }
                    // phiếu đăng ký khám dành cho dịch vụ kỹ thuật  thuộc nhóm sàng lọc tiêm chủng 
                    if (entity.YeuCauDichVuKyThuats.Any(o => o.DichVuKyThuatBenhVien != null && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung))
                    {
                        var html = _danhSachChoKhamService.GetBodyByName("PhieuDangKyKham");
                        foreach (var yckb in entity.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung))
                        {
                            data.DangKyKham = yckb.TenDichVu + (yckb.NoiThucHien != null &&
                                                             yckb.NoiThucHien.KhoaPhong != null ? " - " + yckb.NoiThucHien.KhoaPhong.Ten : "")
                                                             + (yckb.NoiThucHien != null ? " - P" + yckb.NoiThucHien.Ma : "");
                            if (content != "")
                            {
                                content = content + "<div class=\"pagebreak\"> </div>";
                            }
                            content += headerPhieuDangKyKham + TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                        }
                    }

                    #region BVHD-3761
                    var dichVuKyThuatIds = new List<long>();
                    var info = _tiepNhanBenhNhanService.GetSarsCoVs();
                    if (info.Result.Ids != null)
                    {
                        dichVuKyThuatIds = info.Result.Ids;
                    }
                    if (entity.YeuCauDichVuKyThuats.Where(d => dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId)).ToList() != null)
                    {
                        if (!string.IsNullOrEmpty(content))
                        {
                            content = content + "<div class=\"pagebreak\"> </div>";
                        }
                        var listdichVuCanIns = new List<long>();
                        content = AddPhieuInDichVuKyThuatSarsCoV2(content, LoaiDichVuKyThuat.XetNghiem, entity, hostingName, listdichVuCanIns);
                    }

                    #endregion BVHD-3761
                    #region dịch vụ kỹ thuật
                    if (entity.YeuCauDichVuKyThuats.Any(o => o.DichVuKyThuatBenhVien != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan && o.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung))
                    {
                        //BVHD-3800
                        data.LaCapCuu = entity.LaCapCuu ?? entity.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.ChuanDoanHinhAnh, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.ThuThuatPhauThuat, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.Khac, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.ThamDoChucNang, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.TheoYeuCau, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.XetNghiem, entity, data);

                        var listdichVuCanIns = new List<long>();
                        content = AddPhieuInDichVuKyThuatSarsCoV2(content, LoaiDichVuKyThuat.XetNghiem, entity, hostingName, listdichVuCanIns);
                    }
                    #endregion

                    #region In Chung Tất cả dịch vụ được kê
                    if(entity.YeuCauKhamBenhs.Where(o => o.DichVuKhamBenhBenhVien != null).ToList().Count() >= 2 ||

                       (entity.YeuCauKhamBenhs.Where(o => o.DichVuKhamBenhBenhVien != null).ToList().Count() >= 1 && 
                        entity.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && !listSarsCov2CauHinh.Contains(s.DichVuKyThuatBenhVienId)).ToList().Count() >= 1) ||

                        entity.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && !listSarsCov2CauHinh.Contains(s.DichVuKyThuatBenhVienId)).ToList().Count() >= 2)
                    {
                        var listTheoNguoiChiDinh = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                        //Khi tạo yêu cầu tiếp nhận thì chỉ có duy nhất 1 Người tạo và thêm dịch vụ
                        foreach (var yckb in listDVK.ToList())
                        {
                            var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                            objNguoiChidinh.dichVuChiDinhId = yckb.Id;
                            objNguoiChidinh.nhomChiDinhId = (int)EnumNhomGoiDichVu.DichVuKhamBenh;
                            objNguoiChidinh.TenNhom = EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription();
                            objNguoiChidinh.NhanVienChiDinhId = yckb.NhanVienChiDinhId;
                            objNguoiChidinh.ThoiDiemChiDinh = new DateTime(yckb.ThoiDiemDangKy.Year, yckb.ThoiDiemDangKy.Month, yckb.ThoiDiemDangKy.Day, 0, 0, 0);
                            listTheoNguoiChiDinh.Add(objNguoiChidinh);
                        }


                        foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null).ToList())
                        {
                            var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                            objNguoiChidinh.dichVuChiDinhId = ycdvkt.Id;  // to do
                            objNguoiChidinh.nhomChiDinhId = (int)EnumNhomGoiDichVu.DichVuKyThuat;  // to do
                            objNguoiChidinh.TenNhom = ycdvkt.LoaiDichVuKyThuat.GetDescription();
                            objNguoiChidinh.NhanVienChiDinhId = ycdvkt.NhanVienChiDinhId;
                            objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0);

                            listTheoNguoiChiDinh.Add(objNguoiChidinh);

                        }


                        var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();



                        foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                        {
                            var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                            listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                            content = content + "<div class=\"pagebreak\"> </div>";
                            content = _tiepNhanBenhNhanService.AddChiDinhKhamBenhTheoNguoiChiDinhVaNhom(id, listCanIn, listDVK, listDVKT, content, "", hostingName);
                        }
                    }
          
                    #endregion
                }
                else
                {
                    //if (entity.YeuCauKhamBenhs.Any(o => o.DichVuKhamBenhBenhVien != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan))
                    //{
                    //    //BVHD-3800
                    //    data.LaCapCuu = entity.LaCapCuu ?? entity.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

                    //    //foreach (var yckb in entity.YeuCauKhamBenhs.Where(o => o.DichVuKhamBenhBenhVien != null))
                    //    //{
                    //    //    var htmlDanhSachDichVu = SetHTMLDefine();
                    //    //    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    //    //    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>KHÁM BỆNH</b></td>";
                    //    //    htmlDanhSachDichVu += " </tr>";
                    //    //    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    //    //    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    //    //    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.DichVuKhamBenhBenhVien.Ten + "</td>";
                    //    //    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null && yckb.NoiDangKy.KhoaPhong != null ? yckb.NoiDangKy.KhoaPhong.Ten : "") + "-" + (yckb.NoiDangKy != null ? yckb.NoiDangKy.Ten : "") + "</td>";
                    //    //    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>1</td>";
                    //    //    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //    //    //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //    //    //htmlDanhSachDichVu += " </tr>";
                    //    //    i++;
                    //    //    data.NoiYeuCau = entity.NoiTiepNhan != null ? entity.NoiTiepNhan.Ten : "";
                    //    //    data.ChuanDoanSoBo = "Đăng ký khám bệnh";
                    //    //    //data.NguoiChiDinh = entity.NhanVienTiepNhan != null && entity.NhanVienTiepNhan.User != null ? entity.NhanVienTiepNhan.User.HoTen : "";
                    //    //    data.DanhSachDichVu = htmlDanhSachDichVu;
                    //    //    var html = _danhSachChoKhamService.GetBodyByName("PhieuDangKyKham");
                    //    //    content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data) + "<div class=\"pagebreak\"> </div>";
                    //    //}
                    //    var html = _danhSachChoKhamService.GetBodyByName("PhieuDangKyKham");
                    //    foreach (var yckb in entity.YeuCauKhamBenhs.Where(o => o.DichVuKhamBenhBenhVien != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan))
                    //    {
                    //        //data.DangKyKham = yckb.TenDichVu + "-" + (yckb.NoiDangKy != null && yckb.NoiDangKy.KhoaPhong != null ? yckb.NoiDangKy.KhoaPhong.Ten : "") + "-" + (yckb.NoiDangKy != null ? yckb.NoiDangKy.Ten : "");
                    //        data.DangKyKham = yckb.TenDichVu + (yckb.NoiDangKy != null &&
                    //                                            yckb.NoiDangKy.KhoaPhong != null ? " - " + yckb.NoiDangKy.KhoaPhong.Ten : "")
                    //                          + (yckb.NoiDangKy != null ? " - P" + yckb.NoiDangKy.Ma : "");
                    //        if (content != "")
                    //        {
                    //            content = content + "<div class=\"pagebreak\"> </div>";
                    //        }
                    //        content += headerPhieuDangKyKham + TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                    //    }

                    //}
                    if (entity.YeuCauDichVuKyThuats.Any(o => o.DichVuKyThuatBenhVien != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan))
                    {
                        //BVHD-3800
                        data.LaCapCuu = entity.LaCapCuu ?? entity.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.ChuanDoanHinhAnh, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.ThuThuatPhauThuat, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.Khac, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.ThamDoChucNang, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.TheoYeuCau, entity, data);
                        content = AddPhieuInDichVu(content, LoaiDichVuKyThuat.XetNghiem, entity, data);

                        var listdichVuCanIns = new List<long>();
                        content = AddPhieuInDichVuKyThuatSarsCoV2(content, LoaiDichVuKyThuat.XetNghiem, entity,hostingName, listdichVuCanIns);

                        #region In Chung Tất cả dịch vụ được kê
                        if(entity.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan).ToList().Count() >= 2 )
                        {
                            var listTheoNguoiChiDinh = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                            //Khi tạo yêu cầu tiếp nhận thì chỉ có duy nhất 1 Người tạo và thêm dịch vụ
                            foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null).ToList())
                            {
                                var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                                objNguoiChidinh.dichVuChiDinhId = ycdvkt.Id;  // to do
                                objNguoiChidinh.nhomChiDinhId = (int)EnumNhomGoiDichVu.DichVuKyThuat;  // to do
                                objNguoiChidinh.TenNhom = ycdvkt.LoaiDichVuKyThuat.GetDescription();
                                objNguoiChidinh.NhanVienChiDinhId = ycdvkt.NhanVienChiDinhId;
                                objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0);

                                listTheoNguoiChiDinh.Add(objNguoiChidinh);

                            }

                            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();



                            foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
                            {
                                var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                                listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                                content = content + "<div class=\"pagebreak\"> </div>";
                                content = _tiepNhanBenhNhanService.AddChiDinhKhamBenhTheoNguoiChiDinhVaNhom(id, listCanIn, listDVK, listDVKT, content, "", hostingName);

                                
                            }
                        }
                     
                        #endregion
                    }
                    //if (entity.YeuCauDichVuGiuongBenhViens.Any(o => o.DichVuGiuongBenhVien != null))
                    //{
                    //    var i = 1;
                    //    var htmlDanhSachDichVu = SetHTMLDefine();
                    //    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    //    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>DỊCH VỤ GIƯỜNG</b></td>";
                    //    htmlDanhSachDichVu += " </tr>";
                    //    foreach (var ycdvg in entity.YeuCauDichVuGiuongBenhViens.Where(o => o.DichVuGiuongBenhVien != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan))
                    //    {
                    //        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvg.DichVuGiuongBenhVien.Ten + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvg.NoiThucHien != null && ycdvg.NoiThucHien != null ? ycdvg.NoiThucHien.Ma : "") + "-" + (ycdvg.NoiThucHien != null ? "-" + ycdvg.NoiThucHien.Ten : "") + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>1</td>";
                    //        //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //        //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //        htmlDanhSachDichVu += " </tr>";
                    //        i++;
                    //    }
                    //    //data.NoiYeuCau = entity.NoiTiepNhan != null ? entity.NoiTiepNhan.Ten : "";
                    //    data.NoiYeuCau = GetNoiYeuCau();

                    //    data.ChuanDoanSoBo = "Đăng ký khám bệnh";
                    //    //data.NguoiChiDinh = entity.NhanVienTiepNhan != null && entity.NhanVienTiepNhan.User != null ? entity.NhanVienTiepNhan.User.HoTen : "";
                    //    data.DanhSachDichVu = htmlDanhSachDichVu;
                    //    var html = _danhSachChoKhamService.GetBodyByName("PhieuChiDinh");
                    //    if (content != "")
                    //    {
                    //        content = content + "<div class=\"pagebreak\"> </div>";
                    //    }
                    //    content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                    //}
                    //if (entity.YeuCauVatTuBenhViens.Any(o => o.VatTuBenhVien != null && o.VatTuBenhVien.VatTus != null))
                    //{
                    //    var i = 1;
                    //    var htmlDanhSachDichVu = SetHTMLDefine();
                    //    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    //    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>VẬT TƯ TIÊU HAO</b></td>";
                    //    htmlDanhSachDichVu += " </tr>";
                    //    foreach (var ycvt in entity.YeuCauVatTuBenhViens.Where(o => o.VatTuBenhVien != null && o.VatTuBenhVien.VatTus != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan))
                    //    {
                    //        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycvt.VatTuBenhVien.VatTus.Ten + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycvt.NoiCapVatTu != null && ycvt.NoiCapVatTu != null ? ycvt.NoiCapVatTu.Ma : "") + "-" + (ycvt.NoiCapVatTu != null ? "-" + ycvt.NoiCapVatTu.Ten : "") + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycvt.SoLuong + "</td>";
                    //        //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //        //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //        htmlDanhSachDichVu += " </tr>";
                    //        i++;
                    //    }
                    //    //data.NoiYeuCau = entity.NoiTiepNhan != null ? entity.NoiTiepNhan.Ten : "";
                    //    data.NoiYeuCau = GetNoiYeuCau();

                    //    data.ChuanDoanSoBo = "Đăng ký khám bệnh";
                    //    //data.NguoiChiDinh = entity.NhanVienTiepNhan != null && entity.NhanVienTiepNhan.User != null ? entity.NhanVienTiepNhan.User.HoTen : "";
                    //    data.DanhSachDichVu = htmlDanhSachDichVu;
                    //    var html = _danhSachChoKhamService.GetBodyByName("PhieuChiDinh");
                    //    if (content != "")
                    //    {
                    //        content = content + "<div class=\"pagebreak\"> </div>";
                    //    }
                    //    content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                    //}
                    //if (entity.YeuCauDuocPhamBenhViens.Any(o => o.DuocPhamBenhVien != null && o.DuocPhamBenhVien.DuocPham != null))
                    //{
                    //    var i = 1;
                    //    var htmlDanhSachDichVu = SetHTMLDefine();
                    //    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    //    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>DƯỢC PHẨM</b></td>";
                    //    htmlDanhSachDichVu += " </tr>";
                    //    foreach (var ycdp in entity.YeuCauDuocPhamBenhViens.Where(o => o.DuocPhamBenhVien != null && o.DuocPhamBenhVien.DuocPham != null && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan))
                    //    {
                    //        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdp.DuocPhamBenhVien.DuocPham.Ten + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdp.NoiCapThuoc != null && ycdp.NoiCapThuoc.KhoaPhong != null ? ycdp.NoiCapThuoc.KhoaPhong.Ten : "") + "-" + (ycdp.NoiCapThuoc != null ? "-" + ycdp.NoiCapThuoc.Ten : "") + "</td>";
                    //        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdp.SoLuong + "</td>";
                    //        //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //        //htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
                    //        htmlDanhSachDichVu += " </tr>";
                    //        i++;
                    //    }
                    //    //update noi yeu cau

                    //    //data.NoiYeuCau = entity.NoiTiepNhan != null ? entity.NoiTiepNhan.Ten : "";
                    //    data.NoiYeuCau = GetNoiYeuCau();

                    //    data.ChuanDoanSoBo = "Đăng ký khám bệnh";
                    //    //data.NguoiChiDinh = entity.NhanVienTiepNhan != null && entity.NhanVienTiepNhan.User != null ? entity.NhanVienTiepNhan.User.HoTen : "";
                    //    data.DanhSachDichVu = htmlDanhSachDichVu;
                    //    var html = _danhSachChoKhamService.GetBodyByName("PhieuChiDinh");
                    //    if (content != "")
                    //    {
                    //        content = content + "<div class=\"pagebreak\"> </div>";
                    //    }
                    //    content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
                    //}

                }

                if (string.IsNullOrEmpty(entity.NguoiLienHeHoTen))
                {
                    content = content.Replace("id=\"NguoiGiamHo\"", "id=\"NguoiGiamHo\" style=\"display: none\"");
                }
            }
            if(content.Length <= headerPhieuDangKyKham.Length)
            {
                _logger.LogWarn($"InPhieuKhamBenh {id}");
            }
            return Ok(content);

        }

        private string GetNoiYeuCau()
        {
            var result = string.Empty;
            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVienEntity = _tiepNhanBenhNhanService.GetPhongBenhVien(phongBenhVienId).Result;
            if (phongBenhVienEntity != null)
            {
                result = phongBenhVienEntity?.Ma + " - " + phongBenhVienEntity?.Ten;
            }

            return result;
        }
        private string SetHTMLDefine()
        {
            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";

            return htmlDanhSachDichVu;
        }

        [HttpPost("UpdateYeuCauKhamBenh")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> UpdateYeuCauKhamBenh(TiepNhanBenhNhanViewModel model)
        {
            //  if (model != null && model.IsOutOfDate)
            //  {
            //      throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.MaBHYT.OutOfDate"));

            //  }

            if (model != null && model.IsCheckedBHYT == true && model.IsOutOfDate && model.CoBHYT == true)
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.MaBHYT.OutOfDate"));
            }

            if (model != null && model.BHYTMucHuong == null && model.IsCheckedBHYT == true && model.CoBHYT == true)
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.MaBHYT.RemoveBHYT"));

            }

            if (model != null && model.CoBHYT == true)
            {
                var kiemTra = await _tiepNhanBenhNhanService.KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(model.BenhNhanId ?? 0, false, model.Id);
                if (!string.IsNullOrEmpty(kiemTra.ErrorMessage))
                {
                    throw new ApiException(kiemTra.ErrorMessage);
                }
            }

            //if (!model.YeuCauKhacGrid.Any())
            //{
            //    throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.LoaiYeuCauTiepNhanMaDichVu.Required"));
            //}

            //
            if (model.YeuCauKhacGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId)))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ChiDinhDichVu.NoiThucHienRequired"));
            }

            if (model.YeuCauKyThuatGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId))
               || model.YeuCauDichVuKyThuats.Any(p => p.SoLan == null || p.SoLan == 0)
               || model.YeuCauKhacGrid.Any(p => p.SoLuong == 0 || p.SoLuong == null))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ChiDinhDichVu.SoLuongRequired"));
            }

            if (model.YeuCauKyThuatGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId))
                || model.YeuCauDichVuKyThuats.Any(p => p.NhomGiaDichVuKyThuatBenhVienId == null || p.NhomGiaDichVuKyThuatBenhVienId == 0)
                || model.YeuCauKhacGrid.Any(p => p.LoaiGiaId == 0))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ChiDinhDichVu.LoaiGiaRequired"));
            }

            ////
            //if (model.YeuCauKhacGrid.Any(p => string.IsNullOrEmpty(p.NoiThucHienId)))
            //{
            //    throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.ChiDinhDichVu.NoiThucHienRequired"));
            //}
            //
            var isBenhNhanExists = false;

            //

            if (model.HinhThucDenId != 2)
            {
                model.NoiGioiThieuId = null;
            }

            if (await _tiepNhanBenhNhanService.IsUnder6YearOld(model.NgayThangNamSinh, model.NamSinh))
            {
                model.NguoiLienHeTinhThanhId = model.TinhThanhId;
                model.NguoiLienHeQuanHuyenId = model.QuanHuyenId;
                model.NguoiLienHePhuongXaId = model.PhuongXaId;
                model.NguoiLienHeDiaChi = model.DiaChi;
            }


            var yeuCauTiepNhan = await _tiepNhanBenhNhanService.GetByIdHaveIncludeForUpdate(model.Id);



            //
            if (!_roleService.IsHavePermissionForUpdateInformationTNBN())
            {
                model.HoTen = yeuCauTiepNhan.HoTen;
                if (yeuCauTiepNhan.ThangSinh != 0 && yeuCauTiepNhan.ThangSinh != null && yeuCauTiepNhan.NamSinh != null)
                {
                    model.NgayThangNamSinh = new DateTime(yeuCauTiepNhan.NamSinh ?? 0, yeuCauTiepNhan.ThangSinh ?? 0, yeuCauTiepNhan.NgaySinh ?? 0);
                }
                model.NamSinh = yeuCauTiepNhan.NamSinh;
                model.SoChungMinhThu = yeuCauTiepNhan.SoChungMinhThu;
                model.GioiTinh = yeuCauTiepNhan.GioiTinh;
                model.NgheNghiepId = yeuCauTiepNhan.NgheNghiepId;
                model.QuocTichId = yeuCauTiepNhan.QuocTichId;
                model.TinhThanhId = yeuCauTiepNhan.TinhThanhId;
                model.QuanHuyenId = yeuCauTiepNhan.QuanHuyenId;
                model.PhuongXaId = yeuCauTiepNhan.PhuongXaId;
                model.DiaChi = yeuCauTiepNhan.DiaChi;
                model.SoDienThoai = yeuCauTiepNhan.SoDienThoai;
                model.Email = yeuCauTiepNhan.Email;
                model.NoiLamViec = yeuCauTiepNhan.NoiLamViec;
                model.DanTocId = yeuCauTiepNhan.DanTocId;

                model.NguoiLienHeHoTen = yeuCauTiepNhan.NguoiLienHeHoTen;
                model.NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId;
                model.NguoiLienHeSoDienThoai = yeuCauTiepNhan.NguoiLienHeSoDienThoai;
                model.NguoiLienHeEmail = yeuCauTiepNhan.NguoiLienHeEmail;
                model.NguoiLienHeTinhThanhId = yeuCauTiepNhan.NguoiLienHeTinhThanhId;
                model.NguoiLienHeQuanHuyenId = yeuCauTiepNhan.NguoiLienHeQuanHuyenId;
                model.NguoiLienHePhuongXaId = yeuCauTiepNhan.NguoiLienHePhuongXaId;
                model.NguoiLienHeDiaChi = yeuCauTiepNhan.NguoiLienHeDiaChi;
            }


            if (model.DuocChuyenVien == true)
            {
                //remove s3 file
                if ((model.GiayChuyenVien == null && yeuCauTiepNhan.GiayChuyenVien != null)
                    || (model.GiayChuyenVien != null && yeuCauTiepNhan.GiayChuyenVien != null &&
                        !model.GiayChuyenVien.TenGuid.Equals(yeuCauTiepNhan.GiayChuyenVien.TenGuid)))
                {
                    //remove attachment in s3
                    try
                    {
                        await _taiLieuDinhKemService.XoaTaiLieuAsync(yeuCauTiepNhan.GiayChuyenVien.DuongDan, yeuCauTiepNhan.GiayChuyenVien.TenGuid);
                    }
                    catch (Exception e)
                    {
                    }
                }

                //save s3 file
                if ((model.GiayChuyenVien != null && yeuCauTiepNhan.GiayChuyenVien != null &&
                     !model.GiayChuyenVien.TenGuid.Equals(yeuCauTiepNhan.GiayChuyenVien.TenGuid)) ||
                    (model.GiayChuyenVien != null && yeuCauTiepNhan.GiayChuyenVien == null))
                {
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(model.GiayChuyenVien.DuongDan, model.GiayChuyenVien.TenGuid);
                }

                if (model.GiayChuyenVien == null && yeuCauTiepNhan.GiayChuyenVien != null)
                {
                    yeuCauTiepNhan.GiayChuyenVien.WillDelete = true;
                    model.GiayChuyenVienId = null;
                }
            }
            else
            {
                if (yeuCauTiepNhan.GiayChuyenVien != null)
                {
                    //remove s3 file
                    try
                    {
                        await _taiLieuDinhKemService.XoaTaiLieuAsync(yeuCauTiepNhan.GiayChuyenVien.DuongDan, yeuCauTiepNhan.GiayChuyenVien.TenGuid);
                    }
                    catch (Exception e)
                    {
                    }
                    yeuCauTiepNhan.GiayChuyenVien.WillDelete = true;
                    model.GiayChuyenVienId = null;
                }
                model.ThoiGianChuyenVien = null;
                model.SoChuyenTuyen = null;
                model.NoiChuyenId = null;
                model.LyDoChuyen = null;
            }

            if (model.BHYTDuocMienCungChiTra == true)
            {
                //remove s3 file
                if ((model.BHYTGiayMienCungChiTra == null && yeuCauTiepNhan.BHYTGiayMienCungChiTra != null)
                    || (model.BHYTGiayMienCungChiTra != null && yeuCauTiepNhan.BHYTGiayMienCungChiTra != null &&
                        !model.BHYTGiayMienCungChiTra.TenGuid.Equals(yeuCauTiepNhan.BHYTGiayMienCungChiTra.TenGuid)))
                {
                    //remove attachment in s3
                    try
                    {
                        await _taiLieuDinhKemService.XoaTaiLieuAsync(yeuCauTiepNhan.BHYTGiayMienCungChiTra.DuongDan, yeuCauTiepNhan.BHYTGiayMienCungChiTra.TenGuid);
                    }
                    catch (Exception e)
                    {
                    }
                }

                //save s3 file
                if ((model.BHYTGiayMienCungChiTra != null && yeuCauTiepNhan.BHYTGiayMienCungChiTra != null &&
                     !model.BHYTGiayMienCungChiTra.TenGuid.Equals(yeuCauTiepNhan.BHYTGiayMienCungChiTra.TenGuid)) ||
                    (model.BHYTGiayMienCungChiTra != null && yeuCauTiepNhan.BHYTGiayMienCungChiTra == null))
                {
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(model.BHYTGiayMienCungChiTra.DuongDan, model.BHYTGiayMienCungChiTra.TenGuid);
                }

                if (model.BHYTGiayMienCungChiTra == null && yeuCauTiepNhan.BHYTGiayMienCungChiTra != null)
                {
                    yeuCauTiepNhan.BHYTGiayMienCungChiTra.WillDelete = true;
                    model.BHYTGiayMienCungChiTra = null;
                }
            }
            else
            {
                if (yeuCauTiepNhan.BHYTGiayMienCungChiTra != null)
                {
                    //remove s3 file
                    try
                    {
                        await _taiLieuDinhKemService.XoaTaiLieuAsync(yeuCauTiepNhan.BHYTGiayMienCungChiTra.DuongDan, yeuCauTiepNhan.BHYTGiayMienCungChiTra.TenGuid);
                    }
                    catch (Exception e)
                    {
                    }
                    yeuCauTiepNhan.BHYTGiayMienCungChiTra.WillDelete = true;
                    model.BHYTGiayMienCungChiTraId = null;
                }
                model.BHYTNgayDuocMienCungChiTra = null;
            }

            #region add tai lieu dinh kem
            foreach (var entity in yeuCauTiepNhan.HoSoYeuCauTiepNhans)
            {
                if (!model.HoSoYeuCauTiepNhans.Any(p => p.Id == entity.Id))
                {
                    //remove attachment in s3
                    try
                    {
                        await _taiLieuDinhKemService.XoaTaiLieuAsync(entity.DuongDan, entity.TenGuid);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            foreach (var item in model.HoSoYeuCauTiepNhans)
            {
                if (item.Id == 0)
                {
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                }
            }
            #endregion add tai lieu dinh kem

            if (model.DuocUuDai != true)
            {
                model.DoiTuongUuDaiId = null;
                model.CongTyUuDaiId = null;
                model.LstVoucherId = new List<long>();
                model.LstMaVoucher = new List<string>();
            }
            //if (model.DuocUuDai != true || (model.DuocUuDai == true && model.LoaiMienGiam == 1))
            //{
            //    model.LstVoucherId = new List<long>();
            //    model.LstMaVoucher = new List<string>();
            //}

            #region remove chi dinh dich vu

            model.YeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuatTiepNhanViewModel>();
            //model.YeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVienTiepNhanViewModel>();
            model.YeuCauKhamBenhs = new List<YeuCauKhamBenhTiepNhanViewModel>();
            //model.YeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVienTiepNhanViewModel>();

            #endregion remove chi dinh dich vu

            var theBHYTCuTrongYCTN = yeuCauTiepNhan.BHYTMaSoThe;
            model.ToEntity(yeuCauTiepNhan);

            if (model.CoBHYT != true)
            {
                //clear data
                yeuCauTiepNhan = RemoveBHYT(yeuCauTiepNhan);
            }
            else
            {
                //cheat cho lý do khám bệnh feedback 30/7/2020
                if (yeuCauTiepNhan.LyDoVaoVien == null)
                {
                    yeuCauTiepNhan.LyDoVaoVien = EnumLyDoVaoVien.DungTuyen;
                }
                //
                if (!yeuCauTiepNhan.YeuCauTiepNhanLichSuKiemTraTheBHYTs.Any() && model.GridLichSuKiemTraTheBHYT.Any())
                {
                    foreach (var item in model.GridLichSuKiemTraTheBHYT)
                    {
                        var lichSu = new YeuCauTiepNhanLichSuKiemTraTheBHYT
                        {
                            MaUserKiemTra = item.UserKiemTra,
                            ThoiGianKiemTra = item.thoiGianKTDateTime,
                            ThongBao = item.NoiDungThongBao,
                            YeuCauTiepNhan = yeuCauTiepNhan,

                        };

                        yeuCauTiepNhan.YeuCauTiepNhanLichSuKiemTraTheBHYTs.Add(lichSu);
                    }
                }

                if (!yeuCauTiepNhan.YeuCauTiepNhanLichSuKhamBHYT.Any() && model.GridLichSuKCB.Any())
                {
                    foreach (var item in model.GridLichSuKCB)
                    {
                        var lichSu = new YeuCauTiepNhanLichSuKhamBHYT
                        {
                            KetQuaDieuTri = item.KetQuaDieuTriNumber,
                            LyDoVaoVien = item.LyDoVaoVienNumber,
                            MaCSKCB = item.MaCoSoKCB,
                            MaTheBHYT = item.MaTheBHYT,
                            NgayRa = item.NgayRaDateTime,
                            NgayVao = item.NgayVaoDateTime,
                            TenBenh = "",
                            TinhTrangRaVien = item.TinhTrangRaVienNumber,
                            YeuCauTiepNhan = yeuCauTiepNhan
                        };

                        yeuCauTiepNhan.YeuCauTiepNhanLichSuKhamBHYT.Add(lichSu);
                    }
                }
            }


            //if (model.LstVoucherId != null)
            //{
            //    foreach (var voucherId in model.LstVoucherId)
            //    {
            //        //add voucher
            //        if (!yeuCauKhamBenh.TheVoucherYeuCauTiepNhans.Any(p => p.TheVoucherId == voucherId))
            //        {

            //            var theVoucherYeuCauTiepNhan = new TheVoucherYeuCauTiepNhan
            //            {
            //                TheVoucherId = voucherId,
            //                YeuCauTiepNhan = yeuCauKhamBenh
            //            };

            //            if (model.BenhNhanId != null && model.BenhNhanId != 0)
            //            {
            //                theVoucherYeuCauTiepNhan.BenhNhanId = model.BenhNhanId ?? 0;
            //            }
            //            else
            //            {
            //                theVoucherYeuCauTiepNhan.BenhNhan = yeuCauKhamBenh.BenhNhan;
            //            }

            //            yeuCauKhamBenh.TheVoucherYeuCauTiepNhans.Add(theVoucherYeuCauTiepNhan);
            //        }
            //    }
            //}

            if (model.LstMaVoucher != null && model.LstMaVoucher.Any())
            {
                foreach (var mavoucher in model.LstMaVoucher)
                {
                    //if () continue;                   
                    var thongTin = mavoucher.Split("|");
                    if (thongTin.Any())
                    {
                        long voucherId = long.Parse(thongTin[0]);
                        string maVoucher = thongTin[1];
                        var kiemTraTheVoucher = _yeuCauTiepNhanService.kiemTraTheVoucher(maVoucher);
                        if (!kiemTraTheVoucher)
                        {
                            var voucher = _yeuCauTiepNhanService.getThongTinVoucher(voucherId);
                            var newTheVocuher = new TheVoucher()
                            {
                                Ma = maVoucher,
                                TuNgay = voucher.TuNgay,
                                DenNgay = voucher.DenNgay,
                                VoucherId = voucherId
                            };

                            var theVoucherYeuCauTiepNhan = new TheVoucherYeuCauTiepNhan
                            {
                                TheVoucher = newTheVocuher,
                                YeuCauTiepNhan = yeuCauTiepNhan
                            };

                            if (model.BenhNhanId != null && model.BenhNhanId != 0)
                            {
                                theVoucherYeuCauTiepNhan.BenhNhanId = model.BenhNhanId ?? 0;
                            }
                            else
                            {
                                theVoucherYeuCauTiepNhan.BenhNhan = yeuCauTiepNhan.BenhNhan;
                            }

                            yeuCauTiepNhan.TheVoucherYeuCauTiepNhans.Add(theVoucherYeuCauTiepNhan);
                        }

                    }
                }
            }

            foreach (var voucher in yeuCauTiepNhan.TheVoucherYeuCauTiepNhans)
            {
                var idCompare = voucher.TheVoucher.VoucherId + "|" + voucher.TheVoucher.Ma;
                if (!model.LstMaVoucher.Any(p => p == idCompare))
                {
                    voucher.WillDelete = true;
                    voucher.TheVoucher.WillDelete = true;
                }
            }

            if (yeuCauTiepNhan.CoBHTN == true)
            {
                //remove BHTN khong ton tai
                foreach (var bhtnEntity in yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                {
                    if (!model.BaoHiemTuNhans.Any(p => p.Id == bhtnEntity.Id))
                    {
                        //validate
                        var isHaveCongNo = await _tiepNhanBenhNhanService.IsHaveCongNo(yeuCauTiepNhan.Id, bhtnEntity.Id);
                        if (isHaveCongNo)
                        {
                            throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.BHTN.IsHaveCongNo"));
                        }
                        //
                        bhtnEntity.WillDelete = true;
                    }
                }
                //them BHTN moi hoac cap nhat
                foreach (var bhtn in model.BaoHiemTuNhans)
                {
                    //them BHTN moi
                    if (bhtn.Id == 0)
                    {
                        var baoHiemTuNhan = new YeuCauTiepNhanCongTyBaoHiemTuNhan
                        {
                            CongTyBaoHiemTuNhanId = bhtn.BHTNCongTyBaoHiemId ?? 0,
                            DiaChi = bhtn.BHTNDiaChi,
                            SoDienThoai = bhtn.BHTNSoDienThoai,
                            NgayHetHan = bhtn.BHTNNgayHetHan,
                            NgayHieuLuc = bhtn.BHTNNgayHieuLuc,
                            MaSoThe = bhtn.BHTNMaSoThe,
                            Id = bhtn.Id
                        };
                        yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Add(baoHiemTuNhan);
                    }
                    //update BHTN
                    else
                    {
                        var item = yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.FirstOrDefault(p => p.Id == bhtn.Id);
                        item.DiaChi = bhtn.BHTNDiaChi;
                        item.SoDienThoai = bhtn.BHTNSoDienThoai;
                        item.NgayHetHan = bhtn.BHTNNgayHetHan;
                        item.NgayHieuLuc = bhtn.BHTNNgayHetHan;
                        item.MaSoThe = bhtn.BHTNMaSoThe;
                    }
                }
            }

            await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yeuCauTiepNhan);

            await _tiepNhanBenhNhanService.UpdateBenhNhanForUpdateView(yeuCauTiepNhan.BenhNhanId ?? 0, yeuCauTiepNhan);

            await _yeuCauTiepNhanService.CapNhatThongTinHanhChinhVaoNoiTru(yeuCauTiepNhan, theBHYTCuTrongYCTN);
            return Ok(yeuCauTiepNhan);
        }

        [HttpGet("GetLyDoTiepNhanDefaultData")]
        public async Task<ActionResult> GetLyDoTiepNhanDefaultData()
        {
            var lyDoDefaultData = _tiepNhanBenhNhanService.GetLyDoTiepNhanDefault();
            return Ok(lyDoDefaultData);
        }

        [HttpGet("GetDefaultDataTNBN")]
        public async Task<ActionResult> GetDefaultDataTNBN()
        {
            var data = await _tiepNhanBenhNhanService.GetDefaultValueForTNBN();
            return Ok(data);
        }


        #region update feedback of client on 22/5/2020


        #endregion update feedback of client on 22/5/2020

        #region update tiep nhan benh nhan
        [HttpPost("GetYeuCauKhamBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetYeuCauKhamBenh(long id)
        {
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(id);

            if (entity == null) return null;

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            #region Cập nhật 19/12/2022: check có gói dv khuyến mãi
            //result.CoYeuCauGoiDichVu = (entity.BenhNhan.YeuCauGoiDichVus.Any() && entity.BenhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
            //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
            //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
            //                        || (entity.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() && entity.BenhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
            //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
            //                                                                           z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
            //;

            var kiemTraGoi = _yeuCauKhamBenhService.KiemTraCoGoiVaKhuyenMaiTheoNguoiBenhId(entity.BenhNhanId ?? 0);
            result.CoYeuCauGoiDichVu = kiemTraGoi.Item2;
            #endregion
            if (result.LyDoTiepNhanId != null)
            {
                result.LyDoTiepNhanText = await _tiepNhanBenhNhanService.GetLyDoTiepNhan((long)result.LyDoTiepNhanId);
            }

            if (result.NguoiGioiThieuId != null)
            {
                result.GioiThieu = result.NguoiGioiThieu.HoTen + " - CTV";
            }

            if (result.NoiGioiThieuId != null)
            {
                result.GioiThieu = result.NoiGioiThieu.Ten + " - Đơn vị";
            }

            foreach (var item in entity.TheVoucherYeuCauTiepNhans)
            {
                result.LstVoucherId.Add(item.TheVoucherId);
                result.LstMaVoucher.Add(item.TheVoucher.VoucherId + "|" + item.TheVoucher.Ma);
            }

            if (result.LstVoucherId.Any())
            {
                result.LoaiMienGiam = 2;
            }
            else
            {
                result.LoaiMienGiam = 1;
            }

            result = SetValueForGrid(result, entity, false, null, null, null, null);

            var soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(id);
            soDuTk -= _taiKhoanBenhNhanService.GetSoTienCanThanhToanNgoaiTru(entity);
            result.SoTienConLai = soDuTk;
            //result.SoTienConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(id);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            //update Voucher 10/21
            //if (entity.TheVoucherYeuCauTiepNhans.Any())
            //{
            //    result.LoaiVoucher = entity.TheVoucherYeuCauTiepNhans.FirstOrDefault()?.TheVoucher?.Voucher?.LoaiVoucher;
            //}

            if (entity.YeuCauTiepNhanLichSuKhamBHYT.Any())
            {
                var STT = 1;
                foreach (var item in entity.YeuCauTiepNhanLichSuKhamBHYT)
                {
                    var lichSu = new GridLichSuKCB
                    {
                        KetQuaDieuTriNumber = item.KetQuaDieuTri,
                        KetQuaDieuTri = item.KetQuaDieuTri.GetDescription(),

                        LyDoVaoVienNumber = item.LyDoVaoVien,
                        LyDoVaoVien = item.LyDoVaoVien.GetDescription(),

                        MaCoSoKCB = item.MaCSKCB,
                        MaTheBHYT = item.MaTheBHYT,

                        NgayRaDateTime = item.NgayRa,
                        NgayRaVien = item.NgayRa != null ? (item.NgayRa ?? DateTime.Now).ApplyFormatDateTime() : "",

                        NgayVaoDateTime = item.NgayVao,
                        NgayVaoVien = item.NgayVao != null ? (item.NgayVao ?? DateTime.Now).ApplyFormatDateTime() : "",

                        TinhTrangRaVienNumber = item.TinhTrangRaVien,
                        TinhTrangRaVien = item.TinhTrangRaVien.GetDescription(),

                        HoVaTen = entity.HoTen,

                        STT = STT,
                    };
                    lichSu.CoSoKCB = (await _benhVienService.GetBenhVienWithMaBenhVien(lichSu.MaCoSoKCB))?.Ten;
                    result.GridLichSuKCB.Add(lichSu);
                    STT++;
                }
            }

            if (entity.YeuCauTiepNhanLichSuKiemTraTheBHYTs.Any())
            {
                var STT = 1;
                foreach (var item in entity.YeuCauTiepNhanLichSuKiemTraTheBHYTs)
                {
                    var lichSu = new GridLichSuKiemTraTheBHYT
                    {
                        UserKiemTra = item.MaUserKiemTra,

                        thoiGianKTDateTime = item.ThoiGianKiemTra,
                        ThoiGianKiemTra = item.ThoiGianKiemTra != null ? (item.ThoiGianKiemTra ?? DateTime.Now).ApplyFormatDateTime() : "",

                        NoiDungThongBao = item.ThongBao,

                        STT = STT,
                    };

                    if (!string.IsNullOrEmpty(lichSu.UserKiemTra))
                    {
                        var maBenhVien = lichSu.UserKiemTra.Split("_")?[0];
                        lichSu.TenCSKCB = (await _benhVienService.GetBenhVienWithMaBenhVien(maBenhVien))?.Ten;
                    }
                    result.GridLichSuKiemTraTheBHYT.Add(lichSu);
                    STT++;
                }
            }

            ////get khoa kham
            //var phongKhamEntity = await _khoaPhongService.GetByIdAsync(entity.PhongKham?.KhoaPhongId ?? 0);
            //result.KhoaKhamModelText = phongKhamEntity != null ? phongKhamEntity.Ma + " - " + phongKhamEntity.Ten : "";
            //result.KhoaKhamId = entity.PhongKham?.KhoaPhongId ?? 0;
            //if (entity.BenhNhan.BHYTNoiDangKyId != null)
            //{
            //    var noiDangKyEntity = await _benhVienService.GetByIdAsync(entity.BenhNhan.BHYTNoiDangKyId ?? 0);
            //    result.BenhNhan.NoiDangKyBHYT =
            //        noiDangKyEntity != null ? noiDangKyEntity.Ten : "";
            //}

            #region Cập nhật 19/12/2022 xử lý load thông tin miễn giảm -> giảm bớt include đến MienGiamChiPhis
            ////set disable mien giam
            //result.DisableCoMienGiam = entity.MienGiamChiPhis.Any();
            //result.DisableDoiTuongUuDai = entity.MienGiamChiPhis.Any(p => p.LoaiMienGiam == LoaiMienGiam.UuDai);

            var kiemTraMienGiam = _tiepNhanBenhNhanService.KiemTraDisableMienGiamGetYCTN(id);
            result.DisableCoMienGiam = kiemTraMienGiam.Item1;
            result.DisableDoiTuongUuDai = kiemTraMienGiam.Item2;
            #endregion

            #region check cho tạo yctn mới trường hợp viện phí qua ngày , loại trường hợp Trạng thái đã hủy, là yeucautiepNhanLaNgoaiTru
            //if (entity != null)
            //{
            //    if (entity.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && entity.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
            //    {
            //        if (result.CoBHYT == false || result.CoBHYT == null)
            //        {
            //            if (result.ThoiDiemTiepNhan != null)
            //            {
            //                var thoiDiemTNBN = result.ThoiDiemTiepNhan.GetValueOrDefault();
            //                // TH: trong ngày
            //                if (DateTime.Now.Date == thoiDiemTNBN.Date)
            //                {
            //                    result.CheckYeuCauTiepNhanTaoMoi = false;
            //                }
            //                // trường hợp ngoài ngày
            //                if (DateTime.Now.Date > thoiDiemTNBN.Date)
            //                {
            //                    result.CheckYeuCauTiepNhanTaoMoi = true;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        result.CheckYeuCauTiepNhanTaoMoi = false;
            //    }
            if (entity.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
            {
                var kiemTra = await _tiepNhanBenhNhanService.KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(result.BenhNhanId ?? 0, true);
                result.MessageKhongChoPhepTaoMoiYCTN = kiemTra.ErrorMessage;
                result.CheckYeuCauTiepNhanTaoMoi = string.IsNullOrEmpty(result.MessageKhongChoPhepTaoMoiYCTN);
            }


            #endregion
            return Ok(result);
            //return null;

        }

        [HttpPost("SetMucHuongChoDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> SetMucHuongChoDichVu(GridUpdate model)
        {
            var entity = await _tiepNhanBenhNhanService.SetMucHuongChoDichVu(model.yeuCauTiepNhanId ?? 0, model.mucHuong ?? 0);

            entity = UpdateThongTinBHYTFromModel(entity, model.model, model);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, false, null, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("DuocHuongBHYT")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> DuocHuongBHYT(long dichVuId, int loai)
        {
            //1: dich vu kham benh, 2: dich vu ky thuat, 3: dich vu giuong
            var result = await _tiepNhanBenhNhanService.DuocHuongBHYT(dichVuId, loai);
            return Ok(result);
        }

        [HttpPost("CheckOrUncheckBHYTForDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> CheckOrUncheckBHYTForDichVu(GridUpdate model)
        {
            var entity = await _tiepNhanBenhNhanService.CheckOrUncheckBHYTForDichVu(model.isChecked ?? false, model.maDichVuId ?? 0, model.nhom, model.yeuCauTiepNhanId ?? 0, model.mucHuong ?? 0);

            entity = UpdateThongTinBHYTFromModel(entity, model.model, model);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("CapNhatDuocHuongBHYTDichVu")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> CapNhatDuocHuongBHYTDichVu(GridUpdate model)
        {
            var entity = await _tiepNhanBenhNhanService.CapNhatDuocHuongBHYTDichVuAsync(model.isChecked ?? false, model.maDichVuId ?? 0, model.nhom, model.yeuCauTiepNhanId ?? 0);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();
            result = SetValueForGrid(result, entity, true, null, null, null, null);
            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("AddOrDeleteListDichVuToServer")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> AddOrDeleteListDichVuToServer(GridUpdate model)
        {
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(model.yeuCauTiepNhanId ?? 0);

            entity = UpdateThongTinBHYTFromModel(entity, model.model, model);

            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("RemoveDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> RemoveDichVu(GridUpdate model)
        {
            var entity = await _tiepNhanBenhNhanService.RemoveDichVu(model.maDichVuId ?? 0, model.nhom, model.yeuCauTiepNhanId ?? 0, model.mucHuong, model.LyDoHuyDichVu);

            if (entity == null)
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.Remove"));
            }

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForDeleteDichVuAndUpdateAsync(entity);

            #region Cập nhật 21/12/2022
            entity = null;
            entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(model.yeuCauTiepNhanId ?? 0);
            #endregion

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, model.ListDichVuCheckTruocDos, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("RemoveDichVuCoChietKhau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> RemoveDichVuCoChietKhau(GridUpdate model)
        {
            var entity = await _tiepNhanBenhNhanService.RemoveDichVuCoChietKhau(model.maDichVuId ?? 0, model.nhom, model.yeuCauTiepNhanId ?? 0, model.mucHuong);

            if (entity == null)
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.Remove"));
            }

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForDeleteDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("NoiThucHienChange")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> NoiThucHienChange(GridUpdate model)
        {

            if (string.IsNullOrEmpty(model.NoiThucHienId))
            {
                return Ok(null);
            }

            var entity = await _tiepNhanBenhNhanService.NoiThucHienChange(model.maDichVuId ?? 0, model.nhom, model.yeuCauTiepNhanId ?? 0, model.mucHuong, model.NoiThucHienId);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            #region Cập nhật 21/12/2022
            entity = null;
            entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(model.yeuCauTiepNhanId ?? 0);
            #endregion

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("HuyBHYTChoChiDinhDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> HuyBHYTChoChiDinhDichVu(GridUpdate model)
        {

            var entity = await _tiepNhanBenhNhanService.HuyBHYTChiDinhDichVu(model.lstChiDinhDichVu, model.yeuCauTiepNhanId ?? 0, model.mucHuong);

            entity.HoTen = model.HoTen;
            entity.BenhNhan.HoTen = model.HoTen;
            if (model.NgaySinh != null)
            {
                entity.NgaySinh = model.NgaySinh.GetValueOrDefault().Day;
                entity.ThangSinh = model.NgaySinh.GetValueOrDefault().Month;
                entity.NamSinh = model.NgaySinh.GetValueOrDefault().Year;

                entity.BenhNhan.NgaySinh = entity.NgaySinh;
                entity.BenhNhan.ThangSinh = entity.ThangSinh;
                entity.BenhNhan.NamSinh = entity.NamSinh;
            }
            else
            {
                entity.NgaySinh = null;
                entity.ThangSinh = null;
                entity.NamSinh = null;

                entity.BenhNhan.NgaySinh = entity.NgaySinh;
                entity.BenhNhan.ThangSinh = entity.ThangSinh;
                entity.BenhNhan.NamSinh = entity.NamSinh;
            }
            //entity.CoBHYT = false;
            //entity.BenhNhan.CoBHTN = false;
            //entity.TuNhap = false;
            //clear data
            //entity = RemoveBHYT(entity);
            entity.LyDoVaoVien = EnumLyDoVaoVien.TraiTuyen;

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("LoaiGiaGridChange")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> LoaiGiaGridChange(LoaiGiaOrSoLuongChange model)
        {
            #region Cập nhật 21/12/2022
            //var entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(model.yeuCauTiepNhanId ?? 0);
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeForAdddichVu(model.yeuCauTiepNhanId ?? 0);
            #endregion

            switch (model.nhom)
            {
                case Constants.NhomDichVu.DichVuKhamBenh:
                    var yeuCau = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.Id == model.yeuCauId);
                    if (yeuCau == null) break;
                    yeuCau.NhomGiaDichVuKhamBenhBenhVienId = model.loaiGiaId ?? 1;
                    yeuCau.Gia = await _tiepNhanBenhNhanService.GetGiaDichVu(yeuCau.DichVuKhamBenhBenhVienId, model.loaiGiaId ?? 0, Constants.NhomDichVu.DichVuKhamBenh);
                    break;
                case Constants.NhomDichVu.DichVuKyThuat:
                    var yeuCauDVKT = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == model.yeuCauId);
                    if (yeuCauDVKT == null) break;
                    yeuCauDVKT.NhomGiaDichVuKyThuatBenhVienId = model.loaiGiaId ?? 1;
                    yeuCauDVKT.Gia = await _tiepNhanBenhNhanService.GetGiaDichVu(yeuCauDVKT.DichVuKyThuatBenhVienId, model.loaiGiaId ?? 0, Constants.NhomDichVu.DichVuKyThuat);
                    break;
                case Constants.NhomDichVu.DichVuGiuong:
                    var yeuCauDVG = entity.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.Id == model.yeuCauId);
                    if (yeuCauDVG == null) break;
                    yeuCauDVG.NhomGiaDichVuGiuongBenhVienId = model.loaiGiaId ?? 1;
                    yeuCauDVG.Gia = await _tiepNhanBenhNhanService.GetGiaDichVu(yeuCauDVG.DichVuGiuongBenhVienId, model.loaiGiaId ?? 0, Constants.NhomDichVu.DichVuGiuong);
                    break;
                case Constants.NhomDichVu.DuocPham:
                    break;
                default:
                    break;
            };

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            #region Cập nhật 21/12/2022
            entity = null;
            entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(model.yeuCauTiepNhanId ?? 0);
            #endregion

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("SoLuongGridChange")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> SoLuongGridChange(LoaiGiaOrSoLuongChange model)
        {
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(model.yeuCauTiepNhanId ?? 0);

            switch (model.nhom)
            {
                case Constants.NhomDichVu.DichVuKhamBenh:
                    break;
                case Constants.NhomDichVu.DichVuKyThuat:
                    var yeuCauDVKT = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == model.yeuCauId);
                    if (yeuCauDVKT == null) break;

                    // 31/05/2021: cập nhật bổ sung cập nhật số lượng dịch vụ trong gói, dịch vụ khuyến mãi
                    var soLuongMoi = model.soLuong ?? 1;
                    // kiểm tra nếu là dịch vụ chỉ đinh từ gói marketing
                    if (yeuCauDVKT.YeuCauGoiDichVuId != null)
                    {
                        var soLuongConLai = await _khamBenhService.GetSoLuongConLaiDichVuKyThuatTrongGoiMarketingBenhNhanAsync(yeuCauDVKT.YeuCauGoiDichVuId.Value, yeuCauDVKT.DichVuKyThuatBenhVienId);
                        var soLuongKhaDung = soLuongConLai + yeuCauDVKT.SoLan;
                        if (soLuongKhaDung < soLuongMoi)
                        {
                            throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), yeuCauDVKT.TenDichVu, soLuongKhaDung));
                        }
                    }

                    // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                    var laCapNhatSoLuongKhuyenMai = false;
                    long? yeuCauGoiDichVuKhuyenMaiId = null;
                    if (yeuCauDVKT.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                    {
                        if (yeuCauDVKT.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                        }
                        var yeuCauGoiId = yeuCauDVKT.MienGiamChiPhis
                            .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                            .YeuCauGoiDichVuId.Value;
                        var soLuongConLai = await _khamBenhService.GetSoLuongConLaiDichVuKyThuatKhuyenMaiTrongGoiMarketingBenhNhanAsync(yeuCauGoiId, yeuCauDVKT.DichVuKyThuatBenhVienId);
                        var soLuongKhaDung = soLuongConLai + yeuCauDVKT.SoLan;
                        if (soLuongKhaDung < soLuongMoi)
                        {
                            throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), yeuCauDVKT.TenDichVu, soLuongKhaDung));
                        }

                        //BVHD-3825
                        laCapNhatSoLuongKhuyenMai = true;
                        yeuCauGoiDichVuKhuyenMaiId = yeuCauGoiId;
                    }

                    yeuCauDVKT.SoLan = soLuongMoi;// model.soLuong ?? 1;

                    #region BVHD-3825
                    if (laCapNhatSoLuongKhuyenMai)
                    {
                        var thongTin = new ThongTinDichVuTrongGoi()
                        {
                            BenhNhanId = entity.BenhNhanId.Value,
                            DichVuId = yeuCauDVKT.DichVuKyThuatBenhVienId,
                            NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                            SoLuong = yeuCauDVKT.SoLan,
                            NhomGiaId = yeuCauDVKT.NhomGiaDichVuKyThuatBenhVienId,

                            //dùng cho trường hợp cập nhật số lượng hoặc loại giá
                            YeuCauDichVuCapNhatSoLuongLoaiGiaId = yeuCauDVKT.Id,
                            YeucauGoiDichVuKhuyenMaiId = yeuCauGoiDichVuKhuyenMaiId
                        };
                        await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(thongTin);

                        if (yeuCauDVKT.MienGiamChiPhis.Any(x => x.DaHuy != true && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) && x.YeuCauGoiDichVuId != null))
                        {
                            foreach (var mienGiam in yeuCauDVKT.MienGiamChiPhis.Where(a => a.DaHuy != true && (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) && a.YeuCauGoiDichVuId != null))
                            {
                                mienGiam.DaHuy = true;
                                mienGiam.WillDelete = true;

                                var giamSoTienMienGiam = yeuCauDVKT.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                if (giamSoTienMienGiam < 0)
                                {
                                    giamSoTienMienGiam = 0;
                                }
                                yeuCauDVKT.SoTienMienGiam = giamSoTienMienGiam;
                            }
                        }

                        // cập nhật số lượng thì ko thay đổi giá
                        //yeuCauDVKT.Gia = thongTin.DonGia;

                        var thanhTien = yeuCauDVKT.SoLan * yeuCauDVKT.Gia;
                        var thanhTienMienGiam = yeuCauDVKT.SoLan * thongTin.DonGiaKhuyenMai.Value;

                        var tongTienMienGiam = (thanhTien > thanhTienMienGiam) ? (thanhTien - thanhTienMienGiam) : 0;
                        yeuCauDVKT.SoTienMienGiam = yeuCauDVKT.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                        yeuCauDVKT.MienGiamChiPhis.Add(new MienGiamChiPhi()
                        {
                            YeuCauTiepNhanId = yeuCauDVKT.YeuCauTiepNhanId,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                            SoTien = yeuCauDVKT.SoTienMienGiam.Value,
                            YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                        });
                    }
                    #endregion
                    break;
                case Constants.NhomDichVu.DichVuGiuong:
                    //var yeuCauDVG = entity.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.Id == model.yeuCauId);
                    //if (yeuCauDVG == null) break;
                    //yeuCauDVG.so = await _tiepNhanBenhNhanService.GetGiaDichVu(yeuCauDVG.DichVuGiuongBenhVienId, model.loaiGiaId ?? 0, Constants.NhomDichVu.DichVuGiuong);
                    break;
                case Constants.NhomDichVu.DuocPham:
                    break;
                default:
                    break;
            };

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("HuyBHYTChoChiDinhDichVuForCoBHYTChange")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> HuyBHYTChoChiDinhDichVuForCoBHYTChange(GridUpdate model)
        {

            var entity = await _tiepNhanBenhNhanService.HuyBHYTChiDinhDichVu(model.lstChiDinhDichVu, model.yeuCauTiepNhanId ?? 0, model.mucHuong);

            entity.HoTen = model.HoTen;
            entity.BenhNhan.HoTen = model.HoTen;
            if (model.NgaySinh != null)
            {
                entity.NgaySinh = model.NgaySinh.GetValueOrDefault().Day;
                entity.ThangSinh = model.NgaySinh.GetValueOrDefault().Month;
                entity.NamSinh = model.NgaySinh.GetValueOrDefault().Year;

                entity.BenhNhan.NgaySinh = entity.NgaySinh;
                entity.BenhNhan.ThangSinh = entity.ThangSinh;
                entity.BenhNhan.NamSinh = entity.NamSinh;
            }
            else
            {
                entity.NgaySinh = null;
                entity.ThangSinh = null;
                entity.NamSinh = null;

                entity.BenhNhan.NgaySinh = entity.NgaySinh;
                entity.BenhNhan.ThangSinh = entity.ThangSinh;
                entity.BenhNhan.NamSinh = entity.NamSinh;
            }
            entity.CoBHYT = false;
            entity.BenhNhan.CoBHTN = false;
            entity.TuNhap = false;
            //clear data
            entity = RemoveBHYT(entity);

            //await _tiepNhanBenhNhanService.UpdateAsync(entity);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("IsHaveCongNo")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> IsHaveCongNo(long yeuCauTiepNhanId, long congTyId)
        {
            var result = await _tiepNhanBenhNhanService.IsHaveCongNo(yeuCauTiepNhanId, congTyId);
            return Ok(result);
        }
        #endregion update tiep nhan benh nhan

        #region private class
        private YeuCauTiepNhan UpdateThongTinBHYTFromModel(YeuCauTiepNhan entity, TiepNhanBenhNhanViewModel model, GridUpdate modelUpdate)
        {
            var dateTimeNow = DateTime.Now;
            //Không nhớ logic trước như thế nào mà có thêm LyDoVaoVien == đúng tuyến
            //if (model.BHYTMucHuong != null && model.LyDoVaoVien == EnumLyDoVaoVien.DungTuyen)
            if (model.CoBHYT == true
                && model.BHYTNgayHieuLuc != null && model.BHYTNgayHieuLuc <= dateTimeNow
                && model.BHYTNgayHetHan != null && model.BHYTNgayHetHan >= dateTimeNow
                && model.LyDoVaoVien == EnumLyDoVaoVien.DungTuyen)
            {
                //update tiep nhan benh nhan
                entity.CoBHYT = model.CoBHYT;
                entity.BHYTCoQuanBHXH = model.BHYTCoQuanBHXH;
                entity.BHYTDiaChi = model.BHYTDiaChi.ToUpperCaseTheFirstCharacter();
                entity.BHYTMaDKBD = model.BHYTMaDKBD;
                entity.BHYTMaSoThe = model.BHYTMaSoThe;
                entity.BHYTMucHuong = model.BHYTMucHuong;
                entity.BHYTNgayDu5Nam = model.BHYTNgayDu5Nam;
                entity.BHYTNgayHetHan = model.BHYTNgayHetHan;
                entity.BHYTNgayHieuLuc = model.BHYTNgayHieuLuc;
                entity.IsCheckedBHYT = model.IsCheckedBHYT;
                entity.LyDoVaoVien = model.LyDoVaoVien;

                entity.TuNhap = model.TuNhap;

                entity.HoTen = modelUpdate.HoTen.ToUpper();
                entity.BenhNhan.HoTen = modelUpdate.HoTen.ToUpper();
                if (model.NgaySinh != null)
                {
                    entity.NgaySinh = modelUpdate.NgaySinh.GetValueOrDefault().Day;
                    entity.ThangSinh = modelUpdate.NgaySinh.GetValueOrDefault().Month;
                    entity.NamSinh = modelUpdate.NgaySinh.GetValueOrDefault().Year;

                    entity.BenhNhan.NgaySinh = entity.NgaySinh;
                    entity.BenhNhan.ThangSinh = entity.ThangSinh;
                    entity.BenhNhan.NamSinh = entity.NamSinh;
                }
                else
                {
                    entity.NgaySinh = null;
                    entity.ThangSinh = null;
                    entity.NamSinh = null;

                    entity.BenhNhan.NgaySinh = entity.NgaySinh;
                    entity.BenhNhan.ThangSinh = entity.ThangSinh;
                    entity.BenhNhan.NamSinh = entity.NamSinh;
                }

                //update benh nhan
                if (entity.BenhNhan != null)
                {
                    entity.BenhNhan.CoBHYT = model.CoBHYT;
                    entity.BenhNhan.BHYTCoQuanBHXH = model.BHYTCoQuanBHXH;
                    entity.BenhNhan.BHYTDiaChi = model.BHYTDiaChi;
                    entity.BenhNhan.BHYTMaDKBD = model.BHYTMaDKBD;
                    entity.BenhNhan.BHYTMaSoThe = model.BHYTMaSoThe;
                    entity.BenhNhan.BHYTNgayDu5Nam = model.BHYTNgayDu5Nam;
                    entity.BenhNhan.BHYTNgayHetHan = model.BHYTNgayHetHan;
                    entity.BenhNhan.BHYTNgayHieuLuc = model.BHYTNgayHieuLuc;
                }

                //cheat cho lý do vào viện feedback 30/7/2020
                if (model.LyDoVaoVien == null)
                {
                    entity.LyDoVaoVien = EnumLyDoVaoVien.DungTuyen;
                }
            }
            else
            {
                entity.CoBHYT = false;
                entity.TuNhap = false;
                entity.BHYTCoQuanBHXH = null;
                entity.BHYTDiaChi = null;
                entity.BHYTMaDKBD = null;
                entity.BHYTMaSoThe = null;
                entity.BHYTMucHuong = null;
                entity.BHYTNgayDu5Nam = null;
                entity.BHYTNgayHetHan = null;
                entity.BHYTNgayHieuLuc = null;
                entity.IsCheckedBHYT = model.IsCheckedBHYT;
                entity.LyDoVaoVien = EnumLyDoVaoVien.DungTuyen;

                entity.TuNhap = model.TuNhap;
            }

            return entity;
        }
        private TiepNhanBenhNhanViewModel SetValueForGrid(TiepNhanBenhNhanViewModel result, YeuCauTiepNhan entity, bool? themDichVu, List<ListDichVuCheckTruocDo> ListDichVuCheckTruocDos, EnumNhomGoiDichVu? dichEnumNhomGoiDichVu, List<ThemDichVuKhamBenhVo> themDichVuTrongGois, List<ChiDinhDichVuGridVo> goiThuongDungCus, List<ChiDinhDichVuGridVo> dichVuChiDinhCus = null)
        {
            List<YeuCauGoiDichVu> yeuCauGois = null;
            var cauHinhNhomTiemChung = _cauhinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            #region Cập nhật 16/12/2022: get data tập trung
            #region nơi thực hiện dịch vụ
            var lstNoiThucHienId = new List<long>();
            var lstNoiThucHien = new List<LookupItemVo>();
            lstNoiThucHienId.AddRange(result.YeuCauKhamBenhs.Where(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(x => x.NoiThucHienId ?? x.NoiDangKyId ?? 0).ToList());
            lstNoiThucHienId.AddRange(result.YeuCauDichVuKyThuats.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(x => x.NoiThucHienId ?? 0).ToList());
            lstNoiThucHienId = lstNoiThucHienId.Where(x => x > 0).Distinct().ToList();
            if(lstNoiThucHienId.Any())
            {
                lstNoiThucHien = _tiepNhanBenhNhanService.GetListNoiThucHien(lstNoiThucHienId);
            }
            #endregion

            #region dv có giá BH
            var lstDichVuKhamId = result.YeuCauKhamBenhs
                                    .Where(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham 
                                                && p.DichVuKhamBenhBenhVienId != null)
                                    .Select(x => x.DichVuKhamBenhBenhVienId.Value).Distinct().ToList();

            var lstDichVuKyThuatId = result.YeuCauDichVuKyThuats
                                    .Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                                && p.DichVuKyThuatBenhVienId != null)
                                    .Select(x => x.DichVuKyThuatBenhVienId.Value).Distinct().ToList();

            var kiemTraCoGiaBH = _tiepNhanBenhNhanService.KiemTraDichVuCoGiaBaoHiem(lstDichVuKhamId, lstDichVuKyThuatId);
            var lstDichVuKhamCoGiaBHId = kiemTraCoGiaBH.Item1;
            var lstDichVuKyThuatCoGiaBHId = kiemTraCoGiaBH.Item2;
            #endregion

            #region gói dịch vụ, khuyến mãi
            var lstChuongTrinhId = _tiepNhanBenhNhanService.GetChuongTrinhGoiDichVuIdTheoBenhNhanId(entity.BenhNhanId ?? 0);
            var lstDichVuKhamIdTrongGoi = new List<long>();
            var lstDichVuKyThuatIdTrongGoi = new List<long>();

            var lstDichVuKhamIdKhuyenMai = new List<long>();
            var lstDichVuKyThuatIdKhuyenMai = new List<long>();

            var lstYeuCauKhamLaKhuyenMai = new List<LookupItemTemplate>();
            var lstYeuCauKyThuatLaKhuyenMai = new List<LookupItemTemplate>();

            if (lstChuongTrinhId.Any())
            {
                var chiTietGoi = _tiepNhanBenhNhanService.GetDichVuIdTrongGoi(lstChuongTrinhId);
                lstDichVuKhamIdTrongGoi = chiTietGoi.Item1;
                lstDichVuKyThuatIdTrongGoi = chiTietGoi.Item2;

                var chiTietGoiKhuyenMai = _tiepNhanBenhNhanService.GetDichVuKhuyenMaiIdTrongGoi(lstChuongTrinhId);
                lstDichVuKhamIdKhuyenMai = chiTietGoiKhuyenMai.Item1;
                lstDichVuKyThuatIdKhuyenMai = chiTietGoiKhuyenMai.Item2;
            }

            if (lstDichVuKhamIdKhuyenMai.Any())
            {
                var lstYeuCauId = result.YeuCauKhamBenhs
                                            .Where(p => p.Id != 0 
                                                        && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                        && p.DichVuKhamBenhBenhVienId != null
                                                        && lstDichVuKhamIdKhuyenMai.Contains(p.DichVuKhamBenhBenhVienId.Value))
                                            .Select(x => x.Id)
                                            .Distinct().ToList();
                lstYeuCauKhamLaKhuyenMai = _tiepNhanBenhNhanService.GetYeuCauDichVuLaKhuyenMai(lstYeuCauId);
            }

            if(lstDichVuKyThuatIdKhuyenMai.Any())
            {
                var lstYeuCauId = result.YeuCauDichVuKyThuats
                                            .Where(p => p.Id != 0
                                                        && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && p.DichVuKyThuatBenhVienId != null
                                                        && lstDichVuKyThuatIdKhuyenMai.Contains(p.DichVuKyThuatBenhVienId.Value))
                                            .Select(x => x.Id)
                                            .Distinct().ToList();
                lstYeuCauKyThuatLaKhuyenMai = _tiepNhanBenhNhanService.GetYeuCauDichVuLaKhuyenMai(lstYeuCauId, false);
            }

            #endregion


            #region dịch vụ đã hủy thanh toán
            var lstYeuCauKhamChuaThanhToanId = result.YeuCauKhamBenhs
                                    .Where(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                && p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                                && p.Id != 0)
                                    .Select(x => x.Id).Distinct().ToList();

            var lstYeuCauKyThuatChuaThanhToanId = result.YeuCauDichVuKyThuats
                                    .Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                && p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                                && p.Id != 0)
                                    .Select(x => x.Id).Distinct().ToList();
            var lstYeuCauKhamIdHuyThanhToan = new List<long>();
            var lstYeuCauKyThuatIdHuyThanhToan = new List<long>();

            var kiemTra = _tiepNhanBenhNhanService.GetDichVuIdHuyThanhToan(lstYeuCauKhamChuaThanhToanId, lstYeuCauKyThuatChuaThanhToanId).Result;
            lstYeuCauKhamIdHuyThanhToan = kiemTra.Item1;
            lstYeuCauKyThuatIdHuyThanhToan = kiemTra.Item2;

            #endregion
            #endregion

            if (result.YeuCauKhamBenhs.Where(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Any()) //BVHD-3284 || !string.IsNullOrEmpty(p.LyDoHuyDichVu)
            {
                foreach (var item in result.YeuCauKhamBenhs.Where(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)) //BVHD-3284 || !string.IsNullOrEmpty(p.LyDoHuyDichVu)
                {

                    var dichVu = new ChiDinhDichVuGridVo();

                    var noiThucHienId = string.Empty;
                    if (item.BacSiDangKyId != null && item.BacSiDangKyId != 0)
                    {
                        noiThucHienId = (item.NoiThucHienId != null && item.NoiThucHienId > 0 ? item.NoiThucHienId : item.NoiDangKyId) + "," + item.BacSiDangKyId;
                    }
                    else
                    {
                        noiThucHienId = (item.NoiThucHienId != null && item.NoiThucHienId > 0 ? item.NoiThucHienId : item.NoiDangKyId) + ",0";
                    }

                    dichVu.NoiThucHienModelText =
                        //_tiepNhanBenhNhanService.NoiThucHienModelText((item.NoiThucHienId != null && item.NoiThucHienId > 0 ? item.NoiThucHienId ?? 0 : item.NoiDangKyId ?? 0), item.BacSiDangKyId).Result;
                        lstNoiThucHien.FirstOrDefault(x => x.KeyId == (item.NoiThucHienId != null && item.NoiThucHienId > 0 ? item.NoiThucHienId ?? 0 : item.NoiDangKyId ?? 0))?.DisplayName;

                    dichVu.CoGiaBHYT =
                        //_tiepNhanBenhNhanService.DuocHuongBHYT(item.DichVuKhamBenhBenhVienId ?? 0, 1).Result;
                        lstDichVuKhamCoGiaBHId.Contains(item.DichVuKhamBenhBenhVienId ?? 0);

                    dichVu.Id = item.Id;
                    dichVu.Ma = item.MaDichVu;
                    dichVu.TenDichVu = item.TenDichVu;
                    dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem;
                    dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
                    //dichVu.TLMG = item.TiLeUuDai ?? 0;
                    dichVu.NoiThucHienId = noiThucHienId;
                    dichVu.Nhom = Constants.NhomDichVu.DichVuKhamBenh;
                    dichVu.NhomId = EnumNhomGoiDichVu.DichVuKhamBenh; // dịch vụ khám bệnh thì ==1
                    dichVu.SoLuong = 1;

                    ////Thach
                    //if (item.CongTyBaoHiemTuNhanCongNos != null)
                    //{
                    //    dichVu.CongNo = item.CongTyBaoHiemTuNhanCongNos.Select(o => o.SoTien).DefaultIfEmpty(0).Sum();
                    //    dichVu.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault();
                    //}
                    dichVu.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault();
                    dichVu.CongNo = item.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault();

                    dichVu.LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId ?? 0;
                    dichVu.LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien?.Ten;
                    //permission for change noithuchien
                    dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham || item.TrangThai == EnumTrangThaiYeuCauKhamBenh.HuyKham;

                    //them tinh trang
                    dichVu.TinhTrangDisplay = item.TrangThai.GetDescription();
                    dichVu.TrangThaiYeuCauKhamBenh = item.TrangThai;

                    //
                    dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
                    dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

                    #region Cập nhật 19/12/2022: xử lý get thông tin dv trong gói -> giảm bớt include
                    //// cập nhật gói
                    //dichVu.CoDichVuNayTrongGoi = entity.BenhNhan.YeuCauGoiDichVus.Any()
                    //                             && entity.BenhNhan.YeuCauGoiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId));
                    //dichVu.LaDichVuKhuyenMai = entity.YeuCauKhamBenhs.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    //                                                             && x.Id == item.Id).Any(x => x.MienGiamChiPhis.Any(a => a.YeuCauGoiDichVuId != null && a.DaHuy != true));
                    //dichVu.CoDichVuNayTrongGoiKhuyenMai = entity.BenhNhan?.YeuCauGoiDichVus?
                    //    .Where(x => x.BenhNhanId == entity.BenhNhanId || x.BenhNhanSoSinhId == entity.BenhNhanId)
                    //    .Select(x => x.ChuongTrinhGoiDichVu)
                    //    .Any(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(y => y.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId));
                    //var yeuCaGoiKhuyenMaiId = entity.YeuCauKhamBenhs.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    //                                                            && x.Id == item.Id)
                    //                                .SelectMany(x => x.MienGiamChiPhis).Where(a => a.YeuCauGoiDichVu != null && a.DaHuy != true)
                    //                                .Select(x => x.YeuCauGoiDichVuId).FirstOrDefault();

                    dichVu.CoDichVuNayTrongGoi = lstDichVuKhamIdTrongGoi.Contains(item.DichVuKhamBenhBenhVienId ?? 0);
                    dichVu.LaDichVuKhuyenMai = lstYeuCauKhamLaKhuyenMai.Any(a => a.KeyId == item.Id);
                    dichVu.CoDichVuNayTrongGoiKhuyenMai = lstDichVuKhamIdKhuyenMai.Contains(item.DichVuKhamBenhBenhVienId ?? 0);
                    var yeuCaGoiKhuyenMaiId = lstYeuCauKhamLaKhuyenMai.FirstOrDefault(a => a.KeyId == item.Id)?.NhomChaId;
                    #endregion

                    //goi co chiet khau
                    dichVu.IsGoiCoChietKhau = item.YeuCauGoiDichVuId != 0 && item.YeuCauGoiDichVuId != null;
                    dichVu.GoiCoChietKhauId = item.YeuCauGoiDichVuId ?? 0;
                    dichVu.TenGoiChietKhau = _tiepNhanBenhNhanService.GetTenChuongTrinhGoiDichVu(item.YeuCauGoiDichVuId ?? (yeuCaGoiKhuyenMaiId ?? 0), dichVu.LaDichVuKhuyenMai).Result;
                    //
                    dichVu.DonGia = dichVu.IsGoiCoChietKhau ? (double)(_tiepNhanBenhNhanService.GetDonGiaDichVuGoi(item.YeuCauGoiDichVuId ?? 0, item.DichVuKhamBenhBenhVienId ?? 0, true).Result)
                        : (double)(item.Gia ?? 0);
                    //
                    dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
                    dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();
                    dichVu.MaDichVuId = item.DichVuKhamBenhBenhVienId ?? 0;

                    dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
                    dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
                    dichVu.BnThanhToan = dichVu.ThanhTien - (double)dichVu.BHYTThanhToan;
                    dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();
                    //

                    //update 22/11/2022 không thấy sử dụng
                    //if (dichVu.IsGoiCoChietKhau && entity.BenhNhanId != 0)
                    //{
                    //    var slTon = _danhMucMarketingService
                    //        .GetSoLuongDichVuTrongGoiDichVu(entity.BenhNhanId ?? 0, item.YeuCauGoiDichVuId ?? 0
                    //        , item.DichVuKhamBenhBenhVienId ?? 0, Constants.NhomDichVu.DichVuKhamBenh, item.Id).Result;
                    //    dichVu.SoLuongConLai = slTon;
                    //}

                    // cập nhật hủy dịch vụ dã hủy thanh toán
                    //dichVu.IsDichVuHuyThanhToan = item.IsDichVuHuyThanhToan;
                    dichVu.IsDichVuHuyThanhToan = lstYeuCauKhamIdHuyThanhToan.Contains(item.Id);
                    dichVu.LyDoHuyDichVu = item.LyDoHuyDichVu;


                    dichVu.TenNhanVienChiDinh = item.TenNhanVienChiDinh;
                    dichVu.ThoiDiemChiDinh = item.ThoiDiemChiDinh;

                    dichVu.DichVuBenhVienId = item.DichVuKhamBenhBenhVienId;

                    result.YeuCauKhacGrid.Add(dichVu);
                }
            }

            if (result.YeuCauDichVuKyThuats.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Any()) //BVHD-3284 || !string.IsNullOrEmpty(p.LyDoHuyDichVu)
            {
                foreach (var item in result.YeuCauDichVuKyThuats.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)) //BVHD-3284  || !string.IsNullOrEmpty(p.LyDoHuyDichVu)
                {
                    var noiThucHienId = item.NoiThucHienId;

                    var dichVu = new ChiDinhDichVuGridVo();

                    dichVu.NoiThucHienModelText =
                        //_tiepNhanBenhNhanService.NoiThucHienKyThuatGiuongModelText(item.NoiThucHienId ?? 0).Result;
                        lstNoiThucHien.FirstOrDefault(x => x.KeyId == item.NoiThucHienId)?.DisplayName;

                    dichVu.CoGiaBHYT =
                        //_tiepNhanBenhNhanService.DuocHuongBHYT(item.DichVuKyThuatBenhVienId ?? 0, 2).Result;
                        lstDichVuKyThuatCoGiaBHId.Contains(item.DichVuKyThuatBenhVienId ?? 0);

                    dichVu.Id = item.Id;
                    dichVu.Ma = item.MaDichVu;
                    dichVu.TenDichVu = item.TenDichVu;
                    dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem;
                    dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
                    //dichVu.TLMG = item.TiLeUuDai ?? 0;
                    dichVu.NoiThucHienId = noiThucHienId + "";
                    dichVu.Nhom = Constants.NhomDichVu.DichVuKyThuat;

                    //nam
                    dichVu.NhomId = EnumNhomGoiDichVu.DichVuKyThuat; // dịch vụ kỹ thuật ==2
                    //
                    dichVu.SoLuong = item.SoLan ?? 1;

                    ////Thach
                    //if (item.CongTyBaoHiemTuNhanCongNos != null)
                    //{
                    //    dichVu.CongNo = item.CongTyBaoHiemTuNhanCongNos.Select(o => o.SoTien).DefaultIfEmpty(0).Sum();
                    //    dichVu.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault();
                    //}
                    dichVu.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault();
                    dichVu.CongNo = item.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault();

                    //
                    dichVu.LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVienId ?? 0;
                    dichVu.LoaiGia = item.NhomGiaDichVuKyThuatBenhVien?.Ten;

                    //permission for change noithuchien
                    dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || item.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;

                    //them tinh trang
                    dichVu.TinhTrangDisplay = item.TrangThai.GetDescription();
                    dichVu.TrangThaiYeuCauDichVuKyThuat = item.TrangThai;

                    //
                    dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
                    dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

                    #region Cập nhật 19/12/2022: xử lý get thông tin dv trong gói -> giảm bớt include
                    //// cập nhật gói
                    //dichVu.CoDichVuNayTrongGoi = entity.BenhNhan.YeuCauGoiDichVus.Any()
                    //                             && entity.BenhNhan.YeuCauGoiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId));
                    //dichVu.LaDichVuKhuyenMai = entity.YeuCauDichVuKyThuats.Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                    //                                                                  && x.Id == item.Id).Any(x => x.MienGiamChiPhis.Any(a => a.YeuCauGoiDichVuId != null && a.DaHuy != true));
                    //dichVu.CoDichVuNayTrongGoiKhuyenMai = entity.BenhNhan?.YeuCauGoiDichVus?
                    //    .Where(x => x.BenhNhanId == entity.BenhNhanId || x.BenhNhanSoSinhId == entity.BenhNhanId)
                    //    .Select(x => x.ChuongTrinhGoiDichVu)
                    //    .Any(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(y => y.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId));
                    //var yeuCaGoiKhuyenMaiId = entity.YeuCauDichVuKyThuats.Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                    //                                                            && x.Id == item.Id)
                    //    .SelectMany(x => x.MienGiamChiPhis).Where(a => a.YeuCauGoiDichVu != null && a.DaHuy != true)
                    //    .Select(x => x.YeuCauGoiDichVuId).FirstOrDefault();

                    dichVu.CoDichVuNayTrongGoi = lstDichVuKyThuatIdTrongGoi.Contains(item.DichVuKyThuatBenhVienId ?? 0);
                    dichVu.LaDichVuKhuyenMai = lstYeuCauKyThuatLaKhuyenMai.Any(a => a.KeyId == item.Id);
                    dichVu.CoDichVuNayTrongGoiKhuyenMai = lstDichVuKyThuatIdKhuyenMai.Contains(item.DichVuKyThuatBenhVienId ?? 0);
                    var yeuCaGoiKhuyenMaiId = lstYeuCauKyThuatLaKhuyenMai.FirstOrDefault(a => a.KeyId == item.Id)?.NhomChaId;
                    #endregion

                    //goi co chiet khau
                    dichVu.IsGoiCoChietKhau = item.YeuCauGoiDichVuId != 0 && item.YeuCauGoiDichVuId != null;
                    dichVu.GoiCoChietKhauId = item.YeuCauGoiDichVuId ?? 0;
                    dichVu.TenGoiChietKhau = _tiepNhanBenhNhanService.GetTenChuongTrinhGoiDichVu(item.YeuCauGoiDichVuId ?? (yeuCaGoiKhuyenMaiId ?? 0), dichVu.LaDichVuKhuyenMai).Result;


                    dichVu.DonGia = dichVu.IsGoiCoChietKhau ? (double)(_tiepNhanBenhNhanService
                        .GetDonGiaDichVuGoi(item.YeuCauGoiDichVuId ?? 0, item.DichVuKyThuatBenhVienId ?? 0, false, true).Result) : (double)(item.Gia ?? 0);

                    dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
                    dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();
                    dichVu.MaDichVuId = item.DichVuKyThuatBenhVienId ?? 0;

                    dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
                    dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
                    dichVu.BnThanhToan = dichVu.ThanhTien - (double)dichVu.BHYTThanhToan;
                    dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();

                    //update 22/11/2022 không thấy sử dụng
                    //if (dichVu.IsGoiCoChietKhau && entity.BenhNhanId != 0)
                    //{
                    //    var slTon = _danhMucMarketingService
                    //        .GetSoLuongDichVuTrongGoiDichVu(entity.BenhNhanId ?? 0, item.YeuCauGoiDichVuId ?? 0, item.DichVuKyThuatBenhVienId ?? 0
                    //        , Constants.NhomDichVu.DichVuKyThuat, item.Id).Result;
                    //    dichVu.SoLuongConLai = slTon;
                    //}

                    //dichVu.TenNhanVienChiDinh = item.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                    //                            || item.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                    //                            || item.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang ? item.TenNhanVienChiDinh : string.Empty;

                    dichVu.TenNhanVienChiDinh = item.TenNhanVienChiDinh;

                    // cập nhật hủy dịch vụ dã hủy thanh toán
                    //dichVu.IsDichVuHuyThanhToan = item.IsDichVuHuyThanhToan;
                    dichVu.IsDichVuHuyThanhToan = lstYeuCauKyThuatIdHuyThanhToan.Contains(item.Id);
                    dichVu.LyDoHuyDichVu = item.LyDoHuyDichVu;

                    dichVu.ThoiDiemChiDinh = item.ThoiDiemChiDinh;


                    // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
                    dichVu.LoaiDichVuKyThuat = item.LoaiDichVuKyThuat;
                    dichVu.LyDoHuyTrangThaiDaThucHien = item.LyDoHuyTrangThaiDaThucHien;
                    dichVu.ThoiDiemThucHien = item.ThoiDiemThucHien;

                    dichVu.DichVuBenhVienId = item.DichVuKyThuatBenhVienId;  // dịch vụ kỹ thuật bệnh viện

                    dichVu.LaDichVuVacxin = nhomTiemChungId != null && item.NhomDichVuBenhVienId == nhomTiemChungId && item.LaDichVuVacxin == true;

                    //Cập nhật được hưởng BH
                    dichVu.LaChiDinhTuKhamBenh = item.YeuCauKhamBenhId.GetValueOrDefault() != 0;

                    result.YeuCauKhacGrid.Add(dichVu);
                }
            }

            if (entity.YeuCauDichVuGiuongBenhViens.Where(p => p.TrangThai != EnumTrangThaiGiuongBenh.DaHuy).Any())
            {
                foreach (var item in entity.YeuCauDichVuGiuongBenhViens
                    .Where(p => p.TrangThai != EnumTrangThaiGiuongBenh.DaHuy))
                {
                    var noiThucHienId = item.NoiThucHienId;

                    var dichVu = new ChiDinhDichVuGridVo();

                    dichVu.NoiThucHienModelText =
                        _tiepNhanBenhNhanService.NoiThucHienKyThuatGiuongModelText(item.NoiThucHienId ?? 0).Result;

                    dichVu.CoGiaBHYT = _tiepNhanBenhNhanService.DuocHuongBHYT(item.DichVuGiuongBenhVienId, 3).Result;

                    dichVu.Id = item.Id;
                    dichVu.Ma = item.Ma;
                    dichVu.TenDichVu = item.Ten;
                    dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem;
                    //dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
                    //dichVu.TLMG = item.TiLeUuDai ?? 0;
                    dichVu.NoiThucHienId = noiThucHienId + "";
                    dichVu.Nhom = Constants.NhomDichVu.DichVuGiuong;
                    dichVu.SoLuong = 1;

                    ////Thach
                    //if (item.CongTyBaoHiemTuNhanCongNos != null)
                    //{
                    //    dichVu.CongNo = item.CongTyBaoHiemTuNhanCongNos.Select(o => o.SoTien).DefaultIfEmpty(0).Sum();
                    //    dichVu.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault();
                    //}
                    dichVu.CongNo = item.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault();
                    dichVu.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault();


                    //
                    dichVu.LoaiGiaId = item.NhomGiaDichVuGiuongBenhVienId ?? 0;
                    dichVu.LoaiGia = item.NhomGiaDichVuGiuongBenhVien?.Ten;

                    //permission for change noithuchien
                    dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumTrangThaiGiuongBenh.DaThucHien || item.TrangThai == EnumTrangThaiGiuongBenh.DaHuy;

                    //them tinh trang
                    dichVu.TinhTrangDisplay = item.TrangThai.GetDescription();
                    dichVu.TrangThaiGiuongBenh = item.TrangThai;
                    //
                    dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
                    dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

                    // cập nhật gói
                    dichVu.CoDichVuNayTrongGoi = entity.BenhNhan.YeuCauGoiDichVus.Any()
                                                 && entity.BenhNhan.YeuCauGoiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(b => b.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId));
                    dichVu.LaDichVuKhuyenMai = entity.YeuCauDichVuGiuongBenhViens.Where(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                                                                             && x.Id == item.Id).Any(x => x.MienGiamChiPhis.Any(a => a.YeuCauGoiDichVuId != null && a.DaHuy != true));
                    dichVu.CoDichVuNayTrongGoiKhuyenMai = entity.BenhNhan?.YeuCauGoiDichVus?
                        .Where(x => x.BenhNhanId == entity.BenhNhanId || x.BenhNhanSoSinhId == entity.BenhNhanId)
                        .Select(x => x.ChuongTrinhGoiDichVu)
                        .Any(x => x.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any(y => y.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId));
                    var yeuCaGoiKhuyenMaiId = entity.YeuCauDichVuGiuongBenhViens.Where(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                                                                            && x.Id == item.Id)
                        .SelectMany(x => x.MienGiamChiPhis).Where(a => a.YeuCauGoiDichVu != null && a.DaHuy != true)
                        .Select(x => x.YeuCauGoiDichVuId).FirstOrDefault();

                    //goi co chiet khau
                    dichVu.IsGoiCoChietKhau = item.YeuCauGoiDichVuId != 0 && item.YeuCauGoiDichVuId != null;
                    dichVu.GoiCoChietKhauId = item.YeuCauGoiDichVuId ?? 0;
                    dichVu.TenGoiChietKhau = _tiepNhanBenhNhanService.GetTenChuongTrinhGoiDichVu(item.YeuCauGoiDichVuId ?? (yeuCaGoiKhuyenMaiId ?? 0), dichVu.LaDichVuKhuyenMai).Result;
                    //

                    dichVu.DonGia = dichVu.IsGoiCoChietKhau ? (double)(_tiepNhanBenhNhanService.GetDonGiaDichVuGoi(item.YeuCauGoiDichVuId ?? 0, item.DichVuGiuongBenhVienId, false, false, true).Result) : (double)(item.Gia ?? 0);

                    dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
                    dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();
                    dichVu.MaDichVuId = item.DichVuGiuongBenhVienId;

                    dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
                    dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
                    dichVu.BnThanhToan = dichVu.ThanhTien - (double)dichVu.BHYTThanhToan;
                    dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();

                    //update 22/11/2022 không thấy sử dụng
                    //if (dichVu.IsGoiCoChietKhau && entity.BenhNhanId != 0)
                    //{
                    //    var slTon = _danhMucMarketingService
                    //        .GetSoLuongDichVuTrongGoiDichVu(entity.BenhNhanId ?? 0, item.YeuCauGoiDichVuId ?? 0
                    //        , item.DichVuGiuongBenhVienId, Constants.NhomDichVu.DichVuGiuong, item.Id).Result;
                    //    dichVu.SoLuongConLai = slTon;
                    //}
                    dichVu.DichVuBenhVienId = item.DichVuGiuongBenhVienId;  // dịch vụ giường

                    result.YeuCauKhacGrid.Add(dichVu);
                }
            }

            //if (result.YeuCauVatTuBenhViens.Where(p => p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy && p.YeuCauGoiDichVuId == null).Any())
            //{
            //    foreach (var item in result.YeuCauVatTuBenhViens.Where(p => p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy && p.YeuCauGoiDichVuId == null))
            //    {
            //        var dichVu = new ChiDinhDichVuGridVo();

            //        //Tai thoi diem nay chua co gia bhyt cho vat tu
            //        dichVu.CoGiaBHYT = _tiepNhanBenhNhanService.DuocHuongBHYT(item.VatTuBenhVienId ?? 0, 4).Result;

            //        dichVu.Id = item.Id;
            //        dichVu.Ma = item.Ma;
            //        dichVu.TenDichVu = item.Ten;
            //        dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem ?? false;
            //        dichVu.DonGia = (double)(item.Gia ?? 0);
            //        dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
            //        //dichVu.TLMG = item.TiLeUuDai ?? 0;
            //        dichVu.NoiThucHienId = "";
            //        dichVu.Nhom = Constants.NhomDichVu.VatTuTieuHao;
            //        dichVu.SoLuong = Convert.ToInt32(item.SoLuong ?? 0);

            //        dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
            //        dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();
            //        dichVu.MaDichVuId = item.VatTuBenhVienId ?? 0;

            //        ////Thach
            //        if (item.CongTyBaoHiemTuNhanCongNos != null)
            //        {
            //            dichVu.CongNo = item.CongTyBaoHiemTuNhanCongNos.Select(o => o.SoTien).DefaultIfEmpty(0).Sum();
            //            dichVu.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault();
            //        }

            //        dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
            //        dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
            //        dichVu.BnThanhToan = dichVu.ThanhTien - (double)dichVu.BHYTThanhToan;
            //        dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();

            //        //permission for change noithuchien
            //        dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien || item.TrangThai == EnumYeuCauVatTuBenhVien.DaHuy;

            //        //them tinh trang
            //        dichVu.TinhTrangDisplay = item.TrangThai != null ? item.TrangThai.GetDescription() : "";

            //        //
            //        dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
            //        dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

            //        result.YeuCauKhacGrid.Add(dichVu);
            //    }
            //}

            //if (result.YeuCauDuocPhamBenhViens.Where(p => p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && p.YeuCauGoiDichVuId == null).Any())
            //{
            //    foreach (var item in result.YeuCauDuocPhamBenhViens.Where(p => p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && p.YeuCauGoiDichVuId == null))
            //    {
            //        var dichVu = new ChiDinhDichVuGridVo();

            //        dichVu.CoGiaBHYT = _tiepNhanBenhNhanService.DuocHuongBHYT(item.DuocPhamBenhVienId ?? 0, 5).Result;

            //        dichVu.Id = item.Id;
            //        dichVu.Ma = item.MaHoatChat;
            //        dichVu.TenDichVu = item.Ten;
            //        dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem ?? false;
            //        dichVu.DonGia = (double)(item.Gia ?? 0);
            //        dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
            //        //dichVu.TLMG = item.TiLeUuDai ?? 0;
            //        dichVu.NoiThucHienId = "";
            //        dichVu.Nhom = Constants.NhomDichVu.DuocPham;
            //        dichVu.SoLuong = Convert.ToInt32(item.SoLuong ?? 0);

            //        ////Thach
            //        if (item.CongTyBaoHiemTuNhanCongNos != null)
            //        {
            //            dichVu.CongNo = item.CongTyBaoHiemTuNhanCongNos.Select(o => o.SoTien).DefaultIfEmpty(0).Sum();
            //            dichVu.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault();
            //        }

            //        dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
            //        dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();
            //        dichVu.MaDichVuId = item.DuocPhamBenhVienId ?? 0;

            //        dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
            //        dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
            //        dichVu.BnThanhToan = dichVu.ThanhTien - (double)dichVu.BHYTThanhToan;
            //        dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();

            //        //permission for change noithuchien
            //        dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien || item.TrangThai == EnumYeuCauDuocPhamBenhVien.DaHuy;

            //        //them tinh trang
            //        dichVu.TinhTrangDisplay = item.TrangThai != null ? item.TrangThai.GetDescription() : "";

            //        //
            //        dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
            //        dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

            //        result.YeuCauKhacGrid.Add(dichVu);
            //    }
            //}

            //TODO: need update goi dv
            //if (entity.YeuCauGoiDichVus.Any())
            //{
            //    foreach (var goiDichVu in entity.YeuCauGoiDichVus)
            //    {
            //        var lstGridGoi = new List<ChiDinhDichVuGridVo>();
            //        double tongThanhTien = 0;

            //        if (goiDichVu.YeuCauKhamBenhs.Where(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).Any())
            //        {
            //            foreach (var item in goiDichVu.YeuCauKhamBenhs.Where(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham))
            //            {
            //                var noiThucHienId = string.Empty;
            //                if (item.BacSiDangKyId != null && item.BacSiDangKyId != 0)
            //                {
            //                    noiThucHienId = item.NoiDangKyId + "," + item.BacSiDangKyId;
            //                }
            //                else
            //                {
            //                    noiThucHienId = item.NoiDangKyId + ",0";
            //                }
            //                var dichVu = new ChiDinhDichVuGridVo();

            //                dichVu.NoiThucHienModelText =
            //                    _tiepNhanBenhNhanService.NoiThucHienModelText(item.NoiDangKyId ?? 0, item.BacSiDangKyId).Result;

            //                dichVu.TenGoiChietKhau = goiDichVu.Ten;
            //                dichVu.TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu;
            //                dichVu.Id = item.Id;
            //                dichVu.Ma = item.MaDichVu;
            //                dichVu.TenDichVu = item.TenDichVu;
            //                dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem;
            //                dichVu.DonGia = (double)item.Gia;
            //                //dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
            //                //dichVu.TLMG = item.TiLeUuDai ?? 0;
            //                dichVu.NoiThucHienId = noiThucHienId;
            //                dichVu.Nhom = Constants.NhomDichVu.DichVuKhamBenh;
            //                dichVu.SoLuong = 1;
            //                dichVu.IsGoiCoChietKhau = true;
            //                dichVu.GoiCoChietKhauId = goiDichVu.GoiDichVuId;

            //                //
            //                dichVu.MaDichVuId = item.DichVuKhamBenhBenhVienId;

            //                //
            //                dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
            //                dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();

            //                dichVu.BHYTThanhToan = 0;
            //                dichVu.BHYTThanhToanChuaBaoGomMucHuong = 0;

            //                dichVu.SoTienMG = (dichVu.ThanhTien);
            //                dichVu.SoTienMGDisplay = dichVu.SoTienMG.ApplyNumber();
            //                dichVu.BnThanhToan = dichVu.ThanhTien - dichVu.SoTienMG;
            //                dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();

            //                dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
            //                dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
            //                //
            //                dichVu.LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId;
            //                dichVu.LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien?.Ten;
            //                //
            //                //permission for change noithuchien
            //                dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham || item.TrangThai == EnumTrangThaiYeuCauKhamBenh.HuyKham;

            //                //them tinh trang
            //                dichVu.TinhTrangDisplay = item.TrangThai != null ? item.TrangThai.GetDescription() : "";

            //                //
            //                dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
            //                dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

            //                lstGridGoi.Add(dichVu);

            //                tongThanhTien = tongThanhTien + dichVu.ThanhTien;
            //            }
            //        }
            //        if (goiDichVu.YeuCauDichVuKyThuats.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Any())
            //        {
            //            foreach (var item in goiDichVu.YeuCauDichVuKyThuats.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            //            {
            //                var noiThucHienId = item.NoiThucHienId;

            //                var dichVu = new ChiDinhDichVuGridVo();

            //                dichVu.NoiThucHienModelText =
            //                    _tiepNhanBenhNhanService.NoiThucHienKyThuatGiuongModelText(item.NoiThucHienId ?? 0).Result;

            //                dichVu.TenGoiChietKhau = goiDichVu.Ten;
            //                dichVu.TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu;
            //                dichVu.Id = item.Id;
            //                dichVu.Ma = item.MaDichVu;
            //                dichVu.TenDichVu = item.TenDichVu;
            //                dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem;
            //                dichVu.DonGia = (double)(item.Gia);
            //                //dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
            //                //dichVu.TLMG = item.TiLeUuDai ?? 0;
            //                dichVu.NoiThucHienId = noiThucHienId + "";
            //                dichVu.Nhom = Constants.NhomDichVu.DichVuKyThuat;
            //                dichVu.SoLuong = item.SoLan;
            //                dichVu.IsGoiCoChietKhau = true;
            //                dichVu.GoiCoChietKhauId = goiDichVu.GoiDichVuId;

            //                //
            //                dichVu.MaDichVuId = item.DichVuKyThuatBenhVienId;

            //                //
            //                dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
            //                dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();

            //                dichVu.BHYTThanhToan = 0;
            //                dichVu.BHYTThanhToanChuaBaoGomMucHuong = 0;

            //                dichVu.SoTienMG = (dichVu.ThanhTien);
            //                dichVu.SoTienMGDisplay = dichVu.SoTienMG.ApplyNumber();
            //                dichVu.BnThanhToan = dichVu.ThanhTien - dichVu.SoTienMG;
            //                dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();

            //                dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
            //                dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
            //                //
            //                dichVu.LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVienId;
            //                dichVu.LoaiGia = item.NhomGiaDichVuKyThuatBenhVien?.Ten;
            //                //
            //                //permission for change noithuchien
            //                dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || item.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;

            //                //them tinh trang
            //                dichVu.TinhTrangDisplay = item.TrangThai != null ? item.TrangThai.GetDescription() : "";

            //                //
            //                dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
            //                dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

            //                lstGridGoi.Add(dichVu);

            //                tongThanhTien = tongThanhTien + dichVu.ThanhTien;
            //            }
            //        }
            //        if (goiDichVu.YeuCauVatTuBenhViens.Where(p => p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Any())
            //        {
            //            foreach (var item in goiDichVu.YeuCauVatTuBenhViens.Where(p => p.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy))
            //            {
            //                var dichVu = new ChiDinhDichVuGridVo();
            //                dichVu.TenGoiChietKhau = goiDichVu.Ten;
            //                dichVu.TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu;
            //                dichVu.Id = item.Id;
            //                dichVu.Ma = item.Ma;
            //                dichVu.TenDichVu = item.Ten;
            //                dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem;
            //                //TODO update entity kho on 9/9/2020
            //                //dichVu.DonGia = (double)(item.Gia ?? 0);
            //                //dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
            //                //dichVu.TLMG = item.TiLeUuDai ?? 0;
            //                dichVu.NoiThucHienId = "";
            //                dichVu.Nhom = Constants.NhomDichVu.VatTuTieuHao;
            //                dichVu.SoLuong = Convert.ToInt32(item.SoLuong);
            //                dichVu.IsGoiCoChietKhau = true;
            //                dichVu.GoiCoChietKhauId = goiDichVu.GoiDichVuId;

            //                //
            //                dichVu.MaDichVuId = item.VatTuBenhVienId;

            //                //
            //                dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
            //                dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();

            //                dichVu.BHYTThanhToan = 0;
            //                dichVu.BHYTThanhToanChuaBaoGomMucHuong = 0;

            //                dichVu.SoTienMG = (dichVu.ThanhTien);
            //                dichVu.SoTienMGDisplay = dichVu.SoTienMG.ApplyNumber();
            //                dichVu.BnThanhToan = dichVu.ThanhTien - dichVu.SoTienMG;
            //                dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();
            //                //

            //                dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
            //                dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();

            //                //permission for change noithuchien
            //                dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien || item.TrangThai == EnumYeuCauVatTuBenhVien.DaHuy;

            //                //them tinh trang
            //                dichVu.TinhTrangDisplay = item.TrangThai != null ? item.TrangThai.GetDescription() : "";
            //                //
            //                dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
            //                dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

            //                lstGridGoi.Add(dichVu);

            //                tongThanhTien = tongThanhTien + dichVu.ThanhTien;
            //            }
            //        }
            //        if (goiDichVu.YeuCauDuocPhamBenhViens.Where(p => p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Any())
            //        {
            //            foreach (var item in goiDichVu.YeuCauDuocPhamBenhViens.Where(p => p.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy))
            //            {
            //                var dichVu = new ChiDinhDichVuGridVo();
            //                dichVu.TenGoiChietKhau = goiDichVu.Ten;
            //                dichVu.TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu;
            //                dichVu.Id = item.Id;
            //                dichVu.Ma = item.MaHoatChat;
            //                dichVu.TenDichVu = item.Ten;
            //                dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem;
            //                //TODO update entity kho on 9/9/2020
            //                //dichVu.DonGia = (double)(item.Gia ?? 0);
            //                //dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
            //                //dichVu.TLMG = item.TiLeUuDai ?? 0;
            //                dichVu.NoiThucHienId = "";
            //                dichVu.Nhom = Constants.NhomDichVu.DuocPham;
            //                dichVu.SoLuong = Convert.ToInt32(item.SoLuong);
            //                dichVu.IsGoiCoChietKhau = true;
            //                dichVu.GoiCoChietKhauId = goiDichVu.GoiDichVuId;

            //                //
            //                dichVu.MaDichVuId = item.DuocPhamBenhVienId;

            //                //
            //                dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
            //                dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();

            //                dichVu.BHYTThanhToan = 0;
            //                dichVu.BHYTThanhToanChuaBaoGomMucHuong = 0;

            //                dichVu.SoTienMG = (dichVu.ThanhTien);
            //                dichVu.SoTienMGDisplay = dichVu.SoTienMG.ApplyNumber();
            //                dichVu.BnThanhToan = dichVu.ThanhTien - dichVu.SoTienMG;
            //                dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();

            //                dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
            //                dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
            //                //

            //                //permission for change noithuchien
            //                dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumYeuCauDuocPhamBenhVien.DaThucHien || item.TrangThai == EnumYeuCauDuocPhamBenhVien.DaHuy;

            //                //them tinh trang
            //                dichVu.TinhTrangDisplay = item.TrangThai != null ? item.TrangThai.GetDescription() : "";
            //                //
            //                dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
            //                dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

            //                lstGridGoi.Add(dichVu);

            //                tongThanhTien = tongThanhTien + dichVu.ThanhTien;
            //            }
            //        }

            //        if (goiDichVu.YeuCauDichVuGiuongBenhViens.Where(p => p.TrangThai != EnumTrangThaiGiuongBenh.DaHuy).Any())
            //        {
            //            foreach (var item in goiDichVu.YeuCauDichVuGiuongBenhViens.Where(p => p.TrangThai != EnumTrangThaiGiuongBenh.DaHuy))
            //            {
            //                var dichVu = new ChiDinhDichVuGridVo();

            //                dichVu.NoiThucHienModelText =
            //                    _tiepNhanBenhNhanService.NoiThucHienKyThuatGiuongModelText(item.NoiThucHienId ?? 0).Result;

            //                dichVu.TenGoiChietKhau = goiDichVu.Ten;
            //                dichVu.TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu;
            //                dichVu.Id = item.Id;
            //                dichVu.Ma = item.Ma;
            //                dichVu.TenDichVu = item.Ten;
            //                dichVu.DuocHuongBHYT = item.DuocHuongBaoHiem;
            //                dichVu.DonGia = (double)(item.Gia ?? 0);
            //                //dichVu.BHYTThanhToan = item.GiaBaoHiemThanhToan ?? 0;
            //                //dichVu.TLMG = item.TiLeUuDai ?? 0;
            //                dichVu.NoiThucHienId = item.NoiThucHienId + "";
            //                dichVu.Nhom = Constants.NhomDichVu.DichVuGiuong;
            //                dichVu.SoLuong = 1;
            //                dichVu.IsGoiCoChietKhau = true;
            //                dichVu.GoiCoChietKhauId = goiDichVu.GoiDichVuId;

            //                //
            //                dichVu.MaDichVuId = item.DichVuGiuongBenhVienId;

            //                //
            //                dichVu.ThanhTien = dichVu.DonGia * dichVu.SoLuong ?? 0;
            //                dichVu.ThanhTienDisplay = dichVu.ThanhTien.ApplyNumber();

            //                dichVu.BHYTThanhToan = 0;
            //                dichVu.BHYTThanhToanChuaBaoGomMucHuong = 0;

            //                dichVu.SoTienMG = (dichVu.ThanhTien);
            //                dichVu.SoTienMGDisplay = dichVu.SoTienMG.ApplyNumber();
            //                dichVu.BnThanhToan = dichVu.ThanhTien - dichVu.SoTienMG;
            //                dichVu.BnThanhToanDisplay = dichVu.BnThanhToan.ApplyNumber();

            //                dichVu.DonGiaDisplay = dichVu.DonGia.ApplyNumber();
            //                dichVu.BHYTThanhToanDisplay = dichVu.BHYTThanhToan.ApplyVietnameseFloatNumber();
            //                //
            //                dichVu.LoaiGiaId = item.NhomGiaDichVuGiuongBenhVienId ?? 0;
            //                dichVu.LoaiGia = item.NhomGiaDichVuGiuongBenhVien?.Ten;
            //                //

            //                //permission for change noithuchien
            //                dichVu.IsDontHavePermissionChangeNoiThucHien = item.TrangThai == EnumTrangThaiGiuongBenh.DaThucHien || item.TrangThai == EnumTrangThaiGiuongBenh.DaHuy;

            //                //them tinh trang
            //                dichVu.TinhTrangDisplay = item.TrangThai != null ? item.TrangThai.GetDescription() : "";
            //                //
            //                dichVu.GiaBHYT = item.DonGiaBaoHiem != null ? (double)item.DonGiaBaoHiem : 0;
            //                dichVu.TiLeBaoHiemThanhToan = item.MucHuongBaoHiem;

            //                lstGridGoi.Add(dichVu);

            //                tongThanhTien = tongThanhTien + dichVu.ThanhTien;
            //            }
            //        }

            //        foreach (var item in lstGridGoi)
            //        {
            //            item.TyLeChietKhau = Math.Round((tongThanhTien - item.TongChiPhiGoi) / tongThanhTien * 100);
            //            item.TyLeChietKhauDisplay = item.TyLeChietKhau + "%";
            //            item.DuocGiamTrongGoi = item.DonGia - (((tongThanhTien - item.TongChiPhiGoi) / tongThanhTien * 100) * item.DonGia / 100);
            //            item.ThanhTienTrongGoi = item.DuocGiamTrongGoi * item.SoLuong ?? 0;
            //        }

            //        result.YeuCauKhacGrid.AddRange(lstGridGoi);
            //    }
            //}
            // add từng dịch vụ 
            if (dichEnumNhomGoiDichVu != null)
            {
                if (result.YeuCauKhacGrid.Any() && themDichVu == true)
                {
                    result.YeuCauKhacGrid.Where(s => s.NhomId == dichEnumNhomGoiDichVu).LastOrDefault().isCheckRowItem = true;
                }
            }

            // dịch vụ thêm được check trước đó
            if (ListDichVuCheckTruocDos != null)
            {
                if (ListDichVuCheckTruocDos.Any())
                {
                    var list = new List<ChiDinhDichVuGridVo>();
                    foreach (var itemCheck in ListDichVuCheckTruocDos)
                    {
                        foreach (var item in result.YeuCauKhacGrid)
                        {
                            if (itemCheck.Id == item.Id && itemCheck.NhomId == (int)item.NhomId)
                            {
                                item.isCheckRowItem = true;
                            }
                            list.Add(item);
                        }
                    }
                    result.YeuCauKhacGrid = new List<ChiDinhDichVuGridVo>();
                    result.YeuCauKhacGrid = list.Distinct().ToList();
                }

            }
            // dịch vụ gói marketing
            if (themDichVuTrongGois != null)
            {
                if (themDichVuTrongGois.Any())
                {
                    foreach (var item in themDichVuTrongGois)
                    {
                        // DichVuKhamBenhBenhVienId => MaDichVuId
                        //ChuongTrinhGoiDichVuId => id

                        var dichVuVuaThem = result.YeuCauKhacGrid.Where(s => s.DichVuBenhVienId == item.MaDichVuId).ToList();
                        foreach (var dv in dichVuVuaThem)
                        {
                            dv.isCheckRowItem = true;
                        }

                    }
                }
            }
            // dịch vụ gói thường dùng
            if (goiThuongDungCus != null)
            {
                if (goiThuongDungCus.Any())
                {
                    var listChiDinh = new List<ChiDinhDichVuGridVo>();

                    foreach (var items in result.YeuCauKhacGrid)
                    {
                        listChiDinh.Add(items);
                    }
                    var listChiDinhCheckCu = new List<ChiDinhDichVuGridVo>();
                    foreach (var items in goiThuongDungCus)
                    {
                        if (items.isCheckRowItem == true)
                        {
                            listChiDinhCheckCu.Add(items);
                        }
                        var item = listChiDinh.Find(s => s.Id == items.Id && s.NhomId == items.NhomId);
                        listChiDinh.Remove(item);
                        //foreach (var dv in dichVuVuaThem)
                        //{
                        //    dv.isCheckRowItem = true;
                        //}

                    }
                    foreach (var items in result.YeuCauKhacGrid)
                    {
                        foreach (var item in listChiDinh)
                        {
                            if (items.Id == item.Id && items.NhomId == item.NhomId)
                            {
                                items.isCheckRowItem = true;
                            }
                        }
                    }
                    // check cu
                    if (listChiDinhCheckCu.Any())
                    {
                        foreach (var items in result.YeuCauKhacGrid)
                        {
                            foreach (var item in listChiDinhCheckCu)
                            {
                                if (items.Id == item.Id && items.NhomId == item.NhomId)
                                {
                                    items.isCheckRowItem = true;
                                }
                            }
                        }
                    }
                }

            }

            if (dichVuChiDinhCus != null && dichVuChiDinhCus.Any())
            {
                foreach (var items in result.YeuCauKhacGrid)
                {
                    if (!dichVuChiDinhCus.Any(x => x.Id == items.Id && x.NhomId == items.NhomId))
                    {
                        items.isCheckRowItem = true;
                    }
                }
            }
            return result;
        }
        #endregion private class

        #region BẢNG KÊ CHI PHÍ KHÁM BỆNH
        [HttpGet("GetHtmlPhieuChiPhiKhamBenhCoBHYT")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public ActionResult GetHtmlPhieuChiPhiKhamBenhCoBHYT(long yeuCauTiepNhanId, string hostingName)
        {
            var htmlCPKhamNgoaiTru = _yeuCauTiepNhanService.GetHtmlPhieuChiPhiKhamBenhCoBHYT(yeuCauTiepNhanId, hostingName);
            return Ok(htmlCPKhamNgoaiTru);
        }

        [HttpGet("GetHtmlPhieuChiPhiKhamBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public ActionResult GetHtmlPhieuChiPhiKhamBenh(long yeuCauTiepNhanId, string hostingName)
        {
            var htmlCPKhamNgoaiTru = _yeuCauTiepNhanService.GetHtmlPhieuChiPhiKhamBenh(yeuCauTiepNhanId, hostingName);
            return Ok(htmlCPKhamNgoaiTru);
        }

        [HttpGet("InBangKeChiPhiKhamBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public ActionResult InBangKeChiPhiKhamBenh(long yeuCauTiepNhanId, string hostingName)
        {
            var content = "";
            if (content != "") content = content + "<div class=\"pagebreak\"> </div>";
            var phieuKhamCoBHYT = _yeuCauTiepNhanService.GetHtmlPhieuChiPhiKhamBenhCoBHYT(yeuCauTiepNhanId, hostingName);
            if (phieuKhamCoBHYT != "")
                content += _yeuCauTiepNhanService.GetHtmlPhieuChiPhiKhamBenhCoBHYT(yeuCauTiepNhanId, hostingName) + "<div class=\"pagebreak\"></div>";
            else
                content += string.Empty;

            content += _yeuCauTiepNhanService.GetHtmlPhieuChiPhiKhamBenh(yeuCauTiepNhanId, hostingName);
            return Ok(content);
        }

        [HttpPost("KiemTraTheVoucherSuDung")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult KiemTraTheVoucherSuDung([FromBody]ThongTinVoucherTheoYeuCauTiepNhan model)
        {
            var getThongTinVouchers = _yeuCauTiepNhanService.KiemTraTheVoucherSuDung(model);
            return Ok(getThongTinVouchers);
        }

        #endregion


        #region Danh sách các dịch vụ khuyến mãi theo người bệnh

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDichVuKhuyenMai")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDichVuKhuyenMai([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetDataForGridAsyncDichVuKhuyenMais(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDichVuKhuyenMai")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDichVuKhuyenMai([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetTotalPageForGridAsyncDichVuKhuyenMais(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDichVuKhuyenMaiChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDichVuKhuyenMaiChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetDanhSachDichVuKhuyenMaiForGrid(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDichVuKhuyenMaiChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDichVuKhuyenMaiChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetTotalPageForGridAsyncDichVuKhuyenMaiChild(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("KiemTraValidationChiDinhDichVuKhuyenMaiTrongGoiMarketing")]
        public async Task<ActionResult> KiemTraValidationChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel chiDinhViewModel)
        {
            if (!chiDinhViewModel.DichVus.Any())
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.Required"));
            }
            if (chiDinhViewModel.DichVus.Any(x => x.SoLuongSuDung == 0))
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongChiDinh.Required"));
            }

            if (chiDinhViewModel.IsVacxin)
            {
                var lstTenLoi = new List<string>();
                if (chiDinhViewModel.DichVus.Any(x => x.ViTriTiem == null || x.ViTriTiem == 0))
                {
                    lstTenLoi.Add("Vị trí tiêm");
                    //throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.ViTriTiem.Required"));
                }
                if (chiDinhViewModel.DichVus.Any(x => x.MuiSo == null || x.MuiSo == 0))
                {
                    lstTenLoi.Add("Mũi số");
                    //throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.MuiSo.Required"));
                }
                if (chiDinhViewModel.DichVus.Any(x => x.NoiThucHienId == null || x.NoiThucHienId == 0))
                {
                    lstTenLoi.Add("Nơi thực hiện");
                    //throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.NoiThucHienId.Required"));
                }

                if (lstTenLoi.Any())
                {
                    throw new ApiException(string.Format(_localizationService.GetResource("Common.DanhSach.Required"), lstTenLoi.Join(", ")));
                }
            }

            var yeuCauVo = chiDinhViewModel.Map<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>();
            await _tiepNhanBenhNhanService.KiemTraSoLuongConLaiCuaDichVuKhuyenMaiTrongGoiAsync(yeuCauVo);
            return Ok();
        }

        [HttpPost("KiemTraDichVuKhuyenMaiTrongGoiMarketingDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo>>> KiemTraDichVuKhuyenMaiTrongGoiMarketingDaCoTheoYeuCauTiepNhanAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel chiDinhViewModel)
        {
            var dichVuTrung = 
                await _tiepNhanBenhNhanService.KiemTraValidationChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVus.Select(a => a.Id).ToList(), chiDinhViewModel.NoiTruPhieuDieuTriId);
            return dichVuTrung;
        }

        [HttpPost("ThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuKhuyenMaiResultVo>> ThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel yeuCauViewModel)
        {
            // kiểm tra yêu cầu khám bệnh trước khi thêm
            if (yeuCauViewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(yeuCauViewModel.YeuCauKhamBenhId ?? 0);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId ?? 0);
            }

            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            // kiểm tra nếu chưa nhập chẩn đoán sơ bộ thì ko cho thêm dvkt, ghi nhận thuốc/VTTH
            _yeuCauKhamBenhService.KiemTraChanDoanSoBoKhiThemDichVu(yeuCauTiepNhanChiTiet, yeuCauViewModel.YeuCauKhamBenhId ?? 0);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>();
            await _tiepNhanBenhNhanService.XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuKhuyenMaiResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);

            if (yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == yeuCauVo.YeuCauKhamBenhId).TrangThai != EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                && yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang))
            {
                chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh = true;
                var phongHangDoiHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (yeuCauViewModel.IsKhamBenhDangKham)
                {
                    var hangDoi = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs
                        .Where(x => x.Id == yeuCauVo.YeuCauKhamBenhId && x.YeuCauTiepNhanId == yeuCauVo.YeuCauTiepNhanId)
                        .SelectMany(x => x.PhongBenhVienHangDois)
                        .OrderByDescending(x => x.Id)
                        .ToList();
                    if (hangDoi.Any())
                    {
                        phongHangDoiHienTaiId = hangDoi.First().PhongBenhVienId;
                    }
                }
                await _khamBenhService.CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(yeuCauVo.YeuCauTiepNhanId, yeuCauVo.YeuCauKhamBenhId ?? 0, phongHangDoiHienTaiId);
            }

            chiDinhDichVuResultVo.IsVuotQuaBaoLanhGoi = yeuCauVo.IsVuotQuaBaoLanhGoi;
            return chiDinhDichVuResultVo;
        }

        [HttpPost("ThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<ChiDinhDichVuKhuyenMaiResultVo>> ThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanPhauThuatAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuKyThuatKhuyenMaiNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>();
            await _tiepNhanBenhNhanService.XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            //// kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            //var chiDinhDichVuResultVo = new ChiDinhDichVuKhuyenMaiResultVo();
            //chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            //chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            //chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
            //                                               || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
            //                                               || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);

            //if (yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == yeuCauVo.YeuCauKhamBenhId).TrangThai != EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
            //    && yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
            //                                              || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
            //                                              || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
            //                                              || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang))
            //{
            //    chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh = true;
            //    var phongHangDoiHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            //    if (yeuCauViewModel.IsKhamBenhDangKham)
            //    {
            //        var hangDoi = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs
            //            .Where(x => x.Id == yeuCauVo.YeuCauKhamBenhId && x.YeuCauTiepNhanId == yeuCauVo.YeuCauTiepNhanId)
            //            .SelectMany(x => x.PhongBenhVienHangDois)
            //            .OrderByDescending(x => x.Id)
            //            .ToList();
            //        if (hangDoi.Any())
            //        {
            //            phongHangDoiHienTaiId = hangDoi.First().PhongBenhVienId;
            //        }
            //    }
            //    await _khamBenhService.CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(yeuCauVo.YeuCauTiepNhanId, yeuCauVo.YeuCauKhamBenhId ?? 0, phongHangDoiHienTaiId);
            //}

            //chiDinhDichVuResultVo.IsVuotQuaBaoLanhGoi = yeuCauVo.IsVuotQuaBaoLanhGoi;
            //return chiDinhDichVuResultVo;

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuKhuyenMaiResultVo();

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhanChiTiet.NoiTruBenhAn != null || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanChiTiet.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            //dịch vụ chỉ định từ gói marketing ko cần kiểm tra trạng thái thanh toán
            chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);

            return chiDinhDichVuResultVo;
        }

        [HttpPost("ThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ChiDinhDichVuKhuyenMaiResultVo>> ThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanNoiTruAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuKyThuatNoiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>();
            await _tiepNhanBenhNhanService.XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            #region //BVHD-3575: bổ sung thêm dv khám từ nội trú
            var lstDichVuKhamThemTuNoiTru =
                yeuCauVo.YeuCauKhamBenhNews.Where(x => x.LaChiDinhTuNoiTru != null && x.LaChiDinhTuNoiTru == true).ToList();
            if (lstDichVuKhamThemTuNoiTru.Any())
            {
                if (yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId == null)
                {
                    await _dieuTriNoiTruService.XuLyTaoYeuCauNgoaiTruTheoNoiTru(yeuCauTiepNhanChiTiet);
                }

                //var yeuCauTiepNhanNgoaiTru = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
                var yeuCauTiepNhanNgoaiTru = await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
                foreach (var dichVuKham in lstDichVuKhamThemTuNoiTru)
                {
                    dichVuKham.YeuCauTiepNhanId = yeuCauTiepNhanNgoaiTru.Id;
                    yeuCauTiepNhanNgoaiTru.YeuCauKhamBenhs.Add(dichVuKham);
                }

                await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanNgoaiTru);
            }
            #endregion

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuKhuyenMaiResultVo();
            return chiDinhDichVuResultVo;
        }

        [HttpPost("ThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanTiemChungTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<ChiDinhDichVuKhuyenMaiResultVo>> ThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanTiemChungTrucTiepAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>();
            await _tiepNhanBenhNhanService.XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuKhuyenMaiResultVo();

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhanChiTiet.NoiTruBenhAn != null || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanChiTiet.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            //dịch vụ chỉ định từ gói marketing ko cần kiểm tra trạng thái thanh toán
            chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);

            return chiDinhDichVuResultVo;
        }

        [HttpPost("ThemChiDinhGoiDichVuKhuyenMaiTheoTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemChiDinhGoiDichVuKhuyenMaiTheoTiepNhanAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>();
            yeuCauVo.DichVuKyThuatKhongHuongBHYT = true;
            await _tiepNhanBenhNhanService.XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);


            //update basic -> xử lý khá củ chuối, nhưng vì dùng lại hàm xử lý bên khám bệnh nên tạm thời làm vậy
            //var entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(yeuCauViewModel.YeuCauTiepNhanId);
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(yeuCauViewModel.YeuCauTiepNhanId);
            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null, yeuCauViewModel.ChiDinhDichVuGridVos);
            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("ChuyenDoiDichVuKhuyenMai")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ChuyenDoiDichVuKhuyenMaiAsync(LoaiGiaOrSoLuongChange model)
        {
            #region Cập nhật 21/12/2022
            //var entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(model.yeuCauTiepNhanId ?? 0);
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeMienGiam(model.yeuCauTiepNhanId ?? 0);
            #endregion

            switch (model.nhom)
            {
                case Constants.NhomDichVu.DichVuKhamBenh:
                    var yeuCauDVKB = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.Id == model.yeuCauId);
                    if (yeuCauDVKB != null)
                    {
                        if (yeuCauDVKB.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                        }

                        if (model.LaDichVuKhuyenMai == true)
                        {
                            var thongTin = new ThongTinDichVuTrongGoi()
                            {
                                BenhNhanId = entity.BenhNhanId.Value,
                                DichVuId = yeuCauDVKB.DichVuKhamBenhBenhVienId,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                SoLuong = model.soLuong.Value,
                                NhomGiaId = yeuCauDVKB.NhomGiaDichVuKhamBenhBenhVienId
                            };
                            await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(thongTin);

                            if (yeuCauDVKB.MienGiamChiPhis.Any(x => x.DaHuy != true && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) && x.YeuCauGoiDichVuId != null))
                            {
                                //yeuCauDVKB.SoTienBaoHiemTuNhanChiTra = null;
                                //yeuCauDVKB.SoTienMienGiam = null;
                                foreach (var mienGiam in yeuCauDVKB.MienGiamChiPhis.Where(a => a.DaHuy != true && (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) && a.YeuCauGoiDichVuId != null))
                                {
                                    mienGiam.DaHuy = true;
                                    mienGiam.WillDelete = true;

                                    var giamSoTienMienGiam = yeuCauDVKB.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    if (giamSoTienMienGiam < 0)
                                    {
                                        giamSoTienMienGiam = 0;
                                    }
                                    yeuCauDVKB.SoTienMienGiam = yeuCauDVKB.SoTienMienGiam.GetValueOrDefault() == 0 ? yeuCauDVKB.SoTienMienGiam : giamSoTienMienGiam;
                                }

                                //foreach (var congNo in yeuCauDVKB.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                //{
                                //    congNo.WillDelete = true;
                                //}
                            }

                            yeuCauDVKB.Gia = thongTin.DonGia;
                            var thanhTien = yeuCauDVKB.Gia;
                            var thanhTienMienGiam = thongTin.DonGiaKhuyenMai.Value;

                            var tongTienMienGiam = (thanhTien > thanhTienMienGiam) ? (thanhTien - thanhTienMienGiam) : 0;
                            yeuCauDVKB.SoTienMienGiam = yeuCauDVKB.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                            yeuCauDVKB.MienGiamChiPhis.Add(new MienGiamChiPhi()
                            {
                                YeuCauTiepNhanId = entity.Id,
                                LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                SoTien = tongTienMienGiam,
                                YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                            });
                        }
                        else
                        {
                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKhamBenhAsync(yeuCauDVKB.DichVuKhamBenhBenhVienId, yeuCauDVKB.NhomGiaDichVuKhamBenhBenhVienId);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            yeuCauDVKB.Gia = donGiaBenhVien;
                            var mienGiam = yeuCauDVKB.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                            if (mienGiam != null)
                            {
                                mienGiam.DaHuy = true;
                                mienGiam.WillDelete = true;

                                var giamSoTienMienGiam = yeuCauDVKB.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                if (giamSoTienMienGiam < 0)
                                {
                                    giamSoTienMienGiam = 0;
                                }
                                yeuCauDVKB.SoTienMienGiam = yeuCauDVKB.SoTienMienGiam.GetValueOrDefault() == 0 ? yeuCauDVKB.SoTienMienGiam : giamSoTienMienGiam;
                            }
                        }
                    }
                    break;
                case Constants.NhomDichVu.DichVuKyThuat:
                    var yeuCauDVKT = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == model.yeuCauId);
                    if (yeuCauDVKT != null)
                    {
                        if (yeuCauDVKT.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                        }

                        if (model.LaDichVuKhuyenMai == true)
                        {
                            var thongTin = new ThongTinDichVuTrongGoi()
                            {
                                BenhNhanId = entity.BenhNhanId.Value,
                                DichVuId = yeuCauDVKT.DichVuKyThuatBenhVienId,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                SoLuong = model.soLuong.Value,
                                NhomGiaId = yeuCauDVKT.NhomGiaDichVuKyThuatBenhVienId
                            };
                            await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(thongTin);

                            if (yeuCauDVKT.MienGiamChiPhis.Any(x => x.DaHuy != true && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) && x.YeuCauGoiDichVuId != null))
                            {
                                //yeuCauDVKT.SoTienBaoHiemTuNhanChiTra = null;
                                //yeuCauDVKT.SoTienMienGiam = null;
                                foreach (var mienGiam in yeuCauDVKT.MienGiamChiPhis.Where(a => a.DaHuy != true && (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) && a.YeuCauGoiDichVuId != null))
                                {
                                    mienGiam.DaHuy = true;
                                    mienGiam.WillDelete = true;

                                    var giamSoTienMienGiam = yeuCauDVKT.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    if (giamSoTienMienGiam < 0)
                                    {
                                        giamSoTienMienGiam = 0;
                                    }
                                    yeuCauDVKT.SoTienMienGiam = yeuCauDVKT.SoTienMienGiam.GetValueOrDefault() == 0 ? yeuCauDVKT.SoTienMienGiam : giamSoTienMienGiam;
                                }

                                //foreach (var congNo in yeuCauDVKT.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                //{
                                //    congNo.WillDelete = true;
                                //}
                            }

                            yeuCauDVKT.Gia = thongTin.DonGia;
                            var thanhTien = yeuCauDVKT.SoLan * yeuCauDVKT.Gia;
                            var thanhTienMienGiam = yeuCauDVKT.SoLan * thongTin.DonGiaKhuyenMai.Value;

                            var tongTienMienGiam = (thanhTien > thanhTienMienGiam) ? (thanhTien - thanhTienMienGiam) : 0;
                            yeuCauDVKT.SoTienMienGiam = yeuCauDVKT.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                            yeuCauDVKT.MienGiamChiPhis.Add(new MienGiamChiPhi()
                            {
                                YeuCauTiepNhanId = entity.Id,
                                LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                SoTien = tongTienMienGiam,
                                YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                            });
                        }
                        else
                        {
                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(yeuCauDVKT.DichVuKyThuatBenhVienId, yeuCauDVKT.NhomGiaDichVuKyThuatBenhVienId);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            yeuCauDVKT.Gia = donGiaBenhVien;
                            var mienGiam = yeuCauDVKT.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                            if (mienGiam != null)
                            {
                                mienGiam.DaHuy = true;
                                mienGiam.WillDelete = true;

                                var giamSoTienMienGiam = yeuCauDVKT.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                if (giamSoTienMienGiam < 0)
                                {
                                    giamSoTienMienGiam = 0;
                                }
                                yeuCauDVKT.SoTienMienGiam = yeuCauDVKT.SoTienMienGiam.GetValueOrDefault() == 0 ? yeuCauDVKT.SoTienMienGiam : giamSoTienMienGiam;
                            }
                        }
                    }
                    break;
                case Constants.NhomDichVu.DichVuGiuong:

                    break;
                case Constants.NhomDichVu.DuocPham:
                    break;
                default:
                    break;
            };

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("ThemChiDinhGoiDichVuKhuyenMaiKhiTaoYeuCauTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemChiDinhGoiDichVuKhuyenMaiKhiTaoYeuCauTiepNhanAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            var yeuCauTiepNhanChiTiet = new YeuCauTiepNhan();
                //await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>();
            yeuCauVo.DichVuKyThuatKhongHuongBHYT = true;
            yeuCauVo.LaThemTam = true;
            yeuCauTiepNhanChiTiet.CoBHYT = yeuCauViewModel.DuocHuongBaoHiem;
            yeuCauVo.DuocHuongBaoHiemTam = yeuCauViewModel.DuocHuongBaoHiem;
            await _tiepNhanBenhNhanService.XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);

            return Ok(yeuCauVo.DichVuKhuyenMaiThemTamTuTiepNhan);
        }

        [HttpPost("ThemChiDinhGoiDichVuKhuyenMaiTheoKhamSangLoc")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<TiemChungChiDinhDichVuViewModel>> ThemChiDinhGoiDichVuKhuyenMaiTheoKhamSangLocAsync([FromBody] ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            var yeuCauTiepNhanChiTiet = _yeuCauTiepNhanService.GetById(yeuCauViewModel.YeuCauTiepNhanId);
            //await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo>();
            await _tiepNhanBenhNhanService.XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVu = new TiemChungChiDinhDichVuViewModel();
            chiDinhDichVu.IsVuotQuaBaoLanhGoi = yeuCauVo.IsVuotQuaBaoLanhGoi;

            var lstYeuCauGoiDichVuId =
                yeuCauVo.YeuCauDichVuKyThuatNews
                    .Where(x => x.YeuCauGoiDichVuId != null)
                    .Select(x => x.YeuCauGoiDichVuId.Value).Distinct().ToList();
            var lstTenGoiDichVu = await _tiemChungService.GetListTenGoiDichVu(lstYeuCauGoiDichVuId);

            foreach (var item in yeuCauVo.YeuCauDichVuKyThuatNews)
            {
                var vacXinMoiThem = item.ToModel<YeuCauKhamTiemChungViewModel>();

                if (vacXinMoiThem.YeuCauGoiDichVuId != null)
                {
                    vacXinMoiThem.TenGoiDichVu = lstTenGoiDichVu
                        .Where(x => x.KeyId == vacXinMoiThem.YeuCauGoiDichVuId.Value).Select(x => x.DisplayName)
                        .FirstOrDefault();
                    vacXinMoiThem.YeuCauGoiDichVuKhuyenMaiId = item.YeuCauGoiDichVuId;
                    vacXinMoiThem.YeuCauGoiDichVuId = null;
                }

                chiDinhDichVu.YeuCauDichVuKyThuats.Add(vacXinMoiThem);
            }

            return chiDinhDichVu;
        }
        #endregion


        #region Update gói marketing

        [HttpPost("ChuyenDoiDichVuTrongVaNgoaiGoi")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ChuyenDoiDichVuTrongVaNgoaiGoiAsync(LoaiGiaOrSoLuongChange model)
        {
            #region Cập nhật 21/12/2022
            //var entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(model.yeuCauTiepNhanId ?? 0);
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeMienGiam(model.yeuCauTiepNhanId ?? 0);
            #endregion

            switch (model.nhom)
            {
                case Constants.NhomDichVu.DichVuKhamBenh:
                    var yeuCauDVKB = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.Id == model.yeuCauId);
                    if (yeuCauDVKB != null)
                    {
                        if (model.LaDichVuTrongGoi == true)
                        {
                            if (model.soLuong == null)
                            {
                                throw new ApiException(_localizationService.GetResource("CapNhatGridDichVuTiepNhan.SoLuong.Required"));
                            }
                            var thongTin = new ThongTinDichVuTrongGoi()
                            {
                                BenhNhanId = entity.BenhNhanId.Value,
                                DichVuId = yeuCauDVKB.DichVuKhamBenhBenhVienId,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                SoLuong = model.soLuong.Value
                            };
                            await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                            yeuCauDVKB.Gia = thongTin.DonGia;
                            yeuCauDVKB.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                            yeuCauDVKB.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                            yeuCauDVKB.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;

                            if (yeuCauDVKB.MienGiamChiPhis.Any(x => x.DaHuy != true && x.TaiKhoanBenhNhanThuId == null))
                            {
                                yeuCauDVKB.SoTienMienGiam = null;
                                foreach (var mienGiam in yeuCauDVKB.MienGiamChiPhis.Where(a => a.DaHuy != true && a.TaiKhoanBenhNhanThuId == null))
                                {
                                    mienGiam.WillDelete = true;
                                }
                            }



                            if (yeuCauDVKB.CongTyBaoHiemTuNhanCongNos.Any(x => x.DaHuy != true && x.TaiKhoanBenhNhanThuId == null))
                            {
                                yeuCauDVKB.SoTienBaoHiemTuNhanChiTra = null;

                                foreach (var congNo in yeuCauDVKB.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true && a.TaiKhoanBenhNhanThuId == null))
                                {
                                    congNo.WillDelete = true;
                                }
                            }
                        }
                        else
                        {
                            yeuCauDVKB.DonGiaTruocChietKhau = null;
                            yeuCauDVKB.DonGiaSauChietKhau = null;
                            yeuCauDVKB.YeuCauGoiDichVuId = null;

                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKhamBenhAsync(yeuCauDVKB.DichVuKhamBenhBenhVienId, yeuCauDVKB.NhomGiaDichVuKhamBenhBenhVienId);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            yeuCauDVKB.Gia = donGiaBenhVien;
                        }

                    }
                    break;
                case Constants.NhomDichVu.DichVuKyThuat:
                    var yeuCauDVKT = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == model.yeuCauId);
                    if (yeuCauDVKT != null)
                    {
                        if (model.LaDichVuTrongGoi == true)
                        {
                            if (model.soLuong == null)
                            {
                                throw new ApiException(_localizationService.GetResource("CapNhatGridDichVuTiepNhan.SoLuong.Required"));
                            }
                            var thongTin = new ThongTinDichVuTrongGoi()
                            {
                                BenhNhanId = entity.BenhNhanId.Value,
                                DichVuId = yeuCauDVKT.DichVuKyThuatBenhVienId,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                SoLuong = model.soLuong.Value
                            };
                            await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                            yeuCauDVKT.Gia = thongTin.DonGia;
                            yeuCauDVKT.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                            yeuCauDVKT.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                            yeuCauDVKT.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;

                            if (yeuCauDVKT.MienGiamChiPhis.Any(x => x.DaHuy != true && x.TaiKhoanBenhNhanThuId == null))
                            {
                                yeuCauDVKT.SoTienMienGiam = null;
                                foreach (var mienGiam in yeuCauDVKT.MienGiamChiPhis.Where(a => a.DaHuy != true && a.TaiKhoanBenhNhanThuId == null))
                                {
                                    mienGiam.WillDelete = true;
                                }
                            }

                            if (yeuCauDVKT.CongTyBaoHiemTuNhanCongNos.Any(x => x.DaHuy != true && x.TaiKhoanBenhNhanThuId == null))
                            {
                                yeuCauDVKT.SoTienBaoHiemTuNhanChiTra = null;

                                foreach (var congNo in yeuCauDVKT.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true && a.TaiKhoanBenhNhanThuId == null))
                                {
                                    congNo.WillDelete = true;
                                }
                            }
                        }
                        else
                        {
                            yeuCauDVKT.DonGiaTruocChietKhau = null;
                            yeuCauDVKT.DonGiaSauChietKhau = null;
                            yeuCauDVKT.YeuCauGoiDichVuId = null;

                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(yeuCauDVKT.DichVuKyThuatBenhVienId, yeuCauDVKT.NhomGiaDichVuKyThuatBenhVienId);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            yeuCauDVKT.Gia = donGiaBenhVien;
                        }

                    }
                    break;
                case Constants.NhomDichVu.DichVuGiuong:

                    break;
                case Constants.NhomDichVu.DuocPham:
                    break;
                default:
                    break;
            };

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            #region Cập nhật 21/12/2022
            entity = null;
            entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(model.yeuCauTiepNhanId ?? 0);
            #endregion

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, null);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpGet("KiemTraTatCaDichVuTrongGoi/{yeuCauTiepNhanId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult KiemTraTatCaDichVuTrongGoi(long yeuCauTiepNhanId)
        {
            var kiemTraTatCaDichVuTrongGoi = _tiepNhanBenhNhanService.KiemTraTatCaDichVuTrongGoi(yeuCauTiepNhanId).Result;
            return Ok(kiemTraTatCaDichVuTrongGoi);
        }

        #endregion

        #region Update nhóm dịch vụ thường dùng
        [HttpPost("ThemYeuGoiDichVuThuongDung")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemYeuGoiDichVuThuongDungAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauVo = yeuCauViewModel.Map<YeuCauThemGoiDichVuThuongDungVo>();
            await _khamBenhService.XuLyThemGoiDichVuThuongDungAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            //update basic -> xử lý khá củ chuối, nhưng vì dùng lại hàm xử lý bên khám bệnh nên tạm thời làm vậy
            //var entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(yeuCauViewModel.YeuCauTiepNhanId);
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(yeuCauViewModel.YeuCauTiepNhanId);
            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, true, null, null, null, yeuCauViewModel.ChiDinhDichVuGridVos);
            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("KiemTraDichVuTrongGoiDaCoTheoDichVuTrongGrid")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>>> KiemTraDichVuTrongGoiDaCoTheoDichVuTrongGridAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungTaoYCTNViewModel chiDinhViewModel)
        {
            var dichVuTrung = await _khamBenhService.KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhanAsync(0, chiDinhViewModel.GoiDichVuIds, chiDinhViewModel.DanhSachDichVuChons);
            return dichVuTrung;
        }

        [HttpPost("ThemYeuGoiDichVuThuongDungTaoMoiYCTN")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemYeuGoiDichVuThuongDungTaoMoiYCTNAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungTaoYCTNViewModel yeuCauViewModel)
        {
            var yeuCauVo = yeuCauViewModel.Map<YeuCauThemGoiDichVuThuongDungVo>();
            var results = await _tiepNhanBenhNhanService.ThemYeuGoiDichVuThuongDungTaoMoiYCTNAsync(yeuCauVo);
            //var result = await _tiepNhanBenhNhanService.GetDataForDichVuKhamBenhGridAsync(model); // khám
            //var result = await _tiepNhanBenhNhanService.GetDataForDichVuKyThuatGridAsync(model, 1); // dvkt
            return Ok(results);
        }
        #endregion

        #region [PHÁT SINH TRIỂN KHAI] Cập nhật chỉ định trong khám bệnh cho những DV khác 4 nhóm: PTTT, XN, CDHA, TDCN
        [HttpGet("GetGridDichVuDaChiDinh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetGridDichVuDaChiDinhAsync(long yeuCauTiepNhanId)
        {
            //var entity = await _tiepNhanBenhNhanService.GetByIdHaveInclude(yeuCauTiepNhanId);
            var entity = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(yeuCauTiepNhanId);
            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();
            result = SetValueForGrid(result, entity, true, null, null, null, null);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpGet("GetYeuCauDichVuKyThuatById")]
        public async Task<ActionResult<TrangThaiThucHienYeuCauDichVuKyThuatViewModel>> GetYeuCauDichVuKyThuatByIdAsync(long yeuCauDichVuKyThuatId)
        {
            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(yeuCauDichVuKyThuatId);
            var result = yeuCauDichVuKyThuat.ToModel<TrangThaiThucHienYeuCauDichVuKyThuatViewModel>();

            result.NhanVienThucHienId = result.NhanVienThucHienId == null ? _userAgentHelper.GetCurrentUserId() : result.NhanVienThucHienId;
            result.ThoiDiemThucHien = result.ThoiDiemThucHien == null ? DateTime.Now : result.ThoiDiemThucHien;

            return result;
        }

        [HttpPut("XuLyCapNhatThucHienYeuCauDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> XuLyCapNhatThucHienYeuCauDichVuKyThuatAsync([FromBody] TrangThaiThucHienYeuCauDichVuKyThuatViewModel viewModel)
        {
            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(viewModel.Id);
            if (yeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            {
                throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiYeuCauDichVuKyThuat.DaHuy"));
            }
            viewModel.ToEntity(yeuCauDichVuKyThuat);
            if (viewModel.LaThucHienDichVu)
            {
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.CapNhatThanhToan)
                {
                    throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiThanhToanYeuCauDichVuKyThuat.ChuaThanhToan"));
                }

                //yeuCauDichVuKyThuat.LyDoHuyTrangThaiDaThucHien = null;
                //yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = null;
                yeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            }
            else
            {
                yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauDichVuKyThuat.NhanVienThucHienId = yeuCauDichVuKyThuat.NhanVienKetLuanId = null;
                yeuCauDichVuKyThuat.ThoiDiemThucHien = yeuCauDichVuKyThuat.ThoiDiemHoanThanh = null;
                yeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            }

            _yeuCauDichVuKyThuatService.Update(yeuCauDichVuKyThuat);
            return Ok();
        }
        #endregion

        #region [PHÁT SINH TRIỂN KHAI] [Tiếp đón] BN BHYT: sửa điều kiện được phép tạo yêu cầu tiếp nhận mới
        [HttpPost("KiemTraDieuKienTaoMoiYeuCauTiepNhan")]
        public async Task<ActionResult<KetQuaKiemTraTaoMoiYeuCauTiepNhanVo>> KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(long benhNhanId, bool? laKiemTraCungNgay = false)
        {
            var kiemTra = await _tiepNhanBenhNhanService.KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(benhNhanId, laKiemTraCungNgay);

            return kiemTra;
        }


        #endregion

        #region [PHÁT SINH TRIỂN KHAI][THU NGÂN] YÊU CẦU HIỂN THỊ CẢNH BÁO KHI THÊM NB CÓ TÊN, NGÀY SINH VÀ SĐT GIỐNG NHAU
        [HttpPost("KiemTraBenhNhanTrongHeThong")]
        public async Task<ActionResult<KiemTraNguoiBenhTrongHeThongVo>> KiemTraBenhNhanTrongHeThongAsync([FromBody]TiepNhanBenhNhanViewModel model)
        {
            KiemTraNguoiBenhTrongHeThongVo kiemTraVo = new KiemTraNguoiBenhTrongHeThongVo()
            {
                CoBHYT = model.CoBHYT,
                BHYTMaSoThe = model.BHYTMaSoThe,
                HoTen = model.HoTen,
                NgayThangNamSinh = model.NgayThangNamSinh,
                NamSinh = model.NamSinh,
                SoDienThoai = model.SoDienThoai
            };
            var kiemTra = await _tiepNhanBenhNhanService.KiemTraBenhNhanTrongHeThongAsync(kiemTraVo);
            return kiemTra;
        }


        #endregion
        #region BVHD-3761
        [HttpGet("GetSarsCoVs")]
        
        public async Task<ActionResult> GetSarsCoVs()
        {
            var result = await _tiepNhanBenhNhanService.GetSarsCoVs();
            return Ok(result);
        }
        [HttpGet("InPhieuXetNghiemCovid")]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.KhamBenh)]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> InPhieuXetNghiemCovid(long id, string hostingName)
        {
            var entity = await _tiepNhanBenhNhanService.GetByIdAsync(id, s => s.Include(u => u.BHYTGiayMienCungChiTra)
                .Include(u => u.BenhNhan)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien).ThenInclude(o => o.DichVuKhamBenh)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien).ThenInclude(o => o.DichVuKhamBenh)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.DichVuKyThuatBenhVien).ThenInclude(o => o.DichVuKyThuat)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.DichVuGiuongBenhVien).ThenInclude(o => o.DichVuGiuong)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.VatTuBenhVien).ThenInclude(o => o.VatTus)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.DuocPhamBenhVien).ThenInclude(o => o.DuocPham)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiDangKy).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(u => u.NoiTiepNhan).Include(u => u.NhanVienTiepNhan).ThenInclude(o => o.User)
                .Include(cc => cc.PhuongXa)
                .Include(cc => cc.QuanHuyen)
                .Include(cc => cc.TinhThanh)
                .Include(u => u.NguoiLienHeQuanHeNhanThan)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhanVienChiDinh).ThenInclude(o => o.HocHamHocVi)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
            );

            var now = DateTime.Now;
            var content = "";
            #region BVHD-3761
            var dichVuKyThuatIds = new List<long>();
            var info = _tiepNhanBenhNhanService.GetSarsCoVs();
            if (info.Result.Ids != null)
            {
                dichVuKyThuatIds = info.Result.Ids;
            }
            if (entity.YeuCauDichVuKyThuats.Where(d => dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId)).ToList() != null)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    content = content + "<div class=\"pagebreak\"> </div>";
                }
                var listdichVuCanIns = new List<long>();
                content = AddPhieuInDichVuKyThuatSarsCoV2(content, LoaiDichVuKyThuat.XetNghiem, entity, hostingName, listdichVuCanIns);
            }

            #endregion BVHD-3761

            return Ok(content);
        }
        [HttpPost("GetKiemTraYeuCauDichVuKyThuatThuocNhomSarsCov2")]

        public async Task<ActionResult> GetKiemTraYeuCauDichVuKyThuatThuocNhomSarsCov2([FromBody]DichVuChiDinhYCTNViewModel model)
        {
            var result = await _tiepNhanBenhNhanService.GetKiemTraYeuCauDichVuKyThuatThuocNhomSarsCov2(model.YeuCauDichVuKyThuatIds);
            return Ok(result);
        }

        #region dịch vụ bác sĩ khác chỉ định
        [HttpPost("InPhieuXetNghiemCovidBacSiKhacChiDinh")]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.YeuCauTiepNhan)]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.KhamBenh)]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> InPhieuXetNghiemCovidBacSiKhacChiDinh([FromBody]DichVuChiDinhBacSyKhacChiDinhViewModel model)
        {
            var entity = await _tiepNhanBenhNhanService.GetByIdAsync(model.YeuCauTiepNhanId ,s => s.Include(u => u.BHYTGiayMienCungChiTra)
                .Include(u => u.BenhNhan)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien).ThenInclude(o => o.DichVuKhamBenh)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien).ThenInclude(o => o.DichVuKhamBenh)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.DichVuKyThuatBenhVien).ThenInclude(o => o.DichVuKyThuat)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.DichVuGiuongBenhVien).ThenInclude(o => o.DichVuGiuong)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.VatTuBenhVien).ThenInclude(o => o.VatTus)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.DuocPhamBenhVien).ThenInclude(o => o.DuocPham)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiDangKy).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                .Include(u => u.NoiTiepNhan).Include(u => u.NhanVienTiepNhan).ThenInclude(o => o.User)
                .Include(cc => cc.PhuongXa)
                .Include(cc => cc.QuanHuyen)
                .Include(cc => cc.TinhThanh)
                .Include(u => u.NguoiLienHeQuanHeNhanThan)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhanVienChiDinh).ThenInclude(o => o.HocHamHocVi)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
            );

            var now = DateTime.Now;
            var content = "";
            #region BVHD-3761
            var dichVuKyThuatIds = new List<long>();
            var info = _tiepNhanBenhNhanService.GetSarsCoVs();
            if (info.Result.Ids != null)
            {
                dichVuKyThuatIds = info.Result.Ids;
            }
            if (entity.YeuCauDichVuKyThuats.Where(d => dichVuKyThuatIds.Contains(d.DichVuKyThuatBenhVienId)).ToList() != null)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    content = content + "<div class=\"pagebreak\"> </div>";
                }
               
                content = AddPhieuInDichVuKyThuatSarsCoV2(content, LoaiDichVuKyThuat.XetNghiem, entity, model.Hosting, model.YeuCauDichVuKyThuatIds);
            }

            #endregion BVHD-3761

            return Ok(content);
        }

        [HttpGet("GetInfoSarsCoVTheoYeuCauTiepNhan")]

        public async Task<ActionResult> GetInfoSarsCoVTheoYeuCauTiepNhan(long id)
        {
            var result = await _tiepNhanBenhNhanService.GetInfoSarsCoVTheoYeuCauTiepNhan(id);
            return Ok(result);
        }
        [HttpGet("GetInfoSarsCoVTheoYeuCauTiepNhanNoiTru")]

        public async Task<ActionResult> GetInfoSarsCoVTheoYeuCauTiepNhanNoiTru(long id)
        {
            var result = await _tiepNhanBenhNhanService.GetInfoSarsCoVTheoYeuCauTiepNhanNoiTru(id);
            return Ok(result);
        }
        #endregion dịch vụ bác sĩ khác chỉ định
        #endregion
    }
}