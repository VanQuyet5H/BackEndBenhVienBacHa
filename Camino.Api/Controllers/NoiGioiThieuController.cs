using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.NguoiGioiThieu;
using Camino.Services.NoiGioiThieu;
using Camino.Api.Models.NoiGioiThieu;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Services.ExportImport;
using Camino.Core.Domain.ValueObject.NoiGioiThieu;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.Helpers;
using Camino.Core.Domain.Entities.DonViMaus;
using Camino.Services.DonViMaus;
using Camino.Services.TaiLieuDinhKem;

namespace Camino.Api.Controllers
{
    public class NoiGioiThieuController : CaminoBaseController
    {
        private readonly INoiGioiThieuService _noiGioiThieuService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IDonViMauService _donViMauService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;

        public NoiGioiThieuController(INoiGioiThieuService noiGioiThieuService,
            ILocalizationService localizationService,
            IDonViMauService donViMauService,
            IJwtFactory iJwtFactory, IExcelService excelService
            , ITaiLieuDinhKemService taiLieuDinhKemService
            )
        {
            _noiGioiThieuService = noiGioiThieuService;
            _localizationService = localizationService;
            _excelService = excelService;
            _donViMauService = donViMauService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
        }

        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _noiGioiThieuService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _noiGioiThieuService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDonViMauAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDonViMauAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _noiGioiThieuService.GetDataForGridDonViMauAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDonViMauAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDonViMauAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _noiGioiThieuService.GetTotalPageForGridDonViMauAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChiTietMienGiam")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChiTietMienGiamAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _noiGioiThieuService.GetDataForGridChiTietMienGiamAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChiTietMienGiam")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChiTietMienGiamAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _noiGioiThieuService.GetTotalPageForGridChiTietMienGiamAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        [HttpPost("GetNguoiQuanLy")]
        public async Task<ActionResult<ICollection<NguoiQuanLyTemplateVo>>> GetPhongKham(DropDownListRequestModel model)
        {
            var lookup = await _noiGioiThieuService.GetNguoiQuanLyListAsync(model);
            return Ok(lookup);
        }

