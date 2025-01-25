using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Api.Models.HopDongThauVatTu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.HopDongThauVatTu;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.HopDongThauVatTuService;
using Camino.Services.Localization;
using Camino.Services.VatTuBenhViens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class HopDongThauVatTuController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IHopDongThauVatTuService _hopDongThauVatTuService;
        private readonly IVatTuBenhVienService _vatTuBenhVienYTeService;

        public HopDongThauVatTuController(
            IExcelService excelService,
            ILocalizationService localizationService,
            IHopDongThauVatTuService hopDongThauVatTuService,
            IVatTuBenhVienService vatTuBenhVienYTeService
            )
        {
            _excelService = excelService;
            _localizationService = localizationService;
            _hopDongThauVatTuService = hopDongThauVatTuService;
            _vatTuBenhVienYTeService = vatTuBenhVienYTeService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauVatTuService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauVatTuService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauVatTuService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauVatTuService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region exportExcel
        [HttpPost("ExportHopDongThauVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult> ExportHopDongThauVatTu(QueryInfo queryInfo)
        {
            var gridData = await _hopDongThauVatTuService.GetDataForGridAsync(queryInfo, true);
            var hopDongThauVatTuData = gridData.Data.Select(p => (HdtVatTuGridVo)p).ToList();
            var dataExcel = hopDongThauVatTuData.Map<List<HopDongThauVatTuExportExcel>>();

            foreach (var item in dataExcel)
            {
                var gridChildData = await _hopDongThauVatTuService.GetDataForGridChildAsync(queryInfo, item.Id, true);
                var dataChild = gridChildData.Data.Select(p => (HdtVatTuChiTietGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<HopDongThauVatTuExportExcelChild>>();
                item.HopDongThauVatTuExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(HopDongThauVatTuExportExcel.NhaThau), "Nhà Thầu"),
                (nameof(HopDongThauVatTuExportExcel.SoHopDong), "Số Hợp Đồng"),
                (nameof(HopDongThauVatTuExportExcel.SoQuyetDinh), "Số Quyết Định"),
                (nameof(HopDongThauVatTuExportExcel.CongBoDisplay), "Ngày Công Bố"),
                (nameof(HopDongThauVatTuExportExcel.NgayKyDisplay), "Ngày Ký"),
                (nameof(HopDongThauVatTuExportExcel.NgayHieuLucDisplay), "Ngày Hiệu Lực"),
                (nameof(HopDongThauVatTuExportExcel.NgayHetHanDisplay), "Ngày Hết Hạn"),
                (nameof(HopDongThauVatTuExportExcel.TenLoaiThau), "Loại Thầu"),
                (nameof(HopDongThauVatTuExportExcel.NhomThau), "Nhóm Thầu"),
                (nameof(HopDongThauVatTuExportExcel.GoiThau), "Gói Thầu"),
                (nameof(HopDongThauVatTuExportExcel.Nam), "Năm"),
                (nameof(HopDongThauVatTuExportExcel.HopDongThauVatTuExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Hợp Đồng Thầu Vật Tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=HopDongThauVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult<HopDongThauVatTuViewModel>> Post
            ([FromBody]HopDongThauVatTuViewModel hopDongThauVatTuViewModel)
        {
            if (!hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets.Any() || hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets[0].VatTuId == null)
            {
                throw new ApiException(_localizationService.GetResource("HopDongThauVatTuChiTiet.Count.Required"), (int)HttpStatusCode.BadRequest);
            }

            var hopDongThauVatTu = hopDongThauVatTuViewModel.ToEntity<HopDongThauVatTu>();
            if (hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets.Where(s => s.SuDungTaiBenhVien == true).Any())
            {
                foreach (var itemHopDongVTCT in hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets.Where(s => s.SuDungTaiBenhVien == true).ToList())
                {
                    var kiemTraVatTuBenhVienDaTaoChua = await _hopDongThauVatTuService.CheckVatTuBenhVienExist((long)itemHopDongVTCT.VatTuId);
                    if (kiemTraVatTuBenhVienDaTaoChua == false)
                    {
                        var vatTuBenhVien = new VatTuBenhVien
                        {
                            Id = (long)itemHopDongVTCT.VatTuId,
                            HieuLuc = true, // luôn luôn true
                            LoaiSuDung = itemHopDongVTCT.LoaiSuDungId,
                            MaVatTuBenhVien = itemHopDongVTCT.MaVatTuBenhVien
                        };
                        await _vatTuBenhVienYTeService.AddAsync(vatTuBenhVien);
                    }
                }
            }
            await _hopDongThauVatTuService.AddAsync(hopDongThauVatTu);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<HopDongThauVatTuViewModel>> Get(long id)
        {
            var hopDongThauVatTu = await _hopDongThauVatTuService.GetByIdAsync(
                id,
                s => s.Include(k => k.NhaThau).Include(k => k.HopDongThauVatTuChiTiets).ThenInclude(h => h.VatTu).ThenInclude(g => g.VatTuBenhVien)
                    .Include(w => w.NhapKhoVatTuChiTiets)
                );
            if (hopDongThauVatTu == null)
            {
                return NotFound();
            }

            var hopDongThauVatTuResult = hopDongThauVatTu.ToModel<HopDongThauVatTuViewModel>();
            if (hopDongThauVatTuResult.HopDongThauVatTuChiTiets.Any())
            {
                foreach (var itemHDVTCT in hopDongThauVatTuResult.HopDongThauVatTuChiTiets)
                {
                    var chiTiet = hopDongThauVatTu.HopDongThauVatTuChiTiets.Where(s => s.VatTuId == itemHDVTCT.VatTuId).FirstOrDefault();
                    itemHDVTCT.MaVatTuBenhVien = chiTiet.VatTu?.VatTuBenhVien?.MaVatTuBenhVien;
                    itemHDVTCT.LoaiSuDungId = chiTiet.VatTu?.VatTuBenhVien?.LoaiSuDung;
                    itemHDVTCT.LoaiSuDungText = chiTiet.VatTu?.VatTuBenhVien?.LoaiSuDung?.GetDescription();
                    itemHDVTCT.SuDungTaiBenhVien = chiTiet.VatTu?.VatTuBenhVien != null ? true : false;
                    itemHDVTCT.SoLuongDaCap = chiTiet.SoLuongDaCap;

                    //BVHD-3472
                    itemHDVTCT.VatTuBenhVienId = chiTiet.VatTu?.VatTuBenhVien?.Id;
                }
            }
            return Ok(hopDongThauVatTuResult);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatHopDongThauVatTu([FromBody]HopDongThauVatTuViewModel hopDongThauVatTuViewModel)
        {
            if (!hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets.Any() || hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets[0].VatTuId == null)
            {
                throw new ApiException(_localizationService.GetResource("HopDongThauVatTuChiTiet.Count.Required"), (int)HttpStatusCode.BadRequest);
            }

            foreach (var vatTuChiTiet in hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets)
            {
                var conVatTu = _hopDongThauVatTuService.KiemTraConVatTu(vatTuChiTiet.VatTuId.GetValueOrDefault());

                if (conVatTu != true)
                {
                    throw new ApiException(_localizationService.GetResource("HopDongThauVatTuChiTiet.AvailableMedicine.DeleteError"), (int)HttpStatusCode.BadRequest);
                }
            }

            var hopDongThauVatTu = await _hopDongThauVatTuService.GetByIdAsync(hopDongThauVatTuViewModel.Id,
                s => s.Include(k => k.HopDongThauVatTuChiTiets));

            if (hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets.Where(s => s.SuDungTaiBenhVien == true).Any())
            {
                foreach (var itemHopDongVTCT in hopDongThauVatTuViewModel.HopDongThauVatTuChiTiets.Where(s => s.SuDungTaiBenhVien == true).ToList())
                {
                    var kiemTraVatTuBenhVienDaTaoChua = await _hopDongThauVatTuService.CheckVatTuBenhVienExist((long)itemHopDongVTCT.VatTuId);
                    if (kiemTraVatTuBenhVienDaTaoChua == false)
                    {
                        var vatTuBenhVien = new VatTuBenhVien
                        {
                            Id = (long)itemHopDongVTCT.VatTuId,
                            HieuLuc = true, // luôn luôn true
                            LoaiSuDung = itemHopDongVTCT.LoaiSuDungId,
                            MaVatTuBenhVien = itemHopDongVTCT.MaVatTuBenhVien
                        };
                        await _vatTuBenhVienYTeService.AddAsync(vatTuBenhVien);
                    }
                }
            }
            hopDongThauVatTuViewModel.ToEntity(hopDongThauVatTu);

            await _hopDongThauVatTuService.UpdateAsync(hopDongThauVatTu);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult> Delete(long id)
        {
            var hopDongThauVatTu = await _hopDongThauVatTuService.GetByIdAsync(id, s => s.Include(k => k.HopDongThauVatTuChiTiets)
                .Include(k => k.NhapKhoVatTuChiTiets));

            if (hopDongThauVatTu == null)
            {
                return NotFound();
            }

            if (hopDongThauVatTu.NhapKhoVatTuChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("HopDongThauVatTuChiTiet.Stocked.DeleteError"), (int)HttpStatusCode.BadRequest);
            }

            await _hopDongThauVatTuService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var hopDongThauVatTus = await _hopDongThauVatTuService.GetByIdsAsync(model.Ids);
            if (hopDongThauVatTus == null)
            {
                return NotFound();
            }

            var listHopDongThauVatTus = hopDongThauVatTus.ToList();

            if (listHopDongThauVatTus.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }

            foreach (var hopDongThauVatTu in listHopDongThauVatTus)
            {
                var hopDongThauVatTuChiTiet = await _hopDongThauVatTuService.GetByIdAsync(hopDongThauVatTu.Id, s => s.Include(k => k.HopDongThauVatTuChiTiets)
                    .Include(k => k.NhapKhoVatTuChiTiets));

                if (hopDongThauVatTuChiTiet == null)
                {
                    return NotFound();
                }

                if (hopDongThauVatTuChiTiet.NhapKhoVatTuChiTiets.Count != 0)
                {
                    throw new ApiException(_localizationService.GetResource("HopDongThauVatTuChiTiet.Stocked.DeleteError"), (int)HttpStatusCode.BadRequest);
                }

                await _hopDongThauVatTuService.DeleteByIdAsync(hopDongThauVatTu.Id);
            }

            return NoContent();
        }
        #endregion

        #region GetListForCmb
        [HttpPost("GetListVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult> GetListVatTu([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _hopDongThauVatTuService.GetVatTus(model);
            return Ok(lookup);
        }

        [HttpPost("GetHieuLucVatTu")]
        public async Task<ActionResult> GetHieuLucVatTu(long id)
        {
            var hieuLuc = await _hopDongThauVatTuService.GetHieuLucVatTu(id);
            return Ok(hieuLuc);
        }
        #endregion

        #region //BVHD-3472
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("KiemTraHopDongThauVatTuChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHopDongThauVatTu)]
        public async Task<ActionResult> KiemTraHopDongThauVatTuChiTiet(HopDongThauVatTuChiTietViewModel model)
        {

            return Ok();
        }
        #endregion
    }
}
