using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DonViTinh;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Helpers;
using Camino.Services.DonViTinh;
using Camino.Services.DuocPhamBenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.Thuocs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class DuocPhamController : CaminoBaseController
    {
        private readonly IDuocPhamService _thuocBenhVienService;
        private readonly IDuocPhamBenhVienService _duocPhamBenhVienService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IDonViTinhService _donViTinhService;

        public DuocPhamController(IDuocPhamService thuocBenhVienService, IDuocPhamBenhVienService duocPhamBenhVienService, ILocalizationService localizationService, IExcelService excelService, ITaiLieuDinhKemService taiLieuDinhKemService, IDonViTinhService donViTinhService)
        {
            _thuocBenhVienService = thuocBenhVienService;
            _duocPhamBenhVienService = duocPhamBenhVienService;
            _localizationService = localizationService;
            _excelService = excelService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _donViTinhService = donViTinhService;
        }

        #region danh sách thuốc bệnh viện
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {

            var gridData = await _thuocBenhVienService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _thuocBenhVienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region get thông tin chung 
        [HttpPost("GetDanhSachLoaiThuocHoacHoatChat")]
        public List<LookupItemVo> GetDanhSachLoaiThuocHoacHoatChat(DropDownListRequestModel queryInfo)
        {
            return _thuocBenhVienService.GetDanhSachLoaiThuocHoacHoatChat(queryInfo);
        }

        [HttpPost("GetDanhSachDuongDung")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanhSachDuongDungAsync([FromBody]DropDownListRequestModel model)
        {
            var lstDuongDung = await _thuocBenhVienService.GetDanhSachDuongDungAsync(model);
            return Ok(lstDuongDung);
        }

        [HttpGet("GetListTenNhaSanXuatAsync")]
        public async Task<ICollection<string>> GetListTenNhaSanXuatAsync()
        {
            var lstNhaSanXuat = await _thuocBenhVienService.GetListTenNhaSanXuatAsync();
            return lstNhaSanXuat;
        }

        [HttpGet("GetListTenNuocSanXuatAsync")]
        public async Task<ICollection<string>> GetListTenNuocSanXuatAsync()
        {
            var lstNuocSanXuat = await _thuocBenhVienService.GetListTenNuocSanXuatAsync();
            return lstNuocSanXuat;
        }
        [HttpGet("GetListHoatChatAsync")]
        public async Task<ICollection<MaHoatChatHoatChatDuongDungTemplateVo>> GetListTenMaHoatChatAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lstNuocSanXuat = await _thuocBenhVienService.GetListTenMaHoatChatAsync(queryInfo);
            return lstNuocSanXuat;
        }

        [HttpGet("GetListTenHoatChatVaDuongDungAsync")]
        public async Task<ICollection<string>> GetListTenHoatChatVaDuongDungAsync()
        {
            var lstThuocHoacHoatChat = await _thuocBenhVienService.GetListTenHoatChatVaDuongDungAsync();
            return lstThuocHoacHoatChat;
        }
        [HttpGet("GetTenHoatChatAsync")]
        public async Task<string> GetTenHoatChatAsync(string tenHoatChatDuongDung)
        {
            var lstThuocHoacHoatChat = await _thuocBenhVienService.GetTenHoatChatAsync(tenHoatChatDuongDung);
            return lstThuocHoacHoatChat;
        }
        #endregion

        #region xử lý thuốc bệnh viện
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDuocPham)]
        public async Task<ActionResult<DuocPhamViewModel>> PostAsync([FromBody] DuocPhamViewModel thuocBenhVienViewModel)
        {
            if (!await _thuocBenhVienService.CheckDuongDungAsync(thuocBenhVienViewModel.DuongDungId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DuongDung.NotExists"), (int)HttpStatusCode.BadRequest);
            if (!await _thuocBenhVienService.CheckDVTAsync(thuocBenhVienViewModel.DonViTinhId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DVT.NotExists"), (int)HttpStatusCode.BadRequest);
            var thuocBenhVien = thuocBenhVienViewModel.ToEntity<DuocPham>();
            //if (thuocBenhVienViewModel.SuDungThuocBenhVien == true && thuocBenhVienViewModel.HieuLuc == true) <= code tào lao
            if (thuocBenhVienViewModel.SuDungThuocBenhVien == true && thuocBenhVienViewModel.IsDisabled != true)
            {
                var duocPhamBenhVien = new DuocPhamBenhVien
                {
                    DieuKienBaoHiemThanhToan = thuocBenhVienViewModel.DieuKienBaoHiemThanhToan,
                    //HieuLuc = thuocBenhVienViewModel.HieuLuc, <= code tào lao
                    HieuLuc = true,
                    DuocPhamBenhVienPhanNhomId = thuocBenhVienViewModel.DuocPhamBenhVienPhanNhomId,
                    MaDuocPhamBenhVien = thuocBenhVienViewModel.MaDuocPhamBenhVien
                };
                thuocBenhVien.DuocPhamBenhVien = duocPhamBenhVien;
            }
            _thuocBenhVienService.Add(thuocBenhVien);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuocPhamViewModel>> Get(long id)
        {
            var result = await _thuocBenhVienService.GetByIdAsync(id, s => s.Include(x => x.DonViTinh).Include(x => x.DuongDung).Include(k => k.DuocPhamBenhVien).ThenInclude(g => g.DuocPhamBenhVienPhanNhom));
            //var result = await _thuocBenhVienService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var r = result.ToModel<DuocPhamViewModel>();
            r.TenDonViTinh = result.DonViTinh.Ten;
            r.TenDuongDung = result.DuongDung.Ten;
            r.IsDisabled = result.IsDisabled ?? false;
            if (result.DuocPhamBenhVien != null)
            {
                r.SuDungThuocBenhVien = true;
                r.DieuKienBaoHiemThanhToan = result.DuocPhamBenhVien.DieuKienBaoHiemThanhToan;
                r.DuocPhamBenhVienPhanNhomId = result.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId;
                r.MaDuocPhamBenhVien = result.DuocPhamBenhVien.Ma;
                r.DuocPhamBenhVienPhanNhomModelText = result.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom?.Ten;

                //BVHD-3454
                r.DuocPhamBenhVienModel = new DuocPhamBenhVienModel();
                r.DuocPhamBenhVienModel.Id = result.DuocPhamBenhVien.Id;
                r.ChuaTaoDuocPhamBenhVien = false;
            }
            return Ok(r);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDuocPham)]
        public async Task<ActionResult> Put([FromBody] DuocPhamViewModel thuocBenhVienViewModel)
        {
            if (!await _thuocBenhVienService.CheckDuongDungAsync(thuocBenhVienViewModel.DuongDungId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DuongDung.NotExists"), (int)HttpStatusCode.BadRequest);
            if (!await _thuocBenhVienService.CheckDVTAsync(thuocBenhVienViewModel.DonViTinhId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DVT.NotExists"), (int)HttpStatusCode.BadRequest);

            var thuocBenhVien = await _thuocBenhVienService.GetByIdAsync(thuocBenhVienViewModel.Id, s => s.Include(l => l.DuocPhamBenhVien));
            if (thuocBenhVienViewModel.SuDungThuocBenhVien == true && thuocBenhVien.DuocPhamBenhVien != null)
            {
                thuocBenhVien.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId = thuocBenhVienViewModel.DuocPhamBenhVienPhanNhomId;
                thuocBenhVien.DuocPhamBenhVien.MaDuocPhamBenhVien = thuocBenhVienViewModel.MaDuocPhamBenhVien;
                thuocBenhVien.DuocPhamBenhVien.DieuKienBaoHiemThanhToan = thuocBenhVienViewModel.DieuKienBaoHiemThanhToan;
            }
            if (thuocBenhVienViewModel.SuDungThuocBenhVien == true && thuocBenhVien.DuocPhamBenhVien == null)
            {
                //if (thuocBenhVienViewModel.SuDungThuocBenhVien == true && thuocBenhVienViewModel.HieuLuc == true) <= code tào lao
                if (thuocBenhVienViewModel.SuDungThuocBenhVien == true && thuocBenhVienViewModel.IsDisabled != true)
                {
                    var duocPhamBenhVien = new DuocPhamBenhVien
                    {
                        DieuKienBaoHiemThanhToan = thuocBenhVienViewModel.DieuKienBaoHiemThanhToan,
                        //HieuLuc = thuocBenhVienViewModel.HieuLuc, <= code tào lao
                        HieuLuc = true,
                        DuocPhamBenhVienPhanNhomId = thuocBenhVienViewModel.DuocPhamBenhVienPhanNhomId,
                        MaDuocPhamBenhVien = thuocBenhVienViewModel.MaDuocPhamBenhVien
                    };
                    thuocBenhVien.DuocPhamBenhVien = duocPhamBenhVien;
                }
            }
            if (thuocBenhVien == null)
            {
                return NotFound();
            }
            thuocBenhVienViewModel.ToEntity(thuocBenhVien);
            if (thuocBenhVienViewModel.SuDungThuocBenhVien == false)
            {
                if (thuocBenhVien.DuocPhamBenhVien != null)
                {
                    await _duocPhamBenhVienService.DeleteByIdAsync(thuocBenhVien.DuocPhamBenhVien.Id);
                }
            }
            await _thuocBenhVienService.UpdateAsync(thuocBenhVien);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDuocPham)]
        public async Task<ActionResult> Delete(long id)
        {
            var thuocBenhVien = await _thuocBenhVienService.GetByIdAsync(id);
            if (thuocBenhVien == null)
            {
                return NotFound();
            }

            await _thuocBenhVienService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDonViTinh)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var thuocBenhViens = await _thuocBenhVienService.GetByIdsAsync(model.Ids);
            if (thuocBenhViens == null)
            {
                return NotFound();
            }
            if (thuocBenhViens.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _thuocBenhVienService.DeleteAsync(thuocBenhViens);
            return NoContent();
        }
        #endregion

        #region GetListLookupItemVo

        [HttpPost("GetListLookupDuocPham")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLookupDuocPham(DropDownListRequestModel model)
        {
            var lookup = await _thuocBenhVienService.GetListLookupDuocPham(model);
            return Ok(lookup);

        }

        [HttpPost("GetListNhaSanXuat")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhaSanXuat(DropDownListRequestModel model)
        {
            var lookup = await _thuocBenhVienService.GetListNhaSanXuat(model);
            return Ok(lookup);
        }
        #endregion

        [HttpPost("ExportDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDuocPham)]
        public async Task<ActionResult> ExportDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _thuocBenhVienService.GetDataForGridAsync(queryInfo, true);
            var duocPhamData = gridData.Data.Select(p => (DuocPhamGridVo)p).ToList();
            var excelData = duocPhamData.Map<List<DuocPhamExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DuocPhamExportExcel.Ten), "Tên dược phẩm"));
            lstValueObject.Add((nameof(DuocPhamExportExcel.SoDangKy), "Số đăng ký"));
            lstValueObject.Add((nameof(DuocPhamExportExcel.MaHoatChat), "Mã hoạt chất"));
            lstValueObject.Add((nameof(DuocPhamExportExcel.HoatChat), "Hoạt chất"));
            lstValueObject.Add((nameof(DuocPhamExportExcel.QuyCach), "Đơn vị sơ cấp"));
            lstValueObject.Add((nameof(DuocPhamExportExcel.TenDonViTinh), "Đơn vị thứ cấp"));
            lstValueObject.Add((nameof(DuocPhamExportExcel.HeSoDinhMucDonViTinh), "Hệ số định mức"));
            lstValueObject.Add((nameof(DuocPhamExportExcel.TenDuongDung), "Đường dùng"));
            lstValueObject.Add((nameof(DuocPhamExportExcel.TenLoaiThuocHoacHoatChat), "Loại thuốc"));
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Dược phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        #region //BVHD-3454
        [HttpPost("KiemTraTrungDuocPhamBenhVien")]
        public async Task<ActionResult<bool>> KiemTraTrungDuocPhamBenhVienAsync([FromBody] DuocPhamViewModel duocPhamViewModel)
        {
            var duocPham = new DuocPham
            {
                Ten = duocPhamViewModel.Ten,
                SoDangKy = duocPhamViewModel.SoDangKy,
                MaHoatChat = duocPhamViewModel.MaHoatChat,
                HoatChat = duocPhamViewModel.HoatChat,
                NhaSanXuat = duocPhamViewModel.NhaSanXuat,
                NuocSanXuat = duocPhamViewModel.NuocSanXuat,
                DuongDungId = duocPhamViewModel.DuongDungId.GetValueOrDefault(),
                DonViTinhId = duocPhamViewModel.DonViTinhId.GetValueOrDefault(),
                HamLuong = duocPhamViewModel.HamLuong,
                TheTich = duocPhamViewModel.TheTich
            };
            var kiemtra = await _duocPhamBenhVienService.KiemTraTrungDuocPhamBenhVienAsync(duocPham);
            return Ok(kiemtra);
        }
        #endregion
        [HttpPost("DownloadTemplateDuocPham")]
        public ActionResult DownloadTemplateDuocPham()
        {
            var path = @"Resource\\TemplateDuocPham.xlsx";
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);

            long byteLength = new System.IO.FileInfo(path).Length;
            var fileContent = binaryReader.ReadBytes((Int32)byteLength);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TemplateDuocPham.xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(fileContent, "application/vnd.ms-excel");
        }


        [HttpGet("ImportDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDuocPham)]
        public ActionResult<bool> ImportDuocPham(string tenGuid, string duongDan)
        {
            List<DuocPhamViewModel> listDuocPham = new List<DuocPhamViewModel>();
            List<DanhSachNhapDuocPhamExcelError> listError = new List<DanhSachNhapDuocPhamExcelError>();
            var path = _taiLieuDinhKemService.GetObjectStream(duongDan, tenGuid);
            using (var package = new ExcelPackage(path))
            {
                var workSheet = package.Workbook.Worksheets["Dược phẩm"];
                if (workSheet == null)
                {
                    throw new ApiException("Thông tin file dược phẩm không đúng");
                }
                int totalRows = workSheet.Dimension.Rows;
                if (totalRows >= 2)
                {
                    for (int i = 2; i <= totalRows; i++)
                    {
                        DanhSachNhapDuocPhamExcelError errors = new DanhSachNhapDuocPhamExcelError();
                        var duocPhamModel = new DuocPhamViewModel();

                        if (workSheet.Cells[i, 1].Value == null
                            || workSheet.Cells[i, 7].Value == null
                            || workSheet.Cells[i, 10].Value == null
                            || workSheet.Cells[i, 13].Value == null)
                        {
                            var infoTen = workSheet.Cells[i, 1].Value == null ? _localizationService.GetResource("Common.Ten.Required") + "|" : "";
                            var infoLoaiThuocHoacHoatChat = workSheet.Cells[i, 7].Value == null ? _localizationService.GetResource("ThuocBenhVien.LoaiThuocHoacHoatChat.Reqiured") + "|" : "";
                            var infoDuongDung = workSheet.Cells[i, 10].Value == null ? _localizationService.GetResource("ThuocBenhVien.DuongDung.Required") + "|" : "";
                            var infoDonViTinh = workSheet.Cells[i, 13].Value == null ? _localizationService.GetResource("ThuocBenhVien.DonViTinh.Required") + "|" : "";
                            
                            listError.Add(new DanhSachNhapDuocPhamExcelError
                            {
                                Ten = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : ""),
                                Error = infoTen + infoLoaiThuocHoacHoatChat + infoDuongDung + infoDonViTinh
                            });
                            continue;
                        }

                        var suDungThuocBV = workSheet.Cells[i, 28].Value != null ? (workSheet.Cells[i, 28].Value.ToString().Trim().TrimStart().TrimEnd().ToUpper() == ("Có").ToUpper() ? true : false) : false;
                        var hieuLuc = workSheet.Cells[i, 18].Value != null ? (workSheet.Cells[i, 18].Value.ToString().Trim().TrimStart().TrimEnd().ToUpper() == ("Có").ToUpper() ? true : false) : false;
                        if (suDungThuocBV==true)
                        {
                            if(workSheet.Cells[i, 29].Value == null
                            || workSheet.Cells[i, 30].Value == null)
                            {
                                var infoMaDuocPhamBV = workSheet.Cells[i, 29].Value == null ? _localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.Required") + "|" : "";
                                var infoDuocPhamBenhVienPhanNhom = workSheet.Cells[i, 30].Value == null ? _localizationService.GetResource("DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId.Required") + "|" : "";
                                listError.Add(new DanhSachNhapDuocPhamExcelError
                                {
                                    Ten = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : ""),
                                    Error = infoMaDuocPhamBV + infoDuocPhamBenhVienPhanNhom
                                });
                                continue;
                            }
                        }

                        var duongDung = GetDanhSachDuongDung(workSheet.Cells[i, 10].Value.ToString());
                        if(duongDung == null)
                        {
                            listError.Add(new DanhSachNhapDuocPhamExcelError
                            {
                                Ten = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : ""),
                                Error = "Đường dùng không tồn tại"
                            });
                            continue;
                        }

                        var DVT = GetDanhSachDVT(workSheet.Cells[i, 13].Value.ToString());
                        if(DVT == null)
                        {
                            listError.Add(new DanhSachNhapDuocPhamExcelError
                            {
                                Ten = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : ""),
                                Error = "Đơn vị tính không tồn tại"
                            });
                            continue;
                        }
                        NhomDichVuBenhVienPhanNhomTreeViewVo dVBVPhanNhom = new NhomDichVuBenhVienPhanNhomTreeViewVo();
                        if(workSheet.Cells[i, 30].Value != null)
                        {
                            dVBVPhanNhom = GetListDichVuBenhVienPhanNhomAsync(workSheet.Cells[i, 30].Value.ToString());
                            if (dVBVPhanNhom == null)
                            {
                                listError.Add(new DanhSachNhapDuocPhamExcelError
                                {
                                    Ten = (workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : ""),
                                    Error = "Dược phẩm bệnh viện phân nhóm không tồn tại"
                                });
                                continue;
                            }
                        }
                      
                        duocPhamModel.Ten = workSheet.Cells[i, 1].Value.ToString();
                        duocPhamModel.TenTiengAnh = workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : null;
                        duocPhamModel.SoDangKy = workSheet.Cells[i, 3].Value != null ? workSheet.Cells[i, 3].Value.ToString() : null;
                        duocPhamModel.STTHoatChat = workSheet.Cells[i, 4].Value != null ? int.Parse(workSheet.Cells[i, 4].Value.ToString()): (int?)null;
                        duocPhamModel.MaHoatChat = workSheet.Cells[i, 5].Value != null ? workSheet.Cells[i, 5].Value.ToString() : null;
                        duocPhamModel.HoatChat = workSheet.Cells[i, 6].Value != null ? workSheet.Cells[i, 6].Value.ToString() : null;
                        duocPhamModel.LoaiThuocHoacHoatChat = GetLoaiThuocHoacHoatChatEnum(workSheet.Cells[i, 7].Value.ToString());
                        //var NhaSanXuatId = GetDanhSachNhaSanXuat(workSheet.Cells[i, 8].Value.ToString());
                        duocPhamModel.NhaSanXuat = workSheet.Cells[i, 8].Value != null ? workSheet.Cells[i, 8].Value.ToString() : null;
                        duocPhamModel.NuocSanXuat = workSheet.Cells[i, 9].Value != null ? workSheet.Cells[i, 9].Value.ToString() : null;
                        duocPhamModel.DuongDungId = duongDung.KeyId;

                        duocPhamModel.HamLuong = workSheet.Cells[i, 11].Value != null ? workSheet.Cells[i, 11].Value.ToString() : null;
                        duocPhamModel.QuyCach = workSheet.Cells[i, 12].Value != null ? workSheet.Cells[i, 12].Value.ToString() : null;
                        duocPhamModel.DonViTinhId = DVT.KeyId;
                        duocPhamModel.HeSoDinhMucDonViTinh = workSheet.Cells[i, 14].Value != null ? int.Parse(workSheet.Cells[i, 14].Value.ToString()) : (int?)null;
                        duocPhamModel.TieuChuan = workSheet.Cells[i, 15].Value != null ? workSheet.Cells[i, 15].Value.ToString() : null;
                        duocPhamModel.DangBaoChe = workSheet.Cells[i, 16].Value != null ? workSheet.Cells[i, 16].Value.ToString() : null;
                        duocPhamModel.TheTich = workSheet.Cells[i, 17].Value != null ? int.Parse(workSheet.Cells[i, 17].Value.ToString()) : (int?)null;

                        duocPhamModel.HieuLuc = hieuLuc;
                        duocPhamModel.HuongDan = workSheet.Cells[i, 19].Value != null ? workSheet.Cells[i, 19].Value.ToString() : null;
                        duocPhamModel.MoTa = workSheet.Cells[i, 20].Value != null ? workSheet.Cells[i, 20].Value.ToString() : null;
                        duocPhamModel.ChiDinh = workSheet.Cells[i, 21].Value != null ? workSheet.Cells[i, 21].Value.ToString() : null;
                        duocPhamModel.ChongChiDinh = workSheet.Cells[i, 22].Value != null ? workSheet.Cells[i, 22].Value.ToString() : null;
                        duocPhamModel.LieuLuongCachDung = workSheet.Cells[i, 23].Value != null ? workSheet.Cells[i, 23].Value.ToString() : null;
                        duocPhamModel.TacDungPhu = workSheet.Cells[i, 24].Value != null ? workSheet.Cells[i, 24].Value.ToString() : null;
                        duocPhamModel.ChuYDePhong = workSheet.Cells[i, 25].Value != null ? workSheet.Cells[i, 25].Value.ToString() : null;
                        duocPhamModel.LaThucPhamChucNang = workSheet.Cells[i, 26].Value != null ? (workSheet.Cells[i, 26].Value.ToString().Trim().TrimStart().TrimEnd().ToUpper() == ("Có").ToUpper() ? true : false) : (bool?)null;
                        duocPhamModel.LaThuocHuongThanGayNghien = workSheet.Cells[i, 27].Value != null ? (workSheet.Cells[i, 27].Value.ToString().Trim().TrimStart().TrimEnd().ToUpper() == ("Có").ToUpper() ? true : false) : (bool?)null;
                        duocPhamModel.SuDungThuocBenhVien = suDungThuocBV;
                        duocPhamModel.MaDuocPhamBenhVien = workSheet.Cells[i, 29].Value != null ? workSheet.Cells[i, 29].Value.ToString() : null;
                        duocPhamModel.DuocPhamBenhVienPhanNhomId = dVBVPhanNhom!= null ? dVBVPhanNhom.KeyId : 0;
                        duocPhamModel.DieuKienBaoHiemThanhToan = workSheet.Cells[i, 31].Value != null ? workSheet.Cells[i, 31].Value.ToString() : null;

                        listDuocPham.Add(duocPhamModel);


                    }
                }
                else
                {
                    throw new ApiException(_localizationService.GetResource("Thông tin file dược phẩm yâu cầu nhập"));

                }
            }

            int totalThanhCong = 0;
            foreach(var item in listDuocPham)
            {
                var ktSoDangKyTonTai = _thuocBenhVienService.KiemTraSoDangKyTonTaiAsync(item.SoDangKy,item.Id);
                if (ktSoDangKyTonTai.Result == true)
                {
                    listError.Add(new DanhSachNhapDuocPhamExcelError
                    {
                        Ten = item.Ten,
                        Error = "Số đăng ký " + item.SoDangKy + " đã tồn tại trong hệ thống."
                    });
                    continue;
                }

                var duocPham = item.ToEntity<DuocPham>();
                if (item.SuDungThuocBenhVien == true && item.HieuLuc == true)
                {
                    var duocPhamBV = new DuocPhamBenhVien
                    {
                        DieuKienBaoHiemThanhToan = item.DieuKienBaoHiemThanhToan,
                        HieuLuc = item.HieuLuc,
                        DuocPhamBenhVienPhanNhomId = item.DuocPhamBenhVienPhanNhomId,
                        MaDuocPhamBenhVien = item.MaDuocPhamBenhVien
                    };
                    duocPham.DuocPhamBenhVien = duocPhamBV;
                }
                _thuocBenhVienService.Add(duocPham);
                totalThanhCong++;

            }
            if (listError.Any())
                listError[0].TotalThanhCong = totalThanhCong;
            else
                listError.Add(new DanhSachNhapDuocPhamExcelError { TotalThanhCong = totalThanhCong });

            return Ok(listError);
        }

        

        private DonViTinhTemplateVo GetDanhSachDVT(string tenDVT)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenDVT;
            requestModel.Take = 10;
            return _donViTinhService.GetDanhSachDonViTinhAsync(requestModel).Result.FirstOrDefault();

        }
        private DuongDungTemplateVo GetDanhSachDuongDung(string tenDuongDung)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = tenDuongDung;
            requestModel.Take = 10;
            return _thuocBenhVienService.GetDanhSachDuongDungAsync(requestModel).Result.FirstOrDefault();

        }

        private NhomDichVuBenhVienPhanNhomTreeViewVo GetListDichVuBenhVienPhanNhomAsync(string ten)
        {
            DropDownListRequestModel requestModel = new DropDownListRequestModel();
            requestModel.Query = ten;
            requestModel.Take = 10;
            return _duocPhamBenhVienService.GetListDichVuBenhVienPhanNhomAsync(requestModel).Result.FirstOrDefault();

        }

        private LoaiThuocHoacHoatChat GetLoaiThuocHoacHoatChatEnum(string tenLoaiThuocHoacHoatChat)
        {
            if (tenLoaiThuocHoacHoatChat == LoaiThuocHoacHoatChat.ThuocTanDuoc.GetDescription())
                return LoaiThuocHoacHoatChat.ThuocTanDuoc;
            else if (tenLoaiThuocHoacHoatChat == LoaiThuocHoacHoatChat.ChePham.GetDescription())
                return LoaiThuocHoacHoatChat.ChePham;
            else if (tenLoaiThuocHoacHoatChat == LoaiThuocHoacHoatChat.ViThuoc.GetDescription())
                return LoaiThuocHoacHoatChat.ViThuoc;
            else if (tenLoaiThuocHoacHoatChat == LoaiThuocHoacHoatChat.PhongXa.GetDescription())
                return LoaiThuocHoacHoatChat.PhongXa;
            else if (tenLoaiThuocHoacHoatChat == LoaiThuocHoacHoatChat.TanDuocTuBaoChe.GetDescription())
                return LoaiThuocHoacHoatChat.TanDuocTuBaoChe;
            else if (tenLoaiThuocHoacHoatChat == LoaiThuocHoacHoatChat.ChePhamTuBaoChe.GetDescription())
                return LoaiThuocHoacHoatChat.ChePhamTuBaoChe;
            else
                return LoaiThuocHoacHoatChat.ChePhamTuBaoChe;
        }

    }
}