        [HttpPost("DonViMaus")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> DonViMaus([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _noiGioiThieuService.GetDonViMaus(queryInfo);
            return Ok(lookup);
        }

        #region Get/Add/Delete/Update ==>> NoiGioiThieu
        //Add
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> Post([FromBody] NoiGioiThieuViewModel noiGioiThieuViewModel)
        {
            var model = noiGioiThieuViewModel.ToEntity<NoiGioiThieu>();
            
            #region //BVHD-3936: kiểm tra trùng dịch vụ
            if (model.NoiGioiThieuChiTietMienGiams.Any())
            {
                var lstDichVuTrung = model.NoiGioiThieuChiTietMienGiams
                    .GroupBy(x => new
                    {
                        x.DichVuKhamBenhBenhVienId,
                        x.DichVuKyThuatBenhVienId,
                        x.DichVuGiuongBenhVienId,
                        x.NhomGiaDichVuKhamBenhBenhVienId,
                        x.NhomGiaDichVuKyThuatBenhVienId,
                        x.NhomGiaDichVuGiuongBenhVienId,
                        x.DuocPhamBenhVienId,
                        x.VatTuBenhVienId
                    })
                    .Where(x => x.Count() > 1)
                    .ToList();
                if (lstDichVuTrung.Any())
                {
                    throw new Exception(_localizationService.GetResource("NoiGioiThieuChiTietMienGiam.DichVuKhuyenMai.IsExists"));
                }
            }
            #endregion

            await _noiGioiThieuService.AddAsync(model);
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model.ToModel<NoiGioiThieuViewModel>());
        }
        //Get
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNoiGioiThieu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NoiGioiThieuViewModel>> Get(long id)
        {
            var model = await _noiGioiThieuService.GetByIdAsync(id, s => s.Include(u => u.NhanVienQuanLy).ThenInclude(u => u.User)
                                                                          .Include(u => u.DonViMau));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToModel<NoiGioiThieuViewModel>());
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> Put([FromBody] NoiGioiThieuViewModel noiGioiThieuViewModel)
        {
            var model = await _noiGioiThieuService.GetByIdAsync(noiGioiThieuViewModel.Id);
            if (model == null)
            {
                return NotFound();
            }
            noiGioiThieuViewModel.ToEntity(model);
            await _noiGioiThieuService.UpdateAsync(model);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> Delete(long id)
        {
            var noiGioiThieu = await _noiGioiThieuService.GetByIdAsync(id, x => x.Include(a => a.NoiGioiThieuChiTietMienGiams));
            if (noiGioiThieu == null)
            {
                return NotFound();
            }
            await _noiGioiThieuService.DeleteAsync(noiGioiThieu);
            return NoContent();
        }

        //Delete all selected items
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var adrs = await _noiGioiThieuService.GetByIdsAsync(model.Ids);
            if (adrs == null)
            {
                return NotFound();
            }
            if (adrs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _noiGioiThieuService.DeleteAsync(adrs);
            return NoContent();
        }
        #endregion


        #region Get/Add/Delete/Update ==>> DonViMau
        //Add
        [HttpPost("ThemDonViMau")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> ThemDonViMau(DonViMauViewModel donViMauViewModel)
        {
            var model = donViMauViewModel.ToEntity<DonViMau>();
            await _donViMauService.AddAsync(model);
            return Ok();
        }
        //Get
        [HttpGet("GetDonViMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNoiGioiThieu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DonViMauViewModel>> GetDonViMau(long id)
        {
            var model = await _donViMauService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToModel<DonViMauViewModel>());
        }
        //Update
        [HttpPost("CapNhatDonViMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> CapNhatDonViMau(DonViMauViewModel donViMauViewModel)
        {
            var model = await _donViMauService.GetByIdAsync(donViMauViewModel.Id, s => s.Include(c => c.NoiGioiThieus));
            if (model == null)
            {
                return NotFound();
            }
            foreach (var item in model.NoiGioiThieus)
            {
                item.DonVi = donViMauViewModel.Ten;
            }
            donViMauViewModel.ToEntity(model);
            await _donViMauService.UpdateAsync(model);
            return Ok();
        }
        //Delete
        [HttpPost("XoaDonViMau")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> XoaDonViMau(long id)
        {
            var model = await _donViMauService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            await _donViMauService.DeleteByIdAsync(id);
            return NoContent();
        }
        #endregion

        [HttpPost("KichHoatNoiGioiThieu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> KichHoatNoiGioiThieu(long id)
        {
            var entity = await _noiGioiThieuService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _noiGioiThieuService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportNoiGioiThieu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> ExportNoiGioiThieu(QueryInfo queryInfo)
        {
            var gridData = await _noiGioiThieuService.GetDataForGridAsync(queryInfo, true);
            var noiGioiThieuData = gridData.Data.Select(p => (NoiGioiThieuGridVo)p).ToList();
            var excelData = noiGioiThieuData.Map<List<NoiGioiThieuExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(NoiGioiThieuExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(NoiGioiThieuExportExcel.DonVi), "Đơn vị"));
            lstValueObject.Add((nameof(NoiGioiThieuExportExcel.SoDienThoai), "Số điện thoại"));
            lstValueObject.Add((nameof(NoiGioiThieuExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(NoiGioiThieuExportExcel.IsDisabled), "Trạng thái"));
            lstValueObject.Add((nameof(NoiGioiThieuExportExcel.HoTenNguoiQuanLy), "Người quản lý"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nơi giới thiệu");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NoiGioiThieu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #region BVHD-3882
        [HttpPost("GetDonGia")]
        public async Task<ActionResult<ThongTinGiaVo>> GetDonGia(ThongTinGiaVo thongTinDichVu)
        {
            await _noiGioiThieuService.GetDonGia(thongTinDichVu);
            return Ok(thongTinDichVu);
        }

        [HttpGet("GetDanhSachMienGiamDichVu")]
        public async Task<ActionResult> GetDanhSachMienGiamDichVu(long noiGioiThieuId)
        {
            var queryInfo = new QueryInfo()
            {
                AdditionalSearchString = noiGioiThieuId.ToString(),
                SearchString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("<AdvancedQueryParameters><SearchTerms></SearchTerms></AdvancedQueryParameters>"))
            };
            var gridData = await _noiGioiThieuService.GetDataForGridChiTietMienGiamAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDichVuKyThuat")]
        public async Task<ActionResult> GetDichVuKyThuat([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _noiGioiThieuService.GetDichVuKyThuat(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuGiuong")]
        public async Task<ActionResult> GetDichVuGiuong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _noiGioiThieuService.GetDichVuGiuong(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuKham")]
        public async Task<ActionResult> GetDichVuKham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _noiGioiThieuService.GetDichVuKham(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDuocPham")]
        public async Task<ActionResult> GetDuocPham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _noiGioiThieuService.GetDuocPham(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetVatTu")]
        public async Task<ActionResult> GetVatTu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _noiGioiThieuService.GetVatTu(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("XuLyThemMienGiamDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult<NoiGioiThieuChiTietMienGiamViewModel>> XuLyThemMienGiamDichVuAsync(NoiGioiThieuChiTietMienGiamViewModel thongTinMienGiam)
        {
            if (thongTinMienGiam.NoiGioiThieuId != null)
            {
                var noiGioiThieuChiTietMienGiam = thongTinMienGiam.ToEntity(new NoiGioiThieuChiTietMienGiam());
                await _noiGioiThieuService.XuLyThemMienGiamDichVuAsync(noiGioiThieuChiTietMienGiam);
            }
            else
            {
                var noiGioiThieuChiTietMienGiam = thongTinMienGiam.ToEntity(new NoiGioiThieuChiTietMienGiam());
                thongTinMienGiam.TenNhomGia = await _noiGioiThieuService.GetTenNhomGiaTheoLoaiDichVuAsync(noiGioiThieuChiTietMienGiam);
            }
            return Ok(thongTinMienGiam);
        }

        [HttpPut("XuLyCapNhatMienGiamDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult<NoiGioiThieuChiTietMienGiamViewModel>> XuLyCapNhatMienGiamDichVuAsync(NoiGioiThieuChiTietMienGiamViewModel thongTinMienGiam)
        {
            if (thongTinMienGiam.Id != 0)
            {
                var mienGiamDichVu = await _noiGioiThieuService.XuLyGetMienGiamDichVuAsync(thongTinMienGiam.Id);
                thongTinMienGiam.ToEntity(mienGiamDichVu);
                await _noiGioiThieuService.XuLyCapNhatMienGiamDichVuAsync(mienGiamDichVu);
            }
            return thongTinMienGiam;
        }

        [HttpDelete("XuLyXoaMienGiamDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> DanhMucNoiGioiThieu(long id)
        {
            await _noiGioiThieuService.XuLyXoaMienGiamDichVuAsync(id);
            return NoContent();
        }
        #endregion

        #region BVHD-3936
        [HttpPost("DownloadFileExcelTemplateDichVuMienGiamTheoNoiGioiThieu")]
        public ActionResult DownloadFileExcelTemplateDichVuMienGiamTheoNoiGioiThieu()
        {
            var path = @"Resource\\DichVuMienGiamTheoNoiGioiThieu.xlsx";
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);

            long byteLength = new System.IO.FileInfo(path).Length;
            var fileContent = binaryReader.ReadBytes((Int32)byteLength);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DichVuMienGiamTheoNoiGioiThieu.xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(fileContent, "application/vnd.ms-excel");
        }

        [HttpPost("ImportDichVuMienGiamTheoNoiGioiThieu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> ImportDichVuMienGiamTheoNoiGioiThieu(NoiGioiThieuFileImportVo model)
        {
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            model.Path = path;
            var result = await _noiGioiThieuService.XuLyKiemTraDataDichVuMienGiamImportAsync(model);
            return Ok(result);
        }

        [HttpPost("KiemTraDichVuLoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> KiemTraDichVuLoi(DichVuCanKiemTraVo info)
        {
            var result = new NoiGioiThieuDataImportVo();
            await _noiGioiThieuService.KiemTraDataDichVuMienGiamImportAsync(info.datas, result, info.NoiGioiThieuId);
            return Ok(result);
        }

        [HttpPost("XuLyLuuDichVuMienGiamImport")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> XuLyLuuDichVuMienGiamImport(DichVuCanKiemTraVo info)
        {
            var result = await _noiGioiThieuService.XuLyLuuDichVuMienGiamImportAsync(info.datas, info.NoiGioiThieuId);
            return Ok(result);
        }
        #endregion
    }
}