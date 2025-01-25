using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.GayBenhAn;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhAnDienTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhAnDienTus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
namespace Camino.Api.Controllers
{
    public partial class BenhAnDienTuController
    {
        [HttpPost("GetDataForGridAsyncDanhSachGayBenhAn")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhMucGayBenhAn)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachGayBenhAn([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _benhAnDienTuService.GetDataForGridAsyncDanhSachGayBenhAn(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncDanhSachGayBenhAn")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhMucGayBenhAn)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachGayBenhAn([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _benhAnDienTuService.GetTotalPageForGridAsyncDanhSachGayBenhAn(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetPhieuHoSoGayBenhAnLookupVos")]
        public async Task<ActionResult<ICollection<MauVaChePhamTemplateVo>>> GetPhieuHoSoGayBenhAnLookupVos(DropDownListRequestModel model)
        {
            var lookup = await _benhAnDienTuService.GetPhieuHoSoGayBenhAnLookupVos(model);
            return Ok(lookup);
        }

        #region Get/Add/Delete/Update ==>> DanhMucGayBenhAn
        [HttpPost("TaoGayBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucGayBenhAn)]
        public async Task<ActionResult> Post([FromBody] GayBenhAnViewModel gayBenhAnViewModel)
        {
            var phieuHoSoGayBenhAns = new List<KeyIdStringPhieuHoSoGayBenhAnLookupVo>();
            foreach (var item in gayBenhAnViewModel.GayBenhAnPhieuHoSoIds)
            {
                var phieuHoSoGayBenhAn = JsonConvert.DeserializeObject<KeyIdStringPhieuHoSoGayBenhAnLookupVo>(item);
                phieuHoSoGayBenhAns.Add(phieuHoSoGayBenhAn);
            }
            var model = gayBenhAnViewModel.ToEntity<GayBenhAn>();
            foreach (var item in phieuHoSoGayBenhAns)
            {
                if (item.LoaiPhieuHoSoBenhAn == PhieuHoSoBenhAn.NhomCLS)
                {
                    var gayBenhAnPhieuHoSo = new GayBenhAnPhieuHoSo
                    {
                        LoaiPhieuHoSoBenhAnDienTu = LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS,
                        Value = item.PhieuHoSoId
                    };
                    model.GayBenhAnPhieuHoSos.Add(gayBenhAnPhieuHoSo);
                }
                else if (item.LoaiPhieuHoSoBenhAn == PhieuHoSoBenhAn.HoSoKhac)
                {
                    var gayBenhAnPhieuHoSo = new GayBenhAnPhieuHoSo
                    {
                        LoaiPhieuHoSoBenhAnDienTu = LoaiPhieuHoSoBenhAnDienTu.HoSoKhac,
                        Value = item.PhieuHoSoId
                    };
                    model.GayBenhAnPhieuHoSos.Add(gayBenhAnPhieuHoSo);
                }
                else
                {
                    var gayBenhAnPhieuHoSo = new GayBenhAnPhieuHoSo
                    {
                        LoaiPhieuHoSoBenhAnDienTu = (LoaiPhieuHoSoBenhAnDienTu)item.PhieuHoSoId,
                    };
                    model.GayBenhAnPhieuHoSos.Add(gayBenhAnPhieuHoSo);
                }
            }
            await _benhAnDienTuService.AddAsync(model);
            return Ok();
        }
        //Get
        [HttpGet("GetGayBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucGayBenhAn)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GayBenhAnViewModel>> GettGayBenhAn(long id)
        {
            var model = await _benhAnDienTuService.GetByIdAsync(id, s => s.Include(u => u.GayBenhAnPhieuHoSos));
            if (model == null)
            {
                return NotFound();
            }
            var viewModel = model.ToModel<GayBenhAnViewModel>();
            foreach (var item in model.GayBenhAnPhieuHoSos)
            {
                if (item.LoaiPhieuHoSoBenhAnDienTu == LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS)
                {
                    viewModel.GayBenhAnPhieuHoSoIds.Add(JsonConvert.SerializeObject(new KeyIdStringPhieuHoSoGayBenhAnLookupVo()
                    {
                        PhieuHoSoId = item.Value.GetValueOrDefault(),
                        LoaiPhieuHoSoBenhAn = PhieuHoSoBenhAn.NhomCLS,
                    }));
                }
                else if (item.LoaiPhieuHoSoBenhAnDienTu == LoaiPhieuHoSoBenhAnDienTu.HoSoKhac)
                {
                    viewModel.GayBenhAnPhieuHoSoIds.Add(JsonConvert.SerializeObject(new KeyIdStringPhieuHoSoGayBenhAnLookupVo()
                    {
                        PhieuHoSoId = item.Value.GetValueOrDefault(),
                        LoaiPhieuHoSoBenhAn = PhieuHoSoBenhAn.HoSoKhac,
                    }));
                }
                else
                {
                    viewModel.GayBenhAnPhieuHoSoIds.Add(JsonConvert.SerializeObject(new KeyIdStringPhieuHoSoGayBenhAnLookupVo()
                    {
                        PhieuHoSoId = (long)item.LoaiPhieuHoSoBenhAnDienTu,
                        LoaiPhieuHoSoBenhAn = PhieuHoSoBenhAn.PhieuHoSoBenhAnDienTu,
                    }));
                }
            }
            return Ok(viewModel);
        }
        //Update
        [HttpPost("CapNhatGayBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucGayBenhAn)]
        public async Task<ActionResult> Put([FromBody] GayBenhAnViewModel gayBenhAnViewModel)
        {
            var model = await _benhAnDienTuService.GetByIdAsync(gayBenhAnViewModel.Id, s => s.Include(u => u.GayBenhAnPhieuHoSos));
            if (model == null)
            {
                return NotFound();
            }
            gayBenhAnViewModel.ToEntity(model);
            var phieuHoSoGayBenhAns = new List<KeyIdStringPhieuHoSoGayBenhAnLookupVo>();
            foreach (var item in model.GayBenhAnPhieuHoSos)
            {
                item.WillDelete = true;
            }
            foreach (var item in gayBenhAnViewModel.GayBenhAnPhieuHoSoIds)
            {
                var phieuHoSoGayBenhAn = JsonConvert.DeserializeObject<KeyIdStringPhieuHoSoGayBenhAnLookupVo>(item);
                phieuHoSoGayBenhAns.Add(phieuHoSoGayBenhAn);
            }
            foreach (var item in phieuHoSoGayBenhAns)
            {
                if (item.LoaiPhieuHoSoBenhAn == PhieuHoSoBenhAn.NhomCLS)
                {
                    var gayBenhAnPhieuHoSo = new GayBenhAnPhieuHoSo
                    {
                        LoaiPhieuHoSoBenhAnDienTu = LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS,
                        Value = item.PhieuHoSoId
                    };
                    model.GayBenhAnPhieuHoSos.Add(gayBenhAnPhieuHoSo);
                }
                else if (item.LoaiPhieuHoSoBenhAn == PhieuHoSoBenhAn.HoSoKhac)
                {
                    var gayBenhAnPhieuHoSo = new GayBenhAnPhieuHoSo
                    {
                        LoaiPhieuHoSoBenhAnDienTu = LoaiPhieuHoSoBenhAnDienTu.HoSoKhac,
                        Value = item.PhieuHoSoId
                    };
                    model.GayBenhAnPhieuHoSos.Add(gayBenhAnPhieuHoSo);
                }
                else
                {
                    var gayBenhAnPhieuHoSo = new GayBenhAnPhieuHoSo
                    {
                        LoaiPhieuHoSoBenhAnDienTu = (LoaiPhieuHoSoBenhAnDienTu)item.PhieuHoSoId,
                    };
                    model.GayBenhAnPhieuHoSos.Add(gayBenhAnPhieuHoSo);
                }
            }
            await _benhAnDienTuService.UpdateAsync(model);
            return Ok();
        }
        //Delete
        [HttpPost("XoaGayBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucGayBenhAn)]
        public async Task<ActionResult> Delete(long id)
        {
            var adr = await _benhAnDienTuService.GetByIdAsync(id, s => s.Include(u => u.GayBenhAnPhieuHoSos));
            if (adr == null)
            {
                return NotFound();
            }
            await _benhAnDienTuService.DeleteByIdAsync(id);
            return NoContent();
        }

        //Delete all selected items
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucGayBenhAn)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var adrs = await _benhAnDienTuService.GetByIdsAsync(model.Ids, s => s.Include(u => u.GayBenhAnPhieuHoSos));
            if (adrs == null)
            {
                return NotFound();
            }
            if (adrs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _benhAnDienTuService.DeleteAsync(adrs);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportGayBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucGayBenhAn)]
        public async Task<ActionResult> ExportGayBenhAn(QueryInfo queryInfo)
        {
            var gridData = await _benhAnDienTuService.GetDataForGridAsyncDanhSachGayBenhAn(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (GayBenhAnVo)p).ToList();
            var excelData = chucVuData.Map<List<GayBenhAnExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(GayBenhAnExportExcel.Ma), "Mã"),
                (nameof(GayBenhAnExportExcel.ViTri), "Vị trí gáy"),
                (nameof(GayBenhAnExportExcel.Ten), "Tên"),
                (nameof(GayBenhAnExportExcel.TenPhieuHoSo), "Phiếu/ hồ sơ"),
                (nameof(GayBenhAnExportExcel.TenTrangThai), "Trạng thái")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Gáy bệnh án");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GayBenhAn" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
