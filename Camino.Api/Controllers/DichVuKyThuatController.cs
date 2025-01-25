using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.DichVuKyThuatBenhVien;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.DichVuKyThuat;
using Camino.Services.ExportImport;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class DichVuKyThuatController : CaminoBaseController
    {
        private readonly IDichVuKyThuatService _dichVuKyThuatService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IYeuCauDichVuKyThuatService _yeuCauDichVuKyThuatService;


        public DichVuKyThuatController(
            IDichVuKyThuatService dichVuKyThuatService,
            IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService,
            IJwtFactory iJwtFactory,
            ILocalizationService localizationService,
            IExcelService excelService,
            ITaiLieuDinhKemService taiLieuDinhKemService)
        {
            _dichVuKyThuatService = dichVuKyThuatService;
            _localizationService = localizationService;
            _excelService = excelService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _yeuCauDichVuKyThuatService = yeuCauDichVuKyThuatService;
        }

        #region Danh sách
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region xử lý

        [HttpPost("NhomDichVuKyThuatBenhVienPhanNhomTreeViews")]
        public async Task<ActionResult<ICollection<NhomDichVuKyThuatBenhVienPhanNhomTreeViewVo>>> NhomDichVuKyThuatBenhVienPhanNhomTreeViews([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _dichVuKyThuatService.NhomDichVuKyThuatBenhVienPhanNhomTreeViews(model);
            return Ok(lookup);
        }

        [HttpPost("GetNhomDVKTs")]
        public List<LookupItemVo> GetNhomDVKTs(DropDownListRequestModel model)
        {
            var lookup = _dichVuKyThuatService.GetNhomDVKTs(model);
            return lookup;
        }

        [HttpPost("GetLoaiPTTTs")]
        public ActionResult GetLoaiPTTT([FromBody]LookupQueryInfo model)
        {
            var listIcd = _yeuCauDichVuKyThuatService.GetLoaiPttt(model);
            return Ok(listIcd);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult> Post([FromBody] DichVuKyThuatViewModel viewModel)
        {
            var dichVuKyThuat = viewModel.ToEntity<DichVuKyThuat>();
            await _dichVuKyThuatService.AddAsync(dichVuKyThuat);
            return CreatedAtAction(nameof(Get), new { id = dichVuKyThuat.Id }, dichVuKyThuat.ToModel<DichVuKyThuatViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuat)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DichVuKyThuatViewModel>> Get(long id)
        {
            var dichVuKyThuat = await _dichVuKyThuatService.GetByIdAsync(id, s => s.Include(o => o.NhomDichVuKyThuat)
                                                                                   .Include(o => o.DichVuKyThuatThongTinGias));
            if (dichVuKyThuat == null)
            {
                return NotFound();
            }
            var result = dichVuKyThuat.ToModel<DichVuKyThuatViewModel>();
            return Ok(result);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult> Put([FromBody] DichVuKyThuatViewModel viewModel)
        {
            var dichVuKyThuat = await _dichVuKyThuatService.GetByIdAsync(viewModel.Id, s => s.Include(o => o.DichVuKyThuatThongTinGias));
            if (dichVuKyThuat == null)
            {
                return NotFound();
            }
            viewModel.ToEntity(dichVuKyThuat);
            await _dichVuKyThuatService.UpdateAsync(dichVuKyThuat);
            return NoContent();
        }

        [HttpPost("XoaDichVuKyThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult> Delete(long id)
        {
            var dichVuKyThuat = await _dichVuKyThuatService.GetByIdAsync(id, s => s.Include(x => x.DichVuKyThuatThongTinGias));
            if (dichVuKyThuat == null)
            {
                return NotFound();
            }

            await _dichVuKyThuatService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var dichVuKyThuats = await _dichVuKyThuatService.GetByIdsAsync(model.Ids, s => s.Include(x => x.DichVuKyThuatThongTinGias));
            if (dichVuKyThuats == null)
            {
                return NotFound();
            }
            if (dichVuKyThuats.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _dichVuKyThuatService.DeleteAsync(dichVuKyThuats);
            return NoContent();
        }


        [HttpPost("KichHoatKyThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public async Task<ActionResult> KichHoatKyThuat(long id)
        {
            var entity = await _dichVuKyThuatService.GetByIdAsync(id);
            //entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _dichVuKyThuatService.UpdateAsync(entity);
            return NoContent();
        }

        #endregion


        #region Export/Import Excel
        [HttpPost("ExportDichVuKyThuat")]
        public async Task<ActionResult> ExportDichVuKyThuat(QueryInfo queryInfo)
        {
            //var gridData = await _dichVuKyThuatService.GetDataForGridAsync(queryInfo, true);
            //var dichVuKyThuatData = gridData.Data.Select(p => (DichVuKyThuatGridVo)p).ToList();
            //var excelData = dichVuKyThuatData.Map<List<DichVuKyThuatExportExcel>>();

            //foreach (var item in excelData)
            //{
            //    var gridChildData = await _dichVuKyThuatService.GetDataForGridChildAsync(queryInfo, item.Id, true);
            //    var childData = gridChildData.Data.Select(p => (DichVuKyThuatThongTinGiaGridVo)p).ToList();
            //    var childExcelData = childData.Map<List<DichVuKyThuatExportExcelChild>>();
            //    item.DichVuKyThuatExportExcelChild.AddRange(childExcelData);
            //}
            var excelData2 = _dichVuKyThuatService.DataGridForExcel(queryInfo);
            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.TenTiengAnh), "Tên tiếng Anh"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.Ma4350), "Mã TT 43/50"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.MaGia), "Mã giá"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.TenGia), "Tên giá"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.TenNhomChiPhi), "Nhóm chi phí"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.TenNhomDichVuKyThuat), "Nhóm dịch vụ kỹ thuật"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.TenLoaiPhauThuatThuThuat), "Loại phẫu thuật thủ thuật"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(DichVuKyThuatExportExcel.DichVuKyThuatExportExcelChild), ""));

            var bytes = _excelService.ExportManagermentView(excelData2, lstValueObject, "Dịch vụ kỹ thuật");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DichVuKyThuat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("DownloadTemplateDichVuKyThuat")]
        public ActionResult DownloadTemplateDichVuKyThuat()
        {
            var path = @"Resource\\TemplateDichVuKyThuat.xlsx";
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);

            long byteLength = new System.IO.FileInfo(path).Length;
            var fileContent = binaryReader.ReadBytes((Int32)byteLength);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TemplateDichVuKyThuat.xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(fileContent, "application/vnd.ms-excel");
        }

        [HttpGet("ImportDichVuKyThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKyThuat)]
        public ActionResult<bool> ImportDichVuKyThuat(string tenGuid, string duongDan)
        {
            List<DichVuKyThuatViewModel> listModel = new List<DichVuKyThuatViewModel>();
            List<DichVuKyThuatViewModelError> listError = new List<DichVuKyThuatViewModelError>();
            var path = _taiLieuDinhKemService.GetObjectStream(duongDan, tenGuid);
            using (var package = new ExcelPackage(path))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["Dịch vụ kỹ thuật"];
                if (workSheet == null)
                {
                    throw new ApiException("Thông tin file dịch vụ kỹ thuật không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;
                if (totalRows >= 2)
                {
                    for (int i = 2; i <= totalRows; i++)
                    {
                        DichVuKyThuatViewModelError dataError = new DichVuKyThuatViewModelError();
                        //var model = new DichVuKyThuatThongTinGiaViewModel();

                        if (workSheet.Cells[i, 1].Value == null
                            || workSheet.Cells[i, 2].Value == null
                            || workSheet.Cells[i, 5].Value == null
                            || workSheet.Cells[i, 7].Value == null
                            || workSheet.Cells[i, 8].Value == null
                            || workSheet.Cells[i, 12].Value == null
                            || workSheet.Cells[i, 13].Value == null
                            || workSheet.Cells[i, 18].Value == null)
                        {
                            var inforMa = workSheet.Cells[i, 1].Value == null ? _localizationService.GetResource("DichVuKyThuat.Ma.Required") + "|" : "";
                            var inforTen = workSheet.Cells[i, 2].Value == null ? _localizationService.GetResource("DichVuKyThuat.TenChung.Required") + "|" : "";
                            var inforMaGia = workSheet.Cells[i, 5].Value == null ? _localizationService.GetResource("DichVuKyThuat.MaGia.Required") + "|" : "";
                            var inforNhomChiPhi = workSheet.Cells[i, 7].Value == null ? _localizationService.GetResource("DichVuKyThuat.NhomChiPhi.Required") + "|" : "";
                            var inforNhomDichVuKyThuat = workSheet.Cells[i, 8].Value == null ? _localizationService.GetResource("DichVuKyThuat.NhomDichVuKyThuatId.Required") + "|" : "";
                            var inforGia = workSheet.Cells[i, 12].Value == null ? _localizationService.GetResource("DichVuKyThuatThongTinGia.Gia.Required") + "|" : "";
                            var inforTuNgay = workSheet.Cells[i, 13].Value == null ? _localizationService.GetResource("DichVuKyThuatThongTinGia.TuNgay.Required") + "|" : "";
                            var inforHieuLuc = workSheet.Cells[i, 18].Value == null ? _localizationService.GetResource("DichVuKyThuatThongTinGia.HieuLuc.Required") + "|" : "";

                            listError.Add(new DichVuKyThuatViewModelError
                            {
                                Ma = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : "") + "-"
                                            + (workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : ""),
                                Error = inforMa + inforTen + inforMaGia + inforNhomChiPhi + inforNhomDichVuKyThuat + inforGia + inforTuNgay
                                            + inforHieuLuc
                            });
                            continue;
                        }

                        var dichVuKyThuatModel = new DichVuKyThuatViewModel();
                        dichVuKyThuatModel.MaChung = workSheet.Cells[i, 1].Value.ToString();
                        dichVuKyThuatModel.TenChung = workSheet.Cells[i, 2].Value.ToString();
                        dichVuKyThuatModel.TenTiengAnh = workSheet.Cells[i, 3].Value != null ? workSheet.Cells[i, 3].Value.ToString() : "";
                        dichVuKyThuatModel.Ma4350 = workSheet.Cells[i, 4].Value != null ? workSheet.Cells[i, 4].Value.ToString() : "";
                        dichVuKyThuatModel.MaGia = workSheet.Cells[i, 5].Value.ToString();
                        dichVuKyThuatModel.TenGia = workSheet.Cells[i, 6].Value != null ? workSheet.Cells[i, 6].Value.ToString() : "";
                        dichVuKyThuatModel.NhomChiPhi = GetNhomChiPhi(workSheet.Cells[i, 7].Value.ToString());
                        dichVuKyThuatModel.NhomDichVuKyThuatId = GetNhomDichVuKyThuat(workSheet.Cells[i, 8].Value.ToString());
                        dichVuKyThuatModel.LoaiPhauThuatThuThuat = workSheet.Cells[i, 9].Value != null ? GetLoaiPhauThuatThuThuat(workSheet.Cells[i, 9].Value.ToString()) : null;
                        dichVuKyThuatModel.MoTa = workSheet.Cells[i, 10].Value != null ? workSheet.Cells[i, 10].Value.ToString() : "";

                        var dichVuKyThuatThongTinGiaModel = new DichVuKyThuatThongTinGiaViewModel
                        {
                            HangBenhVien = workSheet.Cells[i, 11].Value != null ? GetHangBenhVien(workSheet.Cells[i, 11].Value.ToString()) : null,
                            Gia = decimal.Parse(workSheet.Cells[i, 12].Value.ToString()),
                            TuNgay = (DateTime)workSheet.Cells[i, 13].Value,
                            DenNgay = workSheet.Cells[i, 14].Value != null ? (DateTime)workSheet.Cells[i, 14].Value : (DateTime?)null,
                            ThongTu = workSheet.Cells[i, 15].Value != null ? workSheet.Cells[i, 15].Value.ToString() : null,
                            QuyetDinh = workSheet.Cells[i, 16].Value != null ? workSheet.Cells[i, 16].Value.ToString() : null,
                            MoTa = workSheet.Cells[i, 17].Value != null ? workSheet.Cells[i, 17].Value.ToString() : null,
                            HieuLuc = (workSheet.Cells[i, 18].Value.ToString().Trim().TrimStart().TrimEnd().ToUpper() == ("Có").ToUpper() ? true : false),

                        };
                        var model = listModel.FirstOrDefault(s => s.MaChung == dichVuKyThuatModel.MaChung);
                        if (model != null)
                        {
                            var listDichVuKyThuatThongTinGia = model.DichVuKyThuatThongTinGias;
                            model = dichVuKyThuatModel;

                            listDichVuKyThuatThongTinGia.Add(dichVuKyThuatThongTinGiaModel);
                            model.DichVuKyThuatThongTinGias = listDichVuKyThuatThongTinGia;
                        }
                        else
                        {
                            dichVuKyThuatModel.DichVuKyThuatThongTinGias = new List<DichVuKyThuatThongTinGiaViewModel>();
                            dichVuKyThuatModel.DichVuKyThuatThongTinGias.Add(dichVuKyThuatThongTinGiaModel);
                            listModel.Add(dichVuKyThuatModel);
                        }

                    }
                }
            }
            foreach (var item in listModel)
            {
                var dichVuKyThuat = _dichVuKyThuatService.GetDVKTByMa(item.MaChung);
                if (dichVuKyThuat != null)
                {
                    item.Id = dichVuKyThuat.Id;
                    item.ToEntity(dichVuKyThuat);
                    foreach (var thongTinGia in dichVuKyThuat.DichVuKyThuatThongTinGias)
                    {
                        thongTinGia.WillDelete = true;
                    }

                    foreach (var thongTinGiaModel in item.DichVuKyThuatThongTinGias)
                    {
                        dichVuKyThuat.DichVuKyThuatThongTinGias.Add(new Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatThongTinGia
                        {
                            HangBenhVien = thongTinGiaModel.HangBenhVien,
                            Gia = thongTinGiaModel.Gia.GetValueOrDefault(),
                            TuNgay = thongTinGiaModel.TuNgay.GetValueOrDefault(),
                            DenNgay = thongTinGiaModel.DenNgay,
                            ThongTu = thongTinGiaModel.ThongTu,
                            QuyetDinh = thongTinGiaModel.QuyetDinh,
                            MoTa = thongTinGiaModel.MoTa,
                            HieuLuc = thongTinGiaModel.HieuLuc,
                        });
                    }
                    _dichVuKyThuatService.Update(dichVuKyThuat);
                }
                else
                {
                    dichVuKyThuat = item.ToEntity<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat>();
                    foreach (var thongTinGiaModel in item.DichVuKyThuatThongTinGias)
                    {
                        dichVuKyThuat.DichVuKyThuatThongTinGias.Add(new Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatThongTinGia
                        {
                            HangBenhVien = thongTinGiaModel.HangBenhVien,
                            Gia = thongTinGiaModel.Gia.GetValueOrDefault(),
                            TuNgay = thongTinGiaModel.TuNgay.GetValueOrDefault(),
                            DenNgay = thongTinGiaModel.DenNgay,
                            ThongTu = thongTinGiaModel.ThongTu,
                            QuyetDinh = thongTinGiaModel.QuyetDinh,
                            MoTa = thongTinGiaModel.MoTa,
                            HieuLuc = thongTinGiaModel.HieuLuc,
                        });
                    }
                    _dichVuKyThuatService.Add(dichVuKyThuat);
                }
            }

            if (listError.Any())
                listError[0].TotalThanhCong = listModel.Count;
            else
                listError.Add(new DichVuKyThuatViewModelError { TotalThanhCong = listModel.Count });

            return Ok(listError);
        }

        private EnumDanhMucNhomTheoChiPhi GetNhomChiPhi(string tenNhomChiPhi)
        {
            if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.XetNghiem.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.XetNghiem;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThamDoChucNang.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.ThamDoChucNang;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThuocDieuTriUngThuVaChongThaiGhepNgoaiDanhMuc.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.ThuocDieuTriUngThuVaChongThaiGhepNgoaiDanhMuc;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThuocThanhToanTheoTyLe.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.ThuocThanhToanTheoTyLe;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.MauVaChePhamMau.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.MauVaChePhamMau;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.ThuThuatPhauThuat.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.ThuThuatPhauThuat;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.VTYTThanhToanTheoTyLe.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.VTYTThanhToanTheoTyLe;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.VanChuyen.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.VanChuyen;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.KhamBenh.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.KhamBenh;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNgoaiTru.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNgoaiTru;
            else if (tenNhomChiPhi == EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNoiTru.GetDescription())
                return EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNoiTru;
            else
                return EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNoiTru;
        }

        private LoaiPhauThuatThuThuat? GetLoaiPhauThuatThuThuat(string tenLoaiPhauThuatThuThuat)
        {
            if (tenLoaiPhauThuatThuThuat == LoaiPhauThuatThuThuat.ThuThuatLoai1.GetDescription())
                return LoaiPhauThuatThuThuat.ThuThuatLoai1;
            else if (tenLoaiPhauThuatThuThuat == LoaiPhauThuatThuThuat.ThuThuatLoai2.GetDescription())
                return LoaiPhauThuatThuThuat.ThuThuatLoai2;
            else if (tenLoaiPhauThuatThuThuat == LoaiPhauThuatThuThuat.ThuThuatLoai3.GetDescription())
                return LoaiPhauThuatThuThuat.ThuThuatLoai3;
            else if (tenLoaiPhauThuatThuThuat == LoaiPhauThuatThuThuat.ThuThuatLoaiDacBiet.GetDescription())
                return LoaiPhauThuatThuThuat.ThuThuatLoaiDacBiet;
            else if (tenLoaiPhauThuatThuThuat == LoaiPhauThuatThuThuat.PhauThuatLoai1.GetDescription())
                return LoaiPhauThuatThuThuat.PhauThuatLoai1;
            else if (tenLoaiPhauThuatThuThuat == LoaiPhauThuatThuThuat.PhauThuatLoai2.GetDescription())
                return LoaiPhauThuatThuThuat.PhauThuatLoai2;
            else if (tenLoaiPhauThuatThuThuat == LoaiPhauThuatThuThuat.PhauThuatLoai3.GetDescription())
                return LoaiPhauThuatThuThuat.PhauThuatLoai3;
            else if (tenLoaiPhauThuatThuThuat == LoaiPhauThuatThuThuat.PhauThuatLoaiDacBiet.GetDescription())
                return LoaiPhauThuatThuThuat.PhauThuatLoaiDacBiet;
            else
                return null;
        }

        private long GetNhomDichVuKyThuat(string tenNhom)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenNhom;
            requestModel.Take = 10;

            return _dichVuKyThuatService.GetListNhomDichVuKyThuat(requestModel).Result.FirstOrDefault().KeyId;

        }

        private HangBenhVien? GetHangBenhVien(string tenHangBenhVien)
        {
            if (tenHangBenhVien == HangBenhVien.BenhVienHang1.GetDescription())
                return HangBenhVien.BenhVienHang1;
            else if (tenHangBenhVien == HangBenhVien.BenhVienHang2.GetDescription())
                return HangBenhVien.BenhVienHang2;
            else if (tenHangBenhVien == HangBenhVien.BenhVienHang3.GetDescription())
                return HangBenhVien.BenhVienHang3;
            else if (tenHangBenhVien == HangBenhVien.BenhVienHang4.GetDescription())
                return HangBenhVien.BenhVienHang4;
            else if (tenHangBenhVien == HangBenhVien.BenhVienHangDacBiet.GetDescription())
                return HangBenhVien.BenhVienHangDacBiet;
            else if (tenHangBenhVien == HangBenhVien.BenhVienChuaXepHang.GetDescription())
                return HangBenhVien.BenhVienHang2;
            else
                return null;
        }
        #endregion

    }
}