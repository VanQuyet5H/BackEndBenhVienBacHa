using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.HopDongThauDuocPham;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.HopDongThauDuocPhamService;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Models.DuocPhamBenhVien;
using Camino.Api.Models.Error;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.HopDongThauDuocPham;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.DuocPhamBenhVien;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Helpers;

namespace Camino.Api.Controllers
{
    public class HopDongThauDuocPhamController : CaminoBaseController
    {
        private readonly IHopDongThauDuocPhamService _hopDongThauDuocPhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IDuocPhamBenhVienService _duocPhamBenhVienService;
        public HopDongThauDuocPhamController(IHopDongThauDuocPhamService hopDongThauDuocPhamService, IExcelService excelService, ILocalizationService localizationService, IDuocPhamBenhVienService duocPhamBenhVienService)
        {
            _hopDongThauDuocPhamService = hopDongThauDuocPhamService;
            _excelService = excelService;
            _localizationService = localizationService;
            _duocPhamBenhVienService = duocPhamBenhVienService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauDuocPhamService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauDuocPhamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauDuocPhamService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauDuocPhamService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region GetTemplateListVo

        [HttpPost("GetListDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult> GetListDuocPham([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _hopDongThauDuocPhamService.GetListDuocPham(model);
            return Ok(lookup);
        }

        [HttpPost("GetListNhaThau")]
        public async Task<ActionResult> GetListNhaThau([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _hopDongThauDuocPhamService.GetListNhaThau(model);
            return Ok(lookup);
        }

        [HttpPost("GetListLoaiThau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauDuocPham, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public ActionResult<ICollection<Enums.EnumLoaiThau>> GetListLoaiThau([FromBody]LookupQueryInfo model)
        {
            var listEnum = _hopDongThauDuocPhamService.GetListLoaiThau(model);
            return Ok(listEnum);
        }

        [HttpPost("GetListLoaiThuocThau")]
        public ActionResult<ICollection<Enums.EnumLoaiThuocThau>> GetListLoaiThuocThau([FromBody]LookupQueryInfo model)
        {
            var listEnum = _hopDongThauDuocPhamService.GetListLoaiThuocThau(model);
            return Ok(listEnum);
        }

        [HttpPost("GetListTenHopDongThau")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListTenHopDongThau(DropDownListRequestModel model)
        {
            var lookup = await _hopDongThauDuocPhamService.GetListTenHopDongThau(model);
            return Ok(lookup);
        }

        [HttpPost("GetHieuLucDuocPham")]
        public async Task<ActionResult> GetHieuLucDuocPham(long id)
        {
            var hieuLuc = await _hopDongThauDuocPhamService.GetHieuLucDuocPham(id);
            return Ok(hieuLuc);
        }

        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult<HopDongThauDuocPhamViewModel>> Post
            ([FromBody]HopDongThauDuocPhamViewModel hopDongThauDuocPhamViewModel)
        {
            if (hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets.Count <= 0 || hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets[0].DuocPham == null)
            {
                throw new ApiException(_localizationService.GetResource("HopDongThauDuocPhamChiTiet.Count.Required"), (int)HttpStatusCode.BadRequest);
            }

            var hopDongThauDuocPham = hopDongThauDuocPhamViewModel.ToEntity<HopDongThauDuocPham>();
            if (hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets.Where(s => s.SuDungTaiBenhVien == true).Any())
            {
                foreach (var itemHopDongVTCT in hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets.Where(s => s.SuDungTaiBenhVien == true).ToList())
                {
                    var kiemTraVatTuBenhVienDaTaoChua = await _hopDongThauDuocPhamService.CheckDuocPhamBenhVienExist((long)itemHopDongVTCT.DuocPhamId);
                    if (kiemTraVatTuBenhVienDaTaoChua == false)
                    {
                        var vatTuBenhVien = new DuocPhamBenhVien
                        {
                            Id = (long)itemHopDongVTCT.DuocPhamId,
                            HieuLuc = true, // luôn luôn true
                            MaDuocPhamBenhVien = itemHopDongVTCT.MaDuocPhamBenhVien,
                            //DuocPhamBenhVienPhanNhomId = itemHopDongVTCT.DuocPhamBenhVienPhanNhomId

                            // BVHD-3454
                            DuocPhamBenhVienPhanNhomId = itemHopDongVTCT.DuocPhamBenhVienPhanNhomId
                        };
                        await _duocPhamBenhVienService.AddAsync(vatTuBenhVien);
                    }
                }
            }
            await _hopDongThauDuocPhamService.AddAsync(hopDongThauDuocPham);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<HopDongThauDuocPhamViewModel>> Get(long id)
        {
            var hopDongThauDuocPham = await _hopDongThauDuocPhamService.GetByIdAsync(
                id,
                s => s.Include(k => k.NhaThau).Include(k => k.HopDongThauDuocPhamChiTiets).ThenInclude(h => h.DuocPham).ThenInclude(v=>v.DuocPhamBenhVien).ThenInclude(g=>g.DuocPhamBenhVienPhanNhom)
                    .Include(w => w.NhapKhoDuocPhamChiTiets)
                );
            if (hopDongThauDuocPham == null)
            {
                return NotFound();
            }
            var hopDongThauDuocPhamReSult = hopDongThauDuocPham.ToModel<HopDongThauDuocPhamViewModel>();
            var duocPhamBenhVienPhanNhoms = await _duocPhamBenhVienService.DuocPhamBenhVienPhanNhoms();
            if (hopDongThauDuocPhamReSult.HopDongThauDuocPhamChiTiets.Any())
            {
                foreach (var itemHDVTCT in hopDongThauDuocPhamReSult.HopDongThauDuocPhamChiTiets)
                {
                    var chiTiet = hopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Where(s => s.DuocPhamId == itemHDVTCT.DuocPhamId).FirstOrDefault();
                    
                    itemHDVTCT.MaDuocPhamBenhVien = chiTiet.DuocPham?.DuocPhamBenhVien?.MaDuocPhamBenhVien;
                    //itemHDVTCT.DuocPhamBenhVienPhanNhomId = chiTiet.DuocPham?.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhomId;
                    //itemHDVTCT.DuocPhamBenhVienPhanNhomModelText = chiTiet.DuocPham?.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhom?.Ten;
                    itemHDVTCT.SuDungTaiBenhVien = chiTiet.DuocPham?.DuocPhamBenhVien != null ? true : false;

                    // BVHD-3454
                    itemHDVTCT.DuocPhamBenhVienId = chiTiet?.DuocPham?.DuocPhamBenhVien?.Id;
                    itemHDVTCT.DuocPhamBenhVienPhanNhomId = chiTiet?.DuocPham?.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhomId;
                    if (itemHDVTCT.DuocPhamBenhVienPhanNhomId != null)
                    {
                        var ladpbvNhomCon = await _duocPhamBenhVienService.LaDuocPhamBenhVienPhanNhomCon(itemHDVTCT.DuocPhamBenhVienPhanNhomId.Value);
                        if (ladpbvNhomCon)
                        {
                            itemHDVTCT.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(itemHDVTCT.DuocPhamBenhVienPhanNhomId.Value, duocPhamBenhVienPhanNhoms);
                            itemHDVTCT.TenDuocPhamBenhVienPhanNhomCha = await _duocPhamBenhVienService.GetTenDuocPhamBenhVienPhanNhom(itemHDVTCT.DuocPhamBenhVienPhanNhomChaId.Value);
                            itemHDVTCT.DuocPhamBenhVienPhanNhomConId = itemHDVTCT.DuocPhamBenhVienPhanNhomId.Value;
                            itemHDVTCT.TenDuocPhamBenhVienPhanNhomCon = chiTiet.DuocPham?.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhom?.Ten;
                        }
                        else
                        {
                            itemHDVTCT.DuocPhamBenhVienPhanNhomChaId = itemHDVTCT.DuocPhamBenhVienPhanNhomId.Value;
                            itemHDVTCT.TenDuocPhamBenhVienPhanNhomCha = chiTiet.DuocPham?.DuocPhamBenhVien?.DuocPhamBenhVienPhanNhom?.Ten;
                            itemHDVTCT.DuocPhamBenhVienPhanNhomConId = null;
                            itemHDVTCT.TenDuocPhamBenhVienPhanNhomCon = null;
                        }
                    }

                }
            }
            return Ok(hopDongThauDuocPhamReSult);
        }

        [HttpPost("CoNhapKho")]
        public async Task<ActionResult> CoNhapKho(long id, long idDuocPham)
        {
            var hopDongThauDuocPham = await _hopDongThauDuocPhamService.GetByIdAsync(id,
                s => s.Include(y => y.NhapKhoDuocPhamChiTiets));

            foreach (var nhapKhoChiTiet in hopDongThauDuocPham.NhapKhoDuocPhamChiTiets)
            {
                if (nhapKhoChiTiet.DuocPhamBenhVienId == idDuocPham)
                {
                    return Ok(false);
                }
            }

            return Ok(true);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatHopDongThauDuocPham([FromBody]HopDongThauDuocPhamViewModel hopDongThauDuocPhamViewModel)
        {
            if (hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets.Count <= 0 || hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets[0].DuocPham == null)
            {
                throw new ApiException(_localizationService.GetResource("HopDongThauDuocPhamChiTiet.Count.Required"), (int)HttpStatusCode.BadRequest);
            }

            foreach (var duocPhamChiTiet in hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets)
            {
                var conDuocPham = await _hopDongThauDuocPhamService.KiemTraConDuocPham(duocPhamChiTiet.DuocPhamId.Value);

                if (conDuocPham != true)
                {
                    throw new ApiException(_localizationService.GetResource("HopDongThauDuocPhamChiTiet.AvailableMedicine.DeleteError"), (int)HttpStatusCode.BadRequest);
                }
            }

            var hopDongThauDuocPham = await _hopDongThauDuocPhamService.GetByIdAsync(hopDongThauDuocPhamViewModel.Id,
                s => s.Include(k => k.HopDongThauDuocPhamChiTiets));
            if (hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets.Where(s => s.SuDungTaiBenhVien == true).Any())
            {
                foreach (var itemHopDongVTCT in hopDongThauDuocPhamViewModel.HopDongThauDuocPhamChiTiets.Where(s => s.SuDungTaiBenhVien == true).ToList())
                {
                    var kiemTraVatTuBenhVienDaTaoChua = await _hopDongThauDuocPhamService.CheckDuocPhamBenhVienExist((long)itemHopDongVTCT.DuocPhamId);
                    if (kiemTraVatTuBenhVienDaTaoChua == false)
                    {
                        var vatTuBenhVien = new DuocPhamBenhVien
                        {
                            Id = (long)itemHopDongVTCT.DuocPhamId,
                            HieuLuc = true, // luôn luôn true
                            MaDuocPhamBenhVien = itemHopDongVTCT.MaDuocPhamBenhVien,
                            //DuocPhamBenhVienPhanNhomId = itemHopDongVTCT.DuocPhamBenhVienPhanNhomId

                            // BVHD-3454
                            DuocPhamBenhVienPhanNhomId = itemHopDongVTCT.DuocPhamBenhVienPhanNhomId
                        };
                        await _duocPhamBenhVienService.AddAsync(vatTuBenhVien);
                    }
                }
            }
            hopDongThauDuocPhamViewModel.ToEntity(hopDongThauDuocPham);

            await _hopDongThauDuocPhamService.UpdateAsync(hopDongThauDuocPham);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult> Delete(long id)
        {
            var hopDongThauDuocPham = await _hopDongThauDuocPhamService.GetByIdAsync(id, s => s.Include(k => k.HopDongThauDuocPhamChiTiets)
                .Include(k => k.NhapKhoDuocPhamChiTiets));

            if (hopDongThauDuocPham == null)
            {
                return NotFound();
            }

            if (hopDongThauDuocPham.NhapKhoDuocPhamChiTiets.Count != 0)
            {
                throw new ApiException(_localizationService.GetResource("HopDongThauDuocPhamChiTiet.Stocked.DeleteError"), (int)HttpStatusCode.BadRequest);
            }

            await _hopDongThauDuocPhamService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var hopDongThauDuocPhams = await _hopDongThauDuocPhamService.GetByIdsAsync(model.Ids);
            if (hopDongThauDuocPhams == null)
            {
                return NotFound();
            }

            var listHopDongThauDuocPhams = hopDongThauDuocPhams.ToList();

            if (listHopDongThauDuocPhams.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }

            foreach (var hopDongThauDuocPham in listHopDongThauDuocPhams)
            {
                var hopDongThauDuocPhamChiTiet = await _hopDongThauDuocPhamService.GetByIdAsync(hopDongThauDuocPham.Id, s => s.Include(k => k.HopDongThauDuocPhamChiTiets)
                    .Include(k => k.NhapKhoDuocPhamChiTiets));

                if (hopDongThauDuocPhamChiTiet == null)
                {
                    return NotFound();
                }

                if (hopDongThauDuocPhamChiTiet.NhapKhoDuocPhamChiTiets.Count != 0)
                {
                    throw new ApiException(_localizationService.GetResource("HopDongThauDuocPhamChiTiet.Stocked.DeleteError"), (int)HttpStatusCode.BadRequest);
                }

                await _hopDongThauDuocPhamService.DeleteByIdAsync(hopDongThauDuocPham.Id);
            }

            return NoContent();
        }

        [HttpPost("ExportHopDongThauDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult> ExportHopDongThauDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauDuocPhamService.GetDataForGridAsync(queryInfo, true);
            var hopDongThauDuocPhamData = gridData.Data.Select(p => (HopDongThauDuocPhamGridVo)p).ToList();
            var dataExcel = hopDongThauDuocPhamData.Map<List<HopDongThauDuocPhamExportExcel>>();

            foreach (var item in dataExcel)
            {
                var gridChildData = await _hopDongThauDuocPhamService.GetDataForGridChildAsync(queryInfo, item.Id, true);
                var dataChild = gridChildData.Data.Select(p => (HopDongThauDuocPhamChiTietGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<HopDongThauDuocPhamExportExcelChild>>();
                item.HopDongThauDuocPhamExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(HopDongThauDuocPhamExportExcel.NhaThau), "Nhà Thầu"),
                (nameof(HopDongThauDuocPhamExportExcel.SoHopDong), "Số Hợp Đồng"),
                (nameof(HopDongThauDuocPhamExportExcel.SoQuyetDinh), "Số Quyết Định"),
                (nameof(HopDongThauDuocPhamExportExcel.CongBoDisplay), "Ngày Công Bố"),
                (nameof(HopDongThauDuocPhamExportExcel.NgayKyDisplay), "Ngày Ký"),
                (nameof(HopDongThauDuocPhamExportExcel.NgayHieuLucDisplay), "Ngày Hiệu Lực"),
                (nameof(HopDongThauDuocPhamExportExcel.NgayHetHanDisplay), "Ngày Hết Hạn"),
                (nameof(HopDongThauDuocPhamExportExcel.TenLoaiThau), "Loại Thầu"),
                (nameof(HopDongThauDuocPhamExportExcel.TenLoaiThuocThau), "Loại Thuốc Thầu"),
                (nameof(HopDongThauDuocPhamExportExcel.NhomThau), "Nhóm Thầu"),
                (nameof(HopDongThauDuocPhamExportExcel.GoiThau), "Gói Thầu"),
                (nameof(HopDongThauDuocPhamExportExcel.Nam), "Năm"),
                (nameof(HopDongThauDuocPhamExportExcel.HopDongThauDuocPhamExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Hợp Đồng Thầu Dược Phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=HopDongThauDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ValidateHopDongThauDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> ValidateHopDongThauDuocPham(HopDongThauDuocPhamViewModel model)
        {

            return Ok();
        }

        #region //BVHD-3454
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("KiemTraHopDongThauDuocPhamChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauDuocPham)]
        public async Task<ActionResult> KiemTraHopDongThauDuocPhamChiTiet(HopDongThauDuocPhamChiTietViewModel model)
        {

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinPhanNhomDuocPhamBenhVien")]
        public async Task<ActionResult<DuocPhamBenhVienViewModel>> GetThongTinPhanNhomDuocPhamBenhVienAsync(long id)
        {

            var result = await _duocPhamBenhVienService.GetByIdAsync(id, s => s.Include(p => p.DuocPhamBenhVienPhanNhom));
            if (result == null)
            {
                return NotFound();
            }
            var duocPhamBenhVienPhanNhoms = await _duocPhamBenhVienService.DuocPhamBenhVienPhanNhoms();
            var resultData = new DuocPhamBenhVienViewModel()
            {
                Id = result.Id,
                DuocPhamBenhVienPhanNhomId = result.DuocPhamBenhVienPhanNhomId
            };

            if (result.DuocPhamBenhVienPhanNhomId != null)
            {
                var ladpbvNhomCon = await _duocPhamBenhVienService.LaDuocPhamBenhVienPhanNhomCon(result.DuocPhamBenhVienPhanNhomId.Value);
                if (ladpbvNhomCon)
                {
                    resultData.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(result.DuocPhamBenhVienPhanNhomId.Value, duocPhamBenhVienPhanNhoms);
                    resultData.TenDuocPhamBenhVienPhanNhomCha = await _duocPhamBenhVienService.GetTenDuocPhamBenhVienPhanNhom(resultData.DuocPhamBenhVienPhanNhomChaId.Value);
                    resultData.DuocPhamBenhVienPhanNhomConId = result.DuocPhamBenhVienPhanNhomId.Value;
                    resultData.TenDuocPhamBenhVienPhanNhomCon = await _duocPhamBenhVienService.GetTenDuocPhamBenhVienPhanNhom(resultData.DuocPhamBenhVienPhanNhomConId.Value);
                }
                else
                {
                    resultData.DuocPhamBenhVienPhanNhomChaId = result.DuocPhamBenhVienPhanNhomId.Value;
                    resultData.TenDuocPhamBenhVienPhanNhomCha = result.DuocPhamBenhVienPhanNhom?.Ten;
                    resultData.DuocPhamBenhVienPhanNhomConId = null;
                    resultData.TenDuocPhamBenhVienPhanNhomCon = null;
                }
            }
            return Ok(resultData);
        }
        #endregion
    }
}