using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DanhMucMarketings;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.BenhNhans;
using Camino.Services.DanhMucMarketing;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.TiepNhanBenhNhan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public class DanhMucMarketingController : CaminoBaseController
    {
        private readonly IDanhMucMarketingService _danhMucMarketingService;
        private readonly ILocalizationService _localizationService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IExcelService _excelService;

        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;
        private readonly IPdfService _pdfService;

        public DanhMucMarketingController(IDanhMucMarketingService danhMucMarketingService
            , ILocalizationService localizationService, IBenhNhanService benhNhanService
            , ITiepNhanBenhNhanService tiepNhanBenhNhanService
            , IPdfService pdfService
            , IUserAgentHelper userAgentHelper, IExcelService excelService)
        {
            _danhMucMarketingService = danhMucMarketingService;
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _benhNhanService = benhNhanService;
            _excelService = excelService;
            _pdfService = pdfService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;

        }


        [HttpGet("AllGoiTheoBenhBenh")]
        public ActionResult AllGoiTheoBenhBenh(long benhNhanId)
        {
            var bangKeSuDungs = new List<HtmlToPdfVo>();
            var getAllGoiTheoBenhNhans = _danhMucMarketingService.GetAllGoiTheoBenhNhan(benhNhanId);           
            return Ok(getAllGoiTheoBenhNhans);
        }

        [HttpPost("InBangKeGoiMarketingBenhNhan")]
        public ActionResult InBangKeGoiMarketingBenhNhan(long benhNhanId, string hostingName)
        {
            var bangKeSuDungs = new List<HtmlToPdfVo>();
            var getAllGoiTheoBenhNhans = _danhMucMarketingService.GetAllGoiTheoBenhNhan(benhNhanId);
            if (getAllGoiTheoBenhNhans.Any())
            {
                foreach (var goiDichVuId in getAllGoiTheoBenhNhans)
                {
                    var itemBangKe = new HtmlToPdfVo
                    {
                        Html = _danhMucMarketingService.AllBangKeGoiDichVu(goiDichVuId, hostingName),
                        PageOrientation = "Portrait",
                        PageSize = "A5"
                    };
                    bangKeSuDungs.Add(itemBangKe);
                }
            }

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(bangKeSuDungs);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopPhieu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("KetXuatGoiTheoBenhNhanExcel")]
        public ActionResult KetXuatGoiTheoBenhNhanExcel(long yeuCauGoiDichVuId)
        {
            byte[] bytes = _danhMucMarketingService.KetXuatGoiTheoBenhNhanExcel(yeuCauGoiDichVuId);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KetXuatGoiTheoBenhNhanExcel_" + yeuCauGoiDichVuId  + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetTotalForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetDataForGridChildAsync(queryInfo, null);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetTotalForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetYeuCauGoiDichVuOfBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> GetYeuCauTiepNhanOfBenhNhan(long id)
        {
            var result = await _danhMucMarketingService.IsExistsYeuCauGoiDichVuOfBenhNhan(id);

            return Ok(result);
        }

        [HttpPost("GetListGoiMarketing")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListGoiMarketing([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _danhMucMarketingService.GetListGoiMarketing(model);
            return Ok(lookup);
        }

        [HttpPost("AddThongTinGoiMarketing")]
        public ActionResult AddThongTinGoiMarketing(ChonGoiMarketing model)
        {
            var data = _danhMucMarketingService.AddThongTinGoiMarketing(model);
            return Ok(data);
        }

        [HttpPost("GetThongGoiMRTBenhNhan")]
        public ActionResult GetThongGoiMRTBenhNhan(long benhNhanId)
        {
            var data = _danhMucMarketingService.GetThongGoiMRTBenhNhan(benhNhanId).Result;
            return Ok(data);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataThongTinGoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataThongTinGoiForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetDataThongTinGoiForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalThongTinGoiPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalThongTinGoiPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetTotalThongTinGoiPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataThongTinGoiDaHoanThanhForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataThongTinGoiDaHoanThanhForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetDataThongTinGoiDaHoanThanhForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalThongTinGoiDaHoanThanhPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalThongTinGoiDaHoanThanhPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetTotalThongTinGoiDaHoanThanhPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDichVuGoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataDichVuGoiForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetDataDichVuGoiForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDichVuGoiPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalDichVuGoiPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetTotalDichVuGoiPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataQuaTangGoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataQuaTangGoiForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetDataQuaTangGoiForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalQuaTangGoiPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalQuaTangGoiPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetTotalQuaTangGoiPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataCacDichVuTrongGoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDataCacDichVuTrongGoiForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetDataCacDichVuTrongGoiForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalCacDichVuTrongGoiPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult<GridDataSource>> GetTotalCacDichVuTrongGoiPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetTotalCacDichVuTrongGoiPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> Post
            ([FromBody] ThongTinGoiMarketingViewModel model)
        {
            var benhNhanNew = new BenhNhan();
            if (model.LstDaChon == null || !model.LstDaChon.Any())
            {
                throw new ApiException(_localizationService.GetResource("DanhMucMarketing.ThongTinGoiMarketing.Required"));
            }
            if (!string.IsNullOrEmpty(model.HoTen))
            {
                model.HoTen = model.HoTen.ToUpper();
            }
            if (model.BenhNhanId == null || model.BenhNhanId == 0)
            {
                //throw new ApiException(_localizationService.GetResource("DanhMucMarketing.BenhNhan.Required"));

                //create new benh nhan

                benhNhanNew.HoTen = model.HoTen;

                if (model.NgayThangNamSinh != null)
                {
                    benhNhanNew.NgaySinh = model.NgayThangNamSinh.GetValueOrDefault().Date.Day;
                    benhNhanNew.ThangSinh = model.NgayThangNamSinh.GetValueOrDefault().Date.Month;
                }

                benhNhanNew.NamSinh = model.NamSinh;

                benhNhanNew.PhuongXaId = model.PhuongXaId;
                benhNhanNew.TinhThanhId = model.TinhThanhId;
                benhNhanNew.QuanHuyenId = model.QuanHuyenId;
                benhNhanNew.QuocTichId = model.QuocTichId;
                benhNhanNew.DanTocId = model.DanTocId;
                benhNhanNew.DiaChi = model.DiaChi;
                benhNhanNew.SoDienThoai = model.SoDienThoai;
                benhNhanNew.SoChungMinhThu = model.SoChungMinhThu;
                benhNhanNew.Email = model.Email;
                benhNhanNew.NgheNghiepId = model.NgheNghiepId;
                benhNhanNew.GioiTinh = model.GioiTinh;
                benhNhanNew.NoiLamViec = model.NoiLamViec;
            }

            if (!await _danhMucMarketingService.CheckDichVuValidate(model.LstDaChon))
            {
                throw new ApiException(_localizationService.GetResource("DanhMucMarketing.DichVuValidate.Required"));
            }

            var lstYeuCau = new List<YeuCauGoiDichVu>();

            foreach (var id in model.LstDaChon)
            {
                var ct = await _danhMucMarketingService.GetChuongTrinhGoiDichVu(id);

                var yc = new YeuCauGoiDichVu();
                if (model.BenhNhanId != null && model.BenhNhanId != 0)
                {
                    yc.BenhNhanId = model.BenhNhanId ?? 0;
                }
                else
                {
                    yc.BenhNhan = benhNhanNew;
                }
                yc.ChuongTrinhGoiDichVuId = id;
                yc.MaChuongTrinh = ct.Ma;
                yc.TenChuongTrinh = ct.Ten;
                yc.GiaTruocChietKhau = ct.GiaTruocChietKhau;
                yc.GiaSauChietKhau = ct.GiaSauChietKhau;
                yc.TenGoiDichVu = ct.TenGoiDichVu;
                yc.MoTaGoiDichVu = ct.MoTaGoiDichVu;
                yc.GoiSoSinh = ct.GoiSoSinh;
                yc.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                yc.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                yc.ThoiDiemChiDinh = DateTime.Now;
                yc.BoPhanMarketingDangKy = true;
                yc.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien;
                yc.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
                yc.BoPhanMarketingDangKy = true;

                lstYeuCau.Add(yc);
            }

            await _danhMucMarketingService.AddRangeAsync(lstYeuCau);

            //update benh nhan
            if (model.BenhNhanId != null && model.BenhNhanId != 0)
            {
                var benhNhan = await _benhNhanService.GetByIdAsync(model.BenhNhanId ?? 0);
                benhNhan.HoTen = model.HoTen;
                benhNhan.NamSinh = model.NamSinh;
                if (model.NgayThangNamSinh != null)
                {
                    benhNhan.NgaySinh = model.NgayThangNamSinh.GetValueOrDefault().Day;
                    benhNhan.ThangSinh = model.NgayThangNamSinh.GetValueOrDefault().Month;
                    benhNhan.NamSinh = model.NgayThangNamSinh.GetValueOrDefault().Year;
                }
                benhNhan.SoDienThoai = model.SoDienThoai;
                benhNhan.GioiTinh = model.GioiTinh;
                benhNhan.NgheNghiepId = model.NgheNghiepId;
                benhNhan.QuocTichId = model.QuocTichId;
                benhNhan.TinhThanhId = model.TinhThanhId;
                benhNhan.QuanHuyenId = model.QuanHuyenId;
                benhNhan.PhuongXaId = model.PhuongXaId;
                benhNhan.DiaChi = model.DiaChi;
                benhNhan.SoChungMinhThu = model.SoChungMinhThu;
                benhNhan.Email = model.Email;
                benhNhan.NoiLamViec = model.NoiLamViec;
                benhNhan.DanTocId = model.DanTocId;

                await _benhNhanService.UpdateAsync(benhNhan);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Get(long id)
        {
            var entity = await _benhNhanService.GetByIdAsync(id,
                                        p => p.Include(x => x.YeuCauGoiDichVus));
            var result = entity.ToModel<ThongTinGoiMarketingViewModel>();

            if (entity.NgaySinh != null && entity.NgaySinh != 0 && entity.ThangSinh != null && entity.ThangSinh != 0 && entity.NamSinh != null && entity.NamSinh != 0)
            {
                result.NgayThangNamSinh = new DateTime(entity.NamSinh ?? 0, entity.ThangSinh ?? 0, entity.NgaySinh ?? 0);
            }

            result.LstDaChon = entity.YeuCauGoiDichVus.Select(p => p.ChuongTrinhGoiDichVuId).ToList();
            result.LstDaHoanThanh = entity.YeuCauGoiDichVus.Where(p => p.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DaThucHien).Select(p => p.ChuongTrinhGoiDichVuId).ToList();

            result.BenhNhanId = id;

            return Ok(result);
        }

        [HttpPost("XuatQua")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> XuatQua
            (long benhNhanId, long chuongTrinhGoiDichVuId)
        {
            var result = await _danhMucMarketingService.XuatQuaTang(benhNhanId, chuongTrinhGoiDichVuId);

            if (result != null)
            {
                var phieuXuat = await _danhMucMarketingService.InPhieuXuat(result.First().YeuCauGoiId ?? 0, result);
                return Ok(phieuXuat);
            }

            return NoContent();
        }

        #region child action
        [HttpPost("RemoveMarketingOfChild")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> RemoveMarketingOfChild(long yeuCauGoiId)
        {
            var yeuCauGoi = await _danhMucMarketingService.GetByIdAsync(yeuCauGoiId);

            await _danhMucMarketingService.DeleteAsync(yeuCauGoi);

            return NoContent();
        }

        [HttpPost("ChangeTrangThaiDangChoNhanTienOfChild")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> ChangeTrangThaiDangChoNhanTienOfChild(long yeuCauGoiId)
        {
            var yeuCauGoi = _danhMucMarketingService.GetById(yeuCauGoiId);

            if (yeuCauGoi.BoPhanMarketingDaNhanTien != true)
            {
                yeuCauGoi.BoPhanMarketingDaNhanTien = true;
            }

            await _danhMucMarketingService.UpdateAsync(yeuCauGoi);

            var yctn = await GetYCTNDangThucHienOfBenhNhan(yeuCauGoi.BenhNhanId);
            if (yctn == null)
            {
                var benhNhan = await _benhNhanService.GetByIdAsync(yeuCauGoi.BenhNhanId, p => p.Include(o => o.YeuCauGoiDichVus));

                var now = DateTime.Now;
                var yctnNew = new YeuCauTiepNhan
                {
                    LoaiYeuCauTiepNhan = Enums.EnumLoaiYeuCauTiepNhan.DangKyGoiMarketing,
                    TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien,
                    MaYeuCauTiepNhan = ResourceHelper.CreateMaYeuCauTiepNhan(),
                    BenhNhanId = yeuCauGoi.BenhNhanId,
                    ThoiDiemTiepNhan = now,
                    ThoiDiemCapNhatTrangThai = now,
                    HoTen = benhNhan.HoTen,
                    NgaySinh = benhNhan.NgaySinh,
                    ThangSinh = benhNhan.ThangSinh,
                    NamSinh = benhNhan.NamSinh,
                    SoChungMinhThu = benhNhan.SoChungMinhThu,
                    GioiTinh = benhNhan.GioiTinh,
                    NhomMau = benhNhan.NhomMau,
                    NgheNghiepId = benhNhan.NgheNghiepId,
                    NoiLamViec = benhNhan.NoiLamViec,
                    QuocTichId = benhNhan.QuocTichId,
                    DanTocId = benhNhan.DanTocId,
                    DiaChi = benhNhan.DiaChi,
                    PhuongXaId = benhNhan.PhuongXaId,
                    QuanHuyenId = benhNhan.QuanHuyenId,
                    TinhThanhId = benhNhan.TinhThanhId,
                    SoDienThoai = benhNhan.SoDienThoai,
                    Email = benhNhan.Email,

                };

                await _tiepNhanBenhNhanService.AddAsync(yctnNew);
            }

            return NoContent();
        }

        [HttpPost("ChangeTrangThaiDangDangSuDungOfChild")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> ChangeTrangThaiDangDangSuDungOfChild(long yeuCauGoiId)
        {
            var yeuCauGoi = await _danhMucMarketingService.GetByIdAsync(yeuCauGoiId);

            if (yeuCauGoi.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien)
            {
                yeuCauGoi.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DaThucHien;
            }

            await _danhMucMarketingService.UpdateAsync(yeuCauGoi);

            return NoContent();
        }

        [HttpPost("XuatQuaOfChild")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> XuatQuaOfChild(long benhNhanId, long chuongTrinhGoiDichVuId)
        {
            var result = await _danhMucMarketingService.XuatQuaTang(benhNhanId, chuongTrinhGoiDichVuId);

            //if (result != null)
            //{
            //    var phieuXuat = await _danhMucMarketingService.InPhieuXuat(result.First().YeuCauGoiId ?? 0, result);
            //    return Ok(phieuXuat);
            //}

            return Ok(result);
        }
        #endregion child action

        [HttpPost("RemoveMarketingOfBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> RemoveMarketingOfBenhNhan
            (long benhNhanId)
        {
            var benhNhan = await _benhNhanService.GetByIdAsync(benhNhanId, p => p.Include(o => o.YeuCauGoiDichVus));

            await _danhMucMarketingService.DeleteAsync(benhNhan.YeuCauGoiDichVus);

            return NoContent();
        }

        [HttpPost("ChangeTrangThaiDangChoNhanTienOfBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> ChangeTrangThaiDangChoNhanTienOfBenhNhan
            (long benhNhanId)
        {

            var benhNhan = await _benhNhanService.GetByIdAsync(benhNhanId, p => p.Include(o => o.YeuCauGoiDichVus));

            var yeuCauGois = benhNhan.YeuCauGoiDichVus.ToList();

            foreach (var yc in yeuCauGois)
            {
                if (yc.BoPhanMarketingDaNhanTien != true)
                {
                    yc.BoPhanMarketingDaNhanTien = true;
                }
            }

            await _danhMucMarketingService.UpdateAsync(yeuCauGois);


            var yctn = await GetYCTNDangThucHienOfBenhNhan(benhNhanId);
            if (yctn == null)
            {
                var now = DateTime.Now;
                var yctnNew = new YeuCauTiepNhan
                {
                    LoaiYeuCauTiepNhan = Enums.EnumLoaiYeuCauTiepNhan.DangKyGoiMarketing,
                    TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien,
                    MaYeuCauTiepNhan = ResourceHelper.CreateMaYeuCauTiepNhan(),
                    BenhNhanId = benhNhanId,
                    ThoiDiemTiepNhan = now,
                    ThoiDiemCapNhatTrangThai = now,
                    HoTen = benhNhan.HoTen,
                    NgaySinh = benhNhan.NgaySinh,
                    ThangSinh = benhNhan.ThangSinh,
                    NamSinh = benhNhan.NamSinh,
                    SoChungMinhThu = benhNhan.SoChungMinhThu,
                    GioiTinh = benhNhan.GioiTinh,
                    NhomMau = benhNhan.NhomMau,
                    NgheNghiepId = benhNhan.NgheNghiepId,
                    NoiLamViec = benhNhan.NoiLamViec,
                    QuocTichId = benhNhan.QuocTichId,
                    DanTocId = benhNhan.DanTocId,
                    DiaChi = benhNhan.DiaChi,
                    PhuongXaId = benhNhan.PhuongXaId,
                    QuanHuyenId = benhNhan.QuanHuyenId,
                    TinhThanhId = benhNhan.TinhThanhId,
                    SoDienThoai = benhNhan.SoDienThoai,
                    Email = benhNhan.Email,
                };

                await _tiepNhanBenhNhanService.AddAsync(yctnNew);
            }


            return NoContent();
        }

        private async Task<YeuCauTiepNhan> GetYCTNDangThucHienOfBenhNhan(long benhNhanId)
        {
            var yc = await _danhMucMarketingService.GetYCTNDangThucHienOfBenhNhan(benhNhanId);
            return yc;
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> Update
            ([FromBody] ThongTinGoiMarketingViewModel model)
        {
            if (model.LstDaChon == null || !model.LstDaChon.Any())
            {
                throw new ApiException(_localizationService.GetResource("DanhMucMarketing.ThongTinGoiMarketing.Required"));
            }
            if (!string.IsNullOrEmpty(model.HoTen))
            {
                model.HoTen = model.HoTen.ToUpper();
            }
            if (model.BenhNhanId == null || model.BenhNhanId == 0)
            {
                throw new ApiException(_localizationService.GetResource("DanhMucMarketing.BenhNhan.Required"));
            }

            //if (!await _danhMucMarketingService.CheckDichVuValidate(model.LstDaChon))
            //{
            //    throw new ApiException(_localizationService.GetResource("DanhMucMarketing.DichVuValidate.Required"));
            //}

            var entity = await _benhNhanService.GetByIdAsync(model.BenhNhanId ?? 0, p => p.Include(x => x.YeuCauGoiDichVus).ThenInclude(x => x.ChuongTrinhGoiDichVu));

            foreach (var idModel in model.LstDaChon)
            {
                var checksoLuongChonTrongGoi = model.LstDaChon.Where(p => p == idModel).Count();
                var checkSoLuongDaTaoGoi = entity.YeuCauGoiDichVus.Where(p => p.ChuongTrinhGoiDichVuId == idModel).Count();
                if (checksoLuongChonTrongGoi != checkSoLuongDaTaoGoi)
                {
                    //add YeuCauGoiDichVu
                    var ct = await _danhMucMarketingService.GetChuongTrinhGoiDichVu(idModel);

                    var yc = new YeuCauGoiDichVu();
                    //yc.BenhNhanId = model.BenhNhanId ?? 0;
                    yc.ChuongTrinhGoiDichVuId = idModel;
                    yc.MaChuongTrinh = ct.Ma;
                    yc.TenChuongTrinh = ct.Ten;
                    yc.GiaTruocChietKhau = ct.GiaTruocChietKhau;
                    yc.GiaSauChietKhau = ct.GiaSauChietKhau;
                    yc.TenGoiDichVu = ct.TenGoiDichVu;
                    yc.MoTaGoiDichVu = ct.MoTaGoiDichVu;
                    yc.GoiSoSinh = ct.GoiSoSinh;
                    yc.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                    yc.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    yc.ThoiDiemChiDinh = DateTime.Now;
                    yc.BoPhanMarketingDangKy = true;
                    yc.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien;
                    yc.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;

                    entity.YeuCauGoiDichVus.Add(yc);
                }
            }

            foreach (var yc in entity.YeuCauGoiDichVus)
            {
                if (!model.LstDaChon.Contains(yc.ChuongTrinhGoiDichVuId))
                {
                    yc.WillDelete = true;
                }
            }

            //update benh nhan
            entity.HoTen = model.HoTen;
            entity.NamSinh = model.NamSinh;
            if (model.NgayThangNamSinh != null)
            {
                entity.NgaySinh = model.NgayThangNamSinh.GetValueOrDefault().Day;
                entity.ThangSinh = model.NgayThangNamSinh.GetValueOrDefault().Month;
                entity.NamSinh = model.NgayThangNamSinh.GetValueOrDefault().Year;
            }
            entity.SoDienThoai = model.SoDienThoai;
            entity.GioiTinh = model.GioiTinh;
            entity.NgheNghiepId = model.NgheNghiepId;
            entity.QuocTichId = model.QuocTichId;
            entity.TinhThanhId = model.TinhThanhId;
            entity.QuanHuyenId = model.QuanHuyenId;
            entity.PhuongXaId = model.PhuongXaId;
            entity.DiaChi = model.DiaChi;
            entity.SoChungMinhThu = model.SoChungMinhThu;
            entity.Email = model.Email;
            entity.NoiLamViec = model.NoiLamViec;
            entity.DanTocId = model.DanTocId;

            await _benhNhanService.UpdateAsync(entity);

            return NoContent();
        }

        #region export
        [HttpPost("ExportDanhSachMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhSachMarketing)]
        public async Task<ActionResult> ExportDanhSachMarketing(QueryInfo queryInfo)
        {
            var gridData = await _danhMucMarketingService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (DanhSachMarketingGridVo)p).ToList();
            var dataExcel = data.Map<List<DanhMucMarketingExportExcel>>();

            foreach (var item in dataExcel)
            {
                var gridChildData = await _danhMucMarketingService.GetDataForGridChildAsync(queryInfo, item.Id, true);
                var dataChild = gridChildData.Data.Select(p => (DanhSachMarketingChildGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DanhMucMarketingExportExcelChild>>();
                item.DanhMucMarketingExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DanhMucMarketingExportExcel.MaBenhNhan), "Mã BN"));
            lstValueObject.Add((nameof(DanhMucMarketingExportExcel.TenBenhNhan), "Người Bệnh"));
            lstValueObject.Add((nameof(DanhMucMarketingExportExcel.NamSinh), "Năm Sinh"));
            lstValueObject.Add((nameof(DanhMucMarketingExportExcel.GioiTinh), "Giới Tính"));
            lstValueObject.Add((nameof(DanhMucMarketingExportExcel.DienThoaiDisplay), "Điện Thoại"));
            lstValueObject.Add((nameof(DanhMucMarketingExportExcel.ChungMinhThu), "CMND"));
            lstValueObject.Add((nameof(DanhMucMarketingExportExcel.DiaChi), "Địa Chỉ"));
            //lstValueObject.Add((nameof(DanhMucMarketingExportExcel.NgayTaoDisplay), "Ngày Tạo"));
            lstValueObject.Add((nameof(DanhMucMarketingExportExcel.DanhMucMarketingExportExcelChild), ""));


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Đăng ký gói marketing", 2, "Đăng ký gói marketing");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DangKyGoiMarketing" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        //[HttpPost("GetListGoiMarketing")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachMarketing)]
        //public async Task<ActionResult> GetListGoiMarketing(long id)
        //{
        //    var lstChuongTrinhGoiDichVu = await _danhMucMarketingService.GetListChuongTrinhGoiCurrently();
        //    var result = new List<ThongTinGoiMarketingViewModel>();
        //    if (lstChuongTrinhGoiDichVu != null)
        //    {
        //        foreach (var item in lstChuongTrinhGoiDichVu)
        //        {
        //            var goi = new ThongTinGoiMarketingViewModel();
        //            goi.TenGoi = item.Ten;
        //            goi.Id = item.Id;

        //            goi.GiaTruocChietKhau = item.GiaTruocChietKhau;
        //            goi.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();

        //            goi.GiaSauChietKhau = item.GiaSauChietKhau;
        //            goi.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();

        //            goi.TiLeChietKhau = item.TiLeChietKhau;
        //            goi.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber();

        //            if (item.ChuongTrinhGoiDichVuDichKhamBenhs.Any())
        //            {
        //                foreach (var dichVu in item.ChuongTrinhGoiDichVuDichKhamBenhs)
        //                {
        //                    var dichVuAdd = new DanhSachDichVuTrongGoi { 
        //                        Id = dichVu.Id,
        //                        Ma = dichVu.DichVuKhamBenhBenhVien.Ma,
        //                        TenDichVu = dichVu.DichVuKhamBenhBenhVien.Ten,
        //                        LoaiGiaDisplay = dichVu.NhomGiaDichVuKhamBenhBenhVien.Ten,
        //                        SoLuong = dichVu.SoLan,
        //                        SoLuongDisplay = dichVu.SoLan.ApplyNumber(),
        //                        DonGia = dichVu.DonGia,
        //                        DonGiaDisplay = dichVu.DonGia.ApplyFormatMoneyVND(),
        //                        ThanhTien = dichVu.DonGia * dichVu.SoLan,
        //                        ThanhTienDisplay = (dichVu.DonGia * dichVu.SoLan).ApplyFormatMoneyVND(),
        //                    };

        //                }
        //            }
        //            if (item.ChuongTrinhGoiDichVuDichVuKyThuats.Any())
        //            {
        //                foreach (var dichVu in item.ChuongTrinhGoiDichVuDichVuKyThuats)
        //                {

        //                }
        //            }
        //            if (item.ChuongTrinhGoiDichVuDichVuGiuongs.Any())
        //            {
        //                foreach (var dichVu in item.ChuongTrinhGoiDichVuDichVuGiuongs)
        //                {

        //                }
        //            }
        //        }
        //    }
        //    return Ok(result);
        //}

        #region [PHÁT SINH TRIỂN KHAI][THU NGÂN] YÊU CẦU HIỂN THỊ CẢNH BÁO KHI THÊM NB CÓ TÊN, NGÀY SINH VÀ SĐT GIỐNG NHAU
        [HttpPost("KiemTraBenhNhanTrongHeThong")]
        public async Task<ActionResult<KiemTraNguoiBenhTrongHeThongVo>> KiemTraBenhNhanTrongHeThongAsync([FromBody]ThongTinGoiMarketingViewModel model)
        {
            KiemTraNguoiBenhTrongHeThongVo kiemTraVo = new KiemTraNguoiBenhTrongHeThongVo()
            {
                HoTen = model.HoTen,
                NgayThangNamSinh = model.NgayThangNamSinh,
                NamSinh = model.NamSinh,
                SoDienThoai = model.SoDienThoai
            };
            var kiemTra = await _tiepNhanBenhNhanService.KiemTraBenhNhanTrongHeThongAsync(kiemTraVo);
            return kiemTra;
        }

        #endregion
    }
}
