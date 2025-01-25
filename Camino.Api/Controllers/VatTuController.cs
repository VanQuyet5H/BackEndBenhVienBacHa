using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Services.Localization;
using System.Linq;
using System;
using Camino.Services.VatTu;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.VatTu;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Api.Models.VatTu;
using Camino.Api.Models.Error;
using Camino.Core.Domain.Entities.VatTus;
using Camino.Api.Extensions;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Microsoft.EntityFrameworkCore;
using Camino.Services.VatTuBenhViens;
using Camino.Core.Helpers;
using System.Net;
using Camino.Api.Models.VatTuBenhViens;
using Camino.Services.TaiLieuDinhKem;
using OfficeOpenXml;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class VatTuController : CaminoBaseController
    {
        private readonly IVatTuService _vatTuYTeService;
        private readonly IVatTuBenhVienService _vatTuBenhVienService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;

        public VatTuController(IVatTuService vatTuYTeService, IVatTuBenhVienService vatTuBenhVienService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService, ITaiLieuDinhKemService taiLieuDinhKemService)
        {
            _vatTuYTeService = vatTuYTeService;
            _localizationService = localizationService;
            _excelService = excelService;
            _vatTuBenhVienService = vatTuBenhVienService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVatTuYTe)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _vatTuYTeService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVatTuYTe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _vatTuYTeService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("KichHoatVatTuYTe")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucVatTuYTe)]
        public async Task<ActionResult> KichHoatVatTuYTe(long id)
        {
            var entity = await _vatTuYTeService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _vatTuYTeService.UpdateAsync(entity);
            return NoContent();
        }
        #region xử lý vật tư
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucVatTuYTe)]
        public async Task<ActionResult<VatTuViewModel>> PostAsync([FromBody] VatTuViewModel vatTuViewModel)
        {
            var vatTuBenhVien = vatTuViewModel.ToEntity<VatTu>();
            if (vatTuViewModel.SuDungVatTuBenhVien == true && vatTuViewModel.IsDisabled == false) // false là vat tu k dc tao
            {
                var vatTuBenhVienobj = new VatTuBenhVien
                {
                    DieuKienBaoHiemThanhToan = vatTuViewModel.DieuKienBaoHiemThanhToan,
                    HieuLuc = vatTuViewModel.IsDisabled == false ? true : false,
                    LoaiSuDung = vatTuViewModel.LoaiSuDung,
                    MaVatTuBenhVien = vatTuViewModel.MaVatTuBenhVien,
                };
                vatTuBenhVien.VatTuBenhVien = vatTuBenhVienobj;
            }
            //else
            //{
            //    throw new ArgumentException(_localizationService
            //        .GetResource("VatTu.VatTuBenhVienNotCreate"));
            //}
            _vatTuYTeService.Add(vatTuBenhVien);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVatTuYTe)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<VatTuViewModel>> Get(long id)
        {
            var result = await _vatTuYTeService.GetByIdAsync(id, s => s.Include(k => k.VatTuBenhVien).Include(p=>p.NhomVatTu));
            //var result = await _vatTuYTeService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var r = result.ToModel<VatTuViewModel>();
            r.NhomVatTuModelText = result.NhomVatTu.Ten;
            r.IsDisabled = result.IsDisabled ?? false;
            if (result.VatTuBenhVien != null)
            {
                r.SuDungVatTuBenhVien = true;
                r.LoaiSuDung = result.VatTuBenhVien.LoaiSuDung;
                r.LoaiSuDungText = result.VatTuBenhVien.LoaiSuDung.GetDescription();
                r.DieuKienBaoHiemThanhToan = result.VatTuBenhVien.DieuKienBaoHiemThanhToan;
                r.MaVatTuBenhVien = result.VatTuBenhVien.Ma;

                //BVHD-3472
                r.VatTuBenhViewModel = new VatTuBenhViewModel();
                r.VatTuBenhViewModel.Id = result.VatTuBenhVien.Id;
                r.ChuaTaoVatTuBenhVien = false;
            }
            return Ok(r);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucVatTuYTe)]
        public async Task<ActionResult> Put([FromBody] VatTuViewModel vatTuViewModel)
        {
            var vatTuBenhVien = await _vatTuYTeService.GetByIdAsync(vatTuViewModel.Id, s => s.Include(l => l.VatTuBenhVien));
            if (vatTuBenhVien == null)
            {
                return NotFound();
            }

            vatTuViewModel.ToEntity(vatTuBenhVien);
            if (vatTuViewModel.SuDungVatTuBenhVien == true && vatTuViewModel.IsDisabled == false && vatTuBenhVien.VatTuBenhVien != null) // false là vat tu k dc tao
            {
                vatTuBenhVien.VatTuBenhVien.LoaiSuDung = vatTuViewModel.LoaiSuDung;
                vatTuBenhVien.VatTuBenhVien.MaVatTuBenhVien = vatTuViewModel.MaVatTuBenhVien;
                vatTuBenhVien.VatTuBenhVien.DieuKienBaoHiemThanhToan = vatTuViewModel.DieuKienBaoHiemThanhToan;
            }
            if (vatTuViewModel.SuDungVatTuBenhVien == true && vatTuViewModel.IsDisabled == false && vatTuBenhVien.VatTuBenhVien == null) // false là vat tu k dc tao
            {
                var vatTuBenhVienobj = new VatTuBenhVien
                {
                    DieuKienBaoHiemThanhToan = vatTuViewModel.DieuKienBaoHiemThanhToan,
                    HieuLuc = vatTuViewModel.IsDisabled == false ? true : false,
                    LoaiSuDung = vatTuViewModel.LoaiSuDung,
                    MaVatTuBenhVien = vatTuViewModel.MaVatTuBenhVien,
                };
                vatTuBenhVien.VatTuBenhVien = vatTuBenhVienobj;
            }
            if (vatTuViewModel.SuDungVatTuBenhVien == false)
            {
                if(vatTuBenhVien.VatTuBenhVien != null)
                {
                    await _vatTuBenhVienService.DeleteByIdAsync(vatTuBenhVien.VatTuBenhVien.Id);
                }
            }
            await _vatTuYTeService.UpdateAsync(vatTuBenhVien);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucVatTuYTe)]
        public async Task<ActionResult> Delete(long id)
        {
            var thuocBenhVien = await _vatTuYTeService.GetByIdAsync(id);
            if (thuocBenhVien == null)
            {
                return NotFound();
            }

            await _vatTuYTeService.DeleteByIdAsync(id);
            return NoContent();
        }

        //[HttpPost("Deletes")]
        //[ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDonViTinh)]
        //public async Task<ActionResult> Deletes([FromBody] VatTuViewModel model)
        //{
        //    var thuocBenhViens = await _vatTuYTeService.GetByIdsAsync(model.Ids);
        //    if (thuocBenhViens == null)
        //    {
        //        return NotFound();
        //    }
        //    if (thuocBenhViens.Count() != model.Ids.Length)
        //    {
        //        throw new ArgumentException(_localizationService
        //            .GetResource("Common.WrongLengthMultiDelete"));
        //    }
        //    await _vatTuYTeService.DeleteAsync(thuocBenhViens);
        //    return NoContent();
        //}
        #endregion
        [HttpPost("ExportVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucVatTuYTe)]
        public async Task<ActionResult> ExportVatTu(QueryInfo queryInfo)
        {
            var gridData = await _vatTuYTeService.GetDataForGridAsync(queryInfo, true);
            var vatTuData = gridData.Data.Select(p => (VatTuGridVo)p).ToList();
            var excelData = vatTuData.Map<List<VatTuExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(VatTuExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(VatTuExportExcel.Ten), "Tên đầy đủ"));
            lstValueObject.Add((nameof(VatTuExportExcel.TenNhomVatTu), "Tên nhóm vật tư"));
            lstValueObject.Add((nameof(VatTuExportExcel.TyLeBaoHiemThanhToan), "Tỷ lệ bảo hiểm thanh toán"));
            lstValueObject.Add((nameof(VatTuExportExcel.TenDonViTinh), "Đơn vị sơ cấp"));           
            lstValueObject.Add((nameof(VatTuExportExcel.QuyCach), "Đơn vị thứ cấp"));
            lstValueObject.Add((nameof(VatTuExportExcel.HeSoDinhMucDonViTinh), "Hệ số định mức"));
            lstValueObject.Add((nameof(VatTuExportExcel.NhaSanXuat), "Nhà sản xuất"));
            lstValueObject.Add((nameof(VatTuExportExcel.NuocSanXuat), "Nước sản xuất"));
            lstValueObject.Add((nameof(VatTuExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(VatTuExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Vật tư");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=VatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #region update 8/2/2021
        [HttpPost("GetListNhomVatTuAsync")]
        public async Task<ActionResult<ICollection<NhomVatTuTreeViewVo>>> GetListNhomVatTuAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _vatTuYTeService.GetListNhomVatTuAsync(model);
            return Ok(lookup);
        }
        #endregion

        #region import
        [HttpGet("ImportVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucVatTuYTe)]
        public ActionResult<bool>ImportVatTu(string tenGuid, string duongDan)
        {
            List<VatTuViewModel> listVatTuModel = new List<VatTuViewModel>();
            List<DanhSachNhapVatTuExcelError> listError = new List<DanhSachNhapVatTuExcelError>();
            var path = _taiLieuDinhKemService.GetObjectStream(duongDan,tenGuid);
            using(var package = new ExcelPackage(path))
            {
                var workSheet = package.Workbook.Worksheets["Vật tư"];
                if(workSheet == null)
                {
                    throw new ApiException("Thông tin vật tư không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;
                if(totalRows >= 2)
                {
                    for(int i = 2; i <= totalRows; i++)
                    {
                        DanhSachNhapVatTuExcelError errors = new DanhSachNhapVatTuExcelError();
                        var vatTuModel = new VatTuViewModel();

                        if(workSheet.Cells[i,1].Value == null
                            || workSheet.Cells[i, 2].Value == null
                            || workSheet.Cells[i, 3].Value == null
                            || workSheet.Cells[i, 4].Value == null)
                        {
                            var infoMa = workSheet.Cells[i, 1].Value == null ? _localizationService.GetResource("Common.Ma.Required") + "|" : "";
                            var infoTen = workSheet.Cells[i, 2].Value == null ? _localizationService.GetResource("Common.Ten.Required") + "|" : "";
                            var infoNhomVatTu = workSheet.Cells[i, 3].Value == null ? _localizationService.GetResource("VatTu.NhomVatTuId.Required") + "|" : "";
                            var infoTyLeBaoHiem = workSheet.Cells[i, 4].Value == null ? _localizationService.GetResource("VatTu.TyLeBaoHiemThanhToan.Required") + "|" : "";
                            
                            listError.Add(new DanhSachNhapVatTuExcelError
                            {
                                Ten = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : "") + "-"
                                            + (workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : ""),
                                Error = infoMa + infoTen + infoNhomVatTu + infoTyLeBaoHiem
                            });
                            continue;
                        }
                        var suDungVatTuBV = workSheet.Cells[i, 12].Value != null ? (workSheet.Cells[i, 12].Value.ToString().Trim().TrimStart().TrimEnd().ToUpper() == ("Có").ToUpper() ? true : false) : false;
                        var hieuLuc = workSheet.Cells[i, 10].Value != null ? (workSheet.Cells[i, 10].Value.ToString().Trim().TrimStart().TrimEnd().ToUpper() == ("Có").ToUpper() ? false : true) : true;
                        if (suDungVatTuBV == true)
                        {
                            if(workSheet.Cells[i, 13].Value == null
                            || workSheet.Cells[i, 14].Value == null)
                            {
                                var infoMaVatTuBV = workSheet.Cells[i, 13].Value == null ? _localizationService.GetResource("VatTu.MaVatTuBenhVien.Required") + "|" : "";
                                var infoLoaiSuDung = workSheet.Cells[i, 14].Value == null ? _localizationService.GetResource("VatTu.LoaiSuDung.Required") + "|" : "";

                                listError.Add(new DanhSachNhapVatTuExcelError
                                {
                                    Ten = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : "") + "-"
                                            + (workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : ""),
                                    Error = infoMaVatTuBV + infoLoaiSuDung
                                });
                                continue;
                            }
                        }

                        var nhomVatTu = GetDanhSachNhomVatTu(workSheet.Cells[i, 3].Value.ToString());
                        if (nhomVatTu == null)
                        {
                            listError.Add(new DanhSachNhapVatTuExcelError
                            {
                                Ten = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : "") + "-"
                                            + (workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : ""),
                                Error = "Nhóm vật tư không tồn tại"
                            });
                            continue;
                        }

                        vatTuModel.Ma = workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : null;
                        vatTuModel.Ten = workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : null;
                        vatTuModel.NhomVatTuId = nhomVatTu != null ? nhomVatTu.KeyId : 0;
                        vatTuModel.TyLeBaoHiemThanhToan = int.Parse(workSheet.Cells[i, 4].Value.ToString());
                        vatTuModel.QuyCach = workSheet.Cells[i, 5].Value != null ? workSheet.Cells[i, 5].Value.ToString() : null;
                        vatTuModel.DonViTinh = workSheet.Cells[i, 6].Value != null ? workSheet.Cells[i, 6].Value.ToString() : null;
                        vatTuModel.HeSoDinhMucDonViTinh = workSheet.Cells[i, 7].Value != null ? int.Parse(workSheet.Cells[i, 7].Value.ToString()) : (int?)null;
                        vatTuModel.NhaSanXuat = workSheet.Cells[i, 8].Value != null ? workSheet.Cells[i, 8].Value.ToString() : null;
                        vatTuModel.NuocSanXuat = workSheet.Cells[i, 9].Value != null ? workSheet.Cells[i, 9].Value.ToString() : null;
                        vatTuModel.IsDisabled = hieuLuc;
                        vatTuModel.MoTa = workSheet.Cells[i, 11].Value != null ? workSheet.Cells[i, 11].Value.ToString() : null;
                        vatTuModel.SuDungVatTuBenhVien = suDungVatTuBV;
                        vatTuModel.MaVatTuBenhVien = workSheet.Cells[i, 13].Value != null ? workSheet.Cells[i, 13].Value.ToString() : null;
                        vatTuModel.LoaiSuDung = workSheet.Cells[i, 14].Value != null ? GetLoaiSuDung(workSheet.Cells[i, 14].Value.ToString()): (LoaiSuDung?)null;
                        vatTuModel.DieuKienBaoHiemThanhToan = workSheet.Cells[i, 15].Value != null ? workSheet.Cells[i, 15].Value.ToString() : null;

                        listVatTuModel.Add(vatTuModel);


                    }
                }
                else
                {
                    throw new ApiException("Thông tin vật tư yêu cầu nhập");
                }
                
            }
            foreach (var item in listVatTuModel)
            {
                var vatTu = item.ToEntity<VatTu>();
                if (item.SuDungVatTuBenhVien == true && item.IsDisabled == false)
                {
                    var vatTuBV = new VatTuBenhVien
                    {
                        MaVatTuBenhVien = item.MaVatTuBenhVien,
                        LoaiSuDung = item.LoaiSuDung,
                        DieuKienBaoHiemThanhToan = item.DieuKienBaoHiemThanhToan,
                        HieuLuc = item.IsDisabled
                    };
                    vatTu.VatTuBenhVien = vatTuBV;
                }
                _vatTuYTeService.Add(vatTu);
            }
            if (listError.Any())
                listError[0].TotalThanhCong = listVatTuModel.Count;
            else
                listError.Add(new DanhSachNhapVatTuExcelError { TotalThanhCong = listVatTuModel.Count });

            return Ok(listError);

        }
        
            
        private LoaiSuDung GetLoaiSuDung(string tenLoaiSuDung)
        {
            if (tenLoaiSuDung == LoaiSuDung.VatTuTieuHao.GetDescription())
                return LoaiSuDung.VatTuTieuHao;
            else if (tenLoaiSuDung == LoaiSuDung.VatTuThayThe.GetDescription())
                return LoaiSuDung.VatTuThayThe;
            return LoaiSuDung.VatTuThayThe;
        }

        private NhomVatTuTreeViewVo GetDanhSachNhomVatTu(string nhomVatTu)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = nhomVatTu;
            requestModel.Take = 10;
            return _vatTuYTeService.GetListNhomVatTuAsync(requestModel).Result.FirstOrDefault();

        }
        #endregion
    }
}