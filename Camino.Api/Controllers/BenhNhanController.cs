using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhNhans;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.BenhNhans;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class BenhNhanController : CaminoBaseController
    {
        readonly IBenhNhanService _benhNhanService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public BenhNhanController(
            IBenhNhanService benhNhanService,
            ILocalizationService localizationService,
            IExcelService excelService
            )
        {
            _benhNhanService = benhNhanService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
         ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _benhNhanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _benhNhanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.BenhNhan)]
        public async Task<ActionResult<QuanLyBenhNhanViewModel>> Post([FromBody]QuanLyBenhNhanViewModel QuanLyBenhNhanViewModel)
        {
            var benhNhan = QuanLyBenhNhanViewModel.ToEntity<BenhNhan>();
            if (QuanLyBenhNhanViewModel.BenhNhanCongTyBaoHiemTuNhans.Any())
            {
                benhNhan.CoBHTN = true;
            }
            await _benhNhanService.AddAsync(benhNhan);
            var benhNhanId = await _benhNhanService.GetByIdAsync(benhNhan.Id);
            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = benhNhan.Id },
                benhNhanId.ToModel<QuanLyBenhNhanViewModel>()
            );
        }
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BenhNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<QuanLyBenhNhanViewModel>> Get(long id)
        {
            var benhnhan = await _benhNhanService.GetByIdAsync(id, s => s.Include(r => r.QuocTich)
                                                                         .Include(i => i.NguoiLienHeTinhThanh)
                                                                         .Include(i => i.NgheNghiep)
                                                                         .Include(i => i.BenhNhanDiUngThuocs)
                                                                         .Include(i => i.BenhNhanTienSuBenhs)
                                                                         .Include(i => i.BenhNhanCongTyBaoHiemTuNhans).ThenInclude(o => o.CongTyBaoHiemTuNhan)
                                                                         .Include(i => i.NguoiLienHeQuanHeNhanThan));
            if (benhnhan == null)
            {
                return NotFound();
            }
            if (benhnhan.NgaySinh == 0 || benhnhan.NgaySinh == null)
            {
                benhnhan.NgaySinh = null;
            }
            if (benhnhan.ThangSinh == 0 || benhnhan.ThangSinh == null)
            {
                benhnhan.ThangSinh = null;
            }
            var result = benhnhan.ToModel<QuanLyBenhNhanViewModel>();

            foreach (var item in result.BenhNhanCongTyBaoHiemTuNhans)
            {
                item.CongTyDisplay = item.CongTyBaoHiemTuNhan?.Ten;
                item.NgayHieuLucDisplay = item.NgayHieuLuc == null ? "" : item.NgayHieuLuc.Value.ApplyFormatDate();
                item.NgayHetHanDisplay = item.NgayHetHan == null ? "" : item.NgayHetHan.Value.ApplyFormatDate();
            }
            foreach (var item in result.BenhNhanTienSuBenhs)
            {
                item.TenLoaiTienSuBenh = item.LoaiTienSuBenh.GetDescription();
            }
            foreach (var item in result.BenhNhanDiUngThuocs)
            {
                item.LoaiDiUngDisplay = item.LoaiDiUng.GetDescription();
                item.MucDoDisplay = item.MucDo.GetDescription();
                item.TenThuoc = item.TenDiUng;
            }

            return Ok(result);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.BenhNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Put([FromBody]QuanLyBenhNhanViewModel viewModel)
        {
            var benhNhan = await _benhNhanService.GetByIdAsync(viewModel.Id, s => s.Include(i => i.QuocTich)
                                                                          .Include(i => i.NguoiLienHeTinhThanh)
                                                                          .Include(i => i.NgheNghiep)
                                                                          .Include(i => i.BenhNhanDiUngThuocs)
                                                                          .Include(i => i.BenhNhanTienSuBenhs)
                                                                          .Include(i => i.BenhNhanCongTyBaoHiemTuNhans).ThenInclude(o => o.CongTyBaoHiemTuNhan)
                                                                          .Include(i => i.NguoiLienHeQuanHeNhanThan));
            if (benhNhan == null)
            {
                return NotFound();
            }
            if (viewModel.BenhNhanCongTyBaoHiemTuNhans.Any())
            {
                viewModel.CoBHTN = true;
            }
            else
            {
                viewModel.CoBHTN = null;
            }
            viewModel.ToEntity(benhNhan);
            await _benhNhanService.UpdateAsync(benhNhan);
            return NoContent();
        }


        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.BenhNhan)]
        public async Task<ActionResult> Delete(long id)
        {
            var benhnhan = await _benhNhanService.GetByIdAsync(id, s => s.Include(i => i.BenhNhanCongTyBaoHiemTuNhans)
                                                                          .Include(i => i.BenhNhanDiUngThuocs)
                                                                          .Include(i => i.BenhNhanTienSuBenhs));
            if (benhnhan == null)
            {
                return NotFound();
            }

            await _benhNhanService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.BenhNhan)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var benhnhans = await _benhNhanService.GetByIdsAsync(model.Ids, s => s.Include(i => i.BenhNhanCongTyBaoHiemTuNhans)
                                                                          .Include(i => i.BenhNhanDiUngThuocs)
                                                                          .Include(i => i.BenhNhanTienSuBenhs));
            if (benhnhans == null)
            {
                return NotFound();
            }
            if (benhnhans.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _benhNhanService.DeleteAsync(benhnhans);
            return NoContent();
        }


        [HttpPost("ThemBHTN")]
        public async Task<ActionResult> ThemThongTinBHTN(BenhNhanBaoHiemTuNhansViewModel viewModel)
        {
            var model = new ThemBHTN
            {
                BenhNhanId = viewModel.BenhNhanId.Value,
                CongTyBaoHiemTuNhanId = viewModel.CongTyBaoHiemTuNhanId,
                MaSoThe = viewModel.MaSoThe,
                NgayHieuLuc = viewModel.NgayHieuLuc,
                NgayHetHan = viewModel.NgayHetHan,
                SoDienThoai = viewModel.SoDienThoai,
                DiaChi = viewModel.DiaChi
            };
            var result = await _benhNhanService.ThemBHTN(model);
            return Ok(result);
        }


        [HttpPost("ThemTienSuBenh")]
        public async Task<ActionResult> ThemTienSuBenh(BenhNhanTienSuBenhsViewModel viewModel)
        {
            var model = new ThemTienSuBenh
            {
                BenhNhanId = viewModel.BenhNhanId.Value,
                BenhNhanTienSuBenhId = viewModel.BenhNhanTienSuBenhId,
                TenBenh = viewModel.TenBenh,
                LoaiTienSuBenh = viewModel.LoaiTienSuBenh,
            };
            var result = await _benhNhanService.ThemTienSuBenh(model);
            return Ok(result);
        }

        [HttpPost("ThemDiUngThuoc")]
        public async Task<ActionResult> ThemDiUngThuoc(BenhNhanDiUngThuocsViewModel viewModel)
        {
            var model = new ThemDiUngThuoc
            {
                BenhNhanId = viewModel.BenhNhanId.Value,
                BenhNhanDiUngId = viewModel.BenhNhanDiUngId,
                TenDiUng = viewModel.TenDiUng,
                LoaiDiUng = viewModel.LoaiDiUng,
                BieuHienDiUng = viewModel.BieuHienDiUng,
                ThuocId = viewModel.ThuocId,
                TenThuoc = viewModel.TenThuoc,
                MucDo = viewModel.MucDo,

            };
            var result = await _benhNhanService.ThemDiUngThuoc(model);
            return Ok(result);
        }

        [HttpPost("GetLoaiTienSuBenh")]
        public ActionResult<ICollection<LookupItemVo>> GetLoaiTienSuBenhs()
        {
            var lookup = _benhNhanService.GetLoaiTienSuBenhs();
            return Ok(lookup);
        }

        [HttpPost("TenThuocDiUng")]
        public string TenThuocDiUng(long thuocId)
        {
            var result = _benhNhanService.TenThuocDiUng(thuocId);
            return result;
        }
        #region InTheBenhNhanBenhNhan
        [HttpGet("InTheBenhNhanBenhNhan")]
        public ActionResult InTheBenhNhanBenhNhan(long benhNhanId, string hostingName)//InTheBenhNhanBenhNhan
        {
            var result = _benhNhanService.InTheBenhNhanBenhNhan(benhNhanId, hostingName);
            return Ok(result);
        }
        #endregion


        [HttpPost("ExportBenhNhan")]
        public async Task<ActionResult> ExportBenhNhan(QueryInfo queryInfo)
        {
            var gridData = await _benhNhanService.GetDataForGridAsync(queryInfo, true);
            var benhNhanData = gridData.Data.Select(p => (BenhNhanGridVo)p).ToList();
            var dataExcel = benhNhanData.Map<List<BenhNhanExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(BenhNhanExportExcel.HoTen), "Họ tên"),
                (nameof(BenhNhanExportExcel.NamSinh), "Năm sinh"),
                (nameof(BenhNhanExportExcel.SoChungMinhThu), "Chứng minh thư"),
                (nameof(BenhNhanExportExcel.GioiTinh), "Giới tính"),
                (nameof(BenhNhanExportExcel.Email), "Email"),
                (nameof(BenhNhanExportExcel.DiaChi), "Địa chỉ")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Người bệnh", 2, "Người bệnh", true);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BenhNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

    }
}